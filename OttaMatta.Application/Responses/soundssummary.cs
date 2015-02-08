using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OttaMatta.Application.Responses
{
    /// <summary>
    /// This object is returned by the sound search function. 
    /// </summary>
    public class soundssummary
    {
        [XmlAttribute]
        public string totalresults = null;

        public List<sound> sounds = new List<sound>();
    }
}
