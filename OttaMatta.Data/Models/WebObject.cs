using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OttaMatta.Data.Models
{
    [Serializable]
    public class WebObject
    {
        public string Url { get; set; }
        public string MimeType { get; set; }
        public IList<string> FoundUrls { get; set; }
        public string Content { get; set; }
    }
}
