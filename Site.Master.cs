using OfficialCeisaLite.Models;
using OfficialCeisaLite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OfficialCeisaLite
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Init(Object sender, EventArgs e)
        {
            //disable back button
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetAllowResponseInBrowserHistory(false);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SessionUtils.IsLogin(this.Page) == false)
                    Response.Redirect("~/Auth/Login");

                LoginData user = SessionUtils.GetUserData(this.Page);
                if (user != null)
                {
                    switch (user.role)
                    {
                        case "user":
                            sidebarmenu.InnerHtml = @"<ul class='sidebar-menu' data-widget='tree'>                                                        
                                                        <li hidden='true'>
                                                            <a href='/Views/Request.aspx'>
                                                                <i class='fa fa-user'></i> 
                                                                <span>DCI PIB Lite</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/RequestTPB.aspx'>
                                                                <i class='fa fa-user'></i> 
                                                                <span>DCI TPB Lite</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/QueryHSCode.aspx'>
                                                                <i class='fa fa-book'></i> 
                                                                <span>Query HS Code</span>
                                                            </a>
                                                        </li>
                                                      </ul>";
                            break;
                        case "supervisor":
                            sidebarmenu.InnerHtml = @"<ul class='sidebar-menu' data-widget='tree'>                                                        
                                                        <li hidden='true'>
                                                            <a href='/Views/Request.aspx'>
                                                                <i class='fa fa-user'></i> 
                                                                <span>DCI PIB Lite</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/RequestTPB.aspx'>
                                                                <i class='fa fa-user'></i> 
                                                                <span>DCI TPB Lite</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/QueryHSCode.aspx'>
                                                                <i class='fa fa-book'></i> 
                                                                <span>Query HS Code</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/GtwSettings.aspx'>
                                                                <i class='fa fa-cog'></i> 
                                                                <span>Gateway Settings</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/UserManagement.aspx'>
                                                                <i class='fa fa-user'></i> 
                                                                <span>User Management</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/MasterSkep.aspx'>
                                                                <i class='fa fa-shield'></i> 
                                                                <span>Master Skep</span>
                                                            </a>
                                                        </li>
                                                      </ul>";
                            break;
                        default: //admin
                            sidebarmenu.InnerHtml = @"<ul class='sidebar-menu' data-widget='tree'>                                                        
                                                        <li hidden='true'>
                                                            <a href='/Views/Request.aspx'>
                                                                <i class='fa fa-user'></i> 
                                                                <span>DCI PIB Lite</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/RequestTPB.aspx'>
                                                                <i class='fa fa-user'></i> 
                                                                <span>DCI TPB Lite</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/QueryHSCode.aspx'>
                                                                <i class='fa fa-book'></i> 
                                                                <span>Query HS Code</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/GtwSettings.aspx'>
                                                                <i class='fa fa-cog'></i> 
                                                                <span>Gateway Settings</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/UserManagement.aspx'>
                                                                <i class='fa fa-user'></i> 
                                                                <span>User Management</span>
                                                            </a>
                                                        </li>
                                                        <li>
                                                            <a href='/Views/MasterSkep.aspx'>
                                                                <i class='fa fa-shield'></i> 
                                                                <span>Master Skep</span>
                                                            </a>
                                                        </li>
                                                      </ul>";
                            break;
                    }

                    username_topbar.InnerHtml = user.username;
                    fullname_dropdown.InnerHtml = user.username;
                    role_dropdown.InnerHtml = user.role;
                }                
            }
        }
    }
}