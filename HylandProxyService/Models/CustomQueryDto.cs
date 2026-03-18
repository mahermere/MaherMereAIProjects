using System.Collections.Generic;

namespace HylandProxyService.Models
{
    public class CustomQueryDto
    {
        public string Query { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
