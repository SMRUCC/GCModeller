' ============================================================================
' Module 2: Traitar Model Loader
' File: Models/TraitarModel.vb
'
' 功能: 加载 Traitar (phypat+PGL) 模型文件，包括:
'       - pt2acc.txt: 表型ID -> 表型名称 + 类别
'       - pf2acc_desc.txt: Pfam 家族ID -> 描述
'       - {phenotype_id}_bias.txt: 各 C 值的偏置
'       - {phenotype_id}_feats.txt: 各 Pfam 家族在各 C 值下的权重
'       - {phenotype_id}_non-zero+weights.txt: 非零权重 + 类别 + Pearson 相关系数
'
' 论文对应: 每个表型对应一组 L1正则化 L2损失线性 SVM 模型（不同 C 值），
'           通过交叉验证选出准确率最高的 5 个模型组成投票委员会。
' ============================================================================

Imports System.IO

Namespace Traitar.Models

    ''' <summary>
    ''' 表型信息（来自 pt2acc.txt）
    ''' </summary>
    Public Class PhenotypeInfo
        ''' <summary>表型ID（如 8567）</summary>
        Public Property Id As String
        ''' <summary>表型名称（如 Coagulase production）</summary>
        Public Property Accession As String
        ''' <summary>表型类别（如 Enzyme / Growth / Morphology）</summary>
        Public Property Category As String

        Public Overrides Function ToString() As String
            Return $"[{Id}] {Accession} ({Category})"
        End Function
    End Class

    ''' <summary>
    ''' Pfam 家族描述（来自 pf2acc_desc.txt）
    ''' </summary>
    Public Class PfamDescription
        Public Property PfamAcc As String
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return $"[{PfamAcc}] {Description}"
        End Function
    End Class

    ''' <summary>
    ''' 单个 SVM 模型（对应一个 C 值）
    ''' </summary>
    Public Class SvmModel
        ''' <summary>模型ID（如 "0.5_0"，表示 C=0.5 的第0个模型）</summary>
        Public Property ModelId As String
        ''' <summary>正则化参数 C</summary>
        Public Property C As Double
        ''' <summary>偏置项 b</summary>
        Public Property Bias As Double
        ''' <summary>权重向量: PfamAcc -> weight</summary>
        Public Property Weights As New Dictionary(Of String, Double)

        Public Overrides Function ToString() As String
            Return $"SVM(C={C}, bias={Bias:F4}, #weights={Weights.Count})"
        End Function
    End Class

    ''' <summary>
    ''' 非零权重特征（来自 non-zero+weights 文件，用于特征解释）
    ''' </summary>
    Public Class NonZeroFeature
        ''' <summary>Pfam 家族 accession</summary>
        Public Property PfamAcc As String
        ''' <summary>类别（+ = 正相关, - = 负相关）</summary>
        Public Property [Class] As String
        ''' <summary>各模型下的权重: ModelId -> weight</summary>
        Public Property Weights As New Dictionary(Of String, Double)
        ''' <summary>Pfam 描述</summary>
        Public Property Description As String
        ''' <summary>Pearson 相关系数</summary>
        Public Property PearsonCorrelation As Double

        Public Overrides Function ToString() As String
            Return $"[{PfamAcc}] class={Me.Class}, PCC={PearsonCorrelation:F3}, {Description}"
        End Function
    End Class

    ''' <summary>
    ''' 一个表型的完整模型（包含多个 C 值的 SVM 模型）
    ''' </summary>
    Public Class PhenotypeModel
        ''' <summary>表型ID</summary>
        Public Property PhenotypeId As String
        ''' <summary>表型信息</summary>
        Public Property Info As PhenotypeInfo
        ''' <summary>所有 SVM 模型: ModelId -> model</summary>
        Public Property Models As New Dictionary(Of String, SvmModel)
        ''' <summary>非零权重特征列表（用于特征解释）</summary>
        Public Property NonZeroFeatures As New List(Of NonZeroFeature)
        ''' <summary>所有 C 值列表（按模型文件中的顺序）</summary>
        Public Property CValues As New List(Of Double)
        ''' <summary>所有模型ID列表（如 "0.5_0", "1_0" 等）</summary>
        Public Property ModelIds As New List(Of String)

        Public Overrides Function ToString() As String
            Return $"PhenotypeModel[{PhenotypeId}] {If(Info IsNot Nothing, Info.Accession, "")}, #models={Models.Count}"
        End Function
    End Class

    ''' <summary>
    ''' Traitar 模型加载器
    ''' </summary>
    Public Class TraitarModelLoader

        ''' <summary>
        ''' 加载表型列表 (pt2acc.txt)
        ''' 格式: phenotype_id \t accession \t category
        ''' </summary>
        Public Shared Function LoadPhenotypes(filePath As String) As Dictionary(Of String, PhenotypeInfo)
            Dim result As New Dictionary(Of String, PhenotypeInfo)
            For Each line As String In File.ReadAllLines(filePath)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim fields = line.Split(ControlChars.Tab)
                If fields.Length < 3 Then Continue For
                If fields(0) = "" Then Continue For ' 跳过 header 行

                Dim id = fields(0).Trim()
                Dim info As New PhenotypeInfo With {
                    .Id = id,
                    .Accession = fields(1).Trim(),
                    .Category = fields(2).Trim()
                }
                result(id) = info
            Next
            Return result
        End Function

        ''' <summary>
        ''' 加载 Pfam 描述 (pf2acc_desc.txt)
        ''' 格式: pfam_acc \t description
        ''' </summary>
        Public Shared Function LoadPfamDescriptions(filePath As String) As Dictionary(Of String, String)
            Dim result As New Dictionary(Of String, String)
            For Each line As String In File.ReadAllLines(filePath)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim fields = line.Split(ControlChars.Tab)
                If fields.Length < 2 Then Continue For
                If fields(0) = "" Then Continue For ' 跳过 header 行

                Dim acc = fields(0).Trim()
                Dim desc = fields(1).Trim()
                result(acc) = desc
            Next
            Return result
        End Function

        ''' <summary>
        ''' 加载偏置文件 ({phenotype_id}_bias.txt)
        ''' 格式: C_value \t bias
        ''' 同时返回归一化的 modelId 列表（如 "0.5_0", "1_0"）
        ''' </summary>
        Public Shared Function LoadBiasWithModelIds(filePath As String,
                                                     ByRef modelIds As List(Of String)) _
            As Dictionary(Of String, Double)
            Dim result As New Dictionary(Of String, Double)
            modelIds = New List(Of String)
            For Each line As String In File.ReadAllLines(filePath)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim fields = line.Split(ControlChars.Tab)
                If fields.Length < 2 Then Continue For

                Dim [cStr] = fields(0).Trim()
                Dim b As Double
                If Not Double.TryParse([cStr], b) Then Continue For
                Dim biasStr = fields(1).Trim()
                Dim bias As Double
                If Not Double.TryParse(biasStr, bias) Then Continue For

                ' 归一化 C 值字符串，确保与 feats/non-zero+weights 文件一致
                Dim normalizedC = NormalizeC([cStr])
                Dim modelId = normalizedC & "_0"
                If Not modelIds.Contains(modelId) Then
                    modelIds.Add(modelId)
                End If
                result(modelId) = bias
            Next
            Return result
        End Function

        ''' <summary>
        ''' 归一化 C 值字符串
        ''' 例如: "1.0" -> "1", "0.006999999999999999" -> "0.007"
        ''' </summary>
        Private Shared Function NormalizeC([cStr] As String) As String
            Dim c As Double
            If Not Double.TryParse([cStr], c) Then Return [cStr]
            If c = CInt(c) Then
                Return CStr(CInt(c))
            Else
                ' 使用 G 格式消除浮点数表示差异
                Return c.ToString("G")
            End If
        End Function

        ''' <summary>
        ''' 加载特征权重文件 ({phenotype_id}_feats.txt)
        ''' 格式: header 行: \t modelId1 \t modelId2 ...
        '''        data 行:   PFxxxxx \t w1 \t w2 ...
        ''' </summary>
        Public Shared Function LoadFeatureWeights(filePath As String,
                                                   ByRef modelIds As List(Of String)) _
            As Dictionary(Of String, Dictionary(Of String, Double))

            Dim result As New Dictionary(Of String, Dictionary(Of String, Double))
            modelIds = New List(Of String)
            Dim isFirstLine = True

            For Each line As String In File.ReadAllLines(filePath)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim fields = line.Split(ControlChars.Tab)

                If isFirstLine Then
                    ' header 行: 第1列为空，后续为模型ID
                    For i = 1 To fields.Length - 1
                        Dim mid = fields(i).Trim()
                        If mid <> "" Then modelIds.Add(mid)
                    Next
                    isFirstLine = False
                    Continue For
                End If

                If fields.Length < 1 Then Continue For
                Dim pfamAcc = fields(0).Trim()
                If pfamAcc = "" Then Continue For

                Dim weights As New Dictionary(Of String, Double)
                For i = 1 To Math.Min(fields.Length - 1, modelIds.Count)
                    Dim w As Double
                    If Double.TryParse(fields(i).Trim(), w) Then
                        weights(modelIds(i - 1)) = w
                    End If
                Next
                result(pfamAcc) = weights
            Next

            Return result
        End Function

        ''' <summary>
        ''' 加载非零权重文件 ({phenotype_id}_non-zero+weights.txt)
        ''' 格式: header 行: \t class \t modelId1 \t ... \t Pfam_desc \t cor
        '''        data 行:   PFxxxxx \t +/- \t w1 \t ... \t description \t PCC
        ''' </summary>
        Public Shared Function LoadNonZeroWeights(filePath As String,
                                                   ByRef modelIds As List(Of String)) _
            As List(Of NonZeroFeature)

            Dim result As New List(Of NonZeroFeature)
            modelIds = New List(Of String)
            Dim isFirstLine = True
            Dim descColIdx = -1
            Dim corColIdx = -1

            For Each line As String In File.ReadAllLines(filePath)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim fields = line.Split(ControlChars.Tab)

                If isFirstLine Then
                    ' 解析 header，找到 Pfam_desc 和 cor 列的位置
                    For i = 0 To fields.Length - 1
                        Dim h = fields(i).Trim()
                        If h = "Pfam_desc" Then
                            descColIdx = i
                        ElseIf h = "cor" Then
                            corColIdx = i
                        ElseIf h <> "" AndAlso h <> "class" Then
                            modelIds.Add(h)
                        End If
                    Next
                    isFirstLine = False
                    Continue For
                End If

                If fields.Length < 2 Then Continue For
                Dim pfamAcc = fields(0).Trim()
                If pfamAcc = "" Then Continue For

                Dim feat As New NonZeroFeature With {
                    .PfamAcc = pfamAcc,
                    .[Class] = fields(1).Trim()
                }

                ' 加载各模型权重（从第3列开始，到 descColIdx 之前）
                Dim weightStartIdx = 2
                Dim weightEndIdx = If(descColIdx > 0, descColIdx - 1, fields.Length - 1)
                Dim midIdx = 0
                For i = weightStartIdx To Math.Min(weightEndIdx, fields.Length - 1)
                    If midIdx < modelIds.Count Then
                        Dim w As Double
                        If Double.TryParse(fields(i).Trim(), w) Then
                            feat.Weights(modelIds(midIdx)) = w
                        End If
                        midIdx += 1
                    End If
                Next

                ' 加载描述
                If descColIdx > 0 AndAlso descColIdx < fields.Length Then
                    feat.Description = fields(descColIdx).Trim()
                End If

                ' 加载 Pearson 相关系数
                If corColIdx > 0 AndAlso corColIdx < fields.Length Then
                    Dim cor As Double
                    If Double.TryParse(fields(corColIdx).Trim(), cor) Then
                        feat.PearsonCorrelation = cor
                    End If
                End If

                result.Add(feat)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 加载单个表型的完整模型
        ''' </summary>
        Public Shared Function LoadPhenotypeModel(modelDir As String,
                                                   phenotypeId As String,
                                                   info As PhenotypeInfo) As PhenotypeModel
            Dim model As New PhenotypeModel With {
                .PhenotypeId = phenotypeId,
                .Info = info
            }

            ' 1. 加载偏置
            Dim biasPath = Path.Combine(modelDir, $"{phenotypeId}_bias.txt")
            Dim biasMap = LoadBiasWithModelIds(biasPath, model.ModelIds)
            model.CValues = model.ModelIds.ConvertAll(Function(mid)
                                                          Dim [cStr] = mid.Split("_"c)(0)
                                                          Dim c As Double
                                                          Double.TryParse([cStr], c)
                                                          Return c
                                                      End Function).Distinct().ToList()

            ' 2. 加载特征权重 (feats.txt)
            Dim featsPath = Path.Combine(modelDir, $"{phenotypeId}_feats.txt")
            Dim featsModelIds As List(Of String) = Nothing
            Dim featWeights = LoadFeatureWeights(featsPath, featsModelIds)

            ' 构建 SVM 模型（使用 bias 文件中的 modelIds）
            For Each mid As String In model.ModelIds
                Dim [cStr] = mid.Split("_"c)(0)
                Dim c As Double

                Call Double.TryParse([cStr], c)

                Dim svm As New SvmModel With {
                    .ModelId = mid,
                    .C = c,
                    .Bias = If(biasMap.ContainsKey(mid), biasMap(mid), 0.0)
                }

                ' 填充 feats.txt 权重
                For Each kv In featWeights
                    If kv.Value.ContainsKey(mid) Then
                        svm.Weights(kv.Key) = kv.Value(mid)
                    End If
                Next

                model.Models(mid) = svm
            Next

            ' 3. 加载 non-zero+weights 并合并到 SVM 模型中
            Dim nzPath = Path.Combine(modelDir, $"{phenotypeId}_non-zero+weights.txt")
            If File.Exists(nzPath) Then
                Dim nzModelIds As List(Of String) = Nothing
                model.NonZeroFeatures = LoadNonZeroWeights(nzPath, nzModelIds)

                ' 将 non-zero+weights 中的权重合并到 SVM 模型
                For Each nzFeat In model.NonZeroFeatures
                    For Each wKv In nzFeat.Weights
                        Dim mid = wKv.Key
                        Dim w = wKv.Value
                        If model.Models.ContainsKey(mid) Then
                            ' 仅当 feats.txt 中该权重为 0 或不存在时才更新
                            If Not model.Models(mid).Weights.ContainsKey(nzFeat.PfamAcc) OrElse
                               model.Models(mid).Weights(nzFeat.PfamAcc) = 0.0 Then
                                model.Models(mid).Weights(nzFeat.PfamAcc) = w
                            End If
                        Else
                            ' 若模型不存在则创建
                            Dim [cStr] = mid.Split("_"c)(0)
                            Dim c As Double
                            Double.TryParse([cStr], c)
                            Dim svm As New SvmModel With {
                                .ModelId = mid,
                                .C = c,
                                .Bias = If(biasMap.ContainsKey(mid), biasMap(mid), 0.0)
                            }
                            svm.Weights(nzFeat.PfamAcc) = w
                            model.Models(mid) = svm
                        End If
                    Next
                Next
            End If

            Return model
        End Function

        ''' <summary>
        ''' 加载所有表型模型
        ''' </summary>
        Public Shared Function LoadAllModels(modelDir As String) As Dictionary(Of String, PhenotypeModel)
            Dim result As New Dictionary(Of String, PhenotypeModel)

            ' 加载表型列表
            Dim pt2accPath = Path.Combine(modelDir, "pt2acc.txt")
            If Not File.Exists(pt2accPath) Then
                Throw New FileNotFoundException($"pt2acc.txt not found in {modelDir}")
            End If
            Dim phenotypes = LoadPhenotypes(pt2accPath)

            ' 为每个表型加载模型
            For Each kv In phenotypes
                Dim biasPath = Path.Combine(modelDir, $"{kv.Key}_bias.txt")
                If File.Exists(biasPath) Then
                    result(kv.Key) = LoadPhenotypeModel(modelDir, kv.Key, kv.Value)
                End If
            Next

            Return result
        End Function
    End Class
End Namespace
