' ============================================================================
' IMMO Framework - Example Program
' ============================================================================
' 演示如何使用IMMO框架处理两个组学数据矩阵之间生物学重复样本数量不匹配的问题
'
' 场景:
'   - 组学1 (如转录组): 8个样本, 50个特征
'   - 组学2 (如蛋白质组): 6个样本, 30个特征
'   - 两个组学的样本部分重叠，部分不重叠
'   - 目标: 整合两个组学，获得统一样本的潜在表示和插补后的完整数据
' ============================================================================

Imports std = System.Math

Module Program2

    Sub Main2()
        Console.WriteLine("="c, 80)
        Console.WriteLine("IMMO Framework - 多组学数据整合 (样本不匹配处理)")
        Console.WriteLine("="c, 80)
        Console.WriteLine()

        ' ====================================================================
        ' 1. 创建模拟数据（两个组学，样本部分不匹配）
        ' ====================================================================
        Console.WriteLine("[1] 生成模拟多组学数据...")
        Console.WriteLine()

        ' 组学1: 转录组 - 8个样本, 50个特征
        Dim omics1Samples As String() = {"S01", "S02", "S03", "S04", "S05", "S06", "S07", "S08"}
        Dim omics1Data(,) As Double = GenerateSyntheticData(8, 50, seed:=42)

        ' 组学2: 蛋白质组 - 6个样本, 30个特征
        ' 注意: S01和S08在组学2中缺失，造成样本不匹配
        Dim omics2Samples As String() = {"S02", "S03", "S04", "S05", "S06", "S07"}
        Dim omics2Data(,) As Double = GenerateSyntheticData(6, 30, seed:=123)

        Console.WriteLine($"  组学1 (转录组): {omics1Samples.Length} 个样本 × {omics1Data.GetLength(1)} 个特征")
        Console.WriteLine($"  组学2 (蛋白质组): {omics2Samples.Length} 个样本 × {omics2Data.GetLength(1)} 个特征")
        Console.WriteLine($"  组学1样本: {String.Join(", ", omics1Samples)}")
        Console.WriteLine($"  组学2样本: {String.Join(", ", omics2Samples)}")
        Console.WriteLine($"  缺失样本: S01, S08 在组学2中缺失")
        Console.WriteLine()

        ' ====================================================================
        ' 2. 配置IMMO模型参数
        ' ====================================================================
        Console.WriteLine("[2] 配置IMMO模型参数...")
        Console.WriteLine()

        Dim config As New IMMOConfig()
        config.LatentDim = 32                    ' 潜在空间维度（示例用较小值）
        config.EncoderHiddenDims = {64, 32}      ' 编码器隐藏层
        config.DecoderHiddenDims = {32, 64}      ' 解码器隐藏层
        config.MaxEpochs = 100                   ' 最大训练轮数
        config.LearningRate = 0.005              ' 初始学习率
        config.DropoutRate = 0.3                 ' Dropout率
        config.InitialP = 0.75                   ' 初始保留概率
        config.DecayRate = 0.99                  ' 保留概率增长率
        config.Pmax = 0.95                       ' 最大保留概率
        config.LossWeights = {0.5, 0.5}          ' 两组学等权重
        config.Patience = 15                     ' 早停耐心值
        config.GradientClipNorm = 5.0            ' 梯度裁剪
        config.Seed = 42                          ' 随机种子（可复现）
        config.Verbose = True
        config.LogInterval = 10

        Console.WriteLine($"  潜在空间维度: {config.LatentDim}")
        Console.WriteLine($"  编码器隐藏层: [{String.Join(", ", config.EncoderHiddenDims)}]")
        Console.WriteLine($"  解码器隐藏层: [{String.Join(", ", config.DecoderHiddenDims)}]")
        Console.WriteLine($"  最大训练轮数: {config.MaxEpochs}")
        Console.WriteLine($"  初始学习率: {config.LearningRate}")
        Console.WriteLine($"  Dropout率: {config.DropoutRate}")
        Console.WriteLine($"  初始保留概率P₀: {config.InitialP}")
        Console.WriteLine($"  保留概率增长率: {config.DecayRate}")
        Console.WriteLine($"  损失权重: [{String.Join(", ", config.LossWeights)}]")
        Console.WriteLine()

        ' ====================================================================
        ' 3. 数据预处理（处理样本不匹配）
        ' ====================================================================
        Console.WriteLine("[3] 数据预处理（对齐样本、生成掩码、归一化）...")
        Console.WriteLine()

        Dim matrices As New List(Of Double(,)) From {omics1Data, omics2Data}
        Dim sampleIDLists As New List(Of String()) From {omics1Samples, omics2Samples}
        Dim omicsNames As String() = {"Transcriptome", "Proteome"}

        Dim preparedData As PreparedData = DataPrep.PrepareData(
            matrices, sampleIDLists, omicsNames, normalize:=True)

        Console.WriteLine($"  统一样本数: {preparedData.UnifiedSampleIDs.Length}")
        Console.WriteLine($"  统一样本ID: {String.Join(", ", preparedData.UnifiedSampleIDs)}")
        Console.WriteLine()

        For Each omics In preparedData.OmicsList
            Dim observedCount = 0
            For j = 0 To omics.Mask.Length - 1
                If omics.Mask(j) > 0 Then observedCount += 1
            Next
            Dim totalEntries = omics.NumSamples * omics.NumFeatures
            Console.WriteLine($"  {omics.Name}:")
            Console.WriteLine($"    矩阵维度: {omics.NumSamples} × {omics.NumFeatures}")
            Console.WriteLine($"    观测值: {observedCount}/{totalEntries} ({observedCount * 100.0 / totalEntries:F1}%)")
            Console.WriteLine($"    缺失值: {totalEntries - observedCount}/{totalEntries} ({(totalEntries - observedCount) * 100.0 / totalEntries:F1}%)")
        Next
        Console.WriteLine()

        ' ====================================================================
        ' 4. 构建并训练IMMO模型
        ' ====================================================================
        Console.WriteLine("[4] 构建并训练IMMO模型...")
        Console.WriteLine()

        Dim featureDims As Integer() = preparedData.OmicsList.Select(Function(o) o.NumFeatures).ToArray()
        Dim model As New IMMOModel(featureDims, config)
        Dim trainer As New IMMOTrainer(model, config)

        Console.WriteLine($"  模型结构:")
        Console.WriteLine($"    编码器数量: {model.NumOmics}")
        Console.WriteLine($"    各组学特征维度: [{String.Join(", ", featureDims)}]")
        Console.WriteLine($"    解码器输出维度: {featureDims.Sum()}")
        Console.WriteLine()

        Dim finalLoss = trainer.Train(preparedData)
        Console.WriteLine()
        Console.WriteLine($"  训练完成! 最终损失: {finalLoss:F6}")
        Console.WriteLine()

        ' ====================================================================
        ' 5. 获取潜在空间表示
        ' ====================================================================
        Console.WriteLine("[5] 获取统一样本的潜在空间表示...")
        Console.WriteLine()

        Dim latentReps = model.GetLatentRepresentations(preparedData.OmicsList)
        Console.WriteLine($"  潜在表示维度: {latentReps.Shape(0)} 样本 × {latentReps.Shape(1)} 维")
        Console.WriteLine()
        Console.WriteLine("  潜在表示 (前5个样本, 前5个维度):")
        Console.WriteLine($"  {"样本ID",-8} | {"维度1",10} {"维度2",10} {"维度3",10} {"维度4",10} {"维度5",10}")
        Console.WriteLine($"  {"-",-8}-+-{"-",10}-{"-",10}-{"-",10}-{"-",10}-{"-",10}")

        For i = 0 To std.Min(4, latentReps.Shape(0) - 1)
            Console.Write($"  {preparedData.UnifiedSampleIDs(i),-8} |")
            For j = 0 To std.Min(4, latentReps.Shape(1) - 1)
                Console.Write($" {latentReps(i, j),10:F4}")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()

        ' ====================================================================
        ' 6. 获取插补后的完整数据
        ' ====================================================================
        Console.WriteLine("[6] 获取插补后的完整数据（缺失样本已填充）...")
        Console.WriteLine()

        Dim imputedData = DataPrep.GetImputedData(model, preparedData)

        For i = 0 To preparedData.NumOmics - 1
            Dim omics = preparedData.OmicsList(i)
            Dim imputed = imputedData(i)
            Console.WriteLine($"  {omics.Name} (插补后):")
            Console.WriteLine($"    矩阵维度: {imputed.Shape(0)} × {imputed.Shape(1)}")

            ' 显示缺失样本的插补值（前3个特征）
            For j = 0 To imputed.Shape(0) - 1
                Dim sampleID = preparedData.UnifiedSampleIDs(j)
                Dim wasMissing = False
                For k = 0 To omics.NumFeatures - 1
                    If omics.Mask(j, k) = 0 Then
                        wasMissing = True
                        Exit For
                    End If
                Next

                If wasMissing Then
                    Console.Write($"      [{sampleID}] 插补值(前3特征): ")
                    For k = 0 To std.Min(2, imputed.Shape(1) - 1)
                        Console.Write($"{imputed(j, k):F3} ")
                    Next
                    Console.WriteLine("(原为缺失)")
                End If
            Next
            Console.WriteLine()
        Next

        ' ====================================================================
        ' 7. 评估插补质量（使用已知观测值的重构误差）
        ' ====================================================================
        Console.WriteLine("[7] 评估模型重构质量（基于观测值）...")
        Console.WriteLine()

        Dim reconstructions = model.GetReconstructions(preparedData.OmicsList)
        For i = 0 To preparedData.NumOmics - 1
            Dim omics = preparedData.OmicsList(i)
            Dim recon = reconstructions(i)

            Dim totalSqError = 0.0
            Dim observedCount = 0
            For j = 0 To omics.Data.Length - 1
                If omics.Mask(j) > 0 Then
                    Dim diff = omics.Data(j) - recon(j)
                    totalSqError += diff * diff
                    observedCount += 1
                End If
            Next

            Dim rmse = std.Sqrt(totalSqError / std.Max(1, observedCount))
            Console.WriteLine($"  {omics.Name}:")
            Console.WriteLine($"    观测值数量: {observedCount}")
            Console.WriteLine($"    重构RMSE (归一化尺度): {rmse:F6}")
        Next
        Console.WriteLine()

        ' ====================================================================
        ' 8. 输出结果文件路径
        ' ====================================================================
        Console.WriteLine("[8] 结果输出...")
        Console.WriteLine()

        ' 保存潜在表示到CSV
        SaveLatentToCSV("latent_representations.csv", preparedData.UnifiedSampleIDs, latentReps)
        Console.WriteLine("  潜在表示已保存: latent_representations.csv")

        ' 保存插补数据到CSV
        For i = 0 To preparedData.NumOmics - 1
            Dim omics = preparedData.OmicsList(i)
            Dim imputed = imputedData(i)
            Dim filename = $"imputed_{omics.Name}.csv"
            SaveImputedToCSV(filename, preparedData.UnifiedSampleIDs, imputed, omics)
            Console.WriteLine($"  插补数据已保存: {filename}")
        Next

        Console.WriteLine()
        Console.WriteLine("="c, 80)
        Console.WriteLine("IMMO框架处理完成!")
        Console.WriteLine("="c, 80)
    End Sub

    ' ========================================================================
    ' 辅助函数
    ' ========================================================================

    ''' <summary>
    ''' 生成模拟多组学数据
    ''' </summary>
    Private Function GenerateSyntheticData(numSamples As Integer, numFeatures As Integer, seed As Integer) As Double(,)
        Dim rng As New Random(seed)
        Dim data(numSamples - 1, numFeatures - 1) As Double

        ' 生成具有潜在结构的数据（3个潜在因子驱动）
        Dim numLatentFactors = 3
        Dim latentFactors(numSamples - 1, numLatentFactors - 1) As Double
        Dim featureLoadings(numLatentFactors - 1, numFeatures - 1) As Double

        For i = 0 To numSamples - 1
            For k = 0 To numLatentFactors - 1
                latentFactors(i, k) = rng.NextDouble() * 4 - 2
            Next
        Next

        For k = 0 To numLatentFactors - 1
            For j = 0 To numFeatures - 1
                featureLoadings(k, j) = rng.NextDouble() * 2 - 1
            Next
        Next

        ' 数据 = 潜在因子 × 载荷 + 噪声
        For i = 0 To numSamples - 1
            For j = 0 To numFeatures - 1
                Dim signal = 0.0
                For k = 0 To numLatentFactors - 1
                    signal += latentFactors(i, k) * featureLoadings(k, j)
                Next
                Dim noise = (rng.NextDouble() - 0.5) * 0.5
                data(i, j) = signal + noise
            Next
        Next

        Return data
    End Function

    ''' <summary>
    ''' 保存潜在表示到CSV文件
    ''' </summary>
    Private Sub SaveLatentToCSV(filename As String, sampleIDs As String(), latent As Tensor)
        Dim path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, filename)
        Using writer As New System.IO.StreamWriter(path)
            ' 写表头
            writer.Write("SampleID")
            For j = 0 To latent.Shape(1) - 1
                writer.Write($",LatentDim{j + 1}")
            Next
            writer.WriteLine()

            ' 写数据
            For i = 0 To latent.Shape(0) - 1
                writer.Write(sampleIDs(i))
                For j = 0 To latent.Shape(1) - 1
                    writer.Write($",{latent(i, j):F6}")
                Next
                writer.WriteLine()
            Next
        End Using
    End Sub

    ''' <summary>
    ''' 保存插补数据到CSV文件
    ''' </summary>
    Private Sub SaveImputedToCSV(filename As String, sampleIDs As String(), imputed As Tensor, omics As OmicsData)
        Dim path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, filename)
        Using writer As New System.IO.StreamWriter(path)
            ' 写表头
            writer.Write("SampleID")
            For j = 0 To imputed.Shape(1) - 1
                writer.Write($",{omics.FeatureNames(j)}")
            Next
            writer.WriteLine()

            ' 写数据
            For i = 0 To imputed.Shape(0) - 1
                writer.Write(sampleIDs(i))
                For j = 0 To imputed.Shape(1) - 1
                    writer.Write($",{imputed(i, j):F6}")
                Next
                writer.WriteLine()
            Next
        End Using
    End Sub

End Module
