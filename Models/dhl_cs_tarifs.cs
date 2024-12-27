using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class dhl_cs_tarifs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string HAWB { get; set; }
        public string TGL_HAWB { get; set; }
        public string JENIS { get; set; }
        public int DIT { get; set; }
        public int DIB { get; set; }
        public int TDP { get; set; }
    }
}