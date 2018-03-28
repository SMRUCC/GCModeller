#Region "Microsoft.VisualBasic::b97900adbbe9cbc2282253e4e12f8615, WebCloud\SMRUCC.WebCloud.highcharts\Charts\AreaChart.vb"

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

    '     Class AreaSplineChart
    ' 
    '         Function: ChartType
    ' 
    '     Class areasplineOptions
    ' 
    '         Properties: fillOpacity
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace AreaChart

    Public Class AreaSplineChart : Inherits Highcharts(Of GenericDataSerial)

        Public Shared Function ChartType() As chart
            Return New chart With {.type = "areaspline"}
        End Function
    End Class

    Public Class areasplineOptions
        Public Property fillOpacity As Double?
    End Class
End Namespace
