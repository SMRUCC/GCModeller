Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Public Module GSVA

    Public Function gsva(expr As Matrix,
                         gsetIdxList As Background,
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
                Call $"Estimating ssGSEA scores for {gsetIdxList.Count} gene sets.".__DEBUG_ECHO
            End If

            Throw New NotImplementedException
        ElseIf method = Methods.zscore Then
            If rnaseq Then
                Throw New InvalidProgramException("rnaseq=TRUE does not work with method='zscore'.")
            End If
            If verbose Then
                Call $"Estimating combined z-scores for {gsetIdxList.Count} gene sets.".__DEBUG_ECHO
            End If

            Throw New NotImplementedException
        ElseIf method = Methods.plage Then
            If rnaseq Then
                Throw New InvalidProgramException("rnaseq=TRUE does not work with method='plage'.")
            End If
            If verbose Then
                Call $"Estimating PLAGE scores for {gsetIdxList.Count} gene sets.".__DEBUG_ECHO
            End If

            Throw New NotImplementedException
        Else
            If verbose Then
                Call $"Estimating GSVA scores for {gsetIdxList.Count} gene sets.".__DEBUG_ECHO
            End If
        End If

        Dim nsamples = expr.sampleID.Length
        Dim ngenes = expr.size
        Dim ngset = gsetIdxList.Count
        Dim es_obs As Matrix = compute_geneset_es(
            expr,
            gsetIdxList,
            Sequence(nsamples).ToArray,
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

        If verbose Then
            If kernel Then
                If rnaseq Then
                    Call "Estimating ECDFs with Poisson kernels".__DEBUG_ECHO
                Else
                    Call "Estimating ECDFs with Gaussian kernels".__DEBUG_ECHO
                End If
            Else
                Call "Estimating ECDFs directly".__DEBUG_ECHO
            End If
        End If

        Dim gene_density As NumericMatrix = compute_gene_density(expr, sample_idxs, rnaseq, kernel)
        Dim compute_rank_score = Function(sort_idx_vec As Integer())
                                     Dim tmp As New Vector(0.0, num_genes)
                                     tmp(sort_idx_vec) = Vector.seq(from:=num_genes, [to]:=1) - num_genes / 2
                                     Return tmp
                                 End Function
        Dim sort_sgn_idxs = (From i As Integer
                             In Enumerable.Range(0, gene_density.ColumnDimension)
                             Let v = gene_density.ColumnVector(i)
                             Let order As Double() = v.Ranking(strategy:=Strategies.OrdinalRanking, desc:=True)
                             Select order.Select(Function(d) CInt(d)).ToArray).ToArray
        Dim rank_scores = (From i As Integer
                           In Enumerable.Range(0, gene_density.ColumnDimension)
                           Let idx = sort_sgn_idxs(i)
                           Let v = compute_rank_score(idx)
                           Select v.ToArray).AsMatrix
        Dim m As New Matrix With {
            .expression = gsetIdxList _
                .Select(Function(gsetIdx)
                            Return New DataFrameRow With {
                                .experiments = ks_test_m(gsetIdx.Value, rank_scores, sort_sgn_idxs, mxdiff, abs_ranking, tau, verbose),
                                .geneID = gsetIdx.Key
                            }
                        End Function) _
                .ToArray
        }
        Dim pathIds As String() = gsetIdxList.Keys.ToArray

        m.sampleID = expr.sampleID
        m.eachGene(Sub(gene, i) gene.geneID = pathIds(i))

        Return m
    End Function

    Private Function ks_test_m(gset_idxs As String(),
                               gene_density As NumericMatrix,
                               sort_idxs As Integer()(),
                               mxdiff As Boolean,
                               abs_ranking As Boolean,
                               tau As Double,
                               verbose As Boolean) As Double()

        Dim ngenes = gene_density.RowDimension
        Dim nsamples = gene_density.ColumnDimension
        Dim ngeneset = gset_idxs.Count
        Dim geneSet As Integer() = gset_idxs.Sequence.ToArray
        Dim geneset_sample_es As Double() = C.ks_matrix_R(gene_density, sort_idxs, ngenes, geneSet, ngeneset, tau, nsamples, mxdiff, abs_ranking)

        Return geneset_sample_es
    End Function

    Private Function compute_gene_density(expr As Matrix, sample_idxs As Integer(), rnaseq As Boolean, kernel As Boolean) As NumericMatrix
        Dim ntestsamples = expr.sampleID.Length
        Dim ngenes = expr.size
        Dim ndensitysamples = sample_idxs.Length
        Dim gene_density As NumericMatrix

        If kernel Then
            gene_density = C.matrix_density_R(expr.T, expr.T, (ntestsamples, ngenes), ndensitysamples, ntestsamples, ngenes, rnaseq)
        Else
            gene_density = expr.expression _
                .Select(Function(r)
                            Dim ecdf = r.experiments.ECDF(sample_idxs)
                            Dim p As Double() = sample_idxs.Select(Function(i) ecdf(i)).ToArray

                            Return p
                        End Function) _
                .AsMatrix
            gene_density = (gene_density / DirectCast(1 - gene_density, NumericMatrix)).Log
        End If

        Return gene_density
    End Function

    Private Function filterGeneSets(mapped_gset_idx_list As Dictionary(Of String, String()), min_sz As Integer, max_sz As Integer) As Dictionary(Of String, String())
        Return mapped_gset_idx_list _
            .Where(Function(d)
                       Return d.Value.Length >= min_sz AndAlso d.Value.Length <= max_sz
                   End Function) _
            .ToDictionary
    End Function

    ''' <summary>
    ''' filter out genes with constant expression values
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="method"></param>
    ''' <returns></returns>
    Public Function filterFeatures(expr As Matrix, method As Methods) As Matrix
        Dim sdGenes As Vector = expr.rowSds.Values.AsVector

        sdGenes(sdGenes < 0.0000000001) = Vector.Zero

        If sdGenes.Any(Function(x) x = 0.0 OrElse x.IsNaNImaginary) Then
            Call $"{(sdGenes = 0.0 OrElse sdGenes.IsNaNImaginary).Sum} genes with constant expression values throuhgout the samples.".Warning

            If method <> Methods.ssgsea Then
                Call "Since argument method!='ssgsea', genes with constant expression values are discarded.".Warning

                expr = expr(sdGenes > 0 & Not sdGenes.IsNaNImaginary)
            End If
        End If

        If expr.size < 2 Then
            Throw New InvalidProgramException("Less than two genes in the input assay object")
        Else
            Return expr
        End If
    End Function

    ''' <summary>
    ''' maps gene sets content in 'gsets' to 'features', where 'gsets'
    ''' Is a 'list' object with character string vectors as elements,
    ''' And 'features' is a character string vector object. it assumes
    ''' features In both input objects follow the same nomenclature
    ''' </summary>
    ''' <param name="gsets"></param>
    ''' <param name="features"></param>
    ''' <returns></returns>
    Public Function mapGeneSetsToFeatures(gsets As Background, features As String()) As Dictionary(Of String, String())
        Dim mapdgenesets = gsets.clusters _
            .ToDictionary(Function(c) c.ID,
                          Function(c)
                              Return c.Intersect(features).ToArray
                          End Function)

        If mapdgenesets.Values.IteratesALL.Count = 0 Then
            Throw New InvalidProgramException("No identifiers in the gene sets could be matched to the identifiers in the expression data.")
        Else
            Return mapdgenesets
        End If
    End Function
End Module



