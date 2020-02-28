Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Visualize of the multiple omics data association.
''' </summary>
Public Module OmicsScatter2D

    ''' <summary>
    ''' Plot foldchange data of two omics data in scatter plot.
    ''' </summary>
    ''' <param name="omicsX">Foldchange data or others</param>
    ''' <param name="omicsY">Foldchange data or others</param>
    ''' <param name="xlab"></param>
    ''' <param name="ylab"></param>
    ''' <returns></returns>
    Public Function Plot(omicsX As IEnumerable(Of NamedValue(Of Double)), omicsY As IEnumerable(Of NamedValue(Of Double)), xlab$, ylab$,
                         Optional labels As IEnumerable(Of NamedValue(Of String)) = Nothing,
                         Optional ignoresUnpaired As Boolean = True,
                         Optional pointSize! = 5) As GraphicsData

        Dim dataX = omicsX.ToDictionary(Function(g) g.Name, Function(g) g.Value)
        Dim dataY = omicsY.ToDictionary(Function(g) g.Name, Function(g) g.Value)
        Dim allLabels As String() = (dataX.Keys.AsList + dataY.Keys).Distinct.ToArray
        Dim points As PointData() = allLabels _
            .Where(Function(geneId)
                       If ignoresUnpaired Then
                           Return dataX.ContainsKey(geneId) AndAlso dataY.ContainsKey(geneId)
                       Else
                           Return True
                       End If
                   End Function) _
            .Select(Function(geneId)
                        Return New PointData With {
                            .pt = New PointF With {
                                .X = dataX.TryGetValue(geneId),
                                .Y = dataY.TryGetValue(geneId)
                            }
                        }
                    End Function) _
            .ToArray
        Dim serial As New SerialData With {
            .lineType = DashStyle.Dot,
            .PointSize = pointSize,
            .pts = points,
            .Shape = LegendStyles.Circle,
            .title = $"{xlab} ~ {ylab}",
            .DataAnnotations = labels _
                .SafeQuery _
                .Select(Function(geneId)
                            Return New Annotation With {
                                .Legend = LegendStyles.Triangle,
                                .Text = geneId.Value,
                                .X = dataX.TryGetValue(geneId.Name)
                            }
                        End Function) _
                .ToArray
        }

        Return Scatter.Plot({serial}, drawLine:=False, Xlabel:=xlab, Ylabel:=ylab)
    End Function

    Public Iterator Function Correlation(omicsX As IEnumerable(Of DataSet), omicsY As IEnumerable(Of DataSet), Optional cutoff As Double = 0.65) As IEnumerable(Of Connection)
        Dim dataY As DataSet() = omicsY.ToArray

        For Each dataX As DataSet In omicsX

        Next
    End Function

    Private Iterator Function CorrelationImpl() As IEnumerable

    End Function
End Module

Public Class Connection

    Public Property gene1 As String
    Public Property gene2 As String
    Public Property is_directly As Boolean
    Public Property cor As Double

End Class