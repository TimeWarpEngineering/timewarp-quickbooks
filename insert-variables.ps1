#requires -Version 7.0
# PowerShell script with Push-Location, Pop-Location, PSScriptRoot, and $HOME

# Push the current directory and switch to the script's root directory
Push-Location -Path $PSScriptRoot

try {
  # Get environment variables
  $OS = (Get-CimInstance Win32_OperatingSystem).Caption
  $SHELL = "cmd"  # Hardcoded as in the original script
  $WORKSPACE = Get-Location | Select-Object -ExpandProperty Path

  # Construct paths
  $GLOBAL_SETTINGS = "$env:APPDATA\Code\User\globalStorage\rooveterinaryinc.roo-cline\settings\cline_custom_modes.json"
  $MCP_LOCATION = "$HOME\.local\share\Roo-Code\MCP"
  $MCP_SETTINGS = "$env:APPDATA\Code\User\globalStorage\rooveterinaryinc.roo-cline\settings\cline_mcp_settings.json"

  # Directory setup
  $ROO_DIR = "$WORKSPACE\.roo"

  # Check if the .roo directory exists
  if (-not (Test-Path -Path $ROO_DIR)) {
    Write-Error ".roo directory not found in $WORKSPACE"
    exit 1
  }

  # Process files in .roo directory
  Write-Host "Working Directory: $WORKSPACE"
  $ErrorActionPreference = 'Stop'

  $files = Get-ChildItem -Path $ROO_DIR -Filter 'system-prompt-*'
  foreach ($file in $files) {
    Write-Host "Processing: $($file.FullName)"
    
    # Read file content
    $content = Get-Content -Path $file.FullName -Raw
    
    # Replace placeholders
    $content = $content -replace 'OS_PLACEHOLDER', $OS `
                        -replace 'SHELL_PLACEHOLDER', $SHELL `
                        -replace 'HOME_PLACEHOLDER', $HOME `
                        -replace 'WORKSPACE_PLACEHOLDER', $WORKSPACE `
                        -replace 'GLOBAL_SETTINGS_PLACEHOLDER', $GLOBAL_SETTINGS `
                        -replace 'MCP_LOCATION_PLACEHOLDER', $MCP_LOCATION `
                        -replace 'MCP_SETTINGS_PLACEHOLDER', $MCP_SETTINGS
    
    # Write updated content back to file
    Set-Content -Path $file.FullName -Value $content -NoNewline
    Write-Host "Completed: $($file.FullName)"
  }
}
catch {
  Write-Error "Error processing files: $_"
  exit 1
}
finally {
  # Restore the original directory
  Pop-Location
}

Write-Host "Done."