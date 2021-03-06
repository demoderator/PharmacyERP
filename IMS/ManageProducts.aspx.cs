﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using IMSCommon.Util;
using log4net;
using IMS.Util;

namespace IMS
{
    public partial class ManageProducts : System.Web.UI.Page
    {
        public static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSConnectionString"].ToString());
        public static DataSet ProductSet;
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
                    string searchVal = txtSearch.Text;
                    if (!string.IsNullOrEmpty(searchVal))
                    {
                        //do here 
                        //REPLY  (30-JULY-2014) - Moiz : OK!!!
                    }
                    BindGrid();

                    if (IsWarehouse().Equals(false))
                    {
                        btnAddProduct.Enabled = false;
                    }
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
        private void BindGrid(String SearchText="")
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
                        connection.Open();
                    String Query = null;
                    SqlCommand command = null;
                    if (Convert.ToInt32(Session["UserSys"]).Equals(1))
                    {
                        Query = "sp_GetProductsList";
                        command = new SqlCommand(Query, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_SearchText", SearchText);
                    }
                    else
                    {
                        command = new SqlCommand("Sp_GetProductStoreMapping", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_StoreID", id);
                        command.Parameters.AddWithValue("@p_SearchText", SearchText);
                    }
                    SqlDataAdapter SA = new SqlDataAdapter(command);                    
                   
                  //  ProductSet = null;
                    
                    

                    SA.Fill(ds);
                    Session["dsProducts_MP"] = ds;
                  //  ProductSet = ds;
                    StockDisplayGrid.DataSource = ds;
                    StockDisplayGrid.DataBind();
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
        }
        protected Boolean IsWarehouse()
        {
            int id;
            int.TryParse(Session["UserSys"].ToString(), out id);

            if (Session["UserRole"].ToString().Equals("WareHouse"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //protected void Edit(Object sender, CommandEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("Edit");
        //}

        protected void btnManageProducts_Click(object sender, EventArgs e)
        {

        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            Session["MODE"] = null;
            Session["MODE"] = "ADD";
            Session.Remove("dsProducts_MP");
            Session["PageMasterProduct"] = "false";
            
            //Session.Remove("MS_ItemNo");
            //Session.Remove("MS_ItemName");
            //Session.Remove("MS_ItemType");
            //Session.Remove("MS_Manufacterer");
            ////Session.Remove("MS_Category");
            //Session.Remove("MS_GenericName");
            //Session.Remove("MS_Control");
            //Session.Remove("MS_BinNumber");
            //Session.Remove("MS_GreenRainCode");
            //Session.Remove("MS_BrandName");
            //Session.Remove("MS_MaxiMumDiscount");
            //Session.Remove("MS_LineID");
            //Session.Remove("MS_UnitSale");
            //Session.Remove("MS_UnitCost");
            //Session.Remove("MS_itemAWT");
            //Session.Remove("MS_itemForm");
            //Session.Remove("MS_itemStrength");
            //Session.Remove("MS_itemPackType");
            //Session.Remove("MS_itemPackSize");
            //Session.Remove("MS_Description");
            //Session.Remove("MS_Bonus12");
            //Session.Remove("MS_Bonus25");
            //Session.Remove("MS_Bonus50");
            //Session.Remove("MS_Active");
             
            
                                        


            Response.Redirect("AddProduct.aspx",false);
        }

        protected void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            Session["Linkto"] = "DELETE";
            Response.Redirect("Edit_DeleteProduct.aspx", false);
        }

        protected void btnEditProduct_Click(object sender, EventArgs e)
        {
            Session["Linkto"] = "EDIT";
            Response.Redirect("Edit_DeleteProduct.aspx", false);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(Session["UserSys"]).Equals(1))
            {
                Response.Redirect("WarehouseMain.aspx", false);
            }
            else
            {
                Response.Redirect("StoreMain.aspx", false);
            }
            //Response.Redirect("ManageInventory.aspx", false);
        }

        protected void btnStocks_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageStocks.aspx", false);
        }

        #region GridView Functions & Events
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
                if (e.CommandName.Equals("Edit"))
                {
                    #region Updating Product
                    Label ItemNo = (Label)StockDisplayGrid.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("UPC");
                    Label ItemName = (Label)StockDisplayGrid.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("ProductName");
                    Label ItemType = (Label)StockDisplayGrid.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("Type");
                    Label GreenRainCode = (Label)StockDisplayGrid.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("GreenRain");
                    Label UnitSale = (Label)StockDisplayGrid.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("lblUnitSalePrice");
                    Label UnitCost = (Label)StockDisplayGrid.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("UnitCost");
                    Label lblSubCategoryID = (Label)StockDisplayGrid.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("lblSubCategoryID");

                    #region Get values for Category, Sub Category and Department against Sub Category ID

                    DataSet dsDropdownValues = new DataSet();
                    int SubCategoryID = 0;
                    int.TryParse(lblSubCategoryID.Text.ToString(), out SubCategoryID);
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand command = new SqlCommand("sp_getSubCategoryCategoryDepartment", connection);
                    command.Parameters.Add("@p_SubCategoryID", SubCategoryID);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter dA = new SqlDataAdapter(command);
                    dA.Fill(dsDropdownValues);

                    //set session values for drop downs
                    Session["SubCategoryID"] = SubCategoryID;
                    if (dsDropdownValues.Tables[0].Rows.Count >0)
                    {
                        Session["CategoryID"] = dsDropdownValues.Tables[0].Rows[0]["CategoryID"].ToString();
                    }
                    else
                    {
                        Session["CategoryID"] = string.Empty;
                    }
                    if (dsDropdownValues.Tables[0].Rows.Count > 0)
                    {
                        Session["DepartmentID"] = dsDropdownValues.Tables[0].Rows[0]["DepId"].ToString();
                    }
                    else
                    {
                       
                        Session["DepartmentID"] = string.Empty;
                    }
                    #endregion
                    
                     
                    

                    Session["PageMasterProduct"] = "true";
                    Session["MODE"] = "EDIT";
                   
                    Session["MS_ItemNo"] = ItemNo.Text.ToString();
                    Session["MS_ItemName"] = ItemName.Text.ToString();
                    Session["MS_ItemType"] = ItemType.Text.ToString();

                    DataSet dsProducts = (DataSet)Session["dsProducts_MP"];
                    DataView dv = dsProducts.Tables[0].DefaultView;
                   // DataView dv = ProductSet.Tables[0].DefaultView;
                    dv.RowFilter = "Product_Id_Org = '"+ ItemNo.Text + "'";
                    DataTable dt = dv.ToTable();
                    if (dt.Rows.Count != 0)
                    {
                        Session["DrugType"] = dt.Rows[0]["DrugType"].ToString();
                        Session["MS_Manufacterer"] = "";
                        Session["MS_Category"] = "";
                        Session["MS_Description"] = dt.Rows[0]["Description"] != null ? dt.Rows[0]["Description"].ToString() : string.Empty;
                        Session["MS_GenericName"] = dt.Rows[0]["GName"] != null ? dt.Rows[0]["GName"].ToString() : string.Empty;
                        Session["MS_Control"] = dt.Rows[0]["Control"] != null ? dt.Rows[0]["Control"].ToString() : string.Empty;
                        Session["MS_BinNumber"] = dt.Rows[0]["binNumber"] != null ? dt.Rows[0]["binNumber"].ToString() : string.Empty;
                        Session["MS_GreenRainCode"] = GreenRainCode.Text.ToString();
                        Session["MS_BrandName"] = dt.Rows[0]["Brand_Name"] != null ? dt.Rows[0]["Brand_Name"].ToString() : string.Empty;
                        Session["MS_MaxiMumDiscount"] = dt.Rows[0]["MaxiMumDiscount"] != null ? dt.Rows[0]["MaxiMumDiscount"].ToString() : string.Empty;
                        Session["MS_LineID"] = dt.Rows[0]["LineID"] != null ? dt.Rows[0]["LineID"].ToString() : string.Empty;
                        Session["MS_UnitSale"] = UnitSale.Text.ToString();
                        Session["MS_UnitCost"] = UnitCost.Text.ToString();
                        Session["MS_itemAWT"] = dt.Rows[0]["itemAWT"] != null ? dt.Rows[0]["itemAWT"].ToString() : string.Empty;
                        Session["MS_itemForm"] = dt.Rows[0]["itemForm"] != null ? dt.Rows[0]["itemForm"].ToString() : string.Empty;
                        Session["MS_itemStrength"] = dt.Rows[0]["itemStrength"] != null ? dt.Rows[0]["itemStrength"].ToString() : string.Empty;
                        Session["MS_itemPackType"] = dt.Rows[0]["itemPackType"] != null ? dt.Rows[0]["itemPackType"].ToString() : string.Empty;
                        Session["MS_itemPackSize"] = dt.Rows[0]["itemPackSize"] != null ? dt.Rows[0]["itemPackSize"].ToString() : string.Empty;
                        Session["MS_ProductID"] = dt.Rows[0]["ProductID"] != null ? dt.Rows[0]["ProductID"].ToString() : string.Empty;
                        Session["MS_ProductOrderType"] = dt.Rows[0]["productOrderType"] != null ? dt.Rows[0]["productOrderType"].ToString() : string.Empty;
                        Session["MS_Bonus12"] = dt.Rows[0]["Bonus12Quantity"] != null ? dt.Rows[0]["Bonus12Quantity"].ToString() : string.Empty;
                        Session["MS_Bonus25"] = dt.Rows[0]["Bonus25Quantity"] != null ? dt.Rows[0]["Bonus25Quantity"].ToString() : string.Empty;
                        Session["MS_Bonus50"] = dt.Rows[0]["Bonus50Quantity"] != null ? dt.Rows[0]["Bonus50Quantity"].ToString() : string.Empty;
                        if (dt.Rows[0]["Active"] != null)
                        {
                            Session["MS_Active"] = dt.Rows[0]["Active"] != null ? dt.Rows[0]["Active"].ToString() : string.Empty;
                        }
                        else
                        {
                            Session["MS_Active"] = "1";
                        }
                        Response.Redirect("Addproduct.aspx", false);
                    }
                    else 
                    {
                        if (!Session["UserRole"].ToString().Equals("WareHouse")) 
                        {
                            WebMessageBoxUtil.Show("Product is not associated with this store");
                        }
                    }

                    #endregion

                }
                else if (e.CommandName.Equals("Delete"))
                {

                    #region Delete product
                    try
                    {
                            Label ItemNo = (Label)StockDisplayGrid.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("UPC");
                            if (connection.State == ConnectionState.Closed)
                                connection.Open();
                            SqlCommand command = new SqlCommand("sp_DeleteProduct", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            
                            int res6 =0;

                            DataSet dsProducts = (DataSet)Session["dsProducts_MP"];
                            DataView dv = dsProducts.Tables[0].DefaultView;
                            //DataView dv = ProductSet.Tables[0].DefaultView;
                            dv.RowFilter = "Product_Id_Org = '" + ItemNo.Text + "'";
                            DataTable dt = dv.ToTable();
                            Session["MS_ProductID"] = dt.Rows[0]["ProductID"].ToString();
                        
                            if (int.TryParse(Session["MS_ProductID"].ToString(), out res6))
                            {
                                command.Parameters.AddWithValue("@p_ProductID", res6);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@p_ProductID", 0);
                            }
                            int x = command.ExecuteNonQuery();
                            if (x == 1)
                            {
                                WebMessageBoxUtil.Show("Record Removed Successfully");
                            }
                            else 
                            {
                                WebMessageBoxUtil.Show("Record cannot be removed as stock exists against this value");
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

                    BindGrid();
                }
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

        protected void StockDisplayGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void StockDisplayGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void StockDisplayGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            StockDisplayGrid.EditIndex = e.NewEditIndex;
           // BindGrid();
        }

        protected void StockDisplayGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        #endregion

       
        protected void btnGoBack_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(Session["UserSys"]).Equals(1))
            {
                Response.Redirect("WarehouseMain.aspx", false);
            }
            else
            {
                Response.Redirect("StoreMain.aspx", false);
            }
        }

        

        protected void btnSearchProduct_Click(object sender, ImageClickEventArgs e)
        {
            String Text = txtSearch.Text;
            Session["Text"] = Text;
          //  ProductsPopupGrid.PopulateGridForPM();
          //  mpeCongratsMessageDiv.Show();

            BindGrid(Text);

        }
    }
}