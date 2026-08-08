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

- [ ] Add `id-token: write` (with `contents: read`) permissions to the publish job
- [ ] Add `nuget/login@v1` gated on the publish condition
- [ ] Replace the stored-secret `--api-key` with the login step output
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
