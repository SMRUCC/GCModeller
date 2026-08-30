Namespace Options

    ''' <summary>命令行选项（已应用任务预设后的最终参数）</summary>
    Public Class BlastOptions

        Public Program As String = "blastn"       ' blastn | blastp
        Public Task As String = "blastn"          ' megablast | dc-megablast | blastn | blastn-short | blastp | blastp-short
        Public WordSize As Integer = 11
        Public Reward As Double = 2.0             ' nt 匹配得分
        Public Penalty As Double = -3.0           ' nt 错配得分
        Public Matrix As String = "BLOSUM62"      ' 蛋白矩阵
        Public Threshold As Integer = 11          ' 蛋白邻域词阈值 T
        Public GapOpen As Double = 5.0
        Public GapExtend As Double = 2.0
        Public EvalueCutoff As Double = 10.0
        Public WindowTwoHit As Integer = 40       ' 两-hit 窗 A
        Public UseTwoHit As Boolean = True
        Public Dust As Boolean = True
        Public DustLevel As Integer = 20
        Public Seg As Boolean = True
        Public CompBasedStats As Integer = 0      ' 0=关 1=简化组成校正（2/3 回落为 1 并标记）
        Public XdropUngap As Double = 20.0        ' bits
        Public XdropGap As Double = 30.0          ' bits（预延伸）
        Public XdropGapFinal As Double = 100.0    ' bits（最终延伸）
        Public MaxTargetSeqs As Integer = 500
        Public MaxHsps As Integer = 50

    End Class
End Namespace