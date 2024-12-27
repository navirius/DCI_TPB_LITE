using OfficialCeisaLite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace OfficialCeisaLite.Utils
{
    public class SessionUtils
    {
        public static bool IsLogin(Page page)
        {
            bool bval = true;
            if (page.Session["UserData"] == null)
                bval = false;

            return bval;
        }

        public static bool IsAuthorize(Page page)
        {
            bool bval = false;
            if (page.Session["UserData"] != null)
                bval = true;

            return bval;
        }

        public static void SetUserData(Page page, LoginData user)
        {
            page.Session["UserData"] = user;
        }

        public static LoginData GetUserData(Page page)
        {
            LoginData user = null;
            if (page.Session["UserData"] != null)
                user = (LoginData)page.Session["UserData"];

            return user;
        }
    }
}