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

        Sub New()
        End Sub

        Sub New(copy As NodeData)
            Call MyBase.New(copy)
        End Sub
    End Class
End Namespace