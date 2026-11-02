Imports Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork

Namespace test.Tests

    ''' <summary>
    ''' 参数学习、结构学习与 EM 的数值对照。
    ''' </summary>
    Module Learning

        Public Sub Run()
            Call ParameterLearningChecks()
            Call MaximumLikelihoodChecks()
            Call ExpectationMaximizationChecks()
            Call StructureLearningChecks()
        End Sub

        Private Function Classic() As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork
            Return Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.CreateFrom(
                Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.TestData())
        End Function

        Private Function SampleData(network As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork,
                                    count As Integer,
                                    seed As Integer) As Integer()()

            Dim rng As New Random(seed)
            Dim data As Integer()() = New Integer(count - 1)() {}

            For i As Integer = 0 To count - 1
                data(i) = Sampling.AncestralSample(network, rng)
            Next

            Return data
        End Function

        Private Sub ParameterLearningChecks()
            Dim truth As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork = Classic()
            Dim data As Integer()() = SampleData(truth, 20000, 20261102)
            Dim learner As New ParameterLearner(1.0)
            Dim learned As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork = learner.Learn(truth, data)

            Call Support.Same(truth.Count, learned.Count, "learning must preserve the node count")
            Call Support.True(learned.Validate(), "the learned network must still be a valid network")

            For i As Integer = 0 To truth.Count - 1
                Dim node As BeliefNode = truth.Nodes(i)
                Dim estimate As BeliefNode = learned.Nodes(i)

                For r As Integer = 0 To node.RowCount - 1
                    For v As Integer = 0 To node.Range - 1
                        ' 两万条样本、加一平滑，误差应当远小于 0.05
                        Call Support.Near(node.CP_Table(r, v), estimate.CP_Table(r, v), 0.05,
                                          $"the learned CPT of node '{node.Name}' row {r} value {v} must match the truth")
                    Next
                Next
            Next

            ' 原网络不能被就地修改
            Call Support.Near(0.3, truth.Nodes(0).CP_Table(0, 0), 1.0E-12, "Learn must not modify the source network")

            ' 零计数配置不能退化为 0，而应当退化为先验对应的均匀分布
            Dim sparse As Integer()() = {New Integer() {0, 0, 0, 0}}
            Dim tiny As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork = learner.Learn(truth, sparse)

            Call Support.Near(0.5, tiny.Nodes(1).CP_Table(1, 0), 1.0E-09,
                              "an unseen parent configuration must fall back to the uniform prior")
        End Sub

        Private Sub MaximumLikelihoodChecks()
            ' 手工构造一个两值网络，用 4 条样本验证 MLE + 平滑的闭式结果
            Dim layout As New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout With {
                .Nodes = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode() {
                    New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode With {
                        .Name = "A",
                        .Range = 2,
                        .Parents = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.ParentList With {.ParentNodes = New String() {}},
                        .CPTable = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF With {
                            .CPColumns = {New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF.CPColumn With {.Data = {0.5}}}
                        }
                    }
                }
            }
            Dim network As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork =
                Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.CreateFrom(layout)
            Dim data As Integer()() = {New Integer() {0}, New Integer() {0}, New Integer() {0}, New Integer() {1}}

            Call New ParameterLearner(1.0).LearnInPlace(network, data)

            ' θ = (3 + 1) / (4 + 2) = 2/3
            Call Support.Near(2.0 / 3.0, network.Nodes(0).CP_Table(0, 0), 1.0E-12, "Laplace smoothing gives (n + 1)/(N + 2)")
            Call Support.Near(1.0 / 3.0, network.Nodes(0).CP_Table(0, 1), 1.0E-12, "the complementary probability")

            ' 对数似然：3 * ln(2/3) + 1 * ln(1/3)
            Dim expected As Double = 3.0 * Math.Log(2.0 / 3.0) + Math.Log(1.0 / 3.0)
            Call Support.Near(expected, ParameterLearner.LogLikelihood(network, data), 1.0E-10,
                              "the log likelihood of the smoothed estimate")

            ' 单样本概率
            Call Support.Near(Math.Log(2.0 / 3.0), ParameterLearner.LogProbability(network, {0}), 1.0E-12,
                              "LogProbability of a single sample")
        End Sub

        Private Sub ExpectationMaximizationChecks()
            Dim truth As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork = Classic()
            Dim data As Integer()() = SampleData(truth, 3000, 9241)
            Dim rain As Integer = truth.NodeIndex("Rain")

            ' 把 Rain 列全部置为缺失，模拟隐变量
            For Each row As Integer() In data
                row(rain) = BeliefNode.NotObserved
            Next

            Dim em As New ExpectationMaximization(truth.Clone()) With {
                .MaxIterations = 60,
                .Tolerance = 1.0E-08
            }
            Dim result = em.Run(data)

            Call Support.True(result.LogLikelihood.Length >= 2, "EM must run at least two iterations")
            Call Support.True(result.Network.Validate(), "the EM output must be a valid network")

            ' EM 保证对数似然单调不减；这里使用 Dirichlet 平滑的 MAP 更新，
            ' 目标函数是「似然 × 先验」而不是纯似然，因此允许极小的数值波动。
            Dim previous As Double = Double.NegativeInfinity
            Dim monotonic As Boolean = True

            For Each value As Double In result.LogLikelihood
                If value < previous - 1.0E-02 Then
                    monotonic = False
                End If

                previous = value
            Next

            Call Support.True(monotonic, "the EM log likelihood must be monotonically non-decreasing")

            ' 隐变量被积分掉之后，Cloudy 的边缘分布仍然应当接近真实值(P(Cloudy=0) = 0.3)
            Dim engine As New VariableElimination(result.Network)
            Dim marginal As Double() = engine.Marginal(New Integer() {-1, -1, -1, -1}, 0)

            Call Support.Near(0.3, marginal(0), 0.1, "the recovered prior of Cloudy must stay close to the truth")
        End Sub

        Private Sub StructureLearningChecks()
            ' 由链式网络 X0 -> X1 -> X2 生成数据
            Dim layout As New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout With {
                .Nodes = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode() {
                    New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode With {
                        .Name = "X0",
                        .Range = 2,
                        .Parents = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.ParentList With {.ParentNodes = New String() {}},
                        .CPTable = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF With {
                            .CPColumns = {New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF.CPColumn With {.Data = {0.5}}}
                        }
                    },
                    New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode With {
                        .Name = "X1",
                        .Range = 2,
                        .Parents = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.ParentList With {.ParentNodes = New String() {"X0"}},
                        .CPTable = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF With {
                            .CPColumns = {New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF.CPColumn With {.Data = {0.1, 0.9}}}
                        }
                    },
                    New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode With {
                        .Name = "X2",
                        .Range = 2,
                        .Parents = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.ParentList With {.ParentNodes = New String() {"X1"}},
                        .CPTable = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF With {
                            .CPColumns = {New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF.CPColumn With {.Data = {0.1, 0.9}}}
                        }
                    }
                }
            }
            Dim truth As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork =
                Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.CreateFrom(layout)
            Dim data As Integer()() = SampleData(truth, 5000, 31337)
            Dim learner As New StructureLearner(data, {2, 2, 2}) With {.MaxParents = 2}

            Dim chowLiu As Integer()() = learner.ChowLiu()
            Dim edges As Integer = 0

            For Each parents As Integer() In chowLiu
                edges += parents.Length
            Next

            Call Support.Same(2, edges, "a Chow-Liu tree over three variables must contain two edges")
            Call Support.True(chowLiu(1).Contains(0) OrElse chowLiu(2).Contains(1),
                              "the Chow-Liu tree must recover at least one edge of the generating chain")

            Dim climbed As Integer()() = learner.HillClimbing(chowLiu)
            Dim climbedScore As Double = learner.Bic(climbed)

            Call Support.True(climbedScore >= learner.Bic(chowLiu) - 1.0E-09,
                              "hill climbing must not decrease the BIC score")
            Dim empty0 As Integer() = New Integer() {}
            Dim empty1 As Integer() = New Integer() {}
            Dim empty2 As Integer() = New Integer() {}
            Dim emptyStructure As Integer()() = {empty0, empty1, empty2}

            Call Support.True(learner.Bic(chowLiu) > learner.Bic(emptyStructure),
                              "the learned structure must beat the empty structure")

            Dim learned As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork = learner.LearnNetwork()

            Call Support.Same(3, learned.Count, "the learned network must keep every variable")
            Call Support.True(learned.Validate(), "the learned network must be valid")

            ' 循环检测：0 -> 1 -> 2 的链
            Dim noParent As Integer() = New Integer() {}
            Dim chain As Integer()() = {noParent, New Integer() {0}, New Integer() {1}}

            Call Support.True(StructureLearner.IsAncestor(chain, 0, 2), "0 must be an ancestor of 2 in the chain")
            Call Support.True(Not StructureLearner.IsAncestor(chain, 2, 0), "2 must not be an ancestor of 0 in the chain")
        End Sub
    End Module
End Namespace
