using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OttaMatta.Data.Models
{
    [Serializable]
    public class websearchstatus
    {
        [XmlAttribute]
        public float percentcomplete;

        [XmlAttribute]
        public string searchterm;

        [XmlAttribute]
        public int isdone;

        [XmlAttribute]
        public int urlsSearched;

        [XmlAttribute]
        public int urlsTotal;

        [XmlAttribute]
        public int itemsfound;
    }
}
