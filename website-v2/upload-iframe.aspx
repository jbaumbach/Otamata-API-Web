<%@ Page Language="C#" AutoEventWireup="true" CodeFile="upload-iframe.aspx.cs" Inherits="upload_iframe" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<link rel="stylesheet" type="text/css" href="stylesheets/upload.css" />
	<link rel="stylesheet" type="text/css" href="http://fonts.googleapis.com/css?family=Nothing+You+Could+Do|Quicksand:400,700,300" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="upload">
        <h1>Upload</h1>
        <p>You can upload sounds from your computer or file-system enabled device here.  Then, search for them in Otamata on your device to play or share.</p>
        <%--
        --%>
        <p class="instructions">Enter your sound's information into the fields below.</p>
        <div id="mainForm">
            <p class="status <asp:Literal id="statusClassLiteral" runat="server">statusNone</asp:Literal>"><asp:Label ID="lblStatus" runat="server">Status</asp:Label></p>
            <ul>
                <li><span>Sound Name*</span> <asp:TextBox ID="txtName" runat="server" MaxLength="25" Width="180"></asp:TextBox></li>
                <li><span>Sound Description*</span> <asp:TextBox ID="txtDescription" runat="server" MaxLength="140" Width="290"></asp:TextBox></li>
                <li><span>Your Name/Handle*</span> <asp:TextBox ID="txtUploadby" runat="server" MaxLength="50"></asp:TextBox></li>
                <li><span>Sound*</span> <asp:FileUpload ID="flSoundFile" runat="server" /> <span class="fineprint">(Max <%=SoundFileLengthK %>K, .wav/.mp3)</span></li>
                <li><span>Icon<% = OttaMatta.Common.Config.RequireIconUpload ? "*" : "" %></span> <asp:FileUpload ID="flIconFile" runat="server" /> <span class="fineprint">(Max <%=IconFileLengthK %>K, .png/.jpg/.gif, square shape)</span></li>
                <li><asp:CheckBox ID="chkTermsAndConditions" runat="server" Text="I certify that I have fully read, understand, and comply with the <a href='termsandconditions.aspx' target='_blank'>Otamata Terms and Conditions</a>." /></li>
                <li class="fineprint">* = Required Fields</li>
            </ul>
            <asp:Button UseSubmitBehavior="true" runat="server" ID="btnSubmit" Text="Add to Otamata" 
                    onclick="btnSubmit_Click" />
        </div>
    </div>
    </form>
</body>
</html>
