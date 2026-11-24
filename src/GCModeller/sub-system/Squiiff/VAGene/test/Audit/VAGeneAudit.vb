' ============================================================================
' VAGeneAudit.vb - 条件 VAE 虚拟扰动实验的审计用例
'
' 覆盖：数据容器、计数似然、模型训练与表达空间编解码、反事实方向性、
'       上下文对齐、扰动登记错误路径、模型存档往返。
' 全部断言使用 Microsoft.VisualBasic.Core 的 DebugAudit。
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.VAGene
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Intervention
Imports std = System.Math

''' <summary>VAGene 审计用例。</summary>
Public Module VAGeneAudit

    Private Const GeneCount As Integer = 4
    Private Const ConditionCount As Integer = 2
    Private ReadOnly TestGeneNames As String() = {"gene0", "gene1", "gene2", "gene3"}
    Private ReadOnly TestConditionNames As String() = {"control", "drug"}

#Region "数据"

    ''' <summary>数据集归一化、one-hot 与按条件切片。</summary>
    Public Sub DatasetMustNormalizeAndSlice()
        Dim dataset As PerturbationDataset = BuildDataset()

        Call DebugAudit.Test(dataset.CellCount = 200, $"细胞数应为 200，实际 {dataset.CellCount}")
        Call DebugAudit.Test(dataset.GeneCount = GeneCount, $"基因数应为 {GeneCount}")
        Call DebugAudit.Test(dataset.ConditionCount = ConditionCount, $"条件数应为 {ConditionCount}")
        Call DebugAudit.Test(dataset.ControlCondition() = 0, "对照条件应识别为 control")

        Dim oneHot As Tensor = dataset.OneHot(dataset.Conditions)
        Call DebugAudit.Test(std.Abs(AuditSupport.SumOf(oneHot) - 200.0) < 1.0E-09, "one-hot 行和应为 1")

        Dim normalized As Tensor = dataset.NormalizeLibrarySize(10000)
        Dim rowSum As Double = 0

        For g As Integer = 0 To GeneCount - 1
            rowSum += normalized.Data(g)
        Next

        Call DebugAudit.Test(std.Abs(rowSum - 10000.0) < 1.0E-06, $"文库归一化后行和应为 10000，实际 {rowSum:F3}")

        Dim logNormalized As Tensor = dataset.LogNormalize()
        Call DebugAudit.Test(AuditSupport.AllFinite(logNormalized), "log1p 归一化结果必须为有限值")

        Dim control As PerturbationDataset = dataset.SelectCondition(0)
        Call DebugAudit.Test(control.CellCount = 100, $"对照组细胞数应为 100，实际 {control.CellCount}")
    End Sub

#End Region

#Region "似然"

    ''' <summary>泊松闭式值与负二项梯度有限差分校验。</summary>
    Public Sub LikelihoodMustMatchClosedForm()
        Dim poisson As New CountLikelihood(CountLikelihoodKind.Poisson)
        Dim expected As Double = 3.0 * std.Log(2.0) - 2.0 - std.Log(6.0)
        Dim actual As Double = poisson.LogLikelihood(3, 2)

        Call DebugAudit.Capture("vagene.poisson.loglik", actual)
        ' LogFactorial 走 VBMath.LogGamma 的近似实现，容差取 1E-06 以吸收该数值误差
        Call DebugAudit.Test(std.Abs(actual - expected) < 1.0E-06, $"泊松对数似然应为 {expected:F9}，实际 {actual:F9}")

        Dim nb As New CountLikelihood(CountLikelihoodKind.NegativeBinomial) With {.Dispersion = 1.5}
        Dim mean As Double = 4.0
        Dim observed As Double = 3.0
        Dim h As Double = 1.0E-06
        Dim up As Double = nb.LogLikelihood(observed, mean * std.Exp(h))
        Dim down As Double = nb.LogLikelihood(observed, mean * std.Exp(-h))
        Dim numeric As Double = -(up - down) / (2.0 * h)
        Dim analytic As Double = nb.GradientLogMean(observed, mean)

        Call DebugAudit.Capture("vagene.nb.gradient.analytic", analytic)
        Call DebugAudit.Capture("vagene.nb.gradient.numeric", numeric)
        Call DebugAudit.Test(std.Abs(analytic - numeric) < 1.0E-05,
                             $"NB 梯度应与有限差分一致: {analytic:F6} vs {numeric:F6}")
    End Sub

#End Region

#Region "模型"

    ''' <summary>训练收敛，且表达空间编解码接口形状与数值正确。</summary>
    Public Sub ModelMustFitAndExposeExpressionApi()
        Dim dataset As PerturbationDataset = BuildDataset()
        Dim model As ConditionalVAE = BuildModel()

        Call model.Fit(dataset, epochs:=80, batchSize:=32, learningRate:=0.01, beta:=0.5, verbose:=False)

        Call DebugAudit.Test(model.IsFitted, "训练完成后 IsFitted 应为 True")
        Call DebugAudit.Test(model.Loss.Count > 0, "损失历史不应为空")
        Call DebugAudit.Test(AuditSupport.AllFinite(model.Loss), "训练损失必须全部为有限值")
        Call DebugAudit.Test(AuditSupport.IsFinite(model.MeanLibrarySize), "平均文库大小必须为有限值")

        Dim expression As Tensor = BuildContextVector(dataset)
        Dim latent As Tensor = model.EncodeExpression(expression, 0)

        Call DebugAudit.Test(latent.Shape(0) = 1 AndAlso latent.Shape(1) = model.LatentDim,
                             $"隐变量形状应为 [1,{model.LatentDim}]")
        Call DebugAudit.Test(AuditSupport.AllFinite(latent), "隐变量必须为有限值")

        Dim decoded As Tensor = model.DecodeLatent(latent, 0)

        Call DebugAudit.Test(decoded.Shape(0) = 1 AndAlso decoded.Shape(1) = GeneCount,
                             $"解码形状应为 [1,{GeneCount}]")
        Call DebugAudit.Test(AuditSupport.AllFinite(decoded), "解码结果必须为有限值")
    End Sub

#End Region

#Region "反事实"

    ''' <summary>门面应能注入上下文、登记扰动并产生非零反事实效应。</summary>
    Public Sub FacadeMustPredictCounterfactual()
        Dim dataset As PerturbationDataset = BuildDataset()
        Dim model As ConditionalVAE = BuildModel()
        Dim experiment As New VAGeneExperiment(model, TestGeneNames, TestConditionNames)

        Call experiment.Train(dataset, epochs:=120, batchSize:=32, learningRate:=0.01, beta:=0.5, verbose:=False)

        Call DebugAudit.Test(experiment.IsFitted, "训练完成后 IsFitted 应为 True")
        Call DebugAudit.Test(experiment.ControlCondition = 0, "对照条件应为 control")

        Call experiment.SetContext(BuildContext(dataset))

        Call DebugAudit.Test(experiment.HasContext, "SetContext 后 HasContext 应为 True")
        Call DebugAudit.Test(experiment.ContextDiagnostics.Matched = GeneCount,
                             $"上下文应命中全部 {GeneCount} 个基因")

        Call experiment.RegisterPerturbation("gene0", "drug")

        Call DebugAudit.Test(experiment.RegisteredPerturbations.ContainsKey("gene0"),
                             "显式登记后扰动表应包含 gene0")

        Dim result As InterventionResult = experiment.KnockoutGene("gene0")

        Call DebugAudit.Test(result.GeneNames.Length = GeneCount, $"干预结果基因数应为 {GeneCount}")
        Call DebugAudit.Test(AuditSupport.AllFinite(result.WildtypeMeans), "野生型均值必须为有限值")
        Call DebugAudit.Test(AuditSupport.AllFinite(result.MutantMeans), "扰动后均值必须为有限值")
        Call DebugAudit.Test(AuditSupport.AllFinite(result.FoldChanges), "fold change 必须为有限值")
        Call DebugAudit.Test(result.Spec.Mode = InterventionMode.Knockout, "干预模式应为 Knockout")
        Call DebugAudit.Test(result.Spec.GeneIndex = 0, "gene0 的索引应为 0")
        Call DebugAudit.Test(result.FoldChanges.Any(Function(value) std.Abs(value) > 1.0E-12),
                             "反事实应产生非零效应")
    End Sub

#End Region

#Region "上下文"

    ''' <summary>切换对齐模式后应保留上下文，ClearContext 应清空。</summary>
    Public Sub ContextMustRealignAndClear()
        Dim dataset As PerturbationDataset = BuildDataset()
        Dim model As ConditionalVAE = BuildModel()
        Dim experiment As New VAGeneExperiment(model, TestGeneNames, TestConditionNames)

        Call experiment.Train(dataset, epochs:=40, batchSize:=32, learningRate:=0.01, beta:=0.5, verbose:=False)
        Call experiment.SetContext(BuildContext(dataset))

        Call DebugAudit.Test(experiment.AlignmentMode = ContextAlignmentMode.Quantile,
                             "默认对齐模式应为 Quantile")

        Call experiment.SetContextAlignmentMode(ContextAlignmentMode.Affine)

        Call DebugAudit.Test(experiment.AlignmentMode = ContextAlignmentMode.Affine,
                             "切换后对齐模式应为 Affine")
        Call DebugAudit.Test(experiment.HasContext, "切换对齐模式后上下文仍应有效")

        experiment.ClearContext()

        Call DebugAudit.Test(Not experiment.HasContext, "ClearContext 后 HasContext 应为 False")
    End Sub

#End Region

#Region "错误路径"

    ''' <summary>未注入上下文、未登记扰动、目标等于对照时都应明确抛错。</summary>
    Public Sub ErrorsMustBeExplicit()
        Dim dataset As PerturbationDataset = BuildDataset()
        Dim model As ConditionalVAE = BuildModel()
        Dim experiment As New VAGeneExperiment(model, TestGeneNames, TestConditionNames)

        Call ExpectThrows(Of InvalidOperationException)(
            Sub() experiment.KnockoutGene("gene0"),
            "未训练 / 未注入上下文应抛 InvalidOperationException")

        Call experiment.Train(dataset, epochs:=20, batchSize:=32, learningRate:=0.01, beta:=0.5, verbose:=False)

        Call ExpectThrows(Of InvalidOperationException)(
            Sub() experiment.KnockoutGene("gene0"),
            "未注入上下文应抛 InvalidOperationException")

        Call experiment.SetContext(BuildContext(dataset))

        Call ExpectThrows(Of KeyNotFoundException)(
            Sub() experiment.KnockoutGene("not-a-gene"),
            "未登记扰动应抛 KeyNotFoundException")

        Call ExpectThrows(Of ArgumentException)(
            Sub() experiment.KnockoutGene("control"),
            "目标条件等于对照条件应抛 ArgumentException")
    End Sub

    Private Sub ExpectThrows(Of T As Exception)(action As Action, message As String)
        Try
            Call action()
            Call DebugAudit.Test(False, message & "（未抛出异常）")
        Catch ex As Exception
            Call DebugAudit.Test(TypeOf ex Is T, message & $"（实际抛出 {ex.GetType().Name}）")
        End Try
    End Sub

#End Region

#Region "存档"

    ''' <summary>存档往返后模型预测应逐位一致，元数据应保留。</summary>
    Public Sub StorageMustRoundTrip()
        Dim dataset As PerturbationDataset = BuildDataset()
        Dim model As ConditionalVAE = BuildModel()
        Dim experiment As New VAGeneExperiment(model, TestGeneNames, TestConditionNames)

        Call experiment.Train(dataset, epochs:=60, batchSize:=32, learningRate:=0.01, beta:=0.5, verbose:=False)
        Call experiment.SetContext(BuildContext(dataset))
        Call experiment.RegisterPerturbation("gene0", 1)

        Dim before As InterventionResult = experiment.KnockoutGene("gene0")
        Dim path As String = IO.Path.Combine(AppContext.BaseDirectory, "vagene-audit-model.zip")

        Try
            experiment.Save(path)
            Call DebugAudit.Test(IO.File.Exists(path), "存档文件应已写出")

            Dim restored As VAGeneExperiment = VAGeneExperiment.Load(path)

            Call DebugAudit.Test(restored.IsFitted, "加载后 IsFitted 应为 True")
            Call DebugAudit.Test(restored.ControlCondition = experiment.ControlCondition, "对照条件应保留")
            Call DebugAudit.Test(restored.AlignmentMode = experiment.AlignmentMode, "对齐模式应保留")
            Call DebugAudit.Test(restored.RegisteredPerturbations.ContainsKey("gene0"), "扰动登记表应保留")

            Call restored.SetContext(BuildContext(dataset))

            Dim after As InterventionResult = restored.KnockoutGene("gene0")
            Dim errorValue As Double = AuditSupport.MaxAbsDiff(
                New Tensor(before.MutantMeans, 1, GeneCount),
                New Tensor(after.MutantMeans, 1, GeneCount))

            Call DebugAudit.Capture("vagene.storage.max_abs_diff", errorValue)
            Call DebugAudit.Test(errorValue < 1.0E-09, $"存档往返预测偏差应小于 1E-09，实际 {errorValue:E3}")
        Finally
            If IO.File.Exists(path) Then IO.File.Delete(path)
        End Try
    End Sub

#End Region

#Region "合成数据"

    ''' <summary>两组条件：drug 条件下 gene0 显著上调，其余基因不变。</summary>
    Private Function BuildDataset() As PerturbationDataset
        Dim cellsPerCondition As Integer = 100
        Dim cells As Integer = cellsPerCondition * ConditionCount
        Dim counts As Double() = New Double(cells * GeneCount - 1) {}
        Dim conditions As Integer() = New Integer(cells - 1) {}
        Dim rng As New Random(20261124)

        For i As Integer = 0 To cells - 1
            Dim condition As Integer = If(i < cellsPerCondition, 0, 1)
            Dim drug As Boolean = condition = 1

            conditions(i) = condition

            For g As Integer = 0 To GeneCount - 1
                Dim rate As Double = 5.0

                If drug AndAlso g = 0 Then
                    rate = 25.0
                End If

                counts(i * GeneCount + g) = std.Max(0.0, std.Round(rate + 2.0 * AuditSupport.Randn(rng)))
            Next
        Next

        Return PerturbationDataset.FromCounts(
            New Tensor(counts, cells, GeneCount),
            conditions,
            TestConditionNames)
    End Function

    Private Function BuildModel() As ConditionalVAE
        Dim model As New ConditionalVAE(GeneCount, ConditionCount, latentDim:=4, hiddenDim:=16, seed:=20261124)

        model.Likelihood.Kind = CountLikelihoodKind.NegativeBinomial
        model.Likelihood.Dispersion = 1.5

        Return model
    End Function

    ''' <summary>取模型输入空间（log1p 文库归一化）的逐基因列均值作为上下文。</summary>
    Private Function BuildContextVector(dataset As PerturbationDataset) As Tensor
        Dim logExpression As Tensor = dataset.LogNormalize()
        Dim cells As Integer = logExpression.Shape(0)
        Dim genes As Integer = logExpression.Shape(1)
        Dim data As Double() = logExpression.Data
        Dim values As Double() = New Double(genes - 1) {}

        For g As Integer = 0 To genes - 1
            Dim sum As Double = 0

            For i As Integer = 0 To cells - 1
                sum += data(i * genes + g)
            Next

            values(g) = sum / cells
        Next

        Return Tensor.Wrap(values, 1, genes)
    End Function

    Private Function BuildContext(dataset As PerturbationDataset) As Dictionary(Of String, Double)
        Dim vector As Tensor = BuildContextVector(dataset)
        Dim context As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        For g As Integer = 0 To GeneCount - 1
            context(TestGeneNames(g)) = vector.Data(g)
        Next

        Return context
    End Function

#End Region

End Module
