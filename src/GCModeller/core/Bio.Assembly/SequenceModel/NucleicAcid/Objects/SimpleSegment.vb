#Region "Microsoft.VisualBasic::a53b1c79148939e68733de4b5bb41c5f, core\Bio.Assembly\SequenceModel\NucleicAcid\Objects\SimpleSegment.vb"

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

    '     Class SimpleSegment
    ' 
    '         Properties: Complement, Ends, ID, Length, SequenceData
    '                     Start, Strand
    ' 
    '         Function: __getMappingLoci, SimpleFasta, ToPTTGene
    ' 
    '         Sub: (+4 Overloads) New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' 没有更多的复杂的继承或者接口实现，只是最简单序列片段对象
    ''' </summary>
    Public Class SimpleSegment : Inherits Contig
        Implements IPolymerSequenceModel

        ''' <summary>
        ''' Probably synonym, locus_tag data
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String

#Region "Location of this loci"

        Public Property Strand As String
        Public Property Start As Integer
        Public Property Ends As Integer
#End Region

        ''' <summary>
        ''' 当前的这个位点的序列数据
        ''' </summary>
        ''' <returns></returns>
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
        ''' <summary>
        ''' The complements sequence of data <see cref="SequenceData"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Complement As String

        Public ReadOnly Property Length As Integer
            Get
                Return Len(SequenceData)
            End Get
        End Property

        Sub New()

        End Sub

        Sub New(loci As SimpleSegment)
            ID = loci.ID
            Strand = loci.Strand
            Start = loci.Start
            Ends = loci.Ends
            SequenceData = loci.SequenceData
            Complement = loci.Complement
        End Sub

        Sub New(loci As SimpleSegment, sId As String)
            Call Me.New(loci)
            ID = sId
        End Sub

        Sub New(site As SimpleSegment, loci As NucleotideLocation)
            Call Me.New(site)

            Start = loci.Left
            Ends = loci.Right
            Strand = loci.Strand.GetBriefCode
        End Sub

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(Start, Ends, Strand).Normalization
        End Function

        Public Function ToPTTGene() As GeneBrief
            Return New GeneBrief With {
                .Code = "-",
                .COG = "-",
                .Gene = ID,
                .IsORF = True,
                .Length = MappingLocation.FragmentSize,
                .Location = MappingLocation,
                .PID = "-",
                .Product = "-",
                .Synonym = ID
            }
        End Function

        Public Function SimpleFasta(Optional title$ = Nothing) As FastaSeq
            If String.IsNullOrEmpty(title) Then
                title = ID & " " & If(
                    MappingLocation.Strand = Strands.Forward,
                    $"{Start},{Ends}",
                    $"complement({Start},{Ends})")
            End If

            Return New FastaSeq With {
                .Headers = {title},
                .SequenceData = SequenceData
            }
        End Function

        Public Shared Operator &(seq$, contig As SimpleSegment) As String
            Return seq & contig.SequenceData
        End Operator
    End Class
End Namespace
