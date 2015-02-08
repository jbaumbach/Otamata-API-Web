using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OttaMatta.Common;

namespace OttaMatta.Application
{
/// <summary>
/// Summary description for PlayerRequestInfo
/// </summary>
    public class PlayerRequestInfo
    {
        public enum AudioFileTypeCode
        {
            Wav = 0,
            Mp3 = 1
        }

        //
        // These enumerations should match the enums of the same name in the client apps
        //
        //
        // Possible sound player types.  Need to keep this syncronized with the web server.
        //
        public enum SoundPlayerVersion
        {
            Version1 = 1
        }

        //
        // Possible ways to display the sound.  
        //
        public enum PlayerDisplayType
        {
            AllDetails = 0,
            NoIdentifyingInfo = 1,
            NoInfoButWithOptionToShowInfo = 2
        }

        /// <summary>
        /// The sound id
        /// </summary>
        public int SoundId { get; set; }

        /// <summary>
        /// The sound player version
        /// </summary>
        public SoundPlayerVersion Version { get; set; }

        /// <summary>
        /// The display type for the sound
        /// </summary>
        public PlayerDisplayType Type { get; set; }

        /// <summary>
        /// True if this class contains valid information
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// If invalid, the reason why. Prolly shouldn't display this to the client.
        /// </summary>
        public string InvalidReason { get; set; }

        /// <summary>
        /// Parse the encrypted player request parameter
        /// </summary>
        /// <param name="encryptedPlayerRequest">The passed player request parameter from the client.</param>
        public PlayerRequestInfo(string encryptedPlayerRequest)
        {
            //
            // Initial setup
            //
            SoundId = -1;
            IsValid = false;
            InvalidReason = "Uninitialized";

            try
            {
                InitFromRequestParameter(encryptedPlayerRequest);
            }
            catch (Exception ex)
            {
                InvalidReason = string.Format("Processing error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Attempt to parse the passed player request string and set the class properties
        /// </summary>
        /// <param name="encryptedPlayerRequest">The request to parse.</param>
        private void InitFromRequestParameter(string encryptedPlayerRequest)
        {
            //
            // As of this writing, the minimum # of chars in an encrypted value is 5
            //
            if (!Functions.IsEmptyString(encryptedPlayerRequest) && encryptedPlayerRequest.Length >= 5)
            {
                string versionAndType = encryptedPlayerRequest.Substring(encryptedPlayerRequest.Length - 3);

                int lowerCaseaInAscii = 97;

                int version = versionAndType.Left(1).ToCharArray()[0];
                version = version - lowerCaseaInAscii;

                int type = versionAndType.Substring(versionAndType.Length - 1).ToCharArray()[0];
                type = type - lowerCaseaInAscii;

                if (Enum.IsDefined(typeof(SoundPlayerVersion), version) && Enum.IsDefined(typeof(PlayerDisplayType), type))
                {
                    //
                    // Ok, so far so good
                    //
                    Version = (SoundPlayerVersion)version;
                    Type = (PlayerDisplayType)type;

                    if (Version == SoundPlayerVersion.Version1)
                    {
                        /*
                        /player/([sound number digit as letter][random letter])*[player version as letter (a=0, b=2, etc.)][random letter][display type as letter]

                        Example

                        player version = 1
                        dispay type = 0
                        sound number = 23

                        /player/b6cPbWa
                        */

                        string encryptedSoundId = encryptedPlayerRequest.Left(encryptedPlayerRequest.Length - 3);
                        string soundIdTemp = string.Empty;

                        for (int i = 0; i < encryptedSoundId.Length; i += 2)
                        {
                            int encryptedVal = encryptedSoundId.Substring(i, 1).ToCharArray()[0];
                            int digit = encryptedVal - lowerCaseaInAscii;

                            soundIdTemp += digit.ToString();
                        }

                        SoundId = Functions.ConvertInt(soundIdTemp, -1);

                        if (SoundId < 0)
                        {
                            InvalidReason = string.Format("Unable to parse sound id: {0}", soundIdTemp);
                        }

                        IsValid = (SoundId >= 0);
                    }
                    else
                    {
                        InvalidReason = string.Format("Version {0} not supported", version);
                    }
                }
                else
                {
                    InvalidReason = string.Format("Type {0} or version {1} are not valid", type, version);
                }
            }
            else
            {
                InvalidReason = "Not enough characters to process.";
            }

        }

        /// <summary>
        /// Build a url to the otamata sound player.
        /// </summary>
        /// <param name="soundId">The sound id</param>
        /// <param name="version">What version of player</param>
        /// <param name="displayType">What display type of player</param>
        /// <returns>The player url</returns>
        public static string SoundPlayerUrl(int soundId, SoundPlayerVersion version, PlayerDisplayType displayType)
        {
            return SoundPlayerUrl(soundId.ToString(), version, displayType);
        }

        /// <summary>
        /// Build a url to the otamata sound player.
        /// </summary>
        /// <param name="soundId">The sound id</param>
        /// <param name="version">What version of player</param>
        /// <param name="displayType">What display type of player</param>
        /// <returns>The player url</returns>
        public static string SoundPlayerUrl(string soundId, SoundPlayerVersion version, PlayerDisplayType displayType)
        {
            if (version == SoundPlayerVersion.Version1)
            {

                /*
                 /player/([sound number digit as letter][random letter])*[player version as letter (a=0, b=2, etc.)][random letter][display type as letter]
         
                 Example
         
                 player version = 1
                 dispay type = 0
                 sound number = 23
         
                 /player/b6cPbWa
                 */

                string encryptedSoundId = string.Empty;

                int lowerCaseaInAscii = 97;
                int upperCaseAInAscii = 65;
                int soundIdLen = soundId.Length;

                Random rand1 = new Random();

                for (int i = 0; i < soundIdLen; i++)
                {
                    int digit = soundId[i] - '0'; // [soundId characterAtIndex:i] - '0';

                    // encryptedSoundId = [NSString stringWithFormat:@"%@%c%c", encryptedSoundId, lowerCaseaInAscii + digit, random() % 26 + upperCaseAInAscii];
                    encryptedSoundId = string.Format("{0}{1}{2}", encryptedSoundId, (char)(lowerCaseaInAscii + digit), (char)(rand1.Next(26) + upperCaseAInAscii));
                }

                // encryptedSoundId = [NSString stringWithFormat:@"%@%c%c%c", encryptedSoundId, lowerCaseaInAscii + version, random() % 26 + upperCaseAInAscii, lowerCaseaInAscii + displayType];
                encryptedSoundId = string.Format("{0}{1}{2}{3}", encryptedSoundId, (char)(lowerCaseaInAscii + version), (char)(rand1.Next(26) + upperCaseAInAscii), (char)(lowerCaseaInAscii + displayType));

                // return [NSString stringWithFormat:@"http://%@/player/%@", [self remoteHost], encryptedSoundId];
                return string.Format("{0}/player/{1}", Config.ServerName, encryptedSoundId);
            }
            else
            {
                throw new NotImplementedException(string.Format("Invalid sound player version: {0}", version));
            }
        }
    }
}