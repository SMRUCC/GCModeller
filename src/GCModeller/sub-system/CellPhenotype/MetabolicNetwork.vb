Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

''' <summary>
''' 代谢网络邻接表结构
''' </summary>
Public Class MetabolicNetwork

    ''' <summary>
    ''' 邻接表：Adj(i) 存储从节点 i 出发的所有目标节点及权重
    ''' </summary>
    Public Property Adjacency As Dictionary(Of String, AdjacencyWeight())

    Public Property MetaIDs As String()

    Public ReadOnly Property NodeCount As Integer
        Get
            Return Adjacency.Count
        End Get
    End Property

    ''' <summary>
    ''' 将代谢网络转换为行随机化转移矩阵 P
    ''' </summary>
    Public Function BuildRowStochasticMatrix() As Double(,)
        Dim n As Integer = NodeCount
        Dim P(n - 1, n - 1) As Double
        Dim ids As String() = MetaIDs
        Dim ordinal As Index(Of String) = ids.Indexing

        For i As Integer = 0 To n - 1
            Dim totalOutWeight As Double = 0.0
            For Each edge As AdjacencyWeight In Adjacency(ids(i))
                totalOutWeight += edge.Weight
            Next

            If totalOutWeight > 0 Then
                For Each edge As AdjacencyWeight In Adjacency(ids(i))
                    P(i, ordinal(edge.Target)) = edge.Weight / totalOutWeight
                Next
            Else
                ' 无出边：自环（避免死端）
                P(i, i) = 1.0
            End If
        Next

        Return P
    End Function

    Public Shared Function Create(g As NetworkGraph) As MetabolicNetwork
        Dim adj As New Dictionary(Of String, AdjacencyWeight())
        Dim idList As New List(Of String)

        ' G = {U->V}
        ' current node v is U
        ' adjacency of U->V
        For Each v As Node In g.vertex
            Call idList.Add(v.label)
            Call adj.Add(v.label,
                value:=v.adjacencies _
                    .EnumerateAllEdges _
                    .Select(Function(edge)
                                Return New AdjacencyWeight With {
                                    .Target = edge.V.label,
                                    .Weight = edge.weight
                                }
                            End Function) _
                    .ToArray)
        Next

        Return New MetabolicNetwork With {
            .Adjacency = adj,
            .MetaIDs = idList.ToArray
        }
    End Function

End Class

Public Class AdjacencyWeight

    Public Property Target As String
    Public Property Weight As Double

    Sub New()
    End Sub

    Sub New(target As String, Optional w As Double = 1)
        Me.Target = target
        Me.Weight = w
    End Sub

    Public Overrides Function ToString() As String
        Return $"{Target} = {Weight}"
    End Function

End Class