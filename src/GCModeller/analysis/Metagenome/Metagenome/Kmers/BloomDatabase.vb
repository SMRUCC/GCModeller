Imports SMRUCC.genomics.Analysis.Metagenome.Kmers.Kraken2
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Kmers

    Public Class BloomDatabase

        ReadOnly genomes As KmerBloomFilter()
        ReadOnly k As Integer
        ReadOnly NcbiTaxonomyTree As LCA
        ReadOnly min_supports As Double = 0.5

        Public Function MakeClass(read As IFastaProvider) As KrakenOutputRecord
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

            Dim lca_id As LcaResult = NcbiTaxonomyTree.GetLCAForMetagenomics(From taxid As Integer
                                                                             In hits.Keys
                                                                             Where taxid > 0, min_supports)
            Return New KrakenOutputRecord With {
                .LcaMappings = hits,
                .ReadLength = read.length,
                .ReadName = read.title,
                .StatusCode = If(hits.Keys.Any(Function(t) t > 0), "C", "U"),
                .TaxID = lca_id.lcaNode.taxid
            }
        End Function

    End Class
End Namespace