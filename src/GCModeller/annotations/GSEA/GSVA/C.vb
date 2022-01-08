Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports stdNum = System.Math

Module C

    Public Function matrix_density_R(X As Double()(),
                                     Y As Double()(),
                                     R As Double()(),
                                     n_density_samples As Integer,
                                     n_test_samples As Integer,
                                     n_genes As Integer,
                                     rnaseq As Integer)

        For j As Integer = 0 To n_genes - 1
            Dim offset_density = j * n_density_samples
            Dim offset_test = j * n_test_samples

            row_d(X(offset_density), Y(offset_test), R(offset_test), n_density_samples, n_test_samples, rnaseq)
        Next
    End Function

    Const SIGMA_FACTOR = 4.0
    Const PRECOMPUTE_RESOLUTION = 10000
    Const MAX_PRECOMPUTE = 10.0

    Dim is_precomputed As Integer = 0
    Dim precomputed_cdf As Double() = New Double(PRECOMPUTE_RESOLUTION - 1) {}

    ''' <summary>
    ''' for resampling, x are the resampled points and y are the
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="r"></param>
    ''' <param name="size_density_n"></param>
    ''' <param name="size_test_n"></param>
    ''' <param name="rnaseq"></param>
    ''' <returns></returns>
    Private Function row_d(x As Double(),
                           y As Double(),
                           r As Double(),
                           size_density_n As Integer,
                           size_test_n As Integer,
                           rnaseq As Integer)

        Dim bw As Double = If(rnaseq, 0.5, (sd1(x, size_density_n) / SIGMA_FACTOR))
        Dim left_tail As Double

        If Not rnaseq AndAlso is_precomputed = 0 Then
            Call initCdfs()
            is_precomputed = 1
        End If

        For j As Integer = 0 To size_test_n - 1
            left_tail = 0

            For i As Integer = 0 To size_density_n - 1
                left_tail += If(rnaseq, ppois(y(j), x(i) + bw, True, False), _precomputed_cdf(y(j) - x(i), bw))
            Next

            left_tail = left_tail / size_density_n
            r(j) = -1 * stdNum.Log((1 - left_tail) / left_tail)
        Next
    End Function

    Private Function _precomputed_cdf(x As Double, sigma As Double) As Double
        Dim v As Double = x / sigma

        If v < -1 * MAX_PRECOMPUTE Then
            Return 0
        ElseIf v > MAX_PRECOMPUTE Then
            Return 1
        Else
            Dim cdf As Double = precomputed_cdf(stdNum.Abs(v) / MAX_PRECOMPUTE * PRECOMPUTE_RESOLUTION)

            If v < 0 Then
                Return 1 - cdf
            Else
                Return cdf
            End If
        End If
    End Function

    Private Function ppois(y As Double, x As Double, f1 As Boolean, f2 As Boolean) As Double
        Throw New NotImplementedException
    End Function

    Private Sub initCdfs()
        Dim divisor = PRECOMPUTE_RESOLUTION * 1.0

        For i As Integer = 0 To PRECOMPUTE_RESOLUTION - 1
            precomputed_cdf(i) = pnorm.Eval(MAX_PRECOMPUTE * i / divisor, 0.0, 1.0, lower_tail:=True, logP:=False)
        Next
    End Sub

    ''' <summary>
    ''' calculates standard deviation, largely borrowed from C code in R's src/main/cov.c */
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="n"></param>
    ''' <returns></returns>
    Private Function sd1(x As Double(), n As Integer) As Double
        Dim n1 As Integer
        Dim mean, sd As Double
        Dim sum As Double = 0.0
        Dim tmp As Double

        For i As Integer = 0 To n - 1
            sum += x(i)
        Next

        tmp = sum / n

        If Not tmp.IsNaNImaginary Then
            sum = 0

            For i As Integer = 0 To n - 1
                sum += x(i) - tmp
            Next

            tmp = tmp + sum / n
        End If

        mean = tmp
        n1 = n - 1
        sum = 0

        For i As Integer = 0 To n - 1
            sum += (x(i) - mean) ^ 2
        Next

        sd = stdNum.Sqrt(sum / n1)

        Return sd
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="X">gene density scores</param>
    ''' <param name="R">result</param>
    ''' <param name="sidxs">sorted gene densities idxs</param>
    ''' <param name="n_genes"></param>
    ''' <param name="geneset_idxs"></param>
    ''' <param name="n_geneset"></param>
    ''' <param name="tau"></param>
    ''' <param name="n_samples"></param>
    ''' <param name="mx_diff"></param>
    ''' <param name="abs_rnk"></param>
    ''' <returns></returns>
    Friend Function ks_matrix_R(X As NumericMatrix,
                                R As Double(),
                                sidxs As Integer()(),
                                n_genes As Integer,
                                geneset_idxs As Integer(),
                                n_geneset As Integer,
                                tau As Double,
                                n_samples As Integer,
                                mx_diff As Integer,
                                abs_rnk As Integer) As Object

        Dim geneset_mask As Integer() = New Integer(n_genes - 1) {}
        Dim offset As Integer

        For i As Integer = 0 To n_geneset - 1
            geneset_mask(geneset_idxs(i) - 1) = 1
        Next

        For j As Integer = 0 To n_samples - 1
            offset = j * n_genes
            R(j) = ks_sample(X(offset), sidxs(offset), n_genes, geneset_mask, geneset_idxs, n_geneset, tau, mx_diff, abs_rnk)
        Next
    End Function

    Private Function ks_sample(x As Double(),
                               x_sort_indxs As Integer(),
                               n_genes As Integer,
                               geneset_mask As Integer(),
                               geneset_idxs As Integer(),
                               n_geneset As Integer,
                               tau As Double,
                               mx_diff As Integer,
                               abs_rnk As Integer)

        Dim dec As Double = 1 / (n_genes - n_geneset)
        Dim sum_gset As Double = 0.0

        For i As Integer = 0 To n_geneset - 1
            sum_gset += x(geneset_idxs(i) - 1) ^ tau
        Next

        Dim mx_value As Double = 0
        Dim mx_value_sign As Double = 0
        Dim cum_sum As Double = 0

        Dim mx_pos As Double = 0
        Dim mx_neg As Double = 0
        Dim idx As Integer

        For i As Integer = 0 To n_genes - 1
            idx = x_sort_indxs(i) - 1

            If geneset_mask(idx) = 1 Then
                cum_sum += (x(idx) ^ tau) / sum_gset
            Else
                cum_sum -= dec
            End If

            If cum_sum > mx_pos Then mx_pos = cum_sum
            If cum_sum < mx_neg Then mx_neg = cum_sum
        Next

        If mx_diff <> 0.0 Then
            mx_value_sign = mx_pos + mx_neg

            If abs_rnk <> 0 Then
                mx_value_sign = mx_pos - mx_neg
            End If
        Else
            mx_value_sign = If(mx_pos > stdNum.Abs(mx_neg), mx_pos, mx_neg)
        End If

        Return mx_value_sign
    End Function
End Module
