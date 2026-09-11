' /********************************************************************************/
'
'  Rockhopper —— de novo 转录组组装主流程
'
'  复刻论文（13059_2014_Article_572.pdf / Rockhopper 2）与 readme.md 第 5 节：
'    1) 把所有读段打断为 k-mer，构建 de Bruijn 图（节点为 k-1 mer，边为 k-mer）；
'    2) 仅当节点对应 k-mer 计数达到 `Min count to seed a transcript`（CLI -w，默认 50）
'       时才作为候选起点；延伸时要求后续节点达到 `Min count to extend a transcript`
'       （CLI -x，默认 5），从而过滤低频测序噪声；
'    3) 沿图路径遍历输出候选转录本序列（长度需 ≥ `-u`，默认 2*k；
'       读段先按 `-j`，默认 35 过滤）；
'    4) 候选转录本建立 BWT 索引，把原始读段比对回候选，淘汰覆盖率不足者
'       （`-b`，默认 20）；
'    5) 输出 transcripts.txt（序列 / 长度 / 表达量 / q 值）。
'
'  与原始实现的差异：原实现使用自建哈希表；这里使用 .NET 字典并保留相同的阈值语义。
'  投影表容量由 `-n`（2^n，默认 2^25）约束，避免内存失控。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports SMRUCC.genomics.SequenceModel.RNA_Seq.Rockhopper.Core

Namespace Assembly

    ''' <summary>
    ''' de novo 转录组组装器。
    ''' </summary>
    Public Class DeNovoAssembler

#Region "Parameters"

        Public Property ConditionFiles As New List(Of String)()
        Public Property OutputDir As String = "Rockhopper_Results/"
        Public Property ExpressionFile As String = "transcripts.txt"

        ''' <summary>k-mer 长度（CLI -k，默认 25，范围 15–31）。</summary>
        Public Property K As Integer = 25
        ''' <summary>使用读段前所需的最小长度（CLI -j，默认 35）。</summary>
        Public Property MinReadLength As Integer = 35
        ''' <summary>k-mer 哈希表容量幂次 2^n（CLI -n，默认 25）。</summary>
        Public Property CapacityPower As Integer = 25
        ''' <summary>保留候选所需的最少映射读段数（CLI -b，默认 20）。</summary>
        Public Property MinReadsMapping As Integer = 20
        ''' <summary>转录本最小长度（CLI -u，默认 2*K）。</summary>
        Public Property MinTranscriptLength As Integer = 0
        ''' <summary>作为起点的 k-mer 最小计数（CLI -w，默认 50）。</summary>
        Public Property MinSeedExpression As Integer = 50
        ''' <summary>延伸所需的 k-mer 最小计数（CLI -x，默认 5）。</summary>
        Public Property MinExpression As Integer = 5

        Public Property NumThreads As Integer = 1
        Public Property StopAfterOneHit As Boolean = True
        Public Property OutputSAM As Boolean = False
        Public Property Unstranded As Boolean = False
        Public Property Verbose As Boolean = False
        Public Property Time As Boolean = False
        Public Property Labels As String()
        Public Property MaxPairedEndLength As Integer = 500
        Public Property SingleEndOrientationReverseComplement As Boolean = False
        Public Property PairedEndOrientation As String = "fr"

        Public Property NumConditions As Integer
        Public Property GenomeSize As Integer
        Public Property GenomeSizes As List(Of Integer)

        ''' <summary>走链追踪剩余输出条数（仅 Verbose 时启用，用于诊断）。</summary>
        Private _trace As Integer = 0

#End Region

        Public Sub New(Optional conditionFiles As List(Of String) = Nothing,
                       Optional outputDir As String = "Rockhopper_Results/",
                       Optional expressionFile As String = "transcripts.txt")
            If conditionFiles IsNot Nothing Then Me.ConditionFiles = conditionFiles
            Me.OutputDir = outputDir
            Me.ExpressionFile = expressionFile
        End Sub

        ''' <summary>
        ''' 执行完整的 de novo 组装并输出结果文件。
        ''' </summary>
        Public Sub Run()
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim minLength As Integer = If(MinTranscriptLength > 0, MinTranscriptLength, 2 * K)

            ' 1) 读取读段
            Dim reads As List(Of FileIO.Read) = readAllReads()
            Call Logging.Output($"[DE NOVO] {reads.Count} reads passed the length filter ({MinReadLength} nt).{vbLf}")
            If reads.Count = 0 Then
                Call save(New DeNovoTranscripts())
                Return
            End If

            ' 2) k-mer 计数
            Dim kmers As Dictionary(Of String, Integer) = countKmers(reads)
            Call Logging.Output($"[DE NOVO] {kmers.Count} distinct {K}-mers.{vbLf}")

            ' 3) de Bruijn 图遍历
            Dim candidates As List(Of String) = assembleContigs(kmers)
            Call Logging.Output($"[DE NOVO] {candidates.Count} candidate transcripts assembled.{vbLf}")

            If Verbose Then
                For Each candidate As String In candidates.Take(10)
                    Dim preview As String = candidate.Substring(0, System.Math.Min(60, candidate.Length))
                    Call Logging.Output($"[DE NOVO]   candidate: length={candidate.Length}, head={preview}{vbLf}")
                Next
            End If

            ' 候选长度过滤 + 去冗余
            Dim filtered As New DeNovoTranscripts(
                candidates.Where(Function(s) s.Length >= minLength).Select(Function(s) New DeNovoTranscript(s)))
            filtered = filtered.Distinct()

            ' 4) 二次比对精修
            Dim index As New DeNovoIndex(filtered)
            Dim final As DeNovoTranscripts = index.MapReads(reads, MinReadsMapping, NumThreads).ByExpression()
            Call Logging.Output($"[DE NOVO] {final.Count} transcripts passed the read-support filter ({MinReadsMapping} reads).{vbLf}")

            ' 5) 输出
            Call save(final)

            watch.Stop()
            If Time Then
                Call Logging.Output($"Execution time:{vbTab}{watch.Elapsed.TotalMinutes:F0} minutes {watch.Elapsed.Seconds} seconds{vbLf}")
            End If
        End Sub

#Region "Pipeline steps"

        Private Function readAllReads() As List(Of FileIO.Read)
            Dim reads As New List(Of FileIO.Read)()
            Dim seen As New HashSet(Of String)()

            For Each condition As String In ConditionFiles
                For Each file As String In condition.Split(","c)
                    For Each mate As String In file.Split("%"c)
                        If String.IsNullOrWhiteSpace(mate) OrElse seen.Contains(mate) Then Continue For
                        seen.Add(mate)
                        If Not IO.File.Exists(mate) Then Continue For

                        For Each read As FileIO.Read In FileIO.ReadsReader.ReadAll(mate)
                            If read.Length >= MinReadLength AndAlso Not read.Sequence.Contains("N"c) Then
                                reads.Add(read)
                            End If
                        Next
                    Next
                Next
            Next

            Return reads
        End Function

        ''' <summary>
        ''' 统计全部读段的 k-mer 计数（容量受 -n 约束）。
        ''' </summary>
        Private Function countKmers(reads As IEnumerable(Of FileIO.Read)) As Dictionary(Of String, Integer)
            Dim capacity As Long = CLng(2) ^ CapacityPower
            Dim kmers As New Dictionary(Of String, Integer)()

            For Each read As FileIO.Read In reads
                Dim seq As String = read.Sequence
                For i As Integer = 0 To seq.Length - K
                    Dim kmer As String = seq.Substring(i, K)
                    Dim count As Integer = 0
                    If kmers.TryGetValue(kmer, count) Then
                        kmers(kmer) = count + 1
                    Else
                        If kmers.Count >= capacity Then Continue For
                        kmers(kmer) = 1
                    End If
                Next
            Next

            Return kmers
        End Function

        ''' <summary>
        ''' de Bruijn 图贪心遍历：以高计数 k-mer 为种子，向左/向右延伸，
        ''' 每一步选择计数最高且不低于 <see cref="MinExpression"/> 的相邻 k-mer。
        ''' </summary>
        Private Function assembleContigs(kmers As Dictionary(Of String, Integer)) As List(Of String)
            Dim byPrefix As New Dictionary(Of String, List(Of String))()
            Dim bySuffix As New Dictionary(Of String, List(Of String))()
            Dim overlap As Integer = K - 1

            For Each kmer As String In kmers.Keys
                Dim prefix As String = kmer.Substring(0, overlap)
                Dim suffix As String = kmer.Substring(1, overlap)

                Dim list As List(Of String) = Nothing
                If Not byPrefix.TryGetValue(prefix, list) Then
                    list = New List(Of String)()
                    byPrefix(prefix) = list
                End If
                list.Add(kmer)

                Dim slist As List(Of String) = Nothing
                If Not bySuffix.TryGetValue(suffix, slist) Then
                    slist = New List(Of String)()
                    bySuffix(suffix) = slist
                End If
                slist.Add(kmer)
            Next

            Dim used As New HashSet(Of String)()
            Dim contigs As New List(Of String)()
            If Verbose Then _trace = 90

            ' 以计数从高到低处理种子，保证先构建表达量最高的转录本
            Dim seeds As IEnumerable(Of String) = kmers.Where(Function(kv) kv.Value >= MinSeedExpression) _
                                                       .OrderByDescending(Function(kv) kv.Value) _
                                                       .Select(Function(kv) kv.Key)

            For Each seed As String In seeds
                If used.Contains(seed) Then Continue For

                Dim contig As String = extend(seed, byPrefix, bySuffix, kmers, used, forward:=True)
                contig = extend(contig, byPrefix, bySuffix, kmers, used, forward:=False)
                If contig.Length >= K Then contigs.Add(contig)
            Next

            ' 释放不再需要的索引，便于大基因组场景回收内存
            byPrefix.Clear()
            bySuffix.Clear()

            Return contigs
        End Function

        ''' <summary>
        ''' 从一段序列出发，向前（3'）或向后（5'）延伸。
        ''' </summary>
        Private Function extend(current As String, byPrefix As Dictionary(Of String, List(Of String)),
                                bySuffix As Dictionary(Of String, List(Of String)),
                                kmers As Dictionary(Of String, Integer),
                                used As HashSet(Of String), forward As Boolean) As String
            Dim overlap As Integer = K - 1

            While True
                Dim tail As String = If(forward, current.Substring(current.Length - overlap), current.Substring(0, overlap))
                Dim candidates As List(Of String) = Nothing
                If forward Then
                    If Not byPrefix.TryGetValue(tail, candidates) Then Exit While
                Else
                    If Not bySuffix.TryGetValue(tail, candidates) Then Exit While
                End If

                Dim best As String = Nothing
                Dim bestCount As Integer = -1
                For Each kmer As String In candidates
                    If used.Contains(kmer) Then Continue For
                    Dim count As Integer = If(kmers.ContainsKey(kmer), kmers(kmer), 0)
                    If count < MinExpression Then Continue For
                    If count > bestCount Then
                        bestCount = count
                        best = kmer
                    End If
                Next

                If best Is Nothing Then Exit While

                If _trace > 0 Then
                    _trace -= 1
                    Call Logging.Output($"[TRACE] {(If(forward, "F", "B"))} tail={tail} best={best} count={bestCount}{vbLf}")
                End If

                used.Add(best)
                current = If(forward, current & best(best.Length - 1), best(0) & current)
            End While

            Return current
        End Function

        Private Sub save(transcripts As DeNovoTranscripts)
            If Not IO.Directory.Exists(OutputDir) Then
                Call IO.Directory.CreateDirectory(OutputDir)
            End If
            Dim path As String = IO.Path.Combine(OutputDir, ExpressionFile)
            Call transcripts.Save(path)
            Call Logging.Output($"[DE NOVO] Results written to {path}{vbLf}")
        End Sub

#End Region

    End Class

End Namespace
