Namespace Core

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
            If Math.Abs(x - InterventionValue) < 0.0000000001 Then Return 0.0
            Return Double.NegativeInfinity
        End Function

    End Class

End Namespace