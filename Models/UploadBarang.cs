using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class UploadBarang
    {
        public string HAWB { get; set; }
        public string TGL_HAWB { get; set; }
        public double ASURANSI { get; set; }
        public double CIF { get; set; }
        public double CIF_RUPIAH { get; set; } 
        public double FOB { get; set; }
        public double FREIGHT { get; set; }
        public double HARGA_SATUAN { get; set; }
        public double JUMLAH_SATUAN { get; set; }
        public string KODE_NEGARA_ASAL { get; set; }
        public string KODE_SATUAN_BARANG { get; set; }
        public double NDPBM { get; set; }
        public double NETTO { get; set; }
        public string POS_TARIF { get; set; }
        public int SERI_BARANG { get; set; }
        public double BM_TARIF { get; set; }
        public double CUKAI_TARIF { get; set; }
        public double PPN_TARIF { get; set; }
        public double PPNBM_TARIF { get; set; }
        public double PPH_TARIF { get; set; }
        public string URAIAN { get; set; }
    }
}