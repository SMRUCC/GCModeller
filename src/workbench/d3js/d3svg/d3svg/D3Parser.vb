#Region "Microsoft.VisualBasic::5ac6caa139f4fb4ce64d588279b8d5de, ..\workbench\d3js\d3svg\d3svg\D3Parser.vb"

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

Imports System.Drawing
Imports System.Text.RegularExpressions

Public MustInherit Class D3Parser

    Public Function HtmlParser(html As String) As SVG
        Dim svg As New SVG With {
            .CSS = __css(html),
            .SVGContent = __svgNode(html)
        }
        svg.Size = __trimSVG(svg.SVGContent)
        Return svg
    End Function

    Public Function HtmlFileParser(path As String) As SVG
        Return HtmlParser(path.GET)
    End Function

    Private Function __trimSVG(ByRef doc As String) As Size
        Dim head As String = Regex.Match(doc, "<svg .*?>", RegexICSng).Value
        Dim width As String = Regex.Match(head, "width="".+?""", RegexICSng).Value
        Dim height = Regex.Match(head, "height="".+?""", RegexICSng).Value

        doc = doc.GetValue
        width = Regex.Match(width, "\d+").Value
        height = Regex.Match(height, "\d+").Value

        Return New Size(Scripting.CTypeDynamic(Of Integer)(width), Scripting.CTypeDynamic(Of Integer)(height))
    End Function

    Protected MustOverride Function __css(html As String) As String
    Protected MustOverride Function __svgNode(html As String) As String
End Class

