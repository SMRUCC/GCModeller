Imports System.Linq
Imports std = System.Math
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.GEARS.Graph
Imports SMRUCC.genomics.Analysis.GEARS.Model
Imports SMRUCC.genomics.Analysis.GEARS.Training
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' GEARS：基于图神经网络的基因表达调控网络虚拟扰动实验
''' </summary>
''' <remarks>
''' 本类是整个算法的门面（facade），把 readme 中的五个步骤串联为一条完整的实验流水线：
'''
''' <list type="number">
''' <item><description><strong>Step 1</strong>：由先验调控网络 <see cref="PriorNetwork"/> 构建基因-基因调控图（<see cref="GeneRegulatoryGraph"/>）；</description></item>
''' <item><description><strong>Step 2</strong>：以 control 基线表达 + 基因身份嵌入 + 扰动标记 + 扰动集合向量构成初始节点特征；</description></item>
''' <item><description><strong>Step 3</strong>：多层边类型感知消息传递，让扰动信号沿调控边向下游级联传播；</description></item>
''' <item><description><strong>Step 4</strong>：解码器输出 Δ表达，预测 <c>x̂^pert = x^input + Δx̂</c>；</description></item>
''' <item><description><strong>Step 5</strong>：用（仿真或实测的）Perturb-seq 样本做监督训练，之后即可泛化到未见过的扰动。</description></item>
''' </list>
'''
''' 使用方式：
''' <code>
''' Dim gears As New GEARS(exprData, priorNetwork)
''' Dim ko = gears.KnockoutGene("codY")
''' Dim combo = gears.PredictCombination({"codY", "luxR"}, InterventionMode.Knockout)
''' </code>
'''
''' 本类实现了 <see cref="InsilicoPerturbationExperiment"/> 接口，
''' 因此可以与 BNLearn 中已有的贝叶斯网络虚拟扰动实现互换使用。
''' </remarks>
Public Class GEARS : Implements InsilicoPerturbationExperiment

    ''' <summary>基因表达数据（行=基因，列=样本）</summary>
    ReadOnly exprData As GeneExpressionData

    ''' <summary>先验调控网络</summary>
    ReadOnly priorNetwork As PriorNetwork

    ''' <summary>超参配置</summary>
    ReadOnly config As GEARSConfig

    ''' <summary>随机数发生器</summary>
    ReadOnly rand As Random

    ''' <summary>用于估计 control 基线所选取的样本列索引</summary>
    ReadOnly baselineSamples As Integer()

    ''' <summary>基因调控图</summary>
    ''' <returns><see cref="GeneRegulatoryGraph"/> 实例</returns>
    Public ReadOnly Property GraphData As GeneRegulatoryGraph

    ''' <summary>GNN 模型</summary>
    ''' <returns><see cref="GEARSModel"/> 实例</returns>
    Public ReadOnly Property Model As GEARSModel

    ''' <summary>基因名称列表（顺序与表达矩阵行序一致）</summary>
    ''' <returns>基因名数组</returns>
    Public ReadOnly Property GeneNames As String()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return exprData.GeneNames
        End Get
    End Property

    ''' <summary>control（野生型）表达均值</summary>
    ''' <returns>每个基因的表达均值</returns>
    Public ReadOnly Property WildtypeMeans As Double()

    ''' <summary>control（野生型）表达标准差，用于归一化、Z-score 与显著性判定</summary>
    ''' <returns>每个基因的表达标准差</returns>
    Public ReadOnly Property WildtypeSDs As Double()

    ''' <summary>训练样本集合（默认由内置仿真器生成，可用实测数据覆盖）</summary>
    ''' <returns>训练样本列表</returns>
    Public ReadOnly Property TrainingSamples As New List(Of PerturbSeqSample)()

    ''' <summary>最近一次训练得到的损失曲线（每个 epoch 的平均 MSE）</summary>
    ''' <returns>损失数组；尚未训练时为空数组</returns>
    Public Property LossCurve As Double() = {}

    ''' <summary>训练器实例；尚未训练时为 Nothing</summary>
    ''' <returns><see cref="GEARSTrainer"/> 实例</returns>
    Public Property Trainer As GEARSTrainer

    ''' <summary>
    ''' 创建 GEARS 虚拟扰动实验
    ''' </summary>
    ''' <param name="expression">基因表达数据（行=基因，列=样本）</param>
    ''' <param name="prior">先验调控网络</param>
    ''' <param name="gearsConfig">超参配置；为 Nothing 时使用默认配置</param>
    ''' <param name="nSamples">
    ''' 用于估计 control 基线（均值/标准差）的样本数量；
    ''' 小于等于 0 或大于总样本数时使用全部样本
    ''' </param>
    Public Sub New(expression As GeneExpressionData,
                   prior As PriorNetwork,
                   Optional gearsConfig As GEARSConfig = Nothing,
                   Optional nSamples As Integer = 0)

        If expression Is Nothing Then
            Throw New ArgumentNullException(NameOf(expression))
        End If
        If prior Is Nothing Then
            Throw New ArgumentNullException(NameOf(prior))
        End If

        Me.exprData = expression
        Me.priorNetwork = prior
        Me.config = If(gearsConfig, New GEARSConfig())

        If Not Me.config.Validate() Then
            Throw New ArgumentException($"GEARS 配置参数不合法: {Me.config}")
        End If

        Me.rand = New Random(Me.config.Seed)
        Me.baselineSamples = SelectBaselineSamples(nSamples)
        Me.WildtypeMeans = New Double(expression.NGene - 1) {}
        Me.WildtypeSDs = New Double(expression.NGene - 1) {}

        Call ComputeBaseline()

        ' ---- Step 1: 构建基因-基因调控图 ----
        Me.GraphData = New GeneRegulatoryGraph(
            geneNames:=expression.GeneNames,
            prior:=prior,
            controlExpr:=expression.Matrix,
            coexpressionTopK:=Me.config.CoexpressionTopK,
            minCoexpression:=Me.config.MinCoexpression
        )

        ' ---- Step 2~4: 构建 GNN 模型 ----
        Me.Model = New GEARSModel(
            graph:=Me.GraphData,
            embeddingDim:=Me.config.EmbeddingDim,
            hiddenDim:=Me.config.HiddenDim,
            numLayers:=Me.config.NumLayers,
            activation:=Me.config.Activation,
            usePerRelationTransform:=Me.config.UsePerRelationTransform,
            useDense:=Me.config.UseDense,
            seed:=Me.config.Seed
        )
    End Sub

    ''' <summary>
    ''' 创建 GEARS 虚拟扰动实验（直接由表达矩阵构造）
    ''' </summary>
    ''' <param name="matrix">表达矩阵（行=基因，列=样本）</param>
    ''' <param name="prior">先验调控网络</param>
    ''' <param name="gearsConfig">超参配置；为 Nothing 时使用默认配置</param>
    ''' <param name="nSamples">用于估计 control 基线的样本数量；0 表示使用全部样本</param>
    Public Sub New(matrix As Matrix,
                   prior As PriorNetwork,
                   Optional gearsConfig As GEARSConfig = Nothing,
                   Optional nSamples As Integer = 0)

        Me.New(matrix.ReadGeneExpressionMatrix(), prior, gearsConfig, nSamples)
    End Sub

    ' ==================== 基线统计量 ====================

    ''' <summary>
    ''' 选取用于估计 control 基线的样本列索引
    ''' </summary>
    ''' <param name="nSamples">请求的样本数量</param>
    ''' <returns>样本列索引数组</returns>
    Private Function SelectBaselineSamples(nSamples As Integer) As Integer()
        Dim total As Integer = exprData.NSample

        If nSamples <= 0 OrElse nSamples >= total Then
            Dim all As Integer() = New Integer(total - 1) {}

            For i As Integer = 0 To total - 1
                all(i) = i
            Next

            Return all
        End If

        Dim picked As New HashSet(Of Integer)()

        While picked.Count < nSamples
            picked.Add(rand.Next(total))
        End While

        Return picked.OrderBy(Function(i) i).ToArray()
    End Function

    ''' <summary>
    ''' 用构造时选定的样本集合估计 control 基线
    ''' </summary>
    Private Sub ComputeBaseline()
        Call ComputeBaseline(baselineSamples)
    End Sub

    ''' <summary>
    ''' 用指定的样本集合估计 control 条件下每个基因的表达均值与标准差
    ''' </summary>
    ''' <param name="sampleIdx">参与统计的样本列索引</param>
    Private Sub ComputeBaseline(sampleIdx As Integer())
        Dim n As Integer = exprData.NGene
        Dim m As Integer = sampleIdx.Length
        Dim matrix As Double(,) = exprData.Matrix

        For i As Integer = 0 To n - 1
            Dim sum As Double = 0

            For Each j As Integer In sampleIdx
                sum += matrix(i, j)
            Next

            Dim mean As Double = sum / m
            Dim ss As Double = 0

            For Each j As Integer In sampleIdx
                Dim d As Double = matrix(i, j) - mean

                ss += d * d
            Next

            WildtypeMeans(i) = mean
            WildtypeSDs(i) = If(m > 1, std.Sqrt(ss / (m - 1)), 0.0)
        Next
    End Sub

    ' ==================== 训练样本生成 ====================

    ''' <summary>
    ''' 用内置仿真器生成伪 Perturb-seq 训练样本
    ''' </summary>
    ''' <returns>生成的样本数量</returns>
    ''' <remarks>
    ''' 优先挑选在先验网络中作为转录因子出现的基因（它们拥有下游靶基因，
    ''' 扰动效应更容易在网络中传播），数量不足时再用其余基因补齐。
    ''' </remarks>
    Public Function GenerateTrainingSamples() As Integer
        Dim simulator As New InSilicoPerturbationSimulator(
            graph:=GraphData,
            controlMean:=WildtypeMeans,
            controlSD:=WildtypeSDs,
            decay:=config.PropagationDecay,
            maxHops:=config.MaxHops,
            synergyStrength:=config.SynergyStrength,
            noiseLevel:=config.SimulatorNoise,
            seed:=config.Seed
        )

        Dim candidates As List(Of String) = BuildPerturbationCandidates()

        If candidates.Count = 0 Then
            Return 0
        End If

        TrainingSamples.Clear()

        ' ---- 单基因扰动：轮流使用敲除 / 下调 / 过表达三种模式 ----
        Dim modes As InterventionMode() = {
            InterventionMode.Knockout,
            InterventionMode.Knockdown,
            InterventionMode.Overexpression
        }

        For i As Integer = 0 To config.NSinglePerturbation - 1
            Dim gene As String = candidates(i Mod candidates.Count)
            Dim mode As InterventionMode = modes(i Mod modes.Length)
            Dim sample As PerturbSeqSample = simulator.Simulate({NewGeneSpec(gene, mode)})

            If sample IsNot Nothing Then
                TrainingSamples.Add(sample)
            End If
        Next

        ' ---- 组合扰动：随机不重复地抽取若干个基因同时扰动 ----
        For i As Integer = 0 To config.NComboPerturbation - 1
            Dim specs As New List(Of InterventionSpec)()
            Dim used As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim size As Integer = std.Min(config.ComboSize, candidates.Count)

            While specs.Count < size
                Dim gene As String = candidates(rand.Next(candidates.Count))

                If used.Add(gene) Then
                    specs.Add(NewGeneSpec(gene, modes(i Mod modes.Length)))
                End If
            End While

            Dim combo As PerturbSeqSample = simulator.Simulate(specs)

            If combo IsNot Nothing Then
                TrainingSamples.Add(combo)
            End If
        Next

        Return TrainingSamples.Count
    End Function

    ''' <summary>
    ''' 构建可作为扰动目标的候选基因列表（转录因子优先）
    ''' </summary>
    ''' <returns>基因名列表</returns>
    Private Function BuildPerturbationCandidates() As List(Of String)
        Dim tfSet As New HashSet(Of String)(priorNetwork.TFNames, StringComparer.OrdinalIgnoreCase)
        Dim result As New List(Of String)()
        Dim rest As New List(Of String)()
        Dim idx As Integer = -1

        For Each gene As String In GeneNames
            If GraphData.TryGetGeneIndex(gene, idx) Then
                If tfSet.Contains(gene) Then
                    result.Add(gene)
                Else
                    rest.Add(gene)
                End If
            End If
        Next

        result.AddRange(rest)

        Return result
    End Function

    ''' <summary>
    ''' 构造一个干预定义
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="mode">干预模式</param>
    ''' <returns>干预定义对象</returns>
    Private Function NewGeneSpec(geneName As String, mode As InterventionMode) As InterventionSpec
        Dim idx As Integer = -1

        Call GraphData.TryGetGeneIndex(geneName, idx)

        Return New InterventionSpec With {
            .GeneName = geneName,
            .GeneIndex = idx,
            .Mode = mode
        }
    End Function

    ' ==================== 训练 ====================

    ''' <summary>
    ''' 注入外部的真实 Perturb-seq 训练样本（覆盖内置仿真样本）
    ''' </summary>
    ''' <param name="samples">实测扰动样本集合</param>
    Public Sub SetTrainingSamples(samples As IEnumerable(Of PerturbSeqSample))
        TrainingSamples.Clear()
        TrainingSamples.AddRange(samples.SafeQuery)
    End Sub

    ''' <summary>
    ''' 训练 GNN 模型
    ''' </summary>
    ''' <param name="samples">
    ''' 训练样本；为 Nothing 时使用 <see cref="TrainingSamples"/>
    ''' （若为空则先调用 <see cref="GenerateTrainingSamples"/> 生成）
    ''' </param>
    ''' <returns>损失曲线</returns>
    Public Function Train(Optional samples As IEnumerable(Of PerturbSeqSample) = Nothing) As Double()
        If samples IsNot Nothing Then
            Call SetTrainingSamples(samples)
        End If

        If TrainingSamples.Count = 0 Then
            Call GenerateTrainingSamples()
        End If

        If TrainingSamples.Count = 0 Then
            Throw New InvalidOperationException("没有可用的训练样本：先验网络中没有任何基因能够映射到表达矩阵上")
        End If

        Trainer = New GEARSTrainer(
            model:=Model,
            graphData:=GraphData,
            controlMean:=WildtypeMeans,
            controlSD:=WildtypeSDs,
            learningRate:=config.LearningRate,
            l2Lambda:=config.L2Lambda
        )

        LossCurve = Trainer.Train(TrainingSamples, config.Epochs, config.PrintEvery)

        Return LossCurve
    End Function

    ' ==================== 推理 ====================

    ''' <summary>
    ''' 执行一次（组合）虚拟扰动预测
    ''' </summary>
    ''' <param name="specs">干预定义集合；单个元素为单基因扰动，多个元素为组合扰动</param>
    ''' <returns>干预分析结果</returns>
    ''' <remarks>
    ''' 预测流程：先按 <see cref="InterventionSpec.GetInterventionValue"/> 把被扰动基因的表达
    ''' 改写为干预值（这一步是扰动信号进入网络的"入口"），再交给 GNN 预测全网络的 Δ 响应。
    ''' </remarks>
    Public Function Predict(specs As IEnumerable(Of InterventionSpec)) As InterventionResult
        Dim specList As List(Of InterventionSpec) = specs.SafeQuery.ToList()

        If specList.Count = 0 Then
            Throw New ArgumentException("至少需要一个干预定义")
        End If

        Dim n As Integer = exprData.NGene
        Dim indices As New List(Of Integer)()
        Dim names As New List(Of String)()

        For Each spec As InterventionSpec In specList
            Dim idx As Integer = spec.GeneIndex

            If idx < 0 OrElse idx >= n Then
                Call GraphData.TryGetGeneIndex(spec.GeneName, idx)
            End If

            If idx >= 0 AndAlso idx < n AndAlso Not indices.Contains(idx) Then
                indices.Add(idx)
                names.Add(GeneNames(idx))
            End If
        Next

        If indices.Count = 0 Then
            Return CreateUndefinedResult(specList(0))
        End If

        ' ---- 构造输入侧表达谱：被扰动基因改写为干预值 ----
        Dim inputExpr As Double() = CType(WildtypeMeans.Clone(), Double())
        Dim flag As Double() = New Double(n - 1) {}
        Dim xNorm As Double() = New Double(n - 1) {}
        Dim pertSet As New HashSet(Of Integer)(indices)

        For k As Integer = 0 To indices.Count - 1
            Dim i As Integer = indices(k)
            Dim target As Double = specList(k).GetInterventionValue(WildtypeMeans(i), WildtypeSDs(i))

            inputExpr(i) = target
            flag(i) = 1.0
        Next

        For i As Integer = 0 To n - 1
            Dim sd As Double = std.Max(WildtypeSDs(i), 0.000001)

            xNorm(i) = (inputExpr(i) - WildtypeMeans(i)) / sd
        Next

        ' ---- GNN 预测 Δ 表达 ----
        Dim deltaNorm As Double() = Model.PredictDelta(xNorm, flag)
        Dim mutant As Double() = New Double(n - 1) {}
        Dim fold As Double() = New Double(n - 1) {}
        Dim percent As Double() = New Double(n - 1) {}
        Dim z As Double() = New Double(n - 1) {}
        Dim significant As Boolean() = New Boolean(n - 1) {}

        For i As Integer = 0 To n - 1
            Dim sd As Double = std.Max(WildtypeSDs(i), 0.000001)
            Dim delta As Double = deltaNorm(i) * sd

            If pertSet.Contains(i) Then
                ' 被扰动基因的表达由干预值直接决定，不叠加网络预测
                mutant(i) = inputExpr(i)
            Else
                mutant(i) = std.Max(0.0, inputExpr(i) + delta)
            End If

            fold(i) = mutant(i) - WildtypeMeans(i)

            If std.Abs(WildtypeMeans(i)) > 0.0000000001 Then
                percent(i) = fold(i) / std.Abs(WildtypeMeans(i)) * 100
            End If

            z(i) = fold(i) / sd
            significant(i) = pertSet.Contains(i) OrElse std.Abs(z(i)) > config.SignificanceZScore
        Next

        Dim resultSpec As InterventionSpec

        If specList.Count = 1 Then
            resultSpec = specList(0)
        Else
            resultSpec = New InterventionSpec With {
                .GeneName = String.Join("+", names),
                .GeneIndex = indices(0),
                .Mode = specList(0).Mode
            }
        End If

        Return New InterventionResult With {
            .Spec = resultSpec,
            .WildtypeMeans = CType(WildtypeMeans.Clone(), Double()),
            .WildtypeSDs = CType(WildtypeSDs.Clone(), Double()),
            .MutantMeans = mutant,
            .FoldChanges = fold,
            .PercentChanges = percent,
            .ZScores = z,
            .IsSignificant = significant,
            .GeneNames = CType(GeneNames.Clone(), String())
        }
    End Function

    ''' <summary>
    ''' 构造「目标基因不存在、未执行虚拟扰动」的降级结果
    ''' </summary>
    ''' <param name="spec">干预定义</param>
    ''' <returns>所有变化量均为 0 的结果对象，其 <see cref="InterventionResult.Undefined"/> 为 True</returns>
    Private Function CreateUndefinedResult(spec As InterventionSpec) As InterventionResult
        Dim n As Integer = exprData.NGene

        Return New InterventionResult With {
            .Spec = spec,
            .WildtypeMeans = CType(WildtypeMeans.Clone(), Double()),
            .WildtypeSDs = CType(WildtypeSDs.Clone(), Double()),
            .MutantMeans = CType(WildtypeMeans.Clone(), Double()),
            .FoldChanges = New Double(n - 1) {},
            .PercentChanges = New Double(n - 1) {},
            .IsSignificant = New Boolean(n - 1) {},
            .ZScores = New Double(n - 1) {},
            .GeneNames = CType(GeneNames.Clone(), String()),
            .Undefined = True
        }
    End Function

    ' ==================== InsilicoPerturbationExperiment 接口实现 ====================

    ''' <summary>
    ''' 虚拟基因敲除：把目标基因表达置为 0，预测全网络的级联响应
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">用于估计 control 基线的样本数量；0 表示使用全部样本</param>
    ''' <returns>干预分析结果</returns>
    Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
        Return PredictWithBaseline(geneName, InterventionMode.Knockout, nSamples)
    End Function

    ''' <summary>
    ''' 虚拟基因过表达：把目标基因表达提升到 control 均值 + 3 倍标准差
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">用于估计 control 基线的样本数量；0 表示使用全部样本</param>
    ''' <returns>干预分析结果</returns>
    Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
        Return PredictWithBaseline(geneName, InterventionMode.Overexpression, nSamples)
    End Function

    ''' <summary>
    ''' 虚拟基因下调：把目标基因表达降低到 control 均值 − 2 倍标准差
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="nSamples">用于估计 control 基线的样本数量；0 表示使用全部样本</param>
    ''' <returns>干预分析结果</returns>
    Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
        Return PredictWithBaseline(geneName, InterventionMode.Knockdown, nSamples)
    End Function

    ''' <summary>
    ''' 单次预测的便捷入口（可临时指定用于估计基线的样本数量）
    ''' </summary>
    ''' <param name="geneName">目标基因名</param>
    ''' <param name="mode">干预模式</param>
    ''' <param name="nSamples">基线样本数量；小于等于 0 时沿用构造时确定的基线</param>
    ''' <returns>干预分析结果</returns>
    Private Function PredictWithBaseline(geneName As String, mode As InterventionMode, nSamples As Integer) As InterventionResult
        If nSamples > 0 AndAlso nSamples < exprData.NSample Then
            ' 临时基线：重新计算均值与标准差，预测结束后还原
            Dim keepMean As Double() = CType(WildtypeMeans.Clone(), Double())
            Dim keepSD As Double() = CType(WildtypeSDs.Clone(), Double())

            Try
                Call RecomputeBaseline(nSamples)

                Return Predict({NewGeneSpec(geneName, mode)})
            Finally
                Call System.Array.Copy(keepMean, WildtypeMeans, keepMean.Length)
                Call System.Array.Copy(keepSD, WildtypeSDs, keepSD.Length)
            End Try
        End If

        Return Predict({NewGeneSpec(geneName, mode)})
    End Function

    ''' <summary>
    ''' 使用指定数量的样本重新估计 control 基线（原地修改 <see cref="WildtypeMeans"/> 与 <see cref="WildtypeSDs"/>）
    ''' </summary>
    ''' <param name="nSamples">样本数量</param>
    Private Sub RecomputeBaseline(nSamples As Integer)
        Dim picked As New HashSet(Of Integer)()
        Dim total As Integer = exprData.NSample

        While picked.Count < std.Min(nSamples, total)
            picked.Add(rand.Next(total))
        End While

        Call ComputeBaseline(picked.OrderBy(Function(i) i).ToArray())
    End Sub

    ''' <summary>
    ''' 组合扰动预测：同时扰动多个基因，捕捉非加性的协同/拮抗效应
    ''' </summary>
    ''' <param name="geneNames">同时扰动的基因名集合</param>
    ''' <param name="mode">干预模式，默认敲除</param>
    ''' <returns>干预分析结果</returns>
    Public Function PredictCombination(geneNames As IEnumerable(Of String),
                                       Optional mode As InterventionMode = InterventionMode.Knockout) As InterventionResult
        Dim specs As New List(Of InterventionSpec)()

        For Each gene As String In geneNames.SafeQuery
            specs.Add(NewGeneSpec(gene, mode))
        Next

        Return Predict(specs)
    End Function

    ''' <summary>
    ''' 批量执行虚拟扰动
    ''' </summary>
    ''' <param name="geneNames">待扰动的基因名集合</param>
    ''' <param name="mode">干预模式，默认敲除</param>
    ''' <returns>每个基因对应的干预分析结果</returns>
    Public Function BatchPerturbation(geneNames As IEnumerable(Of String),
                                      Optional mode As InterventionMode = InterventionMode.Knockout) As List(Of InterventionResult)
        Dim results As New List(Of InterventionResult)()

        For Each gene As String In geneNames.SafeQuery
            results.Add(Predict({NewGeneSpec(gene, mode)}))
        Next

        Return results
    End Function

    ''' <summary>
    ''' 输出实验配置与结果的摘要
    ''' </summary>
    ''' <returns>多行摘要文本</returns>
    Public Overrides Function ToString() As String
        Dim lastLoss As String = If(LossCurve.Length > 0, LossCurve(LossCurve.Length - 1).ToString("F6"), "N/A")

        Return $"GEARS: {exprData.NGene} genes x {exprData.NSample} samples; " &
               $"graph={GraphData.NumPriorEdges} prior edges; " &
               $"model layers={Model.NumLayers}, hidden={Model.HiddenDim}; " &
               $"training samples={TrainingSamples.Count}, last loss={lastLoss}"
    End Function
End Class
