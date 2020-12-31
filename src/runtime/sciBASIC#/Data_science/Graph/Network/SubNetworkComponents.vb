Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Network

    Public Class SubNetworkComponents(Of Node As {New, Network.Node}, U As {New, Network.Edge(Of Node)}, Graph As {New, NetworkGraph(Of Node, U)})
        Implements IEnumerable(Of Graph)

        Dim edges As List(Of U)
        Dim network As NetworkGraph(Of Node, U)
        Dim components As Graph()
        Dim populatedNodes As New List(Of Node)

        Sub New(network As NetworkGraph(Of Node, U), Optional singleNodeAsGraph As Boolean = False)
            Me.network = network
            Me.edges = network.edges.Values.AsList
            Me.components = IteratesSubNetworks.ToArray

            If singleNodeAsGraph Then
                Me.components = Me.components _
                    .JoinIterates(GetSingleNodeGraphs) _
                    .ToArray
            End If
        End Sub

        Public Iterator Function GetEnumerator() As IEnumerator(Of Graph) Implements IEnumerable(Of Graph).GetEnumerator
            For Each g As Graph In components
                Yield g
            Next
        End Function

        Private Iterator Function GetSingleNodeGraphs() As IEnumerable(Of Graph)
            Dim removedIndex As Index(Of Node) = populatedNodes.Distinct.Indexing
            Dim [single] As New Graph

            For Each v As Node In network.vertex.Where(Function(n) removedIndex(n) = -1)
                [single] = New Graph
                [single].AddVertex(v)

                Yield [single]
            Next
        End Function

        Private Function popFirstEdge(n As Node) As U
            Return edges _
                .Where(Function(e) e.U Is n OrElse e.V Is n) _
                .FirstOrDefault
        End Function

        Private Function measureSubComponent() As Graph
            Dim subnetwork As New Graph
            Dim edge As U = edges.First
            Dim list As New List(Of Node)

            Call list.Add(edge.U)
            Call list.Add(edge.V)

            Do While list > 0
                ' U和V是由edge带进来的，可能会产生重复
                subnetwork.AddVertex(edge.U)
                subnetwork.AddVertex(edge.V)
                subnetwork.AddEdge(edge.U, edge.V)
                populatedNodes.Add(edge.U)
                populatedNodes.Add(edge.V)
                edges.Remove(edge)

                If -1 = list.IndexOf(edge.U) Then
                    Call list.Add(edge.U)
                End If
                If -1 = list.IndexOf(edge.V) Then
                    Call list.Add(edge.V)
                End If

                edge = Nothing

                Do While edge Is Nothing AndAlso list > 0
                    edge = popFirstEdge(list.First)

                    If edge Is Nothing Then
                        ' 当前的这个节点已经没有相连的边了，移除这个节点
                        Call list.RemoveAt(Scan0)
                    End If
                Loop
            Loop

            Return subnetwork
        End Function

        Private Iterator Function IteratesSubNetworks() As IEnumerable(Of Graph)
            Do While edges > 0
                Yield measureSubComponent()
            Loop
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace