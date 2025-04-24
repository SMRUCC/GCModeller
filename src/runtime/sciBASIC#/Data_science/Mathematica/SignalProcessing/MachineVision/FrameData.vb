Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class FrameData : Implements Enumeration(Of Detection)

    <XmlAttribute> Public Property FrameID As Integer
    <XmlElement("Objects")>
    Public Property Detections As Detection()

    Sub New()
    End Sub

    Sub New(id As Integer, detections As IEnumerable(Of Detection))
        _FrameID = id
        _Detections = detections.SafeQuery.ToArray
    End Sub

    Public Function SetIndex(id As Integer) As FrameData
        FrameID = id
        Return Me
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"#{FrameID} " & Detections _
            .SafeQuery _
            .Select(Function(a) a.ObjectID) _
            .GetJson
    End Function

    Public Iterator Function GenericEnumerator() As IEnumerator(Of Detection) Implements Enumeration(Of Detection).GenericEnumerator
        For Each obj As Detection In Detections.SafeQuery
            Yield obj
        Next
    End Function
End Class

Public Class Detection

    <XmlAttribute>
    Public Property ObjectID As String
    Public Property Position As PointF

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"{ObjectID} [x:{Position.X}, y:{Position.Y}]"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Narrowing Operator CType(obj As Detection) As PointF
        Return obj.Position
    End Operator
End Class

