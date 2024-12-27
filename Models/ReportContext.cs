using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace OfficialCeisaLite.Models
{
    public partial class ReportContext : DbContext
    {
        public ReportContext() : base("name=ReportContext")
        {
        }

        public virtual DbSet<dhl_ceisa_reports> dhl_ceisa_reports { get; set; }
        public virtual DbSet<dhl_ceisa_pemasok> dhl_ceisa_pemasok { get; set; }
        public virtual DbSet<dhl_ceisa_importir> dhl_ceisa_importir { get; set; }
        public virtual DbSet<dhl_ceisa_pemilik> dhl_ceisa_pemilik { get; set; }
        public virtual DbSet<dhl_ceisa_ppjk> dhl_ceisa_ppjk { get; set; }
        public virtual DbSet<dhl_ceisa_items> dhl_ceisa_items { get; set; }
        public virtual DbSet<dhl_cs_tarifs> dhl_cs_tarifs { get; set; }
        public virtual DbSet<dhl_cs_tpb_dokumen> dhl_cs_dokumen { get; set; }
        public virtual DbSet<dhl_ceisa_invoices> dhl_ceisa_invoices { get; set; }
    }
}
