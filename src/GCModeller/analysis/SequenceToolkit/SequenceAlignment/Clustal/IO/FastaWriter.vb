Imports System.IO
Imports System.Text

Namespace Clustal

    ''' <summary>
    ''' 已比对序列的 FASTA 输出
    ''' </summary>
    Public Module FastaWriter

        ''' <summary>
        ''' 输出 FASTA 文本
        ''' </summary>
        ''' <param name="names">序列名称</param>
        ''' <param name="rows">已比对的行</param>
        ''' <param name="lineBreak">每行的字符数，小于等于 0 表示不换行</param>
        Public Function ToText(names As String(), rows As String(), Optional lineBreak As Integer = 60) As String
            Dim sb As New StringBuilder()

            For i As Integer = 0 To rows.Length - 1
                Dim name As String = If(names IsNot Nothing AndAlso i < names.Length AndAlso Not String.IsNullOrEmpty(names(i)),
                                        names(i), $"seq_{i + 1}")

                Call sb.Append(">")
                Call sb.AppendLine(name)
                Call AppendWrapped(sb, rows(i), lineBreak)
            Next

            Return sb.ToString()
        End Function

        Private Sub AppendWrapped(sb As StringBuilder, seq As String, lineBreak As Integer)
            If lineBreak <= 0 OrElse seq.Length <= lineBreak Then
                Call sb.AppendLine(seq)
                Return
            End If

            Dim index As Integer = 0

            While index < seq.Length
                Dim length As Integer = Math.Min(lineBreak, seq.Length - index)
                Call sb.AppendLine(seq.Substring(index, length))
                index += length
            End While
        End Sub

        ''' <summary>
        ''' 保存为 FASTA 文件
        ''' </summary>
        Public Function SaveFile(names As String(), rows As String(), path As String,
                                 Optional lineBreak As Integer = 60) As Boolean

            Dim folder As String = Path.GetDirectoryName(path)

            If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
                Call Directory.CreateDirectory(folder)
            End If

            Call File.WriteAllText(path, ToText(names, rows, lineBreak), Encoding.UTF8)
            Return True
        End Function
    End Module
End Namespace
