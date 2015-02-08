using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using OttaMatta.Application.Responses;
using System.IO;

namespace OttaMatta.Application.Services
{
    [ServiceContract]
    interface IRecordPurchase
    {
        [OperationContract]
        [XmlSerializerFormat]
        status RecordStorePurchaseXML(Stream storePurchaseBody);

        [OperationContract]
        status RecordStorePurchaseJSON(Stream storePurchaseBody);
    }
}
