Imports System.Runtime.CompilerServices
Imports Darwinism.Repository.BucketDb
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' in-memory database of the ncbi genbank assembly index data
''' </summary>
Public Class AssemblySummaryGenbank : Inherits GenomeNameIndex(Of GenomeEntry)

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
        Dim memoryIndex As New List(Of GenomeEntry)

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

        Call MyBase.LoadDatabase(memoryIndex)

        Return Me
    End Function

End Class

Public Class GenomeEntry : Implements IGenomeObject

    Public Property accession_id As String
    Public Property genome_name As String Implements IGenomeObject.genome_name
    Public Property ncbi_taxid As UInteger Implements IGenomeObject.ncbi_taxid

End Class