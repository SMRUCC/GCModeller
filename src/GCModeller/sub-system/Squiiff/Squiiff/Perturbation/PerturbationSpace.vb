Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.Model
Imports SMRUCC.genomics.Analysis.Squiiff.NN
Imports std = System.Math

Namespace Perturbation

    ''' <summary>
    ''' 扰动空间：把"扰动名 / 元数据（<see cref="PerturbationSpec"/>）"与它在语义隐空间中的
    ''' 方向向量 <c>Δz_sem</c> 关联起来的登记表。
    '''
    ''' <c>SquiDiff</c> 在隐空间中把扰动表示为一个方向向量：
    ''' <code>
    ''' Δz_sem = mean(z_sem^perturbed) − mean(z_sem^control)
    ''' z_sem^new = z_sem^control + Δz_sem
    ''' </code>
    ''' 本类型负责**登记并复用**这些方向向量，从而支持：
    ''' <list type="number">
    ''' <item>按扰动名直接预测（<see cref="Get"/> + <see cref="InSilicoPerturbation.Predict"/>）；</item>
    ''' <item><b>组合扰动外推</b>（<see cref="Combine"/>）：把若干单扰动的 <c>Δz</c> 向量相加。
    '''       这是 README 第六节强调的"非可加性"检验场景——若真实组合扰动含相互作用项，
    '''       向量相加的预测必然存在系统性偏差，偏差大小即模型对非可加性的刻画能力。</item>
    ''' <item>跨细胞类型方向一致性分析（同一扰动在不同细胞类型上的 <c>Δz</c> 余弦相似度）。</item>
    ''' </list>
    '''
    ''' 注意 <c>Δz_sem</c> 代表的是一段扰动/分化的**平均方向**，而非精确的瞬时动态。
    ''' </summary>
    Public Class PerturbationSpace

        Private ReadOnly _deltas As New Dictionary(Of String, Tensor)(StringComparer.OrdinalIgnoreCase)
        Private ReadOnly _specs As New Dictionary(Of String, PerturbationSpec)(StringComparer.OrdinalIgnoreCase)
        Private ReadOnly _order As New List(Of String)

        ''' <summary>已登记的扰动名（按登记顺序）。</summary>
        Public ReadOnly Property Names As IReadOnlyList(Of String)
            Get
                Return _order
            End Get
        End Property

        ''' <summary>扰动名 → 元数据的只读视图。</summary>
        Public ReadOnly Property Specs As IReadOnlyDictionary(Of String, PerturbationSpec)
            Get
                Return _specs
            End Get
        End Property

        ''' <summary>已登记的扰动个数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return _order.Count
            End Get
        End Property

#Region "登记"

        ''' <summary>
        ''' 用对照 / 扰动两组细胞的语义隐变量中心估计 <c>Δz_sem</c> 并登记。
        ''' </summary>
        ''' <param name="spec">扰动元数据（<c>Name</c> 作为主键）。</param>
        ''' <param name="model">已训练的扩散自编码器（提供语义编码）。</param>
        ''' <param name="controlCells">对照细胞 <c>[B,G]</c>。</param>
        ''' <param name="perturbedCells">受扰动细胞 <c>[B,G]</c>。</param>
        ''' <returns>估计得到的 <c>Δz_sem</c>（<c>[1,dz]</c>）。</returns>
        Public Function Estimate(spec As PerturbationSpec,
                                 model As DiffusionAutoEncoder,
                                 controlCells As Tensor,
                                 perturbedCells As Tensor) As Tensor
            If spec Is Nothing Then Throw New ArgumentNullException(NameOf(spec))
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            If String.IsNullOrWhiteSpace(spec.Name) Then Throw New ArgumentException("扰动名不能为空", NameOf(spec))

            Dim zControl = model.Encode(controlCells, training:=False)
            Dim zPerturbed = model.Encode(perturbedCells, training:=False)
            Dim delta = LatentArithmetic.EstimateDelta(zPerturbed, zControl)

            Return Register(spec, delta)
        End Function

        ''' <summary>直接登记一个已知的方向向量（例如由外部计算或从存档恢复）。</summary>
        Public Function Register(spec As PerturbationSpec, delta As Tensor) As Tensor
            If spec Is Nothing Then Throw New ArgumentNullException(NameOf(spec))
            If delta Is Nothing Then Throw New ArgumentNullException(NameOf(delta))
            If String.IsNullOrWhiteSpace(spec.Name) Then Throw New ArgumentException("扰动名不能为空", NameOf(spec))

            If Not _deltas.ContainsKey(spec.Name) Then _order.Add(spec.Name)

            _specs(spec.Name) = spec
            _deltas(spec.Name) = delta

            Return delta
        End Function

#End Region

#Region "查询"

        ''' <summary>按名取方向向量；不存在返回 Nothing。</summary>
        Public Function TryGet(name As String, ByRef delta As Tensor) As Boolean
            If String.IsNullOrEmpty(name) Then
                delta = Nothing
                Return False
            End If

            Return _deltas.TryGetValue(name, delta)
        End Function

        ''' <summary>按名取方向向量；不存在时抛异常。</summary>
        Public Function DeltaOf(name As String) As Tensor
            Dim delta As Tensor = Nothing
            If Not TryGet(name, delta) Then
                Throw New KeyNotFoundException($"扰动 '{name}' 尚未登记到扰动空间；已登记: {Describe()}")
            End If

            Return delta
        End Function

        ''' <summary>按名取元数据；不存在返回 Nothing。</summary>
        Public Function TryGetSpec(name As String, ByRef spec As PerturbationSpec) As Boolean
            If String.IsNullOrEmpty(name) Then
                spec = Nothing
                Return False
            End If

            Return _specs.TryGetValue(name, spec)
        End Function

        ''' <summary>
        ''' 组合外推：把若干已登记的方向向量相加 <c>Σ Δz_i</c>，得到组合扰动的方向。
        ''' 这是"隐空间可加性假设"下的预测；真实组合扰动若含相互作用项则会产生系统性偏差。
        ''' </summary>
        Public Function Combine(ParamArray names As String()) As Tensor
            If names Is Nothing OrElse names.Length = 0 Then
                Throw New ArgumentException("至少需要一个扰动名", NameOf(names))
            End If

            Dim sum As Tensor = Nothing

            For Each name In names
                Dim delta = Me.DeltaOf(name)

                If sum Is Nothing Then
                    sum = New Tensor(delta.Shape)
                    Call Array.Copy(delta.Data, sum.Data, delta.Data.Length)
                    Call sum.MarkHostModified()
                Else
                    sum = sum + delta
                End If
            Next

            Return sum
        End Function

        ''' <summary>组合外推，并把结果登记为一个新的（组合）扰动。</summary>
        Public Function CombineAs(spec As PerturbationSpec, ParamArray names As String()) As Tensor
            Return Register(spec, Combine(names))
        End Function

#End Region

        ''' <summary>逐个扰动给出"方向向量范数 + 主导分量"的可读摘要。</summary>
        Public Function Describe(Optional labels As String() = Nothing, Optional topN As Integer = 4) As String
            If _order.Count = 0 Then Return "（空）"

            Dim parts As New List(Of String)
            For Each name In _order
                Dim spec = _specs(name)
                parts.Add($"{name}[{spec.Kind}]: {LatentArithmetic.Describe(_deltas(name), labels, topN)}")
            Next

            Return String.Join(" | ", parts)
        End Function

        Public Overrides Function ToString() As String
            Return $"扰动空间：{_order.Count} 个扰动（{String.Join(", ", _order)}）"
        End Function
    End Class
End Namespace
