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

- [ ] Find all sync-config copies (including `.github/`, `.github/scripts/`, and `workflows-temp/`)
- [ ] Delete them
- [ ] Remove related workflow if any
- [ ] Commit
- [ ] Verify no new sync PRs
- [ ] Close stale sync PRs

## Notes

Created to stop recurring template-sync PRs. Cover every sync-config copy in this repository, not only `.github/sync-config.yml`.
