Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
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

        Public Function DefaultColors() As RangeList(Of Double, NamedValue(Of Color))
            Dim i As int = Scan0
            Dim colors As Color() = Designer.GetColors("RdBu:c5").AsList + Color.LightGray

            Return {
                New RangeTagValue(Of Double, NamedValue(Of Color))(90, 100, New NamedValue(Of Color)("a", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(80, 90, New NamedValue(Of Color)("b", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(60, 80, New NamedValue(Of Color)("c", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(50, 60, New NamedValue(Of Color)("d", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(30, 50, New NamedValue(Of Color)("e", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(.0, 30, New NamedValue(Of Color)("f", Color.LightGray))
            }
        End Function

        ''' <summary>
        ''' 这个函数只适用于KEGG直系同源
        ''' </summary>
        ''' <param name="result"></param>
        ''' <param name="colors"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function OrthologyProfiles(result As IEnumerable(Of BBHIndex), colors As RangeList(Of Double, NamedValue(Of Color))) As IEnumerable(Of OrthologyProfile)
            Dim brites As htext = htext.ko00001
            Dim KOTable As Dictionary(Of String, BriteHText) = brites.GetEntryDictionary
            Dim categories = result _
                .Select(Function(hit)
                            Return (category:=KOTable(hit.HitName).Class, score:=BBHIndex.GetIdentities(hit))
                        End Function) _
                .GroupBy(Function(hit) hit.category) _
                .ToArray

            For Each category As IGrouping(Of String, (name$, score#)) In categories
                Dim name$ = category.Key
                Dim scores#() = category.Select(Function(t) t.score).AsVector * 100
                Dim degrees = scores _
                    .Select(Function(score)
                                Return colors.SelectValue(score)
                            End Function) _
                    .ToArray

                Yield New OrthologyProfile With {
                    .Category = name,
                    .HomologyDegrees = degrees
                }
            Next
        End Function

        ''' <summary>
        ''' 测试用
        ''' </summary>
        ''' <param name="profile"></param>
        ''' <param name="spectrum$"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function RenderColors(profile As IEnumerable(Of OrthologyProfile), Optional spectrum$ = "RdBu:c5") As IEnumerable(Of OrthologyProfile)
            Dim colors As LoopArray(Of Color) = Designer.GetColors(spectrum)
            Dim profileData = profile.ToArray
            Dim allLevel As Dictionary(Of String, Color) =
                profileData _
                .Select(Function(category) category.HomologyDegrees.Keys) _
                .IteratesALL _
                .OrderBy(Function(l) l) _
                .Distinct _
                .ToDictionary(Function(level) level,
                              Function()
                                  Return colors.Next
                              End Function)

            For Each category As OrthologyProfile In profileData
                category.HomologyDegrees = category _
                    .HomologyDegrees _
                    .Select(Function(level)
                                Return New NamedValue(Of Color) With {
                                    .Name = level.Name,
                                    .Description = level.Description,
                                    .Value = allLevel(.Name)
                                }
                            End Function) _
                    .ToArray

                Yield category
            Next
        End Function

        <Extension>
        Public Function Plot(profileData As IEnumerable(Of OrthologyProfile),
                             Optional size$ = "2700,2100",
                             Optional margin$ = g.DefaultPadding,
                             Optional bg$ = "white",
                             Optional labelFontCSS$ = CSSFont.Win7LargeBold,
                             Optional boxBorderStrokeCSS$ = Stroke.AxisStroke,
                             Optional spacing! = 10,
                             Optional title$ = "Orthology Profiles",
                             Optional axisLabel$ = "Number of Orthology Genes",
                             Optional titleFontCSS$ = CSSFont.Win7VeryVeryLarge,
                             Optional axisLabelFontCSS$ = CSSFont.Win7VeryLarge,
                             Optional axisTicksFontCSS$ = CSSFont.Win7LittleLarge,
                             Optional tick# = -1,
                             Optional tickHeight% = 10,
                             Optional tickStroke$ = Stroke.AxisStroke) As GraphicsData

            Dim labelFont As Font = CSSFont.TryParse(labelFontCSS)
            Dim profiles As OrthologyProfile() = profileData.ToArray
            Dim maxCount% = profiles.Max(Function(category) category.Total)
            Dim maxLabel$ = profiles _
                .Select(Function(orth) orth.Category) _
                .MaxLengthString
            Dim boxStroke As Pen = Stroke.TryParse(boxBorderStrokeCSS)
            Dim titleFont As Font = CSSFont.TryParse(titleFontCSS)
            Dim axisLabelFont As Font = CSSFont.TryParse(axisLabelFontCSS)
            Dim ticks#() = CatalogProfiling.GetTicks(maxCount, tick)
            Dim tickFont As Font = CSSFont.TryParse(axisTicksFontCSS)
            Dim tickPen As Pen = Stroke.TryParse(tickStroke)

            ' 将最大的统计数量设置为axis的最大值，可以让图表更加美观
            maxCount = ticks.Max

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
                    Dim box As New Rectangle(pos, New Size(boxWidth, boxHeight))

                    ' 绘制盒子边框
                    Call g.DrawRectangle(boxStroke, box)

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

                    ' 绘制Ticks标签
                    Dim tickSize As SizeF
                    Dim anchor As PointF

                    For Each tickVal As Double In ticks
                        x = boxLeft + tickVal / maxCount * boxWidth
                        tickSize = g.MeasureString(tickVal, tickFont)
                        anchor = New PointF With {
                            .X = x - tickSize.Width / 2,
                            .Y = box.Bottom + spacing + 10
                        }

                        Call g.DrawLine(tickPen, New PointF(x, anchor.Y), New PointF(x, box.Bottom))
                        Call g.DrawString(tickVal, tickFont, Brushes.Black, anchor)
                    Next

                    ' 绘制标题和坐标轴的标签信息
                    ' 标题居中位置
                    labelSize = g.MeasureString(title, titleFont)
                    x = boxLeft + (boxWidth - labelSize.Width) / 2
                    y = top - spacing - labelSize.Height

                    Call g.DrawString(title, titleFont, Brushes.Black, x, y)

                    labelSize = g.MeasureString(axisLabel, axisLabelFont)
                    x = boxLeft + (boxWidth - labelSize.Width) / 2
                    y = top + boxHeight + spacing * 2 + tickSize.Height

                    Call g.DrawString(axisLabel, axisLabelFont, Brushes.Black, x, y)

                End Sub

            Return g.GraphicsPlots(
                size.SizeParser, margin, bg,
                plotAPI:=plotInternal
            )
        End Function
    End Module
End Namespace