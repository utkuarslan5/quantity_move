<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMiktarIlerlet
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtYer2 = New System.Windows.Forms.TextBox
        Me.lblYer2 = New System.Windows.Forms.Label
        Me.lblYer1 = New System.Windows.Forms.Label
        Me.txtLot = New System.Windows.Forms.TextBox
        Me.lblLot1 = New System.Windows.Forms.Label
        Me.lblQty = New System.Windows.Forms.Label
        Me.txtItem = New System.Windows.Forms.TextBox
        Me.btnClear = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnSend = New System.Windows.Forms.Button
        Me.lblItem = New System.Windows.Forms.Label
        Me.cmbBoxCikisYeri = New System.Windows.Forms.ComboBox
        Me.txtQty = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtBasvuru = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'txtYer2
        '
        Me.txtYer2.Location = New System.Drawing.Point(67, 103)
        Me.txtYer2.Name = "txtYer2"
        Me.txtYer2.Size = New System.Drawing.Size(154, 21)
        Me.txtYer2.TabIndex = 0
        Me.txtYer2.Tag = "1"
        '
        'lblYer2
        '
        Me.lblYer2.Location = New System.Drawing.Point(7, 105)
        Me.lblYer2.Name = "lblYer2"
        Me.lblYer2.Size = New System.Drawing.Size(55, 20)
        Me.lblYer2.Text = "Giriş Yeri"
        '
        'lblYer1
        '
        Me.lblYer1.Location = New System.Drawing.Point(5, 78)
        Me.lblYer1.Name = "lblYer1"
        Me.lblYer1.Size = New System.Drawing.Size(55, 20)
        Me.lblYer1.Text = "Çıkış Yeri"
        '
        'txtLot
        '
        Me.txtLot.Location = New System.Drawing.Point(67, 130)
        Me.txtLot.Name = "txtLot"
        Me.txtLot.ReadOnly = True
        Me.txtLot.Size = New System.Drawing.Size(154, 21)
        Me.txtLot.TabIndex = 4
        Me.txtLot.Tag = "1"
        '
        'lblLot1
        '
        Me.lblLot1.Location = New System.Drawing.Point(6, 133)
        Me.lblLot1.Name = "lblLot1"
        Me.lblLot1.Size = New System.Drawing.Size(55, 20)
        Me.lblLot1.Text = "Lot"
        '
        'lblQty
        '
        Me.lblQty.Location = New System.Drawing.Point(5, 50)
        Me.lblQty.Name = "lblQty"
        Me.lblQty.Size = New System.Drawing.Size(55, 20)
        Me.lblQty.Text = "Miktar"
        '
        'txtItem
        '
        Me.txtItem.Location = New System.Drawing.Point(67, 21)
        Me.txtItem.Name = "txtItem"
        Me.txtItem.Size = New System.Drawing.Size(152, 21)
        Me.txtItem.TabIndex = 8
        Me.txtItem.Tag = "1"
        '
        'btnClear
        '
        Me.btnClear.Location = New System.Drawing.Point(16, 207)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(56, 30)
        Me.btnClear.TabIndex = 0
        Me.btnClear.Text = "Temizle"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(90, 207)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(56, 30)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Kapat"
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(164, 207)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(56, 30)
        Me.btnSend.TabIndex = 2
        Me.btnSend.Text = "Kaydet"
        '
        'lblItem
        '
        Me.lblItem.Location = New System.Drawing.Point(6, 22)
        Me.lblItem.Name = "lblItem"
        Me.lblItem.Size = New System.Drawing.Size(55, 20)
        Me.lblItem.Text = "Malzeme"
        '
        'cmbBoxCikisYeri
        '
        Me.cmbBoxCikisYeri.Location = New System.Drawing.Point(67, 75)
        Me.cmbBoxCikisYeri.Name = "cmbBoxCikisYeri"
        Me.cmbBoxCikisYeri.Size = New System.Drawing.Size(153, 22)
        Me.cmbBoxCikisYeri.TabIndex = 75
        Me.cmbBoxCikisYeri.Tag = "1"
        '
        'txtQty
        '
        Me.txtQty.Location = New System.Drawing.Point(67, 48)
        Me.txtQty.Name = "txtQty"
        Me.txtQty.Size = New System.Drawing.Size(152, 21)
        Me.txtQty.TabIndex = 8
        Me.txtQty.Tag = "1"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(6, 159)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 20)
        Me.Label1.Text = "Başvuru"
        '
        'txtBasvuru
        '
        Me.txtBasvuru.Location = New System.Drawing.Point(67, 157)
        Me.txtBasvuru.Name = "txtBasvuru"
        Me.txtBasvuru.Size = New System.Drawing.Size(154, 21)
        Me.txtBasvuru.TabIndex = 83
        '
        'frmMiktarIlerlet
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(239, 290)
        Me.Controls.Add(Me.txtBasvuru)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmbBoxCikisYeri)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSend)
        Me.Controls.Add(Me.txtYer2)
        Me.Controls.Add(Me.lblYer2)
        Me.Controls.Add(Me.lblQty)
        Me.Controls.Add(Me.lblItem)
        Me.Controls.Add(Me.lblYer1)
        Me.Controls.Add(Me.txtQty)
        Me.Controls.Add(Me.txtItem)
        Me.Controls.Add(Me.txtLot)
        Me.Controls.Add(Me.lblLot1)
        Me.Name = "frmMiktarIlerlet"
        Me.Text = "Miktar Ilerlet"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents txtYer2 As System.Windows.Forms.TextBox
    Friend WithEvents lblYer2 As System.Windows.Forms.Label
    Friend WithEvents lblYer1 As System.Windows.Forms.Label
    Friend WithEvents txtLot As System.Windows.Forms.TextBox
    Friend WithEvents lblLot1 As System.Windows.Forms.Label
    Friend WithEvents lblQty As System.Windows.Forms.Label
    Friend WithEvents txtItem As System.Windows.Forms.TextBox
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnSend As System.Windows.Forms.Button
    Friend WithEvents lblItem As System.Windows.Forms.Label
    Friend WithEvents cmbBoxCikisYeri As System.Windows.Forms.ComboBox
    Friend WithEvents txtQty As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtBasvuru As System.Windows.Forms.TextBox
End Class
