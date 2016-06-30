Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Namespace CEG

    ''' <summary>
    ''' CEG看家基因的功能注释数据
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Annotation : Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.sIdEnumerable

        ''' <summary>
        ''' 本看家基因在CEG数据库之中的唯一编号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property GId As String Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.sIdEnumerable.Identifier
        ''' <summary>
        ''' 基因名
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property GeneName As String
        ''' <summary>
        ''' 功能描述
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}){1}", GeneName, Description)
        End Function

        Public Shared Function LoadDocument(Path As String) As Annotation()
            Dim CsvObject As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File =
                Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Load(Path)
            Dim Title As String() = New String() {"GId", "GeneName", "Description"}

            Call CsvObject.Insert(-1, CType(Title, Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject))
            Dim Data = Microsoft.VisualBasic.DocumentFormat.Csv.Extensions.AsDataSource(Of Annotation)(CsvObject, False)
            Return Data
        End Function
    End Class
End Namespace