' ============================================================================
'  MathUtils.vb - Basic mathematical functions used across all modules
'  Traitar Microbial Trait Analyzer - VB.NET Implementation
'
'  All algorithms rely only on VB.NET's built-in Math functions (System.Math).
'  No external numerical libraries are required.
' ============================================================================
Imports System
Imports System.Collections.Generic

Namespace Utils

    ''' <summary>
    ''' Pure-math helpers: Pearson correlation, sigmoid, sign, softmax-like
    ''' normalization, and small linear-algebra primitives used by the SVM.
    ''' </summary>
    Public Module MathUtils

        ''' <summary>Sign function: -1, 0, or +1.</summary>
        Public Function Sign(x As Double) As Integer
            If x > 0.0R Then Return 1
            If x < 0.0R Then Return -1
            Return 0
        End Function

        ''' <summary>Numerically stable logistic sigmoid 1 / (1 + e^-x).</summary>
        Public Function Sigmoid(x As Double) As Double
            If x >= 0.0R Then
                Dim z As Double = Math.Exp(-x)
                Return 1.0R / (1.0R + z)
            Else
                Dim z As Double = Math.Exp(x)
                Return z / (1.0R + z)
            End If
        End Function

        ''' <summary>
        ''' Pearson product-moment correlation between a binary feature vector x
        ''' and a binary label vector y. Used by Module 7 to rank key features.
        ''' Returns a value in [-1, 1]; returns 0 when either vector is constant.
        ''' </summary>
        Public Function PearsonCorrelation(x As IList(Of Double), y As IList(Of Double)) As Double
            If x Is Nothing OrElse y Is Nothing Then Return 0.0R
            If x.Count <> y.Count OrElse x.Count = 0 Then Return 0.0R
            Dim n As Integer = x.Count
            Dim sumX As Double = 0.0R, sumY As Double = 0.0R
            Dim sumXY As Double = 0.0R, sumX2 As Double = 0.0R, sumY2 As Double = 0.0R
            For i As Integer = 0 To n - 1
                sumX += x(i) : sumY += y(i)
                sumXY += x(i) * y(i)
                sumX2 += x(i) * x(i)
                sumY2 += y(i) * y(i)
            Next
            Dim meanX As Double = sumX / n
            Dim meanY As Double = sumY / n
            Dim cov As Double = sumXY / n - meanX * meanY
            Dim varX As Double = sumX2 / n - meanX * meanX
            Dim varY As Double = sumY2 / n - meanY * meanY
            Dim denom As Double = Math.Sqrt(varX * varY)
            If denom = 0.0R Then Return 0.0R
            Return cov / denom
        End Function

        ''' <summary>Overload for Integer arrays (binary feature/label vectors).</summary>
        Public Function PearsonCorrelation(x As IList(Of Integer), y As IList(Of Integer)) As Double
            If x Is Nothing OrElse y Is Nothing Then Return 0.0R
            If x.Count <> y.Count OrElse x.Count = 0 Then Return 0.0R
            Dim xd(x.Count - 1) As Double
            Dim yd(y.Count - 1) As Double
            For i As Integer = 0 To x.Count - 1
                xd(i) = CDbl(x(i))
                yd(i) = CDbl(y(i))
            Next
            Return PearsonCorrelation(xd, yd)
        End Function

        ''' <summary>
        ''' Soft-thresholding operator S(z, gamma) = sign(z) * max(|z| - gamma, 0).
        ''' This is the proximal operator of the L1 norm and the core update
        ''' step in coordinate-descent training of L1-regularized SVMs.
        ''' </summary>
        Public Function SoftThreshold(z As Double, gamma As Double) As Double
            If z > gamma Then Return z - gamma
            If z < -gamma Then Return z + gamma
            Return 0.0R
        End Function

        ''' <summary>
        ''' Joint probability that a gain OR loss event occurred on a branch,
        ''' given independent posterior probabilities g (gain) and l (loss):
        '''   x = g + l - g * l
        ''' Used by Module 3 to extend the sample set with ancestral events.
        ''' </summary>
        Public Function JointGainLoss(g As Double, l As Double) As Double
            Return g + l - g * l
        End Function

        ''' <summary>
        ''' Linear-interpolation helper, used when discretizing ancestral
        ''' posterior probabilities at threshold t (default 0.5).
        ''' Returns 1 if p &gt;= t, 0 if p &lt;= 1 - t, and NaN (uncertain) otherwise.
        ''' </summary>
        Public Function DiscretizeWithThreshold(p As Double, t As Double) As Double
            If p >= t Then Return 1.0R
            If p <= 1.0R - t Then Return 0.0R
            Return Double.NaN   ' uncertain -> sample is dropped
        End Function

        ''' <summary>Euclidean norm of a vector.</summary>
        Public Function L2Norm(v As IList(Of Double)) As Double
            Dim s As Double = 0.0R
            For Each x As Double In v
                s += x * x
            Next
            Return Math.Sqrt(s)
        End Function

        ''' <summary>L1 norm of a vector.</summary>
        Public Function L1Norm(v As IList(Of Double)) As Double
            Dim s As Double = 0.0R
            For Each x As Double In v
                s += Math.Abs(x)
            Next
            Return s
        End Function

        ''' <summary>Dot product of two equal-length vectors.</summary>
        Public Function DotProduct(a As IList(Of Double), b As IList(Of Double)) As Double
            If a Is Nothing OrElse b Is Nothing Then Return 0.0R
            Dim n As Integer = Math.Min(a.Count, b.Count)
            Dim s As Double = 0.0R
            For i As Integer = 0 To n - 1
                s += a(i) * b(i)
            Next
            Return s
        End Function

    End Module

End Namespace
