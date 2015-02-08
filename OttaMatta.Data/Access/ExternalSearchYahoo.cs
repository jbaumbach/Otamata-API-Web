using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OttaMatta.Common;
using OAuth;
using System.Net;
using System.Web;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace OttaMatta.Data.Access
{
    public class ExternalSearchYahoo : IExternalSearch
    {
        private string BuildAuthenticatedUrl(Uri uri)
        {
            //
            // Let's search for this bad boy!
            //
            string result = string.Empty;

            //
            // todo: put this info in the config file
            //
            string consumerKey = "dj0yJmk9MFBTSWVjY3Jhb2dTJmQ9WVdrOWQyRmxkMkZSTm1zbWNHbzlNek0xTmpFNE9EWXkmcz1jb25zdW1lcnNlY3JldCZ4PWI5";
            string consumerSecret = "e3d1783f97fe56b22fffa2209ca45548fcc7a478";

            string url, param;
            var oAuth = new OAuthBase();
            var nonce = oAuth.GenerateNonce();
            var timeStamp = oAuth.GenerateTimeStamp();

            var signature = oAuth.GenerateSignature(uri, consumerKey, consumerSecret, string.Empty, string.Empty, "GET", timeStamp, nonce, OAuthBase.SignatureTypes.HMACSHA1, out url, out param);

            result = string.Format("{0}?{1}&oauth_signature={2}", url, param, signature);

            return result;
        }

        public string GetSearchResults(string term, string clientIp, int resultsPageSize, int pageToGet, Functions.LogMessageDelegate LogMessage)
        {
            string result = string.Empty;

            //
            // Docs for google search:
            //
            // https://developers.google.com/web-search/docs/reference#_intro_fonje
            //
            const string baseAddr = @"http://yboss.yahooapis.com/ysearch/web";

            //
            // Note: Paging is supported in the request.  To ponder.
            // 
            const string searchModifierKeywords = "sound clips wav mp3";
            int startIndex = pageToGet * resultsPageSize;

            string yahooSearchPhrase = string.Format("{0} {1}", term, searchModifierKeywords);

            // http://yboss.yahooapis.com/ysearch/web?q=ipod

            string searchUrl = string.Format(@"{0}?q={1}&count={2}", baseAddr, yahooSearchPhrase, resultsPageSize);

            if (LogMessage != null)
            {
                LogMessage(string.Format("Searching yahoo for: \"{0}\"", searchUrl));
            }

            result = WebProcessor.GetUrlContents(BuildAuthenticatedUrl(new Uri(searchUrl)), @"referrer:http://www.otamata.com", @"OtamataSoundSearchService", LogMessage);

            return result;
        }

        public IList<dynamic> GetResultUrls(string rawJsonResults)
        {
            IEnumerable temp = null;
            IList<dynamic> result = new List<dynamic>();

            if (!Functions.IsEmptyString(rawJsonResults))
            {
                JObject jsonResults = JObject.Parse(rawJsonResults);

                temp = jsonResults["bossresponse"]["web"]["results"].Children().Select(
                    (url, index) => new
                    {
                        Url = url["url"].ToString(),
                        Index = index,
                        Domain = WebProcessor.GetDomainOfUrl(url["url"].ToString())
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
            //
            // Let's search for this bad boy!
            //
            string result = string.Empty;

            string size = "medium";
            var uri = new Uri(string.Format("http://yboss.yahooapis.com/ysearch/images?dimensions={0}&q={1}", size, HttpUtility.UrlEncode(term)));

            string searchUrl = BuildAuthenticatedUrl(uri);

            result = WebProcessor.GetUrlContents(searchUrl, @"referrer:http://www.otamata.com", @"OtamataSoundSearchService", LogMessage);

            return result;
        }


        public IList<dynamic> GetImageResultUrls(string rawJsonResults)
        {
            IEnumerable temp = null;
            IList<dynamic> result = new List<dynamic>();

            if (!Functions.IsEmptyString(rawJsonResults))
            {
                JObject jsonResults = JObject.Parse(rawJsonResults);

                temp = jsonResults["bossresponse"]["images"]["results"].Children().Select(
                    (url, index) => new
                    {
                        Url = url["url"].ToString(),
                        Thumb = url["thumbnailurl"].ToString(),
                        Index = index,
                        Source = url["refererurl"].ToString()
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
    }
}
