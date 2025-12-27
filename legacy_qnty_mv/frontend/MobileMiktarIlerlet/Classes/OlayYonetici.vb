Imports System.IO
Imports System.Data

Public Class OlayYonetici
    '
    Private Shared yzc As StreamWriter
    Private Shared Function DosyaOlustur() As Boolean
        Try
            Dim dosya As String = ""
            Dim yol As String = "Hata\"
            '
            'Eðer Dizin Yoksa oluþturulacak
            If Not Directory.Exists("Hata") Then
                Directory.CreateDirectory("Hata")
            End If
            '
            dosya &= Now.ToString("ddMMyyHHmm") & ".txt"
            yzc = New StreamWriter(yol & dosya)
            '
            Return True
        Catch ex As Exception
            Return False

        End Try

    End Function
    '
    Private Shared Function ParametreleriOku(ByVal p As ArrayList) As String
        '
        Dim parametreler As String = " "
        Try
            '
            For Each p1 As SqlClient.SqlParameter In p
                parametreler &= " Adi : " & p1.ParameterName & " -> "
                parametreler &= " Degeri : " & p1.Value & vbNewLine
            Next
            '
            Return parametreler
        Catch ex As Exception
            Return ""
        End Try
        '
    End Function

    '
    Public Shared Sub HataYaz(ByVal hata As String, ByVal kaynak As String)
        '
        Try
            If DosyaOlustur() Then
                '
                yzc.Write("Kaynak : ")
                yzc.WriteLine(kaynak)
                '
                yzc.Write("Hata : ")
                yzc.WriteLine(hata)
                '
                yzc.Close()
                '
            End If

        Catch ex As Exception

        End Try

    End Sub
    '
    Public Shared Sub HataYaz(ByVal hata As String, _
        ByVal kaynak As String, _
        ByVal Sql As String)
        '
        Exit Sub
        Try
            If DosyaOlustur() Then
                '
                yzc.Write("Kaynak : ")
                yzc.WriteLine(kaynak)
                '
                yzc.Write("Hata : ")
                yzc.WriteLine(hata)
                '
                yzc.Write("Sql Cumlecigi : ")
                yzc.WriteLine(Sql)
                '
                yzc.Close()
                '
            End If

        Catch ex As Exception

        End Try
    End Sub
    '
    Public Shared Sub HataYaz(ByVal hata As String, _
        ByVal kaynak As String, _
        ByVal Sql As String, _
        ByVal parametreler As ArrayList)
        '
        Try
            If DosyaOlustur() Then
                '
                yzc.Write("Kaynak : ")
                yzc.WriteLine(kaynak)
                '
                yzc.Write("Hata : ")
                yzc.WriteLine(hata)
                '
                yzc.Write("Sql Cumlecigi : ")
                yzc.WriteLine(Sql)
                '
                yzc.WriteLine("Parametreler ve Degerleri : ")
                yzc.WriteLine(ParametreleriOku(parametreler))
                '
                yzc.Close()
                '
            End If

        Catch ex As Exception

        End Try
    End Sub
    '

End Class
