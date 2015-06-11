﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReplenishMovement.aspx.cs" Inherits="IMS.ReplenishMovement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="body-cont">
            

		
     
	  
      <table width="100%">

        <tbody><tr>
        	<td> <h4 id="topHead">Replenish ( Movement )</h4></td>
            <td align="right">
            
           <!-- onClick="window.location.href='purchase-order.html'" -->

		  <asp:Button ID="btnBack" runat="server" CssClass="btn btn-default" Text="Back" OnClick="btnBack_Click" />
		    </td>
        </tr>
		<tr><td height="5"></td></tr>
    </tbody></table>
      <hr>
        <asp:GridView ID="gvVendorNames" runat="server" OnRowDataBound="gvVendorNames_RowDataBound" 
                      OnSelectedIndexChanged ="gvVendorNames_SelectedIndexChanged" OnPageIndexChanging="gvVendorNames_PageIndexChanging"
                     AutoGenerateColumns="false" BorderWidth="0px" Width="100%">
            <Columns>                
                <asp:TemplateField>
                    <ItemTemplate>
                        <table class="table table-striped table-bordered table-condensed tblBtm vStockGrid" width:"100%">
		                <tbody>
                        <tr>
        	                <td colspan="6">
                                    <asp:Label ID="hdnVendorID" runat="server" Visible="false" Text='<%# Eval("VendorID") %>' CommandArgument='<%# Container.DataItemIndex %>'/>
    		                        <h4 class="fl-l"><%#Eval("VendorName") %></h4>
                                    <asp:Button ID="btnCreatePO" runat="server" CommandName="CreatePO"  CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-success fl-r" Text="Create PO"/>
                            </td>
                        </tr>
                                 <asp:GridView ID="gvVendorProducts" runat="server" OnRowDataBound="gvVendorProducts_RowDataBound" 
                                      OnSelectedIndexChanged="gvVendorProducts_SelectedIndexChanged" OnRowCommand="gvVendorProducts_RowCommand"
                                      OnPageIndexChanging="gvVendorProducts_PageIndexChanging" OnRowEditing="gvVendorProducts_RowEditing"
                                      OnRowCancelingEdit="gvVendorProducts_RowCancelingEdit" OnRowDeleting="gvVendorProducts_RowDeleting"
                                       AutoGenerateColumns="false" CssClass="table table-striped table-bordered table-condensed tblBtm vStockGrid" BorderWidth="0px">

                                 <Columns>
                                     <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblProductID" runat="server" Text='<%# Eval("ProductID") %>' CommandArgument='<%# Container.DataItemIndex %>' ></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle  Width="1px" HorizontalAlign="Left"/>
                                    </asp:TemplateField>

                                     <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblVendorID" runat="server" Text='<%# Eval("VendorID") %>'  CommandArgument='<%# Container.DataItemIndex %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle  Width="1px" HorizontalAlign="Left"/>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:Button CssClass="btn btn-default edit-btn editPOV" ID="btnEdit" runat="server" 
                                                            CommandName="Edit"  CommandArgument='<%# Container.DataItemIndex %>'/>

                                            <span onclick="return confirm('Are you sure you want to delete this record?')">
                                            <asp:Button CssClass="btn btn-default del-btn" ID="btnDelete" runat="server" 
                                                          CommandName="Delete"  CommandArgument='<%# Eval("ProductID").ToString() + "," + Eval("VendorID").ToString() %>'/>
                                            </span>
                                        </ItemTemplate>

                                        <EditItemTemplate>

                                        <asp:LinkButton CssClass="cancelPOE btn btn-primary btn-xs" ID="btnUpdate" Text="Update" runat="server" CommandName="UpdateStock"
                                                        CommandArgument='<%# Eval("ProductID").ToString() + "," + Eval("VendorID").ToString() %>'/>

                                        <asp:LinkButton CssClass="btn btn-default btn-xs cancelPOE" ID="btnCancel" Text="Cancel" runat="server" CommandName="Cancel" />

                                        </EditItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Product Description">
                                        <ItemTemplate>
                                            <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("Description") %>' CommandArgument='<%# Container.DataItemIndex %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Quantity">
                                        <ItemTemplate>
                                            <asp:Label ID="lblProductQuantity" runat="server" Text='<%# Eval("QtySold") %>' CommandArgument='<%# Container.DataItemIndex %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                        <asp:TextBox ID="txtQuantity" CssClass="grid-input-form" runat="server" Text='<%#Eval("QtySold") %>' ></asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateField>

                                     <asp:TemplateField HeaderText="Assign to different Vendor" HeaderStyle-Width="19%">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlPreviousVendors" runat="server" CssClass="atds" 
                                                CommandName="DropDown"  CommandArgument='<%# Container.DataItemIndex %>' AutoPostBack="true" OnSelectedIndexChanged="ddlPreviousVendors_SelectedIndexChanged"></asp:DropDownList>

                                             <asp:Button ID="btnAddNewVendor" runat="server" CommandName="NewVendor"  CommandArgument='<%# Container.DataItemIndex %>' 
                                                 CssClass="btn btn-info btn-sm opPop" Text="ADD"/>

                                        </ItemTemplate>
                                        <ItemStyle  Width="19%" HorizontalAlign="Left"/>
                                    </asp:TemplateField>

                                </Columns>
                        </asp:GridView>
                            </td></tr>
                        </table>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>
      </div>

	<script src="Scripts/jquery-ui.js"></script>
    <script src="Scripts/general.js"></script>

</asp:Content>