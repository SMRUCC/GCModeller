Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\metaProfiler.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   3.3277.7238.20186
'  // ASSEMBLY:  Settings, Version=3.3277.7238.20186, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/26/2019 11:12:52 AM
'  // 
' 
' 
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /box.plot:                                   
'  /do.enterotype.cluster:                      
'  /Export.Megan.BIOM:                          Export v1.0 biom json file for data visualize in Megan
'                                               program.
'  /gast.stat.names:                            
'  /heatmap.plot:                               
'  /hmp.otu_table:                              Export otu table from hmp biom files.
'  /LefSe.Matrix:                               Processing the relative aboundance matrix to the input
'                                               format file as it describ: http://huttenhower.sph.harvard.edu/galaxy/root?tool_id=lefse_upload
'  /Membrane_transport.network:                 Construct a relationship network based on the Membrane
'                                               transportor in bacteria genome
'  /Metabolic.EndPoint.Profiles.background:     Create Metabolic EndPoint Profiles Background Model
'  /OTU.cluster:                                
'  /Relative_abundance.barplot:                 
'  /Relative_abundance.stacked.barplot:         
'  /significant.difference:                     
'  /SILVA.bacteria:                             
'  /UPGMA.Tree:                                 
' 
' 
' API list that with functional grouping
' 
' 1. 02. Alpha diversity analysis tools
' 
' 
'    /Rank_Abundance:                             https://en.wikipedia.org/wiki/Rank_abundance_curve
' 
' 
' 2. 03. Human Microbiome Project cli tool
' 
' 
'    /handle.hmp.manifest:                        Download files from HMP website through http/fasp.
'    /hmp.manifest.files:                         
' 
' 
' 3. Microbiome antibiotic resistance composition analysis tools
' 
' 
'    /ARO.fasta.header.table:                     
' 
' 
' 4. Microbiome network cli tools
' 
' 
'    /Metagenome.UniProt.Ref:                     Create background model for apply pathway enrichment
'                                                 analysis of the Metagenome data.
'    /microbiome.metabolic.network:               Construct a metabolic complementation network between
'                                                 the bacterial genomes from a given taxonomy list.
'    /microbiome.pathway.profile:                 Generates the pathway network profile for the microbiome
'                                                 OTU result based on the KEGG and UniProt reference.
'    /microbiome.pathway.run.profile:             Build pathway interaction network based on the microbiome
'                                                 profile result.
'    /UniProt.screen.model:                       
' 
' 
' 5. SILVA database cli tools
' 
' 
'    /SILVA.headers:                              
' 
' 
' 6. Taxonomy assign cli tools
' 
' 
'    /gast.Taxonomy.greengenes:                   OTU taxonomy assign by apply gast method on the result
'                                                 of OTU rep sequence alignment against the greengenes.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
'''
''' </summary>
'''
Public Class metaProfiler : Inherits InteropService

    Public Const App$ = "metaProfiler.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As metaProfiler
          Return New metaProfiler(App:=directory & "/" & metaProfiler.App)
     End Function

''' <summary>
''' ```
''' /ARO.fasta.header.table /in &lt;directory> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function AROSeqTable([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/ARO.fasta.header.table")
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
''' ```
''' /box.plot /in &lt;data.csv> /groups &lt;sampleInfo.csv> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function Boxplot([in] As String, groups As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/box.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/groups " & """" & groups & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /do.enterotype.cluster /in &lt;dataset.csv/txt> [/iterations 50000 /parallel /out &lt;clusters.csv>]
''' ```
''' </summary>
'''
Public Function DoEnterotypeCluster([in] As String, Optional iterations As String = "", Optional out As String = "", Optional parallel As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/do.enterotype.cluster")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not iterations.StringEmpty Then
            Call CLI.Append("/iterations " & """" & iterations & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If parallel Then
        Call CLI.Append("/parallel ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.Megan.BIOM /in &lt;relative.table.csv> [/dense /out &lt;out.biom.json>]
''' ```
''' Export v1.0 biom json file for data visualize in Megan program.
''' </summary>
'''
Public Function ExportToMegan([in] As String, Optional out As String = "", Optional dense As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.Megan.BIOM")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If dense Then
        Call CLI.Append("/dense ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /gast.stat.names /in &lt;*.names> /gast &lt;gast.out> [/out &lt;out.Csv>]
''' ```
''' </summary>
'''
Public Function StateNames([in] As String, gast As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/gast.stat.names")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/gast " & """" & gast & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /gast.Taxonomy.greengenes /in &lt;blastn.txt> /query &lt;OTU.rep.fasta> /taxonomy &lt;97_otu_taxonomy.txt> [/removes.lt &lt;default=0.0001> /gast.consensus /min.pct &lt;default=0.6> /out &lt;gastOut.csv>]
''' ```
''' OTU taxonomy assign by apply gast method on the result of OTU rep sequence alignment against the greengenes.
''' </summary>
'''
Public Function gastTaxonomy_greengenes([in] As String, query As String, taxonomy As String, Optional removes_lt As String = "0.0001", Optional min_pct As String = "0.6", Optional out As String = "", Optional gast_consensus As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/gast.Taxonomy.greengenes")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/taxonomy " & """" & taxonomy & """ ")
    If Not removes_lt.StringEmpty Then
            Call CLI.Append("/removes.lt " & """" & removes_lt & """ ")
    End If
    If Not min_pct.StringEmpty Then
            Call CLI.Append("/min.pct " & """" & min_pct & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If gast_consensus Then
        Call CLI.Append("/gast.consensus ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /handle.hmp.manifest /in &lt;manifest.tsv> [/out &lt;save.directory>]
''' ```
''' Download files from HMP website through http/fasp.
''' </summary>
'''
Public Function Download16sSeq([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/handle.hmp.manifest")
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
''' ```
''' /heatmap.plot /in &lt;data.csv> /groups &lt;sampleInfo.csv> [/schema &lt;default=YlGnBu:c9> /tsv /group /title &lt;title> /size &lt;2700,3000> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function HeatmapPlot([in] As String, groups As String, Optional schema As String = "YlGnBu:c9", Optional title As String = "", Optional size As String = "", Optional out As String = "", Optional tsv As Boolean = False, Optional group As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/heatmap.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/groups " & """" & groups & """ ")
    If Not schema.StringEmpty Then
            Call CLI.Append("/schema " & """" & schema & """ ")
    End If
    If Not title.StringEmpty Then
            Call CLI.Append("/title " & """" & title & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If tsv Then
        Call CLI.Append("/tsv ")
    End If
    If group Then
        Call CLI.Append("/group ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /hmp.manifest.files /in &lt;manifest.tsv> [/out &lt;list.txt>]
''' ```
''' </summary>
'''
Public Function ExportFileList([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/hmp.manifest.files")
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
''' ```
''' /hmp.otu_table /in &lt;download.directory> [/out &lt;out.csv>]
''' ```
''' Export otu table from hmp biom files.
''' </summary>
'''
Public Function ExportsOTUTable([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/hmp.otu_table")
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
''' ```
''' /LefSe.Matrix /in &lt;Species_abundance.csv> /ncbi_taxonomy &lt;NCBI_taxonomy> [/all_rank /out &lt;out.tsv>]
''' ```
''' Processing the relative aboundance matrix to the input format file as it describ: http://huttenhower.sph.harvard.edu/galaxy/root?tool_id=lefse_upload
''' </summary>
'''
Public Function LefSeMatrix([in] As String, ncbi_taxonomy As String, Optional out As String = "", Optional all_rank As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/LefSe.Matrix")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/ncbi_taxonomy " & """" & ncbi_taxonomy & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If all_rank Then
        Call CLI.Append("/all_rank ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Membrane_transport.network /metagenome &lt;list.txt/OTU.tab/biom> /ref &lt;reaction.repository.XML> /uniprot &lt;repository.json> [/out &lt;network.directory>]
''' ```
''' Construct a relationship network based on the Membrane transportor in bacteria genome
''' </summary>
'''
Public Function Membrane_transportNetwork(metagenome As String, ref As String, uniprot As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Membrane_transport.network")
    Call CLI.Append(" ")
    Call CLI.Append("/metagenome " & """" & metagenome & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Metabolic.EndPoint.Profiles.Background /ref &lt;reaction.repository.XML> /uniprot &lt;repository.json> [/out &lt;background.XML>]
''' ```
''' Create Metabolic EndPoint Profiles Background Model
''' </summary>
'''
Public Function MetabolicEndPointProfilesBackground(ref As String, uniprot As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Metabolic.EndPoint.Profiles.Background")
    Call CLI.Append(" ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Metagenome.UniProt.Ref /in &lt;uniprot.ultralarge.xml/cache.directory> [/cache /all /out &lt;out.json>]
''' ```
''' Create background model for apply pathway enrichment analysis of the Metagenome data.
''' </summary>
'''
Public Function BuildUniProtReference([in] As String, Optional out As String = "", Optional cache As Boolean = False, Optional all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Metagenome.UniProt.Ref")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If cache Then
        Call CLI.Append("/cache ")
    End If
    If all Then
        Call CLI.Append("/all ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /microbiome.metabolic.network /metagenome &lt;list.txt/OTU.tab/biom> /ref &lt;reaction.repository.XML> /uniprot &lt;repository.json> /Membrane_transport &lt;Membrane_transport.csv> [/out &lt;network.directory>]
''' ```
''' Construct a metabolic complementation network between the bacterial genomes from a given taxonomy list.
''' </summary>
'''
Public Function MetabolicComplementationNetwork(metagenome As String, ref As String, uniprot As String, Membrane_transport As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/microbiome.metabolic.network")
    Call CLI.Append(" ")
    Call CLI.Append("/metagenome " & """" & metagenome & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/Membrane_transport " & """" & Membrane_transport & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /microbiome.pathway.profile /in &lt;gastout.csv> /ref &lt;UniProt.ref.XML> /maps &lt;kegg.maps.ref.XML> [/just.profiles /rank &lt;default=family> /p.value &lt;default=0.05> /out &lt;out.directory>]
''' ```
''' Generates the pathway network profile for the microbiome OTU result based on the KEGG and UniProt reference.
''' </summary>
'''
Public Function PathwayProfiles([in] As String, ref As String, maps As String, Optional rank As String = "family", Optional p_value As String = "0.05", Optional out As String = "", Optional just_profiles As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/microbiome.pathway.profile")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    If Not rank.StringEmpty Then
            Call CLI.Append("/rank " & """" & rank & """ ")
    End If
    If Not p_value.StringEmpty Then
            Call CLI.Append("/p.value " & """" & p_value & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If just_profiles Then
        Call CLI.Append("/just.profiles ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /microbiome.pathway.run.profile /in &lt;profile.csv> /maps &lt;kegg.maps.ref.Xml> [/p.value &lt;default=0.05> /out &lt;out.directory>]
''' ```
''' Build pathway interaction network based on the microbiome profile result.
''' </summary>
'''
Public Function RunProfile([in] As String, maps As String, Optional p_value As String = "0.05", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/microbiome.pathway.run.profile")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    If Not p_value.StringEmpty Then
            Call CLI.Append("/p.value " & """" & p_value & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /OTU.cluster /left &lt;left.fq> /right &lt;right.fq> /silva &lt;silva.bacteria.fasta> [/out &lt;out.directory> /processors &lt;default=2> /@set mothur=path]
''' ```
''' </summary>
'''
Public Function ClusterOTU(left As String, right As String, silva As String, Optional out As String = "", Optional processors As String = "2", Optional _set As String = "") As Integer
    Dim CLI As New StringBuilder("/OTU.cluster")
    Call CLI.Append(" ")
    Call CLI.Append("/left " & """" & left & """ ")
    Call CLI.Append("/right " & """" & right & """ ")
    Call CLI.Append("/silva " & """" & silva & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not processors.StringEmpty Then
            Call CLI.Append("/processors " & """" & processors & """ ")
    End If
    If Not _set.StringEmpty Then
     Call CLI.Append($"/@set """"--internal_pipeline=TRUE;'{_set}'"""" ")
Else
     Call CLI.Append("/@set --internal_pipeline=TRUE ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Rank_Abundance /in &lt;OTU.table.csv> [/schema &lt;color schema, default=Rainbow> /out &lt;out.DIR>]
''' ```
''' https://en.wikipedia.org/wiki/Rank_abundance_curve
''' </summary>
'''
Public Function Rank_Abundance([in] As String, Optional schema As String = "Rainbow", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Rank_Abundance")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not schema.StringEmpty Then
            Call CLI.Append("/schema " & """" & schema & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Relative_abundance.barplot /in &lt;dataset.csv> [/group &lt;sample_group.csv> /desc /asc /take &lt;-1> /size &lt;3000,2700> /column.n &lt;default=9> /interval &lt;10px> /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function Relative_abundance_barplot([in] As String, Optional group As String = "", Optional take As String = "", Optional size As String = "", Optional column_n As String = "9", Optional interval As String = "", Optional out As String = "", Optional desc As Boolean = False, Optional asc As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Relative_abundance.barplot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not group.StringEmpty Then
            Call CLI.Append("/group " & """" & group & """ ")
    End If
    If Not take.StringEmpty Then
            Call CLI.Append("/take " & """" & take & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not column_n.StringEmpty Then
            Call CLI.Append("/column.n " & """" & column_n & """ ")
    End If
    If Not interval.StringEmpty Then
            Call CLI.Append("/interval " & """" & interval & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If desc Then
        Call CLI.Append("/desc ")
    End If
    If asc Then
        Call CLI.Append("/asc ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Relative_abundance.stacked.barplot /in &lt;dataset.csv> [/group &lt;sample_group.csv> /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function Relative_abundance_stackedbarplot([in] As String, Optional group As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Relative_abundance.stacked.barplot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not group.StringEmpty Then
            Call CLI.Append("/group " & """" & group & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /significant.difference /in &lt;data.csv> /groups &lt;sampleInfo.csv> [/out &lt;out.csv.DIR>]
''' ```
''' </summary>
'''
Public Function SignificantDifference([in] As String, groups As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/significant.difference")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/groups " & """" & groups & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SILVA.bacteria /in &lt;silva.fasta> [/out &lt;silva.bacteria.fasta>]
''' ```
''' </summary>
'''
Public Function SILVABacterial([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/SILVA.bacteria")
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
''' ```
''' /SILVA.headers /in &lt;silva.fasta> /out &lt;headers.tsv>
''' ```
''' </summary>
'''
Public Function SILVA_headers([in] As String, out As String) As Integer
    Dim CLI As New StringBuilder("/SILVA.headers")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /UniProt.screen.model /in &lt;model.Xml> [/coverage &lt;default=0.6> /terms &lt;default=1000> /out &lt;subset.xml>]
''' ```
''' </summary>
'''
Public Function ScreenModels([in] As String, Optional coverage As String = "0.6", Optional terms As String = "1000", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UniProt.screen.model")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not terms.StringEmpty Then
            Call CLI.Append("/terms " & """" & terms & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /UPGMA.Tree /in &lt;in.csv> [/out &lt;>]
''' ```
''' </summary>
'''
Public Function UPGMATree([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UPGMA.Tree")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
