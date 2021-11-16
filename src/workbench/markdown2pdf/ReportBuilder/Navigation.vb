#Region "Microsoft.VisualBasic::46acb5d4b6470e2f504811ddbf8a75db, markdown2pdf\ReportBuilder\Navigation.vb"

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

    ' Module Navigation
    ' 
    '     Properties: MEMESWTomQuery, MEMETomQuery
    ' 
    '     Function: BreadcrumbNavigation
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Public Module Navigation

    ''' <summary>
    ''' {display, url}
    ''' </summary>
    ''' <param name="nag">{display, url}</param>
    ''' <returns></returns>
    Public Function BreadcrumbNavigation(current As String, nag As Dictionary(Of String, String)) As String
        Dim nbr As StringBuilder = New StringBuilder(1024)

        Call nbr.AppendLine("<p>")
        Call nbr.AppendLine("<a href=""http://gcmodeller.org"">HOME</a> > ")

        For Each lv In nag
            Call nbr.AppendLine($"<a href=""{lv.Value}"">{lv.Key}</a> / ")
        Next

        Call nbr.AppendLine($"<strong>{current}</strong>")
        Call nbr.AppendLine("</p>")

        Return nbr.ToString
    End Function

    Public ReadOnly Property MEMESWTomQuery As String =
        BreadcrumbNavigation("Show Result", New Dictionary(Of String, String) From {
            {"Services", "http://services.gcmodeller.org"},
            {"MEME", "http://services.gcmodeller.org/meme/"},
            {"TomQuery", "http://services.gcmodeller.org/meme/tom-query.sw/"}})

    Public ReadOnly Property MEMETomQuery As String =
        BreadcrumbNavigation("Show Result", New Dictionary(Of String, String) From {
            {"Services", "http://services.gcmodeller.org"},
            {"MEME", "http://services.gcmodeller.org/meme/"},
            {"TomQuery", "http://services.gcmodeller.org/meme/tom-query/"}})
End Module
