Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.Diffusion
Imports SMRUCC.genomics.Analysis.Squiiff.Model
Imports std = System.Math

Namespace Perturbation

    ''' <summary>生成随机子码 <c>x_T</c> 的方式。</summary>
    Public Enum SubcodeMode
        ''' <summary>
        ''' <c>x_T ~ N(0, I)</c>：完全随机的低层细节。
        ''' 对应 README 第六节流程图的做法，生成的是该扰动的**群体样本**而非某个具体细胞的对应物。
        ''' </summary>
        Fresh = 0

        ''' <summary>
        ''' 沿用对照细胞自身的 DDIM 反演子码：保留该细胞的低层随机细节，
        ''' 因而可以与原始对照细胞**配对比较**（同一细胞扰动前 / 后），适合做逐细胞定量评估。
        ''' </summary>
        InheritControl = 1
    End Enum

    ''' <summary>
    ''' 虚拟扰动实验：把"扰动"实现为语义隐空间中的一次条件生成。
    '''
    ''' 流程（对应 README 第六节）：
    ''' <code>
    ''' 对照细胞 → z_sem^control
    ''' z_sem^new = z_sem^control + Δz_sem
    ''' 采样 x_T
    ''' 条件 DDIM 去噪 ε_θ(x_t, t, z_sem^new) → 预测扰动后转录组
    ''' </code>
    ''' </summary>
    Public Class InSilicoPerturbation

        Private ReadOnly _model As DiffusionAutoEncoder

        Public Sub New(model As DiffusionAutoEncoder)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            Me._model = model
        End Sub

        Public ReadOnly Property Model As DiffusionAutoEncoder
            Get
                Return _model
            End Get
        End Property

        ''' <summary>对照细胞的语义隐变量（取 <c>μ</c>，确定性）。</summary>
        Public Function SemanticOf(cells As Tensor) As Tensor
            Return _model.Encode(cells, training:=False)
        End Function

        ''' <summary>由细胞确定性反演得到的随机子码 <c>x_T</c>。</summary>
        Public Function SubcodeOf(cells As Tensor) As Tensor
            Return _model.EncodeToSubcode(cells)
        End Function

        ''' <summary>
        ''' 预测扰动后的转录组。
        ''' </summary>
        ''' <param name="controlCells">对照细胞 <c>[B,G]</c>。</param>
        ''' <param name="delta">扰动方向向量 <c>[1,dz]</c>；为 Nothing 时表示不施加扰动（用于重建对照）。</param>
        ''' <param name="mode">随机子码的采样方式。</param>
        ''' <param name="snapshots">可选：记录 DDIM 去噪过程。</param>
        Public Function Predict(controlCells As Tensor, delta As Tensor,
                                Optional mode As SubcodeMode = SubcodeMode.Fresh,
                                Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
            Dim zControl = _model.Encode(controlCells, training:=False)
            Dim zNew = If(delta Is Nothing, zControl, LatentArithmetic.ApplyDelta(zControl, delta))

            Dim subcode As Tensor
            Select Case mode
                Case SubcodeMode.InheritControl
                    subcode = _model.Sampler.Invert(_model.Denoiser, zControl, controlCells)
                Case Else
                    subcode = _model.Noise(controlCells.Shape)
            End Select

            Return _model.DecodeFromLatent(zNew, subcode, snapshots)
        End Function

        ''' <summary>
        ''' 按扰动名预测：从 <see cref="PerturbationSpace"/> 取已登记的 <c>Δz_sem</c> 做向量加法。
        ''' </summary>
        Public Function Predict(controlCells As Tensor, space As PerturbationSpace, name As String,
                                Optional mode As SubcodeMode = SubcodeMode.Fresh,
                                Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
            If space Is Nothing Then Throw New ArgumentNullException(NameOf(space))

            Return Predict(controlCells, space.DeltaOf(name), mode, snapshots)
        End Function

        ''' <summary>
        ''' 组合扰动外推：把若干已登记方向向量相加（<c>Σ Δz</c>）后做向量加法。
        '''
        ''' 这是"隐空间可加性假设"下的预测。若真实组合扰动含有单扰动之外的相互作用项
        ''' （README 第六节的非可加性场景），该预测会存在系统性偏差——偏差大小正是
        ''' 模型对非可加性的刻画能力的度量。
        ''' </summary>
        Public Function PredictCombination(controlCells As Tensor, space As PerturbationSpace,
                                           names As String(),
                                           Optional mode As SubcodeMode = SubcodeMode.Fresh,
                                           Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
            If space Is Nothing Then Throw New ArgumentNullException(NameOf(space))

            Return Predict(controlCells, space.Combine(names), mode, snapshots)
        End Function

        ''' <summary>
        ''' 估计一个扰动的方向向量并登记进扰动空间（一步完成 <c>编码 → 求组中心差 → 登记</c>）。
        ''' </summary>
        Public Function Estimate(space As PerturbationSpace, spec As PerturbationSpec,
                                 controlCells As Tensor, perturbedCells As Tensor) As Tensor
            Return space.Estimate(spec, _model, controlCells, perturbedCells)
        End Function

        ''' <summary>
        ''' 条件敏感度诊断：衡量 <c>z_sem</c> 是否真的在调制去噪网络 <c>ε_θ</c>。
        '''
        ''' 在若干时间步上比较同一 <c>x_t</c> 在 <c>z_sem</c> 与 <c>z_sem + Δz</c> 两个条件下的噪声预测：
        ''' <code>
        ''' rel(t) = ‖ ε̂(x_t, t, z + Δz) − ε̂(x_t, t, z) ‖ / ‖ ε̂(x_t, t, z) ‖
        ''' </code>
        ''' 若该比值接近 0，说明条件批归一化的 γ / β 投影尚未被训练开、
        ''' 语义条件没有真正进入去噪轨迹，此时任何隐空间向量算术都不会产生可观测的效应。
        ''' </summary>
        ''' <param name="xt">加噪后的表达谱 <c>[B,G]</c>。</param>
        ''' <param name="zSem">语义条件 <c>[B,dz]</c>。</param>
        ''' <param name="delta">施加的语义偏移 <c>[1,dz]</c>。</param>
        ''' <param name="timesteps">待检查的时间步；缺省取扩散轨迹上的若干代表点。</param>
        ''' <returns>与 <paramref name="timesteps"/> 一一对应的相对变化量。</returns>
        Public Function ConditioningSensitivity(xt As Tensor, zSem As Tensor, delta As Tensor,
                                                Optional timesteps As Integer() = Nothing) As Double()
            If delta Is Nothing Then Throw New ArgumentNullException(NameOf(delta))

            If timesteps Is Nothing OrElse timesteps.Length = 0 Then
                Dim total = _model.Schedule.T
                timesteps = New Integer() {
                    std.Max(1, CInt(total * 0.1)),
                    std.Max(1, CInt(total * 0.5)),
                    total
                }
            End If

            Dim batch = xt.Shape(0)
            Dim zShifted = LatentArithmetic.ApplyDelta(zSem, delta)
            Dim result(timesteps.Length - 1) As Double

            For i As Integer = 0 To timesteps.Length - 1
                Dim tArr = Repeat(timesteps(i), batch)

                Dim eps0 = _model.Denoiser.Predict(xt, tArr, zSem, training:=False)
                Dim eps1 = _model.Denoiser.Predict(xt, tArr, zShifted, training:=False)

                result(i) = RelativeDifference(eps0.Data, eps1.Data)
            Next

            Return result
        End Function

        Private Shared Function Repeat(value As Integer, count As Integer) As Integer()
            Dim result(count - 1) As Integer
            For i As Integer = 0 To count - 1
                result(i) = value
            Next
            Return result
        End Function

        ''' <summary>两组数值的相对差异：<c>‖b − a‖ / ‖a‖</c>（分母为 0 时返回 NaN）。</summary>
        Private Shared Function RelativeDifference(a As Double(), b As Double()) As Double
            Dim diffNorm As Double = 0.0
            Dim baseNorm As Double = 0.0

            For i As Integer = 0 To a.Length - 1
                Dim d = b(i) - a(i)
                diffNorm += d * d
                baseNorm += a(i) * a(i)
            Next

            If baseNorm <= 1.0E-300 Then Return Double.NaN

            Return std.Sqrt(diffNorm) / std.Sqrt(baseNorm)
        End Function

        ''' <summary>直接用给定的语义隐变量做条件生成（Lerp 轨迹逐点解码时使用）。</summary>
        Public Function DecodeAt(zSem As Tensor, subcode As Tensor,
                                 Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
            Return _model.DecodeFromLatent(zSem, subcode, snapshots)
        End Function
    End Class
End Namespace
