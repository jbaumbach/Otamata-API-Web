using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace OttaMatta.Application.Responses
{
    public class errordetail
    {
        public System.Net.HttpStatusCode statuscode;
        public string reason { get; set; }

        public errordetail()
        {
        }

        public errordetail(string reasonDesc, HttpStatusCode statusCode)
        {
            reason = reasonDesc;
            statuscode = statusCode;
        }
    }
}
