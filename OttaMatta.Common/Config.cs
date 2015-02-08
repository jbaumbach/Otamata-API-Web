using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace OttaMatta.Common
{
    /// <summary>
    /// Defines config settings for the application.
    /// </summary>
    public class Config
    {
        #region Public Static Methods
        /// <summary>
        /// The current server name for use in a fully qualified uri.
        /// </summary>
        public static string ServerName { get { return Get("ServerName"); } }

        /// <summary>
        /// The current content server name for use in a fully qualified uri.
        /// </summary>
        public static string ContentServer { get { return Get("ContentServer"); } }

        /// <summary>
        /// Database connection string.
        /// </summary>
        public static string ConnectionString { get { return Get("ConnectionString"); } }

        /// <summary>
        /// Get whether sound uploading is enabled or not.
        /// </summary>
        public static bool EnableSoundUpload { get { return Functions.ConvertBool(Get("EnableSoundUpload"), false); } }

        /// <summary>
        /// Get whether we are requiring an icon to upload with each sound
        /// </summary>
        public static bool RequireIconUpload { get { return Functions.ConvertBool(Get("RequireIconUpload"), false); } }

        /// <summary>
        /// Determine if we should enable analytics or not (default is false)
        /// </summary>
        public static bool AnalyticsEnabled { get { return Get("AnalyticsEnabled") == "true"; } }

        /// <summary>
        /// Where our cached sounds go
        /// </summary>
        public static string CacheSoundsDirectory { get { return Get("CacheSoundsDirectory"); } }

        /// <summary>
        /// Where our cached sounds go
        /// </summary>
        public static readonly string CacheSearchesDirectory = "CacheSearchesDirectory";

        public static readonly string CacheWebobjectsDirectory = "CacheWebobjectsDirectory";

        public static readonly string EnableLogging = "EnableLogging";

        /// <summary>
        /// Path to our log file
        /// </summary>
        public static readonly string PathToLogfile = "PathToLogfile";

        /// <summary>
        /// Grab a string from the config file.
        /// </summary>
        /// <param name="key">The string to get</param>
        /// <returns>The value.</returns>
        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
        #endregion

    }
}
