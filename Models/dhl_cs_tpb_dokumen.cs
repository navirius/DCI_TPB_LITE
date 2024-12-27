using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class dhl_cs_tpb_dokumen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string HAWB { get; set; }
        public DateTime TGL_HAWB { get; set; }
        public string idDokumen { get; set; }
        public string kodeDokumen { get; set; }
        public string nomorDokumen { get; set; }
        public int seriDokumen { get; set; }
        public DateTime tanggalDokumen { get; set; }
    }
}