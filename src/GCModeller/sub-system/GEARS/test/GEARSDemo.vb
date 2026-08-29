Imports System.Diagnostics
Imports System.IO
Imports std = System.Math
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.BNLearn.Core
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

        ' 输出目录（[4.5] 与 [5/5] 都会用到，故提前确定并创建）
        Dim outputDir As String = App.HOME & "/GEARS_output/"

        If Not Directory.Exists(outputDir) Then
            Call Directory.CreateDirectory(outputDir)
        End If

        Dim matrix As Matrix = Matrix.LoadData(exprFile)
        Dim exprData = BnIO.ReadGeneExpressionMatrix(matrix)
        Dim prior As PriorNetwork = PriorNetworkIO.LoadPriorNetwork(priorFile)

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

        ' Save/Load 一致性逐基因对比表
        If consistencyRows IsNot Nothing AndAlso consistencyRows.Count > 0 Then
            Call ExportConsistencyTable(consistencyRows, Path.Combine(outputDir, "save_load_consistency.csv"))
        End If

        Console.WriteLine($"  单扰动明细: {results.Count} 个 CSV")
        Console.WriteLine("  批量汇总:   gears_batch_summary.csv")
        Console.WriteLine("  比较矩阵:   foldchange / percentchange / significance / zscore / wildtype / mutant")
        Console.WriteLine("              comprehensive_comparison / condition_similarity / gene_sensitivity")
        Console.WriteLine("              intervention_ranking / pathway_summary / cross_impact_matrix")
        Console.WriteLine("  一致性对比: save_load_consistency.csv")

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

    ' ================================================================
    '  矩阵版训练样本接口（GEARS.SetTrainingSamples(Matrix, String(), SampleInfo())）
    ' ================================================================

    ''' <summary>
    ''' 演示用的扰动样本定义：样本列名 → （被扰动基因集合, 干预模式）
    ''' </summary>
    ''' <returns>扰动定义数组</returns>
    Private Function PerturbSeqDefinitions() As (Label As String, Genes As String(), Mode As InterventionMode)()
        Return {
            ("codY_Knockout", New String() {"codY"}, InterventionMode.Knockout),
            ("luxR_Knockout", New String() {"luxR"}, InterventionMode.Knockout),
            ("terR_Knockout", New String() {"terR"}, InterventionMode.Knockout),
            ("abrB_Knockout", New String() {"abrB"}, InterventionMode.Knockout),
            ("sigB_Knockout", New String() {"sigB"}, InterventionMode.Knockout),
            ("codY+luxR_Knockdown", New String() {"codY", "luxR"}, InterventionMode.Knockdown),
            ("cysR+fadR_Knockout", New String() {"cysR", "fadR"}, InterventionMode.Knockout),
            ("spo0A_Overexpression", New String() {"spo0A"}, InterventionMode.Overexpression)
        }
    End Function

    ''' <summary>
    ''' 用内置仿真器现场合成一份 Perturb-seq 风格的表达矩阵
    ''' </summary>
    ''' <param name="gears">已建好调控图的 GEARS 对象，提供基因列表与野生型基线</param>
    ''' <returns>行为基因、列为样本的表达矩阵；前 3 列为 WT 重复，其余为扰动样本</returns>
    Private Function BuildPerturbSeqMatrix(gears As GEARS) As Matrix
        Dim simulator As New InSilicoPerturbationSimulator(
            graph:=gears.GraphData,
            controlMean:=gears.WildtypeMeans,
            controlSD:=gears.WildtypeSDs,
            seed:=2024
        )

        Dim n As Integer = gears.GeneNames.Length
        Dim sampleIDs As New List(Of String)()
        Dim columns As New List(Of Double())()
        Dim rnd As New Random(1234)

        ' ---- control（野生型）重复列：野生型均值叠加少量技术噪声 ----
        ' 取 8 个重复：样本标准差需要足够多的重复才能稳定估计，否则归一化后的 Δ 标签会被放大
        For r As Integer = 1 To 8
            sampleIDs.Add($"WT_Rep{r}")

            Dim col As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                col(i) = std.Max(0.0, gears.WildtypeMeans(i) + NextGaussian(rnd) * gears.WildtypeSDs(i))
            Next

            columns.Add(col)
        Next

        ' ---- 扰动样本列：由仿真器给出扰动后的全转录组响应 ----
        For Each def In PerturbSeqDefinitions()
            Dim specs As New List(Of InterventionSpec)()

            For Each g As String In def.Genes
                specs.Add(New InterventionSpec With {.GeneName = g, .Mode = def.Mode})
            Next

            Dim sample As PerturbSeqSample = simulator.Simulate(specs)

            If sample Is Nothing Then
                Continue For
            End If

            sampleIDs.Add(def.Label)
            columns.Add(sample.PerturbedExpression)
        Next

        ' ---- 转置为「行=基因」的矩阵 ----
        Dim rows As DataFrameRow() = New DataFrameRow(n - 1) {}

        For i As Integer = 0 To n - 1
            Dim values As Double() = New Double(columns.Count - 1) {}

            For c As Integer = 0 To columns.Count - 1
                values(c) = columns(c)(i)
            Next

            rows(i) = New DataFrameRow With {
                .geneID = gears.GeneNames(i),
                .experiments = values
            }
        Next

        Return New Matrix With {
            .tag = "GEARS/PerturbSeq_demo",
            .sampleID = sampleIDs.ToArray(),
            .expression = rows
        }
    End Function

    ''' <summary>
    ''' 构造与 <see cref="BuildPerturbSeqMatrix"/> 的扰动列一一对应的样本信息对象
    ''' </summary>
    ''' <returns>样本信息数组；被扰动基因集合以 JSON 数组写进 metadata</returns>
    Private Function BuildPerturbSeqSampleInfo() As SampleInfo()
        Dim list As New List(Of SampleInfo)()

        For Each def In PerturbSeqDefinitions()
            list.Add(New SampleInfo With {
                .ID = def.Label,
                .sample_name = def.Label,
                .sample_info = def.Mode.ToString(),
                .batch = 1,
                .injectionOrder = list.Count + 1,
                .metadata = New Dictionary(Of String, String) From {
                    {GEARS.metadata_perturbed_genes, def.Genes.GetJson},
                    {GEARS.metadata_intervention_mode, def.Mode.ToString()}
                }
            })
        Next

        Return list.ToArray()
    End Function

    ''' <summary>
    ''' 测试 SetTrainingSamples(Matrix, control, perturbed) 接口
    ''' </summary>
    ''' <param name="exprData">基因表达数据</param>
    ''' <param name="prior">先验调控网络</param>
    ''' <param name="config">主实验的超参配置</param>
    ''' <remarks>
    ''' 该接口会用显式 control 列重算并覆盖野生型基线，因此这里在一个<strong>独立的 GEARS 实例</strong>上验证，
    ''' 以免影响主实验对象已经训练好的状态。
    ''' </remarks>
    Private Sub TestSetTrainingSamplesFromMatrix(exprData As GeneExpressionData,
                                                 prior As PriorNetwork,
                                                 config As GEARSConfig)
        Console.WriteLine("[2.5] 矩阵版训练样本接口测试 SetTrainingSamples(Matrix, control, perturbed)")

        ' 这里只做接口验证，故用一份「轻量」配置（少轮次）跑试运行，避免重复完整训练
        Dim probe As New GEARSConfig With {
            .EmbeddingDim = config.EmbeddingDim,
            .HiddenDim = config.HiddenDim,
            .NumLayers = config.NumLayers,
            .Activation = config.Activation,
            .Epochs = 8,
            .LearningRate = config.LearningRate,
            .PrintEvery = 0,
            .Seed = config.Seed
        }

        Dim gears As New GEARS(exprData, prior, probe)
        Dim matrix As Matrix = BuildPerturbSeqMatrix(gears)
        Dim control As String() = Enumerable.Range(1, 8).Select(Function(r) $"WT_Rep{r}").ToArray()
        Dim perturbed As SampleInfo() = BuildPerturbSeqSampleInfo()

        Console.WriteLine($"  合成矩阵: {matrix.size} 基因 x {matrix.sample_count} 样本 " &
                          $"(control {control.Length} 列 + 扰动 {perturbed.Length} 列)")
        Console.WriteLine($"  元数据示例: {perturbed(0).ID} -> " &
                          $"{GEARS.metadata_perturbed_genes} = {perturbed(0).metadata(GEARS.metadata_perturbed_genes)}")

        Call gears.SetTrainingSamples(matrix, control, perturbed)

        Console.WriteLine($"  解析结果: {gears.TrainingSamples.Count} 个训练样本")

        For Each sample As PerturbSeqSample In gears.TrainingSamples
            Console.WriteLine($"    {sample.Label,-28} 扰动基因 [{String.Join(",", sample.PerturbedGeneNames)}]  " &
                              $"|Δ| 均值 {MeanAbs(sample.Delta()).ToString("F4")}")
        Next

        ' 短程试跑，证明解析出来的样本可以直接喂给训练器
        Console.WriteLine($"  用这批样本试跑 {probe.Epochs} 个 epoch 验证可用性...")

        Dim curve As Double() = gears.Train()
        Dim first As Double = curve(0)
        Dim last As Double = curve(curve.Length - 1)

        Console.WriteLine($"    损失 {first.ToString("F6")} -> {last.ToString("F6")} " &
                          If(last < first, "（下降，样本可用）", "（未下降，请检查数据）"))
        Console.WriteLine()
    End Sub

    ''' <summary>
    ''' 计算向量的绝对均值
    ''' </summary>
    ''' <param name="x">输入向量</param>
    ''' <returns>绝对值的平均数</returns>
    Private Function MeanAbs(x As Double()) As Double
        Dim sum As Double = 0

        For Each v As Double In x
            sum += std.Abs(v)
        Next

        Return If(x.Length > 0, sum / x.Length, 0.0)
    End Function

    ''' <summary>
    ''' 生成标准正态随机数（Box-Muller 变换）
    ''' </summary>
    ''' <param name="rnd">随机数发生器</param>
    ''' <returns>标准正态随机样本</returns>
    Private Function NextGaussian(rnd As Random) As Double
        Dim u1 As Double = 1.0 - rnd.NextDouble()
        Dim u2 As Double = 1.0 - rnd.NextDouble()

        Return std.Sqrt(-2.0 * std.Log(u1)) * std.Sin(2.0 * std.PI * u2)
    End Function

    ' ================================================================
    '  Save / Load 一致性验证
    ' ================================================================

    ''' <summary>
    ''' 把训练好的 GEARS 存为 zip 再加载回来，逐基因比对两者的虚拟扰动结果
    ''' </summary>
    ''' <param name="gears">已训练好的 GEARS 对象</param>
    ''' <param name="targetGenes">参与比对的单基因扰动目标</param>
    ''' <param name="combos">参与比对的组合扰动</param>
    ''' <param name="outputDir">zip 文件输出目录</param>
    ''' <returns>逐基因的对比明细，供导出 CSV</returns>
    Private Function TestSaveLoadConsistency(gears As GEARS,
                                             targetGenes As String(),
                                             combos As String()(),
                                             outputDir As String) As List(Of (Condition As String, Gene As String, Direct As Double, Loaded As Double))
        Dim rows As New List(Of (Condition As String, Gene As String, Direct As Double, Loaded As Double))()

        Console.WriteLine("[4.5] Save / Load 一致性对比")

        Dim zipPath As String = Path.Combine(outputDir, "gears_model.zip")

        Dim saveSw As Stopwatch = Stopwatch.StartNew()

        Using fs As New FileStream(zipPath, FileMode.Create, FileAccess.Write)
            Call gears.Save(fs)
        End Using

        saveSw.Stop()

        Dim restored As GEARS
        Dim loadSw As Stopwatch = Stopwatch.StartNew()

        Using fs As New FileStream(zipPath, FileMode.Open, FileAccess.Read)
            restored = GEARS.Load(fs)
        End Using

        loadSw.Stop()

        Console.WriteLine($"  保存: {zipPath}")
        Console.WriteLine($"        {((New FileInfo(zipPath)).Length / 1024.0).ToString("F1")} KB, 耗时 {saveSw.Elapsed.TotalSeconds.ToString("F2")} 秒")
        Console.WriteLine($"  加载: 耗时 {loadSw.Elapsed.TotalSeconds.ToString("F2")} 秒")
        Console.WriteLine($"  还原: {restored.GeneNames.Length} 基因, 图 {restored.GraphData.NumPriorEdges} 边, " &
                          $"模型 {restored.Model.NumLayers} 层 / 隐藏维度 {restored.Model.HiddenDim}")

        ' ---- 对同一组扰动分别用两个对象预测 ----
        Dim cases As New List(Of InterventionSpec())()

        For Each gene As String In targetGenes
            cases.Add({New InterventionSpec With {.GeneName = gene, .Mode = InterventionMode.Knockout}})
        Next

        For Each combo As String() In combos
            Dim specs As New List(Of InterventionSpec)()

            For Each gene As String In combo
                specs.Add(New InterventionSpec With {.GeneName = gene, .Mode = InterventionMode.Knockout})
            Next

            cases.Add(specs.ToArray())
        Next

        Dim sumSq As Double = 0
        Dim maxAbs As Double = 0
        Dim pairs As Integer = 0
        Dim sigDiff As Integer = 0
        Dim sigUnion As Integer = 0
        Dim directAll As New List(Of Double)()
        Dim loadedAll As New List(Of Double)()

        For Each specs As InterventionSpec() In cases
            Dim a As InterventionResult = gears.Predict(specs)
            Dim b As InterventionResult = restored.Predict(specs)
            Dim condition As String = a.Spec.GeneName & "_" & a.Spec.Mode.ToString()

            For i As Integer = 0 To a.GeneNames.Length - 1
                Dim da As Double = a.FoldChanges(i)
                Dim db As Double = b.FoldChanges(i)
                Dim diff As Double = std.Abs(da - db)

                rows.Add((condition, a.GeneNames(i), da, db))

                sumSq += (da - db) * (da - db)
                directAll.Add(da)
                loadedAll.Add(db)
                pairs += 1

                If diff > maxAbs Then
                    maxAbs = diff
                End If

                If a.IsSignificant(i) OrElse b.IsSignificant(i) Then
                    sigUnion += 1

                    If a.IsSignificant(i) <> b.IsSignificant(i) Then
                        sigDiff += 1
                    End If
                End If
            Next
        Next

        Dim rmse As Double = If(pairs > 0, std.Sqrt(sumSq / pairs), 0.0)
        Dim r As Double = Pearson(directAll.ToArray(), loadedAll.ToArray())

        Console.WriteLine($"  比对规模: {cases.Count} 个扰动条件 x 全转录组 = {pairs} 个基因级数据点")
        Console.WriteLine($"  最大绝对偏差 max|Δ FoldChange| = {maxAbs.ToString("E4")}")
        Console.WriteLine($"  均方根偏差 RMSE                = {rmse.ToString("E4")}")
        Console.WriteLine($"  Pearson 相关系数 r             = {r.ToString("F10")}")
        Console.WriteLine($"  显著基因集合差异               = {sigDiff} / {sigUnion}")

        If maxAbs < 0.000000001 AndAlso sigDiff = 0 Then
            Console.WriteLine("  结论: 两者结果完全一致（无显著差异），Save/Load 实现正确")
        ElseIf rmse < 0.001 AndAlso sigDiff = 0 Then
            Console.WriteLine("  结论: 差异远小于表达量量级（无显著差异），Save/Load 实现正确")
        Else
            Console.WriteLine("  结论: 存在显著差异，Save/Load 实现需要检查！")
        End If

        Call VerifyResumeTraining(gears, restored)

        Return rows
    End Function

    ''' <summary>
    ''' 验证从 zip 还原出来的对象保留了训练状态，可以接着训练
    ''' </summary>
    ''' <param name="source">保存前的 GEARS 对象</param>
    ''' <param name="restored">从 zip 还原的 GEARS 对象</param>
    ''' <remarks>
    ''' 判据：还原对象再训练几个 epoch 后，损失应当仍然停留在原对象收敛时的量级；
    ''' 若参数没有被正确还原（等于重新初始化），损失会回到训练刚开始时的高值。
    ''' </remarks>
    Private Sub VerifyResumeTraining(source As GEARS, restored As GEARS)
        Dim baselineLoss As Double = source.LossCurve(source.LossCurve.Length - 1)
        Dim startLoss As Double = source.LossCurve(0)

        ' 训练样本不进 zip，还原后需重新提供；内置仿真器由 seed 决定，可复现出同一批样本
        Call restored.GenerateTrainingSamples()

        restored.Options.Epochs = 3
        restored.Options.PrintEvery = 0

        Dim curve As Double() = restored.Train()
        Dim resumed As Double = curve(curve.Length - 1)

        Console.WriteLine($"  续训验证: 还原对象再训练 {restored.Options.Epochs} 轮后损失 = {resumed.ToString("F6")} " &
                          $"(原对象收敛值 {baselineLoss.ToString("F6")}，训练起始值 {startLoss.ToString("F6")})")

        If resumed < (baselineLoss + startLoss) / 2 Then
            Console.WriteLine("            停留在收敛量级，说明模型参数已被完整还原，可继续训练")
        Else
            Console.WriteLine("            损失偏高，模型参数可能未被正确还原！")
        End If
    End Sub

    ''' <summary>
    ''' 计算两个等长向量的 Pearson 相关系数
    ''' </summary>
    ''' <param name="x">第一个向量</param>
    ''' <param name="y">第二个向量</param>
    ''' <returns>相关系数；方差为 0 时返回 1（两者均无变化视为完全相关）</returns>
    Private Function Pearson(x As Double(), y As Double()) As Double
        Dim n As Integer = std.Min(x.Length, y.Length)

        If n = 0 Then
            Return 0.0
        End If

        Dim mx As Double = 0
        Dim my As Double = 0

        For i As Integer = 0 To n - 1
            mx += x(i)
            my += y(i)
        Next

        mx /= n
        my /= n

        Dim sxy As Double = 0
        Dim sxx As Double = 0
        Dim syy As Double = 0

        For i As Integer = 0 To n - 1
            Dim dx As Double = x(i) - mx
            Dim dy As Double = y(i) - my

            sxy += dx * dy
            sxx += dx * dx
            syy += dy * dy
        Next

        If sxx <= 0 OrElse syy <= 0 Then
            Return 1.0
        End If

        Return sxy / std.Sqrt(sxx * syy)
    End Function

    ''' <summary>
    ''' 导出 Save/Load 一致性逐基因对比表
    ''' </summary>
    ''' <param name="rows">对比明细</param>
    ''' <param name="outputPath">输出 CSV 路径</param>
    Private Sub ExportConsistencyTable(rows As List(Of (Condition As String, Gene As String, Direct As Double, Loaded As Double)),
                                       outputPath As String)
        Dim sb As New System.Text.StringBuilder()

        Call sb.AppendLine("# GEARS Save/Load consistency check: FoldChange of the trained object vs. the zip-restored object")
        Call sb.AppendLine(String.Join(",", "Condition", "Gene", "Direct_FoldChange", "Loaded_FoldChange", "AbsDiff"))

        For Each row In rows
            Call sb.AppendLine(String.Join(",",
                row.Condition,
                row.Gene,
                row.Direct.ToString("F10"),
                row.Loaded.ToString("F10"),
                std.Abs(row.Direct - row.Loaded).ToString("E4")))
        Next

        Call File.WriteAllText(outputPath, sb.ToString(), System.Text.Encoding.UTF8)
    End Sub
End Module
