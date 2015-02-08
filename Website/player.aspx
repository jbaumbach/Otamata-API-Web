<%@ Page Language="C#" AutoEventWireup="true" CodeFile="player.aspx.cs" Inherits="player" EnableEventValidation="false" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/analytics.ascx" TagName="Analytics" %>
<%@ Import Namespace="OttaMatta.Common" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <%-- Responsive web design: http://www.sitepoint.com/responsive-web-design/#fbid=OBcGeSLN3i1  --%>
    <meta name="viewport" content="width=device-width">

    <link href="<%=ContentServer %>/styles/player.css" media="screen,projection" type="text/css" rel="Stylesheet" />
    <title><%=Title %></title>
</head>
<body>
    <div id="content">
    <div id="player" class="page">
        <h1>Otamata Soundplayer</h1>
        <asp:Multiview ID="PlayerMultiview" runat="server" ActiveViewIndex="0">
        <asp:View ID="ValidSound" runat="server">
        <asp:PlaceHolder ID="SoundDetailsPlaceholder" runat="server" Visible="false">
        <img id="soundIconContainer" src="<%=HourglassImage %>" width="95" height="95" />
        <ul id="soundDetail">
        <li><strong><%=theSound.Name %></strong></li>
        <li><em><%=theSound.Description %></em></li>
        </ul>
        <ul class="clear">
        <li>Added to Otamata by <strong><%=theSound.UploadBy %></strong> <%=((DateTime)theSound.UploadDate).ToLongDateString() %></li>
        </ul>
        </asp:PlaceHolder>
        <asp:MultiView ID="PlayerControlsMultiview" runat="server" ActiveViewIndex="0">
            <asp:View ID="HTML5Player" runat="server">
            <audio id="soundPlayer" controls="controls" src="<%=SoundSrcData %>" preload="auto" autoplay="autoplay">
                This feature requires a browser that supports <a href="http://en.wikipedia.org/wiki/HTML5">HTML5</a>!  Sorry.  It was a nice try though.
            </audio>
            </asp:View>
            <asp:View ID="plainLink" runat="server">
            <p id="playLink"><a href="<%=SoundSrcData %>">Click to Play</a></p>
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
        <p id="aboutOtamata"><a href="<%=AboutOtamataLink %>">What is this?</a></p>
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
