Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace SBGN

    Public Class glyph

        <XmlAttribute> Public Property [class] As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property compartmentOrder As String

        Public Property label As label
        Public Property bbox As bbox

        <XmlElement>
        Public Property port As port()

        Public Overrides Function ToString() As String
            Return $"({[class]}) {id}: {label}"
        End Function

    End Class

    Public Class port

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double

    End Class

    Public Class label

        <XmlAttribute>
        Public Property text As String
        Public Property bbox As bbox

        Public Overrides Function ToString() As String
            Return text
        End Function

    End Class

    Public Class bbox

        <XmlAttribute> Public Property w As Double
        <XmlAttribute> Public Property h As Double
        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double

        Public Overrides Function ToString() As String
            Return $"({x},{y}) {{w:{w}, h:{h}}}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(bbox As bbox) As RectangleF
            If bbox Is Nothing Then
                Return Nothing
            Else
                Return New RectangleF(bbox.x, bbox.y, bbox.w, bbox.h)
            End If
        End Operator

    End Class
End Namespace