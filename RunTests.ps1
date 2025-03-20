Push-Location $PSScriptRoot
try {
    # Analyzers Tests
    Write-Host "Running TimeWarp.QuickBooks.Tests Tests..." -ForegroundColor Cyan
    dotnet fixie Tests/TimeWarp.QuickBooks.Tests
}
finally {
    Pop-Location
}
