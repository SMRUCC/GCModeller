Imports System.Threading.Tasks
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Distance
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.MaximumLikelihood
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.TreeIO
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.TreeSearch
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview
Imports NjBuilder = SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.NeighborJoining.NeighborJoining

Namespace Evolution.Bayesian

    ''' <summary>
    ''' 贝叶斯推断的收敛诊断（基于对数似然轨迹的多链 Gelman-Rubin 统计量）。
    ''' </summary>
    Public Class ConvergenceDiagnostic
        Public Property Chains As Integer
        Public Property WithinChainVariance As Double
        Public Property BetweenChainVariance As Double
        ''' <summary>
        ''' 潜在尺度缩减因子（Potential Scale Reduction Factor），小于 1.1 视为收敛
        ''' </summary>
        Public Property PSRF As Double
        Public Property Converged As Boolean
        Public Property MeanLogLikelihood As Double
        Public Property StandardDeviation As Double
    End Class

    ''' <summary>
    ''' 贝叶斯推断（BI）结果。
    ''' </summary>
    Public Class BayesianResult
        ''' <summary>
        ''' 后验支持度最高的采样树（内部节点的 <see cref="PhyloNode.BootStrap"/> 为后验概率百分比）
        ''' </summary>
        Public Property Tree As PhyloNode
        ''' <summary>
        ''' 各 split（分支 bipartition）的后验支持度（0~1）
        ''' </summary>
        Public Property SplitFrequencies As Dictionary(Of String, Double)
        ''' <summary>
        ''' 每个 split 的可读名称（参与该 bipartition 的叶节点名称）
        ''' </summary>
        Public Property SplitLabels As Dictionary(Of String, String)
        ''' <summary>
        ''' 对数似然轨迹（burn-in 之后，全部链合并）
        ''' </summary>
        Public Property LogLikelihoodTrace As Double()
        Public Property BurnIn As Integer
        Public Property Samples As Integer
        Public Property AcceptanceRate As Double
        Public Property Convergence As ConvergenceDiagnostic
    End Class

    ''' <summary>
    ''' 贝叶斯推断（BI）：计算整棵树（以及模型参数）在数据条件下的后验分布
    '''
    ''' <c>P(τ, v, θ | D) ∝ P(D | τ, v, θ) P(τ, v, θ)</c>
    '''
    ''' 由于后验分布没有解析解，使用 Metropolis-Hastings MCMC 在参数空间中随机游走进行采样；
    ''' 每个内部分支的后验概率即“包含该分支的采样树占总样本的比例”。
    ''' </summary>
    ''' <remarks>
    ''' 先验设置：分支长度服从指数分布（均值为 <c>BranchLengthPriorMean</c>），拓扑服从均匀先验，
    ''' Gamma 形状参数 α（若参与采样）服从 [0.02, 50] 上的均匀先验。
    ''' 提议分布包括：NNI / SPR 拓扑移动、全树分支长度乘性缩放、单条分支长度乘性缩放、以及 α 的乘性缩放。
    ''' </remarks>
    Public Module BayesianInference

        ''' <summary>
        ''' 分支长度指数先验的均值（同时作为缩放提议的尺度参考）
        ''' </summary>
        Public Const BranchLengthPriorMean As Double = 0.1

        Private Const AlphaMinimum As Double = 0.02
        Private Const AlphaMaximum As Double = 50.0

        ''' <summary>
        ''' 执行贝叶斯 MCMC 建树。
        ''' </summary>
        ''' <param name="matrix">已比对的位点矩阵</param>
        ''' <param name="model">氨基酸替换模型（默认 LG）</param>
        ''' <param name="rateCategories">离散 Gamma 速率类别数目</param>
        ''' <param name="gammaShape">Gamma 形状参数 α 的初始值</param>
        ''' <param name="invariantProportion">不变位点比例 I</param>
        ''' <param name="chains">独立马尔可夫链的数目（用于收敛诊断，多链并行执行）</param>
        ''' <param name="samples">每条链的采样步数</param>
        ''' <param name="burnIn">弃置的 burn-in 步数</param>
        ''' <param name="sampleFrequency">采样间隔（每隔多少步记录一次样本）</param>
        ''' <param name="optimizeAlpha">是否对 α 进行采样</param>
        ''' <param name="seed">随机种子</param>
        Public Function Run(matrix As CharacterMatrix,
                            Optional model As AminoAcidModel = AminoAcidModel.LG,
                            Optional rateCategories As Integer = 4,
                            Optional gammaShape As Double = 1.0,
                            Optional invariantProportion As Double = 0,
                            Optional chains As Integer = 1,
                            Optional samples As Integer = 2000,
                            Optional burnIn As Integer = 500,
                            Optional sampleFrequency As Integer = 10,
                            Optional optimizeAlpha As Boolean = False,
                            Optional seed As Integer = 20240101) As BayesianResult

            If matrix.SequenceCount < 3 Then
                Throw New ArgumentException("贝叶斯推断至少需要 3 条序列！")
            End If
            If chains < 1 Then
                chains = 1
            End If
            If samples <= burnIn Then
                samples = burnIn + 100
            End If

            Dim substitution As SubstitutionModel = SubstitutionModel.Load(model)
            Dim initial As PhyloNode = NjBuilder.Build(matrix, DistanceModel.PoissonCorrection)

            Call MaximumLikelihoodTree.SanitizeBranchLengths(initial, BranchLengthPriorMean)

            Console.WriteLine($"[BI] model={model}, chains={chains}, samples={samples}, burn-in={burnIn}")

            Dim chainResults(chains - 1) As ChainResult

            If chains > 1 Then
                Call Parallel.For(0, chains,
                    Sub(i As Integer)
                        chainResults(i) = RunChain(matrix, substitution, initial, rateCategories, gammaShape, invariantProportion,
                                                   samples, burnIn, sampleFrequency, optimizeAlpha, seed + i)
                    End Sub)
            Else
                chainResults(0) = RunChain(matrix, substitution, initial, rateCategories, gammaShape, invariantProportion,
                                           samples, burnIn, sampleFrequency, optimizeAlpha, seed)
            End If

            ' ---- 合并各链的 split 统计 ----
            Dim counts As New Dictionary(Of String, Integer)
            Dim labels As New Dictionary(Of String, String)
            Dim totalSamples As Integer = 0
            Dim acceptedTotal As Integer = 0
            Dim stepsTotal As Integer = 0
            Dim trace As New List(Of Double)
            Dim bestTree As PhyloNode = Nothing
            Dim bestPosterior As Double = Double.NegativeInfinity

            For Each chain As ChainResult In chainResults
                For Each kv As KeyValuePair(Of String, Integer) In chain.SplitCounts
                    Dim current As Integer = 0
                    counts.TryGetValue(kv.Key, current)
                    counts(kv.Key) = current + kv.Value
                Next

                For Each kv As KeyValuePair(Of String, String) In chain.SplitLabels
                    labels(kv.Key) = kv.Value
                Next

                totalSamples += chain.TotalSamples
                acceptedTotal += chain.Accepted
                stepsTotal += chain.Steps
                trace.AddRange(chain.Trace)

                If chain.BestPosterior > bestPosterior AndAlso chain.BestTree IsNot Nothing Then
                    bestPosterior = chain.BestPosterior
                    bestTree = chain.BestTree
                End If
            Next

            Dim frequencies As New Dictionary(Of String, Double)

            For Each kv As KeyValuePair(Of String, Integer) In counts
                frequencies(kv.Key) = If(totalSamples > 0, kv.Value / totalSamples, 0)
            Next

            ' ---- 把后验概率写回到最优树的内部节点 ----
            If bestTree IsNot Nothing Then
                Dim leaves As String() = PhyloTreeFactory.LeafLabels(bestTree)

                For Each node As PhyloNode In PhyloTreeFactory.EnumerateNodes(bestTree)
                    If node Is bestTree OrElse node.Descendents.Count = 0 Then
                        Continue For
                    End If

                    Dim key As String = PhyloTreeFactory.SplitKey(PhyloTreeFactory.GetLeafNames(node), leaves)
                    Dim freq As Double = 0

                    If frequencies.TryGetValue(key, freq) Then
                        node.BootStrap = CSng(freq * 100)
                    End If
                Next
            End If

            Dim convergence As ConvergenceDiagnostic = Diagnose(chainResults)

            Console.WriteLine($"[BI] acceptance rate: {If(stepsTotal > 0, acceptedTotal / stepsTotal, 0):P2}, PSRF: {convergence.PSRF:F3}")

            Return New BayesianResult With {
                .Tree = bestTree,
                .SplitFrequencies = frequencies,
                .SplitLabels = labels,
                .LogLikelihoodTrace = trace.ToArray,
                .BurnIn = burnIn,
                .Samples = totalSamples,
                .AcceptanceRate = If(stepsTotal > 0, acceptedTotal / stepsTotal, 0),
                .Convergence = convergence
            }
        End Function

        ''' <summary>
        ''' 单条马尔可夫链的采样结果。
        ''' </summary>
        Private Class ChainResult
            Public Property SplitCounts As New Dictionary(Of String, Integer)
            Public Property SplitLabels As New Dictionary(Of String, String)
            Public Property Trace As New List(Of Double)
            Public Property Tree As PhyloNode
            Public Property BestTree As PhyloNode
            Public Property BestPosterior As Double = Double.NegativeInfinity
            Public Property TotalSamples As Integer
            Public Property Accepted As Integer
            Public Property Steps As Integer
        End Class

        Private Function RunChain(matrix As CharacterMatrix,
                                  substitution As SubstitutionModel,
                                  initial As PhyloNode,
                                  rateCategories As Integer,
                                  gammaShape As Double,
                                  invariantProportion As Double,
                                  samples As Integer,
                                  burnIn As Integer,
                                  sampleFrequency As Integer,
                                  optimizeAlpha As Boolean,
                                  seed As Integer) As ChainResult

            Dim rand As New Random(seed)
            Dim alpha As Double = gammaShape
            Dim gamma As New DiscreteGamma(alpha, rateCategories, invariantProportion)
            Dim pruning As New FelsensteinPruning(matrix, substitution, gamma)

            Dim current As PhyloNode = initial.Clone()
            Call MaximumLikelihoodTree.OptimizeBranchLengths(current, pruning, 1)

            Dim currentLogLikelihood As Double = pruning.LogLikelihood(current)
            Dim currentPrior As Double = LogPrior(current, alpha, optimizeAlpha)
            Dim currentPosterior As Double = currentLogLikelihood + currentPrior

            Dim result As New ChainResult With {.Tree = current}
            Dim accepted As Integer = 0

            For iteration As Integer = 1 To samples
                Dim kind As Double = rand.NextDouble()
                Dim proposal As PhyloNode = current.Clone()
                Dim proposalAlpha As Double = alpha
                Dim logHastings As Double = 0
                Dim valid As Boolean = False

                If kind < 0.5 Then
                    ' ---- NNI 拓扑移动 ----
                    Dim moves As List(Of NniMove) = TreeRearrangement.NniMoves(current)

                    If moves.Count > 0 Then
                        Dim move As NniMove = moves(rand.Next(moves.Count))
                        Call TreeRearrangement.ApplyNni(proposal, move)
                        valid = True
                    End If
                ElseIf kind < 0.75 Then
                    ' ---- 全树分支长度乘性缩放 ----
                    Dim factor As Double = Math.Exp(0.5 * (rand.NextDouble() - 0.5))
                    Dim edges As PhyloNode() = TreeRearrangement _
                        .EnumerateNodes(proposal) _
                        .Where(Function(n) n IsNot proposal) _
                        .ToArray

                    For Each edge As PhyloNode In edges
                        edge.BranchLength = CSng(edge.BranchLength * factor)
                    Next

                    logHastings = edges.Length * Math.Log(factor)
                    valid = True
                ElseIf kind < 0.9 AndAlso optimizeAlpha Then
                    ' ---- α 乘性缩放 ----
                    Dim factor As Double = Math.Exp(0.5 * (rand.NextDouble() - 0.5))
                    proposalAlpha = alpha * factor

                    If proposalAlpha < AlphaMinimum OrElse proposalAlpha > AlphaMaximum Then
                        valid = False
                    Else
                        logHastings = Math.Log(factor)
                        valid = True
                    End If
                Else
                    ' ---- SPR 拓扑移动 ----
                    Dim moves As List(Of SprMove) = TreeRearrangement.SprMoves(current, maxCount:=10, rand:=rand)

                    If moves.Count > 0 Then
                        Dim move As SprMove = moves(rand.Next(moves.Count))
                        Call TreeRearrangement.ApplySpr(proposal, move)
                        valid = True
                    End If
                End If

                If valid Then
                    Call MaximumLikelihoodTree.SanitizeBranchLengths(proposal, 0.01)

                    Dim proposalGamma As DiscreteGamma = gamma
                    Dim proposalPruning As FelsensteinPruning = pruning

                    If proposalAlpha <> alpha Then
                        proposalGamma = New DiscreteGamma(proposalAlpha, rateCategories, invariantProportion)
                        proposalPruning = New FelsensteinPruning(matrix, substitution, proposalGamma)
                    End If

                    Dim proposalLogLikelihood As Double = proposalPruning.LogLikelihood(proposal)
                    Dim proposalPrior As Double = LogPrior(proposal, proposalAlpha, optimizeAlpha)
                    Dim proposalPosterior As Double = proposalLogLikelihood + proposalPrior

                    Dim logRatio As Double = proposalPosterior - currentPosterior + logHastings

                    If Math.Log(rand.NextDouble()) < logRatio Then
                        ' 接受
                        current = proposal
                        alpha = proposalAlpha
                        gamma = proposalGamma
                        pruning = proposalPruning
                        currentLogLikelihood = proposalLogLikelihood
                        currentPrior = proposalPrior
                        currentPosterior = proposalPosterior
                        accepted += 1
                    End If
                End If

                result.Steps += 1

                If iteration > burnIn Then
                    result.Trace.Add(currentLogLikelihood)

                    If iteration Mod sampleFrequency = 0 Then
                        Call RecordSplits(current, result)
                        result.TotalSamples += 1
                    End If

                    If currentPosterior > result.BestPosterior Then
                        result.BestPosterior = currentPosterior
                        result.BestTree = current.Clone()
                    End If
                End If
            Next

            result.Tree = current
            result.Accepted = accepted

            Return result
        End Function

        ''' <summary>
        ''' 记录一棵采样树中的所有 split（内部分支 bipartition）。
        ''' </summary>
        Private Sub RecordSplits(tree As PhyloNode, result As ChainResult)
            Dim leaves As String() = PhyloTreeFactory.LeafLabels(tree)

            For Each node As PhyloNode In PhyloTreeFactory.EnumerateNodes(tree)
                If node Is tree OrElse node.Descendents.Count = 0 Then
                    Continue For
                End If

                Dim members As List(Of String) = PhyloTreeFactory.GetLeafNames(node)
                Dim key As String = PhyloTreeFactory.SplitKey(members, leaves)
                Dim count As Integer = 0

                result.SplitCounts.TryGetValue(key, count)
                result.SplitCounts(key) = count + 1

                If Not result.SplitLabels.ContainsKey(key) Then
                    result.SplitLabels(key) = String.Join(", ", members.OrderBy(Function(s) s, StringComparer.Ordinal))
                End If
            Next
        End Sub

        ''' <summary>
        ''' 后验分布的对数（未归一化）：对数似然 + 对数先验。
        ''' </summary>
        Private Function LogPrior(tree As PhyloNode, alpha As Double, optimizeAlpha As Boolean) As Double
            Dim logP As Double = 0

            For Each node As PhyloNode In TreeRearrangement.EnumerateNodes(tree)
                If node Is tree Then
                    Continue For
                End If

                If node.BranchLength < 0 Then
                    Return Double.NegativeInfinity
                End If

                ' 指数先验：log(1/μ) - t/μ（常数项可以省略）
                logP -= node.BranchLength / BranchLengthPriorMean
            Next

            If optimizeAlpha Then
                If alpha < AlphaMinimum OrElse alpha > AlphaMaximum Then
                    Return Double.NegativeInfinity
                End If
            End If

            Return logP
        End Function

        ''' <summary>
        ''' 基于多条链的对数似然轨迹给出 Gelman-Rubin 收敛诊断。
        ''' </summary>
        Private Function Diagnose(chains As ChainResult()) As ConvergenceDiagnostic
            Dim validChains As List(Of Double()) = chains _
                .Where(Function(c) c.Trace.Count > 1) _
                .Select(Function(c) c.Trace.ToArray) _
                .ToList

            Dim diagnostic As New ConvergenceDiagnostic With {.Chains = validChains.Count}

            If validChains.Count = 0 Then
                Return diagnostic
            End If

            Dim allValues As Double() = validChains.SelectMany(Function(t) t).ToArray
            diagnostic.MeanLogLikelihood = allValues.Average
            diagnostic.StandardDeviation = StandardDeviation(allValues)

            If validChains.Count < 2 Then
                diagnostic.WithinChainVariance = diagnostic.StandardDeviation ^ 2
                diagnostic.BetweenChainVariance = 0
                diagnostic.PSRF = 1
                diagnostic.Converged = True

                Return diagnostic
            End If

            Dim n As Integer = validChains.Min(Function(t) t.Length)
            Dim means(validChains.Count - 1) As Double
            Dim variances(validChains.Count - 1) As Double

            For i As Integer = 0 To validChains.Count - 1
                Dim sample As Double() = validChains(i).Take(n).ToArray
                means(i) = sample.Average
                variances(i) = Variance(sample)
            Next

            Dim within As Double = variances.Average
            Dim between As Double = n * Variance(means)
            Dim pooled As Double = (n - 1.0) / n * within + between / n

            diagnostic.WithinChainVariance = within
            diagnostic.BetweenChainVariance = between

            If within <= 0 Then
                diagnostic.PSRF = 1
            Else
                diagnostic.PSRF = Math.Sqrt(pooled / within)
            End If

            diagnostic.Converged = diagnostic.PSRF < 1.1

            Return diagnostic
        End Function

        Private Function Variance(values As Double()) As Double
            If values.Length < 2 Then
                Return 0
            End If

            Dim mean As Double = values.Average
            Dim sum As Double = 0

            For Each v As Double In values
                sum += (v - mean) ^ 2
            Next

            Return sum / (values.Length - 1)
        End Function

        Private Function StandardDeviation(values As Double()) As Double
            Return Math.Sqrt(Variance(values))
        End Function
    End Module

End Namespace
