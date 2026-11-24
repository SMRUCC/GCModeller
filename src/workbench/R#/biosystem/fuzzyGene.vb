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
''' ``fuzzygene_hybrid_config``: chainable mutator of the solver (Newton / Anderson / legacy /
''' advance) and the hybrid layers (PBN regularisation, ensemble samples, FCM warm start,
''' spectral radius calibration);
''' </description></item>
''' <item><description>
''' ``fuzzygene_pbn`` / ``fuzzygene_ensemble``: the qualitative probabilistic boolean layer
''' (CoD table, selected rules) and the structural uncertainty of a perturbation (mean, standard
''' deviation, confidence interval, sign agreement, boolean attractor periods);
''' </description></item>
''' <item><description>
''' ``fuzzygene_modules``: the coarse grained fuzzy cognitive map layer (module assignment,
''' bounded module steady state, per gene warm start and surrogate);
''' </description></item>
''' <item><description>
''' ``fuzzygene_solver_report``: exit reason, free residual, constraint violation, saturation
''' fractions and attractor period of a single perturbation, which separates "converged" from
''' "ran out of steps" / "oscillates" / "hit the support bounds";
''' </description></item>
''' <item><description>
''' ``fuzzygene_relaxation``: change the damping factor / iteration budget of the damped
''' fallback, which is what you want when a genome scale model keeps oscillating instead of
''' converging;
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
    ''' <param name="x">
    ''' the gene expression matrix: either the ``Matrix`` object returned by
    ''' ``geneExpression::load.expr`` (rows = genes, columns = samples) or an already converted
    ''' ``GeneExpressionData``. Both are accepted because the plain ``load.expr`` result is a
    ''' ``Matrix``; the old signature only took ``GeneExpressionData``, so every script that piped
    ''' ``load.expr()`` straight into ``build_fuzzygene`` failed with an InvalidCastException.
    ''' </param>
    ''' <param name="prior">
    ''' an optional prior regulatory network created by the ``bnlearn`` package (``prior_edges``);
    ''' when omitted the candidate regulators are selected by correlation only.
    ''' </param>
    ''' <param name="config">
    ''' the configuration created by ``FuzzyGene::fuzzygene_config``; ``Nothing`` uses the defaults.
    ''' </param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>an untrained model object</returns>
    <ExportAPI("build_fuzzygene")>
    <RApiReturn(GetType(FuzzyGeneModel))>
    Public Function build_fuzzygene(x As Object,
                                    Optional prior As PriorNetwork = Nothing,
                                    Optional config As fzcfg.FuzzyGeneConfig = Nothing,
                                    Optional env As Environment = Nothing) As Object

        If x Is Nothing Then
            Return RInternal.debug.stop("a gene expression matrix is required", env)
        End If

        Dim expression As GeneExpressionData

        If TypeOf x Is GeneExpressionData Then
            expression = DirectCast(x, GeneExpressionData)
        ElseIf TypeOf x Is SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix Then
            ' 必须全限定：本模块同时 import 了 HTS 的 Matrix 与 R# 运行时的 Matrix，
            ' 直接写 Matrix 会报 BC30561 二义性
            Try
                expression = HTSDataFrame.ReadGeneExpressionMatrix(
                    DirectCast(x, SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix))
            Catch ex As Exception
                Return RInternal.debug.stop("failed to convert the expression matrix: " & ex.Message, env)
            End Try
        Else
            Return RInternal.debug.stop(
                $"unsupported expression input type '{x.GetType().FullName}', expected a Matrix or GeneExpressionData", env)
        End If

        Call "start to build fuzzy gene network!".debug

        Return New fz.FuzzyGeneModel(expression, prior, config)
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
    ''' The column index is a <b>solver iteration</b>, not biological time. The default solver
    ''' finds the constrained fixed point of ``F(z) - z = 0`` (Levenberg-Marquardt, see
    ''' ``fuzzygene_hybrid_config(solver = ...)``), so the frames are Newton steps and the number of
    ''' frames is small (single digit for a well posed network). Only when the solver falls back to
    ''' the damped iteration does ``omega`` shape the curve (``fuzzygene_relaxation``), and only a
    ''' lagged (time series) model in ``advance`` mode makes one frame equal to one sampling interval.
    ''' </para>
    ''' <para>
    ''' Read ``convergence`` / ``summary$converged`` before interpreting the curve: a flat tail
    ''' produced by running out of ``steps`` is a truncated plateau, not a steady state.
    ''' </para>
    ''' <para>
    ''' ``wildtype`` is the control run: it starts from the same baseline and is iterated to its own
    ''' fixed point, so ``foldchange`` is now "mutant fixed point minus wildtype fixed point" instead
    ''' of "mutant minus the training mean". The training mean is generally <b>not</b> a fixed point
    ''' of a nonlinear fuzzy map, so the previous definition mixed the perturbation effect with the
    ''' model's own drift; the wildtype trajectory still exposes that drift, which is worth checking
    ''' (a large drift means the fit is inconsistent with its own steady state).
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
    ''' These three values belong to the <b>damped fallback</b> path
    ''' (<c>z &lt;- z + omega * (F(z) - z)</c>). The default solver is a Levenberg-Marquardt root
    ''' finder for <c>F(z) - z = 0</c>, and it is used whenever it can reduce the residual; the
    ''' damping factor only matters when it falls back (or when
    ''' ``solver = "legacy"`` / ``"anderson"`` is selected through
    ''' ``fuzzygene_hybrid_config``). With ``omega = 1`` the fallback is the plain repeated
    ''' application of <c>F</c>, which on a genome scale model often keeps oscillating instead of
    ''' converging; ``omega = 0.5`` is the usual compromise.
    ''' </para>
    ''' <para>
    ''' Always rerun the trajectory after changing these values, and check
    ''' ``summary$exit_reason``: only ``Converged`` means the flat tail is a real steady state;
    ''' ``MaxIterations`` is a truncated plateau and ``LimitCycle`` means the system oscillates.
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
        ' 退出原因与两个残差是判断「平尾是截断还是真稳态」的唯一依据：
        ' 只有 exit_reason = Converged 才算稳态；clamp_residual 恒大于 0，不能用来判收敛
        Call table.add("exit_reason", New String() {mutant.ExitReason.ToString()})
        Call table.add("free_residual", New Double() {mutant.FreeResidual})
        Call table.add("clamp_residual", New Double() {mutant.ClampResidual})
        Call table.add("out_of_support", New Double() {mutant.OutOfSupportFraction})
        Call table.add("pinned", New Double() {mutant.PinnedFraction})
        Call table.add("attractor_period", New Integer() {mutant.AttractorPeriod})
        ' 终态是怎么稳定下来的：fixed point / limit cycle / fcm surrogate / tail mean
        Call table.add("stabilized_by", New String() {If(mutant.StabilizedBy, "")})
        Call table.add("control_exit_reason", New String() {If(trace.Control Is Nothing, "", trace.Control.ExitReason.ToString())})
        Call table.add("omega", New Double() {mutant.Relaxation})
        Call table.add("tolerance", New Double() {mutant.Tolerance})
        Call table.add("lagged", New Boolean() {mutant.Lagged})
        Call table.add("elapsed_ms", New Double() {trace.ElapsedMs})

        Return table
    End Function

#End Region

#Region "PBN / 集成采样 / FCM / 求解器诊断"

    ''' <summary>
    ''' Chainable mutator of the solver and hybrid layer options.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The existing ``fuzzygene_config`` already carries 30 optional arguments; adding the
    ''' solver / PBN / FCM knobs there would make it unusable and would break every script that
    ''' relies on positional arguments. This function mutates a configuration object in place
    ''' instead, so the whole thing stays readable:
    ''' </para>
    ''' <code>
    ''' let opts = FuzzyGene::fuzzygene_config(maxRegulators = 6, mfCount = 3);
    ''' opts = FuzzyGene::fuzzygene_hybrid_config(opts,
    '''     solver = "newton", pbnRegularization = "weighted", ensembleSamples = 16);
    ''' </code>
    ''' <para>
    ''' Defaults are chosen so that calling this function with only ``config`` changes nothing
    ''' important: the Newton root finder is already the default solver, and the stability
    ''' calibration stays off (the root finder does not need a contractive map, while calibrating
    ''' a strongly cyclic fit can drop R2 from 0.86 to 0.014 — that has to be an explicit choice).
    ''' </para>
    ''' </remarks>
    ''' <param name="config">the configuration created by ``fuzzygene_config``</param>
    ''' <param name="solver">
    ''' ``newton`` (default), ``anderson``, ``legacy`` (the old damped iteration) or ``advance``
    ''' (lagged/time series only: plain time stepping, no steady state claim)
    ''' </param>
    ''' <param name="calibration">
    ''' ``none`` (default), ``spectral`` (scale the cyclic part until the spectral radius of the
    ''' assembled Jacobian is below ``1 - epsilon``) or ``row-l1`` (the conservative row norm bound)
    ''' </param>
    ''' <param name="calibrationEpsilon">target margin ``1 - epsilon`` of the spectral radius. Default 0.05.</param>
    ''' <param name="calibrationMinScale">lower bound of one calibration pass, keeps the model from collapsing. Default 0.05.</param>
    ''' <param name="clampInputToSupport">
    ''' clamp the antecedent inputs to the training domain of each regulator. Turning this off
    ''' re-enables the unbounded extrapolation of the outermost rules. Default TRUE.
    ''' </param>
    ''' <param name="clampOutputToSupport">clamp the predicted expression to the training support. Default TRUE.</param>
    ''' <param name="maxRegulatorCollinearity">
    ''' upper bound of the pairwise |Pearson| between the selected regulators; 1 disables the
    ''' constraint (default). Lowering it to 0.95 improves the conditioning of the least squares
    ''' step at the price of selecting fewer data driven regulators.
    ''' </param>
    ''' <param name="pbnRegularization">
    ''' ``none`` (default), ``filter`` (drop correlation candidates whose CoD is too low) or
    ''' ``weighted`` (order the candidates by CoD). Enabling it also makes
    ''' ``fuzzygene_ensemble`` available.
    ''' </param>
    ''' <param name="pbnMinCod">minimum CoD for a candidate parent set. Default 0.05.</param>
    ''' <param name="pbnWeight">weight of the CoD term when ``pbnRegularization = "weighted"``. Default 1.</param>
    ''' <param name="pbnMaxParents">maximum parents enumerated per target gene. Default 2.</param>
    ''' <param name="pbnMaxGenes">gene limit of the qualitative layer; above it the PBN layer is skipped. Default 2000.</param>
    ''' <param name="pbnCandidateSize">candidate pool size per target gene (prior TFs plus the top correlated genes). Default 5.</param>
    ''' <param name="ensembleSamples">
    ''' default sample count of ``fuzzygene_ensemble``: how many times the PBN rules are resampled.
    ''' 0 (default) keeps the deterministic single solve.
    ''' </param>
    ''' <param name="modeBiasWeight">
    ''' how strongly a sampled boolean mode pulls the continuous solve; 0 (default) only reports
    ''' the boolean consistency without changing the numerical result.
    ''' </param>
    ''' <param name="fcmWarmStart">
    ''' enable the module level fuzzy cognitive map: it provides a bounded warm start for the
    ''' per gene root finding and a surrogate answer when the solver detects a limit cycle.
    ''' Default FALSE.
    ''' </param>
    ''' <param name="fcmModules">number of modules of the coarse grained layer. Default 48.</param>
    ''' <param name="fcmAlpha">slope of the sigmoid activation. Default 1.</param>
    ''' <param name="fcmMaxSteps">propagation budget of the module layer. Default 200.</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>the same configuration object, so the call can be chained</returns>
    <ExportAPI("fuzzygene_hybrid_config")>
    <RApiReturn(GetType(FuzzyGeneConfig))>
    Public Function fuzzygene_hybrid_config(config As fzcfg.FuzzyGeneConfig,
                                            Optional solver As String = "newton",
                                            Optional maxNewtonIterations As Integer = 20,
                                            Optional andersonDepth As Integer = 5,
                                            Optional calibration As String = "none",
                                            Optional calibrationEpsilon As Double = 0.05,
                                            Optional calibrationMinScale As Double = 0.05,
                                            Optional clampInputToSupport As Boolean = True,
                                            Optional clampOutputToSupport As Boolean = True,
                                            Optional maxRegulatorCollinearity As Double = 1.0,
                                            Optional pbnRegularization As String = "none",
                                            Optional pbnMinCod As Double = 0.05,
                                            Optional pbnWeight As Double = 1.0,
                                            Optional pbnMaxParents As Integer = 2,
                                            Optional pbnMaxGenes As Integer = 2000,
                                            Optional pbnCandidateSize As Integer = 5,
                                            Optional ensembleSamples As Integer = 0,
                                            Optional modeBiasWeight As Double = 0.0,
                                            Optional fcmWarmStart As Boolean = False,
                                            Optional fcmModules As Integer = 48,
                                            Optional fcmAlpha As Double = 1.0,
                                            Optional fcmMaxSteps As Integer = 200,
                                            Optional env As Environment = Nothing) As Object
        If config Is Nothing Then
            Return RInternal.debug.stop("a fuzzygene configuration object is required", env)
        End If

        Dim solverMode As fzcfg.SolverMode

        If Not parseSolverMode(solver, solverMode) Then
            Return RInternal.debug.stop($"unknown solver '{solver}', expected newton / anderson / legacy / advance", env)
        End If

        Dim calibrationMode As fzcfg.StabilityCalibrationMode

        If Not parseCalibration(calibration, calibrationMode) Then
            Return RInternal.debug.stop($"unknown calibration '{calibration}', expected none / spectral / row-l1", env)
        End If

        Dim pbnMode As fzcfg.PbnRegularizationMode

        If Not parsePbnRegularization(pbnRegularization, pbnMode) Then
            Return RInternal.debug.stop($"unknown pbnRegularization '{pbnRegularization}', expected none / filter / weighted", env)
        End If

        ' 注意：Solver / Hybrid 是惰性新建的嵌套对象，必须先取出来再改，否则改的是副本
        Dim solverOptions As fzcfg.SolverOptions = config.Solver
        Dim hybridOptions As fzcfg.HybridOptions = config.Hybrid

        solverOptions.Mode = solverMode
        solverOptions.MaxNewtonIterations = maxNewtonIterations
        solverOptions.AndersonDepth = andersonDepth
        solverOptions.ClampInputToSupport = clampInputToSupport
        solverOptions.ClampOutputToSupport = clampOutputToSupport

        hybridOptions.Calibration = calibrationMode
        hybridOptions.CalibrationEpsilon = calibrationEpsilon
        hybridOptions.CalibrationMinScale = calibrationMinScale
        hybridOptions.MaxRegulatorCollinearity = maxRegulatorCollinearity
        hybridOptions.PbnRegularization = pbnMode
        hybridOptions.PbnMinCod = pbnMinCod
        hybridOptions.PbnWeight = pbnWeight
        hybridOptions.PbnMaxParents = pbnMaxParents
        hybridOptions.PbnMaxGenes = pbnMaxGenes
        hybridOptions.PbnCandidateSize = pbnCandidateSize
        hybridOptions.EnsembleSamples = ensembleSamples
        hybridOptions.ModeBiasWeight = modeBiasWeight
        hybridOptions.FcmWarmStart = fcmWarmStart
        hybridOptions.FcmModuleCount = fcmModules
        hybridOptions.FcmAlpha = fcmAlpha
        hybridOptions.FcmMaxSteps = fcmMaxSteps

        Try
            Return config.ValidateGene()
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    Private Function parseSolverMode(text As String, ByRef mode As fzcfg.SolverMode) As Boolean
        Select Case If(text, "newton").Trim().ToLowerInvariant()
            Case "newton", "lm", "root"
                mode = fzcfg.SolverMode.Newton
            Case "anderson", "anderson-damped", "damped"
                mode = fzcfg.SolverMode.AndersonDamped
            Case "legacy", "legacy-damped", "fixed"
                mode = fzcfg.SolverMode.LegacyDamped
            Case "advance", "time", "steps"
                mode = fzcfg.SolverMode.Advance
            Case "auto"
                mode = fzcfg.SolverMode.Auto
            Case Else
                Return False
        End Select

        Return True
    End Function

    Private Function parseCalibration(text As String, ByRef mode As fzcfg.StabilityCalibrationMode) As Boolean
        Select Case If(text, "none").Trim().ToLowerInvariant()
            Case "none", "off", "false"
                mode = fzcfg.StabilityCalibrationMode.None
            Case "spectral", "spectral-radius", "rho"
                mode = fzcfg.StabilityCalibrationMode.SpectralRadius
            Case "row-l1", "row", "l1"
                mode = fzcfg.StabilityCalibrationMode.RowL1
            Case Else
                Return False
        End Select

        Return True
    End Function

    Private Function parsePbnRegularization(text As String, ByRef mode As fzcfg.PbnRegularizationMode) As Boolean
        Select Case If(text, "none").Trim().ToLowerInvariant()
            Case "none", "off", "false"
                mode = fzcfg.PbnRegularizationMode.None
            Case "filter", "hard"
                mode = fzcfg.PbnRegularizationMode.Filter
            Case "weighted", "weight", "soft"
                mode = fzcfg.PbnRegularizationMode.Weighted
            Case Else
                Return False
        End Select

        Return True
    End Function

    ''' <summary>
    ''' The qualitative (probabilistic boolean) layer of a trained model: CoD table and rules.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Returns a named list:
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>
    ''' ``cod``: one row per (target, regulator) pair, columns ``target`` / ``regulator`` /
    ''' ``cod`` / ``rank``; ``cod`` is the coefficient of determination of that parent set on the
    ''' binarized matrix, i.e. how much of the boolean state of the target the regulator set
    ''' explains;
    ''' </description></item>
    ''' <item><description>
    ''' ``rules``: ``target`` / ``parents`` / ``n_parents`` / ``cod``, the selected rule of each
    ''' target gene (parents joined by ``+``);
    ''' </description></item>
    ''' <item><description>
    ''' ``targets``: ``target`` / ``rules`` / ``best_cod``, one row per analysed gene;
    ''' </description></item>
    ''' <item><description>
    ''' ``summary``: one row with ``has_pbn`` / ``targets_analysed`` / ``skipped`` / ``genes`` /
    ''' ``warnings``.
    ''' </description></item>
    ''' </list>
    ''' <para>
    ''' CoD is a **consistency** measure after binarization, not causal evidence: a rule can have
    ''' a high CoD simply because the target and the parent are both driven by a third gene.
    ''' Use it to suppress pure correlation edges and to report structure uncertainty, not as a
    ''' substitute for perturbation data.
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained model that was built with ``pbnRegularization != "none"``</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a named list of data.frames</returns>
    <ExportAPI("fuzzygene_pbn")>
    <RApiReturn(GetType(list))>
    Public Function fuzzygene_pbn(fuzzygene As fz.FuzzyGeneModel, Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Dim pbn As fz.Hybrid.PbnRegulatorPrior = fuzzygene.PbnPrior

        If pbn Is Nothing Then
            Return RInternal.debug.stop(
                "the PBN layer is not available: enable it before training with " &
                "FuzzyGene::fuzzygene_hybrid_config(config, pbnRegularization = 'weighted')", env)
        End If

        Dim codTarget As New List(Of String)
        Dim codRegulator As New List(Of String)
        Dim codValue As New List(Of Double)
        Dim codRank As New List(Of Integer)

        Dim ruleTarget As New List(Of String)
        Dim ruleParents As New List(Of String)
        Dim ruleSize As New List(Of Integer)
        Dim ruleCod As New List(Of Double)

        Dim targetName As New List(Of String)
        Dim targetRules As New List(Of Integer)
        Dim targetBest As New List(Of Double)

        For Each gene As String In pbn.GeneNames
            Dim rules As List(Of fz.Hybrid.PbnRule) = pbn.RulesOf(gene)

            If rules Is Nothing OrElse rules.Count = 0 Then
                Continue For
            End If

            Dim best As Double = 0.0

            For k As Integer = 0 To rules.Count - 1
                Dim rule As fz.Hybrid.PbnRule = rules(k)
                Dim names As New List(Of String)

                If rule.Parents IsNot Nothing Then
                    For Each parent As Integer In rule.Parents
                        If parent >= 0 AndAlso parent < pbn.GeneNames.Length Then
                            Call names.Add(pbn.GeneNames(parent))

                            Call codTarget.Add(gene)
                            Call codRegulator.Add(pbn.GeneNames(parent))
                            Call codValue.Add(rule.Cod)
                            Call codRank.Add(k + 1)
                        End If
                    Next
                End If

                If rule.Cod > best Then
                    best = rule.Cod
                End If

                Call ruleTarget.Add(gene)
                Call ruleParents.Add(String.Join("+", names))
                Call ruleSize.Add(names.Count)
                Call ruleCod.Add(rule.Cod)
            Next

            Call targetName.Add(gene)
            Call targetRules.Add(rules.Count)
            Call targetBest.Add(best)
        Next

        Dim codTable As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call codTable.add("target", codTarget.ToArray())
        Call codTable.add("regulator", codRegulator.ToArray())
        Call codTable.add("cod", codValue.ToArray())
        Call codTable.add("rank", codRank.ToArray())

        Dim ruleTable As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call ruleTable.add("target", ruleTarget.ToArray())
        Call ruleTable.add("parents", ruleParents.ToArray())
        Call ruleTable.add("n_parents", ruleSize.ToArray())
        Call ruleTable.add("cod", ruleCod.ToArray())

        Dim targetTable As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call targetTable.add("target", targetName.ToArray())
        Call targetTable.add("rules", targetRules.ToArray())
        Call targetTable.add("best_cod", targetBest.ToArray())

        Dim summary As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call summary.add("has_pbn", New Boolean() {True})
        Call summary.add("targets_analysed", New Integer() {pbn.TargetsAnalysed})
        Call summary.add("skipped", New Integer() {pbn.Skipped})
        Call summary.add("genes", New Integer() {pbn.GeneNames.Length})
        Call summary.add("warnings", New String() {String.Join(" | ", If(pbn.Warnings, New String() {}))})

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"cod", codTable},
                {"rules", ruleTable},
                {"targets", targetTable},
                {"summary", summary}
            }
        }
    End Function

    ''' <summary>
    ''' Ensemble (resampled boolean structure) prediction of virtual perturbations.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Each sample rebuilds the network by drawing one rule per target gene from the
    ''' probabilistic boolean network according to its CoD derived probability, and then solves
    ''' the continuous perturbation again. The spread of the resulting states therefore measures
    ''' **structural uncertainty** (which regulatory logic the data supports), not numerical noise.
    ''' </para>
    ''' <para>
    ''' Returns a named list:
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>``table``: one row per (perturbation, gene) with ``mean`` / ``sd`` /
    ''' ``lower`` (5%) / ``upper`` (95%) / ``sign_agreement`` / ``high_fraction`` / ``reference``;</description></item>
    ''' <item><description>``summary``: one row per perturbation with ``samples`` /
    ''' ``converged_fraction`` / ``degenerate`` / ``notes``;</description></item>
    ''' <item><description>``attractors``: the boolean attractor period distribution
    ''' (``perturbation`` / ``period`` / ``count``), zero rows when no cycle was detected.</description></item>
    ''' </list>
    ''' <para>
    ''' ``sign_agreement`` is the single most useful column: about 0.5 means the direction of that
    ''' gene's response is decided by the model noise, so it must not be reported as a finding.
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained model built with ``pbnRegularization != "none"``</param>
    ''' <param name="genes">the genes to perturb</param>
    ''' <param name="mode">``knockout`` (default), ``knockdown`` or ``overexpression``</param>
    ''' <param name="samples">how many PBN resamples; &lt;= 0 falls back to the configured value</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a named list of data.frames</returns>
    <ExportAPI("fuzzygene_ensemble")>
    <RApiReturn(GetType(list))>
    Public Function fuzzygene_ensemble(fuzzygene As fz.FuzzyGeneModel,
                                       genes As String(),
                                       Optional mode As String = "knockout",
                                       Optional samples As Integer = 16,
                                       Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If
        If genes Is Nothing OrElse genes.Length = 0 Then
            Return RInternal.debug.stop("at least one target gene is required", env)
        End If

        Dim intervention As InterventionMode

        If Not parseInterventionMode(mode, intervention) Then
            Return RInternal.debug.stop($"unknown perturbation mode '{mode}', expected knockout / knockdown / overexpression", env)
        End If

        Dim tableGene As New List(Of String)
        Dim tableTarget As New List(Of String)
        Dim tableMean As New List(Of Double)
        Dim tableSd As New List(Of Double)
        Dim tableLower As New List(Of Double)
        Dim tableUpper As New List(Of Double)
        Dim tableSign As New List(Of Double)
        Dim tableHigh As New List(Of Double)
        Dim tableReference As New List(Of Double)

        Dim summaryTarget As New List(Of String)
        Dim summarySamples As New List(Of Integer)
        Dim summaryConverged As New List(Of Double)
        Dim summaryDegenerate As New List(Of Boolean)
        Dim summaryNotes As New List(Of String)

        Dim attractorTarget As New List(Of String)
        Dim attractorPeriod As New List(Of Integer)
        Dim attractorCount As New List(Of Integer)

        For Each gene As String In genes
            Dim spec As New InterventionSpec With {
                .GeneName = gene,
                .GeneIndex = -1,
                .Mode = intervention
            }
            Dim result As fz.Hybrid.EnsemblePerturbationResult

            Try
                result = fuzzygene.PredictEnsemble(New InterventionSpec() {spec}, samples)
            Catch ex As InvalidOperationException
                Return RInternal.debug.stop(
                    $"ensemble prediction is not available: {ex.Message}. " &
                    "retrain with FuzzyGene::fuzzygene_hybrid_config(config, pbnRegularization = 'weighted')", env)
            Catch ex As Exception
                Return RInternal.debug.stop(ex.Message, env)
            End Try

            If result Is Nothing OrElse result.GeneNames Is Nothing Then
                Return RInternal.debug.stop($"the target gene '{gene}' is not modelled by this network", env)
            End If

            For i As Integer = 0 To result.GeneNames.Length - 1
                Call tableGene.Add(result.GeneNames(i))
                Call tableTarget.Add(gene)
                Call tableMean.Add(result.Mean(i))
                Call tableSd.Add(result.StandardDeviation(i))
                Call tableLower.Add(result.LowerBound(i))
                Call tableUpper.Add(result.UpperBound(i))
                Call tableSign.Add(result.SignAgreement(i))
                Call tableHigh.Add(result.BooleanHighFraction(i))
                Call tableReference.Add(result.Reference(i))
            Next

            Call summaryTarget.Add(gene)
            Call summarySamples.Add(result.Samples)
            Call summaryConverged.Add(result.ConvergedFraction)
            Call summaryDegenerate.Add(result.Degenerate)
            Call summaryNotes.Add(If(result.Notes, ""))

            For Each item As KeyValuePair(Of Integer, Integer) In result.AttractorPeriods
                Call attractorTarget.Add(gene)
                Call attractorPeriod.Add(item.Key)
                Call attractorCount.Add(item.Value)
            Next
        Next

        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call table.add("perturbation", tableTarget.ToArray())
        Call table.add("gene", tableGene.ToArray())
        Call table.add("mean", tableMean.ToArray())
        Call table.add("sd", tableSd.ToArray())
        Call table.add("lower", tableLower.ToArray())
        Call table.add("upper", tableUpper.ToArray())
        Call table.add("sign_agreement", tableSign.ToArray())
        Call table.add("high_fraction", tableHigh.ToArray())
        Call table.add("reference", tableReference.ToArray())

        Dim summary As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call summary.add("perturbation", summaryTarget.ToArray())
        Call summary.add("samples", summarySamples.ToArray())
        Call summary.add("converged_fraction", summaryConverged.ToArray())
        Call summary.add("degenerate", summaryDegenerate.ToArray())
        Call summary.add("notes", summaryNotes.ToArray())

        Dim attractors As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call attractors.add("perturbation", attractorTarget.ToArray())
        Call attractors.add("period", attractorPeriod.ToArray())
        Call attractors.add("count", attractorCount.ToArray())

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"table", table},
                {"summary", summary},
                {"attractors", attractors}
            }
        }
    End Function

    ''' <summary>
    ''' The coarse grained module layer (FCM): module assignment, bounded steady state and warm start.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Returns a named list:
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>``table``: ``gene`` / ``module`` / ``baseline``, the module assignment
    ''' computed at fit time (only present when ``fcmWarmStart = TRUE``);</description></item>
    ''' <item><description>``modules``: ``module`` / ``n_genes`` / ``activation``, the bounded
    ''' activation of the module level fuzzy cognitive map on the training baseline;</description></item>
    ''' <item><description>``initial``: ``gene`` / ``warm_start`` / ``surrogate``, the per gene
    ''' initial state derived from the module state (this is what the root finder starts from).</description></item>
    ''' <item><description>``summary``: ``available`` / ``module_count`` / ``converged`` /
    ''' ``steps`` / ``period`` / ``notes``.</description></item>
    ''' </list>
    ''' <para>
    ''' The module layer is bounded by construction, so it always terminates with either a fixed
    ''' point or a detected limit cycle. It is a **rough** skeleton: the module average erases the
    ''' individual genes, which is why the default is off and the surrogate must never replace the
    ''' per gene prediction.
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained model built with ``fcmWarmStart = TRUE``</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a named list of data.frames</returns>
    <ExportAPI("fuzzygene_modules")>
    <RApiReturn(GetType(list))>
    Public Function fuzzygene_modules(fuzzygene As fz.FuzzyGeneModel, Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Dim modules As Integer() = fuzzygene.Network.Modules

        If modules Is Nothing Then
            Return RInternal.debug.stop(
                "the model carries no module assignment: enable it before training with " &
                "FuzzyGene::fuzzygene_hybrid_config(config, fcmWarmStart = TRUE)", env)
        End If

        Dim layer As New fz.Hybrid.FcmModuleLayer(fuzzygene.Network)

        If Not layer.Available Then
            Return RInternal.debug.stop(
                "the module layer needs at least two modules; lower fcmModules or train on more genes", env)
        End If

        Dim result As fz.Hybrid.FcmModuleResult

        Try
            result = layer.BuildInitial(fuzzygene.Network.BaselineRaw)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        If result Is Nothing Then
            Return RInternal.debug.stop("the module layer produced no result", env)
        End If

        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call table.add("gene", fuzzygene.Network.GeneNames)
        Call table.add("module", modules)
        Call table.add("baseline", fuzzygene.Network.BaselineRaw)

        Dim moduleCount As Integer = result.ModuleActivation.Length
        Dim counts(moduleCount - 1) As Integer
        Dim total As Integer = 0

        For i As Integer = 0 To modules.Length - 1
            If modules(i) >= 0 AndAlso modules(i) < moduleCount Then
                counts(modules(i)) += 1
                total += 1
            End If
        Next

        Dim moduleIds(moduleCount - 1) As Integer
        Dim geneCounts(moduleCount - 1) As Integer

        For k As Integer = 0 To moduleIds.Length - 1
            moduleIds(k) = k
            geneCounts(k) = counts(k)
        Next

        Dim moduleTable As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call moduleTable.add("module", moduleIds)
        Call moduleTable.add("n_genes", geneCounts)
        Call moduleTable.add("activation", result.ModuleActivation)

        Dim initial As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call initial.add("gene", fuzzygene.Network.GeneNames)
        Call initial.add("warm_start", result.GeneInitial)
        Call initial.add("surrogate", result.Surrogate)

        Dim summary As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Call summary.add("available", New Boolean() {True})
        Call summary.add("module_count", New Integer() {result.ModuleActivation.Length})
        Call summary.add("converged", New Boolean() {result.Converged})
        Call summary.add("steps", New Integer() {result.Steps})
        Call summary.add("period", New Integer() {result.Period})
        Call summary.add("notes", New String() {If(result.Notes, "")})
        Call summary.add("genes_assigned", New Integer() {total})

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"table", table},
                {"modules", moduleTable},
                {"initial", initial},
                {"summary", summary}
            }
        }
    End Function

    ''' <summary>
    ''' Solver diagnostics of one virtual perturbation (exit reason, residuals, saturation).
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' One row with both the mutant and the wildtype run:
    ''' ``exit_reason`` / ``iterations`` / ``free_residual`` / ``clamp_residual`` /
    ''' ``out_of_support`` / ``pinned`` / ``attractor_period`` / ``omega`` / ``tolerance`` /
    ''' ``lagged`` / ``elapsed_ms``, plus the same fields prefixed with ``control_``.
    ''' </para>
    ''' <para>
    ''' Read ``exit_reason`` first. Only ``Converged`` means the flat tail of the curve is a real
    ''' steady state; ``MaxIterations`` is a truncated plateau, ``LimitCycle`` means the system
    ''' oscillates (the period mean is the honest answer), ``SupportSaturated`` means the support
    ''' clamp, not the model, produced the numbers, and ``Diverged`` means there is no answer.
    ''' ``clamp_residual`` is a diagnostic only: the clamped gene is by construction not at a fixed
    ''' point of the map, so it never goes to zero and must not be used as a convergence test.
    ''' </para>
    ''' </remarks>
    ''' <param name="fuzzygene">a trained (or restored) model</param>
    ''' <param name="gene">the target gene of the virtual perturbation</param>
    ''' <param name="mode">``knockout`` (default), ``knockdown`` or ``overexpression``</param>
    ''' <param name="steps">maximum iteration steps; 0 uses the model configuration</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>a one row data.frame</returns>
    <ExportAPI("fuzzygene_solver_report")>
    <RApiReturn(GetType(dataframe))>
    Public Function fuzzygene_solver_report(fuzzygene As fz.FuzzyGeneModel,
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

        If Not parseInterventionMode(mode, intervention) Then
            Return RInternal.debug.stop($"unknown perturbation mode '{mode}', expected knockout / knockdown / overexpression", env)
        End If

        Dim trace As fzpert.PerturbationTrace

        Try
            trace = fuzzygene.TracePerturbation(gene, intervention, steps)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        If trace Is Nothing OrElse trace.Mutant Is Nothing Then
            Return RInternal.debug.stop($"the target gene '{gene}' is not modelled by this network", env)
        End If

        Dim mutant As fzdata.SteadyStateTrace = trace.Mutant
        Dim control As fzdata.SteadyStateTrace = trace.Control

        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call table.add("gene", New String() {gene})
        Call table.add("mode", New String() {trace.Mode.ToString()})
        Call table.add("clamp_value", New Double() {trace.ClampValue})
        Call table.add("exit_reason", New String() {mutant.ExitReason.ToString()})
        Call table.add("converged", New Boolean() {mutant.Converged})
        Call table.add("iterations", New Integer() {mutant.Iterations})
        Call table.add("free_residual", New Double() {mutant.FreeResidual})
        Call table.add("clamp_residual", New Double() {mutant.ClampResidual})
        Call table.add("out_of_support", New Double() {mutant.OutOfSupportFraction})
        Call table.add("pinned", New Double() {mutant.PinnedFraction})
        Call table.add("attractor_period", New Integer() {mutant.AttractorPeriod})
        Call table.add("stabilized_by", New String() {If(mutant.StabilizedBy, "")})
        Call table.add("control_exit_reason", New String() {If(control Is Nothing, "", control.ExitReason.ToString())})
        Call table.add("control_iterations", New Integer() {If(control Is Nothing, 0, control.Iterations)})
        Call table.add("control_free_residual", New Double() {If(control Is Nothing, 0.0, control.FreeResidual)})
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
