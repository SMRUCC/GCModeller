

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
