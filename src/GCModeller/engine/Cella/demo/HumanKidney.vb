' ============================================================
' HumanKidney.vb - 人肾类器官的细胞类型与代谢网络定义
' ============================================================
' 与细菌发酵 demo 的差异（都是刻意的建模取舍）：
'
'   1. 物种 = 细胞命运。8 种人肾细胞类型共用同一套「人细胞中心代谢」
'      骨架，差异体现在命运标志基因的表达程序与少量类型特异通路。
'   2. 生长因子不进反应网络。CHIR99021 / FGF2 / BMP4 / Activin A /
'      视黄酸 / GDNF / EGF 直接作为培养基成分 + 信号转导网络的胞外刺激，
'      再由 CellaBlueprint.SignalGeneCoupling 把通路活性接到靶基因上。
'   3. 分化谱系：NPC（后肾间充质肾单位祖细胞）是全能的起点，
'      可分化出足细胞 / 内皮 / 近端小管 / 远端小管 / 输尿管芽 / 间质，
'      输尿管芽再分化出集合管。
'   4. 肾单位各段在类器官里排成「核心 → 表层」的同心分带，
'      由 CellaBlueprint.PreferredRadius 描述，分化概率与位置校正都按它计算。
'
' 所有分子与数值都是合成名义值，用于演示建模流程与输出格式，
' 不构成对真实肾脏发育的定量预测。
' ============================================================

Imports Cella
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 一个外源生长因子：信号通道名 ↔ 培养基成分 id
''' </summary>
Public Class GrowthFactor

    Public Property channel As String
    Public Property medium As String
    Public Property name As String

End Class

''' <summary>一个肾细胞命运的定义（含几何偏好与分化规则参数）</summary>
Public Class KidneyFate

    Public Property id As String
    Public Property name As String
    Public Property Markers As String()
    Public Property PreferredRadius As Double
    Public Property RadialTolerance As Double
    Public Property SortingRate As Double
    ''' <summary>该命运额外的反应（在共享骨架之外）</summary>
    Public Property ExtraReactions As String()
    ''' <summary>分裂生物量阈值；终末分化细胞设很大表示不再增殖</summary>
    Public Property DivisionThreshold As Double = 2.5
    ''' <summary>分化基础概率</summary>
    Public Property BaseProbability As Double = 0.02
    ''' <summary>前体命运</summary>
    Public Property Precursors As String()
    Public Property RequiredSignals As Dictionary(Of String, Double)
    Public Property InducedBy As Dictionary(Of String, Double)
    Public Property InhibitedBy As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return $"{id} ({name})"
    End Function

End Class

Public Module HumanKidney

    ' ==================== 生长因子 ====================

    Public ReadOnly Property GrowthFactors As GrowthFactor()
        Get
            Return {
                New GrowthFactor With {.channel = "chir", .medium = "chir_e", .name = "CHIR99021 / Wnt 激动剂"},
                New GrowthFactor With {.channel = "fgf2", .medium = "fgf2_e", .name = "FGF2"},
                New GrowthFactor With {.channel = "bmp4", .medium = "bmp4_e", .name = "BMP4"},
                New GrowthFactor With {.channel = "activin", .medium = "activin_e", .name = "Activin A"},
                New GrowthFactor With {.channel = "ra", .medium = "ra_e", .name = "视黄酸 (RA)"},
                New GrowthFactor With {.channel = "gdnef", .medium = "gdnef_e", .name = "GDNF"},
                New GrowthFactor With {.channel = "egf", .medium = "egf_e", .name = "EGF"}
            }
        End Get
    End Property

    Public ReadOnly Property SignalChannels As String()
        Get
            Return GrowthFactors.Select(Function(g) g.channel).ToArray()
        End Get
    End Property

    ''' <summary>基础培养基成分（不含生长因子）</summary>
    Public ReadOnly Property BasalMedium As String()
        Get
            Return {"glc_e", "o2_e", "gln_e", "aa_e", "pi_e", "lac_e", "nh4_e", "hco3_e", "co2_e", "matrix_e"}
        End Get
    End Property

    ''' <summary>真正的生长底物（决定饥饿判定与趋化，不含产物与因子）</summary>
    Public ReadOnly Property Substrates As String()
        Get
            Return {"glc_e", "o2_e", "gln_e", "aa_e", "pi_e"}
        End Get
    End Property

    ' ==================== 转录因子 ====================

    Public ReadOnly Property TranscriptionFactors As String()
        Get
            Return {
                "SIX2", "WT1", "PAX2", "SALL1", "LHX1",
                "HNF4A", "HNF1B", "POU3F3", "GATA3",
                "HIF1A", "TCF7L2", "LEF1", "FOXD1"
            }
        End Get
    End Property

    ''' <summary>人细胞共享的基因（管家 + 中心代谢酶 + 黏附）</summary>
    Public Function BaseGenes() As String()
        Dim genes As New List(Of String)()

        genes.AddRange(TranscriptionFactors)
        ' 糖酵解
        genes.AddRange({"SLC2A1", "HK2", "GPI", "PFKP", "ALDOA", "TPI1", "GAPDH", "PGK1", "PGAM1", "ENO1", "PKM", "LDHA", "MCT4"})
        ' 线粒体
        genes.AddRange({"MPC", "CS", "ACO2", "IDH3A", "OGDH", "SUCLG1", "SDHA", "FH", "MDH2", "NDUFA1", "COX4I1", "ATP5F1A"})
        ' 谷氨酰胺 / 氮 / 酸碱
        genes.AddRange({"GLUL", "GLS", "GLUD1", "CA2", "SLC4A4", "SLC7A5"})
        ' 管家与结构
        genes.AddRange({"ACTB", "VIM", "PPIA", "RPL13A", "ITGB1", "CDH1", "ACTN1"})

        Return genes.ToArray()
    End Function

    ' ==================== 8 种肾细胞命运 ====================

    Public Function Fates() As KidneyFate()
        Dim list As New List(Of KidneyFate)()

        ' ---- 后肾间充质 / 肾单位祖细胞：唯一起点，位于类器官核心 ----
        list.Add(New KidneyFate With {
            .id = "npc",
            .name = "后肾间充质 / 肾单位祖细胞 (SIX2+)",
            .Markers = {"SIX2", "WT1", "PAX2", "SALL1", "LHX1", "CITED1"},
            .PreferredRadius = 0.18,
            .RadialTolerance = 0.30,
            .SortingRate = 0.02,
            .ExtraReactions = Nothing,
            .DivisionThreshold = 2.4,
            .BaseProbability = 0.0,
            .Precursors = Nothing
        })

        ' ---- 足细胞：与内皮共同构成肾小球样血管簇 ----
        list.Add(New KidneyFate With {
            .id = "podocyte",
            .name = "足细胞 (NPHS1+/PODXL+)",
            .Markers = {"NPHS1", "NPHS2", "PODXL", "SYNPO"},
            .PreferredRadius = 0.34,
            .RadialTolerance = 0.22,
            .SortingRate = 0.06,
            .ExtraReactions = Nothing,
            .DivisionThreshold = 1E+09,
            .BaseProbability = 0.035,
            .Precursors = {"npc"},
            .RequiredSignals = New Dictionary(Of String, Double) From {{"ra", 0.45}, {"fgf2", 0.30}},
            .InhibitedBy = New Dictionary(Of String, Double) From {{"podocyte", 2.0}}
        })

        ' ---- 内皮细胞：被足细胞分泌的 VEGFA 招募 ----
        list.Add(New KidneyFate With {
            .id = "endothelium",
            .name = "内皮细胞 (PECAM1+/CDH5+)",
            .Markers = {"PECAM1", "CDH5", "VEGFA", "FLT1", "KDR"},
            .PreferredRadius = 0.30,
            .RadialTolerance = 0.22,
            .SortingRate = 0.06,
            .ExtraReactions = Nothing,
            .DivisionThreshold = 1E+09,
            .BaseProbability = 0.030,
            .Precursors = {"npc"},
            .RequiredSignals = New Dictionary(Of String, Double) From {{"fgf2", 0.40}},
            .InducedBy = New Dictionary(Of String, Double) From {{"podocyte", 2.0}}
        })

        ' ---- 近端小管：重吸收主力，具糖异生与氨生成 ----
        list.Add(New KidneyFate With {
            .id = "proximal_tubule",
            .name = "近端小管 (SLC34A1+/LRP2+/CUBN+)",
            .Markers = {"SLC34A1", "LRP2", "CUBN", "ALDOB", "PC", "PCK1"},
            .PreferredRadius = 0.46,
            .RadialTolerance = 0.26,
            .SortingRate = 0.06,
            .ExtraReactions = {"PIT", "PC", "PCK1", "CUBN"},
            .DivisionThreshold = 1E+09,
            .BaseProbability = 0.042,
            .Precursors = {"npc"},
            .RequiredSignals = New Dictionary(Of String, Double) From {{"ra", 0.55}, {"fgf2", 0.25}},
            .InhibitedBy = New Dictionary(Of String, Double) From {{"proximal_tubule", 3.0}}
        })

        ' ---- 远端小管：被近端小管诱导出现 ----
        list.Add(New KidneyFate With {
            .id = "distal_tubule",
            .name = "远端小管 (SLC12A1+/UMOD+)",
            .Markers = {"SLC12A1", "UMOD", "KCNJ1", "POU3F3"},
            .PreferredRadius = 0.58,
            .RadialTolerance = 0.26,
            .SortingRate = 0.06,
            .ExtraReactions = {"PIT"},
            .DivisionThreshold = 1E+09,
            .BaseProbability = 0.038,
            .Precursors = {"npc"},
            .RequiredSignals = New Dictionary(Of String, Double) From {{"ra", 0.70}},
            .InducedBy = New Dictionary(Of String, Double) From {{"proximal_tubule", 2.0}}
        })

        ' ---- 输尿管芽：被 GDNF 与 NPC 共同诱导，可再分化为集合管 ----
        list.Add(New KidneyFate With {
            .id = "ureteric_bud",
            .name = "输尿管芽 (RET+/GATA3+/WNT11+)",
            .Markers = {"RET", "GATA3", "WNT11", "GDNF", "GFRA1"},
            .PreferredRadius = 0.66,
            .RadialTolerance = 0.22,
            .SortingRate = 0.05,
            .ExtraReactions = Nothing,
            .DivisionThreshold = 1E+09,
            .BaseProbability = 0.016,
            .Precursors = {"npc"},
            .RequiredSignals = New Dictionary(Of String, Double) From {{"gdnef", 0.50}},
            .InducedBy = New Dictionary(Of String, Double) From {{"npc", 2.0}}
        })

        ' ---- 集合管：输尿管芽的终末命运 ----
        list.Add(New KidneyFate With {
            .id = "collecting_duct",
            .name = "集合管 (AQP2+/CALB1+)",
            .Markers = {"AQP2", "CALB1", "HOXB7"},
            .PreferredRadius = 0.74,
            .RadialTolerance = 0.20,
            .SortingRate = 0.05,
            .ExtraReactions = Nothing,
            .DivisionThreshold = 1E+09,
            .BaseProbability = 0.090,
            .Precursors = {"ureteric_bud"},
            .RequiredSignals = New Dictionary(Of String, Double) From {{"gdnef", 0.30}, {"egf", 0.20}}
        })

        ' ---- 间质成纤维细胞：包被在最外层，分泌基质 ----
        list.Add(New KidneyFate With {
            .id = "interstitium",
            .name = "间质成纤维细胞 (FN1+/ACTA2+/COL1A1+)",
            .Markers = {"FN1", "ACTA2", "COL1A1", "PDGFRB", "POSTN"},
            .PreferredRadius = 0.88,
            .RadialTolerance = 0.18,
            .SortingRate = 0.08,
            .ExtraReactions = {"COLSyn", "COLsec"},
            .DivisionThreshold = 1E+09,
            .BaseProbability = 0.055,
            .Precursors = {"npc"},
            .RequiredSignals = New Dictionary(Of String, Double) From {{"bmp4", 0.30}},
            .InhibitedBy = New Dictionary(Of String, Double) From {{"npc", 3.0}}
        })

        Return list.ToArray()
    End Function

    ' ==================== 代谢反应网络 ====================

    ''' <summary>全部反应定义（各命运按子集取用）</summary>
    Public Function BuildReactions() As MetabolicReaction()
        Dim list As New List(Of MetabolicReaction)()

        ' ---- 葡萄糖摄取与糖酵解 ----
        list.Add(Rxn("GLUT1", "glucose transporter GLUT1", {"glc_e"}, {"glc"}))
        list.Add(Rxn("HK2", "hexokinase 2", {"glc", "atp"}, {"g6p", "adp"}))
        list.Add(Rxn("GPI", "glucose-6-phosphate isomerase", {"g6p"}, {"f6p"}, rev:=True))
        list.Add(Rxn("PFKP", "phosphofructokinase", {"f6p", "atp"}, {"fdp", "adp"}))
        list.Add(Rxn("ALDOA", "aldolase A", {"fdp"}, {"dhap", "gap"}, rev:=True))
        list.Add(Rxn("TPI1", "triosephosphate isomerase", {"dhap"}, {"gap"}, rev:=True))
        list.Add(Rxn("GAPDH", "glyceraldehyde-3-phosphate dehydrogenase", {"gap", "pi", "nad"}, {"_13bpg", "nadh"}, rev:=True))
        list.Add(Rxn("PGK1", "phosphoglycerate kinase", {"_13bpg", "adp"}, {"_3pg", "atp"}, rev:=True))
        list.Add(Rxn("PGAM1", "phosphoglycerate mutase", {"_3pg"}, {"_2pg"}, rev:=True))
        list.Add(Rxn("ENO1", "enolase 1", {"_2pg"}, {"pep", "h2o"}, rev:=True))
        list.Add(Rxn("PKM", "pyruvate kinase M", {"pep", "adp"}, {"pyr", "atp"}))
        list.Add(Rxn("LDHA", "lactate dehydrogenase A", {"pyr", "nadh"}, {"lac", "nad"}, rev:=True))
        list.Add(Rxn("MCT4", "lactate exporter MCT4", {"lac"}, {"lac_e"}))

        ' ---- 线粒体：丙酮酸氧化、TCA、呼吸链、氧化磷酸化 ----
        list.Add(Rxn("MPC", "pyruvate dehydrogenase complex", {"pyr", "coa", "nad"}, {"accoa", "co2", "nadh"}))
        list.Add(Rxn("CS", "citrate synthase", {"accoa", "oaa"}, {"cit", "coa"}))
        list.Add(Rxn("ACO2", "aconitase 2", {"cit"}, {"icit"}, rev:=True))
        list.Add(Rxn("IDH3A", "isocitrate dehydrogenase 3", {"icit", "nad"}, {"akg", "co2", "nadh"}))
        list.Add(Rxn("OGDH", "alpha-ketoglutarate dehydrogenase", {"akg", "coa", "nad"}, {"succoa", "co2", "nadh"}))
        list.Add(Rxn("SUCLG1", "succinyl-CoA ligase", {"succoa", "adp"}, {"succ", "coa", "atp"}, rev:=True))
        list.Add(Rxn("SDHA", "succinate dehydrogenase", {"succ", "q8"}, {"fum", "q8h2"}, rev:=True))
        list.Add(Rxn("FH", "fumarase", {"fum", "h2o"}, {"mal"}, rev:=True))
        list.Add(Rxn("MDH2", "malate dehydrogenase 2", {"mal", "nad"}, {"oaa", "nadh"}, rev:=True))
        list.Add(Rxn("NDUFA1", "complex I", {"nadh", "q8"}, {"nad", "q8h2"}))
        list.Add(Rxn("COX4I1", "complex IV", {"q8h2", "o2_e"}, {"q8", "h2o"}))
        list.Add(Rxn("ATP5F1A", "ATP synthase", {"adp", "pi"}, {"atp"}))
        list.Add(Rxn("MAINT", "ATP maintenance", {"atp"}, {"adp", "pi"}))

        ' ---- 谷氨酰胺分解 / 氨生成 / 酸碱平衡（肾脏的核心生理功能）----
        list.Add(Rxn("GLNupt", "glutamine transporter", {"gln_e"}, {"gln"}))
        list.Add(Rxn("GLS", "glutaminase", {"gln"}, {"glu", "nh4"}))
        list.Add(Rxn("GLUD1", "glutamate dehydrogenase", {"glu", "nad"}, {"akg", "nh4"}, rev:=True))
        list.Add(Rxn("GLUL", "glutamine synthetase", {"glu", "nh4", "atp"}, {"gln", "adp", "pi"}))
        list.Add(Rxn("NH4out", "ammonium exporter", {"nh4"}, {"nh4_e"}))
        list.Add(Rxn("CA2", "carbonic anhydrase II", {"co2", "h2o"}, {"hco3"}, rev:=True))
        list.Add(Rxn("HCO3out", "bicarbonate exporter", {"hco3"}, {"hco3_e"}))
        list.Add(Rxn("CO2tex", "carbon dioxide export", {"co2"}, {"co2_e"}))
        list.Add(Rxn("AAupt", "amino acid transporter", {"aa_e"}, {"aa_pool"}))
        list.Add(Rxn("AASyn", "amino acid pool interconversion", {"akg", "nh4"}, {"aa_pool"}, rev:=True))

        ' ---- 类型特异通路 ----
        list.Add(Rxn("PIT", "phosphate reabsorption (SLC34A1)", {"pi_e"}, {"pi"}))
        list.Add(Rxn("PC", "pyruvate carboxylase (gluconeogenesis)", {"pyr", "co2", "atp"}, {"oaa", "adp", "pi"}))
        list.Add(Rxn("PCK1", "PEP carboxykinase", {"oaa", "atp"}, {"pep", "adp", "co2"}))
        list.Add(Rxn("CUBN", "receptor-mediated protein reabsorption (CUBN/LRP2)", {"aa_e"}, {"aa_pool"}))
        list.Add(Rxn("COLSyn", "collagen synthesis", {"aa_pool", "atp"}, {"matrix", "adp", "pi"}))
        list.Add(Rxn("COLsec", "matrix secretion", {"matrix"}, {"matrix_e"}))

        Return list.ToArray()
    End Function

    ''' <summary>
    ''' 共享代谢骨架：所有 8 种细胞命运都具备的中心代谢 + 氮/酸碱处理。
    ''' 类型特异反应由 <see cref="KidneyFate.ExtraReactions"/> 追加。
    ''' </summary>
    Public ReadOnly Property CoreReactions As String()
        Get
            Return {
                "GLUT1", "HK2", "GPI", "PFKP", "ALDOA", "TPI1", "GAPDH", "PGK1",
                "PGAM1", "ENO1", "PKM", "LDHA", "MCT4", "MPC",
                "CS", "ACO2", "IDH3A", "OGDH", "SUCLG1", "SDHA", "FH", "MDH2",
                "NDUFA1", "COX4I1", "ATP5F1A", "MAINT",
                "GLNupt", "GLS", "GLUD1", "GLUL", "NH4out", "CA2", "HCO3out", "CO2tex",
                "AAupt", "AASyn"
            }
        End Get
    End Property

    ' ==================== 调控网络 ====================

    ''' <summary>人肾发育的 TF → 靶基因先验调控网络</summary>
    Public Function BuildPrior(genes As String()) As PriorNetwork
        Dim full As New PriorNetwork()

        Call AddAll(full, "SIX2", {"SALL1", "CITED1", "WT1", "PAX2"}, Effector.Activator, 0.9, "nephron progenitor core")
        Call AddAll(full, "SALL1", {"LHX1", "PAX2", "SIX2"}, Effector.Activator, 0.75, "progenitor maintenance")
        Call AddAll(full, "LHX1", {"PAX2", "WT1"}, Effector.Activator, 0.7, "nephron patterning")
        Call AddAll(full, "PAX2", {"LHX1", "WNT11", "RET", "GATA3", "SLC2A1", "HK2"}, Effector.Activator, 0.8, "UB & progenitor")
        Call AddAll(full, "WT1", {"NPHS1", "NPHS2", "PODXL", "SYNPO"}, Effector.Activator, 0.9, "podocyte program")
        Call AddAll(full, "HNF4A", {"SLC34A1", "LRP2", "CUBN", "ALDOB", "PC", "PCK1"}, Effector.Activator, 0.9, "proximal tubule program")
        Call AddAll(full, "HNF1B", {"AQP2", "CALB1", "UMOD", "SLC12A1", "KCNJ1"}, Effector.Activator, 0.8, "tubule maturation")
        Call AddAll(full, "POU3F3", {"UMOD", "KCNJ1"}, Effector.Activator, 0.75, "distal tubule")
        Call AddAll(full, "GATA3", {"RET", "WNT11", "AQP2"}, Effector.Activator, 0.8, "ureteric bud")
        Call AddAll(full, "FOXD1", {"COL1A1", "ACTA2", "PDGFRB", "FN1", "POSTN"},
                    Effector.Activator, 0.85, "stromal program")
        Call AddAll(full, "HIF1A", {"SLC2A1", "LDHA", "MCT4", "PKM", "VEGFA", "COX4I1"},
                    Effector.Activator, 0.8, "hypoxia response")
        Call AddAll(full, "TCF7L2", {"SIX2", "PAX2", "LEF1", "SALL1", "CTNNB1"},
                    Effector.Activator, 0.75, "Wnt signalling")
        Call AddAll(full, "LEF1", {"SIX2", "PAX2"}, Effector.Activator, 0.7, "Wnt target")
        Call AddAll(full, "SIX2", {"GLS", "GLUD1"}, Effector.Inhibitor, 0.5, "metabolic quiescence")

        ' 裁剪到该命运的基因集（GEARS 按基因名匹配）
        Return Filter(full, genes)
    End Function

    Private Sub AddAll(prior As PriorNetwork, tf As String, targets As String(),
                       effector As Effector, confidence As Double, evidence As String)
        For Each g As String In targets
            Call prior.AddEdge(tf, g, effector, confidence, "synthetic:" & evidence)
        Next
    End Sub

    Private Function Filter(full As PriorNetwork, genes As String()) As PriorNetwork
        Dim index As New HashSet(Of String)(genes, StringComparer.OrdinalIgnoreCase)
        Dim prior As New PriorNetwork()

        For Each edge In full.Edges
            If index.Contains(edge.TF) AndAlso index.Contains(edge.TargetGene) Then
                Call prior.AddEdge(edge.TF, edge.TargetGene, edge.RegulationType, edge.Confidence, edge.Evidence)
            End If
        Next

        Return prior
    End Function

    ''' <summary>
    ''' 生长因子通路 → 靶基因。权重表示该因子通路对该基因的直接影响强度。
    ''' </summary>
    Public Function SignalGeneCoupling() As Dictionary(Of String, Dictionary(Of String, Double))
        Dim map As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.OrdinalIgnoreCase)

        ' 每项写法为 "基因:权重"
        Call Couple(map, "chir", "TCF7L2:0.9", "LEF1:0.9", "SIX2:0.5", "PAX2:0.6", "SALL1:0.4")
        Call Couple(map, "fgf2", "SIX2:0.6", "WT1:0.4", "HIF1A:0.3", "VEGFA:0.5", "SLC2A1:0.3")
        Call Couple(map, "bmp4", "SALL1:0.5", "WT1:0.3", "ACTA2:0.5", "COL1A1:0.4")
        Call Couple(map, "activin", "SIX2:0.3", "LHX1:0.5", "HNF4A:0.3", "FOXD1:0.3")
        Call Couple(map, "ra", "HNF4A:0.8", "HNF1B:0.4", "POU3F3:0.4", "NPHS1:0.5", "SLC34A1:0.5")
        Call Couple(map, "gdnef", "RET:0.9", "GATA3:0.7", "WNT11:0.6", "GFRA1:0.5")
        Call Couple(map, "egf", "HNF1B:0.4", "AQP2:0.5", "CALB1:0.4")

        Return map
    End Function

    Private Sub Couple(map As Dictionary(Of String, Dictionary(Of String, Double)),
                       channel As String, ParamArray targets As String())
        Dim inner As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        For Each item As String In targets
            Dim parts As String() = item.Split(":"c)

            inner(parts(0)) = Double.Parse(parts(1), Globalization.CultureInfo.InvariantCulture)
        Next

        map(channel) = inner
    End Sub

    ''' <summary>生态位信号 → 基因：把空间位置与群体接触强度接到标志基因上</summary>
    Public Function NicheGeneCoupling() As Dictionary(Of String, Dictionary(Of String, Double))
        Dim map As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.OrdinalIgnoreCase)
        Dim outer As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        Dim crowding As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        ' 径向位置越高（越靠表层）→ 祖细胞程序下调、分化程序上调
        For Each g As String In {"SIX2", "CITED1", "SALL1"}
            outer(g) = -0.6
        Next
        For Each g As String In {"HNF4A", "UMOD", "AQP2", "COL1A1"}
            outer(g) = 0.5
        Next

        ' 局部拥挤 / 异质接触促进分化（Notch-Delta 下游 HES1 类抑制祖细胞程序）
        For Each g As String In {"SIX2", "CITED1"}
            crowding(g) = -0.5
        Next

        map("radial_position") = outer
        map("local_density") = crowding

        Return map
    End Function

    ' ==================== 基线表达 ====================

    ''' <summary>
    ''' 某个命运的基线表达矩阵：代谢与管家基因中等表达，本命运的标志基因
    ''' 高表达、其他命运的标志基因低表达 —— 分化时切换蓝图即可完成转录程序重编程。
    ''' </summary>
    Public Function BuildExpression(genes As String(), fate As KidneyFate,
                                    Optional nSamples As Integer = 6,
                                    Optional seed As Integer = 2024) As GeneExpressionData
        Dim rand As New Random(seed)
        Dim n As Integer = genes.Length
        Dim m As Integer = System.Math.Max(4, nSamples)
        Dim matrix As Double(,) = New Double(n - 1, m - 1) {}
        Dim samples As String() = New String(m - 1) {}
        Dim times As Double() = New Double(m - 1) {}

        For j As Integer = 0 To m - 1
            samples(j) = $"ctrl_{j + 1}"
            times(j) = 0
        Next

        Dim allFates As KidneyFate() = Fates()

        For i As Integer = 0 To n - 1
            Dim gene As String = genes(i)
            Dim level As Double = 12.0

            If TranscriptionFactors.Contains(gene, StringComparer.OrdinalIgnoreCase) Then
                level = 6.0
            End If

            ' 本命运的标志基因高表达，其它命运的标志基因被压低
            Dim isOwnMarker As Boolean = Contains(fate.Markers, gene)

            If isOwnMarker Then
                level *= 3.0
            ElseIf allFates.Any(Function(f) Contains(f.Markers, gene)) Then
                level *= 0.25
            End If

            For j As Integer = 0 To m - 1
                matrix(i, j) = level * (1.0 + (rand.NextDouble() - 0.5) * 0.15)
            Next
        Next

        Return New GeneExpressionData With {
            .GeneNames = genes,
            .SampleNames = samples,
            .Matrix = matrix,
            .TimePoints = times
        }
    End Function

    Private Function Contains(set_ As String(), value As String) As Boolean
        Return Not set_.IsNullOrEmpty AndAlso set_.Contains(value, StringComparer.OrdinalIgnoreCase)
    End Function

    ' ==================== 蓝图与命运目录 ====================

    Public Function BuildBlueprint(fate As KidneyFate,
                                   all As MetabolicReaction(),
                                   Optional gearsEpochs As Integer = 12,
                                   Optional metabolicEpochs As Integer = 20) As CellaBlueprint

        ' ---- 1. 基因集：基础基因 + 本命运的额外酶与标志基因 ----
        Dim reactionGene As Dictionary(Of String, String) = ReactionGeneTable()
        Dim ids As String() = CoreReactions.Concat(fate.ExtraReactions.SafeQuery).ToArray()
        Dim reactions As MetabolicReaction() = all.Where(Function(r) ids.Contains(r.id, StringComparer.OrdinalIgnoreCase)).ToArray()

        If reactions.Length <> ids.Length Then
            Dim found As New HashSet(Of String)(reactions.Select(Function(r) r.id), StringComparer.OrdinalIgnoreCase)
            Dim missing As String() = ids.Where(Function(id) Not found.Contains(id)).ToArray()

            Throw New InvalidOperationException($"命运 {fate.id} 引用了不存在的反应: {String.Join(", ", missing)}")
        End If

        Dim genes As New List(Of String)(BaseGenes())

        For Each rxn As MetabolicReaction In reactions
            Dim gene As String = Nothing

            If reactionGene.TryGetValue(rxn.id, gene) AndAlso gene IsNot Nothing Then
                genes.Add(gene)
            End If
        Next

        genes.AddRange(fate.Markers)

        Dim geneSet As String() = genes.Distinct(StringComparer.OrdinalIgnoreCase).ToArray()

        ' ---- 2. 摄取 / 外排能力完全由该命运实际拥有的反应推导 ----
        Dim transporters As Dictionary(Of String, String) = SyntheticData.DeriveTransporters(reactions)
        Dim exporters As Dictionary(Of String, String) = SyntheticData.DeriveExporters(reactions)

        ' ---- 3. 蓝图 ----
        Dim blueprint As New CellaBlueprint With {
            .SpeciesName = fate.id,
            .DisplayName = fate.name,
            .Markers = fate.Markers,
            .PreferredRadius = fate.PreferredRadius,
            .RadialTolerance = fate.RadialTolerance,
            .RadialSortingRate = fate.SortingRate,
            .Prior = BuildPrior(geneSet),
            .Expression = BuildExpression(geneSet, fate),
            .Reactions = reactions,
            .ExplicitBoundary = BoundaryOf(reactions),
            .ReactionGeneMap = reactionGene,
            .Transporters = transporters,
            .Exporters = exporters,
            .Effectors = New Dictionary(Of String, String)(),
            .SignalChannels = SignalChannels,
            .SignalGeneCoupling = SignalGeneCoupling(),
            .NicheGeneCoupling = NicheGeneCoupling(),
            .ExternalStimuli = ExternalStimuli(),
            .TFGenes = TranscriptionFactors,
            .NutrientMetabolites = Substrates,
            .RecycleTargetMetabolite = "aa_pool",
            .GearsConfig = New SMRUCC.genomics.Analysis.GEARS.GEARSConfig With {
                .EmbeddingDim = 16,
                .HiddenDim = 32,
                .NumLayers = 2,
                .Epochs = gearsEpochs,
                .LearningRate = 0.003F,
                .NSinglePerturbation = 16,
                .NComboPerturbation = 8,
                .ComboSize = 2,
                .PrintEvery = 0,
                .Seed = 2024
            },
            .DivisionBiomassThreshold = fate.DivisionThreshold,
            .BiomassYieldPerFlux = 0.02,
            .BiomassYieldPerProtein = 0.012,
            .MinDivisionAge = 3.0,
            .DaughterStateFraction = 0.5,
            .MaxCellAge = 600.0,
            .StarvationThreshold = 0.35,
            .StarvationDeathTicks = 24,
            .MaxCellsPerSpot = 3,
            .DaughterDispersal = True,
            .TransportVmax = 0.12,
            .MotilityBaseProbability = 0.0,
            .FlagellarGenes = Nothing,
            .ChemotaxisGenes = Nothing,
            .DiffusionCoefficient = 0.22,
            .BoundaryFeedRate = 0.0,
            .BoundaryFeedMetabolites = Nothing
        }

        ' 训练时用的 MetaboliteRelease 数据在 Organoid 主流程里单独准备
        Return blueprint
    End Function

    ''' <summary>反应 → 催化基因（每个反应一个代表基因）</summary>
    Public Function ReactionGeneTable() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
            {"GLUT1", "SLC2A1"}, {"HK2", "HK2"}, {"GPI", "GPI"}, {"PFKP", "PFKP"},
            {"ALDOA", "ALDOA"}, {"TPI1", "TPI1"}, {"GAPDH", "GAPDH"}, {"PGK1", "PGK1"},
            {"PGAM1", "PGAM1"}, {"ENO1", "ENO1"}, {"PKM", "PKM"}, {"LDHA", "LDHA"},
            {"MCT4", "MCT4"}, {"MPC", "MPC"}, {"CS", "CS"}, {"ACO2", "ACO2"},
            {"IDH3A", "IDH3A"}, {"OGDH", "OGDH"}, {"SUCLG1", "SUCLG1"}, {"SDHA", "SDHA"},
            {"FH", "FH"}, {"MDH2", "MDH2"}, {"NDUFA1", "NDUFA1"}, {"COX4I1", "COX4I1"},
            {"ATP5F1A", "ATP5F1A"}, {"MAINT", "ATP5F1A"},
            {"GLNupt", "SLC7A5"}, {"GLS", "GLS"}, {"GLUD1", "GLUD1"}, {"GLUL", "GLUL"},
            {"NH4out", "SLC4A4"}, {"CA2", "CA2"}, {"HCO3out", "SLC4A4"}, {"CO2tex", "CA2"},
            {"AAupt", "SLC7A5"}, {"AASyn", "GLUD1"},
            {"PIT", "SLC34A1"}, {"PC", "PC"}, {"PCK1", "PCK1"}, {"CUBN", "CUBN"},
            {"COLSyn", "COL1A1"}, {"COLsec", "COL1A1"}
        }
    End Function

    Private Function BoundaryOf(reactions As MetabolicReaction()) As String()
        Dim external As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each rxn As MetabolicReaction In reactions
            For Each reactant As CompoundSpecieReference In rxn.left.SafeQuery
                If reactant.ID.EndsWith("_e", StringComparison.OrdinalIgnoreCase) Then
                    Call external.Add(reactant.ID)
                End If
            Next
            For Each product As CompoundSpecieReference In rxn.right.SafeQuery
                If product.ID.EndsWith("_e", StringComparison.OrdinalIgnoreCase) Then
                    Call external.Add(product.ID)
                End If
            Next
        Next

        Return external.ToArray()
    End Function

    ''' <summary>信号通道 → 触发它的培养基成分</summary>
    Public Function ExternalStimuli() As Dictionary(Of String, String)
        Dim map As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        For Each gf As GrowthFactor In GrowthFactors
            map(gf.channel) = gf.medium
        Next

        Return map
    End Function

    ''' <summary>构建完整的命运目录与蓝图</summary>
    Public Function BuildCatalog(Optional gearsEpochs As Integer = 12,
                                 Optional metabolicEpochs As Integer = 20) As FateCatalog

        Dim all As MetabolicReaction() = BuildReactions()
        Dim catalog As New FateCatalog()

        For Each fate As KidneyFate In Fates()
            Dim blueprint As CellaBlueprint = BuildBlueprint(fate, all, gearsEpochs, metabolicEpochs)

            catalog.Add(New CellFateDefinition With {
                .Id = fate.id,
                .Name = fate.name,
                .Blueprint = blueprint,
                .Precursors = fate.Precursors,
                .MinGeneration = If(fate.Precursors.IsNullOrEmpty, 0, 1),
                .RequiredSignals = fate.RequiredSignals,
                .InducedBy = fate.InducedBy,
                .InhibitedBy = fate.InhibitedBy,
                .BaseProbability = fate.BaseProbability,
                .BiomassRetention = 0.65
            })
        Next

        Return catalog
    End Function

    Private Function Rxn(id As String, name As String, left As String(), right As String(),
                         Optional rev As Boolean = False) As MetabolicReaction
        Return New MetabolicReaction With {
            .id = id,
            .name = name,
            .description = name,
            .left = left.Select(Function(x) New CompoundSpecieReference(1.0, x)).ToArray(),
            .right = right.Select(Function(x) New CompoundSpecieReference(1.0, x)).ToArray(),
            .is_reversible = rev,
            .is_spontaneous = False,
            .ECNumbers = {}
        }
    End Function

End Module
