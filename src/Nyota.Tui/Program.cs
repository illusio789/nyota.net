using System.Text;

using Nyota.Domain;
using Nyota.Storage.File;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace Nyota.Tui;

class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var dataRoot = args.Length >= 2 && args[0] == "--data" ? args[1] : "./data";
        Directory.CreateDirectory(dataRoot);
        Directory.CreateDirectory(Path.Combine(dataRoot, "universe"));
        Directory.CreateDirectory(Path.Combine(dataRoot, "portfolios"));
        Directory.CreateDirectory(Path.Combine(dataRoot, "reports"));
        var policyPath = Path.Combine(dataRoot, "policy.yaml");
        var journalPath = Path.Combine(dataRoot, "journal.ndjson");

        var policyRepo = new FilePolicyRepository(policyPath);
        var universeRepo = new FileUniverseRepository(Path.Combine(dataRoot, "universe"));
        var portfolioRepo = new FilePortfolioRepository(Path.Combine(dataRoot, "portfolios"));
        var journal = new FileJournal(journalPath);

        Policy? policy = null;
        try { policy = await policyRepo.GetAsync(CancellationToken.None); } catch { }

        var portfolio = await portfolioRepo.GetAsync("default", CancellationToken.None);
        var equityPoints = new List<decimal> { portfolio.Equity.Amount };
        var rng = new Random(42);
        var running = true;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; running = false; };

        AnsiConsole.Live(BuildLayout(policy, portfolio, equityPoints)).AutoClear(false).Start(ctx =>
        {
            while (running)
            {
                var last = equityPoints[^1];
                var step = (decimal)(rng.NextDouble() - 0.48) * 250m;
                equityPoints.Add(Math.Max(last + step, 0m));
                if (equityPoints.Count > 200) equityPoints.RemoveAt(0);

                ctx.UpdateTarget(BuildLayout(policy, portfolio, equityPoints));

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q) running = false;
                Thread.Sleep(120);
            }
        });

        return 0;
    }

    static IRenderable BuildLayout(Policy? policy, Portfolio portfolio, List<decimal> equityPoints)
    {
        var header = new Rule($"[bold]Nyota TUI[/]  ([green]Q[/]=Quit)  [dim]Data:[/] ./data");
        var left = new Panel(new Markup($"[bold]Equity[/]\n{Sparkline(equityPoints)}\n[dim]{equityPoints[^1]:C}[/]"))
            .Header("Equity Curve").Border(BoxBorder.Rounded);
        var right = new Panel(PositionsTable(portfolio)).Header("Positions").Border(BoxBorder.Rounded);
        var top = new Columns(left, right);
        var policyPanel = new Panel(new Markup(policy is null ? "[red]No policy.yaml loaded[/]" : $"[green]Policy version[/]: {policy.Version}"))
            .Header("Compliance").Border(BoxBorder.Rounded);
        return new Rows(header, top, policyPanel);
    }

    static Table PositionsTable(Portfolio p)
    {
        var t = new Table().Border(TableBorder.Rounded);
        t.AddColumn("[bold]Symbol[/]");
        t.AddColumn("[bold]Qty[/]");
        t.AddColumn("[bold]Avg[/]");
        t.AddColumn("[bold]Last[/]");
        t.AddColumn("[bold]PnL[/]");

        if (p.Positions.Count == 0)
        {
            t.AddRow(new List<IRenderable>
            {
                new Markup("[dim]No positions[/]"),
                new Markup("-"),
                new Markup("-"),
                new Markup("-"),
                new Markup("-"),
            });
            return t;
        }

        foreach (var kv in p.Positions)
        {
            var pos = kv.Value;
            var pnl = 0m; // placeholder
            var color = pnl == 0 ? "grey" : (pnl > 0 ? "green" : "red");
            t.AddRow(kv.Key.Value, pos.Qty.Value.ToString("0.####"), pos.AvgPrice.Value.ToString("0.####"),
                pos.AvgPrice.Value.ToString("0.####"), $"[{color}]{pnl:0.##}[/]");
        }
        return t;
    }

    static string Sparkline(IReadOnlyList<decimal> data)
    {
        if (data.Count == 0) return "";
        var blocks = "▁▂▃▄▅▆▇█".ToCharArray();
        var min = data.Min(); var max = data.Max();
        var span = max - min == 0 ? 1 : max - min;
        var sb = new StringBuilder(data.Count);
        foreach (var v in data)
        {
            var idx = (int)Math.Round((v - min) / span * (blocks.Length - 1));
            idx = Math.Clamp(idx, 0, blocks.Length - 1);
            sb.Append(blocks[idx]);
        }
        return sb.ToString();
    }
}