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
'  // VERSION:   3.3277.7609.23259
'  // ASSEMBLY:  Settings, Version=3.3277.7609.23259, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/31/2020 12:55:18 PM
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
'  /GO.clusters.blastp:              Create GO clusters from the blastp besthit dataset.
'  /GO.enrichment.barplot:           
'  /GSEA:                            Do gene set enrichment analysis.
'  /GSEA.GO:                         
'  /id.converts:                     
'  /kegg.metabolites.background:     Create background model for KEGG pathway enrichment based on the
'                                    kegg metabolites, used for LC-MS metabolism data analysis.
'  /KO.clusters:                     Create KEGG pathway map background for a given genome data or a
'                                    reference KO list.
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
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Profiler
          Return New Profiler(App:=directory & "/" & Profiler.App)
     End Function

''' <summary>
''' ```bash
''' /GO.clusters /uniprot &lt;uniprot.XML&gt; /go &lt;go.obo&gt; [/generic /out &lt;clusters.XML&gt;]
''' ```
''' Create GO enrichment background model from uniprot database.
''' </summary>
'''
''' <param name="uniprot"> The uniprot database.
''' </param>
Public Function CreateGOClusters(uniprot As String, go As String, Optional out As String = "", Optional generic As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/GO.clusters")
    Call CLI.Append(" ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/go " & """" & go & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If generic Then
        Call CLI.Append("/generic ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /GO.clusters.blastp /in &lt;besthit.csv&gt; /go &lt;go.obo&gt; [/size &lt;default=-1&gt; /out &lt;clusters.XML&gt;]
''' ```
''' Create GO clusters from the blastp besthit dataset.
''' </summary>
'''

Public Function GOCluster_blastp([in] As String, go As String, Optional size As String = "-1", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/GO.clusters.blastp")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/go " & """" & go & """ ")
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /GO.enrichment.barplot /in &lt;result.csv&gt; [/go &lt;go.obo&gt; /disable.label_trim /top &lt;default=35&gt; /colors &lt;schemaName, default=YlGnBu:c8&gt; /tiff /out &lt;output_directory&gt;]
''' ```
''' </summary>
'''

Public Function GOEnrichmentBarPlot([in] As String, 
                                       Optional go As String = "", 
                                       Optional top As String = "35", 
                                       Optional colors As String = "YlGnBu:c8", 
                                       Optional out As String = "", 
                                       Optional disable_label_trim As Boolean = False, 
                                       Optional tiff As Boolean = False) As Integer
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
    If disable_label_trim Then
        Call CLI.Append("/disable.label_trim ")
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
''' <param name="background"> A genome background data file which is created by ``/KO.clusters`` or ``/GO.clusters`` tools.
''' </param>
''' <param name="cluster_id"> A list of specific cluster id that used for program debug use only.
''' </param>
''' <param name="format"> apply this argument to specify the output table format, by default is in GCModeller table format, or you can assign the ``KOBAS`` format value at this parameter.
''' </param>
''' <param name="out"> The file path of the result output, the output result table format is affects by the ``/format`` argument.
''' </param>
''' <param name="geneSet"> A text file that contains the gene id list that will be apply the GSEA analysis.
''' </param>
''' <param name="hide_progress"> A logical flag argument that controls the console screen display the progress bar or not.
''' </param>
Public Function EnrichmentTest(background As String, 
                                  geneSet As String, 
                                  Optional cluster_id As String = "", 
                                  Optional format As String = "GCModeller", 
                                  Optional out As String = "", 
                                  Optional hide_progress As Boolean = False, 
                                  Optional locus_tag As Boolean = False) As Integer
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
''' /GSEA.GO /background &lt;clusters.XML&gt; /geneSet &lt;geneSet.txt&gt; /go &lt;go.obo&gt; [/hide.progress /locus_tag /cluster_id &lt;null, debug_used&gt; /format &lt;default=GCModeller&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function GSEA_GO(background As String, 
                           geneSet As String, 
                           go As String, 
                           Optional cluster_id As String = "", 
                           Optional format As String = "GCModeller", 
                           Optional out As String = "", 
                           Optional hide_progress As Boolean = False, 
                           Optional locus_tag As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/GSEA.GO")
    Call CLI.Append(" ")
    Call CLI.Append("/background " & """" & background & """ ")
    Call CLI.Append("/geneSet " & """" & geneSet & """ ")
    Call CLI.Append("/go " & """" & go & """ ")
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
''' /kegg.metabolites.background /in &lt;organism.repository_directory&gt; [/ref &lt;KO.assign.csv&gt; /out &lt;background_model.Xml&gt;]
''' ```
''' Create background model for KEGG pathway enrichment based on the kegg metabolites, used for LC-MS metabolism data analysis.
''' </summary>
'''
''' <param name="[in]"> A repository directory that contains the pathway map data and which is generated by ``/Download.Pathway.Maps`` tools in cli app &apos;KEGG_tools&apos;.
''' </param>
Public Function MetaboliteBackground([in] As String, Optional ref As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/kegg.metabolites.background")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not ref.StringEmpty Then
            Call CLI.Append("/ref " & """" & ref & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KO.clusters /background &lt;KO.txt/uniprot.XML&gt; /maps &lt;kegg_maps.XML/directory&gt; [/generic /out &lt;clusters.XML&gt;]
''' ```
''' Create KEGG pathway map background for a given genome data or a reference KO list.
''' </summary>
'''
''' <param name="background"> the KO annotation background data, it can be a ``UniProt`` database that contains the uniprot_id to KO_id mapping. or just
'''               a plain text file that contains a list of KO terms as background, each line in this text file should be one KO term word.
''' </param>
''' <param name="maps"> This argument should be a directory path which this folder contains multiple 
'''               KEGG reference pathway map xml files. A xml file path of the kegg pathway map database 
'''               is also accepted!
''' </param>
Public Function CreateKOCluster(background As String, maps As String, Optional out As String = "", Optional generic As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/KO.clusters")
    Call CLI.Append(" ")
    Call CLI.Append("/background " & """" & background & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If generic Then
        Call CLI.Append("/generic ")
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

Public Function CreateKOClusterFromBBH([in] As String, 
                                          maps As String, 
                                          Optional size As String = "-1", 
                                          Optional genome As String = "", 
                                          Optional out As String = "") As Integer
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
