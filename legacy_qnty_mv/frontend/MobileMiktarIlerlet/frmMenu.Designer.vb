<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmMenu
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
        Me.btnMiktarIlerlet = New System.Windows.Forms.Button
        Me.btnExit = New System.Windows.Forms.Button
        Me.btnUretimeCikis = New System.Windows.Forms.Button
        Me.btnUretimdenGiris = New System.Windows.Forms.Button
        Me.btnRafaTransfer = New System.Windows.Forms.Button
        Me.btnSevkeTransfer = New System.Windows.Forms.Button
        Me.btnYuzdeYuzKontrol = New System.Windows.Forms.Button
        Me.btnSorgula = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'btnMiktarIlerlet
        '
        Me.btnMiktarIlerlet.Location = New System.Drawing.Point(36, 19)
        Me.btnMiktarIlerlet.Name = "btnMiktarIlerlet"
        Me.btnMiktarIlerlet.Size = New System.Drawing.Size(169, 20)
        Me.btnMiktarIlerlet.TabIndex = 0
        Me.btnMiktarIlerlet.Text = "Miktar İlerlet"
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(36, 243)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(169, 20)
        Me.btnExit.TabIndex = 30
        Me.btnExit.Text = "Çıkış"
        '
        'btnUretimeCikis
        '
        Me.btnUretimeCikis.Location = New System.Drawing.Point(36, 147)
        Me.btnUretimeCikis.Name = "btnUretimeCikis"
        Me.btnUretimeCikis.Size = New System.Drawing.Size(169, 20)
        Me.btnUretimeCikis.TabIndex = 20
        Me.btnUretimeCikis.Text = "Üretime Çıkış"
        Me.btnUretimeCikis.Visible = False
        '
        'btnUretimdenGiris
        '
        Me.btnUretimdenGiris.Location = New System.Drawing.Point(36, 115)
        Me.btnUretimdenGiris.Name = "btnUretimdenGiris"
        Me.btnUretimdenGiris.Size = New System.Drawing.Size(169, 20)
        Me.btnUretimdenGiris.TabIndex = 15
        Me.btnUretimdenGiris.Text = "Üretimden Giriş"
        Me.btnUretimdenGiris.Visible = False
        '
        'btnRafaTransfer
        '
        Me.btnRafaTransfer.Location = New System.Drawing.Point(36, 51)
        Me.btnRafaTransfer.Name = "btnRafaTransfer"
        Me.btnRafaTransfer.Size = New System.Drawing.Size(169, 20)
        Me.btnRafaTransfer.TabIndex = 5
        Me.btnRafaTransfer.Text = "Rafa Transfer"
        Me.btnRafaTransfer.Visible = False
        '
        'btnSevkeTransfer
        '
        Me.btnSevkeTransfer.Location = New System.Drawing.Point(36, 83)
        Me.btnSevkeTransfer.Name = "btnSevkeTransfer"
        Me.btnSevkeTransfer.Size = New System.Drawing.Size(169, 20)
        Me.btnSevkeTransfer.TabIndex = 10
        Me.btnSevkeTransfer.Text = "Sevke Transfer"
        Me.btnSevkeTransfer.Visible = False
        '
        'btnYuzdeYuzKontrol
        '
        Me.btnYuzdeYuzKontrol.Location = New System.Drawing.Point(36, 179)
        Me.btnYuzdeYuzKontrol.Name = "btnYuzdeYuzKontrol"
        Me.btnYuzdeYuzKontrol.Size = New System.Drawing.Size(169, 20)
        Me.btnYuzdeYuzKontrol.TabIndex = 25
        Me.btnYuzdeYuzKontrol.Text = "%100 Kontrol"
        Me.btnYuzdeYuzKontrol.Visible = False
        '
        'btnSorgula
        '
        Me.btnSorgula.Location = New System.Drawing.Point(36, 212)
        Me.btnSorgula.Name = "btnSorgula"
        Me.btnSorgula.Size = New System.Drawing.Size(169, 20)
        Me.btnSorgula.TabIndex = 31
        Me.btnSorgula.Text = "Stok Sorgulama"
        Me.btnSorgula.Visible = False
        '
        'frmMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.Controls.Add(Me.btnSorgula)
        Me.Controls.Add(Me.btnYuzdeYuzKontrol)
        Me.Controls.Add(Me.btnSevkeTransfer)
        Me.Controls.Add(Me.btnRafaTransfer)
        Me.Controls.Add(Me.btnUretimdenGiris)
        Me.Controls.Add(Me.btnUretimeCikis)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.btnMiktarIlerlet)
        Me.KeyPreview = True
        Me.Name = "frmMenu"
        Me.Text = "Hareket Girişleri Menü"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnMiktarIlerlet As System.Windows.Forms.Button
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents btnUretimeCikis As System.Windows.Forms.Button
    Friend WithEvents btnUretimdenGiris As System.Windows.Forms.Button
    Friend WithEvents btnRafaTransfer As System.Windows.Forms.Button
    Friend WithEvents btnSevkeTransfer As System.Windows.Forms.Button
    Friend WithEvents btnYuzdeYuzKontrol As System.Windows.Forms.Button
    Friend WithEvents btnSorgula As System.Windows.Forms.Button
End Class
