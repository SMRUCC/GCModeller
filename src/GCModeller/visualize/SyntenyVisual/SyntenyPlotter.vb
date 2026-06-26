' ============================================================================
'  SyntenyPlotter.vb
'  泛基因组共线性可视化绘图模块
'  基于 GDI+ 在 Windows 系统上绘制高质量基因组共线性图，输出 PNG 文件
'
'  功能特性：
'    1. 支持直线 / 贝塞尔曲线两种连接线样式，通过主题对象一键切换
'    2. 内置 6 套配色方案（Nature / Science / Classic / Vibrant / Pastel / Grayscale）
'    3. 支持自定义图像尺寸、DPI、字体、透明度等参数
'    4. 自动布局多基因组、多染色体，按比例缩放
'    5. 输出适合论文插图的高分辨率 PNG（默认 300 DPI）
'
'  作者：Qingyan Agent
' ============================================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Collections.Generic
Imports System.Linq

Namespace PanGenomeSynteny

    ' ========================================================================
    '  基因数据结构
    ' ========================================================================

    ''' <summary>
    ''' 泛基因组中的基因对象。
    ''' orthology_family 字段用于在不同基因组间建立共线性桥梁。
    ''' </summary>
    Public Class Gene
        Public Property geneId As String
        Public Property orthology_family As String
        Public Property start As Integer
        Public Property [end] As Integer
        Public Property chromosome_id As String
        Public Property strand As Char   ' "+" 或 "-"

        Public Sub New()
        End Sub

        Public Sub New(geneId As String,
                       orthology_family As String,
                       start As Integer,
                       [end] As Integer,
                       chromosome_id As String,
                       strand As Char)
            Me.geneId = geneId
            Me.orthology_family = orthology_family
            Me.start = start
            Me.end = [end]
            Me.chromosome_id = chromosome_id
            Me.strand = strand
        End Sub
    End Class


    ' ========================================================================
    '  连接线样式枚举
    ' ========================================================================

    ''' <summary>
    ''' 共线性连接线的绘制样式，可通过主题对象切换。
    ''' </summary>
    Public Enum ConnectionStyle
        ''' <summary>直线连接</summary>
        StraightLine
        ''' <summary>贝塞尔曲线连接（S 形平滑过渡）</summary>
        BezierCurve
    End Enum


    ' ========================================================================
    '  颜色调色板
    ' ========================================================================

    ''' <summary>
    ''' 颜色调色板，定义图中所有元素的颜色。
    ''' 内置多套预设方案，也可完全自定义。
    ''' </summary>
    Public Class ColorPalette

        ''' <summary>调色板名称</summary>
        Public Property Name As String

        ''' <summary>画布背景色</summary>
        Public Property Background As Color

        ''' <summary>染色体填充色</summary>
        Public Property ChromosomeFill As Color

        ''' <summary>染色体边框色</summary>
        Public Property ChromosomeBorder As Color

        ''' <summary>正向链基因默认色（无家族时使用）</summary>
        Public Property GeneForward As Color

        ''' <summary>反向链基因默认色（无家族时使用）</summary>
        Public Property GeneReverse As Color

        ''' <summary>主文字颜色（标题、基因组名）</summary>
        Public Property TextColor As Color

        ''' <summary>次级文字颜色（染色体名、刻度）</summary>
        Public Property SubTextColor As Color

        ''' <summary>网格线颜色</summary>
        Public Property GridColor As Color

        ''' <summary>连接线配色列表（按直系同源家族索引循环取色）</summary>
        Public Property ConnectionColors As List(Of Color)


        ' --------------------------------------------------------------------
        '  预设调色板
        ' --------------------------------------------------------------------

        ''' <summary>Nature 期刊风格：清爽的蓝红主色调</summary>
        Public Shared Function Nature() As ColorPalette
            Dim p As New ColorPalette()
            p.Name = "Nature"
            p.Background = Color.White
            p.ChromosomeFill = Color.FromArgb(235, 235, 235)
            p.ChromosomeBorder = Color.FromArgb(120, 120, 120)
            p.GeneForward = Color.FromArgb(31, 119, 180)
            p.GeneReverse = Color.FromArgb(214, 39, 40)
            p.TextColor = Color.FromArgb(30, 30, 30)
            p.SubTextColor = Color.FromArgb(110, 110, 110)
            p.GridColor = Color.FromArgb(245, 245, 245)
            p.ConnectionColors = New List(Of Color) From {
                Color.FromArgb(31, 119, 180),
                Color.FromArgb(255, 127, 14),
                Color.FromArgb(44, 160, 44),
                Color.FromArgb(214, 39, 40),
                Color.FromArgb(148, 103, 189),
                Color.FromArgb(140, 86, 75),
                Color.FromArgb(227, 119, 194),
                Color.FromArgb(127, 127, 127),
                Color.FromArgb(188, 189, 34),
                Color.FromArgb(23, 190, 207)
            }
            Return p
        End Function

        ''' <summary>Science 期刊风格：暖色调</summary>
        Public Shared Function Science() As ColorPalette
            Dim p As New ColorPalette()
            p.Name = "Science"
            p.Background = Color.FromArgb(252, 252, 248)
            p.ChromosomeFill = Color.FromArgb(230, 225, 215)
            p.ChromosomeBorder = Color.FromArgb(140, 130, 110)
            p.GeneForward = Color.FromArgb(196, 90, 26)
            p.GeneReverse = Color.FromArgb(70, 130, 180)
            p.TextColor = Color.FromArgb(50, 40, 30)
            p.SubTextColor = Color.FromArgb(120, 110, 100)
            p.GridColor = Color.FromArgb(240, 235, 225)
            p.ConnectionColors = New List(Of Color) From {
                Color.FromArgb(196, 90, 26),
                Color.FromArgb(80, 130, 180),
                Color.FromArgb(120, 160, 80),
                Color.FromArgb(200, 60, 60),
                Color.FromArgb(150, 100, 180),
                Color.FromArgb(220, 160, 60),
                Color.FromArgb(90, 170, 160),
                Color.FromArgb(180, 120, 100),
                Color.FromArgb(100, 140, 200),
                Color.FromArgb(160, 180, 60)
            }
            Return p
        End Function

        ''' <summary>经典风格：黑灰底配高饱和强调色</summary>
        Public Shared Function Classic() As ColorPalette
            Dim p As New ColorPalette()
            p.Name = "Classic"
            p.Background = Color.White
            p.ChromosomeFill = Color.FromArgb(220, 220, 220)
            p.ChromosomeBorder = Color.FromArgb(80, 80, 80)
            p.GeneForward = Color.FromArgb(0, 0, 180)
            p.GeneReverse = Color.FromArgb(180, 0, 0)
            p.TextColor = Color.Black
            p.SubTextColor = Color.FromArgb(90, 90, 90)
            p.GridColor = Color.FromArgb(240, 240, 240)
            p.ConnectionColors = New List(Of Color) From {
                Color.FromArgb(0, 114, 178),
                Color.FromArgb(213, 94, 0),
                Color.FromArgb(0, 158, 115),
                Color.FromArgb(204, 121, 167),
                Color.FromArgb(86, 180, 233),
                Color.FromArgb(230, 159, 0),
                Color.FromArgb(240, 228, 66),
                Color.FromArgb(0, 0, 0),
                Color.FromArgb(155, 155, 155),
                Color.FromArgb(180, 80, 80)
            }
            Return p
        End Function

        ''' <summary>鲜艳风格：高饱和度配色，适合演示</summary>
        Public Shared Function Vibrant() As ColorPalette
            Dim p As New ColorPalette()
            p.Name = "Vibrant"
            p.Background = Color.FromArgb(250, 250, 255)
            p.ChromosomeFill = Color.FromArgb(225, 225, 235)
            p.ChromosomeBorder = Color.FromArgb(100, 100, 120)
            p.GeneForward = Color.FromArgb(0, 150, 255)
            p.GeneReverse = Color.FromArgb(255, 80, 80)
            p.TextColor = Color.FromArgb(30, 30, 50)
            p.SubTextColor = Color.FromArgb(100, 100, 120)
            p.GridColor = Color.FromArgb(238, 238, 245)
            p.ConnectionColors = New List(Of Color) From {
                Color.FromArgb(255, 99, 132),
                Color.FromArgb(54, 162, 235),
                Color.FromArgb(255, 206, 86),
                Color.FromArgb(75, 192, 192),
                Color.FromArgb(153, 102, 255),
                Color.FromArgb(255, 159, 64),
                Color.FromArgb(100, 200, 100),
                Color.FromArgb(200, 100, 200),
                Color.FromArgb(64, 224, 208),
                Color.FromArgb(255, 140, 0)
            }
            Return p
        End Function

        ''' <summary>柔和风格：低饱和度粉彩色，适合大面积连接线</summary>
        Public Shared Function Pastel() As ColorPalette
            Dim p As New ColorPalette()
            p.Name = "Pastel"
            p.Background = Color.FromArgb(253, 253, 250)
            p.ChromosomeFill = Color.FromArgb(230, 228, 225)
            p.ChromosomeBorder = Color.FromArgb(160, 158, 155)
            p.GeneForward = Color.FromArgb(150, 180, 210)
            p.GeneReverse = Color.FromArgb(210, 160, 160)
            p.TextColor = Color.FromArgb(70, 70, 70)
            p.SubTextColor = Color.FromArgb(130, 130, 130)
            p.GridColor = Color.FromArgb(242, 240, 237)
            p.ConnectionColors = New List(Of Color) From {
                Color.FromArgb(174, 199, 232),
                Color.FromArgb(255, 187, 120),
                Color.FromArgb(152, 223, 138),
                Color.FromArgb(255, 152, 150),
                Color.FromArgb(197, 176, 213),
                Color.FromArgb(196, 156, 148),
                Color.FromArgb(247, 182, 210),
                Color.FromArgb(199, 199, 199),
                Color.FromArgb(219, 219, 141),
                Color.FromArgb(158, 218, 229)
            }
            Return p
        End Function

        ''' <summary>灰度风格：纯黑白灰，适合纸质印刷</summary>
        Public Shared Function Grayscale() As ColorPalette
            Dim p As New ColorPalette()
            p.Name = "Grayscale"
            p.Background = Color.White
            p.ChromosomeFill = Color.FromArgb(210, 210, 210)
            p.ChromosomeBorder = Color.FromArgb(60, 60, 60)
            p.GeneForward = Color.FromArgb(60, 60, 60)
            p.GeneReverse = Color.FromArgb(160, 160, 160)
            p.TextColor = Color.Black
            p.SubTextColor = Color.FromArgb(100, 100, 100)
            p.GridColor = Color.FromArgb(240, 240, 240)
            p.ConnectionColors = New List(Of Color) From {
                Color.FromArgb(20, 20, 20),
                Color.FromArgb(60, 60, 60),
                Color.FromArgb(100, 100, 100),
                Color.FromArgb(140, 140, 140),
                Color.FromArgb(180, 180, 180),
                Color.FromArgb(40, 40, 40),
                Color.FromArgb(80, 80, 80),
                Color.FromArgb(120, 120, 120),
                Color.FromArgb(160, 160, 160),
                Color.FromArgb(200, 200, 200)
            }
            Return p
        End Function


        ' --------------------------------------------------------------------
        '  辅助方法
        ' --------------------------------------------------------------------

        ''' <summary>按索引循环获取连接线颜色</summary>
        Public Function GetConnectionColor(index As Integer) As Color
            If ConnectionColors Is Nothing OrElse ConnectionColors.Count = 0 Then
                Return Color.Gray
            End If
            Return ConnectionColors(index Mod ConnectionColors.Count)
        End Function

        ''' <summary>返回所有预设调色板名称</summary>
        Public Shared Function GetPresetNames() As String()
            Return {"Nature", "Science", "Classic", "Vibrant", "Pastel", "Grayscale"}
        End Function

        ''' <summary>按名称获取预设调色板</summary>
        Public Shared Function GetPreset(name As String) As ColorPalette
            Select Case name
                Case "Nature" : Return Nature()
                Case "Science" : Return Science()
                Case "Classic" : Return Classic()
                Case "Vibrant" : Return Vibrant()
                Case "Pastel" : Return Pastel()
                Case "Grayscale" : Return Grayscale()
                Case Else : Return Nature()
            End Select
        End Function
    End Class


    ' ========================================================================
    '  绘图主题
    ' ========================================================================

    ''' <summary>
    ''' 绘图主题对象，集中管理所有可视化参数。
    ''' 通过修改此对象可实现连接线样式切换、配色方案切换等。
    ''' </summary>
    Public Class SyntenyTheme

        ' --- 颜色与样式 ---
        ''' <summary>调色板对象</summary>
        Public Property Palette As ColorPalette = ColorPalette.Nature()

        ''' <summary>连接线样式（直线 / 贝塞尔曲线）</summary>
        Public Property ConnectionStyle As ConnectionStyle = ConnectionStyle.BezierCurve


        ' --- 图像尺寸 ---
        ''' <summary>图像宽度（像素）</summary>
        Public Property ImageWidth As Integer = 2400

        ''' <summary>图像高度（像素）</summary>
        Public Property ImageHeight As Integer = 1400

        ''' <summary>输出 DPI（论文插图建议 300 及以上）</summary>
        Public Property Dpi As Integer = 300


        ' --- 边距 ---
        Public Property MarginTop As Integer = 90
        Public Property MarginBottom As Integer = 110
        Public Property MarginLeft As Integer = 160
        Public Property MarginRight As Integer = 120


        ' --- 布局参数 ---
        ''' <summary>每个基因组轨道的垂直高度</summary>
        Public Property TrackHeight As Integer = 90

        ''' <summary>染色体条带高度</summary>
        Public Property ChromosomeHeight As Integer = 16

        ''' <summary>基因块高度</summary>
        Public Property GeneHeight As Integer = 12

        ''' <summary>同一基因组内染色体之间的水平间距</summary>
        Public Property ChromosomeGap As Integer = 50


        ' --- 连接线参数 ---
        ''' <summary>连接线透明度（0-255，建议 60-140）</summary>
        Public Property ConnectionAlpha As Integer = 90

        ''' <summary>连接线宽度</summary>
        Public Property ConnectionWidth As Single = 1.2F


        ' --- 显示开关 ---
        Public Property ShowGeneLabels As Boolean = False
        Public Property ShowChromosomeLabels As Boolean = True
        Public Property ShowGenomeLabels As Boolean = True
        Public Property ShowLegend As Boolean = True
        Public Property ShowScaleBar As Boolean = True
        Public Property ShowGrid As Boolean = False
        Public Property UseAntiAlias As Boolean = True


        ' --- 文字 ---
        Public Property Title As String = "Pan-Genome Synteny Analysis"
        Public Property Subtitle As String = ""
        Public Property FontFamily As String = "Microsoft YaHei"


        ' --------------------------------------------------------------------
        '  预设主题工厂方法
        ' --------------------------------------------------------------------

        ''' <summary>默认主题：贝塞尔曲线 + Nature 配色</summary>
        Public Shared Function CreateDefault() As SyntenyTheme
            Dim t As New SyntenyTheme()
            t.Palette = ColorPalette.Nature()
            t.ConnectionStyle = ConnectionStyle.BezierCurve
            Return t
        End Function

        ''' <summary>直线主题</summary>
        Public Shared Function CreateStraightLineTheme() As SyntenyTheme
            Dim t As New SyntenyTheme()
            t.Palette = ColorPalette.Nature()
            t.ConnectionStyle = ConnectionStyle.StraightLine
            Return t
        End Function

        ''' <summary>贝塞尔曲线主题</summary>
        Public Shared Function CreateBezierTheme() As SyntenyTheme
            Dim t As New SyntenyTheme()
            t.Palette = ColorPalette.Nature()
            t.ConnectionStyle = ConnectionStyle.BezierCurve
            Return t
        End Function

        ''' <summary>论文发表主题：高分辨率 + 柔和配色</summary>
        Public Shared Function CreatePublicationTheme() As SyntenyTheme
            Dim t As New SyntenyTheme()
            t.Palette = ColorPalette.Pastel()
            t.ConnectionStyle = ConnectionStyle.BezierCurve
            t.ImageWidth = 3000
            t.ImageHeight = 1800
            t.Dpi = 600
            t.ConnectionAlpha = 70
            t.ConnectionWidth = 1.0F
            t.Title = ""
            Return t
        End Function
    End Class


    ' ========================================================================
    '  主绘图器
    ' ========================================================================

    ''' <summary>
    ''' 基因组共线性主绘图器。
    ''' 接收 Dictionary(Of String, Gene()) 形式的共线性数据，
    ''' 根据 SyntenyTheme 主题配置渲染 PNG 图像。
    ''' </summary>
    Public Class SyntenyPlotter

        Private _data As Dictionary(Of String, Gene())
        Private _theme As SyntenyTheme

        ' 布局缓存结构
        Private Class GenePos
            Public Property Gene As Gene
            Public Property Rect As RectangleF
            Public Property Center As PointF
        End Class

        Private Class ChromosomePos
            Public Property Id As String
            Public Property Rect As RectangleF
            Public Property Length As Long
            Public Property Genes As New List(Of GenePos)()
        End Class

        Private Class GenomePos
            Public Property Id As String
            Public Property Y As Single
            Public Property Chromosomes As New List(Of ChromosomePos)()
        End Class

        Private _genomeLayouts As New List(Of GenomePos)()
        Private _familyColors As New Dictionary(Of String, Color)()
        Private _familyOrder As New List(Of String)()


        ''' <summary>
        ''' 构造函数。
        ''' </summary>
        ''' <param name="data">共线性数据：键为基因组 ID，值为该基因组的基因数组</param>
        ''' <param name="theme">绘图主题</param>
        Public Sub New(data As Dictionary(Of String, Gene()), theme As SyntenyTheme)
            If data Is Nothing Then Throw New ArgumentNullException(NameOf(data))
            If theme Is Nothing Then Throw New ArgumentNullException(NameOf(theme))
            _data = data
            _theme = theme
        End Sub


        ''' <summary>
        ''' 执行绘图并保存为 PNG 文件。
        ''' </summary>
        ''' <param name="outputPath">输出 PNG 文件路径</param>
        Public Sub Plot(outputPath As String)
            PrepareData()
            ComputeLayout()
            AssignColors()
            Render(outputPath)
        End Sub


        ' --------------------------------------------------------------------
        '  数据预处理
        ' --------------------------------------------------------------------

        Private Sub PrepareData()
            ' 对每个基因组内的基因按染色体分组并按 start 排序，保证绘制顺序稳定
            For Each genomeId In _data.Keys.ToList()
                Dim genes = _data(genomeId)
                If genes Is Nothing Then Continue For
                Dim sorted = genes.OrderBy(Function(g) g.chromosome_id).
                                   ThenBy(Function(g) g.start).ToArray()
                _data(genomeId) = sorted
            Next
        End Sub


        ' --------------------------------------------------------------------
        '  布局计算
        ' --------------------------------------------------------------------

        Private Sub ComputeLayout()
            _genomeLayouts.Clear()

            Dim genomeIds = _data.Keys.ToList()
            Dim nGenomes = genomeIds.Count
            If nGenomes = 0 Then Return

            ' 可用绘图区域
            Dim drawWidth As Single = _theme.ImageWidth - _theme.MarginLeft - _theme.MarginRight
            Dim drawHeight As Single = _theme.ImageHeight - _theme.MarginTop - _theme.MarginBottom

            ' 每个基因组的垂直空间
            Dim trackSpace As Single = drawHeight / nGenomes

            For i = 0 To nGenomes - 1
                Dim gp As New GenomePos()
                gp.Id = genomeIds(i)
                gp.Y = _theme.MarginTop + i * trackSpace + trackSpace / 2

                ' 按染色体分组
                Dim genes = _data(gp.Id)
                If genes Is Nothing Then
                    _genomeLayouts.Add(gp)
                    Continue For
                End If

                Dim chromGroups = genes.GroupBy(Function(g) g.chromosome_id).ToList()

                ' 计算每条染色体长度（取该染色体上基因 end 的最大值）
                Dim chromLengths As New Dictionary(Of String, Long)()
                For Each grp In chromGroups
                    Dim maxEnd = grp.Max(Function(g) g.end)
                    chromLengths(grp.Key) = Math.Max(maxEnd, 1L)
                Next

                Dim totalLength As Long = chromLengths.Values.Sum()
                If totalLength <= 0 Then totalLength = 1L

                ' 染色体间总间距
                Dim totalGaps As Single = (chromGroups.Count - 1) * _theme.ChromosomeGap
                Dim availableWidth As Single = drawWidth - totalGaps
                If availableWidth < 10 Then availableWidth = 10

                ' 逐条染色体布局
                Dim xPos As Single = _theme.MarginLeft
                For Each grp In chromGroups
                    Dim cid = grp.Key
                    Dim clen = chromLengths(cid)
                    Dim cwidth As Single = CSng(clen) / totalLength * availableWidth
                    If cwidth < 5 Then cwidth = 5

                    Dim cp As New ChromosomePos()
                    cp.Id = cid
                    cp.Length = clen
                    cp.Rect = New RectangleF(xPos,
                                              gp.Y - _theme.ChromosomeHeight / 2,
                                              cwidth,
                                              _theme.ChromosomeHeight)

                    ' 布局该染色体上的基因
                    For Each g In grp.OrderBy(Function(x) x.start)
                        Dim genePos As New GenePos()
                        genePos.Gene = g
                        Dim gx As Single = cp.Rect.X + CSng(g.start) / clen * cp.Rect.Width
                        Dim gw As Single = CSng(g.end - g.start) / clen * cp.Rect.Width
                        If gw < 2 Then gw = 2  ' 保证最小可见宽度
                        genePos.Rect = New RectangleF(gx,
                                                       gp.Y - _theme.GeneHeight / 2,
                                                       gw,
                                                       _theme.GeneHeight)
                        genePos.Center = New PointF(gx + gw / 2, gp.Y)
                        cp.Genes.Add(genePos)
                    Next

                    gp.Chromosomes.Add(cp)
                    xPos += cwidth + _theme.ChromosomeGap
                Next

                _genomeLayouts.Add(gp)
            Next
        End Sub


        ' --------------------------------------------------------------------
        '  颜色分配
        ' --------------------------------------------------------------------

        Private Sub AssignColors()
            _familyColors.Clear()
            _familyOrder.Clear()

            ' 按首次出现顺序收集所有直系同源家族
            Dim seen As New HashSet(Of String)()
            For Each gp In _genomeLayouts
                For Each cp In gp.Chromosomes
                    For Each gpos In cp.Genes
                        Dim fam = gpos.Gene.orthology_family
                        If String.IsNullOrEmpty(fam) Then Continue For
                        If seen.Add(fam) Then
                            _familyOrder.Add(fam)
                        End If
                    Next
                Next
            Next

            ' 为每个家族分配颜色（循环使用调色板）
            For i = 0 To _familyOrder.Count - 1
                _familyColors(_familyOrder(i)) = _theme.Palette.GetConnectionColor(i)
            Next
        End Sub


        ' --------------------------------------------------------------------
        '  渲染主流程
        ' --------------------------------------------------------------------

        Private Sub Render(outputPath As String)
            Using bmp As New Bitmap(_theme.ImageWidth, _theme.ImageHeight, PixelFormat.Format32bppArgb)
                bmp.SetResolution(_theme.Dpi, _theme.Dpi)
                Using g As Graphics = Graphics.FromImage(bmp)
                    If _theme.UseAntiAlias Then
                        g.SmoothingMode = SmoothingMode.AntiAlias
                        g.TextRenderingHint = Text.TextRenderingHint.AntiAliasGridFit
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality
                    End If
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic

                    ' 1. 背景
                    g.Clear(_theme.Palette.Background)

                    ' 2. 网格（可选）
                    If _theme.ShowGrid Then DrawGrid(g)

                    ' 3. 标题
                    DrawTitle(g)

                    ' 4. 连接线（绘制在染色体下方）
                    DrawConnections(g)

                    ' 5. 染色体与基因
                    DrawChromosomesAndGenes(g)

                    ' 6. 标签
                    DrawLabels(g)

                    ' 7. 比例尺
                    If _theme.ShowScaleBar Then DrawScaleBar(g)

                    ' 8. 图例
                    If _theme.ShowLegend Then DrawLegend(g)
                End Using

                ' 保存为 PNG
                Dim dir = System.IO.Path.GetDirectoryName(outputPath)
                If Not String.IsNullOrEmpty(dir) AndAlso Not System.IO.Directory.Exists(dir) Then
                    System.IO.Directory.CreateDirectory(dir)
                End If
                bmp.Save(outputPath, ImageFormat.Png)
            End Using
        End Sub


        ' --------------------------------------------------------------------
        '  绘制：标题
        ' --------------------------------------------------------------------

        Private Sub DrawTitle(g As Graphics)
            If String.IsNullOrEmpty(_theme.Title) Then Return

            Dim titleFont As New Font(_theme.FontFamily, 22, FontStyle.Bold, GraphicsUnit.Pixel)
            Dim subFont As New Font(_theme.FontFamily, 13, FontStyle.Regular, GraphicsUnit.Pixel)
            Dim sf As New StringFormat()
            sf.Alignment = StringAlignment.Center
            sf.LineAlignment = StringAlignment.Near

            Dim titleX As Single = _theme.ImageWidth / 2
            g.DrawString(_theme.Title, titleFont,
                         New SolidBrush(_theme.Palette.TextColor),
                         titleX, 25, sf)

            If Not String.IsNullOrEmpty(_theme.Subtitle) Then
                g.DrawString(_theme.Subtitle, subFont,
                             New SolidBrush(_theme.Palette.SubTextColor),
                             titleX, 55, sf)
            End If

            titleFont.Dispose()
            subFont.Dispose()
        End Sub


        ' --------------------------------------------------------------------
        '  绘制：网格
        ' --------------------------------------------------------------------

        Private Sub DrawGrid(g As Graphics)
            Using pen As New Pen(_theme.Palette.GridColor, 1)
                ' 垂直网格线
                Dim drawWidth As Single = _theme.ImageWidth - _theme.MarginLeft - _theme.MarginRight
                Dim stepX As Single = drawWidth / 20
                For i = 0 To 20
                    Dim x = _theme.MarginLeft + i * stepX
                    g.DrawLine(pen, x, _theme.MarginTop, x, _theme.ImageHeight - _theme.MarginBottom)
                Next
                ' 水平网格线（每个基因组轨道）
                For Each gp In _genomeLayouts
                    g.DrawLine(pen, _theme.MarginLeft, gp.Y,
                               _theme.ImageWidth - _theme.MarginRight, gp.Y)
                Next
            End Using
        End Sub


        ' --------------------------------------------------------------------
        '  绘制：连接线（核心）
        ' --------------------------------------------------------------------

        Private Sub DrawConnections(g As Graphics)
            ' 相邻基因组轨道之间绘制共线性连接
            For i = 0 To _genomeLayouts.Count - 2
                Dim gp1 = _genomeLayouts(i)
                Dim gp2 = _genomeLayouts(i + 1)

                ' 构建家族 -> 基因位置 的映射
                Dim map1 As New Dictionary(Of String, List(Of GenePos))()
                Dim map2 As New Dictionary(Of String, List(Of GenePos))()

                For Each cp In gp1.Chromosomes
                    For Each gpos In cp.Genes
                        Dim fam = gpos.Gene.orthology_family
                        If String.IsNullOrEmpty(fam) Then Continue For
                        If Not map1.ContainsKey(fam) Then
                            map1(fam) = New List(Of GenePos)()
                        End If
                        map1(fam).Add(gpos)
                    Next
                Next

                For Each cp In gp2.Chromosomes
                    For Each gpos In cp.Genes
                        Dim fam = gpos.Gene.orthology_family
                        If String.IsNullOrEmpty(fam) Then Continue For
                        If Not map2.ContainsKey(fam) Then
                            map2(fam) = New List(Of GenePos)()
                        End If
                        map2(fam).Add(gpos)
                    Next
                Next

                ' 对每个共有家族绘制连接线
                For Each kvp In map1
                    Dim fam = kvp.Key
                    If Not map2.ContainsKey(fam) Then Continue For

                    Dim baseColor As Color = Color.Gray
                    If _familyColors.ContainsKey(fam) Then
                        baseColor = _familyColors(fam)
                    End If
                    Dim lineColor = Color.FromArgb(_theme.ConnectionAlpha, baseColor)

                    Using pen As New Pen(lineColor, _theme.ConnectionWidth)
                        pen.LineJoin = LineJoin.Round
                        pen.StartCap = LineCap.Round
                        pen.EndCap = LineCap.Round

                        For Each p1 In kvp.Value
                            For Each p2 In map2(fam)
                                DrawSingleConnection(g, pen, p1.Center, p2.Center)
                            Next
                        Next
                    End Using
                Next
            Next
        End Sub


        ''' <summary>
        ''' 根据主题中的 ConnectionStyle 绘制单条连接线。
        ''' </summary>
        Private Sub DrawSingleConnection(g As Graphics, pen As Pen, p1 As PointF, p2 As PointF)
            Select Case _theme.ConnectionStyle
                Case ConnectionStyle.StraightLine
                    g.DrawLine(pen, p1, p2)

                Case ConnectionStyle.BezierCurve
                    ' S 形贝塞尔曲线：控制点位于两端点垂直中点
                    Dim midY As Single = (p1.Y + p2.Y) / 2
                    Dim cp1 As New PointF(p1.X, midY)
                    Dim cp2 As New PointF(p2.X, midY)
                    g.DrawBezier(pen, p1, cp1, cp2, p2)
            End Select
        End Sub


        ' --------------------------------------------------------------------
        '  绘制：染色体与基因
        ' --------------------------------------------------------------------

        Private Sub DrawChromosomesAndGenes(g As Graphics)
            For Each gp In _genomeLayouts
                For Each cp In gp.Chromosomes
                    ' 染色体条带（圆角矩形）
                    Dim path As GraphicsPath = CreateRoundedRectPath(cp.Rect, 4)
                    Using brush As New SolidBrush(_theme.Palette.ChromosomeFill)
                        g.FillPath(brush, path)
                    End Using
                    Using pen As New Pen(_theme.Palette.ChromosomeBorder, 1.2F)
                        g.DrawPath(pen, path)
                    End Using
                    path.Dispose()

                    ' 基因块
                    For Each gpos In cp.Genes
                        Dim geneColor As Color
                        Dim fam = gpos.Gene.orthology_family
                        If Not String.IsNullOrEmpty(fam) AndAlso _familyColors.ContainsKey(fam) Then
                            ' 有家族的基因使用家族颜色
                            geneColor = _familyColors(fam)
                        Else
                            ' 无家族的基因按链方向着色
                            If gpos.Gene.strand = "+"c Then
                                geneColor = _theme.Palette.GeneForward
                            Else
                                geneColor = _theme.Palette.GeneReverse
                            End If
                        End If

                        ' 绘制基因块（带方向箭头）
                        DrawGeneBlock(g, gpos, geneColor)
                    Next
                Next
            Next
        End Sub


        ''' <summary>
        ''' 绘制单个基因块，根据链方向绘制箭头形状。
        ''' </summary>
        Private Sub DrawGeneBlock(g As Graphics, gpos As GenePos, color As Color)
            Dim r = gpos.Rect
            ' 如果基因块足够宽，绘制带方向的五边形箭头
            If r.Width > 8 Then
                Dim arrowW As Single = Math.Min(r.Width * 0.25, 5)
                Dim pts As PointF()
                If gpos.Gene.strand = "+"c Then
                    pts = New PointF() {
                        New PointF(r.Left, r.Top),
                        New PointF(r.Right - arrowW, r.Top),
                        New PointF(r.Right, r.Top + r.Height / 2),
                        New PointF(r.Right - arrowW, r.Bottom),
                        New PointF(r.Left, r.Bottom)
                    }
                Else
                    pts = New PointF() {
                        New PointF(r.Left + arrowW, r.Top),
                        New PointF(r.Right, r.Top),
                        New PointF(r.Right, r.Bottom),
                        New PointF(r.Left + arrowW, r.Bottom),
                        New PointF(r.Left, r.Top + r.Height / 2)
                    }
                End If
                Using brush As New SolidBrush(color)
                    g.FillPolygon(brush, pts)
                End Using
                Using pen As New Pen(Color.FromArgb(180, color), 0.5F)
                    g.DrawPolygon(pen, pts)
                End Using
            Else
                ' 基因太窄，直接画矩形
                Using brush As New SolidBrush(color)
                    g.FillRectangle(brush, r)
                End Using
            End If
        End Sub


        ' --------------------------------------------------------------------
        '  绘制：标签
        ' --------------------------------------------------------------------

        Private Sub DrawLabels(g As Graphics)
            Dim genomeFont As New Font(_theme.FontFamily, 13, FontStyle.Bold, GraphicsUnit.Pixel)
            Dim chromFont As New Font(_theme.FontFamily, 10, FontStyle.Regular, GraphicsUnit.Pixel)
            Dim geneFont As New Font(_theme.FontFamily, 7, FontStyle.Regular, GraphicsUnit.Pixel)

            Dim sfLeft As New StringFormat()
            sfLeft.Alignment = StringAlignment.Far
            sfLeft.LineAlignment = StringAlignment.Center

            Dim sfCenter As New StringFormat()
            sfCenter.Alignment = StringAlignment.Center
            sfCenter.LineAlignment = StringAlignment.Near

            ' 基因组标签（左侧）
            If _theme.ShowGenomeLabels Then
                For Each gp In _genomeLayouts
                    g.DrawString(gp.Id, genomeFont,
                                 New SolidBrush(_theme.Palette.TextColor),
                                 _theme.MarginLeft - 15, gp.Y, sfLeft)
                Next
            End If

            ' 染色体标签（染色体下方）
            If _theme.ShowChromosomeLabels Then
                For Each gp In _genomeLayouts
                    For Each cp In gp.Chromosomes
                        Dim labelY = cp.Rect.Bottom + 4
                        g.DrawString(cp.Id, chromFont,
                                     New SolidBrush(_theme.Palette.SubTextColor),
                                     cp.Rect.X + cp.Rect.Width / 2, labelY, sfCenter)
                    Next
                Next
            End If

            ' 基因标签（可选，仅当基因块足够宽时显示）
            If _theme.ShowGeneLabels Then
                For Each gp In _genomeLayouts
                    For Each cp In gp.Chromosomes
                        For Each gpos In cp.Genes
                            If gpos.Rect.Width > 30 Then
                                g.DrawString(gpos.Gene.geneId, geneFont,
                                             New SolidBrush(_theme.Palette.SubTextColor),
                                             gpos.Rect.X + gpos.Rect.Width / 2,
                                             gpos.Rect.Bottom + 2, sfCenter)
                            End If
                        Next
                    Next
                Next
            End If

            genomeFont.Dispose()
            chromFont.Dispose()
            geneFont.Dispose()
        End Sub


        ' --------------------------------------------------------------------
        '  绘制：比例尺
        ' --------------------------------------------------------------------

        Private Sub DrawScaleBar(g As Graphics)
            ' 在左下角绘制比例尺
            Dim font As New Font(_theme.FontFamily, 10, FontStyle.Regular, GraphicsUnit.Pixel)
            Dim x As Single = _theme.MarginLeft
            Dim y As Single = _theme.ImageHeight - _theme.MarginBottom + 30

            ' 取第一条基因组的总长度作为参考
            If _genomeLayouts.Count = 0 OrElse _genomeLayouts(0).Chromosomes.Count = 0 Then
                font.Dispose()
                Return
            End If

            Dim refGenome = _genomeLayouts(0)
            Dim totalLen As Long = 0
            For Each cp In refGenome.Chromosomes
                totalLen += cp.Length
            Next
            If totalLen <= 0 Then
                font.Dispose()
                Return
            End If

            Dim drawWidth As Single = _theme.ImageWidth - _theme.MarginLeft - _theme.MarginRight
            ' 选择一个"漂亮"的比例尺长度
            Dim niceLen As Long = ChooseNiceScaleLength(totalLen)
            Dim barWidth As Single = CSng(niceLen) / totalLen * drawWidth

            Using pen As New Pen(_theme.Palette.TextColor, 2)
                g.DrawLine(pen, x, y, x + barWidth, y)
                g.DrawLine(pen, x, y - 4, x, y + 4)
                g.DrawLine(pen, x + barWidth, y - 4, x + barWidth, y + 4)
            End Using

            Dim label As String = FormatGenomeLength(niceLen)
            Dim sf As New StringFormat()
            sf.Alignment = StringAlignment.Center
            g.DrawString(label, font, New SolidBrush(_theme.Palette.TextColor),
                         x + barWidth / 2, y + 6, sf)

            font.Dispose()
        End Sub


        ''' <summary>选择一个便于阅读的比例尺长度</summary>
        Private Function ChooseNiceScaleLength(total As Long) As Long
            Dim candidates As Long() = {1000, 5000, 10000, 50000, 100000,
                                        500000, 1000000, 5000000, 10000000}
            For Each c In candidates
                If c * 10 <= total Then Return c
            Next
            Return total / 10
        End Function


        ''' <summary>格式化基因组长度为可读字符串</summary>
        Private Function FormatGenomeLength(len As Long) As String
            If len >= 1000000 Then
                Return (len / 1000000.0).ToString("0.#") & " Mb"
            ElseIf len >= 1000 Then
                Return (len / 1000.0).ToString("0.#") & " kb"
            Else
                Return len.ToString() & " bp"
            End If
        End Function


        ' --------------------------------------------------------------------
        '  绘制：图例
        ' --------------------------------------------------------------------

        Private Sub DrawLegend(g As Graphics)
            Dim font As New Font(_theme.FontFamily, 10, FontStyle.Regular, GraphicsUnit.Pixel)
            Dim titleFont As New Font(_theme.FontFamily, 11, FontStyle.Bold, GraphicsUnit.Pixel)

            ' 图例区域：右下角
            Dim legendW As Single = 220
            Dim legendH As Single = 90
            Dim legendX As Single = _theme.ImageWidth - _theme.MarginRight - legendW
            Dim legendY As Single = _theme.ImageHeight - _theme.MarginBottom + 20

            ' 背景
            Dim bgRect As New RectangleF(legendX, legendY, legendW, legendH)
            Using brush As New SolidBrush(Color.FromArgb(250, 250, 250))
                g.FillRectangle(brush, bgRect)
            End Using
            Using pen As New Pen(_theme.Palette.GridColor, 1)
                g.DrawRectangle(pen, bgRect.X, bgRect.Y, bgRect.Width, bgRect.Height)
            End Using

            ' 标题
            g.DrawString("Legend", titleFont,
                         New SolidBrush(_theme.Palette.TextColor),
                         legendX + 10, legendY + 6)

            ' 正向链
            DrawArrowIcon(g, legendX + 15, legendY + 32, 18, 8,
                          _theme.Palette.GeneForward, "+"c)
            g.DrawString("Forward strand (+)", font,
                         New SolidBrush(_theme.Palette.TextColor),
                         legendX + 40, legendY + 28)

            ' 反向链
            DrawArrowIcon(g, legendX + 15, legendY + 52, 18, 8,
                          _theme.Palette.GeneReverse, "-"c)
            g.DrawString("Reverse strand (-)", font,
                         New SolidBrush(_theme.Palette.TextColor),
                         legendX + 40, legendY + 48)

            ' 连接线样式说明
            Dim styleText As String
            Select Case _theme.ConnectionStyle
                Case ConnectionStyle.StraightLine : styleText = "Straight line"
                Case ConnectionStyle.BezierCurve : styleText = "Bezier curve"
                Case Else : styleText = ""
            End Select
            g.DrawString("Link: " & styleText, font,
                         New SolidBrush(_theme.Palette.SubTextColor),
                         legendX + 10, legendY + 70)

            font.Dispose()
            titleFont.Dispose()
        End Sub


        Private Sub DrawArrowIcon(g As Graphics, x As Single, y As Single,
                                  w As Single, h As Single, color As Color, strand As Char)
            Dim pts As PointF()
            If strand = "+"c Then
                pts = New PointF() {
                    New PointF(x, y),
                    New PointF(x + w - 4, y),
                    New PointF(x + w, y + h / 2),
                    New PointF(x + w - 4, y + h),
                    New PointF(x, y + h)
                }
            Else
                pts = New PointF() {
                    New PointF(x + 4, y),
                    New PointF(x + w, y),
                    New PointF(x + w, y + h),
                    New PointF(x + 4, y + h),
                    New PointF(x, y + h / 2)
                }
            End If
            Using brush As New SolidBrush(color)
                g.FillPolygon(brush, pts)
            End Using
        End Sub


        ' --------------------------------------------------------------------
        '  辅助：创建圆角矩形路径
        ' --------------------------------------------------------------------

        Private Function CreateRoundedRectPath(rect As RectangleF, radius As Single) As GraphicsPath
            Dim path As New GraphicsPath()
            Dim r As Single = Math.Min(radius, rect.Height / 2)
            r = Math.Min(r, rect.Width / 2)
            If r < 0 Then r = 0

            path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90)
            path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90)
            path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90)
            path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90)
            path.CloseFigure()
            Return path
        End Function

    End Class

End Namespace
