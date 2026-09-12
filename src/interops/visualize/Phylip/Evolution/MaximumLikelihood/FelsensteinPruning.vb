Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview

Namespace Evolution.MaximumLikelihood

    ''' <summary>
    ''' Felsenstein 剪枝（pruning / peeling）似然计算引擎。
    ''' </summary>
    ''' <remarks>
    ''' 对每个位点：叶节点按观测状态赋指示向量，内部节点依据
    ''' <c>L_u(i) = Π_children [ Σ_j P_ij(t) L_child(j) ]</c> 自底向上计算，
    ''' 根节点似然为 <c>L = Σ_i π_i L_root(i)</c>，全树对数似然为各位点对数似然之和。
    '''
    ''' 实现上采用对数域缩放（Felsenstein scaling）：每计算完一个内部节点即用其最大分量归一，
    ''' 并把缩放因子的对数累加，避免深树上的数值下溢。
    ''' 当配置了离散 Gamma 速率时，位点似然为各速率类别似然的加权和。
    ''' </remarks>
    Public Class FelsensteinPruning

        Private ReadOnly _matrix As CharacterMatrix
        Private ReadOnly _model As SubstitutionModel
        Private ReadOnly _gamma As DiscreteGamma
        Private ReadOnly _index As Dictionary(Of String, Integer)
        Private ReadOnly _sites As Integer()
        Private ReadOnly _n As Integer
        Private ReadOnly _pi As Double()

        Public ReadOnly Property Model As SubstitutionModel
            Get
                Return _model
            End Get
        End Property

        Public ReadOnly Property Sites As Integer()
            Get
                Return _sites
            End Get
        End Property

        Public Sub New(matrix As CharacterMatrix,
                       model As SubstitutionModel,
                       Optional gamma As DiscreteGamma = Nothing,
                       Optional sites As Integer() = Nothing)

            Me._matrix = matrix
            Me._model = model
            Me._gamma = gamma
            Me._n = model.Dimension
            Me._pi = model.Pi
            Me._sites = If(sites, matrix.AllSites())

            Me._index = New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To matrix.SequenceCount - 1
                If Not _index.ContainsKey(matrix.Names(i)) Then
                    _index(matrix.Names(i)) = i
                End If
            Next
        End Sub

        ''' <summary>
        ''' 计算整棵树在所有位点上的对数似然。
        ''' </summary>
        Public Function LogLikelihood(root As PhyloNode) As Double
            Dim cache As New Dictionary(Of Double, Double()())
            Dim total As Double = 0

            For Each site As Integer In _sites
                total += LogLikelihoodSite(root, site, cache)
            Next

            Return total
        End Function

        ''' <summary>
        ''' 计算各个位点的对数似然（用于调试或诊断）。
        ''' </summary>
        Public Function SiteLogLikelihoods(root As PhyloNode) As Double()
            Dim cache As New Dictionary(Of Double, Double()())
            Dim result(_sites.Length - 1) As Double

            For k As Integer = 0 To _sites.Length - 1
                result(k) = LogLikelihoodSite(root, _sites(k), cache)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 计算单个位点的对数似然（含离散 Gamma 速率类别的加权求和）。
        ''' </summary>
        Public Function LogLikelihoodSite(root As PhyloNode, site As Integer, cache As Dictionary(Of Double, Double()())) As Double
            Dim rates As Double() = {1.0}
            Dim weights As Double() = {1.0}

            If _gamma IsNot Nothing Then
                rates = _gamma.Rates
                weights = _gamma.Weights
            End If

            Dim terms(rates.Length - 1) As Double

            For c As Integer = 0 To rates.Length - 1
                Dim logScale As Double = 0
                Dim vector As Double() = PartialLikelihood(root, site, rates(c), cache, logScale)

                Dim L As Double = 0

                For i As Integer = 0 To _n - 1
                    L += _pi(i) * vector(i)
                Next

                If L <= 0 Then
                    terms(c) = Double.NegativeInfinity
                Else
                    terms(c) = Math.Log(L) + logScale + Math.Log(weights(c))
                End If
            Next

            Return LogSumExp(terms)
        End Function

        ''' <summary>
        ''' 自底向上计算指定节点在给定位点、给定速率下的部分似然向量。
        ''' </summary>
        Private Function PartialLikelihood(node As PhyloNode,
                                            site As Integer,
                                            rate As Double,
                                            cache As Dictionary(Of Double, Double()()),
                                            ByRef logScale As Double) As Double()

            If node.Descendents.Count = 0 Then
                Dim vector(_n - 1) As Double
                Dim i As Integer

                If _index.TryGetValue(node.ID, i) Then
                    Dim state As Integer = _matrix.States(i)(site)

                    If state >= 0 Then
                        vector(state) = 1
                    Else
                        ' 缺失/未知状态：不提供任何信息
                        For k As Integer = 0 To _n - 1
                            vector(k) = 1
                        Next
                    End If
                Else
                    For k As Integer = 0 To _n - 1
                        vector(k) = 1
                    Next
                End If

                Return vector
            End If

            Dim result(_n - 1) As Double

            For k As Integer = 0 To _n - 1
                result(k) = 1
            Next

            For Each child As PhyloNode In node.Descendents
                Dim t As Double = child.BranchLength * rate
                Dim p As Double()() = GetTransition(t, cache)
                Dim childVector As Double() = PartialLikelihood(child, site, rate, cache, logScale)

                For i As Integer = 0 To _n - 1
                    Dim sum As Double = 0
                    Dim pi_ As Double() = p(i)

                    For j As Integer = 0 To _n - 1
                        If childVector(j) <> 0 Then
                            sum += pi_(j) * childVector(j)
                        End If
                    Next

                    result(i) *= sum
                Next
            Next

            ' Felsenstein scaling：用最大分量归一，累加缩放因子
            Dim maxValue As Double = 0

            For k As Integer = 0 To _n - 1
                If result(k) > maxValue Then
                    maxValue = result(k)
                End If
            Next

            If maxValue <= 0 Then
                Return result
            End If

            For k As Integer = 0 To _n - 1
                result(k) /= maxValue
            Next

            logScale += Math.Log(maxValue)

            Return result
        End Function

        Private Function GetTransition(t As Double, cache As Dictionary(Of Double, Double()())) As Double()()
            Dim p As Double()() = Nothing

            If cache.TryGetValue(t, p) Then
                Return p
            End If

            p = _model.TransitionProbability(t)
            cache(t) = p

            Return p
        End Function

        ''' <summary>
        ''' 对数空间下的 log-sum-exp，用于稳定地合并各速率类别的似然贡献。
        ''' </summary>
        Public Shared Function LogSumExp(values As Double()) As Double
            Dim maxValue As Double = Double.NegativeInfinity

            For Each v As Double In values
                If v > maxValue Then
                    maxValue = v
                End If
            Next

            If Double.IsNegativeInfinity(maxValue) Then
                Return Double.NegativeInfinity
            End If

            Dim sum As Double = 0

            For Each v As Double In values
                If Not Double.IsNegativeInfinity(v) Then
                    sum += Math.Exp(v - maxValue)
                End If
            Next

            Return maxValue + Math.Log(sum)
        End Function
    End Class

End Namespace
