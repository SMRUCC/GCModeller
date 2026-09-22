' ============================================================
' Report.vb - 结果导出与控制台打印
' ============================================================

Imports Cella
Imports System.Text
Imports Microsoft.VisualBasic.Linq

''' <summary>某一个 Spot 在某一时刻的细胞数（按物种细分）</summary>
Public Class PopulationSample

    Public Property time As Double
    Public Property x As Integer
    Public Property y As Integer
    Public Property z As Integer
    Public Property species As String
    Public Property cell_count As Integer

End Class

Public Module Report

    ''' <summary>结果输出目录（demo/result）</summary>
    Public Function ResultDirectory() As String
        Dim dir As String = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "result")

        If Not System.IO.Directory.Exists(dir) Then
            Call System.IO.Directory.CreateDirectory(dir)
        End If

        Return dir
    End Function

    Public Sub Section(title As String)
        Console.WriteLine()
        Console.WriteLine(New String("="c, 72))
        Console.WriteLine($"  {title}")
        Console.WriteLine(New String("="c, 72))
    End Sub

    Public Sub Line(Optional text As String = "")
        Console.WriteLine(text)
    End Sub

    Public Sub KeyValue(key As String, value As Object)
        Console.WriteLine($"  {key,-34} : {value}")
    End Sub

    ''' <summary>
    ''' 导出一张「时间 × 变量」的宽表
    ''' </summary>
    Public Sub SaveSeries(path As String,
                          timeHeader As String,
                          times As List(Of Double),
                          series As List(Of Dictionary(Of String, Double)),
                          Optional variables As String() = Nothing)

        Call System.IO.File.WriteAllText(path, SaveSeriesText(timeHeader, times, series, variables), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' 生成一张「时间 × 变量」宽表的 CSV 文本（调用方自行决定落盘位置）
    ''' </summary>
    Public Function SaveSeriesText(timeHeader As String,
                                   times As List(Of Double),
                                   series As List(Of Dictionary(Of String, Double)),
                                   Optional variables As String() = Nothing) As String

        Dim names As String() = variables

        If names.IsNullOrEmpty Then
            Dim probe As Dictionary(Of String, Double) = series.FirstOrDefault()

            names = If(probe Is Nothing, {}, probe.Keys.OrderBy(Function(k) k).ToArray())
        End If

        Dim sb As New StringBuilder()

        sb.Append(timeHeader)

        For Each name As String In names
            sb.Append("," & CsvField(name))
        Next

        sb.AppendLine()

        For i As Integer = 0 To times.Count - 1
            sb.Append(times(i).ToString("F4"))

            For Each name As String In names
                Dim v As Double = 0.0

                If i < series.Count AndAlso series(i) IsNot Nothing Then
                    series(i).TryGetValue(name, v)
                End If

                If Double.IsNaN(v) OrElse Double.IsInfinity(v) Then
                    v = 0.0
                End If

                sb.Append("," & v.ToString("F6"))
            Next

            sb.AppendLine()
        Next

        Return sb.ToString()
    End Function

    ''' <summary>径向分带的细胞类型组成</summary>
    Public Function RadialBandCsv(samples As IEnumerable(Of RadialSample)) As String
        Dim sb As New StringBuilder()

        sb.AppendLine("time,band,band_label,fate,cell_count")

        For Each s In samples.SafeQuery
            sb.Append(s.time.ToString("F2")).Append(",")
            sb.Append(s.band).Append(",")
            sb.Append(s.band_label).Append(",")
            sb.Append(s.fate).Append(",")
            sb.AppendLine(s.count)
        Next

        Return sb.ToString()
    End Function

    ''' <summary>细胞命运转换事件</summary>
    Public Function FateSwitchCsv(records As IEnumerable(Of FateSwitchRecord)) As String
        Dim sb As New StringBuilder()

        sb.AppendLine("time,cell_id,from_fate,to_fate,generation,radial_position,local_density,induction,inhibition")

        For Each r In records.SafeQuery
            sb.Append(r.time.ToString("F2")).Append(",")
            sb.Append(r.cell_id).Append(",")
            sb.Append(r.from_fate).Append(",")
            sb.Append(r.to_fate).Append(",")
            sb.Append(r.generation).Append(",")
            sb.Append(r.radial_position.ToString("F4")).Append(",")
            sb.Append(r.local_density.ToString("F4")).Append(",")
            sb.Append(r.induction.ToString("F4")).Append(",")
            sb.AppendLine(r.inhibition.ToString("F4"))
        Next

        Return sb.ToString()
    End Function

    ''' <summary>位置校正（细胞插入）事件</summary>
    Public Function SortingCsv(records As IEnumerable(Of SortingRecord)) As String
        Dim sb As New StringBuilder()

        sb.AppendLine("time,cell_id,fate,from_radius,to_radius,from_x,from_y,from_z,to_x,to_y,to_z")

        For Each r In records.SafeQuery
            sb.Append(r.time.ToString("F2")).Append(",")
            sb.Append(r.cell_id).Append(",")
            sb.Append(r.fate).Append(",")
            sb.Append(r.from_radius.ToString("F4")).Append(",")
            sb.Append(r.to_radius.ToString("F4")).Append(",")
            sb.Append(r.from_x).Append(",").Append(r.from_y).Append(",").Append(r.from_z).Append(",")
            sb.Append(r.to_x).Append(",").Append(r.to_y).Append(",")
            sb.AppendLine(r.to_z)
        Next

        Return sb.ToString()
    End Function

    ''' <summary>取细胞最多的 z 层，用于切片渲染</summary>
    Public Function CenterSlice(env As Cella.Environment) As Integer
        If env Is Nothing OrElse env.Space Is Nothing OrElse env.Space.Length = 0 Then
            Return 0
        End If

        Dim best As Integer = 0
        Dim bestCount As Integer = -1

        For Each spot As Cella.Spot In env.GetAllSpots()
            If spot.cells.Count > bestCount Then
                bestCount = spot.cells.Count
                best = spot.index.Z
            End If
        Next

        Return best
    End Function

    ''' <summary>
    ''' 导出一张通用的二维表
    ''' </summary>
    Public Sub SaveTable(path As String, headers As String(), rows As IEnumerable(Of String()))
        Dim sb As New StringBuilder()

        sb.AppendLine(String.Join(",", headers.Select(AddressOf CsvField)))

        For Each row In rows.SafeQuery
            sb.AppendLine(String.Join(",", row.Select(AddressOf CsvField)))
        Next

        Call System.IO.File.WriteAllText(path, sb.ToString(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' 导出一条数值序列（如损失曲线）
    ''' </summary>
    Public Sub SaveCurve(path As String, header As String, values As IEnumerable(Of Double))
        Dim sb As New StringBuilder()

        sb.AppendLine(header)

        Dim i As Integer = 0

        For Each v In values.SafeQuery
            sb.AppendLine($"{i},{v.ToString("F8")}")
            i += 1
        Next

        Call System.IO.File.WriteAllText(path, sb.ToString(), Encoding.UTF8)
    End Sub

    ' ==================== 群落专属导出 ====================

    ''' <summary>每个 Spot 内的细胞数量分布（时间 × 格点 × 物种）</summary>
    Public Function SpotPopulationCsv(samples As IEnumerable(Of PopulationSample)) As String
        Dim sb As New StringBuilder()

        sb.AppendLine("time,x,y,z,species,cell_count")

        For Each s In samples.SafeQuery
            sb.Append(s.time.ToString("F2")).Append(",")
            sb.Append(s.x).Append(",")
            sb.Append(s.y).Append(",")
            sb.Append(s.z).Append(",")
            sb.Append(s.species).Append(",")
            sb.AppendLine(s.cell_count)
        Next

        Return sb.ToString()
    End Function

    ''' <summary>鞭毛运动事件</summary>
    Public Function MigrationCsv(events As IEnumerable(Of MigrationEvent)) As String
        Dim sb As New StringBuilder()

        sb.AppendLine("time,cell_id,species,from_x,from_y,from_z,to_x,to_y,to_z,motility,chemotaxis,nutrient_before,nutrient_after")

        For Each e In events.SafeQuery
            sb.Append(e.time.ToString("F2")).Append(",")
            sb.Append(e.cell_id).Append(",")
            sb.Append(e.species).Append(",")
            sb.Append(e.from_x).Append(",").Append(e.from_y).Append(",").Append(e.from_z).Append(",")
            sb.Append(e.to_x).Append(",").Append(e.to_y).Append(",").Append(e.to_z).Append(",")
            sb.Append(e.motility.ToString("F4")).Append(",")
            sb.Append(e.chemotaxis.ToString("F4")).Append(",")
            sb.Append(e.nutrient_before.ToString("F4")).Append(",")
            sb.AppendLine(e.nutrient_after.ToString("F4"))
        Next

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 每个 Spot 的营养指标与占用情况（表格 + 一张最繁忙切片的 ASCII 图）
    ''' </summary>
    Public Function NutrientMap(env As Cella.Environment, blueprint As CellaBlueprint, time As Double) As String
        Dim sb As New StringBuilder()

        sb.AppendLine($"  -- 格点占用与营养场（t={time:F1}，共 {env.Volume} 个格点）--")
        sb.AppendLine("     x    y    z   cells  nutrient  species")

        Dim spots As Spot() = env.GetAllSpots() _
            .OrderByDescending(Function(s) s.cells.Count) _
            .ThenBy(Function(s) s.index.Z) _
            .ThenBy(Function(s) s.index.Y) _
            .ToArray()

        For Each spot As Spot In spots
            sb.Append(String.Format("  {0,5} {1,4} {2,4} {3,6} {4,9:F3}  {5}",
                spot.index.X, spot.index.Y, spot.index.Z,
                spot.cells.Count,
                spot.NutrientLevel(blueprint),
                Fmt.Population(spot.PopulationBySpecies())))
            sb.AppendLine()
        Next

        sb.AppendLine()
        sb.AppendLine(Fmt.Slice(env, Fmt.BusiestSlice(env)))

        Return sb.ToString()
    End Function

    Private Function CsvField(x As String) As String
        If x Is Nothing Then
            Return ""
        End If

        If x.Contains(",") OrElse x.Contains("""") Then
            Return """" & x.Replace("""", """""") & """"
        End If

        Return x
    End Function

    ''' <summary>
    ''' 把若干个细胞的同名变量求平均
    ''' </summary>
    Public Function Average(samples As IEnumerable(Of Dictionary(Of String, Double))) As Dictionary(Of String, Double)
        Dim sum As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        Dim count As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For Each s In samples.SafeQuery
            For Each item In s
                If sum.ContainsKey(item.Key) Then
                    sum(item.Key) += item.Value
                    count(item.Key) += 1
                Else
                    sum(item.Key) = item.Value
                    count(item.Key) = 1
                End If
            Next
        Next

        Dim out As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        For Each item In sum
            out(item.Key) = item.Value / System.Math.Max(1, count(item.Key))
        Next

        Return out
    End Function

End Module
