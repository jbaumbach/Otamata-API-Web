using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OttaMatta.Common;

/// <summary>
/// Base class for all pages in the Otamata site.
/// </summary>
public class BasePage : System.Web.UI.Page
{
    public string ContentServer { get { return Config.ContentServer; } }
    public string ServerName { get { return Config.ServerName; } }

    /// <summary>
    /// Return true if browser can play MP3s
    /// </summary>
    public bool CanPlayHTML5Sounds
    {
        get
        {
            return !(Request.Browser.Browser.CompareTo("Firefox") == 0);
        }
    }
}
