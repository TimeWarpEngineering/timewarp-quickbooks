# Round 1 — general
**Date:** 2026-08-08
**Scope reviewed:** commit 34a1400 workflows + docs for trusted publishing

## Summary

`release-build.yml` correctly migrates NuGet publish to OIDC trusted publishing: job permissions `contents: read` + `id-token: write`, `nuget/login@v1` with `id: nuget-login` and user `TimeWarp.Enterprises`, and push wired to `${{ steps.nuget-login.outputs.NUGET_API_KEY }}`. Live workflows no longer reference `PUBLISH_TO_NUGET_ORG` / `NUGET_AUTH_TOKEN`; login and publish stay ungated so any run of this job still publishes, matching the plan (not the release-only gate used in timewarp-nuru). Docs describe trusted publishing without claiming the long-lived secret was deleted; operator revoke remains out of code.

## Issues

<!-- none — migration matches plan; no correctness, wiring, or security regressions found -->
