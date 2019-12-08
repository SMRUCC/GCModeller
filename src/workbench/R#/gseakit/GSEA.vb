Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Data.GeneOntology.obographs
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("GSEA", Category:=APICategories.ResearchTools)>
Module GSEA

    <ExportAPI("read.enrichment")>
    Public Function ReadEnrichmentTerms(file As String) As EnrichmentResult()
        Return file.LoadCsv(Of EnrichmentResult)
    End Function

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
    Public Function SaveEnrichment(<RRawVectorArgument> enrichment As Object, file$, Optional format$ = "GCModeller") As Boolean
        If enrichment Is Nothing Then
            Throw New ArgumentNullException(NameOf(enrichment))
        End If

        If REnv.isVector(Of EnrichmentResult)(enrichment) Then
            If format = "GCModeller" Then
                Return DirectCast(enrichment, EnrichmentResult()).SaveTo(file)
            ElseIf format = "KOBAS" Then
                Return KOBASFormat(enrichment).SaveTo(file)
            Else
                Throw New NotImplementedException(format)
            End If
        ElseIf REnv.isVector(Of EnrichmentTerm)(enrichment) Then
            Return DirectCast(enrichment, EnrichmentTerm()).SaveTo(file)
        Else
            Throw New InvalidProgramException(enrichment.GetType.FullName)
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

    ''' <summary>
    ''' Create network graph data for Cytoscape
    ''' </summary>
    ''' <param name="go_enrichment"></param>
    ''' <param name="go"></param>
    ''' <returns></returns>
    <ExportAPI("enrichment.go_dag")>
    Public Function CreateGOEnrichmentGraph(go_enrichment As EnrichmentResult(), go As GO_OBO) As NetworkTables
        Dim terms As NamedValue(Of Double)() = go_enrichment _
            .Select(Function(term)
                        Return New NamedValue(Of Double) With {
                            .Name = term.term,
                            .Value = -Math.Log10(term.pvalue)
                        }
                    End Function) _
            .ToArray
        Dim dag As NetworkGraph = go.CreateGraph(terms:=terms)
        Dim tables As NetworkTables = dag.DAGasTabular

        Return tables
    End Function

    <ExportAPI("enrichment.draw.go_dag")>
    Public Function DrawGOEnrichmentGraph(go_enrichment As EnrichmentResult(), go As GO_OBO) As GraphicsData
        Dim terms As NamedValue(Of Double)() = go_enrichment _
            .Select(Function(term)
                        Return New NamedValue(Of Double) With {
                            .Name = term.term,
                            .Value = -Math.Log10(term.pvalue)
                        }
                    End Function) _
            .ToArray
        Dim dag As NetworkGraph = go.CreateGraph(terms:=terms)
        Dim image As GraphicsData = EnrichmentVisualize.DrawGraph(dag)

        Return image
    End Function
End Module

