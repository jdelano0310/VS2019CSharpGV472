﻿<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VS2019CSharpGV472._Default" EnableViewState="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <h2>GridView Test</h2>
    </div>
    
    <div class="row">

                <div class="col-md-4" id="divGridSection" runat="server">
                    <asp:Button ID="btnAddRow1" runat="server" Text="Add Row" OnClick="btnAddRow_Click" usesubmitbehavior="false"/>
                    <asp:Button ID="btnAddGrid1" runat="server" Text="Add Grid" OnClick="btnAddGrid_Click" usesubmitbehavior="false"/>
                    <asp:DropDownList ID="ddlProducts1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProducts_SelectedIndexChanged" >
                    </asp:DropDownList>
                </div>
                <asp:GridView ID="gv1" runat="server" AutoGenerateColumns="False" EmptyDataText="No Data Exists Yet">
                    
                    <Columns>
                        <asp:TemplateField HeaderText="Category">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlCategory1" runat="server">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CategoryID" HeaderText="CategoryID" ReadOnly="True" Visible="False" />
                        <asp:BoundField DataField="Amount" HeaderText="Amount" />
                        <asp:BoundField DataField="TaxAmount" HeaderText="Tax Amount" ReadOnly="True" />
                    </Columns>
                    
                </asp:GridView>
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
