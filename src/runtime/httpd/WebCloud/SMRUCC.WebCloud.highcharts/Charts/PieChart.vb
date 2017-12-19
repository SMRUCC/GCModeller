Namespace PieChart

    Public Class PieChart : Inherits Highcharts(Of serial)

        Public Overrides Function ToString() As String
            Return title.ToString
        End Function
    End Class

    Public Class PieChart3D : Inherits Highcharts3D(Of serial)

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

    Public Class VariablePieSerial : Inherits AbstractSerial(Of VariablePieSerialData)

        Public Property minPointSize As Double?
        Public Property innerSize As String
        Public Property zMin As Double?

    End Class

    Public Class VariablePieSerialData
        Public Property name As String
        Public Property y As Double?
        Public Property z As Double?
    End Class

    Public Class VariablePieChart : Inherits Highcharts(Of VariablePieSerial)

        Public Shared Function ChartType() As chart
            Return New chart With {.type = "variablepie"}
        End Function
    End Class
End Namespace