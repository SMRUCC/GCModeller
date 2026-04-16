Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.MetabolicModel

Public Class Pathway : Inherits MetabolicPathway

    ''' <summary>
    ''' 反应网络
    ''' </summary>
    ''' <returns></returns>
    Public Property ReactionNetwork As NetworkGraph

    Sub New(network As IReadOnlyCollection(Of MetabolicReaction))
        Dim g As New NetworkGraph

        For Each u As MetabolicReaction In network
            Dim right = u.right.ToDictionary(Function(specie) specie.ID)
            Dim uNode As Node = g.GetElementByID(u.id)

            If uNode Is Nothing Then
                uNode = g.CreateNode(u.id)
            End If

            For Each v As MetabolicReaction In network
                If u Is v Then
                    Continue For
                End If

                Dim vNode As Node = g.GetElementByID(v.id)

                If vNode Is Nothing Then
                    vNode = g.CreateNode(v.id)
                End If

                If v.left.Any(Function(specie) right.ContainsKey(specie.ID)) Then
                    Call g.CreateEdge(uNode, vNode)
                End If
            Next
        Next

        metabolicNetwork = network.ToArray
        ReactionNetwork = g
    End Sub

End Class
