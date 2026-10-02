Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI.XML

Public Class CAIBiasTable

    Public Property SpeciesID As String
    Public Property CAI As Double
    Public Property BiasTable As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return $"{SpeciesID}: CAI = {CAI}"
    End Function

    ''' <summary>
    ''' Build the codon usage csv table: one row per species (mean CAI of its genes
    ''' plus one column per codon weight).
    ''' </summary>
    ''' <param name="data">the compiled codon weight tables of the species</param>
    ''' <returns>the csv document</returns>
    Public Shared Iterator Function compileCaiTable(data As IEnumerable(Of (ID As String, MeanCAI As Double, Table As CodonAdaptationIndex))) As IEnumerable(Of CAIBiasTable)
        For Each row As (ID As String, MeanCAI As Double, Table As CodonAdaptationIndex) In data
            Dim biasData As Dictionary(Of String, Double) = row.Table _
                .GetCodonBiasList _
                .ToDictionary(Function(a) CStr(a.Key),
                              Function(a)
                                  Return a.Value.Bias
                              End Function)
            Dim spec As New CAIBiasTable With {
                .SpeciesID = row.ID,
                .CAI = row.MeanCAI,
                .BiasTable = biasData
            }

            Yield spec
        Next
    End Function
End Class
