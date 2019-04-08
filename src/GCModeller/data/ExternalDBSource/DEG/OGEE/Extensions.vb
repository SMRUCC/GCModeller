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

        Public Function LoadGeneSet(file As String, Optional all As Boolean = False, Optional kingdom$ = "bacteria") As Dictionary(Of String, gene_essentiality)
            Return file.LoadTsv(Of gene_essentiality) _
                .Where(Function(gene)
                           If all Then
                               If kingdom <> "*" Then
                                   Return gene.kingdom = kingdom
                               Else
                                   Return True
                               End If
                           Else
                               If kingdom <> "*" Then
                                   Return gene.kingdom = kingdom
                               Else
                                   Return gene.essential = "E"
                               End If
                           End If
                       End Function) _
                .GroupBy(Function(g) g.locus) _
                .ToDictionary(Function(g) g.Key, Function(g)
                                                     If g.Count > 1 Then
                                                         Call g.Key.Warning
                                                     End If

                                                     Return g.First
                                                 End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadDataSet(file As String) As Dictionary(Of String, datasets)
            Return file.LoadTsv(Of datasets).ToDictionary(Function(d) d.datasetID)
        End Function
    End Module
End Namespace