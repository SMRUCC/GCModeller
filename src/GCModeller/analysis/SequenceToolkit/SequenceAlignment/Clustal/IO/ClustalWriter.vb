Imports System.Text

Namespace Clustal

    ''' <summary>
    ''' Clustal(.aln)格式输出：分块显示，每块末尾附一行保守性标记。
    ''' </summary>
    Public Module ClustalWriter

        ''' <summary>
        ''' 每块的列数
        ''' </summary>
        Public Const BlockWidth As Integer = 60
        ''' <summary>
        ''' 名称列的最小宽度
        ''' </summary>
        Public Const MinNameWidth As Integer = 10

        ''' <summary>
        ''' 输出 Clustal 格式文本
        ''' </summary>
        Public Function ToText(names As String(), rows As String(), Optional title As String = "CLUSTAL O(1.2.4) multiple sequence alignment") As String
            If rows Is Nothing OrElse rows.Length = 0 Then
                Return ""
            End If

            Dim sb As New StringBuilder()
            Dim width As Integer = NameWidth(names)
            Dim columns As Integer = rows(0).Length

            Call sb.AppendLine(title)
            Call sb.AppendLine()

            Dim start As Integer = 0

            While start < columns
                Dim length As Integer = Math.Min(BlockWidth, columns - start)

                For i As Integer = 0 To rows.Length - 1
                    Dim name As String = If(names IsNot Nothing AndAlso i < names.Length AndAlso Not String.IsNullOrEmpty(names(i)),
                                            names(i), $"seq_{i + 1}")

                    Call sb.Append(name.PadRight(width))
                    Call sb.Append(rows(i).Substring(start, Math.Min(length, rows(i).Length - start)))
                    Call sb.AppendLine()
                Next

                Call sb.Append(New String(" "c, width))
                Call sb.AppendLine(Conservation(rows, start, length))
                Call sb.AppendLine()

                start += length
            End While

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 计算名称列的宽度
        ''' </summary>
        Public Function NameWidth(names As String()) As Integer
            Dim width As Integer = MinNameWidth

            If names IsNot Nothing Then
                For Each name As String In names
                    If Not String.IsNullOrEmpty(name) AndAlso name.Length + 2 > width Then
                        width = Math.Min(40, name.Length + 2)
                    End If
                Next
            End If

            Return width
        End Function

        ''' <summary>
        ''' 生成一段列的保守性标记：完全一致的列标 <c>*</c>，其余为空格(与 clustalo 一致)
        ''' </summary>
        Public Function Conservation(rows As String(), start As Integer, length As Integer) As String
            Dim marks As New StringBuilder(length)

            For offset As Integer = 0 To length - 1
                Dim column As Integer = start + offset
                Dim ch As Char = ChrW(0)
                Dim same As Boolean = True

                For r As Integer = 0 To rows.Length - 1
                    Dim row As String = rows(r)
                    Dim value As Char = If(column < row.Length, row(column), "-"c)

                    If r = 0 Then
                        ch = value
                    ElseIf value <> ch Then
                        same = False
                        Exit For
                    End If
                Next

                If same AndAlso ch <> "-"c AndAlso ch <> ChrW(0) Then
                    Call marks.Append("*"c)
                Else
                    Call marks.Append(" "c)
                End If
            Next

            Return marks.ToString()
        End Function
    End Module
End Namespace
