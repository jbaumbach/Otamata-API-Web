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
    interface IPurchaseSound
    {
        [OperationContract]
        [XmlSerializerFormat]
        status PurchaseSoundXML(Stream purchaseBody);

        [OperationContract]
        status PurchaseSoundJSON(Stream purchaseBody);
    }
}
