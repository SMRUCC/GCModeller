#Region "Microsoft.VisualBasic::4fe012eba8cb98ae6bd039300e27b15c, core\Bio.Assembly\ContextModel\Algorithm\Context.vb"

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
    '         Function: __relStranede, __relUnstrand, GetRelation, getRelationship, PopulateSequence
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace ContextModel

    ''' <summary>
    ''' Context model of a specific genomics feature site.
    ''' </summary>
    Public Structure Context

        ''' <summary>
        ''' Current feature site
        ''' </summary>
        Public ReadOnly feature As NucleotideLocation
        ''' <summary>
        ''' <see cref="feature"/> its upstream region with a specific length
        ''' </summary>
        Public ReadOnly upstream As NucleotideLocation
        ''' <summary>
        ''' <see cref="feature"/> its downstream region with a specific length
        ''' </summary>
        Public ReadOnly downstream As NucleotideLocation
        ''' <summary>
        ''' the complement strand location of current <see cref="feature"/>
        ''' </summary>
        Public ReadOnly antisense As NucleotideLocation
        Public ReadOnly distance As Integer

        ''' <summary>
        ''' The user custom tag data for this feature site.
        ''' </summary>
        Public ReadOnly tag As String

        Sub New(g As IGeneBrief, dist As Integer)
            Call Me.New(g.Location, dist, g.ToString)
        End Sub

        Sub New(loci As NucleotideLocation, dist As Integer, Optional note As String = Nothing)
            feature = loci
            tag = If(note, loci.ToString)
            distance = dist

            If loci.Strand = Strands.Forward Then
                upstream = New NucleotideLocation(loci.left - dist, loci.left, Strands.Forward)
                downstream = New NucleotideLocation(loci.right, loci.right + dist, Strands.Forward)
                antisense = New NucleotideLocation(loci.left, loci.right, Strands.Reverse)
            Else
                upstream = New NucleotideLocation(loci.right, loci.right + dist, Strands.Reverse)
                downstream = New NucleotideLocation(loci.left - dist, loci.left, Strands.Reverse)
                antisense = New NucleotideLocation(loci.left, loci.right, Strands.Forward)
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
            Dim rel As SegmentRelationships = getRelationship(loci)

            If rel = SegmentRelationships.Blank Then
                If loci.Strand <> feature.Strand Then
                    If antisense.IsInside(loci) Then
                        Return SegmentRelationships.InnerAntiSense
                    End If
                End If
            End If

            Return rel
        End Function

        Private Function getRelationship(loci As NucleotideLocation) As SegmentRelationships
            If upstream.IsInside(loci) Then
                Return SegmentRelationships.UpStream
            ElseIf downstream.IsInside(loci) Then
                Return SegmentRelationships.DownStream
            ElseIf feature.Equals(loci, 1) Then
                Return SegmentRelationships.Equals
            ElseIf feature.IsInside(loci) Then
                Return SegmentRelationships.Inside
            Else
                If feature.IsInside(loci.left) AndAlso upstream.IsInside(loci.right) Then
                    Return SegmentRelationships.UpStreamOverlap
                ElseIf feature.IsInside(loci.right) AndAlso upstream.IsInside(loci.left) Then
                    Return SegmentRelationships.UpStreamOverlap
                ElseIf feature.IsInside(loci.left) AndAlso downstream.IsInside(loci.right) Then
                    Return SegmentRelationships.DownStreamOverlap
                ElseIf feature.IsInside(loci.right) AndAlso downstream.IsInside(loci.left) Then
                    Return SegmentRelationships.DownStreamOverlap
                ElseIf loci.IsInside(feature) Then
                    Return SegmentRelationships.Cover
                Else
                    Return SegmentRelationships.Blank
                End If
            End If
        End Function

        Private Function __relStranede(loci As NucleotideLocation) As SegmentRelationships
            If loci.Strand <> feature.Strand Then  ' 不在同一条链之上
                If antisense.IsInside(loci) Then
                    Return SegmentRelationships.InnerAntiSense
                Else
                    Return SegmentRelationships.Blank
                End If
            Else
                Return getRelationship(loci)
            End If
        End Function

        ''' <summary>
        ''' Get tags data
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return tag
        End Function

        Public Iterator Function PopulateSequence(nt As FastaSeq) As IEnumerable(Of NamedValue(Of SimpleSegment))
            Yield New NamedValue(Of SimpleSegment) With {.Name = NameOf(feature), .Value = nt.CutSequenceLinear(feature)}
            Yield New NamedValue(Of SimpleSegment) With {.Name = NameOf(upstream), .Value = nt.CutSequenceLinear(upstream)}
            Yield New NamedValue(Of SimpleSegment) With {.Name = NameOf(downstream), .Value = nt.CutSequenceLinear(downstream)}
            Yield New NamedValue(Of SimpleSegment) With {.Name = NameOf(antisense), .Value = nt.CutSequenceLinear(antisense)}
        End Function
    End Structure
End Namespace
