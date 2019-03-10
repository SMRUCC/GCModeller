Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

#Const DEBUG = 0

Namespace Interpreter.Parser.TextTokenliser

    ''' <summary>
    ''' Tokenliser working in multiline string literal mode.
    ''' </summary>
    Public Class MSLTokens : Inherits Parserbase

        Public ReadOnly Property Comments As String

        ''' <summary>
        ''' 当前的整行代码是否为注释行
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsCommentLine As Boolean
            Get
                Return Tokens.IsNullOrEmpty AndAlso Not String.IsNullOrEmpty(Comments)
            End Get
        End Property

        ''' <summary>
        ''' 空白行
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsBlank As Boolean
            Get
                Return String.IsNullOrEmpty(Comments) AndAlso Tokens.IsNullOrEmpty
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="OprTag">分隔符的类型</param>
        ''' <param name="TempToken"></param>
        ''' <param name="Stack"></param>
        ''' <param name="TokenList"></param>
        Private Shared Sub __newLine(OprTag As Char, ByRef TempToken As List(Of Char), ByRef Stack As Stack(Of Char), ByRef TokenList As List(Of Tokens.Token))
            If TempToken.IsNullOrEmpty Then
                If Not OprTag = Nothing Then Call TempToken.Add(OprTag)
                Return
            End If

            If Not Stack.IsNullOrEmpty Then
                If Not OprTag = Nothing Then Call TempToken.Add(OprTag)
                Return
            End If

            Dim s_Token As String = New String(TempToken.ToArray)

            Call TokenList.Add(New Tokens.Token(Stack.Count, s_Token) With {.OprTag = OprTag})
            Call TempToken.Clear()
        End Sub

        Public Overrides ReadOnly Property Tokens As Token()
            Get
                Return (From Token In _Tokens Where Not Token.IsNullOrSpace Select Token).ToArray
            End Get
        End Property

        ''' <summary>
        ''' 表达式是否已经解析完毕了
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FinishYet As Boolean
            Get
                Return Stack.IsNullOrEmpty
            End Get
        End Property

        Dim _Tokens As List(Of Tokens.Token) = New List(Of Tokens.Token)
        Dim Stack As Stack(Of Char) = New Stack(Of Char) '{   [  "
        Dim TempToken As List(Of Char) = New List(Of Char) '空白或者右边的分隔符之后所出现的#都是注释的开始的标志
        Dim Prebkt As Char
        Dim ExprBuilder As New StringBuilder(1024)

        Public ReadOnly Property MSLExpression As String
            Get
                Return ExprBuilder.ToString
            End Get
        End Property

        Dim PreSpacer As Boolean = True

        ''' <summary>
        ''' 将表达式解析为词元，每一个词元都不含有前导或者后置的空白符号
        ''' 断词元的条件是Stack必须为空
        ''' </summary>
        ''' <param name="Expression">在调用之前请先试用Trim函数进行处理</param>
        Public Function Parsing(Expression As String) As MSLTokens
            Call ExprBuilder.AppendLine(Expression)

            If String.IsNullOrEmpty(Expression) Then
                Return Me
            ElseIf Len(Expression) > 0 AndAlso Expression.First = "#"c

                If Tokens.IsNullOrEmpty Then
                    '这一行是注释行
                    _Comments = Expression
                    Return Me
                End If
            End If

            For p_i As Integer = 0 To Len(Expression) - 1

                Dim TrimEscape As Boolean = False
                Dim ch As Char = Expression(p_i)
#If DEBUG Then
                Call Debug.Write(ch)
                Call Trace.Write(ch)
                Call Console.Write(ch)
#End If

#Region "BRACKETS.Contains(ch)"

                If BRACKETS.Contains(ch) Then

                    If Not IsEscaped() Then
                        '假若栈空间之中的最后一个元素为双引号，则说明现在在读取字符串，则这个符号不会作为分隔符来堆栈
                        If Not Stack.IsNullOrEmpty Then
                            Prebkt = Stack.Peek
                            If Prebkt = """"c Then
                                '字符串的值，则不可以进行堆栈
                                Call TempToken.Add(ch)
                                PreSpacer = False
                                Continue For
                            End If
                        End If

                        '进行堆栈调用
                        If PreSpacer Then
                            Call __newLine(ch, TempToken, Stack, _Tokens)
                            Call Stack.Push(ch) '断行
                            PreSpacer = False
                        Else
                            TempToken.Add(ch)
                        End If

                        Continue For
                    Else
                        TrimEscape = True
                    End If
                End If
#End Region

#Region "PAIRED_BRACKETS.Contains(ch)"

                If PAIRED_BRACKETS.Contains(ch) Then

                    If Not IsEscaped() Then
                        If ch = """"c Then

                            If Stack.IsNullOrEmpty Then
                                '新的字符串的起始标识符
                                Call Stack.Push(ch)
                                PreSpacer = False
                                Continue For
                            End If

                            Prebkt = Stack.Peek

                            If IsPaired(Prebkt, ch) Then

                                '得到了一个字符串，字符串是和前面的词元是同等级的，只有当出现新的分隔符之后才会增加堆栈深度
                                Call Stack.Pop()
                                Call __newLine(ch, TempToken, Stack, _Tokens)
                                PreSpacer = False
                                Continue For

                            Else

                                '可能是新的字符串的开始位置

                                If ch = """"c Then '新的字符串的起始
                                    Call TempToken.Add(ch)
                                End If

                                Call Stack.Push(ch)
                                PreSpacer = False
                                Continue For

                            End If

                        End If

                        If Stack.IsNullOrEmpty Then         ' 遇到了结束符号但是栈是空的，则是语法错误 ？？？？
                            Call TempToken.Add(ch)
                            PreSpacer = False
                            Continue For
                            'Throw New SyntaxErrorException(Expression)
                        Else
                            Prebkt = Stack.Peek
                        End If

                        If IsPaired(Prebkt, ch) Then '得到了一个匹配的括号，则还需要分情况来进行讨论，主要原因是字符串之中也会包含有这些符号

                            '字符串的双引号在栈空间之中只有一个
                            Call __newLine(ch, TempToken, Stack, _Tokens)
                            Call Stack.Pop()
                            PreSpacer = False
                            Continue For

                        End If

                    Else
                        TrimEscape = True
                    End If

                End If

#End Region

                If ch = " "c OrElse ch = vbTab Then     '空格也会隔断词元

                    If Stack.IsNullOrEmpty Then
                        '肯定是新的分隔符
                        Call __newLine(Nothing, TempToken, Stack, _Tokens)
                        PreSpacer = True

                        Continue For

                        'Else

                        '    '前面的栈空间不为空，则说明这个空格可能是内部表达式之中的一部分
                        '    Call TempToken.Add(ch)
                        '    Continue For

                    End If

                    '但是假设前面没有双引号
                    Prebkt = Stack.Peek

                    If Prebkt = """"c Then

                        '这个空格是字符串值之中的一部分，则不可以用作分隔符
                        Call TempToken.Add(ch)

                    Else

                        '这个空格不是字符串值之中的一部分，则用作为分隔符
                        Call __newLine(" "c, TempToken, Stack, _Tokens)

                    End If
                    PreSpacer = False

                    Continue For
                End If

#Region "Comments"

                If ParserCommon.COMMENTS.IndexOf(ch) > -1 Then  '注释符 1. 前面是空格，并且没有出现在字符串之中，即栈空间之中没有双引号
                    ' 2.  栈空间必须为空

                    If Stack.IsNullOrEmpty Then  '空的栈空间，则这个符号开始后面全都是注释信息，则不要了，只需要前面的就可以了
                        Call __newLine("", TempToken, Stack, _Tokens)
                        _Comments = Mid(Expression, p_i + 1)

                        Exit For
                    End If

                    '栈空间不为空，则这个#号可能是字符串之中的符号

                End If
#End Region

                If TrimEscape Then
                    Call TempToken.RemoveAt(TempToken.Count - 1)
                End If

                If ch = ","c AndAlso Not _Tokens.Count = 0 Then

                    If _Tokens.Last.OprTag = """"c Then
                        Call _Tokens.Last.Append(",")
                    Else
                        Call TempToken.Add(ch)
                    End If

                Else
                    Call TempToken.Add(ch)
                End If

                PreSpacer = False
            Next

            Call __newLine("", TempToken, Stack, _Tokens) '当连续为根栈的时候，最后一个元素会被留下来，这个时候需要添加

            If Not FinishYet AndAlso TempToken.Last = ","c Then
                Call TempToken.Add(" "c)
            Else
                ' 添加换行符以方便下一步的解析工作
                Call TempToken.Add(vbCrLf)
            End If

            Return Me
        End Function

        Private Function IsEscaped() As Boolean
            Return Not TempToken.IsNullOrEmpty AndAlso TempToken.Last = "\"
        End Function
    End Class
End Namespace