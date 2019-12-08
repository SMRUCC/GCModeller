Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Data.GeneOntology

<Package("GSEA", Category:=APICategories.ResearchTools)>
Module GSEA

    <ExportAPI("enrichment")>
    Public Function Enrichment(background As Background, geneSet$()) As EnrichmentResult()
        Return background.Enrichment(geneSet, False).ToArray
    End Function

    <ExportAPI("enrichment.go")>
    Public Function GOEnrichment(background As Background, geneSet$(), go As OBO.GO_OBO) As EnrichmentResult()
        Return background.Enrichment(geneSet, go, False).ToArray
    End Function

    <ExportAPI("write.enrichment")>
    Public Function SaveEnrichment(enrichment As EnrichmentResult(), file$, Optional format$ = "GCModeller") As Boolean
        If format = "GCModeller" Then
            Return enrichment.SaveTo(file)
        Else
            Throw New NotImplementedException(format)
        End If
    End Function
End Module

