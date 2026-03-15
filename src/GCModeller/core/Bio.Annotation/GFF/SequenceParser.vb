Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports std = System.Math

Public Module SequenceParser

    <Extension>
    Public Iterator Function ExtractSequence(gff As GFFTable, seqs As IEnumerable(Of FastaSeq)) As IEnumerable(Of FastaSeq)
        Dim seqdata As Dictionary(Of String, FastaSeq) = seqs _
            .AsEnumerable _
            .ToDictionary(Function(s)
                              Return s.Title
                          End Function)

        For Each feature As Feature In TqdmWrapper.Wrap(gff.features)
            Dim parent_id As String = feature.attributes("parent")
            Dim title As String = $"{parent_id} {feature.feature}.{feature.frame} [{feature.start}-{feature.ends}]"
            Dim chr_id As String = feature.seqname
            Dim seq As FastaSeq = seqdata(chr_id)
            Dim left = std.Min(feature.start, feature.ends)
            Dim right = std.Max(feature.start, feature.ends)
            Dim seq_cut As SimpleSegment = seq.CutSequenceLinear(left, right, title)

            Yield New FastaSeq(seq_cut)
        Next
    End Function

End Module
