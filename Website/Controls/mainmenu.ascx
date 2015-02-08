<%@ Control Language="C#" AutoEventWireup="true" CodeFile="mainmenu.ascx.cs" Inherits="Controls_mainmenu" %>

<ul id="maimmenu">
<li><asp:HyperLink ID="uploadLink" runat="server" NavigateUrl="~/default.aspx">Upload Sound</asp:HyperLink></li>
<li><asp:HyperLink ID="aboutUs" runat="server" NavigateUrl="~/aboutus.aspx">About Us</asp:HyperLink></li>
<li><asp:HyperLink ID="termsandconditions" runat="server" NavigateUrl="~/termsandconditions.aspx" Target="_blank">Terms and Conditions</asp:HyperLink></li>
<li><asp:HyperLink ID="dmca" runat="server" NavigateUrl="~/dmca.aspx">DMCA Compliance</asp:HyperLink></li>
</ul>