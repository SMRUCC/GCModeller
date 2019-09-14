#Region "Microsoft.VisualBasic::fbf96f2f99b2f201cd0db89c977604ab, LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\Models\SubjectHit.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class SubjectHit
    ' 
    '         Properties: Hsp, Length, LengthHit, LengthQuery, Name
    '                     QueryLocation, Score, SubjectLocation
    ' 
    '         Function: GetItems, GetLengthHit, GetLengthQuery, ParseHitSegments, ToString
    '                   TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel
Imports r = System.Text.RegularExpressions.Regex

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Class SubjectHit

        <XmlAttribute> Public Property Name As String
        ''' <summary>
        ''' hit蛋白质序列的全长
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Length As Long
        <XmlElement> Public Property Score As Score

        Public Overridable Property Hsp As HitSegment()

        ''' <summary>
        ''' Hit position on the query sequence.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property QueryLocation As Location
            Get
                Dim left As Integer = If(Hsp.IsNullOrEmpty, 0, Hsp.First.Query.Left)
                Dim right As Integer = If(Hsp.IsNullOrEmpty, 0, Hsp.Last.Query.Right)

                Return New Location(left, right)
            End Get
        End Property

        Public Overridable ReadOnly Property SubjectLocation As Location
            Get
                Dim left As Integer = If(Hsp.IsNullOrEmpty, 0, Hsp.First.Subject.Left)
                Dim right As Integer = If(Hsp.IsNullOrEmpty, 0, Hsp.Last.Subject.Right)

                Return New Location(left, right)
            End Get
        End Property

        ''' <summary>
        ''' 高分区的hit片段的长度
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property LengthHit As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return GetLengthHit(Hsp, Score)
            End Get
        End Property

        Public Overridable ReadOnly Property LengthQuery As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return GetLengthQuery(Hsp, Score)
            End Get
        End Property

        Public Shared Function GetLengthQuery(hsp As HitSegment(), score As Score) As Integer
            Dim LQuery = LinqAPI.Exec(Of Integer)() _
 _
                <= From Segment As HitSegment
                   In hsp
                   Select From ch As Char
                          In Segment.Subject.SequenceData
                          Where ch = "-"c
                          Select 1

            Dim value As Integer = LQuery.Sum
            Dim lengthQuery = score.Gaps.Denominator - value

            Return lengthQuery
        End Function

        Public Shared Function GetLengthHit(hsp As HitSegment(), score As Score) As Integer
            Dim LQuery = LinqAPI.Exec(Of Integer)() _
 _
                <= From Segment As HitSegment
                   In hsp
                   Select From ch As Char
                          In Segment.Query.SequenceData
                          Where ch = "-"c
                          Select 1

            Dim value As Integer = LQuery.Sum
            ' 减去插入的空格就是比对上的长度了
            Dim lengthHit As Integer = score.Gaps.Denominator - value

            Return lengthHit
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("Name: {0}, Length: {1}", Name, Length)
        End Function

        Public Const NO_HITS_FOUND As String = "No hits found"

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        ''' <remarks>请不要在这里使用.AsParallel拓展方法，以保持各个片段的顺序关系</remarks>
        Public Shared Function GetItems(text As String) As SubjectHit()
            If InStr(text, NO_HITS_FOUND) > 0 Then
                Return New SubjectHit() {}
            End If

            Dim tokens = r.Split(text, "^>", RegexOptions.Multiline) _
                          .Skip(1) _
                          .ToArray
            Dim LQuery As SubjectHit() = tokens _
                .Select(AddressOf SubjectHit.TryParse) _
                .IteratesALL _
                .ToArray
            Return LQuery
        End Function

        Protected Const PAIRWISE$ = "Query\s+\d+\s+.+?\s+\d+.+?Sbjct\s+\d+\s+.+?\s+\d+"

        ''' <summary>
        ''' 一个subject可能会出现不止一种的最佳比对结果
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        Public Shared Iterator Function TryParse(text As String) As IEnumerable(Of SubjectHit)
            Dim name$ = Strings.Split(text, "Length=").First.TrimNewLine
            Dim l As Long = CLng(text.Match("Length=\d+").RegexParseDouble)

            ' 上面解析完了subject的基本信息之后，在这里解析多个比对结果的情况
            Dim parts = Strings.Split(text, "Score =") _
                               .Select(Function(p) "Score =" & p) _
                               .ToArray

            ' 第一部分是subject的基本信息部分
            ' 所以需要进行跳过
            For Each part As String In parts.Skip(1)
                Dim hsp$() = r.Matches(part, PAIRWISE, RegexICSng).ToArray
                Dim hit As New SubjectHit With {
                    .Score = Score.TryParse(Of Score)(part),
                    .Name = name,
                    .Length = l,
                    .Hsp = ParseHitSegments(hsp)
                }

                Yield hit
            Next
        End Function

        Protected Shared Function ParseHitSegments(textLines As String()) As HitSegment()
            Dim hsp As HitSegment() = New HitSegment(textLines.Length - 1) {}

            For i As Integer = 0 To textLines.Length - 1
                Dim buffer$() = LinqAPI.Exec(Of String) _
 _
                    () <= From s As String
                          In textLines(i).LineTokens
                          Select s.Replace(vbCr, "")

                hsp(i) = HitSegment.TryParse(buffer)
            Next

            Return hsp
        End Function
    End Class
End Namespace
