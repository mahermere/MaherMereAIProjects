# Android Deployment Guide - Triple-S MAUI AEP

## Prerequisites

### 1. ✅ Android Device Setup

**Option A: Physical Device (Recommended for testing)**
1. Enable **Developer Options** on your Android device:
   - Go to Settings → About Phone
   - Tap "Build Number" 7 times
   - Go back to Settings → System → Developer Options
2. Enable **USB Debugging**
3. Connect your device via USB cable
4. Accept the "Allow USB Debugging" prompt on your device

**Option B: Android Emulator**
1. Open Visual Studio → Tools → Android → Android Device Manager
2. Create or start an emulator (e.g., Pixel 5 - API 34)

### 2. ✅ Verify Android SDK

Check if Android SDK is installed:
```powershell
$env:ANDROID_HOME
# Should show: C:\Program Files\Android\android-sdk (or similar)
```

If not installed:
- Visual Studio Installer → Modify → Mobile development with .NET

---

## Quick Deploy (Visual Studio)

### Option 1: Deploy via Visual Studio GUI ✅

**Steps:**

1. **Select Android as Target Platform:**
   - In Visual Studio toolbar, find the dropdown that says "Windows Machine"
   - Change it to "Android Local Devices" or "Android Emulators"

2. **Select Your Device:**
   - Click the device dropdown next to the platform dropdown
   - Select your connected device or running emulator

3. **Deploy:**
   - Press **F5** (Debug) or **Ctrl+F5** (Release without debugging)
   - Visual Studio will:
     - Build the project
     - Create the APK
     - Install it on your device
     - Launch the app

**Build Output Location:**
```
Triple-S-Maui-AEP\bin\Debug\net9.0-android\
Triple-S-Maui-AEP-Signed.apk
```

---

## Manual Deploy (Command Line)

### Step 1: Build the APK

**Debug Build:**
```powershell
# Navigate to project directory
cd Triple-S-Maui-AEP

# Build for Android
dotnet build -t:Run -f net9.0-android
```

**Release Build (Optimized):**
```powershell
dotnet publish -f net9.0-android -c Release
```

### Step 2: Check Connected Devices

```powershell
# List connected Android devices
adb devices
```

**Expected output:**
```
List of devices attached
XXXXXXXXXX      device
```

If you see "unauthorized", accept the USB debugging prompt on your device.

### Step 3: Install the APK

**Install Debug APK:**
```powershell
adb install -r "bin\Debug\net9.0-android\com.triples.aep-Signed.apk"
```

**Install Release APK:**
```powershell
adb install -r "bin\Release\net9.0-android\publish\com.triples.aep-Signed.apk"
```

The `-r` flag reinstalls if already installed.

### Step 4: Launch the App

```powershell
adb shell am start -n com.triples.aep/crc64e8e8e8e8e8e8e8.MainActivity
```

Or just tap the "Triple-S Annual Enrollment" icon on your device!

---

## Automated Deployment Script

Save this as `Deploy-Android.ps1`:

```powershell
# Triple-S MAUI AEP - Android Deployment Script

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    
    [Parameter(Mandatory=$false)]
    [switch]$Launch
)

Write-Host "`n=== Triple-S MAUI AEP - Android Deployment ===" -ForegroundColor Cyan

# Step 1: Check for connected devices
Write-Host "`nStep 1: Checking for connected Android devices..." -ForegroundColor Yellow
$devices = adb devices | Select-String "device$"
if ($devices.Count -eq 0) {
    Write-Host "ERROR: No Android devices connected!" -ForegroundColor Red
    Write-Host "Please connect your device or start an emulator." -ForegroundColor Red
    exit 1
}
Write-Host "Found $($devices.Count) device(s)" -ForegroundColor Green

# Step 2: Build the project
Write-Host "`nStep 2: Building $Configuration configuration..." -ForegroundColor Yellow
if ($Configuration -eq "Debug") {
    dotnet build -f net9.0-android -c Debug
} else {
    dotnet publish -f net9.0-android -c Release
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "Build successful!" -ForegroundColor Green

# Step 3: Find the APK
Write-Host "`nStep 3: Locating APK..." -ForegroundColor Yellow
if ($Configuration -eq "Debug") {
    $apkPath = "bin\Debug\net9.0-android\com.triples.aep-Signed.apk"
} else {
    $apkPath = "bin\Release\net9.0-android\publish\com.triples.aep-Signed.apk"
}

if (-not (Test-Path $apkPath)) {
    Write-Host "ERROR: APK not found at: $apkPath" -ForegroundColor Red
    exit 1
}
Write-Host "APK found: $apkPath" -ForegroundColor Green
$apkSize = (Get-Item $apkPath).Length / 1MB
Write-Host "APK size: $($apkSize.ToString('0.00')) MB" -ForegroundColor Green

# Step 4: Install the APK
Write-Host "`nStep 4: Installing APK on device..." -ForegroundColor Yellow
adb install -r $apkPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Installation failed!" -ForegroundColor Red
    exit 1
}
Write-Host "Installation successful!" -ForegroundColor Green

# Step 5: Launch the app (optional)
if ($Launch) {
    Write-Host "`nStep 5: Launching app..." -ForegroundColor Yellow
    adb shell am start -n com.triples.aep/crc64e8e8e8e8e8e8e8.MainActivity
    Write-Host "App launched!" -ForegroundColor Green
}

Write-Host "`n=== Deployment Complete! ===" -ForegroundColor Cyan
Write-Host "The Triple-S Annual Enrollment app is now installed on your device.`n" -ForegroundColor Green
```

**Usage:**

```powershell
# Debug build and install
.\Deploy-Android.ps1

# Release build and install
.\Deploy-Android.ps1 -Configuration Release

# Debug build, install, and launch
.\Deploy-Android.ps1 -Launch

# Release build, install, and launch
.\Deploy-Android.ps1 -Configuration Release -Launch
```

---

## Troubleshooting

### Issue 1: "adb is not recognized"

**Cause:** Android SDK tools not in PATH

**Fix:**
```powershell
# Add to PATH temporarily
$env:Path += ";C:\Program Files\Android\android-sdk\platform-tools"

# Or permanently:
# System Properties → Environment Variables → Path → Add:
# C:\Program Files\Android\android-sdk\platform-tools
```

### Issue 2: "INSTALL_FAILED_UPDATE_INCOMPATIBLE"

**Cause:** Trying to install over an existing app with a different signature

**Fix:**
```powershell
# Uninstall the existing app first
adb uninstall com.triples.aep

# Then reinstall
adb install bin\Debug\net9.0-android\com.triples.aep-Signed.apk
```

### Issue 3: "device unauthorized"

**Cause:** USB debugging not authorized on device

**Fix:**
1. Check your device screen for "Allow USB Debugging" prompt
2. Check "Always allow from this computer"
3. Tap "OK"
4. Run `adb devices` again

### Issue 4: "No devices found"

**Cause:** Device not connected or drivers missing

**Fix:**
1. Check USB cable connection
2. Try a different USB port
3. Install device-specific USB drivers (if needed)
4. Run `adb kill-server` then `adb start-server`

### Issue 5: Build fails with "AndroidApiLevel 36"

**Cause:** Android SDK API 36 not installed

**Fix:**
```powershell
# Check installed APIs
sdkmanager --list_installed

# Install API 36
sdkmanager "platforms;android-36"
```

Or change in `.csproj`:
```xml
<AndroidApiLevel>34</AndroidApiLevel>  <!-- Use API 34 instead -->
```

### Issue 6: "App crashes immediately on launch"

**Check logs:**
```powershell
adb logcat | Select-String "Triple-S\|Triple_S\|crash\|exception"
```

Common causes:
- Missing Android permissions
- OnBase authentication service initialization
- Language service initialization

---

## Build Configurations

### Current Android Settings (from .csproj)

```xml
<ApplicationId>com.triples.aep</ApplicationId>
<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
<ApplicationVersion>1</ApplicationVersion>
<AndroidApiLevel>36</AndroidApiLevel>
<SupportedOSPlatformVersion>21.0</SupportedOSPlatformVersion>
<AndroidLinkMode>None</AndroidLinkMode>
<EmbedAssembliesIntoApk>true</EmbedAssembliesIntoApk>
```

**Minimum Android Version:** Android 5.0 (API 21)  
**Target Android Version:** Android 14 (API 36)

---

## APK Locations

**Debug Build:**
```
Triple-S-Maui-AEP\bin\Debug\net9.0-android\com.triples.aep-Signed.apk
```

**Release Build:**
```
Triple-S-Maui-AEP\bin\Release\net9.0-android\publish\com.triples.aep-Signed.apk
```

---

## Testing Checklist

After deployment, test:

- [ ] App launches successfully
- [ ] Login page loads
- [ ] Can enter NPN and password
- [ ] Login authenticates (check network connectivity)
- [ ] Dashboard loads
- [ ] SOA wizard opens
- [ ] Enrollment wizard opens
- [ ] Signature pad works
- [ ] PDF generation works
- [ ] Document upload works (to DMS)
- [ ] Language toggle works (English/Spanish)
- [ ] App doesn't crash on navigation

---

## Viewing App Logs

**Real-time logs:**
```powershell
adb logcat | Select-String "Triple"
```

**Save logs to file:**
```powershell
adb logcat > android-logs.txt
```

**Clear logs:**
```powershell
adb logcat -c
```

---

## Uninstalling the App

```powershell
adb uninstall com.triples.aep
```

---

## Distribution (Production)

For production deployment:

1. **Generate a Release Keystore:**
```powershell
keytool -genkey -v -keystore triple-s-aep.keystore -alias triple-s-key -keyalg RSA -keysize 2048 -validity 10000
```

2. **Update .csproj with signing:**
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidKeyStore>true</AndroidKeyStore>
  <AndroidSigningKeyStore>triple-s-aep.keystore</AndroidSigningKeyStore>
  <AndroidSigningKeyAlias>triple-s-key</AndroidSigningKeyAlias>
  <AndroidSigningKeyPass>YOUR_PASSWORD</AndroidSigningKeyPass>
  <AndroidSigningStorePass>YOUR_PASSWORD</AndroidSigningStorePass>
</PropertyGroup>
```

3. **Build signed APK:**
```powershell
dotnet publish -f net9.0-android -c Release
```

4. **Distribute via:**
   - Google Play Store
   - Internal MDM system
   - Direct APK distribution

---

## Quick Reference

| Task | Command |
|------|---------|
| Build Debug | `dotnet build -f net9.0-android` |
| Build Release | `dotnet publish -f net9.0-android -c Release` |
| List devices | `adb devices` |
| Install APK | `adb install -r path/to/app.apk` |
| Uninstall | `adb uninstall com.triples.aep` |
| Launch app | `adb shell am start -n com.triples.aep/...` |
| View logs | `adb logcat` |
| Screenshot | `adb exec-out screencap -p > screenshot.png` |

---

**Need Help?**

Check the diagnostic output or share the error message for specific troubleshooting!
