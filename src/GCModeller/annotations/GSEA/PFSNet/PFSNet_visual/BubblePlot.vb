Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
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
                         Optional size$ = "2600,3600",
                         Optional colorA$ = "blue",
                         Optional colorB$ = "green",
                         Optional ptSize! = 10) As GraphicsData

        Dim bubbles As SerialData() = {
            data.phenotype1.CreateSerial(colorA.TranslateColor, ptSize, data.class1),
            data.phenotype2.CreateSerial(colorB.TranslateColor, ptSize, data.class2)
        }

        Return Bubble.Plot(bubbles, size.SizeParser, padding:=g.DefaultPadding)
    End Function

    <Extension>
    Public Function CreateSerial(classResult As PFSNetGraph(), color As Color, ptSize!, title$) As SerialData
        Return New SerialData With {
            .color = color,
            .pointSize = ptSize,
            .pts = classResult _
                .Select(Function(a)
                            Return New PointData(a.statistics, -stdNum.Log10(a.pvalue)) With {
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
