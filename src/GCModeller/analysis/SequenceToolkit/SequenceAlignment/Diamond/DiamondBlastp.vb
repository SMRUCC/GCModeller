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

        ''' <summary>
        ''' 构造 DIAMOND blastp 比对器。
        ''' </summary>
        ''' <param name="mode">灵敏度模式,默认 Fast。</param>
        Sub New(Optional mode As SensitivityMode = SensitivityMode.Fast)
            Me.Mode = mode
            Me.seeds = SpacedSeeds.GetSeeds(mode)
            Me.hamming = New HammingFilter()
            Me.ungapped = New UngappedExtension()
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
            Dim subjectSeqs As String() = subjects.Select(Function(s) s.SequenceData).ToArray
            Dim subjectTitles As String() = subjects.Select(Function(s) s.Title).ToArray
            Dim querySeq = query.SequenceData
            Dim queryTitle = query.Title

            Dim collected As New List(Of DiamondHit)

            ' 逐形状:on-the-fly 建索引并哈希连接,用完释放查询侧索引
            For si As Integer = 0 To seeds.Length - 1
                Dim seed = seeds(si)

                ' 阶段 2:双索引
                Dim refIdx As New ReferenceIndex
                refIdx.Build(subjectSeqs, seed)

                Dim qIdx As New QueryIndex
                qIdx.Build(querySeq, seed)

                ' 阶段 2:哈希连接 -> 种子配对
                Dim candidates As New List(Of ScheduledHit)
                leftMost.Reset()

                For Each pair In qIdx.HashJoin(refIdx)
                    ' 阶段 3a:Hamming 48aa 窗口初筛
                    If Not hamming.Pass(querySeq, pair.QueryPos, subjectSeqs(pair.SubjectId), pair.SubjectPos) Then
                        Continue For
                    End If

                    Dim hamDist = hamming.Distance(querySeq, pair.QueryPos, subjectSeqs(pair.SubjectId), pair.SubjectPos)

                    ' 阶段 3b:BLOSUM62 无空位延伸
                    Dim ug = ungapped.Extend(querySeq, pair.QueryPos, subjectSeqs(pair.SubjectId), pair.SubjectPos)

                    If ug.Score < scheduler.MinUngappedScore Then
                        Continue For
                    End If

                    ' 阶段 3c:最左种子去冗余(跨形状累积)
                    If Not leftMost.Keep(pair.QueryPos, pair.SubjectId) Then
                        Continue For
                    End If

                    candidates.Add(New ScheduledHit(pair.QueryPos, pair.SubjectId, pair.SubjectPos, ug, hamDist))
                Next

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

            ' 按原始 SW 得分降序排序并截断
            Dim result = collected _
                .OrderByDescending(Function(h) h.RawScore) _
                .ToArray

            If maxHits > 0 AndAlso result.Length > maxHits Then
                Return result.Take(maxHits)
            End If

            Return result
        End Function
    End Class
End Namespace
