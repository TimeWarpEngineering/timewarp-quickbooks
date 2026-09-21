# Remove sync-config.yml

## Description

Stop template sync noise by removing sync-config.yml (and related sync workflow/config copies if present). These files drive "Sync configurable files from parent repository" PRs.

This repo has multiple sync-config copies that must all be removed.

## Requirements

- Delete `.github/sync-config.yml`
- Delete `.github/scripts/sync-config.yml`
- Delete `workflows-temp/sync-config.yml`
- Remove or disable any related sync-configurable-files workflow if present
- Close open sync PRs after merge if appropriate
- Do not reintroduce sync-config

## Checklist

- [x] Find all sync-config copies (including `.github/`, `.github/scripts/`, and `workflows-temp/`)
- [x] Delete them
- [x] Remove related workflow if any
- [x] Commit
- [x] Verify no new sync PRs
- [x] Close stale sync PRs

## Notes

Created to stop recurring template-sync PRs. Cover every sync-config copy in this repository, not only `.github/sync-config.yml`.

Stale sync PRs were closed as obsolete on this cleanup PR (not after merge), with a pointer comment on each. Their branches were left in place for the coordinator to delete after merge.

## Results

Cleanup PR (open, not merged): https://github.com/TimeWarpEngineering/timewarp-quickbooks/pull/34

**Deleted (every remaining driver / copy):**
- `.github/sync-config.yml`
- `.github/scripts/sync-config.yml`
- `.github/scripts/sync-configurable-files.ps1`
- `workflows-temp/sync-config.yml`
- `workflows-temp/sync-configurable-files.yml`
- `workflows-temp/sync-configurable-files.ps1`
- `workflows-temp/read-me.md` (sync-only staging docs)
- `copy-workflows.ps1` (would re-copy staging files into `.github/`)

`.github/workflows/` already had no `sync-configurable-files` workflow (removed in 010). Empty `workflows-temp/` and `.github/scripts/` directories were removed after the deletes.

**Closed stale sync PRs (not merged), each with a pointer comment to #34:**
- #24 `Sync configurable files from parent repository` (`sync-configurable-files-1751275262`)
- #25 `Sync configurable files from parent repository` (`sync-configurable-files-1751880088`)
- #26 `Sync configurable files from parent repository` (`sync-configurable-files-1752485171`)
- #21 `chore: sync configurable files from timewarp-architecture` (`sync/configurable-files-1`) — same abandoned mechanism, different title

**Leftover remote branches (do not delete until after #34 merges):**
- `sync-configurable-files-1751275262`
- `sync-configurable-files-1751880088`
- `sync-configurable-files-1752485171`
- `sync/configurable-files-1`

**Verify:** tree on `cursor/remove-sync-config-2c70` has no `sync-config.yml` and no `sync-configurable-files` script/workflow. Only remaining mentions are this kanban card and historical notes in `kanban/done/010-*`.
