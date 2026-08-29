Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports GNN = Microsoft.VisualBasic.DeepLearning.GNN

Namespace Layers

    ''' <summary>
    ''' 基因身份嵌入层（Gene identity embedding）
    ''' </summary>
    ''' <remarks>
    ''' 对应 readme Step 2 中的可学习基因身份向量 <c>e_i ∈ R^d</c>（类似 word embedding，
    ''' 维度通常取 16~256）。
    '''
    ''' 本层同时承担 GEARS「扰动基因集合编码器」的第一段工作：
    ''' <list type="number">
    ''' <item><description>把输入的扰动 multi-hot 标记 <c>p</c> 作用到嵌入表上，得到被扰动基因的嵌入子集 <c>p_i · e_i</c>；</description></item>
    ''' <item><description>该结果随后交给 <see cref="GNN.GlobalPoolingLayer"/> 做 Deep Sets 均值池化，
    ''' 得到与基因顺序无关的全局扰动向量 <c>z_pert</c>，再拼接到每一个节点的特征上。</description></item>
    ''' </list>
    '''
    ''' 嵌入表的梯度通过池化路径回传并在这里累积；由于优化器持有梯度张量引用，
    ''' 反向传播只允许原地累加，不允许重新分配梯度张量。
    ''' </remarks>
    Public Class GeneEmbeddingLayer
        Inherits GNN.Layer

        ''' <summary>基因身份嵌入表 [numGenes, embeddingDim]</summary>
        ReadOnly embedding As Tensor

        ''' <summary>嵌入表梯度 [numGenes, embeddingDim]</summary>
        ReadOnly embeddingGrad As Tensor

        ''' <summary>上一次前向传播使用的扰动标记</summary>
        Dim lastFlag As Tensor

        ''' <summary>基因数量</summary>
        ''' <returns>嵌入表的行数</returns>
        Public ReadOnly Property NumGenes As Integer

        ''' <summary>嵌入向量维度</summary>
        ''' <returns>嵌入表的列数</returns>
        Public ReadOnly Property EmbeddingDim As Integer

        ''' <summary>
        ''' 获取基因身份嵌入表（供模型拼接节点特征时读取当前值）
        ''' </summary>
        ''' <returns>嵌入张量 [numGenes, embeddingDim]</returns>
        Public ReadOnly Property Embeddings As Tensor
            Get
                Return embedding
            End Get
        End Property

        ''' <summary>
        ''' 创建基因身份嵌入层
        ''' </summary>
        ''' <param name="numGenes">基因数量</param>
        ''' <param name="embeddingDim">嵌入向量维度</param>
        ''' <param name="scale">初始化缩放系数</param>
        ''' <param name="seed">随机初始化种子；给定种子可保证实验可复现</param>
        ''' <param name="name">层名称</param>
        Public Sub New(numGenes As Integer,
                       embeddingDim As Integer,
                       Optional scale As Double = 1.0,
                       Optional seed As Integer? = Nothing,
                       Optional name As String = Nothing)

            Me.NumGenes = numGenes
            Me.EmbeddingDim = embeddingDim
            MyBase.Name = If(name, $"GeneEmbedding_{numGenes}_{embeddingDim}")

            embedding = Tensor.XavierInit(numGenes, embeddingDim, seed)
            embeddingGrad = New Tensor(numGenes, embeddingDim)

            If scale <> 1.0 Then
                Dim ed As Double() = embedding.Data

                For i As Integer = 0 To ed.Length - 1
                    ed(i) *= scale
                Next
            End If
        End Sub

        ''' <summary>
        ''' 前向传播：输出被扰动基因掩码之后的嵌入矩阵
        ''' </summary>
        ''' <param name="input">
        ''' 扰动 multi-hot 标记，可以是 [numGenes, 1] 的二维张量，也可以是 [numGenes] 的一维张量
        ''' </param>
        ''' <returns>掩码后的嵌入 [numGenes, embeddingDim]，其中未被扰动的基因整行为 0</returns>
        Public Overrides Function Forward(input As Tensor) As Tensor
            lastFlag = input

            Dim n As Integer = NumGenes
            Dim d As Integer = EmbeddingDim
            Dim output As Tensor = New Tensor(n, d)
            Dim ed As Double() = embedding.Data
            Dim od As Double() = output.Data
            Dim fd As Double() = input.Data

            For i As Integer = 0 To n - 1
                Dim p As Double = fd(i)

                If p = 0.0 Then
                    Continue For
                End If

                Dim off As Integer = i * d

                For j As Integer = 0 To d - 1
                    od(off + j) = p * ed(off + j)
                Next
            Next

            Return output
        End Function

        ''' <summary>
        ''' 反向传播：按扰动掩码把梯度累积回嵌入表
        ''' </summary>
        ''' <param name="gradient">上游梯度 [numGenes, embeddingDim]</param>
        ''' <returns>相对于输入扰动标记的梯度（形状与上一次 <see cref="Forward(Tensor)"/> 的输入一致）</returns>
        Public Overrides Function Backward(gradient As Tensor) As Tensor
            Dim n As Integer = NumGenes
            Dim d As Integer = EmbeddingDim
            Dim gd As Double() = gradient.Data
            Dim eg As Double() = embeddingGrad.Data
            Dim fd As Double() = If(lastFlag Is Nothing, Nothing, lastFlag.Data)

            For i As Integer = 0 To n - 1
                Dim p As Double = If(fd Is Nothing, 1.0, fd(i))

                If p = 0.0 Then
                    Continue For
                End If

                Dim off As Integer = i * d

                For j As Integer = 0 To d - 1
                    eg(off + j) += p * gd(off + j)
                Next
            Next

            ' 扰动标记本身是外部给定的常量，不需要梯度，返回与输入同形的零张量
            If lastFlag Is Nothing Then
                Return New Tensor(0)
            End If

            Return New Tensor(lastFlag.Shape)
        End Function

        ''' <summary>
        ''' 获取本层可训练参数（基因身份嵌入表）
        ''' </summary>
        ''' <returns>参数张量列表</returns>
        Public Overrides Function GetParameters() As List(Of Tensor)
            Return New List(Of Tensor) From {embedding}
        End Function

        ''' <summary>
        ''' 获取本层参数梯度（嵌入表梯度）
        ''' </summary>
        ''' <returns>梯度张量列表</returns>
        Public Overrides Function GetGradients() As List(Of Tensor)
            Return New List(Of Tensor) From {embeddingGrad}
        End Function
    End Class
End Namespace
