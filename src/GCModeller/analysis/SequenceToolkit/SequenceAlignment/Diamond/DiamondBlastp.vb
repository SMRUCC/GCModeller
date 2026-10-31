' DIAMOND 顶层比对入口 (DiamondBlastp)
'
' 将整条 DIAMOND 流水线编排为可复用 API:对查询蛋白序列集合 vs 参考蛋白库
' 执行 blastp 加速比对。
'
' 流水线(按文档四阶段):
'   1) 种子生成:缩减字母表 + 间隔种子(按灵敏度模式选形状集);
'   2) 双索引与哈希连接:查询侧紧凑索引 + 参考库流式扫描(见 DiamondScanner);
'   3) 分层过滤链:48aa 窗口 Hamming 初筛 -> BLOSUM62 无空位延伸 -> 候选去重/截断;
'   4) 带状 Smith-Waterman 扩展:仅对候选窗口做有空位局部比对;
'   5) 结果聚合:产出 m8 风格的 <see cref="DiamondHit"/> 集合。
'
' 并行模型(本次重构的重点):
'   阶段 A —— 候选生成:参考库按残基预算切块,以 (块 × 种子形状) 为任务并行扫描,
'             线程本地累积候选,任务结束后按查询下标分片合并;
'   阶段 B —— 局部比对:交由 <see cref="IDiamondScheduler"/> 按查询并行执行,
'             每条查询只对自己去重后的候选做 SW。
'
' 与历史实现相比修复的并行缺陷:
'   - 不再使用跨查询共享的可变过滤器实例(LeftMostSeedFilter 数据竞争);
'   - 不再在 SyncLock 内构建参考库索引(原先等于把整个构建阶段串行化);
'   - 不再为每条查询重复生成参考库字符串数组(O(查询数 × 库规模) 的重复分配);
'   - 不再对参考库构建十亿级键值索引(内存从数十 GB 降到数百 MB 量级)。

Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Runtime.Intrinsics.X86
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    Public Class DiamondBlastp

        ''' <summary>选择的灵敏度模式(决定间隔种子形状集)。</summary>
        Public ReadOnly Mode As SensitivityMode

        ''' <summary>是否启用 SIMD 向量化(Hamming / 无空位延伸)。</summary>
        Public ReadOnly UseSimd As Boolean

        Private ReadOnly seeds As SpacedSeed()
        Private ReadOnly scorer As AminoAcidScorer
        Private ReadOnly hamming As IHammingFilter
        Private ReadOnly ungapped As UngappedExtension
        Private ReadOnly hitSched As HitScheduler
        Private ReadOnly bandSW As BandSW

        ''' <summary>
        ''' 最近一次多查询搜索的阶段耗时画像(便于定位瓶颈与基准对比)。
        ''' </summary>
        Public Property LastSearchProfile As DiamondSearchProfile

        ''' <summary>
        ''' 构造 DIAMOND blastp 比对器。
        ''' </summary>
        ''' <param name="mode">灵敏度模式,默认 Fast。</param>
        ''' <param name="useSimd">是否启用 SIMD 向量化;运行时还会按 IsSupported 进一步判定。</param>
        Sub New(Optional mode As SensitivityMode = SensitivityMode.Fast, Optional useSimd As Boolean = True)
            Me.Mode = mode
            Me.UseSimd = useSimd
            Me.seeds = SpacedSeeds.GetSeeds(mode)
            Me.scorer = AminoAcidScorer.DefaultScorer

            ' 运行时选择 Hamming 实现(两者内部都是"预编码 + 同类查表 + 早退"的快速路径)
            If useSimd AndAlso Sse2.IsSupported Then
                Me.hamming = New HammingFilterSse(scorer:=Me.scorer)
            Else
                Me.hamming = New HammingFilter(scorer:=Me.scorer)
            End If

            ' 运行时选择无空位延伸实现(扁平打分查表)
            If useSimd AndAlso Avx2.IsSupported Then
                Me.ungapped = New UngappedExtensionAvx2(blosum:=Nothing)
            Else
                Me.ungapped = New UngappedExtension(blosum:=Nothing)
            End If

            Me.hitSched = New HitScheduler()
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
            Dim noScheduler As IDiamondScheduler = Nothing
            Return Search(New FastaSeq() {query}, subjectDb, noScheduler, maxHits)
        End Function

        ''' <summary>
        ''' 对多查询集合在参考库上执行 DIAMOND 风格 blastp 比对,支持并行/分布式调度。
        ''' </summary>
        ''' <param name="querySet">查询蛋白序列集合。</param>
        ''' <param name="subjectDb">参考蛋白库(序列集合)。</param>
        ''' <param name="scheduler">调度器;为 Nothing 时使用默认并行调度 <see cref="ParallelScheduler"/>。</param>
        ''' <param name="maxHitsPerQuery">每条查询最多返回的命中数(0 表示不限)。</param>
        Public Function Search(querySet As IEnumerable(Of FastaSeq), subjectDb As IEnumerable(Of FastaSeq),
                               Optional scheduler As IDiamondScheduler = Nothing,
                               Optional maxHitsPerQuery As Integer = 0) As IEnumerable(Of DiamondHit)

            Dim options As DiamondSearchOptions = DiamondSearchOptions.CreateDefault()
            Return Search(querySet, subjectDb, options, scheduler, maxHitsPerQuery)
        End Function

        ''' <summary>
        ''' 使用显式参数执行多查询比对(便于性能调优与基准测试)。
        ''' </summary>
        Public Function Search(querySet As IEnumerable(Of FastaSeq), subjectDb As IEnumerable(Of FastaSeq),
                               options As DiamondSearchOptions,
                               Optional scheduler As IDiamondScheduler = Nothing,
                               Optional maxHitsPerQuery As Integer = 0) As IEnumerable(Of DiamondHit)

            If options Is Nothing Then
                options = DiamondSearchOptions.CreateDefault()
            End If

            Dim queries As FastaSeq() = querySet.ToArray
            Dim subjects As FastaSeq() = subjectDb.ToArray

            If queries.Length = 0 OrElse subjects.Length = 0 Then
                Return New DiamondHit() {}
            End If

            If scheduler Is Nothing Then
                scheduler = New ParallelScheduler(options.ResolveDegreeOfParallelism())
            End If

            Dim log As Action(Of String) = Nothing

            If options.ProgressLogInterval > 0 Then
                log = Sub(msg) Call msg.debug
                Call $"diamond_blastp: mode={Mode}, queries={queries.Length}, subjects={subjects.Length}, options=[{options}]".debug
            End If

            ' 阶段 A:候选生成(参考库流式扫描,x 种子形状并行)
            Dim profile As New DiamondSearchProfile
            Dim scanner As New DiamondScanner(queries, subjects, seeds, scorer, hamming, ungapped, options)
            Dim candidates As DiamondCandidate()() = scanner.Scan(log, profile)

            ' 阶段 B:按查询并行做局部比对(由调度器决定并行策略)
            Dim lookup As New SequenceIndexLookup(queries)
            Dim maxSw As Integer = options.ResolveMaxSwPerQuery(maxHitsPerQuery)
            Dim clock As Stopwatch = Stopwatch.StartNew()

            Dim perQuery As Func(Of FastaSeq, IEnumerable(Of DiamondHit)) =
                Function(q As FastaSeq)
                    Dim qi As Integer = lookup.IndexOf(q)
                    Return AlignQuery(qi, scanner, candidates, maxSw, maxHitsPerQuery)
                End Function

            Dim hits As IEnumerable(Of DiamondHit) = scheduler.Run(queries, subjects, perQuery)

            profile.AlignMilliseconds = clock.Elapsed.TotalMilliseconds
            profile.Hits = If(TypeOf hits Is ICollection(Of DiamondHit),
                              DirectCast(hits, ICollection(Of DiamondHit)).Count,
                              0)
            Me.LastSearchProfile = profile

            Return hits
        End Function

        ''' <summary>
        ''' 阶段 B:对指定查询的候选集合执行带状 SW,并聚合为 m8 风格的命中。
        ''' </summary>
        Private Function AlignQuery(qi As Integer, scanner As DiamondScanner, candidates As DiamondCandidate()(),
                                    maxSw As Integer, maxHits As Integer) As DiamondHit()

            If qi < 0 OrElse qi >= candidates.Length Then
                Return New DiamondHit() {}
            End If

            Dim list As DiamondCandidate() = candidates(qi)

            If list.Length = 0 Then
                Return New DiamondHit() {}
            End If

            Dim queries As SequenceDbView = scanner.Queries
            Dim subjects As SequenceDbView = scanner.Subjects
            Dim qSeq As String = queries.Seqs(qi)
            Dim qTitle As String = queries.Titles(qi)
            Dim qBase As Integer = queries.Offsets(qi)
            Dim found As New List(Of DiamondHit)
            Dim take As Integer = Math.Min(Math.Max(1, maxSw), list.Length)
            Dim block As Integer = Math.Max(1, hitSched.BlockSize)
            Dim i As Integer = 0

            While i < take
                Dim last As Integer = Math.Min(i + block, take) - 1
                Dim produced As Integer = 0

                For k As Integer = i To last
                    Dim cand As DiamondCandidate = list(k)
                    Dim sid As Integer = cand.SubjectId
                    Dim sSeq As String = subjects.Seqs(sid)

                    If String.IsNullOrEmpty(sSeq) Then
                        Continue For
                    End If

                    Dim bh As BandHit? = bandSW.Align(qSeq, queries.Packed, qBase, sSeq, subjects.Packed, subjects.Offsets(sid), cand.Hit)

                    If bh.HasValue Then
                        Call found.Add(DiamondHit.FromBandHit(qSeq, sSeq, qTitle, subjects.Titles(sid), bh.Value))
                        produced += 1
                    End If
                Next

                ' 早停:当前块不再产出达标比对即停止后续块(候选已按无空位得分降序)
                If hitSched.ShouldStop(produced) Then
                    Exit While
                End If

                i = last + 1
            End While

            Dim result As DiamondHit() = found _
                .OrderByDescending(Function(h) h.RawScore) _
                .ToArray

            If maxHits > 0 AndAlso result.Length > maxHits Then
                Array.Resize(result, maxHits)
            End If

            Return result
        End Function
    End Class

    ''' <summary>
    ''' 依据对象引用把查询序列映射回其在查询数组中的下标。
    ''' (调度器回传的是同一批 FastaSeq 实例,因此引用相等即下标相等。)
    ''' </summary>
    Friend NotInheritable Class SequenceIndexLookup

        Private ReadOnly map As Dictionary(Of FastaSeq, Integer)

        Friend Sub New(queries As FastaSeq())
            Me.map = New Dictionary(Of FastaSeq, Integer)(queries.Length, ReferenceComparer.Instance)

            For i As Integer = 0 To queries.Length - 1
                Dim q As FastaSeq = queries(i)

                If q IsNot Nothing AndAlso Not map.ContainsKey(q) Then
                    map(q) = i
                End If
            Next
        End Sub

        Friend Function IndexOf(q As FastaSeq) As Integer
            Dim i As Integer = -1

            If q IsNot Nothing AndAlso map.TryGetValue(q, i) Then
                Return i
            End If

            Return -1
        End Function

        Private NotInheritable Class ReferenceComparer : Implements IEqualityComparer(Of FastaSeq)

            Friend Shared ReadOnly Instance As New ReferenceComparer

            Public Overloads Function Equals(a As FastaSeq, b As FastaSeq) As Boolean Implements IEqualityComparer(Of FastaSeq).Equals
                Return Object.ReferenceEquals(a, b)
            End Function

            Public Overloads Function GetHashCode(obj As FastaSeq) As Integer Implements IEqualityComparer(Of FastaSeq).GetHashCode
                Return RuntimeHelpers.GetHashCode(obj)
            End Function
        End Class
    End Class
End Namespace
