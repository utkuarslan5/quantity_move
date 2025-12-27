Imports System
Imports System.Data
Imports System.Data.SqlClient



Namespace Core
    Public Class Data

        Private conn As New SqlConnection

        Private tran As SqlTransaction

        Private bTransaction As Boolean = False

        Public Result As New ReturnValue
        '
#Region " Properties "
        '
        Public ReadOnly Property Transaction() As Boolean
            Get
                Return bTransaction
            End Get
        End Property
#End Region
        '
#Region " Private Methods "
        '
        '*******************************************************************************
        'MethodName :BuildQueryCommand
        'WhatDoes   :Verilen Parametreler ve Procismini kullanarak 
        '            Bir Command Oluþturur 
        'Inputs     :sProcName  :Proc Ýsmi
        '            param      :sqlParameterler
        'Outputs    :
        'Return     :sqlCommand
        '*******************************************************************************
        Private Function BuildQueryCommand(ByVal sProcName As String, _
                ByVal parameter As ArrayList) As SqlCommand

            Try

                '
                'Command Oluþturuluyor
                Dim cmd As New SqlCommand(sProcName, conn)

                cmd.CommandTimeout = 0
                cmd.CommandType = CommandType.StoredProcedure
                '
                If bTransaction Then

                    cmd.Transaction = tran

                End If
                '
                'Parametreler Ekleniyor
                For Each param As SqlParameter In parameter

                    cmd.Parameters.Add(param)

                Next

                Return cmd

            Catch ex As Exception
                Throw ex

            End Try

        End Function
        '
        '*******************************************************************************
        'MethodName :BuildQueryCommand
        'WhatDoes   :Verilen Procismini kullanarak 
        '            Bir Command Oluþturur 
        'Inputs     :sProcName  :Proc Ýsmi
        'Outputs    :
        'Return     :sqlCommand
        '*******************************************************************************
        Private Function BuildQueryCommand(ByVal sProcName As String) As SqlCommand

            Try

                '
                'Command Oluþturuluyor
                Dim cmd As New SqlCommand(sProcName, conn)

                cmd.CommandTimeout = 0
                cmd.CommandType = CommandType.StoredProcedure
                '
                If bTransaction Then

                    cmd.Transaction = tran

                End If

                Return cmd

            Catch ex As Exception
                Throw ex

            End Try

        End Function
        '
        '*******************************************************************************
        'MethodName :BuildIntCommand
        'WhatDoes   :Verilen Parametreler ve Procismini kullanarak 
        '            Bir Command Oluþturur ve ReturnValue Ekler
        'Inputs     :sProcName  :Proc Ýsmi
        '            param      :sqlParameterler
        'Outputs    :
        'Return     :sqlCommand
        '*******************************************************************************
        Private Function BuildIntCommand(ByVal sProcName As String, _
                ByVal parameter As ArrayList) As SqlCommand

            Try

                Dim cmd As New SqlCommand

                cmd.CommandTimeout = 0
                'BuildQueryCommand 
                cmd = BuildQueryCommand(sProcName, parameter)

                'ReturnValue Parametresi Ekleniyor
                cmd.Parameters.Add("@ReturnValue", SqlDbType.Int)

                cmd.Parameters("@ReturnValue").Direction = ParameterDirection.ReturnValue

                Return cmd

            Catch ex As Exception
                Throw ex

            End Try

        End Function
        '
        '
        '*******************************************************************************
        'MethodName :BuildSqlQueryCommand
        'WhatDoes   :Verilen Parametreler ve Ýfadeyi kullanarak 
        '            Bir Command Oluþturur 
        'Inputs     :sSql   :Sql Ýfadesi
        '            param      :sqlParameterler
        'Outputs    :
        'Return     :sqlCommand
        '*******************************************************************************
        Private Function BuildSqlQueryCommand(ByVal sSql As String, _
                ByVal parameter As ArrayList) As SqlCommand

            Try

                '
                'Command Oluþturuluyor
                Dim cmd As New SqlCommand(sSql, conn)

                cmd.CommandTimeout = 0
                cmd.CommandType = CommandType.Text
                '
                If bTransaction Then

                    cmd.Transaction = tran

                End If
                '
                'Parametreler Ekleniyor
                For Each param As SqlParameter In parameter

                    cmd.Parameters.Add(param)

                Next

                Return cmd

            Catch ex As Exception
                Throw ex

            End Try

        End Function
        '
        '*******************************************************************************
        'MethodName :BuildSqlQueryCommand
        'WhatDoes   :Verilen Parametreler ve Ýfadeyi kullanarak 
        '            Bir Command Oluþturur 
        'Inputs     :sSql   :Sql Ýfadesi
        'Outputs    :
        'Return     :sqlCommand
        '*******************************************************************************
        Private Function BuildSqlQueryCommand(ByVal sSql As String) As SqlCommand

            Try

                '
                'Command Oluþturuluyor
                Dim cmd As New SqlCommand(sSql, conn)

                cmd.CommandTimeout = 0
                cmd.CommandType = CommandType.Text
                '
                If bTransaction Then

                    cmd.Transaction = tran

                End If

                Return cmd

            Catch ex As Exception
                Throw ex

            End Try

        End Function
        '
        '*******************************************************************************
        'MethodName :BuildSqlIntCommand
        'WhatDoes   :Verilen Parametreler ve Sql Ýfadesi kullanarak 
        '            Bir Command Oluþturur ve ReturnValue Ekler
        'Inputs     :sSql  :Sql Ýfadesi
        '            param      :sqlParameterler
        'Outputs    :
        'Return     :sqlCommand
        '*******************************************************************************
        Private Function BuildSqlIntCommand(ByVal sSql As String, _
                ByVal parameter As ArrayList) As SqlCommand

            Try

                Dim cmd As New SqlCommand

                cmd.CommandTimeout = 0
                'BuildSqlQueryCommand 
                cmd = BuildSqlQueryCommand(sSql, parameter)

                'ReturnValue Parametresi Ekleniyor
                cmd.Parameters.Add("@ReturnValue", SqlDbType.Int)

                cmd.Parameters("@ReturnValue").Direction = ParameterDirection.ReturnValue

                Return cmd

            Catch ex As Exception
                Throw ex

            End Try

        End Function
        '
        Private Sub OpenConnection()

            Try

                If conn.State <> ConnectionState.Open Then
                    conn.Open()
                End If

            Catch ex As Exception
                Throw ex

            End Try

        End Sub
        '
        Private Sub CloseConnection()

            Try

                CloseConnection(False)

            Catch ex As Exception
                Throw ex

            End Try

        End Sub
        '
        Private Sub CloseConnection(ByVal Trans As Boolean)
            Try

                If Not bTransaction Then
                    If conn.State <> ConnectionState.Closed Then
                        conn.Close()
                    End If
                End If

            Catch ex As Exception
                Throw ex

            End Try

        End Sub
        '
#End Region
        '
#Region " Sub "
        '
        Public Sub New(ByVal sConnString As String)

            Try

                conn.ConnectionString = sConnString

            Catch ex As Exception
                Throw ex

            End Try

        End Sub
        '
        Public Sub BeginTransaction()

            Try

                '
                OpenConnection()
                '
                If Not bTransaction Then
                    bTransaction = True
                End If
                '
                tran = conn.BeginTransaction

            Catch ex As Exception
                Throw ex

            End Try

        End Sub
        '
        Public Sub RollbackTransaction()

            Try

                '
                If Not tran Is Nothing Then
                    tran.Rollback()
                End If
                '
                If bTransaction Then
                    bTransaction = False
                End If

            Catch ex As Exception
                Throw ex

            End Try

        End Sub
        '
        Public Sub CommitTransaction()

            Try

                '
                If Not tran Is Nothing Then
                    tran.Commit()
                End If
                '
                If bTransaction Then
                    bTransaction = False
                End If

            Catch ex As Exception
                Throw ex

            End Try

        End Sub
        '
        Public Sub Dispose()

            Try

                '
                tran.Dispose()
                conn.Dispose()
                '

            Catch ex As Exception
                Throw ex

            End Try

        End Sub
        '
        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
        '
#End Region
        '
#Region " Generals "
        '
        '**************************************************************************
        'Function   :gfFillBlank_ForNull ; Null Olan Alanlara Tipine Göre 
        '                        "" Veya 0 Atar 
        '           Tarih Deðerlerini Deðiþtiremez
        'Input      :ds     : Alanlarý Kontrol edilmek Ýstenen DataSet
        'Output     :
        'Return     :Ok/Err
        '**************************************************************************
        Protected Function FillBlank_ForNull(ByRef ds As DataSet) As Boolean

            Dim iDt As Integer

            Try

                '
                'Table Sayýsý Kadar Döner
                For iDt = 0 To ds.Tables.Count - 1
                    '
                    FillBlank_ForNull(ds.Tables(iDt))
                Next
                '
                Return True

            Catch ex As Exception

                Throw ex
                'MsgBox(ex.Message)
                'Return False
            End Try

        End Function
        '
        '**************************************************************************
        'Function   :gfFillBlank_ForNull ; Null Olan Alanlara Tipine Göre 
        '                        "" Veya 0 Atar 
        '           Tarih Deðerlerini Deðiþtiremez
        'Input      :dt     : Alanlarý Kontrol edilmek Ýstenen DataTable
        'Output     :
        'Return     :Ok/Err
        '**************************************************************************
        Protected Function FillBlank_ForNull(ByRef dt As DataTable) As Boolean

            Dim iRow As Integer
            Dim iCol As Integer
            '
            Try
                '
                'Satýr Sayýsý Kadar Döner
                For iRow = 0 To dt.Rows.Count - 1
                    '
                    'Kolon Sayýsý Kadar Döner
                    For iCol = 0 To dt.Columns.Count - 1
                        '
                        'Eðer Null Ýse
                        If dt.Rows(iRow).IsNull(iCol) = True Then
                            '
                            'Tipine Göre Deðer Atanacak
                            Select Case dt.Columns(iCol).DataType.FullName.Trim
                                Case "System.Decimal"
                                    dt.Rows(iRow).Item(iCol) = 0
                                Case "System.String"
                                    dt.Rows(iRow).Item(iCol) = ""
                                Case "System.DateTime"
                                    'Throw New Exception("DateTime Alanlar Null Olmamalý.")
                            End Select
                        End If
                    Next
                Next
                '
                Return True
            Catch ex As Exception

                Throw ex
                'MsgBox(ex.Message)
                'Return False
            End Try

        End Function
        '
#End Region
        '
#Region " Functions -> RunSp "

        Public Sub RunSp(ByVal sProcName As String, _
                  ByVal parameter As ArrayList, _
                  ByRef paramterout As ArrayList)

            Dim cmd As New SqlCommand

            cmd = BuildIntCommand(sProcName, parameter)

            Try
                'Conn Close ise Açýlacak
                OpenConnection()
                '
                'ExeCute
                '

                cmd.CommandTimeout = 0
                cmd.ExecuteNonQuery()

                For Each param As SqlParameter In parameter

                    If param.Direction = ParameterDirection.Output Then
                        paramterout.Add(cmd.Parameters(param.ToString).Value)
                    End If

                Next


                '

            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSp", cmd.CommandText, parameter)

                Result.Add(ex.Message)

                Throw ex
                '
            Finally
                'Conn Kapalý Deðilse Kapatýlýyor
                CloseConnection()
            End Try
        End Sub
        '
        '*******************************************************************************
        'MethodName :RunSp
        'WhatDoes   :Verilen Proc. çalýþtýrýr
        'Inputs     :sProcName  :Proc Ýsmi
        '            param      :sqlParameterler
        '            rowsAffected: Proc iþlemleri Sonucu Etkilenen Satýr Sayýsý
        'Outputs    :
        'Return     :returnValue
        '*******************************************************************************
        Public Function RunSp(ByVal sProcName As String, _
                    ByVal parameter As ArrayList, _
                    ByRef rowsAffected As Integer) As Integer

            Dim returnValue As Integer

            Dim cmd As New SqlCommand

            cmd = BuildIntCommand(sProcName, parameter)

            Try
                'Conn Close ise Açýlacak
                OpenConnection()
                '
                'ExeCute
                '
                cmd.CommandTimeout = 0
                cmd.ExecuteNonQuery()

                rowsAffected = cmd.Parameters("@rowsAffected").Value

                returnValue = cmd.Parameters("@returnValue").Value
                '
                Return returnValue

            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSp", cmd.CommandText, parameter)

                Result.Add(ex.Message)
                '
                returnValue = -1
                Throw ex
                'Return returnValue
            Finally
                'Conn Kapalý Deðilse Kapatýlýyor
                CloseConnection()
            End Try
        End Function
        '
        '*******************************************************************************
        'MethodName :RunSp
        'WhatDoes   :Verilen Proc. çalýþtýrýr
        'Inputs     :sProcName  :Proc Ýsmi
        '            param      :sqlParameterler
        '            rowsAffected: Proc iþlemleri Sonucu Etkilenen Satýr Sayýsý
        'Outputs    :
        'Return     :returnValue
        '*******************************************************************************
        Public Function RunSp(ByVal sProcName As String, _
                    ByVal parameter As ArrayList, _
                    ByRef rowsAffected As Integer, ByVal rowRetInclude As Boolean) As Integer

            Dim returnValue As Integer
            Dim cmd As New SqlCommand

            cmd = BuildIntCommand(sProcName, parameter)

            Try
                'Conn Close ise Açýlacak
                OpenConnection()
                '
                'ExeCute
                '
                cmd.CommandTimeout = 0
                cmd.ExecuteNonQuery()

                If rowRetInclude Then

                    rowsAffected = cmd.Parameters("@rowsAffected").Value

                    returnValue = cmd.Parameters("@returnValue").Value

                    Return returnValue

                Else

                    Return 0

                End If

            Catch ex As Exception
                '

                OlayYonetici.HataYaz(ex.Message, "RunSp", cmd.CommandText, parameter)

                Result.Add(ex.Message)
                '
                returnValue = -1

                Throw ex
                'Return returnValue

            Finally
                'Conn Kapalý Deðilse Kapatýlýyor

                CloseConnection()

            End Try

        End Function
        '
        '*******************************************************************************
        'MethodName :RunSp
        'WhatDoes   :Verilen Proc. çalýþtýrýr
        'Inputs     :sProcName  :Proc Ýsmi
        '            param      :sqlParameterler
        'Outputs    :
        'Return     :SqlDataReader
        '*******************************************************************************
        Public Function RunSp(ByVal sProcName As String, _
                    ByVal parameter As ArrayList) As SqlDataReader

            Dim rd As SqlDataReader

            Dim cmd As New SqlCommand

            cmd = BuildQueryCommand(sProcName, parameter)

            Try
                'Conn Close ise Açýlacak
                OpenConnection()

                'ExeCute
                '
                cmd.CommandTimeout = 0
                rd = cmd.ExecuteReader(CommandBehavior.CloseConnection)

                Return rd
                '
            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSp", cmd.CommandText, parameter)

                Result.Add(ex.Message)
                '
                Throw ex

            Finally
                'Conn Kapalý Deðilse Kapatýlýyor
                CloseConnection()

            End Try

        End Function
        '
        '*******************************************************************************
        'MethodName :RunSp
        'WhatDoes   :Verilen Proc. çalýþtýrýr
        'Inputs     :sProcName  :Proc Ýsmi
        '            param      :sqlParameterler
        '            tableName  :Table Ýsimleri
        'Outputs    :
        'Return     :ds: dataset
        '*******************************************************************************
        Public Function RunSp(ByVal sProcName As String, _
                    ByVal parameter As ArrayList, _
                    ByVal sTableName As String) As DataSet

            Dim ds As New DataSet

            Dim da As New SqlDataAdapter

            da.SelectCommand = BuildQueryCommand(sProcName, parameter)

            Try
                'Conn Close ise Açýlacak
                OpenConnection()

                'Fill Dataset
                da.Fill(ds, sTableName)
                '
                If Not FillBlank_ForNull(ds) Then
                    '
                    'Throw New exection("Hata")
                End If
                '
                Me.Result.ReturnValue = True

                Return ds

            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSp", da.SelectCommand.ToString, parameter)

                Result.Add(ex.Message)
                '
                Me.Result.ReturnValue = False

                Throw ex

            Finally
                'Conn Kapalý Deðilse Kapatýlýyor

                CloseConnection()

            End Try

        End Function
        '
        '*******************************************************************************
        'MethodName :RunSp
        'WhatDoes   :Verilen Proc. çalýþtýrýr
        'Inputs     :sProcName  :Proc Ýsmi
        '            param      :sqlParameterler
        '            tableName  :Table Ýsimleri
        'Outputs    :ds : DataSet
        'Return     :
        '*******************************************************************************
        Public Sub RunSp(ByVal sProcName As String, _
                    ByVal parameter As ArrayList, _
                    ByVal sTableName As String, _
                    ByRef ds As DataSet)

            Dim da As New SqlDataAdapter

            da.SelectCommand = BuildQueryCommand(sProcName, parameter)

            Try
                'Conn Close ise Açýlacak
                OpenConnection()

                'Fill Dataset
                da.Fill(ds, sTableName)
                '
                If Not FillBlank_ForNull(ds) Then
                    '
                    'Throw New exection("Hata")
                End If
                '
            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSp", da.SelectCommand.ToString, parameter)

                Result.Add(ex.Message)

                Throw ex
                '
            Finally
                'Conn Kapalý Deðilse Kapatýlýyor
                CloseConnection()

            End Try

        End Sub
        '
        '*******************************************************************************
        'MethodName :RunSp
        'WhatDoes   :Verilen Proc. çalýþtýrýr
        'Inputs     :sProcName  :Proc Ýsmi
        '            tableName  :Table Ýsimleri
        'Outputs    :ds : DataSet
        'Return     :
        '*******************************************************************************
        Public Sub RunSp(ByVal sProcName As String, _
                    ByVal sTableName As String, _
                    ByRef ds As DataSet)

            Dim da As New SqlDataAdapter

            da.SelectCommand = BuildQueryCommand(sProcName)

            Try
                'Conn Close ise Açýlacak
                OpenConnection()

                'Fill Dataset
                da.Fill(ds, sTableName)
                '
                If Not FillBlank_ForNull(ds) Then
                    '
                    'Throw New exection("Hata")
                End If
                '
            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSp", da.SelectCommand.ToString)

                Result.Add(ex.Message)

                Throw ex
                '
            Finally
                'Conn Kapalý Deðilse Kapatýlýyor
                CloseConnection()
            End Try
        End Sub
        '
        '*******************************************************************************
        'MethodName :RunSp
        'WhatDoes   :Verilen Proc. çalýþtýrýr
        'Inputs     :sProcName  :Proc Ýsmi
        '            tableName  :Table Ýsimleri
        'Outputs    :
        'Return     :ds: dataset
        '*******************************************************************************
        Public Function RunSp(ByVal sProcName As String, _
                    ByVal sTableName As String) As DataSet

            Dim ds As New DataSet
            Dim da As New SqlDataAdapter

            da.SelectCommand = BuildQueryCommand(sProcName)

            Try
                'Conn Close ise Açýlacak
                OpenConnection()

                'Fill Dataset
                da.Fill(ds, sTableName)
                '
                If Not FillBlank_ForNull(ds) Then
                    '
                    'Throw New exection("Hata")
                End If
                '
                Return ds
            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSp", da.SelectCommand.ToString)

                Result.Add(ex.Message)

                Throw ex
                '
            Finally
                'Conn Kapalý Deðilse Kapatýlýyor

                CloseConnection()

            End Try

        End Function
        '
        '*******************************************************************************
        'MethodName :RunSp ; scalar sonuçlar için kullanýlacak
        'WhatDoes   :Verilen Proc. çalýþtýrýr
        'Inputs     :sProcName  :Proc Ýsmi
        'Outputs    :
        'Return     :Object
        '*******************************************************************************
        Public Function RunSp(ByVal sProcName As String) As Object
            '
            Dim cmd As New SqlCommand
            '
            cmd = BuildQueryCommand(sProcName)

            Try
                'Conn Close ise Açýlacak
                OpenConnection()
                '
                'ExeCute
                cmd.CommandTimeout = 0
                Return cmd.ExecuteScalar
                '
            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSp", cmd.CommandText.ToString)

                Result.Add(ex.Message)

                Throw ex
                '
                'Return Nothing
            Finally
                'Conn Kapalý Deðilse Kapatýlýyor
                CloseConnection()
            End Try
        End Function
        '
#End Region
        '
#Region " Functions -> RunQuery "
        '
        '*******************************************************************************
        'MethodName :RunSql
        'WhatDoes   :Verilen Sql Ýfadesini çalýþtýrýr
        'Inputs     :sSql  :Sql Ýfadesi
        '            tableName  :Table Ýsimleri
        'Outputs    :
        'Return     :ds: dataset
        '*******************************************************************************
        Public Function RunSql(ByVal sSql As String, _
                    ByVal sTableName As String) As DataSet

            Dim ds As New DataSet
            Dim da As New SqlDataAdapter
            '
            da.SelectCommand = BuildSqlQueryCommand(sSql)
            '
            Try
                'Conn Close ise Açýlacak
                OpenConnection()
                '
                'Fill Dataset
                da.Fill(ds, sTableName)
                '
                If Not FillBlank_ForNull(ds) Then
                    '
                    'Throw New exection("Hata")
                End If
                '
                Me.Result.ReturnValue = True
                Return ds
            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSql", sSql)
                Result.Add(ex.Message)
                Me.Result.ReturnValue = False
                Throw ex
                '
            Finally
                '
                'Conn Kapalý Deðilse Kapatýlýyor
                CloseConnection()
            End Try
        End Function
        '
        '*******************************************************************************
        'MethodName :RunSql
        'WhatDoes   :Verilen Sql Ýfadesini çalýþtýrýr
        'Inputs     :sSql  :Sql Ýfadesi
        'Outputs    :
        'Return     :dt: dataTable
        '*******************************************************************************
        Public Function RunSql(ByVal sSql As String) As DataTable

            Dim dt As New DataTable
            Dim da As New SqlDataAdapter
            '
            da.SelectCommand = BuildSqlQueryCommand(sSql)
            '
            Try
                'Conn Close ise Açýlacak
                OpenConnection()
                '
                'Fill Dataset
                da.Fill(dt)
                '
                If Not FillBlank_ForNull(dt) Then
                    '
                    'Throw New exection("Hata")
                End If
                '
                Return dt
            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSql", sSql)
                Result.Add(ex.Message)
                Throw ex
                '
            Finally
                '
                'Conn Kapalý Deðilse Kapatýlýyor
                CloseConnection()
            End Try

        End Function
        '
        '*******************************************************************************
        'MethodName :RunSql ; Scalar sonuçlar için kullanýlacak
        'WhatDoes   :Verilen Sql Ýfadesini çalýþtýrýr
        'Inputs     :sSql  :Sql Ýfadesi
        'Outputs    :
        'Return     :object
        '*******************************************************************************
        Public Function RunSql(ByVal sSql As String, ByVal Scalar As Boolean) As Object
            '
            Dim cmd As SqlCommand
            '
            cmd = BuildSqlQueryCommand(sSql)
            '
            Try
                'Conn Close ise Açýlacak
                OpenConnection()
                '
                'Execute Scalar
                cmd.CommandTimeout = 0
                Result.ReturnValue = True
                Return cmd.ExecuteScalar()
                '
            Catch ex As Exception
                '
                OlayYonetici.HataYaz(ex.Message, "RunSql", sSql)
                Result.Add(ex.Message)
                Result.ReturnValue = False

                Throw ex
                'Return Nothing
            Finally
                '
                'Conn Kapalý Deðilse Kapatýlýyor
                CloseConnection()
            End Try
        End Function
        '
#End Region
        '
    End Class
    '
    Public Enum AddParamType
        Other = 1
        Delete = 2
        Update = 4
        Load = 8
        Add = 16
    End Enum

End Namespace
