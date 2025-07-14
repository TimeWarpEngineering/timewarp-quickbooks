#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Copies workflow files from workflows-temp to their proper locations.

.DESCRIPTION
    This script performs the manual step of the two-step sync workflow deployment process.
    It copies files from workflows-temp/ to .github/workflows/ and .github/scripts/ 
    directories due to GitHub Apps limitations that prevent direct workflow file updates.

.PARAMETER WhatIf
    Shows what would be copied without actually performing the operations.

.EXAMPLE
    ./copy-workflows.ps1
    
.EXAMPLE  
    ./copy-workflows.ps1 -WhatIf
#>

[CmdletBinding()]
param(
    [switch]$WhatIf
)

# Set error handling
$ErrorActionPreference = 'Stop'

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ️  $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠️  $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Ensure-Directory {
    param([string]$Path)
    
    if (-not (Test-Path -Path $Path)) {
        if ($WhatIf) {
            Write-Info "[WHAT-IF] Would create directory: $Path"
        } else {
            New-Item -Path $Path -ItemType Directory -Force | Out-Null
            Write-Success "Created directory: $Path"
        }
    }
}

function Copy-WorkflowFile {
    param(
        [string]$Source,
        [string]$Destination,
        [string]$Description
    )
    
    if (-not (Test-Path -Path $Source)) {
        Write-Warning "Source file not found: $Source"
        return $false
    }
    
    if ($WhatIf) {
        Write-Info "[WHAT-IF] Would copy $Description`: $Source -> $Destination"
        return $true
    }
    
    try {
        Copy-Item -Path $Source -Destination $Destination -Force
        Write-Success "Copied $Description`: $Destination"
        return $true
    }
    catch {
        Write-Error "Failed to copy $Description`: $($_.Exception.Message)"
        return $false
    }
}

# === Main Script Execution ===

try {
    Write-Info "Starting workflow files deployment $(if ($WhatIf) { '(WHAT-IF MODE)' })"
    
    # Check if workflows-temp directory exists
    if (-not (Test-Path -Path "workflows-temp")) {
        throw "workflows-temp directory not found. Please run the sync workflow generation first."
    }
    
    # Create necessary directories
    Ensure-Directory -Path ".github"
    Ensure-Directory -Path ".github/workflows"
    Ensure-Directory -Path ".github/scripts"
    
    # Define file mappings
    $fileMappings = @(
        @{
            Source = "workflows-temp/sync-configurable-files.yml"
            Destination = ".github/workflows/sync-configurable-files.yml"
            Description = "Sync workflow"
        },
        @{
            Source = "workflows-temp/sync-config.yml"
            Destination = ".github/scripts/sync-config.yml"
            Description = "Sync configuration"
        },
        @{
            Source = "workflows-temp/sync-configurable-files.ps1"
            Destination = ".github/scripts/sync-configurable-files.ps1"
            Description = "Sync PowerShell script"
        }
    )
    
    # Copy files
    $successCount = 0
    $totalCount = $fileMappings.Count
    
    foreach ($mapping in $fileMappings) {
        if (Copy-WorkflowFile -Source $mapping.Source -Destination $mapping.Destination -Description $mapping.Description) {
            $successCount++
        }
    }
    
    # Summary
    if ($WhatIf) {
        Write-Info "What-if analysis complete"
        Write-Info "Files that would be copied: $successCount/$totalCount"
    } else {
        if ($successCount -eq $totalCount) {
            Write-Success "All workflow files deployed successfully! ($successCount/$totalCount)"
            Write-Info ""
            Write-Info "Next steps:"
            Write-Info "1. Commit the new .github/ directory files"
            Write-Info "2. Test the workflow with: gh workflow run sync-configurable-files.yml --ref $(git branch --show-current)"
            Write-Info "3. Or trigger manually in GitHub Actions web interface"
            Write-Info ""
            Write-Info "The workflow will:"
            Write-Info "- Sync configuration files from TimeWarpEngineering/timewarp-architecture"
            Write-Info "- Create a pull request with the changes"
            Write-Info "- Preserve local customizations where configured"
        } else {
            Write-Warning "Deployment completed with some failures ($successCount/$totalCount files copied)"
            Write-Info "Please review the errors above and try again"
        }
    }
    
} catch {
    Write-Error "Deployment failed: $($_.Exception.Message)"
    exit 1
}

# Instructions for cleanup (optional)
if (-not $WhatIf -and $successCount -eq $totalCount) {
    Write-Info ""
    Write-Info "Optional: You can now remove the workflows-temp directory:"
    Write-Info "  Remove-Item -Path workflows-temp -Recurse -Force"
}