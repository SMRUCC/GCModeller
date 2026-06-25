' ============================================================================
' ModelLoader.vb - 模型文件加载器
'
' 负责加载models目录下的所有模型文件：
'   pt2acc.txt              - 表型ID到名称的映射
'   pf2acc_desc.txt         - Pfam家族描述
'   {pheno_id}_bias.txt     - 各C值对应的偏置项
'   {pheno_id}_feats.txt    - 完整特征权重矩阵
'   {pheno_id}_non-zero+weights.txt - 非零特征权重及PCC
' ============================================================================

Imports System.IO

Namespace TraitarVB

    ''' <summary>
    ''' 模型文件加载器
    ''' </summary>
    Public Class ModelLoader

        Friend ReadOnly _modelsDir As String

        ''' <summary>表型模型字典（表型ID -> PhenotypeModel）</summary>
        Public Property Phenotypes As New Dictionary(Of String, Models.PhenotypeModel)()

        ''' <summary>Pfam描述字典（PfamID -> 描述）</summary>
        Public Property PfamDescriptions As New Dictionary(Of String, String)()

        ''' <summary>表型数量</summary>
        Public ReadOnly Property PhenotypeCount As Integer
            Get
                Return Phenotypes.Count
            End Get
        End Property

        Public Sub New(modelsDir As String)
            _modelsDir = modelsDir
        End Sub

        Public Function LoadPhenotypeSVM() As Dictionary(Of String, Modules.SVMClassifier.SVMModel())
            ' 构建表型ID -> 模型列表的映射
            Dim phenotypeModels As New Dictionary(Of String, Modules.SVMClassifier.SVMModel())
            For Each kvp As KeyValuePair(Of String, Models.PhenotypeModel) In Phenotypes
                Dim phenoId As String = kvp.Key
                Dim phenoModel As Models.PhenotypeModel = kvp.Value

                ' 将PhenotypeModel转换为SVMModel列表
                Dim svmModels As New List(Of Modules.SVMClassifier.SVMModel)
                For Each subModel As Models.SVMSubModel In phenoModel.SubModels
                    Dim svmModel As New Modules.SVMClassifier.SVMModel()
                    svmModel.C = subModel.C
                    svmModel.Bias = subModel.Bias
                    svmModel.FeatureIds = New List(Of String)(subModel.Weights.Keys)
                    svmModel.Weights = New Double(subModel.Weights.Count - 1) {}
                    Dim idx As Integer = 0
                    For Each wKvp As KeyValuePair(Of String, Double) In subModel.Weights
                        svmModel.FeatureIds(idx) = wKvp.Key
                        svmModel.Weights(idx) = wKvp.Value
                        idx += 1
                    Next
                    svmModels.Add(svmModel)
                Next

                phenotypeModels(phenoId) = svmModels.ToArray
            Next

            Return phenotypeModels
        End Function

        ''' <summary>
        ''' 加载所有模型文件
        ''' </summary>
        Public Function LoadAll() As ModelLoader
            Console.WriteLine("[ModelLoader] 从目录加载模型: " & _modelsDir)

            ' 1. 加载表型描述
            Dim pt2accPath As String = Path.Combine(_modelsDir, "pt2acc.txt")
            If File.Exists(pt2accPath) Then
                LoadPhenotypeDescriptions(pt2accPath)
            Else
                Console.WriteLine("[ModelLoader] 警告: 未找到 pt2acc.txt")
            End If

            ' 2. 加载Pfam描述
            Dim pf2accPath As String = Path.Combine(_modelsDir, "pf2acc_desc.txt")
            If File.Exists(pf2accPath) Then
                LoadPfamDescriptions(pf2accPath)
            Else
                Console.WriteLine("[ModelLoader] 警告: 未找到 pf2acc_desc.txt")
            End If

            ' 3. 加载各表型的模型文件
            LoadAllPhenotypeModels()

            Console.WriteLine("[ModelLoader] 加载完成: {0} 个表型, {1} 个Pfam描述",
                              Phenotypes.Count, PfamDescriptions.Count)

            Return Me
        End Function

        ''' <summary>
        ''' 加载表型描述文件 pt2acc.txt
        ''' 格式: pheno_id \t pheno_name \t pheno_category
        ''' </summary>
        Private Sub LoadPhenotypeDescriptions(filePath As String)
            Dim lines As String() = File.ReadAllLines(filePath)

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then Continue For
                If line.StartsWith("#") Then Continue For

                Dim parts As String() = line.Split(New Char() {ControlChars.Tab, " "c},
                                                    StringSplitOptions.RemoveEmptyEntries)
                If parts.Length < 2 Then Continue For

                Dim phenoId As String = parts(0).Trim()
                Dim phenoName As String = parts(1).Trim()
                Dim phenoCategory As String = If(parts.Length >= 3, parts(2).Trim(), "")

                Dim model As New Models.PhenotypeModel()
                model.PhenotypeId = phenoId
                model.PhenotypeName = phenoName
                model.Category = phenoCategory

                Phenotypes(phenoId) = model
            Next

            Console.WriteLine("[ModelLoader] 加载表型描述: {0} 个", Phenotypes.Count)
        End Sub

        ''' <summary>
        ''' 加载Pfam描述文件 pf2acc_desc.txt
        ''' 格式: pfam_id \t description
        ''' </summary>
        Private Sub LoadPfamDescriptions(filePath As String)
            Dim lines As String() = File.ReadAllLines(filePath)

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then Continue For
                If line.StartsWith("#") Then Continue For

                Dim parts As String() = line.Split(New Char() {ControlChars.Tab, " "c}, 2)
                If parts.Length < 2 Then Continue For

                Dim pfamId As String = parts(0).Trim()
                Dim desc As String = parts(1).Trim()

                PfamDescriptions(pfamId) = desc
            Next

            Console.WriteLine("[ModelLoader] 加载Pfam描述: {0} 个", PfamDescriptions.Count)
        End Sub

        ''' <summary>
        ''' 加载所有表型的模型文件
        ''' </summary>
        Private Sub LoadAllPhenotypeModels()
            Dim loadedCount As Integer = 0

            For Each phenoId As String In New List(Of String)(Phenotypes.Keys)
                Dim biasFile As String = Path.Combine(_modelsDir, phenoId & "_bias.txt")
                Dim featsFile As String = Path.Combine(_modelsDir, phenoId & "_feats.txt")

                If File.Exists(biasFile) AndAlso File.Exists(featsFile) Then
                    Try
                        LoadPhenotypeModel(phenoId, biasFile, featsFile)
                        loadedCount += 1
                    Catch ex As Exception
                        Console.WriteLine("[ModelLoader] 警告: 加载表型 {0} 失败: {1}",
                                          phenoId, ex.Message)
                    End Try
                End If
            Next

            Console.WriteLine("[ModelLoader] 加载表型模型: {0}/{1} 个",
                              loadedCount, Phenotypes.Count)
        End Sub

        ''' <summary>
        ''' 加载单个表型的模型文件
        ''' </summary>
        Private Sub LoadPhenotypeModel(phenoId As String,
                                       biasFile As String,
                                       featsFile As String)

            Dim phenoModel As Models.PhenotypeModel = Phenotypes(phenoId)

            ' 1. 加载偏置项
            Dim biasDict As New Dictionary(Of Double, Double)()
            Dim biasLines As String() = File.ReadAllLines(biasFile)
            For Each line As String In biasLines
                If String.IsNullOrWhiteSpace(line) Then Continue For
                If line.StartsWith("#") Then Continue For

                Dim parts As String() = line.Split(New Char() {ControlChars.Tab, " "c},
                                                    StringSplitOptions.RemoveEmptyEntries)
                If parts.Length < 2 Then Continue For

                Dim cVal As Double
                Dim bVal As Double
                If Double.TryParse(parts(0), cVal) AndAlso Double.TryParse(parts(1), bVal) Then
                    biasDict(cVal) = bVal
                End If
            Next

            ' 2. 加载特征权重矩阵
            Dim featsLines As String() = File.ReadAllLines(featsFile)
            If featsLines.Length < 2 Then Return

            ' 解析表头获取C值列表
            Dim headerParts As String() = featsLines(0).Split(
                New Char() {ControlChars.Tab, " "c}, StringSplitOptions.RemoveEmptyEntries)

            Dim cValues As New List(Of Double)()
            For j As Integer = 1 To headerParts.Length - 1
                ' 表头格式如 "0.5_0"，取下划线前的部分作为C值
                Dim [cStr] As String = headerParts(j)
                If [cStr].Contains("_") Then
                    [cStr] = [cStr].Split("_"c)(0)
                End If
                Dim cVal As Double
                If Double.TryParse([cStr], cVal) Then
                    cValues.Add(cVal)
                End If
            Next

            ' 解析数据行
            Dim pfamIds As New List(Of String)()
            Dim weightMatrix As New List(Of Double())()

            For i As Integer = 1 To featsLines.Length - 1
                Dim line As String = featsLines(i)
                If String.IsNullOrWhiteSpace(line) Then Continue For

                Dim parts As String() = line.Split(New Char() {ControlChars.Tab, " "c},
                                                    StringSplitOptions.RemoveEmptyEntries)
                If parts.Length < 1 Then Continue For

                pfamIds.Add(parts(0).Trim())

                Dim weights As Double() = New Double(cValues.Count - 1) {}
                For j As Integer = 0 To cValues.Count - 1
                    If j + 1 < parts.Length Then
                        Double.TryParse(parts(j + 1), weights(j))
                    End If
                Next
                weightMatrix.Add(weights)
            Next

            ' 3. 尝试加载non-zero+weights.txt文件（包含实际的非零权重）
            Dim nonzeroPath As String = Path.Combine(_modelsDir, phenoId & "_non-zero+weights.txt")
            Dim nonzeroWeights As New Dictionary(Of String, Double())()
            If File.Exists(nonzeroPath) Then
                Dim nonzeroLines As String() = File.ReadAllLines(nonzeroPath)
                If nonzeroLines.Length > 1 Then
                    ' 跳过表头，解析数据行
                    For i As Integer = 1 To nonzeroLines.Length - 1
                        Dim line As String = nonzeroLines(i)
                        If String.IsNullOrWhiteSpace(line) Then Continue For

                        Dim parts As String() = line.Split(ControlChars.Tab)
                        If parts.Length < 15 Then Continue For

                        Dim pfamId As String = parts(0).Trim()
                        Dim weights As Double() = New Double(cValues.Count - 1) {}
                        For j As Integer = 0 To cValues.Count - 1
                            If j + 2 < parts.Length Then
                                Double.TryParse(parts(j + 2), weights(j))
                            End If
                        Next
                        nonzeroWeights(pfamId) = weights
                    Next
                End If
            End If

            ' 4. 为每个C值构建一个SVM子模型
            For colIdx As Integer = 0 To cValues.Count - 1
                Dim subModel As New Models.SVMSubModel()
                subModel.C = cValues(colIdx)

                ' 查找偏置项
                subModel.Bias = 0.0
                For Each kvp As KeyValuePair(Of Double, Double) In biasDict
                    If Math.Abs(kvp.Key - subModel.C) < 0.000001 Then
                        subModel.Bias = kvp.Value
                        Exit For
                    End If
                Next

                ' 设置权重：优先使用non-zero+weights.txt的权重
                If nonzeroWeights.Count > 0 Then
                    For Each kvp As KeyValuePair(Of String, Double()) In nonzeroWeights
                        Dim w As Double = kvp.Value(colIdx)
                        If Math.Abs(w) > 0.000000000001 Then
                            subModel.Weights(kvp.Key) = w
                        End If
                    Next
                Else
                    ' 回退到feats.txt的权重
                    For rowIdx As Integer = 0 To pfamIds.Count - 1
                        Dim w As Double = weightMatrix(rowIdx)(colIdx)
                        If Math.Abs(w) > 0.000000000001 Then
                            subModel.Weights(pfamIds(rowIdx)) = w
                        End If
                    Next
                End If

                phenoModel.SubModels.Add(subModel)
            Next

        End Sub

    End Class

End Namespace
