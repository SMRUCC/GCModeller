Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace Search

    ''' <summary>
    ''' The orchestration facade of the adaptation topology search: it runs the
    ''' enumerate, sample, simulate, assess and analyze pipeline of the paper
    ''' and writes the product tables.
    ''' (适应拓扑搜索的编排门面：运行论文的枚举、采样、仿真、评估与分析流水线并写入产物表格)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The pipeline mirrors the Figure 1D workflow of the paper:
    ''' </para>
    ''' <list type="number">
    ''' <item> enumerate the connected topologies of the scope; </item>
    ''' <item> reload the finished topologies from the checkpoints when a resume is requested; </item>
    ''' <item> evaluate every pending topology: sample its parameters, simulate every circuit and aggregate the robustness; </item>
    ''' <item> analyze the results: assign the motif flags, compute the Venn counts, cluster the functional topologies by their Hamming distance and run the design table experiment; </item>
    ''' <item> write the product tables and the report. </item>
    ''' </list>
    ''' <para>
    ''' The evaluation is parallel over the topologies when there are at least
    ''' as many pending topologies as threads, and parallel over the circuits of
    ''' a topology otherwise, so that both a wide search of many topologies and
    ''' a deep search of a few topologies use the machine. The random seeds are
    ''' derived deterministically from the base seed and the topology
    ''' identifier, so a run is reproducible and a resumed run produces exactly
    ''' the same numbers as an uninterrupted one.
    ''' </para>
    ''' </remarks>
    Public Class TopologySearch

        ''' <summary>
        ''' The maximum number of topologies that the Hamming clustering is
        ''' computed on. The clustering is cubic in the topology count, so a very
        ''' large functional set is reduced to its top members by the Q value.
        ''' (Hamming 聚类计算的最大拓扑数；聚类复杂度是拓扑数的三次方，过大的功能集合按 Q 值取前若干)
        ''' </summary>
        Public Const MaxClusterTopologies As Integer = 3000

        ''' <summary>
        ''' Run the topology search pipeline.
        ''' (运行拓扑搜索流水线)
        ''' </summary>
        ''' <param name="options">the search options</param>
        ''' <param name="progress">the optional progress callback</param>
        ''' <param name="outputDir">
        ''' the output directory of the product tables, an empty string skips
        ''' writing the products and only returns the report
        ''' </param>
        Public Shared Function Run(options As TopologySearchOptions,
                                   Optional progress As Action(Of SearchProgress) = Nothing,
                                   Optional outputDir As String = "") As SearchReport
            If options Is Nothing Then
                Throw New ArgumentNullException(NameOf(options))
            End If

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim jobs As Integer = options.EffectiveParallelism()
            Dim report As New SearchReport With {.Parallelism = jobs}
            Dim checkpointing As Boolean = Not String.IsNullOrEmpty(options.CheckpointDir)

            ' ---- 1. the topology scope ----
            Dim scope As List(Of Model.NetworkTopology) = Enumeration.TopologyEnumerator _
                .EnumerateUpTo(options.MaxLinks) _
                .Take(If(options.MaxTopologies > 0, options.MaxTopologies, Integer.MaxValue)) _
                .ToList()
            Dim scopeIds As New HashSet(Of Integer)(scope.Select(Function(t) t.Id))

            ' ---- 2. the checkpoints of a resumed run ----
            Dim results As New ConcurrentDictionary(Of Integer, Assessment.TopologyResult)()
            Dim circuitRecords As New ConcurrentBag(Of Assessment.CircuitRecord)()
            Dim resumed As Integer = 0

            If checkpointing AndAlso options.ResumeFromCheckpoints Then
                For Each loaded In IO.CheckpointStore.LoadAll(options.CheckpointDir, options.SThreshold, options.PThreshold)
                    ' a checkpoint is only reused when it belongs to the current
                    ' scope and was produced with the same sample count
                    If scopeIds.Contains(loaded.result.TopologyId) AndAlso loaded.result.Simulated = options.Samples Then
                        Call results.TryAdd(loaded.result.TopologyId, loaded.result)
                        Call KeepRecords(circuitRecords, loaded.records, options.KeepAllCircuits)
                        resumed += 1
                    End If
                Next
            End If

            Dim pending As List(Of Model.NetworkTopology) = scope _
                .Where(Function(t) Not results.ContainsKey(t.Id)) _
                .ToList()

            ' ---- 3. the evaluation of the pending topologies ----
            Dim doneCount As Integer = 0
            Dim functionalSoFar As Integer = 0
            Dim progressLock As New Object()

            Dim evaluateOne As Action(Of Model.NetworkTopology) =
                Sub(topology As Model.NetworkTopology)
                    Dim seed As Integer = Sampling.ParameterSampler.SeedOf(topology.Id, options.Seed)
                    ' the inner loop is only parallelized when the outer loop is
                    ' sequential, otherwise the threads would oversubscribe
                    Dim innerJobs As Integer = 0
                    Dim evaluated = Assessment.TopologyEvaluator.Evaluate(
                        topology, options.Samples, seed, options.I1, options.I2,
                        options.SimulationOptions, options.SThreshold, options.PThreshold, innerJobs)

                    Call results.TryAdd(topology.Id, evaluated.result)
                    Call KeepRecords(circuitRecords, evaluated.records, options.KeepAllCircuits)

                    If checkpointing Then
                        Call IO.CheckpointStore.Save(options.CheckpointDir, topology, evaluated.records)
                    End If

                    SyncLock progressLock
                        doneCount += 1
                        functionalSoFar += evaluated.result.Functional

                        If progress IsNot Nothing Then
                            Call progress(New SearchProgress With {
                                .Completed = doneCount,
                                .Total = pending.Count,
                                .CurrentTopologyId = topology.Id,
                                .FunctionalSoFar = functionalSoFar,
                                .Elapsed = watch.Elapsed
                            })
                        End If
                    End SyncLock
                End Sub

            ' the ODE solver reports its internal Newton failures on the
            ' standard error stream, which would flood the console of a large
            ' run, so the diagnostics are suppressed during the evaluation
            Dim errorWriter As TextWriter = Console.Error

            If options.SuppressSolverDiagnostics Then
                Call Console.SetError(TextWriter.Null)
            End If

            Try
                If pending.Count >= jobs Then
                    ' wide search: parallel over the topologies
                    Parallel.ForEach(pending,
                                     New ParallelOptions With {.MaxDegreeOfParallelism = jobs},
                                     Sub(topology)
                                         Call evaluateOne(topology)
                                     End Sub)
                Else
                    ' deep search: sequential over the topologies, the circuits
                    ' of a topology are parallelized inside the evaluator
                    For Each topology As Model.NetworkTopology In pending
                        Dim evaluated = Assessment.TopologyEvaluator.Evaluate(
                            topology, options.Samples,
                            Sampling.ParameterSampler.SeedOf(topology.Id, options.Seed),
                            options.I1, options.I2, options.SimulationOptions,
                            options.SThreshold, options.PThreshold, jobs)

                        Call results.TryAdd(topology.Id, evaluated.result)
                        Call KeepRecords(circuitRecords, evaluated.records, options.KeepAllCircuits)

                        If checkpointing Then
                            Call IO.CheckpointStore.Save(options.CheckpointDir, topology, evaluated.records)
                        End If

                        SyncLock progressLock
                            doneCount += 1
                            functionalSoFar += evaluated.result.Functional

                            If progress IsNot Nothing Then
                                Call progress(New SearchProgress With {
                                    .Completed = doneCount,
                                    .Total = pending.Count,
                                    .CurrentTopologyId = topology.Id,
                                    .FunctionalSoFar = functionalSoFar,
                                    .Elapsed = watch.Elapsed
                                })
                            End If
                        End SyncLock
                    Next
                End If
            Finally
                If options.SuppressSolverDiagnostics Then
                    Call Console.SetError(errorWriter)
                End If
            End Try

            ' ---- 4. the post processing ----
            Dim allResults As List(Of Assessment.TopologyResult) = results _
                .Values _
                .OrderBy(Function(r) r.TopologyId) _
                .ToList()

            Call Analysis.MotifAnalyzer.AssignMotifFlags(allResults)

            Dim overRepresented As List(Of Assessment.TopologyResult) = Analysis.RobustnessAnalyzer.OverRepresented(allResults, options.QThreshold)
            Dim functionalSet As List(Of Assessment.TopologyResult) = Analysis.RobustnessAnalyzer.FunctionalTopologies(allResults)

            report.VennOverRepresented = Analysis.MotifAnalyzer.VennCounts(overRepresented)
            report.VennFunctional = Analysis.MotifAnalyzer.VennCounts(functionalSet)

            ' the Hamming clustering of the functional topologies
            If options.ClusterCutoff >= 0 AndAlso functionalSet.Count > 0 Then
                Dim clusterSet As List(Of Assessment.TopologyResult) = functionalSet

                If clusterSet.Count > MaxClusterTopologies Then
                    clusterSet = clusterSet.Take(MaxClusterTopologies).ToList()
                End If

                Dim clusterTopologies As Model.NetworkTopology() = clusterSet _
                    .Select(Function(r) Model.NetworkTopology.FromId(r.TopologyId)) _
                    .ToArray()
                Dim clustered = Analysis.TopologyClustering.Cluster(clusterTopologies, options.ClusterCutoff)

                For i As Integer = 0 To clusterTopologies.Length - 1
                    results(clusterTopologies(i).Id).ClusterLabel = clustered.labels(i)
                Next

                report.ClusterCount = clustered.clusterCount
                report.Silhouette = Analysis.TopologyClustering.Silhouette(clusterTopologies, clustered.labels)
            End If

            ' the design table experiment of the motif augmentations
            If options.RunDesignTable Then
                If options.SuppressSolverDiagnostics Then
                    Call Console.SetError(TextWriter.Null)
                End If

                Try
                    report.DesignTable = Analysis.RobustnessAnalyzer _
                        .DesignTableExperiment(options.EffectiveDesignTableSamples(), options.Seed,
                                               options.I1, options.I2, options.SimulationOptions,
                                               options.SThreshold, options.PThreshold, jobs) _
                        .ToList()
                Finally
                    If options.SuppressSolverDiagnostics Then
                        Call Console.SetError(errorWriter)
                    End If
                End Try
            End If

            ' ---- 5. the summary of the report ----
            report.TopologiesEvaluated = pending.Count
            report.TopologiesResumed = resumed
            report.TopologiesTotal = allResults.Count
            report.CircuitsSimulated = allResults.Sum(Function(r) CLng(r.Simulated))
            report.IllBehavedCircuits = allResults.Sum(Function(r) CLng(r.IllBehaved))
            report.FunctionalTopologies = functionalSet.Count
            report.OverRepresentedTopologies = overRepresented.Count
            report.TopTopologies = allResults _
                .OrderByDescending(Function(r) r.Functional) _
                .ThenBy(Function(r) r.TopologyId) _
                .Take(20) _
                .ToList()
            report.Elapsed = watch.Elapsed
            report.Configuration = FormatConfiguration(options, jobs)

            ' ---- 6. the product tables ----
            If Not String.IsNullOrEmpty(outputDir) Then
                Call WriteProducts(outputDir, options, report, allResults, circuitRecords, overRepresented, functionalSet)
            End If

            Return report
        End Function

        ''' <summary>
        ''' Collect the circuit records that the reports keep: every well
        ''' behaved circuit when the full records are requested, otherwise only
        ''' the functional circuits.
        ''' (收集报告保留的电路记录：请求全部记录时保留每个良态电路，否则只保留功能电路)
        ''' </summary>
        Private Shared Sub KeepRecords(target As ConcurrentBag(Of Assessment.CircuitRecord),
                                records As List(Of Assessment.CircuitRecord),
                                keepAll As Boolean)
            If records Is Nothing Then
                Return
            End If

            If keepAll Then
                For Each record In records.Where(Function(r) Not r.IsIllBehaved)
                    Call target.Add(record)
                Next
            Else
                For Each record In records.Where(Function(r) r.IsFunctional)
                    Call target.Add(record)
                Next
            End If
        End Sub

        ''' <summary>
        ''' Write the product tables of the search and the human readable report.
        ''' (写入搜索的产物表格与可读报告)
        ''' </summary>
        Private Shared Sub WriteProducts(outputDir As String,
                                  options As TopologySearchOptions,
                                  report As SearchReport,
                                  allResults As List(Of Assessment.TopologyResult),
                                  circuitRecords As ConcurrentBag(Of Assessment.CircuitRecord),
                                  overRepresented As List(Of Assessment.TopologyResult),
                                  functionalSet As List(Of Assessment.TopologyResult))
            If Not Directory.Exists(outputDir) Then
                Call Directory.CreateDirectory(outputDir)
            End If

            Dim paths As New Dictionary(Of String, String) From {
                {"topologies", Path.Combine(outputDir, "topologies.csv")},
                {"functional_circuits", Path.Combine(outputDir, "functional_circuits.csv")},
                {"clusters", Path.Combine(outputDir, "clusters.csv")},
                {"venn_summary", Path.Combine(outputDir, "venn_summary.csv")},
                {"design_table", Path.Combine(outputDir, "design_table.csv")},
                {"report", Path.Combine(outputDir, "report.txt")}
            }

            Call IO.CircuitsCsvWriter.WriteTopologies(paths("topologies"), allResults, options.QThreshold)
            Call IO.CircuitsCsvWriter.WriteFunctionalCircuits(paths("functional_circuits"), circuitRecords)
            Call IO.CircuitsCsvWriter.WriteClusters(paths("clusters"), allResults)
            Call IO.CircuitsCsvWriter.WriteVenn(paths("venn_summary"), {
                ("over_represented (Q > " & options.QThreshold & ")", report.VennOverRepresented),
                ("functional (Q > 0)", report.VennFunctional)
            })
            Call IO.CircuitsCsvWriter.WriteDesignTable(paths("design_table"), report.DesignTable)

            Dim text As String = BuildReportText(options, report, overRepresented, functionalSet)

            Call File.WriteAllText(paths("report"), text, New UTF8Encoding(False))

            report.OutputDir = outputDir
            report.ArtifactPaths = paths
        End Sub

        ''' <summary>
        ''' Format the configuration of the search for the report header.
        ''' (格式化搜索配置作为报告头部)
        ''' </summary>
        Private Shared Function FormatConfiguration(options As TopologySearchOptions, jobs As Integer) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"topology scope: at most {options.MaxLinks} links" &
                               If(options.MaxTopologies > 0, $" (limited to the first {options.MaxTopologies} topologies)", " (complete space)"))
            Call sb.AppendLine($"samples per topology: {options.Samples} (base seed {options.Seed})")
            Call sb.AppendLine($"step protocol: I1 = {options.I1} -> I2 = {options.I2} (relative change {Assessment.FunctionalAssessor.Format(Assessment.FunctionalAssessor.InputChange(options.I1, options.I2))})")
            Call sb.AppendLine($"functional thresholds: S > {options.SThreshold}, P > {options.PThreshold}")
            Call sb.AppendLine($"over representation threshold: Q > {options.QThreshold}")
            Call sb.AppendLine($"simulation: tEnd = {options.SimulationOptions.TEnd}, rtol = {options.SimulationOptions.RelativeTolerance}, atol = {options.SimulationOptions.AbsoluteTolerance}")
            Call sb.AppendLine($"parallelism: {jobs}")

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Build the human readable report text.
        ''' (构建可读报告文本)
        ''' </summary>
        Private Shared Function BuildReportText(options As TopologySearchOptions,
                                         report As SearchReport,
                                         overRepresented As List(Of Assessment.TopologyResult),
                                         functionalSet As List(Of Assessment.TopologyResult)) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine("=== Defining Network Topologies that Can Achieve Biochemical Adaptation ===")
            Call sb.AppendLine("topology search report")
            Call sb.AppendLine($"generated (UTC): {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}")
            Call sb.AppendLine()
            Call sb.AppendLine("configuration:")
            Call sb.Append(report.Configuration)
            Call sb.AppendLine()
            Call sb.AppendLine("summary:")
            Call sb.AppendLine($"  topologies covered: {report.TopologiesTotal} (evaluated {report.TopologiesEvaluated}, resumed from checkpoints {report.TopologiesResumed})")
            Call sb.AppendLine($"  circuits simulated: {report.CircuitsSimulated} (ill behaved {report.IllBehavedCircuits})")
            Call sb.AppendLine($"  functional topologies (Q > 0): {report.FunctionalTopologies}")
            Call sb.AppendLine($"  over represented topologies (Q > {options.QThreshold}): {report.OverRepresentedTopologies}")
            Call sb.AppendLine($"  elapsed: {report.Elapsed}")
            Call sb.AppendLine()
            Call sb.AppendLine("motif Venn statistics:")
            Call sb.AppendLine($"  over represented set: total={report.VennOverRepresented.total} nfblb_only={report.VennOverRepresented.nfblbOnly} ifflp_only={report.VennOverRepresented.ifflpOnly} both={report.VennOverRepresented.both} neither={report.VennOverRepresented.neither}")
            Call sb.AppendLine($"  functional set:       total={report.VennFunctional.total} nfblb_only={report.VennFunctional.nfblbOnly} ifflp_only={report.VennFunctional.ifflpOnly} both={report.VennFunctional.both} neither={report.VennFunctional.neither}")

            If report.VennOverRepresented.neither > 0 Then
                Call sb.AppendLine($"  WARNING: {report.VennOverRepresented.neither} over represented topologies own neither the NFBLB nor the IFFLP motif.")
            End If

            Call sb.AppendLine()

            If report.ClusterCount >= 0 Then
                Call sb.AppendLine($"Hamming clustering: {report.ClusterCount} clusters, silhouette = {report.Silhouette.ToString("G4", CultureInfo.InvariantCulture)} (cutoff {options.ClusterCutoff})")
            Else
                Call sb.AppendLine("Hamming clustering: disabled")
            End If

            Call sb.AppendLine()

            If report.DesignTable.Count > 0 Then
                Call sb.AppendLine($"design table (Q at {options.EffectiveDesignTableSamples()} samples per network):")

                For Each row In report.DesignTable
                    Call sb.AppendLine($"  {row.Name,-24} links={row.LinkCount} Q={row.Q} fraction={row.Fraction.ToString("G4", CultureInfo.InvariantCulture)}")
                Next
            End If

            Call sb.AppendLine()
            Call sb.AppendLine("top topologies by Q:")

            If report.TopTopologies.Count = 0 Then
                Call sb.AppendLine("  (none)")
            Else
                For Each result In report.TopTopologies
                    Call sb.AppendLine($"  [{result.TopologyId}] {result.Signs} links={result.LinkCount} Q={result.Functional} nfblb={result.HasNFBLB} ifflp={result.HasIFFLP}")
                Next
            End If

            Call sb.AppendLine()
            Call sb.AppendLine("artifacts:")

            For Each pair In report.ArtifactPaths
                Call sb.AppendLine($"  {pair.Key}: {pair.Value}")
            Next

            Return sb.ToString()
        End Function

    End Class

End Namespace
