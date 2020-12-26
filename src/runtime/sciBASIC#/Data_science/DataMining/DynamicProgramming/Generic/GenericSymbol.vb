Public Class GenericSymbol(Of T)

    Friend ReadOnly m_equals As Func(Of T, T, Boolean)
    Friend ReadOnly m_similarity As Func(Of T, T, Double)
    Friend ReadOnly m_viewChar As Func(Of T, Char)

    Sub New(equals As Func(Of T, T, Boolean), similarity As Func(Of T, T, Double), toChar As Func(Of T, Char))
        Me.m_equals = equals
        Me.m_similarity = similarity
        Me.m_viewChar = toChar
    End Sub

End Class
