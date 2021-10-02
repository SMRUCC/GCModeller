#Region "Microsoft.VisualBasic::e8e1993c8f1dcb32570955344edef96e, markdown2pdf\JavaScript\highcharts.js\Charts\ScatterChart.vb"

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

    '     Class scatterOptions
    ' 
    '         Properties: marker, states, tooltip
    ' 
    '     Class marker
    ' 
    '         Properties: radius, states
    ' 
    '     Class states
    ' 
    '         Properties: hover
    ' 
    '     Class effect
    ' 
    '         Properties: enabled, lineColor, marker
    ' 
    '     Class markerOptions
    ' 
    '         Properties: enabled, fillColor, lineColor, lineWidth
    ' 
    '     Class ScatterSerial
    ' 
    '         Properties: color, colorByPoint, data, name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ScatterChart

    Public Class scatterOptions
        Public Property marker As marker
        Public Property states As states
        Public Property tooltip As tooltip
    End Class

    Public Class marker
        Public Property radius As Double
        Public Property states As states
    End Class

    Public Class states
        Public Property hover As effect
    End Class

    Public Class effect
        Public Property enabled As Boolean
        Public Property lineColor As String
        Public Property marker As markerOptions
    End Class

    Public Class markerOptions
        Public Property enabled As Boolean?
        Public Property fillColor As String
        Public Property lineWidth As Double?
        Public Property lineColor As String
    End Class

    Public Class ScatterSerial
        Public Property name As String
        Public Property color As String
        Public Property colorByPoint As Boolean
        Public Property data As Double()()
    End Class
End Namespace
