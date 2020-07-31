Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.FuzzyCMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports DashStyle = System.Drawing.Drawing2D.DashStyle

Public Module PatternPlot

    Public Function DrawMatrix(raw As Matrix,
                               Optional dim$ = "3,3",
                               Optional size$ = "2400,2100",
                               Optional padding$ = g.DefaultPadding,
                               Optional bg$ = "white") As GraphicsData

        Return ExpressionPattern.CMeansCluster(raw, dim$.SizeParser.ToArray).DrawMatrix(
            size:=size,
            padding:=padding,
            bg:=bg
        )
    End Function

    <Extension>
    Public Function DrawMatrix(matrix As ExpressionPattern,
                               Optional size$ = "2400,2100",
                               Optional padding$ = g.DefaultPadding,
                               Optional bg$ = "white") As GraphicsData

        Dim plot = Sub(ByRef g As IGraphics, canvas As GraphicsRegion)
                       Dim w = canvas.PlotRegion.Width / matrix.dim(Scan0)
                       Dim h = canvas.PlotRegion.Height / matrix.dim(1)
                       Dim scatterData As SerialData()
                       Dim i As i32 = 1
                       Dim layout As GraphicsRegion
                       Dim x!
                       Dim y! = canvas.PlotRegion.Top + h

                       For Each row As Matrix() In matrix.GetPartitionMatrix

                           x = canvas.PlotRegion.Left + w

                           For Each col As Matrix In row
                               padding = $"padding: {y}px {canvas.Width - x + w}px {canvas.Height - y + h}px {x}"
                               layout = New GraphicsRegion(canvas.Size, padding)
                               x += w
                               scatterData = col.expression _
                                   .Select(Function(gene)
                                               Return New SerialData With {
                                                   .title = gene.geneID,
                                                   .color = Color.Red,
                                                   .lineType = DashStyle.Solid,
                                                   .pointSize = 5,
                                                   .width = 5,
                                                   .pts = gene.experiments _
                                                       .Select(Function(exp, idx)
                                                                   Return New PointData With {
                                                                       .tag = raw.sampleID(idx),
                                                                       .pt = New PointF With {
                                                                           .X = idx,
                                                                           .Y = exp
                                                                       }
                                                                   }
                                                               End Function) _
                                                       .ToArray
                                               }
                                           End Function) _
                                   .ToArray

                               Scatter.Plot(scatterData, g, layout)
                           Next

                           y += h
                       Next
                   End Sub

        Return g.GraphicsPlots(
            size:=size.SizeParser,
            padding:=padding,
            bg:=bg,
            plotAPI:=plot
        )
    End Function
End Module
