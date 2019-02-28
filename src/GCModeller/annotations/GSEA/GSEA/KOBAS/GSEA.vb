Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Module KOBAS_GSEA

    ''' <summary>
    ''' This function get a matrix of all gene sets
    ''' </summary>
    ''' <param name="gene_list"></param>
    ''' <param name="gene_num"></param>
    ''' <param name="gset_name"></param>
    ''' <param name="gset_des"></param>
    ''' <param name="gset_genes"></param>
    ''' <param name="min_size"></param>
    ''' <param name="max_size"></param>
    ''' <returns></returns>
    Public Function GetHitMatrix(gene_list As String(), gene_num%, gset_name As String(), gset_des As String(), gset_genes As Index(Of String)(), min_size%, max_size%)
        Dim hit_m As New List(Of List(Of Integer))
        Dim hit_genes As New List(Of List(Of String))

        For Each i As Integer In Enumerable.Range(0, gset_name.Length)
            Dim hit_g As New List(Of String)
            ' 0 -- not hit, 1 -- hit
            Dim is_hit As Integer = 0
            ' save  which position of sorted list is hitted
            Dim hit_array As New List(Of Integer)

            For Each j As Integer In Enumerable.Range(0, gene_num)
                Dim gene = gene_list(j)
                ' for each in multi_gene :
                If gene.IsOneOfA(gset_genes(i)) Then
                    is_hit = 1
                    hit_g.Add(gene)
                End If
                hit_array.Add(is_hit)
                is_hit = 0
            Next

            hit_genes.Add(hit_g)
            hit_m.Add(hit_array)
        Next

        Dim hit_matrix As Vector() = hit_m.Select(Function(r) r.AsVector).ToArray

        If Not hit_matrix.Any(Function(r) r.Contains(1.0)) Then
            Throw New Exception("Error Message:
None of genes in input file(gct file) was matched with genes in gene sets. 
Please check the gct file(-e), the gmt file(-g), the idtype(-i), the database type(-d).")
        End If

        Dim hitsum = hit_matrix.Select(Function(v) v.Sum).AsVector
        Dim delindex = Which.IsTrue((hitsum < min_size) Or (hitsum > max_size))(0)
        Dim hit_matrix_filtered = hit_matrix.Delete(delindex)
        Dim hit_genes_filtered = hit_genes.Delete(delindex)
        Dim hit_sum_filtered = hitsum.ToArray.Delete(delindex)
        Dim gset_name_filtered = gset_name.Delete(delindex)
        Dim gset_des_filtered = gset_des.Delete(delindex)

        If hit_matrix_filtered.Length = 0 Then
            Throw New Exception("Error Message:
All gene sets" & hit_matrix.Length & "have been filtered. 
Please check the threshold and ceil of gene set size (values of min_size and max_size). ")
        End If

        Return (hit_matrix_filtered, hit_genes_filtered, hit_sum_filtered, gset_name_filtered, gset_des_filtered)
    End Function

    Public Function rankPro(lb As Integer(), md As String, expr_data As Vector, sample0#, sample1#)
        Dim index_0 As New List(Of Integer)
        Dim index_1 As New List(Of Integer)

        For Each i In lb.SeqIterator
            ' abstract index As i, content As j

            If i.value = 0 Then
                index_0.Add(i)
            Else
                index_1.Add(i)
            End If
        Next

        ' abstract all rows in column index_0 includes. [row,column]
        Dim expr_0 = expr_data(index_0)
        Dim expr_1 = expr_data(index_1)
        Dim mean_0 = expr_0.Average
        Dim mean_1 = expr_1.Average
        Dim std_0 = expr_0.StdError
        Dim std_1 = expr_0.StdError

        Dim sort_gene_index
        Dim sort_r

        If md = "snr" Then
            Dim s2n = (mean_0 - mean_1) / (std_0 + std_1)
            ' this step get index after sorted, then use this index to get gene list from gene_name

            ' this step get s2n value after sorted

        ElseIf md = "ttest" Then
            Dim a = mean_0 - mean_1
            Dim s0 = std_0 ^ 2
            Dim s1 = std_1 ^ 2
            Dim b = Math.Sqrt(s0 / sample0 + s1 / sample1)
            Dim ttest = a / b

        End If

        ' NOTE: sort_r is 1*gene_num matrix
        Return (sort_r, sort_gene_index)
    End Function

    Public Function ES_all(sort_r#, sort_gene_index, hit_matrix_filtered, weighted_score_type%, gene_num%)
        Dim hitm As Vector = hit_matrix_filtered(sort_gene_index)
        Dim missm = hitm - 1
        Dim sort_arr As Vector = sort_r.Repeats(hitm.Length)
        Dim tmp As Vector

        If weighted_score_type = 0 Then
            tmp = hitm
        ElseIf weighted_score_type = 1 Then
            tmp = sort_arr.Abs * hitm
        ElseIf weighted_score_type = 2 Then
            tmp = (sort_arr ^ 2) * hitm
        Else
            tmp = (sort_arr.Abs ^ weighted_score_type) * hitm
        End If

        Dim NR = tmp.Sum
        Dim hit_score = tmp / NR
        Dim miss_score = 1 / (gene_num - hitm.Length) * missm
        Dim pre_score = hit_score + miss_score

        Dim RES As Vector = pre_score.CumSum
        Dim es_idx = Function(x As Vector) As Integer()
                         Return Which.IsTrue(Math.Abs(x.Max()) > Math.Abs(x.Min()), (x.Max(), x.argmax()), (x.Min(), x.argmin()))
                     End Function
        Dim re = es_idx(RES)

    End Function
End Module
