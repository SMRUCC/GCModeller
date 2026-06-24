' ============================================================================
' Module 1: Genome Annotation & Feature Extraction
' File: GenomeAnnotation/FastaParser.vb
'
' 功能: 解析蛋白质 FASTA 文件，提取蛋白质序列。
'       对应论文中 "基因组注释与特征化模块" 的氨基酸序列读取部分。
' ============================================================================

Imports System.IO
Imports System.Collections.Generic
Imports System.Text

Namespace Traitar.GenomeAnnotation

    ''' <summary>
    ''' 表示一条蛋白质序列
    ''' </summary>
    Public Class ProteinSequence
        ''' <summary>序列标识符（> 后第一个 token）</summary>
        Public Property Id As String
        ''' <summary>完整的 header 行（去掉 &gt;）</summary>
        Public Property Header As String
        ''' <summary>氨基酸序列</summary>
        Public Property Sequence As String

        Public Overrides Function ToString() As String
            Return $"[{Id}] len={Sequence.Length}"
        End Function
    End Class

    ''' <summary>
    ''' FASTA 文件解析器（支持蛋白质和核酸序列）
    ''' </summary>
    Public Class FastaParser

        ''' <summary>
        ''' 解析 FASTA 文件，返回所有序列
        ''' </summary>
        Public Shared Function Parse(filePath As String) As List(Of ProteinSequence)
            Dim seqs As New List(Of ProteinSequence)
            Dim current As ProteinSequence = Nothing
            Dim seqBuilder As New StringBuilder()

            For Each line As String In File.ReadAllLines(filePath)
                If String.IsNullOrEmpty(line) Then Continue For

                If line.StartsWith(">") Then
                    ' 保存前一条序列
                    If current IsNot Nothing Then
                        current.Sequence = seqBuilder.ToString()
                        seqs.Add(current)
                        seqBuilder.Clear()
                    End If
                    ' 开始新序列
                    Dim header = line.Substring(1).Trim()
                    Dim id = header
                    Dim spaceIdx = header.IndexOfAny({" "c, ControlChars.Tab})
                    If spaceIdx > 0 Then id = header.Substring(0, spaceIdx)
                    current = New ProteinSequence With {
                        .Header = header,
                        .Id = id
                    }
                Else
                    seqBuilder.Append(line.Trim())
                End If
            Next

            ' 保存最后一条
            If current IsNot Nothing Then
                current.Sequence = seqBuilder.ToString()
                seqs.Add(current)
            End If

            Return seqs
        End Function

        ''' <summary>
        ''' 将蛋白质序列写入 FASTA 文件（每行 60 个字符）
        ''' </summary>
        Public Shared Sub Write(seqs As IEnumerable(Of ProteinSequence), filePath As String)
            Using writer As New StreamWriter(filePath)
                For Each seq In seqs
                    writer.WriteLine(">" & seq.Header)
                    Dim s = seq.Sequence
                    Dim i = 0
                    While i < s.Length
                        Dim len = Math.Min(60, s.Length - i)
                        writer.WriteLine(s.Substring(i, len))
                        i += len
                    End While
                Next
            End Using
        End Sub
    End Class
End Namespace
