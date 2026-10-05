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
- [ ] Add `Account` and query-envelope models under `api/models/`
- [ ] Add `GetAllAccountsAsync` to interface and implementation
### Implementation
- [ ] Pagination over 1000-row QuickBooks cap
- [ ] Unit tests (single page, multi-page, empty)
- [ ] Build + tests green
### Documentation
- [ ] XML docs on new public API
- [ ] Mention chart of accounts in read-me / claude.md architecture notes

## Session

- Created: 3713436 (2026-10-05)

## Notes

- Id 013 collides with a legacy hand-numbered done task `013-remove-sync-configyml.md`; left as-is.
- QBO query reference: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/account#query-an-account
