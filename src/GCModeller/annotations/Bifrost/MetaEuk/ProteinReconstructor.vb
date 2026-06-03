
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
