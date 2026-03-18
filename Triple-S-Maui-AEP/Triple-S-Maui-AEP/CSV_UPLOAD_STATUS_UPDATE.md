# CSV Upload Status Update Implementation

## Overview
Added functionality to update CSV files with upload status when SOA and Enrollment documents are successfully uploaded to DMS.

## Problem
When clicking the upload button and successfully uploading to DMS, the `IsUploaded` flag in the in-memory records was being updated, but the CSV files were not being persisted. This meant that after restarting the app, the upload status would be lost.

## Solution

### 1. Added `UpdateUploadStatus` Method to Services

#### SOAService.cs
```csharp
public static void UpdateUploadStatus(string soaNumber, bool isUploaded)
{
    EnsureLoaded();

    var record = _activeSOARecords.FirstOrDefault(r => r.SOANumber == soaNumber);
    if (record != null)
    {
        record.IsUploaded = isUploaded;
        SaveToCsv();
        System.Diagnostics.Debug.WriteLine($"Updated SOA CSV: {soaNumber}, IsUploaded={isUploaded}");
    }
}
```

#### EnrollmentService.cs
```csharp
public static void UpdateUploadStatus(string enrollmentNumber, bool isUploaded)
{
    EnsureLoaded();

    var record = _activeEnrollmentRecords.FirstOrDefault(r => r.EnrollmentNumber == enrollmentNumber);
    if (record != null)
    {
        record.IsUploaded = isUploaded;
        SaveToCsv();
        System.Diagnostics.Debug.WriteLine($"Updated enrollment CSV: {enrollmentNumber}, IsUploaded={isUploaded}");
    }
}
```

### 2. Updated DashboardViewModel Upload Methods

#### UploadSOAAsync
**Before:**
```csharp
if (response.Success)
{
    soaRecord.IsUploaded = true;
    if (soaItem != null)
    {
        soaItem.IsUploaded = true;
    }
    // ... statistics update
}
```

**After:**
```csharp
if (response.Success)
{
    soaRecord.IsUploaded = true;
    if (soaItem != null)
    {
        soaItem.IsUploaded = true;
    }

    // ✓ NEW: Update CSV file with upload status
    SOAService.UpdateUploadStatus(soaNumber, true);

    // ... statistics update
}
```

#### UploadEnrollmentAsync
**Before:**
```csharp
if (response.Success)
{
    enrollmentRecord.IsUploaded = true;
    if (enrollmentItem != null)
    {
        enrollmentItem.IsUploaded = true;
    }
    // ... debug logging
}
```

**After:**
```csharp
if (response.Success)
{
    enrollmentRecord.IsUploaded = true;
    if (enrollmentItem != null)
    {
        enrollmentItem.IsUploaded = true;
    }

    // ✓ NEW: Update CSV file with upload status
    EnrollmentService.UpdateUploadStatus(enrollmentNumber, true);
    
    // ... debug logging
}
```

## CSV File Format

### SOA CSV (`soa.csv`)
```csv
SOANumber,FirstName,LastName,DateCreated,FilePath,IsUploaded
SOA-2024-001,John,Doe,2024-01-15T10:30:00,/data/.../soas/SOA-2024-001.pdf,false
SOA-2024-002,Jane,Smith,2024-01-16T14:20:00,/data/.../soas/SOA-2024-002.pdf,true
```

### Enrollment CSV (`enrollments.csv`)
```csv
EnrollmentNumber,FirstName,LastName,DateCreated,FilePath,IsUploaded
ENR-2024-001,John,Doe,2024-01-15T10:30:00,/data/.../enrollments/ENR-2024-001.pdf,false
ENR-2024-002,Jane,Smith,2024-01-16T14:20:00,/data/.../enrollments/ENR-2024-002.pdf,true
```

## How It Works

### Upload Flow
```
1. User clicks Upload button (↑)
   ↓
2. DashboardPage.OnUploadSOAClicked()
   ↓
3. DashboardViewModel.UploadSOAAsync(soaNumber)
   ↓
4. DMSService.UploadDocumentAsync()
   ↓
5. If response.Success:
   a. Update in-memory record: soaRecord.IsUploaded = true
   b. Update UI record: soaItem.IsUploaded = true
   c. ✓ NEW: Persist to CSV: SOAService.UpdateUploadStatus(soaNumber, true)
   d. Update statistics
```

### CSV Update Process
```
SOAService.UpdateUploadStatus(soaNumber, true)
   ↓
1. EnsureLoaded() - Load records from CSV if not already loaded
   ↓
2. Find record by SOANumber
   ↓
3. Update record.IsUploaded = true
   ↓
4. SaveToCsv() - Write all records back to CSV file
   ↓
5. Debug log confirmation
```

## Benefits

### ✅ Persistence
- Upload status survives app restarts
- CSV files are the source of truth

### ✅ Reliability
- Changes are written immediately after successful upload
- No data loss if app crashes

### ✅ Consistency
- In-memory records and CSV files stay in sync
- Dashboard UI reflects actual state

### ✅ Auditability
- CSV files can be inspected directly
- Debug logs confirm each update

## Testing

### Manual Test Steps

1. **Create SOA/Enrollment**
   - Go through wizard
   - Submit and return to dashboard

2. **Verify Initial State**
   - Check dashboard shows "Pending" status
   - Upload button (↑) is enabled and blue
   - Open CSV file: `IsUploaded=false`

3. **Upload Document**
   - Click upload button (↑)
   - Wait for upload completion

4. **Verify Updated State**
   - Status changes to "Uploaded"
   - Button changes to (✓) green and disabled
   - Check Debug Output: "Updated SOA CSV: SOA-2024-001, IsUploaded=true"
   - Open CSV file: `IsUploaded=true`

5. **Restart App**
   - Close and reopen app
   - Navigate to dashboard
   - Verify status still shows "Uploaded"
   - Verify button still shows (✓) and disabled

### Automated Test Scenarios
```csharp
[Test]
public void UpdateUploadStatus_UpdatesRecord()
{
    // Arrange
    var record = new SOARecord { SOANumber = "TEST-001", IsUploaded = false };
    SOAService.AddSOARecord(record);

    // Act
    SOAService.UpdateUploadStatus("TEST-001", true);

    // Assert
    var updated = SOAService.ActiveSOARecords.First(r => r.SOANumber == "TEST-001");
    Assert.IsTrue(updated.IsUploaded);
}

[Test]
public void UpdateUploadStatus_PersistsToCsv()
{
    // Arrange
    SOAService.UpdateUploadStatus("TEST-001", true);

    // Act
    SOAService.Reload(); // Force reload from CSV

    // Assert
    var reloaded = SOAService.ActiveSOARecords.First(r => r.SOANumber == "TEST-001");
    Assert.IsTrue(reloaded.IsUploaded);
}
```

## Debug Output

When upload succeeds, you should see:
```
Uploading SOA: SOA-2024-001
SOA uploaded successfully: SOA-2024-001
Updated SOA CSV: SOA-2024-001, IsUploaded=true
```

Or for enrollments:
```
Uploading Enrollment: ENR-2024-001
Enrollment uploaded successfully: ENR-2024-001
Updated enrollment CSV: ENR-2024-001, IsUploaded=true
```

## CSV File Locations

### SOA CSV
```
{FileSystem.AppDataDirectory}/data/soa.csv
```
Example: `/data/user/0/com.triples.aep/files/data/soa.csv`

### Enrollment CSV
```
{FileSystem.AppDataDirectory}/data/enrollments.csv
```
Example: `/data/user/0/com.triples.aep/files/data/enrollments.csv`

## Error Handling

The implementation includes graceful error handling:

```csharp
public static void UpdateUploadStatus(string soaNumber, bool isUploaded)
{
    EnsureLoaded(); // Ensures CSV is loaded before updating

    var record = _activeSOARecords.FirstOrDefault(r => r.SOANumber == soaNumber);
    if (record != null) // Only updates if record exists
    {
        record.IsUploaded = isUploaded;
        SaveToCsv(); // Handles directory creation and file writing
        // ... debug log
    }
    // If record not found, silently fails (no exception)
}
```

## CSV Escaping

The implementation properly escapes CSV values:
```csharp
private static string Escape(string? value)
{
    var safe = value ?? string.Empty;
    if (safe.Contains(',') || safe.Contains('"') || safe.Contains('\n') || safe.Contains('\r'))
    {
        return $"\"{safe.Replace("\"", "\"\"")}\"";
    }
    return safe;
}
```

**Examples**:
- `John,Doe` → `"John,Doe"`
- `O'Brien` → `O'Brien` (no escaping needed)
- `Company "ABC"` → `"Company ""ABC"""`

## Concurrency

Since the services are static and operations are synchronous:
- ✅ Single upload at a time (enforced by UI button disable)
- ✅ SaveToCsv() is atomic (File.WriteAllText)
- ⚠️ Multiple simultaneous uploads could cause race conditions
  - Current implementation: UI prevents this
  - Future enhancement: Add locking if needed

## Build Status
✅ **Build Successful**

## Files Modified

1. `Triple-S-Maui-AEP\Services\SOAService.cs`
   - Added `UpdateUploadStatus` method

2. `Triple-S-Maui-AEP\Services\EnrollmentService.cs`
   - Added `UpdateUploadStatus` method

3. `Triple-S-Maui-AEP\ViewModels\DashboardViewModel.cs`
   - Updated `UploadSOAAsync` to call `SOAService.UpdateUploadStatus`
   - Updated `UploadEnrollmentAsync` to call `EnrollmentService.UpdateUploadStatus`

## Summary

The upload status is now properly persisted to CSV files after successful uploads. When users upload a document:

1. ✅ DMS upload succeeds
2. ✅ In-memory record updated
3. ✅ UI record updated
4. ✅ **CSV file updated** ← NEW!
5. ✅ Statistics refreshed

This ensures that upload status survives app restarts and provides a reliable audit trail of which documents have been uploaded to the DMS system.
