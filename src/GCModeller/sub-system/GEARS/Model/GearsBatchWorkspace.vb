Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

''' <summary>
''' GEARS 批量推理的工作区：按容量分配中间缓冲并跨时间步复用。
''' </summary>
''' <remarks>
''' 为什么需要独立的工作区对象：
''' <list type="bullet">
'''   <item>
'''     类器官仿真里细胞数会随分裂 / 死亡变化，而 <c>[B·n, 2+2d]</c> 的节点特征矩阵
'''     在 B 为数百时就是数十 MB 级；若每步都新建张量，既产生大量垃圾，
'''     也会让 GPU 侧的显存常驻表反复失效。工作区因此按<b>容量</b>（调用方给出的
'''     上界，通常取分块大小）一次性分配，之后整场仿真复用。
'''   </item>
'''   <item>
'''     多层图卷积需要"上一层输出即下一层输入"，因此保留两个隐藏缓冲做乒乓复用
'''     （层数任意多都不会出现读写同一块的情况）。
'''   </item>
'''   <item>
'''     缓冲被 <c>PinDevice64</c> 钉住之后设备成为主副本；重建缓冲时必须先解除钉住，
'''     否则旧显存会一直挂在常驻表里（键是主机数组引用，GC 回收不了显存）。
'''   </item>
''' </list>
''' </remarks>
Public Class GearsBatchWorkspace : Implements IDisposable

    ''' <summary>基因数（图的节点数）</summary>
    Public ReadOnly Property NumGenes As Integer

    ''' <summary>基因身份嵌入维度 d</summary>
    Public ReadOnly Property EmbeddingDim As Integer

    ''' <summary>节点特征维度（2 + 2d）</summary>
    Public ReadOnly Property FeatureDim As Integer

    ''' <summary>容量：本工作区一次能容纳的最大细胞数</summary>
    Public ReadOnly Property Capacity As Integer

    Private ReadOnly _scaled As Tensor
    Private ReadOnly _features As Tensor
    Private ReadOnly _decoded As Tensor
    Private ReadOnly _hidden As Tensor() = New Tensor(1) {}
    Private _cursor As Integer = 0
    Private _disposed As Boolean = False

    ''' <param name="capacity">容量（一次推理能容纳的最大细胞数）</param>
    ''' <param name="numGenes">基因数（图的节点数）</param>
    ''' <param name="embeddingDim">基因身份嵌入维度</param>
    Public Sub New(capacity As Integer, numGenes As Integer, embeddingDim As Integer)
        If capacity <= 0 OrElse numGenes <= 0 OrElse embeddingDim <= 0 Then
            Throw New ArgumentException($"容量 / 节点数 / 嵌入维度必须为正：{capacity} / {numGenes} / {embeddingDim}")
        End If

        Me.Capacity = capacity
        Me.NumGenes = numGenes
        Me.EmbeddingDim = embeddingDim
        Me.FeatureDim = 2 + 2 * embeddingDim

        _scaled = New Tensor(capacity, numGenes)
        _features = New Tensor(capacity * numGenes, FeatureDim)
        _decoded = New Tensor(capacity * numGenes, 1)
    End Sub

    ''' <summary>用于 Deep Sets 均值池化的"缩放后扰动标记" <c>[容量, n]</c>（就地写入）</summary>
    Public ReadOnly Property ScaledFlag As Tensor
        Get
            Return _scaled
        End Get
    End Property

    ''' <summary>逐细胞的全局扰动向量 <c>[容量, d]</c></summary>
    Public ReadOnly Property ZPert As Tensor
        Get
            Return _zPert
        End Get
    End Property

    Private _zPert As Tensor

    ''' <summary>节点特征矩阵 <c>[容量·n, 2+2d]</c></summary>
    Public ReadOnly Property Features As Tensor
        Get
            Return _features
        End Get
    End Property

    ''' <summary>解码器输出 <c>[容量·n, 1]</c>（底层数组即 <c>[容量, n]</c> 的行优先排列）</summary>
    Public ReadOnly Property Decoded As Tensor
        Get
            Return _decoded
        End Get
    End Property

    ''' <summary>登记本轮池化结果（由 <c>ScaledFlag · Embeddings</c> 得到）。</summary>
    Public Sub RefreshZPert(zPert As Tensor)
        _zPert = zPert
    End Sub

    ''' <summary>
    ''' 取下一个隐藏层缓冲（乒乓复用）：返回的缓冲<b>必须被完整写入</b>，
    ''' 因此调用方（图卷积层）无需先清零。
    ''' </summary>
    Public Function NextHidden(outFeatures As Integer) As Tensor
        Dim index As Integer = _cursor Mod _hidden.Length
        Dim rows As Integer = Capacity * NumGenes
        Dim buffer As Tensor = _hidden(index)

        _cursor += 1

        If buffer Is Nothing OrElse buffer.Shape(0) <> rows OrElse buffer.Shape(1) <> outFeatures Then
            Call Release(buffer)
            buffer = New Tensor(rows, outFeatures)
            _hidden(index) = buffer
        End If

        Return buffer
    End Function

    ''' <summary>
    ''' 释放缓冲：先解除设备常驻（否则显存会一直挂在常驻表里），再置空引用。
    ''' </summary>
    Private Shared Sub Release(ByRef buffer As Tensor)
        If buffer Is Nothing Then
            Return
        End If

        Try
            Call Tensor.computeKernel.UnpinDevice(buffer)
        Catch
            ' 解除常驻失败不应影响仿真主流程（后端可能已经切换或释放）
        End Try

        buffer = Nothing
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _disposed Then
            Return
        End If

        _disposed = True

        Call Release(_zPert)
        Call Release(_hidden(0))
        Call Release(_hidden(1))
    End Sub
End Class
