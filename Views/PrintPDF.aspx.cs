using Octopus.Library.Utils;
using OfficialCeisaLite.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OfficialCeisaLite.Views
{
    public partial class PrintPDF : System.Web.UI.Page
    {
        NbLogger logger = new NbLogger(AppDomain.CurrentDomain.BaseDirectory + @"/Log/", "TPB LOAD PDF");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = (DataTable)Session["PrintPDF"];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        GetPdf(row["keterangan"].ToString(), row["HAWB"].ToString(), row["encode_pdf"].ToString());
                        break;
                    }                    
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }

        private void GetPdf(string file_type, string awb, string pdf)
        {
            try
            {
                byte[] pdfBytes = Convert.FromBase64String(pdf);
                Response.AddHeader("Content-Type", "application/pdf");
                Response.AddHeader("Content-Length", pdf.Length.ToString());
                Response.AddHeader("Content-Disposition", $"inline;filename={file_type}_{awb}.pdf");
                Response.AddHeader("Cache-Control", "private, max-age=0, must-revalidate");
                Response.AddHeader("Pragma", "public");
                Response.BinaryWrite(pdfBytes);
                Response.End();
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
    }
}