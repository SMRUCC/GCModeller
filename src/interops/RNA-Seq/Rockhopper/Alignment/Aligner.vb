' /********************************************************************************/
'
'  Rockhopper —— 比对器
'
'  组合 FM-index（精确匹配）与种子-延伸（非精确比对），复刻论文的参考依赖比对流程：
'    第一步：在读段上做 FM-index 反向搜索，命中则记为 0 错配命中；
'    第二步：精确匹配失败时，按 percentSeedLength 取种子，定位候选位点后用
'            质量感知动态规划延伸，允许错配/缺口；
'    第三步：按 percentMismatches 过滤；stopAfterOneHit=True 时保留首个最优命中。
'
'  同时支持：正/负链、单端反向互补(-c)、双端方向(ff/fr/rf/rr)与最大间距(-d)、
'  读段级多线程。原 Peregrine/Alignment 中的全局静态字段改为实例属性。
'
' /********************************************************************************/

Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Namespace Alignment

    ''' <summary>
    ''' 一次比对命中。
    ''' </summary>
    Public Class AlignmentHit
        ''' <summary>复制子下标。</summary>
        Public Property RepliconIndex As Integer
        ''' <summary>1-based 最左位置。</summary>
        Public Property Position As Integer
        ''' <summary>是否比对到负链。</summary>
        Public Property IsReverse As Boolean
        ''' <summary>读段标识。</summary>
        Public Property ReadID As String
        ''' <summary>读段序列（原始正向）。</summary>
        Public Property Sequence As String
        ''' <summary>质量字符串。</summary>
        Public Property Quality As String
        ''' <summary>简化 CIGAR。</summary>
        Public Property Cigar As String
        ''' <summary>错配数。</summary>
        Public Property Mismatches As Integer
        ''' <summary>双端 mate 下标（-1 表示单端）。</summary>
        Public Property PairedEndIndex As Integer = -1
    End Class

    ''' <summary>
    ''' 参考依赖比对器。
    ''' </summary>
    Public Class Aligner

        Private ReadOnly replicons As Core.Replicon()
        Private indexes As FMIndex()

#Region "Options"

        Public ReadOnly Property PercentMismatches As Double
        Public ReadOnly Property PercentSeedLength As Double
        Public ReadOnly Property StopAfterOneHit As Boolean
        Public ReadOnly Property Unstranded As Boolean
        Public ReadOnly Property NumThreads As Integer
        Public ReadOnly Property MaxPairedEndLength As Integer
        Public ReadOnly Property SingleEndReverseComplement As Boolean
        Public ReadOnly Property PairedEndOrientation As String

#End Region

        ''' <summary>用于评估候选种子位点的上限，避免重复序列导致组合爆炸。</summary>
        Public Property MaxSeedCandidates As Integer = 8

        Public ReadOnly Property NumSequences As Integer
            Get
                Return replicons.Length
            End Get
        End Property

        Public Sub New(replicons As Core.Replicon(),
                       Optional percentMismatches As Double = 0.15,
                       Optional percentSeedLength As Double = 0.33,
                       Optional stopAfterOneHit As Boolean = True,
                       Optional unstranded As Boolean = False,
                       Optional numThreads As Integer = 1,
                       Optional maxPairedEndLength As Integer = 500,
                       Optional singleEndReverseComplement As Boolean = False,
                       Optional pairedEndOrientation As String = "fr")

            Me.replicons = If(replicons, New Core.Replicon() {})
            Me.PercentMismatches = percentMismatches
            Me.PercentSeedLength = percentSeedLength
            Me.StopAfterOneHit = stopAfterOneHit
            Me.Unstranded = unstranded
            Me.NumThreads = System.Math.Max(1, numThreads)
            Me.MaxPairedEndLength = maxPairedEndLength
            Me.SingleEndReverseComplement = singleEndReverseComplement
            Me.PairedEndOrientation = If(pairedEndOrientation, "fr")
        End Sub

        ''' <summary>
        ''' 构建全部复制子的 FM 索引（延迟调用、构建后缓存）。
        ''' </summary>
        Public Sub BuildIndex()
            If indexes IsNot Nothing AndAlso indexes.Length = replicons.Length Then Return
            SyncLock replicons
                If indexes IsNot Nothing AndAlso indexes.Length = replicons.Length Then Return
                Dim built As FMIndex() = New FMIndex(replicons.Length - 1) {}
                For i As Integer = 0 To replicons.Length - 1
                    built(i) = New FMIndex(replicons(i).SequenceData)
                Next
                indexes = built
            End SyncLock
        End Sub

        Private ReadOnly Property Index(i As Integer) As FMIndex
            Get
                If indexes Is Nothing Then BuildIndex()
                Return indexes(i)
            End Get
        End Property

#Region "Single-end alignment"

        ''' <summary>
        ''' 单端比对。
        ''' </summary>
        Public Function AlignReads(reads As IEnumerable(Of FileIO.Read)) As IEnumerable(Of AlignmentHit)
            BuildIndex()
            Dim source As FileIO.Read() = reads.ToArray()

            If NumThreads <= 1 OrElse source.Length < 256 Then
                Return source.SelectMany(Function(r) AlignSingle(r)).ToArray()
            End If

            Dim results As New ConcurrentBag(Of AlignmentHit)()
            Dim partition As Integer = System.Math.Max(1, source.Length \ NumThreads)
            Call Parallel.For(0, NumThreads,
                Sub(t)
                    Dim lo As Integer = t * partition
                    Dim hi As Integer = If(t = NumThreads - 1, source.Length, lo + partition)
                    For i As Integer = lo To hi - 1
                        For Each hit In AlignSingle(source(i))
                            results.Add(hit)
                        Next
                    Next
                End Sub)
            Return results.ToArray()
        End Function

        ''' <summary>
        ''' 对单条读段执行"精确匹配 → 种子-延伸"。
        ''' </summary>
        Public Iterator Function AlignSingle(read As FileIO.Read) As IEnumerable(Of AlignmentHit)
            Dim hits As List(Of AlignmentHit) = alignOneStrand(read, False)
            If hits.Count = 0 Then
                ' 正向链无命中时尝试反向互补链
                Dim rc As String = reverseComplement(read.Sequence)
                Dim rcScores As Integer() = If(read.Scores Is Nothing, Nothing, read.Scores.Reverse().ToArray())
                hits = alignOneStrand(New FileIO.Read(read.ID, rc, read.Quality), True, rcScores)
            End If

            For Each hit In hits
                Yield hit
            Next
        End Function

        ''' <summary>
        ''' 在指定方向（正向/反向互补）上比对一条读段。
        ''' </summary>
        Private Function alignOneStrand(read As FileIO.Read, isReverse As Boolean, Optional scoresOverride As Integer() = Nothing) As List(Of AlignmentHit)
            Dim results As New List(Of AlignmentHit)()
            Dim seq As String = read.Sequence
            If String.IsNullOrEmpty(seq) Then Return results

            Dim scores As Integer() = If(scoresOverride, read.Scores)
            Dim maxMismatches As Integer = CInt(System.Math.Floor(PercentMismatches * seq.Length))

            For z As Integer = 0 To replicons.Length - 1
                Dim reference As String = replicons(z).SequenceData
                If reference.Length = 0 Then Continue For
                Dim fmi As FMIndex = Index(z)

                ' 第一步：精确匹配
                Dim exact As Integer() = fmi.Locate(seq, If(StopAfterOneHit, 1, Integer.MaxValue))
                If exact.Length > 0 Then
                    For Each pos As Integer In exact
                        results.Add(makeHit(z, pos, isReverse, read, "M", 0))
                    Next
                    If StopAfterOneHit Then Return results
                    Continue For
                End If

                ' 第二步：种子-延伸
                Dim hit As AlignmentHit = runSeedExtend(fmi, z, reference, seq, scores, maxMismatches, isReverse, read)
                If hit IsNot Nothing Then
                    results.Add(hit)
                    If StopAfterOneHit Then Return results
                End If
            Next

            Return results
        End Function

        ''' <summary>
        ''' 种子-延伸：取最小种子，定位候选位点，并在候选窗口内做质量感知 DP。
        ''' </summary>
        Private Function runSeedExtend(fmi As FMIndex, z As Integer, reference As String, seq As String,
                                    scores As Integer(), maxMismatches As Integer,
                                    isReverse As Boolean, read As FileIO.Read) As AlignmentHit
            Dim seedLen As Integer = System.Math.Max(1, CInt(System.Math.Floor(PercentSeedLength * seq.Length)))
            If seedLen > seq.Length Then seedLen = seq.Length

            Dim best As DpResult? = Nothing
            Dim slack As Integer = System.Math.Max(4, maxMismatches + 4)

            ' 尝试多个种子偏移，优先使用读段中部（受测序错误影响较小）
            Dim offsets As New List(Of Integer)()
            Dim center As Integer = System.Math.Max(0, (seq.Length - seedLen) \ 2)
            offsets.Add(center)
            For d As Integer = 1 To seq.Length - seedLen
                If center - d >= 0 Then offsets.Add(center - d)
                If center + d <= seq.Length - seedLen Then offsets.Add(center + d)
                If offsets.Count >= 12 Then Exit For
            Next

            For Each offset As Integer In offsets
                Dim seed As String = seq.Substring(offset, seedLen)
                If seed.Contains("N"c) Then Continue For

                Dim positions As Integer() = fmi.Locate(seed, MaxSeedCandidates)
                For Each p As Integer In positions
                    ' 种子在参考上的起点 p（1-based），读段起点 ≈ p - offset
                    Dim windowStart As Integer = System.Math.Max(1, p - offset - slack)
                    Dim windowEnd As Integer = System.Math.Min(reference.Length, windowStart + seq.Length + 2 * slack)
                    If windowEnd - windowStart + 1 < seq.Length Then
                        windowStart = System.Math.Max(1, windowEnd - seq.Length * 2 - 1)
                        If windowEnd - windowStart + 1 < seq.Length Then Continue For
                    End If

                    Dim dp As DpResult = SeedExtend.Align(seq, scores, reference, windowStart, windowEnd)
                    If Not dp.IsValid Then Continue For
                    If dp.Mismatches + dp.Gaps > maxMismatches Then Continue For

                    If best Is Nothing Then
                        best = dp
                    ElseIf dp.Score > best.Value.Score Then
                        best = dp
                    End If

                    If StopAfterOneHit AndAlso best.HasValue Then
                        Exit For
                    End If
                Next

                If StopAfterOneHit AndAlso best.HasValue Then Exit For
            Next

            If Not best.HasValue Then Return Nothing
            Return makeHit(z, best.Value.Start, isReverse, read, best.Value.Cigar, best.Value.Mismatches)
        End Function

        Private Function makeHit(z As Integer, pos As Integer, isReverse As Boolean, read As FileIO.Read, cigar As String, mismatches As Integer) As AlignmentHit
            Return New AlignmentHit With {
                .RepliconIndex = z,
                .Position = pos,
                .IsReverse = isReverse,
                .ReadID = read.ID,
                .Sequence = read.Sequence,
                .Quality = read.Quality,
                .Cigar = cigar,
                .Mismatches = mismatches
            }
        End Function

#End Region

#Region "Paired-end alignment"

        ''' <summary>
        ''' 双端比对：按 pairedEndOrientation 判定两 mate 的方向关系，间距不超过 maxPairedEndLength。
        ''' </summary>
        Public Function AlignPaired(mate1 As IEnumerable(Of FileIO.Read), mate2 As IEnumerable(Of FileIO.Read)) As IEnumerable(Of AlignmentHit)
            BuildIndex()
            Dim m1 As FileIO.Read() = mate1.ToArray()
            Dim m2 As FileIO.Read() = mate2.ToArray()
            Dim n As Integer = System.Math.Min(m1.Length, m2.Length)
            Dim results As New List(Of AlignmentHit)()

            For i As Integer = 0 To n - 1
                Dim hits1 As AlignmentHit() = AlignSingle(m1(i)).ToArray()
                Dim hits2 As AlignmentHit() = AlignSingle(m2(i)).ToArray()

                Dim paired As AlignmentHit = Nothing
                For Each a As AlignmentHit In hits1
                    For Each b As AlignmentHit In hits2
                        If a.RepliconIndex <> b.RepliconIndex Then Continue For
                        Dim distance As Integer = System.Math.Abs(a.Position - b.Position)
                        If distance > MaxPairedEndLength Then Continue For
                        If Not orientationOk(a, b) Then Continue For
                        paired = a
                        Exit For
                    Next
                    If paired IsNot Nothing Then Exit For
                Next

                If paired IsNot Nothing Then
                    paired.PairedEndIndex = i
                    paired.ReadID = $"{m1(i).ID}%{m2(i).ID}"
                    results.Add(paired)
                Else
                    ' 未形成有效配对，退回单端结果
                    For Each a In hits1
                        a.PairedEndIndex = i
                        results.Add(a)
                    Next
                    For Each b In hits2
                        b.PairedEndIndex = i
                        results.Add(b)
                    Next
                End If
            Next

            Return results
        End Function

        ''' <summary>
        ''' 依据 pairedEndOrientation（ff/fr/rf/rr）判定 mate 方向。
        ''' f = forward，r = reverse_complement；默认 fr（相对参考链，mate1 正向、mate2 反向）。
        ''' </summary>
        Private Function orientationOk(a As AlignmentHit, b As AlignmentHit) As Boolean
            Select Case PairedEndOrientation.ToLowerInvariant
                Case "ff" : Return Not a.IsReverse AndAlso Not b.IsReverse
                Case "rr" : Return a.IsReverse AndAlso b.IsReverse
                Case "rf" : Return a.IsReverse AndAlso Not b.IsReverse
                Case Else : Return Not a.IsReverse AndAlso b.IsReverse ' fr
            End Select
        End Function

#End Region

#Region "Coverage"

        ''' <summary>
        ''' 由命中构建逐碱基覆盖度（每个复制子一个 AlignmentCoverage，数组为 1-indexed）。
        ''' </summary>
        Public Function BuildCoverages(hits As IEnumerable(Of AlignmentHit), unstranded As Boolean, Optional readFileName As String = Nothing) As Core.AlignmentCoverage()
            Dim coverages As Core.AlignmentCoverage() = New Core.AlignmentCoverage(replicons.Length - 1) {}
            Dim totalLength As Long = 0
            Dim readCount As Long = 0

            For z As Integer = 0 To replicons.Length - 1
                coverages(z) = New Core.AlignmentCoverage With {
                    .PlusReads = New Integer(replicons(z).Length) {},
                    .MinusReads = New Integer(replicons(z).Length) {},
                    .ReadFileName = readFileName,
                    .Name = If(String.IsNullOrEmpty(readFileName), replicons(z).Name, IO.Path.GetFileNameWithoutExtension(readFileName))
                }
            Next

            For Each hit As AlignmentHit In hits
                If hit.RepliconIndex < 0 OrElse hit.RepliconIndex >= coverages.Length Then Continue For
                Dim coverage = coverages(hit.RepliconIndex)
                Dim len As Integer = If(hit.Sequence Is Nothing, 1, System.Math.Max(1, hit.Sequence.Length))
                Dim startPos As Integer = System.Math.Max(1, hit.Position)
                Dim endPos As Integer = System.Math.Min(replicons(hit.RepliconIndex).Length, hit.Position + len - 1)

                For p As Integer = startPos To endPos
                    If hit.IsReverse AndAlso Not unstranded Then
                        coverage.MinusReads(p) += 1
                    Else
                        coverage.PlusReads(p) += 1
                    End If
                Next

                totalLength += len
                readCount += 1
            Next

            Dim avg As Long = If(readCount = 0, 0, totalLength \ readCount)
            For z As Integer = 0 To coverages.Length - 1
                coverages(z).AvgLengthReads = avg
            Next

            Return coverages
        End Function

#End Region

        ''' <summary>反向互补（非 ACGT 字符置为 N）。</summary>
        Public Shared Function reverseComplement(sequence As String) As String
            If String.IsNullOrEmpty(sequence) Then Return sequence
            Dim chars As Char() = New Char(sequence.Length - 1) {}
            For i As Integer = 0 To sequence.Length - 1
                Dim c As Char = Char.ToUpperInvariant(sequence(sequence.Length - 1 - i))
                Select Case c
                    Case "A"c : chars(i) = "T"c
                    Case "T"c, "U"c : chars(i) = "A"c
                    Case "C"c : chars(i) = "G"c
                    Case "G"c : chars(i) = "C"c
                    Case Else : chars(i) = "N"c
                End Select
            Next
            Return New String(chars)
        End Function

    End Class

End Namespace
