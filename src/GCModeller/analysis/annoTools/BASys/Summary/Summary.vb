Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.HtmlParser
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' [index] BASys Annotation Summary
''' </summary>
Public Class Summary : Inherits ClassObject

    ''' <summary>
    ''' Chromosome Id
    ''' </summary>
    ''' <returns></returns>
    <Field("Chromosome Id")>
    Public Property chrId As String
    Public Property Length As String
    ''' <summary>
    ''' Gram Stain
    ''' </summary>
    ''' <returns></returns>
    <Field("Gram Stain")> Public Property GramStain As String
    Public Property Topology As String
    Public Property Genus As String
    Public Property Species As String
    Public Property Strain As String

    ''' <summary>
    ''' Number of Genes Identified
    ''' </summary>
    ''' <returns></returns>
    <Field("Number of Genes Identified")>
    Public Property gIdentified As String
    ''' <summary>
    ''' Number of Genes Annotated
    ''' </summary>
    ''' <returns></returns>
    <Field("Number of Genes Annotated")>
    Public Property gAnnotated As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function IndexParser(path As String) As Summary
        Dim html As String = path.GET
        html = Strings.Split(html, "<!-- MAIN TABLE MAIN COLUMN -->").Last
        html = Regex.Match(html, "<table>.+?</table>", RegexICSng).Value
        Dim rows = html.GetRowsHTML
        Dim schema =
            DataFrameColumnAttribute.LoadMapping(Of Summary)(, True)
        Dim summary As New Summary

        For Each row As String In rows
            Dim cols As String() = row.GetColumnsHTML
            Dim key As String = cols(Scan0)
            Dim value As String = cols(1)

        Next

        Return summary
    End Function
End Class
