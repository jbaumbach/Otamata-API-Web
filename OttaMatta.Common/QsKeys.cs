using System;
using System.Collections.Generic;
using System.Text;

namespace OttaMatta.Common
{
    /// <summary>
    /// Form key values.  These need to match the client's values.
    /// </summary>
    public class QsKeys
    {
        public static string Term { get { return "term"; } }
        public static string Order { get { return "order"; } }
        public static string Incappr { get { return "incappr"; } }
        public static string SoundId { get { return "soundId"; } }
        public static string Rating { get { return "rating"; } }
        public static string Text { get { return "text"; } }
        public static string DeviceId { get { return "deviceId"; } }
        public static string UserId { get { return "userId"; } }
        public static string PurchaseId { get { return "purchaseId"; } }
        public static string AppVersion { get { return "appVersion"; } }
        public static string Name { get { return "name"; } }
        public static string Description { get { return "desc"; } }
        public static string SoundFName { get { return "soundfname"; } }
        public static string SoundData { get { return "sounddata"; } }
        public static string IconFName { get { return "iconfname"; } }
        public static string IconData { get { return "icondata"; } }
        public static string SoundDataMd5 { get { return "sounddatamd5"; } }
        public static string IconDataMd5 { get { return "icondatamd5"; } }
        public static string IsBrowsable { get { return "isBrowsable"; } }
    }
}
