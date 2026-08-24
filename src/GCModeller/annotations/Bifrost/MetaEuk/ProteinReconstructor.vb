#Region "Microsoft.VisualBasic::c67fd283e823ec309a4378c85e8db9d8, annotations\Bifrost\MetaEuk\ProteinReconstructor.vb"

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

    '   Total Lines: 56
    '    Code Lines: 30 (53.57%)
    ' Comment Lines: 12 (21.43%)
    '    - Xml Docs: 33.33%
    ' 
    '   Blank Lines: 14 (25.00%)
    '     File Size: 2.21 KB


    ' Class ProteinReconstructor
    ' 
    '     Sub: ReconstructAll
    ' 
    ' /********************************************************************************/

#End Region


' ========================================================================
' MODULE 10: PROTEIN SEQUENCE RECONSTRUCTION
' ========================================================================

Imports System.Text
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class ProteinReconstructor

    ''' <summary>
    ''' Reconstruct the predicted protein sequence from exon coordinates.
    ''' Translates the DNA subsequence for each exon and concatenates them.
    ''' </summary>
    Public Shared Sub ReconstructAll(predictions As List(Of GenePrediction), contigs As IEnumerable(Of FastaSeq))
        ' Build contig lookup
        Dim contigDict = contigs.ToDictionary(Function(c) c.locus_tag)

        For Each pred In predictions
            Dim protSB As New StringBuilder()

            If contigDict.ContainsKey(pred.ContigID) Then
                Dim dna = contigDict(pred.ContigID).SequenceData.ToUpper()

                ' Sort exons by position
                pred.Exons.Sort(Function(a, b) a.DnaStart.CompareTo(b.DnaStart))

                For Each exon In pred.Exons
                    Dim start0 = Math.Max(0, exon.DnaStart - 1)  ' convert to 0-based
                    Dim end0 = Math.Min(dna.Length - 1, exon.DnaEnd - 1)

                    If start0 > end0 OrElse start0 >= dna.Length Then Continue For

                    Dim exonDna = dna.Substring(start0, end0 - start0 + 1)

                    ' For minus strand, take reverse complement
                    If exon.Strand = Strands.Reverse Then
                        exonDna = CodonTable.ReverseComplement(exonDna)
                    End If

                    ' Translate
                    Dim pep = CodonTable.Translate(exonDna, 0)
                    ' Remove trailing stop codon if present
                    If pep.Length > 0 AndAlso pep.EndsWith("*"c) Then
                        pep = pep.Substring(0, pep.Length - 1)
                    End If
                    protSB.Append(pep)
                Next
            End If

            pred.ProteinSequence = protSB.ToString()
        Next
    End Sub

End Class

