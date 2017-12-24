#Region "Microsoft.VisualBasic::1066563994912613f37f94b5d07eca01, ..\httpd\WebCloud\SMRUCC.WebCloud.highcharts\Charts\BarChart.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.WebCloud.highcharts.LineChart

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

        Public Shared Function variwide() As chart
            Return New chart With {.type = "variwide"}
        End Function
    End Class
End Namespace

