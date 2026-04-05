Imports std = System.Math

''' <summary>
''' 模块特征基因计算结果
''' </summary>
Public Class ModuleEigengeneResult
    ''' <summary>
    ''' 模块名称
    ''' </summary>
    Public Property ModuleName As String

    ''' <summary>
    ''' 模块特征基因值（每个样本一个值）
    ''' </summary>
    Public Property Eigengene As Double()

    ''' <summary>
    ''' 第一主成分解释的方差比例
    ''' </summary>
    Public Property VarianceExplained As Double

    ''' <summary>
    ''' 模块内基因数量
    ''' </summary>
    Public Property GeneCount As Integer
End Class

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

''' <summary>
''' 模块成员结果
''' </summary>
Public Class ModuleMembershipResult
    ''' <summary>
    ''' 基因ID
    ''' </summary>
    Public Property GeneId As String

    ''' <summary>
    ''' 模块名称
    ''' </summary>
    Public Property ModuleName As String

    ''' <summary>
    ''' 相关系数（模块成员MM）
    ''' </summary>
    Public Property Correlation As Double

    ''' <summary>
    ''' p值
    ''' </summary>
    Public Property PValue As Double

    ''' <summary>
    ''' 样本数量
    ''' </summary>
    Public Property SampleCount As Integer

    Public Overrides Function ToString() As String
        Return $"Gene '{GeneId}' in '{ModuleName}': MM={Correlation:F3}, p={PValue:F4}"
    End Function
End Class
