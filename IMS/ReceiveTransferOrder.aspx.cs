﻿using IMS.Util;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace IMS
{
    public partial class ReceiveTransferOrder : System.Web.UI.Page
    {
        public static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSConnectionString"].ToString());
        public static DataSet dsStatic = new DataSet();
        public static DataTable dtStatic = new DataTable();
        private ILog log;
        private string pageURL;
        private ExceptionHandler expHandler = ExceptionHandler.GetInstance();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                System.Uri url = Request.Url;
                pageURL = url.AbsolutePath.ToString();
                log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                if (!IsPostBack)
                {
                     
                    //Add Previously subtracted Stock
                    PopulateddPharmacy();

                }
                expHandler.CheckForErrorMessage(Session);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

            }
        }

        private void PopulateddPharmacy()
        {
            #region Populating Department DropDown
            try
            {
                int Userid;
                if(connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand command = new SqlCommand("sp_GetPharmacyDetials_TransferDetailUserID", connection);
                int.TryParse(Session["UserSys"].ToString(), out Userid);
                command.Parameters.AddWithValue("@UserID", Userid);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataSet ds = new DataSet();
                da.Fill(ds);
                ddPharmacy.DataSource = ds.Tables[0];



                ddPharmacy.DataValueField = "TransferBy";
                ddPharmacy.DataTextField = "SystemName";
                ddPharmacy.DataBind();
                if (ddPharmacy != null)
                {
                    ddPharmacy.Items.Insert(0, "Select Pharmacy");
                    ddPharmacy.Items.Insert(1, "All");
                    ddPharmacy.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            #endregion
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
        private void LoadRepeater(int RequestedPharmacyID)
        {
            try
            {
                if(RequestedPharmacyID == 1)
                {
                    DataSet ds = new DataSet();
                    int Userid;
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand command = new SqlCommand("sp_getTransferDetails_UserId", connection);
                    int.TryParse(Session["UserSys"].ToString(), out Userid);
                    command.Parameters.AddWithValue("@UserID", Userid);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(ds);
                    DataTable dsDistinct = ds.Tables[0];
                    dtStatic = ds.Tables[0];

                    DataTable distinctRequests = dsDistinct.DefaultView.ToTable(true, "TransferID");

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        btnAcceptAll.Visible = false;
                        btnGenTransferAll.Visible = false;
                    }

                    repReceiveTransfer.DataSource = distinctRequests;
                    repReceiveTransfer.DataBind();
                }
                else
                {
                    DataSet ds = new DataSet();
                    int Userid;
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand command = new SqlCommand("sp_getTransferDetails_UserId", connection);
                    int.TryParse(Session["UserSys"].ToString(), out Userid);
                    command.Parameters.AddWithValue("@UserID", Userid);
                    command.Parameters.AddWithValue("@p_RequestedPharmacyID", RequestedPharmacyID);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(ds);
                    DataTable dsDistinct = ds.Tables[0];
                    dtStatic = ds.Tables[0];

                    DataTable distinctRequests = dsDistinct.DefaultView.ToTable(true, "TransferID");

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        btnAcceptAll.Visible = false;
                        btnGenTransferAll.Visible = false;
                    }

                    repReceiveTransfer.DataSource = distinctRequests;
                    repReceiveTransfer.DataBind();
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
        }

        protected void repReceiveTransfer_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                GridView dgvReceiveTransfer = (GridView)e.Item.FindControl("dgvReceiveTransfer");
                Literal litStoreName = (Literal)e.Item.FindControl("litStoreName");

                Literal litReqNo = (Literal)e.Item.FindControl("litReqNo");

                Label lblStoreID = (Label)e.Item.FindControl("lblStoreID");
                Button btnAcceptTransferOrder = (Button)e.Item.FindControl("btnAcceptTransferOrder");
                DataSet ds = new DataSet();
                int Userid;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                int TransferID = Convert.ToInt32(((DataRowView)e.Item.DataItem).Row[0].ToString());

                SqlCommand command = new SqlCommand("sp_getUserTransferDetails_TransferID", connection);
                int.TryParse(Session["UserSys"].ToString(), out Userid);
                command.Parameters.AddWithValue("@UserID", Userid);
                command.Parameters.AddWithValue("@TransferID", TransferID);

                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds);
                dsStatic = ds;

                int totalRows = ds.Tables[0].Rows.Count;
                int i = 0;

                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row["TransferStatus"].ToString() == "Accepted" || row["TransferStatus"].ToString() == "Denied")
                        {
                            i++;

                        }
                    }
                }

                if (i == totalRows) {
                    btnAcceptTransferOrder.Visible = false;
                }

                dgvReceiveTransfer.DataSource = ds;
                dgvReceiveTransfer.DataBind();

                litReqNo.Text = ds.Tables[0].Rows[0]["TransferID"].ToString();
                litStoreName.Text = ds.Tables[0].Rows[0]["RequestedBy"].ToString();
                lblStoreID.Text = ds.Tables[0].Rows[0]["StoreID"].ToString();
            }
            catch(Exception ex)
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
        }

        protected void dgvReceiveTransfer_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int TransferNo, TransferDetailNo, RequestedQty, TransferedQty, ReqestedBonusQty, AvailableQty, ProductId;
                int LogedInStoreID;

                int.TryParse(Session["UserSys"].ToString(), out LogedInStoreID);
                GridView dgvReceiveTransfer = (GridView)sender;
                Label lblTransferNo = (Label)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("lblRequestNo");
                Label lblTransferDetailsID = (Label)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("lblTransferDetailsID");
                Label lblRequestedQty = (Label)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("lblRequestedQty");
                Label lblAvailableQty = (Label)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("lblAvailableQty");
                Label lblSentQty = (Label)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("lblSentQty");
                Label lblProductID = (Label)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("lblProductID");
             //   Label lblRequestedBonusQty = (Label)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("lblRequestedBonusQty");

                int.TryParse(lblTransferNo.Text.ToString(), out TransferNo);
                int.TryParse(lblTransferDetailsID.Text.ToString(), out TransferDetailNo);
                int.TryParse(lblRequestedQty.Text.ToString(), out RequestedQty);
               // int.TryParse(lblRequestedBonusQty.Text.ToString(), out ReqestedBonusQty);
                int.TryParse(lblAvailableQty.Text.ToString(), out AvailableQty);
                int.TryParse(lblSentQty.Text.ToString(), out TransferedQty);
                int.TryParse(lblProductID.Text.ToString(), out ProductId);
                int userID = Convert.ToInt32(Session["UserID"].ToString());

                if (e.CommandName == "Edit")
                {
                    Session["TransferDetailID"] = TransferDetailNo;
                    Session["TransferedQty"] = RequestedQty;
                    Response.Redirect("ReceiveTransferDetails_ReceiveEntry.aspx", false);
                }
                if (e.CommandName == "AcceptProductTransfer")
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand command = new SqlCommand("sp_UpdateTransferOrderDetials_TransferDetialID", connection);
                    command.Parameters.AddWithValue("@p_TransferID", TransferNo);
                    command.Parameters.AddWithValue("@p_TransferDetID", TransferDetailNo);
                    command.Parameters.AddWithValue("@p_RequestedQty", RequestedQty);
                    command.Parameters.AddWithValue("@p_TransferedQty", TransferedQty); 
                    
                 //   command.Parameters.AddWithValue("@p_TransferedBonusQty", ReqestedBonusQty);
                    
                    command.Parameters.AddWithValue("@p_AvailableQty", AvailableQty);
                    command.Parameters.AddWithValue("@p_Status", "Accepted");
                    command.Parameters.AddWithValue("@p_LogedinnStore", LogedInStoreID);
                    command.Parameters.AddWithValue("@p_ProductID", ProductId);
                    command.Parameters.AddWithValue("@p_TransferToUserID", userID);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();

                    Button btnAccept = (Button)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("btnAccept");
                    btnAccept.Visible = false;
                    Button btnDeny = (Button)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("btnDeny");
                    btnDeny.Visible = false;
                    Button btnEdit = (Button)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("btnEdit");
                    btnEdit.Visible = false;
                    //Button btnStaticAccepted = (Button)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("btnStaticAccepted");
                    //btnStaticAccepted.Visible = true;
                  
                    if (RequestedQty != TransferedQty)
                    {
                        if (TransferedQty == 0)
                        {
                            //RequestedQty = RequestedQty + ReqestedBonusQty;// Remove Requested and Bonus   
                            UpdateStockMinus(TransferDetailNo, ProductId, AvailableQty, RequestedQty);
                        }
                        else
                        {
                           // TransferedQty = TransferedQty + ReqestedBonusQty;
                            UpdateStockMinus(TransferDetailNo, ProductId, AvailableQty, TransferedQty);
                        }
                    }
                    LoadRepeater(1);
                    HtmlGenericControl btnStaticAccepted = (HtmlGenericControl)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("btnStaticAccepted");
                    btnStaticAccepted.Visible = true;  
                }
                if (e.CommandName == "DenyProductTransfer")
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand command = new SqlCommand("sp_DenyTransferRequest_TransferDetialID", connection);
                    command.Parameters.AddWithValue("@p_TransferID", TransferNo);
                    command.Parameters.AddWithValue("@p_TransferDetID", TransferDetailNo);
                    command.Parameters.AddWithValue("@p_RequestedTransferedQty", TransferedQty);
                    command.Parameters.AddWithValue("@p_ReceivedQty", 0);
                    command.Parameters.AddWithValue("@p_AvailableQty", AvailableQty);
                    command.Parameters.AddWithValue("@p_Status", "Denied");
                    command.Parameters.AddWithValue("@p_LogedinnStore", LogedInStoreID);
                    command.Parameters.AddWithValue("@p_ProductID", ProductId);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();


                    Button btnAccept = (Button)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("btnAccept");
                    btnAccept.Visible = false;
                    Button btnDeny = (Button)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("btnDeny");
                    btnDeny.Visible = false;
                    Button btnEdit = (Button)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("btnEdit");
                    btnEdit.Visible = false;

                    HtmlGenericControl lblStaticDeny = (HtmlGenericControl)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("lblStaticDeny");
                    lblStaticDeny.Visible = true;  
                    //Label lblStaticDeny = (Label)dgvReceiveTransfer.Rows[Convert.ToInt32(e.CommandArgument.ToString())].FindControl("lblStaticDeny");
                    //lblStaticDeny.Visible = true;
                    LoadRepeater(1);
                     
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
            
        }

        protected void btnAcceptTransferOrder_Click(object sender, EventArgs e)
        {
            
        }

        protected void repReceiveTransfer_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "AcceptTransferOrder")
                {
                   // GridView gvReceiveTransfer = (GridView)repReceiveTransfer.Items[0].FindControl("dgvReceiveTransfer");
                    GridView gvReceiveTransfer = (GridView)e.Item.FindControl("dgvReceiveTransfer");
                    for (int i = 0; i < gvReceiveTransfer.Rows.Count; i++)
                    {
                        int TransferNo, TransferDetailNo, RequestedQty, TransferedQty,  AvailableQty, ProductId, TransferedBonusQty;
                        int LogedInStoreID;


                        int.TryParse(Session["UserSys"].ToString(), out LogedInStoreID);
                        Label lblRequestedBonusQty = (Label)gvReceiveTransfer.Rows[i].FindControl("lblRequestedBonusQty");

                        Label lblTransferNo = (Label)gvReceiveTransfer.Rows[i].FindControl("lblRequestNo");
                        Label lblTransferDetailsID = (Label)gvReceiveTransfer.Rows[i].FindControl("lblTransferDetailsID");
                        Label lblRequestedQty = (Label)gvReceiveTransfer.Rows[i].FindControl("lblRequestedQty");
                        Label lblAvailableQty = (Label)gvReceiveTransfer.Rows[i].FindControl("lblAvailableQty");
                        Label lblSentQty = (Label)gvReceiveTransfer.Rows[i].FindControl("lblSentQty");
                        Label lblProductID = (Label)gvReceiveTransfer.Rows[i].FindControl("lblProductID");

                        int.TryParse(lblTransferNo.Text.ToString(), out TransferNo);
                        int.TryParse(lblTransferDetailsID.Text.ToString(), out TransferDetailNo);
                        int.TryParse(lblRequestedQty.Text.ToString(), out RequestedQty);
                   //     int.TryParse(lblRequestedBonusQty.Text.ToString(), out TransferedBonusQty);

                        int.TryParse(lblAvailableQty.Text.ToString(), out AvailableQty);
                        int.TryParse(lblSentQty.Text.ToString(), out TransferedQty);
                        int.TryParse(lblProductID.Text.ToString(), out ProductId);
                        int userID = Convert.ToInt32(Session["UserID"].ToString());

                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }

                        SqlCommand command = new SqlCommand("sp_UpdateTransferOrderDetials_AcceptAll", connection);
                        command.Parameters.AddWithValue("@p_TransferID", TransferNo);
                        command.Parameters.AddWithValue("@p_TransferDetID", TransferDetailNo);
                        command.Parameters.AddWithValue("@p_RequestedQty", RequestedQty);
                        command.Parameters.AddWithValue("@p_TransferedQty", TransferedQty);
                        command.Parameters.AddWithValue("@p_AvailableQty", AvailableQty);
                        command.Parameters.AddWithValue("@p_Status", "Accepted");
                        command.Parameters.AddWithValue("@p_LogedinnStore", LogedInStoreID);
                        command.Parameters.AddWithValue("@p_ProductID", ProductId);
                        command.Parameters.AddWithValue("@p_TransferToUserID", userID);
                         
                     //   command.Parameters.AddWithValue("@p_TransferedBonQty", TransferedBonusQty);
 
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();

                        Button btnAccept = (Button)gvReceiveTransfer.Rows[i].FindControl("btnAccept");
                        btnAccept.Visible = false;
                        Button btnDeny = (Button)gvReceiveTransfer.Rows[i].FindControl("btnDeny");
                        btnDeny.Visible = false;
                        Button btnEdit = (Button)gvReceiveTransfer.Rows[i].FindControl("btnEdit");
                        btnEdit.Visible = false;
                        //Button btnStaticAccepted = (Button)gvReceiveTransfer.Rows[i].FindControl("btnStaticAccepted");
                        //btnStaticAccepted.Visible = true;

                        HtmlGenericControl btnStaticAccepted = (HtmlGenericControl)gvReceiveTransfer.Rows[i].FindControl("btnStaticAccepted");
                        btnStaticAccepted.Visible = true;

                        if (RequestedQty != TransferedQty)
                        {
                            if (TransferedQty == 0)
                            {
                               
                                UpdateStockMinus(TransferDetailNo, ProductId, AvailableQty, RequestedQty);
                            }
                            else
                            {
                               
                                UpdateStockMinus(TransferDetailNo, ProductId, AvailableQty, TransferedQty);
                            }
                        }
                        //if (RequestedQty != TransferedQty)
                        //{
                        //    if (TransferedQty == 0)
                        //    {
                        //        RequestedQty = RequestedQty + TransferedBonusQty;
                        //        UpdateStockMinus(TransferDetailNo, ProductId, AvailableQty, RequestedQty);
                        //    }
                        //    else
                        //    {
                        //        TransferedQty = TransferedQty + TransferedBonusQty;
                        //        UpdateStockMinus(TransferDetailNo, ProductId, AvailableQty, TransferedQty);
                        //    }
                        //}
                        Button btnAcceptTransferOrder = (Button)repReceiveTransfer.Items[0].FindControl("btnAcceptTransferOrder");

                        btnAcceptTransferOrder.Visible = false;
                        LoadRepeater(1);
                    }
                } 
                if (e.CommandName == "GenTransferOrder")
                {
                    DataSet dsResults = new DataSet();

                    DataTable distinctOrders = dtStatic.DefaultView.ToTable(true, "TransferBy");

                    Literal litReqNo = (Literal)e.Item.FindControl("litReqNo");

                    Literal litStoreName = (Literal)e.Item.FindControl("litStoreName");
                    Label lblStoreID = (Label)e.Item.FindControl("lblStoreID");

                    string abc = lblStoreID.Text;
                    int TransferID = int.Parse(litReqNo.Text.ToString());
                    
                    GridView gvReceiveTransfer = (GridView)repReceiveTransfer.Items[0].FindControl("dgvReceiveTransfer");
                    int TransferNo, TransferDetailNo, RequestedQty, TransferedQty, AvailableQty, ProductId;
                    int LogedInStoreID;


                    int.TryParse(Session["UserSys"].ToString(), out LogedInStoreID);

                    Label lblTransferNo = (Label)gvReceiveTransfer.Rows[0].FindControl("lblRequestNo");
                    Label lblTransferDetailsID = (Label)gvReceiveTransfer.Rows[0].FindControl("lblTransferDetailsID");
                    Label lblRequestedQty = (Label)gvReceiveTransfer.Rows[0].FindControl("lblRequestedQty");
                    Label lblAvailableQty = (Label)gvReceiveTransfer.Rows[0].FindControl("lblAvailableQty");
                    Label lblSentQty = (Label)gvReceiveTransfer.Rows[0].FindControl("lblSentQty");
                    Label lblProductID = (Label)gvReceiveTransfer.Rows[0].FindControl("lblProductID");

                    int.TryParse(lblTransferNo.Text.ToString(), out TransferNo);
                    int.TryParse(lblTransferDetailsID.Text.ToString(), out TransferDetailNo);
                    int.TryParse(lblRequestedQty.Text.ToString(), out RequestedQty);

                    int.TryParse(lblAvailableQty.Text.ToString(), out AvailableQty);
                    int.TryParse(lblSentQty.Text.ToString(), out TransferedQty);
                    int.TryParse(lblProductID.Text.ToString(), out ProductId);
                    int SystemID;
                    int.TryParse(lblStoreID.Text.ToString(), out SystemID);

                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand command = new SqlCommand("sp_UpdateTransferOrderDetials_Generate", connection);
                     
                    command.Parameters.AddWithValue("@p_LogedinnStore", LogedInStoreID);
                    command.Parameters.AddWithValue("@p_SystemID", SystemID);
                    command.Parameters.AddWithValue("@p_TransferID", TransferID);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dsResults);

                    Session["TransferRequestGrid"] = dsResults.Tables[0];
                   
                    Response.Redirect("GenerateAcceptedTransferOrder.aspx", false);
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

        }
        private void UpdateStockMinus(int TransferDetailID, int ProductID, int quantity, int Sent)
        {
            try
            {
                DataSet stockDet;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                #region Query stock
                SqlCommand command = new SqlCommand("sp_GetStockBy_TransferDetail", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_TransferDetail", TransferDetailID);
                command.Parameters.AddWithValue("@p_StoredAt", int.Parse(Session["UserSys"].ToString()));

                DataSet ds = new DataSet();
                SqlDataAdapter dA = new SqlDataAdapter(command);
                dA.Fill(ds);
                stockDet = ds;
                #endregion

                #region Set value for ordered quantity
                Dictionary<int, int> stockSet = new Dictionary<int, int>();
                foreach (DataRow row in stockDet.Tables[0].Rows)
                {
                    int exQuan = int.Parse(row["Quantity"].ToString());
                    if (Sent > 0 && exQuan > 0)
                    {
                        if (exQuan >= Sent)
                        {
                            stockSet.Add(int.Parse(row["StockID"].ToString()), Sent);
                            break;
                        }
                        else if (exQuan < Sent)
                        {
                            stockSet.Add(int.Parse(row["StockID"].ToString()), exQuan);
                            Sent = Sent - exQuan;
                        }
                    }
                }


                #endregion

                #region update stock

                foreach (int id in stockSet.Keys)
                {
                    DataView dv = stockDet.Tables[0].DefaultView;
                    dv.RowFilter = "StockID = " + id;
                    DataTable dt = dv.ToTable();

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        command = new SqlCommand("sp_EntryTransferDetails_Receive", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_TransferDetailID", TransferDetailID);
                        command.Parameters.AddWithValue("@p_ProductID", dt.Rows[0]["ProductID"]);
                        command.Parameters.AddWithValue("@p_StockID", id);
                        command.Parameters.AddWithValue("@p_CostPrice", dt.Rows[0]["UCostPrice"]);
                        command.Parameters.AddWithValue("@p_SalePrice", dt.Rows[0]["USalePrice"]);
                        command.Parameters.AddWithValue("@p_Expiry", dt.Rows[0]["ExpiryDate"]);
                        command.Parameters.AddWithValue("@p_Batch", dt.Rows[0]["BatchNumber"]);
                        command.Parameters.AddWithValue("@p_TransferredQty", stockSet[id]);

                        command.Parameters.AddWithValue("@p_RequestedQty", dt.Rows[0]["RequestedQty"]);
                        //command.Parameters.AddWithValue("@p_RequestedBonusQty", TransferedBonusQty);

                        command.Parameters.AddWithValue("@p_BarCode", dt.Rows[0]["BarCode"]);
                        
                        command.ExecuteNonQuery();

                        command = new SqlCommand("Sp_UpdateStockBy_StockID", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_StockID", id);
                        command.Parameters.AddWithValue("@p_quantity", stockSet[id]);
                        command.Parameters.AddWithValue("@p_Action", "Minus");
                        command.ExecuteNonQuery();
                    }

                }
                #endregion
 

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
        }

        protected void dgvReceiveTransfer_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Convert.ToInt32(((Label)e.Row.FindControl("lblAvailableQty") as Label).Text) == 0) {
                    Button btnEdit = (Button)e.Row.FindControl("btnEdit");
                    btnEdit.Visible = false;
                }
                 
                if (dsStatic.Tables[0].Rows[e.Row.RowIndex]["TransferStatus"].ToString() == "Accepted" || dsStatic.Tables[0].Rows[e.Row.RowIndex]["TransferStatus"].ToString() == "Received")
                {
                    GridView gv = (GridView)sender;
                    Button btnAccept = (Button)e.Row.FindControl("btnAccept");
                    if (btnAccept != null)
                    {
                        btnAccept.Visible = false;
                    }
                    Button btnDeny = (Button)e.Row.FindControl("btnDeny");
                    if (btnDeny != null)
                    {
                        btnDeny.Visible = false;
                    }
                    Button btnEdit = (Button)e.Row.FindControl("btnEdit");
                    if (btnEdit != null)
                    {
                        btnEdit.Visible = false;
                    }
                    
                    HtmlGenericControl btnStaticAccepted = (HtmlGenericControl)e.Row.FindControl("btnStaticAccepted");
                    btnStaticAccepted.Visible = true;  
                }
                if (dsStatic.Tables[0].Rows[e.Row.RowIndex]["TransferStatus"].ToString() == "Denied" )
                {
                    GridView gv = (GridView)sender;
                    Button btnAccept = (Button)e.Row.FindControl("btnAccept");
                    btnAccept.Visible = false;
                    Button btnDeny = (Button)e.Row.FindControl("btnDeny");
                    btnDeny.Visible = false;
                    Button btnEdit = (Button)e.Row.FindControl("btnEdit");
                    btnEdit.Visible = false;
                    HtmlGenericControl lblStaticDeny = (HtmlGenericControl)e.Row.FindControl("lblStaticDeny");
                    lblStaticDeny.Visible = true;  
                }
            }
            

        }

        protected void btnAcceptAll_Click(object sender, EventArgs e)
        {
            try
            {
                for (int rows = 0; rows < repReceiveTransfer.Items.Count; rows++)
                {
                    GridView gvReceiveTransfer = (GridView)repReceiveTransfer.Items[rows].FindControl("dgvReceiveTransfer");
                    for (int i = 0; i < gvReceiveTransfer.Rows.Count; i++)
                    {
                        int TransferNo, TransferDetailNo, RequestedQty, TransferedQty, AvailableQty, ProductId, TransferedBonusQty;
                        int LogedInStoreID;


                        int.TryParse(Session["UserSys"].ToString(), out LogedInStoreID);

                        Label lblRequestedBonusQty = (Label)gvReceiveTransfer.Rows[i].FindControl("lblRequestedBonusQty");
                        Label lblTransferNo = (Label)gvReceiveTransfer.Rows[i].FindControl("lblRequestNo");
                        Label lblTransferDetailsID = (Label)gvReceiveTransfer.Rows[i].FindControl("lblTransferDetailsID");
                        Label lblRequestedQty = (Label)gvReceiveTransfer.Rows[i].FindControl("lblRequestedQty");
                        Label lblAvailableQty = (Label)gvReceiveTransfer.Rows[i].FindControl("lblAvailableQty");
                        Label lblSentQty = (Label)gvReceiveTransfer.Rows[i].FindControl("lblSentQty");
                        Label lblProductID = (Label)gvReceiveTransfer.Rows[i].FindControl("lblProductID");

                        int.TryParse(lblTransferNo.Text.ToString(), out TransferNo);
                        int.TryParse(lblTransferDetailsID.Text.ToString(), out TransferDetailNo);
                        int.TryParse(lblRequestedQty.Text.ToString(), out RequestedQty);

                        int.TryParse(lblRequestedBonusQty.Text.ToString(), out TransferedBonusQty);

                        int.TryParse(lblAvailableQty.Text.ToString(), out AvailableQty);
                        int.TryParse(lblSentQty.Text.ToString(), out TransferedQty);
                        int.TryParse(lblProductID.Text.ToString(), out ProductId);
                        int userID = Convert.ToInt32(Session["UserID"].ToString());

                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }

                        SqlCommand command = new SqlCommand("sp_UpdateTransferOrderDetials_AcceptAll", connection);
                        command.Parameters.AddWithValue("@p_TransferID", TransferNo);
                        command.Parameters.AddWithValue("@p_TransferDetID", TransferDetailNo);
                        command.Parameters.AddWithValue("@p_RequestedQty", RequestedQty);
                        command.Parameters.AddWithValue("@p_TransferedQty", TransferedQty);
                        command.Parameters.AddWithValue("@p_AvailableQty", AvailableQty);
                        command.Parameters.AddWithValue("@p_Status", "Accepted");
                        command.Parameters.AddWithValue("@p_LogedinnStore", LogedInStoreID);
                        command.Parameters.AddWithValue("@p_ProductID", ProductId);
                        command.Parameters.AddWithValue("@p_TransferToUserID", userID);

                        command.Parameters.AddWithValue("@p_TransferedBonQty", TransferedBonusQty);

                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();

                        Button btnAccept = (Button)gvReceiveTransfer.Rows[i].FindControl("btnAccept");
                        btnAccept.Visible = false;
                        Button btnDeny = (Button)gvReceiveTransfer.Rows[i].FindControl("btnDeny");
                        btnDeny.Visible = false;
                        Button btnEdit = (Button)gvReceiveTransfer.Rows[i].FindControl("btnEdit");
                        btnEdit.Visible = false;

                        HtmlGenericControl btnStaticAccepted = (HtmlGenericControl)gvReceiveTransfer.Rows[i].FindControl("btnStaticAccepted");
                        btnStaticAccepted.Visible = true;

                        if (RequestedQty != TransferedQty)
                        {
                            if (TransferedQty == 0)
                            {
                                RequestedQty = TransferedBonusQty + RequestedQty;
                                UpdateStockMinus(TransferDetailNo, ProductId, AvailableQty, RequestedQty);
                            }
                            else
                            {
                                TransferedQty = TransferedBonusQty + TransferedQty;
                                UpdateStockMinus(TransferDetailNo, ProductId, AvailableQty, TransferedQty);
                            }
                        }
                        Button btnAcceptTransferOrder = (Button)repReceiveTransfer.Items[0].FindControl("btnAcceptTransferOrder");

                        btnAcceptTransferOrder.Visible = false;
                        LoadRepeater(1);
                    }
                }
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                throw ex;
            }
            finally {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

        }


        protected void btnGenTransferAll_Click(object sender, EventArgs e)
        {
            try
            {
                for (int rows = 0; rows < repReceiveTransfer.Items.Count; rows++)
                {
                    GridView gvReceiveTransfer = (GridView)repReceiveTransfer.Items[rows].FindControl("dgvReceiveTransfer");
                    DataSet dsResults = new DataSet();

                    int TransferNo, TransferDetailNo, RequestedQty, TransferedQty, AvailableQty, ProductId;
                    int LogedInStoreID;

                     

                    int.TryParse(Session["UserSys"].ToString(), out LogedInStoreID);

                    Label lblTransferNo = (Label)gvReceiveTransfer.Rows[0].FindControl("lblRequestNo");
                    Label lblTransferDetailsID = (Label)gvReceiveTransfer.Rows[0].FindControl("lblTransferDetailsID");
                    Label lblRequestedQty = (Label)gvReceiveTransfer.Rows[0].FindControl("lblRequestedQty");
                    Label lblAvailableQty = (Label)gvReceiveTransfer.Rows[0].FindControl("lblAvailableQty");
                    Label lblSentQty = (Label)gvReceiveTransfer.Rows[0].FindControl("lblSentQty");
                    Label lblProductID = (Label)gvReceiveTransfer.Rows[0].FindControl("lblProductID");

                    int.TryParse(lblTransferNo.Text.ToString(), out TransferNo);
                    int.TryParse(lblTransferDetailsID.Text.ToString(), out TransferDetailNo);
                    int.TryParse(lblRequestedQty.Text.ToString(), out RequestedQty);

                    int.TryParse(lblAvailableQty.Text.ToString(), out AvailableQty);
                    int.TryParse(lblSentQty.Text.ToString(), out TransferedQty);
                    int.TryParse(lblProductID.Text.ToString(), out ProductId);

                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    //SqlCommand command = new SqlCommand("sp_UpdateTransferOrderDetials_AcceptAll", connection);
                    //command.Parameters.AddWithValue("@p_TransferID", TransferNo);
                    //command.Parameters.AddWithValue("@p_TransferDetID", TransferDetailNo);
                    //command.Parameters.AddWithValue("@p_RequestedQty", RequestedQty);
                    //command.Parameters.AddWithValue("@p_TransferedQty", TransferedQty);
                    //command.Parameters.AddWithValue("@p_AvailableQty", AvailableQty);
                    //command.Parameters.AddWithValue("@p_Status", "Accepted");
                    //command.Parameters.AddWithValue("@p_LogedinnStore", LogedInStoreID);
                    //command.Parameters.AddWithValue("@p_ProductID", ProductId);

                    SqlCommand command = new SqlCommand("sp_UpdateTransferOrderDetials_Generate", connection);

                    command.Parameters.AddWithValue("@p_LogedinnStore", LogedInStoreID);
                    command.Parameters.AddWithValue("@p_SystemID", DBNull.Value);
                    command.Parameters.AddWithValue("@p_TransferID", TransferNo);

                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dsResults);

                    Session["TransferRequestGrid"] = dsResults.Tables[0];

                    Response.Redirect("GenerateAcceptedTransferOrder.aspx", false);
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

        }

        protected void repReceiveTransfer_ItemCreated(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("StoreMain.aspx", false);
        }

        protected void ddPharmacy_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int PharmacyId;
                if(this.ddPharmacy.SelectedValue.ToString() == "All")
                {
                    PharmacyId = 1;
                }
                else
                {
                    PharmacyId = int.Parse(this.ddPharmacy.SelectedValue.ToString());
                }

                LoadRepeater(PharmacyId);
            }
            catch  
            {
                  
            }
        }
  
 
    }
}