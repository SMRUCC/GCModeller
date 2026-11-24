Namespace Simulation

    ''' <summary>
    ''' The outcome of the two phase step simulation of one circuit, i.e. the
    ''' three scalar readouts that the functional criteria of the paper are
    ''' computed from, together with the well behavedness diagnostics.
    ''' (单个电路两阶段阶跃仿真的结果：功能判据所需的三个标量读出值与病态诊断)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The readouts follow the input protocol of the paper: the system is
    ''' integrated at the initial input I1 until the steady state (readout
    ''' <see cref="O1"/>), the input is stepped to I2 and the transient of the
    ''' output node is recorded (the peak <see cref="Opeak"/> and the final
    ''' steady state <see cref="O2"/>).
    ''' </para>
    ''' <para>
    ''' A circuit whose simulation does not end with the
    ''' <see cref="SimulationStatus.OK"/> status is an ill behaved circuit of
    ''' the paper: it is excluded from the functional statistics instead of
    ''' being counted as a non adaptive circuit.
    ''' </para>
    ''' </remarks>
    Public Class SimulationOutcome

        ''' <summary>
        ''' The status of a simulation, i.e. the reason why a circuit is well
        ''' behaved or ill behaved.
        ''' (仿真状态，即电路表现良好或病态的原因)
        ''' </summary>
        Public Enum SimulationStatus
            ''' <summary>the simulation succeeded and the readouts are usable</summary>
            OK = 0
            ''' <summary>the ODE solver failed to integrate the circuit</summary>
            SolverFailure = 1
            ''' <summary>the circuit did not reach its steady state within the time horizon</summary>
            NotSettled = 2
            ''' <summary>the steady state of the output node is too small to be meaningful</summary>
            SteadyStateTooSmall = 3
            ''' <summary>a node concentration left its physical range [0, 1]</summary>
            NonPhysical = 4
            ''' <summary>a node concentration became NaN or infinity</summary>
            NonFinite = 5
            ''' <summary>an unexpected exception was raised and recorded</summary>
            Exception = 6
        End Enum

        ''' <summary>The status of the simulation.</summary>
        Public Property Status As SimulationStatus = SimulationStatus.OK

        ''' <summary>
        ''' The steady state value of the output node at the initial input I1,
        ''' i.e. the output level before the stimulus.
        ''' (初始输入 I1 下输出节点的稳态值，即刺激前的输出水平)
        ''' </summary>
        Public Property O1 As Double

        ''' <summary>
        ''' The steady state value of the output node after the step to I2.
        ''' (阶跃到 I2 之后输出节点的稳态值)
        ''' </summary>
        Public Property O2 As Double

        ''' <summary>
        ''' The peak value of the output node during the transient after the
        ''' step, i.e. the sample furthest away from <see cref="O1"/>.
        ''' (阶跃后瞬态过程中距离 O1 最远的输出节点极值)
        ''' </summary>
        Public Property Opeak As Double

        ''' <summary>Whether the circuit reached its steady state at the initial input.</summary>
        Public Property Settled As Boolean

        ''' <summary>
        ''' The full state vector at the end of the phase 1 steady state search,
        ''' i.e. the node concentrations at the initial input I1. It is used by
        ''' the mechanism self tests to verify the integral control signature of
        ''' the buffering node (its state changes with the input while the output
        ''' steady state does not).
        ''' (阶段一稳态搜索结束时的完整状态向量，即初始输入下各节点浓度)
        ''' </summary>
        Public Property PreStepState As Double()

        ''' <summary>
        ''' The full state vector at the end of the phase 2 transient, i.e. the
        ''' node concentrations at the stepped input I2.
        ''' (阶段二瞬态结束时的完整状态向量，即阶跃输入下各节点浓度)
        ''' </summary>
        Public Property FinalState As Double()

        ''' <summary>The total number of accepted solver steps over both phases.</summary>
        Public Property TotalSteps As Long

        ''' <summary>The total number of right hand side evaluations over both phases.</summary>
        Public Property RhsEvaluations As Long

        ''' <summary>The diagnostic message of a failed simulation.</summary>
        Public Property Message As String = ""

        ''' <summary>
        ''' The drift of the output node per unit time at the end of the phase 1
        ''' steady state search, i.e. the value that the settle criterion is
        ''' judged on. It is a diagnostic of how well the circuit converged.
        ''' (阶段一稳态搜索结束时输出节点的单位时间漂移，即稳态判据的实测值)
        ''' </summary>
        Public Property OutputDrift As Double

        ''' <summary>
        ''' Whether the circuit is well behaved, i.e. whether its readouts can
        ''' be used by the functional criteria.
        ''' (电路是否表现良好，即其读出值能否用于功能判据)
        ''' </summary>
        Public ReadOnly Property IsWellBehaved As Boolean
            Get
                Return Status = SimulationStatus.OK
            End Get
        End Property

        ''' <summary>
        ''' The status and the readouts as a compact string.
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"[{Status}] O1={O1.ToString("G6")}, Opeak={Opeak.ToString("G6")}, O2={O2.ToString("G6")}"
        End Function

    End Class

End Namespace
