Imports System.Xml.Serialization

Public Enum TypeScriptTokens
    undefined = 0
    [declare]
    keyword
    [function]
    functionName
    functionCalls
    identifier
    typeName
    funcType
    comment
    delimiter
    logicalLiteral
    numberLiteral
    constructor
    [operator]
    openStack
    ''' <summary>
    ''' 是一个字符串常量表达式
    ''' </summary>
    [string]
    closeStack
End Enum

<XmlType("token")> Public Class Token

    <XmlAttribute("type")> Public Property type As TypeScriptTokens
    <XmlText> Public Property text As String

    <XmlAttribute> Public Property start As Integer
    <XmlAttribute> Public Property ends As Integer

    Public Overrides Function ToString() As String
        Return $"[{type}] {text}"
    End Function

    Public Shared Operator =(t As Token, type As TypeScriptTokens) As Boolean
        Return t.type = type
    End Operator

    Public Shared Operator <>(t As Token, type As TypeScriptTokens) As Boolean
        Return t.type <> type
    End Operator

    Public Shared Operator <>(t As Token, text As String) As Boolean
        Return t.text <> text
    End Operator

    Public Shared Operator =(t As Token, text As String) As Boolean
        Return t.text = text
    End Operator
End Class

Public Class Escapes

    Public Property SingleLineComment As Boolean
    Public Property BlockTextComment As Boolean
    Public Property StringContent As Boolean

End Class

Public Class JavaScriptEscapes : Inherits Escapes
    Public Property StringStackSymbol As Char
End Class