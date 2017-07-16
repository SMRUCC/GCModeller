Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace LocalBLAST.BLASTOutput.BlastPlus.v226

    Public Class Segment

        <XmlAttribute> Public Score, Score2, Identities, Positives, Gaps As Double
        <XmlAttribute> Public Expect, Method As String

        <XmlElement> Public Property Hsp As LocalBLAST.BLASTOutput.ComponentModel.HitSegment()

        Public Shared Function TryParse(Text As String) As Segment()
            If InStr(Text, Legacy.Query.HITS_NOT_FOUND) > 0 Then
                Return Nothing
            End If

            Dim Tokens = Strings.Split(Text, " Score = ").Skip(1)
            Dim LQuery = From Line As String In Tokens
                         Let Segment As Segment = TryParse2(Line)
                         Select Segment
                         Order By Segment.Hsp.First.Query.Left Ascending '
            Return LQuery.ToArray
        End Function

        Public Const HIT_SCORE2 As String = " bits \(\d+\)"
        Public Const HIT_EXPECT As String = "Expect = [^,]+"
        Public Const HIT_IDENTITIES As String = "Identities = \d+/\d+ \(\d+%\)"
        Public Const HIT_POSITIVES As String = "Positives = \d+/\d+ \(\d+%\)"
        Public Const HIT_GAPS As String = "Gaps = \d+/\d+ \(\d+%\)"
        Public Const HIT_METHOD As String = "Method: [^.]+"

        Private Shared Function TryParse2(Text As String) As Segment
            Dim Segment As Segment = New Segment With {.Score = Val(Text), .Score2 = Text.Match(HIT_SCORE2).Match("\d+").RegexParseDouble}
            Dim Tokens = Strings.Split(Text, vbCrLf)
            Segment.Expect = Tokens(0).Match(HIT_EXPECT).Split.Last
            Segment.Method = Tokens(0).Match(HIT_METHOD).Split(CChar(":")).Last.Trim
            Segment.Identities = Tokens(1).Match(HIT_IDENTITIES).Match("\d+[%]").RegexParseDouble
            Segment.Positives = Tokens(1).Match(HIT_POSITIVES).Match("\d+[%]").RegexParseDouble
            Segment.Gaps = Tokens(1).Match(HIT_GAPS).Match("\d+[%]").RegexParseDouble

            Dim HitSeqes As List(Of LocalBLAST.BLASTOutput.ComponentModel.HitSegment) = New List(Of LocalBLAST.BLASTOutput.ComponentModel.HitSegment)
            Tokens = Tokens.Skip(3).ToArray

            Dim IdxList = (From s As String In Tokens Where InStr(s, "Query ") Select Array.IndexOf(Tokens, s)).ToArray
            Dim ChunkBuffer(Legacy.Hit.SEQUENCE_LINE_NUMBER - 1) As String
            For Each Index As Integer In IdxList
                Call Array.ConstrainedCopy(Tokens, Index, ChunkBuffer, 0, Legacy.Hit.SEQUENCE_LINE_NUMBER)
                Call HitSeqes.Add(TryParse(ChunkBuffer))
            Next

            Segment.Hsp = HitSeqes.ToArray

            Return Segment
        End Function

        Private Shared Function TryParse(TextLines As String()) As LocalBLAST.BLASTOutput.ComponentModel.HitSegment
            Dim Hitseq As LocalBLAST.BLASTOutput.ComponentModel.HitSegment = New ComponentModel.HitSegment
            Hitseq.Query = ParseSegment(TextLines(ComponentModel.HitSegment.QUERY_INDEX))
            Hitseq.Sbjct = ParseSegment(TextLines(ComponentModel.HitSegment.SUBJECT_INDEX))
            Hitseq.Consensus = Mid(TextLines(ComponentModel.HitSegment.CONSERVED_INDEX), 13, 60)

            Return Hitseq
        End Function

        Private Shared Function ParseSegment(Text As String) As ComponentModel.Segment
            Dim Tokens = (From token As String In Text.Split
                          Where Not String.IsNullOrEmpty(token)
                          Select token).ToArray
            Dim Reference As ComponentModel.Segment =
                New ComponentModel.Segment With {
                    .Left = Val(Tokens(1)),
                    .SequenceData = Tokens(2),
                    .Right = Val(Tokens(3))
            }
            Return Reference
        End Function
    End Class
End Namespace