using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OttaMatta.Application.Responses
{
    public class sound
    {
        [XmlAttribute]
        public int soundid;

        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public string filename;

        [XmlAttribute]
        public string description;

        [XmlAttribute]
        public string uploadedby;

        [XmlAttribute]
        public string uploadDate;

        [XmlAttribute]
        public float averagerating;

        [XmlAttribute]
        public int downloads;

        [XmlAttribute]
        public int size;

        [XmlAttribute]
        public string md5hash;

        [XmlText]
        public string datasixtyfour;

        [XmlAttribute]
        public int hasicon;

        public imagedata imagethumb;
    }

}
