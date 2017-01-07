Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data

Public Module KEGGOrthology

    Public Const NOT_ASSIGN As String = NameOf(NOT_ASSIGN)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mappings">blast mapping数据</param>
    ''' <param name="KO">基因和KO之间的对应关系</param>
    ''' <param name="level$">统计的等级</param>
    ''' <returns></returns>
    <Extension>
    Public Function CatalogProfiling(Of T As Map(Of String, String).IMap)(mappings As IEnumerable(Of T), KO As KO_gene(), Optional level$ = "A") As Dictionary(Of String, NamedValue(Of Integer)())
        Dim htext As htext = htext.ko00001
        Dim noMapping As Integer
        Dim out As New Dictionary(Of String, NamedValue(Of Integer)())
        Dim KO_genes As Dictionary(Of String, String()) = KO _
            .GroupBy(Function(x) $"{x.sp_code}:{x.gene}".ToLower) _
            .ToDictionary(Function(g) g.Key,
                          Function(k) k.Select(
                          Function(x) x.ko).Distinct.ToArray)
        Dim mappingGenes As Dictionary(Of String, Integer) =
            mappings _
            .GroupBy(Function(x) x.Maps.ToLower) _
            .ToDictionary(Function(k) k.Key,
                          Function(n) n.Count)
        Dim l As Char = level.First
        Dim KOcounts As New Dictionary(Of String, Integer)

        For Each hit In mappingGenes
            If KO_genes.ContainsKey(hit.Key) Then
                Dim k As String() = KO_genes(hit.Key)

                For Each ko_num$ In k
                    If Not KOcounts.ContainsKey(ko_num) Then
                        KOcounts.Add(ko_num, 0)
                    End If
                    KOcounts(ko_num) += hit.Value
                Next
            Else
                noMapping += hit.Value
            End If
        Next

        For Each A As BriteHText In htext.Hierarchical.CategoryItems
            Call out.Add(A.ClassLabel, A.__profiles(KOcounts, level))
        Next

        Call out.Add(
            NOT_ASSIGN, {
                New NamedValue(Of Integer) With {
                    .Name = NOT_ASSIGN,
                    .Value = noMapping
                }
            })

        Return out
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="htext"></param>
    ''' <param name="KOcounts"></param>
    ''' <param name="level"></param>
    <Extension>
    Private Function __profiles(htext As BriteHText,
                                KOcounts As Dictionary(Of String, Integer),
                                level As Char) As NamedValue(Of Integer)()

        Dim out As New List(Of NamedValue(Of Integer))

        If htext.CategoryLevel = level Then  ' 将本对象之中的所有sub都进行求和
            For Each [sub] As BriteHText In htext.CategoryItems.SafeQuery
                Dim counts As Integer = [sub] _
                    .EnumerateEntries _
                    .Where(Function(k) Not k.EntryId Is Nothing) _
                    .Sum(Function(ko) If(
                        KOcounts.ContainsKey(ko.EntryId),
                        KOcounts(ko.EntryId), 0))

                out += New NamedValue(Of Integer) With {
                    .Name = [sub].ClassLabel,
                    .Value = counts
                }
            Next
        Else
            For Each [sub] As BriteHText In htext.CategoryItems
                out += [sub].__profiles(KOcounts, level)
            Next
        End If

        Return out
    End Function

    ''' <summary>
    ''' KEGG Orthology Profiling Bar Plot
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
    Public Function Plot(profile As Dictionary(Of String, NamedValue(Of Integer)()),
                         Optional title$ = "KEGG Orthology Profiling",
                         Optional colorSchema$ = "Paired:c6",
                         Optional bg$ = "white",
                         Optional size As Size = Nothing,
                         Optional margin As Size = Nothing,
                         Optional classFontStyle$ = CSSFont.Win7LargerBold,
                         Optional catalogFontStyle$ = CSSFont.Win7Normal,
                         Optional titleFontStyle$ = CSSFont.PlotTitle,
                         Optional valueFontStyle$ = CSSFont.Win7Bold) As Bitmap

        Static KO_class$() = {
            "Cellular Processes",
            "Environmental Information Processing",
            "Genetic Information Processing",
            "Human Diseases",
            "Metabolism",
            "Organismal Systems"
        }

        If profile.ContainsKey(NOT_ASSIGN) Then
            profile = New Dictionary(Of String, NamedValue(Of Integer)())(profile)
            profile.Remove(NOT_ASSIGN)
        End If

        Dim colors As Color() = Designer.FromSchema(colorSchema, profile.Count - 1)
        Dim mapper As New Scaling(
            profile _
            .Values _
            .Select(Function(c) c.Select(Function(v) CDbl(v.Value))) _
            .IteratesALL, horizontal:=True)

        If size.IsEmpty Then
            size = New Size(2200, 2000)
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
                Dim maxLenClsKey$ = KO_class _
                    .OrderByDescending(Function(s) s.Length) _
                    .First
                Dim maxLenSubKeySize As SizeF = g.MeasureString(maxLenSubKey, catalogFont)
                Dim maxLenClsKeySize As SizeF = g.MeasureString(maxLenClsKey, classFont)
                Dim valueFont As Font = CSSFont.TryParse(valueFontStyle)

                Dim totalHeight = KO_class.Length * (maxLenClsKeySize.Height + 5) +
                    profile.Values.IteratesALL.Count * (maxLenSubKeySize.Height + 4) +
                    KO_class.Length * 20
                Dim left As Single, y! = 100 + (regiong.PlotRegion.Height - totalHeight) / 2
                Dim barRect As New Rectangle(
                    New Point(margin.Width * 2 + Math.Max(maxLenSubKeySize.Width, maxLenClsKeySize.Width), y),
                    New Size(size.Width - margin.Width * 2 - Math.Max(maxLenSubKeySize.Width, maxLenClsKeySize.Width) - margin.Width, totalHeight))

                left = barRect.Left - margin.Width
                left = (size.Width - margin.Width * 2 - left) / 2 + left + margin.Width

                Dim titleSize As SizeF = g.MeasureString(title, titleFont)

                Call g.DrawString(title, titleFont, Brushes.Black, New PointF(barRect.Left + (barRect.Width - titleSize.Width) / 2, (y - titleSize.Height) / 2.0!))
                Call g.DrawRectangle(New Pen(Color.Black, 5), barRect)

                left = margin.Width

                Dim gap! = 10.0!

                For Each [class] As SeqValue(Of String) In KO_class.SeqIterator
                    Dim color As New SolidBrush(colors([class]))
                    Dim linePen As New Pen(colors([class]), 3) With {
                        .DashStyle = DashStyle.Dot
                    }
                    Dim yPlot!
                    Dim barWidth!
                    Dim barRectPlot As Rectangle
                    Dim valueSize As SizeF
                    Dim valueLeft!

                    ' 绘制Class大分类的标签
                    Call g.DrawString(+[class], classFont, Brushes.Black, New PointF(left, y))
                    y += maxLenClsKeySize.Height + 5

                    ' 绘制统计的小分类标签以及barplot图形
                    For Each cata As NamedValue(Of Integer) In profile(+[class])
                        Call g.DrawString(cata.Name, catalogFont, color, New PointF(left + 25, y))

                        ' 绘制虚线
                        yPlot = y + maxLenSubKeySize.Height / 2
                        barWidth = mapper.ScallingWidth(cata.Value, barRect.Width - gap)
                        barRectPlot = New Rectangle(
                            New Point(barRect.Left, y),
                            New Size(barWidth - gap, maxLenSubKeySize.Height))
                        valueSize = g.MeasureString(cata.Value, valueFont)
                        valueLeft = barRectPlot.Right - valueSize.Width

                        If valueLeft < barRect.Left Then
                            valueLeft = barRect.Left + 2
                        End If

                        Call g.DrawLine(linePen, New Point(barRect.Left, yPlot), New Point(barRect.Right, yPlot))
                        Call g.FillRectangle(color, barRectPlot)
                        Call g.DrawString(cata.Value, valueFont, Brushes.Black, New PointF(valueLeft, y - valueSize.Height / 3))

                        y += maxLenSubKeySize.Height + 4
                    Next

                    y += 20
                Next
            End Sub)
    End Function
End Module
