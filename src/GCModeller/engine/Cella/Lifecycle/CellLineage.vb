' ============================================================
' CellLineage.vb - 细胞谱系记录（代际生长繁殖的进化树）
' ============================================================
' 每一次二分裂都会登记一条「亲代 → 子代」的记录；细胞死亡时回填死亡时间
' 与死因。由于死亡记录也被保留，最终可以得到一棵完整的出生-死亡谱系树，
' 而不仅仅是存活细胞的快照。
'
' 导出两种形式：
'   * CSV    —— 扁平的 cell_id / parent_id / generation / birth / death 表
'   * Newick —— 可直接被系统发育树软件读取的谱系森林
' ============================================================

Imports System.Text
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 谱系记录中的一条细胞生命周期记录
''' </summary>
Public Class CellLineageRecord

    Public Property cell_id As String
    Public Property parent_id As String
    Public Property species As String
    Public Property generation As Integer

    Public Property birth_time As Double
    Public Property death_time As Double?
    Public Property death_cause As String

    ''' <summary>出生格点坐标</summary>
    Public Property birth_x As Integer
    Public Property birth_y As Integer
    Public Property birth_z As Integer

    ''' <summary>当前（或死亡时）所在格点坐标</summary>
    Public Property x As Integer
    Public Property y As Integer
    Public Property z As Integer

    ''' <summary>产出的子代数量</summary>
    Public Property offspring As Integer

    ''' <summary>是否存活</summary>
    Public ReadOnly Property is_alive As Boolean
        Get
            Return Not death_time.HasValue
        End Get
    End Property

    ''' <summary>生命周期时长：存活者为当前时刻，死亡者为死亡时刻</summary>
    Public Function Lifespan(now_ As Double) As Double
        Return If(death_time, now_) - birth_time
    End Function

End Class

''' <summary>
''' 细胞谱系登记簿
''' </summary>
Public Class CellLineage

    Private ReadOnly table As New Dictionary(Of String, CellLineageRecord)(StringComparer.OrdinalIgnoreCase)

    ''' <summary>已登记的细胞总数（含已死亡个体）</summary>
    Public ReadOnly Property Count As Integer
        Get
            Return table.Count
        End Get
    End Property

    ''' <summary>历史最大代次</summary>
    Public ReadOnly Property MaxGeneration As Integer
        Get
            Dim max As Integer = 0

            For Each r In table.Values
                If r.generation > max Then
                    max = r.generation
                End If
            Next

            Return max
        End Get
    End Property

    Public ReadOnly Property AllRecords As IEnumerable(Of CellLineageRecord)
        Get
            Return table.Values.OrderBy(Function(r) r.birth_time).ToArray()
        End Get
    End Property

    ''' <summary>登记一个新细胞（初次接种时调用）</summary>
    Public Function Register(cell As VirtualCella, time As Double) As CellLineageRecord
        Dim record As New CellLineageRecord With {
            .cell_id = cell.Id,
            .parent_id = cell.ParentId,
            .species = cell.Species,
            .generation = cell.Generation,
            .birth_time = time,
            .death_time = Nothing,
            .death_cause = Nothing,
            .offspring = 0
        }

        If cell.Spot IsNot Nothing Then
            record.birth_x = cell.Spot.index.X
            record.birth_y = cell.Spot.index.Y
            record.birth_z = cell.Spot.index.Z
            record.x = record.birth_x
            record.y = record.birth_y
            record.z = record.birth_z
        End If

        table(cell.Id) = record

        Return record
    End Function

    ''' <summary>记录一次分裂</summary>
    Public Sub RecordDivision(parent As VirtualCella, child As VirtualCella, time As Double)
        Call Register(child, time)

        Dim record As CellLineageRecord = Nothing

        If table.TryGetValue(parent.Id, record) Then
            record.offspring += 1
        Else
            Call Register(parent, time)
        End If
    End Sub

    ''' <summary>回填死亡信息</summary>
    Public Sub RecordDeath(cell As VirtualCella, cause As String, time As Double)
        Dim record As CellLineageRecord = Nothing

        If Not table.TryGetValue(cell.Id, record) Then
            record = Register(cell, time)
        End If

        record.death_time = time
        record.death_cause = cause
    End Sub

    ''' <summary>刷新细胞的当前位置（游动之后调用）</summary>
    Public Sub UpdateLocation(cell As VirtualCella)
        Dim record As CellLineageRecord = Nothing

        If cell.Spot Is Nothing Then
            Return
        End If

        If table.TryGetValue(cell.Id, record) Then
            record.x = cell.Spot.index.X
            record.y = cell.Spot.index.Y
            record.z = cell.Spot.index.Z
        End If
    End Sub

    ''' <summary>存活个体数</summary>
    Public Function AliveCount() As Integer
        Dim n As Integer = 0

        For Each r In table.Values
            If r.is_alive Then
                n += 1
            End If
        Next

        Return n
    End Function

    ''' <summary>按物种统计存活个体数</summary>
    Public Function AliveBySpecies() As Dictionary(Of String, Integer)
        Dim out As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For Each r In table.Values
            If Not r.is_alive Then
                Continue For
            End If

            Dim key As String = If(r.species, "(unknown)")

            If out.ContainsKey(key) Then
                out(key) += 1
            Else
                out(key) = 1
            End If
        Next

        Return out
    End Function

    ''' <summary>按死因统计死亡个体数</summary>
    Public Function DeathsByCause() As Dictionary(Of String, Integer)
        Dim out As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For Each r In table.Values
            If r.is_alive Then
                Continue For
            End If

            Dim key As String = If(r.death_cause, "(unknown)")

            If out.ContainsKey(key) Then
                out(key) += 1
            Else
                out(key) = 1
            End If
        Next

        Return out
    End Function

    ''' <summary>平均代次</summary>
    Public Function MeanGeneration(Optional aliveOnly As Boolean = True) As Double
        Dim sum As Double = 0.0
        Dim n As Integer = 0

        For Each r In table.Values
            If aliveOnly AndAlso Not r.is_alive Then
                Continue For
            End If

            sum += r.generation
            n += 1
        Next

        If n = 0 Then
            Return 0.0
        End If

        Return sum / n
    End Function

    ' ==================== 导出 ====================

    Public Function ToCsv() As String
        Dim sb As New StringBuilder()

        sb.AppendLine("cell_id,parent_id,species,generation,birth_time,death_time,death_cause,offspring,birth_x,birth_y,birth_z,x,y,z,alive")

        For Each r In AllRecords
            sb.Append(r.cell_id).Append(",")
            sb.Append(r.parent_id).Append(",")
            sb.Append(r.species).Append(",")
            sb.Append(r.generation).Append(",")
            sb.Append(r.birth_time.ToString("F4")).Append(",")
            sb.Append(If(r.death_time.HasValue, r.death_time.Value.ToString("F4"), "")).Append(",")
            sb.Append(r.death_cause).Append(",")
            sb.Append(r.offspring).Append(",")
            sb.Append(r.birth_x).Append(",").Append(r.birth_y).Append(",").Append(r.birth_z).Append(",")
            sb.Append(r.x).Append(",").Append(r.y).Append(",").Append(r.z).Append(",")
            sb.AppendLine(If(r.is_alive, "1", "0"))
        Next

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 导出为 Newick 格式的谱系森林（叶节点 = 存活细胞，内部节点 = 分裂事件）
    ''' </summary>
    ''' <remarks>
    ''' 分支长度 = 该节点从出生到分裂（或到当前时刻）的时长。为抑制深度过大
    ''' 的树，内部节点采用「一次分裂 = 一个内部节点」的标准谱系树表示。
    ''' </remarks>
    Public Function ToNewick(now_ As Double) As String
        Dim children As New Dictionary(Of String, List(Of String))(StringComparer.OrdinalIgnoreCase)

        For Each r In table.Values
            If r.parent_id Is Nothing Then
                Continue For
            End If

            If Not table.ContainsKey(r.parent_id) Then
                Continue For
            End If

            Dim list As List(Of String) = Nothing

            If Not children.TryGetValue(r.parent_id, list) Then
                list = New List(Of String)()
                children(r.parent_id) = list
            End If

            list.Add(r.cell_id)
        Next

        Dim sb As New StringBuilder()
        Dim roots As CellLineageRecord() = table.Values _
            .Where(Function(r) r.parent_id Is Nothing OrElse Not table.ContainsKey(r.parent_id)) _
            .OrderBy(Function(r) r.birth_time) _
            .ToArray()

        For i As Integer = 0 To roots.Length - 1
            If i > 0 Then
                sb.AppendLine()
            End If

            Call AppendNode(sb, roots(i), children, now_)
        Next

        Return sb.ToString()
    End Function

    Private Sub AppendNode(sb As StringBuilder, record As CellLineageRecord,
                           children As Dictionary(Of String, List(Of String)),
                           now_ As Double)
        Dim kids As List(Of String) = Nothing

        sb.Append("(")

        If children.TryGetValue(record.cell_id, kids) AndAlso kids.Count > 0 Then
            For i As Integer = 0 To kids.Count - 1
                If i > 0 Then
                    sb.Append(",")
                End If

                Dim childRecord As CellLineageRecord = Nothing

                If table.TryGetValue(kids(i), childRecord) Then
                    Call AppendNode(sb, childRecord, children, now_)
                End If
            Next
        Else
            ' 叶节点：用一个无名末端表示死亡谱线
            sb.Append("extinct")
        End If

        sb.Append(")")

        Dim lifespan As Double = record.Lifespan(now_)

        If lifespan < 0 Then
            lifespan = 0
        End If

        sb.Append("'").Append(record.cell_id).Append("|").Append(record.species).Append("|G").Append(record.generation).Append("':")
        sb.Append(lifespan.ToString("F4"))
    End Sub

End Class
