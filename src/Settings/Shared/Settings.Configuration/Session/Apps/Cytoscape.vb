Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/Cytoscape.exe

Namespace GCModellerApps


''' <summary>
'''Cytoscape model generator and visualization tools utils for GCModeller
''' </summary>
'''
Public Class Cytoscape : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
'''
''' </summary>
'''
Public Function Analysis_Graph_Properties(_in As String, Optional _colors As String = "", Optional _ignores As String = "", Optional _tick As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Analysis.Graph.Properties")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _colors.StringEmpty Then
Call CLI.Append("/colors " & """" & _colors & """ ")
End If
If Not _ignores.StringEmpty Then
Call CLI.Append("/ignores " & """" & _ignores & """ ")
End If
If Not _tick.StringEmpty Then
Call CLI.Append("/tick " & """" & _tick & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Analysis_Node_Clusters(_in As String, Optional _size As String = "10000,10000", Optional _schema As String = "", Optional _out As String = "", Optional _spcc As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Analysis.Node.Clusters")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _schema.StringEmpty Then
Call CLI.Append("/schema " & """" & _schema & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _spcc Then
Call CLI.Append("/spcc ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function associate(_in As String, _nodes As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/associate")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/nodes " & """" & _nodes & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function BBH_Simple(_in As String, Optional _evalue As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BBH.Simple")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _evalue.StringEmpty Then
Call CLI.Append("/evalue " & """" & _evalue & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function bbh_Trim_Indeitites(_in As String, Optional _identities As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/bbh.Trim.Indeitites")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''> Viral assemblage composition in Yellowstone acidic hot springs assessed by network analysis, DOI: 10.1038/ismej.2015.28
''' </summary>
'''
Public Function BLAST_Metagenome_SSU_Network(_net As String, _tax As String, _taxonomy As String, Optional _x2taxid As String = "", Optional _theme_color As String = "'Paired:c12'", Optional _identities As String = "", Optional _coverage As String = "", Optional _out As String = "", Optional _tax_build_in As Boolean = False, Optional _skip_exists As Boolean = False, Optional _gi2taxid As Boolean = False, Optional _parallel As Boolean = False) As Integer
Dim CLI As New StringBuilder("/BLAST.Metagenome.SSU.Network")
Call CLI.Append("/net " & """" & _net & """ ")
Call CLI.Append("/tax " & """" & _tax & """ ")
Call CLI.Append("/taxonomy " & """" & _taxonomy & """ ")
If Not _x2taxid.StringEmpty Then
Call CLI.Append("/x2taxid " & """" & _x2taxid & """ ")
End If
If Not _theme_color.StringEmpty Then
Call CLI.Append("/theme-color " & """" & _theme_color & """ ")
End If
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _tax_build_in Then
Call CLI.Append("/tax-build-in ")
End If
If _skip_exists Then
Call CLI.Append("/skip-exists ")
End If
If _gi2taxid Then
Call CLI.Append("/gi2taxid ")
End If
If _parallel Then
Call CLI.Append("/parallel ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function BLAST_Network(_in As String, Optional _out As String = "", Optional _type As String = "", Optional _dict As String = "") As Integer
Dim CLI As New StringBuilder("/BLAST.Network")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _type.StringEmpty Then
Call CLI.Append("/type " & """" & _type & """ ")
End If
If Not _dict.StringEmpty Then
Call CLI.Append("/dict " & """" & _dict & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function BLAST_Network_MetaBuild(_in As String, Optional _out As String = "", Optional _dict As String = "") As Integer
Dim CLI As New StringBuilder("/BLAST.Network.MetaBuild")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _dict.StringEmpty Then
Call CLI.Append("/dict " & """" & _dict & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET(_in As String, Optional _out As String = "", Optional _familyinfo As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Build.Tree.NET")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _familyinfo.StringEmpty Then
Call CLI.Append("/familyinfo " & """" & _familyinfo & """ ")
End If
If _brief Then
Call CLI.Append("/brief ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_COGs(_cluster As String, _COGs As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Build.Tree.NET.COGs")
Call CLI.Append("/cluster " & """" & _cluster & """ ")
Call CLI.Append("/COGs " & """" & _COGs & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_DEGs(_in As String, _up As String, _down As String, Optional _out As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Build.Tree.NET.DEGs")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/up " & """" & _up & """ ")
Call CLI.Append("/down " & """" & _down & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _brief Then
Call CLI.Append("/brief ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_KEGG_Modules(_in As String, _mods As String, Optional _out As String = "", Optional _brief As Boolean = False, Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Build.Tree.NET.KEGG_Modules")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/mods " & """" & _mods & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _brief Then
Call CLI.Append("/brief ")
End If
If _trim Then
Call CLI.Append("/trim ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_KEGG_Pathways(_in As String, _mods As String, Optional _out As String = "", Optional _brief As Boolean = False, Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Build.Tree.NET.KEGG_Pathways")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/mods " & """" & _mods & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _brief Then
Call CLI.Append("/brief ")
End If
If _trim Then
Call CLI.Append("/trim ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_Merged_Regulons(_in As String, _family As String, Optional _out As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Build.Tree.NET.Merged_Regulons")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/family " & """" & _family & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _brief Then
Call CLI.Append("/brief ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_TF(_in As String, _maps As String, _map As String, _mods As String, Optional _out As String = "", Optional _cuts As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Build.Tree.NET.TF")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/maps " & """" & _maps & """ ")
Call CLI.Append("/map " & """" & _map & """ ")
Call CLI.Append("/mods " & """" & _mods & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cuts.StringEmpty Then
Call CLI.Append("/cuts " & """" & _cuts & """ ")
End If
If _brief Then
Call CLI.Append("/brief ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function KEGG_Mods_NET(_in As String, Optional _out As String = "", Optional _footprints As String = "", Optional _cut As String = "", Optional _pcc As String = "", Optional _pathway As Boolean = False, Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("/KEGG.Mods.NET")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _footprints.StringEmpty Then
Call CLI.Append("/footprints " & """" & _footprints & """ ")
End If
If Not _cut.StringEmpty Then
Call CLI.Append("/cut " & """" & _cut & """ ")
End If
If Not _pcc.StringEmpty Then
Call CLI.Append("/pcc " & """" & _pcc & """ ")
End If
If _pathway Then
Call CLI.Append("/pathway ")
End If
If _brief Then
Call CLI.Append("/brief ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function linkage_knowledge_network(_in As String, Optional _schema As String = "", Optional _out As String = "", Optional _no_type_prefix As Boolean = False) As Integer
Dim CLI As New StringBuilder("/linkage.knowledge.network")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _schema.StringEmpty Then
Call CLI.Append("/schema " & """" & _schema & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _no_type_prefix Then
Call CLI.Append("/no-type_prefix ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Converts a generic distance matrix or kmeans clustering result to network model.
''' </summary>
'''
Public Function Matrix_NET(_in As String, Optional _out As String = "", Optional _colors As String = "", Optional _cutoff As String = "", Optional _generic As Boolean = False, Optional _cutoff_paired As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Matrix.NET")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _colors.StringEmpty Then
Call CLI.Append("/colors " & """" & _colors & """ ")
End If
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If
If _generic Then
Call CLI.Append("/generic ")
End If
If _cutoff_paired Then
Call CLI.Append("/cutoff.paired ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function modNET_Simple(_in As String, Optional _out As String = "", Optional _pathway As Boolean = False) As Integer
Dim CLI As New StringBuilder("/modNET.Simple")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _pathway Then
Call CLI.Append("/pathway ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_Cluster(_query As String, _LDM As String, Optional _clusters As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Motif.Cluster")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/LDM " & """" & _LDM & """ ")
If Not _clusters.StringEmpty Then
Call CLI.Append("/clusters " & """" & _clusters & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_Cluster_Fast(_query As String, Optional _ldm As String = "", Optional _out As String = "", Optional _map As String = "", Optional _maxw As String = "", Optional _ldm_loads As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Motif.Cluster.Fast")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _ldm.StringEmpty Then
Call CLI.Append("/ldm " & """" & _ldm & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _map.StringEmpty Then
Call CLI.Append("/map " & """" & _map & """ ")
End If
If Not _maxw.StringEmpty Then
Call CLI.Append("/maxw " & """" & _maxw & """ ")
End If
If _ldm_loads Then
Call CLI.Append("/ldm_loads ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_Cluster_Fast_Sites(_in As String, Optional _out As String = "", Optional _ldm As String = "") As Integer
Dim CLI As New StringBuilder("/Motif.Cluster.Fast.Sites")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _ldm.StringEmpty Then
Call CLI.Append("/ldm " & """" & _ldm & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_Cluster_MAT(_query As String, Optional _ldm As String = "", Optional _clusters As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Motif.Cluster.MAT")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _ldm.StringEmpty Then
Call CLI.Append("/ldm " & """" & _ldm & """ ")
End If
If Not _clusters.StringEmpty Then
Call CLI.Append("/clusters " & """" & _clusters & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function net_model(_model As String, Optional _out As String = "", Optional _not_trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("/net.model")
Call CLI.Append("/model " & """" & _model & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _not_trim Then
Call CLI.Append("/not-trim ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function net_pathway(_model As String, Optional _out As String = "", Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("/net.pathway")
Call CLI.Append("/model " & """" & _model & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _trim Then
Call CLI.Append("/trim ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Net_rFBA(_in As String, _fba_out As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Net.rFBA")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/fba.out " & """" & _fba_out & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Builds the regulation network between the TF.
''' </summary>
'''
Public Function NetModel_TF_regulates(_in As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI As New StringBuilder("/NetModel.TF_regulates")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cut.StringEmpty Then
Call CLI.Append("/cut " & """" & _cut & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Regulator phenotype relationship cluster from virtual footprints.
''' </summary>
'''
Public Function Phenotypes_KEGG(_mods As String, _in As String, Optional _out As String = "", Optional _pathway As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Phenotypes.KEGG")
Call CLI.Append("/mods " & """" & _mods & """ ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _pathway Then
Call CLI.Append("/pathway ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function reaction_NET(Optional _model As String = "", Optional _source As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/reaction.NET")
If Not _model.StringEmpty Then
Call CLI.Append("/model " & """" & _model & """ ")
End If
If Not _source.StringEmpty Then
Call CLI.Append("/source " & """" & _source & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function replace(_in As String, _nodes As String, _out As String) As Integer
Dim CLI As New StringBuilder("/replace")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/nodes " & """" & _nodes & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''This method is not recommended.
''' </summary>
'''
Public Function Tree_Cluster(_in As String, Optional _out As String = "", Optional _locus_map As String = "") As Integer
Dim CLI As New StringBuilder("/Tree.Cluster")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _locus_map.StringEmpty Then
Call CLI.Append("/locus.map " & """" & _locus_map & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Tree_Cluster_rFBA(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Tree.Cluster.rFBA")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Drawing a network image visualization based on the generate network layout from the officials cytoscape software.
''' </summary>
'''
Public Function Draw(_network As String, _parser As String, Optional _size As String = "", Optional _out As String = "", Optional _style As String = "", Optional _style_parser As String = "") As Integer
Dim CLI As New StringBuilder("-Draw")
Call CLI.Append("/network " & """" & _network & """ ")
Call CLI.Append("/parser " & """" & _parser & """ ")
If Not _size.StringEmpty Then
Call CLI.Append("-size " & """" & _size & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("-out " & """" & _out & """ ")
End If
If Not _style.StringEmpty Then
Call CLI.Append("/style " & """" & _style & """ ")
End If
If Not _style_parser.StringEmpty Then
Call CLI.Append("/style_parser " & """" & _style_parser & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function graph_regulates(_footprint As String, Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("--graph.regulates")
Call CLI.Append("/footprint " & """" & _footprint & """ ")
If _trim Then
Call CLI.Append("/trim ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function mod_regulations(_model As String, _footprints As String, _out As String, Optional _pathway As Boolean = False, Optional _class As Boolean = False, Optional _type As Boolean = False) As Integer
Dim CLI As New StringBuilder("--mod.regulations")
Call CLI.Append("/model " & """" & _model & """ ")
Call CLI.Append("/footprints " & """" & _footprints & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If _pathway Then
Call CLI.Append("/pathway ")
End If
If _class Then
Call CLI.Append("/class ")
End If
If _type Then
Call CLI.Append("/type ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TCS(_in As String, _regulations As String, _out As String, Optional _Fill_pcc As Boolean = False) As Integer
Dim CLI As New StringBuilder("--TCS")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/regulations " & """" & _regulations & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If _Fill_pcc Then
Call CLI.Append("/Fill-pcc ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
