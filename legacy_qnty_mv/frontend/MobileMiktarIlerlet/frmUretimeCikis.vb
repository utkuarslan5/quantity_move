Imports System.Data
Imports System.Net
Imports System.Net.Sockets
Imports System.Text.Encoding
Imports System.Text


Public Class frmUretimeCikis

    Dim dt As New DataTable
    Dim sStr As String = String.Empty
    Dim srv As New Mobile.wsGeneral
    Dim tanim As String
    Dim ölcüBirimi As String
    Dim DefaultSite As String = String.Empty
    Dim eldekiMiktar As Decimal = 0
    Dim qty As Decimal = 0
    Dim SeriNo As Integer = 0

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Try
            If txtItem.Text = "" Or cmbBoxCikisYeri.Text = "" Or txtLot.Text = "" Then
                MessageBox.Show("Malzeme, Çıkış Yeri, Lot Alanlarından Biri Yada Birkaçı Boş.")
                Exit Sub
            End If

            If txtKutuSayisi.Text = "" Or txtKutuSayisi.Text = 0 Then
                MessageBox.Show("Lütfen kutu sayısını giriniz.")
                Exit Sub
            End If

            qty = CDec(txtKutudakiMiktar.Text) * CDec(txtKutuSayisi.Text)
            'Miktar = qty.ToString
            'txtQty.Text = Miktar

            If qty = 0 Then
                MessageBox.Show("Kutudaki miktar sıfırdan farklı olmalıdır.")
                Exit Sub
            End If

            sStr = "SELECT loc FROM location_mst WHERE loc='" & cmbBoxCikisYeri.Text.Split("-")(0) & "'" & _
            " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

            dt = srv.RunSqlDs(sStr, "location_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count <> 0 Then
                If dt.Rows(0)(0) = "GKK" Then
                    MessageBox.Show("Malzeme Lot Girişi Kalite Lokasyonunda!", "Uyarı")
                    Exit Sub
                End If
            Else
                MessageBox.Show(cmbBoxCikisYeri.Text & " lokasyonu sistemde tanımlı değil. Tekrar deneyiniz.")
                Exit Sub
            End If

            sStr = "select top 1 qty_on_hand from lot_loc_mst where item = '" & txtItem.Text & "' AND loc = '" & _
                 cmbBoxCikisYeri.Text.Split("-")(0) & "'" & " AND lot = '" & txtLot.Text & "' and whse='" & txtAmbar.Text & "'" & _
                 " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

            dt = srv.RunSqlDs(sStr, "lot_loc_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count = 0 Then

                MessageBox.Show("Kaynak yerde malzeme yok !")
                btnClear_Click(Me, e)
                Exit Sub

            End If

            eldekiMiktar = dt.Rows(0).Item("qty_on_hand")

            If eldekiMiktar < qty Then

                MessageBox.Show("Kaynak yerdeki miktar yetersiz !")
                Exit Sub

            End If

            'If txtCikisYeri.Text.Substring(0, 2) <> "GD" Then

            sStr = "select top 1 lot from TRM_FIFO_SUM where item = '" & txtItem.Text & "' And whse = '" & txtAmbar.Text & "'" & _
                 " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

            dt = srv.RunSqlDs(sStr, "TRM_FIFO_SUM", Mobile.ProviderTypes.SqlClient).Tables(0)

            If dt.Rows.Count = 0 Then
                MessageBox.Show("FIFO Kaydı Bulunamadı.", "Uyarı")
                Exit Sub
            End If


            If Trim(dt.Rows(0).Item("lot")) <> txtLot.Text Then

                sStr = "select top 1 item,loc,lot,qty_on_hand,FIFO from TRM_FIFO_SUM where item = '" & _
                     txtItem.Text & "' And whse = '" & txtAmbar.Text & "'" & _
                     " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

                dt = srv.RunSqlDs(sStr, "TR_FIFO_SUM", Mobile.ProviderTypes.SqlClient).Tables(0)


                If MessageBox.Show("Malzeme : " & dt.Rows(0).Item("item") & "  Yer : " & dt.Rows(0).Item("loc") & _
                                   "  Lot : " & dt.Rows(0).Item("lot") & "  Eldeki Miktar : " & dt.Rows(0).Item("qty_on_hand") & _
                                   "  FIFO Trh : " & dt.Rows(0).Item("FIFO") & " Devam etmek istiyor musunuz? ", "Ekip Mapics", _
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then
                    Exit Sub
                End If



            End If


            Try


                sStr = "SELECT * FROM item_mst WHERE lot_tracked <> 1 AND item ='" & (txtItem.Text) & "'" & _
                     " and site_Ref = '" & IIf(DefaultSite = "", "faz", DefaultSite) & "'"

                If (srv.RunSqlDs(sStr, "item_mst", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count > 0) Then
                    MessageBox.Show("Malzeme lot denetimsizdir.", "Uyarı")
                    Exit Sub
                End If

            Catch ex As Exception
                MessageBox.Show("Hata oluştu")
                Exit Sub
            End Try

            'çeşitli girişte aktif edilecek -----------------------------------------------------------------------
            'sStr = "EXEC TR_Stok_Kontrol '" & txtItem.Text & "', '" & _
            'txtLot.Text & "', '" & UCase(txtGirisYeri.Text) & "', '" & ReadConfig("DefaultWhse") & "'"

            'srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            'If IsDBNull(txtDocNo.Text) = True Then
            '    txtDocNo.Text = "0"
            'End If
            '-------------------------------------------------------------------------------------------------------

            Dim MaxTransNum As Integer
            Dim dtTmp As New DataTable
            sStr = " select isnull(max(trans_num),0)+1 from dcitem_mst "
            dtTmp = srv.RunSqlDs(sStr, "dcitem_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

            If Not dtTmp Is Nothing AndAlso dtTmp.Rows.Count > 0 Then
                MaxTransNum = CInt(dtTmp.Rows(0)(0).ToString)
            End If

            If cmbBoxCikisTipi.Text = "Üretime Çıkış" Then

                sStr = " declare @p1 nvarchar(max) " & vbNewLine & _
                 " set @p1 = null " & vbNewLine & _
              " EXEC dbo.TRM_Dcitem_Insert_Post_ck @trans_num = '" & MaxTransNum & "', @trans_date = '" & _
              DateTime.Now.ToString("yyyy-MM-dd").ToString & " 00:00:00:000" & "', @trans_type= 2 " & _
              ", @item = '" & txtItem.Text & "', @loc ='" & cmbBoxCikisYeri.Text.Split("-")(0) & "', @lot='" & txtLot.Text & "'" & _
              ", @count_qty = " & qty & ", @whse='" & txtAmbar.Text & "', @siteref = '" & IIf(DefaultSite = "", "Default", DefaultSite) & _
              "', @emp_num = '" & txtSicil.Text & "', @cikisTipi=0 , @pResult = @p1 output " & vbNewLine & _
              " Select @p1 "

                dt = srv.RunSqlDs(sStr, "TRM_Dcitem_Insert_Post", Mobile.ProviderTypes.SqlClient).Tables(0)
            Else

                sStr = " declare @p1 nvarchar(max) " & vbNewLine & _
                " set @p1 = null " & vbNewLine & _
             " EXEC dbo.TRM_Dcitem_Insert_Post_ck @trans_num = '" & MaxTransNum & "', @trans_date = '" & _
             DateTime.Now.ToString("yyyy-MM-dd").ToString & " 00:00:00:000" & "', @trans_type= 2 " & _
             ", @item = '" & txtItem.Text & "', @loc ='" & cmbBoxCikisYeri.Text.Split("-")(0) & "', @lot='" & txtLot.Text & "'" & _
             ", @count_qty = " & qty & ", @whse='" & txtAmbar.Text & "', @siteref = '" & IIf(DefaultSite = "", "Default", DefaultSite) & _
             "', @emp_num = '" & txtSicil.Text & "', @cikisTipi=1, @pResult = @p1 output " & vbNewLine & _
             " Select @p1 "

                dt = srv.RunSqlDs(sStr, "TRM_Dcitem_Insert_Post", Mobile.ProviderTypes.SqlClient).Tables(0)

            End If

          
            If dt.Rows(0)(1).ToString <> "" Then
                MessageBox.Show(dt.Rows(0)(1))
            End If

            'If SeriNo <> 0 Then
            sStr = " update TRM_LABELDB set durum='1' " & _
                   " where SeriNo=" & SeriNo
            srv.RunSql(sStr, False, Mobile.ProviderTypes.SqlClient)
            'End If

            btnClear_Click(sender, e)

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Hide()
        frmMenu.Visible = True
    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click

        cmbBoxCikisYeri.DataSource = Nothing
        cmbBoxCikisYeri.Text = ""
        cmbBoxCikisYeri.ValueMember = "loc"
        cmbBoxCikisYeri.DisplayMember = "loc"

        txtItem.Text = ""
        txtLot.Text = ""
        txtQty.Text = ""

        txtKutudakiMiktar.Text = ""
        txtKutuSayisi.Text = "1"
        txtItem.Focus()

    End Sub

    'Private Sub cmbBoxCikisYeri_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    '    If txtItem.Text <> "" Then
    '        sStr = "select top 1 qty_on_hand from lot_loc where item = '" & txtItem.Text & "' AND loc = '" & cmbBoxCikisYeri.Text & "'" & _
    '       " AND lot = '" & txtLot.Text & "' and whse='" & txtAmbar.Text & "'"

    '        dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

    '        If dt.Rows.Count = 0 Then

    '            MessageBox.Show("Kaynak yerde malzeme yok !")
    '            Exit Sub
    '        Else
    '            txtEldekiMiktar.Text = dt.Rows(0).Item(0)
    '        End If
    '    End If
    'End Sub

    Private Sub frmUretimeCikis_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        srv.Url = ReadConfig("path")
        txtAmbar.Text = ReadConfig("DefaultWhse")
        DefaultSite = ReadConfig("DefaultSite")
        cmbBoxCikisTipi.Text = "Üretime Çıkış"
        txtKutuSayisi.Text = 1
        txtSicil.Text = kullanici
    End Sub

    Private Sub txtDocNo_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            txtItem.Focus()
        End If
    End Sub

    Private Sub txtItem_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtItem.KeyPress
        Dim Malzeme As String = String.Empty
        Dim lot As String = String.Empty
        'Dim Miktar As String = String.Empty

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            'txtItem.Text = txtItem.Text.Replace(".", "/")
            'txtItem.Text = txtItem.Text.Replace("ç", ".")
            'txtItem.Text = txtItem.Text.Replace("Ç", ".")
            'txtItem.Text = txtItem.Text.Replace("ö", ",")
            'txtItem.Text = txtItem.Text.Replace("Ö", ",")
            'txtItem.Text = txtItem.Text.Replace("*", "-")


            Cursor.Current = Cursors.WaitCursor

            Try
                'txtItem.Text = String.Empty
                txtLot.Text = String.Empty
                txtKutudakiMiktar.Text = String.Empty

                SeriNo = 0

                If txtItem.Text.Trim.Length > 0 Then

                    sStr = " select count(*) from TRM_LABELDB " & _
                           " where SeriNo=" & txtItem.Text.Trim & " and durum='1'"
                    dt = srv.RunSqlDs(sStr, "TRM_LABELDB", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt.Rows.Count <> 0 Then
                        If CInt(dt.Rows(0)(0)) > 0 Then
                            MessageBox.Show("Etiket daha önce okutulmuş!")
                            Exit Sub
                        End If
                    End If

                    Dim dtTmp As New DataTable
                    sStr = " select MALZEMEKODU, LOT_NO, MIKTAR from TRM_LABELDB with (nolock) " & _
                           " where SeriNo=" & txtItem.Text.Trim
                    dtTmp = srv.RunSqlDs(sStr, "TRM_LABELDB", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If Not dtTmp Is Nothing AndAlso dtTmp.Rows.Count > 0 Then
                        With dtTmp.Rows(0)
                            SeriNo = txtItem.Text.Trim
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
                    txtKutuSayisi.Focus()

                    sStr = "SELECT (loc + '-' + cast( cast(qty_on_hand as decimal(10,2)) as nvarchar(18) ) )as loc  FROM lot_loc_mst WHERE item = '" & _
                    txtItem.Text & "' " & "And lot = '" & txtLot.Text & "' And qty_on_hand > 0 and whse='" & txtAmbar.Text & "'"

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

                    dt = srv.RunSqlDs(sStr, "lot_loc_mst", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt.Rows.Count = 0 Then

                        MessageBox.Show("Kaynak yerde malzeme yok !")
                        Exit Sub
                    Else
                        'eldt = dt.Rows(0).Item(0)
                    End If
                End If

                'txtGirisYeri.Focus()
                txtItem.Focus()

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

    Private Sub Print()
        'Dim ipAddress As String = "172.16.1.215"
        Dim ipAddress As String = ReadConfig("IpAddress")
        Dim port As Integer = 6101

        'Dim barkod As String = _
        '"! 0 200 200 264 1" & vbNewLine & _
        '"PW 799" & vbNewLine & _
        '"TONE 0" & vbNewLine & _
        '"SPEED 3" & vbNewLine & _
        '"ON-FEED IGNORE" & vbNewLine & _
        '"NO-PACE" & vbNewLine & _
        '"PRESENT-AT 0 0" & vbNewLine & _
        '"BAR-SENSE" & vbNewLine & _
        '"T 5 0 47 34 " & "MALZEME : " & txtItem.Text & vbNewLine & _
        '"T 5 0 47 8 " & "TANIM : " & tanim & vbNewLine & _
        '"T 5 0 47 60 " & "MIKTAR : " & txtQty.Text & vbNewLine & _
        '"T 5 0 47 86 " & "OLCU BIRIMI : " & ölcüBirimi & vbNewLine & _
        '"B PDF-417 471 128 XD 3 YD 4 C 1 S 0" & vbNewLine & _
        'txtItem.Text & "%" & txtLot.Text & "%" & txtQty.Text & vbNewLine & _
        '"ENDPDF" & vbNewLine & _
        '"PRINT" & vbNewLine
        Dim barkod As String = _
        "! 0 200 200 268 1" & vbNewLine & _
        "PW 799" & vbNewLine & _
        "TONE 0" & vbNewLine & _
        "SPEED 3" & vbNewLine & _
        "ON-FEED IGNORE" & vbNewLine & _
        "NO-PACE" & vbNewLine & _
        "BAR-SENSE" & vbNewLine & _
        "BT 7 0 0" & vbNewLine & _
        "B 128 1 30 47 39 91 " & txtItem.Text & "%" & txtLot.Text & "%" & txtQty.Text & vbNewLine & _
        "T 5 0 465 62 " & "Lot : " & txtLot.Text & vbNewLine & _
        "T 5 0 232 61 " & "Olcu Birimi : " & ölcüBirimi & vbNewLine & _
        "T 5 0 40 62 " & "Miktar : " & txtQty.Text & vbNewLine & _
        "T 5 0 40 35 " & "Tanim : " & tanim & vbNewLine & _
        "T 5 0 40 9 " & "Malzeme : " & txtItem.Text & vbNewLine & _
        "PRINT" & vbNewLine

        Try
            'Open Connection
            Dim client As New System.Net.Sockets.TcpClient
            client.Connect(ipAddress, port)
            'Write String to Connection
            'Dim e1 As Encoding = Encoding.GetEncoding("ISO-8859-9")
            'Dim writer As New System.IO.StreamWriter(client.GetStream(), e1)
            Dim writer As New System.IO.StreamWriter(client.GetStream())
            'Dim byte_ As Byte()
            'byte_ = System.Text.Encoding.UTF7.GetBytes(barkod)
            'writer.Write(System.Text.Encoding.UTF7.GetString(byte_, 0, byte_.Length))
            writer.Write(barkod)
            writer.Flush()
            'Close Connection
            writer.Close()
            client.Close()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
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



   
End Class