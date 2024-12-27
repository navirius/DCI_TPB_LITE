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
      ,[ALAMAT2]
      ,[EMAIL]
      ,[TGLPOA]
      ,[TGLSTATEMENT]
      ,[IMGONSHARE]
      ,[TGLEXPIRED]
      ,[UM]
      ,[UE]
      ,[DM]
      ,[DE],INTRUKSICUSOMER) VALUES ('" & ReplacePetik(txtNama.Text) & "','" & ReplacePetik(txtNpwp.Text) & "','" & ReplacePetik(txtNoApi.Text) & "','" & ReplacePetik(txtNIB.Text) & "','" & ReplacePetik(txtCustome.Text) & "','" & ReplacePetik(txtAccount.Text) & "','" & ReplacePetik(txtAddress1.Text) & "','" & ReplacePetik(txtAddress2.Text) & "','" & ReplacePetik(txtEmail.Text) & "','" & ReplacePetik(tgl1) & "','" & ReplacePetik(tgl2) & "','" & ReplacePetik(txtImage.Text) & "','" & ReplacePetik(tgl3) & "','" & Session("USERNYA") & "','" & Session("USERNYA") & "','" & Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(".", ":") & "','" & Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(".", ":") & "','" & ReplacePetik(txtCustomerIntruksi.Text) & "')")
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
            ds = MyDataset("SELECT top 50 * FROM CUSTOMER WHERE NAMACUSTOMER LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NPWP LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOAPI LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NONIB LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOCUSTOM LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOACCOUNT LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR ALAMAT1 LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR ALAMAT2 LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%'  OR EMAIL LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR IMGONSHARE LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' ORDER BY ID * 1 DESC")
            GridView1.DataSource = ds.Tables(0)
            GridView1.DataBind()
            ds.Dispose()
        Else

            Dim ds As New DataSet
            ds = MyDataset("SELECT  * FROM CUSTOMER WHERE NAMACUSTOMER LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NPWP LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOAPI LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NONIB LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOCUSTOM LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR NOACCOUNT LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR ALAMAT1 LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR ALAMAT2 LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%'  OR EMAIL LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' OR IMGONSHARE LIKE '%" & ReplacePetik(Trim(txtCari.Text)) & "%' ORDER BY ID * 1 DESC")
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
      ,[ALAMAT2] = '" & ReplacePetik(TryCast(row.Cells(8).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[EMAIL] = '" & ReplacePetik(TryCast(row.Cells(9).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[TGLPOA] = '" & ReplacePetik(TryCast(row.Cells(10).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[TGLSTATEMENT] = '" & ReplacePetik(TryCast(row.Cells(11).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[IMGONSHARE] = '" & ReplacePetik(TryCast(row.Cells(12).Controls(0), TextBox).Text.Replace(",", ".")) & "'
      ,[TGLEXPIRED] = '" & ReplacePetik(TryCast(row.Cells(13).Controls(0), TextBox).Text.Replace(",", ".")) & "'
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
