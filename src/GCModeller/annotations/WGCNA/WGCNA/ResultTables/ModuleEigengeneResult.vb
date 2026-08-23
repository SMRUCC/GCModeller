
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