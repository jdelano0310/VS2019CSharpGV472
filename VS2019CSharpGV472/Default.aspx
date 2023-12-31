﻿<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VS2019CSharpGV472._Default" EnableViewState="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <h2>GridView Test</h2>
    </div>
    
    <div class="row">
        <asp:updatepanel id="upGridViews" UpdateMode="Conditional" class="col-md-4" runat="server">
            <ContentTemplate>
                <asp:panel id="divControlSet" runat="server" style="margin-bottom: 10px">
                    <div class="row">
                        <asp:Button ID="btnAddRow1" runat="server" Text="Add Row" OnClick="btnAddRow_Click" usesubmitbehavior="false"/>
                        <asp:Button ID="btnAddGrid1" runat="server" Text="Add Grid" OnClick="btnAddGrid_Click" usesubmitbehavior="false"/>
                        <asp:DropDownList ID="ddlProducts1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProducts_SelectedIndexChanged" >
                        </asp:DropDownList>
                    </div>
                    <asp:GridView ID="gv1" runat="server" AutoGenerateColumns="False" EmptyDataText="No Data Exists Yet">
                    
                        <Columns>
                            <asp:TemplateField HeaderText="Category">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlCategory" runat="server">
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="CategoryID" ReadOnly="True" >
                                <HeaderStyle Width="0px" />
                                <ItemStyle Width="0px" Font-Size="0pt" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Amount">
                                <ItemTemplate>
                                    <asp:TextBox runat="server" ID="txtAmount" Text='<%# Eval("Amount") %>'></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField  DataField="TaxAmount" HeaderText="Tax Amount" ReadOnly="True" >
                            <ItemStyle Width="200px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="SelectedMarkUpType" ReadOnly="True" >
                                <HeaderStyle Width="0px" />
                                <ItemStyle Width="0px" Font-Size="0pt" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Markup Type">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlMarkUpType" runat="server">
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    
                    </asp:GridView>
                </asp:panel>
                <p></p>
            </ContentTemplate>
        </asp:updatepanel>
    </div>

</asp:Content>
