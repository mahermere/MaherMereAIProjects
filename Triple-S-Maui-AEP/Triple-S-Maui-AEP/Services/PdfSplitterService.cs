using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;
using System.Diagnostics;

namespace Triple_S_Maui_AEP.Services
{
    /// <summary>
    /// Service for splitting and processing Medicare Enrollment Form PDFs
    /// </summary>
    public class PdfSplitterService
    {
        /// <summary>
        /// Split a multi-page PDF into individual page PDFs
        /// </summary>
        /// <param name="inputPdfPath">Path to the source PDF file</param>
        /// <param name="outputDirectory">Directory where split PDFs will be saved</param>
        /// <param name="fileNamePrefix">Prefix for output files (e.g., "enrollment_page")</param>
        /// <returns>List of paths to the created PDF files</returns>
        public async Task<List<string>> SplitPdfIntoIndividualPagesAsync(
            string inputPdfPath, 
            string? outputDirectory = null, 
            string fileNamePrefix = "page")
        {
            return await Task.Run(() =>
            {
                var outputPaths = new List<string>();

                try
                {
                    Debug.WriteLine($"📄 Starting PDF split: {inputPdfPath}");

                    // Set default output directory
                    outputDirectory ??= Path.Combine(FileSystem.AppDataDirectory, "split_pdfs");
                    
                    // Ensure output directory exists
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                        Debug.WriteLine($"📁 Created output directory: {outputDirectory}");
                    }

                    // Load the source PDF
                    using var fileStream = new FileStream(inputPdfPath, FileMode.Open, FileAccess.Read);
                    using var sourceDocument = new PdfLoadedDocument(fileStream);
                    
                    Debug.WriteLine($"📊 PDF loaded: {sourceDocument.Pages.Count} pages");

                    // Split each page into a separate PDF
                    for (int i = 0; i < sourceDocument.Pages.Count; i++)
                    {
                        // Create new PDF document for this page
                        using var newDocument = new PdfDocument();
                        
                        // Import the page from source document
                        newDocument.ImportPage(sourceDocument, i);

                        // Generate output file path
                        var outputFileName = $"{fileNamePrefix}_{i + 1:D3}.pdf";
                        var outputPath = Path.Combine(outputDirectory, outputFileName);

                        // Save the single-page PDF
                        using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                        newDocument.Save(outputStream);

                        outputPaths.Add(outputPath);
                        Debug.WriteLine($"✅ Created: {outputFileName}");
                    }

                    Debug.WriteLine($"✅ Successfully split PDF into {outputPaths.Count} files");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error splitting PDF: {ex.Message}");
                    Debug.WriteLine($"   Stack: {ex.StackTrace}");
                    throw;
                }

                return outputPaths;
            });
        }

        /// <summary>
        /// Extract specific pages from a PDF into a new PDF
        /// </summary>
        /// <param name="inputPdfPath">Path to the source PDF file</param>
        /// <param name="pageNumbers">Page numbers to extract (1-based)</param>
        /// <param name="outputPdfPath">Path where the new PDF will be saved</param>
        /// <returns>Path to the created PDF file</returns>
        public async Task<string> ExtractPagesAsync(
            string inputPdfPath, 
            int[] pageNumbers, 
            string? outputPdfPath = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine($"📄 Extracting pages from: {inputPdfPath}");
                    Debug.WriteLine($"   Pages to extract: {string.Join(", ", pageNumbers)}");

                    // Set default output path
                    outputPdfPath ??= Path.Combine(
                        FileSystem.AppDataDirectory, 
                        "extracted_pdfs", 
                        $"extracted_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                    // Ensure output directory exists
                    var outputDir = Path.GetDirectoryName(outputPdfPath);
                    if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    // Load source document
                    using var fileStream = new FileStream(inputPdfPath, FileMode.Open, FileAccess.Read);
                    using var sourceDocument = new PdfLoadedDocument(fileStream);
                    
                    Debug.WriteLine($"📊 Source PDF: {sourceDocument.Pages.Count} total pages");

                    // Validate page numbers
                    foreach (var pageNum in pageNumbers)
                    {
                        if (pageNum < 1 || pageNum > sourceDocument.Pages.Count)
                        {
                            throw new ArgumentException($"Invalid page number: {pageNum}. PDF has {sourceDocument.Pages.Count} pages.");
                        }
                    }

                    // Create new document with extracted pages
                    using var newDocument = new PdfDocument();
                    
                    foreach (var pageNum in pageNumbers.OrderBy(p => p))
                    {
                        // Import page (pageNum is 1-based, array is 0-based)
                        newDocument.ImportPage(sourceDocument, pageNum - 1);
                        Debug.WriteLine($"   Imported page {pageNum}");
                    }

                    // Save the new PDF
                    using var outputStream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write);
                    newDocument.Save(outputStream);

                    Debug.WriteLine($"✅ Extracted {pageNumbers.Length} pages to: {outputPdfPath}");
                    return outputPdfPath;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error extracting pages: {ex.Message}");
                    throw;
                }
            });
        }

        /// <summary>
        /// Extract a range of pages from a PDF into a new PDF
        /// </summary>
        /// <param name="inputPdfPath">Path to the source PDF file</param>
        /// <param name="startPage">Starting page number (1-based, inclusive)</param>
        /// <param name="endPage">Ending page number (1-based, inclusive)</param>
        /// <param name="outputPdfPath">Path where the new PDF will be saved</param>
        /// <returns>Path to the created PDF file</returns>
        public async Task<string> ExtractPageRangeAsync(
            string inputPdfPath, 
            int startPage, 
            int endPage, 
            string? outputPdfPath = null)
        {
            // Generate array of page numbers from range
            var pageNumbers = Enumerable.Range(startPage, endPage - startPage + 1).ToArray();
            return await ExtractPagesAsync(inputPdfPath, pageNumbers, outputPdfPath);
        }

        /// <summary>
        /// Split PDF based on Medicare Enrollment Form sections
        /// </summary>
        /// <param name="inputPdfPath">Path to the complete enrollment form PDF</param>
        /// <param name="outputDirectory">Directory where split sections will be saved</param>
        /// <returns>Dictionary with section names and their file paths</returns>
        public async Task<Dictionary<string, string>> SplitMedicareEnrollmentFormAsync(
            string inputPdfPath, 
            string? outputDirectory = null)
        {
            return await Task.Run(() =>
            {
                var results = new Dictionary<string, string>();

                try
                {
                    Debug.WriteLine($"📄 Splitting Medicare Enrollment Form: {inputPdfPath}");

                    // Set default output directory
                    outputDirectory ??= Path.Combine(FileSystem.AppDataDirectory, "enrollment_sections");
                    
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    using var fileStream = new FileStream(inputPdfPath, FileMode.Open, FileAccess.Read);
                    using var sourceDocument = new PdfLoadedDocument(fileStream);
                    
                    var totalPages = sourceDocument.Pages.Count;
                    Debug.WriteLine($"📊 Total pages: {totalPages}");

                    // Define typical Medicare Enrollment Form sections
                    // Adjust these ranges based on actual form structure
                    var sections = new Dictionary<string, (int start, int end)>
                    {
                        { "Part1_PersonalInfo", (1, 2) },
                        { "Part2_PlanSelection", (3, 4) },
                        { "Part3_CurrentCoverage", (5, 6) },
                        { "Part4_Authorization", (7, 8) }
                    };

                    foreach (var section in sections)
                    {
                        var sectionName = section.Key;
                        var (startPage, endPage) = section.Value;

                        // Skip if pages don't exist in this document
                        if (startPage > totalPages)
                        {
                            Debug.WriteLine($"⚠️ Skipping {sectionName}: pages {startPage}-{endPage} not found");
                            continue;
                        }

                        // Adjust end page if it exceeds total pages
                        endPage = Math.Min(endPage, totalPages);

                        try
                        {
                            using var newDocument = new PdfDocument();
                            
                            // Import pages for this section
                            for (int i = startPage - 1; i < endPage; i++)
                            {
                                newDocument.ImportPage(sourceDocument, i);
                            }

                            var outputPath = Path.Combine(outputDirectory, $"{sectionName}.pdf");
                            using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                            newDocument.Save(outputStream);

                            results[sectionName] = outputPath;
                            Debug.WriteLine($"✅ Created section: {sectionName} (pages {startPage}-{endPage})");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"❌ Error creating section {sectionName}: {ex.Message}");
                        }
                    }

                    Debug.WriteLine($"✅ Created {results.Count} section PDFs");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error splitting Medicare form: {ex.Message}");
                    throw;
                }

                return results;
            });
        }

        /// <summary>
        /// Merge multiple PDF files into a single PDF
        /// </summary>
        /// <param name="inputPdfPaths">List of PDF file paths to merge</param>
        /// <param name="outputPdfPath">Path where the merged PDF will be saved</param>
        /// <returns>Path to the merged PDF file</returns>
        public async Task<string> MergePdfsAsync(
            List<string> inputPdfPaths, 
            string? outputPdfPath = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine($"📄 Merging {inputPdfPaths.Count} PDFs");

                    // Set default output path
                    outputPdfPath ??= Path.Combine(
                        FileSystem.AppDataDirectory, 
                        "merged_pdfs", 
                        $"merged_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                    var outputDir = Path.GetDirectoryName(outputPdfPath);
                    if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    using var mergedDocument = new PdfDocument();

                    foreach (var pdfPath in inputPdfPaths)
                    {
                        if (!File.Exists(pdfPath))
                        {
                            Debug.WriteLine($"⚠️ Skipping missing file: {pdfPath}");
                            continue;
                        }

                        using var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
                        using var sourceDocument = new PdfLoadedDocument(fileStream);
                        
                        Debug.WriteLine($"   Merging: {Path.GetFileName(pdfPath)} ({sourceDocument.Pages.Count} pages)");

                        // Import all pages from this document
                        for (int i = 0; i < sourceDocument.Pages.Count; i++)
                        {
                            mergedDocument.ImportPage(sourceDocument, i);
                        }
                    }

                    using var outputStream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write);
                    mergedDocument.Save(outputStream);

                    Debug.WriteLine($"✅ Merged PDF created: {outputPdfPath}");
                    Debug.WriteLine($"   Total pages: {mergedDocument.Pages.Count}");

                    return outputPdfPath;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error merging PDFs: {ex.Message}");
                    throw;
                }
            });
        }

        /// <summary>
        /// Get page count of a PDF file
        /// </summary>
        public async Task<int> GetPageCountAsync(string pdfPath)
        {
            return await Task.Run(() =>
            {
                using var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
                using var document = new PdfLoadedDocument(fileStream);
                return document.Pages.Count;
            });
        }

        /// <summary>
        /// Get PDF document information
        /// </summary>
        public async Task<PdfDocumentInfo> GetPdfInfoAsync(string pdfPath)
        {
            return await Task.Run(() =>
            {
                using var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
                using var document = new PdfLoadedDocument(fileStream);

                return new PdfDocumentInfo
                {
                    FilePath = pdfPath,
                    FileName = Path.GetFileName(pdfPath),
                    PageCount = document.Pages.Count,
                    FileSize = new FileInfo(pdfPath).Length,
                    Title = document.DocumentInformation.Title,
                    Author = document.DocumentInformation.Author,
                    Subject = document.DocumentInformation.Subject,
                    CreationDate = document.DocumentInformation.CreationDate
                };
            });
        }
    }

    /// <summary>
    /// Information about a PDF document
    /// </summary>
    public class PdfDocumentInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public long FileSize { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Subject { get; set; }
        public DateTime CreationDate { get; set; }

        public string FileSizeFormatted => FormatFileSize(FileSize);

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
