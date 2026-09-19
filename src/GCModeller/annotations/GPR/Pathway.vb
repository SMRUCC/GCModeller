Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 参考通路：在 <see cref="MetabolicPathway"/> 的基础上额外维护一张反应网络图。
''' </summary>
Public Class Pathway : Inherits MetabolicPathway

    ''' <summary>
    ''' 反应网络。边的方向为 ``上游反应 -&gt; 下游反应``，即上游反应的产物是下游反应的底物。
    ''' </summary>
    Public ReadOnly Property ReactionNetwork As NetworkGraph

    Sub New(network As IReadOnlyCollection(Of MetabolicReaction))
        Me.metabolicNetwork = If(network Is Nothing, New MetabolicReaction() {}, network.ToArray)
        Me.ReactionNetwork = BuildReactionNetwork(Me.metabolicNetwork)
    End Sub

    ''' <summary>
    ''' 构建反应网络图。
    ''' 
    ''' 与直接两层遍历的做法相比，这里先建立"化合物 -&gt; 生产者反应"的倒排索引，
    ''' 把复杂度从 O(R²) 降到 O(ΣR · |left|)，同时对边做显式去重，
    ''' 避免 <see cref="NetworkGraph"/> 内部因为重复插入同一条边而抛出异常。
    ''' </summary>
    Private Shared Function BuildReactionNetwork(network As MetabolicReaction()) As NetworkGraph
        Dim g As New NetworkGraph
        Dim nodes As New Dictionary(Of String, Node)(StringComparer.OrdinalIgnoreCase)

        ' 1. 为每一个反应建立唯一节点
        For Each reaction As MetabolicReaction In network
            If reaction Is Nothing OrElse String.IsNullOrEmpty(reaction.id) Then Continue For
            If nodes.ContainsKey(reaction.id) Then Continue For

            Dim node As Node = g.GetElementByID(reaction.id)
            If node Is Nothing Then node = g.CreateNode(reaction.id)

            nodes(reaction.id) = node
        Next

        ' 2. 化合物 -> 生产者反应 倒排索引
        Dim producers As New Dictionary(Of String, List(Of String))(StringComparer.OrdinalIgnoreCase)

        For Each reaction As MetabolicReaction In network
            If reaction Is Nothing OrElse String.IsNullOrEmpty(reaction.id) Then Continue For

            For Each product As String In ContextIndices.SafeCompounds(reaction.right)
                If Not producers.ContainsKey(product) Then
                    producers(product) = New List(Of String)
                End If
                producers(product).Add(reaction.id)
            Next
        Next

        ' 3. 建边，并以 "上游 -> 下游" 作为去重键
        Dim linked As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each consumer As MetabolicReaction In network
            If consumer Is Nothing OrElse String.IsNullOrEmpty(consumer.id) Then Continue For

            For Each substrate As String In ContextIndices.SafeCompounds(consumer.left)
                Dim sources As List(Of String) = Nothing
                If Not producers.TryGetValue(substrate, sources) Then Continue For

                For Each producerId As String In sources
                    If String.Equals(producerId, consumer.id, StringComparison.OrdinalIgnoreCase) Then Continue For
                    If Not nodes.ContainsKey(producerId) Then Continue For

                    If Not linked.Add(producerId & vbLf & consumer.id) Then Continue For

                    Call g.CreateEdge(nodes(producerId), nodes(consumer.id))
                Next
            Next
        Next

        Return g
    End Function

    Public Shared Iterator Function FromKEGGPathways(pathways As IEnumerable(Of Map), reactions As IEnumerable(Of Reaction)) As IEnumerable(Of Pathway)
        Dim reactionIndex As Dictionary(Of String, MetabolicReaction) = reactions _
            .Where(Function(r)
                       ' 20260506 filter out the possible empty equation
                       Return r IsNot Nothing AndAlso r.ID <> "" AndAlso r.Equation <> ""
                   End Function) _
            .GroupBy(Function(r) r.ID) _
            .ToDictionary(Function(r) r.Key,
                          Function(r)
                              Return KEGGConvertor.ConvertReaction(r.First)
                          End Function)
        Dim bar As ProgressBar = Nothing

        Call "processing on build reference pathway map from kegg database...".info

        For Each map As Map In TqdmWrapper.WrapIterator(pathways, bar:=bar)
            Dim rxnIDs As String() = (From id As String
                                      In map.GetMembers
                                      Distinct
                                      Where reactionIndex.ContainsKey(id)).ToArray
            Dim network As MetabolicReaction() = rxnIDs.Select(Function(id) reactionIndex(id)).ToArray

            Call bar.SetLabel(map.name)

            Dim metabolites As MetabolicCompound() = map _
                .GetCompoundSet _
                .Select(Function(c)
                            Return New MetabolicCompound With {.id = c.Name, .name = c.Value}
                        End Function) _
                .ToArray

            Yield New Pathway(network) With {
                .ID = map.EntryId,
                .metabolicNetwork = network,
                .metabolites = metabolites,
                .name = map.name
            }
        Next
    End Function

End Class
