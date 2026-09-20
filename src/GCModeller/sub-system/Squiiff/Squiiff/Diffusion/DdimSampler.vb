Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Diffusion

    ''' <summary>DDIM 去噪过程的单步快照（用于导出"从噪声逐步还原到转录组"的可视化数据）。</summary>
    Public Class SamplingSnapshot

        ''' <summary>采样顺序（0 为起点）。</summary>
        Public Property Index As Integer

        ''' <summary>该快照对应的扩散时间步（0 表示干净数据）。</summary>
        Public Property Timestep As Integer

        ''' <summary>该步的样本状态 <c>[B,G]</c>。</summary>
        Public Property X As Tensor

        ''' <summary>阶段标签（例如 <c>x_T</c> / <c>x_250</c> / <c>x_0</c>）。</summary>
        Public Property Stage As String
    End Class

    ''' <summary>
    ''' 条件 DDIM 采样器（确定性生成轨迹）。
    '''
    ''' 与标准 DDPM 不同，SquiDiff 采用 DDIM 的**确定性**反向过程：
    ''' <code>
    ''' x_{t-1} = √ᾱ_{t-1} · x̂_0 + √(1−ᾱ_{t-1}) · ε_θ(x_t, t, z_sem)
    ''' x̂_0    = ( x_t − √(1−ᾱ_t) · ε_θ(x_t, t, z_sem) ) / √ᾱ_t
    ''' </code>
    ''' 确定性有两点收益：
    ''' <list type="number">
    ''' <item>采样路径固定，降低随机方差；</item>
    ''' <item>该过程**可被反向运行**：给定 <c>x_0</c> 可逐步"编码"回 <c>x_T</c>，
    '''       这正是扩散自编码器的"编码"一半（<see cref="Invert"/>）。</item>
    ''' </list>
    ''' 为加速推理，采样/反演共用同一条**跨步子轨迹**（在 <c>ᾱ</c> 序列上等距取索引），
    ''' 把 K 次串行前向从 T 降到 <see cref="InferenceSteps"/>。
    ''' </summary>
    Public NotInheritable Class DdimSampler

        Private ReadOnly _schedule As DiffusionSchedule
        Private ReadOnly _trajectory As Integer()

        ''' <param name="schedule">噪声调度。</param>
        ''' <param name="inferenceSteps">推理子轨迹长度（跨步 DDIM）。</param>
        Public Sub New(schedule As DiffusionSchedule, Optional inferenceSteps As Integer = 100)
            If schedule Is Nothing Then Throw New ArgumentNullException(NameOf(schedule))
            If inferenceSteps < 1 Then Throw New ArgumentOutOfRangeException(NameOf(inferenceSteps))

            Me._schedule = schedule
            Me._trajectory = BuildTrajectory(schedule.T, inferenceSteps)
        End Sub

        Public ReadOnly Property Schedule As DiffusionSchedule
            Get
                Return _schedule
            End Get
        End Property

        ''' <summary>实际使用的推理步数（可能因去重少于请求值）。</summary>
        Public ReadOnly Property InferenceSteps As Integer
            Get
                Return _trajectory.Length
            End Get
        End Property

        ''' <summary>降序子轨迹 <c>[T, ..., t_min]</c>（不含 0，因为 t=0 不属于去噪网络的训练范围）。</summary>
        Public ReadOnly Property Trajectory As Integer()
            Get
                Return _trajectory
            End Get
        End Property

        ''' <summary>
        ''' 构造降序等距子轨迹：<c>t_k = T − k·(T−1)/(steps−1)</c>，四舍五入并保证严格递减。
        ''' </summary>
        Private Shared Function BuildTrajectory(T As Integer, inferenceSteps As Integer) As Integer()
            Dim steps = std.Min(std.Max(inferenceSteps, 1), T)

            If steps = 1 Then Return New Integer() {T}

            Dim sequence As New List(Of Integer)(steps)
            Dim previous As Integer = Integer.MaxValue

            For k As Integer = 0 To steps - 1
                Dim raw = T - k * CDbl(T - 1) / (steps - 1)
                Dim t = CInt(std.Round(raw, MidpointRounding.AwayFromZero))
                t = std.Min(std.Max(t, 1), T)

                ' 保证严格递减并去重（当 steps 接近 T 时可能出现重复）
                If t >= previous Then t = previous - 1
                If t < 1 Then Exit For

                sequence.Add(t)
                previous = t
            Next

            Return sequence.ToArray()
        End Function

        ''' <summary>
        ''' 条件采样：从 <c>x_T</c> 出发逐步去噪生成 <c>x_0</c>。
        ''' </summary>
        ''' <param name="predictor">去噪网络。</param>
        ''' <param name="zSem">语义条件 <c>[B,dz]</c>（扰动预测时即 <c>z_sem^new</c>）。</param>
        ''' <param name="xT">起点噪声 <c>[B,G]</c>。</param>
        ''' <param name="snapshots">可选：记录各步状态以导出可视化数据。</param>
        Public Function Sample(predictor As INoisePredictor, zSem As Tensor, xT As Tensor,
                               Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
            Dim batch = xT.Shape(0)
            Dim x = xT

            Call Record(snapshots, 0, _trajectory(0), $"x_{_trajectory(0)}", x)

            For k As Integer = 0 To _trajectory.Length - 1
                Dim tFrom = _trajectory(k)

                If k = _trajectory.Length - 1 Then
                    ' 末步：直接输出 x̂_0（不把 t=0 送入网络）
                    Dim tArr = Repeat(tFrom, batch)
                    Dim eps = predictor.Predict(x, tArr, zSem, training:=False)
                    x = _schedule.PredictX0(x, eps, tArr)

                    Call Record(snapshots, k + 1, 0, "x_0", x)
                Else
                    x = Step(predictor, x, tFrom, _trajectory(k + 1), zSem)

                    Call Record(snapshots, k + 1, _trajectory(k + 1), $"x_{_trajectory(k + 1)}", x)
                End If
            Next

            Return x
        End Function

        ''' <summary>
        ''' 确定性反演（扩散自编码器的"编码"一半）：由 <c>x_0</c> 逐步"加噪"回到 <c>x_T</c>，
        ''' 得到只携带低层随机细节的随机子码。
        ''' 与 <see cref="Sample"/> 使用同一条子轨迹，因此 <c>Sample(Invert(x0)) ≈ x0</c>。
        ''' </summary>
        Public Function Invert(predictor As INoisePredictor, zSem As Tensor, x0 As Tensor,
                               Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
            Dim batch = x0.Shape(0)
            Dim tMin = _trajectory(_trajectory.Length - 1)

            Call Record(snapshots, 0, 0, "x_0", x0)

            ' 起点：用网络在当前 x_0 上预测的噪声把数据加噪到 t_min
            Dim tArr = Repeat(tMin, batch)
            Dim eps = predictor.Predict(x0, tArr, zSem, training:=False)
            Dim x = _schedule.AddNoise(x0, tArr, eps)

            Call Record(snapshots, 1, tMin, $"x_{tMin}", x)

            ' 沿子轨迹逐级升噪
            For k As Integer = _trajectory.Length - 2 To 0 Step -1
                x = Step(predictor, x, _trajectory(k + 1), _trajectory(k), zSem)

                Call Record(snapshots, _trajectory.Length - k, _trajectory(k), $"x_{_trajectory(k)}", x)
            Next

            Return x
        End Function

        ''' <summary>
        ''' 单步确定性更新：由时刻 <paramref name="tFrom"/> 的状态估计 <c>x̂_0</c>，
        ''' 再用同一份噪声预测重组合到时刻 <paramref name="tTo"/>。
        ''' 正向（<c>tTo &lt; tFrom</c>）即去噪；反向（<c>tTo &gt; tFrom</c>）即加噪，两者共用同一公式。
        ''' </summary>
        Public Function Step(predictor As INoisePredictor, x As Tensor, tFrom As Integer, tTo As Integer, zSem As Tensor) As Tensor
            Dim tArr = Repeat(tFrom, x.Shape(0))
            Dim eps = predictor.Predict(x, tArr, zSem, training:=False)
            Dim x0Hat = _schedule.PredictX0(x, eps, tArr)

            Return _schedule.Recombine(x0Hat, eps, tTo)
        End Function

        Private Shared Function Repeat(value As Integer, count As Integer) As Integer()
            Dim result(count - 1) As Integer
            For i As Integer = 0 To count - 1
                result(i) = value
            Next
            Return result
        End Function

        Private Shared Sub Record(snapshots As List(Of SamplingSnapshot), index As Integer,
                                  timestep As Integer, stage As String, x As Tensor)
            If snapshots Is Nothing Then Return

            snapshots.Add(New SamplingSnapshot With {
                .Index = index,
                .Timestep = timestep,
                .Stage = stage,
                .X = x
            })
        End Sub
    End Class
End Namespace
