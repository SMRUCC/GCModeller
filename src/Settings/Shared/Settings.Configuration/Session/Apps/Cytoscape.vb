Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/Cytoscape.exe

Namespace GCModellerApps


''' <summary>
''' Cytoscape model generator and visualization tools utils for GCModeller
''' </summary>
'''
Public Class Cytoscape : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /Analysis.Graph.Properties /in &lt;net.DIR> [/colors &lt;Paired:c12> /ignores &lt;fields> /tick 5 /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function AnalysisNetworkProperty(_in As String, Optional _colors As String = "", Optional _ignores As String = "", Optional _tick As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("AnalysisNetworkProperty")
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
''' ```
''' /Analysis.Node.Clusters /in &lt;network.DIR> [/spcc /size "10000,10000" /schema &lt;YlGn:c8> /out &lt;DIR>]
''' ```
''' </summary>
'''
Public Function NodeCluster(_in As String, Optional _size As String = "10000,10000", Optional _schema As String = "", Optional _out As String = "", Optional _spcc As Boolean = False) As Integer
Dim CLI As New StringBuilder("NodeCluster")
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
''' ```
''' /associate /in &lt;net.csv> /nodes &lt;nodes.csv> [/out &lt;out.net.DIR>]
''' ```
''' </summary>
'''
Public Function Assciates(_in As String, _nodes As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("Assciates")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/nodes " & """" & _nodes & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /BBH.Simple /in &lt;sbh.csv> [/evalue &lt;evalue: 1e-5> /out &lt;out.bbh.csv>]
''' ```
''' </summary>
'''
Public Function SimpleBBH(_in As String, Optional _evalue As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("SimpleBBH")
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
''' ```
''' /bbh.Trim.Indeitites /in &lt;bbh.csv> [/identities &lt;0.3> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function BBHTrimIdentities(_in As String, Optional _identities As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("BBHTrimIdentities")
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
''' ```
''' /BLAST.Metagenome.SSU.Network /net &lt;blastn.self.txt/blastnmapping.csv> /tax &lt;ssu-nt.blastnMaps.csv> /taxonomy &lt;ncbi_taxonomy:names,nodes> [/x2taxid &lt;x2taxid.dmp/DIR> /tax-build-in /skip-exists /gi2taxid /parallel /theme-color &lt;default='Paired:c12'> /identities &lt;default:0.3> /coverage &lt;default:0.3> /out &lt;out-net.DIR>]
''' ```
''' > Viral assemblage composition in Yellowstone acidic hot springs assessed by network analysis, DOI: 10.1038/ismej.2015.28
''' </summary>
'''
Public Function SSU_MetagenomeNetwork(_net As String, _tax As String, _taxonomy As String, Optional _x2taxid As String = "", Optional _theme_color As String = "'Paired:c12'", Optional _identities As String = "", Optional _coverage As String = "", Optional _out As String = "", Optional _tax_build_in As Boolean = False, Optional _skip_exists As Boolean = False, Optional _gi2taxid As Boolean = False, Optional _parallel As Boolean = False) As Integer
Dim CLI As New StringBuilder("SSU_MetagenomeNetwork")
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
''' ```
''' /BLAST.Network /in &lt;inFile> [/out &lt;outDIR> /type &lt;default:blast_out; values: blast_out, sbh, bbh> /dict &lt;dict.xml>]
''' ```
''' </summary>
'''
Public Function GenerateBlastNetwork(_in As String, Optional _out As String = "", Optional _type As String = "", Optional _dict As String = "") As Integer
Dim CLI As New StringBuilder("GenerateBlastNetwork")
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
''' ```
''' /BLAST.Network.MetaBuild /in &lt;inDIR> [/out &lt;outDIR> /dict &lt;dict.xml>]
''' ```
''' </summary>
'''
Public Function MetaBuildBLAST(_in As String, Optional _out As String = "", Optional _dict As String = "") As Integer
Dim CLI As New StringBuilder("MetaBuildBLAST")
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
''' ```
''' /Build.Tree.NET /in &lt;cluster.csv> [/out &lt;outDIR> /brief /FamilyInfo &lt;regulons.DIR>]
''' ```
''' </summary>
'''
Public Function BuildTreeNET(_in As String, Optional _out As String = "", Optional _familyinfo As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("BuildTreeNET")
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
''' ```
''' /Build.Tree.NET.COGs /cluster &lt;cluster.csv> /COGs &lt;myvacog.csv> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function BuildTreeNETCOGs(_cluster As String, _COGs As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("BuildTreeNETCOGs")
Call CLI.Append("/cluster " & """" & _cluster & """ ")
Call CLI.Append("/COGs " & """" & _COGs & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.Tree.NET.DEGs /in &lt;cluster.csv> /up &lt;locus.txt> /down &lt;locus.txt> [/out &lt;outDIR> /brief]
''' ```
''' </summary>
'''
Public Function BuildTreeNET_DEGs(_in As String, _up As String, _down As String, Optional _out As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("BuildTreeNET_DEGs")
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
''' ```
''' /Build.Tree.NET.KEGG_Modules /in &lt;cluster.csv> /mods &lt;modules.XML.DIR> [/out &lt;outDIR> /brief /trim]
''' ```
''' </summary>
'''
Public Function BuildTreeNET_KEGGModules(_in As String, _mods As String, Optional _out As String = "", Optional _brief As Boolean = False, Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("BuildTreeNET_KEGGModules")
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
''' ```
''' /Build.Tree.NET.KEGG_Pathways /in &lt;cluster.csv> /mods &lt;pathways.XML.DIR> [/out &lt;outDIR> /brief /trim]
''' ```
''' </summary>
'''
Public Function BuildTreeNET_KEGGPathways(_in As String, _mods As String, Optional _out As String = "", Optional _brief As Boolean = False, Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("BuildTreeNET_KEGGPathways")
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
''' ```
''' /Build.Tree.NET.Merged_Regulons /in &lt;cluster.csv> /family &lt;family_Hits.Csv> [/out &lt;outDIR> /brief]
''' ```
''' </summary>
'''
Public Function BuildTreeNET_MergeRegulons(_in As String, _family As String, Optional _out As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("BuildTreeNET_MergeRegulons")
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
''' ```
''' /Build.Tree.NET.TF /in &lt;cluster.csv> /maps &lt;TF.Regprecise.maps.Csv> /map &lt;keyvaluepair.xml> /mods &lt;kegg_modules.DIR> [/out &lt;outDIR> /brief /cuts 0.8]
''' ```
''' </summary>
'''
Public Function BuildTreeNetTF(_in As String, _maps As String, _map As String, _mods As String, Optional _out As String = "", Optional _cuts As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("BuildTreeNetTF")
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
''' ```
''' /KEGG.Mods.NET /in &lt;mods.xml.DIR> [/out &lt;outDIR> /pathway /footprints &lt;footprints.Csv> /brief /cut 0 /pcc 0]
''' ```
''' </summary>
'''
Public Function ModsNET(_in As String, Optional _out As String = "", Optional _footprints As String = "", Optional _cut As String = "", Optional _pcc As String = "", Optional _pathway As Boolean = False, Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("ModsNET")
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
''' ```
''' /linkage.knowledge.network /in &lt;knowledge.network.csv/DIR> [/schema &lt;material> /no-type_prefix /out &lt;out.network.DIR>]
''' ```
''' </summary>
'''
Public Function LinkageKnowledgeNetwork(_in As String, Optional _schema As String = "", Optional _out As String = "", Optional _no_type_prefix As Boolean = False) As Integer
Dim CLI As New StringBuilder("LinkageKnowledgeNetwork")
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
''' ```
''' /Matrix.NET /in &lt;kmeans-out.csv> [/out &lt;net.DIR> /generic /colors &lt;clusters> /cutoff 0 /cutoff.paired]
''' ```
''' Converts a generic distance matrix or kmeans clustering result to network model.
''' </summary>
'''
Public Function MatrixToNetwork(_in As String, Optional _out As String = "", Optional _colors As String = "", Optional _cutoff As String = "", Optional _generic As Boolean = False, Optional _cutoff_paired As Boolean = False) As Integer
Dim CLI As New StringBuilder("MatrixToNetwork")
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
''' ```
''' /modNET.Simple /in &lt;mods/pathway_DIR> [/out &lt;outDIR> /pathway]
''' ```
''' </summary>
'''
Public Function SimpleModesNET(_in As String, Optional _out As String = "", Optional _pathway As Boolean = False) As Integer
Dim CLI As New StringBuilder("SimpleModesNET")
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
''' ```
''' /Motif.Cluster /query &lt;meme.txt/MEME_OUT.DIR> /LDM &lt;LDM-name/xml.path> [/clusters &lt;3> /out &lt;outCsv>]
''' ```
''' </summary>
'''
Public Function MotifCluster(_query As String, _LDM As String, Optional _clusters As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("MotifCluster")
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
''' ```
''' /Motif.Cluster.Fast /query &lt;meme_OUT.DIR> [/LDM &lt;ldm-DIR> /out &lt;outDIR> /map &lt;gb.gbk> /maxw -1 /ldm_loads]
''' ```
''' </summary>
'''
Public Function FastCluster(_query As String, Optional _ldm As String = "", Optional _out As String = "", Optional _map As String = "", Optional _maxw As String = "", Optional _ldm_loads As Boolean = False) As Integer
Dim CLI As New StringBuilder("FastCluster")
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
''' ```
''' /Motif.Cluster.Fast.Sites /in &lt;meme.txt.DIR> [/out &lt;outDIR> /LDM &lt;ldm-DIR>]
''' ```
''' </summary>
'''
Public Function MotifClusterSites(_in As String, Optional _out As String = "", Optional _ldm As String = "") As Integer
Dim CLI As New StringBuilder("MotifClusterSites")
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
''' ```
''' /Motif.Cluster.MAT /query &lt;meme_OUT.DIR> [/LDM &lt;ldm-DIR> /clusters 5 /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ClusterMatrix(_query As String, Optional _ldm As String = "", Optional _clusters As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("ClusterMatrix")
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
''' ```
''' /net.model /model &lt;kegg.xmlModel.xml> [/out &lt;outDIR> /not-trim]
''' ```
''' </summary>
'''
Public Function BuildModelNet(_model As String, Optional _out As String = "", Optional _not_trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("BuildModelNet")
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
''' ```
''' /net.pathway /model &lt;kegg.pathway.xml> [/out &lt;outDIR> /trim]
''' ```
''' </summary>
'''
Public Function PathwayNet(_model As String, Optional _out As String = "", Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("PathwayNet")
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
''' ```
''' /Net.rFBA /in &lt;metacyc.sbml> /fba.out &lt;flux.Csv> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function net_rFBA(_in As String, _fba_out As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("net_rFBA")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/fba.out " & """" & _fba_out & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


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
Public Function TFNet(_in As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI As New StringBuilder("TFNet")
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
''' ```
''' /Phenotypes.KEGG /mods &lt;KEGG_Modules/Pathways.DIR> /in &lt;VirtualFootprints.csv> [/pathway /out &lt;outCluster.csv>]
''' ```
''' Regulator phenotype relationship cluster from virtual footprints.
''' </summary>
'''
Public Function KEGGModulesPhenotypeRegulates(_mods As String, _in As String, Optional _out As String = "", Optional _pathway As Boolean = False) As Integer
Dim CLI As New StringBuilder("KEGGModulesPhenotypeRegulates")
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
''' ```
''' /reaction.NET [/model &lt;xmlModel.xml> /source &lt;rxn.DIR> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ReactionNET(Optional _model As String = "", Optional _source As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("ReactionNET")
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
''' ```
''' /replace /in &lt;net.csv> /nodes &lt;nodes.Csv> /out &lt;out.Csv>
''' ```
''' </summary>
'''
Public Function replaceName(_in As String, _nodes As String, _out As String) As Integer
Dim CLI As New StringBuilder("replaceName")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/nodes " & """" & _nodes & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


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
Public Function TreeCluster(_in As String, Optional _out As String = "", Optional _locus_map As String = "") As Integer
Dim CLI As New StringBuilder("TreeCluster")
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
''' ```
''' /Tree.Cluster.rFBA /in &lt;in.flux.pheno_OUT.Csv> [/out &lt;out.cluster.csv>]
''' ```
''' </summary>
'''
Public Function rFBATreeCluster(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("rFBATreeCluster")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


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
Public Function DrawingInvoke(_network As String, _parser As String, Optional _size As String = "", Optional _out As String = "", Optional _style As String = "", Optional _style_parser As String = "") As Integer
Dim CLI As New StringBuilder("DrawingInvoke")
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
''' ```
''' --graph.regulates /footprint &lt;footprints.csv> [/trim]
''' ```
''' </summary>
'''
Public Function SimpleRegulation(_footprint As String, Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("SimpleRegulation")
Call CLI.Append("/footprint " & """" & _footprint & """ ")
If _trim Then
Call CLI.Append("/trim ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --mod.regulations /model &lt;KEGG.xml> /footprints &lt;footprints.csv> /out &lt;outDIR> [/pathway /class /type]
''' ```
''' </summary>
'''
Public Function ModuleRegulations(_model As String, _footprints As String, _out As String, Optional _pathway As Boolean = False, Optional _class As Boolean = False, Optional _type As Boolean = False) As Integer
Dim CLI As New StringBuilder("ModuleRegulations")
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
''' ```
''' --TCS /in &lt;TCS.csv.DIR> /regulations &lt;TCS.virtualfootprints> /out &lt;outForCytoscape.xml> [/Fill-pcc]
''' ```
''' </summary>
'''
Public Function TCS(_in As String, _regulations As String, _out As String, Optional _Fill_pcc As Boolean = False) As Integer
Dim CLI As New StringBuilder("TCS")
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
