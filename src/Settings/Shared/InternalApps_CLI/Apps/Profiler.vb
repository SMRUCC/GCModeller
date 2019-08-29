Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\Profiler.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   1.0.0.0
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // 
' 
' 
'  < Profiler.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /GO.clusters:            
'  /GSEA:                   Do gene set enrichment analysis.
'  /id.converts:            
'  /KO.clusters:            Create KEGG pathway map background for a given genome data.
'  /KO.clusters.By_bbh:     
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Profiler.CLI
''' </summary>
'''
Public Class Profiler : Inherits InteropService

    Public Const App$ = "Profiler.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Profiler
          Return New Profiler(App:=directory & "/" & Profiler.App)
     End Function

''' <summary>
''' ```
''' /GO.clusters /uniprot &lt;uniprot.XML> /go &lt;go.obo> [/out &lt;clusters.XML>]
''' ```
''' </summary>
'''
Public Function CreateGOClusters(uniprot As String, go As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/GO.clusters")
    Call CLI.Append(" ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/go " & """" & go & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /GSEA /background &lt;clusters.XML> /geneSet &lt;geneSet.txt> [/hide.progress /locus_tag /cluster_id &lt;null, debug_used> /out &lt;out.csv>]
''' ```
''' Do gene set enrichment analysis.
''' </summary>
'''
Public Function EnrichmentTest(background As String, geneSet As String, Optional cluster_id As String = "", Optional out As String = "", Optional hide_progress As Boolean = False, Optional locus_tag As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/GSEA")
    Call CLI.Append(" ")
    Call CLI.Append("/background " & """" & background & """ ")
    Call CLI.Append("/geneSet " & """" & geneSet & """ ")
    If Not cluster_id.StringEmpty Then
            Call CLI.Append("/cluster_id " & """" & cluster_id & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If hide_progress Then
        Call CLI.Append("/hide.progress ")
    End If
    If locus_tag Then
        Call CLI.Append("/locus_tag ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /id.converts /uniprot &lt;uniprot.XML> /geneSet &lt;geneSet.txt> [/out &lt;converts.txt>]
''' ```
''' </summary>
'''
Public Function IDconverts(uniprot As String, geneSet As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/id.converts")
    Call CLI.Append(" ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/geneSet " & """" & geneSet & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /KO.clusters /uniprot &lt;uniprot.XML> /maps &lt;kegg_maps.XML/directory> [/out &lt;clusters.XML>]
''' ```
''' Create KEGG pathway map background for a given genome data.
''' </summary>
'''
Public Function CreateKOCluster(uniprot As String, maps As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KO.clusters")
    Call CLI.Append(" ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /KO.clusters.By_bbh /in &lt;KO.bbh.csv> /maps &lt;kegg_maps.XML/directory> [/size &lt;backgroundSize, default=-1> /genome &lt;genomeName/taxonomy> /out &lt;clusters.XML>]
''' ```
''' </summary>
'''
Public Function CreateKOClusterFromBBH([in] As String, maps As String, Optional size As String = "-1", Optional genome As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KO.clusters.By_bbh")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not genome.StringEmpty Then
            Call CLI.Append("/genome " & """" & genome & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
