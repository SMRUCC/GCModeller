Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Nodes
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow.Compute
Imports Microsoft.VisualBasic.MachineLearning.Transformer
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.GPS
Imports SMRUCC.genomics.Analysis.GPS.Perturbation
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Data
Imports SMRUCC.genomics.Analysis.GPS.Graph
Imports SMRUCC.genomics.Analysis.GPS.Model
Imports SMRUCC.genomics.Analysis.GPS.Training
Imports SMRUCC.genomics.Analysis.GEARS.Layers
Imports GEARSGraph = SMRUCC.genomics.Analysis.GEARS.Graph
Imports GNN = Microsoft.VisualBasic.DeepLearning.GNN

''' <summary>
''' GPS 数据管线与图编码自测：预处理、滑窗切分、SPD/度编码/LapPE 的正确性。
''' </summary>
Module Program

    Private passed As Integer
    Private failed As Integer

    ''' <summary>自测入口；带 <c>--demo</c> 时改跑 TestData1 端到端 DEMO。</summary>
    Function Main(args As String()) As Integer
        ' ---- DEMO 分发：不带 --demo 时行为与既有自测完全一致 ----
        If args IsNot Nothing AndAlso args.Any(Function(a) String.Equals(a, "--demo", StringComparison.OrdinalIgnoreCase)) Then
            Return DemoTestData1.Run(args)
        End If

        If args IsNot Nothing AndAlso args.Any(Function(a) String.Equals(a, "--help", StringComparison.OrdinalIgnoreCase)) Then
            Call DemoTestData1.PrintUsage()
            Return 0
        End If

        Console.WriteLine("== GPS 数据管线与图编码自测 ==")

        Call TestPreprocess()
        Call TestWindows()
        Call TestGraphEncoding()
        Call TestBiasedAttentionGradient()
        Call TestMpnnGradient()
        Call TestGpsLayerGradient()
        Call TestGruGradient()
        Call TestGrnModelGradient()
        Call TestTrainingSmoke()
        Call TestPerturbationFacade()
        Call TestModelPersistence()
        Call TestCheckpointResume()
        Call TestMixedPrecisionParity()

        Console.WriteLine()
        Console.WriteLine($"{passed} passed, {failed} failed")

        Return If(failed = 0, 0, 1)
    End Function

    ' ==================== 预处理 ====================

    Private Sub TestPreprocess()
        Console.WriteLine()
        Console.WriteLine("[预处理]")

        Dim data As GeneExpressionData = CreateExpression()
        Dim config As New GPSConfig With {
            .MaxGenes = 0,
            .CpmNormalize = True,
            .LogTransform = True,
            .ZScore = True
        }

        Dim pre As PreprocessedExpression = ExpressionPreprocess.Preprocess(data, config)

        Call Check(pre.NGene = data.NGene, "基因数保持")
        Call Check(pre.NSample = data.NSample, "样本数保持")
        Call Check(pre.Matrix.GetLength(0) = pre.NGene AndAlso pre.Matrix.GetLength(1) = pre.NSample, "矩阵尺寸一致")

        ' 逐基因 z-score：均值≈0、样本标准差≈1
        Dim maxAbsMean As Double = 0.0
        Dim maxSdError As Double = 0.0

        For i As Integer = 0 To pre.NGene - 1
            Dim sum As Double = 0.0

            For j As Integer = 0 To pre.NSample - 1
                sum += pre.Matrix(i, j)
            Next

            Dim mean As Double = sum / pre.NSample
            maxAbsMean = System.Math.Max(maxAbsMean, System.Math.Abs(mean))

            Dim ss As Double = 0.0

            For j As Integer = 0 To pre.NSample - 1
                Dim d As Double = pre.Matrix(i, j) - mean
                ss += d * d
            Next

            Dim sd As Double = System.Math.Sqrt(ss / (pre.NSample - 1))
            maxSdError = System.Math.Max(maxSdError, System.Math.Abs(sd - 1.0))
        Next

        Call Check(maxAbsMean < 1.0E-09, $"z-score 均值归零（偏差 {maxAbsMean:G3}）")
        Call Check(maxSdError < 1.0E-09, $"z-score 标准差归一（偏差 {maxSdError:G3}）")

        ' 反标准化：z=0 对应标准化前的基因均值
        Call CheckNear(pre.Denormalize(0, 0.0), pre.GeneMeans(0), 1.0E-12, "反标准化回均值")

        ' 高变异过滤 + 强制保留 TF
        Dim filtered As PreprocessedExpression = ExpressionPreprocess.Preprocess(
            data,
            New GPSConfig With {.MaxGenes = 6},
            New String() {"TF1"})

        Call Check(filtered.NGene <= 20, "过滤后基因数不超过原值")
        Call Check(filtered.GeneNames.Contains("TF1"), "强制保留的 TF1 未被过滤")
        Call Check(IsAscending(filtered.GeneNames, data.GeneNames), "过滤后基因顺序保持原序")
    End Sub

    ' ==================== 滑窗 ====================

    Private Sub TestWindows()
        Console.WriteLine()
        Console.WriteLine("[滑窗数据集]")

        Dim data As GeneExpressionData = CreateExpression()
        Dim config As New GPSConfig With {
            .MaxGenes = 0,
            .Window = 3,
            .TrainRatio = 0.6,
            .ValidationRatio = 0.2
        }

        Dim pre As PreprocessedExpression = ExpressionPreprocess.Preprocess(data, config)
        Dim dataset As TimeSeriesWindowDataset = TimeSeriesWindowDataset.Create(pre, config)

        ' 8 个唯一时间点、窗口 3 ⇒ 5 个窗口
        Call Check(dataset.NTimePoint = 8, $"唯一时间点数（{dataset.NTimePoint}）")
        Call Check(dataset.Count = 5, $"窗口数量（{dataset.Count}）")
        Call Check(dataset.Profiles.GetLength(1) = 8, "重复样本按时间点聚合")
        Call Check(dataset.Train().Count = 3, $"训练集窗口（{dataset.Train().Count}）")
        Call Check(dataset.Validation().Count = 1, $"验证集窗口（{dataset.Validation().Count}）")
        Call Check(dataset.Test().Count = 1, $"测试集窗口（{dataset.Test().Count}）")

        ' 严格时间顺序：目标时刻单调递增，且三段首尾相接无重叠
        Dim ascending As Boolean = True

        For k As Integer = 1 To dataset.Windows.Count - 1
            If dataset.Windows(k).TargetTime <= dataset.Windows(k - 1).TargetTime Then
                ascending = False
            End If
        Next

        Call Check(ascending, "窗口按目标时刻升序")

        Dim first As TimeSeriesWindow = dataset.Windows(0)
        Call Check(first.Input.GetLength(0) = dataset.NGene AndAlso first.Input.GetLength(1) = 3, "窗口输入形状 [基因, w]")
        Call Check(first.Target.Length = dataset.NGene, "目标向量长度")
        Call CheckNear(first.Previous(0), first.Input(0, 2), 1.0E-12, "窗口最后一列即 Previous")

        Dim lastWindow As TimeSeriesWindow = dataset.Windows(dataset.Windows.Count - 1)
        Call CheckNear(lastWindow.TargetTime, dataset.UniqueTimes(dataset.NTimePoint - 1), 1.0E-12, "末窗口目标为最后时间点")
    End Sub

    ' ==================== 图编码 ====================

    Private Sub TestGraphEncoding()
        Console.WriteLine()
        Console.WriteLine("[图编码]")

        Dim data As GeneExpressionData = CreateExpression()
        Dim prior As PriorNetwork = CreatePrior()
        Dim config As New GPSConfig With {
            .MaxGenes = 0,
            .PeDim = 4,
            .SpdMaxDist = 2
        }

        Dim pre As PreprocessedExpression = ExpressionPreprocess.Preprocess(data, config)
        Dim graph As New GEARSGraph.GeneRegulatoryGraph(pre.GeneNames, prior)
        Dim encoding As GraphEncoding = GraphEncoding.Compute(graph, config)

        Call Check(encoding.NGene = 20, $"节点数（{encoding.NGene}）")
        Call Check(encoding.SpdBucketCount = 5, $"SPD 桶数（{encoding.SpdBucketCount}）")

        ' 对角线恒为自身桶；矩阵对称
        Dim diagonalOk As Boolean = True
        Dim symmetricOk As Boolean = True

        For i As Integer = 0 To encoding.NGene - 1
            If encoding.SpdBuckets(i, i) <> GraphEncoding.SelfBucket Then diagonalOk = False

            For j As Integer = 0 To encoding.NGene - 1
                If encoding.SpdBuckets(i, j) <> encoding.SpdBuckets(j, i) Then symmetricOk = False
            Next
        Next

        Call Check(diagonalOk, "对角桶恒为自身")
        Call Check(symmetricOk, "SPD 桶矩阵对称")

        Dim tf1 As Integer = Array.IndexOf(pre.GeneNames, "TF1")
        Dim g1 As Integer = Array.IndexOf(pre.GeneNames, "G1")
        Dim g13 As Integer = Array.IndexOf(pre.GeneNames, "G13")
        Dim g17 As Integer = Array.IndexOf(pre.GeneNames, "G17")
        Dim g18 As Integer = Array.IndexOf(pre.GeneNames, "G18")

        Call Check(encoding.SpdBuckets(tf1, g1) = 1, "TF1→G1 距离 1")
        Call Check(encoding.SpdBuckets(tf1, g13) = 2, "TF1→G13 距离 2")
        Call Check(encoding.SpdBuckets(tf1, g17) = 3, "TF1→G17 超距归入远距离桶")
        Call Check(encoding.SpdBuckets(tf1, g18) = 4, "孤立基因归入不连通桶")

        ' 度编码
        Call Check(encoding.OutDegree(tf1) = 6, $"TF1 出度（{encoding.OutDegree(tf1)}）")
        Call Check(encoding.InDegree(g1) = 1, $"G1 入度（{encoding.InDegree(g1)}）")
        Call Check(encoding.OutDegree(g1) = 1, $"G1 出度（{encoding.OutDegree(g1)}）")
        Call Check(encoding.InDegreeBucket(g1) = 1, "入度桶编号")
        Call Check(encoding.InDegreeBucket(g18) = 0, "无连接基因入度桶为 0")

        ' LapPE：形状与单位范数
        Call Check(encoding.LapPeDim = 4, $"LapPE 维度（{encoding.LapPeDim}）")
        Call Check(encoding.LapPe.GetLength(0) = 20 AndAlso encoding.LapPe.GetLength(1) = 4, "LapPE 形状 [N, K]")

        Dim normOk As Boolean = True

        For c As Integer = 0 To encoding.LapPeDim - 1
            Dim ss As Double = 0.0

            For i As Integer = 0 To encoding.NGene - 1
                ss += encoding.LapPe(i, c) * encoding.LapPe(i, c)
            Next

            If System.Math.Abs(ss - 1.0) > 1.0E-06 Then normOk = False
        Next

        Call Check(normOk, "LapPE 各列单位范数")

        ' 确定性：同图重复计算得到相同结果
        Dim again As GraphEncoding = GraphEncoding.Compute(graph, config)
        Dim reproducible As Boolean = True

        For c As Integer = 0 To encoding.LapPeDim - 1
            If System.Math.Abs(encoding.LapPe(0, c) - again.LapPe(0, c)) > 1.0E-12 Then reproducible = False
        Next

        Call Check(reproducible, "LapPE 符号修正后结果可复现")
    End Sub

    ' ==================== 梯度校验 ====================

    ''' <summary>
    ''' 偏置多头注意力的有限差分梯度校验（参数 + 输入）。
    ''' </summary>
    Private Sub TestBiasedAttentionGradient()
        Console.WriteLine()
        Console.WriteLine("[偏置注意力梯度]")

        TensorOps.Seed = 20240718

        Dim store As New ParameterStore()
        Dim attn As New BiasedMultiHeadAttention(4, 4, 2, 8, 5, store, "testAttn")
        Dim input As Tensor = RandomTensor(New Integer() {6, 8}, 0.3)
        Dim spd As Integer(,) = CreateSpdBuckets(6, 5)

        Dim forwardLoss As Func(Of Double) = Function()
                                                      Dim o As Tensor = attn.Forward(input, spd)
                                                      Dim s As Double = 0.0

                                                      For Each v As Double In o.Data
                                                          s += v * v
                                                      Next

                                                      Return 0.5 * s
                                                  End Function

        Dim output As Tensor = attn.Forward(input, spd)

        ' L = 0.5·Σo² 对输出的梯度即 o 本身
        Call store.ZeroGradients()
        Dim dInput As Tensor = attn.Backward(TensorOps.CloneTensor(output))

        Call Check(store.Entries.Count = 8, $"偏置注意力参数项（{store.Entries.Count}）")
        Call Check(dInput IsNot Nothing AndAlso dInput.Shape(0) = 6 AndAlso dInput.Shape(1) = 8, "输入梯度形状 [N, d]")
        Call CheckGradient(store, forwardLoss, 5.0E-04, "偏置注意力")
        Call CheckInputGradient(input, dInput, forwardLoss, 5.0E-04, "偏置注意力输入梯度")
    End Sub

    ''' <summary>
    ''' 单独校验消息传递支路（复用 GEARSConvLayer）的梯度，用于区分
    ''' "图卷积反向错误" 与 "多层复合的数值条件"。
    ''' </summary>
    Private Sub TestMpnnGradient()
        Console.WriteLine()
        Console.WriteLine("[MPNN 梯度]")

        TensorOps.Seed = 20240720

        Dim genes As String() = New String() {"TF1", "G1", "G2", "G3", "G4", "G5"}
        Dim prior As PriorNetwork = CreateSmallPrior()
        Dim graph As New GEARSGraph.GeneRegulatoryGraph(genes, prior)

        ' 用线性激活单独校验图卷积的聚合与散射梯度
        ' （GNN.ActivationFunctions 的 Tanh 是 Single 精度，会掩盖 1% 量级的梯度差异）
        Dim mpnn As New GEARSConvLayer(8, 8, GNN.ActivationType.None, False, False, 3, "mpnnOnly")
        Dim store As New ParameterStore()
        Dim p As List(Of Tensor) = mpnn.GetParameters()
        Dim g As List(Of Tensor) = mpnn.GetGradients()

        Call store.RegisterExternal("mpnnOnly.wSelf", p(0), g(0))
        Call store.RegisterExternal("mpnnOnly.bias", p(1), g(1))
        Call store.RegisterExternal("mpnnOnly.wRel", p(2), g(2))

        Dim input As Tensor = RandomTensor(New Integer() {6, 8}, 0.3)

        Dim forwardLoss As Func(Of Double) = Function()
                                                      Dim o As Tensor = mpnn.Forward(input, graph)
                                                      Dim s As Double = 0.0

                                                      For Each v As Double In o.Data
                                                          s += v * v
                                                      Next

                                                      Return 0.5 * s
                                                  End Function

        Dim output As Tensor = mpnn.Forward(input, graph)

        Call store.ZeroGradients()
        Dim dInput As Tensor = mpnn.Backward(output, graph)

        Call CheckInputGradient(input, dInput, forwardLoss, 1.0E-06, "MPNN 输入梯度")
        Call CheckGradient(store, forwardLoss, 1.0E-06, "MPNN")
    End Sub

    ''' <summary>
    ''' GPS 融合层（MPNN + 偏置注意力 + FFN + AddNorm）的有限差分梯度校验。
    ''' </summary>
    Private Sub TestGpsLayerGradient()
        Console.WriteLine()
        Console.WriteLine("[GPS 融合层梯度]")

        TensorOps.Seed = 20240719

        Dim store As New ParameterStore()
        Dim layer As New GPSLayer(8, 12, 2, 5, store,
                                  useMpnn:=True, useAttn:=True, dropout:=0.0, seed:=11, name:="testGps")

        Dim genes As String() = New String() {"TF1", "G1", "G2", "G3", "G4", "G5"}
        Dim prior As PriorNetwork = CreateSmallPrior()
        Dim graph As New GEARSGraph.GeneRegulatoryGraph(genes, prior)
        Dim input As Tensor = RandomTensor(New Integer() {6, 8}, 0.3)
        Dim spd As Integer(,) = CreateSpdBuckets(6, 5)

        Dim forwardLoss As Func(Of Double) = Function()
                                                      Dim o As Tensor = layer.Forward(input, graph, spd, False)
                                                      Dim s As Double = 0.0

                                                      For Each v As Double In o.Data
                                                          s += v * v
                                                      Next

                                                      Return 0.5 * s
                                                  End Function

        Dim output As Tensor = layer.Forward(input, graph, spd, False)

        Call store.ZeroGradients()
        Dim dInput As Tensor = layer.Backward(output, graph)

        ' MPNN 3 项 + 注意力 8 项 + FFN 4 项 = 15
        Call Check(store.Entries.Count = 15, $"GPS 层参数项（{store.Entries.Count}）")
        Call Check(dInput.Shape(0) = 6 AndAlso dInput.Shape(1) = 8, "GPS 层输入梯度形状")
        Call CheckGradient(store, forwardLoss, 5.0E-04, "GPS 融合层")
        Call CheckInputGradient(input, dInput, forwardLoss, 5.0E-04, "GPS 层输入梯度")

        ' 消融分支可运行：关闭两条支路时退化为纯 FFN
        Dim store2 As New ParameterStore()
        Dim ffnOnly As New GPSLayer(8, 12, 2, 5, store2,
                                    useMpnn:=False, useAttn:=False, dropout:=0.0, seed:=12, name:="ffnOnly")
        Dim output2 As Tensor = ffnOnly.Forward(input, graph, spd, False)

        Call Check(output2.Shape(0) = 6 AndAlso output2.Shape(1) = 8, "纯 FFN 消融可运行")
        Call Check(store2.Entries.Count = 4, $"纯 FFN 参数项（{store2.Entries.Count}）")
    End Sub

    ''' <summary>
    ''' 模型持久化往返：保存为 zip 包、重新加载后预测必须与原模型一致。
    ''' </summary>
    Private Sub TestModelPersistence()
        Console.WriteLine()
        Console.WriteLine("[模型持久化]")

        TensorOps.Seed = 20240726

        Dim data As GeneExpressionData = CreateExpression()
        Dim prior As PriorNetwork = CreatePrior()
        Dim config As New GPSConfig With {
            .MaxGenes = 0,
            .Window = 3,
            .DModel = 16,
            .NLayers = 1,
            .NHeads = 2,
            .FfnDim = 24,
            .GruHidden = 12,
            .PeDim = 4,
            .SpdMaxDist = 2,
            .Dropout = 0.0,
            .PretrainEpochs = 0,
            .FinetuneEpochs = 1,
            .LearningRate = 0.01,
            .PrintEvery = 0
        }

        Dim gps As New GPSModel(data, prior, config)
        Call gps.Train(0)

        Dim tempFile As String = Path.Combine(Path.GetTempPath(), $"gps_selftest_{Guid.NewGuid():N}{GPSFile.FileExtension}")

        Try
            Call GPSFile.Save(gps, tempFile)

            Call Check(GPSFile.IsPackage(tempFile), "模型包可识别")
            Call Check(New FileInfo(tempFile).Length > 0, "模型包非空")

            Dim manifest As GPSPackageManifest = GPSFile.ReadManifest(tempFile)

            Call Check(manifest.NGenes = gps.GeneNames.Length, $"清单基因数（{manifest.NGenes}）")
            Call Check(manifest.Window = gps.Dataset.WindowSize, $"清单窗口长度（{manifest.Window}）")

            Dim loaded As GPSModel = GPSFile.Load(tempFile)

            Call Check(loaded.IsTrained, "加载后保持已训练标记")
            Call Check(loaded.GeneNames.Length = gps.GeneNames.Length, "加载后基因数一致")
            Call Check(loaded.Dataset.WindowSize = gps.Dataset.WindowSize, "加载后窗口长度一致")

            Dim expected As InterventionResult = gps.KnockoutGene("TF1")
            Dim actual As InterventionResult = loaded.KnockoutGene("TF1")
            Dim maxDifference As Double = 0.0

            For i As Integer = 0 To expected.FoldChanges.Length - 1
                maxDifference = System.Math.Max(maxDifference,
                                                System.Math.Abs(expected.FoldChanges(i) - actual.FoldChanges(i)))
            Next

            Call Check(maxDifference <= 1.0E-12, $"往返预测一致（最大差异 {maxDifference:G3}）")

        Finally
            If File.Exists(tempFile) Then Call File.Delete(tempFile)
        End Try
    End Sub

    ''' <summary>
    ''' checkpoint 与中断恢复续训：
    ''' ① 默认关闭不落盘；② v2 包清单元信息；③ Adam 动量 TSV 往返；
    ''' ④ 中断-恢复轨迹与连续训练逐位一致（参数 + 损失曲线）；
    ''' ⑤ v1 包（缺 optimizer.tsv / state 字段）降级加载；⑥ 最优 epoch=0 的回滚修复。
    ''' </summary>
    Private Sub TestCheckpointResume()
        Console.WriteLine()
        Console.WriteLine("[checkpoint 与续训]")

        TensorOps.Seed = 20240725

        Dim data As GeneExpressionData = CreateExpression()
        Dim prior As PriorNetwork = CreatePrior()

        Dim folder As String = Path.Combine(Path.GetTempPath(), $"gps_ckpt_{Guid.NewGuid():N}")
        Dim fullPath As String = Path.Combine(folder, "continuous" & GPSFile.FileExtension)
        Dim ckptPath As String = Path.Combine(folder, "ckpt" & GPSFile.FileExtension)
        Dim v1Path As String = Path.Combine(folder, "v1compat" & GPSFile.FileExtension)

        Try
            ' ---- 1. 默认关闭：interval=0 时即使设置了 CheckpointPath 也不落盘 ----
            Dim plain As GPSModel = NewCheckpointTestModel(data, prior, pretrainEpochs:=2, finetuneEpochs:=2, checkpointInterval:=0)
            plain.CheckpointPath = ckptPath
            Call plain.Train(0)

            Call Check(Not File.Exists(ckptPath), "interval=0 不产生 checkpoint 文件")

            ' ---- 2. 连续训练基准（间隔 1 epoch，滚动覆盖同一文件） ----
            Dim continuous As GPSModel = NewCheckpointTestModel(data, prior, pretrainEpochs:=3, finetuneEpochs:=3, checkpointInterval:=1)
            continuous.CheckpointPath = fullPath
            Dim continuousResult As GPSTrainingResult = continuous.Train(0)

            Call Check(File.Exists(fullPath), "interval=1 每 epoch 落盘")

            Dim manifest As GPSPackageManifest = GPSFile.ReadManifest(fullPath)

            Call Check(manifest.Version = 2, $"包版本升级为 2（实际 {manifest.Version}）")
            Call Check(manifest.StepIndex > 0, $"清单记录步计数（{manifest.StepIndex}）")
            Call Check(manifest.PretrainEpochsDone = 3 AndAlso manifest.FinetuneEpochsDone = 3,
                       $"清单记录阶段进度（{manifest.PretrainEpochsDone}/{manifest.FinetuneEpochsDone}）")
            Call Check(continuous.TrainerState IsNot Nothing AndAlso
                       continuous.TrainerState.StepIndex = manifest.StepIndex,
                       "TrainerState 与清单一致")

            ' ---- 3. Adam 动量 TSV 往返 ----
            Dim store As ParameterStore = continuous.Model.Parameters
            Dim momentaTsv As String = store.SaveMomenta()
            Dim probe As ParameterEntry = store.Entries(0)
            Dim beforeM As Double() = CType(probe.State.FirstMoment.Data.Clone(), Double())

            For i As Integer = 0 To probe.State.FirstMoment.Data.Length - 1
                probe.State.FirstMoment.Data(i) = 0.0
            Next

            Call store.RestoreMomenta(momentaTsv)

            Dim maxMomentDiff As Double = 0.0

            For i As Integer = 0 To beforeM.Length - 1
                maxMomentDiff = System.Math.Max(maxMomentDiff,
                                                System.Math.Abs(beforeM(i) - probe.State.FirstMoment.Data(i)))
            Next

            Call Check(maxMomentDiff <= 1.0E-12, $"动量 TSV 往返无损（最大差异 {maxMomentDiff:G3}）")

            ' ---- 4. 中断-恢复：前半段 + checkpoint，加载后续跑，与连续训练对拍 ----
            Dim interrupted As GPSModel = NewCheckpointTestModel(data, prior, pretrainEpochs:=3, finetuneEpochs:=3, checkpointInterval:=1)
            Dim partialTrainer As New GPSTrainer(interrupted.Model, interrupted.Config)

            ' 只跑预训练前 2 轮，模拟"意外中断"
            Call partialTrainer.Pretrain(interrupted.Dataset.Train(), epochs:=2, printEvery:=0)

            ' 隔离断言 A：同配置全新实例跑同样 2 轮，应与 interrupted 逐位一致
            ' （验证初始化与训练路径的确定性——续训轨迹一致的前提）
            Dim probeModel As GPSModel = NewCheckpointTestModel(data, prior, pretrainEpochs:=2, finetuneEpochs:=2, checkpointInterval:=0)
            Dim probeTrainer As New GPSTrainer(probeModel.Model, probeModel.Config)
            Call probeTrainer.Pretrain(probeModel.Dataset.Train(), epochs:=2, printEvery:=0)

            Dim dbgProbeDiff As Double = 0.0

            For Each entry As ParameterEntry In probeModel.Model.Parameters.Entries
                Dim twin As ParameterEntry = interrupted.Model.Parameters.Find(entry.Name)
                Dim a As Double() = entry.Value.Data
                Dim b As Double() = twin.Value.Data

                For i As Integer = 0 To a.Length - 1
                    dbgProbeDiff = System.Math.Max(dbgProbeDiff, System.Math.Abs(a(i) - b(i)))
                Next
            Next

            Call Check(dbgProbeDiff <= 1.0E-12, $"两实例同训 2 轮参数一致（最大差异 {dbgProbeDiff:G3}）")

            Call GPSFile.SaveCheckpoint(interrupted, ckptPath, partialTrainer.CaptureState())

            Call Check(File.Exists(ckptPath), "中断现场已存为 checkpoint")

            ' 模拟进程重启：从 checkpoint 恢复并续跑剩余轮次
            Dim resumed As GPSModel = GPSFile.Load(ckptPath)

            ' 隔离断言 B：加载后、续训前的参数应与 interrupted 逐位一致
            ' （验证参数与动量的存取往返无损）
            Dim dbgLoadDiff As Double = 0.0

            For Each entry As ParameterEntry In interrupted.Model.Parameters.Entries
                Dim twin As ParameterEntry = resumed.Model.Parameters.Find(entry.Name)
                Dim a As Double() = entry.Value.Data
                Dim b As Double() = twin.Value.Data

                For i As Integer = 0 To a.Length - 1
                    dbgLoadDiff = System.Math.Max(dbgLoadDiff, System.Math.Abs(a(i) - b(i)))
                Next
            Next

            Call Check(dbgLoadDiff <= 1.0E-12, $"存取往返参数一致（最大差异 {dbgLoadDiff:G3}）")

            resumed.CheckpointPath = ckptPath
            Dim resumedResult As GPSTrainingResult = resumed.Train(resumeFromCheckpoint:=True, printEvery:=0)

            Call Check(resumed.TrainerState IsNot Nothing AndAlso
                       resumed.TrainerState.PretrainEpochsDone = 3 AndAlso
                       resumed.TrainerState.FinetuneEpochsDone = 3,
                       "续训后 TrainerState 反映完成进度")
            Call Check(resumedResult.PretrainLoss.Length = 3,
                       $"预训练曲线前缀拼接（{resumedResult.PretrainLoss.Length} 段）")
            Call Check(resumedResult.TrainLoss.Length = 3,
                       $"微调曲线长度（{resumedResult.TrainLoss.Length} 段）")

            ' 4a. 全部参数逐位对拍
            Dim maxParamDiff As Double = 0.0

            For Each entry As ParameterEntry In continuous.Model.Parameters.Entries
                Dim twin As ParameterEntry = resumed.Model.Parameters.Find(entry.Name)

                If twin Is Nothing Then
                    Call Check(False, $"续训模型缺少参数 {entry.Name}")
                    Continue For
                End If

                Dim a As Double() = entry.Value.Data
                Dim b As Double() = twin.Value.Data

                For i As Integer = 0 To a.Length - 1
                    maxParamDiff = System.Math.Max(maxParamDiff, System.Math.Abs(a(i) - b(i)))
                Next
            Next

            Call Check(maxParamDiff <= 1.0E-12, $"续训轨迹与连续训练一致（参数最大差异 {maxParamDiff:G3}）")

            ' 4b. 损失曲线逐位对拍
            Dim maxLossDiff As Double = 0.0

            For i As Integer = 0 To continuousResult.TrainLoss.Length - 1
                maxLossDiff = System.Math.Max(maxLossDiff,
                                              System.Math.Abs(continuousResult.TrainLoss(i) - resumedResult.TrainLoss(i)))
            Next

            For i As Integer = 0 To continuousResult.PretrainLoss.Length - 1
                maxLossDiff = System.Math.Max(maxLossDiff,
                                              System.Math.Abs(continuousResult.PretrainLoss(i) - resumedResult.PretrainLoss(i)))
            Next

            Call Check(maxLossDiff <= 1.0E-12, $"损失曲线一致（最大差异 {maxLossDiff:G3}）")

            ' ---- 5. v1 包降级：去掉 optimizer.tsv 与 training.json 内的 state 字段 ----
            Call WriteV1CompatiblePackage(ckptPath, v1Path)

            Dim degraded As GPSModel = GPSFile.Load(v1Path)

            Call Check(degraded.IsTrained, "v1 包仍标记已训练")
            Call Check(degraded.TrainerState Is Nothing, "v1 包无训练状态（降级）")
            Call Check(degraded.KnockoutGene("TF1") IsNot Nothing, "v1 包加载后可正常推理")

            ' ---- 6. 最优 epoch=0 的回滚修复 ----
            ' 大学习率 + 早停耐心 1：第 0 轮验证最优、第 1 轮变差即停。
            ' 修复前（BestEpoch > 0 条件）不会回滚，终值停留在第 1 轮；
            ' 修复后终值应回到第 0 轮快照，即等于只跑 1 轮微调的模型参数。
            ' 切分比例压低训练集占比，保证小数据集也有验证窗口
            Dim oneEpoch As GPSModel = NewCheckpointTestModel(data, prior, pretrainEpochs:=0, finetuneEpochs:=1,
                                                              checkpointInterval:=0, learningRate:=6.0,
                                                              trainRatio:=0.4, validationRatio:=0.4,
                                                              earlyStopPatience:=1)
            Call oneEpoch.Train(0)

            Dim rollback As GPSModel = NewCheckpointTestModel(data, prior, pretrainEpochs:=0, finetuneEpochs:=4,
                                                              checkpointInterval:=0, learningRate:=6.0,
                                                              trainRatio:=0.4, validationRatio:=0.4,
                                                              earlyStopPatience:=1)
            Dim rollbackResult As GPSTrainingResult = rollback.Train(0)

            Call Check(rollbackResult.BestEpoch = 0, $"第 0 轮即验证最优（BestEpoch={rollbackResult.BestEpoch}）")
            Call Check(rollbackResult.TrainLoss.Length = 2, $"耐心 1 触发早停（{rollbackResult.TrainLoss.Length} 轮）")

            Dim maxRollbackDiff As Double = 0.0

            For Each entry As ParameterEntry In oneEpoch.Model.Parameters.Entries
                Dim twin As ParameterEntry = rollback.Model.Parameters.Find(entry.Name)
                Dim a As Double() = entry.Value.Data
                Dim b As Double() = twin.Value.Data

                For i As Integer = 0 To a.Length - 1
                    maxRollbackDiff = System.Math.Max(maxRollbackDiff, System.Math.Abs(a(i) - b(i)))
                Next
            Next

            Call Check(maxRollbackDiff <= 1.0E-12, $"最优 epoch=0 正确回滚（最大差异 {maxRollbackDiff:G3}）")

        Finally
            If Directory.Exists(folder) Then Call Directory.Delete(folder, recursive:=True)
        End Try
    End Sub

    ''' <summary>构造 checkpoint 测试用的小模型（数据/先验由调用方给定）。</summary>
    ''' <param name="data">表达矩阵</param>
    ''' <param name="prior">先验网络</param>
    ''' <param name="pretrainEpochs">预训练轮数</param>
    ''' <param name="finetuneEpochs">微调轮数</param>
    ''' <param name="checkpointInterval">checkpoint 间隔（epoch）</param>
    ''' <param name="learningRate">学习率</param>
    ''' <param name="trainRatio">训练集时间点占比</param>
    ''' <param name="validationRatio">验证集时间点占比</param>
    ''' <param name="earlyStopPatience">早停耐心值</param>
    Private Function NewCheckpointTestModel(data As GeneExpressionData,
                                            prior As PriorNetwork,
                                            pretrainEpochs As Integer,
                                            finetuneEpochs As Integer,
                                            checkpointInterval As Integer,
                                            Optional learningRate As Double = 0.01,
                                            Optional trainRatio As Double = 0.7,
                                            Optional validationRatio As Double = 0.15,
                                            Optional earlyStopPatience As Integer = 15) As GPSModel

        Dim config As New GPSConfig With {
            .MaxGenes = 0,
            .Window = 3,
            .DModel = 16,
            .NLayers = 1,
            .NHeads = 2,
            .FfnDim = 24,
            .GruHidden = 12,
            .PeDim = 4,
            .SpdMaxDist = 2,
            .Dropout = 0.0,
            .PretrainEpochs = pretrainEpochs,
            .FinetuneEpochs = finetuneEpochs,
            .LearningRate = learningRate,
            .PrintEvery = 0,
            .EarlyStopPatience = earlyStopPatience,
            .Seed = 77,
            .TrainRatio = trainRatio,
            .ValidationRatio = validationRatio,
            .CheckpointIntervalEpochs = checkpointInterval
        }

        ' HeNormalInit 消费 TensorOps 的全局随机流：每次构造前重设种子，
        ' 保证"连续训练"与"中断-恢复"两个模型的初始权重完全一致
        TensorOps.Seed = 20240725

        Return New GPSModel(data, prior, config)
    End Function

    ''' <summary>
    ''' 由 v2 包派生一个 v1 兼容包：剔除 <c>optimizer.tsv</c> 条目并去掉
    ''' <c>training.json</c> 中的 <c>state</c> 字段（旧格式没有这两样东西）。
    ''' </summary>
    ''' <param name="source">v2 包路径</param>
    ''' <param name="target">输出路径</param>
    Private Sub WriteV1CompatiblePackage(source As String, target As String)
        Using sourceArchive As ZipArchive = ZipFile.OpenRead(source)
            Using targetStream As New FileStream(target, FileMode.CreateNew, FileAccess.Write)
                Using targetArchive As New ZipArchive(targetStream, ZipArchiveMode.Create)

                    For Each entry As ZipArchiveEntry In sourceArchive.Entries
                        If String.Equals(entry.Name, "optimizer.tsv", StringComparison.Ordinal) Then Continue For

                        Dim content As String

                        Using reader As New StreamReader(entry.Open(), Encoding.UTF8)
                            content = reader.ReadToEnd()
                        End Using

                        If String.Equals(entry.Name, "training.json", StringComparison.Ordinal) Then
                            Dim node As JsonNode = JsonNode.Parse(content)
                            Dim obj As JsonObject = CType(node, JsonObject)

                            Call obj.Remove("state")
                            content = obj.ToJsonString(New JsonSerializerOptions With {.WriteIndented = True})
                        End If

                        Dim newEntry As ZipArchiveEntry = targetArchive.CreateEntry(entry.Name, CompressionLevel.Optimal)

                        Using writer As New StreamWriter(newEntry.Open(), New UTF8Encoding(False))
                            Call writer.Write(content)
                        End Using
                    Next

                End Using
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' 混合精度对拍：热点 GEMM 走单精度与整体走双精度的前向结果应一致，
    ''' 且诊断计数应证明单精度路径确实被命中。
    ''' </summary>
    Private Sub TestMixedPrecisionParity()
        Console.WriteLine()
        Console.WriteLine("[混合精度对拍]")

        TensorOps.Seed = 20240725

        ' 64 个基因 × d_model=128 使 FFN 的 GEMM 超过策略阈值（262144）
        Const nGenes As Integer = 64

        Dim genes As String() = Enumerable.Range(0, nGenes).
            Select(Function(i) $"G{i}").
            ToArray()

        Dim prior As New PriorNetwork()

        For i As Integer = 1 To 20
            Call prior.AddEdge("G0", $"G{i}", Effector.Activator, 1.0, "")
        Next

        Call prior.AddEdge("G1", "G21", Effector.Activator, 1.0, "")
        Call prior.AddEdge("G21", "G22", Effector.Inhibitor, 1.0, "")

        Dim config As New GPSConfig With {
            .MaxGenes = 0,
            .Window = 2,
            .DModel = 128,
            .NLayers = 1,
            .NHeads = 4,
            .FfnDim = 128,
            .GruHidden = 32,
            .PeDim = 0,
            .SpdMaxDist = 2,
            .Dropout = 0.0,
            .TrainRatio = 0.6,
            .ValidationRatio = 0.2
        }

        Dim graph As New GEARSGraph.GeneRegulatoryGraph(genes, prior)
        Dim encoding As GraphEncoding = GraphEncoding.Compute(graph, config)
        Dim model As New GRNModel(graph, encoding, config)

        Dim input As Double(,) = New Double(nGenes - 1, config.Window - 1) {}
        Dim r As New Random(31)

        For i As Integer = 0 To nGenes - 1
            For t As Integer = 0 To config.Window - 1
                input(i, t) = (r.NextDouble() * 2.0 - 1.0) * 0.5
            Next
        Next

        ' ---- 纯双精度 ----
        MixedPrecisionPolicy.Enabled = False
        MixedPrecisionDiagnostics.Reset()

        Dim reference As Tensor = model.Forward(input, False)
        Dim referenceAccelerated As Long = MixedPrecisionDiagnostics.[Of]("MatMul").Accelerated

        ' ---- 热点单精度 ----
        MixedPrecisionPolicy.Enabled = True
        MixedPrecisionDiagnostics.Reset()

        Dim mixed As Tensor = model.Forward(input, False)
        Dim mixedAccelerated As Long = MixedPrecisionDiagnostics.[Of]("MatMul").Accelerated

        Dim referenceNorm As Double = 0.0
        Dim mixedNorm As Double = 0.0
        Dim diffNorm As Double = 0.0

        For i As Integer = 0 To reference.Data.Length - 1
            referenceNorm += reference.Data(i) * reference.Data(i)
            mixedNorm += mixed.Data(i) * mixed.Data(i)

            Dim d As Double = mixed.Data(i) - reference.Data(i)
            diffNorm += d * d
        Next

        Dim relative As Double = System.Math.Sqrt(diffNorm) /
                                 System.Math.Max(1.0E-12, System.Math.Max(referenceNorm, mixedNorm))

        Call Check(referenceAccelerated = 0, "关闭混合精度后没有单精度 GEMM")
        Call Check(mixedAccelerated > 0, $"热点 GEMM 命中单精度（{mixedAccelerated} 次）")
        Call Check(relative <= 1.0E-04, $"单精度与双精度前向一致（相对 L2 差 {relative:G3}）")

        MixPrecisionRestore()
    End Sub

    ''' <summary>恢复混合精度默认设置，避免影响后续用例。</summary>
    Private Sub MixPrecisionRestore()
        MixedPrecisionPolicy.Enabled = True
        MixedPrecisionDiagnostics.Reset()
    End Sub

    ''' <summary>
    ''' 门面与 InsilicoPerturbationExperiment 接口：敲除/敲低/过表达、组合扰动与级联。
    ''' </summary>
    Private Sub TestPerturbationFacade()
        Console.WriteLine()
        Console.WriteLine("[虚拟扰动接口]")

        TensorOps.Seed = 20240724

        Dim data As GeneExpressionData = CreateExpression()
        Dim prior As PriorNetwork = CreatePrior()
        Dim config As New GPSConfig With {
            .MaxGenes = 0,
            .Window = 3,
            .DModel = 16,
            .NLayers = 2,
            .NHeads = 2,
            .FfnDim = 24,
            .GruHidden = 12,
            .PeDim = 4,
            .SpdMaxDist = 2,
            .Dropout = 0.0,
            .PretrainEpochs = 2,
            .FinetuneEpochs = 6,
            .LearningRate = 0.005,
            .PrintEvery = 0
        }

        Dim gps As New GPSModel(data, prior, config)
        Dim result As GPSTrainingResult = gps.Train(0)

        Call Check(gps.IsTrained, "训练完成标记")
        Call Check(result.FinetuneEpochs > 0, "门面训练轮数")

        ' ---- 敲除 ----
        Dim ko As InterventionResult = gps.KnockoutGene("TF1")

        Call Check(Not ko.Undefined, "敲除结果已定义")
        Call Check(ko.GeneNames.Length = gps.GeneNames.Length, "结果基因数与模型一致")
        Call Check(ko.FoldChanges.Length = gps.GeneNames.Length, "FoldChanges 长度")
        Call Check(ko.IsSignificant(Array.IndexOf(gps.GeneNames, "TF1")), "被扰动基因标记为显著")
        Call Check(ko.GetTopChangedGenes(5).Count = 5, "Top 变化基因可输出")

        ' ---- 敲低 / 过表达 ----
        Dim kd As InterventionResult = gps.KnockDownGene("TF1")
        Dim oe As InterventionResult = gps.OverexpressGene("TF1")

        Call Check(Not kd.Undefined AndAlso Not oe.Undefined, "敲低与过表达结果已定义")
        Call Check(kd.FoldChanges(Array.IndexOf(gps.GeneNames, "TF1")) <= 0.0, "敲低使目标基因下调")
        Call Check(oe.FoldChanges(Array.IndexOf(gps.GeneNames, "TF1")) >= 0.0, "过表达使目标基因上调")

        ' ---- 组合扰动 ----
        Dim combo As CombinationResult = gps.PredictCombination(New String() {"TF1", "TF2"}, InterventionMode.Knockout)

        Call Check(combo.Joint.FoldChanges.Length = gps.GeneNames.Length, "组合扰动联合结果")
        Call Check(combo.Additive.Length = gps.GeneNames.Length, "加性期望长度")
        Call Check(combo.Synergy.Length = gps.GeneNames.Length, "非加性效应长度")
        Call Check(Not Double.IsNaN(combo.SynergyScore), "协同分数可计算")

        ' ---- 多步级联 ----
        Dim cascade As InterventionResult = gps.SimulateCascade("TF1", InterventionMode.Knockout, 3)

        Call Check(cascade.DynamicTrajectory IsNot Nothing, "级联轨迹已生成")
        Call Check(cascade.DynamicTrajectory.GetLength(0) = 4, "轨迹时间步数（1 + steps）")
        Call Check(cascade.DynamicTrajectory.GetLength(1) = gps.GeneNames.Length, "轨迹基因数")

        ' ---- 未知基因降级 ----
        Dim unknown As InterventionResult = gps.KnockoutGene("NOT_A_GENE")

        Call Check(unknown.Undefined, "未知基因返回 Undefined")
    End Sub

    ''' <summary>
    ''' 端到端训练冒烟：两阶段训练、损失下降、评估指标与持续性基线。
    ''' </summary>
    Private Sub TestTrainingSmoke()
        Console.WriteLine()
        Console.WriteLine("[训练冒烟]")

        TensorOps.Seed = 20240723

        Dim data As GeneExpressionData = CreateExpression()
        Dim config As New GPSConfig With {
            .MaxGenes = 0,
            .Window = 3,
            .DModel = 16,
            .NLayers = 2,
            .NHeads = 2,
            .FfnDim = 24,
            .GruHidden = 12,
            .PeDim = 4,
            .SpdMaxDist = 2,
            .Dropout = 0.0,
            .TrainRatio = 0.6,
            .ValidationRatio = 0.2,
            .PretrainEpochs = 3,
            .FinetuneEpochs = 12,
            .LearningRate = 0.005,
            .PrintEvery = 0
        }

        Dim pre As PreprocessedExpression = ExpressionPreprocess.Preprocess(data, config, New String() {"TF1", "TF2"})
        Dim prior As PriorNetwork = CreatePrior()
        Dim graph As New GEARSGraph.GeneRegulatoryGraph(pre.GeneNames, prior)
        Dim encoding As GraphEncoding = GraphEncoding.Compute(graph, config)
        Dim dataset As TimeSeriesWindowDataset = TimeSeriesWindowDataset.Create(pre, config)

        Dim model As New GRNModel(graph, encoding, config)

        ' 记录训练前的参数快照，用于确认确实发生了学习
        Dim before As Double() = CType(model.Parameters.Entries(0).Value.Data.Clone(), Double())

        Dim trainer As New GPSTrainer(model, config)
        Dim pretrainLoss As List(Of Double) = trainer.Pretrain(dataset.Train())
        Dim result As GPSTrainingResult = trainer.Finetune(dataset.Train(), dataset.Validation())

        Call Check(pretrainLoss.Count = config.PretrainEpochs, "预训练轮数")
        Call Check(result.FinetuneEpochs > 0, "微调轮数")
        Call Check(result.TrainLoss.All(Function(v) Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v)), "训练损失有限")

        Dim changed As Boolean = False

        For i As Integer = 0 To before.Length - 1
            If System.Math.Abs(before(i) - model.Parameters.Entries(0).Value.Data(i)) > 1.0E-12 Then changed = True
        Next

        Call Check(changed, "训练后参数发生变化")

        ' 学习应当降低训练损失（允许少量噪声）
        Call Check(result.TrainLoss.Last() <= result.TrainLoss.First() * 1.05,
                   $"训练损失下降（{result.TrainLoss.First():G4} → {result.TrainLoss.Last():G4}）")

        ' ---- 评估：模型 vs 历史持续性基线 ----
        Dim testWindows As List(Of TimeSeriesWindow) = dataset.Test()

        If testWindows.Count = 0 Then testWindows = dataset.Validation()

        Dim isolated As Boolean() = Evaluator.IsolatedGeneMask(model)
        Dim metrics As EvaluationMetrics = Evaluator.Evaluate(model, testWindows, 5, isolated)
        Dim baseline As EvaluationMetrics = Evaluator.EvaluatePersistence(testWindows, isolated)

        Call Check(metrics.NWindows = testWindows.Count, "评估窗口数")
        Call Check(Not Double.IsNaN(metrics.R2), "R² 可计算")
        Call Check(metrics.DirectionAccuracy >= 0.0 AndAlso metrics.DirectionAccuracy <= 1.0, "方向准确率范围")
        Call Check(Not Double.IsNaN(baseline.MSE), "持续性基线可计算")
        Call Check(isolated.Where(Function(v) v).Count() > 0, "存在先验图外基因")

        Console.WriteLine($"  模型: {metrics}")
        Console.WriteLine($"  基线: {baseline}")
    End Sub

    ''' <summary>
    ''' 批量 GRU 的 BPTT 梯度校验（参数 + 输入序列）。
    ''' </summary>
    Private Sub TestGruGradient()
        Console.WriteLine()
        Console.WriteLine("[GRU 梯度]")

        TensorOps.Seed = 20240722

        Const steps As Integer = 3
        Const batch As Integer = 6
        Const inputSize As Integer = 4
        Const hidden As Integer = 5

        Dim store As New ParameterStore()
        Dim gru As New BatchGRU(inputSize, hidden, store, "testGru")
        Dim sequence As Tensor = RandomTensor(New Integer() {steps, batch, inputSize}, 0.4)

        Dim forwardLoss As Func(Of Double) = Function()
                                                      Dim r As (AllHidden As Tensor, FinalHidden As Tensor) = gru.ForwardSequence(sequence)
                                                      Dim s As Double = 0.0

                                                      For Each v As Double In r.FinalHidden.Data
                                                          s += v * v
                                                      Next

                                                      Return 0.5 * s
                                                  End Function

        Dim result As (AllHidden As Tensor, FinalHidden As Tensor) = gru.ForwardSequence(sequence)
        Dim dAll As Tensor = New Tensor(steps, batch, hidden)

        ' 只有最后一步的隐藏状态进入下游
        Call System.Array.Copy(result.FinalHidden.Data, 0,
                               dAll.Data, (steps - 1) * batch * hidden, batch * hidden)
        Call dAll.MarkHostModified()

        Call store.ZeroGradients()
        Dim dSequence As Tensor = gru.BackwardSequence(dAll)

        Call CheckGradient(store, forwardLoss, 5.0E-05, "GRU")
        Call CheckInputGradient(sequence, dSequence, forwardLoss, 5.0E-05, "GRU 输入梯度")
    End Sub

    ''' <summary>
    ''' 完整 GRN 模型（初始嵌入 + GPS 堆叠 + GRU + 解码器）的端到端梯度校验。
    ''' </summary>
    Private Sub TestGrnModelGradient()
        Console.WriteLine()
        Console.WriteLine("[GRN 模型梯度]")

        TensorOps.Seed = 20240721

        Dim genes As String() = New String() {"TF1", "G1", "G2", "G3", "G4", "G5"}
        Dim prior As PriorNetwork = CreateSmallPrior()
        Dim graph As New GEARSGraph.GeneRegulatoryGraph(genes, prior)

        Dim config As New GPSConfig With {
            .DModel = 8,
            .NLayers = 2,
            .NHeads = 2,
            .FfnDim = 12,
            .Window = 3,
            .GruHidden = 6,
            .PeDim = 3,
            .SpdMaxDist = 2,
            .Dropout = 0.0,
            .MaxGenes = 0,
            .TrainRatio = 0.6,
            .ValidationRatio = 0.2
        }

        Dim encoding As GraphEncoding = GraphEncoding.Compute(graph, config)
        Dim model As New GRNModel(graph, encoding, config)

        Dim input As Double(,) = New Double(genes.Length - 1, config.Window - 1) {}
        Dim r As New Random(13)

        For i As Integer = 0 To input.GetLength(0) - 1
            For t As Integer = 0 To input.GetLength(1) - 1
                input(i, t) = (r.NextDouble() * 2.0 - 1.0) * 0.5
            Next
        Next

        Dim forwardLoss As Func(Of Double) = Function()
                                                      Dim o As Tensor = model.Forward(input, False)
                                                      Dim s As Double = 0.0

                                                      For Each v As Double In o.Data
                                                          s += v * v
                                                      Next

                                                      Return 0.5 * s
                                                  End Function

        Dim output As Tensor = model.Forward(input, False)

        Call Check(output.Shape(0) = genes.Length AndAlso output.Shape(1) = 1, "预测形状 [N, 1]")

        Dim finite As Boolean = True

        For Each v As Double In output.Data
            If Double.IsNaN(v) OrElse Double.IsInfinity(v) Then finite = False
        Next

        Call Check(finite, "预测值有限")

        Call model.Parameters.ZeroGradients()
        Dim dInput As Double(,) = model.Backward(output)

        Call Check(dInput.GetLength(0) = genes.Length AndAlso dInput.GetLength(1) = config.Window,
                   "输入梯度形状 [N, w]")
        Call CheckGradient(model.Parameters, forwardLoss, 5.0E-03, "GRN 模型")
        Call CheckArrayGradient(input, dInput, forwardLoss, 5.0E-03, "GRN 模型输入梯度")
    End Sub

    ''' <summary>
    ''' 对二维输入数组抽样做有限差分校验。
    ''' </summary>
    Private Sub CheckArrayGradient(input As Double(,),
                                   analytic As Double(,),
                                   forwardLoss As Func(Of Double),
                                   tolerance As Double,
                                   label As String)

        Dim rows As Integer = input.GetLength(0)
        Dim cols As Integer = input.GetLength(1)
        Dim total As Integer = rows * cols
        Dim stepSize As Integer = System.Math.Max(1, total \ 6)
        Dim sumNumeric As Double = 0.0
        Dim sumAnalytic As Double = 0.0
        Dim sumDiff As Double = 0.0

        For k As Integer = 0 To total - 1 Step stepSize
            Dim i As Integer = k \ cols
            Dim t As Integer = k Mod cols
            Const eps As Double = 1.0E-05
            Dim original As Double = input(i, t)

            input(i, t) = original + eps
            Dim plus As Double = forwardLoss()
            input(i, t) = original - eps
            Dim minus As Double = forwardLoss()
            input(i, t) = original

            Dim numeric As Double = (plus - minus) / (2.0 * eps)
            Dim analyticValue As Double = analytic(i, t)

            sumNumeric += numeric * numeric
            sumAnalytic += analyticValue * analyticValue
            Dim diff As Double = numeric - analyticValue
            sumDiff += diff * diff
        Next

        Dim relative As Double = RelativeVectorError(sumNumeric, sumAnalytic, sumDiff)

        Call Check(relative < tolerance, $"{label}（相对 L2 误差 {relative:G3}）")
    End Sub

    ''' <summary>
    ''' 逐个参数抽样比对解析梯度与中心差分梯度。
    ''' </summary>
    ''' <param name="store">参数登记表（梯度已由反向传播填充）</param>
    ''' <param name="forwardLoss">给定当前参数重新计算损失的闭包</param>
    ''' <param name="tolerance">相对误差上限</param>
    ''' <param name="label">用例名</param>
    Private Sub CheckGradient(store As ParameterStore,
                              forwardLoss As Func(Of Double),
                              tolerance As Double,
                              label As String)

        Dim sumNumeric As Double = 0.0
        Dim sumAnalytic As Double = 0.0
        Dim sumDiff As Double = 0.0
        Dim worstValue As Double = 0.0
        Dim worst As String = ""

        For Each entry As ParameterEntry In store.Entries
            Dim data As Double() = entry.Value.Data
            Dim grad As Double() = entry.Gradient.Data
            Dim stepSize As Integer = System.Math.Max(1, data.Length \ 3)

            For k As Integer = 0 To data.Length - 1 Step stepSize
                Dim numeric As Double = NumericGradient(data, k, forwardLoss, entry.Value)
                Dim analytic As Double = grad(k)
                Dim diff As Double = System.Math.Abs(numeric - analytic)

                sumNumeric += numeric * numeric
                sumAnalytic += analytic * analytic
                sumDiff += diff * diff

                If diff > worstValue Then
                    worstValue = diff
                    worst = $"{entry.Name}[{k}] numeric={numeric:G6} analytic={analytic:G6}"
                End If
            Next
        Next

        ' 以整个梯度向量的相对 L2 误差为准：接近零的分量不会因为相对误差放大而误报
        Dim relative As Double = RelativeVectorError(sumNumeric, sumAnalytic, sumDiff)

        Call Check(relative < tolerance,
                   $"{label}参数梯度一致（相对 L2 误差 {relative:G3}；最大绝对差 {worstValue:G3} @ {worst}）")
    End Sub

    ''' <summary>
    ''' 对输入张量抽样做有限差分校验。
    ''' </summary>
    Private Sub CheckInputGradient(input As Tensor,
                                   analytic As Tensor,
                                   forwardLoss As Func(Of Double),
                                   tolerance As Double,
                                   label As String)

        Dim data As Double() = input.Data
        Dim sumNumeric As Double = 0.0
        Dim sumAnalytic As Double = 0.0
        Dim sumDiff As Double = 0.0
        Dim stepSize As Integer = System.Math.Max(1, data.Length \ 6)

        For k As Integer = 0 To data.Length - 1 Step stepSize
            Dim numeric As Double = NumericGradient(data, k, forwardLoss, input)
            Dim analyticValue As Double = analytic.Data(k)

            sumNumeric += numeric * numeric
            sumAnalytic += analyticValue * analyticValue
            Dim diff As Double = numeric - analyticValue
            sumDiff += diff * diff
        Next

        Dim relative As Double = RelativeVectorError(sumNumeric, sumAnalytic, sumDiff)

        Call Check(relative < tolerance, $"{label}（相对 L2 误差 {relative:G3}）")
    End Sub

    ''' <summary>
    ''' 对张量底层数组的第 <paramref name="index"/> 个元素做中心差分。
    ''' </summary>
    Private Function NumericGradient(data As Double(),
                                     index As Integer,
                                     forwardLoss As Func(Of Double),
                                     owner As Tensor) As Double

        Const eps As Double = 1.0E-05
        Dim original As Double = data(index)

        data(index) = original + eps
        Call owner.MarkHostModified()
        Dim plus As Double = forwardLoss()

        data(index) = original - eps
        Call owner.MarkHostModified()
        Dim minus As Double = forwardLoss()

        data(index) = original
        Call owner.MarkHostModified()

        Return (plus - minus) / (2.0 * eps)
    End Function

    ''' <summary>
    ''' 梯度向量的相对 L2 误差：<c>|num − ana| / max(|num|, |ana|)</c>。
    ''' </summary>
    ''' <remarks>
    ''' 逐元素相对误差在分量接近零时会被噪声放大（解析值为 1e-7、绝对误差 1e-10 也算 0.1%），
    ''' 因此以整个采样向量的范数为分母，是数值梯度校验的常规做法。
    ''' </remarks>
    Private Function RelativeVectorError(sumNumeric As Double, sumAnalytic As Double, sumDiff As Double) As Double
        Dim numericNorm As Double = System.Math.Sqrt(sumNumeric)
        Dim analyticNorm As Double = System.Math.Sqrt(sumAnalytic)
        Dim scale As Double = System.Math.Max(numericNorm, analyticNorm)

        If scale <= 1.0E-12 Then Return 0.0

        Return System.Math.Sqrt(sumDiff) / scale
    End Function

    ''' <summary>构造确定性的随机张量（[-scale, scale] 均匀分布）。</summary>
    Private Function RandomTensor(shape As Integer(), scale As Double) As Tensor
        Dim t As Tensor = New Tensor(shape)
        Dim r As New Random(7)
        Dim data As Double() = t.Data

        For i As Integer = 0 To data.Length - 1
            data(i) = (r.NextDouble() * 2.0 - 1.0) * scale
        Next

        Call t.MarkHostModified()

        Return t
    End Function

    ''' <summary>构造对称的 SPD 桶矩阵（对角为自身桶）。</summary>
    Private Function CreateSpdBuckets(n As Integer, bucketCount As Integer) As Integer(,)
        Dim m As Integer(,) = New Integer(n - 1, n - 1) {}

        For i As Integer = 0 To n - 1
            For j As Integer = 0 To n - 1
                If i = j Then
                    m(i, j) = GraphEncoding.SelfBucket
                Else
                    Dim a As Integer = System.Math.Min(i, j)
                    Dim b As Integer = System.Math.Max(i, j)
                    m(i, j) = 1 + ((a * 31 + b * 17) Mod (bucketCount - 1))
                End If
            Next
        Next

        Return m
    End Function

    ' ==================== 合成数据 ====================

    ''' <summary>
    ''' 构造 20 基因 × 8 时间点（每点 2 个重复）的合成时序表达数据。
    ''' </summary>
    Private Function CreateExpression() As GeneExpressionData
        Dim genes As String() = New String() {
            "TF1", "TF2",
            "G1", "G2", "G3", "G4", "G5", "G6",
            "G7", "G8", "G9", "G10", "G11", "G12",
            "G13", "G14", "G15", "G16", "G17", "G18"
        }

        Dim nGene As Integer = genes.Length
        Dim nTime As Integer = 8
        Dim replicate As Integer = 2
        Dim nSample As Integer = nTime * replicate

        Dim matrix As Double(,) = New Double(nGene - 1, nSample - 1) {}
        Dim times As Double() = New Double(nSample - 1) {}
        Dim samples As String() = New String(nSample - 1) {}

        For t As Integer = 0 To nTime - 1
            For r As Integer = 0 To replicate - 1
                Dim j As Integer = t * replicate + r

                times(j) = t
                samples(j) = $"t{t}_r{r}"

                For i As Integer = 0 To nGene - 1
                    ' 非负的周期信号 + 基因特异的基线，便于 CPM 与 log 变换
                    Dim baseline As Double = 100.0 + i * 7.0
                    Dim amplitude As Double = 40.0 + (i Mod 5) * 10.0
                    matrix(i, j) = baseline + amplitude * System.Math.Sin((t + i * 0.3) * 0.8) + r * 0.5
                Next
            Next
        Next

        Return New GeneExpressionData With {
            .GeneNames = genes,
            .SampleNames = samples,
            .Matrix = matrix,
            .TimePoints = times
        }
    End Function

    ''' <summary>
    ''' 构造覆盖 6 个基因的小型先验网络（含激活与抑制边）。
    ''' </summary>
    Private Function CreateSmallPrior() As PriorNetwork
        Dim prior As New PriorNetwork()

        Call prior.AddEdge("TF1", "G1", Effector.Activator, 1.0, "")
        Call prior.AddEdge("G1", "G2", Effector.Activator, 1.0, "")
        Call prior.AddEdge("G1", "G3", Effector.Inhibitor, 1.0, "")
        Call prior.AddEdge("G2", "G4", Effector.Activator, 1.0, "")

        Return prior
    End Function

    ''' <summary>
    ''' 构造含 2-hop 与 3-hop 链路的先验调控网络。
    ''' </summary>
    Private Function CreatePrior() As PriorNetwork
        Dim prior As New PriorNetwork()

        For g As Integer = 1 To 6
            Call prior.AddEdge("TF1", $"G{g}", Effector.Activator, 0.9, "motif")
        Next

        For g As Integer = 7 To 12
            Call prior.AddEdge("TF2", $"G{g}", Effector.Activator, 0.8, "motif")
        Next

        Call prior.AddEdge("G1", "G13", Effector.Activator, 0.7, "motif")
        Call prior.AddEdge("G13", "G15", Effector.Activator, 0.6, "motif")
        Call prior.AddEdge("G15", "G17", Effector.Activator, 0.6, "motif")

        Return prior
    End Function

    ' ==================== 断言工具 ====================

    Private Sub Check(condition As Boolean, name As String)
        If condition Then
            passed += 1
        Else
            failed += 1
            Console.WriteLine($"  FAIL: {name}")
        End If
    End Sub

    Private Sub CheckNear(actual As Double, expected As Double, tolerance As Double, name As String)
        Call Check(System.Math.Abs(actual - expected) <= tolerance,
                   $"{name}（actual={actual:G6}, expected={expected:G6}）")
    End Sub

    ''' <summary>校验子集数组在原数组中的出现顺序是否保持升序</summary>
    Private Function IsAscending(subset As String(), full As String()) As Boolean
        Dim last As Integer = -1

        For Each name As String In subset
            Dim idx As Integer = Array.IndexOf(full, name)

            If idx <= last Then Return False

            last = idx
        Next

        Return True
    End Function

End Module
