Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Evaluation

Namespace test.Tests

    ''' <summary>
    ''' 评估指标的手算值对照。
    ''' </summary>
    Module Evaluation

        Public Sub Run()
            Call ConfusionChecks()
            Call CurveChecks()
            Call CalibrationChecks()
            Call DistributionChecks()
        End Sub

        Private Sub ConfusionChecks()
            ' 打分 0.9 / 0.6 / 0.4 判正，其中 0.9 与 0.6 是正样本
            Dim scores As Double() = {0.9, 0.6, 0.4, 0.2}
            Dim labels As Boolean() = {True, True, False, False}
            Dim matrix As Metrics.ConfusionMatrix = Metrics.Confusion(scores, labels, 0.5)

            Call Support.Same(2, matrix.TruePositive, "true positives")
            Call Support.Same(0, matrix.FalsePositive, "false positives")
            Call Support.Same(2, matrix.TrueNegative, "true negatives")
            Call Support.Same(0, matrix.FalseNegative, "false negatives")
            Call Support.Near(1.0, matrix.Sensitivity, 1.0E-12, "sensitivity")
            Call Support.Near(1.0, matrix.Precision, 1.0E-12, "precision")
            Call Support.Near(1.0, matrix.F1, 1.0E-12, "F1")
            Call Support.Near(1.0, matrix.Accuracy, 1.0E-12, "accuracy")
            Call Support.Near(1.0, matrix.MCC, 1.0E-12, "MCC of a perfect classifier")

            ' 全部判正的极端情形
            Dim allPositive As Metrics.ConfusionMatrix = Metrics.Confusion(scores, labels, -1.0)

            Call Support.Near(1.0, allPositive.Sensitivity, 1.0E-12, "sensitivity of the all-positive classifier")
            Call Support.Near(0.5, allPositive.Precision, 1.0E-12, "precision of the all-positive classifier")
            Call Support.Near(0.0, allPositive.Specificity, 1.0E-12, "specificity of the all-positive classifier")

            ' 正负样本比例为 1 时 MCC 为 0
            Call Support.Near(0.0, allPositive.MCC, 1.0E-12, "MCC of a trivial classifier")
        End Sub

        Private Sub CurveChecks()
            ' 打分 0.9(正)、0.6(负)、0.4(正)、0.2(负)
            ' ROC 上的点为 (FPR, TPR)：(0, 0.5)、(0.5, 0.5)、(0.5, 1)、(1, 1)
            ' 梯形积分 = 0.5*0.5 + 0.5*1 = 0.75
            Dim scores As Double() = {0.9, 0.6, 0.4, 0.2}
            Dim labels As Boolean() = {True, False, True, False}
            Dim curve As New RocPrCurve(scores, labels)

            Call Support.Same(2, curve.Positives, "the number of positives")
            Call Support.Same(2, curve.Negatives, "the number of negatives")

            Call Support.Near(0.75, curve.AucRoc(), 1.0E-12, "AUC-ROC of a hand-computed curve")

            ' 平均精确率 = 0.5*1 + 0*0.5 + 0.5*(2/3) = 0.5 + 1/3
            Call Support.Near(0.5 + 1.0 / 3.0, curve.AucPr(), 1.0E-12, "AUC-PR of a hand-computed curve")

            ' 完美分类器的两个面积都是 1
            Dim perfect As New RocPrCurve({0.9, 0.8, 0.2, 0.1}, {True, True, False, False})

            Call Support.Near(1.0, perfect.AucRoc(), 1.0E-12, "AUC-ROC of a perfect classifier")
            Call Support.Near(1.0, perfect.AucPr(), 1.0E-12, "AUC-PR of a perfect classifier")

            ' 排序完全反过来的分类器：负样本的得分全部高于正样本，两个面积都退化为 0
            Dim inverted As New RocPrCurve({0.4, 0.8, 0.35}, {True, False, True})

            Call Support.Near(0.0, inverted.AucRoc(), 1.0E-12, "AUC-ROC of a fully inverted classifier")
            Call Support.True(inverted.AucPr() < curve.AucPr(),
                              "the average precision of the inverted classifier must be lower")

            ' 灵敏度 50% 时的 PPV：在 TPR ≥ 0.5 的阈值里精确率最高的是 (FPR=0, TPR=0.5)，精确率为 1
            Call Support.Near(1.0, curve.PpvAtSensitivity(0.5), 1.0E-12, "PPV at 50% sensitivity")

            ' 灵敏度 100% 时的 PPV 只有两个候选点，取较大的 2/3
            Call Support.Near(2.0 / 3.0, curve.PpvAtSensitivity(1.0), 1.0E-12, "PPV at full sensitivity")

            ' 导出表格
            Dim table As String = curve.ToTable()

            Call Support.True(table.StartsWith("threshold"), "the exported table must contain a header")
        End Sub

        Private Sub CalibrationChecks()
            ' 四条样本，其中两条为正；全部落在最后一个分箱
            Dim curve As New RocPrCurve({0.95, 0.85, 0.75, 0.65}, {True, True, False, False})
            Dim bins As RocPrCurve.CalibrationBin() = curve.Calibration(2)

            Call Support.True(bins.Length = 1, "empty bins must be skipped in the calibration curve")

            Dim occupied As RocPrCurve.CalibrationBin = bins(0)

            Call Support.Same(4, occupied.Count, "all four samples fall into the upper bin")
            Call Support.Near(0.5, occupied.Observed, 1.0E-12, "the observed positive fraction of the upper bin")
            Call Support.Near(0.8, occupied.Predicted, 1.0E-12, "the mean predicted probability of the upper bin")
        End Sub

        Private Sub DistributionChecks()
            Dim p As Double() = {0.5, 0.5}
            Dim q As Double() = {0.5, 0.5}

            Call Support.Near(0.0, Metrics.KLDivergence(p, q), 1.0E-12, "KL divergence of identical distributions")
            Call Support.Near(0.0, Metrics.TotalVariation(p, q), 1.0E-12, "total variation of identical distributions")

            ' KL 散度不对称
            Dim left As Double() = {0.9, 0.1}
            Dim right As Double() = {0.5, 0.5}

            Call Support.True(Math.Abs(Metrics.KLDivergence(left, right) - Metrics.KLDivergence(right, left)) > 1.0E-06,
                              "the KL divergence must not be symmetric")
            Call Support.Near(0.4, Metrics.TotalVariation(left, right), 1.0E-12, "total variation of two point masses")

            ' 除以零的保护
            Call Support.Near(0.0, Metrics.SafeDivide(1.0, 0.0), 1.0E-12, "SafeDivide must return zero for a zero denominator")
        End Sub
    End Module
End Namespace
