' /********************************************************************************/
'
'  Rockhopper —— TSS/TTS 边界推断
'
'  复刻论文（gkt444.pdf / 13059_2014_Article_572.pdf）与 readme.md 第 4 节的策略：
'    * 原核转录本没有内含子，但存在 5'/3' UTR、反义转录本与基因间小 RNA；
'    * 对每个显著表达的基因，依据读段覆盖分布推断转录起始位点(TSS)与终止位点(TTS)，
'      从而构建完整的转录组图谱；
'    * 与另一基因在同一链上重叠的基因（两侧无基因间区）不单独预测边界，避免假阳性；
'    * 灵敏度由 `-z`（transcriptSensitivity ∈ [0,1]）控制，经 Replicate.MinExpression
'      映射为 UTR 与 ncRNA 的表达阈值（见 Core.Replicate.transformation）。
'
'  判据实现（与原始 Rockhopper 的覆盖度判据一致）：
'    对基因的 5' 侧，自 ATG 向上一碱基一碱基地扩展窗口 [pos, ATG-1]，
'    只要窗口内的平均覆盖度仍 ≥ 阈值，就把 pos 视为候选 TSS；取满足条件的最远位置。
'    3' 侧同理，自终止密码子向下游扩展窗口 [stop+1, pos]。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports SMRUCC.genomics.SequenceModel.RNA_Seq.Rockhopper.Core

Namespace Transcription

    ''' <summary>
    ''' 基于读段覆盖分布的转录边界推断。
    ''' </summary>
    Public Module TSSsInference

        ''' <summary>UTR 搜索的最大长度（论文中长 5'UTR 可超过 300 bp）。</summary>
        Public Const MAX_UTR_LENGTH As Integer = 500
        ''' <summary>基因间区新转录本的最小长度。</summary>
        Public Const MIN_INTERGENIC_LENGTH As Integer = 50

        ''' <summary>
        ''' 推断单个基因组（replicon）上的全部转录本边界。
        ''' </summary>
        ''' <param name="genome">基因组及其注释。</param>
        ''' <param name="genomeSize">该复制子的长度（= <see cref="Replicate"/> 覆盖数组的长度 - 1）。</param>
        ''' <param name="conditions">全部实验条件（其重复应已设置 MinExpression）。</param>
        ''' <param name="sensitivity">转录灵敏度 -z，范围 [0,1]。</param>
        Public Function InferBoundaries(genome As Genome, genomeSize As Integer,
                                        conditions As List(Of Condition),
                                        sensitivity As Double) As List(Of Transcript)

            ' 依灵敏度设置每个重复的 UTR / ncRNA 表达阈值
            For Each condition As Condition In conditions
                For r As Integer = 0 To condition.NumReplicates() - 1
                    condition.GetReplicate(r).MinExpression = sensitivity
                Next
            Next

            Dim results As New List(Of Transcript)()
            Dim genes As List(Of Gene) = genome.Genes

            For i As Integer = 0 To genes.Count - 1
                Dim g As Gene = genes(i)
                Dim transcript As Transcript = fromGene(g)

                If Not g.ORF Then
                    ' 已注释的 RNA：直接采用注释的转录位点
                    results.Add(transcript)
                    Continue For
                End If

                ' 同一链上两侧相邻基因的边界（用于限制 UTR 搜索范围、避免假阳性）
                Dim sameStrandNeighbors As (upstream As Integer, downstream As Integer) = neighborLimits(genes, i, genomeSize)

                Dim startT As Integer = 0
                Dim stopT As Integer = 0

                If isExpressed(g, genomeSize, conditions, Function(c, r) avgCoverageOfGene(g, genomeSize, c, r)) Then
                    startT = inferFivePrime(g, sameStrandNeighbors.upstream, genomeSize, conditions)
                    stopT = inferThreePrime(g, sameStrandNeighbors.downstream, genomeSize, conditions)
                End If

                transcript.TSSs = startT
                transcript.TTSs = stopT

                results.Add(transcript)
            Next

            ' 基因间区的新转录本（ncRNA / sRNA）
            results.AddRange(inferIntergenicTranscripts(genome, genomeSize, conditions))

            Return results
        End Function

        ''' <summary>
        ''' 由注释基因构造转录本模型。
        ''' </summary>
        Private Function fromGene(g As Gene) As Transcript
            Dim t As New Transcript With {
                .Name = g.Name,
                .Synonym = g.Synonym,
                .Product = g.Product,
                .Strand = g.Strand,
                .ATG = g.Start,
                .TGA = g.[Stop]
            }
            If Not g.ORF Then
                ' RNA：注释的 startT/stopT 即转录位点
                t.TSSs = System.Math.Min(g.StartT, g.StopT)
                t.TTSs = System.Math.Max(g.StartT, g.StopT)
            End If
            Return t
        End Function

        ''' <summary>
        ''' 计算同一链上、位于该基因两侧且距离最近的基因边界。
        ''' 用于把 UTR 搜索限制在基因间区内（两侧无基因间区时返回自身坐标，即不预测边界）。
        ''' </summary>
        Private Function neighborLimits(genes As List(Of Gene), index As Integer, genomeSize As Integer) As (upstream As Integer, downstream As Integer)
            Dim g As Gene = genes(index)
            Dim upstream As Integer = 0
            Dim downstream As Integer = genomeSize

            For j As Integer = 0 To genes.Count - 1
                If j = index Then Continue For
                Dim other As Gene = genes(j)
                If other.Strand <> g.Strand Then Continue For

                If other.Last < g.First Then
                    upstream = System.Math.Max(upstream, other.Last)
                ElseIf other.First > g.Last Then
                    downstream = System.Math.Min(downstream, other.First)
                End If
            Next

            Return (upstream, downstream)
        End Function

        ''' <summary>
        ''' 5' 端（TSS）推断：自 ATG 向上游扩展，取平均覆盖度仍达阈值的最远位置。
        ''' </summary>
        Private Function inferFivePrime(g As Gene, upstreamLimit As Integer, genomeSize As Integer,
                                        conditions As List(Of Condition)) As Integer
            Dim atg As Integer = If(g.Strand = "-"c, g.[Stop], g.Start)
            Dim threshold As Double = averageThreshold(conditions)
            If threshold <= 0.0 Then Return 0

            Dim limit As Integer = System.Math.Max(System.Math.Max(1, atg - MAX_UTR_LENGTH), upstreamLimit + 1)
            Dim best As Integer = 0
            Dim running As Double = 0.0
            Dim count As Integer = 0

            For pos As Integer = atg - 1 To limit Step -1
                running += meanCoverage(conditions, genomeSize, pos, pos, g.Strand)
                count += 1
                If count > 0 AndAlso running / count >= threshold Then
                    best = pos
                End If
            Next

            ' 没有任何 UTR 覆盖时，若 ATG 处即有表达，则视为 leaderless（TSS = ATG）
            If best = 0 AndAlso meanCoverage(conditions, genomeSize, atg, atg, g.Strand) >= threshold Then
                Return atg
            End If

            Return best
        End Function

        ''' <summary>
        ''' 3' 端（TTS）推断：自终止密码子向下游扩展，取平均覆盖度仍达阈值的最远位置。
        ''' </summary>
        Private Function inferThreePrime(g As Gene, downstreamLimit As Integer, genomeSize As Integer,
                                         conditions As List(Of Condition)) As Integer
            Dim stopCodon As Integer = If(g.Strand = "-"c, g.Start, g.[Stop])
            Dim threshold As Double = averageThreshold(conditions)
            If threshold <= 0.0 Then Return 0

            Dim limit As Integer = System.Math.Min(System.Math.Min(genomeSize, stopCodon + MAX_UTR_LENGTH), downstreamLimit - 1)
            Dim best As Integer = 0
            Dim running As Double = 0.0
            Dim count As Integer = 0

            For pos As Integer = stopCodon + 1 To limit
                running += meanCoverage(conditions, genomeSize, pos, pos, g.Strand)
                count += 1
                If count > 0 AndAlso running / count >= threshold Then
                    best = pos
                End If
            Next

            Return best
        End Function

        ''' <summary>
        ''' 基因间区的新转录本：扫描未被注释覆盖、且平均覆盖度超过 ncRNA 阈值的连续区段。
        ''' </summary>
        Private Function inferIntergenicTranscripts(genome As Genome, genomeSize As Integer,
                                                    conditions As List(Of Condition)) As List(Of Transcript)
            Dim results As New List(Of Transcript)()

            For Each strand As Char In {"+"c, "-"c}
                Dim annotation As String() = genome.GetAnnotations(strand)
                Dim threshold As Double = rnaThreshold(conditions, strand)
                If threshold <= 0.0 Then Continue For

                Dim pos As Integer = 1
                While pos < annotation.Length
                    If Not String.IsNullOrEmpty(annotation(pos)) OrElse coverageAt(conditions, genomeSize, pos, strand) < threshold Then
                        pos += 1
                        Continue While
                    End If

                    Dim start As Integer = pos
                    While pos < annotation.Length AndAlso String.IsNullOrEmpty(annotation(pos)) AndAlso
                          coverageAt(conditions, genomeSize, pos, strand) >= threshold
                        pos += 1
                    End While
                    Dim [stop] As Integer = pos - 1

                    If [stop] - start + 1 >= MIN_INTERGENIC_LENGTH Then
                        results.Add(New Transcript With {
                            .Name = "predicted RNA",
                            .Synonym = "predicted RNA",
                            .Product = "predicted RNA",
                            .Strand = strand,
                            .TSSs = start,
                            .TTSs = [stop],
                            .IsPredicted = True,
                            .Expression = meanCoverage(conditions, genomeSize, start, [stop], strand)
                        })
                    End If
                End While
            Next

            Return results
        End Function

#Region "Coverage helpers"

        ''' <summary>各重复在某个坐标上的平均覆盖度（跨全部条件/重复取平均）。</summary>
        Private Function meanCoverage(conditions As List(Of Condition), genomeSize As Integer, start As Integer, [stop] As Integer, strand As Char) As Double
            If conditions.Count = 0 Then Return 0.0
            Dim total As Double = 0.0
            Dim n As Integer = 0
            For Each condition As Condition In conditions
                For r As Integer = 0 To condition.NumReplicates() - 1
                    total += condition.GetReplicate(r).GetMeanOfRange(0, start, [stop], strand)
                    n += 1
                Next
            Next
            If n = 0 Then Return 0.0
            Return total / n
        End Function

        Private Function coverageAt(conditions As List(Of Condition), genomeSize As Integer, pos As Integer, strand As Char) As Double
            Return meanCoverage(conditions, genomeSize, pos, pos, strand)
        End Function

        ''' <summary>落在一个基因体内、正向链覆盖度的平均值。</summary>
        Private Function avgCoverageOfGene(g As Gene, genomeSize As Integer, condition As Condition, replicate As Integer) As Double
            Dim strand As Char = If(g.Strand = "-"c, "-"c, "+"c)
            Return condition.GetReplicate(replicate).GetMeanOfRange(0, g.First, g.Last, strand)
        End Function

        Private Function isExpressed(g As Gene, genomeSize As Integer, conditions As List(Of Condition),
                                     avg As Func(Of Integer, Integer, Double)) As Boolean
            For c As Integer = 0 To conditions.Count - 1
                For r As Integer = 0 To conditions(c).NumReplicates() - 1
                    If avg(c, r) > 0.0 Then Return True
                Next
            Next
            Return False
        End Function

        ''' <summary>UTR 阈值：跨全部重复的 MinExpressionUTR 平均值。</summary>
        Private Function averageThreshold(conditions As List(Of Condition)) As Double
            Dim total As Double = 0.0
            Dim n As Integer = 0
            For Each condition As Condition In conditions
                For r As Integer = 0 To condition.NumReplicates() - 1
                    total += condition.GetReplicate(r).MinExpressionUTR
                    n += 1
                Next
            Next
            Return If(n = 0, 0.0, total / n)
        End Function

        ''' <summary>ncRNA 阈值：跨全部重复的 MinExpressionRNA 平均值。</summary>
        Private Function rnaThreshold(conditions As List(Of Condition), strand As Char) As Double
            Dim total As Double = 0.0
            Dim n As Integer = 0
            For Each condition As Condition In conditions
                For r As Integer = 0 To condition.NumReplicates() - 1
                    total += condition.GetReplicate(r).MinExpressionRNA
                    n += 1
                Next
            Next
            Return If(n = 0, 0.0, total / n)
        End Function

#End Region

    End Module

End Namespace
