#Region "Microsoft.VisualBasic::3c8da615370f8eab799f27f2d6910ea4, R#\gseakit\GSEA.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:

' Module GSEA
' 
'     Function: CreateGOEnrichmentGraph, DrawGOEnrichmentGraph, Enrichment, FDR, GOEnrichment
'               KOBASFormat, ReadEnrichmentTerms, SaveEnrichment
' 
' Enum EnrichmentTableFormat
' 
'     GCModeller, KOBAS
' 
'  
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Data.GeneOntology.obographs
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' The GCModeller GSEA toolkit
''' </summary>
<Package("GSEA", Category:=APICategories.ResearchTools)>
Module GSEA

    Sub Main()
        Internal.Object.Converts.makeDataframe.addHandler(GetType(EnrichmentResult()), AddressOf enrichmentTable)
    End Sub

    Private Function enrichmentTable(result As EnrichmentResult(), args As list, env As Environment) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = result.Select(Function(d) d.term).ToArray
        }

        table.columns("name") = result.Select(Function(d) d.name).ToArray
        table.columns("description") = result.Select(Function(d) d.description).ToArray
        table.columns("cluster") = result.Select(Function(d) d.cluster).ToArray
        table.columns("enriched") = result.Select(Function(d) d.enriched).ToArray
        table.columns("score") = result.Select(Function(d) d.score).ToArray
        table.columns("pvalue") = result.Select(Function(d) d.pvalue).ToArray
        table.columns("FDR") = result.Select(Function(d) d.FDR).ToArray
        table.columns("geneIDs") = result.Select(Function(d) d.geneIDs.JoinBy(";")).ToArray

        Return table
    End Function

    ''' <summary>
    ''' read the enrichment result table
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.enrichment")>
    Public Function ReadEnrichmentTerms(file As String) As EnrichmentResult()
        Return file.LoadCsv(Of EnrichmentResult)
    End Function

    ''' <summary>
    ''' do GSEA enrichment analysis
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="geneSet">a given geneset id list</param>
    ''' <returns></returns>
    <ExportAPI("enrichment")>
    Public Function Enrichment(background As Background,
                               geneSet$(),
                               Optional outputAll As Boolean = True,
                               Optional showProgress As Boolean = False) As EnrichmentResult()

        Return background.Enrichment(
            list:=geneSet,
            outputAll:=outputAll,
            showProgress:=showProgress
        ).OrderBy(Function(a) a.pvalue) _
         .ToArray
    End Function

    ''' <summary>
    ''' fisher enrichment test
    ''' </summary>
    ''' <param name="list"></param>
    ''' <param name="geneSet"></param>
    ''' <param name="background"></param>
    ''' <returns></returns>
    <ExportAPI("fisher")>
    Public Function fisher(list As String(),
                           geneSet As String(),
                           background As String(),
                           Optional term As String = "unknown") As EnrichmentResult

        Dim info As New Cluster With {
            .ID = term,
            .description = term,
            .names = term,
            .members = New BackgroundGene(geneSet.Length - 1) {}
        }
        Dim enrich As EnrichmentResult = info.calcResult(
            enriched:=list,
            inputSize:=list.Length,
            genes:=background.Length,
            outputAll:=True
        )

        Return enrich
    End Function

    ''' <summary>
    ''' do GO GSEA enrichment analysis
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="geneSet"></param>
    ''' <param name="go">the go database object can be the raw go obo data or the DAG graph model.</param>
    ''' <returns></returns>
    <ExportAPI("enrichment.go")>
    <RApiReturn(GetType(EnrichmentResult))>
    Public Function GOEnrichment(background As Background,
                                 geneSet$(),
                                 go As Object,
                                 Optional showProgress As Boolean = True,
                                 Optional env As Environment = Nothing) As Object
        If go Is Nothing Then
            Return Internal.debug.stop(New ArgumentNullException("Missing the required GO database!"), env)
        ElseIf TypeOf go Is vbObject Then
            go = DirectCast(go, vbObject).target
        End If

        If go.GetType() Is GetType(GO_OBO) Then
            Return background.Enrichment(
                list:=geneSet,
                goClusters:=New DAG.Graph(DirectCast(go, GO_OBO).AsEnumerable),
                showProgress:=showProgress
            ).ToArray
        ElseIf go.GetType Is GetType(DAG.Graph) Then
            Return background.Enrichment(geneSet, DirectCast(go, DAG.Graph), showProgress:=showProgress).ToArray
        Else
            Return Internal.debug.stop(New InvalidProgramException(go.GetType.FullName), env)
        End If
    End Function

    ''' <summary>
    ''' save the enrichment analysis result
    ''' </summary>
    ''' <param name="enrichment"></param>
    ''' <param name="file"></param>
    ''' <param name="format"></param>
    ''' <returns></returns>
    <ExportAPI("write.enrichment")>
    <RApiReturn(GetType(Boolean))>
    Public Function SaveEnrichment(<RRawVectorArgument>
                                   enrichment As Object,
                                   file$,
                                   Optional format As EnrichmentTableFormat = EnrichmentTableFormat.GCModeller,
                                   Optional env As Environment = Nothing) As Object

        If enrichment Is Nothing Then
            Return Internal.debug.stop(New ArgumentNullException(NameOf(enrichment)), env)
        End If

        Dim verbose As Boolean = env.globalEnvironment.options.verbose

        If REnv.isVector(Of EnrichmentResult)(enrichment) Then
            If format = EnrichmentTableFormat.GCModeller Then
                Return DirectCast(enrichment, EnrichmentResult()).SaveTo(file, silent:=Not verbose)
            ElseIf format = EnrichmentTableFormat.KOBAS Then
                Return KOBASFormat(enrichment).SaveTo(file, silent:=Not verbose)
            Else
                Return Internal.debug.stop(New NotImplementedException(format), env)
            End If
        ElseIf REnv.isVector(Of EnrichmentTerm)(enrichment) Then
            Return DirectCast(enrichment, EnrichmentTerm()).SaveTo(file, silent:=Not verbose)
        Else
            Return Internal.debug.stop(New InvalidProgramException(enrichment.GetType.FullName), env)
        End If
    End Function

    <ExportAPI("enrichment.FDR")>
    Public Function FDR(enrichment As EnrichmentResult()) As EnrichmentResult()
        Return enrichment _
            .FDRCorrection _
            .OrderBy(Function(e) e.FDR) _
            .ToArray
    End Function

    ''' <summary>
    ''' Convert GSEA enrichment result from GCModeller output format to KOBAS output format
    ''' </summary>
    ''' <param name="enrichment"></param>
    ''' <param name="database$"></param>
    ''' <returns></returns>
    <ExportAPI("as.KOBAS_terms")>
    Public Function KOBASFormat(enrichment As EnrichmentResult(), Optional database$ = "n/a") As EnrichmentTerm()
        Return enrichment.Converts(database).ToArray
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

Public Enum EnrichmentTableFormat
    GCModeller
    KOBAS
End Enum
