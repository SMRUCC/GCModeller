Namespace Kmers

    Public Class PriorProbabilityBuilder
        ''' <summary>
        ''' 模拟的分类学数据库。在实际应用中，您应该从NCBI taxdump文件中加载这些数据。
        ''' Key: Child TaxId, Value: Parent TaxId
        ''' </summary>
        Public MockTaxonomyDB As New Dictionary(Of Integer, Integer) From {
        {12345, 567}, ' 物种12345的父级是属567
        {12346, 567}, ' 物种12346的父级是属567
        {12347, 568}, ' 物种12347的父级是属568
        {567, 99},    ' 属567的父级是科99
        {568, 99}     ' 属568的父级是科99
    }

        ''' <summary>
        ''' 根据子分类单元ID和目标父级等级，查找其父级分类单元ID。
        ''' 这是一个简化的模拟函数，真实环境中需要遍历完整的分类树。
        ''' </summary>
        ''' <param name="childTaxId">子分类单元的TaxId（如物种）。</param>
        ''' <param name="parentRank">目标父级等级（如 "genus"）。</param>
        ''' <returns>父级分类单元的TaxId，如果找不到则返回0。</returns>
        Public Function GetParentTaxId(childTaxId As Integer, parentRank As String) As Integer
            ' 在真实系统中，您需要知道每个TaxId对应的rank（species, genus, family等）
            ' 这里我们简化处理，假设我们知道属的ID
            If MockTaxonomyDB.ContainsKey(childTaxId) Then
                Return MockTaxonomyDB(childTaxId)
            End If
            Return 0
        End Function

        ''' <summary>
        ''' 构建物种的先验概率数据库。
        ''' </summary>
        ''' <param name="kmerDatabase">完整的k-mer数据库，即KmerSeed数组。</param>
        ''' <param name="sequenceLookup">一个全局查找表，用于通过seqid获取SequenceSource信息。</param>
        ''' <returns>一个字典，Key是物种的ncbi_taxid，Value是该物种的先验概率 p(s)。</returns>
        Public Function BuildPriorDatabase(
        kmerDatabase As KmerSeed(),
        sequenceLookup As Dictionary(Of UInteger, SequenceSource)
    ) As Dictionary(Of Integer, Double)

            ' --- 步骤 1: 初始化计数器 ---
            ' 存储每个物种的k-mer总数
            Dim speciesKmerCounts As New Dictionary(Of Integer, ULong)()
            ' 存储每个属的k-mer总数
            Dim genusKmerCounts As New Dictionary(Of Integer, ULong)()
            ' 缓存物种到属的映射关系，避免重复查询
            Dim speciesToGenusMap As New Dictionary(Of Integer, Integer)()

            Console.WriteLine("开始遍历k-mer数据库以统计k-mer数量...")

            ' --- 步骤 2: 遍历数据库，统计物种和属的k-mer数量 ---
            For Each seed As KmerSeed In kmerDatabase
                For Each src As KmerSource In seed.source
                    ' 通过seqid找到对应的SequenceSource
                    If sequenceLookup.TryGetValue(src.seqid, Nothing) Then
                        Dim seqSrc As SequenceSource = sequenceLookup(src.seqid)
                        Dim speciesTaxId As Integer = seqSrc.ncbi_taxid

                        ' 累加该物种的k-mer数量
                        ' 注意：src.count表示该k-mer在这个序列中出现的次数
                        speciesKmerCounts(speciesTaxId) = speciesKmerCounts.TryGetValue(speciesTaxId, [default]:=0UL) + CULng(src.count)

                        ' 如果还未确定该物种所属的属，则查找并缓存
                        If Not speciesToGenusMap.ContainsKey(speciesTaxId) Then
                            Dim genusTaxId As Integer = GetParentTaxId(speciesTaxId, "genus")
                            If genusTaxId > 0 Then
                                speciesToGenusMap(speciesTaxId) = genusTaxId
                            End If
                        End If
                    End If
                Next
            Next

            Console.WriteLine("k-mer统计完成，开始计算属的总数...")

            ' --- 步骤 3: 根据物种k-mer数量，计算每个属的k-mer总数 ---
            For Each kvp In speciesToGenusMap
                Dim speciesTaxId As Integer = kvp.Key
                Dim genusTaxId As Integer = kvp.Value

                Dim kmerCountForSpecies As ULong = speciesKmerCounts.TryGetValue(speciesTaxId, default:=0UL)
                genusKmerCounts(genusTaxId) = genusKmerCounts.TryGetValue(genusTaxId, default:=0UL) + kmerCountForSpecies
            Next

            Console.WriteLine("属总数计算完成，开始计算先验概率...")

            ' --- 步骤 4: 计算每个物种的先验概率 p(s) ---
            Dim priors As New Dictionary(Of Integer, Double)()
            For Each kvp In speciesToGenusMap
                Dim speciesTaxId As Integer = kvp.Key
                Dim genusTaxId As Integer = kvp.Value

                Dim speciesCount As ULong = speciesKmerCounts.TryGetValue(speciesTaxId, default:=0UL)
                Dim genusCount As ULong = genusKmerCounts.TryGetValue(genusTaxId, default:=0UL)

                ' 避免除以零的错误
                If genusCount > 0UL Then
                    priors(speciesTaxId) = CDbl(speciesCount) / CDbl(genusCount)
                Else
                    ' 如果一个属下没有物种（理论上不应发生），则概率为0
                    priors(speciesTaxId) = 0.0
                End If
            Next

            Console.WriteLine($"先验概率数据库构建完成，共包含 {priors.Count} 个物种的概率数据。")
            Return priors
        End Function

    End Class
End Namespace