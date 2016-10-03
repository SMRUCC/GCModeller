#Region "Microsoft.VisualBasic::9715e95695ea3d76f80071640a6e6afd, ..\workbench\d3js\d3svg\d3svg\ForceDirectedGraph.vb"

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

Public Class ForceDirectedGraph : Inherits D3Parser

    Const Style As String = "<style.*?>.+?</style>"

    Protected Overrides Function __css(html As String) As String
        Dim style As String = Regex.Match(html, ForceDirectedGraph.Style, RegexICSng).Value
        style = style.GetValue
        Return style
    End Function

    Const SVG As String = "<svg .+?</svg>"

    Protected Overrides Function __svgNode(html As String) As String
        Dim svg As String = Regex.Match(html, ForceDirectedGraph.SVG, RegexICSng).Value
        Return svg
    End Function
End Class

