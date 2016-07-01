#Region "Microsoft.VisualBasic::08b0feac97b2abfa51ba7a806099cead, ..\interops\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.2.26\Query.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Partial Class _2_2_26

        Public Overrides Function CheckIntegrity(QuerySource As SequenceModel.FASTA.FastaFile) As Boolean
            Throw New NotImplementedException
        End Function

        Public Class Query

            <XmlElement> Public Query, Subject As Item
            <XmlAttribute> Public EffectiveSearchSpace As Long

            <XmlElement> Public p, Gapped As LocalBLAST.BLASTOutput.ComponentModel.Parameter
            <XmlArray> Public Hits As Segment()

            Public Overrides Function ToString() As String
                If Hits Is Nothing OrElse Hits.Count = 0 Then
                    Return LocalBLAST.BLASTOutput.Standard.Query.HITS_NOT_FOUND
                Else
                    Return String.Format("{0} <--> {1} ({2} hit segments.)", Query.ToString, Subject.ToString, Hits.Count)
                End If
            End Function

            Public Class Segment
                <XmlAttribute> Public Score, Score2, Identities, Positives, Gaps As Double
                <XmlAttribute> Public Expect, Method As String

                <XmlElement> Public Property Hsp As LocalBLAST.BLASTOutput.ComponentModel.HitSegment()

                Public Shared Function TryParse(Text As String) As Segment()
                    If InStr(Text, LocalBLAST.BLASTOutput.Standard.Query.HITS_NOT_FOUND) > 0 Then
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
                    Dim ChunkBuffer(LocalBLAST.BLASTOutput.Standard.Hit.SEQUENCE_LINE_NUMBER - 1) As String
                    For Each Index As Integer In IdxList
                        Call Array.ConstrainedCopy(Tokens, Index, ChunkBuffer, 0, LocalBLAST.BLASTOutput.Standard.Hit.SEQUENCE_LINE_NUMBER)
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

            Public Class Item
                <XmlAttribute> Public Property Name As String
                <XmlAttribute> Public Property Length As Long

                Public Const QUERY_HEADER As String = "Query= .+?Length[=]\d+"
                Public Const SUBJECT_HEADER As String = "Subject= .+?Length[=]\d+"

                Public Overrides Function ToString() As String
                    Return String.Format("Name: {0}, Length: {1}", Name, Length)
                End Function
            End Class

            Public Shared Function TryParse(Text As String) As Query
                Dim Query As Query = New Query With {
                    .Query = ParseHead(Text, _2_2_26.Query.Item.QUERY_HEADER, "Query= "),
                    .Subject = ParseHead(Text, _2_2_26.Query.Item.SUBJECT_HEADER, "Subject= ")
                }
                Dim TEMP = LocalBLAST.BLASTOutput.ComponentModel.Parameter.TryParseBlastPlusParameters(Text)
                Query.p = TEMP(0)
                Query.Gapped = TEMP(1)
                Query.Hits = Segment.TryParse(Text)

                Return Query
            End Function

            Private Shared Function ParseHead(Text As String, Regx As String, Replaced As String) As Query.Item
                Dim Head As String = Regex.Match(Text, Regx, RegexOptions.Singleline).Value
                Dim Tokens = Head.Split(Chr(10))
                Dim NameBuilder As StringBuilder = New StringBuilder(128)
                Dim Length As Long = Tokens.Last.Match("\d+").RegexParseDouble
                For i As Integer = 0 To Tokens.Count - 2
                    Call NameBuilder.Append(Tokens(i))
                Next
                Call NameBuilder.Replace(Replaced, "")

                Return New Query.Item With {.Name = Trim(NameBuilder.ToString), .Length = Length}
            End Function
        End Class
    End Class
End Namespace
