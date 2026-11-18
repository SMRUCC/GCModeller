Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataMining.FuzzyLogic.TSK
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.FuzzyGene
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.FuzzyGene.IO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Core
Imports SMRUCC.genomics.BioSystem.Intervention
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports fz = SMRUCC.genomics.Analysis.FuzzyGene
Imports fzcfg = SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports fzana = SMRUCC.genomics.Analysis.FuzzyGene.Perturbation.Analysis
Imports fzdata = SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports fzpert = SMRUCC.genomics.Analysis.FuzzyGene.Perturbation
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' FuzzyGene: the neuro-fuzzy (Wilson/Takagi-Sugeno ANFIS) gene regulatory network toolkit.
''' </summary>
''' <remarks>
''' <para>
''' This R# package module wraps the ``FuzzyGene`` sub-system. Every target gene is described by
''' its own first-order Takagi-Sugeno fuzzy model whose inputs are 3-6 candidate regulators;
''' the rules of that model can be read as biological statements ("IF TF-A is High AND
''' TF-B is Low THEN gene-G is activated"), and the analytic partial derivatives of the model
''' give a signed, weighted regulatory edge table.
''' </para>
''' <para>
''' The workflow exposed by this module is:
''' </para>
''' <list type="bullet">
''' <item><description>
''' ``fuzzygene_config``: create the hyper-parameter configuration object
''' (<see cref="FuzzyGeneConfig"/>, registered as the
''' ``fuzzygene_config`` R# type);
''' </description></item>
''' <item><description>
''' ``build_fuzzygene``: build an untrained model from a gene expression matrix and an optional
''' prior regulatory network;
''' </description></item>
''' <item><description>
''' ``train``: fit every target gene (static route, or the lagged <c>x(t+1) = F(x(t))</c> route
''' when the matrix carries multiple time points);
''' </description></item>
''' <item><description>
''' ``fuzzygene_rules`` / ``fuzzygene_edges``: export the interpretable IF-THEN rules and the
''' signed regulatory edge table;
''' </description></item>
''' <item><description>
''' ``fuzzygene_trace_perturbation`` / ``fuzzygene_export_trace``: record the whole-genome
''' steady-state trajectory of one virtual perturbation (mutant and wildtype), either as two
''' matrices in memory or as CSV + ``SampleInfo`` sidecar files on disk;
''' </description></item>
''' <item><description>
''' ``fuzzygene_trace_convergence``: the solver diagnostics of that run (iteration vs
''' ``max_delta``), which tells whether the flat tail of a curve is a real steady state;
''' </description></item>
''' <item><description>
''' ``fuzzygene_rank_perturbations`` / ``fuzzygene_strongest_perturbation``: rank candidate
''' perturbations by predicted response strength;
''' </description></item>
''' <item><description>
''' ``fuzzygene_relaxation``: change the damping factor / iteration budget of the solver, which
''' is what you want when a genome scale model keeps oscillating instead of converging;
''' </description></item>
''' </list>
''' <para>
''' A trained <see cref="FuzzyGeneModel"/> implements the
''' <c>InsilicoPerturbationExperiment</c> interface, so the ``knockouts``, ``knockdown`` and
''' ``overexpress`` api of the ``bnlearn`` package can be applied to it directly for in silico
''' perturbation experiments (partial knockdown is natively supported because the fuzzy
''' membership functions describe graded regulation). The generic ``writeBin`` handler of this
''' module persists a model into a zip based binary stream, and ``readBin.fuzzygene`` restores
''' it back to a fully usable model object.
''' </para>
''' </remarks>
<Package("FuzzyGene")>
<RTypeExport("fuzzygene", GetType(FuzzyGeneModel))>
<RTypeExport("fuzzygene_config", GetType(FuzzyGeneConfig))>
Module fuzzyGene

    ''' <summary>
    ''' Register the custom binary serialization handlers of the FuzzyGene package.
    ''' </summary>
    ''' <remarks>
    ''' <list type="bullet">
    ''' <item><description>
    ''' ``writeBin`` for <see cref="FuzzyGeneModel"/>: the
    ''' model is written into the connection supplied by the ``con`` argument as a
    ''' ``*.fuzzygene.zip`` archive (manifest, one entry per target gene model, rule text and
    ''' the signed edge table).
    ''' </description></item>
    ''' <item><description>
    ''' ``readBin.fuzzygene`` for <see cref="Stream"/>: the stream is parsed as a FuzzyGene model
    ''' package and a fully reconstructed model object is returned, ready for inference and
    ''' virtual perturbation without retraining.
    ''' </description></item>
    ''' </list>
    ''' </remarks>
    <RInitialize>
    Public Sub Main()
        Call RInternal.generic.add("writeBin", GetType(fz.FuzzyGeneModel), AddressOf saveModel)
        Call RInternal.generic.add("readBin.fuzzygene", GetType(Stream), AddressOf loadModel)
    End Sub

    ''' <summary>
    ''' R# ``writeBin`` handler: persist a FuzzyGene model into the given connection.
    ''' </summary>
    ''' <param name="model">the trained model to be serialized</param>
    ''' <param name="args">the R# named argument list; the ``con`` argument supplies the output stream</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns><c>True</c> when the model has been written successfully</returns>
    Private Function saveModel(model As fz.FuzzyGeneModel, args As list, env As Environment) As Object
        Dim con As Stream = args!con

        If con Is Nothing Then
            Return RInternal.debug.stop("a valid connection stream is required for writeBin", env)
        End If

        Try
            Call model.Save(con)
            Call con.Flush()
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return True
    End Function

    ''' <summary>
    ''' R# ``readBin.fuzzygene`` handler: restore a model from a connection.
    ''' </summary>
    ''' <param name="s">the stream that contains a ``*.fuzzygene.zip`` package</param>
    ''' <param name="args">the R# named argument list (unused)</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the reconstructed model object, or an R# error object</returns>
    Private Function loadModel(s As Stream, args As list, env As Environment) As Object
        Try
            Return fz.FuzzyGeneModel.Load(s)
        Catch ex As Exception
            Return RInternal.debug.stop("failed to restore the fuzzy gene model package: " & ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Create a FuzzyGene modelling configuration object.
    ''' </summary>
    ''' <remarks>
    ''' The returned object (registered as the ``fuzzygene_config`` R# type) is accepted by the
    ''' ``config`` argument of ``FuzzyGene::build_fuzzygene``. Structure and training options are
    ''' inherited from the shared ANFIS configuration; this api exposes the subset that matters for
    ''' gene regulatory network inference.
    ''' </remarks>
    ''' <param name="maxRegulators">
    ''' how many candidate regulators (ANFIS inputs) are kept per target gene. The rule count of a
    ''' grid partition is <c>m^n</c>, so this is the primary guard against the rule explosion;
    ''' 3-6 is the practically useful range. Default 4.
    ''' </param>
    ''' <param name="selection">
    ''' regulator selection strategy: ``prior-first`` (default, prior edges by confidence then
    ''' correlation), ``correlation`` (data driven only) or ``prior`` (prior edges only).
    ''' </param>
    ''' <param name="minCorrelation">
    ''' minimum |Pearson| for a gene to be taken as a correlation based regulator. Default 0.3.
    ''' </param>
    ''' <param name="mode">
    ''' fit route: ``auto`` (default, lagged when the matrix has at least ``minTimePoints`` distinct
    ''' time points), ``static`` or ``lagged``.
    ''' </param>
    ''' <param name="mfCount">number of membership functions per input. Default 3.</param>
    ''' <param name="mfKind">
    ''' membership function shape: ``gaussian``, ``bell``, ``sigmoid``, ``triangular`` or
    ''' ``trapezoidal``. Default ``gaussian``.
    ''' </param>
    ''' <param name="maxEpoch">maximum training epochs per target gene. Default 100.</param>
    ''' <param name="learningRate">learning rate of the premise parameter optimizer. Default 0.02.</param>
    ''' <param name="pruneThreshold">
    ''' rules whose average normalized firing strength falls below this value are pruned after
    ''' training; 0 disables pruning. Default 0.01.
    ''' </param>
    ''' <param name="maxGenes">
    ''' upper bound on the number of modelled genes (highest variance first, prior TFs always kept);
    ''' 0 keeps everything. Default 2000.
    ''' </param>
    ''' <param name="maxTrainRows">
    ''' maximum number of training rows per target gene; 0 uses every row. The dominant cost of the
    ''' hybrid learning is the least squares normal equations, which scale as
    ''' <c>rows x P^2</c>, so capping the rows is the cheapest large speedup: a few thousand rows
    ''' are already far more than the few hundred consequent parameters need.
    ''' Default 0 (all rows); 3000-5000 is the recommended setting for genome scale data.
    ''' </param>
    ''' <param name="maxCandidateGenes">
    ''' size limit of the candidate pool used by the correlation screening
    ''' (prior TFs plus the highest variance genes); 0 screens every gene. The screening cost is
    ''' <c>O(genes^2 x rows)</c>, which at genome scale becomes comparable with the training itself.
    ''' Default 0; 2000-4000 is the recommended setting for genome scale data.
    ''' </param>
    ''' <param name="minBestCorrelation">
    ''' skip a target gene when it has no prior regulator and its best |Pearson| against the
    ''' candidates is below this value (such models have R2 close to 0). 0 disables the filter.
    ''' </param>
    ''' <param name="noveltyQuota">
    ''' fraction (0-1) of the regulator slots reserved for genes that are **not** in the prior
    ''' network of that target, i.e. for links discovered from the expression data.
    ''' ``0`` (default) keeps the old "prior first" behaviour, where prior TFs can occupy every
    ''' slot and the model can only repeat the prior. ``0.2``-``0.4`` is a sensible range:
    ''' with K = 6 and ``0.34`` two slots are always given to data driven regulators.
    ''' Ignored when ``selection = "prior-only"``.
    ''' </param>
    ''' <param name="rulePartition">
    ''' rule generation strategy: ``auto`` (default), ``grid`` or ``scatter``.
    ''' ``grid`` places m membership functions per input and enumerates <c>m^K</c> rules, so the rule
    ''' count explodes with the input count. ``scatter`` clusters the training samples (kmeans) and
    ''' creates one rule per cluster, each with its own gaussian center/width per input - the number of
    ''' rules no longer depends on K, which is what makes ``maxRegulators = 18`` feasible.
    ''' ``auto`` keeps ``grid`` while <c>m^K &lt;= maxRules</c> and <c>m^K*(K+1) &lt;= maxConsequentParams</c>,
    ''' otherwise it switches to ``scatter``.
    ''' </param>
    ''' <param name="scatterRules">number of clusters (= rules) of the scatter partition. Default 48.</param>
    ''' <param name="scatterInputs">maximum number of antecedents per scatter rule. Default 8.</param>
    ''' <param name="scatterMinInputs">minimum number of antecedents per scatter rule. Default 2.</param>
    ''' <param name="scatterZThreshold">
    ''' an input joins a cluster rule when the cluster center deviates from the global mean by at
    ''' least this many standard deviations. Default 1.
    ''' </param>
    ''' <param name="scatterSeed">random seed of the clustering, keeps the structure reproducible. Default 0.</param>
    ''' <param name="maxRules">rule budget per target gene. Default 128.</param>
    ''' <param name="maxConsequentParams">consequent parameter budget per target gene. Default 512.</param>
    ''' <param name="standardize">z-score the expression matrix before fitting. Default TRUE.</param>
    ''' <param name="propagationSteps">maximum fixed point iterations of a perturbation. Default 50.</param>
    ''' <param name="relaxation">damping factor of the fixed point iteration. Default 0.5.</param>
    ''' <param name="knockdownResidual">
    ''' fraction of the baseline expression retained after a virtual knockdown. Default 0.2.
    ''' </param>
    ''' <param name="overexpressionSigmas">
    ''' overexpression level in training standard deviations above the baseline. Default 3.
    ''' </param>
    ''' <param name="significanceZScore">|z| threshold used to flag a gene as significantly changed. Default 2.</param>
    ''' <param name="degreeOfParallelism">per-gene training parallelism; 0 = all cores, 1 = serial.</param>
    ''' <param name="minTimePoints">distinct time points required to switch to the lagged route. Default 3.</param>
    ''' <param name="env">the R# runtime environment (unused, kept for error reporting symmetry)</param>
    ''' <returns>a validated configuration object</returns>
    <ExportAPI("fuzzygene_config")>
    <RApiReturn(GetType(FuzzyGeneConfig))>
    Public Function fuzzygene_config(Optional maxRegulators As Integer = 4,
                                     Optional selection As String = "prior-first",
                                     Optional minCorrelation As Double = 0.3,
                                     Optional mode As String = "auto",
                                     Optional mfCount As Integer = 3,
                                     Optional mfKind As String = "gaussian",
                                     Optional maxEpoch As Integer = 100,
                                     Optional learningRate As Double = 0.02,
                                     Optional pruneThreshold As Double = 0.01,
                                     Optional maxGenes As Integer = 2000,
                                     Optional maxTrainRows As Integer = 0,
                                     Optional maxCandidateGenes As Integer = 0,
                                     Optional minBestCorrelation As Double = 0,
                                     Optional noveltyQuota As Double = 0,
                                     Optional rulePartition As String = "auto",
                                     Optional scatterRules As Integer = 48,
                                     Optional scatterInputs As Integer = 8,
                                     Optional scatterMinInputs As Integer = 2,
                                     Optional scatterZThreshold As Double = 1,
                                     Optional scatterSeed As Integer = 0,
                                     Optional maxRules As Integer = 128,
                                     Optional maxConsequentParams As Integer = 512,
                                     Optional standardize As Boolean = True,
                                     Optional propagationSteps As Integer = 50,
                                     Optional relaxation As Double = 0.5,
                                     Optional knockdownResidual As Double = 0.2,
                                     Optional overexpressionSigmas As Double = 3.0,
                                     Optional significanceZScore As Double = 2.0,
                                     Optional degreeOfParallelism As Integer = 0,
                                     Optional minTimePoints As Integer = 3,
                                     Optional env As Environment = Nothing) As fzcfg.FuzzyGeneConfig

        Dim config As New fzcfg.FuzzyGeneConfig With {
            .MaxRegulators = maxRegulators,
            .Selection = parseSelection(selection),
            .MinCorrelation = minCorrelation,
            .TargetMode = parseMode(mode),
            .MfCount = mfCount,
            .MfKind = parseMfKind(mfKind),
            .MaxEpoch = maxEpoch,
            .LearningRate = learningRate,
            .PruneRuleThreshold = pruneThreshold,
            .MaxGenes = maxGenes,
            .MaxTrainRows = maxTrainRows,
            .MaxCandidateGenes = maxCandidateGenes,
            .MinBestCorrelation = minBestCorrelation,
            .NoveltyQuota = noveltyQuota,
            .RulePartition = parseRulePartition(rulePartition),
            .ScatterRuleCount = scatterRules,
            .ScatterInputs = scatterInputs,
            .ScatterMinInputs = scatterMinInputs,
            .ScatterZThreshold = scatterZThreshold,
            .ScatterSeed = scatterSeed,
            .MaxRules = maxRules,
            .MaxConsequentParams = maxConsequentParams,
            .Standardize = standardize,
            .PropagationSteps = propagationSteps,
            .Relaxation = relaxation,
            .KnockdownResidual = knockdownResidual,
            .OverexpressionSigmas = overexpressionSigmas,
            .SignificanceZScore = significanceZScore,
            .DegreeOfParallelism = degreeOfParallelism,
            .MinTimePoints = minTimePoints
        }

        Return config.ValidateGene()
    End Function

    Private Function parseSelection(text As String) As fzcfg.RegulatorSelection
        Select Case If(text, "").Trim().ToLowerInvariant()
            Case "correlation", "correlation-only", "cor"
                Return fzcfg.RegulatorSelection.CorrelationOnly
            Case "prior", "prior-only"
                Return fzcfg.RegulatorSelection.PriorOnly
            Case Else
                Return fzcfg.RegulatorSelection.PriorFirst
        End Select
    End Function

    ''' <summary>
    ''' 解析规则划分方式：<c>auto</c> / <c>grid</c> / <c>scatter</c>。
    ''' </summary>
    ''' <remarks>
    ''' <c>auto</c>（默认）在网格装得下时用网格划分，装不下（例如 18 个调控因子）自动切散射划分。
    ''' </remarks>
    Private Function parseRulePartition(text As String) As RulePartitionKind
        Select Case If(text, "").Trim().ToLowerInvariant()
            Case "grid", "网格"
                Return RulePartitionKind.Grid
            Case "scatter", "sc", "cluster", "clustering", "散射"
                Return RulePartitionKind.Scatter
            Case Else
                Return RulePartitionKind.Auto
        End Select
    End Function

    Private Function parseMode(text As String) As fzcfg.FuzzyTargetMode
        Select Case If(text, "").Trim().ToLowerInvariant()
            Case "static"
                Return fzcfg.FuzzyTargetMode.StaticRoute
            Case "lagged", "dynamic", "time-series"
                Return fzcfg.FuzzyTargetMode.LaggedRoute
            Case Else
                Return fzcfg.FuzzyTargetMode.Auto
        End Select
    End Function

    Private Function parseMfKind(text As String) As MembershipKind
        Select Case If(text, "").Trim().ToLowerInvariant()
            Case "bell", "gbell", "gbellmf"
                Return MembershipKind.GeneralizedBell
            Case "sigmoid", "sigmf"
                Return MembershipKind.Sigmoid
            Case "triangular", "tri", "trimf"
                Return MembershipKind.Triangular
            Case "trapezoidal", "trap", "trapmf"
                Return MembershipKind.Trapezoidal
            Case Else
                Return MembershipKind.Gaussian
        End Select
    End Function

    ''' <summary>
    ''' Build an untrained neuro-fuzzy gene regulatory network model.
    ''' </summary>
    ''' <remarks>
    ''' The expression matrix provides the training data: when it carries ``TimePoints`` with at
    ''' least three distinct values the model learns the lagged map <c>x(t+1) = F(x(t))</c>,
    ''' otherwise it learns the static map <c>x_g = F(regulators)</c>. The prior network supplies
    ''' TF-to-target edges that are used to order (not to force) the candidate inputs.
    ''' </remarks>
    ''' <param name="x">the gene expression matrix (rows = genes, columns = samples)</param>
    ''' <param name="prior">
    ''' an optional prior regulatory network created by the ``bnlearn`` package (``prior_edges``);
    ''' when omitted the candidate regulators are selected by correlation only.
    ''' </param>
    ''' <param name="config">
    ''' the configuration created by ``FuzzyGene::fuzzygene_config``; ``Nothing`` uses the defaults.
    ''' </param>
    ''' <param name="env">the R# runtime environment (unused)</param>
    ''' <returns>an untrained model object</returns>
    <ExportAPI("build_fuzzygene")>
    <RApiReturn(GetType(FuzzyGeneModel))>
    Public Function build_fuzzygene(x As GeneExpressionData,
                                    Optional prior As PriorNetwork = Nothing,
                                    Optional config As fzcfg.FuzzyGeneConfig = Nothing,
                                    Optional env As Environment = Nothing) As fz.FuzzyGeneModel

        Call "start to build fuzzy gene network!".debug

        Return New fz.FuzzyGeneModel(x, prior, config)
    End Function

    ''' <summary>
    ''' Train the neuro-fuzzy model: one Takagi-Sugeno network per target gene.
    ''' </summary>
    ''' <remarks>
    ''' Training is per gene and therefore parallelizable; a gene that cannot be fitted (constant
    ''' expression, no candidate regulator, ill conditioned least squares) is recorded as skipped
    ''' or failed instead of aborting the whole run. Use ``FuzzyGene::fuzzygene_report`` to inspect
    ''' the per gene diagnostics.
    ''' </remarks>
    ''' <param name="fuzzygene">the untrained model created by ``FuzzyGene::build_fuzzygene``</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>the trained model object, or an R# error object when training fails</returns>
    <ExportAPI("train")>
    <RApiReturn(GetType(FuzzyGeneModel))>
    Public Function train(fuzzygene As fz.FuzzyGeneModel, Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing Then
            Return RInternal.debug.stop("a fuzzy gene model object is required", env)
        End If

        Try
            Call fuzzygene.Train()
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return fuzzygene
    End Function

    ''' <summary>
    ''' Export the IF-THEN rules of every fitted target gene.
    ''' </summary>
    ''' <remarks>
    ''' Each line reads like
    ''' <c>rule3 [12.34%] IF TF-A IS high AND TF-B IS low THEN gene-G = 1.2*TF-A + -0.8*TF-B + 0.1</c>：
    ''' the bracketed number is the average normalized firing strength on the training set, the
    ''' coefficients are the local linear conclusions of that rule.
    ''' </remarks>
    ''' <param name="fuzzygene">a trained model</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a character vector, one element per rule (with per gene section headers)</returns>
    <ExportAPI("fuzzygene_rules")>
    <RApiReturn(TypeCodes.string)>
    Public Function fuzzygene_rules(fuzzygene As fz.FuzzyGeneModel, Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Return fuzzygene.Rules()
    End Function

    ''' <summary>
    ''' Export the signed regulatory edge table of a trained model.
    ''' </summary>
    ''' <remarks>
    ''' The edge weight is the average analytic partial derivative
    ''' <c>∂x_target/∂x_regulator</c> over the training support: positive means activation,
    ''' negative means repression. ``confidence`` is the prior confidence when the edge is
    ''' supported by the prior network, otherwise the absolute expression correlation.
    ''' </remarks>
    ''' <param name="fuzzygene">a trained model</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a data.frame with columns source, target, weight, regulation, confidence and prior</returns>
    <ExportAPI("fuzzygene_edges")>
    <RApiReturn(GetType(dataframe))>
    Public Function fuzzygene_edges(fuzzygene As fz.FuzzyGeneModel, Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Dim edges As fzdata.RegulationEdgeModel() = fuzzygene.Edges
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Call table.add("source", edges.Select(Function(e) e.Source))
        Call table.add("target", edges.Select(Function(e) e.Target))
        Call table.add("weight", edges.Select(Function(e) e.Weight))
        Call table.add("regulation", edges.Select(Function(e) If(e.IsActivation, "activation", "repression")))
        Call table.add("confidence", edges.Select(Function(e) e.Confidence))
        Call table.add("prior", edges.Select(Function(e) e.IsPrior))

        Return table
    End Function

    ''' <summary>
    ''' Export the signed regulation weight matrix of a trained model.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Rows are target genes, columns are regulators, and the element
    ''' <c>W[i, j]</c> is the signed influence of regulator <c>j</c> on target <c>i</c>
    ''' (the average analytic partial derivative <c>dx_target/dx_regulator</c>):
    ''' positive means activation, negative means repression, zero means no modelled edge.
    ''' </para>
    ''' <para>
    ''' The result is a standard ``Matrix`` object, so it can be written directly with
    ''' ``geneExpression::write.expr_matrix()`` or fed into downstream network analysis:
    ''' <code>
    ''' let w = FuzzyGene::fuzzygene_regulation_matrix(model, layout = "compact");
    ''' geneExpression::write.expr_matrix(w, file = "/path/to/network_weight.csv", id = "geneID");
    ''' </code>
    ''' </para>
    ''' <para>
    ''' Layouts: ``square`` (strict N x N, rows and columns are all model genes),
    ''' ``regulators`` (columns limited to genes that really appear as regulators) and
    ''' ``compact`` (rows = targets with at least one edge, columns = regulators).
    ''' For a genome scale model the square layout is <c>N^2</c> doubles
    ''' (about 2.7 GB for 18,533 genes), so prefer ``compact``/``regulators``
    ''' together with the ``genes`` row filter.
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained model</param>
    ''' <param name="layout">``square`` (default), ``regulators`` or ``compact``</param>
    ''' <param name="minWeight">edges with a smaller absolute weight are exported as zero; 0 keeps all</param>
    ''' <param name="genes">restrict the exported rows to these target genes; empty means all</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a Matrix object (rows = targets, columns = regulators)</returns>
    <ExportAPI("fuzzygene_regulation_matrix")>
    <RApiReturn(GetType(SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix))>
    Public Function fuzzygene_regulation_matrix(fuzzygene As fz.FuzzyGeneModel,
                                                Optional layout As String = "square",
                                                Optional minWeight As Double = 0,
                                                Optional genes As String() = Nothing,
                                                Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Try
            Dim mode As RegulationMatrixLayout = ParseRegulationMatrixLayout(layout)

            Call WarnSquareLayout(fuzzygene, mode)

            Return fuzzygene.RegulationMatrix(mode, minWeight, genes)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Export the confidence matrix of a trained model (same shape as the weight matrix).
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Rows and columns are exactly the same as in ``fuzzygene_regulation_matrix`` (same layout,
    ''' same order), only the element value changes: <c>W[i, j]</c> becomes the confidence of the
    ''' edge <c>j -&gt; i</c>, that is the prior confidence when the edge is supported by the prior
    ''' network and the absolute expression correlation <c>|Pearson|</c> otherwise.
    ''' </para>
    ''' <para>
    ''' Weight and confidence answer two different questions ("how strong / which direction" vs
    ''' "how trustworthy"), so exporting both matrices and using weight for the edge width and
    ''' confidence for the edge opacity or a cutoff is the usual combination:
    ''' <code>
    ''' let w = FuzzyGene::fuzzygene_regulation_matrix(model, layout = "compact");
    ''' let c = FuzzyGene::fuzzygene_confidence_matrix(model, layout = "compact");
    ''' geneExpression::write.expr_matrix(w, file = "/path/weight.csv", id = "geneID");
    ''' geneExpression::write.expr_matrix(c, file = "/path/confidence.csv", id = "geneID");
    ''' </code>
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained model</param>
    ''' <param name="layout">``square`` (default), ``regulators`` or ``compact``</param>
    ''' <param name="minConfidence">edges with a smaller confidence are exported as zero; 0 keeps all</param>
    ''' <param name="genes">restrict the exported rows to these target genes; empty means all</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a Matrix object (rows = targets, columns = regulators, non negative values)</returns>
    <ExportAPI("fuzzygene_confidence_matrix")>
    <RApiReturn(GetType(SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix))>
    Public Function fuzzygene_confidence_matrix(fuzzygene As fz.FuzzyGeneModel,
                                                Optional layout As String = "square",
                                                Optional minConfidence As Double = 0,
                                                Optional genes As String() = Nothing,
                                                Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Try
            Dim mode As RegulationMatrixLayout = ParseRegulationMatrixLayout(layout)

            Call WarnSquareLayout(fuzzygene, mode)

            Return fuzzygene.ConfidenceMatrix(mode, minConfidence, genes)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>把 R# 侧的布局字符串解析成枚举（大小写与别名都容忍）。</summary>
    Private Function ParseRegulationMatrixLayout(layout As String) As RegulationMatrixLayout
        Select Case If(layout, "square").Trim().ToLowerInvariant()
            Case "square", "nxn"
                Return RegulationMatrixLayout.Square
            Case "regulators", "regulator", "tf", "tfonly"
                Return RegulationMatrixLayout.RegulatorsOnly
            Case "compact", "sparse"
                Return RegulationMatrixLayout.Compact
            Case Else
                Throw New ArgumentException($"unknown layout '{layout}': expected square, regulators or compact")
        End Select
    End Function

    ''' <summary>建模基因数很大时，方阵会占用 GB 级内存，这里给出显式告警。</summary>
    Private Sub WarnSquareLayout(fuzzygene As fz.FuzzyGeneModel, mode As RegulationMatrixLayout)
        If mode <> RegulationMatrixLayout.Square OrElse fuzzygene.Network.NGene <= 5000 Then
            Return
        End If

        Dim n As Integer = fuzzygene.Network.NGene
        Dim gigabytes As Double = CDbl(n) * n * 8.0 / 1024 / 1024 / 1024
        Dim message As String = $"warning: the square layout materialises {n} x {n} doubles (~{gigabytes:F2} GB in memory); " &
                                "consider layout = 'compact' or 'regulators', optionally combined with the genes filter"

        Call message.debug
    End Sub

    ''' <summary>
    ''' Inject a transcriptome context as the initial condition of the virtual perturbation.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The context replaces the control baseline: both the wildtype and the mutant state are
    ''' propagated **from the given condition**, so ``foldchange`` is read as
    ''' "mutant vs wildtype inside this context" instead of "mutant vs the training mean".
    ''' </para>
    ''' <para>
    ''' The table (csv or tsv) must carry at least one gene column and one expression column.
    ''' All non-numeric columns are treated as candidate gene keys and the one that matches the
    ''' most model genes is used, so a ``gene_name,gene_id,mean_counts`` pseudobulk table works
    ''' out of the box. Rows whose expression value is not a finite number are skipped (never
    ''' silently turned into zero), and model genes that are absent from the table keep the
    ''' training baseline.
    ''' </para>
    ''' <para>
    ''' Note that a fitted static map is usually contractive: after it converges, the state no
    ''' longer depends on where the propagation started. The context therefore matters most for
    ''' (i) the reference state used by fold changes, (ii) non-converged / transient responses
    ''' and (iii) lagged (time series) models.
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained model</param>
    ''' <param name="context">the context table path (csv/tsv) with a gene column and an expression column</param>
    ''' <param name="valueColumn">
    ''' the name of the expression column; empty uses the last column of the table
    ''' </param>
    ''' <param name="minCoverage">
    ''' the minimum fraction (0-1) of model genes that must be covered by the table,
    ''' 0 disables the check
    ''' </param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a multi line coverage summary, or an R# error object when the context is rejected</returns>
    <ExportAPI("fuzzygene_context")>
    <RApiReturn(TypeCodes.string)>
    Public Function fuzzygene_context(fuzzygene As fz.FuzzyGeneModel,
                                      context As Object,
                                      Optional valueColumn As String = Nothing,
                                      Optional minCoverage As Double = 0,
                                      Optional align As ContextAlignmentMode? = Nothing,
                                      Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Dim report As ContextAlignmentDiagnostics

        Try
            ' 覆盖率下限现在是模型属性（不再作为 LoadContext 的参数）
            fuzzygene.MinContextCoverage = minCoverage

            If TypeOf context Is InterventionResult Then
                align = If(align, ContextAlignmentMode.None)
                report = fuzzygene.LoadContext(DirectCast(context, InterventionResult), align)
            Else
                If align IsNot Nothing Then
                    Call fuzzygene.SetContextAlignmentMode(align)
                End If

                report = fuzzygene.LoadContext(CStr(context), valueColumn)
            End If

            ' 拆成字符串数组返回，print() 在 R# 里才会一行一行显示
            Return report.ToString().LineTokens
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        Finally
            fuzzygene.MinContextCoverage = 0
        End Try
    End Function

    ''' <summary>
    ''' Drop the injected context and restore the training baseline.
    ''' </summary>
    ''' <param name="fuzzygene">a trained model</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns><c>True</c> on success</returns>
    <ExportAPI("fuzzygene_reset_context")>
    <RApiReturn(TypeCodes.boolean)>
    Public Function fuzzygene_reset_context(fuzzygene As fz.FuzzyGeneModel, Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Try
            Call fuzzygene.ResetContext()
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return True
    End Function

    ''' <summary>
    ''' The configuration attached to a model (identical to ``config.json`` inside the package).
    ''' </summary>
    ''' <remarks>
    ''' Useful to confirm what the model was actually fitted with, especially the rule budget
    ''' (``maxRules`` / ``maxConsequentParams``) and ``noveltyQuota``. Remember that
    ''' ``maxRegulators`` / ``mfCount`` are only upper bounds: the real structure is reduced by the
    ''' rule budget, which is reported on the ``structure:`` line of ``fuzzygene_report``.
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>the configuration object</returns>
    <ExportAPI("fuzzygene_model_config")>
    <RApiReturn(GetType(FuzzyGeneConfig))>
    Public Function fuzzygene_model_config(fuzzygene As fz.FuzzyGeneModel, Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing Then
            Return RInternal.debug.stop("a fuzzy gene model object is required", env)
        End If

        Return fuzzygene.Config
    End Function

    ''' <summary>
    ''' Inject the result of an **upstream perturbation** as the control baseline (iterative perturbation).
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' After this call the mutant state of the upstream experiment becomes the reference state of the
    ''' model, so the next perturbation reports "effect of B given that A is already knocked out"
    ''' instead of "effect of B from the original baseline".
    ''' <code>
    ''' let round1 = model |> knockouts(c("A"));           # A 失活
    ''' print(FuzzyGene::fuzzygene_context_upstream(model, round1[[1]]));
    ''' let round2 = model |> knockouts(c("B"));           # 在 A 失活的背景上敲 B
    ''' </code>
    ''' </para>
    ''' <para>
    ''' ``useMutant = TRUE`` (default) takes ``MutantMeans``; ``FALSE`` takes ``WildtypeMeans``, which
    ''' restores the reference frame of the upstream round. An upstream result flagged as
    ''' ``Undefined`` (the target gene was not modelled) is rejected instead of being silently applied.
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained model</param>
    ''' <param name="upstream">an <c>InterventionResult</c>, e.g. one element of <c>knockouts(...)</c></param>
    ''' <param name="useMutant">take the mutant (perturbed) state instead of the wildtype one. Default TRUE.</param>
    ''' <param name="minCoverage">minimum fraction (0-1) of model genes the upstream result must cover; 0 disables the check</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a multi line coverage summary, or an R# error object when the context is rejected</returns>
    <ExportAPI("fuzzygene_context_upstream")>
    <RApiReturn(TypeCodes.string)>
    Public Function fuzzygene_context_upstream(fuzzygene As fz.FuzzyGeneModel,
                                               upstream As InterventionResult,
                                               Optional useMutant As Boolean = True,
                                               Optional minCoverage As Double = 0,
                                               Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If upstream Is Nothing Then
            Return RInternal.debug.stop("an upstream perturbation result is required, for example one element of knockouts(...)", env)
        End If

        Try
            Dim table As ContextTable = ContextTable.FromUpstreamIntervention(upstream, useMutant)

            ' 覆盖率下限现在是模型属性，不再作为 LoadContext 的参数
            fuzzygene.MinContextCoverage = minCoverage

            Try
                Dim report = fuzzygene.LoadContext(table)

                Return report.ToString().LineTokens
            Finally
                fuzzygene.MinContextCoverage = 0
            End Try
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Human readable training report of a fitted model.
    ''' </summary>
    ''' <param name="fuzzygene">a trained model</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a multi line summary, or an R# error object when no training result is attached</returns>
    <ExportAPI("fuzzygene_report")>
    <RApiReturn(TypeCodes.string)>
    Public Function fuzzygene_report(fuzzygene As fz.FuzzyGeneModel, Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing Then
            Return RInternal.debug.stop("a fuzzy gene model object is required", env)
        End If

        Dim report = fuzzygene.LastTrainingResult
        Dim text As New StringBuilder()

        Call text.AppendLine(fuzzygene.ToString())

        If report IsNot Nothing Then
            Call text.AppendLine(report.ToString())
        Else
            Call text.AppendLine("(restored from a package: no training diagnostics available)")
        End If

        Return text.ToString()
    End Function

    ''' <summary>
    ''' Save a trained model into a ``*.fuzzygene.zip`` package.
    ''' </summary>
    ''' <remarks>
    ''' ``writeBin`` is the generic R# way to do the same thing on a connection; this api is a
    ''' convenience wrapper for scripts that already have a file path.
    ''' </remarks>
    ''' <param name="fuzzygene">a trained model</param>
    ''' <param name="file">the output package path</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns><c>True</c> when the package has been written</returns>
    <ExportAPI("save_fuzzygene")>
    <RApiReturn(TypeCodes.boolean)>
    Public Function save_fuzzygene(fuzzygene As fz.FuzzyGeneModel, file As String, Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If String.IsNullOrWhiteSpace(file) Then
            Return RInternal.debug.stop("the output file path is empty", env)
        End If

        Try
            Call fuzzygene.Save(file)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return True
    End Function

    ''' <summary>
    ''' Load a model from a ``*.fuzzygene.zip`` package.
    ''' </summary>
    ''' <param name="file">the package path</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a trained model object ready for inference and virtual perturbation</returns>
    <ExportAPI("load_fuzzygene")>
    <RApiReturn(GetType(FuzzyGeneModel))>
    Public Function load_fuzzygene(file As String, Optional env As Environment = Nothing) As Object
        If String.IsNullOrWhiteSpace(file) Then
            Return RInternal.debug.stop("the package path is empty", env)
        End If
        If Not file.FileExists Then
            Return RInternal.debug.stop($"the model package does not exist: {file}", env)
        End If

        Try
            Return fz.FuzzyGeneModel.Load(file)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

#Region "稳态求解轨迹"

    ''' <summary>
    ''' Record the whole-genome steady-state trajectory of one virtual perturbation.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Returns a named list:
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>
    ''' ``mutant``: a Matrix (rows = all genes, columns = iteration frames ``mut_t000``...);
    ''' </description></item>
    ''' <item><description>
    ''' ``wildtype``: the same shape for the unperturbed baseline;
    ''' </description></item>
    ''' <item><description>
    ''' ``convergence``: a data.frame ``iteration`` / ``max_delta`` of the mutant run;
    ''' </description></item>
    ''' <item><description>
    ''' ``control_convergence``: the same for the wildtype run;
    ''' </description></item>
    ''' <item><description>
    ''' ``changes``: a data.frame ``gene`` / ``fold_change`` / ``percent_change`` / ``z_score`` /
    ''' ``significant`` of the final state;
    ''' </description></item>
    ''' <item><description>
    ''' ``summary``: a one row data.frame with the perturbation definition and the solver diagnostics.
    ''' </description></item>
    ''' </list>
    ''' <para>
    ''' ``mutant`` and ``wildtype`` are .NET <c>HTS.DataFrame.Matrix</c> objects (rows = genes,
    ''' columns = frames), not R matrices, so ``dim()`` returns 1 on them. Pass them straight to a
    ''' downstream consumer instead:
    ''' <code>
    ''' let trace = FuzzyGene::fuzzygene_trace_perturbation(model, "TPPP3");
    ''' geneExpression::write.expr_matrix(trace$mutant, file = "/path/mutant.csv", id = "geneID");
    ''' </code>
    ''' </para>
    ''' <para>
    ''' The column index is a <b>damped iteration step</b>, not biological time: ``omega`` changes
    ''' the shape of the curve while a converged end point stays the same (see
    ''' ``fuzzygene_relaxation``). Only a lagged (time series) model with ``omega = 1`` makes one
    ''' frame equal to one sampling interval.
    ''' </para>
    ''' <para>
    ''' Read ``convergence`` / ``summary$converged`` before interpreting the curve: a flat tail
    ''' produced by running out of ``steps`` is a truncated plateau, not a steady state.
    ''' </para>
    ''' <para>
    ''' ``wildtype`` is not decoration. The control steady state used by the perturbation engine is
    ''' the training baseline, which is generally <b>not</b> a fixed point of the network; the
    ''' wildtype trajectory shows how much the system drifts on its own, so that drift can be told
    ''' apart from the perturbation effect.
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="gene">the target gene of the virtual perturbation</param>
    ''' <param name="mode">``knockout`` (default), ``knockdown`` or ``overexpression``</param>
    ''' <param name="steps">maximum iteration steps; 0 uses the model configuration</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a named list of matrices, data.frames and the run summary</returns>
    <ExportAPI("fuzzygene_trace_perturbation")>
    <RApiReturn(GetType(list))>
    Public Function fuzzygene_trace_perturbation(fuzzygene As fz.FuzzyGeneModel,
                                                 gene As String,
                                                 Optional mode As String = "knockout",
                                                 Optional steps As Integer = 0,
                                                 Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If String.IsNullOrEmpty(gene) Then
            Return RInternal.debug.stop("a target gene is required", env)
        End If

        Dim intervention As InterventionMode
        Dim trace As fzpert.PerturbationTrace

        If Not parseInterventionMode(mode, intervention) Then
            Return RInternal.debug.stop($"unknown perturbation mode '{mode}', expected knockout / knockdown / overexpression", env)
        End If

        Try
            trace = fuzzygene.TracePerturbation(gene, intervention, steps)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        If trace Is Nothing OrElse trace.Mutant Is Nothing OrElse trace.Control Is Nothing Then
            Return RInternal.debug.stop($"the target gene '{gene}' is not modelled by this network", env)
        End If

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"mutant", trace.Mutant.ToMatrix("mut_t", $"fuzzygene trace (mutant, {intervention}:{gene})")},
                {"wildtype", trace.Control.ToMatrix("wt_t", "fuzzygene trace (wildtype)")},
                {"convergence", convergenceTable(trace.Mutant)},
                {"control_convergence", convergenceTable(trace.Control)},
                {"changes", changesTable(trace.Result)},
                {"summary", summaryTable(trace, gene)}
            }
        }
    End Function

    ''' <summary>
    ''' Solver diagnostics of one virtual perturbation, without building the two large matrices.
    ''' </summary>
    ''' <remarks>
    ''' Same solve as ``fuzzygene_trace_perturbation`` but only the ``convergence`` /
    ''' ``control_convergence`` / ``summary`` elements are returned. Use it when all you need is the
    ''' <c>log10(max_delta)</c> versus iteration plot: constructing the 18k x 250 matrices is the
    ''' expensive part.
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="gene">the target gene of the virtual perturbation</param>
    ''' <param name="mode">``knockout`` (default), ``knockdown`` or ``overexpression``</param>
    ''' <param name="steps">maximum iteration steps; 0 uses the model configuration</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a named list with ``convergence``, ``control_convergence`` and ``summary``</returns>
    <ExportAPI("fuzzygene_trace_convergence")>
    <RApiReturn(GetType(list))>
    Public Function fuzzygene_trace_convergence(fuzzygene As fz.FuzzyGeneModel,
                                                gene As String,
                                                Optional mode As String = "knockout",
                                                Optional steps As Integer = 0,
                                                Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If String.IsNullOrEmpty(gene) Then
            Return RInternal.debug.stop("a target gene is required", env)
        End If

        Dim intervention As InterventionMode
        Dim trace As fzpert.PerturbationTrace

        If Not parseInterventionMode(mode, intervention) Then
            Return RInternal.debug.stop($"unknown perturbation mode '{mode}', expected knockout / knockdown / overexpression", env)
        End If

        Try
            trace = fuzzygene.TracePerturbation(gene, intervention, steps)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        If trace Is Nothing OrElse trace.Mutant Is Nothing Then
            Return RInternal.debug.stop($"the target gene '{gene}' is not modelled by this network", env)
        End If

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"convergence", convergenceTable(trace.Mutant)},
                {"control_convergence", convergenceTable(trace.Control)},
                {"summary", summaryTable(trace, gene)}
            }
        }
    End Function

    ''' <summary>
    ''' Export the two trajectory matrices of one virtual perturbation to a folder.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Writes, into <paramref name="dir"/>:
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>``trace.mutant.csv`` / ``trace.wildtype.csv``: gene x iteration matrices;</description></item>
    ''' <item><description>``*.sampleinfo.json``: one <c>SampleInfo</c> row per column (``time`` / ``iteration``
    ''' / ``omega`` / ``converged`` / ``perturbed_genes`` ...), ready for the ExperimentDesigner
    ''' pipeline;</description></item>
    ''' <item><description>``trace.perturbation.json``: the run summary plus the whole ``max_delta`` sequence.</description></item>
    ''' </list>
    ''' <para>
    ''' ``dt`` only affects ``metadata$time`` of the sidecar (<c>time = iteration * dt</c>); it does
    ''' not change the matrix. Leave it at 1 unless the model is a lagged (time series) model and
    ''' the sampling interval is known.
    ''' </para>
    ''' <code>
    ''' let files = FuzzyGene::fuzzygene_export_trace(model, "TPPP3", "/path/trace", mode = "knockout");
    ''' print(files);
    ''' </code>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="gene">the target gene of the virtual perturbation</param>
    ''' <param name="dir">the output folder (created when missing)</param>
    ''' <param name="mode">``knockout`` (default), ``knockdown`` or ``overexpression``</param>
    ''' <param name="steps">maximum iteration steps; 0 uses the model configuration</param>
    ''' <param name="dt">time unit of one iteration frame, used by the sidecar only</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a character vector of the written file paths</returns>
    <ExportAPI("fuzzygene_export_trace")>
    <RApiReturn(TypeCodes.string)>
    Public Function fuzzygene_export_trace(fuzzygene As fz.FuzzyGeneModel,
                                           gene As String,
                                           dir As String,
                                           Optional mode As String = "knockout",
                                           Optional steps As Integer = 0,
                                           Optional dt As Double = 1,
                                           Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If String.IsNullOrEmpty(gene) Then
            Return RInternal.debug.stop("a target gene is required", env)
        End If
        If String.IsNullOrWhiteSpace(dir) Then
            Return RInternal.debug.stop("the output folder path is empty", env)
        End If
        If dt <= 0 Then
            Return RInternal.debug.stop("dt must be positive", env)
        End If

        Dim intervention As InterventionMode
        Dim trace As fzpert.PerturbationTrace

        If Not parseInterventionMode(mode, intervention) Then
            Return RInternal.debug.stop($"unknown perturbation mode '{mode}', expected knockout / knockdown / overexpression", env)
        End If

        Try
            trace = fuzzygene.TracePerturbation(gene, intervention, steps)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        If trace Is Nothing OrElse trace.Mutant Is Nothing Then
            Return RInternal.debug.stop($"the target gene '{gene}' is not modelled by this network", env)
        End If

        Try
            Return trace.Export(dir, dt)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Rank candidate perturbations by their predicted response strength.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Running a perturbation for every gene of a genome scale model takes hours, so only
    ''' <paramref name="candidates"/> genes (an evenly spaced sample of the modelled genes, no
    ''' random numbers involved) are evaluated.
    ''' </para>
    ''' <para>
    ''' The score is the largest <c>|Z|</c> among the genes that were <b>not</b> clamped: the Z score
    ''' of the target itself is the imposed shift, not a model prediction. Note that with
    ''' ``clampToSupport`` enabled the largest <c>|Z|</c> often saturates at the same value for many
    ''' candidates; the ``affected`` column (number of significant genes) is the tie breaker, and
    ''' is a better ranking criterion in that case.
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="candidates">number of candidate genes to evaluate; 0 means all modelled genes</param>
    ''' <param name="mode">``knockout`` (default), ``knockdown`` or ``overexpression``</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a data.frame ``gene`` / ``max_abs_z`` / ``affected`` / ``elapsed_ms``, best first</returns>
    <ExportAPI("fuzzygene_rank_perturbations")>
    <RApiReturn(GetType(dataframe))>
    Public Function fuzzygene_rank_perturbations(fuzzygene As fz.FuzzyGeneModel,
                                                 Optional candidates As Integer = 24,
                                                 Optional mode As String = "knockout",
                                                 Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Dim intervention As InterventionMode
        Dim ranked As fzpert.PerturbationCandidate()

        If Not parseInterventionMode(mode, intervention) Then
            Return RInternal.debug.stop($"unknown perturbation mode '{mode}', expected knockout / knockdown / overexpression", env)
        End If

        Try
            ranked = fuzzygene.RankPerturbations(candidates, intervention)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Call table.add("gene", ranked.Select(Function(c) c.GeneName))
        Call table.add("max_abs_z", ranked.Select(Function(c) c.MaxAbsZScore))
        Call table.add("affected", ranked.Select(Function(c) c.NAffected))
        Call table.add("elapsed_ms", ranked.Select(Function(c) c.ElapsedMs))

        Return table
    End Function

    ''' <summary>
    ''' The single gene whose perturbation produces the strongest predicted response.
    ''' </summary>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="candidates">number of candidate genes to evaluate; 0 means all modelled genes</param>
    ''' <param name="mode">``knockout`` (default), ``knockdown`` or ``overexpression``</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>the gene name, or an R# error object when nothing could be evaluated</returns>
    <ExportAPI("fuzzygene_strongest_perturbation")>
    <RApiReturn(TypeCodes.string)>
    Public Function fuzzygene_strongest_perturbation(fuzzygene As fz.FuzzyGeneModel,
                                                     Optional candidates As Integer = 24,
                                                     Optional mode As String = "knockout",
                                                     Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Dim intervention As InterventionMode
        Dim gene As String

        If Not parseInterventionMode(mode, intervention) Then
            Return RInternal.debug.stop($"unknown perturbation mode '{mode}', expected knockout / knockdown / overexpression", env)
        End If

        Try
            gene = fuzzygene.FindStrongestPerturbation(candidates, intervention)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        If String.IsNullOrEmpty(gene) Then
            Return RInternal.debug.stop("no candidate could be evaluated", env)
        End If

        Return gene
    End Function

    ''' <summary>
    ''' Set the damping factor, the iteration budget and the tolerance used by the steady-state solver.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The trajectory produced by ``fuzzygene_trace_perturbation`` is the damped fixed-point
    ''' iteration <c>z &lt;- z + omega * (F(z) - z)</c>. With ``omega = 1`` (no damping) the iteration
    ''' is the plain repeated application of <c>F</c>, and on a genome scale model that often keeps
    ''' oscillating instead of converging; ``omega = 0.5`` is the usual compromise between stability
    ''' and speed.
    ''' </para>
    ''' <para>
    ''' Always rerun the trajectory after changing these values, and check ``summary$converged``:
    ''' a non converged run means the flat tail of the curve is a truncated plateau.
    ''' </para>
    ''' <code>
    ''' model = FuzzyGene::fuzzygene_relaxation(model, omega = 0.5, steps = 80);
    ''' let t = FuzzyGene::fuzzygene_trace_perturbation(model, "TPPP3");
    ''' print(t$summary);
    ''' </code>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="omega">damping factor in (0, 1]; 0.5 is the recommended value</param>
    ''' <param name="steps">maximum iteration steps, must be positive</param>
    ''' <param name="tolerance">convergence threshold on the per step max change, must be positive</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>the same model object, so the call can be chained</returns>
    <ExportAPI("fuzzygene_relaxation")>
    <RApiReturn(GetType(FuzzyGeneModel))>
    Public Function fuzzygene_relaxation(fuzzygene As fz.FuzzyGeneModel,
                                         Optional omega As Double = 0.5,
                                         Optional steps As Integer = 50,
                                         Optional tolerance As Double = 0.000001,
                                         Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If omega <= 0 OrElse omega > 1 Then
            Return RInternal.debug.stop("omega must be in (0, 1]", env)
        End If
        If steps < 1 Then
            Return RInternal.debug.stop("steps must be positive", env)
        End If
        If tolerance <= 0 Then
            Return RInternal.debug.stop("tolerance must be positive", env)
        End If

        fuzzygene.Config.Relaxation = omega
        fuzzygene.Config.PropagationSteps = steps
        fuzzygene.Config.ConvergenceTolerance = tolerance

        Return fuzzygene
    End Function

    ''' <summary>把 R# 侧的扰动模式字符串解析成枚举。</summary>
    ''' <param name="text">模式名（大小写不敏感，支持 ko / kd / oe 简写）</param>
    ''' <param name="mode">解析结果</param>
    ''' <returns>是否识别成功</returns>
    Private Function parseInterventionMode(text As String, ByRef mode As InterventionMode) As Boolean
        Select Case If(text, "knockout").Trim().ToLowerInvariant()
            Case "knockout", "ko"
                mode = InterventionMode.Knockout
            Case "knockdown", "kd"
                mode = InterventionMode.Knockdown
            Case "overexpression", "overexpress", "oe"
                mode = InterventionMode.Overexpression
            Case Else
                Return False
        End Select

        Return True
    End Function

    ''' <summary>求解诊断表：迭代步 → 该步的最大变化量。</summary>
    Private Function convergenceTable(trace As fzdata.SteadyStateTrace) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }
        Dim n As Integer = If(trace Is Nothing, 0, trace.MaxDeltas.Count)
        Dim iteration(n - 1) As Integer
        Dim delta(n - 1) As Double

        For i As Integer = 0 To n - 1
            iteration(i) = i + 1
            delta(i) = trace.MaxDeltas(i)
        Next

        Call table.add("iteration", iteration)
        Call table.add("max_delta", delta)

        Return table
    End Function

    ''' <summary>终态表达变化表（与 ``knockouts`` 的输出列语义一致）。</summary>
    Private Function changesTable(result As InterventionResult) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If result Is Nothing Then
            Return table
        End If

        Call table.add("gene", result.GeneNames)
        Call table.add("fold_change", result.FoldChanges)
        Call table.add("percent_change", result.PercentChanges)
        Call table.add("z_score", result.ZScores)
        Call table.add("significant", result.IsSignificant)

        Return table
    End Function

    ''' <summary>本次扰动的单行摘要（扰动定义 + 求解参数 + 收敛状态）。</summary>
    Private Function summaryTable(trace As fzpert.PerturbationTrace, gene As String) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }
        Dim mutant As fzdata.SteadyStateTrace = trace.Mutant

        Call table.add("gene", New String() {gene})
        Call table.add("mode", New String() {trace.Mode.ToString()})
        Call table.add("clamp_value", New Double() {trace.ClampValue})
        Call table.add("iterations", New Integer() {mutant.Iterations})
        Call table.add("converged", New Boolean() {mutant.Converged})
        Call table.add("omega", New Double() {mutant.Relaxation})
        Call table.add("tolerance", New Double() {mutant.Tolerance})
        Call table.add("lagged", New Boolean() {mutant.Lagged})
        Call table.add("elapsed_ms", New Double() {trace.ElapsedMs})

        Return table
    End Function

#End Region

#Region "多运行自检与剂量分析"

    ''' <summary>
    ''' Model self-check: omega invariance, step invariance and lag ordering robustness.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Returns a named list:
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>``omega``: data.frame ``value`` / ``converged`` / ``iterations`` /
    ''' ``last_max_delta`` / ``max_abs_deviation`` / ``max_relative_deviation``; the deviation
    ''' columns are measured against ``reference_omega`` (see ``summary``);
    ''' </description></item>
    ''' <item><description>``steps``: the same shape for the iteration budget sweep, measured
    ''' against the largest step count;</description></item>
    ''' <item><description>``lag``: wide table ``gene`` + one column per omega, each cell is the
    ''' frame index at which that gene first reaches 10% (``lagRatio``) of its own final change;</description></item>
    ''' <item><description>``spearman``: wide table of pairwise Spearman rho between the omega runs,
    ''' computed on the genes that respond under <b>every</b> omega;</description></item>
    ''' <item><description>``summary``: one row data.frame with the two invariant flags, the
    ''' reference values, the responding gene count, the mean rho and the run count.</description></item>
    ''' </list>
    ''' <para>
    ''' Read ``summary$omega_invariant`` first. ``FALSE`` means the end state still depends on the
    ''' damping factor, i.e. the iteration has not converged and the trajectory is not a steady
    ''' state. That is a property of the model, not a bug of this function.
    ''' </para>
    ''' <para>
    ''' Cost: about ``2 x (length(omegas) + length(stepsList) + length(omegas))`` solves. With the
    ''' default grids on an 18k gene model this takes roughly one minute.
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="gene">the target gene of the virtual perturbation</param>
    ''' <param name="mode">``knockout`` (default), ``knockdown`` or ``overexpression``</param>
    ''' <param name="omegas">relaxation values to sweep, e.g. ``c(0.2, 0.5, 0.8, 1.0)``</param>
    ''' <param name="stepsList">iteration budgets to sweep, e.g. ``c(50, 200, 500)``</param>
    ''' <param name="sweepSteps">fixed iteration budget used by the omega sweep and the lag analysis</param>
    ''' <param name="referenceOmega">omega used as the comparison baseline; ``NaN`` picks the value closest to 0.5</param>
    ''' <param name="tolerance">relative deviation threshold of the invariant flags</param>
    ''' <param name="lagRatio">relative trigger threshold of the lag definition</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a named list of data.frames</returns>
    <ExportAPI("fuzzygene_sensitivity")>
    <RApiReturn(GetType(list))>
    Public Function fuzzygene_sensitivity(fuzzygene As fz.FuzzyGeneModel,
                                          gene As String,
                                          Optional mode As String = "knockout",
                                          Optional omegas As Double() = Nothing,
                                          Optional stepsList As Double() = Nothing,
                                          Optional sweepSteps As Integer = 50,
                                          Optional referenceOmega As Double = Double.NaN,
                                          Optional tolerance As Double = 0.001,
                                          Optional lagRatio As Double = 0.1,
                                          Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If String.IsNullOrEmpty(gene) Then
            Return RInternal.debug.stop("a target gene is required", env)
        End If

        Dim intervention As InterventionMode

        If Not parseInterventionMode(mode, intervention) Then
            Return RInternal.debug.stop($"unknown perturbation mode '{mode}', expected knockout / knockdown / overexpression", env)
        End If

        Dim budget As Integer() = toIntegerArray(stepsList)
        Dim omegaSweep As fzana.OmegaSweepResult
        Dim stepSweep As fzana.StepSweepResult
        Dim lag As fzana.LagRobustnessResult

        Try
            omegaSweep = fuzzygene.SweepOmega(gene, intervention, omegas, sweepSteps, referenceOmega, tolerance)
            stepSweep = fuzzygene.SweepSteps(gene, intervention, budget, Double.NaN, tolerance)
            lag = fuzzygene.AnalyzeLag(gene, intervention, omegas, sweepSteps, lagRatio)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"omega", omegaTable(omegaSweep)},
                {"steps", stepTable(stepSweep)},
                {"lag", lagTable(lag)},
                {"spearman", spearmanTable(lag)},
                {"summary", sensitivitySummary(omegaSweep, stepSweep, lag)}
            }
        }
    End Function

    ''' <summary>
    ''' Dose-response curve of one virtual perturbation.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Dose <c>d</c> runs from 0 (pin the gene at its baseline) to 1 (full knockout); the clamped
    ''' value is <c>baseline x (1 - d)</c>. Note that <c>d = 0</c> is <b>not</b> the same as "no
    ''' perturbation": the gene is held fixed while the wildtype trajectory drifts away from the
    ''' baseline, which is why its own fold change is 0 at <c>d = 0</c> but other genes can still
    ''' move.
    ''' </para>
    ''' <para>
    ''' Only the top genes are returned (ranked by ``|z|`` at the largest dose) to keep the
    ''' data.frame small; pass ``top = 0`` for all 18k genes.
    ''' </para>
    ''' <code>
    ''' let r = FuzzyGene::fuzzygene_dose_response(model, "TPPP3", doses = c(0, 0.5, 1));
    ''' print(r);
    ''' </code>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="gene">the target gene of the virtual perturbation</param>
    ''' <param name="doses">dose points in [0, 1]; ``NULL`` uses 5 evenly spaced points</param>
    ''' <param name="steps">iteration budget of each solve</param>
    ''' <param name="top">number of genes to return; 0 means all</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a long data.frame ``dose`` / ``clamp_value`` / ``gene`` / ``fold_change`` / ``percent_change`` / ``z_score`` / ``significant``</returns>
    <ExportAPI("fuzzygene_dose_response")>
    <RApiReturn(GetType(dataframe))>
    Public Function fuzzygene_dose_response(fuzzygene As fz.FuzzyGeneModel,
                                            gene As String,
                                            Optional doses As Double() = Nothing,
                                            Optional steps As Integer = 60,
                                            Optional top As Integer = 50,
                                            Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If String.IsNullOrEmpty(gene) Then
            Return RInternal.debug.stop("a target gene is required", env)
        End If

        Dim result As fzana.DoseResponseResult

        Try
            result = fuzzygene.AnalyzeDose(gene, doses, steps, Double.NaN, top)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return doseTable(result)
    End Function

    ''' <summary>
    ''' Dose x iteration response surface of one virtual perturbation.
    ''' </summary>
    ''' <remarks>
    ''' Every cell is <c>mutant(t) - control(t)</c> at the same frame, i.e. the drift of the system
    ''' itself has already been subtracted. Only the top genes (ranked by ``|z|`` at the largest
    ''' dose) are kept, so the cube stays small enough to plot as a contour or a 3D surface.
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="gene">the target gene of the virtual perturbation</param>
    ''' <param name="doses">dose points in [0, 1]; ``NULL`` uses 5 evenly spaced points</param>
    ''' <param name="steps">iteration budget of each solve</param>
    ''' <param name="top">number of genes to keep</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a long data.frame ``dose`` / ``iteration`` / ``gene`` / ``delta``</returns>
    <ExportAPI("fuzzygene_dose_time_surface")>
    <RApiReturn(GetType(dataframe))>
    Public Function fuzzygene_dose_time_surface(fuzzygene As fz.FuzzyGeneModel,
                                                gene As String,
                                                Optional doses As Double() = Nothing,
                                                Optional steps As Integer = 60,
                                                Optional top As Integer = 20,
                                                Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If String.IsNullOrEmpty(gene) Then
            Return RInternal.debug.stop("a target gene is required", env)
        End If

        Dim result As fzana.DoseTimeSurfaceResult

        Try
            result = fuzzygene.AnalyzeDoseTime(gene, doses, steps, Double.NaN, top)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return surfaceTable(result)
    End Function

    ''' <summary>
    ''' Non-additive (synergy / antagonism) analysis of a two gene combination.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Additive expectation is <c>control + (A - control) + (B - control)</c>; the non additive
    ''' effect is <c>joint - additive</c>. Positive means synergy, negative means antagonism.
    ''' </para>
    ''' <para>
    ''' Returns a named list:
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>``summary``: one row with the two genes and ``max_abs_non_additive``;</description></item>
    ''' <item><description>``table``: the full 18k gene table ``gene`` / ``single_a`` / ``single_b`` /
    ''' ``joint`` / ``additive`` / ``non_additive``;</description></item>
    ''' <item><description>``top``: the top genes ranked by ``|non_additive|``;</description></item>
    ''' <item><description>``trajectory``: the frame level version ``iteration`` / ``gene`` /
    ''' ``additive`` / ``joint`` / ``non_additive``, which shows whether the deviation appears
    ''' together with the single perturbations or only later.</description></item>
    ''' </list>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="geneA">the first target gene</param>
    ''' <param name="geneB">the second target gene; must differ from ``geneA``</param>
    ''' <param name="mode">``knockout`` (default), ``knockdown`` or ``overexpression``</param>
    ''' <param name="steps">iteration budget of each solve</param>
    ''' <param name="top">number of genes kept in ``top`` and ``trajectory``</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a named list of data.frames</returns>
    <ExportAPI("fuzzygene_combination")>
    <RApiReturn(GetType(list))>
    Public Function fuzzygene_combination(fuzzygene As fz.FuzzyGeneModel,
                                          geneA As String,
                                          geneB As String,
                                          Optional mode As String = "knockout",
                                          Optional steps As Integer = 60,
                                          Optional top As Integer = 50,
                                          Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If String.IsNullOrEmpty(geneA) OrElse String.IsNullOrEmpty(geneB) Then
            Return RInternal.debug.stop("two target genes are required", env)
        End If

        Dim intervention As InterventionMode

        If Not parseInterventionMode(mode, intervention) Then
            Return RInternal.debug.stop($"unknown perturbation mode '{mode}', expected knockout / knockdown / overexpression", env)
        End If

        Dim result As fzana.CombinationTraceResult

        Try
            result = fuzzygene.AnalyzeCombination(geneA, geneB, intervention, steps, Double.NaN, top)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"summary", combinationSummary(result)},
                {"table", combinationTable(result)},
                {"top", combinationTop(result)},
                {"trajectory", combinationTrajectory(result)}
            }
        }
    End Function

    ''' <summary>
    ''' Run every analysis once and write the tables to a folder.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Writes into the folder:
    ''' ``analysis.sensitivity.csv`` (omega + step sweeps),
    ''' ``analysis.lag.csv``, ``analysis.spearman.csv``,
    ''' ``analysis.dose_response.csv``, ``analysis.dose_time_surface.csv``,
    ''' ``analysis.combination.csv`` (when both genes are given) and
    ''' ``analysis.report.json`` (grid definition, invariant flags, run count, elapsed time).
    ''' </para>
    ''' <para>
    ''' This is the cheap way to get everything: calling the four analysis apis separately would
    ''' repeat the dose sweep twice. Expect one to two minutes on an 18k gene model with the
    ''' default grids.
    ''' </para>
    ''' <code>
    ''' let files = FuzzyGene::fuzzygene_analysis_report(
    '''     model, "TPPP3", geneA = "HSD17B3", geneB = "TPPP3", dir = "/path/analysis");
    ''' print(files);
    ''' </code>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="gene">the target gene of the single perturbation analyses</param>
    ''' <param name="geneA">the first gene of the combination analysis; empty skips that part</param>
    ''' <param name="geneB">the second gene of the combination analysis; empty skips that part</param>
    ''' <param name="dir">the output folder (created when missing)</param>
    ''' <param name="mode">``knockout`` (default), ``knockdown`` or ``overexpression``</param>
    ''' <param name="omegas">relaxation grid; ``NULL`` uses ``c(0.2, 0.5, 0.8, 1.0)``</param>
    ''' <param name="stepsList">step grid of the invariance check; ``NULL`` uses ``c(50, 200, 500)``</param>
    ''' <param name="doses">dose grid; ``NULL`` uses 5 evenly spaced points</param>
    ''' <param name="sweepSteps">fixed iteration budget of the omega / lag analyses</param>
    ''' <param name="doseSteps">iteration budget of the dose analyses</param>
    ''' <param name="combinationSteps">iteration budget of the combination analysis</param>
    ''' <param name="top">number of genes kept in the dose and combination tables</param>
    ''' <param name="surfaceTop">number of genes kept in the dose-time surface</param>
    ''' <param name="lagRatio">relative trigger threshold of the lag definition</param>
    ''' <param name="tolerance">relative deviation threshold of the invariant flags</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a character vector of the written file paths</returns>
    <ExportAPI("fuzzygene_analysis_report")>
    <RApiReturn(TypeCodes.string)>
    Public Function fuzzygene_analysis_report(fuzzygene As fz.FuzzyGeneModel,
                                              gene As String,
                                              Optional geneA As String = Nothing,
                                              Optional geneB As String = Nothing,
                                              Optional dir As String = Nothing,
                                              Optional mode As String = "knockout",
                                              Optional omegas As Double() = Nothing,
                                              Optional stepsList As Double() = Nothing,
                                              Optional doses As Double() = Nothing,
                                              Optional sweepSteps As Integer = 50,
                                              Optional doseSteps As Integer = 60,
                                              Optional combinationSteps As Integer = 60,
                                              Optional top As Integer = 50,
                                              Optional surfaceTop As Integer = 20,
                                              Optional lagRatio As Double = 0.1,
                                              Optional tolerance As Double = 0.001,
                                              Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If String.IsNullOrEmpty(gene) Then
            Return RInternal.debug.stop("a target gene is required", env)
        End If
        If String.IsNullOrWhiteSpace(dir) Then
            Return RInternal.debug.stop("the output folder path is empty", env)
        End If

        Dim intervention As InterventionMode

        If Not parseInterventionMode(mode, intervention) Then
            Return RInternal.debug.stop($"unknown perturbation mode '{mode}', expected knockout / knockdown / overexpression", env)
        End If

        Dim options As New fzana.AnalysisOptions With {
            .Mode = intervention,
            .Omegas = omegas,
            .StepsList = toIntegerArray(stepsList),
            .SweepSteps = sweepSteps,
            .Doses = doses,
            .DoseSteps = doseSteps,
            .CombinationSteps = combinationSteps,
            .TopGenes = top,
            .SurfaceTopGenes = surfaceTop,
            .LagThresholdRatio = lagRatio,
            .InvariantTolerance = tolerance
        }

        Try
            Return fuzzygene.ExportAnalysisReport(dir, gene, geneA, geneB, options)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>R# 侧传进来的是 numeric vector，转成整数步数表。</summary>
    Private Function toIntegerArray(values As Double()) As Integer()
        If values Is Nothing OrElse values.Length = 0 Then
            Return Nothing
        End If

        Return values _
            .Where(Function(v) Not Double.IsNaN(v)) _
            .Select(Function(v) CInt(v)) _
            .ToArray()
    End Function

    ''' <summary>ω 扫描结果 → data.frame。</summary>
    Private Function omegaTable(result As fzana.OmegaSweepResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call table.add("value", result.Omegas)
        Call table.add("converged", result.Converged)
        Call table.add("iterations", result.Iterations)
        Call table.add("last_max_delta", result.LastMaxDelta)
        Call table.add("max_abs_deviation", result.MaxAbsDeviation)
        Call table.add("max_relative_deviation", result.MaxRelativeDeviation)

        Return table
    End Function

    ''' <summary>步数扫描结果 → data.frame。</summary>
    Private Function stepTable(result As fzana.StepSweepResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call table.add("value", result.StepsList)
        Call table.add("converged", result.Converged)
        Call table.add("iterations", result.ActualIterations)
        Call table.add("last_max_delta", result.LastMaxDelta)
        Call table.add("max_abs_deviation", result.MaxAbsDeviation)
        Call table.add("max_relative_deviation", result.MaxRelativeDeviation)

        Return table
    End Function

    ''' <summary>lag 矩阵 → 宽表（行 = 基因，列 = ω）。</summary>
    Private Function lagTable(result As fzana.LagRobustnessResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim genes As Integer = result.GeneNames.Length

        Call table.add("gene", result.GeneNames)

        For k As Integer = 0 To result.Omegas.Length - 1
            Dim values(genes - 1) As Double

            For j As Integer = 0 To genes - 1
                values(j) = result.Lag(k, j)
            Next

            Call table.add($"omega_{result.Omegas(k)}", values)
        Next

        Return table
    End Function

    ''' <summary>Spearman 矩阵 → 宽表。</summary>
    Private Function spearmanTable(result As fzana.LagRobustnessResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim n As Integer = result.Omegas.Length

        Call table.add("omega", result.Omegas)

        For k As Integer = 0 To n - 1
            Dim values(n - 1) As Double

            For j As Integer = 0 To n - 1
                values(j) = result.Spearman(k, j)
            Next

            Call table.add($"omega_{result.Omegas(k)}", values)
        Next

        Return table
    End Function

    ''' <summary>三项自检的单行摘要。</summary>
    Private Function sensitivitySummary(omega As fzana.OmegaSweepResult,
                                        steps As fzana.StepSweepResult,
                                        lag As fzana.LagRobustnessResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call table.add("omega_invariant", New Boolean() {omega.Invariant})
        Call table.add("reference_omega", New Double() {omega.ReferenceOmega})
        Call table.add("omega_runs", New Integer() {omega.Runs})
        Call table.add("steps_invariant", New Boolean() {steps.Invariant})
        Call table.add("reference_steps", New Integer() {steps.ReferenceSteps})
        Call table.add("step_runs", New Integer() {steps.Runs})
        Call table.add("responding_genes", New Integer() {lag.GeneNames.Length})
        Call table.add("mean_rho", New Double() {lag.MeanRho})
        Call table.add("lag_runs", New Integer() {lag.Runs})
        Call table.add("elapsed_ms", New Double() {omega.ElapsedMs + steps.ElapsedMs + lag.ElapsedMs})

        Return table
    End Function

    ''' <summary>剂量-响应 → 长表（只保留 Top-K 基因）。</summary>
    Private Function doseTable(result As fzana.DoseResponseResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim genes As Integer() = result.TopGenes
        Dim count As Integer = result.Doses.Length * genes.Length
        Dim doses(count - 1) As Double
        Dim clamps(count - 1) As Double
        Dim names(count - 1) As String
        Dim fold(count - 1) As Double
        Dim percent(count - 1) As Double
        Dim zscores(count - 1) As Double
        Dim significant(count - 1) As Boolean
        Dim cursor As Integer = 0

        For d As Integer = 0 To result.Doses.Length - 1
            For j As Integer = 0 To genes.Length - 1
                doses(cursor) = result.Doses(d)
                clamps(cursor) = result.ClampValues(d)
                names(cursor) = result.GeneNames(genes(j))
                fold(cursor) = result.FoldChanges(d, genes(j))
                percent(cursor) = result.PercentChanges(d, genes(j))
                zscores(cursor) = result.ZScores(d, genes(j))
                significant(cursor) = result.Significant(d, genes(j))
                cursor += 1
            Next
        Next

        Call table.add("dose", doses)
        Call table.add("clamp_value", clamps)
        Call table.add("gene", names)
        Call table.add("fold_change", fold)
        Call table.add("percent_change", percent)
        Call table.add("z_score", zscores)
        Call table.add("significant", significant)

        Return table
    End Function

    ''' <summary>剂量-时间曲面 → 长表。</summary>
    Private Function surfaceTable(result As fzana.DoseTimeSurfaceResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim doses As Integer = result.Doses.Length
        Dim genes As Integer = result.GeneNames.Length
        Dim count As Integer = doses * result.NFrames * genes
        Dim doseColumn(count - 1) As Double
        Dim iteration(count - 1) As Integer
        Dim names(count - 1) As String
        Dim deltas(count - 1) As Double
        Dim cursor As Integer = 0

        For d As Integer = 0 To doses - 1
            For f As Integer = 0 To result.NFrames - 1
                For j As Integer = 0 To genes - 1
                    doseColumn(cursor) = result.Doses(d)
                    iteration(cursor) = f
                    names(cursor) = result.GeneNames(j)
                    deltas(cursor) = result.Deltas(d, f, j)
                    cursor += 1
                Next
            Next
        Next

        Call table.add("dose", doseColumn)
        Call table.add("iteration", iteration)
        Call table.add("gene", names)
        Call table.add("delta", deltas)

        Return table
    End Function

    ''' <summary>组合分析的单行摘要。</summary>
    Private Function combinationSummary(result As fzana.CombinationTraceResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call table.add("gene_a", New String() {result.GeneA})
        Call table.add("gene_b", New String() {result.GeneB})
        Call table.add("max_abs_non_additive", New Double() {result.MaxAbsNonAdditive})
        Call table.add("runs", New Integer() {result.Runs})
        Call table.add("elapsed_ms", New Double() {result.ElapsedMs})

        Return table
    End Function

    ''' <summary>组合分析的全基因表。</summary>
    Private Function combinationTable(result As fzana.CombinationTraceResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call table.add("gene", result.GeneNames)
        Call table.add("single_a", result.SingleAMeans)
        Call table.add("single_b", result.SingleBMeans)
        Call table.add("joint", result.JointMeans)
        Call table.add("additive", result.AdditiveMeans)
        Call table.add("non_additive", result.NonAdditive)

        Return table
    End Function

    ''' <summary>组合分析的非加性 Top-K 表。</summary>
    Private Function combinationTop(result As fzana.CombinationTraceResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim genes As Integer() = result.TopGenes
        Dim names(genes.Length - 1) As String
        Dim values(genes.Length - 1) As Double

        For j As Integer = 0 To genes.Length - 1
            names(j) = result.GeneNames(genes(j))
            values(j) = result.NonAdditive(genes(j))
        Next

        Call table.add("gene", names)
        Call table.add("non_additive", values)

        Return table
    End Function

    ''' <summary>组合分析的逐帧非加性长表。</summary>
    Private Function combinationTrajectory(result As fzana.CombinationTraceResult) As dataframe
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim trajectory As fzana.CombinationTrajectory = result.Trajectory

        If trajectory Is Nothing Then
            Return table
        End If

        Dim genes As Integer = trajectory.GeneNames.Length
        Dim count As Integer = trajectory.NFrames * genes
        Dim iteration(count - 1) As Integer
        Dim names(count - 1) As String
        Dim additive(count - 1) As Double
        Dim joint(count - 1) As Double
        Dim nonAdditive(count - 1) As Double
        Dim cursor As Integer = 0

        For t As Integer = 0 To trajectory.NFrames - 1
            For j As Integer = 0 To genes - 1
                iteration(cursor) = t
                names(cursor) = trajectory.GeneNames(j)
                additive(cursor) = trajectory.Additive(t, j)
                joint(cursor) = trajectory.Joint(t, j)
                nonAdditive(cursor) = trajectory.NonAdditive(t, j)
                cursor += 1
            Next
        Next

        Call table.add("iteration", iteration)
        Call table.add("gene", names)
        Call table.add("additive", additive)
        Call table.add("joint", joint)
        Call table.add("non_additive", nonAdditive)

        Return table
    End Function

#End Region

End Module
