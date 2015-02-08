<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_default" %>

<%-- 

Otamata Website - Todos

* Implement "no-cache" pragma headers so that form can't be cached by the browser (help prevent double submit)
* Add more RAM to hosted server
* Get this url to work: http://otamata.com/
* Add version number to content dirs (like Fandango) so that browser cache clearing is easier (prolly adding a url rewrite)
* Start creating unit/integration tests

Maybe

* Textbox for link to more info about the item (e.g. link to movies.com).  
* Allow for cropping of an image.  Use the yahoo cropper: http://developer.yahoo.com/yui/imagecropper/
* Allow any size image, and resize/crop on the server: http://imagemagick.codeplex.com/

Done

* Change doctype to be HTML5
* Finish up legal language
* Update stored procs to record uuid and program version
* Create google analytics account for otamata
* Update app icon - verify all colors: http://colorschemedesigner.com/#2Y61Aw0w0w0w0
* Add fav icon
* Add language about obscene, porn, racist, hate content



--%>

<%@ Register TagPrefix="Otm" Src="~/Controls/mainmenu.ascx" TagName="MainMenu" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/head.ascx" TagName="Head" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/topnav.ascx" TagName="TopNav" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/footer.ascx" TagName="Footer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <Otm:Head id="HeadContent" runat="server" Title="Otamata"></Otm:Head>
</head>
<body>
    <form id="form1" runat="server">
    <div id="content">
    <Otm:TopNav ID="TopNav" runat="server" H1="Otamata" />
    <div id="home" class="page">
        <p>Enter your sound's information into the fields below.</p>
        <div id="mainForm">
            <p class="status <asp:Literal id="statusClassLiteral" runat="server">statusNone</asp:Literal>"><asp:Label ID="lblStatus" runat="server">Status</asp:Label></p>
            <ul>
                <li><span>Sound Name*</span> <asp:TextBox ID="txtName" runat="server" MaxLength="25" Width="180"></asp:TextBox></li>
                <li><span>Sound Description*</span> <asp:TextBox ID="txtDescription" runat="server" MaxLength="140" Width="290"></asp:TextBox></li>
                <li><span>Your Name/Handle*</span> <asp:TextBox ID="txtUploadby" runat="server" MaxLength="50"></asp:TextBox></li>
                <li><span>File*</span> <asp:FileUpload ID="flSoundFile" runat="server" /> <span class="fineprint">(Max <%=SoundFileLengthK %>K, .wav/.mp3)</span></li>
                <li><span>Icon<% = OttaMatta.Common.Config.RequireIconUpload ? "*" : "" %></span> <asp:FileUpload ID="flIconFile" runat="server" /> <span class="fineprint">(Max <%=IconFileLengthK %>K, .png/.jpg/.gif, square shape)</span></li>
                <li><asp:CheckBox ID="chkTermsAndConditions" runat="server" Text="I certify that I have fully read, understand, and comply with the <a href='termsandconditions.aspx' target='_blank'>Otamata Terms and Conditions</a>." /></li>
                <li class="fineprint">* = Required Fields</li>
            </ul>
            <asp:Button UseSubmitBehavior="true" runat="server" ID="btnSubmit" Text="Add to Otamata" 
                    onclick="btnSubmit_Click" />
        </div>
    </div>
    <Otm:MainMenu ID="MainMenu" runat="server" />
    <Otm:Footer ID="Footer" runat="server" />
    </div>
    </form>
</body>
</html>
