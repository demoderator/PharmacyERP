﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewSalesOrders.aspx.cs" Inherits="IMS.ViewSalesOrders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <%-- <script src="Scripts/jquery-ui.js" type="text/javascript"></script>
          <link rel="stylesheet" href="Style/jquery-ui.css" />
          <script>
              $(function () { $("#<%= DateTextBox.ClientID %>").datepicker(); });

          </script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     
    <table width="100%">

        <tbody><tr>
        	<td> <h4>View Sale Orders</h4></td>
            <td align="right">
            <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Enabled="true" Text="SEARCH" CssClass="btn btn-primary btn-default"/>
            <asp:Button ID="btnRefresh" runat="server" OnClick="btnRefresh_Click" Enabled="true" Text="REFRESH" CssClass="btn btn-info"/>
            <asp:Button ID="btnBack" runat="server" CssClass="btn btn-default btn-large" Text="Go Back" OnClick="btnBack_Click"/>    
            </td>
        </tr>
		<tr><td height="5"></td></tr>
    </tbody>

    </table>
     <hr>
     <script src="Scripts/jquery.js"  type="text/javascript"></script>
          <script src="Scripts/jquery-ui.js" type="text/javascript"></script>
          <link rel="stylesheet" href="Style/jquery-ui.css" />
          <script>
              $(function () { $("#<%= DateTextBox.ClientID %>").datepicker(); });

          </script>

       

   <table width="100%" class="formTbl">

        <tr>
            <td><asp:Label runat="server" AssociatedControlID="StockAt" CssClass="control-label">Pharmacy Name </asp:Label></td>
            <td>
                <asp:DropDownList runat="server" ID="StockAt" CssClass="form-control product" Width="29%" AutoPostBack="True" OnSelectedIndexChanged="StockAt_SelectedIndexChanged" />

                <!--DataSourceID="StockAtDataSource" DataTextField="SystemName" DataValueField="SystemID"-->
                <!--<asp:SqlDataSource ID="StockAtDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:IMSConnectionString %>" SelectCommand="SELECT [SystemID], [SystemName] FROM [tbl_System]"></asp:SqlDataSource>-->

                <asp:TextBox runat="server" ID="SelectProduct" CssClass="form-control product" Visible="false"/>
                <asp:ImageButton ID="btnSearchProduct" Visible="false" runat="server" OnClick="btnSearchProduct_Click" Text="SearchProduct" Height="30px" ImageUrl="~/Images/search-icon-512.png" Width="45px" />
               
                
               

            </td>
            <td>
                 <asp:Label runat="server" AssociatedControlID="DateTextBox"  CssClass="control-label">Order Date</asp:Label>
            </td>
            <td>
                <asp:TextBox runat="server" ID="DateTextBox" CssClass="form-control" />
            </td>
        </tr>
        <tr>
            <td><asp:Label runat="server" AssociatedControlID="txtOrderNO"  CssClass="control-label">Order Number</asp:Label></td>
            <td> <asp:TextBox runat="server" ID="txtOrderNO" CssClass="form-control" /></td>
            <td>
                <asp:Label runat="server" AssociatedControlID="OrderStatus" CssClass="control-label">Order Status </asp:Label>
            </td>
            <td>
                <asp:DropDownList runat="server" ID="OrderStatus" CssClass="form-control" Width="29%" AutoPostBack="True" OnSelectedIndexChanged="OrderStatus_SelectedIndexChanged"/>
            </td>
        </tr>
       
       

    </table>

    
    <br />
       <asp:GridView ID="StockDisplayGrid" CssClass="table table-striped table-bordered table-condensed"  Visible="true" runat="server" AllowPaging="True" PageSize="10" 
                AutoGenerateColumns="false" OnSelectedIndexChanged="StockDisplayGrid_SelectedIndexChanged" OnPageIndexChanging="StockDisplayGrid_PageIndexChanging"   onrowcancelingedit="StockDisplayGrid_RowCancelingEdit" 
                onrowcommand="StockDisplayGrid_RowCommand" onrowediting="StockDisplayGrid_RowEditing"  OnRowDataBound="StockDisplayGrid_RowDataBound" OnRowDeleting="StockDisplayGrid_RowDeleting">
                 <Columns>
                     <asp:TemplateField HeaderText="Action" HeaderStyle-Width ="150px">
                        <ItemTemplate>
                            <asp:Button CssClass="btn btn-default edit-btn" ID="btnEdit" Text="Edit" runat="server" CommandName="Edit" CommandArgument='<%# Container.DisplayIndex  %>'   Visible='<%# IsStatusPending((String) Eval("Status")) %>'/>
                            <span onclick="return confirm('Are you sure you want to delete this record?')">
                                <asp:Button CssClass="btn btn-default del-btn" ID="btnDelete" Text="Delete" runat="server" CommandName="Delete" CommandArgument='<%# Container.DisplayIndex  %>'/>
                            </span>
                              </ItemTemplate>
                         <ItemStyle  Width="150px" HorizontalAlign="Left"/>
                    </asp:TemplateField>

                      <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="SalesManID" CssClass="control-label" runat="server" Text='<%# Eval("SalesManID") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle  Width="110px" HorizontalAlign="Left"/>

                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Order No." HeaderStyle-Width ="110px">
                        <ItemTemplate>
                            <asp:Label ID="OrderNO" CssClass="control-label" runat="server" Text='<%# Eval("OrderID") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle  Width="110px" HorizontalAlign="Left"/>

                    </asp:TemplateField>
                     
                      <asp:TemplateField HeaderText="Invoice" Visible="false" HeaderStyle-Width ="150px">
                        <ItemTemplate>
                            <asp:Label ID="Invoice" CssClass="col-md-2 control-label" runat="server" Text='<%# Eval("InvoiceNumber") %>'  Width="180px"></asp:Label>
                        </ItemTemplate>
                        <ItemStyle  Width="180px" HorizontalAlign="Left"/>
                      </asp:TemplateField>
                     
                     <asp:TemplateField HeaderText="SystemID" Visible="false" HeaderStyle-Width ="150px">
                        <ItemTemplate>
                            <asp:Label ID="SystemID" CssClass="col-md-2 control-label" runat="server" Text='<%# Eval("SystemID") %>'  Width="180px"></asp:Label>
                        </ItemTemplate>
                        <ItemStyle  Width="180px" HorizontalAlign="Left"/>
                      </asp:TemplateField>
                     <asp:TemplateField HeaderText="Order Date" HeaderStyle-Width ="150px">
                        <ItemTemplate>
                            <asp:Label ID="OrderDate" CssClass="control-label" runat="server" Text='<%# Eval("OrderDate") %>' ></asp:Label>
                        </ItemTemplate>
                        <ItemStyle  Width="180px" HorizontalAlign="Left"/>
                    </asp:TemplateField>

                     <asp:TemplateField HeaderText="Order To" HeaderStyle-Width ="200px">
                        <ItemTemplate>
                            <asp:Label ID="OrderTo" CssClass="control-label" runat="server" Text='<%# Eval("Location") %>'></asp:Label>
                        </ItemTemplate>
                         <ItemStyle  Width="300px" HorizontalAlign="Left"/>
                    </asp:TemplateField>

                     <asp:TemplateField HeaderText="Order Status" HeaderStyle-Width ="130px">
                        <ItemTemplate>
                            <asp:Label ID="OrderStatus" CssClass="control-label" runat="server" Text='<%# Eval("Status") %>' ></asp:Label>
                        </ItemTemplate>
                          <ItemStyle  Width="130px" HorizontalAlign="Left"/>
                    </asp:TemplateField>
                    
                     
                 </Columns>
                    <PagerStyle CssClass = "GridPager" />
             </asp:GridView>
   
    
</asp:Content>
