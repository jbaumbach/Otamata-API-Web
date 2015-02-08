using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _default : System.Web.UI.Page
{
    protected string iTunesStoreUrl { get { return @"http://itunes.apple.com/us/app/otamata-infinite-soundboard/id498904123?mt=8"; } }
    protected string FullAppName { get { return @"Otamata Social Soundboard"; } }
    protected string UnlimitedDLPrice { get { return @"$2.99"; } }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}