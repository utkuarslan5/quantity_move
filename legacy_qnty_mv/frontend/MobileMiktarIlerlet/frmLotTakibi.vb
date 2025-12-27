Imports System.Data

Public Class frmLotTakibi

    Dim dt As New DataTable
    Dim sStr As String = String.Empty
    Dim srv As New Mobile.wsGeneral
    Dim Lot As String = String.Empty
    Dim MalzemeKodu As String = String.Empty
    Dim Tanim As String = String.Empty
    Dim Job As String = String.Empty
    Dim flag As Boolean = False


    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Try

            If txtLot.Text = "" Then
                MessageBox.Show("Lot Bilgisini Giriniz")
                Exit Sub
            End If

            sStr = "INSERT INTO Tr_Job_Lot" & _
                       " VALUES( '" & Job & "','" & MalzemeKodu & "','" & Tanim.Replace(",", " ").Replace("'", " ") & "','" & Lot & "','" & _
                       Date.Now.ToString("yyyy-MM-dd hh:mm:ss") & "')"

            srv.RunSql(sStr, True, Mobile.ProviderTypes.SqlClient)

            txtSira.Text = ""
            txtMalzemeKodu.Text = ""
            txtTanim.Text = ""
            txtLot.Text = ""

            If txtTalimat.Text <> "" Then
                txtJob.Text = ""
                txtTalimat.Focus()
            Else
                txtJob.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
            flag = True
        End Try

        If Not flag Then
            MessageBox.Show("Kayıt işlendi.")
        End If
        flag = False
        
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Hide()
        frmMenu.Visible = True
    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        txtTalimat.Text = ""
        txtSira.Text = ""
        txtMalzemeKodu.Text = ""
        txtTanim.Text = ""
        txtLot.Text = ""
        txtJob.Text = ""
        txtTalimat.Focus()
    End Sub

    Private Sub frmLotTakibi_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        srv.Url = ReadConfig("path")

    End Sub

    Private Sub txtTalimat_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtTalimat.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            txtSira.Focus()
        End If
    End Sub

    Private Sub txtSira_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtSira.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            Cursor.Current = Cursors.WaitCursor

            Try
                If txtTalimat.Text = "" Then
                    MessageBox.Show("Talimat No Giriniz.")
                    Exit Sub
                End If

                sStr = "SELECT * FROM TR_TLMT WHERE TLMT = '" & txtTalimat.Text & "'"

                If (srv.RunSqlDs(sStr, "TR_TLMT", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count = 0) Then
                    MessageBox.Show("Geçerli bir talimat numarası giriniz.", "Uyarı")
                    Exit Sub
                End If

                If txtSira.Text = "" Then
                    MessageBox.Show("Sıra No Giriniz.")
                    Exit Sub
                End If

                sStr = "SELECT * FROM TR_TLMT WHERE TLMT = '" & txtTalimat.Text & "' AND SEQUENCE='" & txtSira.Text & "'"

                If (srv.RunSqlDs(sStr, "TR_TLMT", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count = 0) Then
                    MessageBox.Show("Geçerli bir sıra numarası giriniz.", "Uyarı")
                    Exit Sub
                End If

                sStr = "SELECT CITEM,CDESC,JOB FROM TR_TLMT WHERE TLMT='" & txtTalimat.Text & "' AND SEQUENCE='" & txtSira.Text & "'"
                dt = srv.RunSqlDs(sStr, "TR_TLMT", Mobile.ProviderTypes.SqlClient).Tables(0)

                If dt.Rows.Count > 0 Then
                    Job = dt.Rows(0)(2).ToString
                    txtMalzemeKodu.Text = dt.Rows(0)(0).ToString
                    txtTanim.Text = dt.Rows(0)(1).ToString
                    txtJob.Text = Job
                End If
                

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
                Cursor.Current = Cursors.Default
            End Try

        End If
    End Sub

    Private Sub txtJob_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtJob.KeyPress

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            Try
                If txtJob.Text = "" Then
                    MessageBox.Show("İşemri Giriniz.")
                    Exit Sub
                End If

                Dim ilkHarf As String

                If Len(txtJob.Text) < 10 Then
                    ilkHarf = txtJob.Text.Chars(0)
                    txtJob.Text = Mid(txtJob.Text, 2, Len(txtJob.Text))
                    txtJob.Text = CStr(txtJob.Text).PadLeft(9, "0")
                    txtJob.Text = ilkHarf & txtJob.Text
                End If

                Job = txtJob.Text

                sStr = "SELECT * FROM JOB WHERE JOB = '" & txtJob.Text & "'"

                If (srv.RunSqlDs(sStr, "JOB", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count = 0) Then
                    MessageBox.Show("Geçerli bir iş emri giriniz.", "Uyarı")
                    Exit Sub
                End If

                sStr = "SELECT ITEM,DESCRIPTION FROM JOB WHERE JOB='" & Job & "'"
                dt = srv.RunSqlDs(sStr, "TR_TLMT", Mobile.ProviderTypes.SqlClient).Tables(0)

                txtTalimat.Text = ""
                txtSira.Text = ""

                txtMalzemeKodu.Text = dt.Rows(0)(0).ToString
                txtTanim.Text = dt.Rows(0)(1).ToString

                txtLot.Focus()

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

        End If
    End Sub

    Private Sub txtLot_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtLot.KeyPress

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            txtLot.Text = txtLot.Text.Replace(".", "/")
            txtLot.Text = txtLot.Text.Replace("ç", ".")
            txtLot.Text = txtLot.Text.Replace("Ç", ".")
            txtLot.Text = txtLot.Text.Replace("ö", ",")
            txtLot.Text = txtLot.Text.Replace("Ö", ",")
            txtLot.Text = txtLot.Text.Replace("*", "-")

            Cursor.Current = Cursors.WaitCursor

            Try
                If txtLot.Text.Trim.Length > 0 Then

                    If txtLot.Text.IndexOf("%") <> -1 Then

                        MalzemeKodu = txtLot.Text.Split("%")(0)
                        Lot = txtLot.Text.Split("%")(1)

                        sStr = "SELECT * FROM jobmatl WHERE job='" & Job & "' AND item='" & MalzemeKodu & "'"
                        dt = srv.RunSqlDs(sStr, "jobmatl", Mobile.ProviderTypes.SqlClient).Tables(0)

                        If dt.Rows.Count = 0 Then
                            MessageBox.Show("Geçersiz bileşen.Bileşen değişikliği yapınız.", "Uyarı")
                            txtLot.Text = ""
                            Exit Sub
                        End If

                        sStr = "SELECT description FROM item WHERE item='" & MalzemeKodu & "'"
                        dt = srv.RunSqlDs(sStr, "item", Mobile.ProviderTypes.SqlClient).Tables(0)

                        If dt.Rows.Count <> 0 Then
                            Tanim = dt.Rows(0)(0)
                        End If

                    End If
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
                Cursor.Current = Cursors.Default
            End Try

        End If
    End Sub

    Private Sub btnX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnX.Click
        Me.Hide()
        frmMenu.Visible = True
    End Sub
End Class