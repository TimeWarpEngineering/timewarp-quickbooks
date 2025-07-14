# Sync Configurable Files Workflow

This directory contains the sync workflow system for maintaining configuration file consistency with the TimeWarp Architecture repository.

## Overview

The sync workflow automatically downloads and updates configuration files from the parent `TimeWarpEngineering/timewarp-architecture` repository to ensure organizational consistency across all TimeWarp projects.

## Two-Step Deployment Process

Due to GitHub Apps security limitations that prevent direct workflow file updates, this system uses a two-step deployment:

### Step 1: Automated Generation (Completed)
✅ Workflow files have been generated in `workflows-temp/` directory:
- `sync-configurable-files.yml` - GitHub Actions workflow
- `sync-config.yml` - Configuration defining which files to sync
- `sync-configurable-files.ps1` - PowerShell script handling the sync logic

### Step 2: Manual Deployment (Required)
🔄 **Next Action Required:** Run the deployment script to move files to their final locations:

```powershell
# Preview what will be copied (recommended first)
./copy-workflows.ps1 -WhatIf

# Deploy the workflow files
./copy-workflows.ps1
```

This will copy:
- `workflows-temp/sync-configurable-files.yml` → `.github/workflows/sync-configurable-files.yml`
- `workflows-temp/sync-config.yml` → `.github/scripts/sync-config.yml`  
- `workflows-temp/sync-configurable-files.ps1` → `.github/scripts/sync-configurable-files.ps1`

## Configuration for .NET Repository

The sync configuration has been tailored for this .NET 9.0 QuickBooks integration library:

### Files That Will Be Synced
- ✅ `.editorconfig` - Code formatting standards
- ✅ `global.json` - .NET SDK version pinning
- ✅ `.gitignore` - Standard ignore patterns (merged with local additions)
- ⚠️ `Directory.Build.props` - Build configuration (local customizations preserved)
- ⚠️ `Directory.Build.targets` - Build targets (if available in parent repo)

### Files Excluded from Sync
- Local project metadata (version info, package details)
- Environment-specific configurations
- Repository-specific customizations

### GitIgnore Merge Strategy
The `.gitignore` file uses a special merge strategy:
- Global patterns are synced from the parent repository
- Local repository-specific patterns are preserved
- Section markers separate global vs. local content

## Workflow Features

### Scheduling
- Runs automatically every Sunday at 2 AM UTC
- Can be triggered manually via GitHub Actions interface
- Supports dry-run mode for testing

### Pull Request Creation
- Automatically creates PRs when changes are detected
- Includes detailed change summaries
- Uses branch naming: `sync/configurable-files-{run-number}`

### Local Preservation
- Preserves repository-specific package metadata
- Maintains local version information
- Keeps local .gitignore additions

## Testing the Workflow

After deployment, test the workflow:

```bash
# Using GitHub CLI
gh workflow run sync-configurable-files.yml --ref $(git branch --show-current)

# Or trigger manually in GitHub Actions web interface
```

For testing, use the dry-run option:
1. Go to Actions → Sync Configurable Files
2. Click "Run workflow" 
3. Set "Perform a dry run" to `true`
4. Click "Run workflow"

## Monitoring

The workflow will:
1. Download files from `TimeWarpEngineering/timewarp-architecture`
2. Apply appropriate merge strategies
3. Create backups of existing files
4. Generate a pull request if changes are detected
5. Include detailed logs of all operations

## Troubleshooting

### Common Issues

**Workflow fails with "Configuration file not found":**
- Ensure `copy-workflows.ps1` was run successfully
- Check that `.github/scripts/sync-config.yml` exists

**No changes detected when changes are expected:**
- Check the parent repository for recent updates
- Verify file paths in `sync-config.yml` are correct
- Review workflow logs for download errors

**Local customizations lost:**
- Check preserve_sections configuration in `sync-config.yml`
- File an issue to update preservation rules

### Getting Help

1. Check workflow run logs in GitHub Actions
2. Review the sync configuration in `.github/scripts/sync-config.yml`
3. Test locally with dry-run mode
4. File issues in this repository for sync-specific problems

## Security Notes

- Workflow runs with `contents: write` and `pull-requests: write` permissions
- No secrets are synced - only public configuration files
- All changes go through pull request review process
- Backups are created before any file modifications

---

**Generated with [Claude Code](https://claude.ai/code)**