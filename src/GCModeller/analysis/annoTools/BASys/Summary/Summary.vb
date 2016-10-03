#Region "Microsoft.VisualBasic::804a894472e13559edb970d9fcc1e0c4, ..\GCModeller\analysis\annoTools\BASys\Summary\Summary.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
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
            DataFrameColumnAttribute.LoadMapping(Of Summary)(, True) _
            .ToDictionary(Function(x) x.Value.Field.Name,
                          Function(x) x.Value)
        Dim summary As New Summary

        For Each row As String In rows
            Dim cols As String() = row.GetColumnsHTML
            Dim key As String = cols(Scan0)
            Dim value As String = cols(1)

            key = key.TrimEnd(":"c)
            Call schema(key).SetValue(summary, value)
        Next

        Return summary
    End Function
End Class

