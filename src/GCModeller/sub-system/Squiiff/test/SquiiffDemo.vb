Imports System.Diagnostics
Imports System.IO
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff
Imports SMRUCC.genomics.Analysis.Squiiff.Diffusion
Imports SMRUCC.genomics.Analysis.Squiiff.Evaluation
Imports SMRUCC.genomics.Analysis.Squiiff.IO
Imports SMRUCC.genomics.Analysis.Squiiff.Model
Imports SMRUCC.genomics.Analysis.Squiiff.NN
Imports SMRUCC.genomics.Analysis.Squiiff.Perturbation
Imports SMRUCC.genomics.Analysis.Squiiff.Training
Imports std = System.Math

''' <summary>
''' SquiDiff（条件 DDIM + 扩散自编码器）端到端演示。
'''
''' 走完 README 描述的完整链路：
''' <list type="number">
''' <item>合成单细胞扰动基准（含一个**非可加**的组合扰动，且该组合不出现在训练集中）；</item>
''' <item>导出 CSV 后用通用加载链 <c>Matrix.LoadData → BnIO.ReadGeneExpressionMatrix</c> 读回；</item>
''' <item>真实预处理链：log1p 归一化 → 高变基因选择 → 逐基因 z-score；</item>
''' <item>训练扩散自编码器（简化噪声预测损失 + β·KL，Adam + warmup/余弦退火 + 早停）；</item>
''' <item>语义编码 <c>z_sem</c> 与 DDIM 反演子码 <c>x_T</c>，检验自编码往返保真度；</item>
''' <item>估计扰动方向 <c>Δz_sem</c> 并做向量加法预测，与"对照基线"对比评估；</item>
''' <item>组合扰动外推（<c>Σ Δz</c>）检验非可加性；</item>
''' <item>隐空间线性插值生成连续过渡态；</item>
''' <item>模型存档 Save → Load 的推理一致性；</item>
''' <item>导出损失曲线 / 评估表 / PCA 坐标 / 文本报告。</item>
''' </list>
'''
''' ### 断言口径
''' 只对**与训练质量无关的确定性性质**做硬断言：训练损失下降、DDIM 往返重建、存档往返一致、
''' 插值端点一致。扰动预测的 PCC / Top-K 重叠只打印报告，避免合成数据上的训练波动导致偶发失败。
''' </summary>
Module SquiiffDemo

    ''' <summary>输出根目录。</summary>
    Public ReadOnly Property OutputDirectory As String
        Get
            Return Path.Combine(App.HOME, "Squiiff_output")
        End Get
    End Property

    ''' <summary>
    ''' 运行完整演示。
    ''' </summary>
    ''' <returns>全部硬断言通过返回 True。</returns>
    Public Function Run() As Boolean
        Dim sw As Stopwatch = Stopwatch.StartNew()
        Dim failures As New List(Of String)

        Console.WriteLine()
        Console.WriteLine(New String("="c, 78))
        Console.WriteLine("  SquiDiff：条件 DDIM + 扩散自编码器的单细胞虚拟扰动预测")
        Console.WriteLine(New String("="c, 78))
        Console.WriteLine()

        Dim outputDir As String = OutputDirectory
        Call Directory.CreateDirectory(outputDir)
        Console.WriteLine($"  输出目录: {outputDir}")
        Console.WriteLine()

        ' ==================== [1/8] 合成数据 ====================
        Console.WriteLine("[1/8] 生成合成单细胞扰动基准")

        Dim benchmark = DemoData.Generate(seed:=20240920, geneCount:=200, programCount:=12, cellsPerCondition:=50)
        Call benchmark.SaveCsv(Path.Combine(outputDir, "raw"))

        Console.WriteLine($"  {benchmark}")
        Console.WriteLine($"  条件: {String.Join(", ", benchmark.Conditions)}")
        Console.WriteLine($"  组合扰动 '{DemoData.CombinationName}' 刻意不进入训练集，用于检验非可加性")

        ' 细胞名 → 标签（加载链可能重排列序，故按名字而非下标对齐）
        Dim labelOfCellType As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim labelOfCondition As New Dictionary(Of String, String)(StringComparer.Ordinal)
        For i As Integer = 0 To benchmark.NCell - 1
            labelOfCellType(benchmark.CellNames(i)) = benchmark.CellTypeLabels(i)
            labelOfCondition(benchmark.CellNames(i)) = benchmark.ConditionLabels(i)
        Next

        ' ==================== [2/8] 加载链 + 预处理 ====================
        Console.WriteLine()
        Console.WriteLine("[2/8] 通用加载链读取 + 预处理")

        Dim countsPath As String = Path.Combine(outputDir, "raw", "single_cell_counts.csv")
        Dim loaded = ExpressionIO.LoadGeneSampleMatrix(countsPath)

        Console.WriteLine($"  原始计数: {loaded}")

        Dim logNormalized = ExpressionIO.LogNormalize(loaded)
        Dim hvg = ExpressionIO.SelectHighlyVariableGenes(logNormalized, 500)
        Dim hvgMatrix = logNormalized.SubsetGenes(hvg)
        Dim standardized = ExpressionIO.StandardizeGenes(hvgMatrix)

        Dim x0 As Tensor = standardized.ToTensor()
        Dim geneNames As String() = standardized.GeneNames

        Console.WriteLine($"  高变基因: {hvg.Length} / {logNormalized.NGene}")
        Console.WriteLine($"  标准化后: {standardized}")

        ' ==================== [3/8] 配置与训练 ====================
        Console.WriteLine()
        Console.WriteLine("[3/8] 构建并训练扩散自编码器")

        Dim config As New SquiiffConfig With {
            .DiffusionSteps = 500,
            .InferenceSteps = 40,
            .BetaStart = 0.001,
            .BetaEnd = 0.01,
            .SemanticDim = 16,
            .EncoderHiddenDim = 128,
            .EncoderBlocks = 2,
            .DenoiserHiddenDim = 128,
            .DenoiserBlocks = 3,
            .TimeEmbeddingDim = 32,
            .LearningRate = 0.001,
            .BatchSize = 64,
            .Epochs = 60,
            .BetaKL = 0.0001,
            .ClipNorm = 1.0,
            .Seed = 20240920
        }

        Dim configError = config.Validate()
        If Not String.IsNullOrEmpty(configError) Then Throw New InvalidOperationException($"配置非法: {configError}")

        Console.WriteLine($"  配置: {config.Describe()}")

        Dim model As New SquiDiff(config, geneNames)
        Console.WriteLine($"  噪声调度: {model.Model.Schedule.Describe()}")
        Console.WriteLine($"  DDIM 子轨迹: {model.Model.Sampler.InferenceSteps} 步（末步 t={model.Model.Sampler.Trajectory(model.Model.Sampler.Trajectory.Length - 1)}）")

        ' 训练集刻意排除组合扰动
        Dim trainingRows = RowsOf(standardized, labelOfCondition, benchmark.TrainingConditions())
        Dim xTrain = TakeRows(x0, trainingRows)

        Console.WriteLine($"  训练细胞: {trainingRows.Length}（排除 '{DemoData.CombinationName}'）")

        Dim history As TrainingHistory = model.Train(xTrain, epochs:=config.Epochs, verbose:=True)

        Console.WriteLine()
        Console.WriteLine(history.RenderCurve())

        Dim firstLoss = history.Records(0).TotalLoss
        Dim lastLoss = history.Records(history.Records.Count - 1).TotalLoss

        Console.WriteLine($"  首轮总损失 = {firstLoss:F6} ；末轮总损失 = {lastLoss:F6} ；最佳 = {history.BestLoss:F6}（第 {history.BestEpoch} 轮）")

        If Not (lastLoss < firstLoss) Then
            failures.Add($"训练损失未下降（首轮 {firstLoss:F6} → 末轮 {lastLoss:F6}）")
        End If

        Call history.SaveCsv(Path.Combine(outputDir, "training_history.csv"))

        ' ==================== [4/8] 双隐变量与自编码往返 ====================
        Console.WriteLine()
        Console.WriteLine("[4/8] 双隐变量（z_sem / x_T）与自编码往返保真度")

        Dim ctrlAll = TakeRows(x0, RowsOf(standardized, labelOfCondition, {DemoData.ControlName}))
        Dim zSem As Tensor = model.Semantic(ctrlAll)
        Dim subcode As Tensor = model.Subcode(ctrlAll)
        Dim reconstructed = model.Reconstruct(ctrlAll)

        Console.WriteLine($"  z_sem 形状 = [{String.Join(",", zSem.Shape)}] ；x_T 形状 = [{String.Join(",", subcode.Shape)}]")
        Console.WriteLine($"  x_T 的经验均值 = {MeanOf(subcode.Data):F4}  标准差 = {StdOf(subcode.Data):F4}")
        Console.WriteLine($"  （线性 β 调度下 ᾱ_T={model.Model.Schedule.AlphaBar(model.Model.Schedule.T):F3}，" &
                          $"x_T = √ᾱ_T·x_0 + √(1−ᾱ_T)·ε 并非纯高斯，故标准差不必为 1）")

        Dim reconstruction = RegressionMetrics.Compute(reconstructed, ctrlAll)
        Console.WriteLine($"  自编码往返 x_0 → (z_sem, x_T) → x̂_0 : {reconstruction}")

        If Double.IsNaN(reconstruction.Pcc) OrElse reconstruction.Pcc < 0.5 Then
            failures.Add($"DDIM 往返重建保真度不足（PCC = {reconstruction.Pcc:F4} < 0.5）")
        End If

        ' ==================== [5/8] 扰动方向 Δz_sem ====================
        Console.WriteLine()
        Console.WriteLine("[5/8] 估计扰动方向 Δz_sem 并检验跨细胞类型一致性")

        Dim specA As New PerturbationSpec With {
            .Name = DemoData.KnockoutA,
            .Kind = PerturbationKind.Knockout,
            .TargetGenes = New String() {"kinA"},
            .Description = "单基因敲除 A"
        }
        Dim specB As New PerturbationSpec With {
            .Name = DemoData.KnockoutB,
            .Kind = PerturbationKind.Knockout,
            .TargetGenes = New String() {"kinB"},
            .Description = "单基因敲除 B"
        }
        Dim specCombo As New PerturbationSpec With {
            .Name = DemoData.CombinationName,
            .Kind = PerturbationKind.Knockout,
            .TargetGenes = New String() {"kinA", "kinB"},
            .Description = "组合敲除（含非可加相互作用项）"
        }

        Dim pertAAll = TakeRows(x0, RowsOf(standardized, labelOfCondition, {DemoData.KnockoutA}))
        Dim pertBAll = TakeRows(x0, RowsOf(standardized, labelOfCondition, {DemoData.KnockoutB}))

        ' 池化方向（跨细胞类型平均）——用于预测
        Dim deltaA = model.RegisterPerturbation(specA, ctrlAll, pertAAll)
        Dim deltaB = model.RegisterPerturbation(specB, ctrlAll, pertBAll)

        Console.WriteLine($"  {DemoData.KnockoutA}: {LatentArithmetic.Describe(deltaA)}")
        Console.WriteLine($"  {DemoData.KnockoutB}: {LatentArithmetic.Describe(deltaB)}")

        ' 逐细胞类型的方向 → 方向一致性（同一扰动在不同细胞类型上应沿相近语义方向）
        Dim perTypeA As New List(Of Tensor)
        Dim perTypeB As New List(Of Tensor)

        For Each cellType In benchmark.CellTypes
            Dim ctrlT = TakeRows(x0, RowsOf(standardized, labelOfCellType, labelOfCondition, cellType, DemoData.ControlName))
            Dim pertAT = TakeRows(x0, RowsOf(standardized, labelOfCellType, labelOfCondition, cellType, DemoData.KnockoutA))
            Dim pertBT = TakeRows(x0, RowsOf(standardized, labelOfCellType, labelOfCondition, cellType, DemoData.KnockoutB))

            Dim dA = LatentArithmetic.EstimateDelta(model.Semantic(pertAT), model.Semantic(ctrlT))
            Dim dB = LatentArithmetic.EstimateDelta(model.Semantic(pertBT), model.Semantic(ctrlT))

            perTypeA.Add(dA)
            perTypeB.Add(dB)

            Console.WriteLine($"    {cellType,-8} ‖Δz_A‖={LatentArithmetic.Norm(dA):F3}  ‖Δz_B‖={LatentArithmetic.Norm(dB):F3}  " &
                              $"cos(Δz_A,Δz_B)={LatentArithmetic.Cosine(dA, dB):F3}")
        Next

        Dim consistencyA = MeanOfPairwiseCosine(perTypeA)
        Dim consistencyB = MeanOfPairwiseCosine(perTypeB)

        Console.WriteLine($"  跨细胞类型方向一致性: {DemoData.KnockoutA} = {consistencyA:F3} ；{DemoData.KnockoutB} = {consistencyB:F3}")
        Console.WriteLine($"  扰动空间: {model.Space}")

        ' 条件敏感度：z_sem 是否真的在调制去噪网络 ε_θ
        Dim probeX = model.Model.Schedule.AddNoise(ctrlAll, Repeat(model.Model.Schedule.T, ctrlAll.Shape(0)), model.Model.Noise(ctrlAll.Shape))
        Dim sensitivity = model.Perturbation.ConditioningSensitivity(probeX, model.Semantic(ctrlAll), deltaA)

        Console.WriteLine($"  条件敏感度 ‖Δε̂‖/‖ε̂‖: t=20 → {sensitivity(0):F4} ；t=100 → {sensitivity(1):F4} ；t=200 → {sensitivity(2):F4}")
        Console.WriteLine($"    （接近 0 表示语义条件未进入去噪轨迹，隐空间向量算术不会产生可观测效应）")

        ' ==================== [6/8] 单扰动预测与评估 ====================
        Console.WriteLine()
        Console.WriteLine("[6/8] 向量加法预测单扰动响应（对照基线对比）")

        Dim evaluationRows As New List(Of String())

        For Each cellType In benchmark.CellTypes
            Dim ctrlT = TakeRows(x0, RowsOf(standardized, labelOfCellType, labelOfCondition, cellType, DemoData.ControlName))

            For Each condition In New String() {DemoData.KnockoutA, DemoData.KnockoutB}
                Dim actualT = TakeRows(x0, RowsOf(standardized, labelOfCellType, labelOfCondition, cellType, condition))
                Dim predicted = model.PredictByPerturbation(ctrlT, condition, SubcodeMode.InheritControl)
                Dim evaluation = PerturbationMetrics.Evaluate(predicted, actualT, ctrlT, topK:=20)

                Console.WriteLine($"    {cellType,-8} {condition,-10} {evaluation}")
                Console.WriteLine($"    {"",-8} 群体效应剖面 Top{evaluation.TopK}重叠 = {evaluation.MeanProfileTopKOverlap:F3} ；" &
                                  $"逐基因 PCC 增益 = {evaluation.MeanGenePccGain:F4} ；逐细胞 ΔPCC = {evaluation.MeanCellDeltaPcc:F4}")

                evaluationRows.Add(New String() {
                    cellType, condition, "single",
                    ResultWriter.Format(evaluation.Prediction.Pcc),
                    ResultWriter.Format(evaluation.ControlBaseline.Pcc),
                    ResultWriter.Format(evaluation.PccGain),
                    ResultWriter.Format(evaluation.DeltaProfilePcc),
                    ResultWriter.Format(evaluation.MeanProfileDeltaPcc),
                    ResultWriter.Format(evaluation.MeanProfileTopKOverlap),
                    ResultWriter.Format(evaluation.MeanCellDeltaPcc),
                    ResultWriter.Format(evaluation.MeanTopKOverlap),
                    ResultWriter.Format(evaluation.MeanGenePccGain)
                })
            Next
        Next

        ' ==================== [7/8] 组合扰动外推与非可加性 ====================
        Console.WriteLine()
        Console.WriteLine("[7/8] 组合扰动外推（Σ Δz）与非可加性检验")

        Dim comboAll = TakeRows(x0, RowsOf(standardized, labelOfCondition, {DemoData.CombinationName}))
        Dim deltaComboTrue = LatentArithmetic.EstimateDelta(model.Semantic(comboAll), model.Semantic(ctrlAll))
        Dim deltaComboAdditive = model.Space.Combine(DemoData.KnockoutA, DemoData.KnockoutB)

        Dim additiveCosine = LatentArithmetic.Cosine(deltaComboAdditive, deltaComboTrue)
        Dim residualNorm = LatentArithmetic.Norm(deltaComboTrue - deltaComboAdditive)

        Console.WriteLine($"  真实组合方向 ‖Δz‖   = {LatentArithmetic.Norm(deltaComboTrue):F3}")
        Console.WriteLine($"  可加外推方向 ‖Δz‖   = {LatentArithmetic.Norm(deltaComboAdditive):F3}")
        Console.WriteLine($"  两者余弦相似度       = {additiveCosine:F3}")
        Console.WriteLine($"  残差（相互作用项）范数 = {residualNorm:F3}  ← 非零即说明扰动非可加")

        For Each cellType In benchmark.CellTypes
            Dim ctrlT = TakeRows(x0, RowsOf(standardized, labelOfCellType, labelOfCondition, cellType, DemoData.ControlName))
            Dim actualT = TakeRows(x0, RowsOf(standardized, labelOfCellType, labelOfCondition, cellType, DemoData.CombinationName))
            Dim predicted = model.PredictCombination(ctrlT, New String() {DemoData.KnockoutA, DemoData.KnockoutB}, SubcodeMode.InheritControl)
            Dim evaluation = PerturbationMetrics.Evaluate(predicted, actualT, ctrlT, topK:=20)

            Console.WriteLine($"    {cellType,-8} 组合外推 {evaluation}")

            evaluationRows.Add(New String() {
                cellType, DemoData.CombinationName, "combination(additive)",
                ResultWriter.Format(evaluation.Prediction.Pcc),
                ResultWriter.Format(evaluation.ControlBaseline.Pcc),
                ResultWriter.Format(evaluation.PccGain),
                ResultWriter.Format(evaluation.DeltaProfilePcc),
                ResultWriter.Format(evaluation.MeanProfileDeltaPcc),
                ResultWriter.Format(evaluation.MeanProfileTopKOverlap),
                ResultWriter.Format(evaluation.MeanCellDeltaPcc),
                ResultWriter.Format(evaluation.MeanTopKOverlap),
                ResultWriter.Format(evaluation.MeanGenePccGain)
            })
        Next

        ' ==================== [8/8] 插值 / 存档 / 导出 ====================
        Console.WriteLine()
        Console.WriteLine("[8/8] 隐空间插值、模型存档往返与结果导出")

        ' 8.1 插值轨迹语义端点一致性
        Dim zCtrl = LatentArithmetic.MeanVector(model.Semantic(ctrlAll))
        Dim zPertA = LatentArithmetic.MeanVector(model.Semantic(pertAAll))
        Dim trajectory = LatentArithmetic.LerpTrajectory(zCtrl, zPertA, 5)

        Dim endpointError0 = MaxAbsDifference(trajectory(0).Data, zCtrl.Data)
        Dim endpointError1 = MaxAbsDifference(trajectory(trajectory.Count - 1).Data, zPertA.Data)

        Console.WriteLine($"  插值轨迹 {trajectory.Count} 点；端点最大绝对误差 α=0: {endpointError0:E2} / α=1: {endpointError1:E2}")

        If endpointError0 > 1.0E-12 OrElse endpointError1 > 1.0E-12 Then
            failures.Add($"插值端点与组中心不一致（{endpointError0:E2} / {endpointError1:E2}）")
        End If

        ' 8.2 逐点解码：观察从对照到扰动的连续过渡（用群体平均剖面，压掉个体噪声）
        Dim decodedTrajectory = model.InterpolateBetween(ctrlAll, pertAAll, 5, SubcodeMode.InheritControl)
        Dim trueProfile = SubtractMeanProfile(pertAAll, ctrlAll)

        For i As Integer = 0 To decodedTrajectory.Count - 1
            Dim alpha = CDbl(i) / (decodedTrajectory.Count - 1)
            Dim deltaProfile = SubtractMeanProfile(decodedTrajectory(i), ctrlAll)
            Dim pcc = RegressionMetrics.Pearson(deltaProfile, trueProfile)

            Console.WriteLine($"    α={alpha:F2}  群体平均效应剖面 vs 真实效应剖面: PCC={pcc:F4}")
        Next

        ' 8.3 存档往返一致性
        Dim archivePath As String = Path.Combine(outputDir, "squiiff_model.zip")
        Call model.Save(archivePath)

        Dim reloaded As SquiDiff = SquiDiff.Load(archivePath)
        Dim ctrlT0 = TakeRows(x0, RowsOf(standardized, labelOfCellType, labelOfCondition, benchmark.CellTypes(0), DemoData.ControlName))

        ' 扰动登记表（PerturbationSpace）属于实验侧的元数据，不随模型存档落盘，
        ' 因此这里直接用原始 Δz 张量在存档前后各预测一次做逐位比对。
        Dim deltaForArchive = model.Space.DeltaOf(DemoData.KnockoutA)

        Dim beforeSave = model.PredictPerturbation(ctrlT0, deltaForArchive, SubcodeMode.InheritControl)
        Dim afterLoad = reloaded.PredictPerturbation(ctrlT0, deltaForArchive, SubcodeMode.InheritControl)

        Dim archiveError = MaxAbsDifference(beforeSave.Data, afterLoad.Data)
        Console.WriteLine($"  存档往返: Save → Load 后预测的最大绝对偏差 = {archiveError:E3}")

        If archiveError > 1.0E-09 Then
            failures.Add($"存档往返后推理结果不一致（最大绝对偏差 {archiveError:E3}）")
        End If

        ' 8.4 导出
        Call ResultWriter.WriteTextTable(
            Path.Combine(outputDir, "perturbation_evaluation.csv"),
            New String() {"cell_type", "condition", "mode", "pcc", "baseline_pcc", "pcc_gain",
                          "cell_delta_pcc", "mean_profile_delta_pcc", "mean_profile_top20_overlap",
                          "cell_delta_pcc_mean", "cell_top20_overlap", "gene_pcc_gain"},
            evaluationRows)

        Dim zAll = model.Semantic(x0)
        Dim pca = PcaProjection.Fit(zAll, 2)
        Dim coords = pca.Project2D(zAll)
        Dim pcaRows As New List(Of String())
        For i As Integer = 0 To standardized.NCell - 1
            pcaRows.Add(New String() {
                labelOfCellType(standardized.CellNames(i)),
                labelOfCondition(standardized.CellNames(i)),
                ResultWriter.Format(coords(i)(0)),
                ResultWriter.Format(coords(i)(1))
            })
        Next

        Call ResultWriter.WriteTextTable(
            Path.Combine(outputDir, "latent_pca.csv"),
            New String() {"cell_type", "condition", "pc1", "pc2"},
            pcaRows)

        Console.WriteLine($"  PCA 解释方差占比: PC1={pca.ExplainedVarianceRatio(0):F3} PC2={pca.ExplainedVarianceRatio(1):F3}")

        Call WriteReport(outputDir, config, benchmark, history, reconstruction, consistencyA, consistencyB,
                         additiveCosine, residualNorm, archiveError, failures)

        ' ==================== 汇总 ====================
        sw.Stop()

        Console.WriteLine()
        Console.WriteLine(New String("-"c, 78))
        Console.WriteLine($"  演示耗时: {sw.Elapsed.TotalSeconds:F1}s")
        Console.WriteLine($"  硬断言: {(If(failures.Count = 0, "全部通过", failures.Count & " 项失败"))}")

        For Each f In failures
            Console.WriteLine($"    [FAIL] {f}")
        Next

        For Each file In ResultWriter.ListFiles(outputDir)
            Console.WriteLine($"    {file}")
        Next

        Console.WriteLine(New String("="c, 78))

        Return failures.Count = 0
    End Function

#Region "行选择 / 张量切片"

    ''' <summary>按扰动条件取细胞行下标。</summary>
    Private Function RowsOf(matrix As CellExpressionMatrix,
                            conditionOf As Dictionary(Of String, String),
                            conditions As String()) As Integer()
        Dim wanted As New HashSet(Of String)(conditions, StringComparer.OrdinalIgnoreCase)
        Dim result As New List(Of Integer)

        For i As Integer = 0 To matrix.NCell - 1
            Dim condition As String = Nothing
            If conditionOf.TryGetValue(matrix.CellNames(i), condition) AndAlso wanted.Contains(condition) Then
                result.Add(i)
            End If
        Next

        Return result.ToArray()
    End Function

    ''' <summary>按（细胞类型 × 扰动条件）取细胞行下标。</summary>
    Private Function RowsOf(matrix As CellExpressionMatrix,
                            cellTypeOf As Dictionary(Of String, String),
                            conditionOf As Dictionary(Of String, String),
                            cellType As String, condition As String) As Integer()
        Dim result As New List(Of Integer)

        For i As Integer = 0 To matrix.NCell - 1
            Dim name = matrix.CellNames(i)
            Dim t As String = Nothing
            Dim c As String = Nothing

            If cellTypeOf.TryGetValue(name, t) AndAlso conditionOf.TryGetValue(name, c) Then
                If String.Equals(t, cellType, StringComparison.OrdinalIgnoreCase) AndAlso
                   String.Equals(c, condition, StringComparison.OrdinalIgnoreCase) Then
                    result.Add(i)
                End If
            End If
        Next

        Return result.ToArray()
    End Function

    ''' <summary>从 <c>[cell, gene]</c> 张量中抽取指定行，组成新批量。</summary>
    Private Function TakeRows(source As Tensor, rows As Integer()) As Tensor
        Dim columns = source.Shape(1)
        Dim batch = New Tensor(New Integer() {rows.Length, columns})
        Dim src = source.Data
        Dim dst = batch.Data

        For i As Integer = 0 To rows.Length - 1
            Call Array.Copy(src, rows(i) * columns, dst, i * columns, columns)
        Next

        Call batch.MarkHostModified()
        Return batch
    End Function

#End Region

#Region "统计辅助"

    Private Function MeanOf(values As Double()) As Double
        If values Is Nothing OrElse values.Length = 0 Then Return Double.NaN

        Dim sum As Double = 0.0
        For Each v In values
            sum += v
        Next
        Return sum / values.Length
    End Function

    Private Function StdOf(values As Double()) As Double
        If values Is Nothing OrElse values.Length < 2 Then Return Double.NaN

        Dim m = MeanOf(values)
        Dim ss As Double = 0.0
        For Each v In values
            ss += (v - m) * (v - m)
        Next
        Return std.Sqrt(ss / (values.Length - 1))
    End Function

    Private Function MaxAbsDifference(a As Double(), b As Double()) As Double
        Dim n = std.Min(a.Length, b.Length)
        Dim maxDiff As Double = 0.0
        For i As Integer = 0 To n - 1
            maxDiff = std.Max(maxDiff, std.Abs(a(i) - b(i)))
        Next
        Return maxDiff
    End Function

    ''' <summary>同一扰动在不同细胞类型上的 Δz 两两余弦相似度均值。</summary>
    Private Function MeanOfPairwiseCosine(deltas As List(Of Tensor)) As Double
        If deltas.Count < 2 Then Return Double.NaN

        Dim values As New List(Of Double)
        For i As Integer = 0 To deltas.Count - 2
            For j As Integer = i + 1 To deltas.Count - 1
                Dim c = LatentArithmetic.Cosine(deltas(i), deltas(j))
                If Not Double.IsNaN(c) Then values.Add(c)
            Next
        Next

        If values.Count = 0 Then Return Double.NaN
        Return MeanOf(values.ToArray())
    End Function

    ''' <summary>构造长度 <paramref name="count"/>、元素全为 <paramref name="value"/> 的时间步数组。</summary>
    Private Function Repeat(value As Integer, count As Integer) As Integer()
        Dim result(count - 1) As Integer
        For i As Integer = 0 To count - 1
            result(i) = value
        Next
        Return result
    End Function

    ''' <summary>某批细胞的平均表达谱（长度 = 基因数）。</summary>
    Private Function MeanProfile(cells As Tensor) As Double()
        Dim rows = cells.Shape(0)
        Dim columns = cells.Shape(1)
        Dim data = cells.Data
        Dim result(columns - 1) As Double

        For i As Integer = 0 To rows - 1
            Dim offset = i * columns
            For j As Integer = 0 To columns - 1
                result(j) += data(offset + j)
            Next
        Next
        For j As Integer = 0 To columns - 1
            result(j) /= rows
        Next

        Return result
    End Function

    ''' <summary>两组细胞平均表达谱之差（预测/真实的"扰动效应剖面"）。</summary>
    Private Function SubtractMeanProfile(a As Tensor, b As Tensor) As Double()
        Dim pa = MeanProfile(a)
        Dim pb = MeanProfile(b)
        Dim result(pa.Length - 1) As Double

        For i As Integer = 0 To pa.Length - 1
            result(i) = pa(i) - pb(i)
        Next

        Return result
    End Function

#End Region

    ''' <summary>写出人类可读的文本报告。</summary>
    Private Sub WriteReport(outputDir As String, config As SquiiffConfig, benchmark As SyntheticBenchmark,
                            history As TrainingHistory, reconstruction As RegressionMetrics,
                            consistencyA As Double, consistencyB As Double,
                            additiveCosine As Double, residualNorm As Double,
                            archiveError As Double, failures As List(Of String))
        Dim lines As New List(Of String)

        lines.Add("SquiDiff 端到端演示报告")
        lines.Add(New String("="c, 70))
        lines.Add($"生成时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
        lines.Add($"数据集:   {benchmark}")
        lines.Add($"条件:     {String.Join(", ", benchmark.Conditions)}")
        lines.Add($"配置:     {config.Describe()}")
        lines.Add(New String("-"c, 70))
        lines.Add($"训练轮数:       {history.Records.Count}")
        lines.Add($"首轮总损失:     {history.Records(0).TotalLoss:F6}")
        lines.Add($"末轮总损失:     {history.Records(history.Records.Count - 1).TotalLoss:F6}")
        lines.Add($"最佳总损失:     {history.BestLoss:F6}（第 {history.BestEpoch} 轮）")
        lines.Add($"训练耗时:       {history.TotalSeconds:F1}s")
        lines.Add(New String("-"c, 70))
        lines.Add($"自编码往返重建: {reconstruction}")
        lines.Add($"跨细胞类型方向一致性  {DemoData.KnockoutA}: {consistencyA:F4}")
        lines.Add($"跨细胞类型方向一致性  {DemoData.KnockoutB}: {consistencyB:F4}")
        lines.Add(New String("-"c, 70))
        lines.Add("非可加性检验（组合扰动）：")
        lines.Add($"  可加外推 Δz 与真实组合 Δz 的余弦相似度: {additiveCosine:F4}")
        lines.Add($"  残差（相互作用项）范数:                 {residualNorm:F4}")
        lines.Add(New String("-"c, 70))
        lines.Add($"存档往返推理最大绝对偏差: {archiveError:E3}")
        lines.Add(New String("="c, 70))

        If failures.Count = 0 Then
            lines.Add("硬断言: 全部通过")
        Else
            lines.Add($"硬断言: {failures.Count} 项失败")
            For Each f In failures
                lines.Add($"  [FAIL] {f}")
            Next
        End If

        Call File.WriteAllLines(Path.Combine(outputDir, "report.txt"), lines)
    End Sub
End Module
