Imports Microsoft.VisualBasic.MarkupLanguage.CSS
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Legend

    Public Property style As LegendStyles
    Public Property title As String
    Public Property color As String
    ''' <summary>
    ''' CSS expression, which can be parsing by <see cref="CSSFont"/> 
    ''' </summary>
    ''' <returns></returns>
    Public Property font As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

Public Enum LegendStyles
    Bar
    Circle
    SolidLine
    DashLine
    Diamond
End Enum
