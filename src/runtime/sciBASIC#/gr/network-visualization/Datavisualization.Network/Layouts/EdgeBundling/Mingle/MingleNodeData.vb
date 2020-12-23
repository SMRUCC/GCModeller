Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language

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
    End Class

    Public Class MingleData
        Public Property bundle As Node()
        Public Property inkTotal As Double
        Public Property combined As Node
    End Class
End Namespace