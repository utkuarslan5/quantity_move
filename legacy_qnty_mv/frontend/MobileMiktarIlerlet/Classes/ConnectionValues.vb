Imports System.Xml
Public Class ConnectionValues

#Region "Enums"

    Enum enumSqlConnectionType
        Eti = 0
        'Pwms = 1
        'Pps = 2
        'Msds = 3
    End Enum

#End Region

#Region "Properties"

    Public Shared _SqlConnectionStringEti As String
    Public ReadOnly Property SqlConnectionStringEti() As String
        Get
            Return _SqlConnectionStringEti
        End Get
    End Property

    Dim _WebSrvServer As String
    Public ReadOnly Property WebSrvServer() As String
        Get
            Return _WebSrvServer
        End Get
    End Property

    'Public Shared _Path As String
    'Public WriteOnly Property Path() As String
    '    Set(ByVal value As String)
    '        _Path = value
    '    End Set
    'End Property

#End Region

#Region "Method"

    Public Shared Function ReadConnectionValues(ByVal SqlConnectionType As enumSqlConnectionType) As String
        Try

            _Path = StartupPath()

            'If _Path = "" Then Throw New Exception("Lütfen Config.xml dosyasının bulunduğu yolu giriniz !")

            Dim XmlReader As New XmlTextReader(_Path + "\" + "Config.xml")
            Dim XmlDocument As New XmlDocument
            XmlDocument.Load(XmlReader)

            If SqlConnectionType = enumSqlConnectionType.Eti Then
                _SqlConnectionStringEti = XmlDocument.Item("root").Item("SqlConnectionStringEti").InnerText
                Return _SqlConnectionStringEti
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Sub ReadConnectionValues()
        Exit Sub
        Try

            If _Path = "" Then Throw New Exception("Lütfen Config.xml dosyasının bulunduğu yolu giriniz !")

            Dim XmlReader As New XmlTextReader(_Path + "\" + "Config.xml")
            Dim XmlDocument As New XmlDocument
            XmlDocument.Load(XmlReader)

            _WebSrvServer = XmlDocument.Item("root").Item("WebSrvServer").InnerText

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#End Region

End Class
