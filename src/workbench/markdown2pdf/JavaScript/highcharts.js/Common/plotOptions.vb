#Region "Microsoft.VisualBasic::0858c73daa87e92ea8c4536b4b2937fd, markdown2pdf\JavaScript\highcharts.js\Common\plotOptions.vb"

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

    ' Class plotOptions
    ' 
    '     Properties: areaspline, bar, column, columnrange, pie
    '                 scatter, series
    ' 
    '     Function: ToString
    ' 
    ' Class seriesOptions
    ' 
    '     Properties: borderWidth, dataLabels, label, pointStart, type
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.WebCloud.JavaScript.highcharts.AreaChart
Imports SMRUCC.WebCloud.JavaScript.highcharts.BarChart
Imports SMRUCC.WebCloud.JavaScript.highcharts.PieChart
Imports SMRUCC.WebCloud.JavaScript.highcharts.ScatterChart

Public Class plotOptions

    Public Property pie As pieOptions
    Public Property series As seriesOptions
    Public Property scatter As scatterOptions
    Public Property columnrange As columnrangeOptions
    Public Property column As columnOptions
    Public Property areaspline As areasplineOptions
    Public Property bar As barOptions

    Public Overrides Function ToString() As String
        If Not pie Is Nothing Then
            Return pie.ToString
        ElseIf Not series Is Nothing Then
            Return series.ToString
        ElseIf Not scatter Is Nothing Then
            Return scatter.ToString
        ElseIf Not columnrange Is Nothing Then
            Return columnrange.ToString
        ElseIf Not column Is Nothing Then
            Return column.ToString
        Else
            Return "null"
        End If
    End Function
End Class

Public Class seriesOptions
    Public Property type As String
    Public Property borderWidth As Double?
    Public Property dataLabels As dataLabels
    Public Property label As labelOptions
    Public Property pointStart As String
End Class
