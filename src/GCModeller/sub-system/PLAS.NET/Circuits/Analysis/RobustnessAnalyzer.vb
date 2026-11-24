Imports System.Collections.Generic
Imports System.Linq

Namespace Analysis

    ''' <summary>
    ''' The robustness analysis of the topology search: the over representation
    ''' filter of the paper and the design table experiment that shows how the
    ''' motif augmentations boost the robustness Q value.
    ''' (拓扑搜索的鲁棒性分析：论文的过度代表筛选与展示基序增强提升 Q 值的设计表实验)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' A topology is over represented inside of the functional region when its
    ''' Q value, the number of functional parameter sets, exceeds the threshold
    ''' (10 in the paper, which selects 395 of the 16,038 topologies). The
    ''' over represented set is the population on which the necessity claim of
    ''' the paper, that every robust adaptation network owns an NFBLB or an
    ''' IFFLP motif, is tested.
    ''' </para>
    ''' <para>
    ''' The design table experiment evaluates the minimal networks of the two
    ''' motifs together with their motif augmentations at the same sample count,
    ''' which reproduces the qualitative progression of the design table of the
    ''' paper (adding a positive self loop on the buffering node or an extra
    ''' negative feedback loop through it boosts the robustness).
    ''' </para>
    ''' </remarks>
    Public Module RobustnessAnalyzer

        ''' <summary>
        ''' One row of the design table: a named network, its topology and the
        ''' robustness that the experiment measured for it.
        ''' (设计表的一行：命名网络、其拓扑与实验测得的鲁棒性)
        ''' </summary>
        Public Class DesignTableRow

            ''' <summary>The name of the network, for example NFBLB-min.</summary>
            Public Property Name As String

            ''' <summary>The sign vector of the network as a symbol string.</summary>
            Public Property Signs As String

            ''' <summary>The number of active links of the network.</summary>
            Public Property LinkCount As Integer

            ''' <summary>The robustness Q value of the network at the sample count of the experiment.</summary>
            Public Property Q As Integer

            ''' <summary>The number of simulated circuits of the network.</summary>
            Public Property Simulated As Integer

            ''' <summary>The fraction of the sampled circuits that are functional.</summary>
            Public Property Fraction As Double

        End Class

        ''' <summary>
        ''' The canonical networks of the design table experiment: the minimal
        ''' networks of the two adaptation motifs and their augmentations.
        ''' (设计表实验的规范网络：两大适应基序的最小网络及其增强)
        ''' </summary>
        ''' <remarks>
        ''' The slot order of the symbol strings is
        ''' [A→B, A→C, B→A, B→C, C→A, C→B, A→A, B→B, C→C].
        ''' </remarks>
        Public Function DesignTableNetworks() As (name As String, signs As String)()
            Return {
                ("NFBLB-min", "0+0-0+000"),
                ("NFBLB+selfloop", "0+0-0+0+0"),
                ("NFBLB+selfloop+negFB", "0+--0+0+0"),
                ("IFFLP-min", "++0-00000"),
                ("IFFLP+negFB", "++0-0+000"),
                ("IFFLP+negFB+2", "++0--+0+0")
            }
        End Function

        ''' <summary>
        ''' Run the design table experiment: evaluate every canonical network of
        ''' <see cref="DesignTableNetworks"/> with the same sample count, the same
        ''' seed derivation and the same protocol, so that their Q values are
        ''' directly comparable.
        ''' (运行设计表实验：以相同采样数、相同种子派生与相同协议评估全部规范网络，使 Q 值可直接比较)
        ''' </summary>
        ''' <param name="samples">the number of sampled parameter sets per network</param>
        ''' <param name="seed">the base seed of the experiment</param>
        ''' <param name="i1">the initial input level of the step protocol</param>
        ''' <param name="i2">the stepped input level of the step protocol</param>
        ''' <param name="simOptions">the simulation options</param>
        ''' <param name="sThreshold">the sensitivity threshold of the functional region</param>
        ''' <param name="pThreshold">the precision threshold of the functional region</param>
        ''' <param name="maxDegreeOfParallelism">the thread count of the inner simulation loop, 0 for sequential</param>
        Public Iterator Function DesignTableExperiment(samples As Integer,
                                                        seed As Integer,
                                                        i1 As Double,
                                                        i2 As Double,
                                                        simOptions As Simulation.CircuitSimulator.SimulationOptions,
                                                        sThreshold As Double,
                                                        pThreshold As Double,
                                                        Optional maxDegreeOfParallelism As Integer = 0) As IEnumerable(Of DesignTableRow)
            For Each network In DesignTableNetworks()
                Dim topology = Model.NetworkTopology.FromSymbolString(network.signs)
                Dim samplerSeed = SMRUCC.genomics.Analysis.Circuits.Sampling.ParameterSampler.SeedOf(topology.Id, seed)
                Dim evaluated = Assessment.TopologyEvaluator.Evaluate(topology, samples, samplerSeed, i1, i2, simOptions, sThreshold, pThreshold, maxDegreeOfParallelism)
                Dim result = evaluated.result

                Yield New DesignTableRow With {
                    .Name = network.name,
                    .Signs = network.signs,
                    .LinkCount = topology.LinkCount,
                    .Q = result.Functional,
                    .Simulated = result.Simulated,
                    .Fraction = If(result.Simulated > 0, result.Functional / CDbl(result.Simulated), 0.0)
                }
            Next
        End Function

        ''' <summary>
        ''' The over represented topologies of a search: the topologies whose Q
        ''' value exceeds the threshold, ordered by the descending Q value.
        ''' (搜索的过度代表拓扑：Q 值超过门槛的拓扑，按 Q 值降序排列)
        ''' </summary>
        ''' <param name="results">the topology results of the search</param>
        ''' <param name="qThreshold">the Q threshold of the over representation</param>
        Public Function OverRepresented(results As IEnumerable(Of Assessment.TopologyResult),
                                        qThreshold As Integer) As List(Of Assessment.TopologyResult)
            If results Is Nothing Then
                Throw New ArgumentNullException(NameOf(results))
            End If

            Return results _
                .Where(Function(r) r.Functional > qThreshold) _
                .OrderByDescending(Function(r) r.Functional) _
                .ThenBy(Function(r) r.TopologyId) _
                .ToList()
        End Function

        ''' <summary>
        ''' The topologies with at least one functional circuit, ordered by the
        ''' descending Q value. This is the population of the broad motif
        ''' necessity check, which does not apply the over representation
        ''' threshold.
        ''' (至少有一个功能电路的拓扑，按 Q 值降序，即未施加过度代表门槛的广义基序必要性检查总体)
        ''' </summary>
        ''' <param name="results">the topology results of the search</param>
        Public Function FunctionalTopologies(results As IEnumerable(Of Assessment.TopologyResult)) As List(Of Assessment.TopologyResult)
            If results Is Nothing Then
                Throw New ArgumentNullException(NameOf(results))
            End If

            Return results _
                .Where(Function(r) r.Functional > 0) _
                .OrderByDescending(Function(r) r.Functional) _
                .ThenBy(Function(r) r.TopologyId) _
                .ToList()
        End Function

    End Module

End Namespace
