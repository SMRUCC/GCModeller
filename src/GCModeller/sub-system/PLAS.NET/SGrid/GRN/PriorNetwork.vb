Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn
Imports RegulationEdge = SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge

Namespace GRN

    ''' <summary>
    ''' The prior knowledge network which is used for constraining the regulatory
    ''' network structure inference.
    ''' (用于约束调控网络结构拟合的先验知识网络)
    ''' </summary>
    ''' <remarks>
    ''' 先验知识在本算法之中是以「软约束」的方式使用的：
    ''' 1. 拟合阶段对先验不支持的交互施加收缩惩罚（<see cref="FittingOptions.PriorPenalty"/>）
    ''' 2. 对与先验方向冲突的交互施加符号惩罚（<see cref="FittingOptions.PriorSignPenalty"/>）
    ''' 3. 稀疏化阶段对先验支持的边给予排序加权（<see cref="FittingOptions.PriorRankBoost"/>）
    ''' 
    ''' 注意：先验知识之中的基因符号与拟合所使用的规整化编号可能并不一致，
    ''' 在进入拟合过程之前需要先调用 <see cref="Remap"/> 做符号映射。
    ''' </remarks>
    Public Class PriorNetwork

        ''' <summary>
        ''' 全部的先验调控边
        ''' </summary>
        ''' <returns></returns>
        Public Property Edges As RegulationEdge()

        ''' <summary>
        ''' 靶基因（大写符号）到其调控因子记录的索引
        ''' </summary>
        Private ReadOnly byTarget As New Dictionary(Of String, List(Of RegulationEdge))(StringComparer.OrdinalIgnoreCase)
        ''' <summary>
        ''' 源到靶的边索引，键为 <c>TF &amp; vbTab &amp; target</c>
        ''' </summary>
        Private ReadOnly byPair As New Dictionary(Of String, RegulationEdge)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>
        ''' 先验边的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            Get
                If Edges Is Nothing Then Return 0 Else Return Edges.Length
            End Get
        End Property

        ''' <summary>
        ''' 先验网络之中所出现的全部基因符号（源与靶的并集）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GeneSymbols As String()
            Get
                Dim genes As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                For Each edge As RegulationEdge In Edges.SafeQuery
                    If Not String.IsNullOrEmpty(edge.TF) Then
                        Call genes.Add(edge.TF)
                    End If
                    If Not String.IsNullOrEmpty(edge.TargetGene) Then
                        Call genes.Add(edge.TargetGene)
                    End If
                Next

                Return genes.ToArray
            End Get
        End Property

        ''' <summary>
        ''' 创建一个空的先验网络
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Empty() As PriorNetwork
            Return New PriorNetwork With {.Edges = New RegulationEdge() {}}
        End Function

        ''' <summary>
        ''' 从给定的边集合创建先验网络对象
        ''' </summary>
        ''' <param name="records"></param>
        ''' <returns></returns>
        Public Shared Function FromEdges(records As IEnumerable(Of RegulationEdge)) As PriorNetwork
            Dim network As New PriorNetwork With {.Edges = records.SafeQuery().ToArray}

            Call network.BuildIndex()

            Return network
        End Function

        ''' <summary>
        ''' 重建内部的查找索引
        ''' </summary>
        Private Sub BuildIndex()
            Call byTarget.Clear()
            Call byPair.Clear()

            Dim merged As New Dictionary(Of String, RegulationEdge)(StringComparer.OrdinalIgnoreCase)

            For Each edge As RegulationEdge In Edges.SafeQuery
                Dim key As String = pairKey(edge.TF, edge.TargetGene)
                Dim exists As RegulationEdge = Nothing

                If merged.TryGetValue(key, exists) Then
                    ' 重复的记录保留置信度更高的一条（置信度相同时保留先出现的记录）
                    If edge.Confidence > exists.Confidence Then
                        merged(key) = edge
                    End If
                Else
                    merged.Add(key, edge)
                End If
            Next

            Edges = merged.Values.ToArray

            For Each edge As RegulationEdge In Edges
                Dim key As String = pairKey(edge.TF, edge.TargetGene)
                Dim list As List(Of RegulationEdge) = Nothing

                Call byPair.Add(key, edge)

                If Not byTarget.TryGetValue(edge.TargetGene, list) Then
                    list = New List(Of RegulationEdge)
                    byTarget.Add(edge.TargetGene, list)
                End If

                Call list.Add(edge)
            Next
        End Sub

        ''' <summary>
        ''' 查询目标基因的全部已知调控因子
        ''' </summary>
        ''' <param name="target">靶基因的符号</param>
        ''' <returns></returns>
        Public Function RegulatorsOf(target As String) As RegulationEdge()
            If String.IsNullOrEmpty(target) Then
                Return New RegulationEdge() {}
            End If

            Dim list As List(Of RegulationEdge) = Nothing

            If byTarget.TryGetValue(target, list) Then
                Return list.ToArray
            Else
                Return New RegulationEdge() {}
            End If
        End Function

        ''' <summary>
        ''' 查询指定的调控关系；不存在的时候返回空值
        ''' </summary>
        ''' <param name="tf"></param>
        ''' <param name="target"></param>
        ''' <returns></returns>
        Public Function Lookup(tf As String, target As String) As RegulationEdge
            If String.IsNullOrEmpty(tf) OrElse String.IsNullOrEmpty(target) Then
                Return Nothing
            End If

            Dim edge As RegulationEdge = Nothing

            If byPair.TryGetValue(pairKey(tf, target), edge) Then
                Return edge
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 判断指定的调控关系是否有先验知识的支持
        ''' </summary>
        ''' <param name="tf"></param>
        ''' <param name="target"></param>
        ''' <returns></returns>
        Public Function Supports(tf As String, target As String) As Boolean
            Return Not Lookup(tf, target) Is Nothing
        End Function

        ''' <summary>
        ''' 查询指定的调控关系在先验知识之中的方向符号（+1/-1/0）
        ''' </summary>
        ''' <param name="tf"></param>
        ''' <param name="target"></param>
        ''' <returns></returns>
        Public Function SignOf(tf As String, target As String) As Integer
            Dim edge As RegulationEdge = Lookup(tf, target)

            If edge Is Nothing Then
                Return 0
            Else
                Return If(edge.RegulationType = Effector.Activator, 1, -1)
            End If
        End Function

        ''' <summary>
        ''' 只保留两端都落在给定基因集合之中的先验边
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <returns></returns>
        Public Function Subset(genes As IEnumerable(Of String)) As PriorNetwork
            Dim index As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each gene As String In genes.SafeQuery
                If Not String.IsNullOrEmpty(gene) Then
                    Call index.Add(gene)
                End If
            Next

            Return FromEdges(Edges _
                .SafeQuery() _
                .Where(Function(e) index.Contains(e.TF) AndAlso index.Contains(e.TargetGene)))
        End Function

        ''' <summary>
        ''' 对先验网络之中的基因符号做映射（例如映射到拟合所使用的规整化编号）
        ''' </summary>
        ''' <param name="nameMap">原始基因符号到目标基因符号的映射表（大小写不敏感）</param>
        ''' <returns>映射之后的新对象；映射不到的先验边会被丢弃</returns>
        Public Function Remap(nameMap As IDictionary(Of String, String)) As PriorNetwork
            If nameMap Is Nothing OrElse nameMap.Count = 0 Then
                Return FromEdges(Edges.SafeQuery())
            End If

            Dim records As New List(Of RegulationEdge)
            Dim source As String = Nothing
            Dim target As String = Nothing

            For Each edge As RegulationEdge In Edges.SafeQuery
                If Not nameMap.TryGetValue(edge.TF, source) Then
                    Continue For
                End If
                If Not nameMap.TryGetValue(edge.TargetGene, target) Then
                    Continue For
                End If

                Call records.Add(New RegulationEdge With {
                    .TF = source,
                    .TargetGene = target,
                    .RegulationType = edge.RegulationType,
                    .Confidence = edge.Confidence,
                    .Evidence = edge.Evidence
                })
            Next

            Return FromEdges(records)
        End Function

        ''' <summary>
        ''' 深拷贝
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As PriorNetwork
            Return FromEdges(Edges.SafeQuery().Select(Function(e) New RegulationEdge(e)))
        End Function

        ''' <summary>
        ''' 按调控方向统计边的数量
        ''' </summary>
        ''' <returns></returns>
        Public Function TypeStatistic() As String
            Dim act As Integer = Aggregate e As RegulationEdge In Edges.SafeQuery Where e.RegulationType = Effector.Activator Into Count()
            Dim inh As Integer = Aggregate e As RegulationEdge In Edges.SafeQuery Where e.RegulationType = Effector.Inhibitor Into Count()
            Dim unk As Integer = Size - act - inh

            Return $"activator={act}, inhibitor={inh}, unknown={unk}"
        End Function

        Public Overrides Function ToString() As String
            Return $"prior network: {Size} edge(s), {GeneSymbols.Length} gene symbol(s)"
        End Function

        Private Shared Function pairKey(tf As String, target As String) As String
            Return tf & vbTab & target
        End Function

    End Class
End Namespace
