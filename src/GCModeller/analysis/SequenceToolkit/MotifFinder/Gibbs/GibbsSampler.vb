#Region "Microsoft.VisualBasic::30e932d92877880646538e69fe580e08, analysis\SequenceToolkit\MotifFinder\Gibbs\GibbsSampler.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 385
    '    Code Lines: 233 (60.52%)
    ' Comment Lines: 96 (24.94%)
    '    - Xml Docs: 80.21%
    ' 
    '   Blank Lines: 56 (14.55%)
    '     File Size: 15.47 KB


    ' Class GibbsSampler
    ' 
    '     Properties: SequenceCount, Sequences
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: BuildMotifResult, CalculateGlobalBackground, calculateMotifProbability, DefaultRestarts,
    '               EstimateEvalue, FillNNNN, find, findTopN, getMotifStrings, getRandomSites, gibbsSample,
    '               informationContent, MaskSites, predictiveUpdateStep, RunRestarts, samplingStep,
    '               SiteOdds, weightedChooseIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.Matrix
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' Gibbs Sampler 是一种基于马尔可夫链蒙特卡洛（MCMC）思想的随机抽样算法，被广泛用于从一组序列中发现未知的保守 motif。其基本流程为：
''' 假设每条输入序列中均包含一个长度为 W 的 motif 实例，算法首先在每条序列中随机选取一个长度为 W 的窗口作为 motif 位点的初始状态；
''' 在随后每一轮迭代中，随机抽出一条序列并暂时移除，用其余序列中已定位的 motif 片段构建位置特异性评分矩阵（PWM），并结合背景模型计算
''' 被移除序列中每个候选窗口作为 motif 位点的后验概率，再依据该概率分布随机抽样新的位点并放回；如此反复迭代更新各序列的 motif 位置，
''' 直至全体位点趋于收敛。由于每一步均以概率采样（而非贪心选取最优）的方式更新，Gibbs Sampler 能有效规避 EM 类算法（如 MEME）易陷入
''' 局部最优的问题；实践中通常设置伪计数和平滑项、从多个随机初始状态独立运行并保留总得分最高的比对结果，以提高发现真实 motif 的准确性与稳健性。
''' </summary>
Public Class GibbsSampler

    Shared ReadOnly LOG_2 As Double = Math.Log(2)

    ''' <summary>
    ''' 背景概率的下限值：输入序列中完全没有出现过的碱基其背景频率为 0，
    ''' 会在计算似然比 q/p 时产生除零(+Inf)，需要钳制到一个极小的概率。
    ''' </summary>
    Const MIN_BACKGROUND As Double = 0.000001

    ''' <summary>
    ''' 信息含量的理论上限：log2(4) = 2 bits/column（DNA 四个碱基）
    ''' </summary>
    Friend Const MAX_ICPC As Double = 2.0R

    ''' <summary>
    ''' 自适应信息含量阈值所使用的倍数：随机对齐的期望信息含量约为
    ''' <see cref="Probability.E(Integer)"/>（即有限样本带来的偏置），
    ''' 默认为其 5 倍，也就是要求 motif 的信息含量至少是随机噪声水平的 5 倍。
    ''' </summary>
    Friend Const ICPC_NOISE_FACTOR As Double = 5.0R

    ''' <summary>
    ''' 落在屏蔽区（窗口内含有 N）之上的候选起点所使用的 log 权重。
    ''' 
    ''' 这里取一个足够小的常数而不是负无穷：若某条序列的候选起点在若干轮屏蔽之后
    ''' 全部落入屏蔽区，采样分布依然是可归一化的（此时退化为均匀分布）。
    ''' </summary>
    Const MASKED_LOG_WEIGHT As Double = -1000.0R

    ''' <summary>
    ''' populate all fasta <see cref="FastaSeq.SequenceData"/> in upper case.
    ''' </summary>
    ''' <returns></returns>
    Public Overridable ReadOnly Property Sequences As IEnumerable(Of String)
        Get
            Return From seq As FastaSeq
                   In m_sequences
                   Select seq.SequenceData.ToUpper
        End Get
    End Property

    Friend ReadOnly m_motifLength As Integer
    ''' <summary>
    ''' sequence length of the input sequence collection
    ''' </summary>
    Friend ReadOnly m_sequenceLength As Integer()
    Friend ReadOnly m_sequenceCount As Integer
    Friend ReadOnly m_sequences As FastaSeq()
    Friend ReadOnly m_ignored As Integer
    Friend ReadOnly m_globalBackground As Double()

    ''' <returns> the size of the list sequences </returns>
    Public Overridable ReadOnly Property SequenceCount As Integer
        Get
            Return m_sequenceCount
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="fastaFile">the un-aligned raw sequence data, any sequence object with length less 
    ''' than the required <paramref name="motifLength"/> will be ignored.</param>
    ''' <param name="motifLength">
    ''' recommended use value of 0.8 multiply of the average length of the fasta sequence
    ''' </param>
    Sub New(fastaFile As IEnumerable(Of FastaSeq), Optional motifLength As Integer = 8)
        m_sequences = fastaFile.ToArray
        m_sequenceCount = m_sequences.Length
        m_motifLength = motifLength
        m_sequences = FillNNNN(m_sequences, motifLength).ToArray
        m_ignored = m_sequenceCount - m_sequences.Length
        m_sequenceLength = m_sequences _
            .Select(Function(a) a.Length) _
            .ToArray
        m_sequenceCount = m_sequences.Length
        ' 预计算全局背景概率
        m_globalBackground = CalculateGlobalBackground(m_sequences)
    End Sub

    Private Iterator Function FillNNNN(sites As IEnumerable(Of FastaSeq), motifLength As Integer) As IEnumerable(Of FastaSeq)
        For Each seq As FastaSeq In sites
            If seq.Length < motifLength Then
                Yield New FastaSeq(seq.SequenceData & New String("N"c, motifLength - seq.Length + 1), title:=seq.Title)
            Else
                Yield seq
            End If
        Next
    End Function

    Private Function CalculateGlobalBackground(sequences As FastaSeq()) As Double()
        Dim counts = New Double() {0, 0, 0, 0}
        Dim total As Integer = 0

        For Each seq In sequences
            For Each c In seq.SequenceData.ToUpper()
                Dim idx = Utils.indexOfBase(c)
                If idx > -1 Then
                    counts(idx) += 1
                    total += 1
                End If
            Next
        Next

        If total <= 0 Then
            ' 默认均匀分布
            Return {0.25, 0.25, 0.25, 0.25}
        End If

        ' 输入中不曾出现的碱基其背景概率为 0，会在计算似然比 q / P 时产生除零，
        ' 使得包含该碱基的候选起点权重恒为 +Inf，这里统一钳制到极小的下限概率
        Return counts _
            .Select(Function(c)
                        Dim p As Double = c / total

                        Return If(p > 0, p, MIN_BACKGROUND)
                    End Function) _
            .ToArray()
    End Function

    ''' <summary>
    ''' Runs numSamples gibbsSamples to find a prediction on the sites
    ''' and motifs with the highest information content in the sequences </summary>
    ''' <param name="maxIterations">maximum number of times to iterate in a Gibbs Sample </param>
    ''' <returns>
    ''' 信息含量最高的那一个 motif；当所有重启都没有产出有效结果时返回 Nothing。
    ''' 若需要一次发现多个 motif，请改用 <see cref="findTopN"/>。
    ''' </returns>
    Public Function find(Optional maxIterations As Integer = 1000) As MSAMotif
        Dim numSamples As Integer = SequenceCount
        Dim sampler As New RunSample(Me)
        Dim println As Action(Of String) = AddressOf VBDebugger.EchoLine

        Call println("============= Input Sequences =============")
        Call println(" * number of sequence samples: " & numSamples)
        Call println(" * range of sequence length: " & New DoubleRange(m_sequenceLength).MinMax.GetJson)
        Call println(" * motif width for search: " & m_motifLength)
        Call println(" * ignores of short sequence with length less than required motif width: " & m_ignored)
        Call println("")
        Call println("============= Result of Gibbs Sampling Algorithm in each iteration =============")

        ' 保持既有的重启次数：每一个输入序列对应一次独立重启
        Call RunRestarts(sampler, restarts:=numSamples, maxIterations:=maxIterations)

        Dim result As MSAMotif = BuildMotifResult(sampler, rank:=1)

        If result Is Nothing Then
            Call println("!!!! no valid motif was found in all of the restarts !!!!")
        Else
            Call println("======== Maximum Information Content :: " & result.cost & " =========" & vbLf)
        End If

        Return result
    End Function

    ''' <summary>
    ''' 迭代式（find -&gt; mask -&gt; resample）的多 motif 发现：
    ''' 
    ''' 1. 在当前的序列集合上运行一轮标准的吉布斯采样，得到 motif Mk 及其在每条序列上的位点；
    ''' 2. 若 Mk 的信息含量或者 E-value 达不到阈值要求，则终止整个发现过程；
    ''' 3. 把 Mk 的所有位点窗口（两侧各外扩 <paramref name="maskPadding"/> 倍 motif 宽度）
    '''    屏蔽为 N，然后重新随机初始化位点，在屏蔽之后的序列上继续发现下一个 motif。
    ''' 
    ''' 由于每一轮都会把上一轮的发现屏蔽掉，同一个 motif 不会被重复发现，
    ''' 各轮结果之间的位点窗口也互不重叠。发现的数量允许少于 <paramref name="topN"/>：
    ''' 一旦新的 motif 达不到阈值要求就立即终止，绝不会返回低质量的结果。
    ''' 
    ''' 返回值按照「发现顺序」排列。需要注意每一轮面对的是不同的搜索空间
    ''' （越往后屏蔽区越大），因此各轮的信息含量并不保证严格单调递减：
    ''' 某一轮若陷入了较差的局部最优，其后一轮完全可能找到信息含量更高的 motif。
    ''' </summary>
    ''' <param name="topN">期望发现的 motif 数量上限</param>
    ''' <param name="maxIterations">maximum number of times to iterate in a Gibbs Sample </param>
    ''' <param name="restarts">
    ''' 每一轮发现所使用的随机重启次数；小于等于 0 时按照序列规模自动推算。
    ''' 注意：一旦某个重启达到了信息含量的理论上限，其余重启会提前空转退出。
    ''' </param>
    ''' <param name="maskPadding">
    ''' 位点窗口两侧的屏蔽外扩量，单位为 motif 宽度的倍数（默认 ±w/2）
    ''' </param>
    ''' <param name="icpcCutoff">
    ''' 单位列信息含量(bits/column)的下限，其理论上限为 log2(4) = 2.0。
    ''' 
    ''' 信息含量的绝对值会随序列条数的增加而系统性下降（随机对齐的期望信息含量约为
    ''' <see cref="Probability.E(Integer)"/>：120 条序列时约 0.018，400 条时约 0.0054），
    ''' 所以用固定的绝对阈值并不合适。默认 -1 表示按噪声水平自适应推算，
    ''' 即 <see cref="ICPC_NOISE_FACTOR"/> × <see cref="Probability.E(Integer)"/>；
    ''' 置为 0 表示关闭该闸门。
    ''' </param>
    ''' <param name="evalueCutoff">
    ''' E-value 的上限，默认是 <see cref="Double.PositiveInfinity"/>，即默认关闭该闸门。
    ''' 这里的 E-value 是对「在全部候选起点上至少出现一次同等或更强匹配」的期望次数
    ''' 所做的 Chernoff 上界，作为上界它偏保守，因此默认不作为终止条件；
    ''' 若需要严格的显著性判据，可将其设为 1.0。
    ''' </param>
    ''' <returns>
    ''' 按发现顺序排列的 motif 数组，其长度允许小于 <paramref name="topN"/>。
    ''' </returns>
    Public Function findTopN(Optional topN As Integer = 5,
                             Optional maxIterations As Integer = 1000,
                             Optional restarts As Integer = 0,
                             Optional maskPadding As Double = 0.5,
                             Optional icpcCutoff As Double = -1,
                             Optional evalueCutoff As Double = Double.PositiveInfinity) As MSAMotif()
        If topN <= 0 Then
            Return {}
        End If

        If restarts <= 0 Then
            restarts = DefaultRestarts()
        End If
        If icpcCutoff < 0 Then
            ' 按有限样本噪声水平自适应：随机对齐的期望信息含量约为 Probability.E(n)
            icpcCutoff = ICPC_NOISE_FACTOR * Probability.E(Math.Max(m_sequenceCount, 1))
        End If

        Dim println As Action(Of String) = AddressOf VBDebugger.EchoLine
        Dim pad As Integer = CInt(Math.Floor(m_motifLength * maskPadding))
        Dim result As New List(Of MSAMotif)()
        ' 迭代式屏蔽的可变工作副本：每一轮发现结束之后就地写入 N。
        ' 屏蔽只替换字符、绝不改变序列长度，索引体系才能保持不变。
        Dim work As String() = Sequences.ToArray

        Call println("============= Input Sequences =============")
        Call println(" * number of sequence samples: " & m_sequenceCount)
        Call println(" * range of sequence length: " & New DoubleRange(m_sequenceLength).MinMax.GetJson)
        Call println(" * motif width for search: " & m_motifLength)
        Call println(" * ignores of short sequence with length less than required motif width: " & m_ignored)
        Call println(" * restarts of each round: " & restarts)
        Call println(" * mask padding: +/-" & pad & " bp")
        Call println($" * icpc cutoff : {icpcCutoff.ToString("G4")} bits/column")
        Call println($" * e-value cutoff: {If(Double.IsPositiveInfinity(evalueCutoff), "<disabled>", evalueCutoff.ToString("G4"))}")
        Call println("")

        For round As Integer = 1 To topN
            Call println($"============= motif #{round} =============")

            Dim sampler As New RunSample(Me, work)

            Call RunRestarts(sampler, restarts, maxIterations)

            Dim motif As MSAMotif = BuildMotifResult(sampler, rank:=round)

            If motif Is Nothing Then
                Call println($" -> stop: no valid motif was found at round {round}")
                Exit For
            End If

            If motif.cost < icpcCutoff Then
                Call println($" -> stop: information content {motif.cost.ToString("F4")} bits/column is less than cutoff {icpcCutoff}")
                Exit For
            End If

            If motif.evalue > evalueCutoff Then
                Call println($" -> stop: e-value {motif.evalue.ToString("G4")} is greater than cutoff {evalueCutoff}")
                Exit For
            End If

            Call println($" -> found: {motif.cost.ToString("F4")} bits/column, e-value {motif.evalue.ToString("G4")}")

            result.Add(motif)

            If round < topN Then
                Call MaskSites(work, sampler.predictedSites, pad)
                Call println($" -> masked {sampler.predictedSites.Count} sites with +/-{pad} bp padding" & vbLf)
            End If
        Next

        Call println($"============= found {result.Count} motifs in total =============")

        Return result.ToArray()
    End Function

    ''' <summary>
    ''' 在未显式指定重启次数时，依据序列规模推算一个合理的重启次数
    ''' </summary>
    Private Function DefaultRestarts() As Integer
        Return Math.Max(Environment.ProcessorCount, Math.Min(4 * m_sequenceCount, 200))
    End Function

    ''' <summary>
    ''' 以 restarts 个互不相同的随机初始状态并行运行吉布斯采样，
    ''' 各重启之间共享的最优结果汇总在 <paramref name="sampler"/> 之中。
    ''' </summary>
    Private Sub RunRestarts(sampler As RunSample, restarts As Integer, maxIterations As Integer)
        Dim parallelOptions As New ParallelOptions With {
            .MaxDegreeOfParallelism = Environment.ProcessorCount
        }

        Call System.Threading.Tasks.Parallel.For(
            fromInclusive:=0,
            toExclusive:=restarts,
            parallelOptions,
            body:=Sub(j)
                      Call sampler.RunOne(maxIterations)
                  End Sub)
    End Sub

    ''' <summary>
    ''' 把本轮发现的位点窗口（两侧各外扩 <paramref name="pad"/> 个碱基）就地屏蔽为 N。
    ''' 
    ''' 屏蔽是「等长替换」：只改写字符而不改变序列长度，因为
    ''' <see cref="m_sequenceLength"/>、随机初始化以及候选起点的计算全部依赖于长度不变。
    ''' 被屏蔽的位置 <see cref="Utils.indexOfBase"/> 会返回 -1，
    ''' 计数矩阵与似然比计算都会自动跳过它们。
    ''' </summary>
    Private Sub MaskSites(work As String(), sites As List(Of Integer), pad As Integer)
        Dim n As Integer = Math.Min(work.Length, sites.Count)

        For i As Integer = 0 To n - 1
            Dim sequence As Char() = work(i).ToCharArray
            Dim start As Integer = Math.Max(0, sites(i) - pad)
            Dim [end] As Integer = Math.Min(sequence.Length - 1, sites(i) + m_motifLength - 1 + pad)

            For j As Integer = start To [end]
                sequence(j) = "N"c
            Next

            work(i) = New String(sequence)
        Next
    End Sub

    ''' <summary>
    ''' 依据一轮多重启采样的结果构造 <see cref="MSAMotif"/>；
    ''' 当所有重启都没有产出有效结果时（例如信息含量恒为 NaN）返回 Nothing。
    ''' </summary>
    Private Function BuildMotifResult(sampler As RunSample, rank As Integer) As MSAMotif
        If sampler.predictedMotifs.Count = 0 Then
            Return Nothing
        End If

        Dim motifMatrix As SequenceMatrix = New SequenceMatrix(sampler.predictedMotifs)
        Dim n As Integer = sampler.predictedMotifs.Count
        Dim p As Double() = New Double(n - 1) {}
        Dim q As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            Dim odds As (q As Double, p As Double) = SiteOdds(motifMatrix, sampler.predictedMotifs(i))

            q(i) = odds.q
            p(i) = odds.p
        Next

        Return New MSAMotif With {
            .rank = rank,
            .evalue = EstimateEvalue(motifMatrix),
            .cost = sampler.ICPC,
            .MSA = sampler.predictedMotifs.ToArray,
            .names = m_sequences.Select(Function(fa) fa.Title).ToArray,
            .start = New ints(sampler.predictedSites),
            .countMatrix = motifMatrix.countsMatrix _
                .Select(Function(row) New ints(row)) _
                .ToArray,
            .rowSum = motifMatrix.rowSum,
            .p = p,
            .q = q,
            .alphabets = Utils.ACGT
        }
    End Function

    ''' <summary>
    ''' 计算某一条 motif 实例在 PWM 模型之下的概率 q 与在背景模型之下的概率 p。
    ''' 
    ''' 两者都以「逐列几何平均」的形式给出，以避免较长 motif 的连乘下溢，
    ''' 因此 <see cref="MSAMotif.score"/>（= q / p）是逐列的优势比(odds ratio)：
    ''' 大于 1 表示该位点相比于随机背景更像是 motif 实例。
    ''' </summary>
    Private Function SiteOdds(pwm As SequenceMatrix, motif As String) As (q As Double, p As Double)
        Dim logQ As Double = 0
        Dim logP As Double = 0
        Dim observed As Integer = 0

        For i As Integer = 0 To m_motifLength - 1
            Dim baseIdx As Integer = Utils.indexOfBase(motif(i))

            ' 被屏蔽为 N 的位置不参与似然比的计算
            If baseIdx < 0 Then
                Continue For
            End If

            logQ += Math.Log(pwm.probability(i, baseIdx))
            logP += Math.Log(If(m_globalBackground(baseIdx) > 0, m_globalBackground(baseIdx), MIN_BACKGROUND))
            observed += 1
        Next

        If observed = 0 Then
            Return (0.0R, 0.0R)
        End If

        Return (Math.Exp(logQ / observed), Math.Exp(logP / observed))
    End Function

    ''' <summary>
    ''' 对 motif 的显著性做启发式的保守估计：
    ''' 
    ''' 1. 逐列计算信息含量 bits（沿用 <see cref="MSAMotif.CreateMotif"/> 的口径，
    '''    其中包含 <see cref="Probability.E(Integer)"/> 的小样本校正）；
    ''' 2. 以整个 motif 的 bits 之和作为得分（它是 PWM 之下的期望位点对数似然比，单位为 bit），
    '''    再按 E = 候选位点总数 × 2^(-score) 折算。
    ''' 
    ''' 对于以 bit 计量的对数似然比得分，P(随机位点得分 ≥ score) ≤ 2^(-score) 成立，
    ''' 因此上面给出的是 E-value 的 Chernoff 上界，而不是严格意义上的 Karlin-Altschul E-value；
    ''' 作为上界它偏保守（同样的 motif 会给出偏大的 E 值）。
    ''' </summary>
    Private Function EstimateEvalue(motifMatrix As SequenceMatrix) As Double
        Dim En As Double = Probability.E(Math.Max(motifMatrix.rowSum, 1))
        Dim score As Double = 0

        For i As Integer = 0 To m_motifLength - 1
            Dim col As Double() = New Double(3) {}

            For j As Integer = 0 To 3
                col(j) = motifMatrix.probability(i, j)
            Next

            score += Probability.CalculatesBits(Probability.HI(col), En, NtMol:=True)
        Next

        ' 所有输入序列之上的候选起点总数
        Dim candidateSites As Double = 0

        For i As Integer = 0 To m_sequenceCount - 1
            candidateSites += m_sequenceLength(i) - m_motifLength + 1
        Next

        Return Math.Max(candidateSites, 1) * Math.Pow(2, -score)
    End Function

    Friend Function informationContent(motifs As List(Of String)) As Double
        Dim sm As New SequenceMatrix(motifs)
        Dim sum As Double = 0
        Dim d As Double = 0

        For i As Integer = 0 To m_motifLength - 1
            For j As Integer = 0 To 3
                d = sm.probability(i, j) * (Math.Log(sm.probability(i, j) * 4) / LOG_2)

                If Not d.IsNaNImaginary Then
                    sum += d
                End If
            Next
        Next

        Return sum
    End Function

    ''' <summary>
    ''' Implements the Gibbs Sampling algorithm found in the lawrence93.pdf </summary>
    ''' <param name="maxIterations">, maximum number of iterations sampling may take </param>
    ''' <returns> Sets of int predicting the position motifs located in each sequence </returns>
    Friend Function gibbsSample(maxIterations As Integer, S As List(Of String)) As List(Of Integer)
        Dim A = getRandomSites().AsList

        ' 背景概率在整轮采样中是不变量，没必要在每一次迭代里重复拷贝
        Dim P As List(Of Double) = m_globalBackground.ToList()

        For i As Integer = 0 To maxIterations - 1
            ' Choose the next sequence
            Dim idx As Integer = randf.Next(m_sequenceCount)
            Dim z As String = S(idx)

            ' Remove the sequence from the sequences and sites
            S.RemoveAt(idx)
            A.RemoveAt(idx)

            ' Run the predictive step on z
            Dim q_ij = predictiveUpdateStep(S, A)

            ' Run the sampling step on q_ij
            Dim a_z = samplingStep(q_ij, z, P)

            ' Add z back into the set of sequences and sites
            S.Insert(idx, z)
            A.Insert(idx, a_z)
        Next

        Return A
    End Function

    ''' <summary>
    ''' One of the sequenceLength sequences, z,
    ''' is chosen either at random
    ''' The pattern description q_{i,j} frequency is
    ''' then calculated from the current positions a_k
    ''' in all sequences excluding z </summary>
    ''' <param name="S">, the sequences other than z </param>
    ''' <param name="A">, the sites for the sequences other than z </param>
    Private Function predictiveUpdateStep(S As List(Of String), A As List(Of Integer)) As SequenceMatrix
        ' Compute q_{i,j} from the current positions a_k
        Return New SequenceMatrix(getMotifStrings(S, A))
    End Function

    ''' <summary>
    ''' Grabs the motif strings of length motifLength
    ''' from each sequence and site </summary>
    ''' <param name="S">, sequences </param>
    ''' <param name="A">, sites </param>
    ''' <returns> sequenceCount motif strings </returns>
    Friend Function getMotifStrings(S As IEnumerable(Of String), A As List(Of Integer)) As List(Of String)
        Dim motif As New List(Of String)
        Dim i As Integer = 0
        Dim span As String
        Dim site As Integer

        For Each sequence As String In S
            site = A(i)
            span = sequence.Substring(site, m_motifLength)
            i += 1
            motif.Add(span)
        Next

        Return motif
    End Function

    ''' <summary>
    ''' Every possible segment of width motifLength within sequence z
    ''' is considered as a possible instance of the pattern. The
    ''' probabilities Q_x of generating each segment x according to
    ''' the current pattern probabilities q_{i,j} are calculated
    ''' The weight A_x = Q_x/P_x is assigned to segment x, and
    ''' with each segment so weighted, a random one is selected.
    ''' Its position then becomes the new a_z. </summary>
    ''' <param name="z">, sequence we are iterating through </param>
    Private Function samplingStep(q_ij As SequenceMatrix, z As String, P As List(Of Double)) As Integer
        ' 使用当前序列长度而非全局最小长度
        ' 合法起点为 [0, z.Length - motifLength]，共 z.Length - motifLength + 1 个；
        ' 这里必须 +1，否则每条序列上的最后一个合法起点永远不会被采样到
        Dim candidates As Integer = z.Length - m_motifLength + 1
        Dim A As New List(Of Double)(candidates)

        ' 候选起点的个数通常只有几十个，PLINQ 的调度开销远大于计算本身，
        ' 这里改用普通串行循环，并行度由外层的重启级 Parallel.For 提供
        For x As Integer = 0 To candidates - 1
            A.Add(calculateMotifProbability(q_ij, z, x, P))
        Next

        Return weightedChooseIndex(A)
    End Function

    ''' <summary>
    ''' calculates the log probability of a character appearing at a specific index in a motif </summary>
    ''' <param name="q_ij">, motif weight matrix </param>
    ''' <param name="z">, string of characters </param>
    ''' <param name="x">, index of site in z </param>
    ''' <param name="P">, background frequencies </param>
    ''' <returns> log probability </returns>
    Private Function calculateMotifProbability(q_ij As SequenceMatrix,
                                               z As String,
                                               x As Integer,
                                               P As List(Of Double)) As Double
        Dim sum As Double = 0
        Dim q As Double
        Dim background As Double

        For i As Integer = 0 To m_motifLength - 1
            Dim baseIdx = Utils.indexOfBase(z(x + i))

            ' 窗口内含有 N，说明该候选起点落入了上一轮已发现 motif 的屏蔽区。
            ' 必须直接判定为不可选：若让它参与计算，由于 N 位被跳过，
            ' 完全落在屏蔽区内的窗口会拿到 0 分（等价于随机背景水平），
            ' 当真实位点的对数似然比为负时反而会被优先采样到，
            ' 且随着屏蔽轮次累积，屏蔽区会越来越大，问题会愈发严重。
            If baseIdx < 0 Then
                Return MASKED_LOG_WEIGHT
            End If

            q = q_ij.probability(i, baseIdx)
            background = P(baseIdx)

            ' 背景概率为 0 时 q / 0 会得到 +Inf，使得该候选起点的权重恒为
            ' +Inf 从而独占整个采样分布，这里做下限钳制保证似然比始终有限
            If background <= 0 Then
                background = MIN_BACKGROUND
            End If

            ' 累加对数似然比 log(q / P)
            sum += Math.Log(q / background)
        Next

        Return sum
    End Function

    ''' <summary>
    ''' 依据 log 概率分布做轮盘赌随机采样（而非贪心地选取最大值），
    ''' 这正是吉布斯采样能够跳出局部最优的关键所在。
    ''' </summary>
    ''' <param name="weightDistribution">, 每一个候选起点上的 log 似然比 </param>
    ''' <returns> new index of the site </returns>
    Private Function weightedChooseIndex(weightDistribution As List(Of Double)) As Integer
        Dim n As Integer = weightDistribution.Count

        If n = 0 Then
            Return 0
        End If

        ' log-sum-exp 稳定化：
        ' log 似然比是 W 项之和，当 motif 较宽时其典型值可以远低于 -700，
        ' 直接 Exp 会全部下溢为 0，使得采样静默退化为均匀分布而彻底失去导向性；
        ' 反过来较大的正值又会溢出为 +Inf。先减去最大值即可同时避免这两种情况。
        Dim max As Double = Double.NegativeInfinity

        For i As Integer = 0 To n - 1
            If weightDistribution(i) > max Then
                max = weightDistribution(i)
            End If
        Next

        If max.Equals(Double.NegativeInfinity) OrElse max.IsNaNImaginary Then
            ' 所有候选起点都不可达，退化为均匀随机采样
            Return randf.Next(n)
        End If

        Dim probabilities As Double() = New Double(n - 1) {}
        Dim total As Double = 0

        For i As Integer = 0 To n - 1
            probabilities(i) = Math.Exp(weightDistribution(i) - max)
            total += probabilities(i)
        Next

        If total <= 0 OrElse total.IsNaNImaginary Then
            Return randf.Next(n)
        End If

        ' 轮盘赌选择
        Dim r As Double = randf.NextDouble() * total
        Dim cumulative As Double = 0.0

        For i As Integer = 0 To n - 1
            cumulative += probabilities(i)

            If r <= cumulative Then
                Return i
            End If
        Next

        Return n - 1 ' 浮点误差保护
    End Function

    ''' <summary>
    ''' Creates a list of sequenceLength random numbers
    ''' using the random object supplied
    ''' the numbers are from 0 to sequenceLength-motifLength-1 inclusive </summary>
    ''' <returns> sequenceLength random ints </returns>
    Private Iterator Function getRandomSites() As IEnumerable(Of Integer)
        For i As Integer = 0 To m_sequenceCount - 1
            Yield randf.Next(m_sequenceLength(i) - m_motifLength)
        Next
    End Function
End Class
