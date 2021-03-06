﻿using IMS.Util;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IMS
{
    public partial class StockManipulationPrint : System.Web.UI.Page
    {
        public static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSConnectionString"].ToString());
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
                    BindGridbyFilters();
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
        public void BindGridbyFilters()
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            #region Getting Product Details
            //try
            //{
            //    int id;
            //    if (int.TryParse(Session["UserSys"].ToString(), out id))
            //    {

            //        if (connection.State == ConnectionState.Closed)
            //        {
            //            connection.Open();
            //        }
            //        SqlCommand command = new SqlCommand("sp_ViewInventory_byFilters", connection);
            //        #region with parameter approach
            //        #region Filters with null value
            //        command.CommandType = CommandType.StoredProcedure;
            //        //command.Parameters.AddWithValue("@p_SysID", id);
            //        //command.Parameters.AddWithValue("@p_DeptID", DBNull.Value);
            //        //command.Parameters.AddWithValue("@p_CatID", DBNull.Value);
            //        //command.Parameters.AddWithValue("@p_SubCatID", DBNull.Value);
            //        //command.Parameters.AddWithValue("@p_productOrderType", DBNull.Value);
            //        //command.Parameters.AddWithValue("@p_ProdType", DBNull.Value);
            //        //command.Parameters.AddWithValue("@p_ProdID", DBNull.Value);
            //        //command.Parameters.AddWithValue("@p_isActive", DBNull.Value);
            //        //command.Parameters.AddWithValue("@p_prodName", DBNull.Value);
            //        //command.Parameters.AddWithValue("@p_isPrint", 1); 
            //        #endregion
            //        #region  Filtered Conditions
                   
            //        int prodID;
            //        int depID, catID, subCatID, prodIDOrg, prodType = 0;
            //        int.TryParse(Session["Search_DepID"].ToString(), out depID);
            //        int.TryParse(Session["Search_CatID"].ToString(), out catID);
            //        int.TryParse(Session["Search_SubCatID"].ToString(), out subCatID);
            //        int.TryParse(Session["Search_ProdIdOrg"].ToString(), out prodIDOrg);
            //        int.TryParse(Session["Search_ProdType"].ToString(), out prodType);
            //        int.TryParse(Session["Search_ProdId"].ToString(), out prodID);

            //        command.Parameters.AddWithValue("@p_SysID", id);
            //        if (depID <= 0)
            //        {
            //            command.Parameters.AddWithValue("@p_DeptID", DBNull.Value);

            //        }
            //        else
            //        {
            //            command.Parameters.AddWithValue("@p_DeptID", depID);
            //        }

            //        if (catID <= 0)
            //        {
            //            command.Parameters.AddWithValue("@p_CatID", DBNull.Value);
            //        }
            //        else
            //        {
            //            command.Parameters.AddWithValue("@p_CatID", catID);
            //        }

            //        if (subCatID <= 0)
            //        {
            //            command.Parameters.AddWithValue("@p_SubCatID", DBNull.Value);
            //        }
            //        else
            //        {
            //            command.Parameters.AddWithValue("@p_SubCatID", subCatID);
            //        }

            //        if (prodIDOrg <= 0)
            //        {
            //            command.Parameters.AddWithValue("@p_productOrderType", DBNull.Value);
            //        }
            //        else
            //        {
            //            command.Parameters.AddWithValue("@p_productOrderType", prodIDOrg);
            //        }


            //        if (prodType <= 0)
            //        {
            //            command.Parameters.AddWithValue("@p_ProdType", DBNull.Value);
            //        }
            //        else
            //        {
            //            command.Parameters.AddWithValue("@p_ProdType", prodType);
            //        }

            //        if (prodID <= 0)
            //        {
            //            command.Parameters.AddWithValue("@p_ProdID", DBNull.Value);
            //        }
            //        else
            //        {
            //            command.Parameters.AddWithValue("@p_ProdID", prodID);
            //        }

            //        if (Session["Search_Active"] == null || String.IsNullOrEmpty(Session["Search_Active"].ToString()))
            //        {
            //            command.Parameters.AddWithValue("@p_isActive", DBNull.Value);
                        
            //        }
            //        else 
            //        {
            //            switch (Session["Search_Active"].ToString())
            //            {
            //                case "Select Option":

            //                    command.Parameters.AddWithValue("@p_isActive", DBNull.Value);
            //                    break;
            //                case "All Active":
            //                    command.Parameters.AddWithValue("@p_isActive", 1);
            //                    break;
            //                case "All in-Active":
            //                    command.Parameters.AddWithValue("@p_isActive", 0);
            //                    break;

            //                case "All Active & Non-Zero Stock":
            //                    command.Parameters.AddWithValue("@p_isActive", 3);
            //                    break;
            //            }
            //        }

                   
            //        if (Session["Search_ProdName"] == null || String.IsNullOrEmpty(Session["Search_ProdName"].ToString()))
            //        {
            //            command.Parameters.AddWithValue("@p_prodName", DBNull.Value);
            //        }
            //        else 
            //        {
            //            command.Parameters.AddWithValue("@p_prodName", Session["Search_ProdName"].ToString());
            //        }
            //        command.Parameters.AddWithValue("@p_isPrint", 1); 
            //        #endregion
                    
            //        #endregion

            //        SqlDataAdapter SA = new SqlDataAdapter(command);
            //        SA.Fill(ds);
            //        StockDisplayGrid.DataSource = ds;
            //        StockDisplayGrid.DataBind();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    if (connection.State == ConnectionState.Open)
            //        connection.Close();
            //    throw ex;
            //}
            //finally
            //{
            //    if (connection.State == ConnectionState.Open)
            //        connection.Close();
            //}

            DataTable Print = (DataTable)Session["Print"];

          

            dgvStockDisplayGrid.DataSource = Print;

            dgvStockDisplayGrid.DataBind();
            dgvStockDisplayGrid.HeaderRow.TableSection = TableRowSection.TableHeader;

            #endregion
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {

            Session["Print"] = null;
            Response.Redirect("StockManipulation.aspx?Param=Printing", false);
           
        }

        private void ExportGridToPDF()
        {
            try
            {
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "attachment;filename=PO_" + Session["OrderNumber"].ToString() + ".pdf");
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                StringWriter sw = new StringWriter();
                sw.WriteLine("");
                HtmlTextWriter hw = new HtmlTextWriter(sw);

                dgvStockDisplayGrid.RenderControl(hw);

                StringReader sr = new StringReader(sw.ToString());
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                pdfDoc.Open();

                htmlparser.Parse(sr);
                String FilePath = Server.MapPath(@"~\PurchaseOrders");
                String FileName = "Inventory" + ".pdf";
                FileStream fs = new FileStream(FilePath + @"\" + FileName, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, fs);
                writer.Close();
                pdfDoc.Close();
                fs.Close();
                //Response.Write(pdfDoc);

                //Response.Flush();
                //Response.SuppressContent = true; 
                //Response.End();
                //Response.Redirect("PO_GENEREATE.aspx");
                //StockDisplayGrid.AllowPaging = true;
                //StockDisplayGrid.DataBind();
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                throw ex;
            }
            finally
            {
                // MAINDIV.Visible = false;
                // TotalCostDiv.Visible = false;
                // btnEmail.Visible = true;
                //btnFax.Visible = true;
                //btnPrint.Enabled = false;
            }

        }
        protected void btnPrint_Click(object sender, EventArgs e)
        {

            ExportGridToPDF();

            Page_Load(sender, e);

        }

        protected void StockDisplayGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label ProductStrength = (Label)e.Row.FindControl("ProductStrength2");
                    Label Label1 = (Label)e.Row.FindControl("Label1");

                    Label dosage = (Label)e.Row.FindControl("dosage2");
                    Label Label2 = (Label)e.Row.FindControl("Label2");

                    Label packSize = (Label)e.Row.FindControl("packSize2");
                    Label Label3 = (Label)e.Row.FindControl("Label3");

                    if (String.IsNullOrWhiteSpace(ProductStrength.Text))
                    {
                        ProductStrength.Visible = false;
                        Label1.Visible = false;
                    }
                    else
                    {
                        ProductStrength.Visible = true;
                        Label1.Visible = true;
                    }

                    if (String.IsNullOrWhiteSpace(dosage.Text))
                    {
                        dosage.Visible = false;
                        Label2.Visible = false;
                    }
                    else
                    {
                        dosage.Visible = true;
                        Label2.Visible = true;
                    }

                    if (String.IsNullOrWhiteSpace(packSize.Text))
                    {
                        packSize.Visible = false;
                        Label3.Visible = false;
                    }
                    else
                    {
                        packSize.Visible = true;
                        Label3.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                
                //throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

            }
        }
    }
}