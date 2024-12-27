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

namespace OfficialCeisaLite.Views
{
    public partial class Request : System.Web.UI.Page
    {
        NbLogger logger = new NbLogger(AppDomain.CurrentDomain.BaseDirectory + @"/Log/", "TEST LOG JSON");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadSummary();            
            }
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(btnDownloadBarang);
        }
        private void LoadSummary()
        {
            DbOfficialDCI.LoadParameter();
            DbOfficialCeisa.LoadParameter();
            try
            {
                string query = $@"select top 1000
                                a.id as [ID],
                                gateway as [Gateway],
                                (select distinct nomorDokumen from dhl_cs_dokumen d
                                where d.hawb = a.HAWB and d.kodeDokumen = 741) as MAWB,
                                (select distinct convert(char(10),tanggalDokumen,126) from dhl_cs_dokumen d
                                where d.hawb = a.HAWB and d.kodeDokumen = 741) as [Tgl MAWB],
                                a.HAWB,
                                (select distinct convert(char(10),tanggalDokumen,126) from dhl_cs_dokumen d
                                where d.hawb = a.HAWB and d.kodeDokumen = 740) as [Tgl HAWB],
                                (select distinct namaEntitas from dhl_cs_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 9) as [Nama Pengirim],
                                (select distinct kodeNegara from dhl_cs_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 9) as [Neg Pengirim],
                                'NPWP' as [Type Identitas],
                                (select distinct nomorIdentitas from dhl_cs_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 1) as [No Identitas],
                                (select distinct namaEntitas from dhl_cs_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 1) as [Nama Penerima],
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
                                convert(varchar,update_time,120) as [Update Time]
                                from dhl_cs_header a
                                order by [ID] desc";
                DataTable dt = DbOfficialCeisa.getRecords(query);
                ViewState["ListDataDCI"] = dt;
                GV_DataDCI.DataSource = dt;
                GV_DataDCI.DataBind();
            }
            catch (Exception ex)
            {
                new ArgumentException(ex.Message);
            }
        }
        protected void GV_DataDCI_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "SendData" || e.CommandName == "GetResponse" || e.CommandName == "ViewHistoryResponse" || e.CommandName == "ViewRecord")
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
                        default: //ViewHistoryResponse
                            string[] commandArgs_4 = e.CommandArgument.ToString().Split(new char[] { ',' });
                            string HAWB_4 = commandArgs_4[0];
                            string TGL_HAWB_4 = commandArgs_4[1];
                            string respCode = commandArgs_4[2];
                            if (string.IsNullOrEmpty(respCode)) return;
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

        #region filter search
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string query = $@"select top 1000
                                a.id as [ID],
                                gateway as [Gateway],
                                (select distinct nomorDokumen from dhl_cs_dokumen d
                                where d.hawb = a.HAWB and d.kodeDokumen = 741) as MAWB,
                                (select distinct convert(char(10),tanggalDokumen,126) from dhl_cs_dokumen d
                                where d.hawb = a.HAWB and d.kodeDokumen = 741) as [Tgl MAWB],
                                a.HAWB,
                                (select distinct convert(char(10),tanggalDokumen,126) from dhl_cs_dokumen d
                                where d.hawb = a.HAWB and d.kodeDokumen = 740) as [Tgl HAWB],
                                (select distinct namaEntitas from dhl_cs_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 9) as [Nama Pengirim],
                                (select distinct kodeNegara from dhl_cs_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 9) as [Neg Pengirim],
                                'NPWP' as [Type Identitas],
                                (select distinct nomorIdentitas from dhl_cs_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 1) as [No Identitas],
                                (select distinct namaEntitas from dhl_cs_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 1) as [Nama Penerima],
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
                                convert(varchar,update_time,120) as [Update Time]
                                from dhl_cs_header a";
                string hawb = srcHAWB.Text.Trim();
                string tgl_hawb = srcTglHawb.Text.Trim();
                string no_aju = srcNoAju.Text.Trim();
                string status = srcStatus.Text;
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
                    parameter = $@" where a.HAWB in ({hawb}) order by [ID] desc";
                    query += parameter;
                }
                else if (string.IsNullOrEmpty(tgl_hawb) == false)
                {
                    parameter = $@" where a.TGL_HAWB = '{tgl_hawb}' order by [ID] desc";
                    query += parameter;
                }
                else if (string.IsNullOrEmpty(no_aju) == false)
                {
                    parameter = $@" where nomorAju like '%{no_aju}%' order by [ID] desc";
                    query += parameter;
                }
                else if (isBlankAju.Checked == true)
                {
                    parameter = $@" where nomorAju = '' order by [ID] desc";
                    query += parameter;
                }
                else if (string.IsNullOrEmpty(status) == false)
                {
                    parameter = $@" where (select distinct g.latest_response_note from dhl_cs_response_header g
                                    where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) like '%{status}%' order by [ID] desc";
                    query += parameter;
                }
                else if (isBlankStatus.Checked == true)
                {
                    parameter = $@" where (select distinct g.latest_response_note from dhl_cs_response_header g
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
                            order by WAKTU_RESPONSE desc";
                    DataTable tbl = DbOfficialCeisa.getRecords(query);
                    GV_HistoryRejectNotes.DataSource = tbl;
                    GV_HistoryRejectNotes.DataBind();

                    ShowModalHistoryReject();
                }
                else
                {
                    GV_HistoryResponse.DataSource = dt;
                    GV_HistoryResponse.DataBind();
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
                                ndpbm as [NDPBM],
                                netto as [NETTO],
                                posTarif as [POS TARIF],
                                seriBarang as [SERI BARANG],
                                (select tarif from dhl_cs_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'BM' and seriBarang = barang.seriBarang) as [BM TARIF],
                                (select tarif from dhl_cs_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'CUKAI' and seriBarang = barang.seriBarang) as [CUKAI TARIF],
                                (select tarif from dhl_cs_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'PPN' and seriBarang = barang.seriBarang) as [PPN TARIF],
                                (select tarif from dhl_cs_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'PPNBM' and seriBarang = barang.seriBarang) as [PPNBM TARIF],
                                (select tarif from dhl_cs_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                                and kodeJenisPungutan = 'PPH' and seriBarang = barang.seriBarang) as [PPH TARIF],
                                uraian as [URAIAN]
                                from dhl_cs_barang barang
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
                DataTable tblBarangTarif = DbOfficialCeisa.getRecords(query);
                ViewState["ListBarangTarif"] = tblBarangTarif;
                GV_BarangTarif.DataSource = tblBarangTarif;
                GV_BarangTarif.DataBind();

                ShowModalBarang();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        protected void GV_BarangTarif_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_BarangTarif.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)ViewState["ListBarangTarif"];
            GV_BarangTarif.DataSource = dt;
            GV_BarangTarif.DataBind();
        }
        protected void GV_Barang_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_Barang.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)ViewState["ListBarang"];
            GV_Barang.DataSource = dt;
            GV_Barang.DataBind();
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
                        DataTable dt = (DataTable)ViewState["ListDataDCI"];
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                string NO_AJU = row["No Aju"].ToString();
                                if (string.IsNullOrEmpty(NO_AJU)) continue;

                                string HAWB = row["HAWB"].ToString();
                                string TGL_HAWB = row["Tgl HAWB"].ToString();

                                listAju.Add($"{NO_AJU}|{HAWB}|{TGL_HAWB}");
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


                if (listAju.Count > 0)
                {
                    //get user password for request token
                    string query = $@"select username, password from dhl_cs_settings where gateway = '{gateway}'";
                    DataRow row = DbOfficialCeisa.getRow(query);
                    if (row == null)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Please fill username & password on gateway settings.');", true);
                        return;
                    }

                    string userCeisa = row["username"].ToString();
                    string passwordCeisa = row["password"].ToString();

                    ResponseToken response = JsonConvert.DeserializeObject<ResponseToken>(GetRequestToken(userCeisa, passwordCeisa));
                    if (response.status != "success")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{response.message}');", true);
                        return;
                    }

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
                        requestObjGet.Headers.Add("Authorization", $"Bearer {response.item.access_token}");

                        #if DEBUG
                        #else
                        WebProxy myproxy = new WebProxy("185.46.212.32", 10123);
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
                                    foreach (var item in responseStatusAju.dataRespon)
                                    {
                                        listQueryRespon.Add($@"insert into dhl_cs_response_data values (
                                                    '{HAWB}','{TGL_HAWB}','{no_aju}',
                                                    '{item.kodeRespon}','{item.nomorDaftar}',
                                                    '{item.tanggalDaftar}','{item.nomorRespon}',
                                                    '{item.tanggalRespon}','{item.waktuRespon}',
                                                    '{item.keterangan}')");

                                        if (item.keterangan == "REJECT")
                                        {
                                            foreach (var items in item.pesan)
                                            {
                                                listQueryRespon.Add($@"insert into dhl_cs_response_pesan values (
                                                            '{HAWB}','{TGL_HAWB}','{no_aju}',
                                                            '{items}','{item.waktuRespon}')");
                                            }

                                            isRejected = true;
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

                                        if (DbOfficialCeisa.runCommand(listQueryRespon) == 0)
                                            logger.Log($"insert data response success");
                                    }
                                    #endregion

                                    #region dataHeader
                                    query = $@"select top 1 * from dhl_cs_response_header where NO_AJU = '{no_aju}'";
                                    row = DbOfficialCeisa.getRow(query);

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
                string query = $@"select username, password from dhl_cs_settings where gateway = '{gateway}'";
                DataRow row = DbOfficialCeisa.getRow(query);
                if (row == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Gateway not found, please fill on gateway settings.');", true);                    
                    return;
                }

                string userCeisa = row["username"].ToString();
                string passwordCeisa = row["password"].ToString();

                ResponseToken response = JsonConvert.DeserializeObject<ResponseToken>(GetRequestToken(userCeisa, passwordCeisa));
                if (response.status != "success")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{response.message}');", true);                    
                    return;
                }

                //string Url = $@"https://apis-gw.beacukai.go.id/openapi/status/{nomorAju}";
                string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/status/{nomorAju}";
                Uri myUri = new Uri(Url, UriKind.Absolute);

                WebRequest requestObjGet = WebRequest.Create(myUri);
                requestObjGet.Method = "GET";
                requestObjGet.Headers.Add("Authorization", $"Bearer {response.item.access_token}");

                #if DEBUG
                #else
                WebProxy myproxy = new WebProxy("185.46.212.32", 10123);
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
                            List<string> listQueryRespon= new List<string>();
                            bool isRejected = false;
                            foreach (var item in responseStatusAju.dataRespon)
                            {
                                listQueryRespon.Add($@"insert into dhl_cs_response_data values (
                                                    '{HAWB}','{TGL_HAWB}','{nomorAju}',
                                                    '{item.kodeRespon}','{item.nomorDaftar}',
                                                    '{item.tanggalDaftar}','{item.nomorRespon}',
                                                    '{item.tanggalRespon}','{item.waktuRespon}',
                                                    '{item.keterangan}')");

                                if (item.keterangan == "REJECT")
                                {
                                    foreach (var items in item.pesan)
                                    {
                                        listQueryRespon.Add($@"insert into dhl_cs_response_pesan values (
                                                            '{HAWB}','{TGL_HAWB}','{nomorAju}',
                                                            '{items}','{item.waktuRespon}')");
                                    }

                                    isRejected = true;
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

                                if (DbOfficialCeisa.runCommand(listQueryRespon) == 0)
                                    logger.Log($"insert data response success");
                            }
                            #endregion

                            #region dataHeader
                            query = $@"select top 1 * from dhl_cs_response_header where NO_AJU = '{nomorAju}'";
                            row = DbOfficialCeisa.getRow(query);

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
                Response.Redirect("~/Views/Request.aspx");
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
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
                    CloseModalLoader();
                    return;
                }
                
                
                if (listHAWB.Count > 0)
                {
                    //get user password for request token
                    string query = $@"select username, password from dhl_cs_settings where gateway = '{gateway}'";
                    DataRow row = DbOfficialCeisa.getRow(query);
                    if (row == null)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Please fill username & password on gateway settings.');", true);
                        return;
                    }

                    string userCeisa = row["username"].ToString();
                    string passwordCeisa = row["password"].ToString();

                    ResponseToken response = JsonConvert.DeserializeObject<ResponseToken>(GetRequestToken(userCeisa, passwordCeisa));
                    if (response.status != "success")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{response.message}');", true);
                        return;
                    }

                    int countRecord = 0;
                    foreach (var item in listHAWB)
                    {
                        string[] dr = item.Split('|');
                        countRecord += SendData(dr[0], dr[1], response.item.access_token, response.item.id_token, gateway);
                    }
                    logger.Log($"Send data success, {countRecord} nomor aju created");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Send data success, {countRecord} aju number created');", true);
                    CloseModalLoader();
                    LoadSummary();
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
                if (string.IsNullOrEmpty(sendHAWB.Text))
                {
                    ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "alert", $"alert('Please input HAWB number');", true);
                    return;
                }

                List<string> listHAWB = new List<string>();

                string[] arr = sendHAWB.Text.Trim().Split('\n');
                foreach (var item in arr)
                {
                    listHAWB.Add($"'{item.Trim()}'");
                }
                string hawb = String.Join(",", listHAWB);

                //generate list aju & send request
                string query = $@"select HAWB, TGL_HAWB
                                from dhl_cs_header
                                where nomorAju = '' and HAWB in ({hawb})";
                DataTable tbl = DbOfficialCeisa.getRecords(query);
                if (tbl.Rows.Count > 0)
                {
                    //get user password for request token
                    query = $@"select username, password from dhl_cs_settings where gateway = '{gateway}'";
                    DataRow row = DbOfficialCeisa.getRow(query);
                    if (row == null)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Please fill username & password on gateway settings.');", true);
                        return;
                    }

                    string userCeisa = row["username"].ToString();
                    string passwordCeisa = row["password"].ToString();

                    ResponseToken response = JsonConvert.DeserializeObject<ResponseToken>(GetRequestToken(userCeisa, passwordCeisa));
                    if (response.status != "success")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{response.message}');", true);
                        return;
                    }

                    int countRecord = 0;
                    foreach (DataRow dr in tbl.Rows)
                    {
                        countRecord += SendData(dr["HAWB"].ToString(), dr["TGL_HAWB"].ToString(), response.item.access_token, response.item.id_token, gateway);
                    }
                    logger.Log($"Send data success, {countRecord} nomor aju created");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Send data success, {countRecord} aju number created');", true);
                    CloseModalLoader();
                    CloseModalSend();
                    LoadSummary();
                }
                else
                {
                    CloseModalLoader();
                    CloseModalSend();
                }
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
                string query = $@"select username, password from dhl_cs_settings where update_by = '{username}'";
                DataRow row = DbOfficialCeisa.getRow(query);
                if (row == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Please fill username & password on data settings.');", true);
                    return;
                }

                string userCeisa = row["username"].ToString();
                string passwordCeisa = row["password"].ToString();

                ResponseToken response = JsonConvert.DeserializeObject<ResponseToken>(GetRequestToken(userCeisa, passwordCeisa));
                if (response.status != "success")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('{response.message}');", true);
                    return;
                }

                string queryHeader = $@"select
                                    idPengguna, jabatanTtd, kodeKantor,
                                    kodeTps, kotaTtd, namaTtd,
                                    asuransi, bruto, fob, freight,
                                    kodeIncoterm, kodePelMuat, kodePelTujuan, kodeValuta, 
                                    ndpbm, netto, nilaiBarang, nomorBc11, 
                                    posBc11, subPosBc11, tanggalBc11, tanggalTiba
                                    from dhl_cs_header
                                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                DataRow rowHeader = DbOfficialCeisa.getRow(queryHeader);                          

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = new NullToEmptyListResolver();
                settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                //settings.Formatting = Formatting.Indented;

                //Model Request AJU
                #region header
                RequestAju req = new RequestAju();
                req.asalData = "S";
                req.asuransi = Convert.ToDouble(rowHeader["asuransi"]);
                req.bruto = Convert.ToDouble(rowHeader["bruto"]);
                req.cif = Convert.ToDouble(rowHeader["fob"]);
                req.disclaimer = "1";
                req.kodeJenisProsedur = "1";
                req.kodeJenisImpor = "1";
                req.kodeJenisEkspor = "";
                req.flagVd = "T";
                req.fob = Convert.ToDouble(rowHeader["fob"]);
                req.freight = Convert.ToDouble(rowHeader["freight"]);
                req.hargaPenyerahan = 0;
                req.idPengguna = rowHeader["idPengguna"].ToString();
                req.jabatanTtd = rowHeader["jabatanTtd"].ToString();
                req.jumlahKontainer = 0;
                req.jumlahTandaPengaman = "";
                req.kodeAsuransi = "LN";
                req.kodeCaraBayar = "1";
                req.kodeDokumen = "20";
                req.kodeIncoterm = rowHeader["kodeIncoterm"].ToString();
                req.kodeJenisNilai = "LAI";
                req.kodeKantor = rowHeader["kodeKantor"].ToString();
                req.kodePelMuat = rowHeader["kodePelMuat"].ToString();
                req.kodePelTujuan = rowHeader["kodePelTujuan"].ToString();
                req.kodeTps = rowHeader["kodeTps"].ToString();
                req.kodeTutupPu = "11";
                req.kodeValuta = rowHeader["kodeValuta"].ToString();
                req.kotaTtd = rowHeader["kotaTtd"].ToString();
                req.namaTtd = rowHeader["namaTtd"].ToString();
                req.ndpbm = Convert.ToDouble(rowHeader["ndpbm"]);
                req.netto = Convert.ToDouble(rowHeader["netto"]);
                req.nilaiBarang = Convert.ToDouble(rowHeader["nilaiBarang"]);
                req.nilaiIncoterm = 0;
                req.nilaiMaklon = 0;
                req.nomorAju = GenerateAju();
                req.nomorBc11 = rowHeader["nomorBc11"].ToString();
                req.posBc11 = rowHeader["posBc11"].ToString();
                req.seri = "";
                req.subposBc11 = rowHeader["subPosBc11"].ToString() + "0000";
                req.tanggalAju = DateTime.Now.ToString("yyyy-MM-dd");
                req.tanggalBc11 = GetDateJson(rowHeader["tanggalBc11"].ToString());
                req.tanggalTiba = GetDateJson(rowHeader["tanggalTiba"].ToString());
                req.tanggalTtd = DateTime.Now.ToString("yyyy-MM-dd");
                req.totalDanaSawit = 0;
                req.volume = 0;
                req.biayaTambahan = 0;
                req.biayaPengurang = 0;
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
                    string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/document";
                    //string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/document?isFinal=true";
                    Uri myUri = new Uri(Url, UriKind.Absolute);

                    WebRequest requestObjPost = WebRequest.Create(myUri);
                    requestObjPost.Method = "POST";
                    requestObjPost.ContentType = "application/json";
                    requestObjPost.Headers.Add("Authorization",$"Bearer {response.item.access_token}");
                    requestObjPost.Headers.Add("Origin", "dhl.com");
                    requestObjPost.Headers.Add("Beacukai-Api-Key", $"{response.item.id_token}");

                    #if DEBUG
                    #else
                    WebProxy myproxy = new WebProxy("185.46.212.32", 10123);
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
                            logger.Log($"response [kirim dokumen - {req.nomorAju}]" + Environment.NewLine + respJson);

                            List<string> listQuery = new List<string>();
                            ResponseAju responseAju = JsonConvert.DeserializeObject<ResponseAju>(respJson);
                            if (responseAju.status == "OK")
                            {
                                //update nomor aju
                                int sequenceAju = Convert.ToInt32(req.nomorAju.Substring(req.nomorAju.Length - 6));
                                string tanggalAju = GetDateAju(req.nomorAju.Substring(req.nomorAju.Length - 14, 8));
                                listQuery.Add($@"update dhl_cs_sequence_aju set
                                                sequence_aju = {sequenceAju} where
                                                gateway = '{gateway}'");
                                listQuery.Add($@"update dhl_cs_header set
                                                nomorAju = '{req.nomorAju}',
                                                tanggalAju = '{tanggalAju}'
                                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'");
                                DbOfficialCeisa.runCommand(listQuery);
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Send data success, no.aju {req.nomorAju}')", true);
                            }
                            else
                            {
                                if (responseAju.status.ToLower() == "failed")
                                {
                                    if (responseAju.message.ToLower() == "gagal, nomor aju sudah ada.")
                                    {
                                        //update nomor aju
                                        int sequenceAju = Convert.ToInt32(req.nomorAju.Substring(req.nomorAju.Length - 6));
                                        query = $@"update dhl_cs_sequence_aju set
                                        sequence_aju = {sequenceAju} where
                                        gateway = '{gateway}'";
                                        DbOfficialCeisa.runCommand(query);

                                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Send data failed, no.aju {req.nomorAju} already exists, please try send again.')", true);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Send data failed, {response.message.ToLower()}, please try send again.')", true);
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

                LoadSummary();
                #endregion

            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        protected int SendData(string HAWB, string TGL_HAWB, string access_token, string id_token, string gateway)
        {
            int retval = 0;
            try
            {
                string queryHeader = $@"select
                                    idPengguna, jabatanTtd, kodeKantor,
                                    kodeTps, kotaTtd, namaTtd,
                                    asuransi, bruto, fob, freight,
                                    kodeIncoterm, kodePelMuat, kodePelTujuan, kodeValuta, 
                                    ndpbm, netto, nilaiBarang, nomorBc11, 
                                    posBc11, subPosBc11, tanggalBc11, tanggalTiba
                                    from dhl_cs_header
                                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                DataRow rowHeader = DbOfficialCeisa.getRow(queryHeader);

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = new NullToEmptyListResolver();
                settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                //settings.Formatting = Formatting.Indented;

                //Model Request AJU
                #region header
                RequestAju req = new RequestAju();
                req.asalData = "S";
                req.asuransi = Convert.ToDouble(rowHeader["asuransi"]);
                req.bruto = Convert.ToDouble(rowHeader["bruto"]);
                req.cif = Convert.ToDouble(rowHeader["fob"]);
                req.disclaimer = "1";
                req.kodeJenisProsedur = "1";
                req.kodeJenisImpor = "1";
                req.kodeJenisEkspor = "";
                req.flagVd = "T";
                req.fob = Convert.ToDouble(rowHeader["fob"]);
                req.freight = Convert.ToDouble(rowHeader["freight"]);
                req.hargaPenyerahan = 0;
                req.idPengguna = rowHeader["idPengguna"].ToString();
                req.jabatanTtd = rowHeader["jabatanTtd"].ToString();
                req.jumlahKontainer = 0;
                req.jumlahTandaPengaman = "";
                req.kodeAsuransi = "LN";
                req.kodeCaraBayar = "1";
                req.kodeDokumen = "20";
                req.kodeIncoterm = rowHeader["kodeIncoterm"].ToString();
                req.kodeJenisNilai = "LAI";
                req.kodeKantor = rowHeader["kodeKantor"].ToString();
                req.kodePelMuat = rowHeader["kodePelMuat"].ToString();
                req.kodePelTujuan = rowHeader["kodePelTujuan"].ToString();
                req.kodeTps = rowHeader["kodeTps"].ToString();
                req.kodeTutupPu = "11";
                req.kodeValuta = rowHeader["kodeValuta"].ToString();
                req.kotaTtd = rowHeader["kotaTtd"].ToString();
                req.namaTtd = rowHeader["namaTtd"].ToString();
                req.ndpbm = Convert.ToDouble(rowHeader["ndpbm"]);
                req.netto = Convert.ToDouble(rowHeader["netto"]);
                req.nilaiBarang = Convert.ToDouble(rowHeader["nilaiBarang"]);
                req.nilaiIncoterm = 0;
                req.nilaiMaklon = 0;
                req.nomorAju = GenerateAju();
                req.nomorBc11 = rowHeader["nomorBc11"].ToString();
                req.posBc11 = rowHeader["posBc11"].ToString();
                req.seri = "";
                req.subposBc11 = rowHeader["subPosBc11"].ToString() + "0000";
                req.tanggalAju = DateTime.Now.ToString("yyyy-MM-dd");
                req.tanggalBc11 = GetDateJson(rowHeader["tanggalBc11"].ToString());
                req.tanggalTiba = GetDateJson(rowHeader["tanggalTiba"].ToString());
                req.tanggalTtd = DateTime.Now.ToString("yyyy-MM-dd");
                req.totalDanaSawit = 0;
                req.volume = 0;
                req.biayaTambahan = 0;
                req.biayaPengurang = 0;
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
                    string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/document";
                    //string Url = $@"https://apisdev-gw.beacukai.go.id/openapi/document?isFinal=true";
                    Uri myUri = new Uri(Url, UriKind.Absolute);

                    WebRequest requestObjPost = WebRequest.Create(myUri);
                    requestObjPost.Method = "POST";
                    requestObjPost.ContentType = "application/json";
                    requestObjPost.Headers.Add("Authorization", $"Bearer {access_token}");
                    requestObjPost.Headers.Add("Origin", "dhl.com");
                    requestObjPost.Headers.Add("Beacukai-Api-Key", $"{id_token}");

                    #if DEBUG
                    #else
                    WebProxy myproxy = new WebProxy("185.46.212.32", 10123);
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
                            logger.Log($"response [kirim dokumen - {req.nomorAju}]" + Environment.NewLine + respJson);

                            List<string> listQuery = new List<string>();
                            ResponseAju responseAju = JsonConvert.DeserializeObject<ResponseAju>(respJson);
                            if (responseAju.status == "OK")
                            {
                                //update nomor aju
                                int sequenceAju = Convert.ToInt32(req.nomorAju.Substring(req.nomorAju.Length - 6));
                                string tanggalAju = GetDateAju(req.nomorAju.Substring(req.nomorAju.Length - 14, 8));
                                listQuery.Add($@"update dhl_cs_sequence_aju set
                                                sequence_aju = {sequenceAju} where
                                                gateway = '{gateway}'");
                                listQuery.Add($@"update dhl_cs_header set
                                                nomorAju = '{req.nomorAju}',
                                                tanggalAju = '{tanggalAju}'
                                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'");
                                DbOfficialCeisa.runCommand(listQuery);
                                retval = 1;
                            }
                            else
                            {
                                if (responseAju.status.ToLower() == "failed")
                                {
                                    if (responseAju.message.ToLower() == "gagal, nomor aju sudah ada.")
                                    {
                                        //update nomor aju
                                        int sequenceAju = Convert.ToInt32(req.nomorAju.Substring(req.nomorAju.Length - 6));
                                        query = $@"update dhl_cs_sequence_aju set
                                        sequence_aju = {sequenceAju} where
                                        gateway = '{gateway}'";
                                        DbOfficialCeisa.runCommand(query);

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
                throw new ArgumentException(ex.Message);
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

                retval = $"{kodeAju}{tglAju}{(++sequenceAju).ToString().PadLeft(6, '0')}";
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

                //string Url = $@"https://apisdev-gw.beacukai.go.id/auth-amws/v1/user/login";
                string Url = $@"https://apis-gw.beacukai.go.id/amws/v1/user/login";
                Uri myUri = new Uri(Url, UriKind.Absolute);

                WebRequest requestObjPost = WebRequest.Create(myUri);
                requestObjPost.Method = "POST";
                requestObjPost.ContentType = "application/json";

                #if DEBUG
                #else
                WebProxy myproxy = new WebProxy("185.46.212.32", 10123);
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
        private List<Barang> GetBarang(string HAWB, string TGL_HAWB, double bruto)
        {
            List<Barang> barang = new List<Barang>();
            try
            {
                string queryBarang = $@"select 
                                        asuransi, cif, cifRupiah, fob, freight,
                                        hargaSatuan, jumlahSatuan, 
                                        kodeNegaraAsal, kodeSatuanBarang,
                                        ndpbm, netto, posTarif, seriBarang, uraian
                                        from dhl_cs_barang
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
                        if (rowNetto == 0) rowNetto = Math.Round((double)bruto / tblBarang.Rows.Count, 2);
                        barang.Add(new Barang()
                        {
                            asuransi = Convert.ToDouble(dr["asuransi"]),
                            bruto = 0,
                            cif = Convert.ToDouble(dr["cif"]),
                            cifRupiah = Convert.ToDouble(dr["cifRupiah"]),
                            diskon = 0,
                            fob = Convert.ToDouble(dr["fob"]),
                            freight = Convert.ToDouble(dr["freight"]),
                            hargaEkspor = 0,
                            hargaPatokan = 0,
                            hargaPenyerahan = 0,
                            hargaPerolehan = 0,
                            hargaSatuan = Convert.ToDouble(dr["hargaSatuan"]),
                            hjeCukai = 0,
                            isiPerKemasan = 0,
                            jumlahBahanBaku = 0,
                            jumlahDilekatkan = 0,
                            jumlahKemasan = 1,
                            jumlahPitaCukai = 0,
                            jumlahRealisasi = 0,
                            jumlahSatuan = Convert.ToDouble(dr["jumlahSatuan"]),
                            kapasitasSilinder = 0,
                            kodeJenisKemasan = "PK",
                            kodeKondisiBarang = "1",
                            kodeNegaraAsal = dr["kodeNegaraAsal"].ToString(),
                            kodeSatuanBarang = dr["kodeSatuanBarang"].ToString(),
                            merk = "-",
                            ndpbm = Convert.ToDouble(dr["ndpbm"]),
                            netto = rowNetto,
                            nilaiBarang = 0,
                            nilaiDanaSawit = 0,
                            nilaiDevisa = 0,
                            nilaiTambah = 0,
                            pernyataanLartas = "T",
                            persentaseImpor = 0,
                            posTarif = dr["posTarif"].ToString(),
                            seriBarang = Convert.ToInt32(dr["seriBarang"]),
                            seriBarangDokAsal = "",
                            seriIjin = "",
                            tahunPembuatan = "",
                            tarifCukai = 0,
                            tipe = "-",
                            uraian = dr["uraian"].ToString(),
                            volume = 0,
                            barangTarif = GetBarangTarif(HAWB, TGL_HAWB, Convert.ToInt32(dr["seriBarang"]))
                        });
                    }
                }
            }
            catch { }
            return barang;
        }
        private List<BarangTarif> GetBarangTarif(string HAWB, string TGL_HAWB, int seriBarang)
        {
            List<BarangTarif> barangTarifs = new List<BarangTarif>();
            try
            {
                string queryBarangTarif = $@"select 
                                            kodeJenisPungutan, nilaiBayar, seriBarang, tarif
                                            from dhl_cs_barang_tarif
                                            where HAWB = '{HAWB}' and 
                                            TGL_HAWB = '{TGL_HAWB}' and
                                            seriBarang = '{seriBarang}'";
                DataTable tblBarangTarif = DbOfficialCeisa.getRecords(queryBarangTarif);
                if (tblBarangTarif.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblBarangTarif.Rows)
                    {
                        barangTarifs.Add(new BarangTarif()
                        {
                            kodeJenisTarif = "1",
                            jumlahSatuan = 1,
                            kodeFasilitasTarif = "1",
                            kodeJenisPungutan = dr["kodeJenisPungutan"].ToString(),
                            nilaiBayar = Convert.ToDouble(dr["nilaiBayar"]),
                            seriBarang = Convert.ToInt32(dr["seriBarang"]),
                            tarif = Convert.ToDouble(dr["tarif"]),
                            tarifFasilitas = 0,
                            nilaiFasilitas = 0
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
                                        kodeNegara, seriEntitas
                                        from dhl_cs_entitas
                                        where HAWB = '{HAWB}' and 
                                        TGL_HAWB = '{TGL_HAWB}'
                                        order by seriEntitas";
                DataTable tblEntitas = DbOfficialCeisa.getRecords(queryEntitas);
                if (tblEntitas.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblEntitas.Rows)
                    {
                        Entitas ent = new Entitas();
                        ent.alamatEntitas = dr["alamatEntitas"].ToString();
                        ent.kodeEntitas = dr["kodeEntitas"].ToString();
                        ent.kodeJenisApi = dr["kodeJenisApi"].ToString();
                        ent.kodeJenisIdentitas = dr["kodeJenisIdentitas"].ToString();
                        ent.kodeStatus = dr["kodeStatus"].ToString();
                        ent.namaEntitas = dr["namaEntitas"].ToString();
                        ent.nibEntitas = dr["nibEntitas"].ToString();
                        ent.nomorIdentitas = dr["nomorIdentitas"].ToString();
                        ent.kodeNegara = dr["kodeNegara"].ToString();
                        ent.seriEntitas = dr["seriEntitas"].ToString();

                        Dictionary<string, string> objEntitas = new Dictionary<string, string>();
                        if (ent.kodeEntitas == "1")
                        {
                            objEntitas.Add("alamatEntitas", ent.alamatEntitas);
                            objEntitas.Add("kodeEntitas", ent.kodeEntitas);
                            objEntitas.Add("kodeJenisApi", ent.kodeJenisApi);
                            objEntitas.Add("kodeJenisIdentitas", ent.kodeJenisIdentitas);
                            objEntitas.Add("kodeStatus", ent.kodeStatus);
                            objEntitas.Add("namaEntitas", ent.namaEntitas);
                            objEntitas.Add("nibEntitas", ent.nibEntitas);
                            objEntitas.Add("nomorIdentitas", ent.nomorIdentitas);
                            objEntitas.Add("seriEntitas", ent.seriEntitas);
                        }
                        else if (ent.kodeEntitas == "7" || ent.kodeEntitas == "11" || ent.kodeEntitas == "4")
                        {
                            objEntitas.Add("alamatEntitas", ent.alamatEntitas);
                            objEntitas.Add("kodeEntitas", ent.kodeEntitas);
                            objEntitas.Add("kodeJenisIdentitas", ent.kodeJenisIdentitas);
                            objEntitas.Add("namaEntitas", ent.namaEntitas);
                            objEntitas.Add("nomorIdentitas", ent.nomorIdentitas);
                            objEntitas.Add("seriEntitas", ent.seriEntitas);
                        }
                        else
                        {
                            //9 or 10
                            objEntitas.Add("alamatEntitas", ent.alamatEntitas);
                            objEntitas.Add("kodeEntitas", ent.kodeEntitas);
                            objEntitas.Add("kodeNegara", ent.kodeNegara);
                            objEntitas.Add("namaEntitas", ent.namaEntitas);
                            objEntitas.Add("seriEntitas", ent.seriEntitas);
                        }

                        entitas.Add(objEntitas);
                    }
                }
            }
            catch { }
            return entitas;
        }
        private List<Kemasan> GetKemasan(string HAWB, string TGL_HAWB)
        {
            List<Kemasan> kemasan = new List<Kemasan>();
            try
            {
                string queryKemasan = $@"select 
                                        jumlahKemasan, kodeJenisKemasan,
                                        merkKemasan, seriKemasan
                                        from dhl_cs_kemasan
                                        where HAWB = '{HAWB}' and 
                                        TGL_HAWB = '{TGL_HAWB}'";
                DataTable tblKemasan = DbOfficialCeisa.getRecords(queryKemasan);
                if (tblKemasan.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblKemasan.Rows)
                    {
                        kemasan.Add(new Kemasan() { 
                            jumlahKemasan = Convert.ToInt32(dr["jumlahKemasan"]),
                            kodeJenisKemasan = dr["kodeJenisKemasan"].ToString(),
                            merkKemasan = dr["merkKemasan"].ToString(),
                            seriKemasan = Convert.ToInt32(dr["seriKemasan"])
                        });
                    }
                }
            }
            catch { }
            return kemasan;
        }
        private List<Dokumen> GetDokumen(string HAWB, string TGL_HAWB)
        {
            List<Dokumen> dokumen = new List<Dokumen>();
            try
            {
                string queryDokumen = $@"select
                                        kodeDokumen, nomorDokumen,
                                        seriDokumen, tanggalDokumen
                                        from dhl_cs_dokumen
                                        where HAWB = '{HAWB}' and 
                                        TGL_HAWB = '{TGL_HAWB}'";
                DataTable tblDokumen = DbOfficialCeisa.getRecords(queryDokumen);
                if (tblDokumen.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblDokumen.Rows)
                    {
                        dokumen.Add(new Dokumen()
                        {
                            kodeDokumen = dr["kodeDokumen"].ToString(),
                            nomorDokumen = dr["nomorDokumen"].ToString(),
                            seriDokumen = dr["seriDokumen"].ToString(),
                            tanggalDokumen = GetDateJson(dr["tanggalDokumen"].ToString())
                        });
                    }
                }
            }
            catch { }
            return dokumen;
        }
        private List<Pengangkut> GetPengangkut(string HAWB, string TGL_HAWB)
        {
            List<Pengangkut> pengangkut = new List<Pengangkut>();
            try
            {
                string queryPengangkut = $@"select
                                        kodeBendera, namaPengangkut, nomorPengangkut,
                                        kodeCaraAngkut, seriPengangkut
                                        from dhl_cs_pengangkut
                                        where HAWB = '{HAWB}' and 
                                        TGL_HAWB = '{TGL_HAWB}'";
                DataTable tblPengangkut = DbOfficialCeisa.getRecords(queryPengangkut);
                if (tblPengangkut.Rows.Count > 0)
                {
                    foreach (DataRow dr in tblPengangkut.Rows)
                    {
                        pengangkut.Add(new Pengangkut()
                        {
                            //kodeBendera = dr["kodeBendera"].ToString(),
                            kodeBendera = "SG",
                            namaPengangkut = dr["namaPengangkut"].ToString(),
                            nomorPengangkut = dr["nomorPengangkut"].ToString(),
                            kodeCaraAngkut = dr["kodeCaraAngkut"].ToString(),
                            seriPengangkut = dr["seriPengangkut"].ToString()
                        });
                    }
                }
            }
            catch { }
            return pengangkut;
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
                
                int result = 0;
                if (string.IsNullOrEmpty(hawb) == false)
                    result = LoadDataDCI(hawb, gateway, update_by);
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
                string query = $@"select hawb from dhl_cs_header where hawb in ({hawb})";
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
                string queryHeader = $@"delete from dhl_cs_header where hawb in";
                string queryBrg = $@"delete from dhl_cs_barang where hawb in";
                string queryBrgTarif = $@"delete from dhl_cs_barang_tarif where hawb in";
                string queryKemasan = $@"delete from dhl_cs_kemasan where hawb in";
                string queryEntitas = $@"delete from dhl_cs_entitas where hawb in";
                string queryDokumen = $@"delete from dhl_cs_dokumen where hawb in";
                string queryPengangkut = $@"delete from dhl_cs_pengangkut where hawb in";

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
                    }
                }

                DbOfficialCeisa.runCommand(listDelete);
            }
            catch { }
        }
        private int LoadDataDCI(string hawb, string gateway, string update_by)
        {
            int retval = 0;
            try
            {
                List<string> listQuery = new List<string>();
                string query = string.Empty;

                #region load setting [kode kantor, kode gudang]
                query = $@"select kode_kantor, kode_gudang from dhl_cs_sequence_aju where gateway = '{gateway}'";
                DataRow rowSetting = DbOfficialCeisa.getRow(query);

                query = $"select username, jabatan, kota_ttd, nama_ttd from dhl_cs_settings where gateway = '{gateway}'";
                DataRow rowHeader = DbOfficialCeisa.getRow(query);
                #endregion

                #region load data header
                query = $@"select
                        GATEWAY as [gateway],
                        b.HAWB,
                        ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB) as [TGLHAWB],
                        'S' as [asalData],
                        ASURANSI as [asuransi],
                        QTY1 as [bruto],
                        0 as [cif],
                        1 as [disclaimer],
                        1 as [kodeJenisProsedur],
                        1 as [kodeJenisImpor],
                        '' as [kodeJenisEkspor],
                        'T' as [flagVd],
                        FOBVAL as [fob],
                        FREIGHT as [freight],
                        0 as [hargaPenyerahan],
                        '{rowHeader["username"]}' as [idPengguna],
                        '{rowHeader["jabatan"]}' as [jabatanTtd],
                        0 as [jumlahKontainer],
                        ''  as [jumlahTandaPengaman],
                        'LN' as [kodeAsuransi],
                        1 as [kodeCaraBayar],
                        '20' as [kodeDokumen],
                        INCOTERM as [kodeIncoterm],
                        'NTR' as [kodeJenisNilai],
                        'kode kantor ceisa' as [kodeKantor],
                        KODENEGARAPENGIRIM + SHPORIGIN as [kodePelMuat],
                        KODENEGARAPENERIMA + SHPDESTINATION as [kodePelTujuan],
                        'kode gudang ceisa' as [kodeTps],
                        '11' as [kodeTutupPu],
                        VALUTA as [kodeValuta],
                        '{rowHeader["kota_ttd"]}' as [kotaTtd],
                        '{rowHeader["nama_ttd"]}' as [namaTtd],
                        NILAICURRENCY as [ndpbm],
                        QTY1 as [netto],
                        NILAICIFRP as [nilaiBarang],
                        0 as [nilaiIncoterm],
                        0 as [nilaiMaklon],
                        ISNULL(s.NOMORAJU,'') as [nomorAju],
                        isnull(hd.NOBC11,'') as [nomorBc11],
                        dt.NOPOS as [posBc11],
                        '' as seri,
                        dt.NOSUBPOS as [subPosBc11],
                        '' as [tanggalAju],
                        isnull(hd.TGLBC11,'') as [tanggalBc11],
                        isnull(hd.TGLTIBA,'') as [tanggalTiba],
                        '' as [tanggalTtd],
                        0 as [totalDanaSawit],
                        0 as [volume],
                        0 as [biayaTambahan],
                        0 as [biayaPengurang]
                        from BILLINGREPORT b
                        left join SPPB s on b.HAWB = s.HAWB and b.TGLHAWB = s.TGLHAWB 
                        left join DETIL dt on b.HAWB = NOHOSTBLAWB
                        left join HEADER hd on dt.NOMASTERBLAWB = hd.MAWB
                        where GATEWAY = '{gateway}' and
                        b.HAWB in ({hawb})";
                DataTable dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        listQuery.Add($@"insert into dhl_cs_header values (
                                        '{dr["gateway"].ToString().Trim()}',
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{dr["asalData"]}',
                                        '{dr["asuransi"]}',
                                        '{dr["bruto"]}',
                                        '{dr["cif"]}',
                                        '{dr["disclaimer"]}',
                                        '{dr["kodeJenisProsedur"]}',
                                        '{dr["kodeJenisImpor"]}',
                                        '{dr["kodeJenisEkspor"]}',
                                        '{dr["flagVd"]}',
                                        '{dr["fob"]}',
                                        '{dr["freight"]}',
                                        '{dr["hargaPenyerahan"]}',
                                        '{dr["idPengguna"]}',
                                        '{dr["jabatanTtd"]}',
                                        '{dr["jumlahKontainer"]}',
                                        '{dr["jumlahTandaPengaman"]}',
                                        '{dr["kodeAsuransi"]}',
                                        '{dr["kodeCaraBayar"]}',
                                        '{dr["kodeDokumen"]}',
                                        '{dr["kodeIncoterm"]}',
                                        '{dr["kodeJenisNilai"]}',
                                        '{rowSetting["kode_kantor"]}',
                                        '{dr["kodePelMuat"]}',
                                        '{dr["kodePelTujuan"]}',
                                        '{rowSetting["kode_gudang"]}',
                                        '{dr["kodeTutupPu"]}',
                                        '{dr["kodeValuta"]}',
                                        '{dr["kotaTtd"]}',
                                        '{dr["namaTtd"]}',
                                        '{dr["ndpbm"]}',
                                        '{dr["netto"]}',
                                        '{dr["nilaiBarang"]}',
                                        '{dr["nilaiIncoterm"]}',
                                        '{dr["nilaiMaklon"]}',
                                        '{dr["nomorAju"]}',
                                        '{dr["nomorBc11"]}',
                                        '{dr["posBc11"]}',
                                        '{dr["seri"]}',
                                        '{dr["subPosBc11"]}',
                                        '{GetDate(dr["tanggalAju"].ToString())}',
                                        '{GetDate(dr["tanggalBc11"].ToString())}',
                                        '{GetDate(dr["tanggalTiba"].ToString())}',
                                        '{GetDate(dr["tanggalTtd"].ToString())}',
                                        '{dr["totalDanaSawit"]}',
                                        '{dr["volume"]}',
                                        '{dr["biayaTambahan"]}',
                                        '{dr["biayaPengurang"]}',
                                        '{update_by}',
                                        '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff")}'
                                        )");
                    }

                    //if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    //    retval = listQuery.Count;
                }
                #endregion

                #region load data barang
                query = $@"select
                        db.HAWB,
                        ISNULL(dt.TGLHOSTBLAWB,db.TGLHAWB) as [TGLHAWB],
                        db.ASURANSI as [asuransi],
                        0 as [bruto],
                        db.FOB as [cif],
                        db.HARGA as [cifRupiah],
                        0 as [diskon],
                        db.FOB as [fob],
                        db.FREIGHT as [freight],
                        0 as [hargaEkspor],
                        0 as [hargaPatokan],
                        0 as [hargaPenyerahan],
                        0 as [hargaPerolehan],
                        format((db.FOB / db.QTY),'N2') as [hargaSatuan],
                        0 as [hjeCukai],
                        0 as [isiPerKemasan],
                        0 as [jumlahBahanBaku],
                        0 as [jumlahDilekatkan],
                        1 as [jumlahKemasan],
                        0 as [jumlahPitaCukai],
                        0 as [jumlahRealisasi],
                        db.QTY as [jumlahSatuan],
                        0 as [kapasitasSilinder],
                        'PK' as [kodeJenisKemasan],
                        1 as [kodeKondisiBarang],
                        db.KODENEGARAASAL as [kodeNegaraAsal],
                        db.SATUAN as [kodeSatuanBarang],
                        '-' as [merk],
                        b.NILAICURRENCY as [ndpbm],
                        db.NetWeight as [netto],
                        0 as [nilaiBarang],
                        0 as [nilaiDanaSawit],
                        0 as [nilaiDevisa],
                        0 as [nilaiTambah],
                        'T' as [pernyataanLartas],
                        0 as [persentaseImpor],
                        db.HSCODE as [posTarif],
                        0 as [saldoAkhir],
                        0 as [saldoAwal],
                        db.SHORT as [seriBarang],
                        '' as [seriBarangDokAsal],
                        '' as [seriIjin],
                        '' as [tahunPembuatan],
                        0 as [tarifCukai],
                        '-' as [tipe],
                        db.NAMABARANG as [uraian],
                        0 as [volume]
                        from DETAILBARANGBILLINGREPORT db
                        left join BILLINGREPORT b on db.HAWB = b.HAWB
                        left join DETIL dt on db.HAWB = dt.NOHOSTBLAWB
                        where GATEWAY = '{gateway}' and
                        db.HAWB in ({hawb})";
                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        listQuery.Add($@"insert into dhl_cs_barang values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{dr["asuransi"]}',
                                        '{dr["bruto"]}',
                                        '{dr["cif"]}',
                                        '{dr["cifRupiah"]}',
                                        '{dr["diskon"]}',
                                        '{dr["fob"]}',
                                        '{dr["freight"]}',
                                        '{dr["hargaEkspor"]}',
                                        '{dr["hargaPatokan"]}',
                                        '{dr["hargaPenyerahan"]}',
                                        '{dr["hargaPerolehan"]}',
                                        '{dr["hargaSatuan"].ToString().Replace(",", "")}',
                                        '{dr["hjeCukai"]}',
                                        '{dr["isiPerKemasan"]}',
                                        '{dr["jumlahBahanBaku"]}',
                                        '{dr["jumlahDilekatkan"]}',
                                        '{dr["jumlahKemasan"]}',
                                        '{dr["jumlahPitaCukai"]}',
                                        '{dr["jumlahRealisasi"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '{dr["kapasitasSilinder"]}',
                                        '{dr["kodeJenisKemasan"]}',
                                        '{dr["kodeKondisiBarang"]}',
                                        '{dr["kodeNegaraAsal"]}',
                                        '{dr["kodeSatuanBarang"]}',
                                        '{dr["merk"]}',
                                        '{dr["ndpbm"]}',
                                        '{dr["netto"]}',
                                        '{dr["nilaiBarang"]}',
                                        '{dr["nilaiDanaSawit"]}',
                                        '{dr["nilaiDevisa"]}',
                                        '{dr["nilaiTambah"]}',
                                        '{dr["pernyataanLartas"]}',
                                        '{dr["persentaseImpor"]}',
                                        '{dr["posTarif"]}',
                                        '{dr["saldoAkhir"]}',
                                        '{dr["saldoAwal"]}',
                                        '{dr["seriBarang"]}',
                                        '{dr["seriBarangDokAsal"]}',
                                        '{dr["seriIjin"]}',
                                        '{dr["tahunPembuatan"]}',
                                        '{dr["tarifCukai"]}',
                                        '{dr["tipe"]}',
                                        '{dr["uraian"]}',
                                        '{dr["volume"]}'
                                        )");
                    }

                    //if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    //    retval = listQuery.Count;
                }
                #endregion

                #region load data tarif
                query = $@"select
                        db.HAWB,
                        ISNULL(dt.TGLHOSTBLAWB,db.TGLHAWB) as [TGLHAWB],
                        1 as [kodeJenisTarif],
                        1 as [jumlahSatuan],
                        1 as [kodeFasilitasTarif],
                        'BM|CUKAI|PPN|PPNBM|PPH' as [kodeJenisPungutan],
                        concat(db.BMITEMS ,'|',db.CUKAIITEMS,'|',db.PPNITEMS,'|',db.PPNBMITEMS,'|',db.PPHITEMS) as [nilaiBayar],
                        db.SHORT as [seriBarang],
                        db.RUMUS as [tarif],
                        0 as [tarifFasilitas],
                        0 as [nilaiFasilitas]
                        from DETAILBARANGBILLINGREPORT db
                        left join BILLINGREPORT b on db.HAWB = b.HAWB
                        left join DETIL dt on db.HAWB = dt.NOHOSTBLAWB
                        where b.GATEWAY = '{gateway}' and
                        db.HAWB in ({hawb})";
                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
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

                        string[] arr_tarif = dr["tarif"].ToString().Split(',');
                        string tarif_bm = arr_tarif[0];
                        string tarif_cukai = arr_tarif[1];
                        string tarif_ppn = arr_tarif[2];
                        string tarif_ppnbm = arr_tarif[3];
                        string tarif_pph = arr_tarif[4];

                        for (int i = 0; i < 5; i++)
                        {
                            switch (i)
                            {
                                case 1: //cukai
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '{dr["kodeFasilitasTarif"]}',
                                        '{kode_cukai}',
                                        '{nilai_cukai}',
                                        '{dr["seriBarang"]}',
                                        '{tarif_cukai}',
                                        '{dr["tarifFasilitas"]}',
                                        '{dr["nilaiFasilitas"]}'
                                        )");
                                    break;
                                case 2: //ppn
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '{dr["kodeFasilitasTarif"]}',
                                        '{kode_ppn}',
                                        '{nilai_ppn}',
                                        '{dr["seriBarang"]}',
                                        '{tarif_ppn}',
                                        '{dr["tarifFasilitas"]}',
                                        '{dr["nilaiFasilitas"]}'
                                        )");
                                    break;
                                case 3: //ppnbm
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '{dr["kodeFasilitasTarif"]}',
                                        '{kode_ppnbm}',
                                        '{nilai_ppnbm}',
                                        '{dr["seriBarang"]}',
                                        '{tarif_ppnbm}',
                                        '{dr["tarifFasilitas"]}',
                                        '{dr["nilaiFasilitas"]}'
                                        )");
                                    break;
                                case 4: //pph
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '{dr["kodeFasilitasTarif"]}',
                                        '{kode_pph}',
                                        '{nilai_pph}',
                                        '{dr["seriBarang"]}',
                                        '{tarif_pph}',
                                        '{dr["tarifFasilitas"]}',
                                        '{dr["nilaiFasilitas"]}'
                                        )");
                                    break;
                                default: //bm
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{dr["kodeJenisTarif"]}',
                                        '{dr["jumlahSatuan"]}',
                                        '{dr["kodeFasilitasTarif"]}',
                                        '{kode_bm}',
                                        '{nilai_bm}',
                                        '{dr["seriBarang"]}',
                                        '{tarif_bm}',
                                        '{dr["tarifFasilitas"]}',
                                        '{dr["nilaiFasilitas"]}'
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
                        ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB) as [TGLHAWB],
                        concat(ALAMATPENERIMA,'|',ALAMATPENGIRIM,'|','MULIA BUSINESS PARK BUILDING F JL. MT. HARYONO KAV 58-60, JAKARTA') as [alamatEntitas],
                        '1|7|9|10|11|4' as [kodeEntitas],
                        '01' as [kodeJenisApi],
                        '5' as [kodeJenisIdentitas],
                        'LAINNYA' as [kodeStatus],
                        concat(NAMAPENERIMA,'|',NAMAPENGIRIM,'|','PT. BIROTIKA SEMESTA') as [namaEntitas],
                        NOAPI as [nibEntitas],
                        concat(KITASPENERIMA,'|','013107743062000') as [nomorIdentitas],
                        KODENEGARAPENGIRIM as [kodeNegara],
                        '' as [seriEntitas]
                        from BILLINGREPORT b
                        left join DETIL dt on b.HAWB = dt.NOHOSTBLAWB 
                        where GATEWAY = '{gateway}' and
                        HAWB in ({hawb})";
                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
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
                        string kode_1 = arr_kode[0];
                        string kode_7 = arr_kode[1];
                        string kode_9 = arr_kode[2];
                        string kode_10 = arr_kode[3];
                        string kode_11 = arr_kode[4];
                        string kode_4 = arr_kode[5];

                        string[] arr_identitas = dr["nomorIdentitas"].ToString().Split('|');
                        string kitas_penerima = arr_identitas[0];
                        string kitas_DHL = arr_identitas[1];

                        for (int i = 1; i <= 6; i++)
                        {
                            switch (i)
                            {
                                case 1: //kode 1
                                    listQuery.Add($@"insert into dhl_cs_entitas values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{alamat_penerima}',
                                        '{kode_1}',
                                        '{dr["kodeJenisApi"]}',
                                        '{dr["kodeJenisIdentitas"]}',
                                        '{dr["kodeStatus"]}',
                                        '{nama_penerima}',
                                        '{dr["nibEntitas"]}',
                                        '{kitas_penerima}',
                                        '', '{i}'
                                        )");
                                    break;
                                case 2: //kode 7
                                    listQuery.Add($@"insert into dhl_cs_entitas values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{alamat_penerima}',
                                        '{kode_7}',
                                        '',
                                        '{dr["kodeJenisIdentitas"]}',
                                        '',
                                        '{nama_penerima}',
                                        '',
                                        '{kitas_penerima}',
                                        '', '{i}'
                                        )");
                                    break;
                                case 3: //kode 9
                                    listQuery.Add($@"insert into dhl_cs_entitas values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{alamat_pengirim}',
                                        '{kode_9}',
                                        '',
                                        '',
                                        '',
                                        '{nama_pengirim}',
                                        '',
                                        '',
                                        '{dr["kodeNegara"]}', '{i}'
                                        )");
                                    break;
                                case 4: //kode 10
                                    listQuery.Add($@"insert into dhl_cs_entitas values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{alamat_pengirim}',
                                        '{kode_10}',
                                        '',
                                        '',
                                        '',
                                        '{nama_pengirim}',
                                        '',
                                        '',
                                        '{dr["kodeNegara"]}', '{i}'
                                        )");
                                    break;
                                case 5: //kode 11
                                    listQuery.Add($@"insert into dhl_cs_entitas values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{alamat_penerima}',
                                        '{kode_11}',
                                        '',
                                        '{dr["kodeJenisIdentitas"]}',
                                        '',
                                        '{nama_pengirim}',
                                        '',
                                        '{kitas_penerima}',
                                        '', '{i}'
                                        )");
                                    break;
                                default: //kode 4
                                    listQuery.Add($@"insert into dhl_cs_entitas values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{alamat_DHL}',
                                        '{kode_4}',
                                        '',
                                        '{dr["kodeJenisIdentitas"]}',
                                        '',
                                        '{nama_DHL}',
                                        '',
                                        '{kitas_DHL}',
                                        '', '{i}'
                                        )");
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
                        ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB) as [TGLHAWB],
                        SDPieces as [jumlahKemasan],
                        'PK' as [kodeJenisKemasan],
                        'DHL' as [merkKemasan],
                        1 as [seriKemasan]
                        from BILLINGREPORT b
                        left join DETIL dt on b.HAWB = dt.NOHOSTBLAWB
                        where GATEWAY = '{gateway}' and
                        HAWB in ({hawb})";
                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        listQuery.Add($@"insert into dhl_cs_kemasan values (
                                        '{dr["HAWB"]}',
                                        '{GetDate(dr["TGLHAWB"].ToString())}',
                                        '{dr["jumlahKemasan"]}',
                                        '{dr["kodeJenisKemasan"]}',
                                        '{dr["merkKemasan"]}',
                                        '{dr["seriKemasan"]}'
                                        )");
                    }

                    //if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    //    retval = listQuery.Count;
                }
                #endregion

                #region load data dokumen
                query = $@"select
                        db.HAWB,
                        ISNULL(dt.TGLHOSTBLAWB,db.TGLHAWB) as [TGLHAWB],
                        '380|741|740' as [kodeDokumen],
                        concat(db.InvNo,'|',b.MAWB,'|',b.HAWB) as [nomorDokumen],
                        concat(db.Invoice_Date,'|',b.TGLARRIVAL,'|',b.TGLHAWB) as [tanggalDokumen]
                        from DETAILBARANGBILLINGREPORT db
                        left join BILLINGREPORT b on db.HAWB = b.HAWB
                        left join DETIL dt on db.HAWB = dt.NOHOSTBLAWB
                        where GATEWAY = '{gateway}' and
                        db.InvNo != '' and
                        db.HAWB in ({hawb})";

                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string[] arr_kode = dr["kodeDokumen"].ToString().Split('|');
                        string kode_380 = arr_kode[0];
                        string kode_741 = arr_kode[1];
                        string kode_740 = arr_kode[2];

                        string[] arr_no_dokumen = dr["nomorDokumen"].ToString().Split('|');
                        string nomor_380 = arr_no_dokumen[0];
                        string nomor_741 = arr_no_dokumen[1];
                        string nomor_740 = arr_no_dokumen[2];

                        string[] arr_tgl_dokumen = dr["tanggalDokumen"].ToString().Split('|');
                        string tgl_380 = arr_tgl_dokumen[0];
                        string tgl_741 = arr_tgl_dokumen[1];
                        string tgl_740 = arr_tgl_dokumen[2];

                        for (int i = 1; i <= 3; i++)
                        {
                            switch (i)
                            {
                                case 1://kode 380
                                    listQuery.Add($@"insert into dhl_cs_dokumen values (
                                        '{dr["HAWB"]}', '{GetDate(dr["TGLHAWB"].ToString())}', null,
                                        '{kode_380}', null, null,
                                        '{nomor_380}', '{i}',
                                        '{tgl_380}', null
                                        )");
                                    break;
                                case 2://kode 741
                                    listQuery.Add($@"insert into dhl_cs_dokumen values (
                                        '{dr["HAWB"]}', '{GetDate(dr["TGLHAWB"].ToString())}', null,
                                        '{kode_741}', null, null,
                                        '{nomor_741}', '{i}',
                                        '{tgl_741}', null
                                        )");
                                    break;
                                default://kode 740
                                    listQuery.Add($@"insert into dhl_cs_dokumen values (
                                        '{dr["HAWB"]}', '{GetDate(dr["TGLHAWB"].ToString())}', null,
                                        '{kode_740}', null, null,
                                        '{nomor_740}', '{i}',
                                        '{tgl_740}', null
                                        )");
                                    break;
                            }
                        }
                    }

                    //if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    //    retval = listQuery.Count;
                }
                #endregion

                #region load data pengangkut
                query = $@"select
                        HAWB,
                        ISNULL(dt.TGLHOSTBLAWB,b.TGLHAWB) as [TGLHAWB],
                        NAMATRANSPORT as [namaPengangkut],
                        KODETRANSPORT as [nomorPengangkut],
                        NOTRANSPORT as [kodeCaraAngkut],
                        1 as [seriPengangkut]
                        from BILLINGREPORT b
                        left join DETIL dt on b.HAWB = dt.NOHOSTBLAWB
                        where GATEWAY = '{gateway}' and
                        HAWB in ({hawb})";
                dt = DbOfficialDCI.getRecords(query);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        listQuery.Add($@"insert into dhl_cs_pengangkut values (
                                        '{dr["HAWB"]}', '{GetDate(dr["TGLHAWB"].ToString())}', null,
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

                if (DbOfficialCeisa.runCommand(listQuery) == 0)
                    retval = listQuery.Count;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return retval;
        }
        #endregion

        #region modals
        private void ClearFormDCI()
        {
            inputHAWB.Text = string.Empty;
            sendHAWB.Text = string.Empty;
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
        #endregion

        #region download CSV
        protected void btnDownloadBarang_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tblBarang = (DataTable)ViewState["ListBarang"];

                //set file name
                string filename = "Barang_" + DateTime.Now.ToString("yyyy-MM-dd");

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
                    ndpbm as [NDPBM],
                    netto as [NETTO],
                    posTarif as [POS TARIF],
                    seriBarang as [SERI BARANG],
                    (select tarif from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'BM' and seriBarang = barang.seriBarang) as [BM TARIF],
                    (select tarif from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'CUKAI' and seriBarang = barang.seriBarang) as [CUKAI TARIF],
                    (select tarif from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'PPN' and seriBarang = barang.seriBarang) as [PPN TARIF],
                    (select tarif from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'PPNBM' and seriBarang = barang.seriBarang) as [PPNBM TARIF],
                    (select tarif from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'PPH' and seriBarang = barang.seriBarang) as [PPH TARIF],
                    uraian as [URAIAN]
                    from dhl_cs_barang barang
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
            DataTable tblBarangTarif = DbOfficialCeisa.getRecords(query);
            ViewState["ListBarangTarif"] = tblBarangTarif;
            GV_BarangTarif.DataSource = tblBarangTarif;
            GV_BarangTarif.DataBind();
        }
        private void ProcessExcelBarang(string filePath, string fileName)
        {
            try
            {
                DataTable tbl = new DataTable();
                FileInfo newFile = new FileInfo(filePath);
                ExcelPackage pck = new ExcelPackage(newFile);
                var summary = pck.Workbook.Worksheets[1];
                var range = summary.Cells["A2:U20"];
                tbl = GetDataTableFromRange(range);
                InsertToDb(tbl);
            }
            catch { }
        }
        private DataTable GetDataTableFromRange(ExcelRange range)
        {
            DataTable tbl = new DataTable();
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
            tbl.Columns.Add("NDPBM");
            tbl.Columns.Add("NETTO");
            tbl.Columns.Add("POS_TARIF");
            tbl.Columns.Add("SERI_BARANG");
            tbl.Columns.Add("BM_TARIF");
            tbl.Columns.Add("CUKAI_TARIF");
            tbl.Columns.Add("PPN_TARIF");
            tbl.Columns.Add("PPNBM_TARIF");
            tbl.Columns.Add("PPH_TARIF");
            tbl.Columns.Add("URAIAN");

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
                newRow[dataTableColumn] = item.Value.ToString();
                dataTableColumn += 1;
            }
            return tbl;
        }
        private void InsertToDb(DataTable tbl)
        {
            try
            {
                List<string> listQuery = new List<string>();
                if (tbl.Rows.Count > 0)
                {
                    double asuransi = 0;
                    double fob = 0;
                    double freight = 0;
                    string HAWB = string.Empty;
                    string TGL_HAWB = string.Empty;

                    foreach (DataRow row in tbl.Rows)
                    {
                        double BM_Items = 0.0;
                        double CUKAI_Items = 0.0;
                        double PPN_Items = 0.0;
                        double PPNBM_Items = 0.0;
                        double PPH_Items = 0.0;
                        double CIF_Items = 0.0;

                        double ASURANSI = Math.Round(Convert.ToDouble(row["ASURANSI"]), 2);
                        double NDPBM = Math.Round(Convert.ToDouble(row["NDPBM"]), 2);
                        double FOB = Math.Round(Convert.ToDouble(row["FOB"]), 2);
                        double FREIGHT = Math.Round(Convert.ToDouble(row["FREIGHT"]), 2);
                        double BM_Tarif = Math.Round(Convert.ToDouble(row["BM_TARIF"]), 2);
                        double CUKAI_Tarif = Math.Round(Convert.ToDouble(row["CUKAI_TARIF"]), 2);
                        double PPN_Tarif = Math.Round(Convert.ToDouble(row["PPN_TARIF"]), 2);
                        double PPNBM_Tarif = Math.Round(Convert.ToDouble(row["PPNBM_TARIF"]), 2);
                        double PPH_Tarif = Math.Round(Convert.ToDouble(row["PPH_TARIF"]), 2);

                        CIF_Items = Math.Round((double)ASURANSI * NDPBM, 2) + Math.Round((double)FOB * NDPBM, 2) + Math.Round((double)FREIGHT * NDPBM, 2);
                        BM_Items = ((double)CIF_Items * BM_Tarif / 100);
                        CUKAI_Items = Math.Round((double)(CIF_Items + BM_Items) * CUKAI_Tarif / 100, 2);
                        PPN_Items = Math.Round((double)(CIF_Items + BM_Items) * PPN_Tarif / 100, 2);
                        PPNBM_Items = Math.Round((double)(CIF_Items + BM_Items) * PPNBM_Tarif / 100, 2);
                        PPH_Items = Math.Round((double)(CIF_Items + BM_Items) * PPH_Tarif / 100, 2);

                        listQuery.Add($@"insert into dhl_cs_barang values (
                                        '{row["HAWB"]}',
                                        '{row["TGL_HAWB"]}',
                                        '{row["ASURANSI"]}',
                                        '0',
                                        '{row["CIF"]}',
                                        '{row["CIF_RUPIAH"]}',
                                        '0',
                                        '{row["FOB"]}',
                                        '{row["FREIGHT"]}',
                                        '0','0','0','0',
                                        '{row["HARGA_SATUAN"]}',
                                        '0','0','0','0','1','0','0',
                                        '{row["JUMLAH_SATUAN"]}',
                                        '0','PK','1',
                                        '{row["KODE_NEGARA_ASAL"]}',
                                        '{row["KODE_SATUAN_BARANG"]}',
                                        '-',
                                        '{row["NDPBM"]}',
                                        '{row["NETTO"]}',
                                        '0','0','0','0','T','0',
                                        '{row["POS_TARIF"]}',
                                        '0','0',
                                        '{row["SERI_BARANG"]}',
                                        '0','0','0','0','-',
                                        '{row["URAIAN"]}',
                                        '0'
                                        )");

                        for (int i = 0; i < 5; i++)
                        {
                            switch (i)
                            {
                                case 1: //cukai
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{row["HAWB"]}',
                                        '{row["TGL_HAWB"]}',
                                        '1','1','1','CUKAI','{CUKAI_Items}',
                                        '{row["SERI_BARANG"]}',
                                        '{row["CUKAI_TARIF"]}',
                                        '0','0'
                                        )");
                                    break;
                                case 2: //ppn
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{row["HAWB"]}',
                                        '{row["TGL_HAWB"]}',
                                        '1','1','1','PPN','{PPN_Items}',
                                        '{row["SERI_BARANG"]}',
                                        '{row["PPN_TARIF"]}',
                                        '0','0'
                                        )");
                                    break;
                                case 3: //ppnbm
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{row["HAWB"]}',
                                        '{row["TGL_HAWB"]}',
                                        '1','1','1','PPNBM','{PPNBM_Items}',
                                        '{row["SERI_BARANG"]}',
                                        '{row["PPNBM_TARIF"]}',
                                        '0','0'
                                        )");
                                    break;
                                case 4: //pph
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{row["HAWB"]}',
                                        '{row["TGL_HAWB"]}',
                                        '1','1','1','PPH','{PPH_Items}',
                                        '{row["SERI_BARANG"]}',
                                        '{row["PPH_TARIF"]}',
                                        '0','0'
                                        )");
                                    break;
                                default: //bm
                                    listQuery.Add($@"insert into dhl_cs_barang_tarif values (
                                        '{row["HAWB"]}',
                                        '{row["TGL_HAWB"]}',
                                        '1','1','1','BM','{BM_Items}',
                                        '{row["SERI_BARANG"]}',
                                        '{row["BM_TARIF"]}',
                                        '0','0'
                                        )");
                                    break;
                            }
                        }

                        HAWB = row["HAWB"].ToString();
                        TGL_HAWB = row["TGL_HAWB"].ToString();

                        asuransi += Math.Round(ASURANSI, 2);
                        fob += Math.Round(FOB, 2);
                        freight += Math.Round(FREIGHT, 2);
                    }

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
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Upload Excel Success.');", true);
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
                    ndpbm as [NDPBM],
                    netto as [NETTO],
                    posTarif as [POS TARIF],
                    seriBarang as [SERI BARANG],
                    (select tarif from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'BM' and seriBarang = barang.seriBarang) as [BM TARIF],
                    (select tarif from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'CUKAI' and seriBarang = barang.seriBarang) as [CUKAI TARIF],
                    (select tarif from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'PPN' and seriBarang = barang.seriBarang) as [PPN TARIF],
                    (select tarif from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'PPNBM' and seriBarang = barang.seriBarang) as [PPNBM TARIF],
                    (select tarif from dhl_cs_barang_tarif
                    where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                    and kodeJenisPungutan = 'PPH' and seriBarang = barang.seriBarang) as [PPH TARIF],
                    uraian as [URAIAN]
                    from dhl_cs_barang barang
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
                    DataTable tblBarangTarif = DbOfficialCeisa.getRecords(query);
                    ViewState["ListBarangTarif"] = tblBarangTarif;
                    GV_BarangTarif.DataSource = tblBarangTarif;
                    GV_BarangTarif.DataBind();
                }
            }
            catch { }
        }
        #endregion


    }
}