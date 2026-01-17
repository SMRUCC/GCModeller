Imports System.Runtime.CompilerServices
Imports Darwinism.Repository.BucketDb
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' in-memory database of the ncbi genbank assembly index data
''' </summary>
Public Class AssemblySummaryGenbank : Inherits GenomeNameIndex(Of GenomeEntry)
    Implements IDisposable

    Dim flash As Buckets

    Private disposedValue As Boolean

    Sub New(qgram As Integer, repo As String, Optional in_memory As Boolean = True)
        Call Me.New(qgram, New Buckets(database_dir:=repo, buckets:=8, in_memory:=in_memory))
    End Sub

    Sub New(qgram As Integer, repo As Buckets)
        Call MyBase.New(qgram)

        flash = repo
        LoadDatabase(LoadTextSearch)
    End Sub

    Private Iterator Function LoadTextSearch() As IEnumerable(Of GenomeEntry)
        Dim buf As Byte() = flash.Get("genbank-entry")
        Dim list As JsonArray = BSONFormat.SafeLoadArrayList(buf)

        Call "load fulltext search index...".info

        For Each item As JsonElement In TqdmWrapper.Wrap(list.ToArray)
            Yield item.CreateObject(Of GenomeEntry)
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByAccessionId(asm_id As String) As GenBankAssemblyIndex
        Dim key As String = asm_id.Split("."c).First
        Dim buf As Byte() = flash.Get(key)

        If buf.IsNullOrEmpty Then
            Return Nothing
        Else
            Return BSONFormat.Load(buf).CreateObject(Of GenBankAssemblyIndex)
        End If
    End Function

    Public Shared Sub CreateRepository(file As String, repo As String, Optional qgram As Integer = 6)
        Dim memoryIndex As New List(Of GenomeEntry)
        Dim flash As New Buckets(repo, buckets:=8)

        For Each asm As GenBankAssemblyIndex In GenBankAssemblyIndex.LoadIndex(file)
            Dim key As String = asm.assembly_accession.Split("."c).First
            Dim bson As Byte() = BSONFormat.GetBuffer(JSONSerializer.CreateJSONElement(asm)).ToArray

            Call flash.Put(key, bson)
            Call memoryIndex.Add(New GenomeEntry With {
                .accession_id = asm.assembly_accession,
                .genome_name = asm.organism_name,
                .ncbi_taxid = asm.taxid
            })
        Next

        Call flash.Put("genbank-entry", BSONFormat.SafeGetBuffer(JSONSerializer.CreateJSONElement(memoryIndex.ToArray)).ToArray)
        Call flash.Flush()
        Call flash.Dispose()
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call flash.Dispose()
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

Public Class GenomeEntry : Implements IGenomeObject

    Public Property accession_id As String
    Public Property genome_name As String Implements IGenomeObject.genome_name
    Public Property ncbi_taxid As UInteger Implements IGenomeObject.ncbi_taxid

End Class