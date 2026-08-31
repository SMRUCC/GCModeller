Imports System.IO
Imports System.Text.Json
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel
Imports std = System.Math

''' <summary>
''' 代谢网络的拓扑结构：由 <see cref="MetabolicReaction"/> 集合派生出化学计量矩阵、
''' 结构化掩码以及内/外（边界）代谢物的划分。
''' </summary>
''' <remarks>
''' 术语与 <c>readme.md</c> 保持一致：
''' <list type="bullet">
''' <item><description>S：化学计量矩阵（代谢物 × 反应），用于 <c>S·v ≈ 0</c> 的质量守恒软约束</description></item>
''' <item><description>A_adj：代谢物邻接掩码，用于约束 LTC 的循环权重 W</description></item>
''' <item><description>P / InputMask：参与掩码，用于约束 LTC 的输入权重 U</description></item>
''' </list>
''' 边界代谢物（胞外底物如 glc_e、o2_e，或胞外产物如 co2_e、lac_e）不进入隐藏状态，
''' 而是作为网络的外部驱动输入 u(t)。
''' </remarks>
Public Class MetabolicNetworkGraph

#Region "属性"

    ''' <summary>
    ''' 全部反应（顺序即反应索引）
    ''' </summary>
    Public ReadOnly Property Reactions As MetabolicReaction()

    ''' <summary>反应 id 列表</summary>
    Public ReadOnly Property ReactionIds As String()

    ''' <summary>全部代谢物 id（内部 + 边界，顺序即索引）</summary>
    Public ReadOnly Property MetaboliteIds As String()

    ''' <summary>进入隐藏状态的代谢物 id（状态维度 m）</summary>
    Public ReadOnly Property InternalIds As String()

    ''' <summary>作为外部驱动输入的边界代谢物 id（输入维度 nB）</summary>
    Public ReadOnly Property BoundaryIds As String()

    ''' <summary>
    ''' 化学计量矩阵 S，形状 (代谢物总数 × 反应数)。
    ''' S(i,j) = 产物化学计量数 - 反应物化学计量数
    ''' </summary>
    Public ReadOnly Property Stoichiometry As Tensor

    ''' <summary>
    ''' 代谢物邻接掩码 A_adj，形状 (m × m)。
    ''' A_adj(i,j) = 1 当且仅当代谢物 i 与 j 被同一条反应关联（含对角自连接）。
    ''' </summary>
    Public ReadOnly Property AdjacencyMask As Tensor

    ''' <summary>
    ''' 参与掩码 P，形状 (m × 反应数)。P(i,j) = 1 当代谢物 i 是反应 j 的反应物或产物。
    ''' </summary>
    Public ReadOnly Property ParticipationMask As Tensor

    ''' <summary>
    ''' 输入掩码，形状 (输入维度 × m)，输入维度 = 反应数 + 边界代谢物数。
    ''' 前 r 行对应各反应的酶表达量，后 nB 行对应各边界代谢物浓度。
    ''' </summary>
    Public ReadOnly Property InputMask As Tensor

    ''' <summary>不可逆反应标记（用于热力学方向性约束）</summary>
    Public ReadOnly Property Reversible As Boolean()

    ''' <summary>状态维度（内部代谢物数）</summary>
    Public ReadOnly Property MetaboliteCount As Integer
        Get
            Return InternalIds.Length
        End Get
    End Property

    ''' <summary>反应维度</summary>
    Public ReadOnly Property ReactionCount As Integer
        Get
            Return Reactions.Length
        End Get
    End Property

    ''' <summary>边界代谢物维度</summary>
    Public ReadOnly Property BoundaryCount As Integer
        Get
            Return BoundaryIds.Length
        End Get
    End Property

    ''' <summary>网络输入维度 = 反应数（酶）+ 边界代谢物数</summary>
    Public ReadOnly Property InputSize As Integer
        Get
            Return ReactionCount + BoundaryCount
        End Get
    End Property

    Private ReadOnly _metaboliteIndex As Dictionary(Of String, Integer)
    Private ReadOnly _internalIndex As Dictionary(Of String, Integer)
    Private ReadOnly _reactionIndex As Dictionary(Of String, Integer)
    Private ReadOnly _boundaryIndex As Dictionary(Of String, Integer)
    Private ReadOnly _internalToAll As Integer()

#End Region

#Region "构造函数"

    ''' <summary>
    ''' 由反应集合构建代谢网络拓扑
    ''' </summary>
    ''' <param name="reactions">反应集合</param>
    ''' <param name="explicitBoundary">
    ''' 显式指定的边界代谢物 id；未指定时采用启发式判定
    ''' （只被消耗而从不生成，或只被生成而从不消耗的代谢物，以及带 _e / _ext 后缀的 id）
    ''' </param>
    Public Sub New(reactions As MetabolicReaction(), Optional explicitBoundary As IEnumerable(Of String) = Nothing)
        If reactions Is Nothing OrElse reactions.Length = 0 Then
            Throw New ArgumentException("反应集合不能为空")
        End If

        Me.Reactions = reactions
        Me.ReactionIds = reactions.Select(Function(r) r.id).ToArray()

        ' ---------- 代谢物收集（保持首次出现顺序，保证结果可复现） ----------
        Dim order As New List(Of String)()
        Dim seen As New HashSet(Of String)()

        For Each rxn In reactions
            For Each c In ReactantsOf(rxn).Concat(ProductsOf(rxn))
                If Not seen.Contains(c.ID) Then
                    seen.Add(c.ID)
                    order.Add(c.ID)
                End If
            Next
        Next

        Me.MetaboliteIds = order.ToArray()
        _metaboliteIndex = BuildIndex(Me.MetaboliteIds)
        _reactionIndex = BuildIndex(Me.ReactionIds)

        ' ---------- 边界判定 ----------
        Dim produced As New HashSet(Of String)()
        Dim consumed As New HashSet(Of String)()

        For Each rxn In reactions
            For Each c In ProductsOf(rxn)
                produced.Add(c.ID)
            Next
            For Each c In ReactantsOf(rxn)
                consumed.Add(c.ID)
            Next
        Next

        Dim boundary As New List(Of String)()

        If explicitBoundary IsNot Nothing Then
            For Each id In explicitBoundary
                If _metaboliteIndex.ContainsKey(id) AndAlso Not boundary.Contains(id) Then
                    boundary.Add(id)
                End If
            Next
        Else
            For Each id In Me.MetaboliteIds
                Dim isProduced = produced.Contains(id)
                Dim isConsumed = consumed.Contains(id)

                If (isProduced Xor isConsumed) OrElse
                    id.EndsWith("_e", StringComparison.OrdinalIgnoreCase) OrElse
                    id.EndsWith("_ext", StringComparison.OrdinalIgnoreCase) Then
                    boundary.Add(id)
                End If
            Next
        End If

        Me.BoundaryIds = boundary.ToArray()
        _boundaryIndex = BuildIndex(Me.BoundaryIds)

        Dim boundarySet As New HashSet(Of String)(Me.BoundaryIds)
        Me.InternalIds = Me.MetaboliteIds.Where(Function(id) Not boundarySet.Contains(id)).ToArray()
        _internalIndex = BuildIndex(Me.InternalIds)
        _internalToAll = Me.InternalIds.Select(Function(id) _metaboliteIndex(id)).ToArray()

        ' ---------- 矩阵构建 ----------
        Me.Stoichiometry = BuildStoichiometry()
        Me.AdjacencyMask = BuildAdjacency()
        Me.ParticipationMask = BuildParticipation()
        Me.InputMask = BuildInputMask()
        Me.Reversible = reactions.Select(Function(r) r.is_reversible).ToArray()
    End Sub

    Private Shared Function BuildIndex(ids As String()) As Dictionary(Of String, Integer)
        Dim map As New Dictionary(Of String, Integer)()

        For i = 0 To ids.Length - 1
            map(ids(i)) = i
        Next

        Return map
    End Function

    Private Shared Iterator Function ReactantsOf(rxn As MetabolicReaction) As IEnumerable(Of CompoundSpecieReference)
        If rxn.left Is Nothing Then Return

        For Each c In rxn.left
            Yield c
        Next
    End Function

    Private Shared Iterator Function ProductsOf(rxn As MetabolicReaction) As IEnumerable(Of CompoundSpecieReference)
        If rxn.right Is Nothing Then Return

        For Each c In rxn.right
            Yield c
        Next
    End Function

    ''' <summary>
    ''' S(i,j) = 产物化学计量数 − 反应物化学计量数
    ''' </summary>
    Private Function BuildStoichiometry() As Tensor
        Dim mAll = MetaboliteIds.Length
        Dim n = Reactions.Length
        Dim S = New Tensor(mAll, n)

        For j = 0 To n - 1
            Dim rxn = Reactions(j)

            For Each c In ProductsOf(rxn)
                S(_metaboliteIndex(c.ID), j) += If(c.Stoichiometry = 0, 1.0, c.Stoichiometry)
            Next
            For Each c In ReactantsOf(rxn)
                S(_metaboliteIndex(c.ID), j) -= If(c.Stoichiometry = 0, 1.0, c.Stoichiometry)
            Next
        Next

        Return S
    End Function

    ''' <summary>
    ''' A_adj(i,j)：内部代谢物 i 与 j 是否共享同一条反应（含对角）
    ''' </summary>
    Private Function BuildAdjacency() As Tensor
        Dim m = InternalIds.Length
        Dim A = New Tensor(m, m)
        ' 邻居集合：代谢物 -&gt; 与之共现的代谢物
        Dim coOccur = New List(Of HashSet(Of Integer))(m)

        For i = 0 To m - 1
            coOccur.Add(New HashSet(Of Integer)())
            coOccur(i).Add(i)   ' 自连接：代谢物自身的衰减/合成
        Next

        For j = 0 To Reactions.Length - 1
            Dim involved As New List(Of Integer)()

            For Each c In ReactantsOf(Reactions(j)).Concat(ProductsOf(Reactions(j)))
                If _internalIndex.ContainsKey(c.ID) Then
                    involved.Add(_internalIndex(c.ID))
                End If
            Next

            For a = 0 To involved.Count - 1
                For b = 0 To involved.Count - 1
                    coOccur(involved(a)).Add(involved(b))
                Next
            Next
        Next

        For i = 0 To m - 1
            For Each j In coOccur(i)
                A(i, j) = 1.0
            Next
        Next

        Return A
    End Function

    ''' <summary>
    ''' P(i,j)：内部代谢物 i 是否参与反应 j
    ''' </summary>
    Private Function BuildParticipation() As Tensor
        Dim m = InternalIds.Length
        Dim P = New Tensor(m, Reactions.Length)

        For j = 0 To Reactions.Length - 1
            For Each c In ReactantsOf(Reactions(j)).Concat(ProductsOf(Reactions(j)))
                If _internalIndex.ContainsKey(c.ID) Then
                    P(_internalIndex(c.ID), j) = 1.0
                End If
            Next
        Next

        Return P
    End Function

    ''' <summary>
    ''' 输入掩码：酶 j 只允许驱动它参与的反应所涉及的代谢物；
    ''' 边界代谢物 k 只允许驱动与它共享反应的代谢物。
    ''' </summary>
    Private Function BuildInputMask() As Tensor
        Dim m = InternalIds.Length
        Dim n = Reactions.Length
        Dim nB = BoundaryIds.Length
        Dim mask = New Tensor(n + nB, m)

        ' 酶输入通道
        For j = 0 To n - 1
            For Each c In ReactantsOf(Reactions(j)).Concat(ProductsOf(Reactions(j)))
                If _internalIndex.ContainsKey(c.ID) Then
                    mask(j, _internalIndex(c.ID)) = 1.0
                End If
            Next
        Next

        ' 边界底物输入通道
        For k = 0 To nB - 1
            Dim bid = BoundaryIds(k)

            For j = 0 To n - 1
                Dim touches = ReactantsOf(Reactions(j)).Concat(ProductsOf(Reactions(j))).
                              Any(Function(c) c.ID = bid)

                If touches Then
                    For Each c In ReactantsOf(Reactions(j)).Concat(ProductsOf(Reactions(j)))
                        If _internalIndex.ContainsKey(c.ID) Then
                            mask(n + k, _internalIndex(c.ID)) = 1.0
                        End If
                    Next
                End If
            Next
        Next

        Return mask
    End Function

#End Region

#Region "索引查询"

    Public Function IndexOfMetabolite(id As String) As Integer
        Return If(_metaboliteIndex.ContainsKey(id), _metaboliteIndex(id), -1)
    End Function

    Public Function IndexOfInternal(id As String) As Integer
        Return If(_internalIndex.ContainsKey(id), _internalIndex(id), -1)
    End Function

    Public Function IndexOfReaction(id As String) As Integer
        Return If(_reactionIndex IsNot Nothing AndAlso _reactionIndex.ContainsKey(id), _reactionIndex(id), Array.IndexOf(ReactionIds, id))
    End Function

    Public Function IndexOfBoundary(id As String) As Integer
        Return If(_boundaryIndex.ContainsKey(id), _boundaryIndex(id), -1)
    End Function

    ''' <summary>内部代谢物索引 -&gt; 全代谢物索引</summary>
    Public Function ToMetaboliteIndex(internalIdx As Integer) As Integer
        Return _internalToAll(internalIdx)
    End Function

    ''' <summary>该反应是否为可逆反应</summary>
    Public Function IsReversible(reactionIndex As Integer) As Boolean
        Return Reversible(reactionIndex)
    End Function

#End Region

#Region "质量守恒"

    ''' <summary>
    ''' 稳态残差 S·v（长度 = 代谢物总数）
    ''' </summary>
    ''' <param name="v">反应通量向量（长度 = 反应数）</param>
    Public Function SteadyStateResidual(v As Tensor) As Tensor
        Dim mAll = MetaboliteIds.Length
        Dim n = Reactions.Length

        If v.Length <> n Then
            Throw New ArgumentException($"通量维度不匹配：期望 {n}，实际 {v.Length}")
        End If

        Dim residual = New Tensor(mAll)

        For i = 0 To mAll - 1
            Dim acc As Double = 0.0

            For j = 0 To n - 1
                acc += Stoichiometry(i, j) * v(j)
            Next

            residual(i) = acc
        Next

        Return residual
    End Function

    ''' <summary>
    ''' 稳态违反度 ‖S·v‖₂，越接近 0 表示越满足质量守恒
    ''' </summary>
    Public Function SteadyStateViolation(v As Tensor) As Double
        Dim r = SteadyStateResidual(v)
        Dim sq As Double = 0.0

        For i = 0 To r.Length - 1
            sq += r(i) * r(i)
        Next

        Return std.Sqrt(sq)
    End Function

    ''' <summary>
    ''' Sᵀ·r：把对稳态残差的梯度回传到通量空间
    ''' </summary>
    Public Function ResidualGradientToFlux(residual As Tensor) As Tensor
        Dim mAll = MetaboliteIds.Length
        Dim n = Reactions.Length
        Dim grad = New Tensor(n)

        For j = 0 To n - 1
            Dim acc As Double = 0.0

            For i = 0 To mAll - 1
                acc += Stoichiometry(i, j) * residual(i)
            Next

            grad(j) = acc
        Next

        Return grad
    End Function

#End Region

#Region "JSON 存取"

    ''' <summary>把反应集合序列化为 JSON 文件</summary>
    Public Sub SaveJson(path As String)
        Dim json = JsonSerializer.Serialize(Reactions, New JsonSerializerOptions With {
            .WriteIndented = True
        })

        File.WriteAllText(path, json)
    End Sub

    ''' <summary>从 JSON 文件载入反应集合并构建拓扑</summary>
    ''' <param name="path">network.json 路径</param>
    ''' <param name="explicitBoundary">显式指定的边界代谢物 id（可选）</param>
    Public Shared Function LoadJson(path As String, Optional explicitBoundary As IEnumerable(Of String) = Nothing) As MetabolicNetworkGraph
        Dim json = File.ReadAllText(path)
        Dim reactions = JsonSerializer.Deserialize(Of MetabolicReaction())(json)

        Return New MetabolicNetworkGraph(reactions, explicitBoundary)
    End Function

#End Region

    Public Overrides Function ToString() As String
        Return $"{MetaboliteCount} metabolites({BoundaryCount} boundary) x {ReactionCount} reactions"
    End Function

End Class
