# PDF Splitter Service - Usage Guide

## Overview
The `PdfSplitterService` uses Syncfusion.Pdf.Net.Core to split, merge, and process Medicare Enrollment Form PDFs.

---

## Features

1. **Split PDF into Individual Pages**
2. **Extract Specific Pages**
3. **Extract Page Ranges**
4. **Split Medicare Forms by Section**
5. **Merge Multiple PDFs**
6. **Get PDF Information**

---

## Usage Examples

### 1. Split PDF into Individual Pages

```csharp
var splitterService = new PdfSplitterService();

// Split entire PDF into individual page files
var pdfPath = "/path/to/enrollment_form.pdf";
var outputPaths = await splitterService.SplitPdfIntoIndividualPagesAsync(
    inputPdfPath: pdfPath,
    outputDirectory: Path.Combine(FileSystem.AppDataDirectory, "split_pages"),
    fileNamePrefix: "enrollment_page"
);

// Result: enrollment_page_001.pdf, enrollment_page_002.pdf, etc.
foreach (var path in outputPaths)
{
    Debug.WriteLine($"Created: {path}");
}
```

### 2. Extract Specific Pages

```csharp
var splitterService = new PdfSplitterService();

// Extract pages 1, 3, and 5 from enrollment form
var extractedPdf = await splitterService.ExtractPagesAsync(
    inputPdfPath: "/path/to/enrollment_form.pdf",
    pageNumbers: new[] { 1, 3, 5 },
    outputPdfPath: "/path/to/extracted_pages.pdf"
);

Debug.WriteLine($"Extracted PDF: {extractedPdf}");
```

### 3. Extract Page Range

```csharp
var splitterService = new PdfSplitterService();

// Extract pages 1-4 (Part A of form)
var partA = await splitterService.ExtractPageRangeAsync(
    inputPdfPath: "/path/to/enrollment_form.pdf",
    startPage: 1,
    endPage: 4,
    outputPdfPath: "/path/to/part_a.pdf"
);

// Extract pages 5-8 (Part B of form)
var partB = await splitterService.ExtractPageRangeAsync(
    inputPdfPath: "/path/to/enrollment_form.pdf",
    startPage: 5,
    endPage: 8,
    outputPdfPath: "/path/to/part_b.pdf"
);
```

### 4. Split Medicare Enrollment Form by Sections

```csharp
var splitterService = new PdfSplitterService();

// Automatically split form into predefined sections
var sections = await splitterService.SplitMedicareEnrollmentFormAsync(
    inputPdfPath: "/path/to/complete_enrollment.pdf",
    outputDirectory: Path.Combine(FileSystem.AppDataDirectory, "enrollment_sections")
);

// Results:
// - Part1_PersonalInfo.pdf (pages 1-2)
// - Part2_PlanSelection.pdf (pages 3-4)
// - Part3_CurrentCoverage.pdf (pages 5-6)
// - Part4_Authorization.pdf (pages 7-8)

foreach (var section in sections)
{
    Debug.WriteLine($"{section.Key}: {section.Value}");
}
```

**Note:** Adjust section page ranges in the code based on your actual Medicare form structure.

### 5. Merge Multiple PDFs

```csharp
var splitterService = new PdfSplitterService();

var pdfFiles = new List<string>
{
    "/path/to/part1.pdf",
    "/path/to/part2.pdf",
    "/path/to/part3.pdf",
    "/path/to/signatures.pdf"
};

var mergedPdf = await splitterService.MergePdfsAsync(
    inputPdfPaths: pdfFiles,
    outputPdfPath: "/path/to/complete_enrollment.pdf"
);

Debug.WriteLine($"Merged PDF: {mergedPdf}");
```

### 6. Get PDF Information

```csharp
var splitterService = new PdfSplitterService();

var info = await splitterService.GetPdfInfoAsync("/path/to/enrollment.pdf");

Debug.WriteLine($"File: {info.FileName}");
Debug.WriteLine($"Pages: {info.PageCount}");
Debug.WriteLine($"Size: {info.FileSizeFormatted}");
Debug.WriteLine($"Created: {info.CreationDate}");
Debug.WriteLine($"Title: {info.Title}");
```

---

## Practical Scenarios

### Scenario 1: Process Uploaded Medicare Form

```csharp
public async Task ProcessUploadedEnrollmentFormAsync(string uploadedPdfPath)
{
    var splitterService = new PdfSplitterService();
    
    // Get PDF info first
    var info = await splitterService.GetPdfInfoAsync(uploadedPdfPath);
    Debug.WriteLine($"Processing {info.FileName} with {info.PageCount} pages");
    
    // Split into sections
    var sections = await splitterService.SplitMedicareEnrollmentFormAsync(uploadedPdfPath);
    
    // Process each section separately
    foreach (var section in sections)
    {
        Debug.WriteLine($"Section {section.Key} saved to: {section.Value}");
        
        // You could now:
        // - Upload each section separately to DMS
        // - Process form data from specific sections
        // - Store sections in encrypted database
    }
}
```

### Scenario 2: Create Custom Enrollment Package

```csharp
public async Task CreateEnrollmentPackageAsync(string enrollmentNumber)
{
    var splitterService = new PdfSplitterService();
    
    var pdfsToMerge = new List<string>
    {
        $"/path/to/enrollment_{enrollmentNumber}.pdf",
        $"/path/to/soa_{enrollmentNumber}.pdf",
        "/path/to/terms_and_conditions.pdf",
        "/path/to/privacy_notice.pdf"
    };
    
    var packagePath = await splitterService.MergePdfsAsync(
        inputPdfPaths: pdfsToMerge,
        outputPdfPath: $"/path/to/complete_package_{enrollmentNumber}.pdf"
    );
    
    Debug.WriteLine($"Complete enrollment package: {packagePath}");
    
    // Upload to DMS
    await UploadToDMSAsync(packagePath, enrollmentNumber);
}
```

### Scenario 3: Extract Signature Pages Only

```csharp
public async Task ExtractSignaturePagesAsync(string enrollmentPdfPath)
{
    var splitterService = new PdfSplitterService();
    
    // Assuming signature pages are the last 2 pages
    var pageCount = await splitterService.GetPageCountAsync(enrollmentPdfPath);
    
    var signaturePages = await splitterService.ExtractPageRangeAsync(
        inputPdfPath: enrollmentPdfPath,
        startPage: pageCount - 1,
        endPage: pageCount,
        outputPdfPath: "/path/to/signatures_only.pdf"
    );
    
    Debug.WriteLine($"Signature pages extracted: {signaturePages}");
}
```

### Scenario 4: Split and Upload Each Page

```csharp
public async Task SplitAndUploadEachPageAsync(string enrollmentPdfPath, string enrollmentNumber)
{
    var splitterService = new PdfSplitterService();
    
    // Split into individual pages
    var pages = await splitterService.SplitPdfIntoIndividualPagesAsync(
        inputPdfPath: enrollmentPdfPath,
        fileNamePrefix: $"enrollment_{enrollmentNumber}_page"
    );
    
    // Upload each page separately
    for (int i = 0; i < pages.Count; i++)
    {
        var pagePath = pages[i];
        Debug.WriteLine($"Uploading page {i + 1} of {pages.Count}...");
        
        // Upload to DMS with page number in metadata
        await UploadPageToDMSAsync(pagePath, enrollmentNumber, i + 1);
    }
}
```

---

## Integration with Existing Services

### With EnrollmentService

```csharp
public async Task ProcessAndStoreEnrollmentAsync(
    string uploadedPdfPath, 
    string firstName, 
    string lastName)
{
    var splitterService = new PdfSplitterService();
    var enrollmentNumber = new SOANumberService().GenerateEnrollmentNumber();
    
    // Split the form
    var sections = await splitterService.SplitMedicareEnrollmentFormAsync(
        uploadedPdfPath,
        Path.Combine(FileSystem.AppDataDirectory, "enrollments", enrollmentNumber)
    );
    
    // Store in encrypted database
    await EnrollmentService.AddEnrollmentRecordAsync(new EnrollmentService.EnrollmentRecord
    {
        EnrollmentNumber = enrollmentNumber,
        FirstName = firstName,
        LastName = lastName,
        DateCreated = DateTime.Now,
        FilePath = uploadedPdfPath,
        IsUploaded = false
    });
    
    // Store section paths in database (extend EnrollmentRecord if needed)
    foreach (var section in sections)
    {
        Debug.WriteLine($"Section {section.Key}: {section.Value}");
    }
}
```

### With DMS Service

```csharp
public async Task UploadSplitEnrollmentToDMSAsync(string enrollmentPdfPath)
{
    var splitterService = new PdfSplitterService();
    var dmsService = new DMSService();
    
    // Split form into sections
    var sections = await splitterService.SplitMedicareEnrollmentFormAsync(enrollmentPdfPath);
    
    // Upload each section with appropriate metadata
    foreach (var section in sections)
    {
        var sectionBytes = await File.ReadAllBytesAsync(section.Value);
        var base64Content = Convert.ToBase64String(sectionBytes);
        
        var uploadRequest = new
        {
            DocumentTypeId = 842, // Enrollment
            DocumentSection = section.Key,
            Base64Document = base64Content,
            // ... other metadata
        };
        
        // await dmsService.UploadDocumentAsync(uploadRequest);
        Debug.WriteLine($"Uploaded section: {section.Key}");
    }
}
```

---

## Error Handling

```csharp
public async Task SafePdfSplitAsync(string pdfPath)
{
    var splitterService = new PdfSplitterService();
    
    try
    {
        // Check if file exists
        if (!File.Exists(pdfPath))
        {
            throw new FileNotFoundException($"PDF file not found: {pdfPath}");
        }
        
        // Get info first
        var info = await splitterService.GetPdfInfoAsync(pdfPath);
        Debug.WriteLine($"Processing {info.PageCount} page PDF");
        
        // Split with error handling
        var pages = await splitterService.SplitPdfIntoIndividualPagesAsync(pdfPath);
        Debug.WriteLine($"Successfully split into {pages.Count} files");
    }
    catch (FileNotFoundException ex)
    {
        Debug.WriteLine($"File error: {ex.Message}");
        // Handle missing file
    }
    catch (UnauthorizedAccessException ex)
    {
        Debug.WriteLine($"Permission error: {ex.Message}");
        // Handle permission issues
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"PDF processing error: {ex.Message}");
        // Handle other errors
    }
}
```

---

## Performance Tips

1. **Use Task.Run for large PDFs** (already implemented in service)
2. **Process in background** to avoid UI blocking
3. **Clean up temporary files** after processing
4. **Monitor memory usage** for very large PDFs
5. **Use page ranges** instead of splitting all pages when possible

---

## Common Medicare Form Structures

### CMS-40B (Medicare Advantage)
```
Pages 1-2: Personal Information
Pages 3-4: Plan Selection
Pages 5-6: Current Coverage
Pages 7-8: Authorization & Signatures
```

### CMS-10802 (Enrollment Request)
```
Pages 1-3: Enrollee Information
Pages 4-5: Plan Options
Pages 6-7: Additional Information
Page 8: Signatures
```

**Customize the section splits in `SplitMedicareEnrollmentFormAsync()` based on your actual forms!**

---

## Next Steps

1. Test with sample Medicare PDFs
2. Adjust section page ranges for your specific forms
3. Integrate with DMS upload workflow
4. Add to EnrollmentWizard if needed
5. Consider adding UI for manual page selection

---

## Need Help?

- Check Syncfusion documentation: https://help.syncfusion.com/file-formats/pdf/overview
- Review debug logs for detailed operation info
- Test with small PDFs first before processing large batches
