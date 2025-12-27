Imports System.Data
Public Class frmAmbarlarArasiTransfer

    Dim dt As New DataTable
    Dim sStr As String = String.Empty
    Dim srv As New Mobile.wsGeneral
    Dim dt1 As New DataTable
    Dim dt2 As New DataTable


    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click

        txtItem.Text = ""
        txtQty.Text = ""
        txtLot.Text = ""
        txtItem.Focus()
        txtYer2.Text = ""
        txtBasvuru.Text = ""
        cmbGirisWhse.SelectedValue = ""
        cmbCikisWhse.SelectedValue = ""
        cmbBoxCikisYeri.SelectedValue = ""

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Close()
        frmMenu.Visible = True
    End Sub

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Try

            If cmbCikisWhse.Text = "" Or cmbGirisWhse.Text = "" Then
                MessageBox.Show("Çıkış/Giriş Ambarlarını tanımlayınız.")
                Exit Sub
            End If

            If txtBasvuru.Text = "" Then
                MessageBox.Show("Başvuru bilgisini tanımlayınız.")
                Exit Sub
            End If

            If txtYer2.Text = "" Then
                MessageBox.Show("Giriş Yerini tanımlayınız.")
                Exit Sub
            Else
                sStr = "SELECT loc FROM location WHERE loc = '" & txtYer2.Text & "'"
                dt = srv.RunSqlDs(sStr, "loc", Mobile.ProviderTypes.SqlClient).Tables(0)

                If dt.Rows.Count = 0 Then
                    MessageBox.Show("Giriş yeri tanımsız.")
                    Exit Sub
                End If

            End If

            sStr = "SELECT * FROM itemwhse WHERE item = '" & txtItem.Text & "' AND whse = '" & cmbGirisWhse.Text & "'"
            If srv.RunSqlDs(sStr, "itemwhse", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count = 0 Then
                MessageBox.Show("Malzeme Ambar Kaydı Tanımsız.")
                Exit Sub
            End If

            Dim cmd As String

            cmd = "select qty_on_hand from lot_loc where item = '" & txtItem.Text & "' AND loc = '" & cmbBoxCikisYeri.Text & "'" & _
                " AND lot = '" & txtLot.Text & "' and whse = '" & cmbCikisWhse.Text & "'"

            dt = srv.RunSqlDs(cmd, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

            Try
                If (txtQty.Text > dt.Rows(0)(0)) Then
                    MessageBox.Show("Eldeki miktar: " & Convert.ToInt32(dt.Rows(0)(0)) & vbNewLine & "Bu miktardan fazlası gönderilemez!")
                    Exit Sub
                End If

                cmd = "SELECT * FROM item WHERE lot_tracked <> 1 AND item ='" & txtItem.Text & "'"
                If (srv.RunSqlDs(cmd, "item", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count > 0) Then
                    MessageBox.Show("Malzeme lot denetimsizdir.", "Uyarı")
                    Exit Sub
                End If

            Catch ex As Exception
                MessageBox.Show("Hata oluştu")
                Exit Sub
            End Try

            sStr = "DECLARE @ReturnMessage AS NVARCHAR(MAX) " & _
                   "EXEC TR_Stok_Kontrol '" & txtItem.Text & "', '" & _
                   txtLot.Text & "', '" & txtYer2.Text & "','" & cmbGirisWhse.Text & "',@ReturnMessage output " & _
                   " SELECT @ReturnMessage"

            dt = srv.RunSqlDs(sStr, "TR_Stok_Kontrol", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count <> 0 AndAlso dt.Rows(0)(0).ToString <> "" Then
                MessageBox.Show(dt.Rows(0)(0).ToString)
                Exit Sub
            End If

            cmd = "EXEC dbo.TR_Miktar_Ilerlet_MultiWhse @Item = '" & txtItem.Text & "', @loc1 = '" & cmbBoxCikisYeri.Text + "', @lot1 = '" & _
                txtLot.Text & "', @loc2 = '" & txtYer2.Text & "', @qty ='" & txtQty.Text & "', @whse1='" & cmbCikisWhse.Text & "'," & _
                " @whse2='" & cmbGirisWhse.Text & "', @DocumentNum='" & txtBasvuru.Text & "', @UserName='" & frmKullaniciGirisi.kullanici & "'"

            srv.RunSqlSp(cmd, Mobile.ProviderTypes.SqlClient)

            cmd = "SELECT * FROM lot_loc WHERE whse = '" & cmbGirisWhse.Text & "' AND item = '" & txtItem.Text & _
                "' AND lot='" & txtLot.Text & "' AND loc = '" & txtYer2.Text & "' AND qty_on_hand >= " & txtQty.Text

            If (srv.RunSqlDs(cmd, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count = 0) Then
                MessageBox.Show("İşlem başarısız, tekrar deneyiniz")
            End If

            'btnClear_Click(Me, e)
            txtItem.Text = ""
            txtQty.Text = ""
            txtLot.Text = ""
            txtItem.Focus()
            txtYer2.Text = ""

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try


    End Sub

    Private Sub frmAmbarlarArasiTransfer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            srv.Url = ReadConfig("path")

            sStr = "SELECT whse FROM whse"
            dt1 = srv.RunSqlDs(sStr, "whse", Mobile.ProviderTypes.SqlClient).Tables(0)
            cmbGirisWhse.DataSource = dt1

            sStr = "SELECT whse FROM whse"
            dt2 = srv.RunSqlDs(sStr, "whse", Mobile.ProviderTypes.SqlClient).Tables(0)
            cmbCikisWhse.DataSource = dt2

            cmbCikisWhse.DisplayMember = "whse"
            cmbCikisWhse.ValueMember = "whse"
            cmbGirisWhse.DisplayMember = "whse"
            cmbGirisWhse.ValueMember = "whse"
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try


    End Sub

    Private Sub cmbCikisWhse_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCikisWhse.SelectedValueChanged
        If cmbCikisWhse.Text = "MAIN" Then
            cmbCikisWhse.BackColor = Color.Blue
            cmbCikisWhse.ForeColor = Color.White
        ElseIf cmbCikisWhse.Text = "DEPO" Then
            cmbCikisWhse.BackColor = Color.White
            cmbCikisWhse.ForeColor = Color.Blue
        End If
    End Sub

    Private Sub cmbGirisWhse_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGirisWhse.SelectedIndexChanged
        If cmbGirisWhse.Text = "MAIN" Then
            cmbGirisWhse.BackColor = Color.Blue
            cmbGirisWhse.ForeColor = Color.White
        ElseIf cmbGirisWhse.Text = "DEPO" Then
            cmbGirisWhse.BackColor = Color.White
            cmbGirisWhse.ForeColor = Color.Blue
        End If
    End Sub

    Private Sub cmbBoxCikisYeri_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbBoxCikisYeri.SelectedValueChanged
        'Try
        '    Dim qty As String
        '    sStr = "SELECT qty_on_hand FROM lot_loc WHERE lot ='" & txtLot.Text & "' AND item ='" & txtItem.Text & "' AND loc = '" & cmbBoxCikisYeri.Text & "'"
        '    dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)
        '    If dt.Rows.Count = 0 Then
        '        txtQty.Text = "0"
        '    Else
        '        qty = dt.Rows(0)(0)
        '        qty = qty.Split(".")(0) & "." & qty.Split(".")(1).Substring(0, 3)
        '        txtQty.Text = qty
        '    End If
        'Catch ex As Exception
        '    MessageBox.Show(ex.Message)
        'End Try
    End Sub

    Private Sub txtItem_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtItem.KeyPress
        Dim Malzeme As String = String.Empty
        Dim lot As String = String.Empty
        Dim Miktar As String = String.Empty

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            Try
                Cursor.Current = Cursors.WaitCursor

                If txtItem.Text.Trim.Length > 0 Then

                    If txtItem.Text.IndexOf("%") <> -1 Then

                        Malzeme = txtItem.Text.Split("%")(0)
                        lot = txtItem.Text.Split("%")(1)

                        If txtItem.Text.Split("%").Length > 2 Then
                            Miktar = txtItem.Text.Split("%")(2)
                        Else
                            Miktar = 0
                        End If

                        txtItem.Text = Malzeme
                        txtLot.Text = lot
                        txtQty.Text = Miktar

                        sStr = "SELECT loc FROM lot_loc WHERE item = '" & _
                            txtItem.Text & "' " & "And lot = '" & txtLot.Text & "' And qty_on_hand > 0 AND whse='" & cmbCikisWhse.Text & "'"

                        dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)
                        cmbBoxCikisYeri.DataSource = dt

                        cmbBoxCikisYeri.DisplayMember = "loc"
                        cmbBoxCikisYeri.ValueMember = "loc"
                        cmbBoxCikisYeri.Focus()

                    End If

                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
                Cursor.Current = Cursors.Default
            End Try

        End If
    End Sub
End Class