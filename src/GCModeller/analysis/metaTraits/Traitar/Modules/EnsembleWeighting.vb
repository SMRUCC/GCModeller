' ============================================================================
' EnsembleWeighting.vb
'
' 用 K 折交叉验证分别评估每个基学习器，再把评估指标归一化为投票权重：
'   分类 -> 准确率
'   回归 -> R²（负值截断为 0）
' 全部指标都小于等于 0 时退化为等权，避免除零。
' ============================================================================

Imports System
Imports System.Linq
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping.Ensemble
Imports EnsembleCV = Microsoft.VisualBasic.MachineLearning.Bootstrapping.Ensemble.CrossValidation

Namespace Traitar.Modules

    ''' <summary>一次性返回原始评估指标与归一化权重</summary>
    Public Class EnsembleWeightResult

        ''' <summary>交叉验证原始指标（分类准确率 / 回归 R²）</summary>
        Public Property Scores As Double()
        ''' <summary>归一化之后的投票权重，与 <see cref="Scores"/> 顺序对齐</summary>
        Public Property Weights As Double()

    End Class

    ''' <summary>集成学习基模型的交叉验证加权</summary>
    Public Module EnsembleWeighting

        ''' <summary>
        ''' 用分层 K 折交叉验证估计每个分类基学习器的准确率，并归一化为权重。
        ''' </summary>
        ''' <param name="learners">分类基学习器</param>
        ''' <param name="x">0-based 稠密特征矩阵</param>
        ''' <param name="yIndex">0-based 类别索引</param>
        ''' <param name="classes">类别标签值（0..K-1）</param>
        ''' <param name="folds">期望折数，内部会按样本数自适应</param>
        ''' <param name="seed">随机种子</param>
        Public Function EstimateClassifierWeights(learners As IClassifierLearner(),
                                                  x As Double()(),
                                                  yIndex As Integer(),
                                                  classes As Double(),
                                                  folds As Integer,
                                                  Optional seed As Integer = 42) As EnsembleWeightResult

            Dim scores As Double() = New Double(learners.Length - 1) {}

            If x Is Nothing OrElse x.Length = 0 Then
                Return EqualWeights(scores)
            End If

            Dim n As Integer = x.Length
            Dim foldCount As Integer = Math.Max(2, Math.Min(Math.Max(folds, 2), Math.Max(2, n \ 2)))
            Dim testFolds As Integer()() = EnsembleCV.StratifiedKFold(yIndex, foldCount, seed)

            For l As Integer = 0 To learners.Length - 1
                Dim correct As Integer = 0
                Dim total As Integer = 0

                For Each testIndex As Integer() In testFolds
                    Dim trainIndex As Integer() = EnsembleCV.Complement(n, testIndex)
                    Dim model As IClassifier = Nothing

                    Try
                        model = learners(l).Fit(
                            BootstrapSampler.Resample(x, trainIndex),
                            BootstrapSampler.Resample(yIndex, trainIndex),
                            classes)
                    Catch ex As Exception
                        ' 单个折训练失败（例如类别缺失）不应中断整个加权过程
                        Continue For
                    End Try

                    For Each i As Integer In testIndex
                        Dim predicted As Double = ProbabilityMath.PredictClass(model, x(i))

                        If predicted = classes(yIndex(i)) Then
                            correct += 1
                        End If

                        total += 1
                    Next
                Next

                scores(l) = If(total > 0, correct / total, 0.0R)
            Next

            Return CreateResult(scores)
        End Function

        ''' <summary>
        ''' 用 K 折交叉验证估计每个回归基学习器的 R²，并归一化为权重。
        ''' </summary>
        ''' <param name="learners">回归基学习器</param>
        ''' <param name="x">0-based 稠密特征矩阵</param>
        ''' <param name="y">连续目标值</param>
        ''' <param name="folds">期望折数，内部会按样本数自适应</param>
        ''' <param name="seed">随机种子</param>
        Public Function EstimateRegressorWeights(learners As IRegressorLearner(),
                                                 x As Double()(),
                                                 y As Double(),
                                                 folds As Integer,
                                                 Optional seed As Integer = 42) As EnsembleWeightResult

            Dim scores As Double() = New Double(learners.Length - 1) {}

            If x Is Nothing OrElse x.Length = 0 Then
                Return EqualWeights(scores)
            End If

            Dim n As Integer = x.Length
            Dim foldCount As Integer = Math.Max(2, Math.Min(Math.Max(folds, 2), Math.Max(2, n \ 2)))
            Dim testFolds As Integer()() = EnsembleCV.KFold(n, foldCount, seed)
            Dim mean As Double = y.Average()
            Dim sst As Double = y.Sum(Function(v) (v - mean) ^ 2)

            For l As Integer = 0 To learners.Length - 1
                Dim sumSquared As Double = 0
                Dim total As Integer = 0

                For Each testIndex As Integer() In testFolds
                    Dim trainIndex As Integer() = EnsembleCV.Complement(n, testIndex)
                    Dim model As IRegressor = Nothing

                    Try
                        model = learners(l).Fit(
                            BootstrapSampler.Resample(x, trainIndex),
                            BootstrapSampler.Resample(y, trainIndex))
                    Catch ex As Exception
                        Continue For
                    End Try

                    For Each i As Integer In testIndex
                        Dim predicted As Double = model.Predict(x(i))

                        If Double.IsNaN(predicted) OrElse Double.IsInfinity(predicted) Then
                            Continue For
                        End If

                        sumSquared += (predicted - y(i)) ^ 2
                        total += 1
                    Next
                Next

                If total > 0 AndAlso sst > 0 Then
                    scores(l) = Math.Max(0.0R, 1.0R - sumSquared / sst)
                Else
                    scores(l) = 0.0R
                End If
            Next

            Return CreateResult(scores)
        End Function

        ''' <summary>把一组指标归一化为权重；全为 0 时等权</summary>
        Public Function CreateResult(scores As Double()) As EnsembleWeightResult
            If scores Is Nothing OrElse scores.Length = 0 Then
                Return New EnsembleWeightResult With {
                    .Scores = New Double() {},
                    .Weights = New Double() {}
                }
            End If

            Dim weights As Double()

            If scores.All(Function(v) v <= 0.0R) Then
                weights = Enumerable.Repeat(1.0R / scores.Length, scores.Length).ToArray()
            Else
                weights = ProbabilityMath.NormalizeWeights(scores, scores.Length)
            End If

            Return New EnsembleWeightResult With {
                .Scores = CType(scores.Clone(), Double()),
                .Weights = weights
            }
        End Function

        Private Function EqualWeights(scores As Double()) As EnsembleWeightResult
            If scores.Length = 0 Then
                Return CreateResult(scores)
            End If

            Return New EnsembleWeightResult With {
                .Scores = CType(scores.Clone(), Double()),
                .Weights = Enumerable.Repeat(1.0R / scores.Length, scores.Length).ToArray()
            }
        End Function

    End Module
End Namespace
