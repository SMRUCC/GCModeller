Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace ARACNE

    ''' <summary>
    ''' ARACNE 网络推断的快捷入口与结果导出扩展
    ''' </summary>
    <HideModuleName>
    Public Module AracneExtensions

        ''' <summary>
        ''' 从基因表达矩阵之中推断 ARACNE 调控网络
        ''' 
        ''' ```
        ''' Dim net = expr.InferAracneNetwork(New AracneOptions With {
        '''     .transcriptionFactors = tfs,
        '''     .miEstimator = MiEstimator.GaussianCopula
        ''' })
        ''' ```
        ''' </summary>
        ''' <param name="expr">行是基因、列是样本的基因表达矩阵</param>
        ''' <param name="options">推断参数，为空的时候使用默认参数</param>
        ''' <returns>已经完成推断的网络对象</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function InferAracneNetwork(expr As Matrix, Optional options As AracneOptions = Nothing) As AracneNetwork
            Return New AracneNetwork(expr, options).Fit()
        End Function

        ''' <summary>
        ''' 把 ARACNE 的调控边转换成工程既有的 <see cref="Connection"/> 网络边对象，
        ''' 从而可以直接复用既有的网络导出与可视化流程。
        ''' </summary>
        ''' <param name="edges">ARACNE 推断出来的调控边</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <see cref="Connection.cor"/> 之中保存的是互信息(也就是边的权重)，
        ''' <see cref="Connection.interaction"/> 之中保存的是方向类型。
        ''' </remarks>
        <Extension>
        Public Iterator Function ToConnections(edges As IEnumerable(Of AracneEdge)) As IEnumerable(Of Connection)
            For Each edge As AracneEdge In edges
                Yield New Connection With {
                    .gene1 = edge.regulator,
                    .gene2 = edge.target,
                    .cor = edge.mi,
                    .pval = edge.pvalue,
                    .is_directly = True,
                    .interaction = edge.interaction
                }
            Next
        End Function

        ''' <summary>
        ''' 按照互信息降序取最强的若干条调控边
        ''' </summary>
        ''' <param name="edges"></param>
        ''' <param name="top">需要保留的边的数量，小于等于零的时候保留全部</param>
        ''' <returns></returns>
        <Extension>
        Public Function TopEdges(edges As IEnumerable(Of AracneEdge), Optional top As Integer = 100) As AracneEdge()
            If top <= 0 Then
                Return edges.ToArray
            Else
                Return edges _
                    .OrderByDescending(Function(e) e.mi) _
                    .Take(top) _
                    .ToArray
            End If
        End Function

        ''' <summary>
        ''' 统计网络之中每一个调控因子的下游基因数量(regulon 的大小)，降序排列。
        ''' </summary>
        ''' <param name="edges"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function RegulonSizes(edges As IEnumerable(Of AracneEdge)) As IEnumerable(Of (regulator As String, size As Integer))
            Dim counts As Dictionary(Of String, Integer) = edges _
                .GroupBy(Function(e) e.regulator) _
                .ToDictionary(Function(g) g.Key, Function(g) g.Count)

            For Each key As String In counts.Keys _
                .OrderByDescending(Function(id) counts(id)) _
                .ToArray

                Yield (key, counts(key))
            Next
        End Function

        ''' <summary>
        ''' 按照互信息排序的边表
        ''' </summary>
        ''' <param name="network"></param>
        ''' <returns></returns>
        <Extension>
        Public Function EdgeTable(network As AracneNetwork) As AracneEdge()
            If network Is Nothing Then
                Return New AracneEdge() {}
            Else
                Return network.Regulations().ToArray
            End If
        End Function
    End Module
End Namespace
