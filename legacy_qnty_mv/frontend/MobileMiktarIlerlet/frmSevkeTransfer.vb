Imports System.Data

Public Class frmSevkeTransfer

    Dim dt As New DataTable
    Dim sStr As String = String.Empty
    Dim srv As New Mobile.wsGeneral
    Dim DefaultSite As String = String.Empty

    Dim Malzeme As String = String.Empty
    Dim lot As String = String.Empty
    Dim Miktar As String = String.Empty
    Dim qty As Decimal = 0
    Dim seriNo As String = String.Empty

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Try
            If txtItem.Text.Trim = "" Or txtQty.Text.Trim = "" Or cmbBoxCikisYeri.Text.Trim = "" Or _
                cmbGirisYeri.Text.Trim = "" Or txtLot.Text.Trim = "" Or txtCeklist.Text.Trim = "" Then

                MessageBox.Show("Malzeme, Miktar, Çıkış Yeri, Giriş Yeri, Lot, Eldeki Miktar, Çekme No Alanlarından Biri Yada Birkaçı Boş.")
                Exit Sub

            End If

            If cmbBoxCikisYeri.Text.Trim.Split("-")(0) = cmbGirisYeri.Text.Trim Then
                MessageBox.Show("Giriş ve Çıkış Yeri Aynı Olamaz!")
                Exit Sub
            End If

            sStr = "select * from TrM_TRCEKLIST where CEKLIST = " & txtCeklist.Text
            dt = srv.RunSqlDs(sStr, "TrM_TRCEKLIST", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count = 0 Then
                MessageBox.Show("Girmiş olduğunuz çekme numarası geçersiz.")
                Exit Sub
            End If

            sStr = "select * from TrM_TRCEKLIST where CEKLIST = " & txtCeklist.Text & " and item = '" & txtItem.Text & "'"
            dt = srv.RunSqlDs(sStr, "TrM_TRCEKLIST", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count = 0 Then
                MessageBox.Show("Girmiş olduğunuz malzeme çekme listesinde bulunamadı.")
                Exit Sub
            End If

          

            sStr = "select * from TRM_LABELDB where isnull(Cekme,0) > 0 and SeriNo = " & seriNo
            dt = srv.RunSqlDs(sStr, "TRM_LABELDB", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count > 0 Then
                MessageBox.Show("Etiket sevke transfer edilmiş.")
                Exit Sub
            End If


            sStr = "select top 1 qty_on_hand from lot_loc_mst " & _
                 " where item = '" & txtItem.Text & "' AND loc = '" & cmbBoxCikisYeri.Text.Split("-")(0) & "'" & _
                 " AND lot = '" & txtLot.Text & "' and whse='" & txtAmbar.Text & "'" & _
                 " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

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

            sStr = "select top 1 lot from TRM_FIFO_SUM " & _
                 " where item = '" & txtItem.Text & "' And whse = '" & txtAmbar.Text & "'" & _
                 " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

            dt = srv.RunSqlDs(sStr, "TRM_FIFO_SUM", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count <> 0 Then

                If dt.Rows(0).Item("lot") <> lot Then

                    MessageBox.Show("Uyarı : Daha eski tarihli lot mevcut !")

                    sStr = "select top 1 item,loc,lot,qty_on_hand,FIFO from TRM_FIFO_SUM " & _
                        " where item = '" & Malzeme & "' And whse = '" & ReadConfig("DefaultWhse") & "'" & _
                        " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

                    dt = srv.RunSqlDs(sStr, "TRM_FIFO_SUM", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If MessageBox.Show("Malzeme : " & dt.Rows(0).Item("item") & "  Yer : " & dt.Rows(0).Item("loc") & _
                                       "  Lot : " & dt.Rows(0).Item("lot") & "  Eldeki Miktar : " & dt.Rows(0).Item("qty_on_hand") & _
                                       "  FIFO Trh : " & dt.Rows(0).Item("FIFO") & " Devam etmek istiyor musunuz? ", "Ekip Mapics", _
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then
                        Exit Sub
                    End If

                End If

            End If

            Try
                sStr = "SELECT * FROM item_mst WHERE lot_tracked <> 1 AND item = '" & Malzeme & "'" & _
                    " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"
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
            txtLot.Text & "', '" & UCase(cmbGirisYeri.Text) & "', '" & ReadConfig("DefaultSite") & "'"

            srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            'If IsDBNull(txtSicil.Text) = True Then
            '    txtSicil.Text = "0"
            'End If

            sStr = "EXEC dbo.TRM_Miktar_Ilerlet @Item = '" & txtItem.Text & "', @loc1 = '" & cmbBoxCikisYeri.Text.Split("-")(0) & "', @lot1 = '" & _
                txtLot.Text & "', @loc2 = '" & UCase(cmbGirisYeri.Text) & "', @qty ='" & qty & "', @siteSp='" & ReadConfig("DefaultSite") & "'" & _
                " , @Emp_Num ='" & kullanici & "',@cekme = " & txtCeklist.Text


            srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            sStr = "UPDATE TRM_LABELDB SET Cekme = " & txtCeklist.Text & " WHERE SeriNo = " & seriNo
            srv.RunSql(sStr, True, Mobile.ProviderTypes.SqlClient)

            sStr = "SELECT * FROM lot_loc_mst WHERE whse = '" & ReadConfig("DefaultWhse") & "' AND item = '" & txtItem.Text & _
                 "' AND lot='" & txtLot.Text & "' AND loc = '" & UCase(cmbGirisYeri.Text) & "' AND qty_on_hand >= " & qty & _
                 " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

            If (srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count = 0) Then
                MessageBox.Show("İşlem başarısız, tekrar deneyiniz")
            End If


            'btnClear_Click(Me, e)
            txtItem.Text = ""
            txtLot.Text = ""
            txtQty.Text = ""
            cmbBoxCikisYeri.Text = ""
            cmbBoxCikisYeri.DataSource = Nothing
            txtKutudakiMiktar.Text = ""
            txtKutuSayisi.Text = "1"
            txtItem.Focus()


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
        cmbBoxCikisYeri.Text = ""
        cmbBoxCikisYeri.DataSource = Nothing
        txtKutudakiMiktar.Text = ""
        txtKutuSayisi.Text = "1"
        txtItem.Focus()
        cmbGirisYeri.Text = ""

    End Sub


    Private Sub frmYeniMiktarIlerlet_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        srv.Url = ReadConfig("path")
        txtAmbar.Text = ReadConfig("DefaultWhse")
        DefaultSite = ReadConfig("DefaultSite")
        'txtSicil.Focus()
        'txtSicil.Text = kullanici
        txtKutuSayisi.Text = 1
        'txtGirisYeri.Text = "SEVK"

        'sStr = "select * from TR_SEVKLOC"
        'dt = srv.RunSqlDs(sStr, "TR_SEVKLOC", Mobile.ProviderTypes.SqlClient).Tables(0)

        'cmbGirisYeri.DataSource = dt
        'cmbGirisYeri.ValueMember = "loc"
        'cmbGirisYeri.DisplayMember = "loc"
        'cmbGirisYeri.Text = "SEVK"

    End Sub

    Private Sub txtDocNo_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
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

                    seriNo = txtItem.Text
                    txtItem.Text = Malzeme
                    txtLot.Text = lot
                    txtQty.Text = CDec(txtKutudakiMiktar.Text) * CDec(txtKutuSayisi.Text)
                    'txtGirisYeri.Text = ""

                    '01.10.2012
                    'sStr = "SELECT loc FROM lot_loc_mst WHERE item = '" & _
                    '     txtItem.Text & "' " & "And lot = '" & txtLot.Text & "' And qty_on_hand > 0 " & _
                    '     " and whse='" & txtAmbar.Text & "' and loc NOT LIKE '%SEVK%' "

                    'dt = srv.RunSqlDs(sStr, "lot_loc_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

                    'cmbBoxCikisYeri.DataSource = dt
                    'cmbBoxCikisYeri.DisplayMember = "loc"
                    'cmbBoxCikisYeri.ValueMember = "loc"

                    'cmbBoxCikisYeri.Focus()

                    sStr = "SELECT (loc + '-' + cast( cast(qty_on_hand as decimal(10,2)) as nvarchar(18) ) )as loc  FROM lot_loc_mst WHERE item = '" & _
                       txtItem.Text & "' " & "And lot = '" & txtLot.Text & "' And qty_on_hand > 0 and whse='" & txtAmbar.Text & "' and loc NOT LIKE '%SEVK%'"

                    dt = srv.RunSqlDs(sStr, "lot_loc_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

                    cmbBoxCikisYeri.DataSource = dt
                    cmbBoxCikisYeri.DisplayMember = "loc"
                    cmbBoxCikisYeri.ValueMember = "loc"
                    cmbBoxCikisYeri.Focus()

                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
                Cursor.Current = Cursors.Default
            End Try

        End If
    End Sub

    Private Sub txtCikisYeri_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            Cursor.Current = Cursors.WaitCursor
            Try
                If txtItem.Text <> "" Then
                    sStr = "select top 1 qty_on_hand from lot_loc_mst " & _
                         " where item = '" & txtItem.Text & "' AND loc = '" & cmbBoxCikisYeri.Text & "'" & _
                         " AND lot = '" & txtLot.Text & "' and whse='" & txtAmbar.Text & "'" & _
                         " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

                    dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt.Rows.Count = 0 Then
                        MessageBox.Show("Kaynak yerde malzeme yok !")
                        Exit Sub
                    Else
                        txtKutuSayisi.Text = dt.Rows(0).Item(0)
                    End If

                End If

                cmbGirisYeri.Focus()

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
                Cursor.Current = Cursors.Default
            End Try
        End If

    End Sub

    Private Sub txtQty_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtQty.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            cmbBoxCikisYeri.Focus()
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
        If txtKutuSayisi.Text.Trim = "" Then
            txtKutuSayisi.Text = "1"
        End If
    End Sub

    Private Sub txtCeklist_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtCeklist.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            sStr = "select TOP 1 SEVKYERI FROM TRM_TRCEKLIST_V WHERE CEKLIST = " & txtCeklist.Text
            dt = srv.RunSqlDs(sStr, "TRM_TRCEKLIST_V", Mobile.ProviderTypes.SqlClient).Tables(0)

            'If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            '    With dt.Rows(0)
            cmbGirisYeri.Text = dt.Rows(0)(0)
            'End With
            'End If

            txtItem.Focus()

        End If
    End Sub
End Class