Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace NCBIBlastResult

    ''' <summary>
    ''' 直系同源的数量统计结果数据模型
    ''' </summary>
    Public Class OrthologyProfile

        ''' <summary>
        ''' 功能分组名称标签信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Category As String

        ''' <summary>
        ''' + name: degree
        ''' + value: color
        ''' + description: n genes
        ''' </summary>
        ''' <returns></returns>
        Public Property HomologyDegrees As NamedValue(Of Color)()

        ''' <summary>
        ''' 当前的这个功能分组之中的同源基因的总数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Total As Integer
            Get
                Return Aggregate level As NamedValue(Of Color)
                       In HomologyDegrees
                       Let n = Val(level.Description)
                       Into Sum(n)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Category} ({Total})"
        End Function

    End Class

    Public Module OrthologyProfiles

        <Extension>
        Public Function OrthologyProfiles(result As IEnumerable(Of BBHIndex), colors As RangeList(Of Double, NamedValue(Of Color))) As OrthologyProfile()

        End Function

        <Extension>
        Public Function Plot(profileData As IEnumerable(Of OrthologyProfile),
                             Optional size$ = "3300,2700",
                             Optional margin$ = g.DefaultPadding,
                             Optional bg$ = "white",
                             Optional labelFontCSS$ = CSSFont.Win7LargeBold,
                             Optional boxBorderStrokeCSS$ = Stroke.AxisStroke,
                             Optional spacing! = 10,
                             Optional title$ = "Orthology Profiles",
                             Optional axisLabel$ = "Number of Orthology Genes",
                             Optional titleFontCSS$ = CSSFont.Win7VeryVeryLarge,
                             Optional axisLabelFontCSS$ = CSSFont.Win7VeryLarge) As GraphicsData

            Dim labelFont As Font = CSSFont.TryParse(labelFontCSS)
            Dim profiles As OrthologyProfile() = profileData.ToArray
            Dim maxCount% = profiles.Max(Function(category) category.Total)
            Dim maxLabel$ = profiles _
                .Select(Function(orth) orth.Category) _
                .MaxLengthString
            Dim boxStroke As Pen = Stroke.TryParse(boxBorderStrokeCSS)
            Dim titleFont As Font = CSSFont.TryParse(titleFontCSS)
            Dim axisLabelFont As Font = CSSFont.TryParse(axisLabelFontCSS)
            Dim plotInternal =
                Sub(ByRef g As IGraphics, region As GraphicsRegion)

                    ' 所有的category标签加起来的总高度，这个总高度也是绘图区域的总高度
                    Dim labelSize As SizeF = g.MeasureString("A", labelFont)
                    Dim totalHeight# = labelSize.Height * profiles.Length + spacing * (profiles.Length - 1)
                    Dim maxLabelWidth% = g.MeasureString(maxLabel, labelFont).Width

                    ' 绘图区域垂直居中显示
                    Dim top% = (region.Height - totalHeight) / 2
                    Dim left% = region.Padding.Left

                    ' 绘制盒子
                    ' 得到盒子的宽和高
                    Dim boxWidth = region.PlotRegion.Width - maxLabelWidth - 5
                    Dim boxHeight = totalHeight + boxStroke.Width * 2 + labelSize.Height
                    Dim boxLeft% = left + maxLabelWidth + 5
                    Dim pos As New Point(boxLeft, top)

                    ' 绘制盒子边框
                    Call g.DrawRectangle(boxStroke, New Rectangle(pos, New Size(boxWidth, boxHeight)))

                    ' 开始绘制每一个category的条形图
                    ' 这个条形图里面还包含有该分类之中的不同程度的同源结果
                    Dim x! = left
                    Dim y! = top + labelSize.Height / 2
                    Dim barWidth!
                    Dim barRect As Rectangle

                    For Each category As OrthologyProfile In profiles
                        Call g.DrawString(category.Category, labelFont, Brushes.Black, New PointF(x, y))

                        ' 绘制该功能分组之下的每一个同源层次的条形结果
                        x = boxLeft + boxStroke.Width / 2

                        For Each level As NamedValue(Of Color) In category _
                            .HomologyDegrees _
                            .OrderBy(Function(lv) lv.Name)

                            barWidth = boxWidth * (Val(level.Description) / maxCount)
                            barRect = New Rectangle With {
                                .Location = New Point(x, y),
                                .Size = New Size(barWidth, labelSize.Height)
                            }

                            g.FillRectangle(New SolidBrush(level.Value), barRect)
                            x += barWidth
                        Next

                        x = left
                        y = y + labelSize.Height + spacing
                    Next

                    ' 绘制标题和坐标轴的标签信息
                    ' 标题居中位置
                    labelSize = g.MeasureString(title, titleFont)
                    x = left + (region.PlotRegion.Width - labelSize.Width) / 2
                    y = top - spacing - labelSize.Height

                    Call g.DrawString(title, titleFont, Brushes.Black, x, y)

                    labelSize = g.MeasureString(axisLabel, axisLabelFont)
                    x = left + (region.PlotRegion.Width - labelSize.Width) / 2
                    y = top + boxHeight + spacing

                    Call g.DrawString(axisLabel, axisLabelFont, Brushes.Black, x, y)

                End Sub

            Return g.GraphicsPlots(
                size.SizeParser, margin, bg,
                plotAPI:=plotInternal
            )
        End Function
    End Module
End Namespace