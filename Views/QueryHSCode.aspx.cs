using Octopus.Library.Utils;
using OfficialCeisaLite.App_Start;
using OfficialCeisaLite.Models;
using OfficialCeisaLite.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OfficialCeisaLite.Views
{
    public partial class QueryHSCode : System.Web.UI.Page
    {
        NbLogger logger = new NbLogger(AppDomain.CurrentDomain.BaseDirectory + @"/Log/", "QueryHS_Code");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }
        private void LoadData()
        {
            try
            {
                DbOfficialDCI.LoadParameter();

                //validate session
                string update_by = string.Empty;
                LoginData user = SessionUtils.GetUserData(this.Page);
                if (user != null)
                {
                    update_by = user.username;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Session user expired, please login again.');", true);
                    Response.Redirect("~/Auth/Login.aspx");
                    return;
                }

                string query = $@"select top 1000 
                                HS_CODE, BM, 
                                case 
	                                when PPH = 0 then 2.5
	                                else PPH
                                end as PPH, 
                                case 
	                                when PPN = 0 then 11
	                                else PPN
                                end as PPN, 
                                case 
	                                when isnull(IsBmtp,0) != 0 then cast(bmtp as float)
	                                else cast(0 as float)
                                end as BMTP,
                                convert(varchar(20),DE,20) as update_time,
                                UE as update_by
                                from HSCODE order by DE desc";
                DataTable dt = DbOfficialDCI.getRecords(query);
                ViewState["ListDataHSCode"] = dt;
                GV_HSCode.DataSource = dt;
                GV_HSCode.DataBind();
                totalData.InnerText = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                logger.Log($"err LoadData: {ex.Message}, {ex.StackTrace}");
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //validate session
                string gateway = string.Empty;
                LoginData user = SessionUtils.GetUserData(this.Page);
                if (user != null)
                {
                    gateway = user.location;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Session user expired, please login again.');", true);
                    Response.Redirect("~/Auth/Login.aspx");
                    return;
                }

                string query = $@"select top 1000 
                                HS_CODE, BM, 
                                case 
	                                when PPH = 0 then 2.5
	                                else PPH
                                end as PPH, 
                                case 
	                                when PPN = 0 then 11
	                                else PPN
                                end as PPN, 
                                case 
	                                when isnull(IsBmtp,0) != 0 then cast(bmtp as float)
	                                else cast(0 as float)
                                end as BMTP,
                                convert(varchar(20),DE,20) as update_time,
                                UE as update_by
                                from HSCODE";

                string hscode = srcHSCODE.Text.Trim();
                string parameter = string.Empty;

                if (string.IsNullOrEmpty(hscode) == false)
                {
                    List<string> listHSCODE = new List<string>();
                    string[] arr = hscode.Split('\n');
                    foreach (var item in arr)
                    {
                        listHSCODE.Add($@"'{item.Trim()}'");
                    }
                    hscode = String.Join(",", listHSCODE);
                    parameter = $@" where HS_CODE in ({hscode}) order by DE desc";
                    query += parameter;
                }
                else
                {
                    parameter = $@" order by DE desc";
                    query += parameter;
                }

                DataTable dt = DbOfficialDCI.getRecords(query);
                ViewState["ListDataHSCode"] = dt;
                GV_HSCode.DataSource = dt;
                GV_HSCode.DataBind();
                totalData.InnerText = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                logger.Log($"err btnSearch_Click: {ex.Message}, {ex.StackTrace}");
            }
        }

        protected void GV_HSCode_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_HSCode.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)ViewState["ListDataHSCode"];
            GV_HSCode.DataSource = dt;
            GV_HSCode.DataBind();
        }
    }
}