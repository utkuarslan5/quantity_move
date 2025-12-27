<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmUretimeCikis
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
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtAmbar = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtItem = New System.Windows.Forms.TextBox
        Me.txtLot = New System.Windows.Forms.TextBox
        Me.btnClear = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnSend = New System.Windows.Forms.Button
        Me.Label7 = New System.Windows.Forms.Label
        Me.btnX = New System.Windows.Forms.Button
        Me.txtSicil = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.txtKutudakiMiktar = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.txtKutuSayisi = New System.Windows.Forms.TextBox
        Me.txtQty = New System.Windows.Forms.TextBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.cmbBoxCikisTipi = New System.Windows.Forms.ComboBox
        Me.cmbBoxCikisYeri = New System.Windows.Forms.ComboBox
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(3, 7)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(84, 20)
        Me.Label1.Text = "Ambar"
        '
        'txtAmbar
        '
        Me.txtAmbar.Location = New System.Drawing.Point(86, 5)
        Me.txtAmbar.Name = "txtAmbar"
        Me.txtAmbar.ReadOnly = True
        Me.txtAmbar.Size = New System.Drawing.Size(126, 21)
        Me.txtAmbar.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(3, 53)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(100, 20)
        Me.Label3.Text = "Malzeme"
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(3, 77)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(100, 20)
        Me.Label4.Text = "Çıkış Yeri"
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(3, 99)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(100, 20)
        Me.Label6.Text = "Lot"
        '
        'txtItem
        '
        Me.txtItem.Location = New System.Drawing.Point(86, 51)
        Me.txtItem.Name = "txtItem"
        Me.txtItem.Size = New System.Drawing.Size(126, 21)
        Me.txtItem.TabIndex = 4
        '
        'txtLot
        '
        Me.txtLot.Location = New System.Drawing.Point(86, 98)
        Me.txtLot.Name = "txtLot"
        Me.txtLot.ReadOnly = True
        Me.txtLot.Size = New System.Drawing.Size(126, 21)
        Me.txtLot.TabIndex = 8
        '
        'btnClear
        '
        Me.btnClear.Location = New System.Drawing.Point(25, 225)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(56, 30)
        Me.btnClear.TabIndex = 80
        Me.btnClear.Text = "Temizle"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(94, 225)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(56, 30)
        Me.btnCancel.TabIndex = 81
        Me.btnCancel.Text = "Kapat"
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(163, 225)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(56, 30)
        Me.btnSend.TabIndex = 82
        Me.btnSend.Text = "Kaydet"
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(3, 170)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(78, 20)
        Me.Label7.Text = "Miktar"
        '
        'btnX
        '
        Me.btnX.Location = New System.Drawing.Point(212, 5)
        Me.btnX.Name = "btnX"
        Me.btnX.Size = New System.Drawing.Size(21, 21)
        Me.btnX.TabIndex = 128
        Me.btnX.Text = "x"
        '
        'txtSicil
        '
        Me.txtSicil.Enabled = False
        Me.txtSicil.Location = New System.Drawing.Point(86, 28)
        Me.txtSicil.Name = "txtSicil"
        Me.txtSicil.Size = New System.Drawing.Size(126, 21)
        Me.txtSicil.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(4, 28)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 20)
        Me.Label2.Text = "Sicil"
        '
        'Label5
        '
        Me.Label5.Location = New System.Drawing.Point(3, 123)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(100, 20)
        Me.Label5.Text = "Kutudaki Miktar"
        '
        'txtKutudakiMiktar
        '
        Me.txtKutudakiMiktar.Location = New System.Drawing.Point(109, 121)
        Me.txtKutudakiMiktar.Name = "txtKutudakiMiktar"
        Me.txtKutudakiMiktar.Size = New System.Drawing.Size(103, 21)
        Me.txtKutudakiMiktar.TabIndex = 5
        '
        'Label8
        '
        Me.Label8.Location = New System.Drawing.Point(3, 146)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(100, 20)
        Me.Label8.Text = "Kutu Sayısı"
        '
        'txtKutuSayisi
        '
        Me.txtKutuSayisi.Location = New System.Drawing.Point(109, 144)
        Me.txtKutuSayisi.Name = "txtKutuSayisi"
        Me.txtKutuSayisi.Size = New System.Drawing.Size(103, 21)
        Me.txtKutuSayisi.TabIndex = 6
        '
        'txtQty
        '
        Me.txtQty.Enabled = False
        Me.txtQty.Location = New System.Drawing.Point(86, 168)
        Me.txtQty.Name = "txtQty"
        Me.txtQty.Size = New System.Drawing.Size(126, 21)
        Me.txtQty.TabIndex = 7
        '
        'Label9
        '
        Me.Label9.Location = New System.Drawing.Point(3, 193)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(78, 20)
        Me.Label9.Text = "Çıkış Tipi"
        '
        'cmbBoxCikisTipi
        '
        Me.cmbBoxCikisTipi.DisplayMember = "loc"
        Me.cmbBoxCikisTipi.Items.Add("Üretime Çıkış")
        Me.cmbBoxCikisTipi.Items.Add("Çeşitli Çıkış")
        Me.cmbBoxCikisTipi.Location = New System.Drawing.Point(86, 193)
        Me.cmbBoxCikisTipi.Name = "cmbBoxCikisTipi"
        Me.cmbBoxCikisTipi.Size = New System.Drawing.Size(126, 22)
        Me.cmbBoxCikisTipi.TabIndex = 134
        Me.cmbBoxCikisTipi.Tag = "1"
        Me.cmbBoxCikisTipi.ValueMember = "loc"
        '
        'cmbBoxCikisYeri
        '
        Me.cmbBoxCikisYeri.DisplayMember = "loc"
        Me.cmbBoxCikisYeri.Location = New System.Drawing.Point(86, 74)
        Me.cmbBoxCikisYeri.Name = "cmbBoxCikisYeri"
        Me.cmbBoxCikisYeri.Size = New System.Drawing.Size(126, 22)
        Me.cmbBoxCikisYeri.TabIndex = 143
        Me.cmbBoxCikisYeri.Tag = "1"
        Me.cmbBoxCikisYeri.ValueMember = "loc"
        '
        'frmUretimeCikis
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.Controls.Add(Me.cmbBoxCikisYeri)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.cmbBoxCikisTipi)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.txtKutuSayisi)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.txtKutudakiMiktar)
        Me.Controls.Add(Me.txtSicil)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnX)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtQty)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSend)
        Me.Controls.Add(Me.txtLot)
        Me.Controls.Add(Me.txtItem)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtAmbar)
        Me.Controls.Add(Me.Label1)
        Me.Name = "frmUretimeCikis"
        Me.Text = "Üretime Çıkış"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtAmbar As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtItem As System.Windows.Forms.TextBox
    Friend WithEvents txtLot As System.Windows.Forms.TextBox
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnSend As System.Windows.Forms.Button
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents btnX As System.Windows.Forms.Button
    Friend WithEvents txtSicil As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtKutudakiMiktar As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtKutuSayisi As System.Windows.Forms.TextBox
    Friend WithEvents txtQty As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents cmbBoxCikisTipi As System.Windows.Forms.ComboBox
    Friend WithEvents cmbBoxCikisYeri As System.Windows.Forms.ComboBox
End Class
