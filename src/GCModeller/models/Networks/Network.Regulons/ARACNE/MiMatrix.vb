Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace ARACNE

    ''' <summary>
    ''' 互信息矩阵之中的一条无向边
    ''' </summary>
    Public Structure MiEdge

        ''' <summary>
        ''' 边的一个端点(较小的节点下标)
        ''' </summary>
        Public ReadOnly u As Integer

        ''' <summary>
        ''' 边的另一个端点(较大的节点下标)
        ''' </summary>
        Public ReadOnly v As Integer

        ''' <summary>
        ''' 边上的互信息(比特)
        ''' </summary>
        Public ReadOnly mi As Double

        ''' <summary>
        ''' 构造一条无向的互信息边
        ''' </summary>
        ''' <param name="u"></param>
        ''' <param name="v"></param>
        ''' <param name="mi"></param>
        Public Sub New(u As Integer, v As Integer, mi As Double)
            If u <= v Then
                Me.u = u
                Me.v = v
            Else
                Me.u = v
                Me.v = u
            End If

            Me.mi = mi
        End Sub

        ''' <summary>
        ''' 以节点下标表达的唯一边标识符，形如 ``3~17``
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UID As String
            Get
                Return $"{u}~{v}"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{u}, {v}] MI={mi.ToString("F4")}"
        End Function
    End Structure

    ''' <summary>
    ''' 稀疏的互信息矩阵。
    ''' 
    ''' ARACNE 的第一步需要计算所有候选基因对的互信息，但是对于 ``N = 20000`` 的基因
    ''' 表达矩阵而言，稠密的 ``N x N`` 矩阵需要 3.2 GB 内存，而且其中的绝大部分元素
    ''' 在阈值过滤之后都会变成零。因此这里采用“边列表 + CSR 邻接表”的稀疏结构：
    ''' 
    ''' + 构建阶段：<see cref="SetValue(Integer, Integer, Double)"/> 把边追加到边列表之中；
    ''' + 首次读取的时候自动 <see cref="Freeze"/> 成压缩稀疏行(CSR)的对称邻接表，
    '''   邻居按照节点下标升序排列，因此 <see cref="Neighbors(Integer)"/> 的结果可以直接
    '''   用于两个集合求交(DPI 的共同邻居枚举)；
    ''' + <see cref="Prune"/> / <see cref="PruneTopK"/> 返回一个新的矩阵，
    '''   原始数据不被修改。
    ''' </summary>
    Public Class MiMatrix

        ''' <summary>
        ''' 节点(基因)的编号，下标与邻接表的节点下标一一对应
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property names As String()

        Private ReadOnly edgeList As New List(Of MiEdge)

        Private m_offsets As Integer() = Nothing
        Private m_neighbors As Integer() = Nothing
        Private m_weights As Double() = Nothing

        ''' <summary>
        ''' 构造一个空的互信息矩阵
        ''' </summary>
        ''' <param name="names">节点(基因)的编号列表，顺序决定了节点的下标</param>
        Public Sub New(names As String())
            If names Is Nothing Then
                Throw New ArgumentNullException(NameOf(names))
            End If

            Me.names = names
        End Sub

        ''' <summary>
        ''' 节点(基因)的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return names.Length
            End Get
        End Property

        ''' <summary>
        ''' 当前矩阵之中边的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer
            Get
                Return edgeList.Count
            End Get
        End Property

        ''' <summary>
        ''' 是否已经建立好 CSR 邻接表的索引
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsFrozen As Boolean
            Get
                Return m_offsets IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' 追加(或者覆盖)一条无向边。
        ''' 
        ''' 构建阶段是单线程的：并行计算出来的边会先按行收集，
        ''' 然后再集中写入本矩阵，这样可以避免并发写入字典的锁开销。
        ''' </summary>
        ''' <param name="u">节点下标</param>
        ''' <param name="v">节点下标</param>
        ''' <param name="mi">互信息(比特)</param>
        Public Sub SetValue(u As Integer, v As Integer, mi As Double)
            If u = v Then
                Return
            End If
            If u < 0 OrElse u >= Size OrElse v < 0 OrElse v >= Size Then
                Throw New IndexOutOfRangeException($"the node index is out of the range [0, {Size}): {u}, {v}")
            End If

            m_offsets = Nothing

            Call edgeList.Add(New MiEdge(u, v, mi))
        End Sub

        ''' <summary>
        ''' 把边列表展开成对称的 CSR 邻接表(惰性执行，只会执行一次)
        ''' </summary>
        Public Sub Freeze()
            If m_offsets IsNot Nothing Then
                Return
            End If

            Dim n As Integer = Size
            Dim degree As Integer() = New Integer(n - 1) {}

            For Each e As MiEdge In edgeList
                degree(e.u) += 1
                degree(e.v) += 1
            Next

            Dim offsets As Integer() = New Integer(n) {}

            For i As Integer = 0 To n - 1
                offsets(i + 1) = offsets(i) + degree(i)
            Next

            Dim total As Integer = offsets(n)
            Dim neighbors As Integer() = New Integer(total - 1) {}
            Dim weights As Double() = New Double(total - 1) {}
            Dim cursor As Integer() = DirectCast(offsets.Clone(), Integer())

            For Each e As MiEdge In edgeList
                neighbors(cursor(e.u)) = e.v
                weights(cursor(e.u)) = e.mi
                cursor(e.u) += 1

                neighbors(cursor(e.v)) = e.u
                weights(cursor(e.v)) = e.mi
                cursor(e.v) += 1
            Next

            ' 邻居按照下标升序排列，便于两个邻居集合的双指针求交
            For i As Integer = 0 To n - 1
                Dim offset As Integer = offsets(i)
                Dim len As Integer = offsets(i + 1) - offset

                If len > 1 Then
                    Array.Sort(neighbors, weights, offset, len)
                End If
            Next

            m_neighbors = neighbors
            m_weights = weights
            m_offsets = offsets
        End Sub

        ''' <summary>
        ''' 获取一条边的互信息，不存在的时候返回零
        ''' </summary>
        ''' <param name="u"></param>
        ''' <param name="v"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property Item(u As Integer, v As Integer) As Double
            Get
                If u = v Then
                    Return 0.0R
                End If

                Dim offset As Integer = OffsetOf(u)
                Dim idx As Integer = Array.BinarySearch(m_neighbors, offset, m_offsets(u + 1) - offset, v)

                If idx >= 0 Then
                    Return m_weights(idx)
                Else
                    Return 0.0R
                End If
            End Get
        End Property

        ''' <summary>
        ''' 获取指定节点的全部邻居(升序排列)
        ''' </summary>
        ''' <param name="u"></param>
        ''' <returns></returns>
        Public Function Neighbors(u As Integer) As Integer()
            Dim offset As Integer = OffsetOf(u)
            Dim len As Integer = m_offsets(u + 1) - offset
            Dim result As Integer() = New Integer(len - 1) {}

            Array.Copy(m_neighbors, offset, result, 0, len)

            Return result
        End Function

        ''' <summary>
        ''' 获取指定节点的邻居以及对应的互信息
        ''' </summary>
        ''' <param name="u"></param>
        ''' <returns></returns>
        Public Function NeighborEdges(u As Integer) As (neighbor As Integer, mi As Double)()
            Dim offset As Integer = OffsetOf(u)
            Dim len As Integer = m_offsets(u + 1) - offset
            Dim result(len - 1) As (neighbor As Integer, mi As Double)

            For i As Integer = 0 To len - 1
                result(i) = (m_neighbors(offset + i), m_weights(offset + i))
            Next

            Return result
        End Function

        ''' <summary>
        ''' 指定节点的度(与之相连的基因数量)
        ''' </summary>
        ''' <param name="u"></param>
        ''' <returns></returns>
        Public Function Degree(u As Integer) As Integer
            Dim offset As Integer = OffsetOf(u)

            Return m_offsets(u + 1) - offset
        End Function

        ''' <summary>
        ''' 枚举全部的无向边(每一条边只会出现一次，``u &lt; v``)
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function Edges() As IEnumerable(Of MiEdge)
            Call Freeze()

            For u As Integer = 0 To Size - 1
                For k As Integer = m_offsets(u) To m_offsets(u + 1) - 1
                    Dim v As Integer = m_neighbors(k)

                    If v > u Then
                        Yield New MiEdge(u, v, m_weights(k))
                    End If
                Next
            Next
        End Function

        ''' <summary>
        ''' 按照互信息阈值剪枝：只保留 ``MI &gt;= minMi`` 的边。
        ''' 
        ''' 如果提供了 <paramref name="protect"/>，那么受保护的边(通常是 curated 先验
        ''' 支持的调控关系)只要 ``MI`` 不低于这条边自己被分配的门槛就会被保留下来，
        ''' 即使它低于常规阈值。
        ''' 
        ''' 这就是“先验知识放宽阈值”的实现：先验提供倾斜，但是仍然要求数据
        ''' 至少给出一点点支持，避免把完全没有信号的关系强行加进网络；
        ''' 而每条先验边的门槛由它的置信度决定(置信度越高、门槛越低)。
        ''' </summary>
        ''' <param name="minMi">常规的互信息阈值</param>
        ''' <param name="protect">
        ''' 受保护的边：键值是 <see cref="EdgeKey(Integer, Integer)"/>，数值是这条边能够接受的
        ''' 最低互信息(也就是按先验置信度缩放之后的 ``tau_prior``)。
        ''' 
        ''' 不同的先验边可以有<b>不同</b>的放宽阈值，这就实现了“置信度越高、门槛越低”的分级放宽。
        ''' </param>
        ''' <returns></returns>
        Public Function Prune(minMi As Double,
                              Optional protect As IDictionary(Of Long, Double) = Nothing) As MiMatrix

            Dim result As New MiMatrix(names)

            For Each e As MiEdge In Edges()
                Dim key As Long = EdgeKey(e.u, e.v)

                If e.mi >= minMi Then
                    Call result.SetValue(e.u, e.v, e.mi)
                ElseIf protect IsNot Nothing Then
                    Dim relaxed As Double

                    If protect.TryGetValue(key, relaxed) AndAlso e.mi >= relaxed Then
                        Call result.SetValue(e.u, e.v, e.mi)
                    End If
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 每个节点最多保留 ``k`` 条最强的边(top-K 剪枝)。
        ''' 
        ''' 一条边只要位于<b>任意一个</b>端点的 top-K 名单之中就会被保留下来，
        ''' 这样可以避免非对称地切断某个 hub 基因的边缘。
        ''' </summary>
        ''' <param name="k">每个节点保留的最大邻居数量，小于等于零的时候不剪枝</param>
        ''' <param name="protect">受保护的边(使用 <see cref="EdgeKey(Integer, Integer)"/> 构造的键值)，这些边不受 top-K 剪枝的影响</param>
        ''' <returns></returns>
        Public Function PruneTopK(k As Integer, Optional protect As HashSet(Of Long) = Nothing) As MiMatrix
            If k <= 0 Then
                Return Me
            End If

            Call Freeze()

            Dim keep As New HashSet(Of Long)
            Dim n As Integer = Size

            For u As Integer = 0 To n - 1
                Dim offset As Integer = m_offsets(u)
                Dim len As Integer = m_offsets(u + 1) - offset

                If len = 0 Then
                    Continue For
                ElseIf len <= k Then
                    ' 邻居数量没有超过上限：全部保留
                    For t As Integer = offset To offset + len - 1
                        Call keep.Add(EdgeKey(u, m_neighbors(t)))
                    Next
                Else
                    ' 取互信息最强的 k 个邻居
                    Dim order As Integer() = Enumerable _
                        .Range(0, len) _
                        .OrderByDescending(Function(t) m_weights(offset + t)) _
                        .Take(k) _
                        .ToArray

                    For Each t As Integer In order
                        Call keep.Add(EdgeKey(u, m_neighbors(offset + t)))
                    Next
                End If
            Next

            Dim result As New MiMatrix(names)

            For Each e As MiEdge In Edges()
                Dim key As Long = EdgeKey(e.u, e.v)

                If keep.Contains(key) OrElse (protect IsNot Nothing AndAlso protect.Contains(key)) Then
                    Call result.SetValue(e.u, e.v, e.mi)
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 展开成稠密的对称矩阵(只用于小规模的调试与导出)
        ''' </summary>
        ''' <returns></returns>
        Public Function ToDense() As Double(,)
            Call Freeze()

            Dim n As Integer = Size
            Dim dense As Double(,) = New Double(n - 1, n - 1) {}

            For Each e As MiEdge In Edges()
                dense(e.u, e.v) = e.mi
                dense(e.v, e.u) = e.mi
            Next

            Return dense
        End Function

        ''' <summary>
        ''' 转换成工程既有的 <see cref="Matrix"/> 对象：行是调控因子、列是靶基因。
        ''' 
        ''' 为了控制内存占用，只导出至少拥有一条边的节点(孤立基因不会出现在矩阵之中)，
        ''' 因此返回值的行列数量可能小于 <see cref="Size"/>。
        ''' </summary>
        ''' <param name="tag">矩阵的标签，为空的时候自动生成</param>
        ''' <returns></returns>
        Public Function ToMatrix(Optional tag As String = Nothing) As Matrix
            Call Freeze()

            Dim rows As Integer() = NodesByActivity()
            Dim labels As String() = rows.Select(Function(i) names(i)).ToArray
            Dim data As DataFrameRow() = New DataFrameRow(rows.Length - 1) {}

            For r As Integer = 0 To rows.Length - 1
                Dim u As Integer = rows(r)
                Dim values As Double() = New Double(rows.Length - 1) {}

                For c As Integer = 0 To rows.Length - 1
                    values(c) = Item(u, rows(c))
                Next

                data(r) = New DataFrameRow With {
                    .geneID = names(u),
                    .experiments = values
                }
            Next

            Return New Matrix With {
                .tag = If(String.IsNullOrEmpty(tag), $"ARACNE:MI[{Count} edges, {rows.Length} nodes]", tag),
                .sampleID = labels,
                .expression = data
            }
        End Function

        ''' <summary>
        ''' 转换成工程既有的 <see cref="NamedSparseMatrix"/> 对象，便于复用既有的
        ''' 网络导出与可视化流程。
        ''' </summary>
        ''' <returns></returns>
        Public Function ToNamedSparseMatrix() As NamedSparseMatrix
            Dim sparse As New NamedSparseMatrix()

            For Each e As MiEdge In Edges()
                sparse(names(e.u), names(e.v)) = e.mi
                sparse(names(e.v), names(e.u)) = e.mi
            Next

            Return sparse
        End Function

        ''' <summary>
        ''' 全部边之中的最大互信息，没有边的时候返回零
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MaxMi As Double
            Get
                Dim max As Double = 0.0R

                For Each e As MiEdge In Edges()
                    If e.mi > max Then
                        max = e.mi
                    End If
                Next

                Return max
            End Get
        End Property

        ''' <summary>
        ''' 全部边的平均互信息
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MeanMi As Double
            Get
                If Count = 0 Then
                    Return 0.0R
                End If

                Dim sum As Double = 0.0R

                For Each e As MiEdge In Edges()
                    sum += e.mi
                Next

                Return sum / Count
            End Get
        End Property

        ''' <summary>
        ''' 至少拥有一条边的节点数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NodeCount As Integer
            Get
                Call Freeze()

                Dim n As Integer = 0

                For i As Integer = 0 To Size - 1
                    If m_offsets(i + 1) > m_offsets(i) Then
                        n += 1
                    End If
                Next

                Return n
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"MI matrix: {Size} nodes, {Count} edges, mean={MeanMi.ToString("F4")}, max={MaxMi.ToString("F4")}"
        End Function

#Region "helpers"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function OffsetOf(u As Integer) As Integer
            Call Freeze()

            If u < 0 OrElse u >= Size Then
                Throw New IndexOutOfRangeException($"the node index is out of the range [0, {Size}): {u}")
            End If

            Return m_offsets(u)
        End Function

        ''' <summary>
        ''' 按度数降序返回非孤立节点的下标
        ''' </summary>
        ''' <returns></returns>
        Private Function NodesByActivity() As Integer()
            Call Freeze()

            Return Enumerable _
                .Range(0, Size) _
                .Where(Function(i) m_offsets(i + 1) > m_offsets(i)) _
                .OrderByDescending(Function(i) m_offsets(i + 1) - m_offsets(i)) _
                .ToArray
        End Function

        ''' <summary>
        ''' 无向边的 64 位键值 ``(lo &lt;&lt; 32) Or hi``，两个端点的顺序会被归一化。
        ''' 
        ''' 先验网络、DPI 保护集合与本稀疏矩阵都使用同一个键值，
        ''' 因此调用方可以直接用它来表达“这条边受先验保护”。
        ''' </summary>
        ''' <param name="u">节点下标</param>
        ''' <param name="v">节点下标</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function EdgeKey(u As Integer, v As Integer) As Long
            Dim lo As Integer = std.Min(u, v)
            Dim hi As Integer = std.Max(u, v)

            Return (CLng(lo) << 32) Or CLng(hi)
        End Function

#End Region
    End Class
End Namespace
