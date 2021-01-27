#Region "Microsoft.VisualBasic::3808ad2b06004d925fe7d86a3247eeb4, LocalBLAST\Web\Alignment\Hit.vb"

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

    '     Class HitRecord
    ' 
    '         Properties: AlignmentLength, BitScore, data, EValue, GapOpens
    '                     GI, Identity, MisMatches, QueryAccVer, QueryEnd
    '                     QueryID, QueryStart, SubjectAccVer, SubjectEnd, SubjectIDs
    '                     SubjectStart
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: SplitByHeaders, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language

Namespace NCBIBlastResult.WebBlast

    ''' <summary>
    ''' 在目标基因组之上的blast hit的结果
    ''' </summary>
    ''' <remarks>
    ''' Fields: query id, subject ids, % identity, alignment length, mismatches, gap opens, q. start, q. end, s. start, s. end, evalue, bit score
    ''' </remarks>
    <XmlType("hit", [Namespace]:="http://gcmodeller.org/visual/circos/blast_hit")>
    Public Class HitRecord

        <XmlAttribute("query_name")>
        <Column("query id")>
        Public Property QueryID As String
        ''' <summary>
        ''' 不同的编号，但是都是代表着同一个对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("hits")>
        <Column("subject ids")>
        Public Property SubjectIDs As String

        <Column("query acc.ver")>
        Public Property QueryAccVer As String
        <Column("subject acc.ver")>
        Public Property SubjectAccVer As String

        <XmlAttribute>
        <Column("% identity")>
        Public Property Identity As Double
        <XmlAttribute>
        <Column("alignment length")>
        Public Property AlignmentLength As Integer
        <XmlAttribute("mismatches")>
        <Column("mismatches")>
        Public Property MisMatches As Integer
        <XmlAttribute("gaps")>
        <Column("gap opens")>
        Public Property GapOpens As Integer
        <XmlAttribute("query.start")>
        <Column("q. start")>
        Public Property QueryStart As Integer
        <XmlAttribute("query.ends")>
        <Column("q. end")>
        Public Property QueryEnd As Integer
        <XmlAttribute("hit.start")>
        <Column("s. start")>
        Public Property SubjectStart As Integer
        <XmlAttribute("hit.ends")>
        <Column("s. end")>
        Public Property SubjectEnd As Integer
        <XmlAttribute("E-value")>
        <Column("evalue")>
        Public Property EValue As Double
        <XmlAttribute("bits")>
        <Column("bit score")>
        Public Property BitScore As Integer

        Friend DebugTag As String

        Public Property data As Dictionary(Of String, String)

        Public ReadOnly Property GI As String()
            Get
                Dim GIList As String() =
                    Regex.Matches(Me.SubjectIDs, "gi\|\d+") _
                         .ToArray(Function(s)
                                      Return s.Split("|"c).Last
                                  End Function)
                Return GIList
            End Get
        End Property

        Sub New()
        End Sub

        ''' <summary>
        ''' 请注意，在这里是按值复制
        ''' </summary>
        ''' <param name="copy"></param>
        Sub New(copy As HitRecord)
            With Me
                .AlignmentLength = copy.AlignmentLength
                .BitScore = copy.BitScore
                .DebugTag = copy.DebugTag
                .EValue = copy.EValue
                .GapOpens = copy.GapOpens
                .Identity = copy.Identity
                .MisMatches = copy.MisMatches
                .QueryAccVer = copy.QueryAccVer
                .QueryEnd = copy.QueryEnd
                .QueryID = copy.QueryID
                .QueryStart = copy.QueryStart
                .SubjectAccVer = copy.SubjectAccVer
                .SubjectEnd = copy.SubjectEnd
                .SubjectIDs = copy.SubjectIDs
                .SubjectStart = copy.SubjectStart

                If Not copy.data Is Nothing Then
                    .data = New Dictionary(Of String, String)(copy.data)
                End If
            End With
        End Sub

        ''' <summary>
        ''' 当<see cref="SubjectIDs"/>之中包含有多个比对结果序列的时候，使用分号``;``作为分隔符将表头分开
        ''' </summary>
        ''' <returns></returns>
        Public Function SplitByHeaders() As HitRecord()
            Dim tokens$() = SubjectIDs.Split(";"c)
            Dim out As New List(Of HitRecord)

            For Each t$ In tokens
                out += New HitRecord(Me) With {
                    .SubjectIDs = t$
                }
            Next

            Return out
        End Function

        Public Overrides Function ToString() As String
            If Not String.IsNullOrEmpty(DebugTag) Then
                Return String.Format("[{0}, {1}]   ===> {2}   //{3}", QueryStart, QueryEnd, SubjectIDs, DebugTag)
            Else
                Return String.Format("[{0}, {1}]   ===> {2}", QueryStart, QueryEnd, SubjectIDs)
            End If
        End Function
    End Class
End Namespace
