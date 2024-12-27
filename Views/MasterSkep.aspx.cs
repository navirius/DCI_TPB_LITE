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
    public partial class MasterSkep : System.Web.UI.Page
    {
        NbLogger logger = new NbLogger(AppDomain.CurrentDomain.BaseDirectory + @"/Log/", "TPB_MASTER_SKEP");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }
        private void LoadData()
        {
            DbOfficialDCI.LoadParameter();

            //validate session
            string update_by = string.Empty;
            string gateway = string.Empty;
            LoginData user = SessionUtils.GetUserData(this.Page);
            if (user != null)
            {
                update_by = user.username;
                gateway = user.location;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Session user expired, please login again.');", true);
                Response.Redirect("~/Auth/Login.aspx");
                return;
            }

            try
            {
                string query = $@"SELECT TOP 100
                                ID,
                                REPLACE(REPLACE(NPWP,'.',''),'-','') AS NPWP,
                                NAMACUSTOMER,
                                skkb AS NOSKEP,
                                ISNULL(tgl_skkb,'2000-01-01') AS TGLSKEP,
                                NONIB AS NIB,
                                ALAMAT1 AS ALAMAT,
                                ISNULL(tgl_active_skb_pph,'2000-01-01') AS TGLACTIVESKEP,
                                ISNULL(tgl_expired_skb_pph,'2000-01-01') AS TGLEXPIREDSKEP,
                                UE,DE
                                FROM CUSTOMER
                                WHERE LEN(REPLACE(REPLACE(NPWP,'.',''),'-','')) = 15
                                ORDER BY ID DESC";
                DataTable dt = DbOfficialDCI.getRecords(query);
                ViewState["ListDataSkep"] = dt;
                GV_MasterSkep.DataSource = dt;
                GV_MasterSkep.DataBind();
                totalData.InnerText = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                new ArgumentException(ex.Message);
            }
        }
        protected void GV_MasterSkep_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_MasterSkep.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)ViewState["ListDataSkep"];
            GV_MasterSkep.DataSource = dt;
            GV_MasterSkep.DataBind();
        }

        protected void GV_MasterSkep_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow")
                {
                    //get id
                    string key = e.CommandArgument.ToString();

                    if (e.CommandName == "EditRow")
                    {
                        //display data
                        ViewRecord(key);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log($"{ex.Message},{ex.StackTrace}");
            }
        }
        protected void ViewRecord(string ID)
        {
            try
            {
                string query = $@"SELECT 
                                NAMACUSTOMER,
                                skkb AS NOSKEP,
                                ISNULL(tgl_skkb,'2000-01-01') AS TGLSKEP,
                                NONIB AS NIB,
                                ISNULL(tgl_active_skb_pph,'2000-01-01') AS TGLACTIVESKEP,
                                ISNULL(tgl_expired_skb_pph,'2000-01-01') AS TGLEXPIREDSKEP
                                FROM CUSTOMER WHERE ID = '{ID}'";
                DataRow dr = DbOfficialDCI.getRow(query);

                txtNamaCust.Text = dr["NAMACUSTOMER"].ToString();
                txtNoSkep.Text = dr["NOSKEP"].ToString();
                txtTglSkep.Text = dr["TGLSKEP"].ToString();
                txtNoNIB.Text = dr["NIB"].ToString();
                txtTglAktif.Text = dr["TGLACTIVESKEP"].ToString();
                txtTglExpired.Text = dr["TGLEXPIREDSKEP"].ToString();

                //disable edit username
                txtNamaCust.ReadOnly = true;

                ViewState["IDCUSTOMER"] = ID;
                ShowModal();
            }
            catch (Exception ex)
            {
                logger.Log($"{ex.Message},{ex.StackTrace}");
            }
        }
        private void ShowModal()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "popup", "$('#btnPopup').click();", true);
        }
        private void CloseModal()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "close", "$('#btnClose').click();", true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
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

                string ID = ViewState["IDCUSTOMER"].ToString();

                string query = $@"UPDATE CUSTOMER SET 
                                skkb = '{txtNoSkep.Text.Trim()}',
                                tgl_skkb = '{txtTglSkep.Text.Trim()}',
                                NONIB = '{txtNoNIB.Text.Trim()}',
                                tgl_active_skb_pph = '{txtTglAktif.Text.Trim()}',
                                tgl_expired_skb_pph = '{txtTglExpired.Text.Trim()}',
                                UE = '{update_by}',
                                DE = '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss")}'
                                WHERE ID = '{ID}'";
                logger.Log(query);

                if (DbOfficialDCI.runCommand(query) == 0)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Update Data SKEP Success.');", true);
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Update Data SKEP Failed.');", true);

                CloseModal();
                LoadData();

                ViewState["IDCUSTOMER"] = null;
            }
            catch (Exception ex)
            {
                logger.Log($"{ex.Message},{ex.StackTrace}");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //validate session
                string username = string.Empty;
                LoginData user = SessionUtils.GetUserData(this.Page);
                if (user != null)
                {
                    username = user.location;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Session user expired, please login again.');", true);
                    Response.Redirect("~/Auth/Login.aspx");
                    return;
                }

                string query = $@"SELECT TOP 100
                                ID,
                                REPLACE(REPLACE(NPWP,'.',''),'-','') AS NPWP,
                                NAMACUSTOMER,
                                skkb AS NOSKEP,
                                ISNULL(tgl_skkb,'2000-01-01') AS TGLSKEP,
                                NONIB AS NIB,
                                ALAMAT1 AS ALAMAT,
                                ISNULL(tgl_active_skb_pph,'2000-01-01') AS TGLACTIVESKEP,
                                ISNULL(tgl_expired_skb_pph,'2000-01-01') AS TGLEXPIREDSKEP,
                                UE,DE
                                FROM CUSTOMER
                                WHERE LEN(REPLACE(REPLACE(NPWP,'.',''),'-','')) = 15";

                string npwp = srcNPWP.Text.Trim().Replace(".","").Replace("-","");
                string nama = srcNama.Text.Trim();
                string parameter = string.Empty;

                if (string.IsNullOrEmpty(npwp) == false)
                {
                    parameter = $@" AND REPLACE(REPLACE(NPWP,'.',''),'-','') = '{npwp}' order by ID desc";
                    query += parameter;
                }
                else if (string.IsNullOrEmpty(nama) == false)
                {
                    parameter = $@" AND NAMACUSTOMER LIKE '%{nama}%' order by ID desc";
                    query += parameter;
                }
                else
                {
                    parameter = $@" order by ID desc";
                    query += parameter;
                }

                DataTable dt = DbOfficialDCI.getRecords(query);
                ViewState["ListDataSkep"] = dt;
                GV_MasterSkep.DataSource = dt;
                GV_MasterSkep.DataBind();
                totalData.InnerText = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                logger.Log($"err btnSearch_Click: {ex.Message}, {ex.StackTrace}");
            }
        }
    }
}