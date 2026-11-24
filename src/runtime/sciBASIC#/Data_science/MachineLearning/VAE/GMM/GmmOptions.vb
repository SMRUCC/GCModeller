' ============================================================================
' GmmOptions.vb - 高斯混合模型的扩展选项
'
' 既有的构造签名
'     New(nComponents, maxIter, tol, regCovar, diagCovariance, initMethod, seed)
' 保持完全不变；这里提供的是**新增**的选项类与枚举，通过新增构造重载暴露，
' 因此不会破坏任何既有调用。
' ============================================================================

''' <summary>
''' 高斯混合模型的协方差结构类型。
''' </summary>
Public Enum GmmCovarianceType
    ''' <summary>每个成分一个完整的协方差矩阵（参数量最大，最灵活）。</summary>
    Full = 0
    ''' <summary>每个成分一个对角协方差（高维数据的推荐默认值）。</summary>
    Diagonal = 1
    ''' <summary>所有成分共享同一个协方差矩阵。</summary>
    Tied = 2
    ''' <summary>所有成分共享同一个标量方差。</summary>
    Spherical = 3
End Enum

''' <summary>
''' 高斯混合模型的扩展选项。
''' </summary>
Public Class GmmOptions

    ''' <summary>协方差结构类型。</summary>
    Public Property CovarianceType As GmmCovarianceType = GmmCovarianceType.Full

    ''' <summary>混合权重下限，避免权重塌缩到 0 之后 log(0)。</summary>
    Public Property WeightFloor As Double = 0.00000001

    ''' <summary>协方差对角线的最小值。</summary>
    Public Property CovarianceFloor As Double = 0.0001

    ''' <summary>有效样本数低于该阈值的成分被视为空簇并重新初始化。</summary>
    Public Property EmptyClusterSize As Double = 1.0

    ''' <summary>是否启用空簇重初始化。</summary>
    Public Property ReinitializeEmptyClusters As Boolean = True

    Public Sub New()
    End Sub

    Public Sub New(covarianceType As GmmCovarianceType)
        Me.CovarianceType = covarianceType
    End Sub

    ''' <summary>
    ''' 转换为 <see cref="Core.ModelSelection.GaussianMixtureParameters"/> 使用的协方差类别编号。
    ''' </summary>
    Public Function ToCovarianceKind() As Integer
        Return CInt(CovarianceType)
    End Function

    ''' <summary>由旧的布尔开关构造选项（保持旧语义）。</summary>
    Public Shared Function FromDiagonalFlag(diagCovariance As Boolean) As GmmOptions
        Return New GmmOptions(If(diagCovariance, GmmCovarianceType.Diagonal, GmmCovarianceType.Full))
    End Function

    Public Overrides Function ToString() As String
        Return $"GmmOptions(covariance={CovarianceType}, weightFloor={WeightFloor}, emptyCluster={EmptyClusterSize})"
    End Function

End Class
