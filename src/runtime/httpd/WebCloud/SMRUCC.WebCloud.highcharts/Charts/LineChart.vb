Namespace LineChart

    Public Class LineSerial
        Public Property name As String
        Public Property data As Double()
    End Class

    Public Class lineOptions
        Public Property label As labelOptions
        Public Property pointStart As String
    End Class

    Public Class LineChart : Inherits Highcharts(Of LineSerial)

    End Class
End Namespace