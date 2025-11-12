Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' in-memory database of the ncbi genbank assembly index data
''' </summary>
Public Class AssemblySummaryGenbank : Inherits GenomeNameIndex(Of GenBankAssemblyIndex)

    Sub New(qgram As Integer)
        Call MyBase.New(qgram)
    End Sub

    Public Function LoadIntoMemory(file As String) As AssemblySummaryGenbank
        Return MyBase.LoadDatabase(GenBankAssemblyIndex.LoadIndex(file))
    End Function

End Class
