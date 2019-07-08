#Region "Microsoft.VisualBasic::e69900062878e93517025dddb1dd5c41, Bio.Assembly\ContextModel\Algorithm\Context.vb"

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

    '     Structure Context
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: __getRel, __relStranede, __relUnstrand, GetRelation, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace ContextModel

    ''' <summary>
    ''' Context model of a specific genomics feature site.
    ''' </summary>
    Public Structure Context

        ''' <summary>
        ''' Current feature site
        ''' </summary>
        Public ReadOnly Feature As NucleotideLocation
        ''' <summary>
        ''' <see cref="Feature"/> its upstream region with a specific length
        ''' </summary>
        Public ReadOnly Upstream As NucleotideLocation
        ''' <summary>
        ''' <see cref="Feature"/> its downstream region with a specific length
        ''' </summary>
        Public ReadOnly Downstream As NucleotideLocation
        Public ReadOnly Antisense As NucleotideLocation

        ''' <summary>
        ''' The user custom tag data for this feature site.
        ''' </summary>
        Public ReadOnly Tag As String

        Sub New(g As IGeneBrief, dist As Integer)
            Call Me.New(g.Location, dist, g.ToString)
        End Sub

        Sub New(loci As NucleotideLocation, dist As Integer, Optional userTag As String = Nothing)
            Feature = loci
            Tag = FirstNotEmpty(userTag, loci.ToString)

            If loci.Strand = Strands.Forward Then
                Upstream = New NucleotideLocation(loci.Left - dist, loci.Left, Strands.Forward)
                Downstream = New NucleotideLocation(loci.Right, loci.Right + dist, Strands.Forward)
                Antisense = New NucleotideLocation(loci.Left, loci.Right, Strands.Reverse)
            Else
                Upstream = New NucleotideLocation(loci.Right, loci.Right + dist, Strands.Reverse)
                Downstream = New NucleotideLocation(loci.Left - dist, loci.Left, Strands.Reverse)
                Antisense = New NucleotideLocation(loci.Left, loci.Right, Strands.Forward)
            End If
        End Sub

        Sub New(g As Contig, dist As Integer)
            Call Me.New(g.MappingLocation, dist, g.ToString)
        End Sub

        ''' <summary>
        ''' Get relationship between target <see cref="NucleotideLocation"/> with current feature site.
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="stranded">Get <see cref="SegmentRelationships"/> ignored of nucleotide <see cref="Strands"/>?</param>
        ''' <returns></returns>
        Public Function GetRelation(loci As NucleotideLocation, stranded As Boolean) As SegmentRelationships
            If stranded Then
                Return __relStranede(loci)
            Else
                Return __relUnstrand(loci)
            End If
        End Function

        ''' <summary>
        ''' Get <see cref="SegmentRelationships"/> ignored of nucleotide <see cref="Strands"/>.
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <returns></returns>
        Private Function __relUnstrand(loci As NucleotideLocation) As SegmentRelationships
            Dim rel As SegmentRelationships = __getRel(loci)

            If rel = SegmentRelationships.Blank Then
                If loci.Strand <> Feature.Strand Then
                    If Antisense.IsInside(loci) Then
                        Return SegmentRelationships.InnerAntiSense
                    End If
                End If
            End If

            Return rel
        End Function

        Private Function __getRel(loci As NucleotideLocation) As SegmentRelationships
            If Upstream.IsInside(loci) Then
                Return SegmentRelationships.UpStream
            ElseIf Downstream.IsInside(loci) Then
                Return SegmentRelationships.DownStream
            ElseIf Feature.Equals(loci, 1) Then
                Return SegmentRelationships.Equals
            ElseIf Feature.IsInside(loci) Then
                Return SegmentRelationships.Inside
            Else
                If Feature.IsInside(loci.Left) AndAlso Upstream.IsInside(loci.Right) Then
                    Return SegmentRelationships.UpStreamOverlap
                ElseIf Feature.IsInside(loci.Right) AndAlso Upstream.IsInside(loci.Left) Then
                    Return SegmentRelationships.UpStreamOverlap
                ElseIf Feature.IsInside(loci.Left) AndAlso Downstream.IsInside(loci.Right) Then
                    Return SegmentRelationships.DownStreamOverlap
                ElseIf Feature.IsInside(loci.Right) AndAlso Downstream.IsInside(loci.Left) Then
                    Return SegmentRelationships.DownStreamOverlap
                ElseIf loci.IsInside(Feature) Then
                    Return SegmentRelationships.Cover
                Else
                    Return SegmentRelationships.Blank
                End If
            End If
        End Function

        Private Function __relStranede(loci As NucleotideLocation) As SegmentRelationships
            If loci.Strand <> Feature.Strand Then  ' 不在同一条链之上
                If Antisense.IsInside(loci) Then
                    Return SegmentRelationships.InnerAntiSense
                Else
                    Return SegmentRelationships.Blank
                End If
            Else
                Return __getRel(loci)
            End If
        End Function

        ''' <summary>
        ''' Get tags data
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Tag
        End Function
    End Structure
End Namespace
