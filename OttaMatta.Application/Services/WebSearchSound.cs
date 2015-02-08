using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using OttaMatta.Application.Responses;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using OttaMatta.Application.Security;
using OttaMatta.Common;
using OttaMatta.Data;
using OttaMatta.Data.Access;
using System.Web;

namespace OttaMatta.Application.Services
{
    /// <summary>
    /// Grab a sound search result from the server
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WebSearchSound : IWebSearchSound
    {
        [WebGet(UriTemplate = "xml?term={term}&soundId={soundId}&deviceId={deviceId}&appVersion={appVersion}")]
        public Data.Models.websearchsound GetWebsearchSoundXML(string term, string soundId, string deviceId, string appVersion)
        {
            return GetSound(term, soundId, deviceId, appVersion);
        }

        [WebGet(UriTemplate = "json?term={term}&soundId={soundId}&deviceId={deviceId}&appVersion={appVersion}", ResponseFormat = WebMessageFormat.Json)]
        public Data.Models.websearchsound GetWebsearchSoundJSON(string term, string soundId, string deviceId, string appVersion)
        {
            return GetSound(term, soundId, deviceId, appVersion);
        }

        private OttaMatta.Data.Models.websearchsound GetSound(string term, string soundId, string deviceId, string appVersion)
        {
            RequestValidation.Validate();

            if (Functions.IsEmptyString(soundId))
            {
                errordetail err = new errordetail("No sound id passed", System.Net.HttpStatusCode.BadRequest);
                throw new WebFaultException<errordetail>(err, err.statuscode);
            }

            OttaMatta.Data.Models.websearchsound result = WebSearchManager.GetSound(term, soundId, 
                new DataSourceFileSystem(HttpContext.Current.Server.MapPath(Config.Get(Config.CacheSearchesDirectory)),
                                        HttpContext.Current.Server.MapPath(Config.Get(Config.CacheWebobjectsDirectory))));

            if (result == null)
            {
                errordetail err = new errordetail("Sound id not found", System.Net.HttpStatusCode.BadRequest);
                throw new WebFaultException<errordetail>(err, err.statuscode);
            }
            else
            {
                return result;
            }

        }
    }
}
