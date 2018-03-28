#Region "Microsoft.VisualBasic::36b876db3b95e8c46cbc736f5509bd09, WebCloud\SMRUCC.WebCloud.highcharts\Common\chart.vb"

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

' Class chart
' 
'     Properties: inverted, margin, options3d, polar, renderTo
'                 type, zoomType
' 
'     Function: BarChart3D, PieChart3D, ToString
' 
' Class Axis
' 
'     Properties: allowDecimals, categories, className, crosshair, dateTimeLabelFormats
'                 endOnTick, gridLineWidth, labels, max, min
'                 opposite, plotBands, showFirstLabel, showLastLabel, startOnTick
'                 tickInterval, title, type
' 
' Class dateTimeLabelFormats
' 
'     Properties: month, year
' 
' Class Band
' 
'     Properties: [to], color, from
' 
' Class legendOptions
' 
'     Properties: align, backgroundColor, borderWidth, enabled, floating
'                 layout, reversed, shadow, verticalAlign, x
'                 y
' 
' Class title
' 
'     Properties: align, enable, skew3d, text
' 
'     Function: ToString
' 
' Class tooltip
' 
'     Properties: [shared], footerFormat, headerFormat, pointFormat, useHTML
'                 valueSuffix
' 
' Class labelOptions
' 
'     Properties: connectorAllowed, formatter, overflow, skew3d, style
' 
' Class lambda
' 
'     Properties: func
' 
' Class styleOptions
' 
'     Properties: fontSize
' 
' Class dataLabels
' 
'     Properties: enabled, format
' 
' Class responsiveOptions
' 
'     Properties: rules
' 
' Class rule
' 
'     Properties: chartOptions, condition
' 
' Class ruleConditions
' 
'     Properties: maxWidth
' 
' Class chartOptions
' 
'     Properties: legend
' 
' Class credits
' 
'     Properties: enabled
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.WebCloud.highcharts.viz3D

Public Class chart

    Public Property type As String
    Public Property options3d As options3d
    Public Property zoomType As String
    Public Property inverted As Boolean?
    Public Property renderTo As String
    Public Property margin As Double?
    Public Property polar As Boolean?
    Public Property plotBackgroundColor As String
    Public Property plotBorderWidth As String
    Public Property plotShadow As Boolean?

    Public Overrides Function ToString() As String
        If options3d Is Nothing OrElse Not options3d.enabled Then
            Return type
        Else
            Return $"[3D] {type}"
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function PieChart3D() As chart
        Return New chart With {
            .type = "pie",
            .options3d = New options3d With {
                .enabled = True,
                .alpha = 45,
                .beta = 0
            }
        }
    End Function

    Public Shared Function BarChart3D() As chart
        Return New chart With {
            .type = "column",
            .options3d = New options3d With {
                .enabled = True,
                .alpha = 10,
                .beta = 25,
                .depth = 70
            }
        }
    End Function
End Class