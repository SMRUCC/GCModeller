Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.GeneOntology.OBO

<Package("profiles")>
Module profiles

    ''' <summary>
    ''' Create catalog profiles data for GO enrichment result its data visualization.
    ''' </summary>
    ''' <param name="enrichments"></param>
    ''' <param name="goDb"></param>
    ''' <param name="top">display the top n enriched GO terms.</param>
    ''' <returns></returns>
    <ExportAPI("GO.enrichment.profile")>
    Public Function GOEnrichmentProfiles(enrichments As EnrichmentTerm(), goDb As GO_OBO, Optional top% = 10) As CatalogProfiles
        Dim GO_terms = goDb.AsEnumerable.ToDictionary
        ' 在这里是不进行筛选的
        ' 筛选应该是发生在脚本之中
        Dim profiles = enrichments.CreateEnrichmentProfiles(GO_terms, False, top, 1)

        Return profiles
    End Function

    <ExportAPI("KEGG.enrichment.profile")>
    Public Function KEGGEnrichmentProfiles(enrichments As EnrichmentTerm(), Optional top% = 10) As CatalogProfiles
        Dim profiles As Dictionary(Of String, Double) = enrichments.ToDictionary(Function(a) a.ID, Function(a) -Math.Log10(a.Pvalue))
        Dim result As CatalogProfiles = profiles.DoKeggProfiles(displays:=top)

        Return result
    End Function
End Module
