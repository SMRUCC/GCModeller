' ============================================================================
' VBDemo.vb - 变分贝叶斯模型族的演示与审计
'
' 覆盖三个模型：
'   1. VB-GMM   : 合成三簇二维数据，训练 K = 8 的模型，验证成分剪枝与聚类精度
'   2. VB 回归  : 合成线性关系 y = 2 + 3x1 - 1.5x2 + 噪声，验证系数恢复
'   3. VB-PCA   : 秩 2 的六维加噪数据，验证 ARD 自动确定有效维度与重构精度
'
' 所有断言经由 DebugAudit 记录，运行结束之后在 bin 目录下生成 JSON 测试报告。
' 注意：Snapshot 带有 <Conditional("DEBUG")>，因此请使用 Debug 配置运行本演示。
' ============================================================================

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.VariationalBayes
Imports std = System.Math

''' <summary>
''' 变分贝叶斯（VB）模型族的演示程序与自检套件。
''' </summary>
Public Class VBDemo

    Private Const ReportName As String = "vb-audit-report.json"

    ''' <summary>
    ''' 演示入口。
    ''' </summary>
    Public Shared Sub Main3(args As String())
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 8192
        Call DebugAudit.Mark("variational bayes audit started")

        Try
            Call RunGmmAudit()
            Call RunRegressionAudit()
            Call RunPcaAudit()
        Catch ex As Exception
            Call DebugAudit.Mark("audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"审计套件不应抛出未处理异常: {ex.Message}")
        End Try

        Call DebugAudit.Snapshot(
            DebugAudit.V(ReportName),
            DebugAudit.V(DebugAudit.FailureCount))

        Dim dump As String = DebugAudit.Dump()

        Call DebugAudit.SaveReport(reportPath)

        Console.WriteLine()
        Console.WriteLine($"   [audit] VB report -> {reportPath}")
        Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")
        Console.WriteLine()
        Console.WriteLine(dump)
    End Sub

#Region "VB-GMM"

    Private Shared Sub RunGmmAudit()
        Console.WriteLine("="c, 80)
        Console.WriteLine("VB-GMM: 变分贝叶斯高斯混合模型 (Dirichlet + Normal-Wishart)")
        Console.WriteLine("="c, 80)

        Dim rng As New Random(2024)
        Dim perCluster As Integer = 200
        Dim trueComponents As Integer = 3
        Dim n As Integer = perCluster * trueComponents

        Dim means As Double(,) = {
            {0.0R, 0.0R},
            {6.0R, 3.0R},
            {2.5R, 8.0R}
        }

        ' 每个簇的线性变换 [a11, a12, a21, a22]
        Dim transforms As Double(,) = {
            {1.0R, 0.3R, 0.0R, 0.8R},
            {0.9R, 0.0R, 0.2R, 0.9R},
            {0.7R, -0.3R, 0.3R, 0.6R}
        }

        Dim data As Double(,) = New Double(n - 1, 1) {}
        Dim truth As Integer() = New Integer(n - 1) {}

        For i As Integer = 0 To n - 1
            Dim cluster As Integer = i \ perCluster
            Dim z1 As Double = NextGaussian(rng)
            Dim z2 As Double = NextGaussian(rng)

            truth(i) = cluster
            data(i, 0) = means(cluster, 0) + transforms(cluster, 0) * z1 + transforms(cluster, 1) * z2
            data(i, 1) = means(cluster, 1) + transforms(cluster, 2) * z1 + transforms(cluster, 3) * z2
        Next

        Dim X As New Tensor(data)

        ' 变分贝叶斯混合模型对初值比较敏感：这里使用多次随机重启，
        ' 之后保留下界最大的那一次（标准做法）。
        Dim restarts As Integer = 8
        Dim model As VariationalGaussianMixture = Nothing

        Using DebugAudit.Watch("vb-gmm.fit", logOnExit:=True)
            For round As Integer = 0 To restarts - 1
                Dim candidate As New VariationalGaussianMixture(
                    nComponents:=8, maxIter:=300, tol:=0.0000001R, seed:=42 + round)

                Call candidate.Fit(X)

                Dim candidateAccuracy As Double = GreedyAccuracy(truth, candidate.Predict(X), trueComponents, 8)

                Console.WriteLine($"   重启 {round + 1}/{restarts}: ELBO={candidate.ELBO:F2}, " &
                                  $"有效成分={candidate.EffectiveComponents}, 精度={candidateAccuracy:P2}")

                If model Is Nothing OrElse candidate.ELBO > model.ELBO Then
                    model = candidate
                End If
            Next
        End Using

        Call DebugAudit.Mark("vb-gmm.elbo", model.ELBO)
        Call DebugAudit.Mark("vb-gmm.iterations", model.NIter)
        Call DebugAudit.Mark("vb-gmm.converged", model.Converged)
        Call DebugAudit.Mark("vb-gmm.effective", model.EffectiveComponents)

        Console.WriteLine($"   训练完成: iter={model.NIter}, converged={model.Converged}, ELBO={model.ELBO:F4}")
        Console.WriteLine($"   有效成分数 = {model.EffectiveComponents} (真实 = {trueComponents})")

        Dim weights As Tensor = model.Weights

        For k As Integer = 0 To weights.Length - 1
            Console.WriteLine($"     成分 {k}: 权重 = {weights(k):F6}")
        Next

        Console.WriteLine()

        Call AssertMonotoneELBO("vb-gmm", model.ELBOHistory)

        ' ---------- 责任度每一行的和应该为 1 ----------
        Dim resp As Tensor = model.Responsibilities
        Dim maxRowError As Double = 0

        For i As Integer = 0 To resp.Shape(0) - 1
            Dim acc As Double = 0

            For k As Integer = 0 To resp.Shape(1) - 1
                acc += resp(i, k)
            Next

            maxRowError = std.Max(maxRowError, std.Abs(acc - 1.0R))
        Next

        Call DebugAudit.Mark("vb-gmm.responsibilityError", maxRowError)
        Call DebugAudit.Test(maxRowError < 0.000001R, "责任度矩阵的每一行之和应该为 1")

        ' ---------- 聚类精度（贪心匹配） ----------
        Dim predicted As Integer() = model.Predict(X)
        Dim accuracy As Double = GreedyAccuracy(truth, predicted, trueComponents, 8)

        Call DebugAudit.Mark("vb-gmm.accuracy", accuracy)
        Call DebugAudit.Test(accuracy >= 0.95R, $"聚类精度应该不低于 95%，实际为 {accuracy:P2}")

        ' ---------- 成分剪枝 ----------
        Call DebugAudit.Test(model.EffectiveComponents = trueComponents,
                             $"K=8 的模型应该自动剪枝到 {trueComponents} 个有效成分，实际为 {model.EffectiveComponents}")

        ' ---------- 责任度的确定性 ----------
        Dim score1 As Double = model.Score(X)
        Dim score2 As Double = model.Score(X)

        Call DebugAudit.Mark("vb-gmm.score", score1)
        Call DebugAudit.Test(std.Abs(score1 - score2) < 0.000000001R, "对同一批数据的评分应该可以重复")
        Call DebugAudit.Test(Not Double.IsNaN(score1), "评分不应该为 NaN")

        Console.WriteLine($"   平均变分下界 = {score1:F4}, 聚类精度 = {accuracy:P2}")
        Console.WriteLine()
    End Sub

#End Region

#Region "VB 线性回归"

    Private Shared Sub RunRegressionAudit()
        Console.WriteLine("="c, 80)
        Console.WriteLine("VB-LinearRegression: 变分贝叶斯线性回归")
        Console.WriteLine("="c, 80)

        Dim rng As New Random(7)
        Dim n As Integer = 400
        Dim features As Double(,) = New Double(n - 1, 1) {}
        Dim targets As Double() = New Double(n - 1) {}
        Dim noiseSigma As Double = 0.25R

        For i As Integer = 0 To n - 1
            Dim x1 As Double = rng.NextDouble() * 4.0R - 2.0R
            Dim x2 As Double = rng.NextDouble() * 4.0R - 2.0R

            features(i, 0) = x1
            features(i, 1) = x2
            targets(i) = 2.0R + 3.0R * x1 - 1.5R * x2 + NextGaussian(rng) * noiseSigma
        Next

        Dim design As Tensor = VariationalLinearRegression.DesignMatrix(New Tensor(features), addIntercept:=True)
        Dim y As New Tensor(targets, n)
        Dim model As New VariationalLinearRegression(maxIter:=300, tol:=0.00000001R, seed:=11)

        Using DebugAudit.Watch("vb-regression.fit", logOnExit:=True)
            Call model.Fit(design, y)
        End Using

        Call DebugAudit.Mark("vb-regression.elbo", model.ELBO)
        Call DebugAudit.Mark("vb-regression.iterations", model.NIter)
        Call DebugAudit.Mark("vb-regression.alpha", model.Alpha)
        Call DebugAudit.Mark("vb-regression.beta", model.Beta)

        Console.WriteLine($"   训练完成: iter={model.NIter}, converged={model.Converged}, ELBO={model.ELBO:F4}")
        Console.WriteLine($"   E[α] = {model.Alpha:F6}, E[β] = {model.Beta:F6}, 噪声方差 = {model.ResidualVariance:F6}")

        Call AssertMonotoneELBO("vb-regression", model.ELBOHistory)

        ' ---------- 系数恢复 ----------
        Dim coefficient As Tensor = model.Mean
        Dim truth As Double() = {2.0R, 3.0R, -1.5R}
        Dim maxRelativeError As Double = 0

        For i As Integer = 0 To truth.Length - 1
            Dim relative As Double = std.Abs(coefficient(i) - truth(i)) / std.Max(1.0R, std.Abs(truth(i)))

            maxRelativeError = std.Max(maxRelativeError, relative)
            Console.WriteLine($"   系数 {i}: 估计 = {coefficient(i):F4}, 真实 = {truth(i):F4}, 相对误差 = {relative:P2}")
        Next

        Call DebugAudit.Mark("vb-regression.maxRelativeError", maxRelativeError)
        Call DebugAudit.Test(maxRelativeError < 0.05R,
                             $"回归系数的相对误差应该小于 5%，实际最大值为 {maxRelativeError:P2}")

        ' ---------- 拟合优度 ----------
        Dim prediction As Tensor = model.Predict(design)
        Dim meanTarget As Double = 0

        For i As Integer = 0 To n - 1
            meanTarget += targets(i)
        Next

        meanTarget /= n

        Dim ssResidual As Double = 0
        Dim ssTotal As Double = 0

        For i As Integer = 0 To n - 1
            Dim residual As Double = targets(i) - prediction(i)
            Dim total As Double = targets(i) - meanTarget

            ssResidual += residual * residual
            ssTotal += total * total
        Next

        Dim r2 As Double = 1.0R - ssResidual / ssTotal

        Call DebugAudit.Mark("vb-regression.r2", r2)
        Call DebugAudit.Test(r2 >= 0.95R, $"训练集上的决定系数应该不低于 0.95，实际为 {r2:F4}")
        Console.WriteLine($"   R² = {r2:F6}")

        ' ---------- 预测方差 ----------
        Dim variance As Tensor = model.PredictiveVariance(design)
        Dim minVariance As Double = Double.MaxValue

        For i As Integer = 0 To variance.Length - 1
            minVariance = std.Min(minVariance, variance(i))
        Next

        Call DebugAudit.Mark("vb-regression.minPredictiveVariance", minVariance)
        Call DebugAudit.Test(minVariance > 0, "预测方差必须为正数")
        Console.WriteLine($"   最小预测方差 = {minVariance:F6}")
        Console.WriteLine()
    End Sub

#End Region

#Region "VB-PCA"

    Private Shared Sub RunPcaAudit()
        Console.WriteLine("="c, 80)
        Console.WriteLine("VB-PCA: 变分贝叶斯主成分分析 (ARD 自动确定有效维度)")
        Console.WriteLine("="c, 80)

        Dim rng As New Random(99)
        Dim n As Integer = 400
        Dim dims As Integer = 6
        Dim trueRank As Integer = 2
        Dim noiseSigma As Double = 0.05R

        Dim trueLoadings As Double(,) = {
            {1.0R, 0.0R},
            {0.8R, 0.2R},
            {0.5R, -0.6R},
            {-0.3R, 0.9R},
            {0.2R, 0.2R},
            {0.6R, -0.1R}
        }

        Dim data As Double(,) = New Double(n - 1, dims - 1) {}

        For i As Integer = 0 To n - 1
            Dim z As Double() = {NextGaussian(rng), NextGaussian(rng)}

            For j As Integer = 0 To dims - 1
                Dim value As Double = 0

                For k As Integer = 0 To trueRank - 1
                    value += trueLoadings(j, k) * z(k)
                Next

                data(i, j) = value + NextGaussian(rng) * noiseSigma
            Next
        Next

        Dim X As New Tensor(data)
        ' VB-PCA 的坐标上升在 ARD 精度参数上收敛较慢，这里使用相对宽松的 1e-4 阈值
        Dim model As New VariationalPCA(latentDim:=dims, maxIter:=1000, tol:=0.0001R, seed:=5)

        Using DebugAudit.Watch("vb-pca.fit", logOnExit:=True)
            Call model.Fit(X)
        End Using

        Call DebugAudit.Mark("vb-pca.elbo", model.ELBO)
        Call DebugAudit.Mark("vb-pca.iterations", model.NIter)
        Call DebugAudit.Mark("vb-pca.effective", model.EffectiveDimension)
        Call DebugAudit.Mark("vb-pca.beta", model.NoisePrecision)

        Console.WriteLine($"   训练完成: iter={model.NIter}, converged={model.Converged}, ELBO={model.ELBO:F4}")
        Console.WriteLine($"   有效维度 = {model.EffectiveDimension} (真实秩 = {trueRank})")
        Console.WriteLine($"   噪声精度 E[β] = {model.NoisePrecision:F4}, 噪声标准差估计 = {std.Sqrt(model.ResidualVariance):F4}")

        Dim precision As Tensor = model.LatentPrecision

        For i As Integer = 0 To precision.Length - 1
            Console.WriteLine($"     潜变量 {i}: E[α_i] = {precision(i):E3}")
        Next

        Console.WriteLine()

        Call AssertMonotoneELBO("vb-pca", model.ELBOHistory)

        Call DebugAudit.Test(model.Components.Shape(0) = dims AndAlso model.Components.Shape(1) = dims,
                             "载荷矩阵的形状应该是 [D, L]")
        Call DebugAudit.Test(model.Scores.Shape(0) = n AndAlso model.Scores.Shape(1) = dims,
                             "隐变量得分矩阵的形状应该是 [N, L]")

        Call DebugAudit.Test(model.EffectiveDimension = trueRank,
                             $"ARD 应该把有效维度确定在 {trueRank}，实际为 {model.EffectiveDimension}")

        Dim rmse As Double = model.ReconstructionError(X)

        Call DebugAudit.Mark("vb-pca.rmse", rmse)
        Call DebugAudit.Test(rmse < 0.3R, $"重构 RMSE 应该明显低于信号尺度，实际为 {rmse:F4}")
        Console.WriteLine($"   重构 RMSE = {rmse:F6}")

        ' ---------- 缺失值掩码 ----------
        Dim masked As Double(,) = DirectCast(data.Clone(), Double(,))

        masked(3, 1) = Double.NaN
        masked(17, 4) = Double.NaN

        Dim maskedModel As New VariationalPCA(latentDim:=trueRank, maxIter:=500, tol:=0.0001R, seed:=5)

        Using DebugAudit.Watch("vb-pca.masked.fit", logOnExit:=False)
            Call maskedModel.Fit(New Tensor(masked))
        End Using

        Dim maskedError As Double = maskedModel.ReconstructionError(New Tensor(masked))

        Call DebugAudit.Mark("vb-pca.masked.rmse", maskedError)
        Call DebugAudit.Test(Not Double.IsNaN(maskedError) AndAlso maskedError < 0.3R,
                             $"带缺失值的数据也应该可以正常重构，实际 RMSE = {maskedError:F4}")

        Console.WriteLine($"   带缺失值的重构 RMSE = {maskedError:F6}")
        Console.WriteLine()
    End Sub

#End Region

#Region "审计辅助"

    ''' <summary>
    ''' 检查变分下界序列是否单调不减（允许数值噪声级别的回落）。
    ''' </summary>
    Private Shared Sub AssertMonotoneELBO(tag As String, history As List(Of Double))
        Dim violations As Integer = 0
        Dim worst As Double = 0

        For i As Integer = 1 To history.Count - 1
            Dim delta As Double = history(i) - history(i - 1)
            Dim tolerance As Double = 0.000001R * std.Max(1.0R, std.Abs(history(i - 1)))

            If delta < -tolerance Then
                violations += 1
                worst = std.Min(worst, delta)
            End If
        Next

        Call DebugAudit.Mark($"{tag}.elbo.decreases", violations)
        Call DebugAudit.Mark($"{tag}.elbo.worstDelta", worst)
        Call DebugAudit.Test(violations = 0, $"{tag}: 变分下界应该单调不减，实际出现 {violations} 次回落（最差 {worst:E3}）")
        Call DebugAudit.Test(history.Count > 1, $"{tag}: 至少应该执行两轮迭代")
    End Sub

    ''' <summary>
    ''' 用贪心的 1-1 匹配把预测标签映射到真实标签，再计算准确率。
    ''' </summary>
    Private Shared Function GreedyAccuracy(truth As Integer(),
                                           predicted As Integer(),
                                           truthLabels As Integer,
                                           predictedLabels As Integer) As Double
        Dim counts As Integer(,) = New Integer(truthLabels - 1, predictedLabels - 1) {}

        For i As Integer = 0 To truth.Length - 1
            If predicted(i) >= 0 AndAlso predicted(i) < predictedLabels Then
                counts(truth(i), predicted(i)) += 1
            End If
        Next

        Dim used As Boolean() = New Boolean(predictedLabels - 1) {}
        Dim correct As Integer = 0

        For label As Integer = 0 To truthLabels - 1
            Dim best As Integer = -1
            Dim bestCount As Integer = -1

            For candidate As Integer = 0 To predictedLabels - 1
                If Not used(candidate) AndAlso counts(label, candidate) > bestCount Then
                    bestCount = counts(label, candidate)
                    best = candidate
                End If
            Next

            If best >= 0 Then
                used(best) = True
                correct += counts(label, best)
            End If
        Next

        Return correct / truth.Length
    End Function

    ''' <summary>
    ''' Box-Muller 标准正态采样。
    ''' </summary>
    Private Shared Function NextGaussian(rng As Random) As Double
        Dim u1 As Double = 1.0R - rng.NextDouble()
        Dim u2 As Double = 1.0R - rng.NextDouble()

        Return std.Sqrt(-2.0R * std.Log(u1)) * std.Cos(2.0R * std.PI * u2)
    End Function

#End Region

End Class
