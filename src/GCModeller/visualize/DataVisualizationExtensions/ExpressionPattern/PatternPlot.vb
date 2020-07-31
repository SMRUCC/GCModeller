Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports DashStyle = System.Drawing.Drawing2D.DashStyle

Namespace ExpressionPattern

    Public Class PatternPlot : Inherits Plot

        Public Property matrix As ExpressionPattern

        Public Sub New(theme As Theme)
            MyBase.New(theme)
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim w = canvas.PlotRegion.Width / matrix.dim(Scan0)
            Dim h = canvas.PlotRegion.Height / matrix.dim(1)
            Dim scatterData As SerialData()
            Dim i As i32 = 1
            Dim layout As GraphicsRegion
            Dim x!
            Dim y! = canvas.PlotRegion.Top + h
            Dim padding As String

            For Each row As Matrix() In matrix.GetPartitionMatrix
                x = canvas.PlotRegion.Left + w

                For Each col As Matrix In row
                    padding = $"padding: {y}px {canvas.Width - x + w}px {canvas.Height - y + h}px {x}"
                    layout = New GraphicsRegion(canvas.Size, padding)
                    x += w
                    scatterData = col.DoCall(AddressOf createLines).ToArray

                    Call Scatter.Plot(scatterData, g, layout, Xlabel:=xlabel, Ylabel:=ylabel)
                Next

                y += h
            Next
        End Sub

        Private Iterator Function createLines(col As Matrix) As IEnumerable(Of SerialData)
            Dim rawSampleId As String() = matrix.sampleNames

            For Each gene In col.expression
                Yield New SerialData With {
                    .title = gene.geneID,
                    .color = Color.Red,
                    .lineType = DashStyle.Solid,
                    .pointSize = 5,
                    .width = 5,
                    .pts = gene.experiments _
                        .Select(Function(exp, idx)
                                    Return New PointData With {
                                        .tag = rawSampleId(idx),
                                        .pt = New PointF With {
                                            .X = idx,
                                            .Y = exp
                                        }
                                    }
                                End Function) _
                        .ToArray
                }
            Next
        End Function
    End Class
End Namespace