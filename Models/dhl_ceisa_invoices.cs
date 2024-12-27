using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class dhl_ceisa_invoices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string HAWB {get;set;}
        public string TGL_HAWB { get; set; }
        public string DOC_NO { get; set; }
        public string DOC_TYPE { get; set; }
        public string DOC_DATE { get; set; }
    }
}