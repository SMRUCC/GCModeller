Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports F = Microsoft.VisualBasic.Math.Statistics.FisherTest

''' <summary>
''' 基于Fisher Extract test算法的富集分析
''' </summary>
Public Module Enrichment

    <Extension>
    Public Iterator Function Enrichment(genome As Genome, list As IEnumerable(Of String)) As IEnumerable(Of EnrichmentResult)
        With list.ToArray
            Dim genes% = Aggregate cluster
                         In genome.Clusters
                         Into Sum(cluster.Members.Length)

            For Each cluster In genome.Clusters
                Dim enriched$() = cluster.Intersect(.ByRef).ToArray
                Dim a% = enriched.Length
                Dim b% = cluster.Members.Length
                Dim c% = .Length
                Dim d% = genes
                Dim pvalue# = F.FisherPvalue(a, b, c, d)
                Dim score# = a / b

                Yield New EnrichmentResult With {
                    .Term = cluster.Name,
                    .Enriched = enriched,
                    .Pvalue = pvalue,
                    .Score = score
                }
            Next
        End With
    End Function

    <Extension>
    Public Function FDRCorrection(enrichments As IEnumerable(Of EnrichmentResult)) As EnrichmentResult()
        With enrichments.Shadows
            !FDR = !Pvalue.FDR
            Return .ByRef.ToArray
        End With
    End Function
End Module
