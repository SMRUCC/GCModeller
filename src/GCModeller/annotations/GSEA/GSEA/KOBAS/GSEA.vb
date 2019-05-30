Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports np = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

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
    Public Function get_hit_matrix(gene_list As String(), gene_num%, gset_name As String(), gset_des As String(), gset_genes As Index(Of String)(), min_size%, max_size%)
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
                If gene Like gset_genes(i) Then
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
        Dim delindex = Which.IsTrue((hitsum < min_size) Or (hitsum > max_size))
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

    Public Function ES_all(sort_r#, sort_gene_index As Object, hit_matrix_filtered As Object, weighted_score_type%, gene_num%)
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
                         ' Return Which.IsTrue(Math.Abs(x.Max()) > Math.Abs(x.Min()), (x.Max(), x.argmax()), (x.Min(), x.argmin()))
                     End Function
        Dim re = es_idx(RES)

    End Function

    Public Function ES_null(lb%(), times%, method$, sample0, sample1, hit_matrix_filtered, weighted_score_type, expr_data, gene_num)
        Dim lb_matrix As Integer()() = Enumerable.Range(0, times).Select(Function(null) lb).ToArray   ' np.array([lb for i in range(times)])
        Dim ran_labels = lb_matrix.Select(Function(x) x.Shuffles).ToArray
        Dim def_get_es_null = Function(x As Integer()) ES_for_permutation(x, method, sample0, sample1, hit_matrix_filtered, weighted_score_type, expr_data, gene_num)
        Dim es_null2 = ran_labels.Select(Function(x) def_get_es_null(x)).MatrixTranspose
        Return es_null2
    End Function

    Public Function nominal_p(es, es_null)
        Dim ES_all = np.column_stack((es_null, es))
        Dim def_pval = Function(x) sum(x[:   -1] >= x[-1])/float(sum(x[:-1] >=0)) if (x[-1]>=0) else sum(x[:-1] <= x[-1])/float(sum(x[:-1] <=0))
    ' def_pval = lambda x: where(x[-1]>=0, sum(x[:-1] >= x[-1])/float(sum(x[:-1]) >=0), sum(x[:-1] <= x[-1])/float(sum(x[:-1]) <=0))
  Dim r = map(def_pval, ES_all)
        Dim pval = np.array([r]).T     ' m*1 array  m: num of filter gene sets
        Return pval
    End Function


    Public Function normalized(es As Vector, es_null As Vector)

        Dim def_mean_pos = Function(x As Vector) x(x >= 0).Average
        Dim def_mean_neg = Function(x As Vector) Vector.Abs(x(x <= 0)).Average
        Dim def_nor = Function(x As Vector) np.Where(x(, -2) >= 0, x(, -2) / x(-2), x(, -2) / x(-1))

        Dim mean_p As Vector = es_null.Select(def_mean_pos)
        mean_p = mean_p.reshape(Len(mean_p), 1)          ' shape=(m,1)
        Dim mean_n As Vector = es_null.Select(def_mean_neg).ToArray
        mean_n = mean_n.reshape(Len(mean_n), 1)

        Dim es_null_mean = np.column_stack(es_null, mean_p, mean_n).ToArray
        Dim nes_null = es_null_mean.Select(def_nor).toarray

        Dim es_mean = np.column_stack(es, mean_p, mean_n).ToArray
        Dim nes_obs = es_mean.Select(def_nor).toarray
        Return (nes_obs, nes_null)
    End Function

    Public Function fdr_cal(nes_obs As Vector, nes_null As Vector) As Double()

        Dim nullmore0 = np.sum(nes_null >= 0)
        Dim nullless0 = np.sum(nes_null <= 0)

        Dim obsmore0 = np.sum(nes_obs >= 0)
        Dim obsless0 = np.sum(nes_obs <= 0)

        Dim def_top = Function(x As Double) If(x >= 0, np.sum(nes_null >= x) / Float(nullmore0) If (nullmore0>0) Else 0,
                              np.sum(nes_null <= x)/float(nullless0) If(nullless0>0) Else 0)

Dim def_down = Function(x) If(x >= 0, np.sum(nes >= x) / Float(obsmore0) If(obsmore0>0) Else 0,
                               np.sum(nes <= x)/float(obsless0) If(obsless0>0) Else 0)

    'def_top = lambda x : where(x>=0, sum(nes_null >= x)/float(nullmore0), sum(nes_null <= x)/float(nullless0))
    'def_down = lambda x : where(x>=0, sum(nes>= x)/float(obsmore0), sum(nes <= x)/float(obsless0))

   Dim top = nes_obs.Select(def_top).toarray
        Dim nes = nes_obs.copy()
        Dim down = np.array(map(def_down, nes_obs))
        Dim cal As Double()() = np.column_stack((top, down))
        Dim def_fdr = Function(x As Double()) If(x(0) / x(1) < 1, x(0) / x(1), 1)
        Dim fdr = cal.Select(def_fdr).ToArray
        Return fdr
    End Function
    Public Function ES_for_permutation(lb%(), md$, sample0, sample1, hit_matrix_filtered, weighted_score_type, expr_data, gene_num) As Double()
        Dim index_0 As New List(Of Integer)
        Dim index_1 As New List(Of Integer)

        ' abstract index as i, content as j
        For Each element As (i%, j%) In lb.SeqIterator.Tuples
            If element.j = 0 Then
                index_0.Add(element.i)
            Else
                index_1.Add(element.i)
            End If
        Next

        ' : abstract all rows In column index_0 includes. [row, column]
        Dim expr_0 = expr_data(index_0)
        Dim expr_1 = expr_data(index_1)
        ' axis 1 Is meaning Get average from column adds
        Dim mean_0 As Vector = expr_0.mean(1)
        Dim mean_1 As Vector = expr_1.mean(1)
        Dim std_0 As Vector = expr_0.std(1)
        Dim std_1 As Vector = expr_1.std(1)
        Dim s2n As Vector
        Dim sort_gene_index As Integer()
        Dim sort_r

        If (md = "snr") Then
            s2n = (mean_0 - mean_1) / (std_0 + std_1)
            sort_gene_index = np.argsort(s2n, 0)[:-1].T  ' this step get index after sorted, then use this index to get gene list from gene_name
        sort_r = np.sort(s2n, 0)[:-1].T ' this step get s2n value after sorted
        End If

        Dim a As Vector
        Dim s0 As Vector
        Dim s1 As Vector
        Dim b As Vector
        Dim ttest As Vector

        If (md = "ttest") Then
            a = mean_0 - mean_1
            s0 = std_0 ^ 2 ' np.square(std_0)
            s1 = std_1 ^ 2 ' np.square(std_1)
            b = Vector.Sqrt(s0 / sample0 + s1 / sample1)  ' np.sqrt(s0 / sample0 + s1 / sample1)
            ttest = a / b
            sort_gene_index = np.argsort(ttest, 0)[:-1].T
        sort_r = np.sort(ttest, 0)[:-1].T
        End If

        Dim hitm = hit_matrix_filtered[:,sort_gene_index]
  Dim missm = hitm - 1
        Dim sort_arr As Vector = Enumerable.Range(hitm.length).Select(Function(null) sort_r).ToArray    '  np.array([sort_r for i in range(len(hitm))])
        Dim tmp

        If weighted_score_type = 0 Then
            tmp = hitm
        End If
        If weighted_score_type = 1 Then
            tmp = Vector.Abs(sort_arr) * hitm  ' np.absolute(sort_arr) * hitm
        End If
        If weighted_score_type = 2 Then
            tmp = sort_arr ^ 2 * hitm
        Else
            tmp = Vector.Abs(sort_arr) ^ weighted_score_type * hitm '  np.absolute(sort_arr) ^ weighted_score_type * hitm
        End If

        Dim NR = np.sum(tmp, axis = 1)
        Dim hit_score = np.array(map(np.divide, tmp, NR))
        Dim miss_score = 1.0 / (gene_num - Len(hitm)) * missm
        Dim Res As DoubleRange() = np.cumsum(hit_score + miss_score, axis = 1)
        Dim get_es = Function(x As DoubleRange) If(Math.Abs(x.Max()) > Math.Abs(x.Min()), x.Max(), x.Min())
        Dim es = Res.Select(get_es).ToArray
        Return es
    End Function

    Public Function output_set(result_path, phnameA, phnameB, gset_name_filtered, gset_des_filtered, hit_matrix_filtered, ES, NES, nompval, FDR)

    End Function
End Module
