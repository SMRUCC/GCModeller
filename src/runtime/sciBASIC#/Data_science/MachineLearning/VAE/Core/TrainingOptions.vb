' ============================================================================
' TrainingOptions.vb - 训练超参数聚合
'
' 集中管理 epochs / batch / lr / beta / gamma / 早停 / 梯度裁剪 / KL 退火，
' 让各模型的 Fit 重载只需要接收一个对象。
' ============================================================================

Imports std = System.Math

Namespace Core

    ''' <summary>
    ''' 训练超参数与调度策略。
    ''' </summary>
    Public Class TrainingOptions

        ''' <summary>训练轮数。</summary>
        Public Property Epochs As Integer = 100

        ''' <summary>批大小（0 或负数表示全量）。</summary>
        Public Property BatchSize As Integer = 32

        ''' <summary>学习率。</summary>
        Public Property LearningRate As Double = 0.001

        ''' <summary>KL 项总权重（退火的终值）。</summary>
        public Property Beta As Double = 1.0

        ''' <summary>额外 KL 项的权重（GMVAE 的 gamma）。</summary>
        Public Property Gamma As Double = 1.0

        ''' <summary>
        ''' KL 退火的预热轮数。设为零或负数表示不退火（等价于旧行为）。
        ''' 预热期间 beta 从接近 0 线性升到 <see cref="Beta"/>。
        ''' </summary>
        Public Property KlWarmupEpochs As Integer = 0

        ''' <summary>梯度 L2 范数上限；零或负数表示不裁剪。</summary>
        Public Property GradientClip As Double = 0

        ''' <summary>早停耐心值（以轮为单位）；零或负数表示不早停。</summary>
        Public Property Patience As Integer = 0

        ''' <summary>早停判定所需的最小改善量。</summary>
        Public Property MinDelta As Double = 0.000001

        ''' <summary>是否输出训练进度。</summary>
        Public Property Verbose As Boolean = True

        ''' <summary>随机种子。</summary>
        Public Property Seed As Integer? = Nothing

        ''' <summary>是否使用 Adam 优化器（否则使用普通 SGD）。</summary>
        Public Property UseAdam As Boolean = True

        ''' <summary>
        ''' 训练过程观察者。为 <c>Nothing</c> 时由各模型按 <see cref="Verbose"/>
        ''' 自行选择对应的控制台文案格式。
        ''' </summary>
        Public Property Observer As ITrainingObserver

        ''' <summary>把 <see cref="Observer"/> 解析为实际可用的观察者。</summary>
        Public Function ResolveObserver(fallback As ITrainingObserver) As ITrainingObserver
            If Observer IsNot Nothing Then
                Return Observer
            End If

            If Verbose AndAlso fallback IsNot Nothing Then
                Return fallback
            End If

            Return New NullTrainingObserver()
        End Function

        ''' <summary>第 <paramref name="epoch"/> 轮实际使用的 beta（含 KL 退火）。</summary>
        Public Function BetaAt(epoch As Integer) As Double
            Return Schedule(Beta, epoch, KlWarmupEpochs)
        End Function

        ''' <summary>第 <paramref name="epoch"/> 轮实际使用的 gamma（含 KL 退火）。</summary>
        Public Function GammaAt(epoch As Integer) As Double
            Return Schedule(Gamma, epoch, KlWarmupEpochs)
        End Function

        Private Shared Function Schedule(value As Double, epoch As Integer, warmup As Integer) As Double
            If warmup <= 0 Then
                Return value
            End If

            If epoch >= warmup Then
                Return value
            End If

            Return value * (CDbl(std.Max(epoch, 0)) / warmup)
        End Function

        ''' <summary>创建一份浅拷贝。</summary>
        Public Function Clone() As TrainingOptions
            Return New TrainingOptions With {
                .Epochs = Epochs,
                .BatchSize = BatchSize,
                .LearningRate = LearningRate,
                .Beta = Beta,
                .Gamma = Gamma,
                .KlWarmupEpochs = KlWarmupEpochs,
                .GradientClip = GradientClip,
                .Patience = Patience,
                .MinDelta = MinDelta,
                .Verbose = Verbose,
                .Seed = Seed,
                .UseAdam = UseAdam,
                .Observer = Observer
            }
        End Function

        ''' <summary>一组保守的默认值。</summary>
        Public Shared Function CreateDefault() As TrainingOptions
            Return New TrainingOptions()
        End Function

        Public Overrides Function ToString() As String
            Return $"TrainingOptions(epochs={Epochs}, batch={BatchSize}, lr={LearningRate}, beta={Beta}, gamma={Gamma}, warmup={KlWarmupEpochs})"
        End Function

    End Class

End Namespace
