Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging

Namespace Evolview.Drawing

    ''' <summary>
    ''' 树绘制所需的通用工具：节点遍历、颜色解析、极坐标换算以及文本尺寸缓存。
    ''' </summary>
    Public Module DrawingHelper

        ''' <summary>
        ''' 以先序遍历返回根节点下的所有节点（含根节点自身）。
        ''' </summary>
        Public Function EnumerateNodes(root As PhyloNode) As List(Of PhyloNode)
            Dim list As New List(Of PhyloNode)
            Call __travel(root, list)

            Return list
        End Function

        Private Sub __travel(node As PhyloNode, list As List(Of PhyloNode))
            If node Is Nothing Then
                Return
            End If

            list.Add(node)

            For Each child As PhyloNode In node.Descendents
                Call __travel(child, list)
            Next
        End Sub

        ''' <summary>
        ''' 返回树中的所有叶节点。
        ''' </summary>
        Public Function EnumerateLeaves(root As PhyloNode) As List(Of PhyloNode)
            Dim leaves As New List(Of PhyloNode)

            For Each node As PhyloNode In EnumerateNodes(root)
                If node.Descendents.Count = 0 Then
                    leaves.Add(node)
                End If
            Next

            Return leaves
        End Function

        ''' <summary>
        ''' 依据父节点链累计分支长度，得到节点到根的深度（不依赖 PhyloTree 的预计算值）。
        ''' </summary>
        Public Function NodeDepth(node As PhyloNode) As Double
            Dim depth As Double = 0
            Dim current As PhyloNode = node

            While current IsNot Nothing AndAlso Not current.IsRoot
                depth += Math.Max(0, current.BranchLength)
                current = current.Parent
            End While

            Return depth
        End Function

        ''' <summary>
        ''' 创建字体；字体族不可用时回退到系统默认字体。
        ''' </summary>
        Public Function CreateFont(name As String, size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
            Dim family As String = If(String.IsNullOrEmpty(name), FontFace.SegoeUI, name)

            Try
                Return New Font(family, size, style)
            Catch
                Return New Font(FontFace.SegoeUI, size, style)
            End Try
        End Function

        ''' <summary>
        ''' 把 CSS/HTML/OLE 颜色表达式解析为 <see cref="Color"/>；解析失败时返回回退色。
        ''' </summary>
        Public Function ResolveColor(expression As String, fallback As Color) As Color
            If String.IsNullOrEmpty(expression) Then
                Return fallback
            End If

            Dim success As Boolean = False
            Dim color As Color = expression.TranslateColor(throwEx:=False, success:=success)

            If success Then
                Return color
            Else
                Return fallback
            End If
        End Function

        ''' <summary>
        ''' 把角度（度）归一化到 [0, 360)。
        ''' </summary>
        Public Function NormalizeDegrees(angle As Single) As Single
            Dim value As Single = angle Mod 360.0F

            If value < 0 Then
                value += 360.0F
            End If

            Return value
        End Function

        ''' <summary>
        ''' 极坐标转笛卡尔坐标（屏幕坐标系，Y 轴向下）。
        ''' </summary>
        Public Function PolarToCartesian(center As PointF, radius As Single, theta As Single) As PointF
            Return New PointF(
                center.X + radius * Math.Cos(theta),
                center.Y - radius * Math.Sin(theta))
        End Function

        ''' <summary>
        ''' 构造环形扇（annular sector）多边形顶点，用于绘制叶背景色块。
        ''' </summary>
        Public Function BuildAnnularSector(center As PointF,
                                           innerRadius As Single,
                                           outerRadius As Single,
                                           thetaFrom As Single,
                                           thetaTo As Single,
                                           Optional segments As Integer = 6) As PointF()

            If segments < 1 Then
                segments = 1
            End If

            Dim points As New List(Of PointF)((segments + 1) * 2)

            For k As Integer = 0 To segments
                Dim t As Single = thetaFrom + (thetaTo - thetaFrom) * k / segments
                points.Add(PolarToCartesian(center, outerRadius, t))
            Next

            For k As Integer = segments To 0 Step -1
                Dim t As Single = thetaFrom + (thetaTo - thetaFrom) * k / segments
                points.Add(PolarToCartesian(center, innerRadius, t))
            Next

            Return points.ToArray
        End Function

        ''' <summary>
        ''' 合并两个矩形（其中任意一个可以为空）。
        ''' </summary>
        Public Function Union(a As RectangleF?, b As RectangleF) As RectangleF
            If Not a.HasValue Then
                Return b
            End If

            Dim left As Single = Math.Min(a.Value.Left, b.Left)
            Dim top As Single = Math.Min(a.Value.Top, b.Top)
            Dim right As Single = Math.Max(a.Value.Right, b.Right)
            Dim bottom As Single = Math.Max(a.Value.Bottom, b.Bottom)

            Return RectangleF.FromLTRB(left, top, right, bottom)
        End Function
    End Module

    ''' <summary>
    ''' 文本尺寸测量缓存：同一 (文本, 字体) 组合只测量一次，避免大树上逐节点重复测量。
    ''' </summary>
    Public Class TextMeasureCache

        Private ReadOnly _graphics As IGraphics
        Private ReadOnly _cache As New Dictionary(Of String, SizeF)

        Public Sub New(graphics As IGraphics)
            Me._graphics = graphics
        End Sub

        ''' <summary>
        ''' 测量文本尺寸（失败或空文本时返回零尺寸）。
        ''' </summary>
        Public Function Measure(text As String, font As Font) As SizeF
            If String.IsNullOrEmpty(text) OrElse font Is Nothing Then
                Return SizeF.Empty
            End If

            Dim key As String = $"{font.Name}|{font.Size}|{CInt(font.Style)}|{text}"
            Dim size As SizeF = Nothing

            If _cache.TryGetValue(key, size) Then
                Return size
            End If

            Try
                size = _graphics.MeasureString(text, font)
            Catch
                ' 某些 driver 在极少数情况下测量失败，给出一个保守估计避免整体绘制中断
                size = New SizeF(text.Length * font.Size * 0.6F, font.Size * 1.3F)
            End Try

            _cache(key) = size

            Return size
        End Function
    End Class

End Namespace
