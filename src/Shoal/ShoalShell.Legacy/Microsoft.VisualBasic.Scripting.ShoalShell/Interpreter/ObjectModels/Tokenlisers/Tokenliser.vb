Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.FullTokenliser

Namespace Interpreter.ObjectModels

    ''' <summary>
    ''' 保留有内部表达式，只展开一级表达式
    ''' </summary>
    Public Class Tokenliser

        Public ReadOnly Property Tokens As Tokens.Token()
        Public ReadOnly Property Comments As String

        Public ReadOnly Property IsCommentLine As Boolean
            Get
                Return Tokens.IsNullOrEmpty AndAlso Not String.IsNullOrEmpty(Comments)
            End Get
        End Property

        Private Shared Sub NewLine(OprTag As Char, ByRef TempToken As List(Of Char), ByRef Stack As Stack(Of Char), ByRef TokenList As List(Of Tokens.Token))
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

        ''' <summary>
        ''' 将表达式解析为词元，每一个词元都不含有前导或者后置的空白符号
        ''' 断词元的条件是Stack必须为空
        ''' </summary>
        ''' <param name="Expression"></param>
        Sub New(Expression As String)

            Expression = Trim(Expression)
            If InStr(Expression, "Call ", CompareMethod.Text) = 1 Then
                Expression = Mid(Expression, 6).Trim
            ElseIf Len(Expression) > 0 AndAlso Expression.First = "#"c
                '这一行是注释行
                Tokens = New Tokens.Token() {}
                Comments = Expression

                Return
            End If

            Dim Stack As Stack(Of Char) = New Stack(Of Char) '{   [  "
            Dim TokenList As List(Of Tokens.Token) = New List(Of Tokens.Token)
            Dim TempToken As List(Of Char) = New List(Of Char) '空白或者右边的分隔符之后所出现的#都是注释的开始的标志
            Dim Prebkt As Char

            For p_i As Integer = 0 To Len(Expression) - 1

                Dim ch As Char = Expression(p_i)

                Call Debug.Write(ch)
                Call Trace.Write(ch)
#If DEBUG Then
                Call Console.Write(ch)
#End If

                If BRACKETS.Contains(ch) Then

                    '假若栈空间之中的最后一个元素为双引号，则说明现在在读取字符串，则这个符号不会作为分隔符来堆栈
                    If Not Stack.IsNullOrEmpty Then
                        Prebkt = Stack.Peek
                        If Prebkt = """"c Then
                            '字符串的值，则不可以进行堆栈
                            Call TempToken.Add(ch)
                            Continue For
                        End If
                    End If

                    '进行堆栈调用
                    Call NewLine(ch, TempToken, Stack, TokenList)
                    Call Stack.Push(ch) '断行

                    Continue For
                End If

                If PAIRED_BRACKETS.Contains(ch) Then

                    If ch = """"c Then

                        If Stack.IsNullOrEmpty Then
                            '新的字符串的起始标识符
                            Call Stack.Push(ch)
                            Continue For
                        End If

                        Prebkt = Stack.Peek

                        If IsPaired(Prebkt, ch) Then

                            '得到了一个字符串，字符串是和前面的词元是同等级的，只有当出现新的分隔符之后才会增加堆栈深度
                            Call Stack.Pop()
                            Call NewLine(ch, TempToken, Stack, TokenList)
                            Continue For

                        Else

                            '可能是新的字符串的开始位置
                            Call Stack.Push(ch)
                            Continue For

                        End If

                    End If

                    If Stack.IsNullOrEmpty Then
                        '遇到了结束符号但是栈是空的，则是语法错误
                        Throw New SyntaxErrorException(Expression)
                    Else
                        Prebkt = Stack.Peek
                    End If

                    If IsPaired(Prebkt, ch) Then '得到了一个匹配的括号，则还需要分情况来进行讨论，主要原因是字符串之中也会包含有这些符号

                        '字符串的双引号在栈空间之中只有一个
                        Call NewLine(ch, TempToken, Stack, TokenList)
                        Call Stack.Pop()
                        Continue For

                    End If

                End If



                '空格也会隔断词元
                If ch = " "c Then

                    If Stack.IsNullOrEmpty Then
                        '肯定是新的分隔符
                        Call NewLine(Nothing, TempToken, Stack, TokenList)
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
                        Call NewLine(" "c, TempToken, Stack, TokenList)

                    End If

                    Continue For
                End If


                If ch = "#"c Then  '注释符 1. 前面是空格，并且没有出现在字符串之中，即栈空间之中没有双引号
                    ' 2.  栈空间必须为空

                    If Stack.IsNullOrEmpty Then  '空的栈空间，则这个符号开始后面全都是注释信息，则不要了，只需要前面的就可以了
                        Call NewLine("", TempToken, Stack, TokenList)
                        Comments = Mid(Expression, p_i + 1)

                        Exit For
                    End If

                    '栈空间不为空，则这个#号可能是字符串之中的符号

                End If

                Call TempToken.Add(ch)
            Next

            '假若解析完毕之后任然有数据，则很明显语法错误
            If Not Stack.IsNullOrEmpty Then
                Throw New SyntaxErrorException($"Syntax error at parsing:  {Expression}
                                                 
                                                 {NameOf(Stack)}:   { String.Join(" -> ", (From ch In Stack Select CStr(ch)).ToArray)}
                                                 {NameOf(TempToken)}:  {New String(TempToken.ToArray)}")
            End If

            Call NewLine("", TempToken, Stack, TokenList) '当连续为根栈的时候，最后一个元素会被留下来，这个时候需要添加

#If DEBUG Then
            Call Console.WriteLine()
            Call Console.WriteLine()
            Call Console.WriteLine()

            For Each Token In TokenList
                Call Console.WriteLine(Token.ToString)
            Next
#End If
            Tokens = (From Token In TokenList.ToArray Where Not Token.IsNullOrSpace Select Token).ToArray
        End Sub

    End Class
End Namespace