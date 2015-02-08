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

namespace OttaMatta.Application.Services
{
    /// <summary>
    /// This is the soundsearch operation.  It needs to be renamed.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Sounds : ISounds
    {
        [WebGet(UriTemplate = "xml?term={term}&order={order}&incappr={includeInappropriate}&page={page}&pageSize={pageSize}&deviceId={deviceId}&appVersion={appVersion}")]
        public soundssummary GetSummaryXML(string term, string order, string includeInappropriate, string page, string pageSize, string deviceId, string appVersion)
        {
            return GetSummary(term, order, includeInappropriate, page, pageSize, deviceId, appVersion);
        }

        [WebGet(UriTemplate = "json?term={term}&order={order}&incappr={includeInappropriate}&page={page}&pageSize={pageSize}&deviceId={deviceId}&appVersion={appVersion}", ResponseFormat = WebMessageFormat.Json)]
        public soundssummary GetSummaryJSON(string term, string order, string includeInappropriate, string page, string pageSize, string deviceId, string appVersion)
        {
            return GetSummary(term, order, includeInappropriate, page, pageSize, deviceId, appVersion);
        }

        private soundssummary GetSummary(string term, string order, string includeInappropriate, string page, string pageSize, string deviceId, string appVersion)
        {
            RequestValidation.Validate();
            int reqPage = Functions.ConvertInt(page, 0);
            int reqPageSize = Functions.ConvertInt(pageSize, 50);

            long totalResults = -1;

            List<dynamic> soundList = DataManager.SearchSounds(term.Left(10), Functions.ConvertInt(order, 0), Functions.ConvertInt(includeInappropriate, 0) == 1, reqPage, reqPageSize, out totalResults);
            soundssummary result = new soundssummary();

            result.sounds = (from item in soundList
                            select new sound { name = item.Name, 
                                soundid = item.Id,
                                filename = item.FileName,
                                description = item.Description, 
                                downloads = item.DownloadCount, 
                                uploadedby = item.UploadBy,
                                uploadDate = item.UploadDate.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),  // RFC 3339 date-time format
                                averagerating = item.AverageRating,
                                size = item.Size,
                                hasicon = item.HasIcon,
                                imagethumb = null }).ToList<sound>();

            result.totalresults = totalResults.ToString();

            return result;
        }
    }
}
