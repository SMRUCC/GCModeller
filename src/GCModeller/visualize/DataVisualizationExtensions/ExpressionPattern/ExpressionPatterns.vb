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

''' <summary>
''' 表达模式聚类
''' </summary>
Public Class ExpressionPattern

    Public Property Patterns As FuzzyCMeansEntity()
    Public Property sampleNames As String()
    Public Property [dim] As Integer()

    Public Function GetPartitionMatrix() As IEnumerable(Of Matrix())
        Return populatePartitions(Patterns, [dim], sampleNames)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="dim">``[row, columns]``</param>
    ''' <returns></returns>
    ''' 
    Public Shared Function CMeansCluster(matrix As Matrix, [dim] As Integer()) As ExpressionPattern
        Dim nsize As Integer = [dim](Scan0) * [dim](1)
        Dim sampleNames = matrix.sampleID
        Dim clusters = matrix.expression _
            .AsParallel _
            .Select(Function(gene)
                        Dim vector As New List(Of Double)

                        For i As Integer = 0 To sampleNames.Length - 1
                            Call vector.Add(gene.experiments(i))
                        Next

                        Return New FuzzyCMeansEntity With {
                            .uid = gene.geneID,
                            .Memberships = New Dictionary(Of Integer, Double),
                            .entityVector = vector.ToArray
                        }
                    End Function) _
            .FuzzyCMeans(numberOfClusters:=nsize)

        Return New ExpressionPattern With {
            .Patterns = clusters.ToArray,
            .sampleNames = sampleNames,
            .[dim] = [dim]
        }
    End Function

    Private Shared Iterator Function populatePartitions(clusters As IEnumerable(Of FuzzyCMeansEntity), dim%(), sampleNames As String()) As IEnumerable(Of Matrix())
        Dim row As New List(Of Matrix)

        For Each cluster In clusters.GroupBy(Function(c) c.cluster)
            Dim matrix = New Matrix With {
                .sampleID = sampleNames,
                .expression = cluster _
                    .Select(Function(a)
                                Return New DataFrameRow With {
                                    .geneID = a.uid,
                                    .experiments = a.entityVector
                                }
                            End Function) _
                    .ToArray
            }

            row += matrix

            If row = [dim](1) Then
                Yield row.PopAll
            End If
        Next

        If row > 0 Then
            Yield row.PopAll
        End If
    End Function

    Public Shared Function DrawMatrix(raw As Matrix,
                                      Optional dim$ = "3,3",
                                      Optional size$ = "2400,2100",
                                      Optional padding$ = g.DefaultPadding,
                                      Optional bg$ = "white") As GraphicsData

    End Function

    Public Shared Function DrawMatrix(raw As Matrix,
                                      Optional dim$ = "3,3",
                                      Optional size$ = "2400,2100",
                                      Optional padding$ = g.DefaultPadding,
                                      Optional bg$ = "white") As GraphicsData

        Dim matrix As ExpressionPattern = CMeansCluster(raw, dim$.SizeParser.ToArray)
        Dim plot = Sub(ByRef g As IGraphics, canvas As GraphicsRegion)
                       Dim w = canvas.PlotRegion.Width / matrix(Scan0).Length
                       Dim h = canvas.PlotRegion.Height / matrix.Length
                       Dim scatterData As SerialData()
                       Dim i As i32 = 1
                       Dim layout As GraphicsRegion
                       Dim x!
                       Dim y! = canvas.PlotRegion.Top + h

                       For Each row In matrix

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
End Class
