Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Kmers

    Public Module Extensions

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="kmers_db"></param>
        ''' <param name="sample">all reads data in a given sample</param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function QueryKmers(Of T As ISequenceProvider)(kmers_db As DatabaseReader, sample As IEnumerable(Of T)) As IEnumerable(Of KmerSeed)
            For Each reads As T In sample

            Next
        End Function

    End Module
End Namespace