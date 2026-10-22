Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.Linq
Imports Microsoft.VisualBasic.DataMining.Hamming
Imports SMRUCC.genomics.Analysis.Circuits.Analysis
Imports SMRUCC.genomics.Analysis.Circuits.Assessment
Imports SMRUCC.genomics.Analysis.Circuits.Enumeration
Imports SMRUCC.genomics.Analysis.Circuits.Model
Imports SMRUCC.genomics.Analysis.Circuits.Sampling
Imports SMRUCC.genomics.Analysis.Circuits.Search
Imports SMRUCC.genomics.Analysis.Circuits.Simulation

Module Program

    ''' <summary>The entry point of the circuits command line interface.</summary>
    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        If args.Length = 0 Then
            Call PrintUsage()
            Return
        End If

        Dim command As String = args(0).ToLowerInvariant()

        Select Case command
            Case "stats"
                Call CommandStats()
            Case "motif"
                Call CommandMotif()
            Case "mechanism"
                Call CommandMechanism()
            Case "search"
                Call CommandSearch(args.Skip(1).ToArray())
            Case "selftest"
                Dim samples As Integer = 100
                Dim parsed = ParseOptions(args.Skip(1).ToArray())

                If parsed.ContainsKey("samples") Then
                    Integer.TryParse(parsed("samples"), samples)
                End If

                Dim failures As Integer = SelfTest.Run(samples)

                Console.WriteLine()
                Console.WriteLine("selftest finished with {0} failure(s), exit code {1}.", failures, failures)

                Environment.ExitCode = failures
            Case Else
                Console.WriteLine("unknown command: {0}", command)
                Call PrintUsage()
                Environment.ExitCode = 2
        End Select
    End Sub

    Private Sub PrintUsage()
        Console.WriteLine("adaptation topology search CLI")
        Console.WriteLine()
        Console.WriteLine("usage:")
        Console.WriteLine("  stats                                     enumerate the topology space and print the counts")
        Console.WriteLine("  motif                                     run the motif detection on the canonical networks")
        Console.WriteLine("  mechanism                                 verify the integral control and proportioner mechanisms")
        Console.WriteLine("  search [options]                          run the topology search pipeline")
        Console.WriteLine("    --max-links N      the maximum link count of the searched topologies (default 9)")
        Console.WriteLine("    --topologies N     the maximum number of topologies (default 0 = all)")
        Console.WriteLine("    --samples N        the sampled parameter sets per topology (default 10000)")
        Console.WriteLine("    --seed N           the base seed (default 42)")
        Console.WriteLine("    --q N              the over representation threshold (default 10)")
        Console.WriteLine("    --jobs N           the thread count (default = processor count)")
        Console.WriteLine("    --cluster-cutoff X the Hamming clustering cutoff (default 3, negative disables)")
        Console.WriteLine("    --checkpoints DIR  the checkpoint directory (default none)")
        Console.WriteLine("    --no-design-table  skip the design table experiment")
        Console.WriteLine("    --out DIR          the output directory of the products (default ./circuits_results)")
        Console.WriteLine("  selftest [--samples N]                   run the paper anchor assertion suite (default 100 samples)")
    End Sub

    Private Function ParseOptions(args As String()) As Dictionary(Of String, String)
        Dim options As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To args.Length - 2
            If args(i).StartsWith("--") Then
                options(args(i).Substring(2)) = args(i + 1)
            End If
        Next

        Return options
    End Function

    ' ------------------------------------------------------------------
    ' stats
    ' ------------------------------------------------------------------

    Private Sub CommandStats()
        Console.WriteLine("three node network:")
        Console.WriteLine("  total topologies      = {0} (expected 19683)", TopologyEnumerator.CountAll())
        Console.WriteLine("  connected topologies  = {0} (expected 16038)", TopologyEnumerator.CountConnected())

        For n As Integer = 0 To 9
            Console.WriteLine("  connected, at most {0} links: {1}", n, TopologyEnumerator.CountUpTo(n))
        Next

        Dim connected As Integer = 0

        For Each signs In SelfTestFixtures.TwoNodeModel.Enumerate(requireConnected:=True)
            connected += 1
        Next

        Dim total As Integer = SelfTestFixtures.TwoNodeModel.Enumerate().Count()

        Console.WriteLine("two node control network:")
        Console.WriteLine("  total topologies      = {0} (expected 81)", total)
        Console.WriteLine("  connected topologies  = {0}", connected)
    End Sub

    ' ------------------------------------------------------------------
    ' motif
    ' ------------------------------------------------------------------

    Private Sub CommandMotif()
        Dim cases As (name As String, signs As String)() = {
            ("NFBLB minimal (A→C+, B⊣C−, C→B+)", "0+0-0+000"),
            ("IFFLP minimal (A→B+, A→C+, B⊣C−)", "++0-00000"),
            ("coherent FFL (A→B+, A→C+, B→C+)", "++0+00000"),
            ("positive feedback (A→B+, B→C+, C→B+)", "+00+0+000"),
            ("negative feedback without buffer (A→C+, C⊣A−)", "0+00-0000"),
            ("negative self loop (A⊣A)", "-00000000"),
            ("NFBLB plus self loop (B→B+)", "0+0-0+0+0")
        }

        For Each c In cases
            Dim topo = NetworkTopology.FromSymbolString(c.signs)
            Dim flags = MotifAnalyzer.MotifFlags(topo)

            Console.WriteLine("{0,-42} {1}  nfblb={2,-5} ifflp={3,-5} negFB={4,-5} incoherentPaths={5}",
                              c.name, c.signs, flags.nfblb, flags.ifflp,
                              MotifAnalyzer.HasNegativeFeedbackLoop(topo),
                              MotifAnalyzer.IncoherentPathPairs(topo))
        Next
    End Sub

    ' ------------------------------------------------------------------
    ' mechanism
    ' ------------------------------------------------------------------

    Private Sub CommandMechanism()
        Console.WriteLine("NFBLB integral control (A→C+, B⊣C−, C→B+):")
        Console.WriteLine("  C* is predicted to be F_B*k'_FB/k_CB = 0.5*0.1/0.1 = 0.5, independent of the input")

        Dim nfblb As New CircuitModel(NetworkTopology.FromSymbolString("0+0-0+000"), {
            10.0, 0.001,
            10.0, 0.5,
            0.1, 0.001,
            1.0, 8.0,
            0.1, 0.001,
            0.1, 0.001
        })

        Call ReportMechanism(nfblb)

        Console.WriteLine()
        Console.WriteLine("IFFLP proportioner (A→B+, A→C+, B⊣C−):")
        Console.WriteLine("  B* is predicted to be proportional to A*, and C* is predicted to be constant")

        Dim ifflp As New CircuitModel(NetworkTopology.FromSymbolString("++0-00000"), {
            0.01, 0.001,
            1.0, 0.01,
            100.0, 30.0,
            1.0, 8.0,
            0.1, 0.001,
            0.4, 20.0
        })

        Call ReportMechanism(ifflp)
    End Sub

    Private Sub ReportMechanism(model As CircuitModel)
        Dim opts As New CircuitSimulator.SimulationOptions()

        For Each pair As Double() In {New Double() {0.5, 0.6}, New Double() {0.5, 0.7}, New Double() {0.7, 0.84}}
            Dim r = CircuitSimulator.Simulate(model, pair(0), pair(1), opts)

            If r.IsWellBehaved Then
                Dim s As Double = Math.Abs(r.Opeak - r.O1) / r.O1 / 0.2
                Dim e As Double = Math.Abs(r.O2 - r.O1) / r.O1 / 0.2
                Dim p As Double = If(e > 0, 1.0 / e, Double.PositiveInfinity)

                Console.WriteLine("  I {0}→{1}: O1={2:F6} Opeak={3:F6} O2={4:F6} S={5:G4} P={6:G4} {7}",
                                  pair(0), pair(1), r.O1, r.Opeak, r.O2, s, p,
                                  If(s > 1 AndAlso p > 10, "FUNCTIONAL", ""))
                Console.WriteLine("      pre A={0:G6} B={1:G6} C={2:G6} | post A={3:G6} B={4:G6} C={5:G6}",
                                  r.PreStepState(0), r.PreStepState(1), r.PreStepState(2),
                                  r.FinalState(0), r.FinalState(1), r.FinalState(2))
            Else
                Console.WriteLine("  I {0}→{1}: {2} ({3})", pair(0), pair(1), r.Status, r.Message)
            End If
        Next
    End Sub

    ' ------------------------------------------------------------------
    ' search
    ' ------------------------------------------------------------------

    Private Sub CommandSearch(args As String())
        Dim options As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To args.Length - 2
            If args(i).StartsWith("--") Then
                options(args(i).Substring(2)) = args(i + 1)
            End If
        Next

        Dim hasFlag As Func(Of String, Boolean) = Function(name) args.Contains("--" & name)
        Dim search As New TopologySearchOptions()
        Dim outDir As String = "circuits_results"

        If options.ContainsKey("out") Then outDir = options("out")
        If options.ContainsKey("max-links") Then Integer.TryParse(options("max-links"), search.MaxLinks)
        If options.ContainsKey("topologies") Then Integer.TryParse(options("topologies"), search.MaxTopologies)
        If options.ContainsKey("samples") Then Integer.TryParse(options("samples"), search.Samples)
        If options.ContainsKey("seed") Then Integer.TryParse(options("seed"), search.Seed)
        If options.ContainsKey("q") Then Integer.TryParse(options("q"), search.QThreshold)
        If options.ContainsKey("jobs") Then Integer.TryParse(options("jobs"), search.MaxDegreeOfParallelism)
        If options.ContainsKey("cluster-cutoff") Then Double.TryParse(options("cluster-cutoff"), search.ClusterCutoff)
        If options.ContainsKey("checkpoints") Then search.CheckpointDir = options("checkpoints")
        If hasFlag("no-design-table") Then search.RunDesignTable = False

        Console.WriteLine("search scope: at most {0} links, {1} topologies, {2} samples per topology, seed {3}",
                          search.MaxLinks,
                          If(search.MaxTopologies > 0, search.MaxTopologies.ToString(), "all"),
                          search.Samples, search.Seed)

        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim lastReport As DateTime = DateTime.MinValue

        Dim report As SearchReport = TopologySearch.Run(search,
            Sub(progress)
                ' throttle the console output to at most one line per second
                If (DateTime.UtcNow - lastReport).TotalSeconds >= 1.0 Then
                    lastReport = DateTime.UtcNow
                    Console.WriteLine("  [{0}/{1}] topology {2}, functional circuits so far {3}, {4:F1} s",
                                      progress.Completed, progress.Total,
                                      progress.CurrentTopologyId, progress.FunctionalSoFar,
                                      progress.Elapsed.TotalSeconds)
                End If
            End Sub, outDir)

        watch.Stop()

        Console.WriteLine()
        Console.WriteLine("search finished in {0:F1} s (wall clock {1:F1} s)", report.Elapsed.TotalSeconds, watch.Elapsed.TotalSeconds)
        Console.WriteLine("  topologies covered : {0} (evaluated {1}, resumed {2})", report.TopologiesTotal, report.TopologiesEvaluated, report.TopologiesResumed)
        Console.WriteLine("  circuits simulated : {0} (ill behaved {1})", report.CircuitsSimulated, report.IllBehavedCircuits)
        Console.WriteLine("  functional topologies (Q>0)  : {0}", report.FunctionalTopologies)
        Console.WriteLine("  over represented (Q>{0})      : {1}", search.QThreshold, report.OverRepresentedTopologies)

        Dim v = report.VennOverRepresented

        Console.WriteLine("  Venn (over represented): total={0} nfblb_only={1} ifflp_only={2} both={3} neither={4}",
                          v.total, v.nfblbOnly, v.ifflpOnly, v.both, v.neither)

        If report.ClusterCount >= 0 Then
            Console.WriteLine("  Hamming clusters: {0} (silhouette {1:G4}, cutoff {2})", report.ClusterCount, report.Silhouette, search.ClusterCutoff)
        End If

        If report.DesignTable.Count > 0 Then
            Console.WriteLine("  design table (Q at {0} samples):", search.EffectiveDesignTableSamples())

            For Each row In report.DesignTable
                Console.WriteLine("    {0,-24} links={1} Q={2} fraction={3:G4}", row.Name, row.LinkCount, row.Q, row.Fraction)
            Next
        End If

        If report.TopTopologies.Count > 0 Then
            Console.WriteLine("  top topologies by Q:")

            For Each result In report.TopTopologies.Take(10)
                Console.WriteLine("    [{0}] {1} links={2} Q={3} nfblb={4} ifflp={5}",
                                  result.TopologyId, result.Signs, result.LinkCount, result.Functional,
                                  result.HasNFBLB, result.HasIFFLP)
            Next
        End If

        Console.WriteLine()
        Console.WriteLine("products:")

        For Each pair In report.ArtifactPaths
            Console.WriteLine("  {0}: {1}", pair.Key, pair.Value)
        Next
    End Sub

End Module
