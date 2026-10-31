' DIAMOND 候选生成引擎 (Seed -> Candidate Scanner)
'
' 这是本次并行重构的核心:把"参考库建索引 + 每查询哈希连接"反转为
' "查询建索引 + 参考库流式扫描"。
'
' 为什么反转?
'   参考库实测规模可达 134 万条 / 5.92 亿残基。若对参考库建
'   Dictionary(Of Long, List(Of SubjectHit)) 索引,单形状就有近 6 亿键值项,
'   两个形状超过 11 亿项 —— 内存需求数十 GB,必然换页并拖垮 GC。
'   而查询集通常只有数千条、数百万残基,索引常驻内存仅数十 MB,
'   且能被 CPU 三级缓存覆盖,探测延迟极低。
'
' 并行结构:
'   参考库按残基预算切块(SequenceRange),每个 (块, 种子形状) 是一个独立任务;
'   任务之间互不通信,各自把命中的候选写入线程本地字典,
'   任务结束时再按查询下标分片合并(减少锁竞争)。
'   这既保证并行度随 CPU 核心数线性扩展,也彻底消除了原实现中
'   "首个线程在 SyncLock 内建全库索引、其余线程全部阻塞"的伪并行。
'
' 结果一经合并即可交给下游调度器按查询并行做局部比对(阶段 B)。

Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    ''' <summary>
    ''' 一条候选目标(已完成种子命中 + Hamming 初筛 + 无空位延伸)。
    ''' </summary>
    Public Structure DiamondCandidate
        Public ReadOnly SubjectId As Integer
        Public ReadOnly Hit As UngappedHit

        Sub New(subjectId As Integer, hit As UngappedHit)
            Me.SubjectId = subjectId
            Me.Hit = hit
        End Sub
    End Structure

    ''' <summary>
    ''' 参考库流式扫描引擎:产生每条查询的候选目标集合。
    ''' </summary>
    Friend NotInheritable Class DiamondScanner

        Friend ReadOnly Queries As SequenceDbView
        Friend ReadOnly Subjects As SequenceDbView
        Friend ReadOnly Scorer As AminoAcidScorer

        Private ReadOnly queryLens As Integer()
        Private ReadOnly plans As SeedPlan()
        Private ReadOnly tables As QuerySeedTable()
        Private ReadOnly reducedOfCode As Byte()
        Private ReadOnly hamming As IHammingFilter
        Private ReadOnly ungapped As UngappedExtension
        Private ReadOnly options As DiamondSearchOptions

        Friend Sub New(queries As IList(Of FastaSeq), subjects As IList(Of FastaSeq),
                       seeds As SpacedSeed(), scorer As AminoAcidScorer,
                       hamming As IHammingFilter, ungapped As UngappedExtension,
                       options As DiamondSearchOptions)

            Me.Queries = SequenceDbView.Build(queries, scorer)
            Me.Subjects = SequenceDbView.Build(subjects, scorer)
            Me.Scorer = scorer
            Me.reducedOfCode = scorer.ReducedOfCode
            Me.hamming = hamming
            Me.ungapped = ungapped
            Me.options = options

            Me.plans = New SeedPlan(seeds.Length - 1) {}
            Me.tables = New QuerySeedTable(seeds.Length - 1) {}

            For i As Integer = 0 To seeds.Length - 1
                Me.plans(i) = New SeedPlan(seeds(i))
            Next

            Me.queryLens = New Integer(Queries.Count - 1) {}

            For i As Integer = 0 To Queries.Count - 1
                Me.queryLens(i) = Queries.LengthOf(i)
            Next
        End Sub

        ''' <summary>查询序列数。</summary>
        Friend ReadOnly Property QueryCount As Integer
            Get
                Return Queries.Count
            End Get
        End Property

        ''' <summary>
        ''' 执行阶段 A:并行扫描参考库,返回每条查询的候选目标(按无空位得分降序)。
        ''' </summary>
        Friend Function Scan(Optional progress As Action(Of String) = Nothing) As DiamondCandidate()()
            Dim nq As Integer = Queries.Count
            Dim result As DiamondCandidate()() = New DiamondCandidate(nq - 1)() {}

            If nq = 0 OrElse Subjects.Count = 0 Then
                Return result
            End If

            BuildQueryIndexes()

            Dim blocks As List(Of SequenceRange) = Subjects.Blocks(options.BlockResidues)
            Dim shapes As Integer = plans.Length
            Dim tasks As Integer = blocks.Count * shapes
            Dim shards As New CandidateShard(32, options.ResolveMaxCandidates())
            Dim opt As New ParallelOptions With {.MaxDegreeOfParallelism = options.ResolveDegreeOfParallelism()}
            Dim completed As Integer = 0
            Dim interval As Integer = options.ProgressLogInterval

            Call Parallel.For(Of Dictionary(Of Long, UngappedHit))(
                0, tasks, opt,
                Function() New Dictionary(Of Long, UngappedHit),
                Function(t As Integer, state As ParallelLoopState, local As Dictionary(Of Long, UngappedHit))
                    Dim bi As Integer = t \ shapes
                    Dim si As Integer = t - bi * shapes

                    Call ScanBlock(blocks(bi), plans(si), tables(si), local)

                    If interval > 0 Then
                        Dim done = System.Threading.Interlocked.Increment(completed)

                        If done Mod interval = 0 Then
                            progress?.Invoke($"diamond_scan: {done}/{tasks} tasks")
                        End If
                    End If

                    Return local
                End Function,
                Sub(local As Dictionary(Of Long, UngappedHit))
                    Call shards.Merge(local)
                End Sub)

            For qi As Integer = 0 To nq - 1
                result(qi) = shards.Export(qi)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 为每个种子形状构建查询侧索引(小索引,常驻内存)。
        ''' </summary>
        Private Sub BuildQueryIndexes()
            For i As Integer = 0 To plans.Length - 1
                tables(i) = QuerySeedTable.Build(Queries, plans(i), reducedOfCode)
            Next
        End Sub

        ''' <summary>
        ''' 扫描一个参考库块(单一形状),把通过的候选写入线程本地字典。
        ''' 键为 (查询下标 &lt;&lt; 32) Or 参考序列下标。
        ''' </summary>
        Private Sub ScanBlock(range As SequenceRange, plan As SeedPlan, table As QuerySeedTable,
                              sink As Dictionary(Of Long, UngappedHit))

            Dim packed As Byte() = Subjects.Packed
            Dim offsets As Integer() = Subjects.Offsets
            Dim qPacked As Byte() = Queries.Packed
            Dim qOffsets As Integer() = Queries.Offsets
            Dim span As Integer = plan.Span
            Dim cap As Integer = options.ResolveSeedFrequencyCap()
            Dim minScore As Integer = options.MinUngappedScore
            Dim codes As Long() = table.codes
            Dim nexts As Integer() = table.nexts
            Dim vals As Long() = table.vals

            For sid As Integer = range.Start To range.[End] - 1
                Dim sOff As Integer = offsets(sid)
                Dim sLen As Integer = offsets(sid + 1) - sOff
                Dim last As Integer = sLen - span

                If last < 0 Then
                    Continue For
                End If

                For p As Integer = 0 To last
                    Dim code As Long = plan.Encode(packed, reducedOfCode, sOff + p)
                    Dim e As Integer = table.HeadOf(code)
                    Dim walked As Integer = 0
                    Dim sPos As Integer = sOff + p

                    While e >= 0 AndAlso walked < cap
                        If codes(e) = code Then
                            Dim v As Long = vals(e)
                            Dim qi As Integer = CInt(v >> 32)
                            Dim qPos As Integer = CInt(v - (CLng(qi) << 32))

                            ' 阶段 3a:48aa 窗口 Hamming 初筛(预编码字节 + 同类查表)
                            If hamming.PassPacked(qPacked, qOffsets(qi), queryLens(qi), qPos, packed, sOff, sLen, p) Then
                                ' 阶段 3b:BLOSUM62 无空位延伸(扁平打分查表)
                                Dim ug As UngappedHit = ungapped.Extend(qPacked, qOffsets(qi), queryLens(qi), qPos, packed, sOff, sLen, p)

                                If ug.Score >= minScore Then
                                    Dim key As Long = (CLng(qi) << 32) Or sid
                                    Dim old As UngappedHit

                                    If Not sink.TryGetValue(key, old) OrElse ug.Score > old.Score Then
                                        sink(key) = ug
                                    End If
                                End If
                            End If
                        End If

                        e = nexts(e)
                        walked += 1
                    End While
                Next
            Next
        End Sub
    End Class

    ''' <summary>
    ''' 候选集合的分片存储:按查询下标分片,降低并行合并时的锁竞争。
    ''' </summary>
    Friend NotInheritable Class CandidateShard

        Private ReadOnly locks As Object()
        Private ReadOnly maps As Dictionary(Of Integer, Dictionary(Of Integer, UngappedHit))()
        Private ReadOnly mask As Integer
        Private ReadOnly maxPerQuery As Integer

        Friend Sub New(count As Integer, maxPerQuery As Integer)
            Dim n As Integer = QuerySeedTable.NextPowerOfTwo(Math.Max(1, count))

            Me.mask = n - 1
            Me.maxPerQuery = Math.Max(16, maxPerQuery)
            Me.locks = New Object(n - 1) {}
            Me.maps = New Dictionary(Of Integer, Dictionary(Of Integer, UngappedHit))(n - 1) {}

            For i As Integer = 0 To n - 1
                locks(i) = New Object
            Next
        End Sub

        ''' <summary>
        ''' 把线程本地的候选合并到全局分片。
        ''' </summary>
        Friend Sub Merge(local As Dictionary(Of Long, UngappedHit))
            If local.Count = 0 Then
                Return
            End If

            Dim n As Integer = locks.Length
            Dim buckets As List(Of KeyValuePair(Of Long, UngappedHit))() = New List(Of KeyValuePair(Of Long, UngappedHit))(n - 1) {}

            For Each kvp In local
                Dim qi As Integer = CInt(kvp.Key >> 32)
                Dim s As Integer = qi And mask

                If buckets(s) Is Nothing Then
                    buckets(s) = New List(Of KeyValuePair(Of Long, UngappedHit))
                End If

                buckets(s).Add(kvp)
            Next

            For s As Integer = 0 To n - 1
                Dim bucket = buckets(s)

                If bucket Is Nothing Then
                    Continue For
                End If

                SyncLock locks(s)
                    Dim map = maps(s)

                    If map Is Nothing Then
                        map = New Dictionary(Of Integer, Dictionary(Of Integer, UngappedHit))
                        maps(s) = map
                    End If

                    For Each kvp In bucket
                        Dim qi As Integer = CInt(kvp.Key >> 32)
                        Dim sid As Integer = CInt(kvp.Key - (CLng(qi) << 32))
                        Dim inner As Dictionary(Of Integer, UngappedHit) = Nothing

                        If Not map.TryGetValue(qi, inner) Then
                            inner = New Dictionary(Of Integer, UngappedHit)
                            map(qi) = inner
                        End If

                        Dim old As UngappedHit

                        If Not inner.TryGetValue(sid, old) OrElse kvp.Value.Score > old.Score Then
                            inner(sid) = kvp.Value
                        End If

                        If inner.Count > maxPerQuery * 2 Then
                            Prune(inner)
                        End If
                    Next
                End SyncLock
            Next
        End Sub

        ''' <summary>
        ''' 候选数量超过上限时,按无空位得分保留最高的 <see cref="maxPerQuery"/> 条。
        ''' </summary>
        Private Sub Prune(inner As Dictionary(Of Integer, UngappedHit))
            Dim keep = inner _
                .OrderByDescending(Function(kv) kv.Value.Score) _
                .Take(maxPerQuery) _
                .ToArray

            inner.Clear()

            For Each kv In keep
                inner(kv.Key) = kv.Value
            Next
        End Sub

        ''' <summary>
        ''' 导出指定查询的候选集合(按无空位得分降序)。
        ''' </summary>
        Friend Function Export(qi As Integer) As DiamondCandidate()
            Dim s As Integer = qi And mask
            Dim inner As Dictionary(Of Integer, UngappedHit) = Nothing

            SyncLock locks(s)
                Dim map = maps(s)

                If map IsNot Nothing Then
                    Call map.TryGetValue(qi, inner)
                End If
            End SyncLock

            If inner Is Nothing OrElse inner.Count = 0 Then
                Return New DiamondCandidate() {}
            End If

            Dim list = inner _
                .OrderByDescending(Function(kv) kv.Value.Score) _
                .Select(Function(kv) New DiamondCandidate(kv.Key, kv.Value)) _
                .ToArray

            If list.Length > maxPerQuery Then
                Array.Resize(list, maxPerQuery)
            End If

            Return list
        End Function
    End Class
End Namespace
