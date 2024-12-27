using OfficialCeisaLite.App_Start;
using OfficialCeisaLite.Models;
using OfficialCeisaLite.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OfficialCeisaLite.Auth
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            //disable back button
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetAllowResponseInBrowserHistory(false);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSign_Click(object sender, EventArgs e)
        {
            if (VerifyUser() == true)
            {
                LoginData user = SessionUtils.GetUserData(this.Page);
                if (user != null)
                {
                    switch (user.role)
                    {
                        case "IT":
                            Response.Redirect("~/Views/UserManagement.aspx");
                            break;
                        default:
                            Response.Redirect("~/Views/RequestTPB.aspx");
                            break;
                    }
                }
            }
            else
            {
                string msg = ViewState["LoginStatus"].ToString();
                ScriptManager.RegisterStartupScript(this.Page, base.GetType(), "script", $"alert('{msg}')", true);
            }
        }
        protected bool VerifyUser()
        {
            ViewState["LoginStatus"] = null;
            bool bval = false;
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (ValidateUserLDAP(username, password))
            {
                string query = $@"select role, location, fullname from dhl_cs_userdata where username='{username}'";
                DbOfficialCeisa.LoadParameter();
                DataRow dr = DbOfficialCeisa.getRow(query);
                if (dr != null)
                {
                    LoginData user = new LoginData();
                    user.username = username;
                    user.password = password;
                    user.role = dr["role"].ToString();
                    user.location = dr["location"].ToString();
                    user.fullname = dr["fullname"].ToString();
                    SessionUtils.SetUserData(this.Page, user);
                    query = $@"update dhl_cs_userdata set last_login_time = '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff")}'
                            where username = '{username}'";
                    DbOfficialCeisa.runCommand(query);
                    bval = true;
                }
                else
                {
                    ViewState["LoginStatus"] = "Username not registered, please contact your system administrator.";
                }
            }
            else
            {
                ViewState["LoginStatus"] = "Invalid username / password";
            }            

            return bval;
        }

        protected bool ValidateUserLDAP(string username, string password)
        {
            bool retval = true;
            string url = @"edsldap.dhl.com";
            var credentials = new NetworkCredential(username, password);
            var serverId = new LdapDirectoryIdentifier(url);

            var conn = new LdapConnection(serverId, credentials);

            try
            {
                conn.Bind();
            }
            catch (Exception)
            {
                retval = false;
            }

            conn.Dispose();

            return retval;
        }
    }
}