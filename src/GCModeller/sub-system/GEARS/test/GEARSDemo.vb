Imports System.Diagnostics
Imports System.IO
Imports std = System.Math
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.GEARS
Imports SMRUCC.genomics.Analysis.GEARS.IO
Imports SMRUCC.genomics.Analysis.GEARS.Training
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' GEARS 虚拟扰动实验的演示程序
''' </summary>
''' <remarks>
''' 演示完整流程：
''' <list type="number">
''' <item><description>从 CSV 读取基因表达矩阵（行=基因、列=样本）与先验调控网络；</description></item>
''' <item><description>构建基因调控图并用内置仿真器合成伪 Perturb-seq 数据训练 GNN；</description></item>
''' <item><description>[2.5] 用现场合成的 Perturb-seq 风格矩阵测试 <c>SetTrainingSamples(Matrix, control, perturbed)</c> 接口；</description></item>
''' <item><description>执行单基因敲除 / 下调 / 过表达以及多基因组合扰动；</description></item>
''' <item><description>[4.5] 把训练好的 GEARS 存为 zip 再加载回来，逐基因比对扰动结果验证 Save/Load 实现；</description></item>
''' <item><description>导出结果 CSV：单扰动明细、批量汇总、各类比较分析矩阵与一致性对比表。</description></item>
''' </list>
''' </remarks>
Module GEARSDemo

    ''' <summary>测试数据目录</summary>
    Const DataDir As String = "G:\GCModeller\src\GCModeller\sub-system\demo\TestData1"

    ''' <summary>
    ''' 运行完整演示
    ''' </summary>
    Sub Run()
        Dim sw As Stopwatch = Stopwatch.StartNew()

        Console.WriteLine(New String("="c, 74))
        Console.WriteLine("GEARS: 基于图神经网络的基因表达调控网络虚拟扰动")
        Console.WriteLine(New String("="c, 74))
        Console.WriteLine()

        ' ==================== 1. 加载数据 ====================
        Dim exprFile As String = Path.Combine(DataDir, "gene_expression_matrix.csv")
        Dim priorFile As String = Path.Combine(DataDir, "regulatory_network_prior.csv")
        Dim pathwayFile As String = Path.Combine(DataDir, "pathway_info.json")

        Console.WriteLine("[1/5] 加载数据")
        Console.WriteLine($"  表达矩阵: {exprFile}")
        Console.WriteLine($"  先验网络: {priorFile}")

        Dim matrix As Matrix = Matrix.LoadData(exprFile)
        Dim exprData = BnIO.ReadGeneExpressionMatrix(matrix)
        Dim prior = PriorNetworkIO.LoadPriorNetwork(priorFile)

        Console.WriteLine($"  表达矩阵: {exprData.NGene} 基因 x {exprData.NSample} 样本")
        Console.WriteLine($"  先验网络: {prior.Edges.Count} 条调控边, {prior.TFNames.Count} 个转录因子")

        Dim pathways As Dictionary(Of String, MetabolicPathway) = Nothing

        If File.Exists(pathwayFile) Then
            pathways = pathwayFile.LoadJsonFile(Of Dictionary(Of String, MetabolicPathway))
            Console.WriteLine($"  通路注释: {pathways.Count} 个通路")
        End If

        Console.WriteLine()

        ' ==================== 2. 构建并训练模型 ====================
        Console.WriteLine("[2/5] 构建基因调控图并训练 GNN")

        Dim config As New GEARSConfig With {
            .EmbeddingDim = 16,
            .HiddenDim = 32,
            .NumLayers = 2,
            .Epochs = 50,
            .LearningRate = 0.003F,
            .NSinglePerturbation = 24,
            .NComboPerturbation = 16,
            .ComboSize = 2,
            .PropagationDecay = 0.6,
            .MaxHops = 3,
            .SynergyStrength = 0.35,
            .PrintEvery = 5,
            .Seed = 2024
        }

        Console.WriteLine($"  配置: {config}")

        Dim gears As New GEARS(exprData, prior, config)

        Call gears.GenerateTrainingSamples()
        Call gears.Train()

        Console.WriteLine()
        Console.WriteLine($"  调控图: {gears.GraphData}")
        Console.WriteLine($"  模型:   {gears.Model.NumLayers} 层, 隐藏维度 {gears.Model.HiddenDim}, " &
                          $"节点特征维度 {gears.Model.FeatureDim}")
        Console.WriteLine($"  训练样本: {gears.TrainingSamples.Count}")
        Console.WriteLine($"  最终损失: {gears.LossCurve(gears.LossCurve.Length - 1).ToString("F6")} " &
                          $"(首轮 {gears.LossCurve(0).ToString("F6")})")
        Console.WriteLine($"  训练耗时: {sw.Elapsed.TotalSeconds.ToString("F1")} 秒")
        Console.WriteLine()

        ' ==================== 2.5 矩阵版训练样本接口 ====================
        ' 注意：SetTrainingSamples(Matrix, ...) 会用显式 control 列重算并覆盖野生型基线，
        ' 因此在独立的实例上做验证，避免破坏主实验 gears 已训练好的状态。
        Call TestSetTrainingSamplesFromMatrix(exprData, prior, config)

        ' ==================== 3. 单基因虚拟扰动 ====================
        Console.WriteLine("[3/5] 单基因虚拟扰动（敲除 / 下调 / 过表达）")

        Dim results As New List(Of InterventionResult)()
        Dim targetGenes As String() = {"codY", "terR", "luxR", "spo0A", "cysR", "fadR"}

        For Each gene As String In targetGenes
            Dim ko As InterventionResult = gears.KnockoutGene(gene)

            results.Add(ko)

            Console.WriteLine($"  {gene,-8} Knockout       受影响基因 {ko.NAffected,4} / {ko.GeneNames.Length}" &
                              $"   Top: {TopChangedText(ko, 3)}")
        Next

        For Each gene As String In {"codY", "terR"}
            Dim kd As InterventionResult = gears.KnockDownGene(gene)
            Dim oe As InterventionResult = gears.OverexpressGene(gene)

            results.Add(kd)
            results.Add(oe)

            Console.WriteLine($"  {gene,-8} Knockdown     受影响基因 {kd.NAffected,4} / {kd.GeneNames.Length}" &
                              $"   Top: {TopChangedText(kd, 3)}")
            Console.WriteLine($"  {gene,-8} Overexpression 受影响基因 {oe.NAffected,4} / {oe.GeneNames.Length}" &
                              $"   Top: {TopChangedText(oe, 3)}")
        Next

        Console.WriteLine()

        ' ==================== 4. 组合扰动 ====================
        Console.WriteLine("[4/5] 组合扰动（捕捉非加性的协同 / 拮抗效应）")

        Dim combos As String()() = {
            New String() {"codY", "luxR"},
            New String() {"spo0A", "abrB"},
            New String() {"cysR", "fadR"},
            New String() {"terR", "spo0A"}
        }

        For Each combo As String() In combos
            Dim label As String = String.Join("+", combo)
            Dim res As InterventionResult = gears.PredictCombination(combo, InterventionMode.Knockout)

            results.Add(res)

            Console.WriteLine($"  {label,-16} 受影响基因 {res.NAffected,4} / {res.GeneNames.Length}" &
                              $"   Top: {TopChangedText(res, 3)}")
        Next

        Console.WriteLine()

        ' ---- 组合扰动 vs 单基因扰动的非加性检验 ----
        Call PrintNonAdditivity(gears, "codY", "luxR")

        Console.WriteLine()

        ' ==================== 4.5 Save / Load 一致性对比 ====================
        Dim consistencyRows As List(Of (Condition As String, Gene As String, Direct As Double, Loaded As Double)) =
            TestSaveLoadConsistency(gears, targetGenes, combos, outputDir)

        Console.WriteLine()

        ' ==================== 5. 导出结果 ====================
        Console.WriteLine("[5/5] 导出结果")
        Console.WriteLine($"  输出目录: {outputDir}")

        ' 逐个扰动输出明细表（复用 BNLearn\IO\IO.vb）
        For Each res As InterventionResult In results
            Dim name As String = res.Spec.GeneName.Replace("+"c, "_"c)
            Dim file As String = Path.Combine(outputDir, $"gears_{name}_{res.Spec.Mode.ToString()}.csv")

            Call BnIO.WriteInterventionResult(res, file)
        Next

        ' 批量汇总表
        Call BnIO.WriteBatchInterventionResults(results, Path.Combine(outputDir, "gears_batch_summary.csv"))

        ' 比较分析矩阵（复用 BNLearn\Intervention\InterventionComparison.vb）
        Dim exporter As New InterventionComparisonExporter(results)

        Call exporter.ExportAll(outputDir, pathways, topN:=30)

        Console.WriteLine($"  单扰动明细: {results.Count} 个 CSV")
        Console.WriteLine("  批量汇总:   gears_batch_summary.csv")
        Console.WriteLine("  比较矩阵:   foldchange / percentchange / significance / zscore / wildtype / mutant")
        Console.WriteLine("              comprehensive_comparison / condition_similarity / gene_sensitivity")
        Console.WriteLine("              intervention_ranking / pathway_summary / cross_impact_matrix")

        sw.Stop()

        Console.WriteLine()
        Console.WriteLine(New String("="c, 74))
        Console.WriteLine($"演示完成，总耗时 {sw.Elapsed.TotalSeconds.ToString("F1")} 秒")
        Console.WriteLine(New String("="c, 74))
    End Sub

    ''' <summary>
    ''' 把结果中变化最大的若干个基因格式化为一行文本
    ''' </summary>
    ''' <param name="result">干预结果</param>
    ''' <param name="n">展示的基因数量</param>
    ''' <returns>形如 "codY(-1.23), luxR(+0.88)" 的文本</returns>
    Private Function TopChangedText(result As InterventionResult, n As Integer) As String
        Dim items As List(Of (GeneName As String, FoldChange As Double, PercentChange As Double)) =
            result.GetTopChangedGenes(n)
        Dim parts As New List(Of String)()

        For Each item In items
            parts.Add($"{item.GeneName}({item.FoldChange.ToString("+0.000;-0.000")})")
        Next

        Return String.Join(", ", parts)
    End Function

    ''' <summary>
    ''' 打印组合扰动相对单基因扰动线性叠加的偏差（非加性效应）
    ''' </summary>
    ''' <param name="gears">GEARS 实验对象</param>
    ''' <param name="geneA">第一个基因</param>
    ''' <param name="geneB">第二个基因</param>
    Private Sub PrintNonAdditivity(gears As GEARS, geneA As String, geneB As String)
        Dim a As InterventionResult = gears.KnockoutGene(geneA)
        Dim b As InterventionResult = gears.KnockoutGene(geneB)
        Dim ab As InterventionResult = gears.PredictCombination({geneA, geneB}, InterventionMode.Knockout)

        Dim names As String() = ab.GeneNames
        Dim deviations As New List(Of (GeneName As String, Expected As Double, Actual As Double, Deviation As Double))()

        For i As Integer = 0 To names.Length - 1
            Dim expected As Double = a.FoldChanges(i) + b.FoldChanges(i)
            Dim actual As Double = ab.FoldChanges(i)
            Dim deviation As Double = actual - expected

            deviations.Add((names(i), expected, actual, deviation))
        Next

        deviations.Sort(Function(x, y) std.Abs(y.Deviation).CompareTo(std.Abs(x.Deviation)))

        Console.WriteLine($"  非加性效应检验: KO({geneA}) + KO({geneB})  vs  KO({geneA}+{geneB})")
        Console.WriteLine($"  {"Gene",-12} {"线性叠加期望",14} {"GNN 预测实际",14} {"偏差",12}")

        For i As Integer = 0 To std.Min(9, deviations.Count - 1)
            Dim d = deviations(i)

            Console.WriteLine($"  {d.GeneName,-12} {d.Expected,14:F4} {d.Actual,14:F4} {d.Deviation,12:F4}")
        Next
    End Sub
End Module
