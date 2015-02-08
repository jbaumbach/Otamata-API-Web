using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OttaMatta.Common;

namespace OttaMatta.Data.Access
{
    public interface IExternalSearch
    {
        /// <summary>
        /// Get the search results JSON data from Google
        /// </summary>
        /// <param name="term">The plain search term.  Some keywords are added to get better search results.</param>
        /// <param name="clientIp">The client IP address</param>
        /// <param name="resultsPageSize">The number of results per page (8 recommended)</param>
        /// <param name="pageToGet">The zero-based index of the page to get</param>
        /// <returns>The raw JSON from Google.</returns>
        string GetSearchResults(string term, string clientIp, int resultsPageSize, int pageToGet, Functions.LogMessageDelegate LogMessage);

        /// <summary>
        /// From a JSON results string (must be from GetSearchResults()), grab all the urls
        /// </summary>
        /// <param name="rawJsonResults">The JSON string</param>
        /// <returns>A list of dynamic objects</returns>
        IList<dynamic> GetResultUrls(string rawJsonResults);

        /// <summary>
        /// From a JSON results string (must be from GetSearchResults()), grab all the urls
        /// </summary>
        /// <param name="rawJsonResults">The JSON string</param>
        /// <returns>A list of dynamic objects</returns>
        /// <remarks>
        ///     Url = url["unescapedUrl"].ToString(),
        ///     Thumb = url["thumbnailurl"].ToString(),
        ///     Index = index,
        ///     Domain = WebProcessor.GetDomainOfUrl(url["unescapedUrl"].ToString())
        /// </remarks>
        IList<dynamic> GetImageResultUrls(string rawJsonResults);

        string GetImageSearchResults(string term, string clientIp, int resultsPageSize, int pageToGet, Functions.LogMessageDelegate LogMessage);
    }
}
