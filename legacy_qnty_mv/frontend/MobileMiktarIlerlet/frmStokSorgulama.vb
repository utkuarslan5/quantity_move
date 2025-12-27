Imports System.Data
Imports System.Net
Imports System.Net.Sockets
Imports System.Text.Encoding
Imports System.Text


Public Class frmStokSorgulama

    Dim dt As New DataTable
    Dim sStr As String = String.Empty
    Dim srv As New Mobile.wsGeneral
    Dim SeriNo As Integer = 0

    Private Sub txtItem_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtItem.KeyPress

        Dim Malzeme As String
        Malzeme = txtItem.Text

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            Try
                
                Dim dtTmp As New DataTable
                sStr = " select MALZEMEKODU,MALZEMETANIMI from TRM_LABELDB with (nolock) " & _
                       " where SeriNo=" & txtItem.Text.Trim
                dtTmp = srv.RunSqlDs(sStr, "TRM_LABELDB", Mobile.ProviderTypes.SqlClient).Tables(0)

                If Not dtTmp Is Nothing AndAlso dtTmp.Rows.Count > 0 Then
                    With dtTmp.Rows(0)
                        SeriNo = txtItem.Text.Trim
                        Malzeme = .Item("MALZEMEKODU").ToString
                        txtDescription.Text = .Item("MALZEMETANIMI").ToString
                    End With
                End If

                txtItem.Text = Malzeme

                sStr = "SELECT item,loc,lot,cast(qty_on_hand as decimal(18,2)) qty_on_hand, " & _
                " (select isnull(create_date, RecordDate) as FIFO from lot_mst lot " & _
                " WHERE lot.item = lot_loc.item AND lot.lot = lot_loc.lot) AS fifo " & _
                " FROM lot_loc_mst lot_loc WHERE qty_on_hand > 0 and item ='" & Malzeme & "'" & _
                " ORDER BY item,fifo "

                dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

                DataGrid1.DataSource = dt
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

        End If

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Hide()
        frmMenu.Visible = True
    End Sub
    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        txtItem.Text = String.Empty
        txtDescription.Text = String.Empty
        txtYer.Text = String.Empty
        DataGrid1.DataSource = Nothing
    End Sub

    Private Sub txtYer_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtYer.KeyPress

        'If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
        '    txtYer.Text = txtYer.Text.Replace("-", "*")
        'End If

        Dim Yer As String
        Yer = txtYer.Text

        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

            Try
                sStr = "SELECT loc FROM location_mst WHERE loc ='" & Yer & "'"
                dt = srv.RunSqlDs(sStr, "item", Mobile.ProviderTypes.SqlClient).Tables(0)

                If dt.Rows.Count = 0 Then
                    MessageBox.Show("Sistemde kayıtlı böyle bir lokasyon bulunamadı.")
                    txtItem.Text = String.Empty
                End If

                sStr = "SELECT item,loc,lot,cast(qty_on_hand as decimal(18,2)) qty_on_hand, " & _
                " (select isnull(create_date, RecordDate) as FIFO from lot_mst lot " & _
                " WHERE lot.item = lot_loc.item AND lot.lot = lot_loc.lot) AS fifo " & _
                " FROM lot_loc_mst lot_loc WHERE qty_on_hand > 0 and loc ='" & Yer & "'" & _
                " ORDER BY item,fifo "

                dt = srv.RunSqlDs(sStr, "lot_loc", Mobile.ProviderTypes.SqlClient).Tables(0)

                DataGrid1.DataSource = dt
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

        End If
    End Sub

    Private Sub frmStokSorgulama_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        srv.Url = ReadConfig("path")
    End Sub
End Class