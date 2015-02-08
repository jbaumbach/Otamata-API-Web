using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using OttaMatta.Application.Responses;
using OttaMatta.Data;
using OttaMatta.Application.Security;
using System.Threading;
using System.Xml;
using OttaMatta.Common;
using System.Web;
using OttaMatta.Data.Access;

namespace OttaMatta.Application.Services
{
    /// <summary>
    /// This is the soundsearch operation.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WebSearch : IWebSearch
    {
        [WebGet(UriTemplate = "xml?term={term}&clientIp={clientIp}&deviceId={deviceId}&appVersion={appVersion}")]
        public OttaMatta.Data.Models.websearch GetResultsXML(string term, string clientIp, string deviceId, string appVersion)
        {
            return GetSummary(term, clientIp, deviceId, appVersion);
        }

        [WebGet(UriTemplate = "json?term={term}&clientIp={clientIp}&deviceId={deviceId}&appVersion={appVersion}", ResponseFormat = WebMessageFormat.Json)]
        public OttaMatta.Data.Models.websearch GetResultsJSON(string term, string clientIp, string deviceId, string appVersion)
        {
            return GetSummary(term, clientIp, deviceId, appVersion);
        }

        private OttaMatta.Data.Models.websearch GetSummary(string term, string clientIp, string deviceId, string appVersion)
        {
            RequestValidation.Validate();

            if (Functions.IsEmptyString(term))
            {
                errordetail err = new errordetail("No search term present", System.Net.HttpStatusCode.BadRequest);
                throw new WebFaultException<errordetail>(err, err.statuscode);
            }

            if (Functions.IsEmptyString(clientIp))
            {
                clientIp = ApplicationManager.GetUserIPFromOperationContect(OperationContext.Current);
            }

            /* Google version
            OttaMatta.Data.Models.websearch result = WebSearchManager.SearchSounds(term, 
                clientIp, 
                new DataSourceFileSystem(HttpContext.Current.Server.MapPath(Config.Get(Config.CacheSearchesDirectory))),
                new ExternalSearchGoogle());
            */

            /* Yahoo version
            */ 
            OttaMatta.Data.Models.websearch result = WebSearchManager.SearchSounds(term,
                clientIp,
                new DataSourceFileSystem(HttpContext.Current.Server.MapPath(Config.Get(Config.CacheSearchesDirectory)),
                                         HttpContext.Current.Server.MapPath(Config.Get(Config.CacheWebobjectsDirectory))),
                new ExternalSearchYahoo());

            return result;
        }
    }
}
