#Region "Metabopolis: layout result model"

' ============================================================================
' Metabopolis 布局结果模型
' ----------------------------------------------------------------------------
' 承载流水线各阶段产出的几何信息：
'   * Rect            —— 街区/建筑块的轴对齐矩形（floor-planning 的基本单元）
'   * BlockLayout     —— 一个「城市街区」（= 一个代谢类别）
'   * BuildingBlock   —— 街区内部由 TreeMap 划分出的「建筑块」
'   * Junction        —— 落在街区边界上的枢纽代谢物（论文中的 conjunction）
'   * RoutePolyline   —— 反应边的正交折线路由
'
' 这些类型同时提供 JSON 快照能力，便于回归对比与调试。
' ============================================================================

Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Model

    ''' <summary>
    ''' 两个类别之间代谢物角色组合的九种编码（论文 Figure 4b）。
    ''' </summary>
    ''' <remarks>
    ''' 一个连接两个类别的代谢物，在源类别中可能是产物/底物/两者兼有，
    ''' 在目标类别中同样有三种可能，总计 3 × 3 = 9 种组合。
    ''' 枚举值 = 源类别角色 * 3 + 目标类别角色。
    ''' </remarks>
    Public Enum EdgeRole As Integer

        ''' <summary>源类别中为产物，目标类别中为底物。</summary>
        ProductToReactant = 0
        ''' <summary>源类别中为产物，目标类别中为产物。</summary>
        ProductToProduct = 1
        ''' <summary>源类别中为产物，目标类别中两者兼有。</summary>
        ProductToBoth = 2
        ''' <summary>源类别中为底物，目标类别中为底物。</summary>
        ReactantToReactant = 3
        ''' <summary>源类别中为底物，目标类别中为产物。</summary>
        ReactantToProduct = 4
        ''' <summary>源类别中为底物，目标类别中两者兼有。</summary>
        ReactantToBoth = 5
        ''' <summary>源类别中两者兼有，目标类别中为底物。</summary>
        BothToReactant = 6
        ''' <summary>源类别中两者兼有，目标类别中为产物。</summary>
        BothToProduct = 7
        ''' <summary>源类别中两者兼有，目标类别中两者兼有。</summary>
        BothToBoth = 8

    End Enum

    ''' <summary>
    ''' 轴对齐矩形：左下角 (X, Y) 与宽高，对应论文中的 (x_i, y_i, p_i, q_i)。
    ''' </summary>
    Public Class Rect

        Public Property X As Double
        Public Property Y As Double
        Public Property Width As Double
        Public Property Height As Double

        ''' <summary>右上角 X 坐标（论文中的 p_i）。</summary>
        <JsonIgnore>
        Public ReadOnly Property P As Double
            Get
                Return X + Width
            End Get
        End Property

        ''' <summary>右上角 Y 坐标（论文中的 q_i）。</summary>
        <JsonIgnore>
        Public ReadOnly Property Q As Double
            Get
                Return Y + Height
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property Area As Double
            Get
                Return Width * Height
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property Center As PointF
            Get
                Return New PointF(CSng(X + Width / 2.0), CSng(Y + Height / 2.0))
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property Bounds As RectangleF
            Get
                Return New RectangleF(CSng(X), CSng(Y), CSng(Width), CSng(Height))
            End Get
        End Property

        ''' <summary>判断一个点是否落在矩形内（含边界）。</summary>
        Public Function Contains(px As Double, py As Double) As Boolean
            Return px >= X AndAlso px <= P AndAlso py >= Y AndAlso py <= Q
        End Function

        ''' <summary>与另一个矩形是否相交（允许共边接触）。</summary>
        Public Function IntersectsWith(other As Rect) As Boolean
            If other Is Nothing Then
                Return False
            End If

            Return X < other.P AndAlso other.X < P AndAlso Y < other.Q AndAlso other.Y < Q
        End Function

        ''' <summary>与另一个矩形的重叠面积；不相交时为 0。</summary>
        Public Function OverlapArea(other As Rect) As Double
            If other Is Nothing Then
                Return 0
            End If

            Dim ox As Double = Math.Min(P, other.P) - Math.Max(X, other.X)
            Dim oy As Double = Math.Min(Q, other.Q) - Math.Max(Y, other.Y)

            If ox <= 0 OrElse oy <= 0 Then
                Return 0
            End If

            Return ox * oy
        End Function

        ''' <summary>向外扩张 pad 像素（pad 为负时收缩）。</summary>
        Public Function Inflate(pad As Double) As Rect
            Return New Rect With {
                .X = X - pad,
                .Y = Y - pad,
                .Width = Width + 2 * pad,
                .Height = Height + 2 * pad
            }
        End Function

        ''' <summary>与另一个矩形的外接矩形。</summary>
        Public Function Union(other As Rect) As Rect
            If other Is Nothing Then
                Return Clone()
            End If

            Dim x1 As Double = Math.Min(X, other.X)
            Dim y1 As Double = Math.Min(Y, other.Y)
            Dim x2 As Double = Math.Max(P, other.P)
            Dim y2 As Double = Math.Max(Q, other.Q)

            Return New Rect With {
                .X = x1,
                .Y = y1,
                .Width = x2 - x1,
                .Height = y2 - y1
            }
        End Function

        ''' <summary>沿 x 轴方向的共享边长（用于 CS3 长共享边界约束）。</summary>
        Public Function SharedLengthX(other As Rect) As Double
            If other Is Nothing Then
                Return 0
            End If

            Return Math.Max(0, Math.Min(Q, other.Q) - Math.Max(Y, other.Y))
        End Function

        ''' <summary>沿 y 轴方向的共享边长。</summary>
        Public Function SharedLengthY(other As Rect) As Double
            If other Is Nothing Then
                Return 0
            End If

            Return Math.Max(0, Math.Min(P, other.P) - Math.Max(X, other.X))
        End Function

        Public Function Clone() As Rect
            Return New Rect With {
                .X = X,
                .Y = Y,
                .Width = Width,
                .Height = Height
            }
        End Function

        Public Shared Function FromSize(x As Double, y As Double, width As Double, height As Double) As Rect
            Return New Rect With {
                .X = x,
                .Y = y,
                .Width = Math.Max(0, width),
                .Height = Math.Max(0, height)
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"({X:0.##},{Y:0.##}) {Width:0.##}x{Height:0.##}"
        End Function

    End Class

    ''' <summary>
    ''' 一个「城市街区」的最终几何：类别 + 矩形 + 配色索引。
    ''' </summary>
    Public Class BlockLayout

        Public Property CategoryId As String
        Public Property Label As String
        Public Property X As Double
        Public Property Y As Double
        Public Property Width As Double
        Public Property Height As Double

        ''' <summary>面积权重（来自类别内反应物/产物数量）。</summary>
        Public Property Weight As Double

        ''' <summary>调色板索引，保证同一类别在多次渲染中颜色一致。</summary>
        Public Property ColorIndex As Integer

        ''' <summary>街区的几何矩形。</summary>
        <JsonIgnore>
        Public ReadOnly Property Box As Rect
            Get
                Return Metabopolis.Model.Rect.FromSize(X, Y, Width, Height)
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property Center As PointF
            Get
                Return Box.Center
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{CategoryId} @ {Box}"
        End Function

    End Class

    ''' <summary>
    ''' 街区内部由 TreeMap 划分出的建筑块（论文中的 building block）。
    ''' </summary>
    Public Class BuildingBlock

        Public Property CategoryId As String
        Public Property X As Double
        Public Property Y As Double
        Public Property Width As Double
        Public Property Height As Double

        ''' <summary>落在本建筑块内的反应编号。</summary>
        Public Property ReactionIds As String()

        ''' <summary>落在本建筑块内的代谢物编号。</summary>
        Public Property MetaboliteIds As String()

        ''' <summary>建筑块的几何矩形。</summary>
        <JsonIgnore>
        Public ReadOnly Property Box As Rect
            Get
                Return Metabopolis.Model.Rect.FromSize(X, Y, Width, Height)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{CategoryId} bldg {Box}"
        End Function

    End Class

    ''' <summary>
    ''' 街区边界上的枢纽代谢物（论文中的 conjunction）。
    ''' </summary>
    ''' <remarks>
    ''' 跨类别的高连接度代谢物（例如 ATP）会被放置在每个参与街区的边界上，
    ''' 作为该街区的对外接口，块间路由即从这里出发。
    ''' </remarks>
    Public Class Junction

        Public Property Id As String
        Public Property Label As String

        ''' <summary>所属街区（类别）。</summary>
        Public Property CategoryId As String

        Public Property X As Double
        Public Property Y As Double

        ''' <summary>该代谢物参与的类别数量（枢纽程度）。</summary>
        Public Property Degree As Integer

        <JsonIgnore>
        Public ReadOnly Property Point As PointF
            Get
                Return New PointF(CSng(X), CSng(Y))
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Id} @ ({X:0.##},{Y:0.##})"
        End Function

    End Class

    ''' <summary>
    ''' 一条反应边的正交折线路由结果。
    ''' </summary>
    Public Class RoutePolyline

        Public Property Id As String

        ''' <summary>出发点（源代谢物或源反应的节点编号）。</summary>
        Public Property SourceId As String

        ''' <summary>到达点（目标节点编号）。</summary>
        Public Property TargetId As String

        ''' <summary>该路由所代表的代谢物编号。</summary>
        Public Property MetaboliteId As String

        ''' <summary>九色角色编码。</summary>
        Public Property Role As EdgeRole

        ''' <summary>是否为有向边（不可逆反应产生的车道为有向）。</summary>
        Public Property IsDirected As Boolean

        ''' <summary>显示标签（通常为代谢物名称）。</summary>
        Public Property Label As String

        ''' <summary>流量/权重，用于线宽编码。</summary>
        Public Property Weight As Double = 1

        ''' <summary>
        ''' 折线的顶点序列，每个元素为 [x, y]。使用数组而非 PointF
        ''' 是为了让 JSON 快照保持紧凑且与平台绘图类型解耦。
        ''' </summary>
        Public Property Points As Double()()

        ''' <summary>把折线转换为绘图用的点序列。</summary>
        Public Function Polyline() As PointF()
            If Points Is Nothing Then
                Return New PointF() {}
            End If

            Dim result As New List(Of PointF)(Points.Length)

            For Each pt As Double() In Points
                If pt Is Nothing OrElse pt.Length < 2 Then
                    Continue For
                End If

                result.Add(New PointF(CSng(pt(0)), CSng(pt(1))))
            Next

            Return result.ToArray
        End Function

        ''' <summary>折线总长度，用于线束宽度与统计。</summary>
        Public Function TotalLength() As Double
            Dim pts As PointF() = Polyline()

            If pts.Length < 2 Then
                Return 0
            End If

            Dim sum As Double = 0

            For i As Integer = 1 To pts.Length - 1
                Dim dx As Double = pts(i).X - pts(i - 1).X
                Dim dy As Double = pts(i).Y - pts(i - 1).Y
                sum += Math.Sqrt(dx * dx + dy * dy)
            Next

            Return sum
        End Function

        Public Shared Function Create(id As String,
                                      sourceId As String,
                                      targetId As String,
                                      metaboliteId As String,
                                      role As EdgeRole,
                                      directed As Boolean,
                                      points As IEnumerable(Of PointF),
                                      Optional label As String = Nothing,
                                      Optional weight As Double = 1) As RoutePolyline

            Dim buffer As New List(Of Double())

            For Each pt As PointF In points.SafeQuery
                buffer.Add(New Double() {CDbl(pt.X), CDbl(pt.Y)})
            Next

            Return New RoutePolyline With {
                .Id = id,
                .SourceId = sourceId,
                .TargetId = targetId,
                .MetaboliteId = metaboliteId,
                .Role = role,
                .IsDirected = directed,
                .Label = label,
                .Weight = weight,
                .Points = buffer.ToArray
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{Id} {SourceId} -> {TargetId} ({Role})"
        End Function

    End Class

    ''' <summary>
    ''' 完整布局结果，是渲染层的唯一输入。
    ''' </summary>
    Public Class NetworkLayout

        Public Property CanvasWidth As Double
        Public Property CanvasHeight As Double

        Public Property Blocks As BlockLayout()
        Public Property Buildings As BuildingBlock()
        Public Property Routes As RoutePolyline()
        Public Property Junctions As Junction()

        ''' <summary>阶段耗时、规模等元信息。</summary>
        Public Property Meta As Dictionary(Of String, String)

        ''' <summary>按类别编号查询街区。</summary>
        Public Function GetBlock(categoryId As String) As BlockLayout
            For Each block As BlockLayout In Blocks.SafeQuery
                If String.Equals(block.CategoryId, categoryId, StringComparison.Ordinal) Then
                    Return block
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>全部街区的外接矩形。</summary>
        Public Function Bounds() As Rect
            Dim rect As Rect = Nothing

            For Each block As BlockLayout In Blocks.SafeQuery
                If rect Is Nothing Then
                    rect = block.Box
                Else
                    rect = rect.Union(block.Box)
                End If
            Next

            Return rect
        End Function

        ''' <summary>把布局的几何范围归一化到 <c>[margin, canvas - margin]</c>。</summary>
        ''' <remarks>
        ''' floor-planning 产出的是相对坐标；渲染前统一平移到画布内，
        ''' 保证不同规模的数据集都能落在可视区域内。
        ''' </remarks>
        Public Sub Normalize(margin As Double)
            Dim bounds As Rect = Bounds()

            If bounds Is Nothing OrElse bounds.Width <= 0 OrElse bounds.Height <= 0 Then
                Return
            End If

            Dim targetWidth As Double = Math.Max(1, CanvasWidth - 2 * margin)
            Dim targetHeight As Double = Math.Max(1, CanvasHeight - 2 * margin)
            Dim scale As Double = Math.Min(targetWidth / bounds.Width, targetHeight / bounds.Height)

            ' 布局坐标放大/缩小，并整体平移，使外接矩形贴合画布
            Dim ox As Double = margin + (targetWidth - bounds.Width * scale) / 2.0
            Dim oy As Double = margin + (targetHeight - bounds.Height * scale) / 2.0

            For Each block As BlockLayout In Blocks.SafeQuery
                Dim nx As Double = ox + (block.X - bounds.X) * scale
                Dim ny As Double = oy + (block.Y - bounds.Y) * scale
                block.Width *= scale
                block.Height *= scale
                block.X = nx
                block.Y = ny
            Next

            For Each bldg As BuildingBlock In Buildings.SafeQuery
                Dim nx As Double = ox + (bldg.X - bounds.X) * scale
                Dim ny As Double = oy + (bldg.Y - bounds.Y) * scale
                bldg.Width *= scale
                bldg.Height *= scale
                bldg.X = nx
                bldg.Y = ny
            Next

            For Each junction As Junction In Junctions.SafeQuery
                junction.X = ox + (junction.X - bounds.X) * scale
                junction.Y = oy + (junction.Y - bounds.Y) * scale
            Next

            For Each route As RoutePolyline In Routes.SafeQuery
                For Each pt As Double() In route.Points.SafeQuery
                    If pt Is Nothing OrElse pt.Length < 2 Then
                        Continue For
                    End If

                    pt(0) = ox + (pt(0) - bounds.X) * scale
                    pt(1) = oy + (pt(1) - bounds.Y) * scale
                Next
            Next
        End Sub

        ''' <summary>生成统计摘要。</summary>
        Public Function Statistics() As String
            Dim sb As New StringBuilder()
            Dim length As Double = 0

            For Each route As RoutePolyline In Routes.SafeQuery
                length += route.TotalLength()
            Next

            sb.AppendLine($"canvas       : {CanvasWidth:0} x {CanvasHeight:0}")
            sb.AppendLine($"blocks       : {If(Blocks Is Nothing, 0, Blocks.Length)}")
            sb.AppendLine($"buildings    : {If(Buildings Is Nothing, 0, Buildings.Length)}")
            sb.AppendLine($"junctions    : {If(Junctions Is Nothing, 0, Junctions.Length)}")
            sb.AppendLine($"routes       : {If(Routes Is Nothing, 0, Routes.Length)} ({length:0} px)")

            Return sb.ToString()
        End Function

        ''' <summary>写出 JSON 快照。</summary>
        Public Function ToJson(path As String) As NetworkLayout
            Dim dir As String = Path.GetDirectoryName(Path.GetFullPath(path))

            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Directory.CreateDirectory(dir)
            End If

            Dim json As String = JsonSerializer.Serialize(Me, JsonOptions())
            File.WriteAllText(path, json)

            Return Me
        End Function

        ''' <summary>读取 JSON 快照。</summary>
        Public Shared Function LoadJson(path As String) As NetworkLayout
            Dim json As String = File.ReadAllText(path)
            Return JsonSerializer.Deserialize(Of NetworkLayout)(json, JsonOptions())
        End Function

        Private Shared Function JsonOptions() As JsonSerializerOptions
            Return New JsonSerializerOptions With {
                .WriteIndented = True,
                .PropertyNameCaseInsensitive = True,
                .Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"layout: {If(Blocks Is Nothing, 0, Blocks.Length)} blocks / {If(Buildings Is Nothing, 0, Buildings.Length)} buildings / {If(Routes Is Nothing, 0, Routes.Length)} routes"
        End Function

    End Class

End Namespace

#End Region
