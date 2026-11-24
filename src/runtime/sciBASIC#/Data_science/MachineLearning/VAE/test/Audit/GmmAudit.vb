' ============================================================================
' GmmAudit.vb - 高斯混合模型的生产级审计
'
' 覆盖：四种协方差结构、BIC/AIC、模型持久化往返、全协方差采样相关性还原、
'       空簇处理、输入维度校验、静态线性代数门面、TrainingOptions 重载。
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports std = System.Math

''' <summary>
''' 高斯混合模型生产级审计。
''' </summary>
Public Module GmmAudit

    Public Sub Run()
        Call AuditSupport.RunCase("gmm.covariance-types", AddressOf CovarianceTypesMustConverge)
        Call AuditSupport.RunCase("gmm.model-selection", AddressOf BicMustSelectTrueComponents)
        Call AuditSupport.RunCase("gmm.persistence", AddressOf ModelMustRoundTrip)
        Call AuditSupport.RunCase("gmm.sampling-correlation", AddressOf SamplingMustKeepCorrelation)
        Call AuditSupport.RunCase("gmm.empty-cluster", AddressOf EmptyClustersMustNotProduceNaN)
        Call AuditSupport.RunCase("gmm.validation", AddressOf InputValidationMustThrow)
        Call AuditSupport.RunCase("gmm.linear-algebra", AddressOf StaticLinearAlgebraMustDelegate)
        Call AuditSupport.RunCase("gmm.options-overload", AddressOf TrainingOptionsOverloadMustWork)
    End Sub

#Region "协方差结构"

    Private Sub CovarianceTypesMustConverge()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(3, 120, 3, 0.4, 21, X, labels)

        For Each kind As GmmCovarianceType In {GmmCovarianceType.Full,
                                               GmmCovarianceType.Diagonal,
                                               GmmCovarianceType.Tied,
                                               GmmCovarianceType.Spherical}

            Dim options As New GmmOptions(kind)
            Dim model As New GaussianMixtureModel(3, options, maxIter:=120, seed:=21)

            Call model.Fit(X)

            Dim tag As String = kind.ToString()

            Call DebugAudit.Test(AuditSupport.AllFinite(model.Weights), $"[{tag}] 权重必须有限")
            Call DebugAudit.Test(AuditSupport.AllFinite(model.Means), $"[{tag}] 均值必须有限")
            Call DebugAudit.Test(AuditSupport.AllFinite(model.Covariances), $"[{tag}] 协方差必须有限")
            Call DebugAudit.Test(AuditSupport.IsFinite(model.LogLikelihood), $"[{tag}] 对数似然必须有限")

            Dim total As Double = AuditSupport.SumOf(model.Weights)
            Call DebugAudit.Test(std.Abs(total - 1.0) < 1.0E-6, $"[{tag}] 权重之和必须为 1，实际 {total}")

            Dim accuracy As Double = AuditSupport.ClusterAccuracy(model.Predict(X), labels, 3)
            Call DebugAudit.Capture($"gmm.accuracy.{tag}", accuracy)

            ' Spherical 的自由度最低，在可分离数据上仍应恢复主要结构
            Call DebugAudit.Test(accuracy > 0.9, $"[{tag}] 聚类准确率应高于 90%，实际 {accuracy:P2}")

            ' 形状约定
            If kind = GmmCovarianceType.Diagonal OrElse kind = GmmCovarianceType.Spherical Then
                Call DebugAudit.Test(model.Covariances.Rank = 2,
                                     $"[{tag}] 对角协方差应为 [K, D]，实际秩 {model.Covariances.Rank}")
            Else
                Call DebugAudit.Test(model.Covariances.Rank = 3,
                                     $"[{tag}] 全协方差应为 [K, D, D]，实际秩 {model.Covariances.Rank}")
            End If
        Next
    End Sub

#End Region

#Region "模型选择"

    Private Sub BicMustSelectTrueComponents()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(3, 100, 3, 0.35, 33, X, labels)

        Dim results As ModelSelectionResult() =
            ModelSelection.GridSearch(
                New Integer() {1, 2, 3, 4, 5, 6},
                Function(k) New GaussianMixtureModel(k, GmmCovarianceType.Diagonal, maxIter:=120, seed:=33),
                Sub(model) model.Fit(X),
                X.Shape(0))

        Call DebugAudit.Test(results.Length = 6, $"网格搜索应返回 6 个候选，实际 {results.Length}")

        For Each item As ModelSelectionResult In results
            Call DebugAudit.Test(AuditSupport.IsFinite(item.BIC), $"K={item.NComponents} 的 BIC 必须有限")
            Call DebugAudit.Test(AuditSupport.IsFinite(item.AIC), $"K={item.NComponents} 的 AIC 必须有限")
        Next

        ' 结果按 BIC 升序
        For i As Integer = 1 To results.Length - 1
            Call DebugAudit.Test(results(i - 1).BIC <= results(i).BIC, "网格搜索结果必须按 BIC 升序排列")
        Next

        Dim best As ModelSelectionResult = ModelSelection.BestByBic(results)
        Call DebugAudit.Capture("gmm.bic.best-k", best.NComponents)

        Call DebugAudit.Test(best.NComponents = 3,
                             $"BIC 应选择真实成分数 3，实际选择了 {best.NComponents}")

        ' 参数计数
        Dim model3 As New GaussianMixtureModel(3, GmmCovarianceType.Diagonal, maxIter:=10, seed:=1)
        Call model3.Fit(X)

        Call DebugAudit.Test(model3.NParameters = (3 - 1) + 3 * 3 + 3 * 3,
                             $"对角协方差 K=3, D=3 的自由参数应为 2+9+9，实际 {model3.NParameters}")
    End Sub

#End Region

#Region "持久化"

    Private Sub ModelMustRoundTrip()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 80, 3, 0.4, 44, X, labels)

        Dim model As New GaussianMixtureModel(2, maxIter:=80, seed:=44)
        Call model.Fit(X)

        Dim path As String = IO.Path.Combine(AppContext.BaseDirectory, "gmm-audit-model.bin")
        Call model.Save(path)

        Dim restored As New GaussianMixtureModel(2, maxIter:=80, seed:=44)
        Dim errorMessage As String = Nothing
        Dim succeeded As Boolean = restored.TryLoad(path, errorMessage)

        Call DebugAudit.Test(succeeded, $"模型加载失败: {errorMessage}")

        If Not succeeded Then
            Return
        End If

        Call DebugAudit.Test(restored.Weights IsNot Nothing, "还原后的权重不应为 Nothing")
        Call DebugAudit.Test(restored.Means IsNot Nothing, "还原后的均值不应为 Nothing")
        Call DebugAudit.Test(restored.Covariances IsNot Nothing, "还原后的协方差不应为 Nothing")

        If restored.Weights Is Nothing OrElse restored.Means Is Nothing OrElse restored.Covariances Is Nothing Then
            Return
        End If

        Call DebugAudit.Test(AuditSupport.AllFinite(restored.Weights), "还原后的权重必须为有限值")
        Call DebugAudit.Test(AuditSupport.AllFinite(restored.Means), "还原后的均值必须为有限值")
        Call DebugAudit.Test(AuditSupport.AllFinite(restored.Covariances), "还原后的协方差必须为有限值")

        Call DebugAudit.Test(std.Abs(restored.LogLikelihood - model.LogLikelihood) < 1.0E-10,
                             "还原后的对数似然必须一致")
        Call DebugAudit.Test(AuditSupport.MaxAbsDiff(restored.Means, model.Means) < 1.0E-15,
                             "还原后的均值必须逐元素一致")
        Call DebugAudit.Test(AuditSupport.MaxAbsDiff(restored.Covariances, model.Covariances) < 1.0E-15,
                             "还原后的协方差必须逐元素一致")

        Dim before As Integer() = model.Predict(X)
        Dim after As Integer() = restored.Predict(X)

        For i As Integer = 0 To before.Length - 1
            If before(i) <> after(i) Then
                Call DebugAudit.Test(False, $"还原后的预测在样本 {i} 处不一致")
                Exit For
            End If
        Next

        ' 加载不存在的文件必须优雅失败
        Dim missing As New GaussianMixtureModel(2)
        Call DebugAudit.Test(Not missing.TryLoad(path & ".nope", errorMessage),
                             "加载不存在的文件应返回 False")

        If IO.File.Exists(path) Then
            IO.File.Delete(path)
        End If
    End Sub

#End Region

#Region "采样"

    Private Sub SamplingMustKeepCorrelation()
        ' 构造强相关的二维数据：y = 0.95x + 弱噪声
        Dim rng As New Random(77)
        Const count As Integer = 6000
        Dim data As Double() = New Double(count * 2 - 1) {}

        For i As Integer = 0 To count - 1
            Dim pointX As Double = AuditSupport.Randn(rng) * 2.0
            Dim pointY As Double = 0.95 * pointX + AuditSupport.Randn(rng) * 0.6
            data(i * 2) = pointX
            data(i * 2 + 1) = pointY
        Next

        Dim trainingData As New Tensor(data, count, 2)

        Dim model As New GaussianMixtureModel(1, GmmCovarianceType.Full, maxIter:=40, seed:=77)
        Call model.Fit(trainingData)

        Dim samples As Tensor = model.Sample(30000, seed:=99)

        Dim sumX As Double = 0
        Dim sumY As Double = 0
        Dim sumXY As Double = 0

        For i As Integer = 0 To samples.Shape(0) - 1
            Dim sampleX As Double = samples(i, 0)
            Dim sampleY As Double = samples(i, 1)
            sumX += sampleX
            sumY += sampleY
            sumXY += sampleX * sampleY
        Next

        Dim n As Integer = samples.Shape(0)
        Dim covariance As Double = sumXY / n - (sumX / n) * (sumY / n)

        ' 真实协方差：var(x)=4, cov(x,y)=0.95*4=3.8
        Call DebugAudit.Capture("gmm.sample.cov", covariance)

        Call DebugAudit.Test(std.Abs(covariance - 3.8) < 0.3,
                             $"采样协方差应还原为约 3.8，实际 {covariance}（只取对角元的旧实现恒为 0）")
    End Sub

#End Region

#Region "健壮性"

    Private Sub EmptyClustersMustNotProduceNaN()
        ' 只有 2 个真实簇，却要求 5 个成分 —— 必然出现空簇
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 60, 3, 0.3, 55, X, labels)

        Dim model As New GaussianMixtureModel(5, maxIter:=100, seed:=55)
        Call model.Fit(X)

        Call DebugAudit.Test(AuditSupport.AllFinite(model.Weights),
                             "空簇场景下权重必须保持有限（旧实现会坍缩为 NaN）")
        Call DebugAudit.Test(AuditSupport.AllFinite(model.Means), "空簇场景下均值必须保持有限")
        Call DebugAudit.Test(AuditSupport.AllFinite(model.Covariances), "空簇场景下协方差必须保持有限")
        Call DebugAudit.Test(AuditSupport.IsFinite(model.LogLikelihood), "空簇场景下对数似然必须保持有限")

        Dim total As Double = AuditSupport.SumOf(model.Weights)
        Call DebugAudit.Test(std.Abs(total - 1.0) < 1.0E-6, $"权重之和必须为 1，实际 {total}")

        ' 重复调用 Fit 不应累积历史
        Call model.Fit(X)
        Call DebugAudit.Test(model.LogLikelihoodHistory.Count <= 100,
                             $"重复 Fit 不应累积历史，实际 {model.LogLikelihoodHistory.Count} 条")
    End Sub

    Private Sub InputValidationMustThrow()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 40, 3, 0.4, 66, X, labels)

        Dim model As New GaussianMixtureModel(2, maxIter:=50, seed:=66)

        ' 未训练就预测
        Call DebugAudit.CheckThrows(Of InvalidOperationException)(
            Sub() Call model.Predict(X),
            "未训练时 Predict 必须抛出 InvalidOperationException")

        Call model.Fit(X)

        ' 维度不符
        Dim wrong As New Tensor(New Double(5 * 4 - 1) {}, 5, 4)
        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub() Call model.Predict(wrong),
            "维度不符时 Predict 必须抛出 ArgumentException")

        ' 非二维输入
        Dim flat As New Tensor(New Double() {1.0, 2.0, 3.0}, 3)
        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub() Call model.Fit(flat),
            "非二维输入时 Fit 必须抛出 ArgumentException")
    End Sub

    Private Sub StaticLinearAlgebraMustDelegate()
        Dim data As Double(,) = {
            {3.0, 1.0},
            {1.0, 2.0}
        }

        Dim a As New Tensor(data)
        Dim inverse As Tensor = GaussianMixtureModel.MatrixInverse(a, 2)

        Dim product As Tensor = a.MatMul(inverse)

        For r As Integer = 0 To 1
            For c As Integer = 0 To 1
                Dim expected As Double = If(r = c, 1.0, 0.0)
                Call DebugAudit.Test(std.Abs(product(r, c) - expected) < 1.0E-10,
                                     $"A·A⁻¹ 应为单位阵，位置 ({r},{c}) 实际为 {product(r, c)}")
            Next
        Next

        Dim logDet As Double = GaussianMixtureModel.LogDeterminant(a, 2)
        ' det = 6-1 = 5
        Call DebugAudit.Test(std.Abs(logDet - std.Log(5.0)) < 1.0E-10,
                             $"ln|A| 应为 ln5，实际 {logDet}")
    End Sub

    Private Sub TrainingOptionsOverloadMustWork()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 50, 2, 0.4, 88, X, labels)

        Dim model As New GaussianMixtureModel(2, seed:=88)
        Dim options As New TrainingOptions With {.Epochs = 60}

        Dim trained As GaussianMixtureModel = model.Fit(X, options)

        Call DebugAudit.Test(trained Is model, "Fit(X, options) 应返回自身以支持链式调用")
        Call DebugAudit.Test(model.NIter <= 60, $"迭代次数不得超过 60，实际 {model.NIter}")
        Call DebugAudit.Test(AuditSupport.IsFinite(model.LogLikelihood), "对数似然必须有限")
    End Sub

#End Region

End Module
