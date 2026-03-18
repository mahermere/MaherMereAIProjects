# App Crash Before Login - Diagnostic Guide

## Issue
The app is crashing during startup **before** the login page appears.

## Most Common Causes

### 1. ✅ Service Initialization Error
- LanguageService initialization failing
- ServiceLocator not properly initialized
- Dependency injection issue

### 2. ✅ View/Page Loading Error
- MainPage.xaml syntax error
- AppShell.xaml issue
- XAML parsing error

### 3. ✅ Resource Loading Error
- Missing fonts
- Missing embedded resources
- Bad resource references

### 4. ✅ Platform-Specific Issue
- Android-specific crash
- Windows-specific crash
- Framework issue

---

## How to Diagnose

### Step 1: Check Visual Studio Output Window

**Visual Studio → Debug → Windows → Output**

Look for error messages like:
```
FATAL ERROR in App constructor: [error message]
```

or stack traces showing where it crashed.

### Step 2: Run in Debug Mode

Press **F5** to run in debug mode (if not already doing so)

The app should pause at the exception.

### Step 3: Check the Stack Trace

The stack trace will show:
1. **Which method** threw the exception
2. **Which file and line** caused the crash
3. **The exact error** that occurred

### Step 4: Common Locations to Check

If you see these in the error, it's likely the cause:

**App.xaml.cs:**
```
Error in App constructor: ...
```

**MauiProgram.cs:**
```
Error in CreateMauiApp: ...
```

**Language initialization:**
```
Error setting language: ...
```

**LanguageService:**
```
Singleton initialization failed: ...
```

---

## Quick Fixes to Try

### Option 1: Temporarily Disable Language Service

Edit `App.xaml.cs`:

```csharp
public App()
{
    try
    {
        InitializeComponent();
        
        // TEMPORARILY DISABLE - Comment out to test
        /*
        try
        {
            SetCultureFromLanguage(Services.LanguageService.Instance.CurrentLanguage);
            Services.LanguageService.Instance.LanguageChanged += lang => SetCultureFromLanguage(lang);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error setting language: {ex.Message}");
        }
        */
        
        Debug.WriteLine("App initialized successfully (language service disabled for testing)");
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"FATAL ERROR in App constructor: {ex.Message}");
        Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        throw;
    }
}
```

**Then run and see if app starts.**

### Option 2: Disable AppShell

Edit `App.xaml.cs`, line 50:

```csharp
protected override Window CreateWindow(IActivationState? activationState)
{
    try
    {
        // Temporarily use MainPage directly instead of AppShell
        return new Window(new MainPage());
        
        // ORIGINAL - Temporarily commented out
        // return new Window(new AppShell());
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error creating window: {ex.Message}");
        throw;
    }
}
```

**Then run and see if app starts.**

### Option 3: Disable PDF Opener Registration (Android)

Edit `MauiProgram.cs`, comment out lines 37-41:

```csharp
/*
#if ANDROID
    var pdfOpener = app.Services.GetService<Triple_S_Maui_AEP.Services.IPdfOpener>();
    if (pdfOpener != null)
        Triple_S_Maui_AEP.Services.ServiceLocator.PdfOpener = pdfOpener;
#endif
*/
```

**Then run and see if app starts.**

---

## What I Need From You

To help diagnose the crash, please share:

1. **The exact error message** from the Output window
2. **The stack trace** (full if possible)
3. **Which platform** you're running on (Android, Windows, iOS, etc.)
4. **When did it start crashing** (after my latest changes?)

Example:
```
FATAL ERROR in App constructor: Object reference not set to an instance of an object
Stack trace:
   at Triple_S_Maui_AEP.Services.LanguageService.get_Instance()
   at Triple_S_Maui_AEP.App..ctor() in App.xaml.cs:line 17
```

---

## Temporary Workaround

If you need to get the app running **immediately** to test:

1. **Comment out the language service** in App.xaml.cs (see Option 1 above)
2. **Run the app**
3. **If it works**, the issue is language service initialization
4. **If it still crashes**, the issue is elsewhere

---

## Next Steps

1. **Get the exact error message** from the debug output
2. **Tell me where it's crashing** (which file/line)
3. **I'll provide a specific fix**

Once I know the exact error, I can fix it in seconds!
