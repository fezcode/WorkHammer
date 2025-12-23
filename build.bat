@echo off
echo.
echo ========================================
echo   WorkHammer - Standalone Build Script
echo ========================================
echo.

echo Cleaning old builds...
if exist "publish_single" rd /s /q "publish_single"

echo Building standalone executable (win-x64)...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false -o ./publish_single --source https://api.nuget.org/v3/index.json --ignore-failed-sources

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build failed!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ========================================
echo   Build Successful!
echo   Location: .\publish_single\WorkHammer.exe
echo ========================================
echo.
pause
