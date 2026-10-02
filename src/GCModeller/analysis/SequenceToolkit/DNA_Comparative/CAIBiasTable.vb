Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI.XML

Public Class CAIBiasTable

    Public Property SpeciesID As String
    Public Property CAI As Double
    Public Property BiasTable As Dictionary(Of String, Double)

    ''' <summary>
    ''' Build the codon usage csv table: one row per species (mean CAI of its genes
    ''' plus one column per codon weight).
    ''' </summary>
    ''' <param name="data">the compiled codon weight tables of the species</param>
    ''' <returns>the csv document</returns>
    Public Shared Iterator Function compileCaiTable(data As IEnumerable(Of (ID As String, MeanCAI As Double, Table As CodonAdaptationIndex))) As IEnumerable(Of CAIBiasTable)
        Dim csv As New IO.File
        Dim head As New IO.RowObject From {"SpeciesID", "CAI"}

        Call csv.Add(head)

        For Each bias In data.First.Table.GetCodonBiasList
            Call head.Add(bias.Value.CodonString)
        Next

        For Each item In data
            Dim row As New IO.RowObject From {item.ID, item.MeanCAI}
            Dim biasData = item.Table.GetCodonBiasList

            For i As Integer = 0 To biasData.Length - 1
                Call row.Add(biasData(i).Value.Bias)
            Next

            Call csv.Add(row)
        Next

        Return csv
    End Function
End Class
