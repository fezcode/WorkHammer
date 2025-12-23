Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  WorkHammer - Folder Publish Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Cleaning old publish..."
if (Test-Path "publish") { Remove-Item -Path "publish" -Recurse -Force }

Write-Host "Publishing to folder (win-x64)..."
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o ./publish --source https://api.nuget.org/v3/index.json --ignore-failed-sources

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[ERROR] Publish failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Publish Successful!" -ForegroundColor Green
Write-Host "  Location: \.\publish\" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
