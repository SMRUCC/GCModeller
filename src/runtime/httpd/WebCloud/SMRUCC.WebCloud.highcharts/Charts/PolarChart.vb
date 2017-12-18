Namespace PolarChart

    Public Class paneOptions
        Public Property startAngle As Double?
        Public Property endAngle As Double?
    End Class

    Public Class PolarChart : Inherits Highcharts(Of GenericDataSerial)

        Public Property pane As paneOptions

        Public Shared Function CreateEmptyChart() As chart
            Return New chart With {.polar = True}
        End Function
    End Class
End Namespace