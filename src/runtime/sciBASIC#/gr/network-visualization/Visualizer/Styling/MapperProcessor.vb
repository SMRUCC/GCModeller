Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Styling.CSS

Namespace Styling

    ''' <summary>
    ''' Do style mapping from the parsed css file at here
    ''' </summary>
    Module MapperProcessor

        <Extension>
        Public Function WritePropertyValue(g As NetworkGraph, styles As StyleMapper) As NetworkGraph

        End Function

        ''' <summary>
        ''' 因为在这个模块之中会涉及到修改原来的属性值
        ''' 所以为了不影响原始的数据,
        ''' 在这里必须要在新建的复制副本上来完成操作
        ''' </summary>
        ''' <param name="graph"></param>
        ''' <returns></returns>
        Private Function copy(graph As NetworkGraph) As NetworkGraph
            Dim nodes As Node() = graph.vertex.Select(Function(n As Node)
                                                          Return n.Clone
                                                      End Function).ToArray
        End Function
    End Module
End Namespace