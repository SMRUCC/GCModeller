Namespace PieChart

    Public Class PieChart : Inherits Highcharts(Of serial)

        Public Overrides Function ToString() As String
            Return title.ToString
        End Function
    End Class

    Public Class pieOptions
        Public Property allowPointSelect As Boolean
        Public Property cursor As String
        Public Property depth As String
        Public Property dataLabels As dataLabels
    End Class
End Namespace