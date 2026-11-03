Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.GeneTree
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Intervention
Imports SMRUCC.genomics.Model.Network.Regulons.ARACNE
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports std = System.Math
Imports RDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' XGBoost based gene regulatory network simulator and the in silico perturbation toolkit
''' </summary>
''' 
''' <remarks>
''' <para>
''' This package module is the R# front end of the ``GeneTree`` model: the ARACNE
''' inferred mutual information network is turned into a non linear dynamical system
''' by fitting one gradient boosting tree regressor per target gene
''' (<c>x_i = f_i(X_Reg(i))</c>), and the virtual perturbation experiment is then
''' performed by solving the fixed point of the fitted system with the damped
''' iteration <c>x_i^new = (1 - alpha) * x_i^old + alpha * f_i(X_Reg(i)^old)</c>.
''' </para>
''' 
''' <para>
''' The typical workflow:
''' </para>
''' 
''' <code>
''' imports ["geneTree", "bnlearn"] from "biosystem";
''' 
''' let opts = geneTree::make_profile("fast", population = 100);
''' let grn = geneTree::new(aracne, expr, opts) |> fit_tree();
''' 
''' # 相对训练时的野生型基线做虚拟敲除
''' let ko = grn |> knockouts(c("TP53", "MYC"));
''' 
''' # 切换到指定的表达状态上下文之后再扰动（fix = TRUE 时上下文基因被钳制）
''' let state = grn |> geneTree::steady_state();
''' let perturbed = grn
'''     |> geneTree::set_context(c("TP53"), c(0.0), fix = TRUE)
'''     |> knockouts(c("EGFR"))
''' ;
''' </code>
''' 
''' <para>
''' The fitted <see cref="XGBoostGRN"/> object implements the
''' <see cref="SMRUCC.genomics.BioSystem.InsilicoPerturbationExperiment"/> interface,
''' so the generic ``knockouts`` / ``overexpress`` / ``knockdown`` / ``make_exports``
''' api of the ``bnlearn`` package module can be used directly on it.
''' </para>
''' </remarks>
<Package("geneTree")>
<RTypeExport("xgboost_grn", GetType(XGBoostGRN))>
<RTypeExport("geneTree_opts", GetType(GeneTreeConfig))>
<RTypeExport("geneTree_report", GetType(TrainingReport))>
<RTypeExport("geneTree_graph", GetType(GeneRegulationGraph))>
Module geneTreeTools

    ''' <summary>
    ''' Register the custom serialization handlers of the geneTree package.
    ''' </summary>
    Sub Main()
        Call RInternal.generic.add("writeBin", GetType(XGBoostGRN), AddressOf SaveModelZip)
        Call RInternal.generic.add("readBin.xgboost_grn", GetType(Stream), AddressOf LoadModelZip)
    End Sub

    Private Function SaveModelZip(model As XGBoostGRN, args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call model.Save(con)
        Call con.Flush()
        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="s"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' <see cref="XGBoostGRN"/>
    ''' </returns>
    Private Function LoadModelZip(s As Stream, args As list, env As Environment) As Object
        Return XGBoostGRN.Load(s)
    End Function

    ''' <summary>
    ''' Create the training/solving configuration profile of the GeneTree model.
    ''' </summary>
    ''' <param name="profile">
    ''' the performance profile: ``fast`` (at most 1000 cells per model, top 10
    ''' regulators, 40 boosting rounds), ``balanced`` (3000 cells, top 20 regulators,
    ''' 100 rounds) or ``full`` (all of the cells and all of the regulators).
    ''' </param>
    ''' <param name="population">
    ''' the number of cells that are used as the independent initial states of the
    ''' damped fixed point iteration, more cells give a more reliable population
    ''' level statistic but cost more time.
    ''' </param>
    ''' <param name="iterations">the maximum iteration rounds of the fixed point solving.</param>
    ''' <param name="cells">
    ''' the maximum number of cells that is used by each gene model, ``0`` keeps the
    ''' value of the selected profile.
    ''' </param>
    ''' <param name="max_genes">
    ''' only the first n target genes(ordered by the regulator count in descending
    ''' order) are trained, ``0`` means all of the target genes.
    ''' </param>
    ''' <param name="max_regulators">
    ''' the number of the regulators that is kept for each target gene(the top n
    ''' regulator ordered by the mutual information in descending order), a negative
    ''' value keeps the value of the selected profile, ``0`` keeps all of them.
    ''' </param>
    ''' <returns>the <see cref="GeneTreeConfig"/> configuration object.</returns>
    <ExportAPI("make_profile")>
    <RApiReturn(GetType(GeneTreeConfig))>
    Public Function make_profile(Optional profile As GeneTreeProfile = GeneTreeProfile.Fast,
                                 Optional population As Integer = 100,
                                 Optional iterations As Integer = 60,
                                 Optional cells As Integer = 0,
                                 Optional max_genes As Integer = 0,
                                 Optional max_regulators As Integer = -1,
                                 Optional include_undirected As Boolean = True,
                                 Optional threads As Integer = -1,
                                 Optional threads_per_model As Integer = -1) As GeneTreeConfig

        Dim config As GeneTreeConfig = GeneTreeConfig.FromProfile(profile)

        config.PopulationSize = population
        config.MaxIterations = iterations
        config.Verbose = True
        config.IncludeUndirectedEdges = include_undirected

        ' 外层（按基因）并行度；<=0 表示自动使用全部可用核心
        If threads > 0 Then
            config.MaxDegreeOfParallelism = threads
        End If

        config.ThreadsPerModel = threads_per_model

        If cells > 0 Then
            config.MaxSamplesPerModel = cells
        End If
        If max_genes > 0 Then
            config.MaxGenes = max_genes
        End If
        If max_regulators >= 0 Then
            config.MaxRegulators = max_regulators
        End If

        Call config.ValidateOrThrow()

        Return config
    End Function

    ''' <summary>
    ''' Override the parallelism of a trained model (the perturbation solver) or of a
    ''' configuration object (the model training).
    ''' </summary>
    ''' <param name="tree">the GeneTree model or the configuration object.</param>
    ''' <param name="threads">
    ''' the outer (per gene) degree of parallelism; ``&lt;= 0`` restores the automatic mode
    ''' which uses all of the available processors.
    ''' </param>
    ''' <param name="threads_per_model">
    ''' the TGBoost thread number of a single gene model: ``-1`` automatic (1 when the genes
    ''' are trained in parallel), ``0`` sequential, ``&gt;0`` explicit. It only affects the
    ''' model training, it is ignored for the perturbation solving.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>the input object with the new parallelism settings.</returns>
    ''' <remarks>
    ''' the training is parallel over the target genes, therefore the per model threads are
    ''' resolved to 1 automatically: nested parallelism (outer over genes plus inner tree
    ''' building) starves the .NET thread pool and the CPU utilization collapses to a few
    ''' cores even on a 128 threads server.
    ''' </remarks>
    <ExportAPI("parallel_opts")>
    Public Function parallel_opts(tree As Object,
                                  Optional threads As Integer = -1,
                                  Optional threads_per_model As Integer = -1,
                                  Optional env As Environment = Nothing) As Object

        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model or configuration is nothing!", env)
        End If

        Dim config As GeneTreeConfig = Nothing

        If TypeOf tree Is GeneTreeConfig Then
            config = DirectCast(tree, GeneTreeConfig)
        ElseIf TypeOf tree Is XGBoostGRN Then
            config = DirectCast(tree, XGBoostGRN).Config
        Else
            Return RInternal.debug.stop($"unsupported object type '{tree.GetType.FullName}' for the parallel_opts api!", env)
        End If

        config.MaxDegreeOfParallelism = If(threads > 0, threads, -1)
        config.ThreadsPerModel = threads_per_model

        If TypeOf tree Is XGBoostGRN Then
            Return DirectCast(tree, XGBoostGRN).ResetSolver()
        Else
            Return config
        End If
    End Function

    ''' <summary>
    ''' Enable the residual two-stage structure discovery on a GeneTree configuration.
    ''' </summary>
    ''' <param name="config">
    ''' the configuration object created by the ``make_profile`` api (it is modified in
    ''' place and returned, so it can be chained).
    ''' </param>
    ''' <param name="candidates">
    ''' a character vector of the candidate regulator gene id; the discovery only searches
    ''' inside this set, it never guesses the candidate space by itself.
    ''' 
    ''' to make a non-transcription-factor perturbation target able to propagate to its
    ''' downstream genes, the target gene itself must be included in this candidate set.
    ''' </param>
    ''' <param name="max_per_gene">
    ''' the maximum number of the new edges that could be accepted for each target gene,
    ''' default is 2.
    ''' </param>
    ''' <param name="max_candidates">
    ''' the maximum number of the candidates that are tried for each target gene (each try
    ''' costs one more model fitting), default is 5.
    ''' </param>
    ''' <param name="min_correlation">
    ''' the minimal |pearson r| between the target gene residual and the candidate gene
    ''' expression, default is 0.1.
    ''' </param>
    ''' <param name="min_delta_r2">
    ''' the minimal validation R² gain that is required for accepting one new edge,
    ''' default is 0.002.
    ''' </param>
    ''' <param name="sample_size">
    ''' the number of the cells that are used by the residual correlation screening,
    ''' default is 1000, ``0`` uses all of the training cells.
    ''' </param>
    ''' <param name="max_genes">
    ''' only the first n target genes run the discovery, ``0`` means all of them (useful for
    ''' a quick trial).
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the input configuration object with ``EdgeDiscovery = ResidualTwoStage``; an R#
    ''' error message object is returned when the candidate set is empty or the resulting
    ''' configuration is invalid.
    ''' </returns>
    ''' <remarks>
    ''' <para>
    ''' The discovery runs in two stages:
    ''' </para>
    ''' <list type="number">
    ''' <item><description>
    ''' only the ARACNE edges are used to fit the target gene model, and the residual
    ''' (label - prediction) is computed;
    ''' </description></item>
    ''' <item><description>
    ''' the candidates are ranked by the correlation with the residual, then they are added
    ''' into the feature set one by one; a candidate is accepted as a new regulation edge
    ''' only when the validation R² gain is above ``min_delta_r2`` and the feature is
    ''' actually used by the trees.
    ''' </description></item>
    ''' </list>
    ''' <para>
    ''' the ARACNE edges are always kept, the accepted edges are appended after them, so the
    ''' topology of a discovery model is still comparable with the previous models.
    ''' </para>
    ''' </remarks>
    ''' <example>
    ''' imports ["geneTree"] from "biosystem";
    ''' 
    ''' let tfs = read.csv("./tfs.csv");
    ''' let opts = geneTree::make_profile("fast", population = 100, max_regulators = 10)
    '''          |> geneTree::discovery(tfs$gene_id, max_per_gene = 2);
    ''' let grn = geneTree::new(aracne, expr, opts) |> fit_tree();
    ''' geneTree::discovered_edges(grn, top_n = 100) |> print();
    ''' </example>
    <ExportAPI("discovery")>
    <RApiReturn(GetType(GeneTreeConfig))>
    Public Function discovery(config As GeneTreeConfig,
                              <RRawVectorArgument(TypeCodes.string)> candidates As Object,
                              Optional max_per_gene As Integer = 2,
                              Optional max_candidates As Integer = 5,
                              Optional min_correlation As Double = 0.1,
                              Optional min_delta_r2 As Double = 0.002,
                              Optional sample_size As Integer = 1000,
                              Optional max_genes As Integer = 0,
                              Optional env As Environment = Nothing) As Object

        If config Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree configuration is nothing!", env)
        End If

        Dim geneIds As String() = CLRVector.asCharacter(candidates)

        If geneIds.IsNullOrEmpty Then
            Return RInternal.debug.stop("the candidate gene list is empty, at least one gene id is required for the edge discovery!", env)
        End If

        config.EdgeDiscovery = EdgeDiscoveryMode.ResidualTwoStage
        Call config.SetDiscoveryCandidates(geneIds)
        config.DiscoveryMaxPerGene = std.Max(1, max_per_gene)
        config.DiscoveryMaxCandidates = std.Max(config.DiscoveryMaxPerGene, max_candidates)
        config.DiscoveryMinCorrelation = min_correlation
        config.DiscoveryMinDeltaR2 = min_delta_r2
        config.DiscoverySampleSize = sample_size
        config.DiscoveryMaxGenes = std.Max(0, max_genes)

        Try
            Call config.ValidateOrThrow()
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return config
    End Function

    ''' <summary>
    ''' Build the regulation graph model from the ARACNE inferred network.
    ''' </summary>
    ''' <param name="aracne">
    ''' the ARACNE network object, which could be created by the ``aracne`` package
    ''' module or loaded from the model zip archive via ``readBin``.
    ''' </param>
    ''' <param name="config">
    ''' the configuration object created by the ``make_profile`` api; the
    ''' ``max_regulators`` and ``include_undirected_edges`` options determine the
    ''' structure of the graph.
    ''' </param>
    ''' <returns>the <see cref="GeneRegulationGraph"/> object: target gene -&gt; regulators.</returns>
    <ExportAPI("load_topology")>
    <RApiReturn(GetType(GeneRegulationGraph))>
    Public Function load_topology(aracne As AracneNetwork,
                                  Optional config As GeneTreeConfig = Nothing,
                                  Optional env As Environment = Nothing) As Object

        If aracne Is Nothing Then
            Return RInternal.debug.stop("the given ARACNE network model is nothing!", env)
        End If

        Try
            Return GeneRegulationGraph.FromAracne(aracne, config)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Attach the gene expression matrix to the regulation graph, the created model is
    ''' not trained yet.
    ''' </summary>
    ''' <param name="graph">
    ''' the regulation graph object that is created by the ``load_topology`` api.
    ''' </param>
    ''' <param name="expr">
    ''' the gene expression matrix object(rows are genes, columns are samples), which
    ''' could be loaded from a csv table file via the ``geneExpression::load.expr`` api.
    ''' the numeric values of the model training and the perturbation are all taken from
    ''' this matrix(the ARACNE model archive stores the rank transformed vectors instead
    ''' of the raw expression values).
    ''' </param>
    ''' <param name="config">the configuration object created by the ``make_profile`` api.</param>
    ''' <returns>the untrained <see cref="XGBoostGRN"/> model object.</returns>
    <ExportAPI("load_expression")>
    <RApiReturn(GetType(XGBoostGRN))>
    Public Function load_expression(graph As GeneRegulationGraph,
                                    expr As HTSMatrix,
                                    Optional config As GeneTreeConfig = Nothing,
                                    Optional env As Environment = Nothing) As Object

        If graph Is Nothing Then
            Return RInternal.debug.stop("the given regulation graph object is nothing!", env)
        End If
        If expr Is Nothing Then
            Return RInternal.debug.stop("the given gene expression matrix is nothing!", env)
        End If

        Try
            Return XGBoostGRN.Open(graph, expr, config)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Create the GeneTree model from the ARACNE network and the gene expression matrix.
    ''' </summary>
    ''' <param name="aracne">the ARACNE inferred network object.</param>
    ''' <param name="expr">the gene expression matrix object(rows are genes, columns are samples).</param>
    ''' <param name="config">the configuration object created by the ``make_profile`` api.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the untrained <see cref="XGBoostGRN"/> model object; an R# error message object
    ''' is returned when the network or the expression matrix is invalid.
    ''' </returns>
    ''' <remarks>
    ''' this function is the combination of the ``load_topology`` and the
    ''' ``load_expression`` api, the model should be trained by the ``fit_tree`` api
    ''' before any virtual perturbation experiment.
    ''' </remarks>
    <ExportAPI("new")>
    <RApiReturn(GetType(XGBoostGRN))>
    Public Function create_tree(aracne As AracneNetwork,
                                expr As HTSMatrix,
                                Optional config As GeneTreeConfig = Nothing,
                                Optional env As Environment = Nothing) As Object

        If aracne Is Nothing Then
            Return RInternal.debug.stop("the given ARACNE network model is nothing!", env)
        End If
        If expr Is Nothing Then
            Return RInternal.debug.stop("the given gene expression matrix is nothing!", env)
        End If

        Try
            Return XGBoostGRN.FromAracne(aracne, expr, config)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Train the gradient boosting tree models of each target gene.
    ''' </summary>
    ''' <param name="tree">the untrained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the <see cref="TrainingReport"/> object: the R²/RMSE of each gene model, the
    ''' comparison against the mean baseline and the split importance of each
    ''' regulator; an R# error message object is returned when the training fails.
    ''' </returns>
    ''' <remarks>
    ''' the training is parallel over the target genes, the training progress is
    ''' written to the console when the ``verbose`` option of the configuration is on.
    ''' </remarks>
    <ExportAPI("fit_tree")>
    <RApiReturn(GetType(TrainingReport))>
    Public Function fit_tree(tree As XGBoostGRN, Optional env As Environment = Nothing) As Object
        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Try
            Return tree.Fit()
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Export the training report as two readable table files.
    ''' </summary>
    ''' <param name="report">the <see cref="TrainingReport"/> object returned by the ``fit_tree`` api.</param>
    ''' <param name="folder">
    ''' the output directory: the ``report.tsv``(fitting quality of each gene model) and
    ''' the ``importance.tsv``(split importance of each regulator) files are written
    ''' into this directory, it will be created if it is not exists.
    ''' </param>
    ''' <returns>TRUE when the table files have been written.</returns>
    <ExportAPI("write_report")>
    <RApiReturn(GetType(Boolean))>
    Public Function write_report(report As TrainingReport, folder As String) As Object
        Call report.Write(folder)
        Return True
    End Function

    ''' <summary>
    ''' Get the summary statistics of the model training.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a list with the slots: ``genes``, ``models``, ``boosted_models``,
    ''' ``constant_models``, ``source_nodes``, ``median_r2``, ``mean_r2``,
    ''' ``informative_models``, ``cells``, ``population`` and ``fitted``.
    ''' </returns>
    <ExportAPI("training_stats")>
    <RApiReturn(GetType(Double))>
    Public Function training_stats(tree As XGBoostGRN, Optional env As Environment = Nothing) As Object
        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If
        If tree.TrainingReport Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is not trained yet, please run the fit_tree api at first!", env)
        End If

        Dim report As TrainingReport = tree.TrainingReport
        Dim slots As New Dictionary(Of String, Object) From {
            {"genes", tree.GeneNames.Length},
            {"models", report.Models.SafeQuery.Count(Function(m) m IsNot Nothing)},
            {"boosted_models", report.BoostedModels},
            {"constant_models", report.ConstantModels},
            {"source_nodes", report.SourceNodes},
            {"median_r2", report.MedianR2},
            {"mean_r2", report.MeanR2},
            {"informative_models", report.InformativeModels},
            {"cells", report.Cells},
            {"population", tree.PopulationSize},
            {"fitted", tree.IsFitted},
            {"discovery_mode", tree.Config.EdgeDiscovery.ToString()},
            {"discovered_edges", tree.DiscoveredEdgesCount},
            {"discovery_targets", If(tree.Discovery Is Nothing, 0, tree.Discovery.TargetsExamined)}
        }

        Return New list(slots)
    End Function

    ''' <summary>
    ''' Get the candidate edges that were examined by the residual two-stage structure discovery.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="top_n">
    ''' the number of the records that are ordered by the R² gain in descending order, ``0``
    ''' returns all of them.
    ''' </param>
    ''' <param name="accepted_only">when TRUE only the accepted new edges are returned.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a dataframe whose columns are ``gene``, ``regulator``, ``status``, ``correlation``,
    ''' ``delta_r2``, ``r2_before``, ``r2_after``, ``splits``, ``sensitivity`` and ``reason``;
    ''' an R# error message object is returned when the model does not contain any discovery
    ''' result.
    ''' </returns>
    ''' <remarks>
    ''' the ``delta_r2`` of an accepted edge is the validation R² gain that the edge brought
    ''' to its target gene model, the ``correlation`` column is the pearson correlation between
    ''' the target gene residual and the candidate gene expression.
    ''' </remarks>
    <ExportAPI("discovered_edges")>
    <RApiReturn(GetType(RDataframe))>
    Public Function discovered_edges(tree As XGBoostGRN,
                                     Optional top_n As Integer = 50,
                                     Optional accepted_only As Boolean = True,
                                     Optional env As Environment = Nothing) As Object

        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If
        If tree.Discovery Is Nothing OrElse tree.Discovery.Edges.Length = 0 Then
            Return RInternal.debug.stop("no edge discovery result was found on the given GeneTree model, please enable the discovery via the discovery api before the model training!", env)
        End If

        Dim data As DiscoveryEdge() = tree.Discovery.Edges _
            .Where(Function(e) Not accepted_only OrElse e.isAccepted) _
            .OrderByDescending(Function(e) e.deltaR2) _
            .ToArray

        If top_n > 0 AndAlso data.Length > top_n Then
            data = data.Take(top_n).ToArray
        End If

        If data.Length = 0 Then
            Return RInternal.debug.stop($"the given GeneTree model contains no {"accepted"} edge discovery result!", env)
        End If

        Dim df As New RDataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = Enumerable.Range(1, data.Length).Select(Function(i) CStr(i)).ToArray
        }

        Call df.add("gene", data.Select(Function(e) e.target))
        Call df.add("regulator", data.Select(Function(e) e.regulator))
        Call df.add("status", data.Select(Function(e) e.status))
        Call df.add("correlation", data.Select(Function(e) e.correlation))
        Call df.add("delta_r2", data.Select(Function(e) e.deltaR2))
        Call df.add("r2_before", data.Select(Function(e) e.r2Before))
        Call df.add("r2_after", data.Select(Function(e) e.r2After))
        Call df.add("splits", data.Select(Function(e) e.splits))
        Call df.add("sensitivity", data.Select(Function(e) e.sensitivity))
        Call df.add("reason", data.Select(Function(e) If(e.reason, "")))

        Return df
    End Function

    ''' <summary>
    ''' Get the per gene fitting quality table of the trained model.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a dataframe whose rows are the modelled genes and whose columns are ``kind``,
    ''' ``n_regulators``, ``n_train``, ``n_validate``, ``r2``, ``rmse``,
    ''' ``baseline_rmse``, ``improvement``, ``trees``, ``min_value`` and ``max_value``.
    ''' </returns>
    <ExportAPI("report_table")>
    <RApiReturn(GetType(RDataframe))>
    Public Function report_table(tree As XGBoostGRN, Optional env As Environment = Nothing) As Object
        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Dim models As GeneModel() = tree.Models _
            .SafeQuery _
            .Where(Function(m) m IsNot Nothing) _
            .ToArray

        If models.Length = 0 Then
            Return RInternal.debug.stop("the given GeneTree model does not contains any gene model!", env)
        End If

        Dim df As New RDataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = models.Select(Function(m) m.Gene).ToArray
        }

        Call df.add("kind", models.Select(Function(m) If(m.Performance.kind, m.Kind)))
        Call df.add("n_regulators", models.Select(Function(m) m.Performance.nRegulators))
        Call df.add("n_train", models.Select(Function(m) m.Performance.nTrain))
        Call df.add("n_validate", models.Select(Function(m) m.Performance.nValidate))
        Call df.add("r2", models.Select(Function(m) m.Performance.r2))
        Call df.add("rmse", models.Select(Function(m) m.Performance.rmse))
        Call df.add("baseline_rmse", models.Select(Function(m) m.Performance.baselineRmse))
        Call df.add("improvement", models.Select(Function(m) m.Performance.improvement))
        Call df.add("trees", models.Select(Function(m) m.Performance.trees))
        Call df.add("min_value", models.Select(Function(m) m.Performance.minValue))
        Call df.add("max_value", models.Select(Function(m) m.Performance.maxValue))

        Return df
    End Function

    ''' <summary>
    ''' Get the split importance of the regulators, this table is the data driven
    ''' "sign and weight" of the ARACNE network edges.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="top_n">
    ''' the number of the regulator records that are ordered by the sensitivity in
    ''' descending order, ``0`` returns all of them.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a dataframe whose columns are ``gene``, ``regulator``, ``rank``, ``mi``,
    ''' ``correlation``, ``splits`` and ``sensitivity``.
    ''' </returns>
    <ExportAPI("feature_importance")>
    <RApiReturn(GetType(RDataframe))>
    Public Function feature_importance(tree As XGBoostGRN,
                                       Optional top_n As Integer = 20,
                                       Optional env As Environment = Nothing) As Object

        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Dim records As List(Of (gene As String, item As RegulatorImportance)) = New List(Of (gene As String, item As RegulatorImportance))

        For Each model As GeneModel In tree.Models.SafeQuery.Where(Function(m) m IsNot Nothing)
            For Each item As RegulatorImportance In model.Performance.importance.SafeQuery
                Call records.Add((model.Gene, item))
            Next
        Next

        If records.Count = 0 Then
            Return RInternal.debug.stop("the given GeneTree model does not contains any feature importance data!", env)
        End If

        Dim data As (gene As String, item As RegulatorImportance)() = records _
            .OrderByDescending(Function(r) r.item.sensitivity) _
            .ThenByDescending(Function(r) r.item.splits) _
            .ToArray

        If top_n > 0 AndAlso data.Length > top_n Then
            data = data.Take(top_n).ToArray
        End If

        Dim df As New RDataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = Enumerable.Range(1, data.Length).Select(Function(i) CStr(i)).ToArray
        }

        Call df.add("gene", data.Select(Function(r) r.gene))
        Call df.add("regulator", data.Select(Function(r) r.item.regulator))
        Call df.add("rank", data.Select(Function(r) r.item.rank))
        Call df.add("mi", data.Select(Function(r) r.item.mi))
        Call df.add("correlation", data.Select(Function(r) r.item.correlation))
        Call df.add("splits", data.Select(Function(r) r.item.splits))
        Call df.add("sensitivity", data.Select(Function(r) r.item.sensitivity))

        Return df
    End Function

    ''' <summary>
    ''' Get the gene alignment and the wildtype baseline of the model.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a list with the slots ``gene_names``, ``means``, ``sds``, ``population``,
    ''' ``has_context`` and ``context_fix``.
    ''' </returns>
    ''' <remarks>
    ''' the ``means`` and ``sds`` vectors are estimated from the cells that are used
    ''' as the initial states of the fixed point iteration, they define the z-score
    ''' normalization of the perturbation result.
    ''' </remarks>
    <ExportAPI("gene_stats")>
    <RApiReturn(GetType(Double))>
    Public Function gene_stats(tree As XGBoostGRN, Optional env As Environment = Nothing) As Object
        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Dim slots As New Dictionary(Of String, Object) From {
            {"gene_names", CType(tree.GeneNames.Clone(), String())},
            {"means", CType(tree.WildtypeMeans.Clone(), Double())},
            {"sds", CType(tree.WildtypeSDs.Clone(), Double())},
            {"population", tree.PopulationSize},
            {"has_context", tree.HasContext},
            {"context_fix", tree.ContextFix}
        }

        Return New list(slots)
    End Function

    ''' <summary>
    ''' Get the gene ids of the model.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <returns>a character vector of the gene id, the order is the row order of the expression matrix.</returns>
    <ExportAPI("gene_names")>
    <RApiReturn(GetType(String))>
    Public Function gene_names(tree As XGBoostGRN, Optional env As Environment = Nothing) As Object
        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Return CType(tree.GeneNames.Clone(), String())
    End Function

    ' ==================== expression context ====================

    ''' <summary>
    ''' Inject an expression state context into the model, the context becomes the
    ''' starting state as well as the differential analysis baseline of the following
    ''' in silico perturbation experiments.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="genes">
    ''' a character vector of the gene id of the context; the ids must live in the same
    ''' identifier space as the expression matrix that the model was trained on.
    ''' </param>
    ''' <param name="expression">
    ''' a numeric vector of the expression value of the context, in the same order as
    ''' <paramref name="genes"/>; the values may come from a different normalisation
    ''' pipeline, they are mapped onto the model scale by the quantile alignment below.
    ''' </param>
    ''' <param name="fix">
    ''' when TRUE the context genes are clamped to the given values during the whole
    ''' iteration(a gene knock-in like experiment); otherwise they are only used as
    ''' the starting point and will be updated by the network like the other genes.
    ''' 
    ''' clamping only makes sense for a small gene set: when the clamped gene number
    ''' exceeds ``MaxClampRatio``(5% by default) of the network, the whole network would
    ''' lose its degrees of freedom and the perturbation effect would vanish, so the
    ''' library degrades the context to "starting state only" automatically and reports
    ''' a warning here.
    ''' </param>
    ''' <param name="align">
    ''' whether to run the quantile alignment of the context onto the model expression
    ''' scale, default is TRUE: the context values are mapped onto the per gene
    ''' distribution of the model population, so that the relative order of the genes is
    ''' kept and no feature falls outside of the training range of its tree model.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the input model object that the context has been applied, so that it can be
    ''' chained with ``knockouts`` / ``overexpress`` / ``knockdown``;
    ''' an R# error message object is returned when the sizes of the two vectors do not
    ''' match or no gene of the context could be mapped onto the model.
    ''' </returns>
    ''' <remarks>
    ''' genes that are not described in the context keep their own wildtype value, so a
    ''' context is allowed to cover only a subset of the network.
    ''' </remarks>
    <ExportAPI("set_context")>
    <RApiReturn(GetType(XGBoostGRN))>
    Public Function set_context(tree As XGBoostGRN,
                                <RRawVectorArgument(TypeCodes.string)> genes As Object,
                                <RRawVectorArgument(TypeCodes.double)> expression As Object,
                                Optional fix As Boolean = False,
                                Optional align As Boolean = True,
                                Optional env As Environment = Nothing) As Object

        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Dim geneIds As String() = CLRVector.asCharacter(genes)
        Dim values As Double() = CLRVector.asNumeric(expression)

        If geneIds.Length <> values.Length Then
            Return RInternal.debug.stop(
                $"the size of the genes vector ({geneIds.Length}) does not match the size of the expression vector ({values.Length})", env)
        End If

        Dim context As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To geneIds.Length - 1
            context(geneIds(i)) = values(i)
        Next

        ' 对齐模式的切换是模型状态的一部分（不随本次调用恢复）：
        ' 关掉对齐之后再恢复模式会按新的模式重新对齐，反而让 align:=FALSE 失效。
        If Not align Then
            Call tree.SetContextAlignmentMode(ContextAlignmentMode.None)
        End If

        Try
            tree.FixContext = fix
            Call tree.SetContext(context)

            Dim note As String = tree.ContextNote

            If Not String.IsNullOrEmpty(note) AndAlso env IsNot Nothing Then
                Call env.AddMessage($"[geneTree] {note}", MSG_TYPES.WRN)
            End If

            Return tree
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Override the fixed point solving options of a trained model at runtime, so that
    ''' the solver can be tuned without re-training the whole network.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="iterations">
    ''' the soft iteration limit; a negative value keeps the current value.
    ''' </param>
    ''' <param name="damping">
    ''' the damping coefficient ``alpha`` in the range (0, 1]; a negative value keeps the
    ''' current value. A larger alpha moves faster but is easier to oscillate.
    ''' </param>
    ''' <param name="tolerance">
    ''' the relative convergence tolerance; a negative value keeps the current value.
    ''' </param>
    ''' <param name="hard_cap">
    ''' the hard iteration limit which is used when the soft limit is not enough for
    ''' convergence; a negative value keeps the current value, ``0`` restores the default
    ''' (``max(iterations * 5, 300)``).
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>the input model object that the solver options have been applied.</returns>
    ''' <remarks>
    ''' the control steady state and the last perturbation result are invalidated, so the
    ''' next perturbation experiment re-solves the control steady state with the new
    ''' options.
    ''' </remarks>
    <ExportAPI("solver_opts")>
    <RApiReturn(GetType(XGBoostGRN))>
    Public Function solver_opts(tree As XGBoostGRN,
                                Optional iterations As Integer = -1,
                                Optional damping As Double = -1,
                                Optional tolerance As Double = -1,
                                Optional hard_cap As Integer = -1,
                                Optional env As Environment = Nothing) As Object

        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Dim opt As GeneTreeConfig = tree.Config

        If iterations > 0 Then
            opt.MaxIterations = iterations
        End If
        If damping > 0 Then
            opt.DampingAlpha = damping
        End If
        If tolerance > 0 Then
            opt.ConvergenceTolerance = tolerance
        End If
        If hard_cap >= 0 Then
            opt.MaxIterationsHardCap = hard_cap
        End If

        Try
            Call opt.ValidateOrThrow()
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return tree.ResetSolver()
    End Function

    ''' <summary>
    ''' Remove the expression state context, the model is restored back to the wildtype
    ''' baseline that was estimated during the model training.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <returns>the input model object, so that it can be chained with the perturbation api.</returns>
    <ExportAPI("reset_context")>
    <RApiReturn(GetType(XGBoostGRN))>
    Public Function reset_context(tree As XGBoostGRN, Optional env As Environment = Nothing) As Object
        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Return tree.ClearContext()
    End Function

    ''' <summary>
    ''' Get the expression state context that is currently applied on the model.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <returns>
    ''' a list with the slots ``genes``, ``expression``, ``fix``, ``aligned``,
    ''' ``clamp_genes``, ``note`` and ``unknown``(the gene ids that could not be mapped
    ''' onto the model), the ``genes`` slot is an empty character vector when no context
    ''' has been applied.
    ''' </returns>
    <ExportAPI("get_context")>
    <RApiReturn(GetType(Double))>
    Public Function get_context(tree As XGBoostGRN, Optional env As Environment = Nothing) As Object
        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Dim genes As String() = New String() {}
        Dim values As Double() = New Double() {}
        Dim context As IReadOnlyDictionary(Of String, Double) = tree.Context

        If context IsNot Nothing Then
            genes = context.Keys.ToArray
            values = genes.Select(Function(g) context(g)).ToArray
        End If

        Dim slots As New Dictionary(Of String, Object) From {
            {"genes", genes},
            {"expression", values},
            {"fix", tree.ContextFix},
            {"aligned", tree.ContextAligned},
            {"clamp_genes", tree.ContextClampGenes},
            {"note", If(tree.ContextNote, "")},
            {"unknown", CType(tree.UnknownContextGenes.Clone(), String())}
        }

        Return New list(slots)
    End Function

    ''' <summary>
    ''' Solve the control steady state of the network, i.e. the "attractor" of the fitted
    ''' dynamical system when no intervention is applied.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a list with the slots ``gene_names`` and ``means``: the difference between the
    ''' mean and the wildtype baseline is the systematic drift of the tree model itself,
    ''' which could be subtracted from the perturbation result.
    ''' </returns>
    ''' <remarks>
    ''' the result is cached in the model object, the first call runs the fixed point
    ''' iteration and the later calls are cheap. When an expression context is applied,
    ''' the control steady state starts from the context state.
    ''' </remarks>
    <ExportAPI("steady_state")>
    <RApiReturn(GetType(Double))>
    Public Function steady_state(tree As XGBoostGRN, Optional env As Environment = Nothing) As Object
        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If
        If Not tree.IsFitted Then
            Return RInternal.debug.stop("the given GeneTree model is not trained yet, please run the fit_tree api at first!", env)
        End If

        Dim means As Double()

        Try
            means = tree.ControlSteadyState
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Dim slots As New Dictionary(Of String, Object) From {
            {"gene_names", CType(tree.GeneNames.Clone(), String())},
            {"means", means}
        }

        Return New list(slots)
    End Function

    ''' <summary>
    ''' Get the diagnostics of the last fixed point solving.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a list with the slots ``iterations``, ``converged``, ``converged_cells``,
    ''' ``max_change``, ``population`` and ``history``(the maximum relative change of
    ''' each iteration); an R# error message object is returned when no perturbation
    ''' experiment has been run yet.
    ''' </returns>
    <ExportAPI("solver_history")>
    <RApiReturn(GetType(Double))>
    Public Function solver_history(tree As XGBoostGRN, Optional env As Environment = Nothing) As Object
        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If
        If tree.LastSolve Is Nothing Then
            Return RInternal.debug.stop("no perturbation experiment has been run on the given GeneTree model yet!", env)
        End If

        Dim solved As SolveResult = tree.LastSolve
        Dim slots As New Dictionary(Of String, Object) From {
            {"iterations", solved.Iterations},
            {"converged", solved.Converged},
            {"converged_cells", solved.ConvergedCells},
            {"max_change", solved.MaxChange},
            {"population", solved.PopulationSize},
            {"collapsed", solved.Collapsed},
            {"exceeded_soft_limit", solved.ExceededSoftLimit},
            {"history", If(solved.History, New Double() {})}
        }

        Return New list(slots)
    End Function

    ' ==================== custom perturbation ====================

    ''' <summary>
    ''' Set the expression value of a set of genes to the given values, i.e. run a
    ''' ``Custom`` mode in silico intervention.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="genes">a character vector of the target gene id.</param>
    ''' <param name="expression">
    ''' a numeric vector of the expected expression value of each target gene, in the
    ''' same order as <paramref name="genes"/>; the genes are set to these values and
    ''' are clamped during the whole iteration.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the <see cref="InterventionResult"/> object of the intervention, which could be
    ''' exported via the ``make_exports`` api of the ``bnlearn`` package module;
    ''' an R# error message object is returned when the sizes of the two vectors do not
    ''' match or the prediction fails.
    ''' </returns>
    <ExportAPI("perturb")>
    <RApiReturn(GetType(InterventionResult))>
    Public Function perturb(tree As XGBoostGRN,
                            <RRawVectorArgument(TypeCodes.string)> genes As Object,
                            <RRawVectorArgument(TypeCodes.double)> expression As Object,
                            Optional env As Environment = Nothing) As Object

        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Dim geneIds As String() = CLRVector.asCharacter(genes)
        Dim values As Double() = CLRVector.asNumeric(expression)

        If geneIds.Length <> values.Length Then
            Return RInternal.debug.stop(
                $"the size of the genes vector ({geneIds.Length}) does not match the size of the expression vector ({values.Length})", env)
        End If

        Dim specs As New List(Of InterventionSpec)

        For i As Integer = 0 To geneIds.Length - 1
            Call specs.Add(New InterventionSpec With {
                .GeneName = geneIds(i),
                .GeneIndex = -1,
                .Mode = InterventionMode.Custom,
                .Value = values(i)
            })
        Next

        Try
            Return tree.Predict(specs)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Run a combined in silico intervention on a set of genes at the same time.
    ''' </summary>
    ''' <param name="tree">the trained <see cref="XGBoostGRN"/> model object.</param>
    ''' <param name="genes">a character vector of the gene id that are perturbed together.</param>
    ''' <param name="mode">
    ''' the intervention mode of all of the target genes: ``Knockout``, ``Knockdown``,
    ''' ``Overexpression`` or ``Custom``.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' one <see cref="InterventionResult"/> object of the combined intervention, the
    ''' non additive synergy/antagonism effect of the gene combination is kept;
    ''' an R# error message object is returned when the prediction fails.
    ''' </returns>
    ''' <remarks>
    ''' the ``Spec`` property of the result object is a synthetic intervention
    ''' specification whose gene name is the ``+`` joined id list.
    ''' </remarks>
    <ExportAPI("perturb_combined")>
    <RApiReturn(GetType(InterventionResult))>
    Public Function perturb_combined(tree As XGBoostGRN,
                                     <RRawVectorArgument(TypeCodes.string)> genes As Object,
                                     Optional mode As InterventionMode = InterventionMode.Knockout,
                                     Optional env As Environment = Nothing) As Object

        If tree Is Nothing Then
            Return RInternal.debug.stop("the given GeneTree model is nothing!", env)
        End If

        Dim geneIds As String() = CLRVector.asCharacter(genes)

        If geneIds.IsNullOrEmpty Then
            Return RInternal.debug.stop("the given target gene list is empty!", env)
        End If

        Try
            Return tree.PredictCombination(geneIds, mode)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

End Module
