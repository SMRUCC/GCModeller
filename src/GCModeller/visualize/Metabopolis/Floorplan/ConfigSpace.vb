#Region "Metabopolis: rectangle configuration space"

' ============================================================================
' 矩形配置空间（论文 CH1 贴附约束的几何基础）
' ----------------------------------------------------------------------------
' 论文把 bc(i) 沿参考点反射后与 bc(j) 做 Minkowski 和，再取凸包，得到
' 「两个矩形刚好贴合且不重叠」的全部参考点位置（配置空间），并把它分解成
' k = 4 条线段 L(r)。
'
' 对于轴对齐矩形，Minkowski 和退化为四条很规整的线段，无需通用凸包计算：
'   * bc(j) 贴在 bc(i) 左侧：x_j = x_i - W_j，y_j ∈ [y_i - H_j, q_i]
'   * bc(j) 贴在 bc(i) 下侧：y_j = y_i - H_j，x_j ∈ [x_i - W_j, p_i]
'   * bc(j) 贴在 bc(i) 右侧：x_j = p_i，      y_j ∈ [y_i - H_j, q_i]
'   * bc(j) 贴在 bc(i) 上侧：y_j = q_i，      x_j ∈ [x_i - W_j, p_i]
' 这与论文「k = 4 by default since a rectangle has four boundaries」完全一致。
' ============================================================================

Imports System.Drawing
Imports Metabopolis.Model

Namespace Floorplan

    ''' <summary>
    ''' bc(j) 相对 bc(i) 的贴附方位（= 落在 bc(i) 的哪一条边界上）。
    ''' </summary>
    Public Enum AttachSide As Integer

        ''' <summary>bc(j) 位于 bc(i) 左侧。</summary>
        Left = 0
        ''' <summary>bc(j) 位于 bc(i) 下侧。</summary>
        Bottom = 1
        ''' <summary>bc(j) 位于 bc(i) 右侧。</summary>
        Right = 2
        ''' <summary>bc(j) 位于 bc(i) 上侧。</summary>
        Top = 3

    End Enum

    ''' <summary>
    ''' 配置空间中的一条线段 L(r)。
    ''' </summary>
    Public Class AttachmentSegment

        ''' <summary>线段编号 r（0..3）。</summary>
        Public Property Index As Integer

        ''' <summary>该线段对应的贴附方位。</summary>
        Public Property Side As AttachSide

        ''' <summary>
        ''' 线段是否为竖直方向（此时固定的是 X 坐标）。
        ''' </summary>
        Public Property IsVertical As Boolean

        ''' <summary>固定坐标（竖直线段为 x，水平线段为 y）。</summary>
        Public Property Fixed As Double

        ''' <summary>可变坐标的下界。</summary>
        Public Property RangeMin As Double

        ''' <summary>可变坐标的上界。</summary>
        Public Property RangeMax As Double

        ''' <summary>线段长度。</summary>
        Public ReadOnly Property Length As Double
            Get
                Return Math.Max(0, RangeMax - RangeMin)
            End Get
        End Property

        ''' <summary>
        ''' 判断参考点 (x, y) 是否精确落在该线段上（带容差）。
        ''' </summary>
        Public Function Contains(x As Double, y As Double, Optional tolerance As Double = 0.5) As Boolean
            If IsVertical Then
                If Math.Abs(x - Fixed) > tolerance Then
                    Return False
                End If

                Return y >= RangeMin - tolerance AndAlso y <= RangeMax + tolerance
            Else
                If Math.Abs(y - Fixed) > tolerance Then
                    Return False
                End If

                Return x >= RangeMin - tolerance AndAlso x <= RangeMax + tolerance
            End If
        End Function

        ''' <summary>
        ''' 把参考点投影到该线段上（即最近的有效贴合位置）。
        ''' </summary>
        Public Function Snap(x As Double, y As Double) As PointF
            If IsVertical Then
                Return New PointF(CSng(Fixed), CSng(Math.Max(RangeMin, Math.Min(RangeMax, y))))
            Else
                Return New PointF(CSng(Math.Max(RangeMin, Math.Min(RangeMax, x))), CSng(Fixed))
            End If
        End Function

        ''' <summary>
        ''' 到该线段的距离（不重合时表示违规程度）。
        ''' </summary>
        Public Function Distance(x As Double, y As Double) As Double
            Dim snapped As PointF = Snap(x, y)
            Dim dx As Double = x - snapped.X
            Dim dy As Double = y - snapped.Y

            Return Math.Sqrt(dx * dx + dy * dy)
        End Function

        Public Overrides Function ToString() As String
            If IsVertical Then
                Return $"L({Index}) {Side}: x={Fixed:0.##}, y∈[{RangeMin:0.##},{RangeMax:0.##}]"
            Else
                Return $"L({Index}) {Side}: y={Fixed:0.##}, x∈[{RangeMin:0.##},{RangeMax:0.##}]"
            End If
        End Function

    End Class

    ''' <summary>
    ''' 两个矩形之间的贴合配置空间。
    ''' </summary>
    Public Class ConfigSpace

        ''' <summary>作为「锚点」的矩形 bc(i)。</summary>
        Public Property Anchor As Rect

        ''' <summary>被放置的矩形 bc(j) 的宽度。</summary>
        Public Property Width As Double

        ''' <summary>被放置的矩形 bc(j) 的高度。</summary>
        Public Property Height As Double

        ''' <summary>配置空间的 k = 4 条线段。</summary>
        Public Property Segments As AttachmentSegment()

        ''' <summary>
        ''' 由锚点矩形与待放置矩形的尺寸构造配置空间。
        ''' </summary>
        Public Shared Function Create(anchor As Rect, width As Double, height As Double) As ConfigSpace
            Dim w As Double = Math.Max(0, width)
            Dim h As Double = Math.Max(0, height)

            Dim segments As AttachmentSegment() = {
                New AttachmentSegment With {
                    .Index = 0, .Side = AttachSide.Left, .IsVertical = True,
                    .Fixed = anchor.X - w,
                    .RangeMin = anchor.Y - h,
                    .RangeMax = anchor.Q
                },
                New AttachmentSegment With {
                    .Index = 1, .Side = AttachSide.Bottom, .IsVertical = False,
                    .Fixed = anchor.Y - h,
                    .RangeMin = anchor.X - w,
                    .RangeMax = anchor.P
                },
                New AttachmentSegment With {
                    .Index = 2, .Side = AttachSide.Right, .IsVertical = True,
                    .Fixed = anchor.P,
                    .RangeMin = anchor.Y - h,
                    .RangeMax = anchor.Q
                },
                New AttachmentSegment With {
                    .Index = 3, .Side = AttachSide.Top, .IsVertical = False,
                    .Fixed = anchor.Q,
                    .RangeMin = anchor.X - w,
                    .RangeMax = anchor.P
                }
            }

            Return New ConfigSpace With {
                .Anchor = anchor,
                .Width = w,
                .Height = h,
                .Segments = segments
            }
        End Function

        ''' <summary>按方位取线段。</summary>
        Public Function Segment(side As AttachSide) As AttachmentSegment
            For Each hit As AttachmentSegment In Segments
                If hit.Side = side Then
                    Return hit
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' 判断参考点是否落在配置空间的任一线段上（即两个矩形是否恰好贴合）。
        ''' </summary>
        Public Function Contains(x As Double, y As Double, Optional tolerance As Double = 0.5) As Boolean
            For Each segment As AttachmentSegment In Segments
                If segment.Contains(x, y, tolerance) Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' 到配置空间的最近距离：0 表示精确贴合；正值表示既不相邻也不（该方向上的）重叠。
        ''' </summary>
        Public Function Distance(x As Double, y As Double) As Double
            Dim best As Double = Double.MaxValue

            For Each segment As AttachmentSegment In Segments
                best = Math.Min(best, segment.Distance(x, y))
            Next

            Return best
        End Function

        ''' <summary>
        ''' 把参考点吸附到指定方位线段上（论文中的「强制参考点落在其中一条线段上」）。
        ''' </summary>
        Public Function Snap(side As AttachSide, x As Double, y As Double) As PointF
            Dim hit As AttachmentSegment = Segment(side)

            If hit Is Nothing Then
                Return New PointF(CSng(x), CSng(y))
            End If

            Return hit.Snap(x, y)
        End Function

        Public Overrides Function ToString() As String
            Return $"config space of {Anchor} for {Width:0.##}x{Height:0.##}"
        End Function

    End Class

    ''' <summary>
    ''' 矩形贴合相关的几何工具。
    ''' </summary>
    Public Module ConfigSpaceHelper

        ''' <summary>
        ''' 由两个矩形当前的中心相对位置推断贴附方位。
        ''' </summary>
        Public Function DetermineSide(anchor As Rect, other As Rect) As AttachSide
            Dim dx As Double = other.Center.X - anchor.Center.X
            Dim dy As Double = other.Center.Y - anchor.Center.Y

            If Math.Abs(dx) >= Math.Abs(dy) Then
                Return If(dx >= 0, AttachSide.Right, AttachSide.Left)
            Else
                Return If(dy >= 0, AttachSide.Top, AttachSide.Bottom)
            End If
        End Function

    End Module

End Namespace

#End Region
