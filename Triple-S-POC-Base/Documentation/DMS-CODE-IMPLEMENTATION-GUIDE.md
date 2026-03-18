# DMS Integration - Code Implementation Guide

## How to Integrate DMS Upload into EnrollmentWizardPage.xaml.cs

### Step 1: Import the DMS Service

At the top of `EnrollmentWizardPage.xaml.cs`, add:

```csharp
using TripleS.SOA.AEP.UI.Services;
```

### Step 2: Modify SubmitEnrollment() Method

Replace the TODO in `SubmitEnrollment()` with the DMS upload logic:

```csharp
private async void SubmitEnrollment()
{
    // ... existing PDF generation code ...
    
    try
    {
        // Your existing PDF generation logic here...
        var fieldData = new Dictionary<string, string> { /* ... */ };
        // ... PDF generation code ...
        
        // NEW: Upload to DMS
        if (System.IO.File.Exists(outputPath))
        {
            await UploadToDMSAsync(
                outputPath,
                enrollmentNumber,
                FirstNameBox.Text,
                LastNameBox.Text);
        }
        
        // ... existing success message ...
        DisplayAlert("Success", $"Enrollment saved and uploaded to DMS", "OK");
    }
    catch (Exception ex)
    {
        DisplayAlert("Error", $"Failed to process enrollment: {ex.Message}", "OK");
    }
}

private async Task UploadToDMSAsync(
    string filePath,
    string enrollmentNumber,
    string firstName,
    string lastName)
{
    try
    {
        System.Diagnostics.Debug.WriteLine("[Enrollment] Starting DMS upload...");
        
        var dmsService = new DMSService();
        
        // Gather form data
        var uploadResponse = await dmsService.UploadEnrollmentAsync(
            filePath: filePath,
            enrollmentNumber: enrollmentNumber,
            firstName: firstName,
            lastName: LastNameBox.Text,
            middleName: MiddleInitialBox.Text,
            dateOfBirth: DOBPicker.Date.ToString("MM/dd/yyyy"),
            gender: GenderCombo.SelectedItem?.ToString(),
            ssn: SSNBox.Text,
            medicare: MedicareBox.Text,
            medicaid: null, // Add if applicable
            primaryPhone: PrimaryPhoneBox.Text,
            secondaryPhone: SecondaryPhoneBox.Text,
            primaryPhoneIsMobile: PrimaryPhoneIsMobileCheckbox?.IsChecked ?? false,
            email: EmailBox.Text,
            address1: Address1Box.Text,
            address2: Address2Box.Text,
            city: CityBox.Text,
            state: StateBox.Text,
            zipCode: ZIPBox.Text,
            mailingAddress1: DifferentMailingCheckbox.IsChecked ? MailingAddress1Box.Text : null,
            mailingAddress2: DifferentMailingCheckbox.IsChecked ? MailingAddress2Box.Text : null,
            mailingCity: DifferentMailingCheckbox.IsChecked ? MailingCityBox.Text : null,
            mailingState: DifferentMailingCheckbox.IsChecked ? MailingStateBox.Text : null,
            mailingZip: DifferentMailingCheckbox.IsChecked ? MailingZIPBox.Text : null,
            emergencyContactName: EmergencyNameBox.Text,
            emergencyContactPhone: EmergencyPhoneBox.Text,
            emergencyContactRelationship: EmergencyRelationshipCombo.SelectedItem?.ToString(),
            planName: PlanNamePicker.SelectedItem?.ToString(),
            contractNumber: PlanContractBox.Text,
            planPBP: null, // Add if available
            language: LanguagePicker.SelectedIndex == 1 ? "Spanish" : "English",
            contactMethod: ContactMethodPicker.SelectedItem?.ToString()
        );
        
        if (uploadResponse.Success)
        {
            System.Diagnostics.Debug.WriteLine($"[Enrollment] Upload successful: {uploadResponse.DocumentId}");
            
            // Update enrollment record with Document ID
            var enrollmentRecord = new TripleS.SOA.AEP.UI.Services.EnrollmentService.EnrollmentRecord
            {
                EnrollmentNumber = enrollmentNumber,
                FirstName = firstName,
                LastName = lastName,
                DateCreated = DateTime.Now,
                FilePath = filePath,
                IsUploaded = true,
                DocumentId = uploadResponse.DocumentId // Capture DMS Document ID
            };
            
            // Save to service and CSV
            TripleS.SOA.AEP.UI.Services.EnrollmentService.AddEnrollmentRecord(enrollmentRecord);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[Enrollment] Upload failed: {uploadResponse.Message}");
            
            await MainThread.InvokeOnMainThread(async () =>
            {
                await DisplayAlert(
                    "Upload Warning",
                    $"PDF saved but DMS upload failed: {uploadResponse.Message}\n\nYou can retry from the Dashboard.",
                    "OK"
                );
            });
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[Enrollment] DMS Error: {ex.Message}");
        System.Diagnostics.Debug.WriteLine($"[Enrollment] Stack: {ex.StackTrace}");
        
        // Don't fail enrollment submission if DMS upload fails
        // Just log and warn the user
        await MainThread.InvokeOnMainThread(async () =>
        {
            await DisplayAlert(
                "Upload Error",
                $"Could not upload to DMS: {ex.Message}\n\nYou can try again from the Dashboard.",
                "OK"
            );
        });
    }
}
```

### Step 3: Update EnrollmentService Record Model

If not already present, add DocumentId property to `EnrollmentRecord` class in `Services/EnrollmentService.cs`:

```csharp
public class EnrollmentRecord
{
    public string EnrollmentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateCreated { get; set; }
    public string FilePath { get; set; }
    public bool IsUploaded { get; set; }
    public string DocumentId { get; set; }  // NEW: DMS Document ID
}
```

### Step 4: Update CSV Serialization

Update the CSV persistence to include DocumentId:

```csharp
// When saving to CSV
var csvLine = $"{record.EnrollmentNumber},{record.FirstName},{record.LastName}," +
              $"{record.DateCreated:yyyy-MM-dd HH:mm:ss},{record.FilePath}," +
              $"{record.IsUploaded},{record.DocumentId}";

// When loading from CSV
var docId = parts.Length > 6 ? parts[6] : string.Empty;
var record = new EnrollmentRecord
{
    EnrollmentNumber = parts[0],
    FirstName = parts[1],
    LastName = parts[2],
    DateCreated = DateTime.Parse(parts[3]),
    FilePath = parts[4],
    IsUploaded = bool.Parse(parts[5]),
    DocumentId = docId
};
```

### Step 5: Configure DMS Endpoint

In `Configuration/AppSettings.cs`, ensure endpoint is set:

```csharp
public static string DMSEndpoint { get; set; } = "https://localhost:44304/api/document/upload";
public static string DMSApiKey { get; set; } = null; // Add if required
```

For different environments:

```csharp
// Development
public static string DMSEndpoint { get; set; } = "https://localhost:44304/api/document/upload";

// QA
// public static string DMSEndpoint { get; set; } = "https://qa-dms.tripless.com/api/document/upload";

// Production
// public static string DMSEndpoint { get; set; } = "https://dms.tripless.com/api/document/upload";
```

---

## Integration Points Summary

### 1. **PDF Generation to DMS Upload Flow**
```
SubmitEnrollment()
    ↓
Generate PDF
    ↓
[Success]
    ↓
Call UploadToDMSAsync()
    ↓
[Success] → Capture DocumentId
[Failure] → Warn user but don't block
    ↓
Save enrollment record
    ↓
Navigate to Dashboard
```

### 2. **Error Handling Strategy**
- **PDF Generation Failure**: Block submission, show error
- **DMS Upload Failure**: Warn user but allow submission (can retry from Dashboard)
- **Network Issues**: Handled gracefully with timeout

### 3. **Data Flow**
```
Form Fields
    ↓
Validate & Collect
    ↓
Generate PDF
    ↓
Convert to Base64
    ↓
Create Keywords Array (32 fields)
    ↓
POST JSON to DMS
    ↓
Receive DocumentId
    ↓
Update Enrollment Record
    ↓
Save to CSV
```

---

## Testing the Integration

### Test Case 1: Successful Upload
```
1. Fill in enrollment form completely
2. Submit enrollment
3. Verify PDF is created
4. Check for success message
5. Verify DocumentId is captured in logs
6. Check enrollment record in Dashboard
```

### Test Case 2: Network Failure
```
1. Temporarily disable network or set wrong endpoint
2. Submit enrollment
3. Verify PDF is still created
4. Verify warning message is shown
5. Verify enrollment appears in Dashboard with IsUploaded = false
6. Verify retry button works from Dashboard
```

### Test Case 3: Missing Required Fields
```
1. Leave Medicare number blank
2. Submit enrollment
3. Verify PDF is created
4. Verify Medicare keyword is not included (filtered out)
5. Verify upload still succeeds
6. Verify DocumentId is captured
```

### Test Case 4: Retry from Dashboard
```
1. Have failed enrollment in Dashboard
2. Click upload button
3. Verify upload is retried
4. Check for success message
5. Verify checkmark appears
```

---

## Debug Logging

To troubleshoot, check Visual Studio Output window for:

```
[Enrollment] Starting DMS upload...
[DMSService] Starting upload for: C:\AppData\Enrollment_...
[DMSService] File size: 245632 bytes
[DMSService] Base64 length: 327509 characters
[DMSService] Total keywords: 32
[DMSService] Sending POST to: https://localhost:44304/api/document/upload
[DMSService] Response Status Code: 200
[DMSService] Response Content: {"success":true,"documentId":"DOC-2025-00001234"...}
[Enrollment] Upload successful: DOC-2025-00001234
```

---

## Common Issues & Solutions

### Issue: "Network error: No connection could be made"
**Solution**: Verify DMS endpoint URL in AppSettings, check network connectivity

### Issue: "Upload failed: 400 Bad Request"
**Solution**: Check keyword KeywordTypeIds, verify JSON serialization

### Issue: "DocumentId is null"
**Solution**: Ensure DMS is returning proper response format, check logs

### Issue: "File not found"
**Solution**: Verify PDF generation completed successfully before upload attempt

---

## Performance Considerations

- **Upload Timeout**: 5 minutes (configurable in AppSettings)
- **Base64 Encoding**: Only for PDF content, minimal overhead
- **Network**: Asynchronous, doesn't block UI
- **Retry**: Implement exponential backoff for production

---

## Production Checklist

- [ ] DMS endpoint configured correctly
- [ ] API key set (if required)
- [ ] Timeout values appropriate for production
- [ ] Error handling tested
- [ ] Retry logic implemented
- [ ] DocumentId properly captured
- [ ] CSV persistence working
- [ ] Dashboard upload retry working
- [ ] Logging configured for monitoring
- [ ] HTTPS endpoint verified

---

**Implementation Guide Version**: 1.0
**Date**: January 15, 2025
**Status**: Ready for Implementation
