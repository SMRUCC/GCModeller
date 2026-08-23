
''' <summary>
''' 基因显著性结果
''' </summary>
Public Class GeneSignificanceResult
    ''' <summary>
    ''' 基因ID
    ''' </summary>
    Public Property GeneId As String

    ''' <summary>
    ''' 表型名称
    ''' </summary>
    Public Property PhenotypeName As String

    ''' <summary>
    ''' 相关系数
    ''' </summary>
    Public Property Correlation As Double

    ''' <summary>
    ''' 相关系数的绝对值（基因显著性GS）
    ''' </summary>
    Public Property AbsoluteCorrelation As Double

    ''' <summary>
    ''' p值
    ''' </summary>
    Public Property PValue As Double

    ''' <summary>
    ''' 样本数量
    ''' </summary>
    Public Property SampleCount As Integer

    Public Overrides Function ToString() As String
        Return $"Gene '{GeneId}' vs '{PhenotypeName}': GS={AbsoluteCorrelation:F3}, p={PValue:F4}"
    End Function
End Class