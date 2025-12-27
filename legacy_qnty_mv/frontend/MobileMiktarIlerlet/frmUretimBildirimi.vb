Imports System.Windows.Forms
Imports System.Threading
Imports System.IO
Imports System.Globalization
Imports System.Data


Public Class frmUretimBildirimi
    Dim dt As New DataTable
    Dim dtOper As New DataTable
    Dim dtReason As New DataTable
    Dim sBeforeOper As String
    Dim sNextOper As String
    Dim dMiktar As Decimal
    Dim sStr As String = String.Empty
    Dim reasonCode As String
    Dim srv As New Mobile.wsGeneral

    

    Private Sub cmbOperNo_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbOperNo.TextChanged
        Dim sFilter As String
        Dim foundRows() As DataRow = Nothing

        txtYer.Text = cmbOperNo.Text

        If dtOper.Rows.Count > 0 Then
            sFilter = "job='" & txtJob.Text & "' And " & " Oper_num=" & IIf(cmbOperNo.Text = "", 0, cmbOperNo.Text)
            foundRows = dtOper.Select(sFilter)
            If foundRows.Length > 0 Then
                sBeforeOper = foundRows(0).ItemArray(6).ToString
                sNextOper = foundRows(0).ItemArray(7).ToString
                dMiktar = foundRows(0).ItemArray(5).ToString
                txtYer.Text = foundRows(0).ItemArray(8).ToString
            Else
                sBeforeOper = String.Empty
                sNextOper = String.Empty
                dMiktar = 0
                txtYer.Text = String.Empty
            End If
        Else
            sBeforeOper = String.Empty
            sNextOper = String.Empty
            dMiktar = 0
            txtYer.Text = String.Empty
        End If

    End Sub

    Private Sub btnTemizle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTemizle.Click

        txtItem.Text = String.Empty
        txtJob.Text = String.Empty
        txtYer.Text = String.Empty
        txtLot.Text = String.Empty
        cmbOperNo.Text = String.Empty
        txtQty.Text = 0
        cmbOperNo.DataSource = Nothing
        sBeforeOper = String.Empty
        sNextOper = String.Empty
        dMiktar = 0
        txtItem.Focus()

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Close()
        frmMenu.Visible = True
    End Sub

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Try

            If CInt(txtQty.Text) = 0 Then
                MessageBox.Show("Miktar 0'dan farklı olmalıdır.")
                Exit Sub
            End If

            If txtYer.Text = String.Empty Then Throw New Exception("Yer Tanımsız")
            If txtLot.Text = String.Empty Then Throw New Exception("Lot Tanımsız")
            If txtItem.Text = String.Empty Then Throw New Exception("Malzeme Tanımsız")
            If txtQty.Text > dMiktar Then Throw New Exception("Miktar Gerekenden Büyük")
            'If txtIrsaliyeNo.Text = String.Empty Then Throw New Exception("Irsaliye No Giriniz")

            Dim sorgu As String = "SELECT * FROM job WHERE job = '" & txtJob.Text & "' AND (qty_complete > qty_released OR stat = 'C')"

            If (srv.RunSqlDs(sorgu, "job", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count > 0) Then
                MessageBox.Show("İş emri / işlem kapalıdır.", "Uyarı")
                Exit Sub
            End If

            sorgu = "SELECT * FROM jobroute WHERE complete = 1 AND job = '" & txtJob.Text & "'"
            If (srv.RunSqlDs(sorgu, "jobroute", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count > 0) Then
                MessageBox.Show("İş emri / işlem kapalıdır.", "Uyarı")
                Exit Sub
            End If

            sorgu = "SELECT * FROM item WHERE lot_tracked <> 1 AND item ='" & txtItem.Text & "'"
            If (srv.RunSqlDs(sorgu, "item", Mobile.ProviderTypes.SqlClient).Tables(0).Rows.Count > 0) Then
                MessageBox.Show("Malzeme lot denetimsizdir.", "Uyarı")
                Exit Sub
            End If


            sorgu = "EXEC TR_Stok_Kontrol '" & txtItem.Text & "', '" & _
                txtLot.Text & "', '" & txtYer.Text & "', '" & ReadConfig("DefaultWhse") & "'"

            srv.RunSqlSp(sorgu, Mobile.ProviderTypes.SqlClient)

            sStr = " Exec TR_Bilesen_StokSP " & _
                    "@job='" & txtJob.Text & "'," & _
                    "@Oper_num='" & cmbOperNo.Text & "'," & _
                    "@qty=" & txtQty.Text

            srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            sStr = " Exec Tr_UretimBildirimi " & _
                        "@Job='" & txtJob.Text & "'," & _
                        "@Oper_num='" & cmbOperNo.Text & "'," & _
                        "@Loc='" & txtYer.Text & "'," & _
                        "@Lot='" & txtLot.Text & "'," & _
                        "@qty_complete=" & txtQty.Text & "," & _
                        "@NextOper=" & sNextOper

          
            srv.RunSqlSp(sStr, Mobile.ProviderTypes.SqlClient)

            sStr = "UPDATE matltran" & _
                        " SET document_num='" & txtIrsaliyeNo.Text & _
                        "',Uf_user='" & frmKullaniciGirisi.kullanici & "'" & _
                    " WHERE trans_type + ref_type='FJ'" & _
                        " AND ref_num='" & txtJob.Text & _
                        "' AND document_num IS NULL" & _
                        " AND ref_release='" & cmbOperNo.Text & "'"

            srv.RunSql(sStr, True, Mobile.ProviderTypes.SqlClient)

            'btnTemizle_Click(Me, e)
            txtItem.Text = String.Empty
            txtJob.Text = String.Empty
            txtYer.Text = String.Empty
            txtLot.Text = String.Empty
            txtIrsaliyeNo.Text = String.Empty
            cmbOperNo.Text = String.Empty
            txtQty.Text = 0
            cmbOperNo.DataSource = Nothing
            sBeforeOper = String.Empty
            sNextOper = String.Empty
            dMiktar = 0
            txtItem.Focus()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub frmUretimBildirimi_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        srv.Url = ReadConfig("path")
        Try
            txtItem.Focus()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub txtItem_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtItem.KeyPress
        Dim Malzeme As String = String.Empty
        Dim Isemri As String = String.Empty

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            txtItem.Text = txtItem.Text.Replace(".", "/")
            txtItem.Text = txtItem.Text.Replace("ç", ".")
            txtItem.Text = txtItem.Text.Replace("Ç", ".")
            txtItem.Text = txtItem.Text.Replace("ö", ",")
            txtItem.Text = txtItem.Text.Replace("Ö", ",")
            txtItem.Text = txtItem.Text.Replace("*", "-")

            Cursor.Current = Cursors.WaitCursor

            Try
                If txtItem.Text.Trim.Length > 0 Then

                    If txtItem.Text.IndexOf("%") <> -1 Then

                        Malzeme = txtItem.Text.Split("%")(0)
                        Isemri = txtItem.Text.Split("%")(1)
                        txtItem.Text = Malzeme
                        txtJob.Text = Isemri
                        txtLot.Text = Isemri

                        sStr = "Exec Tr_Edi_Uretim_Bildirimi_Load @Job='" & Isemri & "'"
                        dtOper = srv.RunSqlDs(sStr, "Tr_Edi_Uretim_Bildirimi_Load", Mobile.ProviderTypes.SqlClient).Tables(0)

                        cmbOperNo.ValueMember = "oper_num"
                        cmbOperNo.DisplayMember = "oper_num"
                        cmbOperNo.DataSource = dtOper
                        cmbOperNo.Focus()
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