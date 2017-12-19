Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.highcharts.PieChart

Public MustInherit Class AbstractSerial(Of T)

    Public Property type As String
    Public Property name As String

    Public Overridable Property data As T()

    Public Overrides Function ToString() As String
        Return $"Dim {name} As {type} = {data.GetJson}"
    End Function
End Class

Public Class GenericDataSerial : Inherits AbstractSerial(Of Double)

End Class

''' <summary>
''' Object array
''' </summary>
Public Class serial : Inherits AbstractSerial(Of Object)

    ''' <summary>
    ''' + <see cref="Double"/>
    ''' + <see cref="pieData"/>
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Property data As Object()
    Public Property dataLabels As dataLabels
    Public Property tooltip As tooltip
    Public Property colorByPoint As Boolean?
End Class