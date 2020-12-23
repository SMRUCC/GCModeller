Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace Layouts.EdgeBundling.Mingle

    Public Class GraphKdNode

        Friend x, y, z, w As Double
        Friend v As Node

        Sub New()
        End Sub

        Sub New(v As Node)
            Me.v = v
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{x}, {y}, {z}, {w}]"
        End Function

    End Class
End Namespace