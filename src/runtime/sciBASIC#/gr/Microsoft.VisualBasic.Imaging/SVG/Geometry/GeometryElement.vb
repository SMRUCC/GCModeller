Imports System.Xml
Imports Microsoft.VisualBasic.Imaging.SVG.XML

Namespace SVG

    Public Class GeometryElement : Implements Layout2D

        Public Property X As Double Implements Layout2D.X
        Public Property Y As Double Implements Layout2D.Y
        Public Property svgElement As SvgElement

        Public Shared Iterator Function LoadElements(svg As SvgContainer, offsetX As Double, offsetY As Double) As IEnumerable(Of GeometryElement)
            Dim list As XmlNodeList = svg.GetSvgElement.ChildNodes
            Dim node As XmlElement

            For i As Integer = 0 To list.Count - 1
                node = TryCast(list(i), XmlElement)

                If Not node Is Nothing Then
                    If node.Name = "i:pgf" Then
                        Continue For
                    End If

                    Dim svgElement As SvgElement = SvgElement.Create(node)
                    Dim x As Double = Val(svgElement("x"))
                    Dim y As Double = Val(svgElement("y"))
                    Dim transform As New transform(svgElement("transform"))

                    If TypeOf svgElement Is SvgGroup Then

                    End If
                End If
            Next
        End Function
    End Class
End Namespace