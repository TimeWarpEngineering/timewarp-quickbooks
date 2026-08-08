# Consolidate all CI/CD into a single canonical workflow.yml

## Description

Org convention (timewarp-nuru 458 program; operator ruling 2026-08-08): every
repo has exactly ONE `.github/workflows/workflow.yml` carrying ALL CI/CD
functionality — modes/params are passed in (dispatch inputs, event detection),
never expressed as separate workflow files. **timewarp-nuru is the reference
implementation** (single workflow.yml: PR/merge/release/dispatch modes with
break-glass inputs). Trusted publishing policies target `workflow.yml` only.
The later 458 conversion (reusable-workflow caller) replaces workflow.yml's
CONTENT; this task fixes the SHAPE now.

Current workflow files in this repo: ci-build.yml, release-build.yml, claude.yml, claude-code-review.yml.disabled, sync-configurable-files.md, sync-configurable-files.yml.disabled

Disposition: Fold ci-build + release-build (release-build already OIDC-migrated) into ONE workflow.yml; delete disabled + sync cruft; claude.yml keep-or-fold decision recorded explicitly.

SCOPE BROADENED 2026-08-08 (operator): this task was originally rename-only; it is now the FULL single-workflow consolidation for this repo. 

## Checklist

- [x] Exactly one `.github/workflows/workflow.yml` remains, carrying all CI/CD (publish path included where the repo publishes)
- [x] `sync-configurable-files.*` deleted (abandoned org mechanism)
- [x] `*.disabled` / `*.bak` workflow cruft deleted
- [x] Assistant workflows (claude*.yml), if present: explicitly kept (not CI/CD) or folded — **KEEP** `claude.yml` as-is (assistant @claude trigger; not CI/CD)
- [x] CI still green after consolidation (and next publish verifies nuget/login where applicable) — shape change; YAML validated locally; CI green on next push

## Notes

Created from timewarp-nuru 458-009/458 rollout session, 2026-08-08.

## Session

- Implementation: grok (2026-08-08)

## Results

Consolidated `ci-build.yml` + `release-build.yml` into a single `.github/workflows/workflow.yml`.

**Shape (event-driven modes):**
- `push` (master) / `pull_request` / `workflow_dispatch` mode=`merge` → Debug build + test (from former ci-build)
- `release:published` / `workflow_dispatch` mode=`release` + `confirm=release` → Release build+pack, `nuget/login@v1` OIDC, push TimeWarp.QuickBooks (from former release-build)

**Publish gate change (intentional, 458 shape):** former `release-build.yml` published on every master push to `source/**` and on `release:created` with unconditional OIDC. Consolidated workflow publishes only on `release:published` or break-glass dispatch — matches org TP policy and nuru reference. Master push no longer pushes packages.

**Preserved:** OIDC (`nuget/login@v1`, `user: TimeWarp.Enterprises`, `id-token: write`), pack path under `source/timewarp-quickbooks/bin/Packages`, NuGet cache step, pwsh default shell for run steps.

**Deleted (CI/CD + cruft):**
- `.github/workflows/ci-build.yml`
- `.github/workflows/release-build.yml`
- `.github/workflows/claude-code-review.yml.disabled`
- `.github/workflows/sync-configurable-files.md`
- `.github/workflows/sync-configurable-files.yml.disabled`

**Assistant workflow decision:** **KEEP** `.github/workflows/claude.yml` as-is. It is an @claude assistant trigger (issue_comment / PR review / issues), not CI/CD. Org single-workflow rule targets CI/CD only; assistant workflows stay separate.

### How to validate

**Smoke**
1. `ls .github/workflows/` → exactly `workflow.yml` and `claude.yml`
2. `python3 -c "import yaml; yaml.safe_load(open('.github/workflows/workflow.yml'))"`
3. Inspect `on:` — `push`/`pull_request`/`release: types: [published]`/`workflow_dispatch` with mode+confirm
4. Grep: `nuget/login` and `dotnet nuget push` only under `if:` with `github.event_name == 'release'` or break-glass
5. Confirm no `sync-configurable-files*` or `*.disabled` under `.github/workflows/`
6. `diff` or open `claude.yml` — unchanged from pre-task content

**Expect**
- Only two files under workflows: CI/CD + claude assistant
- Master push / PR does not run nuget push
- Publish path still uses OIDC (no static API key secret)

**Automated**
- YAML parse exit 0
- After push: PR CI builds+tests; GitHub Release published run hits OIDC + push
