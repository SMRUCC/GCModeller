Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.BioSystem.Core
Imports SMRUCC.genomics.Model.Network.Regulons.ARACNE
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports PriorEdge = SMRUCC.genomics.BioSystem.Core.RegulatoryEdge
Imports std = System.Math

''' <summary>
''' ARACNE (Algorithm for the Reconstruction of Accurate Cellular Networks) gene
''' regulatory network inference.
''' </summary>
''' <remarks>
''' The pipeline is: ``mi -> threshold -> DPI -> direction``, and the transcription
''' factor (TF) prior knowledge is used in four places:
''' 
''' + the computation space is reduced from ``N x N`` to ``TF x N``;
''' + the ``TF -> target`` direction is assigned from the TF list;
''' + the curated ``TF -> target`` evidence relaxes the MI threshold and protects
'''   the supported edges from the top-K pruning;
''' + the curated evidence also protects the supported edges from the DPI filtering,
'''   and the protection strength is graded by the prior confidence.
''' 
''' The api groups:
''' 
''' + ``aracne``: run the whole inference from an expression matrix;
''' + ``aracne_options`` / ``infer``: build the options object, then run the inference
'''   in two steps (useful when the same options are reused for several matrices);
''' + ``edges`` / ``regulons`` / ``summary`` / ``threshold`` / ``dpi_stats`` /
'''   ``mi_matrix`` / ``as_graph`` / ``write_edges``: inspect and export the network.
''' </remarks>
<Package("ARACNE")>
<RTypeExport("aracne.options", GetType(AracneOptions))>
<RTypeExport("aracne_network", GetType(AracneNetwork))>
Module ARACNETools

    ''' <summary>
    ''' Create an <see cref="AracneOptions"/> object from the scalar arguments.
    ''' </summary>
    ''' <param name="estimator">
    ''' the mutual information estimator: ``copula`` (default, ``O(n)`` per gene pair),
    ''' ``adaptive`` (the adaptive partitioning of the ARACNE paper), ``histogram``
    ''' (equal frequency bins with the Miller-Madow correction) or ``kernel``
    ''' (gaussian kernel density, ``O(n^2)``).
    ''' </param>
    ''' <param name="threshold">
    ''' ``permutation`` (default, the quantile of the null distribution that is created
    ''' by shuffling the samples of each gene) or ``fixed`` (use <paramref name="mi_cutoff"/>).
    ''' </param>
    ''' <param name="mi_cutoff">
    ''' the fixed mutual information threshold; a positive value forces the ``fixed``
    ''' mode even when <paramref name="threshold"/> is ``permutation``.
    ''' </param>
    ''' <param name="quantile">the quantile of the permutation null distribution, default 0.95</param>
    ''' <param name="permutations">
    ''' the number of the gene pairs that are sampled for the null distribution,
    ''' a value that is less than or equals to 0 means "auto".
    ''' </param>
    ''' <param name="dpi">
    ''' ``auto`` (TF guided when a TF list is given, otherwise standard),
    ''' ``standard``, ``tf`` (TF guided) or ``off``.
    ''' </param>
    ''' <param name="epsilon">the DPI tolerance, default 0.1</param>
    ''' <param name="direction">``tf`` (default) or ``undirected``</param>
    ''' <param name="bins">the bin number of the ``histogram`` estimator, default 4</param>
    ''' <param name="max_neighbors">
    ''' the max number of the edges that each regulator can keep (top-K pruning),
    ''' a value that is less than or equals to 0 means "no pruning".
    ''' </param>
    ''' <param name="max_genes">
    ''' keep only the top variant genes; the genes of the TF list and of the prior
    ''' network are always kept.
    ''' </param>
    ''' <param name="min_nonzero">the minimal ratio of the non-zero samples for a gene to be kept</param>
    ''' <param name="log2">apply a ``log2(x + 1)`` transformation before the inference</param>
    ''' <param name="rank">use the ranks (spearman) instead of the values (pearson)</param>
    ''' <param name="standardize">z-score the expression vectors before the inference</param>
    ''' <param name="prior_min_confidence">
    ''' the eligibility threshold of the curated prior edges, default 0.45 (i.e. Low and above)
    ''' </param>
    ''' <param name="prior_factor">
    ''' the threshold relaxation factor at the full confidence, default 0.5;
    ''' the prior edge of the confidence ``c`` uses ``tau * (1 - (1 - factor) * c)``.
    ''' </param>
    ''' <param name="prior_protection">
    ''' ``graded`` (default: the DPI protection window is scaled by the confidence),
    ''' ``sampled`` (a prior edge is fully protected with the probability ``p = c``)
    ''' or ``off``.
    ''' </param>
    ''' <param name="prior_strength">the strength of the graded protection, default 0.5</param>
    ''' <param name="prior_tf_augment">
    ''' append the regulators of the prior network into the TF set, so that a incomplete
    ''' TF annotation does not lose the regulation edges.
    ''' </param>
    ''' <param name="threads">the max degree of parallelism, a value that is less than or equals to 0 means "auto"</param>
    ''' <param name="seed">the random seed of the permutation sampling and of the sampled protection</param>
    ''' <param name="silent">do not print the progress messages</param>
    ''' <returns>the options object that can be used by the ``infer`` api</returns>
    <ExportAPI("aracne_options")>
    <RApiReturn(GetType(AracneOptions))>
    Public Function createOptions(Optional estimator As String = "copula",
                                  Optional threshold As String = "permutation",
                                  Optional mi_cutoff As Double = 0,
                                  Optional quantile As Double = 0.95,
                                  Optional permutations As Integer = 0,
                                  Optional dpi As String = "auto",
                                  Optional epsilon As Double = 0.1,
                                  Optional direction As String = "tf",
                                  Optional bins As Integer = 4,
                                  Optional max_neighbors As Integer = 0,
                                  Optional max_genes As Integer = 0,
                                  Optional min_nonzero As Double = 0,
                                  Optional log2 As Boolean = False,
                                  Optional rank As Boolean = False,
                                  Optional standardize As Boolean = False,
                                  Optional prior_min_confidence As Double = 0.45,
                                  Optional prior_factor As Double = 0.5,
                                  Optional prior_protection As String = "graded",
                                  Optional prior_strength As Double = 0.5,
                                  Optional prior_tf_augment As Boolean = True,
                                  Optional threads As Integer = -1,
                                  Optional seed As Integer = 1234567,
                                  Optional silent As Boolean = True) As AracneOptions

        Dim options As New AracneOptions With {
            .miEstimator = ParseEstimator(estimator),
            .miThreshold = std.Max(0.0R, mi_cutoff),
            .permutationPairs = permutations,
            .permutationQuantile = std.Max(0.5R, std.Min(quantile, 0.999R)),
            .tolerance = std.Max(0.0R, std.Min(epsilon, 0.999R)),
            .dpiMode = ParseDpiMode(dpi),
            .directionMode = ParseDirection(direction),
            .histogramBins = std.Max(2, bins),
            .maxNeighbors = std.Max(0, max_neighbors),
            .maxGenes = std.Max(0, max_genes),
            .minNonZeroRatio = std.Max(0.0R, std.Min(min_nonzero, 1.0R)),
            .log2Transform = log2,
            .rankTransform = rank,
            .standardize = standardize,
            .priorMinConfidence = std.Max(0.0R, std.Min(prior_min_confidence, 1.0R)),
            .priorThresholdFactor = std.Max(0.0R, std.Min(prior_factor, 1.0R)),
            .priorProtectionMode = ParsePriorProtection(prior_protection),
            .priorProtectionStrength = std.Max(0.0R, std.Min(prior_strength, 1.0R)),
            .priorTfAugment = prior_tf_augment,
            .protectPriorEdges = ParsePriorProtection(prior_protection) <> PriorProtectionMode.Off,
            .maxDegreeOfParallelism = If(threads > 0, threads, -1),
            .randomSeed = seed,
            .silent = silent
        }

        options.thresholdMode = ParseThresholdMode(threshold, options.miThreshold)

        Return options
    End Function

    ''' <summary>
    ''' Infer the ARACNE gene regulatory network from the expression matrix.
    ''' </summary>
    ''' <param name="x">the gene expression matrix, rows are the genes and columns are the samples</param>
    ''' <param name="TF">
    ''' an optional gene id vector of the transcription factors. When it is given the
    ''' computation space is reduced from ``N x N`` to ``TF x N``, and the edges are
    ''' oriented from the TF to the targets; the prior network (the ``prior`` argument)
    ''' is additionally used for the threshold relaxation and the DPI protection.
    ''' </param>
    ''' <param name="estimator">see the ``aracne_options`` api; default ``copula``</param>
    ''' <param name="threshold">see the ``aracne_options`` api; default ``permutation``</param>
    ''' <param name="mi_cutoff">the fixed MI threshold, a positive value forces the ``fixed`` mode</param>
    ''' <param name="quantile">the quantile of the permutation null distribution, default 0.95</param>
    ''' <param name="permutations">the number of the gene pairs that are sampled for the null distribution</param>
    ''' <param name="dpi">``auto`` (default), ``standard``, ``tf`` or ``off``</param>
    ''' <param name="epsilon">the DPI tolerance, default 0.1</param>
    ''' <param name="direction">``tf`` (default) or ``undirected``</param>
    ''' <param name="bins">the bin number of the ``histogram`` estimator, default 4</param>
    ''' <param name="max_neighbors">the top-K pruning of each regulator, 0 = no pruning</param>
    ''' <param name="max_genes">keep only the top variant genes (the TF genes are always kept)</param>
    ''' <param name="min_nonzero">the minimal non-zero ratio of the retained genes</param>
    ''' <param name="log2">apply ``log2(x + 1)`` before the inference</param>
    ''' <param name="rank">spearman (rank) based copula instead of the pearson one</param>
    ''' <param name="standardize">z-score the expression vectors before the inference</param>
    ''' <param name="prior">
    ''' the curated ``TF -> target`` evidence: the value of the ``bnlearn::prior_network``
    ''' api (a <see cref="RegulatoryEdge"/> vector or pipeline) can be passed directly.
    ''' Only the edges whose confidence reaches <paramref name="prior_min_confidence"/>
    ''' are used, and the ``RegulationType`` (activator/inhibitor) is exported as the
    ''' ``regulation`` column of the edge table.
    ''' </param>
    ''' <param name="prior_min_confidence">the eligibility threshold of the prior edges, default 0.45</param>
    ''' <param name="prior_factor">the threshold relaxation factor at the full confidence, default 0.5</param>
    ''' <param name="prior_protection">``graded`` (default), ``sampled`` or ``off``</param>
    ''' <param name="prior_strength">the strength of the graded protection, default 0.5</param>
    ''' <param name="prior_tf_augment">append the prior regulators into the TF set</param>
    ''' <param name="threads">the max degree of parallelism, a value that is less than or equals to 0 means "auto"</param>
    ''' <param name="seed">the random seed, the result is reproducible for a fixed seed</param>
    ''' <param name="verbose">print the statistics, the top regulons and the top edges</param>
    ''' <param name="env">the R# runtime environment object</param>
    ''' <returns>
    ''' the fitted <see cref="AracneNetwork"/> object. Use the ``edges``/``regulons``/
    ''' ``summary``/``as_graph`` api to inspect it, and the ``write_edges`` api to export it.
    ''' </returns>
    ''' <example>
    ''' let net = ARACNE::aracne(hsa, TF = TF$Symbol, prior = prior, prior_protection = "graded");
    ''' let tab = ARACNE::edges(net);
    ''' write.csv(ARACNE::edges(net), file = "./aracne-edges.csv");
    ''' </example>
    <ExportAPI("aracne")>
    <RApiReturn(GetType(AracneNetwork))>
    Public Function aracneNetwork(x As Matrix,
                                  <RRawVectorArgument(TypeCodes.string)> Optional TF As Object = Nothing,
                                  Optional estimator As String = "copula",
                                  Optional threshold As String = "permutation",
                                  Optional mi_cutoff As Double = 0,
                                  Optional quantile As Double = 0.95,
                                  Optional permutations As Integer = 0,
                                  Optional dpi As String = "auto",
                                  Optional epsilon As Double = 0.1,
                                  Optional direction As String = "tf",
                                  Optional bins As Integer = 4,
                                  Optional max_neighbors As Integer = 0,
                                  Optional max_genes As Integer = 0,
                                  Optional min_nonzero As Double = 0,
                                  Optional log2 As Boolean = False,
                                  Optional rank As Boolean = False,
                                  Optional standardize As Boolean = False,
                                  <RRawVectorArgument(GetType(RegulatoryEdge))> Optional prior As Object = Nothing,
                                  Optional prior_min_confidence As Double = 0.45,
                                  Optional prior_factor As Double = 0.5,
                                  Optional prior_protection As String = "graded",
                                  Optional prior_strength As Double = 0.5,
                                  Optional prior_tf_augment As Boolean = True,
                                  Optional threads As Integer = -1,
                                  Optional seed As Integer = 1234567,
                                  Optional verbose As Boolean = True,
                                  Optional env As Environment = Nothing) As Object

        Dim options As AracneOptions = createOptions(
            estimator:=estimator,
            threshold:=threshold,
            mi_cutoff:=mi_cutoff,
            quantile:=quantile,
            permutations:=permutations,
            dpi:=dpi,
            epsilon:=epsilon,
            direction:=direction,
            bins:=bins,
            max_neighbors:=max_neighbors,
            max_genes:=max_genes,
            min_nonzero:=min_nonzero,
            log2:=log2,
            rank:=rank,
            standardize:=standardize,
            prior_min_confidence:=prior_min_confidence,
            prior_factor:=prior_factor,
            prior_protection:=prior_protection,
            prior_strength:=prior_strength,
            prior_tf_augment:=prior_tf_augment,
            threads:=threads,
            seed:=seed,
            silent:=Not verbose
        )

        If TF IsNot Nothing Then
            Dim tfs As String() = CLRVector _
                .asCharacter(TF) _
                .Where(Function(id) Not String.IsNullOrWhiteSpace(id)) _
                .Select(Function(id) id.Trim) _
                .ToArray

            If tfs.Length > 0 Then
                options.transcriptionFactors = tfs

                If verbose Then
                    Console.WriteLine($"     >> {tfs.Length} transcription factor(s) are used as the prior knowledge")
                End If
            End If
        End If

        If prior IsNot Nothing Then
            Dim parsed As Object = ToPriorEdges(prior, options.priorMinConfidence, env)

            If TypeOf parsed Is Message Then
                Return parsed
            End If

            Dim evidence As PriorEdge() = DirectCast(parsed, PriorEdge())

            If evidence.Length = 0 Then
                Console.WriteLine($"     >> [warn] no prior edge reaches the confidence {options.priorMinConfidence:G3}, the prior is ignored")
            Else
                options.priorNetwork = evidence

                If verbose Then
                    Console.WriteLine(
                        $"     >> prior knowledge: {evidence.Length} edge(s) accepted " &
                        $"(confidence >= {options.priorMinConfidence:G3}, " &
                        $"threshold x{options.priorThresholdFactor:G3}, protection = {options.priorProtectionMode} x {options.priorProtectionStrength:G3})")
                End If
            End If
        End If

        If verbose Then
            Console.WriteLine($"     >> {options.ToString}")
        End If

        Dim net As AracneNetwork = x.InferAracneNetwork(options)

        If verbose Then
            Call printSummary(net)
        End If

        Return net
    End Function

    ''' <summary>
    ''' Run the inference with a pre-built <see cref="AracneOptions"/> object.
    ''' </summary>
    ''' <param name="x">the gene expression matrix, rows are the genes and columns are the samples</param>
    ''' <param name="opts">the options object that is created via the ``aracne_options`` api</param>
    ''' <returns>the fitted <see cref="AracneNetwork"/> object</returns>
    ''' <example>
    ''' let opts = ARACNE::aracne_options(estimator = "adaptive", max_neighbors = 200);
    ''' let net = ARACNE::infer(hsa, opts);
    ''' </example>
    <ExportAPI("infer")>
    <RApiReturn(GetType(AracneNetwork))>
    Public Function Infer(x As Matrix, Optional opts As AracneOptions = Nothing) As AracneNetwork
        Return x.InferAracneNetwork(If(opts, New AracneOptions))
    End Function

    ''' <summary>
    ''' Get the regulation edges of the inferred network as a data frame.
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <returns>
    ''' a data frame with the columns: ``regulator``, ``target``, ``mi``, ``correlation``,
    ''' ``interaction``, ``is_tf_source``, ``is_tf_target``, ``prior`` and ``regulation``.
    ''' The rows are sorted by the mutual information in the descending order.
    ''' </returns>
    <ExportAPI("edges")>
    <RApiReturn(GetType(dataframe))>
    Public Function edges(net As AracneNetwork) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }
        Dim links As AracneEdge() = net.Regulations().ToArray

        Call table.add("regulator", links.Select(Function(e) e.regulator).ToArray)
        Call table.add("target", links.Select(Function(e) e.target).ToArray)
        Call table.add("mi", links.Select(Function(e) e.mi).ToArray)
        Call table.add("correlation", links.Select(Function(e) e.correlation).ToArray)
        Call table.add("interaction", links.Select(Function(e) e.interaction).ToArray)
        Call table.add("is_tf_source", links.Select(Function(e) e.isTfSource).ToArray)
        Call table.add("is_tf_target", links.Select(Function(e) e.isTfTarget).ToArray)
        Call table.add("prior", links.Select(Function(e) e.prior).ToArray)
        Call table.add("regulation", links.Select(Function(e) e.regulation).ToArray)

        Return table
    End Function

    ''' <summary>
    ''' Get the regulons (the target gene count of each regulator) of the network.
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <returns>
    ''' a data frame with the columns ``regulator``, ``size`` (the number of the targets)
    ''' and ``mean_mi`` (the mean mutual information of the regulon), sorted by the size
    ''' in the descending order.
    ''' </returns>
    <ExportAPI("regulons")>
    <RApiReturn(GetType(dataframe))>
    Public Function regulons(net As AracneNetwork) As dataframe
        Dim links As AracneEdge() = net.Edges
        Dim groups = links _
            .GroupBy(Function(e) e.regulator) _
            .OrderByDescending(Function(g) g.Count()) _
            .ToArray
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Call table.add("regulator", groups.Select(Function(g) g.Key).ToArray)
        Call table.add("size", groups.Select(Function(g) g.Count()).ToArray)
        Call table.add("mean_mi", groups.Select(Function(g) g.Average(Function(e) e.mi)).ToArray)
        Call table.add("prior", groups.Select(Function(g) g.Count(Function(e) e.prior)).ToArray)

        Return table
    End Function

    ''' <summary>
    ''' Get the gene ids of the network, in the row order of the internal expression vectors.
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <returns>a character vector of the gene ids</returns>
    <ExportAPI("gene_names")>
    <RApiReturn(GetType(String))>
    Public Function gene_names(net As AracneNetwork) As Object
        Return DirectCast(net.Genes.Clone(), String())
    End Function

    ''' <summary>
    ''' Get the transcription factors that are matched by the network (the regulators).
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <returns>a character vector of the TF gene ids</returns>
    <ExportAPI("tf_ids")>
    <RApiReturn(GetType(String))>
    Public Function tf_ids(net As AracneNetwork) As Object
        Return DirectCast(net.TFs.Clone(), String())
    End Function

    ''' <summary>
    ''' Get the mutual information matrix of the inferred network.
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <returns>
    ''' a matrix whose rows are the regulators and columns are the targets, an element is
    ''' the mutual information (bits) of the corresponding gene pair.
    ''' </returns>
    <ExportAPI("mi_matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function mi_matrix(net As AracneNetwork) As Matrix
        Return net.CoefMatrix
    End Function

    ''' <summary>
    ''' Get the mutual information matrix of the inferred network (the alias of ``mi_matrix``).
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <returns></returns>
    <ExportAPI("coef")>
    <RApiReturn(GetType(Matrix))>
    Public Function coef(net As AracneNetwork) As Matrix
        Return net.CoefMatrix
    End Function

    ''' <summary>
    ''' Get the inference statistics of the network.
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <returns>
    ''' a single row data frame with the columns: ``genes_scanned``, ``genes``, ``samples``,
    ''' ``tf``, ``pairs``, ``threshold``, ``edges_threshold``, ``edges_dpi``, ``edges``,
    ''' ``prior_edges``, ``prior_supported``, ``prior_rescued``, ``seconds``.
    ''' </returns>
    <ExportAPI("summary")>
    <RApiReturn(GetType(dataframe))>
    Public Function summary(net As AracneNetwork) As dataframe
        Dim stat As AracneStatistics = net.Statistics
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If stat Is Nothing Then
            Return table
        End If

        Call table.add("genes_scanned", stat.genesScanned)
        Call table.add("genes", stat.genes)
        Call table.add("samples", stat.samples)
        Call table.add("tf", stat.transcriptionFactors)
        Call table.add("pairs", stat.candidatePairs)
        Call table.add("threshold", If(net.Threshold Is Nothing, 0.0R, net.Threshold.value))
        Call table.add("edges_threshold", stat.edgesAfterThreshold)
        Call table.add("edges_dpi", stat.edgesAfterDpi)
        Call table.add("edges", stat.edges)
        Call table.add("prior_edges", stat.priorEdges)
        Call table.add("prior_supported", stat.priorSupportedPairs)
        Call table.add("prior_rescued", stat.priorProtectedEdges)
        Call table.add("seconds", stat.totalSeconds)

        Return table
    End Function

    ''' <summary>
    ''' Get the significance threshold (``tau``) information of the network.
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <returns>
    ''' a single row data frame with the columns: ``mode``, ``value``, ``quantile``,
    ''' ``null_mean``, ``null_sd``, ``null_max``, ``samples``, ``replicates`` and ``signal_ratio``.
    ''' </returns>
    <ExportAPI("threshold")>
    <RApiReturn(GetType(dataframe))>
    Public Function threshold(net As AracneNetwork) As dataframe
        Dim tau As MiThreshold = net.Threshold
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If tau Is Nothing Then
            Return table
        End If

        Call table.add("mode", tau.mode.ToString)
        Call table.add("value", tau.value)
        Call table.add("quantile", tau.quantile)
        Call table.add("null_mean", tau.nullMean)
        Call table.add("null_sd", tau.nullSD)
        Call table.add("null_max", tau.nullMax)
        Call table.add("samples", tau.samples)
        Call table.add("replicates", tau.replicates)
        Call table.add("signal_ratio", tau.SignalRatio)

        Return table
    End Function

    ''' <summary>
    ''' Get the DPI (data processing inequality) filtering statistics of the network.
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <returns>
    ''' a single row data frame with the columns: ``edges_before``, ``edges_after``,
    ''' ``removed``, ``rescued`` (the prior protected edges), ``triplets`` and ``passes``;
    ''' the ``rescued_high``, ``rescued_medium`` and ``rescued_low`` columns report the
    ''' rescued edges by the prior confidence grade.
    ''' </returns>
    <ExportAPI("dpi_stats")>
    <RApiReturn(GetType(dataframe))>
    Public Function dpi_stats(net As AracneNetwork) As dataframe
        Dim stat As AracneStatistics = net.Statistics
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If stat Is Nothing OrElse stat.dpi Is Nothing Then
            Return table
        End If

        Call table.add("edges_before", stat.dpi.EdgesBefore)
        Call table.add("edges_after", stat.dpi.EdgesAfter)
        Call table.add("removed", stat.dpi.RemovedEdges.Count)
        Call table.add("rescued", stat.dpi.ProtectedEdges.Count)
        Call table.add("triplets", stat.dpi.TrianglesExamined)
        Call table.add("passes", stat.dpi.Passes)
        Call table.add("rescued_high", GradeOf(stat.priorRescuedByConfidence, "high"))
        Call table.add("rescued_medium", GradeOf(stat.priorRescuedByConfidence, "medium"))
        Call table.add("rescued_low", GradeOf(stat.priorRescuedByConfidence, "low"))

        Return table
    End Function

    ''' <summary>
    ''' Convert the inferred network into a graph model, so that it can be exported as a
    ''' network file or be visualized by the ``igraph`` package.
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <returns>
    ''' a <see cref="NetworkGraph"/> object: the nodes are the genes of the network, the
    ''' edges carry the ``interaction_type`` (``directed``/``mutual``/``undirected``),
    ''' the ``prior`` flag and the ``regulation`` type (``activator``/``inhibitor``) metadata.
    ''' </returns>
    <ExportAPI("as_graph")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function asGraph(net As AracneNetwork) As NetworkGraph
        Dim g As New NetworkGraph()

        For Each id As String In net.Genes
            Call g.CreateNode(id)
        Next

        For Each e As AracneEdge In net.Regulations()
            Dim data As New EdgeData()

            data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE) = e.interaction
            data("prior") = If(e.prior, "1", "0")
            data("mi") = e.mi.ToString("G6", Globalization.CultureInfo.InvariantCulture)

            If Not String.IsNullOrEmpty(e.regulation) Then
                data("regulation") = e.regulation
            End If

            Call g.CreateEdge(e.regulator, e.target, e.mi, data)
        Next

        Return g
    End Function

    ''' <summary>
    ''' Export the regulation edges of the network into a csv file.
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <param name="file">the output csv file, its folder will be created automatically</param>
    <ExportAPI("write_edges")>
    Public Sub write_edges(net As AracneNetwork, file As String)
        Call net.WriteEdges(file)
    End Sub

    ''' <summary>
    ''' Print the statistics, the top regulons and the top edges of the network.
    ''' </summary>
    ''' <param name="net">the network that is created via the ``aracne`` api</param>
    ''' <param name="top">the number of the rows that are printed, default 8</param>
    Private Sub printSummary(net As AracneNetwork, Optional top As Integer = 8)
        Dim stat As AracneStatistics = net.Statistics

        If stat Is Nothing Then
            Return
        End If

        Call Console.WriteLine($"     >> {stat.ToString}")

        If net.Threshold IsNot Nothing Then
            Call Console.WriteLine($"     >> {net.Threshold.ToString}")
        End If

        If stat.priorEdges > 0 Then
            Call Console.WriteLine($"     >> prior: {stat.priorEdges} pair(s) in the gene space, " &
                                   $"{stat.priorSupportedPairs} supported, {stat.priorProtectedEdges} rescued from the DPI " &
                                   $"({String.Join(", ", stat.priorRescuedByConfidence.Select(Function(kv) $"{kv.Key}={kv.Value}"))})")
        End If

        Call Console.WriteLine("     >> top regulons (by the number of target genes):")

        For Each item In net.Edges.RegulonSizes().Take(top)
            Call Console.WriteLine($"        {item.regulator} ({item.size} targets)")
        Next

        Call Console.WriteLine("     >> top regulation edges (by MI):")

        For Each edge As AracneEdge In net.Edges.TopEdges(top)
            Call Console.WriteLine($"        {edge.ToString}")
        Next
    End Sub

    ''' <summary>
    ''' Read one value of the confidence grade statistics.
    ''' </summary>
    ''' <param name="counts">the grade statistics table</param>
    ''' <param name="grade">``high``/``medium``/``low``</param>
    ''' <returns></returns>
    Private Function GradeOf(counts As Dictionary(Of String, Integer), grade As String) As Integer
        Dim value As Integer

        If counts IsNot Nothing AndAlso counts.TryGetValue(grade, value) Then
            Return value
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' Convert the prior network that is created by the ``bnlearn::prior_network`` api
    ''' into the lightweight <see cref="PriorEdge"/> structure of the ARACNE inference.
    ''' </summary>
    ''' <param name="prior">
    ''' a <see cref="RegulatoryEdge"/> vector (or pipeline), or a <see cref="PriorEdge"/> vector
    ''' </param>
    ''' <param name="minConfidence">the eligibility threshold of the prior confidence</param>
    ''' <param name="env">the R# runtime environment object</param>
    ''' <returns>
    ''' a <see cref="PriorEdge"/> vector, or a R# error message object when the given
    ''' prior object can not be cast to a prior network.
    ''' </returns>
    Private Function ToPriorEdges(prior As Object, minConfidence As Double, env As Environment) As Object
        If TypeOf prior Is PriorEdge() Then
            Return DirectCast(prior, PriorEdge()) _
                .Where(Function(e) e.Confidence >= minConfidence) _
                .ToArray
        End If

        Dim net As PipeIterator(Of RegulatoryEdge) = pipeline.Stream(Of RegulatoryEdge)(prior, env, nullPipe:=True)

        If net IsNot Nothing AndAlso net.isError Then
            Return net.getError
        End If

        Dim accepted As New List(Of PriorEdge)

        If net IsNot Nothing Then
            ' 注意：这里要用 For Each 消费 pipe，PipeIterator.getData 对类数组不返回元素
            For Each item As RegulatoryEdge In net
                If item Is Nothing Then
                    Continue For
                End If
                If String.IsNullOrWhiteSpace(item.TF) OrElse String.IsNullOrWhiteSpace(item.TargetGene) Then
                    Continue For
                End If
                If item.Confidence < minConfidence Then
                    Continue For
                End If

                ' 因果符号：激活 +1、抑制 -1、未知 0，它会被写进输出边的 regulation 字段
                Call accepted.Add(New PriorEdge(item.TF.Trim, item.TargetGene.Trim, item.Confidence, item.Sign))
            Next
        End If

        Return accepted.ToArray
    End Function

    ''' <summary>
    ''' Parse the mutual information estimator name.
    ''' </summary>
    ''' <param name="value">
    ''' ``copula``/``gaussian``, ``adaptive``/``partition``, ``histogram``/``bins``,
    ''' ``kernel``/``kde``; the unknown names fall back to the ``copula`` estimator.
    ''' </param>
    ''' <returns></returns>
    Private Function ParseEstimator(value As String) As MiEstimator
        Select Case If(value, "copula").Trim.ToLowerInvariant()
            Case "adaptive", "partition", "adaptive_partitioning", "adaptive-partitioning"
                Return MiEstimator.AdaptivePartitioning
            Case "histogram", "bins", "quantile", "equal_frequency", "discretize", "discrete"
                Return MiEstimator.EqualFrequencyHistogram
            Case "kernel", "kde", "density", "kerneldensity", "kernel_density"
                Return MiEstimator.KernelDensity
            Case Else
                Return MiEstimator.GaussianCopula
        End Select
    End Function

    ''' <summary>
    ''' Parse the threshold mode: a positive <paramref name="cutoff"/> always means the
    ''' ``fixed`` mode.
    ''' </summary>
    ''' <param name="value">``permutation`` or ``fixed``</param>
    ''' <param name="cutoff">the fixed MI threshold</param>
    ''' <returns></returns>
    Private Function ParseThresholdMode(value As String, cutoff As Double) As MiThresholdMode
        If cutoff > 0 Then
            Return MiThresholdMode.Fixed
        ElseIf String.Equals(If(value, "permutation").Trim(), "fixed", StringComparison.OrdinalIgnoreCase) Then
            Return MiThresholdMode.Fixed
        Else
            Return MiThresholdMode.Permutation
        End If
    End Function

    ''' <summary>
    ''' Parse the DPI mode; ``auto`` returns <c>Nothing</c> so that the inference selects
    ''' the TF guided mode when a TF list is given.
    ''' </summary>
    ''' <param name="value">``auto``, ``standard``, ``tf`` or ``off``</param>
    ''' <returns></returns>
    Private Function ParseDpiMode(value As String) As DpiMode?
        Select Case If(value, "auto").Trim.ToLowerInvariant()
            Case "off", "none", "no", "false", "0"
                Return DpiMode.Off
            Case "standard", "std", "plain"
                Return DpiMode.Standard
            Case "tf", "tf_guided", "tf-guided", "guided"
                Return DpiMode.TfGuided
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Parse the direction mode.
    ''' </summary>
    ''' <param name="value">``tf``/``directed`` or ``undirected``</param>
    ''' <returns></returns>
    Private Function ParseDirection(value As String) As DirectionMode
        Select Case If(value, "tf").Trim.ToLowerInvariant()
            Case "undirected", "none", "off", "false", "0"
                Return DirectionMode.Undirected
            Case Else
                Return DirectionMode.TfDirected
        End Select
    End Function

    ''' <summary>
    ''' Parse the prior protection mode.
    ''' </summary>
    ''' <param name="value">``graded`` (default), ``sampled`` or ``off``</param>
    ''' <returns></returns>
    Private Function ParsePriorProtection(value As String) As PriorProtectionMode
        Select Case If(value, "graded").Trim.ToLowerInvariant()
            Case "off", "none", "false", "no", "0"
                Return PriorProtectionMode.Off
            Case "sampled", "sample", "random", "probability", "probabilistic"
                Return PriorProtectionMode.Sampled
            Case Else
                Return PriorProtectionMode.Graded
        End Select
    End Function
End Module
