Namespace PieChart

    Public Class PieChart : Inherits Highcharts(Of serial)

        Public Overrides Function ToString() As String
            Return title.ToString
        End Function
    End Class

    Public Class pieOptions : Inherits seriesoptions
        Public Property allowPointSelect As Boolean
        Public Property cursor As String
        Public Property depth As String
        Public Property dataLabels As dataLabels
        Public Property showInLegend As Boolean
    End Class

    Public Class pieData
        Public Property name As String
        Public Property y As Double
        Public Property sliced As Boolean
        Public Property selected As Boolean
    End Class
End Namespace