Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.KEGG.DBGET
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Compiler.Components

    Public Class DownloadedData
        <XmlAttribute> Public Property MetaCycId As String
        <XmlAttribute> Public Property CommonName As String
        Public Property KEGGCompounds As bGetObject.Compound
    End Class

    <PackageNamespace("Model.Retrive_Info")>
    Public Module DownloadsAPI

        ''' <summary>
        ''' 返回所成功下载的数目
        ''' </summary>
        ''' <param name="ModelLoader"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("downloads_for")>
        Public Function Invoke(ModelLoader As FileStream.IO.XmlresxLoader, sourceFrom As String) As Integer
            Return New MetaboliteInformationDownloader(sourceFrom).Match(ModelLoader)
        End Function
    End Module
End Namespace