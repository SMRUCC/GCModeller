Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module NamespaceCategoryPlots

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data">输出的结果都应该是不重复的</param>
    ''' <param name="GO_terms"></param>
    ''' <param name="size"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function NamespaceEnrichmentPlot(Of EnrichmentTerm As IGoTermEnrichment)(
                    data As IEnumerable(Of EnrichmentTerm),
                    GO_terms As Dictionary(Of Term),
                    Optional pvalue# = 0.05,
                    Optional size$ = "2200,2000",
                    Optional tick# = 1,
                    Optional usingCorrected As Boolean = False,
                    Optional top% = -1,
                    Optional colorSchema$ = "Set1:c6") As IEnumerable(Of NamedValue(Of GraphicsData))

        Dim namespaceProfiles = data.CreateEnrichmentProfiles(GO_terms, usingCorrected, top, pvalue)
        Dim image As GraphicsData
        Dim namespaceTitle$

        For Each [namespace] In namespaceProfiles
            namespaceTitle = [namespace].Key
            image = [namespace].Value.doSingleBarplot(namespaceTitle, size, tick, colorSchema)

            Yield New NamedValue(Of GraphicsData) With {
                .Name = [namespace].Key,
                .Value = image
            }
        Next
    End Function

    <Extension>
    Private Function doSingleBarplot(profiles As NamedValue(Of Double)(), namespace$, size$, tick#, colorSchema$) As GraphicsData
        Return LevelBarplot.Plot(
            data:=profiles,
            size:=size,
            title:=$"Enrichment of {[namespace]}"
        )
    End Function

End Module
