' ============================================================================
' Program.vb - RasterGene 的端到端测试
'
'   1. 用 GDI+ 现画一批 32x32 的彩色合成图当作训练 / 测试数据
'   2. 训练 RGB 生成模型（卷积变分自编码器）
'   3. 输出重构与随机生成的预览图，肉眼可查
'   4. 导出 ONNX → 重新加载 → 逐项比对（结构 / 权重 / 重构 / 生成 / 图求值）
'   5. 把断言结果写进 DebugAudit 报告，退出码表示是否全部通过
'
' 命令行参数（全部可选）：
'   --epochs 80        训练轮数
'   --batch 32         批大小
'   --samples 600      训练样本数
'   --size 32          图像边长（必须是 4 的倍数）
'   --latent 16        潜变量维度
'   --seed 2026        随机种子
'   --out <dir>        输出目录（默认为可执行文件目录下的 rastergene-output）
'   --skip-train       跳过训练，仅用随机初始化的权重跑通流程
' ============================================================================

Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports Microsoft.VisualBasic.MachineVision.RasterGenerative
Imports Microsoft.VisualBasic.MachineVision.RasterGenerative.Raster
Imports std = System.Math

Module Program

    Sub New()
        Call ImageDriver.Register()
    End Sub

    Sub Main(args As String())
        Dim settings As Settings = Nothing

        Try
            settings = Settings.Parse(args)
        Catch ex As ArgumentException
            Console.WriteLine(ex.Message)
            Call Settings.PrintUsage()

            Environment.ExitCode = 2
            Return
        End Try

        If settings.Help Then
            Call Settings.PrintUsage()

            Environment.ExitCode = 0
            Return
        End If

        Call DebugAudit.Reset()

        Dim outputDir As String = PreviewBoard.EnsureDirectory(settings.OutputDirectory)

        Try
            Call RunExperiment(settings, outputDir)
        Catch ex As Exception
            Call DebugAudit.Test(False, $"测试执行过程中抛出异常: {ex.GetType().Name}: {ex.Message}")
            Console.WriteLine(ex.ToString())
        End Try

        Dim reportText As String = Path.Combine(outputDir, "rastergene-test-report.txt")
        Dim reportJson As String = Path.Combine(outputDir, "rastergene-test-report.json")

        Call File.WriteAllText(reportText, DebugAudit.Dump())
        Call DebugAudit.SaveReport(reportJson)

        Console.WriteLine()
        Console.WriteLine(DebugAudit.Dump())
        Console.WriteLine($"失败断言数: {DebugAudit.FailureCount}")
        Console.WriteLine($"文本报告: {reportText}")
        Console.WriteLine($"JSON 报告: {reportJson}")

        Environment.ExitCode = If(DebugAudit.FailureCount = 0, 0, 1)
    End Sub

    Private Sub RunExperiment(settings As Settings, outputDir As String)
        Console.WriteLine("== RasterGene: RGB 生成模型测试 ==")
        Console.WriteLine($"  图像尺寸 {settings.Size}x{settings.Size}x3, 潜变量 {settings.LatentDim}, 训练 {settings.Samples} 张, {settings.Epochs} 轮")
        Console.WriteLine($"  输出目录 {outputDir}")

        ' ---------------- 1. 合成数据集 ----------------
        Console.WriteLine("== 1. 生成合成图像 ==")

        Dim trainBitmaps As Bitmap() = SyntheticImages.CreateDataset(settings.Samples, settings.Size, settings.Seed)
        Dim testCount As Integer = std.Max(60, settings.Samples \ 5)
        Dim testBitmaps As Bitmap() = SyntheticImages.CreateDataset(testCount, settings.Size, settings.Seed + 7919)

        Call PreviewBoard.SaveMontage(Path.Combine(outputDir, "dataset-samples.png"), Take(trainBitmaps, 32), 8, 4)
        Call PreviewBoard.SaveMontage(Path.Combine(outputDir, "test-originals.png"), Take(testBitmaps, 32), 8, 4)

        Dim trainImages As Tensor = RgbImageTensor.FromBitmaps(trainBitmaps)
        Dim testImages As Tensor = RgbImageTensor.FromBitmaps(testBitmaps)

        Call DebugAudit.AreEqual(trainBitmaps.Length, trainImages.Shape(0), "训练张量的批大小必须与图像数一致")
        Call DebugAudit.AreEqual(settings.Size, trainImages.Shape(1), "训练张量的高度必须等于图像边长")
        Call DebugAudit.AreEqual(settings.Size, trainImages.Shape(2), "训练张量的宽度必须等于图像边长")
        Call DebugAudit.AreEqual(RgbImageTensor.ColorChannels, trainImages.Shape(3), "训练张量必须是 3 通道 RGB")
        Call DebugAudit.Test(IsWithinUnitInterval(trainImages), "位图转换后的像素必须落在 [0, 1]")
        Call DebugAudit.Test(IsWithinUnitInterval(testImages), "测试集像素必须落在 [0, 1]")

        ' 位图 → 张量 → 位图 → 张量的往返误差（量化到 8 位，允许 1/255 的偏差）
        Dim roundTrip As Tensor = RgbImageTensor.FromBitmap(RgbImageTensor.ToBitmap(trainImages, 0))
        Dim roundTripDiff As Double = MaxAbsDiff(SliceRows(trainImages, 1), roundTrip)

        Call DebugAudit.Capture("raster.roundtrip.maxdiff", roundTripDiff)
        Call DebugAudit.Test(roundTripDiff <= 1.0 / 255.0 + 0.000001,
                             $"位图往返误差过大: {roundTripDiff:E3}")

        Console.WriteLine($"  训练集 {trainImages.Shape(0)} 张, 测试集 {testImages.Shape(0)} 张")

        If settings.SkipTraining Then
            Console.WriteLine("  已跳过训练与后续校验（--skip-train）")
            Return
        End If

        ' ---------------- 2. 训练 ----------------
        Console.WriteLine("== 2. 训练变分自编码器 ==")

        Dim model As New RgbGenerativeVae(settings.Size, settings.Size,
                                          latentDim:=settings.LatentDim,
                                          filter1:=RgbGenerativeVae.DefaultFilter1,
                                          filter2:=RgbGenerativeVae.DefaultFilter2,
                                          seed:=settings.Seed)

        Dim before As Tuple(Of Double, Double, Double) = model.Evaluate(testImages)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim options As TrainingOptions = RgbGenerativeVae.CreateDefaultOptions()

        options.Epochs = settings.Epochs
        options.BatchSize = settings.BatchSize
        options.LearningRate = settings.LearningRate
        options.Beta = settings.Beta
        options.KlWarmupEpochs = std.Max(1, settings.Epochs \ 5)
        options.Verbose = True

        Call model.Fit(trainImages, options)

        watch.Stop()

        Dim after As Tuple(Of Double, Double, Double) = model.Evaluate(testImages)

        Call DebugAudit.Capture("train.elapsed.ms", watch.ElapsedMilliseconds)
        Call DebugAudit.Capture("train.bce.before", before.Item1)
        Call DebugAudit.Capture("train.bce.after", after.Item1)
        Call DebugAudit.Capture("train.mse.after", after.Item2)
        Call DebugAudit.Capture("train.kl.after", after.Item3)

        Console.WriteLine($"  耗时 {watch.ElapsedMilliseconds} ms, 测试集 BCE {before.Item1:F5} -> {after.Item1:F5}, MSE {after.Item2:F5}")

        Call DebugAudit.AreEqual(settings.Epochs, model.Loss.Count, "训练损失记录数必须等于训练轮数")
        Call DebugAudit.Test(AllFinite(model.Loss), "训练损失必须全部为有限值")
        Call DebugAudit.Test(model.Loss.Last() < model.Loss.First(),
                             $"训练损失应下降: {model.Loss.First():F5} -> {model.Loss.Last():F5}")
        Call DebugAudit.Test(after.Item1 < before.Item1,
                             $"测试集重构误差应下降: {before.Item1:F5} -> {after.Item1:F5}")
        Call DebugAudit.Test(after.Item2 < before.Item2,
                             $"测试集 MSE 应下降: {before.Item2:F5} -> {after.Item2:F5}")

        ' ---------------- 3. 重构与生成 ----------------
        Console.WriteLine("== 3. 重构与随机生成 ==")

        Dim previewCount As Integer = std.Min(32, testImages.Shape(0))
        Dim preview As Tensor = SliceRows(testImages, previewCount)
        Dim reconstructed As Tensor = model.Reconstruct(preview)
        Dim generated As Tensor = model.Generate(64, settings.Seed + 101)

        Call DebugAudit.Test(reconstructed.Shape(1) = settings.Size AndAlso reconstructed.Shape(3) = RgbImageTensor.ColorChannels,
                             "重构结果的形状必须与输入一致")
        Call DebugAudit.Test(IsWithinUnitInterval(reconstructed), "伯努利似然下重构值必须落在 [0, 1]")
        Call DebugAudit.Test(IsWithinUnitInterval(generated), "伯努利似然下生成值必须落在 [0, 1]")
        Call DebugAudit.AreEqual(64, generated.Shape(0), "生成样本数必须与请求一致")

        Dim previewBitmaps As Bitmap() = RgbImageTensor.ToBitmaps(preview)
        Dim reconstructedBitmaps As Bitmap() = RgbImageTensor.ToBitmaps(reconstructed)

        Call PreviewBoard.SaveMontage(Path.Combine(outputDir, "test-reconstructed.png"), reconstructedBitmaps, 8, 4)
        Call PreviewBoard.SaveComparison(Path.Combine(outputDir, "reconstruct-compare.png"),
                                         previewBitmaps, reconstructedBitmaps, 8, 3)
        Call PreviewBoard.SaveMontage(Path.Combine(outputDir, "generated-samples.png"),
                                      RgbImageTensor.ToBitmaps(generated), 8, 4)

        ' ---------------- 4. ONNX 持久化 ----------------
        Dim onnxPath As String = Path.Combine(outputDir, "rastergene-vae.onnx")

        Call OnnxRoundTripAudit.Run(model, preview, onnxPath, outputDir)

        ' ---------------- 5. 汇总 ----------------
        Console.WriteLine("== 5. 产物 ==")
        Console.WriteLine($"  {model}")
        Console.WriteLine($"  ONNX: {onnxPath} ({New FileInfo(onnxPath).Length} 字节)")
    End Sub

#Region "数值辅助"

    Private Function SliceRows(source As Tensor, count As Integer) As Tensor
        Dim shape As Integer() = source.Shape
        Dim available As Integer = shape(0)
        Dim take As Integer = std.Max(1, std.Min(count, available))
        Dim rowSize As Integer = source.Data.Length \ available
        Dim result As New Tensor(take, shape(1), shape(2), shape(3))

        Call Array.Copy(source.Data, 0, result.Data, 0, take * rowSize)

        Return result
    End Function

    Private Function Take(Of T)(items As T(), count As Integer) As T()
        Dim size As Integer = std.Max(1, std.Min(count, items.Length))
        Dim result As T() = New T(size - 1) {}

        Call Array.Copy(items, 0, result, 0, size)

        Return result
    End Function

    Private Function IsWithinUnitInterval(values As Tensor) As Boolean
        Dim buffer As Double() = values.Data

        For i As Integer = 0 To buffer.Length - 1
            If Double.IsNaN(buffer(i)) OrElse buffer(i) < 0 OrElse buffer(i) > 1 Then
                Return False
            End If
        Next

        Return True
    End Function

    Private Function AllFinite(values As IEnumerable(Of Double)) As Boolean
        For Each value As Double In values
            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                Return False
            End If
        Next

        Return True
    End Function

    Private Function MaxAbsDiff(left As Tensor, right As Tensor) As Double
        If left Is Nothing OrElse right Is Nothing Then
            Return Double.PositiveInfinity
        End If

        If left.Data.Length <> right.Data.Length Then
            Return Double.PositiveInfinity
        End If

        Dim difference As Double = 0

        For i As Integer = 0 To left.Data.Length - 1
            difference = std.Max(difference, std.Abs(left.Data(i) - right.Data(i)))
        Next

        Return difference
    End Function

#End Region

    ''' <summary>命令行设置。</summary>
    Private Class Settings

        Public Property Epochs As Integer = 80
        Public Property BatchSize As Integer = 32
        Public Property Samples As Integer = 600
        Public Property Size As Integer = 32
        Public Property LatentDim As Integer = 16
        Public Property Seed As Integer = 2026
        Public Property LearningRate As Double = 0.002
        Public Property Beta As Double = 0.2
        Public Property SkipTraining As Boolean = False
        Public Property Help As Boolean = False
        Public Property OutputDirectory As String = Path.Combine(AppContext.BaseDirectory, "rastergene-output")

        Public Shared Function Parse(args As String()) As Settings
            Dim settings As New Settings()

            If args Is Nothing Then
                Return settings
            End If

            Dim index As Integer = 0

            While index < args.Length
                Dim name As String = If(args(index), "").ToLowerInvariant()

                Select Case name
                    Case "--epochs"
                        settings.Epochs = NextInt(args, index, "--epochs")
                        index += 2
                    Case "--batch"
                        settings.BatchSize = NextInt(args, index, "--batch")
                        index += 2
                    Case "--samples"
                        settings.Samples = NextInt(args, index, "--samples")
                        index += 2
                    Case "--size"
                        settings.Size = NextInt(args, index, "--size")
                        index += 2
                    Case "--latent"
                        settings.LatentDim = NextInt(args, index, "--latent")
                        index += 2
                    Case "--seed"
                        settings.Seed = NextInt(args, index, "--seed")
                        index += 2
                    Case "--lr"
                        settings.LearningRate = NextDouble(args, index, "--lr")
                        index += 2
                    Case "--beta"
                        settings.Beta = NextDouble(args, index, "--beta")
                        index += 2
                    Case "--out"
                        If index + 1 >= args.Length Then
                            Throw New ArgumentException("参数 --out 缺少目录")
                        End If

                        settings.OutputDirectory = args(index + 1)
                        index += 2
                    Case "--skip-train"
                        settings.SkipTraining = True
                        index += 1
                    Case "--help", "-h", "/?"
                        settings.Help = True
                        index += 1
                    Case Else
                        Throw New ArgumentException($"无法识别的参数: {args(index)}")
                End Select
            End While

            If settings.Epochs <= 0 OrElse settings.Samples <= 0 OrElse settings.BatchSize <= 0 Then
                Throw New ArgumentException("--epochs / --samples / --batch 必须为正数")
            End If

            If settings.Size < 8 OrElse settings.Size Mod 4 <> 0 Then
                Throw New ArgumentException($"--size 必须是 4 的倍数且不小于 8，实际 {settings.Size}")
            End If

            If settings.LatentDim <= 0 Then
                Throw New ArgumentException($"--latent 必须为正数，实际 {settings.LatentDim}")
            End If

            Return settings
        End Function

        Private Shared Function NextDouble(args As String(), index As Integer, name As String) As Double
            If index + 1 >= args.Length Then
                Throw New ArgumentException($"参数 {name} 缺少取值")
            End If

            Dim value As Double

            If Not Double.TryParse(args(index + 1), Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, value) Then
                Throw New ArgumentException($"参数 {name} 的取值无法解析为浮点数: {args(index + 1)}")
            End If

            Return value
        End Function

        Private Shared Function NextInt(args As String(), index As Integer, name As String) As Integer
            If index + 1 >= args.Length Then
                Throw New ArgumentException($"参数 {name} 缺少取值")
            End If

            Dim value As Integer

            If Not Integer.TryParse(args(index + 1), value) Then
                Throw New ArgumentException($"参数 {name} 的取值无法解析为整数: {args(index + 1)}")
            End If

            Return value
        End Function

        Public Shared Sub PrintUsage()
            Console.WriteLine("用法: test [--epochs N] [--batch N] [--samples N] [--size N] [--latent N] [--seed N] [--out DIR] [--skip-train]")
            Console.WriteLine("  --size 必须是 4 的倍数（模型中包含两级 stride-2 卷积）")
        End Sub

    End Class

End Module
