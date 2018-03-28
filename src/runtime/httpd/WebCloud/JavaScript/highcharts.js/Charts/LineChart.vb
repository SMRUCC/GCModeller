#Region "Microsoft.VisualBasic::ddb718caaa87037b04e772455cff225c, WebCloud\SMRUCC.WebCloud.highcharts\Charts\LineChart.vb"

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
    '         Properties: label, pointInterval, pointStart, stacking
    ' 
    '     Class LineChart
    ' 
    ' 
    ' 
    '     Class DateTimeLineChart
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace LineChart

    Public Class lineOptions
        Public Property label As labelOptions
        Public Property pointStart As Object
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
End Namespace
