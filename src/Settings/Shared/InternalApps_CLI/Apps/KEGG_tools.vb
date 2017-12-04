Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: D:/GCModeller/GCModeller/bin/KEGG_tools.exe

Namespace GCModellerApps


''' <summary>
''' KEGG web services API tools.
''' </summary>
'''
Public Class KEGG_tools : Inherits InteropService

Public Const App$ = "KEGG_tools.exe"

Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /16s_rna [/out &lt;outDIR>]
''' ```
''' Download 16S rRNA data from KEGG.
''' </summary>
'''
Public Function Download16SRNA(Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/16s_rna")
Call CLI.Append(" ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /blastn /query &lt;query.fasta> [/out &lt;outDIR>]
''' ```
''' Blastn analysis of your DNA sequence on KEGG server for the functional analysis.
''' </summary>
'''
Public Function Blastn(_query As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/blastn")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.Ko.repository /DIR &lt;DIR> /repo &lt;root>
''' ```
''' </summary>
'''
Public Function BuildKORepository(_DIR As String, _repo As String) As Integer
Dim CLI As New StringBuilder("/Build.Ko.repository")
Call CLI.Append(" ")
Call CLI.Append("/DIR " & """" & _DIR & """ ")
Call CLI.Append("/repo " & """" & _repo & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Compile.Model /pathway &lt;pathwayDIR> /mods &lt;modulesDIR> /sp &lt;sp_code> [/out &lt;out.Xml>]
''' ```
''' KEGG pathway model compiler
''' </summary>
'''
Public Function Compile(_pathway As String, _mods As String, _sp As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Compile.Model")
Call CLI.Append(" ")
Call CLI.Append("/pathway " & """" & _pathway & """ ")
Call CLI.Append("/mods " & """" & _mods & """ ")
Call CLI.Append("/sp " & """" & _sp & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Compound.Map.Render /list &lt;csv/txt> [/repo &lt;pathwayMap.repository> /scale &lt;default=1> /color &lt;default=red> /out &lt;out.DIR>]
''' ```
''' Render draw of the KEGG pathway map by using a given KEGG compound id list.
''' </summary>
'''
Public Function CompoundMapRender(_list As String, Optional _repo As String = "", Optional _scale As String = "1", Optional _color As String = "red", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Compound.Map.Render")
Call CLI.Append(" ")
Call CLI.Append("/list " & """" & _list & """ ")
If Not _repo.StringEmpty Then
Call CLI.Append("/repo " & """" & _repo & """ ")
End If
If Not _scale.StringEmpty Then
Call CLI.Append("/scale " & """" & _scale & """ ")
End If
If Not _color.StringEmpty Then
Call CLI.Append("/color " & """" & _color & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Cut_sequence.upstream /in &lt;list.txt> /PTT &lt;genome.ptt> /org &lt;kegg_sp> [/len &lt;100bp> /overrides /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function CutSequence_Upstream(_in As String, _PTT As String, _org As String, Optional _len As String = "", Optional _out As String = "", Optional _overrides As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Cut_sequence.upstream")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/org " & """" & _org & """ ")
If Not _len.StringEmpty Then
Call CLI.Append("/len " & """" & _len & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _overrides Then
Call CLI.Append("/overrides ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Download.Compounds [/chebi &lt;accessions.tsv> /flat /updates /save &lt;DIR>]
''' ```
''' Downloads the KEGG compounds data from KEGG web server using dbget API
''' </summary>
'''
Public Function DownloadCompounds(Optional _chebi As String = "", Optional _save As String = "", Optional _flat As Boolean = False, Optional _updates As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Download.Compounds")
Call CLI.Append(" ")
If Not _chebi.StringEmpty Then
Call CLI.Append("/chebi " & """" & _chebi & """ ")
End If
If Not _save.StringEmpty Then
Call CLI.Append("/save " & """" & _save & """ ")
End If
If _flat Then
Call CLI.Append("/flat ")
End If
If _updates Then
Call CLI.Append("/updates ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Download.human.genes /in &lt;geneID.list/DIR> [/batch /out &lt;save.DIR>]
''' ```
''' </summary>
'''
Public Function DownloadHumanGenes(_in As String, Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Download.human.genes")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _batch Then
Call CLI.Append("/batch ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Download.Module.Maps [/out &lt;EXPORT_DIR, default="./">]
''' ```
''' Download the KEGG reference modules map data.
''' </summary>
'''
Public Function DownloadReferenceModule(Optional _out As String = "./") As Integer
Dim CLI As New StringBuilder("/Download.Module.Maps")
Call CLI.Append(" ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Download.Ortholog -i &lt;gene_list_file.txt/gbk> -export &lt;exportedDIR> [/gbk /sp &lt;KEGG.sp>]
''' ```
''' Downloads the KEGG gene ortholog annotation data from the web server.
''' </summary>
'''
Public Function DownloadOrthologs(_i As String, _export As String, Optional _sp As String = "", Optional _gbk As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Download.Ortholog")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-export " & """" & _export & """ ")
If Not _sp.StringEmpty Then
Call CLI.Append("/sp " & """" & _sp & """ ")
End If
If _gbk Then
Call CLI.Append("/gbk ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Download.Pathway.Maps /sp &lt;kegg.sp_code> [/out &lt;EXPORT_DIR>]
''' ```
''' </summary>
'''
Public Function DownloadPathwayMaps(_sp As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Download.Pathway.Maps")
Call CLI.Append(" ")
Call CLI.Append("/sp " & """" & _sp & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Download.Pathway.Maps.Bacteria.All [/in &lt;brite.keg> /out &lt;out.directory>]
''' ```
''' </summary>
'''
Public Function DownloadsBacteriasRefMaps(Optional _in As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Download.Pathway.Maps.Bacteria.All")
Call CLI.Append(" ")
If Not _in.StringEmpty Then
Call CLI.Append("/in " & """" & _in & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Download.Reaction [/save &lt;DIR>]
''' ```
''' </summary>
'''
Public Function DownloadKEGGReaction(Optional _save As String = "") As Integer
Dim CLI As New StringBuilder("/Download.Reaction")
Call CLI.Append(" ")
If Not _save.StringEmpty Then
Call CLI.Append("/save " & """" & _save & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /dump.kegg.maps /htext &lt;htext.txt> [/out &lt;save_dir>]
''' ```
''' Dumping the KEGG maps database for human species.
''' </summary>
'''
Public Function DumpKEGGMaps(_htext As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/dump.kegg.maps")
Call CLI.Append(" ")
Call CLI.Append("/htext " & """" & _htext & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Dump.sp [/res sp.html /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function DumpOrganisms(Optional _res As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Dump.sp")
Call CLI.Append(" ")
If Not _res.StringEmpty Then
Call CLI.Append("/res " & """" & _res & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Fasta.By.Sp /in &lt;KEGG.fasta> /sp &lt;sp.list> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function GetFastaBySp(_in As String, _sp As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Fasta.By.Sp")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/sp " & """" & _sp & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Get.prot_motif /query &lt;sp:locus> [/out out.json]
''' ```
''' </summary>
'''
Public Function ProteinMotifs(_query As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Get.prot_motif")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Gets.prot_motif /query &lt;query.txt/genome.PTT> [/PTT /sp &lt;kegg-sp> /out &lt;out.json> /update]
''' ```
''' </summary>
'''
Public Function GetsProteinMotifs(_query As String, Optional _sp As String = "", Optional _out As String = "", Optional _ptt As Boolean = False, Optional _update As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Gets.prot_motif")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _sp.StringEmpty Then
Call CLI.Append("/sp " & """" & _sp & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _ptt Then
Call CLI.Append("/ptt ")
End If
If _update Then
Call CLI.Append("/update ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Imports.KO /pathways &lt;DIR> /KO &lt;DIR> [/save &lt;DIR>]
''' ```
''' Imports the KEGG reference pathway map and KEGG orthology data as mysql dumps.
''' </summary>
'''
Public Function ImportsKODatabase(_pathways As String, _KO As String, Optional _save As String = "") As Integer
Dim CLI As New StringBuilder("/Imports.KO")
Call CLI.Append(" ")
Call CLI.Append("/pathways " & """" & _pathways & """ ")
Call CLI.Append("/KO " & """" & _KO & """ ")
If Not _save.StringEmpty Then
Call CLI.Append("/save " & """" & _save & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Imports.SSDB /in &lt;source.DIR> [/out &lt;ssdb.csv>]
''' ```
''' </summary>
'''
Public Function ImportsDb(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Imports.SSDB")
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
''' /ko.index.sub.match /index &lt;index.csv> /maps &lt;maps.csv> /key &lt;key> /map &lt;mapTo> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function IndexSubMatch(_index As String, _maps As String, _key As String, _map As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/ko.index.sub.match")
Call CLI.Append(" ")
Call CLI.Append("/index " & """" & _index & """ ")
Call CLI.Append("/maps " & """" & _maps & """ ")
Call CLI.Append("/key " & """" & _key & """ ")
Call CLI.Append("/map " & """" & _map & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Organism.Table [/in &lt;br08601-htext.keg> /Bacteria /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function KEGGOrganismTable(Optional _in As String = "", Optional _out As String = "", Optional _bacteria As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Organism.Table")
Call CLI.Append(" ")
If Not _in.StringEmpty Then
Call CLI.Append("/in " & """" & _in & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _bacteria Then
Call CLI.Append("/bacteria ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Pathway.geneIDs /in &lt;pathway.XML> [/out &lt;out.list.txt>]
''' ```
''' </summary>
'''
Public Function PathwayGeneList(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Pathway.geneIDs")
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
''' /Pathways.Downloads.All [/out &lt;outDIR>]
''' ```
''' Download all of the KEGG reference pathway map data.
''' </summary>
'''
Public Function DownloadsAllPathways(Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Pathways.Downloads.All")
Call CLI.Append(" ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Query.KO /in &lt;blastnhits.csv> [/out &lt;out.csv> /evalue 1e-5 /batch]
''' ```
''' </summary>
'''
Public Function QueryKOAnno(_in As String, Optional _out As String = "", Optional _evalue As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Query.KO")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _evalue.StringEmpty Then
Call CLI.Append("/evalue " & """" & _evalue & """ ")
End If
If _batch Then
Call CLI.Append("/batch ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Views.mod_stat /in &lt;KEGG_Modules/Pathways_DIR> /locus &lt;in.csv> [/locus_map Gene /pathway /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function Stats(_in As String, _locus As String, Optional _locus_map As String = "", Optional _out As String = "", Optional _pathway As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Views.mod_stat")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/locus " & """" & _locus & """ ")
If Not _locus_map.StringEmpty Then
Call CLI.Append("/locus_map " & """" & _locus_map & """ ")
End If
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
''' -Build.KO [/fill-missing]
''' ```
''' Download data from KEGG database to local server.
''' </summary>
'''
Public Function BuildKEGGOrthology(Optional _fill_missing As Boolean = False) As Integer
Dim CLI As New StringBuilder("-Build.KO")
Call CLI.Append(" ")
If _fill_missing Then
Call CLI.Append("/fill-missing ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' Download.Sequence /query &lt;querySource.txt> [/out &lt;outDIR> /source &lt;existsDIR>]
''' ```
''' </summary>
'''
Public Function DownloadSequence(_query As String, Optional _out As String = "", Optional _source As String = "") As Integer
Dim CLI As New StringBuilder("Download.Sequence")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _source.StringEmpty Then
Call CLI.Append("/source " & """" & _source & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --Dump.Db /KEGG.Pathways &lt;DIR> /KEGG.Modules &lt;DIR> /KEGG.Reactions &lt;DIR> /sp &lt;sp.Code> /out &lt;out.Xml>
''' ```
''' </summary>
'''
Public Function DumpDb(_KEGG_Pathways As String, _KEGG_Modules As String, _KEGG_Reactions As String, _sp As String, _out As String) As Integer
Dim CLI As New StringBuilder("--Dump.Db")
Call CLI.Append(" ")
Call CLI.Append("/KEGG.Pathways " & """" & _KEGG_Pathways & """ ")
Call CLI.Append("/KEGG.Modules " & """" & _KEGG_Modules & """ ")
Call CLI.Append("/KEGG.Reactions " & """" & _KEGG_Reactions & """ ")
Call CLI.Append("/sp " & """" & _sp & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' -function.association.analysis -i &lt;matrix_csv>
''' ```
''' </summary>
'''
Public Function FunctionAnalysis(_i As String) As Integer
Dim CLI As New StringBuilder("-function.association.analysis")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --Get.KO /in &lt;KASS-query.txt>
''' ```
''' </summary>
'''
Public Function GetKOAnnotation(_in As String) As Integer
Dim CLI As New StringBuilder("--Get.KO")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --part.from /source &lt;source.fasta> /ref &lt;referenceFrom.fasta> [/out &lt;out.fasta> /brief]
''' ```
''' source and ref should be in KEGG annotation format.
''' </summary>
'''
Public Function GetSource(_source As String, _ref As String, Optional _out As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("--part.from")
Call CLI.Append(" ")
Call CLI.Append("/source " & """" & _source & """ ")
Call CLI.Append("/ref " & """" & _ref & """ ")
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
''' -query -keyword &lt;keyword> -o &lt;out_dir>
''' ```
''' Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.
''' </summary>
'''
Public Function QueryGenes(_keyword As String, _o As String) As Integer
Dim CLI As New StringBuilder("-query")
Call CLI.Append(" ")
Call CLI.Append("-keyword " & """" & _keyword & """ ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' -query.orthology -keyword &lt;gene_name> -o &lt;output_csv>
''' ```
''' </summary>
'''
Public Function QueryOrthology(_keyword As String, _o As String) As Integer
Dim CLI As New StringBuilder("-query.orthology")
Call CLI.Append(" ")
Call CLI.Append("-keyword " & """" & _keyword & """ ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' -query.ref.map -id &lt;id> -o &lt;out_dir>
''' ```
''' </summary>
'''
Public Function DownloadReferenceMap(_id As String, _o As String) As Integer
Dim CLI As New StringBuilder("-query.ref.map")
Call CLI.Append(" ")
Call CLI.Append("-id " & """" & _id & """ ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' -ref.map.download -o &lt;out_dir>
''' ```
''' </summary>
'''
Public Function DownloadReferenceMapDatabase(_o As String) As Integer
Dim CLI As New StringBuilder("-ref.map.download")
Call CLI.Append(" ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' -table.create -i &lt;input_dir> -o &lt;out_csv>
''' ```
''' </summary>
'''
Public Function CreateTABLE(_i As String, _o As String) As Integer
Dim CLI As New StringBuilder("-table.create")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
