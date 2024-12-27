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
    public partial class GtwSettings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadData();
                LoadDropdown();
            }
        }

        private void LoadData()
        {
            DbOfficialCeisa.LoadParameter();
            string query = $@"select
                            a.gateway as [Gateway],
                            username as [ID Pengguna],
                            nama_ttd as [Nama],
                            jabatan as [Jabatan],
                            kota_ttd as [Tempat],
                            kode_kantor as [Kode Kantor Pabean],
                            kode_gudang as [Kode Gudang],
                            kode_aju as [Kode Nomor Pengajuan],
                            update_by as [Updated By],
                            convert(varchar,update_time,120) as [Updated Time]
                            from dhl_cs_settings a
                            left join dhl_cs_sequence_aju b
                            on a.gateway = b.gateway";
            DataTable tbl = DbOfficialCeisa.getRecords(query);
            ViewState["ListData"] = tbl;
            GV_GtwSettings.DataSource = tbl;
            GV_GtwSettings.DataBind();
        }
        private void LoadDropdown()
        {
            try
            {
                ddlLocation.Items.Clear();
                string query = @"select * from dhl_cs_gateway_location";
                DataTable dt = DbOfficialCeisa.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ddlLocation.Items.Add($"{dr["location_code"].ToString()} | {dr["location_name"].ToString()}");
                    }
                }
            }
            catch { }
        }
        protected void GV_GtwSettings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditRow" || e.CommandName == "DeleteRow")
                {
                    string key = e.CommandArgument.ToString();
                    switch (e.CommandName)
                    {
                        case "EditRow":
                            EditRecord(key);
                            break;
                        default: //ViewHistoryResponse
                            DeleteRecord(key);
                            break;
                    }
                }
            }
            catch { }
        }
        protected void GV_GtwSettings_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                //bind gridview
                GV_GtwSettings.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["ListData"];
                GV_GtwSettings.DataSource = dt;
                GV_GtwSettings.DataBind();
            }
            catch { }
        }

        #region Add, Edit, Delete
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ViewState["IsNewRecord"] = "true";
            ClearForm();
            ShowModal();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int status = 0;
                if (ViewState["IsNewRecord"].ToString() == "false")
                {
                    status = UpdateRecord();
                    if (status != 0)
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Update data failed');", true);
                    else
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Update data success');", true);
                }
                else
                {
                    status = AddRecord();
                    if (status != 0)
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Insert data failed');", true);
                    else
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Insert data success');", true);
                }

                CloseModal();
                LoadData();
            }
            catch { }
        }
        private void EditRecord(string gateway)
        {
            try
            {
                string query = $@"select
                                a.gateway as [Gateway],
                                username as [ID Pengguna],
                                nama_ttd as [Nama],
                                jabatan as [Jabatan],
                                kota_ttd as [Tempat],
                                kode_kantor as [Kode Kantor Pabean],
                                kode_gudang as [Kode Gudang],
                                kode_aju as [Kode Nomor Pengajuan],
                                update_by as [Updated By],
                                convert(varchar,update_time,120) as [Updated Time]
                                from dhl_cs_settings a
                                left join dhl_cs_sequence_aju b
                                on a.gateway = b.gateway
                                where a.gateway = '{gateway}'";
                DataRow dr = DbOfficialCeisa.getRow(query);

                ddlLocation.SelectedItem.Text = $"{dr["Gateway"]}";
                txtIDPengguna.Text = dr["ID Pengguna"].ToString();
                txtPassword.Text = string.Empty;
                txtNama.Text = dr["Nama"].ToString();
                txtJabatan.Text = dr["Jabatan"].ToString();
                txtKota.Text = dr["Tempat"].ToString();
                txtKodeKantor.Text = dr["Kode Kantor Pabean"].ToString();
                txtKodeGudang.Text = dr["Kode Gudang"].ToString();
                txtKodeAju.Text = dr["Kode Nomor Pengajuan"].ToString();

                //disable edit location gateway
                ddlLocation.Enabled = false;

                ViewState["IsNewRecord"] = "false";
                ShowModal();
            }
            catch { }
        }
        private int UpdateRecord()
        {
            int retval = 0;
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
                }

                string[] arr = ddlLocation.SelectedItem.Text.Split('|');
                string gateway = arr[0].Trim();
                List<string> listQuery = new List<string>();
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0) 
                    {
                        listQuery.Add($@"update dhl_cs_settings set
                                        username = '{txtIDPengguna.Text}',
                                        password = '{txtPassword.Text}',
                                        jabatan = '{txtJabatan.Text}',
                                        kota_ttd = '{txtKota.Text}',
                                        nama_ttd = '{txtNama.Text}',
                                        update_by = '{update_by}',
                                        update_time = '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff")}'
                                        where gateway = '{gateway}'");
                    }
                    else
                    {
                        listQuery.Add($@"update dhl_cs_sequence_aju set
                                        kode_kantor = '{txtKodeKantor.Text}',
                                        kode_gudang = '{txtKodeGudang.Text}',
                                        kode_aju = '{txtKodeAju.Text}'
                                        where gateway = '{gateway}'");
                    }
                }

                if (DbOfficialCeisa.runCommand(listQuery) != 0) retval = -1;
            }
            catch { }
            return retval;
        }
        private int AddRecord()
        {
            int retval = 0;
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
                }

                string[] arr = ddlLocation.SelectedItem.Text.Split('|');
                string gateway = arr[0].Trim();

                //validate duplicate gateway
                string query = $@"select count(1) as jml from dhl_cs_settings where gateway = '{gateway}'";
                string jml = DbOfficialCeisa.getFieldValue(query);
                if (Convert.ToInt32(jml) > 0)
                {
                    return retval = -1;
                }

                List<string> listQuery = new List<string>();
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                    {
                        listQuery.Add($@"insert into dhl_cs_settings values(
                                        '{gateway}',
                                        '{txtIDPengguna.Text}',
                                        '{txtPassword.Text}',
                                        '{txtJabatan.Text}',
                                        '{txtKota.Text}',
                                        '{txtNama.Text}',
                                        '{update_by}',
                                        '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff")}')");
                    }
                    else
                    {
                        listQuery.Add($@"update dhl_cs_sequence_aju set
                                        kode_kantor = '{txtKodeKantor.Text}',
                                        kode_gudang = '{txtKodeGudang.Text}',
                                        kode_aju = '{txtKodeAju.Text}'
                                        where gateway = '{gateway}'");
                    }
                }

                if (DbOfficialCeisa.runCommand(listQuery) != 0) retval = -1;
            }
            catch { }
            return retval;
        }
        private void DeleteRecord(string gateway)
        {
            try
            {
                string query = $"delete from dhl_cs_settings where gateway = '{gateway}'";
                DbOfficialCeisa.runCommand(query);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Delete data success');", true);
                LoadData();
            }
            catch { }
        }
        #endregion

        #region Modals
        private void ClearForm()
        {
            ddlLocation.SelectedItem.Text = string.Empty;
            ddlLocation.Enabled = true;
            LoadDropdown();

            txtIDPengguna.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtNama.Text = string.Empty;
            txtKota.Text = string.Empty;
            txtJabatan.Text = string.Empty;
            txtKodeKantor.Text = string.Empty;
            txtKodeGudang.Text = string.Empty;
            txtKodeAju.Text = string.Empty;
        }
        private void ShowModal()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "popup", "$('#btnPopupAdd').click();", true);
        }
        private void CloseModal()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "close", "$('#btnClose').click();", true);
        }
        #endregion
    }
}