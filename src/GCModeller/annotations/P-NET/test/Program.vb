Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.Linq

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.HTS.P_NET
Imports std = System.Math

Module Program

    ''' <summary>
    ''' 演示数据集输出的目录（位于程序运行目录之下，不会污染项目源码目录）
    ''' </summary>
    Private ReadOnly Property DemoDirectory As String
        Get
            Return Path.Combine(AppContext.BaseDirectory, "demo")
        End Get
    End Property

    Sub Main(args As String())
        Call Console.WriteLine("================================================================")
        Call Console.WriteLine(" P-NET: Biologically informed deep neural network (Nature, 2021)")
        Call Console.WriteLine(" demo: build -> train -> predict -> interpret")
        Call Console.WriteLine("================================================================")
        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 1. 生成 demo 数据集：合成的类 Reactome 通路层级 + 合成患者样本
        ' ------------------------------------------------------------------
        Dim demo As DemoDataset = DemoData.Create(geneCount:=60, sampleCount:=600, seed:=2021)
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Call demo.Save(DemoDirectory)

        Call Console.WriteLine("[1] demo dataset generated")
        Call Console.WriteLine($"    hierarchy : {demo.Hierarchy}")
        Call Console.WriteLine($"    samples   : {demo.Samples}")
        Call Console.WriteLine($"    output    : {DemoDirectory}")
        Call Console.WriteLine($"    drivers   : {String.Join(", ", demo.DriverGenes)}")
        Call Console.WriteLine($"    driver pw : {String.Join(", ", demo.DriverPathways)}")
        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 2. 校验 gmt 读写：把层级落盘之后再重新读回，拓扑应当完全一致
        ' ------------------------------------------------------------------
        Dim gmtFiles As String() = Directory _
            .GetFiles(DemoDirectory, "*.gmt") _
            .OrderBy(Function(f) f) _
            .ToArray()
        Dim reloaded As PathwayHierarchy = GmtIO.FromGmtFiles(gmtFiles)

        Call Console.WriteLine("[2] gmt round-trip check")
        Call Console.WriteLine($"    reloaded  : {reloaded}")
        Call Console.WriteLine($"    identical : {reloaded.ToString() = demo.Hierarchy.ToString()}")
        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 3. 由层级构建 P-NET：层数、节点数、连接方式全部由生物层级决定
        ' ------------------------------------------------------------------
        Dim buildConfig As New PNETBuildConfig With {
            .Seed = 42,
            .DeepSupervisionLambda = 1.0
        }
        Dim model As PNETModel = NNBuilder.Build(demo.Hierarchy, buildConfig)

        Call Console.WriteLine("[3] P-NET built from hierarchy")
        Call Console.WriteLine(model.PrintArchitecture())
        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 4. 分层划分 80% 训练 / 10% 验证 / 10% 测试
        ' ------------------------------------------------------------------
        Dim trainer As New PNETTrainer(model, New TrainConfig With {
            .Epochs = 120,
            .BatchSize = 32,
            .LearningRate = 0.001,
            .LrDecayStep = 50,
            .LrDecayFactor = 0.5,
            .UseClassWeights = True,
            .Seed = 2021,
            .Verbose = True,
            .VerboseInterval = 10
        })

        Dim split As DataSplit = trainer.StratifiedSplit(demo.Samples, 0.8, 0.1)

        ' 类别权重由训练集的类别比例决定（等价于 sklearn 的 class_weight='balanced'）
        Call trainer.FitClassWeights(split.Train)

        Call Console.WriteLine("[4] stratified split (80% / 10% / 10%)")
        Call Console.WriteLine(split.ToString())
        Call Console.WriteLine($"    class weights: positive={trainer.PositiveWeight.ToString("F4")}, negative={trainer.NegativeWeight.ToString("F4")}")
        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 5. 训练：加权 BCE + Adam + 每 50 epoch 学习率衰减
        ' ------------------------------------------------------------------
        Call Console.WriteLine("[5] training")

        watch.Restart()

        Dim history As TrainingHistory = trainer.Train(split.Train, split.Validation)

        watch.Stop()

        Call Console.WriteLine()
        Call Console.WriteLine($"    training finished in {watch.Elapsed.TotalSeconds.ToString("F2")}s")
        Call Console.WriteLine($"    final loss={history.Loss.Last().ToString("F4")}, train_auc={history.TrainAUC.Last().ToString("F4")}")

        If history.ValidationAUC.Count > 0 Then
            Call Console.WriteLine($"    best val_auc={history.BestValidationAUC.ToString("F4")} @ epoch {history.BestEpoch + 1}")
        End If
        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 6. 测试集评估
        ' ------------------------------------------------------------------
        Dim result As EvaluationResult = trainer.Evaluate(split.Test)

        Call Console.WriteLine("[6] test set evaluation")
        Call Console.WriteLine($"    {result}")
        Call Console.WriteLine($"    confusion: TP={result.TP}, FP={result.FP}, TN={result.TN}, FN={result.FN}")
        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 7. 单样本预测演示
        ' ------------------------------------------------------------------
        Call Console.WriteLine("[7] single sample prediction")

        Dim showCount As Integer = std.Min(5, split.Test.Count)

        For i As Integer = 0 To showCount - 1
            Dim vec As Double() = split.Test.GetFeatureVector(i)
            Dim p As Double = model.PredictSingle(vec)
            Dim truth As String = If(split.Test.Labels(i) > 0.5, "metastatic", "primary")

            Call Console.WriteLine($"    {split.Test.SampleNames(i)}: P-NET score={p.ToString("F4")}  ({truth})")
        Next

        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 8. DeepLIFT 归因：逐层读出基因与通路的重要性
        ' ------------------------------------------------------------------
        Call Console.WriteLine("[8] DeepLIFT attribution on the test set")

        watch.Restart()

        Dim attribution As DeepLIFTResult = DeepLIFT.Attribute(
            model,
            split.Test.Features,
            Nothing,
            split.Test.Labels
        )

        watch.Stop()

        Call Console.WriteLine($"    attribution finished in {watch.Elapsed.TotalSeconds.ToString("F2")}s")
        Call Console.WriteLine($"    conservation error = {attribution.ConservationError().ToString("E3")} " &
                               "(Σ C 应当等于 Δt，用于验证归因的守恒性)")
        Call Console.WriteLine()

        Dim importance As List(Of NodeImportance) = ImportanceAnalyzer.Analyze(attribution)

        For l As Integer = 0 To model.LayerCount - 1
            Dim top As List(Of NodeImportance) = ImportanceAnalyzer.TopPathways(importance, l, 5)
            Dim layerName As String = model.Hierarchy.GetLayerName(l)

            Call Console.WriteLine($"    --- layer {l}: {layerName} ---")

            For Each item As NodeImportance In top
                Call Console.WriteLine($"        {item}")
            Next
        Next

        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 9. 与数据生成真值比对：归因给出的 Top 基因是否命中真正的驱动基因
        ' ------------------------------------------------------------------
        Dim truthSet As New HashSet(Of String)(demo.DriverGenes)
        Dim topGenes As List(Of NodeImportance) = ImportanceAnalyzer.TopGenes(importance, 20)

        Call Console.WriteLine("[9] top-ranked genes vs. ground-truth driver genes")

        For i As Integer = 0 To std.Min(9, topGenes.Count) - 1
            Dim hit As String = If(truthSet.Contains(topGenes(i).NodeName), "  <== ground truth driver", "")

            Call Console.WriteLine($"    {i + 1,2}. {topGenes(i).NodeName,-10} score={topGenes(i).AdjustedScore.ToString("F4")}{hit}")
        Next

        Dim hitCount As Integer = topGenes.Where(Function(x) truthSet.Contains(x.NodeName)).Count()

        Call Console.WriteLine()
        Call Console.WriteLine($"    recall@{topGenes.Count} of ground-truth drivers = {hitCount}/{demo.DriverGenes.Length}")
        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 10. 改变类型层面的归因（对应论文图 3 的 Sankey 图）
        ' ------------------------------------------------------------------
        Call Console.WriteLine("[10] top gene-alteration attributions (Sankey-style)")

        For Each item As GeneAlterationImportance In ImportanceAnalyzer.TopAlterations(attribution, 10)
            Call Console.WriteLine($"    {item}")
        Next

        Call Console.WriteLine()

        ' ------------------------------------------------------------------
        ' 11. 稀疏性校验：被掩码屏蔽的连接在训练之后必须仍然恒为 0
        '     （de novo 稀疏，而不是"训练完成之后再剪掉"）
        ' ------------------------------------------------------------------
        Dim maskedTotal As Integer = 0
        Dim maskedLeaked As Integer = 0

        For Each layer As MaskedDenseLayer In model.Layers
            Dim mask As Double() = layer.Mask.Data
            Dim weight As Double() = layer.Weights.Data

            For i As Integer = 0 To mask.Length - 1
                If mask(i) = 0.0 Then
                    maskedTotal += 1

                    If weight(i) <> 0.0 Then
                        maskedLeaked += 1
                    End If
                End If
            Next
        Next

        Call Console.WriteLine("[11] de novo sparsity check")
        Call Console.WriteLine($"    masked connections        : {maskedTotal}")
        Call Console.WriteLine($"    connections leaked to != 0: {maskedLeaked}")
        Call Console.WriteLine()

        Call Console.WriteLine("demo finished.")

        If Debugger.IsAttached Then
            Call Console.ReadKey()
        End If
    End Sub

End Module
