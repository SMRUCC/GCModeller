Namespace Kmers

    Public Module AbundanceEstimate

        ''' <summary>
        ''' 阶段一：从单条Read的KmerSeed数组中提取原始分类信息
        ''' </summary>
        ''' <param name="kmerSeedsForRead"></param>
        ''' <param name="sequenceLookup"></param>
        ''' <returns></returns>
        Public Function GetRawTaxonomyCounts(kmerSeedsForRead As KmerSeed(), sequenceLookup As SequenceCollection) As Dictionary(Of Integer, Integer)
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

        Function ReestimateAbundanceForGenus(kmerSeedsForRead As KmerSeed(), genusC_taxid As Integer) As Dictionary(Of Integer, Double)
            '' 1. 获取原始分类计数
            'Dim rawCounts As Dictionary(Of Integer, Integer) = GetRawTaxonomyCounts(kmerSeedsForRead)

            '' 2. 找到属C下的所有物种（假设我们有一个函数可以做到）
            'Dim speciesInGenus As List(Of Integer) = GetSpeciesInGenus(genusC_taxid) ' 例如返回 {taxid_s1, taxid_s2}

            '' 3. 收集所有与属C相关的k-mer
            'Dim relevantKmers As New List(Of String)
            'For Each seed As KmerSeed In kmerSeedsForRead
            '    For Each src As KmerSource In seed.source
            '        Dim taxId As Integer = SequenceLookup(src.seqid).ncbi_taxid
            '        If IsDescendantOf(taxId, genusC_taxid) Then
            '            relevantKmers.Add(seed.kmer)
            '            Exit For ' 一个k-mer只算一次
            '        End If
            '    Next
            'Next

            'If relevantKmers.Count = 0 OrElse speciesInGenus.Count = 0 Then
            '    Return New Dictionary(Of Integer, Double)()
            'End If

            '' 4. 计算每个物种的后验概率
            'Dim posteriors As New Dictionary(Of Integer, Double)()
            'Dim logDenominator As Double = Double.NegativeInfinity

            '' 计算分子部分的对数值，并找出分母的对数值
            'For Each speciesId As Integer In speciesInGenus
            '    If Not Priors.ContainsKey(speciesId) Then Continue For

            '    Dim prior As Double = Priors(speciesId)
            '    Dim logLikelihood As Double = 0

            '    For Each kmer As String In relevantKmers
            '        If KmerDistributions.ContainsKey(kmer) AndAlso KmerDistributions(kmer).ContainsKey(speciesId) Then
            '            Dim prob As Double = KmerDistributions(kmer)(speciesId)
            '            ' 避免log(0)
            '            If prob > 0 Then
            '                logLikelihood += Math.Log(prob)
            '            End If
            '        End If
            '    Next

            '    Dim logNumerator As Double = Math.Log(prior) + logLikelihood
            '    posteriors(speciesId) = logNumerator

            '    ' 使用 log-sum-exp 技巧来计算分母的对数，防止数值溢出
            '    logDenominator = LogSumExp(logDenominator, logNumerator)
            'Next

            '' 5. 归一化，得到最终的后验概率
            'Dim finalPosteriors As New Dictionary(Of Integer, Double)()
            'For Each kvp In posteriors
            '    finalPosteriors(kvp.Key) = Math.Exp(kvp.Value - logDenominator)
            'Next

            'Return finalPosteriors
        End Function

        ' 辅助函数，用于稳定地计算 log(a + b)
        Function LogSumExp(a As Double, b As Double) As Double
            If Double.IsNegativeInfinity(a) Then Return b
            If Double.IsNegativeInfinity(b) Then Return a
            Return Math.Max(a, b) + Math.Log(1 + Math.Exp(-Math.Abs(a - b)))
        End Function
    End Module
End Namespace