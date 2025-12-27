'Public Class frmArabadanUretimeTeslim

'    Private Sub txtDocNo_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)

'    End Sub
'    Private Sub txtItem_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)

'    End Sub
'    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

'    End Sub
'    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

'    End Sub
'    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

'    End Sub
'    Private Sub txtQty_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)

'    End Sub
'    Private Sub txtCikisYeri_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)

'    End Sub
'    Private Sub btnX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

'    End Sub
'End Class


Imports System.Globalization.RegionInfo
Imports System.Data
Imports System.IO

Public Class frmArabadanUretimeTeslim

    Dim srv As New Mobile.wsGeneral
    'Dim db As New Core.Data(ConnectionStrSQL) 'sql connection
    Dim sStr As String
    Dim dt As DataTable



    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Try

            If txtItem.Text = "" Or txtQty.Text = "" Or txtCikisYeri.Text = "" Or txtGirisYeri.Text = "" Or txtLot.Text = "" Or txtEldekiMiktar.Text = "" Or txtSicil.Text = "" Then
                MessageBox.Show("Malzeme, Miktar, Çıkış Yeri, Giriş Yeri, Lot, Eldeki Miktar, Sicil Alanlarından Biri Yada Birkaçı Boş.")
                Exit Sub
            End If

            sStr = "SELECT uf_yer_tipi FROM location WHERE loc='" & txtCikisYeri.Text & "'"
            'dt = db.RunSql(Str)
            dt = srv.RunSqlDs(sStr, "location", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows(0)(0) = "GKK" Then
                MessageBox.Show("Malzeme Lot Girişi Kalite Lokasyonunda!", "Uyarı")
                Exit Sub
            ElseIf dt.Rows(0)(0) <> "URT" Then
                MessageBox.Show("Çıkış yeri tipi URT olmalı.")
                Exit Sub
            End If

            sStr = "SELECT uf_yer_tipi FROM location WHERE loc='" & txtGirisYeri.Text & "'"
            'dt = db.RunSql(Str)
            dt = srv.RunSqlDs(sStr, "location", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows(0)(0) <> "URT" AndAlso dt.Rows(0)(0) <> "SEVK" Then
                MessageBox.Show("Giriş yeri tipi URT olmalı yada SEVK olmalı!", "Uyarı")
                Exit Sub
            End If

            sStr = "select top 1 qty_on_hand from lot_loc where item = '" & txtItem.Text & "' AND loc = '" & _
                 txtCikisYeri.Text & "'" & " AND lot = '" & txtLot.Text & "' and whse='" & txtAmbar.Text & "'"
            'dt = db.RunSql(Str)
            dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count = 0 Then
                MessageBox.Show("Kaynak yerde malzeme yok !")
                btnClear_Click(Me, e)
                Exit Sub
            End If

            If dt.Rows(0).Item("qty_on_hand") < txtQty.Text Then
                MessageBox.Show("Kaynak yerdeki miktar yetersiz !")
                Exit Sub
            End If

            If txtGirisYeri.Text = txtCikisYeri.Text Then
                MessageBox.Show("Giriş ve çıkış yeri aynı olamaz !", "Uyarı")
                Exit Sub
            End If

            Try

                If (CInt(txtQty.Text) > CInt(txtEldekiMiktar.Text)) Then
                    MessageBox.Show("Eldeki miktar: " & txtEldekiMiktar.Text & vbNewLine & "Bu miktardan fazlası ilerletilemez!")
                    Exit Sub
                End If

                sStr = "SELECT * FROM item WHERE lot_tracked <> 1 AND item ='" & (txtItem.Text) & "'"
                dt = srv.RunSqlDs(sStr, "item", Mobile.ProviderTypes.SqlClient).Tables(0)
                'If (db.RunSql(Str).Rows.Count > 0) Then
                If (dt.Rows.Count > 0) Then
                    MessageBox.Show("Malzeme lot denetimsizdir.", "Uyarı")
                    Exit Sub
                End If

            Catch ex As Exception
                MessageBox.Show("Hata oluştu")
                Exit Sub
            End Try

            'db.BeginTransaction()

            sStr = "EXEC TR_Stok_Kontrol '" & txtItem.Text & "', '" & _
            txtLot.Text & "', '" & UCase(txtGirisYeri.Text) & "', '" & ReadConfig("DefaultWhse") & "'"
            'db.RunSql(Str)
            srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            If IsDBNull(txtDocNo.Text) = True Then
                txtDocNo.Text = "0"
            End If

            sStr = "EXEC dbo.TR_Miktar_Ilerlet_MI @Item = '" & txtItem.Text & "', @loc1 = '" & txtCikisYeri.Text + "', @lot1 = '" & _
                txtLot.Text & "', @loc2 = '" & UCase(txtGirisYeri.Text) & "', @qty ='" & txtQty.Text & "', @whse='" & txtAmbar.Text & "'" & _
                ", @DocumentNum='" & txtDocNo.Text & "', @UserName='" & frmKullaniciGirisi.kullanici & "', @Sicil='" & txtSicil.Text & "'"
            'db.RunSql(Str)
            srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            sStr = "SELECT * FROM lot_loc WHERE whse = '" & ReadConfig("DefaultWhse") & "' AND item = '" & txtItem.Text & _
                "' AND lot='" & txtLot.Text & "' AND loc = '" & UCase(txtGirisYeri.Text) & "' AND qty_on_hand >= " & txtQty.Text
            dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

            If (dt.Rows.Count = 0) Then
                MessageBox.Show("İşlem başarısız, tekrar deneyiniz")
            End If

            'If db.Transaction() Then
            '    db.CommitTransaction()
            'End If

            'btnClear_Click(Me, e)
            txtItem.Text = ""
            txtLot.Text = ""
            txtQty.Text = ""
            txtEldekiMiktar.Text = ""
            txtSonYer.Text = ""
            txtItem.Focus()

        Catch ex As Exception

            'If db.Transaction() Then
            '    db.RollbackTransaction()
            'End If

            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Hide()
        frmMenu.Visible = True
    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click

        txtItem.Text = ""
        txtLot.Text = ""
        txtQty.Text = ""
        txtCikisYeri.Text = ""
        txtGirisYeri.Text = ""
        txtEldekiMiktar.Text = ""
        txtSonYer.Text = ""
        txtItem.Focus()
        txtSicil.Text = ""

    End Sub

    Private Sub frmYeniMiktarIlerlet_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        srv.Url = ReadConfig("path")
        txtAmbar.Text = ReadConfig("DefaultWhse")
        txtDocNo.Focus()
    End Sub

    Private Sub txtDocNo_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtDocNo.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            txtItem.Focus()
        End If
    End Sub

    Private Sub txtItem_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtItem.KeyPress
        Dim Malzeme As String = String.Empty
        Dim lot As String = String.Empty
        Dim Miktar As String = String.Empty

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            Cursor.Current = Cursors.WaitCursor

            Try

                If txtItem.Text.Trim.Length > 0 Then

                    If txtItem.Text.IndexOf("%") <> -1 Then

                        Malzeme = txtItem.Text.Split("%")(0)
                        lot = txtItem.Text.Split("%")(1)

                        If txtItem.Text.Split("%").Length > "2" Then
                            Miktar = txtItem.Text.Split("%")(2)
                        Else
                            Miktar = 0
                        End If

                        txtItem.Text = Malzeme
                        txtLot.Text = lot
                        txtQty.Text = Miktar
                        txtGirisYeri.Text = ""

                        sStr = "SELECT TOP 1 matltran.loc FROM matltran " & _
                           "LEFT JOIN location ON matltran.loc = location.loc AND mrb_flag=0 " & _
                           "WHERE trans_type+ref_type = 'MI' AND qty>0 AND uf_yer_tipi IN ('RAF','ARAF') AND " & _
                           "item = '" & txtItem.Text & "' AND whse='" & txtAmbar.Text & "'  " & _
                           "ORDER BY trans_date DESC"
                        'dt = db.RunSql(Str)
                        dt = srv.RunSqlDs(sStr, "matltran", Mobile.ProviderTypes.SqlClient).Tables(0)

                        If dt.Rows.Count <> 0 Then
                            txtSonYer.Text = dt.Rows(0).Item(0)
                        Else
                            txtSonYer.Text = ""
                        End If

                        txtQty.Focus()

                    End If

                End If


            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
                Cursor.Current = Cursors.Default
            End Try

        End If

    End Sub

    Private Sub txtCikisYeri_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtCikisYeri.KeyPress

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            Cursor.Current = Cursors.WaitCursor

            Try

                If txtItem.Text <> "" Then
                    sStr = "select top 1 qty_on_hand from lot_loc where item = '" & txtItem.Text & "' AND loc = '" & txtCikisYeri.Text & "'" & _
                   " AND lot = '" & txtLot.Text & "' and whse='" & txtAmbar.Text & "'"
                    'dt = db.RunSql(Str)
                    dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt.Rows.Count = 0 Then
                        MessageBox.Show("Kaynak yerde malzeme yok !")
                        Exit Sub
                    Else
                        txtEldekiMiktar.Text = dt.Rows(0).Item(0)
                    End If

                End If

                txtGirisYeri.Focus()

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
                Cursor.Current = Cursors.Default
            End Try
        End If

    End Sub

    Private Sub txtQty_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtQty.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            txtCikisYeri.Focus()
        End If
    End Sub

    Private Sub btnX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnX.Click
        Me.Hide()
        frmMenu.Visible = True
    End Sub

End Class