Imports Modul
Imports System.Data
Partial Class MasterGateway
    Inherits System.Web.UI.Page

    Protected Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
        If txtNama.Text <> "" Then
            Dim tgl1 As String = ""
            Dim tgl2 As String = ""
            Dim tgl3 As String = ""
            Try
                Dim array1() As String = txtPOA.Text.Split("/")
                tgl1 = array1(2) & "-" & array1(1) & "-" & array1(0)
                Dim array2() As String = txtStatement.Text.Split("/")
                tgl2 = array2(2) & "-" & array2(1) & "-" & array2(0)
                Dim array3() As String = txtExpired.Text.Split("/")
                tgl3 = array3(2) & "-" & array3(1) & "-" & array3(0)
            Catch ex As Exception

            End Try

            ExecuteQuery("INSERT INTO CUSTOMER ([NAMACUSTOMER]
            ,[NPWP]
            ,[NOAPI]
            ,[NONIB]
            ,[NOCUSTOM]
            ,[NOACCOUNT]
            ,[ALAMAT1]
            ,[EMAIL]
            ,[TGLPOA]
            ,[TGLSTATEMENT]
            ,[TGLEXPIRED]
            ,[UM]
            ,[UE]
            ,[DM]
            ,[DE]
            ,INTRUKSICUSOMER
            ,city
            ,zipcode
            ,mobile_phone
            ,contact_name
            ,phone_no
            ,skkb
            ,skb_pph
            ,kemudahan_imp_tujuan_exp
            ,pp_8
            ,pp_19
            ,other
            ,url_importer_id
            ,url_nik
            ,url_nib
            ,url_api
            ,url_skkb
            ,url_poa_date
            ,url_skb_pph
            ,url_kemudahan_imp_tujuan_exp
            ,url_pp_8
            ,url_pp_19
            ,url_other) VALUES ('" & ReplacePetik(txtNama.Text) & "','" & ReplacePetik(txtNpwp.Text) & "','" & ReplacePetik(txtNoApi.Text) & "','" & ReplacePetik(txtNIB.Text) & "','" & ReplacePetik(txtCustome.Text) & "','" & ReplacePetik(txtAccount.Text) & "','" & ReplacePetik(txtAddress1.Text) & "','" & ReplacePetik(txtEmail.Text) & "','" & ReplacePetik(tgl1) & "','" & ReplacePetik(tgl2) & "','" & ReplacePetik(tgl3) & "','" & Session("USERNYA") & "','" & Session("USERNYA") & "','" & Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(".", ":") & "','" & Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(".", ":") & "','" & ReplacePetik(txtCustomerIntruksi.Text) & "','" & ReplacePetik(txtCity.Text) & "','" & ReplacePetik(txtZipcode.Text) & "','" & ReplacePetik(txtMobilePhone.Text) & "','" & ReplacePetik(txtContactName.Text) & "','" & ReplacePetik(txtPhoneNo.Text) & "','" & ReplacePetik(txtSkkb.Text) & "','" & ReplacePetik(txtSkbPph.Text) & "','" & ReplacePetik(txtKemImpTujExp.Text) & "','" & ReplacePetik(txtPP8.Text) & "','" & ReplacePetik(txtPP19.Text) & "','" & ReplacePetik(txtOther.Text) & "','" & ReplacePetik(txtUrlImporterId.Text) & "','" & ReplacePetik(txtUrlNik.Text) & "','" & ReplacePetik(txtUrlNib.Text) & "','" & ReplacePetik(txtUrlApi.Text) & "','" & ReplacePetik(txtUrlSkkb.Text) & "','" & ReplacePetik(txtUrlPoaDate.Text) & "','" & ReplacePetik(txtUrlSkbPph.Text) & "','" & ReplacePetik(txtUrlKemImpTujExp.Text) & "','" & ReplacePetik(txtUrlPP8.Text) & "','" & ReplacePetik(txtUrlPP19.Text) & "','" & ReplacePetik(txtUrlOther.Text) & "')")
            loaddata(False)
        Else
            loaddata(True)
        End If
    End Sub
    Private Sub GridView1_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        Me.loaddata(False)
    End Sub
    Protected Sub cmdCari_Click(sender As Object, e As EventArgs)
        loaddata(False)
    End Sub
    Sub loaddata(awal As Boolean)
        If awal = True Then
            Dim ds As New DataSet
            ds = MyDataset("Select top 50 * FROM CUSTOMER WHERE NAMACUSTOMER Like '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NPWP LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOAPI LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NONIB LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOCUSTOM LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOACCOUNT LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR ALAMAT1 LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%'  OR EMAIL LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' ORDER BY ID * 1 DESC")
            GridView1.DataSource = ds.Tables(0)
            GridView1.DataBind()
            ds.Dispose()
        Else

            Dim ds As New DataSet
            ds = MyDataset("SELECT  * FROM CUSTOMER WHERE NAMACUSTOMER LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NPWP LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOAPI LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NONIB LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOCUSTOM LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOACCOUNT LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR ALAMAT1 LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%'  OR EMAIL LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' ORDER BY ID * 1 DESC")
            GridView1.DataSource = ds.Tables(0)
            GridView1.DataBind()
            ds.Dispose()
        End If

    End Sub
    Protected Sub cmdClear_Click(sender As Object, e As EventArgs) Handles cmdClear.Click
        Response.Redirect("MasterCustomer.aspx")
    End Sub
    Protected Sub OnCancel(sender As Object, e As EventArgs)
        GridView1.EditIndex = -1
        Me.loaddata(False)
    End Sub
    Protected Sub cmdEdit_Click(sender As Object, e As EventArgs)
        Dim row As GridViewRow = TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow)
        Response.Redirect("MasterCustomerEdit.aspx?id=" & row.Cells(0).Text)
    End Sub
    Private Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        loaddata(False)
    End Sub
    Protected Sub OnUpdate(sender As Object, e As EventArgs)
        Dim row As GridViewRow = TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow)

        ''''  [NAMACUSTOMER]
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

        ExecuteQuery("UPDATE KBPCBONGKAR SET [NAMACUSTOMER] = '" & ReplacePetik(TryCast(row.Cells(1).Controls(0), TextBox).Text) & "'
      ,[NPWP] = '" & ReplacePetik(TryCast(row.Cells(2).Controls(0), TextBox).Text) & "'
      ,[NOAPI] = '" & ReplacePetik(TryCast(row.Cells(3).Controls(0), TextBox).Text) & "'
      ,[NONIB] = '" & ReplacePetik(TryCast(row.Cells(4).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[NOCUSTOM] = '" & ReplacePetik(TryCast(row.Cells(5).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[NOACCOUNT] = '" & ReplacePetik(TryCast(row.Cells(6).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[ALAMAT1] = '" & ReplacePetik(TryCast(row.Cells(7).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[EMAIL] = '" & ReplacePetik(TryCast(row.Cells(8).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[TGLPOA] = '" & ReplacePetik(TryCast(row.Cells(9).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[TGLSTATEMENT] = '" & ReplacePetik(TryCast(row.Cells(10).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[TGLEXPIRED] = '" & ReplacePetik(TryCast(row.Cells(11).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,mobile_phone = '" & ReplacePetik(TryCast(row.Cells(12).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,contact_name = '" & ReplacePetik(TryCast(row.Cells(13).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,phone_no = '" & ReplacePetik(TryCast(row.Cells(14).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,skkb = '" & ReplacePetik(TryCast(row.Cells(15).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,skb_pph = '" & ReplacePetik(TryCast(row.Cells(16).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,kemudahan_imp_tujuan_exp = '" & ReplacePetik(TryCast(row.Cells(17).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,pp_8 = '" & ReplacePetik(TryCast(row.Cells(18).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,pp_19 = '" & ReplacePetik(TryCast(row.Cells(19).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,other = '" & ReplacePetik(TryCast(row.Cells(20).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_importer_id = '" & ReplacePetik(TryCast(row.Cells(21).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_nik = '" & ReplacePetik(TryCast(row.Cells(22).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_nib = '" & ReplacePetik(TryCast(row.Cells(23).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_api = '" & ReplacePetik(TryCast(row.Cells(24).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_skkb = '" & ReplacePetik(TryCast(row.Cells(25).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_poa_date = '" & ReplacePetik(TryCast(row.Cells(26).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_skb_pph = '" & ReplacePetik(TryCast(row.Cells(27).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_kemudahan_imp_tujuan_exp = '" & ReplacePetik(TryCast(row.Cells(28).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_pp_8 = '" & ReplacePetik(TryCast(row.Cells(29).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_pp_19 = '" & ReplacePetik(TryCast(row.Cells(30).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,url_other = '" & ReplacePetik(TryCast(row.Cells(31).Controls(0), TextBox).Text.Replace(",", ".")) & "'
       WHERE [ID] ='" & row.Cells(0).Text & "'")


        GridView1.EditIndex = -1
        Me.loaddata(False)
    End Sub
    Protected Sub cmdDel_Click(sender As Object, e As EventArgs)
        Dim row As GridViewRow = TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow)
        Try
            ExecuteQuery("delete from customer where id ='" & row.Cells(0).Text & "'")
            loaddata(False)
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('Delete Data Sukses')", True)
        Catch ex As Exception
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('Delete Data Gagal')", True)

        End Try

    End Sub
    Private Sub MasterGateway_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack = False Then
            If Session("LOGIN") = "OK" Then
            Else
                Session("LOGIN") = "NOOK"
                Response.Redirect("Default.aspx")
            End If
            SetHideMenuNavigation()
            loaddata(False)
        End If
        Session("STATUSMENU") = "MENUMASTERCUSTOMER"
        For Each de As DictionaryEntry In HttpContext.Current.Cache
            HttpContext.Current.Cache.Remove(DirectCast(de.Key, String))
        Next
    End Sub
    Public Sub SetHideMenuNavigation()
        Dim ArrayList() As String = ChekPermisiMenuOrFungsi(1, Session("LEVELIDNYA")).Split("#")

    End Sub

    Private Sub txtCari_TextChanged(sender As Object, e As EventArgs) Handles txtCari.TextChanged
        loaddata(False)
    End Sub
End Class
