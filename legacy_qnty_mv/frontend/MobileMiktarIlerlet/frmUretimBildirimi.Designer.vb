<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUretimBildirimi
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblYer = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblLot = New System.Windows.Forms.Label
        Me.txtYer = New System.Windows.Forms.TextBox
        Me.txtQty = New System.Windows.Forms.TextBox
        Me.lblQty = New System.Windows.Forms.Label
        Me.txtItem = New System.Windows.Forms.TextBox
        Me.lblItem = New System.Windows.Forms.Label
        Me.txtLot = New System.Windows.Forms.TextBox
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.btnTemizle = New System.Windows.Forms.Button
        Me.txtSayi = New System.Windows.Forms.TextBox
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnSend = New System.Windows.Forms.Button
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtIrsaliyeNo = New System.Windows.Forms.TextBox
        Me.cmbOperNo = New System.Windows.Forms.ComboBox
        Me.txtJob = New System.Windows.Forms.TextBox
        Me.lblJob = New System.Windows.Forms.Label
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblYer
        '
        Me.lblYer.Location = New System.Drawing.Point(3, 147)
        Me.lblYer.Name = "lblYer"
        Me.lblYer.Size = New System.Drawing.Size(64, 16)
        Me.lblYer.Text = "Yer"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(3, 62)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 17)
        Me.Label1.Text = "Oper. No"
        '
        'lblLot
        '
        Me.lblLot.Location = New System.Drawing.Point(3, 116)
        Me.lblLot.Name = "lblLot"
        Me.lblLot.Size = New System.Drawing.Size(64, 20)
        Me.lblLot.Text = "Lot"
        '
        'txtYer
        '
        Me.txtYer.Location = New System.Drawing.Point(87, 142)
        Me.txtYer.MaxLength = 15
        Me.txtYer.Name = "txtYer"
        Me.txtYer.ReadOnly = True
        Me.txtYer.Size = New System.Drawing.Size(143, 21)
        Me.txtYer.TabIndex = 5
        '
        'txtQty
        '
        Me.txtQty.Location = New System.Drawing.Point(87, 88)
        Me.txtQty.MaxLength = 6
        Me.txtQty.Name = "txtQty"
        Me.txtQty.Size = New System.Drawing.Size(143, 21)
        Me.txtQty.TabIndex = 3
        Me.txtQty.Text = "0"
        '
        'lblQty
        '
        Me.lblQty.Location = New System.Drawing.Point(3, 88)
        Me.lblQty.Name = "lblQty"
        Me.lblQty.Size = New System.Drawing.Size(82, 21)
        Me.lblQty.Text = "Miktar"
        '
        'txtItem
        '
        Me.txtItem.Location = New System.Drawing.Point(87, 10)
        Me.txtItem.Name = "txtItem"
        Me.txtItem.Size = New System.Drawing.Size(143, 21)
        Me.txtItem.TabIndex = 0
        '
        'lblItem
        '
        Me.lblItem.Location = New System.Drawing.Point(3, 11)
        Me.lblItem.Name = "lblItem"
        Me.lblItem.Size = New System.Drawing.Size(72, 20)
        Me.lblItem.Text = "Malzeme"
        '
        'txtLot
        '
        Me.txtLot.Location = New System.Drawing.Point(87, 115)
        Me.txtLot.Name = "txtLot"
        Me.txtLot.ReadOnly = True
        Me.txtLot.Size = New System.Drawing.Size(143, 21)
        Me.txtLot.TabIndex = 4
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.btnTemizle)
        Me.Panel2.Controls.Add(Me.txtSayi)
        Me.Panel2.Controls.Add(Me.btnCancel)
        Me.Panel2.Controls.Add(Me.btnSend)
        Me.Panel2.Location = New System.Drawing.Point(3, 215)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(230, 27)
        '
        'btnTemizle
        '
        Me.btnTemizle.Location = New System.Drawing.Point(3, 3)
        Me.btnTemizle.Name = "btnTemizle"
        Me.btnTemizle.Size = New System.Drawing.Size(60, 20)
        Me.btnTemizle.TabIndex = 3
        Me.btnTemizle.Text = "Temizle"
        '
        'txtSayi
        '
        Me.txtSayi.Enabled = False
        Me.txtSayi.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.txtSayi.ForeColor = System.Drawing.Color.Red
        Me.txtSayi.Location = New System.Drawing.Point(68, 4)
        Me.txtSayi.Name = "txtSayi"
        Me.txtSayi.Size = New System.Drawing.Size(40, 19)
        Me.txtSayi.TabIndex = 2
        Me.txtSayi.Text = "0"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(115, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(48, 20)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Kapat"
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(168, 3)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(56, 20)
        Me.btnSend.TabIndex = 0
        Me.btnSend.Text = "Gönder"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.txtIrsaliyeNo)
        Me.Panel1.Controls.Add(Me.cmbOperNo)
        Me.Panel1.Controls.Add(Me.lblLot)
        Me.Panel1.Controls.Add(Me.lblYer)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.txtLot)
        Me.Panel1.Controls.Add(Me.txtQty)
        Me.Panel1.Controls.Add(Me.txtYer)
        Me.Panel1.Controls.Add(Me.lblQty)
        Me.Panel1.Controls.Add(Me.txtItem)
        Me.Panel1.Controls.Add(Me.lblItem)
        Me.Panel1.Controls.Add(Me.txtJob)
        Me.Panel1.Controls.Add(Me.lblJob)
        Me.Panel1.Location = New System.Drawing.Point(3, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(233, 206)
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(3, 174)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(64, 16)
        Me.Label2.Text = "İrsaliye No:"
        '
        'txtIrsaliyeNo
        '
        Me.txtIrsaliyeNo.Location = New System.Drawing.Point(87, 169)
        Me.txtIrsaliyeNo.MaxLength = 12
        Me.txtIrsaliyeNo.Name = "txtIrsaliyeNo"
        Me.txtIrsaliyeNo.Size = New System.Drawing.Size(143, 21)
        Me.txtIrsaliyeNo.TabIndex = 6
        '
        'cmbOperNo
        '
        Me.cmbOperNo.DisplayMember = "oper_num"
        Me.cmbOperNo.Location = New System.Drawing.Point(87, 62)
        Me.cmbOperNo.Name = "cmbOperNo"
        Me.cmbOperNo.Size = New System.Drawing.Size(106, 22)
        Me.cmbOperNo.TabIndex = 2
        Me.cmbOperNo.ValueMember = "oper_num"
        '
        'txtJob
        '
        Me.txtJob.Location = New System.Drawing.Point(87, 36)
        Me.txtJob.Name = "txtJob"
        Me.txtJob.ReadOnly = True
        Me.txtJob.Size = New System.Drawing.Size(112, 21)
        Me.txtJob.TabIndex = 1
        '
        'lblJob
        '
        Me.lblJob.Location = New System.Drawing.Point(3, 37)
        Me.lblJob.Name = "lblJob"
        Me.lblJob.Size = New System.Drawing.Size(72, 20)
        Me.lblJob.Text = "İş emri No"
        '
        'frmUretimBildirimi
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(239, 290)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmUretimBildirimi"
        Me.Text = "Üretim Bildirimi"
        Me.Panel2.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblYer As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblLot As System.Windows.Forms.Label
    Friend WithEvents txtYer As System.Windows.Forms.TextBox
    Friend WithEvents txtQty As System.Windows.Forms.TextBox
    Friend WithEvents lblQty As System.Windows.Forms.Label
    Friend WithEvents txtItem As System.Windows.Forms.TextBox
    Friend WithEvents lblItem As System.Windows.Forms.Label
    Friend WithEvents txtLot As System.Windows.Forms.TextBox
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents btnTemizle As System.Windows.Forms.Button
    Friend WithEvents txtSayi As System.Windows.Forms.TextBox
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnSend As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents txtJob As System.Windows.Forms.TextBox
    Friend WithEvents lblJob As System.Windows.Forms.Label
    Friend WithEvents cmbOperNo As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtIrsaliyeNo As System.Windows.Forms.TextBox
End Class
