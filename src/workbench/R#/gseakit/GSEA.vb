Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Data.GeneOntology.obographs

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
        ElseIf format = "KOBAS" Then
            Return KOBASFormat(enrichment).SaveTo(file)
        Else
            Throw New NotImplementedException(format)
        End If
    End Function

    <ExportAPI("enrichment.FDR")>
    Public Function FDR(enrichment As EnrichmentResult()) As EnrichmentResult()
        Return enrichment.FDRCorrection
    End Function

    <ExportAPI("as.KOBAS_terms")>
    Public Function KOBASFormat(enrichment As EnrichmentResult()) As EnrichmentTerm()
        Return enrichment.Converts.ToArray
    End Function

    <ExportAPI("enrichment.draw.go_dag")>
    Public Function DrawGOEnrichmentGraph(go_enrichment As EnrichmentResult(), go As GO_OBO) As GraphicsData
        Dim terms As String() = go_enrichment.Select(Function(term) term.term).ToArray
        Dim dag As NetworkGraph = go.CreateGraph(terms:=terms)
        Dim image As GraphicsData = EnrichmentVisualize.DrawGraph(dag)

        Return image
    End Function
End Module

