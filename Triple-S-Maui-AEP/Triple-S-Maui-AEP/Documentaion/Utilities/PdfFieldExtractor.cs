using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Interactive;
using System;
using System.IO;

namespace TripleS.Utilities
{
    public static class PdfFieldExtractor
    {
        /// <summary>
        /// Extracts and prints all form field names and types from a PDF template.
        /// </summary>
        /// <param name="templatePath">Path to the CMS PDF template.</param>
        public static void ListPdfFormFields(string templatePath)
        {
            using (FileStream templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                PdfLoadedDocument loadedDoc = new PdfLoadedDocument(templateStream);
                foreach (PdfField field in loadedDoc.Form.Fields)
                {
                    Console.WriteLine($"Field: {field.Name}, Type: {field.GetType().Name}");
                }
            }
        }
    }
}
