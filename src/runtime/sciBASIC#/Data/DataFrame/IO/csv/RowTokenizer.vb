Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser

Namespace IO

    Friend Class RowTokenizer

        ReadOnly rowStr As CharPtr

        Dim buf As New CharBuffer

        Sub New(chars As String)
            rowStr = chars
        End Sub

        Sub New(ptr As CharPtr)
            rowStr = ptr
        End Sub

        Public Iterator Function GetTokens(Optional delimiter As Char = ","c, Optional quot As Char = ASCII.Quot) As IEnumerable(Of String)
            buf *= 0

            Do While rowStr
                If walkChar(++rowStr, delimiter, quot) Then
                    Yield buf.PopAllChars.CharString
                End If
            Loop

            If buf > 0 Then
                Yield buf.ToString
            End If
        End Function

        Private Function walkChar(c As Char, delimiter As Char, quot As Char) As Boolean
            ' row data 
            Dim tokens As New List(Of String)
            Dim temp As New List(Of Char)
            ' 解析器是否是处于由双引号所产生的栈之中？
            Dim openStack As Boolean = False
            Dim buffer As New CharPtr(s)
            Dim doubleQuot$ = quot & quot

            Do While Not buffer.EndRead
                Dim c As Char = ++buffer

                If openStack Then

                    If c = quot Then

                        ' \" 会被转义为单个字符 "
                        If temp.StartEscaping Then
                            Call temp.RemoveLast
                            Call temp.Add(c)
                        Else
                            ' 查看下一个字符是否为分隔符
                            ' 因为前面的 Dim c As Char = +buffer 已经位移了，所以在这里直接取当前的字符
                            Dim peek = buffer.Current
                            ' 也有可能是 "" 转义 为单个 "
                            Dim lastQuot = (temp > 0 AndAlso temp.Last = quot)

                            If temp = 0 AndAlso peek = delimiter Then

                                ' openStack意味着前面已经出现一个 " 了
                                ' 这里又出现了一个 " 并且下一个字符为分隔符
                                ' 则说明是 "", 当前的cell内容是一个空字符串
                                tokens += ""
                                temp *= 0
                                buffer += 1
                                openStack = False

                            ElseIf (peek = delimiter OrElse buffer.EndRead) AndAlso lastQuot Then

                                ' 下一个字符为分隔符，则结束这个token
                                tokens += New String(temp).Replace(doubleQuot, quot)
                                temp *= 0
                                ' 跳过下一个分隔符，因为已经在这里判断过了
                                buffer += 1
                                openStack = False

                            Else
                                ' 不是，则继续添加
                                temp += c
                            End If
                        End If
                    Else
                        ' 由于双引号而产生的转义                   
                        temp += c
                    End If
                Else
                    If temp.Count = 0 AndAlso c = quot Then
                        ' token的第一个字符串为双引号，则开始转义
                        openStack = True
                    Else
                        If c = delimiter Then
                            tokens += New String(temp).Replace(doubleQuot, quot)
                            temp *= 0
                        Else
                            temp += c
                        End If
                    End If
                End If
            Loop

            If temp.Count > 0 Then
                tokens += New String(temp).Replace(doubleQuot, quot)
            End If

            Return tokens
        End Function
    End Class
End Namespace