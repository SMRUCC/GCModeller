Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace Layouts.Radial

    Public Module RadialLayout

        Public Function LayoutNodes(g As NetworkGraph) As NetworkGraph
            ' 首先计算出所有节点的连接度
            ' 将最高连接度的节点作为布局的中心点
            Dim degreeOrders = g.ComputeNodeDegrees.OrderByDescending(Function(a) a.Value).ToArray
            Dim layout = g.layoutCurrentCenter(cid:=degreeOrders.First.Key, degreeOrders.ToDictionary)

            Return layout
        End Function

        <Extension>
        Private Function layoutCurrentCenter(g As NetworkGraph, cid As String, degrees As Dictionary(Of String, Integer)) As NetworkGraph
            ' 其余的节点与中心节点的距离与度有关，度越高距离越远
            Dim current As Node = g.GetElementByID(cid)
            Dim connected = g.GetEdges(current).Select(Function(a) a.other(current)).orderby()
        End Function
    End Module
End Namespace