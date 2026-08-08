# Review framework — task 009

**Date:** 2026-08-08
**Host task:** kanban/in-progress/009-migrate-nuget-publish-workflow-to-trusted-publishing-nugetlogin/
**Diff scope:** commit `34a1400` (feat(ci): migrate NuGet publish to trusted publishing) — workflows + docs
**Plan / brief:** Migrate release-build from `PUBLISH_TO_NUGET_ORG` secret to `nuget/login@v1` OIDC trusted publishing; remove dead secret env from ci-build; update read-me/claude docs. Keep always-on publish for this job.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** grok orchestration (2026-08-08)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
