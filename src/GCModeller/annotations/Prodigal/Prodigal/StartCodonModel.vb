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
