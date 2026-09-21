' ============================================================
' Environment.vb - 虚拟细胞生长的空间环境
' ============================================================
' Space 是一个三维交错数组，不同形状（培养皿 / 圆柱发酵罐 / 锥形摇瓶 /
' 长方体发酵池）由 SpaceInitializer 生成，形状之外的格点为 Nothing。
'
' 一个时间步的物理顺序：
'
'   1. Diffuse(dt)          —— 相邻格点之间按浓度差做 Fickian 扩散
'   2. Spot.Tick(dt)        —— 每个细胞推进自己的六个子网络
'   3. CellLifecycle.Step   —— 生物量累积 → 二分裂 / 饥饿死亡 / 老化死亡
'   4. FlagellarMotor.Step  —— 鞭毛运动，在格点之间迁移
'
' 顺序是有意为之：先让物质混合，再让细胞消耗/分泌，然后才根据消耗结果
' 决定谁分裂、谁饿死，最后才是空间迁移——这样「迁移」看到的是本步已经
' 更新过的营养场（也就是趋化性有实际的梯度可追）。
' ============================================================

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Spot 之间的物质扩散 / 补料配置
''' </summary>
Public Class DiffusionConfig

    ''' <summary>Fickian 扩散系数（0 = 不扩散）</summary>
    Public Property Coefficient As Double = 0.05

    ''' <summary>按代谢物覆盖的扩散系数</summary>
    Public Property ByMetabolite As Dictionary(Of String, Double)

    ''' <summary>贴壁格点的补料速率（单位时间把浓度按比例拉回补料水平）</summary>
    Public Property FeedRate As Double = 0.0

    ''' <summary>参与补料的代谢物 id</summary>
    Public Property FeedMetabolites As String()

    ''' <summary>补料目标浓度相对初始培养基的倍数（1.0 = 回补到新鲜培养基水平）</summary>
    Public Property FeedLevel As Double = 1.0

    Public Function CoefficientOf(metabolite As String) As Double
        If ByMetabolite Is Nothing Then
            Return Coefficient
        End If

        Dim d As Double = Coefficient

        If ByMetabolite.TryGetValue(metabolite, d) Then
            Return d
        End If

        Return Coefficient
    End Function

    Public Function IsFeedComponent(metabolite As String) As Boolean
        If FeedRate <= 0 OrElse FeedMetabolites.IsNullOrEmpty Then
            Return False
        End If

        Return FeedMetabolites.Contains(metabolite, StringComparer.OrdinalIgnoreCase)
    End Function

    ''' <summary>从蓝图取一份默认配置</summary>
    Public Shared Function FromBlueprint(blueprint As CellaBlueprint) As DiffusionConfig
        If blueprint Is Nothing Then
            Return New DiffusionConfig()
        End If

        Return New DiffusionConfig With {
            .Coefficient = blueprint.DiffusionCoefficient,
            .ByMetabolite = blueprint.DiffusionByMetabolite,
            .FeedRate = blueprint.BoundaryFeedRate,
            .FeedMetabolites = blueprint.BoundaryFeedMetabolites,
            .FeedLevel = blueprint.BoundaryFeedLevel
        }
    End Function

End Class

Public Class Environment

    ''' <summary>三维空间格点（形状之外的位置为 Nothing）</summary>
    Public Property Space As Spot()()()

    ''' <summary>默认时间步长</summary>
    Public Property TimeStep As Double = 1.0

    ''' <summary>相邻格点之间的物质扩散 / 补料配置</summary>
    Public Property Diffusion As New DiffusionConfig()

    ''' <summary>
    ''' 初始培养基模板（由 SpaceInitializer 写入）：贴壁补料时按此模板
    ''' 乘 <see cref="DiffusionConfig.FeedLevel"/> 作为目标浓度
    ''' </summary>
    Public Property MediumTemplate As Dictionary(Of String, Double)

    ''' <summary>细胞谱系登记簿（代际生长繁殖的进化树）</summary>
    Public ReadOnly Property Lineage As New CellLineage()

    ''' <summary>生命周期与运动使用的随机数发生器</summary>
    Public Property Rand As Random

    ''' <summary>最近一个时间步发生的生命周期事件</summary>
    Public ReadOnly Property LastEvents As LifecycleEvents
        Get
            Return _lastEvents
        End Get
    End Property

    ''' <summary>最近一个时间步发生的迁移事件</summary>
    Public ReadOnly Property LastMigrations As List(Of MigrationEvent)
        Get
            Return _lastMigrations
        End Get
    End Property

    Private _lastEvents As New LifecycleEvents()
    Private _lastMigrations As New List(Of MigrationEvent)()

    ''' <summary>当前仿真时间</summary>
    Public ReadOnly Property CurrentTime As Double
        Get
            Return clock
        End Get
    End Property

    ''' <summary>已经推进的步数</summary>
    Public ReadOnly Property Iteration As Integer
        Get
            Return steps
        End Get
    End Property

    Private clock As Double = 0.0
    Private steps As Integer = 0

    Sub New(Optional seed As Integer = 2024)
        Me.Rand = New Random(seed)
    End Sub

    ''' <summary>推进一个默认时间步</summary>
    Public Sub Tick()
        Call Tick(TimeStep)
    End Sub

    ''' <summary>推进 dt 个时间单位</summary>
    Public Sub Tick(dt As Double)
        If dt <= 0 Then
            Return
        End If

        ' 1. 物质混合
        Call Diffuse(dt)

        ' 2. 细胞内部物理
        For Each spot As Spot In GetAllSpots()
            Call spot.Tick(dt)
        Next

        ' 3. 生命周期：分裂 / 死亡
        _lastEvents = CellLifecycle.Run(Me, dt)

        ' 4. 鞭毛运动：格点间迁移
        _lastMigrations = FlagellarMotor.Swim(Me, dt, Rand)

        clock += dt
        steps += 1
    End Sub

    ''' <summary>连续推进若干步（每步一个 TimeStep）</summary>
    Public Sub Run(steps As Integer)
        For i As Integer = 1 To steps
            Call Tick(TimeStep)
        Next
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAllCells() As IEnumerable(Of VirtualCella)
        Return GetAllSpots().SelectMany(Function(s) s.cells)
    End Function

    ''' <summary>枚举所有有效（非 Nothing）格点</summary>
    Public Function GetAllSpots() As IEnumerable(Of Spot)
        Dim list As New List(Of Spot)

        For Each row As Spot()() In Space.SafeQuery
            For Each col As Spot() In row.SafeQuery
                For Each spot As Spot In col.SafeQuery
                    If spot IsNot Nothing Then
                        list.Add(spot)
                    End If
                Next
            Next
        Next

        Return list
    End Function

    ''' <summary>按坐标取格点；越界或形状之外返回 Nothing</summary>
    Public Function GetSpotAt(x As Integer, y As Integer, z As Integer) As Spot
        If Space Is Nothing Then
            Return Nothing
        End If
        If x < 0 OrElse x >= Space.Length Then
            Return Nothing
        End If
        If Space(x) Is Nothing OrElse y < 0 OrElse y >= Space(x).Length Then
            Return Nothing
        End If
        If Space(x)(y) Is Nothing OrElse z < 0 OrElse z >= Space(x)(y).Length Then
            Return Nothing
        End If

        Return Space(x)(y)(z)
    End Function

    ''' <summary>取一个格点的 x/y/z 六个邻居（形状之外为 Nothing 的会被过滤）</summary>
    Public Function GetNeighbors(spot As Spot) As Spot()
        If spot Is Nothing Then
            Return {}
        End If

        Dim x As Integer = spot.index.X
        Dim y As Integer = spot.index.Y
        Dim z As Integer = spot.index.Z
        Dim list As New List(Of Spot)(6)

        AddNeighbor(list, GetSpotAt(x + 1, y, z))
        AddNeighbor(list, GetSpotAt(x - 1, y, z))
        AddNeighbor(list, GetSpotAt(x, y + 1, z))
        AddNeighbor(list, GetSpotAt(x, y - 1, z))
        AddNeighbor(list, GetSpotAt(x, y, z + 1))
        AddNeighbor(list, GetSpotAt(x, y, z - 1))

        Return list.ToArray()
    End Function

    Private Shared Sub AddNeighbor(list As List(Of Spot), spot As Spot)
        If spot IsNot Nothing Then
            list.Add(spot)
        End If
    End Sub

    ' ==================== 扩散 ====================

    ''' <summary>
    ''' 相邻格点之间的 Fickian 扩散：flux = D · (cA − cB) · dt
    ''' </summary>
    ''' <remarks>
    ''' 只处理 +x / +y / +z 三个方向的正向相邻对，避免同一对格点被计算两次；
    ''' 所有格点的增量先累加到缓冲，最后一次性应用，避免遍历顺序影响结果。
    ''' </remarks>
    Public Sub Diffuse(dt As Double)
        If Diffusion Is Nothing OrElse Diffusion.Coefficient <= 0 Then
            Return
        End If

        ' 只考虑持有培养基的格点
        Dim spots As Spot() = GetAllSpots() _
            .Where(Function(s) s.Medium IsNot Nothing) _
            .ToArray()

        If spots.Length = 0 Then
            Return
        End If

        Dim idSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each spot As Spot In spots
            For Each id In spot.Medium
                Call idSet.Add(id.Key)
            Next
        Next

        Dim ids As String() = idSet.ToArray()

        If ids.Length = 0 Then
            Return
        End If

        ' 缓冲：spot => 每个代谢物的净增量
        Dim delta As New Dictionary(Of Spot, Double())()
        Dim index As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To ids.Length - 1
            index(ids(i)) = i
        Next

        For Each spot As Spot In spots
            delta(spot) = New Double(ids.Length - 1) {}
        Next

        For Each spot As Spot In spots
            Dim x As Integer = spot.index.X
            Dim y As Integer = spot.index.Y
            Dim z As Integer = spot.index.Z

            ' 只向正方向扩散，负方向由对方作为起点处理
            Call Exchange(spot, GetSpotAt(x + 1, y, z), ids, index, delta, dt)
            Call Exchange(spot, GetSpotAt(x, y + 1, z), ids, index, delta, dt)
            Call Exchange(spot, GetSpotAt(x, y, z + 1), ids, index, delta, dt)
        Next

        For Each spot As Spot In spots
            Dim d As Double() = delta(spot)

            For i As Integer = 0 To ids.Length - 1
                If d(i) = 0 Then
                    Continue For
                End If

                Dim level As Double = 0.0

                Call spot.Medium.TryGetValue(ids(i), level)

                level += d(i)

                If level < 0 Then
                    level = 0
                End If
                If Double.IsNaN(level) OrElse Double.IsInfinity(level) Then
                    level = 0
                End If

                spot.Medium(ids(i)) = level
            Next
        Next

        ' 贴壁补料：模拟恒化器 / 补料发酵
        If Diffusion.FeedRate > 0 Then
            Call Feed(spots, dt)
        End If
    End Sub

    Private Sub Exchange(a As Spot, b As Spot, ids As String(),
                         index As Dictionary(Of String, Integer),
                         delta As Dictionary(Of Spot, Double()), dt As Double)
        If b Is Nothing OrElse b.Medium Is Nothing Then
            Return
        End If

        Dim da As Double() = delta(a)
        Dim db As Double() = delta(b)

        For i As Integer = 0 To ids.Length - 1
            Dim id As String = ids(i)
            Dim ca As Double = 0.0
            Dim cb As Double = 0.0

            Call a.Medium.TryGetValue(id, ca)
            Call b.Medium.TryGetValue(id, cb)

            Dim flux As Double = Diffusion.CoefficientOf(id) * (ca - cb) * dt

            da(i) -= flux
            db(i) += flux
        Next
    End Sub

    ''' <summary>
    ''' 贴壁格点补料：处于空间边界（邻居数少于 6）的格点，按一阶松弛
    ''' 把指定的补料成分拉回新鲜培养基水平，模拟恒化器 / 补料发酵
    ''' </summary>
    Private Sub Feed(spots As Spot(), dt As Double)
        Dim rate As Double = System.Math.Min(1.0, Diffusion.FeedRate * dt)

        For Each spot As Spot In spots
            If GetNeighbors(spot).Length >= 6 Then
                Continue For
            End If

            For Each id As String In spot.Medium.Keys.ToArray()
                If Not Diffusion.IsFeedComponent(id) Then
                    Continue For
                End If

                Dim target As Double = 0.0
                Dim initial As Double = 0.0

                If MediumTemplate IsNot Nothing Then
                    Call MediumTemplate.TryGetValue(id, initial)
                End If

                target = initial * Diffusion.FeedLevel

                Dim level As Double = spot.Medium(id)

                spot.Medium(id) = level + rate * (target - level)
            Next
        Next
    End Sub

    ''' <summary>有效格点数量（即当前形状的体积）</summary>
    Public ReadOnly Property Volume As Integer
        Get
            Return GetAllSpots().Count
        End Get
    End Property

    ''' <summary>
    ''' 统计每个物种（或指定分组）在各格点上的细胞数分布
    ''' </summary>
    Public Function PopulationBySpot() As Dictionary(Of Spot, Dictionary(Of String, Integer))
        Dim out As New Dictionary(Of Spot, Dictionary(Of String, Integer))()

        For Each spot As Spot In GetAllSpots()
            out(spot) = spot.PopulationBySpecies()
        Next

        Return out
    End Function

    ''' <summary>整个环境的物种级细胞计数</summary>
    Public Function PopulationBySpecies() As Dictionary(Of String, Integer)
        Dim out As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For Each cella As VirtualCella In GetAllCells()
            Dim key As String = If(cella.Species, "(unknown)")

            If out.ContainsKey(key) Then
                out(key) += 1
            Else
                out(key) = 1
            End If
        Next

        Return out
    End Function

    Public Overrides Function ToString() As String
        Return $"space[{Volume} spots] t={clock:F2} iter={steps} cells={GetAllCells().Count()}"
    End Function

End Class
