#Region "Microsoft.VisualBasic::6a794e9d0e61e350d107497ed7b75200, visualize\DataVisualizationTools\NCBIBlastResult\OrthologyProfiles.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class OrthologyProfile
    ' 
    '         Properties: Category, HomologyDegrees, Total
    ' 
    '         Function: ToString
    ' 
    '     Module OrthologyProfiles
    ' 
    '         Function: DefaultColors, OrthologyProfiles, Plot, RenderColors
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
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
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
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
            Dim i As i32 = Scan0
            Dim colors As Color() = Designer.GetColors("RdBu:c8").AsList + Color.LightGray

            Return {
                New RangeTagValue(Of Double, NamedValue(Of Color))(99.99999999, 100, New NamedValue(Of Color)("a", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(97, 99.99999999, New NamedValue(Of Color)("b", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(95, 97, New NamedValue(Of Color)("c", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(90, 95, New NamedValue(Of Color)("d", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(80, 90, New NamedValue(Of Color)("e", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(60, 80, New NamedValue(Of Color)("f", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(50, 60, New NamedValue(Of Color)("g", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(40, 50, New NamedValue(Of Color)("h", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(30, 40, New NamedValue(Of Color)("i", colors(++i))),
                New RangeTagValue(Of Double, NamedValue(Of Color))(.0, 30, New NamedValue(Of Color)("j", Color.LightGray))
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
            Dim geneTable = result _
                .ToDictionary(Function(gene) gene.QueryName,
                              Function(gene)
                                  Dim coverage# = Val(gene.Properties!coverage)

                                  ' coverage可能会出现大于1的情况
                                  If coverage > 1 Then
                                      coverage = 1
                                  End If

                                  Return (KO:=gene.HitName, score:=BBHIndex.GetIdentities(gene) * coverage)
                              End Function)
            Dim KO_maps = geneTable _
                .Where(Function(gene) (Not gene.Value.KO.StringEmpty) AndAlso (Not gene.Value.KO.TextEquals("HITS_NOT_FOUND"))) _
                .Select(Function(gene)
                            Return New NamedValue(Of String) With {
                                .Name = gene.Key,
                                .Value = gene.Value.KO
                            }
                        End Function) _
                .ToArray
            Dim categories = PathwayMapping _
                .KOCatalog(KO_maps) _
                .GroupBy(Function(gene) gene.Value!Category) _
                .ToArray

            For Each category As IGrouping(Of String, NamedValue(Of Dictionary(Of String, String))) In categories
                Dim name$ = category.Key
                Dim scores#() = category.Select(Function(t) geneTable(t.Name).score).AsVector * 100
                Dim degrees = scores _
                    .Select(Function(score)
                                Return colors.SelectValue(score)
                            End Function) _
                    .GroupBy(Function(s) s.Name) _
                    .Select(Function(g)
                                Return New NamedValue(Of Color) With {
                                    .Name = g.Key,
                                    .Description = Math.Log(g.Count, 2),
                                    .Value = g.First.Value
                                }
                            End Function) _
                    .OrderBy(Function(s) s.Name) _
                    .ToArray

                Call name.__INFO_ECHO

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
                             Optional size$ = "3300,2700",
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
                             Optional tickStroke$ = Stroke.AxisStroke,
                             Optional dpi As Integer = 100) As GraphicsData

            Dim labelFont As Font = CSSFont.TryParse(labelFontCSS).GDIObject(dpi)
            Dim profiles As OrthologyProfile() = profileData _
                .OrderByDescending(Function(p) p.Total) _
                .ToArray
            Dim maxCount% = profiles.Max(Function(category) category.Total)
            Dim maxLabel$ = profiles _
                .Select(Function(orth) orth.Category) _
                .MaxLengthString
            Dim boxStroke As Pen = Stroke.TryParse(boxBorderStrokeCSS)
            Dim titleFont As Font = CSSFont.TryParse(titleFontCSS).GDIObject(dpi)
            Dim axisLabelFont As Font = CSSFont.TryParse(axisLabelFontCSS).GDIObject(dpi)
            Dim ticks#() = CatalogProfiling.GetTicks(maxCount, tick)
            Dim tickFont As Font = CSSFont.TryParse(axisTicksFontCSS).GDIObject(dpi)
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
                        Dim label$ = category.Category

                        With g.MeasureString(label, labelFont)
                            x = boxLeft - .Width - spacing
                            pos = New Point(x, y)
                            g.DrawString(label, labelFont, Brushes.Black, pos)
                        End With

                        ' 绘制该功能分组之下的每一个同源层次的条形结果
                        x = boxLeft + boxStroke.Width / 2

                        For Each level As NamedValue(Of Color) In category _
                            .HomologyDegrees _
                            .OrderBy(Function(lv) lv.Name)

                            barWidth = boxWidth * (Val(level.Description) / maxCount)
                            barRect = New Rectangle With {
                                .Location = New Point(x, y),
                                .Size = New Size(barWidth, labelSize.Height * 2 / 3)
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
