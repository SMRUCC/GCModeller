''' <summary>
''' 代表 DIAMOND BLASTP 结果文件 (.m8) 中的一行记录
''' </summary>
Public Class DiamondAnnotation
    ' 1. 查询序列ID
    Public Property QseqId As String

    ' 2. 目标序列ID
    Public Property SseqId As String

    ' 3. 比对一致性百分比
    Public Property Pident As Double

    ' 4. 比对长度
    Public Property Length As Integer

    ' 5. 错配数
    Public Property Mismatch As Integer

    ' 6. Gap 打开次数
    Public Property GapOpen As Integer

    ' 7. 查询序列起始位置
    Public Property QStart As Integer

    ' 8. 查询序列结束位置
    Public Property QEnd As Integer

    ' 9. 目标序列起始位置
    Public Property SStart As Integer

    ' 10. 目标序列结束位置
    Public Property SEnd As Integer

    ' 11. E-value (期望值)
    Public Property EValue As Double

    ' 12. Bit Score (比特得分)
    Public Property BitScore As Double

    ''' <summary>
    ''' 重写 ToString，方便调试输出
    ''' </summary>
    Public Overrides Function ToString() As String
        Return $"{QseqId} vs {SseqId} | Identity: {Pident}% | E-value: {EValue}"
    End Function
End Class
