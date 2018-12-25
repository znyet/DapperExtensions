<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Test.index" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="float: left; width: 600px; height: 600px;">


            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Insert" />
            <br />
            <br />
            <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="InsertAsync" />
            <br />
            <br />
            <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="InsertIdentityKey" />


            <br />
            <br />
            <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="DeleteByIds" />


            <br />
            <br />
            <asp:Button ID="Button5" runat="server" OnClick="Button5_Click" Text="BulkCopy" />
            <br />
            <br />
            <asp:Button ID="Button6" runat="server" OnClick="Button6_Click" Text="BulkUpdate" />


        </div>

        <div style="float: left">
            <asp:TextBox ID="TextBox1" runat="server" Height="586px" style="margin-left: 0px" TextMode="MultiLine" Width="467px"></asp:TextBox>
        </div>
    </form>
</body>
</html>
