using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class RequestAjuTpb
    {
        public string asalData { get; set; }
        public double asuransi { get; set; }
        public double bruto { get; set; }
        public double cif { get; set; }
        public double fob { get; set; }
        public double freight { get; set; }
        public int hargaPenyerahan { get; set; }
        public string jabatanTtd { get; set; }
        public int jumlahKontainer { get; set; }
        public string kodeAsuransi { get; set; }
        public string kodeDokumen { get; set; }
        public string kodeIncoterm { get; set; }
        public string kodeKantor { get; set; }
        public string kodeKantorBongkar { get; set; }
        public string kodePelBongkar { get; set; }
        public string kodePelMuat { get; set; }
        public string kodePelTransit { get; set; }
        public string kodeTps { get; set; }
        public string kodeTujuanTpb { get; set; }
        public string kodeTutupPu { get; set; }
        public string kodeValuta { get; set; }
        public string kotaTtd { get; set; }
        public string namaTtd { get; set; }
        public double ndpbm { get; set; }
        public double netto { get; set; }
        public string nik { get; set; }
        public double nilaiBarang { get; set; }
        public string nomorAju { get; set; }
        public string nomorBc11 { get; set; }
        public string posBc11 { get; set; }
        public int seri { get; set; }
        public string subposBc11 { get; set; }
        public string tanggalBc11 { get; set; }
        public string tanggalTiba { get; set; }
        public string tanggalTtd { get; set; }
        public int biayaTambahan { get; set; }
        public int biayaPengurang { get; set; }
        public string kodeKenaPajak { get; set; }
        public List<BarangTpb> barang { get; set; }
        public List<Object> entitas { get; set; }
        public List<KemasanTpb> kemasan { get; set; }
        public List<DokumenTpb> dokumen { get; set; }
        public List<PengangkutTpb> pengangkut { get; set; }
        public RequestAjuTpb()
        {
            barang = new List<BarangTpb>();
            entitas = new List<Object>();
            kemasan = new List<KemasanTpb>();
            dokumen = new List<DokumenTpb>();
            pengangkut = new List<PengangkutTpb>();
        }
    }
    public class BarangTpb
    {
        public string idBarang { get; set; }
        public double asuransi { get; set; }        
        public double cif { get; set; }
        public int diskon { get; set; }
        public double fob { get; set; }
        public double freight { get; set; }
        public int hargaEkspor { get; set; }
        public int hargaPenyerahan { get; set; }
        public double hargaSatuan { get; set; }
        public int isiPerKemasan { get; set; }
        public int jumlahKemasan { get; set; }
        public double jumlahSatuan { get; set; }
        public string kodeBarang { get; set; }
        public string kodeDokumen { get; set; }
        public string kodeKategoriBarang { get; set; }
        public string kodeJenisKemasan { get; set; }
        public string kodeNegaraAsal { get; set; }
        public string kodePerhitungan { get; set; }
        public string kodeSatuanBarang { get; set; }
        public string merk { get; set; }
        public double netto { get; set; }
        public double nilaiBarang { get; set; }
        public int nilaiTambah { get; set; }
        public string posTarif { get; set; }
        public int seriBarang { get; set; }
        public string spesifikasiLain { get; set; }
        public string tipe { get; set; }
        public string ukuran { get; set; }
        public string uraian { get; set; }
        public double ndpbm { get; set; }
        public double cifRupiah { get; set; }
        public int hargaPerolehan { get; set; }
        public string kodeAsalBahanBaku { get; set; }
        public List<BarangTarifTpb> barangTarif { get; set; }
        public List<Dictionary<string,string>> barangDokumen { get; set; }
        public BarangTpb()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("seriDokumen", "1");

            barangTarif = new List<BarangTarifTpb>();
            barangDokumen = new List<Dictionary<string,string>>();
            barangDokumen.Add(dict);
        }
    }
    public class BarangTarifTpb
    {
        public string kodeJenisTarif { get; set; }
        public double jumlahSatuan { get; set; }
        public string kodeFasilitasTarif { get; set; }
        public string kodeSatuanBarang { get; set; }
        public string kodeJenisPungutan { get; set; }
        public double nilaiBayar { get; set; }
        public double nilaiFasilitas { get; set; }
        public double nilaiSudahDilunasi { get; set; }
        public int seriBarang { get; set; }
        public double tarif { get; set; }
        public int tarifFasilitas { get; set; }        
    }
    public class EntitasTpb
    {
        public string alamatEntitas { get; set; }
        public string kodeEntitas { get; set; }
        public string kodeJenisApi { get; set; }
        public string kodeJenisIdentitas { get; set; }
        public string kodeStatus { get; set; }
        public string namaEntitas { get; set; }
        public string nibEntitas { get; set; }
        public string nomorIdentitas { get; set; }
        public string nomorIjinEntitas { get; set; }
        public string tanggalIjinEntitas { get; set; }
        public int seriEntitas { get; set; }
        public string kodeNegara { get; set; }
        public string npwp16 { get; set; }
    }
    public class KemasanTpb
    {
        public int jumlahKemasan { get; set; }
        public string kodeJenisKemasan { get; set; }
        public int seriKemasan { get; set; }
        public string merkKemasan { get; set; }
    }
    public class DokumenTpb
    {
        public string idDokumen { get; set; }
        public string kodeDokumen { get; set; }
        public string nomorDokumen { get; set; }
        public int seriDokumen { get; set; }
        public string tanggalDokumen { get; set; }
    }
    public class PengangkutTpb
    {
        public string kodeBendera { get; set; }
        public string namaPengangkut { get; set; }
        public string nomorPengangkut { get; set; }
        public string kodeCaraAngkut { get; set; }
        public int seriPengangkut { get; set; }
    }
}