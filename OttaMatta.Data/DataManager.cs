using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using OttaMatta.Common;
using System.Dynamic;
using System.Data.SqlClient;
using System.IO;

namespace OttaMatta.Data
{
    /// <summary>
    /// Class to help grabbing stuff from the database.
    /// </summary>
    public class DataManager : DataFunctions
    {
        /// <summary>
        /// The activity codes for various actions.
        /// </summary>
        /// <remarks>
        /// Some of these are record in other areas, so they're not used here.
        /// </remarks>
        public enum ActivityTypeCode
        {
            SearchSounds = 0,
            //DownloadSound = 1,
            InAppPurchase = 2,
            UserContact = 3,
            //SoundRating = 4,
            //SoundInappropriate = 5
        }

        /// <summary>
        /// Search the database for sounds matching the term.
        /// </summary>
        /// <param name="term">The search term</param>
        /// <returns>A list of dynamic objects holding a summary of each sound's data.</returns>
        public static List<dynamic> SearchSounds(string term, int order, bool includeInappropriate)
        {
            long temp;
            return SearchSounds(term, order, includeInappropriate, 0, 50, out temp);
        }

        /// <summary>
        /// Search the database for sounds matching the term.
        /// </summary>
        /// <param name="term">The search term</param>
        /// <returns>A list of dynamic objects holding a summary of each sound's data.</returns>
        public static List<dynamic> SearchSounds(string term, int order, bool includeInappropriate, int pageToGet, int pageSize, out long totalResultSize)
        {
            List<dynamic> result = new List<dynamic>();

            string sql = "[dbo].[spSound_Search]";
            totalResultSize = -1;

            List<SqlParameter> prams = new List<SqlParameter>();
            prams.Add(new SqlParameter("@term", term));
            prams.Add(new SqlParameter("@order", order));
            prams.Add(new SqlParameter("@includeInappropriate", includeInappropriate ? 1 : 0));
            prams.Add(new SqlParameter("@pageToReturn", pageToGet));
            prams.Add(new SqlParameter("@rowsToReturn", pageSize));

            DataSet truckList = GetProcResults(sql, prams.ToArray());

            if (Functions.HasTables(truckList))
            {
                foreach (DataRow row in truckList.Tables[0].Rows)
                {
                    dynamic theSound = new ExpandoObject();
                    theSound.Id = Functions.GetIntFromDataRow(row, "sound_id", -1);
                    theSound.Name = Functions.GetStringFromDataRow(row, "sound_name");
                    theSound.FileName = Functions.GetStringFromDataRow(row, "sound_filename");
                    theSound.Description = Functions.GetStringFromDataRow(row, "sound_description");
                    theSound.UploadBy = Functions.GetStringFromDataRow(row, "sound_upload_by");
                    DateTime ulDate = Functions.GetDateTimeFromDataRow(row, "sound_date");
                    theSound.UploadDate = new DateTime(ulDate.Year, ulDate.Month, ulDate.Day, ulDate.Hour, ulDate.Minute, ulDate.Second, ulDate.Millisecond, DateTimeKind.Utc);
                    theSound.DownloadCount = Functions.GetIntFromDataRow(row, "sound_download_count", 0);
                    theSound.HasIcon = Functions.GetIntFromDataRow(row, "has_icon", 0);
                    theSound.Size = Functions.GetIntFromDataRow(row, "sound_size", -1);
                    theSound.AverageRating = Functions.GetFloatFromDataRow(row, "sound_rating", -1);
                    result.Add(theSound);
                }

                if (Functions.HasRowsInTable(truckList, 1))
                {
                    totalResultSize = Functions.GetLongFromDataRow(truckList.Tables[1].Rows[0], "search_total", -1);
                }
            }

            return result;
        }

        /// <summary>
        /// Inserts a sound into the database.
        /// </summary>
        /// <param name="name">Sound name</param>
        /// <param name="fileName">Sound filename</param>
        /// <param name="description">Sound description</param>
        /// <param name="uploadBy">Who uploaded the sound</param>
        /// <param name="fileData">The bytes of the sound</param>
        /// <param name="iconExt">The extension of the icon file name</param>
        /// <param name="iconData">The icon's bytes</param>
        /// <returns>True if it worked.</returns>
        public static bool InsertSound(string name, string fileName, string description, string uploadBy, byte[] fileData, string iconExt, byte[] iconData)
        {
            int newId;
            return InsertSound(name, fileName, description, uploadBy, fileData, iconExt, iconData, true, out newId);
        }
                    /// <summary>
        /// Inserts a sound into the database.
        /// </summary>
        /// <param name="name">Sound name</param>
        /// <param name="fileName">Sound filename</param>
        /// <param name="description">Sound description</param>
        /// <param name="uploadBy">Who uploaded the sound</param>
        /// <param name="fileData">The bytes of the sound</param>
        /// <param name="iconExt">The extension of the icon file name</param>
        /// <param name="iconData">The icon's bytes</param>
        /// <param name="isEnabled">If the sound should be enabled (default = true)</param>
        /// <param name="newId">(out) The new sound's id if it worked</param>
        /// <returns>True if it worked.</returns>
        public static bool InsertSound(string name, string fileName, string description, string uploadBy, byte[] fileData, string iconExt, byte[] iconData, bool isEnabled, out int newId)
        {
            newId = -1;
            string sql = "[dbo].[spSound_Insert]";

            List<SqlParameter> prams = new List<SqlParameter>();

            prams.Add(new SqlParameter("@name", name));
            prams.Add(new SqlParameter("@fileName", fileName));
            prams.Add(new SqlParameter("@description", description));
            prams.Add(new SqlParameter("@uploadBy", uploadBy));

            SqlParameter file = new SqlParameter("@soundData", SqlDbType.VarBinary);
            file.Value = fileData;
            prams.Add(file);

            prams.Add(new SqlParameter("@md5", Functions.GetMd5Hash(fileData)));

            if (!Functions.IsEmptyString(iconExt) && iconData != null)
            {
                SqlParameter icon = new SqlParameter("@iconData", SqlDbType.VarBinary);
                icon.Value = iconData;
                prams.Add(icon);

                prams.Add(new SqlParameter("@icon_ext", iconExt.Left(10)));
                prams.Add(new SqlParameter("@icon_md5", Functions.GetMd5Hash(iconData)));
            }

            prams.Add(new SqlParameter("@isEnabled", isEnabled ? 1 : 0));

            //
            // 2012/06/05 JB: updated stored proc to return the new id if it worked
            //
            // int rowsInserted = DataFunctions.ExecuteNonQueryProc(sql, prams.ToArray());

            //newId = DataFunctions.ExecuteNonQueryProc(sql, prams.ToArray());

            DataSet res = GetProcResults(sql, prams.ToArray());

            if (res.HasRows())
            {
                newId = res.Tables[0].Rows[0].GetIntFromDataRow("new_sound_id", -1);
            }

            return newId > 0;
        }

        /// <summary>
        /// Get sound details from the DB for the passed ID
        /// </summary>
        /// <param name="soundId">The ID to lookup</param>
        /// <returns>A dynamically built object holding the returned data, or null</returns>
        public static dynamic GetSoundDetails(int soundId)
        {
            dynamic result = null;
            string sql = "[dbo].[spSound_GetDetails]";

            List<SqlParameter> prams = new List<SqlParameter>();

            prams.Add(new SqlParameter("@soundId", soundId));

            DataSet res = GetProcResults(sql, prams.ToArray());

            if (res.HasRows())
            {
                result = new ExpandoObject();

                DataRow row = res.Tables[0].Rows[0];

                result.Id = row.GetIntFromDataRow("sound_id", -1);
                result.Name = row.GetStringFromDataRow("sound_name");
                result.FileName = row.GetStringFromDataRow("sound_filename");
                result.Description = row.GetStringFromDataRow("sound_description");
                result.UploadBy = Functions.GetStringFromDataRow(row, "sound_upload_by");
                DateTime ulDate = Functions.GetDateTimeFromDataRow(row, "sound_date");
                result.UploadDate = new DateTime(ulDate.Year, ulDate.Month, ulDate.Day, ulDate.Hour, ulDate.Minute, ulDate.Second, ulDate.Millisecond, DateTimeKind.Utc);
                result.Size = Functions.GetIntFromDataRow(row, "sound_size", -1);
                result.HasIcon = Functions.GetIntFromDataRow(row, "has_icon", -1) == 1;
            }

            return result;
        }

        /// <summary>
        /// Get the sound bytes and other assorted data from the database.
        /// </summary>
        /// <param name="soundId">The sound id</param>
        /// <returns>A dynamic object containing the info.</returns>
        public static dynamic GetSoundData(int soundId)
        {
            dynamic result = new ExpandoObject();
            string sql = "[dbo].[spSound_GetBinaryData]";

            List<SqlParameter> prams = new List<SqlParameter>();

            prams.Add(new SqlParameter("@soundId", soundId));

            DataSet res = GetProcResults(sql, prams.ToArray());

            //
            // Todo: remember why I'm initing these values here rather than just returning NULL
            //
            result.Id = -1;
            result.FileName = string.Empty;
            result.Data = string.Empty;
            result.Md5 = string.Empty;

            if (res.HasRows())
            {
                DataRow row = res.Tables[0].Rows[0];

                result.Id = row.GetIntFromDataRow("sound_id", -1);
                result.FileName = row.GetStringFromDataRow("sound_filename");
                result.Md5 = row.GetStringFromDataRow("sound_md5");
                result.Data = System.Convert.ToBase64String((byte[]) row["sound_data"], Base64FormattingOptions.None);
                result.Size = row.GetIntFromDataRow("sound_size", -1);
            }

            return result;
        }

        /// <summary>
        /// Get the sound's icon file data from the database
        /// </summary>
        /// <param name="soundId">The sound id</param>
        /// <returns>A dynamic object containing the sound data</returns>
        public static dynamic GetIconData(int soundId)
        {
            dynamic result = new ExpandoObject();
            string sql = "[dbo].[spSound_GetIconData]";

            List<SqlParameter> prams = new List<SqlParameter>();

            prams.Add(new SqlParameter("@soundId", soundId));

            DataSet res = GetProcResults(sql, prams.ToArray());

            result.Id = -1;
            result.Extension = string.Empty;
            result.Data = string.Empty;
            result.Md5 = string.Empty;

            if (res.HasRows())
            {
                DataRow row = res.Tables[0].Rows[0];

                //
                // For some reason, the proc does an inner join on the icon table.  So, this can be null.  Dumb.
                //
                if (!Functions.IsEmptyString(row.GetStringFromDataRow("icon_extension")))
                {
                    result.Id = row.GetIntFromDataRow("sound_id", -1);
                    result.Extension = row.GetStringFromDataRow("icon_extension");
                    result.Md5 = row.GetStringFromDataRow("icon_md5");
                    result.Data = System.Convert.ToBase64String((byte[])row["sound_icon_data"], Base64FormattingOptions.None);
                }
            }

            return result;
        }

        /// <summary>
        /// Insert a sound rating to the DB
        /// </summary>
        /// <param name="soundId">The sound id</param>
        /// <param name="ratingValue">The rating value</param>
        /// <param name="ratingText">The rating text</param>
        /// <returns>True if there was no exception</returns>
        public static bool RateSound(int soundId, int ratingValue, string ratingText)
        {
            // exec [spRating_Insert] @soundId = 12, @ratingValue = 5, @ratingText = 'Reasonably solid'

            string sql = "[dbo].[spRating_Insert]";

            List<SqlParameter> prams = new List<SqlParameter>();
            prams.Add(new SqlParameter("@soundId", soundId));
            prams.Add(new SqlParameter("@ratingValue", ratingValue));
            prams.Add(new SqlParameter("@ratingText", ratingText == null ? null : ratingText.Left(140)));

            int rows = ExecuteNonQuery(sql, prams.ToArray());


            return (rows > 0);
        }

        /// <summary>
        /// Mark a sound as inappropriate
        /// </summary>
        /// <param name="soundId">The sound id</param>
        /// <returns>True if there was no exception</returns>
        public static bool MarkSoundInappropriate(int soundId)
        {
            string sql = "[dbo].[spInappropriate_Insert]";

            List<SqlParameter> prams = new List<SqlParameter>();
            prams.Add(new SqlParameter("@soundId", soundId));

            int rows = ExecuteNonQuery(sql, prams.ToArray());

            return (rows > 0);
        }

        /// <summary>
        /// Mark a sound as purchased
        /// </summary>
        /// <param name="soundId">The sound id</param>
        /// <returns>True if there was no exception</returns>
        public static bool PurchaseSound(int soundId, string deviceId)
        {
            string sql = "[dbo].[spPurchase_Insert]";

            List<SqlParameter> prams = new List<SqlParameter>();
            prams.Add(new SqlParameter("@soundId", soundId));
            prams.Add(new SqlParameter("@purchaseBy", deviceId));
            int rows = ExecuteNonQuery(sql, prams.ToArray());

            return (rows > 0);
        }

        /// <summary>
        /// Record a store purchase
        /// </summary>
        /// <param name="purchaseId">The purchase id (aka product id from iTunesConnect)</param>
        /// <param name="userId">The user id</param>
        /// <returns>True if the row was inserted correctly</returns>
        public static bool RecordPurchase(string purchaseId, string deviceId, string appVersion)
        {
            return RecordActivity(ActivityTypeCode.InAppPurchase, 0, deviceId, appVersion, purchaseId);
        }

        /// <summary>
        /// Record activity happing with devices
        /// </summary>
        /// <param name="type">The activity type</param>
        /// <param name="deviceType">The device type</param>
        /// <param name="deviceId">The device ID (usually the UUID)</param>
        /// <param name="appVersion">The app version running on the device</param>
        /// <param name="detail">Optional detail for the activity</param>
        /// <returns></returns>
        public static bool RecordActivity(ActivityTypeCode type, int deviceType, string deviceId, string appVersion, string detail)
        {
            string sql = "[dbo].[spDeviceActivity_Insert]";

            List<SqlParameter> prams = new List<SqlParameter>();
            prams.Add(new SqlParameter("@deviceId", deviceId.Left(40)));
            prams.Add(new SqlParameter("@deviceType", deviceType));
            prams.Add(new SqlParameter("@appVersion", appVersion.Left(10)));
            prams.Add(new SqlParameter("@activityTypeCode", type));
            prams.Add(new SqlParameter("@detail", detail.Left(50)));

            int rows = ExecuteNonQuery(sql, prams.ToArray());

            return (rows > 0);
        }


    }
}
