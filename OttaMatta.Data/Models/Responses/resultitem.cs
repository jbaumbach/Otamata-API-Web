using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OttaMatta.Data.Models
{
    public class resultitem
    {
        [XmlAttribute]
        public string filename;

        [XmlAttribute]
        public string sourceurl;

        [XmlAttribute]
        public string itemid;

        [XmlAttribute]
        public string md5hash;

        [XmlText]
        public string datasixtyfour;

        [XmlAttribute]
        public long size;
    }
}
