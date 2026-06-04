
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
    Public Shared Function ComputeUpstreamScore(orf As CandidateORF, fullSequence As String) As Double
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
