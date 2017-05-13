Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/eggHTS.exe

Namespace GCModellerApps


''' <summary>
'''eggHTS.CLI
''' </summary>
'''
Public Class eggHTS : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
'''
''' </summary>
'''
Public Function COG_profiling_plot(_in As String, Optional _size As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/COG.profiling.plot")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
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
Public Function Data_Add_Mappings(_in As String, _bbh As String, _ID_mappings As String, _uniprot As String, Optional _id As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Data.Add.Mappings")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/ID.mappings " & """" & _ID_mappings & """ ")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
If Not _id.StringEmpty Then
Call CLI.Append("/id " & """" & _id & """ ")
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
Public Function Data_Add_ORF(_in As String, _uniprot As String, Optional _id As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Data.Add.ORF")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
If Not _id.StringEmpty Then
Call CLI.Append("/id " & """" & _id & """ ")
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
Public Function Data_Add_uniprotIDs(_in As String, _data As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Data.Add.uniprotIDs")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/data " & """" & _data & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Generates the heatmap plot input data. The default label profile is using for the iTraq result.
''' </summary>
'''
Public Function DEP_heatmap(_data As String, Optional _level As String = "", Optional _fc_tag As String = "", Optional _pvalue As String = "", Optional _out As String = "", Optional _non_dep_blank As Boolean = False) As Integer
Dim CLI As New StringBuilder("/DEP.heatmap")
Call CLI.Append("/data " & """" & _data & """ ")
If Not _level.StringEmpty Then
Call CLI.Append("/level " & """" & _level & """ ")
End If
If Not _fc_tag.StringEmpty Then
Call CLI.Append("/fc.tag " & """" & _fc_tag & """ ")
End If
If Not _pvalue.StringEmpty Then
Call CLI.Append("/pvalue " & """" & _pvalue & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _non_dep_blank Then
Call CLI.Append("/non_dep.blank ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Using for plots the FC histogram when the experiment have no biological replicates.
''' </summary>
'''
Public Function DEP_logFC_hist(_in As String, Optional _step As String = "", Optional _tag As String = "", Optional _legend_title As String = "", Optional _x_axis As String = "(min,max),tick=0.25", Optional _color As String = "", Optional _size As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.logFC.hist")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _step.StringEmpty Then
Call CLI.Append("/step " & """" & _step & """ ")
End If
If Not _tag.StringEmpty Then
Call CLI.Append("/tag " & """" & _tag & """ ")
End If
If Not _legend_title.StringEmpty Then
Call CLI.Append("/legend.title " & """" & _legend_title & """ ")
End If
If Not _x_axis.StringEmpty Then
Call CLI.Append("/x.axis " & """" & _x_axis & """ ")
End If
If Not _color.StringEmpty Then
Call CLI.Append("/color " & """" & _color & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
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
Public Function DEP_logFC_Volcano(_in As String, Optional _size As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.logFC.Volcano")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
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
Public Function DEP_uniprot_list(_DEP As String, _sample As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.uniprot.list")
Call CLI.Append("/DEP " & """" & _DEP & """ ")
Call CLI.Append("/sample " & """" & _sample & """ ")
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
Public Function DEP_uniprot_list2(_in As String, Optional _dep_flag As String = "", Optional _uniprot_flag As String = "", Optional _species As String = "", Optional _uniprot As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.uniprot.list2")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _dep_flag.StringEmpty Then
Call CLI.Append("/dep.flag " & """" & _dep_flag & """ ")
End If
If Not _uniprot_flag.StringEmpty Then
Call CLI.Append("/uniprot.flag " & """" & _uniprot_flag & """ ")
End If
If Not _species.StringEmpty Then
Call CLI.Append("/species " & """" & _species & """ ")
End If
If Not _uniprot.StringEmpty Then
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Generate the VennDiagram plot data and the venn plot tiff. The default parameter profile is using for the iTraq data.
''' </summary>
'''
Public Function DEP_venn(_data As String, Optional _level As String = "", Optional _fc_tag As String = "", Optional _title As String = "", Optional _pvalue As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.venn")
Call CLI.Append("/data " & """" & _data & """ ")
If Not _level.StringEmpty Then
Call CLI.Append("/level " & """" & _level & """ ")
End If
If Not _fc_tag.StringEmpty Then
Call CLI.Append("/fc.tag " & """" & _fc_tag & """ ")
End If
If Not _title.StringEmpty Then
Call CLI.Append("/title " & """" & _title & """ ")
End If
If Not _pvalue.StringEmpty Then
Call CLI.Append("/pvalue " & """" & _pvalue & """ ")
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
Public Function enricher_background(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/enricher.background")
Call CLI.Append("/in " & """" & _in & """ ")
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
Public Function enrichment_go(_deg As String, _backgrounds As String, _t2g As String, Optional _go As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/enrichment.go")
Call CLI.Append("/deg " & """" & _deg & """ ")
Call CLI.Append("/backgrounds " & """" & _backgrounds & """ ")
Call CLI.Append("/t2g " & """" & _t2g & """ ")
If Not _go.StringEmpty Then
Call CLI.Append("/go " & """" & _go & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Filter the specific term result from the analysis output by using pattern keyword
''' </summary>
'''
Public Function Enrichment_Term_Filter(_in As String, _filter As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Enrichment.Term.Filter")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/filter " & """" & _filter & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Retrive KEGG/GO info for the genes in the enrichment result.
''' </summary>
'''
Public Function Enrichments_ORF_info(_in As String, _proteins As String, Optional _out As String = "", Optional _nocut As Boolean = False, Optional _orf As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Enrichments.ORF.info")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/proteins " & """" & _proteins & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _nocut Then
Call CLI.Append("/nocut ")
End If
If _orf Then
Call CLI.Append("/orf ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Fasta_IDlist(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Fasta.IDlist")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Visualize of the subcellular location result from the GO enrichment analysis.
''' </summary>
'''
Public Function GO_cellular_location_Plot(_in As String, Optional _go As String = "", Optional _colors As String = "Paired:c8", Optional _out As String = "", Optional _3d As Boolean = False) As Integer
Dim CLI As New StringBuilder("/GO.cellular_location.Plot")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _go.StringEmpty Then
Call CLI.Append("/go " & """" & _go & """ ")
End If
If Not _colors.StringEmpty Then
Call CLI.Append("/colors " & """" & _colors & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _3d Then
Call CLI.Append("/3d ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Go_enrichment_plot(_in As String, Optional _r As String = "log(x,1.5)", Optional _displays As String = "", Optional _pvalue As String = "", Optional _size As String = "", Optional _tick As String = "", Optional _go As String = "", Optional _out As String = "", Optional _bubble As Boolean = False, Optional _corrected As Boolean = False, Optional _plantregmap As Boolean = False, Optional _label_right As Boolean = False, Optional _gray As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Go.enrichment.plot")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _r.StringEmpty Then
Call CLI.Append("/r " & """" & _r & """ ")
End If
If Not _displays.StringEmpty Then
Call CLI.Append("/displays " & """" & _displays & """ ")
End If
If Not _pvalue.StringEmpty Then
Call CLI.Append("/pvalue " & """" & _pvalue & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _tick.StringEmpty Then
Call CLI.Append("/tick " & """" & _tick & """ ")
End If
If Not _go.StringEmpty Then
Call CLI.Append("/go " & """" & _go & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _bubble Then
Call CLI.Append("/bubble ")
End If
If _corrected Then
Call CLI.Append("/corrected ")
End If
If _plantregmap Then
Call CLI.Append("/plantregmap ")
End If
If _label_right Then
Call CLI.Append("/label.right ")
End If
If _gray Then
Call CLI.Append("/gray ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Reverse the FC value from the source result.
''' </summary>
'''
Public Function iTraq_Reverse_FC(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/iTraq.Reverse.FC")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Show the KEGG pathway map image by using KOBAS KEGG pathway enrichment result.
''' </summary>
'''
Public Function KEGG_Enrichment_PathwayMap(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KEGG.Enrichment.PathwayMap")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Bar plots of the KEGG enrichment analysis result.
''' </summary>
'''
Public Function KEGG_enrichment_plot(_in As String, Optional _pvalue As String = "", Optional _tick As String = "", Optional _size As String = "", Optional _out As String = "", Optional _gray As Boolean = False, Optional _label_right As Boolean = False) As Integer
Dim CLI As New StringBuilder("/KEGG.enrichment.plot")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _pvalue.StringEmpty Then
Call CLI.Append("/pvalue " & """" & _pvalue & """ ")
End If
If Not _tick.StringEmpty Then
Call CLI.Append("/tick " & """" & _tick & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _gray Then
Call CLI.Append("/gray ")
End If
If _label_right Then
Call CLI.Append("/label.right ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Display the barplot of the KEGG orthology match.
''' </summary>
'''
Public Function KO_Catalogs(_in As String, _ko As String, Optional _key As String = "", Optional _mapto As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KO.Catalogs")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/ko " & """" & _ko & """ ")
If Not _key.StringEmpty Then
Call CLI.Append("/key " & """" & _key & """ ")
End If
If Not _mapto.StringEmpty Then
Call CLI.Append("/mapto " & """" & _mapto & """ ")
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
Public Function KOBAS_add_ORF(_in As String, _sample As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KOBAS.add.ORF")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/sample " & """" & _sample & """ ")
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
Public Function KOBAS_split(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KOBAS.split")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Usually using for generates the heatmap plot matrix of the DEPs. This function call will generates two dataset, one is using for the heatmap plot and another is using for the venn diagram plot.
''' </summary>
'''
Public Function Merge_DEPs(_in As String, Optional _threshold As String = "log(1.5,2)", Optional _raw As String = "", Optional _out As String = "", Optional _log2 As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Merge.DEPs")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _threshold.StringEmpty Then
Call CLI.Append("/threshold " & """" & _threshold & """ ")
End If
If Not _raw.StringEmpty Then
Call CLI.Append("/raw " & """" & _raw & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _log2 Then
Call CLI.Append("/log2 ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Perseus_Stat(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Perseus.Stat")
Call CLI.Append("/in " & """" & _in & """ ")
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
Public Function Perseus_Table(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Perseus.Table")
Call CLI.Append("/in " & """" & _in & """ ")
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
Public Function Perseus_Table_annotations(_in As String, _uniprot As String, Optional _scientifcname As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Perseus.Table.annotations")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
If Not _scientifcname.StringEmpty Then
Call CLI.Append("/scientifcname " & """" & _scientifcname & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''''calc. pI'/'MW [kDa]' scatter plot of the protomics raw sample data.
''' </summary>
'''
Public Function plot_pimw(_in As String, Optional _field_pi As String = "", Optional _field_mw As String = "", Optional _legend_fontsize As String = "", Optional _legend_size As String = "", Optional _x_axis As String = "(min,max),tick=2", Optional _y_axis As String = "(min,max),n=10", Optional _out As String = "", Optional _size As String = "", Optional _color As String = "", Optional _pt_size As String = "") As Integer
Dim CLI As New StringBuilder("/plot.pimw")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _field_pi.StringEmpty Then
Call CLI.Append("/field.pi " & """" & _field_pi & """ ")
End If
If Not _field_mw.StringEmpty Then
Call CLI.Append("/field.mw " & """" & _field_mw & """ ")
End If
If Not _legend_fontsize.StringEmpty Then
Call CLI.Append("/legend.fontsize " & """" & _legend_fontsize & """ ")
End If
If Not _legend_size.StringEmpty Then
Call CLI.Append("/legend.size " & """" & _legend_size & """ ")
End If
If Not _x_axis.StringEmpty Then
Call CLI.Append("/x.axis " & """" & _x_axis & """ ")
End If
If Not _y_axis.StringEmpty Then
Call CLI.Append("/y.axis " & """" & _y_axis & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _color.StringEmpty Then
Call CLI.Append("/color " & """" & _color & """ ")
End If
If Not _pt_size.StringEmpty Then
Call CLI.Append("/pt.size " & """" & _pt_size & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Total proteins functional annotation by using uniprot database.
''' </summary>
'''
Public Function protein_annotations(_uniprot As String, Optional _list As String = "", Optional _out As String = "", Optional _itraq As Boolean = False) As Integer
Dim CLI As New StringBuilder("/protein.annotations")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
If Not _list.StringEmpty Then
Call CLI.Append("/list " & """" & _list & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _itraq Then
Call CLI.Append("/itraq ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function protein_annotations_shotgun(_p1 As String, _p2 As String, _uniprot As String, Optional _out As String = "", Optional _remapping As Boolean = False) As Integer
Dim CLI As New StringBuilder("/protein.annotations.shotgun")
Call CLI.Append("/p1 " & """" & _p1 & """ ")
Call CLI.Append("/p2 " & """" & _p2 & """ ")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _remapping Then
Call CLI.Append("/remapping ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function protein_EXPORT(_in As String, Optional _sp As String = "", Optional _out As String = "", Optional _exclude As Boolean = False) As Integer
Dim CLI As New StringBuilder("/protein.EXPORT")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _sp.StringEmpty Then
Call CLI.Append("/sp " & """" & _sp & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _exclude Then
Call CLI.Append("/exclude ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''ProteinGroups sample data go profiling plot from the uniprot annotation data.
''' </summary>
'''
Public Function proteins_Go_plot(_in As String, Optional _go As String = "", Optional _tick As String = "", Optional _top As String = "", Optional _size As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/proteins.Go.plot")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _go.StringEmpty Then
Call CLI.Append("/go " & """" & _go & """ ")
End If
If Not _tick.StringEmpty Then
Call CLI.Append("/tick " & """" & _tick & """ ")
End If
If Not _top.StringEmpty Then
Call CLI.Append("/top " & """" & _top & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
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
Public Function proteins_KEGG_plot(_in As String, Optional _size As String = "", Optional _tick As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/proteins.KEGG.plot")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
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
Public Function Sample_Species_Normalization(_bbh As String, _uniprot As String, _idMapping As String, _sample As String, Optional _description As String = "", Optional _id As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Sample.Species.Normalization")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
Call CLI.Append("/idMapping " & """" & _idMapping & """ ")
Call CLI.Append("/sample " & """" & _sample & """ ")
If Not _description.StringEmpty Then
Call CLI.Append("/description " & """" & _description & """ ")
End If
If Not _id.StringEmpty Then
Call CLI.Append("/id " & """" & _id & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Extracts the protein hits from the protomics sample data, and using this ID list for downlaods the uniprot annotation data.
''' </summary>
'''
Public Function Samples_IDlist(_in As String, Optional _pair As String = "", Optional _out As String = "", Optional _perseus As Boolean = False, Optional _shotgun As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Samples.IDlist")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _pair.StringEmpty Then
Call CLI.Append("/pair " & """" & _pair & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _perseus Then
Call CLI.Append("/perseus ")
End If
If _shotgun Then
Call CLI.Append("/shotgun ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Shotgun_Data_Strip(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Shotgun.Data.Strip")
Call CLI.Append("/in " & """" & _in & """ ")
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
Public Function Species_Normalization(_bbh As String, _uniprot As String, _idMapping As String, _annotations As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Species.Normalization")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
Call CLI.Append("/idMapping " & """" & _idMapping & """ ")
Call CLI.Append("/annotations " & """" & _annotations & """ ")
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
Public Function Term2genes(_in As String, Optional _term As String = "", Optional _id As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Term2genes")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _term.StringEmpty Then
Call CLI.Append("/term " & """" & _term & """ ")
End If
If Not _id.StringEmpty Then
Call CLI.Append("/id " & """" & _id & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Retrieve the uniprot annotation data by using ID mapping operations.
''' </summary>
'''
Public Function Uniprot_Mappings(_in As String, Optional _type As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Uniprot.Mappings")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _type.StringEmpty Then
Call CLI.Append("/type " & """" & _type & """ ")
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
Public Function Venn_Functions(_venn As String, _anno As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Venn.Functions")
Call CLI.Append("/venn " & """" & _venn & """ ")
Call CLI.Append("/anno " & """" & _anno & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
