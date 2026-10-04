Imports SMRUCC.genomics.Analysis.BNLearn.Core

Namespace RegulationNetwork

    ''' <summary>
    ''' <see cref="RegulatoryEdge.Evidence"/> 使用的受控词表
    ''' </summary>
    ''' <remarks>
    ''' 由于 <see cref="RegulatoryEdge"/> 不携带边类型字段，本模块用 Evidence 字符串
    ''' 承载证据来源与拓扑性质（有向 / 无向候选 / 跨模块 / 仅 PPI），
    ''' 多个标签用 <c>+</c> 拼接，便于下游按证据来源做加权与消融实验。
    ''' </remarks>
    Public Module EvidenceTags

        ''' <summary>bicor 稳健共表达证据</summary>
        Public Const COEXPRESSION_BICOR As String = "coexpression:bicor"

        ''' <summary>Pearson 共表达证据</summary>
        Public Const COEXPRESSION_PEARSON As String = "coexpression:pearson"

        ''' <summary>偏相关检验通过（相关性未被条件集解释掉）</summary>
        Public Const PARTIAL_RETAINED As String = "partial:retained"

        ''' <summary>偏相关显著衰减（疑似部分间接），已打折保留</summary>
        Public Const PARTIAL_ATTENUATED As String = "partial:attenuated"

        ''' <summary>STRING 蛋白互作支持（叠加在共表达边之上）</summary>
        Public Const STRING_PPI As String = "string:ppi"

        ''' <summary>仅由 STRING 蛋白互作产生的边，符号未知</summary>
        Public Const STRING_PPI_UNSIGNED As String = "string:ppi:unsigned"

        ''' <summary>两端同为转录因子，无法由 TF 注释确定方向的候选边</summary>
        Public Const UNDIRECTED_TF_TF As String = "undirected:tf-tf"

        ''' <summary>两端同为非转录因子，无法由 TF 注释确定方向的候选边</summary>
        Public Const UNDIRECTED_NON_TF As String = "undirected:non-tf"

        ''' <summary>跨 WGCNA 模块的调控边</summary>
        Public Const CROSS_MODULE As String = "cross-module"

        ''' <summary>
        ''' 拼接多个证据标签
        ''' </summary>
        ''' <param name="tags">按重要性排列的标签</param>
        ''' <returns>用 <c>+</c> 连接的证据字符串；空输入返回空字符串</returns>
        Public Function Join(ParamArray tags As String()) As String
            If tags Is Nothing Then
                Return ""
            End If

            Return String.Join("+", tags.Where(Function(t) Not String.IsNullOrEmpty(t)))
        End Function

        ''' <summary>
        ''' 在已有证据串上追加标签
        ''' </summary>
        ''' <param name="evidence">已有证据串</param>
        ''' <param name="tag">待追加的标签</param>
        ''' <returns>追加后的证据串</returns>
        Public Function Append(evidence As String, tag As String) As String
            If String.IsNullOrEmpty(tag) Then
                Return If(evidence, "")
            End If
            If String.IsNullOrEmpty(evidence) Then
                Return tag
            End If

            Return evidence & "+" & tag
        End Function

        ''' <summary>
        ''' 判断证据串中是否包含指定标签
        ''' </summary>
        ''' <param name="evidence">证据串</param>
        ''' <param name="tag">标签</param>
        ''' <returns></returns>
        Public Function Has(evidence As String, tag As String) As Boolean
            If String.IsNullOrEmpty(evidence) OrElse String.IsNullOrEmpty(tag) Then
                Return False
            End If

            Return evidence.Split("+"c).Any(Function(t) String.Equals(t, tag, StringComparison.OrdinalIgnoreCase))
        End Function

        ''' <summary>
        ''' 该边是否为无法定向的候选边（TF-TF 或 非 TF-非 TF）
        ''' </summary>
        ''' <param name="evidence">证据串</param>
        ''' <returns></returns>
        Public Function IsUndirected(evidence As String) As Boolean
            Return Has(evidence, UNDIRECTED_TF_TF) OrElse Has(evidence, UNDIRECTED_NON_TF)
        End Function
    End Module
End Namespace
