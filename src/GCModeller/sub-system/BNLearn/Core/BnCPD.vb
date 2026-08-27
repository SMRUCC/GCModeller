#Region "Microsoft.VisualBasic::1a8b83d2a9ef2dc2ae16476362a953e6, sub-system\BNLearn\Core\BnCPD.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 178
    '    Code Lines: 106 (59.55%)
    ' Comment Lines: 43 (24.16%)
    '    - Xml Docs: 81.40%
    ' 
    '   Blank Lines: 29 (16.29%)
    '     File Size: 6.90 KB


    '     Class BnCPD
    ' 
    '         Properties: BIC, Coeffs, Intercept, NodeIndex, NSamples
    '                     ParentIndices, ResidualSD, ResidualVariance, RSquared
    ' 
    '         Function: Clone, ConditionalMean, LogDensity, Sample, ToString
    ' 
    '     Class InterventionCPD
    ' 
    '         Properties: InterventionType, InterventionValue
    '         Enum InterventionMode
    ' 
    '             Knockout, Overexpression
    ' 
    ' 
    ' 
    '  
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: Clone, ConditionalMean, LogDensity, Sample
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
End Namespace

