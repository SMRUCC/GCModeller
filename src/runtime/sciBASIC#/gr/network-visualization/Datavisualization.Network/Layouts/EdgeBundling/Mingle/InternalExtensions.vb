Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports stdNum = System.Math

Namespace Layouts.EdgeBundling.Mingle

    Module InternalExtensions

        Public ReadOnly PHI As Double = (1 + stdNum.Sqrt(5)) / 2

        Public Function lerp(a As PointF, b As PointF, delta As Double) As PointF
            Return New PointF With {
                .X = a.X * (1 - delta) + b.X * delta,
                .Y = a.Y * (1 - delta) + b.Y * delta
            }
        End Function

        <Extension>
        Public Sub [Each](g As NetworkGraph, apply As Action(Of Node))
            For Each v As Node In g.vertex
                Call apply(v)
            Next
        End Sub

        <Extension>
        Public Sub [Each](g As NetworkGraph, apply As Action(Of Edge))
            For Each link As Edge In g.graphEdges
                Call apply(link)
            Next
        End Sub
    End Module
End Namespace