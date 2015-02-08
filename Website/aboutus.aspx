<%@ Page Language="C#" AutoEventWireup="true" CodeFile="aboutus.aspx.cs" Inherits="aboutus" %>

<%@ Register TagPrefix="Otm" Src="~/Controls/mainmenu.ascx" TagName="MainMenu" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/head.ascx" TagName="Head" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/topnav.ascx" TagName="TopNav" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/footer.ascx" TagName="Footer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <Otm:Head id="HeadContent" runat="server" Title="Otamata - About Us"></Otm:Head>
</head>
<body>
    <form id="form1" runat="server">
    <div id="content">
    <Otm:TopNav ID="TopNav" runat="server" H1="Otamata - About Us" />
    <div id="home" class="page">
        <p>The goal of Otamata is to be the premier mobile app for playing sound effects. Otamata is one of the few apps that allows you to upload your own sounds to our system for downloading on your
        device.  Our first app is on the iPhone/iPod Touch platform, with Android versions coming in the future.</p>

        <p>Use any short WAV or MP3 sound that you can dream up, and upload it to our servers with an icon of your choosing.  Also browse and download any of the sounds in our growing social community.  Click the app store icon above for screenshots and the current feature list.</p>

        <p>Please email us any feature requests, bug reports, or general questions to: <a href="mailto:general@otamata.com">general@otamata.com</a></p>

    </div>
    <Otm:MainMenu ID="MainMenu" runat="server" />
    <Otm:Footer ID="Footer" runat="server" />
    </div>
    </form>
</body>
</html>
