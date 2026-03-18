# Android Deployment Verification Guide

## Quick Verification Commands

### 1. Check Installed App Info
```powershell
# Check version and install time
adb shell dumpsys package com.triples.aep | Select-String -Pattern "versionCode|versionName|firstInstallTime|lastUpdateTime"
```

### 2. Check Local APK Build Time
```powershell
# Verify APK timestamp matches deployment
Get-ChildItem "Triple-S-Maui-AEP\bin\Debug\net9.0-android\*.apk" | Select-Object Name, LastWriteTime, Length | Format-Table -AutoSize
```

### 3. Launch App from Command Line
```powershell
# Launch the app
adb shell am start -n com.triples.aep/crc643981185b7d03fad5.MainActivity
```

### 4. Monitor App Logs
```powershell
# View real-time logs
adb logcat -s "DOTNET" "mono" "Mono" "*:E"

# View only errors
adb logcat *:E

# Clear logs and start fresh
adb logcat -c && adb logcat -s "DOTNET"
```

## Clean Reinstall Process

When you want to ensure you have the absolute latest version:

```powershell
# Step 1: Uninstall old version
adb uninstall com.triples.aep

# Step 2: Clean build
dotnet clean
dotnet build -f net9.0-android -c Debug

# Step 3: Install fresh
adb install "Triple-S-Maui-AEP\bin\Debug\net9.0-android\com.triples.aep-Signed.apk"

# Step 4: Launch
adb shell am start -n com.triples.aep/crc643981185b7d03fad5.MainActivity
```

## Visual Verification Checklist

After deploying, verify these features exist on your device:

### ✅ Login Page (AgentLoginPage)
- [ ] Language picker visible at top
- [ ] "English" and "Español" options available
- [ ] All text changes when switching language
- [ ] Login button shows feedback when clicked

### ✅ Main Page (HomePage)
- [ ] Language picker visible
- [ ] Agent information displays correctly
- [ ] Quick action buttons work
- [ ] Logout confirmation dialog in correct language

### ✅ Enrollment Wizard Page
- [ ] Language picker visible
- [ ] SOA selection dropdown works
- [ ] All step labels translate correctly
- [ ] Form validation messages in correct language
- [ ] Signature pads have clear buttons
- [ ] PDF generation works

### ✅ SOA Wizard Page
- [ ] Language picker visible (NEWLY ADDED)
- [ ] All steps translate correctly
- [ ] Signature capture works
- [ ] PDF generation works
- [ ] Upload to OnBase works

### ✅ Dashboard Page
- [ ] Language picker visible (NEWLY ADDED)
- [ ] Statistics cards show correct data
- [ ] SOA list displays with status
- [ ] Enrollment list displays with status
- [ ] View PDF buttons work
- [ ] Upload buttons work
- [ ] Language changes update all text

## Troubleshooting Old Version Issues

### Problem: App seems to be old version
**Solution 1: Force Stop and Clear Data**
```powershell
adb shell am force-stop com.triples.aep
adb shell pm clear com.triples.aep
```

**Solution 2: Complete Uninstall/Reinstall**
```powershell
adb uninstall com.triples.aep
# Then rebuild and reinstall
```

**Solution 3: Check if Multiple APKs Exist**
```powershell
# List all APKs in build folder
Get-ChildItem "Triple-S-Maui-AEP\bin\Debug\net9.0-android\" -Recurse -Filter "*.apk"
```

### Problem: Changes not appearing
**Possible Causes:**
1. App is cached in memory - Force stop the app
2. Old APK was installed - Check APK timestamp
3. Build didn't complete - Check build output
4. Wrong APK installed - Verify filename matches

### Problem: App crashes on startup
**Debug Steps:**
```powershell
# View crash logs
adb logcat *:E

# View detailed .NET logs
adb logcat -s "DOTNET"

# Check for permissions issues
adb shell dumpsys package com.triples.aep | Select-String -Pattern "permission"
```

## Version Increment Strategy

To easily track versions, update in `.csproj`:

```xml
<PropertyGroup>
    <ApplicationDisplayVersion>1.1</ApplicationDisplayVersion>
    <ApplicationVersion>2</ApplicationVersion>
</PropertyGroup>
```

Then verify on device:
```powershell
adb shell dumpsys package com.triples.aep | Select-String -Pattern "versionCode|versionName"
```

## Quick Deploy Script

Save this as `deploy-android.ps1`:

```powershell
# Quick Android deployment script
Write-Host "🔨 Building Android app..." -ForegroundColor Cyan
dotnet build -f net9.0-android -c Debug

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful!" -ForegroundColor Green
    
    Write-Host "📦 Installing to device..." -ForegroundColor Cyan
    adb install -r "Triple-S-Maui-AEP\bin\Debug\net9.0-android\com.triples.aep-Signed.apk"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Installation successful!" -ForegroundColor Green
        Write-Host "🚀 Launching app..." -ForegroundColor Cyan
        adb shell am start -n com.triples.aep/crc643981185b7d03fad5.MainActivity
        Write-Host "✅ App launched!" -ForegroundColor Green
    }
} else {
    Write-Host "❌ Build failed!" -ForegroundColor Red
}
```

Usage: `.\deploy-android.ps1`

## Release Build for Distribution

When ready for production:

```powershell
# Create release APK
dotnet publish -f net9.0-android -c Release -p:AndroidPackageFormat=apk

# Create release AAB (for Play Store)
dotnet publish -f net9.0-android -c Release -p:AndroidPackageFormat=aab

# Sign with your keystore (production)
# Configure in .csproj:
# <AndroidSigningKeyStore>path/to/keystore.keystore</AndroidSigningKeyStore>
# <AndroidSigningKeyAlias>your-alias</AndroidSigningKeyAlias>
```

## Current Deployment Status

**Last Deployed:** March 3, 2026 @ 10:03 PM
**Version:** 1.0 (Build 1)
**Device:** RFCRB08DBSE
**Features Added:**
- Language toggles on SOAWizardPage
- Language toggles on DashboardPage
- Complete bilingual support across all pages

**APK Location:**
`Triple-S-Maui-AEP\bin\Debug\net9.0-android\com.triples.aep-Signed.apk`

**APK Size:** ~15.7 MB
**Target Android:** API 21+ (Android 5.0+)
**Compiled Against:** API 35 (Android 15)

---
**Last Updated:** March 3, 2026
