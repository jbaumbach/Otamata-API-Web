<%@ Page Language="C#" AutoEventWireup="true" CodeFile="termsandconditions.aspx.cs" Inherits="termsandconditions" %>

<%@ Register TagPrefix="Otm" Src="~/Controls/head.ascx" TagName="Head" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/topnav.ascx" TagName="TopNav" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/footer.ascx" TagName="Footer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <Otm:Head id="HeadContent" runat="server" Title="Otamata - Terms and Conditions"></Otm:Head>
</head>
<body>
    <form id="form1" runat="server">
    <div id="content">
    <Otm:TopNav ID="TopNav" runat="server" H1="Otamata - Terms and Conditions" />
    <div id="home" class="page">
        <div id="mainForm">
        <p>Updated: 2012/02/14</p>
            </p>
            <p>PROVIDING CONTENT TO OTAMATA</p>
            <p>
            You agree and acknowledge that you will only upload Content, including but not limited to information, photos, text, pictures, graphics, illustrations, videos, data, information, etc. 
            (“Content”) to the Site that you are the sole and exclusive copyright owner of. You agree and acknowledge that you are over the age of 18 years of age and that you have the legal right 
            to publish the Content, including posting to the Site. You warrant and represent that you own or control the Content that you upload to the Site. You further agree that you have the legal 
            right to license the Content to Otamata according to the terms as stated herein.
            </p>
            <p>GRANT OF LICENSE</p>
            <p>
            You agree and do hereby grant Otamata an unrestricted right and perpetual license to use the Content in any way and to license the Content to any third-party at any time for any 
            reason and in any medium. The rights granted to Otamata herein include the unrestricted right to reproduce, distribute, make derivative works from, publicly display and publicly 
            perform any and all such Content.
            </p>
            <p>OFFENSIVE CONTENT</p>
            <p>
            No pornography, racial or religious slurs or hateful Content is allowed on Otamata. Otamata reserves the right to remove any Content deemed offensive at any time at it's sole discretion.
        </div>
    </div>
    <Otm:Footer ID="Footer" runat="server" />
    </div>
    </form>
</body>
</html>
