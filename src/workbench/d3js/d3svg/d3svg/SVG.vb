Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Web.Script.Serialization

Public Class SVG

    Public Property CSS As String
    Public Property Width As Integer
    Public Property Height As Integer
    Public Property SVGContent As String

    <ScriptIgnore> Public Property Size As Size
        Get
            Return New Size(Width, Height)
        End Get
        Friend Set(value As Size)
            Width = value.Width
            Height = value.Height
        End Set
    End Property
End Class

Public Module SVGBuilder

    Const XmlHead As String = "<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>
<!DOCTYPE svg PUBLIC ""-//W3C//DTD SVG 1.1//EN"" ""http//www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd"">
"
    Const CSS As String = "<defs><style type=""text/css""><![CDATA[
{0}
]]></style></defs>"

    Const SVGRoot As String = "<svg width=""{0}px"" height=""{1}px"" version=""1.1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"">"
    Const SVGRootNoNamespace As String = "<svg width=""{0}px"" height=""{1}px"" version=""1.1"">"

    ''' <summary>
    ''' Generate svg document from the SVG data model.
    ''' </summary>
    ''' <param name="svg"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Build(svg As SVG) As String
        Return svg.__build(SVGRoot)
    End Function

    <Extension>
    Private Function __build(svg As SVG, svgRoot As String) As String
        Dim sb As New StringBuilder(XmlHead)

        Call sb.AppendLine(String.Format(svgRoot, svg.Width, svg.Height))
        Call sb.AppendLine(String.Format(SVGBuilder.CSS, svg.CSS))
        Call sb.AppendLine(svg.SVGContent)
        Call sb.AppendLine("</svg>")

        Return sb.ToString
    End Function

    <Extension>
    Public Function BuildModel(svg As SVG) As Nodes.SVG
        Dim doc As String = svg.__build(SVGRootNoNamespace)
        Dim build As Nodes.SVG = doc.CreateObjectFromXml(Of Nodes.SVG)
        Return build
    End Function

    ''' <summary>
    ''' Save svg model as svg document.
    ''' </summary>
    ''' <param name="svg"></param>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SaveSVG(svg As SVG, path As String) As Boolean
        Return svg.Build.SaveTo(path, Encoding.UTF8)
    End Function
End Module