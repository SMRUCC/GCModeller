#Region "Microsoft.VisualBasic::b1583f5f9bf056f5f0af68bec96c7ba8, markdown2pdf\JavaScript\highcharts.js\Charts\PolarChart.vb"

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

    '     Class paneOptions
    ' 
    '         Properties: endAngle, size, startAngle
    ' 
    '     Class PolarChart
    ' 
    '         Properties: pane
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace PolarChart

    Public Class paneOptions
        Public Property startAngle As Double?
        Public Property endAngle As Double?
        Public Property size As String
    End Class

    ''' <summary>
    ''' 雷达图
    ''' </summary>
    Public Class PolarChart : Inherits Highcharts(Of GenericDataSerial)

        Public Property pane As paneOptions

        Sub New()
            Call MyBase.New
            Call MyBase.reference.Add("https://code.highcharts.com/highcharts-more.js")
        End Sub

    End Class
End Namespace
