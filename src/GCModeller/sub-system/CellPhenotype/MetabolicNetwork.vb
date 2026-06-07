''' <summary>
''' 代谢网络邻接表结构
''' </summary>
Public Class MetabolicNetwork

    Public Property NodeCount As Integer
    ''' <summary>
    ''' 邻接表：Adj(i) 存储从节点 i 出发的所有目标节点及权重
    ''' </summary>
    Public Property Adjacency As List(Of List(Of (Target As Integer, Weight As Double)))

End Class