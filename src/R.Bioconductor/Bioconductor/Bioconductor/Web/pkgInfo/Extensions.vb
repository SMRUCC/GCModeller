﻿#Region "Microsoft.VisualBasic::b1291ca9d010b5af046aad69272a9e66, Bioconductor\Bioconductor\Web\pkgInfo\Extensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Module Extensions
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __buildHash, ArchivesParser, DescriptionParser, DetailsParser, (+2 Overloads) GetDetails
    '                   GetURL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Text.HtmlParser

Namespace Web.Packages

    Module Extensions

        Sub New()
            On Error Resume Next

            Call My.Resources.bioconductor.SaveTo(App.HOME & "/assets/bioconductor.css")
            Call My.Resources.bioconductor1.SaveTo(App.HOME & "/assets/js/bioconductor.js")
            Call My.Resources.bioc_style.SaveTo(App.HOME & "/assets/js/bioc-style.js")

            Using ico As FileStream = File.Open(App.HOME & "/assets/favicon.ico", FileMode.OpenOrCreate)
                Call My.Resources.favicon.Save(ico)
            End Using
        End Sub

        <Extension>
        Public Function GetURL(pack As Package) As String
            If pack.Category = BiocTypes.bioc Then
                Return $"http://master.bioconductor.org/packages/release/{pack.Category.ToString}/html/{pack.Package}.html"
            Else
                Return $"http://master.bioconductor.org/packages/release/data/{pack.Category.ToString}/html/{pack.Package}.html"
            End If
        End Function

        <Extension>
        Public Function GetDetails(ByRef pkg As Package) As Package
            Return pkg.GetDetails(pkg.GetURL.GET)
        End Function

        <Extension>
        Public Function GetDetails(ByRef pkg As Package, html As String) As Package
            Dim parts = Strings.Split(html, "<div class='do_not_rebase'>")
            html = parts.Last
            parts = Strings.Split(html, "<table>")

            pkg.Description = DescriptionParser(parts(Scan0), pkg.Package)
            pkg.Details = DetailsParser(parts(1))
            pkg.Archives = ArchivesParser(pkg.Category, parts(2))

            Return pkg
        End Function

        Const Base As String = "http://bioconductor.org/packages/{0}/"

        Public Function DetailsParser(pageHTML As String) As Details
            Dim parts As String() = Strings.Split(pageHTML, "<h3>Details</h3>")
            parts = Strings.Split(parts.Last, "</table>")

            Dim hash As Dictionary(Of String, String) = __buildHash(parts.First)
            Dim props As PropertyInfo() = GetType(Details).GetReadWriteProperties
            Dim info As New Details

            For Each prop As PropertyInfo In props
                Dim column = prop.GetAttribute(Of ColumnAttribute)
                If Not column Is Nothing Then
                    If hash.ContainsKey(column.Name) Then
                        Dim value As String = hash(column.Name)
                        value = value.Replace("../../", Base)
                        Call prop.SetValue(info, value)
                    End If
                End If
            Next

            Return info
        End Function

        Private Function __buildHash(html As String) As Dictionary(Of String, String)
            Dim parts = Regex.Matches(html, "<tr.+?</tr>", RegexOptions.Singleline Or RegexOptions.IgnoreCase).ToArray
            Dim hash = (From row As String In parts
                        Let ms As String() = Regex.Matches(row, "<td.+?</td>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).ToArray
                        Where ms.Length > 1
                        Select key = ms.First.GetValue,
                            value = ms.Last.GetValue) _
                           .ToDictionary(Function(x) x.key,
                                         Function(x) x.value)
            Return hash
        End Function

        Public Function ArchivesParser(type As BiocTypes, pageHTML As String) As Archives
            Dim parts As String() = Strings.Split(pageHTML, "</table>")
            Dim hash As Dictionary(Of String, String) = __buildHash(parts.First)
            Dim props As PropertyInfo() = GetType(Archives).GetReadWriteProperties
            Dim info As New Archives
            Dim base As String = Extensions.Base & If(type = BiocTypes.bioc, type.ToString, "data/" & type.ToString) & "/"

            For Each prop As PropertyInfo In props
                Dim column = prop.GetAttribute(Of ColumnAttribute)
                If Not column Is Nothing Then
                    If hash.ContainsKey(column.Name) Then
                        Dim value As String = hash(column.Name)
                        value = value.Replace("../", base)
                        Call prop.SetValue(info, value)
                    End If
                End If
            Next

            Return info
        End Function

        Public Function DescriptionParser(pageHTML As String, pkg As String) As String
            Dim html As New StringBuilder(My.Resources.Templates)
            Call html.Replace("{Package}", pkg)
            Call html.Replace("{Description}", pageHTML)
            Return html.ToString
        End Function
    End Module
End Namespace
