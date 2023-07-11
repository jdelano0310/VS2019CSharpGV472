<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VS2019CSharpGV472._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <h2>GridView Test</h2>
    </div>

    <div class="row">
        <asp:updatepanel id="Updatepanel2" UpdateMode="Conditional" class="col-md-4" runat="server">
            <ContentTemplate>
                <asp:Button ID="btnAddRow" runat="server" Text="Add Row" OnClick="btnAddRow_Click" usesubmitbehavior="false"/>
                <asp:Button ID="btnAddGrid" runat="server" Text="Add Grid" OnClick="btnAddGrid_Click" usesubmitbehavior="false"/>
            </ContentTemplate>
        </asp:updatepanel>
    </div>
    
    <div class="row">
        <asp:updatepanel id="upGridViews" UpdateMode="Conditional" class="col-md-4" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gv1" runat="server" AutoGenerateColumns="False">
                    
                    <Columns>
                        <asp:TemplateField HeaderText="Travel Type">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlTravelType" runat="server">
                                    <asp:ListItem>Cruise</asp:ListItem>
                                    <asp:ListItem>Flight</asp:ListItem>
                                    <asp:ListItem>Train</asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cost">
                            <Itemtemplate>
                                <asp:TextBox ID="txtCost" runat="server"></asp:TextBox>
                            </Itemtemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total" ItemStyle-Width="100px">
                            <Itemtemplate>
                                <asp:Label ID="lblTotalCost" runat="server"></asp:Label>
                            </Itemtemplate>
                        </asp:TemplateField>
                    </Columns>
                    
                </asp:GridView>
            </ContentTemplate>
        </asp:updatepanel>
    </div>

    <div class="row">
        <div class="col-md-4">
            <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <asp:Label ID="lbCurrentGrid" runat="server" Text="Current: First Grid"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
