' ============================================================
' CrossFeedingRecorder.vb - 交叉喂养网络代谢流
' ============================================================
' 交叉喂养（cross-feeding）指的是「一个物种分泌的代谢物被另一个物种摄取」。
' 细胞的分泌与摄取都由跨膜转运系统给出（TransportSystem.EffluxMass /
' UptakeMass，单位是「本步的物质质量」），这里把同一 Spot 内的分泌与摄取
' 配对成生产者 → 消费者的有向边。
'
' 归因规则（近似，必须在解读结果时记住）：
'   同一 Spot 内、同一个胞外代谢物，其分泌总量按各消费者的摄取占比分摊：
'
'       flux(p → c) = out(p) · in(c) / Σ_c' in(c')
'
'   也就是说，我们无法（也不需要）追踪单个分子，而是假定「池子里的物质是
'   充分混合的」。这正是 Spot 内 Fickian 扩散的意义所在：混合越充分，
'   该近似越准确。同物种内部的摄取不计入交叉喂养（自食不算互养）。
'
' 跨 Spot 的传递由扩散项承担，随后在每个 Spot 内部重新归因。
' ============================================================

Imports System.Text
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 一条「生产者物种 → 消费者物种」的交叉喂养边
''' </summary>
Public Class CrossFeedingEdge

    Public Property metabolite As String
    Public Property producer As String
    Public Property consumer As String

    ''' <summary>累计传递的通量（整个仿真过程）</summary>
    Public Property flux As Double = 0.0

    ''' <summary>最近一个时间步的通量</summary>
    Public Property last_flux As Double = 0.0

    ''' <summary>所属格点坐标（物种级聚合边为 0）</summary>
    Public Property x As Integer
    Public Property y As Integer
    Public Property z As Integer

    Public Function Key() As String
        Return $"{metabolite}|{producer}|{consumer}"
    End Function

    Public Overrides Function ToString() As String
        Return $"{producer} --{metabolite}({flux:F4})--> {consumer}"
    End Function

End Class

''' <summary>
''' 一个时间步上、某个 Spot 内的一条交叉喂养通量采样
''' </summary>
Public Class CrossFeedingSample

    Public Property time As Double

    Public Property x As Integer
    Public Property y As Integer
    Public Property z As Integer

    Public Property metabolite As String
    Public Property producer As String
    Public Property consumer As String
    Public Property flux As Double

End Class

''' <summary>
''' 交叉喂养网络记录器
''' </summary>
Public Class CrossFeedingRecorder

    ''' <summary>Spot 内明细：key = "x,y,z|metabolite|producer|consumer"</summary>
    Private ReadOnly perSpot As New Dictionary(Of String, CrossFeedingEdge)(StringComparer.OrdinalIgnoreCase)

    ''' <summary>物种级聚合：key = "metabolite|producer|consumer"</summary>
    Private ReadOnly summary As New Dictionary(Of String, CrossFeedingEdge)(StringComparer.OrdinalIgnoreCase)

    ''' <summary>时间序列采样</summary>
    Private ReadOnly timeline As New List(Of CrossFeedingSample)()

    ''' <summary>最近一个时间步的总通量（所有 Spot 之和）</summary>
    Public ReadOnly Property LastStepFlux As Double
        Get
            Return _lastStepFlux
        End Get
    End Property

    Private _lastStepFlux As Double = 0.0

    ''' <summary>Spot 内明细边（按累计通量降序）</summary>
    Public ReadOnly Property SpotEdges As IEnumerable(Of CrossFeedingEdge)
        Get
            Return perSpot.Values _
                .Where(Function(e) e.flux > 0) _
                .OrderByDescending(Function(e) e.flux) _
                .ToArray()
        End Get
    End Property

    ''' <summary>物种级聚合边（按累计通量降序）</summary>
    Public ReadOnly Property SummaryEdges As IEnumerable(Of CrossFeedingEdge)
        Get
            Return summary.Values _
                .Where(Function(e) e.flux > 0) _
                .OrderByDescending(Function(e) e.flux) _
                .ToArray()
        End Get
    End Property

    Public ReadOnly Property Samples As IEnumerable(Of CrossFeedingSample)
        Get
            Return timeline
        End Get
    End Property

    ''' <summary>物种级累计总通量（整个仿真过程，所有 Spot 之和）</summary>
    Public ReadOnly Property TotalFlux As Double
        Get
            Dim sum As Double = 0.0

            For Each edge In summary.Values
                sum += edge.flux
            Next

            Return sum
        End Get
    End Property

    ''' <summary>
    ''' 采集一个时间步的交叉喂养通量
    ''' </summary>
    Public Sub Collect(env As Environment, time As Double)
        _lastStepFlux = 0.0

        If env Is Nothing Then
            Return
        End If

        For Each spot As Spot In env.GetAllSpots()
            If spot.cells.Count = 0 Then
                Continue For
            End If

            ' ---- 1. 按物种汇总该 Spot 内每个胞外代谢物的分泌量与摄取量 ----
            Dim outPool As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.OrdinalIgnoreCase)
            Dim inPool As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.OrdinalIgnoreCase)
            Dim totalIn As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            For Each cella As VirtualCella In spot.cells
                Dim transport As TransportSystem = cella.transportation

                If transport Is Nothing Then
                    Continue For
                End If

                Dim species As String = If(cella.Species, "(unknown)")
                Dim names As String() = transport.BoundaryMetabolites

                For i As Integer = 0 To names.Length - 1
                    Dim metabolite As String = names(i)
                    Dim outMass As Double = transport.EffluxMass(i)
                    Dim inMass As Double = transport.UptakeMass(i)

                    If outMass > 0 Then
                        Call Accumulate(outPool, metabolite, species, outMass)
                    End If

                    If inMass > 0 Then
                        Call Accumulate(inPool, metabolite, species, inMass)

                        Dim sum As Double = 0.0

                        Call totalIn.TryGetValue(metabolite, sum)
                        totalIn(metabolite) = sum + inMass
                    End If
                Next
            Next

            ' ---- 2. 按摄取占比把分泌量分摊到各个消费者物种 ----
            For Each metabolite As String In totalIn.Keys.ToArray()
                Dim pool As Double = totalIn(metabolite)

                If pool <= 0 Then
                    Continue For
                End If

                Dim producers As Dictionary(Of String, Double) = Nothing
                Dim consumers As Dictionary(Of String, Double) = Nothing

                If Not outPool.TryGetValue(metabolite, producers) Then
                    Continue For
                End If
                If Not inPool.TryGetValue(metabolite, consumers) Then
                    Continue For
                End If

                For Each producer In producers
                    If producer.Value <= 0 Then
                        Continue For
                    End If

                    For Each consumer In consumers
                        If String.Equals(producer.Key, consumer.Key, StringComparison.OrdinalIgnoreCase) Then
                            ' 同物种自食不计入交叉喂养
                            Continue For
                        End If

                        Dim flux As Double = producer.Value * consumer.Value / pool

                        If flux <= 0 Then
                            Continue For
                        End If

                        _lastStepFlux += flux

                        Dim spotEdge As CrossFeedingEdge = GetOrAdd(
                            perSpot,
                            $"{spot.index.X},{spot.index.Y},{spot.index.Z}|{metabolite}|{producer.Key}|{consumer.Key}",
                            metabolite, producer.Key, consumer.Key, spot.index.X, spot.index.Y, spot.index.Z)

                        spotEdge.flux += flux
                        spotEdge.last_flux = flux

                        Dim summaryEdge As CrossFeedingEdge = GetOrAdd(
                            summary, spotEdge.Key(), metabolite, producer.Key, consumer.Key, 0, 0, 0)

                        summaryEdge.flux += flux
                        summaryEdge.last_flux = flux

                        timeline.Add(New CrossFeedingSample With {
                            .time = time,
                            .x = spot.index.X,
                            .y = spot.index.Y,
                            .z = spot.index.Z,
                            .metabolite = metabolite,
                            .producer = producer.Key,
                            .consumer = consumer.Key,
                            .flux = flux
                        })
                    Next
                Next
            Next
        Next
    End Sub

    Private Shared Function GetOrAdd(table As Dictionary(Of String, CrossFeedingEdge),
                                     key As String, metabolite As String,
                                     producer As String, consumer As String,
                                     x As Integer, y As Integer, z As Integer) As CrossFeedingEdge

        Dim edge As CrossFeedingEdge = Nothing

        If table.TryGetValue(key, edge) Then
            Return edge
        End If

        edge = New CrossFeedingEdge With {
            .metabolite = metabolite,
            .producer = producer,
            .consumer = consumer
        }

        edge.x = x
        edge.y = y
        edge.z = z

        table(key) = edge

        Return edge
    End Function

    Private Shared Sub Accumulate(table As Dictionary(Of String, Dictionary(Of String, Double)),
                                  key As String, species As String, mass As Double)
        Dim row As Dictionary(Of String, Double) = Nothing

        If Not table.TryGetValue(key, row) Then
            row = New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
            table(key) = row
        End If

        Dim sum As Double = 0.0

        Call row.TryGetValue(species, sum)
        row(species) = sum + mass
    End Sub

    ' ==================== 导出 ====================

    ''' <summary>Spot 内交叉喂养代谢流明细</summary>
    Public Function ToSpotCsv() As String
        Dim sb As New StringBuilder()

        sb.AppendLine("x,y,z,metabolite,producer,consumer,flux,last_flux")

        For Each item In perSpot.Values.Where(Function(e) e.flux > 0).OrderByDescending(Function(e) e.flux)
            sb.Append(item.x).Append(",")
            sb.Append(item.y).Append(",")
            sb.Append(item.z).Append(",")
            sb.Append(item.metabolite).Append(",")
            sb.Append(item.producer).Append(",")
            sb.Append(item.consumer).Append(",")
            sb.Append(item.flux.ToString("F6")).Append(",")
            sb.AppendLine(item.last_flux.ToString("F6"))
        Next

        Return sb.ToString()
    End Function

    ''' <summary>物种级交叉喂养代谢流（跨所有 Spot 聚合，附该边出现的 Spot 数）</summary>
    Public Function ToSummaryCsv() As String
        Dim spotCount As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For Each item In perSpot.Values
            Dim key As String = item.Key()
            Dim n As Integer = 0

            Call spotCount.TryGetValue(key, n)
            spotCount(key) = n + 1
        Next

        Dim sb As New StringBuilder()

        sb.AppendLine("metabolite,producer,consumer,flux,last_flux,spot_count")

        For Each edge In SummaryEdges
            Dim n As Integer = 0

            Call spotCount.TryGetValue(edge.Key(), n)

            sb.Append(edge.metabolite).Append(",")
            sb.Append(edge.producer).Append(",")
            sb.Append(edge.consumer).Append(",")
            sb.Append(edge.flux.ToString("F6")).Append(",")
            sb.Append(edge.last_flux.ToString("F6")).Append(",")
            sb.AppendLine(n)
        Next

        Return sb.ToString()
    End Function

    ''' <summary>交叉喂养通量的时间序列</summary>
    Public Function ToTimelineCsv() As String
        Dim sb As New StringBuilder()

        sb.AppendLine("time,x,y,z,metabolite,producer,consumer,flux")

        For Each sample In timeline
            sb.Append(sample.time.ToString("F2")).Append(",")
            sb.Append(sample.x).Append(",")
            sb.Append(sample.y).Append(",")
            sb.Append(sample.z).Append(",")
            sb.Append(sample.metabolite).Append(",")
            sb.Append(sample.producer).Append(",")
            sb.Append(sample.consumer).Append(",")
            sb.AppendLine(sample.flux.ToString("F6"))
        Next

        Return sb.ToString()
    End Function

End Class
