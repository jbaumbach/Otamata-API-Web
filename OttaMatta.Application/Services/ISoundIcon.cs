using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using OttaMatta.Application.Responses;

namespace OttaMatta.Application.Services
{
    [ServiceContract]
    interface ISoundIcon
    {
        [OperationContract]
        [XmlSerializerFormat]
        imagedata GetSoundIconXML(string soundId);

        [OperationContract]
        imagedata GetSoundIconJSON(string soundId);
    }
}
