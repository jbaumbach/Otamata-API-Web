<%@ Page Language="C#" AutoEventWireup="true" CodeFile="player.aspx.cs" Inherits="player" EnableEventValidation="false" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/analytics.ascx" TagName="Analytics" %>
<%@ Import Namespace="OttaMatta.Common" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <%-- Responsive web design: http://www.sitepoint.com/responsive-web-design/#fbid=OBcGeSLN3i1  --%>
    <meta name="viewport" content="width=device-width" />

    <link href="/stylesheets/player.css" media="screen,projection" type="text/css" rel="Stylesheet" />
    <title><%=Title %></title>

    <%-- What tags are: http://developers.facebook.com/docs/opengraphprotocol/ --%>
    <asp:Placeholder ID="opengraphTags" runat="server">
        <meta property="og:title" content="<%=Title %>"/>
        <meta property="og:type" content="article"/>  <%-- "audio" and "richaudio" bomb out with the error below --%>
        <meta property="og:url" content="<%=Request.Url.ToString() %>"/>
        <meta property="og:image" content="<%=IconImage %>"/>

        <%-- FB needs to see ".mp3" at the end I think.  Just a link to the player causes this error:

            Unknown Object Type:	Object at URL 'http://www.otamata.com/player/itgdbca' is invalid because the configured 'og:type' of 'audio' is invalid.
        
        <meta property="og:audio" content="<%=SoundSrcDataMp3 %>" />
        <meta property="og:audio:type" content="application/mp3" />
        --%>
        <meta property="fb:app_id" content="269490483146896"/> 
        <%-- can't seem to find my personal Facebook id anymore... <meta property="fb:admins" content="3730507494989,USER_ID2"/>--%>
               
        <meta property="og:site_name" content="Otamata Social Soundboard" />
        <meta property="og:description" content="Click url to play sound.  Powered by Otamata, the simplest way to share sound clips and sound effects with your friends from your iPhone." />
    </asp:Placeholder>
</head>
<body>
    <div id="content">
    <div id="player" class="page">
        <%--<h1>Soundplayer</h1>--%>
        <asp:Multiview ID="PlayerMultiview" runat="server" ActiveViewIndex="0">
        <asp:View ID="ValidSound" runat="server">
        <asp:Placeholder ID="sharingPlaceholder" runat="server" Visible="true">
        <div id="sharing">
        <%-- Example url for player: http%3A%2F%2Fwww.otamata.com%2Fplayer%2Fbtdxbfa&amp; --%>
        <%-- Twitter --%>
        <iframe src="//platform.twitter.com/widgets/tweet_button.html?text=Check%20out%20this%20sound%3a&amp;url=<%=HttpUtility.UrlEncode(Request.Url.ToString()) %>&amp;count=none&amp;hashtags=OtamataSndBoard" style="width:58px; height:21px;" allowtransparency="true" frameborder="0" scrolling="no"></iframe>
        <%-- Facebook --%>
        <iframe src="//www.facebook.com/plugins/like.php?href=<%=HttpUtility.UrlEncode(Request.Url.ToString()) %>&amp;send=false&amp;layout=button_count&amp;width=90&amp;show_faces=false&amp;action=like&amp;colorscheme=light&amp;font&amp;height=21" scrolling="no" frameborder="0" style="border:none; overflow:hidden; width:90px; height:21px;" allowTransparency="true"></iframe>
        </div>
        </asp:Placeholder>
        <asp:PlaceHolder ID="SoundDetailsPlaceholder" runat="server" Visible="false">
            <img id="soundIconContainer" src="<%=HourglassImage %>" width="95" height="95" />
            <ul id="soundDetail">
            <li><strong><%=HttpUtility.HtmlEncode(theSound.Name) %></strong></li>
            <li><em><%=HttpUtility.HtmlEncode(theSound.Description) %></em></li>
            </ul>
            <ul class="clear">
            <li>Added by <strong><%=HttpUtility.HtmlEncode(theSound.UploadBy) %></strong> <%=((DateTime)theSound.UploadDate).ToLongDateString() %></li>
            </ul>
        </asp:PlaceHolder>
        <asp:MultiView ID="PlayerControlsMultiview" runat="server" ActiveViewIndex="0">
            <asp:View ID="HTML5Player" runat="server">
            <audio id="soundPlayer" controls="controls" preload="auto" autoplay="autoplay">
                <asp:Placeholder ID="mp3LinkPlaceholder" runat="server"><source src="<%=SoundSrcDataMp3 %>" /></asp:Placeholder>
                <asp:PlaceHolder ID="wavLinkPlaceholder" runat="server"><source src="<%=SoundSrcDataWav %>" /></asp:PlaceHolder>
                This feature requires a browser that supports <a href="http://en.wikipedia.org/wiki/HTML5">HTML5</a>!  Sorry.  It was a nice try though.
            </audio>
            </asp:View>
            <asp:View ID="EmbedPlayer" runat="server">
                <%-- embed only works in HTML5, but that's ok.  It's pretty modern.  The other alternative is the object tag.  See here: http://forums.mozillazine.org/viewtopic.php?f=23&t=2117579 --%>
                <embed id="embeddedPlayer" src="<%=EmbedSoundFileUrl %>" autostart="TRUE" loop="FALSE" width="300" height="42" controller="TRUE" bgcolor="#ffffff" />
                <script type="text/javascript">
                    if (!navigator.mimeTypes["<%=EmbedMimeType %>"].enabledPlugin) {
                        document.write('<p id="playLink"><a href="<%=EmbedSoundFileUrl %>">You\'re missing a plugin! No worries, click here to play.</a></p>');
                    }
                </script>
            </asp:View>
        </asp:MultiView>
        <asp:PlaceHolder ID="ViewSoundDetailsLinkPlaceholder" runat="server" Visible="false">
        <h2><a href="<%=LinkToFullDetails %>">View This Sound's Info</a></h2>
        </asp:PlaceHolder>
        <asp:MultiView ID="UserInstructions" runat="server" ActiveViewIndex="0">
            <asp:View ID="default" runat="server">
            <p class="instructions">You may need to <span class="refresh">refresh</span> this page to play the sound again.</p>
            </asp:View>
            <asp:View ID="cantPlayMp3" runat="server">
            <p class="instructions">Oops, sorry, your browser (<%=Request.Browser.Browser %>) cannot play MP3 files via HTML5.  Try IE, Safari, or Chrome.</p>
            </asp:View>
        </asp:MultiView>
        </asp:View>
        <asp:View ID="InvalidSound" runat="server">
        <p class="error">Oops!  Can't load that sound from the system.  Please <a href="mailto:general@otamata.com">tell us about it!</a></p>
        </asp:View>
        </asp:Multiview>
        <p id="aboutOtamata"><a href="<%=AboutOtamataLink %>">Powered by <img id="logo" src="/images/otamata/logo-otamata-100x19.png" alt="Logo" /></a></p>
    </div>
    </div>
    <%-- Todo: figure out if we want to bother with analytics here --%>
    <Otm:Analytics ID="AnalyticsControl" runat="server" Visible="false" />
    <asp:PlaceHolder ID="JavascriptPlaceholder" runat="server" Visible="false">
    <script type="text/javascript">
        function showImg() {
            document.getElementById("soundIconContainer").setAttribute("src", "<%=IconImage %>");
        }
        <%-- // Some javascript to query the status of the player.  For now, don't bother, keep it simple.
        var hasPlayed = false;var hasStartedPlaying = false;
        function isPlaying(player) {
            return document.getElementById(player).currentTime > 0 && !document.getElementById(player).paused && !document.getElementById(player).ended;
        }
        function updateStatus() {
            if (isPlaying("soundPlayer")) {
                hasStartedPlaying = true;
            } else {
                hasPlayed = hasStartedPlaying;
            }
            if (!hasPlayed) {
                setTimeout("updateStatus()", 500);
            }
        }
        updateStatus();
        --%>
        <%-- // Lazy load the image to give the user time to start the sound download.  Helps sound d/l quicker (hypothesis). --%>
        setTimeout("showImg()", 4000);
    </script>
    </asp:PlaceHolder>
</body>
</html>
