<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VS2019CSharpGV472._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <h2>GridView Test</h2>
    </div>

    <div class="row">
        <div class="col-md-4">
            <asp:Button ID="btnAddGrid" runat="server" Text="Add Grid" OnClick="btnAddGrid_Click" />
            <asp:Button ID="btnAddRow" runat="server" Text="Add Row" OnClick="btnAddRow_Click" />
        </div>
    </div>

    <div class="row">
        <div id="divGridView" class="col-md-4" runat="server">
            <asp:GridView ID="gv1" runat="server"></asp:GridView>
        </div>
    </div>

    <div class="row">
        <div class="col-md-4">
            <asp:Label ID="lbCurrentGrid" runat="server" Text="Current: First Grid"></asp:Label>
        </div>
    </div>

</asp:Content>
