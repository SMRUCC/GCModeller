Imports System.Text

Namespace Clustal

    ''' <summary>
    ''' Stockholm 1.0 格式输出。
    ''' </summary>
    ''' <remarks>
    ''' 输出的内容可以被项目内既有的 <c>MSA.Tabular.Reader.Parser</c> 直接回读，因此这里只使用
    ''' <c>#=GF</c>/<c>#=GS</c>/<c>#=GC seq_cons</c> 三类标记，不引入 Reader 尚未支持的扩展字段。
    ''' </remarks>
    Public Module StockholmWriter

        ''' <summary>
        ''' 输出 Stockholm 文本
        ''' </summary>
        ''' <param name="names">序列名称</param>
        ''' <param name="rows">已比对的行</param>
        ''' <param name="id">比对家族标识(可选)</param>
        Public Function ToText(names As String(), rows As String(), Optional id As String = Nothing) As String
            If rows Is Nothing OrElse rows.Length = 0 Then
                Return ""
            End If

            Dim sb As New StringBuilder()
            Dim family As String = If(String.IsNullOrEmpty(id), "MSA", id)

            Call sb.AppendLine("# STOCKHOLM 1.0")
            Call sb.AppendLine($"#=GF ID {family}")
            Call sb.AppendLine($"#=GF AC {family}")
            Call sb.AppendLine($"#=GF DE Clustal Omega style multiple sequence alignment")

            For i As Integer = 0 To rows.Length - 1
                Dim name As String = If(names IsNot Nothing AndAlso i < names.Length AndAlso Not String.IsNullOrEmpty(names(i)),
                                        names(i), $"seq_{i + 1}")

                Call sb.AppendLine($"#=GS {name} AC {name}")
            Next

            For i As Integer = 0 To rows.Length - 1
                Dim name As String = If(names IsNot Nothing AndAlso i < names.Length AndAlso Not String.IsNullOrEmpty(names(i)),
                                        names(i), $"seq_{i + 1}")

                Call sb.AppendLine($"{name} {rows(i)}")
            Next

            Dim marks As String = ClustalWriter.Conservation(rows, 0, rows(0).Length)

            Call sb.AppendLine($"#=GC seq_cons {marks}")
            Call sb.AppendLine("//")

            Return sb.ToString()
        End Function
    End Module
End Namespace
