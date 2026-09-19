Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 一条"反应 -> 直接下游反应"的邻接关系
''' </summary>
Public Class ReactionLink

    ''' <summary>
    ''' 下游反应编号
    ''' </summary>
    Public Property ReactionID As String

    ''' <summary>
    ''' 上下游之间的化学覆盖率，即共享化合物数目 / max(上游产物数, 下游底物数)，取值 (0, 1]
    ''' </summary>
    Public Property Coverage As Double

    Public Overrides Function ToString() As String
        Return $"{ReactionID} ({Coverage.ToString("F2")})"
    End Function

End Class

''' <summary>
''' 参考代谢网络的倒排索引。
''' 
''' 该对象在构造时一次性完成全部索引构建，之后下游算法不再需要线性扫描：
''' 
''' + <see cref="ECtoReactions"/>：EC 编号 -&gt; 反应
''' + <see cref="ReactionIndex"/>：反应编号 -&gt; 反应（覆盖全部反应，包含没有 EC 编号的反应）
''' + <see cref="ReactionToPathways"/> / <see cref="PathwayReactions"/>：反应与通路的双向映射
''' + <see cref="SubstrateToReactions"/> / <see cref="ProductToReactions"/>：化合物 -&gt; 反应
''' + <see cref="ReactionNeighbours"/>：反应 -&gt; 化学上直接相邻的下游反应
''' </summary>
Public Class ContextIndices

    ''' <summary>
    ''' EC 编号 -&gt; 携带该 EC 编号的反应集合
    ''' </summary>
    Public Property ECtoReactions As Dictionary(Of String, List(Of MetabolicReaction))
    ''' <summary>
    ''' 反应编号 -&gt; 该反应所属的通路集合
    ''' </summary>
    Public Property ReactionToPathways As Dictionary(Of String, List(Of Pathway))
    ''' <summary>
    ''' 通路编号 -&gt; 该通路的反应集合
    ''' </summary>
    Public Property PathwayReactions As Dictionary(Of String, List(Of MetabolicReaction))
    ''' <summary>
    ''' 反应编号 -&gt; 反应对象（覆盖全部反应）
    ''' </summary>
    Public Property ReactionIndex As Dictionary(Of String, MetabolicReaction)
    ''' <summary>
    ''' 通路编号 -&gt; 通路对象
    ''' </summary>
    Public Property PathwayIndex As Dictionary(Of String, Pathway)
    ''' <summary>
    ''' 化合物编号 -&gt; 以该化合物为底物的反应集合
    ''' </summary>
    Public Property SubstrateToReactions As Dictionary(Of String, List(Of MetabolicReaction))
    ''' <summary>
    ''' 化合物编号 -&gt; 以该化合物为产物的反应集合
    ''' </summary>
    Public Property ProductToReactions As Dictionary(Of String, List(Of MetabolicReaction))
    ''' <summary>
    ''' 反应编号 -&gt; 化学上直接相邻的下游反应集合
    ''' </summary>
    Public Property ReactionNeighbours As Dictionary(Of String, List(Of ReactionLink))

    ''' <summary>
    ''' 参考通路集合（去重后）
    ''' </summary>
    Public Property Pathways As Pathway()

    Public Sub New(pathways As IEnumerable(Of Pathway))
        Me.ECtoReactions = New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.OrdinalIgnoreCase)
        Me.ReactionToPathways = New Dictionary(Of String, List(Of Pathway))(StringComparer.OrdinalIgnoreCase)
        Me.PathwayReactions = New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.OrdinalIgnoreCase)
        Me.ReactionIndex = New Dictionary(Of String, MetabolicReaction)(StringComparer.OrdinalIgnoreCase)
        Me.PathwayIndex = New Dictionary(Of String, Pathway)(StringComparer.OrdinalIgnoreCase)
        Me.SubstrateToReactions = New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.OrdinalIgnoreCase)
        Me.ProductToReactions = New Dictionary(Of String, List(Of MetabolicReaction))(StringComparer.OrdinalIgnoreCase)
        Me.ReactionNeighbours = New Dictionary(Of String, List(Of ReactionLink))(StringComparer.OrdinalIgnoreCase)

        Me.Pathways = BuildIndices(pathways)
    End Sub

    Private Function BuildIndices(pathways As IEnumerable(Of Pathway)) As Pathway()
        Dim list As New List(Of Pathway)

        If pathways IsNot Nothing Then
            For Each pathway As Pathway In pathways
                If pathway Is Nothing Then Continue For

                ' 同一编号的通路只登记一次，避免重复灌入同一条通路的反应
                Dim key As String = If(String.IsNullOrEmpty(pathway.ID), pathway.name, pathway.ID)
                If String.IsNullOrEmpty(key) Then Continue For
                If PathwayIndex.ContainsKey(key) Then Continue For

                Dim rxns As MetabolicReaction() = If(pathway.metabolicNetwork, New MetabolicReaction() {})
                Dim members As New List(Of MetabolicReaction)
                Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                For Each rxn As MetabolicReaction In rxns
                    If rxn Is Nothing OrElse String.IsNullOrEmpty(rxn.id) Then Continue For
                    If Not seen.Add(rxn.id) Then Continue For

                    members.Add(rxn)

                    If Not ReactionIndex.ContainsKey(rxn.id) Then
                        ReactionIndex(rxn.id) = rxn
                    End If

                    If Not ReactionToPathways.ContainsKey(rxn.id) Then
                        ReactionToPathways(rxn.id) = New List(Of Pathway)
                    End If
                    If Not ReactionToPathways(rxn.id).Any(Function(p) String.Equals(p.ID, key, StringComparison.OrdinalIgnoreCase)) Then
                        ReactionToPathways(rxn.id).Add(pathway)
                    End If

                    For Each ec As String In SafeECNumbers(rxn)
                        If Not ECtoReactions.ContainsKey(ec) Then
                            ECtoReactions(ec) = New List(Of MetabolicReaction)
                        End If
                        If Not ECtoReactions(ec).Any(Function(r) String.Equals(r.id, rxn.id, StringComparison.OrdinalIgnoreCase)) Then
                            ECtoReactions(ec).Add(rxn)
                        End If
                    Next

                    For Each substrate As String In SafeCompounds(rxn.left)
                        If Not SubstrateToReactions.ContainsKey(substrate) Then
                            SubstrateToReactions(substrate) = New List(Of MetabolicReaction)
                        End If
                        SubstrateToReactions(substrate).Add(rxn)
                    Next

                    For Each product As String In SafeCompounds(rxn.right)
                        If Not ProductToReactions.ContainsKey(product) Then
                            ProductToReactions(product) = New List(Of MetabolicReaction)
                        End If
                        ProductToReactions(product).Add(rxn)
                    Next
                Next

                PathwayIndex(key) = pathway
                PathwayReactions(key) = members
                list.Add(pathway)
            Next
        End If

        Call BuildReactionAdjacency()

        Return list.ToArray
    End Function

    ''' <summary>
    ''' 构建"产物即下游底物"的一步反应邻接索引。
    ''' </summary>
    Private Sub BuildReactionAdjacency()
        For Each consumer As KeyValuePair(Of String, List(Of MetabolicReaction)) In SubstrateToReactions
            For Each downstream As MetabolicReaction In consumer.Value
                Dim substrates As String() = SafeCompounds(downstream.left)

                For Each substrate As String In substrates
                    Dim producers As List(Of MetabolicReaction) = Nothing
                    If Not ProductToReactions.TryGetValue(substrate, producers) Then Continue For

                    For Each upstream As MetabolicReaction In producers
                        If String.Equals(upstream.id, downstream.id, StringComparison.OrdinalIgnoreCase) Then Continue For

                        Dim products As String() = SafeCompounds(upstream.right)
                        Dim shared As Integer = products.Intersect(substrates, StringComparer.OrdinalIgnoreCase).Count()
                        If shared = 0 Then Continue For

                        Dim denominator As Integer = Math.Max(products.Length, substrates.Length)
                        Dim coverage As Double = If(denominator = 0, 0, CDbl(shared) / denominator)

                        If Not ReactionNeighbours.ContainsKey(upstream.id) Then
                            ReactionNeighbours(upstream.id) = New List(Of ReactionLink)
                        End If

                        Dim links As List(Of ReactionLink) = ReactionNeighbours(upstream.id)
                        Dim exists As ReactionLink = links.FirstOrDefault(Function(l) String.Equals(l.ReactionID, downstream.id, StringComparison.OrdinalIgnoreCase))

                        If exists Is Nothing Then
                            links.Add(New ReactionLink With {
                                .ReactionID = downstream.id,
                                .Coverage = coverage
                            })
                        ElseIf coverage > exists.Coverage Then
                            exists.Coverage = coverage
                        End If
                    Next
                Next
            Next
        Next
    End Sub

    Friend Shared Function SafeECNumbers(rxn As MetabolicReaction) As String()
        If rxn Is Nothing OrElse rxn.ECNumbers Is Nothing Then Return New String() {}
        Return rxn.ECNumbers _
            .Where(Function(ec) Not String.IsNullOrEmpty(ec)) _
            .Select(Function(ec) ec.Trim) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToArray
    End Function

    Friend Shared Function SafeCompounds(species As IEnumerable(Of CompoundSpecieReference)) As String()
        If species Is Nothing Then Return New String() {}

        Return species _
            .Where(Function(item) item IsNot Nothing) _
            .Select(Function(item) item.ID) _
            .Where(Function(id) Not String.IsNullOrEmpty(id)) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToArray
    End Function

#Region "查询"

    ''' <summary>
    ''' 按 EC 编号查询反应，未命中时返回空集合（不会返回 Nothing）
    ''' </summary>
    Public Function GetReactionsByEC(ec As String) As List(Of MetabolicReaction)
        If String.IsNullOrEmpty(ec) Then Return New List(Of MetabolicReaction)()

        Dim result As List(Of MetabolicReaction) = Nothing
        If ECtoReactions.TryGetValue(ec.Trim, result) Then Return result

        Return New List(Of MetabolicReaction)()
    End Function

    ''' <summary>
    ''' 查询某个反应所属的全部通路
    ''' </summary>
    Public Function GetPathwayForReaction(reaction As MetabolicReaction) As IEnumerable(Of Pathway)
        If reaction Is Nothing Then Return New Pathway() {}
        Return GetPathwaysByReaction(reaction.id)
    End Function

    ''' <summary>
    ''' 按反应编号查询所属通路
    ''' </summary>
    Public Function GetPathwaysByReaction(reactionId As String) As IEnumerable(Of Pathway)
        Dim result As List(Of Pathway) = Nothing
        If String.IsNullOrEmpty(reactionId) Then Return New Pathway() {}
        If ReactionToPathways.TryGetValue(reactionId, result) Then Return result

        Return New Pathway() {}
    End Function

    ''' <summary>
    ''' 按通路编号查询通路对象
    ''' </summary>
    Public Function GetPathwayById(pathwayId As String) As Pathway
        Dim result As Pathway = Nothing
        If String.IsNullOrEmpty(pathwayId) Then Return Nothing
        If PathwayIndex.TryGetValue(pathwayId, result) Then Return result

        Return Nothing
    End Function

    ''' <summary>
    ''' 判断反应是否属于指定的通路
    ''' </summary>
    Public Function IsReactionInPathway(pathwayId As String, reactionId As String) As Boolean
        Dim members As List(Of MetabolicReaction) = Nothing
        If String.IsNullOrEmpty(pathwayId) OrElse String.IsNullOrEmpty(reactionId) Then Return False
        If Not PathwayReactions.TryGetValue(pathwayId, members) Then Return False

        Return members.Any(Function(r) String.Equals(r.id, reactionId, StringComparison.OrdinalIgnoreCase))
    End Function

    ''' <summary>
    ''' 查找覆盖了输入 EC 集合中"全部" EC 编号的通路。
    ''' 输入为空时不会返回任何通路。
    ''' </summary>
    Public Iterator Function FindCommonPathways(ecNumbers As IEnumerable(Of String)) As IEnumerable(Of Pathway)
        If ecNumbers Is Nothing Then Return

        Dim query As String() = ecNumbers _
            .Where(Function(ec) Not String.IsNullOrEmpty(ec)) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToArray

        If query.Length = 0 Then Return

        For Each pathway As Pathway In Pathways
            Dim members As List(Of MetabolicReaction) = Nothing
            Dim key As String = If(String.IsNullOrEmpty(pathway.ID), pathway.name, pathway.ID)

            If Not PathwayReactions.TryGetValue(key, members) Then Continue For

            Dim covered As Boolean = query.All(
                Function(ec)
                    Return members.Any(Function(rxn) SafeECNumbers(rxn).Contains(ec, StringComparer.OrdinalIgnoreCase))
                End Function)

            If covered Then Yield pathway
        Next
    End Function

    ''' <summary>
    ''' 在指定通路内部计算两个反应之间的最短间隔步数（基于"产物即下游底物"的有向邻接）。
    ''' 
    ''' 返回 <c>-1</c> 表示在 <paramref name="maxDepth"/> 步以内不可达。
    ''' </summary>
    Public Function GetReactionGap(pathwayId As String, fromId As String, toId As String, maxDepth As Integer) As Integer
        If String.IsNullOrEmpty(fromId) OrElse String.IsNullOrEmpty(toId) Then Return -1
        If String.Equals(fromId, toId, StringComparison.OrdinalIgnoreCase) Then Return 0
        If maxDepth <= 0 Then Return -1

        Dim members As List(Of MetabolicReaction) = Nothing
        If Not PathwayReactions.TryGetValue(pathwayId, members) Then Return -1

        Dim scope As New HashSet(Of String)(members.Select(Function(r) r.id), StringComparer.OrdinalIgnoreCase)
        If Not scope.Contains(fromId) OrElse Not scope.Contains(toId) Then Return -1

        Dim visited As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {fromId}
        Dim frontier As New List(Of String) From {fromId}

        For depth As Integer = 1 To maxDepth
            Dim next As New List(Of String)

            For Each current As String In frontier
                Dim links As List(Of ReactionLink) = Nothing
                If Not ReactionNeighbours.TryGetValue(current, links) Then Continue For

                For Each link As ReactionLink In links
                    If Not scope.Contains(link.ReactionID) Then Continue For
                    If Not visited.Add(link.ReactionID) Then Continue For

                    If String.Equals(link.ReactionID, toId, StringComparison.OrdinalIgnoreCase) Then Return depth

                    next.Add(link.ReactionID)
                Next
            Next

            If next.Count = 0 Then Return -1
            frontier = next
        Next

        Return -1
    End Function

#End Region

End Class
