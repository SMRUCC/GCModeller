Namespace LineChart

    Public Class lineOptions
        Public Property label As labelOptions
        Public Property pointStart As String
        Public Property stacking As String
        Public Property pointInterval As Boolean?
    End Class

    Public Class LineChart : Inherits Highcharts(Of GenericDataSerial)

    End Class
End Namespace