using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using OttaMatta.Application.Responses;
using System.ServiceModel.Activation;
using OttaMatta.Data;
using System.Threading;
using OttaMatta.Application.Security;

namespace OttaMatta.Application.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SoundIcon : ISoundIcon
    {

        [WebGet(UriTemplate = "xml/{soundId}")]
        public imagedata GetSoundIconXML(string soundId)
        {
            return GetIconData(soundId);
        }

        [WebGet(UriTemplate = "json/{soundId}", ResponseFormat = WebMessageFormat.Json)]
        public imagedata GetSoundIconJSON(string soundId)
        {
            return GetIconData(soundId);
        }


        private imagedata GetIconData(string soundId)
        {
            RequestValidation.Validate();
            
            dynamic data = DataManager.GetIconData(int.Parse(soundId));
            imagedata result = new imagedata();

            result.soundid = data.Id;

            result.extension = data.Extension;
            result.md5hash = data.Md5;
            result.datasixtyfour = data.Data;

            return result;
        }
    }
}
