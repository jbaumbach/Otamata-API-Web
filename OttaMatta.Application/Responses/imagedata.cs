using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OttaMatta.Application.Responses
{
    public class imagedata
    {
        [XmlAttribute]
        public int soundid;

        [XmlAttribute]
        public string extension;

        [XmlAttribute]
        public string md5hash;
        
        [XmlText]
        public string datasixtyfour;
    }
}
