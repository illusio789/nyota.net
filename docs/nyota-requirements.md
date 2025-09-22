# Nyota – Requirements & Entity Definitions (for Code Generation)

## 1) Purpose & Goals
- Build a **compliance-first automated trading platform** that scans markets, generates signals, simulates, and (optionally) executes trades for **allowed instruments only**:  
  **commodity ETFs, currency-pair ETFs, commodities, index-linked instruments (ETFs/options/trackers/futures/warrants), and cryptocurrencies**.
- Emphasize **extensibility** (plug-in strategies, data providers, execution providers) and **auditable compliance** (default-deny, pre-trade checks, immutable journal).

## 2) High-Level Architecture
- **Pipelines per asset class** (Commodity ETFs, Currency ETFs, Commodities, Index-Linked, Crypto), sharing common abstractions.
- **Core services:** Universe/Policy, Market Data, Signal Engine, Compliance, Sizing/Risk, Execution (paper/live), Simulation/Backtest, Journaling/Attestations, Reporting.
- **Default-deny**: nothing proceeds unless explicitly whitelisted by policy + universe.

... (full details as we discussed previously, entities, rules, acceptance criteria) ...


---

# Nyota – TUI Requirements

## Purpose
Provide a **keyboard-first Text User Interface (TUI)** for Nyota to monitor trading runs, equity, compliance, positions, and journal in real time.  
The TUI should be **extensible**, support **widgets**, and handle **real-time charts**.

## Core Views (MVP)
- Dashboard (equity, portfolio, compliance)
- Universe browser
- Signals & orders feed
- Journal explorer
- Reports

## Interaction Model
- Read-only by default
- Config upload
- Run controls
- Export controls
- Keyboard shortcuts (Q quit, R refresh, L clear, F-keys for views)

## Technical Approach
- Spectre.Console
- Widgets (`ITuiWidget`, `ITuiRealtime`)
- Host (`TuiHost` with event bus)
- Extensible pub/sub
- Real-time updates with Unicode charts



---

# Nyota – TUI Folder Structure & Conventions

## Folder Structure
```
/tui
  /Nyota.Tui
    Program.cs
    /Host
    /Widgets
    /Adapters
    /Theme
    /Config
```
(Full details in earlier notes)


---

# Nyota – API & Event Contracts (for TUI Adapters)

## 1) REST Endpoints (HTTP/JSON)
- /api/health
- /api/run/start, stop, status
- /api/equity/series, /api/equity/kpi
- /api/portfolio, /api/positions
- /api/compliance/summary, /api/compliance/recent
- /api/signals/recent
- /api/orders/recent
- /api/journal/recent
- /api/reports/daily
- /api/universe
- /api/policy

## 2) WebSocket Events
- equity.tick
- positions.snapshot
- compliance.summary
- signal
- order
- journal
- batch

## 3) DTO Contracts (C#)
(See earlier section for record definitions)

## 4) Versioning & Stability
- Use version header or prefix
- Never break fields

## 5) Acceptance Criteria
- health returns ok
- equity series monotonic
- positions contain only open
- compliance summary matches recent
- websocket tick >=1Hz
- batch <=256
- UTC timestamps


---

# Nyota – Sample Run Config (JSON)

This shows the structure for launching a simulation, paper trade, or live run.

```json
{
  "mode": "simulate",                       // simulate | paper | live
  "clock": {
    "type": "historical",                   // historical | realtime
    "start": "2024-01-01",
    "end": "2024-12-31"
  },
  "asset_classes": ["commodity_etf","crypto"],
  "strategies": [
    {
      "id": "ma_pullback_1",
      "type": "MovingAveragePullback",
      "params": { "fast": 20, "slow": 100, "atr": 14 }
    },
    {
      "id": "crypto_trend_1",
      "type": "TrendFollow",
      "params": { "lookback": 50, "volScale": true }
    }
  ],
  "data": {
    "provider": "Yahoo",
    "resolution": "D",
    "cache": { "enabled": true }
  },
  "execution": {
    "provider": "Paper",
    "slippage_model": "ProportionalBps",
    "fees_model": "Tiered"
  },
  "reporting": {
    "output_dir": "./reports",
    "generate_html": true
  }
}
```

## Notes
- `mode`: controls whether orders are simulated, routed to a paper engine, or to a real broker API.
- `clock`: in simulation, defines historical period; in live mode, uses realtime clock.
- `strategies`: array of strategy configs, each with an ID, type, and params dict.
- `data`: which market data provider to use (Yahoo, Polygon, Binance, CSV, etc.).
- `execution`: provider and models for fills, slippage, and fees.
- `reporting`: controls where outputs go and whether to generate HTML/PDF dashboards.



---

# Nyota – Sample Policy (YAML)

```yaml
policy:
  allowed_classes:
    - commodity_etf
    - currency_etf
    - commodity
    - index_linked
    - crypto
  venues_whitelist: [LSE, XETRA, NYSE, NASDAQ, CME, ICE, CBOE, Eurex, Binance]
  risk:
    max_position_notional_pct: 0.02
    max_asset_class_notional_pct: 0.40
    max_daily_turnover_pct: 0.10
    min_avg_daily_volume_usd: 2000000
    max_spread_bps: 25
    leverage_allowed: false
    shorting_allowed: false
  governance:
    holding_period_days: 2
    blackout_times_utc: ["21:55-22:10"]
    restricted_list: []
  execution:
    default_time_in_force: "DAY"
    eod_batch_for_etfs: true
```

---

# Nyota – Sample Universe (CSV)

```
symbol,asset_class,venue,base_ccy,quote_ccy,tick_size,lot_size,min_notional,isin,sedol,figi,leverage,notes
GLD,commodity_etf,NYSE,USD,,0.01,1,100,US78463V1070,,BBG000N9MNX3,1,Gold ETF
SLV,commodity_etf,NYSE,USD,,0.01,1,100,US78468R1014,,BBG000Q2MZ46,1,Silver ETF
UUP,currency_etf,NYSE,USD,,0.01,1,100,US73936T6150,,BBG000Q2Y4X8,1,US Dollar Index ETF
BTC-USD,crypto,Binance,USD,,0.01,0.0001,10,,,,1,Bitcoin Spot
```

---

# Nyota – Sample Journal Entry (JSON)

```json
{
  "timeUtc": "2025-09-20T10:21:06Z",
  "level": "Info",
  "category": "Order",
  "entityRefs": { "orderId": "O-001", "symbol": "GLD" },
  "payload": {
    "side": "Buy",
    "qty": 10,
    "avgFill": 191.93,
    "fees": 0.12,
    "provider": "Paper"
  },
  "hash": "ABC123...",
  "prevHash": "XYZ789..."
}
```

---

# Nyota – Developer Workflow / Commands

- `nyota simulate --run ./config/runs/ma-pullback.json`
- `nyota paper --run ./config/runs/crypto-trend.json`
- `nyota live --run ./config/runs/index-linked.json`
- `nyota tui` → launches Spectre.Console dashboard
- `nyota report --date 2025-09-20` → exports daily report

---

# Nyota – Glossary

- **Signal**: Suggestion to trade a specific instrument, emitted by a strategy.
- **Compliance Receipt**: Immutable record of rule checks for a trade decision.
- **Order**: Instruction to execute a trade (Buy/Sell, type, qty).
- **Execution Result**: Confirmation of order acceptance/fill/rejection.
- **Position**: Current holding in an instrument (qty, avg price, PnL).
- **Attestation**: Daily summary of compliance state, positions, and breaches.
- **Journal**: Append-only log of all events (signals, orders, compliance, fills).

---

# Nyota – Extensibility Guidelines

- **Strategies**: Implement `IStrategy` → drop into `/Strategies` folder → register in run config.
- **Rules**: Implement `IRule` → add to ComplianceEngine → appears in compliance receipts.
- **Execution Providers**: Implement `IExecutionProvider` → configure in run config.
- **Market Data Providers**: Implement `IMarketDataProvider` → swap in via config.
- **TUI Widgets**: Implement `ITuiWidget`/`ITuiRealtime` → register in `Program.cs`.
- **Asset Classes**: Extend enum + add to policy/universe schema + update tests.

---

# Nyota – Data Sources Reference

Supported providers for market data:
- **Yahoo Finance** (EOD/daily, ETFs, equities, commodities)
- **Polygon.io** (US equities, ETFs, FX)
- **Binance** (crypto spot/futures)
- **CSV Loader** (custom historical datasets)

Future: add CME, ICE, Quandl, etc.

---

# Nyota – Testing Expectations

- **Unit Tests**: for strategies, compliance rules, sizers, journal hashing.
- **Golden Files**: deterministic sims with fixed seeds → verify equity curve, compliance receipts.
- **Integration Tests**: run sample policy + run config over toy universe; assert correct orders, PnL, compliance results.
- **TUI Smoke Test**: launch `nyota tui` → verify equity sparkline, positions table, compliance chart render.
- **API Contract Tests**: `GET /api/health` returns 200; equity/positions/compliance endpoints return valid DTOs.  
