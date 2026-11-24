Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Model
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Tree

Namespace test.Tests

    ''' <summary>
    ''' PPI 模型与静态打分的测试。
    ''' </summary>
    Module Ppi

        ''' <summary>
        ''' 构造一个第 0 列跨家族完全耦合的联合比对
        ''' </summary>
        Public Function CoupledAlignment(rows As Integer, lengthA As Integer, lengthB As Integer, seed As ULong) As JointAlignment
            Dim alphabet As ResidueAlphabet = ResidueAlphabet.Standard21
            Dim rng As New Fixtures.Lcg(seed)
            Dim data As Integer(,) = New Integer(rows - 1, lengthA + lengthB - 1) {}
            Dim names As String() = New String(rows - 1) {}

            For r As Integer = 0 To rows - 1
                names(r) = "genome_" & r.ToString("000")
                Dim coupled As Integer = rng.Next(6)

                data(r, 0) = coupled
                data(r, lengthA) = coupled

                For c As Integer = 1 To lengthA - 1
                    data(r, c) = rng.Next(4)
                Next

                For c As Integer = lengthA + 1 To lengthA + lengthB - 1
                    data(r, c) = rng.Next(4)
                Next
            Next

            Return New JointAlignment(data, names, lengthA, lengthB, alphabet)
        End Function

        Public Sub Run()
            Call ModelChecks()
            Call ScoringChecks()
            Call ContactChecks()
            Call OrphanDecompositionChecks()
            Call InterologChecks()
        End Sub

        Private Sub ModelChecks()
            Dim alignment As JointAlignment = CoupledAlignment(60, 4, 4, 271828UL)
            Dim model As New PpiBayesianModel(alignment)

            Call Support.True(model.Validate(), "the Bayesian model must pass its own consistency checks")
            Call Support.Same(alignment.Length - 1, model.FullTree.Edges.Length, "the dependency tree must span every column")
            Call Support.Same(alignment.LengthA - 1, model.BlockATree.Edges.Length, "T_k must span the family A columns")
            Call Support.Same(alignment.LengthB - 1, model.BlockBTree.Edges.Length, "T_r must span the family B columns")

            Dim maxTree As Double = model.LogEvidence()
            Dim exact As Double = model.LogDetEvidence()

            Call Support.True(Double.IsNaN(maxTree) = False, "the maximum spanning tree evidence must be finite")
            Call Support.True(Double.IsNaN(exact) = False, "the matrix-tree evidence must be finite")

            ' 对全部生成树求和的结果不小于任何单一一棵树的贡献
            Call Support.True(exact >= maxTree - 1.0E-06,
                              $"the matrix-tree sum must dominate the single best tree ({exact:G6} vs {maxTree:G6})")

            ' 耦合的那一列必然是树上的边
            Dim found As Boolean = False

            For Each edge As DependencyTree.Edge In model.FullTree.Edges
                If (edge.I = 0 AndAlso edge.J = alignment.LengthA) OrElse (edge.J = 0 AndAlso edge.I = alignment.LengthA) Then
                    found = True
                End If
            Next

            Call Support.True(found, "the coupled A0/B0 column pair must appear in the dependency tree")

            ' 交换伙伴会破坏耦合，因此该列对的似然比必须下降
            Dim edgeBefore As Double = model.LogR.LogR(0, alignment.LengthA)
            Dim evidenceBefore As Double = model.LogEvidence()

            Call model.ApplySwap(0, 1)

            Dim edgeAfter As Double = model.LogR.LogR(0, alignment.LengthA)

            Call Support.True(edgeBefore > 0, "the coupled column pair must have a positive log ratio")
            Call Support.True(edgeAfter < edgeBefore,
                              $"breaking the coupled pairing must lower the log ratio ({edgeAfter:G6} < {edgeBefore:G6})")
            Call Support.True(Double.IsNaN(model.LogEvidence()) = False, "the evidence must stay finite after a swap")
            Call Support.True(model.Validate(), "the model must remain consistent after a partner swap")
        End Sub

        Private Sub ScoringChecks()
            Dim alignment As JointAlignment = CoupledAlignment(40, 3, 3, 1234UL)
            Dim model As New PpiBayesianModel(alignment)
            Dim scoring As New PpiScoring(model)
            Dim codesA As Integer() = alignment.Row(0).Take(alignment.LengthA).ToArray()
            Dim codesB As Integer() = alignment.Row(0).Skip(alignment.LengthA).ToArray()
            Dim detail As PpiScoring.ScoreDetail = scoring.ScoreDetailed(codesA, codesB)

            Call Support.Same(alignment.Length - 1, detail.Contributions.Length, "every tree edge must contribute to the score")
            Call Support.Near(detail.Score, scoring.Score(codesA, codesB), 1.0E-12, "Score must delegate to ScoreDetailed")

            ' 用逐边公式独立重算一遍
            Dim manual As Double = 0.0
            Dim prior As DirichletMultinomial = model.LogR.Prior
            Dim k As Integer = model.Counts.AlphabetSize

            For Each edge As DependencyTree.Edge In model.FullTree.Edges
                Dim columnA As Integer
                Dim columnB As Integer
                Dim left As Integer
                Dim right As Integer

                If alignment.IsBlockA(edge.I) AndAlso Not alignment.IsBlockA(edge.J) Then
                    columnA = edge.I
                    columnB = edge.J
                    left = codesA(edge.I)
                    right = codesB(edge.J - alignment.LengthA)
                ElseIf alignment.IsBlockA(edge.J) AndAlso Not alignment.IsBlockA(edge.I) Then
                    columnA = edge.J
                    columnB = edge.I
                    left = codesA(edge.J)
                    right = codesB(edge.I - alignment.LengthA)
                ElseIf alignment.IsBlockA(edge.I) Then
                    columnA = edge.I
                    columnB = edge.J
                    left = codesA(edge.I)
                    right = codesA(edge.J)
                Else
                    columnA = edge.I
                    columnB = edge.J
                    left = codesB(edge.I - alignment.LengthA)
                    right = codesB(edge.J - alignment.LengthA)
                End If

                Dim joint As Dictionary(Of Integer, Integer) = model.Counts.PairCounts(columnA, columnB)
                Dim pairTotal As Double = model.Counts.PairTotal(columnA, columnB)
                Dim n As Integer = 0

                Call joint.TryGetValue(left * k + right, n)

                Dim pij As Double = (n + prior.Lambda) / (pairTotal + prior.Lambda * k * k)
                Dim pi As Double = (model.Counts.SingleCounts(columnA)(left) + prior.LambdaSingle) /
                    (model.Counts.SingleTotal(columnA) + prior.LambdaSingle * k)
                Dim pj As Double = (model.Counts.SingleCounts(columnB)(right) + prior.LambdaSingle) /
                    (model.Counts.SingleTotal(columnB) + prior.LambdaSingle * k)

                manual += Math.Log(pij / (pi * pj))
            Next

            Call Support.Near(manual, detail.Score, 1.0E-10, "the score must equal the sum over the per-edge log ratios")

            ' 逐边贡献之和
            Dim sum As Double = 0.0

            For Each contribution As PpiScoring.EdgeContribution In detail.Contributions
                sum += contribution.LogRatio
            Next

            Call Support.Near(detail.Score, sum, 1.0E-12, "the contributions must add up to the total score")

            ' 阈值判定
            Call Support.True(scoring.IsInteraction(1.0), "a score equal to the threshold must be accepted")
            Call Support.True(Not scoring.IsInteraction(0.99), "a score below the threshold must be rejected")

            ' 用序列文本打分应当与编码之后的结果一致
            Dim textA As String = alignment.Alphabet.Decode(codesA)
            Dim textB As String = alignment.Alphabet.Decode(codesB)

            Call Support.Near(detail.Score, scoring.Score(textA, textB), 1.0E-12, "scoring by text must match scoring by code")
        End Sub

        Private Sub ContactChecks()
            Dim alignment As JointAlignment = CoupledAlignment(60, 4, 4, 5150UL)
            Dim model As New PpiBayesianModel(alignment)
            Dim contacts As New ResidueContacts(model)
            Dim top As ResidueContacts.ContactPair() = contacts.TopContacts(5)

            Call Support.True(top.Length = 5, "TopContacts must honour the requested count")

            Dim coupled As Boolean = top.Any(
                Function(p) (p.ColumnA = 0 AndAlso p.ColumnB = alignment.LengthA))

            Call Support.True(coupled, "the coupled A0/B0 column pair must be the strongest contact")

            Dim previous As Double = Double.PositiveInfinity
            Dim ordered As Boolean = True

            For Each pair As ResidueContacts.ContactPair In top
                If pair.LogR > previous + 1.0E-09 Then
                    ordered = False
                End If

                previous = pair.LogR
            Next

            Call Support.True(ordered, "the contacts must be sorted by log R in descending order")
            Call Support.True(contacts.AggregateScore(10) > 0, "the aggregated residue score must be positive")

            Dim probability As Double = contacts.AggregateProbability(10)

            Call Support.True(probability > 0 AndAlso probability <= 1.0,
                              $"the aggregated probability must stay inside (0,1] (got {probability:G6})")
        End Sub

        Private Sub OrphanDecompositionChecks()
            Dim alignment As JointAlignment = CoupledAlignment(40, 3, 3, 8080UL)
            Dim model As New PpiBayesianModel(alignment)

            ' cognate 部分用 A0/A1/B0/B1 四列；激酶一侧另有 A2 列作为「未分配」，
            ' 响应调节蛋白一侧另有 B2 列作为「未分配」
            Dim orphan As New OrphanModel(model,
                                          {0, 1, 3, 4},
                                          {0, 1, 2},
                                          {0, 1},
                                          {3, 4, 5},
                                          {3, 4})

            Call Support.True(orphan.Validate(), "the orphan model must pass its consistency checks")

            Dim joint As Double = orphan.LogJointEvidence()
            Dim conditional As Double = orphan.LogOrphanKinaseGivenCognate()

            Call Support.True(Double.IsNaN(joint) = False, "the joint evidence must be finite")
            Call Support.True(Double.IsNaN(conditional) = False, "the conditional evidence must be finite")

            ' P(K|J_k) = P(K, J_k) / P(J_k)，因此条件似然可以大于 1(对数可以大于 0)
            Dim evidence As Double = orphan.LogEvidence()

            Call Support.Near(joint + conditional + orphan.LogOrphanReceiverGivenCognate(), evidence, 1.0E-12,
                              "the decomposition must add up to the total evidence")
        End Sub

        Private Sub InterologChecks()
            Dim model As New InterologModel With {.MinCoverage = 0.8, .MinIdentity = 0.8}
            Dim orthologs As InterologModel.OrthologPair() = {
                New InterologModel.OrthologPair With {.Query = "T1", .Subject = "S1", .Coverage = 0.95, .Identity = 0.9},
                New InterologModel.OrthologPair With {.Query = "T2", .Subject = "S2", .Coverage = 0.85, .Identity = 0.85},
                New InterologModel.OrthologPair With {.Query = "T3", .Subject = "S3", .Coverage = 0.5, .Identity = 0.99}
            }

            Dim filtered As InterologModel.OrthologPair() = model.Filter(orthologs)

            Call Support.Same(2, filtered.Length, "the coverage/identity thresholds must be applied")

            Dim known As New List(Of (a As String, b As String))
            Call known.Add(("S1", "S2"))

            Dim transferred As InterologModel.TransferredInteraction() = model.Transfer(known, filtered)

            Call Support.Same(1, transferred.Length, "one source interaction must be transferred")
            Call Support.True(String.Equals(transferred(0).ProteinA, "T1"), "the target protein id of the first member")
            Call Support.True(String.Equals(transferred(0).ProteinB, "T2"), "the target protein id of the second member")

            ' 伙伴缺少直系同源时不能迁移
            Dim unmatched As New List(Of (a As String, b As String))
            Call unmatched.Add(("S1", "S3"))

            Call Support.Same(0, model.Transfer(unmatched, filtered).Length,
                              "an interaction whose partner has no ortholog must not be transferred")
        End Sub
    End Module
End Namespace
