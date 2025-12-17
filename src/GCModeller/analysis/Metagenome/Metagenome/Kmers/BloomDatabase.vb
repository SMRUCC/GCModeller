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
        ReadOnly min_supports As Double = 0.5
        ReadOnly coverage As Double = 0.85

        Sub New(genomes As IEnumerable(Of KmerBloomFilter), lca As LCA,
                Optional minSupports As Double = 0.5,
                Optional coverage As Double = 0.85)

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
            Dim kmers As String() = KSeq.KmerSpans(read.GetSequenceData, k).ToArray

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

            Dim numCov As Integer = kmers.Length * coverage
            Dim tax_id = From tax As KeyValuePair(Of Integer, Integer)
                         In hits
                         Where tax.Key > 0 AndAlso tax.Value >= numCov
                         Select tax.Key
            Dim lca_id As LcaResult = NcbiTaxonomyTree.GetLCAForMetagenomics(tax_id, min_supports)

            Return New KrakenOutputRecord With {
                .LcaMappings = hits _
                    .ToDictionary(Function(a) a.Key.ToString,
                                  Function(a)
                                      Return a.Value
                                  End Function),
                .ReadLength = read.length,
                .ReadName = read.title,
                .StatusCode = If(hits.Keys.Any(Function(t) t > 0), "C", "U"),
                .TaxID = lca_id.LCATaxid
            }
        End Function

    End Class
End Namespace