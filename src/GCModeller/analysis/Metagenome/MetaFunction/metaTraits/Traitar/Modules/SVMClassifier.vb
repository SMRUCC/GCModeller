' ============================================================================
' SVMClassifier.vb - 模块4：机器学习核心分类器模块
'
' 论文对应：
'   "构建表型分类模型（基于支持向量机SVM）"
'
' 核心功能：
'   1. 线性支持向量机：寻找最大间隔超平面
'   2. L1正则化：生成稀疏权重，实现特征选择
'   3. L2损失函数：优化目标函数
'
' 算法原理：
'   L1正则化L2损失线性SVM的目标函数：
'     min_w  ||w||_1 + C × Σ max(0, 1 - y_i × (w·x_i + b))²
'
'   其中：
'     ||w||_1 = L1正则化项（促进稀疏性，实现特征选择）
'     C = 正则化参数（控制正则化强度）
'     L2损失 = (max(0, 1 - y·f(x)))²（平方Hinge损失）
'
'   求解方法：坐标下降法（Coordinate Descent）
'     - 每次优化一个坐标（一个权重w_j）
'     - 固定其他权重，对w_j求最优
'     - 重复直到收敛
'
'   预测公式：
'     score = b + Σ w_j × x_j
'     label = +1 if score > 0, else -1
' ============================================================================

Namespace TraitarVB.Modules

    ''' <summary>
    ''' 模块4：机器学习核心分类器模块
    ''' 基于特征矩阵和标签，训练能区分表型有无的二分类模型
    ''' </summary>
    Public Class SVMClassifier

        ' SVM模型参数
        Public Class SVMModel
            ''' <summary>权重向量w</summary>
            Public Property Weights As Double()
            ''' <summary>偏置项b</summary>
            Public Property Bias As Double
            ''' <summary>正则化参数C</summary>
            Public Property C As Double
            ''' <summary>特征ID列表</summary>
            Public Property FeatureIds As List(Of String)
            ''' <summary>训练损失历史</summary>
            Public Property LossHistory As New List(Of Double)()
        End Class

        ' 训练参数
        Private _maxIterations As Integer
        Private _tolerance As Double

        ''' <summary>
        ''' 构造函数
        ''' </summary>
        ''' <param name="maxIterations">最大迭代次数</param>
        ''' <param name="tolerance">收敛阈值</param>
        Public Sub New(Optional maxIterations As Integer = 1000,
                       Optional tolerance As Double = 1e-6)
            _maxIterations = maxIterations
            _tolerance = tolerance
        End Sub

        ''' <summary>
        ''' 训练L1正则化L2损失线性SVM
        ''' 使用坐标下降法求解
        '''
        ''' 目标函数：
        '''   f(w) = ||w||_1 + C × Σ_i max(0, 1 - y_i × (w·x_i + b))²
        '''
        ''' 坐标下降法：
        '''   对每个权重w_j，固定其他权重，求解w_j的最优值
        '''   w_j的子问题：
        '''     min_{w_j} |w_j| + C × Σ_i max(0, 1 - y_i × (w_j × x_ij + r_i))²
        '''   其中 r_i = w·x_i - w_j × x_ij + b 是残差
        ''' </summary>
        ''' <param name="X">特征矩阵（样本×特征）</param>
        ''' <param name="y">标签数组（-1或+1）</param>
        ''' <param name="C">正则化参数</param>
        ''' <param name="featureIds">特征ID列表</param>
        ''' <returns>训练好的SVM模型</returns>
        Public Function Train(X As Integer(,), y As Integer(),
                              C As Double,
                              Optional featureIds As List(Of String) = Nothing) As SVMModel

            Dim nSamples As Integer = X.GetLength(0)
            Dim nFeatures As Integer = X.GetLength(1)

            If featureIds Is Nothing Then
                featureIds = New List(Of String)()
                For j As Integer = 0 To nFeatures - 1
                    featureIds.Add("F" & j.ToString())
                Next
            End If

            Console.WriteLine("[模块4] 训练L1正则化L2损失线性SVM")
            Console.WriteLine("       样本数: {0}", nSamples)
            Console.WriteLine("       特征数: {0}", nFeatures)
            Console.WriteLine("       正则化参数C: {0}", C)

            ' 初始化权重和偏置
            Dim w As Double() = New Double(nFeatures - 1) {}
            Dim b As Double = 0.0

            ' 计算每个样本的预测值 w·x + b
            Dim pred As Double() = New Double(nSamples - 1) {}
            For i As Integer = 0 To nSamples - 1
                pred(i) = b
            Next

            ' 坐标下降法主循环
            Dim iter As Integer
            For iter = 0 To _maxIterations - 1
                Dim prevLoss As Double = ComputeObjective(X, y, w, b, C)

                ' 1. 更新每个权重w_j
                For j As Integer = 0 To nFeatures - 1
                    UpdateWeightCoordinate(X, y, w, b, pred, j, C, nSamples)
                Next

                ' 2. 更新偏置b
                UpdateBias(X, y, w, b, pred, C, nSamples)

                ' 检查收敛
                Dim currLoss As Double = ComputeObjective(X, y, w, b, C)
                If iter Mod 100 = 0 Then
                    Console.WriteLine("       迭代 {0}: 损失 = {1:F6}", iter, currLoss)
                End If

                If Math.Abs(prevLoss - currLoss) < _tolerance Then
                    Console.WriteLine("       收敛于迭代 {0}", iter)
                    Exit For
                End If
            Next

            ' 构建模型
            Dim model As New SVMModel()
            model.Weights = w
            model.Bias = b
            model.C = C
            model.FeatureIds = featureIds

            ' 统计非零权重数量
            Dim nNonZero As Integer = 0
            For Each wi As Double In w
                If Math.Abs(wi) > 1e-10 Then nNonZero += 1
            Next
            Console.WriteLine("[模块4] 训练完成: 非零权重 {0}/{1}", nNonZero, nFeatures)

            Return model
        End Function

        ''' <summary>
        ''' 坐标下降：更新单个权重w_j
        '''
        ''' 子问题：
        '''   min_{w_j} |w_j| + C × Σ_i max(0, 1 - y_i × (w_j × x_ij + r_i))²
        '''   其中 r_i = pred(i) - w_j × x_ij
        '''
        ''' 求导后得到次梯度条件，解析解为：
        '''   1. 计算梯度 G = -2C × Σ_i x_ij × y_i × max(0, 1 - y_i × pred(i))
        '''   2. 若 |G| &lt; 1，则 w_j = 0（L1正则化导致稀疏）
        '''   3. 否则 w_j -= (G - sign(w_j)) / H，其中H是Hessian对角元素
        ''' </summary>
        Private Sub UpdateWeightCoordinate(X As Integer(,), y As Integer(),
                                           w As Double(), b As Double,
                                           pred As Double(), j As Integer,
                                           C As Double, nSamples As Integer)

            Dim wj_old As Double = w(j)

            ' 计算梯度和Hessian
            Dim grad As Double = 0.0
            Dim hess As Double = 0.0

            For i As Integer = 0 To nSamples - 1
                Dim xij As Double = CDbl(X(i, j))
                If xij = 0 Then Continue For

                Dim yi As Double = CDbl(y(i))
                Dim margin As Double = yi * pred(i)

                ' L2 Hinge Loss: L = max(0, 1 - margin)²
                ' dL/d(w_j) = -2 × x_ij × y_i × max(0, 1 - margin)
                ' d²L/d(w_j)² = 2 × x_ij² × I(margin < 1)
                If margin < 1.0 Then
                    grad -= 2.0 * C * xij * yi * (1.0 - margin)
                    hess += 2.0 * C * xij * xij
                End If
            Next

            ' L1正则化项的次梯度
            ' 目标 = |w_j| + (1/2) × hess × (w_j - w_j_old)² + grad × (w_j - w_j_old)
            ' 最优解：
            '   若 hess = 0，则 w_j = 0
            '   否则 w_j = -SoftThreshold(grad, 1) / hess + w_j_old
            '   其中 SoftThreshold(z, λ) = sign(z) × max(|z| - λ, 0)

            If hess < 1e-12 Then
                ' 无样本依赖此特征，L1正则化使权重为0
                w(j) = 0.0
            Else
                ' 软阈值操作
                Dim z As Double = wj_old - grad / hess
                w(j) = SoftThreshold(z, 1.0 / hess)
            End If

            ' 更新预测值
            Dim delta As Double = w(j) - wj_old
            If Math.Abs(delta) > 1e-12 Then
                For i As Integer = 0 To nSamples - 1
                    pred(i) += delta * CDbl(X(i, j))
                Next
            End If
        End Sub

        ''' <summary>
        ''' 软阈值函数（L1正则化的核心）
        ''' SoftThreshold(z, λ) = sign(z) × max(|z| - λ, 0)
        ''' </summary>
        Public Function SoftThreshold(z As Double, lambda As Double) As Double
            If z > lambda Then
                Return z - lambda
            ElseIf z < -lambda Then
                Return z + lambda
            Else
                Return 0.0
            End If
        End Function

        ''' <summary>
        ''' 更新偏置项b
        ''' 偏置项不受L1正则化约束
        ''' </summary>
        Private Sub UpdateBias(X As Integer(,), y As Integer(),
                               w As Double(), ByRef b As Double,
                               pred As Double(), C As Double,
                               nSamples As Integer)

            Dim grad As Double = 0.0
            Dim hess As Double = 0.0

            For i As Integer = 0 To nSamples - 1
                Dim yi As Double = CDbl(y(i))
                Dim margin As Double = yi * pred(i)

                If margin < 1.0 Then
                    grad -= 2.0 * C * yi * (1.0 - margin)
                    hess += 2.0 * C
                End If
            Next

            If hess > 1e-12 Then
                Dim delta As Double = -grad / hess
                b += delta
                For i As Integer = 0 To nSamples - 1
                    pred(i) += delta
                Next
            End If
        End Sub

        ''' <summary>
        ''' 计算目标函数值
        ''' f(w, b) = ||w||_1 + C × Σ_i max(0, 1 - y_i × (w·x_i + b))²
        ''' </summary>
        Public Function ComputeObjective(X As Integer(,), y As Integer(),
                                         w As Double(), b As Double,
                                         C As Double) As Double
            Dim nSamples As Integer = X.GetLength(0)
            Dim nFeatures As Integer = X.GetLength(1)

            ' L1正则化项
            Dim l1Norm As Double = 0.0
            For j As Integer = 0 To nFeatures - 1
                l1Norm += Math.Abs(w(j))
            Next

            ' L2 Hinge Loss
            Dim loss As Double = 0.0
            For i As Integer = 0 To nSamples - 1
                Dim score As Double = b
                For j As Integer = 0 To nFeatures - 1
                    score += w(j) * CDbl(X(i, j))
                Next
                Dim margin As Double = CDbl(y(i)) * score
                Dim hinge As Double = 1.0 - margin
                If hinge < 0 Then hinge = 0
                loss += hinge * hinge
            Next

            Return l1Norm + C * loss
        End Function

        ''' <summary>
        ''' 使用模型预测单个样本
        ''' 公式：score = b + Σ w_j × x_j
        '''       label = +1 if score > 0, else -1
        ''' </summary>
        Public Function PredictScore(model As SVMModel,
                                     features As Dictionary(Of String, Integer)) As Double
            Dim score As Double = model.Bias
            For j As Integer = 0 To model.FeatureIds.Count - 1
                Dim fid As String = model.FeatureIds(j)
                If features.ContainsKey(fid) AndAlso features(fid) = 1 Then
                    score += model.Weights(j)
                End If
            Next
            Return score
        End Function

        ''' <summary>
        ''' 预测样本标签
        ''' </summary>
        Public Function PredictLabel(model As SVMModel,
                                     features As Dictionary(Of String, Integer)) As Integer
            Dim score As Double = PredictScore(model, features)
            Return If(score > 0, 1, -1)
        End Function

        ''' <summary>
        ''' 批量预测
        ''' </summary>
        Public Function PredictBatch(model As SVMModel,
                                     X As Integer(,)) As Integer()
            Dim nSamples As Integer = X.GetLength(0)
            Dim nFeatures As Integer = X.GetLength(1)
            Dim predictions As Integer() = New Integer(nSamples - 1) {}

            For i As Integer = 0 To nSamples - 1
                Dim score As Double = model.Bias
                For j As Integer = 0 To nFeatures - 1
                    score += model.Weights(j) * CDbl(X(i, j))
                Next
                predictions(i) = If(score > 0, 1, -1)
            Next

            Return predictions
        End Function

        ''' <summary>
        ''' 从模型文件加载预训练模型
        ''' 文件格式：
        '''   {id}_bias.txt:  C值 \t 偏置值
        '''   {id}_feats.txt: PfamID \t 各C值对应的权重
        ''' </summary>
        Public Function LoadModelFromFiles(biasFile As String,
                                           featsFile As String) As List(Of SVMModel)
            Dim models As New List(Of SVMModel)()

            ' 1. 解析bias文件
            Dim biasDict As New Dictionary(Of Double, Double)
            Dim biasLines As String() = System.IO.File.ReadAllLines(biasFile)
            For Each line As String In biasLines
                If String.IsNullOrWhiteSpace(line) Then Continue For
                Dim parts As String() = line.Split(New Char() {" "c, ControlChars.Tab},
                                                    StringSplitOptions.RemoveEmptyEntries)
                If parts.Length >= 2 Then
                    Dim cVal As Double
                    Dim bVal As Double
                    If Double.TryParse(parts(0), cVal) AndAlso Double.TryParse(parts(1), bVal) Then
                        biasDict(cVal) = bVal
                    End If
                End If
            Next

            ' 2. 解析feats文件
            Dim featLines As String() = System.IO.File.ReadAllLines(featsFile)
            If featLines.Length < 2 Then Return models

            ' 解析表头（C值列表）
            Dim headerParts As String() = featLines(0).Split(New Char() {" "c, ControlChars.Tab},
                                                              StringSplitOptions.RemoveEmptyEntries)
            Dim cValues As New List(Of Double)()
            Dim colNames As New List(Of String)()
            For Each h As String In headerParts
                colNames.Add(h)
                ' 解析C值：格式为 "0.5_0" -> 0.5
                Dim [cStr] As String = h.Split("_"c)(0)
                Dim cVal As Double
                If Double.TryParse([cStr], cVal) Then
                    cValues.Add(cVal)
                End If
            Next

            ' 收集所有Pfam ID和权重
            Dim pfamIds As New List(Of String)()
            Dim weightMatrix As New List(Of Double())()

            For i As Integer = 1 To featLines.Length - 1
                Dim line As String = featLines(i)
                If String.IsNullOrWhiteSpace(line) Then Continue For
                Dim parts As String() = line.Split(New Char() {" "c, ControlChars.Tab},
                                                    StringSplitOptions.RemoveEmptyEntries)
                If parts.Length < 1 Then Continue For

                pfamIds.Add(parts(0))
                Dim weights As Double() = New Double(cValues.Count - 1) {}
                For j As Integer = 0 To cValues.Count - 1
                    If j + 1 < parts.Length Then
                        Double.TryParse(parts(j + 1), weights(j))
                    End If
                Next
                weightMatrix.Add(weights)
            Next

            ' 3. 为每个C值构建一个SVM模型
            For colIdx As Integer = 0 To cValues.Count - 1
                Dim model As New SVMModel()
                model.C = cValues(colIdx)
                model.FeatureIds = New List(Of String)(pfamIds)
                model.Weights = New Double(pfamIds.Count - 1) {}
                For rowIdx As Integer = 0 To pfamIds.Count - 1
                    model.Weights(rowIdx) = weightMatrix(rowIdx)(colIdx)
                Next

                If biasDict.ContainsKey(model.C) Then
                    model.Bias = biasDict(model.C)
                Else
                    ' 尝试匹配（处理浮点精度问题）
                    For Each kvp As KeyValuePair(Of Double, Double) In biasDict
                        If Math.Abs(kvp.Key - model.C) < 1e-6 Then
                            model.Bias = kvp.Value
                            Exit For
                        End If
                    Next
                End If

                models.Add(model)
            Next

            Console.WriteLine("[模块4] 加载模型完成: {0} 个子模型, {1} 个特征",
                              models.Count, pfamIds.Count)
            Return models
        End Function

    End Class

End Namespace
