<%@ WebHandler Language="C#" Class="soundbuilder" %>

using System;
using System.Web;
using System.IO;
using OttaMatta.Common;
using OttaMatta.Data;
using Yeti.MMedia;
using Yeti.MMedia.Mp3;
using NAudio;
using WaveLib;
using System.Diagnostics;
using OttaMatta.Application;

/// <summary>
/// Return a sound from the DB as an HTTP document.
/// </summary>
public class soundbuilder : IHttpHandler {

    /// <summary>
    /// Make the magic happen.
    /// </summary>
    /// <param name="context">The request context</param>
    public void ProcessRequest (HttpContext context) {

        string encryptedPlayerRequest = context.Request["soundid"];
        int audioFileType = Functions.ConvertInt(context.Request["type"], 0);
        
        string contentType = "text/plain";
        string audioContentType = string.Empty;
        string sourceDataFile = string.Empty;
        string errMsg = string.Empty;
        
        PlayerRequestInfo requestInfo = new PlayerRequestInfo(encryptedPlayerRequest);
        AudioFileManager files = new AudioFileManager(encryptedPlayerRequest, context.Server.MapPath(Config.CacheSoundsDirectory));

        if (requestInfo.IsValid)
        {

            if (audioFileType == (int)PlayerRequestInfo.AudioFileTypeCode.Mp3)
            {
                audioContentType = "audio/mp3";
                sourceDataFile = files.LocalMp3File;
            }
            else
            {
                audioContentType = "audio/wav";
                sourceDataFile = files.LocalWavFile;
            }


            //
            // If there's an MP3 file already converted, let's use that.
            //
            if (File.Exists(sourceDataFile))
            {
                using (FileStream sourceAudioFile = new FileStream(sourceDataFile, FileMode.Open))
                {
                    sourceAudioFile.CopyTo(context.Response.OutputStream);
                    sourceAudioFile.Close();
                    contentType = audioContentType;
                }
            }
            else
            {
                errMsg = "Can't grab the audio bytes";
            }
        }
        else
        {
            errMsg = "Invalid request info";
        }
        
        context.Response.ContentType = contentType;

        if (!Functions.IsEmptyString(errMsg))
        {
            context.Response.Write(errMsg);
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}