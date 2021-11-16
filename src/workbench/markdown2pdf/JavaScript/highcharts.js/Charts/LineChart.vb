#Region "Microsoft.VisualBasic::9ff71f649f12a785091cd6d1a1edda06, markdown2pdf\JavaScript\highcharts.js\Charts\LineChart.vb"

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

    '     Class lineOptions
    ' 
    '         Properties: pointInterval, stacking
    ' 
    '     Class LineChart
    ' 
    ' 
    ' 
    '     Class DateTimeLineChart
    ' 
    ' 
    ' 
    '     Class LineWithRangeChart
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class LineRangeSerial
    ' 
    '         Properties: color, fillOpacity, lineWidth, linkedTo, marker
    '                     zIndex
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.WebCloud.JavaScript.highcharts.ScatterChart

Namespace LineChart

    Public Class lineOptions : Inherits seriesOptions

        Public Property stacking As String
        ''' <summary>
        ''' 这个属性可能是逻辑值或者数值，所以在这里使用字符串来兼容
        ''' </summary>
        ''' <returns></returns>
        Public Property pointInterval As Object

    End Class

    Public Class LineChart : Inherits Highcharts(Of GenericDataSerial)

    End Class

    Public Class DateTimeLineChart : Inherits Highcharts(Of serial)

    End Class

    Public Class LineWithRangeChart : Inherits Highcharts(Of LineRangeSerial)

        Sub New()
            Call MyBase.New
            Call MyBase.reference.Add("https://code.highcharts.com/highcharts-more.js")
        End Sub
    End Class

    ''' <summary>
    ''' <see cref="LineRangeSerial.data"/> 如果是datetime类型的话，则应该为``{unix_time_stamp, value}``的集合
    ''' 对于range类型，则应该是``{unix_time_stamp, min, max}``的集合
    ''' </summary>
    Public Class LineRangeSerial : Inherits AbstractSerial(Of Object())
        Public Property zIndex As Integer?
        Public Property marker As markerOptions
        Public Property lineWidth As Double?
        Public Property linkedTo As String
        Public Property color As String
        Public Property fillOpacity As Double?
    End Class
End Namespace
