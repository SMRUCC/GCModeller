Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Distance
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.TreeSearch
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview
Imports NjBuilder = SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.NeighborJoining.NeighborJoining

Namespace Evolution.Parsimony

    ''' <summary>
    ''' 最大简约法（Maximum Parsimony）的建树结果。
    ''' </summary>
    Public Class ParsimonyResult
        ''' <summary>
        ''' 找到的最简约树拓扑
        ''' </summary>
        Public Property Tree As PhyloNode
        ''' <summary>
        ''' 树长（所有信息位点所需的最少替换数之和）
        ''' </summary>
        Public Property Score As Integer
        ''' <summary>
        ''' 参与计算的信息位点数目
        ''' </summary>
        Public Property InformativeSites As Integer
        ''' <summary>
        ''' 启发式搜索的迭代轮次
        ''' </summary>
        Public Property Iterations As Integer
    End Class

    ''' <summary>
    ''' 最大简约法（MP）：在所有候选树拓扑中，寻找能用最少替换事件解释观测位点模式的树。
    ''' 仅使用**信息位点**，通过 Fitch 算法（自底向上的动态规划）计算树长，
    ''' 并以 NJ 树为初始拓扑，使用 NNI / SPR 算子进行启发式爬山搜索。
    ''' </summary>
    Public Module MaximumParsimony

        ''' <summary>
        ''' 计算指定树在给定信息位点集合上的 Fitch 树长。
        ''' </summary>
        Public Function FitchScore(root As PhyloNode, matrix As CharacterMatrix, sites As Integer()) As Integer
            Dim index As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To matrix.SequenceCount - 1
                If Not index.ContainsKey(matrix.Names(i)) Then
                    index(matrix.Names(i)) = i
                End If
            Next

            Dim total As Integer = 0

            For Each site As Integer In sites
                Dim evaluator As New FitchEvaluator(matrix, site, index)
                total += evaluator.Score(root)
            Next

            Return total
        End Function

        ''' <summary>
        ''' 执行最大简约法建树。
        ''' </summary>
        ''' <param name="matrix">已比对的位点矩阵</param>
        ''' <param name="initial">可选的初始树；默认使用基于 p 距离的 NJ 树</param>
        ''' <param name="maxIterations">启发式搜索的最大迭代轮次（默认为 100）</param>
        ''' <param name="useSpr">是否在 NNI 无法改进时尝试 SPR 移动</param>
        ''' <param name="seed">SPR 随机采样的种子</param>
        Public Function Build(matrix As CharacterMatrix,
                              Optional initial As PhyloNode = Nothing,
                              Optional maxIterations As Integer = 0,
                              Optional useSpr As Boolean = True,
                              Optional seed As Integer = 12345) As ParsimonyResult

            Dim sites As Integer() = matrix.InformativeSites()

            If sites.Length = 0 Then
                Throw New ArgumentException("比对矩阵之中没有信息位点（informative site），无法执行最大简约法！")
            End If

            If initial Is Nothing Then
                initial = NjBuilder.Build(matrix, DistanceModel.PDistance)
            End If

            If maxIterations <= 0 Then
                maxIterations = 100
            End If

            Console.WriteLine($"[MP] informative sites: {sites.Length}")

            Dim current As PhyloNode = initial
            Dim best As Integer = FitchScore(current, matrix, sites)
            Dim rand As New Random(seed)
            Dim iter As Integer = 0

            Console.WriteLine($"[MP] initial tree length: {best}")

            While iter < maxIterations
                Dim improved As Boolean = False

                ' ---- 第一阶段：NNI 爬山 ----
                Dim bestNni As NniMove? = Nothing
                Dim bestNniTree As PhyloNode = Nothing
                Dim bestNniScore As Integer = best

                For Each move As NniMove In TreeRearrangement.NniMoves(current)
                    Dim trial As PhyloNode = current.Clone()
                    Call TreeRearrangement.ApplyNni(trial, move)
                    Dim score As Integer = FitchScore(trial, matrix, sites)

                    If score < bestNniScore Then
                        bestNniScore = score
                        bestNni = move
                        bestNniTree = trial
                    End If
                Next

                If bestNni.HasValue Then
                    current = bestNniTree
                    best = bestNniScore
                    improved = True
                End If

                ' ---- 第二阶段：SPR 爬山 ----
                If Not improved AndAlso useSpr Then
                    Dim bestSprScore As Integer = best
                    Dim bestSprTree As PhyloNode = Nothing

                    For Each move As SprMove In TreeRearrangement.SprMoves(current, maxCount:=60, rand:=rand)
                        Dim trial As PhyloNode = current.Clone()
                        Call TreeRearrangement.ApplySpr(trial, move)
                        Dim score As Integer = FitchScore(trial, matrix, sites)

                        If score < bestSprScore Then
                            bestSprScore = score
                            bestSprTree = trial
                        End If
                    Next

                    If bestSprTree IsNot Nothing Then
                        current = bestSprTree
                        best = bestSprScore
                        improved = True
                    End If
                End If

                iter += 1

                If Not improved Then
                    Exit While
                End If

                Console.WriteLine($"[MP] iteration {iter}: tree length -> {best}")
            End While

            Console.WriteLine($"[MP] final tree length: {best}, iterations: {iter}")

            Return New ParsimonyResult With {
                .Tree = current,
                .Score = best,
                .InformativeSites = sites.Length,
                .Iterations = iter
            }
        End Function

        ''' <summary>
        ''' Fitch 算法的单次位点求值器（对某个特定位点，自底向上累计最少替换数）。
        ''' </summary>
        Private Class FitchEvaluator

            Private ReadOnly _matrix As CharacterMatrix
            Private ReadOnly _site As Integer
            Private ReadOnly _index As Dictionary(Of String, Integer)
            Private ReadOnly _fullMask As Integer
            Private _steps As Integer

            Public Sub New(matrix As CharacterMatrix, site As Integer, index As Dictionary(Of String, Integer))
                Me._matrix = matrix
                Me._site = site
                Me._index = index
                Me._fullMask = (1 << matrix.CharacterSet.Size) - 1
            End Sub

            ''' <summary>
            ''' 返回该位点在给定树上的最少替换步数。
            ''' </summary>
            Public Function Score(root As PhyloNode) As Integer
                Me._steps = 0
                Call __fitch(root)

                Return Me._steps
            End Function

            Private Function __fitch(node As PhyloNode) As Integer
                If node.Descendents.Count = 0 Then
                    Dim i As Integer

                    If _index.TryGetValue(node.ID, i) Then
                        Dim s As Integer = _matrix.States(i)(_site)

                        If s < 0 Then
                            Return _fullMask
                        Else
                            Return 1 << s
                        End If
                    Else
                        Return _fullMask
                    End If
                End If

                Dim masks As New List(Of Integer)(node.Descendents.Count)
                Dim inter As Integer = -1

                For Each child As PhyloNode In node.Descendents
                    Dim m As Integer = __fitch(child)
                    masks.Add(m)

                    If inter = -1 Then
                        inter = m
                    Else
                        inter = inter And m
                    End If
                Next

                If inter <> 0 Then
                    Return inter
                End If

                ' 交集为空：取并集，并累计一次替换事件
                Dim union As Integer = 0

                For Each m As Integer In masks
                    union = union Or m
                Next

                Me._steps += 1

                Return union
            End Function
        End Class
    End Module

End Namespace
