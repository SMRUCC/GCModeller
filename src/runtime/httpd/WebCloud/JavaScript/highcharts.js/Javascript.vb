#Region "Microsoft.VisualBasic::10d8a118f1be5af7d2dda536ed1bff0b, WebCloud\JavaScript\highcharts.js\Javascript.vb"

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

    ' Module Javascript
    ' 
    '     Function: CreateDataSequence, FixDate, GetHtmlViewer, GetHtmlViews, NewtonsoftJsonWriter
    '               RemovesEmptyLine, RemoveTrailingComma, WriteJavascript
    ' 
    '     Sub: WriteHighchartsHTML
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml
Imports Newtonsoft.Json
Imports SMRUCC.WebCloud.JavaScript.highcharts.PieChart
Imports r = System.Text.RegularExpressions.Regex

''' <summary>
''' The highcharts.js helper
''' </summary>
Public Module Javascript

    ''' <summary>
    ''' 在这里输出的日期格式都被统一为``\/Date(1198908717056)\/``.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function NewtonsoftJsonWriter(Of T)(obj As T) As String
        Return JsonConvert.SerializeObject(
            obj,
            Formatting.Indented,
            settings:=New JsonSerializerSettings With {
                .DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            }
        )
    End Function

    ''' <summary>
    ''' Generates the javascript call for highcharts.js charting from a given data model. 
    ''' </summary>
    ''' <typeparam name="S"></typeparam>
    ''' <param name="container$">The ``id`` attribute of a ``&lt;div>`` html tag.</param>
    ''' <param name="chart">highcharts.js data model.</param>
    ''' <returns></returns>
    <Extension>
    Public Function WriteJavascript(Of S)(container$, chart As Highcharts(Of S), Optional UTCdate As Boolean = True) As String
        Dim knownTypes As Type() = {
            GetType(String),
            GetType(Double),
            GetType(pieData),
            GetType(Date)
        }
        'Dim json$ = chart _
        '    .GetType _
        '    .GetObjectJson(chart, indent:=True, knownTypes:=knownTypes) _
        '    .RemoveJsonNullItems _
        '    .FixDate
        Dim JSON$ = chart _
            .NewtonsoftJsonWriter _
            .FixDate(UTCdate) _
            .RemoveJsonNullItems _
            .RemoveTrailingComma _
            .RemovesEmptyLine
        Dim javascript$ = $"Highcharts.chart('{container}', {LambdaWriter.StripLambda(JSON)});"
        Return javascript
    End Function

    Const MicrosoftDatePattern$ = "[""]\\/Date\(\d+(.\d+)?\)\\/[""]"

    <Extension>
    Public Function FixDate(json$, UTCdate As Boolean) As String
        If Not UTCdate Then
            Return json
        Else
            Dim dates$() = r.Matches(json, MicrosoftDatePattern, RegexICSng).ToArray
            Dim sb As New StringBuilder(json)

            For Each d As String In dates
                Dim [date] As Date = d.LoadJSON(Of Date)
                Dim UTC$ = $"Date.UTC({[date].Year}, {[date].Month}, {[date].Day})"

                Call sb.Replace(d, UTC)
            Next

            Return sb.ToString
        End If
    End Function

    <Extension>
    Public Function RemovesEmptyLine(str As String) As String
        Return r.Replace(str, "(((\r)|(\n)){2,}\s*)+", vbCrLf, RegexICMul)
    End Function

    <Extension>
    Public Function RemoveTrailingComma(json As String) As String
        Dim trim As New StringBuilder(json)

        For Each match As String In json.Matches(",\s*\]", RegexICSng)
            Call trim.Replace(match, vbCrLf & "]")
        Next
        For Each match As String In json.Matches(",\s*}", RegexICSng)
            Call trim.Replace(match, vbCrLf & "}")
        Next

        Return trim.ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function CreateDataSequence(obj As Dictionary(Of String, Object)) As Object()
        Return obj _
            .Select(Function(item) {CObj(item.Key), item.Value}) _
            .ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub WriteHighchartsHTML(template As StringBuilder, name$, javascript$, div$, Optional style$ = "height: 450px;")
        Call template.Replace(name, javascript.GetHtmlViewer(div, style))
    End Sub

    <Extension>
    Public Function GetHtmlViewer(javascript$, div$, Optional style$ = "width:100%; height: 450px;", Optional class$ = "") As String
        Return sprintf(
            <p>
                <div id=<%= div %> class=<%= [class] %> style=<%= style %>></div>
                <script type="text/javascript">
                    %s
                </script>
            </p>, javascript)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function GetHtmlViews(Of T)(chart As Highcharts(Of T), div$, Optional style$ = "width:100%; height: 450px;", Optional class$ = "") As String
        Return div.WriteJavascript(chart).GetHtmlViewer(div, style, [class])
    End Function
End Module
