#Region "Microsoft.VisualBasic::887cce171be016eb5f9e626c43a16e92, R#\gseakit\GSVA.vb"

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

'   Total Lines: 157
'    Code Lines: 94 (59.87%)
' Comment Lines: 48 (30.57%)
'    - Xml Docs: 95.83%
' 
'   Blank Lines: 15 (9.55%)
'     File Size: 6.39 KB


' Module GSVA
' 
'     Function: diff, gsva, matrixToDiff
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame.MultipleOmics
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSVA
Imports SMRUCC.genomics.Analysis.HTS.GSVA.Omics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' Gene Set Variation Analysis for microarray and RNA-seq data
''' </summary>
<Package("GSVA")>
<RTypeExport("mogsva_opts", GetType(MultiOmicsGsvaOptions))>
Module GSVA

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(GSVADiff()), AddressOf gsva_diff_df)
    End Sub

    Private Function gsva_diff_df(diffs As GSVADiff(), args As list, env As Environment) As rdataframe
        Dim df As New rdataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = diffs.Keys
        }

        Call df.add(NameOf(GSVADiff.t), From p As GSVADiff In diffs Select p.t)
        Call df.add(NameOf(GSVADiff.pvalue), From p As GSVADiff In diffs Select p.pvalue)
        Call df.add(NameOf(GSVADiff.adjPVal), From p As GSVADiff In diffs Select p.adjPVal)

        Return df
    End Function

    ''' <summary>
    ''' Gene Set Variation Analysis (GSVA) for microarray and RNA-seq data.
    ''' </summary>
    ''' <remarks>
    ''' GSVA is a non-parametric, unsupervised method for estimating variation
    ''' of gene set enrichment through the samples of an expression data set.
    ''' It performs a change in coordinate systems, transforming the data from a
    ''' gene-by-sample matrix to a gene-set-by-sample matrix, thereby allowing
    ''' the evaluation of pathway enrichment for each sample. This new matrix of
    ''' GSVA enrichment scores facilitates applying standard analytical methods
    ''' such as functional enrichment, survival analysis, clustering,
    ''' CNV-pathway analysis or cross-tissue pathway analysis, in a
    ''' pathway-centric manner.
    ''' Hänzelmann S., Castelo R. and Guinney J. GSVA: gene set variation analysis
    ''' for microarray and RNA-Seq data. BMC Bioinformatics, 14:7, 2013.
    ''' </remarks>
    ''' <param name="expr">
    ''' A raw gene expression data matrix. Acceptable inputs are an
    ''' <see cref="HTSMatrix"/> object or an R# <c>dataframe</c> whose rows are
    ''' features (genes) and whose columns are samples.
    ''' </param>
    ''' <param name="geneSet">
    ''' A gene-set collection supplied either as a GSEA
    ''' <see cref="Background"/> model or as anything that
    ''' <c>BackgroundHelper.CastBackground</c> can coerce into one (for example a
    ''' GMT file path or a list of gene-set objects).
    ''' </param>
    ''' <param name="method">
    ''' The enrichment scoring method:
    ''' <see cref="Methods.gsva"/> (default, kernel based ES),
    ''' <see cref="Methods.ssgsea"/>, <see cref="Methods.zscore"/> or
    ''' <see cref="Methods.plage"/>.
    ''' </param>
    ''' <param name="kcdf">
    ''' The kernel density estimation type used to estimate the cumulative
    ''' distribution function of each expression profile:
    ''' <see cref="KCDFs.Gaussian"/> for continuous data,
    ''' <see cref="KCDFs.Poisson"/> for count data, or
    ''' <see cref="KCDFs.none"/> for the empirical cumulative distribution
    ''' function. Defaults to <see cref="KCDFs.none"/>, i.e. the same behavior as
    ''' the underlying function when no kernel is requested.
    ''' </param>
    ''' <param name="min_sz">
    ''' Minimum gene-set size (number of mapped features). Gene sets with fewer
    ''' mapped features are removed from the analysis. Default 1.
    ''' </param>
    ''' <param name="max_sz">
    ''' Maximum gene-set size (number of mapped features). Gene sets with more
    ''' mapped features are removed from the analysis. Default unlimited.
    ''' </param>
    ''' <param name="mxdiff">
    ''' When TRUE (default) the ES range is transformed to the interval
    ''' [-1, 1] by using the difference between the largest and the smallest ES
    ''' values; otherwise the original ES range is kept.
    ''' </param>
    ''' <param name="tau">
    ''' Exponent (τ, default 1) applied to the weight of the cumulative
    ''' distribution function when <paramref name="method"/> is
    ''' <see cref="Methods.gsva"/>; higher values give more weight to the tails
    ''' of the distribution.
    ''' </param>
    ''' <param name="kernel">
    ''' Logical flag to use the Gaussian kernel to estimate the cumulative
    ''' distribution function when <paramref name="method"/> is
    ''' <see cref="Methods.gsva"/>. Ignored when a non-kernel
    ''' <paramref name="kcdf"/> is requested.
    ''' </param>
    ''' <param name="rnaseq">
    ''' Logical flag indicating that <paramref name="expr"/> is read count data
    ''' from an RNA-seq experiment; this switches the kernel to a Poisson
    ''' density when a kernel is used.
    ''' </param>
    ''' <param name="abs_ranking">
    ''' When TRUE (default FALSE) the enrichment scores are computed using the
    ''' absolute values of the ranking, which is appropriate for single-sample
    ''' (ssGSEA-style) scoring.
    ''' </param>
    ''' <param name="alpha">
    ''' The α (alpha) parameter (default 0.25) of the ssGSEA scoring method,
    ''' controlling the weight of the random-walk deviation.
    ''' </param>
    ''' <param name="normalize">
    ''' Logical flag (default TRUE) to normalize the enrichment scores so that
    ''' each gene set has a mean enrichment of zero across samples.
    ''' </param>
    ''' <param name="name_suffix">
    ''' Append the gene-set description (<c>names</c>) to the pathway identifier
    ''' as a suffix in the returned matrix, e.g. <c>"hsa00010 - Glycolysis"</c>.
    ''' </param>
    ''' <param name="verbose">
    ''' Logical flag (default FALSE) to emit progress and diagnostic messages
    ''' during the calculation.
    ''' </param>
    ''' <param name="env">The R# runtime environment (used for error reporting).</param>
    ''' <returns>
    ''' An <see cref="HTSMatrix"/> of GSVA enrichment scores with gene sets in
    ''' rows and samples in columns, or a <c>Message</c> describing the error
    ''' when the gene-set collection cannot be parsed.
    ''' </returns>
    <ExportAPI("gsva")>
    <RApiReturn(GetType(HTSMatrix))>
    Public Function gsva(expr As Object,
                         geneSet As Object,
                         Optional method As Methods = Methods.gsva,
                         Optional kcdf As KCDFs = KCDFs.none,
                         Optional min_sz As Integer = 1,
                         Optional max_sz As Integer = Integer.MaxValue,
                         Optional mxdiff As Boolean = True,
                         Optional tau As Double = 1,
                         Optional kernel As Boolean = True,
                         Optional rnaseq As Boolean = False,
                         Optional abs_ranking As Boolean = False,
                         Optional alpha As Double = 0.25,
                         Optional normalize As Boolean = True,
                         Optional name_suffix As Boolean = False,
                         Optional verbose As Boolean = False,
                         Optional env As Environment = Nothing) As Object
        Dim mat As HTSMatrix
        Dim background As [Variant](Of Background, Message) = BackgroundHelper.CastBackground(geneSet, env)

        If background Like GetType(Message) Then
            Return background.TryCast(Of Message)
        End If

        If TypeOf expr Is dataframe Then
            mat = New HTSMatrix With {
                .sampleID = DirectCast(expr, dataframe).colnames,
                .expression = DirectCast(expr, dataframe) _
                    .forEachRow _
                    .Select(Function(r)
                                Return New DataFrameRow With {
                                    .geneID = r.name,
                                    .experiments = CLRVector.asNumeric(r.ToArray)
                                }
                            End Function) _
                    .ToArray
            }
        Else
            mat = expr
        End If

        Dim gsva_result As HTSMatrix = mat.gsva(background,
                                                method,
                                                kcdf,
                                                min_sz,
                                                max_sz,
                                                mxdiff,
                                                tau,
                                                kernel,
                                                rnaseq,
                                                abs_ranking,
                                                alpha,
                                                normalize,
                                                verbose)

        If name_suffix Then
            For Each pathway As DataFrameRow In gsva_result.expression
                pathway.geneID = $"{pathway.geneID} - {background(pathway.geneID).names}"
            Next
        End If

        Return gsva_result
    End Function

    ''' <summary>
    ''' Assemble the multiple omics data layers into a <see cref="MultiOmicsDataset"/> object
    ''' for the multi-omics GSVA (moGSVA) analysis.
    ''' </summary>
    ''' <param name="layers">
    ''' the omics layer collection, each element could be an <see cref="OmicsLayer"/> object or
    ''' a tuple that could be cast into the omics layer model. Every layer should define its
    ''' unique omics name and the expression matrix whose rows are the features of that omics.
    ''' </param>
    ''' <param name="verbose">prints the progress and the diagnostic message of the layer alignment? Default ``TRUE``.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' a <see cref="MultiOmicsDataset"/> object which contains the sample aligned omics layers,
    ''' or an error message when the layer collection could not be assembled.
    ''' </returns>
    ''' <remarks>
    ''' The samples of all of the omics layers are aligned by the sample name: the intersection
    ''' of the sample names is used as the final sample axis of the multi-omics dataset.
    ''' </remarks>
    <ExportAPI("multiple_dataset")>
    <RApiReturn(GetType(MultiOmicsDataset))>
    Public Function mo_layers(<RRawVectorArgument(GetType(OmicsLayer))> layers As Object, Optional verbose As Boolean = True, Optional env As Environment = Nothing) As Object
        Dim pull As PipeIterator(Of OmicsLayer) = pipeline.Stream(Of OmicsLayer)(layers, env)

        If pull.isError Then
            Return pull.getError
        End If

        Return MultiOmicsDataset.Create(pull, verbose)
    End Function

    ''' <summary>
    ''' Build a <see cref="MultiOmicsGsvaOptions"/> configuration object that controls
    ''' the whole multi-omics GSVA (moGSVA) pipeline: per-omics enrichment scoring,
    ''' cross-omics weighted fusion with a consistency reward, and a gene-to-pathway
    ''' permutation test for significance assessment.
    ''' </summary>
    ''' <param name="score_method">
    ''' The enrichment scoring method applied independently inside each omics layer
    ''' (recommended <see cref="Methods.gsva"/>, the default). Other choices are
    ''' ssGSEA, z-score and PLAGE.
    ''' </param>
    ''' <param name="kcdf">
    ''' The kernel density estimation type used by the GSVA scoring step:
    ''' <see cref="KCDFs.Gaussian"/> for continuous data (default),
    ''' <see cref="KCDFs.Poisson"/> for count data, or
    ''' <see cref="KCDFs.none"/> for the empirical cumulative distribution function.
    ''' </param>
    ''' <param name="fusion_strategy">
    ''' The cross-omics weighted fusion strategy. Defaults to
    ''' <see cref="Fusion.FusionMethods.Equal"/> (a plain average, i.e. baseline 1).
    ''' Alternatives: variance weighting, information-entropy weighting,
    ''' mutual-information consistency weighting and a balanced scheme that also
    ''' applies the per-omics feature-count balancing term.
    ''' </param>
    ''' <param name="consistency_lambda">
    ''' The hyper-parameter λ (lambda) of the cross-omics consistency reward term.
    ''' A value of 0 disables the reward and reduces the fusion to a pure weighted
    ''' average. Default 0.25.
    ''' </param>
    ''' <param name="balance_feature_size">
    ''' Whether to superimpose the per-omics feature-count balancing term
    ''' 1/log2(2+|S_k|) so that omics layers with many mapped members cannot
    ''' dominate the fused score. Off by default so that the equal-weight fusion
    ''' stays strictly identical to the simple per-omics average.
    ''' </param>
    ''' <param name="min_size">
    ''' Minimum gene-set size (number of mapped features) after the feature
    ''' mapping step. Gene sets smaller than this are dropped. Default 5.
    ''' </param>
    ''' <param name="max_size">
    ''' Maximum gene-set size (number of mapped features) after the feature
    ''' mapping step. Gene sets larger than this are dropped. Default 500.
    ''' </param>
    ''' <param name="ssgsea_alpha">
    ''' The α (alpha) parameter of the ssGSEA scoring path. Default 0.25.
    ''' </param>
    ''' <param name="ssgsea_normalize">
    ''' Whether the ssGSEA scoring path normalizes the enrichment scores.
    ''' Default TRUE.
    ''' </param>
    ''' <param name="permutations">
    ''' Number of permutations for the gene-to-pathway permutation test.
    ''' 0 (the default) skips the significance assessment entirely.
    ''' </param>
    ''' <param name="permutation_method">
    ''' The scoring method used to build the permutation null distribution.
    ''' Defaults to ssGSEA (rank based) to avoid repeating the expensive
    ''' kernel density estimation.
    ''' </param>
    ''' <param name="permutation_pathways">
    ''' A subset of pathway identifiers to evaluate in the permutation test.
    ''' When nothing is supplied the first <paramref name="max_permutation_pathways"/>
    ''' pathways from the fused result are used.
    ''' </param>
    ''' <param name="permutation_gene_limit">
    ''' Upper bound on the number of features kept in the permutation view
    ''' (0 means no trimming). When trimming is applied, the target pathway
    ''' members are kept first and the remaining budget is filled with background
    ''' features using a fixed seed.
    ''' </param>
    ''' <param name="max_permutation_pathways">
    ''' Maximum number of pathways evaluated in the permutation test when no
    ''' explicit pathway subset is supplied. Default 40.
    ''' </param>
    ''' <param name="seed">
    ''' Random seed used for deterministic data derivation and permutation
    ''' shuffling so that runs are fully reproducible. Default 20261026.
    ''' </param>
    ''' <param name="compute_pooled">
    ''' Whether to also compute the ablation control "concatenate then single-omics
    ''' GSVA" baseline. Default TRUE.
    ''' </param>
    ''' <param name="verbose">
    ''' Whether to emit progress/diagnostic logging. Default TRUE.
    ''' </param>
    ''' <param name="env">The R# runtime environment (used for error reporting).</param>
    ''' <returns>
    ''' A fully populated <see cref="MultiOmicsGsvaOptions"/> object that can be
    ''' passed to the <c>mogsva</c> function.
    ''' </returns>
    ''' <remarks>
    ''' All parameters are optional and default to the values defined on the
    ''' <see cref="MultiOmicsGsvaOptions"/> class, so calling this function with
    ''' no arguments yields the same default configuration that the moGSVA engine
    ''' uses.
    ''' </remarks>
    <ExportAPI("mogsva_opts")>
    <RApiReturn(GetType(MultiOmicsGsvaOptions))>
    Public Function mogsva_opts(Optional score_method As Methods = Methods.gsva,
                                Optional kcdf As KCDFs = KCDFs.Gaussian,
                                Optional fusion_strategy As Fusion.FusionMethods = Fusion.FusionMethods.Equal,
                                Optional consistency_lambda As Double = 0.25,
                                Optional balance_feature_size As Boolean = False,
                                Optional min_size As Integer = 5,
                                Optional max_size As Integer = 500,
                                Optional ssgsea_alpha As Double = 0.25,
                                Optional ssgsea_normalize As Boolean = True,
                                Optional permutations As Integer = 0,
                                Optional permutation_method As Methods = Methods.ssgsea,
                                Optional permutation_pathways As String() = Nothing,
                                Optional permutation_gene_limit As Integer = 0,
                                Optional max_permutation_pathways As Integer = 40,
                                Optional seed As Integer = 20261026,
                                Optional compute_pooled As Boolean = True,
                                Optional verbose As Boolean = True,
                                Optional env As Environment = Nothing) As MultiOmicsGsvaOptions

        Return New MultiOmicsGsvaOptions With {
            .ScoreMethod = score_method,
            .Kcdf = kcdf,
            .FusionStrategy = fusion_strategy,
            .ConsistencyLambda = consistency_lambda,
            .BalanceFeatureSize = balance_feature_size,
            .MinSize = min_size,
            .MaxSize = max_size,
            .SsGseaAlpha = ssgsea_alpha,
            .SsGseaNormalize = ssgsea_normalize,
            .Permutations = permutations,
            .PermutationMethod = permutation_method,
            .PermutationPathways = permutation_pathways,
            .PermutationGeneLimit = permutation_gene_limit,
            .MaxPermutationPathways = max_permutation_pathways,
            .Seed = seed,
            .ComputePooled = compute_pooled,
            .Verbose = verbose
        }
    End Function

    ''' <summary>
    ''' Run the multi-omics GSVA (moGSVA) analysis on the given multi-omics dataset.
    ''' </summary>
    ''' <param name="x">the multi-omics dataset object that is created by the ``mo_layers`` function.</param>
    ''' <param name="geneSet">
    ''' the gene set collection, it could be a GSEA <see cref="Background"/> model, a GMT file
    ''' path or other data that could be cast into the background model.
    ''' </param>
    ''' <param name="opts">the running configuration object that is created by the ``mogsva_opts`` function.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' a <see cref="MultiOmicsGsvaResult"/> object which contains the fused pathway activity
    ''' matrix, the per-omics scores, the pathway weights, the omics contribution decomposition
    ''' and the permutation test p-value table (when ``permutations`` is greater than zero).
    ''' </returns>
    ''' <remarks>
    ''' The scoring of each omics layer is performed independently (the kernel density
    ''' estimation never mixes the features from the different omics), then the omics specific
    ''' enrichment scores are fused by the configured weight strategy.
    ''' </remarks>
    <ExportAPI("mogsva")>
    <RApiReturn(GetType(MultiOmicsGsvaResult))>
    Public Function mogsva(x As MultiOmicsDataset, geneSet As Object, opts As MultiOmicsGsvaOptions, Optional env As Environment = Nothing) As Object
        Dim background As [Variant](Of Background, Message) = BackgroundHelper.CastBackground(geneSet, env)

        If background Like GetType(Message) Then
            Return background.TryCast(Of Message)
        End If

        Dim gsva_result As MultiOmicsGsvaResult = MultiOmicsGsva.Compute(x, background, opts)

        Return gsva_result
    End Function

    ''' <summary>
    ''' different analysis of the gsva result
    ''' </summary>
    ''' <param name="gsva">the gsva analysis result</param>
    ''' <param name="compares">
    ''' the <see cref="DataAnalysis"/> comparision collection
    ''' </param>
    ''' <returns>
    ''' this function returns a array of the <see cref="GSVADiff"/> object which is compares the different of the gsva score via the limma function. 
    ''' for get the different score of gsva score in raw <see cref="LimmaTable"/> array type, please use the ``geneExpression::limma`` function for analysis the gsva difference result across two condition groups.
    ''' </returns>
    <ExportAPI("diff")>
    <RApiReturn(GetType(GSVADiff))>
    Public Function diff(gsva As HTSMatrix, compares As DataAnalysis) As Object
        If compares.size <> 2 Then
            Throw New InvalidProgramException
        End If

        Dim limmaResult As LimmaTable() = Limma.LmFit(gsva, compares).ToArray
        Dim diffResult = limmaResult _
            .Select(Function(term)
                        Return New GSVADiff With {
                            .pathName = term.id,
                            .pvalue = term.P_Value,
                            .t = term.t,
                            .adjPVal = term.adj_P_Val
                        }
                    End Function) _
            .ToArray

        Return diffResult
    End Function

    ''' <summary>
    ''' Convert the limma differential analysis result table into the GSVA difference objects.
    ''' </summary>
    ''' <param name="diff">the dataframe object of the limma differential analysis result.</param>
    ''' <param name="pathId">the column name of the pathway id in the given dataframe. Default ``pathNames``.</param>
    ''' <param name="t">the column name of the t statistics. Default ``t``.</param>
    ''' <param name="pvalue">the column name of the p-value. Default ``pvalue``.</param>
    ''' <returns>an array of the <see cref="GSVADiff"/> objects.</returns>
    <ExportAPI("matrix_to_diff")>
    Public Function matrixToDiff(diff As dataframe,
                                 Optional pathId As String = "pathNames",
                                 Optional t As String = "t",
                                 Optional pvalue As String = "pvalue") As GSVADiff()

        Dim pathNames As String() = CLRVector.asCharacter(diff(pathId))
        Dim testVal As Double() = CLRVector.asNumeric(diff(t))
        Dim pVal As Double() = CLRVector.asNumeric(diff(pvalue))

        Return pathNames _
            .Select(Function(ref, i)
                        Return New GSVADiff With {
                            .pathName = ref,
                            .pvalue = pVal(i),
                            .t = testVal(i)
                        }
                    End Function) _
            .ToArray
    End Function
End Module
