Public Class frmMenu

   

    Private Sub frmMenu_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyCode = System.Windows.Forms.Keys.Up) Then
            'Rocker Up
            'Up
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Down) Then
            'Rocker Down
            'Down
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Left) Then
            'Left
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Right) Then
            'Right
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Enter) Then
            'Enter
        End If

    End Sub

    Private Sub btnUretimeCikis_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUretimeCikis.Click
        Me.Hide()
        frmUretimeCikis.Show()
    End Sub

    Private Sub btnUretimdenGiris_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUretimdenGiris.Click
        Me.Hide()
        frmUretimdenGiris.Show()
    End Sub

    Private Sub btnRafaTransfer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRafaTransfer.Click
        Me.Hide()
        frmRafaTransfer.Show()
    End Sub

    Private Sub btnSevkeTransfer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSevkeTransfer.Click
        Me.Hide()
        frmSevkeTransfer.Show()
    End Sub

    Private Sub btnYuzdeYuzKontrol_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnYuzdeYuzKontrol.Click
        Me.Hide()
        frmYuzdeYuzKontrol.Show()
    End Sub

    Private Sub btnSorgula_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSorgula.Click
        Me.Hide()
        frmStokSorgulama.Show()
    End Sub

    Private Sub btnMiktarIlerlet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMiktarIlerlet.Click
        Me.Hide()
        frmYeniMiktarIlerlet.Show()
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Application.Exit()
    End Sub

    Private Sub frmMenu_Closed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        Application.Exit()
    End Sub

End Class