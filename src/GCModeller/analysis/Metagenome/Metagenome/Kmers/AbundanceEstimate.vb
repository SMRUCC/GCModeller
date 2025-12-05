Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Namespace Kmers

    Public Class AbundanceEstimate

        ReadOnly taxonomyDB As NcbiTaxonomyTree

#Region "kmer database background/information"
        ''' <summary>
        ''' 全局先验概率：Key是物种的ncbi_taxid, Value是该物种的先验概率
        ''' </summary>
        Dim priors As Dictionary(Of Integer, Double)
        ''' <summary>
        ''' 全局k-mer分布：Key是k-mer字符串, Value是一个字典
        ''' 内层字典的Key是物种的ncbi_taxid, Value是该k-mer在该物种中的概率
        ''' </summary>
        Dim KmerDistributions As New Dictionary(Of String, Dictionary(Of Integer, Double))
        Dim sequenceLookup As SequenceCollection
#End Region

        Sub New(taxonomyDB As NcbiTaxonomyTree)
            Me.taxonomyDB = taxonomyDB
        End Sub

        Sub New(taxonomyDB As String)
            Call Me.New(New NcbiTaxonomyTree(taxonomyDB))
        End Sub

        Public Function SetBackground(bg As KmerBackground) As AbundanceEstimate
            priors = bg.Prior
            KmerDistributions = bg.KmerDistributions
            Return Me
        End Function

        Public Function SetSequenceDb(seqs As SequenceCollection) As AbundanceEstimate
            sequenceLookup = seqs
            Return Me
        End Function

        ''' <summary>
        ''' 阶段一：从单条Read的KmerSeed数组中提取原始分类信息
        ''' </summary>
        ''' <param name="kmerSeedsForRead"></param>
        ''' <returns></returns>
        Public Function GetRawTaxonomyCounts(kmerSeedsForRead As KmerSeed()) As Dictionary(Of Integer, Integer)
            Dim taxonomyCounts As New Dictionary(Of Integer, Integer)()

            For Each seed As KmerSeed In kmerSeedsForRead
                ' 一个k-mer可能匹配多个来源，我们按权重分配
                ' weight = 1 / source.Length，表示每个来源分得1/N的票
                Dim voteWeight As Double = seed.weight

                For Each src As KmerSource In seed.source
                    ' 通过seqid找到对应的SequenceSource，从而获得ncbi_taxid
                    If sequenceLookup.HasSequence(src.seqid) Then
                        Dim taxId As Integer = sequenceLookup(src.seqid).ncbi_taxid

                        If taxonomyCounts.ContainsKey(taxId) Then
                            taxonomyCounts(taxId) += CInt(Math.Ceiling(voteWeight))
                        Else
                            taxonomyCounts(taxId) = CInt(Math.Ceiling(voteWeight))
                        End If
                    End If
                Next
            Next

            Return taxonomyCounts
        End Function

        Private Iterator Function GetSpeciesInGenus(genusC_taxid As Integer) As IEnumerable(Of Integer)
            Dim node As TaxonomyNode = taxonomyDB(genusC_taxid)

            If node.rank = "species" Then
                Yield node.taxid
                Return
            End If

            If node.HasChilds Then
                For Each child_id As String In node.children
                    For Each id As Integer In GetSpeciesInGenus(child_id)
                        Yield id
                    Next
                Next
            End If
        End Function

        Private Function IsDescendantOf(taxId As Integer, genusC_taxid As Integer) As Boolean
            Dim lineage As TaxonomyNode() = taxonomyDB.GetAscendantsWithRanksAndNames(taxId)

            For Each parent As TaxonomyNode In lineage
                If parent.taxid = genusC_taxid Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="kmerOfRead">这个参数代表了**一条宏基因组测序Read**在k-mer数据库中的所有匹配结果。
        ''' 当处理一条测序Read时，我们会将其分解成多个k-mer，然后用这些k-mer去查询kmer数据库。
        ''' 对于每一个成功匹配的k-mer，数据库都会返回一个对应的 `KmerSeed` 对象。
        ''' 这个 `KmerSeed` 对象包含了匹配上的k-mer序列（`kmer` 属性）以及这个k-mer在所有参考基因组中的来源信息（`source` 属性，是一个 `KmerSource` 数组）。
        ''' 因此，`kmerOfRead` 这个数组**完整地描述了这条Read的组成信息**，即它是由哪些k-mer构成的，以及这些k-mer都可能来源于哪些物种。
        ''' 
        ''' 这是进行贝叶斯重估的**原始数据**。
        ''' 函数会遍历这个数组，统计Read中的k-mer“投票”给了哪些分类单元（物种、属等）。
        ''' 这些投票信息是计算贝叶斯公式中“似然”（Likelihood）部分的基础。
        ''' 
        ''' 可以看作是**一张完整的选票**。上面记录了选民（这条Read）的每一部分（每个k-mer）都投给了哪些候选人（物种）。
        ''' </param>
        ''' <param name="ncbi_taxid">这个参数代表想要进行丰度重估的**目标分类单元的NCBI Taxonomy ID**。
        ''' 
        ''' 在宏基因组分析中，很多Read无法被唯一地分类到物种级别，因为它们的k-mer可能同时存在于同一个属下的多个物种中。
        ''' 这个参数允许我们指定一个**分类层级**（通常是一个属，但也可以是科、目等），告诉函数：“请帮我重新估算这个分类单元下所有物种的丰度”。
        ''' 例如，如果大肠杆菌（`taxid: 562`）和沙门氏菌（`taxid: 590`）都属于埃希氏菌-志贺氏菌属（`taxid: 561`），
        ''' 当我们传入 `genusC_taxid = 561` 时，函数就会尝试将那些只被分类到属（561）而无法区分到种（562或590）的Read，根据贝叶斯模型重新分配给这两个物种。
        ''' 
        ''' **定义了贝叶斯重估的范围**。函数只会关注那些与这个 `genusC_taxid` 相关的Read和物种。
        ''' 它是连接“原始分类结果”和“贝叶斯模型”的桥梁。函数会查找所有被分类到这个 `genusC_taxid`（或其下级物种）的k-mer，然后将它们作为待重估的数据集 `D`。
        ''' 它也用于从全局的先验概率数据库中，提取出该属下所有物种的先验概率 `p(s)`。
        ''' 
        ''' 可以看作是**一个特定的选区**（例如“物理学奖”）。基于此以重新计算这个选区内各位候选人（物理学家）的真实得票率。
        ''' </param>
        ''' <returns></returns>
        Public Function EstimateAbundanceForGenus(kmerOfRead As KmerSeed(), ncbi_taxid As Integer) As Dictionary(Of Integer, Double)
            ' 1. 获取原始分类计数
            Dim rawCounts As Dictionary(Of Integer, Integer) = GetRawTaxonomyCounts(kmerOfRead)
            ' 2. 找到属C下的所有物种（假设我们有一个函数可以做到）
            Dim speciesInGenus As New List(Of Integer)(GetSpeciesInGenus(ncbi_taxid)) ' 例如返回 {taxid_s1, taxid_s2}
            ' 3. 收集所有与属C相关的k-mer
            Dim relevantKmers As New List(Of String)

            For Each seed As KmerSeed In kmerOfRead
                For Each src As KmerSource In seed.source
                    Dim taxId As Integer = sequenceLookup(src.seqid).ncbi_taxid
                    ' taxid is the child of target genusC_taxid?
                    If IsDescendantOf(taxId, ncbi_taxid) Then
                        relevantKmers.Add(seed.kmer)
                        Exit For ' 一个k-mer只算一次
                    End If
                Next
            Next

            If relevantKmers.Count = 0 OrElse speciesInGenus.Count = 0 Then
                Return New Dictionary(Of Integer, Double)()
            End If

            ' 4. 计算每个物种的后验概率
            Dim posteriors As New Dictionary(Of Integer, Double)()
            Dim logDenominator As Double = Double.NegativeInfinity

            ' 计算分子部分的对数值，并找出分母的对数值
            For Each speciesId As Integer In speciesInGenus
                If Not priors.ContainsKey(speciesId) Then Continue For

                Dim prior As Double = priors(speciesId)
                Dim logLikelihood As Double = 0

                For Each kmer As String In relevantKmers
                    If KmerDistributions.ContainsKey(kmer) AndAlso KmerDistributions(kmer).ContainsKey(speciesId) Then
                        Dim prob As Double = KmerDistributions(kmer)(speciesId)
                        ' 避免log(0)
                        If prob > 0 Then
                            logLikelihood += Math.Log(prob)
                        End If
                    End If
                Next

                Dim logNumerator As Double = Math.Log(prior) + logLikelihood
                posteriors(speciesId) = logNumerator

                ' 使用 log-sum-exp 技巧来计算分母的对数，防止数值溢出
                logDenominator = LogSumExp(logDenominator, logNumerator)
            Next

            ' 5. 归一化，得到最终的后验概率
            Dim finalPosteriors As New Dictionary(Of Integer, Double)()
            For Each kvp In posteriors
                finalPosteriors(kvp.Key) = Math.Exp(kvp.Value - logDenominator)
            Next

            Return finalPosteriors
        End Function

        ''' <summary>
        ''' 辅助函数，用于稳定地计算 log(a + b)
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Shared Function LogSumExp(a As Double, b As Double) As Double
            If Double.IsNegativeInfinity(a) Then Return b
            If Double.IsNegativeInfinity(b) Then Return a
            Return Math.Max(a, b) + Math.Log(1 + Math.Exp(-Math.Abs(a - b)))
        End Function
    End Class
End Namespace