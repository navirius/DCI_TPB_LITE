using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class ResponseToken
    {
        public string status { get; set; }
        public string message { get; set; }
        public Item item { get; set; }
        public ResponseToken()
        {
            item = new Item();
        }
    }

    public class Item
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public int refresh_expires_in { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string id_token { get; set; }
    }
}