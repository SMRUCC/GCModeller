' /********************************************************************************/
'
'  Rockhopper —— de novo 转录本集合
'
'  复刻自原始 Rockhopper `Java/DeNovoTranscripts.vb`：
'  候选转录本的集合容器，提供去冗余（包含关系折叠）、按表达量排序与结果写出。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace Assembly

    ''' <summary>
    ''' de novo 转录本集合。
    ''' </summary>
    Public Class DeNovoTranscripts
        Inherits List(Of DeNovoTranscript)

        Public Sub New()
        End Sub

        Public Sub New(transcripts As IEnumerable(Of DeNovoTranscript))
            MyBase.New(transcripts)
        End Sub

        ''' <summary>
        ''' 去冗余：若一条转录本完全包含在另一条更长（且表达量不低太多）的转录本中，则丢弃它。
        ''' </summary>
        ''' <param name="expressionTolerance">表达量容差比例（默认 0.2，即短转录本表达量不超过长转录本的 1.2 倍时视为冗余）。</param>
        Public Function Distinct(Optional expressionTolerance As Double = 0.2) As DeNovoTranscripts
            Dim ordered As List(Of DeNovoTranscript) = Me.OrderByDescending(Function(t) t.Length).ToList()
            Dim kept As New List(Of DeNovoTranscript)()

            For Each candidate As DeNovoTranscript In ordered
                Dim redundant As Boolean = False
                For Each longer As DeNovoTranscript In kept
                    If longer.Sequence.Contains(candidate.Sequence) AndAlso
                       candidate.Expression <= longer.Expression * (1.0 + expressionTolerance) Then
                        redundant = True
                        Exit For
                    End If
                Next
                If Not redundant Then kept.Add(candidate)
            Next

            Return New DeNovoTranscripts(kept)
        End Function

        ''' <summary>
        ''' 按表达量降序排列。
        ''' </summary>
        Public Function ByExpression() As DeNovoTranscripts
            Return New DeNovoTranscripts(Me.OrderByDescending(Function(t) t.Expression))
        End Function

        ''' <summary>
        ''' 输出 transcripts.txt（表头 + 数据行）。
        ''' </summary>
        Public Function Save(path As String) As Boolean
            Using writer As New StreamWriter(path, False, System.Text.Encoding.ASCII)
                writer.WriteLine($"Sequence{vbTab}Length{vbTab}Expression{vbTab}QValue")
                For Each transcript As DeNovoTranscript In Me
                    writer.WriteLine(transcript.ToString())
                Next
            End Using
            Return True
        End Function

        ''' <summary>
        ''' 从 transcripts.txt 读取（跳过表头行）。
        ''' </summary>
        Public Shared Function Load(path As String) As DeNovoTranscripts
            Dim list As New List(Of DeNovoTranscript)()
            Dim lines As String() = File.ReadAllLines(path)
            For i As Integer = 1 To lines.Length - 1
                If String.IsNullOrWhiteSpace(lines(i)) Then Continue For
                Dim tokens As String() = lines(i).Split(ControlChars.Tab)
                If tokens.Length < 3 Then Continue For
                list.Add(New DeNovoTranscript With {
                    .Sequence = tokens(0),
                    .Expression = Val(tokens(2)),
                    .QValue = If(tokens.Length > 3, Val(tokens(3)), 1.0)
                })
            Next
            Return New DeNovoTranscripts(list)
        End Function

    End Class

End Namespace
