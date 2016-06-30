Namespace Interpreter.ObjectModels

    ''' <summary>
    ''' 将表达式之中的所有词元进行完全展开
    ''' </summary>
    Public Class FullTokenliser

        Protected ReadOnly Tokens As Tokens.Token()

        Public Function GetNormalizedTokens() As Tokens.Token()()()

            Dim Normalized As New List(Of Tokens.Token())
            Dim SetLeft As Boolean = False
            Dim SetOperator As Boolean = False
            Dim SetFunction As Boolean = False
            Dim IsParameter As Boolean = True
            Dim PreDepth As Integer = Tokens.First.DepthLevel

            Dim TempCluster As New List(Of Tokens.Token)

            For Each Token In Tokens

                If Token.DepthLevel <> PreDepth Then

                    '堆栈的深度发生了变化，则新起一行
                    Call Normalized.Add(TempCluster.ToArray)
                    Call TempCluster.Clear()

                    PreDepth = Token.DepthLevel

                End If

                Call TempCluster.Add(Token)
            Next

            Call Normalized.Add(TempCluster.ToArray)

            '将第一集调用的都进行分组

            Dim RootNormalized As New List(Of Tokens.Token()())
            Dim TempChunk As New List(Of Tokens.Token())

            For i As Integer = 0 To Normalized.Count - 1

                Dim Token = Normalized(i)

                If Token.First.DepthLevel <> 0 Then

                    Call TempChunk.Add(Token)

                    Do While Token.First.DepthLevel <> 0 AndAlso i < Normalized.Count - 1

                        i += 1
                        Token = Normalized(i)

                        If Token.First.DepthLevel <> 0 Then

                            Call TempChunk.Add(Token)

                        Else

                            Exit Do
                        End If

                    Loop

                    Call RootNormalized.Add(TempChunk.ToArray)
                    Call TempChunk.Clear()
                End If

                Token = Normalized(i)

                If i = Normalized.Count - 1 AndAlso Token.First.DepthLevel <> 0 Then
                    Continue For
                End If

                For Each RootToken In Token

                    Call RootNormalized.Add(New Tokens.Token()() {New Tokens.Token() {RootToken}})

                Next

            Next

            Return RootNormalized.ToArray

        End Function

        Private Shared Sub NewLine(OprTag As Char, ByRef TempToken As List(Of Char), ByRef Stack As Stack(Of Char), ByRef TokenList As List(Of Tokens.Token))
            If TempToken.IsNullOrEmpty Then
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

            Dim Stack As Stack(Of Char) = New Stack(Of Char) '{   [  "
            Dim TokenList As List(Of Tokens.Token) = New List(Of Tokens.Token)
            Dim TempToken As List(Of Char) = New List(Of Char)
            Dim Prebkt As Char

            For Each ch As Char In Expression

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

                    Call Stack.Push(ch) '断行
                    '进行堆栈调用
                    Call NewLine(ch, TempToken, Stack, TokenList)

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
                        Call NewLine(Prebkt, TempToken, Stack, TokenList)
                        Call Stack.Pop()
                        Continue For

                    End If

                End If



                '空格也会隔断词元
                If ch = " "c Then

                    If Stack.IsNullOrEmpty Then
                        '肯定是新的分隔符
                        Call NewLine(" "c, TempToken, Stack, TokenList)
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

                Else

                    Call TempToken.Add(ch)

                End If

            Next

            '假若解析完毕之后任然有数据，则很明显语法错误
            If Not Stack.IsNullOrEmpty Then
                Throw New SyntaxErrorException($"Syntax error at parsing:  {Expression}
                                                 
                                                 {NameOf(Stack)}:   { String.Join(" -> ", (From ch In Stack Select CStr(ch)).ToArray)}
                                                 {NameOf(TempToken)}:  {New String(TempToken.ToArray)}")
            End If

            Call NewLine(" "c, TempToken, Stack, TokenList) '当连续为根栈的时候，最后一个元素会被留下来，这个时候需要添加

#If DEBUG Then
            Call Console.WriteLine()
            Call Console.WriteLine()
            Call Console.WriteLine()

            For Each Token In TokenList
                Call Console.WriteLine(Token.ToString)
            Next
#End If
            Tokens = TokenList.ToArray
        End Sub

        Public Const BRACKETS As String = "{["
        Public Const PAIRED_BRACKETS As String = "}]"""

        Public Shared Function IsPaired(Left As Char, Right As Char) As Boolean

            Select Case Left
                Case "{"c : Return Right = "}"c
                Case "["c : Return Right = "]"c
                Case """"c : Return Right = """"c

                Case Else

                    Throw New SyntaxErrorException($"The bracket is not paired!  {NameOf(Left)} = { Left },  { NameOf(Right)} = { Right }")

            End Select

        End Function
    End Class
End Namespace