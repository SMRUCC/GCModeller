#Region "Microsoft.VisualBasic::e1d27e8722311b056263f3369232751c, ..\interops\localblast\LocalBLAST\LocalBLAST\BlastOutput\Common\Segment.vb"

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

Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace LocalBLAST.BLASTOutput.ComponentModel

    ''' <summary>
    ''' 表示一个Query和Subject匹配上的序列片段相对应的位置，即HSP高分区片段
    ''' </summary>
    ''' <remarks></remarks>
    Public Class HitSegment

        <XmlElement> Public Property Query As Segment
        <XmlElement> Public Property Sbjct As Segment
        <XmlAttribute> Public Property Consensus As String

        Public Overrides Function ToString() As String
            Return Consensus
        End Function

        Public Const QUERY_START As String = "Query: \d+"
        Public Const QUERY_INDEX = 0
        Public Const CONSERVED_INDEX = 1
        Public Const SUBJECT_INDEX = 2
        Public Const SUBJECT_START As String = "Sbjct: \d+"

        Public Shared Function TryParse(TextLines As String()) As HitSegment
            If TextLines.IsNullOrEmpty Then
                Return Nothing
            ElseIf TextLines.Length < 3 Then
                Dim sBuilder As StringBuilder = New StringBuilder(1024)
                For Each item In TextLines
                    Call sBuilder.AppendLine(vbTab & vbTab & item)
                Next

                Call Console.WriteLine("System.IndexOutOfRangeException:" & vbCrLf & "---------------------------------------------------------------------------------------")
                Call Console.WriteLine(sBuilder.ToString)
                Return Nothing
            Else
                Return New HitSegment With {
                    .Query = Segment.TryParse(TextLines(QUERY_INDEX)),
                    .Consensus = TextLines(CONSERVED_INDEX).Trim,
                    .Sbjct = Segment.TryParse(TextLines(SUBJECT_INDEX))
                }
            End If
        End Function
    End Class

    ''' <summary>
    ''' 匹配上的序列片段
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Segment : Implements I_PolymerSequenceModel, ILocationSegment

        <XmlAttribute> Public Property Left As Long
        <XmlAttribute> Public Property Right As Long
        <XmlAttribute> Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

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

        Public Shared Function TryParse(Text As String) As Segment
            Dim Numbers = Regex.Matches(Text, "\d+")
            Dim Tokens As String() = Text.Split
            Dim sg As New Segment With {
                .Left = Numbers(0).Value.RegexParseDouble,
                .Right = Numbers(1).Value.RegexParseDouble
            }

            For i As Integer = Tokens.Count - 1 To 1 Step -1
                If Not Tokens(i).IsNullOrEmpty AndAlso Tokens(i).RegexParseDouble = 0.0R Then
                    sg.SequenceData = Tokens(i)
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
