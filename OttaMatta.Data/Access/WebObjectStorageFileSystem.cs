using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OttaMatta.Data.Models;
using System.IO;
using OttaMatta.Common;

namespace OttaMatta.Data.Access
{
    public class WebObjectStorageFileSystem : IWebObjectStorage
    {
        private string _root = string.Empty;

        public static string ConvertLastDelimToOtherDelim(string source, string origDelim, string newDelim)
        {
            string result = source;

            int loc = source.LastIndexOf(origDelim);

            if (loc >= 0 && loc < source.Length)
            {
                string leftPart = source.Left(loc);
                string rightPart = source.Substring(loc + 1);

                result = string.Format("{0}{1}{2}", leftPart, newDelim, rightPart);
            }

            return result;
        }

        public static string InsertDelimiterAtInterval(string source, string delimiter, int interval)
        {
            if (source.Length > interval)
            {
                string leftChars = source.Left(interval);
                string rightChars = source.Substring(interval);

                return string.Format("{0}{1}{2}", leftChars, delimiter, InsertDelimiterAtInterval(rightChars, delimiter, interval));
            }
            else
            {
                return source;
            }
        }

        public static string WebObjectPathAndName(string root, string url)
        {
            string hash = Functions.GetMd5Hash(url);
            int lengthOfEachDir = 3;
            string delim = @"\";

            string subPath = InsertDelimiterAtInterval(hash, delim, lengthOfEachDir);
            string relativePathToCachedFile = ConvertLastDelimToOtherDelim(subPath, delim, ".");

            string fullPathToCachedFile = Functions.CombineElementsWithDelimiter(delim, root, relativePathToCachedFile);
            return fullPathToCachedFile;
        }

        public void SetUrlObject(WebObject urlObject)
        {
            string pathToObject = WebObjectPathAndName(_root, urlObject.Url);

            string fullPath = Path.GetDirectoryName(pathToObject);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            try
            {
                Functions.SerializeObjectToFile<WebObject>(pathToObject, urlObject);
            }
            catch
            {
                // Crap!
            }
        }

        public WebObject GetUrlObject(string url)
        {
            WebObject result = null;

            string pathToObject = WebObjectPathAndName(_root, url);

            if (File.Exists(pathToObject))
            {
                result = Functions.DeserializeObjectFromFile<WebObject>(pathToObject);
            }

            return result;
        }

        public WebObjectStorageFileSystem(string root)
        {
            _root = root;
        }
    }
}
