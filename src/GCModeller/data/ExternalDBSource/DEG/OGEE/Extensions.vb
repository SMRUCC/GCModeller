Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadGenes(file As String) As IEnumerable(Of genes)
            Return file.OpenHandle(, tsv:=True).AsLinq(Of genes)
        End Function

        Public Function LoadGeneSet(file As String, Optional all As Boolean = False) As Dictionary(Of String, gene_essentiality)
            Return file.LoadTsv(Of gene_essentiality) _
                .Where(Function(gene)
                           If all Then
                               Return True
                           Else
                               Return gene.essential = "E"
                           End If
                       End Function) _
                .ToDictionary(Function(g) g.locus)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadDataSet(file As String) As Dictionary(Of String, datasets)
            Return file.LoadTsv(Of datasets).ToDictionary(Function(d) d.datasetID)
        End Function
    End Module
End Namespace