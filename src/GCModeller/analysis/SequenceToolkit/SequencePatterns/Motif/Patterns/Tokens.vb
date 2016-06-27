Namespace Motif.Patterns

    Public Enum Tokens
        ''' <summary>
        ''' [a-z0-9]，匹配的字符被限定为有限的几个
        ''' </summary>
        QualifyingMatches
        ''' <summary>
        ''' ATGCN
        ''' </summary>
        Residue
        ''' <summary>
        ''' {n}, {m,n}, n
        ''' </summary>
        QualifyingNumber
        ''' <summary>
        ''' (....)
        ''' </summary>
        Fragment
        ''' <summary>
        ''' x={a,b}
        ''' </summary>
        Expression
    End Enum
End Namespace