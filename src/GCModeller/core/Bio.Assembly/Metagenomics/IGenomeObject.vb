Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq

Namespace Metagenomics

    Public Interface IGenomeObject

        Property genome_name As String
        Property ncbi_taxid As UInteger

    End Interface

    Public Class GenomeNameIndex(Of T As IGenomeObject)

        ReadOnly qgram As QGramIndex
        ReadOnly pool As New List(Of T)

        Sub New(Optional qgram As Integer = 6)
            Me.qgram = New QGramIndex(qgram)
        End Sub

        Public Function LoadDatabase(data As IEnumerable(Of T))
            For Each genome As T In data.SafeQuery
                Call pool.Add(genome)
                Call qgram.AddString(genome.genome_name)
            Next

            Return Me
        End Function

        Public Iterator Function Query(name As String, Optional cutoff As Double = 0.8) As IEnumerable(Of (genome As T, match As FindResult))
            Dim offsets As FindResult() = qgram.FindSimilar(name, cutoff) _
                .OrderByDescending(Function(a) a.similarity) _
                .ToArray

            For i As Integer = 0 To offsets.Length - 1
                Yield (pool(offsets(i).index), offsets(i))
            Next
        End Function

        Public Function GetBestMatch(name As String, Optional cutoff As Double = 0.8, Optional ByRef match As FindResult = Nothing) As T
            match = qgram.FindSimilar(name, cutoff) _
                .OrderByDescending(Function(a) a.similarity) _
                .FirstOrDefault

            If match IsNot Nothing Then
                Return pool(match.index)
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace