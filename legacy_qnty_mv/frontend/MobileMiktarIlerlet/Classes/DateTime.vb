'Imports System
'Imports System.DateTime
'Imports AsVol
Public NotInheritable Class DateTime

    'Private Sub New()
    'End Sub

    'Public Shared ReadOnly Property Now() As Date
    '  Get
    '    Return System.DateTime.Now
    '  End Get
    'End Property

    Public Shared ReadOnly Property Now() As Date
        Get

            'Dim db As New Core.Data(ConnectionStrSQL) 'Sql connection
            Dim db As New Mobile.wsGeneral
            db.Url = ReadConfig("path")
            Dim sStr As String

            'Dim wsPwsDateTime As New Core.Data(ConnectionStrSQL) 'wsPws.wsPws
            'Dim cslConnectionValues As New ConnectionValues
            'cslConnectionValues.Path = StartupPath()  '"\Program Files\PwsMobileMenu"
            'cslConnectionValues.ReadConnectionValues()
            'GetUserInformation()

            'WEB SERVÝS URL'si
            'wsPwsDateTime.Url = cslConnectionValues.WebSrvServer   '"http://192.168.20.137/PwsWs/wsPwms.asmx"

            'Dim sStr As String
            Dim ServerDate As Date
            sStr = " select getdate() "
            ServerDate = CDate(db.RunSqlDs(sStr, "whse", Mobile.ProviderTypes.SqlClient).Tables(0).Rows(0)(0).ToString) 'CDate(db.RunSql(sStr, True))
            'ServerDate = CDate("2009-10-17 16:52:37.763")

            Return ServerDate

        End Get
    End Property

    'Protected Overrides Sub 

    'Dim _Date As Date
    'Public Overloads Overrides Property Now() As Date
    '  Get
    '    Return _Date
    '  End Get
    '  Set(ByVal value As Date)
    '    value = _Date
    '  End Set
    'End Property

    'Public Overloads Overrides Sub Remove(ByVal key As String)

    'End Sub


End Class



