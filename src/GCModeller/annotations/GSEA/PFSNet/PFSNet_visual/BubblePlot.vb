Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports stdNum = System.Math

''' <summary>
''' the bubble plot for the PFSNnet result.
''' </summary>
Public Module BubblePlot

    ReadOnly class1 As [Default](Of String) = NameOf(class1)
    ReadOnly class2 As [Default](Of String) = NameOf(class2)

    Public Function Plot(data As PFSNetResultOut,
                         Optional size$ = "1400,1400",
                         Optional padding$ = "padding:100px 200px 150px 150px;",
                         Optional colorA$ = "blue",
                         Optional colorB$ = "green",
                         Optional ptSize! = 10,
                         Optional topN% = 8) As GraphicsData

        Dim bubbles As SerialData() = {
            data.phenotype1.CreateSerial(colorA.TranslateColor, ptSize, data.class1 Or class1, topN),
            data.phenotype2.CreateSerial(colorB.TranslateColor, ptSize, data.class2 Or class2, topN)
        }

        Return Bubble.Plot(
            data:=bubbles,
            size:=size.SizeParser,
            padding:=padding,
            xlabel:="subnetwork statistics",
            ylabel:="-log10(pvalue)",
            xAxis:="(-40,40),n=11",
            ylayout:=YAxisLayoutStyles.Centra,
            title:="Consist Subnetwork Enrichment"
        )
    End Function

    <Extension>
    Public Function CreateSerial(classResult As PFSNetGraph(), color As Color, ptSize!, title$, topN%) As SerialData
        Return New SerialData With {
            .color = color,
            .pointSize = ptSize,
            .pts = popoutPoints(classResult, topN).ToArray,
            .shape = LegendStyles.Circle,
            .title = title,
            .width = 2,
            .lineType = DashStyle.Dash
        }
    End Function

    Private Iterator Function popoutPoints(classResult As PFSNetGraph(), topN%) As IEnumerable(Of PointData)
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
        Dim i As i32 = Scan0

        For Each subnetwork As PFSNetGraph In classResult.OrderBy(Function(a) a.pvalue)
            Dim y As Double = -stdNum.Log10(subnetwork.pvalue)
            Dim x As Double

            If y.IsNaNImaginary Then
                If subnetwork.statistics > 0 Then
                    x = xrange.Max * 1.25
                Else
                    x = xrange.Min * 1.25
                End If
            Else
                x = subnetwork.statistics
            End If

            Yield New PointData(x, If(y.IsNaNImaginary, yMax * 1.25, y)) With {
                .tag = If(++i > topN, Nothing, subnetwork.Id),
                .value = subnetwork.nodes.Length
            }
        Next
    End Function
End Module
