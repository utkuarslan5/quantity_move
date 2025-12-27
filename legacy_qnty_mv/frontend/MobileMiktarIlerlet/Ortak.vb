Imports System.Xml
Imports System.IO



Module Ortak

    Public Function ReadConfig(ByVal ConfigName As String) As String
        Try
            Dim ConfigValue1 As String
            'Dim XmlReader As New XmlTextReader("\Application\TestMobilHareketGirisleri\Config.xml")
            'Dim XmlReader As New XmlTextReader("\Program Files\MobilHareketGirisleri\Config.xml")
            Dim XmlReader As New XmlTextReader(StartupPath() & "\Config.xml")
            Dim XmlDocument As New XmlDocument
            XmlDocument.Load(XmlReader)

            ConfigValue1 = XmlDocument.Item("root").Item(ConfigName).InnerText

            Return ConfigValue1

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function StartupPath() As String

        Try

            'MessageType = enumMessageType.SystemHatası

            Dim TmpPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.GetName.CodeBase)

            Return IIf(TmpPath.IndexOf("file:\").ToString <> "-1", Microsoft.VisualBasic.Right(TmpPath, Len(TmpPath) - 6), TmpPath)

        Catch ex As Exception
            Throw ex

        End Try

    End Function

End Module
