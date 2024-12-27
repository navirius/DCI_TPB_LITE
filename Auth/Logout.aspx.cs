using OfficialCeisaLite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OfficialCeisaLite.Auth
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    SessionUtils.SetUserData(this.Page, null);
                }
                catch { }
                finally
                {
                    Response.Redirect("~/Auth/Login");
                }
            }
        }
    }
}