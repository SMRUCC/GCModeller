Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Namespace Kmers

    Public Class PriorProbabilityBuilder

        ''' <summary>
        ''' 分类学数据库。在实际应用中，您应该从NCBI taxdump文件中加载这些数据。
        ''' Key: Child TaxId, Value: Parent TaxId
        ''' </summary>
        ReadOnly TaxonomyDB As NcbiTaxonomyTree

        Sub New(TaxonomyDB As NcbiTaxonomyTree)
            Me.TaxonomyDB = TaxonomyDB
        End Sub

        Sub New(TaxonomyDB As String)
            Me.New(New NcbiTaxonomyTree(TaxonomyDB))
        End Sub

        ''' <summary>
        ''' 根据子分类单元ID和目标父级等级，查找其父级分类单元ID。
        ''' 此函数会沿着分类树向上遍历，直到找到指定等级的节点。
        ''' </summary>
        ''' <param name="childTaxId">子分类单元的TaxId（如物种）。</param>
        ''' <param name="targetRank">目标父级等级（如 "genus", "family", "order"）。</param>
        ''' <returns>父级分类单元的TaxId，如果找不到则返回0。</returns>
        Public Function GetParentTaxId(childTaxId As Integer, targetRank As String) As Integer
            Dim currentNode As TaxonomyNode = TaxonomyDB.GetNode(childTaxId)
            If currentNode Is Nothing Then Return 0

            ' 从当前节点开始，沿着分类树向上查找
            While currentNode IsNot Nothing
                If String.Equals(currentNode.rank, targetRank, StringComparison.OrdinalIgnoreCase) Then
                    Return currentNode.taxid
                End If
                currentNode = TaxonomyDB.GetParent(currentNode.taxid)
            End While

            ' 如果遍历到根节点仍未找到，则返回0
            Return 0
        End Function

        ''' <summary>
        ''' 构建物种的先验概率数据库，该数据库基于指定的分类层级。
        ''' 计算公式：p(s | G) = (物种 s 的k-mer总数) / (其所属的目标分类单元 G 的k-mer总数)
        ''' </summary>
        ''' <param name="kmerDatabase">完整的k-mer数据库，即KmerSeed数组。</param>
        ''' <param name="sequenceLookup">一个全局查找表，用于通过seqid获取SequenceSource信息。</param>
        ''' <param name="targetRank">用于计算先验概率的目标分类层级，例如 "genus", "family", "order"。</param>
        ''' <returns>一个字典，Key是物种的ncbi_taxid，Value是该物种相对于目标层级的先验概率。</returns>
        Public Function BuildPriorDatabase(
        kmerDatabase As IEnumerable(Of KmerSeed),
        sequenceLookup As SequenceCollection,
        targetRank As String
    ) As Dictionary(Of Integer, Double)

            ' --- 步骤 1: 初始化计数器 ---
            ' 存储每个物种的k-mer总数
            Dim speciesKmerCounts As New Dictionary(Of Integer, ULong)()
            ' 存储每个目标分类单元（如属、科）的k-mer总数
            Dim parentGroupKmerCounts As New Dictionary(Of Integer, ULong)()
            ' 缓存物种到其目标父级分类单元的映射关系，避免重复查询
            Dim speciesToParentGroupMap As New Dictionary(Of Integer, Integer)()

            Console.WriteLine($"开始遍历k-mer数据库以统计k-mer数量... (目标层级: {targetRank})")

            ' --- 步骤 2: 遍历数据库，统计物种的k-mer数量 ---
            For Each seed As KmerSeed In kmerDatabase
                For Each src As KmerSource In seed.source
                    If sequenceLookup(src.seqid) IsNot Nothing Then
                        Dim seqSrc As SequenceSource = sequenceLookup(src.seqid)
                        Dim speciesTaxId As Integer = seqSrc.ncbi_taxid
                        speciesKmerCounts(speciesTaxId) = speciesKmerCounts.TryGetValue(speciesTaxId, default:=0UL) + CULng(src.count)
                    End If
                Next
            Next

            Console.WriteLine("物种k-mer统计完成，开始建立物种与目标分类单元的映射关系...")

            ' --- 步骤 3: 建立每个物种到其目标父级分类单元的映射 ---
            For Each speciesTaxId In speciesKmerCounts.Keys
                Dim parentGroupTaxId As Integer = GetParentTaxId(speciesTaxId, targetRank)
                If parentGroupTaxId > 0 Then
                    speciesToParentGroupMap(speciesTaxId) = parentGroupTaxId
                End If
            Next

            Console.WriteLine("映射关系建立完成，开始计算每个目标分类单元的k-mer总数...")

            ' --- 步骤 4: 根据物种k-mer数量，计算每个目标分类单元的k-mer总数 ---
            For Each kvp In speciesToParentGroupMap
                Dim speciesTaxId As Integer = kvp.Key
                Dim parentGroupTaxId As Integer = kvp.Value

                Dim kmerCountForSpecies As ULong = speciesKmerCounts.TryGetValue(speciesTaxId, default:=0UL)
                parentGroupKmerCounts(parentGroupTaxId) = parentGroupKmerCounts.TryGetValue(parentGroupTaxId, default:=0UL) + kmerCountForSpecies
            Next

            Console.WriteLine("目标分类单元总数计算完成，开始计算先验概率...")

            ' --- 步骤 5: 计算每个物种的先验概率 p(s | G) ---
            Dim priors As New Dictionary(Of Integer, Double)()
            For Each kvp In speciesToParentGroupMap
                Dim speciesTaxId As Integer = kvp.Key
                Dim parentGroupTaxId As Integer = kvp.Value

                Dim speciesCount As ULong = speciesKmerCounts.TryGetValue(speciesTaxId, default:=0UL)
                Dim parentGroupCount As ULong = parentGroupKmerCounts.TryGetValue(parentGroupTaxId, default:=0UL)

                ' 避免除以零的错误
                If parentGroupCount > 0UL Then
                    priors(speciesTaxId) = CDbl(speciesCount) / CDbl(parentGroupCount)
                Else
                    ' 如果一个分类单元下没有物种（理论上不应发生），则概率为0
                    priors(speciesTaxId) = 0.0
                End If
            Next

            Console.WriteLine($"针对 '{targetRank}' 层级的先验概率数据库构建完成，共包含 {priors.Count} 个物种的概率数据。")
            Return priors
        End Function

    End Class
End Namespace