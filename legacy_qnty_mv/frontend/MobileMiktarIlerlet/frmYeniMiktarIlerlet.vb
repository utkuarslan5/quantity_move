Imports System.Data

Public Class frmYeniMiktarIlerlet

    Dim dt As New DataTable
    Dim sStr As String = String.Empty
    Dim srv As New Mobile.wsGeneral
    Dim DefaultSite As String = String.Empty

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Try
            If txtItem.Text = "" Or txtQty.Text = "" Or cmbBoxCikisYeri.Text = "" Or txtGirisYeri.Text = "" Or txtLot.Text = "" Or txtEldekiMiktar.Text = "" Then
                MessageBox.Show("Malzeme, Miktar, Çıkış Yeri, Giriş Yeri, Lot, Eldeki Miktar Alanlarından Biri Yada Birkaçı Boş.")
                Exit Sub
            End If

            sStr = "select top 1 qty_on_hand from lot_loc_mst where item = '" & txtItem.Text & "' AND loc = '" & _
                 cmbBoxCikisYeri.Text & "'" & " AND lot = '" & txtLot.Text & "' and whse='" & txtAmbar.Text & "'" & _
                 " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

            dt = srv.RunSqlDs(sStr, "lot_loc_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count = 0 Then

                MessageBox.Show("Kaynak yerde malzeme yok !")
                btnClear_Click(Me, e)
                Exit Sub

            End If

            If dt.Rows(0).Item("qty_on_hand") < txtQty.Text Then

                MessageBox.Show("Kaynak yerdeki miktar yetersiz !")
                Exit Sub

            End If

            If txtGirisYeri.Text = cmbBoxCikisYeri.Text Then
                MessageBox.Show("Giriş ve çıkış yeri aynı olamaz !", "Uyarı")
                Exit Sub
            End If

         

            Try

                If (CInt(txtQty.Text) > CInt(txtEldekiMiktar.Text)) Then
                    MessageBox.Show("Eldeki miktar: " & txtEldekiMiktar.Text & vbNewLine & "Bu miktardan fazlası ilerletilemez!")
                    Exit Sub
                End If

                sStr = "SELECT * FROM item_mst WHERE lot_tracked <> 1 AND item ='" & (txtItem.Text) & "'" & _
                  " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

                If (srv.RunSqlDs(sStr, "item", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count > 0) Then
                    MessageBox.Show("Malzeme lot denetimsizdir.", "Uyarı")
                    Exit Sub
                End If

            Catch ex As Exception
                MessageBox.Show("Hata oluştu")
                Exit Sub
            End Try

            sStr = "EXEC TrM_Stok_Kontrol_SL9 '" & txtItem.Text & "', '" & _
            txtLot.Text & "', '" & UCase(txtGirisYeri.Text) & "', '" & ReadConfig("DefaultSite") & "'"

            srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            'If IsDBNull(txtDocNo.Text) = True Then
            '    txtDocNo.Text = "0"
            'End If

            sStr = "EXEC dbo.TrM_Miktar_Ilerlet_SL9 @Item = '" & txtItem.Text & "', @loc1 = '" & cmbBoxCikisYeri.Text + "', @lot1 = '" & _
                txtLot.Text & "', @loc2 = '" & UCase(txtGirisYeri.Text) & "', @qty ='" & txtQty.Text & "', @siteSp='" & ReadConfig("DefaultSite") & "'" & _
                ",@Emp_Num='" & frmKullaniciGirisi.kullanici & "'"

            srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            sStr = "SELECT * FROM lot_loc_mst WHERE whse = '" & ReadConfig("DefaultWhse") & "' AND item = '" & txtItem.Text & _
                "' AND lot='" & txtLot.Text & "' AND loc = '" & UCase(txtGirisYeri.Text) & "' AND qty_on_hand >= " & txtQty.Text & _
              " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

            If (srv.RunSqlDs(sStr, "lot_loc_mst", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count = 0) Then
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
        cmbBoxCikisYeri.Text = ""
        txtGirisYeri.Text = ""
        txtEldekiMiktar.Text = ""
        'txtSonYer.Text = ""
        txtItem.Focus()

    End Sub

    Private Sub cmbBoxCikisYeri_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)


    End Sub

    Private Sub frmYeniMiktarIlerlet_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        srv.Url = ReadConfig("path")
        txtAmbar.Text = ReadConfig("DefaultWhse")
        'txtDocNo.Focus()
    End Sub

    Private Sub txtDocNo_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
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

                        If Miktar = 0 Then
                            txtQty.Text = ""
                        Else
                            txtQty.Text = Miktar
                        End If

                        txtGirisYeri.Text = ""


                        sStr = "SELECT loc FROM lot_loc_mst WHERE item = '" & _
                             txtItem.Text & "' " & "And lot = '" & txtLot.Text & "' And qty_on_hand > 0 AND whse='" & txtAmbar.Text & "'"

                        dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

                        cmbBoxCikisYeri.DataSource = dt
                        cmbBoxCikisYeri.DisplayMember = "loc"
                        cmbBoxCikisYeri.ValueMember = "loc"

                        'If ReadConfig("kalite") = "1" Then
                        '    cmbBoxCikisYeri.Text = "KK"
                        'End If

                        cmbBoxCikisYeri.Focus()

                    
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

    Private Sub txtCikisYeri_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            Cursor.Current = Cursors.WaitCursor
            Try
                If txtItem.Text <> "" Then
                    sStr = "select top 1 qty_on_hand from lot_loc_mst where item = '" & txtItem.Text & "' AND loc = '" & cmbBoxCikisYeri.Text & "'" & _
                   " AND lot = '" & txtLot.Text & "' and whse='" & txtAmbar.Text & "'" & _
                     " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

                    dt = srv.RunSqlDs(sStr, "lot_loc_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

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
            txtGirisYeri.Focus()
        End If
    End Sub

    Private Sub btnX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnX.Click
        Me.Hide()
        frmMenu.Visible = True
    End Sub

    Private Sub cmbBoxCikisYeri_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbBoxCikisYeri.SelectedValueChanged
        Try
            Dim qty As String
            sStr = "SELECT qty_on_hand FROM lot_loc_mst WHERE lot ='" & txtLot.Text & "' AND item ='" & txtItem.Text & "' AND loc = '" & cmbBoxCikisYeri.Text & "'"
            dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)
            If dt.Rows.Count = 0 Then
                txtEldekiMiktar.Text = "0"
            Else
                'txtQty.Text = dt.Rows(0)(0)
                qty = dt.Rows(0)(0)
                qty = qty.Split(".")(0) & "." & qty.Split(".")(1).Substring(0, 3)
                txtEldekiMiktar.Text = qty
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
End Class