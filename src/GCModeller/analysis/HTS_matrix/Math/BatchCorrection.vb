Option Strict On
Option Explicit On

''' <summary>
''' Batch-effect correction for gene-expression matrices
''' (rows = genes/features, columns = samples).
''' <para>
''' Two strategies are provided:
''' 1. <see cref="RemoveBatchEffect"/> — the limma idea (Ritchie et al. 2015):
'''    fit y = covariates + batch + noise per gene by OLS and return
'''    covariates + noise, i.e. subtract only the fitted batch part.
''' 2. <see cref="ComBat"/> — the location/scale empirical-Bayes model of
'''    Johnson, Li &amp; Rabinovic (Biostatistics 2007), parametric version:
'''    standardise the data, shrink the per-batch offsets (γ) and scales (δ)
'''    towards their cross-batch priors, then back-transform.
''' </para>
''' <para>
''' Covariates that must be protected from removal (phenotype groups, etc.)
''' are supplied as a samples × c matrix and enter the model like sva's
''' <c>mod</c> argument.
''' </para>
''' </summary>
Public NotInheritable Class BatchCorrection

    Private Sub New()
    End Sub

    ' ---------------------------------------------------------------- helpers

    Private Shared Function BatchLevels(labels As String()) As List(Of String)
        Dim lv As New List(Of String)
        For Each s In labels
            If Not lv.Contains(s) Then lv.Add(s)
        Next
        Return lv
    End Function

    Private Shared Sub Validate(expr As Double(,), batchLabels As String(), covariates As Double(,),
                                ByRef genes As Integer, ByRef n As Integer, ByRef levels As List(Of String))
        genes = expr.GetLength(0)
        n = expr.GetLength(1)
        If batchLabels Is Nothing OrElse batchLabels.Length <> n Then
            Throw New ArgumentException("exactly one batch label per sample is required")
        End If
        levels = BatchLevels(batchLabels)
        If levels.Count < 2 Then Throw New ArgumentException("need at least two different batches")
        For Each lv In levels
            Dim cnt = 0
            For j = 0 To n - 1
                If batchLabels(j) = lv Then cnt += 1
            Next
            If cnt < 2 Then Throw New ArgumentException($"batch '{lv}' has fewer than 2 samples")
        Next
        If covariates IsNot Nothing AndAlso covariates.GetLength(0) <> n Then
            Throw New ArgumentException("covariate rows must match the sample count")
        End If
    End Sub

    ''' <summary>Row of a genes × samples matrix as a flat vector.</summary>
    Private Shared Function Row(m As Double(,), i As Integer) As Double()
        Dim v(m.GetLength(1) - 1) As Double
        For j = 0 To v.Length - 1
            v(j) = m(i, j)
        Next
        Return v
    End Function

    ' ------------------------------------------------------------- limma route

    ''' <summary>
    ''' limma-style batch removal: per gene, fit
    ''' y = intercept + covariates + batch-dummies + noise by OLS and return
    ''' intercept + covariates + noise. All batch means end up equal while
    ''' the covariate structure is kept intact.
    ''' </summary>
    ''' <param name="expr">genes × samples expression matrix.</param>
    ''' <param name="batchLabels">one label per sample.</param>
    ''' <param name="covariates">optional samples × c matrix of covariates to preserve (no intercept column).</param>
    Public Shared Function RemoveBatchEffect(expr As Double(,), batchLabels As String(),
                                             Optional covariates As Double(,) = Nothing) As Double(,)
        Dim genes, n As Integer
        Dim levels As List(Of String) = Nothing
        Validate(expr, batchLabels, covariates, genes, n, levels)

        Dim k = levels.Count
        Dim nCov = If(covariates Is Nothing, 0, covariates.GetLength(1))

        ' design = [1 | covariates | batch dummies (k-1, first batch = reference)]
        Dim p = 1 + nCov + (k - 1)
        Dim design(n - 1, p - 1) As Double
        For j = 0 To n - 1
            design(j, 0) = 1.0
            For a = 0 To nCov - 1
                design(j, 1 + a) = covariates(j, a)
            Next
            Dim lvl = levels.IndexOf(batchLabels(j))
            If lvl >= 1 Then design(j, 1 + nCov + lvl - 1) = 1.0
        Next

        Dim result(genes - 1, n - 1) As Double
        For g = 0 To genes - 1
            Dim y = Row(expr, g)
            Dim beta = MatrixOps.LeastSquares(design, y)

            For j = 0 To n - 1
                ' fitted batch part (intercept + covariates + noise stay in y)
                Dim batchFit = 0.0
                For m = 0 To k - 2
                    batchFit += beta(1 + nCov + m) * design(j, 1 + nCov + m)
                Next
                ' keep everything except the batch part
                result(g, j) = y(j) - batchFit
            Next
        Next
        Return result
    End Function

    ' ------------------------------------------------------------ ComBat route

    ''' <summary>
    ''' ComBat empirical-Bayes batch correction (Johnson, Li &amp; Rabinovic,
    ''' 2007), parametric version, following the reference sva implementation:
    ''' per-gene OLS on [batch one-hot | covariates], standardisation, then
    ''' iterative shrinkage of γ (additive) and δ² (multiplicative) towards
    ''' their cross-gene/cross-batch empirical priors, and back-transform.
    ''' </summary>
    ''' <param name="expr">genes × samples expression matrix.</param>
    ''' <param name="batchLabels">one label per sample.</param>
    ''' <param name="covariates">optional samples × c matrix of covariates to preserve (no intercept column).</param>
    Public Shared Function ComBat(expr As Double(,), batchLabels As String(),
                                   Optional covariates As Double(,) = Nothing) As Double(,)
        Dim genes, n As Integer
        Dim levels As List(Of String) = Nothing
        Validate(expr, batchLabels, covariates, genes, n, levels)

        Dim k = levels.Count
        Dim nCov = If(covariates Is Nothing, 0, covariates.GetLength(1))

        ' sample indices per batch
        Dim members(k - 1) As List(Of Integer)
        For b = 0 To k - 1
            members(b) = New List(Of Integer)
        Next
        For j = 0 To n - 1
            members(levels.IndexOf(batchLabels(j))).Add(j)
        Next

        ' design = [batch one-hot (k cols, no intercept) | covariates]
        Dim p = k + nCov
        Dim design(n - 1, p - 1) As Double
        For j = 0 To n - 1
            design(j, levels.IndexOf(batchLabels(j))) = 1.0
            For a = 0 To nCov - 1
                design(j, k + a) = covariates(j, a)
            Next
        Next

        ' ---------- per-gene OLS ----------
        Dim betaHat(genes - 1, p - 1) As Double
        For g = 0 To genes - 1
            Dim beta = MatrixOps.LeastSquares(design, Row(expr, g))
            For c = 0 To p - 1
                betaHat(g, c) = beta(c)
            Next
        Next

        ' ---------- grand mean α and pooled residual variance ----------
        Dim alpha(genes - 1) As Double
        Dim sigma2(genes - 1) As Double
        For g = 0 To genes - 1
            For b = 0 To k - 1
                alpha(g) += members(b).Count / CDbl(n) * betaHat(g, b)
            Next
            For j = 0 To n - 1
                Dim fitted = 0.0
                For c = 0 To p - 1
                    fitted += design(j, c) * betaHat(g, c)
                Next
                Dim e = expr(g, j) - fitted
                sigma2(g) += e * e
            Next
            sigma2(g) /= n
        Next

        ' ---------- standardise ----------
        ' z = (y − α − covariate contribution) / σ̂
        Dim z(genes - 1, n - 1) As Double
        Dim constant(genes - 1) As Boolean
        For g = 0 To genes - 1
            If sigma2(g) <= 1.0E-12 Then
                constant(g) = True          ' pass this gene through unchanged
                Continue For
            End If
            For j = 0 To n - 1
                Dim standMean = alpha(g)
                For a = 0 To nCov - 1
                    standMean += covariates(j, a) * betaHat(g, k + a)
                Next
                z(g, j) = (expr(g, j) - standMean) / Math.Sqrt(sigma2(g))
            Next
        Next

        ' ---------- per-batch γ̂ and δ̂² on standardised data ----------
        Dim gammaHat(k - 1, genes - 1) As Double
        Dim deltaHat(k - 1, genes - 1) As Double
        For b = 0 To k - 1
            Dim nb = members(b).Count
            For g = 0 To genes - 1
                If constant(g) Then Continue For
                Dim s = 0.0
                For Each j In members(b)
                    s += z(g, j)
                Next
                gammaHat(b, g) = s / nb
                Dim ss = 0.0
                For Each j In members(b)
                    Dim d = z(g, j) - gammaHat(b, g)
                    ss += d * d
                Next
                deltaHat(b, g) = ss / (nb - 1)         ' sample variance (like R's var)
            Next
        Next

        ' ---------- empirical priors ----------
        Dim gammaBar(genes - 1) As Double            ' across batches, per gene
        Dim tau2(genes - 1) As Double
        For g = 0 To genes - 1
            Dim s = 0.0
            For b = 0 To k - 1
                s += gammaHat(b, g)
            Next
            gammaBar(g) = s / k
            Dim ss = 0.0
            For b = 0 To k - 1
                Dim d = gammaHat(b, g) - gammaBar(g)
                ss += d * d
            Next
            tau2(g) = ss / (k - 1)
        Next

        ' inverse-gamma prior (λ, θ) on δ², per batch, by moments across genes
        Dim lambdaPrior(k - 1) As Double
        Dim thetaPrior(k - 1) As Double
        For b = 0 To k - 1
            Dim m = 0.0
            For g = 0 To genes - 1
                m += deltaHat(b, g)
            Next
            m /= genes
            Dim v = 0.0
            For g = 0 To genes - 1
                Dim d = deltaHat(b, g) - m
                v += d * d
            Next
            v /= (genes - 1)
            If v < 1.0E-12 Then v = 1.0E-12
            lambdaPrior(b) = (2.0 * v + m * m) / v
            thetaPrior(b) = (m * v + m * m * m) / v
        Next

        ' ---------- empirical-Bayes shrinkage (it.sol) ----------
        Dim gammaStar(k - 1, genes - 1) As Double
        Dim deltaStar(k - 1, genes - 1) As Double
        For b = 0 To k - 1
            Dim nb = members(b).Count
            For g = 0 To genes - 1
                If constant(g) Then Continue For
                Dim gs = gammaHat(b, g)
                Dim ds = Math.Max(deltaHat(b, g), 1.0E-12)
                For iteration = 1 To 100
                    ' γ* = (τ²·n_b·γ̂ + δ*·γ̄) / (τ²·n_b + δ*)
                    Dim gsNew = (tau2(g) * nb * gammaHat(b, g) + ds * gammaBar(g)) / (tau2(g) * nb + ds)
                    ' δ*² = (½·Σ(z − γ*)² + θ) / (n_b/2 + λ − 1)
                    Dim sum2 = 0.0
                    For Each j In members(b)
                        Dim d = z(g, j) - gsNew
                        sum2 += d * d
                    Next
                    Dim dsNew = (0.5 * sum2 + thetaPrior(b)) / (nb / 2.0 + lambdaPrior(b) - 1.0)
                    If dsNew < 1.0E-12 Then dsNew = 1.0E-12

                    Dim converged = Math.Abs(gsNew - gs) < 1.0E-6 AndAlso Math.Abs(dsNew - ds) < 1.0E-6
                    gs = gsNew
                    ds = dsNew
                    If converged Then Exit For
                Next
                gammaStar(b, g) = gs
                deltaStar(b, g) = ds
            Next
        Next

        ' ---------- adjust and back-transform ----------
        ' y* = σ̂ · (z − γ*)/√δ* + α + covariate contribution
        Dim result(genes - 1, n - 1) As Double
        For g = 0 To genes - 1
            If constant(g) Then
                For j = 0 To n - 1
                    result(g, j) = expr(g, j)
                Next
                Continue For
            End If
            For j = 0 To n - 1
                Dim b = levels.IndexOf(batchLabels(j))
                Dim zAdj = (z(g, j) - gammaStar(b, g)) / Math.Sqrt(deltaStar(b, g))
                Dim standMean = alpha(g)
                For a = 0 To nCov - 1
                    standMean += covariates(j, a) * betaHat(g, k + a)
                Next
                result(g, j) = zAdj * Math.Sqrt(sigma2(g)) + standMean
            Next
        Next
        Return result
    End Function

End Class
