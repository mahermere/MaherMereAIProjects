using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Graphics;
using System.IO;

namespace TripleS.Utilities
{
        public static class PdfFormOverlayUtility
        {
            /// <summary>
            /// Overload for OverlayDataOnCmsPdf without signature images.
            /// </summary>
            public static void OverlayDataOnCmsPdf(
                string templatePath,
                string outputPath,
                Dictionary<string, string> fieldData)
            {
                OverlayDataOnCmsPdf(templatePath, outputPath, fieldData, new Dictionary<int, Stream>());
            }
        /// <summary>
        /// Overlays user data onto a multi-page CMS PDF template (English or Spanish).
        /// </summary>
        /// <param name="templatePath">Path to the CMS PDF template (17 pages).</param>
        /// <param name="outputPath">Path to save the filled PDF.</param>
        /// <param name="fieldData">Dictionary of field names and values to overlay.</param>
        /// <param name="signatureImages">Dictionary of page numbers and signature image streams.</param>
        public static void OverlayDataOnCmsPdf(
            string templatePath,
            string outputPath,
            Dictionary<string, string> fieldData,
            Dictionary<int, Stream> signatureImages)
        {
            // Load the CMS PDF template
            using (FileStream templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                PdfLoadedDocument loadedDoc = new PdfLoadedDocument(templateStream);

                // Example: Overlay text fields
                foreach (var field in fieldData)
                {
                    // You must define the mapping: field name -> page number, coordinates, font, etc.
                    // Example: OverlayField(loadedDoc, pageNum, x, y, field.Value);
                }

                // Example: Overlay signatures
                if (signatureImages != null)
                {
                    foreach (var sig in signatureImages)
                    {
                        int pageNum = sig.Key;
                        Stream sigStream = sig.Value;
                        if (pageNum >= 0 && pageNum < loadedDoc.Pages.Count)
                        {
                            PdfLoadedPage? page = loadedDoc.Pages[pageNum] as PdfLoadedPage;
                            if (page != null)
                            {
                                var graphics = page.Graphics;
                                if (graphics != null)
                                {
                                    PdfBitmap sigImage = new PdfBitmap(sigStream);
                                    // Example coordinates: adjust as needed
                                    graphics.DrawImage(sigImage, 100, 600, 200, 60);
                                }
                            }
                        }
                    }
                }

                // Save the filled PDF
                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    loadedDoc.Save(outputStream);
                }
            }
        }

        // You can add helper methods for field mapping, font selection, etc.
    }
}
