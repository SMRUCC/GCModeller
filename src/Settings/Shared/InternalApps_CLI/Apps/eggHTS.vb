Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: D:/GCModeller/GCModeller/bin/eggHTS.exe

Namespace GCModellerApps


''' <summary>
''' eggHTS.CLI
''' </summary>
'''
Public Class eggHTS : Inherits InteropService

Public Const App$ = "eggHTS.exe"

Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /blastX.fill.ORF /in &lt;annotations.csv> /blastx &lt;blastx.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function BlastXFillORF(_in As String, _blastx As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/blastX.fill.ORF")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/blastx " & """" & _blastx & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /COG.profiling.plot /in &lt;myvacog.csv> [/size &lt;image_size, default=1800,1200> /out &lt;out.png>]
''' ```
''' Plots the COGs category statics profiling of the target genome from the COG annotation file.
''' </summary>
'''
Public Function COGCatalogProfilingPlot(_in As String, Optional _size As String = "1800,1200", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/COG.profiling.plot")
Call CLI.Append(" ")
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
''' ```
''' /Data.Add.Mappings /in &lt;data.csv> /bbh &lt;bbh.csv> /ID.mappings &lt;uniprot.ID.mappings.tsv> /uniprot &lt;uniprot.XML> [/ID &lt;fieldName> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function AddReMapping(_in As String, _bbh As String, _ID_mappings As String, _uniprot As String, Optional _id As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Data.Add.Mappings")
Call CLI.Append(" ")
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
''' ```
''' /Data.Add.ORF /in &lt;data.csv> /uniprot &lt;uniprot.XML> [/ID &lt;fieldName> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function DataAddORF(_in As String, _uniprot As String, Optional _id As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Data.Add.ORF")
Call CLI.Append(" ")
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
''' ```
''' /Data.Add.uniprotIDs /in &lt;annotations.csv> /data &lt;data.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function DataAddUniprotIDs(_in As String, _data As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Data.Add.uniprotIDs")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/data " & """" & _data & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DAVID.Split /in &lt;DAVID.txt> [/out &lt;out.DIR, default=./>]
''' ```
''' </summary>
'''
Public Function SplitDAVID(_in As String, Optional _out As String = "./") As Integer
Dim CLI As New StringBuilder("/DAVID.Split")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DEP.heatmap.scatter.3D /in &lt;kmeans.csv> /sampleInfo &lt;sampleInfo.csv> [/cluster.prefix &lt;default="cluster: #"> /size &lt;default=1600,1400> /schema &lt;default=clusters> /view.angle &lt;default=30,60,-56.25> /view.distance &lt;default=2500> /out &lt;out.png>]
''' ```
''' Visualize the DEPs' kmeans cluster result by using 3D scatter plot.
''' </summary>
'''
Public Function DEPHeatmap3D(_in As String, _sampleInfo As String, Optional _cluster_prefix As String = "cluster: #", Optional _size As String = "1600,1400", Optional _schema As String = "clusters", Optional _view_angle As String = "30,60,-56.25", Optional _view_distance As String = "2500", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.heatmap.scatter.3D")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/sampleInfo " & """" & _sampleInfo & """ ")
If Not _cluster_prefix.StringEmpty Then
Call CLI.Append("/cluster.prefix " & """" & _cluster_prefix & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _schema.StringEmpty Then
Call CLI.Append("/schema " & """" & _schema & """ ")
End If
If Not _view_angle.StringEmpty Then
Call CLI.Append("/view.angle " & """" & _view_angle & """ ")
End If
If Not _view_distance.StringEmpty Then
Call CLI.Append("/view.distance " & """" & _view_distance & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DEP.kmeans.scatter2D /in &lt;kmeans.csv> /sampleInfo &lt;sampleInfo.csv> [/t.log &lt;default=-1> /cluster.prefix &lt;default="cluster: #"> /size &lt;1600,1400> /schema &lt;default=clusters> /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function DEPKmeansScatter2D(_in As String, _sampleInfo As String, Optional _t_log As String = "-1", Optional _cluster_prefix As String = "cluster: #", Optional _size As String = "", Optional _schema As String = "clusters", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.kmeans.scatter2D")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/sampleInfo " & """" & _sampleInfo & """ ")
If Not _t_log.StringEmpty Then
Call CLI.Append("/t.log " & """" & _t_log & """ ")
End If
If Not _cluster_prefix.StringEmpty Then
Call CLI.Append("/cluster.prefix " & """" & _cluster_prefix & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _schema.StringEmpty Then
Call CLI.Append("/schema " & """" & _schema & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DEP.logFC.hist /in &lt;log2test.csv> [/step &lt;0.25> /type &lt;default=log2fc> /legend.title &lt;Frequency(log2FC)> /x.axis "(min,max),tick=0.25" /color &lt;lightblue> /size &lt;1400,900> /out &lt;out.png>]
''' ```
''' Using for plots the FC histogram when the experiment have no biological replicates.
''' </summary>
'''
Public Function logFCHistogram(_in As String, Optional _step As String = "", Optional _type As String = "log2fc", Optional _legend_title As String = "", Optional _x_axis As String = "(min,max),tick=0.25", Optional _color As String = "", Optional _size As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.logFC.hist")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _step.StringEmpty Then
Call CLI.Append("/step " & """" & _step & """ ")
End If
If Not _type.StringEmpty Then
Call CLI.Append("/type " & """" & _type & """ ")
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
''' ```
''' /DEP.logFC.Volcano /in &lt;DEP-log2FC.t.test-table.csv> [/title &lt;title> /p.value &lt;default=0.05> /level &lt;default=1.5> /colors &lt;up=red;down=green;other=black> /size &lt;1400,1400> /display.count /out &lt;plot.csv>]
''' ```
''' Volcano plot of the DEPs' analysis result.
''' </summary>
'''
Public Function logFCVolcano(_in As String, Optional _title As String = "", Optional _p_value As String = "0.05", Optional _level As String = "1.5", Optional _colors As String = "", Optional _size As String = "", Optional _out As String = "", Optional _display_count As Boolean = False) As Integer
Dim CLI As New StringBuilder("/DEP.logFC.Volcano")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _title.StringEmpty Then
Call CLI.Append("/title " & """" & _title & """ ")
End If
If Not _p_value.StringEmpty Then
Call CLI.Append("/p.value " & """" & _p_value & """ ")
End If
If Not _level.StringEmpty Then
Call CLI.Append("/level " & """" & _level & """ ")
End If
If Not _colors.StringEmpty Then
Call CLI.Append("/colors " & """" & _colors & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _display_count Then
Call CLI.Append("/display.count ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DEP.uniprot.list /DEP &lt;log2-test.DEP.csv> /sample &lt;sample.csv> [/out &lt;out.txt>]
''' ```
''' </summary>
'''
Public Function DEPUniprotIDlist(_DEP As String, _sample As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.uniprot.list")
Call CLI.Append(" ")
Call CLI.Append("/DEP " & """" & _DEP & """ ")
Call CLI.Append("/sample " & """" & _sample & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DEP.uniprot.list2 /in &lt;log2.test.csv> [/DEP.Flag &lt;is.DEP?> /uniprot.Flag &lt;uniprot> /species &lt;scientifcName> /uniprot &lt;uniprotXML> /out &lt;out.txt>]
''' ```
''' </summary>
'''
Public Function DEPUniprotIDs2(_in As String, Optional _dep_flag As String = "", Optional _uniprot_flag As String = "", Optional _species As String = "", Optional _uniprot As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.uniprot.list2")
Call CLI.Append(" ")
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
''' ```
''' /DEP.venn /data &lt;Directory> [/title &lt;VennDiagram title> /out &lt;out.DIR>]
''' ```
''' Generate the VennDiagram plot data and the venn plot tiff. The default parameter profile is using for the iTraq data.
''' </summary>
'''
Public Function VennData(_data As String, Optional _title As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEP.venn")
Call CLI.Append(" ")
Call CLI.Append("/data " & """" & _data & """ ")
If Not _title.StringEmpty Then
Call CLI.Append("/title " & """" & _title & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DEPs.heatmap /data &lt;Directory/csv_file> [/schema &lt;color_schema, default=RdYlGn:c11> /no-clrev /KO.class /annotation &lt;annotation.csv> /row.labels.geneName /hide.labels /is.matrix /cluster.n &lt;default=6> /sampleInfo &lt;sampleinfo.csv> /non_DEP.blank /title "Heatmap of DEPs log2FC" /t.log2 /tick &lt;-1> /size &lt;size, default=2000,3000> /legend.size &lt;size, default=600,100> /out &lt;out.DIR>]
''' ```
''' Generates the heatmap plot input data. The default label profile is using for the iTraq result.
''' </summary>
'''
Public Function DEPs_heatmapKmeans(_data As String, Optional _schema As String = "RdYlGn:c11", Optional _annotation As String = "", Optional _cluster_n As String = "6", Optional _sampleinfo As String = "", Optional _title As String = "Heatmap of DEPs log2FC", Optional _tick As String = "", Optional _size As String = "2000,3000", Optional _legend_size As String = "600,100", Optional _out As String = "", Optional _no_clrev As Boolean = False, Optional _ko_class As Boolean = False, Optional _row_labels_genename As Boolean = False, Optional _hide_labels As Boolean = False, Optional _is_matrix As Boolean = False, Optional _non_dep_blank As Boolean = False, Optional _t_log2 As Boolean = False) As Integer
Dim CLI As New StringBuilder("/DEPs.heatmap")
Call CLI.Append(" ")
Call CLI.Append("/data " & """" & _data & """ ")
If Not _schema.StringEmpty Then
Call CLI.Append("/schema " & """" & _schema & """ ")
End If
If Not _annotation.StringEmpty Then
Call CLI.Append("/annotation " & """" & _annotation & """ ")
End If
If Not _cluster_n.StringEmpty Then
Call CLI.Append("/cluster.n " & """" & _cluster_n & """ ")
End If
If Not _sampleinfo.StringEmpty Then
Call CLI.Append("/sampleinfo " & """" & _sampleinfo & """ ")
End If
If Not _title.StringEmpty Then
Call CLI.Append("/title " & """" & _title & """ ")
End If
If Not _tick.StringEmpty Then
Call CLI.Append("/tick " & """" & _tick & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _legend_size.StringEmpty Then
Call CLI.Append("/legend.size " & """" & _legend_size & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _no_clrev Then
Call CLI.Append("/no-clrev ")
End If
If _ko_class Then
Call CLI.Append("/ko.class ")
End If
If _row_labels_genename Then
Call CLI.Append("/row.labels.genename ")
End If
If _hide_labels Then
Call CLI.Append("/hide.labels ")
End If
If _is_matrix Then
Call CLI.Append("/is.matrix ")
End If
If _non_dep_blank Then
Call CLI.Append("/non_dep.blank ")
End If
If _t_log2 Then
Call CLI.Append("/t.log2 ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DEPs.stat /in &lt;log2.test.csv> [/log2FC &lt;default=log2FC> /out &lt;out.stat.csv>]
''' ```
''' https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R
''' </summary>
'''
Public Function DEPStatics(_in As String, Optional _log2fc As String = "log2FC", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEPs.stat")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _log2fc.StringEmpty Then
Call CLI.Append("/log2fc " & """" & _log2fc & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DEPs.takes.values /in &lt;DEPs.csv> [/boolean.tag &lt;default=is.DEP> /by.FC &lt;tag=value, default=logFC=log2(1.5)> /by.p.value &lt;tag=value, p.value=0.05> /data &lt;data.csv> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function TakeDEPsValues(_in As String, Optional _boolean_tag As String = "is.DEP", Optional _by_fc As String = "logFC=log2(1.5)", Optional _by_p_value As String = "", Optional _data As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEPs.takes.values")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _boolean_tag.StringEmpty Then
Call CLI.Append("/boolean.tag " & """" & _boolean_tag & """ ")
End If
If Not _by_fc.StringEmpty Then
Call CLI.Append("/by.fc " & """" & _by_fc & """ ")
End If
If Not _by_p_value.StringEmpty Then
Call CLI.Append("/by.p.value " & """" & _by_p_value & """ ")
End If
If Not _data.StringEmpty Then
Call CLI.Append("/data " & """" & _data & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DEPs.union /in &lt;csv.DIR> [/FC &lt;default=logFC> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function DEPsUnion(_in As String, Optional _fc As String = "logFC", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DEPs.union")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _fc.StringEmpty Then
Call CLI.Append("/fc " & """" & _fc & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /edgeR.Designer /in &lt;proteinGroups.csv> /designer &lt;designer.csv> [/label &lt;default is empty> /deli &lt;default=-> /out &lt;out.DIR>]
''' ```
''' Generates the edgeR inputs table
''' </summary>
'''
Public Function edgeRDesigner(_in As String, _designer As String, Optional _label As String = "", Optional _deli As String = "-", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/edgeR.Designer")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/designer " & """" & _designer & """ ")
If Not _label.StringEmpty Then
Call CLI.Append("/label " & """" & _label & """ ")
End If
If Not _deli.StringEmpty Then
Call CLI.Append("/deli " & """" & _deli & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /enricher.background /in &lt;uniprot.XML> [/mapping &lt;maps.tsv> /out &lt;term2gene.txt.DIR>]
''' ```
''' Create enrichment analysis background based on the uniprot xml database.
''' </summary>
'''
Public Function Backgrounds(_in As String, Optional _mapping As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/enricher.background")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _mapping.StringEmpty Then
Call CLI.Append("/mapping " & """" & _mapping & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /enrichment.go /deg &lt;deg.list> /backgrounds &lt;genome_genes.list> /t2g &lt;term2gene.csv> [/go &lt;go_brief.csv> /out &lt;enricher.result.csv>]
''' ```
''' </summary>
'''
Public Function GoEnrichment(_deg As String, _backgrounds As String, _t2g As String, Optional _go As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/enrichment.go")
Call CLI.Append(" ")
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
''' ```
''' /Enrichment.Term.Filter /in &lt;enrichment.csv> /filter &lt;key-string> [/out &lt;out.csv>]
''' ```
''' Filter the specific term result from the analysis output by using pattern keyword
''' </summary>
'''
Public Function EnrichmentTermFilter(_in As String, _filter As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Enrichment.Term.Filter")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/filter " & """" & _filter & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Enrichments.ORF.info /in &lt;enrichment.csv> /proteins &lt;uniprot-genome.XML> [/nocut /ORF /out &lt;out.csv>]
''' ```
''' Retrive KEGG/GO info for the genes in the enrichment result.
''' </summary>
'''
Public Function RetriveEnrichmentGeneInfo(_in As String, _proteins As String, Optional _out As String = "", Optional _nocut As Boolean = False, Optional _orf As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Enrichments.ORF.info")
Call CLI.Append(" ")
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
''' ```
''' /Exocarta.Hits /in &lt;list.txt> /annotation &lt;annotations.csv> /exocarta &lt;Exocarta.tsv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ExocartaHits(_in As String, _annotation As String, _exocarta As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Exocarta.Hits")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/annotation " & """" & _annotation & """ ")
Call CLI.Append("/exocarta " & """" & _exocarta & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Fasta.IDlist /in &lt;prot.fasta> [/out &lt;geneIDs.txt>]
''' ```
''' </summary>
'''
Public Function GetFastaIDlist(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Fasta.IDlist")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /FoldChange.Matrix.Invert /in &lt;data.csv> [/log2FC /out &lt;invert.csv>]
''' ```
''' Reverse the FoldChange value from the source result matrix.
''' </summary>
'''
Public Function iTraqInvert(_in As String, Optional _out As String = "", Optional _log2fc As Boolean = False) As Integer
Dim CLI As New StringBuilder("/FoldChange.Matrix.Invert")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _log2fc Then
Call CLI.Append("/log2fc ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /func.rich.string /in &lt;string_interactions.tsv> /uniprot &lt;uniprot.XML> /DEP &lt;dep.t.test.csv> [/map &lt;map.tsv> /r.range &lt;default=12,30> /log2FC &lt;default=log2FC> /layout &lt;string_network_coordinates.txt> /out &lt;out.network.DIR>]
''' ```
''' DEPs' functional enrichment network based on string-db exports, and color by KEGG pathway.
''' </summary>
'''
Public Function FunctionalNetworkEnrichment(_in As String, _uniprot As String, _DEP As String, Optional _map As String = "", Optional _r_range As String = "12,30", Optional _log2fc As String = "log2FC", Optional _layout As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/func.rich.string")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
Call CLI.Append("/DEP " & """" & _DEP & """ ")
If Not _map.StringEmpty Then
Call CLI.Append("/map " & """" & _map & """ ")
End If
If Not _r_range.StringEmpty Then
Call CLI.Append("/r.range " & """" & _r_range & """ ")
End If
If Not _log2fc.StringEmpty Then
Call CLI.Append("/log2fc " & """" & _log2fc & """ ")
End If
If Not _layout.StringEmpty Then
Call CLI.Append("/layout " & """" & _layout & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Gene.list.from.KOBAS /in &lt;KOBAS.csv> [/p.value &lt;default=1> /out &lt;out.txt>]
''' ```
''' Using this command for generates the gene id list input for the STRING-db search.
''' </summary>
'''
Public Function GeneIDListFromKOBASResult(_in As String, Optional _p_value As String = "1", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Gene.list.from.KOBAS")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _p_value.StringEmpty Then
Call CLI.Append("/p.value " & """" & _p_value & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /GO.cellular_location.Plot /in &lt;KOBAS.GO.csv> [/GO &lt;go.obo> /3D /colors &lt;schemaName, default=Paired:c8> /out &lt;out.png>]
''' ```
''' Visualize of the subcellular location result from the GO enrichment analysis.
''' </summary>
'''
Public Function GO_cellularLocationPlot(_in As String, Optional _go As String = "", Optional _colors As String = "Paired:c8", Optional _out As String = "", Optional _3d As Boolean = False) As Integer
Dim CLI As New StringBuilder("/GO.cellular_location.Plot")
Call CLI.Append(" ")
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
''' ```
''' /GO.enrichment.DAVID /in &lt;DAVID.csv> [/tsv /go &lt;go.obo> /size &lt;default=1200,1000> /tick 1 /p.value &lt;0.05> /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function DAVID_GOplot(_in As String, Optional _go As String = "", Optional _size As String = "1200,1000", Optional _tick As String = "", Optional _p_value As String = "", Optional _out As String = "", Optional _tsv As Boolean = False) As Integer
Dim CLI As New StringBuilder("/GO.enrichment.DAVID")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _go.StringEmpty Then
Call CLI.Append("/go " & """" & _go & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _tick.StringEmpty Then
Call CLI.Append("/tick " & """" & _tick & """ ")
End If
If Not _p_value.StringEmpty Then
Call CLI.Append("/p.value " & """" & _p_value & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _tsv Then
Call CLI.Append("/tsv ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Go.enrichment.plot /in &lt;enrichmentTerm.csv> [/bubble /r "log(x,1.5)" /Corrected /displays &lt;default=10> /PlantRegMap /label.right /gray /pvalue &lt;0.05> /size &lt;2000,1600> /tick 1 /go &lt;go.obo> /out &lt;out.png>]
''' ```
''' Go enrichment plot base on the KOBAS enrichment analysis result.
''' </summary>
'''
Public Function GO_enrichmentPlot(_in As String, Optional _r As String = "log(x,1.5)", Optional _displays As String = "10", Optional _pvalue As String = "", Optional _size As String = "", Optional _tick As String = "", Optional _go As String = "", Optional _out As String = "", Optional _bubble As Boolean = False, Optional _corrected As Boolean = False, Optional _plantregmap As Boolean = False, Optional _label_right As Boolean = False, Optional _gray As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Go.enrichment.plot")
Call CLI.Append(" ")
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
''' ```
''' /iBAQ.Cloud /in &lt;expression.csv> /annotations &lt;annotations.csv> /DEPs &lt;DEPs.csv> /tag &lt;expression> [/out &lt;out.png>]
''' ```
''' Cloud plot of the iBAQ DEPs result.
''' </summary>
'''
Public Function DEPsCloudPlot(_in As String, _annotations As String, _DEPs As String, _tag As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/iBAQ.Cloud")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/annotations " & """" & _annotations & """ ")
Call CLI.Append("/DEPs " & """" & _DEPs & """ ")
Call CLI.Append("/tag " & """" & _tag & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /ID.Replace.bbh /in &lt;dataset.csv> /bbh &lt;bbh.csv> [/out &lt;ID.replaced.csv>]
''' ```
''' Replace the source ID to a unify organism protein ID by using ``bbh`` method.
''' This tools required the protein in ``datatset.csv`` associated with the alignment result in ``bbh.csv`` by using the ``query_name`` property.
''' </summary>
'''
Public Function BBHReplace(_in As String, _bbh As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/ID.Replace.bbh")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Imports.Go.obo.mysql /in &lt;go.obo> [/out &lt;out.sql>]
''' ```
''' Dumping GO obo database as mysql database files.
''' </summary>
'''
Public Function DumpGOAsMySQL(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Imports.Go.obo.mysql")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Imports.Uniprot.Xml /in &lt;uniprot.xml> [/out &lt;out.sql>]
''' ```
''' Dumping the UniprotKB XML database as mysql database file.
''' </summary>
'''
Public Function DumpUniprot(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Imports.Uniprot.Xml")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /iTraq.matrix.split /in &lt;matrix.csv> /sampleInfo &lt;sampleInfo.csv> /designer &lt;analysis.design.csv> [/allowed.swap /out &lt;out.Dir>]
''' ```
''' Split the raw matrix into different compare group based on the experimental designer information.
''' </summary>
'''
Public Function iTraqAnalysisMatrixSplit(_in As String, _sampleInfo As String, _designer As String, Optional _out As String = "", Optional _allowed_swap As Boolean = False) As Integer
Dim CLI As New StringBuilder("/iTraq.matrix.split")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/sampleInfo " & """" & _sampleInfo & """ ")
Call CLI.Append("/designer " & """" & _designer & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _allowed_swap Then
Call CLI.Append("/allowed.swap ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /iTraq.RSD-P.Density /in &lt;matrix.csv> [/out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function iTraqRSDPvalueDensityPlot(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/iTraq.RSD-P.Density")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /iTraq.Symbol.Replacement /in &lt;iTraq.data.csv/xlsx> /symbols &lt;symbols.csv> [/sheet.name &lt;Sheet1> /out &lt;out.DIR>]
''' ```
''' * Using this CLI tool for processing the tag header of iTraq result at first.
''' </summary>
'''
Public Function iTraqSignReplacement(_in As String, _symbols As String, Optional _sheet_name As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/iTraq.Symbol.Replacement")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/symbols " & """" & _symbols & """ ")
If Not _sheet_name.StringEmpty Then
Call CLI.Append("/sheet.name " & """" & _sheet_name & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /iTraq.t.test /in &lt;matrix.csv> [/level &lt;default=1.5> /p.value &lt;default=0.05> /FDR &lt;default=0.05> /skip.significant.test /pairInfo &lt;sampleTuple.csv> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function iTraqTtest(_in As String, Optional _level As String = "1.5", Optional _p_value As String = "0.05", Optional _fdr As String = "0.05", Optional _pairinfo As String = "", Optional _out As String = "", Optional _skip_significant_test As Boolean = False) As Integer
Dim CLI As New StringBuilder("/iTraq.t.test")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _level.StringEmpty Then
Call CLI.Append("/level " & """" & _level & """ ")
End If
If Not _p_value.StringEmpty Then
Call CLI.Append("/p.value " & """" & _p_value & """ ")
End If
If Not _fdr.StringEmpty Then
Call CLI.Append("/fdr " & """" & _fdr & """ ")
End If
If Not _pairinfo.StringEmpty Then
Call CLI.Append("/pairinfo " & """" & _pairinfo & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _skip_significant_test Then
Call CLI.Append("/skip.significant.test ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /KEGG.Color.Pathway /in &lt;protein.annotations.csv> /ref &lt;KEGG.ref.pathwayMap.directory repository> [/out &lt;out.directory>]
''' ```
''' </summary>
'''
Public Function ColorKEGGPathwayMap(_in As String, _ref As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KEGG.Color.Pathway")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/ref " & """" & _ref & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /KEGG.enrichment.DAVID /in &lt;david.csv> [/tsv /custom &lt;ko00001.keg> /size &lt;default=1200,1000> /p.value &lt;default=0.05> /tick 1 /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function DAVID_KEGGplot(_in As String, Optional _custom As String = "", Optional _size As String = "1200,1000", Optional _p_value As String = "0.05", Optional _tick As String = "", Optional _out As String = "", Optional _tsv As Boolean = False) As Integer
Dim CLI As New StringBuilder("/KEGG.enrichment.DAVID")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _custom.StringEmpty Then
Call CLI.Append("/custom " & """" & _custom & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _p_value.StringEmpty Then
Call CLI.Append("/p.value " & """" & _p_value & """ ")
End If
If Not _tick.StringEmpty Then
Call CLI.Append("/tick " & """" & _tick & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _tsv Then
Call CLI.Append("/tsv ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /KEGG.enrichment.DAVID.pathwaymap /in &lt;david.csv> /uniprot &lt;uniprot.XML> [/tsv /DEPs &lt;deps.csv> /colors &lt;default=red,blue,green> /tag &lt;default=log2FC> /pvalue &lt;default=0.05> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function DAVID_KEGGPathwayMap(_in As String, _uniprot As String, Optional _deps As String = "", Optional _colors As String = "red,blue,green", Optional _tag As String = "log2FC", Optional _pvalue As String = "0.05", Optional _out As String = "", Optional _tsv As Boolean = False) As Integer
Dim CLI As New StringBuilder("/KEGG.enrichment.DAVID.pathwaymap")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
If Not _deps.StringEmpty Then
Call CLI.Append("/deps " & """" & _deps & """ ")
End If
If Not _colors.StringEmpty Then
Call CLI.Append("/colors " & """" & _colors & """ ")
End If
If Not _tag.StringEmpty Then
Call CLI.Append("/tag " & """" & _tag & """ ")
End If
If Not _pvalue.StringEmpty Then
Call CLI.Append("/pvalue " & """" & _pvalue & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _tsv Then
Call CLI.Append("/tsv ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /KEGG.Enrichment.PathwayMap /in &lt;kobas.csv> [/DEPs &lt;deps.csv> /colors &lt;default=red,blue,green> /map &lt;id2uniprotID.txt> /uniprot &lt;uniprot.XML> /pvalue &lt;default=0.05> /out &lt;DIR>]
''' ```
''' Show the KEGG pathway map image by using KOBAS KEGG pathway enrichment result.
''' </summary>
'''
Public Function KEGGEnrichmentPathwayMap(_in As String, Optional _deps As String = "", Optional _colors As String = "red,blue,green", Optional _map As String = "", Optional _uniprot As String = "", Optional _pvalue As String = "0.05", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KEGG.Enrichment.PathwayMap")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _deps.StringEmpty Then
Call CLI.Append("/deps " & """" & _deps & """ ")
End If
If Not _colors.StringEmpty Then
Call CLI.Append("/colors " & """" & _colors & """ ")
End If
If Not _map.StringEmpty Then
Call CLI.Append("/map " & """" & _map & """ ")
End If
If Not _uniprot.StringEmpty Then
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
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
''' ```
''' /KEGG.enrichment.plot /in &lt;enrichmentTerm.csv> [/gray /label.right /pvalue &lt;0.05> /tick 1 /size &lt;2000,1600> /out &lt;out.png>]
''' ```
''' Bar plots of the KEGG enrichment analysis result.
''' </summary>
'''
Public Function KEGG_enrichment(_in As String, Optional _pvalue As String = "", Optional _tick As String = "", Optional _size As String = "", Optional _out As String = "", Optional _gray As Boolean = False, Optional _label_right As Boolean = False) As Integer
Dim CLI As New StringBuilder("/KEGG.enrichment.plot")
Call CLI.Append(" ")
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
''' ```
''' /KO.Catalogs /in &lt;blast.mapping.csv> /ko &lt;ko_genes.csv> [/key &lt;Query_id> /mapTo &lt;Subject_id> /out &lt;outDIR>]
''' ```
''' Display the barplot of the KEGG orthology match.
''' </summary>
'''
Public Function KOCatalogs(_in As String, _ko As String, Optional _key As String = "", Optional _mapto As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KO.Catalogs")
Call CLI.Append(" ")
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
''' ```
''' /KOBAS.add.ORF /in &lt;table.csv> /sample &lt;sample.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function KOBASaddORFsource(_in As String, _sample As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KOBAS.add.ORF")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/sample " & """" & _sample & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /KOBAS.Sim.Heatmap /in &lt;sim.csv> [/size &lt;1024,800> /colors &lt;RdYlBu:8> /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function SimHeatmap(_in As String, Optional _size As String = "", Optional _colors As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KOBAS.Sim.Heatmap")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _colors.StringEmpty Then
Call CLI.Append("/colors " & """" & _colors & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /KOBAS.Similarity /group1 &lt;DIR> /group2 &lt;DIR> [/fileName &lt;default=output_run-Gene Ontology.csv> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function KOBASSimilarity(_group1 As String, _group2 As String, Optional _filename As String = "output_run-Gene Ontology.csv", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KOBAS.Similarity")
Call CLI.Append(" ")
Call CLI.Append("/group1 " & """" & _group1 & """ ")
Call CLI.Append("/group2 " & """" & _group2 & """ ")
If Not _filename.StringEmpty Then
Call CLI.Append("/filename " & """" & _filename & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /KOBAS.split /in &lt;kobas.out_run.txt> [/out &lt;DIR>]
''' ```
''' Split the KOBAS run output result text file as seperated csv file.
''' </summary>
'''
Public Function KOBASSplit(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KOBAS.split")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /KOBAS.Term.Kmeans /in &lt;dir.input> [/n &lt;default=3> /out &lt;out.clusters.csv>]
''' ```
''' </summary>
'''
Public Function KOBASKMeans(_in As String, Optional _n As String = "3", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/KOBAS.Term.Kmeans")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _n.StringEmpty Then
Call CLI.Append("/n " & """" & _n & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /labelFree.t.test /in &lt;matrix.csv> /sampleInfo &lt;sampleInfo.csv> /design &lt;analysis_designer.csv> [/level &lt;default=1.5> /p.value &lt;default=0.05> /FDR &lt;default=0.05> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function labelFreeTtest(_in As String, _sampleInfo As String, _design As String, Optional _level As String = "1.5", Optional _p_value As String = "0.05", Optional _fdr As String = "0.05", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/labelFree.t.test")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/sampleInfo " & """" & _sampleInfo & """ ")
Call CLI.Append("/design " & """" & _design & """ ")
If Not _level.StringEmpty Then
Call CLI.Append("/level " & """" & _level & """ ")
End If
If Not _p_value.StringEmpty Then
Call CLI.Append("/p.value " & """" & _p_value & """ ")
End If
If Not _fdr.StringEmpty Then
Call CLI.Append("/fdr " & """" & _fdr & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Merge.DEPs /in &lt;*.csv,DIR> [/log2 /threshold "log(1.5,2)" /raw &lt;sample.csv> /out &lt;out.csv>]
''' ```
''' Usually using for generates the heatmap plot matrix of the DEPs. This function call will generates two dataset, one is using for the heatmap plot and another is using for the venn diagram plot.
''' </summary>
'''
Public Function MergeDEPs(_in As String, Optional _threshold As String = "log(1.5,2)", Optional _raw As String = "", Optional _out As String = "", Optional _log2 As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Merge.DEPs")
Call CLI.Append(" ")
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
''' ```
''' /Network.PCC /in &lt;matrix.csv> [/cut &lt;default=0.45> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function PccNetwork(_in As String, Optional _cut As String = "0.45", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Network.PCC")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _cut.StringEmpty Then
Call CLI.Append("/cut " & """" & _cut & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /paired.sample.designer /sampleinfo &lt;sampleInfo.csv> /designer &lt;analysisDesigner.csv> /tuple &lt;sampleTuple.csv> [/out &lt;designer.out.csv.Directory>]
''' ```
''' </summary>
'''
Public Function PairedSampleDesigner(_sampleinfo As String, _designer As String, _tuple As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/paired.sample.designer")
Call CLI.Append(" ")
Call CLI.Append("/sampleinfo " & """" & _sampleinfo & """ ")
Call CLI.Append("/designer " & """" & _designer & """ ")
Call CLI.Append("/tuple " & """" & _tuple & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Perseus.MajorityProteinIDs /in &lt;table.csv> [/out &lt;out.txt>]
''' ```
''' Export the uniprot ID list from ``Majority Protein IDs`` row and generates a text file for batch search of the uniprot database.
''' </summary>
'''
Public Function MajorityProteinIDs(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Perseus.MajorityProteinIDs")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Perseus.Stat /in &lt;proteinGroups.txt> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function PerseusStatics(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Perseus.Stat")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Perseus.Table /in &lt;proteinGroups.txt> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function PerseusTable(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Perseus.Table")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Perseus.Table.annotations /in &lt;proteinGroups.csv> /uniprot &lt;uniprot.XML> [/scientifcName &lt;""> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function PerseusTableAnnotations(_in As String, _uniprot As String, Optional _scientifcname As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Perseus.Table.annotations")
Call CLI.Append(" ")
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
''' ```
''' /plot.pimw /in &lt;samples.csv> [/field.pi &lt;default="calc. pI"> /field.mw &lt;default="MW [kDa]"> /legend.fontsize &lt;20> /legend.size (100,30) /quantile.removes &lt;default=1> /out &lt;pimw.png> /size &lt;1600,1200> /color &lt;black> /ticks.Y &lt;-1> /pt.size &lt;8>]
''' ```
''' 'calc. pI' - 'MW [kDa]' scatter plot of the protomics raw sample data.
''' </summary>
'''
Public Function pimwScatterPlot(_in As String, Optional _field_pi As String = "calc. pI", Optional _field_mw As String = "MW [kDa]", Optional _legend_fontsize As String = "", Optional _legend_size As String = "", Optional _quantile_removes As String = "1", Optional _out As String = "", Optional _size As String = "", Optional _color As String = "", Optional _ticks_y As String = "", Optional _pt_size As String = "") As Integer
Dim CLI As New StringBuilder("/plot.pimw")
Call CLI.Append(" ")
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
If Not _quantile_removes.StringEmpty Then
Call CLI.Append("/quantile.removes " & """" & _quantile_removes & """ ")
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
If Not _ticks_y.StringEmpty Then
Call CLI.Append("/ticks.y " & """" & _ticks_y & """ ")
End If
If Not _pt_size.StringEmpty Then
Call CLI.Append("/pt.size " & """" & _pt_size & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /protein.annotations /uniprot &lt;uniprot.XML> [/accession.ID /iTraq /list &lt;uniprot.id.list.txt/rawtable.csv> /mapping &lt;mappings.tab/tsv> /out &lt;out.csv>]
''' ```
''' Total proteins functional annotation by using uniprot database.
''' </summary>
'''
Public Function SampleAnnotations(_uniprot As String, Optional _list As String = "", Optional _mapping As String = "", Optional _out As String = "", Optional _accession_id As Boolean = False, Optional _itraq As Boolean = False) As Integer
Dim CLI As New StringBuilder("/protein.annotations")
Call CLI.Append(" ")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
If Not _list.StringEmpty Then
Call CLI.Append("/list " & """" & _list & """ ")
End If
If Not _mapping.StringEmpty Then
Call CLI.Append("/mapping " & """" & _mapping & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _accession_id Then
Call CLI.Append("/accession.id ")
End If
If _itraq Then
Call CLI.Append("/itraq ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /protein.annotations.shotgun /p1 &lt;data.csv> /p2 &lt;data.csv> /uniprot &lt;data.DIR/*.xml,*.tab> [/remapping /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function SampleAnnotations2(_p1 As String, _p2 As String, _uniprot As String, Optional _out As String = "", Optional _remapping As Boolean = False) As Integer
Dim CLI As New StringBuilder("/protein.annotations.shotgun")
Call CLI.Append(" ")
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
''' ```
''' /protein.EXPORT /in &lt;uniprot.xml> [/sp &lt;name> /exclude /out &lt;out.fasta>]
''' ```
''' Export the protein sequence and save as fasta format from the uniprot database dump XML.
''' </summary>
'''
Public Function proteinEXPORT(_in As String, Optional _sp As String = "", Optional _out As String = "", Optional _exclude As Boolean = False) As Integer
Dim CLI As New StringBuilder("/protein.EXPORT")
Call CLI.Append(" ")
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
''' ```
''' /proteinGroups.venn /in &lt;proteinGroups.csv> /designer &lt;designer.csv> [/label &lt;tag label> /deli &lt;delimiter, default=_> /out &lt;out.venn.DIR>]
''' ```
''' </summary>
'''
Public Function proteinGroupsVenn(_in As String, _designer As String, Optional _label As String = "", Optional _deli As String = "_", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/proteinGroups.venn")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/designer " & """" & _designer & """ ")
If Not _label.StringEmpty Then
Call CLI.Append("/label " & """" & _label & """ ")
End If
If Not _deli.StringEmpty Then
Call CLI.Append("/deli " & """" & _deli & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /proteins.Go.plot /in &lt;proteins-uniprot-annotations.csv> [/GO &lt;go.obo> /label.right /tick &lt;default=-1> /level &lt;default=2> /selects Q3 /size &lt;2000,2200> /out &lt;out.DIR>]
''' ```
''' ProteinGroups sample data go profiling plot from the uniprot annotation data.
''' </summary>
'''
Public Function ProteinsGoPlot(_in As String, Optional _go As String = "", Optional _tick As String = "-1", Optional _level As String = "2", Optional _selects As String = "", Optional _size As String = "", Optional _out As String = "", Optional _label_right As Boolean = False) As Integer
Dim CLI As New StringBuilder("/proteins.Go.plot")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _go.StringEmpty Then
Call CLI.Append("/go " & """" & _go & """ ")
End If
If Not _tick.StringEmpty Then
Call CLI.Append("/tick " & """" & _tick & """ ")
End If
If Not _level.StringEmpty Then
Call CLI.Append("/level " & """" & _level & """ ")
End If
If Not _selects.StringEmpty Then
Call CLI.Append("/selects " & """" & _selects & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _label_right Then
Call CLI.Append("/label.right ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /proteins.KEGG.plot /in &lt;proteins-uniprot-annotations.csv> [/label.right /custom &lt;sp00001.keg> /size &lt;2200,2000> /tick 20 /out &lt;out.DIR>]
''' ```
''' KEGG function catalog profiling plot of the TP sample.
''' </summary>
'''
Public Function proteinsKEGGPlot(_in As String, Optional _custom As String = "", Optional _size As String = "", Optional _tick As String = "", Optional _out As String = "", Optional _label_right As Boolean = False) As Integer
Dim CLI As New StringBuilder("/proteins.KEGG.plot")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _custom.StringEmpty Then
Call CLI.Append("/custom " & """" & _custom & """ ")
End If
If Not _size.StringEmpty Then
Call CLI.Append("/size " & """" & _size & """ ")
End If
If Not _tick.StringEmpty Then
Call CLI.Append("/tick " & """" & _tick & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _label_right Then
Call CLI.Append("/label.right ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Relative.amount /in &lt;proteinGroups.csv> /designer &lt;designer.csv> [/uniprot &lt;annotations.csv> /label &lt;tag label> /deli &lt;delimiter, default=_> /out &lt;out.csv>]
''' ```
''' Statistics of the relative expression value of the total proteins.
''' </summary>
'''
Public Function RelativeAmount(_in As String, _designer As String, Optional _uniprot As String = "", Optional _label As String = "", Optional _deli As String = "_", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Relative.amount")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/designer " & """" & _designer & """ ")
If Not _uniprot.StringEmpty Then
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
End If
If Not _label.StringEmpty Then
Call CLI.Append("/label " & """" & _label & """ ")
End If
If Not _deli.StringEmpty Then
Call CLI.Append("/deli " & """" & _deli & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /richfun.KOBAS /in &lt;string_interactions.tsv> /uniprot &lt;uniprot.XML> /DEP &lt;dep.t.test.csv> /KOBAS &lt;enrichment.csv> [/r.range &lt;default=5,20> /fold &lt;1.5> /iTraq /logFC &lt;logFC> /layout &lt;string_network_coordinates.txt> /out &lt;out.network.DIR>]
''' ```
''' </summary>
'''
Public Function KOBASNetwork(_in As String, _uniprot As String, _DEP As String, _KOBAS As String, Optional _r_range As String = "5,20", Optional _fold As String = "", Optional _logfc As String = "", Optional _layout As String = "", Optional _out As String = "", Optional _itraq As Boolean = False) As Integer
Dim CLI As New StringBuilder("/richfun.KOBAS")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
Call CLI.Append("/DEP " & """" & _DEP & """ ")
Call CLI.Append("/KOBAS " & """" & _KOBAS & """ ")
If Not _r_range.StringEmpty Then
Call CLI.Append("/r.range " & """" & _r_range & """ ")
End If
If Not _fold.StringEmpty Then
Call CLI.Append("/fold " & """" & _fold & """ ")
End If
If Not _logfc.StringEmpty Then
Call CLI.Append("/logfc " & """" & _logfc & """ ")
End If
If Not _layout.StringEmpty Then
Call CLI.Append("/layout " & """" & _layout & """ ")
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
''' ```
''' /Sample.Species.Normalization /bbh &lt;bbh.csv> /uniprot &lt;uniprot.XML/DIR> /idMapping &lt;refSeq2uniprotKB_mappings.tsv> /sample &lt;sample.csv> [/Description &lt;Description.FileName> /ID &lt;columnName> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function NormalizeSpecies_samples(_bbh As String, _uniprot As String, _idMapping As String, _sample As String, Optional _description As String = "", Optional _id As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Sample.Species.Normalization")
Call CLI.Append(" ")
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
''' ```
''' /Samples.IDlist /in &lt;samples.csv> [/Perseus /shotgun /pair &lt;samples2.csv> /out &lt;out.list.txt>]
''' ```
''' Extracts the protein hits from the protomics sample data, and using this ID list for downlaods the uniprot annotation data.
''' </summary>
'''
Public Function GetIDlistFromSampleTable(_in As String, Optional _pair As String = "", Optional _out As String = "", Optional _perseus As Boolean = False, Optional _shotgun As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Samples.IDlist")
Call CLI.Append(" ")
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
''' ```
''' /Shotgun.Data.Strip /in &lt;data.csv> [/out &lt;output.csv>]
''' ```
''' </summary>
'''
Public Function StripShotgunData(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Shotgun.Data.Strip")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Species.Normalization /bbh &lt;bbh.csv> /uniprot &lt;uniprot.XML> /idMapping &lt;refSeq2uniprotKB_mappings.tsv> /annotations &lt;annotations.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function NormalizeSpecies(_bbh As String, _uniprot As String, _idMapping As String, _annotations As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Species.Normalization")
Call CLI.Append(" ")
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
''' ```
''' /T.test.Designer.iTraq /in &lt;proteinGroups.csv> /designer &lt;designer.csv> [/label &lt;default is empty> /deli &lt;default=-> /out &lt;out.DIR>]
''' ```
''' Generates the iTraq data t.test DEP method inputs table
''' </summary>
'''
Public Function TtestDesigner(_in As String, _designer As String, Optional _label As String = "", Optional _deli As String = "-", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/T.test.Designer.iTraq")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/designer " & """" & _designer & """ ")
If Not _label.StringEmpty Then
Call CLI.Append("/label " & """" & _label & """ ")
End If
If Not _deli.StringEmpty Then
Call CLI.Append("/deli " & """" & _deli & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /T.test.Designer.LFQ /in &lt;proteinGroups.csv> /designer &lt;designer.csv> [/label &lt;default is empty> /deli &lt;default=-> /out &lt;out.DIR>]
''' ```
''' Generates the LFQ data t.test DEP method inputs table
''' </summary>
'''
Public Function TtestDesignerLFQ(_in As String, _designer As String, Optional _label As String = "", Optional _deli As String = "-", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/T.test.Designer.LFQ")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/designer " & """" & _designer & """ ")
If Not _label.StringEmpty Then
Call CLI.Append("/label " & """" & _label & """ ")
End If
If Not _deli.StringEmpty Then
Call CLI.Append("/deli " & """" & _deli & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Term2genes /in &lt;uniprot.XML> [/term &lt;GO> /id &lt;ORF> /out &lt;out.tsv>]
''' ```
''' </summary>
'''
Public Function Term2Genes(_in As String, Optional _term As String = "", Optional _id As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Term2genes")
Call CLI.Append(" ")
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
''' ```
''' /Uniprot.Mappings /in &lt;id.list> [/type &lt;P_REFSEQ_AC> /out &lt;out.DIR>]
''' ```
''' Retrieve the uniprot annotation data by using ID mapping operations.
''' </summary>
'''
Public Function UniprotMappings(_in As String, Optional _type As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Uniprot.Mappings")
Call CLI.Append(" ")
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
''' ```
''' /UniRef.map.organism /in &lt;uniref.xml> [/org &lt;organism_name> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function UniRefMap2Organism(_in As String, Optional _org As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/UniRef.map.organism")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _org.StringEmpty Then
Call CLI.Append("/org " & """" & _org & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /UniRef.UniprotKB /in &lt;uniref.xml> [/out &lt;maps.csv>]
''' ```
''' </summary>
'''
Public Function UniRef2UniprotKB(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/UniRef.UniprotKB")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /update.uniprot.mapped /in &lt;table.csv> /mapping &lt;mapping.tsv/tab> [/source /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function Update2UniprotMappedID(_in As String, _mapping As String, Optional _out As String = "", Optional _source As Boolean = False) As Integer
Dim CLI As New StringBuilder("/update.uniprot.mapped")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/mapping " & """" & _mapping & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _source Then
Call CLI.Append("/source ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
