﻿using AjaxControlToolkit;
using IMSBusinessLogic;
using IMSCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IMS.UserControl
{
    public partial class VendorsPopupGrid : System.Web.UI.UserControl
    {
        DataSet ds;
        public static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSConnectionString"].ToString());
        public static DataSet ProductSet;
        bool selectSearch = false;

        public bool SelectSearch
        {
           // get { return selectSearch; }
            set { selectSearch = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    BindGrid();
                     if ((ViewState["displaySearch"] != null && ((bool)ViewState["displaySearch"]) == true)) 
                     {
                         lblSelectVendor.Visible = true;
                         txtVendor.Visible = true;
                         btnSearch.Visible = true;
                     }
                }
                catch (Exception exp) { }
            }
        }

       
        public void PopulateGrid()
        {
            if (Session["txtVendor"] != null)
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                #region Getting Product Details
                try
                {
                    int id;
                    if (int.TryParse(Session["UserSys"].ToString(), out id))
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        SqlCommand command = new SqlCommand("dbo.Sp_GetVendorByName", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        if (Session["Text"] != null)
                        {
                            command.Parameters.AddWithValue("@p_Supp_Name", Session["Text"].ToString());
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@p_Supp_Name", DBNull.Value);
                        }
                        command.Parameters.AddWithValue("@p_SysID", id);
                        if (!Session["UserRole"].ToString().Equals("Store"))
                        {
                            command.Parameters.AddWithValue("@p_isStore", false);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@p_isStore", true);
                        }
                        SqlDataAdapter SA = new SqlDataAdapter(command);

                        ProductSet = null;
                        SA.Fill(ds);
                        
                        gdvVendor.DataSource = ds;
                        gdvVendor.DataBind();
                    }

                }
                catch (Exception ex)
                {
                }
                finally
                {
                    connection.Close();
                }
                #endregion
            }
        }
        public void PopulateWithSearch() 
        {
            ViewState["displaySearch"] = true;
            lblSelectVendor.Visible = true;
            txtVendor.Visible = true;
            btnSearch.Visible = true;
            BindGrid();
        }
        private void BindGrid()
        {
            int id;
            DataSet ds1 = new DataSet();
            if (int.TryParse(Session["UserSys"].ToString(), out id))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand command = new SqlCommand("dbo.Sp_GetVendorByName", connection);
                command.CommandType = CommandType.StoredProcedure;
                if (Session["Text"] != null)
                {
                    command.Parameters.AddWithValue("@p_Supp_Name", Session["Text"].ToString());
                }
                else
                {
                    command.Parameters.AddWithValue("@p_Supp_Name", DBNull.Value);
                }
                command.Parameters.AddWithValue("@p_SysID", id);
                if (!Session["UserRole"].ToString().Equals("Store"))
                {
                    command.Parameters.AddWithValue("@p_isStore", false);
                }
                else
                {
                    command.Parameters.AddWithValue("@p_isStore", true);
                }
                SqlDataAdapter SA = new SqlDataAdapter(command);

                ProductSet = null;
                SA.Fill(ds1);

                ProductSet = ds1;
                gdvVendor.DataSource = null;
                gdvVendor.DataSource = ds1;
                gdvVendor.DataBind();
            } 
        }

        protected void gdvVendor_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gdvVendor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gdvVendor_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gdvVendor.PageIndex = e.NewPageIndex;
            if (Session["txtVendor"] != null)
            {
                PopulateGrid();
            }
            else
            {
                BindGrid();
            }
            ModalPopupExtender mpe = (ModalPopupExtender)this.Parent.FindControl("mpeCongratsMessageDiv");
            mpe.Show();
            
        }

        protected void gdvVendor_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            VendorBLL _vendorBll = new VendorBLL();
            if (e.CommandName.Equals("CheckChanged"))
            {
                Label ID = (Label)gdvVendor.Rows[0].FindControl("lblSupID");
                int SupID = int.Parse(ID.Text);
                if (SupID > 0)
                {

                }
                
            }
        }

        protected void gdvVendor_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //gdvVendor.EditIndex = -1;
            //BindGrid();
        }

        protected void gdvVendor_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                Label ID = (Label)gdvVendor.Rows[e.RowIndex].FindControl("lblSupID");
                int id = int.Parse(ID.Text);
                Vendor vendor = new Vendor();//= empid.Text;
                vendor.supp_ID = id;
                ds = VendorBLL.GetDistinct(connection, vendor);

                Session["VendorName"] = ds.Tables[0].Rows[0]["SupName"];
                Session["VendorId"] = ds.Tables[0].Rows[0]["SuppID"];

                Control ctl = this.Parent;
                TextBox ltMetaTags = null;
                ltMetaTags = (TextBox)ctl.FindControl("txtVendor");
                if (ltMetaTags != null)
                {
                    ltMetaTags.Text = ds.Tables[0].Rows[0]["SupName"].ToString();
                }
            }
            catch (Exception exp) { }
             
        }

        protected void SelectVendor_Click(object sender, EventArgs e)
        {

            GridViewRow rows = gdvVendor.SelectedRow;
            foreach (GridViewRow row in gdvVendor.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkRow = (row.Cells[0].FindControl("chkCtrl") as CheckBox);
                    if (chkRow.Checked)
                    {
                        Control ctl = this.Parent;
                        if ((ViewState["displaySearch"] != null && ((bool)ViewState["displaySearch"]) == true))
                        {

                            string val=  row.Cells[1].Text;
                            Label id = row.Cells[8].FindControl("lblSupID") as Label;
                            string val2= id.Text;
                            Session["Rep_Params"] = val+"~"+val2;
                        }
                        else
                        {
                            TextBox ltMetaTags = null;
                            Button btnContinue = (Button)ctl.FindControl("btnContinue");
                            // Label lblVendirId = (Label)
                            btnContinue.Visible = true;
                            ltMetaTags = (TextBox)ctl.FindControl("txtVendor");
                            if (ltMetaTags != null)
                            {
                                ltMetaTags.Text = Server.HtmlDecode(row.Cells[1].Text);
                            }
                        }
                    }
                }
            }

            Session.Remove("txtVendor");
        }

        protected void chkCtrl_CheckedChanged(object sender, EventArgs e)
        {
            if(sender is CheckBox)
            {

            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            int id;
            if (int.TryParse(Session["UserSys"].ToString(), out id))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlCommand command = new SqlCommand("dbo.Sp_GetVendorByName", connection);
                command.CommandType = CommandType.StoredProcedure;
                if (txtVendor.Text != null)
                {
                    command.Parameters.AddWithValue("@p_Supp_Name", txtVendor.Text);
                }
                else
                {
                    command.Parameters.AddWithValue("@p_Supp_Name", DBNull.Value);
                }
                command.Parameters.AddWithValue("@p_SysID", id);
                if (!Session["UserRole"].ToString().Equals("Store"))
                {
                    command.Parameters.AddWithValue("@p_isStore", false);
                }
                else
                {
                    command.Parameters.AddWithValue("@p_isStore", true);
                }
                SqlDataAdapter SA = new SqlDataAdapter(command);

                ProductSet = null;
                SA.Fill(ds);

                ProductSet = ds;
                gdvVendor.DataSource = null;
                gdvVendor.DataSource = ds;
                gdvVendor.DataBind();

                ModalPopupExtender mpe = (ModalPopupExtender)this.Parent.FindControl("mpeCongratsMessageDiv");
                mpe.Show();
            } 
        }
    }
}