Public Class GenomeAnalyzer

    ''' <summary>
    ''' 执行泛基因组分析的主函数（支持任意多个基因组）
    ''' </summary>
    ''' <param name="orthologDict">包含比对结果的字典，Key为比对组名称(如"Genome1vsGenome2")</param>
    ''' <param name="genomeGenes">所有基因组及其基因的字典，Key为基因组名称，Value为该基因组所有基因ID列表</param>
    ''' <returns>分析结果对象</returns>
    Public Function AnalyzePanGenome(orthologDict As Dictionary(Of String, Ortholog()),
                                     genomeGenes As Dictionary(Of String, HashSet(Of String))) As PanGenomeResult

        ' 1. 初始化并查集
        Dim uf As New UnionFind()

        ' 将所有基因加入并查集（确保特异性基因也被包含进来）
        For Each genomeKvp In genomeGenes
            For Each gene In genomeKvp.Value
                uf.AddElement(gene)
            Next
        Next

        ' 2. 处理直系同源表格，建立连接关系
        For Each kvp In orthologDict
            Dim orthologs = kvp.Value
            If orthologs Is Nothing Then Continue For

            For Each ortho In orthologs
                ' 忽略无效数据
                If ortho Is Nothing OrElse String.IsNullOrEmpty(ortho.gene1) OrElse String.IsNullOrEmpty(ortho.gene2) Then
                    Continue For
                End If

                ' 核心逻辑：如果两个基因互为直系同源，则将它们在并查集中连接
                uf.Union(ortho.gene1, ortho.gene2)
            Next
        Next

        ' 3. 提取基因家族
        ' Key: 家族根节点ID, Value: 该家族所有基因ID列表
        Dim familyMap As New Dictionary(Of String, List(Of String))()

        ' 遍历所有参与的基因来构建家族
        Dim allProcessedGenes As New HashSet(Of String)()
        For Each genomeKvp In genomeGenes
            allProcessedGenes.UnionWith(genomeKvp.Value)
        Next

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
        Dim genomeNames = genomeGenes.Keys.ToList()
        Dim totalGenomes As Integer = genomeNames.Count

        ' 预先构建基因组基因集合的字典，用于快速查找
        Dim genomeGeneSets = genomeGenes

        For Each family In familyMap
            familyId += 1
            Dim genes = family.Value ' 当前家族的所有基因
            result.GeneFamilies.Add(familyId, genes)

            ' 统计该家族在每个品种中的基因数量
            Dim genomeGeneCounts As New Dictionary(Of String, Integer)()
            Dim presenceCount As Integer = 0

            For Each genomeName In genomeNames
                Dim count = genes.Where(Function(g) genomeGeneSets(genomeName).Contains(g)).Count()
                genomeGeneCounts(genomeName) = count
                If count > 0 Then presenceCount += 1
            Next

            ' 分类逻辑
            If presenceCount = totalGenomes Then
                ' 核心基因：所有品种都存在
                result.CoreGeneFamilies.Add(familyId)

                ' 检查是否为单拷贝直系同源：所有品种都存在，且每个品种仅1个拷贝
                Dim isSingleCopy As Boolean = True
                For Each genomeName In genomeNames
                    If genomeGeneCounts(genomeName) <> 1 Then
                        isSingleCopy = False
                        Exit For
                    End If
                Next

                If isSingleCopy Then
                    result.SingleCopyOrthologFamilies.Add(familyId)
                End If

            ElseIf presenceCount = 1 Then
                ' 特异性基因：仅在一个品种中存在
                result.SpecificGeneFamilies.Add(familyId)
                ' 注意：特异性基因也是附属基因的一部分
                result.DispensableGeneFamilies.Add(familyId)

            Else ' presenceCount > 1 AndAlso presenceCount < totalGenomes
                ' 附属基因：在部分品种中存在（但不是全部）
                result.DispensableGeneFamilies.Add(familyId)
            End If

        Next

        ' 填充统计信息
        For Each genomeKvp In genomeGenes
            result.TotalGenesInGenomes.Add(genomeKvp.Key, genomeKvp.Value.Count)
        Next

        Return result
    End Function

End Class