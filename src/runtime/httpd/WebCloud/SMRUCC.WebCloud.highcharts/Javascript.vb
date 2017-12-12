Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.highcharts.PieChart

Public Module Javascript

    <Extension>
    Public Function WriteJavascript(Of S)(container$, chart As Highcharts(Of S)) As String
        Dim knownTypes = {GetType(String), GetType(Double), GetType(pieData)}
        Dim json$ = chart _
            .GetType _
            .GetObjectJson(chart, indent:=True, knownTypes:=knownTypes)
        Dim javascript$ = $"Highcharts.chart('{container}', {json});"
        Return javascript
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function CreateDataSequence(obj As Dictionary(Of String, Object)) As Object()
        Return obj _
            .Select(Function(item) {CObj(item.Key), item.Value}) _
            .ToArray
    End Function
End Module
