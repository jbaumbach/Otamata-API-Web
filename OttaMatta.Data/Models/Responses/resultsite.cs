using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OttaMatta.Data.Models
{
    public class resultsite
    {
        [XmlAttribute]
        public string sourcedomain;

        public List<resultitem> sounds = new List<resultitem>();
    }
}
