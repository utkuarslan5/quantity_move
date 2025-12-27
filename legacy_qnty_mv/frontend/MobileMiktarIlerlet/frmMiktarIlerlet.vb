Imports System.Data

Public Class frmMiktarIlerlet
    Dim dt As New DataTable
    Dim sStr As String = String.Empty
    Dim srv As New Mobile.wsGeneral


    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        txtItem.Text = ""
        txtQty.Text = ""
        txtLot.Text = ""
        txtBasvuru.Text = ""
        txtItem.Focus()
        txtItem.Enabled = True
    End Sub


    Private Sub txtItem_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtItem.KeyDown
        'Dim Malzeme As String = String.Empty
        'Dim lot As String = String.Empty
        'Dim qty As String = String.Empty


        'If e.KeyCode = Keys.Enter Then

        '    Try
        '        If txtItem.Text.Trim.Length > 0 Then

        '            If txtItem.Text.IndexOf("%") <> -1 Then
        '                Malzeme = txtItem.Text.Split("%")(0)
        '                lot = txtItem.Text.Split("%")(1)

        '                If UBound(txtItem.Text.Split("%")).ToString = "2" Then
        '                    qty = txtItem.Text.Split("%")(2)
        '                Else
        '                    qty = 0
        '                End If

        '                'qty = txtItem.Text.Split("%")(2)
        '                txtItem.Text = Malzeme
        '                txtLot.Text = lot
        '                txtQty.Text = qty

        '                'If ReadConfig("kalite") = "0" Then

        '                sStr = "SELECT loc FROM lot_loc WHERE item = '" & _
        '                     txtItem.Text & "' " & "And lot = '" & txtLot.Text & "' And qty_on_hand > 0 AND whse='" & ReadConfig("DefaultWhse") & "'"
        '                cmbBoxCikisYeri.DisplayMember = "loc"
        '                cmbBoxCikisYeri.ValueMember = "loc"

        '                dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

        '                cmbBoxCikisYeri.DataSource = dt

        '                'Else

        '                If ReadConfig("kalite") = "1" Then
        '                    cmbBoxCikisYeri.Text = "KK"
        '                End If

        '                cmbBoxCikisYeri.Focus()

        '            End If
        '        End If

        '    Catch ex As Exception
        '        MessageBox.Show(ex.Message)
        '    Finally
        '        Cursor.Current = Cursors.Default

        '    End Try

        'End If
    End Sub

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Try
            If (txtBasvuru.Text = "" Or txtItem.Text = "" Or txtLot.Text = "" Or txtQty.Text = "" Or txtYer2.Text = "") Then
                MessageBox.Show("Tüm alanları doldurduğunuzdan emin olunuz.")
                Exit Sub
            End If

            If ReadConfig("kalite") = "0" Then
                If cmbBoxCikisYeri.Text.ToUpper = "KK" Then
                    MessageBox.Show("Onaysız malzeme!")
                    Exit Sub
                End If
            End If

            sStr = "SELECT uf_yer_tipi FROM location WHERE loc='" & cmbBoxCikisYeri.Text & "'"
            dt = srv.RunSqlDs(sStr, "location", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows(0)(0) = "GKK" Then
                MessageBox.Show("Malzeme Lot Girişi Kalite Lokasyonunda!", "Uyarı")
                Exit Sub
            End If

            Dim cmd As String

            cmd = "select qty_on_hand from lot_loc where item = '" & txtItem.Text & "' AND loc = '" & cmbBoxCikisYeri.Text & "'" & _
                " AND lot = '" & txtLot.Text & "'"

            dt = srv.RunSqlDs(cmd, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

            Try
                If (txtQty.Text > dt.Rows(0)(0)) Then
                    MessageBox.Show("Eldeki miktar: " & Convert.ToInt32(dt.Rows(0)(0)) & vbNewLine & _
                    "Bu miktardan fazlası ilerletilemez!")
                    Exit Sub
                End If

                cmd = "SELECT * FROM item WHERE lot_tracked <> 1 AND item ='" & txtItem.Text & "'"
                If (srv.RunSqlDs(cmd, "lot_tracked", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count > 0) Then
                    MessageBox.Show("Malzeme lot denetimsizdir.", "Uyarı")
                    Exit Sub
                End If

            Catch ex As Exception
                MessageBox.Show("Hata oluştu")
                Exit Sub
            End Try

            cmd = "EXEC TR_Stok_Kontrol '" & txtItem.Text & "', '" & _
            txtLot.Text & "', '" & txtYer2.Text & "', '" & ReadConfig("DefaultWhse") & "'"

            srv.RunSqlSp(cmd, Mobile.ProviderTypes.SqlClient)
            'db.RunSp("TR_Stok_Kontrol", p, 1, False)

            'If Not (IsDBNull(p2.Value) Or p2.Value Is Nothing) Then
            '    MessageBox.Show(p2.Value.ToString)
            '    Return
            'End If

            cmd = "EXEC dbo.TR_Miktar_Ilerlet @Item = '" & txtItem.Text & "', @loc1 = '" & cmbBoxCikisYeri.Text + "', @lot1 = '" & _
                txtLot.Text & "', @loc2 = '" & txtYer2.Text & "', @qty ='" & txtQty.Text & "',@DocumentNum='" & txtBasvuru.Text & "'"

            srv.RunSqlSp(cmd, Mobile.ProviderTypes.SqlClient)

            cmd = "SELECT * FROM lot_loc WHERE  item = '" & txtItem.Text & _
                "' AND lot='" & txtLot.Text & "' AND loc = '" & txtYer2.Text & "' AND qty_on_hand >= " & txtQty.Text

            If (srv.RunSqlDs(cmd, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count = 0) Then
                MessageBox.Show("İşlem başarısız, tekrar deneyiniz")
            End If

            txtItem.Text = ""
            txtLot.Text = ""
            txtQty.Text = ""

            If ReadConfig("kalite") = "0" Then
                txtYer2.Text = ""
            End If

            cmbBoxCikisYeri.Text = ""
            cmbBoxCikisYeri.DataSource = Nothing
            txtItem.Focus()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try


    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Close()
        frmMenu.Visible = True
    End Sub

    Private Sub frmMiktarIlerlet_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        srv.Url = ReadConfig("path")

        If ReadConfig("kalite") = "1" Then
            txtYer2.Text = "ONAY"
        End If

        txtItem.Focus()

    End Sub

    Private Sub txtItem_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtItem.KeyPress
        Dim Malzeme As String = String.Empty
        Dim lot As String = String.Empty
        Dim qty As String = String.Empty


        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            Try
                If txtItem.Text.Trim.Length > 0 Then

                    If txtItem.Text.IndexOf("%") <> -1 Then
                        Malzeme = txtItem.Text.Split("%")(0)
                        lot = txtItem.Text.Split("%")(1)

                        If UBound(txtItem.Text.Split("%")).ToString = "2" Then
                            qty = txtItem.Text.Split("%")(2)
                        Else
                            qty = 0
                        End If

                        'qty = txtItem.Text.Split("%")(2)
                        txtItem.Text = Malzeme
                        txtLot.Text = lot
                        txtQty.Text = qty

                        'If ReadConfig("kalite") = "0" Then

                        sStr = "SELECT loc FROM lot_loc WHERE item = '" & _
                             txtItem.Text & "' " & "And lot = '" & txtLot.Text & "' And qty_on_hand > 0 AND whse='" & ReadConfig("DefaultWhse") & "'"
                        cmbBoxCikisYeri.DisplayMember = "loc"
                        cmbBoxCikisYeri.ValueMember = "loc"

                        dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

                        cmbBoxCikisYeri.DataSource = dt

                        'Else

                        If ReadConfig("kalite") = "1" Then
                            cmbBoxCikisYeri.Text = "KK"
                        End If

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