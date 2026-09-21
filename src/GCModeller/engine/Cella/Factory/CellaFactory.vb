' ============================================================
' CellaFactory.vb - 虚拟细胞装配层
' ============================================================
' 由 CellaBlueprint 装配出 VirtualCella 及其六个子网络。
'
' 两条构建路径：
'   1. BuildFrom(blueprint)                 —— 轻量入口（推荐，demo 走这条）
'   2. FromModel(VirtualCell, Definition…)  —— GCMarkup 全基因组模型入口，
'      内部先把 GCMarkup 模型翻译成蓝图，再走同一套装配逻辑。
'
' 训练策略：GEARS 的图神经网络与 Metaboliq 的液态网络都需要训练，而训练
' 开销不应该随细胞数量线性增长。因此
'   * GEARS 只训练**一个**共享实例（推理无副作用，可跨细胞共享）；
'   * Metaboliq 只训练**一个**模板，逐细胞复制参数（隐藏状态是实例私有的）。
' ============================================================

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.GEARS
Imports SMRUCC.genomics.Analysis.Metaboliq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.genomics.Metagenomics

Public Module CellaFactory

    ' ==================== 拓扑 ====================

    ''' <summary>
    ''' 由反应列表构建代谢网络拓扑（内部自动派生 S / A_adj / P 掩码），
    ''' 结果缓存在蓝图上，供所有细胞共享。
    ''' </summary>
    Public Function GetMetabolicGraph(blueprint As CellaBlueprint) As MetabolicNetworkGraph
        If blueprint.MetabolicGraph Is Nothing Then
            blueprint.MetabolicGraph = New MetabolicNetworkGraph(blueprint.Reactions, blueprint.ExplicitBoundary)
        End If

        Return blueprint.MetabolicGraph
    End Function

    ' ==================== 共享模型训练 ====================

    ''' <summary>
    ''' 训练共享的 GEARS 图神经网络（用内置仿真器合成伪 Perturb-seq 样本）
    ''' </summary>
    Public Function TrainGears(blueprint As CellaBlueprint, Optional epochs As Integer = -1) As GEARS
        Call blueprint.Validate()

        If blueprint.Gears Is Nothing Then
            blueprint.Gears = New GEARS(blueprint.Expression, blueprint.Prior, blueprint.GearsConfig)
        End If

        If epochs > 0 AndAlso blueprint.Gears.Options IsNot Nothing Then
            blueprint.Gears.Options.Epochs = epochs
        End If

        If blueprint.Gears.TrainingSamples.IsNullOrEmpty Then
            Call blueprint.Gears.GenerateTrainingSamples()
        End If

        Call blueprint.Gears.Train()

        Return blueprint.Gears
    End Function

    ''' <summary>
    ''' 训练 Metaboliq 液态网络模板
    ''' </summary>
    Public Function TrainMetabolicTemplate(blueprint As CellaBlueprint, data As MetabolicTrainingSet) As MetabolicLiquidNetwork
        Call blueprint.Validate()
        Call data.Validate()

        Dim graph As MetabolicNetworkGraph = GetMetabolicGraph(blueprint)
        Dim model As New MetabolicLiquidNetwork(
            graph:=graph,
            mode:=blueprint.MetabolicMode,
            solver:=blueprint.MetabolicSolver
        )

        Call model.SetTauBounds(blueprint.TauMin, blueprint.TauMax)

        model.MaxSubStep = blueprint.MaxSubStep

        Dim trainer As New MetabolicTrainer(model, data.Config)
        Dim history As List(Of EpochLoss) = trainer.Fit(
            times:=data.Times,
            observed:=data.Observed,
            enzymeSeries:=data.Enzymes,
            boundarySeries:=data.Boundary,
            observedFlux:=data.Flux
        )

        blueprint.MetabolicTemplate = model
        blueprint.MetabolicLoss = history

        Return model
    End Function

    ' ==================== 装配 ====================

    ''' <summary>
    ''' 由蓝图构建一个虚拟细胞
    ''' </summary>
    Public Function BuildCell(blueprint As CellaBlueprint,
                             Optional taxonomy As Taxonomy = Nothing,
                             Optional id As String = Nothing,
                             Optional initialMetabolite As Double() = Nothing,
                             Optional initialMrna As Double() = Nothing) As VirtualCella

        Call blueprint.Validate()

        Dim graph As MetabolicNetworkGraph = GetMetabolicGraph(blueprint)
        Dim genes As String() = blueprint.Genes
        Dim cella As New VirtualCella With {
            .Id = If(id, NextCellId(blueprint.SpeciesName)),
            .taxonomy_info = taxonomy,
            .Blueprint = blueprint,
            .Species = blueprint.SpeciesName,
            .State = New CellularState(
                genes:=genes,
                metabolites:=graph.InternalIds,
                boundaryMetabolites:=graph.BoundaryIds,
                signals:=blueprint.GetSignalChannels()
            )
        }

        Dim state As CellularState = cella.State

        ' ---- 初始转录本 ----
        If Not initialMrna.IsNullOrEmpty Then
            Call Array.Copy(initialMrna, state.mRNA, System.Math.Min(initialMrna.Length, state.mRNA.Length))
        ElseIf blueprint.Expression IsNot Nothing Then
            For i As Integer = 0 To genes.Length - 1
                state.mRNA(i) = RowMean(blueprint.Expression, i)
            Next
        End If

        ' ---- 初始蛋白：按翻译/降解的准稳态估计 ----
        For i As Integer = 0 To genes.Length - 1
            Dim decay As Double = blueprint.ProteinDegradationOf(genes(i)) + blueprint.DilutionRate

            If decay > 0 Then
                state.Protein(i) = blueprint.TranslationRateOf(genes(i)) * state.mRNA(i) / decay
            End If
        Next

        ' ---- 信号初值：未受刺激的基线 ----
        For i As Integer = 0 To state.Signal.Length - 1
            state.Signal(i) = 0.5
        Next

        ' ---- 子网络 ----
        cella.signaling = New SignalTransductionNetwork(cella, blueprint)
        cella.grn = New GeneRegulatoryNetwork(cella, blueprint, blueprint.Gears)
        cella.translation = New TranslationSystem(cella, blueprint)
        cella.transportation = New TransportSystem(cella, blueprint)
        cella.metabolic = New MetabolicNetwork(graph, cella, blueprint, blueprint.MetabolicTemplate, initialMetabolite)
        cella.turnover = New TurnoverSystem(cella, blueprint)

        Return cella
    End Function

    ' ==================== 标识与播种 ====================

    Private cellSerial As Integer = 0

    ''' <summary>生成一个全局唯一的细胞 id（形如 species_0001）</summary>
    Public Function NextCellId(species As String) As String
        cellSerial += 1

        Dim prefix As String = If(species, "cell")

        Return $"{prefix}_{cellSerial.ToString("D4")}"
    End Function

    ''' <summary>
    ''' 把细胞播种到环境的每一个有效格点上
    ''' </summary>
    Public Function SeedCells(env As Environment,
                             blueprint As CellaBlueprint,
                             Optional cellsPerSpot As Integer = 1,
                             Optional taxonomy As Taxonomy = Nothing,
                             Optional seed As Integer = 2024) As Integer

        Dim count As Integer = 0

        For Each spot As Spot In env.GetAllSpots()
            For k As Integer = 1 To cellsPerSpot
                Dim cella As VirtualCella = BuildCell(blueprint, taxonomy)

                Call BindToSpot(cella, spot, env)
                count += 1
            Next
        Next

        Return count
    End Function

    ''' <summary>
    ''' 多物种混合接种：每个物种随机分配到若干不同的格点上
    ''' </summary>
    ''' <param name="blueprints">各物种的蓝图；<see cref="CellaBlueprint.SpeciesName"/> 必须唯一</param>
    ''' <param name="perSpecies">每个物种接种的细胞数</param>
    Public Function SeedMixedCulture(env As Environment,
                                     blueprints As CellaBlueprint(),
                                     Optional perSpecies As Integer = 2,
                                     Optional taxonomy As Taxonomy = Nothing,
                                     Optional seed As Integer = 2024) As Integer

        If env Is Nothing OrElse blueprints.IsNullOrEmpty Then
            Return 0
        End If

        Dim rand As New Random(seed)
        Dim spots As Spot() = env.GetAllSpots().ToArray()
        Dim count As Integer = 0

        For Each blueprint As CellaBlueprint In blueprints
            For k As Integer = 1 To perSpecies
                Dim spot As Spot = spots(rand.Next(spots.Length))
                Dim cella As VirtualCella = BuildCell(blueprint, taxonomy)

                Call BindToSpot(cella, spot, env)
                count += 1
            Next
        Next

        Return count
    End Function

    ''' <summary>
    ''' 把细胞放到指定格点：初始化边界浓度、登记谱系、加入格点细胞列表
    ''' </summary>
    Public Sub BindToSpot(cella As VirtualCella, spot As Spot, env As Environment)
        If cella Is Nothing OrElse spot Is Nothing Then
            Return
        End If

        cella.Spot = spot
        cella.IsAlive = True

        If env IsNot Nothing Then
            cella.BirthTime = env.CurrentTime
        End If

        If spot.Medium IsNot Nothing Then
            For i As Integer = 0 To cella.State.BoundaryNames.Length - 1
                Dim level As Double = 0.0

                If spot.Medium.TryGetValue(cella.State.BoundaryNames(i), level) Then
                    cella.State.Boundary(i) = level
                End If
            Next
        End If

        spot.cells.Add(cella)

        If env IsNot Nothing Then
            Call env.Lineage.Register(cella, env.CurrentTime)
        End If
    End Sub

    ''' <summary>
    ''' 确保每个格点的培养基覆盖所有物种需要的胞外代谢物（缺失的补 0）
    ''' </summary>
    Public Sub EnsureMediumCoverage(env As Environment, blueprints As CellaBlueprint())
        If env Is Nothing OrElse blueprints.IsNullOrEmpty Then
            Return
        End If

        Dim ids As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each blueprint As CellaBlueprint In blueprints
            Dim graph As MetabolicNetworkGraph = GetMetabolicGraph(blueprint)

            For Each id As String In graph.BoundaryIds
                Call ids.Add(id)
            Next
        Next

        For Each spot As Spot In env.GetAllSpots()
            If spot.Medium Is Nothing Then
                spot.Medium = New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
            End If

            For Each id As String In ids
                If Not spot.Medium.ContainsKey(id) Then
                    spot.Medium(id) = 0.0
                End If
            Next
        Next

        ' 培养基模板同步补齐，贴壁补料才能覆盖全部成分
        If env.MediumTemplate IsNot Nothing Then
            For Each id As String In ids
                If Not env.MediumTemplate.ContainsKey(id) Then
                    env.MediumTemplate(id) = 0.0
                End If
            Next
        End If
    End Sub

    ' ==================== 二分裂 ====================

    ''' <summary>
    ''' 二分裂：创建一个继承亲代一半物质状态的子代，放入同一格点
    ''' </summary>
    ''' <returns>子代细胞；因格点承载上限等原因无法分裂时返回 Nothing</returns>
    ''' <remarks>
    ''' 子代的六个子网络是全新构建的（内部积分器从继承后的状态重新起算），
    ''' 因此调用 ResyncSubNetworks 让积分器与状态池对齐。
    ''' 谱系登记由 CellLifecycle 负责，这里只管创建。
    ''' </remarks>
    Public Function DivideCell(parent As VirtualCella, time As Double,
                               Optional daughterSpot As Spot = Nothing) As VirtualCella

        If parent Is Nothing OrElse parent.Blueprint Is Nothing Then
            Return Nothing
        End If

        Dim blueprint As CellaBlueprint = parent.Blueprint
        Dim spot As Spot = If(daughterSpot, parent.Spot)

        If spot Is Nothing Then
            Return Nothing
        End If

        If spot.cells.Count >= blueprint.MaxCellsPerSpot Then
            Return Nothing
        End If

        Dim fraction As Double = System.Math.Min(1.0, System.Math.Max(0.0, blueprint.DaughterStateFraction))
        Dim child As VirtualCella = BuildCell(blueprint, parent.taxonomy_info)

        child.ParentId = parent.Id
        child.Species = If(parent.Species, blueprint.SpeciesName)
        child.Generation = parent.Generation + 1
        child.BirthTime = time

        ' 继承一半物质型状态池，强度型状态池直接复制
        Call child.State.CopyFrom(parent.State, fraction)
        Call child.ResyncSubNetworks()

        ' 生物量对半
        child.Biomass = parent.Biomass * fraction
        parent.Biomass *= (1.0 - fraction)
        parent.offspringCounter += 1

        ' 子代不再继承亲代的饥饿计数与年龄
        child.StarvedTicks = 0

        Call BindToSpot(child, spot, Nothing)

        Return child
    End Function

    ' ==================== GCMarkup 全基因组模型 → 蓝图 ====================

    ''' <summary>
    ''' 把 GCMarkup 全基因组模型翻译为蓝图
    ''' </summary>
    Public Function BlueprintFromModel(cell As VirtualCell,
                                       Optional define As Object = Nothing,
                                       Optional dynamics As Object = Nothing) As CellaBlueprint

        Dim operonIndex As Dictionary(Of String, TranscriptUnit) = cell.genome.GetAllOperon
        Dim geneSet As New List(Of String)

        For Each operon As TranscriptUnit In operonIndex.Values
            For Each g In operon.genes.SafeQuery
                If g.locus_tag IsNot Nothing Then
                    geneSet.Add(g.locus_tag)
                End If
            Next
        Next

        ' ---- 先验调控网络：操纵子展开为基因级边（GEARS 按基因名匹配）----
        Dim prior As New PriorNetwork()

        For Each trn In cell.genome.regulations.SafeQuery
            If trn.regulator IsNot Nothing AndAlso Not geneSet.Contains(trn.regulator) Then
                geneSet.Add(trn.regulator)
            End If

            If trn.operonId Is Nothing OrElse Not operonIndex.ContainsKey(trn.operonId) Then
                Continue For
            End If

            Dim effector As Effector = If(trn.mode = "activator", Effector.Activator, Effector.Inhibitor)

            For Each g In operonIndex(trn.operonId).genes.SafeQuery
                If g.locus_tag Is Nothing Then
                    Continue For
                End If

                Call prior.AddEdge(trn.regulator, g.locus_tag, effector, 1.0, If(trn.motif Is Nothing, "", trn.motif.ToString()))
            Next
        Next

        Dim genes As String() = geneSet.Distinct(StringComparer.OrdinalIgnoreCase).ToArray()

        ' ---- 代谢反应网络 ----
        Dim reactions As New List(Of MetabolicReaction)()
        Dim reactionGene As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        If cell.metabolismStructure IsNot Nothing Then
            If cell.metabolismStructure.reactions IsNot Nothing Then
                For Each r In cell.metabolismStructure.reactions.enzymatic.SafeQuery
                    reactions.Add(ToMetabolicReaction(r))
                Next
                For Each r In cell.metabolismStructure.reactions.none_enzymatic.SafeQuery
                    reactions.Add(ToMetabolicReaction(r))
                Next
            End If

            For Each enzyme In cell.metabolismStructure.enzymes.SafeQuery
                If enzyme.catalysis Is Nothing Then
                    Continue For
                End If

                Dim proteinId As String = enzyme.proteinID

                For Each link In enzyme.catalysis
                    Dim reactionId As String = link.reaction

                    If reactionId IsNot Nothing AndAlso proteinId IsNot Nothing Then
                        reactionGene(reactionId) = proteinId
                    End If
                Next
            Next
        End If

        If reactions.Count = 0 Then
            Throw New InvalidOperationException("GCMarkup 模型中没有可用的代谢反应，无法构建虚拟细胞")
        End If

        ' ---- 合成基线表达矩阵（GCMarkup 模型不含实测表达量）----
        Dim expression As GeneExpressionData = SyntheticBaseline(genes)

        Return New CellaBlueprint With {
            .Prior = prior,
            .Expression = expression,
            .Reactions = reactions.ToArray(),
            .ReactionGeneMap = reactionGene,
            .TFGenes = prior.TFNames.ToArray(),
            .SignalChannels = prior.TFNames.ToArray()
        }
    End Function

    ''' <summary>取基线表达矩阵中某个基因的行均值</summary>
    Public Function RowMean(expression As GeneExpressionData, geneIndex As Integer) As Double
        Dim sum As Double = 0.0
        Dim m As Integer = expression.NSample

        If m <= 0 Then
            Return 0.0
        End If

        For j As Integer = 0 To m - 1
            sum += expression.Matrix(geneIndex, j)
        Next

        Return sum / m
    End Function

    Private Function ToMetabolicReaction(r As Reaction) As MetabolicReaction
        Return New MetabolicReaction With {
            .id = r.ID,
            .name = r.name,
            .description = r.note,
            .left = r.substrate.SafeQuery _
                .Select(Function(c) New CompoundSpecieReference(c.factor, c.compound)) _
                .ToArray(),
            .right = r.product.SafeQuery _
                .Select(Function(c) New CompoundSpecieReference(c.factor, c.compound)) _
                .ToArray(),
            .is_reversible = False,
            .is_spontaneous = Not r.is_enzymatic,
            .ECNumbers = If(r.ec_number, {}),
            .gibbs = r.gibbs
        }
    End Function

    ''' <summary>
    ''' 合成一份基线表达矩阵：GCMarkup 模型里没有实测表达量，只能给出一份
    ''' 名义基线（用于估计 GEARS 的野生型均值与标准差），后续可用实测矩阵替换。
    ''' </summary>
    Public Function SyntheticBaseline(genes As String(), Optional nSamples As Integer = 4, Optional seed As Integer = 2024) As GeneExpressionData
        Dim rand As New Random(seed)
        Dim n As Integer = genes.Length
        Dim m As Integer = System.Math.Max(2, nSamples)
        Dim matrix As Double(,) = New Double(n - 1, m - 1) {}
        Dim samples As String() = New String(m - 1) {}
        Dim times As Double() = New Double(m - 1) {}

        For j As Integer = 0 To m - 1
            samples(j) = $"ctrl_{j + 1}"
            times(j) = 0
        Next

        For i As Integer = 0 To n - 1
            Dim base As Double = 8.0 + rand.NextDouble() * 6.0

            For j As Integer = 0 To m - 1
                Dim noise As Double = 1.0 + (rand.NextDouble() - 0.5) * 0.2

                matrix(i, j) = base * noise
            Next
        Next

        Return New GeneExpressionData With {
            .GeneNames = genes,
            .SampleNames = samples,
            .Matrix = matrix,
            .TimePoints = times
        }
    End Function

End Module
