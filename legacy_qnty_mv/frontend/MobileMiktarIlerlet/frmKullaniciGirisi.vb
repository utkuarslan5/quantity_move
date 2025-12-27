Imports System.Data

Public Class frmKullaniciGirisi

    Dim dt As New DataTable
    Dim sStr As String = String.Empty
    Dim srv As New Mobile.wsGeneral
    Public kullanici As String
    Public ambar As String
    Public varsayilanGiris As String
    Dim cursor As Windows.Forms.Cursor
    Dim DefaultSite As String = String.Empty

    Private Sub btnGiris_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGiris.Click
        Try

            If System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator = "," OrElse _
               System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = "," Then
                'MessageType = enumMessageType.GirisKontrol
                Throw New Exception("Lütfen cihazın bölgesel ayarlarını (nokta , virgül) düzeltiniz !")
                'ManuelMessageShow("Lütfen el terminalinin bölgesel ayarlarını [nokta , virgül] düzeltiniz !", MessageType)
                'btnKpt_Click(sender, e)
            End If

            If txtKullanici.Text.Trim = "" Then
                MessageBox.Show("Kullanıcı alanını boş bıraktınız!")
                Exit Sub
            End If

            If txtSifre.Text.Trim = "" Then
                MessageBox.Show("Şifre alanını boş bıraktınız!")
                Exit Sub
            End If

            cursor = Cursors.WaitCursor

            sStr = "SELECT * FROM TRM_EDIUSER WHERE Kullanici = '" & txtKullanici.Text.Trim & "' AND Sifre ='" & txtSifre.Text & "'"
            dt = srv.RunSqlDs(sStr, "TRM_EDIUSER", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count = 0 Then
                MessageBox.Show("Girmiş olduğunuz kullanıcı adı ve şifre bilgisi eşleşmiyor!")
                txtKullanici.Text = ""
                txtSifre.Text = ""
                Exit Sub
            End If

            kullanici = dt.Rows(0)("Kullanici").ToString.Trim
            ambar = dt.Rows(0)("Ambar").ToString.Trim

            sStr = "select * from employee_mst where LTRIM(emp_num) = '" & kullanici.Trim & "'" & _
                 " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"
            dt = srv.RunSqlDs(sStr, "employee_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count = 0 Then
                MessageBox.Show("Kullanıcının sicili tanımsız. Sicil tanımladıktan sonra tekrar deneyiniz.")
                Exit Sub
            End If

            'varsayilanGiris = dt.Rows(0)("VarsayilanGiris")
            txtKullanici.Text = ""
            txtSifre.Text = ""

            Me.Visible = False
            frmMenu.Show()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            cursor = Cursors.Default
        End Try
    End Sub

    Private Sub frmKullaniciGirisi_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        srv.Url = ReadConfig("path")
        DefaultSite = ReadConfig("DefaultSite")
    End Sub

    Private Sub txtKullanici_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtKullanici.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            txtSifre.Focus()
        End If
    End Sub

    Private Sub txtSifre_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtSifre.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            btnGiris_Click(Me, e)
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Application.Exit()
    End Sub
End Class