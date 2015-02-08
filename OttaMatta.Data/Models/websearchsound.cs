using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OttaMatta.Common;
using System.Xml.Serialization;

namespace OttaMatta.Data.Models
{
    [Serializable]
    public class websearchsound
    {
        [XmlAttribute]
        public string contenttype = string.Empty;

        [XmlAttribute]
        public bool issound;

        [XmlAttribute]
        public string filename;

        [XmlAttribute]
        public string extension;

        [XmlAttribute]
        public string sourceurl;

        [XmlAttribute]
        public string sourceDomain;

        [XmlAttribute]
        public int searchResultOrder;

        [XmlAttribute]
        public string soundid;

        /// <summary>
        /// Bytes serialized to the raw file
        /// </summary>
        [XmlText]
        public byte[] soundbytes;

        [XmlAttribute]
        public string md5hash;

        /// <summary>
        /// Base64 encoded version of soundbytes, transferred w/web service.
        /// </summary>
        [XmlAttribute]
        public string datasixtyfour;

        [XmlAttribute]
        public long size;

        public override string ToString()
        {
            if (!Functions.IsEmptyString(filename))
            {
                return string.Format("({0}-{1}) {2} ({3} bytes)", searchResultOrder, sourceDomain, filename, size);
            }
            else
            {
                return "Not initialized";
            }
        }
    }
}
