using Octopus.Library.Utils;
using OfficialCeisaLite.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OfficialCeisaLite.Views
{
    public partial class EditBarang : System.Web.UI.Page
    {
        NbLogger logger = new NbLogger(AppDomain.CurrentDomain.BaseDirectory + @"/Log/", "TPB_EDIT_BARANG");
        protected void Page_Load(object sender, EventArgs e)
        {
            //string HAWB = Request.QueryString["HAWB"].ToString();
            //string TGLHAWB = Request.QueryString["TGLHAWB"].ToString();

            string HAWB = Session["editHAWB"].ToString();
            string TGLHAWB = Session["editTGLHAWB"].ToString();
            if (!Page.IsPostBack)
            {
                LoadData(HAWB, TGLHAWB);
                ViewState["editHAWB"] = HAWB;
                ViewState["editTGLHAWB"] = TGLHAWB;
                return;
            }

            //DataTable dt = (DataTable)ViewState["ListEditBarang"];
            //GV_EditBarang.DataSource = dt;
            //GV_EditBarang.DataBind();
        }
        private void LoadData(string HAWB, string TGLHAWB)
        {
            DbOfficialCeisa.LoadParameter();
            try
            {
                string query = $@"select id as [ID],
                                HAWB, convert(char(10),TGL_HAWB,126) as TGL_HAWB,
                                asuransi as [ASURANSI], 
                                cif as [CIF], 
                                cifRupiah as [CIF RUPIAH],
                                fob as [FOB],
                                freight as [FREIGHT],
                                hargaSatuan as [HARGA SATUAN],
                                jumlahSatuan as [JUMLAH SATUAN],
                                kodeNegaraAsal as [KODE NEGARA ASAL],
                                kodeSatuanBarang as [KODE SATUAN BARANG],
                                jumlahKemasan as [JUMLAH KEMASAN],
                                kodeJenisKemasan as [KODE KEMASAN],
                                case
                                    when kodeBarang = '' then '-'
                                    else kodeBarang
                                end as [KODE BARANG],
                                ndpbm as [NDPBM],
                                (select top 1 kodeValuta from dhl_cs_tpb_header
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGLHAWB}') as [KODE VALUTA],
                                netto as [NETTO],
                                kodeKategoriBarang as [KATEGORI BARANG],
                                posTarif as [POS TARIF],
                                seriBarang as [SERI BARANG],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGLHAWB}'
                                and kodeJenisPungutan = 'BM' and seriBarang = barang.seriBarang) as [BM TARIF],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGLHAWB}'
                                and kodeJenisPungutan = 'BMTP' and seriBarang = barang.seriBarang) as [BMTP TARIF],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGLHAWB}'
                                and kodeJenisPungutan = 'CUKAI' and seriBarang = barang.seriBarang) as [CUKAI TARIF],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGLHAWB}'
                                and kodeJenisPungutan = 'PPN' and seriBarang = barang.seriBarang) as [PPN TARIF],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGLHAWB}'
                                and kodeJenisPungutan = 'PPNBM' and seriBarang = barang.seriBarang) as [PPNBM TARIF],
                                (select tarif from dhl_cs_tpb_barang_tarif
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGLHAWB}'
                                and kodeJenisPungutan = 'PPH' and seriBarang = barang.seriBarang) as [PPH TARIF],
                                uraian as [URAIAN],
                                merk as [MEREK],
                                tipe as [TIPE],
                                case 
                                    when ukuran = '' then '-' 
                                    else ukuran
                                end as [UKURAN],
                                case 
                                    when spesifikasiLain = '' then '-'
                                    else spesifikasiLain
                                end as [SPESIFIKASI LAINNYA]
                                from dhl_cs_tpb_barang barang
                                where HAWB = '{HAWB}' and TGL_HAWB = '{TGLHAWB}'
                                order by seriBarang asc";

                //query = $@"select top 10 * from dhl_cs_tpb_barang";
                DataTable dt = DbOfficialCeisa.getRecords(query);

                ViewState["ListEditBarang"] = dt;
                GV_Barang.DataSource = dt;
                GV_Barang.DataBind();

                //GridView1.DataSource = dt;
                //GridView1.DataBind();
                lblHAWB.InnerText = HAWB;
            }
            catch (Exception ex)
            {
                logger.Log($"err LoadData: {ex.Message}, {ex.StackTrace}");
            }
        }
        private void BindData()
        {
            //string query = $@"select
            //                    HAWB, convert(char(10),TGL_HAWB,126) as TGL_HAWB,
            //                    asuransi as [ASURANSI], 
            //                    cif as [CIF], 
            //                    cifRupiah as [CIF RUPIAH],
            //                    fob as [FOB],
            //                    freight as [FREIGHT],
            //                    hargaSatuan as [HARGA SATUAN],
            //                    jumlahSatuan as [JUMLAH SATUAN],
            //                    kodeNegaraAsal as [KODE NEGARA ASAL],
            //                    kodeSatuanBarang as [KODE SATUAN BARANG],
            //                    jumlahKemasan as [JUMLAH KEMASAN],
            //                    kodeJenisKemasan as [KODE KEMASAN],
            //                    case
            //                        when kodeBarang = '' then '-'
            //                        else kodeBarang
            //                    end as [KODE BARANG],
            //                    ndpbm as [NDPBM],
            //                    netto as [NETTO],
            //                    kodeKategoriBarang as [KATEGORI BARANG],
            //                    posTarif as [POS TARIF],
            //                    seriBarang as [SERI BARANG],
            //                    (select tarif from dhl_cs_tpb_barang_tarif
            //                    where HAWB = '{ViewState["editHAWB"]}' and TGL_HAWB = '{ViewState["editTGLHAWB"]}'
            //                    and kodeJenisPungutan = 'BM' and seriBarang = barang.seriBarang) as [BM TARIF],
            //                    (select tarif from dhl_cs_tpb_barang_tarif
            //                    where HAWB = '{ViewState["editHAWB"]}' and TGL_HAWB = '{ViewState["editTGLHAWB"]}'
            //                    and kodeJenisPungutan = 'PPN' and seriBarang = barang.seriBarang) as [PPN TARIF],
            //                    (select tarif from dhl_cs_tpb_barang_tarif
            //                    where HAWB = '{ViewState["editHAWB"]}' and TGL_HAWB = '{ViewState["editTGLHAWB"]}'
            //                    and kodeJenisPungutan = 'PPH' and seriBarang = barang.seriBarang) as [PPH TARIF],
            //                    uraian as [URAIAN],
            //                    merk as [MEREK],
            //                    tipe as [TIPE],
            //                    case 
            //                        when ukuran = '' then '-' 
            //                        else ukuran
            //                    end as [UKURAN],
            //                    case 
            //                        when spesifikasiLain = '' then '-'
            //                        else spesifikasiLain
            //                    end as [SPESIFIKASI LAINNYA]
            //                    from dhl_cs_tpb_barang barang
            //                    where HAWB = '{ViewState["editHAWB"]}' and TGL_HAWB = '{ViewState["editTGLHAWB"]}'
            //                    order by seriBarang asc";
            //DataTable dt = DbOfficialCeisa.getRecords(query);

            DataTable dt = (DataTable)ViewState["ListEditBarang"];
            GV_Barang.DataSource = dt;
            GV_Barang.DataBind();
        }
        protected void GV_Barang_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_Barang.PageIndex = e.NewPageIndex;
            DataTable dt = (DataTable)ViewState["ListEditBarang"];
            GV_Barang.DataSource = dt;
            GV_Barang.DataBind();   
        }
        protected void cmdUpdate_Click(object sender, EventArgs e)
        {

        }
        protected void cmdCancel_Click(object sender, EventArgs e)
        {

        }
        protected void cmdCopy_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lb = (sender as LinkButton);
                GridViewRow clickedRow = (lb.NamingContainer as GridViewRow);
                string ID = GV_Barang.DataKeys[clickedRow.RowIndex].Values[0].ToString();

                //logger.Log($"click cmdCopy_Click: {ID}");

                DataTable dt = (DataTable)ViewState["ListEditBarang"];
                string find = $"[ID] = '{ID}'";

                DataRow[] foundRows = dt.Select(find);
                if (foundRows.Length > 0)
                {
                    DataRow newRow = null;
                    newRow = dt.NewRow();
                    dt.Rows.Add(newRow);

                    foreach (DataRow row in foundRows)
                    {
                        newRow[0] = GetTempID(ID);
                        newRow[1] = row["HAWB"];
                        newRow[2] = row["TGL_HAWB"];
                        newRow[3] = row["ASURANSI"];
                        newRow[4] = row["CIF"];
                        newRow[5] = row["CIF RUPIAH"];
                        newRow[6] = row["FOB"];
                        newRow[7] = row["FREIGHT"];
                        newRow[8] = row["HARGA SATUAN"];
                        newRow[9] = row["JUMLAH SATUAN"];
                        newRow[10] = row["KODE NEGARA ASAL"];
                        newRow[11] = row["KODE SATUAN BARANG"];
                        newRow[12] = row["JUMLAH KEMASAN"];
                        newRow[13] = row["KODE KEMASAN"];
                        newRow[14] = row["KODE BARANG"];
                        newRow[15] = row["NDPBM"];
                        newRow[16] = row["KODE VALUTA"];
                        newRow[17] = row["NETTO"];
                        newRow[18] = row["KATEGORI BARANG"];
                        newRow[19] = row["POS TARIF"];
                        newRow[20] = row["SERI BARANG"];
                        newRow[21] = row["BM TARIF"];
                        newRow[22] = row["BMTP TARIF"];
                        newRow[23] = row["CUKAI TARIF"];
                        newRow[24] = row["PPN TARIF"];
                        newRow[25] = row["PPNBM TARIF"];
                        newRow[26] = row["PPH TARIF"];
                        newRow[27] = row["URAIAN"];
                        newRow[28] = row["MEREK"];
                        newRow[29] = row["TIPE"];
                        newRow[30] = row["UKURAN"];
                        newRow[31] = row["SPESIFIKASI LAINNYA"];
                    }

                    ViewState["ListEditBarang"] = dt;
                }
                GV_Barang.EditIndex = -1;
                BindData();
            }
            catch (Exception ex)
            {
                logger.Log($"err cmdCopy: {ex.Message}, {ex.StackTrace}");
            }
        }
        protected void btnSubmitData_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> listQuery = new List<string>();
                DataTable tbl = (DataTable)ViewState["ListEditBarang"];
                if (tbl.Rows.Count > 0)
                {
                    double asuransi = 0;
                    double fob = 0;
                    double freight = 0;
                    double ndpbm_final = 0;
                    double cif_final = 0;
                    string HAWB = string.Empty;
                    string TGL_HAWB = string.Empty;

                    #region calculate total fob & freight
                    double total_fob = 0;
                    double total_fre = 0;
                    bool is_freight_header = false;
                    bool is_single_item = false;

                    foreach (DataRow row in tbl.Rows)
                    {
                        double FOB = Convert.ToDouble(row["FOB"]);
                        double FREIGHT = Convert.ToDouble(row["FREIGHT"]);
                        if (tbl.Rows.Count == 1)
                        {
                            total_fob += Math.Round(FOB, 2);
                            total_fre += Math.Round(FREIGHT, 2);
                            is_single_item = true;
                        }
                        else
                        {
                            if (is_freight_header == false)
                            {
                                foreach (DataRow row_freight in tbl.Rows)
                                {
                                    double FREIGHT_HEADER = Convert.ToDouble(row_freight["FREIGHT"]);
                                    total_fre += Math.Round(FREIGHT_HEADER, 2);
                                }
                                is_freight_header = true;
                            }

                            total_fob += Math.Round(FOB, 2);
                        }
                    }
                    #endregion

                    foreach (DataRow row in tbl.Rows)
                    {
                        if (is_single_item)
                        {
                            double BM_Items = 0.0;
                            double BMTP_Items = 0.0;
                            double PPN_Items = 0.0;
                            double PPH_Items = 0.0;
                            double CIF_Items = 0.0;
                            double CIF = 0.0;

                            double ASURANSI = (total_fob + total_fre) * 0.005;
                            //double ASURANSI = Math.Round(Convert.ToDouble(row["ASURANSI"]), 2);
                            double NDPBM = Math.Round(Convert.ToDouble(row["NDPBM"]), 2);
                            double FOB = total_fob;
                            //double FOB = Math.Round(Convert.ToDouble(row["FOB"]), 2);
                            double FREIGHT = total_fre;
                            //double FREIGHT = Math.Round(Convert.ToDouble(row["FREIGHT"]), 2);
                            double BM_Tarif = Math.Round(Convert.ToDouble(row["BM TARIF"]), 2);
                            double BMTP_Tarif = Math.Round(Convert.ToDouble(row["BMTP TARIF"]), 2);
                            double PPN_Tarif = Math.Round(Convert.ToDouble(row["PPN TARIF"]), 2);
                            double PPH_Tarif = Math.Round(Convert.ToDouble(row["PPH TARIF"]), 2);

                            CIF = Math.Round((double)ASURANSI, 2) + Math.Round((double)FOB, 2) + Math.Round((double)FREIGHT, 2);
                            CIF_Items = Math.Round((double)ASURANSI * NDPBM, 2) + Math.Round((double)FOB * NDPBM, 2) + Math.Round((double)FREIGHT * NDPBM, 2);
                            BM_Items = ((double)CIF_Items * BM_Tarif / 100);
                            PPN_Items = Math.Round((double)(CIF_Items + BM_Items) * PPN_Tarif / 100, 2);
                            PPH_Items = Math.Round((double)(CIF_Items + BM_Items) * PPH_Tarif / 100, 2);
                            BMTP_Items = Convert.ToDouble(row["JUMLAH SATUAN"]) * BMTP_Tarif;

                            listQuery.Add($@"insert into dhl_cs_tpb_barang values (
                                                '{row["HAWB"]}',
                                                '{row["TGL_HAWB"]}',
                                                '1',
                                                '{ASURANSI}',
                                                '{CIF}',
                                                '{CIF_Items}',
                                                '0',
                                                '{FOB}',
                                                '{FREIGHT}',
                                                '0','0',
                                                '{row["HARGA SATUAN"]}',
                                                '0',
                                                '{row["JUMLAH KEMASAN"]}',
                                                '{row["JUMLAH SATUAN"]}',
                                                '{row["KODE BARANG"]}',
                                                '23',
                                                '{row["KATEGORI BARANG"]}',
                                                '{row["KODE KEMASAN"]}',
                                                '{row["KODE NEGARA ASAL"]}',
                                                '0',
                                                '{row["KODE SATUAN BARANG"]}',
                                                '{row["MEREK"]}',
                                                '{row["NDPBM"]}',
                                                '{row["NETTO"]}',
                                                '{FOB}',
                                                '0',
                                                '{row["POS TARIF"]}',
                                                '{row["SERI BARANG"]}',
                                                '{row["SPESIFIKASI LAINNYA"]}',
                                                '{row["TIPE"]}',
                                                '{row["UKURAN"]}',
                                                '{row["URAIAN"]}',
                                                '0','0'
                                                )");

                            for (int i = 0; i < 6; i++)
                            {
                                switch (i)
                                {
                                    case 1: //pph
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','3',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'PPH',
                                                            '{PPH_Items}','0','{PPH_Items}',
                                                            '{row["SERI BARANG"]}',
                                                            '{row["PPH TARIF"]}',
                                                            '100'
                                                            )");
                                        break;
                                    case 2: //ppn
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','3',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'PPN',
                                                            '{PPN_Items}','0','{PPN_Items}',
                                                            '{row["SERI BARANG"]}',
                                                            '{row["PPN TARIF"]}',
                                                            '100'
                                                            )");
                                        break;
                                    case 3: //ppnbm
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','3',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'PPNBM',
                                                            '{PPN_Items}','0','{PPN_Items}',
                                                            '{row["SERI BARANG"]}',
                                                            '0',
                                                            '0'
                                                            )");
                                        break;
                                    case 4: //bmtp
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','3',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'BMTP',
                                                            '{BMTP_Items}','0','{BMTP_Items}',
                                                            '{row["SERI BARANG"]}',
                                                            '{row["BMTP TARIF"]}',
                                                            '100'
                                                            )");
                                        break;
                                    case 5: //cukai
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','3',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'CUKAI',
                                                            '{PPN_Items}','0','{PPN_Items}',
                                                            '{row["SERI BARANG"]}',
                                                            '0',
                                                            '0'
                                                            )");
                                        break;
                                    default: //bm
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','3',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'BM',
                                                            '{BM_Items}','0','{BM_Items}',
                                                            '{row["SERI BARANG"]}',
                                                            '{row["BM TARIF"]}',
                                                            '100'
                                                            )");
                                        break;
                                }
                            }

                            HAWB = row["HAWB"].ToString();
                            TGL_HAWB = row["TGL_HAWB"].ToString();

                            //asuransi += Math.Round(ASURANSI, 2);
                            //asuransi ditotal apa adanya, baru roundup 2024-05-15
                            asuransi += ASURANSI;
                            fob += Math.Round(FOB, 2);
                            freight += Math.Round(FREIGHT, 2);
                            ndpbm_final = NDPBM; //UPDATE KURS HEADER
                        }
                        else
                        {
                            double BM_Items = 0.0;
                            double BMTP_Items = 0.0;
                            double PPN_Items = 0.0;
                            double PPH_Items = 0.0;
                            double CIF_Items = 0.0;
                            double CIF = 0.0;

                            double NDPBM = Math.Round(Convert.ToDouble(row["NDPBM"]), 2);
                            double FOB = Math.Round(Convert.ToDouble(row["FOB"]), 2);
                            double PERSENFOB = FOB / total_fob;
                            double FREIGHT = total_fre * PERSENFOB;
                            double ASURANSI = 0;
                            string valFreight = $"{FREIGHT}";
                            bool hasExponential = (valFreight.Contains("E") || valFreight.Contains("e")) && double.TryParse(valFreight, out FREIGHT);
                            if (hasExponential)
                            {
                                decimal convertedFreight = decimal.Parse(valFreight, NumberStyles.Float);
                                decimal v = (Convert.ToDecimal(FOB) + convertedFreight);
                                ASURANSI = Math.Round(Convert.ToDouble(v) * 0.005, 3);
                                valFreight = $"{convertedFreight}";
                            }
                            else
                            {
                                ASURANSI = Math.Round((FOB + FREIGHT) * 0.005, 3);
                            }
                            //double FREIGHT = Math.Round(Convert.ToDouble(row["FREIGHT"]), 2);
                            //double ASURANSI = Math.Round((FOB + FREIGHT) * 0.005, 3);
                            //double ASURANSI = Math.Round(Convert.ToDouble(row["ASURANSI"]), 2);
                            double BM_Tarif = Math.Round(Convert.ToDouble(row["BM TARIF"]), 2);
                            double BMTP_Tarif = Math.Round(Convert.ToDouble(row["BMTP TARIF"]), 2);
                            double PPN_Tarif = Math.Round(Convert.ToDouble(row["PPN TARIF"]), 2);
                            double PPH_Tarif = Math.Round(Convert.ToDouble(row["PPH TARIF"]), 2);

                            CIF = Math.Round((double)ASURANSI, 2) + Math.Round((double)FOB, 2) + Math.Round((double)FREIGHT, 2);
                            CIF_Items = Math.Round((double)ASURANSI * NDPBM, 2) + Math.Round((double)FOB * NDPBM, 2) + Math.Round((double)FREIGHT * NDPBM, 2);
                            BM_Items = ((double)CIF_Items * BM_Tarif / 100);
                            PPN_Items = Math.Round((double)(CIF_Items + BM_Items) * PPN_Tarif / 100, 2);
                            PPH_Items = Math.Round((double)(CIF_Items + BM_Items) * PPH_Tarif / 100, 2);
                            BMTP_Items = Convert.ToDouble(row["JUMLAH SATUAN"]) * BMTP_Tarif;

                            listQuery.Add($@"insert into dhl_cs_tpb_barang values (
                                                '{row["HAWB"]}',
                                                '{row["TGL_HAWB"]}',
                                                '1',
                                                '{ASURANSI}',
                                                '{CIF}',
                                                '{CIF_Items}',
                                                '0',
                                                '{FOB}',
                                                '{(hasExponential == false ? FREIGHT.ToString() : valFreight)}',
                                                '0','0',
                                                '{row["HARGA SATUAN"]}',
                                                '0',
                                                '{row["JUMLAH KEMASAN"]}',
                                                '{row["JUMLAH SATUAN"]}',
                                                '{row["KODE BARANG"]}',
                                                '23',
                                                '{row["KATEGORI BARANG"]}',
                                                '{row["KODE KEMASAN"]}',
                                                '{row["KODE NEGARA ASAL"]}',
                                                '0',
                                                '{row["KODE SATUAN BARANG"]}',
                                                '{row["MEREK"]}',
                                                '{row["NDPBM"]}',
                                                '{row["NETTO"]}',
                                                '{FOB}',
                                                '0',
                                                '{row["POS TARIF"]}',
                                                '{row["SERI BARANG"]}',
                                                '{row["SPESIFIKASI LAINNYA"]}',
                                                '{row["TIPE"]}',
                                                '{row["UKURAN"]}',
                                                '{row["URAIAN"]}',
                                                '0','0'
                                                )");

                            for (int i = 0; i < 6; i++)
                            {
                                switch (i)
                                {
                                    case 1: //pph
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','6',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'PPH',
                                                            '{PPH_Items}','0','{PPH_Items}',
                                                            '{row["SERI BARANG"]}',
                                                            '{row["PPH TARIF"]}',
                                                            '100'
                                                            )");
                                        break;
                                    case 2: //ppn
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','6',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'PPN',
                                                            '{PPN_Items}','0','{PPN_Items}',
                                                            '{row["SERI BARANG"]}',
                                                            '{row["PPN TARIF"]}',
                                                            '100'
                                                            )");
                                        break;
                                    case 3: //ppnbm
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','3',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'PPNBM',
                                                            '0','0','0',
                                                            '{row["SERI BARANG"]}',
                                                            '0',
                                                            '0'
                                                            )");
                                        break;
                                    case 4: //bmtp
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','3',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'BMTP',
                                                            '{BMTP_Items}','0','{BMTP_Items}',
                                                            '{row["SERI BARANG"]}',
                                                            '{row["BMTP TARIF"]}',
                                                            '100'
                                                            )");
                                        break;
                                    case 5: //cukai
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','3',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'CUKAI',
                                                            '0','0','0',
                                                            '{row["SERI BARANG"]}',
                                                            '0',
                                                            '0'
                                                            )");
                                        break;
                                    default: //bm
                                        listQuery.Add($@"insert into dhl_cs_tpb_barang_tarif values (
                                                            '{row["HAWB"]}',
                                                            '{row["TGL_HAWB"]}',
                                                            '1','{row["JUMLAH SATUAN"]}','3',
                                                            '{row["KODE SATUAN BARANG"]}',
                                                            'BM',
                                                            '{BM_Items}','0','{BM_Items}',
                                                            '{row["SERI BARANG"]}',
                                                            '{row["BM TARIF"]}',
                                                            '100'
                                                            )");
                                        break;
                                }
                            }

                            HAWB = row["HAWB"].ToString();
                            TGL_HAWB = row["TGL_HAWB"].ToString();

                            //asuransi += Math.Round(ASURANSI, 2);
                            //asuransi ditotal apa adanya, baru roundup 2024-05-15
                            asuransi += ASURANSI;
                            fob += Math.Round(FOB, 2);
                            freight += Math.Round(FREIGHT, 2);
                            ndpbm_final = NDPBM; //UPDATE KURS HEADER
                        }
                    }

                    string query = $@"delete from dhl_cs_tpb_barang where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                    DbOfficialCeisa.runCommand(query);

                    query = $@"delete from dhl_cs_tpb_barang_tarif where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                    DbOfficialCeisa.runCommand(query);

                    //UPDATE CIF & ASURANSI
                    asuransi = Math.Round(asuransi, 2);
                    cif_final = Math.Round((asuransi + fob + freight), 2);

                    query = $@"update dhl_cs_tpb_header set asuransi = '{asuransi}', fob = '{fob}', freight = '{freight}', cif = '{cif_final}', ndpbm = '{ndpbm_final}'
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'";
                    DbOfficialCeisa.runCommand(query);

                    if (listQuery.Count > 0)
                    {
                        if (DbOfficialCeisa.runCommand(listQuery) == 0)
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Update Data Success.');", true);
                        else
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "script", $"alert('Update Data Failed, silahkan cek kembali data yang akan diupdate!');", true);
                    }

                    query = $@"select id as [ID],
                            HAWB, convert(char(10),TGL_HAWB,126) as TGL_HAWB,
                            asuransi as [ASURANSI], 
                            cif as [CIF], 
                            cifRupiah as [CIF RUPIAH],
                            fob as [FOB],
                            freight as [FREIGHT],
                            hargaSatuan as [HARGA SATUAN],
                            jumlahSatuan as [JUMLAH SATUAN],
                            kodeNegaraAsal as [KODE NEGARA ASAL],
                            kodeSatuanBarang as [KODE SATUAN BARANG],
                            jumlahKemasan as [JUMLAH KEMASAN],
                            kodeJenisKemasan as [KODE KEMASAN],
                            case
                                when kodeBarang = '' then '-'
                                else kodeBarang
                            end as [KODE BARANG],
                            ndpbm as [NDPBM],
                            (select top 1 kodeValuta from dhl_cs_tpb_header
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}') as [KODE VALUTA],
                            netto as [NETTO],
                            kodeKategoriBarang as [KATEGORI BARANG],
                            posTarif as [POS TARIF],
                            seriBarang as [SERI BARANG],
                            (select tarif from dhl_cs_tpb_barang_tarif
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                            and kodeJenisPungutan = 'BM' and seriBarang = barang.seriBarang) as [BM TARIF],
                            (select tarif from dhl_cs_tpb_barang_tarif
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                            and kodeJenisPungutan = 'BMTP' and seriBarang = barang.seriBarang) as [BMTP TARIF],
                            (select tarif from dhl_cs_tpb_barang_tarif
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                            and kodeJenisPungutan = 'CUKAI' and seriBarang = barang.seriBarang) as [CUKAI TARIF],
                            (select tarif from dhl_cs_tpb_barang_tarif
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                            and kodeJenisPungutan = 'PPN' and seriBarang = barang.seriBarang) as [PPN TARIF],
                            (select tarif from dhl_cs_tpb_barang_tarif
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                            and kodeJenisPungutan = 'PPNBM' and seriBarang = barang.seriBarang) as [PPNBM TARIF],
                            (select tarif from dhl_cs_tpb_barang_tarif
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                            and kodeJenisPungutan = 'PPH' and seriBarang = barang.seriBarang) as [PPH TARIF],
                            uraian as [URAIAN],
                            merk as [MEREK],
                            tipe as [TIPE],
                            case 
                                when ukuran = '' then '-' 
                                else ukuran
                            end as [UKURAN],
                            case 
                                when spesifikasiLain = '' then '-'
                                else spesifikasiLain
                            end as [SPESIFIKASI LAINNYA]
                            from dhl_cs_tpb_barang barang
                            where HAWB = '{HAWB}' and TGL_HAWB = '{TGL_HAWB}'
                            order by seriBarang asc";
                    DataTable tblBarang = DbOfficialCeisa.getRecords(query);
                    ViewState["ListEditBarang"] = tblBarang;
                    GV_Barang.DataSource = tblBarang;
                    GV_Barang.DataBind();
                }
            }
            catch (Exception ex)
            {
                logger.Log($"{ex.Message}, {ex.StackTrace}");
            }
        }
        protected void GV_Barang_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //Set the edit index.
            GV_Barang.EditIndex = e.NewEditIndex;

            //Bind data to the GridView control.
            BindData();

            //Set the TextBox as ReadOnly.
            (GV_Barang.Rows[e.NewEditIndex].Cells[2].Controls[0] as TextBox).Enabled = false;
            (GV_Barang.Rows[e.NewEditIndex].Cells[3].Controls[0] as TextBox).Enabled = false;
        }
        protected void GV_Barang_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GV_Barang.EditIndex = -1;
            BindData();
        }
        protected void GV_Barang_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //Get the new Values.
            GridViewRow clickedRow = GV_Barang.Rows[e.RowIndex];

            //LinkButton lb = (sender as LinkButton);
            //GridViewRow clickedRow = (lb.NamingContainer as GridViewRow);
            string ID = GV_Barang.DataKeys[clickedRow.RowIndex].Values[0].ToString();

            // Code to update the DataSource.
            DataTable dt = (DataTable)ViewState["ListEditBarang"];
            string find = $"[ID] = '{ID}'";

            DataRow[] foundRows = dt.Select(find);
            if (foundRows.Length > 0)
            {
                foreach (DataRow row in foundRows)
                {
                    row["ASURANSI"] = ((TextBox)clickedRow.Cells[4].Controls[0]).Text;
                    row["CIF"] = ((TextBox)clickedRow.Cells[5].Controls[0]).Text;
                    row["CIF RUPIAH"] = ((TextBox)clickedRow.Cells[6].Controls[0]).Text;
                    row["FOB"] = ((TextBox)clickedRow.Cells[7].Controls[0]).Text;
                    row["FREIGHT"] = ((TextBox)clickedRow.Cells[8].Controls[0]).Text;
                    row["HARGA SATUAN"] = ((TextBox)clickedRow.Cells[9].Controls[0]).Text;
                    row["JUMLAH SATUAN"] = ((TextBox)clickedRow.Cells[10].Controls[0]).Text;
                    row["KODE NEGARA ASAL"] = ((TextBox)clickedRow.Cells[11].Controls[0]).Text;
                    row["KODE SATUAN BARANG"] = ((TextBox)clickedRow.Cells[12].Controls[0]).Text;
                    row["JUMLAH KEMASAN"] = ((TextBox)clickedRow.Cells[13].Controls[0]).Text;
                    row["KODE KEMASAN"] = ((TextBox)clickedRow.Cells[14].Controls[0]).Text;
                    row["KODE BARANG"] = ((TextBox)clickedRow.Cells[15].Controls[0]).Text;
                    row["NDPBM"] = ((TextBox)clickedRow.Cells[16].Controls[0]).Text;
                    row["KODE VALUTA"] = ((TextBox)clickedRow.Cells[17].Controls[0]).Text;
                    row["NETTO"] = ((TextBox)clickedRow.Cells[18].Controls[0]).Text;
                    row["KATEGORI BARANG"] = ((TextBox)clickedRow.Cells[19].Controls[0]).Text;
                    row["POS TARIF"] = ((TextBox)clickedRow.Cells[20].Controls[0]).Text;
                    row["SERI BARANG"] = ((TextBox)clickedRow.Cells[21].Controls[0]).Text;
                    row["BM TARIF"] = ((TextBox)clickedRow.Cells[22].Controls[0]).Text;
                    row["BMTP TARIF"] = ((TextBox)clickedRow.Cells[23].Controls[0]).Text;
                    row["CUKAI TARIF"] = ((TextBox)clickedRow.Cells[24].Controls[0]).Text;
                    row["PPN TARIF"] = ((TextBox)clickedRow.Cells[25].Controls[0]).Text;
                    row["PPNBM TARIF"] = ((TextBox)clickedRow.Cells[26].Controls[0]).Text;
                    row["PPH TARIF"] = ((TextBox)clickedRow.Cells[27].Controls[0]).Text;
                    row["URAIAN"] = ((TextBox)clickedRow.Cells[28].Controls[0]).Text;
                    row["MEREK"] = ((TextBox)clickedRow.Cells[29].Controls[0]).Text;
                    row["TIPE"] = ((TextBox)clickedRow.Cells[30].Controls[0]).Text;
                    row["UKURAN"] = ((TextBox)clickedRow.Cells[31].Controls[0]).Text;
                    row["SPESIFIKASI LAINNYA"] = ((TextBox)clickedRow.Cells[32].Controls[0]).Text;
                }

                ViewState["ListEditBarang"] = dt;
            }

            //Reset the edit index.
            GV_Barang.EditIndex = -1;

            //Bind data to the GridView control.
            BindData();
        }
        private int GetTempID(string ID)
        {
            int retval = 0;

            retval = Convert.ToInt32(ID) + 99;

            return retval;
        }

        
    }
}