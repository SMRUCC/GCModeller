Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq

Namespace Kmers

    Public Class ShardingWriter : Implements IDisposable

        Dim disposedValue As Boolean

        ReadOnly writer As KmerWriter
        ReadOnly database_dir As String
        ReadOnly indexCache As New Dictionary(Of String, (offset&, size%))
        ''' <summary>
        ''' in-memory data cache of the kmer database file
        ''' </summary>
        ReadOnly cache As New Dictionary(Of String, KmerSeed)

        ''' <summary>
        ''' reader of the kmer data
        ''' </summary>
        Dim reader As BinaryDataReader
        Dim k As Integer
        Dim batchDataSize As Integer = 500000
        Dim batch As Integer = 0
        Dim prefix As String

        Sub New(database As String, k As Integer, writer As KmerWriter)
            Me.database_dir = database
            Me.k = k
            Me.ResetReader()
            Me.prefix = database.BaseName

            Using index As New BinaryDataReader($"{database_dir}/index.dat".Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True))
                If Not ShardingReader.LoadIndex(index, k, indexCache) Then
                    ' create new
                End If
            End Using
        End Sub

        Private Sub ResetReader()
            reader = New BinaryDataReader($"{database_dir}/kmers.dat".Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True), Encoding.ASCII, leaveOpen:=False)
            reader.ByteOrder = ByteOrder.BigEndian
        End Sub

        ''' <summary>
        ''' add batch data into k-mer database.
        ''' </summary>
        ''' <param name="batch"></param>
        Friend Sub Add(batch As IEnumerable(Of KmerSeed))
            For Each seed As KmerSeed In batch
                Call Add(seed)
            Next
        End Sub

        Friend Sub Add(kmer As KmerSeed)
            If Not cache.ContainsKey(kmer.kmer) Then
                If indexCache.ContainsKey(kmer.kmer) Then
                    ' read data from file
                    With indexCache(kmer.kmer)
                        Call cache.Add(kmer.kmer, ShardingReader.ReadKmer(.offset, .size, k, reader))
                    End With
                Else
                    ' is new seed
                    Call cache.Add(kmer.kmer, New KmerSeed With {
                    .kmer = kmer.kmer,
                    .source = Nothing
                })
                End If
            End If

            batch += 1
            cache(kmer.kmer).source = Union(cache(kmer.kmer).source, kmer.source)

            If batch > batchDataSize Then
                batch = 0
                FlushBatchData()
            End If
        End Sub

        ''' <summary>
        ''' flush current batch of the in-memory kmer cache data into file
        ''' </summary>
        Private Sub FlushBatchData()
            Call $"{prefix} - commit batch data into database file. index_size={StringFormats.Lanudry(indexCache.Count)}".info
            Call FlushData()
            Call ResetReader()
            Call cache.Clear()
            Call GC.Collect()
        End Sub

        Friend Shared Function Union(s1 As KmerSource(), s2 As KmerSource()) As KmerSource()
            Return s1.JoinIterates(s2) _
            .GroupBy(Function(a) a.seqid) _
            .Select(Function(a)
                        Return New KmerSource With {
                            .seqid = a.Key,
                            .locations = a.Select(Function(i) i.locations) _
                                .IteratesALL _
                                .ToArray
                        }
                    End Function) _
            .ToArray
        End Function

        Private Sub WriteIndex()
            Using index As New BinaryDataWriter($"{database_dir}/index.dat".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
                index.ByteOrder = ByteOrder.BigEndian

                Call index.Write(Encoding.ASCII.GetBytes(KmerWriter.Magic))
                Call index.Write(indexCache.Count)
                Call index.Write(k)

                For Each span In indexCache
                    Call index.Write(Encoding.ASCII.GetBytes(span.Key))
                    Call index.Write(span.Value.offset)
                    Call index.Write(span.Value.size)
                Next

                Call index.Flush()
            End Using
        End Sub

        ''' <summary>
        ''' flush kmer cache data into file and then update the index data 
        ''' </summary>
        Private Sub FlushData()
            Dim scan0 As Long

            Call reader.Dispose()

            Using writer As New BinaryDataWriter($"{database_dir}/kmers.dat".Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))
                writer.ByteOrder = ByteOrder.BigEndian

                If indexCache.Count = 0 Then
                    writer.Write(Encoding.ASCII.GetBytes(KmerWriter.Magic2))
                    scan0 = writer.Position
                Else
                    scan0 = indexCache.Values.Select(Function(p) p.offset + p.size).Max
                    scan0 += 4 ' pad a zero
                End If

                writer.Position = scan0

                For Each seed As KeyValuePair(Of String, KmerSeed) In cache
                    Dim ms As New MemoryStream
                    Dim buf As New BinaryDataWriter(ms) With {
                    .ByteOrder = ByteOrder.BigEndian
                }

                    scan0 = writer.Position

                    Call buf.Write(Encoding.ASCII.GetBytes(seed.Key))
                    Call buf.Write(seed.Value.source.Length)

                    For Each s In seed.Value.source
                        Call buf.Write(s.locations.Length + 1)
                        Call buf.Write(s.seqid)
                        Call buf.Write(s.locations)
                    Next

                    Call buf.Flush()

                    Dim buf_data = ms.ToArray
                    Dim buf_size As Integer = buf_data.Length

                    Call writer.Write(buf_size)
                    Call writer.Write(buf_data)
                    ' Call writer.Flush()

                    indexCache(seed.Key) = (scan0, buf_size)
                Next
            End Using
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    ' TODO: dispose managed state (managed objects)
                    Call FlushData()
                    Call WriteIndex()
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