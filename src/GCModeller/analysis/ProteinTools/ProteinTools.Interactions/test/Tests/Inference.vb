Imports Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork

Namespace test.Tests

    ''' <summary>
    ''' 贝叶斯信念网络推断的数值对照。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 主要基准是一个独立实现的<b>穷举推断</b>：枚举全部变量的取值组合，
    ''' 按条件概率表计算联合概率，再求出目标变量的条件分布。这个实现与
    ''' 变量消除、团树、置信传播在结构上完全不同，因此构成有效的交叉验证。
    ''' </para>
    ''' <para>
    ''' 另外还使用两个手算的解析值，以及针对旧版本「证据被覆盖」缺陷的回归测试。
    ''' </para>
    ''' </remarks>
    Module Inference

        Public Sub Run()
            Call TwoNodeAnalyticChecks()
            Call ClassicNetworkChecks()
            Call EvidenceMergeRegression()
            Call CrossEngineAgreement()
            Call MultiValuedChecks()
            Call SerializationChecks()
        End Sub

#Region "Reference implementation"

        ''' <summary>
        ''' 穷举全部变量取值之后的联合概率
        ''' </summary>
        Private Function JointProbability(network As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork,
                                          values As Integer()) As Double
            Dim p As Double = 1.0

            For i As Integer = 0 To network.Count - 1
                Dim node As BeliefNode = network.Nodes(i)
                Dim parentValues As Integer() = node.Parents.Select(Function(x) values(x.Id)).ToArray()

                p *= node.CP_Table(node.RowIndex(parentValues), values(i))
            Next

            Return p
        End Function

        ''' <summary>
        ''' 用穷举法计算 <c>P(targets | evidence)</c>
        ''' </summary>
        Private Function ReferenceQuery(network As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork,
                                        evidence As Integer(),
                                        targets As Integer()) As Double()

            Dim sortedTargets As Integer() = targets.Distinct().OrderBy(Function(v) v).ToArray()
            Dim cards As Integer() = network.Nodes.Select(Function(n) n.Range).ToArray()
            Dim strides As Integer() = Factor.ComputeStrides(cards)
            Dim total As Integer = strides(cards.Length - 1) * cards(cards.Length - 1)
            Dim targetCards As Integer() = sortedTargets.Select(Function(v) cards(v)).ToArray()
            Dim targetStrides As Integer() = Factor.ComputeStrides(targetCards)
            Dim targetTotal As Integer = If(targetCards.Length = 0, 1, targetStrides(targetCards.Length - 1) * targetCards(targetCards.Length - 1))
            Dim raw As Double() = New Double(targetTotal - 1) {}
            Dim values As Integer() = New Integer(cards.Length - 1) {}
            Dim targetValues As Integer() = New Integer(sortedTargets.Length - 1) {}

            For code As Integer = 0 To total - 1
                Call Factor.Decode(code, cards, strides, values)

                Dim consistent As Boolean = True

                If evidence IsNot Nothing Then
                    For i As Integer = 0 To Math.Min(evidence.Length, values.Length) - 1
                        If evidence(i) <> BeliefNode.NotObserved AndAlso evidence(i) <> values(i) Then
                            consistent = False
                            Exit For
                        End If
                    Next
                End If

                If Not consistent Then
                    Continue For
                End If

                For k As Integer = 0 To sortedTargets.Length - 1
                    targetValues(k) = values(sortedTargets(k))
                Next

                raw(Factor.Encode(targetValues, targetStrides)) += JointProbability(network, values)
            Next

            Return NumericLogSpace.NormalizeLog(NumericLogSpace.Log(raw))
        End Function

#End Region

#Region "Layout builders"

        Private Function Layout(nodes As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode()) _
            As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout

            Return New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout With {.Nodes = nodes}
        End Function

        ''' <summary>
        ''' 构造一个节点。<paramref name="data"/> 按照<b>列优先</b>排列：
        ''' 先给出各个父配置下取值为 0 的概率，再给出取值为 1 的概率，依此类推。
        ''' </summary>
        Private Function Node(name As String, range As Integer, parents As String(), data As Double()) _
            As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode

            Dim rows As Integer = data.Length \ (range - 1)
            Dim columns As New List(Of Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF.CPColumn)

            For k As Integer = 0 To range - 2
                Dim values As Double() = New Double(rows - 1) {}

                For r As Integer = 0 To rows - 1
                    values(r) = data(r * (range - 1) + k)
                Next

                Call columns.Add(New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF.CPColumn With {.Data = values})
            Next

            Return New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode With {
                .Name = name,
                .Range = range,
                .Parents = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.ParentList With {.ParentNodes = parents},
                .CPTable = New Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.BeliefNode.CPTableF With {.CPColumns = columns.ToArray()}
            }
        End Function

        Private Function Classic() As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork
            Return Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.CreateFrom(
                Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.NetworkLayout.TestData())
        End Function

#End Region

        Private Sub TwoNodeAnalyticChecks()
            ' A: P(A=1) = 0.3；B|A: P(B=1|A=0)=0.5、P(B=1|A=1)=0.9
            Dim network As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork =
                Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.CreateFrom(Layout({
                    Node("A", 2, New String() {}, {0.3}),
                    Node("B", 2, New String() {"A"}, {0.5, 0.9})
                }))

            Call Support.True(network.Validate(), "the hand-built two node network must be valid")

            Dim engine As New VariableElimination(network)
            Dim marginal As Double() = engine.Query(New Integer() {-1, -1}, {1})

            ' 条件概率表的第 0 列对应「取值为 0」的概率，因此 P(A=0) = 0.3、P(B=0|A=0) = 0.5、P(B=0|A=1) = 0.9
            ' P(B=0) = 0.3*0.5 + 0.7*0.9 = 0.78
            Call Support.Near(0.78, marginal(0), 1.0E-12, "P(B=0) of the analytic two node network")
            Call Support.Near(0.22, marginal(1), 1.0E-12, "P(B=1) of the analytic two node network")

            Dim elim As New BElim(network)

            ' P(A=1, B=1) = 0.7 * 0.1 = 0.07
            Call Support.Near(0.07, elim.GetBelief({1, 1}, {-1, -1}), 1.0E-12, "P(A=1, B=1)")

            ' P(A=1 | B=1) = 0.07 / 0.22
            Call Support.Near(0.07 / 0.22, elim.GetBelief({1, -1}, {-1, 1}), 1.0E-12, "P(A=1 | B=1)")

            ' 没有查询变量时按照定义返回 1
            Call Support.Near(1.0, elim.GetBelief({-1, -1}, {-1, 1}), 1.0E-12, "GetBelief without query variables")
        End Sub

        Private Sub ClassicNetworkChecks()
            Dim network As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork = Classic()

            Call Support.Same(4, network.Count, "the classic network must have four nodes")
            Call Support.True(network.Validate(), "the classic network must be valid")

            Dim wetGrass As Integer = network.NodeIndex("WetGrass")
            Dim rain As Integer = network.NodeIndex("Rain")
            Dim sprinkler As Integer = network.NodeIndex("Sprinkler")
            Dim engine As New VariableElimination(network)
            Dim none As Integer() = {-1, -1, -1, -1}

            Call Support.Near(
                ReferenceQuery(network, Nothing, {wetGrass})(1),
                engine.Query(none, {wetGrass})(1),
                1.0E-10,
                "P(WetGrass=1) variable elimination versus enumeration")

            Dim evidence As Integer() = {-1, -1, -1, 1}

            Call Support.Near(
                ReferenceQuery(network, evidence, {rain})(1),
                engine.Query(evidence, {rain})(1),
                1.0E-10,
                "P(Rain=1 | WetGrass=1)")

            Call Support.Near(
                ReferenceQuery(network, evidence, {sprinkler})(1),
                engine.Query(evidence, {sprinkler})(1),
                1.0E-10,
                "P(Sprinkler=1 | WetGrass=1)")

            Call Support.Near(
                ReferenceQuery(network, evidence, {1, 2})(0),
                engine.Query(evidence, {1, 2})(0),
                1.0E-10,
                "P(Rain=0, Sprinkler=0 | WetGrass=1)")
        End Sub

        Private Sub EvidenceMergeRegression()
            ' 旧实现在这里会把条件证据覆盖成 -1，得到的是 P(Rain=1) 而不是 P(Rain=1 | Cloudy=1, WetGrass=1)
            Dim network As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork = Classic()
            Dim cloudy As Integer = network.NodeIndex("Cloudy")
            Dim rain As Integer = network.NodeIndex("Rain")
            Dim wetGrass As Integer = network.NodeIndex("WetGrass")
            Dim evidence As Integer() = New Integer() {-1, -1, -1, -1}
            Dim query As Integer() = New Integer() {-1, -1, -1, -1}

            evidence(wetGrass) = 1
            evidence(cloudy) = 1
            query(rain) = 1

            Dim elim As New BElim(network)
            Dim actual As Double = elim.GetBelief(query, evidence)
            Dim expected As Double = ReferenceQuery(network, evidence, {rain})(1)

            Call Support.Near(expected, actual, 1.0E-10, "GetBelief must keep the conditional evidence")

            Dim unconditional As Double = elim.GetBelief(query, {-1, -1, -1, -1})

            Call Support.True(
                Math.Abs(expected - unconditional) > 1.0E-06,
                "the conditional and unconditional posterior must differ (regression guard for the evidence merge bug)")

            Dim engine As New VariableElimination(network)
            Call Support.Near(engine.Probability(evidence, rain, 1), actual, 1.0E-10, "BElim versus variable elimination")
        End Sub

        Private Sub CrossEngineAgreement()
            Dim network As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork = Classic()
            Dim tree As New JunctionTree(network)
            Dim engine As New VariableElimination(network)
            Dim bp As New LoopyBeliefPropagation(network)
            Dim evidence As Integer() = {-1, -1, -1, 1}

            For i As Integer = 0 To network.Count - 1
                Dim expected As Double() = engine.Marginal(evidence, i)
                Dim fromTree As Double() = tree.Marginal(evidence, i)
                Dim fromBp As Double() = bp.Run(evidence, 500, 1.0E-12, 1.0).Marginals(i)

                For v As Integer = 0 To expected.Length - 1
                    Call Support.Near(expected(v), fromTree(v), 1.0E-08,
                                      $"junction tree versus variable elimination on node {i} value {v}")

                    ' Cloudy-Sprinkler-WetGrass-Rain 构成一条无向环，因此这是一个 loopy 网络，
                    ' BP 只能给出 Bethe 近似，允许千分之一量级以上的偏差。
                    Call Support.Near(expected(v), fromBp(v), 0.1,
                                      $"loopy BP versus variable elimination on node {i} value {v}")
                Next
            Next

            Call Support.True(tree.Cliques.Length > 0, "the junction tree must contain at least one clique")

            Call Support.Near(
                engine.LogPartition(evidence),
                tree.LogPartition(evidence),
                1.0E-08,
                "junction tree versus variable elimination partition function")

            Call Support.True(
                bp.Run(evidence, 500, 1.0E-12, 1.0).Converged,
                "loopy BP must converge on the classic network")
        End Sub

        Private Sub MultiValuedChecks()
            ' 三值链式网络：A(3) -> B(3) -> C(2)
            Dim network As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork =
                Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.CreateFrom(Layout({
                    Node("A", 3, New String() {}, {0.3, 0.5}),
                    Node("B", 3, New String() {"A"}, {0.2, 0.5, 0.3, 0.1, 0.6, 0.1}),
                    Node("C", 2, New String() {"B"}, {0.9, 0.4, 0.05})
                }))

            Call Support.True(network.Validate(), "the multi-valued chain must be valid")

            Dim engine As New VariableElimination(network)
            Dim elimination As New BElim(network)
            Dim none As Integer() = {-1, -1, -1}

            For Each node As Integer In {0, 1, 2}
                Dim expected As Double() = ReferenceQuery(network, Nothing, {node})
                Dim actual As Double() = engine.Marginal(none, node)

                For v As Integer = 0 To expected.Length - 1
                    Call Support.Near(expected(v), actual(v), 1.0E-10, $"multi-valued marginal of node {node} value {v}")
                Next
            Next

            Dim evidence As Integer() = {-1, -1, 1}
            Dim posterior As Double() = engine.Marginal(evidence, 1)
            Dim reference As Double() = ReferenceQuery(network, evidence, {1})

            Call Support.Same(3, posterior.Length, "the posterior of a three-valued node must have three entries")
            Call Support.Near(1.0, posterior.Sum(), 1.0E-10, "the posterior must be normalized")

            For v As Integer = 0 To 2
                Call Support.Near(reference(v), posterior(v), 1.0E-10, $"three-valued posterior entry {v}")
            Next

            ' 旧实现只会枚举 0 与 1，两个取值时结果是均匀的；这里必须不是均匀分布
            Dim uniform As Boolean = True

            For v As Integer = 1 To 2
                If Math.Abs(posterior(v) - posterior(0)) > 1.0E-06 Then
                    uniform = False
                End If
            Next

            Call Support.True(Not uniform, "the three-valued posterior must not degenerate into a uniform distribution")

            ' 同时给定两个条件变量的证据
            Dim both As Integer() = {-1, 1, 1}

            Call Support.Near(
                ReferenceQuery(network, both, {0})(1),
                elimination.Marginal(both, 0)(1),
                1.0E-10,
                "multi-valued evidence merge")
        End Sub

        Private Sub SerializationChecks()
            Dim network As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork = Classic()
            Dim round As Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork =
                Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.CreateFrom(network.ToLayout())

            Call Support.Same(network.Count, round.Count, "the round-tripped network must keep the node count")

            For i As Integer = 0 To network.Count - 1
                Call Support.True(String.Equals(network.Nodes(i).Name, round.Nodes(i).Name),
                                  $"node {i} name must survive the round trip")
                Call Support.Same(network.Nodes(i).Range, round.Nodes(i).Range, $"node {i} range must survive the round trip")
                Call Support.Same(network.Nodes(i).Parents.Count, round.Nodes(i).Parents.Count,
                                  $"node {i} parents must survive the round trip")

                For r As Integer = 0 To network.Nodes(i).RowCount - 1
                    For v As Integer = 0 To network.Nodes(i).Range - 1
                        Call Support.Near(network.Nodes(i).CP_Table(r, v), round.Nodes(i).CP_Table(r, v), 1.0E-12,
                                          $"the CPT of node {i} must survive the round trip")
                    Next
                Next
            Next

            ' 条件概率表的列数与节点取值范围不一致时必须被拒绝
            Call Support.Throws(Of ArgumentException)(
                Sub() Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.CreateFrom(
                    Layout({Node("X", 3, New String() {}, {0.3})})),
                "a CPT column count that does not match the node range must be rejected")

            ' 父节点未定义(拓扑序错误或者存在环)时必须被拒绝
            Call Support.Throws(Of ArgumentException)(
                Sub() Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork.BeliefNetwork.CreateFrom(
                    Layout({Node("Y", 2, New String() {"Z"}, {0.5})})),
                "an undefined parent must be rejected")
        End Sub
    End Module
End Namespace
