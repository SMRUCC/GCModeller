Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\Cytoscape.exe

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
'  Cytoscape model generator and visualization tools utils for GCModeller
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /Analysis.Graph.Properties:          
'  /Analysis.Node.Clusters:             
'  /associate:                          
'  /Build.Tree.NET:                     
'  /Build.Tree.NET.COGs:                
'  /Build.Tree.NET.DEGs:                
'  /Build.Tree.NET.KEGG_Modules:        
'  /Build.Tree.NET.KEGG_Pathways:       
'  /Build.Tree.NET.Merged_Regulons:     
'  /Build.Tree.NET.TF:                  
'  /kegg.compound.network:              
'  /linkage.knowledge.network:          
'  /Matrix.NET:                         Converts a generic distance matrix or kmeans clustering result
'                                       to network model.
'  /Motif.Cluster:                      
'  /Motif.Cluster.Fast:                 
'  /Motif.Cluster.Fast.Sites:           
'  /Motif.Cluster.MAT:                  
'  /Plot.Cytoscape.Table:               
'  /replace:                            
'  /Tree.Cluster:                       This method is not recommended.
'  /Tree.Cluster.rFBA:                  
'  -Draw:                               Drawing a network image visualization based on the generate
'                                       network layout from the officials cytoscape software.
' 
' 
' API list that with functional grouping
' 
' 1. Bacterial TCS network tools
' 
' 
'    --TCS:                               
' 
' 
' 2. KEGG phenotype network analysis tools
' 
'    Associates the KEGG pathway category information with the gene annotations.
' 
' 
'    /modNET.Simple:                      
'    /net.model:                          
'    /net.pathway:                        
'    /Phenotypes.KEGG:                    Regulator phenotype relationship cluster from virtual footprints.
' 
' 
' 3. KEGG tools
' 
' 
'    /KEGG.Mods.NET:                      
'    /KEGG.pathwayMap.Network:            
'    /KO.link:                            
'    /reaction.NET:                       
'    /Write.Compounds.Table:              
'    /Write.Reaction.Table:               
'    --mod.regulations:                   
' 
' 
' 4. MetaCyc pathway network tools
' 
' 
'    /Net.rFBA:                           
' 
' 
' 5. Metagenomics tools
' 
' 
'    /BBH.Simple:                         
'    /bbh.Trim.Indeitites:                
'    /BLAST.Metagenome.SSU.Network:       > Viral assemblage composition in Yellowstone acidic hot springs
'                                         assessed by network analysis, DOI: 10.1038/ismej.2015.28
'    /BLAST.Network:                      
'    /BLAST.Network.MetaBuild:            
' 
' 
' 6. TF/Regulon network tools
' 
' 
'    /NetModel.TF_regulates:              Builds the regulation network between the TF.
'    --graph.regulates:                   
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Cytoscape model generator and visualization tools utils for GCModeller
''' </summary>
'''
Public Class Cytoscape : Inherits InteropService

    Public Const App$ = "Cytoscape.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Cytoscape
          Return New Cytoscape(App:=directory & "/" & Cytoscape.App)
     End Function

''' <summary>
''' ```
''' /Analysis.Graph.Properties /in &lt;net.DIR> [/colors &lt;Paired:c12> /ignores &lt;fields> /tick 5 /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function AnalysisNetworkProperty([in] As String, Optional colors As String = "", Optional ignores As String = "", Optional tick As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Analysis.Graph.Properties")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not ignores.StringEmpty Then
            Call CLI.Append("/ignores " & """" & ignores & """ ")
    End If
    If Not tick.StringEmpty Then
            Call CLI.Append("/tick " & """" & tick & """ ")
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
''' /Analysis.Node.Clusters /in &lt;network.DIR> [/spcc /size "10000,10000" /schema &lt;YlGn:c8> /out &lt;DIR>]
''' ```
''' </summary>
'''
Public Function NodeCluster([in] As String, Optional size As String = "10000,10000", Optional schema As String = "", Optional out As String = "", Optional spcc As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Analysis.Node.Clusters")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not schema.StringEmpty Then
            Call CLI.Append("/schema " & """" & schema & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If spcc Then
        Call CLI.Append("/spcc ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /associate /in &lt;net.csv> /nodes &lt;nodes.csv> [/out &lt;out.net.DIR>]
''' ```
''' </summary>
'''
Public Function Assciates([in] As String, nodes As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/associate")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/nodes " & """" & nodes & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /BBH.Simple /in &lt;sbh.csv> [/evalue &lt;evalue: 1e-5> /out &lt;out.bbh.csv>]
''' ```
''' </summary>
'''
Public Function SimpleBBH([in] As String, Optional evalue As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/BBH.Simple")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
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
''' /bbh.Trim.Indeitites /in &lt;bbh.csv> [/identities &lt;0.3> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function BBHTrimIdentities([in] As String, Optional identities As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/bbh.Trim.Indeitites")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
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
''' /BLAST.Metagenome.SSU.Network /net &lt;blastn.self.txt/blastn.mapping.csv> /tax &lt;ssu-nt.blastnMaps.csv> /taxonomy &lt;ncbi_taxonomy:names,nodes> [/x2taxid &lt;x2taxid.dmp/DIR> /tax-build-in /skip-exists /gi2taxid /parallel /theme-color &lt;default='Paired:c12'> /identities &lt;default:0.3> /coverage &lt;default:0.3> /out &lt;out-net.DIR>]
''' ```
''' > Viral assemblage composition in Yellowstone acidic hot springs assessed by network analysis, DOI: 10.1038/ismej.2015.28
''' </summary>
'''
Public Function SSU_MetagenomeNetwork(net As String, tax As String, taxonomy As String, Optional x2taxid As String = "", Optional theme_color As String = "'Paired:c12'", Optional identities As String = "", Optional coverage As String = "", Optional out As String = "", Optional tax_build_in As Boolean = False, Optional skip_exists As Boolean = False, Optional gi2taxid As Boolean = False, Optional parallel As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/BLAST.Metagenome.SSU.Network")
    Call CLI.Append(" ")
    Call CLI.Append("/net " & """" & net & """ ")
    Call CLI.Append("/tax " & """" & tax & """ ")
    Call CLI.Append("/taxonomy " & """" & taxonomy & """ ")
    If Not x2taxid.StringEmpty Then
            Call CLI.Append("/x2taxid " & """" & x2taxid & """ ")
    End If
    If Not theme_color.StringEmpty Then
            Call CLI.Append("/theme-color " & """" & theme_color & """ ")
    End If
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If tax_build_in Then
        Call CLI.Append("/tax-build-in ")
    End If
    If skip_exists Then
        Call CLI.Append("/skip-exists ")
    End If
    If gi2taxid Then
        Call CLI.Append("/gi2taxid ")
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
''' /BLAST.Network /in &lt;inFile> [/out &lt;outDIR> /type &lt;default:blast_out; values: blast_out, sbh, bbh> /dict &lt;dict.xml>]
''' ```
''' </summary>
'''
Public Function GenerateBlastNetwork([in] As String, Optional out As String = "", Optional type As String = "", Optional dict As String = "") As Integer
    Dim CLI As New StringBuilder("/BLAST.Network")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not type.StringEmpty Then
            Call CLI.Append("/type " & """" & type & """ ")
    End If
    If Not dict.StringEmpty Then
            Call CLI.Append("/dict " & """" & dict & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /BLAST.Network.MetaBuild /in &lt;inDIR> [/out &lt;outDIR> /dict &lt;dict.xml>]
''' ```
''' </summary>
'''
Public Function MetaBuildBLAST([in] As String, Optional out As String = "", Optional dict As String = "") As Integer
    Dim CLI As New StringBuilder("/BLAST.Network.MetaBuild")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not dict.StringEmpty Then
            Call CLI.Append("/dict " & """" & dict & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.Tree.NET /in &lt;cluster.csv> [/out &lt;outDIR> /brief /FamilyInfo &lt;regulons.DIR>]
''' ```
''' </summary>
'''
Public Function BuildTreeNET([in] As String, Optional out As String = "", Optional familyinfo As String = "", Optional brief As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Build.Tree.NET")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not familyinfo.StringEmpty Then
            Call CLI.Append("/familyinfo " & """" & familyinfo & """ ")
    End If
    If brief Then
        Call CLI.Append("/brief ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.Tree.NET.COGs /cluster &lt;cluster.csv> /COGs &lt;myvacog.csv> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function BuildTreeNETCOGs(cluster As String, COGs As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Build.Tree.NET.COGs")
    Call CLI.Append(" ")
    Call CLI.Append("/cluster " & """" & cluster & """ ")
    Call CLI.Append("/COGs " & """" & COGs & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.Tree.NET.DEGs /in &lt;cluster.csv> /up &lt;locus.txt> /down &lt;locus.txt> [/out &lt;outDIR> /brief]
''' ```
''' </summary>
'''
Public Function BuildTreeNET_DEGs([in] As String, up As String, down As String, Optional out As String = "", Optional brief As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Build.Tree.NET.DEGs")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/up " & """" & up & """ ")
    Call CLI.Append("/down " & """" & down & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If brief Then
        Call CLI.Append("/brief ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.Tree.NET.KEGG_Modules /in &lt;cluster.csv> /mods &lt;modules.XML.DIR> [/out &lt;outDIR> /brief /trim]
''' ```
''' </summary>
'''
Public Function BuildTreeNET_KEGGModules([in] As String, mods As String, Optional out As String = "", Optional brief As Boolean = False, Optional trim As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Build.Tree.NET.KEGG_Modules")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/mods " & """" & mods & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If brief Then
        Call CLI.Append("/brief ")
    End If
    If trim Then
        Call CLI.Append("/trim ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.Tree.NET.KEGG_Pathways /in &lt;cluster.csv> /mods &lt;pathways.XML.DIR> [/out &lt;outDIR> /brief /trim]
''' ```
''' </summary>
'''
Public Function BuildTreeNET_KEGGPathways([in] As String, mods As String, Optional out As String = "", Optional brief As Boolean = False, Optional trim As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Build.Tree.NET.KEGG_Pathways")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/mods " & """" & mods & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If brief Then
        Call CLI.Append("/brief ")
    End If
    If trim Then
        Call CLI.Append("/trim ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.Tree.NET.Merged_Regulons /in &lt;cluster.csv> /family &lt;family_Hits.Csv> [/out &lt;outDIR> /brief]
''' ```
''' </summary>
'''
Public Function BuildTreeNET_MergeRegulons([in] As String, family As String, Optional out As String = "", Optional brief As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Build.Tree.NET.Merged_Regulons")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/family " & """" & family & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If brief Then
        Call CLI.Append("/brief ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.Tree.NET.TF /in &lt;cluster.csv> /maps &lt;TF.Regprecise.maps.Csv> /map &lt;keyvaluepair.xml> /mods &lt;kegg_modules.DIR> [/out &lt;outDIR> /brief /cuts 0.8]
''' ```
''' </summary>
'''
Public Function BuildTreeNetTF([in] As String, maps As String, map As String, mods As String, Optional out As String = "", Optional cuts As String = "", Optional brief As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Build.Tree.NET.TF")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    Call CLI.Append("/map " & """" & map & """ ")
    Call CLI.Append("/mods " & """" & mods & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cuts.StringEmpty Then
            Call CLI.Append("/cuts " & """" & cuts & """ ")
    End If
    If brief Then
        Call CLI.Append("/brief ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /kegg.compound.network /in &lt;compound.csv> /reactions &lt;reaction_table.csv> [/enzyme &lt;annotation.csv> /extended /enzymeRelated /size &lt;default=10000,7000> /out &lt;network.directory>]
''' ```
''' </summary>
'''
Public Function CompoundNetwork([in] As String, reactions As String, Optional enzyme As String = "", Optional size As String = "10000,7000", Optional out As String = "", Optional extended As Boolean = False, Optional enzymerelated As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/kegg.compound.network")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/reactions " & """" & reactions & """ ")
    If Not enzyme.StringEmpty Then
            Call CLI.Append("/enzyme " & """" & enzyme & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If extended Then
        Call CLI.Append("/extended ")
    End If
    If enzymerelated Then
        Call CLI.Append("/enzymerelated ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /KEGG.Mods.NET /in &lt;mods.xml.DIR> [/out &lt;outDIR> /pathway /footprints &lt;footprints.Csv> /brief /cut 0 /pcc 0]
''' ```
''' </summary>
'''
Public Function ModsNET([in] As String, Optional out As String = "", Optional footprints As String = "", Optional cut As String = "", Optional pcc As String = "", Optional pathway As Boolean = False, Optional brief As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/KEGG.Mods.NET")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not footprints.StringEmpty Then
            Call CLI.Append("/footprints " & """" & footprints & """ ")
    End If
    If Not cut.StringEmpty Then
            Call CLI.Append("/cut " & """" & cut & """ ")
    End If
    If Not pcc.StringEmpty Then
            Call CLI.Append("/pcc " & """" & pcc & """ ")
    End If
    If pathway Then
        Call CLI.Append("/pathway ")
    End If
    If brief Then
        Call CLI.Append("/brief ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /KEGG.pathwayMap.Network /in &lt;br08901.DIR> [/node &lt;nodes.data.csv> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function KEGGPathwayMapNetwork([in] As String, Optional node As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KEGG.pathwayMap.Network")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not node.StringEmpty Then
            Call CLI.Append("/node " & """" & node & """ ")
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
''' /KO.link /in &lt;ko00001.DIR> [/out &lt;out.XML>]
''' ```
''' </summary>
'''
Public Function BuildKOLinks([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KO.link")
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
''' /linkage.knowledge.network /in &lt;knowledge.network.csv/DIR> [/schema &lt;material> /no-type_prefix /out &lt;out.network.DIR>]
''' ```
''' </summary>
'''
Public Function LinkageKnowledgeNetwork([in] As String, Optional schema As String = "", Optional out As String = "", Optional no_type_prefix As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/linkage.knowledge.network")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not schema.StringEmpty Then
            Call CLI.Append("/schema " & """" & schema & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If no_type_prefix Then
        Call CLI.Append("/no-type_prefix ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Matrix.NET /in &lt;kmeans-out.csv> [/out &lt;net.DIR> /generic /colors &lt;clusters> /cutoff 0 /cutoff.paired]
''' ```
''' Converts a generic distance matrix or kmeans clustering result to network model.
''' </summary>
'''
Public Function MatrixToNetwork([in] As String, Optional out As String = "", Optional colors As String = "", Optional cutoff As String = "", Optional generic As Boolean = False, Optional cutoff_paired As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Matrix.NET")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If generic Then
        Call CLI.Append("/generic ")
    End If
    If cutoff_paired Then
        Call CLI.Append("/cutoff.paired ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /modNET.Simple /in &lt;mods/pathway_DIR> [/out &lt;outDIR> /pathway]
''' ```
''' </summary>
'''
Public Function SimpleModesNET([in] As String, Optional out As String = "", Optional pathway As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/modNET.Simple")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If pathway Then
        Call CLI.Append("/pathway ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Motif.Cluster /query &lt;meme.txt/MEME_OUT.DIR> /LDM &lt;LDM-name/xml.path> [/clusters &lt;3> /out &lt;outCsv>]
''' ```
''' </summary>
'''
Public Function MotifCluster(query As String, LDM As String, Optional clusters As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Motif.Cluster")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/LDM " & """" & LDM & """ ")
    If Not clusters.StringEmpty Then
            Call CLI.Append("/clusters " & """" & clusters & """ ")
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
''' /Motif.Cluster.Fast /query &lt;meme_OUT.DIR> [/LDM &lt;ldm-DIR> /out &lt;outDIR> /map &lt;gb.gbk> /maxw -1 /ldm_loads]
''' ```
''' </summary>
'''
Public Function FastCluster(query As String, Optional ldm As String = "", Optional out As String = "", Optional map As String = "", Optional maxw As String = "", Optional ldm_loads As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Motif.Cluster.Fast")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not ldm.StringEmpty Then
            Call CLI.Append("/ldm " & """" & ldm & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not map.StringEmpty Then
            Call CLI.Append("/map " & """" & map & """ ")
    End If
    If Not maxw.StringEmpty Then
            Call CLI.Append("/maxw " & """" & maxw & """ ")
    End If
    If ldm_loads Then
        Call CLI.Append("/ldm_loads ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Motif.Cluster.Fast.Sites /in &lt;meme.txt.DIR> [/out &lt;outDIR> /LDM &lt;ldm-DIR>]
''' ```
''' </summary>
'''
Public Function MotifClusterSites([in] As String, Optional out As String = "", Optional ldm As String = "") As Integer
    Dim CLI As New StringBuilder("/Motif.Cluster.Fast.Sites")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not ldm.StringEmpty Then
            Call CLI.Append("/ldm " & """" & ldm & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Motif.Cluster.MAT /query &lt;meme_OUT.DIR> [/LDM &lt;ldm-DIR> /clusters 5 /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ClusterMatrix(query As String, Optional ldm As String = "", Optional clusters As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Motif.Cluster.MAT")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not ldm.StringEmpty Then
            Call CLI.Append("/ldm " & """" & ldm & """ ")
    End If
    If Not clusters.StringEmpty Then
            Call CLI.Append("/clusters " & """" & clusters & """ ")
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
''' /net.model /model &lt;kegg.xmlModel.xml> [/out &lt;outDIR> /not-trim]
''' ```
''' </summary>
'''
Public Function BuildModelNet(model As String, Optional out As String = "", Optional not_trim As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/net.model")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If not_trim Then
        Call CLI.Append("/not-trim ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /net.pathway /model &lt;kegg.pathway.xml> [/out &lt;outDIR> /trim]
''' ```
''' </summary>
'''
Public Function PathwayNet(model As String, Optional out As String = "", Optional trim As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/net.pathway")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If trim Then
        Call CLI.Append("/trim ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Net.rFBA /in &lt;metacyc.sbml> /fba.out &lt;flux.Csv> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function net_rFBA([in] As String, fba_out As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Net.rFBA")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/fba.out " & """" & fba_out & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /NetModel.TF_regulates /in &lt;footprints.csv> [/out &lt;outDIR> /cut 0.45]
''' ```
''' Builds the regulation network between the TF.
''' </summary>
'''
Public Function TFNet([in] As String, Optional out As String = "", Optional cut As String = "") As Integer
    Dim CLI As New StringBuilder("/NetModel.TF_regulates")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cut.StringEmpty Then
            Call CLI.Append("/cut " & """" & cut & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Phenotypes.KEGG /mods &lt;KEGG_Modules/Pathways.DIR> /in &lt;VirtualFootprints.csv> [/pathway /out &lt;outCluster.csv>]
''' ```
''' Regulator phenotype relationship cluster from virtual footprints.
''' </summary>
'''
Public Function KEGGModulesPhenotypeRegulates(mods As String, [in] As String, Optional out As String = "", Optional pathway As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Phenotypes.KEGG")
    Call CLI.Append(" ")
    Call CLI.Append("/mods " & """" & mods & """ ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If pathway Then
        Call CLI.Append("/pathway ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Plot.Cytoscape.Table /in &lt;table.csv> [/size &lt;default=1600,1440> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function PlotCytoscapeTable([in] As String, Optional size As String = "1600,1440", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Plot.Cytoscape.Table")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
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
''' ```
''' /reaction.NET [/model &lt;xmlModel.xml> /source &lt;rxn.DIR> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ReactionNET(Optional model As String = "", Optional source As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/reaction.NET")
    Call CLI.Append(" ")
    If Not model.StringEmpty Then
            Call CLI.Append("/model " & """" & model & """ ")
    End If
    If Not source.StringEmpty Then
            Call CLI.Append("/source " & """" & source & """ ")
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
''' /replace /in &lt;net.csv> /nodes &lt;nodes.Csv> /out &lt;out.Csv>
''' ```
''' </summary>
'''
Public Function replaceName([in] As String, nodes As String, out As String) As Integer
    Dim CLI As New StringBuilder("/replace")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/nodes " & """" & nodes & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Tree.Cluster /in &lt;in.MAT.csv> [/out &lt;out.cluster.csv> /Locus.Map &lt;Name>]
''' ```
''' This method is not recommended.
''' </summary>
'''
Public Function TreeCluster([in] As String, Optional out As String = "", Optional locus_map As String = "") As Integer
    Dim CLI As New StringBuilder("/Tree.Cluster")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not locus_map.StringEmpty Then
            Call CLI.Append("/locus.map " & """" & locus_map & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Tree.Cluster.rFBA /in &lt;in.flux.pheno_OUT.Csv> [/out &lt;out.cluster.csv>]
''' ```
''' </summary>
'''
Public Function rFBATreeCluster([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Tree.Cluster.rFBA")
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
''' /Write.Compounds.Table /in &lt;kegg_compounds.DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function WriteKEGGCompoundsSummary([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Write.Compounds.Table")
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
''' /Write.Reaction.Table /in &lt;br08201.DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function WriteReactionTable([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Write.Reaction.Table")
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
''' -draw /network &lt;net_file> /parser &lt;xgmml/cyjs> [-size &lt;width,height> -out &lt;out_image> /style &lt;style_file> /style_parser &lt;vizmap/json>]
''' ```
''' Drawing a network image visualization based on the generate network layout from the officials cytoscape software.
''' </summary>
'''
Public Function DrawingInvoke(network As String, parser As String, Optional size As String = "", Optional out As String = "", Optional style As String = "", Optional style_parser As String = "") As Integer
    Dim CLI As New StringBuilder("-draw")
    Call CLI.Append(" ")
    Call CLI.Append("/network " & """" & network & """ ")
    Call CLI.Append("/parser " & """" & parser & """ ")
    If Not size.StringEmpty Then
            Call CLI.Append("-size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("-out " & """" & out & """ ")
    End If
    If Not style.StringEmpty Then
            Call CLI.Append("/style " & """" & style & """ ")
    End If
    If Not style_parser.StringEmpty Then
            Call CLI.Append("/style_parser " & """" & style_parser & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --graph.regulates /footprint &lt;footprints.csv> [/trim]
''' ```
''' </summary>
'''
Public Function SimpleRegulation(footprint As String, Optional trim As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--graph.regulates")
    Call CLI.Append(" ")
    Call CLI.Append("/footprint " & """" & footprint & """ ")
    If trim Then
        Call CLI.Append("/trim ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --mod.regulations /model &lt;KEGG.xml> /footprints &lt;footprints.csv> /out &lt;outDIR> [/pathway /class /type]
''' ```
''' </summary>
'''
Public Function ModuleRegulations(model As String, footprints As String, out As String, Optional pathway As Boolean = False, Optional [class] As Boolean = False, Optional type As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--mod.regulations")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    Call CLI.Append("/footprints " & """" & footprints & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If pathway Then
        Call CLI.Append("/pathway ")
    End If
    If [class] Then
        Call CLI.Append("/class ")
    End If
    If type Then
        Call CLI.Append("/type ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --TCS /in &lt;TCS.csv.DIR> /regulations &lt;TCS.virtualfootprints> /out &lt;outForCytoscape.xml> [/Fill-pcc]
''' ```
''' </summary>
'''
Public Function TCS([in] As String, regulations As String, out As String, Optional fill_pcc As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--TCS")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/regulations " & """" & regulations & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If fill_pcc Then
        Call CLI.Append("/fill-pcc ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
