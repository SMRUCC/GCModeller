Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports stdNum = System.Math

''' <summary>
''' the bubble plot for the PFSNnet result.
''' </summary>
Public Module BubblePlot

    Public Function Plot(data As PFSNetResultOut,
                         Optional size$ = "1800,2100",
                         Optional colorA$ = "blue",
                         Optional colorB$ = "green",
                         Optional ptSize! = 10) As GraphicsData

        Dim bubbles As SerialData() = {
            data.phenotype1.CreateSerial(colorA.TranslateColor, ptSize, data.class1),
            data.phenotype2.CreateSerial(colorB.TranslateColor, ptSize, data.class2)
        }

        Return Bubble.Plot(
            data:=bubbles,
            size:=size.SizeParser,
            padding:=g.DefaultPadding,
            xlabel:="subnetwork statistics",
            ylabel:="-log10(pvalue)",
            xAxis:="(-40,40),n=10"
        )
    End Function

    <Extension>
    Public Function CreateSerial(classResult As PFSNetGraph(), color As Color, ptSize!, title$) As SerialData
        Dim xrange As DoubleRange = classResult _
            .Where(Function(a) a.pvalue > 0) _
            .Select(Function(a) a.statistics) _
            .Range
        Dim yMax As Double = classResult _
            .Where(Function(a) a.pvalue > 0) _
            .Select(Function(a)
                        Return -stdNum.Log10(a.pvalue)
                    End Function) _
            .Max

        Return New SerialData With {
            .color = color,
            .pointSize = ptSize,
            .pts = classResult _
                .Select(Function(a)
                            Dim y As Double = -stdNum.Log10(a.pvalue)
                            Dim x As Double

                            If y.IsNaNImaginary Then
                                If a.statistics > 0 Then
                                    x = xrange.Max * 1.25
                                Else
                                    x = xrange.Min * 1.25
                                End If
                            Else
                                x = a.statistics
                            End If

                            Return New PointData(x, If(y.IsNaNImaginary, yMax, y)) With {
                                .Tag = a.Id,
                                .value = a.nodes.Length
                            }
                        End Function) _
                .ToArray,
            .shape = LegendStyles.Circle,
            .title = title,
            .width = 2,
            .lineType = DashStyle.Dash
        }
    End Function
End Module
