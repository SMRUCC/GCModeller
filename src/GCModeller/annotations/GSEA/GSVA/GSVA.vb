#Region "Microsoft.VisualBasic::a5c673f2bf1c9ebabad677a4ed9d4d96, annotations\GSEA\GSVA\GSVA.vb"

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

    '   Total Lines: 247
    '    Code Lines: 213 (86.23%)
    ' Comment Lines: 5 (2.02%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 29 (11.74%)
    '     File Size: 10.74 KB


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

        Return gsva(expr, mapped_gset_idx_list, method, kcdf, rnaseq, kernel, mxdiff, tau, abs_ranking, verbose)
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

            Throw New NotImplementedException
        ElseIf method = Methods.zscore Then
            If rnaseq Then
                Throw New InvalidProgramException("rnaseq=TRUE does not work with method='zscore'.")
            End If
            If verbose Then
                Call $"Estimating combined z-scores for {gsetIdxList.Count} gene sets.".debug
            End If

            Throw New NotImplementedException
        ElseIf method = Methods.plage Then
            If rnaseq Then
                Throw New InvalidProgramException("rnaseq=TRUE does not work with method='plage'.")
            End If
            If verbose Then
                Call $"Estimating PLAGE scores for {gsetIdxList.Count} gene sets.".debug
            End If

            Throw New NotImplementedException
        Else
            If verbose Then
                Call $"Estimating GSVA scores for {gsetIdxList.Count} gene sets.".debug
            End If
        End If

        Dim nsamples = expr.sampleID.Length
        Dim ngenes = expr.size
        Dim ngset = gsetIdxList.Count
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
        Dim compute_rank_score = Function(sort_idx_vec As Integer())
                                     Dim tmp As New Vector(0.0, num_genes)
                                     Dim v As Vector = Vector.seq(from:=num_genes, [to]:=1, by:=-1) - num_genes / 2
                                     tmp(sort_idx_vec) = v
                                     Return tmp
                                 End Function
        Dim sort_sgn_idxs = (From i As Integer
                             In Enumerable.Range(0, gene_density.ColumnDimension)
                             Let v = gene_density.ColumnVector(i)
                             Let order As Double() = v.Ranking(strategy:=Strategies.OrdinalRanking, desc:=True)
                             Let zeroI = order.Select(Function(d) CInt(d) - 1).ToArray
                             Select zeroI).ToArray
        Dim rank_scores = (From i As Integer
                           In Enumerable.Range(0, gene_density.ColumnDimension)
                           Let idx = sort_sgn_idxs(i)
                           Let v = compute_rank_score(idx)
                           Select v.ToArray) _
                           .AsMatrix _
                           .Transpose
        Dim m As New Matrix With {
            .expression = gsetIdxList _
                .Select(Function(gsetIdx)
                            Dim idx As Integer() = gsetIdx.Value.Select(Function(id) rowIndex.IndexOf(id)).ToArray
                            Dim test = ks_test_m(idx, rank_scores, sort_sgn_idxs, mxdiff, abs_ranking, tau, verbose)

                            Return New DataFrameRow With {
                                .experiments = test,
                                .geneID = gsetIdx.Key
                            }
                        End Function) _
                .ToArray
        }
        Dim pathIds As String() = gsetIdxList.Keys.ToArray

        m.sampleID = expr.sampleID
        m.eachGene(Sub(gene, i) gene.geneID = pathIds(i))

        For Each gene As DataFrameRow In m.expression
            Dim range As New DoubleRange(gene.experiments.Where(Function(d) Not d.IsNaNImaginary))

            For i As Integer = 0 To gene.experiments.Length - 1
                If Double.IsPositiveInfinity(gene.experiments(i)) Then
                    gene.experiments(i) = range.Max
                ElseIf Double.IsNegativeInfinity(gene.experiments(i)) Then
                    gene.experiments(i) = range.Min
                ElseIf gene.experiments(i).IsNaNImaginary Then
                    gene.experiments(i) = 0
                End If
            Next
        Next

        Return m
    End Function

    Private Function ks_test_m(gset_idxs As Integer(),
                               gene_density As NumericMatrix,
                               sort_idxs As Integer()(),
                               mxdiff As Boolean,
                               abs_ranking As Boolean,
                               tau As Double,
                               verbose As Boolean) As Double()

        Dim ngenes = gene_density.RowDimension
        Dim nsamples = gene_density.ColumnDimension
        Dim ngeneset = gset_idxs.Count
        Dim geneset_sample_es As Double() = C.ks_matrix_R(gene_density, sort_idxs, ngenes, gset_idxs, ngeneset, tau, nsamples, mxdiff, abs_ranking)

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
            gene_density = expr.expression _
                .Select(Function(r)
                            Dim ecdf = r.experiments.ECDF(sample_idxs)
                            Dim p As Double() = sample_idxs _
                                .Select(Function(i) ecdf(i)) _
                                .ToArray

                            Return p
                        End Function) _
                .AsMatrix
            gene_density = (gene_density / DirectCast(1 - gene_density, NumericMatrix)).Log
        End If

        Return gene_density
    End Function
End Module
