using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using OttaMatta.Application.Responses;

namespace OttaMatta.Application.Services
{
    [ServiceContract]
    interface ISounds
    {
        [OperationContract]
        [XmlSerializerFormat]
        soundssummary GetSummaryXML(string term, string order, string includeInappropriate, string page, string pageSize, string deviceId, string appVersion);

        [OperationContract]
        soundssummary GetSummaryJSON(string term, string order, string includeInappropriate, string page, string pageSize, string deviceId, string appVersion);
    }
}
