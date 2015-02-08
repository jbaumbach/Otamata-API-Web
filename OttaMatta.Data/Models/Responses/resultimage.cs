using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OttaMatta.Data.Models
{
    public class resultimage
    {
        [XmlAttribute]
        public string url;

        [XmlAttribute]
        public string thumbnailurl;

        [XmlAttribute]
        public string sourceurl;
    }
}
