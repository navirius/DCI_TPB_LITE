using CrystalDecisions.CrystalReports.Engine;
using Octopus.Library.Utils;
using OfficialCeisaLite.App_Start;
using OfficialCeisaLite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;

namespace OfficialCeisaLite.Views
{
    public partial class PreviewReport : System.Web.UI.Page
    {
        private ReportContext _context = new ReportContext();
        private Config config = new Config(AppDomain.CurrentDomain.BaseDirectory + @"CONFIG.xml");
        NbLogger logger = new NbLogger(AppDomain.CurrentDomain.BaseDirectory + @"/Log/", "TPB_LOG_REPORT_BC_23");
        protected void Page_Load(object sender, EventArgs e)
        {
            DbOfficialCeisa.LoadParameter();
            SqlConnection conn = new SqlConnection(DbOfficialCeisa.getConnectionString());
            conn.Open();
            string _ReportCheck = $@"SELECT * FROM dhl_ceisa_reports WHERE HAWB = '{Request.QueryString["HAWB"]}'";
            //DataTable ReportCheck = DbOfficialCeisa.getRecords(_ReportCheck);
            //if (ReportCheck.Rows.Count < 1)
            //{
            //    GenerateReport(Request.QueryString["HAWB"], Request.QueryString["TGL"]);
            //}

            RefreshReport(Request.QueryString["HAWB"], Request.QueryString["TGL"]);
            GenerateReport(Request.QueryString["HAWB"], Request.QueryString["TGL"]);

            SqlCommand cmd = new SqlCommand(_ReportCheck, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            conn.Close();
            GenerateMultiReport(Request.QueryString["HAWB"], Request.QueryString["TGL"]);
                
        }

        private string pajakStatus(string hawb, string date)
        {
            string _Result = string.Empty;
            DbOfficialCeisa.LoadParameter();
            SqlConnection conn = new SqlConnection(DbOfficialCeisa.getConnectionString());
            string _Query = $@"SELECT DISTINCT a.kodeJenisPungutan, CAST(a.tarif AS FLOAT) AS tarif, CAST(a.tarifFasilitas AS INT) AS tarifFasilitas, b.Alias AS status FROM dhl_cs_tpb_barang_tarif AS a INNER JOIN dhl_cs_fasilitas_tarif AS b ON a.kodeFasilitasTarif = b.Id INNER JOIN dhl_cs_tpb_barang AS c ON a.seriBarang = c.seriBarang WHERE a.HAWB = '{hawb}' AND a.TGL_HAWB = '{date}' AND tarif > 0 AND tarifFasilitas > 0";
            SqlCommand cmd = new SqlCommand(_Query, conn);
            conn.Open();
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    _Result += sdr["kodeJenisPungutan"].ToString() + " " + sdr["tarif"] + "% " + sdr["tarifFasilitas"] + "% " + sdr["status"] + "<br>";
                }
            }

            conn.Close();

            return _Result;
        }

        private void GenerateMultiReport(string hawb, string date)
        {
            DbOfficialCeisa.LoadParameter();
            string SingleTemplate = "~/Templates/BC23/SingleDraft.rpt";
            string MultiTemplate = "~/Templates/BC23/MultiDraft.rpt";
            string MultiTemplateRelease = "~/Templates/BC23/MultiRelease.rpt";
            string SingleRelease = "~/Templates/BC23/SingleRelease.rpt";
            SqlConnection conn = new SqlConnection(DbOfficialCeisa.getConnectionString());
            conn.Open();
            string _ReportCheck = $@"SELECT * FROM dhl_ceisa_reports WHERE HAWB = '{hawb}'";
            string _ReportItems = $@"SELECT * FROM dhl_ceisa_items WHERE HAWB = '{hawb}'";
            string _ReportPajak = $@"SELECT * FROM dhl_cs_tarifs WHERE HAWB = '{hawb}'";
            string _ReportDokumen = $@"SELECT * FROM dhl_ceisa_invoices WHERE HAWB = '{hawb}'";
            SqlCommand cmd1 = new SqlCommand(_ReportCheck, conn);
            SqlCommand cmd2 = new SqlCommand(_ReportPajak, conn);
            SqlCommand cmd3 = new SqlCommand(_ReportItems, conn);
            SqlCommand cmd4 = new SqlCommand(_ReportDokumen, conn);
            SqlDataAdapter adapter1 = new SqlDataAdapter(cmd1);
            SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
            SqlDataAdapter adapter3 = new SqlDataAdapter(cmd3);
            SqlDataAdapter adapter4 = new SqlDataAdapter(cmd4);
            DataSet Ds1 = new DataSet();
            DataSet Ds2 = new DataSet();
            DataSet Ds3 = new DataSet();
            DataSet Ds4 = new DataSet();
            adapter1.Fill(Ds1, "dhl_ceisa_reports");
            adapter2.Fill(Ds2, "dhl_cs_tarifs");
            adapter3.Fill(Ds3, "dhl_ceisa_items");
            adapter4.Fill(Ds4, "dhl_ceisa_invoices");
            ReportDocument reportDocument = new ReportDocument();
            
            if (ChooseForm(hawb,date) > 1)
            {
                if (DraftStatus(hawb, date) < 1)
                {
                    reportDocument.Load(Server.MapPath(MultiTemplate));
                }
                else
                {
                    reportDocument.Load(Server.MapPath(MultiTemplateRelease));
                }
            }
            else
            {
                if (DraftStatus(hawb, date) < 1)
                {
                    reportDocument.Load(Server.MapPath(SingleTemplate));
                }
                else
                {
                    reportDocument.Load(Server.MapPath(SingleRelease));
                }
            }


            for(int index = 0; index < reportDocument.DataSourceConnections.Count - 1;index++)
            {
                reportDocument.DataSourceConnections[index].SetConnection("MYKULWSPC000159,1525","db_ceisa_lite", false);
            }

            reportDocument.SetDatabaseLogon(config.GetValue("LocalUser"), config.GetValue("LocalPassword"));
            reportDocument.Database.Tables[0].SetDataSource(Ds1.Tables["dhl_ceisa_reports"]);
            reportDocument.Database.Tables[1].SetDataSource(Ds2.Tables["dhl_cs_tarifs"]);
            reportDocument.Database.Tables[2].SetDataSource(Ds3.Tables["dhl_ceisa_items"]);
            reportDocument.Database.Tables[3].SetDataSource(Ds4.Tables["dhl_ceisa_invoices"]);
            string filename = $"{hawb}_bc23";
            reportDocument.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Response, false, filename);
            //reportDocument.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            CrystalReportViewer1.ReportSource = reportDocument;
            CrystalReportViewer1.DataBind();
            CrystalReportViewer1.HasToggleGroupTreeButton = false;           
            CrystalReportViewer1.RefreshReport();
            reportDocument.Refresh();
            
        }

        private int ChooseForm(string hawb, string date)
        {
            DbOfficialCeisa.LoadParameter();
            SqlConnection conn = new SqlConnection(DbOfficialCeisa.getConnectionString());
            string _ItemCounter = $@"SELECT * FROM dhl_ceisa_items WHERE HAWB = '{hawb}'";
            DataTable ReportCheck = DbOfficialCeisa.getRecords(_ItemCounter);
            return ReportCheck.Rows.Count;
        }

        private int DraftStatus(string hawb, string date)
        {
            DbOfficialCeisa.LoadParameter();
            SqlConnection conn = new SqlConnection(DbOfficialCeisa.getConnectionString());
            string _ItemCounter = $@"SELECT * FROM dhl_cs_response_data WHERE HAWB = '{hawb}' AND TGL_HAWB = '{date}' AND keterangan = 'SPPB'";
            DataTable StatusCounter = DbOfficialCeisa.getRecords(_ItemCounter);
            return StatusCounter.Rows.Count;
        }

        private void GenerateTarif(string hawb, string date)
        {
            DbOfficialCeisa.LoadParameter();
            SqlConnection conn = new SqlConnection(DbOfficialCeisa.getConnectionString());
            string _QueryString = $@"
                SELECT * FROM (
                SELECT 
                CASE 
	                WHEN kodeJenisPungutan = 'BM' THEN '1'
	                WHEN kodeJenisPungutan = 'BMTP' THEN '2'
	                WHEN kodeJenisPungutan = 'Cukai' THEN '3'
	                WHEN kodeJenisPungutan = 'PPn' THEN '4'
	                WHEN kodeJenisPungutan = 'PPnBM' THEN '5'
	                WHEN kodeJenisPungutan = 'PPh' THEN '6'
                END AS [No],
                CASE
	                WHEN kodeJenisPungutan = 'BMTP' THEN 'BMT'
	                ELSE kodeJenisPungutan
                END AS kodeJenisPungutan, kodeFasilitasTarif, SUM(nilaiBayar) AS SUBTOTAL 
                FROM dhl_cs_tpb_barang_tarif INNER JOIN dhl_cs_kode_jenis_pungutan AS B
                ON kodeJenisPungutan = B.Kode
                WHERE HAWB = '{hawb}' AND TGL_HAWB = '{date}'
                GROUP BY kodeJenisPungutan, kodeFasilitasTarif
                ) AS myDerivedTable
                ORDER BY [No] ASC
            ";

            SqlCommand cmd = new SqlCommand(_QueryString, conn);
            conn.Open();

            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    if (sdr["kodeFasilitasTarif"].ToString() == "3")
                    {
                        int tarif_DIT = 0;
                        int tarif_DIT_ori = 0;

                        tarif_DIT_ori = (int)float.Parse(sdr["SUBTOTAL"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        if (tarif_DIT_ori > 0) tarif_DIT = tarif_DIT_ori % 1000 >= 500 ? tarif_DIT_ori + 1000 - tarif_DIT_ori % 1000 : tarif_DIT_ori + 1000 - tarif_DIT_ori % 1000;

                        _context.dhl_cs_tarifs.Add(new dhl_cs_tarifs
                        {
                            HAWB = hawb,
                            TGL_HAWB = date,
                            JENIS = sdr["kodeJenisPungutan"].ToString(),
                            DIT = tarif_DIT,
                            DIB = (int)float.Parse("0", CultureInfo.InvariantCulture.NumberFormat),
                            TDP = (int)float.Parse("0", CultureInfo.InvariantCulture.NumberFormat),
                        });

                        _context.SaveChanges();
                    }

                    if (sdr["kodeFasilitasTarif"].ToString() == "5")
                    {
                        int tarif_DIB = 0;
                        int tarif_DIB_ori = 0;

                        tarif_DIB_ori = (int)float.Parse(sdr["SUBTOTAL"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        if (tarif_DIB_ori > 0) tarif_DIB = tarif_DIB_ori % 1000 >= 500 ? tarif_DIB_ori + 1000 - tarif_DIB_ori % 1000 : tarif_DIB_ori + 1000 - tarif_DIB_ori % 1000;

                        _context.dhl_cs_tarifs.Add(new dhl_cs_tarifs
                        {
                            HAWB = hawb,
                            TGL_HAWB = date,
                            JENIS = sdr["kodeJenisPungutan"].ToString(),
                            DIT = 1 * Convert.ToInt32("0"),
                            DIB = tarif_DIB,
                            TDP = 1 * Convert.ToInt32("0"),
                        });

                        _context.SaveChanges();
                    }

                    if (sdr["kodeFasilitasTarif"].ToString() == "6")
                    {
                        int tarif_TDP = 0;
                        int tarif_TDP_ori = 0;

                        tarif_TDP_ori = (int)float.Parse(sdr["SUBTOTAL"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        if (tarif_TDP_ori > 0) tarif_TDP = tarif_TDP_ori % 1000 >= 500 ? tarif_TDP_ori + 1000 - tarif_TDP_ori % 1000 : tarif_TDP_ori + 1000 - tarif_TDP_ori % 1000;

                        _context.dhl_cs_tarifs.Add(new dhl_cs_tarifs
                        {
                            HAWB = hawb,
                            TGL_HAWB = date,
                            JENIS = sdr["kodeJenisPungutan"].ToString(),
                            DIT = 1 * Convert.ToInt32("0"),
                            DIB = 1 * Convert.ToInt32("0"),
                            TDP = tarif_TDP,
                        });

                        _context.SaveChanges();
                    }

                }
            }

            conn.Close();

        }

        private void GenerateInvoice(string hawb, string date)
        {
            DbOfficialCeisa.LoadParameter();
            SqlConnection conn = new SqlConnection(DbOfficialCeisa.getConnectionString());
            string _QueryString = $@"SELECT a.HAWB, FORMAT(a.TGL_HAWB, 'dd-MM-yyyy') AS TGL_HAWB, b.form, a.nomorDokumen, FORMAT(a.tanggalDokumen,'dd-MM-yyyy') AS tanggalDokumen FROM dhl_cs_tpb_dokumen a 
                INNER JOIN dhl_cs_document AS b ON a.kodeDokumen = b.kode
                WHERE HAWB = '{hawb}' AND TGL_HAWB = '{date}'
                ORDER BY a.tanggalDokumen DESC
            ";

            SqlCommand cmd = new SqlCommand(_QueryString, conn);
            conn.Open();

            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    _context.dhl_ceisa_invoices.Add(new dhl_ceisa_invoices { 
                        HAWB = sdr["HAWB"].ToString(),
                        TGL_HAWB = sdr["TGL_HAWB"].ToString(),
                        DOC_NO = sdr["nomorDokumen"].ToString(),
                        DOC_TYPE = sdr["form"].ToString(),
                        DOC_DATE = sdr["tanggalDokumen"].ToString()
                    });

                    _context.SaveChanges();
                }
            }

            conn.Close();
        }

        public void GenerateReport(string hawb, string date)
        {
            string json = string.Empty;

            string _QueryCheckItem = $"SELECT * FROM dhl_ceisa_items WHERE HAWB = '{hawb}' ";
            string _QueryCheckPemasok = $"SELECT * FROM dhl_ceisa_pemasok WHERE HAWB = '{hawb}'";
            string _QueryCheckPemilik = $"SELECT * FROM dhl_ceisa_pemilik WHERE HAWB = '{hawb}'";
            string _QueryCheckPPJK = $"SELECT * FROM dhl_ceisa_ppjk WHERE HAWB = '{hawb}'";
            string _QueryCheckImportir = $"SELECT * FROM dhl_ceisa_importir WHERE HAWB = '{hawb}'";
            string _QueryCheckTarif = $"SELECT * FROM dhl_cs_tarifs WHERE HAWB = '{hawb}'";
            string _QueryCheckInvoice = $"SELECT * FROM dhl_ceisa_invoices WHERE HAWB = '{hawb}' AND TGL_HAWB = '{date}'";

            DbOfficialCeisa.LoadParameter();
            DataTable ItemData = DbOfficialCeisa.getRecords(_QueryCheckItem);
            DataTable PemasokData = DbOfficialCeisa.getRecords(_QueryCheckPemasok);
            DataTable PemilikData = DbOfficialCeisa.getRecords(_QueryCheckPemilik);
            DataTable PPJKData = DbOfficialCeisa.getRecords(_QueryCheckPPJK);
            DataTable ImportirData = DbOfficialCeisa.getRecords(_QueryCheckImportir);
            DataTable TarifData = DbOfficialCeisa.getRecords(_QueryCheckTarif);
            DataTable InvoiceData = DbOfficialCeisa.getRecords(_QueryCheckInvoice);

            if (ItemData.Rows.Count < 1 || PemasokData.Rows.Count < 1 || PemilikData.Rows.Count < 1 || PPJKData.Rows.Count < 1 || ImportirData.Rows.Count < 1 || TarifData.Rows.Count < 1 || InvoiceData.Rows.Count < 1)
            {
                GenerateEntitas(hawb, date, 3);
                GenerateEntitas(hawb, date, 4);
                GenerateEntitas(hawb, date, 5);
                GenerateEntitas(hawb, date, 7);
                GenerateTarif(hawb, date);
                GenerateItem(hawb, date);
                GenerateInvoice(hawb, date);
            }

            string _QueryString = 
                $"SELECT a.HAWB, FORMAT(a.TGL_HAWB,'dd-MM-yyyy') AS TGL_HAWB, b.form AS Form, a.kodeTujuanTpb, a.nomorAju, " +
                $"a.nomorBc11, a.posBc11, a.subPosBc11, FORMAT(a.tanggalBc11,'dd-MM-yyyy') AS tanggalBc11, f.Kantor AS kantorBongkar, " +
                $"a.kodeKantorBongkar, g.Kantor AS kantorPabean, a.kodeKantor, " +
                $"UPPER(c.Name) AS PelBongkar, a.kodePelBongkar, UPPER(d.Name) AS PelTransit, " +
                $"a.kodePelTransit, UPPER(e.Name) AS PelMuat, a.kodePelMuat, h.Perusahaan AS Pemasok, " +
                $"h.Alamat AS AlamatPemasok, h.Negara AS AsalPemasok, h.ISO AS KodeAsalPemasok, " +
                $"i.Perusahaan AS Importir, i.Alamat AS AlamatImportir, i.Negara AS AsalImportir, " +
                $"i.NPWP AS NPWPImportir, i.NomorIjin AS IjinImportir, i.TanggalIjin AS TerbitIjinImportir, " +
                $"i.NIB AS NIB, j.Perusahaan AS Pemilik, j.Alamat AS AlamatPemilik, j.NPWP AS NPWPPemilik, " +
                $"k.Perusahaan AS Ppjk, k.Alamat AS AlamatPpjk, k.NPWP AS NPWPPpjk, m.Jenis AS JenisAngkutan, " +
                $"l.kodeCaraAngkut, n.Nama AS namaTps, a.kodeTps, l.namaPengangkut AS Maskapai, l.kodeBendera AS BenderaMaskapai, r.nama AS AsalMaskapai, UPPER(o.Nama) As namaValuta, a.kodeValuta, " +
                $"a.ndpbm, a.fob, a.freight, a.asuransi, a.cif, a.bruto, a.netto, q.Nama AS namaKemasan, l.nomorPengangkut, " +
                $"(SELECT merkKemasan FROM dhl_cs_tpb_kemasan WHERE HAWB = a.HAWB) AS JenisKemasan, " +
                $"(SELECT SUM(jumlahKemasan) FROM dhl_cs_tpb_kemasan WHERE HAWB = a.HAWB) AS TotalKemasan, " +
                $"(SELECT TOP 1 nomorDokumen FROM dhl_cs_tpb_dokumen WHERE HAWB = a.HAWB AND kodeDokumen = '380') AS Invoice, " +
                $"(SELECT TOP 1 FORMAT(tanggalDokumen,'dd-MM-yyyy') FROM dhl_cs_tpb_dokumen WHERE HAWB = a.HAWB AND kodeDokumen = '380') AS TglInvoice, " +
                $"(SELECT SUM(nilaiBayar) FROM dhl_cs_tpb_barang_tarif WHERE HAWB = a.HAWB AND TGL_HAWB = a.TGL_HAWB AND kodeJenisPungutan = 'BM') AS NilaiBayarBM," +
                $"(SELECT SUM(nilaiBayar) FROM dhl_cs_tpb_barang_tarif WHERE HAWB = a.HAWB AND TGL_HAWB = a.TGL_HAWB AND kodeJenisPungutan = 'PPN') AS NilaiBayarPPN," +
                $"(SELECT SUM(nilaiBayar) FROM dhl_cs_tpb_barang_tarif WHERE HAWB = a.HAWB AND TGL_HAWB = a.TGL_HAWB AND kodeJenisPungutan = 'PPH') AS NilaiBayarPPH," +
                $"(SELECT TOP 1 nomorDaftar FROM dhl_cs_response_data WHERE HAWB = a.HAWB) AS KodeDaftar, " +
                $"(SELECT TOP 1 convert(varchar(10),tanggalDaftar,20) FROM dhl_cs_response_data WHERE HAWB = a.HAWB) AS TanggalDaftar, " +
                $"k.Pejabat, k.Jabatan, FORMAT(a.tanggalTtd, 'dd-MM-yyyy') AS tanggalTtd, a.kotaTtd, " +
                $"(SELECT nomorDokumen FROM dhl_cs_tpb_dokumen WHERE HAWB = a.HAWB AND kodeDokumen = 741) AS MAWB, " +
                $"(SELECT DISTINCT FORMAT(tanggalDokumen,'dd-MM-yyyy') from dhl_cs_tpb_dokumen WHERE kodeDokumen = 741 AND HAWB = a.HAWB) AS TGL_MAWB, " +
                $"(SELECT COUNT(*) FROM dhl_cs_tpb_barang WHERE HAWB = a.HAWB AND TGL_HAWB = a.TGL_HAWB ) AS TotalItem, u.description AS KodePajak " +
                $"FROM dhl_cs_tpb_header AS a INNER JOIN dhl_cs_document AS b ON a.kodeDokumen = b.kode " +
                $"INNER JOIN dhl_cs_pelabuhan AS c ON RIGHT(a.kodePelBongkar,3) = c.IATA " +
                $"INNER JOIN dhl_cs_pelabuhan AS d ON RIGHT(a.kodePelTransit,3) = d.IATA " +
                $"INNER JOIN dhl_cs_pelabuhan AS e ON RIGHT(a.kodePelMuat,3) = e.IATA " +
                $"INNER JOIN dhl_cs_kantor_pabean AS f ON a.kodeKantorBongkar = f.Kode " +
                $"INNER JOIN dhl_cs_kantor_pabean AS g ON a.kodeKantor = g.Kode " +
                $"INNER JOIN dhl_ceisa_pemasok AS h ON a.HAWB = h.HAWB " +
                $"INNER JOIN dhl_ceisa_importir AS i ON a.HAWB = i.HAWB " +
                $"INNER JOIN dhl_ceisa_pemilik AS j ON a.HAWB = j.HAWB " +
                $"INNER JOIN dhl_ceisa_ppjk AS k ON a.HAWB = k.HAWB " +
                $"INNER JOIN dhl_cs_tpb_pengangkut AS l ON a.HAWB = l.HAWB " +
                $"INNER JOIN dhl_cs_angkutan AS m ON l.kodeCaraAngkut = m.id " +
                $"INNER JOIN dhl_cs_penimbun AS n ON a.kodeTps = n.Kode " +
                $"INNER JOIN dhl_cs_valuta AS o ON a.kodeValuta = o.Kode " +
                $"INNER JOIN dhl_cs_tpb_kemasan AS p ON a.HAWB = p.HAWB " +
                $"INNER JOIN dhl_cs_kemasan AS q ON p.kodeJenisKemasan = q.kode " +
                $"INNER JOIN dhl_cs_negara AS r ON l.kodeBendera = r.kode " +
                $"INNER JOIN dhl_cs_kenapajak AS u ON a.kodeKenaPajak = u.kode " +
                $"WHERE a.HAWB = '{hawb}' AND a.TGL_HAWB = '{date}'";

            SqlConnection con = new SqlConnection(DbOfficialCeisa.getConnectionString());
            SqlCommand cmd = new SqlCommand(_QueryString, con);
            con.Open();

            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    _context.dhl_ceisa_reports.Add(new dhl_ceisa_reports
                    {
                        HAWB = sdr["HAWB"].ToString(),
                        TGL_AWB = sdr["TGL_HAWB"].ToString(),
                        Form = sdr["Form"].ToString(),
                        kodeTujuanTpb = sdr["kodeTujuanTpb"].ToString(),
                        nomorAju = sdr["nomorAju"].ToString(),
                        nomorBc11 = sdr["nomorBc11"].ToString(),
                        posBc11 = sdr["posBc11"].ToString(),
                        subPosBc11 = $"{sdr["subPosBc11"]}0000",
                        tanggalBc11 = sdr["tanggalBc11"].ToString(),
                        kantorBongkar = sdr["kantorBongkar"].ToString(),
                        kodeKantorBongkar = sdr["kodeKantorBongkar"].ToString(),
                        kantorPabean = sdr["kantorPabean"].ToString(),
                        kodeKantor = sdr["kodeKantor"].ToString(),
                        PelBongkar = sdr["PelBongkar"].ToString(),
                        kodePelBongkar = sdr["kodePelBongkar"].ToString(),
                        PelTransit = sdr["PelTransit"].ToString(),
                        kodePelTransit = sdr["kodePelTransit"].ToString(),
                        PelMuat = sdr["PelMuat"].ToString(),
                        kodePelMuat = sdr["kodePelMuat"].ToString(),
                        Pemasok = sdr["Pemasok"].ToString(),
                        AlamatPemasok = sdr["AlamatPemasok"].ToString(),
                        AsalPemasok = sdr["AsalPemasok"].ToString(),
                        KodeAsalPemasok = sdr["KodeAsalPemasok"].ToString(),
                        Importir = sdr["Importir"].ToString(),
                        AlamatImportir = sdr["AlamatImportir"].ToString(),
                        AsalImportir = sdr["AsalImportir"].ToString(),
                        NPWPImportir = sdr["NPWPImportir"].ToString(),
                        IjinImportir = sdr["IjinImportir"].ToString(),
                        TerbitIjinImportir = sdr["TerbitIjinImportir"].ToString(),
                        NIB = sdr["NIB"].ToString(),
                        Pemilik = sdr["Pemilik"].ToString(),
                        AlamatPemilik = sdr["AlamatPemilik"].ToString(),
                        NPWPPemilik = sdr["NPWPPemilik"].ToString(),
                        Ppjk = sdr["Ppjk"].ToString(),
                        AlamatPpjk = sdr["AlamatPpjk"].ToString(),
                        NPWPPpjk = sdr["NPWPPpjk"].ToString(),
                        JenisAngkutan = sdr["JenisAngkutan"].ToString(),
                        kodeCaraAngkut = sdr["kodeCaraAngkut"].ToString(),
                        Maskapai = $"{sdr["Maskapai"]}/{sdr["nomorPengangkut"]}",
                        BenderaMaskapai = sdr["BenderaMaskapai"].ToString(),
                        AsalMaskapai = sdr["AsalMaskapai"].ToString(),
                        namaTps = sdr["namaTps"].ToString(),
                        kodeTps = sdr["kodeTps"].ToString(),
                        namaValuta = sdr["namaValuta"].ToString(),
                        kodeValuta = sdr["kodeValuta"].ToString(),
                        ndpbm = Math.Round(Convert.ToDouble(sdr["ndpbm"]), 2).ToString("########.00"),
                        fob = sdr["fob"].ToString(),
                        freight = sdr["freight"].ToString(),
                        asuransi = sdr["asuransi"].ToString(),
                        cif = sdr["cif"].ToString(),
                        bruto = sdr["bruto"].ToString(),
                        netto = sdr["netto"].ToString(),
                        namaKemasan = sdr["namaKemasan"].ToString(),
                        JenisKemasan = sdr["JenisKemasan"].ToString(),
                        TotalKemasan = sdr["TotalKemasan"].ToString(),
                        Invoice = sdr["Invoice"].ToString(),
                        TglInvoice = sdr["TglInvoice"].ToString(),
                        TotalItem = sdr["TotalItem"].ToString(),
                        KodeDaftar = sdr["KodeDaftar"].ToString(),
                        TanggalDaftar = sdr["TanggalDaftar"].ToString(),
                        Pejabat = sdr["Pejabat"].ToString(),
                        Jabatan = sdr["Jabatan"].ToString(),
                        tanggalTtd = sdr["tanggalTtd"].ToString(),
                        MAWB = sdr["MAWB"].ToString(),
                        TGL_MAWB = sdr["TGL_MAWB"].ToString(),
                        NilaiBayarBM = sdr["NilaiBayarBM"].ToString(),
                        NilaiBayarPPN = sdr["NilaiBayarPPN"].ToString(),
                        NilaiBayarPPH = sdr["NilaiBayarPPH"].ToString(),
                        KodePajak = sdr["KodePajak"].ToString(),
                        Gateway = sdr["kotaTtd"].ToString()                    
                    });

                    _context.SaveChanges();
                    
                }

            }

            con.Close();

        }

        public void GenerateItem(string hawb, string date)
        {
            string _QueryString = $@"
            SELECT 
            a.posTarif AS PosTarif, 
            a.kodeBarang AS KodeBarang, 
            a.uraian AS Uraian, 
            a.merk AS Merk, 
            a.ukuran AS Ukuran, 
            a.spesifikasiLain AS Lainnya, 
            CAST(a.jumlahKemasan AS DECIMAL(11,0)) AS JumlahBarang, 
            a.kodeJenisKemasan,
            b.Nama AS JenisKemasan,
            c.Kode AS kodeKategoriBarang,
            c.Deskripsi AS KetegoriBarang,
            a.kodeNegaraAsal,
            d.nama AS NegaraAsal,
            CAST(a.jumlahSatuan AS DECIMAL(11,0)) AS JumlahSatuan,
            a.kodeSatuanBarang,
            a.netto,
            a.cif,
            a.cifRupiah
            FROM dhl_cs_tpb_barang AS a
            INNER JOIN dhl_cs_kemasan AS b
            ON a.kodeJenisKemasan = b.Kode
            INNER JOIN dhl_cs_kategori_barang AS c
            ON a.kodeKategoriBarang = c.Kode
            INNER JOIN dhl_cs_negara AS d
            ON a.kodeNegaraAsal = d.kode
            WHERE a.HAWB = '{hawb}'
            AND a.TGL_HAWB = '{date}'
            ";

            List<dhl_ceisa_items> _SourceItem = new List<dhl_ceisa_items>();
            SqlConnection con = new SqlConnection(DbOfficialCeisa.getConnectionString());
            SqlCommand cmd = new SqlCommand(_QueryString, con);
            con.Open();
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    _SourceItem.Add(new dhl_ceisa_items
                    {
                        HAWB = hawb,
                        TGL_HAWB = date,
                        PosTarif = sdr["PosTarif"].ToString(),
                        KodeBarang = sdr["KodeBarang"].ToString(),
                        Uraian = sdr["Uraian"].ToString(),
                        Merk = sdr["Merk"].ToString(),
                        Ukuran = sdr["Ukuran"].ToString(),
                        Lainnya = sdr["Lainnya"].ToString(),
                        JumlahBarang = sdr["JumlahBarang"].ToString(),
                        kodeJenisKemasan = sdr["kodeJenisKemasan"].ToString(),
                        JenisKemasan = sdr["JenisKemasan"].ToString(),
                        kodeKategoriBarang = sdr["kodeKategoriBarang"].ToString(),
                        KetegoriBarang = sdr["KetegoriBarang"].ToString(),
                        kodeNegaraAsal = sdr["kodeNegaraAsal"].ToString(),
                        NegaraAsal = sdr["NegaraAsal"].ToString(),
                        JumlahSatuan = sdr["JumlahSatuan"].ToString(),
                        kodeSatuanBarang = sdr["kodeSatuanBarang"].ToString(),
                        netto = sdr["netto"].ToString(),
                        cif = sdr["cif"].ToString(),
                        cifRupiah = sdr["cifRupiah"].ToString(),
                        Pajak = pajakStatus(hawb, date)
                    });

                    _context.dhl_ceisa_items.Add(new dhl_ceisa_items
                    {
                        HAWB = hawb,
                        TGL_HAWB = date,
                        PosTarif = sdr["PosTarif"].ToString(),
                        KodeBarang = sdr["KodeBarang"].ToString(),
                        Uraian = sdr["Uraian"].ToString(),
                        Merk = sdr["Merk"].ToString(),
                        Ukuran = sdr["Ukuran"].ToString(),
                        Lainnya = sdr["Lainnya"].ToString(),
                        JumlahBarang = sdr["JumlahBarang"].ToString(),
                        kodeJenisKemasan = sdr["kodeJenisKemasan"].ToString(),
                        JenisKemasan = sdr["JenisKemasan"].ToString(),
                        kodeKategoriBarang = sdr["kodeKategoriBarang"].ToString(),
                        KetegoriBarang = sdr["KetegoriBarang"].ToString(),
                        kodeNegaraAsal = sdr["kodeNegaraAsal"].ToString(),
                        NegaraAsal = sdr["NegaraAsal"].ToString(),
                        JumlahSatuan = sdr["JumlahSatuan"].ToString(),
                        kodeSatuanBarang = sdr["kodeSatuanBarang"].ToString(),
                        netto = sdr["netto"].ToString(),
                        cif = sdr["cif"].ToString(),
                        cifRupiah = sdr["cifRupiah"].ToString(),
                        Pajak = pajakStatus(hawb,date)
                    });

                    _context.SaveChanges();
                }
            }

            con.Close();
        }

        public void GenerateEntitas(string hawb, string date, int type)
        {
            string _QueryString = $@"SELECT a.HAWB,a.TGL_HAWB,b.nibEntitas AS NIB, a.namaTtd AS Pejabat,c.nama AS Jabatan,b.namaEntitas AS Perusahaan,b.alamatEntitas AS Alamat,country.nama AS Negara,b.kodeNegara AS ISO,b.nomorIdentitas AS NPWP,b.nomorIjinEntitas AS NomorIjin,b.tanggalIjinEntitas AS TanggalIjin FROM dhl_cs_tpb_header AS a
            INNER JOIN dhl_cs_tpb_entitas AS b
            ON a.HAWB = b.HAWB
            INNER JOIN dhl_cs_entitas AS c
            ON b.kodeEntitas = c.kode
            INNER JOIN dhl_cs_negara AS country
            ON b.kodeNegara = country.kode
            WHERE a.HAWB = '{hawb}'
            AND b.kodeEntitas = '{type}'
            AND a.TGL_HAWB = '{date}'";

            SqlConnection con = new SqlConnection(DbOfficialCeisa.getConnectionString());
            SqlCommand cmd = new SqlCommand(_QueryString, con);
            con.Open();
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    if (type == 5)
                    {
                        _context.dhl_ceisa_pemasok.Add(new dhl_ceisa_pemasok
                        {
                            HAWB = sdr["HAWB"].ToString(),
                            NIB = sdr["NIB"].ToString(),
                            Pejabat = sdr["Pejabat"].ToString(),
                            Jabatan = sdr["Jabatan"].ToString(),
                            Perusahaan = sdr["Perusahaan"].ToString(),
                            Alamat = sdr["Alamat"].ToString(),
                            Negara = sdr["Negara"].ToString(),
                            ISO = sdr["ISO"].ToString(),
                            NPWP = sdr["NPWP"].ToString(),
                            NomorIjin = sdr["NomorIjin"].ToString(),
                            TanggalIjin = sdr["TanggalIjin"].ToString()
                        });

                        _context.SaveChanges();
                    }

                    if (type == 3)
                    {
                        _context.dhl_ceisa_importir.Add(new dhl_ceisa_importir
                        {
                            HAWB = sdr["HAWB"].ToString(),
                            NIB = sdr["NIB"].ToString(),
                            Pejabat = sdr["Pejabat"].ToString(),
                            Jabatan = sdr["Jabatan"].ToString(),
                            Perusahaan = sdr["Perusahaan"].ToString(),
                            Alamat = sdr["Alamat"].ToString(),
                            Negara = sdr["Negara"].ToString(),
                            ISO = sdr["ISO"].ToString(),
                            NPWP = sdr["NPWP"].ToString(),
                            NomorIjin = sdr["NomorIjin"].ToString(),
                            TanggalIjin = sdr["TanggalIjin"].ToString()
                        });

                        _context.SaveChanges();
                    }

                    if (type == 4)
                    {
                        _context.dhl_ceisa_ppjk.Add(new dhl_ceisa_ppjk
                        {
                            HAWB = sdr["HAWB"].ToString(),
                            NIB = sdr["NIB"].ToString(),
                            Pejabat = sdr["Pejabat"].ToString(),
                            Jabatan = sdr["Jabatan"].ToString(),
                            Perusahaan = sdr["Perusahaan"].ToString(),
                            Alamat = sdr["Alamat"].ToString(),
                            Negara = sdr["Negara"].ToString(),
                            ISO = sdr["ISO"].ToString(),
                            NPWP = sdr["NPWP"].ToString(),
                            NomorIjin = sdr["NomorIjin"].ToString(),
                            TanggalIjin = sdr["TanggalIjin"].ToString()
                        });

                        _context.SaveChanges();
                    }

                    if (type == 7)
                    {
                        _context.dhl_ceisa_pemilik.Add(new dhl_ceisa_pemilik
                        {
                            HAWB = sdr["HAWB"].ToString(),
                            NIB = sdr["NIB"].ToString(),
                            Pejabat = sdr["Pejabat"].ToString(),
                            Jabatan = sdr["Jabatan"].ToString(),
                            Perusahaan = sdr["Perusahaan"].ToString(),
                            Alamat = sdr["Alamat"].ToString(),
                            Negara = sdr["Negara"].ToString(),
                            ISO = sdr["ISO"].ToString(),
                            NPWP = sdr["NPWP"].ToString(),
                            NomorIjin = sdr["NomorIjin"].ToString(),
                            TanggalIjin = sdr["TanggalIjin"].ToString()
                        });

                        _context.SaveChanges();
                    }

                }
            }
            con.Close();
        }

        public void RefreshReport(string hawb, string date)
        {
            try
            {
                string _QueryTarifs = $"delete from dhl_cs_tarifs where HAWB = '{hawb}'";
                string _QueryReports = $"delete from dhl_cs_tarifs where HAWB = '{hawb}'";
                string _QueryItems = $"delete from dhl_cs_tarifs where HAWB = '{hawb}'";
                string _QueryPemasok = $"delete from dhl_cs_tarifs where HAWB = '{hawb}'";
                string _QueryPemilik = $"delete from dhl_cs_tarifs where HAWB = '{hawb}'";
                string _QueryPpjk = $"delete from dhl_cs_tarifs where HAWB = '{hawb}'";
                string _QueryImportir = $"delete from dhl_cs_tarifs where HAWB = '{hawb}'";
                string _QueryInvoices = $"delete from dhl_cs_tarifs where HAWB = '{hawb}'";

                string query = $@"delete from dhl_cs_tarifs			where HAWB = '{hawb}'; 
                                delete from dhl_ceisa_reports		where HAWB = '{hawb}';
                                delete from dhl_ceisa_items			where HAWB = '{hawb}';
                                delete from dhl_ceisa_pemasok		where HAWB = '{hawb}';
                                delete from dhl_ceisa_pemilik		where HAWB = '{hawb}';
                                delete from dhl_ceisa_ppjk			where HAWB = '{hawb}';
                                delete from dhl_ceisa_importir		where HAWB = '{hawb}';
                                delete from dhl_ceisa_invoices		where HAWB = '{hawb}';";
                if (DbOfficialCeisa.runCommand(query) == 0)
                    logger.Log($"refresh report hawb {hawb}..");
            }
            catch (Exception ex)
            {
                logger.Log($"err RefreshReport: {ex.Message}, {ex.StackTrace}");
            }
        }
    }
}