Imports System.Text

Namespace Clustal

    ''' <summary>
    ''' PHYLIP 顺序格式输出(名称截断到 10 个字符并保证唯一)
    ''' </summary>
    Public Module PhylipWriter

        ''' <summary>
        ''' PHYLIP 名称列宽
        ''' </summary>
        Public Const NameWidth As Integer = 10

        ''' <summary>
        ''' 输出 PHYLIP 文本
        ''' </summary>
        Public Function ToText(names As String(), rows As String()) As String
            If rows Is Nothing OrElse rows.Length = 0 Then
                Return ""
            End If

            Dim sb As New StringBuilder()
            Dim labels As String() = UniqueLabels(names, rows.Length)

            Call sb.AppendLine($" {rows.Length} {rows(0).Length}")

            For i As Integer = 0 To rows.Length - 1
                Call sb.Append(labels(i).PadRight(NameWidth))
                Call sb.AppendLine(rows(i))
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 生成截断且唯一的 10 字符名称
        ''' </summary>
        Private Function UniqueLabels(names As String(), count As Integer) As String()
            Dim labels As String() = New String(count - 1) {}
            Dim used As New HashSet(Of String)()

            For i As Integer = 0 To count - 1
                Dim name As String = If(names IsNot Nothing AndAlso i < names.Length AndAlso Not String.IsNullOrEmpty(names(i)),
                                        names(i), $"seq_{i + 1}")
                Dim candidate As String = Normalize(name)

                If used.Contains(candidate) Then
                    Dim suffix As Integer = 1

                    While used.Contains(Truncate(candidate, suffix.ToString()))
                        suffix += 1
                    End While

                    candidate = Truncate(candidate, suffix.ToString())
                End If

                Call used.Add(candidate)

                labels(i) = candidate
            Next

            Return labels
        End Function

        Private Function Normalize(name As String) As String
            Dim sb As New StringBuilder(NameWidth)

            For Each ch As Char In name
                If sb.Length >= NameWidth Then
                    Exit For
                End If

                If Char.IsWhiteSpace(ch) OrElse Char.IsControl(ch) Then
                    Call sb.Append("_"c)
                Else
                    Call sb.Append(ch)
                End If
            Next

            Return sb.ToString()
        End Function

        Private Function Truncate(name As String, suffix As String) As String
            Dim head As String = name

            If head.Length + suffix.Length > NameWidth Then
                head = head.Substring(0, Math.Max(0, NameWidth - suffix.Length))
            End If

            Return head & suffix
        End Function
    End Module
End Namespace
