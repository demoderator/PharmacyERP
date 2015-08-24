﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_PendingSalesOrderPopUp.ascx.cs" Inherits="IMS.UserControl.uc_PendingSalesOrderPopUp" %>


 <div class="popupMain" id="products">

    <div class="popupHead">
    Queued SOs for Generation
    <input type="submit" class="close" value="" />
    </div>
     
      <div class="bodyPop">
          
    <asp:GridView ID="gdvPendingSOs" runat="server" Width="100%" CssClass="table table-striped table-bordered table-condensed" AllowPaging="True" PageSize="10"
        AutoGenerateColumns="false" OnRowDataBound="gdvPendingSOs_RowDataBound" OnPageIndexChanging="gdvPendingSOs_PageIndexChanging" ShowFooter="true"
        OnRowCommand="gdvPendingSOs_RowCommand" OnRowDeleting="gdvPendingSOs_RowDeleting" OnRowEditing="gdvPendingSOs_RowEditing">
        <Columns>
           
             <asp:TemplateField HeaderText="Action">
                <ItemTemplate >
                     
                    <asp:CheckBox ID="chkCtrl" runat="server" onClick="CheckSingleCheckbox(this);"  />
                   
                </ItemTemplate>
            </asp:TemplateField>
            
          

            <asp:BoundField DataField="ProductName" HeaderText="Product Name"   />
           <asp:BoundField DataField="FromPlace" HeaderText="Order From"   />
              <asp:TemplateField HeaderText="OrderDetailID" Visible="false">
                <ItemTemplate>
                    <asp:Label ID="lblOrderDetailID" runat="server" Text='<%# Eval("OrderDetailID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="OrderID" Visible="false">
                <ItemTemplate>
                    <asp:Label ID="lblOrderID" runat="server" Text='<%# Eval("OrderID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
			
			   <asp:TemplateField HeaderText="Order To">
                <ItemTemplate>
                    <asp:Label ID="lblOrderTo" runat="server" Text='<%# Eval("ToPlace") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ProductID" Visible="false">
                <ItemTemplate>
                    <asp:Label ID="lblProductID" runat="server" Text='<%# Eval("ProductID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="AvailableStock">
                <ItemTemplate>
                    <asp:Label ID="lblAvailableStock" runat="server" Text='<%# Eval("AvailableStock") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Qauntity Ordered">
                <ItemTemplate>
                    <asp:Label ID="lblQauntity" runat="server" Text='<%# Eval("Qauntity") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Bonus Qty">
                <ItemTemplate>
                    <asp:Label ID="lblBonus" runat="server" Text='<%# Eval("Bonus") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Discount">
                <ItemTemplate>
                    <asp:Label ID="lblDiscount" runat="server" Text='<%# Eval("Discount") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
             <asp:TemplateField HeaderText="Order Requested For" Visible="false">
                <ItemTemplate>
                    <asp:Label ID="lblOrderRequestedFor" runat="server" Text='<%# Eval("OrderRequestedFor") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
         
        </Columns>
        <PagerStyle CssClass="GridPager" />


    </asp:GridView>
    
     <asp:Button ID="SelectSalesOrder" runat="server" CssClass="btn btn-primary fl-r btn-sm" Text="Select" OnClick="SelectSalesOrder_Click"  />
 </div>
</div>

 <script type="text/javascript">
     function CheckSingleCheckbox(ob) {
         var grid = ob.parentNode.parentNode.parentNode;
         var inputs = grid.getElementsByTagName("input");
         for (var i = 0; i < inputs.length; i++) {
             if (inputs[i].type == "checkbox") {
                 if (ob.checked && inputs[i] != ob && inputs[i].checked) {
                     inputs[i].checked = false;
                 }
             }
         }
     }
    </script>
