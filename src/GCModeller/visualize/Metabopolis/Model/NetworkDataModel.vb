#Region "Metabopolis: network data model"

' ============================================================================
' Metabopolis 核心数据模型
' ----------------------------------------------------------------------------
' Metabopolis 算法把一张大型代谢网络按功能「类别」（BioCyc 的通路 / SBML 的
' compartment）划分成若干矩形街区（urban block），街区之间按共享代谢物频次
' 建立骨架连接，再依次执行 floor-planning、块内正交布局和块间边路由。
'
' 这个文件定义流水线的统一输入：代谢物、反应、类别以及类别之间的连接权重。
' 代谢物/反应实体直接复用现有代码框架中的 SMRUCC.genomics.MetabolicModel 模型，
' 以保证后续可以接入既有的代谢工作流。
' ============================================================================

Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel

Namespace Model

    ''' <summary>
    ''' 代谢通路类别，对应论文中的「城市街区」(urban block)。
    ''' </summary>
    Public Class Category

        ''' <summary>
        ''' 类别的唯一编号（BioCyc 为通路 UniqueId，SBML 为 compartment Id）。
        ''' </summary>
        Public Property Id As String

        ''' <summary>
        ''' 用于显示的类别名称。
        ''' </summary>
        Public Property Name As String

        ''' <summary>
        ''' 绘制面积权重：论文中区块的初始面积正比于类别内反应物与产物的数量。
        ''' </summary>
        Public Property Weight As Double

        ''' <summary>
        ''' 类别的展示次序（默认按 <see cref="Weight"/> 降序编号）。
        ''' </summary>
        Public Property Order As Integer

        ''' <summary>
        ''' 归属于本类别的反应编号。
        ''' </summary>
        Public Property ReactionIds As String()

        ''' <summary>
        ''' 本类别内出现过的代谢物编号（可能与其他类别共享）。
        ''' </summary>
        Public Property MetaboliteIds As String()

        ''' <summary>
        ''' 类别内的反应数量。
        ''' </summary>
        Public ReadOnly Property ReactionCount As Integer
            Get
                Return If(ReactionIds Is Nothing, 0, ReactionIds.Length)
            End Get
        End Property

        ''' <summary>
        ''' 类别内的代谢物数量。
        ''' </summary>
        Public ReadOnly Property MetaboliteCount As Integer
            Get
                Return If(MetaboliteIds Is Nothing, 0, MetaboliteIds.Length)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Id} ({ReactionCount} reactions, {MetaboliteCount} metabolites)"
        End Function

    End Class

    ''' <summary>
    ''' 骨架图中两个类别之间的一条连接。
    ''' </summary>
    ''' <remarks>
    ''' 边权定义为两个类别共享的代谢物频次：共享的代谢物越多，两个类别在
    ''' 地图上越应该被摆放成相邻街区。
    ''' </remarks>
    Public Class CategoryLink

        ''' <summary>
        ''' 连接的一个端点类别编号。
        ''' </summary>
        Public Property Source As String

        ''' <summary>
        ''' 连接的另一个端点类别编号。
        ''' </summary>
        Public Property Target As String

        ''' <summary>
        ''' 两个类别共享的代谢物数量，即骨架图的边权。
        ''' </summary>
        Public Property Weight As Integer

        ''' <summary>
        ''' 两个类别共享的代谢物编号列表。
        ''' </summary>
        Public Property SharedMetabolites As String()

        ''' <summary>
        ''' 无向边的标准编号，保证 (a,b) 与 (b,a) 得到同一个 Id。
        ''' </summary>
        Public ReadOnly Property Id As String
            Get
                If String.CompareOrdinal(Source, Target) <= 0 Then
                    Return $"{Source}|{Target}"
                Else
                    Return $"{Target}|{Source}"
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Id} w={Weight}"
        End Function

    End Class

    ''' <summary>
    ''' Metabopolis 布局流水线的输入网络模型。
    ''' </summary>
    Public Class MetabolicNetwork

        ''' <summary>
        ''' 网络编号。
        ''' </summary>
        Public Property Id As String

        ''' <summary>
        ''' 网络名称。
        ''' </summary>
        Public Property Name As String

        ''' <summary>
        ''' 数据来源描述，例如 BioCyc PGDB 目录或者 SBML 文件路径。
        ''' </summary>
        Public Property Source As String

        ''' <summary>
        ''' 全部代谢物。
        ''' </summary>
        Public Property Compounds As MetabolicCompound()

        ''' <summary>
        ''' 全部反应（方向已归正为 left -&gt; right）。
        ''' </summary>
        Public Property Reactions As MetabolicReaction()

        ''' <summary>
        ''' 全部功能类别。
        ''' </summary>
        Public Property Categories As Category()

        Private _compoundIndex As Dictionary(Of String, MetabolicCompound)
        Private _reactionIndex As Dictionary(Of String, MetabolicReaction)
        Private _categoryIndex As Dictionary(Of String, Category)
        Private _reactionCategory As Dictionary(Of String, String)
        Private _metaboliteCategories As Dictionary(Of String, String())
        Private _categoryLinks As CategoryLink()

        ''' <summary>
        ''' 代谢物编号索引。
        ''' </summary>
        Public Function CompoundIndex() As Dictionary(Of String, MetabolicCompound)
            If _compoundIndex Is Nothing Then
                _compoundIndex = New Dictionary(Of String, MetabolicCompound)(StringComparer.Ordinal)

                For Each cpd As MetabolicCompound In Compounds.SafeQuery
                    If Not String.IsNullOrEmpty(cpd?.id) AndAlso Not _compoundIndex.ContainsKey(cpd.id) Then
                        _compoundIndex.Add(cpd.id, cpd)
                    End If
                Next
            End If

            Return _compoundIndex
        End Function

        ''' <summary>
        ''' 反应编号索引。
        ''' </summary>
        Public Function ReactionIndex() As Dictionary(Of String, MetabolicReaction)
            If _reactionIndex Is Nothing Then
                _reactionIndex = New Dictionary(Of String, MetabolicReaction)(StringComparer.Ordinal)

                For Each rxn As MetabolicReaction In Reactions.SafeQuery
                    If Not String.IsNullOrEmpty(rxn?.id) AndAlso Not _reactionIndex.ContainsKey(rxn.id) Then
                        _reactionIndex.Add(rxn.id, rxn)
                    End If
                Next
            End If

            Return _reactionIndex
        End Function

        ''' <summary>
        ''' 类别编号索引。
        ''' </summary>
        Public Function CategoryIndex() As Dictionary(Of String, Category)
            If _categoryIndex Is Nothing Then
                _categoryIndex = New Dictionary(Of String, Category)(StringComparer.Ordinal)

                For Each cat As Category In Categories.SafeQuery
                    If Not String.IsNullOrEmpty(cat?.Id) AndAlso Not _categoryIndex.ContainsKey(cat.Id) Then
                        _categoryIndex.Add(cat.Id, cat)
                    End If
                Next
            End If

            Return _categoryIndex
        End Function

        ''' <summary>
        ''' 反应编号到所属类别的映射。
        ''' </summary>
        Public Function ReactionCategoryIndex() As Dictionary(Of String, String)
            If _reactionCategory Is Nothing Then
                _reactionCategory = New Dictionary(Of String, String)(StringComparer.Ordinal)

                For Each cat As Category In Categories.SafeQuery
                    For Each rxnId As String In cat.ReactionIds.SafeQuery
                        If Not _reactionCategory.ContainsKey(rxnId) Then
                            _reactionCategory.Add(rxnId, cat.Id)
                        End If
                    Next
                Next
            End If

            Return _reactionCategory
        End Function

        ''' <summary>
        ''' 查询一个代谢物出现在哪些类别中（通过它参与的反应推导）。
        ''' </summary>
        Public Function CategoriesOfMetabolite(cpdId As String) As String()
            If _metaboliteCategories Is Nothing Then
                Dim buffer As New Dictionary(Of String, HashSet(Of String))(StringComparer.Ordinal)

                For Each cat As Category In Categories.SafeQuery
                    If cat.MetaboliteIds Is Nothing Then
                        Continue For
                    End If

                    For Each cpd As String In cat.MetaboliteIds
                        Dim bucket As HashSet(Of String) = Nothing
                        If Not buffer.TryGetValue(cpd, bucket) Then
                            bucket = New HashSet(Of String)(StringComparer.Ordinal)
                            buffer.Add(cpd, bucket)
                        End If

                        bucket.Add(cat.Id)
                    Next
                Next

                _metaboliteCategories = New Dictionary(Of String, String())(StringComparer.Ordinal)

                For Each item As KeyValuePair(Of String, HashSet(Of String)) In buffer
                    _metaboliteCategories.Add(item.Key, item.Value.OrderBy(Function(s) s).ToArray)
                Next
            End If

            Dim hits As String() = Nothing
            If _metaboliteCategories.TryGetValue(cpdId, hits) Then
                Return hits
            Else
                Return New String() {}
            End If
        End Function

        ''' <summary>
        ''' 某个代谢物参与的类别数量（用于判断是否为跨类别的枢纽代谢物）。
        ''' </summary>
        Public Function CategoryDegreeOfMetabolite(cpdId As String) As Integer
            Return CategoriesOfMetabolite(cpdId).Length
        End Function

        ''' <summary>
        ''' 计算两个类别共享的代谢物编号。
        ''' </summary>
        Public Function SharedMetabolites(a As String, b As String) As String()
            Dim ca As Category = GetCategory(a)
            Dim cb As Category = GetCategory(b)

            If ca Is Nothing OrElse cb Is Nothing Then
                Return New String() {}
            End If

            Dim sb As New HashSet(Of String)(cb.MetaboliteIds.SafeQuery, StringComparer.Ordinal)

            Return ca.MetaboliteIds.SafeQuery _
                .Where(Function(id) sb.Contains(id)) _
                .Distinct(StringComparer.Ordinal) _
                .OrderBy(Function(id) id, StringComparer.Ordinal) _
                .ToArray
        End Function

        ''' <summary>
        ''' 类别连接图：以共享代谢物频次为边权的无向带权图。
        ''' </summary>
        ''' <remarks>
        ''' 只返回权重大于 0 的边；自环被忽略。结果按权重降序排列，
        ''' 以便骨架构建阶段直接按序贪心加边。
        ''' </remarks>
        Public Function CategoryLinks() As CategoryLink()
            If _categoryLinks Is Nothing Then
                Dim list As New List(Of CategoryLink)()
                Dim cats As Category() = Categories.SafeQuery.ToArray

                For i As Integer = 0 To cats.Length - 1
                    For j As Integer = i + 1 To cats.Length - 1
                        Dim shared_ As String() = SharedMetabolites(cats(i).Id, cats(j).Id)

                        If shared_.Length > 0 Then
                            list.Add(New CategoryLink With {
                                .Source = cats(i).Id,
                                .Target = cats(j).Id,
                                .Weight = shared_.Length,
                                .SharedMetabolites = shared_
                            })
                        End If
                    Next
                Next

                _categoryLinks = list _
                    .OrderByDescending(Function(l) l.Weight) _
                    .ThenBy(Function(l) l.Id, StringComparer.Ordinal) _
                    .ToArray
            End If

            Return _categoryLinks
        End Function

        ''' <summary>
        ''' 查询一个类别中的反应实体。
        ''' </summary>
        Public Function ReactionsOfCategory(catId As String) As MetabolicReaction()
            Dim cat As Category = GetCategory(catId)

            If cat Is Nothing Then
                Return New MetabolicReaction() {}
            End If

            Dim index As Dictionary(Of String, MetabolicReaction) = ReactionIndex()
            Dim result As New List(Of MetabolicReaction)()

            For Each rxnId As String In cat.ReactionIds.SafeQuery
                Dim rxn As MetabolicReaction = Nothing
                If index.TryGetValue(rxnId, rxn) Then
                    result.Add(rxn)
                End If
            Next

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 查询一个类别中的代谢物实体。
        ''' </summary>
        Public Function CompoundsOfCategory(catId As String) As MetabolicCompound()
            Dim cat As Category = GetCategory(catId)

            If cat Is Nothing Then
                Return New MetabolicCompound() {}
            End If

            Dim index As Dictionary(Of String, MetabolicCompound) = CompoundIndex()
            Dim result As New List(Of MetabolicCompound)()

            For Each cpdId As String In cat.MetaboliteIds.SafeQuery
                Dim cpd As MetabolicCompound = Nothing
                If index.TryGetValue(cpdId, cpd) Then
                    result.Add(cpd)
                End If
            Next

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 按编号查询代谢物。
        ''' </summary>
        Public Function GetCompound(id As String) As MetabolicCompound
            Dim hit As MetabolicCompound = Nothing

            If Not String.IsNullOrEmpty(id) AndAlso CompoundIndex().TryGetValue(id, hit) Then
                Return hit
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 按编号查询反应。
        ''' </summary>
        Public Function GetReaction(id As String) As MetabolicReaction
            Dim hit As MetabolicReaction = Nothing

            If Not String.IsNullOrEmpty(id) AndAlso ReactionIndex().TryGetValue(id, hit) Then
                Return hit
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 按编号查询类别。
        ''' </summary>
        Public Function GetCategory(id As String) As Category
            Dim hit As Category = Nothing

            If Not String.IsNullOrEmpty(id) AndAlso CategoryIndex().TryGetValue(id, hit) Then
                Return hit
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 查询一个反应所属的类别编号（每个反应只归属一个主类别）。
        ''' </summary>
        Public Function CategoryOfReaction(rxnId As String) As String
            Dim hit As String = Nothing

            If Not String.IsNullOrEmpty(rxnId) AndAlso ReactionCategoryIndex().TryGetValue(rxnId, hit) Then
                Return hit
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 代谢物总数。
        ''' </summary>
        Public ReadOnly Property TotalCompoundCount As Integer
            Get
                Return If(Compounds Is Nothing, 0, Compounds.Length)
            End Get
        End Property

        ''' <summary>
        ''' 反应总数。
        ''' </summary>
        Public ReadOnly Property TotalReactionCount As Integer
            Get
                Return If(Reactions Is Nothing, 0, Reactions.Length)
            End Get
        End Property

        ''' <summary>
        ''' 类别总数。
        ''' </summary>
        Public ReadOnly Property TotalCategoryCount As Integer
            Get
                Return If(Categories Is Nothing, 0, Categories.Length)
            End Get
        End Property

        ''' <summary>
        ''' 结构性自检：返回所有问题的描述；返回空数组表示网络结构自洽。
        ''' </summary>
        Public Function Verify() As String()
            Dim problems As New List(Of String)()
            Dim cpdIndex As Dictionary(Of String, MetabolicCompound) = CompoundIndex()

            If TotalCompoundCount = 0 Then
                problems.Add("network contains no metabolite")
            End If
            If TotalReactionCount = 0 Then
                problems.Add("network contains no reaction")
            End If
            If TotalCategoryCount = 0 Then
                problems.Add("network contains no category")
            End If

            Dim assigned As Integer = 0

            For Each cat As Category In Categories.SafeQuery
                If cat.ReactionIds.SafeQuery.Any Then
                    assigned += 1
                Else
                    problems.Add($"category '{cat.Id}' contains no reaction")
                End If

                For Each cpdId As String In cat.MetaboliteIds.SafeQuery
                    If Not cpdIndex.ContainsKey(cpdId) Then
                        problems.Add($"category '{cat.Id}' references unknown metabolite '{cpdId}'")
                    End If
                Next
            Next

            If assigned = 0 AndAlso TotalCategoryCount > 0 Then
                problems.Add("no category contains any reaction")
            End If

            For Each rxn As MetabolicReaction In Reactions.SafeQuery
                If rxn.left Is Nothing OrElse rxn.left.Length = 0 Then
                    If rxn.right Is Nothing OrElse rxn.right.Length = 0 Then
                        problems.Add($"reaction '{rxn.id}' has neither reactant nor product")
                    End If
                End If

                If String.IsNullOrEmpty(CategoryOfReaction(rxn.id)) Then
                    problems.Add($"reaction '{rxn.id}' is not assigned to any category")
                End If
            Next

            Return problems.Distinct().ToArray
        End Function

        ''' <summary>
        ''' 生成用于日志输出的统计摘要。
        ''' </summary>
        Public Function Statistics() As String
            Dim sb As New StringBuilder()
            Dim links As CategoryLink() = CategoryLinks()

            sb.AppendLine($"network      : {Id} ({Name})")
            sb.AppendLine($"source       : {Source}")
            sb.AppendLine($"metabolites  : {TotalCompoundCount}")
            sb.AppendLine($"reactions    : {TotalReactionCount}")
            sb.AppendLine($"categories   : {TotalCategoryCount}")
            sb.AppendLine($"cat links    : {links.Length}")

            If links.Length > 0 Then
                sb.AppendLine($"max link w   : {links(0).Weight} ({links(0).Id})")
            End If

            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Return $"{Id}: {TotalCompoundCount} metabolites / {TotalReactionCount} reactions / {TotalCategoryCount} categories"
        End Function

    End Class

    ''' <summary>
    ''' 由「代谢物 + 反应 + 类别归属函数」装配 <see cref="MetabolicNetwork"/> 的工厂。
    ''' </summary>
    ''' <remarks>
    ''' 两种数据源适配器（BioCyc / SBML）共用这段装配逻辑，保证类别权重、
    ''' 索引与自检行为一致。
    ''' </remarks>
    Public Module MetabolicNetworkBuilder

        ''' <summary>
        ''' 构建网络模型。
        ''' </summary>
        ''' <param name="id">网络编号。</param>
        ''' <param name="name">网络名称。</param>
        ''' <param name="source">数据来源描述。</param>
        ''' <param name="compounds">代谢物集合。</param>
        ''' <param name="reactions">反应集合（方向需已归正为 left -&gt; right）。</param>
        ''' <param name="categoryOf">返回一条反应所属的类别编号；返回空值时归入 <paramref name="fallbackCategory"/>。</param>
        ''' <param name="categoryName">由类别编号解析显示名称，可为空。</param>
        ''' <param name="fallbackCategory">未归类反应的兜底类别编号。</param>
        Public Function Build(id As String,
                              name As String,
                              source As String,
                              compounds As IEnumerable(Of MetabolicCompound),
                              reactions As IEnumerable(Of MetabolicReaction),
                              categoryOf As Func(Of MetabolicReaction, String),
                              Optional categoryName As Func(Of String, String) = Nothing,
                              Optional fallbackCategory As String = "UNASSIGNED") As MetabolicNetwork

            Dim cpdList As New List(Of MetabolicCompound)()
            Dim cpdSeen As New HashSet(Of String)(StringComparer.Ordinal)

            For Each cpd As MetabolicCompound In compounds.SafeQuery
                If cpd Is Nothing OrElse String.IsNullOrEmpty(cpd.id) Then
                    Continue For
                End If

                If cpdSeen.Add(cpd.id) Then
                    cpdList.Add(cpd)
                End If
            Next

            Dim rxnList As New List(Of MetabolicReaction)()
            Dim rxnSeen As New HashSet(Of String)(StringComparer.Ordinal)

            For Each rxn As MetabolicReaction In reactions.SafeQuery
                If rxn Is Nothing OrElse String.IsNullOrEmpty(rxn.id) Then
                    Continue For
                End If

                If rxnSeen.Add(rxn.id) Then
                    rxnList.Add(rxn)
                End If
            Next

            ' 按主类别对反应分组
            Dim groups As New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.Ordinal)

            For Each rxn As MetabolicReaction In rxnList
                Dim catId As String = Nothing

                Try
                    catId = categoryOf(rxn)
                Catch ex As Exception
                    catId = Nothing
                End Try

                If String.IsNullOrEmpty(catId) Then
                    catId = fallbackCategory
                End If

                Dim bucket As List(Of MetabolicReaction) = Nothing
                If Not groups.TryGetValue(catId, bucket) Then
                    bucket = New List(Of MetabolicReaction)()
                    groups.Add(catId, bucket)
                End If

                bucket.Add(rxn)
            Next

            ' 组装类别：权重 = 类别内所有反应的（反应物+产物）数量
            Dim categoryList As New List(Of Category)()

            For Each item As KeyValuePair(Of String, List(Of MetabolicReaction)) In groups
                Dim metabolites As New List(Of String)()
                Dim metaboliteSeen As New HashSet(Of String)(StringComparer.Ordinal)
                Dim weight As Double = 0

                For Each rxn As MetabolicReaction In item.Value
                    weight += CountSpecies(rxn.left) + CountSpecies(rxn.right)

                    For Each cpdId As String In EnumerateSpeciesIds(rxn)
                        If metaboliteSeen.Add(cpdId) Then
                            metabolites.Add(cpdId)
                        End If
                    Next
                Next

                Dim display As String = item.Key
                If categoryName IsNot Nothing Then
                    Dim resolved As String = categoryName(item.Key)
                    If Not String.IsNullOrEmpty(resolved) Then
                        display = resolved
                    End If
                End If

                categoryList.Add(New Category With {
                    .Id = item.Key,
                    .Name = display,
                    .Weight = weight,
                    .ReactionIds = item.Value.Select(Function(r) r.id).ToArray,
                    .MetaboliteIds = metabolites.ToArray
                })
            Next

            Dim ordered As Category() = categoryList _
                .OrderByDescending(Function(c) c.Weight) _
                .ThenBy(Function(c) c.Id, StringComparer.Ordinal) _
                .ToArray

            For i As Integer = 0 To ordered.Length - 1
                ordered(i).Order = i
            Next

            Return New MetabolicNetwork With {
                .Id = id,
                .Name = name,
                .Source = source,
                .Compounds = cpdList.ToArray,
                .Reactions = rxnList.ToArray,
                .Categories = ordered
            }
        End Function

        Private Function CountSpecies(species As CompoundSpecieReference()) As Double
            If species Is Nothing Then
                Return 0
            End If

            Dim sum As Double = 0

            For Each item As CompoundSpecieReference In species
                If item Is Nothing Then
                    Continue For
                End If

                ' 化学计量数可能是 NaN/Infinity（数据源解析异常）或者缺失，
                ' 这类值会沿着「类别权重 -> 区块面积」一路污染成 NaN 几何，
                ' 因此在这里统一兜底为 1
                Dim n As Double = item.Stoichiometry

                If Double.IsNaN(n) OrElse Double.IsInfinity(n) OrElse n <= 0 Then
                    n = 1
                End If

                sum += n
            Next

            Return sum
        End Function

        Private Iterator Function EnumerateSpeciesIds(rxn As MetabolicReaction) As IEnumerable(Of String)
            For Each item As CompoundSpecieReference In rxn.left.SafeQuery
                If item IsNot Nothing AndAlso Not String.IsNullOrEmpty(item.ID) Then
                    Yield item.ID
                End If
            Next

            For Each item As CompoundSpecieReference In rxn.right.SafeQuery
                If item IsNot Nothing AndAlso Not String.IsNullOrEmpty(item.ID) Then
                    Yield item.ID
                End If
            Next
        End Function

    End Module

End Namespace

#End Region
