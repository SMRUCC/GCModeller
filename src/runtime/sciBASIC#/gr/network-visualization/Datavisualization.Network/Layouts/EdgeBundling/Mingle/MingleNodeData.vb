Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Layouts.EdgeBundling.Mingle

    Public Class MingleNodeData : Inherits NodeData

        Public Property nodes As Node()
        Public Property m1 As Double()
        Public Property m2 As Double()
        Public Property coords As Double()
        Public Property bundle As Node
        Public Property ink As New Value(Of Double)
        Public Property parents As Node()
        Public Property nodeArray As Node()
        Public Property parentsInk As Double
        Public Property group As Integer

        Sub New()
        End Sub

        Sub New(copy As NodeData)
            Call MyBase.New(copy)
        End Sub

        Public Overrides Function Clone() As NodeData
            Return New MingleNodeData With {
                .betweennessCentrality = betweennessCentrality,
                .bundle = bundle,
                .color = color,
                .coords = coords.SafeQuery.ToArray,
                .force = force,
                .group = group,
                .initialPostion = initialPostion + 0,
                .ink = ink?.Value,
                .label = label,
                .m1 = m1.SafeQuery.ToArray,
                .m2 = m2.SafeQuery.ToArray,
                .mass = mass,
                .neighbours = neighbours.SafeQuery.ToArray,
                .nodeArray = nodeArray.SafeQuery.ToArray,
                .nodes = nodes.SafeQuery.ToArray,
                .origID = origID,
                .parents = parents.SafeQuery.ToArray,
                .parentsInk = parentsInk,
                .Properties = New Dictionary(Of String, String)(Properties),
                .size = size.SafeQuery.ToArray,
                .weights = weights.SafeQuery.ToArray
            }
        End Function
    End Class

    Public Class MingleData
        Public Property bundle As Node()
        Public Property inkTotal As Double
        Public Property combined As Node
    End Class
End Namespace