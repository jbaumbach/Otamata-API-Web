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
    interface IMarkInappropriate
    {
        [OperationContract]
        [XmlSerializerFormat]
        status MarkInappropriateXML(Stream markBody);

        [OperationContract]
        status MarkInappropriateJSON(Stream markBody);
    }
}
