Imports SMRUCC.genomics.SequenceModel.FASTA

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
    Private Shared ReadOnly SdMotifs As New Dictionary(Of String, Double) From {
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
    Public Shared Sub BuildModel(model As TrainingModel, sequences As IEnumerable(Of FastaSeq),
                                  trainingOrfs As List(Of CandidateOrf))
        ' 构建序列索引以便快速查找上游序列
        Dim seqDict As New Dictionary(Of String, String)
        For Each seq As FastaSeq In sequences
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
