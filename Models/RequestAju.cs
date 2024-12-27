using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace OfficialCeisaLite.Models
{
    public class RequestAju
    {
        public string asalData { get; set; }
        public double asuransi { get; set; }
        public double bruto { get; set; }
        public double cif { get; set; }
        public string disclaimer { get; set; }
        public string kodeJenisProsedur { get; set; }
        public string kodeJenisImpor { get; set; }
        public string kodeJenisEkspor { get; set; }
        public string flagVd { get; set; }
        public double fob { get; set; }
        public double freight { get; set; }
        public double hargaPenyerahan { get; set; }
        public string idPengguna { get; set; }
        public string jabatanTtd { get; set; }
        public int jumlahKontainer { get; set; }
        public string jumlahTandaPengaman { get; set; }
        public string kodeAsuransi { get; set; }
        public string kodeCaraBayar { get; set; }
        public string kodeDokumen { get; set; }
        public string kodeIncoterm { get; set; }
        public string kodeJenisNilai { get; set; }
        public string kodeKantor { get; set; }
        public string kodePelMuat { get; set; }
        public string kodePelTujuan { get; set; }
        public string kodeTps { get; set; }
        public string kodeTutupPu { get; set; }
        public string kodeValuta { get; set; }
        public string kotaTtd { get; set; }
        public string namaTtd { get; set; }
        public double ndpbm { get; set; }
        public double netto { get; set; }
        public double nilaiBarang { get; set; }
        public double nilaiIncoterm { get; set; }
        public double nilaiMaklon { get; set; }
        public string nomorAju { get; set; }
        public string nomorBc11 { get; set; }
        public string posBc11 { get; set; }
        public string seri { get; set; }
        public string subposBc11 { get; set; }
        public string tanggalAju { get; set; }
        public string tanggalBc11 { get; set; }
        public string tanggalTiba { get; set; }
        public string tanggalTtd { get; set; }
        public double totalDanaSawit { get; set; }
        public int volume { get; set; }
        public int biayaTambahan { get; set; }
        public int biayaPengurang { get; set; }
        public List<Barang> barang { get; set; }
        public List<Object> entitas { get; set; }
        public List<Kemasan> kemasan { get; set; }
        public List<string> kontainer { get; set; }
        public List<Dokumen> dokumen { get; set; }
        public List<Pengangkut> pengangkut { get; set; }
        public RequestAju()
        {
            barang = new List<Barang>();
            entitas = new List<Object>();
            kemasan = new List<Kemasan>();
            dokumen = new List<Dokumen>();
            pengangkut = new List<Pengangkut>();
        }
    }

    public class Barang
    {
        public double asuransi { get; set; }
        public int bruto { get; set; }
        public double cif { get; set; }
        public double cifRupiah { get; set; }
        public int diskon { get; set; }
        public double fob { get; set; }
        public double freight { get; set; }
        public int hargaEkspor { get; set; }
        public int hargaPatokan { get; set; }
        public int hargaPenyerahan { get; set; }
        public int hargaPerolehan { get; set; }
        public double hargaSatuan { get; set; }
        public int hjeCukai { get; set; }
        public int isiPerKemasan { get; set; }
        public int jumlahBahanBaku { get; set; }
        public int jumlahDilekatkan { get; set; }
        public int jumlahKemasan { get; set; }
        public int jumlahPitaCukai { get; set; }
        public int jumlahRealisasi { get; set; }
        public double jumlahSatuan { get; set; }
        public int kapasitasSilinder { get; set; }
        public string kodeJenisKemasan { get; set; }
        public string kodeKondisiBarang { get; set; }
        public string kodeNegaraAsal { get; set; }
        public string kodeSatuanBarang { get; set; }
        public string merk { get; set; }
        public double ndpbm { get; set; }
        public double netto { get; set; }
        public int nilaiBarang { get; set; }
        public int nilaiDanaSawit { get; set; }
        public int nilaiDevisa { get; set; }
        public int nilaiTambah { get; set; }
        public string pernyataanLartas { get; set; }
        public int persentaseImpor { get; set; }
        public string posTarif { get; set; }
        public int saldoAwal { get; set; }
        public int saldoAkhir { get; set; }
        public int seriBarang { get; set; }
        public string seriBarangDokAsal { get; set; }
        public string seriIjin { get; set; }
        public string tahunPembuatan { get; set; }
        public int tarifCukai { get; set; }
        public string tipe { get; set; }
        public string uraian { get; set; }
        public int volume { get; set; }
        public List<string> barangVd { get; set; }
        public List<BarangTarif> barangTarif { get; set; }
        public List<string> barangDokumen { get; set; }
        public List<string> barangSpekKhusus { get; set; }
        public List<string> barangPemilik { get; set; }
        public Barang()
        {
            barangTarif = new List<BarangTarif>();
        }
    }
    public class BarangTarif
    {
        public string kodeJenisTarif { get; set; }
        public int jumlahSatuan { get; set; }
        public string kodeFasilitasTarif { get; set; }
        public string kodeJenisPungutan { get; set; }
        public double nilaiBayar { get; set; }
        public int seriBarang { get; set; }
        public double tarif { get; set; }
        public int tarifFasilitas { get; set; }
        public int nilaiFasilitas { get; set; }
    }
    public class Entitas
    {
        public string alamatEntitas { get; set; }
        public string kodeEntitas { get; set; }
        public string kodeJenisApi { get; set; }
        public string kodeJenisIdentitas { get; set; }
        public string kodeStatus { get; set; }
        public string namaEntitas { get; set; }
        public string nibEntitas { get; set; }
        public string nomorIdentitas { get; set; }
        public string seriEntitas { get; set; }
        public string kodeNegara { get; set; }
    }
    public class Kemasan
    {
        public int jumlahKemasan { get; set; }
        public string kodeJenisKemasan { get; set; }
        public string merkKemasan { get; set; }
        public int seriKemasan { get; set; }
    }
    public class Dokumen
    {
        public string kodeDokumen { get; set; }
        public string nomorDokumen { get; set; }
        public string seriDokumen { get; set; }
        public string tanggalDokumen { get; set; }
    }
    public class Pengangkut
    {
        public string kodeBendera { get; set; }
        public string namaPengangkut { get; set; }
        public string nomorPengangkut { get; set; }
        public string kodeCaraAngkut { get; set; }
        public string seriPengangkut { get; set; }
    }

    public class NullToEmptyListResolver : DefaultContractResolver
    {
        protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            IValueProvider provider = base.CreateMemberValueProvider(member);

            if (member.MemberType == MemberTypes.Property)
            {
                Type propType = ((PropertyInfo)member).PropertyType;
                if (propType.IsGenericType &&
                    propType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return new EmptyListValueProvider(provider, propType);
                }
            }

            return provider;
        }

        public class EmptyListValueProvider : IValueProvider
        {
            private IValueProvider innerProvider;
            private object defaultValue;

            public EmptyListValueProvider(IValueProvider innerProvider, Type listType)
            {
                this.innerProvider = innerProvider;
                defaultValue = Activator.CreateInstance(listType);
            }

            public void SetValue(object target, object value)
            {
                innerProvider.SetValue(target, value ?? defaultValue);
            }

            public object GetValue(object target)
            {
                return innerProvider.GetValue(target) ?? defaultValue;
            }
        }
    }
}