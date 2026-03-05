Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.HashMaps.MinHash
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Module minhashtest

    Sub Main()
        Dim seqset = FastaFile.Read("U:\metagenomics_LLMs\demo\seq.fa")
        Dim seqs As New List(Of SequenceItem)
        Dim idset As New List(Of String)
        Dim id As i32 = 0

        For Each seq As FastaSeq In TqdmWrapper.Wrap(seqset.Take(10000).ToArray)
            Call idset.Add(seq.Title)
            Call seqs.Add(KSeq.KmerSpans(seq.SequenceData, k:=12).CreateSequenceData(++id))
        Next

        For Each result As SimilarityIndex In LSH.FindSimilarItems(seqs.ToArray)
            Call Console.WriteLine(result)
        Next

        Pause()
    End Sub
End Module
