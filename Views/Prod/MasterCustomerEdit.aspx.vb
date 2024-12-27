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
            txtAddress2.Text = row(8).ToString
            txtEmail.Text = row(9).ToString
            If row(10).ToString <> "" Then
                txtPOA.Text = CDate(row(10).ToString).ToString("dd/MM/yyyy")
            End If
            If row(11).ToString <> "" Then
                txtStatement.Text = CDate(row(11).ToString).ToString("dd/MM/yyyy")
            End If

            txtImage.Text = row(12).ToString
            If row(13).ToString <> "" Then
                txtExpired.Text = CDate(row(13).ToString).ToString("dd/MM/yyyy")
            End If

            txtLasteUpdate.Text = row("DE").ToString
            txtCustomerIntruksi.Text = row("INTRUKSICUSOMER").ToString

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
      ,[ALAMAT2] = '" & ReplacePetik(txtAddress2.Text) & "'
      ,[EMAIL] = '" & ReplacePetik(txtEmail.Text) & "'
      ,[TGLPOA] = '" & ReplacePetik(tgl1) & "'
      ,[TGLSTATEMENT] = '" & ReplacePetik(tgl2) & "'
      ,[IMGONSHARE] = '" & ReplacePetik(txtImage.Text) & "'
      ,[TGLEXPIRED] = '" & ReplacePetik(tgl3) & "'
      ,[INTRUKSICUSOMER] = '" & ReplacePetik(txtCustomerIntruksi.Text) & "'
      ,[ue] = '" & Session("USERNYA") & "'
       ,[de] = '" & Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(".", ":") & "'
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
      ,[ALAMAT2] = '" & ReplacePetik(txtAddress2.Text) & "'
      ,[EMAIL] = '" & ReplacePetik(txtEmail.Text) & "'
      ,[TGLPOA] = '" & ReplacePetik(tgl1) & "'
      ,[TGLSTATEMENT] = '" & ReplacePetik(tgl2) & "'
      ,[IMGONSHARE] = '" & ReplacePetik(txtImage.Text) & "'
      ,[TGLEXPIRED] = '" & ReplacePetik(tgl3) & "'
      ,[INTRUKSICUSOMER] = '" & ReplacePetik(txtCustomerIntruksi.Text) & "'
      ,[ue] = '" & Session("USERNYA") & "'
       ,[de] = '" & Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(".", ":") & "'
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
