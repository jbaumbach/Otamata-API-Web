<%@ WebHandler Language="C#" Class="getsoundicon" %>

using System;
using System.Web;
using OttaMatta.Data;
using OttaMatta.Common;
using OttaMatta.Application;

public class getsoundicon : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        
        string encryptedPlayerRequest = context.Request["soundid"];
        bool haveSound = false;
        string contentType = "text/plain";
        
        PlayerRequestInfo requestInfo = new PlayerRequestInfo(encryptedPlayerRequest);
        
        dynamic theSound = null;

        if (requestInfo.IsValid)   // Functions.IsNumeric(soundId))
        {
            int soundId = requestInfo.SoundId;

            theSound = DataManager.GetIconData(Functions.ConvertInt(soundId, -1));

            if (theSound != null && theSound.Id >= 0)
            {
                contentType = Functions.MimeTypeFromFileName(theSound.Extension);
                
                byte[] originalSourceByteArray = System.Convert.FromBase64String(theSound.Data);

                context.Response.OutputStream.Write(originalSourceByteArray, 0, originalSourceByteArray.Length);
                haveSound = true;
            }
        }

        if (!haveSound)
        {
            context.Response.Write("Oops!  Cant find the icon.");
        }

        context.Response.ContentType = contentType;
        
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}