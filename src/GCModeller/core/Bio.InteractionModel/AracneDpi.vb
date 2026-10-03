Option Strict On
Option Explicit On

Imports System.Linq

''' <summary>
''' ARACNe-style network inference step (Margolin et al., BMC Bioinformatics
''' 2006): build a mutual-information adjacency, then apply the Data
''' Processing Inequality (DPI) triplet filter.
''' <para>
''' DPI: in a triangle (i, j, k), if x_i regulates x_j through x_k then
''' MI(i,j) &lt; MI(i,k) and MI(i,j) &lt; MI(j,k); the weakest edge is therefore
''' likely indirect. An edge is removed only when it is more than
''' <c>tolerance</c> (default 0.1, ARACNe's ε) below BOTH other edges —
''' near-ties are kept.
''' </para>
''' </summary>
Public NotInheritable Class AracneDpi

    Private Sub New()
    End Sub

    ''' <summary>Outcome of a DPI filter pass.</summary>
    Public NotInheritable Class DpiResult

        ''' <summary>Adjacency AFTER the filter (symmetric, diagonal false).</summary>
        Public Property Adjacency As Boolean(,)

        ''' <summary>Removed edges as {i, j} index pairs (i &lt; j).</summary>
        Public Property RemovedEdges As New List(Of Integer())

        Public Property TrianglesExamined As Integer
        Public Property EdgesBefore As Integer
        Public Property EdgesAfter As Integer

    End Class

    ''' <summary>
    ''' Gaussian-equivalent mutual information from a correlation matrix:
    ''' MI_ij = −½·ln(1 − r²) — exact for jointly Gaussian variables, which
    ''' makes it the deterministic ARACNe surrogate for linear-Gaussian
    ''' synthetic data. Diagonal is 0.
    ''' </summary>
    Public Shared Function GaussianMutualInformation(correlation As Double(,)) As Double(,)
        Dim n = correlation.GetLength(0)
        If correlation.GetLength(1) <> n Then Throw New ArgumentException("matrix must be square")

        Dim mi(n - 1, n - 1) As Double
        For i = 0 To n - 1
            mi(i, i) = 0.0
            For j = i + 1 To n - 1
                Dim r = correlation(i, j)
                If r > 1.0 Then r = 1.0
                If r < -1.0 Then r = -1.0
                Dim rr = r * r
                Dim v = If(rr < 1.0, -0.5 * Math.Log(1.0 - rr), 0.0)
                mi(i, j) = v
                mi(j, i) = v
            Next
        Next
        Return mi
    End Function

    ''' <summary>
    ''' Equal-frequency binned plug-in mutual information with the
    ''' Miller–Madow bias correction — a distribution-free alternative for
    ''' non-Gaussian dependence.
    ''' </summary>
    ''' <param name="x">First sample.</param>
    ''' <param name="y">Second sample (same length).</param>
    ''' <param name="bins">Number of quantile bins per variable (default 4).</param>
    Public Shared Function BinnedMutualInformation(x As Double(), y As Double(),
                                                    Optional bins As Integer = 4) As Double
        Dim n = x.Length
        If y.Length <> n Then Throw New ArgumentException("length mismatch")
        If bins < 2 Then Throw New ArgumentOutOfRangeException(NameOf(bins))
        If n < 4 * bins Then Throw New ArgumentException("need at least 4 samples per bin for a stable estimate")

        Dim bx(n - 1) As Integer
        Dim by(n - 1) As Integer
        FillEqualFrequencyBins(x, bx, bins)
        FillEqualFrequencyBins(y, by, bins)

        Dim joint(bins - 1, bins - 1) As Double
        For s = 0 To n - 1
            joint(bx(s), by(s)) += 1.0
        Next

        Dim px(bins - 1) As Double, py(bins - 1) As Double
        For a = 0 To bins - 1
            For b = 0 To bins - 1
                px(a) += joint(a, b)
                py(b) += joint(b, a)
            Next
        Next

        Dim mi = 0.0
        For a = 0 To bins - 1
            If px(a) = 0.0 Then Continue For
            For b = 0 To bins - 1
                If joint(a, b) = 0.0 OrElse py(b) = 0.0 Then Continue For
                Dim pxy = joint(a, b) / n
                mi += pxy * Math.Log(pxy / ((px(a) / n) * (py(b) / n)))
            Next
        Next

        ' Miller–Madow bias correction: (m_x − 1)(m_y − 1) / (2n)
        Dim mx = 0, myCount = 0
        For a = 0 To bins - 1
            If px(a) > 0.0 Then mx += 1
            If py(a) > 0.0 Then myCount += 1
        Next
        mi += (mx - 1) * (myCount - 1) / (2.0 * n)

        Return Math.Max(mi, 0.0)
    End Function

    ''' <summary>
    ''' DPI triplet filter. The adjacency is <c>weights(i,j) &gt; threshold</c>
    ''' (symmetric, diagonal ignored); removals are all evaluated against the
    ''' ORIGINAL adjacency (single pass, like the reference ARACNe code).
    ''' For every triangle, the weakest edge is removed when
    ''' w_min &lt; (1 − tolerance)·w_other1 AND w_min &lt; (1 − tolerance)·w_other2.
    ''' </summary>
    ''' <param name="weights">symmetric non-negative weight matrix (e.g. MI).</param>
    ''' <param name="threshold">edge inclusion threshold.</param>
    ''' <param name="tolerance">ARACNe ε (default 0.1): near-ties survive.</param>
    Public Shared Function Filter(weights As Double(,), threshold As Double,
                                  Optional tolerance As Double = 0.1) As DpiResult
        Dim n = weights.GetLength(0)
        If weights.GetLength(1) <> n Then Throw New ArgumentException("weight matrix must be square")
        If tolerance < 0.0 OrElse tolerance >= 1.0 Then Throw New ArgumentOutOfRangeException(NameOf(tolerance))

        Dim adj(n - 1, n - 1) As Boolean
        Dim before = 0
        For i = 0 To n - 1
            For j = i + 1 To n - 1
                If weights(i, j) > threshold Then
                    adj(i, j) = True
                    adj(j, i) = True
                    before += 1
                End If
            Next
        Next

        Dim result As New DpiResult With {
            .Adjacency = adj,
            .EdgesBefore = before,
            .EdgesAfter = before
        }

        ' one flag per edge so several triangles condemning the same edge
        ' only count once (the weakest edge may be weakest in many triangles)
        Dim marked(n - 1, n - 1) As Boolean

        ' (edge index, weight) of the three triangle edges
        For i = 0 To n - 3
            For j = i + 1 To n - 2
                If Not adj(i, j) Then Continue For
                For kk = j + 1 To n - 1
                    If Not adj(i, kk) OrElse Not adj(j, kk) Then Continue For

                    result.TrianglesExamined += 1

                    Dim eij = New Integer() {i, j}
                    Dim eik = New Integer() {i, kk}
                    Dim ejk = New Integer() {j, kk}
                    Dim wij = weights(i, j)
                    Dim wik = weights(i, kk)
                    Dim wjk = weights(j, kk)

                    ' identify the weakest edge and the two competitors
                    Dim wMin = wij
                    Dim eMin = eij
                    If wik < wMin Then wMin = wik : eMin = eik
                    If wjk < wMin Then wMin = wjk : eMin = ejk

                    Dim o1 = If(ReferenceEquals(eMin, eij), wik, wij)
                    Dim o2 = If(ReferenceEquals(eMin, eij), wjk, If(ReferenceEquals(eMin, eik), wij, wik))

                    If wMin < (1.0 - tolerance) * o1 AndAlso wMin < (1.0 - tolerance) * o2 AndAlso
                       Not marked(eMin(0), eMin(1)) Then
                        marked(eMin(0), eMin(1)) = True
                        marked(eMin(1), eMin(0)) = True
                        result.RemovedEdges.Add(New Integer() {eMin(0), eMin(1)})
                    End If
                Next
            Next
        Next

        ' apply removals
        For Each e In result.RemovedEdges
            result.Adjacency(e(0), e(1)) = False
            result.Adjacency(e(1), e(0)) = False
            result.EdgesAfter -= 1
        Next

        Return result
    End Function

    Private Shared Sub FillEqualFrequencyBins(v As Double(), b As Integer(), bins As Integer)
        Dim n = v.Length
        ' ordinal ranks → quantile bins
        Dim idx = Enumerable.Range(0, n).OrderBy(Function(t) v(t)).ToArray()
        For pos = 0 To n - 1
            Dim bi = CInt(Math.Floor(pos * bins / CDbl(n)))
            If bi > bins - 1 Then bi = bins - 1
            b(idx(pos)) = bi
        Next
    End Sub

End Class
