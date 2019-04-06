Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Data.DEG.OGEE.Models

Namespace DEG.OGEE

    Public Module Extensions

        <Extension>
        Public Iterator Function Join(genes As IEnumerable(Of genes),
                                      dataset As Dictionary(Of String, datasets),
                                      geneSet As Dictionary(Of String, gene_essentiality)) As IEnumerable(Of geneSetInfo)

            For Each gene As genes In genes
                If Not geneSet.ContainsKey(gene.locus) Then
                    Continue For
                End If

                Dim essentiality As gene_essentiality = geneSet(gene.locus)
                Dim dataInfo As datasets = dataset(essentiality.datasetID)

                Yield New geneSetInfo With {
                    .dataset = dataInfo,
                    .essentiality = essentiality,
                    .gene = gene
                }
            Next
        End Function
    End Module
End Namespace