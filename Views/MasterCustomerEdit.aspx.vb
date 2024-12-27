Imports System.Data
Imports Modul
Partial Class MasterCustomerEdit
    Inherits System.Web.UI.Page

    Protected Sub cmdBack_Click(sender As Object, e As EventArgs) Handles cmdBack.Click
        Response.Redirect("MasterCustomer.aspx")
    End Sub
    Protected Sub cmdClear_Click(sender As Object, e As EventArgs) Handles cmdClear.Click
        Response.Redirect("MasterCustomerEdit.aspx")
    End Sub

    Private Sub MasterGateway_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack = False Then
            If Session("LOGIN") = "OK" Then
            Else
                Session("LOGIN") = "NOOK"
                Response.Redirect("Default.aspx")
            End If

            LoadData()
        End If
        Session("STATUSMENU") = "MENUMASTERCUSTOMER"
        For Each de As DictionaryEntry In HttpContext.Current.Cache
            HttpContext.Current.Cache.Remove(DirectCast(de.Key, String))
        Next
    End Sub
    Sub LoadData()
        Dim ds As New DataSet
        ds = MyDataset("SELECT * FROM CUSTOMER WHERE ID ='" & Request.QueryString("id") & "'")
        For Each row As DataRow In ds.Tables(0).Rows
            ''''      ,[NAMACUSTOMER]
            '''',[NPWP]
            '''',[NOAPI]
            '''',[NONIB]
            '''',[NOCUSTOM]
            '''',[NOACCOUNT]
            '''',[ALAMAT1]
            '''',[ALAMAT2]
            '''',[EMAIL]
            '''',[TGLPOA]
            '''',[TGLSTATEMENT]
            '''',[IMGONSHARE]
            '''',[TGLEXPIRED]
            txtID.Text = row(0).ToString
            txtNama.Text = row(1).ToString
            txtNpwp.Text = row(2).ToString
            txtNoApi.Text = row(3).ToString
            txtNIB.Text = row(4).ToString
            txtCustome.Text = row(5).ToString
            txtAccount.Text = row(6).ToString
            txtAddress1.Text = row(7).ToString
            txtEmail.Text = row(9).ToString
            If row(10).ToString <> "" Then
                txtPOA.Text = CDate(row(10).ToString).ToString("dd/MM/yyyy")
            End If
            If row(11).ToString <> "" Then
                txtStatement.Text = CDate(row(11).ToString).ToString("dd/MM/yyyy")
            End If

            If row(13).ToString <> "" Then
                txtExpired.Text = CDate(row(13).ToString).ToString("dd/MM/yyyy")
            End If

            txtCustomerIntruksi.Text = row("INTRUKSICUSOMER").ToString

            txtMobilePhone.Text = row("mobile_phone").ToString
            txtContactName.Text = row("contact_name").ToString
            txtPhoneNo.Text = row("phone_no").ToString
            txtSkkb.Text = row("skkb").ToString
            txtSkbPph.Text = row("skb_pph").ToString
            txtKemImpTujExp.Text = row("kemudahan_imp_tujuan_exp").ToString
            txtPP8.Text = row("pp_8").ToString
            txtPP19.Text = row("pp_19").ToString
            txtOther.Text = row("other").ToString
            txtUrlImporterId.Text = row("url_importer_id").ToString
            txtUrlNik.Text = row("url_nik").ToString
            txtUrlNib.Text = row("url_nib").ToString
            txtUrlApi.Text = row("url_api").ToString
            txtUrlSkkb.Text = row("url_skkb").ToString
            txtUrlPoaDate.Text = row("url_poa_date").ToString
            txtUrlSkbPph.Text = row("url_skb_pph").ToString
            txtUrlKemImpTujExp.Text = row("url_kemudahan_imp_tujuan_exp").ToString
            txtUrlPP8.Text = row("url_pp_8").ToString
            txtUrlPP19.Text = row("url_pp_19").ToString
            txtUrlOther.Text = row("url_other").ToString
            txtCity.Text = row("city").ToString
            txtZipcode.Text = row("zipcode").ToString

        Next
        ds.Dispose()
    End Sub
    Protected Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
        Try
            If txtExpired.Text.IndexOf("/") > -1 And txtPOA.Text.IndexOf("/") > -1 And txtStatement.Text.IndexOf("/") > -1 Then
                Dim tgl1 As String = ""
                Dim tgl2 As String = ""
                Dim tgl3 As String = ""
                Dim array1() As String = txtPOA.Text.Split("/")
                tgl1 = array1(2) & "-" & array1(1) & "-" & array1(0)
                Dim array2() As String = txtStatement.Text.Split("/")
                tgl2 = array2(2) & "-" & array2(1) & "-" & array2(0)
                Dim array3() As String = txtExpired.Text.Split("/")
                tgl3 = array3(2) & "-" & array3(1) & "-" & array3(0)
                ExecuteQuery("UPDATE CUSTOMER SET [NAMACUSTOMER] = '" & ReplacePetik(txtNama.Text) & "'
                              ,[NPWP] = '" & ReplacePetik(txtNpwp.Text) & "'
                              ,[NOAPI] = '" & ReplacePetik(txtNoApi.Text) & "'
                              ,[NONIB] = '" & ReplacePetik(txtNIB.Text) & "'
                              ,[NOCUSTOM] = '" & ReplacePetik(txtCustome.Text) & "'
                              ,[NOACCOUNT] = '" & ReplacePetik(txtAccount.Text) & "'
                              ,[ALAMAT1] = '" & ReplacePetik(txtAddress1.Text) & "'
                              ,[EMAIL] = '" & ReplacePetik(txtEmail.Text) & "'
                              ,[TGLPOA] = '" & ReplacePetik(tgl1) & "'
                              ,[TGLSTATEMENT] = '" & ReplacePetik(tgl2) & "'
                              ,[TGLEXPIRED] = '" & ReplacePetik(tgl3) & "'
                              ,[INTRUKSICUSOMER] = '" & ReplacePetik(txtCustomerIntruksi.Text) & "'
                              ,[ue] = '" & Session("USERNYA") & "'
                              ,[de] = '" & Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(".", ":") & "'
                              ,mobile_phone = '" & ReplacePetik(txtMobilePhone.Text) & "'
                              ,city = '" & ReplacePetik(txtCity.Text) & "'
                              ,zipcode = '" & ReplacePetik(txtZipcode.Text) & "'
                              ,contact_name = '" & ReplacePetik(txtContactName.Text) & "'
                              ,phone_no = '" & ReplacePetik(txtPhoneNo.Text) & "'
                              ,skkb = '" & ReplacePetik(txtSkkb.Text) & "'
                              ,skb_pph = '" & ReplacePetik(txtSkbPph.Text) & "'
                              ,kemudahan_imp_tujuan_exp = '" & ReplacePetik(txtKemImpTujExp.Text) & "'
                              ,pp_8 = '" & ReplacePetik(txtPP8.Text) & "'
                              ,pp_19 = '" & ReplacePetik(txtPP19.Text) & "'
                              ,other = '" & ReplacePetik(txtOther.Text) & "'
                              ,url_importer_id = '" & ReplacePetik(txtUrlImporterId.Text) & "'
                              ,url_nik = '" & ReplacePetik(txtUrlNik.Text) & "'
                              ,url_nib = '" & ReplacePetik(txtUrlNib.Text) & "'
                              ,url_api = '" & ReplacePetik(txtUrlApi.Text) & "'
                              ,url_skkb = '" & ReplacePetik(txtUrlSkkb.Text) & "'
                              ,url_poa_date = '" & ReplacePetik(txtUrlPoaDate.Text) & "'
                              ,url_skb_pph = '" & ReplacePetik(txtUrlSkbPph.Text) & "'
                              ,url_kemudahan_imp_tujuan_exp = '" & ReplacePetik(txtUrlKemImpTujExp.Text) & "'
                              ,url_pp_8 = '" & ReplacePetik(txtUrlPP8.Text) & "'
                              ,url_pp_19 = '" & ReplacePetik(txtUrlPP19.Text) & "'
                              ,url_other = '" & ReplacePetik(txtUrlOther.Text) & "'
                               WHERE [ID] ='" & txtID.Text & "'")
                ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('Edit Data Sukses')", True)
                Response.Redirect("MasterCustomer.aspx")
            Else
                Dim tgl1 As String = ""
                Dim tgl2 As String = ""
                Dim tgl3 As String = ""
                If txtExpired.Text.IndexOf("/") > -1 Then
                    Dim array3() As String = txtExpired.Text.Split("/")
                    tgl3 = array3(2) & "-" & array3(1) & "-" & array3(0)
                End If
                If txtPOA.Text.IndexOf("/") > -1 Then
                    Dim array1() As String = txtPOA.Text.Split("/")
                    tgl1 = array1(2) & "-" & array1(1) & "-" & array1(0)
                End If
                If txtStatement.Text.IndexOf("/") > -1 Then
                    Dim array2() As String = txtStatement.Text.Split("/")
                    tgl2 = array2(2) & "-" & array2(1) & "-" & array2(0)
                End If


                ExecuteQuery("UPDATE CUSTOMER SET [NAMACUSTOMER] = '" & ReplacePetik(txtNama.Text) & "'
                              ,[NPWP] = '" & ReplacePetik(txtNpwp.Text) & "'
                              ,[NOAPI] = '" & ReplacePetik(txtNoApi.Text) & "'
                              ,[NONIB] = '" & ReplacePetik(txtNIB.Text) & "'
                              ,[NOCUSTOM] = '" & ReplacePetik(txtCustome.Text) & "'
                              ,[NOACCOUNT] = '" & ReplacePetik(txtAccount.Text) & "'
                              ,[ALAMAT1] = '" & ReplacePetik(txtAddress1.Text) & "'
                              ,[EMAIL] = '" & ReplacePetik(txtEmail.Text) & "'
                              ,[TGLPOA] = '" & ReplacePetik(tgl1) & "'
                              ,[TGLSTATEMENT] = '" & ReplacePetik(tgl2) & "'
                              ,[TGLEXPIRED] = '" & ReplacePetik(tgl3) & "'
                              ,[INTRUKSICUSOMER] = '" & ReplacePetik(txtCustomerIntruksi.Text) & "'
                              ,[ue] = '" & Session("USERNYA") & "'
                              ,[de] = '" & Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(".", ":") & "'
                              ,mobile_phone = '" & ReplacePetik(txtMobilePhone.Text) & "'
                              ,city = '" & ReplacePetik(txtCity.Text) & "'
                              ,zipcode = '" & ReplacePetik(txtZipcode.Text) & "'
                              ,contact_name = '" & ReplacePetik(txtContactName.Text) & "'
                              ,phone_no = '" & ReplacePetik(txtPhoneNo.Text) & "'
                              ,skkb = '" & ReplacePetik(txtSkkb.Text) & "'
                              ,skb_pph = '" & ReplacePetik(txtSkbPph.Text) & "'
                              ,kemudahan_imp_tujuan_exp = '" & ReplacePetik(txtKemImpTujExp.Text) & "'
                              ,pp_8 = '" & ReplacePetik(txtPP8.Text) & "'
                              ,pp_19 = '" & ReplacePetik(txtPP19.Text) & "'
                              ,other = '" & ReplacePetik(txtOther.Text) & "'
                              ,url_importer_id = '" & ReplacePetik(txtUrlImporterId.Text) & "'
                              ,url_nik = '" & ReplacePetik(txtUrlNik.Text) & "'
                              ,url_nib = '" & ReplacePetik(txtUrlNib.Text) & "'
                              ,url_api = '" & ReplacePetik(txtUrlApi.Text) & "'
                              ,url_skkb = '" & ReplacePetik(txtUrlSkkb.Text) & "'
                              ,url_poa_date = '" & ReplacePetik(txtUrlPoaDate.Text) & "'
                              ,url_skb_pph = '" & ReplacePetik(txtUrlSkbPph.Text) & "'
                              ,url_kemudahan_imp_tujuan_exp = '" & ReplacePetik(txtUrlKemImpTujExp.Text) & "'
                              ,url_pp_8 = '" & ReplacePetik(txtUrlPP8.Text) & "'
                              ,url_pp_19 = '" & ReplacePetik(txtUrlPP19.Text) & "'
                              ,url_other = '" & ReplacePetik(txtUrlOther.Text) & "'
                               WHERE [ID] ='" & txtID.Text & "'")
                ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('Edit Data Sukses')", True)
                Response.Redirect("MasterCustomer.aspx")
                ' 
            End If
        Catch ex As Exception
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('Edit Data Gagal Pastikan Tanggal Format dd/MM/yyyy')", True)
        End Try


    End Sub
End Class
