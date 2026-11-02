Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Inference
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Model
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics

Namespace test.Tests

    ''' <summary>
    ''' Gibbs 分配采样器与精确枚举后验的对照。
    ''' </summary>
    ''' <remarks>
    ''' 构造 4 个基因组、每个基因组 2 个候选成员的小规模数据：只有 2 个候选成员时，
    ''' 每个基因组只有「保持原配」与「互换伙伴」两种分配，因此整个分配空间只有
    ''' <c>2⁴ = 16</c> 个状态，可以精确枚举。
    ''' 对于第 0 个基因组，其边缘后验由 <c>P(a) ∝ P(D|a)</c> 在两个状态上归一化得到，
    ''' 采样频率应当收敛到该值。
    ''' </remarks>
    Module Gibbs

        ''' <summary>
        ''' 构造「家族 A 的第 0 列与家族 B 的第 0 列完全耦合」的联合比对
        ''' </summary>
        Private Function BuildCoupled(groups As Integer, perGroup As Integer, seed As ULong) As (joint As JointAlignment, groupIds As Integer())
            Dim alphabet As ResidueAlphabet = ResidueAlphabet.Standard21
            Dim rng As New Fixtures.Lcg(seed)
            Dim rows As Integer = groups * perGroup
            Dim lengthA As Integer = 2
            Dim lengthB As Integer = 2
            Dim data As Integer(,) = New Integer(rows - 1, lengthA + lengthB - 1) {}
            Dim names As String() = New String(rows - 1) {}
            Dim ids As Integer() = New Integer(rows - 1) {}
            Dim index As Integer = 0

            For g As Integer = 0 To groups - 1
                For k As Integer = 0 To perGroup - 1
                    names(index) = "genome_" & g
                    ids(index) = g

                    Dim coupled As Integer = (index * 3) Mod 12

                    data(index, 0) = coupled
                    data(index, lengthA) = coupled
                    data(index, 1) = rng.Next(4)
                    data(index, lengthA + 1) = rng.Next(4)

                    index += 1
                Next
            Next

            Return (New JointAlignment(data, names, lengthA, lengthB, alphabet), ids)
        End Function

        Public Sub Run()
            Call ExactPosteriorChecks()
            Call ReproducibilityChecks()
        End Sub

        Private Sub ExactPosteriorChecks()
            Dim fixture = BuildCoupled(4, 2, 606060UL)
            Dim joint As JointAlignment = fixture.joint
            Dim groupIds As Integer() = fixture.groupIds
            Dim options As New PpiModelOptions()

            Dim identity As New PpiBayesianModel(joint, options)
            Dim swapped As New PpiBayesianModel(joint.SwapPartners(0, 1), options)
            Dim logIdentity As Double = identity.LogEvidence()
            Dim logSwapped As Double = swapped.LogEvidence()

            ' 保持原配时第 0 列完全耦合，因此该状态的后验概率应当更高
            Dim exact As Double = 1.0 / (1.0 + Math.Exp(logSwapped - logIdentity))

            Call Support.True(exact > 0.5,
                              $"the identity assignment must be a posteriori more likely (got {exact:0.000})")

            Dim samplerOptions As New GibbsSamplerOptions With {
                .Iterations = 4000,
                .BurnIn = 500,
                .Runs = 3,
                .RandomSeed = 20261102,
                .Groups = groupIds
            }
            Dim sampler As New GibbsSampler(joint, options, samplerOptions)
            Dim result As InferenceResult = sampler.Run()

            Call Support.Same(joint.Rows, result.CognateProbability.Length, "every candidate pair must get a frequency")

            Call Support.Near(exact, result.CognateProbability(0), 0.15,
                              "the sampled frequency must approach the exact two-state posterior")

            ' 频率必须归一化
            For r As Integer = 0 To result.CognateProbability.Length - 1
                Dim total As Double = 0.0

                For Each partner As InferenceResult.Partner In result.Partners(r)
                    total += partner.Frequency
                Next

                Call Support.Near(1.0, total, 1.0E-09, $"the partner frequencies of row {r} must sum to one")
                Call Support.True(result.CognateProbability(r) >= 0 AndAlso result.CognateProbability(r) <= 1.0 + 1.0E-09,
                                  $"the cognate frequency of row {r} must lie in [0,1]")
            Next

            ' 每一个基因组内部的候选伙伴都应当被统计到
            For r As Integer = 0 To result.Partners.Length - 1
                Call Support.Same(2, result.Partners(r).Count, $"row {r} must have two candidate partners")
                Call Support.Same(1, result.Partners(r)(0).Rank, $"the best partner of row {r} must be ranked first")
            Next

            ' 导出的表格必须包含全部候选
            Dim table As String = result.ToTable()

            Call Support.True(table.Split(vbLf).Length >= joint.Rows, "the table must contain one line per candidate pair")
        End Sub

        Private Sub ReproducibilityChecks()
            Dim fixture = BuildCoupled(3, 2, 909090UL)
            Dim options As New PpiModelOptions()
            Dim settings As New GibbsSamplerOptions With {
                .Iterations = 800,
                .BurnIn = 100,
                .Runs = 2,
                .RandomSeed = 7,
                .Groups = fixture.groupIds
            }
            Dim first As InferenceResult = New GibbsSampler(fixture.joint, options, settings).Run()
            Dim second As InferenceResult = New GibbsSampler(fixture.joint, options, settings).Run()

            For r As Integer = 0 To first.CognateProbability.Length - 1
                Call Support.Near(first.CognateProbability(r), second.CognateProbability(r), 1.0E-12,
                                  $"the sampler must be reproducible for row {r} with a fixed seed")
            Next

            ' 系统发育置换检验：在完全耦合的数据上，置换之后的似然比应当更低
            Dim permutation As New PhylogeneticPermutation(fixture.joint, fixture.groupIds, options)
            Dim report = permutation.Run(200, 20, {0.5, 0.9})

            Call Support.Same(2, report.TrueQuantiles.Length, "one quantile per requested probability")
            Call Support.True(report.Permutations > 0, "at least one permutation must be executed")
            Call Support.True(report.KSStatistic >= 0 AndAlso report.KSStatistic <= 1.0,
                              "the KS statistic must lie in [0,1]")
            Call Support.True(report.SignalDetected,
                              $"the coupled alignment must show a phylogenetic-independent signal ({report})")
        End Sub
    End Module
End Namespace
