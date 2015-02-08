using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using OttaMatta.Application.Responses;

namespace OttaMatta.Application.Services
{
    [ServiceContract]
    interface IWebSearch
    {
        [OperationContract]
        [XmlSerializerFormat]
        OttaMatta.Data.Models.websearch GetResultsXML(string term, string clientIp, string deviceId, string appVersion);

        [OperationContract]
        OttaMatta.Data.Models.websearch GetResultsJSON(string term, string clientIp, string deviceId, string appVersion);
    }
}
