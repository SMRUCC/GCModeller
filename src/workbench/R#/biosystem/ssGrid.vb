Imports System.Globalization
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports SMRUCC.genomics.Analysis.SGrid.IO
Imports SMRUCC.genomics.Analysis.SGrid.Perturbation
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports ODEs = SMRUCC.genomics.Analysis.SSystem.Kernel.ODEs
Imports ODEsOut = Microsoft.VisualBasic.Math.Calculus.Dynamics.Data.ODEsOut
Imports PriorEdge = SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge
Imports PriorNet = SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports std = System.Math

''' <summary>
''' The S-system based gene regulatory network inference toolkit
''' (基于 S-system 动力学系统的基因表达调控网络拟合工具)
''' </summary>
''' 
''' <remarks>
''' This R# package module exposes the ``SGrid`` pipeline as a set of R functions:
''' 
''' + ``sfit_options``: create the fitting configuration object(<see cref="FittingOptions"/>);
''' + ``sfit``: infer the S-system gene regulatory network from a gene expression 
'''   matrix(rows are genes and the columns are the time points), the optional 
'''   ``prior`` knowledge network(<see cref="PriorNet"/>) is used as a soft 
'''   constraint of the network structure;
''' + ``network_edges``, ``sgrid_parameters``, ``sgrid_coefficients`` and ``sgrid_r2``:
'''   query the fitted model, these functions return ``data.frame`` objects;
''' + ``sgrid_simulate``: integrate the fitted S-system model and return the 
'''   trajectory as a ``data.frame``;
''' + ``sgrid_context``: map a pseudobulk context(the raw counts) onto the model 
'''   state space, so that it can be used as the ODE initial state;
''' + ``perturb``: run the in silico knockdown/knockout/overexpression 
'''   experiment, optionally under a given sample context;
''' + ``perturb_export``: save the perturbation screening result as csv files;
''' + ``sgrid_prior``: read the prior knowledge regulatory network from a csv 
'''   table(or a ``data.frame``);
''' + ``sgrid_save``: export all of the model tables and the ``.plas`` model script;
''' 
''' and the binary serialization support of the fitted model:
''' ``writeBin(model, con = file("network.zip"))`` and 
''' ``readBin(file("network.zip"), what = "ssgrid_network")``.
''' </remarks>
''' <example>
''' imports "ssGrid" from "biosystem";
''' 
''' let opts    = sfit_options(sparsity = 0.01, max_edges = 8, max_regulators = 24, smooth = 9, l1 = 0);
''' let prior   = sgrid_prior("/data/TF_target_prior_curated.csv");
''' let network = sfit(x, opts, prior = prior, time = t);
''' 
''' print(sgrid_r2(network) |> mean);
''' write.csv(sgrid_coefficients(network), file = "/tmp/coef.csv");
''' sgrid_save(network, "/tmp/network");
''' 
''' let ctx  = sgrid_context(network, "/data/pseudobulk_context_A.csv");
''' let resp = perturb(network, c("ABCD1","ACLY"), type = "knockdown", magnitude = 0.8,
'''                    context = ctx, export = "/tmp/ko");
''' write.csv(as.data.frame(resp), file = "/tmp/ko/response.csv");
''' 
''' </example>
<Package("ssGrid")>
<RTypeExport("ssgrid_network", GetType(SSystemNetwork))>
<RTypeExport("sfit_options", GetType(FittingOptions))>
<RTypeExport("sgrid_prior", GetType(PriorNet))>
<RTypeExport("sgrid_screening", GetType(ScreeningResult))>
Module ssGrid

    Sub Main()
        ' binary serialization of the fitted model object
        Call RInternal.generic.add("writeBin", GetType(SSystemNetwork), AddressOf saveModel)
        Call RInternal.generic.add("readBin.ssgrid_network", GetType(Stream), AddressOf loadModel)

        ' as.data.frame() support of the custom model objects
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(SSystemNetwork), AddressOf networkTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(SSystemParameters), AddressOf parameterTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(PriorNet), AddressOf priorTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(ScreeningResult), AddressOf screeningTable)
    End Sub

#Region "as.data.frame"

    ''' <summary>
    ''' convert the fitted network model to the edge table data
    ''' </summary>
    ''' <param name="network"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a dataframe with the columns: ``source,target,weight,g,h,interaction,
    ''' prior_supported,prior_type,prior_confidence``
    ''' </returns>
    ''' <example>
    ''' as.data.frame(network);
    ''' </example>
    Private Function networkTable(network As SSystemNetwork, args As list, env As Environment) As dataframe
        Return newEdgeTable(network.Edges)
    End Function

    ''' <summary>
    ''' convert the fitted S-system parameters to the gene parameter table
    ''' </summary>
    ''' <param name="parameters"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a dataframe with the columns: ``gene,original,alpha,beta,rmse,r2,converged``
    ''' </returns>
    ''' <example>
    ''' as.data.frame(network$Parameters);
    ''' </example>
    Private Function parameterTable(parameters As SSystemParameters, args As list, env As Environment) As dataframe
        Return newParameterTable(parameters)
    End Function

    ''' <summary>
    ''' convert the prior knowledge network to the edge table data
    ''' </summary>
    ''' <param name="prior"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a dataframe with the columns: ``tf,target,type,confidence,evidence``
    ''' </returns>
    ''' <example>
    ''' as.data.frame(prior);
    ''' </example>
    Private Function priorTable(prior As PriorNet, args As list, env As Environment) As dataframe
        Return newPriorTable(prior)
    End Function

    ''' <summary>
    ''' convert the virtual perturbation screening result to the response table
    ''' </summary>
    ''' <param name="screening"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a dataframe with the columns: ``target,perturbation,gene,control,perturbed,
    ''' fold_change,max_deviation,response_time,is_target``
    ''' </returns>
    ''' <example>
    ''' as.data.frame(screening);
    ''' </example>
    Private Function screeningTable(screening As ScreeningResult, args As list, env As Environment) As dataframe
        Return newScreeningTable(screening)
    End Function

#End Region

#Region "binary serialization"

    ''' <summary>
    ''' save the fitted S-system network model as a zip archive
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' writeBin(model, con = file("/sgrid.zip"));
    ''' </example>
    Private Function saveModel(model As SSystemNetwork, args As list, env As Environment) As Object
        Dim con As Stream = args!con

        Try
            Call model.Save(con)
            Call con.Flush()
        Catch ex As Exception
            Call App.LogException(ex)
            Call Console.WriteLine($"[ERROR] {ex.Message}")

            Return RInternal.debug.stop(ex, env)
        End Try

        Return True
    End Function

    ''' <summary>
    ''' load the fitted S-system network model from a zip archive
    ''' </summary>
    ''' <param name="s"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' return(readBin(file("/sgrid.zip"), what = "ssgrid_network"));
    ''' </example>
    Private Function loadModel(s As Stream, args As list, env As Environment) As Object
        Return ZipPersists.Load(s)
    End Function

#End Region

#Region "fitting"

    ''' <summary>
    ''' create the fitting configuration object of the S-system network inference
    ''' </summary>
    ''' <param name="normalize">
    ''' the normalization method of the gene expression matrix, could be 
    ''' ``log2``(default), ``none``, ``rowzscore`` or ``quantile``.
    ''' </param>
    ''' <param name="pseudo">the pseudo count that used by the log2 normalization.</param>
    ''' <param name="sparsity">
    ''' the sparsity threshold of the net elasticity, the interaction with a smaller 
    ''' ``|g - h|`` than this threshold will not be kept in the network topology.
    ''' </param>
    ''' <param name="max_edges">the max number of the regulators that could be kept for each target gene.</param>
    ''' <param name="max_regulators">
    ''' the max number of the candidate regulators for each target gene, the prior 
    ''' supported regulators are always included before the correlation based 
    ''' preselection; zero means that all of the genes will be used as candidate.
    ''' </param>
    ''' <param name="l1">the L1 penalty weight of the kinetic orders.</param>
    ''' <param name="stability">the penalty weight of a positive self loop.</param>
    ''' <param name="smooth">
    ''' the moving average window that used in the numerical differentiation, 
    ''' it is required by the noisy pseudotime data; zero means no smoothing.
    ''' </param>
    ''' <param name="prior_penalty">the soft penalty weight of the interactions that are not supported by the prior.</param>
    ''' <param name="prior_sign_penalty">the penalty weight of the direction conflict against the prior.</param>
    ''' <param name="prior_rank_boost">the ranking boost of the prior supported edges in the sparsification.</param>
    ''' <param name="prior">the prior knowledge network object.</param>
    ''' <param name="de_pop">the population size of the differential evolution solver.</param>
    ''' <param name="de_iters">the max iteration number of the differential evolution solver.</param>
    ''' <param name="seed">the random seed.</param>
    ''' <param name="parallel">run the per gene fitting in parallel or not?</param>
    ''' <param name="verbose">print the fitting progress or not?</param>
    ''' <param name="refine">
    ''' refine the kinetic parameters by the trajectory fitting after the 
    ''' decoupled slope fitting?
    ''' </param>
    ''' <param name="final_time">the final time of the simulation, zero means the end of the time axis.</param>
    ''' <param name="precise">the integration step of the simulation.</param>
    ''' <param name="steps">the number of the simulation sample points, zero means auto.</param>
    ''' <returns>a <see cref="FittingOptions"/> object</returns>
    ''' <example>
    ''' let opts = sfit_options(sparsity = 0.01, max_edges = 6, smooth = 9);
    ''' </example>
    <ExportAPI("sfit_options")>
    <RApiReturn(GetType(FittingOptions))>
    Public Function sfitOptions(Optional normalize As String = "log2",
                                Optional pseudo As Double = 1,
                                Optional sparsity As Double = 0.2,
                                Optional max_edges As Integer = 3,
                                Optional max_regulators As Integer = 0,
                                Optional l1 As Double = 0.01,
                                Optional stability As Double = 0.2,
                                Optional smooth As Integer = 5,
                                Optional prior_penalty As Double = 0.2,
                                Optional prior_sign_penalty As Double = 0.2,
                                Optional prior_rank_boost As Double = 1,
                                Optional prior As PriorNet = Nothing,
                                Optional de_pop As Integer = 40,
                                Optional de_iters As Integer = 1200,
                                Optional seed As Integer = 1234567,
                                Optional parallel As Boolean = True,
                                Optional verbose As Boolean = False,
                                Optional refine As Boolean = False,
                                Optional final_time As Double = 0,
                                Optional precise As Double = 0.1,
                                Optional thread_tasks As Integer = -1,
                                Optional steps As Integer = 0) As FittingOptions

        Dim opts As New FittingOptions With {
            .Normalize = parseNormalization(normalize),
            .PseudoCount = pseudo,
            .Sparsity = sparsity,
            .MaxEdgesPerGene = max_edges,
            .MaxRegulators = max_regulators,
            .L1Penalty = l1,
            .StabilityPenalty = stability,
            .SmoothWindow = smooth,
            .PriorPenalty = prior_penalty,
            .PriorSignPenalty = prior_sign_penalty,
            .PriorRankBoost = prior_rank_boost,
            .Prior = prior,
            .UsePrior = prior IsNot Nothing,
            .DEPopulationSize = de_pop,
            .DEMaxIterations = de_iters,
            .Seed = seed,
            .Parallel = parallel,
            .Verbose = verbose,
            .RefineTrajectory = refine,
            .FinalTime = final_time,
            .Precise = precise,
            .Steps = steps,
            .MaxDegreeOfParallelism = thread_tasks
        }

        Call opts.Validate()

        Return opts
    End Function

    ''' <summary>
    ''' infer the S-system gene regulatory network from the gene expression matrix
    ''' </summary>
    ''' <param name="x">
    ''' the gene expression matrix(rows are genes and columns are the time points), 
    ''' this matrix object could be loaded from a csv file via ``geneExpression::load.expr``.
    ''' </param>
    ''' <param name="opts">the fitting configuration object, see also: <see cref="sfitOptions"/>.</param>
    ''' <param name="prior">
    ''' the prior knowledge regulatory network, which could be a 
    ''' <see cref="PriorNet"/> object(from ``bnlearn::as.prior_net`` or ``sgrid_prior``) 
    ''' or a csv file path. This prior is used as a **soft** constraint: an interaction 
    ''' that is not supported by the prior will be shrunk, but a new regulation 
    ''' relation could still be discovered from the expression data.
    ''' </param>
    ''' <param name="time">the time points of the matrix columns, a numeric vector.</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="SSystemNetwork"/> object that contains the fitted network 
    ''' topology and the kinetic parameters; a R# error message object will be 
    ''' returned if the input data is not valid.
    ''' </returns>
    ''' <example>
    ''' let opts = sfit_options(sparsity = 0.01, max_edges = 6, smooth = 9);
    ''' let network = sfit(x, opts, prior = as.prior_net(prior_edges), time = t);
    ''' </example>
    <ExportAPI("sfit")>
    <RApiReturn(GetType(SSystemNetwork))>
    Public Function sfit(x As matrix,
                         Optional opts As FittingOptions = Nothing,
                         <RRawVectorArgument> Optional prior As Object = Nothing,
                         <RRawVectorArgument> Optional time As Object = Nothing,
                         Optional env As Environment = Nothing) As Object

        If x Is Nothing Then
            Return RInternal.debug.stop("the gene expression matrix for the S-system fitting is nothing!", env)
        End If

        If opts Is Nothing Then
            opts = New FittingOptions()
        End If
        If Not time Is Nothing Then
            Dim axis As Double() = CLRVector.asNumeric(time)

            If axis.Length <> x.sample_count Then
                Return RInternal.debug.stop(
                    $"the size of the given time axis({axis.Length}) should be equals to the column number of the expression matrix({x.sample_count})!",
                    env)
            End If

            opts.TimePoints = axis
        End If
        If opts.TimePoints.IsNullOrEmpty AndAlso x.sample_count < 4 Then
            Return RInternal.debug.stop("at least 4 time points are required by the S-system network inference!", env)
        End If

        Dim priorNet As PriorNet = Nothing

        If Not prior Is Nothing Then
            Dim load As Object = readPrior(prior, env)

            If TypeOf load Is PriorNet Then
                priorNet = DirectCast(load, PriorNet)
            Else
                Return load
            End If
        Else
            priorNet = opts.Prior
        End If

        opts.Prior = priorNet
        opts.UsePrior = priorNet IsNot Nothing

        Console.WriteLine("[INFO] fitting the S-system model...")

        Dim watch As Stopwatch = Stopwatch.StartNew()

        Try
            Dim network As SSystemNetwork = SSystemNetwork.Fit(x, opts)

            Console.WriteLine($"[INFO] fitting finished in {watch.Elapsed.TotalSeconds.ToString("F1")}s: {network.ToString}")
            Console.WriteLine(network.Report())

            Return network
        Catch ex As Exception
            Call Console.WriteLine($"[ERROR] {ex.Message}")

            Return RInternal.debug.stop(ex, env)
        End Try
    End Function

#End Region

#Region "prior knowledge"

    ''' <summary>
    ''' read the prior knowledge regulatory network from a csv table or a data.frame
    ''' </summary>
    ''' <param name="data">
    ''' a csv file path, or a data.frame object which contains the columns of 
    ''' ``tf,target,type,confidence,evidence``(the column name is case insensitive, 
    ''' and the ``confidence`` column could be a numeric score or the 
    ''' ``High/Medium/Low`` label).
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>a <see cref="PriorNet"/> object, or a R# error message object.</returns>
    ''' <example>
    ''' let prior = sgrid_prior("/data/TF_target_prior_curated.csv");
    ''' let prior = sgrid_prior(data.frame(tf = ..., target = ..., type = ..., confidence = ...));
    ''' </example>
    <ExportAPI("sgrid_prior")>
    <RApiReturn(GetType(PriorNet))>
    Public Function sgridPrior(<RRawVectorArgument> data As Object, Optional env As Environment = Nothing) As Object
        Return readPrior(data, env)
    End Function

    ''' <summary>
    ''' the prior network loading helper, it accepts a csv file path or a data.frame
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Private Function readPrior(data As Object, env As Environment) As Object
        If data Is Nothing Then
            Return RInternal.debug.stop("the prior knowledge network data is nothing!", env)
        End If

        If TypeOf data Is PriorNet Then
            Return DirectCast(data, PriorNet)
        ElseIf TypeOf data Is String Then
            Dim filePath As String = CStr(data)

            If Not File.Exists(filePath) Then
                Return RInternal.debug.stop($"the prior knowledge network file '{filePath}' is not exists!", env)
            End If

            Return PriorKnowledge.LoadPriorNetwork(filePath)
        ElseIf TypeOf data Is dataframe Then
            Return priorFromTable(DirectCast(data, dataframe), env)
        Else
            Return Message.InCompatibleType(GetType(PriorNet), data.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' create the prior network from a data.frame table
    ''' </summary>
    ''' <param name="table"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Private Function priorFromTable(table As dataframe, env As Environment) As Object
        Dim tf As String() = getColumnText(table, "tf", "source", "regulator")
        Dim target As String() = getColumnText(table, "target", "targetgene", "target_gene", "gene")
        Dim type As String() = getColumnText(table, "type", "regulationtype", "regulation_type", "effector")
        Dim conf As String() = getColumnText(table, "confidence", "score")
        Dim evidence As String() = getColumnText(table, "evidence", "source_db")

        If tf.IsNullOrEmpty Then
            Return RInternal.debug.stop("the prior network table does not contains the 'tf' column!", env)
        End If
        If target.IsNullOrEmpty OrElse target.Length <> tf.Length Then
            Return RInternal.debug.stop("the 'target' column of the prior network table is missing or its size is not matched with the 'tf' column!", env)
        End If

        Dim edges As New List(Of PriorEdge)

        For i As Integer = 0 To tf.Length - 1
            Dim confidence As Double = 1

            If Not conf.IsNullOrEmpty AndAlso i < conf.Length Then
                confidence = parseConfidence(conf(i))
            End If

            Call edges.Add(New PriorEdge With {
                .TF = If(tf(i), "").Trim,
                .TargetGene = If(target(i), "").Trim,
                .RegulationType = PriorEdge.ParseEffector(If(type.IsNullOrEmpty OrElse i >= type.Length, "", type(i))),
                .Confidence = confidence,
                .Evidence = If(evidence.IsNullOrEmpty OrElse i >= evidence.Length, "", evidence(i))
            })
        Next

        Return New PriorNet(edges)
    End Function

#End Region

#Region "model query"

    ''' <summary>
    ''' get the network topology of the fitted S-system model as a data.frame
    ''' </summary>
    ''' <param name="network"></param>
    ''' <returns>
    ''' a dataframe with the columns: ``source,target,weight,g,h,interaction,
    ''' prior_supported,prior_type,prior_confidence``
    ''' </returns>
    ''' <example>
    ''' let edges = network_edges(network);
    ''' </example>
    <ExportAPI("network_edges")>
    <RApiReturn(GetType(dataframe))>
    Public Function networkEdges(network As SSystemNetwork) As dataframe
        Return newEdgeTable(network.Edges)
    End Function

    ''' <summary>
    ''' get the fitted kinetic parameters of each gene as a data.frame
    ''' </summary>
    ''' <param name="network"></param>
    ''' <returns>
    ''' a dataframe with the columns: ``gene,original,alpha,beta,rmse,r2,converged``
    ''' </returns>
    ''' <example>
    ''' let params = sgrid_parameters(network);
    ''' </example>
    <ExportAPI("sgrid_parameters")>
    <RApiReturn(GetType(dataframe))>
    Public Function modelParameters(network As SSystemNetwork) As dataframe
        Return newParameterTable(network.Parameters)
    End Function

    ''' <summary>
    ''' get the coefficient matrix of the fitted S-system model
    ''' </summary>
    ''' <param name="network"></param>
    ''' <param name="type">
    ''' the matrix type: ``net``(default, the net elasticity ``g - h``), 
    ''' ``g``(the kinetic order of the synthesis term) or 
    ''' ``h``(the kinetic order of the degradation term).
    ''' </param>
    ''' <param name="original">
    ''' use the original gene symbol instead of the sanitized gene id? default is TRUE.
    ''' </param>
    ''' <returns>
    ''' a wide dataframe: the rows are the target genes and the columns are the 
    ''' regulators. the matrix element is the fitted coefficient.
    ''' </returns>
    ''' <example>
    ''' let net = sgrid_coefficients(network);
    ''' let g = sgrid_coefficients(network, type = "g");
    ''' </example>
    <ExportAPI("sgrid_coefficients")>
    <RApiReturn(GetType(dataframe))>
    Public Function modelCoefficients(network As SSystemNetwork,
                                      Optional type As String = "net",
                                      Optional original As Boolean = True,
                                      Optional env As Environment = Nothing) As Object

        Dim parameters As SSystemParameters = network.Parameters
        Dim n As Integer = parameters.nGenes
        Dim names As String() = If(original, parameters.OriginalNames, parameters.GeneNames)
        Dim values As Double()()

        Select Case If(type, "net").Trim.ToLowerInvariant()
            Case "net", "g-h", "elasticity"
                values = parameters.NetElasticity()
            Case "g", "synthesis"
                values = parameters.G
            Case "h", "degradation"
                values = parameters.H
            Case Else
                Return RInternal.debug.stop($"unknown coefficient matrix type '{type}', it should be one of: net, g, h", env)
        End Select

        Dim columns As New Dictionary(Of String, Array)

        For j As Integer = 0 To n - 1
            Dim regulator As Integer = j

            columns(names(j)) = Enumerable _
                .Range(0, n) _
                .Select(Function(i) values(i)(regulator)) _
                .ToArray
        Next

        Return New dataframe With {
            .columns = columns,
            .rownames = names
        }
    End Function

    ''' <summary>
    ''' get the R square of the slope fitting of each gene
    ''' </summary>
    ''' <param name="network"></param>
    ''' <returns>
    ''' a numeric vector of the R square value of each gene, the order of the 
    ''' values is the same as the row order of ``sgrid_parameters(network)$gene``
    ''' </returns>
    ''' <example>
    ''' let r2 = sgrid_r2(network);
    ''' </example>
    <ExportAPI("sgrid_r2")>
    <RApiReturn(GetType(Double))>
    Public Function modelR2(network As SSystemNetwork) As Double()
        Dim r2 As Double() = Enumerable _
            .Range(0, network.Parameters.nGenes) _
            .Select(Function(i) network.Parameters.RSquared(i)) _
            .ToArray

        Return r2
    End Function

    ''' <summary>
    ''' get the fitting report of the S-system network model
    ''' </summary>
    ''' <param name="network"></param>
    ''' <returns>a character text</returns>
    ''' <example>
    ''' cat(sgrid_report(network));
    ''' </example>
    <ExportAPI("sgrid_report")>
    <RApiReturn(GetType(String))>
    Public Function networkReport(network As SSystemNetwork) As String
        Return network.Report()
    End Function

    ''' <summary>
    ''' export the model tables and the generated ``.plas`` model script into a directory
    ''' </summary>
    ''' <param name="network"></param>
    ''' <param name="dir">the output directory, it will be created if not exists.</param>
    ''' <returns>TRUE if the model data has been saved successfully.</returns>
    ''' <example>
    ''' sgrid_save(network, "/tmp/network");
    ''' </example>
    <ExportAPI("sgrid_save")>
    Public Function saveNetwork(network As SSystemNetwork, dir As String) As Boolean
        Return network.SaveAll(dir)
    End Function

#End Region

#Region "simulation"

    ''' <summary>
    ''' integrate the fitted S-system model and return the trajectory
    ''' </summary>
    ''' <param name="network"></param>
    ''' <param name="final_time">
    ''' the final time of the integration, zero means the end of the fitted time axis.
    ''' </param>
    ''' <param name="context">
    ''' the optional initial state of the ODE integration, this value should be in 
    ''' the model state space(see also: <see cref="sgridContext"/>), the first 
    ''' sample point of the fitted time series will be used if this parameter is 
    ''' not provided.
    ''' </param>
    ''' <param name="genes">
    ''' the optional gene list that returns in the trajectory table.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>a dataframe, the first column ``#Time`` is the time axis.</returns>
    ''' <example>
    ''' let baseline = sgrid_simulate(network, context = sgrid_context(network, counts));
    ''' </example>
    <ExportAPI("sgrid_simulate")>
    <RApiReturn(GetType(dataframe))>
    Public Function simulate(network As SSystemNetwork,
                             Optional final_time As Double = 0,
                             Optional context As list = Nothing,
                             <RRawVectorArgument(TypeCodes.string)> Optional genes As Object = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim t As Double = If(final_time > 0, final_time, network.Series.TimeEnd)
        Dim x0 As Dictionary(Of String, Double) = contextValues(context)
        Dim model = network.CreateModel(finalTime:=t, initial:=x0)
        Dim output As ODEsOut = ODEs.RunSystem(model, Nothing, network.CreateRunOptions(t))

        If output Is Nothing Then
            Return RInternal.debug.stop("the ODE integration of the S-system model is failed!", env)
        End If

        Dim names As String() = output.y.Keys.ToArray

        If Not genes Is Nothing Then
            names = CLRVector.asCharacter(genes) _
                .Select(Function(g) findGeneId(network, g)) _
                .Where(Function(g) Not String.IsNullOrEmpty(g) AndAlso output.y.ContainsKey(g)) _
                .Distinct(StringComparer.OrdinalIgnoreCase) _
                .ToArray
        End If

        Return newTrajectoryTable(output, names)
    End Function

    ''' <summary>
    ''' map a pseudobulk context(the raw expression counts) onto the model state space
    ''' </summary>
    ''' <param name="network"></param>
    ''' <param name="x">
    ''' the pseudobulk context data, could be a csv file path, a data.frame(the 
    ''' columns are ``gene`` and ``mean_counts``), or a named numeric vector/list 
    ''' (the names are the gene symbols, the values are the raw counts).
    ''' </param>
    ''' <param name="missing">
    ''' the value that used for the genes which is missing in the given context: 
    ''' ``baseline``(default) means the minimal value of that gene in the training 
    ''' matrix, ``zero`` means the zero expression.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a named list of the model state space value, which can be used as the 
    ''' ``context`` parameter of the ``perturb`` and ``sgrid_simulate`` api.
    ''' </returns>
    ''' <remarks>
    ''' the model state is not the raw counts, but ``log2(max(counts,0)+pseudo) + delta``, 
    ''' the per gene offset ``delta`` was generated in the model fitting step. So the 
    ''' pseudobulk context data must be transformed by this function before it can be 
    ''' used as the ODE initial state.
    ''' </remarks>
    ''' <example>
    ''' let ctx = sgrid_context(network, "/data/pseudobulk_context_A.csv");
    ''' let resp = perturb(network, genes, context = ctx);
    ''' </example>
    <ExportAPI("sgrid_context")>
    <RApiReturn(GetType(list))>
    Public Function sgridContext(network As SSystemNetwork,
                                 <RRawVectorArgument> x As Object,
                                 Optional missing As String = "baseline",
                                 Optional env As Environment = Nothing) As Object

        Dim counts As Dictionary(Of String, Double) = contextCounts(x, env)

        If counts Is Nothing Then
            Return RInternal.debug.stop("the context data is invalid, it should be a csv file, a data.frame or a named numeric vector!", env)
        End If

        Return New list(network.MapContext(counts, missing))
    End Function

#End Region

#Region "virtual perturbation"

    ''' <summary>
    ''' run the in silico perturbation experiment on the fitted S-system network model
    ''' </summary>
    ''' <param name="network">the fitted network model, which is the output of the ``sfit`` api.</param>
    ''' <param name="geneIds">a character vector of the target gene id of the perturbation.</param>
    ''' <param name="type">
    ''' the perturbation type: ``knockdown``(default), ``knockout`` or 
    ''' ``overexpress``.
    ''' </param>
    ''' <param name="magnitude">
    ''' the perturbation strength: for the knockdown, ``0.8`` means that the target 
    ''' gene loses 80% of its activity; for the overexpress, ``1`` means that an 
    ''' extra 100% of the baseline expression will be added to the target gene.
    ''' </param>
    ''' <param name="kicks">
    ''' the times that the perturbation will be applied, a periodic stimulation is 
    ''' used to simulate a sustained treatment.
    ''' </param>
    ''' <param name="start">the time point that the first perturbation kick is applied.</param>
    ''' <param name="final_time">the final time of the simulation, zero means the end of the fitted time axis.</param>
    ''' <param name="context">
    ''' the optional sample context(the ODE initial state), which should be 
    ''' created via the ``sgrid_context`` api.
    ''' </param>
    ''' <param name="export">
    ''' the optional output directory for save the trajectory files of each 
    ''' perturbation design; nothing will be written if this parameter is not 
    ''' provided.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a <see cref="ScreeningResult"/> object which could be converted to a 
    ''' ``data.frame`` via ``as.data.frame``, or a R# error message object.
    ''' </returns>
    ''' <example>
    ''' let ctx = sgrid_context(network, "/data/pseudobulk_context_A.csv");
    ''' let resp = perturb(network, c("ABCD1","ACLY"), context = ctx, export = "/tmp/ko");
    ''' </example>
    <ExportAPI("perturb")>
    <RApiReturn(GetType(ScreeningResult))>
    Public Function perturb(network As SSystemNetwork,
                            <RRawVectorArgument(TypeCodes.string)> geneIds As Object,
                            Optional type As String = "knockdown",
                            Optional magnitude As Double = 0.8,
                            Optional kicks As Integer = 1,
                            Optional start As Double = 0.1,
                            Optional final_time As Double = 0,
                            Optional context As list = Nothing,
                            Optional export As String = Nothing,
                            Optional env As Environment = Nothing) As Object

        Dim targets As String() = CLRVector.asCharacter(geneIds) _
            .Where(Function(g) Not String.IsNullOrEmpty(g)) _
            .Select(Function(g) findGeneId(network, g)) _
            .Where(Function(g) Not String.IsNullOrEmpty(g)) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToArray

        If targets.IsNullOrEmpty Then
            Return RInternal.debug.stop("none of the given gene id is exists in the fitted network model!", env)
        End If

        Dim t As Double = If(final_time > 0, final_time, network.Series.TimeEnd)
        Dim first As Double = t * std.Max(std.Min(start, 0.9), 0)
        Dim interval As Double = std.Max((t - first) / std.Max(kicks, 1), 0.000001)
        Dim mode As InterventionMode = parseMode(type)
        Dim x0 As Dictionary(Of String, Double) = contextValues(context)
        Dim designs As New List(Of PerturbationDesign)

        For Each target As String In targets
            Call designs.Add(New PerturbationDesign With {
                .TargetGene = target,
                .Type = mode,
                .Magnitude = magnitude,
                .Start = first,
                .Interval = interval,
                .Kicks = kicks
            })
        Next

        Console.WriteLine($"[INFO] virtual perturbation on {designs.Count} target gene(s) under the {If(x0 Is Nothing, "default", "given")} context...")

        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim onSimulated As Action(Of PerturbationDesign, ODEsOut) = Nothing
        Dim dir As String = export

        If Not String.IsNullOrEmpty(dir) Then
            Call Directory.CreateDirectory(dir)

            onSimulated = Sub(design, output)
                              Call VirtualScreening.SaveTrajectory(
                                  output,
                                  Path.Combine(dir, VirtualScreening.TrajectoryFileOf(design)))
                          End Sub
        End If

        Dim screening As ScreeningResult

        Try
            screening = VirtualScreening.Run(network, designs, finalTime:=t, context:=x0, onSimulated:=onSimulated)
        Catch ex As Exception
            Call Console.WriteLine($"[ERROR] {ex.Message}")

            Return RInternal.debug.stop(ex, env)
        End Try

        If Not String.IsNullOrEmpty(dir) Then
            Call VirtualScreening.SaveScreening(screening, dir)
            Call Console.WriteLine($"[INFO] the screening result was saved into: {dir}")
        End If

        Console.WriteLine($"[INFO] the screening was finished in {watch.Elapsed.TotalSeconds.ToString("F1")}s: {screening.Records.SafeQuery().Count()} record(s)")

        Return screening
    End Function

    ''' <summary>
    ''' save the virtual perturbation screening result as csv files
    ''' </summary>
    ''' <param name="screening">the output of the ``perturb`` api.</param>
    ''' <param name="dir">the output directory.</param>
    ''' <returns>the number of the files that has been written.</returns>
    ''' <remarks>
    ''' this function writes the ``control.csv`` and the ``perturb_records.csv``; 
    ''' the trajectory of each perturbation design is not kept in the screening 
    ''' result object, so please use the ``export`` parameter of the ``perturb`` 
    ''' api if the trajectory files are required.
    ''' </remarks>
    ''' <example>
    ''' perturb_export(resp, "/tmp/ko");
    ''' </example>
    <ExportAPI("perturb_export")>
    <RApiReturn(GetType(Integer))>
    Public Function exportScreening(screening As ScreeningResult, dir As String) As Integer
        Return VirtualScreening.SaveScreening(screening, dir)
    End Function

#End Region

#Region "helpers"

    Private Function newEdgeTable(edges As RegulatoryEdge()) As dataframe
        Dim columns As New Dictionary(Of String, Array)
        Dim data As RegulatoryEdge() = edges.SafeQuery().ToArray

        columns("source") = data.Select(Function(e) e.source).ToArray
        columns("target") = data.Select(Function(e) e.target).ToArray
        columns("weight") = data.Select(Function(e) e.weight).ToArray
        columns("g") = data.Select(Function(e) e.g).ToArray
        columns("h") = data.Select(Function(e) e.h).ToArray
        columns("interaction") = data.Select(Function(e) e.interaction).ToArray
        columns("prior_supported") = data.Select(Function(e) e.PriorSupported).ToArray
        columns("prior_type") = data.Select(Function(e) e.PriorType).ToArray
        columns("prior_confidence") = data.Select(Function(e) e.PriorConfidence).ToArray

        Return New dataframe With {
            .columns = columns,
            .rownames = data _
                .Select(Function(e, i) $"[{i + 1}] {e.source} -> {e.target}") _
                .ToArray
        }
    End Function

    Private Function newParameterTable(parameters As SSystemParameters) As dataframe
        Dim columns As New Dictionary(Of String, Array)
        Dim n As Integer = parameters.nGenes

        columns("gene") = parameters.GeneNames
        columns("original") = parameters.OriginalNames
        columns("alpha") = parameters.alpha
        columns("beta") = parameters.beta
        columns("rmse") = parameters.SlopeRMSE
        columns("r2") = Enumerable.Range(0, n).Select(Function(i) parameters.RSquared(i)).ToArray
        columns("converged") = parameters.Converged

        Return New dataframe With {
            .columns = columns,
            .rownames = parameters.OriginalNames
        }
    End Function

    Private Function newPriorTable(prior As PriorNet) As dataframe
        Dim columns As New Dictionary(Of String, Array)
        Dim data As PriorEdge() = If(prior Is Nothing, New PriorEdge() {}, prior.Edges.ToArray)

        columns("tf") = data.Select(Function(e) e.TF).ToArray
        columns("target") = data.Select(Function(e) e.TargetGene).ToArray
        columns("type") = data.Select(Function(e) e.RegulationType.ToString()).ToArray
        columns("confidence") = data.Select(Function(e) e.Confidence).ToArray
        columns("evidence") = data.Select(Function(e) If(e.Evidence, "")).ToArray

        Return New dataframe With {
            .columns = columns,
            .rownames = data _
                .Select(Function(e, i) $"[{i + 1}] {e.TF} -> {e.TargetGene}") _
                .ToArray
        }
    End Function

    Private Function newScreeningTable(screening As ScreeningResult) As dataframe
        Dim columns As New Dictionary(Of String, Array)
        Dim data As ScreeningRecord() = screening.Records.SafeQuery().ToArray

        columns("target") = data.Select(Function(r) r.Target).ToArray
        columns("perturbation") = data.Select(Function(r) r.Perturbation).ToArray
        columns("gene") = data.Select(Function(r) r.Gene).ToArray
        columns("control") = data.Select(Function(r) r.ControlLevel).ToArray
        columns("perturbed") = data.Select(Function(r) r.PerturbedLevel).ToArray
        columns("fold_change") = data.Select(Function(r) r.FoldChange).ToArray
        columns("max_deviation") = data.Select(Function(r) r.MaxDeviation).ToArray
        columns("response_time") = data.Select(Function(r) r.ResponseTime).ToArray
        columns("is_target") = data.Select(Function(r) r.IsTarget).ToArray

        Return New dataframe With {
            .columns = columns,
            .rownames = data _
                .Select(Function(r, i) $"[{i + 1}] {r.Target} -> {r.Gene}") _
                .ToArray
        }
    End Function

    Private Function newTrajectoryTable(output As ODEsOut, names As String()) As dataframe
        Dim columns As New Dictionary(Of String, Array)
        Dim time As String() = output _
            .x _
            .Select(Function(t) t.ToString("G6", CultureInfo.InvariantCulture)) _
            .ToArray

        For Each name As String In names.SafeQuery()
            columns(name) = CType(output.y(name).value.Clone(), Double())
        Next

        Return New dataframe With {
            .columns = columns,
            .rownames = time
        }
    End Function

    ''' <summary>
    ''' resolve the gene id in the fitted model: it accepts both of the original 
    ''' gene symbol and the sanitized gene id
    ''' </summary>
    ''' <param name="network"></param>
    ''' <param name="gene"></param>
    ''' <returns></returns>
    Private Function findGeneId(network As SSystemNetwork, gene As String) As String
        Dim id As String = If(gene, "").Trim

        If id.Length = 0 Then
            Return Nothing
        End If

        Dim idx As Integer = network.Series.GeneIndex(id)

        If idx >= 0 Then
            Return network.Parameters.GeneNames(idx)
        End If

        idx = Array.IndexOf(network.Parameters.OriginalNames, id)

        If idx >= 0 Then
            Return network.Parameters.GeneNames(idx)
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' convert the R context object into the model state space dictionary
    ''' </summary>
    ''' <param name="context"></param>
    ''' <returns></returns>
    Private Function contextValues(context As list) As Dictionary(Of String, Double)
        If context Is Nothing Then
            Return Nothing
        End If

        Return context.asGeneric(Of Double)(Function(x) CLRVector.asNumeric(x)(Scan0))
    End Function

    ''' <summary>
    ''' read the pseudobulk context raw counts from a csv file, a data.frame or a named vector
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Private Function contextCounts(x As Object, env As Environment) As Dictionary(Of String, Double)
        If x Is Nothing Then
            Return Nothing
        End If

        If TypeOf x Is String Then
            Return readCountsFile(CStr(x))
        ElseIf TypeOf x Is dataframe Then
            Return countsFromTable(DirectCast(x, dataframe))
        ElseIf TypeOf x Is list Then
            Return DirectCast(x, list).asGeneric(Of Double)(Function(v) CLRVector.asNumeric(v)(Scan0))
        Else
            Return Nothing
        End If
    End Function

    Private Function countsFromTable(table As dataframe) As Dictionary(Of String, Double)
        Dim genes As String() = getColumnText(table, "gene_name", "gene", "symbol", "id")
        Dim values As String() = getColumnText(table, "mean_counts", "counts", "mean", "expression")
        Dim counts As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        If genes.IsNullOrEmpty OrElse values.IsNullOrEmpty Then
            Return counts
        End If

        For i As Integer = 0 To std.Min(genes.Length, values.Length) - 1
            Dim value As Double

            If Double.TryParse(values(i), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                counts(genes(i)) = value
            End If
        Next

        Return counts
    End Function

    Private Function readCountsFile(filePath As String) As Dictionary(Of String, Double)
        Dim counts As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        If String.IsNullOrEmpty(filePath) OrElse Not File.Exists(filePath) Then
            Return counts
        End If

        Dim iGene As Integer = 0
        Dim iValue As Integer = 1
        Dim header As Boolean = True

        For Each line As String In File.ReadLines(filePath)
            If String.IsNullOrWhiteSpace(line) Then
                Continue For
            End If

            Dim fields As String() = PriorKnowledge.SplitCsvLine(line)

            If header Then
                header = False

                For i As Integer = 0 To fields.Length - 1
                    Select Case fields(i).Trim().ToLowerInvariant()
                        Case "gene_name", "gene", "symbol"
                            iGene = i
                        Case "mean_counts", "counts", "mean", "expression"
                            iValue = i
                    End Select
                Next

                Continue For
            End If

            If fields.Length <= std.Max(iGene, iValue) Then
                Continue For
            End If

            Dim gene As String = fields(iGene).Trim().Trim(""""c)
            Dim value As Double

            If gene.Length = 0 Then
                Continue For
            End If
            If Double.TryParse(fields(iValue), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                counts(gene) = value
            End If
        Next

        Return counts
    End Function

    Private Function getColumnText(table As dataframe, ParamArray candidates As String()) As String()
        For Each name As String In table.colnames
            For Each candidate As String In candidates
                If String.Equals(name, candidate, StringComparison.OrdinalIgnoreCase) Then
                    Return CLRVector.asCharacter(table(name))
                End If
            Next
        Next

        Return Nothing
    End Function

    Private Function parseNormalization(text As String) As NormalizationMethods
        Dim value As NormalizationMethods

        If Not String.IsNullOrEmpty(text) AndAlso [Enum].TryParse(text, ignoreCase:=True, result:=value) Then
            Return value
        Else
            Return NormalizationMethods.Log2
        End If
    End Function

    Private Function parseMode(text As String) As InterventionMode
        Select Case If(text, "knockdown").Trim.ToLowerInvariant()
            Case "knockout", "ko"
                Return InterventionMode.Knockout
            Case "overexpress", "overexpression", "oe"
                Return InterventionMode.Overexpression
            Case "custom"
                Return InterventionMode.Custom
            Case Else
                Return InterventionMode.Knockdown
        End Select
    End Function

    Private Function parseConfidence(text As String) As Double
        Dim value As Double

        If String.IsNullOrWhiteSpace(text) Then
            Return 1
        End If
        If Double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
            Return value
        End If

        Dim mapped As Double = PriorEdge.ParseConfidenceEnum(text.Trim())

        If mapped <= 0 Then
            Return 0.1R
        Else
            Return mapped
        End If
    End Function

#End Region
End Module
