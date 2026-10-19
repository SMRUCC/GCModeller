Imports System.Collections.Generic

Namespace Graph

    ''' <summary>
    ''' 大图稀疏注意力的候选模式：每个基因固定 <c>c</c> 个候选基因及其空间距离桶编号。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 稠密注意力需要 <c>[N, N]</c> 的分数与权重矩阵，N=6 万时单头就要 29 GB，
    ''' 无法用于全基因组规模。本类型把每个节点的注意力范围限制为一个固定大小的
    ''' 候选集（BigBird 式"局部 + 全局 + 随机"结构）：
    ''' <list type="number">
    ''' <item><description>自身（桶 0，与稠密实现的对角线语义一致）；</description></item>
    ''' <item><description>图邻域：从节点出发的预算截断 BFS，深度不超过 <c>spdMax</c>，
    ''' 按边迭代顺序确定性展开；</description></item>
    ''' <item><description>高连接枢纽：全局度数最高的若干基因，代表远程先验调控；</description></item>
    ''' <item><description>随机长程链接：用固定种子抽取，保证先验图无连边时注意力也不会退化为恒等。</description></item>
    ''' </list>
    ''' 桶编号与 <see cref="GraphEncoding"/> 的稠密实现完全一致：
    ''' <c>0</c>=自身、<c>1..spdMax</c>=最短路径距离、<c>spdMax+1</c>=超距、<c>spdMax+2</c>=不连通。
    ''' </para>
    ''' <para>
    ''' 全部存储都是一维/二维整数数组：<c>N=60273、c=32</c> 时 <see cref="Targets"/> 与
    ''' <see cref="Buckets"/> 各约 7.7 MB，反向索引约 15 MB，与 N² 无关。
    ''' </para>
    ''' </remarks>
    Public Class SparseAttentionPattern

        Private ReadOnly _nGene As Integer
        Private ReadOnly _candidates As Integer
        Private ReadOnly _targets As Integer(,)
        Private ReadOnly _buckets As Integer(,)
        Private ReadOnly _reverseOffsets As Integer()
        Private ReadOnly _reverseSources As Integer()
        Private ReadOnly _bucketCount As Integer
        Private ReadOnly _graphBudget As Integer
        Private ReadOnly _hubBudget As Integer
        Private ReadOnly _randomBudget As Integer

        ''' <summary>节点（基因）数量</summary>
        Public ReadOnly Property NGene As Integer
            Get
                Return _nGene
            End Get
        End Property

        ''' <summary>每个节点的候选数 <c>c</c>（含自身）</summary>
        Public ReadOnly Property CandidateCount As Integer
            Get
                Return _candidates
            End Get
        End Property

        ''' <summary>候选目标矩阵 <c>[N, c]</c></summary>
        Public ReadOnly Property Targets As Integer(,)
            Get
                Return _targets
            End Get
        End Property

        ''' <summary>候选的 SPD 桶编号矩阵 <c>[N, c]</c></summary>
        Public ReadOnly Property Buckets As Integer(,)
            Get
                Return _buckets
            End Get
        End Property

        ''' <summary>
        ''' 反向索引的行偏移 <c>[N+1]</c>：目标节点 <c>j</c> 的入边来源为
        ''' <c>ReverseSources(ReverseOffsets(j) .. ReverseOffsets(j+1)-1)</c>。
        ''' </summary>
        Public ReadOnly Property ReverseOffsets As Integer()
            Get
                Return _reverseOffsets
            End Get
        End Property

        ''' <summary>
        ''' 反向索引的来源编码：<c>source * c + slot</c>。
        ''' 反向传播时按目标节点并行累加 <c>dK/dV</c>，每行只被一个线程写。
        ''' </summary>
        Public ReadOnly Property ReverseSources As Integer()
            Get
                Return _reverseSources
            End Get
        End Property

        ''' <summary>SPD 桶总数（与稠密实现一致：<c>spdMax + 3</c>）</summary>
        Public ReadOnly Property BucketCount As Integer
            Get
                Return _bucketCount
            End Get
        End Property

        ''' <summary>图邻域允许占用的槽位数</summary>
        Public ReadOnly Property GraphNeighborBudget As Integer
            Get
                Return _graphBudget
            End Get
        End Property

        ''' <summary>枢纽基因允许占用的槽位数</summary>
        Public ReadOnly Property HubGeneBudget As Integer
            Get
                Return _hubBudget
            End Get
        End Property

        ''' <summary>随机长程链接允许占用的槽位数（不足的邻域槽位会并入随机链接）</summary>
        Public ReadOnly Property RandomLinkBudget As Integer
            Get
                Return _randomBudget
            End Get
        End Property

        Private Sub New(n As Integer, candidates As Integer,
                        targets As Integer(,), buckets As Integer(,),
                        reverseOffsets As Integer(), reverseSources As Integer(),
                        bucketCount As Integer,
                        graphBudget As Integer, hubBudget As Integer, randomBudget As Integer)

            _nGene = n
            _candidates = candidates
            _targets = targets
            _buckets = buckets
            _reverseOffsets = reverseOffsets
            _reverseSources = reverseSources
            _bucketCount = bucketCount
            _graphBudget = graphBudget
            _hubBudget = hubBudget
            _randomBudget = randomBudget
        End Sub

        ''' <summary>
        ''' 构建稀疏候选模式。
        ''' </summary>
        ''' <param name="adjacency">无向邻接表（已去重）</param>
        ''' <param name="n">节点数</param>
        ''' <param name="spdMax">最大编码距离</param>
        ''' <param name="bucketCount">桶总数（<c>spdMax + 3</c>）</param>
        ''' <param name="neighbors">每节点候选预算（含自身）</param>
        ''' <param name="randomLinks">期望的随机长程链接数</param>
        ''' <param name="hubs">高连接枢纽基因（已按度数降序截断；可为空）</param>
        ''' <param name="seed">随机长程链接的种子（固定后结果可复现）</param>
        ''' <returns>稀疏候选模式</returns>
        Public Shared Function Build(adjacency As List(Of Integer)(),
                                     n As Integer,
                                     spdMax As Integer,
                                     bucketCount As Integer,
                                     neighbors As Integer,
                                     randomLinks As Integer,
                                     hubs As Integer(),
                                     seed As Integer) As SparseAttentionPattern

            If adjacency Is Nothing Then Throw New ArgumentNullException(NameOf(adjacency))
            If adjacency.Length <> n Then
                Throw New ArgumentException($"邻接表大小 {adjacency.Length} 与节点数 {n} 不一致", NameOf(adjacency))
            End If
            If n <= 0 Then Throw New ArgumentException("节点数必须为正", NameOf(n))

            Dim cappedSpd As Integer = System.Math.Max(1, spdMax)
            Dim c As Integer = System.Math.Min(System.Math.Max(neighbors, 2), n)

            ' 槽位分配：枢纽与随机链接优先，其余留给图邻域；
            ' 邻域预算至少 1 个槽位，保证连通节点不会因预算挤占而看不到直接邻居
            Dim hubSlots As Integer = If(hubs Is Nothing, 0,
                                         System.Math.Min(hubs.Length, System.Math.Max(0, c - 2)))
            Dim randomSlots As Integer = System.Math.Min(System.Math.Max(randomLinks, 0),
                                                         System.Math.Max(0, c - 1 - hubSlots))
            Dim graphSlots As Integer = System.Math.Max(1, c - hubSlots - randomSlots)

            Dim farBucket As Integer = cappedSpd + 1
            Dim disconnectedBucket As Integer = cappedSpd + 2

            Dim targets As Integer(,) = New Integer(n - 1, c - 1) {}
            Dim buckets As Integer(,) = New Integer(n - 1, c - 1) {}

            ' visited 用 token 标记（token = source + 1）避免每个节点清空 O(N) 的数组
            Dim visited As Integer() = New Integer(n - 1) {}
            Dim distance As Integer() = New Integer(n - 1) {}
            Dim queue As Integer() = New Integer(n - 1) {}

            Dim rand As New Random(seed)
            Dim graphCandidates As Integer = 1 + graphSlots

            For source As Integer = 0 To n - 1
                Dim token As Integer = source + 1
                Dim count As Integer = 0

                ' ---- 1. 自身 ----
                targets(source, 0) = source
                buckets(source, 0) = GraphEncoding.SelfBucket
                visited(source) = token
                distance(source) = 0
                count = 1

                ' ---- 2. 预算截断的 BFS（深度 <= spdMax） ----
                Dim head As Integer = 0
                Dim tail As Integer = 0

                queue(0) = source

                While head <= tail AndAlso count < graphCandidates
                    Dim u As Integer = queue(head)

                    head += 1

                    If distance(u) >= cappedSpd Then Continue While

                    For Each v As Integer In adjacency(u)
                        If count >= graphCandidates Then Exit For
                        If visited(v) = token Then Continue For

                        visited(v) = token
                        distance(v) = distance(u) + 1

                        targets(source, count) = v
                        buckets(source, count) = distance(v)
                        count += 1

                        tail += 1
                        queue(tail) = v
                    Next
                End While

                ' ---- 3. 高连接枢纽（远程先验调控） ----
                If hubs IsNot Nothing Then
                    For Each hub As Integer In hubs
                        If count >= c Then Exit For
                        If hub < 0 OrElse hub >= n Then Continue For
                        If visited(hub) = token Then Continue For

                        visited(hub) = token

                        targets(source, count) = hub
                        buckets(source, count) = farBucket
                        count += 1
                    Next
                End If

                ' ---- 4. 随机长程链接（补齐剩余槽位） ----
                Dim attempts As Integer = 0
                Dim attemptLimit As Integer = System.Math.Max(64 * c, 8 * n)

                While count < c AndAlso attempts < attemptLimit
                    attempts += 1

                    Dim v As Integer = rand.Next(n)

                    If visited(v) = token Then Continue While

                    visited(v) = token

                    targets(source, count) = v
                    buckets(source, count) = disconnectedBucket
                    count += 1
                End While

                ' 兜底：随机采样未填满（只可能在 c 接近 n 时出现）时按序号补齐
                If count < c Then
                    For v As Integer = 0 To n - 1
                        If count >= c Then Exit For
                        If visited(v) = token Then Continue For

                        visited(v) = token

                        targets(source, count) = v
                        buckets(source, count) = disconnectedBucket
                        count += 1
                    Next
                End If
            Next

            ' ---- 反向索引：目标节点 -> (source, slot) ----
            Dim incoming As Integer() = New Integer(n - 1) {}

            For i As Integer = 0 To n - 1
                For slot As Integer = 0 To c - 1
                    incoming(targets(i, slot)) += 1
                Next
            Next

            Dim offsets As Integer() = New Integer(n) {}

            For i As Integer = 0 To n - 1
                offsets(i + 1) = offsets(i) + incoming(i)
            Next

            Dim cursor As Integer() = CType(offsets.Clone(), Integer())
            Dim sources As Integer() = New Integer(offsets(n) - 1) {}

            For i As Integer = 0 To n - 1
                For slot As Integer = 0 To c - 1
                    Dim target As Integer = targets(i, slot)

                    sources(cursor(target)) = i * c + slot
                    cursor(target) += 1
                Next
            Next

            Return New SparseAttentionPattern(
                n, c, targets, buckets, offsets, sources,
                System.Math.Max(bucketCount, cappedSpd + 3),
                graphSlots, hubSlots, randomSlots)
        End Function

        ''' <summary>输出单行规模摘要（诊断用）</summary>
        ''' <returns>摘要文本</returns>
        Public Overrides Function ToString() As String
            Return $"稀疏注意力 N={_nGene} c={_candidates} " &
                   $"(邻域<={_graphBudget} 枢纽<={_hubBudget} 随机<={_randomBudget})"
        End Function

    End Class

End Namespace
