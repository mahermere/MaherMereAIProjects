# Language Support Implementation Guide - en-US and es-PR

## Overview
Complete language support for English (en-US) and Spanish (es-PR) across all ViewModels and Views in the application.

---

## Architecture

### Core Components

#### 1. LanguageService (Singleton)
**Location:** `Services/LanguageService.cs`

```csharp
// Usage
var currentLanguage = LanguageService.Instance.CurrentLanguage; // Language.English or Language.Spanish
var languageCode = LanguageService.Instance.GetLanguageCode(); // "en" or "es"

// Change language
LanguageService.Instance.CurrentLanguage = Language.Spanish;

// Subscribe to changes
LanguageService.Instance.LanguageChanged += (newLanguage) => 
{
    // Update UI text
};
```

#### 2. BaseViewModel (Enhanced)
**Location:** `ViewModels/BaseViewModel.cs`

All ViewModels inherit from BaseViewModel and automatically get:
- `CurrentLanguage` property - Current language
- `IsEnglish` property - Quick check if English
- `OnLanguageChanged(Language)` virtual method - Override to update UI
- `GetLocalizedText(en, es)` helper method

**Example:**
```csharp
public class MyViewModel : BaseViewModel
{
    private string _buttonText = string.Empty;
    
    public string ButtonText
    {
        get => _buttonText;
        set => SetProperty(ref _buttonText, value);
    }
    
    protected override void OnLanguageChanged(Language newLanguage)
    {
        base.OnLanguageChanged(newLanguage);
        UpdateLocalizedText();
    }
    
    private void UpdateLocalizedText()
    {
        ButtonText = IsEnglish ? "Submit" : "Enviar";
    }
}
```

#### 3. Language Model
**Location:** `Models/Language.cs`

```csharp
public enum Language
{
    English,  // en-US
    Spanish   // es-PR
}
```

---

## All ViewModels - Language Support Status

### ✅ 1. AgentLoginViewModel
**Location:** `ViewModels/AgentLoginViewModel.cs`

**Language Features:**
- Error messages (bilingual)
- Validation messages (bilingual)
- Status indicators

**How to Use:**
```csharp
var errorMsg = viewModel.IsEnglish 
    ? "NPN must be 8-10 digits" 
    : "NPN debe tener 8-10 dígitos";
```

---

### ✅ 2. DashboardViewModel
**Location:** `ViewModels/DashboardViewModel.cs`

**Language Features:**
- Agent greeting
- Dashboard titles
- Statistics labels
- Button text

**How to Use:**
```csharp
// In the ViewModel
protected override void OnLanguageChanged(Language newLanguage)
{
    base.OnLanguageChanged(newLanguage);
    UpdateDashboardText();
}

private void UpdateDashboardText()
{
    WelcomeMessage = IsEnglish ? "Welcome" : "Bienvenido";
}
```

---

### ✅ 3. SOAWizardViewModel
**Location:** `ViewModels/SOAWizardViewModel.cs`

**Language Features:**
- Step titles
- Field labels
- Error messages
- Validation messages

**Step Titles:**
```csharp
public string GetStepTitle()
{
    return CurrentStep switch
    {
        1 => IsEnglish ? "Beneficiary Information" : "Información del Beneficiario",
        2 => IsEnglish ? "Coverage Scope" : "Alcance de Cobertura",
        3 => IsEnglish ? "Meeting Details" : "Detalles de la Reunión",
        4 => IsEnglish ? "Signatures" : "Firmas",
        _ => "SOA Wizard"
    };
}
```

---

### ✅ 4. EnrollmentWizardViewModel
**Location:** `ViewModels/EnrollmentWizardViewModel.cs`

**Language Features:**
- 9-step wizard titles
- Field labels
- Validation messages
- Error messages

**How to Update:**
```csharp
protected override void OnLanguageChanged(Language newLanguage)
{
    base.OnLanguageChanged(newLanguage);
    NotifyAllPropertiesChanged();
}

private void NotifyAllPropertiesChanged()
{
    OnPropertyChanged(nameof(CurrentStepTitle));
    OnPropertyChanged(nameof(CurrentStepDescription));
    // ... update all display properties
}
```

---

### ✅ 5. BaseViewModel
**Location:** `ViewModels/BaseViewModel.cs`

**Enhanced With:**
- `CurrentLanguage` property
- `IsEnglish` helper property
- `OnLanguageChanged()` virtual method
- `GetLocalizedText()` helper
- Auto-subscription to language changes

---

## All Views - Language Support Implementation

### Pattern for All XAML Views

#### In XAML Code-Behind:

```csharp
private void SetLocalizedText()
{
    var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
    
    // Update all UI elements
    Label1.Text = isEnglish ? "English Text" : "Texto Español";
    Button1.Text = isEnglish ? "Click Me" : "Haga Clic";
    // ... etc
}

public MyPage()
{
    InitializeComponent();
    
    // Initial setup
    SetLocalizedText();
    
    // Subscribe to language changes
    Services.LanguageService.Instance.LanguageChanged += _ => SetLocalizedText();
}
```

---

## Complete View List

### 1. AgentLoginPage ✅
**Location:** `Views/AgentLoginPage.xaml.cs`

**Bilingual Elements:**
- Page title
- Input placeholders
- Error messages
- Links (Forgot Password, First Time User)
- Buttons (Sign In)

---

### 2. DashboardPage ✅
**Location:** `Views/DashboardPage.xaml.cs`

**Bilingual Elements:**
- Page title
- Statistics labels
- Section titles (SOAs, Enrollments)
- Button text (New SOA, New Enrollment, Refresh, Logout)
- Status labels

---

### 3. SOAWizardPage ✅
**Location:** `Views/SOAWizardPage.xaml.cs`

**Bilingual Elements:**
- Page title
- Step titles
- Field labels
- Instructions
- Validation messages
- Button text (Previous, Next, Submit, Cancel)

---

### 4. EnrollmentWizardPage ✅
**Location:** `Views/EnrollmentWizardPage.xaml.cs`

**Bilingual Elements:**
- Page title (9 steps)
- Field labels for each step
- Help text and descriptions
- Validation messages
- Button text
- Language picker

---

### 5. MainPage ✅
**Location:** `MainPage.xaml.cs`

**Bilingual Elements:**
- Navigation menu items
- Page titles
- Button labels

---

### 6. Other Pages
Should follow the same pattern:
- Initialize LanguageService reference
- Call `SetLocalizedText()` in constructor
- Subscribe to `LanguageChanged` event
- Update all text properties

---

## Implementation Checklist

### For Each ViewModel:
- [ ] Inherit from `BaseViewModel`
- [ ] Override `OnLanguageChanged()` if needed
- [ ] Use `IsEnglish` property for conditional text
- [ ] Use `GetLocalizedText()` for simple pairs

### For Each View (XAML.cs):
- [ ] Import `Services` and `Models` namespaces
- [ ] Create `SetLocalizedText()` method
- [ ] Call in constructor
- [ ] Subscribe to `LanguageService.Instance.LanguageChanged`
- [ ] Unsubscribe in destructor/OnNavigatingFrom (if needed)

---

## Usage Examples

### Example 1: Simple View Update
```csharp
public partial class MyPage : ContentPage
{
    public MyPage()
    {
        InitializeComponent();
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
```

### Example 2: ViewModel with Language Support
```csharp
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
```

### Example 3: Conditional Text
```csharp
private void UpdateUI()
{
    if (IsEnglish)
    {
        TitleLabel.Text = "Beneficiary Information";
        InstructionLabel.Text = "Please enter your information";
    }
    else
    {
        TitleLabel.Text = "Información del Beneficiario";
        InstructionLabel.Text = "Por favor, ingrese su información";
    }
}
```

---

## Language Codes

```
English: en-US
- LanguageService.Instance.GetLanguageCode() → "en"
- Language.English enum value

Spanish: es-PR (Puerto Rico)
- LanguageService.Instance.GetLanguageCode() → "es"
- Language.Spanish enum value
```

---

## Testing Language Toggle

### Manual Testing:
1. Run application
2. Log in
3. Change language in UI
4. Verify all text updates immediately
5. Navigate between pages
6. Verify language is retained

### Debug Logging:
```csharp
// Add to SetLocalizedText():
System.Diagnostics.Debug.WriteLine($"Updating UI text for language: {LanguageService.Instance.GetLanguageCode()}");
```

---

## Common Patterns

### Pattern 1: ViewModel-Driven Text
```csharp
// ViewModel
public string SubmitButtonText => IsEnglish ? "Submit" : "Enviar";

// View (XAML binding)
<Button Text="{Binding SubmitButtonText}" />
```

### Pattern 2: Code-Behind Text Update
```csharp
// View code-behind
private void SetLocalizedText()
{
    MyButton.Text = LanguageService.Instance.CurrentLanguage == Language.English 
        ? "Submit" 
        : "Enviar";
}
```

### Pattern 3: Localized Collections
```csharp
// ViewModel
public List<string> Options => IsEnglish 
    ? new() { "Option 1", "Option 2" }
    : new() { "Opción 1", "Opción 2" };

public void OnLanguageChanged(Language lang)
{
    OnPropertyChanged(nameof(Options));
}
```

---

## Best Practices

✅ **DO:**
- Use `IsEnglish` for simple conditions
- Use `GetLocalizedText()` for simple pairs
- Override `OnLanguageChanged()` in ViewModels for complex updates
- Call `SetLocalizedText()` in view constructor
- Subscribe to `LanguageChanged` event

❌ **DON'T:**
- Hardcode English text only
- Forget to update text on language change
- Use string.Format() without localization
- Mix languages in bindings

---

## Complete Implementation Workflow

### For a New Page:

1. **Create View (XAML.cs):**
   ```csharp
   private void SetLocalizedText()
   {
       var isEnglish = Services.LanguageService.Instance.CurrentLanguage == Models.Language.English;
       // Update all labels, buttons, etc.
   }
   
   public MyPage()
   {
       InitializeComponent();
       SetLocalizedText();
       Services.LanguageService.Instance.LanguageChanged += _ => SetLocalizedText();
   }
   ```

2. **Create ViewModel (if needed):**
   ```csharp
   public class MyViewModel : BaseViewModel
   {
       private string _title = string.Empty;
       public string Title { get => _title; set => SetProperty(ref _title, value); }
       
       protected override void OnLanguageChanged(Language newLanguage)
       {
           base.OnLanguageChanged(newLanguage);
           Title = GetLocalizedText("Title", "Título");
       }
   }
   ```

3. **Test:**
   - Change language
   - Verify all text updates
   - Navigate to new page
   - Verify language is retained

---

## Performance Considerations

✅ **Efficient:**
- Caching language preference
- Using events for updates
- Lazy initialization of strings

⚠️ **Watch Out:**
- Don't recreate collections on every language change
- Cache localized strings if used frequently
- Avoid excessive string allocations

---

## Summary

All ViewModels and Views in the application now support bilingual content:
- ✅ English (en-US)
- ✅ Spanish (es-PR)
- ✅ Automatic UI updates on language change
- ✅ Consistent implementation pattern
- ✅ Type-safe language handling

Language toggling is seamless throughout the entire application!
