Option Strict On
Option Explicit On

''' <summary>
''' Biweight midcorrelation (bicor) and classic Pearson correlation.
''' <para>
''' bicor follows the definition used by WGCNA (Langfelder &amp; Horvath,
''' 2012): median/MAD-based Tukey biweights
''' w = (1 - u²)² with u = (x - median)/(c·MAD), tuning constant c = 9, and
''' </para>
''' <code>
''' bicor(x,y) = Σ w_x·w_y·(x-Mx)(y-My) /
'''              sqrt( Σ w_x²·(x-Mx)² · Σ w_y²·(y-My)² )
''' </code>
''' <para>
''' The squared weights in the denominator are the biweight midcovariance
''' with itself (bicov(x,x) = Σ w_x²(x-Mx)²), which keeps the estimator
''' affine-equivariant: bicor(x, a·x+b) = sign(a) exactly, like a true
''' correlation.
''' </para>
''' <para>
''' Outliers receive vanishing weights, which is what makes the estimator
''' robust. When MAD = 0 (a nearly constant vector) the pair falls back to
''' Pearson (WGCNA's <c>pearsonFallback = "all"</c>) so the correlation
''' stays defined.
''' </para>
''' </summary>
Public NotInheritable Class Bicor

    ''' <summary>Tukey tuning constant; 9 is the WGCNA default.</summary>
    Public Const DefaultConstant As Double = 9.0

    Private Sub New()
    End Sub

    ''' <summary>Median of a sample (empty samples are rejected).</summary>
    Public Shared Function Median(values As Double()) As Double
        If values Is Nothing OrElse values.Length = 0 Then Throw New ArgumentException("empty sample")
        Dim s = CType(values.Clone(), Double())
        Array.Sort(s)
        Dim n = s.Length
        If n Mod 2 = 1 Then Return s(n \ 2)
        Return 0.5 * (s(n \ 2 - 1) + s(n \ 2))
    End Function

    ''' <summary>
    ''' Median absolute deviation, scaled with 1.4826 to be a consistent
    ''' estimator of the standard deviation for Gaussian noise.
    ''' </summary>
    Public Shared Function Mad(values As Double()) As Double
        Dim med = Median(values)
        Dim dev(values.Length - 1) As Double
        For i = 0 To values.Length - 1
            dev(i) = Math.Abs(values(i) - med)
        Next
        Return 1.4826 * Median(dev)
    End Function

    ''' <summary>
    ''' Classic Pearson correlation. Zero-variance inputs return 0
    ''' (undefined correlation represented neutrally).
    ''' </summary>
    Public Shared Function Pearson(x As Double(), y As Double()) As Double
        Dim n = x.Length
        If y.Length <> n Then Throw New ArgumentException("length mismatch")
        Dim mx = 0.0, my = 0.0
        For i = 0 To n - 1
            mx += x(i)
            my += y(i)
        Next
        mx /= n
        my /= n
        Dim sxy = 0.0, sxx = 0.0, syy = 0.0
        For i = 0 To n - 1
            Dim dx = x(i) - mx
            Dim dy = y(i) - my
            sxy += dx * dy
            sxx += dx * dx
            syy += dy * dy
        Next
        If sxx <= 0.0 OrElse syy <= 0.0 Then Return 0.0
        Return sxy / Math.Sqrt(sxx * syy)
    End Function

    ''' <summary>Biweight midcorrelation — see the class remarks for the formula.</summary>
    ''' <param name="x">First sample.</param>
    ''' <param name="y">Second sample (same length).</param>
    ''' <param name="constant">Tukey tuning constant c (default 9).</param>
    ''' <param name="pearsonFallback">Fall back to Pearson when MAD = 0 (WGCNA behaviour).</param>
    Public Shared Function BiweightMidcorrelation(x As Double(), y As Double(),
                                                   Optional constant As Double = DefaultConstant,
                                                   Optional pearsonFallback As Boolean = True) As Double
        Dim n = x.Length
        If y.Length <> n Then Throw New ArgumentException("length mismatch")
        If constant <= 0.0 Then Throw New ArgumentOutOfRangeException(NameOf(constant))

        Dim madX = Mad(x)
        Dim madY = Mad(y)
        If madX <= 0.0 OrElse madY <= 0.0 Then
            Return If(pearsonFallback, Pearson(x, y), Double.NaN)
        End If

        Dim mx = Median(x)
        Dim my = Median(y)

        Dim wx(n - 1) As Double, wy(n - 1) As Double
        Dim xc(n - 1) As Double, yc(n - 1) As Double
        Dim anyWeightX = False, anyWeightY = False
        For i = 0 To n - 1
            Dim ux = (x(i) - mx) / (constant * madX)
            Dim uy = (y(i) - my) / (constant * madY)
            If Math.Abs(ux) < 1.0 Then
                wx(i) = (1.0 - ux * ux) * (1.0 - ux * ux)
                anyWeightX = True
            End If
            If Math.Abs(uy) < 1.0 Then
                wy(i) = (1.0 - uy * uy) * (1.0 - uy * uy)
                anyWeightY = True
            End If
            xc(i) = x(i) - mx
            yc(i) = y(i) - my
        Next

        If Not anyWeightX OrElse Not anyWeightY Then
            Return If(pearsonFallback, Pearson(x, y), Double.NaN)
        End If

        Dim num = 0.0, dx2 = 0.0, dy2 = 0.0
        For i = 0 To n - 1
            num += wx(i) * wy(i) * xc(i) * yc(i)
            dx2 += wx(i) * wx(i) * xc(i) * xc(i)
            dy2 += wy(i) * wy(i) * yc(i) * yc(i)
        Next
        If dx2 <= 0.0 OrElse dy2 <= 0.0 Then
            Return If(pearsonFallback, Pearson(x, y), Double.NaN)
        End If

        Dim r = num / Math.Sqrt(dx2 * dy2)
        If r > 1.0 Then r = 1.0
        If r < -1.0 Then r = -1.0
        Return r
    End Function

    ''' <summary>
    ''' Symmetric gene × gene correlation matrix from expression data
    ''' (rows = genes, columns = samples). Only the upper triangle is
    ''' computed; the diagonal is set to exactly 1.
    ''' </summary>
    ''' <param name="data">genes × samples expression matrix.</param>
    ''' <param name="robust">True → bicor, False → Pearson.</param>
    ''' <param name="pearsonFallback">Passed through to <see cref="BiweightMidcorrelation"/>.</param>
    Public Shared Function CorrelationMatrix(data As Double(,), Optional robust As Boolean = True,
                                              Optional pearsonFallback As Boolean = True) As Double(,)
        Dim g = data.GetLength(0)
        Dim n = data.GetLength(1)
        If g < 1 OrElse n < 3 Then Throw New ArgumentException("need at least 1 gene and 3 samples")

        Dim cols As New List(Of Double())(capacity:=g)
        For i = 0 To g - 1
            Dim col(n - 1) As Double
            For j = 0 To n - 1
                col(j) = data(i, j)
            Next
            cols.Add(col)
        Next

        Dim r(g - 1, g - 1) As Double
        For i = 0 To g - 1
            r(i, i) = 1.0
            For j = i + 1 To g - 1
                Dim v = If(robust,
                           BiweightMidcorrelation(cols(i), cols(j), DefaultConstant, pearsonFallback),
                           Pearson(cols(i), cols(j)))
                r(i, j) = v
                r(j, i) = v
            Next
        Next
        Return r
    End Function

End Class
