# Create GitHub Workflow for NuGet Publish

## Description

Create a GitHub Actions workflow to automatically build and publish the TimeWarp.QuickBooks library to NuGet.org. The workflow should follow best practices for .NET library publishing and securely handle secrets.

## Requirements

- Create a workflow YAML file in `.github/workflows/` (e.g., `publish-nuget.yml`)
- The workflow must:
  - Trigger on push to master or on new release/tag
  - Build the library using the correct .NET SDK version (target net9.0)
  - Pack the NuGet package
  - Publish to NuGet.org using a secure API key stored in GitHub Secrets
  - Only publish when version changes or on release
- Reference similar workflows from other TimeWarpEngineering projects for consistency
- Document the workflow in the README or a separate documentation file

## Checklist

### Implementation
- [x] Create workflow YAML file
- [x] Configure triggers (push to master, release, or tag)
- [x] Build and pack the library
- [x] Publish to NuGet.org using GitHub Secrets
- [ ] Test the workflow in a dry run (if possible)

### Documentation
- [x] Update documentation to describe the workflow and required secrets

### Review
- [ ] Code Review
- [x] Consider Security Implications (secrets, permissions)
- [x] Consider Performance Implications

## Notes

- Reference TimeWarpEngineering workflows for best practices
- Use the workflow from https://github.com/TimeWarpEngineering/timewarp-source-generators/ repository as an example

## Implementation Notes

1. Created two GitHub workflow files based on the TimeWarpEngineering/timewarp-source-generators repository:
   - `.github/workflows/ci-build.yml`: For continuous integration testing on pull requests
   - `.github/workflows/release-build.yml`: For building and publishing to NuGet

2. CI Build Workflow:
   - Triggers on pull requests and manual workflow dispatch
   - Sets up .NET 9.0 environment
   - Builds the TimeWarp.QuickBooks library
   - Runs tests

3. Release Build Workflow:
   - Triggers on push to master branch, new releases, and manual workflow dispatch
   - Sets up .NET 9.0 environment
   - Builds and packs the TimeWarp.QuickBooks library
   - Publishes the package to NuGet.org using PUBLISH_TO_NUGET_ORG secret

4. Updated README.md with:
   - GitHub Actions build badge
   - Documentation on CI/CD pipeline
   - Required GitHub secrets explanation

5. Note: The GitHub workflow requires the PUBLISH_TO_NUGET_ORG secret to be configured in the repository settings.