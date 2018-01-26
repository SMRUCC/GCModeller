Namespace AreaChart

    Public Class AreaSplineChart : Inherits Highcharts(Of GenericDataSerial)

        Public Shared Function ChartType() As chart
            Return New chart With {.type = "areaspline"}
        End Function
    End Class

    Public Class areasplineOptions
        Public Property fillOpacity As Double?
    End Class
End Namespace
