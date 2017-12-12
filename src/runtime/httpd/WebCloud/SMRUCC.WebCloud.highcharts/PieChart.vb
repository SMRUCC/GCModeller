Public Class PieChart : Inherits Highcharts
    Public Property chart As chart
    Public Property title As title
    Public Property tooltip As tooltip
    Public Property plotOptions As plotOptions
    Public Property series As serial()
End Class

Public Class chart
    Public Property type As String
    Public Property options3d As options3d
End Class

Public Class options3d
    Public Property enabled As Boolean
    Public Property alpha As Double
    Public Property beta As Double
End Class

Public Class title
    Public Property text As String
End Class

Public Class tooltip
    Public Property pointFormat As String
End Class

Public Class plotOptions
    Public Property pie As pie
End Class

Public Class pie
    Public Property allowPointSelect As Boolean
    Public Property cursor As String
    Public Property depth As String
    Public Property dataLabels As dataLabels
End Class

Public Class dataLabels
    Public Property enabled As Boolean
    Public Property format As String
End Class

Public Class serial
    Public Property type As String
    Public Property name As String
    Public Property data
End Class

Public Class data

    ''' <summary>
    ''' + <see cref="Double"/>
    ''' + <see cref="pieData"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property data As Dictionary(Of String, Object)
End Class

Public Class pieData
    Public Property name As String
    Public Property y As Double
    Public Property sliced As Boolean
    Public Property selected As Boolean
End Class