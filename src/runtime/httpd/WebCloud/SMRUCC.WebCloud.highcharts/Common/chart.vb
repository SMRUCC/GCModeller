Imports System.Runtime.CompilerServices
Imports SMRUCC.WebCloud.highcharts.LineChart
Imports SMRUCC.WebCloud.highcharts.PieChart

Public Class chart

    Public Property type As String
    Public Property options3d As options3d

    Public Overrides Function ToString() As String
        If options3d Is Nothing OrElse Not options3d.enabled Then
            Return type
        Else
            Return $"[3D] {type}"
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function PieChart3D() As chart
        Return New chart With {
            .type = "pie",
            .options3d = New options3d With {
                .enabled = True,
                .alpha = 45,
                .beta = 0
            }
        }
    End Function
End Class

Public Class Axis
    Public Property title As title
End Class

Public Class options3d
    Public Property enabled As Boolean
    Public Property alpha As Double
    Public Property beta As Double

    Public Overrides Function ToString() As String
        If enabled Then
            Return NameOf(enabled)
        Else
            Return $"Not {NameOf(enabled)}"
        End If
    End Function
End Class

Public Class legendOptions
    Public Property layout As String
    Public Property align As String
    Public Property verticalAlign As String
End Class

Public Class title
    Public Property text As String

    Public Overrides Function ToString() As String
        Return text
    End Function
End Class

Public Class tooltip
    Public Property pointFormat As String
End Class

Public Class labelOptions
    Public Property connectorAllowed As Boolean
End Class

Public Class plotOptions
    Public Property pie As pieOptions
    Public Property series As seriesOptions
End Class

Public Class dataLabels
    Public Property enabled As Boolean
    Public Property format As String
End Class

Public Class serial
    Public Property type As String
    Public Property name As String
    ''' <summary>
    ''' + <see cref="Double"/>
    ''' + <see cref="pieData"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property data As Object()
End Class

Public Class pieData
    Public Property name As String
    Public Property y As Double
    Public Property sliced As Boolean
    Public Property selected As Boolean
End Class

Public Class responsiveOptions
    Public Property rules As rule()
End Class

Public Class rule
    Public Property condition As ruleConditions
    Public Property chartOptions As chartOptions
End Class

Public Class ruleConditions
    Public Property maxWidth As Double
End Class

Public Class chartOptions
    Public Property legend As legendOptions
End Class