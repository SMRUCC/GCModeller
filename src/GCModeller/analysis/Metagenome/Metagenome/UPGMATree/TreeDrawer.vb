Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Namespace UPGMATree

    ''' <summary>
    ''' 用于缓存每个节点的极坐标布局信息
    ''' </summary>
    Public Class NodeLayout
        Public Property Node As Taxa
        Public Property Radius As Double     ' 极径（到根的距离）
        Public Property Angle As Double      ' 极角（弧度，0°在右侧，逆时针增加）
        Public Property X As Double          ' 屏幕坐标 X
        Public Property Y As Double          ' 屏幕坐标 Y
    End Class

    Public Module TreeDrawer

        ''' <summary>
        ''' 统计树中的叶子节点数量
        ''' </summary>
        Private Function CountLeaves(node As Taxa) As Integer
            If node.Childs.IsNullOrEmpty Then
                Return 1
            Else
                Dim sum As Integer = 0
                For Each child As Taxa In From t In node.Childs.Values Select DirectCast(t, Taxa)
                    sum += CountLeaves(child)
                Next
                Return sum
            End If
        End Function

        ''' <summary>
        ''' 递归计算每个节点的极坐标布局信息
        ''' </summary>
        Private Sub ComputeLayout(node As Taxa,
                               ByRef leafIndex As Integer,
                               leafCount As Integer,
                               radius As Double,
                               ByRef layouts As Dictionary(Of Taxa, NodeLayout),
                               startAngle As Double,
                               sweepAngle As Double)

            If layouts Is Nothing Then layouts = New Dictionary(Of Taxa, NodeLayout)()

            ' 记录当前节点
            Dim lay As New NodeLayout With {
                .Node = node,
                .Radius = radius,
                .Angle = 0.0 ' 暂时为0，后面根据情况计算
            }
            layouts(node) = lay

            If node.Childs.IsNullOrEmpty Then
                ' 叶子节点：均匀分布在上半圆
                If leafCount > 1 Then
                    lay.Angle = startAngle + (leafIndex / (leafCount - 1)) * sweepAngle
                Else
                    lay.Angle = startAngle + sweepAngle / 2.0
                End If
                leafIndex += 1
            Else
                ' 内部节点：角度为子节点角度的平均
                Dim sumAngle As Double = 0.0
                Dim childCount As Integer = 0

                For Each child As Taxa In From t In node.Childs.Values Select DirectCast(t, Taxa)
                    Dim childDist = child.Data.distance  ' 注意：这是子节点到当前节点的距离
                    ComputeLayout(child, leafIndex, leafCount, radius + childDist, layouts, startAngle, sweepAngle)

                    Dim childLayout = layouts(child)
                    sumAngle += childLayout.Angle
                    childCount += 1
                Next

                If childCount > 0 Then
                    lay.Angle = sumAngle / childCount
                Else
                    lay.Angle = startAngle + sweepAngle / 2.0
                End If
            End If
        End Sub

        Private _classColorMap As New Dictionary(Of String, Color)

        ''' <summary>
        ''' 根据 taxonomy.class 获取颜色，相同 class 使用相同颜色
        ''' </summary>
        Private Function GetClassColor(node As Taxa) As Color
            If node Is Nothing OrElse node.taxonomy Is Nothing Then
                Return Color.Gray
            End If

            Dim cls = node.taxonomy.class
            If String.IsNullOrEmpty(cls) Then
                Return Color.Gray
            End If

            ' 已有缓存
            If _classColorMap.ContainsKey(cls) Then
                Return _classColorMap(cls)
            End If

            ' 根据类名字符串哈希生成一个固定颜色
            Dim hash = cls.GetHashCode()
            Dim r As Integer = CInt((hash And &HFF0000) >> 16)
            Dim g As Integer = CInt((hash And &HFF00) >> 8)
            Dim b As Integer = CInt(hash And &HFF)

            ' 避免太暗或太亮
            r = CInt((r + 64) Mod 256)
            g = CInt((g + 64) Mod 256)
            b = CInt((b + 64) Mod 256)

            Dim c = Color.FromArgb(r, g, b)
            _classColorMap(cls) = c
            Return c
        End Function

        ''' <summary>
        ''' 极坐标 → 屏幕坐标，圆心在 (cx, cy)
        ''' </summary>
        Private Sub PolarToCartesian(lay As NodeLayout, cx As Double, cy As Double)
            ' 注意：GDI+ 中 Y 轴向下，所以 Y 用减号
            lay.X = cx + lay.Radius * Math.Cos(lay.Angle)
            lay.Y = cy - lay.Radius * Math.Sin(lay.Angle)
        End Sub

        ''' <summary>
        ''' 在指定 Graphics 上绘制上半圆形进化树
        ''' </summary>
        ''' <param name="g">绘图对象</param>
        ''' <param name="tree">Taxa 进化树</param>
        ''' <param name="size">绘图区域大小（宽度即为上半圆直径）</param>
        Public Sub DrawCircularTree(g As IGraphics, tree As Taxa, size As Size, Optional offset As PointF = Nothing)
            If tree Is Nothing Then Return

            ' 1. 计算布局信息
            Dim leafCount = CountLeaves(tree)
            Dim layouts As New Dictionary(Of Taxa, NodeLayout)

            ' 这里 startAngle=0, sweepAngle=PI（上半圆）
            Dim leafIndex As Integer = 0
            ComputeLayout(tree, leafIndex, leafCount, 0.0, layouts, 0.0, Math.PI)

            ' 2. 计算圆心与缩放比例
            Dim cx As Double = size.Width / 2.0
            Dim cy As Double = size.Height  ' 圆心在底部中央（上半圆）

            ' 找出最大半径，用于缩放
            Dim maxRadius = If(layouts.Count > 0, layouts.Values.Max(Function(l) l.Radius), 0.0)
            Dim scale As Double = 1.0
            If maxRadius > 0.0 Then
                scale = Math.Min(size.Width, size.Height) * 0.45 / maxRadius ' 留一些边距
            End If

            ' 3. 先对所有节点做坐标转换
            For Each lay In layouts.Values
                lay.Radius *= scale
                PolarToCartesian(lay, cx, cy)
            Next

            ' 4. 绘制树边（递归）
            DrawTreeEdges(g, tree, layouts, cx, cy)

            ' 5. 绘制节点（可选：只画叶子，或画所有节点）
            DrawTreeNodes(g, tree, layouts)
        End Sub

        Private Sub DrawTreeEdges(g As IGraphics, node As Taxa, layouts As Dictionary(Of Taxa, NodeLayout), cx As Double, cy As Double)
            If node.Childs.IsNullOrEmpty Then Return

            Dim parentLayout = layouts(node)

            For Each child As Taxa In From t In node.Childs.Values Select DirectCast(t, Taxa)
                Dim childLayout = layouts(child)

                ' 获取子节点的 taxonomy.class 对应颜色
                Dim edgeColor = GetClassColor(child)

                Using pen As New Pen(edgeColor, 2.0F)
                    ' 从父节点到子节点画线
                    g.DrawLine(pen,
                           CSng(parentLayout.X), CSng(parentLayout.Y),
                           CSng(childLayout.X), CSng(childLayout.Y))
                End Using

                ' 递归绘制子树
                DrawTreeEdges(g, child, layouts, cx, cy)
            Next
        End Sub

        Private Sub DrawTreeNodes(g As IGraphics, node As Taxa, layouts As Dictionary(Of Taxa, NodeLayout))
            If Not layouts.ContainsKey(node) Then Return

            Dim lay = layouts(node)
            Dim x = CSng(lay.X)
            Dim y = CSng(lay.Y)

            ' 这里简单画小圆点；可以改成画标签、OTU 名等
            Dim r = 3.0F

            Dim nodeColor = GetClassColor(node)
            Using brush As New SolidBrush(nodeColor)
                g.FillEllipse(brush, x - r, y - r, 2 * r, 2 * r)
            End Using

            ' 递归绘制子节点
            If Not node.Childs.IsNullOrEmpty Then
                For Each child As Taxa In From t In node.Childs.Values Select DirectCast(t, Taxa)
                    DrawTreeNodes(g, child, layouts)
                Next
            End If
        End Sub

    End Module
End Namespace