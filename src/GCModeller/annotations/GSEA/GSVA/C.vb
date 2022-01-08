Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports stdNum = System.Math

Module C

    Public Function matrix_density_R()

    End Function

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

    Private Function ks_sample(x As Double(), x_sort_indxs As Integer(), n_genes As Integer, geneset_mask As Integer(), geneset_idxs As Integer(), n_geneset As Integer, tau As Double, mx_diff As Integer, abs_rnk As Integer)
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
