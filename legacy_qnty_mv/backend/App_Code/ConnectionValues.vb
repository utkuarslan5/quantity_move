Imports System.Xml
Public Class ConnectionValues

#Region "Enums"

  Enum enumConnectionType
    SqlClient = 1 'System.Data.SqlClient
    OleDb = 2 'System.Data.OleDb
    Odbc = 3 'System.Data.Odbc
    OracleClient = 4 'System.Data.OracleClient
  End Enum

#End Region

#Region "Properties"

  Dim _SqlClientConnectionString As String
  Public ReadOnly Property SqlClientConnectionString() As String
    Get
      Return _SqlClientConnectionString
    End Get
  End Property

  Dim _OleDbConnectionString As String
  Public ReadOnly Property OleDbConnectionString() As String
    Get
      Return _OleDbConnectionString
    End Get
  End Property

  'Dim _SqlConnectionStringPws As String
  'Public ReadOnly Property SqlConnectionStringPws() As String
  '  Get
  '    Return _SqlConnectionStringPws
  '  End Get
  'End Property

  'Dim _SqlConnectionStringPwms As String
  'Public ReadOnly Property SqlConnectionStringPwms() As String
  '  Get
  '    Return _SqlConnectionStringPwms
  '  End Get
  'End Property

  'Dim _SapConnectionString As String
  'Public ReadOnly Property SapConnectionString() As String
  '  Get
  '    Return _SapConnectionString
  '  End Get
  'End Property

#End Region

#Region "Method"

  Public Function ReadConnectionValues(ByVal ConnectionType As enumConnectionType) As String
    Try
      Dim XmlReader As New XmlTextReader(AppDomain.CurrentDomain.BaseDirectory.ToString + "Config.xml")
      Dim XmlDocument As New XmlDocument
      XmlDocument.Load(XmlReader)

      If ConnectionType = enumConnectionType.SqlClient Then
        _SqlClientConnectionString = XmlDocument.Item("root").Item("SqlClientConnectionString").InnerText
        Return _SqlClientConnectionString
      ElseIf ConnectionType = enumConnectionType.OleDb Then
        _OleDbConnectionString = XmlDocument.Item("root").Item("OleDbConnectionString").InnerText
        Return _OleDbConnectionString
        '  _SqlConnectionStringPws = XmlDocument.Item("root").Item("SqlConnectionStringPws").InnerText
        '  Return _SqlConnectionStringPws
        'ElseIf SqlConnectionType = enumSqlConnectionType.Pwms Then
        '  _SqlConnectionStringPwms = XmlDocument.Item("root").Item("SqlConnectionStringPwms").InnerText
        '  Return _SqlConnectionStringPwms
      End If

    Catch ex As Exception
      Throw ex
    End Try
  End Function

  'Public Function ReadSapConnectionValues() As String
  '  Try
  '    Dim XmlReader As New XmlTextReader(AppDomain.CurrentDomain.BaseDirectory.ToString + "Config.xml")
  '    Dim XmlDocument As New XmlDocument
  '    XmlDocument.Load(XmlReader)

  '    _SapConnectionString = XmlDocument.Item("root").Item("SapConnectionString").InnerText
  '    Return _SapConnectionString

  '  Catch ex As Exception
  '    Throw ex
  '  End Try
  'End Function

#End Region

End Class
