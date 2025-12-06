Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Kmers

    Public Module Extensions

        ''' <summary>
        ''' Get k-mer raw data from all of the sample reads data
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="kmers_db"></param>
        ''' <param name="sample">all reads data in a given sample</param>
        ''' <returns>use this function prepares for make sample quantification</returns>
        <Extension>
        Public Iterator Function QueryKmers(Of T As ISequenceProvider)(kmers_db As DatabaseReader, sample As IEnumerable(Of T)) As IEnumerable(Of KmerSeed)
            For Each reads As T In TqdmWrapper.Wrap(sample.ToArray)
                For Each kmer As KmerSeed In KSeq.KmerSpans(reads.GetSequenceData, kmers_db.k).Select(Function(k) kmers_db.GetKmer(k))
                    If Not kmer Is Nothing Then
                        Yield kmer
                    End If
                Next
            Next
        End Function

    End Module
End Namespace