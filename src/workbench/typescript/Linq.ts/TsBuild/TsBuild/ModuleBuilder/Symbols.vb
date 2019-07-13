Public Enum TypeScriptTokens
    undefined = 0
    [declare]
    keyword
    [function]
    functionName
    identifier
    typeName
    funcType
    comment
    constructor
    openStack
    closeStack
End Enum

Public Class Token

    Public Property Text As String
    Public Property Type As TypeScriptTokens

    Public Overrides Function ToString() As String
        Return $"[{Type}] {Text}"
    End Function

End Class

Public Class Escapes

    Public Property SingleLineComment As Boolean
    Public Property BlockTextComment As Boolean

End Class