Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.Transformer

Namespace Model

    ''' <summary>
    ''' 基因特异性线性解码器（method.md §4.3 步骤 5）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 每个基因拥有独立的读出权重 <c>W[i, :]</c> 与偏置 <c>b[i]</c>：
    ''' <code>
    ''' ŷ_i = b_i + Σ_k rep[i, k] · W[i, k]
    ''' </code>
    ''' 参数总量为 <c>N × (d + 1)</c>（N=2000、d=128 时约 26 万），
    ''' 远小于共享解码器所需的隐藏层规模，同时保留了"每个基因有自己的扰动响应模式"
    ''' 这一 GEARS 的核心设计；共享解码器会抹平这种异质性，是既有 GEARS 模型
    ''' 表达能力受限的原因之一。
    ''' </para>
    ''' <para>
    ''' 全部运算为双精度逐元素乘加，没有矩阵乘，因此不涉及混合精度降级。
    ''' </para>
    ''' </remarks>
    Public Class PerGeneDecoder

        Private ReadOnly nGenes As Integer
        Private ReadOnly featureDim As Integer

        Private ReadOnly W As Tensor
        Private ReadOnly b As Tensor

        Private ReadOnly wEntry As ParameterEntry
        Private ReadOnly bEntry As ParameterEntry

        Private _lastInput As Tensor

        ''' <summary>基因数量</summary>
        Public ReadOnly Property NumGenes As Integer
            Get
                Return nGenes
            End Get
        End Property

        ''' <summary>输入特征维度</summary>
        Public ReadOnly Property InputFeatureDim As Integer
            Get
                Return featureDim
            End Get
        End Property

        ''' <summary>
        ''' 创建基因特异性解码器。
        ''' </summary>
        ''' <param name="nGenes">基因数量（输出维度）</param>
        ''' <param name="featureDim">输入特征维度</param>
        ''' <param name="store">参数登记表</param>
        ''' <param name="name">参数名前缀</param>
        Public Sub New(nGenes As Integer, featureDim As Integer, store As ParameterStore,
                       Optional name As String = "decoder")

            If store Is Nothing Then Throw New ArgumentNullException(NameOf(store))
            If nGenes <= 0 OrElse featureDim <= 0 Then
                Throw New ArgumentException("解码器维度必须为正")
            End If

            Me.nGenes = nGenes
            Me.featureDim = featureDim

            ' 以 [featureDim, nGenes] 初始化（fan-in = featureDim），再转置为 [nGenes, featureDim]
            Dim initial As Tensor = TensorOps.HeNormalInit(New Integer() {featureDim, nGenes})
            W = TensorOps.TransposeLastTwo(initial)
            b = New Tensor(nGenes)

            wEntry = store.Register($"{name}.w", W)
            bEntry = store.Register($"{name}.b", b)
        End Sub

        ''' <summary>
        ''' 前向传播：把每个基因的表示读成标量输出。
        ''' </summary>
        ''' <param name="representation">基因表示 <c>[N, d]</c></param>
        ''' <returns>输出 <c>[N, 1]</c></returns>
        Public Function Forward(representation As Tensor) As Tensor
            If representation Is Nothing Then Throw New ArgumentNullException(NameOf(representation))
            If representation.Shape(0) <> nGenes OrElse representation.Shape(1) <> featureDim Then
                Throw New ArgumentException(
                    $"解码器输入形状应为 [{nGenes},{featureDim}]，实际 [{representation.Shape(0)},{representation.Shape(1)}]")
            End If

            _lastInput = representation

            Dim output As Tensor = New Tensor(nGenes, 1)
            Dim rd As Double() = representation.Data
            Dim wd As Double() = W.Data
            Dim bd As Double() = b.Data
            Dim od As Double() = output.Data

            For i As Integer = 0 To nGenes - 1
                Dim offset As Integer = i * featureDim
                Dim sum As Double = bd(i)

                For k As Integer = 0 To featureDim - 1
                    sum += rd(offset + k) * wd(offset + k)
                Next

                od(i) = sum
            Next

            Call output.MarkHostModified()

            Return output
        End Function

        ''' <summary>
        ''' 反向传播：累积权重/偏置梯度并返回输入梯度。
        ''' </summary>
        ''' <param name="dOut">输出梯度 <c>[N, 1]</c></param>
        ''' <returns>输入梯度 <c>[N, d]</c></returns>
        Public Function Backward(dOut As Tensor) As Tensor
            If _lastInput Is Nothing Then
                Throw New InvalidOperationException("必须先执行前向传播才能反向传播")
            End If

            Dim dInput As Tensor = New Tensor(nGenes, featureDim)
            Dim rd As Double() = _lastInput.Data
            Dim wd As Double() = W.Data
            Dim gd As Double() = dOut.Data
            Dim wid As Double() = wEntry.Gradient.Data
            Dim bid As Double() = bEntry.Gradient.Data
            Dim did As Double() = dInput.Data

            For i As Integer = 0 To nGenes - 1
                Dim g As Double = gd(i)
                Dim offset As Integer = i * featureDim

                bid(i) += g

                For k As Integer = 0 To featureDim - 1
                    did(offset + k) = g * wd(offset + k)
                    wid(offset + k) += g * rd(offset + k)
                Next
            Next

            Call dInput.MarkHostModified()
            Call wEntry.Gradient.MarkHostModified()
            Call bEntry.Gradient.MarkHostModified()

            Return dInput
        End Function

        ''' <summary>读出权重 <c>[N, d]</c>（诊断用）</summary>
        Public ReadOnly Property Weight As Tensor
            Get
                Return W
            End Get
        End Property

        ''' <summary>读出偏置 <c>[N]</c>（诊断用）</summary>
        Public ReadOnly Property Bias As Tensor
            Get
                Return b
            End Get
        End Property

    End Class

End Namespace
