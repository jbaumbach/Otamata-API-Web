using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using OttaMatta.Common;
using OttaMatta.Data.Models;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace OttaMatta.Data.Access
{

    public class DataSourceFileSystem : IDataSource
    {
        public static readonly string DEFAULT_EXT = "otamata";
        public static readonly string STATUS_FNAME = "current.status";
        private HashSet<string> _currentSoundMd5s = new HashSet<string>();

        private string _root = string.Empty;
        private string _webObjectRoot = string.Empty;

        public DataSourceFileSystem(string root, string webObjectRoot)
        {
            _root = root;
            _webObjectRoot = webObjectRoot;
        }


        private static string CacheDirectoryFromSearchTerm(string term)
        {
            string decodedTerm = HttpUtility.UrlDecode(term);
            return decodedTerm.ReplaceAllNonAlphaNumericCharsInString();
        }

        private string PhysicalCacheDir(string searchTerm)
        {
            return Functions.CombineElementsWithDelimiter(@"\", _root, CacheDirectoryFromSearchTerm(searchTerm));
        }

        public websearch GetWebsearch(string term, IList<websearchsound> searchResultSounds)
        {
            websearch result = null;

            //
            // The directory existance is how different threads communicate.  If the dir exists, this thread believes that
            // another thread is currently conducting the search.  It's important that the searching thread complete
            // w/o errors and set the status appropriately.  Otherwise, we're kind of in trouble.
            //

            if (Directory.Exists(PhysicalCacheDir(term)))
            {
                //
                // If we have a directory, then we've started/done a search.  New-up the object and grab the status.
                //
                result = new websearch();
                string statusFilename = Functions.CombineElementsWithDelimiter(@"\", PhysicalCacheDir(term), STATUS_FNAME);
                result.status = Functions.DeserializeObjectFromFile<websearchstatus>(statusFilename);

                //
                // Only return files if we're done with the search.
                //
                if (result.status != null && result.status.isdone == 1)
                {
                    //
                    // The reason we only return files if we're done with the search is that the GetFiles() operation is a shared
                    // resource - read: not thread-friendly.  The calling function doesn't need the results if the search isn't 
                    // finished, and we want to return the status pretty quickly.
                    //
                    string[] sounds = Directory.GetFiles(PhysicalCacheDir(term), string.Format("*.{0}", DEFAULT_EXT));

                    if (sounds.Length > 0)
                    {
                        foreach (string sound in sounds)
                        {
                            websearchsound loadedSound = Functions.DeserializeObjectFromFile<websearchsound>(sound);
                            //
                            // Performance optimization: we're not going to return the sound data itself with the search
                            // so let's free up the mem here
                            //
                            loadedSound.soundbytes = null;

                            searchResultSounds.Add(loadedSound);
                        }
                    }
                }
            }

            return result;
        }

        public bool SetWebsearch(websearch theSearch)
        {
            if (!Directory.Exists(PhysicalCacheDir(theSearch.status.searchterm)))
            {
                Directory.CreateDirectory(PhysicalCacheDir(theSearch.status.searchterm));
            }

            return true;
        }

        public bool SetWebsearchStatus(websearch theSearch, websearchstatus status)
        {
            string statusFilename = Functions.CombineElementsWithDelimiter(@"\", PhysicalCacheDir(theSearch.status.searchterm), STATUS_FNAME);
            bool result = false;

            try
            {
                result = Functions.SerializeObjectToFile<websearchstatus>(statusFilename, status);
            }
            catch
            {
                //
                // Crap, sometimes the file is in use despite the thread lock.  Not sure how to debug.
                //
            }

            return result;
        }

        public websearchsound GetSoundInSearch(string searchTerm, string soundId)
        {
            websearchsound result = null;

            string inputFileName = Functions.CombineElementsWithDelimiter(@"\", PhysicalCacheDir(searchTerm), string.Format("{0}.{1}", soundId, DEFAULT_EXT));

            if (File.Exists(inputFileName))
            {
                result = Functions.DeserializeObjectFromFile<websearchsound>(inputFileName);
            }
            else
            {
                Debug.WriteLine(string.Format("Crap!  Can't find file: \"{0}\"", inputFileName));
            }

            return result;
        }

        public string SetSoundInSearch(websearch search, websearchsound sound)
        {
            if (Functions.IsEmptyString(sound.soundid))
            {
                sound.soundid = Guid.NewGuid().ToString();
            }

            string outputFileName = Functions.CombineElementsWithDelimiter(@"\", PhysicalCacheDir(search.status.searchterm), string.Format("{0}.{1}", sound.soundid, DEFAULT_EXT));

            Functions.SerializeObjectToFile<websearchsound>(outputFileName, sound);

            return sound.soundid;
        }

        public HashSet<string> CurrentSoundMd5s 
        { 
            get 
            { 
                return _currentSoundMd5s; 
            } 
        }


        public string GetUrlContents(string url, string header, string userAgent, Functions.LogMessageDelegate LogMessage)
        {
            string result = string.Empty;
            IWebObjectStorage objectStorage = new WebObjectStorageFileSystem(_webObjectRoot);

            WebObject cached = objectStorage.GetUrlObject(url);

            if (cached != null)
            {
                result = cached.Content;
            }
            else
            {
                result = WebProcessor.GetUrlContents(url, header, userAgent, LogMessage);

                if (!Functions.IsEmptyString(result))
                {
                    cached = new WebObject() { Url = url, Content = result, MimeType = "text/html" };
                    objectStorage.SetUrlObject(cached);
                }
            }

            return result;
        }

    }
}
