Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/localblast.exe

Namespace GCModellerApps


''' <summary>
'''Wrapper tools for the ncbi blast+ program and the blast output data analysis program. 
'''                  For running a large scale parallel alignment task, using ``/venn.BlastAll`` command for ``blastp`` and ``/blastn.Query.All`` command for ``blastn``.
''' </summary>
'''
Public Class localblast : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
'''Add locus_tag qualifier into the feature slot.
''' </summary>
'''
Public Function add_locus_tag(_gb As String, _prefix As String, Optional _out As String = "", Optional _add_gene As Boolean = False) As Integer
Dim CLI As New StringBuilder("/add.locus_tag")
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
'''
''' </summary>
'''
Public Function add_names(_anno As String, _gb As String, Optional _out As String = "", Optional _tag As String = "") As Integer
Dim CLI As New StringBuilder("/add.names")
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
'''
''' </summary>
'''
Public Function AlignmentTable_TopBest(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/AlignmentTable.TopBest")
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
Public Function Bash_Venn(_blast As String, _inDIR As String, _inRef As String, Optional _out As String = "", Optional _evalue As String = "") As Integer
Dim CLI As New StringBuilder("/Bash.Venn")
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
'''Export bbh mapping result from the blastp raw output.
''' </summary>
'''
Public Function bbh_EXPORT(_query As String, _subject As String, Optional _out As String = "", Optional _evalue As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("/bbh.EXPORT")
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
'''
''' </summary>
'''
Public Function BBH_Merge(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BBH.Merge")
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
Public Function BestHits_Filtering(_in As String, _sp As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BestHits.Filtering")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/sp " & """" & _sp & """ ")
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
Public Function Blastn_Maps_Taxid(_in As String, _2taxid As String, Optional _tax As String = "", Optional _out As String = "", Optional _gi2taxid As Boolean = False, Optional _trim As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Blastn.Maps.Taxid")
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
'''Using target fasta sequence query against all of the fasta sequence in target direcotry. This function is single thread.
''' </summary>
'''
Public Function blastn_Query(_query As String, _db As String, Optional _evalue As String = "", Optional _word_size As String = "", Optional _out As String = "", Optional _thread As Boolean = False) As Integer
Dim CLI As New StringBuilder("/blastn.Query")
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
'''Using the fasta sequence in a directory query against all of the sequence in another directory.
''' </summary>
'''
Public Function blastn_Query_All(_query As String, _db As String, Optional _evalue As String = "", Optional _word_size As String = "", Optional _out As String = "", Optional _penalty As String = "", Optional _reward As String = "", Optional _skip_format As Boolean = False, Optional _parallel As Boolean = False) As Integer
Dim CLI As New StringBuilder("/blastn.Query.All")
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
'''
''' </summary>
'''
Public Function BlastnMaps_Match_Taxid(_in As String, _acc2taxid As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BlastnMaps.Match.Taxid")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/acc2taxid " & """" & _acc2taxid & """ ")
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
Public Function BlastnMaps_Select(_in As String, _data As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BlastnMaps.Select")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/data " & """" & _data & """ ")
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
Public Function BlastnMaps_Select_Top(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BlastnMaps.Select.Top")
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
Public Function BlastnMaps_Summery(_in As String, Optional _split As String = "-", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BlastnMaps.Summery")
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
'''Using query fasta invoke blastp against the fasta files in a directory.
'''               * This command tools required of NCBI blast+ suite, you must config the blast bin path by using ``settings.exe`` before running this command.
''' </summary>
'''
Public Function Blastp_BBH_Query(_query As String, _hit As String, Optional _out As String = "", Optional _num_threads As String = "", Optional _overrides As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Blastp.BBH.Query")
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
'''
''' </summary>
'''
Public Function Chromosomes_Export(_reads As String, _maps As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Chromosomes.Export")
Call CLI.Append("/reads " & """" & _reads & """ ")
Call CLI.Append("/maps " & """" & _maps & """ ")
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
Public Function COG_myva(_blastp As String, _whog As String, Optional _out As String = "", Optional _simple As Boolean = False) As Integer
Dim CLI As New StringBuilder("/COG.myva")
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
'''
''' </summary>
'''
Public Function COG_Statics(_in As String, Optional _locus As String = "", Optional _locumap As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/COG.Statics")
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
'''
''' </summary>
'''
Public Function COG2014_result(_sbh As String, _cog As String, Optional _cog_names As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/COG2014.result")
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
'''Copy target type files from different sub directory into a directory.
''' </summary>
'''
Public Function Copy_Fasta(_imports As String, Optional _type As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Copy.Fasta")
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
'''
''' </summary>
'''
Public Function Copy_PTT(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Copy.PTT")
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
Public Function Copys(_imports As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Copys")
Call CLI.Append("/imports " & """" & _imports & """ ")
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
Public Function Export_AlignmentTable(_in As String, Optional _out As String = "", Optional _split As Boolean = False, Optional _header_split As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.AlignmentTable")
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
'''
''' </summary>
'''
Public Function Export_AlignmentTable_giList(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.AlignmentTable.giList")
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
Public Function Export_Blastn(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.Blastn")
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
Public Function Export_blastnMaps(_in As String, Optional _out As String = "", Optional _best As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.blastnMaps")
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
'''Multiple processor task.
''' </summary>
'''
Public Function Export_blastnMaps_Batch(_in As String, Optional _out As String = "", Optional _num_threads As String = "", Optional _best As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.blastnMaps.Batch")
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
'''
''' </summary>
'''
Public Function Export_blastnMaps_littles(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.blastnMaps.littles")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Exports large amount of blastn output files and write all data into a specific csv file.
''' </summary>
'''
Public Function Export_blastnMaps_Write(_in As String, Optional _out As String = "", Optional _best As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.blastnMaps.Write")
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
'''
''' </summary>
'''
Public Function Export_BlastX(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.BlastX")
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
Public Function EXPORT_COGs_from_DOOR(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/EXPORT.COGs.from.DOOR")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Export the *.fna, *.faa, *.ptt file from the gbk file.
''' </summary>
'''
Public Function Export_gb(_gb As String, Optional _out As String = "", Optional _simple As Boolean = False, Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.gb")
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
'''
''' </summary>
'''
Public Function Export_gb_genes(_gb As String, Optional _out As String = "", Optional _genename As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.gb.genes")
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
'''
''' </summary>
'''
Public Function Export_gpff(_in As String, _gff As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.gpff")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/gff " & """" & _gff & """ ")
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
Public Function Export_gpffs(Optional _in As String = "") As Integer
Dim CLI As New StringBuilder("/Export.gpffs")
If Not _in.StringEmpty Then
Call CLI.Append("/in " & """" & _in & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Export_Locus(_in As String, Optional _out As String = "", Optional _hit As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.Locus")
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
'''Export all of the protein sequence from the genbank database file.
''' </summary>
'''
Public Function Export_Protein(_gb As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.Protein")
Call CLI.Append("/gb " & """" & _gb & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Filter the fasta sequence subset from a larger fasta database by using the regexp for match on the fasta title.
''' </summary>
'''
Public Function Fasta_Filters(_in As String, _key As String, Optional _out As String = "", Optional _tokens As Boolean = False, Optional _p As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Fasta.Filters")
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
'''
''' </summary>
'''
Public Function Identities_Matrix(_hit As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI As New StringBuilder("/Identities.Matrix")
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
'''Config the ``prot2003-2014.fasta`` database for GCModeller localblast tools. This database will be using for the COG annotation. 
'''               This command required of the blast+ install first.
''' </summary>
'''
Public Function install_cog2003_2014(_db As String) As Integer
Dim CLI As New StringBuilder("/install.cog2003-2014")
Call CLI.Append("/db " & """" & _db & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function locus_Selects(_locus As String, _bh As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/locus.Selects")
Call CLI.Append("/locus " & """" & _locus & """ ")
Call CLI.Append("/bh " & """" & _bh & """ ")
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
Public Function MAT_evalue(_in As String, Optional _out As String = "", Optional _flip As Boolean = False) As Integer
Dim CLI As New StringBuilder("/MAT.evalue")
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
'''
''' </summary>
'''
Public Function Merge_faa(_in As String, _out As String) As Integer
Dim CLI As New StringBuilder("/Merge.faa")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Paralog(_blastp As String, Optional _coverage As String = "", Optional _identities As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Paralog")
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
'''
''' </summary>
'''
Public Function Print(_in As String, Optional _ext As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Print")
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
'''Protein COG annotation by using NCBI cog2003-2014.fasta database.
''' </summary>
'''
Public Function query_cog2003_2014(_query As String, Optional _evalue As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _out As String = "", Optional _db As String = "", Optional _blast_ As String = "", Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/query.cog2003-2014")
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
'''If the blastnmapping data have the duplicated OTU tags, then this function will makes a copy of the duplicated OTU tag data. top-best data will not.
''' </summary>
'''
Public Function Reads_OTU_Taxonomy(_in As String, _OTU As String, _tax As String, Optional _out As String = "", Optional _fill_empty As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Reads.OTU.Taxonomy")
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
'''
''' </summary>
'''
Public Function ref_acc_list(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/ref.acc.list")
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
Public Function ref_gi_list(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/ref.gi.list")
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
Public Function SBH_BBH_Batch(_in As String, Optional _identities As String = "", Optional _coverage As String = "", Optional _out As String = "", Optional _num_threads As String = "", Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SBH.BBH.Batch")
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
'''Using this command for export the sbh result of your blastp raw data.
''' </summary>
'''
Public Function SBH_Export_Large(_in As String, Optional _out As String = "", Optional _identities As String = "", Optional _coverage As String = "", Optional _trim_kegg As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SBH.Export.Large")
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
'''
''' </summary>
'''
Public Function SBH_Trim(_in As String, _evalue As String, Optional _identities As String = "", Optional _coverage As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/SBH.Trim")
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
'''Export bbh result from the sbh pairs.
''' </summary>
'''
Public Function sbh2bbh(_qvs As String, _svq As String, Optional _identities As String = "", Optional _coverage As String = "", Optional _out As String = "", Optional _trim As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/sbh2bbh")
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
'''
''' </summary>
'''
Public Function Select_Meta(_in As String, _bbh As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Select.Meta")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
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
Public Function SSBH2BH_LDM(_in As String, Optional _coverage As String = "", Optional _identities As String = "", Optional _out As String = "", Optional _xml As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SSBH2BH_LDM")
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
'''
''' </summary>
'''
Public Function SSDB_Export(_in As String, Optional _coverage As String = "", Optional _identities As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/SSDB.Export")
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
'''Fetch the taxonomy information of the fasta sequence from NCBI web server.
''' </summary>
'''
Public Function Taxonomy_efetch(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Taxonomy.efetch")
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
Public Function Taxonomy_efetch_Merge(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Taxonomy.efetch.Merge")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''2. Build venn table And bbh data from the blastp result out Or sbh data cache.
''' </summary>
'''
Public Function venn_BBH(_imports As String, Optional _query As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _out As String = "", Optional _skip_load As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/venn.BBH")
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
'''Completely paired combos blastp bbh operations for the venn diagram Or network builder.
''' </summary>
'''
Public Function venn_BlastAll(_query As String, Optional _out As String = "", Optional _num_threads As String = "", Optional _evalue As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _overrides As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/venn.BlastAll")
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
'''1. [SBH_Batch] Creates the sbh cache data for the downstream bbh analysis. 
'''               And this batch function is suitable with any scale of the blastp sbh data output.
''' </summary>
'''
Public Function venn_cache(_imports As String, Optional _out As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _num_threads As String = "", Optional _overrides As Boolean = False) As Integer
Dim CLI As New StringBuilder("/venn.cache")
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
'''
''' </summary>
'''
Public Function venn_sbh_thread(_in As String, Optional _out As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _overrides As Boolean = False) As Integer
Dim CLI As New StringBuilder("/venn.sbh.thread")
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
'''
''' </summary>
'''
Public Function Venn_Single(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Venn.Single")
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
Public Function Whog_XML(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Whog.XML")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Batch export bbh result data from a directory.
''' </summary>
'''
Public Function bbh_export(_in As String, Optional _out As String = "", Optional _single_query As String = "", Optional _coverage As String = "", Optional _identities As String = "", Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("--bbh.export")
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
'''In order to draw as venn diagram for a specific set of genome and study the diferrence and consists between these genomes, you should do the blast operation from the protein amino aciad sequence first. The blastp operation can be performenced by the blast+ program which you can download from the NCBI website, this command is a interop service for the NCBI blast program, you should install the blast+ program at first.
''' </summary>
'''
Public Function blast(_i As String, _blast_bin As String, _program As String, Optional _ld As String = "", Optional _xld As String = "") As Integer
Dim CLI As New StringBuilder("blast")
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
'''Query fasta query against itself for paralogs.
''' </summary>
'''
Public Function blast_self(_query As String, Optional _blast As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--blast.self")
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
'''
''' </summary>
'''
Public Function copy(_i As String, _os As String, Optional _osidx As String = "", Optional _os_skip_first As String = "") As Integer
Dim CLI As New StringBuilder("-copy")
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
'''
''' </summary>
'''
Public Function Export_Fasta(_hits As String, _query As String, _subject As String) As Integer
Dim CLI As New StringBuilder("--Export.Fasta")
Call CLI.Append("/hits " & """" & _hits & """ ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Export_Overviews(_blast As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--Export.Overviews")
Call CLI.Append("/blast " & """" & _blast & """ ")
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
Public Function Export_SBH(_in As String, _prefix As String, _out As String, Optional _txt As Boolean = False) As Integer
Dim CLI As New StringBuilder("--Export.SBH")
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
'''
''' </summary>
'''
Public Function export_besthit(_i As String, _o As String) As Integer
Dim CLI As New StringBuilder("-export_besthit")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''The gene id in the blast output log file are not well format for reading and program processing, so before you generate the venn diagram you should call this command to parse the gene id from the log file. You can also done this id parsing job using other tools.
''' </summary>
'''
Public Function grep(_i As String, _q As String, _h As String) As Integer
Dim CLI As New StringBuilder("grep")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-q " & """" & _q & """ ")
Call CLI.Append("-h " & """" & _h & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Parsing the xml format blast log into a csv data file that use for venn diagram drawing.
''' </summary>
'''
Public Function logs_analysis(_d As String, _export As String) As Integer
Dim CLI As New StringBuilder("logs_analysis")
Call CLI.Append("-d " & """" & _d & """ ")
Call CLI.Append("-export " & """" & _export & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''This program can not use the blast parsing result for the venn diagram drawing operation, and this command is using for generate the drawing data for the venn diagram drawing command, this command merge the blast log parsing result and then using the parsing result for drawing a venn diagram.
''' </summary>
'''
Public Function merge(_d As String, _o As String) As Integer
Dim CLI As New StringBuilder("merge")
Call CLI.Append("-d " & """" & _d & """ ")
Call CLI.Append("-o " & """" & _o & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function merge_besthit(_i As String, _o As String, _os As String, Optional _osidx As String = "", Optional _os_skip_first As String = "") As Integer
Dim CLI As New StringBuilder("-merge_besthit")
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
'''
''' </summary>
'''
Public Function Xml2Excel(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--Xml2Excel")
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
Public Function Xml2Excel_Batch(_in As String, Optional _out As String = "", Optional _merge As Boolean = False) As Integer
Dim CLI As New StringBuilder("--Xml2Excel.Batch")
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
