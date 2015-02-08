using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OttaMatta.Common;

/// <summary>
/// Summary description for BaseControl
/// </summary>
public class BaseControl : System.Web.UI.UserControl
{
    public string ContentServer { get { return Config.ContentServer; } }
    public string ServerName { get { return Config.ServerName; } }
}