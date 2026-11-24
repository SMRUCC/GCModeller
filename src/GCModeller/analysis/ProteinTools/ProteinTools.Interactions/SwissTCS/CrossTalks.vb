Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.ComponentModels
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph

Namespace SwissTCS

    ''' <summary>
    ''' 双组分系统的蛋白质之间的互作的可能性的高低。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这个类型同时承担两个角色：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>
    ''' SwissTCS 网站下载数据的行模型(<c>kinase</c> / <c>regulator</c> / <c>probability</c>)，
    ''' 保持与历史 CSV 文件的列名完全兼容；
    ''' </description></item>
    ''' <item><description>
    ''' 新流水线的互作预测结果容器：额外携带 <see cref="Score"/> 与
    ''' <see cref="Source"/> 等字段，用于区分「来自实验数据」、「来自贝叶斯网络预测」
    ''' 与「来自直系同源迁移」三种来源。
    ''' </description></item>
    ''' </list>
    ''' <para>
    ''' 由于 <c>LoadCsv</c> 只绑定带有 <see cref="Column"/> 特性的属性，
    ''' 新增字段不会影响历史文件的读取。
    ''' </para>
    ''' </remarks>
    Public Class CrossTalks : Implements IInteraction, INetworkEdge

        ''' <summary>
        ''' 激酶(或者互作对之中的第一个成员)
        ''' </summary>
        <Column("kinase")> Public Property Kinase As String Implements IInteraction.target

        ''' <summary>
        ''' 响应调节蛋白(或者互作对之中的第二个成员)
        ''' </summary>
        <Column("regulator")> Public Property Regulator As String Implements IInteraction.source

        ''' <summary>
        ''' 互作可能性
        ''' </summary>
        <Column("probability")> Public Property Probability As Double Implements INetworkEdge.value

        ''' <summary>
        ''' 边上的互作类型标注
        ''' </summary>
        Public Property InteractionType As String Implements INetworkEdge.Interaction

        Private _score As Double = Double.NaN

        ''' <summary>
        ''' 论文静态打分 <c>X(S_k, S_r)</c>；未做打分的时候为 <see cref="Double.NaN"/>
        ''' </summary>
        Public Property Score As Double
            Get
                Return _score
            End Get
            Set(value As Double)
                _score = value
            End Set
        End Property

        ''' <summary>
        ''' 结果来源：<c>cognate</c>、<c>sampled</c>、<c>ipfam</c> 或者 <c>interolog</c>
        ''' </summary>
        Public Property Source As String

        ''' <summary>
        ''' 贡献最大的残基对描述，用于可解释性展示
        ''' </summary>
        Public Property TopContact As String

        ''' <summary>
        ''' 是否为直系同源迁移得到的互作
        ''' </summary>
        Public ReadOnly Property IsInterolog As Boolean
            Get
                Return String.Equals(Source, "interolog", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        ''' <summary>
        ''' 按照当前打分与默认阈值(1.0)判断是否预测为互作
        ''' </summary>
        Public ReadOnly Property IsPredicted As Boolean
            Get
                Return Not Double.IsNaN(Score) AndAlso Score >= 1.0
            End Get
        End Property

        ''' <summary>
        ''' 去掉基因编号后面可能附带的括号注释
        ''' </summary>
        Public Shared Function Trim(sId As String) As String
            If String.IsNullOrEmpty(sId) Then
                Return sId
            End If

            Dim p As Integer = InStr(sId, "(")

            If p > 0 Then
                Return Mid(sId, 1, p - 1)
            Else
                Return sId
            End If
        End Function

        ''' <summary>
        ''' 去掉括号注释之后的激酶编号
        ''' </summary>
        Public Function TrimedKinaseId() As String
            Return Trim(Kinase)
        End Function

        ''' <summary>
        ''' 去掉括号注释之后的响应调节蛋白编号
        ''' </summary>
        Public Function TrimedRegulatorId() As String
            Return Trim(Regulator)
        End Function

        ''' <summary>
        ''' 由打分构造一条预测结果
        ''' </summary>
        ''' <param name="kinase">激酶编号</param>
        ''' <param name="regulator">响应调节蛋白编号</param>
        ''' <param name="score">论文静态打分</param>
        ''' <param name="source">结果来源标注</param>
        Public Shared Function FromScore(kinase As String, regulator As String, score As Double, Optional source As String = "sampled") As CrossTalks
            Return New CrossTalks With {
                .Kinase = kinase,
                .Regulator = regulator,
                .Score = score,
                .Probability = Logistic(score),
                .Source = source
            }
        End Function

        ''' <summary>
        ''' 把论文的打分转换为 <c>[0, 1]</c> 区间之内的概率，便于与实验概率直接比较
        ''' </summary>
        ''' <param name="score">静态打分 <c>X</c></param>
        ''' <remarks>
        ''' 使用中点位于阈值 1.0、斜率 1.0 的 logistic 函数；这样 <c>X = 1</c> 恰好映射到
        ''' <c>0.5</c>，与论文的判定规则保持一致的单调性。
        ''' </remarks>
        Public Shared Function Logistic(score As Double) As Double
            Return 1.0 / (1.0 + Math.Exp(-(score - 1.0)))
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1};  {2}", Kinase, Regulator, Probability)
        End Function
    End Class

    ''' <summary>
    ''' 双组分系统的成员类型
    ''' </summary>
    Public Enum TCSComponentTypes
        ''' <summary>
        ''' 组氨酸激酶
        ''' </summary>
        kinase
        ''' <summary>
        ''' 响应调节蛋白
        ''' </summary>
        receiver
    End Enum
End Namespace
