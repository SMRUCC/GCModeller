Imports System.Drawing

Namespace Evolview.Drawing

    ''' <summary>
    ''' 边（分支）的绘制形状
    ''' </summary>
    Public Enum TreeEdgeStyle As Byte
        ''' <summary>直角折线（矩形分支图/扇形图）</summary>
        Elbow = 0
        ''' <summary>单段直线（斜线图 / 辐射状）</summary>
        Straight = 1
        ''' <summary>父半径圆弧 + 径向直线（圆形图）</summary>
        Arc = 2
    End Enum

    ''' <summary>
    ''' 单个树节点的布局结果（全部为画布像素坐标）
    ''' </summary>
    Public Class NodeLayout

        Public Property Node As PhyloNode

        ''' <summary>节点自身的坐标</summary>
        Public Property Position As PointF

        ''' <summary>父节点的坐标（用于绘制边与分支长度标签）</summary>
        Public Property ParentPosition As PointF

        ''' <summary>叶标签起始坐标</summary>
        Public Property LabelPosition As PointF

        ''' <summary>叶标签旋转角度（度，GDI+ 顺时针为正）</summary>
        Public Property LabelAngle As Single = 0.0F

        ''' <summary>叶标签是否需要右对齐（圆形布局左半圆）</summary>
        Public Property LabelRightAligned As Boolean = False

        ''' <summary>bootstrap 支持度文本位置</summary>
        Public Property BootstrapPosition As PointF

        ''' <summary>bootstrap 文本旋转角度</summary>
        Public Property BootstrapAngle As Single = 0.0F

        ''' <summary>分支长度文本位置</summary>
        Public Property BranchLengthPosition As PointF

        ''' <summary>分支长度文本旋转角度</summary>
        Public Property BranchLengthAngle As Single = 0.0F

        ''' <summary>叶背景色块（矩形布局）</summary>
        Public Property BackgroundRect As RectangleF? = Nothing

        ''' <summary>叶背景色块（圆形布局的环形扇多边形顶点）</summary>
        Public Property BackgroundFan As PointF() = Nothing

        ''' <summary>节点半径（像素；圆形/辐射布局使用）</summary>
        Public Property Radius As Single = 0.0F

        ''' <summary>父节点半径（像素；圆形/辐射布局使用）</summary>
        Public Property ParentRadius As Single = 0.0F

        ''' <summary>节点极角（弧度；圆形/辐射布局使用）</summary>
        Public Property Theta As Single = 0.0F

        ''' <summary>父节点极角（弧度；圆形/辐射布局使用）</summary>
        Public Property ParentTheta As Single = 0.0F

    End Class

    ''' <summary>
    ''' 整棵树的布局结果：节点布局映射表 + 内容包围盒 + 布局元信息。
    ''' </summary>
    ''' <remarks>
    ''' 该结果与绘制完全解耦，既可用于渲染，也可用于单元测试与内容度量。
    ''' </remarks>
    Public Class TreeLayoutResult

        Public Property Mode As TreePlotMode
        Public Property EdgeStyle As TreeEdgeStyle

        ''' <summary>节点 → 布局</summary>
        Public Property Nodes As New Dictionary(Of PhyloNode, NodeLayout)

        ''' <summary>内容包围盒（画布像素坐标）</summary>
        Public Property Bounds As RectangleF

        ''' <summary>根节点坐标</summary>
        Public Property RootPosition As PointF

        ''' <summary>圆形/辐射布局的圆心</summary>
        Public Property Center As PointF

        ''' <summary>单位水平坐标对应的像素（矩形/斜线布局）</summary>
        Public Property PixelsPerUnitX As Single

        ''' <summary>每个垂直层对应的像素</summary>
        Public Property PixelsPerUnitY As Single

        ''' <summary>每单位半径对应的像素（圆形/辐射布局）</summary>
        Public Property PixelsPerRadius As Single

        ''' <summary>实际使用的分支长度比例尺（像素/单位分支长度），用于比例尺绘制</summary>
        Public Property BranchLengthScale As Single

        Public Function GetLayout(node As PhyloNode) As NodeLayout
            Dim layout As NodeLayout = Nothing

            If node IsNot Nothing AndAlso Nodes.TryGetValue(node, layout) Then
                Return layout
            End If

            Return Nothing
        End Function
    End Class

End Namespace
