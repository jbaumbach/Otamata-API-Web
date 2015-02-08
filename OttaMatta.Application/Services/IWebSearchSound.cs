using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using OttaMatta.Application.Responses;

namespace OttaMatta.Application.Services
{
    [ServiceContract]
    public interface IWebSearchSound
    {
        [OperationContract]
        [XmlSerializerFormat]
        OttaMatta.Data.Models.websearchsound GetWebsearchSoundXML(string term, string soundId, string deviceId, string appVersion);

        [OperationContract]
        OttaMatta.Data.Models.websearchsound GetWebsearchSoundJSON(string term, string soundId, string deviceId, string appVersion);
    }
}
