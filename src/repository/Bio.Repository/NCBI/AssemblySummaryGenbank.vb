Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

''' <summary>
''' in-memory database of the ncbi genbank assembly index data
''' </summary>
Public Class AssemblySummaryGenbank

    ReadOnly pool As New List(Of GenBankAssemblyIndex)

    Public ReadOnly Property organism_name As QGramIndex

    Default Public ReadOnly Property Assembly(q As FindResult) As GenBankAssemblyIndex
        Get
            Return pool(q.index)
        End Get
    End Property

    Sub New(qgram As Integer)
        organism_name = New QGramIndex(qgram)
    End Sub

    Public Function Query(name As String, Optional cutoff As Double = 0.85) As FindResult()
        Return organism_name.FindSimilar(name, cutoff).ToArray
    End Function

    Public Function LoadIntoMemory(file As String) As AssemblySummaryGenbank
        For Each asm As GenBankAssemblyIndex In GenBankAssemblyIndex.LoadIndex(file)
            Call pool.Add(asm)
            Call organism_name.AddString(asm.organism_name)
        Next

        Return Me
    End Function

End Class
