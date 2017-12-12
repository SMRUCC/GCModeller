Imports SMRUCC.WebCloud.highcharts.BarChart
Imports SMRUCC.WebCloud.highcharts.LineChart
Imports SMRUCC.WebCloud.highcharts.PieChart
Imports SMRUCC.WebCloud.highcharts.ScatterChart

Public Class plotOptions

    Public Property pie As pieOptions
    Public Property series As lineOptions
    Public Property scatter As scatterOptions
    Public Property columnrange As columnrangeOptions
    Public Property column As columnOptions

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
End Class
