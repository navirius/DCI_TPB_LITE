using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficialCeisaLite.Models
{
    public class CeisaEntitas
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string HAWB { get; set; }
        public string NIB { get; set; }
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
    }
}