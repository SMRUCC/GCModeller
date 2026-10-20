Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph
Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 一条基因表达调控关系：``regulator -&gt; target``，边的权重为线性模型的回归系数。
    ''' </summary>
    ''' <remarks>
    ''' 这个类同时实现了 <see cref="IInteraction"/> 与 <see cref="INetworkEdge"/> 接口，
    ''' 因此可以直接被生成为网络图数据。
    ''' </remarks>
    Public Class RegulationEdge : Implements IInteraction, INetworkEdge

        ''' <summary>
        ''' 调控因子基因编号(上游基因)
        ''' </summary>
        ''' <returns></returns>
        Public Property regulator As String Implements IInteraction.source

        ''' <summary>
        ''' 被调控的目标基因编号(下游基因)
        ''' </summary>
        ''' <returns></returns>
        Public Property target As String Implements IInteraction.target

        ''' <summary>
        ''' 线性模型之中调控因子对目标基因的回归系数，
        ''' 正数表示激活型的调控关系，负数表示抑制型的调控关系。
        ''' </summary>
        ''' <returns></returns>
        Public Property weight As Double Implements INetworkEdge.value

        ''' <summary>
        ''' 目标基因的模型解释度(R^2)
        ''' </summary>
        ''' <returns></returns>
        Public Property r2 As Double

        ''' <summary>
        ''' 回归系数的显著性 p 值，LASSO 模型之下恒为 1。
        ''' </summary>
        ''' <returns></returns>
        Public Property pvalue As Double = 1

        ''' <summary>
        ''' 调控关系的类型描述，一般为求解方法的名称。
        ''' </summary>
        ''' <returns></returns>
        Public Property interaction As String Implements INetworkEdge.Interaction

        ''' <summary>
        ''' 产生这条边的模型之中的常数项
        ''' </summary>
        ''' <returns></returns>
        Public Property intercept As Double

        ''' <summary>
        ''' 回归系数的绝对值
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property absWeight As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return std.Abs(weight)
            End Get
        End Property

        ''' <summary>
        ''' 边的唯一编号，形如 ``regulator-&gt;target``
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UID As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return $"{regulator}->{target}"
            End Get
        End Property

        ''' <summary>
        ''' 调控关系的文本表示
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim type As String = If(weight >= 0, "activate", "repress")

            Return $"[{type}] {regulator} -> {target}, beta={weight.ToString("G4")}, pvalue={pvalue.ToString("G3")}"
        End Function
    End Class
End Namespace
