#Region "Microsoft.VisualBasic::50821021557adbfffe0054394476bb1d, Shared\InternalApps_CLI\Apps\Cytoscape.vb"

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

    ' Class Cytoscape
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: FromEnvironment
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
'  // VERSION:   3.3277.7609.23646
'  // ASSEMBLY:  Settings, Version=3.3277.7609.23646, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/31/2020 1:08:12 PM
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
'  /KEGG.referenceMap.info:             
'  /linkage.knowledge.network:          
'  /Matrix.NET:                         Converts a generic distance matrix or kmeans clustering result
'                                       to network model.
'  /Motif.Cluster:                      
'  /Motif.Cluster.Fast:                 
'  /Motif.Cluster.Fast.Sites:           
'  /Motif.Cluster.MAT:                  
'  /Plot.Cytoscape.Table:               
'  /renames.kegg.node:                  Update the KEGG compound id and KEGG reaction id as the metabolite
'                                       common name and enzyme gene name.
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
' 3. KEGG reference pathway map visualization
' 
' 
'    /KEGG.referenceMap.Model:            Create network model of KEGG reference pathway map for cytoscape
'                                         data visualization.
'    /KEGG.referenceMap.render:           Render pathway map as image after cytoscape layout progress.
' 
' 
' 4. KEGG tools
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
' 5. MetaCyc pathway network tools
' 
' 
'    /Net.rFBA:                           
' 
' 
' 6. Metagenomics tools
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
' 7. TF/Regulon network tools
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
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Cytoscape
          Return New Cytoscape(App:=directory & "/" & Cytoscape.App)
     End Function

''' <summary>
''' ```bash
''' /Analysis.Graph.Properties /in &lt;net.DIR&gt; [/colors &lt;Paired:c12&gt; /ignores &lt;fields&gt; /tick 5 /out &lt;out.DIR&gt;]
''' ```
''' </summary>
'''

Public Function AnalysisNetworkProperty([in] As String, 
                                           Optional colors As String = "", 
                                           Optional ignores As String = "", 
                                           Optional tick As String = "", 
                                           Optional out As String = "") As Integer
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
''' ```bash
''' /Analysis.Node.Clusters /in &lt;network.DIR&gt; [/spcc /size &quot;10000,10000&quot; /schema &lt;YlGn:c8&gt; /out &lt;DIR&gt;]
''' ```
''' </summary>
'''

Public Function NodeCluster([in] As String, 
                               Optional size As String = "10000,10000", 
                               Optional schema As String = "", 
                               Optional out As String = "", 
                               Optional spcc As Boolean = False) As Integer
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
''' ```bash
''' /associate /in &lt;net.csv&gt; /nodes &lt;nodes.csv&gt; [/out &lt;out.net.DIR&gt;]
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
''' ```bash
''' /BBH.Simple /in &lt;sbh.csv&gt; [/evalue &lt;evalue: 1e-5&gt; /out &lt;out.bbh.csv&gt;]
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
''' ```bash
''' /bbh.Trim.Indeitites /in &lt;bbh.csv&gt; [/identities &lt;0.3&gt; /out &lt;out.csv&gt;]
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
''' ```bash
''' /BLAST.Metagenome.SSU.Network /net &lt;blastn.self.txt/blastn.mapping.csv&gt; /tax &lt;ssu-nt.blastnMaps.csv&gt; /taxonomy &lt;ncbi_taxonomy:names,nodes&gt; [/x2taxid &lt;x2taxid.dmp/DIR&gt; /tax-build-in /skip-exists /gi2taxid /parallel /theme-color &lt;default=&apos;Paired:c12&apos;&gt; /identities &lt;default:0.3&gt; /coverage &lt;default:0.3&gt; /out &lt;out-net.DIR&gt;]
''' ```
''' &gt; Viral assemblage composition in Yellowstone acidic hot springs assessed by network analysis, DOI: 10.1038/ismej.2015.28
''' </summary>
'''
''' <param name="net"> The blastn mapping that you can creates from the self pairwise blastn alignment of your SSU sequence. Using for create the network graph based on the similarity result between the aligned sequnece.
''' </param>
''' <param name="tax"> The blastn mapping that you can creates from the blastn alignment of your SSU sequence against the NCBI nt database.
''' </param>
''' <param name="x2taxid"> NCBI taxonomy database that you can download from the NCBI ftp server.
''' </param>
Public Function SSU_MetagenomeNetwork(net As String, 
                                         tax As String, 
                                         taxonomy As String, 
                                         Optional x2taxid As String = "", 
                                         Optional theme_color As String = "'Paired:c12'", 
                                         Optional identities As String = "", 
                                         Optional coverage As String = "", 
                                         Optional out As String = "", 
                                         Optional tax_build_in As Boolean = False, 
                                         Optional skip_exists As Boolean = False, 
                                         Optional gi2taxid As Boolean = False, 
                                         Optional parallel As Boolean = False) As Integer
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
''' ```bash
''' /BLAST.Network /in &lt;inFile&gt; [/out &lt;outDIR&gt; /type &lt;default:blast_out; values: blast_out, sbh, bbh&gt; /dict &lt;dict.xml&gt;]
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
''' ```bash
''' /BLAST.Network.MetaBuild /in &lt;inDIR&gt; [/out &lt;outDIR&gt; /dict &lt;dict.xml&gt;]
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
''' ```bash
''' /Build.Tree.NET /in &lt;cluster.csv&gt; [/out &lt;outDIR&gt; /brief /FamilyInfo &lt;regulons.DIR&gt;]
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
''' ```bash
''' /Build.Tree.NET.COGs /cluster &lt;cluster.csv&gt; /COGs &lt;myvacog.csv&gt; [/out &lt;outDIR&gt;]
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
''' ```bash
''' /Build.Tree.NET.DEGs /in &lt;cluster.csv&gt; /up &lt;locus.txt&gt; /down &lt;locus.txt&gt; [/out &lt;outDIR&gt; /brief]
''' ```
''' </summary>
'''

Public Function BuildTreeNET_DEGs([in] As String, 
                                     up As String, 
                                     down As String, 
                                     Optional out As String = "", 
                                     Optional brief As Boolean = False) As Integer
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
''' ```bash
''' /Build.Tree.NET.KEGG_Modules /in &lt;cluster.csv&gt; /mods &lt;modules.XML.DIR&gt; [/out &lt;outDIR&gt; /brief /trim]
''' ```
''' </summary>
'''

Public Function BuildTreeNET_KEGGModules([in] As String, 
                                            mods As String, 
                                            Optional out As String = "", 
                                            Optional brief As Boolean = False, 
                                            Optional trim As Boolean = False) As Integer
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
''' ```bash
''' /Build.Tree.NET.KEGG_Pathways /in &lt;cluster.csv&gt; /mods &lt;pathways.XML.DIR&gt; [/out &lt;outDIR&gt; /brief /trim]
''' ```
''' </summary>
'''

Public Function BuildTreeNET_KEGGPathways([in] As String, 
                                             mods As String, 
                                             Optional out As String = "", 
                                             Optional brief As Boolean = False, 
                                             Optional trim As Boolean = False) As Integer
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
''' ```bash
''' /Build.Tree.NET.Merged_Regulons /in &lt;cluster.csv&gt; /family &lt;family_Hits.Csv&gt; [/out &lt;outDIR&gt; /brief]
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
''' ```bash
''' /Build.Tree.NET.TF /in &lt;cluster.csv&gt; /maps &lt;TF.Regprecise.maps.Csv&gt; /map &lt;keyvaluepair.xml&gt; /mods &lt;kegg_modules.DIR&gt; [/out &lt;outDIR&gt; /brief /cuts 0.8]
''' ```
''' </summary>
'''

Public Function BuildTreeNetTF([in] As String, 
                                  maps As String, 
                                  map As String, 
                                  mods As String, 
                                  Optional out As String = "", 
                                  Optional cuts As String = "", 
                                  Optional brief As Boolean = False) As Integer
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
''' ```bash
''' /kegg.compound.network /in &lt;compound.csv&gt; /reactions &lt;reaction_table.csv&gt; [/enzyme &lt;annotation.csv&gt; /extended /enzymeRelated /size &lt;default=10000,7000&gt; /out &lt;network.directory&gt;]
''' ```
''' </summary>
'''
''' <param name="[in]"> The [compound_id =&gt; compound_name] information.
''' </param>
''' <param name="reactions"> A csv table of reaction brief information, which it could be generated from the ``/Write.Reaction.Table`` command.
''' </param>
''' <param name="enzyme"> A protein annotation table which is generated by the ``/protein.annotations`` command in eggHTS tool.
''' </param>
''' <param name="extended"> If the compounds can not create a network model by link each other through reaction model, then you could enable
'''               this argument will makes extension connection for create a compound network model.
''' </param>
Public Function CompoundNetwork([in] As String, 
                                   reactions As String, 
                                   Optional enzyme As String = "", 
                                   Optional size As String = "10000,7000", 
                                   Optional out As String = "", 
                                   Optional extended As Boolean = False, 
                                   Optional enzymerelated As Boolean = False) As Integer
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
''' ```bash
''' /KEGG.Mods.NET /in &lt;mods.xml.DIR&gt; [/out &lt;outDIR&gt; /pathway /footprints &lt;footprints.Csv&gt; /brief /cut 0 /pcc 0]
''' ```
''' </summary>
'''
''' <param name="brief"> If this parameter is represented, then the program just outs the modules, all of the non-pathway genes wil be removes.
''' </param>
Public Function ModsNET([in] As String, 
                           Optional out As String = "", 
                           Optional footprints As String = "", 
                           Optional cut As String = "", 
                           Optional pcc As String = "", 
                           Optional pathway As Boolean = False, 
                           Optional brief As Boolean = False) As Integer
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
''' ```bash
''' /KEGG.pathwayMap.Network /in &lt;br08901.DIR&gt; [/node &lt;nodes.data.csv&gt; /out &lt;out.DIR&gt;]
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
''' ```bash
''' /KEGG.referenceMap.info /model &lt;network.xgmml&gt; /compounds &lt;names.json&gt; /KO &lt;reactionKOMapping.json&gt; [/out &lt;table.csv&gt;]
''' ```
''' </summary>
'''

Public Function NodeInformationTable(model As String, compounds As String, KO As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KEGG.referenceMap.info")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    Call CLI.Append("/compounds " & """" & compounds & """ ")
    Call CLI.Append("/KO " & """" & KO & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

        ''' <summary>
        ''' ```bash
        ''' /KEGG.referenceMap.Model /repository &lt;[reference/organism]kegg_maps.directory&gt; /reactions &lt;kegg_reactions.directory&gt; [/top.priority &lt;map.name.list&gt; /category.level2 /reaction_class &lt;repository&gt; /organism &lt;name&gt; /coverage.cutoff &lt;[0,1], default=0&gt; /delete.unmapped /delete.tupleEdges /split /ignores &lt;compoind idlist&gt; /out &lt;result_network.directory&gt;]
        ''' ```
        ''' Create network model of KEGG reference pathway map for cytoscape data visualization.
        ''' </summary>
        '''
        ''' <param name="repository"> This parameter accept two kind of parameters: The kegg reference map data or organism specific pathway map model data.
        ''' </param>
        ''' <param name="reactions"> The KEGG reference reaction data models.
        ''' </param>
        ''' <param name="organism"> The organism name or code, if this argument presents in the cli command input, then it means 
        '''               the ``/repository`` parameter data model is the organism specific pathway map data.
        ''' </param>
        ''' <param name="out"> The network file data output directory that used for cytoscape network visualization.
        ''' </param>
        ''' <param name="reaction_class"> Apply reaction class filter for reduce network size.
        ''' </param>
        ''' <param name="coverage_cutoff"> The coverage cutoff of the pathway map, cutoff value in range [0,1]. Default value is zero means no cutoff.
        ''' </param>
        ''' <param name="ignores"> A list of kegg compound id list that will be ignores in the generated pathway map model, this optional
        '''               value could be a id list which use the comma symbol as delimiter or an id list file with format of one id per line.
        ''' </param>
        Public Function KEGGReferenceMapModel(repository As String, 
                                         Optional reactions As String = "", 
                                         Optional __top_priority As String = "", 
                                         Optional reaction_class As String = "", 
                                         Optional organism As String = "", 
                                         Optional coverage_cutoff As String = "0", 
                                         Optional ignores As String = "", 
                                         Optional out As String = "", 
                                         Optional category_level2 As Boolean = False, 
                                         Optional delete_unmapped As Boolean = False, 
                                         Optional delete_tupleedges As Boolean = False, 
                                         Optional split As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/KEGG.referenceMap.Model")
    Call CLI.Append(" ")
    Call CLI.Append("/repository " & """" & repository & """ ")
    If Not reactions.StringEmpty Then
            Call CLI.Append("/reactions " & """" & reactions & """ ")
    End If
    If Not __top_priority.StringEmpty Then
            Call CLI.Append("[/top.priority " & """" & __top_priority & """ ")
    End If
    If Not reaction_class.StringEmpty Then
            Call CLI.Append("/reaction_class " & """" & reaction_class & """ ")
    End If
    If Not organism.StringEmpty Then
            Call CLI.Append("/organism " & """" & organism & """ ")
    End If
    If Not coverage_cutoff.StringEmpty Then
            Call CLI.Append("/coverage.cutoff " & """" & coverage_cutoff & """ ")
    End If
    If Not ignores.StringEmpty Then
            Call CLI.Append("/ignores " & """" & ignores & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If category_level2 Then
        Call CLI.Append("/category.level2 ")
    End If
    If delete_unmapped Then
        Call CLI.Append("/delete.unmapped ")
    End If
    If delete_tupleedges Then
        Call CLI.Append("/delete.tupleedges ")
    End If
    If split Then
        Call CLI.Append("/split ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KEGG.referenceMap.render /model &lt;network.xgmml/directory&gt; [/edge.bends /compounds &lt;names.json&gt; /KO &lt;reactionKOMapping.json&gt; /convexHull &lt;category.txt&gt; /style2 /size &lt;10(A0)&gt; /out &lt;viz.png&gt;]
''' ```
''' Render pathway map as image after cytoscape layout progress.
''' </summary>
'''
''' <param name="compounds"> The kegg compound id to its command names mapping table file. 
'''               Content in this table file should be ``Cid -&gt; name``, which could be created 
'''               by using ``/compound.names`` command from ``kegg_tools``.
''' </param>
Public Function RenderReferenceMapNetwork(model As String, 
                                             Optional compounds As String = "", 
                                             Optional ko As String = "", 
                                             Optional convexhull As String = "", 
                                             Optional size As String = "", 
                                             Optional out As String = "", 
                                             Optional edge_bends As Boolean = False, 
                                             Optional style2 As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/KEGG.referenceMap.render")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    If Not compounds.StringEmpty Then
            Call CLI.Append("/compounds " & """" & compounds & """ ")
    End If
    If Not ko.StringEmpty Then
            Call CLI.Append("/ko " & """" & ko & """ ")
    End If
    If Not convexhull.StringEmpty Then
            Call CLI.Append("/convexhull " & """" & convexhull & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If edge_bends Then
        Call CLI.Append("/edge.bends ")
    End If
    If style2 Then
        Call CLI.Append("/style2 ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KO.link /in &lt;ko00001.DIR&gt; [/out &lt;out.XML&gt;]
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
''' ```bash
''' /linkage.knowledge.network /in &lt;knowledge.network.csv/DIR&gt; [/schema &lt;material&gt; /no-type_prefix /out &lt;out.network.DIR&gt;]
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
''' ```bash
''' /Matrix.NET /in &lt;kmeans-out.csv&gt; [/out &lt;net.DIR&gt; /generic /colors &lt;clusters&gt; /cutoff 0 /cutoff.paired]
''' ```
''' Converts a generic distance matrix or kmeans clustering result to network model.
''' </summary>
'''
''' <param name="[in]">
''' </param>
''' <param name="generic"> If this argument parameter was presents, then the &quot;/in&quot; input data is a generic matrix(DataSet) type, otherwise is a kmeans output result csv file.
''' </param>
Public Function MatrixToNetwork([in] As String, 
                                   Optional out As String = "", 
                                   Optional colors As String = "", 
                                   Optional cutoff As String = "", 
                                   Optional generic As Boolean = False, 
                                   Optional cutoff_paired As Boolean = False) As Integer
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
''' ```bash
''' /modNET.Simple /in &lt;mods/pathway_DIR&gt; [/out &lt;outDIR&gt; /pathway]
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
''' ```bash
''' /Motif.Cluster /query &lt;meme.txt/MEME_OUT.DIR&gt; /LDM &lt;LDM-name/xml.path&gt; [/clusters &lt;3&gt; /out &lt;outCsv&gt;]
''' ```
''' </summary>
'''
''' <param name="clusters"> If the expects clusters number is greater than the maps number, then the maps number divid 2 is used.
''' </param>
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
''' ```bash
''' /Motif.Cluster.Fast /query &lt;meme_OUT.DIR&gt; [/LDM &lt;ldm-DIR&gt; /out &lt;outDIR&gt; /map &lt;gb.gbk&gt; /maxw -1 /ldm_loads]
''' ```
''' </summary>
'''
''' <param name="maxw"> If this parameter value is not set, then no motif in the query will be filterd, or all of the width greater then the width value will be removed.
'''                    If a filterd is necessary, value of 52 nt is recommended as the max width of the motif in the RegPrecise database is 52.
''' </param>
Public Function FastCluster(query As String, 
                               Optional ldm As String = "", 
                               Optional out As String = "", 
                               Optional map As String = "", 
                               Optional maxw As String = "", 
                               Optional ldm_loads As Boolean = False) As Integer
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
''' ```bash
''' /Motif.Cluster.Fast.Sites /in &lt;meme.txt.DIR&gt; [/out &lt;outDIR&gt; /LDM &lt;ldm-DIR&gt;]
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
''' ```bash
''' /Motif.Cluster.MAT /query &lt;meme_OUT.DIR&gt; [/LDM &lt;ldm-DIR&gt; /clusters 5 /out &lt;outDIR&gt;]
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
''' ```bash
''' /net.model /model &lt;kegg.xmlModel.xml&gt; [/out &lt;outDIR&gt; /not-trim]
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
''' ```bash
''' /net.pathway /model &lt;kegg.pathway.xml&gt; [/out &lt;outDIR&gt; /trim]
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
''' ```bash
''' /Net.rFBA /in &lt;metacyc.sbml&gt; /fba.out &lt;flux.Csv&gt; [/out &lt;outDIR&gt;]
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
''' ```bash
''' /NetModel.TF_regulates /in &lt;footprints.csv&gt; [/out &lt;outDIR&gt; /cut 0.45]
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
''' ```bash
''' /Phenotypes.KEGG /mods &lt;KEGG_Modules/Pathways.DIR&gt; /in &lt;VirtualFootprints.csv&gt; [/pathway /out &lt;outCluster.csv&gt;]
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
''' ```bash
''' /Plot.Cytoscape.Table /in &lt;table.csv&gt; [/size &lt;default=1600,1440&gt; /out &lt;out.DIR&gt;]
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
''' ```bash
''' /reaction.NET [/model &lt;xmlModel.xml&gt; /source &lt;rxn.DIR&gt; /out &lt;outDIR&gt;]
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
''' ```bash
''' /renames.kegg.node /network &lt;tables.csv.directory&gt; /compounds &lt;names.json&gt; /KO &lt;reactionKOMapping.json&gt; [/out &lt;output_renames.directory&gt;]
''' ```
''' Update the KEGG compound id and KEGG reaction id as the metabolite common name and enzyme gene name.
''' </summary>
'''

Public Function RenamesKEGGNode(network As String, compounds As String, KO As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/renames.kegg.node")
    Call CLI.Append(" ")
    Call CLI.Append("/network " & """" & network & """ ")
    Call CLI.Append("/compounds " & """" & compounds & """ ")
    Call CLI.Append("/KO " & """" & KO & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /replace /in &lt;net.csv&gt; /nodes &lt;nodes.Csv&gt; /out &lt;out.Csv&gt;
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
''' ```bash
''' /Tree.Cluster /in &lt;in.MAT.csv&gt; [/out &lt;out.cluster.csv&gt; /Locus.Map &lt;Name&gt;]
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
''' ```bash
''' /Tree.Cluster.rFBA /in &lt;in.flux.pheno_OUT.Csv&gt; [/out &lt;out.cluster.csv&gt;]
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
''' ```bash
''' /Write.Compounds.Table /in &lt;kegg_compounds.DIR&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
''' /Write.Reaction.Table /in &lt;br08201.DIR&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
''' -draw /network &lt;net_file&gt; /parser &lt;xgmml/cyjs&gt; [-size &lt;width,height&gt; -out &lt;out_image&gt; /style &lt;style_file&gt; /style_parser &lt;vizmap/json&gt;]
''' ```
''' Drawing a network image visualization based on the generate network layout from the officials cytoscape software.
''' </summary>
'''

Public Function DrawingInvoke(network As String, 
                                 parser As String, 
                                 Optional size As String = "", 
                                 Optional out As String = "", 
                                 Optional style As String = "", 
                                 Optional style_parser As String = "") As Integer
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
''' ```bash
''' --graph.regulates /footprint &lt;footprints.csv&gt; [/trim]
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
''' ```bash
''' --mod.regulations /model &lt;KEGG.xml&gt; /footprints &lt;footprints.csv&gt; /out &lt;outDIR&gt; [/pathway /class /type]
''' ```
''' </summary>
'''
''' <param name="[class]"> This parameter can not be co-exists with ``/type`` parameter
''' </param>
''' <param name="type"> This parameter can not be co-exists with ``/class`` parameter
''' </param>
Public Function ModuleRegulations(model As String, 
                                     footprints As String, 
                                     out As String, 
                                     Optional pathway As Boolean = False, 
                                     Optional [class] As Boolean = False, 
                                     Optional type As Boolean = False) As Integer
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
''' ```bash
''' --TCS /in &lt;TCS.csv.DIR&gt; /regulations &lt;TCS.virtualfootprints&gt; /out &lt;outForCytoscape.xml&gt; [/Fill-pcc]
''' ```
''' </summary>
'''
''' <param name="Fill_pcc"> If the predicted regulation data did&apos;nt contains pcc correlation value, then you can using this parameter to fill default value 0.6 or just left it default as ZERO
''' </param>
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

