' ============================================================
' Format.vb - 控制台排版辅助
' ============================================================

Imports Cella
Imports System.Text
Imports Microsoft.VisualBasic.Linq

Public Module Fmt

    ''' <summary>把「名称 → 计数」字典格式化为紧凑的一行文本</summary>
    Public Function Population(table As IDictionary(Of String, Integer)) As String
        If table Is Nothing OrElse table.Count = 0 Then
            Return "(空)"
        End If

        Return String.Join(" ", table _
            .OrderByDescending(Function(x) x.Value) _
            .Select(Function(x) $"{Brief(x.Key)}={x.Value}"))
    End Function

    ''' <summary>把物种 id 缩短成便于对齐的标签</summary>
    Public Function Brief(species As String) As String
        If species Is Nothing Then
            Return "?"
        End If

        Select Case species.ToLowerInvariant()
            Case "glc_fermenter" : Return "S1"
            Case "lactate_utilizer" : Return "S2"
            Case "acetate_utilizer" : Return "S3"
            Case "aa_auxotroph" : Return "S4"
            Case Else
                Return If(species.Length > 6, species.Substring(0, 6), species)
        End Select
    End Function

    ''' <summary>
    ''' 渲染一个 2D 平面切片：每个格点显示「主导物种 + 细胞数」
    ''' </summary>
    Public Function Slice(env As Cella.Environment, z As Integer) As String
        If env Is Nothing OrElse env.Space Is Nothing Then
            Return "(空环境)"
        End If

        Dim sb As New StringBuilder()
        Dim ys As Integer = If(env.Space(0) Is Nothing, 0, env.Space(0).Length)

        sb.AppendLine($"      z={z} 切片（列 = x，行 = y；'.' = 空，S1..S4 = 主导物种，数字 = 细胞数）")

        For y As Integer = 0 To ys - 1
            sb.Append($"  y={y} ")

            For x As Integer = 0 To env.Space.Length - 1
                Dim spot As Cella.Spot = env.GetSpotAt(x, y, z)

                If spot Is Nothing Then
                    sb.Append("  ## ")
                    Continue For
                End If

                If spot.cells.Count = 0 Then
                    If spot.Medium IsNot Nothing AndAlso spot.Medium.Values.Any(Function(v) v > 0.5) Then
                        sb.Append("  .. ")
                    Else
                        sb.Append("  -- ")
                    End If

                    Continue For
                End If

                Dim dominant As String = spot.PopulationBySpecies() _
                    .OrderByDescending(Function(kv) kv.Value) _
                    .First().Key

                ' 细胞数超过 9 时用 '+'，保证每格固定 4 字符宽
                Dim count As String = If(spot.cells.Count > 9, "+", spot.cells.Count.ToString())

                sb.Append($" {Brief(dominant)}{count} ")
            Next

            sb.AppendLine()
        Next

        Return sb.ToString()
    End Function

    ''' <summary>找出细胞数最多的 z 层，用于切片渲染</summary>
    Public Function BusiestSlice(env As Cella.Environment) As Integer
        If env Is Nothing OrElse env.Space Is Nothing OrElse env.Space.Length = 0 Then
            Return 0
        End If

        Dim zCount As Integer = If(env.Space(0)(0) Is Nothing, 1, env.Space(0)(0).Length)
        Dim best As Integer = 0
        Dim bestCount As Integer = -1

        For z As Integer = 0 To zCount - 1
            Dim n As Integer = 0

            For Each spot As Cella.Spot In env.GetAllSpots()
                If spot.index.Z = z Then
                    n += spot.cells.Count
                End If
            Next

            If n > bestCount Then
                bestCount = n
                best = z
            End If
        Next

        Return best
    End Function

End Module
