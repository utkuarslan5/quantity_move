'Imports System
Imports System.Data
Imports System.Data.Common
'Imports System.Configuration
Imports AsVol 

Namespace Core
  Public Class DataForGeneral

    Private Factory As DbProviderFactory '= DbProviderFactories.GetFactory("System.Data.SqlClient")

    Private conn As DbConnection '= Factory.CreateConnection

    Private tran As DbTransaction

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
                                       ByVal parameter As ArrayList) As DbCommand

      Try

        '
        'Command Oluþturuluyor
        Dim cmd As DbCommand = Factory.CreateCommand

        cmd.Connection = conn
        cmd.CommandText = sProcName
        cmd.CommandTimeout = 0
        cmd.CommandType = CommandType.StoredProcedure
        '
        If bTransaction Then

          cmd.Transaction = tran

        End If
        '
        'Parametreler Ekleniyor
        For Each param As DbParameter In parameter

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
    Private Function BuildQueryCommand(ByVal sProcName As String) As DbCommand

      Try

        '
        'Command Oluþturuluyor
        Dim cmd As DbCommand = Factory.CreateCommand

        cmd.Connection = conn
        cmd.CommandText = sProcName
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
                                     ByVal parameter As ArrayList) As DbCommand

      Try

        Dim cmd As DbCommand = Factory.CreateCommand

        cmd.CommandTimeout = 0
        'BuildQueryCommand 
        cmd = BuildQueryCommand(sProcName, parameter)

        'ReturnValue Parametresi Ekleniyor
        Dim Param As DbParameter = Factory.CreateParameter

        Param.ParameterName = "@ReturnValue"

        'Param.Value = 0

        Param.DbType = DbType.Int32

        Param.Direction = ParameterDirection.ReturnValue

        cmd.Parameters.Add(Param)

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
                                          ByVal parameter As ArrayList) As DbCommand

      Try

        '
        'Command Oluþturuluyor
        Dim cmd As DbCommand = Factory.CreateCommand

        cmd.Connection = conn
        cmd.CommandText = sSql
        cmd.CommandTimeout = 0
        cmd.CommandType = CommandType.Text
        '
        If bTransaction Then

          cmd.Transaction = tran

        End If
        '
        'Parametreler Ekleniyor
        For Each param As DbParameter In parameter

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
    Private Function BuildSqlQueryCommand(ByVal sSql As String) As DbCommand

      Try

        '
        'Command Oluþturuluyor
        Dim cmd As DbCommand = Factory.CreateCommand

        cmd.Connection = conn
        cmd.CommandText = sSql
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
                                        ByVal parameter As ArrayList) As DbCommand

      Try

        Dim cmd As DbCommand = Factory.CreateCommand

        cmd.CommandTimeout = 0
        'BuildSqlQueryCommand 
        cmd = BuildSqlQueryCommand(sSql, parameter)

        'ReturnValue Parametresi Ekleniyor
        Dim Param As DbParameter = Factory.CreateParameter

        Param.ParameterName = "@ReturnValue"

        'Param.Value = 0

        Param.DbType = DbType.Int32

        Param.Direction = ParameterDirection.ReturnValue

        cmd.Parameters.Add(Param)

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

    Public Function GetProviderName(ByVal ProviderType As ProviderTypes) As String

      Try

        Dim dtProviders As New DataTable()
        dtProviders = DbProviderFactories.GetFactoryClasses()
        For Each drProvider As DataRow In dtProviders.Rows
          'For i As Integer = 0 To dtProviders.Columns.Count - 1
          'TextBox1.Text &= ((drProvider(i).ToString())) & vbNewLine
          If drProvider("InvariantName").ToString.IndexOf(ProviderType.ToString) <> -1 Then
            Return drProvider("InvariantName").ToString
          End If
          'Next
          'TextBox1.Text &= vbNewLine
        Next

        'Select Case ProviderType

        '  Case ProviderTypes.SqlClient
        '    Return DbProviderFactories.GetFactoryClasses.Rows(4)(3).ToString

        '  Case ProviderTypes.OleDb
        '    Return DbProviderFactories.GetFactoryClasses.Rows(2)(3).ToString

        '  Case ProviderTypes.Odbc
        '    Return DbProviderFactories.GetFactoryClasses.Rows(1)(3).ToString

        '  Case ProviderTypes.OracleClient
        '    Return DbProviderFactories.GetFactoryClasses.Rows(3)(3).ToString

        'End Select

      Catch ex As Exception
        Throw ex

      End Try

    End Function

    Public Sub New(ByVal sConnString As String, _
                   Optional ByVal ProviderType As ProviderTypes = ProviderTypes.SqlClient)

      Try


        Factory = DbProviderFactories.GetFactory(GetProviderName(ProviderType))

        'Dim FactoryDataTable As DataTable = DbProviderFactories.GetFactoryClasses()

        'For Each row As DataRow In FactoryDataTable.Rows
        '  'For Each column As DataColumn In row.Table.Columns
        '  Factory = DbProviderFactories.GetFactory(row)
        '  'Next
        'Next

        conn = Factory.CreateConnection
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

      Dim cmd As DbCommand

      cmd = BuildIntCommand(sProcName, parameter)

      Try
        'Conn Close ise Açýlacak
        OpenConnection()
        '
        'ExeCute
        '

        cmd.CommandTimeout = 0
        cmd.ExecuteNonQuery()

        For Each param As DbParameter In parameter

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

      Dim cmd As DbCommand

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
                          ByRef rowsAffected As Integer, _
                          ByVal rowRetInclude As Boolean) As Integer

      Dim returnValue As Integer
      Dim cmd As DbCommand

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
                          ByVal parameter As ArrayList) As DbDataReader

      Dim rd As DbDataReader

      Dim cmd As DbCommand

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

      Dim da As DbDataAdapter = Factory.CreateDataAdapter

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

      Dim da As DbDataAdapter = Factory.CreateDataAdapter

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

      Dim da As DbDataAdapter = Factory.CreateDataAdapter

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
      Dim da As DbDataAdapter = Factory.CreateDataAdapter

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
      Dim cmd As DbCommand
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
      Dim da As DbDataAdapter = Factory.CreateDataAdapter
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
      Dim da As DbDataAdapter = Factory.CreateDataAdapter
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
      Dim cmd As DbCommand
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
        Public Sub RunSqlSp(ByVal sSql As String)
            '
            Dim cmd As DbCommand
            '
            cmd = BuildSqlQueryCommand(sSql)
            '
            Try
                'Conn Close ise Açýlacak
                OpenConnection()
                '
                'Execute Scalar
                cmd.CommandTimeout = 0

                cmd.ExecuteNonQuery()
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
        End Sub
#Region "Enums"
    Public Enum AddParamTypeGeneral
      Other = 1
      Delete = 2
      Update = 4
      Load = 8
      Add = 16
    End Enum

    Public Enum ProviderTypes
      SqlClient = 1 'System.Data.SqlClient
      OleDb = 2 'System.Data.OleDb
      Odbc = 3 'System.Data.Odbc
      OracleClient = 4 'System.Data.OracleClient
    End Enum
#End Region

    'Public Function RunSelect(ByVal TableName As String, _
    '                          ByVal Where As Hashtable, _
    '                          ByVal OrderBy As String, _
    '                          ByVal Columns As String) As Hashtable
    '  ' Kullanacaðýmýz Hashtable nesnemizi oluþturuyoruz.
    '  Dim HT As New Hashtable()
    '  ' Hata korumasý bloðu içerisinde yazýyoruz.
    '  Try
    '    ' Baðlantý için gerekli Connection satýrlarýný yazýyoruz.

    '    OpenConnection()

    '    Dim DS As New DataSet()
    '    Dim DT As New DataTable()
    '    Dim DAP As DbDataAdapter = Factory.CreateDataAdapter
    '    Dim Comm As DbCommand = Factory.CreateCommand()

    '    Comm.Connection = conn

    '    Comm.CommandText = "SELECT " & Columns & " FROM " & TableName & " {0} " & OrderBy

    '    Dim strWhere As String = ""

    '    Dim qCount As Integer = 0

    '    For Each var As String In TryCast(Where.Keys, ICollection)

    '      If var(0) <> "@" Then

    '        If (qCount / 2) = 0 Then

    '          strWhere += " " + Where("@" & qCount) & " "

    '        End If
    '        If strWhere <> "  " Then
    '          strWhere += " And " & "[" & var & "] = @" & var & " "
    '        Else
    '          strWhere += "[" & var & "] = @" & var & " "
    '        End If
    '        qCount += 1

    '        Dim Parameter As DbParameter = Factory.CreateParameter

    '        Parameter.ParameterName = "@" & var

    '        Parameter.Value = Where(var)

    '        Comm.Parameters.Add(Parameter)

    '      End If

    '    Next

    '    strWhere = "Where " & strWhere

    '    Dim strCommand As String = String.Format(Comm.CommandText, strWhere)

    '    Comm.CommandText = strCommand

    '    DAP.SelectCommand = Comm

    '    DAP.Fill(DS)

    '    DAP.Fill(DT)

    '    HT.Add("DataSet", DS)

    '    HT.Add("DataTable", DT)

    '    HT.Add("DataCount", DS.Tables(0).Rows.Count)

    '    CloseConnection()

    '    DAP.Dispose()

    '    Comm.Dispose()

    '  Catch Ex As Exception
    '    HT.Add("Error", Ex.Message)
    '  End Try
    '  Return HT
    'End Function

    'Public Function InsertData(ByVal TableName As String, _
    '                           ByVal ColumnsValues As Hashtable) _
    '                           As Hashtable

    '  Dim rHt As New Hashtable()

    '  Try

    '    OpenConnection()

    '    Dim Comm As DbCommand = Factory.CreateCommand()

    '    Comm.Connection = conn

    '    If bTransaction Then

    '      Comm.Transaction = tran

    '    End If


    '    Dim Values As String = ""

    '    Dim Columns As String = ""

    '    Dim i As Integer = 0

    '    For Each var As String In TryCast(ColumnsValues.Keys, ICollection)

    '      i += 1

    '      Columns += "[" & var & "]"

    '      Values += "@" & var

    '      If i <> ColumnsValues.Keys.Count Then

    '        Columns += ","

    '        Values += ","

    '      End If

    '      Dim Parameter As DbParameter = Factory.CreateParameter()

    '      Parameter.ParameterName = "@" & var

    '      Parameter.Value = ColumnsValues(var)

    '      Comm.Parameters.Add(Parameter)

    '    Next

    '    Dim strCommand As String = "INSERT INTO " & TableName & "(" & Columns & ") VALUES(" & Values & ")"

    '    Comm.CommandText = strCommand

    '    Comm.ExecuteNonQuery()

    '    Dim IComm As DbCommand = Factory.CreateCommand()

    '    IComm.CommandText = "SELECT @@IDENTITY"

    '    IComm.Connection = conn

    '    If bTransaction Then

    '      IComm.Transaction = tran

    '    End If

    '    Dim intIdentity As Integer = Convert.ToInt32(IIf(IsDBNull(IComm.ExecuteScalar()), 0, IComm.ExecuteScalar()))

    '    rHt.Add("Identity", intIdentity)

    '    CloseConnection()

    '    Comm.Dispose()

    '    IComm.Dispose()

    '    Return rHt

    '  Catch Ex As Exception

    '    rHt.Add("Error", Ex.Message)

    '    Return rHt

    '  End Try

    'End Function

    'Public Function UpdateData(ByVal TableName As String, _
    '                           ByVal Where As Hashtable, _
    '                           ByVal ColumnsValues As Hashtable) _
    '                           As Hashtable

    '  Dim rHt As New Hashtable()

    '  Try
    '    OpenConnection()

    '    Dim Comm As DbCommand = Factory.CreateCommand()

    '    Comm.Connection = conn

    '    If bTransaction Then

    '      Comm.Transaction = tran

    '    End If

    '    Dim ColVal As String = ""

    '    Dim i As Integer = 0

    '    For Each var As String In TryCast(ColumnsValues.Keys, ICollection)
    '      i += 1
    '      ColVal += "[" & var & "]" & " = @" & var

    '      Dim Parameter As DbParameter = Factory.CreateParameter()

    '      Parameter.ParameterName = "@" & var

    '      Parameter.Value = ColumnsValues(var)

    '      Comm.Parameters.Add(Parameter)

    '      If i <> ColumnsValues.Keys.Count Then

    '        ColVal += ","

    '      End If

    '    Next

    '    Comm.CommandText = "UPDATE " & TableName & " SET " & ColVal & " "

    '    Dim strWhere As String = ""

    '    Dim qCount As Integer = 0
    '    Dim whereKey As Integer = 0

    '    For Each var As String In TryCast(Where.Keys, ICollection)

    '      If var(0) <> "@" Then

    '        If (qCount / 2) = 0 Then

    '          strWhere += " " + Where("@" & qCount) & " "

    '        End If

    '        If Comm.Parameters.Contains("@" & var) Then
    '          whereKey += 1
    '        End If

    '        If strWhere <> "  " Then
    '          strWhere += " And " & "[" & var & "] = @" & var & whereKey & " "
    '        Else
    '          strWhere += "[" & var & "] = @" & var & whereKey & " "
    '        End If
    '        qCount += 1

    '        Dim Parameter As DbParameter = Factory.CreateParameter

    '        Parameter.ParameterName = "@" & var & whereKey

    '        Parameter.Value = Where(var)

    '        Comm.Parameters.Add(Parameter)

    '      End If

    '    Next

    '    strWhere = "Where " & strWhere

    '    Dim strCommand As String = Comm.CommandText & strWhere

    '    Comm.CommandText = strCommand

    '    Comm.ExecuteNonQuery()

    '    CloseConnection()

    '    Comm.Dispose()

    '    Return rHt

    '  Catch Ex As Exception

    '    rHt.Add("Error", Ex.Message)

    '    Return rHt

    '  End Try

    'End Function

  End Class

End Namespace
