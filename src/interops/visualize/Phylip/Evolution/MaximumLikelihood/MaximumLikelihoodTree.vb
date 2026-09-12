Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Distance
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.TreeSearch
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview
Imports NjBuilder = SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.NeighborJoining.NeighborJoining

Namespace Evolution.MaximumLikelihood

    ''' <summary>
    ''' 最大似然法建树结果。
    ''' </summary>
    Public Class MaximumLikelihoodResult
        Public Property Tree As PhyloNode
        Public Property LogLikelihood As Double
        Public Property Sites As Integer
        Public Property Model As AminoAcidModel
        Public Property GammaShape As Double
        Public Property InvariantProportion As Double
        Public Property RateCategories As Integer
        Public Property Iterations As Integer
        ''' <summary>
        ''' 单位位点的平均对数似然（用于不同模型之间比较）
        ''' </summary>
        Public Property LogLikelihoodPerSite As Double
    End Class

    ''' <summary>
    ''' 最大似然法（ML）：给定带分支长度的树 T 与替换模型 M，计算观测数据 D 的条件概率
    ''' <c>P(D|T, M)</c>，并选择使似然最大的树。
    ''' </summary>
    ''' <remarks>
    ''' 实现要点：
    ''' <list type="number">
    ''' <item>以 NJ 树作为初始拓扑；</item>
    ''' <item>使用 <see cref="FelsensteinPruning"/> 计算对数似然；</item>
    ''' <item>对每条分支做一维（黄金分割）分支长度优化；</item>
    ''' <item>使用 NNI / SPR 拓扑算子做爬山式树搜索；</item>
    ''' <item>可选地优化离散 Gamma 的形状参数 α。</item>
    ''' </list>
    ''' </remarks>
    Public Module MaximumLikelihoodTree

        ''' <summary>
        ''' 执行最大似然法建树。
        ''' </summary>
        ''' <param name="matrix">已比对的位点矩阵</param>
        ''' <param name="model">氨基酸替换模型（默认 LG）</param>
        ''' <param name="rateCategories">离散 Gamma 速率类别数目（默认为 4）</param>
        ''' <param name="gammaShape">Gamma 形状参数 α（默认为 1，表示无速率异质性）</param>
        ''' <param name="invariantProportion">不变位点比例 I（默认 0）</param>
        ''' <param name="initial">可选初始树；默认使用 NJ 树</param>
        ''' <param name="maxIterations">拓扑搜索的最大迭代轮次（默认为 10）</param>
        ''' <param name="useSpr">是否在 NNI 无法改进时尝试 SPR 移动</param>
        ''' <param name="optimizeShape">是否对 Gamma 形状参数 α 进行最大似然估计</param>
        ''' <param name="edgeOptimizationRounds">分支长度优化的轮次</param>
        ''' <param name="seed">随机种子（用于 SPR 采样）</param>
        Public Function Build(matrix As CharacterMatrix,
                              Optional model As AminoAcidModel = AminoAcidModel.LG,
                              Optional rateCategories As Integer = 4,
                              Optional gammaShape As Double = 1.0,
                              Optional invariantProportion As Double = 0,
                              Optional initial As PhyloNode = Nothing,
                              Optional maxIterations As Integer = 0,
                              Optional useSpr As Boolean = True,
                              Optional optimizeShape As Boolean = False,
                              Optional edgeOptimizationRounds As Integer = 2,
                              Optional seed As Integer = 1234) As MaximumLikelihoodResult

            If matrix.SequenceCount < 3 Then
                Throw New ArgumentException("最大似然法至少需要 3 条序列！")
            End If

            Dim substitution As SubstitutionModel = SubstitutionModel.Load(model)
            Dim gamma As New DiscreteGamma(gammaShape, rateCategories, invariantProportion)

            If initial Is Nothing Then
                initial = NjBuilder.Build(matrix, DistanceModel.PoissonCorrection)
            End If

            Call SanitizeBranchLengths(initial, 0.05)

            If maxIterations <= 0 Then
                maxIterations = 10
            End If

            Dim pruning As New FelsensteinPruning(matrix, substitution, gamma)
            Dim current As PhyloNode = initial
            Dim rand As New Random(seed)

            Console.WriteLine($"[ML] model={model}, gamma categories={rateCategories}, alpha={gammaShape}, I={invariantProportion}")

            ' 初始分支长度优化
            Call OptimizeBranchLengths(current, pruning, edgeOptimizationRounds)

            Dim best As Double = pruning.LogLikelihood(current)

            Console.WriteLine($"[ML] initial log-likelihood: {best}")

            Dim iter As Integer = 0

            While iter < maxIterations
                Dim improved As Boolean = False

                ' ---- NNI 爬山 ----
                Dim bestNniTree As PhyloNode = Nothing
                Dim bestNniScore As Double = best

                For Each move As NniMove In TreeRearrangement.NniMoves(current)
                    Dim trial As PhyloNode = current.Clone()
                    Call TreeRearrangement.ApplyNni(trial, move)
                    Call SanitizeBranchLengths(trial, 0.05)

                    Dim score As Double = pruning.LogLikelihood(trial)

                    If score > bestNniScore Then
                        bestNniScore = score
                        bestNniTree = trial
                    End If
                Next

                If bestNniTree IsNot Nothing Then
                    current = bestNniTree
                    best = bestNniScore
                    Call OptimizeBranchLengths(current, pruning, edgeOptimizationRounds)
                    best = pruning.LogLikelihood(current)
                    improved = True
                End If

                ' ---- SPR 爬山 ----
                If Not improved AndAlso useSpr Then
                    Dim bestSprTree As PhyloNode = Nothing
                    Dim bestSprScore As Double = best

                    For Each move As SprMove In TreeRearrangement.SprMoves(current, maxCount:=40, rand:=rand)
                        Dim trial As PhyloNode = current.Clone()
                        Call TreeRearrangement.ApplySpr(trial, move)
                        Call SanitizeBranchLengths(trial, 0.05)

                        Dim score As Double = pruning.LogLikelihood(trial)

                        If score > bestSprScore Then
                            bestSprScore = score
                            bestSprTree = trial
                        End If
                    Next

                    If bestSprTree IsNot Nothing Then
                        current = bestSprTree
                        best = bestSprScore
                        Call OptimizeBranchLengths(current, pruning, edgeOptimizationRounds)
                        best = pruning.LogLikelihood(current)
                        improved = True
                    End If
                End If

                iter += 1

                If Not improved Then
                    Exit While
                End If

                Console.WriteLine($"[ML] iteration {iter}: log-likelihood -> {best}")
            End While

            ' ---- 可选的 Gamma 形状参数最大似然估计 ----
            If optimizeShape Then
                Dim shapeResult = OptimizeGammaShape(matrix, substitution, current, rateCategories, invariantProportion, gammaShape)
                gammaShape = shapeResult.Shape

                Dim newGamma As New DiscreteGamma(gammaShape, rateCategories, invariantProportion)
                Dim newPruning As New FelsensteinPruning(matrix, substitution, newGamma)

                Call OptimizeBranchLengths(current, newPruning, edgeOptimizationRounds)
                best = newPruning.LogLikelihood(current)
                pruning = newPruning

                Console.WriteLine($"[ML] optimized gamma shape alpha = {gammaShape}, log-likelihood = {best}")
            End If

            Console.WriteLine($"[ML] final log-likelihood: {best} ({pruning.Sites.Length} sites), iterations: {iter}")

            Return New MaximumLikelihoodResult With {
                .Tree = current,
                .LogLikelihood = best,
                .Sites = pruning.Sites.Length,
                .Model = model,
                .GammaShape = gammaShape,
                .InvariantProportion = invariantProportion,
                .RateCategories = rateCategories,
                .Iterations = iter,
                .LogLikelihoodPerSite = best / pruning.Sites.Length
            }
        End Function

        ''' <summary>
        ''' 保证树中所有内部节点的分支长度为一个正数，避免似然计算中出现退化的零长度分支。
        ''' </summary>
        Public Sub SanitizeBranchLengths(root As PhyloNode, defaultLength As Double)
            For Each node As PhyloNode In TreeRearrangement.EnumerateNodes(root)
                If node Is root Then
                    Continue For
                End If

                If node.BranchLength <= 0 Then
                    node.BranchLength = CSng(defaultLength)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 逐条分支做一维分支长度优化，重复若干轮直至稳定。
        ''' </summary>
        Public Sub OptimizeBranchLengths(root As PhyloNode, pruning As FelsensteinPruning, rounds As Integer)
            Dim edges As PhyloNode() = TreeRearrangement _
                .EnumerateNodes(root) _
                .Where(Function(n) n IsNot root) _
                .ToArray

            For r As Integer = 1 To Math.Max(1, rounds)
                For Each edge As PhyloNode In edges
                    Call OptimizeEdge(root, edge, pruning)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 对单条分支的长度进行一维最大化（黄金分割搜索）。
        ''' </summary>
        Private Sub OptimizeEdge(root As PhyloNode, edge As PhyloNode, pruning As FelsensteinPruning)
            Dim evaluate As Func(Of Double, Double) =
                Function(t As Double)
                    edge.BranchLength = CSng(t)

                    Return pruning.LogLikelihood(root)
                End Function

            Dim lo As Double = 1.0E-06
            Dim hi As Double = Math.Max(edge.BranchLength * 4, 0.1)

            ' 扩张上界，直到似然开始下降
            Dim guard As Integer = 0

            While guard < 25 AndAlso evaluate(hi) > evaluate(hi / 2)
                hi *= 2
                guard += 1
            End While

            Dim bestT As Double = GoldenSection(evaluate, lo, hi, 1.0E-05, 25)
            edge.BranchLength = CSng(bestT)
        End Sub

        ''' <summary>
        ''' 通过最大化对数似然估计离散 Gamma 的形状参数 α。
        ''' </summary>
        Private Function OptimizeGammaShape(matrix As CharacterMatrix,
                                            substitution As SubstitutionModel,
                                            tree As PhyloNode,
                                            categories As Integer,
                                            invariantProportion As Double,
                                            initialShape As Double) As (Shape As Double, LogLikelihood As Double)

            Dim evaluate As Func(Of Double, Double) =
                Function(logAlpha As Double)
                    Dim alpha As Double = Math.Exp(logAlpha)
                    Dim gamma As New DiscreteGamma(alpha, categories, invariantProportion)
                    Dim pruning As New FelsensteinPruning(matrix, substitution, gamma)

                    Return pruning.LogLikelihood(tree)
                End Function

            Dim lo As Double = Math.Log(0.02)
            Dim hi As Double = Math.Log(50)
            Dim bestLogAlpha As Double = GoldenSection(evaluate, lo, hi, 1.0E-03, 30)

            Return (Math.Exp(bestLogAlpha), evaluate(bestLogAlpha))
        End Function

        ''' <summary>
        ''' 黄金分割搜索（求一维函数的最大值）。
        ''' </summary>
        Public Function GoldenSection(f As Func(Of Double, Double), lo As Double, hi As Double, tol As Double, maxIterations As Integer) As Double
            Const phi As Double = 0.6180339887498949

            Dim a As Double = lo
            Dim b As Double = hi
            Dim c As Double = b - phi * (b - a)
            Dim d As Double = a + phi * (b - a)
            Dim fc As Double = f(c)
            Dim fd As Double = f(d)

            For i As Integer = 1 To maxIterations
                If Math.Abs(b - a) < tol Then
                    Exit For
                End If

                If fc > fd Then
                    b = d
                    d = c
                    fd = fc
                    c = b - phi * (b - a)
                    fc = f(c)
                Else
                    a = c
                    c = d
                    fc = fd
                    d = a + phi * (b - a)
                    fd = f(d)
                End If
            Next

            Return (a + b) / 2
        End Function
    End Module

End Namespace
