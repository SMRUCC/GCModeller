Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph

Namespace ARACNE

    ''' <summary>
    ''' ARACNE 推断出来的调控边。
    ''' 
    ''' ARACNE 的核心输出是无向的互信息网络，本类型在此基础上按照 TF 先验
    ''' (readme 第四章第 3 节)给边赋予了方向：
    ''' 
    ''' + <c>"directed"</c>：``TF -> 靶基因``；
    ''' + <c>"mutual"</c>：两个端点都是转录因子，构成双向的 TF-TF 调控关系；
    ''' + <c>"undirected"</c>：两个端点都不是转录因子，只是互信息意义上的候选边。
    ''' </summary>
    Public Class AracneEdge : Implements IInteraction, INetworkEdge

        ''' <summary>
        ''' 调控因子(边的起点)
        ''' </summary>
        ''' <returns></returns>
        Public Property regulator As String Implements IInteraction.source

        ''' <summary>
        ''' 靶基因(边的终点)
        ''' </summary>
        ''' <returns></returns>
        Public Property target As String Implements IInteraction.target

        ''' <summary>
        ''' 两个基因之间的互信息(比特)，也就是边的权重
        ''' </summary>
        ''' <returns></returns>
        Public Property mi As Double Implements INetworkEdge.value

        ''' <summary>
        ''' 两个基因表达谱的 pearson 相关系数
        ''' 
        ''' 互信息本身没有符号，相关系数的符号可以用来区分
        ''' 激活(正相关)与抑制(负相关)的倾向。
        ''' </summary>
        ''' <returns></returns>
        Public Property correlation As Double

        ''' <summary>
        ''' 边的方向类型：``directed`` / ``mutual`` / ``undirected``
        ''' </summary>
        ''' <returns></returns>
        Public Property interaction As String Implements INetworkEdge.Interaction

        ''' <summary>
        ''' 显著性 p 值。DPI 过滤本身不做逐边的假设检验，因此默认写为 1。
        ''' </summary>
        ''' <returns></returns>
        Public Property pvalue As Double = 1.0R

        ''' <summary>
        ''' 调控因子是否为转录因子
        ''' </summary>
        ''' <returns></returns>
        Public Property isTfSource As Boolean

        ''' <summary>
        ''' 靶基因是否为转录因子
        ''' </summary>
        ''' <returns></returns>
        Public Property isTfTarget As Boolean

        ''' <summary>
        ''' 这条边是否受到 curated 先验网络的支持。
        ''' 
        ''' 受先验支持的边享有更高的门槛(阈值放宽、top-K 与 DPI 保护)，
        ''' 因此这里的标记可以直接用于下游分析之中区分
        ''' “数据 + 先验共同支持”与“仅由表达数据支持”的调控关系。
        ''' </summary>
        ''' <returns></returns>
        Public Property prior As Boolean

        ''' <summary>
        ''' 先验给出的调控类型：``activator``(激活) / ``inhibitor``(抑制) /
        ''' 空字符串(没有先验或者先验未给出因果符号)。
        ''' 
        ''' 互信息与相关系数都没有方向性，无法区分激活与抑制，
        ''' 因此这个字段完全来自 curated 先验。
        ''' </summary>
        ''' <returns></returns>
        Public Property regulation As String

        ''' <summary>
        ''' 以字符串表达的唯一标识符，形如 ``TP53 -> MDM2``
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UID As String
            Get
                Return $"{regulator} -> {target}"
            End Get
        End Property

        Public ReadOnly Property absMi As Double
            Get
                Return System.Math.Abs(mi)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim tags As New List(Of String)

            If prior Then
                Call tags.Add("prior")
            End If
            If Not String.IsNullOrEmpty(regulation) Then
                Call tags.Add(regulation)
            End If

            Return $"{interaction}: {regulator} -> {target}, MI={mi.ToString("F4")}, r={correlation.ToString("F3")}" &
                If(tags.Count = 0, "", " [" & String.Join(", ", tags) & "]")
        End Function
    End Class
End Namespace
