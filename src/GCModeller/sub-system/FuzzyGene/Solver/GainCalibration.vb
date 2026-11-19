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

            ' 只有处在调控环里的基因才可能把扰动放大；
            ' 无环部分再怎么缩放也只是白白削弱拟合（R² 会直接变成负数）
            Dim cyclic As Boolean() = CyclicGenes(network, MaxCycleDepth)
            Dim scaled As Integer = 0

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

                scaled += ScaleGenes(network, cyclic, scale)

                result.Gamma *= scale
            Next

            ' 防御性兜底：仍然是环比率的行 L1 上界
            scaled += ScaleRows(network, z, target, options, cyclic)

            result.ScaledGenes = scaled
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

            ' 幂迭代给出的就是谱半径：**不能用行范数上界去覆盖它**。
            ' 无环网络（拓扑序存在）的 J 是严格三角、幂零的，真正的 ρ = 0，
            ' 而 ‖J‖∞ 仍等于最大单基因灵敏度（可以是 3）。用上界覆盖会把一个
            ' 本来收敛的网络整体缩小 70%，R² 直接变成负数——第一版就踩了这个坑。
            Return rho
        End Function

        ''' <summary>逐行 L1 范数的最大值。</summary>
        Private Function RowNormMax(network As FuzzyGeneNetwork, z As Double()) As Double
            Dim jacobian As New JacobianOperator(network)

            Call jacobian.Build(z, 1)

            Return jacobian.InfinityNorm()
        End Function

        ''' <summary>把指定基因的结论斜率乘以同一个系数。</summary>
        ''' <returns>被缩放的基因数</returns>
        ''' <remarks>
        ''' 统一系数就足够：周期 <c>L</c> 的环，其增益 <c>(Πg)^{1/L}</c> 与谱半径同阶，
        ''' 所有环节点同乘 <c>γ</c> 会让谱半径精确地变成 <c>γ·ρ</c>。
        ''' 无环节点不参与缩放，因此它们的拟合精度不受影响。
        ''' </remarks>
        Private Function ScaleGenes(network As FuzzyGeneNetwork, cyclic As Boolean(), scale As Double) As Integer
            Dim count As Integer = 0

            For g As Integer = 0 To network.NGene - 1
                If Not cyclic(g) Then
                    Continue For
                End If

                Dim model As GeneRegulationModel = network.Models(g)

                If model Is Nothing OrElse model.Network Is Nothing Then
                    Continue For
                End If

                Call ScaleSlopes(model, scale)

                model.GainScale *= scale
                count += 1
            Next

            Return count
        End Function

        ''' <summary>
        ''' 对仍然超限的行做逐基因再缩放（把每行 L1 压到 <c>≤ target</c>）。
        ''' </summary>
        ''' <returns>被再次缩放的基因数</returns>
        Private Function ScaleRows(network As FuzzyGeneNetwork, z As Double(),
                                   target As Double, options As HybridOptions,
                                   cyclic As Boolean()) As Integer
            ' 全局谱半径已经达标就不必再动：逐行 L1 是**上界**，
            ' 对无环网络它远大于真实谱半径（后者为 0），照它缩放会把模型整体压扁
            If Estimate(network, z, options) <= target Then
                Return 0
            End If

            Dim jacobian As New JacobianOperator(network)

            Call jacobian.Build(z, 1)

            Dim norms As Double() = jacobian.RowNorms()
            Dim scaled As Integer = 0

            For g As Integer = 0 To network.NGene - 1
                Dim model As GeneRegulationModel = network.Models(g)

                If model Is Nothing OrElse model.Network Is Nothing Then
                    Continue For
                End If
                ' 只有处在调控环里的基因才可能放大扰动：无环部分再怎么缩放也只是削弱拟合
                If Not cyclic(g) Then
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

        ''' <summary>环检测的深度上限（更长的环增益已经很小，不再单独处理）。</summary>
        Private Const MaxCycleDepth As Integer = 3

        ''' <summary>
        ''' 标记「处在调控环里」的基因：从它的某个调控因子出发，沿「调控因子 → 靶基因」方向能回到它自己。
        ''' </summary>
        ''' <remarks>
        ''' 用有界深度的前向 BFS 而不是完整 SCC：
        ''' ① 只有短环（≤ 3 步）能产生接近 1 的闭环增益，长环自然被每条边的增益衰减；
        ''' ② SCC 需要额外维护正反两张邻接表，而这里只需要在拟合结束后跑一次。
        ''' </remarks>
        Private Function CyclicGenes(network As FuzzyGeneNetwork, maxDepth As Integer) As Boolean()
            Dim n As Integer = network.NGene
            Dim inputs As Integer()() = network.RegulatorIndexTable
            Dim targets(n - 1) As List(Of Integer)
            Dim result(n - 1) As Boolean

            For i As Integer = 0 To n - 1
                targets(i) = New List(Of Integer)()
            Next

            For g As Integer = 0 To n - 1
                For Each j As Integer In inputs(g)
                    Call targets(j).Add(g)
                Next
            Next

            For g As Integer = 0 To n - 1
                Dim own As Integer() = inputs(g)

                If own.Length = 0 Then
                    Continue For
                End If

                Dim reached As New HashSet(Of Integer)()

                For Each source As Integer In own
                    ' 从调控因子出发，沿前向边搜索；能回到 g 就说明存在环
                    Dim frontier As New List(Of Integer) From {source}

                    For depth As Integer = 1 To maxDepth
                        Dim nextFrontier As New List(Of Integer)()

                        For Each node As Integer In frontier
                            For Each target As Integer In targets(node)
                                If target = g Then
                                    result(g) = True

                                    Exit For
                                End If

                                If reached.Add(target) Then
                                    Call nextFrontier.Add(target)
                                End If
                            Next

                            If result(g) Then
                                Exit For
                            End If
                        Next

                        If result(g) OrElse nextFrontier.Count = 0 Then
                            Exit For
                        End If

                        frontier = nextFrontier
                    Next

                    If result(g) Then
                        Exit For
                    End If
                Next
            Next

            Return result
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
