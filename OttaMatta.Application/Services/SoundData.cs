using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using OttaMatta.Application.Responses;
using System.ServiceModel.Activation;
using OttaMatta.Data;
using OttaMatta.Application.Security;

namespace OttaMatta.Application.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SoundData : ISoundData
    {

        [WebGet(UriTemplate = "xml/{soundId}")]
        public sound GetSoundDataXML(string soundId)
        {
            return GetSoundData(soundId);
        }

        [WebGet(UriTemplate = "json/{soundId}", ResponseFormat = WebMessageFormat.Json)]
        public sound GetSoundDataJSON(string soundId)
        {
            return GetSoundData(soundId);
        }


        private sound GetSoundData(string soundId)
        {
            RequestValidation.Validate();

            dynamic data = DataManager.GetSoundData(int.Parse(soundId));
            sound result = new sound();

            result.soundid = data.Id;
            result.filename = data.FileName;
            result.md5hash = data.Md5;
            result.datasixtyfour = data.Data;
            result.size = data.Size;

            return result;
        }
    }
}
