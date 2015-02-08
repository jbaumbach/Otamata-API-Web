using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OttaMatta.Common;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace OttaMatta.Data.Access
{
    public class ExternalSearchGoogle : IExternalSearch
    {
        /// <summary>
        /// Get the search results JSON data from Google
        /// </summary>
        /// <param name="term">The plain search term.  Some keywords are added to get better search results.</param>
        /// <param name="clientIp">The client IP address</param>
        /// <param name="resultsPageSize">The number of results per page (8 recommended)</param>
        /// <param name="pageToGet">The zero-based index of the page to get</param>
        /// <returns>The raw JSON from Google.</returns>
        public string GetSearchResults(string term, string clientIp, int resultsPageSize, int pageToGet, Functions.LogMessageDelegate LogMessage)
        {
            string result = string.Empty;

            //
            // Docs for google search:
            //
            // https://developers.google.com/web-search/docs/reference#_intro_fonje
            //
            const string baseAddr = @"https://ajax.googleapis.com/ajax/services/search/web";

            //
            // Note: Paging is supported in the request.  To ponder.
            // 
            const string searchModifierKeywords = "sound clips wav mp3";
            int startIndex = pageToGet * resultsPageSize;

            string googleSearchPhrase = string.Format("{0} {1}", term, searchModifierKeywords);

            string searchUrl = string.Format(@"{0}?q={1}&v=1.0&userip={2}&start={3}&rsz={4}", baseAddr, googleSearchPhrase, clientIp, startIndex, resultsPageSize);

            if (LogMessage != null)
            {
                LogMessage(string.Format("Searching google for: \"{0}\"", searchUrl));
            }

            result = WebProcessor.GetUrlContents(searchUrl, @"referrer:http://www.otamata.com", @"OtamataSoundSearchService", LogMessage);

            return result;
        }

        /// <summary>
        /// From a JSON results string (must be from GetSearchResults()), grab all the urls
        /// </summary>
        /// <param name="rawJsonResults">The JSON string</param>
        /// <returns>A list of dynamic objects</returns>
        /// <remarks>
        ///     Url = url["unescapedUrl"].ToString(),
        ///     Index = index,
        ///     Domain = WebProcessor.GetDomainOfUrl(url["unescapedUrl"].ToString())
        /// </remarks>
        public IList<dynamic> GetResultUrls(string rawJsonResults)
        {
            IEnumerable temp = null;
            IList<dynamic> result = new List<dynamic>();

            if (!Functions.IsEmptyString(rawJsonResults))
            {
                JObject jsonResults = JObject.Parse(rawJsonResults);

                temp = jsonResults["responseData"]["results"].Children().Select(
                    (url, index) => new
                    {
                        Url = url["unescapedUrl"].ToString(),
                        Index = index,
                        Domain = WebProcessor.GetDomainOfUrl(url["unescapedUrl"].ToString())
                    });

                //
                // "temp" is some crazy type of variable.  IEnumerable doesn't have a "Count" property, so it's pretty much useless.
                // Todo: figure more of this Linq stuff out.  It seems cool, but so hard to use.
                //

                foreach (object item in temp)
                {
                    result.Add(item);
                }

            }

            return result;
        }


        public string GetImageSearchResults(string term, string clientIp, int resultsPageSize, int pageToGet, Functions.LogMessageDelegate LogMessage)
        {
            throw new NotImplementedException();
        }


        public IList<dynamic> GetImageResultUrls(string rawJsonResults)
        {
            throw new NotImplementedException();
        }
    }
}
