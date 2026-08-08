# Add trusted-publishing probe mode to workflow.yml

## Description

org 458-009 probe (NuGet has no policy-enumeration API; probe = dispatch mode that runs only the nuget/login OIDC exchange and stops — success proves the workflow.yml policy matches; reference timewarp-nuru's workflow.yml).

## Checklist

- [x] probe input added
- [x] login step condition extended
- [x] probe-result step added
- [x] pipeline step skipped in probe mode
- [x] YAML valid

## Results

- Added `probe` as a third `mode` choice on `workflow_dispatch` (alongside existing `merge`/`release`), with updated description text.
- Extended the `NuGet login (OIDC Trusted Publishing)` step's `if:` condition to also run when `inputs.mode == 'probe'`.
- Added a new `Trusted publishing probe result` step immediately after login that echoes success when probe mode reaches it.
- All build/test/pack/publish steps were already positively allow-listed to specific event/mode combinations that exclude `probe`, so no additional gating was needed on them — they simply do not run in probe mode.
- Single job (`ci`) shape, matching reference implementation pattern from timewarp-nuru.

### How to validate

**Smoke:** `gh workflow run workflow.yml -f mode=probe` after push → expect the "Trusted publishing probe result" step to run and go green.
**Expect:** a failure of the NuGet login step means the trusted-publishing policy is missing or misconfigured on NuGet.org for this repo + workflow.yml — not a bug in this change.
