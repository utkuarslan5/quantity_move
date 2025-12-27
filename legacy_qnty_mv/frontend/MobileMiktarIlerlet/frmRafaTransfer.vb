Imports System.Data

Public Class frmRafaTransfer

    Dim dt As New DataTable
    Dim sStr As String = String.Empty
    Dim srv As New Mobile.wsGeneral
    Dim DefaultSite As String = String.Empty

    Dim Malzeme As String = String.Empty
    Dim lot As String = String.Empty
    Dim Miktar As String = String.Empty
    Dim qty As Decimal = 0

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Try
            If txtItem.Text = "" Or txtQty.Text = "" Or txtCikisYeri.Text = "" Or _
                txtGirisYeri.Text = "" Or txtLot.Text = "" Then

                MessageBox.Show("Malzeme, Miktar, Çıkış Yeri, Giriş Yeri, Lot, Eldeki Miktar Alanlarından Biri Yada Birkaçı Boş.")
                Exit Sub

            End If

            If txtCikisYeri.Text.Trim = txtGirisYeri.Text.Trim Then
                MessageBox.Show("Giriş ve Çıkış Yeri Aynı Olamaz!")
                Exit Sub
            End If

            sStr = "select top 1 qty_on_hand from lot_loc_mst " & _
                 " where item = '" & txtItem.Text & "' AND loc = '" & txtCikisYeri.Text & "'" & _
                 " AND lot = '" & txtLot.Text & "' and whse='" & txtAmbar.Text & "'" & _
                 " and site_Ref = '" & IIf(DefaultSite = "", "Default", DefaultSite) & "'"

            dt = srv.RunSqlDs(sStr, "lot_loc_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count = 0 Then

                MessageBox.Show("Kaynak yerde malzeme yok !")
                btnClear_Click(Me, e)
                Exit Sub

            End If

            qty = CDec(txtKutudakiMiktar.Text) * CDec(txtKutuSayisi.Text)


            If dt.Rows(0).Item("qty_on_hand") < qty Then

                MessageBox.Show("Kaynak yerdeki miktar yetersiz !")
                Exit Sub

            End If

          
            'sStr = "select top 1 lot from TRM_FIFO_SUM " & _
            '     " where item = '" & txtItem.Text & "' And whse = '" & txtAmbar.Text & "'" & _
            '     " and site_Ref = '" & IIf(DefaultSite = "", "Default", DefaultSite) & "'"

            'dt = srv.RunSqlDs(sStr, "TRM_FIFO_SUM", Mobile.ProviderTypes.SqlClient).Tables(0)

            'If dt.Rows.Count <> 0 Then

            '    If dt.Rows(0).Item("lot") <> lot Then

            '        MessageBox.Show("Uyarı : Daha eski tarihli lot mevcut !")

            '        sStr = "select top 1 item,loc,lot,qty_on_hand,FIFO from TRM_FIFO_SUM " & _
            '            " where item = '" & Malzeme & "' And whse = '" & ReadConfig("DefaultWhse") & "'" & _
            '            " and site_Ref = '" & IIf(DefaultSite = "", "Default", DefaultSite) & "'"

            '        dt = srv.RunSqlDs(sStr, "TRM_FIFO_SUM", Mobile.ProviderTypes.SqlClient).Tables(0)

            '        If MessageBox.Show("Malzeme : " & dt.Rows(0).Item("item") & "  Yer : " & dt.Rows(0).Item("loc") & _
            '                           "  Lot : " & dt.Rows(0).Item("lot") & "  Eldeki Miktar : " & dt.Rows(0).Item("qty_on_hand") & _
            '                           "  FIFO Trh : " & dt.Rows(0).Item("FIFO") & " Devam etmek istiyor musunuz? ", "Ekip Mapics", _
            '                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then
            '            Exit Sub
            '        End If

            '    End If

            'End If

            Try
               
                sStr = "SELECT * FROM item_mst WHERE lot_tracked <> 1 AND item = '" & Malzeme & "'" & _
                    " and site_Ref = '" & IIf(DefaultSite = "", "Default", DefaultSite) & "'"
                dt = srv.RunSqlDs(sStr, "item_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

                If (dt.Rows.Count > 0) Then
                    MessageBox.Show("Malzeme lot denetimsizdir.", "Uyarı")
                    Exit Sub
                End If

            Catch ex As Exception
                MessageBox.Show("Hata oluştu")
                Exit Sub
            End Try


            sStr = "EXEC TRM_Stok_Kontrol '" & txtItem.Text & "', '" & _
            txtLot.Text & "', '" & UCase(txtGirisYeri.Text) & "', '" & ReadConfig("DefaultSite") & "'"

            srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            If IsDBNull(txtSicil.Text) = True Then
                txtSicil.Text = "0"
            End If

            sStr = "EXEC dbo.TRM_Miktar_Ilerlet @Item = '" & txtItem.Text & "', @loc1 = '" & txtCikisYeri.Text & "', @lot1 = '" & _
                txtLot.Text & "', @loc2 = '" & UCase(txtGirisYeri.Text) & "', @qty ='" & qty & "', @siteSp='" & ReadConfig("DefaultSite") & "'" & _
                " , @Emp_Num ='" & txtSicil.Text & "'"


            srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            sStr = "SELECT * FROM lot_loc_mst WHERE whse = '" & ReadConfig("DefaultWhse") & "' AND item = '" & txtItem.Text & _
                 "' AND lot='" & txtLot.Text & "' AND loc = '" & UCase(txtGirisYeri.Text) & "' AND qty_on_hand >= " & qty & _
                 " and site_Ref = '" & IIf(DefaultSite = "", "Default", DefaultSite) & "'"

            If (srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count = 0) Then
                MessageBox.Show("İşlem başarısız, tekrar deneyiniz")
            End If


            btnClear_Click(Me, e)

        Catch ex As Exception
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
        'cmbBoxCikisYeri.Text = ""
        'cmbBoxCikisYeri.DataSource = Nothing
        txtGirisYeri.Text = ""
        txtKutudakiMiktar.Text = ""
        txtKutuSayisi.Text = "1"
        txtItem.Focus()

    End Sub


    Private Sub frmYeniMiktarIlerlet_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        srv.Url = ReadConfig("path")
        txtAmbar.Text = ReadConfig("DefaultWhse")
        DefaultSite = ReadConfig("DefaultSite")
        txtSicil.Focus()
        txtSicil.Text = kullanici
        txtKutuSayisi.Text = 1
        txtCikisYeri.Text = "ONY"
    End Sub

    Private Sub txtDocNo_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtSicil.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            txtItem.Focus()
        End If
    End Sub

    Private Sub txtItem_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtItem.KeyPress


        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then


            Cursor.Current = Cursors.WaitCursor

            Try
                'txtItem.Text = String.Empty
                txtLot.Text = String.Empty
                txtKutudakiMiktar.Text = String.Empty

                If txtItem.Text.Trim.Length > 0 Then
                    Dim dtTmp As New DataTable
                    sStr = " select MALZEMEKODU, LOT_NO, MIKTAR from TRM_LABELDB with (nolock) " & _
                           " where SeriNo=" & txtItem.Text.Trim
                    dtTmp = srv.RunSqlDs(sStr, "TRM_LABELDB", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If Not dtTmp Is Nothing AndAlso dtTmp.Rows.Count > 0 Then
                        With dtTmp.Rows(0)
                            Malzeme = .Item("MALZEMEKODU").ToString
                            lot = .Item("LOT_NO").ToString
                            txtKutudakiMiktar.Text = .Item("MIKTAR").ToString
                        End With
                    End If

                    If String.IsNullOrEmpty(txtKutudakiMiktar.Text.Trim) Then
                        txtKutudakiMiktar.Text = 0
                    End If

                    txtItem.Text = Malzeme
                    txtLot.Text = lot
                    txtQty.Text = CDec(txtKutudakiMiktar.Text) * CDec(txtKutuSayisi.Text)
                    txtGirisYeri.Text = ""

                    '    '01.10.2012
                    '    'sStr = "SELECT loc FROM lot_loc_mst WHERE item = '" & _
                    '    '     txtItem.Text & "' " & "And lot = '" & txtLot.Text & "' And qty_on_hand > 0 and whse='" & txtAmbar.Text & "'"

                    '    'dt = srv.RunSqlDs(sStr, "lot_loc_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

                    '    'cmbBoxCikisYeri.DataSource = dt

                    'txtQty.Focus()

                    'If txtItem.Text.IndexOf("%") <> -1 Then

                    '    Malzeme = txtItem.Text.Split("%")(0)
                    '    lot = txtItem.Text.Split("%")(1)

                    '    'If txtItem.Text.Split("%").Length > "2" Then
                    '    '    Miktar = txtItem.Text.Split("%")(2)
                    '    'Else
                    '    '    Miktar = 0
                    '    'End If

                    '    If txtItem.Text.Split("%").Length > 2 Then
                    '        txtKutudakiMiktar.Text = txtItem.Text.Split("%")(2)
                    '    Else
                    '        txtKutudakiMiktar.Text = 0
                    '    End If


                    '    txtItem.Text = Malzeme
                    '    txtLot.Text = lot
                    '    txtQty.Text = CDec(txtKutudakiMiktar.Text) * CDec(txtKutuSayisi.Text)
                    '    txtGirisYeri.Text = ""

                    '    '01.10.2012
                    '    'sStr = "SELECT loc FROM lot_loc_mst WHERE item = '" & _
                    '    '     txtItem.Text & "' " & "And lot = '" & txtLot.Text & "' And qty_on_hand > 0 and whse='" & txtAmbar.Text & "'"

                    '    'dt = srv.RunSqlDs(sStr, "lot_loc_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

                    '    'cmbBoxCikisYeri.DataSource = dt

                    '    txtQty.Focus()

                    'End If

                    txtGirisYeri.Focus()

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
                    sStr = "select top 1 qty_on_hand from lot_loc_mst " & _
                         " where item = '" & txtItem.Text & "' AND loc = '" & txtCikisYeri.Text & "'" & _
                         " AND lot = '" & txtLot.Text & "' and whse='" & txtAmbar.Text & "'" & _
                         " and site_Ref = '" & IIf(DefaultSite = "", "Default", DefaultSite) & "'"

                    dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt.Rows.Count = 0 Then
                        MessageBox.Show("Kaynak yerde malzeme yok !")
                        Exit Sub
                    Else
                        txtKutuSayisi.Text = dt.Rows(0).Item(0)
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

    Private Sub txtKutudakiMiktar_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtKutudakiMiktar.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            txtQty.Text = CDec(txtKutudakiMiktar.Text) * CDec(txtKutuSayisi.Text)
        End If
    End Sub

    Private Sub txtKutuSayisi_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtKutuSayisi.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            txtQty.Text = CDec(txtKutudakiMiktar.Text) * CDec(txtKutuSayisi.Text)
        End If
    End Sub

    Private Sub txtGirisYeri_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtGirisYeri.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            txtKutuSayisi.Focus()
        End If
    End Sub
End Class