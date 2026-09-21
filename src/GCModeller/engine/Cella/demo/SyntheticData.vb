' ============================================================
' SyntheticData.vb - 演示用合成数据
' ============================================================
' 全部数据都在代码里现场合成，不依赖任何外部文件：
'
'   1. 一张「中心碳代谢」反应网络（糖酵解 + TCA + 呼吸链 + 副产物分泌
'      + 氨基酸池），边界代谢物以 _e 结尾表示胞外。
'   2. 一个手写的 TF → 靶基因 先验调控网络（活化 / 抑制 + 置信度）。
'   3. 一份基线表达矩阵（用于估计 GEARS 的野生型均值与标准差）。
'   4. 一套用于拟合 Metaboliq 的时序代谢 / 酶 / 边界数据。
'
' 注意：这些数字只用于把整条流水线跑通并展示输出格式，不具生物学意义。
' ============================================================

Imports Cella
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.Metaboliq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel

Public Module SyntheticData

    ''' <summary>转录因子</summary>
    Public ReadOnly Property TranscriptionFactors As String()
        Get
            Return {"crp", "fis", "arcA", "fnr", "codY", "luxR"}
        End Get
    End Property

    ''' <summary>全部基因 = 转录因子 + 代谢酶 + 转运蛋白</summary>
    Public Function GeneSet() As String()
        Dim genes As New List(Of String)()

        genes.AddRange(TranscriptionFactors)
        genes.AddRange({"ptsG", "glk", "pgi", "pfkA", "fbaA", "tpiA", "gapA", "pgk", "pgm", "eno", "pykA"})
        genes.AddRange({"aceE", "gltA", "acnA", "icd", "sucA", "sucC", "sdhA", "fumA", "mdh"})
        genes.AddRange({"ndh", "cytbo3", "atps4r"})
        genes.AddRange({"ldhA", "adhE", "ackA"})
        genes.AddRange({"cyoA", "lacY", "atoD", "pitA"})
        genes.AddRange({"gdhA"})

        Return genes.ToArray()
    End Function

    ''' <summary>胞外（边界）代谢物</summary>
    Public Function BoundaryMetabolites() As String()
        Return {"glc_e", "o2_e", "pi_e", "lac_e", "etoh_e", "ac_e", "co2_e"}
    End Function

    ' ==================== 代谢反应网络 ====================

    Public Function BuildReactions() As MetabolicReaction()
        Dim list As New List(Of MetabolicReaction)()

        ' ---- 糖酵解 ----
        list.Add(Rxn("PTS", "glucose PTS uptake", {"glc_e", "pep"}, {"g6p", "pyr"}))
        list.Add(Rxn("GLK", "glucokinase", {"glc_e", "atp"}, {"g6p", "adp"}))
        list.Add(Rxn("PGI", "glucose-6-phosphate isomerase", {"g6p"}, {"f6p"}, rev:=True))
        list.Add(Rxn("PFK", "phosphofructokinase", {"f6p", "atp"}, {"fdp", "adp"}))
        list.Add(Rxn("FBA", "fructose-bisphosphate aldolase", {"fdp"}, {"dhap", "gap"}, rev:=True))
        list.Add(Rxn("TPI", "triose-phosphate isomerase", {"dhap"}, {"gap"}, rev:=True))
        list.Add(Rxn("GAPD", "glyceraldehyde-3-phosphate dehydrogenase", {"gap", "pi", "nad"}, {"_13dpg", "nadh"}, rev:=True))
        list.Add(Rxn("PGK", "phosphoglycerate kinase", {"_13dpg", "adp"}, {"_3pg", "atp"}, rev:=True))
        list.Add(Rxn("PGM", "phosphoglycerate mutase", {"_3pg"}, {"_2pg"}, rev:=True))
        list.Add(Rxn("ENO", "enolase", {"_2pg"}, {"pep", "h2o"}, rev:=True))
        list.Add(Rxn("PYK", "pyruvate kinase", {"pep", "adp"}, {"pyr", "atp"}))

        ' ---- 丙酮酸 → 乙酰辅酶A ----
        list.Add(Rxn("PDH", "pyruvate dehydrogenase", {"pyr", "coa", "nad"}, {"accoa", "co2", "nadh"}))

        ' ---- TCA 循环 ----
        list.Add(Rxn("CS", "citrate synthase", {"accoa", "oaa"}, {"cit", "coa"}))
        list.Add(Rxn("ACON", "aconitase", {"cit"}, {"icit"}, rev:=True))
        list.Add(Rxn("ICD", "isocitrate dehydrogenase", {"icit", "nad"}, {"akg", "co2", "nadh"}))
        list.Add(Rxn("AKGD", "alpha-ketoglutarate dehydrogenase", {"akg", "coa", "nad"}, {"succoa", "co2", "nadh"}))
        list.Add(Rxn("SUCOAS", "succinyl-CoA synthetase", {"succoa", "adp"}, {"succ", "coa", "atp"}, rev:=True))
        list.Add(Rxn("SDH", "succinate dehydrogenase", {"succ", "q8"}, {"fum", "q8h2"}, rev:=True))
        list.Add(Rxn("FUM", "fumarase", {"fum", "h2o"}, {"mal"}, rev:=True))
        list.Add(Rxn("MDH", "malate dehydrogenase", {"mal", "nad"}, {"oaa", "nadh"}, rev:=True))

        ' ---- 呼吸链与氧化磷酸化 ----
        list.Add(Rxn("NDH", "NADH dehydrogenase", {"nadh", "q8"}, {"nad", "q8h2"}))
        list.Add(Rxn("CYTBO3", "cytochrome bo3 terminal oxidase", {"q8h2", "o2_e"}, {"q8", "h2o"}))
        list.Add(Rxn("ATPS4R", "ATP synthase", {"adp", "pi"}, {"atp"}))

        ' ---- 副产物分泌 ----
        list.Add(Rxn("LDH", "lactate dehydrogenase", {"pyr", "nadh"}, {"lac", "nad"}, rev:=True))
        list.Add(Rxn("ADH", "alcohol dehydrogenase", {"accoa", "nadh"}, {"etoh", "nad", "coa"}, rev:=True))
        list.Add(Rxn("PTAR", "acetate kinase", {"accoa", "adp"}, {"ac", "atp", "coa"}, rev:=True))

        ' ---- 氨基酸池（周转系统的回收物去向）----
        list.Add(Rxn("GDH", "glutamate dehydrogenase", {"akg", "nh4"}, {"aa_pool"}, rev:=True))

        ' ---- 跨膜转运 ----
        list.Add(Rxn("PIT", "phosphate uptake", {"pi_e"}, {"pi"}))
        list.Add(Rxn("LACtex", "lactate export", {"lac"}, {"lac_e"}))
        list.Add(Rxn("ETOHtex", "ethanol export", {"etoh"}, {"etoh_e"}))
        list.Add(Rxn("ACtex", "acetate export", {"ac"}, {"ac_e"}))
        list.Add(Rxn("CO2tex", "carbon dioxide export", {"co2"}, {"co2_e"}))

        Return list.ToArray()
    End Function

    Private Function Species(id As String) As CompoundSpecieReference
        Return New CompoundSpecieReference(1.0, id)
    End Function

    Private Function Rxn(id As String, name As String, left As String(), right As String(),
                         Optional rev As Boolean = False) As MetabolicReaction
        Return New MetabolicReaction With {
            .id = id,
            .name = name,
            .description = name,
            .left = left.Select(AddressOf Species).ToArray(),
            .right = right.Select(AddressOf Species).ToArray(),
            .is_reversible = rev,
            .is_spontaneous = False,
            .ECNumbers = {}
        }
    End Function

    ' ==================== 耦合映射 ====================

    ''' <summary>反应 id → 催化该反应的基因 id</summary>
    Public Function ReactionGeneMap() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
            {"PTS", "ptsG"}, {"GLK", "glk"}, {"PGI", "pgi"}, {"PFK", "pfkA"},
            {"FBA", "fbaA"}, {"TPI", "tpiA"}, {"GAPD", "gapA"}, {"PGK", "pgk"},
            {"PGM", "pgm"}, {"ENO", "eno"}, {"PYK", "pykA"}, {"PDH", "aceE"},
            {"CS", "gltA"}, {"ACON", "acnA"}, {"ICD", "icd"}, {"AKGD", "sucA"},
            {"SUCOAS", "sucC"}, {"SDH", "sdhA"}, {"FUM", "fumA"}, {"MDH", "mdh"},
            {"NDH", "ndh"}, {"CYTBO3", "cytbo3"}, {"ATPS4R", "atps4r"},
            {"LDH", "ldhA"}, {"ADH", "adhE"}, {"PTAR", "ackA"}, {"GDH", "gdhA"},
            {"PIT", "pitA"}, {"LACtex", "lacY"}, {"ETOHtex", "adhE"}, {"ACtex", "atoD"}
        }
    End Function

    ''' <summary>边界代谢物 → 转运蛋白基因</summary>
    Public Function TransporterMap() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
            {"glc_e", "ptsG"}, {"o2_e", "cyoA"}, {"pi_e", "pitA"},
            {"lac_e", "lacY"}, {"etoh_e", "adhE"}, {"ac_e", "atoD"}
        }
    End Function

    ''' <summary>边界代谢物 → 与之对应的胞内代谢物（产物外排的来源）</summary>
    Public Function ExporterMap() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
            {"lac_e", "lac"}, {"etoh_e", "etoh"}, {"ac_e", "ac"}, {"co2_e", "co2"}
        }
    End Function

    ''' <summary>代谢物效应物 → 受其调控的转录因子基因</summary>
    Public Function EffectorMap() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
            {"atp", "codY"}, {"nadh", "arcA"}, {"akg", "crp"}, {"aa_pool", "fis"}
        }
    End Function

    ''' <summary>信号通道（= 转录因子） → 触发它的胞外代谢物</summary>
    Public Function StimulusMap() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
            {"crp", "glc_e"}, {"fis", "glc_e"}, {"arcA", "o2_e"},
            {"fnr", "o2_e"}, {"codY", "ac_e"}, {"luxR", "lac_e"}
        }
    End Function

    ' ==================== 先验调控网络 ====================

    ''' <summary>
    ''' TF → 靶基因 的先验调控网络（直接作为 GEARS 图神经网络的图结构）
    ''' </summary>
    Public Function BuildPrior() As PriorNetwork
        Dim prior As New PriorNetwork()

        ' 全局调控因子 CRP：碳源充足时活化糖酵解与摄取
        For Each g As String In {"ptsG", "glk", "pgi", "pfkA", "pykA", "cyoA"}
            Call prior.AddEdge("crp", g, Effector.Activator, 0.9, "synthetic:carbon catabolite activation")
        Next
        Call prior.AddEdge("crp", "gltA", Effector.Inhibitor, 0.6, "synthetic:overflow metabolism")
        Call prior.AddEdge("crp", "fis", Effector.Activator, 0.7, "synthetic:TF cascade")

        ' FIS：生长旺盛期活化下游糖酵解
        For Each g As String In {"gapA", "pgk", "tpiA", "eno"}
            Call prior.AddEdge("fis", g, Effector.Activator, 0.8, "synthetic:growth phase")
        Next

        ' ArcA：微好氧/厌氧时抑制 TCA 与呼吸链
        For Each g As String In {"gltA", "acnA", "icd", "sucA", "sdhA", "cyoA"}
            Call prior.AddEdge("arcA", g, Effector.Inhibitor, 0.85, "synthetic:anaerobic repression")
        Next
        Call prior.AddEdge("arcA", "luxR", Effector.Inhibitor, 0.5, "synthetic:TF cascade")

        ' FNR：厌氧活化发酵途径
        For Each g As String In {"ldhA", "adhE", "ackA"}
            Call prior.AddEdge("fnr", g, Effector.Activator, 0.9, "synthetic:fermentation activation")
        Next
        For Each g As String In {"cyoA", "sdhA"}
            Call prior.AddEdge("fnr", g, Effector.Inhibitor, 0.7, "synthetic:anaerobic repression")
        Next

        ' CodY：营养限制时抑制中心代谢与氨基酸合成
        For Each g As String In {"pykA", "aceE", "gdhA", "gltA"}
            Call prior.AddEdge("codY", g, Effector.Inhibitor, 0.75, "synthetic:stringent response")
        Next

        ' LuxR：群体感应，活化分泌途径
        For Each g As String In {"ldhA", "adhE", "lacY"}
            Call prior.AddEdge("luxR", g, Effector.Activator, 0.65, "synthetic:quorum sensing")
        Next

        Return prior
    End Function

    ' ==================== 基线表达矩阵 ====================

    Public Function BuildExpression(genes As String(),
                                    Optional nSamples As Integer = 8,
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

        For i As Integer = 0 To n - 1
            ' 转录因子表达量偏低，代谢酶偏高——与真实表达谱的量级关系一致
            Dim isTF As Boolean = TranscriptionFactors.Contains(genes(i), StringComparer.OrdinalIgnoreCase)
            Dim baseLevel As Double = If(isTF, 4.0, 12.0) + rand.NextDouble() * 4.0

            For j As Integer = 0 To m - 1
                Dim noise As Double = 1.0 + (rand.NextDouble() - 0.5) * 0.15

                matrix(i, j) = baseLevel * noise
            Next
        Next

        Return New GeneExpressionData With {
            .GeneNames = genes,
            .SampleNames = samples,
            .Matrix = matrix,
            .TimePoints = times
        }
    End Function

    ' ==================== 代谢网络训练数据 ====================

    ''' <summary>
    ''' 合成一套用于拟合 Metaboliq 的时序数据（归一化空间：log1p + z-score）
    ''' </summary>
    Public Function BuildTrainingSet(graph As MetabolicNetworkGraph,
                                     Optional nSteps As Integer = 40,
                                     Optional dt As Double = 1.0,
                                     Optional seed As Integer = 2024) As MetabolicTrainingSet
        Dim rand As New Random(seed)
        Dim totalSteps As Integer = nSteps
        Dim m As Integer = graph.MetaboliteCount
        Dim r As Integer = graph.ReactionCount
        Dim nB As Integer = graph.BoundaryCount

        Dim times As Double() = New Double(totalSteps - 1) {}
        Dim observed As Tensor = New Tensor(totalSteps, m)
        Dim enzymes As Tensor = New Tensor(totalSteps, r)
        Dim boundary As Tensor = New Tensor(totalSteps, nB)

        For t As Integer = 0 To totalSteps - 1
            times(t) = t * dt
        Next

        ' ---- 胞外：葡萄糖被消耗，氧气维持，产物外排累积 ----
        For t As Integer = 0 To totalSteps - 1
            Dim x As Double = times(t)

            For k As Integer = 0 To nB - 1
                Dim id As String = graph.BoundaryIds(k)
                Dim raw As Double

                Select Case id
                    Case "glc_e"
                        raw = 10.0 * System.Math.Exp(-x / 25.0)
                    Case "o2_e"
                        raw = 8.0
                    Case "pi_e"
                        raw = 5.0
                    Case "lac_e"
                        raw = 3.0 * (1.0 - System.Math.Exp(-x / 12.0))
                    Case "etoh_e"
                        raw = 1.5 * (1.0 - System.Math.Exp(-x / 16.0))
                    Case "ac_e"
                        raw = 2.0 * (1.0 - System.Math.Exp(-x / 20.0))
                    Case "co2_e"
                        raw = 6.0 * (1.0 - System.Math.Exp(-x / 10.0))
                    Case Else
                        raw = 1.0
                End Select

                boundary(t, k) = raw
            Next

            ' ---- 酶水平：野生型 = 1.0，带一点表达噪声 ----
            For j As Integer = 0 To r - 1
                enzymes(t, j) = System.Math.Min(1.0, System.Math.Max(0.2, 0.85 + rand.NextDouble() * 0.2))
            Next
        Next

        ' ---- 胞内代谢物：按角色给出不同的响应曲线 ----
        For i As Integer = 0 To m - 1
            Dim id As String = graph.InternalIds(i)
            Dim baseLevel As Double = 1.5 + (Hash(id) Mod 30) / 10.0
            Dim tau As Double = 4.0 + (Hash(id) Mod 9)

            Select Case id
                Case "atp", "nadh", "aa_pool"
                    baseLevel = 2.5
                Case "lac", "etoh", "ac", "co2"
                    baseLevel = 1.0
                    tau = 14.0
                Case "nad", "adp", "coa", "q8"
                    baseLevel = 2.0
            End Select

            For t As Integer = 0 To totalSteps - 1
                Dim x As Double = times(t)
                Dim rise As Double = 1.0 - System.Math.Exp(-x / tau)
                Dim drain As Double = System.Math.Exp(-x / (tau * 6.0))
                Dim raw As Double = baseLevel * (0.4 + 0.9 * rise * (0.35 + 0.65 * drain))

                raw *= 1.0 + (rand.NextDouble() - 0.5) * 0.05

                observed(t, i) = raw
            Next
        Next

        ' ---- 归一化：log1p + z-score（逐代谢物在时间轴上做）----
        Call NormalizeColumns(observed)
        Call NormalizeColumns(boundary)

        Return New MetabolicTrainingSet With {
            .Times = times,
            .Observed = observed,
            .Enzymes = enzymes,
            .Boundary = boundary,
            .Config = New MetabolicTrainerConfig With {
                .LambdaData = 1.0,
                .LambdaMass = 0.5,
                .LambdaThermo = 0.2,
                .LambdaFlux = 0.0,
                .LearningRate = 0.02,
                .Epochs = 40,
                .WarmupEpochs = 8,
                .GradientClip = 5.0,
                .TeacherForcingStart = 0.9,
                .TeacherForcingEnd = 0.0,
                .LogEvery = 10,
                .Verbose = False,
                .Seed = seed
            }
        }
    End Function

    ''' <summary>逐列做 log1p + z-score 归一化</summary>
    Private Sub NormalizeColumns(x As Tensor)
        Dim rows As Integer = x.Shape(0)
        Dim cols As Integer = x.Shape(1)

        For j As Integer = 0 To cols - 1
            Dim mean As Double = 0.0

            For i As Integer = 0 To rows - 1
                Dim v As Double = x(i, j)

                If v > 0 Then
                    v = System.Math.Log(1.0 + v)
                Else
                    v = 0.0
                End If

                x(i, j) = v
                mean += v
            Next

            mean /= rows

            Dim variance As Double = 0.0

            For i As Integer = 0 To rows - 1
                Dim d As Double = x(i, j) - mean

                variance += d * d
            Next

            Dim sd As Double = System.Math.Sqrt(variance / System.Math.Max(1, rows - 1))

            If sd < 0.000001 Then
                sd = 1.0
            End If

            For i As Integer = 0 To rows - 1
                x(i, j) = (x(i, j) - mean) / sd
            Next
        Next
    End Sub

    Private Function Hash(id As String) As Integer
        Dim h As Long = 0

        For Each ch As Char In id
            h = (h * 31L + AscW(ch)) And &H7FFFFFFFL
        Next

        Return CInt(h)
    End Function

End Module
