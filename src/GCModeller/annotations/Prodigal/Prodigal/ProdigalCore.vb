' ============================================================================
' ProdigalCore.vb - Prodigal VB.NET 基因预测程序 核心算法
' 包含：ORF查找、编码区打分、RBS检测、起始密码子评分、
'       动态规划基因选择、无监督训练引擎、预测管线
' ============================================================================

' ========================================================================
' ORF查找器 - 六框翻译找所有候选ORF
' ========================================================================

Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 在六框翻译中查找所有候选ORF
''' 核心思路：对每个阅读框，先找所有终止密码子位置，再在每个终止-终止区间内
''' 查找所有可能的起始密码子，生成多个候选ORF（同一终止密码子，不同起始密码子）
''' </summary>
Public Class OrfFinder

    ''' <summary>最小ORF长度（核苷酸数），默认90nt=30aa</summary>
    Private _minOrfLength As Integer

    ''' <summary>允许的起始密码子列表</summary>
    Private _startCodons As String()

    Public Sub New(Optional minOrfLength As Integer = 90)
        _minOrfLength = minOrfLength
        _startCodons = {"ATG", "GTG", "TTG"}
    End Sub

    ''' <summary>
    ''' 在一条序列上查找所有候选ORF
    ''' </summary>
    Public Function FindOrfs(fastaSeq As FastaSeq) As List(Of CandidateOrf)
        Dim orfs As New List(Of CandidateOrf)()
        Dim seq = fastaSeq.SequenceData
        Dim seqId = fastaSeq.locus_tag

        If seq.Length < 6 Then Return orfs

        ' 正向链（阅读框0, 1, 2）
        FindOrfsOnStrand(seq, seqId, "+"c, orfs)

        ' 反向链
        Dim rcSeq = SequenceUtils.ReverseComplement(seq)
        FindOrfsOnStrand(rcSeq, seqId, "-"c, orfs)

        ' 转换反向链ORF的坐标到原始序列坐标系
        For Each orf In orfs
            If orf.Strand = "-"c Then
                ConvertReverseStrandCoords(orf, seq.Length)
            End If
        Next

        Return orfs
    End Function

    ''' <summary>
    ''' 在单条链上查找ORF
    ''' 算法：对每个阅读框，先定位所有终止密码子，再在每个终止-终止区间内
    ''' 查找所有起始密码子，为每个起始-终止对生成一个候选ORF
    ''' </summary>
    Private Sub FindOrfsOnStrand(seq As String, seqId As String, strand As Char, orfs As List(Of CandidateOrf))
        For frame As Integer = 0 To 2
            ' 第一步：收集该阅读框内所有终止密码子的位置
            Dim stopPositions As New List(Of Integer)
            For i As Integer = frame To seq.Length - 3 Step 3
                Dim codon = seq.Substring(i, 3)
                If SequenceUtils.IsStopCodon(codon) Then
                    stopPositions.Add(i)
                End If
            Next

            ' 第二步：在每个终止-终止区间内查找起始密码子
            Dim regionStart As Integer = frame  ' 区间起始（上一个终止密码子之后，或序列开头）

            For Each stopPos In stopPositions
                ' 在 [regionStart, stopPos) 范围内查找所有起始密码子
                Dim foundStart As Boolean = False
                For i As Integer = regionStart To stopPos - 3 Step 3
                    Dim codon = seq.Substring(i, 3)
                    If Array.IndexOf(_startCodons, codon) >= 0 Then
                        ' 找到一个起始密码子，生成候选ORF
                        Dim orfLen = stopPos + 3 - i
                        If orfLen >= _minOrfLength Then
                            Dim orfSeq = seq.Substring(i, stopPos + 3 - i)
                            Dim aaSeq = SequenceUtils.Translate(orfSeq)
                            ' 去除末尾的终止密码子对应的 '*'
                            If aaSeq.Length > 0 AndAlso aaSeq(aaSeq.Length - 1) = "*"c Then
                                aaSeq = aaSeq.Substring(0, aaSeq.Length - 1)
                            End If

                            Dim orf As New CandidateOrf With {
                                .SeqId = seqId,
                                .RawStart = i,
                                .RawEnd = stopPos + 2,
                                .Start = i + 1,  ' 暂用0-based+1，反向链后面会转换
                                .[End] = stopPos + 3,
                                .Strand = strand,
                                .Frame = frame,
                                .StartCodon = codon,
                                .StopCodon = seq.Substring(stopPos, 3),
                                .Length = orfLen,
                                .NtSequence = orfSeq,
                                .AaSequence = aaSeq
                            }
                            orfs.Add(orf)
                            foundStart = True
                        End If
                    End If
                Next

                ' 如果该区间没有起始密码子，跳过（非编码区间）
                regionStart = stopPos + 3
            Next

            ' 第三步：处理序列末尾的边缘ORF（无终止密码子）
            If regionStart < seq.Length - 2 Then
                For i As Integer = regionStart To seq.Length - 3 Step 3
                    If i + 2 >= seq.Length Then Exit For
                    Dim codon = seq.Substring(i, 3)
                    If Array.IndexOf(_startCodons, codon) >= 0 Then
                        Dim orfLen = seq.Length - i
                        If orfLen >= _minOrfLength Then
                            Dim orfSeq = seq.Substring(i)
                            Dim aaSeq = SequenceUtils.Translate(orfSeq)

                            Dim orf As New PredictedGene With {
                                .SeqId = seqId,
                                .RawStart = i,
                                .RawEnd = seq.Length - 1,
                                .Start = i + 1,
                                .[End] = seq.Length,
                                .Strand = strand,
                                .Frame = frame,
                                .StartCodon = codon,
                                .StopCodon = "---",
                                .Length = orfLen,
                                .NtSequence = orfSeq,
                                .AaSequence = aaSeq,
                                .PartialType = "3'"  ' 3'端截断
                            }
                            orfs.Add(orf)
                        End If
                    End If
                Next
            End If

            ' 第四步：处理序列开头的边缘ORF（5'端截断，无起始密码子）
            ' 在第一个终止密码子之前，可能存在没有起始密码子的部分ORF
            If stopPositions.Count > 0 Then
                Dim firstStop = stopPositions(0)
                If firstStop >= _minOrfLength Then
                    ' 检查从序列开头到第一个终止密码子之间是否有in-frame的编码区
                    Dim partialOrfSeq = seq.Substring(frame, firstStop + 3 - frame)
                    Dim aaSeq = SequenceUtils.Translate(partialOrfSeq)
                    If aaSeq.Length > 10 Then  ' 至少10个氨基酸
                        Dim orf As New PredictedGene With {
                            .SeqId = seqId,
                            .RawStart = frame,
                            .RawEnd = firstStop + 2,
                            .Start = frame + 1,
                            .[End] = firstStop + 3,
                            .Strand = strand,
                            .Frame = frame,
                            .StartCodon = ">>>",  ' 边缘标记
                            .StopCodon = seq.Substring(firstStop, 3),
                            .Length = firstStop + 3 - frame,
                            .NtSequence = partialOrfSeq,
                            .AaSequence = aaSeq,
                            .PartialType = "5'"  ' 5'端截断
                        }
                        orfs.Add(orf)
                    End If
                End If
            End If
        Next
    End Sub

    ''' <summary>
    ''' 将反向链ORF的坐标从反向互补链坐标转换为原始序列坐标
    ''' </summary>
    Private Sub ConvertReverseStrandCoords(orf As CandidateOrf, seqLen As Integer)
        ' 反向链上：rawStart在RC序列中的位置 → 原始序列中的位置
        ' 原始序列位置 = seqLen - rcPos - 1
        Dim origStart = seqLen - orf.RawEnd
        Dim origEnd = seqLen - orf.RawStart
        orf.Start = origStart + 1  ' 1-based
        orf.[End] = origEnd
    End Sub

End Class

' ========================================================================
' 编码区打分模型 - 基于六聚体频率的对数似然比
' ========================================================================

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
    Public Shared Sub BuildModel(model As TrainingModel, sequences As List(Of FastaSeq),
                                  trainingOrfs As List(Of CandidateOrf))
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
    Public Shared Function ComputeCodingScore(orf As CandidateOrf, model As TrainingModel) As Double
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

' ========================================================================
' RBS（Shine-Dalgarno）检测模型
' ========================================================================

''' <summary>
''' RBS检测模型
''' 在起始密码子上游搜索Shine-Dalgarno序列（如AGGAGG、GGAGG等），
''' 并基于位置权重矩阵（PWM）和间距评分来计算RBS得分（rscore）
''' </summary>
Public Class RbsModel

    ''' <summary>RBS搜索窗口：起始密码子上游的最大距离</summary>
    Private Const UpstreamWindow As Integer = 20

    ''' <summary>RBS搜索窗口：起始密码子上游的最小距离</summary>
    Private Const MinSpacing As Integer = 4

    ''' <summary>预定义的SD模体及其权重</summary>
    Private Shared ReadOnly SdMotifs As Dictionary(Of String, Double) =
        New Dictionary(Of String, Double) From {
            {"AGGAGG", 10.0},
            {"AGGAG", 7.0},
            {"GGAGG", 7.0},
            {"GGAG", 5.0},
            {"GAGG", 5.0},
            {"AGGA", 5.0},
            {"AGG", 3.0},
            {"GAG", 3.0},
            {"GGG", 2.0},
            {"GAAGGAG", 12.0},
            {"GGAGGT", 8.0},
            {"AGGAGT", 8.0}
        }

    ''' <summary>
    ''' 从训练基因构建RBS模型
    ''' 统计起始密码子上游的SD模体频率，更新模体得分
    ''' </summary>
    Public Shared Sub BuildModel(model As TrainingModel, sequences As List(Of FastaSeq),
                                  trainingOrfs As List(Of CandidateOrf))
        ' 构建序列索引以便快速查找上游序列
        Dim seqDict As New Dictionary(Of String, String)
        For Each seq In sequences
            seqDict(seq.locus_tag) = seq.SequenceData.ToUpper()
        Next

        ' 统计各SD模体在上游出现的次数
        Dim motifCounts As New Dictionary(Of String, Integer)
        For Each motif In model.RbsMotifs
            motifCounts(motif) = 0
        Next
        Dim totalMotifHits As Integer = 0

        ' 统计间距分布
        Dim spacingCounts(UpstreamWindow) As Integer

        For Each orf In trainingOrfs
            If orf.Strand <> "+"c Then Continue For  ' 简化：只统计正向链
            If Not seqDict.ContainsKey(orf.SeqId) Then Continue For

            Dim seq = seqDict(orf.SeqId)
            Dim upstreamStart = Math.Max(0, orf.RawStart - UpstreamWindow)
            Dim upstreamLen = orf.RawStart - upstreamStart
            If upstreamLen < MinSpacing Then Continue For

            Dim upstream = seq.Substring(upstreamStart, upstreamLen)

            ' 在上游序列中搜索SD模体
            Dim bestMotif As String = ""
            Dim bestSpacing As Integer = 0
            Dim bestScore As Double = 0

            For Each kv In SdMotifs
                Dim motif = kv.Key
                Dim baseScore = kv.Value
                Dim pos = upstream.LastIndexOf(motif)
                If pos >= 0 Then
                    Dim spacing = upstreamLen - pos - motif.Length
                    If spacing >= MinSpacing - 1 AndAlso spacing <= 15 Then
                        ' 间距评分：最优间距约5-10bp
                        Dim spacingScore = ComputeSpacingScore(spacing)
                        Dim totalScore = baseScore * spacingScore
                        If totalScore > bestScore Then
                            bestScore = totalScore
                            bestMotif = motif
                            bestSpacing = spacing
                        End If
                    End If
                End If
            Next

            If bestMotif <> "" Then
                If motifCounts.ContainsKey(bestMotif) Then
                    motifCounts(bestMotif) += 1
                Else
                    motifCounts(bestMotif) = 1
                End If
                totalMotifHits += 1
                If bestSpacing >= 0 AndAlso bestSpacing <= UpstreamWindow Then
                    spacingCounts(bestSpacing) += 1
                End If
            End If
        Next

        ' 更新RBS模体得分
        model.RbsMotifScores.Clear()
        If totalMotifHits > 0 Then
            For Each kv In motifCounts
                model.RbsMotifScores(kv.Key) = CDbl(kv.Value) / totalMotifHits
            Next
        Else
            ' 使用默认得分
            For Each kv In SdMotifs
                model.RbsMotifScores(kv.Key) = kv.Value / SdMotifs.Values.Sum()
            Next
        End If

        ' 构建RBS PWM（6个位置，4种碱基）
        BuildRbsPwm(model, trainingOrfs, seqDict)
    End Sub

    ''' <summary>
    ''' 构建RBS位置权重矩阵
    ''' </summary>
    Private Shared Sub BuildRbsPwm(model As TrainingModel, trainingOrfs As List(Of CandidateOrf),
                                    seqDict As Dictionary(Of String, String))
        ' 初始化PWM计数
        Dim pwmCounts(5, 3) As Double  ' 6个位置，ACGT
        For i As Integer = 0 To 5
            For j As Integer = 0 To 3
                pwmCounts(i, j) = 1.0  ' 伪计数
            Next
        Next

        Dim totalSequences As Integer = 0

        For Each orf In trainingOrfs
            If orf.Strand <> "+"c Then Continue For
            If Not seqDict.ContainsKey(orf.SeqId) Then Continue For

            Dim seq = seqDict(orf.SeqId)
            ' 取起始密码子上游7-12bp的6个位置作为RBS窗口
            Dim rbsStart = orf.RawStart - 12
            If rbsStart < 0 OrElse rbsStart + 5 >= seq.Length Then Continue For

            For pos As Integer = 0 To 5
                Dim baseIdx = BaseToIndex(seq(rbsStart + pos))
                If baseIdx >= 0 Then
                    pwmCounts(pos, baseIdx) += 1
                End If
            Next
            totalSequences += 1
        Next

        ' 转换为频率
        For i As Integer = 0 To 5
            Dim total = 0.0
            For j As Integer = 0 To 3
                total += pwmCounts(i, j)
            Next
            For j As Integer = 0 To 3
                model.RbsPwm(i, j) = pwmCounts(i, j) / total
            Next
        Next
    End Sub

    ''' <summary>
    ''' 计算一个ORF的RBS得分（rscore）
    ''' 在起始密码子上游搜索SD模体，返回最佳匹配的得分
    ''' </summary>
    Public Shared Function ComputeRbsScore(orf As CandidateOrf, fullSequence As String,
                                           model As TrainingModel) As Double
        If String.IsNullOrEmpty(fullSequence) Then Return 0.0

        Dim seq = fullSequence.ToUpper()
        Dim upstreamStart As Integer
        Dim upstreamLen As Integer

        If orf.Strand = "+"c Then
            upstreamStart = Math.Max(0, orf.RawStart - UpstreamWindow)
            upstreamLen = orf.RawStart - upstreamStart
        Else
            ' 反向链：在RC序列上，起始密码子的上游对应原始序列的下游
            ' 简化处理：使用原始序列坐标
            Dim orfEnd = orf.RawEnd
            upstreamStart = Math.Min(seq.Length, orfEnd + 1)
            upstreamLen = Math.Min(UpstreamWindow, seq.Length - upstreamStart)
        End If

        If upstreamLen < MinSpacing Then Return 0.0

        Dim upstream As String
        If orf.Strand = "+"c Then
            upstream = seq.Substring(upstreamStart, upstreamLen)
        Else
            ' 反向链的上游需要取反向互补
            Dim downstream = seq.Substring(upstreamStart, upstreamLen)
            upstream = SequenceUtils.ReverseComplement(downstream)
        End If

        ' 搜索最佳SD模体
        Dim bestScore As Double = 0.0
        Dim bestMotif As String = ""
        Dim bestSpacing As Integer = 0

        For Each kv In SdMotifs
            Dim motif = kv.Key
            Dim baseScore = kv.Value
            Dim pos = upstream.LastIndexOf(motif)
            If pos >= 0 Then
                Dim spacing = upstreamLen - pos - motif.Length
                If spacing >= MinSpacing - 1 AndAlso spacing <= 15 Then
                    Dim spacingScore = ComputeSpacingScore(spacing)
                    ' 如果有训练好的模体频率，使用训练值
                    Dim motifFreq As Double = 1.0
                    If model.RbsMotifScores.ContainsKey(motif) Then
                        motifFreq = model.RbsMotifScores(motif) * 100  ' 放大权重
                    End If
                    Dim totalScore = baseScore * spacingScore * motifFreq
                    If totalScore > bestScore Then
                        bestScore = totalScore
                        bestMotif = motif
                        bestSpacing = spacing
                    End If
                End If
            End If
        Next

        ' 归一化得分
        Dim normalizedScore = Math.Min(bestScore / 20.0, 5.0)  ' 上限5分

        orf.RbsMotif = If(bestMotif = "", "None", bestMotif)
        orf.RbsSpacing = bestSpacing

        Return normalizedScore
    End Function

    ''' <summary>
    ''' 计算间距得分
    ''' 最优间距约5-9bp，偏离越多得分越低
    ''' </summary>
    Private Shared Function ComputeSpacingScore(spacing As Integer) As Double
        ' 使用高斯函数：最优间距=7，标准差=3
        Dim optimalSpacing As Double = 7.0
        Dim sigma As Double = 3.0
        Dim diff = spacing - optimalSpacing
        Return Math.Exp(-(diff * diff) / (2 * sigma * sigma))
    End Function

    ''' <summary>
    ''' 碱基转索引：A=0, C=1, G=2, T=3
    ''' </summary>
    Private Shared Function BaseToIndex(c As Char) As Integer
        Select Case Char.ToUpper(c)
            Case "A"c : Return 0
            Case "C"c : Return 1
            Case "G"c : Return 2
            Case "T"c : Return 3
            Case Else : Return -1
        End Select
    End Function

End Class

' ========================================================================
' 起始密码子评分模型
' ========================================================================

''' <summary>
''' 起始密码子评分模型
''' 统计不同起始密码子（ATG/GTG/TTG）的使用频率，
''' 计算起始密码子类型得分（tscore）
''' </summary>
Public Class StartCodonModel

    ''' <summary>
    ''' 从训练基因构建起始密码子频率模型
    ''' </summary>
    Public Shared Sub BuildModel(model As TrainingModel, trainingOrfs As List(Of CandidateOrf))
        Dim counts As New Dictionary(Of String, Integer) From {
            {"ATG", 0}, {"GTG", 0}, {"TTG", 0}
        }
        Dim total As Integer = 0

        For Each orf In trainingOrfs
            Dim codon = orf.StartCodon.ToUpper()
            If counts.ContainsKey(codon) Then
                counts(codon) += 1
                total += 1
            End If
        Next

        If total > 0 Then
            For Each kv In counts
                model.StartCodonFreq(kv.Key) = CDbl(kv.Value) / total
            Next
        End If
    End Sub

    ''' <summary>
    ''' 计算起始密码子类型得分（tscore）
    ''' ATG通常最常见，得分最高；GTG次之；TTG最低
    ''' </summary>
    Public Shared Function ComputeTypeScore(orf As CandidateOrf, model As TrainingModel) As Double
        Dim codon = orf.StartCodon.ToUpper()
        If model.StartCodonFreq.ContainsKey(codon) Then
            Dim freq = model.StartCodonFreq(codon)
            ' 使用对数频率作为得分，ATG约0.75 → log2(0.75) ≈ -0.42
            ' 但我们需要正向得分，所以使用 freq * 加权系数
            ' Prodigal中ATG约得2.5分，GTG约1.2分，TTG约0.5分
            If freq > 0 Then
                Return Math.Log(freq * 10, 2) * 2.0
            End If
        End If

        ' 默认得分
        Select Case codon
            Case "ATG" : Return 2.5
            Case "GTG" : Return 1.2
            Case "TTG" : Return 0.5
            Case Else : Return -1.0  ' 非标准起始密码子
        End Select
    End Function

End Class

' ========================================================================
' 上游序列评分
' ========================================================================

''' <summary>
''' 上游序列评分（uscore）
''' 评估起始密码子上游的序列特征，如A/T丰富度（典型原核启动子区域）
''' </summary>
Public Class UpstreamModel

    ''' <summary>
    ''' 计算上游序列得分
    ''' 原核生物启动子上游通常有A/T丰富的-10区和-35区
    ''' </summary>
    Public Shared Function ComputeUpstreamScore(orf As CandidateOrf, fullSequence As String) As Double
        If String.IsNullOrEmpty(fullSequence) Then Return 0.0

        Dim seq = fullSequence.ToUpper()
        Dim upstreamLen As Integer = 30
        Dim upstreamStart As Integer

        If orf.Strand = "+"c Then
            upstreamStart = Math.Max(0, orf.RawStart - upstreamLen)
            upstreamLen = orf.RawStart - upstreamStart
        Else
            Dim orfEnd = orf.RawEnd
            upstreamStart = Math.Min(seq.Length, orfEnd + 1)
            upstreamLen = Math.Min(30, seq.Length - upstreamStart)
        End If

        If upstreamLen < 5 Then Return 0.0

        Dim upstream As String
        If orf.Strand = "+"c Then
            upstream = seq.Substring(upstreamStart, upstreamLen)
        Else
            upstream = SequenceUtils.ReverseComplement(seq.Substring(upstreamStart, upstreamLen))
        End If

        ' 计算A/T含量（启动子区域通常A/T丰富）
        Dim atCount As Integer = 0
        For Each c In upstream
            If c = "A"c OrElse c = "T"c Then atCount += 1
        Next
        Dim atFreq = CDbl(atCount) / upstreamLen

        ' A/T丰富度得分：AT频率>0.6时给正分
        Dim uscore As Double = (atFreq - 0.5) * 4.0
        Return Math.Max(-2.0, Math.Min(2.0, uscore))
    End Function

End Class

' ========================================================================
' 综合打分引擎
' ========================================================================

''' <summary>
''' 综合打分引擎
''' 将编码区得分、RBS得分、起始密码子类型得分、上游序列得分
''' 组合为总得分：score = cscore + sscore
''' 其中 sscore = rscore + tscore + uscore
''' </summary>
Public Class ScoringEngine

    ''' <summary>
    ''' 对所有候选ORF进行打分
    ''' </summary>
    Public Shared Sub ScoreAll(orfs As List(Of CandidateOrf), model As TrainingModel,
                                fullSequence As String)
        For Each orf In orfs
            ' 编码区得分
            orf.CodingScore = CodingModel.ComputeCodingScore(orf, model)

            ' RBS得分
            orf.RbsScore = RbsModel.ComputeRbsScore(orf, fullSequence, model)

            ' 起始密码子类型得分
            orf.TypeScore = StartCodonModel.ComputeTypeScore(orf, model)

            ' 上游序列得分
            orf.UpstreamScore = UpstreamModel.ComputeUpstreamScore(orf, fullSequence)

            ' 起始位点得分 = rscore + tscore + uscore
            orf.StartScore = orf.RbsScore + orf.TypeScore + orf.UpstreamScore

            ' 总得分 = cscore + sscore
            orf.TotalScore = orf.CodingScore + orf.StartScore
        Next
    End Sub

    ''' <summary>
    ''' 对单条序列的所有ORF进行打分
    ''' </summary>
    Public Shared Sub ScoreForSequence(orfs As List(Of CandidateOrf), model As TrainingModel,
                                        sequence As String)
        ScoreAll(orfs, model, sequence)
    End Sub

End Class

' ========================================================================
' 动态规划基因选择
' ========================================================================

''' <summary>
''' 动态规划基因选择算法
''' 在满足基因不重叠（同链）等约束下，选择全局得分最高的基因组合。
''' 本质上是加权区间调度问题（Weighted Interval Scheduling）的变体。
''' 
''' 算法：
''' 1. 将所有候选ORF按终止位置排序
''' 2. 对每个ORF，找到不与其重叠的最近前驱ORF
''' 3. DP递推：dp[i] = max(score[i] + dp[p(i)], dp[i-1])
''' 4. 回溯得到最优基因集
''' </summary>
Public Class DynamicProgramming

    ''' <summary>最小基因间距（同链相邻基因之间的最小间隔bp数）</summary>
    Private Const MinGeneSpacing As Integer = 1

    ''' <summary>
    ''' 执行动态规划，选择最优基因组合
    ''' </summary>
    Public Shared Function SelectGenes(orfs As List(Of CandidateOrf)) As List(Of CandidateOrf)
        If orfs.Count = 0 Then Return New List(Of CandidateOrf)()

        ' 过滤掉得分过低的ORF
        Dim validOrfs = orfs.Where(Function(o) o.TotalScore > 0 AndAlso o.Length >= 90).ToList()
        If validOrfs.Count = 0 Then Return New List(Of CandidateOrf)()

        ' 按终止位置排序
        validOrfs = validOrfs.OrderBy(Function(o) o.[End]).ThenBy(Function(o) o.Start).ToList()

        ' 分链处理：正向链和反向链分别做DP
        Dim fwdOrfs = validOrfs.Where(Function(o) o.Strand = "+"c).ToList()
        Dim revOrfs = validOrfs.Where(Function(o) o.Strand = "-"c).ToList()

        Dim selectedFwd = DpSelect(fwdOrfs)
        Dim selectedRev = DpSelect(revOrfs)

        ' 合并结果
        Dim result = selectedFwd.Concat(selectedRev).OrderBy(Function(o) o.Start).ToList()

        ' 处理跨链重叠（可选：允许反向链基因重叠）
        ' Prodigal允许反向链基因重叠，所以这里直接合并

        Return result
    End Function

    ''' <summary>
    ''' 对单条链的ORF执行加权区间调度DP
    ''' </summary>
    Private Shared Function DpSelect(orfs As List(Of CandidateOrf)) As List(Of CandidateOrf)
        If orfs.Count = 0 Then Return New List(Of CandidateOrf)()

        Dim n = orfs.Count

        ' 按终止位置排序（已排序）
        For i As Integer = 0 To n - 1
            orfs(i).SortIndex = i
        Next

        ' 预计算每个ORF的不重叠前驱索引（二分查找）
        Dim prevIdx(n - 1) As Integer
        For i As Integer = 0 To n - 1
            prevIdx(i) = FindLastNonOverlapping(orfs, i)
        Next

        ' DP数组
        Dim dp(n) As Double
        Dim selected(n - 1) As Boolean

        dp(0) = 0  ' dp[0] = 不选任何ORF的得分 = 0

        For i As Integer = 0 To n - 1
            ' 选项1：不选ORF i
            Dim skipScore = If(i > 0, dp(i), 0)

            ' 选项2：选ORF i
            Dim takeScore = orfs(i).TotalScore
            If prevIdx(i) >= 0 Then
                takeScore += dp(prevIdx(i) + 1)  ' +1因为dp是1-indexed
            End If

            If takeScore > skipScore Then
                dp(i + 1) = takeScore
                selected(i) = True
            Else
                dp(i + 1) = skipScore
                selected(i) = False
            End If
        Next

        ' 回溯
        Dim result As New List(Of CandidateOrf)()
        Dim idx = n - 1
        While idx >= 0
            If selected(idx) Then
                orfs(idx).Selected = True
                result.Add(orfs(idx))
                idx = prevIdx(idx)
            Else
                idx -= 1
            End If
        End While

        result.Reverse()
        Return result
    End Function

    ''' <summary>
    ''' 二分查找：找到不与ORF i重叠的最后一个ORF的索引
    ''' </summary>
    Private Shared Function FindLastNonOverlapping(orfs As List(Of CandidateOrf), i As Integer) As Integer
        Dim targetEnd = orfs(i).Start - MinGeneSpacing
        Dim lo As Integer = 0
        Dim hi As Integer = i - 1
        Dim result As Integer = -1

        While lo <= hi
            Dim mid = (lo + hi) \ 2
            If orfs(mid).[End] < targetEnd Then
                result = mid
                lo = mid + 1
            Else
                hi = mid - 1
            End If
        End While

        Return result
    End Function

End Class

' ========================================================================
' 无监督训练引擎
' ========================================================================

''' <summary>
''' 无监督训练引擎
''' 从输入序列本身学习统计特征，不需要外接训练集。
''' 
''' 训练流程：
''' 1. 使用默认参数做一次快速预测
''' 2. 根据预测到的"高置信度"基因，估计编码区/非编码区统计量
''' 3. 重新构建编码模型和起始位点模型
''' 4. 用新模型重新打分和预测
''' 5. 迭代直到模型参数和基因预测结果稳定
''' </summary>
Public Class TrainingEngine

    ''' <summary>最大训练迭代次数</summary>
    Private Const MaxIterations As Integer = 5

    ''' <summary>收敛阈值（基因数量变化）</summary>
    Private Const ConvergenceThreshold As Integer = 5

    ''' <summary>训练用最小基因数</summary>
    Private Const MinTrainingGenes As Integer = 20

    ''' <summary>
    ''' 执行无监督训练，生成训练模型
    ''' </summary>
    Public Shared Function Train(sequences As IReadOnlyCollection(Of FastaSeq)) As TrainingModel
        Console.WriteLine("开始无监督训练...")

        Dim model As New TrainingModel()

        ' 计算整体GC含量
        Dim totalGc As Double = 0
        Dim totalLen As Integer = 0
        For Each seq In sequences
            totalGc += SequenceUtils.ComputeGcContent(seq.SequenceData) * seq.Length
            totalLen += seq.Length
        Next
        model.GcContent = If(totalLen > 0, totalGc / totalLen, 0.5)
        Console.WriteLine($"  GC含量: {model.GcContent:P1}")

        ' 初始化默认模型
        CodingModel.InitializeDefaultModel(model, model.GcContent)
        Console.WriteLine("  已初始化默认编码区模型")

        ' 迭代训练
        Dim prevGeneCount As Integer = 0

        For iteration = 1 To MaxIterations
            Console.WriteLine($"  迭代 {iteration}/{MaxIterations}...")

            ' 第一步：用当前模型预测基因
            Dim allOrfs As New List(Of CandidateOrf)()
            For Each seq In sequences
                Dim finder As New OrfFinder(90)
                Dim orfs = finder.FindOrfs(seq)
                ScoringEngine.ScoreForSequence(orfs, model, seq.SequenceData)
                allOrfs.AddRange(orfs)
            Next

            ' 第二步：选择高置信度基因作为训练集
            Dim selectedGenes = DynamicProgramming.SelectGenes(allOrfs)
            Console.WriteLine($"    预测基因数: {selectedGenes.Count}")

            If selectedGenes.Count < MinTrainingGenes Then
                Console.WriteLine($"    训练基因数不足({selectedGenes.Count} < {MinTrainingGenes})，使用默认模型")
                Exit For
            End If

            ' 第三步：用选中的基因重新构建模型
            ' 过滤：选择得分较高的基因
            Dim trainingGenes = SelectTrainingGenes(selectedGenes, sequences)
            Console.WriteLine($"    训练基因数: {trainingGenes.Count}")

            ' 构建编码区模型
            CodingModel.BuildModel(model, sequences, trainingGenes)

            ' 构建RBS模型
            RbsModel.BuildModel(model, sequences, trainingGenes)

            ' 构建起始密码子模型
            StartCodonModel.BuildModel(model, trainingGenes)

            ' 更新平均基因长度
            model.AvgGeneLength = trainingGenes.Average(Function(g) CDbl(g.Length))
            model.TrainingGeneCount = trainingGenes.Count
            model.IterationCount = iteration

            ' 检查收敛
            If iteration > 1 AndAlso Math.Abs(selectedGenes.Count - prevGeneCount) < ConvergenceThreshold Then
                Console.WriteLine("    模型已收敛")
                Exit For
            End If
            prevGeneCount = selectedGenes.Count
        Next

        model.Trained = True
        Console.WriteLine($"训练完成！迭代次数: {model.IterationCount}, 训练基因数: {model.TrainingGeneCount}")

        Return model
    End Function

    ''' <summary>
    ''' 选择用于训练的高置信度基因
    ''' 策略：选择得分在上半部分的基因，且长度>120bp
    ''' </summary>
    Private Shared Function SelectTrainingGenes(genes As List(Of CandidateOrf),
                                                  sequences As List(Of FastaSeq)) As List(Of CandidateOrf)
        ' 按总得分降序排序
        Dim sorted = genes.OrderByDescending(Function(g) g.TotalScore).ToList()

        ' 选择上半部分且长度>120bp的基因
        Dim cutoff = Math.Max(MinTrainingGenes, sorted.Count \ 2)
        Dim trainingGenes = sorted.Take(cutoff).Where(Function(g) g.Length >= 120).ToList()

        ' 如果还不够，降低标准
        If trainingGenes.Count < MinTrainingGenes Then
            trainingGenes = sorted.Where(Function(g) g.Length >= 90).Take(MinTrainingGenes).ToList()
        End If

        Return trainingGenes
    End Function

End Class

' ========================================================================
' 基因预测管线
' ========================================================================

''' <summary>
''' 基因预测管线
''' 整合ORF查找、打分、动态规划选择，输出最终预测结果
''' </summary>
Public Class PredictionPipeline

    ''' <summary>最小ORF长度</summary>
    Private _minOrfLength As Integer

    Public Sub New(Optional minOrfLength As Integer = 90)
        _minOrfLength = minOrfLength
    End Sub

    ''' <summary>
    ''' 使用已有模型进行基因预测
    ''' </summary>
    Public Function Predict(sequences As IReadOnlyCollection(Of FastaSeq), model As TrainingModel) As List(Of PredictionResult)
        Dim results As New List(Of PredictionResult)()

        For Each seq In sequences
            Console.WriteLine($"  预测序列: {seq.locus_tag } ({seq.Length:N0} bp)")

            ' 第一步：查找所有候选ORF
            Dim finder As New OrfFinder(_minOrfLength)
            Dim orfs = finder.FindOrfs(seq)
            Console.WriteLine($"    候选ORF数: {orfs.Count}")

            If orfs.Count = 0 Then
                results.Add(New PredictionResult With {
                    .SeqId = seq.locus_tag,
                    .SeqLength = seq.Length,
                    .Model = model
                })
                Continue For
            End If

            ' 第二步：对所有ORF打分
            ScoringEngine.ScoreForSequence(orfs, model, seq.SequenceData)

            ' 第三步：动态规划选择最优基因组合
            Dim selectedOrfs = DynamicProgramming.SelectGenes(orfs)
            Console.WriteLine($"    预测基因数: {selectedOrfs.Count}")

            ' 第四步：构建预测结果
            Dim result As New PredictionResult With {
                .SeqId = seq.locus_tag,
                .SeqLength = seq.Length,
                .Model = model
            }

            Dim geneIndex = 1
            For Each orf As CandidateOrf In selectedOrfs.OrderBy(Function(o) o.Start)
                Dim gene As New PredictedGene With {
                    .SeqId = orf.SeqId,
                    .Start = orf.Start,
                    .[End] = orf.[End],
                    .Strand = orf.Strand,
                    .Frame = orf.Frame,
                    .StartCodon = orf.StartCodon,
                    .StopCodon = orf.StopCodon,
                    .Length = orf.Length,
                    .CodingScore = orf.CodingScore,
                    .StartScore = orf.StartScore,
                    .RbsScore = orf.RbsScore,
                    .UpstreamScore = orf.UpstreamScore,
                    .TypeScore = orf.TypeScore,
                    .TotalScore = orf.TotalScore,
                    .AaSequence = orf.AaSequence,
                    .NtSequence = orf.NtSequence,
                    .RbsMotif = orf.RbsMotif,
                    .RbsSpacing = orf.RbsSpacing,
                    .GeneIndex = geneIndex,
                    .PartialType = orf.PartialType,
                    .RawStart = orf.RawStart,
                    .RawEnd = orf.RawEnd
                }

                ' 计算置信度
                gene.Confidence = ComputeConfidence(gene, model)
                result.Genes.Add(gene)
                geneIndex += 1
            Next

            results.Add(result)
        Next

        Return results
    End Function

    ''' <summary>
    ''' 计算基因置信度（0-1之间）
    ''' </summary>
    Private Function ComputeConfidence(gene As PredictedGene, model As TrainingModel) As Double
        ' 基于总得分的置信度
        ' 得分>20为高置信度，<5为低置信度
        Dim scoreConfidence = 1.0 / (1.0 + Math.Exp(-(gene.TotalScore - 10) / 5.0))

        ' 基于编码区得分的置信度
        Dim codingConfidence = 1.0 / (1.0 + Math.Exp(-(gene.CodingScore - 5) / 3.0))

        ' 综合置信度
        Return (scoreConfidence * 0.6 + codingConfidence * 0.4)
    End Function

End Class


