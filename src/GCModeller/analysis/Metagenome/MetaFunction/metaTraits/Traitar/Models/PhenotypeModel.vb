' ============================================================================
' PhenotypeModel.vb - 表型预测模型数据结构
'
' 对应论文中的：
'   - L1正则化L2损失线性SVM模型
'   - 投票委员会机制（5个最佳SVM模型）
'   - 每个表型对应一组不同C参数的SVM模型
' ============================================================================

Namespace TraitarVB.Models

    ''' <summary>
    ''' 单个SVM子模型（对应一个C参数值）
    ''' </summary>
    Public Class SVMSubModel
        ''' <summary>正则化参数C</summary>
        Public Property C As Double

        ''' <summary>偏置项 b</summary>
        Public Property Bias As Double

        ''' <summary>特征权重字典：PfamID -> 权重值</summary>
        Public Property Weights As New Dictionary(Of String, Double)()

        ''' <summary>
        ''' 对样本进行预测，返回原始得分
        ''' 公式：score = bias + Σ(weight_i × feature_i)
        ''' </summary>
        Public Function PredictScore(features As Dictionary(Of String, Integer)) As Double
            Dim score As Double = Bias
            For Each kvp As KeyValuePair(Of String, Double) In Weights
                Dim featVal As Integer = 0
                If features.ContainsKey(kvp.Key) Then
                    featVal = features(kvp.Key)
                End If
                score += kvp.Value * CDbl(featVal)
            Next
            Return score
        End Function

        ''' <summary>
        ''' 对样本进行预测，返回标签（1=正，-1=负）
        ''' </summary>
        Public Function PredictLabel(features As Dictionary(Of String, Integer)) As Integer
            Dim score As Double = PredictScore(features)
            If score > 0 Then
                Return 1
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' 检查该模型是否有效（有非零权重或非零偏置）
        ''' </summary>
        Public ReadOnly Property IsActive As Boolean
            Get
                If Math.Abs(Bias) > 1e-12 Then Return True
                For Each w As Double In Weights.Values
                    If Math.Abs(w) > 1e-12 Then Return True
                Next
                Return False
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 表型预测模型（包含多个SVM子模型）
    ''' </summary>
    Public Class PhenotypeModel
        ''' <summary>表型ID</summary>
        Public Property PhenotypeId As String

        ''' <summary>表型名称</summary>
        Public Property PhenotypeName As String

        ''' <summary>表型类别</summary>
        Public Property Category As String

        ''' <summary>SVM子模型列表（每个对应一个C值）</summary>
        Public Property SubModels As New List(Of SVMSubModel)()

        ''' <summary>关键特征信息列表</summary>
        Public Property KeyFeatures As New List(Of KeyFeatureInfo)()

        ''' <summary>投票委员会大小</summary>
        Public Const COMMITTEE_SIZE As Integer = 5

        ''' <summary>多数表决阈值</summary>
        Public Const MAJORITY_THRESHOLD As Integer = 3

        ''' <summary>
        ''' 获取投票委员会
        ''' 论文：选出交叉验证中准确率最高的5个SVM模型
        ''' 由于模型文件中没有交叉验证准确率信息，这里使用所有活跃模型
        ''' （有非零偏置或非零权重的模型）作为投票委员会
        ''' </summary>
        Public Function GetVotingCommittee() As List(Of SVMSubModel)
            ' 筛选活跃模型
            Dim activeModels As New List(Of SVMSubModel)()
            For Each m As SVMSubModel In SubModels
                If m.IsActive Then
                    activeModels.Add(m)
                End If
            Next

            ' 使用所有活跃模型作为投票委员会
            Return activeModels
        End Function

        ''' <summary>
        ''' 比较两个模型的权重绝对值之和
        ''' </summary>
        Private Function CompareByWeightMagnitude(a As SVMSubModel, b As SVMSubModel) As Integer
            Dim sumA As Double = Math.Abs(a.Bias)
            For Each w As Double In a.Weights.Values
                sumA += Math.Abs(w)
            Next

            Dim sumB As Double = Math.Abs(b.Bias)
            For Each w As Double In b.Weights.Values
                sumB += Math.Abs(w)
            Next

            Return sumA.CompareTo(sumB)
        End Function

        ''' <summary>
        ''' 使用投票委员会进行预测
        ''' 论文：5个模型中至少有3个预测为正，则最终判定为表型存在
        ''' 由于使用所有活跃模型，多数表决阈值为半数以上
        ''' </summary>
        Public Function Predict(features As Dictionary(Of String, Integer)) As Integer
            Dim committee As List(Of SVMSubModel) = GetVotingCommittee()
            If committee.Count = 0 Then
                Return 0  ' 无可用模型，默认预测为负
            End If

            Dim positiveVotes As Integer = 0
            For Each m As SVMSubModel In committee
                If m.PredictLabel(features) = 1 Then
                    positiveVotes += 1
                End If
            Next

            ' 多数表决：超过半数即为阳性
            Dim threshold As Integer = committee.Count \ 2 + 1
            If positiveVotes >= threshold Then
                Return 1
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' 获取预测置信度（正票比例）
        ''' </summary>
        Public Function GetConfidence(features As Dictionary(Of String, Integer)) As Double
            Dim committee As List(Of SVMSubModel) = GetVotingCommittee()
            If committee.Count = 0 Then
                Return 0.0
            End If

            Dim positiveVotes As Integer = 0
            For Each m As SVMSubModel In committee
                If m.PredictLabel(features) = 1 Then
                    positiveVotes += 1
                End If
            Next
            Return CDbl(positiveVotes) / CDbl(committee.Count)
        End Function

    End Class

    ''' <summary>
    ''' 关键特征信息（来自non-zero+weights.txt的每一行）
    ''' </summary>
    Public Class KeyFeatureInfo
        ''' <summary>Pfam家族ID</summary>
        Public Property PfamId As String

        ''' <summary>类别（+或-）</summary>
        Public Property FeatureClass As String

        ''' <summary>各C值对应的权重</summary>
        Public Property WeightsByC As New Dictionary(Of Double, Double)()

        ''' <summary>Pfam描述</summary>
        Public Property Description As String

        ''' <summary>皮尔逊相关系数</summary>
        Public Property PearsonCorrelation As Double

        ''' <summary>该特征是否在多数模型（≥3）中拥有正权重</summary>
        Public ReadOnly Property IsMajorityPositive As Boolean
            Get
                Dim positiveCount As Integer = 0
                For Each w As Double In WeightsByC.Values
                    If w > 0 Then positiveCount += 1
                Next
                Return positiveCount >= 3
            End Get
        End Property
    End Class

End Namespace
