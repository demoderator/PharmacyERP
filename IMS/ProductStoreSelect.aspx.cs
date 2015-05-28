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
    public partial class ProductStoreSelect : System.Web.UI.Page
    {
        DataSet ds;
        public static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSConnectionString"].ToString());
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    btnContinue.Visible = false;
                    Session.Remove("dsStoresPopup");
                   // Session.Remove("dsProducts_MP");
                   // BindGrid();

                }
                catch (Exception exp) { }
            }
        }
        private void BindGrid()
        {
            
            try
            {
                if (connection.State == ConnectionState.Closed) 
                {
                    connection.Open();
                }
                SqlCommand command = new SqlCommand("Sp_GetSystem_ByID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_SystemID", DBNull.Value);


                DataSet ds = new DataSet();
                SqlDataAdapter dA = new SqlDataAdapter(command);
                dA.Fill(ds);

            }
            catch (Exception exp) { }
            finally 
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            string storeName = txtStore.Text;
            Session["StoreName"] = storeName;
            Session["StoreID"] = lblStoreId.Text;
            Response.Redirect("Prod2Store.aspx",false);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            String Text = txtStore.Text + '%';
            Session["txtStore"] = Text;
            StoresPopupGrid.PopulateGrid();
            mpeCongratsMessageDiv.Show();
            
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Session.Remove("StoreName");
            Session.Remove("StoreID");
            Response.Redirect("WarehouseMain.aspx", false);
        }


    }
}