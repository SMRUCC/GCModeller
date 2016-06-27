Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.BLASTOutput.Standard

    Public Class Hit
        <XmlAttribute> Public Property Name As String
        <XmlAttribute> Public Property Length As Integer
        <XmlElement> Public Property Score As LocalBLAST.BLASTOutput.ComponentModel.Score

        <XmlElement> Public Property HitSegments As LocalBLAST.BLASTOutput.ComponentModel.HitSegment()

        Public Overrides Function ToString() As String
            Return String.Format("{0}, (score: {1}, e-value: {2})", Name, Score.Score, Score.Expect)
        End Function

        Public Const HIT_HEADER As String = ".+          Length = \d+"
        Public Const SEQUENCE_LINE_NUMBER As Integer = 3

        Public Function Grep(method As TextGrepMethod) As Integer
            If Not Name.IsNullOrEmpty Then
                Name = method(Name)
                '    Name = Name.Split(vbCrLf).First.Split(vbCrLf).First.Split(vbCr).First
            Else
                Name = "Unknown"
            End If
            Return 0
        End Function

        Friend Shared Function TryParse(Text As String) As Hit
            Dim HitHeader As String = Regex.Match(Text, Standard.Hit.HIT_HEADER, RegexOptions.Singleline).Value
            Dim Tokens As String() = HitHeader.Split(Chr(10))
            Dim Length As Integer = Val(Regex.Match(Tokens.Last, "\d+").Value)
            Dim NameBuilder As StringBuilder = New StringBuilder(128)

            Const Scan0 = 0

            For i As Integer = 0 To Tokens.Count - 2
                Call NameBuilder.Append(Tokens(i))
            Next

            Call NameBuilder.Replace(vbCrLf, " ")
            Call NameBuilder.Replace(vbCr, " ")
            Call NameBuilder.Replace(vbLf, " ")

            Text = Mid(Text, Len(HitHeader) + 5)
            Tokens = Text.Split(Chr(10))

            Dim Hit As Hit = New Hit With {.Length = Length, .Name = NameBuilder.ToString}
            Hit.Score = LocalBLAST.BLASTOutput.ComponentModel.Score.TryParse(Of LocalBLAST.BLASTOutput.ComponentModel.Score)(Text)

            Dim HitSeqes As List(Of LocalBLAST.BLASTOutput.ComponentModel.HitSegment) = New List(Of LocalBLAST.BLASTOutput.ComponentModel.HitSegment)
            Tokens = Tokens.Skip(3).ToArray

            Dim IdxList = (From s As String In Tokens Where InStr(s, "Query:") Select Array.IndexOf(Tokens, s)).ToArray
            Dim ChunkBuffer(Standard.Hit.SEQUENCE_LINE_NUMBER - 1) As String
            For Each Index As Integer In IdxList
                Call Array.ConstrainedCopy(Tokens, Index, ChunkBuffer, Scan0, Standard.Hit.SEQUENCE_LINE_NUMBER)
                Call HitSeqes.Add(LocalBLAST.BLASTOutput.ComponentModel.HitSegment.TryParse(ChunkBuffer))
            Next

            Hit.HitSegments = HitSeqes.ToArray

            Return Hit
        End Function
    End Class

End Namespace