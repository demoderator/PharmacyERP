﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Product_Search_Popup.ascx.cs" Inherits="IMS.UserControl.Product_Search_Popup" %>


<div class="popupMain" id="products">
<div class="popupHead">
    Products List
    <input type="submit" class="close" value="" />
    </div>
     
    
    <asp:GridView ID="StockDisplayGrid" CssClass="table table-striped table-bordered table-condensed" runat="server" AllowPaging="True" PageSize="10" 
                AutoGenerateColumns="false" OnPageIndexChanging="StockDisplayGrid_PageIndexChanging"   onrowcancelingedit="StockDisplayGrid_RowCancelingEdit" 
            onrowcommand="StockDisplayGrid_RowCommand" OnRowDataBound="StockDisplayGrid_RowDataBound" onrowdeleting="StockDisplayGrid_RowDeleting" 
            onrowediting="StockDisplayGrid_RowEditing" OnSelectedIndexChanged="StockDisplayGrid_SelectedIndexChanged" >
                 <Columns>

                     <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkCtrl" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>

                     <asp:BoundField DataField="Product_Name" HeaderText="Product Name"   />

                     <asp:TemplateField HeaderText="UPC">
                        <ItemTemplate>
                            <asp:Label ID="UPC" CssClass="col-md-2 control-label" runat="server" Text='<%# Eval("Product_Id_Org") %>' Width="110px"></asp:Label>
                        </ItemTemplate>
                         <ItemStyle  Width="120px" HorizontalAlign="Left"/>
                    </asp:TemplateField>
                    
                     <asp:BoundField DataField="Description" HeaderText="Description"   /> 
                     <asp:BoundField DataField="itemStrength" HeaderText="Item Strength"   />
                     <asp:BoundField DataField="itemForm" HeaderText="Item Form"   />
                     <asp:BoundField DataField="itemPackSize" HeaderText="Pack Size"   />

                     
                      
                 </Columns>
            <PagerStyle CssClass = "GridPager" />
             </asp:GridView>
     
     <asp:Button ID="SelectProduct" runat="server" CssClass="btn btn-primary fl-r btn-sm" Text="Select" OnClick="SelectProduct_Click"  />
 </div>