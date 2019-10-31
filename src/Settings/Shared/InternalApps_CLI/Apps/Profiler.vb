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
'  // VERSION:   3.3277.7242.27856
'  // ASSEMBLY:  Settings, Version=3.3277.7242.27856, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     2019/10/30 15:28:32
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
'  /GO.clusters:                     Create GO enrichment background model from uniprot database.
'  /GO.enrichment.barplot:           
'  /GSEA:                            Do gene set enrichment analysis.
'  /id.converts:                     
'  /kegg.metabolites.background:     Create background model for KEGG pathway enrichment based on the
'                                    kegg metabolites, used for LC-MS metabolism data analysis.
'  /KO.clusters:                     Create KEGG pathway map background for a given genome data.
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
''' ```bash
''' /GO.clusters /uniprot &lt;uniprot.XML&gt; /go &lt;go.obo&gt; [/out &lt;clusters.XML&gt;]
''' ```
''' Create GO enrichment background model from uniprot database.
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
''' ```bash
''' /GO.enrichment.barplot /in &lt;result.csv&gt; [/go &lt;go.obo&gt; /top &lt;default=35&gt; /colors &lt;schemaName, default=YlGnBu:c8&gt; /tiff /out &lt;output_directory&gt;]
''' ```
''' </summary>
'''
Public Function GOEnrichmentBarPlot([in] As String, Optional go As String = "", Optional top As String = "35", Optional colors As String = "YlGnBu:c8", Optional out As String = "", Optional tiff As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/GO.enrichment.barplot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not go.StringEmpty Then
            Call CLI.Append("/go " & """" & go & """ ")
    End If
    If Not top.StringEmpty Then
            Call CLI.Append("/top " & """" & top & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If tiff Then
        Call CLI.Append("/tiff ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /GSEA /background &lt;clusters.XML&gt; /geneSet &lt;geneSet.txt&gt; [/hide.progress /locus_tag /cluster_id &lt;null, debug_used&gt; /format &lt;default=GCModeller&gt; /out &lt;out.csv&gt;]
''' ```
''' Do gene set enrichment analysis.
''' </summary>
'''
Public Function EnrichmentTest(background As String, geneSet As String, Optional cluster_id As String = "", Optional format As String = "GCModeller", Optional out As String = "", Optional hide_progress As Boolean = False, Optional locus_tag As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/GSEA")
    Call CLI.Append(" ")
    Call CLI.Append("/background " & """" & background & """ ")
    Call CLI.Append("/geneSet " & """" & geneSet & """ ")
    If Not cluster_id.StringEmpty Then
            Call CLI.Append("/cluster_id " & """" & cluster_id & """ ")
    End If
    If Not format.StringEmpty Then
            Call CLI.Append("/format " & """" & format & """ ")
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
''' ```bash
''' /id.converts /uniprot &lt;uniprot.XML&gt; /geneSet &lt;geneSet.txt&gt; [/out &lt;converts.txt&gt;]
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
''' ```bash
''' /kegg.metabolites.background /in &lt;organism.repository_directory&gt; [/out &lt;background_model.Xml&gt;]
''' ```
''' Create background model for KEGG pathway enrichment based on the kegg metabolites, used for LC-MS metabolism data analysis.
''' </summary>
'''
Public Function MetaboliteBackground([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/kegg.metabolites.background")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KO.clusters /uniprot &lt;uniprot.XML&gt; /maps &lt;kegg_maps.XML/directory&gt; [/out &lt;clusters.XML&gt;]
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
''' ```bash
''' /KO.clusters.By_bbh /in &lt;KO.bbh.csv&gt; /maps &lt;kegg_maps.XML/directory&gt; [/size &lt;backgroundSize, default=-1&gt; /genome &lt;genomeName/taxonomy&gt; /out &lt;clusters.XML&gt;]
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
