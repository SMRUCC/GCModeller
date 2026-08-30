Namespace Core


    ''' <summary>延伸阶段产出的原始 HSP（未做统计换算）</summary>
    Public Class RawHsp

        Public Property QueryFrom As Integer      ' 0-based inclusive
        Public Property QueryTo As Integer
        Public Property SubjectFrom As Integer
        Public Property SubjectTo As Integer
        Public Property RawScore As Double
        Public Property QueryAlign As String
        Public Property SubjectAlign As String
        Public Property Midline As String
        Public Property Identities As Integer
        Public Property Positives As Integer
        Public Property Gaps As Integer

    End Class
End Namespace