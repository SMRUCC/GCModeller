#Region "Microsoft.VisualBasic::aa414bd30540235b530144e495104c6a, analysis\Metagenome\Metagenome\Kmers\KmerHashIndexFilter.vb"

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

    '   Total Lines: 81
    '    Code Lines: 56 (69.14%)
    ' Comment Lines: 9 (11.11%)
    '    - Xml Docs: 88.89%
    ' 
    '   Blank Lines: 16 (19.75%)
    '     File Size: 3.15 KB


    '     Class KmerHashIndexFilter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Create, KmerHitNumber, KmerHits
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Namespace Kmers

    Public Class KmerHashIndexFilter : Inherits KmerFilter

        ReadOnly hashFilter As HashSet(Of String)

        Sub New(k As Integer, name As IEnumerable(Of String), ncbi_taxid As Integer, filter As HashSet(Of String))
            Me.k = k
            Me.names = name.ToArray
            Me.ncbi_taxid = ncbi_taxid
            Me.hashFilter = filter
        End Sub

        Public Overrides Function KmerHitNumber(kmers As IEnumerable(Of String)) As Integer
            Dim hits As Integer = 0

            For Each kmer As String In kmers
                If hashFilter.Contains(kmer) Then
                    hits += 1
                End If
            Next

            Return hits
        End Function

        Public Overrides Function KmerHits(kmers As IEnumerable(Of String)) As Dictionary(Of String, Integer)
            Dim hits As New Dictionary(Of String, Integer)

            For Each kmer As String In kmers
                If hashFilter.Contains(kmer) Then
                    If Not hits.ContainsKey(kmer) Then
                        hits.Add(kmer, 1)
                    Else
                        hits(kmer) += 1
                    End If
                End If
            Next

            Return hits
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="Fasta"></typeparam>
        ''' <param name="genomics">the genomics sequence data of a organism, single chromosome sequence data</param>
        ''' <param name="ncbi_taxid"></param>
        ''' <param name="k"></param>
        ''' <param name="spanSize"></param>
        ''' <returns></returns>
        Public Shared Function Create(Of Fasta As IFastaProvider)(genomics As Fasta,
                                                                  ncbi_taxid As Integer,
                                                                  Optional k As Integer = 35,
                                                                  Optional spanSize As Integer = 50 * ByteSize.MB) As KmerHashIndexFilter

            Dim estimatedKmers As Integer = Math.Max(0, Math.Min(spanSize, genomics.length - k + 1))
            Dim filter As New HashSet(Of String)
            Dim ntseq As String = genomics.GetSequenceData

            For i As Integer = 0 To ntseq.Length Step spanSize
                Dim len As Integer = spanSize

                If i + len > ntseq.Length Then
                    len = ntseq.Length - i
                End If

                For Each kmer As String In KSeq.KmerSpans(ntseq.Substring(i, len), k)
                    Call filter.Add(kmer)
                Next
            Next

            Return New KmerHashIndexFilter(k, {genomics.title}, ncbi_taxid, filter)
        End Function
    End Class
End Namespace
