Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Kmers

    Public Class KmerWriter : Implements IDisposable, DatabaseWriter

        Friend ReadOnly seqs As New List(Of SequenceSource)
        Friend ReadOnly database_dir As String
        Friend ReadOnly shardings As New Dictionary(Of String, ShardingWriter)

        Dim disposedValue As Boolean
        Dim k As Integer
        Dim config As Dictionary(Of String, String)
        Dim parallel As Boolean = True
        Dim n_threads As Integer = 16

        Sub New(database As String, k As Integer)
            Me.database_dir = database
            Me.k = k
            Me.config = $"{database_dir}/config.txt".LoadJsonFile(Of Dictionary(Of String, String))(throwEx:=False)
            Me.config = If(config, New Dictionary(Of String, String))

            config!k = k
            seqs = $"{database_dir}/seq_ids.csv".LoadCsv(Of SequenceSource).AsList

            Call My.Resources.Docs.readme.FlushStream($"{database_dir}/readme.txt")
        End Sub

        Public Const Magic$ = "kmer-index"
        Public Const Magic2 = "kmer-seeds"

        Public Sub SetKSize(k As Integer) Implements DatabaseWriter.SetKSize
            Me.k = k
        End Sub

        Public Function AddSequenceID(taxid As UInteger, name As String) As UInteger Implements DatabaseWriter.AddSequenceID
            Dim id As UInteger = seqs.Count + 1
            Dim genbank_info As NamedValue(Of String) = name.GetTagValue(" ", trim:=True, failureNoName:=False)
            Dim seq As New SequenceSource With {
            .id = id,
            .name = If(genbank_info.Value.StringEmpty, "no_name", genbank_info.Value),
            .ncbi_taxid = taxid,
            .accession_id = genbank_info.Name
        }

            Call seqs.Add(seq)

            Return id
        End Function

        ''' <summary>
        ''' add sequence into database
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="taxid"></param>
        Public Sub Add(seq As IFastaProvider, taxid As UInteger) Implements DatabaseWriter.Add
            Dim seqid As UInteger = AddSequenceID(taxid, seq.title)
            Dim seeds As IEnumerable(Of KmerSeed) = CreateFromSequence(seq, k, seqid)

            Call Add(seeds)
        End Sub

        ''' <summary>
        ''' add sequence into database
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="taxid"></param>
        Public Sub Add(seq As ChunkedNtFasta, taxid As UInteger) Implements DatabaseWriter.Add
            Dim seqid As UInteger = AddSequenceID(taxid, seq.title)
            Dim seeds As IEnumerable(Of KmerSeed) = CreateFromSequence(seq, k, seqid)

            Call Add(seeds)
        End Sub

        Private Sub Add(seeds As IEnumerable(Of KmerSeed))
            If parallel Then
                For Each batch As KmerSeed() In seeds.SplitIterator(partitionSize:=500000)
                    Dim prefixGroups As IGrouping(Of String, KmerSeed)() = batch _
                    .GroupBy(Function(k)
                                 Return k.kmer.Substring(3, length:=KmersDatabase.prefixSize)
                             End Function) _
                    .ToArray
                    Dim opt As New ParallelOptions With {.MaxDegreeOfParallelism = n_threads}

                    Call System.Threading.Tasks.Parallel.For(
                    fromInclusive:=0,
                    toExclusive:=prefixGroups.Length,
                    body:=Sub(i)
                              Dim data As IGrouping(Of String, KmerSeed) = prefixGroups(i)
                              Dim part As ShardingWriter
                              Dim prefixKey As String = prefixGroups(i).Key

                              If Not shardings.ContainsKey(prefixKey) Then
                                  SyncLock shardings
                                      Call shardings.Add(prefixKey, New ShardingWriter($"{database_dir}/data/{prefixKey}", k, Me))
                                  End SyncLock
                              End If

                              part = shardings(prefixKey)
                              part.Add(DirectCast(data, IEnumerable(Of KmerSeed)))
                          End Sub)
                Next
            Else
                ' processing in sequential mode
                Dim prefixKey As String
                Dim part As ShardingWriter

                For Each kmer As KmerSeed In seeds
                    prefixKey = kmer.kmer.Substring(3, length:=KmersDatabase.prefixSize)

                    If Not shardings.ContainsKey(prefixKey) Then
                        shardings.Add(prefixKey, New ShardingWriter($"{database_dir}/data/{prefixKey}", k, Me))
                    End If

                    part = shardings(prefixKey)
                    part.Add(kmer)
                Next
            End If
        End Sub

        ''' <summary>
        ''' used for make processing of the small genome sequence data, example as virus or bacterial
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="k"></param>
        ''' <param name="seqid"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CreateFromSequence(nt As IFastaProvider, k As Integer, seqid As UInteger) As IEnumerable(Of KmerSeed)
            Return KmerSeedsInternal(KSeq.KmerSpans(nt.GetSequenceData.ToUpper, k), seqid, batchSize:=5000000)
        End Function

        ''' <summary>
        ''' 检查k-mer是否仅包含标准的A, T, C, G碱基
        ''' </summary>
        Private Shared Function IsValidKmer(kmer As String) As Boolean
            ' 使用 Any 和一个简单的字符检查，效率很高
            Return Not kmer.Any(Function(c) c <> "A"c AndAlso c <> "T"c AndAlso c <> "C"c AndAlso c <> "G"c)
        End Function

        Private Shared Iterator Function KmerSeedsInternal(kmers As IEnumerable(Of String), seqid As UInteger, batchSize As Integer) As IEnumerable(Of KmerSeed)
            ' rawdata cache
            Dim batchData As New Dictionary(Of String, List(Of UInteger))
            Dim offset As UInteger = 0
            Dim batch As Integer = 0

            For Each kmer As String In kmers
                If Not IsValidKmer(kmer) Then
                    Continue For
                End If

                If Not batchData.ContainsKey(kmer) Then
                    Call batchData.Add(kmer, New List(Of UInteger))
                End If

                batch += 1
                offset += 1
                batchData(kmer).Add(offset)

                If batch > batchSize Then
                    Call "processing batch k-mer data".info

                    batch = 0

                    For Each seed As KmerSeed In KmerSeedsInternal(batchData, seqid)
                        Yield seed
                    Next

                    Call batchData.Clear()
                End If
            Next

            For Each seed As KmerSeed In KmerSeedsInternal(batchData, seqid)
                Yield seed
            Next

            Call batchData.Clear()
            Call GC.Collect()
        End Function

        Private Shared Iterator Function KmerSeedsInternal(seeds As Dictionary(Of String, List(Of UInteger)), seqid As UInteger) As IEnumerable(Of KmerSeed)
            For Each seed As KeyValuePair(Of String, List(Of UInteger)) In seeds
                Dim offsets As List(Of UInteger) = seed.Value
                Dim source As New KmerSource With {
                .count = offsets.Count,
                .locations = offsets.ToArray,
                .seqid = seqid
            }

                Yield New KmerSeed With {
                .kmer = seed.Key,
                .source = New KmerSource() {source}
            }
            Next
        End Function

        ''' <summary>
        ''' used for make processing of large genome sequence data, example as human genome
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="k"></param>
        ''' <param name="seqid"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CreateFromSequence(nt As ChunkedNtFasta, k As Integer, seqid As UInteger,
                                              Optional batchSize As Integer = 5000000) As IEnumerable(Of KmerSeed)

            Return KmerSeedsInternal(nt.Kmers(k), seqid, batchSize)
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    For Each shard As ShardingWriter In shardings.Values
                        Call shard.Dispose()
                    Next

                    Call seqs.SaveTo($"{database_dir}/seq_ids.csv")
                    Call config.GetJson.SaveTo($"{database_dir}/config.txt")
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace