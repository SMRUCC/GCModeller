''' <summary>
''' 一条被“残差二阶段”流程检验过的候选调控边。
''' </summary>
Public Class DiscoveryEdge

    ''' <summary>靶基因（被调控的一端）</summary>
    Public Property target As String

    ''' <summary>候选调控者</summary>
    Public Property regulator As String

    ''' <summary>靶基因模型残差与候选表达之间的 pearson 相关系数</summary>
    Public Property correlation As Double

    ''' <summary>把这条边加入特征之后，验证集 R² 的增益</summary>
    Public Property deltaR2 As Double

    ''' <summary>加入这条边之前的验证集 R²</summary>
    Public Property r2Before As Double

    ''' <summary>加入这条边之后的验证集 R²</summary>
    Public Property r2After As Double

    ''' <summary>该特征在最终模型之中被用作分裂特征的节点数量</summary>
    Public Property splits As Integer

    ''' <summary>该特征的偏依赖灵敏度</summary>
    Public Property sensitivity As Double

    ''' <summary>
    ''' 是否被接受为新调控边：<c>accepted</c> / <c>rejected</c>
    ''' </summary>
    Public Property status As String

    ''' <summary>被拒绝的原因（接受时为空）</summary>
    Public Property reason As String

    Public ReadOnly Property isAccepted As Boolean
        Get
            Return String.Equals(status, "accepted", StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{regulator} -> {target}: r={correlation.ToString("G3")}, dR2={deltaR2.ToString("G3")}, {status}"
    End Function
End Class