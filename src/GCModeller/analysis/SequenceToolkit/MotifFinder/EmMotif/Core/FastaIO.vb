' ============================================================================
' FastaIO.vb — FASTA 读取（多行序列、CRLF、空行容忍、小写归一）
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text

Namespace EmMotif.Core

    Public Class FastaRecord

        Public Id As String
        Public Description As String
        Public Seq As String

    End Class

    Public Module FastaIO

        ''' <summary>解析 FASTA；返回记录列表（保留顺序）</summary>
        Public Function Read(path As String) As List(Of FastaRecord)
            Dim records As New List(Of FastaRecord)()
            Dim cur As FastaRecord = Nothing
            Dim sb As New StringBuilder()

            For Each raw In File.ReadLines(path)
                Dim line = raw.TrimEnd(Convert.ToChar(13), Convert.ToChar(10))
                If line.StartsWith(">"c) Then
                    If cur IsNot Nothing Then
                        cur.Seq = sb.ToString()
                        records.Add(cur)
                    End If
                    Dim header = line.Substring(1).Trim()
                    Dim sp = header.IndexOf(" "c)
                    If sp < 0 Then
                        cur = New FastaRecord With {.Id = If(header.Length > 0, header, $"seq_{records.Count + 1}"),
                                                    .Description = ""}
                    Else
                        cur = New FastaRecord With {.Id = header.Substring(0, sp),
                                                    .Description = header.Substring(sp + 1).Trim()}
                    End If
                    sb.Clear()
                ElseIf line.StartsWith(";"c) Then
                    Continue For          ' 旧式注释行
                ElseIf line.Length > 0 Then
                    sb.Append(line.Trim())
                End If
            Next
            If cur IsNot Nothing Then
                cur.Seq = sb.ToString()
                records.Add(cur)
            End If
            If records.Count = 0 Then Throw New InvalidDataException("FASTA 无记录: " & path)
            Return records
        End Function

        ''' <summary>写入 FASTA（测试数据用）</summary>
        Public Sub Write(path As String, records As IEnumerable(Of FastaRecord), lineWrap As Int32)
            Dim sb As New StringBuilder()
            For Each r In records
                sb.Append(">"c).Append(r.Id)
                If r.Description IsNot Nothing AndAlso r.Description.Length > 0 Then
                    sb.Append(" "c).Append(r.Description)
                End If
                sb.AppendLine()
                For i = 0 To r.Seq.Length - 1 Step lineWrap
                    Dim len = Math.Min(lineWrap, r.Seq.Length - i)
                    sb.AppendLine(r.Seq.Substring(i, len))
                Next
            Next
            File.WriteAllText(path, sb.ToString())
        End Sub

    End Module

End Namespace
