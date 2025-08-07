# Task 008: Implement Kebab-Case Naming Convention

## Summary
Migrate entire codebase to follow kebab-case naming convention for all files and directories as documented in the TimeWarp standards.

## Impact Analysis
- **75 files** with uppercase letters need renaming
- **32 directories** need renaming
- Multiple project file references need updating
- NuGet package metadata references (Logo.png, README.md)
- Solution file references
- Test file references
- Launch settings and configuration files

## Implementation Plan

### Phase 1: Documentation and Non-Code Files
This phase is safe to implement first as it doesn't affect compilation.

#### 1.1 Rename Documentation Structure
- [ ] `Documentation/` → `documentation/`
- [ ] `Documentation/User/` → `documentation/user/`
- [ ] `Documentation/Developer/` → `documentation/developer/`
- [ ] `Documentation/Developer/Conceptual/` → `documentation/developer/conceptual/`
- [ ] `Documentation/Developer/Conceptual/ArchitecturalDecisionRecords/` → `documentation/developer/conceptual/architectural-decision-records/`
- [ ] `Documentation/Developer/Conceptual/ArchitecturalDecisionRecords/Approved/` → `documentation/developer/conceptual/architectural-decision-records/approved/`
- [ ] `Documentation/Developer/Conceptual/ArchitecturalDecisionRecords/Proposed/` → `documentation/developer/conceptual/architectural-decision-records/proposed/`
- [ ] `Documentation/Developer/Conceptual/ArchitecturalDecisionRecords/ProjectStructureAndConventions/` → `documentation/developer/conceptual/architectural-decision-records/project-structure-and-conventions/`
- [ ] `Documentation/Developer/Conceptual/ArchitecturalDecisionRecords/Examples/` → `documentation/developer/conceptual/architectural-decision-records/examples/`
- [ ] `Documentation/Developer/Reference/` → `documentation/developer/reference/`
- [ ] `Documentation/Developer/Tutorials/` → `documentation/developer/tutorials/`
- [ ] `Documentation/Developer/HowToGuides/` → `documentation/developer/how-to-guides/`
- [ ] `Documentation/StarUml/` → `documentation/star-uml/`

#### 1.2 Rename Documentation Files
- [ ] All `Overview.md` → `overview.md`
- [ ] `Documentation/Developer/Conceptual/ProjectGenesis.md` → `documentation/developer/conceptual/project-genesis.md`
- [ ] `Documentation/Developer/Reference/Glossary.md` → `documentation/developer/reference/glossary.md`
- [ ] `Documentation/StarUml/ArchitectureComparisons.mdj` → `documentation/star-uml/architecture-comparisons.mdj`

#### 1.3 Rename Kanban Structure
- [ ] `Kanban/` → `kanban/`
- [ ] `Kanban/ToDo/` → `kanban/to-do/`
- [ ] `Kanban/InProgress/` → `kanban/in-progress/`
- [ ] `Kanban/Backlog/` → `kanban/backlog/`
- [ ] `Kanban/Backlog/Scratch/` → `kanban/backlog/scratch/`
- [ ] `Kanban/Done/` → `kanban/done/`
- [ ] `Kanban/Done/007_Implement-QuickBooks-API-Client/` → `kanban/done/007-implement-quickbooks-api-client/`

#### 1.4 Rename Kanban Files
- [ ] `Kanban/ReadMe.md` → `kanban/readme.md`
- [ ] `Kanban/Task-Template.md` → `kanban/task-template.md`
- [ ] All kanban task files: Convert pattern `NNN_Task-Name.md` to `nnn-task-name.md`
  - Example: `003_Create-QuickBooks-Auth-and-Token-Service.md` → `003-create-quickbooks-auth-and-token-service.md`

#### 1.5 Rename Assets
- [ ] `Assets/` → `assets/`
- [ ] `Assets/Logo.png` → `assets/logo.png`
- [ ] `Assets/Logo.svg` → `assets/logo.svg`
- [ ] Update `Directory.Build.props` references to logos

#### 1.6 Rename Root Documentation Files
- [ ] `README.md` → `readme.md`
- [ ] `LICENSE` → `license`
- [ ] `CLAUDE.md` → `claude.md`
- [ ] `Diagram.md` → `diagram.md`
- [ ] `TimeWarp-QuickBooks-API-Client.md` → `timewarp-quickbooks-api-client.md`
- [ ] Update `Directory.Build.props` reference to README.md

### Phase 2: Scripts and Configuration
#### 2.1 Rename Scripts Directory
- [ ] `Scripts/` → `scripts/`
- [ ] `Scripts/CheckVersion.cs` → `scripts/check-version.cs`

#### 2.2 Rename PowerShell Scripts
- [ ] `RunTests.ps1` → `run-tests.ps1`

#### 2.3 Rename .ai Directory
- [ ] `.ai/` → `.ai/`
- [ ] `.ai/ProjectSage/` → `.ai/project-sage/`
- [ ] `.ai/Overview.md` → `.ai/overview.md`
- [ ] `.ai/ProjectSage/Overview.md` → `.ai/project-sage/overview.md`

### Phase 3: Project Structure (Most Complex)
#### 3.1 Rename Source Structure
- [ ] `Source/` → `source/`
- [ ] `Source/TimeWarp.QuickBooks/` → `source/timewarp-quickbooks/`
- [ ] `Source/TimeWarp.QuickBooks/Api/` → `source/timewarp-quickbooks/api/`
- [ ] `Source/TimeWarp.QuickBooks/Authentication/` → `source/timewarp-quickbooks/authentication/`
- [ ] `Source/TimeWarp.QuickBooks/Authentication/Models/` → `source/timewarp-quickbooks/authentication/models/`

#### 3.2 Rename Source Files
- [ ] `Source/TimeWarp.QuickBooks/TimeWarp.QuickBooks.csproj` → `source/timewarp-quickbooks/timewarp-quickbooks.csproj`
- [ ] `Source/TimeWarp.QuickBooks/GlobalUsings.cs` → `source/timewarp-quickbooks/global-usings.cs`
- [ ] `Source/TimeWarp.QuickBooks/Api/IQuickBooksApiClient.cs` → `source/timewarp-quickbooks/api/i-quickbooks-api-client.cs`
- [ ] `Source/TimeWarp.QuickBooks/Api/QuickBooksApiClient.cs` → `source/timewarp-quickbooks/api/quickbooks-api-client.cs`
- [ ] `Source/TimeWarp.QuickBooks/Api/QuickBooksApiException.cs` → `source/timewarp-quickbooks/api/quickbooks-api-exception.cs`
- [ ] `Source/TimeWarp.QuickBooks/Api/QuickBooksApiOptions.cs` → `source/timewarp-quickbooks/api/quickbooks-api-options.cs`
- [ ] `Source/TimeWarp.QuickBooks/Api/QuickBooksHttpClient.cs` → `source/timewarp-quickbooks/api/quickbooks-http-client.cs`
- [ ] `Source/TimeWarp.QuickBooks/Authentication/IQuickBooksOAuthService.cs` → `source/timewarp-quickbooks/authentication/i-quickbooks-oauth-service.cs`
- [ ] `Source/TimeWarp.QuickBooks/Authentication/QuickBooksOAuthService.cs` → `source/timewarp-quickbooks/authentication/quickbooks-oauth-service.cs`
- [ ] `Source/TimeWarp.QuickBooks/Authentication/ServiceCollectionExtensions.cs` → `source/timewarp-quickbooks/authentication/service-collection-extensions.cs`
- [ ] `Source/TimeWarp.QuickBooks/Authentication/Models/QuickBooksOAuthCallbackResult.cs` → `source/timewarp-quickbooks/authentication/models/quickbooks-oauth-callback-result.cs`
- [ ] `Source/TimeWarp.QuickBooks/Authentication/Models/QuickBooksOAuthOptions.cs` → `source/timewarp-quickbooks/authentication/models/quickbooks-oauth-options.cs`
- [ ] `Source/TimeWarp.QuickBooks/Authentication/Models/QuickBooksTokens.cs` → `source/timewarp-quickbooks/authentication/models/quickbooks-tokens.cs`
- [ ] `Source/Overview.md` → `source/overview.md`

#### 3.3 Rename Tests Structure
- [ ] `Tests/` → `tests/`
- [ ] `Tests/TimeWarp.QuickBooks.Tests/` → `tests/timewarp-quickbooks-tests/`

#### 3.4 Rename Test Files
- [ ] `Tests/TimeWarp.QuickBooks.Tests/TimeWarp.QuickBooks.Tests.csproj` → `tests/timewarp-quickbooks-tests/timewarp-quickbooks-tests.csproj`
- [ ] `Tests/TimeWarp.QuickBooks.Tests/GlobalUsings.cs` → `tests/timewarp-quickbooks-tests/global-usings.cs`
- [ ] `Tests/TimeWarp.QuickBooks.Tests/ConventionTests.cs` → `tests/timewarp-quickbooks-tests/convention-tests.cs`
- [ ] `Tests/TimeWarp.QuickBooks.Tests/TestingConvention.cs` → `tests/timewarp-quickbooks-tests/testing-convention.cs`
- [ ] `Tests/TimeWarp.QuickBooks.Tests/QuickBooksApiClient_Should_.cs` → `tests/timewarp-quickbooks-tests/quickbooks-api-client-should.cs`
- [ ] `Tests/TimeWarp.QuickBooks.Tests/QuickBooksApiException_Should_.cs` → `tests/timewarp-quickbooks-tests/quickbooks-api-exception-should.cs`
- [ ] `Tests/Overview.md` → `tests/overview.md`

#### 3.5 Rename Samples Structure
- [ ] `Samples/` → `samples/`
- [ ] `Samples/TimeWarp.QuickBooks.Sample.Web/` → `samples/timewarp-quickbooks-sample-web/`
- [ ] `Samples/TimeWarp.QuickBooks.Sample.Web/Properties/` → `samples/timewarp-quickbooks-sample-web/properties/`

#### 3.6 Rename Sample Files
- [ ] `Samples/TimeWarp.QuickBooks.Sample.Web/TimeWarp.QuickBooks.Sample.Web.csproj` → `samples/timewarp-quickbooks-sample-web/timewarp-quickbooks-sample-web.csproj`
- [ ] `Samples/TimeWarp.QuickBooks.Sample.Web/Program.cs` → `samples/timewarp-quickbooks-sample-web/program.cs`
- [ ] `Samples/TimeWarp.QuickBooks.Sample.Web/Properties/launchSettings.json` → `samples/timewarp-quickbooks-sample-web/properties/launch-settings.json`
- [ ] `Samples/TimeWarp.QuickBooks.Sample.Web/appsettings.Development.json` → `samples/timewarp-quickbooks-sample-web/appsettings.development.json`
- [ ] `Samples/TimeWarp.QuickBooks.Sample.Web/README.md` → `samples/timewarp-quickbooks-sample-web/readme.md`

#### 3.7 Update Build Property Files
- [ ] `Directory.Build.props` → `directory.build.props`
- [ ] `Directory.Packages.props` → `directory.packages.props`

### Phase 4: Update All References
#### 4.1 Update Solution File
- [ ] Update all project references in `timewarp-quickbooks.slnx`

#### 4.2 Update Project File References
- [ ] Update all `<ProjectReference>` paths in .csproj files
- [ ] Update all `<PackageReference>` paths if any refer to local packages

#### 4.3 Update Directory.Build.props
- [ ] Update `<PackageIcon>` path from `Assets\Logo.png` to `assets/logo.png`
- [ ] Update `<None Include="$(MSBuildThisFileDirectory)Assets\Logo.png"` to use new path
- [ ] Update `<PackageReadmeFile>` from `README.md` to `readme.md`
- [ ] Update `<None Include="$(MSBuildThisFileDirectory)README.md"` to use new path

#### 4.4 Update Script References
- [ ] Update any file paths in PowerShell scripts
- [ ] Update paths in `run-tests.ps1` (formerly RunTests.ps1)

#### 4.5 Update Documentation Cross-References
- [ ] Search and replace all internal documentation links to use new paths
- [ ] Update any references to file names in markdown files

### Phase 5: CI/CD and Workflows
- [ ] Review and update any GitHub Actions workflow references
- [ ] Update any build scripts that reference specific paths

### Phase 6: Verification
- [ ] Run `dotnet build` to ensure solution builds
- [ ] Run tests to ensure they pass
- [ ] Verify NuGet package can be created with correct metadata
- [ ] Check that all git-tracked files follow kebab-case
- [ ] Update .gitignore if needed for new paths

## Special Considerations

### Namespace Handling
- C# namespaces will automatically convert from kebab-case directories to underscore format
- Example: `timewarp-quickbooks` directory → `timewarp_quickbooks` namespace
- No code changes needed for namespace declarations

### Git History Preservation
- Use `git mv` for all renames to preserve history
- Consider doing renames in logical batches with separate commits

### Breaking Changes
- This will be a breaking change for anyone referencing the NuGet package
- Consider versioning strategy (major version bump)

### Case-Sensitive vs Case-Insensitive File Systems
- Windows/Mac are case-insensitive, Linux is case-sensitive
- May need special handling for some renames (rename to temp name first, then to final name)

## Estimated Effort
- Phase 1: 2-3 hours (documentation, safe changes)
- Phase 2: 1 hour (scripts and config)
- Phase 3: 3-4 hours (project structure, most complex)
- Phase 4: 2-3 hours (updating all references)
- Phase 5: 1 hour (CI/CD verification)
- Phase 6: 1 hour (final verification)

**Total: 10-14 hours**

## Risk Assessment
- **High Risk**: Breaking existing builds if references aren't updated correctly
- **Medium Risk**: Git history confusion if not using `git mv`
- **Low Risk**: Documentation and non-code file renames

## Rollback Plan
- Keep a branch backup before starting
- Document all changes made
- Can revert using git if needed