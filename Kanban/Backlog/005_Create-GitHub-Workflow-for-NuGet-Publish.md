# Create GitHub Workflow for NuGet Publish

## Description

Create a GitHub Actions workflow to automatically build and publish the TimeWarp.QuickBooks library to NuGet.org. The workflow should follow best practices for .NET library publishing and securely handle secrets.

## Requirements

- Create a workflow YAML file in `.github/workflows/` (e.g., `publish-nuget.yml`)
- The workflow must:
  - Trigger on push to main or on new release/tag
  - Build the library using the correct .NET SDK version (target net9.0)
  - Pack the NuGet package
  - Publish to NuGet.org using a secure API key stored in GitHub Secrets
  - Only publish when version changes or on release
- Reference similar workflows from other TimeWarpEngineering projects for consistency
- Document the workflow in the README or a separate documentation file

## Checklist

### Implementation
- [ ] Create workflow YAML file
- [ ] Configure triggers (push to main, release, or tag)
- [ ] Build and pack the library
- [ ] Publish to NuGet.org using GitHub Secrets
- [ ] Test the workflow in a dry run (if possible)

### Documentation
- [ ] Update documentation to describe the workflow and required secrets

### Review
- [ ] Code Review
- [ ] Consider Security Implications (secrets, permissions)
- [ ] Consider Performance Implications

## Notes

- Reference TimeWarpEngineering workflows for best practices

## Implementation Notes

[Include notes while task is in progress]