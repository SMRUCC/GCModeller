Imports rng = Microsoft.VisualBasic.Math.RandomExtensions

' ============================================================
' BnCPD.vb - 条件概率分布（CPD）参数
' ============================================================
' 针对基因表达调控网络，采用高斯贝叶斯网络（GBN）模型：
'   Xi | Pa(Xi) ~ N(β0 + Σ βj·Xj, σ²)
' 其中 Pa(Xi) 是 Xi 的父节点集合，βj 是回归系数，σ² 是残差方差
' ============================================================

Namespace Core

    ''' <summary>
    ''' 高斯条件概率分布参数
    ''' Xi | Pa(Xi) ~ N(β0 + β1·Pa1 + β2·Pa2 + ... + βk·Pak, σ²)
    ''' </summary>
    Public Class BnCPD

        ''' <summary>所属节点索引</summary>
        Public Property NodeIndex As Integer

        ''' <summary>截距 β0</summary>
        Public Property Intercept As Double = 0.0

        ''' <summary>回归系数 βj，对应每个父节点</summary>
        ''' <remarks>Coeffs(j) 对应 Parents(j) 的回归系数</remarks>
        Public Property Coeffs As Double() = New Double() {}

        ''' <summary>父节点索引列表（与 Coeffs 一一对应）</summary>
        Public Property ParentIndices As Integer() = New Integer() {}

        ''' <summary>残差标准差 σ</summary>
        Public Property ResidualSD As Double = 1.0

        ''' <summary>残差方差 σ²</summary>
        Public Property ResidualVariance As Double = 1.0

        ''' <summary>拟合 R²</summary>
        Public Property RSquared As Double = 0.0

        ''' <summary>BIC 评分</summary>
        Public Property BIC As Double = 0.0

        ''' <summary>样本数</summary>
        Public Property NSamples As Integer = 0

        Public Overridable Function Clone() As BnCPD
            Return New BnCPD With {
                .NodeIndex = NodeIndex,
                .Intercept = Intercept,
                .Coeffs = Coeffs.ToArray,
                .BIC = BIC,
                .NSamples = NSamples,
                .ParentIndices = ParentIndices.ToArray,
                .ResidualSD = ResidualSD,
                .ResidualVariance = ResidualVariance,
                .RSquared = RSquared
            }
        End Function

        ''' <summary>
        ''' 根据父节点值计算条件均值
        ''' E[Xi | Pa(Xi)] = β0 + Σ βj·Pa_j
        ''' </summary>
        Public Overridable Function ConditionalMean(parentValues As Double()) As Double
            Dim mean As Double = Intercept
            For j = 0 To Coeffs.Length - 1
                If j < parentValues.Length Then
                    mean += Coeffs(j) * parentValues(j)
                End If
            Next
            Return mean
        End Function

        ''' <summary>
        ''' 从条件分布中采样
        ''' Xi ~ N(β0 + Σ βj·Pa_j, σ²)
        ''' </summary>
        Public Overridable Function Sample(parentValues As Double()) As Double
            Dim mean As Double = ConditionalMean(parentValues)
            ' Box-Muller 变换生成正态随机数
            Dim u1 As Double = rng.NextDouble()
            Dim u2 As Double = rng.NextDouble()
            If u1 < 0.000000000000001 Then u1 = 0.000000000000001
            Dim z As Double = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2)
            Return mean + ResidualSD * z
        End Function

        ''' <summary>
        ''' 计算条件概率密度
        ''' f(Xi | Pa(Xi)) = (1/√(2πσ²))·exp(-(Xi-μ)²/(2σ²))
        ''' </summary>
        Public Overridable Function LogDensity(x As Double, parentValues As Double()) As Double
            Dim mean As Double = ConditionalMean(parentValues)
            Dim diff As Double = x - mean
            Dim logPdf As Double = -0.5 * Math.Log(2.0 * Math.PI * ResidualVariance) - diff * diff / (2.0 * ResidualVariance)
            Return logPdf
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendFormat("Node {0}: β0={1:F4}", NodeIndex, Intercept)
            For j = 0 To Coeffs.Length - 1
                sb.AppendFormat(", β_{0}={1:F4}", ParentIndices(j), Coeffs(j))
            Next
            sb.AppendFormat(", σ={0:F4}, R²={1:F4}", ResidualSD, RSquared)
            Return sb.ToString()
        End Function
    End Class

    ''' <summary>
    ''' 被干预节点的 CPD —— 用于虚拟敲除/过表达
    ''' 将节点值固定为常数，忽略所有父节点
    ''' </summary>
    Public Class InterventionCPD
        Inherits BnCPD

        ''' <summary>干预类型</summary>
        Public Property InterventionType As InterventionMode

        ''' <summary>干预值（敲除=0，过表达=指定值）</summary>
        Public Property InterventionValue As Double = 0.0

        Public Enum InterventionMode
            ''' <summary>基因敲除（表达量设为0）</summary>
            Knockout
            ''' <summary>基因过表达（表达量设为指定高值）</summary>
            Overexpression
        End Enum

        Public Sub New(originalCPD As BnCPD, mode As InterventionMode, value As Double)
            Me.NodeIndex = originalCPD.NodeIndex
            Me.ParentIndices = originalCPD.ParentIndices
            Me.Coeffs = New Double(originalCPD.Coeffs.Length - 1) {}
            Me.InterventionType = mode
            Me.InterventionValue = value
            Me.Intercept = value
            Me.ResidualSD = 0.0
            Me.ResidualVariance = 0.0
            Me.RSquared = 1.0
        End Sub

        Private Sub New()
        End Sub

        Public Overrides Function Clone() As BnCPD
            Return New InterventionCPD With {
                .BIC = BIC,
                .ParentIndices = ParentIndices.ToArray,
                .Coeffs = Coeffs.ToArray,
                .Intercept = Intercept,
                .InterventionType = InterventionType,
                .InterventionValue = InterventionValue,
                .NodeIndex = NodeIndex,
                .NSamples = NSamples,
                .ResidualSD = ResidualSD,
                .ResidualVariance = ResidualVariance,
                .RSquared = RSquared
            }
        End Function

        ''' <summary>干预后：直接返回固定值</summary>
        Public Overrides Function ConditionalMean(parentValues As Double()) As Double
            Return InterventionValue
        End Function

        Public Overrides Function Sample(parentValues As Double()) As Double
            Return InterventionValue
        End Function

        Public Overrides Function LogDensity(x As Double, parentValues As Double()) As Double
            If Math.Abs(x - InterventionValue) < 1e-10 Then Return 0.0
            Return Double.NegativeInfinity
        End Function

    End Class

End Namespace
