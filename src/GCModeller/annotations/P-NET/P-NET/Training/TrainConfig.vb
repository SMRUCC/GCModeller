Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' P-NET 的训练超参数配置
''' </summary>
''' <remarks>
''' 默认值均取自论文 Methods 部分的描述：Adam 优化器、初始学习率 0.001、
''' 每 50 个 epoch 主动衰减学习率、mini-batch 训练、按训练集类别比例对损失加权。
''' </remarks>
Public Class TrainConfig

    ''' <summary>
    ''' 训练迭代轮数
    ''' </summary>
    ''' <returns>轮数，默认 150</returns>
    Public Property Epochs As Integer = 150

    ''' <summary>
    ''' mini-batch 的批大小
    ''' </summary>
    ''' <returns>批大小，默认 32</returns>
    Public Property BatchSize As Integer = 32

    ''' <summary>
    ''' 初始学习率，论文取 0.001
    ''' </summary>
    ''' <returns>学习率</returns>
    Public Property LearningRate As Double = 0.001

    ''' <summary>
    ''' 学习率衰减的周期（单位：epoch），论文中每 50 个 epoch 衰减一次
    ''' </summary>
    ''' <returns>衰减周期，取 0 时表示不做衰减</returns>
    Public Property LrDecayStep As Integer = 50

    ''' <summary>
    ''' 每一次衰减时学习率所乘的系数
    ''' </summary>
    ''' <returns>衰减系数，默认 0.5</returns>
    Public Property LrDecayFactor As Double = 0.5

    ''' <summary>
    ''' Adam 的一阶矩衰减率
    ''' </summary>
    ''' <returns>beta1，默认 0.9</returns>
    Public Property Beta1 As Double = 0.9

    ''' <summary>
    ''' Adam 的二阶矩衰减率
    ''' </summary>
    ''' <returns>beta2，默认 0.999</returns>
    Public Property Beta2 As Double = 0.999

    ''' <summary>
    ''' Adam 的数值稳定性常数
    ''' </summary>
    ''' <returns>epsilon，默认 1e-8</returns>
    Public Property Epsilon As Double = 0.00000001

    ''' <summary>
    ''' 是否按照训练集的类别比例对二元交叉熵损失加权
    ''' </summary>
    ''' <returns>默认 True</returns>
    ''' <remarks>
    ''' 论文的数据集存在类别不平衡（333 例 CRPC / 转移性对比 680 例原发性），
    ''' 按类别比例加权可以抑制模型向多数类偏移。
    ''' 权重取值为 <c>N / (2 · N_class)</c>，与 sklearn 的 <c>class_weight='balanced'</c> 一致。
    ''' </remarks>
    Public Property UseClassWeights As Boolean = True

    ''' <summary>
    ''' 随机数种子，给出之后数据打乱的结果可复现
    ''' </summary>
    ''' <returns>种子值</returns>
    Public Property Seed As Integer? = Nothing

    ''' <summary>
    ''' 是否在训练过程中逐轮打印训练日志
    ''' </summary>
    ''' <returns>默认 True</returns>
    Public Property Verbose As Boolean = True

    ''' <summary>
    ''' 每隔多少个 epoch 打印一次日志
    ''' </summary>
    ''' <returns>打印间隔，默认 10</returns>
    Public Property VerboseInterval As Integer = 10

    ''' <summary>
    ''' 早停的耐心轮数，取 0 时表示不使用早停
    ''' </summary>
    ''' <returns>默认 0（关闭）</returns>
    Public Property EarlyStopPatience As Integer = 0

    ''' <summary>
    ''' 计算指定 epoch（从 0 开始计数）时应当使用的学习率
    ''' </summary>
    ''' <param name="epoch">当前的 epoch 下标</param>
    ''' <returns>衰减之后的学习率</returns>
    ''' <remarks>
    ''' 采用阶梯衰减：<c>lr = lr0 · factor ^ floor(epoch / step)</c>。
    ''' 论文的经验发现"Adam + 自适应衰减"比单一学习率的 Adam 收敛更平滑、性能更好。
    ''' </remarks>
    Public Function GetLearningRate(epoch As Integer) As Double
        If LrDecayStep <= 0 Then
            Return LearningRate
        End If

        Dim stages As Integer = epoch \ LrDecayStep

        Return LearningRate * std.Pow(LrDecayFactor, stages)
    End Function

End Class

''' <summary>
''' 训练过程的历史记录
''' </summary>
Public Class TrainingHistory

    ''' <summary>
    ''' 逐轮的训练损失
    ''' </summary>
    ''' <returns>损失列表</returns>
    Public ReadOnly Property Loss As New List(Of Double)()

    ''' <summary>
    ''' 逐轮的训练集 AUC
    ''' </summary>
    ''' <returns>AUC 列表</returns>
    Public ReadOnly Property TrainAUC As New List(Of Double)()

    ''' <summary>
    ''' 逐轮的验证集 AUC，未给出验证集时该列表为空
    ''' </summary>
    ''' <returns>AUC 列表</returns>
    Public ReadOnly Property ValidationAUC As New List(Of Double)()

    ''' <summary>
    ''' 逐轮所使用的学习率
    ''' </summary>
    ''' <returns>学习率列表</returns>
    Public ReadOnly Property LearningRate As New List(Of Double)()

    ''' <summary>
    ''' 验证集 AUC 最高的那一轮（从 0 开始计数）
    ''' </summary>
    ''' <returns>最优轮下标</returns>
    Public Property BestEpoch As Integer = 0

    ''' <summary>
    ''' 验证集上的最优 AUC
    ''' </summary>
    ''' <returns>最优 AUC 值</returns>
    Public Property BestValidationAUC As Double = 0.0

    ''' <summary>
    ''' 生成训练历史的多行文本摘要
    ''' </summary>
    ''' <returns>逐轮指标文本</returns>
    Public Overrides Function ToString() As String
        Dim sb As New System.Text.StringBuilder()

        For i As Integer = 0 To Loss.Count - 1
            Call sb.Append($"epoch {i + 1}")

            Call sb.Append($", loss={Loss(i).ToString("F4")}")
            Call sb.Append($", train_auc={TrainAUC(i).ToString("F4")}")

            If i < ValidationAUC.Count Then
                Call sb.Append($", val_auc={ValidationAUC(i).ToString("F4")}")
            End If

            Call sb.Append($", lr={LearningRate(i).ToString("F6")}")
            Call sb.AppendLine()
        Next

        Return sb.ToString()
    End Function

End Class
