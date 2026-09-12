#Region "Metabopolis: network preprocessing"

' ============================================================================
' 网络预处理（论文第六节「支撑策略」）
' ----------------------------------------------------------------------------
' 三件事：
'   1. 节点复制 —— 「不重要」的代谢物（水、质子等）在每个参与的类别中复制一份，
'      从而降低图的密度；而「连接型」代谢物（ATP 这类跨类别枢纽）不复制，
'      它们被放到街区边界上作为对外接口。
'   2. 九色角色编码 —— 区分一个共享代谢物在两个类别中分别是底物/产物/两者兼有。
'   3. 长有向边分解 —— 把长有向路径切成「有向段 + 无向段 + 有向段」，
'      无向段可以沿线束捆绑而不丢失方向信息。
' ============================================================================

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Metabopolis.Model
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel

Namespace Preprocess

    ''' <summary>
    ''' 代谢物在某个类别中的角色（按位组合，可直接用 <c>Or</c> 累加）。
    ''' </summary>
    <Flags>
    Public Enum MetaboliteRole As Integer

        ''' <summary>不参与该类别中的任何反应。</summary>
        None = 0
        ''' <summary>作为底物被消耗。</summary>
        Reactant = 1
        ''' <summary>作为产物被生成。</summary>
        Product = 2
        ''' <summary>既作为底物又作为产物。</summary>
        Both = 3

    End Enum

    ''' <summary>
    ''' 一个类别内部的代谢物副本（节点复制的产物）。
    ''' </summary>
    Public Class MetaboliteCopy

        ''' <summary>
        ''' 布局图中的唯一节点编号，形如 <c>M:ATP@GLYCOLYSIS</c>。
        ''' </summary>
        Public Property NodeId As String

        ''' <summary>原始代谢物编号。</summary>
        Public Property MetaboliteId As String

        ''' <summary>所属类别编号。</summary>
        Public Property CategoryId As String

        ''' <summary>该代谢物在本类别中的角色。</summary>
        Public Property Role As MetaboliteRole

        ''' <summary>
        ''' 是否为枢纽：枢纽代谢物绘制在街区边界上，并参与块间路由；
        ''' 非枢纽副本是街区内部节点，不与其它街区相连。
        ''' </summary>
        Public Property IsHub As Boolean

        ''' <summary>是否为「不重要」的代谢物（被完全复制以降低图密度）。</summary>
        Public Property IsTrivial As Boolean

        Public Overrides Function ToString() As String
            Return NodeId
        End Function

    End Class

    ''' <summary>
    ''' 一个类别内部反应节点。
    ''' </summary>
    Public Class ReactionNode

        ''' <summary>布局图中的唯一节点编号，形如 <c>R:RXN-1234</c>。</summary>
        Public Property NodeId As String

        ''' <summary>原始反应编号。</summary>
        Public Property ReactionId As String

        ''' <summary>所属类别编号。</summary>
        Public Property CategoryId As String

        ''' <summary>反应是否可逆。</summary>
        Public Property IsReversible As Boolean

        Public Overrides Function ToString() As String
            Return NodeId
        End Function

    End Class

    ''' <summary>
    ''' 预处理结果：类别内子图 + 类别间复制节点 + 九色角色编码。
    ''' </summary>
    Public Class PreprocessedNetwork

        ''' <summary>原始网络模型。</summary>
        Public Property Network As MetabolicNetwork

        ''' <summary>全部代谢物副本。</summary>
        Public Property Copies As MetaboliteCopy()

        ''' <summary>全部反应节点。</summary>
        Public Property ReactionNodes As ReactionNode()

        ''' <summary>被判定为枢纽（绘制在边界并参与块间连接）的代谢物编号。</summary>
        Public Property HubMetabolites As String()

        ''' <summary>被完全复制的「不重要」代谢物编号。</summary>
        Public Property TrivialMetabolites As String()

        Private _copyIndex As Dictionary(Of String, MetaboliteCopy)
        Private _copyByCategory As Dictionary(Of String, List(Of MetaboliteCopy))
        Private _reactionByCategory As Dictionary(Of String, List(Of ReactionNode))
        Private _roleIndex As Dictionary(Of String, Dictionary(Of String, MetaboliteRole))
        Private _hubSet As HashSet(Of String)

        ''' <summary>构造节点编号的规则（同时供模型与路由复用）。</summary>
        Public Shared Function CopyNodeId(metaboliteId As String, categoryId As String) As String
            Return $"M:{metaboliteId}@{categoryId}"
        End Function

        ''' <summary>构造反应节点编号的规则。</summary>
        Public Shared Function ReactionNodeId(reactionId As String) As String
            Return $"R:{reactionId}"
        End Function

        Private Sub BuildIndex()
            If _copyIndex IsNot Nothing Then
                Return
            End If

            _copyIndex = New Dictionary(Of String, MetaboliteCopy)(StringComparer.Ordinal)
            _copyByCategory = New Dictionary(Of String, List(Of MetaboliteCopy))(StringComparer.Ordinal)
            _reactionByCategory = New Dictionary(Of String, List(Of ReactionNode))(StringComparer.Ordinal)
            _hubSet = New HashSet(Of String)(HubMetabolites.SafeQuery, StringComparer.Ordinal)

            For Each copy As MetaboliteCopy In Copies.SafeQuery
                If Not _copyIndex.ContainsKey(copy.NodeId) Then
                    _copyIndex.Add(copy.NodeId, copy)
                    AppendTo(_copyByCategory, copy.CategoryId, copy)
                End If
            Next

            For Each rxn As ReactionNode In ReactionNodes.SafeQuery
                AppendTo(_reactionByCategory, rxn.CategoryId, rxn)
            Next
        End Sub

        Private Shared Sub AppendTo(Of T)(table As Dictionary(Of String, List(Of T)), key As String, value As T)
            Dim bucket As List(Of T) = Nothing

            If Not table.TryGetValue(key, bucket) Then
                bucket = New List(Of T)()
                table.Add(key, bucket)
            End If

            bucket.Add(value)
        End Sub

        ''' <summary>查询某个类别中的全部代谢物副本。</summary>
        Public Function CopiesIn(categoryId As String) As MetaboliteCopy()
            BuildIndex()

            Dim bucket As List(Of MetaboliteCopy) = Nothing

            If Not String.IsNullOrEmpty(categoryId) AndAlso _copyByCategory.TryGetValue(categoryId, bucket) Then
                Return bucket.ToArray
            Else
                Return New MetaboliteCopy() {}
            End If
        End Function

        ''' <summary>查询某个类别中的全部反应节点。</summary>
        Public Function ReactionNodesIn(categoryId As String) As ReactionNode()
            BuildIndex()

            Dim bucket As List(Of ReactionNode) = Nothing

            If Not String.IsNullOrEmpty(categoryId) AndAlso _reactionByCategory.TryGetValue(categoryId, bucket) Then
                Return bucket.ToArray
            Else
                Return New ReactionNode() {}
            End If
        End Function

        ''' <summary>按节点编号查询代谢物副本。</summary>
        Public Function GetCopy(nodeId As String) As MetaboliteCopy
            BuildIndex()

            Dim hit As MetaboliteCopy = Nothing

            If Not String.IsNullOrEmpty(nodeId) AndAlso _copyIndex.TryGetValue(nodeId, hit) Then
                Return hit
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>查询某个代谢物在某个类别中的副本。</summary>
        Public Function GetCopy(metaboliteId As String, categoryId As String) As MetaboliteCopy
            Return GetCopy(CopyNodeId(metaboliteId, categoryId))
        End Function

        ''' <summary>查询一个代谢物的全部副本。</summary>
        Public Function CopiesOf(metaboliteId As String) As MetaboliteCopy()
            BuildIndex()

            Return Copies.SafeQuery _
                .Where(Function(c) String.Equals(c.MetaboliteId, metaboliteId, StringComparison.Ordinal)) _
                .OrderBy(Function(c) c.CategoryId, StringComparer.Ordinal) _
                .ToArray
        End Function

        ''' <summary>判断某个代谢物是否为枢纽。</summary>
        Public Function IsHub(metaboliteId As String) As Boolean
            BuildIndex()
            Return _hubSet.Contains(metaboliteId)
        End Function

        ''' <summary>全部枢纽代谢物的副本（每个参与类别一份，位于街区边界）。</summary>
        Public Function HubCopies() As MetaboliteCopy()
            BuildIndex()

            Return Copies.SafeQuery _
                .Where(Function(c) c.IsHub) _
                .OrderBy(Function(c) c.MetaboliteId, StringComparer.Ordinal) _
                .ThenBy(Function(c) c.CategoryId, StringComparer.Ordinal) _
                .ToArray
        End Function

        Private Sub EnsureRoleIndex(network As MetabolicNetwork)
            If _roleIndex IsNot Nothing Then
                Return
            End If

            _roleIndex = New Dictionary(Of String, Dictionary(Of String, MetaboliteRole))(StringComparer.Ordinal)

            For Each cat As Category In network.Categories.SafeQuery
                Dim table As New Dictionary(Of String, MetaboliteRole)(StringComparer.Ordinal)
                _roleIndex.Add(cat.Id, table)

                For Each rxn As MetabolicReaction In network.ReactionsOfCategory(cat.Id)
                    Accumulate(table, rxn.left, MetaboliteRole.Reactant)
                    Accumulate(table, rxn.right, MetaboliteRole.Product)
                Next
            Next
        End Sub

        Private Shared Sub Accumulate(table As Dictionary(Of String, MetaboliteRole),
                                      species As CompoundSpecieReference(),
                                      role As MetaboliteRole)

            For Each item As CompoundSpecieReference In species.SafeQuery
                If item Is Nothing OrElse String.IsNullOrEmpty(item.ID) Then
                    Continue For
                End If

                Dim current As MetaboliteRole = MetaboliteRole.None
                table.TryGetValue(item.ID, current)
                table(item.ID) = current Or role
            Next
        End Sub

        ''' <summary>查询一个代谢物在指定类别中的角色。</summary>
        Public Function RoleIn(metaboliteId As String, categoryId As String) As MetaboliteRole
            If Network Is Nothing Then
                Return MetaboliteRole.None
            End If

            EnsureRoleIndex(Network)

            Dim table As Dictionary(Of String, MetaboliteRole) = Nothing

            If Not String.IsNullOrEmpty(categoryId) AndAlso _roleIndex.TryGetValue(categoryId, table) Then
                Dim role As MetaboliteRole = MetaboliteRole.None

                If table.TryGetValue(metaboliteId, role) Then
                    Return role
                End If
            End If

            Return MetaboliteRole.None
        End Function

        ''' <summary>
        ''' 计算一个共享代谢物在两个类别之间的九色角色编码。
        ''' </summary>
        Public Function RoleBetween(metaboliteId As String, sourceCategory As String, targetCategory As String) As EdgeRole
            Dim ra As MetaboliteRole = RoleIn(metaboliteId, sourceCategory)
            Dim rb As MetaboliteRole = RoleIn(metaboliteId, targetCategory)

            Return CType(RoleIndexOf(ra) * 3 + RoleIndexOf(rb), EdgeRole)
        End Function

        ''' <summary>把角色位掩码映射为 0/1/2（底物/产物/两者兼有）。</summary>
        Public Shared Function RoleIndexOf(role As MetaboliteRole) As Integer
            Select Case role
                Case MetaboliteRole.Reactant
                    Return 0
                Case MetaboliteRole.Product
                    Return 1
                Case MetaboliteRole.Both
                    Return 2
                Case Else
                    Return 0
            End Select
        End Function

        ''' <summary>
        ''' 为一个类别构建「反应—代谢物」二部子图，供块内正交布局使用。
        ''' </summary>
        ''' <remarks>
        ''' 节点半径按度数做简单缩放，让 HOLA 的初始散布更稳定；
        ''' 枢纽代谢物仍参与子图，但后续会被移动到街区边界。
        ''' </remarks>
        Public Function SubGraph(categoryId As String) As NetworkGraph
            Dim g As New NetworkGraph()
            Dim reactions As ReactionNode() = ReactionNodesIn(categoryId)
            Dim copies As MetaboliteCopy() = CopiesIn(categoryId)

            For Each rxn As ReactionNode In reactions
                Dim node As Node = g.CreateNode(rxn.NodeId, New NodeData With {
                    .label = rxn.ReactionId,
                    .size = New Double() {6, 6},
                    .mass = 1.0,
                    .origID = rxn.ReactionId,
                    .weights = New Double() {},
                    .neighbours = New Integer() {}
                })

                node.SetMetadata("kind", "reaction")
                node.SetMetadata("category", categoryId)
            Next

            For Each copy As MetaboliteCopy In copies
                Dim cpd As MetabolicCompound = Network.GetCompound(copy.MetaboliteId)
                Dim radius As Double = If(copy.IsHub, 9, 6)

                Dim node As Node = g.CreateNode(copy.NodeId, New NodeData With {
                    .label = If(cpd Is Nothing, copy.MetaboliteId, If(cpd.name, copy.MetaboliteId)),
                    .size = New Double() {radius, radius},
                    .mass = If(copy.IsHub, 2.0, 1.0),
                    .origID = copy.MetaboliteId,
                    .weights = New Double() {},
                    .neighbours = New Integer() {}
                })

                node.SetMetadata("kind", "metabolite")
                node.SetMetadata("category", categoryId)
                node.SetMetadata("hub", If(copy.IsHub, "true", "false"))
            Next

            For Each rxn As ReactionNode In reactions
                Dim rxnModel As MetabolicReaction = Network.GetReaction(rxn.ReactionId)

                If rxnModel Is Nothing Then
                    Continue For
                End If

                For Each cpdId As String In rxnModel.SpeciesIds()
                    Dim cpdNodeId As String = CopyNodeId(cpdId, categoryId)

                    If g.ExistVertex(cpdNodeId) AndAlso Not g.ExistEdge(rxn.NodeId, cpdNodeId) Then
                        g.CreateEdge(rxn.NodeId, cpdNodeId)
                    End If
                Next
            Next

            Return g
        End Function

        ''' <summary>生成统计摘要。</summary>
        Public Function Statistics() As String
            Dim sb As New StringBuilder()

            sb.AppendLine($"copies       : {If(Copies Is Nothing, 0, Copies.Length)}")
            sb.AppendLine($"reaction nds : {If(ReactionNodes Is Nothing, 0, ReactionNodes.Length)}")
            sb.AppendLine($"hubs         : {If(HubMetabolites Is Nothing, 0, HubMetabolites.Length)}")
            sb.AppendLine($"trivial      : {If(TrivialMetabolites Is Nothing, 0, TrivialMetabolites.Length)}")

            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Return $"preprocessed: {If(Copies Is Nothing, 0, Copies.Length)} copies / {If(HubMetabolites Is Nothing, 0, HubMetabolites.Length)} hubs"
        End Function

    End Class

    ''' <summary>
    ''' 预处理选项。
    ''' </summary>
    Public Class PreprocessOptions

        ''' <summary>
        ''' 需要被「完全复制」的不重要代谢物编号（水、质子等）。
        ''' </summary>
        Public Property TrivialMetabolites As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>
        ''' 名称/编号中包含这些子串的代谢物一律视为不重要代谢物。
        ''' </summary>
        Public Property TrivialNamePatterns As New List(Of String) From {"WATER", "PROTON"}

        ''' <summary>
        ''' 化学式等于这些值的代谢物一律视为不重要代谢物。
        ''' </summary>
        Public Property TrivialFormulas As New List(Of String) From {"H2O"}

        ''' <summary>
        ''' 强制视为枢纽的代谢物编号（优先级高于不重要判定）。
        ''' </summary>
        Public Property ForceHubMetabolites As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>
        ''' 判定一个代谢物是否「不重要」（从而被完全复制）。
        ''' </summary>
        Public Function IsTrivial(id As String, cpd As MetabolicCompound) As Boolean
            If String.IsNullOrEmpty(id) Then
                Return False
            End If

            If ForceHubMetabolites.Contains(id) Then
                Return False
            End If

            If TrivialMetabolites.Contains(id) Then
                Return True
            End If

            Dim upper As String = id.ToUpperInvariant()

            For Each pattern As String In TrivialNamePatterns
                If Not String.IsNullOrEmpty(pattern) AndAlso upper.Contains(pattern.ToUpperInvariant()) Then
                    Return True
                End If
            Next

            If cpd IsNot Nothing AndAlso Not String.IsNullOrEmpty(cpd.formula) Then
                Dim formula As String = cpd.formula.Replace(" ", "")

                For Each candidate As String In TrivialFormulas
                    If String.Equals(formula, candidate, StringComparison.OrdinalIgnoreCase) Then
                        Return True
                    End If
                Next
            End If

            Return False
        End Function

    End Class

    ''' <summary>
    ''' 代谢网络的预处理器：节点复制 + 枢纽判定 + 九色角色预计算。
    ''' </summary>
    Public Class NetworkPreprocessor

        ''' <summary>预处理选项。</summary>
        Public Property Options As PreprocessOptions

        Public Sub New(Optional options As PreprocessOptions = Nothing)
            Me.Options = If(options, New PreprocessOptions())
        End Sub

        ''' <summary>
        ''' 执行预处理。
        ''' </summary>
        Public Function Process(network As MetabolicNetwork) As PreprocessedNetwork
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            Dim copies As New List(Of MetaboliteCopy)()
            Dim reactions As New List(Of ReactionNode)()
            Dim hubs As New HashSet(Of String)(StringComparer.Ordinal)
            Dim trivial As New HashSet(Of String)(StringComparer.Ordinal)
            Dim categoryCount As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For Each cpd As MetabolicCompound In network.Compounds.SafeQuery
                categoryCount(cpd.id) = network.CategoriesOfMetabolite(cpd.id).Length
            Next

            For Each cat As Category In network.Categories.SafeQuery
                For Each rxn As MetabolicReaction In network.ReactionsOfCategory(cat.Id)
                    reactions.Add(New ReactionNode With {
                        .NodeId = PreprocessedNetwork.ReactionNodeId(rxn.id),
                        .ReactionId = rxn.id,
                        .CategoryId = cat.Id,
                        .IsReversible = rxn.is_reversible
                    })
                Next

                For Each cpdId As String In cat.MetaboliteIds.SafeQuery
                    Dim cpd As MetabolicCompound = network.GetCompound(cpdId)
                    Dim isTrivial As Boolean = Options.IsTrivial(cpdId, cpd)
                    Dim shared_ As Integer = 0
                    categoryCount.TryGetValue(cpdId, shared_)

                    ' 不重要代谢物：完全复制（每个类别一份，互不相连）
                    ' 其它跨类别代谢物：作为枢纽，绘制在街区边界并参与块间路由
                    Dim isHub As Boolean = (Not isTrivial) AndAlso shared_ >= 2

                    If isTrivial Then
                        trivial.Add(cpdId)
                    ElseIf isHub Then
                        hubs.Add(cpdId)
                    End If

                    copies.Add(New MetaboliteCopy With {
                        .NodeId = PreprocessedNetwork.CopyNodeId(cpdId, cat.Id),
                        .MetaboliteId = cpdId,
                        .CategoryId = cat.Id,
                        .Role = MetaboliteRole.None,
                        .IsHub = isHub,
                        .IsTrivial = isTrivial
                    })
                Next
            Next

            Dim result As New PreprocessedNetwork With {
                .Network = network,
                .Copies = copies.ToArray,
                .ReactionNodes = reactions.ToArray,
                .HubMetabolites = hubs.OrderBy(Function(s) s, StringComparer.Ordinal).ToArray,
                .TrivialMetabolites = trivial.OrderBy(Function(s) s, StringComparer.Ordinal).ToArray
            }

            ' 回填每个副本的角色（底物/产物/两者兼有）
            For Each copy As MetaboliteCopy In result.Copies
                copy.Role = result.RoleIn(copy.MetaboliteId, copy.CategoryId)
            Next

            Return result
        End Function

    End Class

    ''' <summary>
    ''' 长边分解产生的线段：有向段承载方向信息，无向段允许沿街区边界成束捆绑。
    ''' </summary>
    Public Class RouteSegment

        ''' <summary>线段顶点序列。</summary>
        Public Property Points As PointF()

        ''' <summary>是否有向。</summary>
        Public Property IsDirected As Boolean

        Public Overrides Function ToString() As String
            Return $"{If(IsDirected, "directed", "undirected")} segment, {If(Points Is Nothing, 0, Points.Length)} pts"
        End Function

    End Class

    ''' <summary>
    ''' 长有向边分解（论文 Figure 4a）。
    ''' </summary>
    ''' <remarks>
    ''' 一条较长的有向边被切成三段：离开源点的有向段、中间可捆绑的无向段、
    ''' 进入目标点的有向段。当总长度不足以切分时保持整段有向。
    ''' </remarks>
    Public Module LongEdgeDecomposer

        ''' <summary>默认的端部有向段长度（像素）。</summary>
        Public Const DefaultHeadLength As Double = 48

        ''' <summary>
        ''' 分解一条折线。
        ''' </summary>
        ''' <param name="points">折线顶点。</param>
        ''' <param name="directed">原始边是否有向。</param>
        ''' <param name="headLength">端部有向段的长度。</param>
        Public Function Decompose(points As PointF(),
                                  directed As Boolean,
                                  Optional headLength As Double = DefaultHeadLength) As RouteSegment()

            If points Is Nothing OrElse points.Length < 2 Then
                Return New RouteSegment() {}
            End If

            If Not directed Then
                Return New RouteSegment() {
                    New RouteSegment With {.Points = points, .IsDirected = False}
                }
            End If

            Dim total As Double = PolylineLength(points)

            ' 太短的长边不值得分解，整段保持有向
            If total <= headLength * 2 Then
                Return New RouteSegment() {
                    New RouteSegment With {.Points = points, .IsDirected = True}
                }
            End If

            Dim cutA As PointF() = SubPolyline(points, 0, headLength, True)
            Dim cutB As PointF() = SubPolyline(points, total - headLength, total, False)

            If cutA Is Nothing OrElse cutB Is Nothing OrElse cutA.Length < 2 OrElse cutB.Length < 2 Then
                Return New RouteSegment() {
                    New RouteSegment With {.Points = points, .IsDirected = True}
                }
            End If

            Dim middle As PointF() = SubPolyline(points, headLength, total - headLength, False)

            If middle Is Nothing OrElse middle.Length < 2 Then
                Return New RouteSegment() {
                    New RouteSegment With {.Points = points, .IsDirected = True}
                }
            End If

            Return New RouteSegment() {
                New RouteSegment With {.Points = cutA, .IsDirected = True},
                New RouteSegment With {.Points = middle, .IsDirected = False},
                New RouteSegment With {.Points = cutB, .IsDirected = True}
            }
        End Function

        ''' <summary>折线总长度。</summary>
        Public Function PolylineLength(points As PointF()) As Double
            Dim sum As Double = 0

            For i As Integer = 1 To points.Length - 1
                Dim dx As Double = points(i).X - points(i - 1).X
                Dim dy As Double = points(i).Y - points(i - 1).Y
                sum += Math.Sqrt(dx * dx + dy * dy)
            Next

            Return sum
        End Function

        ''' <summary>
        ''' 按弧长区间 [from, [to]] 截取折线，并保留区间端点处的插值点。
        ''' </summary>
        Private Function SubPolyline(points As PointF(), from As Double, [to] As Double, includeStart As Boolean) As PointF()
            If [to] <= from Then
                Return Nothing
            End If

            Dim result As New List(Of PointF)()
            Dim acc As Double = 0
            Dim started As Boolean = False

            For i As Integer = 1 To points.Length - 1
                Dim a As PointF = points(i - 1)
                Dim b As PointF = points(i)
                Dim dx As Double = b.X - a.X
                Dim dy As Double = b.Y - a.Y
                Dim seg As Double = Math.Sqrt(dx * dx + dy * dy)

                If seg <= 0 Then
                    Continue For
                End If

                Dim segStart As Double = acc
                Dim segEnd As Double = acc + seg

                If segEnd < from OrElse segStart > [to] Then
                    acc = segEnd
                    Continue For
                End If

                If Not started Then
                    Dim t0 As Double = Math.Max(0, (from - segStart) / seg)
                    result.Add(New PointF(CSng(a.X + dx * t0), CSng(a.Y + dy * t0)))
                    started = True
                End If

                If segEnd <= [to] Then
                    result.Add(b)
                Else
                    Dim t1 As Double = ([to] - segStart) / seg
                    result.Add(New PointF(CSng(a.X + dx * t1), CSng(a.Y + dy * t1)))
                End If

                acc = segEnd
            Next

            Return result.ToArray
        End Function

    End Module

    ''' <summary>
    ''' 反应模型的枚举扩展（左右两侧统一处理）。
    ''' </summary>
    Public Module MetabolicReactionExtensions

        ''' <summary>枚举一条反应的全部物种引用（先底物后产物）。</summary>
        <System.Runtime.CompilerServices.Extension>
        Public Iterator Function EnumerateSpecies(rxn As MetabolicReaction) As IEnumerable(Of CompoundSpecieReference)
            For Each item As CompoundSpecieReference In rxn.left.SafeQuery
                Yield item
            Next

            For Each item As CompoundSpecieReference In rxn.right.SafeQuery
                Yield item
            Next
        End Function

        ''' <summary>枚举一条反应涉及的全部代谢物编号（去重）。</summary>
        <System.Runtime.CompilerServices.Extension>
        Public Function SpeciesIds(rxn As MetabolicReaction) As String()
            Return rxn.EnumerateSpecies() _
                .Where(Function(item) item IsNot Nothing AndAlso Not String.IsNullOrEmpty(item.ID)) _
                .Select(Function(item) item.ID) _
                .Distinct(StringComparer.Ordinal) _
                .ToArray
        End Function

    End Module

End Namespace

#End Region
