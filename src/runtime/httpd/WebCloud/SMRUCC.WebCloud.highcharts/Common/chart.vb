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

Public Class chart

    Public Property type As String
    Public Property options3d As options3d
    Public Property zoomType As String
    Public Property inverted As Boolean?
    Public Property renderTo As String
    Public Property margin As Double?
    Public Property polar As Boolean?

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

Public Class Axis
    Public Property type As String
    Public Property allowDecimals As Boolean?
    Public Property className As String
    Public Property opposite As Boolean?
    Public Property title As title
    Public Property min As Double?
    Public Property max As Double?
    Public Property labels As labelOptions
    Public Property categories As String()
    Public Property startOnTick As Boolean?
    Public Property endOnTick As Boolean?
    Public Property showLastLabel As Boolean?
    Public Property gridLineWidth As Boolean?
    Public Property showFirstLabel As Boolean?
    Public Property crosshair As Boolean?
    Public Property tickInterval As Boolean?
    Public Property plotBands As Band()
    Public Property dateTimeLabelFormats As dateTimeLabelFormats
End Class

Public Class dateTimeLabelFormats
    Public Property month As String
    Public Property year As String
End Class

Public Class Band
    Public Property from As Double?
    Public Property [to] As Double?
    Public Property color As String
End Class

Public Class legendOptions
    Public Property enabled As Boolean?
    Public Property layout As String
    Public Property align As String
    Public Property verticalAlign As String
    Public Property x As Double?
    Public Property y As Double?
    Public Property floating As Boolean?
    Public Property borderWidth As Double?
    Public Property backgroundColor As String
    Public Property shadow As Boolean?
    Public Property reversed As Boolean?
End Class

Public Class title
    Public Property text As String
    Public Property align As String
    Public Property enable As Boolean?
    Public Property skew3d As Boolean?

    Public Overrides Function ToString() As String
        Return text
    End Function

    Public Shared Widening Operator CType(title As String) As title
        Return New title With {.text = title}
    End Operator
End Class

Public Class tooltip
    Public Property headerFormat As String
    Public Property pointFormat As String
    Public Property valueSuffix As String
    Public Property footerFormat As String
    Public Property [shared] As Boolean?
    Public Property useHTML As Boolean?
End Class

Public Class labelOptions
    Public Property connectorAllowed As Boolean?
    Public Property overflow As String
    Public Property skew3d As Boolean?
    Public Property style As styleOptions
    Public Property formatter As lambda
End Class

Public Class lambda
    Public Property func As String
End Class

Public Class styleOptions
    Public Property fontSize As String
End Class

Public Class dataLabels
    Public Property enabled As Boolean?
    Public Property format As String
End Class

Public Class responsiveOptions
    Public Property rules As rule()
End Class

Public Class rule
    Public Property condition As ruleConditions
    Public Property chartOptions As chartOptions
End Class

Public Class ruleConditions
    Public Property maxWidth As Double?
End Class

Public Class chartOptions
    Public Property legend As legendOptions
End Class

Public Class credits
    Public Property enabled As Boolean?
End Class
