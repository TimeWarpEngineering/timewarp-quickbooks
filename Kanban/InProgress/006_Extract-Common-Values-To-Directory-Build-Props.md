# 006 - Extract Common Values to Directory.Build.props and Use Directory.Packages.props

## Description

Extract common project values to Directory.Build.props and centralize package references using Directory.Packages.props in the timewarp-quickbooks repository. This follows TimeWarp's standard approach to .NET project organization, using the TimeWarp.SourceGenerators repository as a reference. This task improves maintainability by centralizing common properties and package versions, making updates more efficient and consistent across the entire solution.

## Requirements

- Create Directory.Build.props at the root level with common project properties following TimeWarp's conventions
- Create Directory.Packages.props at the root level for centralized NuGet package version management
- Update existing project files to use the centralized properties and package references
- Ensure all projects build successfully after the changes
- Include standard TimeWarp metadata (authors, repository info, etc.)

## Checklist

### Implementation
- [ ] Create Directory.Build.props with common properties:
  - [ ] TargetFramework (net9.0)
  - [ ] ImplicitUsings (enable)
  - [ ] Nullable (enable)
  - [ ] Common assembly info (Authors, Product, etc.)
  - [ ] Package metadata (PackageProjectUrl, RepositoryUrl, PackageTags, etc.)
  - [ ] Source Link and deterministic build settings
  - [ ] Documentation generation settings
  - [ ] Version information and versioning strategy
- [ ] Create Directory.Packages.props with:
  - [ ] All NuGet package references and their versions
  - [ ] Standard `ManagePackageVersionsCentrally` setting (true)
- [ ] Update project files to remove redundant properties now defined in Directory.Build.props
- [ ] Update project files to reference packages without explicit versions
- [ ] Test building solution to verify changes work correctly

### Documentation
- [ ] Add comments in Directory.Build.props and Directory.Packages.props explaining their purpose
- [ ] Update project documentation if necessary

### Review
- [ ] Consider Performance Implications (faster builds due to centralized management)
- [ ] Consider Maintenance Implications (easier version upgrades)
- [ ] Code Review

## Notes

Directory.Build.props and Directory.Packages.props are MSBuild features that allow for centralizing common build properties and package versions:

- **Directory.Build.props**: Automatically imported by MSBuild before the project file, allowing common properties to be defined once
- **Directory.Packages.props**: Centralizes NuGet package versions when using the `ManagePackageVersionsCentrally` feature

Based on the TimeWarp.SourceGenerators implementation, the Directory.Build.props should include:
- Common project properties (TargetFramework, ImplicitUsings, Nullable)
- Package metadata (Authors, Product, URLs, etc.)
- Build configuration settings (LangVersion, TreatWarningsAsErrors, etc.)
- Documentation generation settings
- Source Link and deterministic build settings for proper package generation

Current packages to centralize in Directory.Packages.props:
- TimeWarp.Fixie (3.0.0)
- Fixie.TestAdapter (4.1.0)
- Shouldly (4.3.0)
- IppDotNetSdkForQuickBooksApiV3 (14.7.0)

See TimeWarp.SourceGenerators repository: https://github.com/TimeWarpEngineering/timewarp-source-generators

## Implementation Notes

To be filled in as the task progresses.
