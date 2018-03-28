
Imports Newtonsoft.Json

Public Class Axis
    Public Property type As String
    Public Property allowDecimals As Boolean?
    Public Property className As String
    Public Property opposite As Boolean?
    Public Property title As title
    Public Property min As Double?
    Public Property max As Double?
    Public Property labels As labelOptions
    Public Property categories As String()
    Public Property startOnTick As Boolean?
    Public Property endOnTick As Boolean?
    Public Property showLastLabel As Boolean?
    Public Property gridLineWidth As Boolean?
    Public Property showFirstLabel As Boolean?
    Public Property crosshair As Boolean?
    ''' <summary>
    ''' 逻辑值或者一个实数
    ''' </summary>
    ''' <returns></returns>
    Public Property tickInterval As Object
    Public Property plotBands As Band()
    Public Property dateTimeLabelFormats As dateTimeLabelFormats
End Class

Public Class dateTimeLabelFormats
    Public Property month As String
    Public Property year As String
End Class

Public Class Band
    Public Property from As Double?
    Public Property [to] As Double?
    Public Property color As String
End Class

Public Class legendOptions

    ''' <summary>
    ''' 是否允许图注。
    ''' </summary>
    ''' <returns></returns>
    Public Property enabled As Boolean?
    Public Property layout As String
    Public Property align As String
    Public Property verticalAlign As String
    Public Property x As Double?
    Public Property y As Double?
    Public Property floating As Boolean?
    Public Property borderWidth As Double?
    Public Property backgroundColor As String
    Public Property shadow As Boolean?
    Public Property reversed As Boolean?
End Class

Public Class title
    Public Property text As String
    Public Property align As String
    Public Property enable As Boolean?
    Public Property skew3d As Boolean?

    Public Overrides Function ToString() As String
        Return text
    End Function

    Public Shared Widening Operator CType(title As String) As title
        Return New title With {.text = title}
    End Operator
End Class

Public Class tooltip
    Public Property headerFormat As String
    Public Property pointFormat As String
    Public Property valueSuffix As String
    Public Property footerFormat As String
    Public Property [shared] As Boolean?
    Public Property useHTML As Boolean?
End Class

<JsonConverter(GetType(LambdaWriter))>
Public Class labelOptions
    Public Property connectorAllowed As Boolean?
    Public Property overflow As String
    Public Property skew3d As Boolean?
    Public Property style As styleOptions
    Public Property formatter As Lambda
End Class

Public Class styleOptions
    Public Property fontSize As String
    Public Property color As String
End Class

Public Class dataLabels
    Public Property enabled As Boolean?
    Public Property format As String
    Public Property style As styleOptions
End Class

Public Class responsiveOptions
    Public Property rules As rule()
End Class

Public Class rule
    Public Property condition As ruleConditions
    Public Property chartOptions As chartOptions
End Class

Public Class ruleConditions
    Public Property maxWidth As Double?
End Class

Public Class chartOptions
    Public Property legend As legendOptions
End Class

Public Class credits
    ''' <summary>
    ''' 是否允许显示版权信息。
    ''' </summary>
    ''' <returns></returns>
    Public Property enabled As Boolean?
    ''' <summary>
    ''' 版权所有的链接。
    ''' </summary>
    ''' <returns></returns>
    Public Property href As String
    ''' <summary>
    ''' 版权信息显示文字。
    ''' </summary>
    ''' <returns></returns>
    Public Property text As String
End Class
