Imports System.Drawing

Namespace Evolview.Drawing

    ''' <summary>
    ''' 进化树绘制的配置项：布局样式、画布与边距、字体、颜色与线宽、显示开关、
    ''' 圆形布局的起止角参数以及缩放策略。
    ''' </summary>
    ''' <remarks>
    ''' 该类只描述「画什么、怎么画」的参数，不参与几何计算与绘制本身；
    ''' 与之配套的几何计算见 <see cref="TreeLayoutEngine"/>，绘制见 <see cref="TreeRenderer"/>。
    ''' </remarks>
    Public Class TreeDrawingOptions

        ''' <summary>
        ''' 布局样式（对应 <see cref="TreePlotMode"/> 的 8 种取值）
        ''' </summary>
        Public Property Mode As TreePlotMode = TreePlotMode.RECT_PHYLOGRAM

        ''' <summary>
        ''' 输出画布尺寸（像素）
        ''' </summary>
        Public Property CanvasSize As Size = New Size(1200, 900)

        ''' <summary>
        ''' 画布四周的留白
        ''' </summary>
        Public Property Padding As System.Drawing.Padding = New System.Drawing.Padding(60)

        ''' <summary>
        ''' 画布 DPI
        ''' </summary>
        Public Property Dpi As Integer = 100

        ''' <summary>
        ''' 背景色；设置为 <see cref="Color.Transparent"/> 可输出透明背景
        ''' </summary>
        Public Property Background As Color = Color.White

#Region "字体"

        ''' <summary>叶标签字体；为空时使用默认字体</summary>
        Public Property LeafFont As Font = Nothing
        ''' <summary>bootstrap 支持度字体；为空时使用默认字体</summary>
        Public Property BootstrapFont As Font = Nothing
        ''' <summary>分支长度字体；为空时使用默认字体</summary>
        Public Property BranchLengthFont As Font = Nothing
        ''' <summary>标题字体；为空时使用默认字体</summary>
        Public Property TitleFont As Font = Nothing

#End Region

#Region "颜色与线宽"

        ''' <summary>默认分支颜色</summary>
        Public Property BranchColor As Color = Color.Black
        ''' <summary>默认叶标签颜色</summary>
        Public Property LeafColor As Color = Color.Black
        ''' <summary>节点标记颜色</summary>
        Public Property NodeColor As Color = Color.Black
        ''' <summary>标题文字颜色</summary>
        Public Property TitleColor As Color = Color.Black

        ''' <summary>
        ''' 是否使用 <see cref="PhyloNode"/> 上通过 <see cref="TreeDecoType"/> 设置的颜色集
        ''' （分支色 / 叶文字色 / 叶背景色）。为 False 时全部使用本配置的默认色。
        ''' </summary>
        Public Property UseNodeColors As Boolean = True

        ''' <summary>
        ''' 颜色集 ID；对应 <c>PhyloNode.getBranchColorByColorsetID(id)</c> 等 API 的 key
        ''' </summary>
        Public Property ActiveColorSetID As String = Nothing

        ''' <summary>分支线宽</summary>
        Public Property BranchWidth As Single = 1.0F
        ''' <summary>节点标记半径（像素）</summary>
        Public Property NodeRadius As Single = 1.5F
        ''' <summary>标签与树骨架之间的间距（像素）</summary>
        Public Property LabelGap As Single = 6.0F

#End Region

#Region "显示开关"

        Public Property ShowLeafLabels As Boolean = True
        Public Property ShowBootstrap As Boolean = True
        Public Property ShowBranchLength As Boolean = False
        Public Property ShowNodeMarkers As Boolean = True
        ''' <summary>是否把所有叶标签对齐到同一列（仅矩形/斜线布局有效）</summary>
        Public Property AlignLeafLabels As Boolean = False
        Public Property ShowTitle As Boolean = False
        Public Property Title As String = Nothing
        Public Property ShowScaleBar As Boolean = False
        ''' <summary>比例尺长度（分支长度单位）；&lt;= 0 表示自动选取</summary>
        Public Property ScaleBarLength As Double = 0

#End Region

#Region "圆形/辐射布局参数"

        ''' <summary>角度跨度（度）；&lt;= 0 时按布局样式取默认值（圆形 350，辐射 360）</summary>
        Public Property AngleSpan As Single = 0
        ''' <summary>起始角度（度）</summary>
        Public Property AngleStart As Single = 0
        ''' <summary>是否按顺时针方向排布</summary>
        Public Property Clockwise As Boolean = False

#End Region

        ''' <summary>
        ''' 显式的单位像素（矩形布局为「每水平单位 X 像素 / 每垂直层 Y 像素」，
        ''' 圆形布局取两者的平均值作为「每半径单位像素」）。
        ''' 为空时按画布大小自适应缩放。
        ''' </summary>
        Public Property UnitSize As SizeF? = Nothing

        ''' <summary>
        ''' 按布局样式取实际使用的角度跨度。
        ''' </summary>
        Public Function EffectiveAngleSpan() As Single
            If AngleSpan > 0 Then
                Return AngleSpan
            End If

            If Mode = TreePlotMode.RADIAL_CLADOGRAM Then
                ' 辐射状铺满整圆
                Return 360.0F
            Else
                ' 圆形布局留出缺口，避免首尾叶节点重叠
                Return 350.0F
            End If
        End Function

        ''' <summary>
        ''' 返回指定样式的默认配置。
        ''' </summary>
        Public Shared Function Defaults(Optional mode As TreePlotMode = TreePlotMode.RECT_PHYLOGRAM) As TreeDrawingOptions
            Return New TreeDrawingOptions With {
                .Mode = mode,
                .ShowLeafLabels = True,
                .ShowNodeMarkers = True,
                .ShowBootstrap = True
            }
        End Function

        ''' <summary>
        ''' 复制当前配置。
        ''' </summary>
        Public Function Clone() As TreeDrawingOptions
            Return New TreeDrawingOptions With {
                .Mode = Mode,
                .CanvasSize = CanvasSize,
                .Padding = Padding,
                .Dpi = Dpi,
                .Background = Background,
                .LeafFont = LeafFont,
                .BootstrapFont = BootstrapFont,
                .BranchLengthFont = BranchLengthFont,
                .TitleFont = TitleFont,
                .BranchColor = BranchColor,
                .LeafColor = LeafColor,
                .NodeColor = NodeColor,
                .TitleColor = TitleColor,
                .UseNodeColors = UseNodeColors,
                .ActiveColorSetID = ActiveColorSetID,
                .BranchWidth = BranchWidth,
                .NodeRadius = NodeRadius,
                .LabelGap = LabelGap,
                .ShowLeafLabels = ShowLeafLabels,
                .ShowBootstrap = ShowBootstrap,
                .ShowBranchLength = ShowBranchLength,
                .ShowNodeMarkers = ShowNodeMarkers,
                .AlignLeafLabels = AlignLeafLabels,
                .ShowTitle = ShowTitle,
                .Title = Title,
                .ShowScaleBar = ShowScaleBar,
                .ScaleBarLength = ScaleBarLength,
                .AngleSpan = AngleSpan,
                .AngleStart = AngleStart,
                .Clockwise = Clockwise,
                .UnitSize = UnitSize
            }
        End Function
    End Class

End Namespace
