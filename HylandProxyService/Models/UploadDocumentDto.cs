using System.Collections.Generic;

namespace HylandProxyService.Models
{
    public class UploadDocumentDto
    {
        public string Name { get; set; }
        public string ContentBase64 { get; set; }
        public string DocumentType { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
