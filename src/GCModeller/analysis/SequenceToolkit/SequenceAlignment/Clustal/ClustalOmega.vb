Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Clustal

    ''' <summary>
    ''' Clustal Omega 风格多序列比对流水线的门面。
    ''' </summary>
    ''' <remarks>
    ''' <para>流水线分为六个阶段，每个阶段都只通过不可变的数据结构通信，因此可以单独测试与替换：</para>
    ''' <list type="number">
    ''' <item><description>序列归一化与类型判定(<see cref="SequenceAlphabet"/>)</description></item>
    ''' <item><description>k-mer/mBed 距离估计(<see cref="MBedDistance"/>)</description></item>
    ''' <item><description>k-means 分簇 + UPGMA 引导树(<see cref="ClusterTreeBuilder"/>)</description></item>
    ''' <item><description>profile HMM 构建(<see cref="ProfileHMM"/>)</description></item>
    ''' <item><description>4 状态 pair-HMM 比对与 MAC 解码(<see cref="PairHMMAligner"/>)</description></item>
    ''' <item><description>渐进合并与迭代精炼(<see cref="ProgressiveAligner"/> / <see cref="Refinement"/>)</description></item>
    ''' </list>
    ''' </remarks>
    Public Module ClustalOmega

        ''' <summary>
        ''' 对一组 FASTA 序列做多序列比对
        ''' </summary>
        Public Function Align(seqs As IEnumerable(Of FastaSeq),
                              Optional options As ClustalOmegaOptions = Nothing) As ClustalOmegaResult

            Dim source As FastaSeq() = If(seqs Is Nothing, New FastaSeq() {}, seqs.ToArray())
            Dim names As String() = New String(source.Length - 1) {}
            Dim raw As String() = New String(source.Length - 1) {}

            For i As Integer = 0 To source.Length - 1
                Dim fa As FastaSeq = source(i)
                Dim title As String = If(fa Is Nothing, Nothing, fa.Title)

                names(i) = If(String.IsNullOrEmpty(title), $"seq_{i + 1}", title)
                raw(i) = If(fa Is Nothing, "", fa.SequenceData)
            Next

            Return AlignCore(names, raw, options)
        End Function

        ''' <summary>
        ''' 对一组纯字符串序列做多序列比对
        ''' </summary>
        ''' <param name="seqs">序列集合</param>
        ''' <param name="names">可选的序列名称，为空时自动命名为 seq_1..seq_N</param>
        ''' <param name="options">流水线参数</param>
        Public Function Align(seqs As IEnumerable(Of String),
                              Optional names As IEnumerable(Of String) = Nothing,
                              Optional options As ClustalOmegaOptions = Nothing) As ClustalOmegaResult

            Dim raw As String() = If(seqs Is Nothing, New String() {}, seqs.ToArray())
            Dim labels As String() = Nothing

            If names IsNot Nothing Then
                labels = names.ToArray()
            End If

            Return AlignCore(labels, raw, options)
        End Function

        ''' <summary>
        ''' 读取 FASTA 文件并做多序列比对
        ''' </summary>
        Public Function AlignFile(path As String,
                                  Optional options As ClustalOmegaOptions = Nothing) As ClustalOmegaResult

            Return Align(FastaFile.Read(path), options)
        End Function

        ''' <summary>
        ''' 以扩展方法的风格提供比对入口
        ''' </summary>
        <Extension>
        Public Function MultipleAlignmentOmega(seqs As IEnumerable(Of FastaSeq),
                                               Optional options As ClustalOmegaOptions = Nothing) As ClustalOmegaResult

            Return Align(seqs, options)
        End Function

        ''' <summary>
        ''' 流水线的主体
        ''' </summary>
        Private Function AlignCore(names As String(), raw As String(),
                                   options As ClustalOmegaOptions) As ClustalOmegaResult

            Dim opts As ClustalOmegaOptions = (If(options, New ClustalOmegaOptions())).Validate()
            Dim n As Integer = If(raw Is Nothing, 0, raw.Length)
            Dim labels As String() = If(names, New String() {})

            If labels.Length < n Then
                Dim filled As String() = New String(n - 1) {}

                For i As Integer = 0 To n - 1
                    filled(i) = If(i < labels.Length AndAlso Not String.IsNullOrEmpty(labels(i)), labels(i), $"seq_{i + 1}")
                Next

                labels = filled
            End If

            Dim timings As New Dictionary(Of String, Double)()
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim result As New ClustalOmegaResult With {.Options = opts, .StageTimings = timings}

            If n = 0 Then
                result.names = New String() {}
                result.MSA = New String() {}
                result.edits = New Integer() {}
                result.cost = 0
                result.Statistics = AlignmentStatistics.Compute(New String() {}, ScoringMatrix.Gonnet())
                result.GuideTree = New GuideTree(Nothing, 0)
                result.SequenceTypeUsed = SequenceType.Protein
                result.KmerSizeUsed = 0
                result.Sequences = New String() {}

                Return result
            End If

            ' 1. 归一化与类型判定
            watch.Restart()

            Dim alphabet As SequenceAlphabet

            Select Case opts.SeqType
                Case SequenceType.Protein
                    alphabet = SequenceAlphabet.Protein
                Case SequenceType.DNA
                    alphabet = SequenceAlphabet.DNA
                Case Else
                    alphabet = SequenceAlphabet.Detect(raw)
            End Select

            Dim kmer As Integer = opts.EffectiveKmer(alphabet)
            Dim seqs As String() = New String(n - 1) {}

            For i As Integer = 0 To n - 1
                seqs(i) = alphabet.Normalize(raw(i))
            Next

            timings("normalize") = watch.Elapsed.TotalMilliseconds
            watch.Restart()

            Dim matrix As ScoringMatrix = If(alphabet.Kind = SequenceType.Protein,
                                             ScoringMatrix.Gonnet(),
                                             ScoringMatrix.DNAMatrix())

            result.SequenceTypeUsed = alphabet.Kind
            result.KmerSizeUsed = kmer
            result.Sequences = seqs

            Dim rows As String()
            Dim tree As GuideTree

            If n = 1 Then
                rows = New String() {seqs(0)}
                tree = New GuideTree(New GuideTreeNode With {
                    .Index = 0,
                    .Label = labels(0),
                    .Height = 0,
                    .LeafCount = 1
                }, 1)

                timings("distance") = 0
                timings("clustering") = 0
                timings("progressive") = 0
            ElseIf AllIdentical(seqs) Then
                ' 全部序列都相同：不需要任何比对，直接返回(树用一棵平衡二叉树表示)
                rows = seqs.ToArray()
                tree = BalancedTree(labels)

                timings("distance") = 0
                timings("clustering") = 0
                timings("progressive") = 0
            Else
                ' 2. mBed 距离
                Dim mbed As MBedDistance = MBedDistance.Compute(seqs, alphabet, kmer, opts)

                timings("distance") = watch.Elapsed.TotalMilliseconds
                watch.Restart()

                ' 3. 分簇 + 引导树
                Dim clusterTree As ClusterTree = ClusterTreeBuilder.Build(seqs, mbed, alphabet, kmer, opts)

                timings("clustering") = watch.Elapsed.TotalMilliseconds
                watch.Restart()

                ' 4-5. 渐进式 profile-profile 合并
                Dim progressive As New ProgressiveAligner(seqs, matrix, opts)

                rows = progressive.Run(clusterTree.Tree)
                tree = clusterTree.Tree

                timings("progressive") = watch.Elapsed.TotalMilliseconds
                watch.Restart()

                result.MergeCount = progressive.MergeCount
                result.ProfileCells = progressive.Cells
                result.MacFallbacks = progressive.Fallbacks
                result.ClusterSizes = clusterTree.ClusterSizes
                result.DistanceMatrix = mbed.Matrix

                ' 6. 迭代精炼
                If opts.Iterations > 0 Then
                    Dim refinement As New Refinement(matrix, opts)

                    rows = refinement.Run(rows, tree, opts.Iterations)

                    timings("refinement") = watch.Elapsed.TotalMilliseconds
                    watch.Restart()

                    Call VBDebugger.EchoLine($"refinement: accepted={refinement.Accepted}, SP {Math.Round(refinement.ScoreBefore, 3)} -> {Math.Round(refinement.ScoreAfter, 3)}")
                End If
            End If

            ' 7. 统计
            Dim statistics As AlignmentStatistics = AlignmentStatistics.Compute(rows, matrix, gapPenalty:=opts.GapExtendBits)

            timings("statistics") = watch.Elapsed.TotalMilliseconds

            result.names = labels
            result.MSA = rows
            result.edits = statistics.Edits
            result.cost = statistics.SpScore
            result.Statistics = statistics
            result.GuideTree = tree
            result.Options = opts

            Call VBDebugger.EchoLine($"clustal-omega: N={n}, type={alphabet.Kind}, k={kmer}, columns={statistics.ColumnCount}, SP={Math.Round(statistics.SpScore, 3)}")

            Return result
        End Function

        Private Function AllIdentical(seqs As String()) As Boolean
            For i As Integer = 1 To seqs.Length - 1
                If seqs(i) <> seqs(0) Then
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' 为一组相同的序列构造一棵平衡二叉树(避免对全等序列做 O(N^2) 的 UPGMA)
        ''' </summary>
        Private Function BalancedTree(labels As String()) As GuideTree
            Dim n As Integer = labels.Length

            If n = 0 Then
                Return New GuideTree(Nothing, 0)
            End If

            Dim root As GuideTreeNode = BuildBalanced(labels, 0, n - 1)

            Return New GuideTree(root, n)
        End Function

        Private Function BuildBalanced(labels As String(), left As Integer, right As Integer) As GuideTreeNode
            If left = right Then
                Return New GuideTreeNode With {
                    .Index = left,
                    .Label = labels(left),
                    .Height = 0,
                    .LeafCount = 1
                }
            End If

            Dim mid As Integer = (left + right) \ 2
            Dim a As GuideTreeNode = BuildBalanced(labels, left, mid)
            Dim b As GuideTreeNode = BuildBalanced(labels, mid + 1, right)

            Return New GuideTreeNode With {
                .Index = -1,
                .Left = a,
                .Right = b,
                .Height = 0,
                .LeafCount = a.LeafCount + b.LeafCount
            }
        End Function
    End Module
End Namespace
