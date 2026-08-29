Imports std = System.Math
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.GEARS.Graph

Namespace Training

    ''' <summary>
    ''' 虚拟扰动的计算机仿真器：基于先验调控网络生成伪 Perturb-seq 训练标签
    ''' </summary>
    ''' <remarks>
    ''' 真实的 GEARS 使用 CROP-seq / Perturb-seq 实测数据做监督训练；当没有实测扰动数据时，
    ''' 本仿真器沿先验调控网络做带衰减的多跳信号传播，合成出「扰动 → 全转录组响应」的标签，
    ''' 使 GNN 可以先行拟合出「扰动沿网络级联传播」的一般规律，再泛化到未见过的扰动。
    '''
    ''' 单基因扰动的生成过程：
    ''' <list type="number">
    ''' <item><description>直接效应：由 <see cref="InterventionSpec.GetInterventionValue"/> 给出被扰动基因的干预值；</description></item>
    ''' <item><description>级联效应：沿 TF → Target 方向做多跳扩散，
    ''' 每一跳乘以衰减系数 <c>decay</c>，并按入度归一化、按边类型取符号（抑制边取反）；</description></item>
    ''' <item><description>饱和与噪声：用 tanh 做软饱和防止多跳叠加爆量，并叠加少量高斯噪声模拟生物变异。</description></item>
    ''' </list>
    '''
    ''' 组合扰动额外引入非加性效应：若某个基因是多个被扰动基因的共同下游，
    ''' 其响应会被协同放大（乘性增强），这正是 GEARS 相对传统「效应线性叠加」方法的核心优势。
    ''' </remarks>
    Public Class InSilicoPerturbationSimulator

        ''' <summary>基因调控图（提供稀疏入边缓存与边类型符号）</summary>
        ReadOnly graphData As GeneRegulatoryGraph

        ''' <summary>control 条件下每个基因的表达均值</summary>
        ReadOnly controlMean As Double()

        ''' <summary>control 条件下每个基因的表达标准差</summary>
        ReadOnly controlSD As Double()

        ''' <summary>每一跳的信号衰减系数</summary>
        ReadOnly decay As Double

        ''' <summary>最大传播跳数</summary>
        ReadOnly maxHops As Integer

        ''' <summary>组合扰动的协同放大系数</summary>
        ReadOnly synergyStrength As Double

        ''' <summary>标签噪声水平（以基因标准差为单位）</summary>
        ReadOnly noiseLevel As Double

        ''' <summary>随机数发生器</summary>
        ReadOnly rand As Random

        ''' <summary>信号传播的贡献阈值，低于该值的下游基因不参与「共同下游」判定</summary>
        Const ContributionEpsilon As Double = 0.001

        ''' <summary>
        ''' 创建虚拟扰动仿真器
        ''' </summary>
        ''' <param name="graph">基因调控图</param>
        ''' <param name="controlMean">control 表达均值 [numGenes]</param>
        ''' <param name="controlSD">control 表达标准差 [numGenes]（用于饱和上限与噪声尺度）</param>
        ''' <param name="decay">每跳衰减系数，取值建议在 0.3~0.8 之间</param>
        ''' <param name="maxHops">最大传播跳数，通常取 3~4</param>
        ''' <param name="synergyStrength">组合扰动的协同放大系数，0 表示退化为线性叠加</param>
        ''' <param name="noiseLevel">标签噪声水平（以基因为单位的标准差倍数）</param>
        ''' <param name="seed">随机种子；给定后仿真结果可复现</param>
        Public Sub New(graph As GeneRegulatoryGraph,
                       controlMean As Double(),
                       controlSD As Double(),
                       Optional decay As Double = 0.6,
                       Optional maxHops As Integer = 3,
                       Optional synergyStrength As Double = 0.35,
                       Optional noiseLevel As Double = 0.02,
                       Optional seed As Integer = 2024)

            Me.graphData = graph
            Me.controlMean = controlMean
            Me.controlSD = controlSD
            Me.decay = decay
            Me.maxHops = maxHops
            Me.synergyStrength = synergyStrength
            Me.noiseLevel = noiseLevel
            Me.rand = New Random(seed)
        End Sub

        ''' <summary>
        ''' 仿真一次（组合）扰动
        ''' </summary>
        ''' <param name="specs">干预定义集合；单个元素为单基因扰动，多个元素为组合扰动</param>
        ''' <returns>合成出的训练样本；若所有目标基因都不在图中则返回 Nothing</returns>
        Public Function Simulate(specs As IEnumerable(Of InterventionSpec)) As PerturbSeqSample
            Dim specList As List(Of InterventionSpec) = specs.SafeQuery.ToList()

            If specList.Count = 0 Then
                Return Nothing
            End If

            Dim n As Integer = graphData.NumGenes
            Dim indices As New List(Of Integer)()
            Dim names As New List(Of String)()
            Dim direct As New List(Of Double)()
            Dim inputExpr As Double() = CType(controlMean.Clone(), Double())

            For Each spec As InterventionSpec In specList
                Dim idx As Integer = -1

                If Not graphData.TryGetGeneIndex(spec.GeneName, idx) Then
                    Continue For
                End If
                If indices.Contains(idx) Then
                    Continue For
                End If

                Dim target As Double = spec.GetInterventionValue(controlMean(idx), controlSD(idx))

                indices.Add(idx)
                names.Add(spec.GeneName)
                direct.Add(target - controlMean(idx))
                inputExpr(idx) = target
            Next

            If indices.Count = 0 Then
                Return Nothing
            End If

            ' ---- 逐个被扰动基因做多跳扩散，分别记录贡献以便识别共同下游 ----
            Dim contributions As List(Of Double()) = New List(Of Double())()

            For k As Integer = 0 To indices.Count - 1
                contributions.Add(Diffuse(indices(k), direct(k)))
            Next

            Dim perturbedSet As New HashSet(Of Integer)(indices)
            Dim delta As Double() = New Double(n - 1) {}
            Dim hits As Integer() = New Integer(n - 1) {}

            For j As Integer = 0 To n - 1
                If perturbedSet.Contains(j) Then
                    Continue For
                End If

                Dim sum As Double = 0

                For k As Integer = 0 To contributions.Count - 1
                    Dim c As Double = contributions(k)(j)

                    sum += c

                    If std.Abs(c) > ContributionEpsilon Then
                        hits(j) += 1
                    End If
                Next

                ' 组合扰动的非加性协同：共同下游基因被多个扰动同时命中时放大响应
                If hits(j) >= 2 Then
                    sum *= 1.0 + synergyStrength * (hits(j) - 1)
                End If

                delta(j) = sum
            Next

            ' ---- 软饱和 + 噪声 ----
            Dim perturbedExpr As Double() = New Double(n - 1) {}

            For j As Integer = 0 To n - 1
                If perturbedSet.Contains(j) Then
                    perturbedExpr(j) = inputExpr(j)
                    Continue For
                End If

                Dim cap As Double = 3.0 * std.Max(controlSD(j), 1E-6)
                Dim soft As Double = cap * std.Tanh(delta(j) / cap)
                Dim noisy As Double = soft + noiseLevel * controlSD(j) * NextGaussian()

                delta(j) = noisy
                perturbedExpr(j) = std.Max(0.0, inputExpr(j) + noisy)
            Next

            Dim label As String = String.Join("+", names) & "_" & specList(0).Mode.ToString()

            Return New PerturbSeqSample With {
                .PerturbedGeneIndices = indices.ToArray(),
                .PerturbedGeneNames = names.ToArray(),
                .ControlExpression = inputExpr,
                .PerturbedExpression = perturbedExpr,
                .Label = label,
                .Mode = specList(0).Mode
            }
        End Function

        ''' <summary>
        ''' 从单个被扰动基因出发，沿调控方向做多跳扩散，返回每个基因收到的累计贡献
        ''' </summary>
        ''' <param name="source">被扰动基因的节点索引</param>
        ''' <param name="directDelta">该基因的直接表达变化量</param>
        ''' <returns>每个基因从本次扰动中收到的累计贡献 [numGenes]</returns>
        Private Function Diffuse(source As Integer, directDelta As Double) As Double()
            Dim n As Integer = graphData.NumGenes
            Dim signs As Double() = EdgeRelationTypes.SignTable()
            Dim accumulated As Double() = New Double(n - 1) {}
            Dim signal As Double() = New Double(n - 1) {}

            signal(source) = directDelta

            For hop As Integer = 1 To maxHops
                Dim nextSignal As Double() = New Double(n - 1) {}
                Dim attenuation As Double = std.Pow(decay, hop)

                For j As Integer = 0 To n - 1
                    Dim sources As Integer() = graphData.InEdgeSources(j)

                    If sources.Length = 0 Then
                        Continue For
                    End If

                    Dim types As Integer() = graphData.InEdgeTypes(j)
                    Dim weights As Double() = graphData.InEdgeWeights(j)
                    Dim sum As Double = 0

                    For e As Integer = 0 To sources.Length - 1
                        sum += weights(e) * signs(types(e)) * signal(sources(e))
                    Next

                    nextSignal(j) = attenuation * sum
                    accumulated(j) += nextSignal(j)
                Next

                ' 被扰动基因自身不接受回传，避免信号在网络里自激放大
                nextSignal(source) = 0.0
                signal = nextSignal
            Next

            Return accumulated
        End Function

        ''' <summary>
        ''' 生成标准正态分布随机数（Box-Muller 变换）
        ''' </summary>
        ''' <returns>标准正态随机样本</returns>
        Private Function NextGaussian() As Double
            Dim u1 As Double = 1.0 - rand.NextDouble()
            Dim u2 As Double = 1.0 - rand.NextDouble()

            Return std.Sqrt(-2.0 * std.Log(u1)) * std.Sin(2.0 * std.PI * u2)
        End Function
    End Class
End Namespace
