#Region "Microsoft.VisualBasic::dc5928d159c29b01d6c08fe247ec21bf, GCModeller\annotations\GSEA\GSEA\KOBAS\GSEA.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 328
    '    Code Lines: 236
    ' Comment Lines: 44
    '   Blank Lines: 48
    '     File Size: 15.69 KB


    '     Module KOBAS_GSEA
    ' 
    '         Function: ES_all, ES_for_permutation, ES_null, fdr_cal, get_hit_matrix
    '                   nominal_p, normalized, output_set, rank_pro
    '         Structure hitMatrix
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports np = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

Namespace KOBAS

    <HideModuleName>
    Public Module KOBAS_GSEA

        Public Structure hitMatrix
            Dim hit_matrix_filtered As Vector(Of np)
            Dim hit_genes_filtered As List(Of String)()
            Dim hit_sum_filtered As Vector
            Dim gset_name_filtered As String()
            Dim gset_des_filtered As String()
        End Structure

        ''' <summary>
        ''' This function get a matrix of all gene sets
        ''' </summary>
        ''' <param name="gene_list">所需要进行富集分析的目标基因列表</param>
        ''' <param name="gene_num">目标基因列表的长度</param>
        ''' <param name="gset_name">代谢途径的名称列表</param>
        ''' <param name="gset_des">应该是与<paramref name="gset_name"/>等长的唯一标识符，其实这个参数可以直接用<paramref name="gset_name"/>来代替</param>
        ''' <param name="gset_genes">每一个代谢途径之中的背景基因列表</param>
        ''' <param name="min_size"></param>
        ''' <param name="max_size"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 在这里的gene set是某一个代谢途径之中的所有的背景基因编号列表
        ''' 背景基因的数据一般来自于<see cref="Gmt"/>文件的读取结果
        ''' </remarks>
        Public Function get_hit_matrix(gene_list As String(), gene_num%, gset_name As String(), gset_des As String(), gset_genes As Index(Of String)(), min_size%, max_size%) As hitMatrix
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
            Dim delindex = which.IsTrue((hitsum < min_size) Or (hitsum > max_size))
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

            Return New hitMatrix With {
            .hit_matrix_filtered = New Vector(Of np)(hit_matrix_filtered),
            .hit_genes_filtered = hit_genes_filtered,
            .hit_sum_filtered = hit_sum_filtered,
            .gset_name_filtered = gset_name_filtered,
            .gset_des_filtered = gset_des_filtered
        }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="lb">Labels</param>
        ''' <param name="md"></param>
        ''' <param name="expr_data"></param>
        ''' <param name="sample0#"></param>
        ''' <param name="sample1#"></param>
        ''' <returns></returns>
        Public Function rank_pro(lb As Integer(), md As String, expr_data As Vector(Of Vector), sample0#, sample1#) As (sort_r As Vector, sort_gene_index As Vector)
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
            Dim mean_0 = expr_0.Mean(axis:=1)
            Dim mean_1 = expr_1.Mean(axis:=1)
            Dim std_0 = expr_0.Std(axis:=1)
            Dim std_1 = expr_0.Std(axis:=1)

            Dim sort_gene_index As Vector
            Dim sort_r As Vector

            If md = "snr" Then
                Dim s2n = (mean_0 - mean_1) / (std_0 + std_1)
                ' this step get index after sorted, then use this index to get gene list from gene_name
                sort_gene_index = np.argsort(s2n).AsVector.slice(, -1) '.T 
                ' this step get s2n value after sorted
                sort_r = np.Sort(s2n).slice(, -1) '.T 
            ElseIf md = "ttest" Then
                Dim a = mean_0 - mean_1
                Dim s0 = std_0 ^ 2
                Dim s1 = std_1 ^ 2
                Dim b = Vector.Sqrt(s0 / sample0 + s1 / sample1)
                Dim ttest = a / b

                sort_gene_index = np.argsort(ttest).AsVector.slice(, -1) '.T
                sort_r = np.Sort(ttest).slice(, -1) '.T
            Else
                Throw New NotSupportedException(md)
            End If

            ' NOTE: sort_r is 1*gene_num matrix
            Return (sort_r, sort_gene_index)
        End Function

        Public Function ES_all(sort_r#, sort_gene_index As Vector, hit_matrix_filtered As Object, weighted_score_type%, gene_num%)
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
            Dim es_idx = Function(x As Vector)
                             ' Return np.Where(Math.Abs(x.Max()) > Math.Abs(x.Min()), (Val:=x.Max(), Index:=Which.Max(x)), (Val:=x.Min(), Index:=Which.Min(x)))
                         End Function
            Dim re As Vector = es_idx(RES)
            Dim es As Vector = re.slice(, 0)
            Dim idx As Vector = re.slice(, 1)

            Return (es, idx, RES)
        End Function

        Public Function ES_null(lb%(), times%, method$, sample0 As Vector, sample1 As Vector, hit_matrix_filtered As Vector(Of Vector), weighted_score_type%, expr_data As Vector(Of Vector), gene_num%)
            Dim lb_matrix As Integer()() = Enumerable.Range(0, times).Select(Function(null) lb).ToArray   ' np.array([lb for i in range(times)])
            Dim ran_labels = lb_matrix.Select(Function(x) x.Shuffles).ToArray
            Dim def_get_es_null = Function(x As Integer()) ES_for_permutation(x, method, sample0, sample1, hit_matrix_filtered, weighted_score_type, expr_data, gene_num)
            Dim es_null2 = ran_labels.Select(Function(x) def_get_es_null(x)).MatrixTranspose
            Return es_null2
        End Function

        Public Function nominal_p(es As Vector, es_null As Vector) As Vector
            Dim ES_all = np.column_stack(es_null, es)
            Dim def_pval = Function(x As Vector) np.Sum(x.slice(, -1) >= x(-1)) / np.Sum(x.slice(, -1) >= 0) Or (np.Sum(x.slice(, -1) <= x(-1)) / np.Sum(x.slice(, -1) <= 0)).When(x(-1) >= 0)
            ' def_pval = lambda x: where(x[-1]>=0, sum(x[:-1] >= x[-1])/float(sum(x[:-1]) >=0), sum(x[:-1] <= x[-1])/float(sum(x[:-1]) <=0))
            Dim r = ES_all.Select(def_pval).ToArray
            Dim pval = r.IteratesALL.AsVector      ' m*1 array  m: num of filter gene sets
            Return pval
        End Function


        Public Function normalized(es As Vector, es_null As Vector)

            Dim def_mean_pos = Function(x As Vector) x(x >= 0).Average
            Dim def_mean_neg = Function(x As Vector) Vector.Abs(x(x <= 0)).Average
            Dim def_nor = Function(x As Vector) np.Where(x.slice(, -2) >= 0, x.slice(, -2) / x(-2), x.slice(, -2) / x(-1))

            Dim mean_p As Vector = es_null.Select(def_mean_pos)
            ' mean_p = mean_p.reshape(Len(mean_p), 1)          ' shape=(m,1)
            Dim mean_n As Vector = es_null.Select(def_mean_neg).ToArray
            ' mean_n = mean_n.reshape(Len(mean_n), 1)

            Dim es_null_mean = np.column_stack(es_null, mean_p, mean_n).ToArray
            Dim nes_null = es_null_mean.Select(def_nor).ToArray

            Dim es_mean = np.column_stack(es, mean_p, mean_n).ToArray
            Dim nes_obs = es_mean.Select(def_nor).ToArray
            Return (nes_obs, nes_null)
        End Function

        Public Function fdr_cal(nes_obs As Vector, nes_null As Vector) As Double()
            Dim nullmore0 = BooleanVector.Sum(nes_null >= 0)
            Dim nullless0 = BooleanVector.Sum(nes_null <= 0)
            Dim obsmore0 = BooleanVector.Sum(nes_obs >= 0)
            Dim obsless0 = BooleanVector.Sum(nes_obs <= 0)
            Dim nes As New Vector(nes_obs)

            Dim def_top = Function(x As Vector)
                              Dim a = (BooleanVector.Sum(nes_null >= x) / nullmore0) Or 0#.When(nullmore0 > 0)
                              Dim b = (BooleanVector.Sum(nes_null <= x) / nullless0) Or 0#.When(nullless0 > 0)

                              Return np.Where(x >= 0, a, b)
                          End Function
            Dim def_down = Function(x As Vector)
                               Dim a = (BooleanVector.Sum(nes >= x) / obsmore0) Or 0#.When(obsmore0 > 0)
                               Dim b = (BooleanVector.Sum(nes <= x) / obsless0) Or 0#.When(obsless0 > 0)

                               Return np.Where(x >= 0, a, b)
                           End Function

            'def_top = lambda x : where(x>=0, sum(nes_null >= x)/float(nullmore0), sum(nes_null <= x)/float(nullless0))
            'def_down = lambda x : where(x>=0, sum(nes>= x)/float(obsmore0), sum(nes <= x)/float(obsless0))

            Dim top = nes_obs.Select(def_top).ToArray
            Dim down = nes_obs.Select(def_down).ToArray
            Dim cal As Double()() ' = np.column_stack(top, down)
            Dim def_fdr = Function(x As Double())
                              Return If(x(0) / x(1) < 1, x(0) / x(1), 1)
                          End Function
            Dim fdr = cal.Select(def_fdr).ToArray
            Return fdr
        End Function

        Public Function ES_for_permutation(lb%(), md$, sample0 As Vector, sample1 As Vector, hit_matrix_filtered As Vector(Of Vector), weighted_score_type%, expr_data As Vector(Of Vector), gene_num%) As Double()
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
            Dim mean_0 As Vector = expr_0.Mean(axis:=1)
            Dim mean_1 As Vector = expr_1.Mean(axis:=1)
            Dim std_0 As Vector = expr_0.Std(axis:=1)
            Dim std_1 As Vector = expr_1.Std(axis:=1)
            Dim s2n As Vector
            Dim sort_gene_index As Integer()
            Dim sort_r As Vector

            If (md = "snr") Then
                s2n = (mean_0 - mean_1) / (std_0 + std_1)
                sort_gene_index = np.argsort(s2n).AsVector().slice(, -1).AsInteger  '.T  ' this step get index after sorted, then use this index to get gene list from gene_name
                sort_r = np.Sort(s2n).slice(, -1) ' this step get s2n value after sorted
            ElseIf (md = "ttest") Then
                Dim a = mean_0 - mean_1
                Dim s0 = std_0 ^ 2 ' np.square(std_0)
                Dim s1 = std_1 ^ 2 ' np.square(std_1)
                Dim b = Vector.Sqrt(s0 / sample0 + s1 / sample1)  ' np.sqrt(s0 / sample0 + s1 / sample1)
                Dim ttest = a / b
                sort_gene_index = np.argsort(ttest).AsVector().slice(, -1).AsInteger  '.T
                sort_r = np.Sort(ttest).slice(, -1) '.T
            Else
                Throw New NotSupportedException(md)
            End If

            Dim hitm As New NumericMatrix(hit_matrix_filtered(sort_gene_index))
            Dim missm As NumericMatrix = hitm - 1
            Dim sort_arr As New NumericMatrix(Enumerable.Range(0, hitm.Length).Select(Function(null) sort_r).ToArray)  '  np.array([sort_r for i in range(len(hitm))])
            Dim tmp As NumericMatrix

            If weighted_score_type = 0 Then
                tmp = hitm
            End If
            If weighted_score_type = 1 Then
                ' tmp = Vector.Abs(sort_arr) * hitm  ' np.absolute(sort_arr) * hitm
            End If
            If weighted_score_type = 2 Then
                'tmp = sort_arr ^ 2 * hitm
            Else
                '  tmp = Vector.Abs(sort_arr) ^ weighted_score_type * hitm '  np.absolute(sort_arr) ^ weighted_score_type * hitm
            End If

            Dim NR '= np.Sum(tmp, axis = 1)
            Dim hit_score = tmp / NR
            Dim miss_score = 1.0 / (gene_num - hitm.Length) * missm
            Dim res As DoubleRange() '= np.CumSum(hit_score + miss_score, axis = 1)
            Dim get_es = Function(x As DoubleRange)
                             Return If(Math.Abs(x.Max()) > Math.Abs(x.Min()), x.Max(), x.Min())
                         End Function
            Dim es = res.Select(get_es).ToArray
            Return es
        End Function

        Public Function output_set(result_path, phnameA, phnameB, gset_name_filtered, gset_des_filtered, hit_matrix_filtered, ES, NES, nompval, FDR)

        End Function
    End Module
End Namespace
