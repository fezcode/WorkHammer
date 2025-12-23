# build_all.ps1
Write-Host ""
Write-Host "========================================" -ForegroundColor Magenta
Write-Host "  WorkHammer - Multiplatform Build" -ForegroundColor Magenta
Write-Host "========================================" -ForegroundColor Magenta
Write-Host ""

$projectName = "WorkHammer"
$outBase = "publish_dist"
$iconName = "wh-logo.icns"
$assetsDir = "Assets"

# Clean previous builds
if (Test-Path $outBase) {
    Write-Host "Cleaning old builds..." -ForegroundColor Yellow
    Remove-Item -Path $outBase -Recurse -Force
}

# Define targets
$targets = @(
    @{ Rid = "win-x64";   Name = "Windows x64";   Ext = ".exe"; IsMac = $false },
    @{ Rid = "linux-x64"; Name = "Linux x64";     Ext = "";     IsMac = $false },
    @{ Rid = "osx-arm64"; Name = "macOS (Apple Silicon)"; Ext = ""; IsMac = $true },
    @{ Rid = "osx-x64";   Name = "macOS (Intel)"; Ext = "";     IsMac = $true }
)

foreach ($t in $targets) {
    $rid = $t.Rid
    $name = $t.Name
    $targetDir = "$outBase\$rid"

        Write-Host "Building for $name ($rid)..." -ForegroundColor Cyan
        
        # Ensure target directory exists
        if (!(Test-Path $targetDir)) { New-Item -ItemType Directory -Path $targetDir | Out-Null }
    
        # For macOS, we initially publish to a temp folder inside the target dir
        $publishDir = if ($t.IsMac) { "$targetDir\temp" } else { $targetDir }
    # Standard publish command
    dotnet publish -c Release -r $rid `
        --self-contained true `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:PublishTrimmed=false `
        -p:DebugType=None `
        -p:DebugSymbols=false `
        -o $publishDir

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error building for $name (Exit Code: $LASTEXITCODE)" -ForegroundColor Red
        exit $LASTEXITCODE
    }

    # macOS .app Bundle Creation
    if ($t.IsMac) {
        $appBundle = "$targetDir\$projectName.app"
        $contents = "$appBundle\Contents"
        $macOsDir = "$contents\MacOS"
        $resources = "$contents\Resources"

        # Create structure
        New-Item -ItemType Directory -Force -Path $macOsDir | Out-Null
        New-Item -ItemType Directory -Force -Path $resources | Out-Null

        # Move Binary
        Move-Item "$publishDir\$projectName" "$macOsDir\$projectName"

        # Cleanup temp publish dir (remove pdb or other leftovers if any, then dir)
        Remove-Item $publishDir -Recurse -Force

        # Create Info.plist
        $plistContent = @"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>$projectName</string>
    <key>CFBundleDisplayName</key>
    <string>$projectName</string>
    <key>CFBundleIdentifier</key>
    <string>com.fezcode.workhammer</string>
    <key>CFBundleVersion</key>
    <string>1.0.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleSignature</key>
    <string>????</string>
    <key>CFBundleExecutable</key>
    <string>$projectName</string>
    <key>CFBundleIconFile</key>
    <string>$iconName</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
"@
        Set-Content -Path "$contents\Info.plist" -Value $plistContent

        # Copy Icon if exists
        if (Test-Path "$assetsDir\$iconName") {
            Copy-Item "$assetsDir\$iconName" "$resources\$iconName"
        } elseif (Test-Path "$assetsDir\wh-logo.png") {
            Write-Host "  [INFO] Using wh-logo.png as fallback (Note: macOS prefers .icns)" -ForegroundColor Gray
            Copy-Item "$assetsDir\wh-logo.png" "$resources\wh-logo.png"
            # Update plist to point to png if that's all we have
            (Get-Content "$contents\Info.plist") -replace $iconName, "wh-logo.png" | Set-Content "$contents\Info.plist"
        } else {
            Write-Host "  [WARN] Icon file not found in Assets. App will have default system icon." -ForegroundColor Yellow
        }
    }

    # Compression Step
    Write-Host "Compressing output for $name..." -ForegroundColor Cyan
    $zipPath = "$outBase\$projectName-$rid.zip"
    if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
    
    # Compress-Archive has issues with some paths or in-use files, adding a small delay or retry isn't usually needed but good to know.
    # We zip the *content* of the target dir so the user gets the file/app directly, or the folder itself?
    # Standard practice: Zip the folder contents if it's a single file, or the .app bundle itself.
    
    if ($t.IsMac) {
        # Zip the .app bundle directory
        Compress-Archive -Path "$targetDir\$projectName.app" -DestinationPath $zipPath -Force
    } else {
        # Zip everything inside the target dir (exe + maybe config)
        Compress-Archive -Path "$targetDir\*" -DestinationPath $zipPath -Force
    }
    Write-Host "  -> Created: $zipPath" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  All Builds Successful!" -ForegroundColor Green
Write-Host "  Output directory: .\$outBase" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""