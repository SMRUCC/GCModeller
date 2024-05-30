#Region "Microsoft.VisualBasic::a25c46ea3e8f5eb8a019d8f3eff60221, visualize\DataVisualizationExtensions\KeggPlot\EnrichmentCategoryHeatmap.vb"

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


    ' Code Statistics:

    '   Total Lines: 375
    '    Code Lines: 272
    ' Comment Lines: 40
    '   Blank Lines: 63
    '     File Size: 17.32 KB


    ' Class EnrichmentCategoryHeatmap
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetGroupHeat
    ' 
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports dataframe = Microsoft.VisualBasic.Math.DataFrame.DataFrame
Imports std = System.Math

Public Class EnrichmentCategoryHeatmap : Inherits HeatMapPlot

    ReadOnly rawdata As dataframe
    ReadOnly data As dataframe
    ReadOnly groupd As Dictionary(Of String, SampleInfo)
    ReadOnly metadata As dataframe
    ReadOnly kegg_class As String
    ReadOnly featureTree As Cluster

    ReadOnly no_class As SolidBrush = Brushes.LightGray
    ReadOnly group_labels As String()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data">
    ''' a dataframe object that contains the molecular expression data: 
    ''' the row names in the dataframe is the molecule name labels and 
    ''' all the column fields should be the expression value in different 
    ''' samples.
    ''' </param>
    ''' <param name="metadata">
    ''' the metadata for the molecules of given expression <paramref name="data"/>, should contains the metadata fields of:
    ''' 
    ''' 1. class: a character vector of the kegg class labels, example as pathway names, module names, or orthology labels
    ''' 2. logp: a numeric vector of the multiple group ANOVA test pvalue its log transform result of the molecules
    ''' 3. VIP: a numeric vector of the multiple group pls-da VIP result value for the molecules
    ''' 
    ''' the data field name is case-sensitive.
    ''' </param>
    ''' <param name="groupd"></param>
    ''' <param name="theme"></param>
    ''' <param name="kegg_class"></param>
    Public Sub New(data As dataframe, metadata As dataframe,
                   groupd As SampleInfo(),
                   theme As Theme,
                   Optional kegg_class As String = "class")

        Call MyBase.New(theme)

        data = data.Log(2).ZScale(byrow:=True)
        featureTree = data.PullDataSet(Of DataSet).RunCluster(, New CompleteLinkageStrategy)

        ' reorder by tree
        Me.group_labels = groupd.Select(Function(s) s.sample_info).Distinct.ToArray
        Me.rawdata = data.slice(featureTree.OrderLeafs)
        Me.metadata = metadata.slice(featureTree.OrderLeafs)
        Me.data = rawdata
        Me.groupd = groupd.ToDictionary(Function(s) s.ID)
        ' re-order column of samples by groups 
        Me.data = Me.data(groupd _
            .GroupBy(Function(s) s.sample_info) _
            .Select(Function(group)
                        Return group _
                            .OrderBy(Function(s) s.sample_name) _
                            .Select(Function(s)
                                        Return s.ID
                                    End Function)
                    End Function) _
            .IteratesALL)
        Me.kegg_class = kegg_class
    End Sub

    Private Function GetGroupHeat() As dataframe
        Dim group_heat = groupd.Values _
            .GroupBy(Function(s) s.sample_info) _
            .ToDictionary(Function(s) s.Key,
                          Function(s)
                              Dim group_data = rawdata(s.Select(Function(si) si.ID))
                              Dim sum As Vector = Nothing

                              For Each sample As FeatureVector In group_data.features.Values
                                  sum = sum + sample.TryCast(Of Double).AsVector
                              Next

                              Return FeatureVector.FromGeneral(s.Key, (sum / group_data.features.Count).ToArray)
                          End Function)

        Return New dataframe With {
            .rownames = rawdata.rownames,
            .features = group_heat
        }.Standard(byrow:=True).ZScale(byrow:=True)
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim rect As Rectangle = canvas.PlotRegion
        Dim delta As Double = rect.Width * 0.005
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim label_font As Font = css.GetFont(CSSFont.TryParse(theme.tagCSS))
        Dim tick_font As Font = css.GetFont(CSSFont.TryParse(theme.axisTickCSS))
        Dim charRectangle = g.MeasureString("A", label_font)
        Dim max_label_size As SizeF = g.MeasureString(data.rownames.MaxLengthString, label_font)
        Dim width As Double
        Dim label_region As New Rectangle(rect.Left, rect.Top, std.Max(rect.Width * 0.2, max_label_size.Width), rect.Height)
        Dim heatmap_region As New Rectangle(label_region.Right, rect.Top, rect.Width * 0.55, rect.Height)
        Dim tree_left As Double = heatmap_region.Right + delta / 2

        width = std.Min(rect.Width * 0.05, 4 * charRectangle.Width)
        Dim vip_region As New Rectangle(rect.Right - width - delta, rect.Top, width, rect.Height)
        width = rect.Width * 0.025
        Dim mean_log_region As New Rectangle(vip_region.Left - width - delta, rect.Top, width, rect.Height)
        width = std.Min(rect.Width * 0.1, 3 * charRectangle.Width * group_labels.Length)
        Dim group_heatmap_region As New Rectangle(mean_log_region.Left - width - delta, rect.Top, width, rect.Height)
        width = group_heatmap_region.Left - tree_left - delta
        Dim tree_region As New Rectangle(tree_left, rect.Top, width, rect.Height)

        Dim label_maxh As Single = label_region.Height / data.nsamples
        Dim legend_region As New Rectangle(rect.Right + delta, rect.Top, canvas.Padding.Right / 3, rect.Height)

        ' draw labels on left
        Dim y As Double = label_region.Top
        Dim x As Double
        Dim size As SizeF
        Dim i As Integer = 0

        ' draw metabolite name labels
        For Each name As String In data.rownames
            size = g.MeasureString(name, label_font)
            x = label_region.Right - size.Width - charRectangle.Width
            y = label_maxh * i + label_region.Top
            i += 1
            g.DrawString(name, label_font, Brushes.Black, New PointF(x, y))
        Next

        ' draw heatmap
        Dim dx = heatmap_region.Width / data.featureNames.Length
        Dim dy = heatmap_region.Height / data.rownames.Length
        Dim boxCell As RectangleF
        Dim heatmap As Brush() = Designer.GetColors(theme.colorSet, mapLevels) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray
        Dim range As New DoubleRange(data.features.Values.Select(Function(v) DirectCast(v.vector, Double())).IteratesALL)
        Dim index As New DoubleRange(0, mapLevels - 1)
        Dim vec As Func(Of Integer, Double)
        Dim color As Integer
        Dim [class] As String() = metadata(kegg_class).TryCast(Of String)
        Dim class_colors As New CategoryColorProfile([class].Where(Function(str) Not str.StringEmpty), "paper")
        Dim heatmap_ticks As Double() = range.CreateAxisTicks

        x = heatmap_region.Left
        y = heatmap_region.Top

        For Each col As String In data.featureNames
            boxCell = New RectangleF(x, y - dy - delta, dx + 3, dy)
            vec = data(col).NumericGetter

            ' draw group color bar
            Call g.FillRectangle(groupd(col).color.GetBrush, boxCell)

            ' fill heatmap column
            For i = 0 To data.rownames.Length - 1
                color = range.ScaleMapping(vec(i), index)
                boxCell = New RectangleF(x, y, dx, dy)
                g.FillRectangle(heatmap(color), boxCell)
                y += dy
            Next

            ' draw sample label
            Call g.DrawString(col, label_font, Brushes.Black, x + boxCell.Width, y + 20, 90)

            x += dx
            y = heatmap_region.Top
        Next

        ' draw class HC-tree
        x = tree_region.Left + delta
        dx = tree_region.Width * 0.2

        Dim big_label As New Font(label_font.FontFamily, emSize:=label_font.Size * 2.0!)
        Dim big_char As SizeF = g.MeasureString("A", big_label)

        Call g.DrawString("Group", big_label, Brushes.Black, x, y - dy - big_char.Height / 2)

        For i = 0 To data.rownames.Length - 1
            boxCell = New RectangleF(x, y, dx, dy)

            If [class](i).StringEmpty Then
                g.FillRectangle(no_class, boxCell)
            Else
                g.FillRectangle(New SolidBrush(class_colors.GetColor([class](i))), boxCell)
            End If

            y += dy
        Next

        If Not IsMicrosoftPlatform Then
            Call g.DrawString("KEGG Class", big_label, Brushes.Black, x + big_char.Width * 1.5, y, 90)
        Else
            Call g.DrawString("KEGG Class", big_label, Brushes.Black, x + big_char.Width, y, 90)
        End If

        Dim axis_line_pen As Pen = Stroke.TryParse(theme.axisStroke).GDIObject
        Dim treePlot As New HorizonRightToLeft With {
            .labelFont = label_font,
            .labelPadding = 0,
            .linkColor = axis_line_pen,
            .pointSize = 5,
            .showLeafLabels = False,
            .GetColor = Nothing,
            .log_scale = False,
            .log_base = 10
        }

        Call treePlot.DendrogramPlot(featureTree, g, New Rectangle(tree_region.Left + tree_region.Width * 0.3, tree_region.Top, tree_region.Width * 0.7, tree_region.Height))

        ' draw logp
        Dim logp = metadata("logp").TryCast(Of Double)
        Dim pval_range As New DoubleRange(logp)
        Dim pval_colors As SolidBrush() = Designer.GetBrushes(ScalerPalette.turbo.Description, mapLevels)

        x = mean_log_region.Left
        y = mean_log_region.Top
        dx = mean_log_region.Width * 0.95

        For i = 0 To data.rownames.Length - 1
            boxCell = New RectangleF(x, y, dx, dy)
            color = pval_range.ScaleMapping(logp(i), index)
            g.FillRectangle(pval_colors(color), boxCell)
            y += dy
        Next

        Call g.DrawString("-log(P)", big_label, Brushes.Black, x + big_char.Height, y, 90)

        ' draw average VIP
        Dim vip = metadata("VIP").TryCast(Of Double)
        Dim vip_ticks = vip.CreateAxisTicks(ticks:=5)
        Dim vip_scale = d3js.scale.linear.range(0, vip_region.Width).domain(values:=vip_ticks)
        Dim vip_color As Brush = Brushes.BlueViolet
        ' draw average pvalue significatent besides the vip bar plot
        Dim sig As String() = logp _
            .Select(Function(p)
                        If p >= 4 Then
                            Return New String("*"c, 4)
                        ElseIf p <= 0 Then
                            Return "not_sig"
                        Else
                            Return New String("*"c, CInt(p))
                        End If
                    End Function) _
            .ToArray

        x = vip_region.Left
        y = vip_region.Top

        For i = 0 To data.rownames.Length - 1
            boxCell = New RectangleF(x, y, vip_scale(vip(i)), dy * 0.85)
            y += dy
            g.FillRectangle(vip_color, boxCell)
            g.DrawString(sig(i), label_font, Brushes.Black, x, boxCell.Top)
        Next

        Dim tickFont As Font = css.GetFont(CSSFont.TryParse(theme.axisTickCSS))
        Dim tick_char As SizeF = g.MeasureString("A", tickFont)

        Call g.DrawLine(Pens.Black, CSng(vip_region.Left), CSng(y), vip_region.Right, CSng(y))

        For Each tick As Double In vip_ticks
            x = vip_scale(tick) + vip_region.Left

            Call g.DrawLine(Pens.Black, CSng(x), CSng(y), CSng(x), CSng(y + tick_char.Height / 2))
            Call g.DrawString(tick.ToString("F1"), tickFont, Brushes.Black, x + tick_char.Width / 2, y + tick_char.Height / 2, 90)
        Next

        Call g.DrawString("VIP", big_label, Brushes.Black, vip_region.Left, y + tick_char.Height * 2.5)

        ' draw group heatmap
        Dim group_heat = GetGroupHeat()
        Dim group_heatcolors As SolidBrush() = Designer.GetBrushes(ColorBrewer.DivergingSchemes.RdYlBu7, mapLevels).Reverse.ToArray
        Dim group_range As New DoubleRange(group_heat.features.Values.Select(Function(v) v.TryCast(Of Double)).IteratesALL)
        Dim group_tree = group_heat.features.Select(Function(v) New ClusterEntity(v.Key, v.Value.TryCast(Of Double))).RunVectorCluster

        dx = (group_heatmap_region.Width * 0.9) / group_heat.nfeatures
        x = group_heatmap_region.Left + group_heatmap_region.Width * 0.05
        y = group_heatmap_region.Top

        For Each group_name As String In group_tree.OrderLeafs
            Dim mean_z = group_heat(group_name).TryCast(Of Double)

            For i = 0 To mean_z.Length - 1
                color = group_range.ScaleMapping(mean_z(i), index)
                boxCell = New RectangleF(x, y, dx - 5, dy)
                y += dy
                g.FillRectangle(group_heatcolors(color), boxCell)
            Next

            Call g.DrawString(group_name, label_font, Brushes.Black, x + boxCell.Width / 2, y + 10, 90)

            x += dx
            y = group_heatmap_region.Top
        Next

        Dim group_tree_region As New Rectangle(group_heatmap_region.Left, group_heatmap_region.Top - 20, group_heatmap_region.Width, 20)
        'Dim plot_groupTree As New Horizon(group_tree, theme, showAllLabels:=False, showRuler:=False, showLeafLabels:=False)

        'Call plot_groupTree.Plot(g, group_tree_region)

        ' draw legends
        Dim scale_intensity_region As New Rectangle(legend_region.Left, legend_region.Top, legend_region.Width, legend_region.Height / 5)
        Dim group_mean_region As New Rectangle(legend_region.Left, legend_region.Top + 1 * (legend_region.Height / 4.5), legend_region.Width, legend_region.Height / 5)
        Dim logp_legend_region As New Rectangle(legend_region.Left, legend_region.Top + 2 * (legend_region.Height / 4.5), legend_region.Width, legend_region.Height / 5)
        Dim kegg_class_legend As New Rectangle(legend_region.Left, legend_region.Top + 3 * (legend_region.Height / 4.5), legend_region.Width, legend_region.Height / 4)

        'Call g.DrawString("Scaled Intensity", label_font, Brushes.Black, scale_intensity_region.Left, scale_intensity_region.Top)
        'Call g.DrawString("Scaled Mean Intensity", label_font, Brushes.Black, group_mean_region.Left, group_mean_region.Top)
        'Call g.DrawString("-log(p)", label_font, Brushes.Black, logp_legend_region.Left, logp_legend_region.Top)
        Call g.DrawString("KEGG Class", big_label, Brushes.Black, kegg_class_legend.Left, kegg_class_legend.Top)

        x = kegg_class_legend.Left
        y = kegg_class_legend.Top + big_label.Height * 2
        dy = (kegg_class_legend.Height - 20) / class_colors.size

        boxCell = New RectangleF(x, y, dy, dy)
        y += dy * 1.25

        g.FillRectangle(Brushes.Gray, boxCell)
        g.DrawString("no class", label_font, Brushes.Black, boxCell.Right + 5, boxCell.Top)

        For Each term As NamedValue(Of Color) In class_colors.GetTermColors
            boxCell = New RectangleF(x, y, dy, dy)
            y += dy * 1.25

            g.FillRectangle(New SolidBrush(term.Value), boxCell)
            g.DrawString(term.Name, label_font, Brushes.Black, boxCell.Right + charRectangle.Width / 2, boxCell.Top)
        Next

        big_label = New Font(label_font.FontFamily, label_font.Size * 1.5)

        Call New ColorMapLegend(heatmap.OfType(Of SolidBrush)) With {
            .format = "F1",
            .tickFont = label_font,
            .ticks = heatmap_ticks,
            .titleFont = big_label,
            .title = "Scaled Intensity",
            .tickAxisStroke = Pens.Black
        }.Draw(g, scale_intensity_region)

        Call New ColorMapLegend(group_heatcolors) With {
            .format = "F1",
            .tickFont = label_font,
            .ticks = group_range.CreateAxisTicks,
            .titleFont = big_label,
            .title = "Scaled Mean Intensity",
            .tickAxisStroke = Pens.Black
        }.Draw(g, group_mean_region)

        Call New ColorMapLegend(pval_colors) With {
            .format = "F1",
            .tickFont = label_font,
            .ticks = pval_range.CreateAxisTicks,
            .titleFont = big_label,
            .title = "-log(p)",
            .tickAxisStroke = Pens.Black
        }.Draw(g, logp_legend_region)
    End Sub
End Class
