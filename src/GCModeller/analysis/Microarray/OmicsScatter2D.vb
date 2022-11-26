#Region "Microsoft.VisualBasic::f19e936f2bea42c3378821abce623519, GCModeller\analysis\Microarray\OmicsScatter2D.vb"

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

    '   Total Lines: 176
    '    Code Lines: 141
    ' Comment Lines: 19
    '   Blank Lines: 16
    '     File Size: 7.29 KB


    ' Module OmicsScatter2D
    ' 
    '     Function: Correlation, CorrelationImpl, Plot
    ' 
    ' Class Connection
    ' 
    '     Properties: cor, gene1, gene2, is_directly
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations

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
                         Optional size$ = "3000,3000",
                         Optional padding$ = g.DefaultPadding,
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
                        Dim xi# = dataX.TryGetValue(geneId)
                        Dim yi# = dataY.TryGetValue(geneId)
                        Dim color As Color

                        If Math.Abs(xi) >= 1 AndAlso Math.Abs(yi) >= 1 Then
                            color = Color.Red
                        ElseIf Math.Abs(xi) >= 1 OrElse Math.Abs(yi) >= 1 Then
                            color = Color.Green
                        Else
                            color = Color.Gray
                        End If

                        Return New PointData With {
                            .pt = New PointF With {
                                .X = xi,
                                .Y = yi
                            },
                            .color = color.ToHtmlColor
                        }
                    End Function) _
            .ToArray
        Dim serial As New SerialData With {
            .lineType = DashStyle.Dot,
            .pointSize = pointSize,
            .pts = points,
            .shape = LegendStyles.Circle,
            .title = $"{xlab} ~ {ylab}",
            .DataAnnotations = labels _
                .SafeQuery _
                .Select(Function(geneId)
                            Return New ChartPlots.Annotation With {
                                .Legend = LegendStyles.Triangle,
                                .Text = geneId.Value,
                                .X = dataX.TryGetValue(geneId.Name),
                                .Y = dataY.TryGetValue(geneId.Name)
                            }
                        End Function) _
                .ToArray
        }

        Return Scatter.Plot(
            c:={serial},
            drawLine:=False,
            Xlabel:=xlab,
            Ylabel:=ylab,
            absoluteScaling:=False,
            xlayout:=XAxisLayoutStyles.Bottom,
            ylayout:=YAxisLayoutStyles.Right,
            gridFill:="white",
            gridColor:=Color.LightGray.ToHtmlColor,
            size:=size,
            padding:=padding
        )
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="omicsX"></param>
    ''' <param name="omicsY"></param>
    ''' <param name="cutoff">An absolute value for the correlation cutoff.</param>
    ''' <param name="skipIndirect"></param>
    ''' <returns></returns>
    Public Function Correlation(omicsX As IEnumerable(Of DataSet),
                                omicsY As IEnumerable(Of DataSet),
                                Optional cutoff As Double = 0.65,
                                Optional skipIndirect As Boolean = False) As IEnumerable(Of Connection)

        Dim dataY As DataSet() = omicsY.ToArray
        Dim dataX As DataSet() = omicsX.ToArray
        Dim samples As String() = (dataY.PropertyNames.AsList + dataX.PropertyNames.AsList) _
            .Distinct _
            .ToArray

        Return dataX _
            .Select(Function(gene)
                        Return gene.CorrelationImpl(dataY, samples, isSelfComparison:=False, skipIndirect:=skipIndirect, cutoff:=cutoff)
                    End Function) _
            .IteratesALL
    End Function

    <Extension>
    Public Function CorrelationImpl(gene As DataSet, matrix As DataSet(), sampleNames$(), isSelfComparison As Boolean, skipIndirect As Boolean, cutoff#) As Connection()
        Dim fpkm As Double() = gene(sampleNames)
        Dim links As Connection() = matrix _
            .Where(Function(g)
                       If isSelfComparison Then
                           Return g.ID <> gene.ID
                       Else
                           Return True
                       End If
                   End Function) _
            .AsParallel _
            .Select(Function(g)
                        Dim fpkm2 As Double() = g(sampleNames)
                        Dim cor As Double = GetPearson(fpkm, fpkm2)

                        If Math.Abs(cor) >= cutoff AndAlso skipIndirect Then
                            Return New Connection With {
                                .cor = cor,
                                .gene1 = gene.ID,
                                .gene2 = g.ID,
                                .is_directly = True
                            }
                        Else
                            Return New Connection With {
                                .cor = Spearman(fpkm, fpkm2),
                                .gene1 = gene.ID,
                                .gene2 = g.ID,
                                .is_directly = False
                            }
                        End If
                    End Function) _
            .ToArray

        Call gene.ID.__INFO_ECHO

        Return links
    End Function
End Module

Public Class Connection

    Public Property gene1 As String
    Public Property gene2 As String
    Public Property is_directly As Boolean
    Public Property cor As Double

End Class
