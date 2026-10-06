# Add GetAllAccountsAsync to fetch the full chart of accounts

## Description

Add a typed, high-level operation to `IQuickBooksApiClient` that returns the company's
complete chart of accounts. QuickBooks Online exposes accounts via
`SELECT * FROM Account`, capped at 1000 rows per query, so the method must page with
`STARTPOSITION` / `MAXRESULTS` until all rows are returned.

## Requirements

- `Task<IReadOnlyList<Account>> GetAllAccountsAsync(string realmId, CancellationToken ct = default)` on `IQuickBooksApiClient`.
- Strongly typed `Account` model (Id, Name, FullyQualifiedName, AccountType, AccountSubType,
  Classification, AccountNumber, Active, SubAccount, ParentRef, CurrentBalance,
  CurrentBalanceWithSubAccounts, CurrencyRef, Description, SyncToken, MetaData).
- Generic `QueryResponse` envelope model reused by future typed queries.
- Pagination: issue successive queries until a page returns fewer than the page size.
- Unit tests with `TestHttpMessageHandler`: single page, multi-page, empty company.

## Checklist

### Design
- [x] Add `Account` and query-envelope models under `api/models/`
- [x] Add `GetAllAccountsAsync` to interface and implementation
### Implementation
- [x] Pagination over 1000-row QuickBooks cap
- [x] Unit tests (single page, multi-page, empty)
- [x] Build + tests green
### Documentation
- [x] XML docs on new public API
- [x] Mention chart of accounts in read-me / claude.md architecture notes

## Session

- Created: 3713436 (2026-10-05)

## Notes

- Id 013 collides with a legacy hand-numbered done task `013-remove-sync-configyml.md`; left as-is.
- QBO query reference: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/account#query-an-account

## Implementation Notes

- Models live in `source/timewarp-quickbooks/api/models/` (`Account`, `ReferenceType`, `ModificationMetaData`, `query-response.cs` with `QueryResponseEnvelope<T>`, `QueryResponsePage`, `AccountQueryResponse`, `AccountQueryResponseEnvelope`).
- `GetAllAccountsAsync` pages 1000 rows at a time via `QueryAsync<AccountQueryResponseEnvelope>` and stops on an empty or short page.
- `TestHttpMessageHandler` gained a FIFO `EnqueueResponse` queue and a `Requests` list; existing tests are unaffected.
- `read-me.md` has no API client usage section, so only `claude.md` was updated.

## Audit fold-in (2026-10-06)

`ganda repo audit` failed on master with 18 pre-existing scaffold errors; per operator decision the
fixes were folded into this PR via `ganda repo audit --fix`:

- Scaffolded: `.editorconfig`, `.envrc`, `.githooks/`, `.memsearch.toml`, `.vscode/tasks.json`,
  `BannedSymbols.txt`, `msbuild/repository.props`, `source/Directory.Build.props`, `tools/dev-cli/`,
  `skills/`, `kanban/archived/`, gitignore entries, BannedApiAnalyzers / SourceGenerators / Build.Tasks refs.
- Hand corrections after the fixer:
  - Reformatted the one-line `<ItemGroup>` appends in both root props files.
  - `TimeWarp.Amuru` pin 1.0.0-beta.1 → 1.0.0 and `TimeWarp.Amuru.Tools` 2.0.0-beta.1 → 1.0.0-beta.2
    (NU1605 downgrade against Nuru 3.0.0-beta.76; matches timewarp-flow).
  - `source/Directory.Build.props` template had `<Version>1.0.0-beta.1</Version>`, Unlicense, and
    duplicate metadata that overrode the root (effective package version regressed to beta.1).
    Reduced to `<Version>1.0.0-beta.3</Version>` only; root remains the value `scripts/check-version.cs` reads.
  - Removed stray `tools/dev-cli/source/Directory.Build.props` scaffold copy.
- `bin/dev` is gitignored; bootstrap locally with `dotnet run tools/dev-cli/dev.cs -- self-install`.
