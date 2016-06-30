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
