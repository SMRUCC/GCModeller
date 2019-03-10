
Public Class CharSequence

    Public Property Chars As Char()

    Public Overrides Function ToString() As String
        Return CType(Chars, String)
    End Function

    Public Shared Widening Operator CType(strValue As String) As CharSequence
        Return New CharSequence With {.Chars = strValue.ToArray}
    End Operator

    Public Shared Widening Operator CType(charData As Char()) As CharSequence
        Return New CharSequence With {.Chars = charData}
    End Operator

    Public Shared Narrowing Operator CType(cs As CharSequence) As String
        Return CType(cs.Chars, String)
    End Operator

    Public Shared Narrowing Operator CType(cs As CharSequence) As Char()
        Return cs.Chars
    End Operator
End Class
