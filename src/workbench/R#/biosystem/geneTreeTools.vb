Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.GeneTree
Imports SMRUCC.genomics.BioSystem.Intervention
Imports SMRUCC.genomics.Model.Network.Regulons.ARACNE
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
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
                                 Optional max_regulators As Integer = -1) As GeneTreeConfig

        Dim config As GeneTreeConfig = GeneTreeConfig.FromProfile(profile)

        config.PopulationSize = population
        config.MaxIterations = iterations
        config.Verbose = True

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
            {"fitted", tree.IsFitted}
        }

        Return New list(slots)
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
    ''' <paramref name="genes"/>; the values are on the same scale as the expression
    ''' matrix(the model does not apply any transform).
    ''' </param>
    ''' <param name="fix">
    ''' when TRUE the context genes are clamped to the given values during the whole
    ''' iteration(a gene knock-in like experiment); otherwise they are only used as
    ''' the starting point and will be updated by the network like the other genes.
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

        Try
            Return tree.SetContext(geneIds, values, fix)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
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
    ''' a list with the slots ``genes``, ``expression``, ``fix`` and ``unknown``(the
    ''' gene ids that could not be mapped onto the model), the ``genes`` slot is an
    ''' empty character vector when no context has been applied.
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
