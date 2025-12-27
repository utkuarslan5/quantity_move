Public Class ReturnValue
    '
    Private mOthers As New Hashtable
    Private mMessages As New ArrayList
    '
    Public ReturnValue As Boolean
    '
    Public Sub Add(ByVal msg As String)
        mMessages.Add(msg)
    End Sub
    '
    Default Public Property Items(ByVal index As Integer) As String
        Get
            Return mMessages(index)
        End Get
        Set(ByVal Value As String)
            mMessages(index) = Value
        End Set
    End Property
    '
    Public Function GetMessages() As String
        '
        Dim returnMsg As String
        For Each msg As String In mMessages
            '
            returnMsg &= msg & vbNewLine
            '
        Next
        '
        Return returnMsg
    End Function
    '
    Public Sub AddOther(ByVal key As String, ByVal value As String)
        '
        '10.10 tarinde eklendi
        If mOthers.Contains(key) Then
            mOthers.Remove(key)
        End If
        '
        mOthers.Add(key, value)
        '
    End Sub
    '
    Public Property Others(ByVal key As String) As String
        Get
            Return mOthers.Item(key)
        End Get
        Set(ByVal Value As String)
            mOthers.Item(key) = Value
        End Set
    End Property
    '
End Class
