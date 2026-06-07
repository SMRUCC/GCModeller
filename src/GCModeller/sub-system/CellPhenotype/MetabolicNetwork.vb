' 代谢网络邻接表结构
Public Class MetabolicNetwork
    Public NodeCount As Integer
    ' 邻接表：Adj(i) 存储从节点 i 出发的所有目标节点及权重
    Public Adjacency As List(Of List(Of (Target As Integer, Weight As Double)))
End Class