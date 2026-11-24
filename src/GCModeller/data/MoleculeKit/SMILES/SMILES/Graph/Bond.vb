Imports GraphTheory = Microsoft.VisualBasic.Data.GraphTheory.Network

Namespace Graph

    ''' <summary>
    ''' 分子图中的一条化学键：由两个原子下标与键级构成。
    ''' </summary>
    ''' <remarks>
    ''' 键级使用 <see cref="Double"/> 而不是整数，是为了在同一字段内同时表达
    ''' "Kekulé 化之后的 1/2/3" 与 "尚未 Kekulé 化的 1.5 芳香键"。
    ''' 解析器完成 Kekulé 化之后，<see cref="order"/> 会归一到 1/2/3，
    ''' 而 <see cref="IsAromatic"/> 继续保留芳香性信息供绘图使用。
    ''' 实现 <see cref="Microsoft.VisualBasic.Data.GraphTheory.Network.IndexEdge"/> 使分子图可以直接接入 sciBASIC 的图论算法。
    ''' </remarks>
    Public Class Bond
        Implements GraphTheory.IndexEdge

        ''' <summary>
        ''' 键一端的原子下标。
        ''' </summary>
        Public Property a As Integer Implements GraphTheory.IndexEdge.U

        ''' <summary>
        ''' 键另一端的原子下标。
        ''' </summary>
        Public Property b As Integer Implements GraphTheory.IndexEdge.V

        ''' <summary>
        ''' 键级：<c>1</c> 单键、<c>2</c> 双键、<c>3</c> 叁键；芳香键在 Kekulé 化之前为 <c>1.5</c>。
        ''' </summary>
        Public Property order As Double = 1

        ''' <summary>
        ''' 该键是否属于芳香体系。
        ''' </summary>
        Public Property IsAromatic As Boolean

        ''' <summary>
        ''' 立体标记（实楔形 / 虚楔形 / 双键顺反）。
        ''' </summary>
        Public Property Stereo As BondStereo

        ''' <summary>
        ''' 该键是否位于某个环上（由 <see cref="MoleculeGraph.Rings"/> 计算后回填）。
        ''' </summary>
        Public Property IsRing As Boolean

        ''' <summary>
        ''' 是否被立体记号标记为未知的双键构型（<c>/?</c>）。
        ''' </summary>
        Public ReadOnly Property IsStereoUnknown As Boolean
            Get
                Return Stereo = BondStereo.Either
            End Get
        End Property

        ''' <summary>
        ''' 创建一个单键。
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' 以原子下标与键级创建一条键。
        ''' </summary>
        ''' <param name="a">一端原子下标。</param>
        ''' <param name="b">另一端原子下标。</param>
        ''' <param name="order">键级。</param>
        Public Sub New(a As Integer, b As Integer, Optional order As Double = 1)
            Me.a = a
            Me.b = b
            Me.order = order
        End Sub

        ''' <summary>
        ''' 键级取整（用于绘图时选择单/双/叁键画法）。
        ''' </summary>
        Public ReadOnly Property OrderInt As Integer
            Get
                Return CInt(Math.Round(order))
            End Get
        End Property

        ''' <summary>
        ''' 从 <c>(a, b, order)</c> 三元组隐式转换为键对象。
        ''' </summary>
        ''' <param name="tri">三元组：起点、终点、键级。</param>
        Public Shared Widening Operator CType(tri As (Integer, Integer, Integer)) As Bond
            Return New Bond(tri.Item1, tri.Item2, tri.Item3)
        End Operator

        ''' <summary>
        ''' 生成用于调试的紧凑描述，例如 <c>1-4:2</c>。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"{a}-{b}:{order}"
        End Function

    End Class

End Namespace
