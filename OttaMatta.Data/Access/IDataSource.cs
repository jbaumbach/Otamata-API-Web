using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OttaMatta.Data.Models;
using OttaMatta.Common;

namespace OttaMatta.Data.Access
{
    public interface IDataSource
    {
        /// <summary>
        /// List of items' MD5 hashes, maintained as search is being built
        /// </summary>
        // HashSet<string> currentSoundMd5s = new HashSet<string>();
        HashSet<string> CurrentSoundMd5s { get; }   // = new HashSet<string>();

        /// <summary>
        /// Do a web search for the passed term.  This stores the results on the server, including sound files.
        /// </summary>
        /// <param name="term">The search term.</param>
        /// <returns>The results of the search, including progress.</returns>
        websearch GetWebsearch(string term, IList<websearchsound> searchResultSounds);

        bool SetWebsearch(websearch theSearch);

        websearchsound GetSoundInSearch(string searchTerm, string soundId);
        string SetSoundInSearch(websearch search, websearchsound sound);

        bool SetWebsearchStatus(websearch theSearch, websearchstatus status);

        /// <summary>
        /// For the passed url, grab the contents.  Should only be used for text content types.
        /// </summary>
        /// <param name="url">The url</param>
        /// <param name="header">Extra header values to include, if any.</param>
        /// <param name="userAgent">The user agent to use, if any.</param>
        /// <returns>The url contents, or string.Empty if something goes wrong.</returns>
        /// <remarks>
        /// Using a 5 second timeout.
        /// </remarks>
        string GetUrlContents(string url, string header, string userAgent, Functions.LogMessageDelegate LogMessage);

    }
}
