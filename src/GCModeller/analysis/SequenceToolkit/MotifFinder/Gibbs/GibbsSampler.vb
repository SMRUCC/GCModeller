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
    '     Function: CalculateGlobalBackground, calculateMotifProbability, calculateP, FillNNNN, find
    '               getMotifStrings, getRandomSites, gibbsSample, informationContent, minExceptInfinity
    '               predictiveUpdateStep, samplingStep, smoothProbabilities, weightedChooseIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.GibbsSampling
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

        Dim parallelOptions As New ParallelOptions With {
            .MaxDegreeOfParallelism = Environment.ProcessorCount
        }

        Call System.Threading.Tasks.Parallel.For(
            fromInclusive:=0,
            toExclusive:=numSamples,
            parallelOptions,
            body:=Sub(j)
                      Call sampler.RunOne(maxIterations)
                  End Sub)

        Dim motifMatrix As WeightMatrix = New SequenceMatrix(sampler.predictedMotifs)
        Dim icpc As Double = CDbl(sampler.maxInformationContent) / m_motifLength
        Dim p As Double() = New Double(sampler.predictedMotifs.Count - 1) {}
        Dim q As Double() = New Double(sampler.predictedMotifs.Count - 1) {}
        Dim eval As New Gibbs(Sequences.ToArray, m_motifLength)

        Call println("======== Maximum Information Content :: " & icpc & " =========" & vbLf)

        For i As Integer = 0 To sampler.predictedMotifs.Count - 1
            Dim pq = eval.PQ(i)

            p(i) = pq.p.Average
            q(i) = pq.q.Average
        Next

        Return New MSAMotif With {
            .cost = icpc,
            .MSA = sampler.predictedMotifs.ToArray,
            .names = m_sequences.Select(Function(fa) fa.Title).ToArray,
            .start = New ints(sampler.predictedSites),
            .countMatrix = motifMatrix.countsMatrix _
                .Select(Function(n) New ints(n)) _
                .ToArray,
            .rowSum = motifMatrix.rowSum,
            .p = p,
            .q = q,
            .alphabets = Utils.ACGT
        }
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
    ''' Calculates the background probabilities for each base </summary>
    ''' <param name="S">, sequenceCount sequences </param>
    ''' <returns> List of Double length 4 </returns>
    Private Function calculateP(S As List(Of String)) As List(Of Double)
        Dim P = New Double() {0, 0, 0, 0}

        For i As Integer = 0 To S.Count - 1
            Dim seq As String = S(i)

            For j As Integer = 0 To seq.Length - 1
                Dim c As Char = seq(j)
                Dim offset As Integer = Utils.indexOfBase(c)

                If offset > -1 Then
                    P(offset) += 1
                End If
            Next
        Next

        Dim sum As Double = P.Sum()

        Return P.Select(Function(d) d / sum).ToList()
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

            If baseIdx > -1 Then
                q = q_ij.probability(i, baseIdx)
                background = P(baseIdx)

                ' 背景概率为 0 时 q / 0 会得到 +Inf，使得该候选起点的权重恒为
                ' +Inf 从而独占整个采样分布，这里做下限钳制保证似然比始终有限
                If background <= 0 Then
                    background = MIN_BACKGROUND
                End If

                ' 直接计算似然比 log(q / P)
                sum += Math.Log(q / background)
            End If
        Next

        Return sum
    End Function

    ''' <summary>
    ''' Takes Q a list of log probabilities
    ''' Replaces negative infinities with 1 less than the minimum log probability </summary>
    ''' <param name="A">, log probabilities </param>
    ''' <returns> list of smoothed probabilities </returns>
    Private Function smoothProbabilities(A As List(Of Double)) As List(Of Double)
        ' Find the smallest probability greater than 0
        Dim min As Double = A.Aggregate(Double.NegativeInfinity, AddressOf minExceptInfinity)

        ' Assert that there is some non zero probability so that we may smooth
        If min <= Double.NegativeInfinity + 1 Then
            Return A.Select(Function(i) -100.0).ToList()
        Else
            ' Replace the 0 probability indices with (min - 1) log probability
            Return A.Select(Function(i) If(i.Equals(Double.NegativeInfinity), min - 1, i)).ToList()
        End If
    End Function

    Private Shared Function minExceptInfinity(i As Double, b As Double) As Double
        Return If(i < b AndAlso Not b.Equals(Double.NegativeInfinity), b, i)
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
