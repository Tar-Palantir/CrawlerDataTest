<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="CrawlerDataTest.Pages.HomePage"
ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="txtUrl" runat="server" Text="http://" Width="400px"></asp:TextBox>
        <asp:Button ID="Start" runat="server" Text="Start" onclick="Start_Click" />
        <br />
    </div>
    <div>
        <asp:TextBox ID="txtPostUrl" runat="server" Text="http://" Width="400px"></asp:TextBox>
        <asp:TextBox ID="txtParams" runat="server" Text="" Width="400px"></asp:TextBox>
        <asp:Button ID="PostStart" runat="server" Text="Start" onclick="PostStart_Click" />
        <asp:TextBox ID="txtData" Width="862px" runat="server" TextMode="MultiLine" 
            Height="317px"></asp:TextBox>
    </div>
    </form>
</body>
</html>
