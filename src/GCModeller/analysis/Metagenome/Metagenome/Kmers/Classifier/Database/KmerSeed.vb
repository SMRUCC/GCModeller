#Region "Microsoft.VisualBasic::7e2ccb1d23831bf964c19096537d58d9, analysis\Metagenome\Metagenome\Kmers\Classifier\Database\KmerSeed.vb"

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


    ' Code Statistics:

    '   Total Lines: 83
    '    Code Lines: 34 (40.96%)
    ' Comment Lines: 35 (42.17%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 14 (16.87%)
    '     File Size: 2.45 KB


    '     Class KmerSeed
    ' 
    '         Properties: kmer, source, weight
    ' 
    '         Function: ToString
    ' 
    '     Class KmerSource
    ' 
    '         Properties: count, locations, seqid
    ' 
    '         Function: ToString
    ' 
    '     Class SequenceSource
    ' 
    '         Properties: accession_id, id, name, ncbi_taxid, taxname
    '                     taxonomy
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Kmers

    Public Class KmerSeed

        ''' <summary>
        ''' the k-mer sequence string
        ''' </summary>
        ''' <returns></returns>
        Public Property kmer As String
        ''' <summary>
        ''' taxonomy source information
        ''' </summary>
        ''' <returns></returns>
        Public Property source As KmerSource()

        Public ReadOnly Property weight As Double
            Get
                Return 1 / source.Length
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{kmer} has {source.Length} taxonomy source"
        End Function

    End Class

    Public Class KmerSource

        ''' <summary>
        ''' sequence source <see cref="SequenceSource.id"/> of the kmer
        ''' </summary>
        ''' <returns></returns>
        Public Property seqid As UInteger
        ''' <summary>
        ''' size of <see cref="locations"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property count As Integer
        ''' <summary>
        ''' kmer location on the genome sequence of <see cref="SequenceSource"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property locations As UInteger()

        Public Overrides Function ToString() As String
            Return $"#{seqid} = {locations.GetJson}"
        End Function

    End Class

    ''' <summary>
    ''' source information of the target genome source sequence
    ''' </summary>
    Public Class SequenceSource

        ''' <summary>
        ''' the sequence source id
        ''' </summary>
        ''' <returns></returns>
        Public Property id As UInteger
        Public Property ncbi_taxid As Integer
        Public Property accession_id As String
        ''' <summary>
        ''' genomics sequence name
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        Public Property taxname As String
        ''' <summary>
        ''' taxonomy lineage information in BIOM style string
        ''' </summary>
        ''' <returns></returns>
        Public Property taxonomy As String

        Public Overrides Function ToString() As String
            Return $"[{id}, ncbi_taxid:{ncbi_taxid}] {name}"
        End Function

    End Class
End Namespace
