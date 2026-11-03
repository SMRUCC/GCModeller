Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.Diffusion
Imports SMRUCC.genomics.Analysis.Squiiff.IO
Imports SMRUCC.genomics.Analysis.Squiiff.Model
Imports SMRUCC.genomics.Analysis.Squiiff.NN
Imports SMRUCC.genomics.Analysis.Squiiff.Perturbation
Imports SMRUCC.genomics.Analysis.Squiiff.Training
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Intervention

''' <summary>
''' SquiDiff 门面：串联「数据 → 建模型 → 训练 → 编码/重建 → 扰动预测 → 插值 → 存档」全流程。
'''
''' SquiDiff（Single-cell QUantitative Inference of stimuli responses by DIFFusion models）
''' 是一个基于**条件 DDIM** 与**扩散自编码器**的生成式框架：
''' <list type="number">
''' <item>语义编码器把表达谱压缩为低维语义隐变量 <c>z_sem</c>（携带细胞类型 / 状态 / 扰动等高层语义）；</item>
''' <item>确定性 DDIM 反演把表达谱编码为随机子码 <c>x_T</c>（携带低层随机细节）；</item>
''' <item>条件 DDIM 去噪网络以 <c>z_sem</c> 为显式条件（经条件批归一化注入）从噪声"雕刻"出目标转录组；</item>
''' <item>扰动效应通过在 <c>z_sem</c> 空间中做向量加法或线性插值来编码。</item>
''' </list>
''' </summary>
Public Class SquiDiff : Implements InsilicoPerturbationExperiment

    Private ReadOnly _model As DiffusionAutoEncoder
    Private ReadOnly _geneNames As String()
    Private _trainer As DiffusionTrainer
    Private _history As TrainingHistory

    ''' <param name="config">超参数配置。</param>
    ''' <param name="geneNames">基因名（行序即模型输入 / 输出维度的语义顺序）。</param>
    Public Sub New(config As SquiiffConfig, geneNames As String())
        If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))
        If geneNames Is Nothing OrElse geneNames.Length = 0 Then
            Throw New ArgumentException("基因名清单不能为空", NameOf(geneNames))
        End If

        Me.Config = config
        Me._geneNames = geneNames
        Me._model = New DiffusionAutoEncoder(config, geneNames.Length, config.Seed)
        Me.Perturbation = New InSilicoPerturbation(Me._model)
        Me.Space = New PerturbationSpace()
    End Sub

    ''' <summary>超参数。</summary>
    Public ReadOnly Property Config As SquiiffConfig

    ''' <summary>基因名清单。</summary>
    Public ReadOnly Property GeneNames As String()
        Get
            Return _geneNames
        End Get
    End Property

    ''' <summary>基因维度。</summary>
    Public ReadOnly Property GeneCount As Integer
        Get
            Return _geneNames.Length
        End Get
    End Property

    ''' <summary>底层扩散自编码器。</summary>
    Public ReadOnly Property Model As DiffusionAutoEncoder
        Get
            Return _model
        End Get
    End Property

    ''' <summary>虚拟扰动实验接口。</summary>
    Public ReadOnly Property Perturbation As InSilicoPerturbation

    ''' <summary>
    ''' 扰动空间：扰动名 / <see cref="PerturbationSpec"/> → 语义方向向量 <c>Δz_sem</c> 的登记表。
    ''' </summary>
    Public ReadOnly Property Space As PerturbationSpace

    ''' <summary>训练器（首次调用 <see cref="Train"/> 后可用）。</summary>
    Public ReadOnly Property Trainer As DiffusionTrainer
        Get
            Return _trainer
        End Get
    End Property

    ''' <summary>训练历史（首次调用 <see cref="Train"/> 后可用）。</summary>
    Public ReadOnly Property History As TrainingHistory
        Get
            Return _history
        End Get
    End Property

    ''' <summary>训练模型。</summary>
    ''' <remarks>
    ''' 训练时会顺带记录每个基因在<b>模型输入空间</b>（逐基因 z-score）中的经验分布，
    ''' 供 <see cref="SetContext"/> 的上下文对齐使用。
    ''' </remarks>
    Public Function Train(x0 As Tensor, Optional epochs As Integer? = Nothing, Optional verbose As Boolean = True) As TrainingHistory
        Me._trainer = New DiffusionTrainer(_model, verbose)
        Me._history = _trainer.Fit(x0, epochs)

        Call capturePopulation(x0)

        Return _history
    End Function

#Region "隐空间操作"

    ''' <summary>语义编码（确定性，返回 <c>μ</c>）。</summary>
    Public Function Semantic(x0 As Tensor) As Tensor
        Return _model.Encode(x0, training:=False)
    End Function

    ''' <summary>随机子码（DDIM 确定性反演 <c>x_0 → x_T</c>）。</summary>
    Public Function Subcode(x0 As Tensor) As Tensor
        Return _model.EncodeToSubcode(x0)
    End Function

    ''' <summary>自编码重建 <c>x_0 → (z_sem, x_T) → x̂_0</c>。</summary>
    Public Function Reconstruct(x0 As Tensor, Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
        Return _model.Reconstruct(x0, snapshots)
    End Function

    ''' <summary>估计扰动方向向量 <c>Δz_sem</c>。</summary>
    Public Function EstimateDelta(zPerturbed As Tensor, zControl As Tensor) As Tensor
        Return LatentArithmetic.EstimateDelta(zPerturbed, zControl)
    End Function

    ''' <summary>向量加法：把扰动叠加到对照细胞的语义隐变量上。</summary>
    Public Function ApplyDelta(zControl As Tensor, delta As Tensor) As Tensor
        Return LatentArithmetic.ApplyDelta(zControl, delta)
    End Function

    ''' <summary>线性插值 <c>(1−α)·z_A + α·z_B</c>。</summary>
    Public Function Interpolate(zA As Tensor, zB As Tensor, alpha As Double) As Tensor
        Return LatentArithmetic.Lerp(zA, zB, alpha)
    End Function

    ''' <summary>生成插值轨迹（含两端点）。</summary>
    Public Function InterpolateTrajectory(zA As Tensor, zB As Tensor, steps As Integer) As List(Of Tensor)
        Return LatentArithmetic.LerpTrajectory(zA, zB, steps)
    End Function

#End Region

#Region "扰动预测"

    ''' <summary>
    ''' 预测扰动后的转录组：对照细胞 + <c>Δz_sem</c> → 条件 DDIM 生成。
    ''' </summary>
    Public Function PredictPerturbation(controlCells As Tensor, delta As Tensor,
                                        Optional mode As SubcodeMode = SubcodeMode.Fresh,
                                        Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
        Return Me.Perturbation.Predict(controlCells, delta, mode, snapshots)
    End Function

    ''' <summary>
    ''' 估计一个扰动在语义隐空间中的方向向量 <c>Δz_sem</c> 并登记进 <see cref="Space"/>。
    ''' </summary>
    Public Function RegisterPerturbation(spec As PerturbationSpec,
                                         controlCells As Tensor,
                                         perturbedCells As Tensor) As Tensor
        Return Me.Space.Estimate(spec, _model, controlCells, perturbedCells)
    End Function

    ''' <summary>按扰动名预测（需先用 <see cref="RegisterPerturbation"/> 登记）。</summary>
    Public Function PredictByPerturbation(controlCells As Tensor, name As String,
                                          Optional mode As SubcodeMode = SubcodeMode.Fresh,
                                          Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
        Return Me.Perturbation.Predict(controlCells, Me.Space, name, mode, snapshots)
    End Function

    ''' <summary>
    ''' 组合扰动外推：把若干已登记方向向量相加（<c>Σ Δz</c>）后条件生成。
    ''' 用于检验非可加性——真实组合扰动含相互作用项时该预测会有系统性偏差。
    ''' </summary>
    Public Function PredictCombination(controlCells As Tensor, names As String(),
                                       Optional mode As SubcodeMode = SubcodeMode.Fresh,
                                       Optional snapshots As List(Of SamplingSnapshot) = Nothing) As Tensor
        Return Me.Perturbation.PredictCombination(controlCells, Me.Space, names, mode, snapshots)
    End Function

    ''' <summary>
    ''' 两个状态之间的插值生成轨迹：在 <c>z_sem</c> 空间做 Lerp 并逐点条件解码，
    ''' 用于生成连续过渡态（分化中间态等）。
    ''' </summary>
    Public Function InterpolateBetween(cellsA As Tensor, cellsB As Tensor, steps As Integer,
                                       Optional mode As SubcodeMode = SubcodeMode.Fresh) As List(Of Tensor)
        Dim zA = LatentArithmetic.MeanVector(Me.Semantic(cellsA))
        Dim zB = LatentArithmetic.MeanVector(Me.Semantic(cellsB))
        Dim trajectory = LatentArithmetic.LerpTrajectory(zA, zB, steps)
        Dim batch = cellsA.Shape(0)
        Dim decoded As New List(Of Tensor)(trajectory.Count)

        For Each z In trajectory
            ' 把 [1,dz] 的插值点广播到 [B,dz]，使每个细胞都按同一插值位置生成
            Dim zBatch = TensorUtil.BroadcastRow(z, batch)
            Dim subcode = If(mode = SubcodeMode.InheritControl,
                             _model.Sampler.Invert(_model.Denoiser, Me.Semantic(cellsA), cellsA),
                             _model.Noise(cellsA.Shape))

            decoded.Add(_model.DecodeFromLatent(zBatch, subcode))
        Next

        Return decoded
    End Function

#End Region

#Region "上下文与虚拟扰动（InsilicoPerturbationExperiment）"

    ''' <summary>
    ''' 上下文状态：默认 <see cref="ContextAlignmentMode.Quantile"/>。
    ''' </summary>
    ''' <remarks>
    ''' 扩散模型的输入是<b>逐基因 z-score 之后</b>的表达谱（HVG 子集），库本身不保存原始管线的
    ''' 文库大小与对数变换参数，因此无法做“原始计数 → 模型输入”的确定换算；
    ''' 分位对齐把外部 context 的跨基因排序映射到每个基因在训练细胞中的经验分布上，
    ''' 是这种信息条件下唯一不依赖具体管线公式的选择。
    ''' </remarks>
    Private m_context As ContextAlignmentState
    Private m_contextCells As Tensor
    Private m_population As Double()()

    Private ReadOnly Property ContextState As ContextAlignmentState
        Get
            If m_context Is Nothing Then
                m_context = New ContextAlignmentState(ContextAlignmentMode.Quantile)
            End If

            Return m_context
        End Get
    End Property

    ''' <summary>当前生效的上下文对齐模式。</summary>
    Public ReadOnly Property ContextAlignmentMode As ContextAlignmentMode
        Get
            Return ContextState.Mode
        End Get
    End Property

    ''' <summary>最近一次上下文注入的诊断。</summary>
    Public ReadOnly Property ContextDiagnostics As ContextAlignmentDiagnostics
        Get
            Return ContextState.Diagnostics
        End Get
    End Property

    ''' <summary>是否已经注入了上下文（对照细胞）。</summary>
    Public ReadOnly Property HasContext As Boolean
        Get
            Return m_contextCells IsNot Nothing
        End Get
    End Property

    ''' <summary>当前注入的对照细胞 <c>[1, G]</c>（模型输入空间）；未注入时为 Nothing。</summary>
    Public ReadOnly Property ContextCells As Tensor
        Get
            Return m_contextCells
        End Get
    End Property

    ''' <summary>
    ''' 把外部表达状态注入为扰动预测的对照细胞。
    ''' </summary>
    ''' <param name="context">基因名 → 表达值（大小写不敏感；可以只覆盖部分基因）</param>
    ''' <returns>当前对象（便于链式调用）</returns>
    ''' <remarks>
    ''' 未覆盖的基因在模型输入空间中取 0（= 训练细胞的平均表达水平）。
    ''' 必须在 <see cref="Train"/> 之后调用：分位对齐需要每个基因在训练细胞中的经验分布。
    ''' </remarks>
    ''' <exception cref="InvalidOperationException">模型尚未训练（缺少训练细胞的分布）</exception>
    Public Function SetContext(context As Dictionary(Of String, Double)) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContext
        Dim aligned As ContextAlignmentResult = ContextState.SetContext(context, buildContextReference())

        If aligned.Diagnostics.Matched = 0 Then
            Throw New InvalidOperationException("上下文中没有任何基因能够映射到模型的基因维度上")
        End If

        Call applyContextCells(aligned)

        Return Me
    End Function

    ''' <summary>切换上下文对齐模式；已注入上下文时会立即重新对齐。</summary>
    ''' <param name="mode">新的对齐模式</param>
    ''' <returns>当前对象（便于链式调用）</returns>
    Public Function SetContextAlignmentMode(mode As ContextAlignmentMode) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContextAlignmentMode
        Call ContextState.SetMode(mode)

        If ContextState.HasContext Then
            Call applyContextCells(ContextState.Result)
        End If

        Return Me
    End Function

    ''' <summary>撤销上下文。</summary>
    Public Sub ClearContext()
        Call ContextState.Clear()

        m_contextCells = Nothing
    End Sub

    ''' <summary>
    ''' 虚拟敲除：使用已登记的扰动方向向量生成扰动后的转录组。
    ''' </summary>
    ''' <param name="geneName">
    ''' 扰动名。扩散模型不能从基因名直接推出方向向量，必须先用
    ''' <see cref="RegisterPerturbation"/> 登记同名扰动
    ''' </param>
    ''' <param name="nSamples">保留参数（生成过程由采样器控制）</param>
    ''' <returns>模型输入空间（z-score）下的干预结果</returns>
    ''' <remarks>
    ''' 预测值与对照值都在模型输入空间（逐基因 z-score）中，未反变换回原始表达量——
    ''' 库不保存训练期的逐基因均值/标准差，无法可靠反算。
    ''' </remarks>
    ''' <exception cref="InvalidOperationException">尚未注入对照细胞（请先 SetContext）</exception>
    ''' <exception cref="KeyNotFoundException">该扰动尚未登记到 <see cref="Space"/></exception>
    Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
        Return predictByName(geneName, InterventionMode.Knockout)
    End Function

    ''' <summary>虚拟过表达：见 <see cref="KnockoutGene"/>（同样需要先登记方向向量）。</summary>
    Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
        Return predictByName(geneName, InterventionMode.Overexpression)
    End Function

    ''' <summary>虚拟敲低：见 <see cref="KnockoutGene"/>（同样需要先登记方向向量）。</summary>
    Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
        Return predictByName(geneName, InterventionMode.Knockdown)
    End Function

    Private Function predictByName(geneName As String, mode As InterventionMode) As InterventionResult
        If m_contextCells Is Nothing Then
            Throw New InvalidOperationException(
                "尚未注入对照细胞：请先调用 SetContext 指定扰动的起点，扩散模型没有默认细胞")
        End If

        Dim delta As Tensor

        If Not Space.TryGet(geneName, delta) Then
            Throw New KeyNotFoundException(
                $"扰动 '{geneName}' 尚未登记到扰动空间；请先用 RegisterPerturbation 登记方向向量")
        End If

        Dim predicted As Tensor = PredictPerturbation(m_contextCells, delta)
        Dim control As Double() = CType(m_contextCells.Data.Clone(), Double())
        Dim mutant As Double() = CType(predicted.Data.Clone(), Double())
        Dim n As Integer = control.Length
        Dim fold(n - 1) As Double
        Dim percent(n - 1) As Double
        Dim z(n - 1) As Double
        Dim significant(n - 1) As Boolean

        For i As Integer = 0 To n - 1
            fold(i) = mutant(i) - control(i)

            If System.Math.Abs(control(i)) > 1.0E-10 Then
                percent(i) = fold(i) / System.Math.Abs(control(i)) * 100.0R
            End If

            z(i) = fold(i)
            significant(i) = System.Math.Abs(fold(i)) > 1.0E-09
        Next

        Return New InterventionResult With {
            .Spec = New InterventionSpec With {.GeneName = geneName, .GeneIndex = -1, .Mode = mode},
            .GeneNames = CType(_geneNames.Clone(), String()),
            .WildtypeMeans = control,
            .MutantMeans = mutant,
            .FoldChanges = fold,
            .PercentChanges = percent,
            .ZScores = z,
            .IsSignificant = significant,
            .Undefined = False
        }
    End Function

    ''' <summary>把对齐结果铺成 <c>[1, G]</c> 的对照细胞张量（未覆盖基因取 0）。</summary>
    Private Sub applyContextCells(aligned As ContextAlignmentResult)
        Dim n As Integer = GeneCount
        Dim values(n - 1) As Double

        For Each item As KeyValuePair(Of String, Double) In aligned.Values
            Dim idx As Integer = Array.FindIndex(
                _geneNames,
                Function(g) String.Equals(g, item.Key, StringComparison.OrdinalIgnoreCase))

            If idx >= 0 Then
                values(idx) = item.Value
            End If
        Next

        m_contextCells = Tensor.Wrap(values, 1, n)
    End Sub

    ''' <summary>构造公共对齐组件所需的统计量（训练细胞在模型输入空间的经验分布）。</summary>
    Private Function buildContextReference() As ContextAlignmentReference
        Dim n As Integer = GeneCount
        Dim means(n - 1) As Double
        Dim sds(n - 1) As Double

        If m_population IsNot Nothing Then
            For i As Integer = 0 To n - 1
                Dim values As Double() = m_population(i)

                If values Is Nothing OrElse values.Length = 0 Then
                    Continue For
                End If

                Dim sum As Double = 0

                For Each v As Double In values
                    sum += v
                Next

                Dim mean As Double = sum / values.Length
                Dim ss As Double = 0

                For Each v As Double In values
                    ss += (v - mean) ^ 2
                Next

                means(i) = mean
                sds(i) = If(values.Length > 1, System.Math.Sqrt(ss / (values.Length - 1)), 0.0R)
            Next
        End If

        Return New ContextAlignmentReference With {
            .GeneNames = _geneNames,
            .Means = means,
            .StandardDeviations = sds,
            .Population = m_population
        }
    End Function

    ''' <summary>记录训练矩阵（模型输入空间）中每个基因的经验分布。</summary>
    Private Sub capturePopulation(x0 As Tensor)
        Dim cells As Integer = x0.Shape(0)
        Dim genes As Integer = x0.Shape(1)
        Dim population(genes - 1)() As Double
        Dim data As Double() = x0.Data

        For j As Integer = 0 To genes - 1
            Dim values(cells - 1) As Double

            For b As Integer = 0 To cells - 1
                values(b) = data(b * genes + j)
            Next

            population(j) = values
        Next

        m_population = population
        Call ContextState.Clear()

        m_contextCells = Nothing
    End Sub

#End Region

#Region "存档"

    ''' <summary>保存模型（超参数 + 基因名 + 全部参数）。</summary>
    Public Sub Save(path As String)
        Call SquiiffStorage.Save(_model, _geneNames, path)
    End Sub

    ''' <summary>读取模型存档。</summary>
    Public Shared Function Load(path As String) As SquiDiff
        Dim archive = SquiiffStorage.Load(path)
        Dim instance = New SquiDiff(archive.Config, archive.GeneNames)
        ' 用存档中的参数与批归一化滑动统计量覆盖新构建模型的对应状态
        Call CopyParameters(archive.Model, instance.Model)
        Call CopyStatistics(archive.Model, instance.Model)

        Return instance
    End Function

    Private Shared Sub CopyParameters(source As DiffusionAutoEncoder, target As DiffusionAutoEncoder)
        Dim map = target.Parameters.ToDictionary(Function(p) p.Name, Function(p) p, StringComparer.Ordinal)

        For Each p In source.Parameters
            Dim destination As Parameter = Nothing
            If Not map.TryGetValue(p.Name, destination) Then Continue For

            Call Array.Copy(p.Value.Data, destination.Value.Data, p.Value.Data.Length)
            Call destination.Value.MarkHostModified()
        Next
    End Sub

    ''' <summary>
    ''' 复制批归一化的滑动均值 / 方差。推理默认使用滑动统计量，
    ''' 若不复制，Load 回来的模型即使参数相同也会给出不同的生成结果。
    ''' </summary>
    Private Shared Sub CopyStatistics(source As DiffusionAutoEncoder, target As DiffusionAutoEncoder)
        Dim map = target.Normalizations.ToDictionary(Function(n) n.Name, Function(n) n, StringComparer.Ordinal)

        ' 循环变量名不能用 norm：Perturbation.LatentArithmetic.Norm(delta) 会被 VB 优先解析为函数
        For Each state In source.Normalizations
            Dim destination As IRunningStatistics = Nothing
            If Not map.TryGetValue(state.Name, destination) Then Continue For

            Call Array.Copy(state.RunningMean.Data, destination.RunningMean.Data, state.RunningMean.Data.Length)
            Call Array.Copy(state.RunningVariance.Data, destination.RunningVariance.Data, state.RunningVariance.Data.Length)

            Call destination.RunningMean.MarkHostModified()
            Call destination.RunningVariance.MarkHostModified()
        Next
    End Sub

#End Region
End Class
