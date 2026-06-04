
' ========================================================================
' 编码区打分模型 - 基于六聚体频率的对数似然比
' ========================================================================

Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 编码区打分模型
''' 使用六聚体（6-mer）频率的编码区/非编码区对数似然比来评估
''' 一段序列的编码潜力。这是Prodigal cscore的核心。
''' </summary>
Public Class CodingModel

    ''' <summary>伪计数，防止零概率</summary>
    Private Const Pseudocount As Double = 0.5

    ''' <summary>
    ''' 从训练基因构建编码区和非编码区的六聚体频率模型
    ''' </summary>
    Public Shared Sub BuildModel(model As TrainingModel, sequences As IEnumerable(Of FastaSeq),
                                  trainingOrfs As List(Of CandidateORF))
        ' 重置计数
        Array.Clear(model.CodingHexamerCount, 0, 4096)
        Array.Clear(model.NoncodingHexamerCount, 0, 4096)
        model.CodingHexamerTotal = 0
        model.NoncodingHexamerTotal = 0

        ' 第一步：统计编码区六聚体（in-frame）
        For Each orf In trainingOrfs
            If String.IsNullOrEmpty(orf.NtSequence) Then Continue For
            Dim ntSeq = orf.NtSequence.ToUpper()
            ' 只统计in-frame的六聚体（从第一个密码子开始，步长3）
            For i As Integer = 0 To ntSeq.Length - 6 Step 3
                Dim hexamer = ntSeq.Substring(i, 6)
                Dim idx = SequenceUtils.HexamerToIndex(hexamer)
                If idx >= 0 Then
                    model.CodingHexamerCount(idx) += 1
                    model.CodingHexamerTotal += 1
                End If
            Next
        Next

        ' 第二步：统计非编码区六聚体
        ' 使用所有序列的移框（frame+1和frame+2）作为非编码区样本
        For Each seq In sequences
            Dim s = seq.SequenceData.ToUpper()
            ' 移框1
            For i As Integer = 1 To s.Length - 6 Step 3
                Dim hexamer = s.Substring(i, 6)
                Dim idx = SequenceUtils.HexamerToIndex(hexamer)
                If idx >= 0 Then
                    model.NoncodingHexamerCount(idx) += 1
                    model.NoncodingHexamerTotal += 1
                End If
            Next
            ' 移框2
            For i As Integer = 2 To s.Length - 6 Step 3
                Dim hexamer = s.Substring(i, 6)
                Dim idx = SequenceUtils.HexamerToIndex(hexamer)
                If idx >= 0 Then
                    model.NoncodingHexamerCount(idx) += 1
                    model.NoncodingHexamerTotal += 1
                End If
            Next
        Next

        ' 第三步：计算对数似然比得分
        ComputeHexamerScores(model)
    End Sub

    ''' <summary>
    ''' 计算每个六聚体的对数似然比得分
    ''' score(h) = log2( P(h|coding) / P(h|noncoding) )
    ''' </summary>
    Public Shared Sub ComputeHexamerScores(model As TrainingModel)
        Dim codingTotal = model.CodingHexamerTotal + Pseudocount * 4096
        Dim noncodingTotal = model.NoncodingHexamerTotal + Pseudocount * 4096

        For i As Integer = 0 To 4095
            Dim pCoding = (model.CodingHexamerCount(i) + Pseudocount) / codingTotal
            Dim pNoncoding = (model.NoncodingHexamerCount(i) + Pseudocount) / noncodingTotal
            model.HexamerScores(i) = Math.Log(pCoding / pNoncoding, 2)
        Next
    End Sub

    ''' <summary>
    ''' 计算一个ORF的编码区得分（cscore）
    ''' cscore = 所有in-frame六聚体的对数似然比之和 / 六聚体数量
    ''' </summary>
    Public Shared Function ComputeCodingScore(orf As CandidateORF, model As TrainingModel) As Double
        If String.IsNullOrEmpty(orf.NtSequence) Then Return -100.0

        Dim ntSeq = orf.NtSequence.ToUpper()
        Dim scoreSum As Double = 0
        Dim count As Integer = 0

        For i As Integer = 0 To ntSeq.Length - 6 Step 3
            Dim hexamer = ntSeq.Substring(i, 6)
            Dim idx = SequenceUtils.HexamerToIndex(hexamer)
            If idx >= 0 Then
                scoreSum += model.HexamerScores(idx)
                count += 1
            End If
        Next

        If count = 0 Then Return -100.0

        ' 返回平均得分乘以长度因子的对数
        ' Prodigal中cscore倾向于与长度相关，但做了适度归一化
        Dim avgScore = scoreSum / count
        ' 长度修正：较长的ORF应该有更高的cscore
        ' 使用 sqrt(count) 作为长度因子，避免过长ORF得分过高
        Dim lengthFactor = Math.Sqrt(count)
        Return avgScore * lengthFactor
    End Function

    ''' <summary>
    ''' 使用默认参数初始化模型（用于第一次迭代）
    ''' 基于GC含量设置初始六聚体得分
    ''' </summary>
    Public Shared Sub InitializeDefaultModel(model As TrainingModel, gcContent As Double)
        ' 基于GC含量设置初始编码/非编码频率
        ' 在高GC基因组中，GC-rich的六聚体在编码区更常见
        Dim atFreq = (1.0 - gcContent) / 2.0
        Dim gcFreq = gcContent / 2.0

        For i As Integer = 0 To 4095
            Dim hexamer = SequenceUtils.IndexToHexamer(i)
            ' 计算该六聚体在随机序列中的期望频率
            Dim expectedFreq As Double = 1.0
            For Each c In hexamer
                If c = "A"c OrElse c = "T"c Then
                    expectedFreq *= atFreq
                Else
                    expectedFreq *= gcFreq
                End If
            Next

            ' 编码区中，密码子第三位偏好G/C（GC偏倚）
            ' 简单模型：给第三位为G/C的六聚体稍高的编码区概率
            Dim codingBias As Double = 1.0
            If hexamer(2) = "G"c OrElse hexamer(2) = "C"c Then codingBias *= 1.1
            If hexamer(5) = "G"c OrElse hexamer(5) = "C"c Then codingBias *= 1.1

            model.CodingHexamerCount(i) = expectedFreq * 10000 * codingBias
            model.NoncodingHexamerCount(i) = expectedFreq * 10000
        Next

        model.CodingHexamerTotal = model.CodingHexamerCount.Sum()
        model.NoncodingHexamerTotal = model.NoncodingHexamerCount.Sum()
        model.GcContent = gcContent

        ComputeHexamerScores(model)
    End Sub

End Class
