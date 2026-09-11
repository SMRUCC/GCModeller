' /********************************************************************************/
'
'  Rockhopper —— de novo 转录本模型
'
'  复刻自原始 Rockhopper `Java/DeNovoTranscript.vb`：
'  无参考基因组时组装得到的转录本，记录序列、长度、表达量（映射读段数）与 q 值。
'  输出 transcripts.txt 时列顺序为：Sequence / Length / Expression / QValue，
'  与 API 层 DeNovolTranscript.LoadDocument 的回读约定一致（Tokens(0)=序列、Tokens(2)=表达量）。
'
' /********************************************************************************/

Namespace Assembly

    ''' <summary>
    ''' 一条 de novo 组装得到的转录本。
    ''' </summary>
    Public Class DeNovoTranscript

        ''' <summary>转录本序列。</summary>
        Public Property Sequence As String
        ''' <summary>表达量（映射到该转录本的读段数归一化后的值）。</summary>
        Public Property Expression As Double
        ''' <summary>差异表达 q 值（未做差异分析时为 1.0）。</summary>
        Public Property QValue As Double = 1.0
        ''' <summary>映射到该转录本的读段数（覆盖支持度）。</summary>
        Public Property Reads As Integer

        Public ReadOnly Property Length As Integer
            Get
                Return If(Sequence Is Nothing, 0, Sequence.Length)
            End Get
        End Property

        Public Sub New()
        End Sub

        Public Sub New(sequence As String)
            Me.Sequence = sequence
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Sequence}{vbTab}{Length}{vbTab}{Expression:F4}{vbTab}{QValue:F4}"
        End Function

    End Class

End Namespace
