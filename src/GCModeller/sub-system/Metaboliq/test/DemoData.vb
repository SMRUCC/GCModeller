Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Metaboliq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel
Imports std = System.Math

''' <summary>
''' Demo 模拟数据的生成器
''' </summary>
''' <remarks>
''' 构建一个把 <c>糖酵解 + TCA 循环 + 有氧呼吸链（ETC / 氧化磷酸化）+ 无氧呼吸（乳酸发酵、乙醇发酵）</c>
''' 整合在一起的中心碳代谢网络（31 个内部代谢物、7 个边界代谢物、33 条反应），
''' 用米氏动力学的真值模型做 RK4 积分，产出：
''' <list type="bullet">
''' <item><description><c>network.json</c>：<see cref="MetabolicReaction"/> 数组</description></item>
''' <item><description><c>metabolites_timeseries.csv</c>：代谢物浓度时序（行=代谢物，列=时间点）</description></item>
''' <item><description><c>enzymes_timeseries.csv</c>：酶/基因表达时序（行=反应 id，列=时间点）</description></item>
''' <item><description><c>fluxes_truth.csv</c>：真值反应通量（用于 λ3 通量监督与结果验证）</description></item>
''' </list>
''' 场景设计：体系从好氧状态启动，在 t≈50 处溶氧被耗尽（o2_e 由 0.25 平滑降到 0），
''' 从而触发"有氧呼吸 → 无氧发酵"的代谢重编程——这正是 LNN 需要复现的动态。
''' 采样时间故意取成<strong>非均匀</strong>网格，用于验证 LNN 处理不规则采样的能力。
''' </remarks>
Public Module DemoData

    ''' <summary>边界（胞外）代谢物 id</summary>
    Public ReadOnly BoundaryIds As String() = {
        "glc_e", "o2_e", "co2_e", "lac_e", "etoh_e", "ac_e", "pi_e"
    }

#Region "网络定义"

    ''' <summary>
    ''' 构建中心碳代谢网络（糖酵解 + TCA + ETC + 乳酸/乙醇发酵）
    ''' </summary>
    Public Function BuildReactions() As MetabolicReaction()
        Dim list As New List(Of MetabolicReaction)()

        ' ---------------- 糖酵解 (Glycolysis) ----------------
        list.Add(Rxn("HEX1", "hexokinase", {"glc_e", "atp"}, {"g6p", "adp"}))
        list.Add(Rxn("PGI", "glucose-6-phosphate isomerase", {"g6p"}, {"f6p"}, rev:=True))
        list.Add(Rxn("PFK", "phosphofructokinase", {"f6p", "atp"}, {"fdp", "adp"}))
        list.Add(Rxn("FBA", "fructose-bisphosphate aldolase", {"fdp"}, {"dhap", "gap"}, rev:=True))
        list.Add(Rxn("TPI", "triose-phosphate isomerase", {"dhap"}, {"gap"}, rev:=True))
        list.Add(Rxn("GAPD", "glyceraldehyde-3-phosphate dehydrogenase", {"gap", "pi", "nad"}, {"_13dpg", "nadh"}, rev:=True))
        list.Add(Rxn("PGK", "phosphoglycerate kinase", {"_13dpg", "adp"}, {"_3pg", "atp"}, rev:=True))
        list.Add(Rxn("PGM", "phosphoglycerate mutase", {"_3pg"}, {"_2pg"}, rev:=True))
        list.Add(Rxn("ENO", "enolase", {"_2pg"}, {"pep"}, rev:=True))
        list.Add(Rxn("PYK", "pyruvate kinase", {"pep", "adp"}, {"pyr", "atp"}))

        ' ---------------- 无氧呼吸 / 发酵 (Anaerobic respiration) ----------------
        list.Add(Rxn("LDH_L", "L-lactate dehydrogenase", {"pyr", "nadh"}, {"lac_c", "nad"}, rev:=True))
        list.Add(Rxn("LACt", "lactate transporter", {"lac_c"}, {"lac_e"}))
        list.Add(Rxn("PDC", "pyruvate decarboxylase", {"pyr"}, {"acald", "co2_e"}))
        list.Add(Rxn("ADH", "alcohol dehydrogenase", {"acald", "nadh"}, {"etoh_c", "nad"}, rev:=True))
        list.Add(Rxn("ETOHt", "ethanol transporter", {"etoh_c"}, {"etoh_e"}))

        ' ---------------- 丙酮酸入口与乙酸支路 (Mixed acid fermentation) ----------------
        list.Add(Rxn("PDH", "pyruvate dehydrogenase", {"pyr", "nad"}, {"accoa", "co2_e", "nadh"}))
        list.Add(Rxn("PTAr", "phosphotransacetylase", {"accoa"}, {"actp"}, rev:=True))
        list.Add(Rxn("ACKr", "acetate kinase", {"actp", "adp"}, {"ac_c", "atp"}))
        list.Add(Rxn("ACt", "acetate transporter", {"ac_c"}, {"ac_e"}))

        ' ---------------- TCA 循环 (Tricarboxylic acid cycle) ----------------
        list.Add(Rxn("CS", "citrate synthase", {"accoa", "oaa"}, {"cit"}))
        list.Add(Rxn("ACONTa", "aconitase", {"cit"}, {"icit"}, rev:=True))
        list.Add(Rxn("ICDH", "isocitrate dehydrogenase", {"icit", "nad"}, {"akg", "co2_e", "nadh"}))
        list.Add(Rxn("AKGDH", "alpha-ketoglutarate dehydrogenase", {"akg", "nad"}, {"succoa", "co2_e", "nadh"}))
        list.Add(Rxn("SUCOAS", "succinyl-CoA synthetase", {"succoa", "adp"}, {"succ", "atp"}, rev:=True))
        list.Add(Rxn("SDH", "succinate dehydrogenase", {"succ", "q8"}, {"fum", "q8h2"}))
        list.Add(Rxn("FUM", "fumarase", {"fum"}, {"mal"}, rev:=True))
        list.Add(Rxn("MDH", "malate dehydrogenase", {"mal", "nad"}, {"oaa", "nadh"}, rev:=True))
        list.Add(Rxn("ME1", "malic enzyme", {"mal", "nad"}, {"pyr", "co2_e", "nadh"}))

        ' ---------------- 有氧呼吸链 (Electron transport chain / OxPhos) ----------------
        list.Add(Rxn("NDH1", "NADH dehydrogenase", {"nadh", "q8"}, {"nad", "q8h2"}))
        list.Add(Rxn("CYTBO3", "cytochrome bo3 terminal oxidase", {"q8h2", "o2_e"}, {"q8"}))
        list.Add(Rxn("ATPS4r", "ATP synthase", {"adp", "pi"}, {"atp"}))
        list.Add(Rxn("ATPM", "ATP maintenance requirement", {"atp"}, {"adp", "pi"}))

        ' ---------------- 磷酸盐交换（维持胞内 Pi 池） ----------------
        list.Add(Rxn("PIt2r", "phosphate exchange", {"pi_e"}, {"pi"}, rev:=True))

        Return list.ToArray()
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

    Private Function Species(text As String) As CompoundSpecieReference
        Dim parts = text.Trim().Split(" "c)

        If parts.Length = 1 Then
            Return New CompoundSpecieReference(1.0, parts(0))
        End If

        Return New CompoundSpecieReference(
            Double.Parse(parts(0), CultureInfo.InvariantCulture), parts(1))
    End Function

#End Region

#Region "真值动力学"

    ''' <summary>每条反应的最大反应速率</summary>
    Private ReadOnly VmaxTable As New Dictionary(Of String, Double) From {
        {"HEX1", 1.0}, {"PGI", 2.0}, {"PFK", 1.2}, {"FBA", 2.0}, {"TPI", 3.0},
        {"GAPD", 2.5}, {"PGK", 2.5}, {"PGM", 2.5}, {"ENO", 2.5}, {"PYK", 1.5},
        {"LDH_L", 1.0}, {"LACt", 0.8}, {"PDC", 0.6}, {"ADH", 1.0}, {"ETOHt", 0.8},
        {"PDH", 1.2}, {"PTAr", 1.0}, {"ACKr", 1.0}, {"ACt", 0.8},
        {"CS", 1.5}, {"ACONTa", 2.0}, {"ICDH", 1.5}, {"AKGDH", 1.5}, {"SUCOAS", 2.0},
        {"SDH", 1.2}, {"FUM", 2.0}, {"MDH", 2.0}, {"ME1", 0.4},
        {"NDH1", 2.0}, {"CYTBO3", 2.0}, {"ATPS4r", 2.5}, {"ATPM", 0.5},
        {"PIt2r", 2.0}
    }

    ''' <summary>米氏常数（代谢物特异，缺省 0.3）</summary>
    Private Function KmOf(id As String) As Double
        Select Case id
            Case "atp", "adp", "pi", "pi_e", "nad", "nadh"
                Return 0.5
            Case "o2_e"
                Return 0.05
            Case "q8", "q8h2"
                Return 0.2
            Case "glc_e"
                Return 0.5
            Case Else
                Return 0.3
        End Select
    End Function

    Private Function Sat(c As Double, K As Double) As Double
        If c <= 0 Then Return 0.0
        Return c / (K + c)
    End Function

    ''' <summary>
    ''' 真值速率方程：不可逆反应走米氏型质量作用，可逆反应再减去反向项（Haldane 简化形式）
    ''' </summary>
    ''' <param name="c">全部代谢物浓度（按 graph.MetaboliteIds 的顺序）</param>
    ''' <param name="idx">代谢物 id → 索引</param>
    ''' <param name="rxn">反应</param>
    ''' <param name="enzyme">该反应的酶表达量</param>
    Private Function RateLaw(c As Double(), idx As Dictionary(Of String, Integer),
                             rxn As MetabolicReaction, enzyme As Double) As Double
        Dim vmax = VmaxTable(rxn.id)
        Dim e = std.Max(0.0, enzyme)

        If e <= 0 Then Return 0.0

        ' 反应物饱和项
        Dim forward As Double = 1.0
        For Each sp In rxn.left
            forward *= Sat(c(idx(sp.ID)), KmOf(sp.ID))
        Next

        ' 特殊耦合：ATP 合成酶受电子传递链与溶氧驱动
        Select Case rxn.id
            Case "ATPS4r"
                forward *= Sat(c(idx("q8h2")), KmOf("q8h2")) * Sat(c(idx("o2_e")), KmOf("o2_e"))
        End Select

        If Not rxn.is_reversible Then
            Return vmax * e * forward
        End If

        ' 可逆反应：减去反向项，Keq=3 表示平衡偏向产物
        Dim backward As Double = 1.0
        For Each sp In rxn.right
            backward *= Sat(c(idx(sp.ID)), KmOf(sp.ID))
        Next

        Const Keq As Double = 3.0

        Return vmax * e * (forward - backward / Keq)
    End Function

    ''' <summary>初始胞内代谢物浓度</summary>
    Private Function InitialConcentrations() As Dictionary(Of String, Double)
        Return New Dictionary(Of String, Double) From {
            {"g6p", 0.50}, {"f6p", 0.20}, {"fdp", 0.30}, {"dhap", 0.10}, {"gap", 0.05},
            {"_13dpg", 0.02}, {"_3pg", 0.30}, {"_2pg", 0.05}, {"pep", 0.10}, {"pyr", 0.50},
            {"accoa", 0.10}, {"cit", 0.50}, {"icit", 0.10}, {"akg", 0.30}, {"succoa", 0.05},
            {"succ", 0.20}, {"fum", 0.10}, {"mal", 0.30}, {"oaa", 0.05},
            {"nad", 1.00}, {"nadh", 0.10}, {"atp", 2.00}, {"adp", 0.50}, {"pi", 1.00},
            {"q8", 0.50}, {"q8h2", 0.10},
            {"lac_c", 0.10}, {"acald", 0.01}, {"etoh_c", 0.10}, {"actp", 0.01}, {"ac_c", 0.10},
            {"glc_e", 10.0}, {"o2_e", 0.25}, {"co2_e", 0.0}, {"lac_e", 0.0},
            {"etoh_e", 0.0}, {"ac_e", 0.0}, {"pi_e", 1.0}
        }
    End Function

#End Region

#Region "场景"

    ''' <summary>
    ''' 非均匀采样时间网格（分钟）：前期密集、后期稀疏，用于验证不规则采样支持
    ''' </summary>
    Public Function SampleTimes() As Double()
        Return New Double() {
            0, 0.5, 1, 1.5, 2, 2.5, 3, 4, 5, 6, 8, 10, 12, 15, 18, 21, 25, 30,
            36, 43, 51, 60, 70, 82, 96, 112, 131, 153, 179, 210
        }
    End Function

    ''' <summary>胞外葡萄糖：随培养进程逐渐耗尽</summary>
    Private Function GlucoseFeed(t As Double) As Double
        Return 10.0 * std.Exp(-t / 120.0)
    End Function

    ''' <summary>
    ''' 溶氧：t≈50 处由好氧平滑切换到厌氧（sigmoid 过渡）
    ''' </summary>
    Private Function OxygenLevel(t As Double) As Double
        Return 0.25 / (1.0 + std.Exp((t - 50.0) / 4.0))
    End Function

    ''' <summary>
    ''' 酶表达程序：以 1.0 为基线做平滑的时序波动（模拟生长阶段依赖的酶表达调控）
    ''' </summary>
    Private Function EnzymeProgram(reactionIndex As Integer, t As Double) As Double
        Dim phase = reactionIndex * 0.7
        Return 1.0 + 0.25 * std.Sin(2.0 * std.PI * t / 90.0 + phase)
    End Function

#End Region

#Region "生成入口"

    ''' <summary>
    ''' 生成全部 demo 数据文件
    ''' </summary>
    ''' <param name="outputDir">输出目录（通常是 test\data）</param>
    ''' <param name="force">为 True 时即使文件已存在也重新生成</param>
    ''' <returns>生成出的文件路径</returns>
    Public Function Generate(outputDir As String, Optional force As Boolean = False) As String()
        Dim networkPath = Path.Combine(outputDir, "network.json")
        Dim metabolitePath = Path.Combine(outputDir, "metabolites_timeseries.csv")
        Dim enzymePath = Path.Combine(outputDir, "enzymes_timeseries.csv")
        Dim fluxPath = Path.Combine(outputDir, "fluxes_truth.csv")

        If Not force AndAlso File.Exists(networkPath) AndAlso File.Exists(metabolitePath) AndAlso
            File.Exists(enzymePath) AndAlso File.Exists(fluxPath) Then
            Return {networkPath, metabolitePath, enzymePath, fluxPath}
        End If

        If Not Directory.Exists(outputDir) Then
            Directory.CreateDirectory(outputDir)
        End If

        ' ---------- 网络拓扑 ----------
        Dim reactions = BuildReactions()
        Dim graph As New MetabolicNetworkGraph(reactions, BoundaryIds)

        Call graph.SaveJson(networkPath)

        ' ---------- 场景矩阵 ----------
        Dim times = SampleTimes()
        Dim T = times.Length
        Dim mAll = graph.MetaboliteIds.Length
        Dim mInt = graph.MetaboliteCount
        Dim r = graph.ReactionCount
        Dim idx = BuildIndex(graph.MetaboliteIds)

        ' 酶表达序列 (T × r)
        Dim enzymes = New Tensor(T, r)
        For t = 0 To T - 1
            For j = 0 To r - 1
                enzymes(t, j) = EnzymeProgram(j, times(t))
            Next
        Next

        ' 边界代谢物浓度序列 (T × nB)
        Dim nB = graph.BoundaryCount
        Dim boundarySeries = New Tensor(T, nB)

        For t = 0 To T - 1
            For k = 0 To nB - 1
                Dim id = graph.BoundaryIds(k)

                Select Case id
                    Case "glc_e"
                        boundarySeries(t, k) = GlucoseFeed(times(t))
                    Case "o2_e"
                        boundarySeries(t, k) = OxygenLevel(times(t))
                    Case "pi_e"
                        boundarySeries(t, k) = 1.0
                    Case Else
                        ' 胞外产物被不断移走，维持在 0
                        boundarySeries(t, k) = 0.0
                End Select
            Next
        Next

        ' ---------- RK4 积分真值动力学 ----------
        Dim init = InitialConcentrations()
        Dim c = New Double(mAll - 1) {}

        For i = 0 To mAll - 1
            c(i) = If(init.ContainsKey(graph.MetaboliteIds(i)), init(graph.MetaboliteIds(i)), 0.1)
        Next

        Dim concAll = New Tensor(T, mAll)
        Dim fluxAll = New Tensor(T, r)

        ' 记录 t=0
        Call Snapshot(c, idx, graph, enzymes, 0, times(0), concAll, fluxAll)

        Const fineDt As Double = 0.02

        For t = 1 To T - 1
            Dim tStart = times(t - 1)
            Dim tEnd = times(t)
            Dim span = tEnd - tStart
            Dim nStep As Integer = CInt(std.Ceiling(span / fineDt))
            Dim dt = span / nStep

            For stepIdx = 1 To nStep
                Dim tc = tStart + (stepIdx - 1) * dt

                Call RK4Step(c, idx, graph, tc, dt, enzymes, times)
            Next

            Call Snapshot(c, idx, graph, enzymes, t, times(t), concAll, fluxAll)
        Next

        ' ---------- 落盘 ----------
        ' 代谢物 CSV 同时包含内部代谢物与胞外（边界）代谢物：
        ' 前者作为监督目标，后者作为网络的外部驱动输入。叠加 3% 乘性噪声模拟 LC-MS 测量误差。
        Dim rng As New Random(20260831)
        Dim observed = New Tensor(T, mAll)

        For t = 0 To T - 1
            For i = 0 To mAll - 1
                Dim truth = concAll(t, i)
                Dim noise = 1.0 + 0.03 * Gauss(rng)

                observed(t, i) = std.Max(1.0E-6, truth * noise)
            Next
        Next

        ' MetabolicDataIO 的约定是"行=分子，列=样本"，因此这里再转置一次
        Call MetabolicDataIO.SaveCsv(metabolitePath, graph.MetaboliteIds, ColumnNames(times), Transpose(observed))
        Call MetabolicDataIO.SaveCsv(enzymePath, graph.ReactionIds, ColumnNames(times), Transpose(enzymes))
        Call MetabolicDataIO.SaveCsv(fluxPath, graph.ReactionIds, ColumnNames(times), Transpose(fluxAll))

        Return {networkPath, metabolitePath, enzymePath, fluxPath}
    End Function

    Private Function BuildIndex(ids As String()) As Dictionary(Of String, Integer)
        Dim map As New Dictionary(Of String, Integer)()

        For i = 0 To ids.Length - 1
            map(ids(i)) = i
        Next

        Return map
    End Function

    ''' <summary>把当前状态与通量写入结果矩阵</summary>
    Private Sub Snapshot(c As Double(), idx As Dictionary(Of String, Integer), graph As MetabolicNetworkGraph,
                         enzymes As Tensor, t As Integer, time As Double,
                         concAll As Tensor, fluxAll As Tensor)
        For i = 0 To c.Length - 1
            concAll(t, i) = c(i)
        Next

        Dim e = InterpolateEnzymes(enzymes, time, SampleTimes())

        For j = 0 To graph.ReactionCount - 1
            fluxAll(t, j) = RateLaw(c, idx, graph.Reactions(j), e(j))
        Next
    End Sub

    ''' <summary>按时间线性插值出当前的酶表达量（零阶保持更贴近实验设计，这里用线性插值）</summary>
    Private Function InterpolateEnzymes(enzymes As Tensor, time As Double, times As Double()) As Double()
        Dim T = times.Length
        Dim width = enzymes.Shape(1)
        Dim out = New Double(width - 1) {}
        Dim k As Integer = 0

        While k < T - 1 AndAlso times(k + 1) < time
            k += 1
        End While

        Dim k2 = std.Min(T - 1, k + 1)
        Dim span = times(k2) - times(k)
        Dim w As Double = If(span > 0, (time - times(k)) / span, 0.0)

        w = std.Max(0.0, std.Min(1.0, w))

        For j = 0 To width - 1
            out(j) = enzymes(k, j) * (1.0 - w) + enzymes(k2, j) * w
        Next

        Return out
    End Function

    ''' <summary>对内部代谢物做一步 RK4，边界代谢物由场景直接给定</summary>
    Private Sub RK4Step(c As Double(), idx As Dictionary(Of String, Integer), graph As MetabolicNetworkGraph,
                        time As Double, dt As Double, enzymes As Tensor, times As Double())
        Dim mAll = c.Length
        Dim k1 = Derivatives(c, idx, graph, time, enzymes, times)
        Dim c2 = AddScaled(c, k1, dt * 0.5)
        Dim k2 = Derivatives(c2, idx, graph, time + dt * 0.5, enzymes, times)
        Dim c3 = AddScaled(c, k2, dt * 0.5)
        Dim k3 = Derivatives(c3, idx, graph, time + dt * 0.5, enzymes, times)
        Dim c4 = AddScaled(c, k3, dt)
        Dim k4 = Derivatives(c4, idx, graph, time + dt, enzymes, times)

        For i = 0 To mAll - 1
            c(i) += dt / 6.0 * (k1(i) + 2.0 * k2(i) + 2.0 * k3(i) + k4(i))
        Next

        ' 边界代谢物由场景强制给定，不参与积分
        For k = 0 To graph.BoundaryCount - 1
            Dim id = graph.BoundaryIds(k)
            Dim i = idx(id)

            Select Case id
                Case "glc_e"
                    c(i) = GlucoseFeed(time + dt)
                Case "o2_e"
                    c(i) = OxygenLevel(time + dt)
                Case "pi_e"
                    c(i) = 1.0
                Case Else
                    c(i) = 0.0
            End Select
        Next

        ' 浓度不允许为负
        For i = 0 To mAll - 1
            If c(i) < 0.0 Then c(i) = 0.0
        Next
    End Sub

    Private Function Derivatives(c As Double(), idx As Dictionary(Of String, Integer),
                                 graph As MetabolicNetworkGraph, time As Double,
                                 enzymes As Tensor, times As Double()) As Double()
        Dim mAll = c.Length
        Dim r = graph.ReactionCount
        Dim v = New Double(r - 1) {}
        Dim e = InterpolateEnzymes(enzymes, time, times)

        For j = 0 To r - 1
            v(j) = RateLaw(c, idx, graph.Reactions(j), e(j))
        Next

        Dim dc = New Double(mAll - 1) {}

        For i = 0 To mAll - 1
            Dim acc As Double = 0.0

            For j = 0 To r - 1
                acc += graph.Stoichiometry(i, j) * v(j)
            Next

            dc(i) = acc
        Next

        Return dc
    End Function

    Private Function AddScaled(c As Double(), d As Double(), factor As Double) As Double()
        Dim out = New Double(c.Length - 1) {}

        For i = 0 To c.Length - 1
            out(i) = std.Max(0.0, c(i) + d(i) * factor)
        Next

        Return out
    End Function

    Private Function ColumnNames(times As Double()) As String()
        Return times.Select(Function(t) "T" & t.ToString("0.###", CultureInfo.InvariantCulture)).ToArray()
    End Function

    Private Function Transpose(mat As Tensor) As Tensor
        Dim rows = mat.Shape(0)
        Dim cols = mat.Shape(1)
        Dim out = New Tensor(cols, rows)

        For i = 0 To rows - 1
            For j = 0 To cols - 1
                out(j, i) = mat(i, j)
            Next
        Next

        Return out
    End Function

    ''' <summary>Box-Muller 变换生成标准正态随机数</summary>
    Private Function Gauss(rng As Random) As Double
        Dim u1 = 1.0 - rng.NextDouble()
        Dim u2 = 1.0 - rng.NextDouble()

        Return std.Sqrt(-2.0 * std.Log(u1)) * std.Sin(2.0 * std.PI * u2)
    End Function

#End Region

End Module
