Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

Namespace SBGN

    Public Class arc

        <XmlAttribute> Public Property [class] As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property source As String
        <XmlAttribute> Public Property target As String

        Public Property glyph As glyph

        Public Property start As point

        <XmlElement>
        Public Property [next] As point()

        Public Property [end] As point

        Public Overrides Function ToString() As String
            Return $"{source} -> {target}"
        End Function

        Public Function GetPolygon() As Polygon2D
            Dim points = GetPoints.ToArray
            Dim x = points.Select(Function(pt) pt.x).ToArray
            Dim y = points.Select(Function(pt) pt.y).ToArray

            Return New Polygon2D(x, y)
        End Function

        Private Iterator Function GetPoints() As IEnumerable(Of point)
            If start IsNot Nothing Then
                Yield start
            End If

            For Each point As point In [next].SafeQuery
                Yield point
            Next

            If [end] IsNot Nothing Then
                Yield [end]
            End If
        End Function

    End Class

    Public Class point

        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double

        Public Overrides Function ToString() As String
            Return $"[{x},{y}]"
        End Function

    End Class
End Namespace