using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OttaMatta.Application.Responses
{
    public enum ResultStatus
    {
        Success = 0,
        Error = 1
    }

    /// <summary>
    /// Generic response 
    /// </summary>
    public class status
    {
        private ResultStatus _code;

        [XmlAttribute]
        public int code 
        { 
            get { return (int)_code; }
            set { if (Enum.IsDefined(typeof(ResultStatus), value)) _code = (ResultStatus)value; else throw new ArgumentException(string.Format("{0} is not a valid ResultStatus", value)); }
        }

        [XmlAttribute]
        public string description;

        public status() { }

        public status(ResultStatus stat) : this()
        {
            _code = stat;
            description = stat.ToString();
        }
    }
}
