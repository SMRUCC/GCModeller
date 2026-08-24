#Region "Microsoft.VisualBasic::89a62d15ca9ad989bc6ac040e88998d8, analysis\Metagenome\MetaFunction\metaTraits\Traitar\Export.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 55
    '    Code Lines: 43 (78.18%)
    ' Comment Lines: 1 (1.82%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (20.00%)
    '     File Size: 2.55 KB


    '     Module Export
    ' 
    '         Function: ResultTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
