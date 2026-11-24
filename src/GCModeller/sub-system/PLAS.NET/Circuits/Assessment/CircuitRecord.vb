Namespace Assessment

    ''' <summary>
    ''' The assessment record of one single circuit, i.e. one (topology,
    ''' parameter set) combination that was simulated and judged.
    ''' (单个电路的评估记录，即一个经过仿真与判定的（拓扑， 参数集）组合)
    ''' </summary>
    Public Class CircuitRecord

        ''' <summary>The identifier of the topology of the circuit.</summary>
        Public Property TopologyId As Integer

        ''' <summary>The index of the parameter set inside of the sampled batch.</summary>
        Public Property SampleIndex As Integer

        ''' <summary>The steady state output at the initial input O1.</summary>
        Public Property O1 As Double

        ''' <summary>The peak output during the transient after the step.</summary>
        Public Property Opeak As Double

        ''' <summary>The steady state output at the stepped input O2.</summary>
        Public Property O2 As Double

        ''' <summary>
        ''' The drift of the output node per unit time at the end of the phase 1
        ''' steady state search, a diagnostic of the convergence quality.
        ''' </summary>
        Public Property OutputDrift As Double

        ''' <summary>The sensitivity S of the circuit, NaN for an ill behaved circuit.</summary>
        Public Property Sensitivity As Double

        ''' <summary>The precision P of the circuit, NaN for an ill behaved circuit.</summary>
        Public Property Precision As Double

        ''' <summary>Whether the circuit falls into the functional region.</summary>
        Public Property IsFunctional As Boolean

        ''' <summary>Whether the circuit is ill behaved and excluded from the statistics.</summary>
        Public Property IsIllBehaved As Boolean

        ''' <summary>
        ''' The simulation status of the circuit as an integer, the
        ''' <see cref="Simulation.SimulationOutcome.SimulationStatus"/> value.
        ''' </summary>
        Public Property Status As Integer

    End Class

End Namespace
