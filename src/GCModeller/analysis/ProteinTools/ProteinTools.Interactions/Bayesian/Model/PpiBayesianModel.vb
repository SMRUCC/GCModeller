Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Tree

Namespace Bayesian.Model

    ''' <summary>
    ''' 蛋白质互作预测的贝叶斯模型(Burger &amp; van Nimwegen 2008 的 General model)。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 模型把联合比对的每一列当作一个离散节点，节点之间的联合分布由 Dirichlet-多项模型给出。
    ''' 给定一个「分配」<c>a</c>(即联合比对的具体配对方式)，数据的边缘似然是
    ''' </para>
    ''' <code>
    ''' P(D | a) = [ Π_i P(D_i) ] · Σ_T Π_(i,j)∈T R_ij
    ''' </code>
    ''' <para>
    ''' 其中对全部生成树的求和由 <see cref="MatrixTree"/> 用行列式计算；
    ''' 当 <see cref="PpiModelOptions.UseMatrixTree"/> 关闭的时候使用论文给出的近似
    ''' <c>P(D|a) ≈ P(D|a, T*)</c>，<c>T*</c> 为以 <c>ln R_ij</c> 为边权的最大生成树。
    ''' </para>
    ''' <para>
    ''' <b>关于分配的不变性</b>：交换同一基因组内两个成员的家族 B 段之后，
    ''' 单列计数以及家族内部(含 B-B)的列对计数都保持不变，只有跨家族的列对计数会改变。
    ''' 因此采样过程中只需调用 <see cref="ApplySwap"/> 做增量更新，然后 <see cref="Refresh"/> 重新建树即可。
    ''' </para>
    ''' </remarks>
    Public Class PpiBayesianModel

        Private ReadOnly _options As PpiModelOptions
        Private ReadOnly _counts As ColumnCounts
        Private ReadOnly _logR As LikelihoodRatioMatrix
        Private _fullTree As DependencyTree = Nothing
        Private _blockATree As DependencyTree = Nothing
        Private _blockBTree As DependencyTree = Nothing
        Private _base As Double
        Private _baseComputed As Boolean = False

        ''' <summary>
        ''' 由联合比对构建模型
        ''' </summary>
        Public Sub New(alignment As JointAlignment, Optional options As PpiModelOptions = Nothing)
            If alignment Is Nothing Then
                Throw New ArgumentNullException(NameOf(alignment))
            End If

            _options = If(options, New PpiModelOptions())
            _counts = New ColumnCounts(alignment, _options.IncludeWithinBlockPairs)
            _logR = New LikelihoodRatioMatrix(_counts, _options.CreatePrior(_counts.AlphabetSize))
        End Sub

        ''' <summary>
        ''' 模型配置
        ''' </summary>
        Public ReadOnly Property Options As PpiModelOptions
            Get
                Return _options
            End Get
        End Property

        ''' <summary>
        ''' 当前的联合比对
        ''' </summary>
        Public ReadOnly Property Alignment As JointAlignment
            Get
                Return _counts.Alignment
            End Get
        End Property

        ''' <summary>
        ''' 列计数
        ''' </summary>
        Public ReadOnly Property Counts As ColumnCounts
            Get
                Return _counts
            End Get
        End Property

        ''' <summary>
        ''' 似然比矩阵 <c>R_ij</c>
        ''' </summary>
        Public ReadOnly Property LogR As LikelihoodRatioMatrix
            Get
                Return _logR
            End Get
        End Property

        ''' <summary>
        ''' 三棵依赖树之中的 <c>T*</c>(全部位置)
        ''' </summary>
        Public ReadOnly Property FullTree As DependencyTree
            Get
                If _fullTree Is Nothing Then
                    _fullTree = DependencyTree.Prim(_logR)
                End If

                Return _fullTree
            End Get
        End Property

        ''' <summary>
        ''' 只含家族 A 位置的依赖树 <c>T_k</c>
        ''' </summary>
        Public ReadOnly Property BlockATree As DependencyTree
            Get
                If _blockATree Is Nothing Then
                    If _counts.Alignment.LengthA = 0 Then
                        _blockATree = DependencyTree.Prim(_logR, New Integer() {})
                    Else
                        _blockATree = DependencyTree.Prim(_logR, Enumerable.Range(0, _counts.Alignment.LengthA).ToArray())
                    End If
                End If

                Return _blockATree
            End Get
        End Property

        ''' <summary>
        ''' 只含家族 B 位置的依赖树 <c>T_r</c>
        ''' </summary>
        Public ReadOnly Property BlockBTree As DependencyTree
            Get
                If _blockBTree Is Nothing Then
                    Dim alignment As JointAlignment = _counts.Alignment

                    If alignment.LengthB = 0 Then
                        _blockBTree = DependencyTree.Prim(_logR, New Integer() {})
                    Else
                        _blockBTree = DependencyTree.Prim(_logR, Enumerable.Range(alignment.LengthA, alignment.LengthB).ToArray())
                    End If
                End If

                Return _blockBTree
            End Get
        End Property

        ''' <summary>
        ''' 单列边际似然之积的对数值 <c>Σ_i ln P(D_i)</c>，即 <c>P(D|a,T)</c> 之中与树无关的部分
        ''' </summary>
        Public ReadOnly Property LogSingleColumnBase As Double
            Get
                If Not _baseComputed Then
                    Dim total As Double = 0.0
                    Dim prior As DirichletMultinomial = _logR.Prior

                    For i As Integer = 0 To _counts.Length - 1
                        total += prior.LogMarginal(_counts.SingleCounts(i))
                    Next

                    _base = total
                    _baseComputed = True
                End If

                Return _base
            End Get
        End Property

        ''' <summary>
        ''' 当前分配之下的对数边缘似然 <c>ln P(D|a)</c>
        ''' </summary>
        Public Function LogEvidence() As Double
            If _options.UseMatrixTree Then
                Return LogDetEvidence()
            Else
                Return LogSingleColumnBase + FullTree.TotalLogWeight
            End If
        End Function

        ''' <summary>
        ''' 使用 Matrix-Tree 行列式精确计算的 <c>ln P(D|a)</c>
        ''' </summary>
        Public Function LogDetEvidence() As Double
            Return LogSingleColumnBase + MatrixTree.LogTreeSum(_logR)
        End Function

        ''' <summary>
        ''' 只考虑给定的列集合时的对数边缘似然
        ''' <c>ln P(D_set) = Σ_(i∈set) ln P(D_i) + ln Σ_T Π_(i,j)∈T R_ij</c>
        ''' </summary>
        ''' <param name="columns">列下标的集合</param>
        ''' <remarks>
        ''' 这个子集似然是 orphan 推广模型之中 <c>P(K|J_k) = P(K ∪ J_k) / P(J_k)</c> 这类
        ''' 条件概率的计算基础。
        ''' </remarks>
        Public Function LogEvidenceSubset(columns As Integer()) As Double
            If columns Is Nothing OrElse columns.Length = 0 Then
                Return 0.0
            End If

            Dim prior As DirichletMultinomial = _logR.Prior
            Dim total As Double = 0.0

            For Each c As Integer In columns
                total += prior.LogMarginal(_counts.SingleCounts(c))
            Next

            If _options.UseMatrixTree Then
                total += MatrixTree.LogTreeSum(_logR, columns)
            Else
                total += DependencyTree.Prim(_logR, columns).TotalLogWeight
            End If

            Return total
        End Function

        ''' <summary>
        ''' 在给定归一化常数之下计算对数后验 <c>ln P(a|D)</c>
        ''' </summary>
        ''' <param name="logNormalizer">所有候选分配的对数配分函数</param>
        Public Function LogPosterior(logNormalizer As Double) As Double
            Return LogEvidence() - logNormalizer
        End Function

        ''' <summary>
        ''' 交换同一基因组内两个成员的家族 B 段，并增量更新计数
        ''' </summary>
        ''' <returns>交换之后的联合比对</returns>
        Public Function ApplySwap(row1 As Integer, row2 As Integer) As JointAlignment
            Dim updated As JointAlignment = _counts.ApplySwap(row1, row2)

            _logR.InvalidateCrossBlock(updated.LengthA)
            Call Refresh()

            Return updated
        End Function

        ''' <summary>
        ''' 丢弃依赖树的缓存，在计数发生变化之后必须调用
        ''' </summary>
        Public Sub Refresh()
            _fullTree = Nothing
            _blockATree = Nothing
            _blockBTree = Nothing
        End Sub

        ''' <summary>
        ''' 检查模型的内部一致性(用于调试审计)
        ''' </summary>
        Public Function Validate() As Boolean
            Dim ok As Boolean = True

            ok = DebugAudit.Test(_counts.Alignment.Rows > 1,
                                 "at least two sequences are required to estimate a dependency model") AndAlso ok
            ok = DebugAudit.Test(_counts.Alignment.LengthA > 0 AndAlso _counts.Alignment.LengthB > 0,
                                 "both protein families must contribute at least one alignment column") AndAlso ok
            ok = DebugAudit.Test(FullTree.Edges.Length = Math.Max(0, _counts.Length - 1),
                                 $"the dependency tree should contain {_counts.Length - 1} edges but got {FullTree.Edges.Length}") AndAlso ok

            Return ok
        End Function

        Public Overrides Function ToString() As String
            Return $"PpiBayesianModel[{Alignment}]"
        End Function
    End Class
End Namespace
