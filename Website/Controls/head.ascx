<%@ Control Language="C#" AutoEventWireup="true" CodeFile="head.ascx.cs" Inherits="Controls_head" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/analytics.ascx" TagName="Analytics" %>

    <title><%=Title %></title>
    <link rel="icon" href="<%=ServerName %>/images/frog-favicon.png" type="image/png" />
    <link rel="shortcut icon" href="<%=ServerName %>/images/frog-favicon.png" type="image/png" />
    <link href="<%=ContentServer %>/styles/global.css" media="screen,projection" type="text/css" rel="Stylesheet" />
    <script type="text/javascript" src="<%=ContentServer %>/scripts/global.js"></script>
    <meta name="viewport" content="width=725"/>
    <Otm:Analytics ID="AnalyticsControl" runat="server" />
