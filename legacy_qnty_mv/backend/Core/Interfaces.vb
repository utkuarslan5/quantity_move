Imports System.Reflection
Imports System.IO


Public Class Eklenti
    '
    Public Shared Function EklentiBul(ByVal strYol As String, ByVal strInterface As String) As UygunEklenti()
        Dim Eklentiler As ArrayList = New ArrayList
        Dim strDLLler() As String, intIndex As Integer
        Dim objDLL As [Assembly]
        '
        'Tüm DLL'leri tarayýp yükle
        strDLLler = Directory.GetFileSystemEntries(strYol, "*.dll")
        '
        For intIndex = 0 To strDLLler.Length - 1
            Try
                objDLL = [Assembly].LoadFrom(strDLLler(intIndex))
                AssemblyIncele(objDLL, strInterface, Eklentiler)
            Catch e As Exception
                'Dll'i yüklemede hata oluþursa...
            End Try
        Next

        'Bulunan eklentileri döndür
        Dim Sonuclar(Eklentiler.Count - 1) As UygunEklenti

        If Eklentiler.Count <> 0 Then
            Eklentiler.CopyTo(Sonuclar)
            Return Sonuclar
        Else
            Return Nothing
        End If
    End Function
    '
    Private Shared Sub AssemblyIncele(ByVal objDLL As [Assembly], ByVal strInterface As String, ByVal Eklentiler As ArrayList)
        Dim objTip As Type
        Dim objInterface As Type
        Dim Eklenti As UygunEklenti

        'Tüm Dllleri tara
        For Each objTip In objDLL.GetTypes
            'Sadece public tiplerle ilgilen
            If objTip.IsPublic = True Then
                'Abstract synyflary dikkate alma

                If Not ((objTip.Attributes And TypeAttributes.Abstract) = TypeAttributes.Abstract) Then
                    'Belirlediðimiz Interface'i destekleyip desteklemediðini kontrol et
                    objInterface = objTip.GetInterface(strInterface, True)
                    '
                    If Not (objInterface Is Nothing) Then
                        'Destekliyor
                        Eklenti = New UygunEklenti
                        Eklenti.AssemblyYolu = objDLL.Location
                        Eklenti.ClassAdi = objTip.FullName
                        Eklentiler.Add(Eklenti)
                    End If
                End If
            End If
        Next
    End Sub

    Public Shared Function OrnekOlustur(ByVal Eklenti As UygunEklenti) As Object
        Dim objDLL As [Assembly]
        Dim objEklenti As Object

        Try
            'Dll'i yükle 
            objDLL = [Assembly].LoadFrom(Eklenti.AssemblyYolu)

            'Asýl iþi yapan CreateInstance metodunu kullarak sýnýftan örnek oluþtur
            objEklenti = objDLL.CreateInstance(Eklenti.ClassAdi)
        Catch e As Exception
            Return Nothing
        End Try

        Return objEklenti

    End Function

    Public Shared Function Form_Load(ByVal Eklenti As UygunEklenti, _
        ByVal FormAd As String) As Windows.Forms.Form

        Dim objDLL As [Assembly]
        Dim objEklenti As Object

        Try
            'Dll'i yükle 
            objDLL = [Assembly].LoadFrom(Eklenti.AssemblyYolu)

            'Asýl iþi yapan CreateInstance metodunu kullarak sýnýftan örnek oluþtur
            objEklenti = objDLL.CreateInstance(Eklenti.ClassAdi)
            '
            Dim frm As IEklenti
            '
            frm = CType(objEklenti, IEklenti)
            If (frm Is Nothing) Then
                Return Nothing
            Else
                Return frm
            End If
        Catch e As Exception
            Return Nothing
        End Try
    End Function
    '
    Public Shared Function Form_Load(ByVal Eklenti As UygunEklenti, _
        ByVal FormAd As String, _
        ByVal Show As Boolean) As Windows.Forms.Form

        If Show Then
            Dim objDLL As [Assembly]
            Dim objEklenti As Object

            Try
                'Dll'i yükle 
                objDLL = [Assembly].LoadFrom(Eklenti.AssemblyYolu)

                'Asýl iþi yapan CreateInstance metodunu kullarak sýnýftan örnek oluþtur
                objEklenti = objDLL.CreateInstance(Eklenti.ClassAdi)
                '
                Dim frm As IEklenti
                '
                frm = CType(objEklenti, IEklenti)
                If (frm Is Nothing) Then
                    Return Nothing
                Else
                    frm.Show(False)
                    Return frm
                End If
            Catch e As Exception
                Return Nothing
            End Try
        Else
            Form_Load(Eklenti, FormAd)
        End If

    End Function
End Class

Public Interface IEklenti
    '
    'Formlar isimli Nesne Eklenecek
    ReadOnly Property Formlar() As Formlar
    '
    'Sub Form_Load(ByVal FormAd As String)
    Property ConnectionString() As String
    Property ReportPaths() As String
    Shadows Sub Show(ByVal Dialog As Boolean)
End Interface
'
Public Structure UygunEklenti
    Public AssemblyYolu As String
    Public ClassAdi As String
End Structure

Public Structure Formlar
    Public Sub New(ByVal Ad As String, ByVal Tanim As String)
        '
        Me.Ad = Ad
        Me.Tanim = Tanim
    End Sub
    '
    Public Ad As String
    Public Tanim As String
End Structure

Public Interface IForm
    '
    Property ConnectionString() As String
    Property ReportPaths() As String
    Shadows Sub Show(ByVal Dialog As Boolean)
End Interface




