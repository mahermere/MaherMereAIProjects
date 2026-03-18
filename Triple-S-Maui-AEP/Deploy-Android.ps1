# Triple-S MAUI AEP - Android Deployment Script

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    
    [Parameter(Mandatory=$false)]
    [switch]$Launch,
    
    [Parameter(Mandatory=$false)]
    [switch]$Uninstall
)

Write-Host "`n╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Triple-S MAUI AEP - Android Deployment                  ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

# Find project directory
$currentDir = Get-Location
$projectDir = $null

# Check if we're already in the project directory
if (Test-Path "Triple-S-Maui-AEP.csproj") {
    $projectDir = $currentDir
} elseif (Test-Path "Triple-S-Maui-AEP\Triple-S-Maui-AEP.csproj") {
    $projectDir = Join-Path $currentDir "Triple-S-Maui-AEP"
} else {
    Write-Host "ERROR: Cannot find Triple-S-Maui-AEP.csproj!" -ForegroundColor Red
    Write-Host "Please run this script from the solution root directory." -ForegroundColor Yellow
    exit 1
}

Set-Location $projectDir
Write-Host "Working directory: $(Get-Location)" -ForegroundColor Gray

# Step 1: Check for ADB
Write-Host "`n[Step 1/6] Checking for ADB..." -ForegroundColor Yellow
$adbPath = Get-Command adb -ErrorAction SilentlyContinue
if (-not $adbPath) {
    Write-Host "ERROR: ADB not found in PATH!" -ForegroundColor Red
    Write-Host "Add Android SDK platform-tools to your PATH:" -ForegroundColor Yellow
    Write-Host "  C:\Program Files\Android\android-sdk\platform-tools" -ForegroundColor Yellow
    exit 1
}
Write-Host "✓ ADB found: $($adbPath.Source)" -ForegroundColor Green

# Step 2: Check for connected devices
Write-Host "`n[Step 2/6] Checking for connected Android devices..." -ForegroundColor Yellow
$devicesOutput = adb devices 2>&1 | Out-String
$devices = $devicesOutput -split "`n" | Where-Object { $_ -match '\s+device$' }

if ($devices.Count -eq 0) {
    Write-Host "ERROR: No Android devices connected!" -ForegroundColor Red
    Write-Host "`nTroubleshooting:" -ForegroundColor Yellow
    Write-Host "  1. Connect your Android device via USB" -ForegroundColor Gray
    Write-Host "  2. Enable Developer Options (tap Build Number 7 times)" -ForegroundColor Gray
    Write-Host "  3. Enable USB Debugging in Developer Options" -ForegroundColor Gray
    Write-Host "  4. Accept 'Allow USB Debugging' prompt on device" -ForegroundColor Gray
    Write-Host "  5. Or start an Android Emulator from Android Device Manager`n" -ForegroundColor Gray
    exit 1
}

Write-Host "✓ Found $($devices.Count) device(s):" -ForegroundColor Green
foreach ($device in $devices) {
    $deviceId = ($device -split '\s+')[0]
    Write-Host "  - $deviceId" -ForegroundColor Cyan
}

# Step 3: Handle uninstall if requested
if ($Uninstall) {
    Write-Host "`n[Optional] Uninstalling existing app..." -ForegroundColor Yellow
    adb uninstall com.triples.aep 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ App uninstalled" -ForegroundColor Green
    } else {
        Write-Host "⚠ App was not installed or already uninstalled" -ForegroundColor Gray
    }
}

# Step 4: Build the project
Write-Host "`n[Step 3/6] Building $Configuration configuration..." -ForegroundColor Yellow
Write-Host "This may take a few minutes on first build...`n" -ForegroundColor Gray

$buildStartTime = Get-Date

if ($Configuration -eq "Debug") {
    dotnet build -f net9.0-android -c Debug
} else {
    dotnet publish -f net9.0-android -c Release
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nERROR: Build failed!" -ForegroundColor Red
    Write-Host "Check the build output above for errors." -ForegroundColor Yellow
    exit 1
}

$buildTime = [math]::Round(((Get-Date) - $buildStartTime).TotalSeconds, 1)
Write-Host "✓ Build successful! (took $buildTime seconds)" -ForegroundColor Green

# Step 5: Find the APK
Write-Host "`n[Step 4/6] Locating APK..." -ForegroundColor Yellow

if ($Configuration -eq "Debug") {
    $apkPath = "bin\Debug\net9.0-android\com.triples.aep-Signed.apk"
} else {
    $apkPath = "bin\Release\net9.0-android\publish\com.triples.aep-Signed.apk"
}

if (-not (Test-Path $apkPath)) {
    Write-Host "ERROR: APK not found at: $apkPath" -ForegroundColor Red
    Write-Host "`nLooking for APK in bin directory..." -ForegroundColor Yellow
    Get-ChildItem -Path "bin" -Filter "*.apk" -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
        Write-Host "  Found: $($_.FullName)" -ForegroundColor Gray
    }
    exit 1
}

$apkFullPath = (Resolve-Path $apkPath).Path
$apkSize = (Get-Item $apkPath).Length / 1MB
Write-Host "✓ APK found!" -ForegroundColor Green
Write-Host "  Path: $apkFullPath" -ForegroundColor Cyan
Write-Host "  Size: $($apkSize.ToString('0.00')) MB" -ForegroundColor Cyan

# Step 6: Install the APK
Write-Host "`n[Step 5/6] Installing APK on device..." -ForegroundColor Yellow
Write-Host "This will reinstall if the app already exists...`n" -ForegroundColor Gray

$installOutput = adb install -r "$apkFullPath" 2>&1 | Out-String
Write-Host $installOutput -ForegroundColor Gray

if ($LASTEXITCODE -ne 0 -or $installOutput -match "Failure") {
    Write-Host "ERROR: Installation failed!" -ForegroundColor Red
    
    if ($installOutput -match "INSTALL_FAILED_UPDATE_INCOMPATIBLE") {
        Write-Host "`nThe app signature doesn't match the existing installation." -ForegroundColor Yellow
        Write-Host "Uninstalling and retrying..." -ForegroundColor Yellow
        adb uninstall com.triples.aep 2>&1 | Out-Null
        
        Write-Host "Reinstalling..." -ForegroundColor Yellow
        $installOutput = adb install "$apkFullPath" 2>&1 | Out-String
        Write-Host $installOutput -ForegroundColor Gray
        
        if ($LASTEXITCODE -eq 0 -and $installOutput -notmatch "Failure") {
            Write-Host "✓ Installation successful after uninstall!" -ForegroundColor Green
        } else {
            Write-Host "ERROR: Installation still failed!" -ForegroundColor Red
            exit 1
        }
    } else {
        exit 1
    }
} else {
    Write-Host "✓ Installation successful!" -ForegroundColor Green
}

# Step 7: Launch the app (optional)
if ($Launch) {
    Write-Host "`n[Step 6/6] Launching app..." -ForegroundColor Yellow
    
    # Get the MainActivity class name
    $activityName = "crc64e1b6b8c3a0e5c1e9.MainActivity"
    
    adb shell am start -n "com.triples.aep/$activityName" 2>&1 | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ App launched!" -ForegroundColor Green
        Write-Host "`nMonitoring app logs (press Ctrl+C to stop)..." -ForegroundColor Yellow
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
        adb logcat | Select-String "Triple|Exception|Error"
    } else {
        Write-Host "⚠ Could not launch app automatically" -ForegroundColor Yellow
        Write-Host "  Please launch 'Triple-S Annual Enrollment' from your device" -ForegroundColor Gray
    }
} else {
    Write-Host "`n[Complete] App installed but not launched" -ForegroundColor Gray
    Write-Host "  Launch 'Triple-S Annual Enrollment' from your device" -ForegroundColor Gray
}

Write-Host "`n╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Deployment Complete! ✓                                   ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

Write-Host "App Information:" -ForegroundColor Yellow
Write-Host "  Name: Triple-S Annual Enrollment" -ForegroundColor Cyan
Write-Host "  Package: com.triples.aep" -ForegroundColor Cyan
Write-Host "  Version: 1.0 (Build 1)" -ForegroundColor Cyan
Write-Host "  Configuration: $Configuration" -ForegroundColor Cyan

Write-Host "`nUseful Commands:" -ForegroundColor Yellow
Write-Host "  View logs:     adb logcat | Select-String Triple" -ForegroundColor Gray
Write-Host "  Uninstall:     adb uninstall com.triples.aep" -ForegroundColor Gray
Write-Host "  Screenshot:    adb exec-out screencap -p > screenshot.png`n" -ForegroundColor Gray
