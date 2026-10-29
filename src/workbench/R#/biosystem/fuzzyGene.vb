Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataMining.FuzzyLogic.TSK
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.FuzzyGene
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports fz = SMRUCC.genomics.Analysis.FuzzyGene
Imports fzcfg = SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports fzdata = SMRUCC.genomics.Analysis.FuzzyGene.Data
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
    ''' ``config`` argument of ``fuzzygene::build_fuzzygene``. Structure and training options are
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
    ''' the configuration created by ``fuzzygene::fuzzygene_config``; ``Nothing`` uses the defaults.
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
    ''' or failed instead of aborting the whole run. Use ``fuzzygene::fuzzygene_report`` to inspect
    ''' the per gene diagnostics.
    ''' </remarks>
    ''' <param name="fuzzygene">the untrained model created by ``fuzzygene::build_fuzzygene``</param>
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
                                      context As String,
                                      Optional valueColumn As String = Nothing,
                                      Optional minCoverage As Double = 0,
                                      Optional env As Environment = Nothing) As Object
        If fuzzygene Is Nothing OrElse Not fuzzygene.IsTrained Then
            Return RInternal.debug.stop("the model has not been trained yet", env)
        End If

        Try
            Dim report = fuzzygene.LoadContext(context, valueColumn, minCoverage)

            ' 拆成字符串数组返回，print() 在 R# 里才会一行一行显示
            Return report.ToString().LineTokens
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
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

End Module
