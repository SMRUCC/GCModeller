Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

''' <summary>
''' COG, GO, KEGG
''' </summary>
Public Module CatalogProfiling

    ''' <summary>
    ''' No classification
    ''' </summary>
    Public Const NOT_ASSIGN As String = NameOf(NOT_ASSIGN)

    ''' <summary>
    ''' Catalog profiling bar plot
    ''' </summary>
    ''' <param name="profile"></param>
    ''' <param name="title$"></param>
    ''' <param name="colorSchema$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="size"></param>
    ''' <param name="margin"></param>
    ''' <param name="classFontStyle$"></param>
    ''' <param name="catalogFontStyle$"></param>
    ''' <param name="titleFontStyle$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ProfilesPlot(profile As Dictionary(Of String, NamedValue(Of Double)()),
                                 Optional title$ = "Profiling Plot",
                                 Optional axisTitle$ = "Number Of Gene",
                                 Optional colorSchema$ = "Set1:c6",
                                 Optional bg$ = "white",
                                 Optional size As Size = Nothing,
                                 Optional margin As Size = Nothing,
                                 Optional classFontStyle$ = CSSFont.Win7LargerBold,
                                 Optional catalogFontStyle$ = CSSFont.Win7Bold,
                                 Optional titleFontStyle$ = CSSFont.PlotTitle,
                                 Optional valueFontStyle$ = CSSFont.Win7Bold,
                                 Optional tickFontStyle$ = CSSFont.Win7LargerBold,
                                 Optional tick# = 50) As Bitmap

        If profile.ContainsKey(NOT_ASSIGN) Then
            profile = New Dictionary(Of String, NamedValue(Of Double)())(profile)
            profile.Remove(NOT_ASSIGN)
        End If

        Dim classes$() = profile.Keys.ToArray
        Dim colors As Color() = Designer.FromSchema(colorSchema, profile.Count - 1)
        Dim mapper As New Scaling(
            profile _
            .Values _
            .Select(Function(c) c.Select(Function(v) CDbl(v.Value))) _
            .IteratesALL, horizontal:=True)

        If size.IsEmpty Then
            size = New Size(2200, 2000)
        End If
        If margin.IsEmpty Then
            margin = New Size(25, 25)
        End If

        Return g.GraphicsPlots(
            size, margin,
            bg,
            Sub(ByRef g, regiong)

                Dim titleFont As Font = CSSFont.TryParse(titleFontStyle).GDIObject
                Dim catalogFont As Font = CSSFont.TryParse(catalogFontStyle).GDIObject
                Dim classFont As Font = CSSFont.TryParse(classFontStyle).GDIObject

                Dim maxLenSubKey$ = profile _
                    .Values _
                    .Select(Function(o) o.Select(Function(oo) oo.Name)) _
                    .IteratesALL _
                    .OrderByDescending(Function(s) s.Length) _
                    .First
                Dim maxLenClsKey$ = classes _
                    .OrderByDescending(Function(s) s.Length) _
                    .First
                Dim maxLenSubKeySize As SizeF = g.MeasureString(maxLenSubKey, catalogFont)
                Dim maxLenClsKeySize As SizeF = g.MeasureString(maxLenClsKey, classFont)
                Dim valueFont As Font = CSSFont.TryParse(valueFontStyle)

                Dim totalHeight = classes.Length * (maxLenClsKeySize.Height + 5) +
                    profile.Values.IteratesALL.Count * (maxLenSubKeySize.Height + 4) +
                    classes.Length * 20
                Dim left As Single, y! = (regiong.PlotRegion.Height - totalHeight) / 2
                Dim barRect As New Rectangle(
                    New Point(margin.Width * 1.5 + Math.Max(maxLenSubKeySize.Width, maxLenClsKeySize.Width), y),
                    New Size(size.Width - margin.Width * 2 - Math.Max(maxLenSubKeySize.Width, maxLenClsKeySize.Width) - margin.Width / 2, totalHeight))

                left = barRect.Left - margin.Width
                left = (size.Width - margin.Width * 2 - left) / 2 + left + margin.Width

                Dim titleSize As SizeF = g.MeasureString(title, titleFont)

                Call g.DrawString(title, titleFont, Brushes.Black, New PointF(barRect.Left + (barRect.Width - titleSize.Width) / 2, (y - titleSize.Height) / 2.0!))
                Call g.DrawRectangle(New Pen(Color.Black, 5), barRect)

                left = margin.Width

                Dim gap! = 10.0!

                For Each [class] As SeqValue(Of String) In classes.SeqIterator
                    Dim color As New SolidBrush(colors([class]))
                    Dim linePen As New Pen(colors([class]), 2) With {
                        .DashStyle = DashStyle.Dot
                    }
                    Dim yPlot!
                    Dim barWidth!
                    Dim barRectPlot As Rectangle
                    Dim valueSize As SizeF
                    Dim valueLeft!
                    Dim valueLabel$

                    ' 绘制Class大分类的标签
                    Call g.DrawString(+[class], classFont, Brushes.Black, New PointF(left, y))
                    y += maxLenClsKeySize.Height + 5

                    ' 绘制统计的小分类标签以及barplot图形
                    For Each cata As NamedValue(Of Double) In profile(+[class])
                        Call g.DrawString(cata.Name, catalogFont, color, New PointF(left + 25, y))

                        ' 绘制虚线
                        yPlot = y + maxLenSubKeySize.Height / 2
                        barWidth = mapper.ScallingWidth(cata.Value, barRect.Width - gap)
                        barRectPlot = New Rectangle(
                            New Point(barRect.Left, y),
                            New Size(barWidth - gap, maxLenSubKeySize.Height))

                        valueLabel = cata.Value.FormatNumeric(2)
                        valueSize = g.MeasureString(valueLabel, valueFont)
                        valueLeft = barRectPlot.Right - valueSize.Width

                        If valueLeft < barRect.Left Then
                            valueLeft = barRect.Left + 2
                        End If

                        Call g.DrawLine(linePen, New Point(barRect.Left, yPlot), New Point(barRect.Right, yPlot))
                        Call g.FillRectangle(color, barRectPlot)
                        Call g.DrawString(valueLabel, valueFont, Brushes.Black, New PointF(valueLeft, y - valueSize.Height / 3))

                        y += maxLenSubKeySize.Height + 4
                    Next

                    y += 20
                Next

                Dim axisTicks#() = AxisScalling.GetAxisByTick(
                    profile.Values.Max(Function(v) If(v.Length = 0, 0, v.Max(Function(n) n.Value))),
                    tick)
                Dim d# = 25
                Dim tickFont = CSSFont.TryParse(tickFontStyle)
                Dim tickSize As SizeF
                Dim tickPen As New Pen(Color.Black, 3)

                For Each tick In axisTicks
                    Dim tickX = barRect.Left + mapper.ScallingWidth(tick, barRect.Width - gap)

                    tickSize = g.MeasureString(tick, tickFont)

                    Call g.DrawLine(tickPen, New PointF(tickX, y), New PointF(tickX, y + d))
                    Call g.DrawString(tick, tickFont, Brushes.Black, New PointF(tickX - tickSize.Width / 2, y + d + 10))
                Next

                y += 100

                titleFont = CSSFont.TryParse(CSSFont.Win7LargerBold)
                titleSize = g.MeasureString(axisTitle, titleFont)

                Call g.DrawString(axisTitle, titleFont, Brushes.Black, New PointF(barRect.Left + (barRect.Width - titleSize.Width) / 2, y))

            End Sub)
    End Function
End Module
