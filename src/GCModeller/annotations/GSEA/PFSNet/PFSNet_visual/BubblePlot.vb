﻿#Region "Microsoft.VisualBasic::c7d7fa842cdfd145b1bc9ee912134249, annotations\GSEA\PFSNet\PFSNet_visual\BubblePlot.vb"

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

    '   Total Lines: 134
    '    Code Lines: 118 (88.06%)
    ' Comment Lines: 3 (2.24%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 13 (9.70%)
    '     File Size: 5.44 KB


    ' Module BubblePlot
    ' 
    '     Function: CreateSerial, Plot, popoutPoints
    ' 
    ' /********************************************************************************/

#End Region

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
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports std = System.Math

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

''' <summary>
''' the bubble plot for the PFSNnet result.
''' </summary>
Public Module BubblePlot

    ReadOnly class1 As [Default](Of String) = NameOf(class1)
    ReadOnly class2 As [Default](Of String) = NameOf(class2)

    Public Function Plot(data As PFSNetResultOut,
                         Optional size$ = "2800,2800",
                         Optional padding$ = "padding:100px 300px 250px 250px;",
                         Optional colorA$ = "blue",
                         Optional colorB$ = "green",
                         Optional ptSize! = 10,
                         Optional topN% = 8,
                         Optional alpha% = 220) As GraphicsData

        Dim bubbles As SerialData() = {
            data.phenotype1.CreateSerial(colorA.TranslateColor.Alpha(alpha), ptSize, data.class1 Or class1, topN),
            data.phenotype2.CreateSerial(colorB.TranslateColor.Alpha(alpha), ptSize, data.class2 Or class2, topN)
        }

        Return Bubble.Plot(
            data:=bubbles,
            size:=size,
            padding:=padding,
            xlabel:="subnetwork statistics",
            ylabel:="-log10(pvalue)",
            xAxis:="(-40,40),n=11",
            ylayout:=YAxisLayoutStyles.Centra,
            title:="Consist Subnetwork Enrichment",
            tagFontCSS:="font-style: normal; font-size: 10; font-family: " & FontFace.SegoeUI & ";"
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
                        Return -std.Log10(a.pvalue)
                    End Function) _
            .Max
        Dim i As i32 = Scan0

        For Each group As IGrouping(Of String, PFSNetGraph) In classResult.GroupBy(Function(a) a.Id).OrderBy(Function(a) a.Min(Function(n) n.pvalue))
            Dim takes As PFSNetGraph()

            If group.All(Function(a) a.pvalue = group.First.pvalue) Then
                takes = {group.First}
            Else
                takes = group.ToArray
            End If

            For Each subnetwork As PFSNetGraph In takes
                Dim y As Double = -std.Log10(subnetwork.pvalue)
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
                    .value = subnetwork.nodes.Length * 3,
                    .stroke = Stroke.StrongHighlightStroke
                }
            Next
        Next
    End Function
End Module
