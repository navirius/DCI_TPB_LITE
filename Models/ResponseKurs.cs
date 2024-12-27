using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class ResponseKurs
    {
        public ResponseKurs()
        {

        }
        public string status { get; set; }
        public string message { get; set; }
        public List<Data> data { get; set; }

    }
    public class Data
    {
        public Data()
        {

        }
        public string kodeValuta { get; set; }
        public string nilaiKurs { get; set; }
        public string tglAwalBerlaku { get; set; }
        public string tglAkhirBerlaku { get; set; }
        public string namaValuta { get; set; }
        public string gambar { get; set; }
    }
}