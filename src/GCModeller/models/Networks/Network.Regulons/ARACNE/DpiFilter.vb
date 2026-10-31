Imports std = System.Math

Namespace ARACNE

    ''' <summary>
    ''' DPI 过滤的结果
    ''' </summary>
    Public Class DpiResult

        ''' <summary>
        ''' 过滤之后的互信息矩阵(稀疏结构，未修改输入对象)
        ''' </summary>
        ''' <returns></returns>
        Public Property Matrix As MiMatrix

        ''' <summary>
        ''' 被判定为间接相互作用而被剔除的边(保留剔除之前的互信息值)
        ''' </summary>
        ''' <returns></returns>
        Public Property RemovedEdges As New List(Of MiEdge)

        ''' <summary>
        ''' 满足 DPI 剔除条件、但是因为受到 curated 先验保护而被保留下来的边。
        ''' 
        ''' 这个列表直接量化了先验知识“救回来”的真实调控关系数量，
        ''' 也就是 ARACNE 的 DPI 假设在这里失效的次数。
        ''' </summary>
        ''' <returns></returns>
        Public Property ProtectedEdges As New List(Of MiEdge)

        ''' <summary>
        ''' 检查过的三元组数量(每条边与它的每一个共同邻居构成一个三元组)
        ''' </summary>
        ''' <returns></returns>
        Public Property TrianglesExamined As Integer

        ''' <summary>
        ''' 过滤之前的边数量
        ''' </summary>
        ''' <returns></returns>
        Public Property EdgesBefore As Integer

        ''' <summary>
        ''' 过滤之后的边数量
        ''' </summary>
        ''' <returns></returns>
        Public Property EdgesAfter As Integer

        ''' <summary>
        ''' 实际执行的迭代轮数
        ''' </summary>
        ''' <returns></returns>
        Public Property Passes As Integer

        Public Overrides Function ToString() As String
            Dim prior As String = If(ProtectedEdges.Count = 0, "", $", {ProtectedEdges.Count} rescued by the curated prior")

            Return $"DPI: {EdgesBefore} -> {EdgesAfter} edges, {RemovedEdges.Count} removed{prior}, " &
                $"{TrianglesExamined} triplets in {Passes} pass(es)"
        End Function
    End Class

    ''' <summary>
    ''' 数据处理不等式(Data Processing Inequality)的实现。
    ''' 
    ''' 如果基因之间存在着 ``X -&gt; Y -&gt; Z`` 这样的级联调控关系，那么 ``X`` 与 ``Z`` 之间
    ''' 虽然没有直接的调控作用，它们的表达谱却依然高度相关。DPI 指出对于马尔科夫链
    ''' ``X -&gt; Y -&gt; Z`` 有
    ''' 
    ''' ```
    ''' MI(X, Z) &lt;= min{ MI(X, Y), MI(Y, Z) }
    ''' ```
    ''' 
    ''' 反过来，如果三元组 ``(i, j, k)`` 之中最弱的边 ``MI(i, j)`` 明显低于另外两条边
    ''' ``MI(i, k)`` 与 ``MI(j, k)``，那么 ``(i, j)`` 更可能是由 ``k`` 传递出来的间接边。
    ''' 
    ''' 本模块的判定规则(与 ARACNE 参考实现一致)：
    ''' 
    ''' ```
    ''' MI(i, j) &lt; (1 - epsilon) * min{ MI(i, k), MI(j, k) }  ==>  剔除 (i, j)
    ''' ```
    ''' 
    ''' 其中容差 ``epsilon``(默认 0.1)用于应对有限样本带来的估计误差，
    ''' 保证“接近平局”的三元组不会被误删。
    ''' </summary>
    <HideModuleName>
    Public Module DpiFilter

        ''' <summary>
        ''' 对互信息矩阵执行 DPI 过滤
        ''' </summary>
        ''' <param name="mi">阈值过滤之后的互信息矩阵</param>
        ''' <param name="options">参数配置(容差、DPI 模式与最大迭代轮数)</param>
        ''' <param name="tfFlags">
        ''' 每一个节点是否为转录因子(下标与 <see cref="MiMatrix.names"/> 一致)。
        ''' 
        ''' 只有在 <see cref="DpiMode.TfGuided"/> 模式之下才会被使用，
        ''' 为空的时候按照“全部不是转录因子”处理。
        ''' </param>
        ''' <param name="protect">
        ''' 受保护的边(通常是 curated 先验支持的调控关系)。
        ''' 
        ''' 键值由 <see cref="MiMatrix.EdgeKey(Integer, Integer)"/> 生成，数值是这条边的
        ''' <b>保护强度</b>(取值 ``0 ~ 1``，由先验置信度换算而来)：
        ''' 
        ''' + ``0`` 表示没有保护(与普通边一样只按容差 ``epsilon`` 判定)；
        ''' + ``1`` 表示完全保护(永远不被 DPI 剔除)；
        ''' + 中间值 ``s`` 会把容差放宽到 ``eps + (1 - eps) * s``，
        '''   也就是允许这条边比另外两条边弱 ``eps + (1 - eps) * s`` 的比例。
        ''' 
        ''' 因为强度是<b>逐条边</b>给定的，所以高置信度的先验边拥有更宽的容错窗口，
        ''' 低置信度的先验边只在数据几乎持平时才被保护
        ''' (被否决的边会记录在 <see cref="DpiResult.ProtectedEdges"/> 之中)。
        ''' </param>
        ''' <returns></returns>
        Public Function Apply(mi As MiMatrix,
                              Optional options As AracneOptions = Nothing,
                              Optional tfFlags As Boolean() = Nothing,
                              Optional protect As IDictionary(Of Long, Double) = Nothing) As DpiResult

            Dim opt As AracneOptions = If(options, New AracneOptions())
            Dim mode As DpiMode = opt.EffectiveDpiMode()
            Dim result As New DpiResult With {
                .Matrix = mi,
                .EdgesBefore = mi.Count,
                .EdgesAfter = mi.Count
            }

            If mode = DpiMode.Off OrElse mi.Count = 0 Then
                Return result
            End If

            Dim tolerance As Double = std.Max(0.0R, std.Min(opt.tolerance, 0.999R))
            Dim maxPasses As Integer = std.Max(1, opt.dpiMaxPasses)
            Dim current As MiMatrix = mi
            Dim guard As IDictionary(Of Long, Double) = Nothing

            If opt.protectPriorEdges AndAlso opt.priorProtectionMode <> PriorProtectionMode.Off Then
                guard = protect
            End If

            For pass As Integer = 1 To maxPasses
                Dim removed As HashSet(Of Long) = ScreenPass(current, mode, tolerance, tfFlags, guard, result)

                result.Passes = pass

                If removed.Count = 0 Then
                    Exit For
                End If

                Dim filtered As New MiMatrix(current.names)

                For Each e As MiEdge In current.Edges()
                    If Not removed.Contains(EdgeKey(e.u, e.v)) Then
                        Call filtered.SetValue(e.u, e.v, e.mi)
                    End If
                Next

                result.EdgesAfter = filtered.Count
                current = filtered
            Next

            result.Matrix = current

            ' 同一条被否决的边可能出现在多轮判定之中；而且它在后续轮次里仍然可能
            ' 因为窗口更窄的另一个三元组而被剔除。因此这里只保留<b>最终确实留下来</b>
            ' 的被否决边，并按键值去重，使 ProtectedEdges 的语义严格等于
            ' “被先验从 DPI 之中救回来、并且留在最终网络里的边”
            If result.ProtectedEdges.Count > 0 Then
                result.ProtectedEdges = result.ProtectedEdges _
                    .Where(Function(e) current(e.u, e.v) > 0.0R) _
                    .GroupBy(Function(e) MiMatrix.EdgeKey(e.u, e.v)) _
                    .Select(Function(g) g.First) _
                    .ToList
            End If

            Return result
        End Function

        ''' <summary>
        ''' 执行一轮 DPI 判定。
        ''' 
        ''' 所有的判定都基于<b>本轮开始时刻</b>的邻接关系(参考实现也是单轮判定)，
        ''' 被剔除的边在本轮结束之后统一生效；判定顺序按照互信息升序，
        ''' 也就是优先处理最弱的边。
        ''' </summary>
        ''' <param name="mi">当前轮次的互信息矩阵</param>
        ''' <param name="mode">DPI 模式</param>
        ''' <param name="tolerance">容差比例</param>
        ''' <param name="tfFlags">转录因子标记</param>
        ''' <param name="protect">
        ''' 受先验保护的边：键值 -> 保护强度(0~1)，强度会把容差窗口放宽
        ''' ``eps + (1 - eps) * strength``
        ''' </param>
        ''' <param name="result">用于累计统计量与被剔除边的结果对象</param>
        ''' <returns>本轮被剔除的边的键值集合</returns>
        Private Function ScreenPass(mi As MiMatrix,
                                    mode As DpiMode,
                                    tolerance As Double,
                                    tfFlags As Boolean(),
                                    protect As IDictionary(Of Long, Double),
                                    result As DpiResult) As HashSet(Of Long)

            Dim removed As New HashSet(Of Long)
            Dim vetoed As New HashSet(Of Long)
            Dim edges As MiEdge() = mi.Edges().OrderBy(Function(e) e.mi).ToArray

            For Each e As MiEdge In edges
                Dim key As Long = MiMatrix.EdgeKey(e.u, e.v)

                If removed.Contains(key) Then
                    Continue For
                End If

                ' 三元组之中最弱的边是否涉及转录因子，决定了它适用的容差
                Dim eps As Double = EdgeTolerance(mode, tolerance, tfFlags, e.u, e.v)
                Dim standard As Double = 1.0R - eps

                ' 先验把这条边的容差从 eps 放宽到 eps + (1 - eps) * strength，
                ' 也就是允许它比另外两条边弱 (eps + (1 - eps) * strength) 的比例；
                ' strength 来自先验置信度，因此高置信度的边享有更大的容错空间
                Dim strength As Double = 0.0R
                Dim window As Double = standard

                If protect IsNot Nothing AndAlso protect.TryGetValue(key, strength) AndAlso strength > 0.0R Then
                    Dim epsProtected As Double = eps + (1.0R - eps) * std.Min(1.0R, strength)

                    window = 1.0R - epsProtected
                End If

                Dim common As Integer() = Intersect(mi.Neighbors(e.u), mi.Neighbors(e.v))

                For Each k As Integer In common
                    result.TrianglesExamined += 1

                    Dim wik As Double = mi(e.u, k)
                    Dim wjk As Double = mi(e.v, k)
                    Dim weakestOther As Double = std.Min(wik, wjk)

                    If e.mi < standard * weakestOther Then
                        If strength > 0.0R AndAlso e.mi >= window * weakestOther Then
                            ' 先验提供了独立于表达数据的直接证据：否决 DPI 的判定，
                            ' 但是把“DPI 判定为间接边”这件事记录下来
                            If vetoed.Add(key) Then
                                Call result.ProtectedEdges.Add(e)
                            End If
                        Else
                            Call removed.Add(key)
                            Call result.RemovedEdges.Add(e)
                        End If

                        Exit For
                    End If
                Next
            Next

            Return removed
        End Function

        ''' <summary>
        ''' 计算一条边在 DPI 判定之中所使用的容差。
        ''' 
        ''' <see cref="DpiMode.TfGuided"/> 模式下，两端都不是转录因子的边会被
        ''' 坚决剔除(容差为零)，因为两个非转录因子之间的直接调控关系在生物学上
        ''' 非常少见，更可能是共同上游传递出来的间接相关。
        ''' </summary>
        Private Function EdgeTolerance(mode As DpiMode,
                                       tolerance As Double,
                                       tfFlags As Boolean(),
                                       u As Integer,
                                       v As Integer) As Double

            If mode <> DpiMode.TfGuided OrElse tfFlags Is Nothing Then
                Return tolerance
            End If

            If tfFlags(u) OrElse tfFlags(v) Then
                Return tolerance
            Else
                Return 0.0R
            End If
        End Function

        ''' <summary>
        ''' 两个升序整数数组的交集(双指针)
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Private Function Intersect(a As Integer(), b As Integer()) As Integer()
            If a.Length = 0 OrElse b.Length = 0 Then
                Return New Integer() {}
            End If

            Dim result As New List(Of Integer)
            Dim i As Integer = 0
            Dim j As Integer = 0

            While i < a.Length AndAlso j < b.Length
                If a(i) = b(j) Then
                    Call result.Add(a(i))

                    i += 1
                    j += 1
                ElseIf a(i) < b(j) Then
                    i += 1
                Else
                    j += 1
                End If
            End While

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 无向边的 64 位键值
        ''' </summary>
        Private Function EdgeKey(u As Integer, v As Integer) As Long
            Dim lo As Integer = std.Min(u, v)
            Dim hi As Integer = std.Max(u, v)

            Return (CLng(lo) << 32) Or CLng(hi)
        End Function
    End Module
End Namespace
