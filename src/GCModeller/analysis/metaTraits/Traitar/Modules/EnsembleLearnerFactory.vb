' ============================================================================
' EnsembleLearnerFactory.vb
'
' 按表型的数据类型与类别数量动态组合基学习器：
'
'   boolean / 二分类 categorical -> 朴素贝叶斯 + SVM + 逻辑回归 + XGBoost
'   多分类 categorical           -> 朴素贝叶斯 + SVM
'   numeric                      -> 随机森林 + 岭回归 + MLR + 贝叶斯回归 + SVR + XGBoost
'
' 逻辑回归与 XGBoost 的分类适配器只支持二分类，随机森林没有分类适配器，
' 因此必须在工厂层面按 classCount 守卫。
' ============================================================================

Imports System.Collections.Generic
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping.Ensemble

Namespace Traitar.Modules

    ''' <summary>参与集成的基学习器算法</summary>
    Public Enum EnsembleAlgorithm

        NaiveBayes = 0
        Logistic = 1
        Svm = 2
        XGBoost = 3
        RandomForest = 4
        BayesianRegression = 5
        Ridge = 6
        Mlr = 7
        Lasso = 8

    End Enum

    ''' <summary>
    ''' 一个基学习器的完整描述：算法标识、学习器实例与超参数（超参会写入模型元数据）。
    ''' </summary>
    Public Class EnsembleLearnerSpec

        Public Property Algorithm As EnsembleAlgorithm
        ''' <summary>算法名称，作为持久化 DTO 的 algorithm 字段</summary>
        Public Property Name As String
        ''' <summary>分类学习器（<see cref="Regressor"/> 为空时有效）</summary>
        Public Property Classifier As IClassifierLearner
        ''' <summary>回归学习器（<see cref="Classifier"/> 为空时有效）</summary>
        Public Property Regressor As IRegressorLearner
        ''' <summary>本次训练使用的超参数快照</summary>
        Public Property Parameters As Dictionary(Of String, Double)

        Public ReadOnly Property IsClassifier As Boolean
            Get
                Return Classifier IsNot Nothing
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Name} ({If(IsClassifier, "classifier", "regressor")})"
        End Function

    End Class

    ''' <summary>按表型类型组合基学习器</summary>
    Public Module EnsembleLearnerFactory

        ''' <summary>
        ''' 分类基学习器组合。
        ''' </summary>
        ''' <param name="classCount">类别数量；等于 2 时追加逻辑回归与 XGBoost</param>
        ''' <param name="dims">特征维度，用于推导 SVM 的默认 gamma</param>
        ''' <param name="compact">精简模式：降低 XGBoost 轮数，便于小样本快速评估</param>
        Public Function CreateClassifiers(classCount As Integer,
                                          Optional dims As Integer = 0,
                                          Optional compact As Boolean = False) As EnsembleLearnerSpec()

            Dim list As New List(Of EnsembleLearnerSpec)()
            Dim gamma As Double = If(dims > 0, 1.0R / dims, 0.5R)

            Call list.Add(New EnsembleLearnerSpec With {
                .Algorithm = EnsembleAlgorithm.NaiveBayes,
                .Name = "naive-bayes",
                .Classifier = New NaiveBayesClassifierLearner With {.Smoothing = 1.0R},
                .Parameters = New Dictionary(Of String, Double) From {{"smoothing", 1.0R}}
            })
            Call list.Add(New EnsembleLearnerSpec With {
                .Algorithm = EnsembleAlgorithm.Svm,
                .Name = "svm",
                .Classifier = New ScaledSvmClassifierLearner With {
                    .C = 1.0R,
                    .Gamma = gamma,
                    .FeatureOffset = 1
                },
                .Parameters = New Dictionary(Of String, Double) From {
                    {"C", 1.0R},
                    {"gamma", gamma},
                    {"featureOffset", 1.0R}
                }
            })

            If classCount = 2 Then
                Dim rounds As Integer = If(compact, 10, 30)
                Dim iterations As Integer = If(compact, 500, 2000)

                Call list.Add(New EnsembleLearnerSpec With {
                    .Algorithm = EnsembleAlgorithm.Logistic,
                    .Name = "logistic",
                    .Classifier = New LogisticClassifierLearner With {
                        .Rate = 0.1R,
                        .Iterations = iterations,
                        .Tolerance = 0.0000001R
                    },
                    .Parameters = New Dictionary(Of String, Double) From {
                        {"rate", 0.1R},
                        {"iterations", iterations}
                    }
                })
                Call list.Add(New EnsembleLearnerSpec With {
                    .Algorithm = EnsembleAlgorithm.XGBoost,
                    .Name = "xgboost",
                    .Classifier = New XGBoostClassifierLearner With {
                        .Rounds = rounds,
                        .MaxDepth = 6,
                        .Seed = 42
                    },
                    .Parameters = New Dictionary(Of String, Double) From {
                        {"rounds", rounds},
                        {"maxDepth", 6.0R},
                        {"seed", 42.0R}
                    }
                })
            End If

            Return list.ToArray()
        End Function

        ''' <summary>
        ''' 回归基学习器组合。
        ''' </summary>
        ''' <param name="dims">特征维度，用于推导 SVM 的默认 gamma</param>
        ''' <param name="compact">精简模式：减少随机森林树数与 XGBoost 轮数</param>
        Public Function CreateRegressors(Optional dims As Integer = 0,
                                         Optional compact As Boolean = False) As EnsembleLearnerSpec()

            Dim list As New List(Of EnsembleLearnerSpec)()
            Dim gamma As Double = If(dims > 0, 1.0R / dims, 0.5R)
            Dim trees As Integer = If(compact, 30, 100)
            Dim rounds As Integer = If(compact, 10, 30)

            Call list.Add(New EnsembleLearnerSpec With {
                .Algorithm = EnsembleAlgorithm.RandomForest,
                .Name = "random-forest",
                .Regressor = New RandomForestRegressorLearner With {
                    .TreeCount = trees,
                    .MaxBranch = 2000,
                    .Mtry = 0,
                    .UseGini = False
                },
                .Parameters = New Dictionary(Of String, Double) From {
                    {"treeCount", trees},
                    {"maxBranch", 2000.0R}
                }
            })
            Call list.Add(New EnsembleLearnerSpec With {
                .Algorithm = EnsembleAlgorithm.Ridge,
                .Name = "ridge",
                .Regressor = New RidgeRegressorLearner With {
                    .NumberOfLambdas = 100,
                    .Standardize = True
                },
                .Parameters = New Dictionary(Of String, Double) From {
                    {"numberOfLambdas", 100.0R},
                    {"standardize", 1.0R}
                }
            })
            Call list.Add(New EnsembleLearnerSpec With {
                .Algorithm = EnsembleAlgorithm.Mlr,
                .Name = "mlr",
                .Regressor = New MlrRegressorLearner(),
                .Parameters = New Dictionary(Of String, Double)()
            })
            Call list.Add(New EnsembleLearnerSpec With {
                .Algorithm = EnsembleAlgorithm.BayesianRegression,
                .Name = "bayesian-regression",
                .Regressor = New BayesianRegressorLearner With {
                    .Bins = 10,
                    .Smoothing = 1.0R,
                    .VarianceFloor = 0.000000001R
                },
                .Parameters = New Dictionary(Of String, Double) From {
                    {"bins", 10.0R},
                    {"smoothing", 1.0R}
                }
            })
            Call list.Add(New EnsembleLearnerSpec With {
                .Algorithm = EnsembleAlgorithm.Svm,
                .Name = "svr",
                .Regressor = New ScaledSvmRegressorLearner With {
                    .C = 1.0R,
                    .Gamma = gamma,
                    .Epsilon = 0.1R,
                    .FeatureOffset = 1
                },
                .Parameters = New Dictionary(Of String, Double) From {
                    {"C", 1.0R},
                    {"gamma", gamma},
                    {"epsilon", 0.1R}
                }
            })
            Call list.Add(New EnsembleLearnerSpec With {
                .Algorithm = EnsembleAlgorithm.XGBoost,
                .Name = "xgboost",
                .Regressor = New XGBoostRegressorLearner With {
                    .Rounds = rounds,
                    .MaxDepth = 6,
                    .Seed = 42
                },
                .Parameters = New Dictionary(Of String, Double) From {
                    {"rounds", rounds},
                    {"maxDepth", 6.0R},
                    {"seed", 42.0R}
                }
            })

            Return list.ToArray()
        End Function

    End Module
End Namespace
