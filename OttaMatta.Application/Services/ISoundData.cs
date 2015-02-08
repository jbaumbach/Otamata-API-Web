using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using OttaMatta.Application.Responses;

namespace OttaMatta.Application.Services
{
    [ServiceContract]
    interface ISoundData
    {
        [OperationContract]
        [XmlSerializerFormat]
        sound GetSoundDataXML(string soundId);

        [OperationContract]
        sound GetSoundDataJSON(string soundId);
    }
}
