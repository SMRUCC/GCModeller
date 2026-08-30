' ============================================================================
' Fasta.vb — FASTA 读取器
' ----------------------------------------------------------------------------
' 流式读取 FASTA 文件为 FastaSequence 列表。
' 处理：多行序列、空行、注释行（';' 开头）、大小写混杂。
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text

Namespace MiniBlast.Core

    ''' <summary>单条 FASTA 序列</summary>
    Public Class FastaSequence

        ''' <summary>序列 ID（头行第一个空白前的字段）</summary>
        Public Property Id As String

        ''' <summary>头行完整描述（不含 '>'）</summary>
        Public Property Description As String

        ''' <summary>序列（原始字符，大写化）</summary>
        Public Property Sequence As String

        Public Sub New(id As String, desc As String, seq As String)
            Me.Id = id
            Me.Description = desc
            Me.Sequence = seq
        End Sub

    End Class

    Public Module FastaIO

        ''' <summary>读取 FASTA 文件全部序列</summary>
        Public Function ReadAll(path As String) As List(Of FastaSequence)
            Dim result As New List(Of FastaSequence)()
            Dim id As String = Nothing
            Dim desc As String = Nothing
            Dim sb As New StringBuilder()

            Using reader As New StreamReader(path)
                Dim line As String = reader.ReadLine()
                While line IsNot Nothing
                    Dim trimmed = line.Trim()
                    If trimmed.Length > 0 AndAlso trimmed(0) = ">"c Then
                        ' 遇到新头行：落盘上一条
                        FlushRecord(result, id, desc, sb)
                        Dim head = trimmed.Substring(1).Trim()
                        Dim sp = head.IndexOf(" "c)
                        If sp >= 0 Then
                            id = head.Substring(0, sp)
                            desc = head
                        Else
                            id = head
                            desc = head
                        End If
                    ElseIf trimmed.Length > 0 AndAlso trimmed(0) = ";"c Then
                        ' 注释行：跳过
                    Else
                        sb.Append(trimmed)
                    End If
                    line = reader.ReadLine()
                End While
            End Using
            FlushRecord(result, id, desc, sb)
            Return result
        End Function

        Private Sub FlushRecord(list As List(Of FastaSequence),
                                ByRef id As String, ByRef desc As String,
                                sb As StringBuilder)
            If id IsNot Nothing Then
                list.Add(New FastaSequence(id, desc, sb.ToString().ToUpperInvariant()))
                sb.Clear()
            End If
            id = Nothing
            desc = Nothing
        End Sub

    End Module

End Namespace
