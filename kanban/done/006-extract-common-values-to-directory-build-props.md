# 006 - Extract Common Values to directory.build.props and Use directory.packages.props

## Description

Extract common project values to directory.build.props and centralize package references using directory.packages.props in the timewarp-quickbooks repository. This follows TimeWarp's standard approach to .NET project organization, using the TimeWarp.SourceGenerators repository as a reference. This task improves maintainability by centralizing common properties and package versions, making updates more efficient and consistent across the entire solution.

## Requirements

- Create directory.build.props at the root level with common project properties following TimeWarp's conventions
- Create directory.packages.props at the root level for centralized NuGet package version management
- Update existing project files to use the centralized properties and package references
- Ensure all projects build successfully after the changes
- Include standard TimeWarp metadata (authors, repository info, etc.)

## Checklist

### Implementation
- [x] Create directory.build.props with common properties:
  - [x] TargetFramework (net9.0)
  - [x] ImplicitUsings (enable)
  - [x] Nullable (enable)
  - [x] Common assembly info (Authors, Product, etc.)
  - [x] Package metadata (PackageProjectUrl, RepositoryUrl, PackageTags, etc.)
  - [x] Source Link and deterministic build settings
  - [x] Documentation generation settings
  - [x] Version information and versioning strategy
- [x] Create directory.packages.props with:
  - [x] All NuGet package references and their versions
  - [x] Standard `ManagePackageVersionsCentrally` setting (true)
- [x] Update project files to remove redundant properties now defined in directory.build.props
- [x] Update project files to reference packages without explicit versions
- [x] Test building solution to verify changes work correctly

### Documentation
- [x] Add comments in directory.build.props and directory.packages.props explaining their purpose
- [x] Update project documentation if necessary

### Review
- [x] Consider Performance Implications (faster builds due to centralized management)
- [x] Consider Maintenance Implications (easier version upgrades)
- [x] Code Review

## Notes

directory.build.props and directory.packages.props are MSBuild features that allow for centralizing common build properties and package versions:

- **directory.build.props**: Automatically imported by MSBuild before the project file, allowing common properties to be defined once
- **directory.packages.props**: Centralizes NuGet package versions when using the `ManagePackageVersionsCentrally` feature

Based on the TimeWarp.SourceGenerators implementation, the directory.build.props should include:
- Common project properties (TargetFramework, ImplicitUsings, Nullable)
- Package metadata (Authors, Product, URLs, etc.)
- Build configuration settings (LangVersion, TreatWarningsAsErrors, etc.)
- Documentation generation settings
- Source Link and deterministic build settings for proper package generation

Current packages to centralize in directory.packages.props:
- TimeWarp.Fixie (3.0.0)
- Fixie.TestAdapter (4.1.0)
- Shouldly (4.3.0)
- IppDotNetSdkForQuickBooksApiV3 (14.7.0)

See TimeWarp.SourceGenerators repository: https://github.com/TimeWarpEngineering/timewarp-source-generators

## Implementation Notes

All required files have been successfully created and configured:

1. Created `directory.build.props` with all common properties:
   - Set TargetFramework to net9.0
   - Enabled ImplicitUsings and Nullable
   - Added common assembly info with TimeWarp Engineering as author
   - Configured package metadata including repository URLs, license, etc.
   - Set up Source Link for deterministic builds
   - Added documentation generation settings
   - Set version information (0.1.0-alpha)

2. Created `directory.packages.props` with:
   - Enabled central package version management
   - Added all package references with their versions:
     - IppDotNetSdkForQuickBooksApiV3 (14.7.0)
     - Fixie.TestAdapter (4.1.0)
     - Shouldly (4.3.0)
     - TimeWarp.Fixie (3.0.0)

3. Updated project files:
   - Removed redundant properties now in directory.build.props
   - Updated package references to use centralized versions

The solution has been tested and builds successfully with the new configuration. This implementation follows the TimeWarp standard practices, ensuring consistency across projects and simplifying future maintenance.
