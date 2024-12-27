using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class dhl_ceisa_reports
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string HAWB { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string TGL_AWB { get; set; }
        public string Form { get; set; }
        public string kodeTujuanTpb { get; set; }
        public string nomorAju { get; set; }
        public string nomorBc11 { get; set; }
        public string posBc11 { get; set; }
        public string subPosBc11 { get; set; }
        public string tanggalBc11 { get; set; }
        public string kantorBongkar { get; set; }
        public string kodeKantorBongkar { get; set; }
        public string kantorPabean { get; set; }
        public string kodeKantor { get; set; }
        public string PelBongkar { get; set; }
        public string kodePelBongkar { get; set; }
        public string PelTransit { get; set; }
        public string kodePelTransit { get; set; }
        public string PelMuat { get; set; }
        public string kodePelMuat { get; set; }
        public string Pemasok { get; set; }
        public string AlamatPemasok { get; set; }
        public string AsalPemasok { get; set; }
        public string KodeAsalPemasok { get; set; }
        public string Importir { get; set; }
        public string AlamatImportir { get; set; }
        public string AsalImportir { get; set; }
        public string NPWPImportir { get; set; }
        public string IjinImportir { get; set; }
        public string TerbitIjinImportir { get; set; }
        public string NIB { get; set; }
        public string Pemilik { get; set; }
        public string AlamatPemilik { get; set; }
        public string NPWPPemilik { get; set; }
        public string Ppjk { get; set; }
        public string AlamatPpjk { get; set; }
        public string NPWPPpjk { get; set; }
        public string JenisAngkutan { get; set; }
        public string kodeCaraAngkut { get; set; }
        public string namaTps { get; set; }
        public string kodeTps { get; set; }
        public string namaValuta { get; set; }
        public string kodeValuta { get; set; }
        public string Maskapai { get; set; }
        public string BenderaMaskapai { get; set; }
        public string AsalMaskapai { get; set; }
        public string ndpbm { get; set; }
        public string fob { get; set; }
        public string freight { get; set; }
        public string asuransi { get; set; }
        public string cif { get; set; }
        public string bruto { get; set; }
        public string netto { get; set; }
        public string namaKemasan { get; set; }
        public string JenisKemasan { get; set; }
        public string TotalKemasan { get; set; }
        public string Invoice { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string TglInvoice { get; set; }
        public string NilaiBayarBM { get; set; }
        public string NilaiBayarPPN { get; set; }
        public string NilaiBayarPPH { get; set; }
        public string TotalItem { get; set; }
        public string KodeDaftar { get; set; }
        public string TanggalDaftar { get; set; }
        public string Pejabat { get; set; } 
        public string Jabatan { get; set; }
        public string tanggalTtd { get; set; }
        public string MAWB { get; set; }
        public string TGL_MAWB { get; set; }
        public string KodePajak { get; set; }
        public string Gateway { get; set; }
    }
}