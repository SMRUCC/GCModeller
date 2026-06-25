Imports System.Runtime.CompilerServices

Namespace metaTraits.Traitar

    Public Module Export

        <Extension>
        Public Iterator Function ResultTable(predictions As IEnumerable(Of Modules.EnsembleVoting.VotingResult),
                                             allKeyFeatures As Dictionary(Of String, Modules.FeatureSelection.KeyFeature()),
                                             models As ModelLoader) As IEnumerable(Of ReportJSON)

            Dim predictionSet = predictions.ToDictionary(Function(p) p.PhenotypeId)

            For Each kvp As KeyValuePair(Of String, Models.PhenotypeModel) In models.Phenotypes
                Dim phenoId As String = kvp.Key
                Dim phenoModel As Models.PhenotypeModel = kvp.Value

                Dim result As Modules.EnsembleVoting.VotingResult = Nothing
                If predictionSet.ContainsKey(phenoId) Then
                    result = predictionSet(phenoId)
                End If

                Dim predStr As PredictionResults = PredictionResults.NA
                Dim confStr As Double = 0
                Dim labels As Integer() = {}
                Dim scores As Double() = {}

                If result IsNot Nothing Then
                    predStr = If(result.IsPositive, PredictionResults.TRUE, PredictionResults.FALSE)
                    confStr = result.Confidence
                    labels = result.ModelLabels.ToArray
                    scores = result.ModelScores.ToArray
                End If

                Dim keys As Modules.FeatureSelection.KeyFeature() = {}

                ' 关键特征
                If allKeyFeatures.ContainsKey(phenoId) Then
                    keys = allKeyFeatures(phenoId).ToArray
                End If

                Yield New ReportJSON With {.phenotypeId = phenoId,
                                             .accession = phenoModel.PhenotypeName,
                                            .category = phenoModel.Category,
                                              .predict = predStr, .confidence = confStr,
                                              .labels = labels,
                                              .scores = scores,
                                              .positive = result?.PositiveVotes,
                                              .negative = result?.NegativeVotes,
                                              .KeyFeatures = keys
                }
            Next
        End Function
    End Module
End Namespace