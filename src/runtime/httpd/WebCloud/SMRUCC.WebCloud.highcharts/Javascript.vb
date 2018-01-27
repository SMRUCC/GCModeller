#Region "Microsoft.VisualBasic::16dcaeab0a827d50b8714d72412f2074, ..\httpd\WebCloud\SMRUCC.WebCloud.highcharts\Javascript.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.highcharts.PieChart
Imports r = System.Text.RegularExpressions.Regex

Public Module Javascript

    ''' <summary>
    ''' Generates the javascript call for highcharts.js charting from a given data model. 
    ''' </summary>
    ''' <typeparam name="S"></typeparam>
    ''' <param name="container$">The ``id`` attribute of a ``&lt;div>`` html tag.</param>
    ''' <param name="chart">highcharts.js data model.</param>
    ''' <returns></returns>
    <Extension>
    Public Function WriteJavascript(Of S)(container$, chart As Highcharts(Of S)) As String
        Dim knownTypes = {GetType(String), GetType(Double), GetType(pieData), GetType(Date)}
        Dim json$ = chart _
            .GetType _
            .GetObjectJson(chart, indent:=True, knownTypes:=knownTypes) _
            .RemoveJsonNullItems _
            .FixDate
        Dim javascript$ = $"Highcharts.chart('{container}', {json});"
        Return javascript
    End Function

    <Extension>
    Public Function FixDate(json As String) As String
        Dim dates$() = r.Matches(json, "[""]\\/Date\(\d+[+]\d+\)\\/[""]", RegexICSng).ToArray
        Dim sb As New StringBuilder(json)

        For Each d As String In dates
            Dim [date] As Date = d.LoadObject(Of Date)
            Dim UTC$ = $"Date.UTC({[date].Year}, {[date].Month}, {[date].Day})"

            Call sb.Replace(d, UTC)
        Next

        Return sb.ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function CreateDataSequence(obj As Dictionary(Of String, Object)) As Object()
        Return obj _
            .Select(Function(item) {CObj(item.Key), item.Value}) _
            .ToArray
    End Function

    <Extension>
    Public Sub WriteHighchartsHTML(template As StringBuilder, name$, javascript$, div$, Optional style$ = "height: 450px")
        Dim html$ =
            (<p>
                 <div id=<%= div %> style=<%= style %>></div>
                 <script type="text/javascript">
                     <%= javascript %>
                 </script>
             </p>).ToString _
                  .Replace("&lt;", "<") _
                  .Replace("&gt;", ">")

        Call template.Replace(name, html)
    End Sub
End Module

