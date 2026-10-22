Imports System.Collections.Generic
Imports System.Threading.Tasks

Namespace Assessment

    ''' <summary>
    ''' The evaluation pipeline of one single topology: sample its parameter
    ''' sets, simulate every circuit, assess the functional criteria and
    ''' aggregate the robustness statistics.
    ''' (单个拓扑的评估流水线：采样参数集、逐电路仿真、功能判据评估与鲁棒性统计聚合)
    ''' </summary>
    ''' <remarks>
    ''' This is the inner loop of the topology search and it is shared by the
    ''' search pipeline and by the design table experiment, so that both evaluate
    ''' a topology with exactly the same protocol, the same tolerances and the
    ''' same random seed derivation. The simulation of the circuits is
    ''' embarrassingly parallel: every circuit owns its own model, its own
    ''' solver and its own scratch buffers, so the loop body is thread safe.
    ''' </remarks>
    Public Module TopologyEvaluator

        ''' <summary>
        ''' The status code of a circuit whose model could not even be built,
        ''' which is distinct from the simulation statuses.
        ''' (电路模型构建失败的状态码，区别于仿真状态)
        ''' </summary>
        Public Const ModelBuildFailureStatus As Integer = 999

        ''' <summary>
        ''' Evaluate one topology: sample, simulate, assess and aggregate.
        ''' (评估一个拓扑：采样、仿真、评估、聚合)
        ''' </summary>
        ''' <param name="topology">the topology to evaluate</param>
        ''' <param name="samples">the number of sampled parameter sets</param>
        ''' <param name="seed">the sampler seed of the topology</param>
        ''' <param name="i1">the initial input level of the step protocol</param>
        ''' <param name="i2">the stepped input level of the step protocol</param>
        ''' <param name="simOptions">the simulation options</param>
        ''' <param name="sThreshold">the sensitivity threshold of the functional region</param>
        ''' <param name="pThreshold">the precision threshold of the functional region</param>
        ''' <param name="maxDegreeOfParallelism">
        ''' the maximum number of threads of the inner simulation loop, 0 or a
        ''' negative number for the sequential evaluation
        ''' </param>
        ''' <returns>
        ''' the aggregated topology result and the circuit records that it was
        ''' computed from
        ''' </returns>
        Public Function Evaluate(topology As Model.NetworkTopology,
                                 samples As Integer,
                                 seed As Integer,
                                 i1 As Double,
                                 i2 As Double,
                                 simOptions As Simulation.CircuitSimulator.SimulationOptions,
                                 sThreshold As Double,
                                 pThreshold As Double,
                                 Optional maxDegreeOfParallelism As Integer = 0) As (result As TopologyResult, records As List(Of CircuitRecord))
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If

            Dim parameterSets = SMRUCC.genomics.Analysis.Circuits.Sampling.ParameterSampler.Sample(topology, samples, seed)
            Dim records As CircuitRecord() = New CircuitRecord(samples - 1) {}

            If maxDegreeOfParallelism > 0 Then
                System.Threading.Tasks.Parallel.For(0, samples,
                             New System.Threading.Tasks.ParallelOptions With {.MaxDegreeOfParallelism = maxDegreeOfParallelism},
                             Sub(i)
                                 records(i) = EvaluateCircuit(topology, parameterSets(i), i, i1, i2, simOptions, sThreshold, pThreshold)
                             End Sub)
            Else
                For i As Integer = 0 To samples - 1
                    records(i) = EvaluateCircuit(topology, parameterSets(i), i, i1, i2, simOptions, sThreshold, pThreshold)
                Next
            End If

            Dim result As TopologyResult = FunctionalAssessor.Aggregate(topology, records)

            Return (result, records.ToList())
        End Function

        ''' <summary>
        ''' Evaluate one single circuit of a topology: build the model from the
        ''' topology and the parameter set, simulate the step protocol and assess
        ''' the functional criteria. Any unexpected failure is recorded as an ill
        ''' behaved circuit instead of breaking the search pipeline.
        ''' (评估单个电路：构建模型、仿真阶跃协议、功能判据；任何意外失败记录为病态电路而不中断流水线)
        ''' </summary>
        Private Function EvaluateCircuit(topology As Model.NetworkTopology,
                                         parameterSet As Double(),
                                         sampleIndex As Integer,
                                         i1 As Double,
                                         i2 As Double,
                                         simOptions As Simulation.CircuitSimulator.SimulationOptions,
                                         sThreshold As Double,
                                         pThreshold As Double) As CircuitRecord
            Try
                Dim model As New Model.CircuitModel(topology, parameterSet)
                Dim outcome As Simulation.SimulationOutcome = Simulation.CircuitSimulator.Simulate(model, i1, i2, simOptions)
                Dim record As CircuitRecord = FunctionalAssessor.Assess(outcome, i1, i2, sThreshold, pThreshold)

                record.TopologyId = topology.Id
                record.SampleIndex = sampleIndex

                Return record
            Catch ex As Exception
                Return New CircuitRecord With {
                    .TopologyId = topology.Id,
                    .SampleIndex = sampleIndex,
                    .Sensitivity = Double.NaN,
                    .Precision = Double.NaN,
                    .IsFunctional = False,
                    .IsIllBehaved = True,
                    .Status = ModelBuildFailureStatus
                }
            End Try
        End Function

    End Module

End Namespace
