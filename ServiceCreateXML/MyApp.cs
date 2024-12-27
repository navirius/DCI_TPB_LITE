using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Octopus.Library.Utils;

namespace ServiceCreateXML
{
    internal class MyApp
    {
        public MyApp()
        {
            Config cfg = new Config(AppDomain.CurrentDomain.BaseDirectory + @"\CONFIG.xml");
            DbSql._SERVER = cfg.GetValue("REMOTE_SERVER", "mykulwspc000159,1525");
            DbSql._DATABASE = cfg.GetValue("REMOTE_DATABASE", "db_ceisa_lite");
            DbSql._USER = cfg.GetValue("REMOTE_USERNAME", "dhlid");
            DbSql._PSWD = cfg.GetValue("REMOTE_PASSWORD", "awCr1*f9E+1c2DujHLO8");
            Parameter.UPLOADPATH = cfg.GetValue("UPLOAD_PATH", @"\\MYKULWSPC000030\ShareFolderDeployment\WCO\");
            Parameter.BUILDPATH = cfg.GetValue("BUILD_PATH", AppDomain.CurrentDomain.BaseDirectory + @"\WCO\build\");
            DbDCI.LoadParameter();
        }

        public void Start()
        {
            try
            {
                string query = $@"select * from dhl_cs_tpb_status_xml where ISCREATEXML = '0'";
                DataTable dt = DbSql.getRecords(query);

                Utils.Logger("Check Flagging XML for DCE..");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        query = $@"select top 1
                                NO_AJU, 
                                SUBSTRING(NO_AJU, 13, 4) + '-' + SUBSTRING(NO_AJU, 17, 2) + '-' + SUBSTRING(NO_AJU, 19, 2) as TANGGAL_AJU, 
                                nomorDaftar, 
                                tanggalDaftar,
                                kodeKantorBongkar as KANTOR,
                                T1.HAWB,
                                T1.TGL_HAWB,
                                keterangan,
                                kodeRespon,
                                nomorRespon,
                                tanggalRespon
                                from dhl_cs_response_data T1 left join
                                dhl_cs_tpb_header T2 on T1.NO_AJU = T2.nomorAju
                                where NO_AJU = '{row["NO_AJU"]}' and
                                kodeRespon = '2303'";
                        DataRow rowXML = DbSql.getRow(query);

                        if (rowXML != null)
                        {
                            query = $@"UPDATE BILLINGREPORT SET 
                                    NOSPPB = '{rowXML["nomorDaftar"]}',
                                    TGLSPPB = '{Convert.ToDateTime(rowXML["tanggalDaftar"]).ToString("yyyy-MM-dd")}',
                                    NOAJU = '{rowXML["NO_AJU"]}',
                                    TGLAJU = '{rowXML["TANGGAL_AJU"]}',
                                    NOBC23 = '{rowXML["nomorDaftar"]}',
                                    TGLBC23 = '{Convert.ToDateTime(rowXML["tanggalDaftar"]).ToString("yyyy-MM-dd")}'
                                    WHERE HAWB = '{rowXML["HAWB"]}' AND TGLHAWB = '{Convert.ToDateTime(rowXML["TGL_HAWB"]).ToString("yyyy-MM-dd")}'";
                            DbDCI.runCommand(query);
                            Utils.Logger(query); //update billingreport

                            int isSppb = IsSppbExist(rowXML["HAWB"].ToString(), Convert.ToDateTime(rowXML["TGL_HAWB"]).ToString("yyyy-MM-dd"));

                            switch (isSppb)
                            {
                                case 1:
                                    query = $@"UPDATE SPPB SET 
                                            NOSPPB = '{rowXML["nomorRespon"]}',
                                            TGLSPPB = '{Convert.ToDateTime(rowXML["tanggalDaftar"]).ToString("yyyy-MM-dd")}',
                                            NOAJU = '{rowXML["NO_AJU"]}',
                                            NODAFTAR = '{rowXML["nomorDaftar"]}',
                                            TGLDAFTAR = '{Convert.ToDateTime(rowXML["tanggalDaftar"]).ToString("yyyy-MM-dd")}',
                                            create_at = '{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm")}',
                                            create_by = 'DCI Lite'
                                            WHERE HAWB = '{rowXML["HAWB"]}' AND TGLHAWB = '{Convert.ToDateTime(rowXML["TGL_HAWB"]).ToString("yyyy-MM-dd")}'";
                                    DbDCI.runCommand(query);
                                    Utils.Logger(query); //update sppb
                                    break;
                                case 2:
                                    query = $@"INSERT INTO [SPPB]
                                                   ([HAWB]
                                                   ,[TGLHAWB]
                                                   ,[NOSPPB]
                                                   ,[TGLSPPB]
                                                   ,[NODAFTAR]
                                                   ,[TGLDAFTAR]
                                                   ,[TYPEDOC]
                                                   ,[NAMADOC]
                                                   ,[NOMORAJU]
                                                   ,[create_at]
                                                   ,[create_by])
                                             VALUES
                                                   ('{rowXML["HAWB"]}'
                                                   ,'{Convert.ToDateTime(rowXML["TGL_HAWB"]).ToString("yyyy-MM-dd")}'
                                                   ,'{rowXML["nomorRespon"]}'
                                                   ,'{Convert.ToDateTime(rowXML["tanggalDaftar"]).ToString("yyyy-MM-dd")}'
                                                   ,'{rowXML["nomorDaftar"]}'
                                                   ,'{Convert.ToDateTime(rowXML["tanggalDaftar"]).ToString("yyyy-MM-dd")}'
                                                   ,'2'
                                                   ,'BC 2.3'
                                                   ,'{rowXML["NO_AJU"]}'
                                                   ,'{DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm")}'
                                                   ,'DCI Lite')";
                                    DbDCI.runCommand(query);
                                    Utils.Logger(query); //insert sppb
                                    break;
                                default://0 - skip karena data sudah ada dan sppb terisi
                                    break;
                            }

                            string MAWB = GetMawb(rowXML["HAWB"].ToString(), Convert.ToDateTime(rowXML["TGL_HAWB"]).ToString("yyyy-MM-dd"));
                            string KETERANGAN = $"NOMOR AJU : {rowXML["NO_AJU"]}, NOMOR DAFTAR : {rowXML["nomorDaftar"]}, TANGGAL DAFTAR : {Convert.ToDateTime(rowXML["tanggalDaftar"]).ToString("yyyy-MM-dd")}";
                            CreateXMLForDCERelease(rowXML["HAWB"].ToString(), Convert.ToDateTime(rowXML["TGL_HAWB"]).ToString("yyyy-MM-dd"), rowXML["kodeRespon"].ToString(), KETERANGAN, rowXML["KANTOR"].ToString(), MAWB);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Logger(ex.Message);
            }
        }

        public void CreateXMLForDCERelease(string HAWB, string TGLHAWB, string KODERESPON, string KETERANGAN, string KODEKANTOR, string MAWB)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            var locXML = Parameter.BUILDPATH;
            if (!Directory.Exists(locXML))
                Directory.CreateDirectory(locXML);

            var locMoveXML = Parameter.UPLOADPATH;
            if (!Directory.Exists(locMoveXML))
                Directory.CreateDirectory(locMoveXML);

            var NAMAXML = locXML + @"TPB_" + HAWB + "_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") + ".XML";

            try
            {
                using (XmlWriter writer = XmlWriter.Create(NAMAXML, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("CstmsInterface"); //AWAL.
                    writer.WriteStartElement("Hdr"); //HDR.
                    writer.WriteStartElement("MsgCtl"); // MsgCtl.
                    writer.WriteElementString("SchemaName", "DCE Cstms Msg");
                    writer.WriteElementString("SchemaVersion", "2.3.0");
                    writer.WriteElementString("MsgRevision", "");
                    writer.WriteElementString("MsgDate", DateTime.Now.ToString("yyyyMMdd"));
                    writer.WriteElementString("MsgTime", DateTime.Now.ToString("HH:mm:ss").Replace(".", ":"));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CstmsMsgDtls"); //CstmsMsgDtls.
                    writer.WriteElementString("MsgEntryNo", $"{GetGateway(KODEKANTOR)}{HAWB}");
                    writer.WriteElementString("CstmsTgtCtry", "ID");
                    writer.WriteElementString("CstmsTgtMsgType", "CUSRES_WCO");
                    writer.WriteElementString("DCEMsgVersion", "");
                    writer.WriteElementString("Module", "I");
                    writer.WriteElementString("Gateway", "OKOK");
                    writer.WriteEndElement(); //CstmsMsgDtls.

                    writer.WriteStartElement("EDIMsgHDrDtls"); // EDIMsgHDrDtls.
                    writer.WriteStartElement("UNB"); // UNB.
                    writer.WriteStartElement("SyntaxId"); // SyntaxId.
                    writer.WriteElementString("SyntaxId", "");
                    writer.WriteElementString("SyntaxVersionNo", "");
                    writer.WriteEndElement(); // SyntaxId.
                    writer.WriteStartElement("InterchangeSender");// InterchangeSender.
                    writer.WriteElementString("SenderID", "");
                    writer.WriteEndElement(); // InterchangeSender.
                    writer.WriteStartElement("InterchangeRecipient"); //InterchangeRecipient.
                    writer.WriteElementString("RecipientID", "");
                    writer.WriteEndElement(); //InterchangeRecipient.
                    writer.WriteStartElement("DateTimeOfPrep");// DateTimeOfPrep.
                    writer.WriteElementString("DateOfPrep", "");
                    writer.WriteElementString("TimeOfPrep", "");
                    writer.WriteEndElement(); // DateTimeOfPrep.
                    writer.WriteElementString("InterchangeControlRef", "");
                    writer.WriteElementString("ApplicationReference", "TPB");
                    writer.WriteEndElement(); // UNB.
                    writer.WriteStartElement("UNH"); //UNH.
                    writer.WriteElementString("MsgRefNo", "");
                    writer.WriteStartElement("MsgId"); // MsgId.
                    writer.WriteElementString("MsgType", "CUSRES_WCO");
                    writer.WriteElementString("MsgVersionNo", "");
                    writer.WriteElementString("MsgReleaseNo", "");
                    writer.WriteElementString("ControllingAgency", "");
                    writer.WriteEndElement(); //MsgId
                    writer.WriteEndElement(); //UNH
                    writer.WriteEndElement(); //EDIMsgHDrDtls
                    writer.WriteEndElement(); //HDR
                    writer.WriteStartElement("Dtls"); // Dtls.
                    writer.WriteStartElement("Entries"); //Entries.
                    writer.WriteStartElement("Entry"); //Entry.

                    writer.WriteElementString("CstmsDeclarationNo", "");
                    writer.WriteElementString("CstmsSubmissionDate", "");

                    writer.WriteStartElement("Mvmts");// Mvmts.
                    writer.WriteStartElement("Mvmt"); // Mvmt.
                    writer.WriteStartElement("TDOCs");// TDOCs.
                    writer.WriteStartElement("TDOC"); // TDOC.
                    writer.WriteElementString("TDOCNo", MAWB);
                    writer.WriteElementString("TransportMode", "");
                    writer.WriteStartElement("Shps"); // Shps.
                    writer.WriteStartElement("Shp"); // Shp.
                    writer.WriteElementString("HAWB", HAWB);

                    writer.WriteStartElement("LineItems");// LineItems.
                    writer.WriteStartElement("LineItem"); // LineItem.
                    writer.WriteElementString("GoodsItemNo", "1");
                    writer.WriteElementString("TariffCdNo", "");
                    writer.WriteEndElement(); //LineItem
                    writer.WriteEndElement(); //LineItems

                    writer.WriteStartElement("ShpCnsgne"); // ShpCnsgne.

                    writer.WriteStartElement("CnsgneCoDtls"); //CnsgneCoDtls.
                    writer.WriteElementString("CoName", "");
                    writer.WriteEndElement(); //CnsgneCoDtls

                    writer.WriteEndElement(); //ShpCnsgne
                    writer.WriteElementString("CustomResponseCd", "RLS4");

                    writer.WriteElementString("CstmsResponseDate", "");

                    writer.WriteElementString("NarrativeText", $"{KETERANGAN}");

                    writer.WriteEndElement();//Shp
                    writer.WriteEndElement();//Shps
                    writer.WriteEndElement();//TDOC
                    writer.WriteEndElement();//TDOCs
                    writer.WriteEndElement();//Mvmt
                    writer.WriteEndElement();//Mvmts
                    writer.WriteEndElement();//Entry
                    writer.WriteEndElement();//Entries
                    writer.WriteEndElement();//Dtls
                    writer.WriteEndElement();//AWAL
                }

                File.Move(NAMAXML, locMoveXML + @"TPB_" + HAWB + "_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") + ".XML");
                Utils.Logger($"Move File {locMoveXML}" + @"TPB_" + HAWB + "_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") + ".XML");

                string time_created = DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss.fff");
                string query = $"UPDATE dhl_cs_tpb_status_xml SET ISCREATEXML = '1', time_created = '{time_created}' where HAWB = '{HAWB}'";
                if (DbSql.runCommand(query) == 0)
                    Utils.Logger($"UPDATE STATUS XML FOR HAWB {HAWB} COMPLETED..");

            }
            catch (Exception ex)
            {
                Utils.Logger($"Error Create XML for DCE release : {ex.Message}");
            }
        }

        public string GetGateway(string KODEKANTOR)
        {
            string retval = "JKT";
            try
            {
                string query = $@"SELECT GTWCODE FROM KBPCBONGKAR WHERE KPBC_BONGKAR = '{KODEKANTOR}' ";
                DataRow row = DbDCI.getRow(query);
                if (row != null) retval = row["GTWCODE"].ToString();
            }
            catch (Exception ex)
            {
                Utils.Logger(ex.Message);
            }
            return retval;
        }

        public string GetMawb(string hawb, string tgl_hawb)
        {
            string retval = "";
            try
            {
                string query = $@"select top 1 mawb from billingreport where hawb ='{hawb}' and tglhawb ='{tgl_hawb}'";
                DataRow row = DbDCI.getRow(query);
                if (row != null) retval = row["mawb"].ToString();
            }
            catch (Exception ex)
            {
                Utils.Logger(ex.Message);
            }
            return retval;
        }

        public int IsSppbExist(string hawb, string tgl_hawb)
        {
            int retval = 0; //default datanya ada - skip
            try
            {
                string query = $"select ISNULL(NOSPPB,'') as [NOSPPB] from SPPB where HAWB = '{hawb}' and TGLHAWB = '{tgl_hawb}'";
                DataRow row = DbDCI.getRow(query);
                if (row != null)
                {
                    string nosppb = row["NOSPPB"].ToString();
                    if (string.IsNullOrEmpty(nosppb))
                    {
                        retval = 1; //datanya ada - harus update
                    }
                }
                else
                {
                    retval = 2; //kosong - harus insert
                }
            }
            catch (Exception ex)
            {
                Utils.Logger($"{ex.Message}, {ex.StackTrace}");
            }
            return retval;
        }
    }
}
