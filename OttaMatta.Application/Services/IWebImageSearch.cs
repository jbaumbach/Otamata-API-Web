using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace OttaMatta.Application.Services
{
    [ServiceContract]
    interface IWebImageSearch
    {
        [OperationContract]
        [XmlSerializerFormat]
        OttaMatta.Data.Models.webimagesearch GetResultsXML(string term, string clientIp, string deviceId, string appVersion);

        [OperationContract]
        OttaMatta.Data.Models.webimagesearch GetResultsJSON(string term, string clientIp, string deviceId, string appVersion);
    }
}
