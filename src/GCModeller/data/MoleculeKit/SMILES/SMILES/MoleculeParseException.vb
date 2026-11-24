''' <summary>
''' 结构式字符串解析失败时抛出的异常，携带出错位置以便定位到具体字符。
''' </summary>
Public Class MoleculeParseException
    Inherits Exception

    ''' <summary>
    ''' 出错字符在原始字符串中的位置（0 基）。
    ''' </summary>
    Public ReadOnly Property Position As Integer

    ''' <summary>
    ''' 解析失败时正在处理的原始字符串。
    ''' </summary>
    Public ReadOnly Property Input As String

    ''' <summary>
    ''' 以出错位置与说明构造异常。
    ''' </summary>
    ''' <param name="message">错误说明。</param>
    ''' <param name="position">出错位置（0 基下标）。</param>
    ''' <param name="input">原始字符串。</param>
    Public Sub New(message As String, position As Integer, input As String)
        MyBase.New($"{message} (at position {position + 1} of '{Truncate(input)}')")
        Me.Position = position
        Me.Input = input
    End Sub

    Private Shared Function Truncate(text As String) As String
        If String.IsNullOrEmpty(text) Then Return ""
        If text.Length <= 80 Then Return text

        Return text.Substring(0, 77) & "..."
    End Function

End Class
