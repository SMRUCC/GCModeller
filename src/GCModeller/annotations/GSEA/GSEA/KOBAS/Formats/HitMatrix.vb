Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace KOBAS

    ''' <summary>
    ''' 一个基因集在排序后的基因列表上的命中信息。
    ''' </summary>
    Public Class GeneSetHits

        ''' <summary>
        ''' 基因集编号（GMT 的第一列 / KEGG 通路编号）。
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String

        ''' <summary>
        ''' 基因集名称（GMT 的第二列）。
        ''' </summary>
        ''' <returns></returns>
        Public Property Names As String

        ''' <summary>
        ''' 基因集的描述信息。
        ''' </summary>
        ''' <returns></returns>
        Public Property Description As String

        ''' <summary>
        ''' 在当前的排序基因列表之中命中的基因数量。
        ''' </summary>
        ''' <returns></returns>
        Public Property HitCount As Integer

        ''' <summary>
        ''' 该行数据对应的原始基因集对象，便于回溯成员列表。
        ''' </summary>
        ''' <returns></returns>
        Public Property Source As Cluster
    End Class

    ''' <summary>
    ''' 基因集命中矩阵：行为基因集、列为排序后的基因。
    ''' </summary>
    ''' <remarks>
    ''' 本类型是 KOBAS 3.0 <c>src/kobas/gsea.py</c> 中 <c>get_hit_matrix</c> 返回的
    ''' <c>hit_matrix_filtered / hit_genes_filterd / hit_sum_filtered / gset_name_filtered /
    ''' gset_des_filtered</c> 五个数组的等价物，使用一个对象统一承载。
    ''' </remarks>
    Public Class HitMatrix

        ''' <summary>
        ''' 列方向上的基因编号，顺序由调用方给定（通常是基因排序之后的顺序）。
        ''' </summary>
        ''' <returns></returns>
        Public Property Genes As String()

        ''' <summary>
        ''' 行方向上的基因集信息（已经按命中数过滤）。
        ''' </summary>
        ''' <returns></returns>
        Public Property GeneSets As GeneSetHits()

        ''' <summary>
        ''' 命中掩膜：<c>Mask(i)(j)</c> 表示第 <c>i</c> 个基因集中是否包含第 <c>j</c> 个基因。
        ''' </summary>
        ''' <returns></returns>
        Public Property Mask As Boolean()()

        ''' <summary>
        ''' 行数（保留下来的基因集数量）。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RowCount As Integer
            Get
                Return If(GeneSets Is Nothing, 0, GeneSets.Length)
            End Get
        End Property

        ''' <summary>
        ''' 列数（参与分析的基因数量）。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ColumnCount As Integer
            Get
                Return If(Genes Is Nothing, 0, Genes.Length)
            End Get
        End Property

        ''' <summary>
        ''' 取得第 <paramref name="index"/> 个基因集的命中基因编号。
        ''' </summary>
        ''' <param name="index">基因集在 <see cref="GeneSets"/> 之中的下标。</param>
        ''' <returns></returns>
        Public Function HitGenes(index As Integer) As String()
            Dim row As Boolean() = Mask(index)
            Dim hits As New List(Of String)

            For j As Integer = 0 To row.Length - 1
                If row(j) Then
                    hits.Add(Genes(j))
                End If
            Next

            Return hits.ToArray
        End Function
    End Class

    ''' <summary>
    ''' 构建基因集命中矩阵（KOBAS <c>gsea.get_hit_matrix</c> 的 VB.NET 移植）。
    ''' </summary>
    Public Module HitMatrixBuilder

        ''' <summary>
        ''' 根据排序后的基因列表与背景模型构建命中矩阵。
        ''' </summary>
        ''' <param name="genes">
        ''' 参与分析的基因编号，顺序即基因在列表之中的位置（通常来自 <c>rank_pro</c> 的结果）。
        ''' </param>
        ''' <param name="background">基因集背景模型（通常由 <see cref="GmtFile"/> 构建）。</param>
        ''' <param name="minSize">基因集命中数下限（闭区间）。</param>
        ''' <param name="maxSize">基因集命中数上限（闭区间）。</param>
        ''' <param name="verbose">是否输出过滤日志。</param>
        ''' <returns>过滤之后的命中矩阵。</returns>
        ''' <exception cref="InvalidOperationException">
        ''' 没有任何基因被任何基因集命中，或者所有基因集都被尺寸过滤条件剔除时抛出。
        ''' </exception>
        <Extension>
        Public Function BuildHitMatrix(genes As String(),
                                       background As Background,
                                       Optional minSize As Integer = 15,
                                       Optional maxSize As Integer = 500,
                                       Optional verbose As Boolean = False) As HitMatrix

            If genes Is Nothing OrElse genes.Length = 0 Then
                Throw New InvalidOperationException("The input gene list is empty, cannot build the hit matrix.")
            End If

            If background Is Nothing OrElse background.clusters.IsNullOrEmpty Then
                Throw New InvalidOperationException("The gene set background model contains no cluster.")
            End If

            Dim rows As New List(Of GeneSetHits)
            Dim mask As New List(Of Boolean())
            Dim anyHit As Boolean = False
            Dim filtered As Integer = 0

            For Each cluster As Cluster In background.clusters
                Dim members As String() = If(cluster.members, New BackgroundGene() {}) _
                    .Select(Function(gene) gene.accessionID) _
                    .Where(Function(id) Not String.IsNullOrEmpty(id)) _
                    .ToArray
                Dim lookup As New HashSet(Of String)(members, StringComparer.Ordinal)
                Dim row As Boolean() = New Boolean(genes.Length - 1) {}
                Dim hits As Integer = 0

                For j As Integer = 0 To genes.Length - 1
                    If lookup.Contains(genes(j)) Then
                        row(j) = True
                        hits += 1
                        anyHit = True
                    End If
                Next

                If hits < minSize OrElse hits > maxSize Then
                    filtered += 1
                    Continue For
                End If

                rows.Add(New GeneSetHits With {
                    .ID = cluster.ID,
                    .Names = cluster.names,
                    .Description = cluster.description,
                    .HitCount = hits,
                    .Source = cluster
                })
                mask.Add(row)
            Next

            If Not anyHit Then
                Throw New InvalidOperationException(
                    "None of genes in the input expression matrix was matched with the genes in the gene sets. " &
                    "Please check the expression matrix, the gene set database and the id type.")
            End If

            If rows.Count = 0 Then
                Throw New InvalidOperationException(
                    $"All {background.clusters.Length} gene set(s) have been filtered out by the size thresholds " &
                    $"(min_size={minSize}, max_size={maxSize}). Please check the threshold and ceil of gene set size.")
            End If

            If verbose Then
                Call Console.WriteLine($"[KOBAS::HitMatrix] {rows.Count} gene set(s) kept, {filtered} filtered by size [{minSize}, {maxSize}].")
            End If

            Return New HitMatrix With {
                .Genes = genes,
                .GeneSets = rows.ToArray,
                .Mask = mask.ToArray
            }
        End Function
    End Module
End Namespace
