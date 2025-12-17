Imports System.Runtime.CompilerServices
Imports Darwinism.Repository.BucketDb
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' in-memory database of the ncbi genbank assembly index data
''' </summary>
Public Class AssemblySummaryGenbank : Inherits GenomeNameIndex(Of GenBankAssemblyIndex)

    Dim flash As Buckets

    Sub New(qgram As Integer, repo As String)
        Call MyBase.New(qgram)

        flash = New Buckets(database_dir:=repo, partitions:=8)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByAccessionId(asm_id As String) As GenBankAssemblyIndex
        Return BSONFormat.Load(flash.Get(asm_id.Split("."c).First)).CreateObject(Of GenBankAssemblyIndex)
    End Function

    Public Function LoadIntoMemory(file As String) As AssemblySummaryGenbank
        Call MyBase.LoadDatabase(GenBankAssemblyIndex.LoadIndex(file))

        For Each asm As GenBankAssemblyIndex In Me.AsEnumerable
            flash.Put(asm.assembly_accession.Split("."c).First, BSONFormat.GetBuffer(JSONSerializer.CreateJSONElement(asm)).ToArray)
        Next

        Return Me
    End Function

End Class
