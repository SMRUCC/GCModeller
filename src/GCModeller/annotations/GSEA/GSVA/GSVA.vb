#Region "Microsoft.VisualBasic::22f7efb1d2986a9fb6d1a28ae86ebc94, annotations\GSEA\GSVA\GSVA.vb"

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

    '   Total Lines: 246
    '    Code Lines: 212 (86.18%)
    ' Comment Lines: 5 (2.03%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 29 (11.79%)
    '     File Size: 10.66 KB


    ' Module GSVA
    ' 
    '     Function: compute_gene_density, compute_geneset_es, (+2 Overloads) gsva, ks_test_m
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports std = System.Math

Public Module GSVA

    <Extension>
    Public Function gsva(expr As Matrix, gsetIdxList As Background,
                         Optional method As Methods = Methods.gsva,
                         Optional kcdf As KCDFs? = Nothing,
                         Optional min_sz As Integer = 1,
                         Optional max_sz As Integer = Integer.MaxValue,
                         Optional mxdiff As Boolean = True,
                         Optional tau As Double = 1,
                         Optional kernel As Boolean = True,
                         Optional rnaseq As Boolean = False,
                         Optional abs_ranking As Boolean = False,
                         Optional alpha As Double = 0.25,
                         Optional normalize As Boolean = True,
                         Optional verbose As Boolean = False) As Matrix

        Dim mapped_gset_idx_list As Dictionary(Of String, String())

        ' filter genes according To verious criteria,
        ' e.g., constant expression
        expr = filterFeatures(expr, method)
        ' map to the actual features for which expression data is available
        mapped_gset_idx_list = mapGeneSetsToFeatures(gsetIdxList, expr.rownames)
        ' remove gene sets from the analysis for which no features are available
        ' And meet the minimum And maximum gene-Set size specified by the user
        mapped_gset_idx_list = filterGeneSets(mapped_gset_idx_list, min_sz, max_sz)

        If Not kcdf Is Nothing Then
            If kcdf = KCDFs.Gaussian Then
                rnaseq = False
                kernel = True
            ElseIf kcdf = KCDFs.Poisson Then
                rnaseq = True
                kernel = True
            Else
                kernel = False
            End If
        Else
            kcdf = KCDFs.none
        End If

        Return gsva(expr, mapped_gset_idx_list, method, kcdf, rnaseq, kernel, mxdiff, tau, abs_ranking, alpha, normalize, verbose)
    End Function

    Private Function gsva(expr As Matrix,
                          gsetIdxList As Dictionary(Of String, String()),
                          method As Methods,
                          kcdf As KCDFs,
                          rnaseq As Boolean,
                          kernel As Boolean,
                          mxdiff As Boolean,
                          tau As Double,
                          abs_ranking As Boolean,
                          alpha As Double,
                          normalize As Boolean,
                          verbose As Boolean) As Matrix

        If gsetIdxList.Count = 0 Then
            Throw New InvalidProgramException("The gene set list is empty! Filter may be too stringent.")
        End If
        If gsetIdxList.Any(Function(d) d.Value.Length = 1) Then
            Call "Some gene sets have size one. Consider setting 'min.sz > 1'.".Warning
        End If

        If method = Methods.ssgsea Then
            If verbose Then
                Call $"Estimating ssGSEA scores for {gsetIdxList.Count} gene sets.".debug
            End If

            Return ssgseaScores(expr, gsetIdxList, alpha:=alpha, normalize:=normalize)
        ElseIf method = Methods.zscore Then
            If rnaseq Then
                Throw New InvalidProgramException("rnaseq=TRUE does not work with method='zscore'.")
            End If
            If verbose Then
                Call $"Estimating combined z-scores for {gsetIdxList.Count} gene sets.".debug
            End If

            Return zscoreScores(expr, gsetIdxList)
        ElseIf method = Methods.plage Then
            If rnaseq Then
                Throw New InvalidProgramException("rnaseq=TRUE does not work with method='plage'.")
            End If
            If verbose Then
                Call $"Estimating PLAGE scores for {gsetIdxList.Count} gene sets.".debug
            End If

            Return plageScores(expr, gsetIdxList)
        Else
            If verbose Then
                Call $"Estimating GSVA scores for {gsetIdxList.Count} gene sets.".debug
            End If
        End If

        Dim nsamples = expr.sampleID.Length
        Dim i As Integer() = Sequence(nsamples).ToArray
        Dim es_obs As Matrix = compute_geneset_es(
            expr,
            gsetIdxList,
            i,
            rnaseq,
            kernel,
            mxdiff,
            tau,
            abs_ranking,
            verbose
        )

        Return es_obs
    End Function

    Private Function compute_geneset_es(expr As Matrix,
                                        gsetIdxList As Dictionary(Of String, String()),
                                        sample_idxs As Integer(),
                                        rnaseq As Boolean,
                                        kernel As Boolean,
                                        mxdiff As Boolean,
                                        tau As Double,
                                        abs_ranking As Boolean,
                                        verbose As Boolean) As Matrix
        Dim num_genes = expr.size
        Dim rowIndex As Index(Of String) = expr.rownames.Indexing

        If verbose Then
            If kernel Then
                If rnaseq Then
                    Call "Estimating ECDFs with Poisson kernels".debug
                Else
                    Call "Estimating ECDFs with Gaussian kernels".debug
                End If
            Else
                Call "Estimating ECDFs directly".debug
            End If
        End If

        Dim gene_density As NumericMatrix = compute_gene_density(expr, sample_idxs, rnaseq, kernel)
        Dim n_samples As Integer = gene_density.ColumnDimension
        ' 逐列排名：1 表示取值最小；并列时下标靠后者秩更大（R 的 ties.method = "last"）
        Dim ranks As Integer()() = colRanksLast(gene_density)
        Dim half As Double = num_genes / 2.0
        Dim decordstat As Integer()() = New Integer(n_samples - 1)() {}
        Dim symrnkstat As Double()() = New Double(n_samples - 1)() {}

        For j As Integer = 0 To n_samples - 1
            Dim rank_j As Integer() = ranks(j)
            Dim dos As Integer() = New Integer(num_genes - 1) {}
            Dim srs As Double() = New Double(num_genes - 1) {}

            For i As Integer = 0 To num_genes - 1
                ' 降序序数：1 表示该样本中表达最高的基因，随机游走按此顺序遍历
                dos(i) = num_genes - rank_j(i) + 1
                ' 对称秩统计量：在表达量的两个极端取最大值，中间接近 0。
                ' 官方实现为 fabs(p / 2 - rank)，此处必须取绝对值，
                ' 否则随机游走的步长权重会出现负值，导致富集分数失去 [-1, 1] 的边界
                srs(i) = std.Abs(half - rank_j(i))
            Next

            decordstat(j) = dos
            symrnkstat(j) = srs
        Next

        Dim m As New Matrix With {
            .sampleID = expr.sampleID,
            .expression = gsetIdxList _
                .Select(Function(gsetIdx)
                            Dim idx As Integer() = gsetIdx.Value.Select(Function(id) rowIndex.IndexOf(id)).ToArray
                            Dim test = ks_test_m(idx, symrnkstat, decordstat, mxdiff, abs_ranking, tau, verbose)

                            Return New DataFrameRow With {
                                .experiments = test,
                                .geneID = gsetIdx.Key
                            }
                        End Function) _
                .ToArray
        }

        Return m
    End Function

    Private Function ks_test_m(gset_idxs As Integer(),
                               symrnkstat As Double()(),
                               decordstat As Integer()(),
                               mxdiff As Boolean,
                               abs_ranking As Boolean,
                               tau As Double,
                               verbose As Boolean) As Double()

        Dim ngenes = symrnkstat(0).Length
        Dim nsamples = symrnkstat.Length
        Dim ngeneset = gset_idxs.Count
        Dim geneset_sample_es As Double() = C.ks_matrix_R(symrnkstat, decordstat, ngenes, gset_idxs, ngeneset, tau, nsamples, mxdiff, abs_ranking)

        Return geneset_sample_es
    End Function

    Private Function compute_gene_density(expr As Matrix, sample_idxs As Integer(), rnaseq As Boolean, kernel As Boolean) As NumericMatrix
        Dim ntestsamples = expr.sampleID.Length
        Dim ngenes = expr.size
        Dim ndensitysamples = sample_idxs.Length
        Dim gene_density As NumericMatrix

        If kernel Then
            gene_density = C.matrix_density_R(
                expr.ArrayPack, expr.ArrayPack, (ntestsamples, ngenes),
                ndensitysamples,
                ntestsamples,
                ngenes,
                rnaseq)
        Else
            gene_density = New NumericMatrix(ecdfLogOdds(expr.expression))
        End If

        Return gene_density
    End Function

    ''' <summary>
    ''' 不使用核函数时，直接用每一行自身的经验累积分布函数（ECDF）做行归一化
    ''' </summary>
    ''' <param name="rows">行是基因，行内是该基因在各样本上的表达量</param>
    ''' <returns>与输入同形的矩阵，元素为 ECDF 取值经 logit 变换后的结果</returns>
    ''' <remarks>
    ''' 对应 R 的 ``apply(expr, 1, function(x) ecdf(x)(x))``：
    ''' 对基因 i 的第 j 个样本，其 ECDF 取值为 ``#{x_k &lt;= x_j} / n``，取值落在 (0, 1]。
    ''' 
    ''' 由于后续只使用该矩阵的秩，logit 变换本身是单调的，不影响结果；
    ''' 此处仍然施加该变换，以保持与核函数分支一致的值域。
    ''' 边界上的 0 与 1 会让 logit 溢出为正负无穷，需要先裁剪到一个极小的邻域内，
    ''' 裁剪是单调映射，因此不会改变并列关系。
    ''' </remarks>
    Private Function ecdfLogOdds(rows As IEnumerable(Of DataFrameRow)) As Double()()
        Return rows _
            .Select(Function(r)
                        Dim x As Double() = r.experiments
                        Dim n As Integer = x.Length
                        Dim sorted As Double() = New Double(n - 1) {}
                        Dim p As Double() = New Double(n - 1) {}
                        Dim eps As Double = 1 / (2.0 * n)

                        Array.Copy(x, sorted, n)
                        Array.Sort(sorted)

                        For j As Integer = 0 To n - 1
                            ' 二分查找最后一个不大于 x(j) 的位置，即得 #{x_k <= x_j}
                            Dim count As Integer = upperBound(sorted, x(j))
                            Dim cdf As Double = count / n

                            If cdf < eps Then
                                cdf = eps
                            ElseIf cdf > 1 - eps Then
                                cdf = 1 - eps
                            End If

                            p(j) = std.Log(cdf / (1 - cdf))
                        Next

                        Return p
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 在已升序排序的数组中找到大于 <paramref name="value"/> 的第一个位置，
    ''' 即数组中不大于 <paramref name="value"/> 的元素个数
    ''' </summary>
    Private Function upperBound(sorted As Double(), value As Double) As Integer
        Dim lo As Integer = 0
        Dim hi As Integer = sorted.Length

        While lo < hi
            Dim mid As Integer = (lo + hi) \ 2

            If sorted(mid) <= value Then
                lo = mid + 1
            Else
                hi = mid
            End If
        End While

        Return lo
    End Function
End Module
