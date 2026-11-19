Imports System
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.FuzzyGene.Fitting
Imports std = System.Math

Namespace Solver

    ''' <summary>
    ''' 一次全局增益校准的结果（写进训练报告与网络元数据）。
    ''' </summary>
    Public Class CalibrationResult

        ''' <summary>实际采用的校准方式。</summary>
        Public Property Mode As StabilityCalibrationMode

        ''' <summary>校准前的谱半径。</summary>
        Public Property SpectralRadius As Double

        ''' <summary>校准后的谱半径（应当 ≤ <c>1 - Epsilon</c>）。</summary>
        Public Property PostSpectralRadius As Double

        ''' <summary>全局缩放的累计系数（<c>1</c> 表示没有改动）。</summary>
        Public Property Gamma As Double

        ''' <summary>校准前逐行 L1 范数的最大值（无穷范数）。</summary>
        Public Property RowNormMax As Double

        ''' <summary>被缩放的基因数。</summary>
        Public Property ScaledGenes As Integer

        ''' <summary>是否真的改动了模型。</summary>
        Public ReadOnly Property Applied As Boolean
            Get
                Return Gamma < 1.0 - 1.0E-12
            End Get
        End Property

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"{Mode}: rho {SpectralRadius:G4} -> {PostSpectralRadius:G4}, gamma={Gamma:G4}, " &
                   $"row-norm {RowNormMax:G4}, scaled {ScaledGenes} gene(s)"
        End Function

    End Class

    ''' <summary>
    ''' 拟合完成后的全局稳定性校准：把 <c>J</c> 的斜率整体缩放，使 <c>ρ(J) ≤ 1 - ε</c>。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么必须做</b>：逐靶基因独立拟合只保证「每个基因在训练数据上拟合得好」，
    ''' 完全不管把这些模型拼成网络之后的谱半径。而固定点迭代的收敛性只由谱半径决定：
    ''' <c>ρ ≥ 1</c> 时阻尼迭代不收缩（<c>ρ ≈ 1</c> 时甚至是中立方向，几百步都走不动）。
    ''' 校准不改变符号、规则结构与相对强弱，只把幅度压进收敛域——
    ''' 这是「让模型在自己的方程上可解」的最小改动。
    ''' </para>
    ''' <para>
    ''' <b>为什么用多轮「测—缩」而不是一次性算 γ</b>：幂迭代对非对称矩阵只给估计值
    ''' （复特征值会让末几位振荡），一次缩放未必达标。这里每轮重新估计，
    ''' 最多三轮，既自校正又不会无限循环。最后再用逐行 L1 做防御性兜底：
    ''' <c>ρ(J) ≤ ‖J‖∞</c> 是严格上界，把每行压到 <c>≤ 1-ε</c> 就能保证谱半径达标。
    ''' </para>
    ''' <para>
    ''' 缩放只作用于结论参数（一阶 Sugeno 的斜率列），截距不动：
    ''' 因此规则文本仍然可读（系数整体变小），基因间的相对关系也保留。
    ''' </para>
    ''' </remarks>
    Public Module GainCalibration

        ''' <summary>校准的目标余量：<c>ρ ≤ 1 - ε</c>。</summary>
        Private Const MaxPasses As Integer = 3

        ''' <summary>
        ''' 就地校准网络的结论参数。
        ''' </summary>
        ''' <param name="network">已拟合的网络（<c>theta</c> 会被就地缩放）</param>
        ''' <param name="config">建模配置（读取 <see cref="HybridOptions.Calibration"/>）</param>
        ''' <returns>校准结果；未启用时返回 <c>Nothing</c></returns>
        ''' <remarks>
        ''' 调用方必须在校准之后再做任何 R² 评估，否则报告的是校准前的精度。
        ''' </remarks>
        Public Function Apply(network As FuzzyGeneNetwork, config As FuzzyGeneConfig) As CalibrationResult
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            Dim options As HybridOptions = If(config, New FuzzyGeneConfig()).Hybrid

            If options.Calibration = StabilityCalibrationMode.None Then
                Return Nothing
            End If

            Dim z As Double() = network.ToZ(network.BaselineRaw)
            Dim result As New CalibrationResult With {
                .Mode = options.Calibration,
                .Gamma = 1.0
            }

            Dim target As Double = 1.0 - options.CalibrationEpsilon

            For pass As Integer = 1 To MaxPasses
                Dim rho As Double = Estimate(network, z, options)

                If pass = 1 Then
                    result.SpectralRadius = rho
                    result.RowNormMax = RowNormMax(network, z)
                End If

                If rho <= target Then
                    Exit For
                End If

                Dim scale As Double = target / rho

                ' 极端情况下（rho 巨大或估计失真）也不要把模型压成 0
                scale = std.Max(scale, options.CalibrationMinScale)

                Call ScaleAll(network, scale)

                result.Gamma *= scale
            Next

            ' 防御性兜底：逐行 L1 上界（ρ ≤ ‖J‖∞）
            result.ScaledGenes = ScaleRows(network, z, target, options)
            result.PostSpectralRadius = Estimate(network, z, options)

            Return result
        End Function

        ''' <summary>
        ''' 估计当前模型的谱半径。
        ''' </summary>
        ''' <remarks>
        ''' 幂迭代给出估计值；当估计值退化（全零或明显小于行范数一个量级）时回落到
        ''' 行 L1 上界，宁可保守也不要漏校准。
        ''' </remarks>
        Public Function Estimate(network As FuzzyGeneNetwork, Optional z As Double() = Nothing,
                                 Optional options As HybridOptions = Nothing) As Double
            Dim state As Double() = If(z, network.ToZ(network.BaselineRaw))
            Dim jacobian As New JacobianOperator(network)

            Call jacobian.Build(state, 1)

            Dim bound As Double = jacobian.InfinityNorm()

            If options Is Nothing OrElse options.Calibration = StabilityCalibrationMode.RowL1 Then
                Return bound
            End If

            Dim iterations As Integer = network.Config.Solver.SpectralPowerIterations
            Dim rho As Double = jacobian.SpectralRadius(iterations)

            ' 幂迭代在复特征值/强非对称下可能收敛到很小的值；差距超过一个量级就信上界
            If rho <= 0.0 OrElse (bound > 0.0 AndAlso rho < bound * 0.1) Then
                Return bound
            End If

            Return rho
        End Function

        ''' <summary>逐行 L1 范数的最大值。</summary>
        Private Function RowNormMax(network As FuzzyGeneNetwork, z As Double()) As Double
            Dim jacobian As New JacobianOperator(network)

            Call jacobian.Build(z, 1)

            Return jacobian.InfinityNorm()
        End Function

        ''' <summary>把全部基因的结论斜率乘以同一个系数。</summary>
        Private Sub ScaleAll(network As FuzzyGeneNetwork, scale As Double)
            For Each model As GeneRegulationModel In network.Models
                If model Is Nothing OrElse model.Network Is Nothing Then
                    Continue For
                End If

                Call ScaleSlopes(model, scale)

                model.GainScale *= scale
            Next
        End Sub

        ''' <summary>
        ''' 对仍然超限的行做逐基因再缩放（把每行 L1 压到 <c>≤ target</c>）。
        ''' </summary>
        ''' <returns>被再次缩放的基因数</returns>
        Private Function ScaleRows(network As FuzzyGeneNetwork, z As Double(),
                                   target As Double, options As HybridOptions) As Integer
            Dim jacobian As New JacobianOperator(network)

            Call jacobian.Build(z, 1)

            Dim norms As Double() = jacobian.RowNorms()
            Dim scaled As Integer = 0

            For g As Integer = 0 To network.NGene - 1
                Dim model As GeneRegulationModel = network.Models(g)

                If model Is Nothing OrElse model.Network Is Nothing Then
                    Continue For
                End If
                If norms(g) <= target OrElse norms(g) <= 0.0 Then
                    Continue For
                End If

                Dim scale As Double = std.Max(target / norms(g), options.CalibrationMinScale)

                Call ScaleSlopes(model, scale)

                model.GainScale *= scale
                scaled += 1
            Next

            Return scaled
        End Function

        ''' <summary>缩放单个模型的一阶结论斜率（截距不动）。</summary>
        Private Sub ScaleSlopes(model As GeneRegulationModel, scale As Double)
            Dim theta As Double(,) = model.Network.Theta
            Dim inputs As Integer = model.Network.InputCount
            Dim rules As Integer = model.Network.Rules.RuleCount

            For r As Integer = 0 To rules - 1
                Dim offset As Integer = r * (inputs + 1)

                For j As Integer = 0 To inputs - 1
                    theta(offset + j, 0) *= scale
                Next
            Next
        End Sub

    End Module

End Namespace
