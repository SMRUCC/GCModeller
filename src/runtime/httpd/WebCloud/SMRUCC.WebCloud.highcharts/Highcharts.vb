Imports SMRUCC.WebCloud.highcharts.PolarChart

Public MustInherit Class Highcharts(Of T)

    Public Property chart As chart
    Public Property title As title
    Public Property subtitle As title
    Public Property yAxis As Axis
    Public Property xAxis As Axis
    Public Property tooltip As tooltip
    Public Property plotOptions As plotOptions
    Public Property legend As legendOptions
    Public Property series As T()
    Public Property responsiveOptions As responsiveOptions
    Public Property credits As credits

End Class

Public MustInherit Class Highcharts3D(Of T) : Inherits Highcharts(Of T)

    Public Property zAxis As Axis

End Class