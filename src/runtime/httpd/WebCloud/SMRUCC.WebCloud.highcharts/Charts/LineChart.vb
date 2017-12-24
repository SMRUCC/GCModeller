#Region "Microsoft.VisualBasic::e336e6766d32af2d6318065fca27aee3, ..\httpd\WebCloud\SMRUCC.WebCloud.highcharts\Charts\LineChart.vb"

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

Namespace LineChart

    Public Class lineOptions
        Public Property label As labelOptions
        Public Property pointStart As String
        Public Property stacking As String
        Public Property pointInterval As Boolean?
    End Class

    Public Class LineChart : Inherits Highcharts(Of GenericDataSerial)

    End Class
End Namespace
