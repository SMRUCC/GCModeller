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
            .Id = If(id, "cell_" & Guid.NewGuid.ToString("N").Substring(0, 8)),
            .taxonomy_info = taxonomy,
            .Blueprint = blueprint,
            .State = New CellularState(
                genes:=genes,
                metabolites:=graph.InternalIds,
                boundary:=graph.BoundaryIds,
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

    ''' <summary>
    ''' 把细胞播种到环境的每一个有效格点上
    ''' </summary>
    Public Function SeedCells(env As Environment,
                              blueprint As CellaBlueprint,
                              Optional cellsPerSpot As Integer = 1,
                              Optional taxonomy As Taxonomy = Nothing,
                              Optional seed As Integer = 2024) As Integer

        Dim rand As New Random(seed)
        Dim graph As MetabolicNetworkGraph = GetMetabolicGraph(blueprint)
        Dim count As Integer = 0

        For Each spot As Spot In env.GetAllSpots()
            For k As Integer = 1 To cellsPerSpot
                Dim cella As VirtualCella = BuildCell(
                    blueprint:=blueprint,
                    taxonomy:=taxonomy,
                    id:=$"cell_{count + 1}"
                )

                ' 边界初值取自该格点的培养基
                If spot.Medium IsNot Nothing Then
                    For i As Integer = 0 To cella.State.BoundaryNames.Length - 1
                        Dim level As Double = 0.0

                        If spot.Medium.TryGetValue(cella.State.BoundaryNames(i), level) Then
                            cella.State.Boundary(i) = level
                        End If
                    Next
                End If

                cella.Spot = spot
                spot.cells.Add(cella)
                count += 1
            Next
        Next

        Return count
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
