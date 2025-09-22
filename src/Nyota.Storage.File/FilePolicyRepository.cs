// File adapters (policy, universe, portfolio, ledger, journal, reports, run-config)

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Nyota.Abstractions;
using Nyota.Domain;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Nyota.Storage.File
{
    public sealed class FilePolicyRepository : IPolicyRepository
    {
        private readonly string _path;
        private readonly IFileSystem _fileSystem;

        private readonly IDeserializer _yaml = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        private readonly ISerializer _yamlSerializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        // Accepts either:
        // A) { policy: { risk:..., governance:..., execution:... } }
        // B) { risk:..., governance:..., execution:... }
        private sealed class PolicyWrapper
        {
            public PolicyInner? Policy { get; set; }
        }

        private sealed class PolicyInner
        {
            public List<string> AllowedClasses { get; set; } = new();
            public List<string> VenuesWhitelist { get; set; } = new();
            public RiskDto Risk { get; set; } = new();
            public GovDto Governance { get; set; } = new();
            public ExecDto Execution { get; set; } = new();
            public string? Version { get; set; }
        }

        private sealed class RiskDto
        {
            public decimal MaxPositionNotionalPct { get; set; }
            public decimal MaxAssetClassNotionalPct { get; set; }
            public decimal MaxDailyTurnoverPct { get; set; }
            public decimal MinAvgDailyVolumeUsd { get; set; }
            public int MaxSpreadBps { get; set; }
            public bool LeverageAllowed { get; set; }
            public bool ShortingAllowed { get; set; }
        }

        private sealed class GovDto
        {
            public int HoldingPeriodDays { get; set; }
            public List<string> BlackoutTimesUtc { get; set; } = new();
            public List<string> RestrictedList { get; set; } = new();
        }

        private sealed class ExecDto
        {
            public string DefaultTimeInForce { get; set; } = "DAY";
            public bool EodBatchForEtfs { get; set; }
        }

        public FilePolicyRepository(string path, IFileSystem fileSystem)
        {
            _path = path;
            _fileSystem = fileSystem;
        }

        public async Task<Policy> GetAsync(CancellationToken ct)
        {
            var yaml = await _fileSystem.File.ReadAllTextAsync(_path, ct);

            PolicyInner inner;
            // Try wrapped first
            try
            {
                var wrapped = _yaml.Deserialize<PolicyWrapper>(yaml);
                if (wrapped?.Policy is not null)
                    inner = wrapped.Policy;
                else
                    throw new YamlDotNet.Core.YamlException("Missing 'policy' root.");
            }
            catch
            {
                // Fallback: unwrapped (risk/governance/execution at root)
                inner = _yaml.Deserialize<PolicyInner>(yaml);
            }

            AssetClass ParseClass(string s) => Enum.Parse<AssetClass>(s, true);

            var risk = new RiskLimits(
                inner.Risk.MaxPositionNotionalPct,
                inner.Risk.MaxAssetClassNotionalPct,
                inner.Risk.MaxDailyTurnoverPct,
                inner.Risk.MinAvgDailyVolumeUsd,
                inner.Risk.MaxSpreadBps,
                inner.Risk.LeverageAllowed,
                inner.Risk.ShortingAllowed);

            var gov = new Governance(
                inner.Governance.HoldingPeriodDays,
                inner.Governance.BlackoutTimesUtc,
                inner.Governance.RestrictedList.Select(Symbol.Normalize).ToList());

            var tifOk = Enum.TryParse<TimeInForce>(inner.Execution.DefaultTimeInForce, true, out var tif);
            var exec = new ExecutionRules(tifOk ? tif : TimeInForce.DAY, inner.Execution.EodBatchForEtfs);

            return new Policy(
                inner.AllowedClasses.Select(ParseClass).ToList(),
                inner.VenuesWhitelist,
                risk, gov, exec,
                inner.Version ?? "1.0.0");
        }

        public async Task SaveAsync(Policy policy, CancellationToken ct)
        {
            var inner = new PolicyInner
            {
                AllowedClasses = policy.AllowedClasses.Select(x => x.ToString()).ToList(),
                VenuesWhitelist = policy.VenuesWhitelist.ToList(),
                Risk = new RiskDto
                {
                    MaxPositionNotionalPct = policy.Risk.MaxPositionNotionalPct,
                    MaxAssetClassNotionalPct = policy.Risk.MaxAssetClassNotionalPct,
                    MaxDailyTurnoverPct = policy.Risk.MaxDailyTurnoverPct,
                    MinAvgDailyVolumeUsd = policy.Risk.MinAvgDailyVolumeUsd,
                    MaxSpreadBps = policy.Risk.MaxSpreadBps,
                    LeverageAllowed = policy.Risk.LeverageAllowed,
                    ShortingAllowed = policy.Risk.ShortingAllowed
                },
                Governance = new GovDto
                {
                    HoldingPeriodDays = policy.Governance.HoldingPeriodDays,
                    BlackoutTimesUtc = policy.Governance.BlackoutTimesUtc.ToList(),
                    RestrictedList = policy.Governance.RestrictedList.Select(s => s.Value).ToList()
                },
                Execution = new ExecDto
                {
                    DefaultTimeInForce = policy.Execution.DefaultTimeInForce.ToString(),
                    EodBatchForEtfs = policy.Execution.EodBatchForEtfs
                },
                Version = policy.Version
            };

            // Always save in the wrapped form for consistency
            var yaml = _yamlSerializer.Serialize(new PolicyWrapper { Policy = inner });
            await _fileSystem.File.WriteAllTextAsync(_path, yaml, ct);
        }
    }
}
