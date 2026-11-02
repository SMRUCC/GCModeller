Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork

Namespace Bayesian.Statistics

    ''' <summary>
    ''' Dirichlet-多项模型的边际似然。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 论文把每个比对列看作一个离散随机变量，其取值分布 <c>ω</c> 服从 Dirichlet 先验，
    ''' 然后把 <c>ω</c> 积分掉，得到只依赖于计数的边际似然(Gamma 函数形式)：
    ''' </para>
    ''' <code>
    ''' P(D_i)   = Γ(Σ_α λ_α) / Γ(n + Σ_α λ_α) · Π_α Γ(n_i(α) + λ_α) / Γ(λ_α)
    ''' P(D_ij)  = Γ(Σ_αβ λ'_αβ) / Γ(n + Σ_αβ λ'_αβ) · Π_αβ Γ(n_ij(αβ) + λ'_αβ) / Γ(λ'_αβ)
    ''' </code>
    ''' <para>
    ''' 先验取 Jeffreys 先验 <c>λ' = 1/2</c>，并且满足一致性约束 <c>λ = K · λ'</c>
    ''' (K 为字母表符号个数，论文写作 <c>λ = 21 λ'</c>)。
    ''' </para>
    ''' <para>
    ''' 似然比 <c>R_ij = P(D_ij) / (P(D_i) P(D_j))</c> 在大量计数时正比于两列之间的互信息，
    ''' 而在小样本时自动包含有限样本修正并惩罚依赖模型多出来的自由参数 —— 这是该方法
    ''' 不需要任何可调参数的原因。
    ''' </para>
    ''' </remarks>
    Public Class DirichletMultinomial

        Private ReadOnly _alphabetSize As Integer
        Private _lambda As Double = 0.5

        ''' <summary>
        ''' 使用指定的字母表大小构造模型
        ''' </summary>
        Public Sub New(alphabetSize As Integer, Optional lambda As Double = 0.5)
            If alphabetSize < 2 Then
                Throw New ArgumentOutOfRangeException(NameOf(alphabetSize), "the alphabet must contain at least 2 symbols")
            End If
            If lambda <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(lambda), "the Dirichlet prior must be positive")
            End If

            _alphabetSize = alphabetSize
            _lambda = lambda
        End Sub

        ''' <summary>
        ''' 字母表之中符号的个数 K
        ''' </summary>
        Public ReadOnly Property AlphabetSize As Integer
            Get
                Return _alphabetSize
            End Get
        End Property

        ''' <summary>
        ''' 成对联合分布的 Dirichlet 先验参数 <c>λ'</c>，默认 0.5(Jeffreys 先验)
        ''' </summary>
        Public Property Lambda As Double
            Get
                Return _lambda
            End Get
            Set(value As Double)
                If value <= 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), "the Dirichlet prior must be positive")
                End If

                _lambda = value
            End Set
        End Property

        ''' <summary>
        ''' 单列分布的先验参数 <c>λ = K · λ'</c>
        ''' </summary>
        Public ReadOnly Property LambdaSingle As Double
            Get
                Return _lambda * _alphabetSize
            End Get
        End Property

        ''' <summary>
        ''' 计算单列的边际似然 <c>ln P(D_i)</c>
        ''' </summary>
        ''' <param name="counts">该列每个符号的计数 <c>n_i(α)</c></param>
        Public Function LogMarginal(counts As Integer()) As Double
            If counts Is Nothing OrElse counts.Length <> _alphabetSize Then
                Throw New ArgumentException($"expected {_alphabetSize} residue counts", NameOf(counts))
            End If

            Dim lambda As Double = LambdaSingle
            Dim n As Double = 0.0
            Dim total As Double = NumericLogSpace.LogGamma(lambda * _alphabetSize)

            For a As Integer = 0 To counts.Length - 1
                Dim na As Double = counts(a)

                If na > 0 Then
                    total += NumericLogSpace.LogGamma(na + lambda) - NumericLogSpace.LogGamma(lambda)
                End If

                n += na
            Next

            Return total - NumericLogSpace.LogGamma(n + lambda * _alphabetSize)
        End Function

        ''' <summary>
        ''' 计算成对列的联合边际似然 <c>ln P(D_ij)</c>
        ''' </summary>
        ''' <param name="joint">联合计数，键为 <c>α · K + β</c></param>
        ''' <param name="total">该列对的计数总数(等于样本数)</param>
        Public Function LogMarginalPair(joint As Dictionary(Of Integer, Integer), total As Integer) As Double
            Dim categories As Integer = _alphabetSize * _alphabetSize
            Dim result As Double = NumericLogSpace.LogGamma(_lambda * categories)
            Dim n As Double = 0.0

            For Each entry As KeyValuePair(Of Integer, Integer) In joint
                Dim count As Double = entry.Value

                If count > 0 Then
                    result += NumericLogSpace.LogGamma(count + _lambda) - NumericLogSpace.LogGamma(_lambda)
                    n += count
                End If
            Next

            If n <= 0 Then
                n = total
            End If

            Return result - NumericLogSpace.LogGamma(n + _lambda * categories)
        End Function

        ''' <summary>
        ''' 计算两列之间的对数似然比 <c>ln R_ij</c>
        ''' </summary>
        Public Function LogLikelihoodRatio(counts As ColumnCounts, i As Integer, j As Integer) As Double
            Dim logPair As Double = LogMarginalPair(counts.PairCounts(i, j), counts.PairTotal(i, j))
            Dim logI As Double = LogMarginal(counts.SingleCounts(i))
            Dim logJ As Double = LogMarginal(counts.SingleCounts(j))

            Return logPair - logI - logJ
        End Function

        ''' <summary>
        ''' 计算对数似然比，计数直接从比对之中现算(用于只关心少量列对的情形)
        ''' </summary>
        Public Function LogLikelihoodRatio(alignment As JointAlignment, i As Integer, j As Integer) As Double
            Dim rows As Integer = alignment.Rows
            Dim k As Integer = _alphabetSize
            Dim singleI As Integer() = New Integer(k - 1) {}
            Dim singleJ As Integer() = New Integer(k - 1) {}
            Dim joint As New Dictionary(Of Integer, Integer)

            For r As Integer = 0 To rows - 1
                Dim a As Integer = alignment(r, i)
                Dim b As Integer = alignment(r, j)

                singleI(a) += 1
                singleJ(b) += 1

                Dim code As Integer = a * k + b
                Dim n As Integer = 0

                If joint.TryGetValue(code, n) Then
                    joint(code) = n + 1
                Else
                    joint(code) = 1
                End If
            Next

            Return LogMarginalPair(joint, rows) - LogMarginal(singleI) - LogMarginal(singleJ)
        End Function
    End Class
End Namespace
