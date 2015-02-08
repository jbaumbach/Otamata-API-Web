using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OttaMatta.Data.Models
{
    public class websearch
    {
        public websearchstatus status { get; set; }
        public List<resultsite> results { get; set; }

        public websearch() 
        {
            //
            // Let's create ourselves completely so the callers don't have to.
            //
            status = new websearchstatus();
            results = new List<resultsite>();
        }
    }
}
