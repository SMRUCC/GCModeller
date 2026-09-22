' ============================================================
' Species.vb - 发酵群落中的 4 个代谢分工物种
' ============================================================
' 四个物种共享同一套「中心碳代谢」反应骨架，但各自取舍不同，从而形成
' 一条碳链分工 + 一条闭环氮循环：
'
'   S1 glc_fermenter   葡萄糖发酵菌（初级生产者）
'        消耗 glc_e / o2_e / pi_e / nh4_e
'        分泌 lac_e / etoh_e / ac_e / co2_e / **aa_e**（唯一的氨基酸输出者）
'
'   S2 lactate_utilizer  乳酸利用菌（次级消费者）
'        消耗 lac_e / o2_e / pi_e / nh4_e        ← 接收 S1 的乳酸
'        分泌 ac_e / **ppa_e** / co2_e            ← 把乙酸交给 S3
'
'   S3 acetate_utilizer  乙酸利用菌（三级消费者）
'        消耗 ac_e / **co2_e** / o2_e / pi_e / nh4_e  ← 接收 S1/S2 的乙酸与 CO2
'        分泌 **but_e** / **h2_e** / co2_e
'
'   S4 aa_auxotroph     氨基酸营养缺陷型（闭环氮循环的一环）
'        缺失 GDH（无法把 akg + nh4 合成氨基酸），必须摄取 **aa_e**
'        消耗 glc_e / aa_e / o2_e / pi_e
'        分泌 **nh4_e**（把摄取的氨基酸脱氨后排出铵）→ 回补 S1/S2/S3
'
' 由此形成的交叉喂养边（预期，用于与仿真结果对照）：
'   S1 → S2 : lac_e          S2 → S3 : ac_e
'   S1 → S3 : ac_e           S1/S2/S4 → S3 : co2_e
'   S1 → S4 : aa_e           S4 → S1/S2/S3 : nh4_e
' ============================================================

Imports Cella
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.GEARS
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.MetabolicModel

''' <summary>营养代谢物：趋化性只向这些底物浓度高的方向游动</summary>
Public Module Nutrients

    Public ReadOnly Property Ids As String()
        Get
            Return {"glc_e", "o2_e", "pi_e", "nh4_e", "aa_e", "lac_e", "ac_e", "co2_e"}
        End Get
    End Property

End Module

''' <summary>
''' 一个物种的定义：反应子集 + 耦合映射 + 运动/趋化能力 + 播种比例
''' </summary>
Public Class SpeciesDefinition

    Public Property id As String
    Public Property name As String
    Public Property taxonomy As String
    Public Property description As String

    ''' <summary>该物种拥有的反应 id 集合</summary>
    Public Property Reactions As String()

    ''' <summary>该物种拥有的基因 id 集合（必须与 <see cref="Expression"/> 的行名一致）</summary>
    Public Property Genes As String()

    ''' <summary>播种比例（相对权重）</summary>
    Public Property Fraction As Double = 0.25

    ''' <summary>该物种的生长底物 / 趋化配体集合（用于饥饿判定与梯度感知）</summary>
    Public Property Substrates As String()

    ''' <summary>运动能力缩放：0 = 不运动（无鞭毛），1 = 正常鞭毛</summary>
    Public Property MotilityFactor As Double = 1.0

    ''' <summary>已经构建好的蓝图</summary>
    Public Property Blueprint As CellaBlueprint

    Public Overrides Function ToString() As String
        Return $"{id} ({name})"
    End Function

End Class

''' <summary>
''' 发酵群落的物种库
''' </summary>
Public Module Species

    Public Const GlcFermenter As String = "glc_fermenter"
    Public Const LactateUtilizer As String = "lactate_utilizer"
    Public Const AcetateUtilizer As String = "acetate_utilizer"
    Public Const AaAuxotroph As String = "aa_auxotroph"

    ''' <summary>糖酵解核心</summary>
    Private ReadOnly Property Glycolysis As String()
        Get
            Return {"PTS", "GLK", "PGI", "PFK", "FBA", "TPI", "GAPD", "PGK", "PGM", "ENO", "PYK"}
        End Get
    End Property

    ''' <summary>糖酵解下游（不含葡萄糖摄取）</summary>
    Private ReadOnly Property GlycolysisCore As String()
        Get
            Return {"PGI", "PFK", "FBA", "TPI", "GAPD", "PGK", "PGM", "ENO", "PYK"}
        End Get
    End Property

    ''' <summary>TCA 循环</summary>
    Private ReadOnly Property Tca As String()
        Get
            Return {"PDH", "CS", "ACON", "ICD", "AKGD", "SUCOAS", "SDH", "FUM", "MDH"}
        End Get
    End Property

    ''' <summary>呼吸链与氧化磷酸化</summary>
    Private ReadOnly Property Respiration As String()
        Get
            Return {"NDH", "CYTBO3", "ATPS4R"}
        End Get
    End Property

    ''' <summary>定义全部 4 个物种</summary>
    Public Function Definitions() As SpeciesDefinition()
        Dim all As MetabolicReaction() = SyntheticData.BuildReactions()

        Dim s1 As New SpeciesDefinition With {
            .id = GlcFermenter,
            .name = "葡萄糖发酵菌",
            .taxonomy = "Bacteria;Firmicutes;S1_glc_fermenter",
            .description = "初级生产者：糖酵解 + 混合酸发酵，唯一的氨基酸输出者",
            .Reactions = Glycolysis _
                .Concat(Tca) _
                .Concat(Respiration) _
                .Concat({"LDH", "ADH", "PTAR", "MAINT"}) _
                .Concat({"GDH"}) _
                .Concat({"PIT", "LACtex", "ETOHtex", "ACtex", "CO2tex"}) _
                .Concat({"AASec", "NH4t"}) _
                .ToArray(),
            .Fraction = 0.35,
            .MotilityFactor = 1.0,
            .Substrates = {"glc_e", "o2_e", "pi_e", "nh4_e"}
        }

        Dim s2 As New SpeciesDefinition With {
            .id = LactateUtilizer,
            .name = "乳酸利用菌",
            .taxonomy = "Bacteria;Proteobacteria;S2_lactate_utilizer",
            .description = "次级消费者：无法利用葡萄糖，靠乳酸氧化与丙酸发酵获取能量",
            .Reactions = GlycolysisCore _
                .Concat(Tca) _
                .Concat(Respiration) _
                .Concat({"LDH", "PTAR", "GDH", "MAINT"}) _
                .Concat({"PIT", "ACtex", "CO2tex"}) _
                .Concat({"LACupt", "PPM", "PPAtex", "NH4t"}) _
                .ToArray(),
            .Fraction = 0.25,
            .MotilityFactor = 0.85,
            .Substrates = {"lac_e", "o2_e", "pi_e", "nh4_e"}
        }

        Dim tcaWithoutPdh As String() = Tca.Where(Function(id) Not String.Equals(id, "PDH", StringComparison.OrdinalIgnoreCase)).ToArray()

        Dim s3 As New SpeciesDefinition With {
            .id = AcetateUtilizer,
            .name = "乙酸利用菌",
            .taxonomy = "Bacteria;Firmicutes;S3_acetate_utilizer",
            .description = "三级消费者：以乙酸为碳源、CO2 为补充碳源，产丁酸与氢气",
            .Reactions = tcaWithoutPdh _
                .Concat(Respiration) _
                .Concat({"PTAR", "GDH", "MAINT"}) _
                .Concat({"PIT", "CO2tex"}) _
                .Concat({"ACupt", "CO2upt", "BUTsyn", "BUTtex", "HYD", "H2tex", "NH4t"}) _
                .ToArray(),
            .Fraction = 0.25,
            .MotilityFactor = 0.7,
            .Substrates = {"ac_e", "co2_e", "o2_e", "pi_e", "nh4_e"}
        }

        Dim s4 As New SpeciesDefinition With {
            .id = AaAuxotroph,
            .name = "氨基酸营养缺陷型",
            .taxonomy = "Bacteria;Actinobacteria;S4_aa_auxotroph",
            .description = "缺失 GDH 的氨基酸缺陷型：摄取 S1 分泌的氨基酸，脱氨后把铵返还群落",
            .Reactions = Glycolysis _
                .Concat(Tca) _
                .Concat(Respiration) _
                .Concat({"LDH", "PTAR", "MAINT"}) _
                .Concat({"PIT", "LACtex", "ACtex", "CO2tex"}) _
                .Concat({"AAupt", "DESAM", "NH4out"}) _
                .ToArray(),
            .Fraction = 0.15,
            .MotilityFactor = 1.0,
            .Substrates = {"glc_e", "aa_e", "o2_e", "pi_e"}
        }

        Dim built As SpeciesDefinition() = {s1, s2, s3, s4}

        For Each def As SpeciesDefinition In built
            Dim reactions As MetabolicReaction() = SyntheticData.SelectReactions(all, def.Reactions)

            If reactions.Length <> def.Reactions.Length Then
                Dim found As New HashSet(Of String)(reactions.Select(Function(r) r.id), StringComparer.OrdinalIgnoreCase)
                Dim missing As String() = def.Reactions.Where(Function(id) Not found.Contains(id)).ToArray()

                Throw New InvalidOperationException($"物种 {def.id} 引用了不存在的反应: {String.Join(", ", missing)}")
            End If

            Call BuildBlueprint(def, reactions)
        Next

        Return built
    End Function

    ''' <summary>
    ''' 由反应子集装配出一个物种的蓝图（包含基因集、先验网络、表达矩阵与耦合映射）
    ''' </summary>
    Private Sub BuildBlueprint(def As SpeciesDefinition, reactions As MetabolicReaction())
        ' ---- 1. 基因集：转录因子 + 该物种实际用到的酶的编码基因 + 鞭毛/趋化 ----
        Dim genes As New List(Of String)(SyntheticData.TranscriptionFactors)
        Dim reactionGene As Dictionary(Of String, String) = SyntheticData.ReactionGeneMap()

        For Each rxn As MetabolicReaction In reactions
            Dim gene As String = Nothing

            If reactionGene.TryGetValue(rxn.id, gene) AndAlso gene IsNot Nothing Then
                genes.Add(gene)
            End If
        Next

        ' 运动与趋化能力：MotilityFactor = 0 的物种不带鞭毛基因（例如未来的非运动型）
        If def.MotilityFactor > 0 Then
            genes.AddRange(SyntheticData.FlagellarGenes)
            genes.AddRange(SyntheticData.ChemotaxisGenes)
        End If

        def.Genes = genes.Distinct(StringComparer.OrdinalIgnoreCase).ToArray()

        ' ---- 2. 该物种的转运/外排映射（只保留其反应中真正出现的胞外代谢物）----
        Dim external As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each rxn As MetabolicReaction In reactions
            For Each specie In rxn.left.SafeQuery
                If specie.ID.EndsWith("_e", StringComparison.OrdinalIgnoreCase) Then
                    Call external.Add(specie.ID)
                End If
            Next
            For Each specie In rxn.right.SafeQuery
                If specie.ID.EndsWith("_e", StringComparison.OrdinalIgnoreCase) Then
                    Call external.Add(specie.ID)
                End If
            Next
        Next

        ' 摄取 / 外排能力必须由该物种**实际拥有的反应**推导，否则会出现
        ' 「谁都能吃任何东西」的伪交叉喂养网络
        Dim transporters As Dictionary(Of String, String) = SyntheticData.DeriveTransporters(reactions)
        Dim exporters As Dictionary(Of String, String) = SyntheticData.DeriveExporters(reactions)

        ' ---- 3. 先验调控网络（裁剪到该物种的基因集）----
        Dim prior As PriorNetwork = SyntheticData.BuildPriorFor(def.Genes)

        ' ---- 4. 蓝图 ----
        def.Blueprint = New CellaBlueprint With {
            .SpeciesName = def.id,
            .Prior = prior,
            .Expression = SyntheticData.BuildExpression(def.Genes),
            .Reactions = reactions,
            .ExplicitBoundary = external.ToArray(),
            .ReactionGeneMap = reactionGene,
            .Transporters = transporters,
            .Exporters = exporters,
            .Effectors = SyntheticData.EffectorMap(),
            .ExternalStimuli = SyntheticData.StimulusMap(),
            .TFGenes = SyntheticData.TranscriptionFactors,
            .SignalChannels = SyntheticData.TranscriptionFactors,
            .RecycleTargetMetabolite = If(def.Genes.Contains("gdhA", StringComparer.OrdinalIgnoreCase), "aa_pool", "akg"),
            .GearsConfig = New GEARSConfig With {
                .EmbeddingDim = 16,
                .HiddenDim = 32,
                .NumLayers = 2,
                .Epochs = 15,
                .LearningRate = 0.003F,
                .NSinglePerturbation = 16,
                .NComboPerturbation = 8,
                .ComboSize = 2,
                .PrintEvery = 0,
                .Seed = 2024
            },
            .TransportVmax = 0.15,
            .StarvationThreshold = 0.5,
            .DivisionBiomassThreshold = 1.6,
            .BiomassYieldPerFlux = 0.02,
            .BiomassYieldPerProtein = 0.012,
            .MinDivisionAge = 3.0,
            .DaughterStateFraction = 0.5,
            .MaxCellAge = 120.0,
            .StarvationDeathTicks = 30,
            .MaxCellsPerSpot = 5,
            .FlagellarGenes = If(def.MotilityFactor > 0, SyntheticData.FlagellarGenes, Nothing),
            .ChemotaxisGenes = If(def.MotilityFactor > 0, SyntheticData.ChemotaxisGenes, Nothing),
            .MotilityBaseProbability = 0.12 * def.MotilityFactor,
            .MotilityGradientBias = 1.8,
            .MotilityGradientScale = 1.0,
            .NutrientMetabolites = def.Substrates,
            .DiffusionCoefficient = 0.08,
            .BoundaryFeedRate = 0.02,
            .BoundaryFeedMetabolites = {"glc_e"},
            .BoundaryFeedLevel = 1.0
        }
    End Sub

    ''' <summary>
    ''' 按 <see cref="SpeciesDefinition.Fraction"/> 的比例把群落接种到随机的格点上
    ''' </summary>
    Public Function SeedCommunity(env As Environment,
                                  defs As SpeciesDefinition(),
                                  Optional totalCells As Integer = 12,
                                  Optional seed As Integer = 2024) As Integer

        If env Is Nothing OrElse defs.IsNullOrEmpty Then
            Return 0
        End If

        Dim spots As Spot() = env.GetAllSpots().ToArray()

        If spots.Length = 0 Then
            Return 0
        End If

        Dim rand As New Random(seed)
        Dim weightSum As Double = defs.Sum(Function(d) d.Fraction)
        Dim counts As New List(Of (def As SpeciesDefinition, n As Integer))()
        Dim assigned As Integer = 0

        For i As Integer = 0 To defs.Length - 1
            Dim n As Integer = CInt(System.Math.Round(totalCells * defs(i).Fraction / weightSum))

            If i = defs.Length - 1 Then
                n = System.Math.Max(1, totalCells - assigned)
            End If

            n = System.Math.Max(0, n)
            assigned += n
            counts.Add((defs(i), n))
        Next

        Dim seeded As Integer = 0

        For Each item In counts
            For k As Integer = 1 To item.n
                Dim spot As Spot = spots(rand.Next(spots.Length))
                Dim cella As VirtualCella = CellaFactory.BuildCell(item.def.Blueprint, Nothing)

                Call CellaFactory.BindToSpot(cella, spot, env)
                seeded += 1
            Next
        Next

        Return seeded
        Return seeded
    End Function

End Module
