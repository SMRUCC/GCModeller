Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.Diffusion
Imports SMRUCC.genomics.Analysis.Squiiff.Model

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

        ''' <summary>直接用给定的语义隐变量做条件生成（Lerp 轨迹逐点解码时使用）。</summary>
        Public Function DecodeAt(zSem As Tensor, subcode As Tensor,
                                 Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
            Return _model.DecodeFromLatent(zSem, subcode, snapshots)
        End Function
    End Class
End Namespace
