Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text

Namespace IO

    ''' <summary>
    ''' The checkpoint store of the topology search: it persists the evaluation
    ''' of every topology into its own file, so that an interrupted search can
    ''' be resumed without repeating the finished topologies.
    ''' (拓扑搜索的断点存储：把每个拓扑的评估结果持久化到独立文件，支持中断后续跑)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' One checkpoint file owns two kinds of rows, both tagged so that the
    ''' parsing is unambiguous:
    ''' </para>
    ''' <list type="bullet">
    ''' <item> a ``META`` row with the topology identity and the sample and ill behaved counts; </item>
    ''' <item> one ``CIRCUIT`` row per well behaved circuit with its readouts and its functional quantities. </item>
    ''' </list>
    ''' <para>
    ''' The functional flag of a circuit is not stored, it is recomputed from
    ''' the stored sensitivity and precision when the checkpoint is loaded, so
    ''' that a resumed run can apply different functional thresholds without
    ''' invalidating the checkpoints of a previous run. The aggregated Q value
    ''' and the sensitivity statistics are recomputed in the same way.
    ''' </para>
    ''' <para>
    ''' The values of a row never contain a comma, the sign vectors are symbol
    ''' strings and the numbers use the invariant culture, so a plain split
    ''' parsing is safe for the files that this module writes itself.
    ''' </para>
    ''' </remarks>
    Public Module CheckpointStore

        ''' <summary>The row tag of the metadata row of a checkpoint file.</summary>
        Public Const MetaTag As String = "META"
        ''' <summary>The row tag of the circuit rows of a checkpoint file.</summary>
        Public Const CircuitTag As String = "CIRCUIT"

        ''' <summary>
        ''' The file path of the checkpoint of a topology.
        ''' (一个拓扑的断点文件路径)
        ''' </summary>
        ''' <param name="dir">the checkpoint directory</param>
        ''' <param name="topologyId">the topology identifier</param>
        Public Function CheckpointPath(dir As String, topologyId As Integer) As String
            Return Path.Combine(dir, $"topo_{topologyId:00000}.csv")
        End Function

        ''' <summary>
        ''' Whether the checkpoint of a topology exists in the directory.
        ''' (目录中是否存在一个拓扑的断点文件)
        ''' </summary>
        ''' <param name="dir">the checkpoint directory</param>
        ''' <param name="topologyId">the topology identifier</param>
        Public Function Exists(dir As String, topologyId As Integer) As Boolean
            Return File.Exists(CheckpointPath(dir, topologyId))
        End Function

        ''' <summary>
        ''' Save the evaluation of one topology into its checkpoint file: the
        ''' metadata row plus one row per well behaved circuit.
        ''' (把一个拓扑的评估结果保存到其断点文件：元数据行加每个良态电路一行)
        ''' </summary>
        ''' <param name="dir">the checkpoint directory</param>
        ''' <param name="topology">the evaluated topology</param>
        ''' <param name="records">the circuit records of the topology</param>
        Public Sub Save(dir As String,
                        topology As Model.NetworkTopology,
                        records As List(Of Assessment.CircuitRecord))
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If
            If records Is Nothing Then
                Throw New ArgumentNullException(NameOf(records))
            End If

            If Not Directory.Exists(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Dim illBehaved As Integer = records.Where(Function(r) r.IsIllBehaved).Count()
            Dim sb As New StringBuilder()

            Call sb.Append(MetaTag).Append(","c)
            Call sb.Append(topology.Id.ToString(CultureInfo.InvariantCulture)).Append(","c)
            Call sb.Append(topology.ToSymbolString()).Append(","c)
            Call sb.Append(topology.LinkCount.ToString(CultureInfo.InvariantCulture)).Append(","c)
            Call sb.Append(records.Count.ToString(CultureInfo.InvariantCulture)).Append(","c)
            Call sb.Append(illBehaved.ToString(CultureInfo.InvariantCulture))
            Call sb.AppendLine()

            For Each record In records.Where(Function(r) Not r.IsIllBehaved)
                Call sb.Append(CircuitTag).Append(","c)
                Call sb.Append(record.SampleIndex.ToString(CultureInfo.InvariantCulture)).Append(","c)
                Call sb.Append(Assessment.FunctionalAssessor.Format(record.O1)).Append(","c)
                Call sb.Append(Assessment.FunctionalAssessor.Format(record.Opeak)).Append(","c)
                Call sb.Append(Assessment.FunctionalAssessor.Format(record.O2)).Append(","c)
                Call sb.Append(Assessment.FunctionalAssessor.Format(record.Sensitivity)).Append(","c)
                Call sb.Append(Assessment.FunctionalAssessor.Format(record.Precision)).Append(","c)
                Call sb.Append(Assessment.FunctionalAssessor.Format(record.OutputDrift)).Append(","c)
                Call sb.Append(record.Status.ToString(CultureInfo.InvariantCulture))
                Call sb.AppendLine()
            Next

            Call File.WriteAllText(CheckpointPath(dir, topology.Id), sb.ToString(), New UTF8Encoding(False))
        End Sub

        ''' <summary>
        ''' Load the evaluation of one topology from its checkpoint file and
        ''' recompute its functional flags and its aggregated statistics with the
        ''' given thresholds.
        ''' (从断点文件加载一个拓扑的评估结果，并用给定阈值重新计算功能标志与聚合统计)
        ''' </summary>
        ''' <param name="dir">the checkpoint directory</param>
        ''' <param name="topologyId">the topology identifier</param>
        ''' <param name="sThreshold">the sensitivity threshold of the functional region</param>
        ''' <param name="pThreshold">the precision threshold of the functional region</param>
        ''' <returns>
        ''' the reloaded topology result and its circuit records, or nothing when
        ''' the checkpoint file does not exist or can not be parsed
        ''' </returns>
        Public Function Load(dir As String,
                             topologyId As Integer,
                             sThreshold As Double,
                             pThreshold As Double) As (result As Assessment.TopologyResult, records As List(Of Assessment.CircuitRecord))
            Dim path As String = CheckpointPath(dir, topologyId)

            If Not File.Exists(path) Then
                Return (Nothing, Nothing)
            End If

            Try
                Dim lines As String() = File.ReadAllLines(path)
                Dim records As New List(Of Assessment.CircuitRecord)()
                Dim topologyIdValue As Integer = topologyId
                Dim signs As String = ""
                Dim linkCount As Integer = 0
                Dim simulated As Integer = 0
                Dim illBehaved As Integer = 0
                Dim hasMeta As Boolean = False

                For Each line As String In lines
                    If String.IsNullOrWhiteSpace(line) Then
                        Continue For
                    End If

                    Dim fields As String() = line.Split(","c)

                    If String.Equals(fields(0), MetaTag, StringComparison.Ordinal) Then
                        ' META,id,signs,link_count,simulated,ill_behaved
                        topologyIdValue = Integer.Parse(fields(1), CultureInfo.InvariantCulture)
                        signs = fields(2)
                        linkCount = Integer.Parse(fields(3), CultureInfo.InvariantCulture)
                        simulated = Integer.Parse(fields(4), CultureInfo.InvariantCulture)
                        illBehaved = Integer.Parse(fields(5), CultureInfo.InvariantCulture)
                        hasMeta = True
                    ElseIf String.Equals(fields(0), CircuitTag, StringComparison.Ordinal) Then
                        ' CIRCUIT,sample,O1,Opeak,O2,S,P,drift,status
                        Dim record As New Assessment.CircuitRecord With {
                            .TopologyId = topologyIdValue,
                            .SampleIndex = Integer.Parse(fields(1), CultureInfo.InvariantCulture),
                            .O1 = Double.Parse(fields(2), CultureInfo.InvariantCulture),
                            .Opeak = Double.Parse(fields(3), CultureInfo.InvariantCulture),
                            .O2 = Double.Parse(fields(4), CultureInfo.InvariantCulture),
                            .Sensitivity = Double.Parse(fields(5), CultureInfo.InvariantCulture),
                            .Precision = Double.Parse(fields(6), CultureInfo.InvariantCulture),
                            .OutputDrift = Double.Parse(fields(7), CultureInfo.InvariantCulture),
                            .Status = Integer.Parse(fields(8), CultureInfo.InvariantCulture),
                            .IsIllBehaved = False
                        }

                        record.IsFunctional = Assessment.FunctionalAssessor.IsFunctional(record.Sensitivity, record.Precision, sThreshold, pThreshold)
                        records.Add(record)
                    End If
                Next

                If Not hasMeta Then
                    Return (Nothing, Nothing)
                End If

                Dim result As New Assessment.TopologyResult With {
                    .TopologyId = topologyIdValue,
                    .Signs = signs,
                    .LinkCount = linkCount,
                    .Simulated = simulated,
                    .IllBehaved = illBehaved
                }

                For Each record In records.Where(Function(r) r.IsFunctional)
                    result.Functional += 1
                Next

                Dim functional = records.Where(Function(r) r.IsFunctional).ToArray()

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

                Return (result, records)
            Catch ex As Exception
                ' a corrupted checkpoint is treated as a missing one, the
                ' topology is simply evaluated again by the search
                Return (Nothing, Nothing)
            End Try
        End Function

        ''' <summary>
        ''' Load every checkpoint of a directory.
        ''' (加载目录中的全部断点)
        ''' </summary>
        ''' <param name="dir">the checkpoint directory</param>
        ''' <param name="sThreshold">the sensitivity threshold of the functional region</param>
        ''' <param name="pThreshold">the precision threshold of the functional region</param>
        Public Iterator Function LoadAll(dir As String,
                                         sThreshold As Double,
                                         pThreshold As Double) As IEnumerable(Of (result As Assessment.TopologyResult, records As List(Of Assessment.CircuitRecord)))
            If Not Directory.Exists(dir) Then
                Return
            End If

            For Each filePath As String In Directory.EnumerateFiles(dir, "topo_*.csv").OrderBy(Function(p) p)
                Dim name As String = Path.GetFileNameWithoutExtension(filePath)

                If name Is Nothing OrElse Not name.StartsWith("topo_", StringComparison.Ordinal) Then
                    Continue For
                End If

                Dim idText As String = name.Substring("topo_".Length)
                Dim topologyId As Integer

                If Not Integer.TryParse(idText, topologyId) Then
                    Continue For
                End If

                Dim loaded = Load(dir, topologyId, sThreshold, pThreshold)

                If loaded.result IsNot Nothing Then
                    Yield loaded
                End If
            Next
        End Function

    End Module

End Namespace
