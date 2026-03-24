Public Class GenomeAnalyzer

    ''' <summary>
    ''' 执行泛基因组分析的主函数
    ''' </summary>
    ''' <param name="orthologDict">包含三组比对结果的字典，Key为比对组名称(如"AvsB")</param>
    ''' <param name="allGenesGenome1">基因组1的所有基因ID列表（用于识别特异性基因）</param>
    ''' <param name="allGenesGenome2">基因组2的所有基因ID列表</param>
    ''' <param name="allGenesGenome3">基因组3的所有基因ID列表</param>
    ''' <returns>分析结果对象</returns>
    Public Function AnalyzePanGenome(orthologDict As Dictionary(Of String, Ortholog()),
                                     allGenesGenome1 As HashSet(Of String),
                                     allGenesGenome2 As HashSet(Of String),
                                     allGenesGenome3 As HashSet(Of String)) As PanGenomeResult

        ' 1. 初始化并查集
        Dim uf As New UnionFind()

        ' 将所有基因加入并查集（这一步确保特异性基因也被包含进来）
        ' 注意：实际项目中，如果基因量极大(几十万)，这里直接Add不影响性能，UnionFind内部会处理
        For Each gene In allGenesGenome1
            uf.AddElement(gene)
        Next
        For Each gene In allGenesGenome2
            uf.AddElement(gene)
        Next
        For Each gene In allGenesGenome3
            uf.AddElement(gene)
        Next

        ' 2. 处理直系同源表格，建立连接关系
        ' 遍历字典中的所有Ortholog数组
        For Each kvp In orthologDict
            Dim orthologs = kvp.Value
            If orthologs Is Nothing Then Continue For

            For Each ortho In orthologs
                ' 忽略无效数据
                If ortho Is Nothing OrElse String.IsNullOrEmpty(ortho.genome1) OrElse String.IsNullOrEmpty(ortho.genome2) Then
                    Continue For
                End If

                ' 核心逻辑：如果两个基因互为直系同源，则将它们在并查集中连接
                ' 这实现了基因家族的聚类
                uf.Union(ortho.genome1, ortho.genome2)
            Next
        Next

        ' 3. 提取基因家族
        ' Key: 家族根节点ID, Value: 该家族所有基因ID列表
        Dim familyMap As New Dictionary(Of String, List(Of String))()

        ' 我们需要遍历所有参与的基因来构建家族
        Dim allProcessedGenes = New HashSet(Of String)(allGenesGenome1)
        allProcessedGenes.UnionWith(allGenesGenome2)
        allProcessedGenes.UnionWith(allGenesGenome3)

        For Each gene In allProcessedGenes
            Dim root = uf.Find(gene)
            If Not familyMap.ContainsKey(root) Then
                familyMap.Add(root, New List(Of String)())
            End If
            familyMap(root).Add(gene)
        Next

        ' 4. 分类分析
        Dim result As New PanGenomeResult()
        Dim familyId As Integer = 0

        ' 辅助HashSet用于快速判断基因所属品种
        Dim g1Set = allGenesGenome1
        Dim g2Set = allGenesGenome2
        Dim g3Set = allGenesGenome3

        For Each family In familyMap
            familyId += 1
            Dim genes = family.Value ' 当前家族的所有基因
            result.GeneFamilies.Add(familyId, genes)

            ' 统计该家族在每个品种中的基因数量
            Dim countG1 = genes.Where(Function(g) g1Set.Contains(g)).Count()
            Dim countG2 = genes.Where(Function(g) g2Set.Contains(g)).Count()
            Dim countG3 = genes.Where(Function(g) g3Set.Contains(g)).Count()

            Dim presenceCount As Integer = 0
            If countG1 > 0 Then presenceCount += 1
            If countG2 > 0 Then presenceCount += 1
            If countG3 > 0 Then presenceCount += 1

            ' 分类逻辑
            If presenceCount = 3 Then
                ' 核心基因：三个品种都存在
                result.CoreGeneFamilies.Add(familyId)

                ' 检查是否为单拷贝直系同源：三个品种都存在，且每个品种仅1个拷贝
                If countG1 = 1 AndAlso countG2 = 1 AndAlso countG3 = 1 Then
                    result.SingleCopyOrthologFamilies.Add(familyId)
                End If

            ElseIf presenceCount = 1 Then
                ' 特异性基因：仅在一个品种中存在
                result.SpecificGeneFamilies.Add(familyId)
                ' 注意：特异性基因也是附属基因的一部分
                result.DispensableGeneFamilies.Add(familyId)

            Else ' presenceCount = 2
                ' 附属基因：在两个品种中存在
                result.DispensableGeneFamilies.Add(familyId)
            End If

        Next

        ' 填充统计信息
        result.TotalGenesInGenome1 = allGenesGenome1.Count
        result.TotalGenesInGenome2 = allGenesGenome2.Count
        result.TotalGenesInGenome3 = allGenesGenome3.Count

        Return result
    End Function

End Class
