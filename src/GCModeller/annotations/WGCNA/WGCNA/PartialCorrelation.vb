Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Solvers

''' <summary>
''' Partial correlations: the correlation of two variables after the linear
''' influence of a conditioning set has been removed.
''' <para>
''' Two equivalent computations are provided:
''' 1. <see cref="FromCovariance"/> — the precision-matrix route for a full
'''    partial-correlation matrix: p_ij = −Ω_ij / sqrt(Ω_ii · Ω_jj) with
'''    Ω = Σ⁻¹ (scaling-invariant, so a covariance or correlation input
'''    both work).
''' 2. <see cref="FromData"/> — the residual-regression route
'''    (Frisch–Waugh–Lovell) for a single pair given an arbitrary condition
'''    set; optionally robust (residuals correlated with bicor instead of
'''    Pearson).
''' </para>
''' </summary>
Public NotInheritable Class PartialCorrelation

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Full partial-correlation matrix from a covariance (or correlation)
    ''' matrix — each entry conditions on ALL remaining variables.
    ''' </summary>
    Public Shared Function FromCovariance(cov As Double(,)) As Double(,)
        Dim n = cov.GetLength(0)
        If cov.GetLength(1) <> n Then Throw New ArgumentException("matrix must be square")

        Dim omega = MatrixOps.Inverse(cov, strict:=True)
        Dim r(n - 1, n - 1) As Double
        For i = 0 To n - 1
            r(i, i) = 1.0
            For j = i + 1 To n - 1
                Dim v = -omega(i, j) / Math.Sqrt(omega(i, i) * omega(j, j))
                If v > 1.0 Then v = 1.0
                If v < -1.0 Then v = -1.0
                r(i, j) = v
                r(j, i) = v
            Next
        Next
        Return r
    End Function

    ''' <summary>
    ''' Covariance matrix (population divisor n) of a genes × samples matrix.
    ''' </summary>
    Public Shared Function CovarianceMatrix(data As Double(,)) As Double(,)
        Dim g = data.GetLength(0)
        Dim n = data.GetLength(1)
        If g < 1 OrElse n < 2 Then Throw New ArgumentException("need at least 1 gene and 2 samples")

        Dim means(g - 1) As Double
        For i = 0 To g - 1
            Dim s = 0.0
            For j = 0 To n - 1
                s += data(i, j)
            Next
            means(i) = s / n
        Next

        Dim cov(g - 1, g - 1) As Double
        For i = 0 To g - 1
            For j = i To g - 1
                Dim s = 0.0
                For t As Integer = 0 To n - 1
                    s += (data(i, t) - means(i)) * (data(j, t) - means(j))
                Next
                s /= n
                cov(i, j) = s
                cov(j, i) = s
            Next
        Next
        Return cov
    End Function

    ''' <summary>
    ''' Partial correlation of genes <paramref name="i"/> and
    ''' <paramref name="j"/> given the genes listed in
    ''' <paramref name="condition"/>: both variables are regressed on
    ''' [1 | condition] by OLS and the residuals are correlated.
    ''' </summary>
    ''' <param name="data">genes × samples expression matrix.</param>
    ''' <param name="i">first gene index.</param>
    ''' <param name="j">second gene index.</param>
    ''' <param name="condition">indices of the conditioning genes (must exclude i and j; may be Nothing).</param>
    ''' <param name="robust">True → correlate the residuals with bicor, False → Pearson.</param>
    Public Shared Function FromData(data As Double(,), i As Integer, j As Integer,
                                    condition As Integer(), Optional robust As Boolean = False) As Double
        Dim g = data.GetLength(0)
        Dim n = data.GetLength(1)
        If i < 0 OrElse i >= g OrElse j < 0 OrElse j >= g Then Throw New ArgumentOutOfRangeException(NameOf(i))
        If i = j Then Throw New ArgumentException("i and j must differ")
        If condition IsNot Nothing Then
            For Each c In condition
                If c < 0 OrElse c >= g Then Throw New ArgumentOutOfRangeException(NameOf(condition))
                If c = i OrElse c = j Then Throw New ArgumentException("condition set must exclude i and j")
            Next
        End If

        Dim p = If(condition Is Nothing, 0, condition.Length)
        Dim design(n - 1, p) As Double           ' [1 | condition]
        For s = 0 To n - 1
            design(s, 0) = 1.0
        Next
        For c = 0 To p - 1
            For s = 0 To n - 1
                design(s, c + 1) = data(condition(c), s)
            Next
        Next

        Dim xi = Row(data, i)
        Dim yj = Row(data, j)
        Dim rx = Residuals(design, xi)
        Dim ry = Residuals(design, yj)

        If robust Then Return Bicor.BiweightMidcorrelation(rx, ry)
        Return Correlations.GetPearson(rx, ry)
    End Function

    Private Shared Function Row(m As Double(,), i As Integer) As Double()
        Dim v(m.GetLength(1) - 1) As Double
        For j = 0 To v.Length - 1
            v(j) = m(i, j)
        Next
        Return v
    End Function

    Private Shared Function Residuals(design As Double(,), y As Double()) As Double()
        Dim beta = OLS.LeastSquares(design, y)
        Dim r(y.Length - 1) As Double
        For s = 0 To y.Length - 1
            Dim fit = 0.0
            For c = 0 To design.GetLength(1) - 1
                fit += design(s, c) * beta(c)
            Next
            r(s) = y(s) - fit
        Next
        Return r
    End Function

End Class
