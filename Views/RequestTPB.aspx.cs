using OfficialCeisaLite.App_Start;
using OfficialCeisaLite.Models;
using OfficialCeisaLite.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Octopus.Library.Utils;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Threading;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Globalization;

namespace OfficialCeisaLite.Views
{
    public partial class RequestTPB : System.Web.UI.Page
    {
        NbLogger logger = new NbLogger(AppDomain.CurrentDomain.BaseDirectory + @"/Log/", "TPB LOG JSON");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadSummary();
                InitDropdown();
            }
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(btnDownloadBarang);
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(btnDownloadDataKPBC);
        }
        private void LoadSummary()
        {
            DbOfficialDCI.LoadParameter();
            DbOfficialCeisa.LoadParameter();

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
                string query = $@"select top 100
                                a.id as [ID],
                                gateway as [Gateway],
                                a.kodeKantor as [KodeKantor],
                                (select distinct nomorDokumen from dhl_cs_tpb_dokumen d
                                where d.hawb = a.HAWB and d.kodeDokumen = 741) as MAWB,
                                (select distinct convert(char(10),tanggalDokumen,126) from dhl_cs_tpb_dokumen d
                                where d.hawb = a.HAWB and d.kodeDokumen = 741) as [Tgl MAWB],
                                a.HAWB,
                                convert(varchar(10),a.TGL_HAWB,20) as [Tgl HAWB],
                                (select distinct namaEntitas from dhl_cs_tpb_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 5) as [Nama Pengirim],
                                (select distinct kodeNegara from dhl_cs_tpb_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 5) as [Neg Pengirim],
                                'NPWP' as [Type Identitas],
                                (select distinct nomorIdentitas from dhl_cs_tpb_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 3) as [No Identitas],
                                (select distinct namaEntitas from dhl_cs_tpb_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 3) as [Nama Penerima],
                                nomorBc11 as [No BC 1.1],
                                convert(char(10),tanggalBc11,126) as [Tgl BC 1.1],
                                posBc11 as [No Pos],
                                subPosBc11 as [No Sub Pos],
                                nomorAju as [No Aju],
                                (select distinct f.nomorRespon from dhl_cs_response_data f
                                where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB
                                and keterangan = 'SPPB') as [SPPB],
                                (select distinct 
                                case
	                                when f.nomorRespon = '' then ''
	                                else convert(varchar,f.tanggalRespon,120)
                                end as tanggalRespon from dhl_cs_response_data f
                                where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB
                                and keterangan = 'SPPB') as [Tgl SPPB],
                                (select distinct f.nomorDaftar from dhl_cs_response_data f
                                where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB) as [Nopen],
                                (select distinct 
                                case
	                                when f.nomorDaftar = '' then ''
	                                else convert(varchar,f.tanggalDaftar,120)
                                end as tanggalDaftar from dhl_cs_response_data f
                                where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB) as [Tgl Nopen],
                                fob as [FOB],
                                freight as [Freight],
                                asuransi as [Asuransi],
                                kodeValuta as [Curr],
                                (select distinct top 1 convert(varchar,g.latest_response_time,120) as latest_response_time
                                from dhl_cs_response_header g where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) as [Latest Response Time],
                                (select distinct g.latest_response_code from dhl_cs_response_header g
                                where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) as [Latest Response Code],
                                (select distinct g.latest_response_note from dhl_cs_response_header g
                                where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) as [Latest Response Description],
                                update_by as [Update By],
                                convert(varchar,update_time,120) as [Update Time],
                                ISNULL((select distinct ISNULL(Freight,0) as freight from dhl_cs_tpb_profect g
                                where g.HAWB = a.HAWB),0) as [Profect Freight],
                                FORMAT(ISNULL((select distinct ISNULL(FreightRupiah,0) as freightRp from dhl_cs_tpb_profect g
                                where g.HAWB = a.HAWB),0),'N0') as [Profect Freight Rp],
                                FORMAT(ndpbm,'N2') as [Nilai Currency]
                                from dhl_cs_tpb_header a
                                where a.gateway = '{gateway}'
                                order by [ID] desc";
                DataTable dt = DbOfficialCeisa.getRecords(query);
                ViewState["ListDataDCI"] = dt;
                GV_DataDCI.DataSource = dt;
                GV_DataDCI.DataBind();
                totalData.InnerText = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                new ArgumentException(ex.Message);
            }
        }
        private void InitDropdown()
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

                string query = $@"select * from (
                                select 
                                KODEKANTOR, NAMAKANTOR,
                                case
	                                when LEFT(KODEKANTOR,2) = '01' THEN 'MES'
	                                when LEFT(KODEKANTOR,2) in ('03','04','05','15','16') THEN 'JKT'
	                                when LEFT(KODEKANTOR,2) = '06' THEN 'SRG'
	                                when LEFT(KODEKANTOR,2) = '07' THEN 'SUB'
                                end as GTW
                                from KODEKANTOR) as KODEKANTOR
                                where GTW = '{gateway}'";
                DataTable dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    srcKodeKantor.Items.Clear();
                    srcKodeKantor.Items.Add(new ListItem($"--Select Kode Kantor--", "0"));
                    foreach (DataRow row in dt.Rows)
                    {
                        srcKodeKantor.Items.Add(new ListItem($"{row["NAMAKANTOR"]}", $"{row["KODEKANTOR"]}"));
                    }
                    srcKodeKantor.SelectedIndex = 0;
                }

                query = $@"select distinct kodeProses, keterangan from dhl_cs_response_status order by kodeProses desc";
                dt = DbOfficialCeisa.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    srcShipmentStatus.Items.Clear();
                    srcShipmentStatus.Items.Add(new ListItem($"--Select Shipment Status--", "0"));
                    foreach (DataRow row in dt.Rows)
                    {
                        srcShipmentStatus.Items.Add(new ListItem($"{row["kodeProses"]} - {row["keterangan"]}", $"{row["kodeProses"]}"));
                    }
                    srcKodeKantor.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        private void InitDropdownSkep(DataTable table)
        {
            try
            {
                ddlNomorIjinEntitas.Items.Clear();
                ddlNomorIjinEntitas.Items.Add(new ListItem($"--Select Nomor SKEP--", "0"));
                foreach (DataRow row in table.Rows)
                {
                    ddlNomorIjinEntitas.Items.Add(new ListItem($"{row["NO_SKEP"]}", $"{row["NO_SKEP"]}"));
                }
                ddlNomorIjinEntitas.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        private void InitDropdownJasaKenaPajak()
        {
            try
            {
                ddlJasaKenaPajak.Items.Clear();

                string query = $"select * from dhl_cs_kenapajak";
                DataTable dt = DbOfficialCeisa.getRecords(query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ddlJasaKenaPajak.Items.Add(new ListItem($"{row["kode"]}-{row["description"]}", $"{row["kode"]}"));
                    }
                }

                ddlJasaKenaPajak.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                logger.Log($"{ex.Message}, {ex.StackTrace}");
            }
        }
        protected void GV_DataDCI_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "SendData" || e.CommandName == "GetResponse" || e.CommandName == "ViewHistoryResponse" || e.CommandName == "ViewRecord" || e.CommandName == "DeleteRecord" || e.CommandName == "CreateReport" || e.CommandName == "EditBarang")
                {
                    string key = string.Empty;
                    switch (e.CommandName)
                    {
                        case "SendData":
                            string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
                            string HAWB = commandArgs[0];
                            string TGL_HAWB = commandArgs[1];
                            string nomorAju = commandArgs[2];
                            if (string.IsNullOrEmpty(nomorAju) == false) return;
                            SendData(HAWB, TGL_HAWB);
                            break;
                        case "GetResponse":
                            string[] commandArgs_2 = e.CommandArgument.ToString().Split(new char[] { ',' });
                            string nomorAju_2 = commandArgs_2[0];
                            string HAWB_2 = commandArgs_2[1];
                            string TGL_HAWB_2 = commandArgs_2[2];
                            if (string.IsNullOrEmpty(nomorAju_2)) return;
                            GetResponseAju(nomorAju_2, HAWB_2, TGL_HAWB_2);
                            break;
                        case "ViewRecord":
                            string[] commandArgs_3 = e.CommandArgument.ToString().Split(new char[] { ',' });
                            string HAWB_3 = commandArgs_3[0];
                            string TGL_HAWB_3 = commandArgs_3[1];
                            if (string.IsNullOrEmpty(TGL_HAWB_3)) return;
                            ViewRecord(HAWB_3, TGL_HAWB_3);
                            break;
                        case "DeleteRecord":
                            string[] commandArgs_5 = e.CommandArgument.ToString().Split(new char[] { ',' });
                            string HAWB_5 = commandArgs_5[0];
                            string TGL_HAWB_5 = commandArgs_5[1];
                            if (string.IsNullOrEmpty(TGL_HAWB_5)) return;
                            DeleteRecord(HAWB_5, TGL_HAWB_5);
                            break;
                        case "CreateReport":
                            string[] commandArgs_6 = e.CommandArgument.ToString().Split(new char[] { ',' });
                            string HAWB_6 = commandArgs_6[0];
                            string TGL_HAWB_6 = commandArgs_6[1];
                            if (string.IsNullOrEmpty(TGL_HAWB_6)) return;
                            CreateReport(HAWB_6, TGL_HAWB_6);
                            break;
                        case "EditBarang":
                            string[] commandArgs_7 = e.CommandArgument.ToString().Split(new char[] { ',' });
                            string HAWB_7 = commandArgs_7[0];
                            string TGL_HAWB_7 = commandArgs_7[1];
                            if (string.IsNullOrEmpty(TGL_HAWB_7)) return;
                            EditBarang(HAWB_7, TGL_HAWB_7);
                            break;
                        default: //ViewHistoryResponse
                            string[] commandArgs_4 = e.CommandArgument.ToString().Split(new char[] { ',' });
                            string HAWB_4 = commandArgs_4[0];
                            string TGL_HAWB_4 = commandArgs_4[1];
                            string respCode = commandArgs_4[2];
                            if (string.IsNullOrEmpty(respCode))
                            {
                                ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "alert", $"alert('Response code not found! please click button download..');", true);
                                return;
                            }
                            ViewHistoryResponse(HAWB_4, TGL_HAWB_4, respCode);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        protected void GV_DataDCI_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_DataDCI.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)ViewState["ListDataDCI"];
            GV_DataDCI.DataSource = dt;
            GV_DataDCI.DataBind();
        }
        protected void GV_DataDCI_PageIndexChanged(object sender, EventArgs e)
        {
            if (ViewState["ListDataDCI"] != null)
            {
                DataTable dtUpdated = ViewState["ListDataDCI"] as DataTable;
                CheckBox chkAll = (GV_DataDCI.HeaderRow.FindControl("chkAll") as CheckBox);

                foreach (GridViewRow row in GV_DataDCI.Rows)
                {
                    CheckBox chkRowData = row.Cells[0].FindControl("chkRowData") as CheckBox;
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        if (ViewState["isChkAll"] != null)
                        {
                            if (ViewState["isChkAll"].ToString() == "false")
                            {
                                chkRowData.Checked = false;
                                chkAll.Checked = false;
                            }
                            else
                            {
                                chkRowData.Checked = true;
                                chkAll.Checked = true;
                            }
                        }
                        else
                        {
                            chkRowData.Checked = false;
                            chkAll.Checked = false;
                        }
                    }
                }
            }
        }
        protected void GV_DataDCI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check if the row is a data row (not header/footer)
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find the label control
                Label lblProfectFreight = e.Row.FindControl("lblProfectFreight") as Label;

                if (lblProfectFreight != null)
                {
                    // Get the data item
                    DataRowView rowView = e.Row.DataItem as DataRowView;

                    if (rowView != null)
                    {
                        // Get the Profect Freight value
                        string profectFreightValue = rowView["Profect Freight"].ToString();

                        // Try to parse as decimal or double
                        if (decimal.TryParse(profectFreightValue, out decimal profectFreight))
                        {
                            // Apply yellow background if value > 0
                            if (profectFreight > 0)
                            {
                                lblProfectFreight.BackColor = System.Drawing.Color.Yellow;
                                // Optional: Add some padding or styling
                                lblProfectFreight.Style.Add("font-weight", "bold");
                                lblProfectFreight.Style.Add("padding", "2px 5px");
                            }
                        }
                        else if (double.TryParse(profectFreightValue, out double profectFreightDouble))
                        {
                            // Apply yellow background if value > 0
                            if (profectFreightDouble > 0)
                            {
                                lblProfectFreight.BackColor = System.Drawing.Color.Yellow;
                                lblProfectFreight.Style.Add("font-weight", "bold");
                                lblProfectFreight.Style.Add("padding", "2px 5px");
                            }
                        }
                    }
                }
            }
        }
        protected void GV_HistoryRejectNotes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_HistoryRejectNotes.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)ViewState["ListHistoryRejectNotes"];
            GV_HistoryRejectNotes.DataSource = dt;
            GV_HistoryRejectNotes.DataBind();
        }
        protected void GV_DataResponse_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "PrintPDFRow")
                {
                    //get receipt no
                    string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
                    string file_type = commandArgs[0];
                    string awb = commandArgs[1];
                    string tgl_awb = commandArgs[2];

                    string query = $@"select * from dhl_cs_response_data where 
                                    HAWB = '{awb}' and TGL_HAWB = '{tgl_awb}' and keterangan = '{file_type}'";
                    DataTable dt = DbOfficialCeisa.getRecords(query);
                    Session["PrintPDF"] = dt;

                    //display data
                    string js = "window.open('PrintPDF.aspx', '_blank');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "Open PrintPDF.aspx", js, true);
                }
            }
            catch { }
        }
        private void EditBarang(string HAWB, string TGLHAWB)
        {
            try
            {
                Session["editHAWB"] = HAWB;
                Session["editTGLHAWB"] = TGLHAWB;

                //display data
                string js = "window.open('EditBarang.aspx', '_blank');";
                ScriptManager.RegisterStartupScript(this, GetType(), "Open EditBarang.aspx", js, true);

                //string tempUrl = ResolveUrl($"~/Views/EditBarang.aspx?HAWB={HAWB}&TGLHAWB={TGLHAWB}");

                //string script = "$(document).ready(function () { window.location.href = '" + tempUrl + "'; });";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "redirectScript", script, true);
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (sender as CheckBox);
            if (chk.ID == "chkAll")
            {
                foreach (GridViewRow row in GV_DataDCI.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked = chk.Checked;
                    }
                }
            }
            CheckBox chkAll = (GV_DataDCI.HeaderRow.FindControl("chkAll") as CheckBox);
            chkAll.Checked = true;
            ViewState["isChkAll"] = "true";
            foreach (GridViewRow row in GV_DataDCI.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    bool isChecked = row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked;
                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        if (!isChecked)
                        {
                            chkAll.Checked = false;
                            ViewState["isChkAll"] = "false";
                        }
                    }
                }
            }
        }
        protected void inputNomorIdentitas_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string npwp = inputNomorIdentitas.Text.Trim();
                string query = $@"select
                                isnull(skkb, '') as NO_SKEP,
                                isnull(convert(varchar(10),tgl_skkb,20),'2000-01-01') as TGL_SKEP, 
                                replace(replace(isnull(NPWP, ''),'.',''),'-','') as NPWP,
                                NAMACUSTOMER,
                                concat(ALAMAT1, ' ', isnull(ALAMAT2, '')) as ALAMAT,
                                NONIB as NIB
                                from CUSTOMER 
                                where '0'+replace(replace(isnull(NPWP, ''),'.',''),'-','')+'000000' = '{npwp}'
                                --and isnull(skkb, '') != ''
                                order by NAMACUSTOMER asc";
                DataTable dt = DbOfficialDCI.getRecords(query);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count > 1)
                    {
                        inputNomorIjinEntitas.Visible = false;
                        ddlNomorIjinEntitas.Visible = true;

                        InitDropdownSkep(dt);

                        return;
                    }

                    inputNomorIjinEntitas.Visible = true;
                    ddlNomorIjinEntitas.Visible = false;

                    foreach (DataRow row in dt.Rows)
                    {
                        inputNomorIjinEntitas.Text = row["NO_SKEP"].ToString();
                        inputTanggalIjinEntitas.Text = row["TGL_SKEP"].ToString();
                        inputNamaEntitas.Text = row["NAMACUSTOMER"].ToString();
                        inputAlamatEntitas.Text = row["ALAMAT"].ToString();
                        inputNibEntitas.Text = row["NIB"].ToString();
                    }
                }
                else
                {
                    inputNomorIjinEntitas.Visible = true;
                    ddlNomorIjinEntitas.Visible = false;

                    inputNomorIjinEntitas.Text = "";
                    inputNamaEntitas.Text = "";
                    inputAlamatEntitas.Text = "";
                    inputNibEntitas.Text = "";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('NPWP tidak ditemukan pada database DCI.');", true);
                }
            }
            catch (Exception ex)
            {
                logger.Log($"err Func inputNomorIdentitas_TextChanged : {ex.Message}");
            }
        }
        protected void ddlNomorIjinEntitas_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string NO_SKEP = ddlNomorIjinEntitas.SelectedItem.Value;
                string NPWP = inputNomorIdentitas.Text.Trim();

                string query = $@"select
                                isnull(skkb, '') as NO_SKEP,
                                isnull(convert(varchar(10),tgl_skkb,20),'2000-01-01') as TGL_SKEP, 
                                replace(replace(isnull(NPWP, ''),'.',''),'-','') as NPWP,
                                NAMACUSTOMER,
                                concat(ALAMAT1, ' ', isnull(ALAMAT2, '')) as ALAMAT,
                                NONIB as NIB
                                from CUSTOMER 
                                where '0'+replace(replace(isnull(NPWP, ''),'.',''),'-','')+'000000' = '{NPWP}'
                                and isnull(skkb, '') = '{NO_SKEP}' ";
                DataRow row = DbOfficialDCI.getRow(query);
                if (row != null)
                {
                    inputNamaEntitas.Text = row["NAMACUSTOMER"].ToString();
                    inputAlamatEntitas.Text = row["ALAMAT"].ToString();
                    inputNibEntitas.Text = row["NIB"].ToString();
                    inputTanggalIjinEntitas.Text = row["TGL_SKEP"].ToString();
                }
                else
                {
                    inputNamaEntitas.Text = "";
                    inputAlamatEntitas.Text = "";
                    inputNibEntitas.Text = "";
                    inputTanggalIjinEntitas.Text = "2000-01-01";
                }
            }
            catch (Exception ex)
            {
                logger.Log($"err Func ddlNomorIjinEntitas_SelectedIndexChanged : {ex.Message}");
            }
        }

        #region filter search
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
                string total_row = rblFilterRow.SelectedItem.Value;
                string query = $@"select top {total_row}
                                a.id as [ID],
                                gateway as [Gateway],
                                a.kodeKantor as [KodeKantor],
                                (select distinct nomorDokumen from dhl_cs_tpb_dokumen d
                                where d.hawb = a.HAWB and d.kodeDokumen = 741) as MAWB,
                                (select distinct convert(char(10),tanggalDokumen,126) from dhl_cs_tpb_dokumen d
                                where d.hawb = a.HAWB and d.kodeDokumen = 741) as [Tgl MAWB],
                                a.HAWB,
                                convert(varchar(10),a.TGL_HAWB,20) as [Tgl HAWB],
                                (select distinct namaEntitas from dhl_cs_tpb_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 5) as [Nama Pengirim],
                                (select distinct kodeNegara from dhl_cs_tpb_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 5) as [Neg Pengirim],
                                'NPWP' as [Type Identitas],
                                (select distinct nomorIdentitas from dhl_cs_tpb_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 3) as [No Identitas],
                                (select distinct namaEntitas from dhl_cs_tpb_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 3) as [Nama Penerima],
                                nomorBc11 as [No BC 1.1],
                                convert(char(10),tanggalBc11,126) as [Tgl BC 1.1],
                                posBc11 as [No Pos],
                                subPosBc11 as [No Sub Pos],
                                nomorAju as [No Aju],
                                (select distinct f.nomorRespon from dhl_cs_response_data f
                                where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB
                                and keterangan = 'SPPB') as [SPPB],
                                (select distinct 
                                case
	                                when f.nomorRespon = '' then ''
	                                else convert(varchar,f.tanggalRespon,120)
                                end as tanggalRespon from dhl_cs_response_data f
                                where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB
                                and keterangan = 'SPPB') as [Tgl SPPB],
                                (select distinct f.nomorDaftar from dhl_cs_response_data f
                                where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB) as [Nopen],
                                (select distinct 
                                case
	                                when f.nomorDaftar = '' then ''
	                                else convert(varchar,f.tanggalDaftar,120)
                                end as tanggalDaftar from dhl_cs_response_data f
                                where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB) as [Tgl Nopen],
                                fob as [FOB],
                                freight as [Freight],
                                asuransi as [Asuransi],
                                kodeValuta as [Curr],
                                (select distinct top 1 convert(varchar,g.latest_response_time,120) as latest_response_time
                                from dhl_cs_response_header g where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) as [Latest Response Time],
                                (select distinct g.latest_response_code from dhl_cs_response_header g
                                where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) as [Latest Response Code],
                                (select distinct g.latest_response_note from dhl_cs_response_header g
                                where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) as [Latest Response Description],
                                update_by as [Update By],
                                convert(varchar,update_time,120) as [Update Time],
                                ISNULL((select distinct ISNULL(Freight,0) as freight from dhl_cs_tpb_profect g
                                where g.HAWB = a.HAWB),0) as [Profect Freight],
                                FORMAT(ISNULL((select distinct ISNULL(FreightRupiah,0) as freightRp from dhl_cs_tpb_profect g
                                where g.HAWB = a.HAWB),0),'N0') as [Profect Freight Rp],
                                FORMAT(ndpbm,'N2') as [Nilai Currency]
                                from dhl_cs_tpb_header a
                                where gateway = '{gateway}'";
                string hawb = srcHAWB.Text.Trim();
                string tgl_hawb = srcTglHawb.Text.Trim();
                string no_aju = srcNoAju.Text.Trim();
                string status = srcStatus.Text;
                string kode_kantor = srcKodeKantor.SelectedItem.Value;
                string shipment_status = srcShipmentStatus.SelectedItem.Value;
                string parameter = string.Empty;

                if (string.IsNullOrEmpty(hawb) == false)
                {
                    List<string> listHAWB = new List<string>();
                    string[] arr = hawb.Split('\n');
                    foreach (var item in arr)
                    {
                        listHAWB.Add($@"'{item.Trim()}'");
                    }
                    hawb = String.Join(",", listHAWB);
                    parameter = $@" and a.HAWB in ({hawb}) order by [ID] desc";
                    query += parameter;
                }
                else if (kode_kantor != "0")
                {
                    parameter = $@" and kodeKantor = '{kode_kantor.Trim()}' order by [ID] desc";
                    query += parameter;
                }
                else if (shipment_status != "0")
                {
                    parameter = $@" and (select distinct g.latest_response_code from dhl_cs_response_header g where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) = '{shipment_status.Trim()}' order by [ID] desc";
                    query += parameter;
                }
                else if (string.IsNullOrEmpty(tgl_hawb) == false)
                {
                    parameter = $@" and a.TGL_HAWB = '{tgl_hawb}' order by [ID] desc";
                    query += parameter;
                }
                else if (string.IsNullOrEmpty(no_aju) == false)
                {
                    parameter = $@" and nomorAju like '%{no_aju}%' order by [ID] desc";
                    query += parameter;
                }
                else if (isBlankAju.Checked == true)
                {
                    parameter = $@" and nomorAju = '' order by [ID] desc";
                    query += parameter;
                }
                else if (string.IsNullOrEmpty(status) == false)
                {
                    parameter = $@" and (select distinct g.latest_response_note from dhl_cs_response_header g
                                    where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) like '%{status}%' order by [ID] desc";
                    query += parameter;
                }
                else if (isBlankStatus.Checked == true)
                {
                    parameter = $@" and (select distinct g.latest_response_note from dhl_cs_response_header g
                                    where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) is null order by [ID] desc";
                    query += parameter;
                }
                else
                {
                    parameter = $@" order by [ID] desc";
                    query += parameter;
                }

                DataTable dt = DbOfficialCeisa.getRecords(query);
                ViewState["ListDataDCI"] = dt;
                GV_DataDCI.DataSource = dt;
                GV_DataDCI.DataBind();
                totalData.InnerText = dt.Rows.Count.ToString();
            }
            catch { }
        }
        #endregion

        #region get history response
        protected void ViewHistoryResponse(string HAWB, string TGL_HAWB, string respCode)
        {
            try
            {
                string query = $@"select 
                                kodeProses as [Kode Response],
                                keterangan as [Keterangan Response],
                                convert(varchar, waktuStatus, 120) as [Waktu Response]
                                from dhl_cs_response_status 
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                order by waktuStatus desc";
                DataTable dt = DbOfficialCeisa.getRecords(query);

                if (respCode == "120")
                {
                    GV_HistoryReject.DataSource = dt;
                    GV_HistoryReject.DataBind();

                    query = $@"select  
                            NO_AJU as [No Aju], 
                            DETAIL as [Reject Notes],
                            convert(varchar,WAKTU_RESPONSE,120) as [Response Time]
                            from dhl_cs_response_pesan
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                            group by NO_AJU, DETAIL, WAKTU_RESPONSE
                            order by WAKTU_RESPONSE desc";
                    DataTable tbl = DbOfficialCeisa.getRecords(query);

                    GV_HistoryRejectNotes.DataSource = tbl;
                    GV_HistoryRejectNotes.DataBind();
                    ViewState["ListHistoryRejectNotes"] = tbl;
                    ShowModalHistoryReject();
                }
                else
                {
                    GV_HistoryResponse.DataSource = dt;
                    GV_HistoryResponse.DataBind();

                    query = $@"select 
                            case 
	                            when len(url_pdf) > 0 then 'Printout PDF'
	                            else '-'
                            end as [Action],
                            keterangan as [File Type],
                            HAWB as [AWB],
                            convert(varchar(10),TGL_HAWB,120) as [TGL AWB],
                            convert(varchar,waktuRespon,120) as [Response Time]
                            from dhl_cs_response_data
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                            order by waktuRespon desc";
                    DataTable tbl = DbOfficialCeisa.getRecords(query);

                    GV_DataResponse.DataSource = tbl;
                    GV_DataResponse.DataBind();

                    ShowModalHistory();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        #endregion

        #region get record barang
        private void ViewRecord(string HAWB, string TGL_HAWB)
        {
            try
            {
                string query = $@"select
                                HAWB, convert(char(10),TGL_HAWB,126) as TGL_HAWB,
                                asuransi as [ASURANSI], 
                                cif as [CIF], 
                                cifRupiah as [CIF RUPIAH],
                                fob as [FOB],
                                freight as [FREIGHT],
                                hargaSatuan as [HARGA SATUAN],
                                jumlahSatuan as [JUMLAH SATUAN],
                                kodeNegaraAsal as [KODE NEGARA ASAL],
                                kodeSatuanBarang as [KODE SATUAN BARANG],
                                jumlahKemasan as [JUMLAH KEMASAN],
                                kodeJenisKemasan as [KODE KEMASAN],
                                case
                                    when kodeBarang = '' then '-'
                                    else kodeBarang
                                end as [KODE BARANG],
                                ndpbm as [NDPBM],
                                (select top 1 kodeValuta from dhl_cs_tpb_header
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}') as [KODE VALUTA],
                                netto as [NETTO],
                                kodeKategoriBarang as [KATEGORI BARANG],
                                posTarif as [POS TARIF],
                                seriBarang as [SERI BARANG],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'BM' and seriBarang = barang.seriBarang) as [BM TARIF],
                                ISNULL((select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'BMTP' and seriBarang = barang.seriBarang),'0.00') as [BMTP TARIF],
                                ISNULL((select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'CUKAI' and seriBarang = barang.seriBarang),'0.00') as [CUKAI TARIF],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'PPN' and seriBarang = barang.seriBarang) as [PPN TARIF],
                                ISNULL((select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'PPNBM' and seriBarang = barang.seriBarang),'0.00') as [PPNBM TARIF],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'PPH' and seriBarang = barang.seriBarang) as [PPH TARIF],
                                uraian as [URAIAN],
                                merk as [MEREK],
                                tipe as [TIPE],
                                case 
                                    when ukuran = '' then '-' 
                                    else ukuran
                                end as [UKURAN],
                                case 
                                    when spesifikasiLain = '' then '-'
                                    else spesifikasiLain
                                end as [SPESIFIKASI LAINNYA]
                                from dhl_cs_tpb_barang barang
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                order by seriBarang asc";
                DataTable tblBarang = DbOfficialCeisa.getRecords(query);
                ViewState["ListBarang"] = tblBarang;
                GV_Barang.DataSource = tblBarang;
                GV_Barang.DataBind();

                query = $@"select
                        HAWB, convert(char(10),TGL_HAWB,126) as TGL_HAWB,
                        kodeDokumen, nomorDokumen, 
                        convert(char(10),tanggalDokumen,126) as tanggalDokumen
                        from dhl_cs_tpb_dokumen
                        where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                        order by seriDokumen asc";
                DataTable tblDokumen = DbOfficialCeisa.getRecords(query);
                ViewState["ListDokumen"] = tblDokumen;
                GV_Dokumen.DataSource = tblDokumen;
                GV_Dokumen.DataBind();

                InitDropdownJasaKenaPajak();

                query = $@"select top 1 nomorIdentitas, namaEntitas, alamatEntitas, nomorIjinEntitas, 
                        convert(varchar(10),tanggalIjinEntitas,20) as tanggalIjinEntitas, nibEntitas,
                        T3.kode
                        from dhl_cs_tpb_entitas T1 inner join dhl_cs_tpb_header T2
                        on T1.HAWB = T2.HAWB and T1.TGL_HAWB = T2.TGL_HAWB inner join dhl_cs_kenapajak T3
                        on T2.kodeKenaPajak = T3.kode
                        where T1.HAWB = '{HAWB}' and T1.TGL_HAWB = '{TGL_HAWB}' and kodeEntitas = '3'";
                DataRow rowEntitas = DbOfficialCeisa.getRow(query);
                if (rowEntitas != null)
                {
                    inputNomorIdentitas.Text = rowEntitas["nomorIdentitas"].ToString();
                    inputNamaEntitas.Text = rowEntitas["namaEntitas"].ToString();
                    inputAlamatEntitas.Text = rowEntitas["alamatEntitas"].ToString();
                    inputNomorIjinEntitas.Text = rowEntitas["nomorIjinEntitas"].ToString();
                    inputTanggalIjinEntitas.Text = rowEntitas["tanggalIjinEntitas"].ToString();
                    inputNibEntitas.Text = rowEntitas["nibEntitas"].ToString();

                    switch (rowEntitas["kode"].ToString())
                    {
                        case "2":
                            ddlJasaKenaPajak.SelectedIndex = 1;
                            break;
                        default://1
                            ddlJasaKenaPajak.SelectedIndex = 0;
                            break;
                    }
                }
                else
                {
                    inputNomorIdentitas.Text = string.Empty;
                    inputNamaEntitas.Text = string.Empty;
                    inputAlamatEntitas.Text = string.Empty;
                    inputNomorIjinEntitas.Text = string.Empty;
                    inputTanggalIjinEntitas.Text = string.Empty;
                    inputNibEntitas.Text = string.Empty;
                }

                ViewState["ViewRecordHawb"] = HAWB;
                ViewState["ViewRecordTglHawb"] = TGL_HAWB;
                ShowModalBarang();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        protected void GV_Dokumen_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_Dokumen.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)ViewState["ListDokumen"];
            GV_Dokumen.DataSource = dt;
            GV_Dokumen.DataBind();
        }
        protected void GV_Barang_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_Barang.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)ViewState["ListBarang"];
            GV_Barang.DataSource = dt;
            GV_Barang.DataBind();
        }
        #endregion

        #region delete record barang
        private void DeleteRecord(string HAWB, string TGL_HAWB)
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

                string queryHeader = $@"delete from dhl_cs_tpb_header where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";
                string queryBrg = $@"delete from dhl_cs_tpb_barang where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";
                string queryBrgTarif = $@"delete from dhl_cs_tpb_barang_tarif where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";
                string queryKemasan = $@"delete from dhl_cs_tpb_kemasan where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";
                string queryEntitas = $@"delete from dhl_cs_tpb_entitas where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";
                string queryDokumen = $@"delete from dhl_cs_tpb_dokumen where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";
                string queryPengangkut = $@"delete from dhl_cs_tpb_pengangkut where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";

                string queryResponData = $@"delete from dhl_cs_response_data where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";
                string queryResponHeader = $@"delete from dhl_cs_response_header where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";
                string queryResponStatus = $@"delete from dhl_cs_response_status where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";
                string queryResponPesan = $@"delete from dhl_cs_response_pesan where hawb = '{HAWB}' and tgl_hawb = '{TGL_HAWB}' ";

                List<string> listDelete = new List<string>();
                listDelete.Add($"{queryHeader}");
                listDelete.Add($"{queryBrg}");
                listDelete.Add($"{queryBrgTarif}");
                listDelete.Add($"{queryKemasan}");
                listDelete.Add($"{queryEntitas}");
                listDelete.Add($"{queryDokumen}");
                listDelete.Add($"{queryPengangkut}");
                listDelete.Add($"{queryResponData}");
                listDelete.Add($"{queryResponHeader}");
                listDelete.Add($"{queryResponStatus}");
                listDelete.Add($"{queryResponPesan}");

                if (DbOfficialCeisa.runCommand(listDelete) == 0)
                {
                    logger.Log($"delete record data.. AWB: {HAWB}.. by {update_by}");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('delete record data.. AWB: {HAWB} success.');", true);
                }

                LoadSummary();
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        #endregion

        #region get response AJU
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
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

                List<string> listAju = new List<string>();
                if (ViewState["isChkAll"] != null)
                {
                    string isChkAll = ViewState["isChkAll"].ToString();

                    if (isChkAll == "false")
                    {
                        foreach (GridViewRow row in GV_DataDCI.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                bool isChecked = row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked;
                                if (!isChecked) continue;

                                string NO_AJU = GV_DataDCI.DataKeys[row.RowIndex].Values[2].ToString();
                                if (string.IsNullOrEmpty(NO_AJU)) continue;

                                string HAWB = GV_DataDCI.DataKeys[row.RowIndex].Values[0].ToString();
                                string TGL_HAWB = GV_DataDCI.DataKeys[row.RowIndex].Values[1].ToString();

                                listAju.Add($"{NO_AJU}|{HAWB}|{TGL_HAWB}");
                            }
                        }
                    }
                    else
                    {
                        foreach (GridViewRow row in GV_DataDCI.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                bool isChecked = row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked;
                                if (!isChecked) continue;

                                string NO_AJU = GV_DataDCI.DataKeys[row.RowIndex].Values[2].ToString();
                                if (string.IsNullOrEmpty(NO_AJU)) continue;

                                string HAWB = GV_DataDCI.DataKeys[row.RowIndex].Values[0].ToString();
                                string TGL_HAWB = GV_DataDCI.DataKeys[row.RowIndex].Values[1].ToString();

                                listAju.Add($"{NO_AJU}|{HAWB}|{TGL_HAWB}");
                            }
                        }

                        //DataTable dt = (DataTable)ViewState["ListDataDCI"];
                        //if (dt.Rows.Count > 0)
                        //{
                        //    foreach (DataRow row in dt.Rows)
                        //    {
                        //        string NO_AJU = row["No Aju"].ToString();
                        //        if (string.IsNullOrEmpty(NO_AJU)) continue;

                        //        string HAWB = row["HAWB"].ToString();
                        //        string TGL_HAWB = row["Tgl HAWB"].ToString();

                        //        listAju.Add($"{NO_AJU}|{HAWB}|{TGL_HAWB}");
                        //    }
                        //}
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('No rows selected.');", true);
                    CloseModalLoader();
                    return;
                }


                if (listAju.Count > 0)
                {
                    //get user password for request token
                    //string query = $@"select username, password from dhl_cs_settings where gateway = '{gateway}'";
                    //DataRow row = DbOfficialCeisa.getRow(query);
                    //if (row == null)
                    //{
                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Please fill username & password on gateway settings.');", true);
                    //    return;
                    //}

                    //string userCeisa = row["username"].ToString();
                    //string passwordCeisa = row["password"].ToString();

                    //ResponseToken response = JsonConvert.DeserializeObject<ResponseToken>(GetRequestToken(userCeisa, passwordCeisa));
                    //if (response.status != "success")
                    //{
                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{response.message}');", true);
                    //    return;
                    //}

                    foreach (var ajuItems in listAju)
                    {
                        string json = string.Empty;
                        string[] item_array = ajuItems.Split('|');
                        string no_aju = item_array[0];
                        string HAWB = item_array[1];
                        string TGL_HAWB = item_array[2];

                        string Url = $@"https://apis-gw.beacukai.go.id/openapi/status/{no_aju}";
                        //string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/status/{no_aju}";
                        Uri myUri = new Uri(Url, UriKind.Absolute);

                        WebRequest requestObjGet = WebRequest.Create(myUri);
                        requestObjGet.Method = "GET";
                        requestObjGet.Headers.Add("Authorization", $"Bearer {GetAccessToken()}");

#if DEBUG
#else
                        WebProxy myproxy = new WebProxy("b2b-http.dhl.com", 8080);
                        myproxy.BypassProxyOnLocal = false;
                        requestObjGet.Proxy = myproxy;
#endif

                        try
                        {
                            var responseAju = (HttpWebResponse)requestObjGet.GetResponse();

                            using (var streamReader = new StreamReader(responseAju.GetResponseStream()))
                            {
                                json = streamReader.ReadToEnd();
                                logger.Log($"response aju {no_aju}" + Environment.NewLine + json);

                                ResponseStatusAju responseStatusAju = JsonConvert.DeserializeObject<ResponseStatusAju>(json);
                                if (responseStatusAju.status == "Success")
                                {
                                    #region dataStatus
                                    List<string> listQueryStatus = new List<string>();
                                    foreach (var item in responseStatusAju.dataStatus)
                                    {
                                        listQueryStatus.Add($@"insert into dhl_cs_response_status values (
                                                    '{HAWB}','{TGL_HAWB}','{no_aju}',
                                                    '{item.nomorDaftar}','{item.tanggalDaftar}',
                                                    '{item.kodeProses}','{item.waktuStatus}',
                                                    '{item.keterangan}')");
                                    }

                                    string queryDelete = $"delete from dhl_cs_response_status where NO_AJU = '{no_aju}'";
                                    DbOfficialCeisa.runCommand(queryDelete);

                                    if (listQueryStatus.Count > 0)
                                    {
                                        if (DbOfficialCeisa.runCommand(listQueryStatus) == 0)
                                            logger.Log($"insert data status success");
                                    }
                                    #endregion

                                    #region dataRespon
                                    List<string> listQueryRespon = new List<string>();
                                    bool isRejected = false;
                                    bool isRelease4 = false;
                                    foreach (var item in responseStatusAju.dataRespon)
                                    {
                                        if (item.keterangan == "REJECT")
                                        {
                                            listQueryRespon.Add($@"insert into dhl_cs_response_data values (
                                                        '{HAWB}','{TGL_HAWB}','{no_aju}',
                                                        '{item.kodeRespon}','{item.nomorDaftar}',
                                                        '{item.tanggalDaftar}','{item.nomorRespon}',
                                                        '{item.tanggalRespon}','{item.waktuRespon}',
                                                        '{item.keterangan}','','')");

                                            foreach (var items in item.pesan)
                                            {
                                                listQueryRespon.Add($@"insert into dhl_cs_response_pesan values (
                                                            '{HAWB}','{TGL_HAWB}','{no_aju}',
                                                            '{items}','{item.waktuRespon}')");
                                            }

                                            isRejected = true;
                                        }
                                        else
                                        {
                                            if (item.pdf == null)
                                            {
                                                listQueryRespon.Add($@"insert into dhl_cs_response_data values (
                                                            '{HAWB}','{TGL_HAWB}','{no_aju}',
                                                            '{item.kodeRespon}','{item.nomorDaftar}',
                                                            '{item.tanggalDaftar}','{item.nomorRespon}',
                                                            '{item.tanggalRespon}','{item.waktuRespon}',
                                                            '{item.keterangan}','','')");

                                                //FLAGGING RLS4
                                                if (item.kodeRespon == "2303")
                                                {
                                                    isRelease4 = true;
                                                    string time_requested = DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                    listQueryRespon.Add($@"insert into dhl_cs_tpb_status_xml (
                                                                        HAWB, SPPBSTATUS, ISCREATEXML, NO_AJU, time_requested
                                                                        ) values (
                                                                        '{HAWB}','{item.kodeRespon}','0','{no_aju}','{time_requested}'
                                                                        )");
                                                }
                                            }
                                            else
                                            {
                                                string base64pdf = string.Empty;
                                                try
                                                {
                                                    base64pdf = GetBase64StringPdf(item.pdf, GetAccessToken(), item.keterangan, no_aju);
                                                }
                                                catch (Exception ex)
                                                {
                                                    logger.Log(ex.Message);
                                                }

                                                listQueryRespon.Add($@"insert into dhl_cs_response_data values (
                                                            '{HAWB}','{TGL_HAWB}','{no_aju}',
                                                            '{item.kodeRespon}','{item.nomorDaftar}',
                                                            '{item.tanggalDaftar}','{item.nomorRespon}',
                                                            '{item.tanggalRespon}','{item.waktuRespon}',
                                                            '{item.keterangan}','{item.pdf}',
                                                            '{base64pdf}')");

                                                //FLAGGING RLS4
                                                if (item.kodeRespon == "2303")
                                                {
                                                    isRelease4 = true;
                                                    string time_requested = DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                    listQueryRespon.Add($@"insert into dhl_cs_tpb_status_xml (
                                                                        HAWB, SPPBSTATUS, ISCREATEXML, NO_AJU, time_requested
                                                                        ) values (
                                                                        '{HAWB}','{item.kodeRespon}','0','{no_aju}','{time_requested}'
                                                                        )");
                                                }
                                            }
                                        }
                                    }

                                    if (listQueryRespon.Count > 0)
                                    {
                                        queryDelete = $"delete from dhl_cs_response_data where NO_AJU = '{no_aju}'";
                                        DbOfficialCeisa.runCommand(queryDelete);

                                        if (isRejected)
                                        {
                                            queryDelete = $"delete from dhl_cs_response_pesan where NO_AJU = '{no_aju}'";
                                            DbOfficialCeisa.runCommand(queryDelete);
                                        }

                                        if (isRelease4)
                                        {
                                            queryDelete = $"delete from dhl_cs_tpb_status_xml where NO_AJU = '{no_aju}'";
                                            DbOfficialCeisa.runCommand(queryDelete);
                                        }

                                        if (DbOfficialCeisa.runCommand(listQueryRespon) == 0)
                                            logger.Log($"insert data response success");
                                    }
                                    #endregion

                                    #region dataHeader
                                    string query = $@"select top 1 * from dhl_cs_response_header where NO_AJU = '{no_aju}'";
                                    DataRow row = DbOfficialCeisa.getRow(query);

                                    if (row != null)
                                    {
                                        //get latest response
                                        query = $@"select top 1 kodeProses, 
                                        convert(varchar,waktuStatus,120) as waktuStatus, 
                                        keterangan from dhl_cs_response_status
                                        where NO_AJU = '{no_aju}' order by waktuStatus desc";
                                        DataRow dr = DbOfficialCeisa.getRow(query);

                                        //update latest response
                                        query = $@"update dhl_cs_response_header set
                                        latest_response_code = '{dr["kodeProses"]}',
                                        latest_response_time = '{dr["waktuStatus"]}',
                                        latest_response_note = '{dr["keterangan"]}'
                                        where NO_AJU = '{no_aju}'";
                                        if (DbOfficialCeisa.runCommand(query) == 0)
                                            logger.Log($"update latest response success");
                                    }
                                    else
                                    {
                                        //get latest response
                                        query = $@"select top 1 kodeProses, 
                                        convert(varchar,waktuStatus,120) as waktuStatus, 
                                        keterangan from dhl_cs_response_status
                                        where NO_AJU = '{no_aju}' order by waktuStatus desc";
                                        DataRow dr = DbOfficialCeisa.getRow(query);

                                        //insert latest response
                                        query = $@"insert into dhl_cs_response_header values(
                                        '{HAWB}','{TGL_HAWB}','{no_aju}',
                                        '{dr["waktuStatus"]}',
                                        '{dr["kodeProses"]}',
                                        '{dr["keterangan"]}')";
                                        if (DbOfficialCeisa.runCommand(query) == 0)
                                            logger.Log($"insert latest response success");
                                    }
                                    #endregion
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WebException we = ex as WebException;
                            if (we != null && we.Response is HttpWebResponse)
                            {
                                HttpWebResponse responseAju = (HttpWebResponse)we.Response;

                                // it can be 404, 500 etc...
                                //Console.WriteLine(response.StatusCode);
                                using (var streamReader = new StreamReader(responseAju.GetResponseStream()))
                                {
                                    json = streamReader.ReadToEnd();
                                    logger.Log($"response aju {no_aju}" + Environment.NewLine + json);
                                }
                            }
                            else
                            {
                                logger.Log($"response aju {no_aju}" + Environment.NewLine + we.Message);
                            }
                        }
                    }


                    logger.Log($"Download response success, {listAju.Count} rows updated");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Download response success, {listAju.Count} rows updated');", true);
                    CloseModalLoader();
                    LoadSummary();
                    InitDropdown();
                }
                else
                {
                    CloseModalLoader();
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        protected void btnGetResp_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton linkButton = sender as LinkButton;

                string[] commandArgs_2 = linkButton.CommandArgument.ToString().Split(new char[] { ',' });
                string nomorAju = commandArgs_2[0];
                string HAWB_2 = commandArgs_2[1];
                string TGL_HAWB_2 = commandArgs_2[2];
                if (string.IsNullOrEmpty(nomorAju)) return;
                GetResponseAju(nomorAju, HAWB_2, TGL_HAWB_2);
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        private void GetResponseAju(string nomorAju, string HAWB, string TGL_HAWB)
        {
            try
            {
                //validate session & get username gateway location
                string username = string.Empty;
                string gateway = string.Empty;
                LoginData user = SessionUtils.GetUserData(this.Page);
                if (user != null) { username = user.username; gateway = user.location; }
                else Response.Redirect("~/Auth/Login.aspx");

                //get user password for request token
                //string query = $@"select username, password from dhl_cs_settings where gateway = '{gateway}'";
                //DataRow row = DbOfficialCeisa.getRow(query);
                //if (row == null)
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Gateway not found, please fill on gateway settings.');", true);
                //    return;
                //}

                //string userCeisa = row["username"].ToString();
                //string passwordCeisa = row["password"].ToString();

                //ResponseToken response = JsonConvert.DeserializeObject<ResponseToken>(GetRequestToken(userCeisa, passwordCeisa));
                //if (response.status != "success")
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{response.message}');", true);
                //    return;
                //}

                string Url = $@"https://apis-gw.beacukai.go.id/openapi/status/{nomorAju}";
                //string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/status/{nomorAju}";
                Uri myUri = new Uri(Url, UriKind.Absolute);

                WebRequest requestObjGet = WebRequest.Create(myUri);
                requestObjGet.Method = "GET";
                requestObjGet.Headers.Add("Authorization", $"Bearer {GetAccessToken()}");

#if DEBUG
#else
                WebProxy myproxy = new WebProxy("b2b-http.dhl.com", 8080);
                myproxy.BypassProxyOnLocal = false;
                requestObjGet.Proxy = myproxy;
#endif

                string json = string.Empty;
                try
                {
                    var responseAju = (HttpWebResponse)requestObjGet.GetResponse();

                    using (var streamReader = new StreamReader(responseAju.GetResponseStream()))
                    {
                        json = streamReader.ReadToEnd();
                        logger.Log($"response aju {nomorAju}" + Environment.NewLine + json);

                        ResponseStatusAju responseStatusAju = JsonConvert.DeserializeObject<ResponseStatusAju>(json);
                        if (responseStatusAju.status == "Success")
                        {
                            #region dataStatus
                            List<string> listQueryStatus = new List<string>();
                            foreach (var item in responseStatusAju.dataStatus)
                            {
                                listQueryStatus.Add($@"insert into dhl_cs_response_status values (
                                                    '{HAWB}','{TGL_HAWB}','{nomorAju}',
                                                    '{item.nomorDaftar}','{item.tanggalDaftar}',
                                                    '{item.kodeProses}','{item.waktuStatus}',
                                                    '{item.keterangan}')");
                            }

                            string queryDelete = $"delete from dhl_cs_response_status where NO_AJU = '{nomorAju}'";
                            DbOfficialCeisa.runCommand(queryDelete);

                            if (listQueryStatus.Count > 0)
                            {
                                if (DbOfficialCeisa.runCommand(listQueryStatus) == 0)
                                    logger.Log($"insert data status success");
                            }
                            #endregion

                            #region dataRespon
                            List<string> listQueryRespon = new List<string>();
                            bool isRejected = false;
                            bool isRelease4 = false;
                            foreach (var item in responseStatusAju.dataRespon)
                            {
                                if (item.keterangan == "REJECT")
                                {
                                    listQueryRespon.Add($@"insert into dhl_cs_response_data values (
                                                        '{HAWB}','{TGL_HAWB}','{nomorAju}',
                                                        '{item.kodeRespon}','{item.nomorDaftar}',
                                                        '{item.tanggalDaftar}','{item.nomorRespon}',
                                                        '{item.tanggalRespon}','{item.waktuRespon}',
                                                        '{item.keterangan}','','')");

                                    foreach (var items in item.pesan)
                                    {
                                        listQueryRespon.Add($@"insert into dhl_cs_response_pesan values (
                                                            '{HAWB}','{TGL_HAWB}','{nomorAju}',
                                                            '{items}','{item.waktuRespon}')");
                                    }

                                    isRejected = true;
                                }
                                else
                                {
                                    if (item.pdf == null)
                                    {
                                        listQueryRespon.Add($@"insert into dhl_cs_response_data values (
                                                            '{HAWB}','{TGL_HAWB}','{nomorAju}',
                                                            '{item.kodeRespon}','{item.nomorDaftar}',
                                                            '{item.tanggalDaftar}','{item.nomorRespon}',
                                                            '{item.tanggalRespon}','{item.waktuRespon}',
                                                            '{item.keterangan}','','')");

                                        if (item.kodeRespon == "2303")
                                        {
                                            isRelease4 = true;
                                            string time_requested = DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            listQueryRespon.Add($@"insert into dhl_cs_tpb_status_xml (
                                                                        HAWB, SPPBSTATUS, ISCREATEXML, NO_AJU, time_requested
                                                                        ) values (
                                                                        '{HAWB}','{item.kodeRespon}','0','{nomorAju}','{time_requested}'
                                                                        )");
                                        }
                                    }
                                    else
                                    {
                                        string base64pdf = string.Empty;
                                        try
                                        {
                                            base64pdf = GetBase64StringPdf(item.pdf, GetAccessToken(), item.keterangan, nomorAju);
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Log(ex.Message);
                                        }

                                        listQueryRespon.Add($@"insert into dhl_cs_response_data values (
                                                            '{HAWB}','{TGL_HAWB}','{nomorAju}',
                                                            '{item.kodeRespon}','{item.nomorDaftar}',
                                                            '{item.tanggalDaftar}','{item.nomorRespon}',
                                                            '{item.tanggalRespon}','{item.waktuRespon}',
                                                            '{item.keterangan}','{item.pdf}',
                                                            '{base64pdf}')");

                                        //FLAGGING RLS4
                                        if (item.kodeRespon == "2303")
                                        {
                                            isRelease4 = true;
                                            string time_requested = DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            listQueryRespon.Add($@"insert into dhl_cs_tpb_status_xml (
                                                                        HAWB, SPPBSTATUS, ISCREATEXML, NO_AJU, time_requested
                                                                        ) values (
                                                                        '{HAWB}','{item.kodeRespon}','0','{nomorAju}','{time_requested}'
                                                                        )");
                                        }
                                    }
                                }
                            }

                            if (listQueryRespon.Count > 0)
                            {
                                queryDelete = $"delete from dhl_cs_response_data where NO_AJU = '{nomorAju}'";
                                DbOfficialCeisa.runCommand(queryDelete);

                                if (isRejected)
                                {
                                    queryDelete = $"delete from dhl_cs_response_pesan where NO_AJU = '{nomorAju}'";
                                    DbOfficialCeisa.runCommand(queryDelete);
                                }

                                if (isRelease4)
                                {
                                    queryDelete = $"delete from dhl_cs_tpb_status_xml where NO_AJU = '{nomorAju}'";
                                    DbOfficialCeisa.runCommand(queryDelete);
                                }

                                if (DbOfficialCeisa.runCommand(listQueryRespon) == 0)
                                    logger.Log($"insert data response success");
                            }
                            #endregion

                            #region dataHeader
                            string query = $@"select top 1 * from dhl_cs_response_header where NO_AJU = '{nomorAju}'";
                            DataRow row = DbOfficialCeisa.getRow(query);

                            if (row != null)
                            {
                                //get latest response
                                query = $@"select top 1 kodeProses, 
                                        convert(varchar,waktuStatus,120) as waktuStatus, 
                                        keterangan from dhl_cs_response_status
                                        where NO_AJU = '{nomorAju}' order by waktuStatus desc";
                                DataRow dr = DbOfficialCeisa.getRow(query);

                                //update latest response
                                query = $@"update dhl_cs_response_header set
                                        latest_response_code = '{dr["kodeProses"]}',
                                        latest_response_time = '{dr["waktuStatus"]}',
                                        latest_response_note = '{dr["keterangan"]}'
                                        where NO_AJU = '{nomorAju}'";
                                if (DbOfficialCeisa.runCommand(query) == 0)
                                    logger.Log($"update latest response success");
                            }
                            else
                            {
                                //get latest response
                                query = $@"select top 1 kodeProses, 
                                        convert(varchar,waktuStatus,120) as waktuStatus, 
                                        keterangan from dhl_cs_response_status
                                        where NO_AJU = '{nomorAju}' order by waktuStatus desc";
                                DataRow dr = DbOfficialCeisa.getRow(query);

                                //insert latest response
                                query = $@"insert into dhl_cs_response_header values(
                                        '{HAWB}','{TGL_HAWB}','{nomorAju}',
                                        '{dr["waktuStatus"]}',
                                        '{dr["kodeProses"]}',
                                        '{dr["keterangan"]}')";
                                if (DbOfficialCeisa.runCommand(query) == 0)
                                    logger.Log($"insert latest response success");
                            }
                            #endregion

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Download Response Success.');", true);
                            LoadSummary();
                            InitDropdown();
                            return;
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{responseStatusAju.message}');", true);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebException we = ex as WebException;
                    if (we != null && we.Response is HttpWebResponse)
                    {
                        HttpWebResponse responseAju = (HttpWebResponse)we.Response;

                        // it can be 404, 500 etc...
                        //Console.WriteLine(response.StatusCode);
                        using (var streamReader = new StreamReader(responseAju.GetResponseStream()))
                        {
                            json = streamReader.ReadToEnd();
                            logger.Log($"response aju {nomorAju}" + Environment.NewLine + json);
                        }
                    }
                    else
                    {
                        logger.Log($"response aju {nomorAju}" + Environment.NewLine + we.Message);
                    }
                }
                LoadSummary();
                InitDropdown();
                //Response.Redirect("~/Views/RequestTPB.aspx");
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        private string GetBase64StringPdf(string url_pdf, string token, string file_type, string aju)
        {
            string retval = url_pdf;
            try
            {
                string linkUrl = $@"https://apis-gw.beacukai.go.id/openapi/download-respon?path={url_pdf}";
                //string linkUrl = $@"https://apisdev-gw.beacukai.go.id/openapi/download-respon?path={url_pdf}";

                string localPath = AppDomain.CurrentDomain.BaseDirectory + @"\Temp";

                if (!Directory.Exists(localPath))
                    Directory.CreateDirectory(localPath);

                bool isDownloaded = false;
                try
                {
                    using (var webClient = new WebClient())
                    {
                        webClient.Headers[HttpRequestHeader.Authorization] = $@"Bearer {token}";
                        webClient.Headers.Add("Content-Type", "application/pdf");

#if DEBUG
#else
                        WebProxy myproxy = new WebProxy("b2b-http.dhl.com", 8080);
                        myproxy.BypassProxyOnLocal = false;
                        webClient.Proxy = myproxy;
#endif
                        webClient.DownloadFile(linkUrl, $@"{localPath}\{file_type}_{aju}.pdf");
                        isDownloaded = true;
                    }
                }
                catch (Exception ex)
                {
                    WebException we = ex as WebException;
                    if (we != null && we.Response is HttpWebResponse)
                    {
                        HttpWebResponse response = (HttpWebResponse)we.Response;

                        // it can be 404, 500 etc...
                        //Console.WriteLine(response.StatusCode);
                        logger.Log($"{response.StatusCode}. File not found.");
                    }
                }

                if (isDownloaded)
                {
                    string fileStamped = $@"{localPath}\{file_type}_{aju}.pdf";
                    byte[] fileBytes = File.ReadAllBytes(fileStamped);
                    retval = Convert.ToBase64String(fileBytes);
                }
                else
                {
                    retval = String.Empty;
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
            return retval;
        }
        #endregion

        #region send data request AJU
        protected void btnSendDCI_Click(object sender, EventArgs e)
        {
            //ClearFormDCI();
            //ShowModalSend();
            try
            {
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

                List<string> listHAWB = new List<string>();
                List<string> listHAWB_Resend = new List<string>();
                List<string> list_resend_aju = new List<string>();
                if (ViewState["isChkAll"] != null)
                {
                    string isChkAll = ViewState["isChkAll"].ToString();


                    if (isChkAll == "false")
                    {
                        foreach (GridViewRow row in GV_DataDCI.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                bool isChecked = row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked;
                                if (!isChecked) continue;

                                string NO_AJU = GV_DataDCI.DataKeys[row.RowIndex].Values[2].ToString();
                                if (string.IsNullOrEmpty(NO_AJU) == false)
                                {
                                    list_resend_aju.Add($"HAWB : {GV_DataDCI.DataKeys[row.RowIndex].Values[0]} | Nomor AJU : {NO_AJU}");
                                    listHAWB_Resend.Add($"{GV_DataDCI.DataKeys[row.RowIndex].Values[0]}|{GV_DataDCI.DataKeys[row.RowIndex].Values[1]}");

                                    continue;
                                }

                                string HAWB = GV_DataDCI.DataKeys[row.RowIndex].Values[0].ToString();
                                string TGL_HAWB = GV_DataDCI.DataKeys[row.RowIndex].Values[1].ToString();
                                listHAWB.Add($"{HAWB}|{TGL_HAWB}");
                            }
                        }
                    }
                    else
                    {
                        DataTable dt = (DataTable)ViewState["ListDataDCI"];
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                string NO_AJU = row["No Aju"].ToString();
                                if (string.IsNullOrEmpty(NO_AJU) == false)
                                {
                                    list_resend_aju.Add($"HAWB : {row["HAWB"]} | Nomor AJU : {NO_AJU}");
                                    listHAWB_Resend.Add($"{row["HAWB"]}|{row["Tgl HAWB"]}");

                                    continue;
                                }

                                string HAWB = row["HAWB"].ToString();
                                string TGL_HAWB = row["Tgl HAWB"].ToString();
                                listHAWB.Add($"{HAWB}|{TGL_HAWB}");
                            }
                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('No rows selected.');", true);
                    CloseModalLoader();
                    return;
                }

                if (listHAWB_Resend.Count > 0)
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message", "if(confirm('Nomor aju sudah pernah dikirim ke CEISA 4.0 (" + listHAWB_Resend.Count + @") apakah yakin ingin kirim ulang?\n\n" + String.Join(@"\n", list_resend_aju.ToArray()) + "')){ alert('" + SendData(list_resend_aju) + "');} else { alert('NO'); }", true);

                    CloseModalLoader();
                    ClearFormDCI();

                    //remove duplicate
                    listHAWB_Resend = listHAWB_Resend.Distinct().ToList();
                    list_resend_aju = list_resend_aju.Distinct().ToList();
                    listHAWB = listHAWB.Distinct().ToList();

                    listAju.Text = String.Join("\n", list_resend_aju.ToArray());
                    ShowModalSend();

                    ViewState["listHAWB_Resend"] = listHAWB_Resend;

                    if (listHAWB.Count > 0) ViewState["listHAWB"] = listHAWB;
                    else ViewState["listHAWB"] = null;

                    ViewState["isFinal"] = false;

                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "confirm", "Confirm('Nomor aju sudah pernah dikirim ke CEISA 4.0 (" + listHAWB_Resend.Count + @") apakah yakin ingin kirim ulang?\n\n" + String.Join(@"\n", list_resend_aju.ToArray()) + "');", true);
                    return;
                }

                if (listHAWB.Count > 0)
                {
                    //get user password for request token
                    //string query = $@"select username, password from dhl_cs_settings where gateway = '{gateway}'";
                    //DataRow row = DbOfficialCeisa.getRow(query);
                    //if (row == null)
                    //{
                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Please fill username & password on gateway settings.');", true);
                    //    return;
                    //}

                    //string userCeisa = row["username"].ToString();
                    //string passwordCeisa = row["password"].ToString();

                    //ResponseToken response = JsonConvert.DeserializeObject<ResponseToken>(GetRequestToken(userCeisa, passwordCeisa));
                    //if (response.status != "success")
                    //{
                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{response.message}');", true);
                    //    return;
                    //}

                    //remove duplicate
                    listHAWB = listHAWB.Distinct().ToList();

                    int countRecord = 0;
                    foreach (var item in listHAWB)
                    {
                        string[] dr = item.Split('|');
                        countRecord += SendData(dr[0], dr[1], GetAccessToken(), GetIdToken(), gateway, false, false);
                    }

                    if (countRecord > 0)
                    {
                        logger.Log($"Send data success, {countRecord} nomor aju created");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Send data success, {countRecord} aju number created');", true);
                    }
                    else
                    {
                        logger.Log($"Send data failed, {countRecord} nomor aju created");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Send data failed.');", true);
                    }

                    CloseModalLoader();
                    //LoadSummary();
                    InitDropdown();
                }
                else
                {
                    CloseModalLoader();
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        protected void btnSendFinalDCI_Click(object sender, EventArgs e)
        {
            //ClearFormDCI();
            //ShowModalSend();
            try
            {
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

                List<string> listHAWB = new List<string>();
                List<string> listHAWB_Resend = new List<string>();
                List<string> list_resend_aju = new List<string>();
                if (ViewState["isChkAll"] != null)
                {
                    string isChkAll = ViewState["isChkAll"].ToString();

                    if (isChkAll == "false")
                    {
                        foreach (GridViewRow row in GV_DataDCI.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                bool isChecked = row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked;
                                if (!isChecked) continue;

                                string NO_AJU = GV_DataDCI.DataKeys[row.RowIndex].Values[2].ToString();
                                if (string.IsNullOrEmpty(NO_AJU) == false)
                                {
                                    list_resend_aju.Add($"HAWB : {GV_DataDCI.DataKeys[row.RowIndex].Values[0]} | Nomor AJU : {NO_AJU}");
                                    listHAWB_Resend.Add($"{GV_DataDCI.DataKeys[row.RowIndex].Values[0]}|{GV_DataDCI.DataKeys[row.RowIndex].Values[1]}");

                                    continue;
                                }

                                string HAWB = GV_DataDCI.DataKeys[row.RowIndex].Values[0].ToString();
                                string TGL_HAWB = GV_DataDCI.DataKeys[row.RowIndex].Values[1].ToString();
                                listHAWB.Add($"{HAWB}|{TGL_HAWB}");
                            }
                        }
                    }
                    else
                    {
                        DataTable dt = (DataTable)ViewState["ListDataDCI"];
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                string NO_AJU = row["No Aju"].ToString();
                                if (string.IsNullOrEmpty(NO_AJU) == false)
                                {
                                    list_resend_aju.Add($"HAWB : {row["HAWB"]} | Nomor AJU : {NO_AJU}");
                                    listHAWB_Resend.Add($"{row["HAWB"]}|{row["Tgl HAWB"]}");

                                    continue;
                                }

                                string HAWB = row["HAWB"].ToString();
                                string TGL_HAWB = row["Tgl HAWB"].ToString();
                                listHAWB.Add($"{HAWB}|{TGL_HAWB}");
                            }
                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('No rows selected.');", true);
                    CloseModalLoader();
                    return;
                }

                //testing loader
                //for (int i = 0; i < 9999; i++)
                //{
                //    logger.Log($"delay {i} second...");
                //}

                if (listHAWB_Resend.Count > 0)
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message", "if(confirm('Nomor aju sudah pernah dikirim ke CEISA 4.0 (" + listHAWB_Resend.Count + @") apakah yakin ingin kirim ulang?\n\n" + String.Join(@"\n", list_resend_aju.ToArray()) + "')){ alert('" + SendData(list_resend_aju) + "');} else { alert('NO'); }", true);

                    CloseModalLoader();
                    ClearFormDCI();

                    //remove duplicates
                    listHAWB_Resend = listHAWB_Resend.Distinct().ToList();
                    list_resend_aju = list_resend_aju.Distinct().ToList();
                    listHAWB = listHAWB.Distinct().ToList();

                    listAju.Text = String.Join("\n", list_resend_aju.ToArray());
                    ShowModalSend();

                    ViewState["listHAWB_Resend"] = listHAWB_Resend;

                    if (listHAWB.Count > 0) ViewState["listHAWB"] = listHAWB;
                    else ViewState["listHAWB"] = null;

                    ViewState["isFinal"] = true;

                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "confirm", "Confirm('Nomor aju sudah pernah dikirim ke CEISA 4.0 (" + listHAWB_Resend.Count + @") apakah yakin ingin kirim ulang?\n\n" + String.Join(@"\n", list_resend_aju.ToArray()) + "');", true);
                    return;
                }

                if (listHAWB.Count > 0)
                {
                    //remove duplicates
                    listHAWB = listHAWB.Distinct().ToList();

                    List<string> err_validation_skep = new List<string>();
                    int countRecord = 0;
                    foreach (var item in listHAWB)
                    {
                        string[] dr = item.Split('|');

                        if (IsSkepValid(dr[0], dr[1]) == false)
                        {
                            err_validation_skep.Add($"Invalid Tanggal SKEP - HAWB {dr[0]}");
                            continue;
                        }

                        countRecord += SendData(dr[0], dr[1], GetAccessToken(), GetIdToken(), gateway, true, false);
                        //countRecord += 1;
                    }

                    if (err_validation_skep.Count == 0)
                    {
                        logger.Log($"Send data success, {countRecord} nomor aju created");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Send data success, {countRecord} aju number created');", true);
                    }
                    else
                    {
                        string list_err = String.Join("\n", err_validation_skep.ToArray());
                        logger.Log($"Send data failed, {list_err}");
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        string jsArray = js.Serialize(err_validation_skep.ToArray());
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{String.Join(@"\n", err_validation_skep.ToArray())}');", true);
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Send data failed. {jsArray}');", true);
                    }

                    CloseModalLoader();
                    //LoadSummary();
                    InitDropdown();
                }
                else
                {
                    CloseModalLoader();
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
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

                //check empty input field
                if (string.IsNullOrEmpty(listAju.Text))
                {
                    ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "alert", $"alert('Please input HAWB number');", true);
                    return;
                }

                List<string> listHAWB = new List<string>();
                List<string> listHAWB_Resend = (List<string>)ViewState["listHAWB_Resend"];

                if (ViewState["listHAWB"] != null)
                {
                    listHAWB = (List<string>)ViewState["listHAWB"];
                }

                bool isFinal = false;
                if (ViewState["isFinal"] != null)
                {
                    isFinal = (bool)ViewState["isFinal"];
                }

                //testing loader
                //for (int i = 0; i < 9999; i++)
                //{
                //    logger.Log($"delay {i} second...");
                //}

                int countRecordResend = 0;
                if (listHAWB_Resend.Count > 0)
                {
                    //remove duplicates
                    listHAWB_Resend = listHAWB_Resend.Distinct().ToList();
                    foreach (var item in listHAWB_Resend)
                    {
                        string[] dr = item.Split('|');
                        countRecordResend += SendData(dr[0], dr[1], GetAccessToken(), GetIdToken(), gateway, isFinal, true);
                        //countRecordResend += 1;
                    }
                    logger.Log($"Resend data {countRecordResend} nomor aju success!");
                }

                int countRecord = 0;
                if (listHAWB.Count > 0)
                {
                    //remove duplicates
                    listHAWB = listHAWB.Distinct().ToList();
                    foreach (var item in listHAWB)
                    {
                        string[] dr = item.Split('|');
                        countRecord += SendData(dr[0], dr[1], GetAccessToken(), GetIdToken(), gateway, isFinal, false);
                        //countRecord += 1;
                    }
                    logger.Log($"Send data success, {countRecord} nomor aju created!");
                }

                if (countRecord > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $@"alert('Resend data {countRecordResend} aju number success!\n\nSend data success! {countRecord} aju number created.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Resend data {countRecordResend} aju number success!');", true);
                }

                CloseModalLoader();
                CloseModalSend();
                //LoadSummary();
                InitDropdown();
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        protected void SendData(string HAWB, string TGL_HAWB)
        {
            try
            {
                //validate session & get username gateway location
                string username = string.Empty;
                string gateway = string.Empty;
                LoginData user = SessionUtils.GetUserData(this.Page);
                if (user != null) { username = user.username; gateway = user.location; }
                else Response.Redirect("~/Auth/Login.aspx");

                //get user password for request token
                //string query = $@"select username, password from dhl_cs_settings where update_by = '{username}'";
                //DataRow row = DbOfficialCeisa.getRow(query);
                //if (row == null)
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Please fill username & password on data settings.');", true);
                //    return;
                //}

                //string userCeisa = row["username"].ToString();
                //string passwordCeisa = row["password"].ToString();

                //ResponseToken response = JsonConvert.DeserializeObject<ResponseToken>(GetRequestToken(userCeisa, passwordCeisa));
                //if (response.status != "success")
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{response.message}');", true);
                //    return;
                //}

                string queryHeader = $@"select
                                    asuransi, bruto, 
                                    fob + freight + asuransi as [cif],
                                    fob, freight, jabatanTtd,
                                    kodeIncoterm, kodeKantor,
                                    kodeKantorBongkar, kodePelBongkar,
                                    kodePelMuat, kodePelTransit,
                                    kodeTps, kodeValuta, kotaTtd,
                                    namaTtd, ndpbm, netto, nilaiBarang,
                                    nomorBc11, posBc11, subPosBc11,
                                    tanggalBc11, tanggalTiba, kodeKenaPajak
                                    from dhl_cs_tpb_header
                                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                DataRow rowHeader = DbOfficialCeisa.getRow(queryHeader);

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = new NullToEmptyListResolver();
                settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                //settings.Formatting = Formatting.Indented;

                //Model Request AJU
                #region header
                RequestAjuTpb req = new RequestAjuTpb();
                req.asalData = "S";
                req.asuransi = Convert.ToDouble(rowHeader["asuransi"]);
                req.bruto = Convert.ToDouble(rowHeader["bruto"]);
                req.cif = Convert.ToDouble(rowHeader["cif"]);
                req.fob = Convert.ToDouble(rowHeader["fob"]);
                req.freight = Convert.ToDouble(rowHeader["freight"]);
                req.hargaPenyerahan = 0;
                req.jabatanTtd = rowHeader["jabatanTtd"].ToString();
                req.jumlahKontainer = 0;
                req.kodeAsuransi = "LN";
                req.kodeDokumen = "23";
                req.kodeIncoterm = rowHeader["kodeIncoterm"].ToString();
                req.kodeKantor = rowHeader["kodeKantor"].ToString();
                req.kodeKantorBongkar = rowHeader["kodeKantorBongkar"].ToString();
                req.kodePelBongkar = rowHeader["kodePelBongkar"].ToString();
                req.kodePelMuat = rowHeader["kodePelMuat"].ToString();
                req.kodePelTransit = rowHeader["kodePelTransit"].ToString();
                req.kodeTps = rowHeader["kodeTps"].ToString();
                req.kodeTujuanTpb = "1";
                req.kodeTutupPu = "11";
                req.kodeValuta = rowHeader["kodeValuta"].ToString();
                req.kotaTtd = rowHeader["kotaTtd"].ToString();
                req.namaTtd = rowHeader["namaTtd"].ToString();
                req.ndpbm = Convert.ToDouble(rowHeader["ndpbm"]);
                req.netto = Convert.ToDouble(rowHeader["netto"]);
                req.nik = @"0018/KA-A5/111 TAHUN 2019";
                req.nilaiBarang = Convert.ToDouble(rowHeader["fob"]); //SAME VALUE WITH FOB
                req.nomorAju = GenerateAju();
                req.nomorBc11 = rowHeader["nomorBc11"].ToString();
                req.posBc11 = rowHeader["posBc11"].ToString();
                req.seri = 0;
                req.subposBc11 = rowHeader["subPosBc11"].ToString() + "0000";
                req.tanggalBc11 = GetDateJson(rowHeader["tanggalBc11"].ToString());
                req.tanggalTiba = GetDateJson(rowHeader["tanggalTiba"].ToString());
                req.tanggalTtd = DateTime.Now.ToString("yyyy-MM-dd");
                req.biayaTambahan = 0;
                req.biayaPengurang = 0;
                req.kodeKenaPajak = rowHeader["kodeKenaPajak"].ToString();

                string query_update_aju = $@"update dhl_cs_tpb_header set
                                            nomorAju = '{req.nomorAju}'
                                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                DbOfficialCeisa.runCommand(query_update_aju);
                #endregion

                #region barang
                req.barang = GetBarang(HAWB, TGL_HAWB, req.bruto);
                #endregion

                #region entitas
                req.entitas = GetEntitas(HAWB, TGL_HAWB);
                #endregion

                #region kemasan
                req.kemasan = GetKemasan(HAWB, TGL_HAWB);
                #endregion

                #region dokumen
                req.dokumen = GetDokumen(HAWB, TGL_HAWB);
                #endregion

                #region pengangkut
                req.pengangkut = GetPengangkut(HAWB, TGL_HAWB);
                #endregion

                string json = JsonConvert.SerializeObject(req, settings);
                logger.Log(json);

                string respJson = string.Empty;

                #region kirim data dokumen
                try
                {
                    string Url = $@"https://apis-gw.beacukai.go.id/openapi/document";
                    //string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/document";
                    //string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/document?isFinal=true";
                    Uri myUri = new Uri(Url, UriKind.Absolute);

                    WebRequest requestObjPost = WebRequest.Create(myUri);
                    requestObjPost.Method = "POST";
                    requestObjPost.ContentType = "application/json";
                    requestObjPost.Headers.Add("Authorization", $"Bearer {GetAccessToken()}");
                    requestObjPost.Headers.Add("Origin", "dhl.com");
                    requestObjPost.Headers.Add("Beacukai-Api-Key", $"{GetIdToken()}");

#if DEBUG
#else
                    WebProxy myproxy = new WebProxy("b2b-http.dhl.com", 8080);
                    myproxy.BypassProxyOnLocal = false;
                    requestObjPost.Proxy = myproxy;
#endif

                    using (var streamWriter = new StreamWriter(requestObjPost.GetRequestStream()))
                    {
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();

                        var resp = (HttpWebResponse)requestObjPost.GetResponse();

                        using (var streamReader = new StreamReader(resp.GetResponseStream()))
                        {
                            respJson = streamReader.ReadToEnd();
                            logger.Log($"response [kirim dokumen tpb - {req.nomorAju}]" + Environment.NewLine + respJson);

                            List<string> listQuery = new List<string>();
                            ResponseAju responseAju = JsonConvert.DeserializeObject<ResponseAju>(respJson);
                            if (responseAju.status == "OK")
                            {
                                //update nomor aju [CHANGE FLOW 21 DEC 2023 | AJU WILL UPDATED FIRST BEFORE GET RESPONSE]
                                //int sequenceAju = Convert.ToInt32(req.nomorAju.Substring(req.nomorAju.Length - 6));
                                //string tanggalAju = GetDateAju(req.nomorAju.Substring(req.nomorAju.Length - 14, 8));
                                //listQuery.Add($@"update dhl_cs_sequence_aju set
                                //                sequence_aju = {sequenceAju} where
                                //                gateway = '{gateway}'");
                                listQuery.Add($@"update dhl_cs_tpb_header set
                                                nomorAju = '{req.nomorAju}'
                                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'");
                                //DbOfficialCeisa.runCommand(listQuery);
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Send data success, no.aju {req.nomorAju}')", true);
                            }
                            else
                            {
                                if (responseAju.status.ToLower() == "failed")
                                {
                                    if (responseAju.message.ToLower() == "Gagal, Nomor Aju Sudah Ada.")
                                    {
                                        //update nomor aju [CHANGE FLOW 21 DEC 2023 | AJU WILL UPDATED FIRST BEFORE GET RESPONSE]
                                        //int sequenceAju = Convert.ToInt32(req.nomorAju.Substring(req.nomorAju.Length - 6));
                                        //string query = $@"update dhl_cs_sequence_aju set
                                        //sequence_aju = {sequenceAju} where
                                        //gateway = '{gateway}'";
                                        //DbOfficialCeisa.runCommand(query);

                                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Send data failed, no.aju {req.nomorAju} already exists, please try send again.')", true);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Send data failed, please try send again.')", true);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebException we = ex as WebException;
                    if (we != null && we.Response is HttpWebResponse)
                    {
                        HttpWebResponse resp = (HttpWebResponse)we.Response;

                        // it can be 404, 500 etc...
                        //Console.WriteLine(response.StatusCode);
                        using (var streamReader = new StreamReader(resp.GetResponseStream()))
                        {
                            respJson = streamReader.ReadToEnd();
                            logger.Log(respJson);
                        }
                    }
                    else
                    {
                        logger.Log(we.Message);
                    }
                }

                //LoadSummary();
                InitDropdown();
                #endregion

            }
            catch (Exception ex)
            {
                logger.Log(ex.Message, ex.StackTrace);
            }
        }
        protected int SendData(string HAWB, string TGL_HAWB, string access_token, string id_token, string gateway, bool isFinal, bool isResend)
        {
            int retval = 0;
            try
            {
                string queryHeader = $@"select
                                    asuransi, bruto, 
                                    fob + freight + asuransi as [cif],
                                    fob, freight, jabatanTtd,
                                    kodeIncoterm, kodeKantor,
                                    kodeKantorBongkar, kodePelBongkar,
                                    kodePelMuat, kodePelTransit,
                                    kodeTps, kodeValuta, kotaTtd,
                                    namaTtd, ndpbm, netto, nilaiBarang,
                                    nomorBc11, posBc11, subPosBc11,
                                    tanggalBc11, tanggalTiba,
                                    nomorAju, kodeKenaPajak
                                    from dhl_cs_tpb_header
                                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                DataRow rowHeader = DbOfficialCeisa.getRow(queryHeader);

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = new NullToEmptyListResolver();
                settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                //settings.Formatting = Formatting.Indented;

                //Model Request AJU
                #region header
                RequestAjuTpb req = new RequestAjuTpb();
                req.asalData = "S";
                req.asuransi = Convert.ToDouble(rowHeader["asuransi"]);
                req.bruto = Convert.ToDouble(rowHeader["bruto"]);
                req.cif = Convert.ToDouble(rowHeader["cif"]);
                req.fob = Convert.ToDouble(rowHeader["fob"]);
                req.freight = Convert.ToDouble(rowHeader["freight"]);
                req.hargaPenyerahan = 0;
                req.jabatanTtd = rowHeader["jabatanTtd"].ToString();
                req.jumlahKontainer = 0;
                req.kodeAsuransi = "LN";
                req.kodeDokumen = "23";
                req.kodeIncoterm = rowHeader["kodeIncoterm"].ToString();
                req.kodeKantor = rowHeader["kodeKantor"].ToString();
                req.kodeKantorBongkar = rowHeader["kodeKantorBongkar"].ToString();
                req.kodePelBongkar = rowHeader["kodePelBongkar"].ToString();
                req.kodePelMuat = rowHeader["kodePelMuat"].ToString();
                req.kodePelTransit = rowHeader["kodePelTransit"].ToString();
                req.kodeTps = rowHeader["kodeTps"].ToString();
                req.kodeTujuanTpb = "1";
                req.kodeTutupPu = "11";
                req.kodeValuta = rowHeader["kodeValuta"].ToString();
                req.kotaTtd = rowHeader["kotaTtd"].ToString();
                req.namaTtd = rowHeader["namaTtd"].ToString();
                req.ndpbm = Convert.ToInt32(rowHeader["ndpbm"]);
                req.netto = Convert.ToDouble(rowHeader["netto"]);
                req.nik = "0018/KA-A5/111 TAHUN 2019";
                req.nilaiBarang = Convert.ToDouble(rowHeader["fob"]); //SAME VALUE WITH FOB
                req.nomorAju = isResend ? rowHeader["nomorAju"].ToString() : GenerateAju();
                req.nomorBc11 = rowHeader["nomorBc11"].ToString();
                req.posBc11 = rowHeader["posBc11"].ToString();
                req.seri = 0;
                req.subposBc11 = rowHeader["subPosBc11"].ToString() + "0000";
                req.tanggalBc11 = GetDateJson(rowHeader["tanggalBc11"].ToString());
                req.tanggalTiba = GetDateJson(rowHeader["tanggalTiba"].ToString());
                req.tanggalTtd = DateTime.Now.ToString("yyyy-MM-dd");
                req.biayaTambahan = 0;
                req.biayaPengurang = 0;
                req.kodeKenaPajak = rowHeader["kodeKenaPajak"].ToString();

                string query_update_aju = $@"update dhl_cs_tpb_header set
                                            nomorAju = '{req.nomorAju}'
                                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                DbOfficialCeisa.runCommand(query_update_aju);
                #endregion

                #region barang
                req.barang = GetBarang(HAWB, TGL_HAWB, req.bruto);
                #endregion

                #region entitas
                req.entitas = GetEntitas(HAWB, TGL_HAWB);
                #endregion

                #region kemasan
                req.kemasan = GetKemasan(HAWB, TGL_HAWB);
                #endregion

                #region dokumen
                req.dokumen = GetDokumen(HAWB, TGL_HAWB);
                #endregion

                #region pengangkut
                req.pengangkut = GetPengangkut(HAWB, TGL_HAWB);
                #endregion

                string json = JsonConvert.SerializeObject(req, settings);
                logger.Log(json);

                string respJson = string.Empty;
                string query = string.Empty;

                #region kirim data dokumen
                try
                {
                    string Url = string.Empty;
                    if (isFinal) Url = $@"https://apis-gw.beacukai.go.id/openapi/document?isFinal=true";
                    else Url = $@"https://apis-gw.beacukai.go.id/openapi/document";

                    //string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/document";
                    //string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/document?isFinal=false";
                    Uri myUri = new Uri(Url, UriKind.Absolute);

                    WebRequest requestObjPost = WebRequest.Create(myUri);
                    requestObjPost.Method = "POST";
                    requestObjPost.ContentType = "application/json";
                    requestObjPost.Headers.Add("Authorization", $"Bearer {access_token}");
                    requestObjPost.Headers.Add("Origin", "dhl.com");
                    requestObjPost.Headers.Add("Beacukai-Api-Key", $"{id_token}");

#if DEBUG
#else
                    WebProxy myproxy = new WebProxy("b2b-http.dhl.com", 8080);
                    myproxy.BypassProxyOnLocal = false;
                    requestObjPost.Proxy = myproxy;
#endif

                    using (var streamWriter = new StreamWriter(requestObjPost.GetRequestStream()))
                    {
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();

                        var resp = (HttpWebResponse)requestObjPost.GetResponse();

                        using (var streamReader = new StreamReader(resp.GetResponseStream()))
                        {
                            respJson = streamReader.ReadToEnd();
                            logger.Log($"response [kirim dokumen tpb - {req.nomorAju}]" + Environment.NewLine + respJson);

                            List<string> listQuery = new List<string>();
                            ResponseAju responseAju = JsonConvert.DeserializeObject<ResponseAju>(respJson);
                            if (responseAju.status == "OK")
                            {
                                //update nomor aju [CHANGE FLOW 21 DEC 2023 | AJU WILL UPDATED FIRST BEFORE GET RESPONSE]
                                //int sequenceAju = Convert.ToInt32(req.nomorAju.Substring(req.nomorAju.Length - 6));
                                //string tanggalAju = GetDateAju(req.nomorAju.Substring(req.nomorAju.Length - 14, 8));
                                //listQuery.Add($@"update dhl_cs_sequence_aju set
                                //                sequence_aju = {sequenceAju} where
                                //                gateway = '{gateway}'");
                                //listQuery.Add($@"update dhl_cs_tpb_header set
                                //                nomorAju = '{req.nomorAju}'
                                //                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'");
                                //DbOfficialCeisa.runCommand(listQuery);
                                retval = 1;
                            }
                            else
                            {
                                if (responseAju.status.ToLower() == "failed")
                                {
                                    if (responseAju.message.ToLower() == "gagal, nomor aju sudah ada.")
                                    {
                                        //update nomor aju [CHANGE FLOW 21 DEC 2023 | AJU WILL UPDATED FIRST BEFORE GET RESPONSE]
                                        //int sequenceAju = Convert.ToInt32(req.nomorAju.Substring(req.nomorAju.Length - 6));
                                        //query = $@"update dhl_cs_sequence_aju set
                                        //sequence_aju = {sequenceAju} where
                                        //gateway = '{gateway}'";
                                        //DbOfficialCeisa.runCommand(query);

                                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Send data failed, no.aju {req.nomorAju} already exists, please try send again.')", true);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Send data failed, {responseAju.message.ToLower()}, please try send again.')", true);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebException we = ex as WebException;
                    if (we != null && we.Response is HttpWebResponse)
                    {
                        HttpWebResponse resp = (HttpWebResponse)we.Response;

                        // it can be 404, 500 etc...
                        //Console.WriteLine(response.StatusCode);
                        using (var streamReader = new StreamReader(resp.GetResponseStream()))
                        {
                            respJson = streamReader.ReadToEnd();
                            logger.Log(respJson);
                        }
                    }
                    else
                    {
                        logger.Log(we.Message);
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
            return retval;
        }
        private string GenerateAju()
        {
            string retval = string.Empty;
            try
            {
                //validate session & get user gateway location
                string gateway = string.Empty;
                LoginData user = SessionUtils.GetUserData(this.Page);
                if (user != null) gateway = user.location;
                else Response.Redirect("~/Auth/Login.aspx");

                string query = $@"select sequence_aju, kode_aju from dhl_cs_sequence_aju
                                where gateway = '{gateway}'";
                DataRow row = DbOfficialCeisa.getRow(query);
                int sequenceAju = Convert.ToInt32(row["sequence_aju"]);
                string kodeAju = row["kode_aju"].ToString();
                string tglAju = DateTime.Now.ToString("yyyyMMdd");
                int temp_sequenceAju = sequenceAju;
                sequenceAju = ++sequenceAju;
                retval = $"{kodeAju}{tglAju}{(sequenceAju).ToString().PadLeft(6, '0')}";

                query = $@"update dhl_cs_sequence_aju set
                        sequence_aju = {sequenceAju} where
                        gateway = '{gateway}'";
                if (DbOfficialCeisa.runCommand(query) == 0)
                    logger.Log($"sequence gateway {gateway} updated from {temp_sequenceAju} to {sequenceAju}..");

            }
            catch { }
            return retval;
        }
        private string GetRequestToken(string username, string password)
        {
            string json = string.Empty;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                RequestToken req = new RequestToken();
                req.username = username;
                req.password = password;
                string reqJson = Newtonsoft.Json.JsonConvert.SerializeObject(req);

#if DEBUG
                //string Url = $@"https://apisdev-gw.beacukai.go.id/amws/v1/user/login";
                string Url = $@"https://apis-gw.beacukai.go.id/v2/amws/v1/user/login";
#else
                string Url = $@"https://apis-gw.beacukai.go.id/v2/amws/v1/user/login";
                //string Url = $@"https://apisdev-gw.beacukai.go.id/amws/v1/user/login";
#endif
                Uri myUri = new Uri(Url, UriKind.Absolute);

                WebRequest requestObjPost = WebRequest.Create(myUri);
                requestObjPost.Method = "POST";
                requestObjPost.ContentType = "application/json";

#if DEBUG
#else
                WebProxy myproxy = new WebProxy("b2b-http.dhl.com", 8080);
                myproxy.BypassProxyOnLocal = false;
                requestObjPost.Proxy = myproxy;
#endif

                using (var streamWriter = new StreamWriter(requestObjPost.GetRequestStream()))
                {
                    streamWriter.Write(reqJson);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var response = (HttpWebResponse)requestObjPost.GetResponse();

                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        json = streamReader.ReadToEnd();
                    }
                }

            }
            catch (Exception ex)
            {
                WebException we = ex as WebException;
                if (we != null && we.Response is HttpWebResponse)
                {
                    HttpWebResponse response = (HttpWebResponse)we.Response;

                    // it can be 404, 500 etc...
                    //Console.WriteLine(response.StatusCode);
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        json = streamReader.ReadToEnd();
                    }
                }
            }
            return json;
        }
        private string GetAccessToken()
        {
            string retval = string.Empty;
            try
            {
                string query = $@"select top 1 AccessToken from DataTokenCeisa4 where Tanggal = '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd")}' order by CreateAt desc";
                DataRow row = DbOfficialDCI.getRow(query);

                if (row != null) retval = row["AccessToken"].ToString();
                logger.Log($"Token Access: {retval}");
            }
            catch (Exception ex)
            {
                logger.Log($"err func GetAccessToken: {ex.Message}");
            }
            return retval;
        }
        private string GetIdToken()
        {
            string retval = string.Empty;
            try
            {
                string query = $@"select top 1 IdToken from DataTokenCeisa4 where Tanggal = '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd")}' order by CreateAt desc";
                DataRow row = DbOfficialDCI.getRow(query);

                if (row != null) retval = row["IdToken"].ToString();
                logger.Log($"Token ID: {retval}");
            }
            catch (Exception ex)
            {
                logger.Log($"err func GetIdToken: {ex.Message}");
            }
            return retval;
        }
        private string GetKurs(string valuta, string token, string ndpbm)
        {
            string retval = ndpbm;
            try
            {
                string Url = $@"https://apis-gw.beacukai.go.id/openapi/kurs/{valuta}";
                Uri myUri = new Uri(Url, UriKind.Absolute);

                WebRequest requestObjGet = WebRequest.Create(myUri);
                requestObjGet.Method = "GET";
                requestObjGet.Headers.Add("Authorization", $"Bearer {token}");

#if DEBUG
#else
                WebProxy myproxy = new WebProxy("b2b-http.dhl.com", 8080);
                myproxy.BypassProxyOnLocal = false;
                requestObjGet.Proxy = myproxy;
#endif

                string json = string.Empty;
                try
                {
                    var responseAju = (HttpWebResponse)requestObjGet.GetResponse();

                    using (var streamReader = new StreamReader(responseAju.GetResponseStream()))
                    {
                        json = streamReader.ReadToEnd();
                        logger.Log($"response kurs {valuta}" + Environment.NewLine + json);

                        ResponseKurs responseKurs = JsonConvert.DeserializeObject<ResponseKurs>(json);
                        if (responseKurs.status == "true" || responseKurs.message == "success")
                        {
                            Data[] list_kurs = responseKurs.data.ToArray();

                            if (list_kurs.Length > 0)
                            {
                                foreach (var item in list_kurs)
                                {
                                    retval = item.nilaiKurs;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebException we = ex as WebException;
                    if (we != null && we.Response is HttpWebResponse)
                    {
                        HttpWebResponse responseAju = (HttpWebResponse)we.Response;

                        // it can be 404, 500 etc...
                        //Console.WriteLine(response.StatusCode);
                        using (var streamReader = new StreamReader(responseAju.GetResponseStream()))
                        {
                            json = streamReader.ReadToEnd();
                            logger.Log($"response kurs {valuta}" + Environment.NewLine + json);
                        }
                    }
                    else
                    {
                        logger.Log($"response kurs {valuta}" + Environment.NewLine + we.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
            return retval;
        }
        private List<BarangTpb> GetBarang(string HAWB, string TGL_HAWB, double bruto)
        {
            List<BarangTpb> barang = new List<BarangTpb>();
            try
            {
                string queryBarang = $@"select 
                                        asuransi, 
                                        fob + freight + asuransi as cif, 
                                        cifRupiah, fob, freight,
                                        hargaSatuan, jumlahSatuan, 
                                        kodeNegaraAsal, kodeSatuanBarang,
                                        kodeJenisKemasan, kodeKategoriBarang,
                                        fob as [nilaiBarang],
                                        ndpbm, netto, posTarif, seriBarang, uraian, kodeBarang,
                                        merk, spesifikasiLain, tipe, ukuran, jumlahKemasan
                                        from dhl_cs_tpb_barang
                                        where HAWB = '{HAWB}' and 
                                        TGL_HAWB = '{TGL_HAWB}'";
                DataTable tblBarang = DbOfficialCeisa.getRecords(queryBarang);
                if (tblBarang.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblBarang.Rows)
                    {
                        double rowNetto = Convert.ToDouble(dr["netto"]);
                        //double percentage = ((double)90 / 100) * 100;
                        //if (rowNetto == 0) rowNetto = Math.Round((double)(((double)percentage / 100) * bruto) / tblBarang.Rows.Count, 2);
                        //if (rowNetto == 0) rowNetto = Math.Round((double)bruto / tblBarang.Rows.Count, 2);

                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("seriDokumen", "1");

                        barang.Add(new BarangTpb()
                        {
                            idBarang = dr["seriBarang"].ToString(),
                            asuransi = Convert.ToDouble(dr["asuransi"]),
                            cif = Convert.ToDouble(dr["cif"]),
                            diskon = 0,
                            fob = Convert.ToDouble(dr["fob"]),
                            freight = Convert.ToDouble(dr["freight"]),
                            hargaEkspor = 0,
                            hargaPenyerahan = 0,
                            hargaSatuan = Convert.ToDouble(dr["hargaSatuan"]),
                            isiPerKemasan = 0,
                            jumlahKemasan = Convert.ToInt32(dr["jumlahKemasan"]),
                            jumlahSatuan = Convert.ToDouble(dr["jumlahSatuan"]),
                            kodeBarang = string.IsNullOrEmpty(dr["kodeBarang"].ToString()) == false ? dr["kodeBarang"].ToString() : "-",
                            kodeDokumen = "23",
                            kodeKategoriBarang = dr["kodeKategoriBarang"].ToString(),
                            kodeJenisKemasan = dr["kodeJenisKemasan"].ToString(),
                            kodeNegaraAsal = dr["kodeNegaraAsal"].ToString(),
                            kodePerhitungan = "0",
                            kodeSatuanBarang = dr["kodeSatuanBarang"].ToString(),
                            merk = dr["merk"].ToString(),
                            netto = rowNetto,
                            nilaiBarang = Math.Round(Convert.ToDouble(dr["nilaiBarang"]), 2, MidpointRounding.AwayFromZero),
                            nilaiTambah = 0,
                            posTarif = dr["posTarif"].ToString(),
                            seriBarang = Convert.ToInt32(dr["seriBarang"]),
                            spesifikasiLain = dr["spesifikasiLain"].ToString(),
                            tipe = dr["tipe"].ToString(),
                            ukuran = dr["ukuran"].ToString(),
                            uraian = dr["uraian"].ToString(),
                            ndpbm = Convert.ToDouble(dr["ndpbm"]),
                            cifRupiah = Convert.ToDouble(dr["cifRupiah"]),
                            hargaPerolehan = 0,
                            kodeAsalBahanBaku = "0",
                            barangTarif = GetBarangTarif(HAWB, TGL_HAWB, Convert.ToInt32(dr["seriBarang"]))
                        });
                    }
                }
            }
            catch { }
            return barang;
        }
        private List<BarangTarifTpb> GetBarangTarif(string HAWB, string TGL_HAWB, int seriBarang)
        {
            List<BarangTarifTpb> barangTarifs = new List<BarangTarifTpb>();
            try
            {
                string queryBarangTarif = $@"select 
                                            a.jumlahSatuan, a.kodeSatuanBarang,
                                            kodeJenisPungutan, nilaiBayar, 
                                            a.seriBarang, tarif,
                                            format(cast(replace(format(fob+freight+asuransi,'N2'),',','') as float) * ndpbm, 'N2') as cifRupiah
                                            from dhl_cs_tpb_barang_tarif a
                                            left join dhl_cs_tpb_barang b
                                            on a.HAWB = b.HAWB and a.TGL_HAWB = b.TGL_HAWB and a.seriBarang = b.seriBarang
                                            where a.HAWB = '{HAWB}' and 
                                            a.TGL_HAWB = '{TGL_HAWB}' and
                                            a.seriBarang = '{seriBarang}'";
                DataTable tblBarangTarif = DbOfficialCeisa.getRecords(queryBarangTarif);
                if (tblBarangTarif.Rows.Count > 0)
                {
                    double tarif_BM = 0;
                    //double tarif_BM_ori = 0;
                    double tarif_PPH_or_PPN = 0;
                    //double tarif_PPH_or_PPN_ori = 0;
                    double tarif_BMTP = 0;
                    double tarif_BMTP_ori = 0;
                    double jumlah_satuan_bmtp = 1;
                    string kode_satuan_bmtp = "KGM";

                    //urutan BMTP dirubah per 28 okt 2024
                    bool isBMTP = false;
                    foreach (DataRow dr in tblBarangTarif.Rows)
                    {
                        string kodeJenisPungutan = dr["kodeJenisPungutan"].ToString();
                        double tarif = Convert.ToDouble(dr["tarif"]);
                        double cifRupiah = Convert.ToDouble(dr["cifRupiah"].ToString().Replace(",", ""));
                        switch (kodeJenisPungutan)
                        {
                            case "PPN":
                            case "PPH":
                                //tarif_PPH_or_PPN_ori = (tarif / 100) * (cifRupiah + tarif_BM_ori);
                                //tarif_PPH_or_PPN = tarif_PPH_or_PPN_ori % 1000 >= 500 ? tarif_PPH_or_PPN_ori + 1000 - tarif_PPH_or_PPN_ori % 1000 : tarif_PPH_or_PPN_ori - tarif_PPH_or_PPN_ori % 1000;

                                //update 2023-12-30 [Tarif tidak dibulatkan 1000]
                                tarif_PPH_or_PPN = (tarif / 100) * (cifRupiah + tarif_BM);
                                barangTarifs.Add(new BarangTarifTpb()
                                {
                                    kodeJenisTarif = "1",
                                    jumlahSatuan = Convert.ToDouble(dr["jumlahSatuan"]),
                                    kodeFasilitasTarif = "6",
                                    kodeSatuanBarang = dr["kodeSatuanBarang"].ToString(),
                                    kodeJenisPungutan = kodeJenisPungutan,
                                    nilaiBayar = 0.0,
                                    //nilaiFasilitas = Math.Round(tarif_PPH_or_PPN, 2, MidpointRounding.AwayFromZero),
                                    nilaiFasilitas = Convert.ToDouble(String.Format("{0:0.00}", tarif_PPH_or_PPN)),
                                    nilaiSudahDilunasi = 0.0,
                                    seriBarang = Convert.ToInt32(dr["seriBarang"]),
                                    tarif = tarif,
                                    tarifFasilitas = 100
                                });
                                break;
                            case "BM": //BM
                                //tarif_BM_ori = (tarif / 100) * cifRupiah;
                                //tarif_BM = tarif_BM_ori % 1000 >= 500 ? tarif_BM_ori + 1000 - tarif_BM_ori % 1000 : tarif_BM_ori - tarif_BM_ori % 1000;                                

                                //update 2023-12-30 [Tarif tidak dibulatkan 1000]
                                tarif_BM = (tarif / 100) * cifRupiah;
                                barangTarifs.Add(new BarangTarifTpb()
                                {
                                    kodeJenisTarif = "1",
                                    jumlahSatuan = Convert.ToDouble(dr["jumlahSatuan"]),
                                    kodeFasilitasTarif = "3",
                                    kodeSatuanBarang = dr["kodeSatuanBarang"].ToString(),
                                    kodeJenisPungutan = kodeJenisPungutan,
                                    nilaiBayar = 0.0,
                                    //nilaiFasilitas = Math.Round(tarif_BM, 2, MidpointRounding.AwayFromZero),
                                    nilaiFasilitas = Convert.ToDouble(String.Format("{0:0.00}", tarif_BM)),
                                    nilaiSudahDilunasi = 0.0,
                                    seriBarang = Convert.ToInt32(dr["seriBarang"]),
                                    tarif = tarif,
                                    tarifFasilitas = 100
                                });
                                break;
                            case "BMTP":
                                if (tarif > 0)
                                {
                                    isBMTP = true;
                                    tarif_BMTP_ori = tarif;
                                    tarif_BMTP = tarif * Convert.ToDouble(dr["jumlahSatuan"]);
                                    jumlah_satuan_bmtp = Convert.ToDouble(dr["jumlahSatuan"]);
                                    kode_satuan_bmtp = dr["kodeSatuanBarang"].ToString();
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    if (isBMTP)
                    {
                        barangTarifs.Add(new BarangTarifTpb()
                        {
                            kodeJenisTarif = "2",
                            jumlahSatuan = jumlah_satuan_bmtp,
                            kodeFasilitasTarif = "3",
                            kodeSatuanBarang = kode_satuan_bmtp,
                            kodeJenisPungutan = "BMTP",
                            nilaiBayar = 0.0,
                            //nilaiFasilitas = Math.Round(tarif_BMTP, 2, MidpointRounding.AwayFromZero),
                            nilaiFasilitas = Convert.ToDouble(String.Format("{0:0.00}", tarif_BMTP)),
                            nilaiSudahDilunasi = 0.0,
                            seriBarang = seriBarang,
                            tarif = tarif_BMTP_ori,
                            tarifFasilitas = 100
                        });
                    }
                }
            }
            catch { }
            return barangTarifs;
        }
        private List<Object> GetEntitas(string HAWB, string TGL_HAWB)
        {
            List<Object> entitas = new List<Object>();
            try
            {
                string queryEntitas = $@"select 
                                        alamatEntitas, kodeEntitas,
                                        kodeJenisApi, kodeJenisIdentitas,
                                        kodeStatus, namaEntitas,
                                        nibEntitas, nomorIdentitas,
                                        nomorIjinEntitas,
                                        kodeNegara, seriEntitas,
                                        isnull(convert(varchar(10),tanggalIjinEntitas,20),'1900-01-01') as tanggalIjinEntitas
                                        from dhl_cs_tpb_entitas
                                        where HAWB = '{HAWB}' and 
                                        TGL_HAWB = '{TGL_HAWB}'
                                        order by seriEntitas";
                DataTable tblEntitas = DbOfficialCeisa.getRecords(queryEntitas);
                if (tblEntitas.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblEntitas.Rows)
                    {
                        EntitasTpb ent = new EntitasTpb();
                        ent.alamatEntitas = dr["alamatEntitas"].ToString();
                        ent.kodeEntitas = dr["kodeEntitas"].ToString();
                        ent.kodeJenisApi = dr["kodeJenisApi"].ToString();
                        ent.kodeJenisIdentitas = dr["kodeJenisIdentitas"].ToString();
                        ent.kodeStatus = dr["kodeStatus"].ToString();
                        ent.namaEntitas = dr["namaEntitas"].ToString();
                        ent.nibEntitas = dr["nibEntitas"].ToString();
                        ent.nomorIdentitas = dr["nomorIdentitas"].ToString();
                        ent.kodeNegara = dr["kodeNegara"].ToString();
                        ent.seriEntitas = Convert.ToInt32(dr["seriEntitas"]);
                        ent.nomorIjinEntitas = dr["nomorIjinEntitas"].ToString();
                        ent.tanggalIjinEntitas = dr["tanggalIjinEntitas"].ToString();

                        Dictionary<string, dynamic> objEntitas = new Dictionary<string, dynamic>();
                        if (ent.kodeEntitas == "3")
                        {
                            objEntitas.Add("alamatEntitas", ent.alamatEntitas);
                            objEntitas.Add("kodeEntitas", ent.kodeEntitas);
                            objEntitas.Add("kodeJenisIdentitas", ent.kodeJenisIdentitas);
                            objEntitas.Add("namaEntitas", ent.namaEntitas);
                            objEntitas.Add("nibEntitas", ent.nibEntitas);
                            objEntitas.Add("nomorIdentitas", ent.nomorIdentitas);
                            objEntitas.Add("tanggalIjinEntitas", ent.tanggalIjinEntitas);
                            objEntitas.Add("nomorIjinEntitas", ent.nomorIjinEntitas);
                            objEntitas.Add("seriEntitas", ent.seriEntitas);

                            // NPWP 16 & NITKU 22
                            string kode3_npwp16 = ent.nomorIdentitas;
                            if (kode3_npwp16.Length == 22)
                            {
                                kode3_npwp16 = kode3_npwp16.Substring(0, 16);
                            }
                            else if (kode3_npwp16.Length == 15)
                            {
                                kode3_npwp16 = $"0{kode3_npwp16}";
                            }
                            objEntitas.Add("npwp16", kode3_npwp16);
                        }
                        else if (ent.kodeEntitas == "5")
                        {
                            objEntitas.Add("alamatEntitas", ent.alamatEntitas);
                            objEntitas.Add("kodeEntitas", ent.kodeEntitas);
                            objEntitas.Add("kodeNegara", ent.kodeNegara);
                            objEntitas.Add("namaEntitas", ent.namaEntitas);
                            objEntitas.Add("seriEntitas", ent.seriEntitas);
                        }
                        else if (ent.kodeEntitas == "7")
                        {
                            objEntitas.Add("alamatEntitas", ent.alamatEntitas);
                            objEntitas.Add("kodeEntitas", ent.kodeEntitas);
                            objEntitas.Add("kodeJenisApi", ent.kodeJenisApi);
                            objEntitas.Add("kodeJenisIdentitas", ent.kodeJenisIdentitas);
                            objEntitas.Add("kodeStatus", ent.kodeStatus);
                            objEntitas.Add("namaEntitas", ent.namaEntitas);
                            objEntitas.Add("nomorIdentitas", ent.nomorIdentitas);
                            objEntitas.Add("nomorIjinEntitas", ent.nomorIjinEntitas);
                            objEntitas.Add("tanggalIjinEntitas", ent.tanggalIjinEntitas);
                            objEntitas.Add("seriEntitas", ent.seriEntitas);

                            // NPWP 16 & NITKU 22
                            string kode7_npwp16 = ent.nomorIdentitas;
                            if (kode7_npwp16.Length == 22)
                            {
                                kode7_npwp16 = kode7_npwp16.Substring(0, 16);
                            }
                            else if (kode7_npwp16.Length == 15)
                            {
                                kode7_npwp16 = $"0{kode7_npwp16}";
                            }
                            objEntitas.Add("npwp16", kode7_npwp16);
                        }
                        else
                        {
                            //4
                            objEntitas.Add("alamatEntitas", ent.alamatEntitas);
                            objEntitas.Add("kodeEntitas", ent.kodeEntitas);
                            objEntitas.Add("kodeJenisIdentitas", ent.kodeJenisIdentitas);
                            objEntitas.Add("kodeStatus", ent.kodeStatus);
                            objEntitas.Add("namaEntitas", ent.namaEntitas);
                            objEntitas.Add("nomorIdentitas", ent.nomorIdentitas);
                            objEntitas.Add("seriEntitas", ent.seriEntitas);

                            // NPWP 16 & NITKU 22
                            string kode4_npwp16 = ent.nomorIdentitas;
                            if (kode4_npwp16.Length == 22)
                            {
                                kode4_npwp16 = kode4_npwp16.Substring(0, 16);
                            }
                            else if (kode4_npwp16.Length == 15)
                            {
                                kode4_npwp16 = $"0{kode4_npwp16}";
                            }
                            objEntitas.Add("npwp16", kode4_npwp16);
                        }

                        entitas.Add(objEntitas);
                    }
                }
            }
            catch { }
            return entitas;
        }
        private List<KemasanTpb> GetKemasan(string HAWB, string TGL_HAWB)
        {
            List<KemasanTpb> kemasan = new List<KemasanTpb>();
            try
            {
                string queryKemasan = $@"select 
                                        jumlahKemasan, kodeJenisKemasan, seriKemasan, merkKemasan
                                        from dhl_cs_tpb_kemasan
                                        where HAWB = '{HAWB}' and 
                                        TGL_HAWB = '{TGL_HAWB}'";
                DataTable tblKemasan = DbOfficialCeisa.getRecords(queryKemasan);
                if (tblKemasan.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblKemasan.Rows)
                    {
                        kemasan.Add(new KemasanTpb()
                        {
                            jumlahKemasan = Convert.ToInt32(dr["jumlahKemasan"]),
                            kodeJenisKemasan = dr["kodeJenisKemasan"].ToString(),
                            seriKemasan = Convert.ToInt32(dr["seriKemasan"]),
                            merkKemasan = dr["merkKemasan"].ToString()
                        });
                    }
                }
            }
            catch { }
            return kemasan;
        }
        private List<DokumenTpb> GetDokumen(string HAWB, string TGL_HAWB)
        {
            List<DokumenTpb> dokumen = new List<DokumenTpb>();
            try
            {
                string queryDokumen = $@"select
                                        kodeDokumen, nomorDokumen,
                                        seriDokumen, tanggalDokumen
                                        from dhl_cs_tpb_dokumen
                                        where HAWB = '{HAWB}' and 
                                        TGL_HAWB = '{TGL_HAWB}'
                                        group by kodeDokumen, nomorDokumen,
                                        seriDokumen, tanggalDokumen
                                        order by seridokumen asc, tanggalDokumen desc";
                DataTable tblDokumen = DbOfficialCeisa.getRecords(queryDokumen);
                if (tblDokumen.Rows.Count > 0)
                {
                    bool seriDok_1 = false;
                    bool seriDok_2 = false;
                    bool seriDok_3 = false;

                    List<string> list_invoice = new List<string>();
                    List<string> list_hawb = new List<string>();
                    List<string> list_mawb = new List<string>();
                    List<string> list_others = new List<string>();

                    foreach (DataRow dr in tblDokumen.Rows)
                    {
                        string kode_dokumen = dr["kodeDokumen"].ToString();

                        if (kode_dokumen == "380" && seriDok_1 == false)
                        {
                            list_invoice.Add($"{dr["nomorDokumen"]}|{dr["tanggalDokumen"]}");
                            seriDok_1 = true;
                        }
                        else if (kode_dokumen == "740" && seriDok_2 == false)
                        {
                            list_hawb.Add($"{dr["nomorDokumen"]}|{dr["tanggalDokumen"]}");
                            seriDok_2 = true;
                        }
                        else if (kode_dokumen == "741" && seriDok_3 == false)
                        {
                            list_mawb.Add($"{dr["nomorDokumen"]}|{dr["tanggalDokumen"]}");
                            seriDok_3 = true;
                        }
                        else
                        {
                            list_others.Add($"{dr["nomorDokumen"]}|{dr["tanggalDokumen"]}|{dr["kodeDokumen"]}");
                        }
                    }

                    if (list_invoice.Count > 0)
                    {
                        foreach (var item in list_invoice)
                        {
                            string[] arr_invoice = item.Split('|');

                            dokumen.Add(new DokumenTpb()
                            {
                                idDokumen = "1",
                                kodeDokumen = "380",
                                nomorDokumen = arr_invoice[0],
                                seriDokumen = 1,
                                tanggalDokumen = GetDateJson(arr_invoice[1])
                            });
                        }
                    }

                    if (list_hawb.Count > 0)
                    {
                        foreach (var item in list_hawb)
                        {
                            string[] arr_hawb = item.Split('|');

                            dokumen.Add(new DokumenTpb()
                            {
                                idDokumen = "1",
                                kodeDokumen = "740",
                                nomorDokumen = arr_hawb[0],
                                seriDokumen = 2,
                                tanggalDokumen = GetDateJson(arr_hawb[1])
                            });
                        }
                    }

                    if (list_mawb.Count > 0)
                    {
                        foreach (var item in list_mawb)
                        {
                            string[] arr_mawb = item.Split('|');

                            dokumen.Add(new DokumenTpb()
                            {
                                idDokumen = "1",
                                kodeDokumen = "741",
                                nomorDokumen = arr_mawb[0],
                                seriDokumen = 3,
                                tanggalDokumen = GetDateJson(arr_mawb[1])
                            });
                        }
                    }

                    if (list_others.Count > 0)
                    {
                        int seri_dokumen = 3;
                        foreach (var item in list_others)
                        {
                            seri_dokumen += 1;
                            string[] arr_others = item.Split('|');

                            dokumen.Add(new DokumenTpb()
                            {
                                idDokumen = "1",
                                kodeDokumen = arr_others[2],
                                nomorDokumen = arr_others[0],
                                seriDokumen = seri_dokumen,
                                tanggalDokumen = GetDateJson(arr_others[1])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Err Func GetDokumen : {ex.Message}");
            }
            return dokumen;
        }
        private List<PengangkutTpb> GetPengangkut(string HAWB, string TGL_HAWB)
        {
            List<PengangkutTpb> pengangkut = new List<PengangkutTpb>();
            try
            {
                string queryPengangkut = $@"select
                                        kodeBendera, namaPengangkut, nomorPengangkut,
                                        kodeCaraAngkut, seriPengangkut
                                        from dhl_cs_tpb_pengangkut
                                        where HAWB = '{HAWB}' and 
                                        TGL_HAWB = '{TGL_HAWB}'";
                DataTable tblPengangkut = DbOfficialCeisa.getRecords(queryPengangkut);
                if (tblPengangkut.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblPengangkut.Rows)
                    {
                        pengangkut.Add(new PengangkutTpb()
                        {
                            kodeBendera = GetKodeBendera(dr["namaPengangkut"].ToString()),
                            namaPengangkut = dr["namaPengangkut"].ToString(),
                            nomorPengangkut = dr["nomorPengangkut"].ToString(),
                            kodeCaraAngkut = dr["kodeCaraAngkut"].ToString(),
                            seriPengangkut = Convert.ToInt32(dr["seriPengangkut"])
                        });
                    }
                }
            }
            catch { }
            return pengangkut;
        }
        #endregion

        #region complete Early Draft TPB
        protected void btnCompleteDraft_Click(object sender, EventArgs e)
        {
            try
            {
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

                List<string> listHAWB = new List<string>();
                if (ViewState["isChkAll"] != null)
                {
                    string isChkAll = ViewState["isChkAll"].ToString();

                    if (isChkAll == "false")
                    {
                        foreach (GridViewRow row in GV_DataDCI.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                bool isChecked = row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked;
                                if (!isChecked) continue;

                                string NO_AJU = GV_DataDCI.DataKeys[row.RowIndex].Values[2].ToString();
                                if (string.IsNullOrEmpty(NO_AJU) == false) continue;

                                string HAWB = GV_DataDCI.DataKeys[row.RowIndex].Values[0].ToString();
                                string TGL_HAWB = GV_DataDCI.DataKeys[row.RowIndex].Values[1].ToString();
                                listHAWB.Add($"{HAWB}|{TGL_HAWB}");
                            }
                        }
                    }
                    else
                    {
                        DataTable dt = (DataTable)ViewState["ListDataDCI"];
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                string NO_AJU = row["No Aju"].ToString();
                                if (string.IsNullOrEmpty(NO_AJU) == false) continue;

                                string HAWB = row["HAWB"].ToString();
                                string TGL_HAWB = row["Tgl HAWB"].ToString();
                                listHAWB.Add($"{HAWB}|{TGL_HAWB}");
                            }
                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('No rows selected.');", true);
                    return;
                }


                if (listHAWB.Count > 0)
                {
                    //int countRecord = 0;
                    List<string> listSuccessDok = new List<string>();
                    List<string> listSuccessHeader = new List<string>();
                    List<string> listSuccessPengangkut = new List<string>();
                    foreach (var item in listHAWB)
                    {
                        List<string> listQuery = new List<string>();
                        string[] arr = item.Split('|');
                        //countRecord += SendData(dr[0], dr[1], GetAccessToken(), GetIdToken(), gateway);

                        #region Complete Draft Data Dokumen TPB
                        string queryDokumen = $@"delete from dhl_cs_tpb_dokumen where hawb in ('{arr[0]}')";
                        if (DbOfficialCeisa.runCommand(queryDokumen) == 0)
                        {
                            logger.Log($"Clean data dokumen HAWB : {arr[0]} Complete.. [Complete Draft TPB]");
                        }
                        else
                        {
                            logger.Log($"Clean data dokumen HAWB : {arr[0]} Not Found.. [Complete Draft TPB]");
                        }

                        string query = $@"select top 1
                                        b.HAWB,
                                        try_convert(date,ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB),105) as [TGLHAWB],
                                        '740|741|380' as [kodeDokumen],
                                        concat(b.HAWB,'|',isnull(dt.NOMASTERBLAWB,b.MAWB),'|',iif(isnull(b.NOINVOICE,'NN')='','NN',isnull(b.NOINVOICE,'NN'))) as [nomorDokumen],
                                        concat(b.TGLHAWB,'|',try_convert(date,ISNULL(dt.TGLMASTERBLAWB,b.TGLHAWB),105),'|',b.InvoiceDate) as [tanggalDokumen]
                                        from DETIL dt 
                                        left join BILLINGREPORT b on b.HAWB = dt.NOHOSTBLAWB
                                        where GATEWAY = '{gateway}' and
                                        b.HAWB in ('{arr[0]}')
                                        and DATEDIFF(month, try_convert(date,dt.TGLHOSTBLAWB,105), GETDATE()) < 3
                                        and DATEDIFF(month, b.TGLHAWB, GETDATE()) < 3
                                        and LEN(dt.NOMASTERBLAWB) = 11
                                        order by HAWB";
                        DataRow row = DbOfficialDCI.getRow(query);
                        if (row != null)
                        {
                            string[] arr_kode = row["kodeDokumen"].ToString().Split('|');
                            string kode_740 = arr_kode[0];
                            string kode_741 = arr_kode[1];
                            string kode_380 = arr_kode[2];

                            string[] arr_no_dokumen = row["nomorDokumen"].ToString().Split('|');
                            string nomor_740 = arr_no_dokumen[0];
                            string nomor_741 = arr_no_dokumen[1];
                            string nomor_380 = arr_no_dokumen[2];

                            string[] arr_tgl_dokumen = row["tanggalDokumen"].ToString().Split('|');
                            string tgl_740 = arr_tgl_dokumen[0];
                            string tgl_741 = arr_tgl_dokumen[1];
                            string tgl_380 = arr_tgl_dokumen[2];

                            for (int i = 1; i <= 3; i++)
                            {
                                switch (i)
                                {
                                    case 1://kode 380
                                        listQuery.Add($@"insert into dhl_cs_tpb_dokumen values (
                                        '{row["HAWB"]}', '{Convert.ToDateTime(row["TGLHAWB"]).ToString("yyyy-MM-dd")}', '1',
                                        '{kode_380}', 
                                        '{nomor_380}', '{i}',
                                        '{tgl_380}'
                                        )");
                                        break;
                                    case 2://kode 740
                                        listQuery.Add($@"insert into dhl_cs_tpb_dokumen values (
                                        '{row["HAWB"]}', '{Convert.ToDateTime(row["TGLHAWB"]).ToString("yyyy-MM-dd")}', '1',
                                        '{kode_740}',
                                        '{nomor_740}', '{i}',
                                        '{tgl_740}'
                                        )");
                                        break;
                                    default://kode 741
                                        listQuery.Add($@"insert into dhl_cs_tpb_dokumen values (
                                        '{row["HAWB"]}', '{Convert.ToDateTime(row["TGLHAWB"]).ToString("yyyy-MM-dd")}', '1',
                                        '{kode_741}',
                                        '{nomor_741}', '{i}',
                                        '{tgl_741}'
                                        )");
                                        break;
                                }
                            }
                        }
                        else
                        {
                            logger.Log($"Query data dokumen HAWB : {arr[0]} Not Found.. `CHECK TABLE DETIL COLUMN NOHOSTBLAWB & NOMASTERBLAWB` [Complete Draft TPB]");
                        }

                        if (listQuery.Count > 0)
                        {
                            if (DbOfficialCeisa.runCommand(listQuery) == 0)
                            {
                                logger.Log($"Complete data dokumen HAWB : {arr[0]} Complete.. [Complete Draft TPB]");
                                listSuccessDok.Add($"{arr[0]}");
                            }
                        }
                        #endregion

                        #region Complete Draft Data Header TPB
                        query = $@"select top 1 
                                isnull(hd.NOBC11,'') as [nomorBc11],
                                dt.NOPOS as [posBc11],
                                dt.NOSUBPOS as [subPosBc11],
                                try_convert(date,isnull(hd.TGLBC11,'1900-01-01'),105) as [tanggalBc11],
                                try_convert(date,isnull(hd.TGLTIBA,'1900-01-01'),105) as [tanggalTiba]
                                from DETIL dt
                                left join HEADER hd on dt.NOMASTERBLAWB = hd.MAWB
                                where 
                                NOHOSTBLAWB in ('{arr[0]}') and
                                LEN(dt.NOMASTERBLAWB) = 11
                                order by TGLHOSTBLAWB desc";
                        row = DbOfficialDCI.getRow(query);
                        if (row != null)
                        {
                            string update_header = $@"update dhl_cs_tpb_header set 
                                                    nomorBc11 = '{row["nomorBc11"]}',
                                                    posBc11 = '{row["posBc11"]}',
                                                    subPosBc11 = '{row["subPosBc11"]}',
                                                    tanggalBc11 = '{row["tanggalBc11"]}',
                                                    tanggalTiba = '{row["tanggalTiba"]}'
                                                    where HAWB = '{arr[0]}' and TGL_HAWB = '{arr[1]}'";

                            if (DbOfficialCeisa.runCommand(update_header) == 0)
                            {
                                logger.Log($"Complete data header HAWB : {arr[0]} Complete.. [Complete Draft TPB]");
                                listSuccessHeader.Add($"{arr[0]}");
                            }
                        }
                        else
                        {
                            logger.Log($"Query data header HAWB : {arr[0]} Not Found.. `CHECK TABLE DETIL COLUMN NOHOSTBLAWB & NOMASTERBLAWB` [Complete Draft TPB]");
                        }
                        #endregion

                        #region Complete Draft Data Pengangkut TPB
                        query = $@"select
                                NOHOSTBLAWB,
                                try_convert(date,dt.TGLHOSTBLAWB,105) as [TGLHAWB],
                                np.bendera as [kodeBendera],
                                np.nama_pesawat as [namaPengangkut],
                                NOIMO as [nomorPengangkut],
                                KODEMODA as [kodeCaraAngkut],
                                1 as [seriPengangkut]
                                from HEADER h
                                left join DETIL dt on h.MAWB = dt.NOMASTERBLAWB
                                left join NAMAPESAWAT np on left(NOIMO,2) = np.kode
                                where NOHOSTBLAWB in ('{arr[0]}') and np.bendera is not null and NOMASTERBLAWB not like '%alert%'";
                        row = DbOfficialDCI.getRow(query);
                        if (row != null)
                        {
                            string update_pengangkut = $@"update dhl_cs_tpb_pengangkut set
                                                        kodeBendera = '{row["kodeBendera"]}',
                                                        namaPengangkut = '{row["namaPengangkut"]}',
                                                        nomorPengangkut = '{row["nomorPengangkut"]}'
                                                        where HAWB = '{arr[0]}' and TGL_HAWB = '{arr[1]}'";
                            if (DbOfficialCeisa.runCommand(update_pengangkut) == 0)
                            {
                                logger.Log($"Complete data pengangkut HAWB : {arr[0]} Complete.. [Complete Draft TPB]");
                                listSuccessPengangkut.Add($"{arr[0]}");
                            }
                        }
                        #endregion

                    }

                    if (listSuccessDok.Count > 0 || listSuccessHeader.Count > 0 || listSuccessPengangkut.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Complete draft success.. Header[{listSuccessHeader.Count}] Dokumen[{listSuccessDok.Count}] Pengangkut[{listSuccessPengangkut.Count}]');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Complete draft failed.. Header[{listSuccessHeader.Count}] Dokumen[{listSuccessDok.Count}] Pengangkut[{listSuccessPengangkut.Count}]');", true);
                    }

                    LoadSummary();
                    InitDropdown();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('No rows selected.');", true);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        #endregion

        #region load data DCI
        protected void btnLoadDCI_Click(object sender, EventArgs e)
        {
            ClearFormDCI();
            ShowModalDCI();
        }
        protected void btnSaveHAWB_Click(object sender, EventArgs e)
        {
            try
            {
                ViewState["ListHAWB"] = null;

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

                //check empty input field
                if (string.IsNullOrEmpty(inputHAWB.Text))
                {
                    ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "alert", $"alert('Please input HAWB number');", true);
                    return;
                }

                List<string> listHAWB = new List<string>();

                string[] arr = inputHAWB.Text.Trim().Split('\n');
                foreach (var item in arr)
                {
                    listHAWB.Add($"'{item.Trim()}'");
                }

                if (listHAWB.Count > 50)
                {
                    ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "alert", $"alert('Maximum input HAWB 50');", true);
                    return;
                }

                ViewState["ListHAWB"] = listHAWB;
                string hawb = String.Join(",", listHAWB);

                // checking request reload
                if (isReload.Checked == true)
                {
                    CleanDuplicate(listHAWB);
                }
                else
                {
                    //checking duplicate
                    hawb = CheckDuplicate(hawb, listHAWB);
                }

                bool is_draft = false;
                if (isDraft.Checked == true) is_draft = true;

                int result = 0;
                if (string.IsNullOrEmpty(hawb) == false)
                    result = LoadDataDCI(hawb, gateway, update_by, is_draft);
                else
                    result = -1;

                if (result > 0)
                    ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "alert", $"alert('Load data success');", true);
                else if (result == 0)
                    ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "alert", $"alert('Load data failed, hawb not found');", true);
                else
                    ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "alert", $"alert('Load data failed, hawb already created');", true);

                CloseModalLoader();
                CloseModalDCI();
                LoadSummary();
                InitDropdown();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        private string CheckDuplicate(string hawb, List<string> listHAWB)
        {
            string retval = hawb;
            try
            {
                string query = $@"select hawb from dhl_cs_tpb_header where hawb in ({hawb})";
                DataTable dt = DbOfficialCeisa.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        listHAWB.Remove($"'{dr["hawb"]}'");
                    }

                    if (listHAWB.Count > 0)
                        retval = String.Join(",", listHAWB);
                    else
                        retval = string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return retval;
        }
        private void CleanDuplicate(List<string> listHAWB)
        {
            try
            {
                string queryHeader = $@"delete from dhl_cs_tpb_header where hawb in";
                string queryBrg = $@"delete from dhl_cs_tpb_barang where hawb in";
                string queryBrgTarif = $@"delete from dhl_cs_tpb_barang_tarif where hawb in";
                string queryKemasan = $@"delete from dhl_cs_tpb_kemasan where hawb in";
                string queryEntitas = $@"delete from dhl_cs_tpb_entitas where hawb in";
                string queryDokumen = $@"delete from dhl_cs_tpb_dokumen where hawb in";
                string queryPengangkut = $@"delete from dhl_cs_tpb_pengangkut where hawb in";
                string queryStatusXML = $@"delete from dhl_cs_tpb_status_xml where hawb in";
                string queryProfect = $@"delete from dhl_cs_tpb_profect where hawb in";

                string queryResponData = $@"delete from dhl_cs_response_data where hawb in";
                string queryResponHeader = $@"delete from dhl_cs_response_header where hawb in";
                string queryResponStatus = $@"delete from dhl_cs_response_status where hawb in";
                string queryResponPesan = $@"delete from dhl_cs_response_pesan where hawb in";

                List<string> listDelete = new List<string>();
                if (listHAWB.Count > 0)
                {
                    foreach (var item in listHAWB)
                    {
                        listDelete.Add($"{queryHeader} ({item})");
                        listDelete.Add($"{queryBrg} ({item})");
                        listDelete.Add($"{queryBrgTarif} ({item})");
                        listDelete.Add($"{queryKemasan} ({item})");
                        listDelete.Add($"{queryEntitas} ({item})");
                        listDelete.Add($"{queryDokumen} ({item})");
                        listDelete.Add($"{queryPengangkut} ({item})");
                        listDelete.Add($"{queryStatusXML} ({item})");
                        listDelete.Add($"{queryProfect} ({item})");
                        listDelete.Add($"{queryResponData} ({item})");
                        listDelete.Add($"{queryResponHeader} ({item})");
                        listDelete.Add($"{queryResponStatus} ({item})");
                        listDelete.Add($"{queryResponPesan} ({item})");
                    }
                }

                if (DbOfficialCeisa.runCommand(listDelete) == 0)
                    logger.Log($"Reload data DCI..\nClean duplicate AWB :\n{String.Join("\n", listHAWB)}");
            }
            catch (Exception ex)
            { logger.Log(ex.Message); }
        }
        private int LoadDataDCI(string hawb, string gateway, string update_by, bool is_draft)
        {
            int retval = 0;
            try
            {
                DataTable dt = new DataTable();
                List<string> listQuery = new List<string>();
                string query = string.Empty;
                string ndpbm_global = string.Empty;

                #region load setting [kode kantor, kode gudang]
                //query = $@"select kode_kantor, kode_gudang from dhl_cs_sequence_aju where gateway = '{gateway}'";
                //DataRow rowSetting = DbOfficialCeisa.getRow(query);

                //query = $"select username, jabatan, kota_ttd, nama_ttd from dhl_cs_settings where gateway = '{gateway}'";
                //DataRow rowHeader = DbOfficialCeisa.getRow(query);
                #endregion

                #region load data barang & header
                query = $@"select 
                            db.HAWB,
                            --try_convert(date,ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB),105) as [TGLHAWB],
                            db.TGLHAWB,
                            db.SHORT as [idBarang],
                            case 
	                            when db.ASURANSI <= 0 then b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)
	                            when db.ASURANSI = b.ASURANSI then b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)
	                            else db.ASURANSI
                            end as [asuransi],
                            case 
	                            when db.ASURANSI <= 0 and db.FOB <= 0 and db.FREIGHT <= 0 then	(b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + 
																	                            (b.FREIGHT/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + 
																	                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))
	                            when db.ASURANSI <= 0 and db.FOB <= 0 then	(b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + db.FREIGHT + 
												                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))
	                            when db.ASURANSI <= 0 then	db.FOB + db.FREIGHT + 
								                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))
	                            when db.ASURANSI = b.ASURANSI and db.FOB = b.FOBVAL and db.FREIGHT = b.FREIGHT then	(b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + 
																						                            (b.FREIGHT/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + 
																						                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))
	                            when db.ASURANSI = b.ASURANSI and db.FOB = b.FOBVAL then(b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + db.FREIGHT + 
															                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))
	                            when db.ASURANSI = b.ASURANSI then	db.FOB + db.FREIGHT + 
										                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))
	                            else db.FOB + db.FREIGHT + db.ASURANSI
                            end as [cif],
                            0 as [diskon],
                            case 
	                            when db.FOB <= 0 then b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)
	                            when db.FOB = b.FOBVAL then b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)
	                            else db.FOB
                            end as [fob],
                            case
	                            when db.FREIGHT <= 0 then b.FREIGHT/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)
	                            when db.FREIGHT = b.FREIGHT then b.FREIGHT/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)
	                            else db.FREIGHT
                            end as [freight],
                            0 as [hargaEkspor],
                            0 as [hargaPenyerahan],
                            case
	                            when db.FOB <= 0 then (b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) / db.QTY
	                            when db.FOB = b.FOBVAL then (b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) / db.QTY
	                            else db.FOB / db.QTY
                            end as [hargaSatuan],
                            0 as [isiPerKemasan],
                            1 as [jumlahKemasan],
                            db.QTY as [jumlahSatuan],
                            '' as [kodeBarang], --kodeBarang
                            '23' as [kodeDokumen],
                            --'14' as [kodeKategoriBarang],
                            '11' as [kodeKategoriBarang],
                            case 
	                            when left(PKTTYPE,2) = '' then 'PK' 
	                            else left(PKTTYPE,2) 
                            end as [kodeJenisKemasan],
                            db.KODENEGARAASAL as [kodeNegaraAsal],
                            '0' as [kodePerhitungan],
                            db.SATUAN as [kodeSatuanBarang],
                            '-' as [merk],
                            isnull(db.NetWeight,(QTY1/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) * 0.9) as [netto],
                            case
	                            when b.OriValuta != 'IDR' then case 
								                            when db.FOB <= 0 then b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)
								                            when db.FOB = b.FOBVAL then b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)
								                            else db.FOB end
	                            else case 
		                             when db.FOB <= 0 then (b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) * b.NILAICURRENCY 
		                             when db.FOB = b.FOBVAL then (b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) * b.NILAICURRENCY
		                             else db.FOB * b.NILAICURRENCY end
                            end as [nilaiBarang],
                            0 as [nilaiTambah],
                            db.HSCODE as [posTarif],
                            db.SHORT as [seriBarang],
                            '' as [spesifikasiLain],
                            '-' as [tipe],
                            '' as [ukuran],
                            db.NAMABARANG as [uraian],
                            b.NILAICURRENCY as [ndpbm],
                            case 
	                            when db.ASURANSI <= 0 and db.FOB <= 0 and db.FREIGHT <= 0 then	((b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + 
																	                            (b.FREIGHT/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + 
																	                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))) * b.NILAICURRENCY
	                            when db.ASURANSI <= 0 and db.FOB <= 0 then	((b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + db.FREIGHT + 
												                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))) * b.NILAICURRENCY
	                            when db.ASURANSI <= 0 then	(db.FOB + db.FREIGHT + 
								                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))) * b.NILAICURRENCY
	                            when db.ASURANSI = b.ASURANSI and db.FOB = b.FOBVAL and db.FREIGHT = b.FREIGHT then	((b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + 
																	                            (b.FREIGHT/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + 
																	                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))) * b.NILAICURRENCY
	                            when db.ASURANSI = b.ASURANSI and db.FOB = b.FOBVAL then((b.FOBVAL/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) + db.FREIGHT + 
															                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))) * b.NILAICURRENCY
	                            when db.ASURANSI = b.ASURANSI then	(db.FOB + db.FREIGHT + 
										                            (b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB))) * b.NILAICURRENCY
	                            else (db.FOB + db.FREIGHT + db.ASURANSI) * b.NILAICURRENCY
                            end as [cifRupiah],
                            0 as [hargaPerolehan],
                            '0' as [kodeAsalBahanBaku],
                            isnull(InvCrncyCd,'USD') as [valuta] --GET KURS FROM BEACUKAI
                            from DETAILBARANGBILLINGREPORT db
                            left join BILLINGREPORT b on db.HAWB = b.HAWB
                            --left join DETIL dt on db.HAWB = dt.NOHOSTBLAWB
                            where GATEWAY = '{gateway}' and
                            db.HAWB in ({hawb})
                            order by db.HAWB,seriBarang";

                List<string> listAWB = new List<string>();
                if (ViewState["ListHAWB"] != null)
                {
                    listAWB = (List<string>)ViewState["ListHAWB"];
                }
                foreach (var item in listAWB)
                {
                    #region load data barang
                    string totalNetto = string.Empty;
                    double totalCif = 0;
                    double totalFreight = 0;
                    double totalFob = 0;
                    double totalAsuransi = 0;
                    string ndpbm = string.Empty;
                    string query_v2 = $@"select 
                                    db.HAWB,
                                    db.TGLHAWB as [TGLHAWB],
                                    db.SHORT as [idBarang],
                                    case
	                                    when db.ASURANSI <= 0 then b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)
	                                    when db.ASURANSI = b.ASURANSI then b.ASURANSI/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)
	                                    else db.ASURANSI
                                    end as [asuransi], --RECALCULATE BASED ON VALUTA (on default USD)
                                    db.TOTALFOB as [cif], --RECALCULATE BASED ON VALUTA (on default USD)
                                    0 as [diskon], 
                                    db.FOB as [fob], --RECALCULATE BASED ON VALUTA (on default USD)
                                    db.FREIGHT as [freight], --RECALCULATE BASED ON VALUTA (on default USD)
                                    0 as [hargaEkspor],
                                    0 as [hargaPenyerahan],
                                    db.FOB as [hargaSatuan],
                                    0 as [isiPerKemasan],
                                    ISNULL(b.TOTALPKT,'1') as [jumlahKemasan],
                                    db.QTY as [jumlahSatuan],
                                    '' as [kodeBarang],
                                    '23' as [kodeDokumen],
                                    '11' as [kodeKategoriBarang],
                                    case 
	                                    when left(PKTTYPE,2) = '' then 'PK' 
	                                    else left(PKTTYPE,2) 
                                    end as [kodeJenisKemasan],
                                    db.KODENEGARAASAL as [kodeNegaraAsal],
                                    '0' as [kodePerhitungan],
                                    db.SATUAN as [kodeSatuanBarang],
                                    '-' as [merk],
                                    isnull(db.NetWeight,(QTY1/(select count(*) as total from DETAILBARANGBILLINGREPORT where hawb=db.HAWB)) * 0.9) as [netto],
                                    db.FOB as [nilaiBarang],
                                    0 as [nilaiTambah],
                                    db.HSCODE as [posTarif],
                                    db.SHORT as [seriBarang],
                                    '' as [spesifikasiLain],
                                    '-' as [tipe],
                                    '' as [ukuran],
                                    db.NAMABARANG as [uraian],
                                    b.NILAICURRENCY as [ndpbm], --GET NDPBM FROM BEACUKAI (on default USD)
                                    (db.FOB + db.FREIGHT + db.ASURANSI) * b.NILAICURRENCY as [cifRupiah], --GET CIF RUPIAH (on default USD to IDR)
                                    0 as [hargaPerolehan],
                                    '0' as [kodeAsalBahanBaku],
                                    isnull(OriValuta,'USD') as [valuta] --GET KURS FROM BEACUKAI
                                    from DETAILBARANGBILLINGREPORT db
                                    left join BILLINGREPORT b on db.HAWB = b.HAWB
                                    --left join DETIL dt on db.HAWB = dt.NOHOSTBLAWB
                                    where GATEWAY = '{gateway}' and
                                    db.HAWB in ({item}) and
                                    DATEDIFF(month, db.TGLHAWB, GETDATE()) < 3
                                    order by db.HAWB,seriBarang";
                    dt = DbOfficialDCI.getRecords(query_v2);
                    if (dt.Rows.Count > 0)
                    {
                        logger.Log($"LOAD DATA BARANG {hawb} : {dt.Rows.Count} Rows..");
                        double netto = 0;
                        string valuta = string.Empty;
                        string cif = string.Empty;
                        string asuransi = string.Empty;
                        string fob = string.Empty;
                        string freight = string.Empty;

                        //get user password for request token
                        //query = $@"select username, password from dhl_cs_settings where gateway = '{gateway}'";
                        //string userCeisa = string.Empty;
                        //string passwordCeisa = string.Empty;
                        //DataRow row = DbOfficialCeisa.getRow(query);
                        //if (row == null)
                        //{
                        //    userCeisa = "asaepudin";
                        //    passwordCeisa = "Nadia@1234";
                        //}
                        //else
                        //{
                        //    userCeisa = row["username"].ToString();
                        //    passwordCeisa = row["password"].ToString();
                        //}

                        //ResponseToken response = JsonConvert.DeserializeObject<ResponseToken>(GetRequestToken(userCeisa, passwordCeisa));
                        //if (response.status != "success")
                        //{
                        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{response.message}');", true);
                        //    return retval;
                        //}

                        int jml_kemasan = 0; //diambil hanya row pertama saja
                        bool isInsertedJmlKemasan = false;
                        foreach (DataRow dr in dt.Rows)
                        {
                            netto += Convert.ToDouble(dr["netto"]);
                            valuta = dr["valuta"].ToString();

#if DEBUG
                            if (valuta == "USD" || valuta == "IDR")
                            {
                                ndpbm = $"{dr["ndpbm"]}";
                                cif = $"{dr["cif"]}";
                                asuransi = $"{dr["asuransi"]}";
                                fob = $"{dr["fob"]}";
                                freight = $"{dr["freight"]}";
                            }
                            else
                            {
                                ndpbm = GetKurs(valuta, GetAccessToken(), $"{dr["ndpbm"]}");
                                asuransi = GetValCurrFinal(ndpbm, $"{dr["ndpbm"]}", $"{dr["asuransi"]}");
                                fob = $"{dr["fob"]}";
                                freight = GetValCurrFinal(ndpbm, $"{dr["ndpbm"]}", $"{dr["freight"]}");
                                cif = $"{Math.Round(Convert.ToDouble(asuransi) + Convert.ToDouble(fob) + Convert.ToDouble(freight), 2, MidpointRounding.AwayFromZero)}";
                            }
#else
                            ndpbm = $"{dr["ndpbm"]}";
                            cif = $"{dr["cif"]}";
                            asuransi = $"{dr["asuransi"]}";
                            fob = $"{dr["fob"]}";
                            freight = $"{dr["freight"]}";
#endif

                            totalCif += Convert.ToDouble(cif);
                            totalAsuransi += Convert.ToDouble(asuransi);
                            totalFreight += Convert.ToDouble(freight);
                            totalFob += Convert.ToDouble(fob);

                            if (isInsertedJmlKemasan == false)
                                jml_kemasan = Convert.ToInt32(dr["jumlahKemasan"]);
                            else
                                jml_kemasan = 0;

                            listQuery.Add($@"insert into dhl_cs_tpb_barang values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{dr["idBarang"]}',
                                        '{asuransi}',                                        
                                        '{cif}',
                                        '{dr["cifRupiah"]}',
                                        '{dr["diskon"]}',
                                        '{fob}',
                                        '{freight}',
                                        '{dr["hargaEkspor"]}',
                                        '{dr["hargaPenyerahan"]}',
                                        '{fob}',
                                        '{dr["isiPerKemasan"]}',
                                        '{jml_kemasan}',
                                        '{dr["jumlahSatuan"]}',
                                        '{dr["kodeBarang"]}',
                                        '{dr["kodeDokumen"]}',
                                        '{dr["kodeKategoriBarang"]}',
                                        '{dr["kodeJenisKemasan"]}',
                                        '{dr["kodeNegaraAsal"].ToString().Trim()}',
                                        '{dr["kodePerhitungan"]}',
                                        '{dr["kodeSatuanBarang"]}',
                                        '{dr["merk"]}',
                                        '{ndpbm}',
                                        '{dr["netto"]}',
                                        '{fob}',
                                        '{dr["nilaiTambah"]}',
                                        '{dr["posTarif"]}',
                                        '{dr["seriBarang"]}',
                                        '{dr["spesifikasiLain"]}',
                                        '{dr["tipe"]}',
                                        '{dr["ukuran"]}',
                                        '{dr["uraian"].ToString().Replace("'", "`")}',
                                        '{dr["hargaPerolehan"]}',
                                        '{dr["kodeAsalBahanBaku"]}'
                                        )");

                            isInsertedJmlKemasan = true;
                        }

                        totalNetto = $"{netto}";
                        ndpbm_global = $"{ndpbm}";
                        //if (DbOfficialCeisa.runCommand(listQuery) == 0)
                        //    retval = listQuery.Count;
                    }
                    #endregion

                    #region load data header
                    query = $@"select top 1 
                            GATEWAY as [gateway],
                            b.HAWB,
                            try_convert(date,ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB),105) as [TGLHAWB],
                            --b.TGLHAWB,
                            'S' as [asalData],
                            '{totalAsuransi}' as [asuransi],
                            QTY1 as [bruto],
                            --FOBVAL + FREIGHT + ASURANSI as [cif],
                            '{totalCif}' as [cif],
                            '{totalFob}' as [fob],
                            '{totalFreight}' as [freight],
                            0 as [hargaPenyerahan],
                            k.JABATANPEMOHONTTDTPB as [jabatanTtd],
                            0 as [jumlahKontainer],
                            'LN' as [kodeAsuransi],
                            '23' as [kodeDokumen],
                            INCOTERM as [kodeIncoterm],
                            b.KANTORPENGAWAS as [kodeKantor],
                            k.KPBC_BONGKAR as [kodeKantorBongkar],
                            k.AIRPORTCODE as [kodePelBongkar],
                            KODENEGARAPENGIRIM + SHPORIGIN as [kodePelMuat],
                            KODEPELABUHANMUAT as [kodePelTransit],
                            k.KODEGUDANG as [kodeTps],
                            '1' as [kodeTujuanTpb],
                            '11' as [kodeTutupPu],
                            b.OriValuta as [kodeValuta],
                            k.KOTATTDTPB as [kotaTtd],
                            k.PEMOHONTTDTPB as [namaTtd],
                            '{ndpbm}' as [ndpbm],
                            '{totalNetto}' as [netto],
                            '0018/KA-A5/111 TAHUN 2019' as [nik],
                            case 
	                            when b.OriValuta != 'IDR' then '{totalFob}'
	                            else NILAICIFRP
                            end as [nilaiBarang],
                            ISNULL(s.NOMORAJU,'') as [nomorAju],
                            isnull(hd.NOBC11,'') as [nomorBc11],
                            dt.NOPOS as [posBc11],
                            0 as seri,
                            dt.NOSUBPOS as [subPosBc11],
                            try_convert(date,isnull(hd.TGLBC11,'1900-01-01'),105) as [tanggalBc11],
                            try_convert(date,isnull(hd.TGLTIBA,'1900-01-01'),105) as [tanggalTiba],
                            convert(varchar(10),getdate(),120) as [tanggalTtd],
                            0 as [biayaTambahan],
                            0 as [biayaPengurang],
                            '1' as [kodeKenaPajak]
                            from BILLINGREPORT b
                            left join SPPB s on b.HAWB = s.HAWB and b.TGLHAWB = s.TGLHAWB 
                            left join DETIL dt on b.HAWB = NOHOSTBLAWB
                            left join HEADER hd on dt.NOMASTERBLAWB = hd.MAWB
                            --left join HEADER hd on b.MAWB = hd.MAWB
                            left join KBPCBONGKAR k on b.GATEWAY = k.GTWCODE
                            where GATEWAY = '{gateway}' and
                            b.HAWB in ({item})
                            order by TGLHAWB desc";
                    dt = DbOfficialDCI.getRecords(query);
                    if (dt.Rows.Count > 0)
                    {
                        logger.Log($"LOAD DATA HEADER {hawb} : {dt.Rows.Count} Rows..");
                        foreach (DataRow dr in dt.Rows)
                        {
                            listQuery.Add($@"insert into dhl_cs_tpb_header values (
                                        '{dr["gateway"].ToString().Trim()}',
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{dr["asalData"]}',
                                        '{dr["asuransi"]}',
                                        '{dr["bruto"]}',
                                        '{dr["cif"]}',
                                        '{dr["fob"]}',
                                        '{dr["freight"]}',
                                        '{dr["hargaPenyerahan"]}',
                                        '{dr["jabatanTtd"]}',
                                        '{dr["jumlahKontainer"]}',
                                        '{dr["kodeAsuransi"]}',
                                        '{dr["kodeDokumen"]}',
                                        '{dr["kodeIncoterm"]}',
                                        '{dr["kodeKantor"]}',
                                        '{dr["kodeKantorBongkar"]}',
                                        '{dr["kodePelBongkar"]}',
                                        '{dr["kodePelMuat"]}',
                                        '{dr["kodePelTransit"]}',
                                        '{dr["kodeTps"]}',
                                        '{dr["kodeTujuanTpb"]}',
                                        '{dr["kodeTutupPu"]}',
                                        '{dr["kodeValuta"]}',
                                        '{dr["kotaTtd"]}',
                                        '{dr["namaTtd"]}',
                                        '{dr["ndpbm"]}',
                                        '{dr["netto"]}',
                                        '{dr["nik"]}',
                                        '{dr["nilaiBarang"]}',
                                        '{dr["nomorAju"]}',                                      
                                        '{dr["nomorBc11"]}',
                                        '{dr["posBc11"]}',
                                        '{dr["seri"]}',
                                        '{dr["subPosBc11"]}',
                                        '{dr["tanggalBc11"]}',
                                        '{dr["tanggalTiba"]}',
                                        '{dr["tanggalTtd"]}',
                                        '{dr["biayaTambahan"]}',
                                        '{dr["biayaPengurang"]}',
                                        '{dr["kodeKenaPajak"]}',
                                        '{update_by}',
                                        '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff")}'
                                        )");
                        }

                        //if (DbOfficialCeisa.runCommand(listQuery) == 0)
                        //    retval = listQuery.Count;
                    }
                    #endregion 
                }
                #endregion

                #region load data tarif
                query = $@"select
                        db.HAWB,
                        --try_convert(date,ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB),105) as [TGLHAWB],
                        db.TGLHAWB,
                        '1' as [kodeJenisTarif],
                        db.QTY as [jumlahSatuan],
                        '3' as [kodeFasilitasTarif],
                        db.SATUAN as[kodeSatuanBarang],
                        'BM|CUKAI|PPN|PPNBM|PPH' as [kodeJenisPungutan],
                        concat(db.BMITEMS ,'|',db.CUKAIITEMS,'|',db.PPNITEMS,'|',db.PPNBMITEMS,'|',db.PPHITEMS) as [nilaiBayar],
                        0 as [nilaiFasilitas],
                        concat(db.BMITEMS ,'|',db.CUKAIITEMS,'|',db.PPNITEMS,'|',db.PPNBMITEMS,'|',db.PPHITEMS) as [nilaiSudahDilunasi],
                        db.SHORT as [seriBarang],
                        isnull((select concat(BM,',',CUKAI,',',PPN,',',PPNBM,',',PPH) from HSCODE where HS_CODE = db.HSCODE),'') as [tarif],
                        isnull(NIB_CONSIGNEE,'0') as [nibEntitas],
                        100 as [tarifFasilitas]
                        from DETAILBARANGBILLINGREPORT db
                        left join BILLINGREPORT b on db.HAWB = b.HAWB
                        --left join DETIL dt on db.HAWB = dt.NOHOSTBLAWB
                        where b.GATEWAY = '{gateway}' and
                        db.HAWB in ({hawb}) and
                        DATEDIFF(month, db.TGLHAWB, GETDATE()) < 3";
                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    logger.Log($"LOAD DATA TARIF {hawb} : {dt.Rows.Count} Rows..");
                    foreach (DataRow dr in dt.Rows)
                    {
                        string[] arr_kode = dr["kodeJenisPungutan"].ToString().Split('|');
                        string kode_bm = arr_kode[0];
                        string kode_cukai = arr_kode[1];
                        string kode_ppn = arr_kode[2];
                        string kode_ppnbm = arr_kode[3];
                        string kode_pph = arr_kode[4];

                        string[] arr_nilai = dr["nilaiBayar"].ToString().Split('|');
                        string nilai_bm = arr_nilai[0];
                        string nilai_cukai = arr_nilai[1];
                        string nilai_ppn = arr_nilai[2];
                        string nilai_ppnbm = arr_nilai[3];
                        string nilai_pph = arr_nilai[4];

                        string nib = dr["nibEntitas"].ToString();

                        string[] arr_tarif = { "" };
                        if (dr["tarif"].ToString() != "")
                            arr_tarif = dr["tarif"].ToString().Split(',');
                        string tarif_bm = "0";
                        string tarif_cukai = "0";
                        string tarif_ppn = "0";
                        string tarif_ppnbm = "0";
                        string tarif_pph = "0";
                        if (arr_tarif.Length == 6)
                        {
                            tarif_bm = arr_tarif[0];
                            tarif_cukai = arr_tarif[1];
                            tarif_ppn = arr_tarif[3];
                            tarif_ppnbm = arr_tarif[4];
                            tarif_pph = arr_tarif[5];
                        }
                        else if (arr_tarif.Length == 5)
                        {
                            tarif_bm = arr_tarif[0];
                            tarif_cukai = arr_tarif[1];
                            tarif_ppn = arr_tarif[2];
                            tarif_ppnbm = arr_tarif[3];
                            tarif_pph = arr_tarif[4];
                        }
                        else
                        {
                            logger.Log($"Tarif kosong, CEK HSCODE AWB {hawb}");
                        }

                        for (int i = 0; i < 6; i++)
                        {
                            switch (i)
                            {
                                case 1: //pph
                                    if (Convert.ToDouble(tarif_pph) == 0)
                                    {
                                        //if (nib != "0")
                                        //{
                                        //    tarif_pph = "2.5";
                                        //}
                                        //else
                                        //{
                                        //    tarif_pph = "7.5";
                                        //}
                                        tarif_pph = "2.5";
                                    }
                                    listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '6',
                                        '{dr["kodeSatuanBarang"]}',
                                        '{kode_pph}',
                                        '{nilai_pph}',
                                        '{dr["nilaiFasilitas"]}',
                                        '{nilai_pph}',
                                        '{dr["seriBarang"]}',
                                        '{tarif_pph}',
                                        '{dr["tarifFasilitas"]}'                                        
                                        )");
                                    break;
                                case 2: //ppn
                                    listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '6',
                                        '{dr["kodeSatuanBarang"]}',
                                        '{kode_ppn}',
                                        '{nilai_ppn}',
                                        '{dr["nilaiFasilitas"]}',
                                        '{nilai_ppn}',
                                        '{dr["seriBarang"]}',
                                        '{tarif_ppn}',
                                        '{dr["tarifFasilitas"]}'                                        
                                        )");
                                    break;
                                case 3: //ppnbm
                                    listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '3',
                                        '{dr["kodeSatuanBarang"]}',
                                        '{kode_ppnbm}',
                                        '{nilai_ppnbm}',
                                        '0',
                                        '{nilai_ppnbm}',
                                        '{dr["seriBarang"]}',
                                        '{tarif_ppnbm}',
                                        '0'                                        
                                        )");
                                    break;
                                case 4: //bmtp
                                    listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '3',
                                        '{dr["kodeSatuanBarang"]}',
                                        'BMTP',
                                        '0',
                                        '0',
                                        '0',
                                        '{dr["seriBarang"]}',
                                        '0',
                                        '0'                                        
                                        )");
                                    break;
                                case 5: //cukai
                                    listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '3',
                                        '{dr["kodeSatuanBarang"]}',
                                        '{kode_cukai}',
                                        '{nilai_cukai}',
                                        '0',
                                        '{nilai_cukai}',
                                        '{dr["seriBarang"]}',
                                        '{tarif_cukai}',
                                        '0'                                        
                                        )");
                                    break;
                                default: //bm
                                    listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '3',
                                        '{dr["kodeSatuanBarang"]}',
                                        '{kode_bm}',
                                        '{nilai_bm}',
                                        '{dr["nilaiFasilitas"]}',
                                        '{nilai_bm}',
                                        '{dr["seriBarang"]}',
                                        '{tarif_bm}',
                                        '{dr["tarifFasilitas"]}'                                        
                                        )");
                                    break;
                            }
                        }

                    }

                    //if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    //    retval = listQuery.Count;
                }
                #endregion

                #region load data entitas
                query = $@"select
                        HAWB,
                        --try_convert(date,ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB),105) as [TGLHAWB],
                        TGLHAWB,
                        concat(ALAMATPENERIMA,'|',ALAMATPENGIRIM,'|','MULIA BUSINESS PARK BUILDING F JL. MT. HARYONO KAV 58-60, JAKARTA') as [alamatEntitas],
                        '3|5|7|4' as [kodeEntitas],
                        '7' as [kodeJenisApi],
                        '6' as [kodeJenisIdentitas],
                        '5|1' as [kodeStatus],
                        concat(NAMAPENERIMA,'|',NAMAPENGIRIM,'|','PT. BIROTIKA SEMESTA') as [namaEntitas],
                        isnull(NIB_CONSIGNEE,'') as [nibEntitas], --NIB_CONSIGNEE
                        concat(KITASPENERIMA,'|','013107743062000') as [nomorIdentitas],
                        KODENEGARAPENGIRIM as [kodeNegara],
                        '1|2|3|4' as [seriEntitas],
                        isnull(NIK_CONSIGNEE,'') as [nomorIjinEntitas] --NIK_CONSIGNEE
                        from BILLINGREPORT b
                        --left join DETIL dt on b.HAWB = dt.NOHOSTBLAWB 
                        where GATEWAY = '{gateway}' and
                        HAWB in ({hawb})";
                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    logger.Log($"LOAD DATA ENTITAS {hawb} : {dt.Rows.Count} Rows..");
                    foreach (DataRow dr in dt.Rows)
                    {
                        string[] arr_nama_entitas = dr["namaEntitas"].ToString().Split('|');
                        string nama_penerima = arr_nama_entitas[0];
                        string nama_pengirim = arr_nama_entitas[1];
                        string nama_DHL = arr_nama_entitas[2];

                        string[] arr_alamat_entitas = dr["alamatEntitas"].ToString().Split('|');
                        string alamat_penerima = arr_alamat_entitas[0];
                        string alamat_pengirim = arr_alamat_entitas[1];
                        string alamat_DHL = arr_alamat_entitas[2];

                        string[] arr_kode = dr["kodeEntitas"].ToString().Split('|');
                        string kode_3 = arr_kode[0];
                        string kode_5 = arr_kode[1];
                        string kode_7 = arr_kode[2];
                        string kode_4 = arr_kode[3];

                        //update NPWP 16 & NITKU 22
                        string[] arr_identitas = dr["nomorIdentitas"].ToString().Split('|');
                        string kitas_penerima = arr_identitas[0];
                        string kitas_DHL = arr_identitas[1];

                        if (kitas_penerima.Length == 15)
                        {
                            kitas_penerima = $"0{kitas_penerima}000000";
                        }
                        if (kitas_DHL.Length == 15)
                        {
                            kitas_DHL = $"0{kitas_DHL}000000";
                        }

                        string[] arr_status = dr["kodeStatus"].ToString().Split('|');
                        string status_5 = arr_status[0];
                        string status_1 = arr_status[1];

                        for (int i = 1; i <= 4; i++)
                        {
                            switch (i)
                            {
                                case 1: //kode 3
                                    listQuery.Add($@"insert into dhl_cs_tpb_entitas values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{alamat_penerima}',
                                        '{kode_3}',
                                        '',
                                        '{dr["kodeJenisIdentitas"]}',
                                        '',
                                        '{nama_penerima}',
                                        '{(dr["nibEntitas"].ToString() == "" ? GetNIB(kitas_penerima, dr["nomorIjinEntitas"].ToString()) : dr["nibEntitas"].ToString())}',
                                        '{kitas_penerima}',
                                        '{dr["nomorIjinEntitas"]}',
                                        '{dr["kodeNegara"]}',
                                        '{i}',
                                        '{GetTanggalSkep(kitas_penerima, dr["nomorIjinEntitas"].ToString())}')");
                                    break;
                                case 2: //kode 5
                                    listQuery.Add($@"insert into dhl_cs_tpb_entitas values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{alamat_pengirim}',
                                        '{kode_5}',
                                        '',
                                        '',
                                        '',
                                        '{nama_pengirim}',
                                        '',
                                        '',
                                        '',
                                        '{dr["kodeNegara"]}',
                                        '{i}',
                                        '')");
                                    break;
                                case 3: //kode 7
                                    listQuery.Add($@"insert into dhl_cs_tpb_entitas values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{alamat_penerima}',
                                        '{kode_7}',
                                        '{dr["kodeJenisApi"]}',
                                        '{dr["kodeJenisIdentitas"]}',
                                        '{status_5}',
                                        '{nama_penerima}',
                                        '',
                                        '{kitas_penerima}',
                                        '{dr["nomorIjinEntitas"]}',
                                        '{dr["kodeNegara"]}',
                                        '{i}',
                                        '{GetTanggalSkep(kitas_penerima, dr["nomorIjinEntitas"].ToString())}')");
                                    break;
                                default: //kode 4
                                    listQuery.Add($@"insert into dhl_cs_tpb_entitas values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{alamat_DHL}',
                                        '{kode_4}',
                                        '',
                                        '{dr["kodeJenisIdentitas"]}',
                                        '{status_1}',
                                        '{nama_DHL}',
                                        '',
                                        '{kitas_DHL}',
                                        '',
                                        '{dr["kodeNegara"]}',
                                        '{i}',
                                        '')");
                                    break;
                            }
                        }
                    }

                    //if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    //    retval = listQuery.Count;
                }
                #endregion

                #region load data kemasan
                query = $@"select
                        HAWB,
                        --try_convert(date,ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB),105) as [TGLHAWB],
                        TGLHAWB,
                        SDPieces as [jumlahKemasan],
                        case when left(PKTTYPE,2) = '' then 'PK' else left(PKTTYPE,2) end as [kodeJenisKemasan],
                        1 as [seriKemasan],
                        'DHL' as [merkKemasan]
                        from BILLINGREPORT b
                        --left join DETIL dt on b.HAWB = dt.NOHOSTBLAWB
                        where GATEWAY = '{gateway}' and
                        HAWB in ({hawb})";
                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    logger.Log($"LOAD DATA KEMASAN {hawb} : {dt.Rows.Count} Rows..");
                    foreach (DataRow dr in dt.Rows)
                    {
                        listQuery.Add($@"insert into dhl_cs_tpb_kemasan values (
                                        '{dr["HAWB"]}',
                                        '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{dr["jumlahKemasan"]}',
                                        '{dr["kodeJenisKemasan"]}',
                                        '{dr["seriKemasan"]}',
                                        '{dr["merkKemasan"]}'
                                        )");
                    }

                    //if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    //    retval = listQuery.Count;
                }
                #endregion

                #region load data dokumen
                if (is_draft == false)
                {
                    foreach (var item in listAWB)
                    {
                        query = $@"select top 1
                            b.HAWB,
                            try_convert(date,ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB),105) as [TGLHAWB],
                            '740|741|380' as [kodeDokumen],
                            concat(b.HAWB,'|',b.MAWB,'|',iif(isnull(b.NOINVOICE,'NN')='','NN',isnull(b.NOINVOICE,'NN'))) as [nomorDokumen],
                            concat(b.TGLHAWB,'|',try_convert(date,ISNULL(dt.TGLMASTERBLAWB,b.TGLHAWB),105),'|',b.InvoiceDate) as [tanggalDokumen]
                            from BILLINGREPORT b
                            left join DETIL dt on b.HAWB = dt.NOHOSTBLAWB
                            where GATEWAY = '{gateway}' and
                            b.HAWB in ({item})
                            and DATEDIFF(month, try_convert(date,dt.TGLHOSTBLAWB,105), GETDATE()) < 3
                            and DATEDIFF(month, b.TGLHAWB, GETDATE()) < 3
                            and LEN(dt.NOMASTERBLAWB) = 11
                            order by HAWB";
                        DataRow row = DbOfficialDCI.getRow(query);
                        if (row != null)
                        {
                            string[] arr_kode = row["kodeDokumen"].ToString().Split('|');
                            string kode_740 = arr_kode[0];
                            string kode_741 = arr_kode[1];
                            string kode_380 = arr_kode[2];

                            string[] arr_no_dokumen = row["nomorDokumen"].ToString().Split('|');
                            string nomor_740 = arr_no_dokumen[0];
                            string nomor_741 = arr_no_dokumen[1];
                            string nomor_380 = arr_no_dokumen[2];

                            string[] arr_tgl_dokumen = row["tanggalDokumen"].ToString().Split('|');
                            string tgl_740 = arr_tgl_dokumen[0];
                            string tgl_741 = arr_tgl_dokumen[1];
                            string tgl_380 = arr_tgl_dokumen[2];

                            for (int i = 1; i <= 3; i++)
                            {
                                switch (i)
                                {
                                    case 1://kode 380
                                        listQuery.Add($@"insert into dhl_cs_tpb_dokumen values (
                                        '{row["HAWB"]}', '{Convert.ToDateTime(row["TGLHAWB"]).ToString("yyyy-MM-dd")}', '1',
                                        '{kode_380}', 
                                        '{nomor_380}', '{i}',
                                        '{tgl_380}'
                                        )");
                                        break;
                                    case 2://kode 740
                                        listQuery.Add($@"insert into dhl_cs_tpb_dokumen values (
                                        '{row["HAWB"]}', '{Convert.ToDateTime(row["TGLHAWB"]).ToString("yyyy-MM-dd")}', '1',
                                        '{kode_740}',
                                        '{nomor_740}', '{i}',
                                        '{tgl_740}'
                                        )");
                                        break;
                                    default://kode 741
                                        listQuery.Add($@"insert into dhl_cs_tpb_dokumen values (
                                        '{row["HAWB"]}', '{Convert.ToDateTime(row["TGLHAWB"]).ToString("yyyy-MM-dd")}', '1',
                                        '{kode_741}',
                                        '{nomor_741}', '{i}',
                                        '{tgl_741}'
                                        )");
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    logger.Log($"Skip load data dokumen..[Early Draft TPB]");
                }
                #endregion

                #region load data pengangkut
                query = $@"select
                        HAWB,
                        --try_convert(date,ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB),105) as [TGLHAWB],
                        TGLHAWB,
                        np.bendera as [kodeBendera],
                        NAMATRANSPORT as [namaPengangkut],
                        KODETRANSPORT as [nomorPengangkut],
                        NOTRANSPORT as [kodeCaraAngkut],
                        1 as [seriPengangkut]
                        from BILLINGREPORT b
                        --left join DETIL dt on b.HAWB = dt.NOHOSTBLAWB
                        left join NAMAPESAWAT np on (left(KODETRANSPORT,2) = np.kode and NAMATRANSPORT = np.nama_pesawat)
                        where GATEWAY = '{gateway}' and
                        HAWB in ({hawb})";
                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    logger.Log($"LOAD DATA PENGANGKUT {hawb} : {dt.Rows.Count} Rows..");
                    foreach (DataRow dr in dt.Rows)
                    {
                        listQuery.Add($@"insert into dhl_cs_tpb_pengangkut values (
                                        '{dr["HAWB"]}', '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{GetKodeBendera(dr["namaPengangkut"].ToString())}',
                                        '{dr["namaPengangkut"]}',
                                        '{dr["nomorPengangkut"]}',
                                        '{dr["kodeCaraAngkut"]}',
                                        '{dr["seriPengangkut"]}'
                                        )");
                    }

                    //if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    //    retval = listQuery.Count;
                }
                #endregion

                #region load data profect
                query = $@"select 
                        T1.HAWB,
                        T2.TGLHAWB as TGLHAWB,
                        FORMAT(TOTAL_FREIGHT,'N0') as FreightRupiah,
                        FORMAT(TOTAL_FREIGHT/{ndpbm_global},'N2') AS Freight
                        from PROFECTFREIGHT T1 LEFT JOIN BILLINGREPORT T2
                        ON T1.HAWB = T2.HAWB
                        where T1.HAWB in ({hawb})
                        AND T2.TGLHAWB >= '{DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd")}'";
                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    logger.Log($"LOAD DATA PROFECT {hawb} : {dt.Rows.Count} Rows..");
                    foreach (DataRow dr in dt.Rows)
                    {
                        listQuery.Add($@"insert into dhl_cs_tpb_profect (HAWB,TGL_HAWB,FreightRupiah,Freight) values (
                                        '{dr["HAWB"]}', '{Convert.ToDateTime(dr["TGLHAWB"]).ToString("yyyy-MM-dd")}',
                                        '{dr["FreightRupiah"].ToString().Replace(",","")}',
                                        '{dr["Freight"].ToString().Replace(",", "")}'
                                        )");
                    }
                }
                #endregion

                if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    retval = listQuery.Count;

                //foreach (var item in listQuery)
                //{
                //    logger.Log(item + "\n");
                //}
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message, ex.StackTrace);
            }
            return retval;
        }
        #endregion

        #region modals
        private void ClearFormDCI()
        {
            inputHAWB.Text = string.Empty;
            listAju.Text = string.Empty;
        }
        private void ShowModalDCI()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "popup", "$('#btnPopupDCI').click();", true);
        }
        private void ShowModalSend()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "popup", "$('#btnPopupSend').click();", true);
        }
        private void ShowModalLoader()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "popup", "$('#btnPopupLoader').click();", true);
        }
        private void ShowModalBarang()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "popup", "$('#btnPopupBarang').click();", true);
        }
        private void ShowModalHistory()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "popup", "$('#btnPopupHistory').click();", true);
        }
        private void ShowModalHistoryReject()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "popup", "$('#btnPopupHistoryReject').click();", true);
        }
        private void CloseModalDCI()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "close", "$('#btnCloseHAWB').click();", true);
        }
        private void CloseModalSend()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "close", "$('#btnCloseSend').click();", true);
        }
        private void CloseModalLoader()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "close", "$('#btnCloseLoader').click();", true);
        }
        private void CloseModalLoaderXD()
        {
            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "close", "$('#btnCloseLoaderXD').click();", true);
        }
        #endregion

        #region custom
        private string GetDate(string date)
        {
            string retval = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(date) == false)
                {
                    string[] dates = date.Split('-');
                    retval = $"{dates[2]}-{dates[1]}-{dates[0]}";
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return retval;
        }
        private string GetDateJson(string date)
        {
            string retval = string.Empty;
            try
            {
                DateTime dt = Convert.ToDateTime(date);
                retval = dt.ToString("yyyy-MM-dd");

                if (retval == "1900-01-01")
                    retval = DateTime.Now.ToString("yyyy-MM-dd");
            }
            catch { }
            return retval;
        }
        private string GetDateAju(string date)
        {
            string retval = string.Empty;
            try
            {
                string yyyy = date.Substring(0, 4);
                string MM = date.Substring(4, 2);
                string dd = date.Substring(6, 2);
                retval = $"{yyyy}-{MM}-{dd}";
            }
            catch { }
            return retval;
        }
        private string GetValCurrFinal(string kursUpdate, string kursNow, string value)
        {
            string retval = $"{value}";
            try
            {
                double cifUpdate = Convert.ToDouble(value) * Convert.ToDouble(kursNow); //jadiin rupiah dulu
                cifUpdate /= Convert.ToDouble(kursUpdate); //dibagi kurs terbaru
                retval = $"{Math.Round(cifUpdate, 2, MidpointRounding.AwayFromZero)}";
            }
            catch (Exception ex)
            {
                logger.Log($"err Func GetValCurrFinal: {ex.Message}");
            }
            return retval;
        }
        private bool IsXmlCreated(string nomorAju, string hawb)
        {
            bool retval = false;
            try
            {
                string query = $@"select top 1 * from dhl_cs_tpb_status_xml where NO_AJU = '{nomorAju}' and HAWB = '{hawb}'";
                DataRow row = DbOfficialCeisa.getRow(query);

                if (row != null) retval = true;
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
            return retval;
        }
        private string GetKodeBendera(string namaPengangkut)
        {
            string retval = "ID";
            try
            {
                string query = $@"select top 1 flag from dhl_cs_master_pengangkut where airlines like '%{namaPengangkut}%' ";
                DataRow row = DbOfficialCeisa.getRow(query);

                if (row != null) retval = row["flag"].ToString();
            }
            catch (Exception ex)
            {
                logger.Log($"err Func GetKodeBendera: {ex.Message}");
            }
            return retval;
        }
        private bool IsSkepValid(string HAWB, string TGLHAWB)
        {
            bool retval = true;
            try
            {
                string query = $@"select top 1 convert(varchar(10),tanggalIjinEntitas,20) as TGL_SKEP from dhl_cs_tpb_entitas where HAWB = '{HAWB}' and TGL_HAWB = '{TGLHAWB}' and seriEntitas = '1'";
                string tgl_skep = DbOfficialCeisa.getFieldValue(query);
                if (tgl_skep == "1900-01-01") retval = false;
            }
            catch (Exception ex)
            {
                logger.Log($"err Func IsSkepValid: {ex.Message}");
            }
            return retval;
        }
        private string GetTanggalSkep(string NPWP, string NO_SKEP)
        {
            string retval = "2000-01-01";
            try
            {
                string query = $@"select top 1 
                                isnull(convert(varchar(10),tgl_skkb,20),'2000-01-01') as TGL_SKEP
                                from CUSTOMER 
                                where '0'+replace(replace(isnull(NPWP, ''),'.',''),'-','')+'000000' = '{NPWP}'
                                and isnull(skkb, '') = '{NO_SKEP}'";
                DataRow row = DbOfficialDCI.getRow(query);
                if (row != null) retval = row["TGL_SKEP"].ToString();
            }
            catch (Exception ex)
            {
                logger.Log($"err Func GetTanggalSkep : {ex.Message}");
            }
            return retval;
        }
        private string GetNIB(string NPWP, string NO_SKEP)
        {
            string retval = "";
            try
            {
                string query = $@"select top 1 
                                isnull(NONIB,'') as NIB
                                from CUSTOMER 
                                where '0'+replace(replace(isnull(NPWP, ''),'.',''),'-','')+'000000' = '{NPWP}'
                                and isnull(skkb, '') = '{NO_SKEP}'";
                DataRow row = DbOfficialDCI.getRow(query);
                if (row != null) retval = row["NIB"].ToString();
            }
            catch (Exception ex)
            {
                logger.Log($"err Func GetNIB : {ex.Message}");
            }
            return retval;
        }
        #endregion

        #region download CSV
        protected void btnDownloadBarang_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tblBarang = (DataTable)ViewState["ListBarang"];

                //set file name
                string filename = "Barang_" + DateTime.Now.ToString("yyyyMMddHHmmss");

                string virtualpath = "~/Temp/" + filename;
                string filepath = MapPath("~/Temp/" + filename);
                GenerateExcel(tblBarang, filename, "", $"{DateTime.Now.ToString("yyyy-MM-dd")}");

                //if (tblBarang.Rows.Count > 0)
                //{
                //    Response.Clear();
                //    Response.Buffer = true;
                //    Response.ContentType = "application/csv";

                //    string HAWB = string.Empty;
                //    StringBuilder sb = new StringBuilder();

                //    //header
                //    sb.AppendLine("HAWB;TGL_HAWB;ASURANSI;CIF;CIF_RUPIAH;FOB;FREIGHT;HARGA_SATUAN;JUMLAH_SATUAN;KODE_NEGARA_ASAL;KODE_SATUAN_BARANG;NDPBM;NETTO;POS_TARIF;SERI_BARANG;BM_TARIF;CUKAI_TARIF;PPN_TARIF;PPNBM_TARIF;PPH_TARIF;URAIAN");

                //    foreach (DataRow dr in tblBarang.Rows)
                //    {
                //        HAWB = dr["HAWB"].ToString();

                //        //write data
                //        sb.Append($"{dr["HAWB"].ToString()};");
                //        sb.Append($"{dr["TGL_HAWB"].ToString()};");
                //        sb.Append($"{dr["ASURANSI"].ToString()};");
                //        sb.Append($"{dr["CIF"].ToString()};");
                //        sb.Append($"{dr["CIF RUPIAH"].ToString()};");
                //        sb.Append($"{dr["FOB"].ToString()};");
                //        sb.Append($"{dr["FREIGHT"].ToString()};");
                //        sb.Append($"{dr["HARGA SATUAN"].ToString()};");
                //        sb.Append($"{dr["JUMLAH SATUAN"].ToString()};");
                //        sb.Append($"{dr["KODE NEGARA ASAL"].ToString()};");
                //        sb.Append($"{dr["KODE SATUAN BARANG"].ToString()};");
                //        sb.Append($"{dr["NDPBM"].ToString()};");
                //        sb.Append($"{dr["NETTO"].ToString()};");
                //        sb.Append($"{dr["POS TARIF"].ToString()};");
                //        sb.Append($"{dr["SERI BARANG"].ToString()};");
                //        sb.Append($"{dr["BM TARIF"].ToString()};");
                //        sb.Append($"{dr["CUKAI TARIF"].ToString()};");
                //        sb.Append($"{dr["PPN TARIF"].ToString()};");
                //        sb.Append($"{dr["PPNBM TARIF"].ToString()};");
                //        sb.Append($"{dr["PPH TARIF"].ToString()};");
                //        sb.AppendLine($"{dr["URAIAN"].ToString()}");
                //    }

                //    string filename = $@"barang_{HAWB}.csv";
                //    Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                //    Response.Output.Write(sb.ToString());
                //    Response.Flush();

                //    // Prevents any other content from being sent to the browser
                //    Response.SuppressContent = true;

                //    // Directs the thread to finish, bypassing additional processing
                //    HttpContext.Current.ApplicationInstance.CompleteRequest();
                //}
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        protected void btnDownloadDokumen_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tblDokumen = (DataTable)ViewState["ListDokumen"];

                //set file name
                string filename = "Dokumen_" + DateTime.Now.ToString("yyyy-MM-dd");

                string virtualpath = "~/Temp/" + filename;
                string filepath = MapPath("~/Temp/" + filename);
                GenerateExcel(tblDokumen, filename, "", $"{DateTime.Now.ToString("yyyy-MM-dd")}");
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        protected void btnDownloadDataKPBC_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tblDokumen = (DataTable)ViewState["ListDataDCI"];

                //set file name
                string filename = "DokumenListTPB_" + DateTime.Now.ToString("yyyyMMddHHmmss");

                string virtualpath = "~/Temp/" + filename;
                string filepath = MapPath("~/Temp/" + filename);
                GenerateExcel(tblDokumen, filename, "", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}");
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        public void GenerateExcel(DataTable pDataTable, string filename, string pStringHeader, string pSheetName)
        {
            try
            {
                using (ExcelPackage pack = new ExcelPackage())
                {
                    ExcelWorksheet ws = pack.Workbook.Worksheets.Add(pSheetName);

                    // METODE AUTOGENERATE BY DATATABLE
                    ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center 
                    ws.Cells["A1"].LoadFromDataTable(pDataTable, true, OfficeOpenXml.Table.TableStyles.Light8);
                    ws.Cells.AutoFitColumns();

                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xlsx");
                        pack.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();

                        // Prevents any other content from being sent to the browser
                        Response.SuppressContent = true;

                        // Directs the thread to finish, bypassing additional processing
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception Ex)
            {
                Response.Write(Ex.Message);
                //Common.LogError(refNumber, GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name, Ex);
                //ShowModalNotif(string.Format("{0} - {1}", GeneralError(), refNumber), 0, "");
            }
        }
        #endregion

        #region upload CSV
        protected void btnSubmitBarang_Click(object sender, EventArgs e)
        {
            try
            {
                if (uploadBarang.HasFile)
                {
                    //get filename only
                    FileInfo fi = new FileInfo(uploadBarang.PostedFile.FileName);
                    string fileName = fi.Name.ToUpper();
                    string filePath = Server.MapPath("~/Temp/") + Path.GetFileName(fileName);
                    uploadBarang.SaveAs(filePath);

                    //process file
                    //ProcessFileBarang(filePath, fileName);
                    ProcessExcelBarang(filePath, fileName);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Required File Upload.');", true);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        protected void btnSubmitDokumen_Click(object sender, EventArgs e)
        {
            try
            {
                if (uploadDokumen.HasFile)
                {
                    //get filename only
                    FileInfo fi = new FileInfo(uploadDokumen.PostedFile.FileName);
                    string fileName = fi.Name.ToUpper();
                    string filePath = Server.MapPath("~/Temp/") + Path.GetFileName(fileName);
                    uploadDokumen.SaveAs(filePath);

                    //process file
                    ProcessExcelDokumen(filePath, fileName);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Required File Upload.');", true);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        private void ProcessFileBarang(string filePath, string fileName)
        {
            //read file
            string[] lines = File.ReadAllLines(filePath);
            List<string> listQuery = new List<string>();
            int idx = 0;
            string HAWB = string.Empty;
            string TGL_HAWB = string.Empty;
            double asuransi = 0;
            double fob = 0;
            double freight = 0;

            #region barang
            foreach (string line in lines)
            {
                ++idx;

                if (idx == 1) continue;

                string[] arr = new string[0];

                if (line.Contains(';'))
                {
                    arr = line.Split(';');

                    //validate file
                    if (arr.Length == 21)
                    {
                        double BM_Items = 0.0;
                        double CUKAI_Items = 0.0;
                        double PPN_Items = 0.0;
                        double PPNBM_Items = 0.0;
                        double PPH_Items = 0.0;
                        double CIF_Items = 0.0;

                        #region array model
                        UploadBarang ub = new UploadBarang();
                        ub.HAWB = arr[0];
                        ub.TGL_HAWB = arr[1];
                        ub.ASURANSI = Math.Round(Convert.ToDouble(arr[2]), 2);
                        ub.CIF = Math.Round(Convert.ToDouble(arr[3]), 2);
                        ub.CIF_RUPIAH = Math.Round(Convert.ToDouble(arr[4]), 2);
                        ub.FOB = Math.Round(Convert.ToDouble(arr[5]), 2);
                        ub.FREIGHT = Math.Round(Convert.ToDouble(arr[6]), 2);
                        ub.HARGA_SATUAN = Math.Round(Convert.ToDouble(arr[7]), 2);
                        ub.JUMLAH_SATUAN = Math.Round(Convert.ToDouble(arr[8]), 2);
                        ub.KODE_NEGARA_ASAL = arr[9];
                        ub.KODE_SATUAN_BARANG = arr[10];
                        ub.NDPBM = Math.Round(Convert.ToDouble(arr[11]), 2);
                        ub.NETTO = Math.Round(Convert.ToDouble(arr[12]), 2);
                        ub.POS_TARIF = arr[13];
                        ub.SERI_BARANG = Convert.ToInt32(arr[14]);
                        ub.BM_TARIF = Convert.ToDouble(arr[15]);
                        ub.CUKAI_TARIF = Convert.ToDouble(arr[16]);
                        ub.PPN_TARIF = Convert.ToDouble(arr[17]);
                        ub.PPNBM_TARIF = Convert.ToDouble(arr[18]);
                        ub.PPH_TARIF = Convert.ToDouble(arr[19]);
                        ub.URAIAN = arr[20];

                        CIF_Items = Math.Round((double)ub.ASURANSI * ub.NDPBM, 2) + Math.Round((double)ub.FOB * ub.NDPBM, 2) + Math.Round((double)ub.FREIGHT * ub.NDPBM, 2);
                        BM_Items = ((double)CIF_Items * ub.BM_TARIF / 100);
                        CUKAI_Items = Math.Round((double)(CIF_Items + BM_Items) * ub.CUKAI_TARIF / 100, 2);
                        PPN_Items = Math.Round((double)(CIF_Items + BM_Items) * ub.PPN_TARIF / 100, 2);
                        PPNBM_Items = Math.Round((double)(CIF_Items + BM_Items) * ub.PPNBM_TARIF / 100, 2);
                        PPH_Items = Math.Round((double)(CIF_Items + BM_Items) * ub.PPH_TARIF / 100, 2);

                        listQuery.Add($@"insert into dhl_cs_barang values (
                                        '{ub.HAWB}',
                                        '{GetDateJson(ub.TGL_HAWB)}',
                                        '{ub.ASURANSI}',
                                        '0',
                                        '{ub.CIF}',
                                        '{ub.CIF_RUPIAH}',
                                        '0',
                                        '{ub.FOB}',
                                        '{ub.FREIGHT}',
                                        '0','0','0','0',
                                        '{ub.HARGA_SATUAN}',
                                        '0','0','0','0','1','0','0',
                                        '{ub.JUMLAH_SATUAN}',
                                        '0','PK','1',
                                        '{ub.KODE_NEGARA_ASAL}',
                                        '{ub.KODE_SATUAN_BARANG}',
                                        '-',
                                        '{ub.NDPBM}',
                                        '{ub.NETTO}',
                                        '0','0','0','0','T','0',
                                        '{ub.POS_TARIF}',
                                        '0','0',
                                        '{ub.SERI_BARANG}',
                                        '0','0','0','0','-',
                                        '{ub.URAIAN}',
                                        '0'
                                        )");

                        for (int i = 0; i < 5; i++)
                        {
                            switch (i)
                            {
                                case 1: //cukai
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{ub.HAWB}',
                                        '{GetDateJson(ub.TGL_HAWB)}',
                                        '1','1','1','CUKAI','{CUKAI_Items}',
                                        '{ub.SERI_BARANG}',
                                        '{ub.CUKAI_TARIF}',
                                        '0','0'
                                        )");
                                    break;
                                case 2: //ppn
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{ub.HAWB}',
                                        '{GetDateJson(ub.TGL_HAWB)}',
                                        '1','1','1','PPN','{PPN_Items}',
                                        '{ub.SERI_BARANG}',
                                        '{ub.PPN_TARIF}',
                                        '0','0'
                                        )");
                                    break;
                                case 3: //ppnbm
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{ub.HAWB}',
                                        '{GetDateJson(ub.TGL_HAWB)}',
                                        '1','1','1','PPNBM','{PPNBM_Items}',
                                        '{ub.SERI_BARANG}',
                                        '{ub.PPNBM_TARIF}',
                                        '0','0'
                                        )");
                                    break;
                                case 4: //pph
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{ub.HAWB}',
                                        '{GetDateJson(ub.TGL_HAWB)}',
                                        '1','1','1','PPH','{PPH_Items}',
                                        '{ub.SERI_BARANG}',
                                        '{ub.PPH_TARIF}',
                                        '0','0'
                                        )");
                                    break;
                                default: //bm
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{ub.HAWB}',
                                        '{GetDateJson(ub.TGL_HAWB)}',
                                        '1','1','1','BM','{BM_Items}',
                                        '{ub.SERI_BARANG}',
                                        '{ub.BM_TARIF}',
                                        '0','0'
                                        )");
                                    break;
                            }
                        }

                        HAWB = ub.HAWB;
                        TGL_HAWB = GetDateJson(ub.TGL_HAWB);

                        asuransi += Math.Round(ub.ASURANSI, 2);
                        fob += Math.Round(ub.FOB, 2);
                        freight += Math.Round(ub.FREIGHT, 2);
                        #endregion
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Upload CSV Failed, Invalid Format Value');", true);
                    return;
                }
            }
            #endregion

            string query = $@"delete from dhl_cs_barang where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
            DbOfficialCeisa.runCommand(query);

            query = $@"delete from dhl_cs_barang_tarif where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
            DbOfficialCeisa.runCommand(query);

            query = $@"update dhl_cs_header set asuransi = '{asuransi}', fob = '{fob}', freight = '{freight}'
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
            DbOfficialCeisa.runCommand(query);

            if (listQuery.Count > 0)
            {
                if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Upload CSV Success.');", true);
            }

            query = $@"select
                    HAWB, convert(char(10),TGL_HAWB,126) as TGL_HAWB,
                    asuransi as [ASURANSI], 
                    cif as [CIF], 
                    cifRupiah as [CIF RUPIAH],
                    fob as [FOB],
                    freight as [FREIGHT],
                    hargaSatuan as [HARGA SATUAN],
                    jumlahSatuan as [JUMLAH SATUAN],
                    kodeNegaraAsal as [KODE NEGARA ASAL],
                    kodeSatuanBarang as [KODE SATUAN BARANG],
                    jumlahKemasan as [JUMLAH KEMASAN],
                    kodeJenisKemasan as [KODE KEMASAN],
                    case
                        when kodeBarang = '' then '-'
                        else kodeBarang
                    end as [KODE BARANG],
                    ndpbm as [NDPBM],
                    (select top 1 kodeValuta from dhl_cs_tpb_header
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}') as [KODE VALUTA],
                    netto as [NETTO],
                    kodeKategoriBarang as [KATEGORI BARANG],
                    posTarif as [POS TARIF],
                    seriBarang as [SERI BARANG],
                    (select tarif from dhl_cs_tpb_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'BM' and seriBarang = barang.seriBarang) as [BM TARIF],
                    (select tarif from dhl_cs_tpb_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'BMTP' and seriBarang = barang.seriBarang) as [BMTP TARIF],
                    (select tarif from dhl_cs_tpb_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'CUKAI' and seriBarang = barang.seriBarang) as [CUKAI TARIF],
                    (select tarif from dhl_cs_tpb_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'PPN' and seriBarang = barang.seriBarang) as [PPN TARIF],
                    (select tarif from dhl_cs_tpb_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'PPNBM' and seriBarang = barang.seriBarang) as [PPNBM TARIF],
                    (select tarif from dhl_cs_tpb_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'PPH' and seriBarang = barang.seriBarang) as [PPH TARIF],
                    uraian as [URAIAN],
                    merk as [MEREK],
                    tipe as [TIPE],
                    case 
                        when ukuran = '' then '-' 
                        else ukuran
                    end as [UKURAN],
                    case 
                        when spesifikasiLain = '' then '-'
                        else spesifikasiLain
                    end as [SPESIFIKASI LAINNYA]
                    from dhl_cs_tpb_barang barang
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    order by seriBarang asc";
            DataTable tblBarang = DbOfficialCeisa.getRecords(query);
            ViewState["ListBarang"] = tblBarang;
            GV_Barang.DataSource = tblBarang;
            GV_Barang.DataBind();

            query = $@"select
                    HAWB, convert(char(10),TGL_HAWB,126) as TGL_HAWB,
                    kodeJenisPungutan as [KODE JENIS PUNGUTAN],
                    nilaiBayar as [NILAI BAYAR],
                    seriBarang as [SERI BARANG],
                    tarif as [TARIF]
                    from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    order by seriBarang asc";
            DataTable tblDokumen = DbOfficialCeisa.getRecords(query);
            ViewState["ListDokumen"] = tblDokumen;
            GV_Dokumen.DataSource = tblDokumen;
            GV_Dokumen.DataBind();
        }
        private void ProcessExcelBarang(string filePath, string fileName)
        {
            try
            {
                DataTable tbl = new DataTable();
                FileInfo newFile = new FileInfo(filePath);
                ExcelPackage pck = new ExcelPackage(newFile);
                var summary = pck.Workbook.Worksheets[1];
                var range = summary.Cells["A2:AE500"];
                string type = "brg";
                tbl = GetDataTableFromRange(range, type);
                InsertToDb(tbl, type);
            }
            catch { }
        }
        private void ProcessExcelDokumen(string filePath, string fileName)
        {
            try
            {
                DataTable tbl = new DataTable();
                FileInfo newFile = new FileInfo(filePath);
                ExcelPackage pck = new ExcelPackage(newFile);
                var summary = pck.Workbook.Worksheets[1];
                var range = summary.Cells["A2:E99"];
                string type = "dok";
                tbl = GetDataTableFromRange(range, type);
                InsertToDb(tbl, type);
            }
            catch { }
        }
        private DataTable GetDataTableFromRange(ExcelRange range, string type)
        {
            DataTable tbl = new DataTable();

            if (type == "brg")
            {
                tbl.Columns.Add("HAWB");
                tbl.Columns.Add("TGL_HAWB");
                tbl.Columns.Add("ASURANSI");
                tbl.Columns.Add("CIF");
                tbl.Columns.Add("CIF_RUPIAH");
                tbl.Columns.Add("FOB");
                tbl.Columns.Add("FREIGHT");
                tbl.Columns.Add("HARGA_SATUAN");
                tbl.Columns.Add("JUMLAH_SATUAN");
                tbl.Columns.Add("KODE_NEGARA_ASAL");
                tbl.Columns.Add("KODE_SATUAN_BARANG");
                tbl.Columns.Add("JUMLAH_KEMASAN");
                tbl.Columns.Add("KODE_KEMASAN");
                tbl.Columns.Add("KODE_BARANG");
                tbl.Columns.Add("NDPBM");
                tbl.Columns.Add("KODE_VALUTA");
                tbl.Columns.Add("NETTO");
                tbl.Columns.Add("KATEGORI_BARANG");
                tbl.Columns.Add("POS_TARIF");
                tbl.Columns.Add("SERI_BARANG");
                tbl.Columns.Add("BM_TARIF");
                tbl.Columns.Add("BMTP_TARIF");
                tbl.Columns.Add("CUKAI_TARIF");
                tbl.Columns.Add("PPN_TARIF");
                tbl.Columns.Add("PPNBM_TARIF");
                tbl.Columns.Add("PPH_TARIF");
                tbl.Columns.Add("URAIAN");
                tbl.Columns.Add("MEREK");
                tbl.Columns.Add("TIPE");
                tbl.Columns.Add("UKURAN");
                tbl.Columns.Add("SPEK_LAINNYA");

                DataRow newRow = null;
                int dataTableColumn = 0;
                int currRow = -1;
                foreach (var item in range)
                {
                    if (currRow != item.Start.Row)
                    {
                        newRow = tbl.NewRow();
                        tbl.Rows.Add(newRow);
                        dataTableColumn = 0;
                        currRow = item.Start.Row;
                    }

                    if (item.Address == $"A{currRow}")
                    {
                        if (string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            tbl.Rows.Remove(newRow);
                            break;
                        }
                    }
                    if (item.Address == $"O{currRow}")
                    {
                        if (dataTableColumn != 14)
                        {
                            for (int i = dataTableColumn; i < 14; i++)
                            {
                                newRow[i] = "";
                                dataTableColumn += 1;
                            }
                        }
                    }
                    logger.Log($"row item excel ke-{currRow}, column ke-{dataTableColumn}");
                    newRow[dataTableColumn] = item.Value.ToString();
                    dataTableColumn += 1;
                }
            }
            else
            {
                tbl.Columns.Add("HAWB");
                tbl.Columns.Add("TGL_HAWB");
                tbl.Columns.Add("KODE_DOKUMEN");
                tbl.Columns.Add("NOMOR_DOKUMEN");
                tbl.Columns.Add("TGL_DOKUMEN");

                DataRow newRow = null;
                int dataTableColumn = 0;
                int currRow = -1;
                foreach (var item in range)
                {
                    if (currRow != item.Start.Row)
                    {
                        newRow = tbl.NewRow();
                        tbl.Rows.Add(newRow);
                        dataTableColumn = 0;
                        currRow = item.Start.Row;
                    }

                    if (item.Address == $"A{currRow}")
                    {
                        if (string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            tbl.Rows.Remove(newRow);
                            break;
                        }
                    }
                    else if (item.Address == $"E{currRow}")
                    {
                        if (item.Value.ToString().Length == 10)
                        {
                            newRow[dataTableColumn] = item.Value.ToString();
                        }
                        else
                        {
                            double time = Convert.ToDouble(item.Value);
                            DateTime conv = DateTime.FromOADate(time);
                            newRow[dataTableColumn] = conv.ToString("yyyy-MM-dd");
                        }
                        continue;
                    }
                    newRow[dataTableColumn] = item.Value.ToString();
                    dataTableColumn += 1;
                }
            }

            return tbl;
        }
        private void InsertToDb(DataTable tbl, string type)
        {
            try
            {
                List<string> listQuery = new List<string>();
                if (tbl.Rows.Count > 0)
                {
                    if (type == "brg")
                    {
                        double asuransi = 0;
                        double fob = 0;
                        double freight = 0;
                        double ndpbm_final = 0;
                        double cif_final = 0;
                        string HAWB = string.Empty;
                        string TGL_HAWB = string.Empty;

                        #region calculate total fob & freight
                        double total_fob = 0;
                        double total_fre = 0;
                        bool is_freight_header = false;
                        bool is_single_item = false;

                        foreach (DataRow row in tbl.Rows)
                        {
                            double FOB = Convert.ToDouble(row["FOB"]);
                            double FREIGHT = Convert.ToDouble(row["FREIGHT"]);
                            if (tbl.Rows.Count == 1)
                            {
                                total_fob += Math.Round(FOB, 2);
                                total_fre += Math.Round(FREIGHT, 2);
                                is_single_item = true;
                            }
                            else
                            {
                                if (is_freight_header == false)
                                {                                    
                                    foreach (DataRow row_freight in tbl.Rows)
                                    {
                                        double FREIGHT_HEADER = Convert.ToDouble(row_freight["FREIGHT"]);
                                        total_fre += Math.Round(FREIGHT_HEADER, 2);
                                    }
                                    is_freight_header = true;
                                }

                                total_fob += Math.Round(FOB, 2);
                            }
                        }
                        #endregion

                        foreach (DataRow row in tbl.Rows)
                        {
                            if (is_single_item)
                            {
                                double BM_Items = 0.0;
                                double BMTP_Items = 0.0;
                                double PPN_Items = 0.0;
                                double PPH_Items = 0.0;
                                double CIF_Items = 0.0;
                                double CIF = 0.0;

                                double ASURANSI = (total_fob + total_fre) * 0.005;
                                //double ASURANSI = Math.Round(Convert.ToDouble(row["ASURANSI"]), 2);
                                double NDPBM = Math.Round(Convert.ToDouble(row["NDPBM"]), 2);
                                double FOB = total_fob;
                                //double FOB = Math.Round(Convert.ToDouble(row["FOB"]), 2);
                                double FREIGHT = total_fre;
                                //double FREIGHT = Math.Round(Convert.ToDouble(row["FREIGHT"]), 2);
                                double BM_Tarif = Math.Round(Convert.ToDouble(row["BM_TARIF"]), 2);
                                double BMTP_Tarif = Math.Round(Convert.ToDouble(row["BMTP_TARIF"]), 2);
                                double PPN_Tarif = Math.Round(Convert.ToDouble(row["PPN_TARIF"]), 2);
                                double PPH_Tarif = Math.Round(Convert.ToDouble(row["PPH_TARIF"]), 2);

                                CIF = Math.Round((double)ASURANSI, 2) + Math.Round((double)FOB, 2) + Math.Round((double)FREIGHT, 2);
                                CIF_Items = Math.Round((double)ASURANSI * NDPBM, 2) + Math.Round((double)FOB * NDPBM, 2) + Math.Round((double)FREIGHT * NDPBM, 2);
                                BM_Items = ((double)CIF_Items * BM_Tarif / 100);
                                PPN_Items = Math.Round((double)(CIF_Items + BM_Items) * PPN_Tarif / 100, 2);
                                PPH_Items = Math.Round((double)(CIF_Items + BM_Items) * PPH_Tarif / 100, 2);
                                BMTP_Items = Convert.ToDouble(row["JUMLAH_SATUAN"]) * BMTP_Tarif;

                                listQuery.Add($@"insert into dhl_cs_tpb_barang values (
                                                '{row["HAWB"]}',
                                                '{row["TGL_HAWB"]}',
                                                '1',
                                                '{ASURANSI}',
                                                '{CIF}',
                                                '{CIF_Items}',
                                                '0',
                                                '{FOB}',
                                                '{FREIGHT}',
                                                '0','0',
                                                '{row["HARGA_SATUAN"]}',
                                                '0',
                                                '{row["JUMLAH_KEMASAN"]}',
                                                '{row["JUMLAH_SATUAN"]}',
                                                '{row["KODE_BARANG"]}',
                                                '23',
                                                '{row["KATEGORI_BARANG"]}',
                                                '{row["KODE_KEMASAN"]}',
                                                '{row["KODE_NEGARA_ASAL"]}',
                                                '0',
                                                '{row["KODE_SATUAN_BARANG"]}',
                                                '{row["MEREK"]}',
                                                '{row["NDPBM"]}',
                                                '{row["NETTO"]}',
                                                '{FOB}',
                                                '0',
                                                '{row["POS_TARIF"]}',
                                                '{row["SERI_BARANG"]}',
                                                '{row["SPEK_LAINNYA"]}',
                                                '{row["TIPE"]}',
                                                '{row["UKURAN"]}',
                                                '{row["URAIAN"]}',
                                                '0','0'
                                                )");

                                for (int i = 0; i < 6; i++)
                                {
                                    switch (i)
                                    {
                                        case 1: //pph
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','6',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'PPH',
                                                            '{PPH_Items}','0','{PPH_Items}',
                                                            '{row["SERI_BARANG"]}',
                                                            '{row["PPH_TARIF"]}',
                                                            '100'
                                                            )");
                                            break;
                                        case 2: //ppn
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','6',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'PPN',
                                                            '{PPN_Items}','0','{PPN_Items}',
                                                            '{row["SERI_BARANG"]}',
                                                            '{row["PPN_TARIF"]}',
                                                            '100'
                                                            )");
                                            break;
                                        case 3: //ppnbm
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','3',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'PPNBM',
                                                            '0','0','0',
                                                            '{row["SERI_BARANG"]}',
                                                            '0','0'                                        
                                                            )");
                                            break;
                                        case 4: //bmtp
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','3',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'BMTP',
                                                            '{BMTP_Items}','0','{BMTP_Items}',
                                                            '{row["SERI_BARANG"]}',
                                                            '{row["BMTP_TARIF"]}',
                                                            '100'                                        
                                                            )");
                                            break;
                                        case 5: //cukai
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','3',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'CUKAI',
                                                            '0','0','0',
                                                            '{row["SERI_BARANG"]}',
                                                            '0','0'                                        
                                                            )");
                                            break;
                                        default: //bm
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','3',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'BM',
                                                            '{BM_Items}','0','{BM_Items}',
                                                            '{row["SERI_BARANG"]}',
                                                            '{row["BM_TARIF"]}',
                                                            '100'
                                                            )");
                                            break;
                                    }
                                }

                                HAWB = row["HAWB"].ToString();
                                TGL_HAWB = row["TGL_HAWB"].ToString();

                                //asuransi += Math.Round(ASURANSI, 2);
                                //asuransi ditotal apa adanya, baru roundup 2024-05-15
                                asuransi += ASURANSI;
                                fob += Math.Round(FOB, 2);
                                freight += Math.Round(FREIGHT, 2);
                                ndpbm_final = NDPBM; //UPDATE KURS HEADER
                            }
                            else
                            {
                                double BM_Items = 0.0;
                                double BMTP_Items = 0.0;
                                double PPN_Items = 0.0;
                                double PPH_Items = 0.0;
                                double CIF_Items = 0.0;
                                double CIF = 0.0;

                                double NDPBM = Math.Round(Convert.ToDouble(row["NDPBM"]), 2);
                                double FOB = Math.Round(Convert.ToDouble(row["FOB"]), 2);
                                double PERSENFOB = FOB / total_fob;
                                double FREIGHT = total_fre * PERSENFOB;
                                double ASURANSI = 0;
                                string valFreight = $"{FREIGHT}";
                                bool hasExponential = (valFreight.Contains("E") || valFreight.Contains("e")) && double.TryParse(valFreight, out FREIGHT);
                                if (hasExponential)
                                {
                                    decimal convertedFreight = decimal.Parse(valFreight, NumberStyles.Float);
                                    decimal v = (Convert.ToDecimal(FOB) + convertedFreight);
                                    ASURANSI = Math.Round(Convert.ToDouble(v) * 0.005, 3);
                                    valFreight = $"{convertedFreight}";
                                }
                                else
                                {
                                    ASURANSI = Math.Round((FOB + FREIGHT) * 0.005, 3);
                                }
                                //double FREIGHT = Math.Round(Convert.ToDouble(row["FREIGHT"]), 2);
                                //double ASURANSI = Math.Round((FOB + FREIGHT) * 0.005, 3);
                                //double ASURANSI = Math.Round(Convert.ToDouble(row["ASURANSI"]), 2);
                                double BM_Tarif = Math.Round(Convert.ToDouble(row["BM_TARIF"]), 2);
                                double BMTP_Tarif = Math.Round(Convert.ToDouble(row["BMTP_TARIF"]), 2);
                                double PPN_Tarif = Math.Round(Convert.ToDouble(row["PPN_TARIF"]), 2);
                                double PPH_Tarif = Math.Round(Convert.ToDouble(row["PPH_TARIF"]), 2);                                

                                CIF = Math.Round((double)ASURANSI, 2) + Math.Round((double)FOB, 2) + Math.Round((double)FREIGHT, 2);
                                CIF_Items = Math.Round((double)ASURANSI * NDPBM, 2) + Math.Round((double)FOB * NDPBM, 2) + Math.Round((double)FREIGHT * NDPBM, 2);
                                BM_Items = ((double)CIF_Items * BM_Tarif / 100);
                                PPN_Items = Math.Round((double)(CIF_Items + BM_Items) * PPN_Tarif / 100, 2);
                                PPH_Items = Math.Round((double)(CIF_Items + BM_Items) * PPH_Tarif / 100, 2);
                                BMTP_Items = Convert.ToDouble(row["JUMLAH_SATUAN"]) * BMTP_Tarif;

                                listQuery.Add($@"insert into dhl_cs_tpb_barang values (
                                                '{row["HAWB"]}',
                                                '{row["TGL_HAWB"]}',
                                                '1',
                                                '{ASURANSI}',
                                                '{CIF}',
                                                '{CIF_Items}',
                                                '0',
                                                '{FOB}',
                                                '{(hasExponential == false ? FREIGHT.ToString() : valFreight)}',
                                                '0','0',
                                                '{row["HARGA_SATUAN"]}',
                                                '0',
                                                '{row["JUMLAH_KEMASAN"]}',
                                                '{row["JUMLAH_SATUAN"]}',
                                                '{row["KODE_BARANG"]}',
                                                '23',
                                                '{row["KATEGORI_BARANG"]}',
                                                '{row["KODE_KEMASAN"]}',
                                                '{row["KODE_NEGARA_ASAL"]}',
                                                '0',
                                                '{row["KODE_SATUAN_BARANG"]}',
                                                '{row["MEREK"]}',
                                                '{row["NDPBM"]}',
                                                '{row["NETTO"]}',
                                                '{FOB}',
                                                '0',
                                                '{row["POS_TARIF"]}',
                                                '{row["SERI_BARANG"]}',
                                                '{row["SPEK_LAINNYA"]}',
                                                '{row["TIPE"]}',
                                                '{row["UKURAN"]}',
                                                '{row["URAIAN"]}',
                                                '0','0'
                                                )");

                                for (int i = 0; i < 6; i++)
                                {
                                    switch (i)
                                    {
                                        case 1: //pph
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','6',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'PPH',
                                                            '{PPH_Items}','0','{PPH_Items}',
                                                            '{row["SERI_BARANG"]}',
                                                            '{row["PPH_TARIF"]}',
                                                            '100'
                                                            )");
                                            break;
                                        case 2: //ppn
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','6',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'PPN',
                                                            '{PPN_Items}','0','{PPN_Items}',
                                                            '{row["SERI_BARANG"]}',
                                                            '{row["PPN_TARIF"]}',
                                                            '100'
                                                            )");
                                            break;
                                        case 3: //ppnbm
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','3',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'PPNBM',
                                                            '0','0','0',
                                                            '{row["SERI_BARANG"]}',
                                                            '0','0'                                        
                                                            )");
                                            break;
                                        case 4: //bmtp
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','3',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'BMTP',
                                                            '{BMTP_Items}','0','{BMTP_Items}',
                                                            '{row["SERI_BARANG"]}',
                                                            '{row["BMTP_TARIF"]}',
                                                            '100'                                        
                                                            )");
                                            break;
                                        case 5: //cukai
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','3',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'CUKAI',
                                                            '0','0','0',
                                                            '{row["SERI_BARANG"]}',
                                                            '0','0'                                        
                                                            )");
                                            break;
                                        default: //bm
                                            listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH_SATUAN"]}','3',
                                                            '{row["KODE_SATUAN_BARANG"]}',
                                                            'BM',
                                                            '{BM_Items}','0','{BM_Items}',
                                                            '{row["SERI_BARANG"]}',
                                                            '{row["BM_TARIF"]}',
                                                            '100'
                                                            )");
                                            break;
                                    }
                                }

                                HAWB = row["HAWB"].ToString();
                                TGL_HAWB = row["TGL_HAWB"].ToString();

                                //asuransi += Math.Round(ASURANSI, 2);
                                //asuransi ditotal apa adanya, baru roundup 2024-05-15
                                asuransi += ASURANSI;
                                fob += Math.Round(FOB, 2);
                                freight += Math.Round(FREIGHT, 2);
                                ndpbm_final = NDPBM; //UPDATE KURS HEADER
                            }
                        }

                        string query = $@"delete from dhl_cs_tpb_barang where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                        DbOfficialCeisa.runCommand(query);

                        query = $@"delete from dhl_cs_tpb_barang_tarif where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                        DbOfficialCeisa.runCommand(query);

                        //UPDATE CIF & ASURANSI
                        asuransi = Math.Round(asuransi, 2);
                        cif_final = Math.Round((asuransi + fob + freight), 2);
                        query = $@"update dhl_cs_tpb_header set asuransi = '{asuransi}', fob = '{fob}', freight = '{freight}', cif = '{cif_final}', ndpbm = '{ndpbm_final}'
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                        DbOfficialCeisa.runCommand(query);

                        if (listQuery.Count > 0)
                        {
                            if (DbOfficialCeisa.runCommand(listQuery) == 0)
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Upload Excel Success.');", true);
                            else
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Upload excel gagal, silahkan cek kembali column pada excel yang belum terisi!');", true);

                            //foreach (var item in listQuery)
                            //{
                            //    if (DbOfficialCeisa.runCommand(item) != 0)
                            //        logger.Log(item);
                            //}
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Upload Excel Success.');", true);
                        }

                        query = $@"select
                                HAWB, convert(char(10),TGL_HAWB,126) as TGL_HAWB,
                                asuransi as [ASURANSI], 
                                cif as [CIF], 
                                cifRupiah as [CIF RUPIAH],
                                fob as [FOB],
                                freight as [FREIGHT],
                                hargaSatuan as [HARGA SATUAN],
                                jumlahSatuan as [JUMLAH SATUAN],
                                kodeNegaraAsal as [KODE NEGARA ASAL],
                                kodeSatuanBarang as [KODE SATUAN BARANG],
                                jumlahKemasan as [JUMLAH KEMASAN],
                                kodeJenisKemasan as [KODE KEMASAN],
                                case
                                    when kodeBarang = '' then '-'
                                    else kodeBarang
                                end as [KODE BARANG],
                                ndpbm as [NDPBM],
                                (select top 1 kodeValuta from dhl_cs_tpb_header
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}') as [KODE VALUTA],
                                netto as [NETTO],
                                kodeKategoriBarang as [KATEGORI BARANG],
                                posTarif as [POS TARIF],
                                seriBarang as [SERI BARANG],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'BM' and seriBarang = barang.seriBarang) as [BM TARIF],
                                ISNULL((select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'BMTP' and seriBarang = barang.seriBarang),'0.00') as [BMTP TARIF],
                                ISNULL((select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'CUKAI' and seriBarang = barang.seriBarang),'0.00') as [CUKAI TARIF],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'PPN' and seriBarang = barang.seriBarang) as [PPN TARIF],
                                ISNULL((select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'PPNBM' and seriBarang = barang.seriBarang),'0.00') as [PPNBM TARIF],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'PPH' and seriBarang = barang.seriBarang) as [PPH TARIF],
                                uraian as [URAIAN],
                                merk as [MEREK],
                                tipe as [TIPE],
                                case 
                                    when ukuran = '' then '-' 
                                    else ukuran
                                end as [UKURAN],
                                case 
                                    when spesifikasiLain = '' then '-'
                                    else spesifikasiLain
                                end as [SPESIFIKASI LAINNYA]
                                from dhl_cs_tpb_barang barang
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                order by seriBarang asc";
                        DataTable tblBarang = DbOfficialCeisa.getRecords(query);
                        ViewState["ListBarang"] = tblBarang;
                        GV_Barang.DataSource = tblBarang;
                        GV_Barang.DataBind();
                    }
                    else
                    {
                        string HAWB = string.Empty;
                        string TGL_HAWB = string.Empty;
                        int SERI_DOKUMEN = 0;

                        foreach (DataRow row in tbl.Rows)
                        {
                            SERI_DOKUMEN += 1;
                            listQuery.Add($@"insert into dhl_cs_tpb_dokumen values (
                                            '{row["HAWB"]}',
                                            '{row["TGL_HAWB"]}',
                                            '1','{row["KODE_DOKUMEN"]}',
                                            '{row["NOMOR_DOKUMEN"]}',
                                            '{SERI_DOKUMEN}',
                                            '{row["TGL_DOKUMEN"]}'
                                            )");

                            HAWB = row["HAWB"].ToString();
                            TGL_HAWB = row["TGL_HAWB"].ToString();
                        }

                        string query = $@"delete from dhl_cs_tpb_dokumen where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                        DbOfficialCeisa.runCommand(query);

                        if (listQuery.Count > 0)
                        {
                            if (DbOfficialCeisa.runCommand(listQuery) == 0)
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Upload Excel Success.');", true);
                        }

                        query = $@"select
                                HAWB, convert(char(10),TGL_HAWB,126) as TGL_HAWB,
                                kodeDokumen, nomorDokumen, 
                                convert(char(10),tanggalDokumen,126) as tanggalDokumen
                                from dhl_cs_tpb_dokumen
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                order by seriDokumen asc";
                        DataTable tblDokumen = DbOfficialCeisa.getRecords(query);
                        ViewState["ListDokumen"] = tblDokumen;
                        GV_Dokumen.DataSource = tblDokumen;
                        GV_Dokumen.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log($"err InsertToDb: {ex.Message}, {ex.StackTrace}");
            }
        }
        protected void btnSubmitEntitas_Click(object sender, EventArgs e)
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

                string HAWB = ViewState["ViewRecordHawb"].ToString();
                string TGL_HAWB = ViewState["ViewRecordTglHawb"].ToString();
                string nomorIdentitas = inputNomorIdentitas.Text.Trim().Replace("-", "").Replace(".", "");
                string namaEntitas = inputNamaEntitas.Text.Trim();
                string alamatEntitas = inputAlamatEntitas.Text.Trim();
                string nomorIjinEntitas = inputNomorIjinEntitas.Text.Trim();
                string tanggalIjinEntitas = inputTanggalIjinEntitas.Text.Trim();
                string nibEntitas = inputNibEntitas.Text.Trim();
                string jasa_kena_pajak = ddlJasaKenaPajak.SelectedItem.Value;

                string query_update = $@"update dhl_cs_tpb_entitas set
                                        nomorIdentitas = '{nomorIdentitas}',
                                        namaEntitas = '{namaEntitas}',
                                        alamatEntitas = '{alamatEntitas}',
                                        nomorIjinEntitas = '{nomorIjinEntitas}',
                                        tanggalIjinEntitas = '{tanggalIjinEntitas}',
                                        nibEntitas = '{nibEntitas}'
                                        where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}' and kodeEntitas in ('3','7') ";
                if (DbOfficialCeisa.runCommand(query_update) == 0)
                {
                    query_update = $"update dhl_cs_tpb_header set kodeKenaPajak = '{jasa_kena_pajak}' where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                    if (DbOfficialCeisa.runCommand(query_update) == 0)
                    {
                        logger.Log($"success update data entitas.. Updated By {update_by}");
                        logger.Log($"{query_update}");
                        logger.Log($"----------------------------------------------------");
                    }                   

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Success update data entitas!');", true);
                }
            }
            catch (Exception ex)
            {
                logger.Log($"err Func btnSubmitEntitas: {ex.Message}");
            }
        }
        #endregion

        #region Printout PDF
        protected void btnPrintPDF_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = sender as LinkButton;
                GridViewRow row = btn.NamingContainer as GridViewRow;

                string is_pdf_exist = GV_DataResponse.DataKeys[row.RowIndex].Values[0].ToString();
                if (is_pdf_exist != "Printout PDF") return;

                string file_type = GV_DataResponse.DataKeys[row.RowIndex].Values[1].ToString();
                string awb = GV_DataResponse.DataKeys[row.RowIndex].Values[2].ToString();
                string tgl_awb = GV_DataResponse.DataKeys[row.RowIndex].Values[3].ToString();

                string query = $@"select * from dhl_cs_response_data where 
                                HAWB = '{awb}' and TGL_HAWB = '{tgl_awb}' and keterangan = '{file_type}'";
                DataTable dt = DbOfficialCeisa.getRecords(query);
                Session["PrintPDF"] = dt;

                //display data
                string js = "window.open('PrintPDF.aspx', '_blank');";
                ScriptManager.RegisterStartupScript(this, GetType(), "Open PrintPDF.aspx", js, true);
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }


        #endregion

        #region GridView Report
        private void CreateReport(string HAWB, string TGL_HAWB)
        {
            string HAWBData = string.Empty;
            string TGLHAWB = string.Empty;
            //HttpContext.Current.Response.Redirect("~/Views/PreviewReport?HAWB=" + HAWB + "&TGL=" + TGL_HAWB);

            //display data
            string js = "window.open('PreviewReport.aspx?HAWB=" + HAWB + "&TGL=" + TGL_HAWB + "', '_blank');";
            ScriptManager.RegisterStartupScript(this, GetType(), "Open PreviewReport.aspx", js, true);
            //Response.Write("<script>window.open('PreviewPDF.aspx/Views/PreviewReport?HAWB=" + HAWB + "&TGL=" + TGL_HAWB +"','_blank');</script>");
        }

        #endregion
    }
}