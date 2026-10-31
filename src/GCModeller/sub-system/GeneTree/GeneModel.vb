Imports Microsoft.VisualBasic.MachineLearning.XGBoost.train

''' <summary>
''' 单个调控因子在一个基因模型之中的重要性统计
''' </summary>
Public Class RegulatorImportance

    ''' <summary>调控因子编号</summary>
    Public Property regulator As String

    ''' <summary>与靶基因之间的互信息</summary>
    Public Property mi As Double

    ''' <summary>与靶基因之间的 pearson 相关系数</summary>
    Public Property correlation As Double

    ''' <summary>在特征向量之中的位置（0 起）</summary>
    Public Property rank As Integer

    ''' <summary>所有树之中，以该特征作为分裂特征的节点数量</summary>
    Public Property splits As Integer

    ''' <summary>
    ''' 灵敏度：其余特征取训练集的中位数时，把该特征从最小值扫描到最大值所引起的预测变化幅度。
    ''' 
    ''' 这是一个廉价的“偏依赖”近似，用于大致判断该调控边对靶基因的贡献大小与强度。
    ''' </summary>
    Public Property sensitivity As Double

    Public Overrides Function ToString() As String
        Return $"{regulator}: splits={splits}, sensitivity={sensitivity.ToString("G4")}, MI={mi.ToString("G4")}"
    End Function
End Class

''' <summary>
''' 单个基因模型的拟合评价
''' </summary>
Public Class ModelPerformance

    ''' <summary>验证集上的决定系数 R²（没有验证集时退化为训练集 R²）</summary>
    Public Property r2 As Double

    ''' <summary>验证集上的均方根误差</summary>
    Public Property rmse As Double

    ''' <summary>
    ''' 用“标签均值”作为预测时得到的均方根误差（也就是标签的标准差），
    ''' 用于衡量模型相对常数的改进幅度。
    ''' </summary>
    Public Property baselineRmse As Double

    ''' <summary>训练集样本数</summary>
    Public Property nTrain As Integer

    ''' <summary>验证集样本数（0 表示没有留出验证集）</summary>
    Public Property nValidate As Integer

    ''' <summary>模型的种类：<c>xgboost</c> 或 <c>constant</c></summary>
    Public Property kind As String

    ''' <summary>树的数量</summary>
    Public Property trees As Integer

    ''' <summary>被选中参与拟合的调控因子数量</summary>
    Public Property nRegulators As Integer

    ''' <summary>训练标签的取值范围下界（外推裁剪用）</summary>
    Public Property minValue As Double

    ''' <summary>训练标签的取值范围上界（外推裁剪用）</summary>
    Public Property maxValue As Double

    ''' <summary>每个被选中调控因子的重要性</summary>
    Public Property importance As RegulatorImportance() = New RegulatorImportance() {}

    ''' <summary>
    ''' 相对常数基线的改进比例：<c>1 - rmse / baselineRmse</c>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property improvement As Double
        Get
            If baselineRmse <= 0 Then
                Return 0.0R
            Else
                Return 1.0R - rmse / baselineRmse
            End If
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{kind}, R2={r2.ToString("F4")}, RMSE={rmse.ToString("G4")} (baseline {baselineRmse.ToString("G4")}), " &
               $"n={nTrain}+{nValidate}, trees={trees}"
    End Function
End Class

''' <summary>
''' 网络中单个靶基因的非线性调控函数 <c>f_i(X_Reg(i))</c>。
''' 
''' 模型的三种状态：
''' 
''' + <see cref="Booster"/> 非空：由 TGBoost 训练得到的梯度提升树；
''' + <see cref="Booster"/> 为空且 <see cref="IsFittedModel"/> 为 True：训练失败/样本不足时的常数模型；
''' + 模型对象本身为 <c>Nothing</c>：该基因没有任何调控者（源节点），在迭代之中保持初始值不变。
''' </summary>
Public Class GeneModel

    Private m_booster As GBM
    Private m_trees As List(Of Tree)
    Private m_eta As Double
    Private m_first As Double
    Private m_squareLoss As Boolean = True

    ''' <summary>靶基因编号</summary>
    Public Property Gene As String

    ''' <summary>
    ''' 特征（调控因子）的编号顺序。训练、预测与存档三处必须完全一致。
    ''' </summary>
    Public Property Regulators As String() = New String() {}

    ''' <summary>调控因子在表达矩阵之中的行下标（与 <see cref="Regulators"/> 一一对应）</summary>
    Public Property RegulatorIndex As Integer() = New Integer() {}

    ''' <summary>训练标签的取值范围下界（外推裁剪用）</summary>
    Public Property MinValue As Double

    ''' <summary>训练标签的取值范围上界（外推裁剪用）</summary>
    Public Property MaxValue As Double

    ''' <summary>
    ''' 常数模型的输出值（训练标签均值）；只有 <see cref="Booster"/> 为空时才会被使用
    ''' </summary>
    Public Property Constant As Double

    ''' <summary>拟合评价与特征重要性</summary>
    Public Property Performance As ModelPerformance = New ModelPerformance()

    ''' <summary>
    ''' 梯度提升树模型。赋值为 <c>Nothing</c> 时模型退化为常数模型。
    ''' </summary>
    ''' <returns></returns>
    Public Property Booster As GBM
        Get
            Return m_booster
        End Get
        Set(value As GBM)
            m_booster = value
            Call prepare()
        End Set
    End Property

    ''' <summary>
    ''' 该基因是否拥有一个（可能退化了的）模型对象。<c>False</c> 表示它是没有调控者的源节点。
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsFittedModel As Boolean = True

    ''' <summary>
    ''' 模型种类：<c>xgboost</c> 或 <c>constant</c>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Kind As String
        Get
            If IsFittedModel AndAlso m_booster IsNot Nothing AndAlso m_trees IsNot Nothing AndAlso m_trees.Count > 0 Then
                Return "xgboost"
            Else
                Return "constant"
            End If
        End Get
    End Property

    ''' <summary>
    ''' 树的数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property TreeCount As Integer
        Get
            If m_trees Is Nothing Then
                Return 0
            Else
                Return m_trees.Count
            End If
        End Get
    End Property

    Private Sub prepare()
        If m_booster Is Nothing Then
            m_trees = Nothing
            m_eta = 0.0R
            m_first = 0.0R
            m_squareLoss = True
            Return
        End If

        m_trees = m_booster.trees
        m_eta = m_booster.eta
        m_first = m_booster.first_round_pred

        ' SquareLoss 的 transform 是恒等映射，此时 GBM.predict 的返回值就是 raw margin，
        ' 可以直接用叶值做紧凑遍历；其它损失函数（例如 logistic）必须回退到库实现。
        Try
            m_squareLoss = String.Equals(m_booster.loss?.GetType().Name, "SquareLoss", StringComparison.Ordinal)
        Catch ex As Exception
            m_squareLoss = False
        End Try
    End Sub

    ''' <summary>
    ''' 批量预测。返回数组的顺序与 <paramref name="features"/> 的行顺序一致。
    ''' </summary>
    ''' <param name="features">特征矩阵（行=细胞，列=调控因子，列序必须与 <see cref="Regulators"/> 一致）</param>
    ''' <returns></returns>
    Public Function PredictBatch(features As Single()()) As Double()
        Dim n As Integer = features.Length
        Dim result As Double() = New Double(n - 1) {}

        If n = 0 Then
            Return result
        End If

        If m_booster Is Nothing OrElse m_trees Is Nothing OrElse m_trees.Count = 0 Then
            For i As Integer = 0 To n - 1
                result(i) = Constant
            Next

            Return result
        End If

        If Not m_squareLoss Then
            ' 非平方损失时不能简单累加叶值，回退到库实现
            Return m_booster.predict(features)
        End If

        For i As Integer = 0 To n - 1
            result(i) = evaluate(features(i))
        Next

        Return result
    End Function

    ''' <summary>
    ''' 单行预测
    ''' </summary>
    ''' <param name="feature">特征向量（列序必须与 <see cref="Regulators"/> 一致）</param>
    ''' <returns></returns>
    Public Function PredictOne(feature As Single()) As Double
        If m_booster Is Nothing OrElse m_trees Is Nothing OrElse m_trees.Count = 0 Then
            Return Constant
        ElseIf Not m_squareLoss Then
            Return m_booster.predict({feature})(0)
        Else
            Return evaluate(feature)
        End If
    End Function

    ''' <summary>
    ''' 与 <see cref="Tree.predict"/> 数值等价的紧凑单行遍历（去掉了 PLINQ 与逐树数组分配）。
    ''' </summary>
    ''' <param name="feature"></param>
    ''' <returns></returns>
    Private Function evaluate(feature As Single()) As Double
        Dim acc As Double = m_first

        For Each tree As Tree In m_trees
            Dim node As TreeNode = tree.root

            While node IsNot Nothing AndAlso Not node.is_leaf
                Dim value As Single = feature(node.split_feature)

                If value = Data.NA OrElse Single.IsNaN(value) Then
                    ' 缺失值分支：0=nan 子节点、1=左、2=右、其它=样本较多的一侧
                    Dim nanBranch As Double = node.nan_go_to

                    If nanBranch = 0 Then
                        node = node.nan_child
                    ElseIf nanBranch = 1 Then
                        node = node.left_child
                    ElseIf nanBranch = 2 Then
                        node = node.right_child
                    ElseIf node.left_child IsNot Nothing AndAlso node.right_child IsNot Nothing AndAlso
                           node.left_child.num_sample > node.right_child.num_sample Then
                        node = node.left_child
                    Else
                        node = node.right_child
                    End If
                ElseIf node.split_left_child_catvalue IsNot Nothing Then
                    If node.split_left_child_catvalue.Contains(CDbl(value)) Then
                        node = node.left_child
                    Else
                        node = node.right_child
                    End If
                ElseIf value <= node.split_threshold Then
                    node = node.left_child
                Else
                    node = node.right_child
                End If
            End While

            If node Is Nothing Then
                Continue For
            End If

            acc += m_eta * node.leafValue
        Next

        Return acc
    End Function

    ''' <summary>
    ''' 把 <see cref="MinValue"/> 与 <see cref="MaxValue"/> 按外推裁剪系数展开为裁剪区间
    ''' </summary>
    ''' <param name="clipFactor">裁剪系数，必须 ≥ 1</param>
    ''' <param name="clipNegative">是否把下界抬到 0</param>
    ''' <returns>(下界, 上界)</returns>
    Public Function GetClippingRange(clipFactor As Double, clipNegative As Boolean) As (lower As Double, upper As Double)
        Dim k As Double = If(clipFactor < 1.0R, 1.0R, clipFactor)
        Dim lo As Double
        Dim hi As Double = MaxValue * k

        If MinValue >= 0 Then
            lo = MinValue / k
        Else
            lo = MinValue * k
        End If

        If hi < lo Then
            Dim tmp As Double = lo
            lo = hi
            hi = tmp
        End If

        If clipNegative AndAlso lo < 0 Then
            lo = 0.0R
        End If

        Return (lo, hi)
    End Function

    Public Overrides Function ToString() As String
        Return $"{Gene} <- ({If(Regulators Is Nothing, 0, Regulators.Length)} regulators) {Performance.ToString}"
    End Function

End Class

