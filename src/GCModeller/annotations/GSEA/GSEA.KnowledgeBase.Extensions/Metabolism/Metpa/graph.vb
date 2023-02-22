Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace Metabolism.Metpa

    Public Class graph

        Public Property g As NetworkGraph

        Sub New()
        End Sub

        Sub New(g As NetworkGraph)
            Me.g = g
        End Sub

    End Class

    Public Class graphList

        Public Property graphs As Dictionary(Of String, graph)

    End Class
End Namespace