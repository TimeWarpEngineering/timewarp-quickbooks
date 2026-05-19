Push-Location $PSScriptRoot
try {
    # Analyzers Tests
    Write-Host "Running TimeWarp.QuickBooks.Tests Tests..." -ForegroundColor Cyan
    dotnet fixie tests/timewarp-quickbooks-tests
}
finally {
    Pop-Location
}
