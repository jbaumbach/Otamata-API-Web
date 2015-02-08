using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using OttaMatta.Application.Responses;

namespace OttaMatta.Application.Services
{
    [ServiceContract]
    interface IUploadSound
    {
        [OperationContract]
        [XmlSerializerFormat]
        sound UploadSoundXML(Stream uploadSoundBody);

        [OperationContract]
        sound UploadSoundJSON(Stream uploadSoundBody);
    }

}
