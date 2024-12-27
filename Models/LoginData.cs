using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class LoginData
    {
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string location { get; set; }
        public string fullname { get; set; }
    }
}