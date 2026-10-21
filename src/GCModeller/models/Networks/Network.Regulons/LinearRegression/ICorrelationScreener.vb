Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 候选调控因子的相关性筛选后端。
    ''' 
    ''' 拟合基因调控网络的时候，每一个目标基因都需要从全体基因之中挑出与它
    ''' 相关性最强的若干个候选调控因子。逐基因扫描的复杂度是 ``O(基因数^2 * 样本数)``
    ''' 的内存流量，实现这个接口之后可以把这一步换成一次分块矩阵乘法
    ''' （CPU 的 <see cref="BlockCorrelationScreener"/>，或者 GPU 上的浮点矩阵乘）。
    ''' 
    ''' 生命周期：<see cref="Initialize"/> 在 <see cref="GeneRegulatoryNetwork.Fit"/>
    ''' 开始之前被调用一次，随后 <see cref="Query"/> 会被并行拟合过程并发调用，
    ''' 因此实现必须是"初始化之后只读"的线程安全对象。
    ''' </summary>
    Public Interface ICorrelationScreener : Inherits IDisposable

        ''' <summary>
        ''' 为负相关候选保留的名额；小于等于 0 表示不启用符号配额。
        ''' 
        ''' 共表达网络里负相关的边很少，强制留出名额可以让扰动响应同时有升有降。
        ''' </summary>
        ''' <returns></returns>
        Property NegativeQuota As Integer

        ''' <summary>
        ''' 注入参与筛选的全部基因向量，并一次性完成每个目标基因的候选集计算。
        ''' </summary>
        ''' <param name="pool">
        ''' 允许作为候选调控因子的基因行号（已经按 <see cref="GrnFittingOptions.regulators"/>
        ''' 过滤过），函数内部不应修改它
        ''' </param>
        ''' <param name="vectors">
        ''' 全部基因的表达向量，下标与基因行号一致；与 <paramref name="pool"/> 中的
        ''' 行号配合使用
        ''' </param>
        ''' <param name="topK">每个目标基因需要保留的候选调控因子数量</param>
        ''' <param name="minAbsR">
        ''' 相关系数绝对值下限，低于它的基因不会被选为候选；小于等于零表示不限制
        ''' </param>
        Sub Initialize(pool As Integer(), vectors As Double()(), topK As Integer, minAbsR As Double)

        ''' <summary>
        ''' 取出某个目标基因的候选调控因子行号，按相关系数绝对值降序排列。
        ''' </summary>
        ''' <param name="target">目标基因的行号</param>
        ''' <returns>
        ''' 候选调控因子的行号（不含目标基因自身）；无法解析的时候返回空数组
        ''' </returns>
        Function Query(target As Integer) As Integer()

    End Interface

    ''' <summary>
    ''' 从一行相关系数之中取出绝对值最大的若干个候选（最小堆，``O(p log k)``）。
    ''' 
    ''' CPU 的分块实现与 GPU 实现共用这段选择逻辑，保证两者的候选集合判据一致。
    ''' </summary>
    Public Module CorrelationTopK

        ''' <summary>
        ''' 取一行相关系数之中绝对值最大的 top-k。
        ''' </summary>
        ''' <param name="row">按候选池下标排列的相关系数</param>
        ''' <param name="pool">候选池下标到基因行号的映射</param>
        ''' <param name="topK">需要保留的候选数量</param>
        ''' <param name="minAbsR">相关系数绝对值下限，小于等于零表示不限制</param>
        ''' <param name="exclude">需要排除的候选池下标（目标基因自身）</param>
        ''' <param name="requireSign">
        ''' 0 = 不限制；1 = 只接受正相关；-1 = 只接受负相关
        ''' </param>
        ''' <returns>候选基因的行号，按相关系数绝对值降序</returns>
        Public Function SelectTopK(row As Double(),
                                   pool As Integer(),
                                   topK As Integer,
                                   minAbsR As Double,
                                   exclude As Integer,
                                   Optional requireSign As Integer = 0) As Integer()

            Dim k As Integer = std.Min(topK, std.Max(0, row.Length - 1))

            If k <= 0 OrElse pool Is Nothing Then
                Return New Integer() {}
            End If

            Dim idx As Integer() = New Integer(k - 1) {}
            Dim key As Double() = New Double(k - 1) {}
            Dim size As Integer = 0
            Dim floor As Double = std.Max(0.0R, minAbsR)

            For j As Integer = 0 To row.Length - 1
                If j = exclude Then
                    Continue For
                End If

                Dim value As Double = std.Abs(row(j))

                If value <= 0.0R OrElse value < floor Then
                    Continue For
                End If
                If requireSign > 0 AndAlso row(j) <= 0.0R Then
                    Continue For
                End If
                If requireSign < 0 AndAlso row(j) >= 0.0R Then
                    Continue For
                End If

                If size < k Then
                    idx(size) = j
                    key(size) = value
                    size += 1

                    If size = k Then
                        ' 堆化：堆顶保持当前 k 个候选之中绝对值最小的那个
                        For h As Integer = (k \ 2) - 1 To 0 Step -1
                            Call SiftDown(idx, key, h, size)
                        Next
                    End If
                ElseIf value > key(0) Then
                    idx(0) = j
                    key(0) = value
                    Call SiftDown(idx, key, 0, size)
                End If
            Next

            If size = 0 Then
                Return New Integer() {}
            End If

            Dim order As Integer() = Enumerable _
                .Range(0, size) _
                .OrderByDescending(Function(i) key(i)) _
                .ToArray
            Dim candidates As Integer() = New Integer(size - 1) {}

            For i As Integer = 0 To size - 1
                candidates(i) = pool(idx(order(i)))
            Next

            Return candidates
        End Function

        ''' <summary>
        ''' 带符号配额的 top-k 选择：优先保留 <paramref name="negativeQuota"/> 个<b>负相关</b>
        ''' 候选，剩下的名额给正相关。
        ''' 
        ''' 共表达网络里的边绝大多数是正的，L1 会优先保留这些强正相关，
        ''' 于是敲降一个基因只会把整片基因一起拖下去(全下调)。强制给负相关留出名额，
        ''' 可以让网络里出现"抑制型"的边，扰动响应才会同时有升有降。
        ''' 
        ''' 若某一侧的数量不足，会自动用另一侧补齐，不会因此少给候选。
        ''' </summary>
        ''' <param name="row">按候选池下标排列的<b>带符号</b>相关系数</param>
        ''' <param name="pool">候选池下标到基因行号的映射</param>
        ''' <param name="topK">需要保留的候选总数</param>
        ''' <param name="minAbsR">相关系数绝对值下限</param>
        ''' <param name="exclude">需要排除的候选池下标(目标基因自身)，-1 表示不排除</param>
        ''' <param name="negativeQuota">为负相关保留的名额；小于等于 0 时退化为普通的 top-k</param>
        ''' <returns>候选基因的行号</returns>
        Public Function SelectTopKBalanced(row As Double(),
                                           pool As Integer(),
                                           topK As Integer,
                                           minAbsR As Double,
                                           exclude As Integer,
                                           negativeQuota As Integer) As Integer()

            If negativeQuota <= 0 Then
                Return SelectTopK(row, pool, topK, minAbsR, exclude)
            End If

            Dim budget As Integer = std.Min(topK, std.Max(0, row.Length - 1))

            If budget <= 0 Then
                Return New Integer() {}
            End If

            Dim slots As Integer = std.Min(negativeQuota, budget)
            Dim negatives As Integer() = SelectTopK(row, pool, slots, minAbsR, exclude, -1)
            Dim positives As Integer() = SelectTopK(row, pool, budget - negatives.Length, minAbsR, exclude, 1)
            Dim result As New List(Of Integer)

            Call result.AddRange(negatives)
            Call result.AddRange(positives)

            ' 某一侧数量不足时，用另一侧补齐
            If result.Count < budget Then
                Dim extra As Integer() = SelectTopK(row, pool, budget, minAbsR, exclude)
                Dim present As New HashSet(Of Integer)(result)

                For Each idx As Integer In extra
                    If present.Add(idx) Then
                        Call result.Add(idx)

                        If result.Count >= budget Then
                            Exit For
                        End If
                    End If
                Next
            End If

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 最小堆的下沉操作（按 <paramref name="key"/> 比较）。
        ''' </summary>
        ''' <param name="idx">与 <paramref name="key"/> 一一对应的载荷</param>
        ''' <param name="key">堆的排序键</param>
        ''' <param name="node">开始下沉的节点</param>
        ''' <param name="size">堆的当前大小</param>
        Private Sub SiftDown(idx As Integer(), key As Double(), node As Integer, size As Integer)
            Dim parent As Integer = node

            Do
                Dim left As Integer = parent * 2 + 1

                If left >= size Then
                    Exit Do
                End If

                Dim smallest As Integer = left
                Dim right As Integer = left + 1

                If right < size AndAlso key(right) < key(left) Then
                    smallest = right
                End If

                If key(parent) <= key(smallest) Then
                    Exit Do
                End If

                Dim tmpIdx As Integer = idx(parent)
                Dim tmpKey As Double = key(parent)

                idx(parent) = idx(smallest)
                key(parent) = key(smallest)
                idx(smallest) = tmpIdx
                key(smallest) = tmpKey

                parent = smallest
            Loop
        End Sub

    End Module

End Namespace
