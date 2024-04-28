#Region "Microsoft.VisualBasic::13d3ede586b42f2d4b8bd7d444cc5cfb, G:/GCModeller/src/GCModeller/visualize/DataVisualizationExtensions//KeggPlot/EnrichmentCategoryHeatmap.vb"

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

'   Total Lines: 185
'    Code Lines: 147
' Comment Lines: 8
'   Blank Lines: 30
'     File Size: 7.80 KB


' Class EnrichmentCategoryHeatmap
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: PlotInternal
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
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
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports dataframe = Microsoft.VisualBasic.Math.DataFrame.DataFrame

Public Class EnrichmentCategoryHeatmap : Inherits HeatMapPlot

    ReadOnly rawdata As dataframe
    ReadOnly data As dataframe
    ReadOnly groupd As Dictionary(Of String, SampleInfo)
    ReadOnly metadata As dataframe
    ReadOnly kegg_class As String
    ReadOnly featureTree As Cluster

    Public Sub New(data As dataframe, metadata As dataframe, groupd As SampleInfo(), theme As Theme, Optional kegg_class As String = "class")
        MyBase.New(theme)

        featureTree = data.PullDataSet(Of DataSet).RunCluster

        Me.rawdata = data.slice(featureTree.OrderLeafs)
        Me.metadata = metadata.slice(featureTree.OrderLeafs)
        Me.data = rawdata.ZScale(byrow:=True)
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

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim rect As Rectangle = canvas.PlotRegion
        Dim label_region As New Rectangle(rect.Left, rect.Top, rect.Width * 0.2, rect.Height)
        Dim heatmap_region As New Rectangle(label_region.Right, rect.Top, rect.Width * 0.4, rect.Height)
        Dim tree_region As New Rectangle(heatmap_region.Right, rect.Top, rect.Width * 0.1, rect.Height)
        Dim group_heatmap_region As New Rectangle(tree_region.Right, rect.Top, rect.Width * 0.1, rect.Height)
        Dim mean_log_region As New Rectangle(group_heatmap_region.Right, rect.Top, rect.Width * 0.1, rect.Height)
        Dim vip_region As New Rectangle(mean_log_region.Right, rect.Top, rect.Width * 0.1, rect.Height)
        Dim label_font As Font = CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)
        Dim label_maxh As Single = label_region.Height / data.nsamples

        ' draw labels on left
        Dim y As Double = label_region.Top
        Dim x As Double
        Dim size As SizeF
        Dim i As Integer = 0

        For Each name As String In data.rownames
            size = g.MeasureString(name, label_font)
            x = label_region.Right - size.Width
            y = label_maxh * i + label_region.Top - size.Height / 2
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
        Dim class_colors As New CategoryColorProfile([class], "paper")

        x = heatmap_region.Left
        y = heatmap_region.Top

        For Each col As String In data.featureNames
            boxCell = New RectangleF(x, y - dy - 10, dx + 3, dy)
            vec = data(col).NumericGetter

            ' draw group color bar
            Call g.FillRectangle(groupd(col).color.GetBrush, boxCell)

            For i = 0 To data.rownames.Length - 1
                color = range.ScaleMapping(vec(i), index)
                boxCell = New RectangleF(x, y, dx, dy)
                g.FillRectangle(heatmap(color), boxCell)
                y += dy
            Next

            Call g.DrawString(col, label_font, Brushes.Black, x + boxCell.Width / 2, y + 20, 60)

            x += dx
            y = heatmap_region.Top
        Next

        ' draw class HC-tree
        x = tree_region.Left + 5
        dx = tree_region.Width * 0.25

        Call g.DrawString("Group", label_font, Brushes.Black, x, y - dy - 5)

        For i = 0 To data.rownames.Length - 1
            boxCell = New RectangleF(x, y, dx, dy)
            g.FillRectangle(New SolidBrush(class_colors.GetColor([class](i))), boxCell)
            y += dy
        Next

        Dim treePlot As New HorizonRightToLeft With {
            .labelFont = label_font,
            .labelPadding = 0,
            .linkColor = New Pen(Brushes.Black, 5),
            .pointSize = 5,
            .showLeafLabels = False,
            .GetColor = Nothing,
            .log_scale = True,
            .log_base = 10
        }

        Call treePlot.DendrogramPlot(featureTree, g, New Rectangle(tree_region.Left + tree_region.Width * 0.3, tree_region.Top, tree_region.Width * 0.7, tree_region.Height))

        ' draw logp
        Dim logp = metadata("logp").TryCast(Of Double)
        Dim prange As New DoubleRange(logp)
        Dim pcolors = Designer.GetColors("jet", mapLevels)

        x = mean_log_region.Left
        y = mean_log_region.Top

        For i = 0 To data.rownames.Length - 1
            boxCell = New RectangleF(x, y, dx, dy)
            color = prange.ScaleMapping(logp(i), index)
            g.FillRectangle(New SolidBrush(pcolors(color)), boxCell)
            y += dy
        Next

        ' draw average VIP
        Dim vip = metadata("VIP").TryCast(Of Double)
        Dim vip_ticks = vip.CreateAxisTicks(ticks:=5)
        Dim vip_scale = d3js.scale.linear.range(0, vip_region.Width).domain(values:=vip_ticks)
        Dim vip_color As Brush = Brushes.BlueViolet

        x = vip_region.Left
        y = vip_region.Top

        For i = 0 To data.rownames.Length - 1
            boxCell = New RectangleF(x, y, vip_scale(vip(i)), dy * 0.85)
            y += dy
            g.FillRectangle(vip_color, boxCell)
        Next

        ' draw group heatmap
        Dim group_heat = groupd.Values _
            .GroupBy(Function(s) s.sample_info) _
            .ToDictionary(Function(s) s.Key,
                          Function(s)
                              Dim group_data = rawdata(s.Select(Function(si) si.ID))
                              Dim sum As Vector = Nothing

                              For Each sample As FeatureVector In group_data.features.Values
                                  sum = sum + sample.TryCast(Of Double).AsVector
                              Next

                              Return (sum / group_data.features.Count).Z.ToArray
                          End Function)
        Dim group_heatcolors As Color() = Designer.GetColors(ColorBrewer.DivergingSchemes.RdYlBu7, mapLevels)
        Dim group_range As New DoubleRange(group_heat.Values.IteratesALL)
        Dim group_tree = group_heat.Select(Function(v) New ClusterEntity(v.Key, v.Value)).RunVectorCluster

        dx = group_heatmap_region.Width / group_heat.Count
        x = group_heatmap_region.Left
        y = group_heatmap_region.Top

        For Each group_name As String In group_tree.OrderLeafs
            Dim mean_z = group_heat(group_name)

            For i = 0 To mean_z.Length - 1
                color = group_range.ScaleMapping(mean_z(i), index)
                boxCell = New RectangleF(x, y, dx - 5, dy)
                y += dy
                g.FillRectangle(New SolidBrush(group_heatcolors(color)), boxCell)
            Next

            Call g.DrawString(group_name, label_font, Brushes.Black, x + boxCell.Width / 2, y + 10, 60)

            x += dx
            y = group_heatmap_region.Top
        Next
    End Sub
End Class
