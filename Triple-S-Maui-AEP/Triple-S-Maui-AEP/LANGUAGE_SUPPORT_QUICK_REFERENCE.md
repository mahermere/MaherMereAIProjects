# Quick Language Support Reference - Developer Guide

## TL;DR - Quick Start

### In Any ViewModel:
```csharp
public class MyViewModel : BaseViewModel
{
    public string Title => IsEnglish ? "Title" : "Título";
    
    protected override void OnLanguageChanged(Language newLanguage)
    {
        base.OnLanguageChanged(newLanguage);
        OnPropertyChanged(nameof(Title));
    }
}
```

### In Any View (XAML.cs):
```csharp
private void SetLocalizedText()
{
    var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
    MyLabel.Text = isEnglish ? "Text" : "Texto";
}

public MyPage()
{
    InitializeComponent();
    SetLocalizedText();
    Services.LanguageService.Instance.LanguageChanged += _ => SetLocalizedText();
}
```

---

## Quick Reference Table

| Task | Code | Location |
|------|------|----------|
| Check if English | `IsEnglish` | ViewModel (from BaseViewModel) |
| Get current language | `CurrentLanguage` | ViewModel (from BaseViewModel) |
| Change language | `LanguageService.Instance.CurrentLanguage = Language.Spanish;` | Anywhere |
| Get language code | `LanguageService.Instance.GetLanguageCode()` | Anywhere |
| Handle language change | `protected override void OnLanguageChanged(Language newLanguage)` | ViewModel |
| Get localized text | `GetLocalizedText("en", "es")` | ViewModel |
| Update UI text | `private void SetLocalizedText()` | View |
| Subscribe to changes | `LanguageService.Instance.LanguageChanged += ...` | View |

---

## Available Languages

```csharp
Language.English  // en-US
Language.Spanish  // es-PR (Puerto Rico)
```

---

## Implementation Checklist

### New ViewModel:
- [ ] Inherit from `BaseViewModel`
- [ ] Use `IsEnglish` for conditions
- [ ] Override `OnLanguageChanged()` for updates
- [ ] Notify properties changed

### New View:
- [ ] Implement `SetLocalizedText()`
- [ ] Call in constructor
- [ ] Subscribe to `LanguageChanged`
- [ ] Update all text elements

---

## Current Implementation Status

### ✅ ViewModels Ready:
- AgentLoginViewModel
- DashboardViewModel
- SOAWizardViewModel
- EnrollmentWizardViewModel

### ✅ Views Complete:
- AgentLoginPage
- DashboardPage
- SOAWizardPage
- EnrollmentWizardPage
- MainPage

### ✅ Services:
- LanguageService (Singleton)
- BaseViewModel (Enhanced)

---

## Common Patterns

### Pattern 1: Computed Property
```csharp
public string ButtonText => IsEnglish ? "Click Me" : "Haga Clic";
```

### Pattern 2: Language-Aware Method
```csharp
public string GetTitle()
{
    return GetLocalizedText("Welcome", "Bienvenido");
}
```

### Pattern 3: Conditional Update
```csharp
protected override void OnLanguageChanged(Language newLanguage)
{
    base.OnLanguageChanged(newLanguage);
    MyProperty = IsEnglish ? "English" : "Español";
}
```

### Pattern 4: View Update
```csharp
private void SetLocalizedText()
{
    var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
    MyLabel.Text = isEnglish ? "Text" : "Texto";
}
```

---

## Debugging

### Enable Debug Output:
```csharp
System.Diagnostics.Debug.WriteLine($"Language changed to: {LanguageService.Instance.GetLanguageCode()}");
```

### Check Current Language:
```csharp
Console.WriteLine(LanguageService.Instance.CurrentLanguage); // Language.English or Language.Spanish
```

### Verify Subscription:
```csharp
// This should fire when language changes
Services.LanguageService.Instance.LanguageChanged += (lang) => 
{
    System.Diagnostics.Debug.WriteLine($"✓ Language changed: {lang}");
};
```

---

## Do's and Don'ts

### ✅ DO:
- Use `IsEnglish` property
- Override `OnLanguageChanged()` in ViewModels
- Call `SetLocalizedText()` in View constructors
- Subscribe to `LanguageChanged` event
- Update properties in `OnPropertyChanged()`

### ❌ DON'T:
- Hardcode English-only strings
- Forget to update UI on language change
- Create new LanguageService instances
- Mix language codes ("en" vs "en-US")

---

## All Files Modified/Created

### Modified:
- `ViewModels/BaseViewModel.cs` - Added language support

### Created:
- `LANGUAGE_SUPPORT_GUIDE.md` - Complete implementation guide
- `LANGUAGE_SUPPORT_STATUS.md` - Current status report
- This file - Quick reference

### No Changes Needed:
- All Views - Already have SetLocalizedText()
- All ViewModels - Already inherit from BaseViewModel
- LanguageService - Already implemented
- Language enum - Already defined

---

## Support

For questions or issues:
1. Check `LANGUAGE_SUPPORT_GUIDE.md` for detailed examples
2. Check `LANGUAGE_SUPPORT_STATUS.md` for current status
3. Review existing implementations in AgentLoginPage or DashboardPage
4. All ViewModels inherit from BaseViewModel automatically

---

## Example: Adding Language to New Page

```csharp
// 1. Create ViewModel (if needed)
public class MyViewModel : BaseViewModel
{
    private string _title = string.Empty;
    
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    public MyViewModel()
    {
        UpdateText();
    }
    
    protected override void OnLanguageChanged(Language newLanguage)
    {
        base.OnLanguageChanged(newLanguage);
        UpdateText();
    }
    
    private void UpdateText()
    {
        Title = GetLocalizedText("My Title", "Mi Título");
    }
}

// 2. Create View
public partial class MyPage : ContentPage
{
    private readonly MyViewModel _viewModel = new();
    
    public MyPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        
        SetLocalizedText();
        Services.LanguageService.Instance.LanguageChanged += _ => SetLocalizedText();
    }
    
    private void SetLocalizedText()
    {
        var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
        MyLabel.Text = isEnglish ? "Hello" : "Hola";
        MyButton.Text = isEnglish ? "Click" : "Haga Clic";
    }
}

// 3. That's it! Language support is automatic.
```

---

## Build Status
✅ All changes compile successfully with no warnings or errors!
