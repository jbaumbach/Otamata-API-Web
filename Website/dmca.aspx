<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dmca.aspx.cs" Inherits="dmca" %>

<%@ Register TagPrefix="Otm" Src="~/Controls/mainmenu.ascx" TagName="MainMenu" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/head.ascx" TagName="Head" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/topnav.ascx" TagName="TopNav" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/footer.ascx" TagName="Footer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <Otm:Head id="HeadContent" runat="server" Title="Otamata - DMCA Compliance"></Otm:Head>
</head>
<body>
    <form id="form1" runat="server">
    <div id="content">
    <Otm:TopNav ID="TopNav" runat="server" H1="Otamata - DMCA Compliance" />
    <div id="home" class="page">
            
        <p>DMCA COMPLIANCE</p>
        <p>Otamata deals with alleged copyright violations on its websites and applictions in accordance with the policies and procedures stated herein and pursuant to the 
        United States Online Copyright Infringement Liability Limitation Act of the <a href="http://en.wikipedia.org/wiki/Digital_Millennium_Copyright_Act" target="_blank">Digital Millennium Copyright Act (DMCA)</a> of 1998 (17 USC Section 512). Therefore Otamata removes 
        allegedly infringing content under any valid DMCA notification or court order.</p>

        <p>Complaints, Notices and Counter-Notices of alleged infringement pursuant to the DMCA should be sent to:</p>

        <p><pre>
        Otamata
        Email: general@otamata.com
        Subject: DMCA Copyright Complaint
        </pre></p>

        <p>When submitting a notification of potential copyright violation(s) under the provisions of DMCA Title II, please include a screenshot of the detail page of the content in question.</p>

        <p>Any person who knowingly and materially misrepresents that activity or content is infringing can be subject to legal liability under the DMCA and other applicable laws.</p>

        <p>Otamata or its affiliates may provide copies of any copyright notices or counter-notices to the participants in the dispute or to any other third-parties in Otamata’s sole discretion and as may be required by law. You are encouraged to consult legal counsel prior to filing a DMCA notification to meet the legal requirements for filing a claim.</p>

        </div>
    <Otm:MainMenu ID="MainMenu" runat="server" />
    <Otm:Footer ID="Footer" runat="server" />
    </div>
    </form>
</body>
</html>
