@echo off
echo.
echo ========================================
echo   WorkHammer - Folder Publish Script
echo ========================================
echo.

echo Cleaning old publish...
if exist "publish" rd /s /q "publish"

echo Publishing to folder (win-x64)...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o ./publish --source https://api.nuget.org/v3/index.json --ignore-failed-sources

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Publish failed!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ========================================
echo   Publish Successful!
echo   Location: .\publish\
echo ========================================
echo.
pause
