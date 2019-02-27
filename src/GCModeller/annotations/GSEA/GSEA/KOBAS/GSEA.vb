Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Module GSEA

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
End Module
