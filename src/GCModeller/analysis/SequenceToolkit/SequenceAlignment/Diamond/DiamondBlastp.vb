' DIAMOND 顶层比对入口 (DiamondBlastp)
'
' 将整条 DIAMOND 流水线编排为可复用 API:对单条查询蛋白序列 vs 单个参考蛋白库
' 执行 blastp 加速比对。
'
' 流水线(按文档四阶段):
'   1) 种子生成:缩减字母表 + 间隔种子(按灵敏度模式选形状集);
'   2) 双索引与哈希连接:查询/参考逐形状建索引并做线性哈希连接;
'   3) 分层过滤链:48aa 窗口 Hamming 初筛 -> BLOSUM62 无空位延伸
'                   -> 最左种子去冗余 -> 按得分排序启发式分块触发;
'   4) 带状 Smith-Waterman 扩展:仅对调度候选的窗口做有空位局部比对;
'   5) 结果聚合:产出 m8 风格的 <see cref="DiamondHit"/> 集合。
'
' 本阶段为"单查询 vs 单库"原型,算法正确性优先;SIMD 向量化作为后续可选
' 优化层(各过滤/扩展边界均已封装接口,替换不影响编排)。

Imports System.Linq
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    Public Class DiamondBlastp

        ''' <summary>选择的灵敏度模式(决定间隔种子形状集)。</summary>
        Public ReadOnly Mode As SensitivityMode

        Private ReadOnly seeds As SpacedSeed()
        Private ReadOnly hamming As IHammingFilter
        Private ReadOnly ungapped As UngappedExtension
        Private ReadOnly leftMost As LeftMostSeedFilter
        Private ReadOnly scheduler As HitScheduler
        Private ReadOnly bandSW As BandSW

        ''' <summary>是否启用 SIMD 向量化(Hamming SSE2 / 无空位延伸 AVX2)。</summary>
        Public ReadOnly UseSimd As Boolean

        ''' <summary>
        ''' 构造 DIAMOND blastp 比对器。
        ''' </summary>
        ''' <param name="mode">灵敏度模式,默认 Fast。</param>
        ''' <param name="useSimd">是否启用 SIMD 向量化;运行时还会按 IsSupported 进一步判定。</param>
        Sub New(Optional mode As SensitivityMode = SensitivityMode.Fast, Optional useSimd As Boolean = True)
            Me.Mode = mode
            Me.UseSimd = useSimd
            Me.seeds = SpacedSeeds.GetSeeds(mode)

            ' 运行时选择 Hamming 实现:SSE2 可用且启用则用向量化版,否则标量
            If useSimd AndAlso System.Runtime.Intrinsics.X86.Sse2.IsSupported Then
                Me.hamming = New HammingFilterSse()
            Else
                Me.hamming = New HammingFilter()
            End If

            ' 运行时选择无空位延伸实现:AVX2 可用且启用则用向量化批量版,否则标量
            If useSimd AndAlso System.Runtime.Intrinsics.X86.Avx2.IsSupported Then
                Me.ungapped = New UngappedExtensionAvx2()
            Else
                Me.ungapped = New UngappedExtension()
            End If

            Me.leftMost = New LeftMostSeedFilter()
            Me.scheduler = New HitScheduler()
            Me.bandSW = New BandSW()
        End Sub

        ''' <summary>
        ''' 对单条查询序列在参考库上执行 DIAMOND 风格 blastp 比对。
        ''' </summary>
        ''' <param name="query">查询蛋白序列。</param>
        ''' <param name="subjectDb">参考蛋白库(序列集合)。</param>
        ''' <param name="maxHits">最多返回的命中数(按得分降序),0 表示不限。</param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Search(query As FastaSeq, subjectDb As IEnumerable(Of FastaSeq), Optional maxHits As Integer = 0) As IEnumerable(Of DiamondHit)
            Dim subjects = subjectDb.ToArray
            Return SearchSingleCore(query, subjects, Nothing, maxHits)
        End Function

        ''' <summary>
        ''' 单查询核心逻辑。多查询场景下可传入按形状缓存的 <paramref name="refCache"/>,
        ''' 以复用已构建的参考索引,避免对每个查询重复建索引。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function SearchSingleCore(query As FastaSeq, subjects As FastaSeq(), ByRef refCache As Dictionary(Of Long, ReferenceIndex), Optional maxHits As Integer = 0) As IEnumerable(Of DiamondHit)
            Dim subjectSeqs As String() = subjects.Select(Function(s) s.SequenceData).ToArray
            Dim subjectTitles As String() = subjects.Select(Function(s) s.Title).ToArray
            Dim querySeq = query.SequenceData
            Dim queryTitle = query.Title
            Dim collected As New List(Of DiamondHit)

            ' 跨查询复用的参考索引缓存(按形状对象的引用标识做 key)
            If refCache Is Nothing Then
                refCache = New Dictionary(Of Long, ReferenceIndex)
            End If

            ' 逐形状:on-the-fly 建索引并哈希连接,用完释放查询侧索引
            For si As Integer = 0 To seeds.Length - 1
                Dim seed = seeds(si)
                Dim seedKey = CLng(si)  ' 形状索引作为缓存 key

                ' 阶段 2:双索引(参考索引跨查询缓存复用)
                Dim refIdx As ReferenceIndex = Nothing

                If Not refCache.TryGetValue(seedKey, refIdx) Then
                    refIdx = New ReferenceIndex
                    refIdx.Build(subjectSeqs, seed)
                    refCache(seedKey) = refIdx
                End If

                Dim qIdx As New QueryIndex
                qIdx.Build(querySeq, seed)

                ' 阶段 2:哈希连接 -> 种子配对(先收集通过 Hamming 的命中)
                Dim passed As New List(Of SeedPair)
                leftMost.Reset()

                For Each pair In qIdx.HashJoin(refIdx)
                    ' 阶段 3a:Hamming 48aa 窗口初筛
                    If Not hamming.Pass(querySeq, pair.QueryPos, subjectSeqs(pair.SubjectId), pair.SubjectPos) Then
                        Continue For
                    End If

                    ' 阶段 3c:最左种子去冗余(跨形状累积)
                    If Not leftMost.Keep(pair.QueryPos, pair.SubjectId) Then
                        Continue For
                    End If

                    passed.Add(pair)
                Next

                ' 阶段 3b:BLOSUM62 无空位延伸
                '   SIMD 模式:对通过 Hamming 的命中按 (查询位置, 参考) 分块(≤32)做 AVX2 批量延伸;
                '   非 SIMD 模式:逐对调用标量 Extend。
                Dim candidates As New List(Of ScheduledHit)

                If TypeOf ungapped Is UngappedExtensionAvx2 Then
                    Dim avx2 = DirectCast(ungapped, UngappedExtensionAvx2)

                    ' 按查询位置 qPos 分组,每组内按参考分块批量延伸
                    For Each grp In passed.GroupBy(Function(p) p.QueryPos)
                        Dim qp = grp.Key
                        Dim items = grp.ToArray

                        For off As Integer = 0 To items.Length - 1 Step UngappedExtensionAvx2.MaxBatch
                            Dim cnt = Math.Min(UngappedExtensionAvx2.MaxBatch, items.Length - off)
                            Dim subs(cnt - 1) As String
                            Dim spos(cnt - 1) As Integer
                            Dim sids(cnt - 1) As Integer

                            For j As Integer = 0 To cnt - 1
                                subs(j) = subjectSeqs(items(off + j).SubjectId)
                                spos(j) = items(off + j).SubjectPos
                                sids(j) = items(off + j).SubjectId
                            Next

                            Dim ugs = avx2.ExtendBatch(querySeq, qp, subs, spos, sids)

                            For j As Integer = 0 To cnt - 1
                                Dim ug = ugs(j)

                                If ug.Score >= scheduler.MinUngappedScore Then
                                    Dim hamDist = hamming.Distance(querySeq, qp, subjectSeqs(sids(j)), spos(j))
                                    candidates.Add(New ScheduledHit(qp, sids(j), spos(j), ug, hamDist))
                                End If
                            Next
                        Next
                    Next
                Else
                    For Each pair In passed
                        Dim ug = ungapped.Extend(querySeq, pair.QueryPos, subjectSeqs(pair.SubjectId), pair.SubjectPos)

                        If ug.Score >= scheduler.MinUngappedScore Then
                            Dim hamDist = hamming.Distance(querySeq, pair.QueryPos, subjectSeqs(pair.SubjectId), pair.SubjectPos)
                            candidates.Add(New ScheduledHit(pair.QueryPos, pair.SubjectId, pair.SubjectPos, ug, hamDist))
                        End If
                    Next
                End If

                ' 阶段 3d + 4:排序启发式分块触发 -> 带状 SW
                For Each block In scheduler.Schedule(candidates)
                    Dim produced As Integer = 0

                    For Each cand In block
                        Dim bh = bandSW.Align(querySeq, subjectSeqs(cand.SubjectId), cand.Ungapped)

                        If bh.HasValue Then
                            Dim hit = DiamondHit.FromBandHit(querySeq, subjectSeqs(cand.SubjectId), queryTitle, subjectTitles(cand.SubjectId), bh.Value)
                            collected.Add(hit)
                            produced += 1
                        End If
                    Next

                    ' 早停:当前块不再产出达标比对即停止后续块
                    If scheduler.ShouldStop(produced) Then
                        Exit For
                    End If
                Next

                ' 显式释放查询侧索引(帮助 GC,控制峰值内存)
                qIdx = Nothing
            Next

            ' 聚合:多种子/多位置命中同一 (query, subject) 比对会产生重复 HSP。
            ' 按 subject 去重,每组仅保留得分最高的 HSP(原型阶段取每库序列最优比对)。
            Dim result = collected _
                .GroupBy(Function(h) h.SubjectTitle) _
                .Select(Function(g) g.OrderByDescending(Function(h) h.RawScore).First()) _
                .OrderByDescending(Function(h) h.RawScore) _
                .ToArray

            If maxHits > 0 AndAlso result.Length > maxHits Then
                Return result.Take(maxHits)
            End If

            Return result
        End Function

        ''' <summary>
        ''' 对多查询集合在参考库上执行 DIAMOND 风格 blastp 比对,支持并行/分布式调度。
        ''' 参考索引按形状在查询集合间缓存复用;每个查询独立运行单查询流水线,
        ''' 共享只读的参考索引、各自独立构建查询侧索引(线程安全)。
        ''' </summary>
        ''' <param name="querySet">查询蛋白序列集合。</param>
        ''' <param name="subjectDb">参考蛋白库(序列集合)。</param>
        ''' <param name="scheduler">调度器;为 Nothing 时使用默认并行调度 <see cref="ParallelScheduler"/>。</param>
        ''' <param name="maxHitsPerQuery">每条查询最多返回的命中数(0 表示不限)。</param>
        Public Function Search(querySet As IEnumerable(Of FastaSeq), subjectDb As IEnumerable(Of FastaSeq), Optional scheduler As IDiamondScheduler = Nothing, Optional maxHitsPerQuery As Integer = 0) As IEnumerable(Of DiamondHit)
            Dim subjects = subjectDb.ToArray
            Dim queries = querySet.ToArray

            If scheduler Is Nothing Then
                scheduler = New ParallelScheduler()
            End If

            ' 逐查询执行;调度器负责并行分发,内部共享同一 ReferenceIndex 缓存
            Dim perQuery As Func(Of FastaSeq, IEnumerable(Of DiamondHit)) =
                Function(q)
                    Dim refCache As Dictionary(Of Long, ReferenceIndex) = Nothing
                    Return SearchSingleCore(q, subjects, refCache, maxHitsPerQuery)
                End Function

            Return scheduler.Run(queries, subjects, perQuery)
        End Function
    End Class
End Namespace
