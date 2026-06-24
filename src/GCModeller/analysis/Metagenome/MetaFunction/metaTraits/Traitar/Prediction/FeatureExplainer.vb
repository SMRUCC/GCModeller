' ============================================================================
' Module 4: Feature Selection & Association Explanation
' File: Prediction/FeatureExplainer.vb
'
' 功能: 识别对表型预测贡献最大的蛋白质家族（Pfam），提供生物学解释。
'       对应论文中 "反向关联: 识别决定表型的关键蛋白质家族"。
'
' 算法原理:
'   1. 多数特征选择: 在投票委员会的 5 个模型中，若某 Pfam 在至少 3 个模型中
'      拥有正权重，则认为该 Pfam 是决定该表型的重要特征。
'   2. 相关性排序: 利用 Pearson 相关系数（PCC）对选中的特征进行排序，
'      输出给用户作为后续实验验证的靶点。
' ============================================================================

Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Traitar.GenomeAnnotation
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Traitar.Models

Namespace Traitar.Prediction

    ''' <summary>
    ''' 关键特征（决定表型的 Pfam 家族）
    ''' </summary>
    Public Class KeyFeature
        ''' <summary>Pfam 家族 accession</summary>
        Public Property PfamAcc As String
        ''' <summary>Pfam 描述</summary>
        Public Property Description As String
        ''' <summary>类别（+ = 正相关, - = 负相关）</summary>
        Public Property [Class] As String
        ''' <summary>Pearson 相关系数</summary>
        Public Property PearsonCorrelation As Double
        ''' <summary>在投票委员会中被选中的次数（正权重次数）</summary>
        Public Property SelectedCount As Integer
        ''' <summary>投票委员会总模型数</summary>
        Public Property TotalModels As Integer
        ''' <summary>平均权重</summary>
        Public Property AverageWeight As Double
        ''' <summary>该 Pfam 是否在当前基因组中存在</summary>
        Public Property IsPresent As Boolean

        ''' <summary>是否为关键特征（SelectedCount >= MajorityThreshold）</summary>
        Public ReadOnly Property IsKeyFeature As Boolean
            Get
                Return SelectedCount >= 3 ' 多数阈值
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim presentStr = If(IsPresent, "[Present]", "[Absent]")
            Return $"{PfamAcc} {presentStr} class={Me.Class}, PCC={PearsonCorrelation:F3}, selected={SelectedCount}/{TotalModels}, avgW={AverageWeight:F4} | {Description}"
        End Function
    End Class

    ''' <summary>
    ''' 特征解释器
    ''' </summary>
    Public Class FeatureExplainer

        ''' <summary>多数阈值（论文中为 3/5）</summary>
        Public Property MajorityThreshold As Integer = 3

        ''' <summary>
        ''' 从投票委员会中提取关键特征
        ''' </summary>
        ''' <param name="phenotypeModel">表型模型</param>
        ''' <param name="committeeModelIds">投票委员会的模型ID列表</param>
        ''' <param name="profile">基因组特征向量（用于判断 Pfam 是否存在）</param>
        Public Function ExplainKeyFeatures(phenotypeModel As PhenotypeModel,
                                           committeeModelIds As List(Of String),
                                           profile As PhyleticProfile) As List(Of KeyFeature)
            Dim features As New List(Of KeyFeature)

            ' 遍历 non-zero+weights 中的所有特征
            For Each nzFeat In phenotypeModel.NonZeroFeatures
                Dim kf As New KeyFeature With {
                    .PfamAcc = nzFeat.PfamAcc,
                    .Description = nzFeat.Description,
                    .[Class] = nzFeat.[Class],
                    .PearsonCorrelation = nzFeat.PearsonCorrelation,
                    .TotalModels = committeeModelIds.Count,
                    .IsPresent = profile.HasPfam(nzFeat.PfamAcc)
                }

                ' 统计在投票委员会中拥有正权重的次数
                Dim positiveCount = 0
                Dim weightSum = 0.0
                Dim weightCount = 0
                For Each mid As String In committeeModelIds
                    If nzFeat.Weights.ContainsKey(mid) Then
                        Dim w = nzFeat.Weights(mid)
                        If w > 0 Then
                            positiveCount += 1
                            weightSum += w
                            weightCount += 1
                        End If
                    End If
                Next
                kf.SelectedCount = positiveCount
                kf.AverageWeight = If(weightCount > 0, weightSum / weightCount, 0.0)

                features.Add(kf)
            Next

            ' 按 Pearson 相关系数降序排序
            features.Sort(Function(a, b) b.PearsonCorrelation.CompareTo(a.PearsonCorrelation))

            Return features
        End Function

        ''' <summary>
        ''' 仅返回关键特征（满足多数阈值的特征）
        ''' </summary>
        Public Function GetKeyFeaturesOnly(features As List(Of KeyFeature)) As List(Of KeyFeature)
            Return features.FindAll(Function(f) f.IsKeyFeature)
        End Function

        ''' <summary>
        ''' 生成特征解释报告
        ''' </summary>
        Public Function GenerateReport(phenotypeId As String,
                                       phenotypeName As String,
                                       features As List(Of KeyFeature)) As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("=" & New String("="c, 70))
            sb.AppendLine($"特征解释报告: 表型 [{phenotypeId}] {phenotypeName}")
            sb.AppendLine("=" & New String("="c, 70))
            sb.AppendLine()

            Dim keyFeats = GetKeyFeaturesOnly(features)
            If keyFeats.Count = 0 Then
                sb.AppendLine("  (无满足多数阈值的关键特征)")
            Else
                sb.AppendLine($"关键特征（在 ≥{MajorityThreshold} 个模型中被选中）:")
                sb.AppendLine()
                sb.AppendLine($"  {"Pfam",-12} {"Class",-6} {"PCC",-8} {"Sel",-6} {"AvgW",-10} {"Present",-8} Description")
                sb.AppendLine($"  " & New String("-"c, 68))
                For Each F As KeyFeature In keyFeats
                    sb.AppendLine($"  {F.PfamAcc,-12} {F.Class,-6} {F.PearsonCorrelation,-8:F3} " &
                                  $"{F.SelectedCount & "/" & F.TotalModels,-6} {F.AverageWeight,-10:F4} " &
                                  $"{If(F.IsPresent, "Yes", "No"),-8} {F.Description}")
                Next
            End If

            sb.AppendLine()
            sb.AppendLine($"所有正权重特征（按 PCC 降序）:")
            sb.AppendLine()
            For Each F As KeyFeature In features
                Dim marker = If(F.IsKeyFeature, " *", "  ")
                sb.AppendLine($"{marker} {F.ToString()}")
            Next

            Return sb.ToString()
        End Function
    End Class
End Namespace
