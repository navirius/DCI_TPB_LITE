using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class ResponseStatusAju
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<DataStatus> dataStatus { get; set; }
        public List<DataRespon> dataRespon { get; set; }
        public ResponseStatusAju()
        {
            dataStatus = new List<DataStatus>();
            dataRespon = new List<DataRespon>();
        }
    }

    public class DataStatus
    {
        public string nomorAju { get; set; }
        public string nomorDaftar { get; set; }
        public string tanggalDaftar { get; set; }
        public string kodeProses { get; set; }
        public string waktuStatus { get; set; }
        public string keterangan { get; set; }
    }
    public class DataRespon
    {
        public string nomorAju { get; set; }
        public string kodeRespon { get; set; }
        public string nomorDaftar { get; set; }
        public string tanggalDaftar { get; set; }
        public string nomorRespon { get; set; }
        public string tanggalRespon { get; set; }
        public string waktuRespon { get; set; }
        public string keterangan { get; set; }
        public List<string> pesan { get; set; }
        public string pdf { get; set; }
        public string kodeDokumen { get; set; }
        public DataRespon()
        {
            pesan = new List<string>();
        }
    }
    public class Pesan
    {
        public string uraian { get; set; }
    }
}