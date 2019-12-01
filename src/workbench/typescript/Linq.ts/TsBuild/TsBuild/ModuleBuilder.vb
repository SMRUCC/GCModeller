Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

''' <summary>
''' The typescript module definition file to VB.NET module convertor
''' </summary>
Public Class ModuleBuilder

    Dim escape As New Escapes
    Dim buffer As New List(Of Char)

    Private Function getTextCode(text As String) As Pointer(Of Char)
        text = text.SolveStream
        text = text.LineTokens.JoinBy(ASCII.LF)

        Return New Pointer(Of Char)(text)
    End Function

    Public Iterator Function ParseIndex(text As String) As IEnumerable(Of Token)
        Dim code As Pointer(Of Char) = getTextCode(text)
        Dim c As Value(Of Char) = ""
        Dim type As Value(Of TypeScriptTokens) = TypeScriptTokens.undefined

        Do While (c = ++code) <> ASCII.NUL
            If (type = walkChar(c)) <> TypeScriptTokens.undefined AndAlso buffer > 0 Then
                Yield New Token With {
                    .Text = buffer.CharString,
                    .Type = type
                }

                ' clear buffer
                buffer *= 0
            End If
        Loop
    End Function

    Private Function bufferEquals(test As String) As Boolean
        Return buffer.SequenceEqual(test)
    End Function

    Private Function bufferStartWith(test As String) As Boolean
        Return buffer.Take(test.Length).SequenceEqual(test)
    End Function

    Private Function bufferEndWith(test As String) As Boolean
        Return buffer.Skip(buffer.Count - test.Length).SequenceEqual(test)
    End Function

    Private Function walkChar(c As Char) As TypeScriptTokens
        If escape.SingleLineComment Then
            If c = ASCII.LF Then
                ' 单行注释在遇到换行符之后结束
                escape.SingleLineComment = False
                Return TypeScriptTokens.comment
            Else
                buffer += c
            End If
        ElseIf escape.BlockTextComment Then
            buffer += c

            If bufferEndWith("*/") Then
                escape.BlockTextComment = False
                Return TypeScriptTokens.comment
            End If
        Else
            If c = " "c OrElse c = ASCII.LF Then
                ' a string delimiter
                If bufferEndWith(":") Then
                    Return TypeScriptTokens.identifier
                ElseIf bufferEndWith("(") Then
                    Return TypeScriptTokens.functionName
                ElseIf buffer.CharString Like Symbols.Keywords Then
                    Return TypeScriptTokens.keyword
                ElseIf bufferEquals("{") Then
                    Return TypeScriptTokens.openStack
                ElseIf bufferEquals("}") Then
                    Return TypeScriptTokens.closeStack
                Else
                    Return TypeScriptTokens.identifier
                End If
            ElseIf c = "("c Then

            Else
                buffer += c

                If bufferStartWith("//") Then
                    escape.SingleLineComment = True
                ElseIf bufferStartWith("/*") Then
                    escape.BlockTextComment = True
                Else

                End If
            End If
        End If

        Return TypeScriptTokens.undefined
    End Function
End Class

Public Enum TypeScriptTokens
    undefined = 0
    [declare]
    keyword
    [function]
    functionName
    identifier
    typeName
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