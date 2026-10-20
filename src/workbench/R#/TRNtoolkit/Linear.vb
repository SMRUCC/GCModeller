Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.CellPhenotype
Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.genomics.Model.Network.Regulons.LinearRegression
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports DataFrameRow = SMRUCC.genomics.Analysis.HTS.DataFrame.DataFrameRow
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' The linear (elastic net) gene regulatory network builder and the in silico
''' perturbation toolkit that runs on top of the fitted linear models.
''' </summary>
''' <remarks>
''' This package provides two groups of the R# api:
''' 
''' + ``elastic_net`` / ``coef``: fit a ``regulator -> target`` linear regulatory
'''   network from a gene expression matrix, and export the coefficient matrix;
''' + ``elastic_model`` / ``set_baseline`` / ``intervene`` / ``profiles_matrix``:
'''   build an in silico perturbation engine from the fitted network, condition it
'''   on a given cell context, and run the RNAi style knockdown experiments.
''' 
''' The perturbation engine implements the <c>InsilicoPerturbationExperiment</c>
''' interface, so the ``knockouts`` / ``overexpress`` / ``knockdown`` api of the
''' ``bnlearn`` package can be used on the object that is created by
''' ``elastic_model`` directly.
''' </remarks>
<Package("linear")>
<RTypeExport("elastic_net.options", GetType(ElasticNetPerturbationOptions))>
<RTypeExport("linear_grn", GetType(LinearRegression.GeneRegulatoryNetwork))>
Module Linear

    Sub Main()
        Call RInternal.generic.add("writeBin", GetType(LinearRegression.GeneRegulatoryNetwork), AddressOf saveModel)
        Call RInternal.generic.add("readBin.linear_grn", GetType(Stream), AddressOf loadModel)
    End Sub

    ''' <summary>
    ''' R# ``writeBin`` handler: persist a fitted linear GRN into the given connection.
    ''' </summary>
    ''' <param name="model">the fitted GRN model to be serialized</param>
    ''' <param name="args">the R# named argument list; the ``con`` argument supplies the output stream</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns><c>True</c> when the model has been written successfully</returns>
    Private Function saveModel(model As LinearRegression.GeneRegulatoryNetwork, args As list, env As Environment) As Object
        Dim con As Stream = args!con

        Try
            Call model.Save(con)
            Call con.Flush()
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return True
    End Function

    ''' <summary>
    ''' R# ``readBin.linear_grn`` handler: restore a linear GRN model from a connection.
    ''' </summary>
    ''' <param name="s">the stream that contains a GRN model package</param>
    ''' <param name="args">the R# named argument list (unused)</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the reconstructed <see cref="LinearRegression.GeneRegulatoryNetwork"/> object</returns>
    Private Function loadModel(s As Stream, args As list, env As Environment) As Object
        Return GRNFile.Load(s)
    End Function

    ''' <summary>
    ''' Fit a linear (elastic net) gene regulatory network from the expression matrix.
    ''' </summary>
    ''' <param name="x">the gene expression matrix, rows are genes and columns are samples</param>
    ''' <param name="candidates">
    ''' the max number of the candidate regulators for each target gene, the candidates
    ''' are picked by the absolute pearson correlation with the target gene.
    ''' </param>
    ''' <param name="maxreg">the max number of the non-zero regulation edges that each target gene can keep</param>
    ''' <param name="l1_ratio">the L1 mixing ratio of the elastic net, in the range [0, 1]</param>
    ''' <param name="alphas">the number of the alpha steps in the regularization path</param>
    ''' <param name="mutual">only keep the reversible (mutual) regulation pairs</param>
    ''' <param name="TF">
    ''' an optional gene id vector of the transcript factors, these genes are <b>always</b>
    ''' included in the candidate regulator pool (they are not affected by the
    ''' <paramref name="candidates"/> truncation). Non TF genes are still able to be the
    ''' regulators, so that the knockdown of a non TF gene still has downstream responses.
    ''' </param>
    ''' <param name="standardize">z-score the expression vectors before the fitting</param>
    ''' <param name="min_abs_r">the minimal absolute pearson correlation for a gene to be a candidate regulator</param>
    ''' <returns>
    ''' a list object with two slots: ``grn`` (the fitted <see cref="LinearRegression.GeneRegulatoryNetwork"/>)
    ''' and ``edges`` (the <see cref="RegulationEdge"/> collection).
    ''' </returns>
    <ExportAPI("elastic_net")>
    <RApiReturn(GetType(LinearRegression.GeneRegulatoryNetwork), GetType(RegulationEdge))>
    Public Function elastic_net(x As Matrix,
                                Optional candidates As Integer = 1000,
                                Optional maxreg As Integer = 10,
                                Optional l1_ratio As Double = 0.7,
                                Optional alphas As Double = 40,
                                Optional mutual As Boolean = False,
                                <RRawVectorArgument(TypeCodes.string)> Optional TF As Object = Nothing,
                                Optional standardize As Boolean = False,
                                Optional min_abs_r As Double = 0) As Object

        Dim priority As String() = Nothing

        If TF IsNot Nothing Then
            priority = CLRVector _
                .asCharacter(TF) _
                .Where(Function(s) Not String.IsNullOrEmpty(s)) _
                .Select(Function(s) s.Trim) _
                .Where(Function(s) s.Length > 0) _
                .ToArray
        End If

        Dim options As New GrnFittingOptions With {
            .silent = True,
            .method = FittingMethod.ElasticNet,
            .maxCandidates = candidates,
            .maxRegulatorsPerGene = maxreg,
            .mutualOnly = mutual,
            .standardize = standardize,
            .minAbsCorrelation = min_abs_r,
            .priorityRegulators = priority,
            .elasticNet = New ElasticNetOptions With {
                .l1Ratio = l1_ratio,
                .numberOfAlphas = alphas
            }
        }
        Dim t0 = Now

        If mutual Then
            Console.WriteLine("     >> only the mutual (reversible) regulation pairs are kept")
        End If

        If priority IsNot Nothing AndAlso priority.Length > 0 Then
            Console.WriteLine($"     >> {priority.Length} priority (TF) genes are always included in the candidate pool")
        End If

        Dim grn As LinearRegression.GeneRegulatoryNetwork = x.InferRegulatoryNetwork(options)
        Dim edges As RegulationEdge() = grn.Regulations().ToArray
        Dim failed As Integer = grn.Models.Where(Function(m) Not m.IsValid).Count()

        Console.WriteLine($"     >> fitting cost {StringFormats.ReadableElapsedTime(Now - t0)}, " &
                          $"{edges.Length} regulation edges, {failed} failed model(s)")

        Console.WriteLine("     >> top regulons (by the number of target genes):")

        For Each item In edges.RegulonSizes().Take(8)
            Console.WriteLine($"        {item.regulator} ({item.size} targets)")
        Next

        Console.WriteLine("     >> top regulation edges (by |beta|):")

        For Each edge As RegulationEdge In edges.TopEdges(10)
            Console.WriteLine($"        {edge.interaction}: {edge.regulator} -> {edge.target}, " &
                              $"beta={edge.weight:G4}, alpha={edge.alpha:G4}, R2={edge.r2:F3}")
        Next

        Dim significant As Integer = edges.Count(Function(e) e.r2 > 0.9)

        Console.WriteLine($"     >> {significant} edges come from the gene models with R2 > 0.9")

        Return New list(slot("grn") = grn, slot("edges") = edges)
    End Function

    ''' <summary>
    ''' Export the coefficient matrix of the fitted network.
    ''' </summary>
    ''' <param name="grn">the fitted linear GRN model</param>
    ''' <returns>
    ''' a matrix whose rows are the target genes and columns are the regulators,
    ''' an element is the regression coefficient of the ``regulator -> target`` edge.
    ''' </returns>
    <ExportAPI("coef")>
    Public Function coef(grn As LinearRegression.GeneRegulatoryNetwork) As Matrix
        Return grn.CoefMatrix
    End Function

    ''' <summary>
    ''' Build the in silico perturbation engine from a fitted linear GRN model.
    ''' </summary>
    ''' <param name="grn">
    ''' the fitted <see cref="LinearRegression.GeneRegulatoryNetwork"/> object, it is the
    ''' ``grn`` slot of the ``elastic_net`` result.
    ''' </param>
    ''' <param name="opts">the perturbation options, a default configuration is used when this argument is not given</param>
    ''' <param name="strict">
    ''' in the strict mode an error will be thrown when the target gene of the perturbation
    ''' is not found in the model; in the non strict mode a warning will be printed and the
    ''' wildtype expression will be returned with the ``Undefined`` flag marked as TRUE.
    ''' For a batch run of hundreds of the perturbations it is recommended to disable it and
    ''' to check the ``MissingGenes`` field of the result.
    ''' </param>
    ''' <param name="scale">
    ''' the numeric space of the expression matrix that was used to fit the network:
    ''' ``linear`` (default) or ``log2cpm`` (i.e. ``log2(x + 1)``). For a log transformed
    ''' matrix this argument is <b>required</b>, otherwise the knockdown magnitude will be
    ''' over-estimated by about 2~3 times.
    ''' </param>
    ''' <param name="propagation">
    ''' ``cascade`` (default) propagates the perturbation to the steady state,
    ''' ``onehop`` only keeps the direct effects,
    ''' ``delta`` propagates the change only (recommended for the context conditioned
    ''' prediction: it is numerically identical to "mutant minus the matched control").
    ''' </param>
    ''' <param name="steps">the max cascade iteration steps, a value that is less than 1 means "use the default"</param>
    ''' <param name="damping">the damping factor of the cascade iteration, a value that is less than or equals to 0 means "use the default"</param>
    ''' <param name="knockdown_residual">
    ''' the fraction of the wildtype expression that is kept after a knockdown,
    ''' a negative value means "use the default" (0.2, i.e. about 80% knockdown).
    ''' </param>
    ''' <param name="env">the R# runtime environment object</param>
    ''' <returns>
    ''' an <see cref="ElasticNet"/> engine which implements the
    ''' <see cref="InsilicoPerturbationExperiment"/> interface.
    ''' </returns>
    <ExportAPI("elastic_model")>
    <RApiReturn(GetType(ElasticNet))>
    Public Function elastic_model(grn As LinearRegression.GeneRegulatoryNetwork,
                                  Optional opts As ElasticNetPerturbationOptions = Nothing,
                                  Optional strict As Boolean = True,
                                  Optional scale As String = "linear",
                                  Optional propagation As String = "cascade",
                                  Optional steps As Integer = -1,
                                  Optional damping As Double = -1,
                                  Optional knockdown_residual As Double = -1,
                                  Optional env As Environment = Nothing) As Object

        Dim config As ElasticNetPerturbationOptions

        If opts Is Nothing Then
            config = New ElasticNetPerturbationOptions()
        Else
            config = opts.Clone()
        End If

        Select Case If(scale, "linear").Trim.ToLowerInvariant()
            Case "linear"
                config.Scale = PerturbationScale.Linear
            Case "log2cpm", "log2plus1", "logcpm", "log2", "log"
                config.Scale = PerturbationScale.Log2Plus1
            Case Else
                Return RInternal.debug.stop($"unknown expression scale '{scale}', expected 'linear' or 'log2cpm'", env)
        End Select

        Select Case If(propagation, "cascade").Trim.ToLowerInvariant()
            Case "cascade"
                config.Propagation = PropagationMode.Cascade
            Case "onehop", "one_hop", "direct"
                config.Propagation = PropagationMode.OneHop
            Case "delta"
                config.Propagation = PropagationMode.Delta
            Case Else
                Return RInternal.debug.stop($"unknown propagation mode '{propagation}', expected 'cascade', 'onehop' or 'delta'", env)
        End Select

        If steps > 0 Then
            config.Steps = steps
        End If
        If damping > 0 Then
            config.Damping = damping
        End If
        If knockdown_residual >= 0 Then
            config.KnockdownResidual = knockdown_residual
        End If

        Try
            Call config.Validate()
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return New ElasticNet(grn, config) With {
            .Strict = strict
        }
    End Function

    ''' <summary>
    ''' Get the gene ids of the fitted network, in the row order of the expression matrix.
    ''' </summary>
    ''' <param name="model">the perturbation engine that is created via ``elastic_model``</param>
    ''' <returns>a character vector of the gene ids</returns>
    <ExportAPI("gene_names")>
    <RApiReturn(GetType(String))>
    Public Function gene_names(model As ElasticNet) As Object
        Return DirectCast(model.Network.Genes.Clone(), String())
    End Function

    ''' <summary>
    ''' Get the currently effective wildtype (baseline) expression vector of the model.
    ''' </summary>
    ''' <param name="model">the perturbation engine that is created via ``elastic_model``</param>
    ''' <returns>
    ''' a numeric vector, one value per gene, in the same order as ``gene_names``.
    ''' Before ``set_baseline`` is called this is the sample mean of the training matrix.
    ''' </returns>
    <ExportAPI("wildtype_means")>
    <RApiReturn(GetType(Double))>
    Public Function wildtype_means(model As ElasticNet) As Object
        Return CType(model.Baseline.Clone(), Double())
    End Function

    ''' <summary>
    ''' Get the implied library size of every training sample of the fitted network.
    ''' </summary>
    ''' <param name="model">the perturbation engine that is created via ``elastic_model``</param>
    ''' <param name="pseudo">the pseudo count offset that was used by the log transformation, default 1</param>
    ''' <returns>
    ''' a numeric vector, one value per training sample, i.e. ``sum(2^x - pseudo)`` of the
    ''' expression matrix that was used to fit the network.
    ''' </returns>
    ''' <remarks>
    ''' This vector is the bridge between an external raw count matrix and the model space:
    ''' normalize the counts with ``y = counts / sum(counts) * median(library_sizes)`` and
    ''' then apply ``x = log2(y + pseudo)``.
    ''' </remarks>
    <ExportAPI("library_sizes")>
    <RApiReturn(GetType(Double))>
    Public Function library_sizes(model As ElasticNet, Optional pseudo As Double = 1.0) As Object
        Return model.ImpliedLibrarySizes(pseudo)
    End Function

    ''' <summary>
    ''' Set the wildtype baseline (the cell context) of the perturbation engine.
    ''' </summary>
    ''' <param name="model">the perturbation engine that is created via ``elastic_model``</param>
    ''' <param name="genes">
    ''' the gene ids of the given expression profile, the matching against the model is
    ''' case insensitive and the genes that are not modelled are ignored.
    ''' </param>
    ''' <param name="values">
    ''' the baseline expression values, they must be in the <b>model space</b> (i.e. the same
    ''' transformation that was used to fit the network, for example ``log2(CPM + 1)``).
    ''' </param>
    ''' <param name="env">the R# runtime environment object</param>
    ''' <returns>the number of the genes that have been matched and written</returns>
    <ExportAPI("set_baseline")>
    Public Function set_baseline(model As ElasticNet,
                                 <RRawVectorArgument(TypeCodes.string)> genes As Object,
                                 <RRawVectorArgument(TypeCodes.double)> values As Object,
                                 Optional env As Environment = Nothing) As Object

        Dim ids As String() = CLRVector.asCharacter(genes)
        Dim vals As Double() = CLRVector.asNumeric(values)

        If ids.Length <> vals.Length Then
            Return RInternal.debug.stop(
                $"the size of the genes vector ({ids.Length}) does not match the size of the values vector ({vals.Length})", env)
        End If

        Try
            Return model.SetBaseline(ids, vals)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Run one in silico perturbation experiment and return the raw result object.
    ''' </summary>
    ''' <param name="model">the perturbation engine that is created via ``elastic_model``</param>
    ''' <param name="gene">the target gene id</param>
    ''' <param name="mode">``knockout``, ``knockdown``, ``overexpress`` or ``custom``</param>
    ''' <param name="value">
    ''' the absolute expression value in the model space, only used by the ``custom`` mode.
    ''' </param>
    ''' <param name="env">the R# runtime environment object</param>
    ''' <returns>
    ''' an <see cref="InterventionResult"/> object; use ``as.list`` to access the fields
    ''' (the ``$`` indexing is not supported on this object).
    ''' </returns>
    <ExportAPI("intervene")>
    <RApiReturn(GetType(InterventionResult))>
    Public Function intervene(model As ElasticNet,
                              gene As String,
                              Optional mode As String = "knockout",
                              Optional value As Double = 0,
                              Optional env As Environment = Nothing) As Object

        Dim spec As InterventionSpec = Nothing

        Select Case If(mode, "knockout").Trim.ToLowerInvariant()
            Case "knockout", "ko"
                spec = NewSpec(gene, InterventionMode.Knockout)
            Case "knockdown", "kd"
                spec = NewSpec(gene, InterventionMode.Knockdown)
            Case "overexpress", "overexpression", "oe"
                spec = NewSpec(gene, InterventionMode.Overexpression)
            Case "custom"
                spec = NewSpec(gene, InterventionMode.Custom)
                spec.Value = value
            Case Else
                Return RInternal.debug.stop($"unknown intervention mode '{mode}'", env)
        End Select

        Try
            Return model.Intervene(spec)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try
    End Function

    ''' <summary>
    ''' Run the perturbation experiment for a set of genes and collect the profiles into a matrix.
    ''' </summary>
    ''' <param name="model">the perturbation engine that is created via ``elastic_model``</param>
    ''' <param name="genes">the target gene ids, one column will be generated for each of them</param>
    ''' <param name="mode">``knockout``, ``knockdown`` (default) or ``overexpress``</param>
    ''' <param name="env">the R# runtime environment object</param>
    ''' <returns>
    ''' a matrix of the ``[gene x condition]`` mutant means. The genes that can not be found in
    ''' the model are skipped, so the column number equals the number of the matched genes.
    ''' </returns>
    ''' <remarks>
    ''' This api is the memory friendly replacement of the ``make_exports`` api for a batch run:
    ''' it does not generate the ten difference analysis tables for every condition.
    ''' </remarks>
    <ExportAPI("profiles_matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function profiles_matrix(model As ElasticNet,
                                    <RRawVectorArgument(TypeCodes.string)> genes As Object,
                                    Optional mode As String = "knockdown",
                                    Optional env As Environment = Nothing) As Object

        Dim targets As String() = CLRVector.asCharacter(genes)

        If targets Is Nothing OrElse targets.Length = 0 Then
            Return RInternal.debug.stop("at least one target gene is required!", env)
        End If

        Dim geneNames As String() = model.Network.Genes
        Dim n As Integer = geneNames.Length
        Dim block As New List(Of Double())
        Dim conditions As New List(Of String)
        Dim missing As New List(Of String)

        For Each name As String In targets
            Dim spec As InterventionSpec

            Select Case If(mode, "knockdown").Trim.ToLowerInvariant()
                Case "knockout", "ko"
                    spec = NewSpec(name, InterventionMode.Knockout)
                Case "overexpress", "overexpression", "oe"
                    spec = NewSpec(name, InterventionMode.Overexpression)
                Case Else
                    spec = NewSpec(name, InterventionMode.Knockdown)
            End Select

            Try
                Dim result As InterventionResult = model.Intervene(spec)

                If result Is Nothing OrElse result.Undefined OrElse result.MutantMeans Is Nothing Then
                    Call missing.Add(name)
                Else
                    Call block.Add(CType(result.MutantMeans.Clone(), Double()))
                    Call conditions.Add(name)
                End If
            Catch ex As MissingMemberException
                Call missing.Add(name)
            End Try
        Next

        If block.Count = 0 Then
            Return RInternal.debug.stop(
                $"none of the given {targets.Length} genes can be found in the model, please check the gene id space!", env)
        End If

        If missing.Count > 0 Then
            Console.WriteLine($"     >> {missing.Count} of the {targets.Length} target genes are missing from the model and have been skipped:")
            Console.WriteLine($"        {String.Join(", ", missing.Take(20))}{If(missing.Count > 20, ", ...", "")}")
        End If

        Dim rows As New List(Of DataFrameRow)

        For i As Integer = 0 To n - 1
            Dim values As Double() = New Double(block.Count - 1) {}

            For k As Integer = 0 To block.Count - 1
                values(k) = block(k)(i)
            Next

            Call rows.Add(New DataFrameRow With {
                .geneID = geneNames(i),
                .experiments = values
            })
        Next

        Return New Matrix With {
            .tag = $"elastic_net.{mode}",
            .sampleID = conditions.ToArray,
            .expression = rows.ToArray
        }
    End Function

    ''' <summary>
    ''' Run the perturbation experiment for a set of genes and save the profiles as a csv file.
    ''' </summary>
    ''' <param name="model">the perturbation engine that is created via ``elastic_model``</param>
    ''' <param name="genes">the target gene ids</param>
    ''' <param name="file">the output csv file, its folder will be created automatically</param>
    ''' <param name="mode">``knockout``, ``knockdown`` (default) or ``overexpress``</param>
    ''' <param name="env">the R# runtime environment object</param>
    ''' <returns>TRUE when the file has been written successfully</returns>
    ''' <remarks>
    ''' The first column is the gene id, the second column is the control (wildtype) profile,
    ''' and the following columns are the mutant profiles of the given perturbations.
    ''' </remarks>
    <ExportAPI("export_profiles")>
    <RApiReturn(TypeCodes.boolean)>
    Public Function export_profiles(model As ElasticNet,
                                    <RRawVectorArgument(TypeCodes.string)> genes As Object,
                                    file As String,
                                    Optional mode As String = "knockdown",
                                    Optional env As Environment = Nothing) As Object

        If String.IsNullOrEmpty(file) Then
            Return RInternal.debug.stop("a valid output file path is required!", env)
        End If

        Dim profiles As Matrix = TryCast(profiles_matrix(model, genes, mode, env), Matrix)

        If profiles Is Nothing Then
            Return RInternal.debug.stop("the perturbation profiles could not be generated!", env)
        End If

        Dim folder As String = Path.GetDirectoryName(Path.GetFullPath(file))

        If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
            Call Directory.CreateDirectory(folder)
        End If

        Dim control As Double() = CType(model.Baseline.Clone(), Double())
        Dim invariant As Globalization.CultureInfo = Globalization.CultureInfo.InvariantCulture

        Using writer As New StreamWriter(file, False, New UTF8Encoding(False))
            Dim header As New List(Of String) From {"Gene", "control"}

            Call header.AddRange(profiles.sampleID)
            Call writer.WriteLine(String.Join(",", header))

            For i As Integer = 0 To profiles.expression.Length - 1
                Dim row As DataFrameRow = profiles.expression(i)
                Dim line As New List(Of String) From {row.geneID, control(i).ToString("R", invariant)}

                For Each value As Double In row.experiments
                    Call line.Add(value.ToString("R", invariant))
                Next

                Call writer.WriteLine(String.Join(",", line))
            Next
        End Using

        Return True
    End Function

    ''' <summary>
    ''' Create an intervention spec for the given gene.
    ''' </summary>
    ''' <param name="gene">the target gene id</param>
    ''' <param name="mode">the intervention mode</param>
    ''' <returns></returns>
    Private Function NewSpec(gene As String, mode As InterventionMode) As InterventionSpec
        Return New InterventionSpec With {
            .GeneName = gene,
            .Mode = mode
        }
    End Function

End Module
