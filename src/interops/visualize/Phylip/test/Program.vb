Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Interops.Visualize.Phylip
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.Drawing
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Bootstrap
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Distance
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.MaximumLikelihood
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.TreeIO
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview
Imports BiRunner = SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Bayesian.BayesianInference
Imports BsRunner = SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Bootstrap.BootstrapAnalysis
Imports MlBuilder = SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.MaximumLikelihood.MaximumLikelihoodTree
Imports MpBuilder = SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Parsimony.MaximumParsimony
Imports NjBuilder = SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.NeighborJoining.NeighborJoining
Imports UpgmaBuilder = SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.UPGMA.UpgmaTree

''' <summary>
''' Phylip 进化树算法模块的端到端验证程序。
''' </summary>
Module Program

    Sub New()
        Call ImageDriver.Register()
    End Sub

    Sub Main(args As String())
        Dim failures As New List(Of String)

        Call TestNeighborJoining(failures)
        Call TestUpgma(failures)
        Call TestSubstitutionModels(failures)
        Call TestDiscreteGamma(failures)
        Call TestMaximumParsimony(failures)
        Call TestMaximumLikelihood(failures)
        Call TestBayesian(failures)
        Call TestBootstrap(failures)
        Call TestFastaWorkflow(failures)
        Call TestTreeDrawing(failures)

        Console.WriteLine()

        If failures.Count = 0 Then
            Console.WriteLine("ALL TESTS PASSED")
        Else
            Console.WriteLine($"FAILED ({failures.Count}):")

            For Each message As String In failures
                Console.WriteLine("  - " & message)
            Next

            Environment.ExitCode = 1
        End If
    End Sub

#Region "测试数据"

    ''' <summary>
    ''' readme.md 之中 NJ 的 4 物种数值算例。
    ''' d(A,B)=0.1, d(A,C)=0.3, d(A,D)=0.4, d(B,C)=0.2, d(B,D)=0.3, d(C,D)=0.1
    ''' 期望第一轮连接 C 与 D。
    ''' </summary>
    Private Function ExampleDistanceMatrix() As DistanceMatrix
        Dim names As String() = {"A", "B", "C", "D"}
        Dim matrix As Double()() = {
            New Double() {0, 0.1, 0.3, 0.4},
            New Double() {0.1, 0, 0.2, 0.3},
            New Double() {0.3, 0.2, 0, 0.1},
            New Double() {0.4, 0.3, 0.1, 0}
        }

        Return DistanceMatrix.FromMatrix(names, matrix)
    End Function

    Private Function ExampleMatrix() As CharacterMatrix
        Dim names As String() = {"A", "B", "C", "D", "E"}
        Dim seqs As String() = {
            "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQ",
            "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQ",
            "MRTAYIAKQRQVSFVKSHFSRQLEERLGLIEVA",
            "MRTAYIAKQRQVSFVKSHFSRQLEERLGLIEVA",
            "MKTAYVAKQRQVSFVKSHFSRQMEERLGLIEVQ"
        }

        Return CharacterMatrix.FromAligned(names, seqs, CharacterSet.Protein)
    End Function

#End Region

    Private Sub TestNeighborJoining(failures As List(Of String))
        Console.WriteLine("=== Neighbor-Joining ===")

        Try
            Dim tree As PhyloNode = NjBuilder.Build(ExampleDistanceMatrix())

            Dim leafC As PhyloNode = FindLeaf(tree, "C")
            Dim leafD As PhyloNode = FindLeaf(tree, "D")

            If leafC Is Nothing OrElse leafD Is Nothing Then
                failures.Add("NJ: 未能找到 C/D 叶节点")
            ElseIf leafC.Parent IsNot leafD.Parent Then
                failures.Add("NJ: 期望 C 与 D 互为姐妹分支（第一轮合并），但实际不是")
            Else
                Console.WriteLine("  C/D cherry: OK")
            End If

            If tree.Descendents.Count <> 3 Then
                failures.Add($"NJ: 无根树根节点应有三条分支，实际为 {tree.Descendents.Count}")
            End If

            Console.WriteLine("  newick: " & PhyloTreeFactory.CreateNewick(tree))
        Catch ex As Exception
            failures.Add("NJ: " & ex.Message)
        End Try
    End Sub

    Private Sub TestUpgma(failures As List(Of String))
        Console.WriteLine("=== UPGMA ===")

        Try
            Dim tree As PhyloNode = UpgmaBuilder.Build(ExampleDistanceMatrix())

            Dim leafC As PhyloNode = FindLeaf(tree, "C")
            Dim leafD As PhyloNode = FindLeaf(tree, "D")

            If leafC Is Nothing OrElse leafD Is Nothing Then
                failures.Add("UPGMA: 未能找到 C/D 叶节点")
            ElseIf leafC.Parent IsNot leafD.Parent Then
                failures.Add("UPGMA: 期望 C 与 D 互为姐妹分支，但实际不是")
            Else
                Console.WriteLine("  C/D cherry: OK")
            End If

            Console.WriteLine("  newick: " & PhyloTreeFactory.CreateNewick(tree))
        Catch ex As Exception
            failures.Add("UPGMA: " & ex.Message)
        End Try
    End Sub

    Private Sub TestSubstitutionModels(failures As List(Of String))
        Console.WriteLine("=== Substitution models ===")

        For Each model As AminoAcidModel In {AminoAcidModel.Dayhoff, AminoAcidModel.JTT, AminoAcidModel.WAG, AminoAcidModel.LG}
            Try
                Dim substitution As SubstitutionModel = SubstitutionModel.Load(model)

                If substitution.Dimension <> 20 Then
                    failures.Add($"{model}: 状态维度应为 20，实际 {substitution.Dimension}")
                    Continue For
                End If

                Dim piSum As Double = substitution.Pi.Sum

                If Math.Abs(piSum - 1) > 1.0E-06 Then
                    failures.Add($"{model}: 平衡频率之和应为 1，实际 {piSum}")
                End If

                Dim exponential As New ReversibleMatrixExponential(CType(substitution, EmpiricalAminoAcidModel).Exchangeability, substitution.Pi)
                Dim maxEigen As Double = exponential.Eigenvalues.Max()

                If maxEigen > 1.0E-06 Then
                    failures.Add($"{model}: 最大特征值应约为 0，实际 {maxEigen}")
                End If

                ' 长时间极限下 P(t) 的每一行应收敛到 π
                Dim p As Double()() = substitution.TransitionProbability(100)

                For i As Integer = 0 To 19
                    Dim rowSum As Double = p(i).Sum

                    If Math.Abs(rowSum - 1) > 1.0E-06 Then
                        failures.Add($"{model}: P(t) 第 {i} 行之和应为 1，实际 {rowSum}")
                        Exit For
                    End If
                Next

                Dim maxPiDeviation As Double = 0

                For j As Integer = 0 To 19
                    maxPiDeviation = Math.Max(maxPiDeviation, Math.Abs(p(0)(j) - substitution.Pi(j)))
                Next

                If maxPiDeviation > 1.0E-03 Then
                    failures.Add($"{model}: P(t→∞) 未收敛到平衡频率，最大偏差 {maxPiDeviation}")
                End If

                Console.WriteLine($"  {model}: OK (pi[0]={substitution.Pi(0):F4}, lambda_max={maxEigen:E2})")
            Catch ex As Exception
                failures.Add($"{model}: {ex.Message}")
            End Try
        Next
    End Sub

    Private Sub TestDiscreteGamma(failures As List(Of String))
        Console.WriteLine("=== Discrete Gamma ===")

        Try
            Dim gamma As New DiscreteGamma(0.5, 4)
            Dim mean As Double = 0

            For k As Integer = 0 To gamma.Rates.Length - 1
                mean += gamma.Rates(k) * gamma.Weights(k)
            Next

            If Math.Abs(mean - 1) > 1.0E-06 Then
                failures.Add($"DiscreteGamma: 平均速率应为 1，实际 {mean}")
            End If

            If gamma.Rates.Length <> 4 Then
                failures.Add($"DiscreteGamma: 速率类别数应为 4，实际 {gamma.Rates.Length}")
            End If

            Console.WriteLine($"  rates: {String.Join(", ", gamma.Rates.Select(Function(r) r.ToString("F4")))}, mean={mean:F6}")

            Dim withInvariant As New DiscreteGamma(0.5, 4, 0.2)
            Dim mean2 As Double = 0

            For k As Integer = 0 To withInvariant.Rates.Length - 1
                mean2 += withInvariant.Rates(k) * withInvariant.Weights(k)
            Next

            If Math.Abs(mean2 - 1) > 1.0E-06 Then
                failures.Add($"DiscreteGamma(+I): 平均速率应为 1，实际 {mean2}")
            End If

            If withInvariant.Rates(0) <> 0 Then
                failures.Add("DiscreteGamma(+I): 第一个类别应为不变位点（速率 0）")
            End If

            Console.WriteLine($"  +I rates: {String.Join(", ", withInvariant.Rates.Select(Function(r) r.ToString("F4")))}, mean={mean2:F6}")
        Catch ex As Exception
            failures.Add("DiscreteGamma: " & ex.Message)
        End Try
    End Sub

    Private Sub TestMaximumParsimony(failures As List(Of String))
        Console.WriteLine("=== Maximum Parsimony ===")

        Try
            Dim matrix As CharacterMatrix = ExampleMatrix()
            Dim sites As Integer() = matrix.InformativeSites()

            Console.WriteLine($"  informative sites: {sites.Length}")

            If sites.Length = 0 Then
                failures.Add("MP: 测试数据应包含信息位点")
                Return
            End If

            Dim result = MpBuilder.Build(matrix, maxIterations:=20)
            Dim leaves As String() = PhyloTreeFactory.LeafLabels(result.Tree)

            If leaves.Length <> matrix.SequenceCount Then
                failures.Add($"MP: 结果树的叶节点数应为 {matrix.SequenceCount}，实际 {leaves.Length}")
            End If

            If result.Score < 0 Then
                failures.Add("MP: 树长不应为负数")
            End If

            Console.WriteLine($"  tree length: {result.Score}, iterations: {result.Iterations}")
            Console.WriteLine("  newick: " & PhyloTreeFactory.CreateNewick(result.Tree))
        Catch ex As Exception
            failures.Add("MP: " & ex.Message)
        End Try
    End Sub

    Private Sub TestMaximumLikelihood(failures As List(Of String))
        Console.WriteLine("=== Maximum Likelihood ===")

        Try
            Dim matrix As CharacterMatrix = ExampleMatrix()
            Dim result = MlBuilder.Build(matrix,
                                         model:=AminoAcidModel.LG,
                                         rateCategories:=4,
                                         gammaShape:=0.8,
                                         maxIterations:=3,
                                         edgeOptimizationRounds:=2,
                                         seed:=100)

            If Double.IsNegativeInfinity(result.LogLikelihood) OrElse Double.IsNaN(result.LogLikelihood) Then
                failures.Add($"ML: 对数似然无效：{result.LogLikelihood}")
            End If

            Dim leaves As String() = PhyloTreeFactory.LeafLabels(result.Tree)

            If leaves.Length <> matrix.SequenceCount Then
                failures.Add($"ML: 结果树的叶节点数应为 {matrix.SequenceCount}，实际 {leaves.Length}")
            End If

            Console.WriteLine($"  logL: {result.LogLikelihood:F4}, per site: {result.LogLikelihoodPerSite:F4}")

            ' 校验：所有分支长度均为正数
            For Each node As PhyloNode In PhyloTreeFactory.EnumerateNodes(result.Tree)
                If node IsNot result.Tree AndAlso node.BranchLength <= 0 Then
                    failures.Add($"ML: 分支长度应大于 0（节点 {node.ID} 为 {node.BranchLength}）")
                    Exit For
                End If
            Next

            Console.WriteLine("  newick: " & PhyloTreeFactory.CreateNewick(result.Tree))
        Catch ex As Exception
            failures.Add("ML: " & ex.Message)
        End Try
    End Sub

    Private Sub TestBayesian(failures As List(Of String))
        Console.WriteLine("=== Bayesian Inference ===")

        Try
            Dim matrix As CharacterMatrix = ExampleMatrix()
            Dim result = BiRunner.Run(matrix,
                                      model:=AminoAcidModel.LG,
                                      rateCategories:=4,
                                      chains:=2,
                                      samples:=400,
                                      burnIn:=150,
                                      sampleFrequency:=25,
                                      seed:=777)

            If result.Tree Is Nothing Then
                failures.Add("BI: 未返回最优树")
            Else
                Console.WriteLine($"  splits: {result.SplitFrequencies.Count}, acceptance: {result.AcceptanceRate:P2}")
                Console.WriteLine("  newick: " & PhyloTreeFactory.CreateNewick(result.Tree))
            End If

            If result.Convergence Is Nothing Then
                failures.Add("BI: 缺少收敛诊断结果")
            Else
                Console.WriteLine($"  PSRF: {result.Convergence.PSRF:F4}, converged: {result.Convergence.Converged}")
            End If

            If result.Samples < 1 Then
                failures.Add("BI: 未采集到任何样本")
            End If
        Catch ex As Exception
            failures.Add("BI: " & ex.Message)
        End Try
    End Sub

    Private Sub TestBootstrap(failures As List(Of String))
        Console.WriteLine("=== Bootstrap ===")

        Try
            Dim matrix As CharacterMatrix = ExampleMatrix()

            ' 距离法 bootstrap
            Dim nj As BootstrapResult = BsRunner.Run(matrix, EvolutionAlgorithm.NeighborJoining, replicates:=20)
            Console.WriteLine($"  NJ bootstrap splits: {nj.SplitSupport.Count}")
            Console.WriteLine("  newick: " & PhyloTreeFactory.CreateNewick(nj.Tree))

            If nj.SplitSupport.Count = 0 Then
                failures.Add("Bootstrap: NJ 未得到任何 split 的支持度")
            End If

            ' 最大简约法 bootstrap（同时验证重采样的健壮性）
            Dim mp As BootstrapResult = BsRunner.Run(matrix, EvolutionAlgorithm.MaximumParsimony, replicates:=10, parallel:=False)
            Console.WriteLine($"  MP bootstrap splits: {mp.SplitSupport.Count}")

            ' 树内部分支的 bootstrap 值应位于 [0, 100]
            For Each node As PhyloNode In PhyloTreeFactory.EnumerateNodes(mp.Tree)
                If node Is mp.Tree OrElse node.Descendents.Count = 0 Then
                    Continue For
                End If

                If node.BootStrap < 0 OrElse node.BootStrap > 100 Then
                    failures.Add($"Bootstrap: 支持度 {node.BootStrap} 超出 [0, 100]")
                    Exit For
                End If
            Next
        Catch ex As Exception
            failures.Add("Bootstrap: " & ex.Message)
        End Try
    End Sub

    Private Sub TestFastaWorkflow(failures As List(Of String))
        Console.WriteLine("=== FASTA workflow ===")

        Dim path As String = IO.Path.Combine(IO.Path.GetTempPath(), "phylip_test_alignment.fasta")

        Try
            Dim names As String() = {"A", "B", "C", "D", "E"}
            Dim seqs As String() = {
                "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQ",
                "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQ",
                "MRTAYIAKQRQVSFVKSHFSRQLEERLGLIEVA",
                "MRTAYIAKQRQVSFVKSHFSRQLEERLGLIEVA",
                "MKTAYVAKQRQVSFVKSHFSRQMEERLGLIEVQ"
            }

            Dim lines As New List(Of String)

            For i As Integer = 0 To names.Length - 1
                lines.Add(">" & names(i))
                lines.Add(seqs(i))
            Next

            Call IO.File.WriteAllLines(path, lines)

            ' 1. FASTA -> 位点矩阵
            Dim matrix As CharacterMatrix = EvolutionAPI.LoadCharacterMatrix(path, align:=False)

            If matrix.SequenceCount <> 5 OrElse matrix.SiteCount <> 33 Then
                failures.Add($"FASTA workflow: 期望 5 x 33 的矩阵，实际 {matrix.SequenceCount} x {matrix.SiteCount}")
            End If

            ' 2. FASTA -> 距离矩阵
            Dim distances As DistanceMatrix = EvolutionAPI.LoadDistanceMatrix(path)
            Console.WriteLine($"  distance matrix: {distances.Size} x {distances.Size}, d(A,B)={distances(0, 1):F4}")

            ' 3. FASTA -> 进化树（Newick）
            Dim newick As String = PhyloTreeFactory.CreateNewick(TreeBuilder.Build(matrix, EvolutionAlgorithm.NeighborJoining))
            Console.WriteLine("  newick: " & newick)

            ' 4. Newick 可以被现有的 PhyloTree 解析器重新解析
            Dim reparsed As New PhyloTree("test", newick, "newick")

            If Not reparsed.treeDataValid Then
                failures.Add("FASTA workflow: 生成的 Newick 无法被 PhyloTree 解析：" & reparsed.errorMessage)
            ElseIf reparsed.LeafNodes.Count <> 5 Then
                failures.Add($"FASTA workflow: 重新解析后的叶节点数应为 5，实际 {reparsed.LeafNodes.Count}")
            End If

            ' 5. 统一 API 入口
            Dim tree As PhyloTree = EvolutionAPI.BuildEvolutionTree(path, EvolutionAlgorithm.UPGMA)

            If tree Is Nothing OrElse tree.LeafNodes.Count <> 5 Then
                failures.Add("FASTA workflow: EvolutionAPI 未能正确返回 UPGMA 树")
            End If
        Catch ex As Exception
            failures.Add("FASTA workflow: " & ex.Message)
        Finally
            If IO.File.Exists(path) Then
                Call IO.File.Delete(path)
            End If
        End Try
    End Sub

    Private Function FindLeaf(tree As PhyloNode, id As String) As PhyloNode
        Return PhyloTreeFactory _
            .EnumerateLeaves(tree) _
            .FirstOrDefault(Function(n) String.Equals(n.ID, id, StringComparison.Ordinal))
    End Function

End Module
