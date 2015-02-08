using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace OttaMatta.Common
{
    public class WebProcessor
    {
        /// <summary>
        /// For the passed url, get the domain
        /// </summary>
        /// <param name="theUrl">The url to search</param>
        /// <returns>The domain</returns>
        public static string GetDomainOfUrl(string theUrl)
        {
            string result = string.Empty;

            Regex reg = new Regex(@"//(.*?($|/))", RegexOptions.Compiled);
            Match fileNameMatch = reg.Match(theUrl);

            if (fileNameMatch.Success)
            {
                result = fileNameMatch.Groups[1].ToString();
            }

            return result;
        }

        /// <summary>
        /// For the passed url to a file, return just the filename or any ending characters after the last slash
        /// </summary>
        /// <param name="theUrl">The url</param>
        /// <returns>The filename</returns>
        public static string GetFileNameFromUrl(string theUrl)
        {
            string result = string.Empty;

            Regex reg = new Regex(@".*/(.*$)", RegexOptions.Compiled);
            Match fileNameMatch = reg.Match(theUrl);

            if (fileNameMatch.Success)
            {
                result = fileNameMatch.Groups[1].ToString();
            }

            return result;
        }

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
        public static string GetUrlContents(string url, string header, string userAgent, Functions.LogMessageDelegate LogMessage)
        {
            string result = null;

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Timeout = 5000;     // Only wait 5 seconds for some of these lame-ass servers

            if (!Functions.IsEmptyString(header))
            {
                request.Headers.Add(header);
            }

            if (!Functions.IsEmptyString(userAgent))
            {
                request.UserAgent = userAgent;
            }

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                //
                // todo: handle things like:         <meta HTTP-EQUIV="refresh" CONTENT="0; URL=http://www.thesoundarchive.com/beavis-and-butthead.asp">
                //

                if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    //
                    // According to the .NET docs, the request should automatically handle this?  See mouse-over tool tip for "HttpStatusCode.Redirect" above
                    //
                    LogMessage(string.Format("Received redirect - prolly not gonna find anything on this page... {0}", response.StatusCode));
                }

                Stream responseStream = response.GetResponseStream();
                result = new StreamReader(responseStream).ReadToEnd();
            }
            catch (Exception ex)
            {
                // Crud.
                LogMessage(string.Format("GetUrlContents() Exception! = \"{0}\" at url: \"{1}\"", ex.Message, url));
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;

        }

        /// <summary>
        /// Build a url to a potential sound 
        /// </summary>
        /// <param name="theUrl">The url where the partial url to the sound was found</param>
        /// <param name="possibleRelativePath">The partial url</param>
        /// <returns>The url</returns>
        /// <remarks>
        /// For example, if the url was "http://otamata.com/sounds/anchorman.htm" and the partial was "player/scotch.wav", you'd 
        /// get "http://otamata.com/sounds/player/scotch.wav".
        /// </remarks>
        public static string GetUrlForObject(string theUrl, string possibleRelativePath)
        {
            Regex currentFullUrlToPageRegex = new Regex(@"(http(s)?://.*/)", RegexOptions.Compiled);
            Regex currentDomainRegex = new Regex(@"(http(s)?://.*?/)", RegexOptions.Compiled);

            Match currentFullUrlMatch = currentFullUrlToPageRegex.Match(theUrl);
            string currentFullUrlToPage = currentFullUrlMatch.Success ? currentFullUrlMatch.Groups[1].ToString() : string.Empty;

            Match currentDomainMatch = currentDomainRegex.Match(theUrl);
            string currentDomain = currentDomainMatch.Success ? currentDomainMatch.Groups[1].ToString() : theUrl;

            bool possibleRelativePathHasDomain = possibleRelativePath.StartsWith("http://");        // (currentDomain);

            if (possibleRelativePathHasDomain)
            {
                //
                // This is an absolute path
                //
                return possibleRelativePath;
            }
            else
            {
                if (possibleRelativePath.StartsWith("/"))
                {
                    //
                    // Path from root
                    //
                    return Functions.CombineUrlElements(currentDomain.TrimTrailingCharacterIfExists("/"), possibleRelativePath);
                }
                else
                {
                    //
                    // This one is tricky.  Not sure if the end of the url is a directory (w/hidden "index.html"), a friendly url to a file, or an actual file.  Hmmmm...
                    //
                    if (currentFullUrlToPage.Equals(currentDomain) && !currentFullUrlToPage.Equals(theUrl))
                    {
                        //
                        // The regex can't cope with urls w/o a trailing slash, but we think it's a dir, so fudge it a bit
                        //
                        currentFullUrlToPage = theUrl;
                    }

                    //
                    // Another regex bug, domains w/o trailing slash no workie
                    //
                    if (Functions.IsEmptyString(currentFullUrlToPage))
                    {
                        currentFullUrlToPage = theUrl;
                    }

                    return Functions.CombineUrlElements(currentFullUrlToPage, possibleRelativePath);
                }
            }

        }


    }
}
