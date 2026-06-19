Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.IMMO
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports std = System.Math

''' <summary>
''' IMMO (Integration Model for Incomplete Multi-Omics)
''' </summary>
<Package("IMMO")>
<RTypeExport("immo_pars", GetType(IMMOConfig))>
Module IMMOTool

    <ExportAPI("prepared_data")>
    <RApiReturn(GetType(PreparedData))>
    Public Function preparedDataTool(<RListObjectArgument> data As list, Optional env As Environment = Nothing) As Object
        Dim i As i32 = 1
        Dim matrices As New List(Of Double(,))
        Dim sampleIDLists As New List(Of String())
        Dim omicsNames As New List(Of String)

        ' ====================================================================
        ' 3. 数据预处理（处理样本不匹配）
        ' ====================================================================
        Console.WriteLine("[1] 数据预处理（对齐样本、生成掩码、归一化）...")
        Console.WriteLine()

        For Each name As String In data.getNames
            Dim val As Matrix = TryCast(data.getByName(name), Matrix)

            If Not val Is Nothing Then
                matrices.Add(val.AsTensorArray)
                sampleIDLists.Add(val.sampleID)
                omicsNames.Add(name)

                Console.WriteLine($"  组学{++i} ({name}): {val.sample_count} 个样本 × {val.size} 个特征")
            End If
        Next

        Dim unionSampleIDs As String() = sampleIDLists.IteratesALL.Distinct.OrderBy(Function(id) id).ToArray
        Dim missingIDs As New List(Of String())

        For idx As Integer = 0 To sampleIDLists.Count - 1
            Dim currentSamples = sampleIDLists(idx)
            Console.WriteLine($"  组学{idx + 1}样本: {sampleIDLists(i).JoinBy(", ")}")
            missingIDs.Add(unionSampleIDs.Where(Function(id) Array.IndexOf(currentSamples, id) < 0).ToArray)
        Next

        i = 1

        For Each row In missingIDs
            If row.Any Then
                Console.WriteLine($"  缺失样本: {row.JoinBy(", ")} 在组学{i}中缺失")
            End If

            i = i + 1
        Next

        Console.WriteLine()

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

        Return preparedData
    End Function

    <ExportAPI("train")>
    Public Function train_immo(preparedData As PreparedData, Optional config As IMMOConfig = Nothing) As IMMOModel
        config = If(config, New IMMOConfig With {
            .LatentDim = 32,                    ' 潜在空间维度（示例用较小值）
            .EncoderHiddenDims = {64, 32},      ' 编码器隐藏层
            .DecoderHiddenDims = {32, 64},      ' 解码器隐藏层
            .MaxEpochs = 100,                   ' 最大训练轮数
            .LearningRate = 0.005,              ' 初始学习率
            .DropoutRate = 0.3,                 ' Dropout率
            .InitialP = 0.75,                   ' 初始保留概率
            .DecayRate = 0.99,                  ' 保留概率增长率
            .Pmax = 0.95,                       ' 最大保留概率
            .LossWeights = {0.5, 0.5},          ' 两组学等权重
            .Patience = 15,                     ' 早停耐心值
            .GradientClipNorm = 5.0,            ' 梯度裁剪
            .Seed = 42,                         ' 随机种子（可复现）
            .Verbose = True,
            .LogInterval = 10
        })

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
        ' 4. 构建并训练IMMO模型
        ' ====================================================================
        Console.WriteLine("[2] 构建并训练IMMO模型...")
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

        Return model
    End Function

    <ExportAPI("latent_data")>
    Public Function immo_latent(model As IMMOModel, preparedData As PreparedData) As Tensor
        ' ====================================================================
        ' 5. 获取潜在空间表示
        ' ====================================================================
        Console.WriteLine("[3] 获取统一样本的潜在空间表示...")
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

        Return latentReps
    End Function

    <ExportAPI("make_omics_impute")>
    Public Function immo_impute(model As IMMOModel, preparedData As PreparedData)
        ' ====================================================================
        ' 6. 获取插补后的完整数据
        ' ====================================================================
        Console.WriteLine("[4] 获取插补后的完整数据（缺失样本已填充）...")
        Console.WriteLine()

        Dim imputedData = DataPrep.GetImputedData(model, preparedData)
        Dim imputes As list = list.empty

        For i = 0 To preparedData.NumOmics - 1
            Dim omics As IMMO.OmicsData = preparedData.OmicsList(i)
            Dim imputed As Tensor = imputedData(i)
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

        Return imputes
    End Function

    <ExportAPI("reconstruct")>
    Public Function evaluation(model As IMMOModel, preparedData As PreparedData) As Tensor()
        ' ====================================================================
        ' 7. 评估插补质量（使用已知观测值的重构误差）
        ' ====================================================================
        Console.WriteLine("[7] 评估模型重构质量（基于观测值）...")
        Console.WriteLine()

        Dim reconstructions = model.GetReconstructions(preparedData.OmicsList).ToArray
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

        Return reconstructions
    End Function
End Module
