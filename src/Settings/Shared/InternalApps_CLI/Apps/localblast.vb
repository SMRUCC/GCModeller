Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices
' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/localblast.exe

Namespace GCModellerApps


''' <summary>
''' Wrapper tools for the ncbi blast+ program and the blast output data analysis program.
''' For running a large scale parallel alignment task, using ``/venn.BlastAll`` command for ``blastp`` and ``/blastn.Query.All`` command for ``blastn``.
''' </summary>
'''
Public Class localblast : Inherits InteropService

Public Const App$ = "localblast.exe"

Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /add.locus_tag /gb &lt;gb.gbk> /prefix &lt;prefix> [/add.gene /out &lt;out.gb>]
''' ```
''' Add locus_tag qualifier into the feature slot.
''' </summary>
'''
Public Function AddLocusTag(_gb As String, _prefix As String, Optional _out As String = "", Optional _add_gene As Boolean = False) As Integer
Dim CLI As New StringBuilder("/add.locus_tag")
Call CLI.Append(" ")
Call CLI.Append("/gb " & """" & _gb & """ ")
Call CLI.Append("/prefix " & """" & _prefix & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _add_gene Then
Call CLI.Append("/add.gene ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /add.names /anno &lt;anno.csv> /gb &lt;genbank.gbk> [/out &lt;out.gbk> /tag &lt;overrides_name>]
''' ```
''' </summary>
'''
Public Function AddNames(_anno As String, _gb As String, Optional _out As String = "", Optional _tag As String = "") As Integer
Dim CLI As New StringBuilder("/add.names")
Call CLI.Append(" ")
Call CLI.Append("/anno " & """" & _anno & """ ")
Call CLI.Append("/gb " & """" & _gb & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _tag.StringEmpty Then
Call CLI.Append("/tag " & """" & _tag & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /AlignmentTable.TopBest /in &lt;table.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function AlignmentTableTopBest(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/AlignmentTable.TopBest")
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
''' /Bash.Venn /blast &lt;blastDIR> /inDIR &lt;fasta.DIR> /inRef &lt;inRefAs.DIR> [/out &lt;outDIR> /evalue &lt;evalue:10>]
''' ```
''' </summary>
'''
Public Function BashShell(_blast As String, _inDIR As String, _inRef As String, Optional _out As String = "", Optional _evalue As String = "") As Integer
Dim CLI As New StringBuilder("/Bash.Venn")
Call CLI.Append(" ")
Call CLI.Append("/blast " & """" & _blast & """ ")
Call CLI.Append("/inDIR " & """" & _inDIR & """ ")
Call CLI.Append("/inRef " & """" & _inRef & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _evalue.StringEmpty Then
Call CLI.Append("/evalue " & """" & _evalue & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /bbh.EXPORT /query &lt;query.blastp_out> /subject &lt;subject.blast_out> [/trim /out &lt;bbh.csv> /evalue 1e-3 /coverage 0.85 /identities 0.3]
''' ```
''' Export bbh mapping result from the blastp raw output.
''' </summary>
'''
Public Function BBHExportFile(_query As String, _subject As String, Optional _out As String = "", Optional _evalue As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("/bbh.EXPORT")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _evalue.StringEmpty Then
Call CLI.Append("/evalue " & """" & _evalue & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If _trim Then
Call CLI.Append("/trim ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /BBH.Merge /in &lt;inDIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function MergeBBH(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BBH.Merge")
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
''' /BestHits.Filtering /in &lt;besthit.xml> /sp &lt;table.txt> [/out &lt;out.Xml>]
''' ```
''' </summary>
'''
Public Function BestHitFiltering(_in As String, _sp As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BestHits.Filtering")
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
''' /Blastn.Maps.Taxid /in &lt;blastnMapping.csv> /2taxid &lt;acc2taxid.tsv/gi2taxid.dmp> [/gi2taxid /trim /tax &lt;NCBI_taxonomy:nodes/names> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function BlastnMapsTaxonomy(_in As String, _2taxid As String, Optional _tax As String = "", Optional _out As String = "", Optional _gi2taxid As Boolean = False, Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Blastn.Maps.Taxid")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/2taxid " & """" & _2taxid & """ ")
If Not _tax.StringEmpty Then
Call CLI.Append("/tax " & """" & _tax & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _gi2taxid Then
Call CLI.Append("/gi2taxid ")
End If
If _trim Then
Call CLI.Append("/trim ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /blastn.Query /query &lt;query.fna/faa> /db &lt;db.DIR> [/thread /evalue 1e-5 /word_size &lt;-1> /out &lt;out.DIR>]
''' ```
''' Using target fasta sequence query against all of the fasta sequence in target direcotry. This function is single thread.
''' </summary>
'''
Public Function BlastnQuery(_query As String, _db As String, Optional _evalue As String = "", Optional _word_size As String = "", Optional _out As String = "", Optional _thread As Boolean = False) As Integer
Dim CLI As New StringBuilder("/blastn.Query")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/db " & """" & _db & """ ")
If Not _evalue.StringEmpty Then
Call CLI.Append("/evalue " & """" & _evalue & """ ")
End If
If Not _word_size.StringEmpty Then
Call CLI.Append("/word_size " & """" & _word_size & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _thread Then
Call CLI.Append("/thread ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /blastn.Query.All /query &lt;query.fasta.DIR> /db &lt;db.DIR> [/skip-format /evalue 10 /word_size &lt;-1> /out &lt;out.DIR> /parallel /penalty &lt;penalty> /reward &lt;reward>]
''' ```
''' Using the fasta sequence in a directory query against all of the sequence in another directory.
''' </summary>
'''
Public Function BlastnQueryAll(_query As String, _db As String, Optional _evalue As String = "", Optional _word_size As String = "", Optional _out As String = "", Optional _penalty As String = "", Optional _reward As String = "", Optional _skip_format As Boolean = False, Optional _parallel As Boolean = False) As Integer
Dim CLI As New StringBuilder("/blastn.Query.All")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/db " & """" & _db & """ ")
If Not _evalue.StringEmpty Then
Call CLI.Append("/evalue " & """" & _evalue & """ ")
End If
If Not _word_size.StringEmpty Then
Call CLI.Append("/word_size " & """" & _word_size & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _penalty.StringEmpty Then
Call CLI.Append("/penalty " & """" & _penalty & """ ")
End If
If Not _reward.StringEmpty Then
Call CLI.Append("/reward " & """" & _reward & """ ")
End If
If _skip_format Then
Call CLI.Append("/skip-format ")
End If
If _parallel Then
Call CLI.Append("/parallel ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /BlastnMaps.Match.Taxid /in &lt;maps.csv> /acc2taxid &lt;acc2taxid.DIR> [/out &lt;out.tsv>]
''' ```
''' </summary>
'''
Public Function MatchTaxid(_in As String, _acc2taxid As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BlastnMaps.Match.Taxid")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/acc2taxid " & """" & _acc2taxid & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /BlastnMaps.Select /in &lt;reads.id.list.txt> /data &lt;blastn.maps.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function SelectMaps(_in As String, _data As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BlastnMaps.Select")
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
''' /BlastnMaps.Select.Top /in &lt;maps.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function TopBlastnMapReads(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BlastnMaps.Select.Top")
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
''' /BlastnMaps.Summery /in &lt;in.DIR> [/split "-" /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function BlastnMapsSummery(_in As String, Optional _split As String = "-", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BlastnMaps.Summery")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _split.StringEmpty Then
Call CLI.Append("/split " & """" & _split & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Blastp.BBH.Query /query &lt;query.fasta> /hit &lt;hit.source> [/out &lt;outDIR> /overrides /num_threads &lt;-1>]
''' ```
''' Using query fasta invoke blastp against the fasta files in a directory.
''' * This command tools required of NCBI blast+ suite, you must config the blast bin path by using ``settings.exe`` before running this command.
''' </summary>
'''
Public Function BlastpBBHQuery(_query As String, _hit As String, Optional _out As String = "", Optional _num_threads As String = "", Optional _overrides As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Blastp.BBH.Query")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/hit " & """" & _hit & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If _overrides Then
Call CLI.Append("/overrides ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Chromosomes.Export /reads &lt;reads.fasta/DIR> /maps &lt;blastnMappings.Csv/DIR> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ChromosomesBlastnResult(_reads As String, _maps As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Chromosomes.Export")
Call CLI.Append(" ")
Call CLI.Append("/reads " & """" & _reads & """ ")
Call CLI.Append("/maps " & """" & _maps & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /COG.myva /blastp &lt;blastp.myva.txt/sbh.csv> /whog &lt;whog.XML> [/simple /out &lt;out.csv/txt>]
''' ```
''' COG myva annotation using blastp raw output or exports sbh/bbh table result.
''' </summary>
'''
Public Function COG_myva(_blastp As String, _whog As String, Optional _out As String = "", Optional _simple As Boolean = False) As Integer
Dim CLI As New StringBuilder("/COG.myva")
Call CLI.Append(" ")
Call CLI.Append("/blastp " & """" & _blastp & """ ")
Call CLI.Append("/whog " & """" & _whog & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _simple Then
Call CLI.Append("/simple ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /COG.Statics /in &lt;myva_cogs.csv> [/locus &lt;locus.txt/csv> /locuMap &lt;Gene> /out &lt;out.csv>]
''' ```
''' Statics the COG profiling in your analysised genome.
''' </summary>
'''
Public Function COGStatics(_in As String, Optional _locus As String = "", Optional _locumap As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/COG.Statics")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _locus.StringEmpty Then
Call CLI.Append("/locus " & """" & _locus & """ ")
End If
If Not _locumap.StringEmpty Then
Call CLI.Append("/locumap " & """" & _locumap & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /COG2014.result /sbh &lt;query-vs-COG2003-2014.fasta> /cog &lt;cog2003-2014.csv> [/cog.names &lt;cognames2003-2014.tab> /out &lt;out.myva_cog.csv>]
''' ```
''' </summary>
'''
Public Function COG2014_result(_sbh As String, _cog As String, Optional _cog_names As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/COG2014.result")
Call CLI.Append(" ")
Call CLI.Append("/sbh " & """" & _sbh & """ ")
Call CLI.Append("/cog " & """" & _cog & """ ")
If Not _cog_names.StringEmpty Then
Call CLI.Append("/cog.names " & """" & _cog_names & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Copy.Fasta /imports &lt;DIR> [/type &lt;faa,fna,ffn,fasta,...., default:=faa> /out &lt;DIR>]
''' ```
''' Copy target type files from different sub directory into a directory.
''' </summary>
'''
Public Function CopyFasta(_imports As String, Optional _type As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Copy.Fasta")
Call CLI.Append(" ")
Call CLI.Append("/imports " & """" & _imports & """ ")
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
''' /Copy.PTT /in &lt;inDIR> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function CopyPTT(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Copy.PTT")
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
''' /Copys /imports &lt;DIR> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function Copys(_imports As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Copys")
Call CLI.Append(" ")
Call CLI.Append("/imports " & """" & _imports & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.AlignmentTable /in &lt;alignment.txt> [/split /header.split /out &lt;outDIR/file>]
''' ```
''' </summary>
'''
Public Function ExportWebAlignmentTable(_in As String, Optional _out As String = "", Optional _split As Boolean = False, Optional _header_split As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.AlignmentTable")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _split Then
Call CLI.Append("/split ")
End If
If _header_split Then
Call CLI.Append("/header.split ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.AlignmentTable.giList /in &lt;table.csv> [/out &lt;gi.txt>]
''' ```
''' </summary>
'''
Public Function ParseAlignmentTableGIlist(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.AlignmentTable.giList")
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
''' /Export.Blastn /in &lt;in.txt> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ExportBlastn(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.Blastn")
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
''' /Export.blastnMaps /in &lt;blastn.txt> [/best /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ExportBlastnMaps(_in As String, Optional _out As String = "", Optional _best As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.blastnMaps")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _best Then
Call CLI.Append("/best ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.blastnMaps.Batch /in &lt;blastn_out.DIR> [/best /out &lt;out.DIR> /num_threads &lt;-1>]
''' ```
''' Multiple processor task.
''' </summary>
'''
Public Function ExportBlastnMapsBatch(_in As String, Optional _out As String = "", Optional _num_threads As String = "", Optional _best As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.blastnMaps.Batch")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If _best Then
Call CLI.Append("/best ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.blastnMaps.littles /in &lt;blastn.txt.DIR> [/out &lt;out.csv.DIR>]
''' ```
''' </summary>
'''
Public Function ExportBlastnMapsSmall(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.blastnMaps.littles")
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
''' /Export.blastnMaps.Write /in &lt;blastn_out.DIR> [/best /out &lt;write.csv>]
''' ```
''' Exports large amount of blastn output files and write all data into a specific csv file.
''' </summary>
'''
Public Function ExportBlastnMapsBatchWrite(_in As String, Optional _out As String = "", Optional _best As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.blastnMaps.Write")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _best Then
Call CLI.Append("/best ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.BlastX /in &lt;blastx.txt> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ExportBlastX(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.BlastX")
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
''' /EXPORT.COGs.from.DOOR /in &lt;DOOR.opr> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ExportDOORCogs(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/EXPORT.COGs.from.DOOR")
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
''' /Export.gb /gb &lt;genbank.gb/DIR> [/out &lt;outDIR> /simple /batch]
''' ```
''' Export the *.fna, *.faa, *.ptt file from the gbk file.
''' </summary>
'''
Public Function ExportPTTDb(_gb As String, Optional _out As String = "", Optional _simple As Boolean = False, Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.gb")
Call CLI.Append(" ")
Call CLI.Append("/gb " & """" & _gb & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _simple Then
Call CLI.Append("/simple ")
End If
If _batch Then
Call CLI.Append("/batch ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.gb.genes /gb &lt;genbank.gb> [/geneName /out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function ExportGenesFasta(_gb As String, Optional _out As String = "", Optional _genename As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.gb.genes")
Call CLI.Append(" ")
Call CLI.Append("/gb " & """" & _gb & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _genename Then
Call CLI.Append("/genename ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.gpff /in &lt;genome.gpff> /gff &lt;genome.gff> [/out &lt;out.PTT>]
''' ```
''' </summary>
'''
Public Function EXPORTgpff(_in As String, _gff As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.gpff")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/gff " & """" & _gff & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.gpffs [/in &lt;inDIR>]
''' ```
''' </summary>
'''
Public Function EXPORTgpffs(Optional _in As String = "") As Integer
Dim CLI As New StringBuilder("/Export.gpffs")
Call CLI.Append(" ")
If Not _in.StringEmpty Then
Call CLI.Append("/in " & """" & _in & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.Locus /in &lt;sbh/bbh_DIR> [/hit /out &lt;out.txt>]
''' ```
''' </summary>
'''
Public Function ExportLocus(_in As String, Optional _out As String = "", Optional _hit As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.Locus")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _hit Then
Call CLI.Append("/hit ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.Protein /gb &lt;genome.gb> [/out &lt;out.fasta>]
''' ```
''' Export all of the protein sequence from the genbank database file.
''' </summary>
'''
Public Function ExportProt(_gb As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.Protein")
Call CLI.Append(" ")
Call CLI.Append("/gb " & """" & _gb & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Fasta.Filters /in &lt;nt.fasta> /key &lt;regex/list.txt> [/tokens /out &lt;out.fasta> /p]
''' ```
''' Filter the fasta sequence subset from a larger fasta database by using the regexp for match on the fasta title.
''' </summary>
'''
Public Function Filter(_in As String, _key As String, Optional _out As String = "", Optional _tokens As Boolean = False, Optional _p As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Fasta.Filters")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/key " & """" & _key & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _tokens Then
Call CLI.Append("/tokens ")
End If
If _p Then
Call CLI.Append("/p ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Identities.Matrix /hit &lt;sbh/bbh.csv> [/out &lt;out.csv> /cut 0.65]
''' ```
''' </summary>
'''
Public Function IdentitiesMAT(_hit As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI As New StringBuilder("/Identities.Matrix")
Call CLI.Append(" ")
Call CLI.Append("/hit " & """" & _hit & """ ")
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
''' /install.cog2003-2014 /db &lt;prot2003-2014.fasta>
''' ```
''' Config the ``prot2003-2014.fasta`` database for GCModeller localblast tools. This database will be using for the COG annotation.
''' This command required of the blast+ install first.
''' </summary>
'''
Public Function InstallCOGDatabase(_db As String) As Integer
Dim CLI As New StringBuilder("/install.cog2003-2014")
Call CLI.Append(" ")
Call CLI.Append("/db " & """" & _db & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /locus.Selects /locus &lt;locus.txt> /bh &lt;bbhindex.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function LocusSelects(_locus As String, _bh As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/locus.Selects")
Call CLI.Append(" ")
Call CLI.Append("/locus " & """" & _locus & """ ")
Call CLI.Append("/bh " & """" & _bh & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /MAT.evalue /in &lt;sbh.csv> [/out &lt;mat.csv> /flip]
''' ```
''' </summary>
'''
Public Function EvalueMatrix(_in As String, Optional _out As String = "", Optional _flip As Boolean = False) As Integer
Dim CLI As New StringBuilder("/MAT.evalue")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _flip Then
Call CLI.Append("/flip ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Merge.faa /in &lt;DIR> /out &lt;out.fasta>
''' ```
''' </summary>
'''
Public Function MergeFaa(_in As String, _out As String) As Integer
Dim CLI As New StringBuilder("/Merge.faa")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Paralog /blastp &lt;blastp.txt> [/coverage 0.5 /identities 0.3 /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ExportParalog(_blastp As String, Optional _coverage As String = "", Optional _identities As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Paralog")
Call CLI.Append(" ")
Call CLI.Append("/blastp " & """" & _blastp & """ ")
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
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
''' /Print /in &lt;inDIR> [/ext &lt;ext> /out &lt;out.Csv>]
''' ```
''' </summary>
'''
Public Function Print(_in As String, Optional _ext As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Print")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _ext.StringEmpty Then
Call CLI.Append("/ext " & """" & _ext & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /query.cog2003-2014 /query &lt;query.fasta> [/evalue 1e-5 /coverage 0.65 /identities 0.85 /all /out &lt;out.DIR> /db &lt;cog2003-2014.fasta> /blast+ &lt;blast+/bin>]
''' ```
''' Protein COG annotation by using NCBI cog2003-2014.fasta database.
''' </summary>
'''
Public Function COG2003_2014(_query As String, Optional _evalue As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _out As String = "", Optional _db As String = "", Optional _blast_ As String = "", Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/query.cog2003-2014")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _evalue.StringEmpty Then
Call CLI.Append("/evalue " & """" & _evalue & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _db.StringEmpty Then
Call CLI.Append("/db " & """" & _db & """ ")
End If
If Not _blast_.StringEmpty Then
Call CLI.Append("/blast+ " & """" & _blast_ & """ ")
End If
If _all Then
Call CLI.Append("/all ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Reads.OTU.Taxonomy /in &lt;blastnMaps.csv> /OTU &lt;OTU_data.csv> /tax &lt;taxonomy:nodes/names> [/fill.empty /out &lt;out.csv>]
''' ```
''' If the blastnmapping data have the duplicated OTU tags, then this function will makes a copy of the duplicated OTU tag data. top-best data will not.
''' </summary>
'''
Public Function ReadsOTU_Taxonomy(_in As String, _OTU As String, _tax As String, Optional _out As String = "", Optional _fill_empty As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Reads.OTU.Taxonomy")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/OTU " & """" & _OTU & """ ")
Call CLI.Append("/tax " & """" & _tax & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _fill_empty Then
Call CLI.Append("/fill.empty ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /ref.acc.list /in &lt;blastnMaps.csv/DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function AccessionList(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/ref.acc.list")
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
''' /ref.gi.list /in &lt;blastnMaps.csv/DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function GiList(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/ref.gi.list")
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
''' /SBH.BBH.Batch /in &lt;sbh.DIR> [/identities &lt;-1> /coverage &lt;-1> /all /out &lt;bbh.DIR> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function SBH_BBH_Batch(_in As String, Optional _identities As String = "", Optional _coverage As String = "", Optional _out As String = "", Optional _num_threads As String = "", Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SBH.BBH.Batch")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If _all Then
Call CLI.Append("/all ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /SBH.Export.Large /in &lt;blastp_out.txt> [/trim-kegg /out &lt;sbh.csv> /identities 0.15 /coverage 0.5]
''' ```
''' Using this command for export the sbh result of your blastp raw data.
''' </summary>
'''
Public Function ExportBBHLarge(_in As String, Optional _out As String = "", Optional _identities As String = "", Optional _coverage As String = "", Optional _trim_kegg As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SBH.Export.Large")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If _trim_kegg Then
Call CLI.Append("/trim-kegg ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /SBH.Trim /in &lt;sbh.csv> /evalue &lt;evalue> [/identities 0.15 /coverage 0.5 /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function SBHTrim(_in As String, _evalue As String, Optional _identities As String = "", Optional _coverage As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/SBH.Trim")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/evalue " & """" & _evalue & """ ")
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /sbh2bbh /qvs &lt;qvs.sbh.csv> /svq &lt;svq.sbh.csv> [/trim /identities &lt;-1> /coverage &lt;-1> /all /out &lt;bbh.csv>]
''' ```
''' Export bbh result from the sbh pairs.
''' </summary>
'''
Public Function BBHExport2(_qvs As String, _svq As String, Optional _identities As String = "", Optional _coverage As String = "", Optional _out As String = "", Optional _trim As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/sbh2bbh")
Call CLI.Append(" ")
Call CLI.Append("/qvs " & """" & _qvs & """ ")
Call CLI.Append("/svq " & """" & _svq & """ ")
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _trim Then
Call CLI.Append("/trim ")
End If
If _all Then
Call CLI.Append("/all ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Select.Meta /in &lt;meta.Xml> /bbh &lt;bbh.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function SelectsMeta(_in As String, _bbh As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Select.Meta")
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
''' /SSBH2BH_LDM /in &lt;ssbh.csv> [/xml /coverage 0.8 /identities 0.3 /out &lt;out.xml>]
''' ```
''' </summary>
'''
Public Function KEGGSSOrtholog2Bh(_in As String, Optional _coverage As String = "", Optional _identities As String = "", Optional _out As String = "", Optional _xml As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SSBH2BH_LDM")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _xml Then
Call CLI.Append("/xml ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /SSDB.Export /in &lt;inDIR> [/coverage 0.8 /identities 0.3 /out &lt;out.Xml>]
''' ```
''' </summary>
'''
Public Function KEGGSSDBExport(_in As String, Optional _coverage As String = "", Optional _identities As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/SSDB.Export")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
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
''' /Taxonomy.efetch /in &lt;nt.fasta> [/out &lt;out.DIR>]
''' ```
''' Fetch the taxonomy information of the fasta sequence from NCBI web server.
''' </summary>
'''
Public Function FetchTaxnData(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Taxonomy.efetch")
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
''' /Taxonomy.efetch.Merge /in &lt;in.DIR> [/out &lt;out.Csv>]
''' ```
''' </summary>
'''
Public Function MergeFetchTaxonData(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Taxonomy.efetch.Merge")
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
''' /venn.BBH /imports &lt;blastp_out.DIR> [/skip-load /query &lt;queryName> /all /coverage &lt;0.6> /identities &lt;0.3> /out &lt;outDIR>]
''' ```
''' 2. Build venn table And bbh data from the blastp result out Or sbh data cache.
''' </summary>
'''
Public Function VennBBH(_imports As String, Optional _query As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _out As String = "", Optional _skip_load As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/venn.BBH")
Call CLI.Append(" ")
Call CLI.Append("/imports " & """" & _imports & """ ")
If Not _query.StringEmpty Then
Call CLI.Append("/query " & """" & _query & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _skip_load Then
Call CLI.Append("/skip-load ")
End If
If _all Then
Call CLI.Append("/all ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /venn.BlastAll /query &lt;queryDIR> [/out &lt;outDIR> /num_threads &lt;-1> /evalue 10 /overrides /all /coverage &lt;0.8> /identities &lt;0.3>]
''' ```
''' Completely paired combos blastp bbh operations for the venn diagram Or network builder.
''' </summary>
'''
Public Function vennBlastAll(_query As String, Optional _out As String = "", Optional _num_threads As String = "", Optional _evalue As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _overrides As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/venn.BlastAll")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If Not _evalue.StringEmpty Then
Call CLI.Append("/evalue " & """" & _evalue & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If _overrides Then
Call CLI.Append("/overrides ")
End If
If _all Then
Call CLI.Append("/all ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /venn.cache /imports &lt;blastp.DIR> [/out &lt;sbh.out.DIR> /coverage &lt;0.6> /identities &lt;0.3> /num_threads &lt;-1> /overrides]
''' ```
''' 1. [SBH_Batch] Creates the sbh cache data for the downstream bbh analysis.
''' And this batch function is suitable with any scale of the blastp sbh data output.
''' </summary>
'''
Public Function VennCache(_imports As String, Optional _out As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _num_threads As String = "", Optional _overrides As Boolean = False) As Integer
Dim CLI As New StringBuilder("/venn.cache")
Call CLI.Append(" ")
Call CLI.Append("/imports " & """" & _imports & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If _overrides Then
Call CLI.Append("/overrides ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /venn.sbh.thread /in &lt;blastp.txt> [/out &lt;out.sbh.csv> /coverage &lt;0.6> /identities &lt;0.3> /overrides]
''' ```
''' </summary>
'''
Public Function SBHThread(_in As String, Optional _out As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _overrides As Boolean = False) As Integer
Dim CLI As New StringBuilder("/venn.sbh.thread")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If _overrides Then
Call CLI.Append("/overrides ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Venn.Single /in &lt;besthits.Xml> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function VennSingle(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Venn.Single")
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
''' /Whog.XML /in &lt;whog> [/out &lt;whog.XML>]
''' ```
''' Converts the whog text file into a XML data file.
''' </summary>
'''
Public Function WhogXML(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Whog.XML")
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
''' --bbh.export /in &lt;blast_out.DIR> [/all /out &lt;out.DIR> /single-query &lt;queryName> /coverage &lt;0.5> /identities 0.15]
''' ```
''' Batch export bbh result data from a directory.
''' </summary>
'''
Public Function ExportBBH(_in As String, Optional _out As String = "", Optional _single_query As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("--bbh.export")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _single_query.StringEmpty Then
Call CLI.Append("/single-query " & """" & _single_query & """ ")
End If
If Not _coverage.StringEmpty Then
Call CLI.Append("/coverage " & """" & _coverage & """ ")
End If
If Not _identities.StringEmpty Then
Call CLI.Append("/identities " & """" & _identities & """ ")
End If
If _all Then
Call CLI.Append("/all ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' blast -i &lt;file_directory> -blast_bin &lt;BLAST_program_directory> -program &lt;program_type_name> [-ld &lt;log_dir> -xld &lt;xml_log_dir>]
''' ```
''' In order to draw as venn diagram for a specific set of genome and study the diferrence and consists between these genomes, you should do the blast operation from the protein amino aciad sequence first. The blastp operation can be performenced by the blast+ program which you can download from the NCBI website, this command is a interop service for the NCBI blast program, you should install the blast+ program at first.
''' </summary>
'''
Public Function BLASTA(_i As String, _blast_bin As String, _program As String, Optional _ld As String = "", Optional _xld As String = "") As Integer
Dim CLI As New StringBuilder("blast")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-blast_bin " & """" & _blast_bin & """ ")
Call CLI.Append("-program " & """" & _program & """ ")
If Not _ld.StringEmpty Then
Call CLI.Append("-ld " & """" & _ld & """ ")
End If
If Not _xld.StringEmpty Then
Call CLI.Append("-xld " & """" & _xld & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --blast.self /query &lt;query.fasta> [/blast &lt;blast_HOME> /out &lt;out.csv>]
''' ```
''' Query fasta query against itself for paralogs.
''' </summary>
'''
Public Function SelfBlast(_query As String, Optional _blast As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--blast.self")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _blast.StringEmpty Then
Call CLI.Append("/blast " & """" & _blast & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' -copy -i &lt;index_file> -os &lt;output_saved> [-osidx &lt;id_column_index> -os_skip_first &lt;T/F>]
''' ```
''' </summary>
'''
Public Function Copy(_i As String, _os As String, Optional _osidx As String = "", Optional _os_skip_first As String = "") As Integer
Dim CLI As New StringBuilder("-copy")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-os " & """" & _os & """ ")
If Not _osidx.StringEmpty Then
Call CLI.Append("-osidx " & """" & _osidx & """ ")
End If
If Not _os_skip_first.StringEmpty Then
Call CLI.Append("-os_skip_first " & """" & _os_skip_first & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --Export.Fasta /hits &lt;query-hits.csv> /query &lt;query.fasta> /subject &lt;subject.fasta>
''' ```
''' </summary>
'''
Public Function ExportFasta(_hits As String, _query As String, _subject As String) As Integer
Dim CLI As New StringBuilder("--Export.Fasta")
Call CLI.Append(" ")
Call CLI.Append("/hits " & """" & _hits & """ ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --Export.Overviews /blast &lt;blastout.txt> [/out &lt;overview.csv>]
''' ```
''' </summary>
'''
Public Function ExportOverviews(_blast As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--Export.Overviews")
Call CLI.Append(" ")
Call CLI.Append("/blast " & """" & _blast & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --Export.SBH /in &lt;in.DIR> /prefix &lt;queryName> /out &lt;out.csv> [/txt]
''' ```
''' </summary>
'''
Public Function ExportSBH(_in As String, _prefix As String, _out As String, Optional _txt As Boolean = False) As Integer
Dim CLI As New StringBuilder("--Export.SBH")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/prefix " & """" & _prefix & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If _txt Then
Call CLI.Append("/txt ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' -export_besthit -i &lt;input_csv_file> -o &lt;output_saved_csv>
''' ```
''' </summary>
'''
Public Function ExportBestHit(_i As String, _o As String) As Integer
Dim CLI As New StringBuilder("-export_besthit")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' grep -i &lt;xml_log_file> -q &lt;script_statements> -h &lt;script_statements>
''' ```
''' The gene id in the blast output log file are not well format for reading and program processing, so before you generate the venn diagram you should call this command to parse the gene id from the log file. You can also done this id parsing job using other tools.
''' </summary>
'''
Public Function Grep(_i As String, _q As String, _h As String) As Integer
Dim CLI As New StringBuilder("grep")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-q " & """" & _q & """ ")
Call CLI.Append("-h " & """" & _h & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' logs_analysis -d &lt;xml_logs_directory> -export &lt;export_csv_file>
''' ```
''' Parsing the xml format blast log into a csv data file that use for venn diagram drawing.
''' </summary>
'''
Public Function bLogAnalysis(_d As String, _export As String) As Integer
Dim CLI As New StringBuilder("logs_analysis")
Call CLI.Append(" ")
Call CLI.Append("-d " & """" & _d & """ ")
Call CLI.Append("-export " & """" & _export & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' merge -d &lt;directory> -o &lt;output_file>
''' ```
''' This program can not use the blast parsing result for the venn diagram drawing operation, and this command is using for generate the drawing data for the venn diagram drawing command, this command merge the blast log parsing result and then using the parsing result for drawing a venn diagram.
''' </summary>
'''
Public Function Merge(_d As String, _o As String) As Integer
Dim CLI As New StringBuilder("merge")
Call CLI.Append(" ")
Call CLI.Append("-d " & """" & _d & """ ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' -merge_besthit -i &lt;input_file_list> -o &lt;output_file> -os &lt;original_idlist_sequence_file> [-osidx &lt;id_column_index> -os_skip_first &lt;T/F>]
''' ```
''' </summary>
'''
Public Function MergeBestHits(_i As String, _o As String, _os As String, Optional _osidx As String = "", Optional _os_skip_first As String = "") As Integer
Dim CLI As New StringBuilder("-merge_besthit")
Call CLI.Append(" ")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-o " & """" & _o & """ ")
Call CLI.Append("-os " & """" & _os & """ ")
If Not _osidx.StringEmpty Then
Call CLI.Append("-osidx " & """" & _osidx & """ ")
End If
If Not _os_skip_first.StringEmpty Then
Call CLI.Append("-os_skip_first " & """" & _os_skip_first & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --Xml2Excel /in &lt;in.xml> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function XmlToExcel(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--Xml2Excel")
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
''' --Xml2Excel.Batch /in &lt;inDIR> [/out &lt;outDIR> /Merge]
''' ```
''' </summary>
'''
Public Function XmlToExcelBatch(_in As String, Optional _out As String = "", Optional _merge As Boolean = False) As Integer
Dim CLI As New StringBuilder("--Xml2Excel.Batch")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _merge Then
Call CLI.Append("/merge ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
