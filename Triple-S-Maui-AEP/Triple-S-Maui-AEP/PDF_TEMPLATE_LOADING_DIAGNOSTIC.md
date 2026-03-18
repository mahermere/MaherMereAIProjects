# PDF Template Loading Diagnostic

## Issue
The system was generating a new PDF instead of filling the existing template in `Resources\Raw\#1_e LONG_Digital (1).pdf`.

## Root Cause
The template loading method wasn't using the correct .NET MAUI API (`FileSystem.OpenAppPackageFileAsync`) to load files from the Resources\Raw folder.

## Solution Applied

### 1. Fixed LoadPdfTemplateAsync Method
Changed from manual file system paths to proper MAUI resource loading:

```csharp
// ✓ CORRECT - Uses MAUI's FileSystem API
var stream = await FileSystem.OpenAppPackageFileAsync("enrollment_template.pdf");

// ✗ WRONG - Tries to load from file system paths
var stream = File.OpenRead(Path.Combine(...));
```

### 2. Multiple Filename Attempts
The method now tries multiple filenames:
1. `enrollment_template.pdf` (preferred clean name)
2. `#1_e LONG_Digital (1).pdf` (original filename)
3. `1_e_LONG_Digital.pdf` (sanitized name)
4. Embedded resources (fallback)
5. AppData directory (runtime fallback)

### 3. Added Debug Logging
Comprehensive logging to track:
- ✓ When template is found
- ✗ When template is not found
- Which filename worked
- Fallback triggers
- Error details

## How to Verify Template is Loading

### Check Debug Output
Run the app and look for these messages in the Output window:

**SUCCESS:**
```
=== FillEnrollmentPdfAsync START ===
Attempting to load PDF template...
✓ Loaded template from: #1_e LONG_Digital (1).pdf
✓ Template loaded successfully, creating PdfLoadedDocument...
✓ PDF loaded: 13 pages
✓ Form found with 87 fields
✓ Signatures added
✓ PDF saved, size: 745823 bytes
=== FillEnrollmentPdfAsync SUCCESS ===
```

**FAILURE (fallback to generation):**
```
=== FillEnrollmentPdfAsync START ===
Attempting to load PDF template...
enrollment_template.pdf not found: ...
#1_e LONG_Digital (1).pdf not found: ...
✗ NO TEMPLATE FOUND - Will use fallback generation
⚠ Template not found - using fallback generation
```

## Testing Steps

### 1. Run the App
```
Debug > Start Debugging
```

### 2. Create an Enrollment
- Go to Dashboard
- Click "New Enrollment"
- Fill all 9 steps
- Submit

### 3. Check Debug Output
- Open **View > Output**
- Select "Debug" from dropdown
- Look for the messages above

### 4. Verify PDF
- Find the generated PDF in:
  ```
  {FileSystem.AppDataDirectory}/enrollments/{EnrollmentNumber}.pdf
  ```
- Open it in a PDF viewer
- If template worked: Will show official CMS form layout
- If fallback used: Will show plain generated format

## Troubleshooting

### If Template Still Not Loading

**1. Verify File Exists**
```powershell
Test-Path "C:\Triple-S\TSS_IT_TSH_AEP\Triple-S-Maui-AEP\Triple-S-Maui-AEP\Resources\Raw\#1_e LONG_Digital (1).pdf"
```
Should return: `True`

**2. Verify Build Action**
In Solution Explorer:
- Right-click the PDF file
- Properties
- Build Action should be: `MauiAsset`

**3. Clean and Rebuild**
```
Build > Clean Solution
Build > Rebuild Solution
```

**4. Check .csproj**
```xml
<MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
```

**5. Try Renaming File**
Special characters in filename might cause issues:
```powershell
Copy-Item "Resources\Raw\#1_e LONG_Digital (1).pdf" "Resources\Raw\enrollment_template.pdf"
```

Then rebuild.

### If Form Fields Not Filling

**1. Check if PDF has form fields:**
- Open PDF in Adobe Acrobat
- Tools > Prepare Form
- Should see form fields highlighted

**2. List field names:**
Add this temporary code to `FillFormFields`:
```csharp
foreach (PdfLoadedField field in form.Fields)
{
    Debug.WriteLine($"Field: '{field.Name}' Type: {field.GetType().Name}");
}
```

**3. Adjust field mappings:**
Update the `fieldMappings` dictionary in `FillFormFields` to match actual field names.

## Expected Behavior

### With Template (CORRECT)
✓ Loads official CMS form  
✓ Fills all form fields  
✓ Adds signatures at designated locations  
✓ Professional appearance  
✓ Includes all CMS branding/disclaimers  

### Without Template (FALLBACK)
⚠ Generates plain PDF  
⚠ Basic layout  
⚠ No form fields  
⚠ Custom format  

## File Locations Reference

| Item | Location |
|------|----------|
| Template Source | `Triple-S-Maui-AEP\Resources\Raw\#1_e LONG_Digital (1).pdf` |
| Generated PDFs | `{FileSystem.AppDataDirectory}/enrollments/` |
| Code | `Triple-S-Maui-AEP\Services\PdfService.cs` |
| Usage | `Triple-S-Maui-AEP\Views\EnrollmentWizardPage.xaml.cs` |

## Common Issues

### Issue: "Template not found"
**Cause**: File not in Resources\Raw or build action incorrect  
**Fix**: Verify file location and build action = MauiAsset

### Issue: "PDF loaded but no form found"
**Cause**: PDF template doesn't have fillable form fields  
**Fix**: Use Adobe Acrobat to add form fields, or accept generation fallback

### Issue: "Form fields not filling"
**Cause**: Field name mismatch  
**Fix**: Update field mappings in `FillFormFields` to match actual PDF field names

### Issue: "Signatures not appearing"
**Cause**: Signature coordinates don't match template layout  
**Fix**: Adjust coordinates in `AddSignaturesToPdfAsync` based on template

## Next Steps

1. **Test the fix**: Run the app and check debug output
2. **Verify PDF**: Open generated PDF to confirm it's using template
3. **Report results**: Note any errors in debug output
4. **Adjust if needed**: Update field mappings or coordinates as necessary

## Success Criteria

✅ Debug output shows "✓ Loaded template from..."  
✅ Debug output shows "✓ Form found with X fields"  
✅ Generated PDF uses official CMS form layout  
✅ All form fields are populated  
✅ Signatures appear in correct locations  
