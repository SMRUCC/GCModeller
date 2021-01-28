#Region "Microsoft.VisualBasic::0db2f65269e4f8c537e8dd65c4350324, LocalBLAST\LocalBLAST\BlastOutput\Common\Segment.vb"

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

    '     Class HitSegment
    ' 
    '         Properties: Consensus, Query, Subject
    ' 
    '         Function: ToString, TryParse
    ' 
    '     Class Segment
    ' 
    '         Properties: Left, Location, Right, SequenceData, UniqueId
    ' 
    '         Function: ToString, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.SequenceModel

Namespace LocalBLAST.BLASTOutput.ComponentModel

    ''' <summary>
    ''' 表示一个Query和Subject匹配上的序列片段相对应的位置，即HSP高分区片段
    ''' </summary>
    ''' <remarks></remarks>
    Public Class HitSegment

        <XmlElement> Public Property Query As Segment
        <XmlElement> Public Property Subject As Segment
        <XmlAttribute>
        Public Property Consensus As String

        Public Overrides Function ToString() As String
            Return Consensus
        End Function

        Public Const QUERY_START As String = "Query: \d+"
        Public Const QUERY_INDEX = 0
        Public Const CONSERVED_INDEX = 1
        Public Const SUBJECT_INDEX = 2
        Public Const SUBJECT_START As String = "Sbjct: \d+"

        Public Shared Function TryParse(hspLines$()) As HitSegment
            If hspLines.IsNullOrEmpty Then
                Return Nothing

            ElseIf hspLines.Length = 2 Then

                ' 没有同源的片段，但是也是高分区的一部分
                Return New HitSegment With {
                    .Query = Segment.TryParse(hspLines(0)),
                    .Consensus = "",
                    .Subject = Segment.TryParse(hspLines(1))
                }
            Else
                Return New HitSegment With {
                    .Query = Segment.TryParse(hspLines(QUERY_INDEX)),
                    .Consensus = hspLines(CONSERVED_INDEX).Trim,
                    .Subject = Segment.TryParse(hspLines(SUBJECT_INDEX))
                }
            End If
        End Function
    End Class

    ''' <summary>
    ''' 匹配上的序列片段
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Segment : Implements IPolymerSequenceModel, ILocationSegment

        <XmlAttribute> Public Property Left As Long
        <XmlAttribute> Public Property Right As Long
        <XmlAttribute> Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        Public ReadOnly Property UniqueId As String Implements ILocationSegment.UniqueId
            Get
                Return Location.ToString
            End Get
        End Property

        Public ReadOnly Property Location As Location Implements ILocationSegment.Location
            Get
                Return New Location(Left, Right)
            End Get
        End Property

        Public Const QUERY_FORMAT As String = "Query\s+\d+\s+[A-Z]+\s+\d+"

        Public Shared Function TryParse(text As String) As Segment
            Dim numbers = Regex.Matches(text, "\d+").ToArray
            Dim tokens As String() = text.Split
            Dim sg As New Segment With {
                .Left = numbers.ElementAtOrDefault(0).RegexParseDouble,
                .Right = numbers.ElementAtOrDefault(1).RegexParseDouble
            }

            For i As Integer = tokens.Length - 1 To 1 Step -1
                If Not tokens(i).StringEmpty AndAlso tokens(i).RegexParseDouble = 0.0R Then
                    sg.SequenceData = tokens(i)
                    Exit For
                End If
            Next

            Return sg
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1}  {2}", Left, SequenceData, Right)
        End Function
    End Class
End Namespace
