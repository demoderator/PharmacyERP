﻿using IMS.Util;
using IMSCommon.Util;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IMS
{
    public partial class ItemRequestWHPH : System.Web.UI.Page
    {
        public static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSConnectionString"].ToString());
        private ILog log;
        private string pageURL;
        private ExceptionHandler expHandler = ExceptionHandler.GetInstance();
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Uri url = Request.Url;
            pageURL = url.AbsolutePath.ToString();
            log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            if (!IsPostBack)
            {
                Session.Remove("dsProdcts");
                Session.Remove("dsProducts_MP");
                if (Session["WH_Name"] != null && Session["PH_ID"]!=null)
                {
                    lblWH.Text = Session["WH_Name"].ToString();
                    lblPH.Text = Session["PH_Name"].ToString();
                    Session["WH_FirstTransfer"] = false;
                    Session["WH_TransferRequestGrid"] = null;
                   // Vendorname = Session["Vendorname"].ToString();
                }
                             
            }
            expHandler.CheckForErrorMessage(Session);
        }
        private void Page_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();
            // Void Page_Load(System.Object, System.EventArgs)
            // Handle specific exception.
            if (exc is HttpUnhandledException || exc.TargetSite.Name.ToLower().Contains("page_load"))
            {
                expHandler.GenerateExpResponse(pageURL, RedirectionStrategy.Remote, Session, Server, Response, log, exc);
            }
            else
            {
                expHandler.GenerateExpResponse(pageURL, RedirectionStrategy.local, Session, Server, Response, log, exc);
            }
            // Clear the error from the server.
            Server.ClearError();
        }
        protected void StockDisplayGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            StockDisplayGrid.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void StockDisplayGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            StockDisplayGrid.EditIndex = -1;
            BindGrid();
        }

        protected void StockDisplayGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("UpdateStock"))
                {
                    int quan, bonusquantity, quanOrg;
                    quan = bonusquantity = quanOrg = 0;
                    int.TryParse(((TextBox)StockDisplayGrid.Rows[StockDisplayGrid.EditIndex].FindControl("txtQuantity")).Text, out quan);
                    int orderDetID = int.Parse(((Label)StockDisplayGrid.Rows[StockDisplayGrid.EditIndex].FindControl("OrderDetailNo")).Text);
                    int.TryParse(((TextBox)StockDisplayGrid.Rows[StockDisplayGrid.EditIndex].FindControl("txtBonusQuantity")).Text, out bonusquantity);
                    int.TryParse(((Label)StockDisplayGrid.Rows[StockDisplayGrid.EditIndex].FindControl("lblQuantityOrg")).Text, out quanOrg);
                    string status = ((Label)StockDisplayGrid.Rows[StockDisplayGrid.EditIndex].FindControl("lblStatus")).Text;
                    if (quan + bonusquantity > 0)
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        SqlCommand command = new SqlCommand("Sp_UpdateTransferDetails_Creation", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_TransferDetailID", orderDetID);
                        command.Parameters.AddWithValue("@p_ReqQuantity", quan);
                        command.Parameters.AddWithValue("@p_ReqBonusQty", bonusquantity);

                        command.ExecuteNonQuery();

                        //if (!status.Equals("Pending"))
                        //{
                        //    command = new SqlCommand("Sp_UpdateOrderStatus", connection);
                        //    command.CommandType = CommandType.StoredProcedure;
                        //    command.Parameters.AddWithValue("@p_OrderDetailID", orderDetID);
                        //    command.Parameters.AddWithValue("@p_Status", "Partial");
                        //    command.ExecuteNonQuery();
                        //}
                    }
                    else
                    {
                        WebMessageBoxUtil.Show("Both ordered quantities cannot be 0");
                    }
                }
                //else if (e.CommandName.Equals("Delete"))
                //{

                //    int orderDetID = int.Parse(((Label)StockDisplayGrid.Rows[StockDisplayGrid.EditIndex].FindControl("OrderDetailNo")).Text);
                //    if (connection.State == ConnectionState.Closed)
                //    {
                //        connection.Open();
                //    }
                //    SqlCommand command = new SqlCommand("Sp_DeleteTransferDetails", connection);
                //    command.CommandType = CommandType.StoredProcedure;
                //    command.Parameters.AddWithValue("@p_OrderDetailID", orderDetID);

                //    command.ExecuteNonQuery();
                //}
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                StockDisplayGrid.EditIndex = -1;
                BindGrid();
            }
        }


        protected void StockDisplayGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int orderDetID = int.Parse(((Label)StockDisplayGrid.Rows[e.RowIndex].FindControl("OrderDetailNo")).Text);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand command = new SqlCommand("Sp_DeleteTransferDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_OrderDetailID", orderDetID);


                command.ExecuteNonQuery();
            }
            catch (Exception ex) 
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                StockDisplayGrid.EditIndex = -1;
                BindGrid();
            }
        }

        protected void StockDisplayGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            StockDisplayGrid.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void btnCreateOrder_Click(object sender, EventArgs e)
        {
            int quan, bQuan;
            quan = bQuan = 0;
            float discount = 0;
            int.TryParse(SelectQuantity.Text.ToString(), out quan);
            int.TryParse(SelectPrice.Text.ToString(), out bQuan);
            float.TryParse(txtDiscount.Text.ToString(), out discount);

            if (quan + bQuan > 0)
            {
                //btnAccept.Visible = true;
                btnDecline.Visible = true;
                if (Session["WH_FirstTransfer"].Equals(false))
                {

                    #region  Creating Transfer Order

                    int userID = Convert.ToInt32(Session["UserID"].ToString());

                    int p_TransferBy = 0;
                    int p_TransferTo = 0;
                    try
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }

                        SqlCommand command = new SqlCommand("sp_CreateTransferOrder", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        //Select Vendor
                        if (int.TryParse(Session["WH_ID"].ToString(), out p_TransferTo))
                        {
                            command.Parameters.AddWithValue("@p_TransferTo", p_TransferTo);
                        }
                        //Select From
                        if (int.TryParse(Session["PH_ID"].ToString(), out p_TransferBy))
                        {
                            command.Parameters.AddWithValue("@p_TransferBy", p_TransferBy);
                        }

                        command.Parameters.AddWithValue("@p_TransferStatus", "Initiated");
                        command.Parameters.AddWithValue("@p_TransferByUserID", userID);
                        DataTable dt = new DataTable();
                        SqlDataAdapter dA = new SqlDataAdapter(command);
                        dA.Fill(dt);
                        if (dt.Rows.Count != 0)
                        {
                            ViewState["WH_TransferNo"] = dt.Rows[0][0].ToString();
                        }

                    }
                    catch (Exception ex)
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                        throw ex;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                    #endregion

                    #region Linking to Transfer Detail table

                    try
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        SqlCommand command = new SqlCommand("sp_InsertIntoTransferOrderDetails", connection);
                        command.CommandType = CommandType.StoredProcedure;


                        int TransferNo, BonusOrdered, ProductNumber, Quantity;
                        TransferNo = BonusOrdered = ProductNumber = Quantity = 0;
                        #region parsing fields
                        int.TryParse(lblProductId.Text.ToString(), out ProductNumber);
                        #endregion
                        if (int.TryParse(ViewState["WH_TransferNo"].ToString(), out TransferNo))
                        {
                            command.Parameters.AddWithValue("@p_TransferID", TransferNo);
                        }
                        
                        command.Parameters.AddWithValue("@p_ProductID", ProductNumber);

                        if (int.TryParse(SelectQuantity.Text.ToString(), out Quantity))
                        {
                            command.Parameters.AddWithValue("@p_RequestedQty", Quantity);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@p_RequestedQty", DBNull.Value);
                        }
                        if (int.TryParse(SelectPrice.Text.ToString(), out BonusOrdered))
                        {
                            command.Parameters.AddWithValue("@p_ReqBonusQty", BonusOrdered);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@p_ReqBonusQty", DBNull.Value);
                        }

                        command.Parameters.AddWithValue("@p_Discount", discount);
                        command.Parameters.AddWithValue("@p_TransferStatus", "Initiated");

                        DataSet LinkResult = new DataSet();
                        SqlDataAdapter sA = new SqlDataAdapter(command);
                        sA.Fill(LinkResult);
                        
                        // we dont need to save transfer detail ID in this case. will check in future.

                        //if (LinkResult != null && LinkResult.Tables[0] != null)
                        //{
                        //    ViewState["WH_TransferDetailID"] = LinkResult.Tables[0].Rows[0][0];
                        //}
                    }
                    catch (Exception ex)
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                        throw ex;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                    #endregion


                    Session["WH_FirstTransfer"] = true;
                }
                else
                {
                    //ViewState["WH_TransferDetailID"] = "0";

                    #region Check wether Product Existing in the Current Transfer
                    DataSet ds = new DataSet();
                    try
                    {
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        SqlCommand command = new SqlCommand("sp_getTransferDetails_TransferID", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        int TransferNo = 0;


                        if (int.TryParse(ViewState["WH_TransferNo"].ToString(), out TransferNo))
                        {
                            command.Parameters.AddWithValue("@p_TransferID", TransferNo);
                        }
                        SqlDataAdapter sA = new SqlDataAdapter(command);
                        sA.Fill(ds);
                    }
                    catch (Exception ex)
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                        throw ex;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }

                    int ProductNO = 0;
                    bool ProductPresent = false;
                    if (int.TryParse(lblProductId.Text.ToString(), out ProductNO))
                    {
                    }

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (Convert.ToInt32(ds.Tables[0].Rows[i]["ProductID"]).Equals(ProductNO))
                        {
                            ProductPresent = true;
                            break;
                        }
                    }
                    #endregion

                    if (ProductPresent.Equals(false))
                    {
                        #region Linking to Order Detail table

                        try
                        {
                            if (connection.State == ConnectionState.Closed)
                            {
                                connection.Open();
                            }
                            SqlCommand command = new SqlCommand("sp_InsertIntoTransferOrderDetails", connection);
                            command.CommandType = CommandType.StoredProcedure;


                            int TransferNo, BonusOrdered, ProductNumber, Quantity;
                            TransferNo = BonusOrdered = ProductNumber = Quantity = 0;

                            if (int.TryParse(ViewState["WH_TransferNo"].ToString(), out TransferNo))
                            {
                                command.Parameters.AddWithValue("@p_TransferID", TransferNo);
                            }
                            int.TryParse(lblProductId.Text.ToString(), out ProductNumber);
                            command.Parameters.AddWithValue("@p_ProductID", ProductNumber);

                            if (int.TryParse(SelectQuantity.Text.ToString(), out Quantity))
                            {
                                command.Parameters.AddWithValue("@p_RequestedQty", Quantity);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@p_RequestedQty", DBNull.Value);
                            }
                            if (int.TryParse(SelectPrice.Text.ToString(), out BonusOrdered))
                            {
                                command.Parameters.AddWithValue("@p_ReqBonusQty", BonusOrdered);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@p_ReqBonusQty", DBNull.Value);
                            }
                            command.Parameters.AddWithValue("@p_Discount", discount);
                            command.Parameters.AddWithValue("@p_TransferStatus", "Initiated");

                            DataSet LinkResult = new DataSet();
                            SqlDataAdapter sA = new SqlDataAdapter(command);
                            sA.Fill(LinkResult);
                            //if (LinkResult != null && LinkResult.Tables[0] != null)
                            //{
                            //    ViewState["WH_TransferDetailID"] = LinkResult.Tables[0].Rows[0][0];
                            //}
                        }
                        catch (Exception ex)
                        {
                            if (connection.State == ConnectionState.Open)
                                connection.Close();
                            throw ex;
                        }
                        finally
                        {
                            if (connection.State == ConnectionState.Open)
                                connection.Close();
                        }
                        #endregion

                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record can not be inserted, because it is already present')", true);
                    }
                }
              
                BindGrid();
                //txtProduct.Text = "";
                //SelectProduct.Visible = false;
                txtSearch.Text = "";
                // RequestTo.Visible = false;
                SelectQuantity.Text = "";
                SelectPrice.Text = "";
                //  txtVendor.Enabled = false;
                // SelectProduct.SelectedIndex = -1;
                //RequestTo.SelectedIndex = -1;
            }
            else
            {
                WebMessageBoxUtil.Show("Both quantity and bonus quantity cannot be 0");
                return;
            }
        }


        private void BindGrid()
        {
            #region Display Products
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand command = new SqlCommand("sp_FetchTransferDetails_TranferID", connection);
                command.CommandType = CommandType.StoredProcedure;
                int OrderNumber = 0;
                DataSet ds = new DataSet();

                if (int.TryParse(ViewState["WH_TransferNo"].ToString(), out OrderNumber))
                {
                    command.Parameters.AddWithValue("@p_TransferID", OrderNumber);
                }


                SqlDataAdapter sA = new SqlDataAdapter(command);
                sA.Fill(ds);
                StockDisplayGrid.DataSource = null;
              //  Session["dsProducts_MP"] = ds;
                Session["WH_TransferRequestGrid"] = ds.Tables[0];
                StockDisplayGrid.DataSource = ds.Tables[0];
                StockDisplayGrid.DataBind();

                float TCost = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    float Cost = 0;
                    if (float.TryParse(ds.Tables[0].Rows[i]["DiscountCostPrice"].ToString(), out Cost))
                    {
                        TCost += Cost;
                    }
                }
                lblttlcst.Visible = true;
                lblTotalCostALL.Visible = true;
                lblTotalCostALL.Text = TCost.ToString();
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            #endregion
        }
        protected void btnRefresh_Click(object sender, EventArgs e)
        {

            StockDisplayGrid.DataSource = null;
            StockDisplayGrid.DataBind();
            SelectQuantity.Text = "";
            ViewState.Remove("WH_TransferNo");
            Session["WH_FirstTransfer"]=false;
            Session.Remove("dsProdcts");
            Session.Remove("dsProducts_MP");
            btnDecline.Visible = false;
            lblttlcst.Visible = false;
            lblTotalCostALL.Visible = false;
        }

        protected void btnCancelOrder_Click(object sender, EventArgs e)
        {
            lblttlcst.Visible = false;
            Session["WH_FirstTransfer"] = false;
            Session.Remove("WH_TransferRequestGrid");
            Session.Remove("dsProdcts");
            Session.Remove("dsProducts_MP");
            Response.Redirect("SalesmanMain.aspx", false);
           
        }


        protected void StockDisplayGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label Status = (Label)e.Row.FindControl("lblStatus");
                    Button btnEdit = (Button)e.Row.FindControl("btnEdit");
                    Button btnDelete = (Button)e.Row.FindControl("btnDelete");

                    if (Status.Text.Equals("Complete") || Status.Text.Equals("Partial"))
                    {
                        if (btnDelete != null)
                        {
                            btnDelete.Enabled = false;
                        }
                    }
                    else
                    {
                        if (btnEdit != null)
                        {
                            btnEdit.Enabled = true;
                        }
                        if (btnDelete != null)
                        {
                            btnDelete.Enabled = true;
                        }
                    }

                }
            }
            catch (Exception ex) { }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

            }
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            int p_TransferNo;
            int.TryParse(ViewState["WH_TransferNo"].ToString(), out p_TransferNo);
            ViewState["WH_TransferNo"] = null;
            Session["WH_FirstTransfer"] = false;
            Session.Remove("WH_Name");
            Session.Remove("WH_ID");
            Session.Remove("dsProdcts");
            Session.Remove("dsProducts_MP");
            lblttlcst.Visible = false;


            Response.Redirect("Print_Request_WH_PH.aspx?Id=" + p_TransferNo, false);
        }


        protected void btnDecline_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Remove("dsProdcts");
                Session.Remove("dsProducts_MP");
                Session.Remove("WH_TransferRequestGrid");
                DataSet delDs = new DataSet();
                if (ViewState["WH_TransferNo"] != null)
                {
                    int orderID = int.Parse(ViewState["WH_TransferNo"].ToString());
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    //fetch order detail entries
                    SqlCommand command = new SqlCommand("Sp_FetchTransferReceiveEntry_byTransferID", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_TransferID", orderID);
                    SqlDataAdapter sA = new SqlDataAdapter(command);
                    sA.Fill(delDs);
                    for (int i = 0; i < delDs.Tables[0].Rows.Count; i++)
                    {

                        //remove entries and remove values from stock
                        command = new SqlCommand("Sp_RemoveTransferEntries", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@p_OrderDetailID", delDs.Tables[0].Rows[i]["TransferDetailID"].ToString());
                        command.Parameters.AddWithValue("@p_expiryOriginal", delDs.Tables[0].Rows[i]["ExpiryDate"].ToString());
                        command.Parameters.AddWithValue("@p_entryID", delDs.Tables[0].Rows[i]["entryID"].ToString());
                        command.Parameters.AddWithValue("@p_ProductID", delDs.Tables[0].Rows[i]["ProductID"].ToString());
                        command.Parameters.AddWithValue("@p_StoreID", delDs.Tables[0].Rows[i]["TransferBy"].ToString());

                        command.ExecuteNonQuery();
                    }
                    //delete order master and order detail
                    command = new SqlCommand("sp_DeleteTransfer", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_TransferID", orderID);
                    command.ExecuteNonQuery();
                }
                ViewState["WH_TransferNo"] = null;
                Session["FromViewPlacedOrders"] = null;
                Session["WH_FirstTransfer"] = false;
                lblttlcst.Visible = false;
                
                StockDisplayGrid.DataSource = null;
                StockDisplayGrid.DataBind();
                SelectQuantity.Text = "";
                btnDecline.Visible = false;
                              
                lblttlcst.Visible = false;
                lblTotalCostALL.Visible = false;
                WebMessageBoxUtil.Show("Order and stock has been successfully removed");
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        protected void btnSearchProduct_Click(object sender, ImageClickEventArgs e)
        {
        //    ModalPopupExtender mpe = (ModalPopupExtender)this.Parent.FindControl("mpeCongratsMessageDiv");
        //    mpe.Show();
        }

        protected void btnSearchProduct_Click1(object sender, ImageClickEventArgs e)
        {
            String Text = txtSearch.Text + '%';
            Session["Text"] = Text;
            ProductsPopupGrid.PopulateGrid();
            mpeCongratsMessageDiv.Show();

        }
    }
}