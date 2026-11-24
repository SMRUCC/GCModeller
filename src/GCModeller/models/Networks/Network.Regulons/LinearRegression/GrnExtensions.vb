Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 基因调控网络的快捷入口
    ''' </summary>
    <HideModuleName>
    Public Module GrnExtensions

        ''' <summary>
        ''' 基于线性拟合从表达矩阵之中推断基因调控网络
        ''' 
        ''' 每一个基因都会轮流作为目标基因被拟合一次：
        ''' 
        ''' ```
        ''' expr(target) = const + sum(beta_i * expr(regulator_i))
        ''' ```
        ''' </summary>
        ''' <param name="expr">行是基因、列是样本的表达矩阵</param>
        ''' <param name="options">线性拟合的参数配置</param>
        ''' <returns>已经完成拟合的网络对象</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function InferRegulatoryNetwork(expr As Matrix, Optional options As GrnFittingOptions = Nothing) As GeneRegulatoryNetwork
            Dim grn As New GeneRegulatoryNetwork(expr, options)

            Return grn.Fit()
        End Function

        ''' <summary>
        ''' 把线性模型推断出来的调控边转换为本模块既有的
        ''' <see cref="Connection"/> 相关性边对象，从而可以直接复用
        ''' 工程内部已有的网络导出与可视化流程。
        ''' </summary>
        ''' <param name="edges">线性模型推断出来的调控边</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <see cref="Connection.cor"/> 之中保存的是回归系数，
        ''' <see cref="Connection.pval"/> 之中保存的是显著性 p 值。
        ''' </remarks>
        <Extension>
        Public Iterator Function ToConnections(edges As IEnumerable(Of RegulationEdge)) As IEnumerable(Of Connection)
            For Each edge As RegulationEdge In edges
                Yield New Connection With {
                    .gene1 = edge.regulator,
                    .gene2 = edge.target,
                    .cor = edge.weight,
                    .pval = edge.pvalue,
                    .is_directly = True,
                    .interaction = edge.interaction
                }
            Next
        End Function

        ''' <summary>
        ''' 按照回归系数绝对值降序取最强的若干条调控边
        ''' </summary>
        ''' <param name="edges"></param>
        ''' <param name="top">需要保留的边的数量</param>
        ''' <returns></returns>
        <Extension>
        Public Function TopEdges(edges As IEnumerable(Of RegulationEdge), Optional top As Integer = 100) As RegulationEdge()
            If top <= 0 Then
                Return edges.ToArray
            Else
                Return edges _
                    .OrderByDescending(Function(e) std.Abs(e.weight)) _
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
        Public Iterator Function RegulonSizes(edges As IEnumerable(Of RegulationEdge)) As IEnumerable(Of (regulator As String, size As Integer))
            Dim counts = edges _
                .GroupBy(Function(e) e.regulator) _
                .ToDictionary(Function(g) g.Key, Function(g) g.Count)

            For Each key As String In counts.Keys _
                .OrderByDescending(Function(id) counts(id)) _
                .ToArray

                Yield (key, counts(key))
            Next
        End Function
    End Module
End Namespace
