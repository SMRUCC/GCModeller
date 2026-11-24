Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Intervention
Imports std = System.Math

''' <summary>
''' 基于条件变分自编码器的虚拟细胞扰动实验门面。
'''
''' 与 <c>Squiiff</c> 的 <c>SquiDiff</c> 门面同构：底层模型只负责
''' <c>q(z|x,c)</c> / <c>p(x|z,c)</c> 的编码与解码，本类型负责实验语义
''' （基因名、条件名、上下文注入、扰动登记、干预结果与存档）。
'''
''' 反事实路径统一为「同一隐变量换条件」：
''' <code>
''' z       = Enc(context, c_control)
''' wildtype = Dec(context, c_control)
''' mutant   = Dec(context, c_target)
''' </code>
''' 由于 <see cref="ConditionalVAE"/> 的条件是离散的 one-hot，<c>Knockout</c> /
''' <c>Overexpression</c> / <c>Knockdown</c> 共用同一条反事实路径，差异体现在
''' <see cref="InterventionSpec.Mode"/> 与所登记的离散条件上。
''' </summary>
''' <remarks>
''' 扰动登记支持两种方式：
''' <list type="number">
''' <item>隐式：基因名与条件名大小写不敏感匹配；</item>
''' <item>显式：<see cref="RegisterPerturbation(String, String)"/> 覆盖默认映射。</item>
''' </list>
''' 两者都无法解析时抛出 <see cref="KeyNotFoundException"/>，与 Squiiff 的
''' “未登记扰动不可预测”约定一致。
''' </remarks>
Public Class VAGeneExperiment
    Implements InsilicoPerturbationExperiment

    Private ReadOnly _model As ConditionalVAE
    Private ReadOnly _geneNames As String()
    Private ReadOnly _conditionNames As String()
    Private ReadOnly _contextState As ContextAlignmentState
    Private ReadOnly _registry As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

    Private _contextExpression As Tensor
    Private _population As Double()()
    Private _means As Double()
    Private _standardDeviations As Double()
    Private _controlCondition As Integer

    ''' <summary>
    ''' 用底层条件 VAE 与基因 / 条件名清单构造实验门面。
    ''' </summary>
    ''' <param name="model">已构造的条件 VAE（可以尚未训练）。</param>
    ''' <param name="geneNames">基因名，顺序必须与模型输入 / 输出维度一致。</param>
    ''' <param name="conditionNames">离散条件名，下标即 one-hot 条件标签。</param>
    Public Sub New(model As ConditionalVAE, geneNames As String(), conditionNames As String())
        If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
        If geneNames Is Nothing OrElse geneNames.Length = 0 Then
            Throw New ArgumentException("基因名清单不能为空", NameOf(geneNames))
        End If
        If conditionNames Is Nothing OrElse conditionNames.Length = 0 Then
            Throw New ArgumentException("条件名清单不能为空", NameOf(conditionNames))
        End If
        If geneNames.Length <> model.GeneCount Then
            Throw New ArgumentException($"基因名数量 {geneNames.Length} 与模型基因维度 {model.GeneCount} 不一致", NameOf(geneNames))
        End If
        If conditionNames.Length <> model.ConditionCount Then
            Throw New ArgumentException($"条件名数量 {conditionNames.Length} 与模型条件数量 {model.ConditionCount} 不一致", NameOf(conditionNames))
        End If

        Me._model = model
        Me._geneNames = CType(geneNames.Clone(), String())
        Me._conditionNames = CType(conditionNames.Clone(), String())
        Me._contextState = New ContextAlignmentState(ContextAlignmentMode.Quantile)
        Me._controlCondition = ResolveControlCondition(Me._conditionNames)
    End Sub

    ''' <summary>底层条件 VAE。</summary>
    Public ReadOnly Property Model As ConditionalVAE
        Get
            Return _model
        End Get
    End Property

    ''' <summary>基因名清单（顺序即模型输出维度语义）。</summary>
    Public ReadOnly Property GeneNames As String()
        Get
            Return _geneNames
        End Get
    End Property

    ''' <summary>离散条件名清单。</summary>
    Public ReadOnly Property ConditionNames As String()
        Get
            Return _conditionNames
        End Get
    End Property

    ''' <summary>当前使用的对照条件下标。</summary>
    Public ReadOnly Property ControlCondition As Integer
        Get
            Return _controlCondition
        End Get
    End Property

    ''' <summary>模型是否已经训练。</summary>
    Public ReadOnly Property IsFitted As Boolean
        Get
            Return _model.IsFitted
        End Get
    End Property

    ''' <summary>是否已经注入对照细胞上下文。</summary>
    Public ReadOnly Property HasContext As Boolean
        Get
            Return _contextExpression IsNot Nothing
        End Get
    End Property

    ''' <summary>当前上下文对齐模式。</summary>
    Public ReadOnly Property AlignmentMode As ContextAlignmentMode
        Get
            Return _contextState.Mode
        End Get
    End Property

    ''' <summary>最近一次上下文对齐的诊断；未注入上下文时为 <c>Nothing</c>。</summary>
    Public ReadOnly Property ContextDiagnostics As ContextAlignmentDiagnostics
        Get
            Return _contextState.Diagnostics
        End Get
    End Property

    ''' <summary>基因名 → 条件下标的显式扰动登记表（只读视图）。</summary>
    Public ReadOnly Property RegisteredPerturbations As IReadOnlyDictionary(Of String, Integer)
        Get
            Return _registry
        End Get
    End Property

#Region "训练"

    ''' <summary>
    ''' 训练底层条件 VAE，并采集逐基因训练统计量供上下文对齐使用。
    ''' </summary>
    ''' <remarks>
    ''' 统计量取自 <see cref="PerturbationDataset.LogNormalize"/>（模型输入空间），
    ''' 因此 <see cref="ContextAlignmentMode.Quantile"/> / <c>Affine</c> 的对齐结果
    ''' 可以直接送入编码器。
    ''' </remarks>
    Public Function Train(dataset As PerturbationDataset,
                          Optional epochs As Integer = 100,
                          Optional batchSize As Integer = 64,
                          Optional learningRate As Double = 0.001,
                          Optional beta As Double = 1.0,
                          Optional verbose As Boolean = True) As VAGeneExperiment
        If dataset Is Nothing Then Throw New ArgumentNullException(NameOf(dataset))
        If dataset.GeneCount <> _model.GeneCount Then
            Throw New ArgumentException($"数据基因数量 {dataset.GeneCount} 与模型 {_model.GeneCount} 不一致", NameOf(dataset))
        End If
        If dataset.ConditionCount <> _model.ConditionCount Then
            Throw New ArgumentException($"数据条件数量 {dataset.ConditionCount} 与模型 {_model.ConditionCount} 不一致", NameOf(dataset))
        End If

        Call _model.Fit(dataset, epochs, batchSize, learningRate, beta, verbose)
        Call capturePopulation(dataset)

        Me._controlCondition = dataset.ControlCondition()
        Me._contextState.Clear()
        Me._contextExpression = Nothing

        Return Me
    End Function

    ''' <summary>记录模型输入空间（log1p 文库归一化）中每个基因的经验分布。</summary>
    Private Sub capturePopulation(dataset As PerturbationDataset)
        Dim expression As Tensor = dataset.LogNormalize()
        Dim cells As Integer = expression.Shape(0)
        Dim genes As Integer = expression.Shape(1)
        Dim data As Double() = expression.Data
        Dim population(genes - 1)() As Double

        For g As Integer = 0 To genes - 1
            Dim values(cells - 1) As Double

            For i As Integer = 0 To cells - 1
                values(i) = data(i * genes + g)
            Next

            population(g) = values
        Next

        Call restorePopulation(population)
    End Sub

    ''' <summary>由逐基因种群重新计算均值 / 标准差缓存。</summary>
    Private Sub restorePopulation(population As Double()())
        Me._population = population
        Me._means = New Double(_geneNames.Length - 1) {}
        Me._standardDeviations = New Double(_geneNames.Length - 1) {}

        If population Is Nothing Then Return

        For g As Integer = 0 To std.Min(population.Length, _geneNames.Length) - 1
            Dim values As Double() = population(g)

            If values Is Nothing OrElse values.Length = 0 Then Continue For

            Dim sum As Double = 0.0

            For Each v As Double In values
                sum += v
            Next

            Dim mean As Double = sum / values.Length
            Dim ss As Double = 0.0

            For Each v As Double In values
                ss += (v - mean) ^ 2
            Next

            Me._means(g) = mean
            Me._standardDeviations(g) = If(values.Length > 1, std.Sqrt(ss / (values.Length - 1)), 0.0)
        Next
    End Sub

#End Region

#Region "上下文"

    ''' <summary>构造上下文对齐所需的训练侧参考统计量。</summary>
    Private Function buildReference() As ContextAlignmentReference
        Return New ContextAlignmentReference With {
            .GeneNames = _geneNames,
            .Means = _means,
            .StandardDeviations = _standardDeviations,
            .Population = _population
        }
    End Function

    ''' <summary>
    ''' 注入对照细胞上下文；未覆盖的基因按训练均值填充。
    ''' </summary>
    Public Function SetContext(context As Dictionary(Of String, Double)) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContext
        Dim aligned As ContextAlignmentResult = _contextState.SetContext(context, buildReference())

        If aligned.Diagnostics Is Nothing OrElse aligned.Diagnostics.Matched = 0 Then
            Throw New InvalidOperationException("上下文中没有任何基因能够映射到模型的基因维度上")
        End If

        Call applyContext()

        Return Me
    End Function

    ''' <summary>切换上下文对齐模式；已注入上下文时会立即重新对齐。</summary>
    Public Function SetContextAlignmentMode(mode As ContextAlignmentMode) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContextAlignmentMode
        Call _contextState.SetMode(mode)

        If _contextState.HasContext Then
            Call applyContext()
        End If

        Return Me
    End Function

    ''' <summary>撤销上下文。</summary>
    Public Sub ClearContext()
        Call _contextState.Clear()
        Me._contextExpression = Nothing
    End Sub

    ''' <summary>把对齐结果铺成 <c>[1, G]</c> 的模型输入空间向量。</summary>
    Private Sub applyContext()
        Dim vector As Double() = _contextState.ToVector(ContextMissingGeneFill.Mean)

        If vector Is Nothing Then
            Me._contextExpression = Nothing
            Return
        End If

        Me._contextExpression = Tensor.Wrap(vector, 1, _geneNames.Length)
    End Sub

#End Region

#Region "扰动登记"

    ''' <summary>显式登记「基因名 → 条件名」的扰动映射。</summary>
    Public Function RegisterPerturbation(geneName As String, conditionName As String) As VAGeneExperiment
        If String.IsNullOrWhiteSpace(geneName) Then
            Throw New ArgumentException("基因名不能为空", NameOf(geneName))
        End If

        Dim index As Integer = Array.FindIndex(
            _conditionNames,
            Function(name) String.Equals(name, conditionName, StringComparison.OrdinalIgnoreCase))

        If index < 0 Then
            Throw New ArgumentException($"未知的实验条件: {conditionName}", NameOf(conditionName))
        End If

        Return RegisterPerturbation(geneName, index)
    End Function

    ''' <summary>显式登记「基因名 → 条件下标」的扰动映射。</summary>
    Public Function RegisterPerturbation(geneName As String, conditionIndex As Integer) As VAGeneExperiment
        If String.IsNullOrWhiteSpace(geneName) Then
            Throw New ArgumentException("基因名不能为空", NameOf(geneName))
        End If
        If conditionIndex < 0 OrElse conditionIndex >= _conditionNames.Length Then
            Throw New ArgumentOutOfRangeException(NameOf(conditionIndex))
        End If

        Me._registry(geneName.Trim()) = conditionIndex

        Return Me
    End Function

    ''' <summary>先查显式登记表，再按条件名大小写不敏感匹配。</summary>
    Private Function resolveCondition(geneName As String) As Integer
        Dim index As Integer

        If _registry.TryGetValue(geneName, index) Then
            Return index
        End If

        index = Array.FindIndex(
            _conditionNames,
            Function(name) String.Equals(name, geneName, StringComparison.OrdinalIgnoreCase))

        If index < 0 Then
            Throw New KeyNotFoundException(
                $"扰动 '{geneName}' 尚未登记到扰动表；请先调用 RegisterPerturbation 或使用与条件名一致的基因名")
        End If

        Return index
    End Function

#End Region

#Region "干预预测"

    ''' <summary>虚拟敲除：按登记条件生成反事实表达谱。</summary>
    ''' <param name="geneName">基因名（或登记时使用的扰动名）。</param>
    ''' <param name="nSamples">保留参数；生成过程由隐变量与条件决定。</param>
    Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
        Return predict(geneName, InterventionMode.Knockout)
    End Function

    ''' <summary>虚拟过表达：见 <see cref="KnockoutGene"/>。</summary>
    Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
        Return predict(geneName, InterventionMode.Overexpression)
    End Function

    ''' <summary>虚拟敲低：见 <see cref="KnockoutGene"/>。</summary>
    Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
        Return predict(geneName, InterventionMode.Knockdown)
    End Function

    ''' <summary>同一隐变量换条件：对照条件下解码为野生型，目标条件下解码为扰动后。</summary>
    Private Function predict(geneName As String, mode As InterventionMode) As InterventionResult
        If _contextExpression Is Nothing Then
            Throw New InvalidOperationException("尚未注入对照细胞：请先调用 SetContext 指定扰动的起点")
        End If
        If Not _model.IsFitted Then
            Throw New InvalidOperationException("模型尚未训练, 请先调用 Train")
        End If

        Dim target As Integer = resolveCondition(geneName)

        If target = _controlCondition Then
            Throw New ArgumentException($"目标条件与对照条件相同: {_conditionNames(target)}", NameOf(geneName))
        End If

        Dim latent As Tensor = _model.EncodeExpression(_contextExpression, _controlCondition)
        Dim wildtype As Tensor = _model.DecodeLatent(latent, _controlCondition)
        Dim mutant As Tensor = _model.DecodeLatent(latent, target)

        Dim controlValues As Double() = CType(wildtype.Data.Clone(), Double())
        Dim mutantValues As Double() = CType(mutant.Data.Clone(), Double())
        Dim n As Integer = _geneNames.Length
        Dim fold(n - 1) As Double
        Dim percent(n - 1) As Double
        Dim z(n - 1) As Double
        Dim significant(n - 1) As Boolean

        For i As Integer = 0 To n - 1
            fold(i) = mutantValues(i) - controlValues(i)

            If std.Abs(controlValues(i)) > 1.0E-10 Then
                percent(i) = fold(i) / std.Abs(controlValues(i)) * 100.0
            End If

            ' 与 Squiiff 的约定一致：ZScores 目前存放计数空间的绝对变化量（delta），
            ' 而不是除以标准差的标准化 z 分数。
            z(i) = fold(i)
            significant(i) = std.Abs(fold(i)) > 1.0E-09
        Next

        Dim geneIndex As Integer = Array.FindIndex(
            _geneNames,
            Function(name) String.Equals(name, geneName, StringComparison.OrdinalIgnoreCase))

        Return New InterventionResult With {
            .Spec = New InterventionSpec With {
                .GeneName = geneName,
                .GeneIndex = geneIndex,
                .Mode = mode
            },
            .GeneNames = CType(_geneNames.Clone(), String()),
            .WildtypeMeans = controlValues,
            .MutantMeans = mutantValues,
            .FoldChanges = fold,
            .PercentChanges = percent,
            .ZScores = z,
            .IsSignificant = significant,
            .Undefined = False
        }
    End Function

#End Region

#Region "存档"

    ''' <summary>保存模型、基因 / 条件名、训练统计、扰动登记表与对齐模式。</summary>
    Public Sub Save(path As String)
        If Not _model.IsFitted Then
            Throw New InvalidOperationException("模型尚未训练, 无法保存")
        End If

        Call VAGeneStorage.Save(_model, _geneNames, _conditionNames, _population, _registry,
                                _controlCondition, _contextState.Mode, path)
    End Sub

    ''' <summary>读取 <see cref="Save"/> 写出的门面存档。</summary>
    Public Shared Function Load(path As String) As VAGeneExperiment
        Dim archive As VAGeneArchive = VAGeneStorage.Load(path)
        Dim experiment As New VAGeneExperiment(archive.Model, archive.GeneNames, archive.ConditionNames)

        Call experiment.RestoreMetadata(archive.Population, archive.Registry,
                                        archive.ControlCondition, archive.AlignmentMode)

        Return experiment
    End Function

    ''' <summary>用存档中的元数据覆盖门面状态（供存档加载使用）。</summary>
    Friend Sub RestoreMetadata(population As Double()(), registry As Dictionary(Of String, Integer),
                               controlCondition As Integer, mode As ContextAlignmentMode)
        Call restorePopulation(population)

        Me._registry.Clear()

        If registry IsNot Nothing Then
            For Each item As KeyValuePair(Of String, Integer) In registry
                Me._registry(item.Key) = item.Value
            Next
        End If

        If controlCondition >= 0 AndAlso controlCondition < _conditionNames.Length Then
            Me._controlCondition = controlCondition
        End If

        Call _contextState.SetMode(mode)
        Me._contextExpression = Nothing
    End Sub

#End Region

#Region "工具"

    ''' <summary>按名称启发式识别对照条件（control / ctrl / mock / wt）。</summary>
    Private Shared Function ResolveControlCondition(names As String()) As Integer
        Dim keywords As String() = {"control", "ctrl", "mock", "wildtype", "wild-type", "wt"}

        For i As Integer = 0 To names.Length - 1
            Dim name As String = If(names(i), "")

            For Each keyword As String In keywords
                If name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return i
                End If
            Next
        Next

        Return 0
    End Function

#End Region

End Class
