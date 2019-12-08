Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Data.GeneOntology

<Package("GSEA", Category:=APICategories.ResearchTools)>
Module GSEA

    <ExportAPI("enrichment")>
    Public Function Enrichment(background As Background, geneSet$())

    End Function

    <ExportAPI("enrichment.go")>
    Public Function GOEnrichment(background As Background, geneSet$(), go As OBO.GO_OBO)

    End Function
End Module
