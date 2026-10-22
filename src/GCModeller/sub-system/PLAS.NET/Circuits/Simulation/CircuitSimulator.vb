Imports std = System.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Sundials.CVODE

Namespace Simulation

    ''' <summary>
    ''' The two phase step simulator of the adaptation circuit search: it
    ''' integrates a circuit at the initial input until the steady state, steps
    ''' the input and records the transient peak of the output node.
    ''' (适应电路搜索的两阶段阶跃仿真器)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The input protocol follows the paper: the initial input is I1 = 0.5,
    ''' the system relaxes to its steady state O1, the input is stepped by 20%
    ''' to I2 = 0.6 and the transient of the output node is recorded until the
    ''' new steady state O2. The output node is the last component of the state
    ''' vector, which is node C for the three node network and the output node
    ''' of the two node control network.
    ''' </para>
    ''' <para>
    ''' The Michaelis Menten terms become very stiff when a sampled K_M reaches
    ''' its lower bound of 0.001, so the stiff BDF method of the CVODE solver is
    ''' used with tight tolerances, as recommended by the README of this
    ''' project.
    ''' </para>
    ''' <para>
    ''' The steady state detection integrates in chunks and stops as soon as
    ''' the drift per unit time drops below <see cref="SimulationOptions.SettleTolerance"/>,
    ''' which keeps non stiff circuits cheap while slow circuits that can not
    ''' relax inside the time horizon are reported as not settled instead of
    ''' being simulated with a wrong O1.
    ''' </para>
    ''' </remarks>
    Public Class CircuitSimulator

        ''' <summary>
        ''' The configuration of the step simulation.
        ''' (阶跃仿真的配置)
        ''' </summary>
        Public Class SimulationOptions

            ''' <summary>The time horizon of both simulation phases, 1000 by default.</summary>
            Public Property TEnd As Double = 1000.0
            ''' <summary>The relative tolerance of the ODE solver.</summary>
            Public Property RelativeTolerance As Double = 0.000001
            ''' <summary>The absolute tolerance of the ODE solver.</summary>
            Public Property AbsoluteTolerance As Double = 0.000000001
            ''' <summary>The integration chunk of the steady state search phase.</summary>
            Public Property SettleChunk As Double = 5.0
            ''' <summary>
            ''' The settle criterion: the maximum absolute drift of a node per
            ''' unit time below which the circuit counts as settled. The default
            ''' bounds the residual drift over the time horizon by 1e-3, which is
            ''' 0.1% of the output scale, while staying above the numerical noise
            ''' floor of the solver tolerances.
            ''' </summary>
            Public Property SettleTolerance As Double = 0.000001
            ''' <summary>The earliest time at which the settle criterion is evaluated.</summary>
            Public Property SettleCheckDelay As Double = 1.0
            ''' <summary>The window of the fine peak sampling after the step.</summary>
            Public Property PeakFineWindow As Double = 10.0
            ''' <summary>The fine sampling step inside of the peak window.</summary>
            Public Property PeakFineStep As Double = 0.5
            ''' <summary>The coarse sampling step after the peak window.</summary>
            Public Property PeakCoarseStep As Double = 10.0
            ''' <summary>The maximum number of accepted solver steps per phase.</summary>
            Public Property MaxSteps As Integer = 100000
            ''' <summary>
            ''' The minimum steady state value of the output node, below which a
            ''' circuit is ill behaved (the paper excludes the too small steady
            ''' states).
            ''' </summary>
            Public Property MinSteadyState As Double = 0.001
            ''' <summary>
            ''' The allowed excursion of a node concentration outside of [0, 1],
            ''' the true solution of the bounded Michaelis Menten system never
            ''' leaves the range, so a larger excursion means a solver failure.
            ''' </summary>
            Public Property PhysicalTolerance As Double = 0.001
            ''' <summary>The initial state of the simulation, 0.5 for every node by default.</summary>
            Public Property InitialState As Double() = {0.5, 0.5, 0.5}

        End Class

        ''' <summary>
        ''' Simulate one circuit with the two phase step protocol.
        ''' (用两阶段阶跃协议仿真一个电路)
        ''' </summary>
        ''' <param name="model">the ODE model of the circuit</param>
        ''' <param name="i1">the initial input level, 0.5 by the paper</param>
        ''' <param name="i2">the stepped input level, 0.6 by the paper</param>
        ''' <param name="options">the simulation options</param>
        ''' <returns>
        ''' the readouts O1, Opeak and O2 together with the well behavedness
        ''' diagnostics; an ill behaved circuit never raises an exception
        ''' </returns>
        Public Shared Function Simulate(model As IOdeModel,
                                        i1 As Double,
                                        i2 As Double,
                                        options As SimulationOptions) As SimulationOutcome
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If
            If options Is Nothing Then
                Throw New ArgumentNullException(NameOf(options))
            End If

            Dim outcome As New SimulationOutcome()
            Dim nDim As Integer = options.InitialState.Length

            Try
                outcome = SimulatePhases(model, i1, i2, options, nDim, outcome)
            Catch ex As Exception
                outcome.Status = SimulationOutcome.SimulationStatus.Exception
                outcome.Message = ex.Message
            End Try

            Return outcome
        End Function

        ''' <summary>
        ''' The two integration phases of the step protocol, separated from
        ''' <see cref="Simulate"/> so that any unexpected failure is recorded as
        ''' an ill behaved circuit instead of breaking the search pipeline.
        ''' </summary>
        Private Shared Function SimulatePhases(model As IOdeModel,
                                               i1 As Double,
                                               i2 As Double,
                                               options As SimulationOptions,
                                               nDim As Integer,
                                               outcome As SimulationOutcome) As SimulationOutcome
            Dim outputNode As Integer = nDim - 1
            Dim state As New Vector(nDim)
            Dim steps As Long = 0
            Dim rhsEvals As Long = 0

            ' ---- phase 1: the initial input, relax towards the steady state ----
            model.Input = i1

            Dim y As Double() = CType(options.InitialState.Clone(), Double())
            Dim rhs As RHSFunction = BuildRhs(model, nDim)
            Dim solver As New CVODESolver(CVODEMethod.BDF, rhs, nDim, BuildSolverOptions(options))
            Dim status As CVODEStatus = solver.Initialize(0.0, New Vector(y))

            If status <> CVODEStatus.Success Then
                outcome.Status = SimulationOutcome.SimulationStatus.SolverFailure
                outcome.Message = $"the phase 1 initialization failed with the status {status}."
                Return outcome
            End If

            Dim t As Double = 0.0
            Dim settled As Boolean = False
            Dim yPrev As Double() = CType(y.Clone(), Double())

            While t < options.TEnd
                Dim tNext As Double = std.Min(t + options.SettleChunk, options.TEnd)

                status = solver.Integrate(tNext, state)

                If status <> CVODEStatus.Success Then
                    outcome.Status = SimulationOutcome.SimulationStatus.SolverFailure
                    outcome.Message = $"the phase 1 integration failed at t={tNext} with the status {status}."
                    Return outcome
                End If

                CopyState(state, y)

                If Not IsFinite(y) Then
                    outcome.Status = SimulationOutcome.SimulationStatus.NonFinite
                    outcome.Message = $"the phase 1 state became non finite at t={tNext}."
                    Return outcome
                End If
                If Not IsPhysical(y, options.PhysicalTolerance) Then
                    outcome.Status = SimulationOutcome.SimulationStatus.NonPhysical
                    outcome.Message = $"the phase 1 state left the range [0, 1] at t={tNext}."
                    Return outcome
                End If

                If tNext >= options.SettleCheckDelay Then
                    Dim drift As Double = MaxAbsDiff(y, yPrev) / (tNext - t)

                    If drift <= options.SettleTolerance Then
                        settled = True
                        Exit While
                    End If
                End If

                Array.Copy(y, yPrev, nDim)
                t = tNext
            End While

            steps += solver.TotalSteps
            rhsEvals += solver.RHSFunctionEvaluations

            outcome.Settled = settled
            outcome.O1 = y(outputNode)

            If Not settled Then
                outcome.Status = SimulationOutcome.SimulationStatus.NotSettled
                outcome.Message = $"the circuit did not settle within the time horizon {options.TEnd}."
                Return outcome
            End If
            If outcome.O1 < options.MinSteadyState Then
                outcome.Status = SimulationOutcome.SimulationStatus.SteadyStateTooSmall
                outcome.Message = $"the steady state of the output node {outcome.O1} is below {options.MinSteadyState}."
                Return outcome
            End If

            ' ---- phase 2: the step, record the transient peak of the output ----
            model.Input = i2

            Dim y2 As Double() = CType(y.Clone(), Double())
            Dim rhs2 As RHSFunction = BuildRhs(model, nDim)
            Dim solver2 As New CVODESolver(CVODEMethod.BDF, rhs2, nDim, BuildSolverOptions(options))

            status = solver2.Initialize(0.0, New Vector(y2))

            If status <> CVODEStatus.Success Then
                outcome.Status = SimulationOutcome.SimulationStatus.SolverFailure
                outcome.Message = $"the phase 2 initialization failed with the status {status}."
                Return outcome
            End If

            Dim cMin As Double = outcome.O1
            Dim cMax As Double = outcome.O1

            t = 0.0

            While t < options.TEnd
                Dim chunk As Double = If(t < options.PeakFineWindow, options.PeakFineStep, options.PeakCoarseStep)
                Dim tNext As Double = std.Min(t + chunk, options.TEnd)

                status = solver2.Integrate(tNext, state)

                If status <> CVODEStatus.Success Then
                    outcome.Status = SimulationOutcome.SimulationStatus.SolverFailure
                    outcome.Message = $"the phase 2 integration failed at t={tNext} with the status {status}."
                    Return outcome
                End If

                CopyState(state, y2)

                If Not IsFinite(y2) Then
                    outcome.Status = SimulationOutcome.SimulationStatus.NonFinite
                    outcome.Message = $"the phase 2 state became non finite at t={tNext}."
                    Return outcome
                End If
                If Not IsPhysical(y2, options.PhysicalTolerance) Then
                    outcome.Status = SimulationOutcome.SimulationStatus.NonPhysical
                    outcome.Message = $"the phase 2 state left the range [0, 1] at t={tNext}."
                    Return outcome
                End If

                Dim c As Double = y2(outputNode)

                If c < cMin Then
                    cMin = c
                End If
                If c > cMax Then
                    cMax = c
                End If

                t = tNext
            End While

            steps += solver2.TotalSteps
            rhsEvals += solver2.RHSFunctionEvaluations

            outcome.O2 = y2(outputNode)

            ' the peak is the transient extreme that is furthest away from O1
            If std.Abs(cMax - outcome.O1) >= std.Abs(outcome.O1 - cMin) Then
                outcome.Opeak = cMax
            Else
                outcome.Opeak = cMin
            End If

            outcome.TotalSteps = steps
            outcome.RhsEvaluations = rhsEvals
            outcome.Status = SimulationOutcome.SimulationStatus.OK

            Return outcome
        End Function

        ''' <summary>
        ''' Build the right hand side closure of a circuit: it copies the state
        ''' out of the solver vector, evaluates the model and copies the
        ''' derivative back, reusing one scratch buffer pair per circuit.
        ''' (构建电路的 ODE 右端闭包，复用暂存缓冲区避免每次求值分配数组)
        ''' </summary>
        Private Shared Function BuildRhs(model As IOdeModel, nDim As Integer) As RHSFunction
            Dim yBuf As Double() = New Double(nDim - 1) {}
            Dim fBuf As Double() = New Double(nDim - 1) {}

            Dim rhs As RHSFunction = Sub(t As Double, y As Vector, ydot As Vector)
                                        For i As Integer = 0 To nDim - 1
                                            yBuf(i) = y(i)
                                        Next

                                        model.Evaluate(yBuf, fBuf)

                                        For i As Integer = 0 To nDim - 1
                                            ydot(i) = fBuf(i)
                                        Next
                                    End Sub

            Return rhs
        End Function

        ''' <summary>
        ''' The solver options of a circuit simulation, the stiff BDF settings
        ''' of the paper with tight tolerances.
        ''' (电路仿真的求解器选项：论文的刚性 BDF 设置与紧容差)
        ''' </summary>
        Private Shared Function BuildSolverOptions(options As SimulationOptions) As CVODEOptions
            Return New CVODEOptions With {
                .RelativeTolerance = options.RelativeTolerance,
                .AbsoluteTolerance = options.AbsoluteTolerance,
                .MaxOrder = 5,
                .MaxSteps = options.MaxSteps,
                .MaxNewtonIterations = 50
            }
        End Function

        ''' <summary>
        ''' Copy the state of the solver vector into a plain array.
        ''' </summary>
        Private Shared Sub CopyState(source As Vector, target As Double())
            For i As Integer = 0 To target.Length - 1
                target(i) = source(i)
            Next
        End Sub

        ''' <summary>
        ''' Whether every component of the state is a finite number.
        ''' </summary>
        Private Shared Function IsFinite(y As Double()) As Boolean
            For i As Integer = 0 To y.Length - 1
                If Double.IsNaN(y(i)) OrElse Double.IsInfinity(y(i)) Then
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' Whether every component of the state stays inside of the physical
        ''' range [0, 1] up to the allowed numerical excursion.
        ''' </summary>
        Private Shared Function IsPhysical(y As Double(), tolerance As Double) As Boolean
            For i As Integer = 0 To y.Length - 1
                If y(i) < -tolerance OrElse y(i) > 1.0 + tolerance Then
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' The maximum absolute difference of two state vectors.
        ''' </summary>
        Private Shared Function MaxAbsDiff(a As Double(), b As Double()) As Double
            Dim m As Double = 0.0

            For i As Integer = 0 To a.Length - 1
                Dim d As Double = std.Abs(a(i) - b(i))

                If d > m Then
                    m = d
                End If
            Next

            Return m
        End Function

    End Class

End Namespace
