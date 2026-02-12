using Newtonsoft.Json;
using Octopus.Library.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficialCeisaLite.App_Start;
using OfficialCeisaLite.Models;
using OfficialCeisaLite.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OfficialCeisaLite.Views
{
    public partial class ReportStatusGate : System.Web.UI.Page
    {
        NbLogger logger = new NbLogger(AppDomain.CurrentDomain.BaseDirectory + @"/Log/", "TPB_REPORT_STATUS_GATE");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //LoadData();
            }
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(btnDownloadDataKPBC);
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
                string query = $@"select 
                                a.id as [ID],
                                gateway as [Gateway],
                                a.HAWB,
                                convert(varchar(10),a.TGL_HAWB,20) as [Tgl HAWB],
                                (select distinct namaEntitas from dhl_cs_tpb_entitas e
                                where e.hawb = a.HAWB and e.kodeEntitas = 3) as [Nama Penerima],
                                nomorBc11 as [No BC 1.1],
                                convert(char(10),tanggalBc11,126) as [Tgl BC 1.1],
                                nomorAju as [No Aju],
                                (select distinct f.nomorRespon from dhl_cs_response_data f
                                where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB
                                and keterangan = 'SPPB') as [SPPB],
                                (select distinct 
                                case
                                when f.nomorRespon = '' then ''
                                else convert(char(10),f.tanggalRespon,126)
                                end as tanggalRespon from dhl_cs_response_data f
                                where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB
                                and keterangan = 'SPPB') as [Tgl SPPB],
                                (select distinct top 1 convert(varchar,g.latest_response_time,120) as latest_response_time
                                from dhl_cs_response_header g where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) as [Latest Response Time],
                                (select distinct g.latest_response_code from dhl_cs_response_header g
                                where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) as [Latest Response Code],
                                (select distinct g.latest_response_note from dhl_cs_response_header g
                                where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) as [Latest Response Description],
                                update_by as [Update By],
                                convert(varchar,update_time,120) as [Update Time]
                                from dhl_cs_tpb_header a
                                where gateway = '{gateway}'";
                string hawb = srcHAWB.Text.Trim();
                string nama = srcNama.Text.Trim();
                string tglBc11From = srcTglBCFrom.Text.Trim();
                string tglBc11To = srcTglBCTo.Text.Trim();
                string tglSPPBFrom = srcTglSppbFrom.Text.Trim();
                string tglSPPBTo = srcTglSppbTo.Text.Trim();
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
                else if (nama != "")
                {
                    parameter = $@" and (select distinct namaEntitas from dhl_cs_tpb_entitas g where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB and g.kodeEntitas = 3 and namaEntitas like '%{nama.Trim()}%') like '%{nama.Trim()}%'";
                    query += parameter;
                    query = $@"SELECT TOP 1000 * FROM ({query}) as TempTable order by [ID] desc";
                }
                else if (tglBc11From != "" && tglBc11To != "")
                {
                    parameter = $@" and convert(char(10),tanggalBc11,126) >= '{tglBc11From}'
                                    and convert(char(10),tanggalBc11,126) <= '{tglBc11To}'";
                    query += parameter;
                }
                else if (tglSPPBFrom != "" && tglSPPBTo != "")
                {
                    parameter = $@" and (select distinct 
                                    case
                                    when f.nomorRespon = '' then ''
                                    else convert(char(10),f.tanggalRespon,126)
                                    end as tanggalRespon from dhl_cs_response_data f
                                    where a.HAWB = f.HAWB and a.TGL_HAWB = f.TGL_HAWB
                                    and keterangan = 'SPPB' and convert(char(10),f.tanggalRespon,126) 
                                    between '{tglSPPBFrom}' and '{tglSPPBTo}') 
                                    between '{tglSPPBFrom}' and '{tglSPPBTo}'";
                    query += parameter;
                    query = $@"SELECT * FROM ({query}) as TempTable order by [ID] desc";
                }
                //else if (shipment_status != "0")
                //{
                //    parameter = $@" and (select distinct g.latest_response_code from dhl_cs_response_header g where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) = '{shipment_status.Trim()}' order by [ID] desc";
                //    query += parameter;
                //}
                //else if (string.IsNullOrEmpty(tgl_hawb) == false)
                //{
                //    parameter = $@" and a.TGL_HAWB = '{tgl_hawb}' order by [ID] desc";
                //    query += parameter;
                //}
                //else if (string.IsNullOrEmpty(no_aju) == false)
                //{
                //    parameter = $@" and nomorAju like '%{no_aju}%' order by [ID] desc";
                //    query += parameter;
                //}
                //else if (isBlankAju.Checked == true)
                //{
                //    parameter = $@" and nomorAju = '' order by [ID] desc";
                //    query += parameter;
                //}
                //else if (string.IsNullOrEmpty(status) == false)
                //{
                //    parameter = $@" and (select distinct g.latest_response_note from dhl_cs_response_header g
                //                    where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) like '%{status}%' order by [ID] desc";
                //    query += parameter;
                //}
                //else if (isBlankStatus.Checked == true)
                //{
                //    parameter = $@" and (select distinct g.latest_response_note from dhl_cs_response_header g
                //                    where g.HAWB = a.HAWB and g.TGL_HAWB = a.TGL_HAWB) is null order by [ID] desc";
                //    query += parameter;
                //}
                else
                {
                    parameter = $@" order by [ID] desc";
                    query += parameter;
                }

                DataTable dt = DbOfficialCeisa.getRecords(query);
                ViewState["QueryDataReport"] = query;
                ViewState["ListDataReport"] = dt;
                GV_ReportData.DataSource = dt;
                GV_ReportData.DataBind();
                totalData.InnerText = dt.Rows.Count.ToString();
            }
            catch { }
        }
        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (sender as CheckBox);
            if (chk.ID == "chkAll")
            {
                foreach (GridViewRow row in GV_ReportData.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked = chk.Checked;
                    }
                }
            }
            CheckBox chkAll = (GV_ReportData.HeaderRow.FindControl("chkAll") as CheckBox);
            chkAll.Checked = true;
            ViewState["isChkAll"] = "true";
            foreach (GridViewRow row in GV_ReportData.Rows)
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
                //requestObjGet.Headers.Add("Authorization", $"Bearer {GetAccessToken()}");

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
                                            //base64pdf = GetBase64StringPdf(item.pdf, GetAccessToken(), item.keterangan, nomorAju);
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
                
                //InitDropdown();
                //Response.Redirect("~/Views/RequestTPB.aspx");
            }
            catch (Exception ex)
            {
                logger.Log($"{ex.Message},{ex.StackTrace}");
            }
        }
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
                        foreach (GridViewRow row in GV_ReportData.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                bool isChecked = row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked;
                                if (!isChecked) continue;

                                string NO_AJU = GV_ReportData.DataKeys[row.RowIndex].Values[2].ToString();
                                if (string.IsNullOrEmpty(NO_AJU)) continue;

                                string HAWB = GV_ReportData.DataKeys[row.RowIndex].Values[0].ToString();
                                string TGL_HAWB = GV_ReportData.DataKeys[row.RowIndex].Values[1].ToString();

                                listAju.Add($"{NO_AJU}|{HAWB}|{TGL_HAWB}");
                            }
                        }
                    }
                    else
                    {
                        foreach (GridViewRow row in GV_ReportData.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                bool isChecked = row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked;
                                if (!isChecked) continue;

                                string NO_AJU = GV_ReportData.DataKeys[row.RowIndex].Values[2].ToString();
                                if (string.IsNullOrEmpty(NO_AJU)) continue;

                                string HAWB = GV_ReportData.DataKeys[row.RowIndex].Values[0].ToString();
                                string TGL_HAWB = GV_ReportData.DataKeys[row.RowIndex].Values[1].ToString();

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
                    //CloseModalLoader();
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
                                                    //base64pdf = GetBase64StringPdf(item.pdf, GetAccessToken(), item.keterangan, no_aju);
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
                    //CloseModalLoader();
                    string cacheQuery = (string)ViewState["QueryDataReport"];
                    DataTable dt = DbOfficialCeisa.getRecords(cacheQuery);
                    ViewState["ListDataReport"] = dt;
                    GV_ReportData.DataSource = dt;
                    GV_ReportData.DataBind();
                    totalData.InnerText = dt.Rows.Count.ToString();
                }
                else
                {
                    //CloseModalLoader();
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
        protected void btnDownloadDataKPBC_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tblDokumen = (DataTable)ViewState["ListDataReport"];

                //set file name
                string filename = "ReportStatusGateTPB_" + DateTime.Now.ToString("yyyyMMddHHmmss");

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
        public string GetAccessToken()
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
        protected void GV_ReportData_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void GV_ReportData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_ReportData.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)ViewState["ListDataReport"];
            GV_ReportData.DataSource = dt;
            GV_ReportData.DataBind();
        }

        protected void GV_ReportData_PageIndexChanged(object sender, EventArgs e)
        {

        }
    }
}