namespace OfficialCeisaLite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class dhl_ceisa_importir
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Pejabat { get; set; }

        public string Jabatan { get; set; }

        public string Perusahaan { get; set; }

        [Column(TypeName = "text")]
        public string Alamat { get; set; }

        public string Negara { get; set; }

        [StringLength(2)]
        public string ISO { get; set; }

        public string NPWP { get; set; }

        public string NomorIjin { get; set; }

        public string TanggalIjin { get; set; }

        public DateTime? AddedAt { get; set; }

        [StringLength(255)]
        public string HAWB { get; set; }

        public string NIB { get; set; }
    }
}
