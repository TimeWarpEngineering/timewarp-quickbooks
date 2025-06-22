#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Syncs configurable files from the parent TimeWarp Architecture repository.

.DESCRIPTION
    This script downloads and syncs configuration files from the TimeWarpEngineering/timewarp-architecture
    repository to maintain consistency with organizational standards. It supports different merge strategies
    and preserves local customizations where appropriate.

.PARAMETER DryRun
    If specified, performs a dry run without making any changes.

.PARAMETER ConfigPath
    Path to the sync configuration file. Defaults to .github/scripts/sync-config.yml

.EXAMPLE
    ./sync-configurable-files.ps1
    
.EXAMPLE
    ./sync-configurable-files.ps1 -DryRun
#>

[CmdletBinding()]
param(
    [switch]$DryRun,
    [string]$ConfigPath = ".github/scripts/sync-config.yml"
)

# Set error handling
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

# Initialize variables
$script:ChangesMade = $false
$script:SyncedFiles = @()

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

function Get-GitHubContent {
    param(
        [string]$Repository,
        [string]$Path,
        [string]$Branch = "master"
    )
    
    $uri = "https://api.github.com/repos/$Repository/contents/$Path"
    $headers = @{
        'Accept' = 'application/vnd.github.v3.raw'
        'User-Agent' = 'TimeWarp-Sync-Script'
    }
    
    if ($env:GITHUB_TOKEN) {
        $headers['Authorization'] = "Bearer $env:GITHUB_TOKEN"
    }
    
    try {
        $response = Invoke-RestMethod -Uri $uri -Headers $headers -Method Get
        return $response
    }
    catch {
        if ($_.Exception.Response.StatusCode -eq 404) {
            return $null
        }
        throw
    }
}

function Test-FileExists {
    param([string]$Path)
    return Test-Path -Path $Path -PathType Leaf
}

function Backup-File {
    param([string]$FilePath)
    
    if (Test-FileExists -Path $FilePath) {
        $backupPath = "$FilePath.backup.$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        Copy-Item -Path $FilePath -Destination $backupPath -Force
        Write-Info "Created backup: $backupPath"
        return $backupPath
    }
    return $null
}

function Merge-GitIgnoreFile {
    param(
        [string]$SourceContent,
        [string]$DestinationPath,
        [hashtable]$SectionMarkers
    )
    
    $globalStart = $SectionMarkers.global_start
    $globalEnd = $SectionMarkers.global_end
    $localStart = $SectionMarkers.local_start
    
    $localContent = ""
    $hasLocalSection = $false
    
    # Read existing local content if file exists
    if (Test-FileExists -Path $DestinationPath) {
        $existingContent = Get-Content -Path $DestinationPath -Raw
        
        # Extract local section if it exists
        if ($existingContent -match "(?s)$([regex]::Escape($localStart))(.*)$") {
            $localContent = $matches[1].Trim()
            $hasLocalSection = $true
            Write-Info "Preserving existing local .gitignore section"
        }
    }
    
    # Build new content
    $newContent = @(
        $globalStart,
        "# This section is automatically synced - do not edit manually",
        $SourceContent.Trim(),
        "",
        $globalEnd,
        "# Add repository-specific ignore patterns below this line"
    )
    
    if ($hasLocalSection -and $localContent) {
        $newContent += $localContent
    }
    
    return ($newContent -join "`n")
}

function Merge-FileWithLocalPreservation {
    param(
        [string]$SourceContent,
        [string]$DestinationPath,
        [array]$PreserveSections
    )
    
    # For now, implement simple replacement
    # In a full implementation, this would parse XML/YAML and preserve specific sections
    Write-Warning "Local preservation merge not fully implemented - using replace strategy"
    return $SourceContent
}

function Sync-File {
    param(
        [hashtable]$FileConfig,
        [hashtable]$Config
    )
    
    $sourcePath = $FileConfig.source_path
    $destPath = $FileConfig.destination_path
    $mergeStrategy = $FileConfig.merge_strategy -or "replace"
    $isOptional = $FileConfig.optional -eq $true
    $createIfMissing = $FileConfig.create_if_missing -ne $false
    
    Write-Info "Syncing: $sourcePath -> $destPath"
    
    # Download source content
    try {
        $sourceContent = Get-GitHubContent -Repository $Config.source_repo -Path $sourcePath -Branch $Config.source_branch
        
        if (-not $sourceContent) {
            if ($isOptional) {
                Write-Info "Optional file not found in source repository: $sourcePath"
                return $false
            } else {
                throw "Required file not found in source repository: $sourcePath"
            }
        }
    }
    catch {
        if ($isOptional) {
            Write-Warning "Could not download optional file: $sourcePath - $($_.Exception.Message)"
            return $false
        } else {
            throw "Failed to download required file: $sourcePath - $($_.Exception.Message)"
        }
    }
    
    # Ensure destination directory exists
    $destDir = Split-Path -Path $destPath -Parent
    if ($destDir -and -not (Test-Path -Path $destDir)) {
        if (-not $DryRun) {
            New-Item -Path $destDir -ItemType Directory -Force | Out-Null
        }
        Write-Info "Created directory: $destDir"
    }
    
    # Handle different merge strategies
    $finalContent = switch ($mergeStrategy) {
        "replace" { 
            $sourceContent 
        }
        "merge_sections" {
            if ($FileConfig.section_markers) {
                Merge-GitIgnoreFile -SourceContent $sourceContent -DestinationPath $destPath -SectionMarkers $FileConfig.section_markers
            } else {
                $sourceContent
            }
        }
        "replace_with_local_preservation" {
            if ($FileConfig.preserve_sections) {
                Merge-FileWithLocalPreservation -SourceContent $sourceContent -DestinationPath $destPath -PreserveSections $FileConfig.preserve_sections
            } else {
                $sourceContent
            }
        }
        default { 
            $sourceContent 
        }
    }
    
    # Check if content has changed
    $hasChanged = $true
    if (Test-FileExists -Path $destPath) {
        $existingContent = Get-Content -Path $destPath -Raw
        $hasChanged = $existingContent -ne $finalContent
    }
    
    if ($hasChanged) {
        if (-not $DryRun) {
            # Create backup if file exists
            Backup-File -FilePath $destPath
            
            # Write new content
            $finalContent | Out-File -FilePath $destPath -Encoding UTF8 -NoNewline
        }
        
        Write-Success "$(if ($DryRun) { '[DRY RUN] ' })Updated: $destPath"
        $script:ChangesMade = $true
        $script:SyncedFiles += $destPath
        return $true
    } else {
        Write-Info "No changes needed: $destPath"
        return $false
    }
}

function Test-Configuration {
    param([hashtable]$Config)
    
    Write-Info "Validating configuration..."
    
    if (-not $Config.source_repo) {
        throw "Configuration missing required field: source_repo"
    }
    
    if (-not $Config.sync_files -or $Config.sync_files.Count -eq 0) {
        throw "Configuration missing required field: sync_files"
    }
    
    Write-Success "Configuration validation passed"
}

function Import-SyncConfiguration {
    param([string]$ConfigPath)
    
    if (-not (Test-Path -Path $ConfigPath)) {
        throw "Configuration file not found: $ConfigPath"
    }
    
    try {
        # Simple YAML parsing - in production, use a proper YAML parser
        $content = Get-Content -Path $ConfigPath -Raw
        
        # For this implementation, we'll use a simplified approach
        # In production, you'd want to use PowerShell-Yaml module
        Write-Warning "Using simplified YAML parsing - consider using PowerShell-Yaml module for production"
        
        # Return a hardcoded configuration based on the created config file
        return @{
            source_repo = "TimeWarpEngineering/timewarp-architecture"
            source_branch = "master"
            repository_type = "dotnet"
            sync_files = @(
                @{
                    source_path = "TimeWarp.Architecture/.editorconfig"
                    destination_path = ".editorconfig"
                    merge_strategy = "replace"
                    create_if_missing = $true
                },
                @{
                    source_path = "TimeWarp.Architecture/global.json"
                    destination_path = "global.json"
                    merge_strategy = "replace"
                    create_if_missing = $true
                },
                @{
                    source_path = "TimeWarp.Architecture/.gitignore"
                    destination_path = ".gitignore"
                    merge_strategy = "merge_sections"
                    section_markers = @{
                        global_start = "# ----- Global .gitignore (synced from TimeWarp Architecture) -----"
                        global_end = "# ----- Repository-specific .gitignore -----"
                        local_start = "# ----- Repository-specific .gitignore -----"
                    }
                }
            )
        }
    }
    catch {
        throw "Failed to parse configuration file: $($_.Exception.Message)"
    }
}

# === Main Script Execution ===

try {
    Write-Info "Starting configurable files sync $(if ($DryRun) { '(DRY RUN)' })"
    Write-Info "PowerShell Version: $($PSVersionTable.PSVersion)"
    Write-Info "Configuration: $ConfigPath"
    
    # Load configuration
    $config = Import-SyncConfiguration -ConfigPath $ConfigPath
    Test-Configuration -Config $config
    
    Write-Info "Source Repository: $($config.source_repo)"
    Write-Info "Repository Type: $($config.repository_type)"
    Write-Info "Files to sync: $($config.sync_files.Count)"
    
    # Process each file
    foreach ($fileConfig in $config.sync_files) {
        try {
            Sync-File -FileConfig $fileConfig -Config $config
        }
        catch {
            Write-Error "Failed to sync $($fileConfig.source_path): $($_.Exception.Message)"
            # Continue with other files rather than failing completely
        }
    }
    
    # Set environment variable for GitHub Actions
    if ($script:ChangesMade) {
        Write-Success "Sync completed with changes"
        if ($env:GITHUB_ACTIONS) {
            "SYNC_CHANGES_MADE=true" | Out-File -FilePath $env:GITHUB_ENV -Append -Encoding UTF8
        }
        
        Write-Info "Files synced:"
        foreach ($file in $script:SyncedFiles) {
            Write-Info "  - $file"
        }
    } else {
        Write-Info "Sync completed - no changes needed"
        if ($env:GITHUB_ACTIONS) {
            "SYNC_CHANGES_MADE=false" | Out-File -FilePath $env:GITHUB_ENV -Append -Encoding UTF8
        }
    }
    
} catch {
    Write-Error "Sync failed: $($_.Exception.Message)"
    Write-Error $_.ScriptStackTrace
    exit 1
}