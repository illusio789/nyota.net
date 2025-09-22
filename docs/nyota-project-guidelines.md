# Nyota Project Guidelines

These guidelines describe coding style, architecture, contribution flow, and general practices for the Nyota trading platform.

---

## 1. Architecture Principles

- **Hexagonal (Ports & Adapters):** Core logic depends only on abstractions (`Nyota.Abstractions`), never directly on file/DB/eventing implementations.
- **Separation of Concerns:**
    - `Nyota.Core`: domain entities and value objects only.
    - `Nyota.Abstractions`: ports (interfaces) for persistence, events, external services.
    - `Nyota.Storage.File`: file-backed implementations (MVP).
    - `Nyota.Tui`: console user interface, presentation layer.
    - `Nyota.Tests`: unit and integration tests.
- **Replaceable Adapters:** File storage is the MVP backend; future DB (SQL/Neo4j) or event bus (Redis/Kafka) can be plugged in by implementing the same interfaces.

---

## 2. Coding Standards

- **Language:** .NET 9+, C# latest features (records, pattern matching, async streams).
- **Nullability:** `Nullable` enabled in all projects. Avoid `null` unless part of explicit semantics.
- **Naming:**
    - Use `PascalCase` for types and public members.
    - Use `camelCase` for locals and private fields (prefix `_` for private fields).
    - Follow domain vocabulary (e.g. `Portfolio`, `JournalEntry`, `Policy`).
- **Enums:** Strongly typed, avoid magic strings in business logic.
- **Records:** Prefer immutable `record` types for domain models; use `class` only when mutation is necessary.

---

## 3. Data & Config

- **Configs:**
    - `policy.yaml` → governance rules, compliance limits.
    - `universe/*.csv` → instrument definitions.
    - `runs/*.json` → run configurations for backtests or live modes.
- **Data Folder:** Keep all runtime data in `./data/`. Version control only sample inputs; never commit live journals, ledgers, or portfolios.
- **Journal:** Append-only, hash-chained for audit integrity.

---

## 4. Testing

- Use **xUnit** for unit/integration tests.
- Every adapter must have at least one test (e.g. reading/writing portfolios, verifying hash chain).
- Test coverage for domain entities is optional if they are pure data records, but required for any custom logic.
- Run `dotnet test` before pushing any branch.

---

## 5. Contribution Flow

1. **Branching:** Use `feature/<short-name>`, `fix/<short-name>`, or `experiment/<short-name>`.
2. **Commits:** Keep them small, focused, and descriptive.
3. **Pull Requests:** Must build and pass tests. Include description of purpose, scope, and testing done.
4. **Code Reviews:** At least one reviewer approval before merging to `main`.

---

## 6. Extensibility Roadmap

- **Storage:** Add `Nyota.Storage.Sql` or `Nyota.Storage.Neo4j` for persistent portfolio and compliance history.
- **Event Bus:** Introduce `IEventBus` abstraction for publishing signals/fills/logs to Redis/Kafka.
- **Execution:** Wire strategies to real broker/exchange APIs behind `IExecutionProvider`.
- **UI:** Extend TUI with real-time charts, hotkeys, and event-driven updates. Future GUI/web UI possible.
- **Compliance:** Expand rule set and provide end-of-day attestations.

---

## 7. Coding Practices

- **Asynchronous APIs:** Use async/await for all I/O, never block threads.
- **Cancellation:** Support `CancellationToken` in all async methods.
- **Logging:** Journal is the canonical log. UI may display logs but must not replace persistence.
- **Error Handling:** Fail fast in core; adapters should catch/rethrow with context.

---

## 8. Tooling

- **IDE:** JetBrains Rider or Visual Studio 2022+.
- **Build:** `dotnet build Nyota.sln`
- **Run:** `dotnet run --project src/Nyota.Tui`
- **Test:** `dotnet test Nyota.sln`
- **Formatting:** Follow C# standard formatting; no custom rules enforced yet.

---

## 9. Security & Compliance

- No proprietary data or live credentials in repo.
- Use environment variables or secrets managers for API keys when added.
- Respect employer trading policy: only trade allowed instruments and venues.

---

## 10. Philosophy

- Start simple (file-backed, TUI).
- Keep everything replaceable and testable.
- Optimize for clarity, auditability, and extensibility.

---
