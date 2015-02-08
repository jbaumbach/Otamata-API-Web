using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OttaMatta.Data.Models
{
    public class webimagesearch
    {
        public List<resultimage> results { get; set; }

        public webimagesearch()
        {
            results = new List<resultimage>();
        }

    }
}
