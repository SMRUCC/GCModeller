Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Kmers

    Public Class KmersDatabase : Implements IDisposable, DatabaseReader

        Private disposedValue As Boolean

        ReadOnly seqs As SequenceSource()
        ReadOnly shardings As New Dictionary(Of String, ShardingReader)

        Public Const prefixSize As Integer = 3

        Public ReadOnly Property k As Integer Implements DatabaseReader.k

        Sub New(database_dir As String)
            Dim config As Dictionary(Of String, String) = $"{database_dir}/config.txt".LoadJsonFile(Of Dictionary(Of String, String))

            seqs = $"{database_dir}/seq_ids.csv".LoadCsv(Of SequenceSource).OrderBy(Function(a) a.id).ToArray
            k = config!k

            For Each subdir As String In $"{database_dir}/data/".ListDirectory(SearchOption.SearchTopLevelOnly)
                Call shardings.Add(subdir.BaseName, New ShardingReader(subdir))
            Next
        End Sub

        ''' <summary>
        ''' try to load all data into memory, make fast debug test for the small database
        ''' </summary>
        ''' <returns></returns>
        Public Function LoadIntoMemory() As KmersDatabase
            For Each part As ShardingReader In shardings.Values
                Call part.LoadIntoMemory()
            Next

            Return Me
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function SequenceToTaxonomyId(seqid As UInteger) As UInteger Implements DatabaseReader.SequenceToTaxonomyId
            Return seqs(seqid - 1).ncbi_taxid
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function SequenceInfomation(seqid As UInteger) As SequenceSource Implements DatabaseReader.SequenceInfomation
            Return seqs(seqid - 1)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="kmer"></param>
        ''' <returns>
        ''' return nothing means target kmer is not exists inside the database.
        ''' </returns>
        Public Function GetKmer(kmer As String, Optional loadLocis As Boolean = False) As KmerSeed Implements DatabaseReader.GetKmer
            Dim prefixKey As String = kmer.Substring(3, length:=prefixSize)

            ' target kmer is not exists in database
            If Not shardings.ContainsKey(prefixKey) Then
                Return Nothing
            End If

            Return shardings(prefixKey).GetKmer(kmer)
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    For Each part As ShardingReader In shardings.Values
                        Call part.Dispose()
                    Next

                    Call shardings.Clear()
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