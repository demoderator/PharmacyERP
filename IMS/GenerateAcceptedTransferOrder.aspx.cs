﻿using System;
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
    public partial class GenerateAcceptedTransferOrder : System.Web.UI.Page
    {
        public static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSConnectionString"].ToString());
        public static DataSet ProductSet;
        public static DataTable distinctStores = new DataTable();
        public static DataTable dsDistinct = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dsDistinct = (DataTable)Session["TransferRequestGrid"];
                distinctStores = dsDistinct.DefaultView.ToTable(true, "SystemID");
                drpTransferDetailsReport.DataSource = distinctStores;
                drpTransferDetailsReport.DataBind();

            }
        }

        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            Session.Remove("TransferRequestGrid");
            if (!Session["UserRole"].ToString().Equals("WareHouse"))
            {
                Response.Redirect("ReceiveTransferOrder.aspx", false);
            }
            else
            {
                Response.Redirect("RespondStoreRequest.aspx", false);
            }
        }
        protected void drpTransferDetailsReport_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                DataTable dtGridSource = new DataTable();
                dtGridSource.Columns.Add("ProductID");
                dtGridSource.Columns.Add("SystemID");
                dtGridSource.Columns.Add("Product_Name");
                dtGridSource.Columns.Add("RequestedFrom");
                dtGridSource.Columns.Add("RequestedTo");
                dtGridSource.Columns.Add("TransferedQty");
                dtGridSource.Columns.Add("SentQty");

                int StoreId = Convert.ToInt32(((DataRowView)e.Item.DataItem).Row[0].ToString());
                DataSet ds = new DataSet();
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand command = new SqlCommand("sp_GetSystems_ByID", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@p_StoreId", StoreId);

                SqlDataAdapter sA = new SqlDataAdapter(command);
                sA.Fill(ds);

                Label lblFROMSystemName = (Label)e.Item.FindControl("lblFROMSystemName");
                Label lblFROMSystemAddress = (Label)e.Item.FindControl("lblFROMSystemAddress");
                Label lblFROMSystemPhone = (Label)e.Item.FindControl("lblFROMSystemPhone");
                Label lblFROMSystemEmail = (Label)e.Item.FindControl("lblFROMSystemEmail");
                Label lblToSystemName = (Label)e.Item.FindControl("lblToSystemName");
                Label lblToSystemAddress = (Label)e.Item.FindControl("lblToSystemAddress");
                Label lblToSystemPhone = (Label)e.Item.FindControl("lblToSystemPhone");
                Label lblToSystemEmail = (Label)e.Item.FindControl("lblToSystemEmail");

                lblFROMSystemName.Text = ds.Tables[0].Rows[0]["FROMName"].ToString();
                lblFROMSystemAddress.Text = ds.Tables[0].Rows[0]["FROMAdress"].ToString();
                lblFROMSystemPhone.Text = ds.Tables[0].Rows[0]["FROMPhone"].ToString();
                lblFROMSystemEmail.Text = ds.Tables[0].Rows[0]["FROMFax"].ToString();
                lblToSystemName.Text = ds.Tables[0].Rows[0]["ToName"].ToString();
                lblToSystemAddress.Text = ds.Tables[0].Rows[0]["ToAddress"].ToString();
                lblToSystemPhone.Text = ds.Tables[0].Rows[0]["ToPhone"].ToString();
                lblToSystemEmail.Text = ds.Tables[0].Rows[0]["ToFax"].ToString();


                GridView dgvTransferDisplay = (GridView)e.Item.FindControl("dgvTransferDisplay");

                DataRow[] drList = dsDistinct.Select("SystemID = " + StoreId);

                //dtGridSource.DefaultView.RowFilter = "SystemID = " + StoreId;
                //dtGridSource.DefaultView.ToTable();
                foreach (DataRow dr in drList)
                {
                    dtGridSource.Rows.Add(dr.ItemArray);
                }
                dtGridSource.AcceptChanges();
                dgvTransferDisplay.DataSource = dtGridSource;
                dgvTransferDisplay.DataBind();

            }
            catch
            {

            }
            finally
            {
                connection.Close();
            }
        }
    }
}