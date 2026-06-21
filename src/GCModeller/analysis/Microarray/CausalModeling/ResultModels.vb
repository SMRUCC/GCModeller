Public Class MeasurementModel

    Public Property latentName As String
    Public Property mode As MeasurementModels
    Public Property manifest_variable As String
    Public Property loading As Double
    Public Property w As Double
    Public Property communality As Double
    Public Property block_communality As Double

    Public Shared Iterator Function FromResult(result As PLSPMResult) As IEnumerable(Of MeasurementModel)
        For j = 0 To result.NumLatents - 1
            Dim lv = result.LatentDefs(j)
            Dim block = result.Communalities(j)

            For k = 0 To lv.featureIDs.Length - 1
                Dim mvName = lv.featureIDs(k)
                Dim load = result.Loadings(j)(k)
                Dim w = result.FinalOuterWeights(j)(k)
                Dim comm = load * load

                Yield New MeasurementModel With {
                    .block_communality = block,
                    .communality = comm,
                    .latentName = lv.varName,
                    .mode = lv.mode,
                    .loading = load,
                    .manifest_variable = mvName,
                    .w = w
                }
            Next
        Next
    End Function

End Class

Public Class EndogenousLatentVariable

    Public Property latentName As String
    Public Property r2 As Double
    Public Property communality As Double
    Public Property redundancy As Double

    Public Overrides Function ToString() As String
        Return $"{latentName,-25}{r2,12:F4}{communality,12:F4}{redundancy,12:F4}"
    End Function

    Public Shared Iterator Function FromResult(result As PLSPMResult) As IEnumerable(Of EndogenousLatentVariable)
        For j = 0 To result.NumLatents - 1
            Dim r2 = If(result.RSquared.ContainsKey(j), result.RSquared(j), 0.0)
            Dim comm = result.Communalities(j)
            Dim red = result.Redundancies(j)

            Yield New EndogenousLatentVariable With {
                .latentName = result.LatentNames(j),
                .r2 = r2,
                .communality = comm,
                .redundancy = red
            }
        Next
    End Function

End Class