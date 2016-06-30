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
