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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OfficialCeisaLite.Views
{
    public partial class UserManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //validate session
                string role = string.Empty;
                LoginData user = SessionUtils.GetUserData(this.Page);
                if (user != null)
                {
                    role = user.role;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Session user expired, please login again.');", true);
                    Response.Redirect("~/Auth/Login.aspx");
                    return;
                }

                LoadData(role);
                LoadDataLocation();
                InitDropDownList(role);
            }
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(btnExport);
        }

        private void LoadData(string role)
        {
            DbOfficialCeisa.LoadParameter();

            string query = string.Empty;

            if (role == "admin")
            {
                query = @"select
                        id as 'ID',
                        username as 'UName',
                        fullname as 'Full Name',
                        role as 'Role',
                        location as 'Location Gateway',
                        update_by as 'Created By',
                        update_time as 'Created Time'
                        from dhl_cs_userdata";
            }
            else
            {
                query = @"select
                        id as 'ID',
                        username as 'UName',
                        fullname as 'Full Name',
                        role as 'Role',
                        location as 'Location Gateway',
                        update_by as 'Created By',
                        update_time as 'Created Time'
                        from dhl_cs_userdata
                        where role != 'admin'";
            }

            DataTable dt = DbOfficialCeisa.getRecords(query);
            ViewState["ListDataUserManagement"] = dt;
            GV_UserManagement.DataSource = dt;
            GV_UserManagement.DataBind();
        }
        private void LoadDataLocation()
        {
            DbOfficialCeisa.LoadParameter();
            string query = @"select * from dhl_cs_gateway_location";
            DataTable dt = DbOfficialCeisa.getRecords(query);
            ViewState["ListDataLocation"] = dt;
            GV_Location.DataSource = dt;
            GV_Location.DataBind();
        }
        private void InitDropDownList(string role)
        {
            ddlRole.Items.Clear();
            
            if (role == "admin")
                ddlRole.Items.Add("admin");

            ddlRole.Items.Add("user");
            ddlRole.Items.Add("supervisor");            

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

        protected void GV_UserManagement_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //validate session
            string role = string.Empty;
            LoginData user = SessionUtils.GetUserData(this.Page);
            if (user != null)
            {
                role = user.role;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Session user expired, please login again.');", true);
                Response.Redirect("~/Auth/Login.aspx");
                return;
            }

            try
            {
                if (e.CommandName == "EditRow" || e.CommandName == "DeleteRow")
                {
                    //get id
                    string key = e.CommandArgument.ToString();

                    if (e.CommandName == "EditRow")
                    {
                        //display data
                        ViewRecord(key, role);
                    }
                    else if (e.CommandName == "DeleteRow")
                    {
                        //delete data
                        DeleteRecord(key, role);
                    }
                }
            }
            catch { }
        }
        protected void GV_UserManagement_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                //bind gridview
                GV_UserManagement.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["ListDataUserManagement"];
                GV_UserManagement.DataSource = dt;
                GV_UserManagement.DataBind();
            }
            catch { }
        }
        protected void GV_Location_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                //bind gridview
                GV_Location.PageIndex = e.NewPageIndex;
                DataTable dt = (DataTable)ViewState["ListDataLocation"];
                GV_Location.DataSource = dt;
                GV_Location.DataBind();
            }
            catch { }
        }

        #region Add, Save, Edit, Delete

        #region Userdata
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (ClearForm() == 0)
            {
                ViewState["IsNewRecord"] = "true";
                ShowModal();
            }            
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            //validate session
            string role = string.Empty;
            LoginData user = SessionUtils.GetUserData(this.Page);
            if (user != null)
            {
                role = user.role;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Session user expired, please login again.');", true);
                Response.Redirect("~/Auth/Login.aspx");
                return;
            }

            try
            {
                if (ViewState["IsNewRecord"].ToString() == "true")
                    AddRecord();
                else
                    UpdateRecord();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            
            LoadData(role);
            CloseModal();
        }
        protected void AddRecord()
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

                string query = string.Empty;

                //check duplicate
                string username = txtUsername.Text;
                query = $"select * from dhl_cs_userdata where username = '{username}'";
                DataRow dr = DbOfficialCeisa.getRow(query);
                if (dr != null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Username already registered!');", true);
                }
                else
                {
                    string[] arr = ddlLocation.SelectedItem.Text.Split('|');
                    string location = arr[0];

                    query = $@"insert into dhl_cs_userdata
                            VALUES('{txtUsername.Text}',
                            'dhl123',
                            '{ddlRole.SelectedItem.Text.ToLower()}',
                            '{location}',
                            '{txtFullname.Text}',
                            '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                            '{update_by}',
                            '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                    DbOfficialCeisa.runCommand(query);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Insert data success');", true);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        protected void UpdateRecord()
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

                string[] arr = ddlLocation.SelectedItem.Text.Split('|');
                string location = arr[0].Trim();

                string query = string.Empty;
                query = $@"update dhl_cs_userdata set 
                            role = '{ddlRole.SelectedItem.Text}',
                            fullname = '{txtFullname.Text}',
                            location = '{location}',
                            update_by = '{update_by}',
                            update_time = '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff")}'
                            WHERE username = '{txtUsername.Text}'";
                DbOfficialCeisa.runCommand(query);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Update data success');", true);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        protected void ViewRecord(string ID, string role)
        {
            try
            {
                string query = $@"select * from dhl_cs_userdata a left join
                                dhl_cs_gateway_location b on
                                a.location = b.location_code WHERE id = '{ID}'";
                DataRow dr = DbOfficialCeisa.getRow(query);

                txtUsername.Text = dr["username"].ToString();
                txtFullname.Text = dr["fullname"].ToString();
                ddlLocation.SelectedItem.Text = $"{dr["location_code"]} | {dr["location_name"]}";

                if (role != "admin")
                {
                    if (dr["role"].ToString() == "admin")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Please contact your administrator.');", true);
                        return;
                    }
                }

                GetRole(dr["role"].ToString());

                //disable edit username
                txtUsername.ReadOnly = true;

                ViewState["IsNewRecord"] = "false";
                ShowModal();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        protected void DeleteRecord(string ID, string role)
        {
            try
            {
                if (role != "admin")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Please contact your administrator.');", true);
                    return;
                }
                string query = $"delete from dhl_cs_userdata where id = '{ID}'";
                DbOfficialCeisa.runCommand(query);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Delete data success');", true);
                LoadData(role);
            }
            catch { }
        }
        private void GetRole(string role)
        {
            try
            {
                if (ddlRole.Items.Count == 2)
                {
                    switch (role)
                    {
                        case "supervisor":
                            ddlRole.SelectedIndex = 1;
                            break;
                        default: //user
                            ddlRole.SelectedIndex = 0;
                            break;
                    }
                }
                else
                {
                    switch (role)
                    {
                        case "supervisor":
                            ddlRole.SelectedIndex = 2;
                            break;
                        case "user":
                            ddlRole.SelectedIndex = 1;
                            break;
                        default:
                            ddlRole.SelectedIndex = 0;
                            break;
                    }

                }
                
            }
            catch { }
        }
        #endregion

        #region Location
        protected void btnAddLocation_Click(object sender, EventArgs e)
        {
            ClearFormLocation();
            ShowModalLocation();
        }
        protected void btnSaveLocation_Click(object sender, EventArgs e)
        {
            try
            {
                string query = string.Empty;

                //check duplicate
                string location_code = txtLocationCode.Text;
                query = $"select * from dhl_cs_gateway_location where location_code = '{location_code}'";
                DataRow dr = DbOfficialCeisa.getRow(query);
                if (dr != null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Location already registered!');", true);
                }
                else
                {
                    query = $@"insert into dhl_cs_gateway_location
                            VALUES('{location_code}',
                            '{txtLocationName.Text}')";
                    DbOfficialCeisa.runCommand(query);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Insert data success');", true);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            LoadDataLocation();
            CloseModalLocation();
        }
        #endregion

        #endregion

        #region modals
        protected int ClearForm()
        {
            int retval = 0;
            //validate session
            string role = string.Empty;
            LoginData user = SessionUtils.GetUserData(this.Page);
            if (user != null)
            {
                role = user.role;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Session user expired, please login again.');", true);
                Response.Redirect("~/Auth/Login.aspx");
                return -1;
            }

            if (role != "admin")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Please contact your administrator.');", true);
                return -1;
            }

            ddlRole.SelectedItem.Text = string.Empty;
            ddlLocation.SelectedItem.Text = string.Empty;
            InitDropDownList(role);
            txtUsername.Text = string.Empty;
            txtUsername.ReadOnly = false;
            txtFullname.Text = string.Empty;
            return retval;
        }
        protected void ClearFormLocation()
        {
            txtLocationCode.Text = string.Empty;
            txtLocationName.Text = string.Empty;
        }
        protected void ShowModal()
        {
            ScriptManager.RegisterStartupScript(
                (Page)this, base.GetType(),
                "popup",
                "$('#btnPopup').click();",
                true
                );
        }
        protected void ShowModalLocation()
        {
            ScriptManager.RegisterStartupScript(
                (Page)this, base.GetType(),
                "popup",
                "$('#btnPopupLocation').click();",
                true
                );
        }
        protected void CloseModal()
        {
            ScriptManager.RegisterStartupScript(
                (Page)this, base.GetType(),
                "close",
                "$('#btnClose').click();",
                true
                );
        }
        protected void CloseModalLocation()
        {
            ScriptManager.RegisterStartupScript(
                (Page)this, base.GetType(),
                "close",
                "$('#btnCloseLocation').click();",
                true
                );
        }

        #endregion

        #region Search
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //validate session
            string role_session = string.Empty;
            LoginData user = SessionUtils.GetUserData(this.Page);
            if (user != null)
            {
                role_session = user.role;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Session user expired, please login again.');", true);
                Response.Redirect("~/Auth/Login.aspx");
                return;
            }

            try
            {
                string username = string.Empty;
                string role = string.Empty;
                string parameter = string.Empty;

                if (string.IsNullOrEmpty(srcUsername.Text) == false)
                    username = $"username = '{srcUsername.Text.ToLower().Trim()}' ";
                if (string.IsNullOrEmpty(srcRole.Text) == false)
                    role = $"role = '{srcRole.Text.ToLower().Trim()}' ";

                if (role_session != "admin")
                {
                    if (role.Contains("admin")) role = "";
                }

                string query = $@"select
                                id as 'ID',
                                username as 'UName',
                                fullname as 'Full Name',
                                role as 'Role',
                                location as 'Location Gateway',
                                update_by as 'Created By',
                                update_time as 'Created Time'
                                from dhl_cs_userdata
                                where ";

                if (string.IsNullOrEmpty(username) == false)
                    parameter = query + username;
                else if (string.IsNullOrEmpty(role) == false)
                    parameter = query + role;
                else
                {
                    LoadData(role_session);
                    return;
                }

                DataTable dt = DbOfficialCeisa.getRecords(parameter);
                ViewState["ListDataUserManagement"] = dt;
                GV_UserManagement.DataSource = dt;
                GV_UserManagement.DataBind();
            }
            catch { }
        }
        #endregion

        #region Export to Excel
        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tbl = (DataTable)ViewState["ListDataUserManagement"];

                //set file name
                string filename = "Userdata_DCI_LITE_" + DateTime.Now.ToString("yyyy-MM-dd");

                string virtualpath = "~/Temp/" + filename;
                string filepath = MapPath("~/Temp/" + filename);
                GenerateExcel(tbl, filename, "", $"{DateTime.Now.ToString("yyyy-MM-dd")}");
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
                        HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
                        HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                        HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.
                    }
                }
            }
            //catch (ThreadAbortException) { }
            catch (Exception Ex)
            {
                Response.Write(Ex.Message);
                //Common.LogError(refNumber, GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name, Ex);
                //ShowModalNotif(string.Format("{0} - {1}", GeneralError(), refNumber), 0, "");
            }
        }
        #endregion
    }
}