Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO

<Package("GSEA", Category:=APICategories.ResearchTools)>
Module GSEA

    <ExportAPI("enrichment")>
    Public Function Enrichment(background As Background, geneSet$()) As EnrichmentResult()
        Return background.Enrichment(geneSet, False).ToArray
    End Function

    <ExportAPI("enrichment.go")>
    Public Function GOEnrichment(background As Background, geneSet$(), go As Object) As EnrichmentResult()
        If go Is Nothing Then
            Throw New ArgumentNullException("GO database is missing!")
        End If

        If go.GetType() Is GetType(GO_OBO) Then
            Return background.Enrichment(geneSet, DirectCast(go, GO_OBO)).ToArray
        ElseIf go.GetType Is GetType(DAG.Graph) Then
            Return background.Enrichment(geneSet, DirectCast(go, DAG.Graph)).ToArray
        Else
            Throw New InvalidProgramException(go.GetType.FullName)
        End If
    End Function

    <ExportAPI("write.enrichment")>
    Public Function SaveEnrichment(enrichment As EnrichmentResult(), file$, Optional format$ = "GCModeller") As Boolean
        If format = "GCModeller" Then
            Return enrichment.SaveTo(file)
        Else
            Throw New NotImplementedException(format)
        End If
    End Function

    <ExportAPI("enrichment.FDR")>
    Public Function FDR(enrichment As EnrichmentResult()) As EnrichmentResult()
        Return enrichment.FDRCorrection
    End Function
End Module

