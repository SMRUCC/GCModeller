Imports std = System.Math
Imports System.Globalization
Imports System.Linq

Namespace Assessment

    ''' <summary>
    ''' The functional criteria of the paper: the sensitivity S, the precision P
    ''' and the functional region that an adaptation circuit has to fall into.
    ''' (论文的功能判据：灵敏度 S、精度 P 与功能区判定)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' With the relative input change ``dI = |I2 - I1| / I1`` of the step
    ''' protocol, the two functional quantities of the paper are
    ''' </para>
    ''' <code>
    ''' S = (|O_peak - O1| / O1) / dI
    ''' P = 1 / ((|O2 - O1| / O1) / dI)
    ''' </code>
    ''' <para>
    ''' A circuit is functional when ``S > 1`` and ``P > 10``, i.e. the output
    ''' responds to the step by more than the relative size of the step itself
    ''' and returns to within 2% of its pre stimulus level. The robustness Q of
    ''' a topology is the number of its sampled parameter sets that land inside
    ''' of this region.
    ''' </para>
    ''' </remarks>
    Public Module FunctionalAssessor

        ''' <summary>The default sensitivity threshold of the functional region.</summary>
        Public Const DefaultSensitivityThreshold As Double = 1.0
        ''' <summary>The default precision threshold of the functional region.</summary>
        Public Const DefaultPrecisionThreshold As Double = 10.0

        ''' <summary>
        ''' The relative change of the input signal of a step protocol.
        ''' (阶跃协议的输入相对变化量)
        ''' </summary>
        ''' <param name="i1">the initial input level</param>
        ''' <param name="i2">the stepped input level</param>
        Public Function InputChange(i1 As Double, i2 As Double) As Double
            Return std.Abs(i2 - i1) / i1
        End Function

        ''' <summary>
        ''' The sensitivity S of a circuit: the peak response of the output
        ''' relative to the pre stimulus level, divided by the relative input
        ''' change.
        ''' (电路的灵敏度 S)
        ''' </summary>
        ''' <param name="o1">the steady state output at the initial input</param>
        ''' <param name="opeak">the peak output during the transient after the step</param>
        ''' <param name="inputChange">the relative input change of the step</param>
        Public Function Sensitivity(o1 As Double, opeak As Double, inputChange As Double) As Double
            Return std.Abs(opeak - o1) / o1 / inputChange
        End Function

        ''' <summary>
        ''' The precision P of a circuit: the inverse of the relative steady
        ''' state deviation of the output after the step.
        ''' (电路的精度 P)
        ''' </summary>
        ''' <param name="o1">the steady state output at the initial input</param>
        ''' <param name="o2">the steady state output at the stepped input</param>
        ''' <param name="inputChange">the relative input change of the step</param>
        Public Function Precision(o1 As Double, o2 As Double, inputChange As Double) As Double
            Dim relativeError As Double = std.Abs(o2 - o1) / o1 / inputChange

            If relativeError <= 0 Then
                Return Double.PositiveInfinity
            End If

            Return 1.0 / relativeError
        End Function

        ''' <summary>
        ''' Whether a circuit falls into the functional region of the paper.
        ''' (电路是否落入论文的功能区)
        ''' </summary>
        ''' <param name="sensitivity">the sensitivity S</param>
        ''' <param name="precision">the precision P</param>
        ''' <param name="sThreshold">the sensitivity threshold, 1 by the paper</param>
        ''' <param name="pThreshold">the precision threshold, 10 by the paper</param>
        Public Function IsFunctional(sensitivity As Double,
                                     precision As Double,
                                     Optional sThreshold As Double = DefaultSensitivityThreshold,
                                     Optional pThreshold As Double = DefaultPrecisionThreshold) As Boolean
            Return sensitivity > sThreshold AndAlso precision > pThreshold
        End Function

        ''' <summary>
        ''' Assess one simulation outcome of a circuit and turn it into the
        ''' record that the statistics of the search are computed from. An ill
        ''' behaved circuit is recorded with its status and is never counted as
        ''' a functional circuit.
        ''' (评估单个电路的仿真结果，生成统计记录；病态电路只记录状态不计入功能统计)
        ''' </summary>
        ''' <param name="outcome">the simulation outcome of the circuit</param>
        ''' <param name="i1">the initial input level of the step protocol</param>
        ''' <param name="i2">the stepped input level of the step protocol</param>
        ''' <param name="sThreshold">the sensitivity threshold</param>
        ''' <param name="pThreshold">the precision threshold</param>
        Public Function Assess(outcome As Simulation.SimulationOutcome,
                               i1 As Double,
                               i2 As Double,
                               Optional sThreshold As Double = DefaultSensitivityThreshold,
                               Optional pThreshold As Double = DefaultPrecisionThreshold) As CircuitRecord
            Dim record As New CircuitRecord With {
                .O1 = outcome.O1,
                .O2 = outcome.O2,
                .Opeak = outcome.Opeak,
                .OutputDrift = outcome.OutputDrift,
                .Status = CInt(outcome.Status)
            }

            If Not outcome.IsWellBehaved Then
                record.Sensitivity = Double.NaN
                record.Precision = Double.NaN
                record.IsFunctional = False
                record.IsIllBehaved = True
                Return record
            End If

            Dim dInput As Double = InputChange(i1, i2)

            record.Sensitivity = Sensitivity(outcome.O1, outcome.Opeak, dInput)
            record.Precision = Precision(outcome.O1, outcome.O2, dInput)
            record.IsFunctional = IsFunctional(record.Sensitivity, record.Precision, sThreshold, pThreshold)
            record.IsIllBehaved = False

            Return record
        End Function

        ''' <summary>
        ''' Aggregate the circuit records of one topology into its robustness
        ''' statistics: the Q value is the number of functional circuits.
        ''' (把一个拓扑的全部电路记录聚合为其鲁棒性统计，Q 值即功能电路数量)
        ''' </summary>
        ''' <param name="topology">the topology of the aggregated circuits</param>
        ''' <param name="records">the circuit records of the topology</param>
        Public Function Aggregate(topology As Model.NetworkTopology,
                                  records As IEnumerable(Of CircuitRecord)) As TopologyResult
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If
            If records Is Nothing Then
                Throw New ArgumentNullException(NameOf(records))
            End If

            Dim list As List(Of CircuitRecord) = records.ToList()
            Dim result As New TopologyResult With {
                .TopologyId = topology.Id,
                .Signs = topology.ToSymbolString(),
                .LinkCount = topology.LinkCount,
                .Simulated = list.Count
            }

            For Each record As CircuitRecord In list
                If record.IsIllBehaved Then
                    result.IllBehaved += 1
                ElseIf record.IsFunctional Then
                    result.Functional += 1
                End If
            Next

            Dim functional As CircuitRecord() = list _
                .Where(Function(r) Not r.IsIllBehaved AndAlso r.IsFunctional) _
                .ToArray()

            If functional.Length > 0 Then
                result.MeanSensitivity = functional.Average(Function(r) r.Sensitivity)
                result.MaxSensitivity = functional.Max(Function(r) r.Sensitivity)
                result.MeanPrecision = functional.Average(Function(r) r.Precision)
                result.MeanO1 = functional.Average(Function(r) r.O1)
            Else
                result.MeanSensitivity = Double.NaN
                result.MaxSensitivity = Double.NaN
                result.MeanPrecision = Double.NaN
                result.MeanO1 = Double.NaN
            End If

            Return result
        End Function

        ''' <summary>
        ''' Format a double value for the CSV reports, using the invariant
        ''' culture so that the reports are stable across locales.
        ''' (用不变文化格式化 CSV 报告中的双精度数值)
        ''' </summary>
        ''' <param name="value">the value to format</param>
        Public Function Format(value As Double) As String
            Return value.ToString("G6", CultureInfo.InvariantCulture)
        End Function

    End Module

End Namespace
