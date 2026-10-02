' ============================================================================
' PhenotypePredictor.vb
'
' 预测引擎（替代旧的 EnsembleVoting.vb）：
' 重构之后每一种表型只对应一个 SVM 模型实例，因此不再需要「多个 C 参数的
' 子模型投票委员会」，直接逐表型调用各自模型的 Prediction.Predict 即可。
' ============================================================================

Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString

Namespace metaTraits.Traitar.Modules

    ''' <summary>
    ''' 基因组表型预测器
    ''' </summary>
    Public Class PhenotypePredictor

        ''' <summary>模型仓库（含 Pfam 词表与编码配置）</summary>
        Public Property Loader As ModelLoader

        Sub New(loader As ModelLoader)
            Me.Loader = loader
        End Sub

        ''' <summary>
        ''' 从一个基因组的蛋白质组 Pfam 注释出发预测其全部表型
        ''' </summary>
        ''' <param name="proteins">该基因组所预测出来的蛋白质组 Pfam 注释</param>
        ''' <returns>每一种表型所对应的预测结果</returns>
        Public Iterator Function PredictGenome(proteins As IEnumerable(Of PfamString)) As IEnumerable(Of TraitPrediction)
            If Loader Is Nothing OrElse Loader.Embedding Is Nothing Then
                Return
            End If

            Dim profile As Dictionary(Of String, Double) = Loader.Embedding.EmbedProteins(proteins)

            For Each pred As TraitPrediction In PredictGenome(profile)
                Yield pred
            Next
        End Function

        ''' <summary>
        ''' 使用已经归一化之后的 Pfam 丰度向量预测其全部表型
        ''' </summary>
        ''' <param name="profile">Pfam 编号 -> 归一化丰度值</param>
        Public Iterator Function PredictGenome(profile As IDictionary(Of String, Double)) As IEnumerable(Of TraitPrediction)
            If Loader Is Nothing OrElse Loader.Embedding Is Nothing Then
                Return
            End If

            Dim nodes As Microsoft.VisualBasic.MachineLearning.SVM.Node() = Loader.Embedding.ToNodes(profile)

            For Each model As Models.PhenotypeModel In Loader.Models.Values
                Yield model.Predict(nodes)
            Next
        End Function

        ''' <summary>
        ''' 只预测某一种指定的表型
        ''' </summary>
        Public Function PredictTrait(trait_name As String,
                                     profile As IDictionary(Of String, Double)) As TraitPrediction

            Dim model As Models.PhenotypeModel = Loader.GetModel(trait_name)

            If model Is Nothing Then
                Return Nothing
            End If
            If Loader.Embedding Is Nothing Then
                Return Nothing
            End If

            Return model.Predict(profile, Loader.Embedding)
        End Function

        ''' <summary>
        ''' 只预测某一种指定的表型（直接传入蛋白质组 Pfam 注释）
        ''' </summary>
        Public Function PredictTrait(trait_name As String,
                                     proteins As IEnumerable(Of PfamString)) As TraitPrediction

            If Loader Is Nothing OrElse Loader.Embedding Is Nothing Then
                Return Nothing
            End If

            Return PredictTrait(trait_name, Loader.Embedding.EmbedProteins(proteins))
        End Function

        ''' <summary>
        ''' 预测全部表型，并按照置信度从高到低排序（回归模型排在最后）
        ''' </summary>
        Public Function PredictSorted(profile As IDictionary(Of String, Double)) As TraitPrediction()
            Dim list As List(Of TraitPrediction) = PredictGenome(profile).ToList

            list.Sort(Function(a, b)
                          If a Is Nothing Then Return 1
                          If b Is Nothing Then Return -1

                          Dim ra As Integer = If(a.IsRegression(), 1, 0)
                          Dim rb As Integer = If(b.IsRegression(), 1, 0)

                          If ra <> rb Then
                              Return ra.CompareTo(rb)
                          End If

                          Return -a.confidence.CompareTo(b.confidence)
                      End Function)

            Return list.ToArray
        End Function

    End Class
End Namespace
