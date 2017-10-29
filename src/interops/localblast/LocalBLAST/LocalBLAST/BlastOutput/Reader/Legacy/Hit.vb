#Region "Microsoft.VisualBasic::6dc23c8504f64487f6625c66f83c8755, ..\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Legacy\Hit.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
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
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Scripting

Namespace LocalBLAST.BLASTOutput.Legacy

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
            Dim HitHeader As String = Regex.Match(Text, Legacy.Hit.HIT_HEADER, RegexOptions.Singleline).Value
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
            Dim ChunkBuffer(Legacy.Hit.SEQUENCE_LINE_NUMBER - 1) As String
            For Each Index As Integer In IdxList
                Call Array.ConstrainedCopy(Tokens, Index, ChunkBuffer, Scan0, Legacy.Hit.SEQUENCE_LINE_NUMBER)
                Call HitSeqes.Add(LocalBLAST.BLASTOutput.ComponentModel.HitSegment.TryParse(ChunkBuffer))
            Next

            Hit.HitSegments = HitSeqes.ToArray

            Return Hit
        End Function
    End Class

End Namespace
