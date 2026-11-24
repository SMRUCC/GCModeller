Imports Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Model
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Tree

Namespace test.Tests

    ''' <summary>
    ''' 最大生成树与 Matrix-Tree 定理的数值对照。
    ''' </summary>
    ''' <remarks>
    ''' 对照基准有两个来源：
    ''' <list type="bullet">
    ''' <item><description>
    ''' <b>Cayley 公式</b>：完全图之上所有权重都为 1 时，生成树的个数为 <c>n^(n-2)</c>，
    ''' 因此 <c>Σ_T Π R_ij = n^(n-2)</c>。这是一个与实现完全无关的解析解。
    ''' </description></item>
    ''' <item><description>
    ''' <b>暴力枚举</b>：对小规模图枚举全部边子集并筛出树，作为行列式结果的独立对照。
    ''' </description></item>
    ''' </list>
    ''' </remarks>
    Module Tree

        Public Sub Run()
            Call CayleyChecks()
            Call MatrixTreeAgainstEnumeration()
            Call PrimAgainstEnumeration()
            Call RealLikelihoodMatrix()
        End Sub

        Private Function UniformLogWeights(n As Integer) As Double(,)
            Return New Double(n - 1, n - 1) {}
        End Function

        Private Sub CayleyChecks()
            For Each n As Integer In {2, 3, 4, 5}
                Dim weights As Double(,) = UniformLogWeights(n)
                Dim expected As Double = Math.Log(Math.Pow(n, n - 2))

                Call Support.Near(
                    expected,
                    MatrixTree.LogTreeSum(weights),
                    1.0E-09,
                    $"Cayley formula: the sum over all spanning trees of {n} nodes must be {n}^{n - 2}")
            Next
        End Sub

        Private Sub MatrixTreeAgainstEnumeration()
            ' 4 个节点的非平凡权重矩阵(对数形式)
            Dim ratios As Double(,) = {
                {1.0, 2.0, 3.0, 1.0},
                {2.0, 1.0, 1.0, 4.0},
                {3.0, 1.0, 1.0, 5.0},
                {1.0, 4.0, 5.0, 1.0}
            }
            Dim logWeights As Double(,) = New Double(3, 3) {}

            For i As Integer = 0 To 3
                For j As Integer = 0 To 3
                    logWeights(i, j) = Math.Log(ratios(i, j))
                Next
            Next

            Dim enumerated As Double = MatrixTree.BruteForceTreeSum(logWeights)
            Dim fromDet As Double = MatrixTree.LogTreeSum(logWeights)

            Call Support.True(enumerated > 0, "the brute force tree sum must be positive")
            Call Support.Near(Math.Log(enumerated), fromDet, 1.0E-09, "Matrix-Tree determinant versus explicit enumeration")

            ' 去掉第一行第一列之后的余子式，其行列式应当给出同样的生成树之和
            Dim kept As Integer() = {1, 2, 3}
            Dim lap As Double(,) = New Double(2, 2) {}

            For a As Integer = 0 To 2
                For b As Integer = 0 To 2
                    Dim i As Integer = kept(a)
                    Dim j As Integer = kept(b)

                    If i = j Then
                        Dim diagonal As Double = 0.0

                        For k As Integer = 0 To 3
                            If k <> i Then
                                diagonal += ratios(i, k)
                            End If
                        Next

                        lap(a, b) = diagonal
                    Else
                        lap(a, b) = -ratios(i, j)
                    End If
                Next
            Next

            Call Support.Near(
                Math.Log(enumerated),
                NumericLogSpace.LogDet(lap),
                1.0E-09,
                "removing a different row/column must give the same cofactor determinant")
        End Sub

        Private Sub PrimAgainstEnumeration()
            Dim logWeights As Double(,) = {
                {0.0, 1.0, -0.5, 2.0},
                {1.0, 0.0, 3.0, -1.0},
                {-0.5, 3.0, 0.0, 0.25},
                {2.0, -1.0, 0.25, 0.0}
            }

            Dim prim As DependencyTree = DependencyTree.Prim(logWeights)
            Dim brute As DependencyTree = DependencyTree.BruteForce(logWeights)

            Call Support.Same(3, prim.Edges.Length, "the maximum spanning tree must contain n-1 edges")
            Call Support.Near(brute.TotalLogWeight, prim.TotalLogWeight, 1.0E-09, "Prim versus brute force maximum spanning tree")

            ' 子集建树：只使用前三个节点
            Dim subset As DependencyTree = DependencyTree.Prim(logWeights, {0, 1, 2})

            Call Support.Same(2, subset.Edges.Length, "the subset tree must contain |subset|-1 edges")
            Call Support.True(subset.TotalLogWeight > 1.0, "the subset tree must pick the heavy edges 0-1 and 1-2")
        End Sub

        Private Sub RealLikelihoodMatrix()
            Dim alignment As JointAlignment = Fixtures.RandomAlignment(120, 5, 5, 7717UL)
            Dim counts As New ColumnCounts(alignment)
            Dim ratios As New LikelihoodRatioMatrix(counts)
            Dim matrix As Double(,) = ratios.ToMatrix()

            Dim prim As DependencyTree = DependencyTree.Prim(ratios)
            Dim treeSum As Double = MatrixTree.LogTreeSum(ratios)

            Call Support.Same(ratios.Size - 1, prim.Edges.Length, "the dependency tree must span every column")
            Call Support.True(Double.IsNaN(treeSum) = False, "the matrix-tree log sum must be a finite number")

            If ratios.Size <= 7 Then
                Call Support.Near(
                    Math.Log(MatrixTree.BruteForceTreeSum(matrix)),
                    treeSum,
                    1.0E-09,
                    "matrix-tree versus enumeration on a real likelihood-ratio matrix")
            End If

            ' 三棵依赖树
            Dim model As New PpiBayesianModel(alignment)
            Call Support.Same(alignment.LengthA - 1, model.BlockATree.Edges.Length, "T_k must span the family A columns")
            Call Support.Same(alignment.LengthB - 1, model.BlockBTree.Edges.Length, "T_r must span the family B columns")
        End Sub
    End Module
End Namespace
