Imports std = System.Math

''' <summary>
''' 模块与表型相关性结果
''' </summary>
Public Class ModulePhenotypeCorrelation
    ''' <summary>
    ''' 模块名称
    ''' </summary>
    Public Property ModuleName As String

    ''' <summary>
    ''' 表型名称
    ''' </summary>
    Public Property PhenotypeName As String

    ''' <summary>
    ''' 相关系数
    ''' </summary>
    Public Property Correlation As Double

    ''' <summary>
    ''' 相关系数的绝对值
    ''' </summary>
    Public ReadOnly Property AbsoluteCorrelation As Double
        Get
            Return std.Abs(Correlation)
        End Get
    End Property

    ''' <summary>
    ''' p值（统计显著性）
    ''' </summary>
    Public Property PValue As Double

    ''' <summary>
    ''' 样本数量
    ''' </summary>
    Public Property SampleCount As Integer

    ''' <summary>
    ''' 是否显著（默认p &lt; 0.05）
    ''' </summary>
    Public ReadOnly Property IsSignificant As Boolean
        Get
            Return PValue < 0.05
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"Module '{ModuleName}' vs '{PhenotypeName}': r={Correlation:F3}, p={PValue:F4}"
    End Function
End Class