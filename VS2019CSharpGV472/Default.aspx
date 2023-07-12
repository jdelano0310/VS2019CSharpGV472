<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VS2019CSharpGV472._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <h2>GridView Test</h2>
    </div>
    
    <div class="row">

        <asp:updatepanel id="upGridViews" UpdateMode="Conditional" class="col-md-4" runat="server">
            <ContentTemplate>
                <div class="row">
                    <asp:Button ID="btnAddRow1" runat="server" Text="Add Row" OnClick="btnAddRow_Click" usesubmitbehavior="false"/>
                    <asp:Button ID="btnAddGrid1" runat="server" Text="Add Grid" OnClick="btnAddGrid_Click" usesubmitbehavior="false"/>
                    <asp:DropDownList ID="ddlProducts1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProducts_SelectedIndexChanged" >
                    </asp:DropDownList>
                </div>
                <asp:GridView ID="gv1" runat="server" AutoGenerateColumns="False">
                    
                    <Columns>
                        <asp:TemplateField HeaderText="Category">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlCategory1" runat="server">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Amount">
                            <Itemtemplate>
                                <asp:TextBox ID="txtAmount" runat="server"></asp:TextBox>
                            </Itemtemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="TaxAmount" ItemStyle-Width="100px">
                            <Itemtemplate>
                                <asp:Label ID="lblTaxAmount" runat="server"></asp:Label>
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
                    <asp:Label ID="lbGridCount" runat="server" Text="Grid Count: 1"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
