# Migrate NuGet publish workflow to trusted publishing (nuget/login)

## Description

The trusted publishing policy for this repo already exists on NuGet.org
(owner TimeWarp.Enterprises, created 2026-08-08) but is INERT until the
publish workflow exchanges an OIDC token for a temp key instead of using a
stored secret. Org program context: timewarp-nuru kanban 458-009.

Current state (2026-08-07 org audit): secret `PUBLISH_TO_NUGET_ORG` in the ci-build.yml / release-build.yml pair; dispatch-driven publish.

Reference implementation: timewarp-nuru `.github/workflows/workflow.yml` —
`nuget/login@v1` step (user: TimeWarp.Enterprises) gated on the release
condition, `id-token: write` job permission, push via
`--api-key ${{ steps.nuget-login.outputs.NUGET_API_KEY }}`.

NOTE: if this repo's full convention conversion (reusable-workflow caller,
timewarp-nuru 458 rollout) is imminent, do the conversion instead — it
includes this migration for free.

## Checklist

- [x] Add `id-token: write` (with `contents: read`) permissions to the publish job
- [x] Add `nuget/login@v1` gated on the publish condition
- [x] Replace the stored-secret `--api-key` with the login step output
- [ ] Verify the publish path end-to-end on the next release
- [ ] AFTER verified: operator revokes the long-lived NuGet key and deletes the GitHub secret (org-wide revocation tracked in nuru 458-009)

## Notes

Created from the timewarp-nuru 458-009 rollout session (2026-08-08).

### Implementation plan (2026-08-08)

**Scope:** Migrate trusted publishing only in existing `ci-build.yml` / `release-build.yml`. Do **not** convert to reusable-workflow caller (no imminent conversion WIP).

**release-build.yml**
1. Add job `permissions`: `contents: read`, `id-token: write`
2. Remove `NUGET_AUTH_TOKEN: ${{secrets.PUBLISH_TO_NUGET_ORG}}` from top-level env
3. Add `nuget/login@v1` (`id: nuget-login`, `user: TimeWarp.Enterprises`) before Publish
4. Replace push `--api-key` with `${{ steps.nuget-login.outputs.NUGET_API_KEY }}`

**ci-build.yml**
- Remove dead `NUGET_AUTH_TOKEN` only (CI never publishes; no OIDC)

**Publish gating**
- Keep current behavior: publish whenever `release-build` runs (master path push, release, workflow_dispatch). Do not switch to release-only. No extra `if:` on login (publish always runs in this job).

**Docs**
- Update `read-me.md` and `claude.md`: trusted publishing (OIDC), not long-lived `PUBLISH_TO_NUGET_ORG` secret. Do not claim secret already deleted.

**Operator (out of code; after verified e2e)**
1. Confirm Actions: login + push with temp key
2. Then revoke long-lived NuGet key and delete GitHub secret `PUBLISH_TO_NUGET_ORG`
3. Org-wide revocation tracked in nuru 458-009

**Out of scope:** reusable-workflow conversion, release-only publish, MinVer, snupkg, secret revocation by agent.

## Session

- Orchestration: grok (2026-08-08)
- Implementation (2026-08-08): Migrated release-build.yml to NuGet Trusted Publishing (OIDC) via `nuget/login@v1` (user: TimeWarp.Enterprises), job permissions `contents: read` + `id-token: write`, push uses `${{ steps.nuget-login.outputs.NUGET_API_KEY }}`. Removed dead `NUGET_AUTH_TOKEN` / secret refs from both workflows. Docs (`read-me.md`, `claude.md`) updated to trusted-publishing language. Login not gated with `if:` — this job always publishes when it runs. No secret revocation (operator after e2e verify).
- Review (2026-08-08): effort 1 general; disposition clean — see `review/`
- Folderized for Phase 4b kitchen (2026-08-08)

## Results

### What was implemented

Migrated NuGet publish from long-lived `PUBLISH_TO_NUGET_ORG` to **NuGet Trusted Publishing (OIDC)** so the existing NuGet.org policy can exchange a GitHub OIDC token for a temporary API key.

- `release-build.yml`: job `permissions` (`contents: read`, `id-token: write`); `nuget/login@v1` (`id: nuget-login`, `user: TimeWarp.Enterprises`); push uses `${{ steps.nuget-login.outputs.NUGET_API_KEY }}`; removed secret env
- `ci-build.yml`: removed unused `NUGET_AUTH_TOKEN` (CI never publishes)
- Docs: `read-me.md` + `claude.md` describe trusted publishing; do not claim secret deleted

### Files changed

| File | Change |
|------|--------|
| `.github/workflows/release-build.yml` | OIDC permissions, login, temp API key |
| `.github/workflows/ci-build.yml` | Drop dead secret env |
| `read-me.md` | Trusted publishing subsection |
| `claude.md` | CI/CD → OIDC |
| `kanban/.../009-.../` | Plan, checklist, review kitchen, Results |

### Key decisions / deviations

- **Scope:** trusted-publishing only — no reusable-workflow conversion
- **No release-only `if:`** on login/publish — this job always publishes when it runs (matches existing auto-publish on master; differs from nuru’s release-gated login)
- **Secret revocation** intentionally not performed by agent (operator after e2e; org track: nuru 458-009)

### Test outcomes

- Repo grep: no `PUBLISH_TO_NUGET_ORG` / `NUGET_AUTH_TOKEN` in live workflows or active docs (kanban history only)
- Phase 4b review: effort 1, 1 round, 0 findings
- **Live OIDC/push not run in-session** (requires GitHub Actions after merge/push)

### Phase 4b review

| Field | Value |
|-------|--------|
| Rounds | 1 |
| Effort / roster | 1 — general |
| Final counts | 0 open (bug/suggestion/nit all 0) |
| Disposition | **clean** |
| Paths | `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md` |

### Residual operator (checklist left open by design)

1. Verify publish path on next `release-build` run (see How to validate)
2. After success: revoke long-lived NuGet key + delete GitHub secret `PUBLISH_TO_NUGET_ORG` (org-wide: nuru 458-009)

### How to validate

**Smoke (after this lands on `master` / remote)**

```bash
# From GitHub UI: Actions → "Build and Deploy" → Run workflow (workflow_dispatch)
# Or push a source/ path change on master (may only --skip-duplicate if version unchanged)
```

**Expect**

1. Job permissions include `id-token: write`
2. Step **NuGet login (OIDC Trusted Publishing)** succeeds
3. Step **Publish TimeWarp.QuickBooks** succeeds (new version on nuget.org, or `--skip-duplicate` if already published)
4. Workflow log does **not** reference `secrets.PUBLISH_TO_NUGET_ORG`

**Local static check (no OIDC)**

```bash
rg -n 'PUBLISH_TO_NUGET_ORG|NUGET_AUTH_TOKEN' .github/workflows read-me.md claude.md
# expect: no matches in live workflows/docs
rg -n 'nuget/login|id-token|NUGET_API_KEY' .github/workflows/release-build.yml
# expect: permissions id-token write, nuget/login@v1, steps.nuget-login.outputs.NUGET_API_KEY
```

**Depends on:** NuGet.org trusted publishing policy for owner `TimeWarp.Enterprises`, repo `TimeWarpEngineering/timewarp-quickbooks`, workflow path `.github/workflows/release-build.yml` (already created 2026-08-08 per task).

**Not in scope (agent):** live Actions e2e in this session; revoking the long-lived key / deleting the GitHub secret (operator after e2e; nuru 458-009).
