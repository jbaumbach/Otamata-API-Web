using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OttaMatta.Application;
using OttaMatta.Common;
using OttaMatta.Data;


public partial class player : BasePage
{
    private enum PlayerMultiviewEnum
    {
        ValidSound = 0,
        InvalidSound = 1
    }

    private enum PlayerControlsMultiviewEnum
    {
        HTML5Player = 0,
        EmbedPlayer = 1
    }

    /// <summary>
    /// Where the sound data will come from
    /// </summary>
    protected string SoundSrcDataMp3;
    protected string SoundSrcDataWav;
    protected string EmbedSoundFileUrl;
    protected string EmbedMimeType;

    /// <summary>
    /// The sound info from the DB
    /// </summary>
    protected dynamic theSound = null;

    /// <summary>
    /// Default title for the page
    /// </summary>
    new protected string Title = "Otamata Soundplayer";

    /// <summary>
    /// An hourglass icon url suitable for the 'src=' attribute.
    /// </summary>
    protected string HourglassImage
    {
        get
        {
            // return string.Format("{0}/images/tiny-hourglass.gif", Config.ServerName);
            string img = "tiny-hourglass-square2.gif";
            return string.Format("data:{0};base64,{1}", "image/gif", Functions.Base64EncodeFile(Server.MapPath(string.Format("/images/{0}", img))));
        }
    }

    /// <summary>
    /// The sound icon url
    /// </summary>
    protected string IconImage { get; set; }

    /// <summary>
    /// Link to the full details page
    /// </summary>
    protected string LinkToFullDetails { get; set; }

    /// <summary>
    /// Link to about page
    /// </summary>
    protected string AboutOtamataLink { get; set; }

    /// <summary>
    /// The page load function
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The event args</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //
        // The route stuff is set up in Global.asax
        //
        string encryptedPlayerRequest = Page.RouteData.Values["encryptedId"] as string;
        //string soundId = string.Empty;

        PlayerRequestInfo requestInfo = new PlayerRequestInfo(encryptedPlayerRequest);

        //
        // Let's validate the ID and, while we're at it, grab some sound info from the DB
        //
        if (requestInfo.IsValid)        // Functions.IsNumeric(soundId))re
        {
            theSound = DataManager.GetSoundDetails(requestInfo.SoundId);
        }
        else
        {
            //
            // Todo: put this into the event log or something?
            //
            string problem = requestInfo.InvalidReason;
        }

        if (theSound != null)
        {
            //
            // Interesting option for loading the sound data.  Put it inline with the element.
            // Pros: mobile browser doesn't have to go to the server again to get the sound data.
            // Cons: the page will seem more sluggish to load.
            //
            // SoundSrcData = string.Format("data:{0};base64,{1}", MimeTypeFromFileName(theSound.FileName), theSound.Data);

            AudioFileManager files = new AudioFileManager(encryptedPlayerRequest, HttpContext.Current.Server.MapPath(Config.CacheSoundsDirectory));
            files.CreateTempFiles();

            if (files.HaveAtLeastOneFile())
            {
                //
                // Tell the browser to load the data from this url.  Pass the encrypted sound id to keep people from being able to easily
                // go through all the sounds.
                //
                SoundSrcDataMp3 = string.Format("{0}/handlers/soundbuilder.ashx?soundid={1}&type={2}", Config.ServerName, encryptedPlayerRequest, (int)PlayerRequestInfo.AudioFileTypeCode.Mp3);
                mp3LinkPlaceholder.Visible = files.HaveFileOfType(PlayerRequestInfo.AudioFileTypeCode.Mp3);

                SoundSrcDataWav = string.Format("{0}/handlers/soundbuilder.ashx?soundid={1}&type={2}", Config.ServerName, encryptedPlayerRequest, (int)PlayerRequestInfo.AudioFileTypeCode.Wav);
                wavLinkPlaceholder.Visible = files.HaveFileOfType(PlayerRequestInfo.AudioFileTypeCode.Wav);

                if (requestInfo.Type == PlayerRequestInfo.PlayerDisplayType.AllDetails)
                {
                    //
                    // Let's show all the sound details
                    //
                    SoundDetailsPlaceholder.Visible = true;
                    JavascriptPlaceholder.Visible = true;

                    Title = string.Format("{0} - Otamata Soundplayer", theSound.Name);

                    if (theSound.HasIcon)
                    {
                        IconImage = string.Format("{0}/handlers/getsoundicon.ashx?soundid={1}", Config.ServerName, encryptedPlayerRequest);
                    }
                    else
                    {
                        IconImage = string.Format("{0}/images/default-icon1.png", Config.ServerName);
                    }
                }
                else if (requestInfo.Type == PlayerRequestInfo.PlayerDisplayType.NoInfoButWithOptionToShowInfo)
                {
                    //
                    // Not showing info, but the user can click a link to more info
                    //
                    ViewSoundDetailsLinkPlaceholder.Visible = true;
                    LinkToFullDetails = PlayerRequestInfo.SoundPlayerUrl(requestInfo.SoundId, requestInfo.Version, PlayerRequestInfo.PlayerDisplayType.AllDetails);

                }
                else
                {
                    //
                    // Default - no details
                    //
                }

                /*

                 Browser        HTML5+WAV   HTML5+MP3   Embed+MP3       Embed+WAV
                 Firefox        N           N           Y               Y
                 IE             N           Y           N               N
                 Safari Mac     Y           Y           Y (weird)       Y (fine)
                 Safari iOS     N           Y           Y (new plyr)    N (err msg)
                 Android        N           Y           N               N
             
                */

                if (CanPlayHTML5Sounds)
                {
                    PlayerControlsMultiview.ActiveViewIndex = (int)PlayerControlsMultiviewEnum.HTML5Player;
                }
                else
                {
                    if (files.HaveFileOfType(PlayerRequestInfo.AudioFileTypeCode.Mp3))
                    {
                        EmbedSoundFileUrl = SoundSrcDataMp3;
                        EmbedMimeType = "audio/mp3";
                    }
                    else if (files.HaveFileOfType(PlayerRequestInfo.AudioFileTypeCode.Wav))
                    {
                        EmbedSoundFileUrl = SoundSrcDataWav;
                        EmbedMimeType = "audio/wav";
                    }

                    PlayerControlsMultiview.ActiveViewIndex = (int)PlayerControlsMultiviewEnum.EmbedPlayer;
                }

                PlayerMultiview.ActiveViewIndex = (int)PlayerMultiviewEnum.ValidSound;

            } // HaveAtLeastOnefile
            else
            {
                PlayerMultiview.ActiveViewIndex = (int)PlayerMultiviewEnum.InvalidSound;
            }
        }
        else
        {
            PlayerMultiview.ActiveViewIndex = (int)PlayerMultiviewEnum.InvalidSound;
        }


        AboutOtamataLink = Functions.CombineUrlElements(Config.ServerName, string.Empty);
    }

}