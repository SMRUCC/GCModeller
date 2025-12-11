Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' in-memory database of the ncbi genbank assembly index data
''' </summary>
Public Class AssemblySummaryGenbank : Inherits GenomeNameIndex(Of GenBankAssemblyIndex)

    Dim accessionIndex As New Dictionary(Of String, GenBankAssemblyIndex)

    Sub New(qgram As Integer)
        Call MyBase.New(qgram)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByAccessionId(asm_id As String) As GenBankAssemblyIndex
        Return accessionIndex.TryGetValue(asm_id.Split("."c).First)
    End Function

    Public Function LoadIntoMemory(file As String) As AssemblySummaryGenbank
        Call MyBase.LoadDatabase(GenBankAssemblyIndex.LoadIndex(file))

        For Each asm As GenBankAssemblyIndex In Me.AsEnumerable
            accessionIndex(asm.assembly_accession.Split("."c).First) = asm
        Next

        Return Me
    End Function

End Class
