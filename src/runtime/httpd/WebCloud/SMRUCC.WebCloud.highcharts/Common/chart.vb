Imports System.Runtime.CompilerServices
Imports SMRUCC.WebCloud.highcharts.PieChart

Public Class chart

    Public Property type As String
    Public Property options3d As options3d
    Public Property zoomType As String
    Public Property inverted As Boolean
    Public Property renderTo As String
    Public Property margin As Double

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
    Public Property className As String
    Public Property opposite As Boolean
    Public Property title As title
    Public Property min As Double
    Public Property max As Double
    Public Property labels As labelOptions
    Public Property categories As String()
    Public Property startOnTick As Boolean
    Public Property endOnTick As Boolean
    Public Property showLastLabel As Boolean
    Public Property gridLineWidth As Boolean
    Public Property showFirstLabel As Boolean
End Class

Public Class legendOptions
    Public Property layout As String
    Public Property align As String
    Public Property verticalAlign As String
    Public Property x As Double
    Public Property y As Double
    Public Property floating As Boolean
    Public Property borderWidth As Double
    Public Property backgroundColor As String
    Public Property shadow As Boolean
End Class

Public Class title
    Public Property text As String
    Public Property align As String
    Public Property enable As Boolean

    Public Overrides Function ToString() As String
        Return text
    End Function
End Class

Public Class tooltip
    Public Property headerFormat As String
    Public Property pointFormat As String
    Public Property valueSuffix As String
End Class

Public Class labelOptions
    Public Property connectorAllowed As Boolean
    Public Property overflow As String
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

Public Class credits
    Public Property enabled As Boolean
End Class