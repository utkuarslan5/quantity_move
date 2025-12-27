Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data
Imports ConnectionValues
Imports AsVol

<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsGeneral
  Inherits System.Web.Services.WebService

#Region "Web Methods"

  Public Enum ProviderTypes
    SqlClient = 1 'System.Data.SqlClient
    OleDb = 2 'System.Data.OleDb
    Odbc = 3 'System.Data.Odbc
    OracleClient = 4 'System.Data.OracleClient
  End Enum

  '<WebMethod(Description:="Prosedür çalýþtýrmayý saðlar.")> _
  'Public Function RunSp(ByVal sProcName As String, _
  '                      ByVal Parameter As ArrayList, _
  '                      ByRef Paramterout As ArrayList)

  '    Try

  '        Dim clsDbConnectionValues As New ConnectionValues
  '        Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(enumSqlConnectionType.Pwms)
  '        Dim db As New Core.DataForGeneral(ConnectionString, Core.DataForGeneral.ProviderTypes.OleDb)
  '        db.RunSp(sProcName, Parameter, Paramterout)

  '    Catch ex As Exception
  '        Throw ex

  '    End Try

  'End Function

  <WebMethod(Description:="Sql sorgusunu dataset olarak döndürür.")> _
 Public Function RunSqlDs(ByVal sSql As String, _
                          ByVal TableName As String, _
                          ByVal ProviderType As ProviderTypes) _
                          As DataSet

    Try

      Dim clsDbConnectionValues As New ConnectionValues
            Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(enumConnectionType.SqlClient)
            Dim db As New Core.DataForGeneral(ConnectionString, Core.DataForGeneral.ProviderTypes.SqlClient)


      Return db.RunSql(sSql, TableName)

    Catch ex As Exception
      Throw ex

    End Try

  End Function

  <WebMethod(Description:="Sql sorgusunu çalýþtýrmayý saðlar.")> _
  Public Function RunSql(ByVal sSql As String, _
                         ByVal Scalar As Boolean, _
                         ByVal ProviderType As ProviderTypes) _
                         As Object

    Dim clsDbConnectionValues As New ConnectionValues
    Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(ProviderType)
    Dim db As New Core.DataForGeneral(ConnectionString, ProviderType)

    Try

      'db.BeginTransaction()
      Dim Obj As Object = db.RunSql(sSql, Scalar)
      'db.CommitTransaction()
      Return Obj

    Catch ex As Exception
      'If db.Transaction = True Then
      '  db.RollbackTransaction()
      'End If
      Throw ex

    End Try

  End Function

    <WebMethod(Description:="Sql sorgusunu çalýþtýrmayý saðlar.")> _
  Public Function RunSqlSp(ByVal sSql As String, _
                         ByVal ProviderType As ProviderTypes) _
                         As Object

        Dim clsDbConnectionValues As New ConnectionValues
        Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(ProviderType)
        Dim db As New Core.DataForGeneral(ConnectionString, ProviderType)

        Try

            'db.BeginTransaction()
            db.RunSqlSp(sSql)
      

        Catch ex As Exception
            'If db.Transaction = True Then
            '  db.RollbackTransaction()
            'End If
            Throw ex

        End Try

    End Function

    <WebMethod(Description:="Sql sorgusunu çalýþtýrmayý saðlar.")> _
    Public Function RunSql2(ByVal sSql As String, _
                             ByVal Scalar As Boolean, _
                             ByVal ProviderType As ProviderTypes) _
                             As Boolean

        Dim clsDbConnectionValues As New ConnectionValues
        Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(ProviderType)
        Dim db As New Core.DataForGeneral(ConnectionString, ProviderType)

        Try

            '            db.BeginTransaction()
            Dim Obj As Object = db.RunSql(sSql, Scalar)
            '            db.CommitTransaction()
            Return db.Result.ReturnValue
            'Return False

        Catch ex As Exception
            'If db.Result.ReturnValue = False Then
            Return False
            'End If
            '            If db.Transaction = True Then
            '                db.RollbackTransaction()
            '            End If
            'Throw New Exception("False")

        End Try

    End Function

  '<WebMethod(Description:="Kullanýcý kodu ve þifreyi kontrol eder.")> _
  'Public Function CheckLogin(ByVal UserCode As String, _
  '                           ByVal UserPassword As String) _
  '                           As Boolean

  '  Try

  '    Dim clsDbConnectionValues As New ConnectionValues
  '    Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(enumConnectionType.OleDb)
  '    Dim db As New Core.DataForGeneral(ConnectionString, Core.DataForGeneral.ProviderTypes.OleDb)
  '    Dim dt As New DataTable
  '    'Dim sStr As String
  '    Dim param As New ArrayList
  '    Dim paramout As New ArrayList

  '    param.Clear()
  '    paramout.Clear()

  '    param.Add(New SqlClient.SqlParameter("@UserCode", UserCode))
  '    param.Add(New SqlClient.SqlParameter("@UserPassword", UserPassword))

  '    Dim p1 As New SqlClient.SqlParameter("@Confirm", SqlDbType.Bit)
  '    p1.Direction = ParameterDirection.Output
  '    param.Add(p1)

  '    Dim p2 As New SqlClient.SqlParameter("@UserName", SqlDbType.NVarChar, 50)
  '    p2.Direction = ParameterDirection.Output
  '    param.Add(p2)

  '    Dim p3 As New SqlClient.SqlParameter("@UserTypeCode", SqlDbType.NVarChar, 1)
  '    p3.Direction = ParameterDirection.Output
  '    param.Add(p3)

  '    Dim p4 As New SqlClient.SqlParameter("@Id", SqlDbType.Int)
  '    p4.Direction = ParameterDirection.Output
  '    param.Add(p4)

  '    db.RunSp("[LoginCheck]", param, paramout)

  '    If paramout(0).ToString = "False" Then
  '      Return False
  '      'Throw New Exception("|Kullanýcý kodu veya þifre yanlýþ !|") ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '      'Return "|Kullanýcý kodu veya þifre yanlýþ !|" ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '    End If

  '    Return True

  '  Catch ex As Exception
  '    'CheckLogin = False
  '    Throw ex

  '  End Try

  'End Function

  '<WebMethod(Description:="Sap tarafýnda Voucherýn bittiðine onay verir.")> _
  'Public Function SapDataVoucherConfirm(ByVal VoucherNo As Long, _
  '                                      ByVal ZVOUCHERL As DataSet) _
  '                                      As Boolean

  '    Try

  '        Dim Destination As New SAP.Connector.SAPLogonDestination
  '        'If SirketValue = "EKIP DANISMANLIK" Then
  '        '    'Dim VoucherBlockTableEkip As New Z_EKP_BLO_VOUCHER.Z_EKP_BLO_VOUCHER_EKIPTable
  '        '    'Dim ProxyEkip = New Z_EKP_BLO_VOUCHER.Z_EKP_BLO_VOUCHER
  '        '    'ProxyEkip.Connectionstring = SapConnectionString
  '        '    'ProxyEkip.Connection.Open()

  '        '    'ProxyEkip.Z_Ekp_Blo_Voucher(VoucherNo, VoucherBlockTableEkip)

  '        '    'Dim dtVoucherBlock As New System.Data.DataTable
  '        '    'dtVoucherBlock = VoucherBlockTableEkip.ToADODataTable
  '        '    'ProxyEkip.Connection.Close()
  '        '    'Return dtVoucherBlock

  '        'Else
  '        Dim VoucherConfirmTableFirma As New Z_EKP_CONF_VOUCHER_ORJ.ZVOUCHERL_EKIPTable
  '        Dim ProxyFirma = New Z_EKP_CONF_VOUCHER_ORJ.Z_EKP_CONF_VOUCHER_ORJ

  '        Dim clsDbConnectionValues As New ConnectionValues
  '        Dim SapConnectionString As String = clsDbConnectionValues.ReadSapConnectionValues()
  '        ProxyFirma.Connectionstring = SapConnectionString
  '        ProxyFirma.Connection.Open()

  '        Dim Rtn1 As String '= String.Empty
  '        Dim Rtn2 As String '= String.Empty
  '        'Dim Rtn3 As String '= String.Empty

  '        '***VoucherConfirmTableFirma.FromADODataTable(ZVOUCHERL.Tables(0))

  '        'For Each RowSap As DataRow In ZVOUCHERL.Tables(0).Rows

  '        '    Dim VoucherConfirmTableX As New Z_EKP_CONF_VOUCHER_ORJ.ZVOUCHERL_EKIP
  '        '    VoucherConfirmTableX.Vounr = RowSap("VOUNR").ToString
  '        '    VoucherConfirmTableX.Vourl = RowSap("VOURL").ToString
  '        '    VoucherConfirmTableX.Vourv = RowSap("VOURV").ToString
  '        '    VoucherConfirmTableX.Vourp = RowSap("VOURP").ToString
  '        '    VoucherConfirmTableX.Matnr = RowSap("MATNR").ToString
  '        '    VoucherConfirmTableX.Menge = CDec(RowSap("MENGE").ToString)
  '        '    VoucherConfirmTableX.Meýns = RowSap("MEINS").ToString
  '        '    VoucherConfirmTableX.Charg = RowSap("CHARG").ToString
  '        '    VoucherConfirmTableX.Lýfnr = RowSap("LIFNR").ToString
  '        '    VoucherConfirmTableX.Txt = RowSap("TXT").ToString
  '        '    VoucherConfirmTableX.Canceled = RowSap("CANCELED").ToString
  '        '    VoucherConfirmTableX.Cname = RowSap("CNAME").ToString
  '        '    VoucherConfirmTableX.Cdate = RowSap("CDATE").ToString
  '        '    VoucherConfirmTableX.Ctýme = RowSap("CTIME").ToString
  '        '    VoucherConfirmTableX.Menges = CDec(RowSap("MENGES").ToString)

  '        '    'VoucherConfirmTableFirma.CreateNewRow()
  '        '    VoucherConfirmTableFirma.Add(VoucherConfirmTableX)

  '        'Next

  '        ProxyFirma.Z_Ekp_Conf_Voucher(VoucherNo, Rtn1, Rtn2, VoucherConfirmTableFirma)

  '        Dim dtVoucherConfirm As New System.Data.DataTable
  '        dtVoucherConfirm = VoucherConfirmTableFirma.ToADODataTable
  '        ProxyFirma.Connection.Close()

  '        'Dim dtTemp As New DataSet
  '        'dtTemp.Tables.Add(dtVoucherConfirm)

  '        If Rtn1 = "X" Then
  '            Return False
  '        ElseIf Rtn2 = "X" Then
  '            Return True
  '        End If

  '        'Return True
  '        'End If

  '    Catch ex As Exception
  '        Throw ex

  '    End Try

  'End Function

  '***************************************************************************************
  ' <WebMethod(Description:="Ýþemrinin olup olmadýðýný kontrol eder.")> _
  'Public Function CheckOrder(ByVal Order As String) _
  '                           As String

  '     Try

  '         Dim clsDbConnectionValues As New ConnectionValues
  '         Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(enumSqlConnectionType.Pwms)
  '         Dim db As New Core.DataForGeneral(ConnectionString, Core.DataForGeneral.ProviderTypes.OleDb)
  '         Dim dt As New DataTable
  '         Dim sStr As String

  '         sStr = " select Product, Description, Quantity, Date from dbo.Z_ED_PLNHDR with (nolock)" & _
  '                " where Plnnbr='" & IIf(String.IsNullOrEmpty(Order), String.Empty, Order) & "'"
  '         dt = db.RunSql(sStr)

  '         If dt.Rows.Count = 0 Then
  '             'Return False
  '             Throw New Exception("|Ýþemri bulunamadý !|") ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '             'Return "|Ýþemri bulunamadý !|" ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '         Else

  '             Dim Product As String = dt.Rows(0)("Product").ToString
  '             Dim Description As String = dt.Rows(0)("Description").ToString
  '             Dim Quantity As Decimal = CDec(dt.Rows(0)("Quantity").ToString)
  '             Dim DateStr As String = dt.Rows(0)("Date").ToString
  '             If DateStr <> String.Empty Then
  '                 DateStr = DateStr.Substring(6, 2) & "/" & DateStr.Substring(4, 2) & "/" & DateStr.Substring(0, 4)
  '             End If

  '             Return Product & "|" & Description & "|" & Quantity & "|" & DateStr

  '         End If

  '     Catch ex As Exception
  '         'CheckOrder = "False"
  '         Throw ex

  '     End Try

  ' End Function

  '<WebMethod(Description:="Malzemenin olup olmadýðýný kontrol eder.")> _
  'Public Function CheckItem(ByVal Order As String, _
  '                          ByVal LabelNo As Long) _
  '                          As String

  '    Try

  '        Dim clsDbConnectionValues As New ConnectionValues
  '        Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(enumSqlConnectionType.Pwms)
  '        Dim db As New Core.DataForGeneral(ConnectionString, Core.DataForGeneral.ProviderTypes.OleDb)
  '        Dim dt As New DataTable
  '        Dim sStr As String

  '        sStr = " select Itnbr from dbo.Z_ED_LBLMST with (nolock)" & _
  '               " where Labelno=" & IIf(String.IsNullOrEmpty(LabelNo), 0, LabelNo)
  '        dt = db.RunSql(sStr)

  '        Dim Itnbr As String
  '        If dt.Rows.Count = 0 Then
  '            'Return False
  '            'Throw New Exception("|Malzeme bulunamadý !|") ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '            Return "|Malzeme bulunamadý !|" ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '        Else
  '            Itnbr = dt.Rows(0)("Itnbr").ToString
  '        End If

  '        sStr = " select isnull(Count(Component),0) as Component from dbo.Z_ED_PLNDTL with (nolock)" & _
  '               " where Component='" & Itnbr & "'" & _
  '               "   and Plnnbr='" & Order & "'"
  '        dt = db.RunSql(sStr)

  '        If dt.Rows.Count = 0 Then
  '            'Return False
  '            'Throw New Exception("|Bileþen recetede yok !|") ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '            Return "|Bileþen recetede yok !|" ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '        ElseIf dt.Rows(0)("Component").ToString = "0" Then
  '            'Return False
  '            'Throw New Exception("|Bileþen recetede yok !|") ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '            Return "|Bileþen recetede yok !|" ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '        End If

  '        sStr = " select Expdate from dbo.Z_ED_LBLMST with (nolock) " & _
  '               " where Labelno=" & IIf(String.IsNullOrEmpty(LabelNo), 0, LabelNo)
  '        Dim Expdate As Long = CLng(db.RunSql(sStr, True))

  '        sStr = " select min(Expdate) from dbo.Z_ED_LBLMST with (nolock) " & _
  '               " where Itnbr='" & Itnbr & "'" & _
  '               "   and isnull(Boxqty,0)-isnull(Issuedqty,0) > 0 " & _
  '               "   and Status='0' "
  '        'isnull(Issuedqty,0) < isnull(totalqty,0)
  '        Dim MinExpDate As Long = CLng(db.RunSql(sStr, True))

  '        sStr = " select min(Labelno) from dbo.Z_ED_LBLMST with (nolock) " & _
  '               " where Expdate='" & MinExpDate & "'" & _
  '               "     and Itnbr='" & Itnbr & "'" & _
  '               "    and Status='0'"
  '        Dim MinLabelno As String = CStr(db.RunSql(sStr, True))

  '        If Expdate > MinExpDate Then
  '            'Return False
  '            'Throw New Exception("|Daha eski lot mevcut !" & Chr(13) & "Etiket" & " " & MinLabelno & "|") ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '            Return "|Daha eski lot mevcut !" & Chr(13) & "Etiket" & " " & MinLabelno & Chr(13) & "Devam etmek istiyor musunuz ?" & "|" ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '        End If

  '        Return "|True|"

  '    Catch ex As Exception
  '        'CheckItem = "False"
  '        Throw ex

  '    End Try

  'End Function

  ' <WebMethod(Description:="Status update eder.")> _
  'Public Function UpdateStatus(ByVal LabelNo As Long) As String

  '     Try

  '         Dim clsDbConnectionValues As New ConnectionValues
  '         Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(enumSqlConnectionType.Pwms)
  '         Dim db As New Core.DataForGeneral(ConnectionString, Core.DataForGeneral.ProviderTypes.OleDb)
  '         Dim dt As New DataTable
  '         Dim sStr As String

  '         sStr = " update dbo.Z_ED_LBLMST with (tablock)" & _
  '                " set Status='1' " & _
  '                " where Labelno=" & IIf(String.IsNullOrEmpty(LabelNo), 0, LabelNo)
  '         db.RunSql(sStr, True)

  '         Return "True"

  '     Catch ex As Exception
  '         'UpdateStatus = "False"
  '         Throw ex

  '     End Try

  ' End Function

  '**************************************************************************************
  '__________________________________________________________________________________

  '<WebMethod(Description:="Üretim etiketi ve bileþen okutularak " & _
  '                        "lot kontrolünün yapýlmasýný saðlar.")> _
  'Public Function CheckLot(ByVal ProductionLabel As String, _
  '                         ByVal Component As String) _
  '                         As Boolean

  '    Try

  '        Dim clsDbConnectionValues As New ConnectionValues
  '        Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(enumSqlConnectionType.Pwms)
  '        Dim db As New Core.DataForGeneral(ConnectionString, Core.DataForGeneral.ProviderTypes.OleDb)
  '        Dim dt As New DataTable
  '        Dim sStr As String

  '        sStr = " select Itnbr from dbo.Z_ED_LBLMST with (nolock)" & _
  '               " where Labelno=" & IIf(String.IsNullOrEmpty(Component), 0, Component)
  '        Dim Itnbr As String = db.RunSql(sStr, True)

  '        sStr = " select isnull(Count(Component),0) from dbo.Z_ED_PLNDTL with (nolock)" & _
  '               " where Labelno=" & IIf(String.IsNullOrEmpty(ProductionLabel), 0, ProductionLabel) & _
  '               "   and Component='" & Itnbr & "'"
  '        Dim CntItnbr As Long = CLng(db.RunSql(sStr, True))

  '        If CntItnbr = 0 Then
  '            'Return False
  '            Throw New Exception("|Bileþen üretim etiketine ait deðil !|") ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '            'Return "|Bileþen üretim etiketine ait deðil !|" ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '        End If

  '        sStr = " select Expdate from dbo.Z_ED_LBLMST with (nolock) " & _
  '               " where Labelno=" & IIf(String.IsNullOrEmpty(Component), 0, Component)
  '        Dim Expdate As Long = CLng(db.RunSql(sStr, True))

  '        sStr = " select min(Expdate) from dbo.Z_ED_LBLMST with (nolock) " & _
  '               " where Itnbr='" & Itnbr & "'" & _
  '               "   and isnull(Boxqty,0)-isnull(Issuedqty,0) > 0 "
  '        Dim MinExpDate As Long = CLng(db.RunSql(sStr, True))

  '        sStr = " select min(Labelno) from dbo.Z_ED_LBLMST with (nolock) " & _
  '               " where Expdate='" & MinExpDate & "'" & _
  '               "    and Itnbr='" & Itnbr & "'"
  '        Dim MinLabelno As String = CStr(db.RunSql(sStr, True))

  '        If Expdate > MinExpDate Then
  '            'Return False
  '            Throw New Exception("|Daha eski lot mevcut !" & Chr(13) & "Etiket" & " " & MinLabelno & "|") ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '            'Return "|Daha eski lot mevcut !" & Chr(13) & "Etiket" & " " & MinLabelno & "|" ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '        End If

  '        Return True

  '    Catch ex As Exception
  '        'CheckLot = False
  '        Throw ex

  '    End Try

  'End Function

  '<WebMethod(Description:="Üretim etiketinin olup olmadýðýný kontrol eder.")> _
  'Public Function CheckProductionLabel(ByVal ProductionLabel As String) _
  '                                     As Boolean

  '    Try

  '        Dim clsDbConnectionValues As New ConnectionValues
  '        Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(enumSqlConnectionType.Pwms)
  '        Dim db As New Core.DataForGeneral(ConnectionString, Core.DataForGeneral.ProviderTypes.OleDb)
  '        Dim dt As New DataTable
  '        Dim sStr As String

  '        sStr = " select isnull(Count(Component),0) from dbo.Z_ED_PLNDTL with (nolock)" & _
  '               " where Labelno=" & IIf(String.IsNullOrEmpty(ProductionLabel), 0, ProductionLabel)
  '        Dim CntItnbr As Long = CLng(db.RunSql(sStr, True))

  '        If CntItnbr = 0 Then
  '            'Return False
  '            Throw New Exception("|Üretim etiketi bulunamadý !|") ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '            'Return "|Üretim etiketi bulunamadý !|" ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '        End If

  '        Return True

  '    Catch ex As Exception
  '        'CheckProductionLabel = False
  '        Throw ex

  '    End Try

  'End Function

  '<WebMethod(Description:="Bileþenin olup olmadýðýný kontrol eder.")> _
  'Public Function CheckComponent(ByVal Component As String) _
  '                               As Boolean

  '    Try

  '        Dim clsDbConnectionValues As New ConnectionValues
  '        Dim ConnectionString As String = clsDbConnectionValues.ReadConnectionValues(enumSqlConnectionType.Pwms)
  '        Dim db As New Core.DataForGeneral(ConnectionString, Core.DataForGeneral.ProviderTypes.OleDb)
  '        Dim dt As New DataTable
  '        Dim sStr As String

  '        sStr = " select isnull(Count(Itnbr),0) from dbo.Z_ED_LBLMST with (nolock)" & _
  '               " where Labelno=" & IIf(String.IsNullOrEmpty(Component), 0, Component)
  '        Dim CntItnbr As Long = CLng(db.RunSql(sStr, True))

  '        If CntItnbr = 0 Then
  '            'Return False
  '            Throw New Exception("|Bileþen bulunamadý !|") ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '            'Return "|Bileþen bulunamadý !|" ' -- | -- Mobil tarafýnda meþajý pars etmek için
  '        End If

  '        Return True

  '    Catch ex As Exception
  '        'CheckComponent = False
  '        Throw ex

  '    End Try

  'End Function

  '__________________________________________________________________________________


#End Region

End Class
