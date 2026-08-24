#Region "Microsoft.VisualBasic::70276343ffa91f5ba0dac79669e90045, analysis\Metagenome\Metagenome\Kmers\KmerFilter.vb"

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

    '   Total Lines: 53
    '    Code Lines: 33 (62.26%)
    ' Comment Lines: 10 (18.87%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (18.87%)
    '     File Size: 1.60 KB


    '     Class KmerFilter
    ' 
    '         Properties: k, ncbi_taxid
    ' 
    '         Function: KmerHits, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Namespace Kmers

    Public MustInherit Class KmerFilter

        ''' <summary>
        ''' the genome name(multiple chromosome name)
        ''' </summary>
        Protected names As String()

        Dim _k As Integer
        Dim _ncbi_taxid As Integer

        ''' <summary>
        ''' the length of the k-mer
        ''' </summary>
        Public Property k As Integer
            Get
                Return _k
            End Get
            Protected Set(value As Integer)
                _k = value
            End Set
        End Property

        ''' <summary>
        ''' the genome taxonomy id
        ''' </summary>
        ''' <returns></returns>
        Public Property ncbi_taxid As Integer
            Get
                Return _ncbi_taxid
            End Get
            Protected Set(value As Integer)
                _ncbi_taxid = value
            End Set
        End Property

        Public Function KmerHits(seq As ISequenceProvider) As Dictionary(Of String, Integer)
            Return KmerHits(KSeq.KmerSpans(seq.GetSequenceData, k))
        End Function

        Public MustOverride Function KmerHitNumber(kmers As IEnumerable(Of String)) As Integer
        Public MustOverride Function KmerHits(kmers As IEnumerable(Of String)) As Dictionary(Of String, Integer)

        Public Overrides Function ToString() As String
            Return $"ncbi_taxid: {ncbi_taxid}; " & names(0)
        End Function

    End Class
End Namespace
