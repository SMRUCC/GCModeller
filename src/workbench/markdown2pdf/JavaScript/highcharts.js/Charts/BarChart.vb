#Region "Microsoft.VisualBasic::b54abc960d96dd71a030fda1a4f091e5, markdown2pdf\JavaScript\highcharts.js\Charts\BarChart.vb"

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

    '     Class barOptions
    ' 
    '         Properties: dataLabels
    ' 
    '     Class columnrangeSerial
    ' 
    '         Properties: data, name
    ' 
    '     Class columnrangeOptions
    ' 
    '         Properties: dataLabels
    ' 
    '     Class columnOptions
    ' 
    '         Properties: borderRadius, borderWidth, depth, groupPadding, pointPadding
    '                     stacking
    ' 
    '     Class BarChart
    ' 
    '         Function: ToString
    ' 
    '     Class VariWideBarChart
    ' 
    ' 
    ' 
    '     Class ColumnChart
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace BarChart

    Public Class barOptions
        Public Property dataLabels As dataLabels
    End Class

    Public Class columnrangeSerial
        Public Property name As String
        Public Property data As Double()()
    End Class

    Public Class columnrangeOptions
        Public Property dataLabels As dataLabels
    End Class

    Public Class columnOptions
        Public Property borderRadius As Double?
        Public Property depth As Integer?
        Public Property stacking As String
        Public Property pointPadding As Double?
        Public Property groupPadding As Double?
        Public Property borderWidth As Double?
    End Class

    Public Class BarChart : Inherits Highcharts(Of GenericDataSerial)

        Public Overrides Function ToString() As String
            Return title.ToString
        End Function
    End Class

    Public Class VariWideBarChart : Inherits Highcharts(Of serial)
    End Class

    Public Class ColumnChart : Inherits Highcharts(Of serial)

    End Class
End Namespace
