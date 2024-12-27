using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class dhl_ceisa_items
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string HAWB { get; set; }

        public string TGL_HAWB { get; set; }

        public string PosTarif { get; set; }

        public string KodeBarang { get; set; }

        [Column(TypeName = "text")]
        public string Uraian { get; set; }

        public string Merk { get; set; }

        public string Ukuran { get; set; }

        public string Lainnya { get; set; }

        public string JumlahBarang { get; set; }

        public string kodeJenisKemasan { get; set; }

        public string JenisKemasan { get; set; }

        public string kodeKategoriBarang { get; set; }

        public string KetegoriBarang { get; set; }

        public string kodeNegaraAsal { get; set; }

        public string NegaraAsal { get; set; }

        public string JumlahSatuan { get; set; }

        public string kodeSatuanBarang { get; set; }

        public string netto { get; set; }

        public string cif { get; set; }

        public string cifRupiah { get; set; }
        public string Pajak { get; set; }
    }
}