﻿Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports stdNum = System.Math

Namespace C

    Module ks_test

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="X">gene density scores</param>
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
                                    sidxs As Integer()(),
                                    n_genes As Integer,
                                    geneset_idxs As Integer(),
                                    n_geneset As Integer,
                                    tau As Double,
                                    n_samples As Integer,
                                    mx_diff As Boolean,
                                    abs_rnk As Boolean) As Double()

            Dim geneset_mask As Integer() = New Integer(n_genes - 1) {}
            ' Dim offset As Integer
            Dim nsamples = X.ColumnDimension
            Dim R As Double() = New Double(nsamples - 1) {}

            For i As Integer = 0 To n_geneset - 1
                geneset_mask(geneset_idxs(i)) = 1
            Next

            For j As Integer = 0 To n_samples - 1
                ' offset = j * n_genes
                ' R(j) = ks_sample(X(offset, byRow:=False), sidxs(offset), n_genes, geneset_mask, geneset_idxs, n_geneset, tau, mx_diff, abs_rnk)
                R(j) = ks_sample(X(j, byRow:=False), sidxs(j), n_genes, geneset_mask, geneset_idxs, n_geneset, tau, mx_diff, abs_rnk)
            Next

            Return R
        End Function

        Private Function ks_sample(x As Double(),
                                   x_sort_indxs As Integer(),
                                   n_genes As Integer,
                                   geneset_mask As Integer(),
                                   geneset_idxs As Integer(),
                                   n_geneset As Integer,
                                   tau As Double,
                                   mx_diff As Boolean,
                                   abs_rnk As Boolean) As Double

            Dim dec As Double = 1 / (n_genes - n_geneset)
            Dim sum_gset As Double = 0.0

            For i As Integer = 0 To n_geneset - 1
                sum_gset += x(geneset_idxs(i)) ^ tau
            Next

            Dim mx_value As Double = 0
            Dim mx_value_sign As Double = 0
            Dim cum_sum As Double = 0

            Dim mx_pos As Double = 0
            Dim mx_neg As Double = 0
            Dim idx As Integer

            For i As Integer = 0 To n_genes - 1
                idx = x_sort_indxs(i)

                If geneset_mask(idx) = 1 Then
                    cum_sum += (x(idx) ^ tau) / sum_gset
                Else
                    cum_sum -= dec
                End If

                If cum_sum > mx_pos Then mx_pos = cum_sum
                If cum_sum < mx_neg Then mx_neg = cum_sum
            Next

            If mx_diff Then
                mx_value_sign = mx_pos + mx_neg

                If abs_rnk Then
                    mx_value_sign = mx_pos - mx_neg
                End If
            Else
                mx_value_sign = If(mx_pos > stdNum.Abs(mx_neg), mx_pos, mx_neg)
            End If

            Return mx_value_sign
        End Function
    End Module
End Namespace