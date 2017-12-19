Imports SMRUCC.WebCloud.highcharts.LineChart

Namespace BarChart

    Public Class barOptions
        Public Property dataLabels As dataLabels
    End Class

    Public Class columnrangeSerial
        Public Property name As String
        Public Property data As Double()()
    End Class

    Public Class columnrangeOptions
        Public Property dataLabels As dataLabels
    End Class

    Public Class columnOptions
        Public Property borderRadius As Double?
        Public Property depth As Integer?
        Public Property stacking As String
        Public Property pointPadding As Double?
        Public Property groupPadding As Double?
        Public Property borderWidth As Double?
    End Class

    Public Class BarChart : Inherits Highcharts(Of GenericDataSerial)

        Public Overrides Function ToString() As String
            Return title.ToString
        End Function
    End Class

    Public Class VariWideBarChart : Inherits Highcharts(Of serial)

        Public Shared Function variwide() As chart
            Return New chart With {.type = "variwide"}
        End Function
    End Class
End Namespace
