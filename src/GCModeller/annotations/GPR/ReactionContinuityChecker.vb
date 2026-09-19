Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 反应连续性检查。
''' 
''' 对于基因当前"已经支持"的反应集合，统计其中存在的"产物即下游底物"关系，
''' 并据此对相关反应给出增益证据。
''' 
''' 注意：本类只对基因已经支持的反应给出增益，不会凭空创造新的关联，
''' 因此它属于网络级的一致性证据，而不是猜测性证据。
''' </summary>
Public Class ReactionContinuityChecker

    ''' <summary>
    ''' 反应编号 -&gt; 反应对象
    ''' </summary>
    ReadOnly reactionIndex As Dictionary(Of String, MetabolicReaction)

    Public Sub New(reactionIndex As Dictionary(Of String, MetabolicReaction))
        Me.reactionIndex = If(reactionIndex, New Dictionary(Of String, MetabolicReaction)(StringComparer.OrdinalIgnoreCase))
    End Sub

    ''' <summary>
    ''' 已经登记索引的反应数量
    ''' </summary>
    Public ReadOnly Property Size As Integer
        Get
            Return reactionIndex.Count
        End Get
    End Property

    ''' <summary>
    ''' 收集连续性证据。
    ''' 
    ''' 修复：原实现从未被任何调用方使用（死代码），且分数更新语句的语义混乱；
    ''' 现在统一改造为"输入已支持反应集合、输出增益证据"的纯函数形式。
    ''' </summary>
    Public Function CollectEvidence(supported As IEnumerable(Of String),
                                    context As ContextIndices,
                                    opt As GPRParameters) As IEnumerable(Of ReactionEvidence)

        Dim results As New List(Of ReactionEvidence)

        If supported Is Nothing OrElse context Is Nothing Then Return results
        If opt Is Nothing Then opt = New GPRParameters

        Dim scope As New HashSet(Of String)(
            supported.Where(Function(id) Not String.IsNullOrEmpty(id)),
            StringComparer.OrdinalIgnoreCase)

        If scope.Count < 2 Then Return results

        For Each upstream As String In scope
            Dim links As List(Of ReactionLink) = Nothing
            If Not context.ReactionNeighbours.TryGetValue(upstream, links) Then Continue For

            For Each link As ReactionLink In links
                If Not scope.Contains(link.ReactionID) Then Continue For
                If Not reactionIndex.ContainsKey(upstream) Then Continue For
                If Not reactionIndex.ContainsKey(link.ReactionID) Then Continue For

                Dim source As String = $"{upstream} -> {link.ReactionID}"

                results.Add(New ReactionEvidence(upstream, New AssociationEvidence With {
                    .Kind = EvidenceKind.ReactionContinuity,
                    .Weight = opt.ReactionContinuityWeight,
                    .RawScore = link.Coverage,
                    .Source = source
                }))
                results.Add(New ReactionEvidence(link.ReactionID, New AssociationEvidence With {
                    .Kind = EvidenceKind.ReactionContinuity,
                    .Weight = opt.ReactionContinuityWeight,
                    .RawScore = link.Coverage,
                    .Source = source
                }))
            Next
        Next

        Return results
    End Function

    ''' <summary>
    ''' 从参考网络上下文构建索引。
    ''' 
    ''' 修复：改为索引 <see cref="ContextIndices.ReactionIndex"/> 中的"全部"反应，
    ''' 原实现只索引带 EC 编号的反应，导致没有 EC 注释的反应永远无法参与连续性推断。
    ''' </summary>
    Public Shared Function LoadFromContext(context As ContextIndices) As ReactionContinuityChecker
        Dim index As New Dictionary(Of String, MetabolicReaction)(StringComparer.OrdinalIgnoreCase)

        If context IsNot Nothing Then
            For Each item As KeyValuePair(Of String, MetabolicReaction) In context.ReactionIndex
                index(item.Key) = item.Value
            Next
        End If

        Return New ReactionContinuityChecker(index)
    End Function

End Class
