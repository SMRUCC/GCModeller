Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.DataMining.Hamming
Imports Microsoft.VisualBasic.Math.Distributions.LHS
Imports SMRUCC.genomics.Analysis.Circuits.Analysis
Imports SMRUCC.genomics.Analysis.Circuits.Assessment
Imports SMRUCC.genomics.Analysis.Circuits.Enumeration
Imports SMRUCC.genomics.Analysis.Circuits.Model
Imports SMRUCC.genomics.Analysis.Circuits.Sampling
Imports SMRUCC.genomics.Analysis.Circuits.Search
Imports SMRUCC.genomics.Analysis.Circuits.Simulation
Imports SelfTestFixtures

''' <summary>
''' The assertion suite of the adaptation topology search. Every check is an
''' anchor that the paper itself provides, so that a regression in the model,
''' the protocol or the criteria is caught instead of silently producing a
''' wrong topology map.
''' (适应拓扑搜索的断言套件：每项检查都是论文自身给出的锚点，用于捕捉模型、协议或判据的回归)
''' </summary>
Friend Module SelfTest

    Private m_passed As Integer = 0
    Private m_failed As Integer = 0
    Private m_failures As New List(Of String)()

    ''' <summary>
    ''' Run the full assertion suite and return the number of failed checks.
    ''' (运行完整断言套件并返回失败的检查数量)
    ''' </summary>
    ''' <param name="samples">the number of sampled parameter sets per topology of the simulation based anchors</param>
    Public Function Run(samples As Integer) As Integer
        m_passed = 0
        m_failed = 0
        m_failures = New List(Of String)()

        Console.WriteLine("=== adaptation topology search self test ===")
        Console.WriteLine("samples per topology of the simulation anchors: {0}", samples)
        Console.WriteLine()

        Call TestEnumerationCounts()
        Call TestTopologyCodec()
        Call TestLinkSlots()
        Call TestParameterLayout()
        Call TestMotifCanonicalCases()
        Call TestFunctionalCriteria()
        Call TestIllBehavedClassification()
        Call TestNFBLBMechanism()
        Call TestIFFPLMechanism()
        Call TestTwoNodeZero(samples)
        Call TestSmallNetworksZero(samples)
        Call TestMinimalNetworkNecessity(samples)
        Call TestDeterminism()
        Call TestHammingDistance()
        Call TestClustering()
        Call TestCheckpointRoundtrip()
        Call TestSearchSmoke()

        Console.WriteLine()
        Console.WriteLine("=== {0} passed, {1} failed ===", m_passed, m_failed)

        If m_failed > 0 Then
            Console.WriteLine()
            Console.WriteLine("failed checks:")

            For Each failure In m_failures
                Console.WriteLine("  - {0}", failure)
            Next
        End If

        Return m_failed
    End Function

    ' ------------------------------------------------------------------
    #Region "assertion helpers"

    Private Sub Check(condition As Boolean, message As String)
        If condition Then
            m_passed += 1
        Else
            m_failed += 1
            m_failures.Add(message)
            Console.WriteLine("  FAIL: {0}", message)
        End If
    End Sub

    Private Sub CheckNear(actual As Double, expected As Double, tolerance As Double, message As String)
        Check(Math.Abs(actual - expected) <= tolerance,
              $"{message} (expected {expected.ToString("G6", CultureInfo.InvariantCulture)} +/- {tolerance.ToString("G2", CultureInfo.InvariantCulture)}, got {actual.ToString("G6", CultureInfo.InvariantCulture)})")
    End Sub

    Private Sub CheckEqual(Of T)(actual As T, expected As T, message As String)
        Check(EqualityComparer(Of T).Default.Equals(actual, expected),
              $"{message} (expected {expected}, got {actual})")
    End Sub

    ''' <summary>
    ''' Evaluate one topology of the search with the standard protocol.
    ''' </summary>
    Private Function EvaluateTopology(topology As NetworkTopology, samples As Integer, seed As Integer) As (result As TopologyResult, records As List(Of CircuitRecord))
        Dim options As New CircuitSimulator.SimulationOptions()

        Return TopologyEvaluator.Evaluate(topology, samples, seed, 0.5, 0.6, options, 1.0, 10.0, Environment.ProcessorCount)
    End Function

    ''' <summary>
    ''' Run an action with the standard error stream silenced, because the ODE
    ''' solver reports its internal Newton failures there.
    ''' </summary>
    Private Sub WithSilencedErrors(action As Action)
        Dim writer As TextWriter = Console.Error

        Call Console.SetError(TextWriter.Null)

        Try
            Call action()
        Finally
            Call Console.SetError(writer)
        End Try
    End Sub

    #End Region

    ' ------------------------------------------------------------------
    #Region "enumeration and codec"

    Private Sub TestEnumerationCounts()
        Console.WriteLine("[1] enumeration counts")

        Call CheckEqual(TopologyEnumerator.CountAll(), 19683, "the three node topology space owns 3^9 = 19683 topologies")
        Call CheckEqual(TopologyEnumerator.CountConnected(), 16038, "the input to output filter keeps 16038 topologies")
        Call CheckEqual(TopologyEnumerator.CountUpTo(2), 38, "the connected topologies with at most 2 links are 38")
        Call CheckEqual(TopologyEnumerator.CountUpTo(3), 310, "the connected topologies with at most 3 links are 310")

        Dim twoNodeTotal As Integer = TwoNodeModel.Enumerate().Count()

        Call CheckEqual(twoNodeTotal, 81, "the two node control network owns 3^4 = 81 topologies")

        Dim twoNodeConnected As Integer = TwoNodeModel.Enumerate(requireConnected:=True).Count()

        Console.WriteLine("      (the connected two node topologies are {0})", twoNodeConnected)
        Check(twoNodeConnected > 0, "the two node control network owns connected topologies")
    End Sub

    Private Sub TestTopologyCodec()
        Console.WriteLine("[2] topology identifier codec")

        For id As Integer = 0 To 19682 Step 97
            Dim topo = NetworkTopology.FromId(id)

            CheckEqual(topo.Id, id, $"the topology id {id} round trips")
        Next

        Dim symbols As String = "0+0-0+000"

        CheckEqual(NetworkTopology.FromSymbolString(symbols).ToSymbolString(), symbols, "the symbol string round trips")
        CheckEqual(NetworkTopology.FromSymbolString(symbols).Id, 10060, "the NFBLB minimal network owns the id 10060")
        CheckEqual(NetworkTopology.FromId(10060).ToSymbolString(), symbols, "the id 10060 decodes into the NFBLB minimal network")
    End Sub

    Private Sub TestLinkSlots()
        Console.WriteLine("[3] link slot mapping")

        Call CheckEqual(NetworkTopology.LinkIndex(0, 1), 0, "the A→B link owns the slot 0")
        Call CheckEqual(NetworkTopology.LinkIndex(0, 2), 1, "the A→C link owns the slot 1")
        Call CheckEqual(NetworkTopology.LinkIndex(1, 0), 2, "the B→A link owns the slot 2")
        Call CheckEqual(NetworkTopology.LinkIndex(1, 2), 3, "the B→C link owns the slot 3")
        Call CheckEqual(NetworkTopology.LinkIndex(2, 0), 4, "the C→A link owns the slot 4")
        Call CheckEqual(NetworkTopology.LinkIndex(2, 1), 5, "the C→B link owns the slot 5")
        Call CheckEqual(NetworkTopology.LinkIndex(0, 0), 6, "the A→A link owns the slot 6")
        Call CheckEqual(NetworkTopology.LinkIndex(1, 1), 7, "the B→B link owns the slot 7")
        Call CheckEqual(NetworkTopology.LinkIndex(2, 2), 8, "the C→C link owns the slot 8")

        For slot As Integer = 0 To 8
            Dim link = NetworkTopology.LinkAt(slot)

            CheckEqual(NetworkTopology.LinkIndex(link.source, link.target), slot, $"the slot {slot} round trips through its link pair")
        Next
    End Sub

    Private Sub TestParameterLayout()
        Console.WriteLine("[4] parameter layout")

        For Each topology In TopologyEnumerator.EnumerateConnected().Take(200)
            Dim expected As Integer = 2 * (topology.LinkCount + 1 + CircuitModel.BackgroundLinkCount(topology))

            CheckEqual(CircuitModel.ParameterCount(topology), expected,
                       $"the parameter count of the topology {topology.Id} matches the layout formula")

            ' a parameter vector of the expected length must build a model
            Dim params As Double() = New Double(expected - 1) {}

            For i As Integer = 0 To expected - 1
                params(i) = 1.0
            Next

            Try
                Dim model As New CircuitModel(topology, params)

                Check(model IsNot Nothing, $"the model of the topology {topology.Id} builds")
            Catch ex As Exception
                Check(False, $"the model of the topology {topology.Id} builds ({ex.Message})")
            End Try
        Next
    End Sub

    #End Region

    ' ------------------------------------------------------------------
    #Region "motif detection"

    Private Sub TestMotifCanonicalCases()
        Console.WriteLine("[5] motif detection on the canonical networks")

        ' the NFBLB minimal network: A→C+, B⊣C−, C→B+
        Dim nfblb = NetworkTopology.FromSymbolString("0+0-0+000")

        Check(MotifAnalyzer.HasNFBLB(nfblb), "the NFBLB minimal network owns the NFBLB motif")
        Check(Not MotifAnalyzer.HasIFFLP(nfblb), "the NFBLB minimal network does not own the IFFLP motif")
        Check(MotifAnalyzer.HasNegativeFeedbackLoop(nfblb), "the NFBLB minimal network owns a negative feedback loop")

        ' the IFFLP minimal network: A→B+, A→C+, B⊣C−
        Dim ifflp = NetworkTopology.FromSymbolString("++0-00000")

        Check(MotifAnalyzer.HasIFFLP(ifflp), "the IFFLP minimal network owns the IFFLP motif")
        Check(Not MotifAnalyzer.HasNFBLB(ifflp), "the IFFLP minimal network does not own the NFBLB motif")

        ' the coherent feedforward loop: both paths from A to C are positive
        Dim coherent = NetworkTopology.FromSymbolString("++0+00000")

        Check(Not MotifAnalyzer.HasIFFLP(coherent), "the coherent feedforward loop does not own the IFFLP motif")
        Check(Not MotifAnalyzer.HasNegativeFeedbackLoop(coherent), "the coherent feedforward loop owns no negative feedback loop")

        ' the positive feedback loop: the B→C→B cycle is positive
        Dim positiveFeedback = NetworkTopology.FromSymbolString("+00+0+000")

        Check(Not MotifAnalyzer.HasNegativeFeedbackLoop(positiveFeedback), "the positive feedback loop owns no negative feedback loop")
        Check(Not MotifAnalyzer.HasNFBLB(positiveFeedback), "the positive feedback loop does not own the NFBLB motif")

        ' the negative feedback loop without a buffer: the A→C→A cycle is
        ' negative but it does not pass through B, and C regulates A directly
        Dim noBuffer = NetworkTopology.FromSymbolString("0+00-0000")

        Check(MotifAnalyzer.HasNegativeFeedbackLoop(noBuffer), "the feedback onto the input node is a negative feedback loop")
        Check(Not MotifAnalyzer.HasNFBLB(noBuffer), "the negative feedback loop without a buffering node does not own the NFBLB motif")

        ' a negative self loop on A is a negative feedback loop of length one
        Dim selfLoop = NetworkTopology.FromSymbolString("-00000000")

        Check(MotifAnalyzer.HasNegativeFeedbackLoop(selfLoop), "a negative self loop is a negative feedback loop")

        ' the NFBLB with the positive self loop on B
        Dim nfblbSelf = NetworkTopology.FromSymbolString("0+0-0+0+0")

        Check(MotifAnalyzer.HasNFBLB(nfblbSelf), "the NFBLB with the positive self loop owns the NFBLB motif")
        CheckEqual(nfblbSelf.LinkCount, 4, "the NFBLB with the positive self loop owns 4 links")
    End Sub

    #End Region

    #Region "functional criteria"

    Private Sub TestFunctionalCriteria()
        Console.WriteLine("[6] sensitivity and precision criteria")

        ' the relative input change of the protocol of the paper is 20%
        Call CheckNear(FunctionalAssessor.InputChange(0.5, 0.6), 0.2, 1e-12, "the input change of the 0.5 to 0.6 step is 0.2")

        ' a circuit that responds by 30% and returns to within 0.1%
        Dim s As Double = FunctionalAssessor.Sensitivity(1.0, 1.3, 0.2)
        Dim p As Double = FunctionalAssessor.Precision(1.0, 1.001, 0.2)

        Call CheckNear(s, 1.5, 1e-12, "the sensitivity of a 30% peak response is 1.5")
        Call CheckNear(p, 200.0, 1e-9, "the precision of a 0.1% steady state deviation is 200")
        Check(FunctionalAssessor.IsFunctional(s, p), "the 30% peak and 0.1% deviation circuit is functional")

        ' a weak response or a large deviation is not functional
        Check(Not FunctionalAssessor.IsFunctional(0.5, 200.0), "a sensitivity below 1 is not functional")
        Check(Not FunctionalAssessor.IsFunctional(1.5, 5.0), "a precision below 10 is not functional")
        Check(FunctionalAssessor.IsFunctional(1.0 + 1e-9, 10.0 + 1e-6), "the thresholds are exclusive")
        Check(Not FunctionalAssessor.IsFunctional(1.0, 10.0), "a circuit exactly on the thresholds is not functional")

        ' a perfect adaptation owns an infinite precision
        Call Check(Double.IsPositiveInfinity(FunctionalAssessor.Precision(0.5, 0.5, 0.2)), "a zero steady state deviation owns an infinite precision")

        ' the sensitivity of a downward transient is positive
        Call CheckNear(FunctionalAssessor.Sensitivity(1.0, 0.7, 0.2), 1.5, 1e-12, "the sensitivity of a downward peak is positive")
    End Sub

    Private Sub TestIllBehavedClassification()
        Console.WriteLine("[7] ill behaved classification")

        Dim failed As New SimulationOutcome With {.Status = SimulationOutcome.SimulationStatus.NotSettled}
        Dim assessed = FunctionalAssessor.Assess(failed, 0.5, 0.6, 1.0, 10.0)

        Check(Not assessed.IsFunctional, "a not settled circuit is not functional")
        Check(assessed.IsIllBehaved, "a not settled circuit is ill behaved")
        Check(Double.IsNaN(assessed.Sensitivity), "an ill behaved circuit owns no sensitivity")

        Dim ok As New SimulationOutcome With {
            .Status = SimulationOutcome.SimulationStatus.OK,
            .O1 = 1.0,
            .Opeak = 1.5,
            .O2 = 1.001
        }
        Dim good = FunctionalAssessor.Assess(ok, 0.5, 0.6, 1.0, 10.0)

        Check(good.IsFunctional, "a well behaved circuit with a large peak is functional")
        Check(Not good.IsIllBehaved, "a well behaved circuit is not ill behaved")
    End Sub

    #End Region

    #Region "mechanism anchors"

    Private Sub TestNFBLBMechanism()
        Console.WriteLine("[8] NFBLB integral control mechanism")

        ' A→C+, B⊣C−, C→B+ with the enzymes of B in the saturated regime:
        ' the steady state of C is predicted to be F_B*k'_FB/k_CB = 0.5*0.1/0.1
        ' = 0.5, independent of the input, while B integrates the error
        Dim model As New CircuitModel(NetworkTopology.FromSymbolString("0+0-0+000"), {
            10.0, 0.001,
            10.0, 0.5,
            0.1, 0.001,
            1.0, 8.0,
            0.1, 0.001,
            0.1, 0.001
        })
        Dim opts As New CircuitSimulator.SimulationOptions()
        Dim r1 = CircuitSimulator.Simulate(model, 0.5, 0.6, opts)
        Dim r2 = CircuitSimulator.Simulate(model, 0.5, 0.7, opts)

        Check(r1.IsWellBehaved, $"the NFBLB circuit simulates well at the 0.5 to 0.6 step ({r1.Message})")
        Check(r2.IsWellBehaved, $"the NFBLB circuit simulates well at the 0.5 to 0.7 step ({r2.Message})")

        If Not (r1.IsWellBehaved AndAlso r2.IsWellBehaved) Then
            Return
        End If

        ' the steady state of the output does not depend on the input
        Call CheckNear(r1.O2, r1.O1, 0.01, "the NFBLB output returns to its pre stimulus level after the step")
        Call CheckNear(r2.O2, r2.O1, 0.02, "the NFBLB output returns to its pre stimulus level after the larger step")
        Call CheckNear(r1.O1, 0.5, 0.01, "the NFBLB steady state matches the analytic prediction 0.5*k'_FB/k_CB")

        ' the buffering node integrates the error: its state differs between
        ' the two stepped runs although the output steady state does not
        Dim b1 As Double = r1.FinalState(1)
        Dim b2 As Double = r2.FinalState(1)

        Check(Math.Abs(b1 - b2) > 0.01, $"the buffering node state differs between the two steps ({b1.ToString("G6")} vs {b2.ToString("G6")})")

        ' the circuit is both sensitive and precise, i.e. functional
        Dim s As Double = FunctionalAssessor.Sensitivity(r1.O1, r1.Opeak, 0.2)
        Dim p As Double = FunctionalAssessor.Precision(r1.O1, r1.O2, 0.2)

        Check(FunctionalAssessor.IsFunctional(s, p),
              $"the NFBLB circuit is functional at the protocol of the paper (S={s.ToString("G4")}, P={p.ToString("G4")})")
    End Sub

    Private Sub TestIFFPLMechanism()
        Console.WriteLine("[9] IFFLP proportioner mechanism")

        ' A→B+, A→C+, B⊣C− with the activation of B saturated and its
        ' inactivation linear: B* is proportional to A*, and C* is constant
        Dim model As New CircuitModel(NetworkTopology.FromSymbolString("++0-00000"), {
            0.01, 0.001,
            1.0, 0.01,
            100.0, 30.0,
            1.0, 8.0,
            0.1, 0.001,
            0.4, 20.0
        })
        Dim opts As New CircuitSimulator.SimulationOptions()
        Dim r1 = CircuitSimulator.Simulate(model, 0.5, 0.6, opts)
        Dim r2 = CircuitSimulator.Simulate(model, 0.7, 0.84, opts)

        Check(r1.IsWellBehaved, $"the IFFLP circuit simulates well at the 0.5 to 0.6 step ({r1.Message})")
        Check(r2.IsWellBehaved, $"the IFFLP circuit simulates well at the 0.7 to 0.84 step ({r2.Message})")

        If Not (r1.IsWellBehaved AndAlso r2.IsWellBehaved) Then
            Return
        End If

        ' the proportioner: B* / A* is the same at both input levels
        Dim gain1 As Double = r1.PreStepState(1) / r1.PreStepState(0)
        Dim gain2 As Double = r2.PreStepState(1) / r2.PreStepState(0)

        Call CheckNear(gain2, gain1, 0.15 * gain1,
                       "the buffering node of the IFFLP is a proportioner of the input node")

        ' the output returns to its pre stimulus level
        Call CheckNear(r1.O2, r1.O1, 0.01, "the IFFLP output returns to its pre stimulus level after the step")

        Dim s As Double = FunctionalAssessor.Sensitivity(r1.O1, r1.Opeak, 0.2)
        Dim p As Double = FunctionalAssessor.Precision(r1.O1, r1.O2, 0.2)

        Check(FunctionalAssessor.IsFunctional(s, p),
              $"the IFFLP circuit is functional at the protocol of the paper (S={s.ToString("G4")}, P={p.ToString("G4")})")
    End Sub

    #End Region

    #Region "simulation based anchors"

    Private Sub TestTwoNodeZero(samples As Integer)
        Console.WriteLine("[10] the two node control networks never adapt")

        Dim adaptive As Integer = 0

        Call WithSilencedErrors(
            Sub()
                For Each signs In TwoNodeModel.Enumerate()
                    Dim sampled = SampleTwoNode(signs, samples, 42)

                    For i As Integer = 0 To sampled.Length - 1
                        Dim model As New TwoNodeModel(signs, sampled(i))
                        Dim outcome = CircuitSimulator.Simulate(model, 0.5, 0.6, New CircuitSimulator.SimulationOptions())

                        If outcome.IsWellBehaved Then
                            Dim s As Double = FunctionalAssessor.Sensitivity(outcome.O1, outcome.Opeak, 0.2)
                            Dim p As Double = FunctionalAssessor.Precision(outcome.O1, outcome.O2, 0.2)

                            If FunctionalAssessor.IsFunctional(s, p) Then
                                adaptive += 1
                            End If
                        End If
                    Next
                Next
            End Sub)

        Call CheckEqual(adaptive, 0, "no two node topology owns a functional circuit")
    End Sub

    ''' <summary>
    ''' Sample the parameter sets of a two node network with the same Latin
    ''' hypercube strategy that the three node sampler uses.
    ''' (用与三节点采样器相同的拉丁超立方策略采样双节点网络的参数集)
    ''' </summary>
    Private Function SampleTwoNode(signs As SByte(), samples As Integer, seed As Integer) As Double()()
        Dim nParams As Integer = TwoNodeModel.ParameterCount(signs)
        Dim sampler As New Distributions.LHS.LatinHypercubeSampler(nDims:=nParams, seed:=seed)
        Dim unit As Double()() = sampler.Random(samples)
        Dim lower As Double() = Distributions.LHS.LhsScale.TileBounds(New Double() {-1.0, -3.0}, nParams)
        Dim upper As Double() = Distributions.LHS.LhsScale.TileBounds(New Double() {1.0, 2.0}, nParams)

        Return Distributions.LHS.LhsScale.ScaleLog10(unit, lower, upper)
    End Function

    Private Sub TestSmallNetworksZero(samples As Integer)
        Console.WriteLine("[11] the three node networks with at most two links never adapt")

        Dim adaptive As New List(Of Integer)()

        Call WithSilencedErrors(
            Sub()
                For Each topology In TopologyEnumerator.EnumerateUpTo(2)
                    Dim evaluated = EvaluateTopology(topology, samples, ParameterSampler.SeedOf(topology.Id, 42))

                    If evaluated.result.Functional > 0 Then
                        adaptive.Add(topology.Id)
                    End If
                Next
            End Sub)

        Call CheckEqual(adaptive.Count, 0,
                        $"no topology with at most two links owns a functional circuit (the adaptive ones are {String.Join(", ", adaptive)})")
    End Sub

    Private Sub TestMinimalNetworkNecessity(samples As Integer)
        Console.WriteLine("[12] the minimal three link networks and the motif necessity")

        Dim functional As New List(Of NetworkTopology)()
        Dim hasNFBLB As Boolean = False
        Dim hasIFFLP As Boolean = False

        Call WithSilencedErrors(
            Sub()
                For Each topology In TopologyEnumerator.EnumerateWithLinkCount(3)
                    Dim evaluated = EvaluateTopology(topology, samples, ParameterSampler.SeedOf(topology.Id, 42))

                    If evaluated.result.Functional > 0 Then
                        functional.Add(topology)

                        Dim flags = MotifAnalyzer.MotifFlags(topology)

                        If flags.nfblb Then
                            hasNFBLB = True
                        End If
                        If flags.ifflp Then
                            hasIFFLP = True
                        End If
                    End If
                Next
            End Sub)

        Console.WriteLine("      ({0} of the 272 connected three link topologies own a functional circuit at {1} samples)",
                          functional.Count, samples)

        ' every adaptive minimal network owns one of the two motifs
        Dim withoutMotif As New List(Of String)()

        For Each topology In functional
            Dim flags = MotifAnalyzer.MotifFlags(topology)

            If Not (flags.nfblb OrElse flags.ifflp) Then
                withoutMotif.Add(topology.ToString())
            End If
        Next

        Call CheckEqual(withoutMotif.Count, 0,
                        $"every adaptive three link network owns the NFBLB or the IFFLP motif (the offenders are {String.Join(", ", withoutMotif)})")

        ' the minimal adaptation networks are rare, the paper reports Q = 5 and
        ' Q = 16 out of 10,000 samples, so the sample count of a validation run
        ' may find none of them; the mechanism anchors above prove that three
        ' link networks can adapt, which is the minimal link count of the paper
        If functional.Count = 0 Then
            Console.WriteLine("      (no minimal network was hit at this sample count, the rarity is expected)")
        Else
            Check(hasNFBLB OrElse hasIFFLP, "at least one adaptive minimal network was found and it owns a motif")
        End If
    End Sub

    Private Sub TestDeterminism()
        Console.WriteLine("[13] determinism of the sampling")

        Dim topology = NetworkTopology.FromSymbolString("++0-00000")
        Dim first = ParameterSampler.Sample(topology, 50, 12345)
        Dim second = ParameterSampler.Sample(topology, 50, 12345)
        Dim third = ParameterSampler.Sample(topology, 50, 12346)

        Dim same As Boolean = True

        For i As Integer = 0 To first.Length - 1
            For j As Integer = 0 To first(i).Length - 1
                If first(i)(j) <> second(i)(j) Then
                    same = False
                    Exit For
                End If
            Next

            If Not same Then
                Exit For
            End If
        Next

        Check(same, "the same sampler seed reproduces the very same design")
        Check(Not ReferenceEquals(first, second), "the sampler returns a fresh matrix")

        Dim different As Boolean = False

        For i As Integer = 0 To first.Length - 1
            If first(i)(0) <> third(i)(0) Then
                different = True
                Exit For
            End If
        Next

        Check(different, "a different sampler seed produces a different design")

        ' the per topology seed derivation is deterministic and injective on a sample
        Dim seeds As New HashSet(Of Integer)()

        For id As Integer = 0 To 5000
            Call seeds.Add(ParameterSampler.SeedOf(id, 42))
        Next

        Call CheckEqual(seeds.Count, 5001, "the per topology seed derivation is injective on 0 .. 5000")

        ' the parameter values stay inside of the bounds of the paper
        Dim bounds = ParameterSampler.RealBounds(first(0).Length)
        Dim inside As Boolean = True

        For Each row In first
            For j As Integer = 0 To row.Length - 1
                If row(j) < bounds.lower(j) OrElse row(j) > bounds.upper(j) Then
                    inside = False
                    Exit For
                End If
            Next

            If Not inside Then
                Exit For
            End If
        Next

        Check(inside, "every sampled parameter stays inside of its range")
    End Sub

    #End Region

    #Region "distance, clustering and checkpoint"

    Private Sub TestHammingDistance()
        Console.WriteLine("[14] Hamming distance of the topologies")

        Dim a As Integer() = {1, 0, -1, 0, 0, 0, 0, 0, 0}
        Dim b As Integer() = {1, 0, 0, 0, 0, 0, 0, 0, 0}
        Dim c As Integer() = {-1, 0, 1, 0, 0, 0, 0, 0, 0}

        Call CheckEqual(HammingDistance.Between(a, a), 0, "the Hamming distance of a vector to itself is zero")
        Call CheckEqual(HammingDistance.Between(a, b), 1, "the Hamming distance of a single sign difference is one")
        Call CheckEqual(HammingDistance.Between(a, c), 2, "the Hamming distance of two sign differences is two")

        Dim topo1 = NetworkTopology.FromSymbolString("0+0-0+000")
        Dim topo2 = NetworkTopology.FromSymbolString("0+0-0+0+0")

        Call CheckEqual(HammingDistance.Between(topo1.ToIntegerVector(), topo2.ToIntegerVector()), 1,
                        "the NFBLB minimal network and its self loop variant differ in one slot")
    End Sub

    Private Sub TestClustering()
        Console.WriteLine("[15] Hamming clustering of the topologies")

        ' two close pairs that are far apart from each other
        Dim topologies As NetworkTopology() = {
            NetworkTopology.FromSymbolString("0+0-0+000"),
            NetworkTopology.FromSymbolString("0+0-0+0+0"),
            NetworkTopology.FromSymbolString("+00+0+000"),
            NetworkTopology.FromSymbolString("+00+0++00")
        }
        Dim clustered = TopologyClustering.Cluster(topologies, 2.0)

        Call CheckEqual(clustered.clusterCount, 2, "the two close pairs form two clusters")
        Call CheckEqual(clustered.labels(0), clustered.labels(1), "the first pair shares a cluster")
        Call CheckEqual(clustered.labels(2), clustered.labels(3), "the second pair shares a cluster")
        Check(clustered.labels(0) <> clustered.labels(2), "the two pairs own different clusters")

        Dim single = TopologyClustering.Cluster(New NetworkTopology() {topologies(0)}, 2.0)

        Call CheckEqual(single.clusterCount, 1, "a single topology forms a single cluster")

        Dim silhouette As Double = TopologyClustering.Silhouette(topologies, clustered.labels)

        Check(silhouette > 0.0, $"the silhouette of the two pair clustering is positive ({silhouette.ToString("G4")})")

        ' the distance matrix is symmetric with a zero diagonal
        Dim dist = TopologyClustering.DistanceMatrix(topologies)
        Dim symmetric As Boolean = True

        For i As Integer = 0 To topologies.Length - 1
            If dist(i, i) <> 0 Then
                symmetric = False
            End If

            For j As Integer = 0 To topologies.Length - 1
                If dist(i, j) <> dist(j, i) Then
                    symmetric = False
                End If
            Next
        Next

        Check(symmetric, "the distance matrix is symmetric with a zero diagonal")
    End Sub

    Private Sub TestCheckpointRoundtrip()
        Console.WriteLine("[16] checkpoint round trip")

        Dim dir As String = Path.Combine(Path.GetTempPath(), "circuits_selftest_checkpoint")

        If Directory.Exists(dir) Then
            Call Directory.Delete(dir, True)
        End If

        Dim topology = NetworkTopology.FromSymbolString("++0-00000")
        Dim evaluated = EvaluateTopology(topology, 30, 999)

        Call CheckpointStore.Save(dir, topology, evaluated.records)
        Check(CheckpointStore.Exists(dir, topology.Id), "the checkpoint file of the topology exists")

        Dim loaded = CheckpointStore.Load(dir, topology.Id, 1.0, 10.0)

        Check(loaded.result IsNot Nothing, "the checkpoint reloads")
        CheckEqual(loaded.result.Functional, evaluated.result.Functional, "the reloaded Q value matches")
        CheckEqual(loaded.records.Count, evaluated.records.Where(Function(r) Not r.IsIllBehaved).Count(),
                   "the reloaded circuit count matches the well behaved circuits")

        Dim missing = CheckpointStore.Load(dir, 12345, 1.0, 10.0)

        Check(missing.result Is Nothing, "a missing checkpoint reloads as nothing")

        Call Directory.Delete(dir, True)
    End Sub

    #End Region

    #Region "search smoke"

    Private Sub TestSearchSmoke()
        Console.WriteLine("[17] search pipeline smoke")

        Dim dir As String = Path.Combine(Path.GetTempPath(), "circuits_selftest_search")

        If Directory.Exists(dir) Then
            Call Directory.Delete(dir, True)
        End If

        Dim options As New TopologySearchOptions With {
            .MaxLinks = 3,
            .MaxTopologies = 12,
            .Samples = 20,
            .Seed = 42,
            .QThreshold = 10,
            .RunDesignTable = True,
            .DesignTableSamples = 20,
            .CheckpointDir = Path.Combine(dir, "checkpoints")
        }
        Dim report As SearchReport = Nothing

        Call WithSilencedErrors(
            Sub()
                report = TopologySearch.Run(options, Nothing, Path.Combine(dir, "products"))
            End Sub)

        Call CheckEqual(report.TopologiesTotal, 12, "the smoke search covered the twelve limited topologies")
        Call CheckEqual(report.TopologiesEvaluated, 12, "the smoke search evaluated all twelve topologies")
        Call CheckEqual(report.CircuitsSimulated, 240, "the smoke search simulated twelve times twenty circuits")
        Check(File.Exists(Path.Combine(dir, "products", "topologies.csv")), "the topologies product exists")
        Check(File.Exists(Path.Combine(dir, "products", "functional_circuits.csv")), "the functional circuits product exists")
        Check(File.Exists(Path.Combine(dir, "products", "clusters.csv")), "the clusters product exists")
        Check(File.Exists(Path.Combine(dir, "products", "venn_summary.csv")), "the venn summary product exists")
        Check(File.Exists(Path.Combine(dir, "products", "design_table.csv")), "the design table product exists")
        Check(File.Exists(Path.Combine(dir, "products", "report.txt")), "the report product exists")
        Check(File.Exists(Path.Combine(dir, "checkpoints", "topo_00000.csv")), "the checkpoint of the first topology exists")

        ' a resumed run reloads every topology from the checkpoints
        Dim resumed As SearchReport = Nothing

        Call WithSilencedErrors(
            Sub()
                resumed = TopologySearch.Run(options, Nothing, Path.Combine(dir, "products2"))
            End Sub)

        Call CheckEqual(resumed.TopologiesTotal, 12, "the resumed search covered the same twelve topologies")
        Call CheckEqual(resumed.TopologiesEvaluated, 0, "the resumed search evaluated no topology again")
        Call CheckEqual(resumed.CircuitsSimulated, report.CircuitsSimulated, "the resumed search reports the same circuit count")
        Call CheckEqual(resumed.FunctionalTopologies, report.FunctionalTopologies, "the resumed search reports the same functional topology count")

        Call Directory.Delete(dir, True)
    End Sub

    #End Region

End Module
