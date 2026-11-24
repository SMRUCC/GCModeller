#Region "Microsoft.VisualBasic::16a726affb663b57ae539f8be4e06a58, R#\gseakit\KOBAS.vb"

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

    ' Module KOBAS
    ' 
    '     Function: KEGGPathway
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame.Formats
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KOBAS
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' ### KOBAS toolkit
''' 
''' The KOBAS (KEGG Orthology Based Annotation System) enrichment analysis toolkit.
''' This package provides the VB.NET port of the KOBAS 3.0 enrichment calculation
''' algorithms:
''' 
''' + ``kobas.gsea``: GSEA analysis of a expression matrix against a gene set collection,
''' + ``kobas.identify``: four statistical enrichment tests (hypergeometric / Fisher /
'''   binomial / chi-square) for a foreground gene list against a background gene list,
''' + ``kobas.plage`` / ``kobas.gage``: SNBEA style pathway level analysis,
''' + ``kobas.term_similarity`` / ``kobas.term_cluster``: term similarity and clustering
'''   of the enrichment result terms,
''' + ``kobas.fdr``: multiple testing correction (BH / BY / q-value),
''' + ``kobas.fisher_test`` / ``kobas.hyper_test`` / ``kobas.binom_test`` / ``kobas.chisq_test``:
'''   the exact statistical tests used by the enrichment analysis.
''' </summary>
''' <remarks>
''' The translated algorithms follow the standard GSEA / Fisher definitions rather than
''' the original KOBAS implementation where the original code deviates from the standard
''' definition (for example the miss step of the running enrichment score or the
''' construction of the 2x2 contingency table). See the remarks of the individual
''' functions for the detailed notes.
''' </remarks>
<Package("KOBAS")>
Module KOBAS

    ''' <summary>
    ''' Register the data.frame conversion handlers of the KOBAS result objects.
    ''' </summary>
    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(GseaResult()), AddressOf gseaResultTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(EnrichmentRow()), AddressOf enrichmentRowTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(SnbeaResult()), AddressOf snbeaResultTable)
    End Sub

    ''' <summary>
    ''' Read KOBAS enrichment output result table file
    ''' </summary>
    ''' <param name="file">the KOBAS enrichment result file path, the file may contains more than one database section.</param>
    ''' <returns>
    ''' the enrichment terms that are read from the ``KEGG PATHWAY`` section of the given file,
    ''' an empty array will be returned when that section is missing.
    ''' </returns>
    <ExportAPI("read.KEGGpathway")>
    Public Function KEGGPathway(file As String) As EnrichmentTerm()
        Return file.SplitTable _
            .Where(Function(part) part.name = "KEGG PATHWAY") _
            .FirstOrDefault _
            .SafeQuery _
            .ToArray
    End Function

    ''' <summary>
    ''' Load a gene set collection from a GMT file as a GSEA background model.
    ''' </summary>
    ''' <param name="gmt">the GMT file path.</param>
    ''' <param name="min_size">
    ''' the minimum number of members of the gene set, gene sets smaller than this
    ''' value will be removed. Default ``0`` means no filtering.
    ''' </param>
    ''' <param name="max_size">
    ''' the maximum number of members of the gene set, gene sets larger than this
    ''' value will be removed. Default ``2147483647`` means no filtering.
    ''' </param>
    ''' <param name="duplicate">
    ''' the strategy for the duplicated gene set id in the GMT file, could be
    ''' ``error`` (default, throw an exception) or ``keep`` (keep the first one).
    ''' </param>
    ''' <returns>a <see cref="Background"/> model.</returns>
    ''' <remarks>
    ''' The ``size`` property of the returned background model is the union size of all
    ''' gene set members, it is NOT the number of genes in the expression matrix. The
    ''' GSEA analysis always uses the number of genes in the ranked list as the
    ''' population size ``N``.
    ''' </remarks>
    <ExportAPI("kobas.background")>
    <RApiReturn(GetType(Background))>
    Public Function KobasBackgroundFile(gmt As String,
                                        Optional min_size As Integer = 0,
                                        Optional max_size As Integer = Integer.MaxValue,
                                        Optional duplicate As String = "error") As Background

        Dim policy As DuplicateGeneSetPolicy = DuplicateGeneSetPolicy.ThrowError

        Select Case If(duplicate, "").Trim().ToLowerInvariant()
            Case "keep", "first"
                policy = DuplicateGeneSetPolicy.KeepFirst
            Case "error", "throw", ""
                policy = DuplicateGeneSetPolicy.ThrowError
        End Select

        Dim model As Background = GmtFile.Load(gmt, policy)

        Return model.FilterBySize(min_size, max_size)
    End Function

    ''' <summary>
    ''' ### Run the KOBAS style GSEA analysis
    ''' 
    ''' Gene Set Enrichment Analysis with the KOBAS ranking and permutation scheme: the
    ''' genes are ranked by the signal-to-noise ratio or the t statistics between the
    ''' two phenotype groups, then a weighted Kolmogorov-Smirnov like running sum is
    ''' evaluated for each gene set. The significance of the enrichment score is
    ''' estimated by permuting the phenotype class labels.
    ''' </summary>
    ''' <param name="expr">
    ''' the expression matrix, rows are genes and columns are samples. The value could be
    ''' a matrix object, a ``dataframe`` object (rows are genes and columns are samples),
    ''' or a file path of the ``gct`` / KOBAS plain text expression matrix.
    ''' </param>
    ''' <param name="cls">
    ''' the phenotype class labels: could be a ``phenotype`` object, a ``CLS`` file path,
    ''' an integer vector (``0`` for the first phenotype group and ``1`` for the second
    ''' group), or a character vector with exactly two distinct class labels.
    ''' </param>
    ''' <param name="gmt">
    ''' the gene set collection: a <see cref="Background"/> model or a GMT file path.
    ''' </param>
    ''' <param name="method">
    ''' the gene ranking method: ``snr`` (signal to noise ratio, default) or ``ttest``.
    ''' </param>
    ''' <param name="weight">
    ''' the weight of the enrichment score, ``0`` for the unweighted Kolmogorov-Smirnov
    ''' statistic, ``1`` (default) for the classic GSEA weighting and ``2`` for the
    ''' over-weighted score. Any other positive value ``p`` will use ``|rank|^p``.
    ''' </param>
    ''' <param name="permutations">
    ''' the number of the phenotype label permutations, default ``1000``. Set to ``0`` for
    ''' skipping the significance estimation (then the ``pvalue`` and ``FDR`` columns of
    ''' the result table will be ``NA`` and ``NES`` equals to ``ES``).
    ''' </param>
    ''' <param name="min_size">the minimum number of the mapped members of a gene set, default ``15``.</param>
    ''' <param name="max_size">the maximum number of the mapped members of a gene set, default ``500``.</param>
    ''' <param name="seed">the random seed of the permutation, default ``20261026``.</param>
    ''' <param name="keep_running_sum">
    ''' keeps the running sum of each gene set in the result objects? Default ``FALSE``
    ''' because the running sum occupies a lot of memory for large gene set collections.
    ''' </param>
    ''' <param name="verbose">prints the progress message? Default ``TRUE``.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' a ``dataframe`` with the gene set id as the row names and the columns:
    ''' ``name``, ``description``, ``size``, ``ES``, ``NES``, ``pvalue``, ``FDR``,
    ''' ``es_index``, ``leading_edge`` (the hit genes before/after the enrichment score
    ''' position, joined by ``;``) and ``hit_genes``.
    ''' </returns>
    ''' <remarks>
    ''' A positive ES means that the gene set is enriched in the first phenotype group
    ''' (the samples labeled with ``0``) because the KOBAS ranking score is
    ''' ``mean(group0) - mean(group1)``.
    ''' </remarks>
    <ExportAPI("kobas.gsea")>
    <RApiReturn(GetType(dataframe))>
    Public Function KobasGseaAnalysis(expr As Object,
                                      cls As Object,
                                      gmt As Object,
                                      Optional method As String = "snr",
                                      Optional weight As Double = 1,
                                      Optional permutations As Integer = 1000,
                                      Optional min_size As Integer = 15,
                                      Optional max_size As Integer = 500,
                                      Optional seed As Integer = 20261026,
                                      Optional keep_running_sum As Boolean = False,
                                      Optional verbose As Boolean = True,
                                      Optional env As Environment = Nothing) As Object

        Dim pheno = AsPhenotype(cls, env)

        If TypeOf pheno Is Message Then
            Return pheno
        End If

        Dim expression = AsMatrix(expr, DirectCast(pheno, phenotype), env)

        If TypeOf expression Is Message Then
            Return expression
        End If

        Dim model As Background = AsBackground(gmt)

        If model Is Nothing Then
            Return RInternal.debug.stop("the given gene set collection could not be loaded, expect a gsea background model or a GMT file path!", env)
        End If

        Dim options As New GseaOptions With {
            .Method = If(String.Equals(If(method, "").Trim(), "ttest", StringComparison.OrdinalIgnoreCase),
                         GeneRankingMethod.TTest,
                         GeneRankingMethod.Snr),
            .Weight = weight,
            .Permutations = permutations,
            .MinSize = min_size,
            .MaxSize = max_size,
            .Seed = seed,
            .KeepRunningSum = keep_running_sum,
            .Verbose = verbose
        }

        Try
            Return GseaTable(KobasGsea.Compute(DirectCast(expression, HTSMatrix), DirectCast(pheno, phenotype), model, options))
        Catch ex As Exception
            Return RInternal.debug.stop(ex, env)
        End Try
    End Function

    ''' <summary>
    ''' ### Run the KOBAS style over-representation (enrichment) analysis
    ''' 
    ''' Perform the over-representation analysis with the KOBAS statistical tests: the
    ''' foreground gene list is mapped onto the given gene set collection, then the
    ''' members of each term are compared with the background gene list by the
    ''' hypergeometric test, the Fisher exact test, the binomial test or the chi-square
    ''' test.
    ''' </summary>
    ''' <param name="foreground">
    ''' the foreground gene list: a character vector of the gene ids, or the file path of
    ''' a KOBAS ``annotate`` output file (the ``query`` column will be used).
    ''' </param>
    ''' <param name="background">
    ''' the background gene list: a character vector of the gene ids, or the file path of
    ''' a KOBAS ``annotate`` output file. It must be a superset of the
    ''' <paramref name="foreground"/> for the Fisher and the chi-square test.
    ''' </param>
    ''' <param name="gmt">the gene set collection: a <see cref="Background"/> model or a GMT file path.</param>
    ''' <param name="method">
    ''' the statistical test: ``h`` for the Fisher exact test (default, also the
    ''' hypergeometric test), ``b`` for the binomial test, ``c`` for the chi-square test
    ''' and ``x`` for the frequency list only.
    ''' </param>
    ''' <param name="fdr">
    ''' the multiple testing correction: ``BH`` (default), ``BY``, ``QVALUE`` or ``None``.
    ''' </param>
    ''' <param name="cutoff">
    ''' the minimum number of the background members of a term, terms with fewer
    ''' background members are not used in the statistical test. Default ``5``.
    ''' </param>
    ''' <param name="database">
    ''' the database label that will be written into the ``database`` column, default ``K``
    ''' (KEGG PATHWAY).
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' a ``dataframe`` with the term id as the row names and the columns:
    ''' ``name``, ``database``, ``input_number``, ``background_number``, ``pvalue``,
    ''' ``p_adjust`` and ``input`` (the enriched gene ids joined by ``;``).
    ''' </returns>
    <ExportAPI("kobas.identify")>
    <RApiReturn(GetType(dataframe))>
    Public Function KobasIdentifyAnalysis(foreground As Object,
                                          background As Object,
                                          gmt As Object,
                                          Optional method As String = "h",
                                          Optional fdr As String = "BH",
                                          Optional cutoff As Integer = 5,
                                          Optional database As String = "K",
                                          Optional env As Environment = Nothing) As Object

        Dim model As Background = AsBackground(gmt)

        If model Is Nothing Then
            Return RInternal.debug.stop("the given gene set collection could not be loaded, expect a gsea background model or a GMT file path!", env)
        End If

        Dim fgGenes As String() = AsGenes(foreground)
        Dim bgGenes As String() = AsGenes(background)

        If fgGenes.Length = 0 Then
            Return RInternal.debug.stop("the foreground gene list could not be empty!", env)
        End If
        If bgGenes.Length = 0 Then
            Return RInternal.debug.stop("the background gene list could not be empty!", env)
        End If

        Dim testMethod As EnrichmentTestMethod
        Dim fdrMethod As FdrMethod

        Try
            testMethod = TwoSampleTest.Parse(method)
            fdrMethod = FdrAdjust.Parse(fdr)
        Catch ex As Exception
            Return RInternal.debug.stop(ex, env)
        End Try

        Dim fg As TermDistribution = TermDistribution.Create(model, fgGenes, database)
        Dim bg As TermDistribution = TermDistribution.Create(model, bgGenes, database)
        Dim keeps As String() = bg.Terms _
            .Where(Function(term) term.Value.Size >= cutoff) _
            .Select(Function(term) term.Key) _
            .ToArray

        If keeps.Length = 0 Then
            Return RInternal.debug.stop($"all of the terms have less than {cutoff} background members!", env)
        End If

        fg = fg.RestrictTo(keeps)
        bg = bg.RestrictTo(keeps)

        Try
            Dim rows As EnrichmentRow() = TwoSampleTest.Compare(fg, bg, testMethod, fgGenes.Length, bgGenes.Length)

            If rows.Length = 0 Then
                Return RInternal.debug.stop("no term is shared between the foreground and the background gene list!", env)
            End If
            If testMethod <> EnrichmentTestMethod.Frequency Then
                Dim adjusted As Double() = FdrAdjust.Adjust(rows.Select(Function(row) row.PValue), fdrMethod)

                For i As Integer = 0 To rows.Length - 1
                    rows(i).CorrectedPValue = adjusted(i)
                Next
            End If

            Return EnrichmentRowsTable(rows, has_adjust:=testMethod <> EnrichmentTestMethod.Frequency)
        Catch ex As Exception
            Return RInternal.debug.stop(ex, env)
        End Try
    End Function

    ''' <summary>
    ''' Read a KOBAS ``annotate`` output file as a ``dataframe``.
    ''' </summary>
    ''' <param name="file">the ``annotate`` output file path.</param>
    ''' <returns>
    ''' a ``dataframe`` with the query id as the row names and the columns ``links``
    ''' (the mapped gene ids joined by ``;``) and ``size``.
    ''' </returns>
    <ExportAPI("kobas.read_annotate")>
    <RApiReturn(GetType(dataframe))>
    Public Function KobasAnnotateTable(file As String) As dataframe
        Dim document As AnnotateDocument = AnnotateFile.Load(file)
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = document.Entries.Select(Function(entry) entry.Query).ToArray
        }

        Call table.add("links", document.Entries.Select(Function(entry) entry.Links.JoinBy(";")))
        Call table.add("size", document.Entries.Select(Function(entry) entry.Links.Length))

        Return table
    End Function

    ''' <summary>
    ''' Multiple testing correction of the p-values.
    ''' </summary>
    ''' <param name="pvalue">the raw p-values.</param>
    ''' <param name="method">
    ''' the correction method: ``BH`` (Benjamini and Hochberg, default), ``BY``
    ''' (Benjamini and Yekutieli), ``QVALUE`` (Storey) or ``None``.
    ''' </param>
    ''' <returns>the adjusted p-values in the same order as the input.</returns>
    ''' <remarks>
    ''' The q-value is estimated with a fixed lambda (0.5) of the Storey's pi0 estimator,
    ''' the R ``qvalue`` package uses a smoothed lambda grid instead, so the result may
    ''' differ slightly from the R package.
    ''' </remarks>
    <ExportAPI("kobas.fdr")>
    Public Function KobasFdr(pvalue As Double(), Optional method As String = "BH") As Double()
        Return FdrAdjust.Adjust(pvalue, FdrAdjust.Parse(method))
    End Function

    ''' <summary>
    ''' ### PLAGE pathway level analysis (KOBAS SNBEA)
    ''' 
    ''' Score each gene set of the expression matrix with the PLAGE method (the first
    ''' right singular vector of the row standardized gene set submatrix), then compare
    ''' the activity scores between the two phenotype groups.
    ''' </summary>
    ''' <param name="expr">
    ''' the expression matrix (matrix object, ``dataframe`` or expression matrix file path).
    ''' </param>
    ''' <param name="cls">the phenotype class labels, same as the ``cls`` parameter of ``kobas.gsea``.</param>
    ''' <param name="gmt">the gene set collection: a <see cref="Background"/> model or a GMT file path.</param>
    ''' <param name="min_size">the minimum number of the mapped members of a gene set, default ``5``.</param>
    ''' <param name="max_size">the maximum number of the mapped members of a gene set, default ``500``.</param>
    ''' <param name="fdr">the multiple testing correction method, default ``BH``.</param>
    ''' <param name="data_type">
    ''' the data type of the expression matrix: ``rseq`` (RNA-seq read counts, default) or
    ''' ``ma`` (microarray data).
    ''' </param>
    ''' <param name="normalize">
    ''' performs the data normalization before the analysis? Default ``TRUE``: the RNA-seq
    ''' data will be transformed by the log2 CPM with the prior count and the microarray
    ''' data will be quantile normalized.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' a ``dataframe`` with the gene set id as the row names and the columns: ``name``,
    ''' ``description``, ``size``, ``statistic``, ``pvalue``, ``p_adjust`` and ``genes``.
    ''' </returns>
    ''' <remarks>
    ''' The differential analysis uses the classic Welch two sample t-test instead of the
    ''' limma moderated t-test because there is no moderated t implementation in the
    ''' GCModeller core library, so the p-values are slightly more conservative than the
    ''' limma result.
    ''' </remarks>
    <ExportAPI("kobas.plage")>
    <RApiReturn(GetType(dataframe))>
    Public Function KobasPlageAnalysis(expr As Object,
                                       cls As Object,
                                       gmt As Object,
                                       Optional min_size As Integer = 5,
                                       Optional max_size As Integer = 500,
                                       Optional fdr As String = "BH",
                                       Optional data_type As String = "rseq",
                                       Optional normalize As Boolean = True,
                                       Optional env As Environment = Nothing) As Object

        Return RunSnbea(expr, cls, gmt, min_size, max_size, fdr, data_type, normalize, plage:=True, env:=env)
    End Function

    ''' <summary>
    ''' ### GAGE style gene set enrichment analysis (KOBAS SNBEA)
    ''' 
    ''' Aggregate the gene level t statistics of each gene set into a gene set level
    ''' z-score (``sum(t) / sqrt(n)``) and estimate the significance from the upper tail
    ''' of the standard normal distribution.
    ''' </summary>
    ''' <param name="expr">the expression matrix (matrix object, ``dataframe`` or expression matrix file path).</param>
    ''' <param name="cls">the phenotype class labels, same as the ``cls`` parameter of ``kobas.gsea``.</param>
    ''' <param name="gmt">the gene set collection: a <see cref="Background"/> model or a GMT file path.</param>
    ''' <param name="min_size">the minimum number of the mapped members of a gene set, default ``5``.</param>
    ''' <param name="max_size">the maximum number of the mapped members of a gene set, default ``500``.</param>
    ''' <param name="fdr">the multiple testing correction method, default ``BH``.</param>
    ''' <param name="data_type">the data type of the expression matrix: ``rseq`` (default) or ``ma``.</param>
    ''' <param name="normalize">performs the data normalization before the analysis? Default ``TRUE``.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' a ``dataframe`` with the gene set id as the row names and the columns: ``name``,
    ''' ``description``, ``size``, ``statistic``, ``pvalue``, ``p_adjust`` and ``genes``.
    ''' </returns>
    ''' <remarks>
    ''' The returned p-value is the upper tail probability, that means the gene set is
    ''' enriched in the second phenotype group (the samples labeled with ``1``).
    ''' </remarks>
    <ExportAPI("kobas.gage")>
    <RApiReturn(GetType(dataframe))>
    Public Function KobasGageAnalysis(expr As Object,
                                      cls As Object,
                                      gmt As Object,
                                      Optional min_size As Integer = 5,
                                      Optional max_size As Integer = 500,
                                      Optional fdr As String = "BH",
                                      Optional data_type As String = "rseq",
                                      Optional normalize As Boolean = True,
                                      Optional env As Environment = Nothing) As Object

        Return RunSnbea(expr, cls, gmt, min_size, max_size, fdr, data_type, normalize, plage:=False, env:=env)
    End Function

    ''' <summary>
    ''' Compute the pairwise similarity matrix of the given term list.
    ''' </summary>
    ''' <param name="terms">
    ''' a named ``list`` of the character vectors, the name of each slot is the term id
    ''' and the value is the member gene id vector of that term (for example the result
    ''' of ``as.geneSet`` of a gsea background model).
    ''' </param>
    ''' <param name="method">the similarity method: ``kappa`` (Cohen's kappa, default) or ``jaccard``.</param>
    ''' <param name="universe">
    ''' the size of the gene universe that is used by the kappa method. Default ``-1``
    ''' means the union size of all of the given terms.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' a ``dataframe`` of the similarity matrix, both the row names and the column names
    ''' are the term ids and the diagonal values are ``1``.
    ''' </returns>
    <ExportAPI("kobas.term_similarity")>
    <RApiReturn(GetType(dataframe))>
    Public Function KobasTermSimilarity(terms As list,
                                        Optional method As String = "kappa",
                                        Optional universe As Integer = -1,
                                        Optional env As Environment = Nothing) As Object

        If terms Is Nothing OrElse terms.is_empty Then
            Return RInternal.debug.stop("the given term list could not be empty!", env)
        End If

        Dim names As String() = terms.slots.Keys.ToArray
        Dim sets As String()() = names _
            .Select(Function(id) CLRVector.asCharacter(terms.slots(id))) _
            .ToArray
        Dim size As Integer = If(universe > 0, universe, sets.SelectMany(Function(gene) gene).Distinct().Count())

        If size < sets.Max(Function(gene) gene.Length) Then
            size = sets.Max(Function(gene) gene.Length)
        End If

        Dim similarities As Double()() = TermSimilarity.Matrix(sets, size, AsSimilarity(method))
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = names
        }

        For i As Integer = 0 To names.Length - 1
            Dim columnIndex As Integer = i
            Dim column As Double() = Enumerable.Range(0, names.Length) _
                .Select(Function(row) similarities(row)(columnIndex)) _
                .ToArray

            table.columns(names(i)) = column
        Next

        Return table
    End Function

    ''' <summary>
    ''' Cluster the given terms by the pairwise similarity.
    ''' </summary>
    ''' <param name="terms">
    ''' a named ``list`` of the character vectors, see the ``terms`` parameter of
    ''' ``kobas.term_similarity``.
    ''' </param>
    ''' <param name="threshold">the similarity threshold of the term cluster, default ``0.35``.</param>
    ''' <param name="method">the similarity method: ``kappa`` (default) or ``jaccard``.</param>
    ''' <param name="universe">the gene universe size of the kappa method, default ``-1`` (auto).</param>
    ''' <param name="pvalue">
    ''' an optional numeric vector of the p-values of the terms, it is used for scoring the
    ''' term cluster (the geometric mean of ``-log10(p)``). Default ``NULL`` means ``NA``.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' a ``dataframe`` with the cluster index as the row names and the columns:
    ''' ``size`` (the number of the terms in the cluster), ``score``, ``members`` (the term
    ''' ids joined by ``;``) and ``index`` (the term index in the input term list).
    ''' </returns>
    ''' <remarks>
    ''' The cluster self test of the KOBAS algorithm requires more than 3 pairwise
    ''' similarities, that means a cluster should contain at least 4 terms.
    ''' </remarks>
    <ExportAPI("kobas.term_cluster")>
    <RApiReturn(GetType(dataframe))>
    Public Function KobasTermCluster(terms As list,
                                     Optional threshold As Double = 0.35,
                                     Optional method As String = "kappa",
                                     Optional universe As Integer = -1,
                                     Optional pvalue As Object = Nothing,
                                     Optional env As Environment = Nothing) As Object

        If terms Is Nothing OrElse terms.is_empty Then
            Return RInternal.debug.stop("the given term list could not be empty!", env)
        End If

        Dim names As String() = terms.slots.Keys.ToArray
        Dim sets As String()() = names _
            .Select(Function(id) CLRVector.asCharacter(terms.slots(id))) _
            .ToArray
        Dim size As Integer = If(universe > 0, universe, sets.SelectMany(Function(gene) gene).Distinct().Count())

        If size < sets.Max(Function(gene) gene.Length) Then
            size = sets.Max(Function(gene) gene.Length)
        End If

        Dim pvalues As Double() = Nothing

        If pvalue IsNot Nothing Then
            pvalues = CLRVector.asNumeric(pvalue)
        End If

        Dim similarities As Double()() = TermSimilarity.Matrix(sets, size, AsSimilarity(method))
        Dim clusters As List(Of Integer()) = TermClustering.Iterate(
            similarities,
            TermClustering.Initialize(similarities, threshold))
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = Enumerable.Range(1, clusters.Count).Select(Function(i) $"cluster{i}").ToArray
        }

        Call table.add("size", clusters.Select(Function(cluster) cluster.Length))
        Call table.add("score", clusters.Select(Function(cluster) ClusterScore(cluster, pvalues)))
        Call table.add("members", clusters.Select(Function(cluster) cluster.Select(Function(i) names(i)).JoinBy(";")))
        Call table.add("index", clusters.Select(Function(cluster) cluster.Select(Function(i) (i + 1).ToString()).JoinBy(";")))

        Return table
    End Function

    ''' <summary>
    ''' The Fisher exact test (greater / enrichment tail) of a 2x2 contingency table.
    ''' </summary>
    ''' <param name="a">the upper left cell: the members of the term in the foreground list.</param>
    ''' <param name="b">the upper right cell: the foreground members that are not in the term.</param>
    ''' <param name="c">the lower left cell: the term members that are not in the foreground.</param>
    ''' <param name="d">the lower right cell: the members that are neither in the term nor in the foreground.</param>
    ''' <returns>the p-value.</returns>
    <ExportAPI("kobas.fisher_test")>
    Public Function KobasFisherTest(a As Integer, b As Integer, c As Integer, d As Integer) As Double
        Return ExactTests.FisherGreater(a, b, c, d)
    End Function

    ''' <summary>
    ''' The hypergeometric test (upper tail) of a term.
    ''' </summary>
    ''' <param name="q">the observed number of the term members in the foreground list.</param>
    ''' <param name="m">the number of the term members in the background.</param>
    ''' <param name="n">the total number of the genes in the background.</param>
    ''' <param name="k">the size of the foreground gene list.</param>
    ''' <returns>the p-value.</returns>
    <ExportAPI("kobas.hyper_test")>
    Public Function KobasHyperTest(q As Integer, m As Integer, n As Integer, k As Integer) As Double
        Return ExactTests.HypergeometricUpperTail(q, m, n, k)
    End Function

    ''' <summary>
    ''' The exact binomial test (upper tail) of a term.
    ''' </summary>
    ''' <param name="m1">the number of the term members in the foreground list.</param>
    ''' <param name="n1">the size of the foreground gene list.</param>
    ''' <param name="m2">the number of the term members in the background.</param>
    ''' <param name="n2">the size of the background gene list.</param>
    ''' <returns>the p-value.</returns>
    <ExportAPI("kobas.binom_test")>
    Public Function KobasBinomTest(m1 As Integer, n1 As Integer, m2 As Integer, n2 As Integer) As Double
        Return ExactTests.BinomialUpperTail(m1, n1, If(n2 = 0, 0, m2 / CDbl(n2)))
    End Function

    ''' <summary>
    ''' The chi-square test (Yates continuity correction) of a 2x2 contingency table.
    ''' </summary>
    ''' <param name="a">the upper left cell.</param>
    ''' <param name="b">the upper right cell.</param>
    ''' <param name="c">the lower left cell.</param>
    ''' <param name="d">the lower right cell.</param>
    ''' <returns>
    ''' the p-value; the Fisher exact test will be applied automatically when any of the
    ''' expected frequencies is less than 10.
    ''' </returns>
    <ExportAPI("kobas.chisq_test")>
    Public Function KobasChiSquareTest(a As Integer, b As Integer, c As Integer, d As Integer) As Double
        Return ExactTests.ImprovedChiSquare(a, b, c, d)
    End Function

    ''' <summary>
    ''' The ``as.data.frame`` handler of the GSEA result set.
    ''' </summary>
    ''' <param name="result">the GSEA result set.</param>
    ''' <param name="args">the additional argument list.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>a ``dataframe`` of the GSEA result set.</returns>
    <RGenericOverloads("as.data.frame")>
    Private Function gseaResultTable(result As GseaResult(), args As list, env As Environment) As dataframe
        Return GseaTable(result)
    End Function

    ''' <summary>
    ''' The ``as.data.frame`` handler of the over-representation analysis result set.
    ''' </summary>
    ''' <param name="result">the over-representation analysis result set.</param>
    ''' <param name="args">the additional argument list.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>a ``dataframe`` of the over-representation analysis result set.</returns>
    <RGenericOverloads("as.data.frame")>
    Private Function enrichmentRowTable(result As EnrichmentRow(), args As list, env As Environment) As dataframe
        Return EnrichmentRowsTable(result, has_adjust:=True)
    End Function

    ''' <summary>
    ''' The ``as.data.frame`` handler of the SNBEA result set.
    ''' </summary>
    ''' <param name="result">the SNBEA result set.</param>
    ''' <param name="args">the additional argument list.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>a ``dataframe`` of the SNBEA result set.</returns>
    <RGenericOverloads("as.data.frame")>
    Private Function snbeaResultTable(result As SnbeaResult(), args As list, env As Environment) As dataframe
        Return SnbeaTable(result)
    End Function

    ''' <summary>
    ''' Convert the GSEA result set into a R# ``dataframe``.
    ''' </summary>
    ''' <param name="result">the GSEA result set.</param>
    ''' <returns>
    ''' a ``dataframe`` with the gene set id as the row names and the columns: ``name``,
    ''' ``description``, ``size``, ``ES``, ``NES``, ``pvalue``, ``FDR``, ``es_index``,
    ''' ``leading_edge`` and ``hit_genes``.
    ''' </returns>
    <Extension>
    Private Function GseaTable(result As GseaResult()) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = result.Select(Function(item) item.ID).ToArray
        }

        Call table.add("name", result.Select(Function(item) item.Names))
        Call table.add("description", result.Select(Function(item) item.Description))
        Call table.add("size", result.Select(Function(item) item.Size))
        Call table.add("ES", result.Select(Function(item) item.ES))
        Call table.add("NES", result.Select(Function(item) item.NES))
        Call table.add("pvalue", result.Select(Function(item) item.PValue))
        Call table.add("FDR", result.Select(Function(item) item.FDR))
        Call table.add("es_index", result.Select(Function(item) item.ESIndex + 1))
        Call table.add("leading_edge", result.Select(Function(item) If(item.CoreGenes, New String() {}).JoinBy(";")))
        Call table.add("hit_genes", result.Select(Function(item) If(item.HitGenes, New String() {}).JoinBy(";")))

        Return table
    End Function

    ''' <summary>
    ''' Convert the over-representation analysis result set into a R# ``dataframe``.
    ''' </summary>
    ''' <param name="result">the over-representation analysis result set.</param>
    ''' <param name="has_adjust">does the result set contains the adjusted p-value?</param>
    ''' <returns>
    ''' a ``dataframe`` with the term id as the row names and the columns: ``name``,
    ''' ``database``, ``input_number``, ``background_number``, ``pvalue``,
    ''' ``p_adjust`` (only when <paramref name="has_adjust"/> is ``TRUE``) and ``input``.
    ''' </returns>
    <Extension>
    Private Function EnrichmentRowsTable(result As EnrichmentRow(), Optional has_adjust As Boolean = True) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = result.Select(Function(item) item.ID).ToArray
        }

        Call table.add("name", result.Select(Function(item) item.Term))
        Call table.add("database", result.Select(Function(item) item.Database))
        Call table.add("input_number", result.Select(Function(item) item.InputNumber))
        Call table.add("background_number", result.Select(Function(item) item.BackgroundNumber))
        Call table.add("pvalue", result.Select(Function(item) item.PValue))

        If has_adjust Then
            Call table.add("p_adjust", result.Select(Function(item) item.CorrectedPValue))
        End If

        Call table.add("input", result.Select(Function(item) If(item.Input, New String() {}).JoinBy(";")))

        Return table
    End Function

    ''' <summary>
    ''' Convert the SNBEA result set into a R# ``dataframe``.
    ''' </summary>
    ''' <param name="result">the SNBEA result set.</param>
    ''' <returns>
    ''' a ``dataframe`` with the gene set id as the row names and the columns: ``name``,
    ''' ``description``, ``size``, ``statistic``, ``pvalue``, ``p_adjust`` and ``genes``.
    ''' </returns>
    <Extension>
    Private Function SnbeaTable(result As SnbeaResult()) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = result.Select(Function(item) item.ID).ToArray
        }

        Call table.add("name", result.Select(Function(item) item.Names))
        Call table.add("description", result.Select(Function(item) item.Description))
        Call table.add("size", result.Select(Function(item) item.SetSize))
        Call table.add("statistic", result.Select(Function(item) item.Statistic))
        Call table.add("pvalue", result.Select(Function(item) item.PValue))
        Call table.add("p_adjust", result.Select(Function(item) item.AdjustedPValue))
        Call table.add("genes", result.Select(Function(item) If(item.Genes, New String() {}).JoinBy(";")))

        Return table
    End Function

    ''' <summary>
    ''' Run the PLAGE or the GAGE analysis for the given expression matrix.
    ''' </summary>
    ''' <param name="expr">the expression matrix.</param>
    ''' <param name="cls">the phenotype class labels.</param>
    ''' <param name="gmt">the gene set collection.</param>
    ''' <param name="min_size">the minimum number of the mapped gene set members.</param>
    ''' <param name="max_size">the maximum number of the mapped gene set members.</param>
    ''' <param name="fdr">the multiple testing correction method.</param>
    ''' <param name="data_type">the expression data type: ``rseq`` or ``ma``.</param>
    ''' <param name="normalize">performs the data normalization?</param>
    ''' <param name="plage">runs the PLAGE analysis when ``TRUE``, otherwise the GAGE analysis.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>a ``dataframe`` or an error message.</returns>
    Private Function RunSnbea(expr As Object,
                              cls As Object,
                              gmt As Object,
                              min_size As Integer,
                              max_size As Integer,
                              fdr As String,
                              data_type As String,
                              normalize As Boolean,
                              plage As Boolean,
                              env As Environment) As Object

        Dim pheno = AsPhenotype(cls, env)

        If TypeOf pheno Is Message Then
            Return pheno
        End If

        Dim expression = AsMatrix(expr, DirectCast(pheno, phenotype), env)

        If TypeOf expression Is Message Then
            Return expression
        End If

        Dim model As Background = AsBackground(gmt)

        If model Is Nothing Then
            Return RInternal.debug.stop("the given gene set collection could not be loaded, expect a gsea background model or a GMT file path!", env)
        End If

        Dim fdrMethod As FdrMethod

        Try
            fdrMethod = FdrAdjust.Parse(fdr)
        Catch ex As Exception
            Return RInternal.debug.stop(ex, env)
        End Try

        Dim dataType As ExpressionDataType = ExpressionDataType.RnaSeq

        If String.Equals(If(data_type, "").Trim(), "ma", StringComparison.OrdinalIgnoreCase) Then
            dataType = ExpressionDataType.Microarray
        End If

        Try
            Dim prepared As HTSMatrix = SnbeaData.Prepare(DirectCast(expression, HTSMatrix), dataType, normalize)
            Dim result As SnbeaResult()

            If plage Then
                result = PlageAnalysis.Compute(prepared, model, DirectCast(pheno, phenotype), min_size, max_size, fdrMethod)
            Else
                result = GageAnalysis.Compute(prepared, model, DirectCast(pheno, phenotype), min_size, max_size, fdrMethod)
            End If

            Return SnbeaTable(result)
        Catch ex As Exception
            Return RInternal.debug.stop(ex, env)
        End Try
    End Function

    ''' <summary>
    ''' Parse the phenotype class labels from the R# function argument.
    ''' </summary>
    ''' <param name="cls">the ``phenotype`` object, the CLS file path, the integer label vector or the class name vector.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>a <see cref="phenotype"/> object or an error message.</returns>
    Private Function AsPhenotype(cls As Object, env As Environment) As Object
        If cls Is Nothing Then
            Return RInternal.debug.stop("missing the phenotype class labels!", env)
        End If
        If TypeOf cls Is phenotype Then
            Return cls
        End If
        If TypeOf cls Is String Then
            Dim clsFile As String = DirectCast(cls, String)

            If Not File.Exists(clsFile) Then
                Return RInternal.debug.stop($"the given CLS file does not exists: {clsFile}", env)
            End If

            Try
                Return phenotype.FromClsFile(clsFile)
            Catch ex As Exception
                Return RInternal.debug.stop(ex, env)
            End Try
        End If

        Dim labels As Integer()

        Try
            labels = CLRVector.asInteger(cls)
        Catch ex As Exception
            Dim classes As String() = CLRVector.asCharacter(cls)

            If classes Is Nothing OrElse classes.Length = 0 Then
                Return RInternal.debug.stop("the phenotype class labels could not be empty!", env)
            End If

            Dim unique As String() = classes.Distinct().ToArray()

            If unique.Length <> 2 Then
                Return RInternal.debug.stop("Sorry, only two categorical phenotype labels are allowed.", env)
            End If

            labels = classes _
                .Select(Function(name) If(String.Equals(name, unique(0), StringComparison.Ordinal), 0, 1)) _
                .ToArray
        End Try

        If labels Is Nothing OrElse labels.Length = 0 Then
            Return RInternal.debug.stop("the phenotype class labels could not be empty!", env)
        End If

        Return PhenotypeFromLabels(labels)
    End Function

    ''' <summary>
    ''' Creates a two group phenotype object from the integer class label vector.
    ''' </summary>
    ''' <param name="labels">the class label vector, ``0`` for the first group and ``1`` for the second group.</param>
    ''' <returns>a <see cref="phenotype"/> object of the two phenotype groups.</returns>
    Private Function PhenotypeFromLabels(labels As Integer()) As phenotype
        Dim names As New Dictionary(Of String, String) From {
            {"C0", "C0"},
            {"C1", "C1"}
        }
        Dim classes As String() = labels _
            .Select(Function(label) If(label = 0, "C0", "C1")) _
            .ToArray
        Dim cls As New CLS With {
            .numOfSamples = labels.Length,
            .numOfClasses = 2,
            .classNameMaps = names,
            .sampleClass = classes
        }

        Return New phenotype(cls)
    End Function

    ''' <summary>
    ''' Load the expression matrix from the R# function argument.
    ''' </summary>
    ''' <param name="expr">the matrix object, the ``dataframe`` object or the expression matrix file path.</param>
    ''' <param name="pheno">the phenotype class labels, used for validating the sample order when reading from a file.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>a <see cref="Matrix"/> object or an error message.</returns>
    Private Function AsMatrix(expr As Object, pheno As phenotype, env As Environment) As Object
        If expr Is Nothing Then
            Return RInternal.debug.stop("missing the expression matrix data!", env)
        End If
        If TypeOf expr Is String Then
            Dim exprFile As String = DirectCast(expr, String)

            If Not File.Exists(exprFile) Then
                Return RInternal.debug.stop($"the given expression matrix file does not exists: {exprFile}", env)
            End If

            Try
                Return ExpressionTable.Load(exprFile, pheno)
            Catch ex As Exception
                Return RInternal.debug.stop(ex, env)
            End Try
        ElseIf TypeOf expr Is dataframe Then
            Dim df As dataframe = DirectCast(expr, dataframe)

            Return New HTSMatrix With {
                .tag = "R#",
                .sampleID = df.colnames,
                .expression = df.forEachRow _
                    .Select(Function(row)
                                Return New DataFrameRow With {
                                    .geneID = row.name,
                                    .experiments = CLRVector.asNumeric(row.ToArray)
                                }
                            End Function) _
                    .ToArray
            }
        ElseIf expr.GetType() Is GetType(SMRUCC.Rsharp.Runtime.Internal.Object.matrix) Then
            Return RMatrixToHTSMatrix(DirectCast(expr, SMRUCC.Rsharp.Runtime.Internal.Object.matrix), env)
        Else
            Return expr
        End If
    End Function

    ''' <summary>
    ''' Convert a R# ``matrix`` object into the GCModeller expression matrix object.
    ''' </summary>
    ''' <param name="rm">the R# matrix object.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>a <see cref="Matrix"/> object or an error message.</returns>
    ''' <remarks>
    ''' The ``mat`` field of the R# matrix object is a jagged array. The rows and the
    ''' columns are distinguished by the ``rownames`` and ``colnames``: when the length
    ''' of ``rownames`` equals the length of ``mat`` the array is treated as the row
    ''' major layout, otherwise when the length of ``colnames`` equals the length of
    ''' ``mat`` the array is treated as the column major layout and transposed.
    ''' </remarks>
    Private Function RMatrixToHTSMatrix(rm As SMRUCC.Rsharp.Runtime.Internal.Object.matrix, env As Environment) As Object
        Dim data As Array() = rm.mat

        If data Is Nothing OrElse data.Length = 0 Then
            Return RInternal.debug.stop("the given matrix object is empty!", env)
        End If

        Dim rowNames As String() = rm.rownames
        Dim colNames As String() = rm.colnames
        Dim rows As Double()()

        If rowNames IsNot Nothing AndAlso rowNames.Length = data.Length Then
            rows = data _
                .Select(Function(line) CLRVector.asNumeric(line)) _
                .ToArray
        ElseIf colNames IsNot Nothing AndAlso colNames.Length = data.Length Then
            Dim columns As Double()() = data _
                .Select(Function(line) CLRVector.asNumeric(line)) _
                .ToArray
            Dim geneCount As Integer = columns(0).Length

            rows = Enumerable.Range(0, geneCount) _
                .Select(Function(gene)
                            Return Enumerable.Range(0, columns.Length) _
                                .Select(Function(column) columns(column)(gene)) _
                                .ToArray
                        End Function) _
                .ToArray
        Else
            Return RInternal.debug.stop(
                "the given matrix object should has the row names (gene id) or the column names (sample id) for determine the data layout!",
                env)
        End If

        Dim sampleIDs As String()

        If colNames IsNot Nothing AndAlso colNames.Length = rows(0).Length Then
            sampleIDs = colNames
        Else
            sampleIDs = Enumerable.Range(1, rows(0).Length).Select(Function(i) $"S{i}").ToArray()
        End If

        Dim genes As String() = If(rowNames IsNot Nothing AndAlso rowNames.Length = rows.Length,
                                   rowNames,
                                   Enumerable.Range(1, rows.Length).Select(Function(i) $"G{i}").ToArray)

        Return New HTSMatrix With {
            .tag = "R#",
            .sampleID = sampleIDs,
            .expression = genes _
                .Select(Function(gene, i)
                            Return New DataFrameRow With {
                                .geneID = gene,
                                .experiments = rows(i)
                            }
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' Load the gene set collection from the R# function argument.
    ''' </summary>
    ''' <param name="gmt">a <see cref="Background"/> model or a GMT file path.</param>
    ''' <returns>the background model, or ``Nothing`` if the input is not supported.</returns>
    Private Function AsBackground(gmt As Object) As Background
        If gmt Is Nothing Then
            Return Nothing
        End If
        If TypeOf gmt Is Background Then
            Return DirectCast(gmt, Background)
        End If
        If TypeOf gmt Is String Then
            Dim gmtPath As String = DirectCast(gmt, String)

            If File.Exists(gmtPath) Then
                Return GmtFile.Load(gmtPath)
            End If
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Load the gene id list from the R# function argument.
    ''' </summary>
    ''' <param name="genes">a character vector of the gene ids or the KOBAS ``annotate`` output file path.</param>
    ''' <returns>the gene id vector (the ``query`` column will be used when a annotate file is given).</returns>
    Private Function AsGenes(genes As Object) As String()
        If genes Is Nothing Then
            Return New String() {}
        End If
        If TypeOf genes Is String Then
            Dim annotatePath As String = DirectCast(genes, String)

            If File.Exists(annotatePath) Then
                Return AnnotateFile.Load(annotatePath) _
                    .Entries _
                    .Select(Function(entry) entry.Query) _
                    .ToArray
            End If
        End If

        Return CLRVector.asCharacter(genes)
    End Function

    ''' <summary>
    ''' Parse the term similarity method name.
    ''' </summary>
    ''' <param name="method">the method name: ``kappa`` or ``jaccard``.</param>
    ''' <returns>
    ''' <see cref="SimilarityMethod.Jaccard"/> when the given method name is ``jaccard``,
    ''' otherwise <see cref="SimilarityMethod.Kappa"/> (the default method).
    ''' </returns>
    Private Function AsSimilarity(method As String) As SimilarityMethod
        If String.Equals(If(method, "").Trim(), "jaccard", StringComparison.OrdinalIgnoreCase) Then
            Return SimilarityMethod.Jaccard
        Else
            Return SimilarityMethod.Kappa
        End If
    End Function

    ''' <summary>
    ''' Evaluate the importance score of a term cluster.
    ''' </summary>
    ''' <param name="cluster">the term index collection of the cluster.</param>
    ''' <param name="pvalues">the p-values of the terms, could be ``Nothing``.</param>
    ''' <returns>the geometric mean of ``-log10(p)``, or ``NA`` when the p-values are missing.</returns>
    Private Function ClusterScore(cluster As Integer(), pvalues As Double()) As Double
        If pvalues Is Nothing OrElse pvalues.Length = 0 Then
            Return Double.NaN
        End If

        Dim valid As Integer() = cluster _
            .Where(Function(index) index >= 0 AndAlso index < pvalues.Length) _
            .ToArray

        If valid.Length <> cluster.Length Then
            Return Double.NaN
        End If

        Return TermClustering.Score(cluster, pvalues)
    End Function
End Module
