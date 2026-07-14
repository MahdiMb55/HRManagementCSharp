param(
    [string]$PublishDirectory = "artifacts\publish\win-x64"
)

$ErrorActionPreference = "Stop"
$root = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot "..")).Path
$publishPath = Join-Path $root $PublishDirectory
$requiredFiles = @(
    "HRManagement.WinForms.exe",
    "HRManagement.WinForms.dll",
    "HRManagement.Application.dll",
    "HRManagement.Infrastructure.dll",
    "HRManagement.Domain.dll",
    "Resources\Fonts\Vazirmatn-Variable.ttf",
    "Resources\Fonts\OFL.txt"
)

if (-not (Test-Path -LiteralPath $publishPath)) {
    throw "Publish directory does not exist: $publishPath"
}

foreach ($file in $requiredFiles) {
    $path = Join-Path $publishPath $file
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing publish artifact: $file"
    }
}

$developmentDatabase = Join-Path $publishPath "hr-management.db"
if (Test-Path -LiteralPath $developmentDatabase) {
    throw "Development database must not be published at root: $developmentDatabase"
}

$dataDirectory = Join-Path $publishPath "Data"
if (Test-Path -LiteralPath $dataDirectory) {
    $dbFiles = Get-ChildItem -LiteralPath $dataDirectory -Recurse -Filter "*.db" -ErrorAction SilentlyContinue
    if ($dbFiles.Count -gt 0) {
        throw "Publish output contains database files under Data. Runtime data must be created on first run."
    }
}

Write-Host "Publish verification passed for $publishPath"
