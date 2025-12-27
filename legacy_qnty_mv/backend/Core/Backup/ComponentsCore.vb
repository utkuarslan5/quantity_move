Imports System.Windows.Forms

Namespace Core
	Public Enum DisplayMode
		Insert
		Update
		Delete
    End Enum
    '
    Public Class ComponentsCore
        Public Shared Sub gsubClearForm(ByRef frm As Form)

            'Her Bir Kontrol Alýnýp Tipine Göre Temizlenecek
            For Each cnt As Control In frm.Controls

                If TypeOf (cnt) Is TextBox Then
                    cnt.Text = ""

                ElseIf TypeOf (cnt) Is ComboBox Then
                    Dim cmb As ComboBox
                    cmb = CType(cnt, ComboBox)

                    cmb.SelectedIndex = -1
                End If
            Next
        End Sub

    End Class

End Namespace
