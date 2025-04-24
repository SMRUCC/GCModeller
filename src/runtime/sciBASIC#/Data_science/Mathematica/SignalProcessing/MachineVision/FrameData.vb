Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class FrameData

    <XmlAttribute> Public Property FrameID As Integer
    <XmlElement("Objects")>
    Public Property Detections As Detection()

    Public Overrides Function ToString() As String
        Return $"#{FrameID} " & Detections _
            .SafeQuery _
            .Select(Function(a) a.ObjectID) _
            .GetJson
    End Function

End Class

Public Class Detection

    <XmlAttribute> Public Property ObjectID As String
    Public Property Position As PointF

    Public Overrides Function ToString() As String
        Return $"{ObjectID} [x:{Position.X}, y:{Position.Y}]"
    End Function
End Class

