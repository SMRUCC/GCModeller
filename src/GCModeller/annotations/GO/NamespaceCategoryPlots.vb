Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
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
                    Optional gray As Boolean = False,
                    Optional labelRightAlignment As Boolean = False,
                    Optional usingCorrected As Boolean = False,
                    Optional top% = -1,
                    Optional colorSchema$ = "Set1:c6") As IEnumerable(Of NamedValue(Of GraphicsData))


    End Function
End Module
