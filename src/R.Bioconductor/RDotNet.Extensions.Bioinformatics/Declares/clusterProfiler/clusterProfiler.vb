Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.RScripts

Namespace clusterProfiler

    Public Module clusterProfiler

        <Extension> Public Function LoadGoBriefTable(goBriefTable As IO.File) As String
            Dim goBrief$ = goBriefTable.PushAsDataFrame
            Dim go2name$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{go2name} <- {goBrief}[, c(""goID"", ""name"")]"
                End With
            End SyncLock

            Return go2name
        End Function

        Public Function enricher(gene As IEnumerable(Of String), universe As IEnumerable(Of String), TERM2GENE As IEnumerable(Of NamedValue(Of String)),
                                 Optional pvalueCutoff# = 0.05,
                                 Optional pAdjustMethod$ = "BH",
                                 Optional minGSSize% = 5,
                                 Optional qvalueCutoff# = 0.2,
                                 Optional TERM2NAME As IEnumerable(Of NamedValue(Of String)) = Nothing) As enrichResult

            Dim t2g = TERM2GENE.dataframe(MappingsHelper.NamedValueMapsWrite("goID", "geneID"))
            Dim genes = base.c(gene.ToArray(AddressOf Rstring))
            Dim background = base.c(universe.ToArray(AddressOf Rstring))

            Return enricherS4(gene:=genes,
                              universe:=background,
                              TERM2GENE:=t2g,
                              minGSSize:=minGSSize,
                              pAdjustMethod:=pAdjustMethod,
                              pvalueCutoff:=pvalueCutoff,
                              qvalueCutoff:=qvalueCutoff)
        End Function

        Public Function enricherS4(gene$, universe$, TERM2GENE$,
                                    Optional pvalueCutoff# = 0.05,
                                    Optional pAdjustMethod$ = "BH",
                                    Optional minGSSize% = 5,
                                    Optional qvalueCutoff# = 0.2,
                                    Optional TERM2NAME$ = "NA") As enrichResult
            Dim var$ = enricher(gene, universe, TERM2GENE, pvalueCutoff, pAdjustMethod, minGSSize, qvalueCutoff, TERM2NAME)

            SyncLock R
                With R
                    Dim pointer = .Evaluate(var)


                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' A universal enrichment analyzer
        ''' </summary>
        ''' <param name="gene$">a vector of gene id</param>
        ''' <param name="universe$">background genes</param>
        ''' <param name="TERM2GENE$">user input annotation of TERM TO GENE mapping, a data.frame of 2 column with term and gene</param>
        ''' <param name="pvalueCutoff#">pvalue cutoff</param>
        ''' <param name="pAdjustMethod$">one of "holm", "hochberg", "hommel", "bonferroni", "BH", "BY", "fdr", "none"</param>
        ''' <param name="minGSSize%">minimal size of genes annotated for testing</param>
        ''' <param name="qvalueCutoff#">qvalue cutoff</param>
        ''' <param name="TERM2NAME$">user input of TERM TO NAME mapping, a data.frame of 2 column with term and name</param>
        ''' <returns>A <see cref="enrichResult"/> instance</returns>
        Public Function enricher(gene$, universe$, TERM2GENE$,
                                 Optional pvalueCutoff# = 0.05,
                                 Optional pAdjustMethod$ = "BH",
                                 Optional minGSSize% = 5,
                                 Optional qvalueCutoff# = 0.2,
                                 Optional TERM2NAME$ = "NA") As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- enricher({gene},
pvalueCutoff = {pvalueCutoff}, pAdjustMethod = {Rstring(pAdjustMethod)},
universe = {universe}, minGSSize = {minGSSize}, qvalueCutoff = {qvalueCutoff}, 
TERM2GENE = {TERM2GENE}, TERM2NAME = {TERM2NAME})"
                End With
            End SyncLock

            Return var
        End Function
    End Module

    Public Class Term2Gene

        Public Property GO_ID As String
        Public Property Locus_tag As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class enrichResult

    End Class
End Namespace