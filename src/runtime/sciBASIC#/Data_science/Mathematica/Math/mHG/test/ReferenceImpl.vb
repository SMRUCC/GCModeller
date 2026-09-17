Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' An independent (naive) reference implementation of the minimum-hypergeometric test.
'''
''' Nothing in this module uses any helper of the ``mHG.vbproj`` project, so that the results
''' of the library under test can be validated against a completely separate computation path:
'''
''' + the hypergeometric distribution is evaluated with log-factorials (log-sum-exp) instead
'''   of the ratio-based dynamic programming used by the library;
''' + the p-value is obtained by brute force enumeration of all the ``C(N, B)`` rearrangements
'''   of the ranked binary list instead of the R-separation-line / ``pi_r`` dynamic programming.
''' </summary>
Public Module ReferenceImpl

    Public Const EPSILON# = 0.0000000001

#Region "Hypergeometric distribution"

    Private ReadOnly logFactorialCache As New Dictionary(Of Integer, Double)

    ''' <summary>
    ''' ``log(n!)``
    ''' </summary>
    Public Function LogFactorial(n As Integer) As Double
        If n < 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(n))
        End If
        If n <= 1 Then
            Return 0.0
        End If

        Dim cached As Double = 0

        If logFactorialCache.TryGetValue(n, cached) Then
            Return cached
        End If

        cached = LogFactorial(n - 1) + System.Math.Log(CDbl(n))
        logFactorialCache(n) = cached

        Return cached
    End Function

    ''' <summary>
    ''' ``log(C(n, k))``, or ``-Inf`` for the out of range ``k``.
    ''' </summary>
    Public Function LogChoose(n As Integer, k As Integer) As Double
        If k < 0 OrElse k > n OrElse n < 0 Then
            Return Double.NegativeInfinity
        End If
        Return LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k)
    End Function

    ''' <summary>
    ''' The hypergeometric probability mass function ``P(X = b)`` for the urn
    ''' containing ``N`` balls among which ``B`` are black, ``n`` draws.
    ''' </summary>
    Public Function HypergeometricPmf(b As Integer, N As Integer, B As Integer, n As Integer) As Double
        Dim lo As Integer = System.Math.Max(0, n - (N - B))
        Dim hi As Integer = System.Math.Min(n, B)

        If b < lo OrElse b > hi Then
            Return 0.0
        End If

        Return System.Math.Exp(LogChoose(B, b) + LogChoose(N - B, n - b) - LogChoose(N, n))
    End Function

    ''' <summary>
    ''' The hypergeometric tail ``P(X &gt;= b)``, which equals to the R expression
    ''' ``phyper(b - 1, B, N - B, n, lower.tail = FALSE)``.
    ''' </summary>
    Public Function HypergeometricTail(b As Integer, N As Integer, B As Integer, n As Integer) As Double
        Dim lo As Integer = System.Math.Max(b, 0)
        Dim hi As Integer = System.Math.Min(n, B)

        If lo > hi Then
            Return 0.0
        End If

        Dim logs As New List(Of Double)

        For k As Integer = lo To hi
            logs.Add(LogChoose(B, k) + LogChoose(N - B, n - k) - LogChoose(N, n))
        Next

        ' numeric stable log-sum-exp
        Dim maxLog As Double = logs.Max
        Dim sum As Double = 0

        For Each x As Double In logs
            sum += System.Math.Exp(x - maxLog)
        Next

        Dim total As Double = System.Math.Exp(maxLog) * sum

        If total > 1.0 Then
            total = 1.0
        ElseIf total < 0.0 Then
            total = 0.0
        End If

        Return total
    End Function

#End Region

#Region "mHG statistic (direct definition)"

    ''' <summary>
    ''' The mHG statistic computed straight from its definition, without any dynamic programming:
    '''
    ''' ``min over 1 &lt;= n &lt;= n_max of P(X &gt;= b_n)``, where ``b_n`` is the number of
    ''' ones among the first ``n`` elements of the ranked list.
    ''' </summary>
    Public Function mHGStatisticSimple(lambdas As Integer(), Optional n_max As Integer = -1) As (mhg As Double, n As Integer, b As Integer)
        Dim N As Integer = lambdas.Length
        Dim B As Integer = lambdas.Sum

        If n_max < 0 Then
            n_max = N
        End If

        Dim mhg As Double = 1
        Dim bestN As Integer = 0
        Dim bestB As Integer = 0
        Dim b As Integer = 0

        For n As Integer = 1 To n_max
            b += lambdas(n - 1)

            Dim hgt As Double = HypergeometricTail(b, N, B, n)

            If hgt < mhg Then
                mhg = hgt
                bestN = n
                bestB = b
            End If
        Next

        Return (mhg, bestN, bestB)
    End Function

#End Region

#Region "Exact permutation p-value (brute force)"

    ''' <summary>
    ''' Enumerates all the ``k``-subsets of ``{0, 1, ..., n - 1}`` in lexicographic order.
    ''' </summary>
    Public Iterator Function Combinations(n As Integer, k As Integer) As IEnumerable(Of Integer())
        If k < 0 OrElse k > n Then
            Return
        End If

        Dim idx As Integer() = Enumerable.Range(0, k).ToArray

        While True
            Yield idx.ToArray

            Dim i As Integer = k - 1

            While i >= 0 AndAlso idx(i) = i + n - k
                i -= 1
            End While

            If i < 0 Then
                Exit While
            End If

            idx(i) += 1

            For j As Integer = i + 1 To k - 1
                idx(j) = idx(j - 1) + 1
            Next
        End While
    End Function

    ''' <summary>
    ''' The exact mHG p-value obtained by enumerating every rearrangement of the ranked binary
    ''' list: ``count(statistic &lt;= observed) / C(N, B)``.
    ''' </summary>
    Public Function ExactPermutationPValue(lambdas As Integer(), Optional n_max As Integer = -1) As Double
        Dim N As Integer = lambdas.Length
        Dim B As Integer = lambdas.Sum

        If n_max < 0 Then
            n_max = N
        End If

        Dim observed As Double = mHGStatisticSimple(lambdas, n_max).mhg
        Dim count As Long = 0
        Dim total As Long = 0

        For Each ones As Integer() In Combinations(N, B)
            Dim arrangement As Integer() = New Integer(N - 1) {}

            For Each i As Integer In ones
                arrangement(i) = 1
            Next

            Dim statistic As Double = mHGStatisticSimple(arrangement, n_max).mhg

            If statistic <= (observed + EPSILON) Then
                count += 1
            End If

            total += 1
        Next

        Return CDbl(count) / CDbl(total)
    End Function

#End Region

#Region "Deterministic test data generator"

    ''' <summary>
    ''' A tiny deterministic linear congruential generator, used to make the demo/random test
    ''' cases reproducible across runs (``System.Random`` may vary between runtime versions).
    ''' </summary>
    Public Class Lcg

        Private state As Long

        Public Sub New(seed As Long)
            state = seed And &H7FFFFFFFL
        End Sub

        Public Function [Next](bound As Integer) As Integer
            state = (state * 1103515245L + 12345L) And &H7FFFFFFFL
            Return CInt(state Mod bound)
        End Function

        Public Function NextDouble() As Double
            Return [Next](1000000) / 1000000.0
        End Function
    End Class

    ''' <summary>
    ''' shuffle ``B`` ones into a list of ``N`` elements.
    ''' </summary>
    Public Function RandomLambdas(N As Integer, B As Integer, rnd As Lcg) As Integer()
        Dim x As Integer() = New Integer(N - 1) {}

        For i As Integer = 0 To B - 1
            x(i) = 1
        Next

        For i As Integer = N - 1 To 1 Step -1
            Dim j As Integer = rnd.Next(i + 1)
            Dim tmp As Integer = x(i)
            x(i) = x(j)
            x(j) = tmp
        Next

        Return x
    End Function

    ''' <summary>
    ''' A "biased towards the top" ranked binary list, simulating a GO term whose annotated
    ''' genes are enriched among the top ranked (e.g. differential expressed) genes.
    ''' </summary>
    Public Function EnrichedLambdas(N As Integer, B As Integer, rnd As Lcg) As Integer()
        Dim x As Integer() = New Integer(N - 1) {}
        Dim placed As Integer = 0
        Dim i As Integer = 0

        While placed < B AndAlso i < N
            Dim p As Double = If(i < (N \ 2), 0.35, 0.15)

            If rnd.NextDouble() < p Then
                x(i) = 1
                placed += 1
            End If

            i += 1
        End While

        Dim k As Integer = N - 1

        While placed < B AndAlso k >= 0
            If x(k) = 0 Then
                x(k) = 1
                placed += 1
            End If

            k -= 1
        End While

        Return x
    End Function

#End Region

End Module
