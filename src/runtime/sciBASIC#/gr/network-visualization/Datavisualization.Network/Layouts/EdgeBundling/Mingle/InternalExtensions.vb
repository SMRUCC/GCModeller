Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdNum = System.Math
Imports number = System.Double

Namespace Layouts.EdgeBundling.Mingle

    Module InternalExtensions

        Public ReadOnly MINGLE_PHI As Double = (1 + stdNum.Sqrt(5)) / 2

        Public Function dist(a As number(), b As number()) As number
            Dim diffX = a(0) - b(0)
            Dim diffY = a(1) - b(1)

            Return stdNum.Sqrt(diffX * diffX + diffY * diffY)
        End Function

        Public Function lerp(a As Vector, b As Vector, delta As Double) As Vector
            Return a * (1 - delta) + b * delta
        End Function

        Public Function norm(a As number()) As number
            Return stdNum.Sqrt(a(0) * a(0) + a(1) * a(1))
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