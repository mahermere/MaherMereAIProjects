using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Hyland.Unity;
using Triple_S_AEP_API.Services;
using Swashbuckle.Swagger;
using System.Reflection;

using System.Web.Http.ModelBinding;
using System.Net.Http.Formatting;
using System.Web.Http.Results;
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Kernel.Pdf;
using iText.IO.Image;
using iText.Kernel.Pdf.Canvas;

namespace Triple_S_AEP_API.Controllers
{
    public class DocumentUploadRequest
    {
        public int DocumentTypeId { get; set; }
        public int FileTypeId { get; set; }
        public string Base64Document { get; set; }
        public List<KeywordValue> Keywords { get; set; }
    }

    public class KeywordValue
    {
        public int KeywordTypeId { get; set; }
        public string Value { get; set; }
    }

    public class DocumentSearchRequest
    {
        public List<int> DocumentTypeIds { get; set; }
        public List<int> DocumentTypeGroupIds { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<SearchKeyword> Keywords { get; set; }
        public int? MaxResults { get; set; }
    }

    public class SearchKeyword
    {
        public int KeywordTypeId { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; } // "Equal", "LessThan", etc.
        public string Relation { get; set; } // "And", "Or", etc.
    }

    public class DocumentController : ApiController
    {
        private static string GetFileExtensionFromMimeType(string mimeType)
        {
            switch (mimeType)
            {
                case "image/tiff":
                    return "tiff";
                case "image/tif":
                    return "tif";
                case "application/pdf":
                    return "pdf";
                case "text/plain":
                    return "txt";
                case "application/octet-stream":
                    return "bin";
                default:
                    return "tmp";
            }
        }

        // POST api/document/upload
        [HttpPost]
        [Route("api/document/upload")]
        public IHttpActionResult UploadDocument([FromBody] DocumentUploadRequest request)
        {
            string tempPath = null;
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;

                var documentType = core.DocumentTypes.Find(request.DocumentTypeId);
                if (documentType == null)
                    return BadRequest("Invalid DocumentTypeId.");

                var fileType = core.FileTypes.Find(request.FileTypeId);
                if (fileType == null)
                    return BadRequest("Invalid FileTypeId.");

                if (!documentType.CanI(DocumentTypePrivileges.DocumentCreation))
                    return StatusCode(HttpStatusCode.Forbidden);

                if (string.IsNullOrWhiteSpace(request.Base64Document))
                    return BadRequest("No document data provided.");

                // Decode base64 and save to temp file
                var fileBytes = Convert.FromBase64String(request.Base64Document);
                tempPath = Path.GetTempFileName();
                File.WriteAllBytes(tempPath, fileBytes);

                var storage = core.Storage;
                var storeProps = storage.CreateStoreNewDocumentProperties(documentType, fileType);

                // Add keywords if provided
                if (request.Keywords != null)
                {
                    foreach (var kv in request.Keywords)
                    {
                        var keywordType = core.KeywordTypes.Find(kv.KeywordTypeId);
                        Keyword keyword = null;
                        
                        if (keywordType != null)
                        {
                            if (keywordType.DataType == KeywordDataType.AlphaNumeric) keyword = keywordType.CreateKeyword(kv.Value);
                            if (keywordType.DataType == KeywordDataType.Date) keyword = keywordType.CreateKeyword(DateTime.Parse(kv.Value).Date);
                            if (keywordType.DataType == KeywordDataType.DateTime) keyword = keywordType.CreateKeyword(DateTime.Parse(kv.Value));
                            if (keywordType.DataType == KeywordDataType.FloatingPoint) keyword = keywordType.CreateKeyword(decimal.Parse(kv.Value));
                            if (keywordType.DataType == KeywordDataType.Numeric20) keyword = keywordType.CreateKeyword(long.Parse(kv.Value));
                            if (keywordType.DataType == KeywordDataType.Numeric9) keyword = keywordType.CreateKeyword(long.Parse(kv.Value));



                            storeProps.AddKeyword(keyword);
                        }
                    }
                }

                var pageData = storage.CreatePageData(tempPath);
                var newDoc = storage.StoreNewDocument(pageData, storeProps);

                return Ok(new { DocumentId = newDoc.ID });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempPath) && File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch { }
                }
            }
        }

        // GET api/document/download/{id}
        [HttpGet]
        [Route("api/document/download/{id}")]
        public IHttpActionResult DownloadDocument(int id)
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;
                var doc = core.GetDocumentByID(id);
                if (doc == null)
                    return NotFound();

                var documentType = doc.DocumentType;
                if (!documentType.CanI(DocumentTypePrivileges.DocumentViewing))
                    return StatusCode(HttpStatusCode.Forbidden);

                var rendition = doc.DefaultRenditionOfLatestRevision;
                var imageDataProvider = core.Retrieval.Image;
                var imageGetDocumentProperties = imageDataProvider.CreateImageGetDocumentProperties();
                imageGetDocumentProperties.Overlay = false;
                imageGetDocumentProperties.OverlayAllPages = false;
                imageGetDocumentProperties.RenderNoteAnnotations = false;
                imageGetDocumentProperties.RenderNoteText = false;

                using (var pageData = imageDataProvider.GetDocument(rendition, imageGetDocumentProperties, ImageContentType.Tiff))
                {
                    var stream = pageData.Stream;
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    string base64Data = Convert.ToBase64String(bytes);
                    string mimeType = "image/tiff";

                    return Ok(new
                    {
                        Base64Data = base64Data,
                        MimeType = mimeType,
                        FileName = $"{doc.ID}.{GetFileExtensionFromMimeType(mimeType)}"
                    });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        // GET api/document/keywords
        [HttpGet]
        [Route("api/document/keywords")]
        public IHttpActionResult GetKeywordInfo()
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;

                var keywordTypes = new List<object>();
                foreach (KeywordType keywordType in core.KeywordTypes)
                {
                    keywordTypes.Add(new
                    {
                        KeywordTypeId = keywordType.ID,
                        Name = keywordType.Name
                    });
                }

                return Ok(keywordTypes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/document/keywords/{documentTypeId}
        [HttpGet]
        [Route("api/document/keywords/{documentTypeId}")]
        public IHttpActionResult GetKeywordsByDocumentType(int documentTypeId)
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;

                var documentType = core.DocumentTypes.Find(documentTypeId);
                if (documentType == null)
                    return BadRequest("Invalid DocumentTypeId.");

                var keywordTypes = new List<object>();
                foreach (KeywordRecordType KR in documentType.KeywordRecordTypes)
                {
                    foreach(KeywordType kt in KR.KeywordTypes)
                    {
                        keywordTypes.Add(new
                        {
                            KeywordTypeId = kt.ID,
                            Name = kt.Name,
                            DataType = kt.DataType.ToString()
                            
                        });
                    }
                    
                }

                return Ok(keywordTypes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/document/download/default/{id}
        [HttpGet]
        [Route("api/document/download/default/{id}")]
        public IHttpActionResult DownloadDefault(int id)
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;
                var doc = core.GetDocumentByID(id);
                if (doc == null)
                    return NotFound();

                var documentType = doc.DocumentType;
                if (!documentType.CanI(DocumentTypePrivileges.DocumentViewing))
                    return StatusCode(HttpStatusCode.Forbidden);

                var rendition = doc.DefaultRenditionOfLatestRevision;
                var defaultProvider = core.Retrieval.Default;
                using (var pageData = defaultProvider.GetDocument(rendition))
                {
                    var stream = pageData.Stream;
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    string base64Data = Convert.ToBase64String(bytes);
                    string mimeType = "application/octet-stream";

                    return Ok(new
                    {
                        Base64Data = base64Data,
                        MimeType = mimeType,
                        FileName = $"{doc.ID}.{GetFileExtensionFromMimeType(mimeType)}"
                    });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/document/download/image/{id}
        [HttpGet]
        [Route("api/document/download/image/{id}")]
        public IHttpActionResult DownloadImage(int id)
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;
                var doc = core.GetDocumentByID(id);
                if (doc == null)
                    return NotFound();

                var documentType = doc.DocumentType;
                if (!documentType.CanI(DocumentTypePrivileges.DocumentViewing))
                    return StatusCode(HttpStatusCode.Forbidden);

                var rendition = doc.DefaultRenditionOfLatestRevision;
                var imageProvider = core.Retrieval.Image;
                var imageProps = imageProvider.CreateImageGetDocumentProperties();
                imageProps.Overlay = false;
                imageProps.OverlayAllPages = false;
                imageProps.RenderNoteAnnotations = false;
                imageProps.RenderNoteText = false;

                using (var pageData = imageProvider.GetDocument(rendition, imageProps, ImageContentType.Tiff))
                {
                    var stream = pageData.Stream;
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    string base64Data = Convert.ToBase64String(bytes);
                    string mimeType = "image/tiff";

                    return Ok(new
                    {
                        Base64Data = base64Data,
                        MimeType = mimeType,
                        FileName = $"{doc.ID}.{GetFileExtensionFromMimeType(mimeType)}"
                    });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/document/download/pdf/{id}
        [HttpGet]
        [Route("api/document/download/pdf/{id}")]
        public IHttpActionResult DownloadPdf(int id)
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;
                var doc = core.GetDocumentByID(id);
                if (doc == null)
                    return NotFound();

                var documentType = doc.DocumentType;
                if (!documentType.CanI(DocumentTypePrivileges.DocumentViewing))
                    return StatusCode(HttpStatusCode.Forbidden);

                var rendition = doc.DefaultRenditionOfLatestRevision;
                var pdfProvider = core.Retrieval.PDF;
                using (var pageData = pdfProvider.GetDocument(rendition))
                {
                    var stream = pageData.Stream;
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    string base64Data = Convert.ToBase64String(bytes);
                    string mimeType = "application/pdf";

                    return Ok(new
                    {
                        Base64Data = base64Data,
                        MimeType = mimeType,
                        FileName = $"{doc.ID}.{GetFileExtensionFromMimeType(mimeType)}"
                    });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/document/download/text/{id}
        [HttpGet]
        [Route("api/document/download/text/{id}")]
        public IHttpActionResult DownloadText(int id)
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;
                var doc = core.GetDocumentByID(id);
                if (doc == null)
                    return NotFound();

                var documentType = doc.DocumentType;
                if (!documentType.CanI(DocumentTypePrivileges.DocumentViewing))
                    return StatusCode(HttpStatusCode.Forbidden);

                var rendition = doc.DefaultRenditionOfLatestRevision;
                var textProvider = core.Retrieval.Text;
                using (var pageData = textProvider.GetDocument(rendition))
                {
                    var stream = pageData.Stream;
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    string base64Data = Convert.ToBase64String(bytes);
                    string mimeType = "text/plain";

                    return Ok(new
                    {
                        Base64Data = base64Data,
                        MimeType = mimeType,
                        FileName = $"{doc.ID}.{GetFileExtensionFromMimeType(mimeType)}"
                    });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/document/search/{id}
        [HttpGet]
        [Route("api/document/search/{id}")]
        public IHttpActionResult SearchDocument(int id)
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;
                var doc = core.GetDocumentByID(id);
                if (doc == null)
                    return NotFound();
                return Ok(new
                {
                    doc.ID,
                    doc.Name,
                    CreatedBy = doc.CreatedBy?.Name,
                    doc.DateStored,
                    doc.DocumentDate,
                    DocumentType = doc.DocumentType?.Name,
                    doc.Status
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST api/document/search
        [HttpPost]
        [Route("api/document/search")]
        public IHttpActionResult SearchDocuments([FromBody] DocumentSearchRequest request)
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;
                var docQuery = core.CreateDocumentQuery();

                // Add DocumentTypeGroups
                if (request.DocumentTypeGroupIds != null)
                {
                    foreach (var groupId in request.DocumentTypeGroupIds)
                    {
                        var group = core.DocumentTypeGroups.Find(groupId);
                        if (group != null)
                            docQuery.AddDocumentTypeGroup(group);
                    }
                }

                // Add DocumentTypes
                if (request.DocumentTypeIds != null)
                {
                    foreach (var typeId in request.DocumentTypeIds)
                    {
                        var docType = core.DocumentTypes.Find(typeId);
                        if (docType != null)
                            docQuery.AddDocumentType(docType);
                    }
                }

                // Add Date Range
                if (request.FromDate.HasValue && request.ToDate.HasValue)
                    docQuery.AddDateRange(request.FromDate.Value, request.ToDate.Value);

                // Add Keywords
                if (request.Keywords != null)
                {
                    foreach (var kw in request.Keywords)
                    {
                        var keywordType = core.KeywordTypes.Find(kw.KeywordTypeId);
                        if (keywordType == null) continue;
                        var keyword = keywordType.CreateKeyword(kw.Value);

                        KeywordOperator op = KeywordOperator.Equal;
                        if (!string.IsNullOrEmpty(kw.Operator))
                            Enum.TryParse(kw.Operator, out op);

                        KeywordRelation rel = KeywordRelation.And;
                        if (!string.IsNullOrEmpty(kw.Relation))
                            Enum.TryParse(kw.Relation, out rel);

                        docQuery.AddKeyword(keyword, op, rel);
                    }
                }

                int maxResults = request.MaxResults ?? 150;
                var results = docQuery.ExecuteQueryResults(maxResults);

                var docs = new List<object>();
                foreach (QueryResultItem qr in results.QueryResultItems)
                {
                    Document doc = qr.Document;
                    if (!doc.DocumentType.CanI(DocumentTypePrivileges.DocumentViewing))
                        continue;
                    docs.Add(new
                    {
                        doc.ID,
                        doc.Name,
                        CreatedBy = doc.CreatedBy?.Name,
                        doc.DateStored,
                        doc.DocumentDate,
                        DocumentType = doc.DocumentType?.Name,
                        doc.Status
                    });
                }

                return Ok(docs);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/document/delete/{id}?purge=true
        [HttpDelete]
        [Route("api/document/delete/{id}")]
        public IHttpActionResult DeleteDocument(int id, bool purge = false)
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                var core = app.Core;
                var doc = core.GetDocumentByID(id);
                if (doc == null)
                    return NotFound();

                var documentType = doc.DocumentType;
                var storage = core.Storage;

                if (!documentType.CanI(DocumentTypePrivileges.DocumentDeletion))
                    return StatusCode(HttpStatusCode.Forbidden);

                if (purge)
                {
                    storage.PurgeDocument(doc);
                    return Ok(new { Message = "Document purged (permanent delete)." });
                }
                else
                {
                    storage.DeleteDocument(doc);
                    return Ok(new { Message = "Document deleted (soft delete)." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/document/verify-user
        [HttpGet]
        [Route("api/document/verify-user")]
        public IHttpActionResult VerifyUser()
        {
            try
            {
                var app = HylandUnityConnectionService.Connect(HttpContext.Current.Request);
                
                if (app == null)
                    return Unauthorized();

                return Ok(new
                {
                    IsVerified = true,
                    Message = "User credentials verified successfully."
                });
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
    }

    public class PdfController : ApiController
    {
        public class PdfFlattenRequest
        {
            public string Base64TemplatePdf { get; set; }
            public Dictionary<string, string> Fields { get; set; }
            public Dictionary<string, string> Images { get; set; }
        }
            
        // POST api/pdf/flatten
        [HttpPost]
        [Route("api/pdf/flatten")]
        public IHttpActionResult FlattenPdf([FromBody] PdfFlattenRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.Base64TemplatePdf))
                return BadRequest("Base64TemplatePdf is required.");

            try
            {
                byte[] templateBytes = Convert.FromBase64String(request.Base64TemplatePdf);

                using (var inputStream = new MemoryStream(templateBytes))
                using (var outputStream = new MemoryStream())
                using (var reader = new PdfReader(inputStream))
                using (var writer = new PdfWriter(outputStream))
                using (var pdfDocument = new PdfDocument(reader, writer))
                {
                    var form = PdfAcroForm.GetAcroForm(pdfDocument, false);
                    if (form == null)
                        return BadRequest("Template PDF does not contain AcroForm fields.");

                    var formFields = form.GetFormFields();

                    if (request.Fields != null)
                    {
                        foreach (var field in request.Fields)
                        {
                            if (!formFields.ContainsKey(field.Key)) continue;

                            var f = formFields[field.Key];
                            var raw = field.Value?.Trim();

                            var checkbox = f as iText.Forms.Fields.PdfButtonFormField;
                            if (checkbox != null)
                            {
                                var states = checkbox.GetAppearanceStates(); // e.g. Off, Yes
                                var onState = states.FirstOrDefault(s => !string.Equals(s, "Off", StringComparison.OrdinalIgnoreCase)) ?? "Yes";

                                var isChecked = string.Equals(raw, "true", StringComparison.OrdinalIgnoreCase)
                                             || string.Equals(raw, "1", StringComparison.OrdinalIgnoreCase)
                                             || string.Equals(raw, "yes", StringComparison.OrdinalIgnoreCase)
                                             || string.Equals(raw, onState, StringComparison.OrdinalIgnoreCase);

                                checkbox.SetValue(isChecked ? onState : "Off");
                            }
                            else
                            {
                                f.SetValue(raw ?? string.Empty);
                            }
                        }
                    }

                    if (request.Images != null)
                    {
                        foreach (var imageField in request.Images)
                        {
                            if (!formFields.ContainsKey(imageField.Key)) continue;
                            if (string.IsNullOrWhiteSpace(imageField.Value)) continue;

                            byte[] imageBytes;
                            try
                            {
                                imageBytes = Convert.FromBase64String(imageField.Value);
                            }
                            catch (FormatException)
                            {
                                return BadRequest($"Image for field '{imageField.Key}' is not valid base64.");
                            }

                            var pdfField = formFields[imageField.Key];
                            var widget = pdfField.GetWidgets().FirstOrDefault();
                            if (widget == null || widget.GetPage() == null)
                                continue;

                            var rect = widget.GetRectangle().ToRectangle();
                            var imageData = ImageDataFactory.Create(imageBytes);
                            var canvas = new PdfCanvas(widget.GetPage());
                            canvas.AddImageFittedIntoRectangle(imageData, rect, false);

                            pdfField.SetValue(string.Empty);
                        }
                    }

                    form.FlattenFields();
                    pdfDocument.Close(); // flushes/finalizes PDF into outputStream

                    var flattenedPdf = outputStream.ToArray();
                    return Ok(new
                    {
                        Base64Pdf = Convert.ToBase64String(flattenedPdf),
                        MimeType = "application/pdf",
                        FileName = "flattened.pdf"
                    });
                }
            }
            catch (FormatException)
            {
                return BadRequest("Base64TemplatePdf is not valid base64.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

