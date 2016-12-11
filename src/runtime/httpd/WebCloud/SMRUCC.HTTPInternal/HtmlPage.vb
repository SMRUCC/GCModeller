#Region "Microsoft.VisualBasic::b88adf02282ffd8bd5bf27a286f2b613, ..\httpd\WebCloud\SMRUCC.HTTPInternal\HtmlPage.vb"

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

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language

Public Class HtmlPage : Inherits ClassObject

    Public Property Url As String
    Public Property Title As String
    Public Property html As String

    Public Function BuildPage(template As String) As String
        Dim sb As New StringBuilder(template)

        Call sb.Replace("{title}", Title)
        Call sb.Replace("{HTML}", html)

        Return sb.ToString
    End Function

    Public Shared Function LoadPage(path As String, wwwroot As String) As HtmlPage
        Dim content As String = path.ReadAllText
        Dim url As String = RelativePath(wwwroot, path)
        Dim html As HtmlPage = HtmlPage.FromStream(content, url)
        Return html
    End Function

    Public Shared Function FromStream(content As String, Optional url As String = "#") As HtmlPage
        Dim head As String = Regex.Match(content, "---.+?---", RegexOptions.Singleline).Value
        Dim title As String = Regex.Match(head, "title:.+?$", RegexICMul).Value

        title = title.GetTagValue(":").Value.Trim
        content = Mid(content, head.Length + 1).Trim

        Return New HtmlPage With {
            .html = content,
            .Title = title,
            .Url = url
        }
    End Function
End Class

