' ============================================================================
' Export.vb
'
' 把表型预测结果导出为 JSON 报告表。
' ============================================================================

Imports System.Runtime.CompilerServices

Namespace metaTraits.Traitar

    Public Module Export

        ''' <summary>
        ''' 把表型预测结果合并上模型的元数据，生成最终的报告表
        ''' </summary>
        ''' <param name="predictions">表型预测结果</param>
        ''' <param name="models">模型仓库，用于补充交叉验证得分与关键特征</param>
        <Extension>
        Public Iterator Function ResultTable(predictions As IEnumerable(Of TraitPrediction),
                                             models As ModelLoader) As IEnumerable(Of ReportJSON)

            For Each pred As TraitPrediction In predictions
                Yield ToReport(pred, models)
            Next
        End Function

        ''' <summary>
        ''' 把单条表型预测结果转换为报告记录
        ''' </summary>
        Public Function ToReport(prediction As TraitPrediction, models As ModelLoader) As ReportJSON
            Dim report As New ReportJSON With {
                .phenotypeId = prediction.trait_name,
                .accession = prediction.trait_name,
                .category = $"{prediction.group_1}/{prediction.group_2}",
                .unit = prediction.unit,
                .data_type = prediction.data_type,
                .predict = prediction.predict,
                .result = prediction.GetBooleanResult(),
                .confidence = prediction.confidence,
                .score = prediction.score,
                .votes = prediction.votes,
                .status = prediction.status
            }

            If models IsNot Nothing Then
                Dim model As Models.PhenotypeModel = models.GetModel(prediction.trait_name)

                If model IsNot Nothing Then
                    report.cvScore = model.CVScore
                    report.sampleCount = model.SampleCount
                    report.KeyFeatures = model.KeyFeatures
                End If
            End If

            Return report
        End Function

    End Module
End Namespace
