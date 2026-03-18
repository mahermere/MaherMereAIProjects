using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Interactive;
using System.IO;
using System.Collections.Generic;
using Syncfusion.Pdf.Graphics;

namespace TripleS.Utilities
{
    public static class EnrollmentPdfGenerator
    {
        /// <summary>
        /// Generates a filled CMS PDF using the official template and overlays user data.
        /// </summary>
        /// <param name="templatePath">Path to the CMS PDF template (English or Spanish).</param>
        /// <param name="outputPath">Path to save the filled PDF.</param>
        /// <param name="fieldData">Dictionary of field names and values to fill.</param>
        /// <param name="signatureImages">Dictionary of page numbers and signature image streams.</param>
        public static void GenerateCMSPDF(
            string templatePath,
            string outputPath,
            Dictionary<string, string> fieldData,
            Dictionary<int, Stream> signatureImages)
        {
            using (FileStream templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                PdfLoadedDocument loadedDoc = new PdfLoadedDocument(templateStream);

                // Fill form fields if present
                foreach (var field in fieldData)
                {
                    // Special handling for SOA number
                    if (field.Key == "ScopeodAppointmentNumber" && !string.IsNullOrWhiteSpace(field.Value))
                    {
                        if (loadedDoc.Form.Fields[field.Key] is PdfLoadedTextBoxField soaField)
                        {
                            soaField.Text = field.Value;
                        }
                        continue;
                    }
                    if (loadedDoc.Form.Fields[field.Key] is PdfLoadedTextBoxField textField)
                    {
                        textField.Text = field.Value;
                    }
                    // Add more field types as needed (checkboxes, etc.)
                }

                // Overlay signatures as images
                if (signatureImages != null)
                {
                    foreach (var sig in signatureImages)
                    {
                        int pageNum = sig.Key;
                        Stream sigStream = sig.Value;
                        PdfLoadedPage? page = loadedDoc.Pages[pageNum] as PdfLoadedPage;
                        if (page != null && page.Graphics != null)
                        {
                            PdfGraphics graphics = page.Graphics;
                            PdfBitmap sigImage = new PdfBitmap(sigStream);
                            // Example coordinates: adjust as needed
                            graphics.DrawImage(sigImage, 100, 600, 200, 60);
                        }
                    }
                }

                // Flatten the form fields so they are no longer editable
                loadedDoc.Form.Flatten = true;

                // Save the filled and flattened PDF
                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    loadedDoc.Save(outputStream);
                }
            }
        }
    }
}
