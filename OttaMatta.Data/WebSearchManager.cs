//
// By default, keep this line.  For debugging, comment it out.
//
#define MULTITHREADED

using System;
using System.Collections;
// using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using OttaMatta.Common;
using OttaMatta.Data.Access;
using OttaMatta.Data.Models;

namespace OttaMatta.Data
{
    /// <summary>
    /// Extensions to various classes
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Extension function that is supposed to be in System.Linq.Enumerable, but somehow it's not???
        /// </summary>
        /// <param name="list">The list</param>
        /// <returns>The first object or null</returns>
        public static dynamic FirstOrDefault(this IEnumerable list)
        {
            dynamic result = null;
            int loop = 0;
            foreach (dynamic obj in list)
            {
                if (loop++ == 0)
                {
                    result = obj;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Class to search the web for sounds 
    /// </summary>
    public class WebSearchManager
    {
        /// <summary>
        /// Object used to synchronize multithreaded operations
        /// </summary>
        private static string _getUrlsLockingVar = @"MyLock";

        private static readonly int _maxDepthToFollow = 1;

        private static HashSet<string> unprocessableDomains = new HashSet<string>();

        //private static HashSet<string> currentSoundMd5s = new HashSet<string>();

        /// <summary>
        /// Log a message to a log file if enabled, otherwise to Debug.Writeline()
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <remarks>Todo: move this to a common location, but not dependent on anything else.</remarks>
        private static void LogMessage(string message)
        {
            if (Functions.ConvertBool(Config.Get(Config.EnableLogging), false))
            {
                LogFile logger = new LogFile();
                logger.LogFileName = "otamatadebuglog";

                string logfilePath = Config.Get(Config.PathToLogfile);

                if (Functions.IsEmptyString(logfilePath))
                {
                    logger.LogFilePath = @"c:\";
                }
                else
                {
                    logger.LogFilePath = logfilePath;
                }

                logger.Log(message);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(message);
            }
        }

        /// <summary>
        /// Return a randomly generated user agent string
        /// </summary>
        /// <returns>The user agent string</returns>
        public static string GetUserAgent()
        {
            //
            // See list here:
            // http://www.useragentstring.com/pages/Internet%20Explorer/
            //

            List<string> userAgents = new List<string>();
            
            userAgents.Add(@"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_3) AppleWebKit/534.55.3 (KHTML, like Gecko) Version/5.1.3 Safari/534.53.10");
            userAgents.Add(@"Mozilla/5.0 (X11; Ubuntu; Linux i686; rv:14.0) Gecko/20100101 Firefox/14.0.1");
            userAgents.Add(@"Mozilla/5.0 (Windows NT 6.0; rv:14.0) Gecko/20100101 Firefox/14.0.1");
            userAgents.Add(@"Mozilla/5.0 (Windows NT 6.1; rv:12.0) Gecko/20120403211507 Firefox/12.0");
            userAgents.Add(@"Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.3) Gecko/20100401 Mozilla/5.0 (X11; U; Linux i686; it-IT; rv:1.9.0.2) Gecko/2008092313 Ubuntu/9.25 (jaunty) Firefox/3.8");
            userAgents.Add(@"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.0; Trident/5.0; chromeframe/11.0.696.57)");
            userAgents.Add(@"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; Media Center PC 6.0; InfoPath.2; MS-RTC LM 8)");

            int rnd = new Random(DateTime.Now.Millisecond).Next(userAgents.Count - 1);

            return userAgents[rnd];
        }

        /// <summary>
        /// For the passed page, grab all potential partial sound urls using the passed regex.
        /// </summary>
        /// <param name="mainList">The list of urls.  The results will be added to this.</param>
        /// <param name="pageContent">The page</param>
        /// <param name="theRegex">The regex to use</param>
        /// <param name="timeoutInMS">The maxiumum time to wait for results</param>
        private static bool GetSoundLinksOnPageByRegex(IList<string> mainList, string pageContent, Regex theRegex, int timeoutInMS)
        {
            //
            // Some pages cause a "Catastrophic Backtracking" Regex.  Let's protect the server by limiting the runtime. 
            //

            /* Infinite(ish?) loop here:
                        About to search for sounds on page: "https://play.google.com/store/apps/details?id=com.mobile.anchor&hl=en"
                        [7/4/2012 12:59:58 PM] GetSoundLinksOnPage()/doubleQuote
                        -->[7/4/2012 12:59:58 PM] GetSoundLinksOnPageByRegex()/startingMatches
                        -->[7/4/2012 12:59:59 PM] GetSoundLinksOnPageByRegex()/startingLoop
             */

            Thread regexThread = new Thread(dummyDelegate => GetSoundLinksOnPageByRegex(mainList, pageContent, theRegex));
            regexThread.Start();

            bool complete = regexThread.Join(timeoutInMS);

            if (!complete)
            {
                LogMessage("Oops!  Have to abort search on page!");
                regexThread.Abort();
            }

            return complete;
        }

        /// <summary>
        /// For the passed page, grab all potential partial sound urls using the passed regex.
        /// </summary>
        /// <param name="mainList">The list of urls.  The results will be added to this.</param>
        /// <param name="pageContent">The page</param>
        /// <param name="theRegex">The regex to use</param>
        private static void GetSoundLinksOnPageByRegex(IList<string> mainList, string pageContent, Regex theRegex)
        {
            if (pageContent != null)
            {
                LogMessage(string.Format("-->[{0}] GetSoundLinksOnPageByRegex()/startingMatches, contentlength={1}", DateTime.Now, pageContent.Length));

                MatchCollection matches = theRegex.Matches(pageContent);

                LogMessage(string.Format("-->[{0}] GetSoundLinksOnPageByRegex()/startingLoop, matches count {1}", DateTime.Now, matches.Count));
                
                foreach (Match match in matches)
                {
                    string url = HttpUtility.HtmlDecode(match.Groups[1].ToString());
                    LogMessage(string.Format("possible sound at: {0}", url));

                    //
                    // There are no plans to multithread this part of the app, but let's future proof this collection just in case.
                    //
                    lock (mainList)
                    {
                        mainList.Add(url);
                    }
                }

                LogMessage(string.Format("-->[{0}] GetSoundLinksOnPageByRegex()/loopComplete", DateTime.Now));
            }
        }

        /// <summary>
        /// For the passed page, get all potential partial sound urls
        /// </summary>
        /// <param name="pageContent">The page</param>
        /// <returns>The list of potential urls</returns>
        public static IList<string> GetSoundLinksOnPage(string pageContent, ref bool wasAborted)
        {
            IList<string> result = new List<string>();

            //
            // Potential enhancement: don't search for a file extension.  Just grab ALL objects from the server, and look at the MIME type.
            // It'll be slower, but potentially more complete.  A problem will be pages that use javascript or flash to protect the files.  Oh well.
            //

            //
            // Limit the amount of time the function is allowed to run to find links.  If it's longer than this, it's just not worth it.
            //
            const int maxExecutionTimeMS = 3000;

            List<Regex> regexes = new List<Regex>()
            {
                new Regex(".*\"(.*?\\.(mp3|wav))\".*", RegexOptions.Compiled),
                new Regex(@"href=(.*?\.(mp3|wav))>", RegexOptions.Compiled),        // for wavcentral.com that doesn't have any quotes in the HTML!  But lots of good wavs.
                //new Regex(@"Sound:.+?href='(.+?movie-sounds.net.+?/[0-9]+/)'", RegexOptions.Compiled), // for movie-sounds.net that has secondary links, but good wavs.  The second level pages don't work.  to fix.
                new Regex(@".*'(.*?\.(mp3|wav))'.*", RegexOptions.Compiled),
            };

            bool res = true;
            foreach (Regex reg in regexes)
            {
                res = GetSoundLinksOnPageByRegex(result, pageContent, reg, maxExecutionTimeMS);

                if (!res || result.Count > 0)
                {
                    break;
                }
            }

            //
            // If either was aborted, that's bad
            //
            wasAborted = !res;

            return result;
        }

        /// <summary>
        /// For a mime type, find the extension
        /// </summary>
        /// <param name="mimeType">The mime type</param>
        /// <returns>The extension</returns>
        public static string GetExtensionFromMimeType(string mimeType)
        {
            string result = mimeType.Equals("audio/wav") |
                mimeType.Equals("audio/x-wav") |
                mimeType.Equals("audio/wave") |
                mimeType.Equals("audio/x-pn-wav")
                ? "wav" : 

                mimeType.Equals("audio/mpeg") | 
                mimeType.Equals("audio/x-mpeg") | 
                mimeType.Equals("audio/mpeg3") | 
                mimeType.Equals("audio/x-mpeg3") | 
                mimeType.Equals("audio/mpg") | 
                mimeType.Equals("audio/x-mpg") | 
                mimeType.Equals("audio/x-mpegaudio") | 
                mimeType.Equals("audio/mp3") | 
                mimeType.Equals("audio/x-mp3") 
                ? "mp3" : 
                
                string.Empty;

            return result;
        }

        /// <summary>
        /// Grab a potential websoundsearch object at a url
        /// </summary>
        /// <param name="url">The url of the object</param>
        /// <param name="header">Additional header to include, if any</param>
        /// <param name="userAgent">The user agent to use, if any</param>
        /// <returns>A websearchsound object, with the properties populated if it's really a sound.</returns>
        private static websearchsound GetWebObjectAtUrl(string url, string header, string userAgent)
        {
            websearchsound result = new websearchsound();   

            const long MAX_SOUND_SIZE_BYTES = 1024 * 1000;  // Let's cap at 1 MB

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Timeout = 5000;                         // If it takes longer than 5 seconds to respond, we're in trouble.  Let's bail

            if (!Functions.IsEmptyString(header))
            {
                request.Headers.Add(header);
            }

            if (!Functions.IsEmptyString(userAgent))
            {
                request.UserAgent = userAgent;
            }

            HttpWebResponse response = null;
            FileStream fileStream = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                string foundContentType = response.ContentType.ToLower(); 
                long responseSize = response.ContentLength;

                if (responseSize > MAX_SOUND_SIZE_BYTES)
                {
                    LogMessage(string.Format("Won't download, too big: {0} (limit is {1})", responseSize, MAX_SOUND_SIZE_BYTES));
                }
                else
                {

                    result.contenttype = foundContentType;
                    string outputExt = GetExtensionFromMimeType(foundContentType);
                    string fileName = WebProcessor.GetFileNameFromUrl(url);

                    result.issound = outputExt != string.Empty;
                    result.filename = fileName;
                    result.extension = outputExt;

                    //
                    // For debugging - setup variables to assist writing out the file to the cache dir
                    //
                    /*
                    string csd = Config.CacheSearchesDirectory;
                    string cgcsd = Config.Get(csd);
                    HttpServerUtility hsu = HttpContext.Current.Server; // Note: for multithreading - this will be NULL.  Need to pass in a value.
                    string outputPath = hsu.MapPath(cgcsd);
                    string outputFile = Functions.CombineElementsWithDelimiter("\\", outputPath, string.Format("{0}.{1}", fileName.ReplaceAllNonAlphaNumericCharsInString(), outputExt));
                    */

                    if (result.issound)
                    {
                        /* To get raw bytes: 
                        */
                        var memStream = new MemoryStream();

                        try
                        {
                            // not sure if this will copy all the bytes: response.GetResponseStream().CopyTo(memStream);
                            Functions.CopyStream(response.GetResponseStream(), memStream);
                            result.soundbytes = memStream.ToArray();
                            long memStreamSizeBytes = memStream.Length;
                            result.size = memStreamSizeBytes;
                        }
                        catch (Exception ex)
                        {
                            LogMessage(string.Format("Exception getting sound bytes for file \"{0}\", was: {1}", result.filename, ex.Message));
                        }
                        finally
                        {
                            if (memStream != null)
                            {
                                memStream.Close();
                            }
                        }
                        //
                        // Don't clog log up with successes, we're worried about the errors
                        //
                        // LogMessage(string.Format("Boom - snagged file \"{0}\" of size {1}", fileName, memStreamSizeBytes));
                        /*
                        const bool writeDebuggingFile = false;

                        if (writeDebuggingFile && !File.Exists(outputFile))
                        {
                            // Debugging - write to disk
                            fileStream = new FileStream(outputFile, FileMode.Create);
                            response.GetResponseStream().Position = 0;
                            Functions.CopyStream(response.GetResponseStream(), fileStream);
                        }
                        */
                    }
                    else
                    {
                        LogMessage(string.Format("Object at \"{0}\" not a sound, has mime type of \"{1}\"", url, foundContentType));
                    }
                }
            }
            catch (Exception ex)
            {
                // Crud.
                LogMessage(string.Format("Error doing stuff with file \"{0}\", was: {1}", url, ex.Message));
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }

            return result;

        }

        /// <summary>
        /// Check if we already have this url in our list of d/l'd sounds.  Automatically adds the url to the list.
        /// </summary>
        /// <param name="alreadySearchedUrls">The list</param>
        /// <param name="potentialNewUrl">The url to test and add</param>
        /// <returns>True if it's new and should be downloaded</returns>
        public static bool IsNewSoundToGrab(HashSet<string> alreadySearchedUrls, string potentialNewUrl)
        {
            string potenalNewUrlLower = potentialNewUrl.ToLower();
            bool result = false;

            lock (alreadySearchedUrls)
            {
                result = !alreadySearchedUrls.Contains(potenalNewUrlLower);

                if (result)
                {
                    alreadySearchedUrls.Add(potentialNewUrl);
                }
            }

            return result;
        }

        /// <summary>
        /// Tests if we already have this sound's hash.  Adds it to the current collection if not.
        /// </summary>
        /// <param name="currentSoundMd5s">The collection of sound hashes.</param>
        /// <param name="newMd5">The sound hash to test.</param>
        /// <returns>True if we already have it.</returns>
        public static bool HaveMd5ForSound(HashSet<string> currentSoundMd5s, string newMd5)
        {
            bool result = false;

            lock (currentSoundMd5s)
            {
                result = currentSoundMd5s.Contains(newMd5);

                if (!result)
                {
                    currentSoundMd5s.Add(newMd5);
                }
            }

            return result;
        }

        private static void WriteStatus()
        {
        }

        /// <summary>
        /// Get all the sounds in the list of urls from the passed IDataSource and add the sounds to the passed websearch
        /// </summary>
        /// <param name="urls">The list of urls</param>
        /// <param name="dataSource">The datasource to use to look for sounds</param>
        /// <param name="currentSearch">The current search</param>
        private static void GetSoundsOnPages(IList<dynamic> urls, IDataSource dataSource, websearch currentSearch, IList<websearchsound> searchResultList, Functions.LogMessageDelegate LogMessage)
        {
            GetSoundsOnPages(urls, dataSource, currentSearch, searchResultList, LogMessage, _maxDepthToFollow);
        }

        /// <summary>
        /// Get all the sounds in the list of urls from the passed IDataSource and add the sounds to the passed websearch
        /// </summary>
        /// <param name="urls">The list of urls</param>
        /// <param name="dataSource">The datasource to use to look for sounds</param>
        /// <param name="currentSearch">The current search</param>
        private static void GetSoundsOnPages(IList<dynamic> urls, IDataSource dataSource, websearch currentSearch, IList<websearchsound> searchResultList, Functions.LogMessageDelegate LogMessage, int maxDepthToFollow)
        {
            //const int MAX_URLS_TO_SEARCH = 20;
            const int MAX_SOUNDS_PER_URL = 150;
            //int urlsProcessed = 0;
            HashSet<string> urlsOfObjectsSearched = new HashSet<string>();

            //
            // Multithreading here for requesting the pages works pretty well speed-wise.  Unfortunately, the regexes bog down the
            // server so badly that it becomes unresponsive for other users.  So, don't do parallel on this outside loop.  
            //
            // However, once the first page is processed, the sounds are webrequested asynchronously.  So, the next page will
            // start being processed while the first page's sounds are still being downloaded.  This works quite well, and
            // the performance is just about the same.  So, let's stick with that.
            // 

            foreach (dynamic url in urls)
            {
                string theUrl = url.Url;
                string domain = WebProcessor.GetDomainOfUrl(theUrl);

                if (unprocessableDomains.Contains(domain))
                {
                    LogMessage(string.Format("Skipping crappy domain: {0}", domain));
                }
                else
                {
                    LogMessage(string.Format("About to search for sounds on page: \"{0}\"", theUrl));

                    // string pageContent = WebProcessor.GetUrlContents(theUrl, null, null, LogMessage);

                    //
                    // todo: test this, make sure it works
                    //
                    string pageContent = dataSource.GetUrlContents(theUrl, null, GetUserAgent(), LogMessage);

                    bool wasAborted = false;

                    //
                    // todo: combine sound links func with above function
                    //
                    IList<string> linksOnPage = GetSoundLinksOnPage(pageContent, ref wasAborted);

                    //
                    // For generating test case files, set breakpoint on if (wasAborted) below with condition:
                    // 
                    // maxDepthToFollow == 1
                    //

                    if (wasAborted)
                    {
                        LogMessage(string.Format("Had to abort link search on domain: {0}", domain));

                        lock (unprocessableDomains)
                        {
                            unprocessableDomains.Add(domain);
                        }
                    }

                    LogMessage(string.Format("Found {0} links on \"{1}\"", linksOnPage.Count, theUrl));


#if MULTITHREADED
                    Parallel.ForEach<string>(linksOnPage.Take(MAX_SOUNDS_PER_URL), partialLink =>       // <=-- normal operation - multithreaded
#else
                    foreach (string partialLink in linksOnPage.Take(MAX_SOUNDS_PER_URL))     // <=-- for debugging stuff, it's easier when not multithreaded
#endif
                    {
                        string soundLink = WebProcessor.GetUrlForObject(theUrl, partialLink);

                        LogMessage(string.Format("About to grab a potential sound here: \"{0}\"", soundLink));

                        if (!unprocessableDomains.Contains(domain) && IsNewSoundToGrab(urlsOfObjectsSearched, soundLink))
                        {
                            websearchsound receivedObject = GetWebObjectAtUrl(soundLink, null, null);

                            //
                            // enhanced search: if not a sound and is text/html and response code is 200, search for sounds on THAT page
                            //

                            if (receivedObject.issound)
                            {
                                receivedObject.sourceurl = theUrl;
                                receivedObject.sourceDomain = domain;
                                receivedObject.searchResultOrder = url.Index;

                                //
                                // Check for dups
                                //
                                string md5Hash = Functions.GetMd5Hash(receivedObject.soundbytes);

                                if (!HaveMd5ForSound(dataSource.CurrentSoundMd5s, md5Hash))
                                {
                                    dataSource.SetSoundInSearch(currentSearch, receivedObject);

                                    //
                                    // Performance optimization: we're not going to return the sound data itself with the search
                                    // so let's free up the mem here
                                    //
                                    receivedObject.soundbytes = null;

                                    searchResultList.Add(receivedObject);
                                }
                                else
                                {
                                    LogMessage("Not adding sound - already in collection");
                                }
                            }
                            else if (receivedObject.contenttype.ToLower().StartsWith("text/html"))
                            {
                                //
                                // We have another HTML page.  Check that too?
                                //
                                if (maxDepthToFollow > 0)
                                {
                                    LogMessage(string.Format("Going to drill down in this page - we're at max level: {0}", maxDepthToFollow));
                                    GetSoundsOnPages(new List<dynamic>() { new { Url = soundLink, Index = url.Index } }, dataSource, currentSearch, searchResultList, LogMessage,  maxDepthToFollow - 1);
                                }
                                else
                                {
                                    LogMessage(string.Format("No more drilling down, we're as low as we can go"));

                                }
                            }
                        }
                        else
                        {
                            LogMessage("Won't process: already had sound from that url, or the domain is unprocessable!");
                        }
#if MULTITHREADED
                    });
#else
                    }
#endif
                }

                //
                // Only record the top level urls we searched, or the progress will be all messed up.
                //
                if (maxDepthToFollow == _maxDepthToFollow)
                {
                    WriteSearchStatus(dataSource, currentSearch, LogMessage);
                }
                //});
            }
        }

        /// <summary>
        /// Write out the current search status (thread safe)
        /// </summary>
        /// <param name="dataSource">The current datasource</param>
        /// <param name="currentSearch">The current search object</param>
        /// <param name="LogMessage">The logging messsage delegate</param>
        /// <param name="maxDepthToFollow">The current max depth to follow.  Pass _maxDepthToFollow to ensure that status is written.</param>
        private static void WriteSearchStatus(IDataSource dataSource, websearch currentSearch, Functions.LogMessageDelegate LogMessage) // , int maxDepthToFollow)
        {
            //
            // Don't want multiple threads writing the status at the same time.  (Note: this part isn't multithreaded, but it may be in the future.)
            //
            lock (_getUrlsLockingVar)
            {
                    // urlsProcessed++;
                    currentSearch.status.urlsSearched++;

                    if (currentSearch.status.urlsTotal > 0)
                    {
                        currentSearch.status.percentcomplete = (float)currentSearch.status.urlsSearched / (float)currentSearch.status.urlsTotal;
                    }
                    else
                    {
                        currentSearch.status.percentcomplete = 100.0f;
                    }

                    currentSearch.status.itemsfound = dataSource.CurrentSoundMd5s.Count();

                    if (currentSearch.status.urlsSearched == currentSearch.status.urlsTotal)
                    {
                        currentSearch.status.isdone = 1;
                    }

                    LogMessage(string.Format("Writing status of: {0}", currentSearch.status.percentcomplete.ToString("#0.##%")));

                    dataSource.SetWebsearchStatus(currentSearch, currentSearch.status);
            }
        }

        public static void PrepareForHttpTransfer(ref websearchsound theSound)
        {
            //
            // Process the file for transferring across http
            //
            theSound.datasixtyfour = Convert.ToBase64String(theSound.soundbytes);
            theSound.md5hash = Functions.GetMd5Hash(theSound.soundbytes);
            theSound.soundbytes = null;
        }

        public static IList<resultsite> GetSortedSitesAndSounds(IList<websearchsound> searchResultList)
        {
            IList<resultsite> result = new List<resultsite>();

            //
            // Sort the results. 
            //
            var soundsSortedByIndex = from s in searchResultList
                                      orderby s.searchResultOrder, s.filename
                                      select s;

            int currentSoundIndex = -1;
            resultsite site = null;
            foreach (websearchsound sound in soundsSortedByIndex)
            {
                if (sound.searchResultOrder != currentSoundIndex)
                {
                    currentSoundIndex = sound.searchResultOrder;

                    if (site != null)
                    {
                        result.Add(site);
                    }

                    site = new resultsite();
                    site.sourcedomain = string.Format("Group {0}", result.Count + 1);       // 2012/7/24 JB: not saving domain     // sound.sourceDomain;
                }

                site.sounds.Add(new resultitem() { itemid = sound.soundid, sourceurl = null, filename = sound.filename, size = sound.size }); // sound.sourceurl
            }

            if (site != null)
            {
                result.Add(site);
            }

            return result;
        }

        /// <summary>
        /// Kick off the search.  All the result sounds will be stored in the datasource.
        /// </summary>
        /// <param name="term">The search term, used to reference this search</param>
        /// <param name="clientIp">The client IP, required by some search results providers (I'm looking at you, Google)</param>
        /// <param name="dataSource">The datasource to use to store results</param>
        public static void DoWebsearch(string term, string clientIp, IDataSource dataSource, IExternalSearch extSearch)
        {
            const int searchEngineResultsPageSize = 10;
            const int resultsPagesToGet = 1;

            websearch result = new websearch();

            //
            // Values used by various functions through the search process.
            //
            result.status.percentcomplete = 0.0f;
            result.status.searchterm = term;
            result.status.urlsSearched = 0;
            result.status.urlsTotal = searchEngineResultsPageSize * resultsPagesToGet;

            IList<websearchsound> searchResultList = new List<websearchsound>();

            if (dataSource.SetWebsearch(result))
            {
                try
                {
                    for (int page = 0; page < resultsPagesToGet; page++)
                    {
                        string searchResultsJson = extSearch.GetSearchResults(term, clientIp, searchEngineResultsPageSize, page, LogMessage);

                        if (Functions.IsEmptyString(searchResultsJson))
                        {
                            //
                            // Handle case where we can't contact the search provider
                            //
                            LogMessage("Crap, no results.  Aborting search w/no results.");
                            ForceSetStatusComplete(dataSource, result);
                        }
                        else
                        {
                            //
                            // As of this writing, urls are objects that look like:
                            //
                            //  new { Url = url["unescapedUrl"].ToString(), Index = index, Domain = GetDomainOfUrl(url["unescapedUrl"].ToString()) });
                            //
                            var urls = extSearch.GetResultUrls(searchResultsJson);
                            GetSoundsOnPages(urls, dataSource, result, searchResultList, LogMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMessage(string.Format("***********  Had exception seaching for term: {0}, exception stack was:", term));
                    while (ex != null)
                    {
                        LogMessage(string.Format("  Message: {0}", ex.Message));
                        ex = ex.InnerException;
                    }
                    LogMessage("****************************************************************************************");

                    ForceSetStatusComplete(dataSource, result);
                }
            }

            //return result;
        }

        private static void ForceSetStatusComplete(IDataSource dataSource, websearch result)
        {
            //
            // Clean up the search as best we can.
            //
            result.status.isdone = 1;
            result.status.itemsfound = dataSource.CurrentSoundMd5s.Count();
            dataSource.SetWebsearchStatus(result, result.status);
        }


        /// <summary>
        /// Kick off the search.  All the result sounds will be stored in the datasource.
        /// </summary>
        /// <param name="term">The search term, used to reference this search</param>
        /// <param name="clientIp">The client IP, required by some search results providers (I'm looking at you, Google)</param>
        /// <param name="dataSource">The datasource to use to store results</param>
        public static void DoWebImagesearch(string term, string clientIp, IDataSource dataSource, IExternalSearch extSearch)
        {
            const int searchEngineResultsPageSize = 35;
            const int resultsPagesToGet = 1;

            websearch result = new websearch();

            //
            // Values used by various functions through the search process.
            //
            result.status.percentcomplete = 0.0f;
            result.status.searchterm = term;
            result.status.urlsSearched = 0;
            result.status.urlsTotal = searchEngineResultsPageSize * resultsPagesToGet;

            IList<websearchsound> searchResultList = new List<websearchsound>();

            if (dataSource.SetWebsearch(result))
            {
                try
                {
                    for (int page = 0; page < resultsPagesToGet; page++)
                    {
                        string searchResultsJson = extSearch.GetSearchResults(term, clientIp, searchEngineResultsPageSize, page, LogMessage);

                        //
                        // As of this writing, urls are objects that look like:
                        //
                        //  new { Url = url["unescapedUrl"].ToString(), Index = index, Domain = GetDomainOfUrl(url["unescapedUrl"].ToString()) });
                        //
                        var urls = extSearch.GetResultUrls(searchResultsJson);
                        GetSoundsOnPages(urls, dataSource, result, searchResultList, LogMessage);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage(string.Format("***********  Had exception seaching for term: {0}, exception stack was:", term));
                    while (ex != null)
                    {
                        LogMessage(string.Format("  Message: {0}", ex.Message));
                        ex = ex.InnerException;
                    }
                    LogMessage("****************************************************************************************");

                    //
                    // Clean up the search as best we can.
                    //
                    result.status.isdone = 1;
                    dataSource.SetWebsearchStatus(result, result.status);
                }
            }

            //return result;
        }


        /// <summary>
        /// Search the web for sounds matching the term.
        /// </summary>
        /// <param name="term">The search term</param>
        /// <returns>A list of dynamic objects holding a summary of each sound's data.</returns>
        public static websearch SearchSounds(string term, string clientIp, IDataSource dataSource, IExternalSearch extSearch)
        {
            //
            // Let's keep things consistent
            //
            term = term.ToLower();

            IList<websearchsound> searchSounds = new List<websearchsound>();
            websearch result = dataSource.GetWebsearch(term, searchSounds);

            if (result == null)
            {
                LogMessage("*** Spawning new search thread!!! ***");

                //
                // todo: think about what to do if there are exceptions in the threads.  does it kill the App?
                //
                var searchTask = Task.Factory.StartNew(() => DoWebsearch(term, clientIp, dataSource, extSearch));

                result = new websearch();
            }
            else
            {
                //bool willReturnResults = result.status == null ? false : result.status.isdone == 1;

                LogMessage(string.Format("*** Got another search request for term: {0} - returning status: {1}", term, result.status == null ? "null??" : result.status.percentcomplete.ToString("#0.##%")));

                if (searchSounds.Count > 0)
                {
                    result.results = GetSortedSitesAndSounds(searchSounds).ToList();
                }
            }

            return result;
        }

        /// <summary>
        /// Return sound details for a specific sound
        /// </summary>
        /// <param name="term">The search term</param>
        /// <param name="soundId">The sound id</param>
        /// <param name="dataSource">The data source to search</param>
        /// <returns>The sound object</returns>
        public static websearchsound GetSound(string term, string soundId, IDataSource dataSource)
        {
            //
            // I think this is doing a lot of extra work - we're not actually using the results returned.  To optimize.
            //
            IList<websearchsound> searchSounds = new List<websearchsound>();
            websearch existingSearchResults = dataSource.GetWebsearch(term, searchSounds);

            if (existingSearchResults != null)
            {
                websearchsound result = dataSource.GetSoundInSearch(term, soundId);
                PrepareForHttpTransfer(ref result);

                return result; 
            }
            else
            {
                return null;
            }
        }

        public static webimagesearch SearchImages(string term, string clientIp, IDataSource dataSource, IExternalSearch extSearch)
        {
            LogMessage(string.Format("Got image search request: {0}", term));

            string searchResultsJson = extSearch.GetImageSearchResults(term, clientIp, 20, 0, LogMessage);

            //
            // As of this writing, urls are objects that look like:
            //
            //      Url = url["url"].ToString(), Index = index, Source = url["refererurl"].ToString()
            //
            var urls = extSearch.GetImageResultUrls(searchResultsJson);

            webimagesearch result = new webimagesearch();

            result.results = (from r in urls
                              where !(r.Url.ToString().EndsWith(".gif"))
                              select new resultimage() { url = r.Url, thumbnailurl = r.Thumb, sourceurl = r.Source }).ToList();

            return result;
        }
    }
}
