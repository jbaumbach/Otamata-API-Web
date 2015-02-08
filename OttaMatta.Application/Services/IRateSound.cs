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
    interface IRateSound
    {
        [OperationContract]
        [XmlSerializerFormat]
        status PostSoundRatingXML(Stream postBody);
 
        [OperationContract]
        status PostSoundRatingJSON(Stream postBody);
    }
}
