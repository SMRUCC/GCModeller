Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Public Module SequenceParser

    <Extension>
    Public Iterator Function ExtractSequence(gff As GFFTable, seqs As IEnumerable(Of FastaSeq)) As IEnumerable(Of FastaSeq)
        Dim seqdata As Dictionary(Of String, FastaSeq) = seqs _
            .AsEnumerable _
            .ToDictionary(Function(s)
                              Return s.Title
                          End Function)

        For Each feature As Feature In gff.features
            Dim parent_id As String = feature.attributes("Parent")
            Dim title As String = $"{parent_id} {feature.feature}.{feature.frame} [{feature.start}-{feature.ends}]"
            Dim chr_id As String = feature.source
            Dim seq As FastaSeq = seqdata(chr_id)
            Dim seq_cut As SimpleSegment = seq.CutSequenceLinear(feature.start, feature.ends, title)

            Yield New FastaSeq(seq_cut)
        Next
    End Function

End Module
