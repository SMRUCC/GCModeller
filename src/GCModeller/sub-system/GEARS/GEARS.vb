Imports System.Linq
Imports std = System.Math
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.GEARS.Graph
Imports SMRUCC.genomics.Analysis.GEARS.IO
Imports SMRUCC.genomics.Analysis.GEARS.Model
Imports SMRUCC.genomics.Analysis.GEARS.Training
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports System.IO
Imports System.IO.Compression
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

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

    ''' <summary>
    ''' 用于估计 control 基线所选取的样本列索引
    ''' </summary>
    ''' <remarks>
    ''' 该字段不是只读的：<see cref="SetTrainingSamples(Matrix, String(), SampleInfo())"/> 指定了显式的
    ''' control 列名后会更新它，从而让 <see cref="RecomputeBaseline"/> 与接口方法的 nSamples 分支
    ''' 与新的野生型基线保持同一口径。
    ''' </remarks>
    Dim baselineSamples As Integer()

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

    ''' <summary>
    ''' control（野生型）表达均值
    ''' </summary>
    ''' <returns>每个基因的表达均值</returns>
    ''' <remarks>
    ''' 可写：<see cref="SetTrainingSamples(Matrix, String(), SampleInfo())"/> 会用显式指定的 control 列
    ''' 重算并覆盖它；<see cref="Load"/> 则从 zip 包中还原保存时的取值。
    ''' </remarks>
    Public Property WildtypeMeans As Double()

    ''' <summary>
    ''' control（野生型）表达标准差，用于归一化、Z-score 与显著性判定
    ''' </summary>
    ''' <returns>每个基因的表达标准差</returns>
    ''' <remarks>可写，语义同 <see cref="WildtypeMeans"/>。</remarks>
    Public Property WildtypeSDs As Double()

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
        Else
            Call randf.SetSeed(Me.config.Seed)
        End If

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
            picked.Add(randf.Next(total))
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
                Dim gene As String = candidates(randf.Next(candidates.Count))

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
    Public Function SetTrainingSamples(samples As IEnumerable(Of PerturbSeqSample)) As GEARS
        TrainingSamples.Clear()
        TrainingSamples.AddRange(samples.SafeQuery)

        Return Me
    End Function

    ''' <summary>
    ''' <see cref="SampleInfo.metadata"/> 中记录「本样本被扰动了哪些基因」的键名
    ''' </summary>
    ''' <remarks>
    ''' 对应值是一个 JSON 字符串数组，例如 <c>["codY","luxR"]</c>。
    ''' </remarks>
    Public Const metadata_perturbed_genes As String = "perturbed_genes"

    ''' <summary>
    ''' <see cref="SampleInfo.metadata"/> 中记录「本样本使用哪种干预模式」的键名
    ''' </summary>
    ''' <remarks>
    ''' 对应值为 <see cref="InterventionMode"/> 的枚举名（大小写不敏感），例如 <c>Knockout</c>、<c>Knockdown</c>、
    ''' <c>Overexpression</c>、<c>Custom</c>。缺失时回退到从样本 ID 的后缀解析，仍解析不出则取 <see cref="InterventionMode.Knockout"/>。
    ''' </remarks>
    Public Const metadata_intervention_mode As String = "intervention_mode"

    ''' <summary>
    ''' Set Perturb-seq training sample from a given gene expression matrix
    ''' </summary>
    ''' <param name="samples">基因表达矩阵对象</param>
    ''' <param name="control">
    ''' 基线样本名称列表，计算出mean/sd作为共享野生型基线
    ''' </param>
    ''' <param name="perturbed">
    ''' 扰动后的样本名称列表，每一个<see cref="SampleInfo.ID"/>为<paramref name="samples"/>矩阵中的样本ID，为扰动样本，每一个扰动样本中被扰动的基因id集合以字符串json数组的形式记录在<see cref="SampleInfo.metadata"/>元数据字典中，通过键名<see cref="metadata_perturbed_genes"/>来获取
    ''' </param>
    Public Function SetTrainingSamples(samples As Matrix, control As String(), perturbed As SampleInfo()) As GEARS
        If samples Is Nothing Then
            Throw New ArgumentNullException(NameOf(samples))
        End If
        If control.IsNullOrEmpty Then
            Throw New ArgumentException("必须至少指定一个 control 样本列名", NameOf(control))
        End If
        If control.Length < 2 Then
            Throw New ArgumentException("control 样本至少需要 2 列，否则无法估计每个基因的标准差", NameOf(control))
        End If
        If perturbed.IsNullOrEmpty Then
            Throw New ArgumentException("必须至少指定一个扰动样本", NameOf(perturbed))
        End If

        Dim n As Integer = exprData.NGene
        Dim rowMap As Dictionary(Of String, Integer) = BuildGeneRowMap(samples)
        Dim exprRows As DataFrameRow() = samples.expression

        ' ---- 1. 由 control 列计算共享野生型基线 ----
        Dim ctrlIdx As Integer() = ResolveSampleColumns(samples, control, NameOf(control))
        Dim mean As Double() = New Double(n - 1) {}
        Dim sd As Double() = New Double(n - 1) {}
        Dim m As Integer = ctrlIdx.Length

        For i As Integer = 0 To n - 1
            Dim row As Double() = exprRows(rowMap(GeneNames(i))).experiments
            Dim sum As Double = 0

            For Each j As Integer In ctrlIdx
                sum += row(j)
            Next

            Dim avg As Double = sum / m
            Dim ss As Double = 0

            For Each j As Integer In ctrlIdx
                Dim d As Double = row(j) - avg

                ss += d * d
            Next

            mean(i) = avg
            sd(i) = std.Sqrt(ss / (m - 1))
        Next

        Me.WildtypeMeans = mean
        Me.WildtypeSDs = sd

        Call SyncBaselineSamples(control)

        ' ---- 2. 逐个扰动样本解析 ----
        Dim list As New List(Of PerturbSeqSample)()
        Dim skipped As New List(Of String)()
        Dim sampleIndex As Dictionary(Of String, Integer) = BuildSampleIndex(samples)

        For Each info As SampleInfo In perturbed
            If info Is Nothing Then
                Continue For
            End If

            Dim colIdx As Integer = -1

            If Not sampleIndex.TryGetValue(info.ID, colIdx) Then
                skipped.Add($"{info.ID}（矩阵中不存在该样本列）")
                Continue For
            End If

            Dim mode As InterventionMode = InterventionMode.Knockout
            Dim genes As String() = ResolvePerturbedGenes(info, mode)

            If genes.IsNullOrEmpty Then
                skipped.Add($"{info.ID}（未能解析出被扰动基因）")
                Continue For
            End If

            Dim indices As New List(Of Integer)()
            Dim geneNames As New List(Of String)()

            For Each g As String In genes
                Dim gi As Integer = -1

                If GraphData.TryGetGeneIndex(g.Trim(), gi) AndAlso Not indices.Contains(gi) Then
                    indices.Add(gi)
                    geneNames.Add(Me.GeneNames(gi))
                End If
            Next

            If indices.Count = 0 Then
                skipped.Add($"{info.ID}（被扰动基因 {String.Join("+", genes)} 都不在调控图中）")
                Continue For
            End If

            ' 标签侧 = 该列观测到的表达谱；输入侧 = 野生型基线，但被扰动基因位置替换为观测值
            Dim perturbedExpr As Double() = New Double(n - 1) {}
            Dim inputExpr As Double() = CType(mean.Clone(), Double())

            For i As Integer = 0 To n - 1
                perturbedExpr(i) = exprRows(rowMap(Me.GeneNames(i))).experiments(colIdx)
            Next

            For Each gi As Integer In indices
                inputExpr(gi) = perturbedExpr(gi)
            Next

            list.Add(New PerturbSeqSample With {
                .PerturbedGeneIndices = indices.ToArray(),
                .PerturbedGeneNames = geneNames.ToArray(),
                .ControlExpression = inputExpr,
                .PerturbedExpression = perturbedExpr,
                .Label = $"{info.ID}_{mode.ToString()}",
                .Mode = mode
            })
        Next

        If skipped.Count > 0 Then
            Console.WriteLine($"  [GEARS] SetTrainingSamples 跳过了 {skipped.Count} 个扰动样本: {String.Join("; ", skipped)}")
        End If

        If list.Count = 0 Then
            Throw New InvalidOperationException(
                "没有任何扰动样本可用：请检查 control/perturbed 列名是否与矩阵一致，" &
                "以及 SampleInfo.metadata 中的 " & metadata_perturbed_genes & " 是否为合法的基因名 JSON 数组")
        End If

        Return SetTrainingSamples(list)
    End Function

    ''' <summary>
    ''' 建立「基因名 → 表达矩阵行索引」的映射，并校验 GEARS 所需的基因全部存在
    ''' </summary>
    ''' <param name="samples">表达矩阵</param>
    ''' <returns>基因名到行索引的映射（大小写不敏感）</returns>
    Private Function BuildGeneRowMap(samples As Matrix) As Dictionary(Of String, Integer)
        Dim rowNames As String() = samples.rownames
        Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To rowNames.Length - 1
            map(rowNames(i)) = i
        Next

        Dim missing As New List(Of String)()

        For Each g As String In GeneNames
            If Not map.ContainsKey(g) Then
                missing.Add(g)
            End If
        Next

        If missing.Count > 0 Then
            Dim preview As String = String.Join(", ", missing.Take(10))

            Throw New ArgumentException(
                $"表达矩阵缺少 {missing.Count} 个 GEARS 基因对应的行（例如：{preview}），无法建立训练样本",
                NameOf(samples))
        End If

        Return map
    End Function

    ''' <summary>
    ''' 建立「样本列名 → 列索引」的映射
    ''' </summary>
    ''' <param name="samples">表达矩阵</param>
    ''' <returns>列名到列索引的映射（大小写不敏感）</returns>
    Private Shared Function BuildSampleIndex(samples As Matrix) As Dictionary(Of String, Integer)
        Dim ids As String() = samples.sampleID
        Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To ids.Length - 1
            map(ids(i)) = i
        Next

        Return map
    End Function

    ''' <summary>
    ''' 把一组样本列名解析为矩阵列索引，任一列名不存在时抛出友好异常
    ''' </summary>
    ''' <param name="samples">表达矩阵</param>
    ''' <param name="names">样本列名</param>
    ''' <param name="paramName">抛异常时使用的参数名</param>
    ''' <returns>列索引数组</returns>
    Private Shared Function ResolveSampleColumns(samples As Matrix, names As String(), paramName As String) As Integer()
        Dim index As Dictionary(Of String, Integer) = BuildSampleIndex(samples)
        Dim result As New List(Of Integer)()
        Dim missing As New List(Of String)()

        For Each name As String In names
            Dim idx As Integer = -1

            If index.TryGetValue(name, idx) Then
                result.Add(idx)
            Else
                missing.Add(name)
            End If
        Next

        If missing.Count > 0 Then
            Throw New ArgumentException(
                $"表达矩阵中不存在以下样本列: {String.Join(", ", missing)}", paramName)
        End If

        Return result.ToArray()
    End Function

    ''' <summary>
    ''' 从样本信息对象中解析出被扰动基因集合与干预模式
    ''' </summary>
    ''' <param name="info">样本信息对象</param>
    ''' <param name="mode">解析得到的干预模式；无法识别时保持调用方传入的默认值</param>
    ''' <returns>被扰动基因名数组；解析失败返回 Nothing</returns>
    ''' <remarks>
    ''' 解析优先级：
    ''' <list type="number">
    ''' <item><description>基因集合：<c>metadata[metadata_perturbed_genes]</c> 的 JSON 数组 → 回退到从样本 ID 解析；</description></item>
    ''' <item><description>干预模式：<c>metadata[metadata_intervention_mode]</c> → 回退到样本 ID 的模式后缀 → 默认 <see cref="InterventionMode.Knockout"/>。</description></item>
    ''' </list>
    ''' </remarks>
    Private Function ResolvePerturbedGenes(info As SampleInfo, ByRef mode As InterventionMode) As String()
        ' 先按列名解析，拿到 ID 里可能携带的模式后缀
        Dim idMode As InterventionMode = InterventionMode.Knockout
        Dim idGenePart As String = If(info.ID, "")
        Dim idGenes As String() = Nothing

        If Not String.IsNullOrWhiteSpace(idGenePart) Then
            idGenes = PerturbSeqIO.ParseMode(idGenePart, idMode).Split("+"c)
        End If

        ' 元数据优先
        Dim metaGenes As String() = Nothing

        If info.metadata IsNot Nothing AndAlso info.metadata.ContainsKey(metadata_perturbed_genes) Then
            Dim json As String = info.metadata(metadata_perturbed_genes)

            If Not String.IsNullOrWhiteSpace(json) Then
                Try
                    metaGenes = json.LoadJSON(Of String())
                Catch ex As Exception
                    metaGenes = Nothing
                End Try
            End If
        End If

        Dim metaMode As InterventionMode = Nothing
        Dim hasMetaMode As Boolean = False

        If info.metadata IsNot Nothing AndAlso info.metadata.ContainsKey(metadata_intervention_mode) Then
            hasMetaMode = ParseInterventionMode(info.metadata(metadata_intervention_mode), metaMode)
        End If

        If hasMetaMode Then
            mode = metaMode
        Else
            mode = idMode
        End If

        If Not metaGenes.IsNullOrEmpty Then
            Return metaGenes
        End If

        Return idGenes
    End Function

    ''' <summary>
    ''' 把文本解析为 <see cref="InterventionMode"/> 枚举
    ''' </summary>
    ''' <param name="text">原始文本，大小写不敏感，支持 ko / kd / oe 等简写</param>
    ''' <param name="mode">解析得到的枚举值</param>
    ''' <returns>解析成功返回 True，否则返回 False</returns>
    Private Shared Function ParseInterventionMode(text As String, ByRef mode As InterventionMode) As Boolean
        If String.IsNullOrWhiteSpace(text) Then
            Return False
        End If

        Select Case text.Trim().ToLower()
            Case "knockout", "ko"
                mode = InterventionMode.Knockout
            Case "knockdown", "kd"
                mode = InterventionMode.Knockdown
            Case "overexpression", "overexpress", "oe"
                mode = InterventionMode.Overexpression
            Case "custom"
                mode = InterventionMode.Custom
            Case Else
                Return False
        End Select

        Return True
    End Function

    ''' <summary>
    ''' 当 control 列名同时存在于主表达矩阵时，同步更新基线样本索引
    ''' </summary>
    ''' <param name="control">control 样本列名</param>
    ''' <remarks>
    ''' 保持 <see cref="baselineSamples"/> 与新的野生型基线同一口径，
    ''' 使 <see cref="RecomputeBaseline"/> 与接口方法的 nSamples 分支行为一致。
    ''' </remarks>
    Private Sub SyncBaselineSamples(control As String())
        ' 基线样本索引是相对「主表达矩阵 exprData」的列索引，因此这里要查的是 exprData 的样本名
        Dim mainIndex As Dictionary(Of String, Integer) = BuildSampleIndex(exprData.SampleNames)
        Dim mapped As New List(Of Integer)()

        For Each id As String In control
            Dim idx As Integer = -1

            If mainIndex.TryGetValue(id, idx) Then
                mapped.Add(idx)
            End If
        Next

        If mapped.Count = control.Length Then
            baselineSamples = mapped.OrderBy(Function(i) i).ToArray()
        Else
            Console.WriteLine($"  [GEARS] control 列名与主表达矩阵不一致，基线样本索引保持为构造时的取值")
        End If
    End Sub

    ''' <summary>
    ''' 建立「样本列名 → 列索引」的映射
    ''' </summary>
    ''' <param name="sampleNames">样本名数组</param>
    ''' <returns>列名到列索引的映射（大小写不敏感）</returns>
    Private Shared Function BuildSampleIndex(sampleNames As String()) As Dictionary(Of String, Integer)
        Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To sampleNames.Length - 1
            map(sampleNames(i)) = i
        Next

        Return map
    End Function

    ''' <summary>
    ''' Save current model as zip file
    ''' </summary>
    ''' <param name="file">
    ''' 目标可写流；调用方负责其生命周期，本方法不会关闭它
    ''' </param>
    ''' <remarks>
    ''' zip 包内包含 <c>manifest.json</c>、<c>prior.csv</c>、<c>expression.bin</c>、
    ''' <c>baseline.bin</c>、<c>model.bin</c> 五个条目，完整保存配置、先验网络、表达矩阵、
    ''' 图结构（按同样入参重建）、模型参数、基线与损失曲线，加载后可继续训练。
    ''' </remarks>
    Public Sub Save(file As Stream)
        If file Is Nothing Then
            Throw New ArgumentNullException(NameOf(file))
        End If
        If Model Is Nothing Then
            Throw New InvalidOperationException("模型尚未构建，无法保存")
        End If

        Using zip As New ZipArchive(file, ZipArchiveMode.Create, leaveOpen:=True)
            Dim parameters As List(Of Tensor) = Model.GetParameters()
            Dim info As New GEARSStorage.Manifest With {
                .formatVersion = GEARSStorage.FormatVersion,
                .savedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                .nGenes = exprData.NGene,
                .nSamples = exprData.NSample,
                .config = config,
                .embeddingDim = Model.EmbeddingDim,
                .hiddenDim = Model.HiddenDim,
                .numLayers = Model.NumLayers,
                .lossCurve = If(LossCurve, New Double() {}),
                .baselineSamples = If(baselineSamples, New Integer() {}),
                .nParameters = parameters.Count
            }

            Call GEARSStorage.WriteManifest(zip, info)
            Call GEARSStorage.WritePrior(zip, priorNetwork)
            Call GEARSStorage.WriteExpression(zip, ToExpressionMatrix())
            Call GEARSStorage.WriteVector(zip, GEARSStorage.EntryBaseline, PackBaseline())
            Call GEARSStorage.WriteTensors(zip, parameters)
        End Using
    End Sub

    ''' <summary>
    ''' Load model from a zip file
    ''' </summary>
    ''' <param name="file">zip 包的可读流；调用方负责其生命周期，本方法不会关闭它</param>
    ''' <returns>还原出来的 GEARS 实例，其推理结果与保存前一致，且可继续训练</returns>
    Public Shared Function Load(file As Stream) As GEARS
        If file Is Nothing Then
            Throw New ArgumentNullException(NameOf(file))
        End If

        Using zip As New ZipArchive(file, ZipArchiveMode.Read, leaveOpen:=True)
            Dim info As GEARSStorage.Manifest = GEARSStorage.ReadManifest(zip)
            Dim prior As PriorNetwork = GEARSStorage.ReadPrior(zip)
            Dim matrix As Matrix = GEARSStorage.ReadExpression(zip)
            Dim expr As GeneExpressionData = matrix.ReadGeneExpressionMatrix()

            ' 构造函数会按保存时的配置重建调控图与模型结构（seed 一致，初始化也一致）
            Dim gears As New GEARS(expr, prior, info.config)

            Call gears.RestoreBaseline(GEARSStorage.ReadVector(zip, GEARSStorage.EntryBaseline))

            gears.LossCurve = If(info.lossCurve, New Double() {})

            If Not info.baselineSamples.IsNullOrEmpty Then
                gears.baselineSamples = info.baselineSamples
            End If

            ' 形状逐一对齐校验后才注入，避免静默错位
            Call GEARSStorage.ReadTensors(zip, gears.Model.GetParameters())

            Return gears
        End Using
    End Function

    ''' <summary>
    ''' 把内部的基因表达数据还原为 <see cref="Matrix"/> 对象
    ''' </summary>
    ''' <returns>表达矩阵（行=基因，列=样本）</returns>
    Private Function ToExpressionMatrix() As Matrix
        Dim n As Integer = exprData.NGene
        Dim rows As DataFrameRow() = New DataFrameRow(n - 1) {}

        For i As Integer = 0 To n - 1
            rows(i) = New DataFrameRow With {
                .geneID = GeneNames(i),
                .experiments = exprData.GetGeneExpression(i)
            }
        Next

        Return New Matrix With {
            .tag = "GEARS/exprData",
            .sampleID = exprData.SampleNames,
            .expression = rows
        }
    End Function

    ''' <summary>
    ''' 把野生型均值与标准差打包为单个向量，便于写入一个 zip 条目
    ''' </summary>
    ''' <returns>长度为 2×基因数 的向量，前一半为均值，后一半为标准差</returns>
    Private Function PackBaseline() As Double()
        Dim n As Integer = exprData.NGene
        Dim packed As Double() = New Double(2 * n - 1) {}

        Call Array.Copy(WildtypeMeans, 0, packed, 0, n)
        Call Array.Copy(WildtypeSDs, 0, packed, n, n)

        Return packed
    End Function

    ''' <summary>
    ''' 从打包向量中还原野生型均值与标准差
    ''' </summary>
    ''' <param name="packed">长度为 2×基因数 的向量</param>
    Private Sub RestoreBaseline(packed As Double())
        Dim n As Integer = exprData.NGene

        If packed Is Nothing OrElse packed.Length <> 2 * n Then
            Throw New InvalidDataException(
                $"基线数据长度异常：zip 中为 {If(packed Is Nothing, 0, packed.Length)}，当前表达矩阵期望 {2 * n}")
        End If

        WildtypeMeans = New Double(n - 1) {}
        WildtypeSDs = New Double(n - 1) {}

        Call Array.Copy(packed, 0, WildtypeMeans, 0, n)
        Call Array.Copy(packed, n, WildtypeSDs, 0, n)
    End Sub

    ''' <summary>
    ''' 训练 GNN 模型
    ''' </summary>
    ''' <returns>损失曲线</returns>
    ''' <remarks>
    ''' 训练样本；为 Nothing 时使用 <see cref="TrainingSamples"/>若为空则先调用 <see cref="GenerateTrainingSamples"/> 生成）
    ''' </remarks>
    Public Function Train() As Double()
        If TrainingSamples.IsNullOrEmpty Then
            Throw New InvalidOperationException("没有可用的训练样本：先验网络中没有任何基因能够映射到表达矩阵上，请先通过SetTrainingSamples函数设置训练样本")
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
            picked.Add(randf.Next(total))
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
