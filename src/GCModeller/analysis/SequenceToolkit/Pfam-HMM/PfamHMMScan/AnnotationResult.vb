''' <summary>
''' 注释结果类
''' </summary>
Public Class AnnotationResult

    ''' <summary>
    ''' 匹配的模型名称
    ''' </summary>
    Public Property ModelName As String

    ''' <summary>
    ''' 比特得分
    ''' </summary>
    Public Property BitScore As Double

    ''' <summary>
    ''' E值
    ''' </summary>
    Public Property EValue As Double

    ''' <summary>
    ''' 是否通过阈值
    ''' </summary>
    Public Property IsSignificant As Boolean

    ''' <summary>
    ''' 匹配的起始位置
    ''' </summary>
    Public Property AlignmentStart As Integer

    ''' <summary>
    ''' 匹配的结束位置
    ''' </summary>
    Public Property AlignmentEnd As Integer

    ''' <summary>
    ''' 匹配区域序列
    ''' </summary>
    Public Property AlignedSequence As String

    ''' <summary>
    ''' 置信度
    ''' </summary>
    Public Property Confidence As Double

    ''' <summary>
    ''' 功能注释
    ''' </summary>
    Public Property FunctionalAnnotation As String

    Public Overrides Function ToString() As String
        Return $"Model: {ModelName}, Score: {BitScore:F2}, E-value: {EValue:G2}, Significant: {IsSignificant}"
    End Function

End Class
