using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Triple_S_Maui_AEP.Models;

namespace Triple_S_Maui_AEP.Services
{
    /// <summary>
    /// Service for PDF generation and manipulation
    /// </summary>
    public class PdfService
    {
        private const string EnrollmentTemplatePath = "Resources/Raw/#1_e LONG_Digital (1).pdf";

        public async Task<byte[]> GeneratePdfAsync(string content)
        {
            using var document = new PdfDocument();
            var page = document.Pages.Add();
            var graphics = page.Graphics;
            var font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

            graphics.DrawString(content ?? string.Empty, font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, 20));

            using var stream = new MemoryStream();
            document.Save(stream);
            return await Task.FromResult(stream.ToArray());
        }

        public async Task<byte[]> GenerateSOAPdfAsync(
            SOAFirstPageRecord soa,
            string soaNumber,
            DateTime meetingDate,
            string? meetingLocation,
            bool productInformationProvided,
            bool complianceDocumentsProvided,
            IEnumerable<string> selectedProducts)
        {
            using var document = new PdfDocument();
            var page = document.Pages.Add();
            var graphics = page.Graphics;

            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
            var headingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
            var bodyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11);

            float y = 24;
            graphics.DrawString("Scope of Appointment (SOA)", titleFont, PdfBrushes.DarkBlue, new Syncfusion.Drawing.PointF(20, y));
            y += 34;

            y = DrawField(graphics, headingFont, bodyFont, "SOA Number", soaNumber, y);
            y = DrawField(graphics, headingFont, bodyFont, "Beneficiary Name", $"{soa.FirstName} {soa.LastName}".Trim(), y);
            y = DrawField(graphics, headingFont, bodyFont, "Date of Birth", soa.DateOfBirth == default ? string.Empty : soa.DateOfBirth.ToString("MM/dd/yyyy"), y);
            y = DrawField(graphics, headingFont, bodyFont, "Medicare Number", soa.MedicareNumber, y);
            y = DrawField(graphics, headingFont, bodyFont, "Phone Number", soa.PrimaryPhone, y);
            y = DrawField(graphics, headingFont, bodyFont, "Meeting Date", meetingDate == default ? string.Empty : meetingDate.ToString("MM/dd/yyyy"), y);
            y = DrawField(graphics, headingFont, bodyFont, "Meeting Location", meetingLocation ?? string.Empty, y);
            y = DrawField(graphics, headingFont, bodyFont, "Product Information Provided", productInformationProvided ? "Yes" : "No", y);
            y = DrawField(graphics, headingFont, bodyFont, "Compliance Documents Provided", complianceDocumentsProvided ? "Yes" : "No", y);
            y = DrawField(graphics, headingFont, bodyFont, "Selected Products", string.Join(", ", selectedProducts.Where(p => !string.IsNullOrWhiteSpace(p))), y);

            y += 8;
            graphics.DrawString("Signatures", headingFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 20;

            y = DrawSignatureBlock(graphics, headingFont, bodyFont, "Beneficiary Signature", soa.EmployeeSignatureBase64, soa.EmployeeSignatureTimestamp, y);
            y = DrawSignatureBlock(graphics, headingFont, bodyFont, "Agent Signature", soa.AgentSignatureBase64, soa.AgentSignatureTimestamp, y);

            using var stream = new MemoryStream();
            document.Save(stream);
            return await Task.FromResult(stream.ToArray());
        }

        /// <summary>
        /// Fills an existing enrollment PDF template with form data
        /// </summary>
        public async Task<byte[]> FillEnrollmentPdfAsync(
            string enrollmentNumber,
            EnrollmentRecord enrollment,
            string? enrolleeSignatureBase64 = null,
            string? agentSignatureBase64 = null,
            string? witnessSignatureBase64 = null,
            bool usesXMark = false)
        {
            System.Diagnostics.Debug.WriteLine("=== FillEnrollmentPdfAsync START ===");
            
            try
            {
                // Load the PDF template
                System.Diagnostics.Debug.WriteLine("Attempting to load PDF template...");
                var templateStream = await LoadPdfTemplateAsync();
                
                if (templateStream == null)
                {
                    System.Diagnostics.Debug.WriteLine("⚠ Template not found - using fallback generation");
                    return await GenerateEnrollmentPdfAsync(enrollmentNumber, enrollment, enrolleeSignatureBase64, agentSignatureBase64, witnessSignatureBase64, usesXMark);
                }

                System.Diagnostics.Debug.WriteLine("✓ Template loaded successfully, creating PdfLoadedDocument...");
                
                using var loadedDocument = new PdfLoadedDocument(templateStream);
                System.Diagnostics.Debug.WriteLine($"✓ PDF loaded: {loadedDocument.Pages.Count} pages");
                
                // Check if PDF has form fields
                if (loadedDocument.Form != null)
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Form found with {loadedDocument.Form.Fields.Count} fields");
                    
                    // Fill form fields
                    FillFormFields(loadedDocument.Form, enrollment, enrollmentNumber);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠ No form fields found in PDF");
                }

                // Add signatures to the document
                System.Diagnostics.Debug.WriteLine("Adding signatures...");
                await AddSignaturesToPdfAsync(loadedDocument, enrolleeSignatureBase64, agentSignatureBase64, witnessSignatureBase64, usesXMark);
                System.Diagnostics.Debug.WriteLine("✓ Signatures added");

                using var outputStream = new MemoryStream();
                loadedDocument.Save(outputStream);
                System.Diagnostics.Debug.WriteLine($"✓ PDF saved, size: {outputStream.Length} bytes");
                System.Diagnostics.Debug.WriteLine("=== FillEnrollmentPdfAsync SUCCESS ===");
                
                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Error filling PDF template: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"  Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"  Stack: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("=== Using fallback generation ===");
                
                // Fallback to generated PDF
                return await GenerateEnrollmentPdfAsync(enrollmentNumber, enrollment, enrolleeSignatureBase64, agentSignatureBase64, witnessSignatureBase64, usesXMark);
            }
        }

        /// <summary>
        /// Loads the enrollment PDF template from resources
        /// </summary>
        private async Task<Stream?> LoadPdfTemplateAsync()
        {
            try
            {
                // Primary: Load from MAUI Raw resources (Resources/Raw/)
                try
                {
                    // Try the exact template name first
                    var stream = await FileSystem.OpenAppPackageFileAsync("enrollment_template.pdf");
                    if (stream != null)
                    {
                        System.Diagnostics.Debug.WriteLine("✓ Loaded template from: enrollment_template.pdf");
                        return stream;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"enrollment_template.pdf not found: {ex.Message}");
                }

                // Try the documentation template name
                try
                {
                    var stream = await FileSystem.OpenAppPackageFileAsync("#1_e LONG_Digital (1).pdf");
                    if (stream != null)
                    {
                        System.Diagnostics.Debug.WriteLine("✓ Loaded template from: #1_e LONG_Digital (1).pdf");
                        return stream;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"#1_e LONG_Digital (1).pdf not found: {ex.Message}");
                }

                // Try without special characters
                try
                {
                    var stream = await FileSystem.OpenAppPackageFileAsync("1_e_LONG_Digital.pdf");
                    if (stream != null)
                    {
                        System.Diagnostics.Debug.WriteLine("✓ Loaded template from: 1_e_LONG_Digital.pdf");
                        return stream;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"1_e_LONG_Digital.pdf not found: {ex.Message}");
                }

                // Fallback: Try from embedded resources
                var assembly = typeof(PdfService).Assembly;
                var resourceNames = assembly.GetManifestResourceNames();
                System.Diagnostics.Debug.WriteLine($"Available embedded resources: {string.Join(", ", resourceNames)}");
                
                var resourceName = resourceNames.FirstOrDefault(n => 
                    n.Contains("LONG_Digital", StringComparison.OrdinalIgnoreCase) || 
                    n.Contains("enrollment", StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(resourceName))
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Loaded template from embedded resource: {resourceName}");
                    return assembly.GetManifestResourceStream(resourceName);
                }

                // Last resort: Try file system (for development/debugging)
                var appDataPath = Path.Combine(FileSystem.AppDataDirectory, "templates", "enrollment_template.pdf");
                if (File.Exists(appDataPath))
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Loaded template from: {appDataPath}");
                    return File.OpenRead(appDataPath);
                }

                System.Diagnostics.Debug.WriteLine("✗ NO TEMPLATE FOUND - Will use fallback generation");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Error loading PDF template: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"  Stack: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Fills form fields in the PDF with enrollment data
        /// </summary>
        private void FillFormFields(PdfLoadedForm form, EnrollmentRecord enrollment, string enrollmentNumber)
        {
            var valueByAlias = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Enrollment / Personal
                ["enrollmentnumber"] = enrollmentNumber,
                ["scopeofappointment"] = enrollmentNumber,
                ["first"] = enrollment.FirstName,
                ["firstname"] = enrollment.FirstName,
                ["middleinitial"] = enrollment.MiddleInitial,
                ["last"] = enrollment.LastName,
                ["lastname"] = enrollment.LastName,
                ["birthdate"] = enrollment.DateOfBirth == default ? string.Empty : enrollment.DateOfBirth.ToString("MM/dd/yyyy"),
                ["dateofbirth"] = enrollment.DateOfBirth == default ? string.Empty : enrollment.DateOfBirth.ToString("MM/dd/yyyy"),
                ["dob"] = enrollment.DateOfBirth == default ? string.Empty : enrollment.DateOfBirth.ToString("MM/dd/yyyy"),
                ["gender"] = enrollment.Gender,
                ["sex"] = enrollment.Gender,

                // Permanent residence address
                ["permanentaddressline1"] = enrollment.Address1,
                ["permanentaddress1"] = enrollment.Address1,
                ["residenceaddressline1"] = enrollment.Address1,
                ["residenceaddress1"] = enrollment.Address1,
                ["addressline1"] = enrollment.Address1,
                ["streetaddressline1"] = enrollment.Address1,
                ["address1"] = enrollment.Address1,

                ["permanentaddressline2"] = enrollment.Address2,
                ["permanentaddress2"] = enrollment.Address2,
                ["residenceaddressline2"] = enrollment.Address2,
                ["residenceaddress2"] = enrollment.Address2,
                ["addressline2"] = enrollment.Address2,
                ["streetaddressline2"] = enrollment.Address2,
                ["address2"] = enrollment.Address2,

                ["permanentcity"] = enrollment.City,
                ["residencecity"] = enrollment.City,
                ["city"] = enrollment.City,
                ["permanentstate"] = enrollment.State,
                ["residencestate"] = enrollment.State,
                ["state"] = enrollment.State,
                ["county"] = enrollment.County,
                ["permanentzip"] = enrollment.ZipCode,
                ["residencezip"] = enrollment.ZipCode,
                ["zip"] = enrollment.ZipCode,
                ["zipcode"] = enrollment.ZipCode,

                // Mailing address
                ["mailingaddressline1"] = enrollment.MailingAddress1,
                ["mailingstreetaddressline1"] = enrollment.MailingAddress1,
                ["mailingaddress1"] = enrollment.MailingAddress1,
                ["mailaddress1"] = enrollment.MailingAddress1,

                ["mailingaddressline2"] = enrollment.MailingAddress2,
                ["mailingstreetaddressline2"] = enrollment.MailingAddress2,
                ["mailingaddress2"] = enrollment.MailingAddress2,
                ["mailaddress2"] = enrollment.MailingAddress2,

                ["mailingcity"] = enrollment.MailingCity,
                ["mailcity"] = enrollment.MailingCity,
                ["mailingstate"] = enrollment.MailingState,
                ["mailstate"] = enrollment.MailingState,
                ["mailingzip"] = enrollment.MailingZipCode,
                ["mailingzipcode"] = enrollment.MailingZipCode,
                ["mailzip"] = enrollment.MailingZipCode,

                // Contact
                ["primaryphone"] = enrollment.PrimaryPhone,
                ["homephone"] = enrollment.PrimaryPhone,
                ["phone"] = enrollment.PrimaryPhone,
                ["phonenumber"] = enrollment.PrimaryPhone,
                ["alternatephone"] = enrollment.SecondaryPhone,
                ["secondaryphone"] = enrollment.SecondaryPhone,
                ["email"] = enrollment.Email,
                ["emailaddress"] = enrollment.Email,

                // Medicare / preference
                ["medicarenumber"] = enrollment.MedicareNumber,
                ["medicare"] = enrollment.MedicareNumber,
                ["ssn"] = string.IsNullOrWhiteSpace(enrollment.SSN) ? string.Empty : "***-**-****",
                ["socialsecurity"] = string.IsNullOrWhiteSpace(enrollment.SSN) ? string.Empty : "***-**-****",
                ["preferredcontact"] = enrollment.PreferredContactMethod,
                ["contactmethod"] = enrollment.PreferredContactMethod
            };

            // Priority order for contains matching. Mailing aliases are intentionally first.
            var aliasPriority = valueByAlias.Keys
                .OrderByDescending(k => k.StartsWith("mail", StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(k => k.Length)
                .ToList();

            foreach (PdfLoadedField field in form.Fields)
            {
                var originalName = field.Name ?? string.Empty;
                var normalizedFieldName = NormalizeFieldName(originalName);

                string? resolvedValue = null;

                // 1) exact alias match after normalization
                if (valueByAlias.TryGetValue(normalizedFieldName, out var exactValue))
                {
                    resolvedValue = exactValue;
                }
                else
                {
                    // 2) contains fallback using priority list
                    foreach (var alias in aliasPriority)
                    {
                        if (normalizedFieldName.Contains(alias, StringComparison.OrdinalIgnoreCase))
                        {
                            resolvedValue = valueByAlias[alias];
                            break;
                        }
                    }
                }

                if (resolvedValue != null)
                {
                    SetFieldValue(field, resolvedValue);
                }

                if (field is PdfLoadedCheckBoxField checkbox)
                {
                    if (normalizedFieldName.Contains("primarymobile") || normalizedFieldName.Contains("primaryismobile") || normalizedFieldName.Contains("homephoneiscellular"))
                    {
                        checkbox.Checked = enrollment.PrimaryPhoneIsMobile;
                    }
                    else if (normalizedFieldName.Contains("secondarymobile") || normalizedFieldName.Contains("alternatephoneiscellular") || normalizedFieldName.Contains("secondaryismobile"))
                    {
                        checkbox.Checked = enrollment.SecondaryPhoneIsMobile;
                    }
                    else if (normalizedFieldName.Contains("differentmail") || normalizedFieldName.Contains("mailingdifferent"))
                    {
                        checkbox.Checked = enrollment.DifferentMailingAddress;
                    }

                    checkbox.BorderWidth = 0;
                }
            }

            form.Flatten = true;
        }

        private static string NormalizeFieldName(string? fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return string.Empty;
            }

            var chars = fieldName
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray();

            return new string(chars);
        }

        private static void SetFieldValue(PdfLoadedField field, string value)
        {
            if (field is PdfLoadedTextBoxField textBox)
            {
                textBox.Text = value;
                textBox.BorderWidth = 0;
            }
            else if (field is PdfLoadedComboBoxField comboBox)
            {
                comboBox.SelectedValue = value;
                comboBox.BorderWidth = 0;
            }
        }
        /// <summary>
        /// Adds signature images to the PDF at predefined locations
        /// </summary>
        private async Task AddSignaturesToPdfAsync(
            PdfLoadedDocument document,
            string? enrolleeSignatureBase64,
            string? agentSignatureBase64,
            string? witnessSignatureBase64,
            bool usesXMark)
        {
            await Task.Run(() =>
            {
                // Get the last page (typically where signatures are)
                var page = document.Pages[document.Pages.Count - 1];
                var graphics = page.Graphics;

                var font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);

                // Enrollee Signature (adjust coordinates based on your PDF)
                float enrolleeSigX = 50;
                float enrolleeSigY = page.Size.Height - 250;
                
                if (usesXMark)
                {
                    graphics.DrawString("X", new PdfStandardFont(PdfFontFamily.Helvetica, 24, PdfFontStyle.Bold), 
                        PdfBrushes.Black, new Syncfusion.Drawing.PointF(enrolleeSigX, enrolleeSigY));
                }
                else if (!string.IsNullOrWhiteSpace(enrolleeSignatureBase64))
                {
                    try
                    {
                        byte[] signatureBytes = Convert.FromBase64String(enrolleeSignatureBase64);
                        using var signatureStream = new MemoryStream(signatureBytes);
                        var signatureImage = new PdfBitmap(signatureStream);
                        graphics.DrawImage(signatureImage, new Syncfusion.Drawing.RectangleF(enrolleeSigX, enrolleeSigY, 150, 50));
                    }
                    catch { }
                }

                // Agent Signature
                float agentSigX = 300;
                float agentSigY = page.Size.Height - 250;
                
                if (!string.IsNullOrWhiteSpace(agentSignatureBase64))
                {
                    try
                    {
                        byte[] signatureBytes = Convert.FromBase64String(agentSignatureBase64);
                        using var signatureStream = new MemoryStream(signatureBytes);
                        var signatureImage = new PdfBitmap(signatureStream);
                        graphics.DrawImage(signatureImage, new Syncfusion.Drawing.RectangleF(agentSigX, agentSigY, 150, 50));
                    }
                    catch { }
                }

                // Witness Signature (if X mark used)
                if (usesXMark && !string.IsNullOrWhiteSpace(witnessSignatureBase64))
                {
                    float witnessSigX = 50;
                    float witnessSigY = page.Size.Height - 150;
                    
                    try
                    {
                        byte[] signatureBytes = Convert.FromBase64String(witnessSignatureBase64);
                        using var signatureStream = new MemoryStream(signatureBytes);
                        var signatureImage = new PdfBitmap(signatureStream);
                        graphics.DrawImage(signatureImage, new Syncfusion.Drawing.RectangleF(witnessSigX, witnessSigY, 150, 50));
                    }
                    catch { }
                }
            });
        }

        /// <summary>
        /// Generates a new enrollment PDF (fallback method)
        /// </summary>
        public async Task<byte[]> GenerateEnrollmentPdfAsync(
            string enrollmentNumber,
            EnrollmentRecord enrollment,
            string? enrolleeSignatureBase64 = null,
            string? agentSignatureBase64 = null,
            string? witnessSignatureBase64 = null,
            bool usesXMark = false)
        {
            using var document = new PdfDocument();
            var page = document.Pages.Add();
            var graphics = page.Graphics;

            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
            var headingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
            var subHeadingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
            var bodyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11);

            float y = 24;

            // Title
            graphics.DrawString("Medicare Advantage Enrollment Application", titleFont, PdfBrushes.DarkBlue, new Syncfusion.Drawing.PointF(20, y));
            y += 34;

            graphics.DrawString($"Enrollment #: {enrollmentNumber}", headingFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 24;

            // Section 1: Personal Information
            y = DrawSectionHeader(graphics, subHeadingFont, "1. Personal Information", y);
            y = DrawField(graphics, bodyFont, bodyFont, "First Name", enrollment.FirstName, y);
            y = DrawField(graphics, bodyFont, bodyFont, "Middle Initial", enrollment.MiddleInitial, y);
            y = DrawField(graphics, bodyFont, bodyFont, "Last Name", enrollment.LastName, y);
            y = DrawField(graphics, bodyFont, bodyFont, "Date of Birth", enrollment.DateOfBirth == default ? string.Empty : enrollment.DateOfBirth.ToString("MM/dd/yyyy"), y);
            y = DrawField(graphics, bodyFont, bodyFont, "Gender", enrollment.Gender, y);
            y = DrawField(graphics, bodyFont, bodyFont, "Primary Phone", $"{enrollment.PrimaryPhone}{(enrollment.PrimaryPhoneIsMobile ? " (Mobile)" : "")}", y);
            y = DrawField(graphics, bodyFont, bodyFont, "Secondary Phone", $"{enrollment.SecondaryPhone}{(enrollment.SecondaryPhoneIsMobile ? " (Mobile)" : "")}", y);
            y = DrawField(graphics, bodyFont, bodyFont, "Email", enrollment.Email, y);
            y = DrawField(graphics, bodyFont, bodyFont, "Medicare Number", enrollment.MedicareNumber, y);
            y = DrawField(graphics, bodyFont, bodyFont, "SSN", string.IsNullOrWhiteSpace(enrollment.SSN) ? "Not Provided" : "***-**-****", y);
            y = DrawField(graphics, bodyFont, bodyFont, "Preferred Contact", enrollment.PreferredContactMethod, y);
            y += 12;

            // Add new page if needed
            if (y > 700)
            {
                page = document.Pages.Add();
                graphics = page.Graphics;
                y = 24;
            }

            // Section 2: Signatures
            y = DrawSectionHeader(graphics, subHeadingFont, "2. Signatures", y);
            
            // Enrollee Signature
            graphics.DrawString("Enrollee Signature:", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 16;

            if (usesXMark)
            {
                graphics.DrawString("X Mark", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                y += 18;
            }
            else if (!string.IsNullOrWhiteSpace(enrolleeSignatureBase64))
            {
                try
                {
                    byte[] signatureBytes = Convert.FromBase64String(enrolleeSignatureBase64);
                    using var signatureStream = new MemoryStream(signatureBytes);
                    var signatureImage = new PdfBitmap(signatureStream);
                    graphics.DrawImage(signatureImage, new Syncfusion.Drawing.RectangleF(24, y, 220, 70));
                    y += 76;
                }
                catch
                {
                    graphics.DrawString("Signature could not be rendered", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                    y += 18;
                }
            }
            else
            {
                graphics.DrawString("Not provided", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                y += 18;
            }

            graphics.DrawString($"Timestamp: {enrollment.EnrolleeSignatureTimestamp}", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
            y += 24;

            // Agent Signature
            graphics.DrawString("Agent Signature:", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 16;

            if (!string.IsNullOrWhiteSpace(agentSignatureBase64))
            {
                try
                {
                    byte[] signatureBytes = Convert.FromBase64String(agentSignatureBase64);
                    using var signatureStream = new MemoryStream(signatureBytes);
                    var signatureImage = new PdfBitmap(signatureStream);
                    graphics.DrawImage(signatureImage, new Syncfusion.Drawing.RectangleF(24, y, 220, 70));
                    y += 76;
                }
                catch
                {
                    graphics.DrawString("Signature could not be rendered", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                    y += 18;
                }
            }
            else
            {
                graphics.DrawString("Not provided", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                y += 18;
            }

            graphics.DrawString($"Timestamp: {enrollment.AgentSignatureTimestamp}", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
            y += 24;

            // Witness Signature (if X mark used)
            if (usesXMark)
            {
                graphics.DrawString("Witness Signature:", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
                y += 16;

                if (!string.IsNullOrWhiteSpace(witnessSignatureBase64))
                {
                    try
                    {
                        byte[] signatureBytes = Convert.FromBase64String(witnessSignatureBase64);
                        using var signatureStream = new MemoryStream(signatureBytes);
                        var signatureImage = new PdfBitmap(signatureStream);
                        graphics.DrawImage(signatureImage, new Syncfusion.Drawing.RectangleF(24, y, 220, 70));
                        y += 76;
                    }
                    catch
                    {
                        graphics.DrawString("Signature could not be rendered", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                        y += 18;
                    }
                }
                else
                {
                    graphics.DrawString("Not provided", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                    y += 18;
                }

                graphics.DrawString($"Timestamp: {enrollment.WitnessSignatureTimestamp}", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                y += 24;
            }

            y += 12;
            graphics.DrawString($"Generated on {DateTime.Now:MM/dd/yyyy HH:mm}", bodyFont, PdfBrushes.Gray, new Syncfusion.Drawing.PointF(20, y));

            using var stream = new MemoryStream();
            document.Save(stream);
            return await Task.FromResult(stream.ToArray());
        }

        public async Task<string?> SavePdfAsync(byte[] pdfData, string fileName, string subFolder = "pdf")
        {
            if (pdfData == null || pdfData.Length == 0)
            {
                return null;
            }

            var appDataPath = FileSystem.AppDataDirectory;
            var pdfPath = Path.Combine(appDataPath, subFolder);

            if (!Directory.Exists(pdfPath))
            {
                Directory.CreateDirectory(pdfPath);
            }

            var safeFileName = string.IsNullOrWhiteSpace(fileName) ? $"Document_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf" : fileName;
            if (!safeFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                safeFileName += ".pdf";
            }

            var fullPath = Path.Combine(pdfPath, safeFileName);
            await File.WriteAllBytesAsync(fullPath, pdfData);

            return fullPath;
        }

        public async Task<string> ExtractTextFromPdfAsync(byte[] pdfData)
        {
            await Task.Delay(1);
            return string.Empty;
        }

        public async Task<bool> ValidatePdfAsync(byte[] pdfData)
        {
            await Task.Delay(1);
            return pdfData.Length > 0;
        }

        private static float DrawField(PdfGraphics graphics, PdfFont headingFont, PdfFont bodyFont, string label, string? value, float y)
        {
            graphics.DrawString($"{label}:", headingFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            graphics.DrawString(value ?? string.Empty, bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(170, y));
            return y + 18;
        }

        private static float DrawSectionHeader(PdfGraphics graphics, PdfFont font, string text, float y)
        {
            graphics.DrawString(text, font, PdfBrushes.DarkBlue, new Syncfusion.Drawing.PointF(20, y));
            return y + 22;
        }

        private static float DrawSignatureBlock(
            PdfGraphics graphics,
            PdfFont headingFont,
            PdfFont bodyFont,
            string label,
            string? signatureBase64,
            string? timestamp,
            float y)
        {
            graphics.DrawString(label, headingFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(20, y));
            y += 16;

            if (string.IsNullOrWhiteSpace(signatureBase64) || signatureBase64.Equals("XMARK", StringComparison.OrdinalIgnoreCase))
            {
                var marker = signatureBase64?.Equals("XMARK", StringComparison.OrdinalIgnoreCase) == true ? "X Mark" : "Not provided";
                graphics.DrawString(marker, bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                y += 18;
            }
            else
            {
                try
                {
                    byte[] signatureBytes = Convert.FromBase64String(signatureBase64);
                    using var signatureStream = new MemoryStream(signatureBytes);
                    var signatureImage = new PdfBitmap(signatureStream);
                    graphics.DrawImage(signatureImage, new Syncfusion.Drawing.RectangleF(24, y, 220, 70));
                    y += 76;
                }
                catch
                {
                    graphics.DrawString("Signature could not be rendered", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                    y += 18;
                }
            }

            if (!string.IsNullOrWhiteSpace(timestamp))
            {
                graphics.DrawString($"Timestamp: {timestamp}", bodyFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(24, y));
                y += 18;
            }

            return y + 8;
        }
    }
}
