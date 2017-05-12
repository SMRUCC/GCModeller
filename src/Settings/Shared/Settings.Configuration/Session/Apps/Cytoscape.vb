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
Dim CLI$ = $"/Analysis.Graph.Properties /in ""{_in}"" /colors ""{_colors}"" /ignores ""{_ignores}"" /tick ""{_tick}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Analysis_Node_Clusters(_in As String, Optional _size As String = "10000,10000", Optional _schema As String = "", Optional _out As String = "", Optional _spcc As Boolean = False) As Integer
Dim CLI$ = $"/Analysis.Node.Clusters /in ""{_in}"" /size ""{_size}"" /schema ""{_schema}"" /out ""{_out}"" {If(_spcc, "/spcc", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function associate(_in As String, _nodes As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/associate /in ""{_in}"" /nodes ""{_nodes}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function BBH_Simple(_in As String, Optional _evalue As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/BBH.Simple /in ""{_in}"" /evalue ""{_evalue}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function bbh_Trim_Indeitites(_in As String, Optional _identities As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/bbh.Trim.Indeitites /in ""{_in}"" /identities ""{_identities}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''> Viral assemblage composition in Yellowstone acidic hot springs assessed by network analysis, DOI: 10.1038/ismej.2015.28
''' </summary>
'''
Public Function BLAST_Metagenome_SSU_Network(_net As String, _tax As String, _taxonomy As String, Optional _x2taxid As String = "", Optional _theme_color As String = "'Paired:c12'", Optional _identities As String = "", Optional _coverage As String = "", Optional _out As String = "", Optional _tax_build_in As Boolean = False, Optional _skip_exists As Boolean = False, Optional _gi2taxid As Boolean = False, Optional _parallel As Boolean = False) As Integer
Dim CLI$ = $"/BLAST.Metagenome.SSU.Network /net ""{_net}"" /tax ""{_tax}"" /taxonomy ""{_taxonomy}"" /x2taxid ""{_x2taxid}"" /theme-color ""{_theme_color}"" /identities ""{_identities}"" /coverage ""{_coverage}"" /out ""{_out}"" {If(_tax_build_in, "/tax-build-in", "")} {If(_skip_exists, "/skip-exists", "")} {If(_gi2taxid, "/gi2taxid", "")} {If(_parallel, "/parallel", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function BLAST_Network(_in As String, Optional _out As String = "", Optional _type As String = "", Optional _dict As String = "") As Integer
Dim CLI$ = $"/BLAST.Network /in ""{_in}"" /out ""{_out}"" /type ""{_type}"" /dict ""{_dict}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function BLAST_Network_MetaBuild(_in As String, Optional _out As String = "", Optional _dict As String = "") As Integer
Dim CLI$ = $"/BLAST.Network.MetaBuild /in ""{_in}"" /out ""{_out}"" /dict ""{_dict}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET(_in As String, Optional _out As String = "", Optional _familyinfo As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI$ = $"/Build.Tree.NET /in ""{_in}"" /out ""{_out}"" /familyinfo ""{_familyinfo}"" {If(_brief, "/brief", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_COGs(_cluster As String, _COGs As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Build.Tree.NET.COGs /cluster ""{_cluster}"" /COGs ""{_COGs}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_DEGs(_in As String, _up As String, _down As String, Optional _out As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI$ = $"/Build.Tree.NET.DEGs /in ""{_in}"" /up ""{_up}"" /down ""{_down}"" /out ""{_out}"" {If(_brief, "/brief", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_KEGG_Modules(_in As String, _mods As String, Optional _out As String = "", Optional _brief As Boolean = False, Optional _trim As Boolean = False) As Integer
Dim CLI$ = $"/Build.Tree.NET.KEGG_Modules /in ""{_in}"" /mods ""{_mods}"" /out ""{_out}"" {If(_brief, "/brief", "")} {If(_trim, "/trim", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_KEGG_Pathways(_in As String, _mods As String, Optional _out As String = "", Optional _brief As Boolean = False, Optional _trim As Boolean = False) As Integer
Dim CLI$ = $"/Build.Tree.NET.KEGG_Pathways /in ""{_in}"" /mods ""{_mods}"" /out ""{_out}"" {If(_brief, "/brief", "")} {If(_trim, "/trim", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_Merged_Regulons(_in As String, _family As String, Optional _out As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI$ = $"/Build.Tree.NET.Merged_Regulons /in ""{_in}"" /family ""{_family}"" /out ""{_out}"" {If(_brief, "/brief", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Tree_NET_TF(_in As String, _maps As String, _map As String, _mods As String, Optional _out As String = "", Optional _cuts As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI$ = $"/Build.Tree.NET.TF /in ""{_in}"" /maps ""{_maps}"" /map ""{_map}"" /mods ""{_mods}"" /out ""{_out}"" /cuts ""{_cuts}"" {If(_brief, "/brief", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function KEGG_Mods_NET(_in As String, Optional _out As String = "", Optional _footprints As String = "", Optional _cut As String = "", Optional _pcc As String = "", Optional _pathway As Boolean = False, Optional _brief As Boolean = False) As Integer
Dim CLI$ = $"/KEGG.Mods.NET /in ""{_in}"" /out ""{_out}"" /footprints ""{_footprints}"" /cut ""{_cut}"" /pcc ""{_pcc}"" {If(_pathway, "/pathway", "")} {If(_brief, "/brief", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function linkage_knowledge_network(_in As String, Optional _schema As String = "", Optional _out As String = "", Optional _no_type_prefix As Boolean = False) As Integer
Dim CLI$ = $"/linkage.knowledge.network /in ""{_in}"" /schema ""{_schema}"" /out ""{_out}"" {If(_no_type_prefix, "/no-type_prefix", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Converts a generic distance matrix or kmeans clustering result to network model.
''' </summary>
'''
Public Function Matrix_NET(_in As String, Optional _out As String = "", Optional _colors As String = "", Optional _cutoff As String = "", Optional _generic As Boolean = False, Optional _cutoff_paired As Boolean = False) As Integer
Dim CLI$ = $"/Matrix.NET /in ""{_in}"" /out ""{_out}"" /colors ""{_colors}"" /cutoff ""{_cutoff}"" {If(_generic, "/generic", "")} {If(_cutoff_paired, "/cutoff.paired", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function modNET_Simple(_in As String, Optional _out As String = "", Optional _pathway As Boolean = False) As Integer
Dim CLI$ = $"/modNET.Simple /in ""{_in}"" /out ""{_out}"" {If(_pathway, "/pathway", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_Cluster(_query As String, _LDM As String, Optional _clusters As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Motif.Cluster /query ""{_query}"" /LDM ""{_LDM}"" /clusters ""{_clusters}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_Cluster_Fast(_query As String, Optional _ldm As String = "", Optional _out As String = "", Optional _map As String = "", Optional _maxw As String = "", Optional _ldm_loads As Boolean = False) As Integer
Dim CLI$ = $"/Motif.Cluster.Fast /query ""{_query}"" /ldm ""{_ldm}"" /out ""{_out}"" /map ""{_map}"" /maxw ""{_maxw}"" {If(_ldm_loads, "/ldm_loads", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_Cluster_Fast_Sites(_in As String, Optional _out As String = "", Optional _ldm As String = "") As Integer
Dim CLI$ = $"/Motif.Cluster.Fast.Sites /in ""{_in}"" /out ""{_out}"" /ldm ""{_ldm}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_Cluster_MAT(_query As String, Optional _ldm As String = "", Optional _clusters As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Motif.Cluster.MAT /query ""{_query}"" /ldm ""{_ldm}"" /clusters ""{_clusters}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function net_model(_model As String, Optional _out As String = "", Optional _not_trim As Boolean = False) As Integer
Dim CLI$ = $"/net.model /model ""{_model}"" /out ""{_out}"" {If(_not_trim, "/not-trim", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function net_pathway(_model As String, Optional _out As String = "", Optional _trim As Boolean = False) As Integer
Dim CLI$ = $"/net.pathway /model ""{_model}"" /out ""{_out}"" {If(_trim, "/trim", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Net_rFBA(_in As String, _fba_out As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Net.rFBA /in ""{_in}"" /fba.out ""{_fba_out}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Builds the regulation network between the TF.
''' </summary>
'''
Public Function NetModel_TF_regulates(_in As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI$ = $"/NetModel.TF_regulates /in ""{_in}"" /out ""{_out}"" /cut ""{_cut}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Regulator phenotype relationship cluster from virtual footprints.
''' </summary>
'''
Public Function Phenotypes_KEGG(_mods As String, _in As String, Optional _out As String = "", Optional _pathway As Boolean = False) As Integer
Dim CLI$ = $"/Phenotypes.KEGG /mods ""{_mods}"" /in ""{_in}"" /out ""{_out}"" {If(_pathway, "/pathway", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function reaction_NET(Optional _model As String = "", Optional _source As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/reaction.NET /model ""{_model}"" /source ""{_source}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function replace(_in As String, _nodes As String, _out As String) As Integer
Dim CLI$ = $"/replace /in ""{_in}"" /nodes ""{_nodes}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''This method is not recommended.
''' </summary>
'''
Public Function Tree_Cluster(_in As String, Optional _out As String = "", Optional _locus_map As String = "") As Integer
Dim CLI$ = $"/Tree.Cluster /in ""{_in}"" /out ""{_out}"" /locus.map ""{_locus_map}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Tree_Cluster_rFBA(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Tree.Cluster.rFBA /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Drawing a network image visualization based on the generate network layout from the officials cytoscape software.
''' </summary>
'''
Public Function Draw(_network As String, _parser As String, Optional _size As String = "", Optional _out As String = "", Optional _style As String = "", Optional _style_parser As String = "") As Integer
Dim CLI$ = $"-draw /network ""{_network}"" /parser ""{_parser}"" -size ""{_size}"" -out ""{_out}"" /style ""{_style}"" /style_parser ""{_style_parser}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function graph_regulates(_footprint As String, Optional _trim As Boolean = False) As Integer
Dim CLI$ = $"--graph.regulates /footprint ""{_footprint}"" {If(_trim, "/trim", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function mod_regulations(_model As String, _footprints As String, _out As String, Optional _pathway As Boolean = False, Optional _class As Boolean = False, Optional _type As Boolean = False) As Integer
Dim CLI$ = $"--mod.regulations /model ""{_model}"" /footprints ""{_footprints}"" /out ""{_out}"" {If(_pathway, "/pathway", "")} {If(_class, "/class", "")} {If(_type, "/type", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TCS(_in As String, _regulations As String, _out As String, Optional _Fill_pcc As Boolean = False) As Integer
Dim CLI$ = $"--TCS /in ""{_in}"" /regulations ""{_regulations}"" /out ""{_out}"" {If(_Fill_pcc, "/Fill-pcc", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function
End Class
End Namespace
