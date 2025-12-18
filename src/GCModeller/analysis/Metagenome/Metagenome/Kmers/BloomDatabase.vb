Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.MIME.application.json
Imports SMRUCC.genomics.Analysis.Metagenome.Kmers.Kraken2
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Kmers

    Public Class BloomDatabase

        ReadOnly genomes As KmerBloomFilter()
        ReadOnly k As Integer
        ReadOnly NcbiTaxonomyTree As LCA
        ReadOnly min_supports As Double = 0.35
        ReadOnly coverage As Double = 0.5

        ' 当前设置的 0.85​ 的覆盖率阈值，相当于要求一条测序reads有超过85%的部分都必须与某个物种的基因组高度相似，才认为它可能属于该物种。这在宏基因组实验中是非常苛刻的条件，因为
        ' 序列多样性：微生物组中物种本身具有遗传多样性。
        ' 测序错误：测序过程会引入错误。
        ' 数据库不完整：参考数据库无法包含所有物种的全部基因组变异信息。
        ' 布隆过滤器假阳性：布隆过滤器本身存在假阳性概率，这可能会增加所有物种的命中数，但通过设置高阈值可以一定程度上抑制假阳性带来的噪声，不过设置过高也会过滤掉真实信号。
        ' 因此，大部分reads都无法满足如此苛刻的条件，导致最终能进入LCA计算的tax_id数量稀少。

        Sub New(genomes As IEnumerable(Of KmerBloomFilter), lca As LCA,
                Optional minSupports As Double = 0.35,
                Optional coverage As Double = 0.5)

            Me.genomes = genomes.ToArray
            Me.NcbiTaxonomyTree = lca
            Me.min_supports = min_supports
            Me.coverage = coverage

            Dim checkKmer = Me.genomes.GroupBy(Function(a) a.k).ToArray

            If checkKmer.Length = 1 Then
                k = checkKmer(0).Key
            Else
                Throw New InvalidProgramException($"there are multiple k-mer length(k={checkKmer.Keys.ToArray.GetJson}) bloom filter is mixed at here!")
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return $"{genomes.Length} genomics k-mer(len={k}) bloom filters"
        End Function

        Public Function MakeClassify(read As IFastaProvider) As KrakenOutputRecord
            Dim hits As New Dictionary(Of Integer, Integer)
            ' split reads sequence as kmers
            Dim kmers As String() = KSeq.KmerSpans(read.GetSequenceData, k).ToArray

            ' scan bloom filter model for each background genome
            ' get kmer hits number
            For Each genome As KmerBloomFilter In genomes
                Dim numHits As Integer = genome.KmerHitNumber(kmers)
                Dim unmap As Integer = kmers.Length - numHits

                If hits.ContainsKey(genome.ncbi_taxid) Then
                    hits(genome.ncbi_taxid) += numHits
                Else
                    hits(genome.ncbi_taxid) = numHits
                End If

                If hits.ContainsKey(0L) Then
                    hits(0L) += unmap
                Else
                    hits(0L) = unmap
                End If
            Next

            ' filter low hits genome, use high hits genome for LCA evaluation
            Dim numCov As Integer = kmers.Length * coverage
            Dim tax_id = From tax As KeyValuePair(Of Integer, Integer)
                         In hits
                         Where tax.Key > 0 AndAlso tax.Value >= numCov
                         Select tax.Key
            Dim lcaNode As LcaResult = NcbiTaxonomyTree.GetLCAForMetagenomics(tax_id, min_supports)

            ' lca node not found
            If lcaNode.LCATaxid = 0 Then
                Dim desc = hits.Where(Function(a) a.Key > 0).OrderByDescending(Function(a) a.Value).ToArray
                Dim topHit = desc.FirstOrDefault

                If topHit.Key > 0 AndAlso topHit.Value > 0 Then
                    ' 1 top hits must be nearly identical to the target reads
                    ' 2 top hits is unique hits
                    ' or top hits is significant greater than other genome hits
                    If topHit.Value / kmers.Length > 0.99 AndAlso (hits.Count = 1 OrElse (hits.Count > 1 AndAlso topHit.Value / desc(1).Value > 2)) Then
                        lcaNode = New LcaResult With {
                            .lcaNode = NcbiTaxonomyTree.GetNode(topHit.Key)
                        }
                    End If
                End If
            End If

            Return New KrakenOutputRecord With {
                .LcaMappings = hits _
                    .ToDictionary(Function(a) a.Key.ToString,
                                  Function(a)
                                      Return a.Value
                                  End Function),
                .ReadLength = read.length,
                .ReadName = read.title,
                .StatusCode = If(hits.Keys.Any(Function(t) t > 0), "C", "U"),
                .TaxID = lcaNode.LCATaxid,
                .Taxonomy = If(lcaNode.LCATaxid = 0, "n/a", lcaNode.lcaNode.name & $"({lcaNode.lcaNode.rank})")
            }
        End Function

    End Class
End Namespace