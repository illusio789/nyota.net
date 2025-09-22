// File adapters (policy, universe, portfolio, ledger, journal, reports, run-config)

using Nyota.Abstractions;
using Nyota.Core;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Nyota.Storage.File
{
// using YamlDotNet.Serialization;
// using YamlDotNet.Serialization.NamingConventions;

    public sealed class FilePolicyRepository : IPolicyRepository
    {
        private readonly string _path;

        private readonly IDeserializer _yaml = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        private readonly ISerializer _yamlSer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        // Accepts either:
        // A) { policy: { risk:..., governance:..., execution:... } }
        // B) { risk:..., governance:..., execution:... }
        private sealed class PolicyWrapper
        {
            public PolicyInner? policy { get; set; }
        }

        private sealed class PolicyInner
        {
            public List<string> allowed_classes { get; set; } = new();
            public List<string> venues_whitelist { get; set; } = new();
            public RiskDto risk { get; set; } = new();
            public GovDto governance { get; set; } = new();
            public ExecDto execution { get; set; } = new();
            public string? version { get; set; }
        }

        private sealed class RiskDto
        {
            public decimal max_position_notional_pct { get; set; }
            public decimal max_asset_class_notional_pct { get; set; }
            public decimal max_daily_turnover_pct { get; set; }
            public decimal min_avg_daily_volume_usd { get; set; }
            public int max_spread_bps { get; set; }
            public bool leverage_allowed { get; set; }
            public bool shorting_allowed { get; set; }
        }

        private sealed class GovDto
        {
            public int holding_period_days { get; set; }
            public List<string> blackout_times_utc { get; set; } = new();
            public List<string> restricted_list { get; set; } = new();
        }

        private sealed class ExecDto
        {
            public string default_time_in_force { get; set; } = "DAY";
            public bool eod_batch_for_etfs { get; set; }
        }

        public FilePolicyRepository(string path)
        {
            _path = path;
        }

        public async Task<Policy> GetAsync(CancellationToken ct)
        {
            var yaml = await System.IO.File.ReadAllTextAsync(_path, ct);

            PolicyInner inner;
            // Try wrapped first
            try
            {
                var wrapped = _yaml.Deserialize<PolicyWrapper>(yaml);
                if (wrapped?.policy is not null)
                    inner = wrapped.policy;
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
                inner.risk.max_position_notional_pct,
                inner.risk.max_asset_class_notional_pct,
                inner.risk.max_daily_turnover_pct,
                inner.risk.min_avg_daily_volume_usd,
                inner.risk.max_spread_bps,
                inner.risk.leverage_allowed,
                inner.risk.shorting_allowed);

            var gov = new Governance(
                inner.governance.holding_period_days,
                inner.governance.blackout_times_utc,
                inner.governance.restricted_list.Select(Symbol.Normalize).ToList());

            var tifOk = Enum.TryParse<TimeInForce>(inner.execution.default_time_in_force, true, out var tif);
            var exec = new ExecutionRules(tifOk ? tif : TimeInForce.DAY, inner.execution.eod_batch_for_etfs);

            return new Policy(
                inner.allowed_classes.Select(ParseClass).ToList(),
                inner.venues_whitelist,
                risk, gov, exec,
                inner.version ?? "1.0.0");
        }

        public async Task SaveAsync(Policy policy, CancellationToken ct)
        {
            var inner = new PolicyInner
            {
                allowed_classes = policy.AllowedClasses.Select(x => x.ToString()).ToList(),
                venues_whitelist = policy.VenuesWhitelist.ToList(),
                risk = new RiskDto
                {
                    max_position_notional_pct = policy.Risk.MaxPositionNotionalPct,
                    max_asset_class_notional_pct = policy.Risk.MaxAssetClassNotionalPct,
                    max_daily_turnover_pct = policy.Risk.MaxDailyTurnoverPct,
                    min_avg_daily_volume_usd = policy.Risk.MinAvgDailyVolumeUsd,
                    max_spread_bps = policy.Risk.MaxSpreadBps,
                    leverage_allowed = policy.Risk.LeverageAllowed,
                    shorting_allowed = policy.Risk.ShortingAllowed
                },
                governance = new GovDto
                {
                    holding_period_days = policy.Governance.HoldingPeriodDays,
                    blackout_times_utc = policy.Governance.BlackoutTimesUtc.ToList(),
                    restricted_list = policy.Governance.RestrictedList.Select(s => s.Value).ToList()
                },
                execution = new ExecDto
                {
                    default_time_in_force = policy.Execution.DefaultTimeInForce.ToString(),
                    eod_batch_for_etfs = policy.Execution.EodBatchForEtfs
                },
                version = policy.Version
            };

            // Always save in the wrapped form for consistency
            var yaml = _yamlSer.Serialize(new PolicyWrapper { policy = inner });
            await System.IO.File.WriteAllTextAsync(_path, yaml, ct);
        }
    }
}
