' ============================================================
' Report.vb - 结果导出与控制台打印
' ============================================================

Imports System.Text
Imports Microsoft.VisualBasic.Linq

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
        Dim names As String() = If(variables, series.FirstOrDefault()?.Keys?.OrderBy(Function(k) k).ToArray())

        If names.IsNullOrEmpty Then
            names = {}
        End If

        Dim sb As New StringBuilder()

        sb.Append(timeHeader)

        For Each name In names
            sb.Append("," & CsvField(name))
        Next

        sb.AppendLine()

        For i As Integer = 0 To times.Count - 1
            sb.Append(times(i).ToString("F4"))

            For Each name In names
                Dim v As Double = 0.0

                If series(i) IsNot Nothing Then
                    series(i).TryGetValue(name, v)
                End If

                sb.Append("," & v.ToString("F6"))
            Next

            sb.AppendLine()
        Next

        Call System.IO.File.WriteAllText(path, sb.ToString(), Encoding.UTF8)
    End Sub

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
