Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  WorkHammer - Standalone Build Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Cleaning old builds..."
if (Test-Path "publish_single") { Remove-Item -Path "publish_single" -Recurse -Force }

Write-Host "Building standalone executable (win-x64)..."
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false -o ./publish_single --source https://api.nuget.org/v3/index.json --ignore-failed-sources

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[ERROR] Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Build Successful!" -ForegroundColor Green
Write-Host "  Location: .\publish_single\WorkHammer.exe" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
