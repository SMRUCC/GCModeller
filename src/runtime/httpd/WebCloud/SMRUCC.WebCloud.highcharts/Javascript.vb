Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module Javascript

    <Extension>
    Public Function WriteJavascript(Of T As Highcharts)(container$, chart As T) As String
        Dim json$ = chart.GetJson(indent:=True)
        Dim javascript$ = $"Highcharts.chart('{container}', {json});"
        Return javascript
    End Function
End Module
