﻿#Region "Microsoft.VisualBasic::78a3bcf5cc5b9090096d19181e6cd1e0, visualize\DataVisualizationExtensions\ExpressionPattern\Extensions.vb"

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

'     Module PatternPlotExtensions
' 
'         Function: (+2 Overloads) DrawMatrix
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace ExpressionPattern

    <HideModuleName>
    Public Module PatternPlotExtensions

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
                                   Optional size$ = "6000,5200",
                                   Optional padding$ = g.DefaultPadding,
                                   Optional bg$ = "white",
                                   Optional title$ = "Expression Patterns",
                                   Optional xlab$ = "time groups",
                                   Optional ylab$ = "expression quantification",
                                   Optional colorSet$ = "YlGnBu:c8",
                                   Optional levels% = 50,
                                   Optional clusterLabelStyle As String = CSSFont.PlotSubTitle,
                                   Optional legendTitleStyle As String = CSSFont.Win7Small,
                                   Optional legendTickStyle As String = CSSFont.Win7Small,
                                   Optional axisTickCSS$ = CSSFont.Win10Normal,
                                   Optional axisLabelCSS$ = CSSFont.Win7Small,
                                   Optional driver As Drivers = Drivers.Default,
                                   Optional ppi As Integer = 300) As GraphicsData

            Dim theme As New Theme With {
                .background = bg,
                .padding = padding,
                .axisTickCSS = axisTickCSS,
                .axisLabelCSS = axisLabelCSS
            }

            Return New PatternPlot(matrix, theme, colorSet, levels) With {
                .main = title,
                .xlabel = xlab,
                .ylabel = ylab,
                .clusterLabelStyle = clusterLabelStyle,
                .legendTitleStyle = legendTitleStyle,
                .legendTickStyle = legendTickStyle
            }.Plot(size, driver:=driver, ppi:=ppi)
        End Function
    End Module
End Namespace
