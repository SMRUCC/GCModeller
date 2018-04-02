Imports Microsoft.VisualBasic.Serialization.JSON

Namespace RadarChart

    Public Class AxisValue
        Public Property axis As String
        Public Property value As Double

        Public Overrides Function ToString() As String
            Return $"Dim {axis} = {value}"
        End Function
    End Class

    Public Class RadarData
        Public Property colors As String()
        Public Property names As String()
        Public Property layers As AxisValue()()

        Public Overrides Function ToString() As String
            Return names.GetJson
        End Function
    End Class
End Namespace
