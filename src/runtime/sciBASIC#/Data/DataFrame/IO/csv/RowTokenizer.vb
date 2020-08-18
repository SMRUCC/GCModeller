Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser

Namespace IO

    Friend Class RowTokenizer

        ReadOnly rowStr As CharPtr

        Dim buf As New CharBuffer
        ''' <summary>
        ''' 解析器是否是处于由双引号所产生的栈之中？
        ''' </summary>
        Dim openStack As Boolean = False

        Sub New(chars As String)
            rowStr = chars
        End Sub

        Sub New(ptr As CharPtr)
            rowStr = ptr
        End Sub

        Public Iterator Function GetTokens(Optional delimiter As Char = ","c, Optional quot As Char = ASCII.Quot) As IEnumerable(Of String)
            Dim doubleQuot$ = quot & quot

            buf *= 0

            Do While rowStr
                If walkChar(++rowStr, delimiter, quot) Then
                    Yield buf.PopAllChars.CharString.Replace(doubleQuot, quot)
                End If
            Loop

            If buf > 0 Then
                Yield buf.ToString.Replace(doubleQuot, quot)
            End If
        End Function

        Private Function walkChar(c As Char, delimiter As Char, quot As Char) As Boolean
            If openStack Then

                If c = quot Then

                    ' \" 会被转义为单个字符 "
                    If buf.StartEscaping Then
                        Call buf.Pop()
                        Call buf.Add(c)
                    Else
                        ' 查看下一个字符是否为分隔符
                        ' 因为前面的 Dim c As Char = +buffer 已经位移了，所以在这里直接取当前的字符
                        Dim peek = rowStr.Current
                        ' 也有可能是 "" 转义 为单个 "
                        Dim lastQuot = (buf > 0 AndAlso buf.Last = quot)

                        If buf = 0 AndAlso peek = delimiter Then

                            ' openStack意味着前面已经出现一个 " 了
                            ' 这里又出现了一个 " 并且下一个字符为分隔符
                            ' 则说明是 "", 当前的cell内容是一个空字符串
                            rowStr.MoveNext()
                            openStack = False
                            Return True

                        ElseIf (peek = delimiter OrElse rowStr.EndRead) AndAlso lastQuot Then

                            ' 下一个字符为分隔符，则结束这个token
                            ' 跳过下一个分隔符，因为已经在这里判断过了
                            rowStr.MoveNext()
                            openStack = False
                            Return True

                        Else
                            ' 不是，则继续添加
                            buf += c
                        End If
                    End If
                Else
                    ' 由于双引号而产生的转义                   
                    buf += c
                End If
            Else
                If buf = 0 AndAlso c = quot Then
                    ' token的第一个字符串为双引号，则开始转义
                    openStack = True
                Else
                    If c = delimiter Then
                        Return True
                    Else
                        buf += c
                    End If
                End If
            End If

            Return False
        End Function
    End Class
End Namespace